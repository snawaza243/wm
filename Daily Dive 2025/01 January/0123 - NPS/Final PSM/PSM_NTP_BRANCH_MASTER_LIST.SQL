create or replace PROCEDURE PSM_NTP_BRANCH_MASTER_LIST(   
    p_result OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_result FOR
    SELECT branch_name, branch_code
    FROM branch_master
    ORDER BY branch_name;
END PSM_NTP_BRANCH_MASTER_LIST;