create or replace PROCEDURE PSAM_GET_BANK_MASTER_LIST (
   P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR 
    SELECT BANK_NAME, BRANCH_CODE 
    FROM BANK_MASTER

    ;
END PSAM_GET_BANK_MASTER_LIST;