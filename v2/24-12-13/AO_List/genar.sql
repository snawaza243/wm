create or replace PROCEDURE PSM_AO_save_adv_tran(  
    p_source_id IN number,
    p_busi_code IN number,
    p_plan_type IN VARCHAR2,
    p_cheque_no IN VARCHAR2,
    p_chq_date IN DATE,
    p_bank_name IN VARCHAR2,
    p_amount IN NUMBER,
    p_mut_code IN VARCHAR2,
    p_scheme_code IN VARCHAR2,
    p_rm_code IN number,
    p_branch IN number,
    p_login_id IN VARCHAR2,
    p_remark IN VARCHAR2,
    p_result out SYS_REFCURSOR
) AS
    v_adv_fr_ren VARCHAR2(20);
    v_my_investor_st VARCHAR2(20);
    v_my_source_id VARCHAR2(20);
    v_my_tran_code VARCHAR2(50);
    v_my_rm_code VARCHAR2(20);
    v_existing_transaction_count NUMBER;
    v_existing_renewal_transaction_count NUMBER;
BEGIN
    -- Advisory Fresh/Renewal selection
    IF p_plan_type = 'Fresh' THEN
        v_adv_fr_ren := 'PURCHASE';
    ELSIF p_plan_type = 'Renewal' THEN
        v_adv_fr_ren := 'REINVESTMENT';
    END IF;

    -- Input validation
    IF p_busi_code IS NULL OR p_busi_code = '' THEN
        OPEN p_result FOR
        SELECT RAISE_APPLICATION_ERROR(-20001, 'Business Code Cannot Be Left Blank')  AS message FROM DUAL;
        return;
    END IF;

    IF p_plan_type IS NULL OR p_plan_type = '' THEN
        OPEN p_result FOR
            select RAISE_APPLICATION_ERROR(-20002, 'Plan Cannot Be Left Blank') AS message FROM DUAL;
        return;

    END IF;

    IF p_cheque_no IS NULL OR p_cheque_no = '' THEN
        OPEN p_result FOR
            select 
            RAISE_APPLICATION_ERROR(-20003, 'Please Enter Cheque No.') 
            AS message FROM DUAL;
        return;

    END IF;

    IF p_bank_name IS NULL OR p_bank_name = '' THEN
        OPEN p_result FOR
            select 
            RAISE_APPLICATION_ERROR(-20004, 'Bank Name Cannot Be Left Blank')
            AS message FROM DUAL;
        return;

    END IF;

    -- Check if the transaction already exists
    SELECT COUNT(*) INTO v_existing_transaction_count
    FROM transaction_st
    WHERE TRIM(cheque_no) = TRIM(p_cheque_no)
    AND remark = 'ADVISORY';

    IF v_existing_transaction_count > 0 THEN
            OPEN p_result FOR
            select 
        RAISE_APPLICATION_ERROR(-20005, 'This Transaction Already Exists') AS message FROM DUAL;   
        return;

    END IF;

    -- Check if the transaction is a Fresh and if it already exists for the same client
    IF v_adv_fr_ren = 'PURCHASE' THEN
        SELECT COUNT(*) INTO v_existing_renewal_transaction_count
        FROM transaction_st
        WHERE client_code = p_source_id
        AND remark = 'ADVISORY'
        AND tran_type = 'PURCHASE';

        IF v_existing_renewal_transaction_count > 0 THEN
OPEN p_result FOR
            select 
            RAISE_APPLICATION_ERROR(-20006, 'Please Select Renewal Option')
            AS message FROM DUAL;   
        return;
        END IF;
    END IF;

    -- Set MySourceId based on old client code
    IF p_source_id IS NULL AND LENGTH(p_busi_code) >= 11 THEN
        v_my_source_id := SUBSTR(p_busi_code, 1, 8);
        v_my_investor_st := p_busi_code;
    END IF;

    -- Insert into transaction_sttemp
    INSERT INTO transaction_sttemp (
        INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, APP_NO, TRAN_TYPE, AMOUNT,
        BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, BANK_NAME, remark
    ) VALUES (
        '1', SYSDATE, v_my_source_id, p_mut_code, p_scheme_code, NULL, v_adv_fr_ren, p_amount,
        p_branch, p_source_id, 
        (select employee_master.rm_code from employee_master where employee_master.payroll_id = p_rm_code), -- ex 20002514 for 39779
        TRIM(p_busi_code), 
        (select employee_master.source from employee_master where employee_master.payroll_id = p_branch) , -- ex 10010108 for 39779
        TRIM(p_cheque_no), p_chq_date,
        TRIM(p_bank_name), 'ADVISORY'
    );

    -- Get the latest transaction code
    SELECT MAX(tran_code) INTO v_my_tran_code
    FROM temp_tran
    WHERE branch_code = p_branch
    AND SUBSTR(tran_code, 1, 2) = '07';

    IF v_my_investor_st IS NULL THEN
        SELECT MIN(inv_code) INTO v_my_investor_st
        FROM investor_master
        WHERE SUBSTR(inv_code, 1, 8) = v_my_source_id
        AND kyc IN ('YES', 'YESP');
    END IF;

    -- Insert into transaction_st
    INSERT INTO transaction_st (
        TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, APP_NO, TRAN_TYPE, AMOUNT,
        BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, BANK_NAME, remark,
        BANK_AC_NO, LOGGEDUSERID
    ) VALUES (
        TRIM(v_my_tran_code), '1', SYSDATE, v_my_investor_st, p_mut_code, p_scheme_code,
        NULL, v_adv_fr_ren, p_amount, 
        (select employee_master.source from employee_master where employee_master.payroll_id = p_branch) , -- ex 10010108 for 39779

        p_source_id, 
        (select employee_master.rm_code from employee_master where employee_master.payroll_id = p_rm_code), -- ex 20002514 for 39779

        TRIM(p_busi_code), p_branch,
        TRIM(p_cheque_no), p_chq_date, TRIM(p_bank_name), 'ADVISORY', p_remark, p_login_id
    );

    -- Delete from transaction_sttemp
    DELETE FROM transaction_sttemp WHERE TRAN_CODE = v_my_tran_code;

    -- Call stored procedure for 'Recd_paid_update'
    DECLARE
        recd_paid_cursor SYS_REFCURSOR;
    BEGIN
        OPEN recd_paid_cursor FOR
            SELECT 'Recd_paid_update' AS proc_name FROM DUAL;
        -- Assuming you will call the procedure in a similar fashion
        -- You should add logic to execute this procedure appropriately
    END;

    -- Commit the transaction
    COMMIT;

    -- Success message
    open p_result for 
    select 
    DBMS_OUTPUT.PUT_LINE('Transaction ' || v_my_tran_code || ' Has Been Saved Successfully')
    as message from dual;
    return

EXCEPTION
    WHEN OTHERS THEN
        -- Error handling
        ROLLBACK;
        DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM) as message from dual;
END PSM_AO_save_adv_tran;
