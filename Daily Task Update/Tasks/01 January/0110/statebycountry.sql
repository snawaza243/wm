create or replace PROCEDURE PSM_AO_GetStatesByCountry (
    p_country_id IN NUMBER,
    p_states_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_states_cursor FOR
    SELECT DISTINCT sm.state_id, sm.state_name
    FROM state_master sm
    WHERE sm.country_id = p_country_id
    and sm.state_id is not null
    and sm.country_id is not null
    and sm.del_flag is null
    ORDER BY sm.state_name;
END PSM_AO_GetStatesByCountry;