CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_STATE_LIST(
    p_country_id IN VARCHAR2,
    p_state_id   IN varchar2,
    p_city_id    IN varchar2,
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

these are tables and columns
in city_master table: city_id, city_name, state_id, del_flag(null or D)
in state_master table: state_id,, state_name, coutnry_id del_flag(null or D)
in country_master country_id, coutnry_name

list unique state name and state id, on  the passing the country id or city id or state id