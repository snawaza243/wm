create or replace PROCEDURE             PSM_GET_MASTER_STATE(
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    SELECT STATE_ID, STATE_NAME
    FROM STATE_MASTER 
    where del_flag is null and STATE_ID is not null AND COUNTRY_ID IS NOT NULL order by state_name;  
END PSM_GET_MASTER_STATE;