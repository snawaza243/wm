

CREATE OR REPLACE PROCEDURE FETCH_EMPLOYEE_MASTER (
    p_srm_code IN VARCHAR2,
    p_branch IN VARCHAR2,
    p_branches IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    IF p_srm_code IS NULL OR p_srm_code = '' THEN
        IF p_branch IS NOT NULL THEN
            OPEN p_cursor FOR
                SELECT RM_CODE, RM_NAME, PAYROLL_ID
                FROM EMPLOYEE_MASTER
                WHERE 
                source = p_branch
                  AND (type = 'A' OR type IS NULL)
                ORDER BY RM_NAME;
        ELSE
            OPEN p_cursor FOR
                SELECT RM_CODE, RM_NAME, PAYROLL_ID
                FROM EMPLOYEE_MASTER
                WHERE source IN (
                    SELECT REGEXP_SUBSTR(p_branches, '[^,]+', 1, LEVEL)
                    FROM DUAL
                    CONNECT BY REGEXP_SUBSTR(p_branches, '[^,]+', 1, LEVEL) IS NOT NULL
                )
                  AND (type = 'A' OR type IS NULL)
                ORDER BY RM_NAME;
        END IF;
    ELSE
        OPEN p_cursor FOR
            SELECT RM_CODE, RM_NAME, PAYROLL_ID
            FROM EMPLOYEE_MASTER
            WHERE rm_code = p_srm_code
            ORDER BY RM_NAME;
    END IF;
END FETCH_EMPLOYEE_MASTER;








