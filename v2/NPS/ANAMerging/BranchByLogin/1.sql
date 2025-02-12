CREATE OR REPLACE PROCEDURE FETCH_BRANCH_MASTER (
    p_glb_data_filter IN VARCHAR2,
    p_branches IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    IF p_glb_data_filter = '72' THEN
        OPEN p_cursor FOR
            SELECT branch_code, branch_name
            FROM branch_master
            WHERE category_id NOT IN ('1004', '1005', '1006')
            ORDER BY branch_name;
    ELSE
        OPEN p_cursor FOR
            SELECT branch_code, branch_name
            FROM branch_master
            WHERE branch_code IN (
                SELECT REGEXP_SUBSTR(p_branches, '[^,]+', 1, LEVEL)
                FROM DUAL
                CONNECT BY REGEXP_SUBSTR(p_branches, '[^,]+', 1, LEVEL) IS NOT NULL
            )
              AND category_id NOT IN ('1004', '1005', '1006')
            ORDER BY branch_name;
    END IF;
END FETCH_BRANCH_MASTER;
