create or replace PROCEDURE PSM_AM_RM_BY_BRANCHDT (
    p_source_id     VARCHAR2,
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_branch_cursor FOR
    SELECT a.rm_name, a.rm_code, a.payroll_id,a.source
    FROM employee_master a
    WHERE a.source = p_source_id;
END PSM_AM_RM_BY_BRANCHDT;