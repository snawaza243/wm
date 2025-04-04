create or replace PROCEDURE PSM_AM_AGENT_LIST5H (
    p_cursor OUT SYS_REFCURSOR
) 
IS
BEGIN
    OPEN p_cursor FOR
    SELECT agent_code, agent_name
    FROM agent_master

    where 
    AGENT_CODE IS NOT NULL 
    AND rownum <500
    order by agent_name

    ;
END;