CREATE OR REPLACE PROCEDURE psm_city_master (
    p_login     VARCHAR2 DEFAULT NULL,
    p_role      VARCHAR2 DEFAULT NULL,
    p_country   VARCHAR2 DEFAULT NULL,
    p_state     VARCHAR2 DEFAULT NULL,
    p_city      VARCHAR2 DEFAULT NULL,
    p_cursor    OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT cm.*
        FROM city_master cm
        JOIN state_master sm ON cm.state_id = sm.state_id
        JOIN country_master cn ON sm.country_id = cn.country_id
        WHERE cm.del_flag IS NULL
          AND (p_country IS NULL OR cn.country_id = p_country)
          AND (p_state IS NULL OR sm.state_id = p_state)
          AND (p_city IS NULL OR cm.city_id = p_city)
        ORDER BY cm.city_name;
END;
