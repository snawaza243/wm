create or replace PROCEDURE PSM_GET_USERBYLOGIN (
    p_login_id IN VARCHAR2, 
    p_user_details OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN p_user_details FOR
        SELECT * 
        FROM USERDETAILS_Ji
        WHERE LOGIN_ID = p_login_id;
END PSM_GET_USERBYLOGIN;
