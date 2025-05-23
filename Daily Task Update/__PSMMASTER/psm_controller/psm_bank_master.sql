CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_BANK_MASTER (
   P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR 
        SELECT * 
        FROM BANK_MASTER 
        WHERE BANK_NAME IS NOT NULL 
        AND BANK_ID IS NOT NULL
        ORDER BY BANK_NAME ASC;
END PSM_BANK_MASTER;
/
