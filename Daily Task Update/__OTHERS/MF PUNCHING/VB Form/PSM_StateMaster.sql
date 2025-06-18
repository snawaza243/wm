CREATE OR REPLACE PROCEDURE psm_state_master (
    p_login    VARCHAR2 DEFAULT NULL,
    p_role     VARCHAR2 DEFAULT NULL,
    p_country  VARCHAR2 DEFAULT NULL,
    p_state    VARCHAR2 DEFAULT NULL,
    p_cursor   OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT sm.*
        FROM state_master sm
        JOIN country_master cm ON sm.country_id = cm.country_id
        WHERE sm.del_flag IS NULL
          AND (
                (p_country IS NOT NULL AND cm.country_id = p_country) OR
                (p_state IS NOT NULL AND sm.state_id = p_state) OR
                (p_country IS NULL AND p_state IS NULL)
              )
        ORDER BY sm.state_name;
END;
