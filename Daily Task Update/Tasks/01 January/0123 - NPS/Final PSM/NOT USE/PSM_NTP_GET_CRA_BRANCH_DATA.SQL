create or replace PROCEDURE PSM_NTP_GET_CRA_BRANCH_DATA (
    p_kra_value IN NUMBER,
    p_cursor OUT SYS_REFCURSOR
)
AS
    v_sql_query VARCHAR2(1000);
BEGIN
    IF p_kra_value = 0 THEN
        v_sql_query := 'SELECT branch_code, branch_name, NSDL_NO 
                        FROM branch_master 
                        WHERE nsdl_no IS NOT NULL 
                          AND BRANCH_CODE = 10010044';
    ELSE
        v_sql_query := 'SELECT branch_code, branch_name, KARVY_NSDL_NO AS NSDL_NO 
                        FROM branch_master 
                        WHERE KARVY_NSDL_NO IS NOT NULL 
                          AND BRANCH_CODE = 10010044';
    END IF;

    OPEN p_cursor FOR v_sql_query;
END;