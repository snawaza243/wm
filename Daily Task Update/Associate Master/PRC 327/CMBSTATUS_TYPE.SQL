create or replace PROCEDURE PSM_AM_OTHER_TYPE (
    p_cursor OUT SYS_REFCURSOR
) 
IS
BEGIN
    -- Open a cursor 
    OPEN p_cursor FOR
        select itemserialnumber, itemname from fixeditem where itemtype = '5' ORDER BY ITEMNAME ;

END;