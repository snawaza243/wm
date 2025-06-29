CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_AR_INSERT(  
    p_client_head_inv       IN VARCHAR2,
    p_source_id             IN NUMBER,
    p_busi_code             IN NUMBER,
    p_plan_type             IN VARCHAR2,
    p_cheque_no             IN VARCHAR2,
    p_chq_date              IN DATE,
    p_bank_name             IN VARCHAR2,
    p_amount                IN NUMBER,
    p_mut_code              IN VARCHAR2,
    p_scheme_code           IN VARCHAR2,
    p_rm_code               IN NUMBER,
    p_branch                IN NUMBER,
    p_login_id              IN VARCHAR2,
    p_remark                IN VARCHAR2,
    p_message               OUT VARCHAR2  -- OUT parameter to capture the result
) AS
    v_adv_fr_ren VARCHAR2(20);
    v_my_investor_st VARCHAR2(20);
    v_my_source_id VARCHAR2(20);
    v_my_tran_code VARCHAR2(50);
    v_ex_tran_count NUMBER;
    v_ex_rtran_count NUMBER;
BEGIN
    -- Advisory Fresh/Renewal selection
    IF p_plan_type = 'Fresh' THEN
        v_adv_fr_ren := 'PURCHASE';
    ELSIF p_plan_type = 'Renewal' THEN
        v_adv_fr_ren := 'REINVESTMENT';
    END IF;

    -- Input validation
    IF p_busi_code IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Business Code Cannot Be Left Blank');
    END IF;

    IF p_plan_type IS NULL THEN
        RAISE_APPLICATION_ERROR(-20002, 'Plan Cannot Be Left Blank');
    END IF;



    IF p_bank_name IS NULL THEN
        RAISE_APPLICATION_ERROR(-20004, 'Bank Name Cannot Be Left Blank');
    END IF;

    -- Check if the transaction already exists
    SELECT COUNT(*) INTO v_ex_tran_count
    FROM transaction_st
    WHERE TRIM(cheque_no) = TRIM(p_cheque_no)
    AND remark = 'ADVISORY';

    IF v_ex_tran_count > 0 THEN
        RAISE_APPLICATION_ERROR(-20005, 'This Transaction Already Exists');
    END IF;

    -- Insert into transaction_sttemp
    INSERT INTO transaction_sttemp (
        tran_code, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT,
        BRANCH_CODE, SOURCE_CODE, cheque_no, CHEQUE_DATE, BANK_NAME, remark
    ) VALUES (
       null, '1', SYSDATE, p_source_id, p_mut_code, p_scheme_code, v_adv_fr_ren, p_amount,
        p_branch, p_source_id, TRIM(p_cheque_no), p_chq_date, TRIM(p_bank_name), 'ADVISORY'
    );

    -- Get the latest transaction code
    SELECT MAX(tran_code) INTO v_my_tran_code
    FROM temp_tran
    WHERE branch_code = p_branch;

    -- Insert into transaction_st
    INSERT INTO transaction_st (
        TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT,
        BRANCH_CODE, cheque_no, CHEQUE_DATE, BANK_NAME, remark, LOGGEDUSERID
    ) VALUES (
        TRIM(v_my_tran_code), '1', SYSDATE, 
        --(SELECT client_codekyc FROM CLIENT_TEST CT WHERE CT.source_code  = p_source_id AND ct.client_code = ct.main_code AND  ROWNUM = 1),
        p_client_head_inv,
        p_mut_code, p_scheme_code, 
        v_adv_fr_ren, p_amount, p_branch, TRIM(p_cheque_no), p_chq_date, TRIM(p_bank_name), 
        'ADVISORY', p_login_id
    );

    -- Commit the transaction
    COMMIT;

    -- Success message
    p_message := 'Transaction ' || v_my_tran_code || ' Has Been Saved Successfully';

--EXCEPTION WHEN OTHERS THEN ROLLBACK; p_message := 'Error: ' || SQLERRM;
END PSM_AO_AR_INSERT;
/
