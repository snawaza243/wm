CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_M_RECONCILE3(
    P_TRAN_CODE      IN VARCHAR2,
    P_TR_TRAN        IN VARCHAR2,
    P_RTA_AMOUNT     IN NUMBER,
    P_RTA_TRAN_CODE  IN VARCHAR2,
    P_LOGIN_ID       IN VARCHAR2,
    P_ROLE_ID        IN VARCHAR2,
    P_CURSOR        OUT SYS_REFCURSOR    
)  
AS
    V_COUNT1 NUMBER := 0;
    V_COUNT2 NUMBER := 0;
    V_COUNT3 NUMBER := 0;
    V_ERROR_MSG VARCHAR2(4000);
    MyDispatch VARCHAR2(10);
    MyRtaTrCode_1   VARCHAR2(400);
    MyTrCode    VARCHAR2(20);
BEGIN

    MyTrCode := P_TRAN_CODE;
MyRtaAmount:= P_RTA_AMOUNT;
Glbloginid := P_LOGIN_ID;

    
    -- Initialize output cursor with status message
    OPEN P_CURSOR FOR 
    SELECT 'Procedure started' AS MESSAGE FROM DUAL;
    
    -- Begin transaction
    SAVEPOINT START_TRANSACTION;
    
    -- Validate input parameters
    IF P_TRAN_CODE IS NULL OR P_RTA_TRAN_CODE IS NULL OR P_LOGIN_ID IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Mandatory parameters (TRAN_CODE, RTA_TRAN_CODE, LOGIN_ID) cannot be null');
    END IF;
    

    IF MyDispatch = 'N' THEN
        Update Transaction_mf_temp1 
        set  
        amount=MyRtaAmount, 
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=Replace(MyRtaTrCode_1, "'", "")  
        WHERE TRAN_CODE=MyTrCode;
       
        Update Transaction_mf_temp1 
        set 
        amount=MyRtaAmount , 
        REC_FLAG='Y',
        RECO_DATE=SYSDATE,
        REC_USER=Glbloginid,
        RTA_TRAN_CODE=Replace(MyRtaTrCode_1, "'", "") 
        WHERE BASE_TRAN_CODE=MyTrCode
       
        Update Transaction_st@MF.BAJAJCAPITAL set 
        REC_FLAG='Y',
        HO_TRAN_CODE=MyTrCode 
        where tran_code in (MyRtaTrCode)
    ELSE 

        IF P_TR_TRAN = 'TRAIL' THEN

  MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "' , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'") ',tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr')
        MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "' ,  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'") 'tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr'),
        MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
    Else
        MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "' ,tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr'), REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'") ',tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr')
        MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "', REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'") 'tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr'),
        MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
    End If


    END IF;



























    SELECT 'SUCCESS: ' AS MESSAGE FROM DUAL;
           
    FROM DUAL;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO START_TRANSACTION;
         
        V_ERROR_MSG := SQLERRM;
         
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || V_ERROR_MSG AS MESSAGE FROM DUAL;
         
END;
/