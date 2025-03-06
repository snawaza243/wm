create or replace PROCEDURE PSM_AM_AssociateType (
    p_cursor OUT SYS_REFCURSOR
) 
IS
BEGIN
    -- Open a cursor 
    OPEN p_cursor FOR
    select INVESTOR_TYPE, INVESTOR_CODE from investortype where investor_code not in (13, 14);

END;





