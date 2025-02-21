create or replace PROCEDURE PSM_AM_Get_Location_List (
p_city_by_branch VARCHAR,
location_cursor OUT SYS_REFCURSOR)
IS
BEGIN
  OPEN location_cursor FOR
    SELECT LOCATION_ID, LOCATION_NAME, PINCODE, CITY_ID, PINCODE, LOGGEDUSERID, TIMEST
    FROM LOCATION_MASTER
    WHERE city_id = (Select branch_master.city_id from branch_master where branch_code = p_city_by_branch)
    ORDER BY LOCATION_NAME DESC;
END;