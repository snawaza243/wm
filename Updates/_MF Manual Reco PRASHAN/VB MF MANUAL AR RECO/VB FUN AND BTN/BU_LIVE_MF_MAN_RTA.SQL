CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_MANUAL_RTA (
    P_DATE_FROM       IN DATE,
    P_DATE_TO         IN DATE,
    P_STATUS          IN CHAR,
    P_AMC             IN VARCHAR2,
    P_BRANCH          IN VARCHAR2,
    P_CHEQUE_TYPE     IN VARCHAR2,
    P_CHEQUE_SEARCH   IN VARCHAR2,
    P_INVESTOR_NAME   IN VARCHAR2,
    P_AMOUNT          IN VARCHAR2,
    P_TRAN_TYPE       IN VARCHAR2,
    P_SEARCH_TEXT     IN VARCHAR2, 
    P_TR_TRAN_TYPE    IN VARCHAR2,
    P_TR_REG          IN VARCHAR2,
    P_LOG_ID          IN VARCHAR2,
    P_ROLE_ID         IN VARCHAR2,

    P_CURSOR          OUT SYS_REFCURSOR
) AS
    V_QUERY         VARCHAR2(4000);  
    SRmCode         VARCHAR2(10) := NULL;
/* This procedure is for RTA Search and Following param is for the values of TR 
P_SEARCH_TEXT   : TR Code       as --> TR12345 ( if user entered  in the tr at search row click box)
P_TR_REG        : TR Registrar  as --> k, c, kcob, ccob 
P_TR_TRAN_TYPE  : TR  Tran Type as --> regular, pms, atm, trailActual, sip
*/   
    
    
    
    
BEGIN
    -- Start building the base query
    V_QUERY := 'SELECT tmf.TRAN_CODE, tmf.INV_NAME, tmf.UNQ_KEY, tmf.SCH_CODE, si.SCH_NAME, tmf.MUT_CODE, ai.MUT_NAME, tmf.TRAN_TYPE, tmf.APP_NO, tmf.APP_DATE, tmf.CHEQUE_NO, tmf.CHEQUE_DATE, ';

    IF P_TR_REG = 'kcob' THEN 
        V_QUERY := V_QUERY || ' WEALTHMAKER.FN_GET_COB_CMV@MF.BAJAJCAPITAL(tmf.FOLIO_NO,tmf.SCH_CODE) amount,';
    ELSE
        V_QUERY := V_QUERY || 'tmf.AMOUNT,';
    END IF;
        
    V_QUERY := V_QUERY || ' tmf.BANK_NAME, tmf.FOLIO_NO, tmf.RMCODE, li.RM_NAME, tmf.INVESTOR_TYPE, tmf.BRANCH_CODE, ti.BRANCH_NAME, tmf.TR_DATE, tmf.UNITS, tmf.NAV_DATE, tmf.REMARK, tmf.FLAG, tmf.BROKER_ID, tmf.REG_TRANTYPE, pi.EXIST_CODE, substr(i.ADDRESS1,1,10) as ADDRESS1, tmf.BUSINESS_RMCODE, c.CITY_NAME, ';

--With MAX 
 
    IF P_TR_REG = 'c' then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(tmf.FOLIO_NO,tmf.SCH_CODE,tmf.TR_DATE,tmf.TRAN_TYPE,tmf.TRAN_ID,MAX(tmf.UNQ_KEY),''CAMS'') UNIQUE_TRAN, ';
    ELSIF P_TR_REG = 'k' then 
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(tmf.FOLIO_NO,tmf.SCH_CODE,tmf.TR_DATE,tmf.TRAN_TYPE,MAX(tmf.TRAN_ID),MAX(tmf.UNQ_KEY),''KARVY'') UNIQUE_TRAN, ';

    elsif P_TR_REG = 'kcob' then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(tmf.FOLIO_NO,tmf.SCH_CODE,MAX(tmf.TR_DATE),tmf.TRAN_TYPE,MAX(tmf.TRAN_ID),tmf.UNQ_KEY,''KARVY COB'') UNIQUE_TRAN, ';

    elsif P_TR_REG = 'ccob' then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(tmf.FOLIO_NO,tmf.SCH_CODE,tmf.TR_DATE,tmf.TRAN_TYPE,tmf.TRAN_ID,MAX(tmf.UNQ_KEY),''CAMS'') UNIQUE_TRAN, ';
    End If;

    V_QUERY := V_QUERY || ' MAX(REG_TRANTYPE) REG_TRANTYPE,max(unq_key) unq_key ';
 
    V_QUERY := V_QUERY || ' 
                FROM transaction_st@MF.BAJAJCAPITAL tmf
                LEFT JOIN scheme_info si ON tmf.SCH_CODE = si.SCH_CODE
                LEFT JOIN MUT_FUND ai ON tmf.MUT_CODE = ai.MUT_CODE
                LEFT JOIN BRANCH_MASTER ti ON tmf.BRANCH_CODE = ti.BRANCH_CODE
                LEFT JOIN EMPLOYEE_MASTER li ON tmf.RMCODE = li.RM_CODE
                LEFT JOIN AGENT_MASTER pi ON tmf.source_code = pi.agent_code
                LEFT JOIN INVESTOR_MASTER i ON tmf.CLIENT_CODE = i.INV_CODE
                LEFT JOIN CITY_MASTER c ON i.CITY_ID = c.CITY_ID 
                WHERE 1 = 1';   
                
    IF P_TR_REG = 'c' THEN 
        V_QUERY := V_QUERY || ' AND (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE=''TICOB'' AND DUP_FLAG2 IN(0,9))) ';
    ELSIF P_TR_REG = 'k' THEN 
        V_QUERY := V_QUERY || ' AND (DUP_FLAG2 = 0 OR (REG_TRAN_FLAG=''TI'' AND DUP_FLAG2 IN(0,9))) ';
    END IF;

    IF P_BRANCH IS NOT NULL AND P_BRANCH != 'ALL' THEN
        V_QUERY := V_QUERY || ' AND tmf.BRANCH_CODE IN( ''' || P_BRANCH || ''')';
    ELSE
        V_QUERY := V_QUERY || ' AND tmf.BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='''||P_LOG_ID||''' AND ROLE_ID='''||P_ROLE_ID||''') ';
    END IF;

    IF SRmCode IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.RMCODE = ''' || SRmCode || '''';
    END IF;

    IF P_AMC IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND TO_CHAR(tmf.MUT_CODE) = ''' || P_AMC || '''';
    END IF;

    IF P_STATUS IS NOT NULL THEN
        IF UPPER(P_STATUS) = 'Y' THEN
            V_QUERY := V_QUERY || ' AND tmf.rec_flag = ''Y''';
        ELSIF UPPER(P_STATUS) = 'N' THEN
            V_QUERY := V_QUERY || ' AND (tmf.rec_flag =''N'' or tmf.rec_flag is null)';
        END IF;
    END IF;

    IF P_TRAN_TYPE IS NOT NULL THEN
        IF UPPER(P_TRAN_TYPE) = 'REGULAR' THEN
            V_QUERY := V_QUERY || ' AND UPPER(tmf.REG_TRANTYPE) NOT LIKE ''%SYS%'' 
                                   AND UPPER(tmf.REG_TRANTYPE) NOT LIKE ''%SIP%'' 
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%S T P IN%'' 
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ%'' 
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ REVERSAL%'' 
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%STP SWITCH IN%'' 
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%OUT%''
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%REINVESTMENT%''
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%STPI%'' 
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%STPIR%''
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%REDEMPTION%''
                                   AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';
                                   
        
        ELSIF UPPER(P_TRAN_TYPE) = 'SIP' THEN
            V_QUERY := V_QUERY || ' AND ((UPPER(tmf.REG_TRANTYPE) LIKE ''%SYS%'' 
                                       OR UPPER(tmf.REG_TRANTYPE) LIKE ''%SIP%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%S T P IN%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%S T P IN REJ%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%S T P IN REJ REVERSAL%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%STP SWITCH IN%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%STPI%'' 
                                       OR TRIM(UPPER(tmf.REG_TRANTYPE)) LIKE ''%STPIR%''))
                                       AND TRIM(UPPER(tmf.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';
        END IF;
    END IF;

    IF P_TR_REG NOT IN ('kcob') THEN
        IF P_DATE_FROM IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND tmf.TR_DATE >= TO_DATE(''' || TO_CHAR(P_DATE_FROM, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
        END IF;

        IF P_DATE_TO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND tmf.TR_DATE <= TO_DATE(''' || TO_CHAR(P_DATE_TO, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
        END IF;
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
        V_QUERY := V_QUERY || ' AND UPPER(tmf.INV_NAME) LIKE ''%' || REPLACE(UPPER(P_INVESTOR_NAME),' ', '%') || '%''';
    END IF;

    IF P_AMOUNT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND abs(round(tmf.amount)) = ' || ABS(ROUND(TO_NUMBER(P_AMOUNT)));
    END IF;

    IF P_SEARCH_TEXT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND tmf.TRAN_CODE LIKE ''%' || P_SEARCH_TEXT || '%''';
    END IF;

    V_QUERY := V_QUERY || ' AND LPAD(tmf.mut_code, 2) = ''MF'' ';
    V_QUERY := V_QUERY || ' AND (tmf.asa <> ''C'' OR tmf.asa IS NULL)';
    V_QUERY := V_QUERY || ' AND tmf.tran_type IN (''PURCHASE'', ''REINVESTMENT'', ''SWITCH IN'') ';   
 
    IF P_TR_REG IN ('c', 'ccob') THEN
        V_QUERY := V_QUERY || ' GROUP BY tmf.TRAN_CODE, tmf.INV_NAME, tmf.UNQ_KEY, tmf.SCH_CODE, si.SCH_NAME, tmf.MUT_CODE, ai.MUT_NAME, tmf.TRAN_TYPE, tmf.APP_NO, 
                                tmf.APP_DATE, tmf.CHEQUE_NO, tmf.CHEQUE_DATE, tmf.AMOUNT, 
                                tmf.BANK_NAME, tmf.FOLIO_NO, tmf.RMCODE, li.RM_NAME, tmf.INVESTOR_TYPE, tmf.BRANCH_CODE, ti.BRANCH_NAME, tmf.TR_DATE, tmf.UNITS, 
                                tmf.NAV_DATE, tmf.REMARK, tmf.FLAG, tmf.BROKER_ID, tmf.REG_TRANTYPE, pi.EXIST_CODE, SUBSTR(I.ADDRESS1,1,10), 
                                tmf.BUSINESS_RMCODE, c.CITY_NAME, TMF.TRAN_ID';
    ELSIF P_TR_REG = 'k' THEN
        V_QUERY := V_QUERY || ' GROUP BY tmf.TRAN_CODE, tmf.INV_NAME, tmf.UNQ_KEY, tmf.SCH_CODE, si.SCH_NAME, tmf.MUT_CODE, ai.MUT_NAME, tmf.TRAN_TYPE, tmf.APP_NO, 
                                  tmf.APP_DATE, tmf.CHEQUE_NO, tmf.CHEQUE_DATE, tmf.AMOUNT, 
                                  tmf.BANK_NAME, tmf.FOLIO_NO, tmf.RMCODE, li.RM_NAME, tmf.INVESTOR_TYPE, tmf.BRANCH_CODE, ti.BRANCH_NAME, tmf.TR_DATE, tmf.UNITS, 
                                  tmf.NAV_DATE, tmf.REMARK, tmf.FLAG, tmf.BROKER_ID, tmf.REG_TRANTYPE, pi.EXIST_CODE, SUBSTR(I.ADDRESS1,1,10), 
                                  tmf.BUSINESS_RMCODE, c.CITY_NAME ';
    ELSIF P_TR_REG = 'kcob' THEN
        V_QUERY := V_QUERY || ' GROUP BY TMF.TRAN_CODE, TMF.INV_NAME, TMF.UNQ_KEY, TMF.SCH_CODE, SI.SCH_NAME, TMF.MUT_CODE, AI.MUT_NAME, TMF.TRAN_TYPE, TMF.APP_NO,
                                  TMF.APP_DATE, TMF.CHEQUE_NO, TMF.CHEQUE_DATE,  WEALTHMAKER.FN_GET_COB_CMV@MF.BAJAJCAPITAL(TMF.FOLIO_NO,TMF.SCH_CODE) , 
                                  TMF.BANK_NAME, TMF.FOLIO_NO, TMF.RMCODE, LI.RM_NAME, TMF.INVESTOR_TYPE, TMF.BRANCH_CODE, TI.BRANCH_NAME, TMF.TR_DATE, TMF.UNITS,
                                  TMF.NAV_DATE, TMF.REMARK, TMF.FLAG, TMF.BROKER_ID, TMF.REG_TRANTYPE, PI.EXIST_CODE, SUBSTR(I.ADDRESS1,1,10) , 
                                  TMF.BUSINESS_RMCODE, C.CITY_NAME ';
    END IF;
 
     V_QUERY := V_QUERY || ' ORDER BY tmf.TR_DATE';
     
    OPEN P_CURSOR FOR V_QUERY;
    --OPEN P_CURSOR FOR SELECT V_QUERY FROM DUAL;
    
    
    
END PSM_MF_RECO_MANUAL_RTA;
/