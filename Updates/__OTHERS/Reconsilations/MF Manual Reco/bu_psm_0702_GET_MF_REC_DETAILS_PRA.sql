CREATE OR REPLACE PROCEDURE WEALTHMAKER.GET_MF_REC_DETAILS_PRA (
    p_Region                IN BRANCH_MASTER.REGION_ID%TYPE,
    p_Zone                  IN BRANCH_MASTER.ZONE_ID%TYPE,
    p_Branch                IN BRANCH_MASTER.BRANCH_CODE%TYPE,
    p_RM                    IN transaction_mf_temp1.RMCODE%TYPE,
    p_FromDate              IN transaction_mf_temp1.TR_DATE%TYPE,
    p_ToDate                IN transaction_mf_temp1.TR_DATE%TYPE,
    p_AMC                   IN transaction_mf_temp1.MUT_CODE%TYPE,
    p_AR                    IN transaction_mf_temp1.TRAN_CODE%TYPE,
    p_ReconciliationStatus  IN CHAR,
    p_COB                   IN CHAR,
    p_ARNo                  IN transaction_mf_temp1.TRAN_CODE%TYPE,
    p_TranType              IN transaction_mf_temp1.TRAN_TYPE%TYPE,
    p_Registrar             IN VARCHAR2,
    p_cursor                OUT SYS_REFCURSOR
) AS
    v_query VARCHAR2(4000);
    v_reg   VARCHAR2(30);
BEGIN
    -- Start building the query
    v_query := 'SELECT 
                      CASE 
                      WHEN tmf.LOGGEDUSERID = ''MFONLINE'' THEN ''Online'' 
                      WHEN tmf.LOGGEDUSERID = ''Valuefy'' THEN ''Online'' 
                         ELSE ''Offline'' 
                      END AS FLAG,
                    tmf.TRAN_CODE, 
                    tmf.PANNO,
                    tmf.BROKER_ID,
                    tmf.INVESTOR_NAME, 
                    tmf.SCH_CODE, 
                    si.SCH_NAME, 
                    tmf.MUT_CODE, 
                    ai.MUT_NAME, 
                    tmf.TRAN_TYPE, 
                    tmf.APP_NO, 
                    tmf.APP_DATE, 
                    tmf.CHEQUE_NO, 
                    tmf.CHEQUE_DATE, 
                    tmf.AMOUNT, 
                    tmf.SIP_TYPE, 
                    tmf.PAYMENT_MODE, 
                    tmf.BANK_NAME, 
                    tmf.FOLIO_NO, 
                    tmf.RMCODE, 
                    li.RM_NAME, 
                    tmf.INVESTOR_TYPE, 
                    tmf.BRANCH_CODE, 
                    ti.BRANCH_NAME, 
                    tmf.TR_DATE, 
                    tmf.UNITS, 
                    tmf.NAV_DATE, 
                    tmf.REMARK, 
                    tmf.Sip_Amount, 
                    tmf.remark_reco,
                    tmf.BROKER_ID, 
                    mi.REG_TRANTYPE, 
                    tmf.UNIQUE_KEY, 
                    pi.exist_code, 
                    i.ADDRESS1, 
                    c.CITY_NAME, 
                    NVL(tmf.cob_flag, 0) AS cob_flag,
                    -- Inline logic for FN_IDENTIFY_REGISTRAR
                    (SELECT REG 
                     FROM WEALTHMAKER.REG_SCHEMES 
                     WHERE MUT_CODE = tmf.MUT_CODE 
                     AND ROWNUM = 1) AS REGISTRAR
                FROM 
                    transaction_mf_temp1 tmf
                LEFT JOIN ALLSCHEME si ON tmf.SCH_CODE = si.SCH_CODE
                LEFT JOIN MUT_FUND ai ON tmf.MUT_CODE = ai.MUT_CODE
                LEFT JOIN BRANCH_MASTER ti ON tmf.BRANCH_CODE = ti.BRANCH_CODE
                LEFT JOIN EMPLOYEE_MASTER li ON tmf.RMCODE = li.RM_CODE
                LEFT JOIN AGENT_MASTER pi ON tmf.source_code = pi.agent_code
                LEFT JOIN INVESTOR_MASTER i ON tmf.CLIENT_CODE = i.INV_CODE
                LEFT JOIN CITY_MASTER c ON i.CITY_ID = c.CITY_ID
                LEFT JOIN WEALTHMAKER_MF.TRANSACTION_ST mi ON tmf.tran_code = mi.tran_code
                WHERE 1 = 1  AND tmf.TRAN_TYPE != ''REVERTAL'' ';

    -- Add dynamic conditions
    IF p_Region IS NOT NULL THEN
        v_query := v_query || ' AND ti.REGION_ID = ''' || p_Region || '''';
    END IF;

    IF p_Zone IS NOT NULL THEN
        v_query := v_query || ' AND ti.ZONE_ID = ''' || p_Zone || '''';
    END IF;

    IF p_Branch IS NOT NULL THEN
        v_query := v_query || ' AND tmf.BRANCH_CODE = ''' || p_Branch || '''';
    END IF;

    IF p_RM IS NOT NULL THEN
        v_query := v_query || ' AND tmf.RMCODE = ''' || p_RM || '''';
    END IF;

    IF p_FromDate IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TR_DATE >= TO_DATE(''' || TO_CHAR(p_FromDate, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;

    IF p_ToDate IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TR_DATE <= TO_DATE(''' || TO_CHAR(p_ToDate, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;

    IF p_AMC IS NOT NULL THEN
        v_query := v_query || ' AND tmf.MUT_CODE = ''' || p_AMC || '''';
    END IF;

    IF p_AR IS NOT NULL THEN
        v_query := v_query || ' AND tmf.INVESTOR_NAME LIKE ''%' || p_AR || '%''';
    END IF;

    IF p_ReconciliationStatus IS NOT NULL THEN
        IF UPPER(p_ReconciliationStatus) = 'Y' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''Y''';
        ELSIF UPPER(p_ReconciliationStatus) = 'N' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''N''';
        END IF;
    END IF;

    IF p_COB = '1' THEN
        v_query := v_query || ' AND tmf.cob_flag = ''1''';
    END IF;

    IF p_ARNo IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TRAN_CODE = ''' || p_ARNo || '''';
    END IF;

    IF p_TranType IS NOT NULL THEN
        IF UPPER(p_TranType) = 'REGULAR' THEN
            v_query := v_query || ' AND (tmf.SIP_TYPE = ''REGULAR'' OR tmf.SIP_TYPE IS NULL)';
        ELSIF UPPER(p_TranType) = 'SIP' THEN
            v_query := v_query || ' AND tmf.DISPATCH = ''N''';
        ELSIF UPPER(p_TranType) = 'PMS' THEN
            v_query := v_query || ' AND si.prd = ''DT027''';
        ELSIF UPPER(p_TranType) = 'ATM' THEN
            v_query := v_query || ' AND NVL(tmf.ATM_FLAG, ''0'') = ''1''';
        ELSIF UPPER(p_TranType) = 'TRAIL' THEN
            v_query := v_query || ' AND get_scheme_nature_trail_NEW(tmf.SCH_CODE, tmf.TR_DATE) = ''T''';
        END IF;
    END IF;

    V_QUERY := V_QUERY || ' ORDER BY tmf.TR_DATE';

    -- Open the cursor with the dynamically constructed query
    OPEN p_cursor FOR v_query;
END GET_MF_REC_DETAILS_PRA;
/
