
CREATE OR REPLACE PROCEDURE psm_ao_get_ac_cat(
    p_cursor OUT SYS_REFCURSOR  -- Use SYS_REFCURSOR to return query results
) 
IS 
BEGIN
    -- Open the cursor to fetch records from FIXEDITEM where itemtype = 38
    OPEN p_cursor FOR 
    SELECT * FROM FIXEDITEM WHERE itemtype = 38 ORDER BY ITEMID;
END psm_ao_get_ac_cat;
