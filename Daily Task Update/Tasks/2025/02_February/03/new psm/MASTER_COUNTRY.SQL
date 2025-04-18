create or replace PROCEDURE PSM_Get_Master_Country
(
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    SELECT DISTINCT  COUNTRY_ID, TRIM(COUNTRY_NAME) AS COUNTRY_NAME
    FROM COUNTRY_MASTER
    WHERE COUNTRY_ID IS NOT NULL AND TRIM(COUNTRY_NAME) IS NOT NULL    
    ORDER BY COUNTRY_NAME ASC;

END PSM_Get_Master_Country;

