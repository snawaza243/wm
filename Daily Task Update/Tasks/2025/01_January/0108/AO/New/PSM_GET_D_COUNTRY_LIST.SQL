CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_D_COUNTRY_LIST (
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
            cmn.COUNTRY_ID,
            cmn.COUNTRY_NAME
        FROM
            COUNTRY_MASTER cmn
        WHERE
             cmn.COUNTRY_ID = p_country_id
        ORDER BY
            cmn.COUNTRY_NAME;

    ELSIF p_state_id IS NOT NULL THEN
        -- Query for state ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            cmn.COUNTRY_ID,
            cmn.COUNTRY_NAME
        FROM
            COUNTRY_MASTER cmn
        JOIN STATE_MASTER sm
            ON cmn.COUNTRY_ID = sm.COUNTRY_ID
        WHERE
            sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND sm.STATE_ID = p_state_id
        ORDER BY
            cmn.COUNTRY_NAME;

    ELSIF p_city_id IS NOT NULL THEN
        -- Query for city ID
        OPEN p_cursor FOR
        SELECT DISTINCT
            cmn.COUNTRY_ID,
            cmn.COUNTRY_NAME
        FROM
            COUNTRY_MASTER cmn
        JOIN STATE_MASTER sm
            ON cmn.COUNTRY_ID = sm.COUNTRY_ID
        JOIN CITY_MASTER cm
            ON sm.STATE_ID = cm.STATE_ID
        WHERE
             sm.DEL_FLAG IS NULL -- Exclude deleted states
            AND cm.DEL_FLAG IS NULL -- Exclude deleted cities
            AND cm.CITY_ID = p_city_id
        ORDER BY
            cmn.COUNTRY_NAME;

    ELSE
        -- Query for all countries if no parameters are passed
        OPEN p_cursor FOR
        SELECT DISTINCT
            cmn.COUNTRY_ID,
            cmn.COUNTRY_NAME
        FROM
            COUNTRY_MASTER cmn

        ORDER BY
            cmn.COUNTRY_NAME;
    END IF;
END PSM_GET_D_COUNTRY_LIST;
/
