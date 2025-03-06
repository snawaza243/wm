create or replace PROCEDURE PSM_LOGIN_DATA (
    p_loginId IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
   -- SELECT USERNAME, ROLE, LAST_LOGIN_DATE FROM USERS WHERE LOGIN_ID = p_loginId;

    select DATA_FILTER, LOGIN_ID, ROLE_ID

    --into v_g_filter
    from datafilter_master where login_id = p_loginId
    --AND DATA_FILTER = '72' --AND ROWNUM = 1
    ;

END PSM_LOGIN_DATA;
