
select iss_name,iss_code from iss_master where iss_code='IS02520';

select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status;



DECLARE v_cursor SYS_REFCURSOR;
BEGIN
    v_cursor := PSM_GET_TABLE_COL('nps_nonecs_tbl_imp');
    -- You need to fetch data from v_cursor in PL/SQL
END;
/

delete PROCEDURE PSM_GET_TABLE_COL;

