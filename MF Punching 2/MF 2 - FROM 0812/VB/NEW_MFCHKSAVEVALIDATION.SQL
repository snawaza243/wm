CREATE OR REPLACE PROCEDURE chkSaveValidation (
    P_FrSave             IN  NUMBER,    -- 1=True, 0=False
    P_FrMody             IN  NUMBER,    -- 1=True, 0=False
    P_TranCode           IN  VARCHAR2,
    P_ImEntryDate        IN  DATE,
    P_Glbins_PreviousDate IN DATE,
    P_Glbins_NextDate     IN DATE,
    P_Glbup_PreviousDate  IN DATE,
    P_Glbup_NextDate      IN DATE,
    RESULT               OUT NUMBER,
    MESSAGE              OUT VARCHAR2
) AS
    v_original_date DATE;
    v_count         NUMBER;
BEGIN
    RESULT  := 1;
    MESSAGE := '';

    -- Get original date if tranCode is provided
    IF P_TranCode IS NOT NULL AND P_TranCode <> '' THEN
        BEGIN
            SELECT tr_date
            INTO v_original_date
            FROM transaction_mf_temp1
            WHERE tran_code = P_TranCode;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                v_original_date := NULL;
        END;
    END IF;

    -- Case 1: Saving a new record
    IF P_FrSave = 1 AND P_FrMody = 0 THEN
        IF (P_ImEntryDate < P_Glbins_PreviousDate) OR (P_ImEntryDate > P_Glbins_NextDate) THEN
            RESULT  := 0;
            MESSAGE := 'Security restrictions for date range';
            RETURN;
        END IF;
    END IF;

    -- Case 2: Modifying an existing record
    IF P_FrSave = 0 AND P_FrMody = 1 THEN
        IF (v_original_date < P_Glbup_PreviousDate) OR (v_original_date > P_Glbup_NextDate) THEN
            RESULT  := 0;
            MESSAGE := 'Security restrictions for date range';
            RETURN;
        END IF;

        SELECT COUNT(*)
        INTO v_count
        FROM WEALTHMAKER.ANATRANDETAILTABLE_NEW_ALL_VB
        WHERE tran_code = P_TranCode;

        IF v_count > 0 THEN
            RESULT  := 0;
            MESSAGE := 'ANG Bills generated for this transaction. You can not modify the payout of this AR.';
            RETURN;
        END IF;
    END IF;
END;
/

/* HOW TO USE

DECLARE
    v_result  NUMBER;
    v_message VARCHAR2(200);
BEGIN
    chkSaveValidation(
        P_FrSave             => 1,
        P_FrMody             => 0,
        P_TranCode           => 'TXN123',
        P_ImEntryDate        => TO_DATE('2025-08-13','YYYY-MM-DD'),
        P_Glbins_PreviousDate => DATE '2025-08-01',
        P_Glbins_NextDate     => DATE '2025-08-31',
        P_Glbup_PreviousDate  => DATE '2025-07-01',
        P_Glbup_NextDate      => DATE '2025-09-01',
        RESULT               => v_result,
        MESSAGE              => v_message
    );

    IF v_result = 0 THEN
        DBMS_OUTPUT.PUT_LINE('Error: ' || v_message);
        RETURN;
    END IF;
END;

*/