create or replace PROCEDURE PSM_NPS_GET_SCH_CODE 
(
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    select DISTINCT SCH_CODE from transaction_st 
    where  SCH_CODE IN ('OP#09971','OP#09972','OP#09973'); 

END PSM_NPS_GET_SCH_CODE;