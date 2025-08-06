CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_M_RECONCILE2(
    P_TRAN_CODE      IN VARCHAR2,
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
BEGIN
-- Return success message with counts

    
    -- Initialize output cursor with status message
    OPEN P_CURSOR FOR 
    SELECT 'Procedure started' AS MESSAGE FROM DUAL;
    
    -- Begin transaction
    SAVEPOINT START_TRANSACTION;
    
    -- Validate input parameters
    IF P_TRAN_CODE IS NULL OR P_RTA_TRAN_CODE IS NULL OR P_LOGIN_ID IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Mandatory parameters (TRAN_CODE, RTA_TRAN_CODE, LOGIN_ID) cannot be null');
    END IF;
    
    -- Update the first table (Transaction_mf_temp1 based on TRAN_CODE)
    UPDATE TRANSACTION_MF_TEMP1
    SET AMOUNT = P_RTA_AMOUNT,
        REC_FLAG = 'Y',
        RECO_DATE = SYSDATE,
        REC_USER = P_LOGIN_ID,
        RTA_TRAN_CODE = P_RTA_TRAN_CODE
    WHERE TRAN_CODE = P_TRAN_CODE;
    
    V_COUNT1 := SQL%ROWCOUNT;
    
    -- Update the second table (Transaction_mf_temp1 based on BASE_TRAN_CODE)
    UPDATE TRANSACTION_MF_TEMP1
    SET AMOUNT = P_RTA_AMOUNT,
        REC_FLAG = 'Y',
        RECO_DATE = SYSDATE,
        REC_USER = P_LOGIN_ID,
        RTA_TRAN_CODE = P_RTA_TRAN_CODE
    WHERE BASE_TRAN_CODE = P_TRAN_CODE;
    
    V_COUNT2 := SQL%ROWCOUNT;
    
    -- Update the third table (Transaction_st@MF.BAJAJCAPITAL)
    UPDATE TRANSACTION_ST@MF.BAJAJCAPITAL
    SET REC_FLAG = 'Y',
        HO_TRAN_CODE = P_TRAN_CODE
    WHERE TRAN_CODE = P_RTA_TRAN_CODE;
    
    V_COUNT3 := SQL%ROWCOUNT;
    
    -- Check if any updates were made
    IF V_COUNT1 = 0 AND V_COUNT2 = 0 THEN
        RAISE_APPLICATION_ERROR(-20002, 'No records found with TRAN_CODE or BASE_TRAN_CODE = ' || P_TRAN_CODE);
    END IF;
    
    IF V_COUNT3 = 0 THEN
        RAISE_APPLICATION_ERROR(-20003, 'No records found in remote table with TRAN_CODE = ' || P_RTA_TRAN_CODE);
    END IF;
    
    -- Commit only if all updates succeeded
    COMMIT;
    
    -- Return success message with counts
    OPEN P_CURSOR FOR 
    SELECT 'SUCCESS: ' || 
           V_COUNT1 || ' record(s) updated in TRANSACTION_MF_TEMP1 (by TRAN_CODE), ' ||
           V_COUNT2 || ' record(s) updated in TRANSACTION_MF_TEMP1 (by BASE_TRAN_CODE), ' ||
           V_COUNT3 || ' record(s) updated in TRANSACTION_ST@MF.BAJAJCAPITAL' AS MESSAGE
    FROM DUAL;
    
EXCEPTION
    WHEN OTHERS THEN
        -- Rollback to savepoint in case of error
        ROLLBACK TO START_TRANSACTION;
        
        -- Get error message
        V_ERROR_MSG := SQLERRM;
        
        -- Return error details
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || V_ERROR_MSG AS MESSAGE FROM DUAL;
        
        -- Optionally log the error to an error table
        -- INSERT INTO error_log(procedure_name, error_message, error_date, user_id)
        -- VALUES ('PSM_MF_RECO_MANUAL_RECO', v_error_msg, SYSDATE, P_LOGIN_ID);
        -- COMMIT;
END;
/