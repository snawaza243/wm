create or replace PROCEDURE PSM_AO_GetCitiesByState (
    p_state_id IN NUMBER,
    p_cities_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cities_cursor FOR
    SELECT DISTINCT cm.city_id, cm.city_name
    FROM city_master cm
    WHERE cm.state_id = p_state_id
    and cm.city_id is not null
    and cm.state_id is not null
    ORDER BY cm.city_name;
END PSM_AO_GetCitiesByState;