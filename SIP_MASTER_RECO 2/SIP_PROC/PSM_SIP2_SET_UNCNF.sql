CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_SIP2_SET_UNCNF(
    MyTrCode       IN VARCHAR2,
    MasterId       IN VARCHAR2,   
    MyDispatch     IN VARCHAR2,
    Glbloginid     IN VARCHAR2,
    P_ROLE         IN VARCHAR2,
    P_CURSOR       OUT SYS_REFCURSOR
)
AS
    v_error_msg VARCHAR2(4000);
BEGIN

    OPEN P_CURSOR FOR 
        SELECT 'Procedure started' AS message FROM dual;
    
    SAVEPOINT start_transaction;
    
    IF MyTrCode IS NULL OR Glbloginid IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, '⚠️ First Double Click The Record , You Want To Map');
    END IF;

    -- IF MasterId IS NULL THEN
    --     RAISE_APPLICATION_ERROR(-20001, 'First Select The Record Of Sip MAster , To Which You Want To Map');
    -- END IF;
    
    IF MyDispatch = 'N' THEN
        UPDATE TRANSACTION_MF_TEMP1 SET REC_FLAG = null, RECO_DATE = SYSDATE, REC_USER = Glbloginid WHERE TRAN_CODE = MyTrCode;
        UPDATE TRANSACTION_MF_TEMP1 SET REC_FLAG = null, RECO_DATE = SYSDATE, REC_USER = Glbloginid WHERE BASE_TRAN_CODE = MyTrCode;
        --Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG=null, HO_TRAN_CODE=MyTrCode where tran_code in ( MyRtaTrCode );
    ELSE
        Update Transaction_mf_temp1 set  REC_FLAG= null,RECO_DATE=TO_DATE(SYSDATE),REC_USER = Glbloginid WHERE TRAN_CODE = MyTrCode;
        Update Transaction_mf_temp1 set  REC_FLAG= null,RECO_DATE=TO_DATE(SYSDATE),REC_USER = Glbloginid WHERE BASE_TRAN_CODE = MyTrCode;
        --Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG=null,HO_TRAN_CODE=MyTrCode where tran_code in (MyRtaTrCode);
    END IF;

    -- Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG=null,RECO_DATE=TO_DATE(SYSDATE),REC_USER=Glbloginid  WHERE TRAN_CODE=MyTrCode;
    -- Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG=null,RECO_DATE=TO_DATE(SYSDATE),REC_USER=Glbloginid  WHERE BASE_TRAN_CODE=MyTrCode;
    -- Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER=Glbloginid  ,BASE_TRAN_CODE=MyTrCode ,REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=MasterId;
    COMMIT;
    
    OPEN P_CURSOR FOR 
    SELECT 'SUCCESS: Base SIP Registration Un-Confirmed Successfully' AS message FROM dual;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO start_transaction;
        v_error_msg := SQLERRM;
        OPEN P_CURSOR FOR 
        SELECT '❌ ERROR: ' || v_error_msg AS message FROM dual;
END;
/