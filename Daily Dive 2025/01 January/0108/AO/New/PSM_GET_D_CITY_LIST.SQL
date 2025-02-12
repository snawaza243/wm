CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_D_CITY_LIST (
    p_country_id IN VARCHAR2,
    p_state_id   IN VARCHAR2,
    p_city_id    IN VARCHAR2,
    p_cursor     OUT SYS_REFCURSOR
) AS
BEGIN
    -- Conditional logic based on input parameters
    IF p_country_id IS NOT NULL THEN
        -- Query for country ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            cm.CITY_ID,
            cm.CITY_NAME
        FROM
            CITY_MASTER cm
        JOIN STATE_MASTER sm
            ON cm.STATE_ID = sm.STATE_ID
        WHERE
            cm.DEL_FLAG IS NULL -- Exclude deleted cities
            AND sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND sm.COUNTRY_ID = p_country_id
        ORDER BY
            cm.CITY_NAME;

    ELSIF p_state_id IS NOT NULL THEN
        -- Query for state ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            cm.CITY_ID,
            cm.CITY_NAME
        FROM
            CITY_MASTER cm
        WHERE
            cm.DEL_FLAG IS NULL -- Exclude deleted cities
            AND cm.STATE_ID = p_state_id
        ORDER BY
            cm.CITY_NAME;

    ELSIF p_city_id IS NOT NULL THEN
        -- Query for city ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            cm.CITY_ID,
            cm.CITY_NAME
        FROM
            CITY_MASTER cm
        WHERE
            cm.DEL_FLAG IS NULL -- Exclude deleted cities
            AND cm.CITY_ID = p_city_id
        ORDER BY
            cm.CITY_NAME;

    ELSE
        -- Query for all cities if no parameters are passed
        OPEN p_cursor FOR
        SELECT DISTINCT
            cm.CITY_ID,
            cm.CITY_NAME
        FROM
            CITY_MASTER cm
        WHERE
            cm.DEL_FLAG IS NULL -- Exclude deleted cities
        ORDER BY
            cm.CITY_NAME;
    END IF;
END PSM_GET_D_CITY_LIST;
/
