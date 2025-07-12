CREATE OR REPLACE PROCEDURE psm_mf_trail_validate (
    P_FROMDATE   IN  DATE,
    P_TODATE     IN  DATE,
    P_IS_VALID   OUT NUMBER,      -- 1 = valid, 0 = invalid
    P_MESSAGE    OUT VARCHAR2
)
AS
    v_row_count NUMBER;
BEGIN
    -- Validate date logic
    IF P_FROMDATE IS NULL THEN
        P_IS_VALID := 0;
        P_MESSAGE := 'Enter valid from date in dd/mm/yyyy format';
        RETURN;
    ELSIF P_TODATE IS NULL THEN
        P_IS_VALID := 0;
        P_MESSAGE := 'Enter valid to date in dd/mm/yyyy format';
        RETURN;
    ELSIF P_FROMDATE >= P_TODATE THEN
        P_IS_VALID := 0;
        P_MESSAGE := 'To date must be greater than from date';
        RETURN;
    END IF;

    -- Validate data exists in IMPORT_FOLIOS_TEMP
    SELECT COUNT(*) INTO v_row_count FROM IMPORT_FOLIOS_TEMP;

    IF v_row_count = 0 THEN
        P_IS_VALID := 0;
        P_MESSAGE := 'Please upload the file first.';
        RETURN;
    END IF;

    -- All validations passed
    P_IS_VALID := 1;
    P_MESSAGE := 'Validation successful.';
END;
