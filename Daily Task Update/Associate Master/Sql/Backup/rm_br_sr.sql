create or replace PROCEDURE PSM_ANA_SR_BR (
    p_source_id     VARCHAR2,
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_branch_cursor FOR
    SELECT a.rm_name as agent_name, a.rm_code as rm_code, a.payroll_id,a.source
    FROM employee_master a
    WHERE a.source = p_source_id and a.type='A'
    and a.category_id in (2001, 2018)  ;
END PSM_ANA_SR_BR;  