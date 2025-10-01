CREATE OR REPLACE PROCEDURE PSMM_INV_SEARCH_ROW_CLICK(

)AS

    v_fct_cd_new_inv varchar2(12);

    v_fim_found varchar2(1);

BEGIN
    v_fim_found :='0';
    V_ALL_INDIA_INV_CODE:= '';

    IF PX_CUR_FORM = 'frmtransactionmf' THEN
        select loggeduserid INTO V_LOG_USER_ID from client_test where client_codekyc= PX_INV_CODE;
        select main_code INTO V_MAIN_CODE from client_test where client_codekyc=PX_INV_CODE;

        IF V_LOG_USER_ID = 'PROC' THEN
            select NVL(UPD_PROC,'N') INTO V_UPD_PROC from client_test where client_codekyc = PX_INV_CODE;

            IF V_UPD_PROC IN ('N', '0') THEN
                OPEN PX_CURSOR SELECT 'Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account (Main Code):' || V_MAIN_CODE || '#OPEN_POPUP#AO' AS MSG FROM DUAL;
                RETURN;
            END IF;
        END IF;


    END IF;

    IF PX_STR_FORM 'Client Transfer' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(px_fct_cd_code, '[^#]+', 1, LEVEL) AS v_px_fct_cd_code
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(px_fct_cd_code, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.v_px_fct_cd_code  THEN
                OPEN PX_CURSOR SELECT 'This Client has already been Added' AS MSG FROM DUAL;
                RETURN;
            END IF;

            v_fct_cd_new_inv:= PX_INV_CODE
        END LOOP;

        IF PX_CAT = 'CLIENT' THEN
            select C.rm_code, rm_name INTO V_FCT_CS_NEW_RM_NAME, V_FCT_CS_NEW_RM_CODE from client_master c,employee_master e where client_code=SUBSTR(PX_INV_CODE, 1,8) and c.rm_code=e.rm_code
        ELSE
            select C.rm_code, rm_name INTO V_FCT_CS_NEW_RM_NAME, V_FCT_CS_NEW_RM_CODE from agent_master c,employee_master e where agent_code= SUBSTR(PX_INV_CODE, 1,8) and c.rm_code=e.rm_code
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmInvestorMerge' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(px_fim_cmb_client, '[^#]+', 1, LEVEL) AS v_px_fim_cmb_client
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(px_fim_cmb_client, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.v_px_fim_cmb_client THEN
                v_px_fim_new_cmb_cliend:= PX_INV_CODE;
                v_fim_found := '1';
            END IF;

        END LOOP;

        if v_fim_found = '0' then
            OPEN PX_CURSOR SELECT 'This Client is not Having Duplicate Investors' AS MSG FROM DUAL; RETURN;
        end if;

    END IF;

    IF PX_CUR_FORM = 'frmClientInvestmentReport' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(px_FCIR_CD, '[^#]+', 1, LEVEL) AS V_px_FCIR_CD
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(px_FCIR_CD, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.V_px_FCIR_CD THEN
                OPEN PX_CURSOR SELECT 'This Client has already been Added' AS MSG FROM DUAL; RETURN;
            END IF;
        END LOOP;

        V_px_FCIR_NEW_INV:= PX_INV_CODE;

        IF PX_CAT = 'CLIENT' THEN
            select C.rm_code, rm_name INTO V_px_FCIR_RM_CODE, V_px_FCIR_RM_NAME from client_master c,employee_master e where client_code=PX_INV_CODE and c.rm_code=e.rm_code
        ELSE
            select C.rm_code,rm_name INTO V_px_FCIR_RM_CODE, V_px_FCIR_RM_NAME from agent_master c,employee_master e where agent_code=PX_INV_CODE and c.rm_code=e.rm_code

        END IF;
        
    END IF;

    IF PX_CUR_FORM = 'frmClientMerging' THEN 
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_CM_INVESTORS, '[^#]+', 1, LEVEL) AS V_PX_CM_INVESTORS
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_CM_INVESTORS, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.V_PX_CM_INVESTORS THEN
                OPEN PX_CURSOR SELECT 'This Client has already been Added' AS MSG FROM DUAL; RETURN;
            END IF;
        END LOOP;

        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_CM_BRANCH_NAMES, '[^#]+', 1, LEVEL) AS V_PX_CM_BRANCH_NAMES
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_CM_BRANCH_NAMES, '#') + 1
        )
        LOOP
            IF PX_INV_BRANCH_NAME <> rec.V_PX_CM_BRANCH_NAMES THEN

                IF rec.V_PX_CM_BRANCH_NAMES <> 'MF Data Import' AND PX_INV_BRANCH_NAME <> 'MF Data Import' THEN

                OPEN PX_CURSOR SELECT 'The Clients should be of the same Branch' AS MSG FROM DUAL; RETURN;
                END IF;
            END IF;
        END LOOP;

         select sum(cnt) INTO V_CM_MAX_ROW_cnt from(select count(*) cnt from transaction_st where source_code=SUBSTR(PX_INV_CODE,1,8) and (ASA<>'C' or ASA is null) and (flag<>'NEWTRAN' or flag is null) 
          Union All 
         select count(*) cnt from transaction_sttemp where source_code=SUBSTR(PX_INV_CODE,1,8) and (ASA<>'C' or ASA is null) 
          Union All 
        select count(*) cnt from bajaj_ar_head where lpad(client_cd,8)=SUBSTR(PX_INV_CODE,1,8) and (status_cd<>'B' or status_cd is null))
                

        Select last_tran_dt1 INTO V_CM_LAST_TR from client_master where client_code=SUBSTR(PX_INV_CODE,1,8);

        IF V_CM_LAST_TR IS NOT NULL THEN
            V_CM_LAST_TR:= TO_CHAR(V_CM_LAST_TR,'DD/MM/RRRR');
        ELSE
            V_CM_LAST_TR:='';
        END IF;

        Select pincode,creation_date INTO V_CM_PINCODE, V_CM_CREATION_DT from client_master where client_code=SUBSTR(PX_INV_CODE,1,8);
    END IF;

    IF PX_CUR_FORM = 'frmtransactionmf' THEN
        IF PX_INDEX = '0' THEN
            SELECT INV_CODE,SUBSTR(INV_CODE,1,8), INVESTOR_NAME 
            INTO V_MFA_INV_CODE, V_MFA_CLIENT_CODE, M_MFA_INVESTOR_NAME FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

            IF(SUBSTR(PX_INV_CODE,1,1) = '4') THEN
                SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                --select payroll_id INTO V_MFA_BUSI_CODE from employee_master where rm_code=(select rm_code from client_master where client_code=SUBSTR(PX_INV_CODE,1,8));
                select client_code INTO V_MFA_AH_CODE from client_test where client_codekyc=PX_INV_CODE;

                IF V_MFA_AH_CODE IS NULL OR LENGTH(V_MFA_AH_CODE)<6 THEN
                    OPEN PX_CURSOR SELECT 'Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same' AS MSG FROM DUAL; RETURN;
                    
                END IF;
            ELSE
                SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                --select payroll_id INTO V_MFA_BUSI_CODE from employee_master where rm_code=(select rm_code from agent_master where agent_code=SUBSTR(PX_INV_CODE,1,8));
            END IF;
        ELSE IF PX_INDEX = '1' THEN
            SELECT INV_CODE,SUBSTR(INV_CODE,1,8), INVESTOR_NAME 
            INTO V_MFM_INV_CODE, V_MFM_CLIENT_CODE, M_MFM_INVESTOR_NAME FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

            IF(SUBSTR(PX_INV_CODE,1,1) = '4') THEN
                SELECT PAN INTO V_MFM_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                select payroll_id INTO V_MFM_BUSI_CODE from employee_master where rm_code=(select rm_code from client_master where client_code=SUBSTR(PX_INV_CODE,1,8));
                select client_code INTO V_MFM_AH_CODE from client_test where client_codekyc=PX_INV_CODE;
            ELSE
                SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                select client_code INTO V_MFM_AH_CODE from client_test where client_codekyc=PX_INV_CODE;
                select payroll_id INTO V_MFM_BUSI_CODE from employee_master where rm_code=(select rm_code from agent_master where agent_code=SUBSTR(PX_INV_CODE,1,8));

            END IF;

            V_MF_LABEL42:= PX_INV_CODE;



        END IF;



    END IF;


    IF PX_CUR_FORM = 'frmIPO' THEN
        select I.INV_CODE,      I.INVESTOR_NAME, t.investor_type, dp_id,        dp_name,        client_id,      deposit_type 
        INTO  V_FIPO_INV_CD,    V_FIPO_INV_NAME, V_FIPO_INV_TYPE, V_FIOP_DP_ID, V_FIOP_DP_NAME, V_FIPO_CLIENT,  V_FIPO_DP_TYPE        
        from investor_master i,investortype t where i.investor_code=t.investor_code(+) and inv_code = PX_INV_CODE;

        IF V_FIPO_DP_TYPE IS NOT NULL THEN
            IF V_FIPO_DP_TYPE = 'N' THEN
                V_FIPO_OPT_NSDL:= '1'; -- TRUE
            ELSE
                V_FIPO_OPT_NSDL:= '0'; -- FALSE
            END IF;
        ELSE
            V_FIPO_OPT_NSDL:= '1'; -- TRUE
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmFP' THEN
        V_FRM_FP_FDT:= '';
        V_FRM_FP_TDT:= '';
        V_FRM_FP_cmbstatus:= '';

        SELECT INV_CODE, INVESTOR_NAME
        INTO V_FRM_FP_INV_CD, V_FRM_FP_INV_NAME
        FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

        IF SUBSTR(PX_INV_CODE,1,1) = '4' THEN
            select family_head INTO V_FRM_FP_FAMHEAD from investor_master where source_id=SUBSTR(PX_INV_CODE,1,8) and fpf_date is not null;
            IF PX_INV_CODE <> V_FRM_FP_FAMHEAD THEN
                V_FRM_FP_FAMHEAD:= V_FRM_FP_FAMHEAD;

                select Investor_name INTO V_FRM_FP_FAMHEADNM from investor_master where inv_code=PX_INV_CODE;
                OPEN PX_CURSOR SELECT 'Financial Planning Already done for the Family Head ' || ' - ' || ' CODE: ' || V_FRM_FP_FAMHEAD || ' NAME: ' ||  V_FRM_FP_FAMHEADNM  AS MSG FROM DUAL; RETURN;
            END IF;
        END IF;

        select investor_name, inv_code INTO V_FRM_FP_TEMP1, V_FRM_FP_TEMP2  from investor_master where source_id=SUBSTR(PX_INV_CODE,1,8) and inv_code <> PX_INV_CODE;
        -- CUONT>0
        IF V_FRM_FP_TEMP1 IS NOT NULL OR V_FRM_FP_TEMP2 IS NOT NULL THEN
            SELECT LISTAGG(investor_name || '#' || inv_code, '@') WITHIN GROUP (ORDER BY inv_code) AS all_investors INTO V_FRM_FP_LIST_INV_NEW FROM investor_master WHERE source_id = SUBSTR(PX_INV_CODE, 1, 8);
        END IF;

        SELECT TO_CHAR(FPF_date, 'DD/MM/YYYY') AS FPF_date, TO_CHAR(fpl_date, 'DD/MM/YYYY') AS fpl_date, status, FAMILY_HEAD, TO_CHAR(implemented_date, 'DD/MM/YYYY') AS implemented_date, TO_CHAR(audit_date, 'DD/MM/YYYY') AS audit_date 
        INTO V_FRM_FP_FDT, V_FRM_FP_TDT, V_FRM_FP_cmbstatus, V_FRM_FP_FAMHEAD, V_FRM_FP_IMP_DT, V_FRM_FP_AUDIT_DT FROM investor_master WHERE inv_code = PX_INV_CODE;
        V_FRM_FP_FDT :=V_FRM_FP_FDT;
        V_FRM_FP_TDT :=V_FRM_FP_TDT;
        V_FRM_FP_cmbstatus :=V_FRM_FP_cmbstatus;
        V_FRM_FP_AUDIT_DT :=V_FRM_FP_AUDIT_DT

        IF V_FRM_FP_AUDIT_DT IS NOT NULL THEN
            V_FRM_FP_AUDIT_CHK:= '1'; -- TRUE
        END IF;
        V_FRM_FP_IMP_DT :=V_FRM_FP_IMP_DT;

        IF V_FRM_FP_IMP_DT IS NOT NULL THEN
            V_FRM_FP_AUDIT_CHK_ENABLE:= '1'; -- TRUE
            V_FRM_FP_AUDIT_ENABLE:= '1'; -- TRUE
        ELSE
            V_FRM_FP_AUDIT_CHK_ENABLE:= '0'; -- FALSE
            V_FRM_FP_AUDIT_ENABLE:= '0'; -- FALSE
        END IF;

        IF V_FRM_FP_cmbstatus IS NULL THEN
            V_FRM_FP_cmbstatus:= 'Snapshot Finalized';
        ELSE
            IF V_ROLE_NAME IN ('FINANCIAL PLANNING', 'FPM (AUDIT)') OR V_GLB_DATA_FILTER := '72' THEN
                V_FRM_FP_cmbstatus_EN :='1'; -- TRUE;

                IF V_FRM_FP_AUDIT_CHK_ENABLE = '1' THEN
                    V_FRM_FP_AUDIT_CHK_ENABLE = '1';
                    V_FRM_FP_AUDIT_ENABLE:= '1'; 
                END IF;
            ELSE
                V_FRM_FP_cmbstatus_EN :='0';
                    V_FRM_FP_AUDIT_CHK_ENABLE = '0';
                    V_FRM_FP_AUDIT_ENABLE:= '0'; 

            END IF;
        END IF;

        select inv_code INTO V_FRM_FP_TEMP3 from investor_master where FAMILY_HEAD=PX_INV_CODE;  
        IF V_FRM_FP_TEMP3 IS NOT NULL
            V_FRM_FP_LIST_INV_SELECT = 'MACHED_INV_ID#' || V_FRM_FP_TEMP3;
        END IF;

                V_FRM_FP_FDT :=V_FRM_FP_FDT;
        V_FRM_FP_TDT :=V_FRM_FP_TDT;
        V_FRM_FP_cmbstatus :=V_FRM_FP_cmbstatus;
        V_FRM_FP_AUDIT_DT :=V_FRM_FP_AUDIT_DT

        BEGIN
            IF V_FRM_FP_FDT IS NOT NULL THEN
                BEGIN
                    -- Try to convert to date
                    DECLARE
                        v_dummy_date DATE;
                    BEGIN
                        v_dummy_date := TO_DATE(V_FRM_FP_FDT, 'DD/MM/YYYY');
                        V_FRM_FP_FDT_EN := '1';
                    EXCEPTION
                        WHEN OTHERS THEN
                            V_FRM_FP_FDT_EN := '0';
                    END;
                END;
            ELSE
                V_FRM_FP_FDT_EN := '0';
            END IF;
        END;

        
        
    END IF;

    IF PX_CUR_FORM = 'frmUpdateRecdSlabs' THEN

        V_FRM_UPRCSLB_INV_NM:= '';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code V_FRM_UPRCSLB_EX_CD from agent_master where agent_code=PX_INV_CODE;
        ELSE
            select exist_code V_FRM_UPRCSLB_EX_CD from agent_master where agent_code=PX_INV_CODE;
        END IF;

        V_FRM_UPRCSLB_txtCD:= V_FRM_UPRCSLB_EX_CD;
        V_FRM_UPRCSLB_txtAgCode:=PX_INV_CODE;
        V_FRM_UPRCSLB_cmbMutFund1_EN:='1';
        V_FRM_UPRCSLB_lstlongname1_EN:='1';
        V_FRM_UPRCSLB_lstSch1_EN:='1';
    END IF;

    IF PX_CUR_FORM = 'frmPayment' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_FPAY_CL_CD, '[^#]+', 1, LEVEL) AS V_PX_FPAY_CL_CD
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_FPAY_CL_CD, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.V_PX_FPAY_CL_CD  THEN
                OPEN PX_CURSOR SELECT 'This Client/Agent has already been Added' AS MSG FROM DUAL;
                RETURN;
            END IF;
        END LOOP;
        SELECT INV_CODE, INVESTOR_NAME INTO V_FRM_PAY_INV_CD, V_FRM_PAY_INV_NM FORM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
    END IF;

    IF PX_CUR_FORM = 'frmjv' THEN
        IF PX_CAT = 'CLIENT' THEN
            FOR rec IN (
                SELECT REGEXP_SUBSTR(PX_FJV_CL_CD, '[^#]+', 1, LEVEL) AS V_PX_FJV_CL_CD
                FROM dual
                CONNECT BY LEVEL <= REGEXP_COUNT(PX_FJV_CL_CD, '#') + 1
            )
            LOOP
                IF PX_INV_CODE = rec.V_PX_FJV_CL_CD  THEN
                    OPEN PX_CURSOR SELECT 'This Client/Agent has already been Added' AS MSG FROM DUAL;
                    RETURN;
                END IF;
            END LOOP;
            SELECT INV_CODE, INVESTOR_NAME INTO V_FRM_JV_INV_CD, V_FRM_JV_INV_NM FORM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
        ELSE
            FOR rec IN (
                SELECT REGEXP_SUBSTR(PX_FJV_AG_CD, '[^#]+', 1, LEVEL) AS V_PX_FJV_AG_CD
                FROM dual
                CONNECT BY LEVEL <= REGEXP_COUNT(PX_FJV_AG_CD, '#') + 1
            )
            LOOP
                IF PX_INV_CODE = rec.V_PX_FJV_AG_CD  THEN
                    OPEN PX_CURSOR SELECT 'This Client/Agent has already been Added' AS MSG FROM DUAL;
                    RETURN;
                END IF;
            END LOOP;
            SELECT INV_CODE, INVESTOR_NAME INTO V_FRM_JV_INV_CD, V_FRM_JV_INV_NM FORM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmSynchronization' THEN
        IF PX_CAT = 'INVESTOR' THEN
            V_FRM_SYNC_NODE_V:= PX_INV_CODE;

            IF PX_FSYNC_REV_TR IS NULL THEN
                V_FRM_SYNC_MSG:= 'Revertal Transactions';
            END IF;

            IF PX_FRM_SYNC_TM1 IS NULL THEN`
                V_FRM_SYNC_ENDPOS := 'M';
            END IF;

            IF PX_FRM_SYNC_TM1 IS NOT NULL AND PX_FRM_SYNC_TM10 = 'Y' PX_FRM_SYNC_TM12 <> 'INV' THEN`
                V_FRM_SYNC_TM12 := 'INV#' || V_FRM_SYNC_NODE_V;
            END IF;

            IF PX_FRM_SYNC_TM1 IS NULL THEN
                V_FRM_SYNC_TM2 := 'New Transactions';
            END IF ;

            IF PX_FRM_SYNC_TM1 IS NOT NULL AND PX_FRM_SYNC_TM10 = 'Y' PX_FRM_SYNC_TM12 <> 'INV' THEN`
                V_FRM_SYNC_TM12 := 'INV#' || V_FRM_SYNC_NODE_V;
            END IF;



        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmAR' and px_far_ar_type IS NULL THEN
        IF PX_FAR_AR_MY_OPT <>'1' THEN
            V_FAR_FrmConfirmBoth_VSB := '1'; -- FOR VISIBLE TRUE
            V_FAR_txtInsured1 := '';
            V_FAR_txtProposer1 := '';
        ELSE
            SELECT INV_CODE, INVESTOR_NAME, '', ADDRESS1, ADDRESS2,  ADDRESS1, ADDRESS2, PHONE, '' GUES_CD
            INTO V_FAR_ClientCD, V_FAR_Insured, V_FAR_Proposer, V_FAR_Add1, V_FAR_Add2, V_FAR_Iadd1, V_FAR_Iadd2, V_FAR_Phone, V_FAR_TguestCode
            FROM INVESTOR_MASTER = PX_INV_CODE;

            V_FAR_ClientCD_EN:='0'; -- FOR ENABLE FASLE

            IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
                V_FAR_TguestCode:= V_FAR_TguestCode;
            END IF;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmAR_Renewal' AND PX_FARREN_AR_TYPE IS NULL THEN

        SELECT INV_CODE, INV_CODE, INVESTOR_NAME, INVESTOR_NAME, ADDRESS1, ADDRESS2, ADDRESS1, ADDRESS2, PHONE
        INTO V_FARREN_ClientCD, V_FARREN_LBL_P_CD, V_FARREN_Insured, V_FARREN_Proposer, V_FARREN_Add1, V_FARREN_Add2, V_FARREN_Iadd1, V_FARREN_Iadd2, V_FARREN_Phone,
        FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

        V_FARREN_ClientCD_EN :='0'; -- ENABLE FALSE
    END IF;

    IF PX_CUR_FORM = 'frmAR' and px_far_ar_type = 'P_NAME' THEN
            SELECT INV_CODE, INVESTOR_NAME, ADDRESS1, ADDRESS2,  ADDRESS1, ADDRESS2, PHONE, '' GUES_CD
            INTO V_FAR_Lbl_P_Code, V_FAR_Proposer, V_FAR_Add1, V_FAR_Add2, V_FAR_Phone, V_FAR_TguestCode
            FROM INVESTOR_MASTER = PX_INV_CODE;

            IF SUBSTR(PX_INV_CODE,1,1) <> '3' THEN
                V_FAR_TguestCode:=V_FAR_TguestCode;
            ELSE
                V_FAR_TguestCode:='';
            END IF;

    END IF;

    IF PX_CUR_FORM = 'frmAR_Renewal' AND PX_FARREN_AR_TYPE IS NULL THEN
        SELECT INV_CODE, INVESTOR_NAME, INVESTOR_NAME, ADDRESS1, ADDRESS2, PHONE
        INTO V_FARREN_LBL_P_CD, V_FARREN_Proposer, V_FARREN_Add1, V_FARREN_Add2, V_FARREN_Phone
        FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
    END IF;

    IF PX_CUR_FORM = 'frmAR' and px_far_ar_type = 'N_NAME' THEN
            SELECT INV_CODE, ''
            INTO V_FAR_Lbl_N_Code, V_FAR_NOMN
            FROM INVESTOR_MASTER = PX_INV_CODE; 
    END IF;

    IF PX_CUR_FORM = 'frmAR_Renewal' AND PX_FARREN_AR_TYPE ='N_NAME' THEN
        SELECT INV_CODE, ''
        INTO V_FARREN_LBL_N_CD, V_FARREN_NOM
        FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
    END IF;

    IF PX_CUR_FORM = 'FrmVenDayBook' THEN
        SELECT INV_CODE, INVESTOR_NAME
        INTO V_FVENDAYBOOK_CL_CD, V_FVENDAYBOOK_CL_NM
        FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
    END IF;

    IF PX_CUR_FORM = 'frmARGeneral' THEN
        SELECT INV_CODE, INVESTOR_NAME, ADDRESS1, ADDRESS2, ADDRESS1, ADDRESS2, PHONE INTO
        V_FARGEN_ClientCD,
        V_FARGEN_Proposer, 
        V_FARGEN_Add1, 
        V_FARGEN_Add2, 
        V_FARGEN_Iadd1, 
        V_FARGEN_Iadd2, 
        V_FARGEN_Phone
        FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

        V_FARGEN_ClientCD_EN:= '0';  
    
    END IF;

    IF PX_CUR_FORM = 'frm_MF_AUM_rpt' THEN
        V_FMFAUMRPT_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FMFAUMRPT_EXCD, V_FMFAUMRPT_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FMFAUMRPT_EXCD, V_FMFAUMRPT_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FMFAUMRPT_EXCD IS NULL THEN
            V_FMFAUMRPT_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FMFAUMRPT_NAME IS NOT NULL THEN
            V_FMFAUMRPT_CMBCLSBRRK:= V_FMFAUMRPT_NAME|| ' EXIST CODE- ' || V_FMFAUMRPT_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmtstatementrpt' THEN
        V_FSTMTRPT_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FSTMTRPT_EXCD, V_FSTMTRPT_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FSTMTRPT_EXCD, V_FSTMTRPT_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FSTMTRPT_EXCD IS NULL THEN
            V_FSTMTRPT_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FSTMTRPT_NAME IS NOT NULL THEN
            V_FSTMTRPT_CMBCLSBRRK:= V_FSTMTRPT_NAME|| ' EXIST CODE- ' || V_FSTMTRPT_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmtstatementrptop' THEN
        V_FSTMTRPTOP_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FSTMTRPTOP_EXCD, V_FSTMTRPTOP_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FSTMTRPTOP_EXCD, V_FSTMTRPTOP_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FSTMTRPTOP_EXCD IS NULL THEN
            V_FSTMTRPTOP_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FSTMTRPTOP_NAME IS NOT NULL THEN
            V_FSTMTRPTOP_CMBCLSBRRK:= V_FSTMTRPTOP_NAME|| ' EXIST CODE- ' || V_FSTMTRPTOP_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'FRM_FP_STATEMENT' THEN
        V_FFPSTMT_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FFPSTMT_EXCD, V_FFPSTMT_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FFPSTMT_EXCD, V_FFPSTMT_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FFPSTMT_EXCD IS NULL THEN
            V_FFPSTMT_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FFPSTMT_NAME IS NOT NULL THEN
            V_FFPSTMT_CMBCLSBRRK:= V_FFPSTMT_NAME|| ' EXIST CODE- ' || V_FFPSTMT_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'FRM_FP_STATEMENT' THEN
        V_FTSOPSTX_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FTSOPSTX_EXCD, V_FTSOPSTX_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FTSOPSTX_EXCD, V_FTSOPSTX_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FTSOPSTX_EXCD IS NULL THEN
            V_FTSOPSTX_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FTSOPSTX_NAME IS NOT NULL THEN
            V_FTSOPSTX_CMBCLSBRRK:= V_FTSOPSTX_NAME|| ' EXIST CODE- ' || V_FTSOPSTX_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmtstatemf_withstax' THEN
        V_FTSEMFWSTX_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FTSEMFWSTX_EXCD, V_FTSEMFWSTX_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FTSEMFWSTX_EXCD, V_FTSEMFWSTX_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FTSEMFWSTX_EXCD IS NULL THEN
            V_FTSEMFWSTX_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FTSEMFWSTX_NAME IS NOT NULL THEN
            V_FTSEMFWSTX_CMBCLSBRRK:= V_FTSEMFWSTX_NAME|| ' EXIST CODE- ' || V_FTSEMFWSTX_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmbussummery' THEN
        V_FBUSISUMRY_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FBUSISUMRY_EXCD, V_FBUSISUMRY_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FBUSISUMRY_EXCD, V_FBUSISUMRY_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FBUSISUMRY_EXCD IS NULL THEN
            V_FBUSISUMRY_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FBUSISUMRY_NAME IS NOT NULL THEN
            V_FBUSISUMRY_CMBCLSBRRK:= V_FBUSISUMRY_NAME|| ' EXIST CODE- ' || V_FBUSISUMRY_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmBrokerBill_Statement' THEN
        V_FBRKBILSTMT_EXCD:='';

        IF SUBSTR(PX_INV_CODE,1,1) = '3' THEN
            select exist_code, AGENT_NAME INTO V_FBRKBILSTMT_EXCD, V_FBRKBILSTMT_NAME from agent_master where agent_code= PX_INV_CODE;
        ELSE
            select exist_code, CLIENT_NAME INTO V_FBRKBILSTMT_EXCD, V_FBRKBILSTMT_NAME from client_master where client_code = PX_INV_CODE;
        END IF;

        IF V_FBRKBILSTMT_EXCD IS NULL THEN
            V_FBRKBILSTMT_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FBRKBILSTMT_NAME IS NOT NULL THEN
            V_FBRKBILSTMT_CMBCLSBRRK:= V_FBRKBILSTMT_NAME|| ' EXIST CODE- ' || V_FBRKBILSTMT_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmSubrokerSIPBillReport' THEN
        V_FSBRSIPBILRPT_EXCD:='';

        select exist_code, AGENT_NAME INTO V_FSBRSIPBILRPT_EXCD, V_FSBRSIPBILRPT_NAME from agent_master where agent_code= PX_INV_CODE;

        IF V_FSBRSIPBILRPT_EXCD IS NULL THEN
            V_FSBRSIPBILRPT_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FSBRSIPBILRPT_NAME IS NOT NULL THEN
            V_FSBRSIPBILRPT_CMBCLSBRRK:= V_FSBRSIPBILRPT_NAME|| ' EXIST CODE- ' || V_FSBRSIPBILRPT_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmANASummaryReport' THEN
        V_FANASUMRPT_EXCD:='';

        select exist_code, AGENT_NAME INTO V_FANASUMRPT_EXCD, V_FANASUMRPT_NAME from agent_master where agent_code= PX_INV_CODE;

        IF V_FANASUMRPT_EXCD IS NULL THEN
            V_FANASUMRPT_EXCD:= 'NOT EXIST';
        END IF;

        IF V_FANASUMRPT_NAME IS NOT NULL THEN
            V_FANASUMRPT_CMBCLSBRRK:= V_FANASUMRPT_NAME|| ' EXIST CODE- ' || V_FANASUMRPT_EXCD || PX_INV_CODE;
        END IF;
    END IF;

    IF PX_CUR_FORM NOT IN('frmAR', 'frmARGeneral', 'FrmVenDayBook' ,'frmAR_Renewal' ) THEN

        IF V_chkIndia = '0' THEN
            V_ALL_INDIA:= '0';
            V_preSelectedCode:='MFIBR';
            V_nodeValue:= PX_INV_CODE;

            IF PX_CUR_FORM = 'FrmtransactionNew' THEN
                IF PX_CAT = 'CLIENT' THEN
                    select rm_code INTO V_FTRN_RM_CD from client_master where client_code=PX_INV_CODE;
                END IF;
                IF PX_CAT = 'AGENT' THEN
                    select rm_code INTO V_FTRN_RM_CD from agent_master where agent_code=PX_INV_CODE;
                END IF;

                IF PX_CAT = 'INVESTOR' THEN
                    Select source_id INTO V_FTRN_SRC_CD from investor_master where inv_code=PX_INV_CODE;

                    IF SUBSTR(V_FTRN_SRC_CD,1,1) = '4' THEN
                        select rm_code INTO V_FTRN_RM_CD from client_master where client_code= V_FTRN_SRC_CD;

                        IF V_FTRN_RM_CD IS NOT NULL THEN
                            IF PX_CUR_FORM = 'FrmtransactionNew' THEN
                                V_FTRN_USER_CD := '';
                                V_FTRN_USER_TYPE:= 'INVESTOR';
                            END IF;
                        END IF;
                    END IF;

                    IF SUBSTR(V_FTRN_SRC_CD,1,1) = '3' THEN
                        select rm_code INTO V_FTRN_RM_CD from agent_master where agent_code=PX_INV_CODE;

                        IF V_FTRN_RM_CD IS NOT NULL THEN 
                            V_FTRN_RM_CD:= V_FTRN_RM_CD;
                        END IF;
                    END IF;
                END IF;
            END IF;
        ELSE
            V_ALL_INDIA:= '1';
            V_INDIA_INV_CODE:= PX_INV_CODE;
        END IF;

    END IF; 




END;