create or replace PROCEDURE PSM_ASS_M_Get_Branch_Master (
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_branch_cursor FOR
    SELECT *
    FROM BRANCH_MASTER
    ORDER BY BRANCH_NAME; 
END PSM_ASS_M_Get_Branch_Master;