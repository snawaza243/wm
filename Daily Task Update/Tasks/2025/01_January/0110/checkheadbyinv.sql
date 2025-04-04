CREATE OR REPLACE PROCEDURE psm_ao_inv_check_head(
    p_inv_code IN VARCHAR2, 
    p_cursor OUT SYS_REFCURSOR
) AS

v_inv_flag number;
BEGIN

    select nvl(count(inv_code), 0)
    into v_inv_flag
    from investor_master 
    where inv_code = (select client_codekyc from client_test where client_codekyc = p_inv_code);

    if v_inv_flag = 0 then 
        OPEN p_cursor FOR 
            SELECT 'Invalid: Investor not in client test' as message from dual;
    else
    
    OPEN p_cursor FOR
        SELECT 
            CASE
                WHEN ct.client_code = ct.main_code THEN 'head'
                WHEN ct.client_code != ct.main_code THEN 'member'
            END AS message,
            ct.client_code as client_code, 
            ct.main_code AS main_code
        FROM client_test ct
        JOIN investor_master im 
            ON im.inv_code = ct.client_codekyc  
        WHERE im.inv_code = '40481416001'
        GROUP BY ct.client_code, ct.main_code;
    end if;
END psm_ao_inv_check_head;

/