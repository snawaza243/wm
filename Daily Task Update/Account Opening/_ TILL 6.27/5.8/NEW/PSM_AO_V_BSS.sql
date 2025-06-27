CREATE OR REPLACE PROCEDURE PSM_AO_V_BSS (
    P_PAYROLL_ID     IN  VARCHAR2,
    P_BRANCH_LIST    IN  VARCHAR2,
    P_SOURCE         OUT VARCHAR2,
    P_BRANCH_NAME    OUT VARCHAR2,
    P_RM_NAME        OUT VARCHAR2,
    P_RM_CODE        OUT VARCHAR2,
    P_RESULT_FLAG    OUT NUMBER -- 1 = success, 2 = fallback found, 0 = not found
)
AS
BEGIN
    -- First attempt: with category filter
    BEGIN
        SELECT e.source, b.branch_name, e.rm_name, e.rm_code
        INTO P_SOURCE, P_BRANCH_NAME, P_RM_NAME, P_RM_CODE
        FROM employee_master e
        JOIN branch_master b ON e.source = b.branch_code
        WHERE b.branch_code IN (
            SELECT REGEXP_SUBSTR(P_BRANCH_LIST, '[^,]+', 1, LEVEL) 
            FROM dual 
            CONNECT BY REGEXP_SUBSTR(P_BRANCH_LIST, '[^,]+', 1, LEVEL) IS NOT NULL
        )
          AND e.payroll_id = P_PAYROLL_ID
          AND (e.type = 'A' OR e.type IS NULL)
          AND e.category_id IN ('2001', '2018');

        P_RESULT_FLAG := 1;
        RETURN;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL; -- try fallback
    END;

    -- Fallback: no category filter
    BEGIN
        SELECT e.source, b.branch_name, e.rm_name, e.rm_code
        INTO P_SOURCE, P_BRANCH_NAME, P_RM_NAME, P_RM_CODE
        FROM employee_master e
        JOIN branch_master b ON e.source = b.branch_code
        WHERE b.branch_code IN (
            SELECT REGEXP_SUBSTR(P_BRANCH_LIST, '[^,]+', 1, LEVEL) 
            FROM dual 
            CONNECT BY REGEXP_SUBSTR(P_BRANCH_LIST, '[^,]+', 1, LEVEL) IS NOT NULL
        )
          AND e.payroll_id = P_PAYROLL_ID
          AND (e.type = 'A' OR e.type IS NULL);

        P_RESULT_FLAG := 2;
        RETURN;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            P_RESULT_FLAG := 0;
    END;
END;
/
