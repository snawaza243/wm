CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_MANUAL_RTA2 (
    P_DATE_FROM1      IN VARCHAR2,
    P_DATE_TO1        IN VARCHAR2,
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
    V_SRM_CODE      VARCHAR2(10) := NULL;
    V_MyOrder       VARCHAR2(5) :=NULL;
    P_DATE_FROM     DATE;
    P_DATE_TO       DATE;
/* This procedure is for RTA Search and Following param is for the values of TR 
P_SEARCH_TEXT   : TR Code       as --> TR12345 ( if user entered  in the tr at search row click box)
P_TR_REG        : TR Registrar  as --> k, c, kcob, ccob 
P_TR_TRAN_TYPE  : TR  Tran Type as --> regular, pms, atm, trailActual, sip
*/   
    
    
    
    
BEGIN


   P_DATE_FROM     := TO_DATE(P_DATE_FROM1,'DD/MM/YYYY');
   P_DATE_TO       := TO_DATE(P_DATE_TO1,'DD/MM/YYYY');

    -- Start building the base query
    V_QUERY := '  SELECT   FOLIO_NO, SCH_CODE, MAX(TR_DATE)TR_DATE, TRAN_TYPE, 
             MAX (INVESTOR_NAME) INVESTOR_NAME, MAX (ADDRESS) ADDRESS, 
             MAX (BROKER_CODE) BROKER_CODE, MAX (CITY_NAME) CITY_NAME, MAX(SCH_NAME)SCH_NAME, 
             MAX(MUT_CODE)MUT_CODE, MAX(MUT_NAME)MUT_NAME, MAX(RM_NAME)RM_NAME, MAX(BRANCH_NAME)BRANCH_NAME, MAX (APP_NO) APP_NO, 
             MAX (CHEQUE_NO) CHEQUE_NO, SUM (AMOUNT) AMOUNT, 
             MAX (BUSI_BRANCH_CODE) BUSI_BRANCH_CODE, 
             MAX (BUSINESS_RMCODE) BUSINESS_RMCODE,    
    ';

-- FOLIO_NO, SCH_CODE, MAX(TR_DATE)TR_DATE, TRAN_TYPE,  INVESTOR_NAME , ADDRESS, 
--BROKER_CODE, CITY_NAME, SCH_NAME, MUT_CODE, MUT_NAME, RM_NAME BRANCH_NAME APP_NO CHEQUE_NO AMOUNT BUSI_BRANCH_CODE BUSINESS_RMCODE
-- UNIQUE_TRAN unq_key
    IF P_TR_REG = 'c' Then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,TRAN_ID,MAX(UNQ_KEY),''CAMS'') UNIQUE_TRAN, ';
    ELSIF P_TR_REG = 'k'  Then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,MAX(TRAN_ID),MAX(UNQ_KEY),''KARVY'') UNIQUE_TRAN, ';
    ELSIF P_TR_REG = 'kcob' Then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,MAX(TR_DATE),TRAN_TYPE,MAX(TRAN_ID),UNQ_KEY,''KARVY COB'') UNIQUE_TRAN, ';
    ELSIF P_TR_REG = 'ccob' Then
        V_QUERY := V_QUERY || ' WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,TRAN_ID,MAX(UNQ_KEY),''CAMS'') UNIQUE_TRAN, ';
    End If;

    V_QUERY := V_QUERY || '  MAX(REG_TRANTYPE) REG_TRANTYPE,max(unq_key) unq_key ';
    V_QUERY := V_QUERY || '    FROM (  ';
    V_QUERY := V_QUERY || '  SELECT t.tran_id,tran_type, t.inv_name Investor_Name,(i.ADDRESS1||'',''||i.ADDRESS2||'',''||i.EMAIL) address,  ';
    V_QUERY := V_QUERY || '          REG_SUBBROK broker_code,c.city_name,  ';
    V_QUERY := V_QUERY || '          t.sch_code,sch_name sch_name,t.mut_code,amc.mut_name mut_name, rm_name,   ';
    V_QUERY := V_QUERY || '          branch_name,   ';
    V_QUERY := V_QUERY || '          app_no,folio_no, cheque_no,  ';

    IF P_TR_REG = 'kcob' Then
        V_QUERY := V_QUERY || ' WEALTHMAKER.FN_GET_COB_CMV@MF.BAJAJCAPITAL(T.FOLIO_NO,T.SCH_CODE)     amount,  ';
    ELSE
        V_QUERY := V_QUERY || ' amount, ';
    End If;


    V_QUERY := V_QUERY || '  t.tran_code,reg_date as tr_date,  ';
    V_QUERY := V_QUERY || '          busi_branch_code, business_rmcode,t.REG_TRANTYPE,t.unq_key unq_key ';
    V_QUERY := V_QUERY || '     FROM employee_master e,  ';
    V_QUERY := V_QUERY || '          branch_master b,  ';
    V_QUERY := V_QUERY || '          mut_fund amc,  ';
    V_QUERY := V_QUERY || '          scheme_info sch,  ';
    V_QUERY := V_QUERY || '          TRANSACTION_ST@MF.BAJAJCAPITAL t,  ';
    V_QUERY := V_QUERY || '          investor_master i,CITY_MASTER C  ';
    V_QUERY := V_QUERY || '    WHERE I.CITY_ID=C.CITY_ID(+)  ';
    V_QUERY := V_QUERY || '      AND t.client_code = i.inv_code  ';
    V_QUERY := V_QUERY || '      AND to_char(t.rmcode) = e.rm_code  ';
    V_QUERY := V_QUERY || '      AND t. BRANCH_CODE = b.branch_code    ';
    V_QUERY := V_QUERY || '      AND t.mut_code = amc.mut_code AND t.sch_code = sch.sch_code  ';
    


    IF P_TR_REG = 'c' Then
        V_QUERY := V_QUERY || '  AND  (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE=''TICOB'' AND DUP_FLAG2 IN(0,9)))  ';
    ELSIF P_TR_REG = 'k' Then
        V_QUERY := V_QUERY || ' AND  (DUP_FLAG2 = 0 OR (REG_TRAN_FLAG=''TI'' AND DUP_FLAG2 IN(0,9))) ';
    End If;

    IF P_BRANCH IS NOT NULL AND P_BRANCH != 'ALL' THEN
        V_QUERY := V_QUERY || ' and b.BRANCH_CODE in ( ''' || P_BRANCH || ''')';
    ELSE
        V_QUERY := V_QUERY || ' and b.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='''||P_LOG_ID||''' AND ROLE_ID='''||P_ROLE_ID||''') ';
    END IF;

    IF V_SRM_CODE IS NOT NULL THEN
        V_QUERY := V_QUERY || ' and e.rm_code = ''' || V_SRM_CODE || '''';
    END IF;

    IF P_AMC IS NOT NULL THEN
        V_QUERY := V_QUERY || ' and to_char(t.mut_code) = ''' || P_AMC || '''';
    END IF;

    IF P_STATUS IS NOT NULL THEN
        IF UPPER(P_STATUS) = 'Y' THEN
            V_QUERY := V_QUERY || ' and t.rec_flag =''Y'' ';
        ELSIF UPPER(P_STATUS) = 'N' THEN
            V_QUERY := V_QUERY || ' and (t.rec_flag =''N'' or rec_flag is null) ';
        END IF;
    END IF;

    IF P_TRAN_TYPE IS NOT NULL THEN
        IF UPPER(P_TRAN_TYPE) = 'REGULAR' THEN

 V_QUERY := V_QUERY || ' AND ((    UPPER (t.REG_TRANTYPE) NOT LIKE ''%SYS%'' AND UPPER (t.REG_TRANTYPE) NOT LIKE ''%SIP%'' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%S T P IN%'' ';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ REVERSAL%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE))NOT LIKE ''%STP SWITCH IN%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%STPI%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%REDEMPTION%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';
        V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%STPIR%''';
        V_QUERY := V_QUERY || ' ) OR t.REG_TRANTYPE IS NULL) ';
        
        ELSIF UPPER(P_TRAN_TYPE) = 'SIP' THEN
        
    V_QUERY := V_QUERY || ' AND (( UPPER (t.REG_TRANTYPE) LIKE ''%SYS%''';
        V_QUERY := V_QUERY || ' OR UPPER (t.REG_TRANTYPE) LIKE ''%SIP%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%S T P IN%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%S T P IN REJ%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%S T P IN REJ REVERSAL%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE)) LIKE ''%STP SWITCH IN%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%STPI%''';
        V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%STPIR%'')) '; 
        END IF;
    END IF;

    IF P_TR_REG NOT IN ('kcob') THEN
        IF P_DATE_FROM IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TR_DATE >= TO_DATE(''' || TO_CHAR(P_DATE_FROM, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
        END IF;

        IF P_DATE_TO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TR_DATE <= TO_DATE(''' || TO_CHAR(P_DATE_TO, 'DD-MM-YYYY')  || ''', ''DD-MM-YYYY'')';
        END IF;
    END IF;

    IF P_CHEQUE_TYPE IS NOT NULL AND P_CHEQUE_SEARCH IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND (';
        IF P_CHEQUE_TYPE = '001' THEN
            V_QUERY := V_QUERY || ' t.CHEQUE_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '002' THEN
            V_QUERY := V_QUERY || ' t.FOLIO_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '003' THEN
            V_QUERY := V_QUERY || ' t.APP_NO = ''' || P_CHEQUE_SEARCH || '''';
        ELSIF P_CHEQUE_TYPE = '004' THEN
           V_QUERY := V_QUERY || ' (UPPER(t.PAN1) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN2) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN3) = UPPER(''' || P_CHEQUE_SEARCH || ''')) ';
        ELSIF P_CHEQUE_TYPE = '005' THEN
            V_QUERY := V_QUERY || ' t.REG_SUBBROK = ''' || P_CHEQUE_SEARCH || '''';
        END IF;
        V_QUERY := V_QUERY || ')';
    END IF;

    IF P_INVESTOR_NAME IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND UPPER(TRIM(t.INV_NAME)) LIKE ''%' || REPLACE(UPPER(P_INVESTOR_NAME),' ', '%') || '%''';
    END IF;

    IF P_AMOUNT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND abs(round(t.amount)) = ' || ABS(ROUND(TO_NUMBER(P_AMOUNT)));
    END IF;

/* TR CODE IF USER WILL PASS 
    IF P_SEARCH_TEXT IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND t.TRAN_CODE LIKE ''%' || P_SEARCH_TEXT || '%''';
    END IF;
*/
    V_QUERY := V_QUERY || ' AND LPAD(t.mut_code, 2) = ''MF'' ';
    V_QUERY := V_QUERY || ' AND (t.asa <> ''C'' OR t.asa IS NULL)';
    V_QUERY := V_QUERY || ' AND t.tran_type IN (''PURCHASE'', ''REINVESTMENT'', ''SWITCH IN'') ';   

    V_QUERY := V_QUERY || ' ORDER BY ';

    IF V_MyOrder IS NULL THEN
        V_QUERY := V_QUERY || ' TR_Date,rm_name ';
    END IF;
    
    V_QUERY := V_QUERY || ' ) ';

 
    IF P_TR_REG IN ('c', 'ccob') THEN
        V_QUERY := V_QUERY || 'GROUP BY FOLIO_NO, ';
        V_QUERY := V_QUERY || '         SCH_CODE, ';
        V_QUERY := V_QUERY || '         TR_DATE, ';
        V_QUERY := V_QUERY || '         TRAN_TYPE,TRAN_ID ';

    ELSIF P_TR_REG = 'k' THEN
        V_QUERY := V_QUERY || ' GROUP BY FOLIO_NO, ';
        V_QUERY := V_QUERY || '          SCH_CODE, ';
        V_QUERY := V_QUERY || '          TR_DATE, ';
        V_QUERY := V_QUERY || '          TRAN_TYPE ';

    ELSIF P_TR_REG = 'kcob' THEN
        V_QUERY := V_QUERY || ' GROUP BY FOLIO_NO, ';
        V_QUERY := V_QUERY || '          SCH_CODE, ';
        V_QUERY := V_QUERY || '          TRAN_TYPE,UNQ_KEY ';

    END IF;
    
    




   /* testing cursor
    OPEN P_CURSOR FOR 
    SELECT AGENT_CODE AS AMOUNT, CITY_ID AS TRAN_CODE, AGENT_MASTER.* 
    FROM AGENT_MASTER WHERE ROWNUM <=100;
    RETURN;*/
    
    
    OPEN P_CURSOR FOR V_QUERY;
    --OPEN P_CURSOR FOR SELECT V_QUERY FROM DUAL;
    
    
    
END PSM_MF_RECO_MANUAL_RTA2;
/