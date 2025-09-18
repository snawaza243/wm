CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSMM_INV_SEARCH_ROW_DB_CLICK (
    PX_CUR_FORM             IN  VARCHAR2,
    PX_INV_CODE             IN  VARCHAR2,
    PX_INDEX                IN  VARCHAR2,
    PX_STR_FORM             IN  VARCHAR2,
    PX_FCT_CD_CODE          IN  VARCHAR2,
    PX_CAT                  IN  VARCHAR2,
    PX_FIM_CMB_CLIENT       IN  VARCHAR2,
    PX_FCIR_CD              IN  VARCHAR2,
    PX_CM_INVESTORS         IN  VARCHAR2,
    PX_CM_BRANCH_NAMES      IN  VARCHAR2,
    PX_INV_BRANCH_NAME      IN  VARCHAR2,
    PX_FAR_AR_TYPE          IN  VARCHAR2,
    PX_FAR_AR_MY_OPT        IN  VARCHAR2,
    PX_FARREN_AR_TYPE       IN  VARCHAR2,
    PX_FPAY_CL_CD           IN  VARCHAR2,
    PX_FJV_CL_CD            IN  VARCHAR2,
    PX_FJV_AG_CD            IN  VARCHAR2,
    PX_FSYNC_REV_TR         IN  VARCHAR2,
    PX_FRM_SYNC_TM1         IN  VARCHAR2,
    PX_FRM_SYNC_TM10        IN  VARCHAR2,
    PX_FRM_SYNC_TM12        IN  VARCHAR2,
    V_chkIndia              IN  VARCHAR2,
    PX_CURSOR               OUT SYS_REFCURSOR
    
) AS

    v_fct_cd_new_inv            VARCHAR2(12);
    v_fim_found                 VARCHAR2(1);
    V_FRM_FP_INV_CD             VARCHAR2(100);
    V_ROLE_NAME                 VARCHAR2(100);
    V_FRM_UPRCSLB_INV_NM        VARCHAR2(100);
    V_FRM_SYNC_NODE_V           VARCHAR2(100);
    V_FRM_SYNC_MSG              VARCHAR2(100);
    V_FRM_SYNC_ENDPOS           VARCHAR2(100);
    V_FRM_SYNC_TM12             VARCHAR2(100);
    V_FRM_SYNC_TM2              VARCHAR2(100);
    V_FRM_FP_INV_NAME           VARCHAR2(100);
    V_GLB_DATA_FILTER           VARCHAR2(100);
    V_LOG_USER_ID               VARCHAR2(100);
    V_MAIN_CODE                 VARCHAR2(100);
    V_UPD_PROC                  VARCHAR2(100);
    V_FCT_CS_NEW_RM_NAME        VARCHAR2(100);
    V_FCT_CS_NEW_RM_CODE        VARCHAR2(100);
    V_px_fim_new_cmb_cliend     VARCHAR2(100);
    V_px_FCIR_NEW_INV           VARCHAR2(100);
    V_px_FCIR_RM_CODE           VARCHAR2(100);
    V_px_FCIR_RM_NAME           VARCHAR2(100);
    V_CM_MAX_ROW_cnt            NUMBER;
    V_CM_LAST_TR                VARCHAR2(100);
    V_CM_PINCODE                VARCHAR2(100);
    V_CM_CREATION_DT            DATE;
    V_MFA_INV_CODE              VARCHAR2(100);
    V_MFA_CLIENT_CODE           VARCHAR2(100);
    V_MFA_INVESTOR_NAME         VARCHAR2(100);
    V_MFA_PAN                   VARCHAR2(100);
    V_MFA_AH_CODE               VARCHAR2(100);
    V_MFA_BUSI_CODE             VARCHAR2(100);
    V_MFM_INV_CODE              VARCHAR2(100);
    V_MFM_CLIENT_CODE           VARCHAR2(100);
    V_MFM_INVESTOR_NAME         VARCHAR2(100);
    V_MFM_PAN                   VARCHAR2(100);
    V_MFM_AH_CODE               VARCHAR2(100);
    V_MFM_BUSI_CODE             VARCHAR2(100);
    V_MF_LABEL42                VARCHAR2(100);
    V_FIPO_INV_CD               VARCHAR2(100);
    V_FIPO_INV_NAME             VARCHAR2(100);
    V_FIPO_INV_TYPE             VARCHAR2(100);
    V_FIOP_DP_ID                VARCHAR2(100);
    V_FIOP_DP_NAME              VARCHAR2(100);
    V_FIPO_CLIENT               VARCHAR2(100);
    V_FIPO_DP_TYPE              VARCHAR2(100);
    V_FIPO_OPT_NSDL             VARCHAR2(100);
    V_FRM_FP_FDT                VARCHAR2(100);
    V_FRM_FP_TDT                VARCHAR2(100);
    V_FRM_FP_cmbstatus          VARCHAR2(100);
    V_FRM_FP_FAMHEAD            VARCHAR2(100);
    V_FRM_FP_FAMHEADNM          VARCHAR2(100);
    V_FRM_FP_TEMP1              VARCHAR2(100);
    V_FRM_FP_TEMP2              VARCHAR2(100);
    V_FRM_FP_TEMP3              VARCHAR2(100);
    V_FRM_FP_LIST_INV_NEW       VARCHAR2(100);
    V_FRM_FP_IMP_DT             VARCHAR2(100);
    V_FRM_FP_AUDIT_DT           VARCHAR2(100);
    V_FRM_FP_AUDIT_CHK          VARCHAR2(100);
    V_FRM_FP_AUDIT_CHK_ENABLE   VARCHAR2(100);
    V_FRM_FP_AUDIT_ENABLE       VARCHAR2(100);
    V_FRM_FP_cmbstatus_EN       VARCHAR2(100);
    V_FRM_FP_LIST_INV_SELECT    VARCHAR2(100);
    V_FRM_FP_FDT_EN             VARCHAR2(100);
    V_FRM_UPRCSLB_EX_CD         VARCHAR2(100);
    V_FRM_UPRCSLB_txtCD         VARCHAR2(100);
    V_FRM_UPRCSLB_txtAgCode     VARCHAR2(100);
    V_FRM_UPRCSLB_cmbMutFund1_EN    VARCHAR2(100);
    V_FRM_UPRCSLB_lstlongname1_EN   VARCHAR2(100);
    V_FRM_UPRCSLB_lstSch1_EN        VARCHAR2(100);
    V_FRM_PAY_INV_CD            VARCHAR2(100);
    V_FRM_PAY_INV_NM            VARCHAR2(100);
    V_FRM_JV_INV_CD             VARCHAR2(100);
    V_FRM_JV_INV_NM             VARCHAR2(100);
    V_FAR_FrmConfirmBoth_VSB    VARCHAR2(100);
    V_FAR_txtInsured1           VARCHAR2(100);
    V_FAR_txtProposer1          VARCHAR2(100);
    V_FAR_ClientCD              VARCHAR2(100);
    V_FAR_Insured               VARCHAR2(100);
    V_FAR_Proposer              VARCHAR2(100);
    V_FAR_Add1                  VARCHAR2(100);
    V_FAR_Add2                  VARCHAR2(100);
    V_FAR_Iadd1                 VARCHAR2(100);
    V_FAR_Iadd2                 VARCHAR2(100);
    V_FAR_Phone                 VARCHAR2(100);
    V_FAR_TguestCode            VARCHAR2(100);
    V_FAR_ClientCD_EN           VARCHAR2(100);
    V_FARREN_ClientCD           VARCHAR2(100);
    V_FARREN_LBL_P_CD           VARCHAR2(100);
    V_FARREN_Insured            VARCHAR2(100);
    V_FARREN_Proposer           VARCHAR2(100);
    V_FARREN_Add1               VARCHAR2(100);
    V_FARREN_Add2               VARCHAR2(100);
    V_FARREN_Iadd1              VARCHAR2(100);
    V_FARREN_Iadd2              VARCHAR2(100);
    V_FARREN_Phone              VARCHAR2(100);
    V_FARREN_ClientCD_EN        VARCHAR2(100);
    V_FAR_Lbl_P_Code            VARCHAR2(100);
    V_FAR_Lbl_N_Code            VARCHAR2(100);
    V_FAR_NOMN                  VARCHAR2(100);
    V_FARREN_LBL_N_CD           VARCHAR2(100);
    V_FARREN_NOM                VARCHAR2(100);
    V_FVENDAYBOOK_CL_CD         VARCHAR2(100);
    V_FVENDAYBOOK_CL_NM         VARCHAR2(100);
    V_FARGEN_ClientCD           VARCHAR2(100);
    V_FARGEN_Proposer           VARCHAR2(100);
    V_FARGEN_Add1               VARCHAR2(100);
    V_FARGEN_Add2               VARCHAR2(100);
    V_FARGEN_Iadd1              VARCHAR2(100);
    V_FARGEN_Iadd2              VARCHAR2(100);
    V_FARGEN_Phone              VARCHAR2(100);
    V_FARGEN_ClientCD_EN        VARCHAR2(100);
    V_FMFAUMRPT_EXCD            VARCHAR2(100);
    V_FMFAUMRPT_NAME            VARCHAR2(100);
    V_FMFAUMRPT_CMBCLSBRRK      VARCHAR2(100);
    V_FSTMTRPT_EXCD             VARCHAR2(100);
    V_FSTMTRPT_NAME             VARCHAR2(100);
    V_FSTMTRPT_CMBCLSBRRK       VARCHAR2(100);
    V_FSTMTRPTOP_EXCD           VARCHAR2(100);
    V_FSTMTRPTOP_NAME           VARCHAR2(100);
    V_FSTMTRPTOP_CMBCLSBRRK     VARCHAR2(100);
    V_FFPSTMT_EXCD              VARCHAR2(100);
    V_FFPSTMT_NAME              VARCHAR2(100);
    V_FFPSTMT_CMBCLSBRRK        VARCHAR2(100);
    V_FTSOPSTX_EXCD             VARCHAR2(100);
    V_FTSOPSTX_NAME             VARCHAR2(100);
    V_FTSOPSTX_CMBCLSBRRK       VARCHAR2(100);
    V_FTSEMFWSTX_EXCD           VARCHAR2(100);
    V_FTSEMFWSTX_NAME           VARCHAR2(100);
    V_FTSEMFWSTX_CMBCLSBRRK     VARCHAR2(100);
    V_FBUSISUMRY_EXCD           VARCHAR2(100);
    V_FBUSISUMRY_NAME           VARCHAR2(100);
    V_FBUSISUMRY_CMBCLSBRRK     VARCHAR2(100);
    V_FBRKBILSTMT_EXCD          VARCHAR2(100);
    V_FBRKBILSTMT_NAME          VARCHAR2(100);
    V_FBRKBILSTMT_CMBCLSBRRK    VARCHAR2(100);
    V_FSBRSIPBILRPT_EXCD        VARCHAR2(100);
    V_FSBRSIPBILRPT_NAME        VARCHAR2(100);
    V_FSBRSIPBILRPT_CMBCLSBRRK  VARCHAR2(100);
    V_FANASUMRPT_EXCD           VARCHAR2(100);
    V_FANASUMRPT_NAME           VARCHAR2(100);
    V_FANASUMRPT_CMBCLSBRRK     VARCHAR2(100);
    V_ALL_INDIA                 VARCHAR2(100);
    V_preSelectedCode           VARCHAR2(100);
    V_nodeValue                 VARCHAR2(100);
    V_INDIA_INV_CODE            VARCHAR2(100);
    V_FTRN_RM_CD                VARCHAR2(100);
    V_FTRN_SRC_CD               VARCHAR2(100);
    V_FTRN_USER_CD              VARCHAR2(100);
    V_FTRN_USER_TYPE            VARCHAR2(100);

BEGIN
    v_fim_found := '0';

IF PX_CUR_FORM = 'frmtransactionmf' THEN
    BEGIN
        -- Get user ID and main code in a single query
        SELECT loggeduserid, main_code, NVL(UPD_PROC, 'N')
        INTO V_LOG_USER_ID, V_MAIN_CODE, V_UPD_PROC
        FROM client_test 
        WHERE client_codekyc = PX_INV_CODE;
        
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            OPEN PX_CURSOR FOR 
                SELECT 'Client not found in client_test table for code: ' || PX_INV_CODE AS MSG 
                FROM DUAL;
            RETURN;
        WHEN TOO_MANY_ROWS THEN
            OPEN PX_CURSOR FOR 
                SELECT 'Multiple clients found in client_test table for code: ' || PX_INV_CODE AS MSG 
                FROM DUAL;
            RETURN;
    END;

    IF V_LOG_USER_ID = 'PROC' AND V_UPD_PROC IN ('N', '0') THEN
        OPEN PX_CURSOR FOR 
            SELECT 'Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account (Main Code):' || 
                   V_MAIN_CODE || '#OPEN_POPUP#AO' AS MSG 
            FROM DUAL;
        RETURN;
    END IF;

    -- Process based on index value
    IF PX_INDEX = '0' THEN
        BEGIN
            SELECT INV_CODE, SUBSTR(INV_CODE, 1, 8), INVESTOR_NAME, PAN
            INTO V_MFA_INV_CODE, V_MFA_CLIENT_CODE, V_MFA_INVESTOR_NAME, V_MFA_PAN
            FROM INVESTOR_MASTER 
            WHERE INV_CODE = PX_INV_CODE;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                OPEN PX_CURSOR FOR 
                    SELECT 'Investor not found in INVESTOR_MASTER for code: ' || PX_INV_CODE AS MSG 
                    FROM DUAL;
                RETURN;
            WHEN TOO_MANY_ROWS THEN
                OPEN PX_CURSOR FOR 
                    SELECT 'Multiple investors found in INVESTOR_MASTER for code: ' || PX_INV_CODE AS MSG 
                    FROM DUAL;
                RETURN;
        END;

        IF SUBSTR(PX_INV_CODE, 1, 1) = '4' THEN
            BEGIN
                -- Get client code
                SELECT client_code 
                INTO V_MFA_AH_CODE 
                FROM client_test 
                WHERE client_codekyc = PX_INV_CODE;
                
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    V_MFA_AH_CODE := NULL;
                WHEN TOO_MANY_ROWS THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Multiple client records found for code: ' || PX_INV_CODE AS MSG 
                        FROM DUAL;
                    RETURN;
            END;

            BEGIN
                -- Get business code
                SELECT payroll_id 
                INTO V_MFA_BUSI_CODE 
                FROM employee_master 
                WHERE rm_code = (SELECT rm_code 
                                FROM client_master 
                                WHERE client_code = SUBSTR(PX_INV_CODE, 1, 8));
                                
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Business code not found for client code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
                WHEN TOO_MANY_ROWS THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Multiple business codes found for client code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
            END;

            IF V_MFA_AH_CODE IS NULL OR LENGTH(V_MFA_AH_CODE) < 6 THEN
                OPEN PX_CURSOR FOR 
                    SELECT 'Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same' AS MSG 
                    FROM DUAL;
                RETURN;
            END IF;
        ELSE
            BEGIN
                -- For non-client accounts
                SELECT payroll_id 
                INTO V_MFA_BUSI_CODE 
                FROM employee_master 
                WHERE rm_code = (SELECT rm_code 
                                FROM agent_master 
                                WHERE agent_code = SUBSTR(PX_INV_CODE, 1, 8));
                                
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Business code not found for agent code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
                WHEN TOO_MANY_ROWS THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Multiple business codes found for agent code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
            END;
        END IF;
        
    ELSIF PX_INDEX = '1' THEN
        BEGIN
            SELECT INV_CODE, SUBSTR(INV_CODE, 1, 8), INVESTOR_NAME, PAN
            INTO V_MFM_INV_CODE, V_MFM_CLIENT_CODE, V_MFM_INVESTOR_NAME, V_MFM_PAN
            FROM INVESTOR_MASTER
            WHERE INV_CODE = PX_INV_CODE;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                OPEN PX_CURSOR FOR 
                    SELECT 'Investor not found in INVESTOR_MASTER for code: ' || PX_INV_CODE AS MSG 
                    FROM DUAL;
                RETURN;
            WHEN TOO_MANY_ROWS THEN
                OPEN PX_CURSOR FOR 
                    SELECT 'Multiple investors found in INVESTOR_MASTER for code: ' || PX_INV_CODE AS MSG 
                    FROM DUAL;
                RETURN;
        END;

        IF SUBSTR(PX_INV_CODE, 1, 1) = '4' THEN
            BEGIN
                SELECT e.payroll_id, c.client_code
                INTO V_MFM_BUSI_CODE, V_MFM_AH_CODE
                FROM employee_master e, client_test c
                WHERE e.rm_code = (SELECT rm_code 
                                  FROM client_master 
                                  WHERE client_code = SUBSTR(PX_INV_CODE, 1, 8))
                AND c.client_codekyc = PX_INV_CODE;
                
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Data not found for client code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
                WHEN TOO_MANY_ROWS THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Multiple records found for client code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
            END;
        ELSE
            BEGIN
                SELECT im.PAN, ct.client_code, em.payroll_id
                INTO V_MFA_PAN, V_MFM_AH_CODE, V_MFM_BUSI_CODE
                FROM INVESTOR_MASTER im, client_test ct, employee_master em
                WHERE im.INV_CODE = PX_INV_CODE
                AND ct.client_codekyc = PX_INV_CODE
                AND em.rm_code = (SELECT rm_code 
                                FROM agent_master 
                                WHERE agent_code = SUBSTR(PX_INV_CODE, 1, 8));
                                
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Data not found for agent code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
                WHEN TOO_MANY_ROWS THEN
                    OPEN PX_CURSOR FOR 
                        SELECT 'Multiple records found for agent code: ' || SUBSTR(PX_INV_CODE, 1, 8) AS MSG 
                        FROM DUAL;
                    RETURN;
            END;
        END IF;
        
        V_MF_LABEL42 := PX_INV_CODE;
    END IF;
    
    -- Return all variables via cursor
    OPEN PX_CURSOR FOR
    SELECT 
        'SUCCESS:' AS MSG,
        V_LOG_USER_ID AS V_LOG_USER_ID,
        V_MAIN_CODE AS V_MAIN_CODE,
        V_UPD_PROC AS V_UPD_PROC,
        V_MFA_INV_CODE AS V_MFA_INV_CODE,
        V_MFA_CLIENT_CODE AS V_MFA_CLIENT_CODE,
        V_MFA_INVESTOR_NAME AS V_MFA_INVESTOR_NAME,
        V_MFA_PAN AS V_MFA_PAN,
        V_MFA_AH_CODE AS V_MFA_AH_CODE,
        V_MFA_BUSI_CODE AS V_MFA_BUSI_CODE,
        V_MFM_INV_CODE AS V_MFM_INV_CODE,
        V_MFM_CLIENT_CODE AS V_MFM_CLIENT_CODE,
        V_MFM_INVESTOR_NAME AS V_MFM_INVESTOR_NAME,
        V_MFM_PAN AS V_MFM_PAN,
        V_MFM_AH_CODE AS V_MFM_AH_CODE,
        V_MFM_BUSI_CODE AS V_MFM_BUSI_CODE,
        V_MF_LABEL42 AS V_MF_LABEL42,
        PX_INV_CODE AS PX_INV_CODE,
        PX_INDEX AS PX_INDEX,
        PX_CUR_FORM AS PX_CUR_FORM
    FROM DUAL;
    
END IF;

    /*Note: 
        If currentForm.Name = "FrmtransactionNew" Then
        Call FrmtransactionNew.Lbl_LeadSearch_Click
        ElseIf currentForm.Name = "frmAR" Then
        Call frmAR.Lbl_LeadSearch_Click
        ElseIf currentForm.Name = "frmARGeneral" Then
        Call frmARGeneral.Lbl_LeadSearch_Click
        End If
    */

END;
/