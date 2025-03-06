create or replace PROCEDURE psm_clean_table (
    p_table_name IN VARCHAR2,
    p_message OUT VARCHAR2
) IS
    v_sql VARCHAR2(1000);
BEGIN
    -- Construct the dynamic SQL to delete all rows from the given table
    v_sql := 'DELETE FROM ' || p_table_name;

    -- Execute the dynamic SQL statement
    EXECUTE IMMEDIATE v_sql;

    -- Return a success message
    p_message := 'All rows deleted successfully from table ' || p_table_name;
EXCEPTION
    WHEN OTHERS THEN
        -- If an error occurs, return the error message
        p_message := 'Error occurred while cleaning the table ' || p_table_name || ': ' || SQLERRM;
END psm_clean_table;
