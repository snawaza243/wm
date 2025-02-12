create or replace PROCEDURE PSM_ANA_Get_AGENTLIST2 (
    p_sourceid       IN VARCHAR2,
    p_rm_code        IN VARCHAR2,
    p_agent_name     IN VARCHAR2,
    p_exist_code     IN VARCHAR2,
    p_old_rm_code    IN VARCHAR2,
    p_old_exist_code IN VARCHAR2,
    p_mobile         IN VARCHAR2,
    p_phone          IN VARCHAR2,
    p_category       IN VARCHAR2,
    p_city           IN VARCHAR2,
    p_sort_by        IN VARCHAR2,
    p_address1       IN VARCHAR2,
    p_address2       IN VARCHAR2,
    p_pan            IN VARCHAR2,
    p_agents_cursor  OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_agents_cursor FOR
    SELECT 
        a.AGENT_CODE, 
        a.EXIST_CODE, 
        a.AGENT_NAME, 
        a.JOININGDATE, 
        a.ADDRESS1, 
        a.ADDRESS2, 
        a.SOURCEID,
        a.CITY_ID
    FROM AGENT_MASTER a
    LEFT JOIN CITY_MASTER c ON a.CITY_ID = c.CITY_ID
    WHERE 
        (p_sourceid IS NULL OR UPPER(a.SOURCEID) = UPPER(p_sourceid))
        AND (p_rm_code IS NULL OR UPPER(a.RM_CODE) = UPPER(p_rm_code))
        AND (p_agent_name IS NULL OR UPPER(a.AGENT_NAME) LIKE UPPER(p_agent_name) || '%')
        AND (p_exist_code IS NULL OR UPPER(a.EXIST_CODE) = UPPER(p_exist_code))
       AND (p_old_rm_code IS NULL OR UPPER(a.AGENT_CODE) = UPPER(p_old_rm_code))
        AND (p_old_exist_code IS NULL OR UPPER(a.agent_code) = UPPER(p_old_exist_code)) 
       AND (p_mobile IS NULL OR a.MOBILE = p_mobile)
     AND (p_phone IS NULL OR a.PHONE = p_phone)
        -- AND (p_category IS NULL OR UPPER(a.CATEGORY) = UPPER(p_category)) -- Uncomment if needed
       AND (p_city IS NULL OR UPPER(a.city_id) = UPPER(p_city)) -- Match city name
         AND (p_address1 IS NULL OR a.ADDRESS1 LIKE UPPER(p_address1) || '%')
    AND (p_address2 IS NULL OR a.ADDRESS2 LIKE UPPER(p_address2) || '%') 
       AND (p_pan IS NULL OR UPPER(a.PAN) = UPPER(p_pan)) -- Match PAN
    ORDER BY 

        CASE 
            WHEN p_sort_by = 'name' THEN UPPER(a.AGENT_NAME)
            WHEN p_sort_by = 'address1' THEN UPPER(a.ADDRESS1)
            WHEN p_sort_by = 'address2' THEN UPPER(a.ADDRESS2)
            WHEN p_sort_by = 'city' THEN UPPER(c.CITY_NAME)
            WHEN p_sort_by = 'phone' THEN a.PHONE
            ELSE UPPER(a.AGENT_CODE) -- Default sorting by agent code
        END;
END PSM_ANA_Get_AGENTLIST2;
