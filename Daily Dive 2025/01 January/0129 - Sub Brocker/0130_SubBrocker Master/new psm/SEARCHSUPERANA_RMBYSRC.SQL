   
        
create or replace PROCEDURE PSM_AM_SANA_RMBSRC (
    p_login         varchar2,
    p_source_id     VARCHAR2,
    p_rm_code       VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    SELECT DISTINCT AM.RM_CODE, EM.RM_NAME
    FROM AGENT_MASTER AM
    JOIN EMPLOYEE_MASTER EM ON AM.RM_CODE = EM.RM_CODE
    WHERE AM.SOURCEID = p_source_id;

END PSM_AM_SANA_RMBSRC;
