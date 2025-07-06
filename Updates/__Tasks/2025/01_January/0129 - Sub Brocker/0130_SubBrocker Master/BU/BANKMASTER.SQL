create or replace PROCEDURE PSM_AM_GET_BANK_MASTER_DETAILS(p_cursor OUT SYS_REFCURSOR)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT 
            BANK_NAME,
            BRANCH,
            BRANCH_CODE,
            BANK_ID
        FROM BANK_MASTER;
END;