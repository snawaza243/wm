     
        
create or replace PROCEDURE PSM_LOGIN_DATA (
    p_loginId IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR

     SELECT  LOGIN_ID, ROLE_ID 
        FROM USERDETAILS_Ji where LOGIN_ID = p_loginId  
        -- and ROLE_ID  = 212 AND ROWNUM = 1
        ;

END PSM_LOGIN_DATA; 
        