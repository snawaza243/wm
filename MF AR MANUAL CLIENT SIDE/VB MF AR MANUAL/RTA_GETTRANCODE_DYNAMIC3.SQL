CREATE OR REPLACE FUNCTION WEALTHMAKER.GETTRANCODE_DYNAMIC (
    P_LOG_ID         IN VARCHAR2,
    P_ROLE_ID        IN VARCHAR2,
    P_BRANCH         IN VARCHAR2,
    P_AMC            IN VARCHAR2,
    P_MUT_CODE       IN VARCHAR2,
    P_RECONCILE      IN VARCHAR2, -- 'Y', 'N', or NULL
    P_TRAN_TYPE      IN VARCHAR2, -- 'REGULAR', 'SIP', or NULL
    P_FROM_DATE      IN DATE,
    P_TO_DATE        IN DATE,
    P_CHEQUE_TYPE    IN VARCHAR2, -- '001', '002', '003', '004', '005'
    P_CHEQUE_SEARCH  IN VARCHAR2,
    P_INVESTOR_NAME  IN VARCHAR2,
    P_AMOUNT         IN NUMBER,
    P_SCH_CODE       IN VARCHAR2,
    P_FOLIO_NO       IN VARCHAR2,
    P_TR_DATE        IN DATE,
    P_FLAG           IN VARCHAR2   -- 'C', 'K', 'KCOB'
) RETURN CLOB
IS
    V_QUERY          CLOB;
    FINALTRANCODE    CLOB := NULL;
    TRANCODE         TRANSACTION_ST.TRAN_CODE%TYPE;
    V_CURSOR         SYS_REFCURSOR;

    V_TR_REG         VARCHAR2(20);
    V_SRM            VARCHAR2(20);
    V_RECO_STATUS    VARCHAR2(1);

BEGIN
    V_TR_REG := UPPER(P_FLAG); -- C, K, KCOB
    V_RECO_STATUS := UPPER(P_RECONCILE); -- Y, N, NULL
    V_SRM := NULL; 
    
    -- Build the dynamic query
    V_QUERY := 'SELECT t.tran_code
          FROM employee_master e,
               branch_master b,
               mut_fund amc,
               scheme_info sch,
               TRANSACTION_ST@MF.BAJAJCAPITAL t,
               investor_master i,
               CITY_MASTER c
         WHERE i.city_id = c.city_id(+)
           AND t.client_code = i.inv_code
           AND TO_CHAR(t.rmcode) = e.rm_code
           AND t.branch_code = b.branch_code
           AND t.mut_code = amc.mut_code
           AND t.sch_code = sch.sch_code';
    
    -- CAMS/KARVY flag condition
    IF V_TR_REG = 'C' THEN
        V_QUERY := V_QUERY || ' AND (t.DUP_FLAG2 = 0 OR (t.REG_TRAN_TYPE = ''TICOB'' AND t.DUP_FLAG2 IN (0, 9)))';
    ELSIF V_TR_REG = 'K' THEN
        V_QUERY := V_QUERY || ' AND (t.DUP_FLAG2 = 0 OR (t.REG_TRAN_FLAG = ''TI'' AND t.DUP_FLAG2 IN (0, 9)))';
    END IF;
    
    -- Branch filter
    IF P_BRANCH IS NOT NULL AND P_BRANCH != 'ALL' THEN
        V_QUERY := V_QUERY || ' AND b.BRANCH_CODE in (''' || P_BRANCH || ''')';
    ELSE
        V_QUERY := V_QUERY || ' AND b.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='''||P_LOG_ID||''' AND ROLE_ID='''||P_ROLE_ID||''')';
    END IF;
    
    -- RM code
    IF V_SRM IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND e.rm_code = ''' || V_SRM || '''';
    END IF;
    
    -- AMC filter
    IF P_AMC IS NOT NULL AND P_AMC <> 'ALL' THEN
        V_QUERY := V_QUERY || ' AND TO_CHAR(t.mut_code) = ''' || P_AMC || '''';
    END IF;
    
    -- Reconcile filter
    IF V_RECO_STATUS = 'Y' THEN
        V_QUERY := V_QUERY || ' AND t.rec_flag = ''Y''';
    ELSIF V_RECO_STATUS = 'N' THEN
        V_QUERY := V_QUERY || ' AND (t.rec_flag = ''N'' OR t.rec_flag IS NULL)';
    END IF;
    
    -- Regular/SIP filter
    IF UPPER(P_TRAN_TYPE) = 'REGULAR' THEN
        V_QUERY := V_QUERY || ' AND ((UPPER(t.REG_TRANTYPE) NOT LIKE ''%SYS%'' AND UPPER(t.REG_TRANTYPE) NOT LIKE ''%SIP%'' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%S T P IN%'' ';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ REVERSAL%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%STP SWITCH IN%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%STPI%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%REDEMPTION%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%STPIR%''';
        V_QUERY := V_QUERY || ' ) OR t.REG_TRANTYPE IS NULL) ';
        
    ELSIF UPPER(P_TRAN_TYPE) = 'SIP' THEN
        V_QUERY := V_QUERY || ' AND (UPPER(t.REG_TRANTYPE) LIKE ''%SYS%''';
        V_QUERY := V_QUERY || ' OR UPPER(t.REG_TRANTYPE) LIKE ''%SIP%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) LIKE ''%S T P IN%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) LIKE ''%S T P IN REJ%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) LIKE ''%S T P IN REJ REVERSAL%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) LIKE ''%STP SWITCH IN%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) LIKE ''%STPI%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) LIKE ''%STPIR%'')';
    END IF;
    
    -- Date range (exclude for KARVY COB)
    IF V_TR_REG <> 'KCOB' THEN
        IF P_FROM_DATE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND t.tr_date >= TO_DATE(''' || TO_CHAR(P_FROM_DATE, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
        END IF;
        IF P_TO_DATE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND t.tr_date <= TO_DATE(''' || TO_CHAR(P_TO_DATE, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
        END IF;
    END IF;
    
    -- Cheque/search filter
    IF P_CHEQUE_TYPE IS NOT NULL AND P_CHEQUE_SEARCH IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND (';
        IF P_CHEQUE_TYPE = '001' THEN
            V_QUERY := V_QUERY || ' t.CHEQUE_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '002' THEN
            V_QUERY := V_QUERY || ' t.FOLIO_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '003' THEN
            V_QUERY := V_QUERY || ' t.APP_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '004' THEN
           V_QUERY := V_QUERY || ' (UPPER(t.PAN1) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN2) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN3) = UPPER(''' || P_CHEQUE_SEARCH || '''))';
        ELSIF P_CHEQUE_TYPE = '005' THEN
            V_QUERY := V_QUERY || ' t.REG_SUBBROK = ''' || P_CHEQUE_SEARCH || '''';
        END IF;
        V_QUERY := V_QUERY || ')';
    END IF;
    
    -- Investor name filter
    IF P_INVESTOR_NAME IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND UPPER(TRIM(t.INV_NAME)) LIKE ''%' || REPLACE(UPPER(P_INVESTOR_NAME), ' ', '%') || '%''';
    END IF;
    
    -- Amount filter
    IF P_AMOUNT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND ABS(ROUND(t.amount)) = ' || ABS(ROUND(P_AMOUNT));
    END IF;
    
    -- Static filters
    V_QUERY := V_QUERY || ' AND LPAD(t.mut_code, 2) = ''MF''
           AND (t.asa <> ''C'' OR t.asa IS NULL)
           AND t.tran_type IN (''PURCHASE'', ''REINVESTMENT'', ''SWITCH IN'')
           AND t.sch_code = ''' || P_SCH_CODE || '''
           AND t.folio_no = ''' || P_FOLIO_NO || '''';
    
    -- Transaction date (exclude for KARVY COB)
    IF V_TR_REG <> 'KCOB' AND P_TR_DATE IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND t.tr_date = TO_DATE(''' || TO_CHAR(P_TR_DATE, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
    END IF;
    
    -- Execute the dynamic query
    OPEN V_CURSOR FOR V_QUERY;
    
    LOOP
        FETCH V_CURSOR INTO TRANCODE;
        EXIT WHEN V_CURSOR%NOTFOUND;
        
        IF FINALTRANCODE IS NOT NULL THEN
            FINALTRANCODE := FINALTRANCODE || ',' || '''' || TRANCODE || '''';
        ELSE
            FINALTRANCODE := '''' || TRANCODE || '''';
        END IF;
    END LOOP;
    
    CLOSE V_CURSOR;
    
    RETURN FINALTRANCODE;
EXCEPTION
    WHEN OTHERS THEN
        IF V_CURSOR%ISOPEN THEN
            CLOSE V_CURSOR;
        END IF;
        RAISE;
END GETTRANCODE_DYNAMIC;
/