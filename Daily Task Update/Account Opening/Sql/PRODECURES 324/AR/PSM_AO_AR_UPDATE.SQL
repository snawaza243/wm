create or replace PROCEDURE PSM_AO_AR_UPDATE(
    p_tran_code IN VARCHAR2,
    p_mut_code IN VARCHAR2,
    p_scheme_code IN VARCHAR2,
    p_amount IN NUMBER,
    p_cheque_no IN VARCHAR2 DEFAULT NULL,
    p_cheque_date IN DATE DEFAULT NULL,
    p_bank_name IN VARCHAR2 DEFAULT NULL,
    p_remark IN VARCHAR2,
    p_logged_user_id IN VARCHAR2,
    p_is_cash IN BOOLEAN,    -- TRUE = Cash, FALSE = Cheque
    P_IS_CHEQUE IN BOOLEAN,
    P_IS_DRAFT  IN BOOLEAN,

    p_message OUT VARCHAR2   -- OUT parameter to return success or error message
) AS
BEGIN
    -- Update logic for cheque transactions


    IF p_is_cash = FALSE THEN
        UPDATE transaction_st
        SET MUT_CODE = p_mut_code,
            SCH_CODE = p_scheme_code,
            AMOUNT = p_amount,
            cheque_no = TRIM(p_cheque_no),
            CHEQUE_DATE = p_cheque_date,
            BANK_NAME = TRIM(p_bank_name),
            remark = 'ADVISORY',
            BANK_AC_NO = p_remark,
            LOGGEDUSERID = p_logged_user_id
        WHERE tran_code = p_tran_code;
    ELSE
        -- Update logic for cash transactions
        UPDATE transaction_st
        SET MUT_CODE = p_mut_code,
            SCH_CODE = p_scheme_code,
            AMOUNT = p_amount,
            remark = 'ADVISORY',
            BANK_AC_NO = p_remark,
            LOGGEDUSERID = p_logged_user_id
        WHERE tran_code = p_tran_code;
    END IF;

    -- Check if any rows were updated
    IF SQL%ROWCOUNT = 0 THEN
        RAISE_APPLICATION_ERROR(-20007, 'Transaction Code Not Found or No Rows Updated');
    END IF;

    -- Commit the update
    COMMIT;

    -- Success message
    p_message := 'Transaction ' || p_tran_code || ' Has Been Modified Successfully';

EXCEPTION
    WHEN OTHERS THEN
        -- Rollback on error
        ROLLBACK;
        p_message := 'Error: ' || SQLERRM;
END PSM_AO_AR_UPDATE;
