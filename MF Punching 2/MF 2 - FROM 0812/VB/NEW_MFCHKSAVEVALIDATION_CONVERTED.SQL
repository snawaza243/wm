
-- chkSaveValidation equivalent
DECLARE
    v_original_date DATE;
    v_count         NUMBER;
BEGIN
    -- Get original date if tranCode is provided
    IF P_TranCode IS NOT NULL AND P_TranCode <> '' THEN
        SELECT tr_date
        INTO v_original_date
        FROM transaction_mf_temp1
        WHERE tran_code = P_TranCode;
    END IF;

    -- Case 1: Saving a new record
    IF P_FrSave = 1 AND P_FrMody = 0 THEN
        IF (P_ImEntryDate < P_Glbins_PreviousDate) OR (P_ImEntryDate > P_Glbins_NextDate) THEN
            OPEN P_CURSOR FOR
                SELECT 'Security restrictions for date range' AS MESSAGE
                FROM dual;
            RETURN;
        END IF;
    END IF;

    -- Case 2: Modifying an existing record
    IF P_FrSave = 0 AND P_FrMody = 1 THEN
        IF (v_original_date < P_Glbup_PreviousDate) OR (v_original_date > P_Glbup_NextDate) THEN
            OPEN P_CURSOR FOR
                SELECT 'Security restrictions for date range' AS MESSAGE
                FROM dual;
            RETURN;
        END IF;

        SELECT COUNT(*)
        INTO v_count
        FROM WEALTHMAKER.ANATRANDETAILTABLE_NEW_ALL_VB
        WHERE tran_code = P_TranCode;

        IF v_count > 0 THEN
            OPEN P_CURSOR FOR
                SELECT 'ANG Bills generated for this transaction. You can not modify the payout of this AR.' AS MESSAGE
                FROM dual;
            RETURN;
        END IF;
    END IF;

    -- If all checks pass, continue
END;

