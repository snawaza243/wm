create or replace PROCEDURE PSM_DAP_INSERTION1 (
    p_column_array     IN SYS.DBMS_SQL.VARCHAR2_TABLE,
    p_value_array      IN SYS.DBMS_SQL.VARCHAR2_TABLE,
    p_table_name       IN VARCHAR2,
    p_month            IN VARCHAR2,
    p_year             IN VARCHAR2,
    p_userid           IN VARCHAR2,
    p_data_type        IN VARCHAR2
)
IS
    v_sql           VARCHAR2(32767);
    v_columns       VARCHAR2(32767);
    v_values        VARCHAR2(32767);
    v_column_count  NUMBER;
    v_cursor        NUMBER;
    v_execute       NUMBER;
    v_target_table  VARCHAR2(100);
    v_is_date_column BOOLEAN;
    v_value         VARCHAR2(4000);
    v_parsed_date   DATE;
BEGIN
    -- Determine target table based on input table name
    IF INSTR(LOWER(p_table_name), 'paid') > 0 THEN
        v_target_table := 'BAJAJ_PAID_DATA';
    ELSIF INSTR(LOWER(p_table_name), 'due') > 0 THEN
        v_target_table := 'BAJAJ_DUE_DATA';
    ELSIF INSTR(LOWER(p_table_name), 'lapse') > 0 THEN
        v_target_table := 'BAJAJ_DUE_DATA';
    ELSE
        RAISE_APPLICATION_ERROR(-20002, 'Invalid table name pattern. Must contain paid, due, or lapse');
    END IF;

    -- Get the count of columns
    v_column_count := p_column_array.COUNT;

    -- Validate dates before processing
    FOR i IN 1..v_column_count LOOP
        v_value := p_value_array(i);

        -- Check if the value looks like a date
        IF REGEXP_LIKE(v_value, '^\d{2}-\d{2}-\d{4}$') THEN
            BEGIN
                -- Attempt to convert to date, which will validate day, month, and year
                v_parsed_date := TO_DATE(v_value, 'DD-MM-YYYY');
            EXCEPTION
                WHEN OTHERS THEN
                    RAISE_APPLICATION_ERROR(-20005, 
                        'Invalid date: ' || v_value || 
                        '. Ensure the day is valid for the given month and year. ' ||
                        SQLERRM);
            END;
        END IF;
    END LOOP;

    -- Initialize variables for dynamic SQL
    v_columns := '';
    v_values := '';

    -- Build column and value strings for dynamic columns
    FOR i IN 1..v_column_count LOOP
        -- Initialize flag as false for each column
        v_is_date_column := FALSE;

        -- Get the value for the current column
        v_value := p_value_array(i);

        -- Check if the value is in DD-MM-YYYY format
        IF REGEXP_LIKE(v_value, '^\d{2}-\d{2}-\d{4}$') THEN
            v_is_date_column := TRUE; -- Set flag to true if it's a date value
        END IF;

        -- Build the column string and value string
        IF i != v_column_count THEN
            v_columns := v_columns || p_column_array(i) || ',';
            IF v_is_date_column THEN
                v_values := v_values || 'TO_DATE(:' || i || ', ''DD-MM-YYYY''),';
            ELSE
                v_values := v_values || ':' || i || ',';
            END IF;
        ELSE
            v_columns := v_columns || p_column_array(i);
            IF v_is_date_column THEN
                v_values := v_values || 'TO_DATE(:' || i || ', ''DD-MM-YYYY'')';
            ELSE
                v_values := v_values || ':' || i;
            END IF;
        END IF;
    END LOOP;

    -- Add all static parameters to columns and values
    v_columns := v_columns || 
                 ',Mon_no,Year_No,UserId,ImportDataType,NEWINSERT,FORCE_FLAG, IMPORT_DT';

    v_values := v_values || 
                ',:month,:year,:userid,:import_data_type,:newinsert,:force_flag,:import_date';

    -- Construct dynamic SQL
    v_sql := 'INSERT INTO ' || v_target_table || '(' || v_columns || ') VALUES (' || v_values || ')';

    -- Open cursor
    v_cursor := DBMS_SQL.OPEN_CURSOR;

    -- Parse the SQL
    DBMS_SQL.PARSE(v_cursor, v_sql, DBMS_SQL.NATIVE);

    -- Bind the dynamic values
    FOR i IN 1..v_column_count LOOP
        v_value := p_value_array(i);

        -- Check if the value is in DD-MM-YYYY format
        IF REGEXP_LIKE(v_value, '^\d{2}-\d{2}-\d{4}$') THEN
            -- Bind as DATE for date columns
            DBMS_SQL.BIND_VARIABLE(v_cursor, ':' || i, v_value);
        ELSE
            -- Bind as string for non-date columns
            DBMS_SQL.BIND_VARIABLE(v_cursor, ':' || i, v_value);
        END IF;
    END LOOP;

    -- Bind all static parameters
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':month', p_month);
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':year', p_year);
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':userid', p_userid);
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':import_data_type', p_data_type);
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':newinsert', 'NULL');
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':force_flag', 'FORCE FULL');
    DBMS_SQL.BIND_VARIABLE(v_cursor, ':import_date', SYSDATE);


    -- Execute the statement
    v_execute := DBMS_SQL.EXECUTE(v_cursor);

    -- Close cursor
    DBMS_SQL.CLOSE_CURSOR(v_cursor);

    -- Commit the transaction
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        -- Close cursor if open
        IF DBMS_SQL.IS_OPEN(v_cursor) THEN
            DBMS_SQL.CLOSE_CURSOR(v_cursor);
        END IF;
        -- Rollback transaction
        ROLLBACK;
        -- Raise the error
        RAISE_APPLICATION_ERROR(-20001, 'Error in PSM_DAP_INSERTION1: ' || SQLERRM);
END PSM_DAP_INSERTION1;