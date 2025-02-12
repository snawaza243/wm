

create or replace PROCEDURE PSM_IRP_GET_PLAN_LIST(
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    SELECT 
        plan_no ,
        plan AS plan_name1
    FROM 
        bajaj_plan_master
    ORDER BY 
        plan;
END PSM_IRP_GET_PLAN_LIST;