CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_M_PMS_UNCNF2(
    P_TRAN_CODE     IN VARCHAR2,
    P_REMARKS       IN VARCHAR2,
    P_USER          IN VARCHAR2,
    P_ROLE          IN VARCHAR2,
    P_OPT_PMS       IN BOOLEAN,
    P_OPT_ATM       IN BOOLEAN,
    P_CURSOR        OUT SYS_REFCURSOR
)
AS
    v_count1 NUMBER := 0;
    v_count2 NUMBER := 0;
    v_error_msg VARCHAR2(4000);
BEGIN
    -- Initialize output cursor with status message
    OPEN P_CURSOR FOR 
    SELECT 'Procedure started' AS message FROM dual;
    
    -- Begin transaction
    SAVEPOINT start_transaction;
    
    -- Validate input parameters
    IF P_TRAN_CODE IS NULL OR P_USER IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Mandatory parameters (TRAN_CODE, USER) cannot be null');
    END IF;
    
    IF P_OPT_PMS THEN
        -- Update for PMS unconfirmation (by TRAN_CODE)
        UPDATE TRANSACTION_MF_TEMP1
        SET 
            REMARK_RECO = TRIM(P_REMARKS),
            REC_FLAG = 'N',  -- Changed from 'Y' to 'N' for unconfirmation
            RECO_DATE = NULL, -- Clearing reconfirmation date
            REC_USER = NULL   -- Clearing reconfirmation user
        WHERE 
            TRAN_CODE = P_TRAN_CODE;

        v_count1 := SQL%ROWCOUNT;
        
        -- Update for PMS unconfirmation (by BASE_TRAN_CODE)
        UPDATE TRANSACTION_MF_TEMP1
        SET 
            REMARK_RECO = TRIM(P_REMARKS),
            REC_FLAG = 'N',  -- Changed from 'Y' to 'N' for unconfirmation
            RECO_DATE = NULL, -- Clearing reconfirmation date
            REC_USER = NULL   -- Clearing reconfirmation user
        WHERE 
            BASE_TRAN_CODE = P_TRAN_CODE;
            
        v_count2 := SQL%ROWCOUNT;
        
        -- Check if any updates were made
        IF v_count1 = 0 AND v_count2 = 0 THEN
            RAISE_APPLICATION_ERROR(-20002, 'No records found with TRAN_CODE or BASE_TRAN_CODE = ' || P_TRAN_CODE);
        END IF;
        
    ELSIF P_OPT_ATM THEN
        -- Update for ATM unconfirmation
        UPDATE TRANSACTION_MF_TEMP1
        SET 
            ATM_RECO_FLAG = 'N'  -- Changed from 'Y' to 'N' for unconfirmation
        WHERE 
            TRAN_CODE = P_TRAN_CODE;
            
        v_count1 := SQL%ROWCOUNT;
        
        -- Check if any updates were made
        IF v_count1 = 0 THEN
            RAISE_APPLICATION_ERROR(-20003, 'No records found with TRAN_CODE = ' || P_TRAN_CODE);
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20004, 'Either P_OPT_PMS or P_OPT_ATM must be TRUE');
    END IF;

    -- Commit only if all updates succeeded
    COMMIT;
    
    -- Return success message with counts (updated messages for unconfirmation)
    IF P_OPT_PMS THEN
        OPEN P_CURSOR FOR 
        SELECT 'SUCCESS: ' || 
               v_count1 || ' record(s) unconfirmed in TRANSACTION_MF_TEMP1 (by TRAN_CODE), ' ||
               v_count2 || ' record(s) unconfirmed in TRANSACTION_MF_TEMP1 (by BASE_TRAN_CODE)' AS message
        FROM dual;
    ELSE
        OPEN P_CURSOR FOR 
        SELECT 'SUCCESS: ' || 
               v_count1 || ' record(s) unconfirmed in TRANSACTION_MF_TEMP1 (ATM reconciliation removed)' AS message
        FROM dual;
    END IF;
    
EXCEPTION
    WHEN OTHERS THEN
        -- Rollback to savepoint in case of error
        ROLLBACK TO start_transaction;
        
        -- Get error message
        v_error_msg := SQLERRM;
        
        -- Return error details
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || v_error_msg AS message FROM dual;
        
        -- Optionally log the error to an error table
        -- INSERT INTO error_log(procedure_name, error_message, error_date, user_id)
        -- VALUES ('PSM_MF_RECO_M_PMS_UNCNF2', v_error_msg, SYSDATE, P_USER);
        -- COMMIT;
END;
/