CREATE OR REPLACE PROCEDURE WEALTHMAKER.GET_TRANSACTIONS_MF_PUNC(
    P_FROM_DATE           IN TRANSACTION_MF_TEMP1.TR_DATE%TYPE,
    P_TO_DATE             IN TRANSACTION_MF_TEMP1.TR_DATE%TYPE,
    P_ORDER_BY            IN VARCHAR2,
    P_ORDER_DIRECTION     IN VARCHAR2,
    P_PAN_NO              IN TRANSACTION_MF_TEMP1.PANNO%TYPE,
    P_TR_NO               IN TRANSACTION_MF_TEMP1.TRAN_CODE%TYPE,
    P_UNIQUE_CLIENT_CODE  IN TRANSACTION_MF_TEMP1.CLIENT_CODE%TYPE,
    P_CHEQUE_NO           IN TRANSACTION_MF_TEMP1.CHEQUE_NO%TYPE,
    P_APP_NO              IN TRANSACTION_MF_TEMP1.APP_NO%TYPE,
    P_ANAEXISTCODE        IN AGENT_MASTER.AGENT_CODE%TYPE,
    P_NEW_LOGIN_ID        in     VARCHAR2, -- 11
    P_NEW_ROLE_ID         in    VARCHAR2, --12
    P_ISVIEW              IN VARCHAR2,
    P_RESULT              OUT SYS_REFCURSOR
) AS
    V_QUERY VARCHAR2(4000);
BEGIN

    IF P_ISVIEW = '1' THEN -- SSTab1.Tab = 0  OF VB
        -- INITIALIZE THE BASE QUERY
        V_QUERY := 'SELECT 
            B.BRANCH_NAME,
            TMF.TRAN_CODE, 
            TMF.INVESTOR_NAME, 
            TMF.PANNO, 
            TMF.SCH_CODE,
            TMF.SWITCH_SCHEME,
            SSI.SCH_NAME AS SW_SCH_NAME,
            COALESCE(SI.SCH_NAME, OP.OSCH_NAME) AS SCH_NAME,
            TMF.MUT_CODE,
            AI.MUT_NAME,
            TMF.R_SCHEMECODE, 
            TMF.LEAD_NAME,
            TMF.CLIENT_CODE,
            TMF.AC_HOLDER_CODE,
            TMF.TR_DATE, 
            TMF.TRAN_TYPE, 
            TMF.APP_NO, 
            TMF.PAYMENT_MODE, 
            TMF.CHEQUE_NO,  
            TO_CHAR(tmf.sip_end_date, ''DD/MM/YYYY'') AS sip_end_date, 
            TO_CHAR(tmf.sip_start_date, ''DD/MM/YYYY'') AS sip_start_date,
            TMF.CHEQUE_DATE, 
            TMF.AMOUNT, 
            TMF.SIP_TYPE, 
            TMF.LEAD_NO, 
            TMF.BANK_NAME, 
            TMF.RMCODE, 
            TMF.R_AMCCODE,
            TMF.FOLIO_NO, 
            TMF.DOC_ID, 
            TMF.INVESTOR_TYPE, 
            TMF.SWITCH_SCHEME, 
            TMF.TARGET_SWITCH_SCHEME, 
            TMF.REMARK,
            TRUNC(TMF.SIP_END_DATE),
            TRUNC(TMF.SIP_START_DATE),
            TMF.FREQUENCY,
            TMF.INSTALLMENTS_NO,
            TMF.BASE_TRAN_CODE,
            TMF.BUSINESS_RMCODE,
            TMF.GOAL,
            IM.MOBILE,
            IM.INV_CODE,
            IM.ADDRESS1,
            IM.ADDRESS2,
            IM.EMAIL,
            IM.DOB,
            IM.PINCODE,
            IM.CITY_ID,
            C.CITY_NAME,
            IM.AADHAR_CARD_NO,
            IM.PAN,
            STM.COUNTRY_ID
        FROM 
            TRANSACTION_MF_TEMP1 TMF
        LEFT JOIN 
            SCHEME_INFO SI ON TMF.SCH_CODE = SI.SCH_CODE
        LEFT JOIN 
            SCHEME_INFO SSI ON TMF.SWITCH_SCHEME = SSI.SCH_CODE
        LEFT JOIN 
            MUT_FUND AI ON TMF.MUT_CODE = AI.MUT_CODE
        LEFT JOIN 
            CLIENT_MASTER CI ON TMF.PANNO = CI.PAN
        LEFT JOIN 
            AGENT_MASTER XI ON TMF.SOURCE_CODE = XI.AGENT_CODE
        LEFT JOIN 
            INVESTOR_MASTER IM ON TMF.CLIENT_CODE = IM.INV_CODE
        LEFT JOIN 
            CITY_MASTER C ON IM.CITY_ID = C.CITY_ID
        LEFT JOIN 
            STATE_MASTER STM ON C.STATE_ID = STM.STATE_ID
        LEFT JOIN
            BRANCH_MASTER B ON TMF.BUSI_BRANCH_CODE = B.BRANCH_CODE
        LEFT JOIN
            OTHER_PRODUCT OP ON TMF.SCH_CODE = OP.OSCH_CODE
        WHERE 1 = 1 AND B.BRANCH_CODE IN(SELECT BRANCH_ID FROM WEALTHMAKER.USERDETAILS_JI WHERE LOGIN_ID='''||P_NEW_LOGIN_ID||''' AND ROLE_ID='''||P_NEW_ROLE_ID||''')';  -- START WITH A TRUE CONDITION

        IF P_FROM_DATE IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND TMF.TR_DATE >= TO_DATE(''' || 
                  TO_CHAR(P_FROM_DATE, 'YYYY-MM-DD') || ''', ''YYYY-MM-DD'') ';
        END IF;

        IF P_TO_DATE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.TR_DATE <= TO_DATE(''' || 
                      TO_CHAR(P_TO_DATE, 'YYYY-MM-DD') || ''', ''YYYY-MM-DD'') ';
        END IF;

        IF P_PAN_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.PANNO = ''' || P_PAN_NO || ''' ';  -- USE EQUALITY
        END IF;

        IF P_TR_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.TRAN_CODE = ''' || P_TR_NO || ''' ';  -- USE EQUALITY
        END IF;

        IF P_UNIQUE_CLIENT_CODE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.CLIENT_CODE = ''' || P_UNIQUE_CLIENT_CODE || ''' ';  -- USE EQUALITY
        END IF;

        IF P_CHEQUE_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.CHEQUE_NO = ''' || P_CHEQUE_NO || ''' ';  -- USE EQUALITY
        END IF;

        IF P_APP_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.APP_NO = ''' || P_APP_NO || ''' ';  -- USE EQUALITY
        END IF;

          IF P_ANAEXISTCODE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND XI.EXIST_CODE = ''' || P_ANAEXISTCODE || ''' ';
        END IF;

        -- ADD ORDERING DYNAMICALLY
        IF P_ORDER_BY IS NOT NULL AND P_ORDER_DIRECTION IS NOT NULL THEN
            V_QUERY := V_QUERY || ' ORDER BY ' || P_ORDER_BY || ' ' || P_ORDER_DIRECTION;
        ELSE
            V_QUERY := V_QUERY || ' ORDER BY TMF.TRAN_CODE';  -- DEFAULT ORDERING
        END IF;

        -- OPEN THE CURSOR FOR THE DYNAMIC QUERY
        OPEN P_RESULT FOR V_QUERY
             
        ;
    ELSE  -- SSTAB1.TAB = 1 THEN OF VB 
        
        V_QUERY:= 'SELECT * FROM TEMPORARYBUSINESS  TMF WHERE 1=1';
     
        IF P_FROM_DATE IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND TMF.TR_DATE >= TO_DATE(''' || 
                  TO_CHAR(P_FROM_DATE, 'YYYY-MM-DD') || ''', ''YYYY-MM-DD'') ';
        END IF;

        IF P_TO_DATE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.TR_DATE <= TO_DATE(''' || 
                      TO_CHAR(P_TO_DATE, 'YYYY-MM-DD') || ''', ''YYYY-MM-DD'') ';
        END IF;

        IF P_PAN_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.PANNO = ''' || P_PAN_NO || ''' ';  -- USE EQUALITY
        END IF;

        IF P_TR_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.TRAN_CODE = ''' || P_TR_NO || ''' ';  -- USE EQUALITY
        END IF;

        IF P_UNIQUE_CLIENT_CODE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.CLIENT_CODE = ''' || P_UNIQUE_CLIENT_CODE || ''' ';  -- USE EQUALITY
        END IF;

        IF P_CHEQUE_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.CHEQUE_NO = ''' || P_CHEQUE_NO || ''' ';  -- USE EQUALITY
        END IF;

        IF P_APP_NO IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND TMF.APP_NO = ''' || P_APP_NO || ''' ';  -- USE EQUALITY
        END IF;

          IF P_ANAEXISTCODE IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND XI.EXIST_CODE = ''' || P_ANAEXISTCODE || ''' ';
        END IF;

        -- ADD ORDERING DYNAMICALLY
        IF P_ORDER_BY IS NOT NULL AND P_ORDER_DIRECTION IS NOT NULL THEN
            V_QUERY := V_QUERY || ' ORDER BY ' || P_ORDER_BY || ' ' || P_ORDER_DIRECTION;
        ELSE
            V_QUERY := V_QUERY || ' ORDER BY TMF.TRAN_CODE';  -- DEFAULT ORDERING
        END IF;
    
    
    
    
    
    
    END IF;
END GET_TRANSACTIONS_MF_PUNC;
/
