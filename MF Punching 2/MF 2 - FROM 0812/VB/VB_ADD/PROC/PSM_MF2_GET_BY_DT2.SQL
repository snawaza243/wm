CREATE OR REPLACE PROCEDURE process_mf_transaction_doc(
    P_INDEX         IN VARCHAR2,
    p_common_id     IN VARCHAR2,
    p_login_id      IN VARCHAR2,
    P_ROLE_ID       IN VARCHAR2,
    P_CHK_SWITCH    IN VARCHAR2,
    p_cursor        OUT SYS_REFCURSOR,
    p_message       OUT VARCHAR2
)
AS
    v_sch_code VARCHAR2(50);
    v_inv_code VARCHAR2(50);
    v_busi_rm_code VARCHAR2(50);
    v_busi_branch_code VARCHAR2(50);
    v_busi_tr_date DATE;
    v_expense NUMBER;
    v_branch_name VARCHAR2(100);
    v_sch_name VARCHAR2(200);
    v_mut_name VARCHAR2(100);
    v_mut_code VARCHAR2(100);

    v_investor_name VARCHAR2(200);
    v_pan VARCHAR2(20);
    v_nature VARCHAR2(1);
    v_approval_flag VARCHAR2(1);
    v_cross_chanel_count NUMBER;
    v_logged_userid VARCHAR2(50);
    v_main_code VARCHAR2(50);
    v_upd_proc VARCHAR2(1);
    v_atm_scheme BOOLEAN;
    v_rm_name VARCHAR2(100);

    v_expense_per VARCHAR2(10);
    v_expense_rs VARCHAR2(20);
    v_my_nat VARCHAR2(1);
    v_chk_atm_tr VARCHAR2(1);



    vm_sch_code VARCHAR2(50);
    vm_inv_code VARCHAR2(50);
    vm_busi_rm_code VARCHAR2(50);
    vm_busi_branch_code VARCHAR2(50);
    vm_busi_tr_date DATE;
    vm_expense NUMBER;
    vm_branch_name VARCHAR2(100);
    vm_sch_name VARCHAR2(200);
    vm_mut_name VARCHAR2(100);
    vm_investor_name VARCHAR2(200);
    vm_pan VARCHAR2(20);
    vm_nature VARCHAR2(1);
    vm_approval_flag VARCHAR2(1);
    vm_cross_chanel_count NUMBER;
    vm_logged_userid VARCHAR2(50);
    vm_main_code VARCHAR2(50);
    vm_upd_proc VARCHAR2(1);
    vm_atm_scheme BOOLEAN;
    vm_rm_name VARCHAR2(100);
    vm_chk_atm_tr VARCHAR2(1);
    vm_switch_amc VARCHAR2(100);
    vm_switch_sch VARCHAR2(100);
    vm_switch_amc_code VARCHAR2(100);
    vm_switch_sch_code VARCHAR2(100);


BEGIN
    -- Initialize message
    p_message := 'PROCESS_COMPLETE';
    
    -- Open cursor with all required fields
    OPEN p_cursor FOR
    SELECT 
        NULL as sch_code,
        NULL as sch_name,
        NULL as mut_name,
        NULL as nature,
        NULL as inv_code,
        NULL as investor_name,
        NULL as pan,
        NULL as busi_rm_code,
        NULL as busi_branch_code,
        NULL as busi_tr_date,
        NULL as expense,
        NULL as branch_name,
        NULL as logged_userid,
        NULL as main_code,
        NULL as rm_name,
        NULL as atm_scheme,
        'DATA_WILL_BE_POPULATED' as status_message
    FROM dual WHERE 1 = 0;

    -- Get basic document information
    BEGIN
        SELECT a.busi_rm_code, a.busi_branch_code, a.busi_tr_date, 
               a.sch_code, a.inv_code, b.branch_name, a.expense
        INTO v_busi_rm_code, v_busi_branch_code, v_busi_tr_date,
             v_sch_code, v_inv_code, v_branch_name, v_expense
        FROM tb_doc_upload a
        JOIN branch_master b ON a.busi_branch_code = b.branch_code
        WHERE a.common_id = p_common_id 
          AND a.tran_type = 'MF' 
          AND a.verification_flag = '1' 
          AND a.rejection_status = '0'  
          AND a.punching_flag = '0'
          AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            p_message := 'ERROR: Document not found or already processed';
            RETURN;
    END;

    IF RECORD%COUNT > 0 THEN
        
        
        -- Check approval status
        BEGIN
            SELECT wealthmaker.fn_check_for_approval_all(p_common_id)
            INTO v_approval_flag
            FROM dual;
            
            IF v_approval_flag = '2' THEN
                p_message := 'ERROR: Approval request already raised';
                RETURN;
            ELSIF v_approval_flag = '4' THEN
                p_message := 'ERROR: Approval request rejected by management';
                RETURN;
            END IF;
        END;



        -- Get scheme information
        IF v_sch_code IS NOT NULL THEN
            BEGIN
                SELECT sch_code, sch_name, mut_name, nature, atm_scheme
                INTO v_sch_code, v_sch_name, v_mut_name, v_nature, v_atm_scheme
                FROM (
                    SELECT osch_code as sch_code, osch_name as sch_name, iss_name as mut_name, 'O' as nature, (CASE WHEN EXISTS (SELECT 1 FROM atm_schemes WHERE scheme_code = osch_code) THEN 1 ELSE 0 END) as atm_scheme
                    FROM other_product o, product_master p, iss_master i 
                    WHERE osch_code = v_sch_code 
                    AND o.prod_class_code = p.prod_code 
                    AND o.iss_code = i.iss_code 
                    AND (i.branch_code <> 'N' OR i.branch_code IS NULL) 
                    AND (o.flag <> 'N' OR o.flag IS NULL)
                    
                    UNION ALL
                    
                    SELECT sch_code, sch_name, mut_name, 'C' as nature, (CASE WHEN EXISTS (SELECT 1 FROM atm_schemes WHERE scheme_code = sch_code) THEN 1 ELSE 0 END) as atm_scheme
                    FROM scheme_info s, mut_fund m 
                    WHERE s.sch_code = v_sch_code 
                    AND s.mut_code = m.mut_code 
                    AND m.mut_code NOT IN ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020')
                    AND s.sch_code IN (SELECT sch_code FROM mp_scheme_master WHERE active_status = 'Active')
                ) WHERE ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    p_message := 'WARNING: Scheme information not found';
            END;

        END IF;

        IF P_INDEX = '0' THEN

            DECLARE
                vt_chek_sch_nature VARCHAR2(1);
            BEGIN
                SELECT NVL(get_scheme_nature(v_sch_code), '') AS a
                INTO vt_chek_sch_nature
                FROM dual;

                IF vt_chek_sch_nature = 'O' THEN
                    V_EXPENSE_PER := 'Expenses agnst. Trail%';
                    V_EXPENSE_RS  := 'Expenses agnst. Trail Rs.';
                    V_MyNat      := 'O';
                ELSE
                    V_EXPENSE_PER := 'Expenses Trail%';
                    V_EXPENSE_RS  := 'Expenses Trail Rs.';
                    V_MyNat      := 'C';
                END IF;
            END;

            DECLARE
            vt_chk_atm_tr NUMBER;

            BEGIN
                IF P_CHK_SWITCH = '0' THEN 
                    v_mut_name := v_mut_name;
                    v_sch_name := v_sch_name;

                    --If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                    select nvl(count(*),0) into vt_chk_atm_tr 
                    from reliance_atm_master where 
                    sch_code = v_sch_code and from_dt<=sysdate and (to_dt>=sysdate or to_dt is null);

                    IF vt_chk_atm_tr >0 THEN
                        v_chk_atm_tr := '1';
                    ELSE
                        v_chk_atm_tr := '0';
                    END IF;
                ELSE
                    DECLARE 
                        VA_SWITCH_AMC VARCHAR2(50);
                        VA_SWITCH_AMC_CODE VARCHAR2(50);
                        VA_SWITCH_SCH VARCHAR2(50);
                        VA_SWITCH_SCH_CODE VARCHAR2(50);
                    BEGIN
                        VA_SWITCH_AMC := V_MUT_NAME;
                        VA_SWITCH_SCH := V_SCH_NAME;
                        VA_SWITCH_AMC_CODE := V_MUT_CODE;
                        VA_SWITCH_SCH_CODE := V_SCH_CODE;
                    END;
                END IF;

            END;
        ELSE -- 1 : MODIFICATION

            DECLARE
                vt_chek_sch_nature VARCHAR2(1);
            BEGIN
                SELECT NVL(get_scheme_nature(v_sch_code), '') AS a
                INTO vt_chek_sch_nature
                FROM dual;

                IF vt_chek_sch_nature = 'O' THEN
                    V_EXPENSE_PER := 'Expenses agnst. Trail%';
                    V_EXPENSE_RS  := 'Expenses agnst. Trail Rs.';
                    V_MyNat      := 'O';
                ELSE
                    V_EXPENSE_PER := 'Expenses Trail%';
                    V_EXPENSE_RS  := 'Expenses Trail Rs.';
                    V_MyNat      := 'C';
                END IF;
            END;

            DECLARE
            vt_chk_atm_tr NUMBER;

            BEGIN
                IF P_CHK_SWITCH = '0' THEN 
                    v_mut_name := v_mut_name;
                    vm_sch_name := v_sch_name;
                    vm_sch_code := v_sch_code;

                    --If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                    select nvl(count(*),0) into vt_chk_atm_tr 
                    from reliance_atm_master where 
                    sch_code = v_sch_code and from_dt<=sysdate and (to_dt>=sysdate or to_dt is null);

                    IF vt_chk_atm_tr >0 THEN
                        vm_chk_atm_tr := '1';
                    ELSE
                        vm_chk_atm_tr := '0';
                    END IF;
                ELSE
                    vm_switch_amc := v_mut_name;
                    vm_switch_sch := v_sch_name;
                    vm_switch_amc_code := v_mut_code;
                    vm_switch_sch_code := v_sch_code;
                END IF;

            END;
        ELSE
            v_mut_code := NULL;
            vm_switch_amc := NULL;
            vm_switch_sch := NULL;
            vm_switch_amc_code := NULL;
            vm_switch_sch_code := NULL;

        END IF;

        -- Get investor information
        IF v_inv_code IS NOT NULL THEN
            BEGIN
                SELECT i.INVESTOR_name, i.pan, e.rm_name
                INTO v_investor_name, v_pan, v_rm_name
                FROM investor_master i
                JOIN employee_master e ON i.RM_CODE = e.RM_CODE
                WHERE i.inv_code = v_inv_code
                AND i.INVESTOR_name IS NOT NULL
                AND ROWNUM = 1;
                
                -- Get additional client info
                BEGIN
                    SELECT loggeduserid, main_code, NVL(UPD_PROC,'N')
                    INTO v_logged_userid, v_main_code, v_upd_proc
                    FROM client_test 
                    WHERE client_codekyc = v_inv_code
                    AND ROWNUM = 1;
                    
                    -- Check for mandatory information
                    IF v_logged_userid = 'PROC' AND SUBSTR(v_inv_code, 1, 1) <> '3' THEN
                        IF v_upd_proc IN ('N', '0') THEN
                            p_message := 'OPEN_ACCOUNT_OPENING_FORM';
                            RETURN;
                        END IF;
                    END IF;
                    
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        NULL; -- Continue without this info
                END;
                
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    p_message := 'WARNING: Investor information not found';
            END;
        END IF;

        -- Cross-channel validation
        BEGIN
            wealthmaker.PRC_VALIDATE_CROSS_CHNL_INFO(
                p_common_id, 
                v_inv_code, 
                p_login_id, 
                v_cross_chanel_count
            );
            
            IF v_cross_chanel_count > 0 THEN
                p_message := 'OPEN_CROSS_CHANNEL_VALIDATION';
                RETURN;
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                p_message := 'WARNING: Cross-channel validation failed: ' || SQLERRM;
        END;

        -- Return all data through cursor
        OPEN p_cursor FOR
        SELECT 
            v_sch_code as sch_code,
            v_sch_name as sch_name,
            v_mut_name as mut_name,
            v_nature as nature,
            v_inv_code as inv_code,
            v_investor_name as investor_name,
            v_pan as pan,
            v_busi_rm_code as busi_rm_code,
            v_busi_branch_code as busi_branch_code,
            v_busi_tr_date as busi_tr_date,
            v_expense as expense,
            v_branch_name as branch_name,
            v_logged_userid as logged_userid,
            v_main_code as main_code,
            v_rm_name as rm_name,
            CASE WHEN v_atm_scheme THEN 'Y' ELSE 'N' END as atm_scheme,
            'SUCCESS' as status_message
        FROM dual;

        p_message := 'SUCCESS';
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        p_message := 'ERROR: ' || SQLERRM;
        OPEN p_cursor FOR
        SELECT 
            NULL as sch_code,
            NULL as sch_name,
            NULL as mut_name,
            NULL as nature,
            NULL as inv_code,
            NULL as investor_name,
            NULL as pan,
            NULL as busi_rm_code,
            NULL as busi_branch_code,
            NULL as busi_tr_date,
            NULL as expense,
            NULL as branch_name,
            NULL as logged_userid,
            NULL as main_code,
            NULL as rm_name,
            NULL as atm_scheme,
            'ERROR: ' || SQLERRM as status_message
        FROM dual;
END process_mf_transaction_doc;
/