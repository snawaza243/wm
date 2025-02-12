select * from client_master where client_code = '41005971';

select * from client_test where client_code = 'AH128930';

select GENDER from investor_master where inv_code = '41005971006';
SELECT GUEST_CD , CLIENT_CODE FROM CLIENT_MASTER WHERE CLIENT_CODE = '42337048';





SELECT CITY_NAME FROM CITY_MASTER WHERE CITY_ID = 'C0914';
-- AH1932922,  
-- 41005971 AH128930

psm_ana_bmbylog


select client_code, main_code from client_test where client_codekyc = '40481416001';



SELECT DISTINCT GENDER FROM INVESTOR_MASTER WHERE INV_CODE = '41005971003';

SELECT DISTINCT act_cat FROM CLIENT_TEST WHERE CLIENT_CODEKYC = '41005971003';

select inv_code from tb_doc_upload where common_id = '40108761';
select * from psm_tv_dt;
PSM_AO_ApproveClient;

select client_code from client_test where client_test.approved = 'NO' and client_code = main_code;

select MARITAL_STATUS from client_test where source_code = '40264118';

create or replace PROCEDURE psm_ao_inv_check_head(
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
            ON SUBSTR(im.inv_code, 1, 8) = ct.source_code  
        WHERE ct.client_codekyc = p_inv_code
        GROUP BY ct.client_code, ct.main_code;
    end if;
END psm_ao_inv_check_head;
