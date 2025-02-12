create or replace PROCEDURE PSM_AM_GET_BANKACCTYPEID (
    p_cursor OUT SYS_REFCURSOR
) 
IS
BEGIN
    -- Open a cursor 
    OPEN p_cursor FOR
    select itemserialnumber, itemname 
    from fixeditem where itemtype = '7' order by itemname;

END;
