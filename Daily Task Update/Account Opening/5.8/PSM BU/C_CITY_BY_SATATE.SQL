CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_GetCitiesByState (
    p_state_id IN NUMBER,
    p_cities_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cities_cursor FOR
    SELECT cm.city_id, cm.city_name
    FROM city_master cm
    WHERE 
    cm.state_id = p_state_id and 
    cm.city_id is not null
    and cm.state_id is not null
    AND CM.DEL_FLAG IS NULL
    ORDER BY cm.city_name;
END PSM_AO_GetCitiesByState;
/
