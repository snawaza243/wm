CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_STATE_LIST(
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    SELECT STATE_ID, STATE_NAME
    FROM STATE_MASTER 
    where del_flag is null 
   and STATE_ID is not null
   AND COUNTRY_ID IS NOT NULL
   AND STATE_NAME IN (SELECT STATE_NAME FROM STATE_MASTER)
    order by state_name;  
END PSM_GET_STATE_LIST;
/