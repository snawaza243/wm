CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_D_STATE_LIST (
    p_country_id IN VARCHAR2,
    p_state_id   IN VARCHAR2,
    p_city_id    IN VARCHAR2,
    p_cursor     OUT SYS_REFCURSOR
) AS
BEGIN
    -- Open the cursor with the query logic based on input parameters
    IF p_country_id IS NOT NULL THEN
        -- Query for country ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            sm.STATE_ID,
            sm.STATE_NAME
        FROM
            STATE_MASTER sm
        WHERE
            sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND sm.STATE_ID IS NOT NULL
            AND UPPER(sm.COUNTRY_ID) = UPPER(p_country_id)
        ORDER BY
            sm.STATE_NAME;

    ELSIF p_city_id IS NOT NULL THEN
        -- Query for city ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            sm.STATE_ID,
            sm.STATE_NAME
        FROM
            STATE_MASTER sm
        JOIN CITY_MASTER cm
            ON sm.STATE_ID = cm.STATE_ID
        WHERE
            sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND cm.DEL_FLAG IS NULL -- Exclude deleted cities
            AND cm.CITY_ID = p_city_id
        ORDER BY
            sm.STATE_NAME;

    ELSIF p_state_id IS NOT NULL THEN
        -- Query for state ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            sm.STATE_ID,
            sm.STATE_NAME
        FROM
            STATE_MASTER sm
        WHERE
            sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND sm.STATE_ID = p_state_id
        ORDER BY
            sm.STATE_NAME;

    ELSE
        -- Query for all states if no parameters are passed
        OPEN p_cursor FOR
        SELECT DISTINCT
            sm.STATE_ID,
            sm.STATE_NAME
        FROM
            STATE_MASTER sm
        WHERE
            sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND sm.STATE_ID IS NOT NULL
            AND sm.COUNTRY_ID IS NOT NULL
        ORDER BY
            sm.STATE_NAME;
    END IF;
END PSM_GET_D_STATE_LIST;
/
