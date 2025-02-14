create or replace PROCEDURE PSM_ANA_Get_AGENTLIST (
    p_sourceid       IN VARCHAR2,    -- New parameter: sourceid
    p_rm_code        IN VARCHAR2,    -- Existing parameter: rm_code
    p_agent_name     IN VARCHAR2,    -- New parameter: agent_name
    p_exist_code     IN VARCHAR2,    -- New parameter: exist_code
    p_agents_cursor  OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_agents_cursor FOR
    SELECT AGENT_CODE, EXIST_CODE, AGENT_NAME, JOININGDATE, Address1, Address2, sourceid
    FROM AGENT_MASTER
    WHERE 
        (p_sourceid IS NULL OR SOURCEID = p_sourceid) -- Filter by sourceid if provided
        AND (p_rm_code IS NULL OR RM_CODE = p_rm_code) -- Filter by rm_code if provided
        AND (p_agent_name IS NULL OR UPPER(trim(AGENT_NAME)) LIKE '%' || UPPER(trim(p_agent_name)) || '%') 
        AND (p_exist_code IS NULL OR trim(EXIST_CODE) = trim(p_exist_code))

        ;
END PSM_ANA_Get_AGENTLIST;