create or replace PROCEDURE PSM_AM_GET_EXAMLIST (
    p_cursor OUT SYS_REFCURSOR
) 
IS
BEGIN
    -- Open a cursor 
    OPEN p_cursor FOR
    select itemserialnumber, itemname 
    from fixeditem where itemtype = '6'
    ORDER BY itemname;

END;