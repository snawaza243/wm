create or replace PROCEDURE PSM_DAP_DB_FIELD_LIST (
    tableType IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
)
IS
    tableName VARCHAR2(100);
    sqlQuery  VARCHAR2(4000);
BEGIN
    -- Determine the table name based on the input parameter
    
    -- BAJAJ_PAID_DATA FOR  PAID, INISIATED
    -- BAJAJ_DUE_DATA FOR DUE, LAPSED

    IF INSTR(LOWER(tableType), 'due') > 0 THEN
        tableName := 'BAJAJ_DUE_DATA';
    ELSIF INSTR(LOWER(tableType), 'paid') > 0 THEN
        tableName := 'BAJAJ_PAID_DATA';
    ELSIF INSTR(LOWER(tableType), 'lapsed') > 0 THEN
        tableName := 'BAJAJ_DUE_DATA';
    ELSIF INSTR(LOWER(tableType), 'reinstate') > 0 THEN
        tableName := 'BAJAJ_PAID_DATA';
    ELSE
        tableName := NULL;
    END IF;

    -- Validate the resolved table name
    IF tableName IS NOT NULL THEN
        -- Construct the dynamic SQL to list all column names
        sqlQuery := 'SELECT column_name AS "Text", column_name AS "Value" ' ||
                    'FROM all_tab_columns ' ||
                    'WHERE table_name = :tableName ORDER BY column_name ASC';

        -- Open the cursor dynamically
        OPEN p_cursor FOR sqlQuery USING UPPER(tableName);
    ELSE
        -- Fallback: Return a generic result if no matching table was found
        OPEN p_cursor FOR
        SELECT 'No Data Found' AS "Text",
               'No Data Found' AS "Value"
        FROM dual;
    END IF;
END PSM_DAP_DB_FIELD_LIST;