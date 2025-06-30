CREATE OR REPLACE PROCEDURE PSM_EXECUTE_BULK_INSERT(
    p_insert_statement  IN CLOB,
    p_success           OUT NUMBER,
    p_error_message     OUT VARCHAR2
)
AS
BEGIN
    p_success := 1; -- Default to success
    p_error_message := NULL;
    
    BEGIN
        EXECUTE IMMEDIATE p_insert_statement;
    EXCEPTION
        WHEN OTHERS THEN
            p_success := 0;
            p_error_message := SQLERRM;
    END;
END PSM_EXECUTE_BULK_INSERT;
/