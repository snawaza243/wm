create or replace PROCEDURE PSM_AM_GET_BANK_BRANCH(
p_bank_branch_mark IN VARCHAR2,
p_branch_by_bank IN VARCHAR2,
p_cursor OUT SYS_REFCURSOR)
AS
BEGIN

    IF p_bank_branch_mark IS NOT NULL THEN
        OPEN p_cursor FOR
           SELECT BANK_NAME, BRANCH, BRANCH_CODE, BANK_ID
                FROM (
                    SELECT BANK_NAME, BRANCH, BRANCH_CODE, BANK_ID,
                           ROW_NUMBER() OVER (PARTITION BY BRANCH ORDER BY BANK_ID) AS rn
                    FROM BANK_MASTER
                    WHERE TRIM(BRANCH) IS NOT NULL
                ) 
            WHERE rn = 1
            ORDER BY BRANCH
        ;
    ELSE

    OPEN p_cursor FOR
        SELECT 
            BANK_NAME,
            BRANCH,
            BRANCH_CODE,
            BANK_ID
        FROM BANK_MASTER
        WHERE 
        TRIM(BANK_NAME) IS NOT NULL
        AND TRIM(BANK_ID) IS NOT NULL  ORDER BY BANK_NAME;
    END IF;

END;