CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_M_RECONCILE4(
    MyTrCode            IN VARCHAR2, 
    MyRtaAmount         IN NUMBER,
    MyRtaTrDate         IN VARCHAR2,
    MyRtaFolio          IN VARCHAR2,
    MyRtaTrCode         IN VARCHAR2,
    MyDispatch          IN VARCHAR2,
    Glbloginid          IN VARCHAR2,
    P_TR_TRAN_TYPE      IN VARCHAR2,
    P_ROLE_ID           IN VARCHAR2,
    P_CURSOR            OUT SYS_REFCURSOR    
)  
AS
    V_ERROR_MSG         VARCHAR2(4000);
    MyRtaTrCode_1       VARCHAR2(4000);
    MyMailId            VARCHAR2(50);   
    rsCheckRecoFlag     CHAR(1);         
BEGIN

    IF LENGTH(MyRtaTrCode) > 400 THEN 
        MyRtaTrCode_1 := SUBSTR(MyRtaTrCode,1,400);
    ELSE
        MyRtaTrCode_1 := MyRtaTrCode;
    END IF;
    
    MyRtaTrCode_1 :=  REGEXP_REPLACE(MyRtaTrCode_1, '[''"\\;]', '');

    -- Initialize output cursor with status message
    OPEN P_CURSOR FOR 
    SELECT 'Procedure started' AS MESSAGE FROM DUAL;
    
    -- Begin transaction
    SAVEPOINT START_TRANSACTION;
    
    -- Validate input parameters
    IF MyTrCode IS NULL OR MyRtaTrCode IS NULL OR Glbloginid IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Mandatory parameters (TRAN_CODE, RTA_TRAN_CODE, LOGIN_ID) cannot be null');
    END IF;
    

    IF MyDispatch = 'N' THEN
        Update Transaction_mf_temp1 set  
        amount=MyRtaAmount, 
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=MyRtaTrCode_1  
        WHERE TRAN_CODE=MyTrCode;
       
        Update Transaction_mf_temp1 
        set 
        amount=MyRtaAmount , 
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=MyRtaTrCode_1
        WHERE BASE_TRAN_CODE=MyTrCode;
       
        Update Transaction_st@MF.BAJAJCAPITAL set 
        REC_FLAG='Y',
        HO_TRAN_CODE=MyTrCode 
        where tran_code in (SELECT * FROM TABLE(WEALTHMAKER.data_SPLIT(MyRtaTrCode,',')));

    ELSE 

        IF P_TR_TRAN_TYPE = 'TRAIL' THEN
        Update Transaction_mf_temp1 set  
        amount=MyRtaAmount,
        folio_no=MyRtaFolio,
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=MyRtaTrCode_1
        WHERE TRAN_CODE=MyTrCode;
        
        Update Transaction_mf_temp1 set 
        amount=MyRtaAmount,
        folio_no=MyRtaFolio,
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=MyRtaTrCode_1
        WHERE BASE_TRAN_CODE= MyTrCode;

        Update Transaction_st@MF.BAJAJCAPITAL set 
        REC_FLAG='Y',
        HO_TRAN_CODE=MyTrCode
        where tran_code in (SELECT * FROM TABLE(WEALTHMAKER.data_SPLIT(MyRtaTrCode,',')));
    Else
        Update Transaction_mf_temp1 set  
        amount=MyRtaAmount,
        folio_no=MyRtaFolio,
        tr_date=NVL(to_date(MyRtaTrDate, 'dd/mm/rrrr'),NULL),
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER= Glbloginid,
        RTA_TRAN_CODE=MyRtaTrCode_1
        WHERE TRAN_CODE=MyTrCode;

        Update Transaction_mf_temp1 set 
        amount=MyRtaAmount,
        folio_no=MyRtaFolio,
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=MyRtaTrCode_1
        WHERE BASE_TRAN_CODE=MyTrCode;

        Update Transaction_st@MF.BAJAJCAPITAL set 
        REC_FLAG='Y',
        HO_TRAN_CODE=MyTrCode
        where tran_code in  (SELECT * FROM TABLE(WEALTHMAKER.data_SPLIT(MyRtaTrCode,',')));
    END IF;

    -- TEMP SEND MAIL
    IF Glbloginid = '39339' THEN 
        MyMailId := 'anamikat@bajajcapital.com';
    ELSIF Glbloginid = '112649' THEN 
        MyMailId := 'rajeshb@bajajcapital.com';
    END IF;

    -- CALLING MAIL SEND PROC AND PASSING VALUE IN PARAM
    SEND_MAIL(MyMailId,'wealthmaker@bajajcapital.com','','','AR Reconciled', '', 'Reco Update '|| MyTrCode);
    END IF;

    SELECT REC_FLAG INTO rsCheckRecoFlag  FROM TRANSACTION_MF_TEMP1 WHERE TRAN_CODE = MyTrCode;

    OPEN P_CURSOR FOR
        SELECT 'SUCCESS: ' || 'Record is mapped successfully ' || rsCheckRecoFlag AS MESSAGE FROM DUAL;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO START_TRANSACTION;
         
        V_ERROR_MSG := SQLERRM;
         
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || V_ERROR_MSG AS MESSAGE FROM DUAL;
         
END;
/