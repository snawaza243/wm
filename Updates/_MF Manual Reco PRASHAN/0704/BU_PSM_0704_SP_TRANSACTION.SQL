/*MF MANUAL: RTA SEARCH BY SEARCHED AR */

CREATE OR REPLACE PROCEDURE WEALTHMAKER.SP_SEARCH_TRANSACTIONS_PRA (
    P_DATE_FROM       IN DATE,
    P_DATE_TO         IN DATE,
    P_STATUS          IN Char,
    P_AMC             IN VARCHAR2,
    P_BRANCH          IN VARCHAR2,
    P_CHEQUE_TYPE     IN VARCHAR2,
    P_CHEQUE_SEARCH   IN VARCHAR2,
    P_INVESTOR_NAME   IN VARCHAR2,
    P_AMOUNT          IN VARCHAR2,
    P_TRAN_TYPE       IN VARCHAR2,
    P_SEARCH_TEXT     IN VARCHAR2,
    P_CURSOR          OUT SYS_REFCURSOR
) AS
    V_QUERY VARCHAR2(4000);
BEGIN
    -- Start building the base query
    V_QUERY := 'SELECT 
                    tmf.TRAN_CODE, 
                    tmf.INV_NAME, 
                    tmf.UNQ_KEY,
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
                    tmf.FLAG, 
                    tmf.BROKER_ID, 
                    tmf.REG_TRANTYPE, 
                    pi.EXIST_CODE, 
                    i.ADDRESS1, 
                    tmf.BUSINESS_RMCODE,
                    c.CITY_NAME 
                FROM 
                    WEALTHMAKER_MF.transaction_st tmf
                LEFT JOIN 
                    scheme_info si ON tmf.SCH_CODE = si.SCH_CODE
                LEFT JOIN 
                    MUT_FUND ai ON tmf.MUT_CODE = ai.MUT_CODE
                LEFT JOIN 
                    BRANCH_MASTER ti ON tmf.BRANCH_CODE = ti.BRANCH_CODE
                LEFT JOIN 
                    EMPLOYEE_MASTER li ON tmf.RMCODE = li.RM_CODE
                LEFT JOIN 
                    AGENT_MASTER pi ON tmf.source_code = pi.agent_code
                LEFT JOIN 
                    INVESTOR_MASTER i ON tmf.CLIENT_CODE = i.INV_CODE
                LEFT JOIN 
                    CITY_MASTER c ON i.CITY_ID = c.CITY_ID 
                WHERE 1 = 1';  -- Base WHERE clause to easily append conditions

    -- Add dynamic conditions
    IF P_DATE_FROM IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.TR_DATE >= TO_DATE(''' || TO_CHAR(P_DATE_FROM, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;

    IF P_DATE_TO IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.TR_DATE <= TO_DATE(''' || TO_CHAR(P_DATE_TO, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;

    IF P_STATUS IS NOT NULL THEN
        IF UPPER(P_STATUS) = 'Y' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''Y''';
        ELSIF UPPER(P_STATUS) = 'N' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''N''';
        END IF;
    END IF;

    IF P_AMC IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.MUT_CODE = ''' || P_AMC || '''';
    END IF;

    IF P_BRANCH IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.BRANCH_CODE = ''' || P_BRANCH || '''';
    END IF;

    IF P_CHEQUE_TYPE IS NOT NULL AND P_CHEQUE_SEARCH IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND (';
        IF P_CHEQUE_TYPE = '001' THEN
            V_QUERY := V_QUERY || ' tmf.CHEQUE_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '002' THEN
            V_QUERY := V_QUERY || ' tmf.FOLIO_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '003' THEN
            V_QUERY := V_QUERY || ' tmf.APP_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '004' THEN
            V_QUERY := V_QUERY || ' tmf.PAN1 = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '005' THEN
            V_QUERY := V_QUERY || ' pi.EXIST_CODE = ''' || P_CHEQUE_SEARCH || '''';
        END IF;
        V_QUERY := V_QUERY || ')';
    END IF;

    IF P_INVESTOR_NAME IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.INV_NAME = ''' || P_INVESTOR_NAME || '''';
    END IF;

    IF P_AMOUNT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.AMOUNT = ' || P_AMOUNT;
    END IF;

    IF P_SEARCH_TEXT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.TRAN_CODE LIKE ''%' || P_SEARCH_TEXT || '%''';
    END IF;

     IF P_TRAN_TYPE IS NOT NULL THEN
        IF UPPER(P_TRAN_TYPE) = 'REGULAR' THEN
            V_QUERY := V_QUERY || ' AND ((UPPER(tmf.REG_TRANTYPE) NOT LIKE ''%SYS%'' 
                                       AND UPPER(tmf.REG_TRANTYPE) NOT LIKE ''%SIP%'' 
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%S T P IN%'' 
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ%'' 
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ REVERSAL%'' 
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%STP SWITCH IN%'' 
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%STPI%'' 
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%STPIR%'') 
                                       OR tmf.REG_TRANTYPE IS NULL)';
        ELSIF UPPER(P_TRAN_TYPE) = 'SIP' THEN
            V_QUERY := V_QUERY || ' AND ((UPPER(tmf.REG_TRANTYPE) LIKE ''%SYS%'' 
                                       OR UPPER(tmf.REG_TRANTYPE) LIKE ''%SIP%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%S T P IN%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%S T P IN REJ%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%S T P IN REJ REVERSAL%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%STP SWITCH IN%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%STPI%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%STPIR%''))';
        END IF;
    END IF;

    -- Append the ORDER BY clause before opening the cursor
    V_QUERY := V_QUERY || ' ORDER BY tmf.TR_DATE';

    -- Open the cursor with the dynamically built query
    OPEN P_CURSOR FOR V_QUERY;
END SP_SEARCH_TRANSACTIONS_PRA;
/
