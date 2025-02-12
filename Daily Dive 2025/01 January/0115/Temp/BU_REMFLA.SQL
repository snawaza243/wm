create or replace PROCEDURE PSM_IR_REMINDER_FLAG(
    p_company_code   IN VARCHAR2,
    p_month          IN VARCHAR2,
    p_year           IN NUMBER,
    p_policy_number  IN VARCHAR2,
    p_record_count   OUT NUMBER,  -- Output parameter to return the number of affected records
    p_message        OUT VARCHAR2 -- Output parameter for the message
)
AS
BEGIN
    -- Perform the UPDATE operation
    UPDATE bajaj_due_data
    SET rem_flage = 'YES'
    WHERE COMPANY_CD = p_company_code
      AND (
            (p_policy_number IS NOT NULL AND POLICY_NO = p_policy_number)
            OR 
            (p_policy_number IS NULL AND TO_CHAR(DUE_DATE, 'MM') = p_month AND TO_CHAR(DUE_DATE, 'YYYY') = p_year)
          );

    -- Get the number of affected records
    p_record_count := SQL%ROWCOUNT;

    -- Set the message based on whether records were updated
    IF p_record_count > 0 THEN
        p_message := p_record_count || ' records updated successfully.';
    ELSE
        p_message := 'No records found to update.';
    END IF;

    -- Commit the transaction to persist changes
    COMMIT;
END PSM_IR_REMINDER_FLAG;