CREATE OR REPLACE PROCEDURE WEALTHMAKER.SP_GET_TRANSACTIONS_RTA_PRA(
    p_branch         IN VARCHAR2,
    p_status_type    IN VARCHAR2,
    p_tran_type      IN VARCHAR2,
    p_date_from      IN DATE,
    p_date_to        IN DATE,
    p_amc            IN VARCHAR2,
    p_cheque_type    IN VARCHAR2,
    p_cheque_search  IN VARCHAR2,
    p_inm            IN VARCHAR2,
    p_amt            IN VARCHAR2,
    o_transactions   OUT SYS_REFCURSOR
) AS
    v_query VARCHAR2(4000);
BEGIN
    -- Start building the dynamic SQL query
    v_query := 'SELECT
                    tmf.TRAN_CODE,
                    tmf.INV_NAME,
                    tmf.MUT_CODE,
                    ai.MUT_NAME,
                    tmf.TR_DATE,
                    tmf.TRAN_TYPE,
                    tmf.APP_NO,
                    tmf.CHEQUE_NO,
                    tmf.FOLIO_NO,
                    tmf.AMOUNT,
                    tmf.RMCODE,
                    tmf.SCH_CODE,
                    tmf.FLAG,
                    si.SCH_NAME,
                    bi.branch_name,  
                    tmf.REG_TRANTYPE,
                    tmf.busi_branch_code,
                    ei.rm_name,
                    i.ADDRESS1 AS ADDRESS,
                    c.CITY_NAME,
                    tmf.BROKER_ID,
                    tmf.UNQ_KEY
                FROM
                    WEALTHMAKER_MF.TRANSACTION_ST tmf
                LEFT JOIN
                    scheme_info si ON tmf.SCH_CODE = si.SCH_CODE
                LEFT JOIN
                    MUT_FUND ai ON tmf.MUT_CODE = ai.MUT_CODE
                LEFT JOIN
                    branch_master bi ON tmf.BUSI_BRANCH_CODE = bi.branch_code
                LEFT JOIN
                    AGENT_MASTER pi ON tmf.source_code = pi.agent_code
                LEFT JOIN
                    employee_master ei ON tmf.rmcode = ei.rm_code
                LEFT JOIN
                    INVESTOR_MASTER i ON tmf.CLIENT_CODE = i.INV_CODE
                LEFT JOIN
                    CITY_MASTER c ON i.CITY_ID = c.CITY_ID
                WHERE 1 = 1
                
                 
                AND upper(TMF.REG_TRANTYPE) NOT like ''%REVERSAL%''
                AND upper(TMF.REG_TRANTYPE) NOT like ''%REJECTION%''
                
                
                ';

    -- Dynamically add conditions
     IF p_tran_type IS NOT NULL THEN
        v_query := v_query || ' AND tmf.tran_type = ''' || p_tran_type || '''';
    END IF;

    IF p_status_type IS NOT NULL THEN
        IF UPPER(p_status_type) = 'Y' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''Y''';
        ELSIF UPPER(p_status_type) = 'N' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''N''';
        END IF;
    END IF;

     IF p_branch IS NOT NULL THEN
        v_query := v_query || ' AND tmf.busi_branch_code = ''' || p_branch || '''';
    END IF;

      IF P_CHEQUE_TYPE IS NOT NULL AND P_CHEQUE_SEARCH IS NOT NULL THEN
        v_query := v_query || ' AND (';
        IF P_CHEQUE_TYPE = '001' THEN
            v_query := v_query || ' tmf.CHEQUE_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '002' THEN
            v_query := v_query || ' tmf.FOLIO_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '003' THEN
            v_query := v_query || ' tmf.APP_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '004' THEN
            v_query := v_query || ' tmf.PAN1 = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '005' THEN
            v_query := v_query || ' pi.EXIST_CODE = ''' || P_CHEQUE_SEARCH || '''';
        END IF;
        v_query := v_query || ')';
    END IF;

    IF p_date_from IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TR_DATE >= TO_DATE(''' || TO_CHAR(p_date_from, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;

    IF p_date_to IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TR_DATE <= TO_DATE(''' || TO_CHAR(p_date_to, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;

       IF p_inm IS NOT NULL THEN
        v_query := v_query || ' AND tmf.inv_name = ''' || p_inm || '''';
    END IF;

     IF p_amc IS NOT NULL THEN
        v_query := v_query || ' AND tmf.MUT_CODE = ''' || p_amc || '''';
    END IF;

    IF p_amt IS NOT NULL THEN
        v_query := v_query || ' AND tmf.amount = ''' || p_amt || '''';
    END IF;


    -- Open the cursor with the final query
    OPEN o_transactions FOR v_query;
END SP_GET_TRANSACTIONS_RTA_PRA;
/
