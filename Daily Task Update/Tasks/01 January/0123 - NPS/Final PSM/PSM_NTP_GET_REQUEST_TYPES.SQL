create or replace PROCEDURE PSM_NTP_GET_REQUEST_TYPES (
p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
    SELECT request_code, request_name 
    FROM request_master 
    ORDER BY request_code;
END PSM_NTP_GET_REQUEST_TYPES;