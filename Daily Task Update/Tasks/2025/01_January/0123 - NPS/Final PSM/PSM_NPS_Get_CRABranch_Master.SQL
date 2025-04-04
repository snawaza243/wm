create or replace PROCEDURE PSM_NPS_Get_CRABranch_Master (
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_branch_cursor FOR
    SELECT BRANCH_CODE, BRANCH_NAME
    FROM BRANCH_MASTER BM
    ORDER BY BRANCH_NAME; 
END PSM_NPS_Get_CRABranch_Master;