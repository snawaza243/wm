create or replace PROCEDURE PSM_ANA_Get_AGENTLIST (
    p_sourceid       IN VARCHAR2,    -- New parameter: sourceid
    p_rm_code        IN VARCHAR2,    -- Existing parameter: rm_code
    p_agent_name     IN VARCHAR2,    -- New parameter: agent_name
    p_exist_code     IN VARCHAR2,    -- New parameter: exist_code
    p_mobile         IN VARCHAR2,
    p_phone          IN VARCHAR2,
    p_agent_code     IN VARCHAR2,
    p_pan            IN VARCHAR2,
    p_branches       IN VARCHAR2,
    p_agents_cursor  OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_agents_cursor FOR
    SELECT AGENT_CODE, EXIST_CODE, AGENT_NAME, JOININGDATE, Address1, Address2, sourceid
    FROM AGENT_MASTER
    WHERE 
        (p_sourceid IS NULL OR SOURCEID = p_sourceid) -- Filter by sourceid if provided
        AND (p_rm_code IS NULL OR RM_CODE = p_rm_code) -- Filter by rm_code if provided
        AND (p_agent_name IS NULL OR UPPER(trim(AGENT_NAME)) LIKE UPPER(trim(p_agent_name)) || '%') 
        AND (p_exist_code IS NULL OR trim(EXIST_CODE) = trim(p_exist_code))

        AND (p_mobile IS NULL OR trim(mobile) = trim(p_mobile))        
        AND (p_phone IS NULL OR trim(phone) = trim(p_phone))        
        AND (p_agent_code IS NULL OR trim(agent_code) = trim(p_agent_code))        
        AND (p_pan IS NULL OR trim(upper(pan)) = trim(upper(p_pan)))
        AND SOURCEID in ( SELECT REGEXP_SUBSTR(p_branches, '[^,]+', 1, LEVEL) FROM DUAL CONNECT BY REGEXP_SUBSTR(p_branches, '[^,]+', 1, LEVEL) IS NOT NULL )

        ;
END PSM_ANA_Get_AGENTLIST;
