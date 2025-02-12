create or replace PROCEDURE PSM_NPS_PRINT_RECEIPT_GRID (
    p_unique_id IN VARCHAR2,
    p_result_cursor OUT SYS_REFCURSOR
) IS
    v_query VARCHAR2(4000);
BEGIN
    -- Check if the receipt number is provided
    IF p_unique_id IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Please Enter Receipt No First');
    END IF;

    -- Construct the query to select data
    v_query := 'SELECT * FROM npstranreceipt_view WHERE unique_id = :unique_id';

    -- Open the cursor for the result set
    OPEN p_result_cursor FOR v_query USING p_unique_id;

    -- Optionally, insert log entry for report generation
    --PROC_SAVE_LOGIN('guserid', 'Reports', 'PROC_PRINT_RECEIPT');

END PSM_NPS_PRINT_RECEIPT_GRID;