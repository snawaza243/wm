CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_D_STATE_LIST (
    p_country_id IN VARCHAR2,
    p_state_id   IN VARCHAR2,
    p_city_id    IN VARCHAR2,
    p_cursor     OUT SYS_REFCURSOR
) AS
    v_query VARCHAR2(4000); -- Variable to hold the dynamic SQL query
BEGIN
    -- Base query
    v_query := 'SELECT DISTINCT sm.STATE_ID, sm.STATE_NAME ' ||
               'FROM STATE_MASTER sm ' ||
               'LEFT JOIN CITY_MASTER cm ON sm.STATE_ID = cm.STATE_ID ' ||
               'LEFT JOIN COUNTRY_MASTER cmn ON sm.COUNTRY_ID = cmn.COUNTRY_ID ' ||
               'WHERE sm.DEL_FLAG IS NULL ' ||
               'AND (cm.DEL_FLAG IS NULL)' ||
               'AND (SM.COUNTRY_ID IS NOT NULL)' ||


               'AND (cm.DEL_FLAG IS NULL)';

    -- Add dynamic filters based on the input parameters
    IF p_country_id IS NOT NULL THEN
        v_query := v_query || ' AND sm.COUNTRY_ID = ''' || p_country_id || '''';
    END IF;

    IF p_state_id IS NOT NULL THEN
        v_query := v_query || ' AND sm.STATE_ID = ''' || p_state_id || '''';
    END IF;

    IF p_city_id IS NOT NULL THEN
        v_query := v_query || ' AND cm.CITY_ID = ''' || p_city_id || '''';
    END IF;

    -- Add the ORDER BY clause
    v_query := v_query || ' ORDER BY sm.STATE_NAME';

    -- Open the cursor with the dynamically constructed query
    OPEN p_cursor FOR v_query;
END PSM_GET_D_STATE_LIST;
/
