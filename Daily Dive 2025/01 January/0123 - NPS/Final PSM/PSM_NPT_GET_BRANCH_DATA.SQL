create or replace PROCEDURE PSM_NPT_GET_BRANCH_DATA(
    p_branch_code IN NUMBER,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    IF p_branch_code = 0 THEN
        OPEN p_cursor FOR
            SELECT branch_code, branch_name, NSDL_NO
            FROM branch_master
            WHERE nsdl_no IS NOT NULL
            AND BRANCH_CODE = 10010044;
    ELSE
        OPEN p_cursor FOR
            SELECT branch_code, branch_name, KARVY_NSDL_NO AS NSDL_NO
            FROM branch_master
            WHERE KARVY_NSDL_NO IS NOT NULL
            AND BRANCH_CODE = 10010044;
    END IF;
END;