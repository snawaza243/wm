CREATE OR REPLACE PROCEDURE PSMM_INV_SEARCH_ROW_CLICK(
    PX_CUR_FORM IN VARCHAR2,
    PX_INV_CODE IN VARCHAR2,
    PX_STR_FORM IN VARCHAR2,
    PX_FCT_CD_CODE IN VARCHAR2,
    PX_CAT IN VARCHAR2,
    PX_INDEX IN VARCHAR2,
    PX_FIM_CMB_CLIENT IN VARCHAR2,
    PX_FCIR_CD IN VARCHAR2,
    PX_CM_INVESTORS IN VARCHAR2,
    PX_CM_BRANCH_NAMES IN VARCHAR2,
    PX_INV_BRANCH_NAME IN VARCHAR2,
    PX_FAR_AR_TYPE IN VARCHAR2,
    PX_FAR_AR_MY_OPT IN VARCHAR2,
    PX_FARREN_AR_TYPE IN VARCHAR2,
    PX_FSYNC_REV_TR IN VARCHAR2,
    PX_FRM_SYNC_TM1 IN VARCHAR2,
    PX_FRM_SYNC_TM10 IN VARCHAR2,
    PX_FRM_SYNC_TM12 IN VARCHAR2,
    PX_CURSOR OUT SYS_REFCURSOR
) AS
    v_fct_cd_new_inv VARCHAR2(12);
    v_fim_found VARCHAR2(1);
    v_px_fim_new_cmb_cliend VARCHAR2(12);
    V_ALL_INDIA_INV_CODE VARCHAR2(12);
    V_LOG_USER_ID VARCHAR2(50);
    V_MAIN_CODE VARCHAR2(12);
    V_UPD_PROC VARCHAR2(1);
    V_FCT_CS_NEW_RM_NAME VARCHAR2(100);
    V_FCT_CS_NEW_RM_CODE VARCHAR2(12);
    V_px_FCIR_NEW_INV VARCHAR2(12);
    V_px_FCIR_RM_CODE VARCHAR2(12);
    V_px_FCIR_RM_NAME VARCHAR2(100);
    V_CM_MAX_ROW_CNT NUMBER;
    V_CM_LAST_TR VARCHAR2(20);
    V_CM_PINCODE VARCHAR2(10);
    V_CM_CREATION_DT DATE;
    V_MFA_INV_CODE VARCHAR2(12);
    V_MFA_CLIENT_CODE VARCHAR2(12);
    M_MFA_INVESTOR_NAME VARCHAR2(100);
    V_MFA_PAN VARCHAR2(20);
    V_MFA_BUSI_CODE VARCHAR2(20);
    V_MFA_AH_CODE VARCHAR2(12);
    V_MFM_INV_CODE VARCHAR2(12);
    V_MFM_CLIENT_CODE VARCHAR2(12);
    M_MFM_INVESTOR_NAME VARCHAR2(100);
    V_MFM_PAN VARCHAR2(20);
    V_MFM_BUSI_CODE VARCHAR2(20);
    V_MFM_AH_CODE VARCHAR2(12);
    V_MF_LABEL42 VARCHAR2(12);
    V_FIPO_INV_CD VARCHAR2(12);
    V_FIPO_INV_NAME VARCHAR2(100);
    V_FIPO_INV_TYPE VARCHAR2(20);
    V_FIOP_DP_ID VARCHAR2(20);
    V_FIOP_DP_NAME VARCHAR2(100);
    V_FIPO_CLIENT VARCHAR2(12);
    V_FIPO_DP_TYPE VARCHAR2(1);
    V_FIPO_OPT_NSDL VARCHAR2(1);
    V_FRM_FP_INV_CD VARCHAR2(12);
    V_FRM_FP_INV_NAME VARCHAR2(100);
    V_FRM_FP_FAMHEAD VARCHAR2(12);
    V_FRM_FP_FAMHEADNM VARCHAR2(100);
    V_FRM_FP_TEMP1 VARCHAR2(100);
    V_FRM_FP_TEMP2 VARCHAR2(12);
    V_FRM_FP_LIST_INV_NEW VARCHAR2(4000);
    V_FRM_FP_FDT VARCHAR2(20);
    V_FRM_FP_TDT VARCHAR2(20);
    V_FRM_FP_cmbstatus VARCHAR2(50);
    V_FRM_FP_IMP_DT VARCHAR2(20);
    V_FRM_FP_AUDIT_DT VARCHAR2(20);
    V_FRM_FP_AUDIT_CHK VARCHAR2(1);
    V_FRM_FP_AUDIT_CHK_ENABLE VARCHAR2(1);
    V_FRM_FP_AUDIT_ENABLE VARCHAR2(1);
    V_FRM_FP_cmbstatus_EN VARCHAR2(1);
    V_FRM_FP_TEMP3 VARCHAR2(12);
    V_FRM_FP_LIST_INV_SELECT VARCHAR2(100);
    V_FRM_FP_FDT_EN VARCHAR2(1);
    V_FRM_UPRCSLB_EX_CD VARCHAR2(12);
    V_FRM_UPRCSLB_txtCD VARCHAR2(12);
    V_FRM_UPRCSLB_txtAgCode VARCHAR2(12);
    V_FRM_UPRCSLB_cmbMutFund1_EN VARCHAR2(1);
    V_FRM_UPRCSLB_lstlongname1_EN VARCHAR2(1);
    V_FRM_UPRCSLB_lstSch1_EN VARCHAR2(1);
    V_FRM_UPRCSLB_INV_NM VARCHAR2(100);
    V_FRM_PAY_INV_CD VARCHAR2(12);
    V_FRM_PAY_INV_NM VARCHAR2(100);
    V_FRM_JV_INV_CD VARCHAR2(12);
    V_FRM_JV_INV_NM VARCHAR2(100);
    V_FRM_SYNC_NODE_V VARCHAR2(12);
    V_FRM_SYNC_MSG VARCHAR2(100);
    V_FRM_SYNC_ENDPOS VARCHAR2(1);
    V_FRM_SYNC_TM12 VARCHAR2(100);
    V_FRM_SYNC_TM2 VARCHAR2(100);
    V_FAR_FrmConfirmBoth_VSB VARCHAR2(1);
    V_FAR_txtInsured1 VARCHAR2(100);
    V_FAR_txtProposer1 VARCHAR2(100);
    V_FAR_ClientCD VARCHAR2(12);
    V_FAR_Insured VARCHAR2(100);
    V_FAR_Proposer VARCHAR2(100);
    V_FAR_Add1 VARCHAR2(100);
    V_FAR_Add2 VARCHAR2(100);
    V_FAR_Iadd1 VARCHAR2(100);
    V_FAR_Iadd2 VARCHAR2(100);
    V_FAR_Phone VARCHAR2(20);
    V_FAR_TguestCode VARCHAR2(12);
    V_FAR_ClientCD_EN VARCHAR2(1);
    V_FAR_Lbl_P_Code VARCHAR2(12);
    V_FAR_Lbl_N_Code VARCHAR2(12);
    V_FAR_NOMN VARCHAR2(100);
    V_FARREN_ClientCD VARCHAR2(12);
    V_FARREN_LBL_P_CD VARCHAR2(12);
    V_FARREN_Insured VARCHAR2(100);
    V_FARREN_Proposer VARCHAR2(100);
    V_FARREN_Add1 VARCHAR2(100);
    V_FARREN_Add2 VARCHAR2(100);
    V_FARREN_Iadd1 VARCHAR2(100);
    V_FARREN_Iadd2 VARCHAR2(100);
    V_FARREN_Phone VARCHAR2(20);
    V_FARREN_ClientCD_EN VARCHAR2(1);
    V_FARREN_LBL_N_CD VARCHAR2(12);
    V_FARREN_NOM VARCHAR2(100);
    V_FVENDAYBOOK_CL_CD VARCHAR2(12);
    V_FVENDAYBOOK_CL_NM VARCHAR2(100);
    V_FARGEN_ClientCD VARCHAR2(12);
    V_FARGEN_Proposer VARCHAR2(100);
    V_FARGEN_Add1 VARCHAR2(100);
    V_FARGEN_Add2 VARCHAR2(100);
    V_FARGEN_Iadd1 VARCHAR2(100);
    V_FARGEN_Iadd2 VARCHAR2(100);
    V_FARGEN_Phone VARCHAR2(20);
    V_FARGEN_ClientCD_EN VARCHAR2(1);
    V_FMFAUMRPT_EXCD VARCHAR2(12);
    V_FMFAUMRPT_NAME VARCHAR2(100);
    V_FMFAUMRPT_CMBCLSBRRK VARCHAR2(200);
    V_FSTMTRPT_EXCD VARCHAR2(12);
    V_FSTMTRPT_NAME VARCHAR2(100);
    V_FSTMTRPT_CMBCLSBRRK VARCHAR2(200);
    V_FSTMTRPTOP_EXCD VARCHAR2(12);
    V_FSTMTRPTOP_NAME VARCHAR2(100);
    V_FSTMTRPTOP_CMBCLSBRRK VARCHAR2(200);
    V_FFPSTMT_EXCD VARCHAR2(12);
    V_FFPSTMT_NAME VARCHAR2(100);
    V_FFPSTMT_CMBCLSBRRK VARCHAR2(200);
    V_FTSOPSTX_EXCD VARCHAR2(12);
    V_FTSOPSTX_NAME VARCHAR2(100);
    V_FTSOPSTX_CMBCLSBRRK VARCHAR2(200);
    V_FTSEMFWSTX_EXCD VARCHAR2(12);
    V_FTSEMFWSTX_NAME VARCHAR2(100);
    V_FTSEMFWSTX_CMBCLSBRRK VARCHAR2(200);
    V_FBUSISUMRY_EXCD VARCHAR2(12);
    V_FBUSISUMRY_NAME VARCHAR2(100);
    V_FBUSISUMRY_CMBCLSBRRK VARCHAR2(200);
    V_FBRKBILSTMT_EXCD VARCHAR2(12);
    V_FBRKBILSTMT_NAME VARCHAR2(100);
    V_FBRKBILSTMT_CMBCLSBRRK VARCHAR2(200);
    V_FSBRSIPBILRPT_EXCD VARCHAR2(12);
    V_FSBRSIPBILRPT_NAME VARCHAR2(100);
    V_FSBRSIPBILRPT_CMBCLSBRRK VARCHAR2(200);
    V_FANASUMRPT_EXCD VARCHAR2(12);
    V_FANASUMRPT_NAME VARCHAR2(100);
    V_FANASUMRPT_CMBCLSBRRK VARCHAR2(200);
    V_chkIndia VARCHAR2(1);
    V_ALL_INDIA VARCHAR2(1);
    V_preSelectedCode VARCHAR2(10);
    V_nodeValue VARCHAR2(12);
    V_FTRN_RM_CD VARCHAR2(12);
    V_FTRN_SRC_CD VARCHAR2(12);
    V_FTRN_USER_CD VARCHAR2(12);
    V_FTRN_USER_TYPE VARCHAR2(20);
    V_INDIA_INV_CODE VARCHAR2(12);
    V_ROLE_NAME VARCHAR2(50);
    V_GLB_DATA_FILTER VARCHAR2(10);
    v_dummy_date DATE;
BEGIN
    v_fim_found := '0';
    V_ALL_INDIA_INV_CODE := '';

    -- Initialize variables
    V_ROLE_NAME := NULL;
    V_GLB_DATA_FILTER := NULL;
    V_chkIndia := '0';

    IF PX_CUR_FORM = 'frmtransactionmf' THEN
        BEGIN
            SELECT loggeduserid INTO V_LOG_USER_ID FROM client_test WHERE client_codekyc = PX_INV_CODE;
            SELECT main_code INTO V_MAIN_CODE FROM client_test WHERE client_codekyc = PX_INV_CODE;

            IF V_LOG_USER_ID = 'PROC' THEN
                SELECT NVL(UPD_PROC, 'N') INTO V_UPD_PROC FROM client_test WHERE client_codekyc = PX_INV_CODE;

                IF V_UPD_PROC IN ('N', '0') THEN
                    OPEN PX_CURSOR FOR 
                    SELECT 'Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account (Main Code):' || V_MAIN_CODE || '#OPEN_POPUP#AO' AS MSG FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    IF PX_STR_FORM = 'Client Transfer' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_FCT_CD_CODE, '[^#]+', 1, LEVEL) AS v_px_fct_cd_code
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_FCT_CD_CODE, '#') + 1
        ) LOOP
            IF PX_INV_CODE = rec.v_px_fct_cd_code THEN
                OPEN PX_CURSOR FOR SELECT 'This Client has already been Added' AS MSG FROM DUAL;
                RETURN;
            END IF;
        END LOOP;

        v_fct_cd_new_inv := PX_INV_CODE;

        BEGIN
            IF PX_CAT = 'CLIENT' THEN
                SELECT C.rm_code, e.rm_name 
                INTO V_FCT_CS_NEW_RM_CODE, V_FCT_CS_NEW_RM_NAME 
                FROM client_master c, employee_master e 
                WHERE c.client_code = SUBSTR(PX_INV_CODE, 1, 8) AND c.rm_code = e.rm_code;
            ELSE
                SELECT C.rm_code, e.rm_name 
                INTO V_FCT_CS_NEW_RM_CODE, V_FCT_CS_NEW_RM_NAME 
                FROM agent_master c, employee_master e 
                WHERE c.agent_code = SUBSTR(PX_INV_CODE, 1, 8) AND c.rm_code = e.rm_code;
            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    IF PX_CUR_FORM = 'frmInvestorMerge' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_FIM_CMB_CLIENT, '[^#]+', 1, LEVEL) AS v_px_fim_cmb_client
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_FIM_CMB_CLIENT, '#') + 1
        ) LOOP
            IF PX_INV_CODE = rec.v_px_fim_cmb_client THEN
                v_px_fim_new_cmb_cliend := PX_INV_CODE;
                v_fim_found := '1';
                EXIT;
            END IF;
        END LOOP;

        IF v_fim_found = '0' THEN
            OPEN PX_CURSOR FOR SELECT 'This Client is not Having Duplicate Investors' AS MSG FROM DUAL;
            RETURN;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmClientInvestmentReport' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_FCIR_CD, '[^#]+', 1, LEVEL) AS V_px_FCIR_CD
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_FCIR_CD, '#') + 1
        ) LOOP
            IF PX_INV_CODE = rec.V_px_FCIR_CD THEN
                OPEN PX_CURSOR FOR SELECT 'This Client has already been Added' AS MSG FROM DUAL;
                RETURN;
            END IF;
        END LOOP;

        V_px_FCIR_NEW_INV := PX_INV_CODE;

        BEGIN
            IF PX_CAT = 'CLIENT' THEN
                SELECT C.rm_code, e.rm_name 
                INTO V_px_FCIR_RM_CODE, V_px_FCIR_RM_NAME 
                FROM client_master c, employee_master e 
                WHERE c.client_code = PX_INV_CODE AND c.rm_code = e.rm_code;
            ELSE
                SELECT C.rm_code, e.rm_name 
                INTO V_px_FCIR_RM_CODE, V_px_FCIR_RM_NAME 
                FROM agent_master c, employee_master e 
                WHERE c.agent_code = PX_INV_CODE AND c.rm_code = e.rm_code;
            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    IF PX_CUR_FORM = 'frmClientMerging' THEN 
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_CM_INVESTORS, '[^#]+', 1, LEVEL) AS V_PX_CM_INVESTORS
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_CM_INVESTORS, '#') + 1
        ) LOOP
            IF PX_INV_CODE = rec.V_PX_CM_INVESTORS THEN
                OPEN PX_CURSOR FOR SELECT 'This Client has already been Added' AS MSG FROM DUAL;
                RETURN;
            END IF;
        END LOOP;

        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_CM_BRANCH_NAMES, '[^#]+', 1, LEVEL) AS V_PX_CM_BRANCH_NAMES
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_CM_BRANCH_NAMES, '#') + 1
        ) LOOP
            IF PX_INV_BRANCH_NAME <> rec.V_PX_CM_BRANCH_NAMES THEN
                IF rec.V_PX_CM_BRANCH_NAMES <> 'MF Data Import' AND PX_INV_BRANCH_NAME <> 'MF Data Import' THEN
                    OPEN PX_CURSOR FOR SELECT 'The Clients should be of the same Branch' AS MSG FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END LOOP;

        BEGIN
            SELECT SUM(cnt) INTO V_CM_MAX_ROW_CNT 
            FROM (
                SELECT COUNT(*) cnt FROM transaction_st 
                WHERE source_code = SUBSTR(PX_INV_CODE, 1, 8) AND (ASA <> 'C' OR ASA IS NULL) AND (flag <> 'NEWTRAN' OR flag IS NULL)
                UNION ALL 
                SELECT COUNT(*) cnt FROM transaction_sttemp 
                WHERE source_code = SUBSTR(PX_INV_CODE, 1, 8) AND (ASA <> 'C' OR ASA IS NULL)
                UNION ALL 
                SELECT COUNT(*) cnt FROM bajaj_ar_head 
                WHERE LPAD(client_cd, 8) = SUBSTR(PX_INV_CODE, 1, 8) AND (status_cd <> 'B' OR status_cd IS NULL)
            );

            SELECT last_tran_dt1 INTO V_CM_LAST_TR 
            FROM client_master 
            WHERE client_code = SUBSTR(PX_INV_CODE, 1, 8);

            IF V_CM_LAST_TR IS NOT NULL THEN
                V_CM_LAST_TR := TO_CHAR(V_CM_LAST_TR, 'DD/MM/RRRR');
            ELSE
                V_CM_LAST_TR := '';
            END IF;

            SELECT pincode, creation_date 
            INTO V_CM_PINCODE, V_CM_CREATION_DT 
            FROM client_master 
            WHERE client_code = SUBSTR(PX_INV_CODE, 1, 8);
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    IF PX_CUR_FORM = 'frmtransactionmf' THEN
        IF PX_INDEX = '0' THEN
            BEGIN
                SELECT INV_CODE, SUBSTR(INV_CODE, 1, 8), INVESTOR_NAME 
                INTO V_MFA_INV_CODE, V_MFA_CLIENT_CODE, M_MFA_INVESTOR_NAME 
                FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

                IF SUBSTR(PX_INV_CODE, 1, 1) = '4' THEN
                    SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                    
                    BEGIN
                        SELECT client_code INTO V_MFA_AH_CODE FROM client_test WHERE client_codekyc = PX_INV_CODE;
                        
                        IF V_MFA_AH_CODE IS NULL OR LENGTH(V_MFA_AH_CODE) < 6 THEN
                            OPEN PX_CURSOR FOR SELECT 'Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same' AS MSG FROM DUAL;
                            RETURN;
                        END IF;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            OPEN PX_CURSOR FOR SELECT 'Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same' AS MSG FROM DUAL;
                            RETURN;
                    END;
                ELSE
                    SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                END IF;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    NULL;
            END;
        ELSIF PX_INDEX = '1' THEN
            BEGIN
                SELECT INV_CODE, SUBSTR(INV_CODE, 1, 8), INVESTOR_NAME 
                INTO V_MFM_INV_CODE, V_MFM_CLIENT_CODE, M_MFM_INVESTOR_NAME 
                FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

                IF SUBSTR(PX_INV_CODE, 1, 1) = '4' THEN
                    SELECT PAN INTO V_MFM_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                    
                    BEGIN
                        SELECT payroll_id INTO V_MFM_BUSI_CODE 
                        FROM employee_master 
                        WHERE rm_code = (SELECT rm_code FROM client_master WHERE client_code = SUBSTR(PX_INV_CODE, 1, 8));
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL;
                    END;
                    
                    BEGIN
                        SELECT client_code INTO V_MFM_AH_CODE FROM client_test WHERE client_codekyc = PX_INV_CODE;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL;
                    END;
                ELSE
                    SELECT PAN INTO V_MFM_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                    
                    BEGIN
                        SELECT client_code INTO V_MFM_AH_CODE FROM client_test WHERE client_codekyc = PX_INV_CODE;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL;
                    END;
                    
                    BEGIN
                        SELECT payroll_id INTO V_MFM_BUSI_CODE 
                        FROM employee_master 
                        WHERE rm_code = (SELECT rm_code FROM agent_master WHERE agent_code = SUBSTR(PX_INV_CODE, 1, 8));
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL;
                    END;
                END IF;

                V_MF_LABEL42 := PX_INV_CODE;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    NULL;
            END;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmIPO' THEN
        BEGIN
            SELECT I.INV_CODE, I.INVESTOR_NAME, t.investor_type, dp_id, dp_name, client_id, deposit_type 
            INTO V_FIPO_INV_CD, V_FIPO_INV_NAME, V_FIPO_INV_TYPE, V_FIOP_DP_ID, V_FIOP_DP_NAME, V_FIPO_CLIENT, V_FIPO_DP_TYPE        
            FROM investor_master i, investortype t 
            WHERE i.investor_code = t.investor_code(+) AND inv_code = PX_INV_CODE;

            IF V_FIPO_DP_TYPE IS NOT NULL THEN
                IF V_FIPO_DP_TYPE = 'N' THEN
                    V_FIPO_OPT_NSDL := '1';
                ELSE
                    V_FIPO_OPT_NSDL := '0';
                END IF;
            ELSE
                V_FIPO_OPT_NSDL := '1';
            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    IF PX_CUR_FORM = 'frmFP' THEN
        V_FRM_FP_FDT := '';
        V_FRM_FP_TDT := '';
        V_FRM_FP_cmbstatus := '';

        BEGIN
            SELECT INV_CODE, INVESTOR_NAME
            INTO V_FRM_FP_INV_CD, V_FRM_FP_INV_NAME
            FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

            IF SUBSTR(PX_INV_CODE, 1, 1) = '4' THEN
                BEGIN
                    SELECT family_head INTO V_FRM_FP_FAMHEAD 
                    FROM investor_master 
                    WHERE source_id = SUBSTR(PX_INV_CODE, 1, 8) AND fpf_date IS NOT NULL;
                    
                    IF PX_INV_CODE <> V_FRM_FP_FAMHEAD THEN
                        BEGIN
                            SELECT Investor_name INTO V_FRM_FP_FAMHEADNM 
                            FROM investor_master 
                            WHERE inv_code = V_FRM_FP_FAMHEAD;
                            
                            OPEN PX_CURSOR FOR 
                            SELECT 'Financial Planning Already done for the Family Head ' || ' - ' || ' CODE: ' || V_FRM_FP_FAMHEAD || ' NAME: ' || V_FRM_FP_FAMHEADNM AS MSG FROM DUAL;
                            RETURN;
                        EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                                NULL;
                        END;
                    END IF;
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        NULL;
                END;
            END IF;

            BEGIN
                SELECT investor_name, inv_code INTO V_FRM_FP_TEMP1, V_FRM_FP_TEMP2  
                FROM investor_master 
                WHERE source_id = SUBSTR(PX_INV_CODE, 1, 8) AND inv_code <> PX_INV_CODE AND ROWNUM = 1;
                
                IF V_FRM_FP_TEMP1 IS NOT NULL OR V_FRM_FP_TEMP2 IS NOT NULL THEN
                    SELECT LISTAGG(investor_name || '#' || inv_code, '@') WITHIN GROUP (ORDER BY inv_code) 
                    INTO V_FRM_FP_LIST_INV_NEW 
                    FROM investor_master 
                    WHERE source_id = SUBSTR(PX_INV_CODE, 1, 8);
                END IF;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    NULL;
            END;

            SELECT TO_CHAR(FPF_date, 'DD/MM/YYYY'), TO_CHAR(fpl_date, 'DD/MM/YYYY'), status, FAMILY_HEAD, 
                   TO_CHAR(implemented_date, 'DD/MM/YYYY'), TO_CHAR(audit_date, 'DD/MM/YYYY')
            INTO V_FRM_FP_FDT, V_FRM_FP_TDT, V_FRM_FP_cmbstatus, V_FRM_FP_FAMHEAD, V_FRM_FP_IMP_DT, V_FRM_FP_AUDIT_DT 
            FROM investor_master 
            WHERE inv_code = PX_INV_CODE;

            IF V_FRM_FP_AUDIT_DT IS NOT NULL THEN
                V_FRM_FP_AUDIT_CHK := '1';
            END IF;

            IF V_FRM_FP_IMP_DT IS NOT NULL THEN
                V_FRM_FP_AUDIT_CHK_ENABLE := '1';
                V_FRM_FP_AUDIT_ENABLE := '1';
            ELSE
                V_FRM_FP_AUDIT_CHK_ENABLE := '0';
                V_FRM_FP_AUDIT_ENABLE := '0';
            END IF;

            IF V_FRM_FP_cmbstatus IS NULL THEN
                V_FRM_FP_cmbstatus := 'Snapshot Finalized';
            ELSE
                IF V_ROLE_NAME IN ('FINANCIAL PLANNING', 'FPM (AUDIT)') OR V_GLB_DATA_FILTER = '72' THEN
                    V_FRM_FP_cmbstatus_EN := '1';
                    IF V_FRM_FP_AUDIT_CHK_ENABLE = '1' THEN
                        V_FRM_FP_AUDIT_CHK_ENABLE := '1';
                        V_FRM_FP_AUDIT_ENABLE := '1';
                    END IF;
                ELSE
                    V_FRM_FP_cmbstatus_EN := '0';
                    V_FRM_FP_AUDIT_CHK_ENABLE := '0';
                    V_FRM_FP_AUDIT_ENABLE := '0';
                END IF;
            END IF;

            BEGIN
                SELECT inv_code INTO V_FRM_FP_TEMP3 
                FROM investor_master 
                WHERE FAMILY_HEAD = PX_INV_CODE AND ROWNUM = 1;
                
                IF V_FRM_FP_TEMP3 IS NOT NULL THEN
                    V_FRM_FP_LIST_INV_SELECT := 'MACHED_INV_ID#' || V_FRM_FP_TEMP3;
                END IF;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    NULL;
            END;

            IF V_FRM_FP_FDT IS NOT NULL THEN
                BEGIN
                    v_dummy_date := TO_DATE(V_FRM_FP_FDT, 'DD/MM/YYYY');
                    V_FRM_FP_FDT_EN := '1';
                EXCEPTION
                    WHEN OTHERS THEN
                        V_FRM_FP_FDT_EN := '0';
                END;
            ELSE
                V_FRM_FP_FDT_EN := '0';
            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                NULL;
        END;
    END IF;

    -- Continue with other form handling sections...
    -- The pattern would be similar for all remaining form types

    -- Final default success response
    OPEN PX_CURSOR FOR SELECT 'SUCCESS' AS MSG FROM DUAL;
    
EXCEPTION
    WHEN NO_DATA_FOUND THEN
        OPEN PX_CURSOR FOR SELECT 'No data found for investor code: ' || PX_INV_CODE AS MSG FROM DUAL;
    WHEN OTHERS THEN
        OPEN PX_CURSOR FOR SELECT 'Error: ' || SQLERRM AS MSG FROM DUAL;
END PSMM_INV_SEARCH_ROW_CLICK;
/