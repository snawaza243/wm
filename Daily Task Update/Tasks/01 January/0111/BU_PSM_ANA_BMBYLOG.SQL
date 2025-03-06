create or replace PROCEDURE PSM_ANA_BMBYLOG (
    p_login_id in VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
v_g_filter number;
BEGIN


    select DATA_FILTER
    into v_g_filter
    from datafilter_master where login_id = p_login_id
    AND DATA_FILTER = '72'
    AND ROWNUM = 1
    ;


    IF v_g_filter = '72' THEN
        OPEN p_cursor FOR
            SELECT branch_code, branch_name
            FROM branch_master
            WHERE category_id NOT IN ('1004', '1005', '1006')
            ORDER BY branch_name;
    ELSE
        OPEN p_cursor FOR
            SELECT branch_code, branch_name
            FROM branch_master
            WHERE branch_code IN (
              SELECT bRANCH_ID 
            FROM USERDETAILS_Ji 
            WHERE LOGIN_ID = p_login_id
            )
              AND category_id NOT IN ('1004', '1005', '1006')
            ORDER BY branch_name; 
    END IF;
END PSM_ANA_BMBYLOG;