create or replace PROCEDURE PSM_AO_ExisingInvestorList (
    p_source_id    IN  client_test.source_code%TYPE,
    p_excluded_inv IN  client_test.client_codekyc%TYPE,
    p_investor_list OUT SYS_REFCURSOR
)
IS
BEGIN
    -- Open the cursor to fetch the required records
    OPEN p_investor_list FOR
        SELECT 
        client_name as investor_name, 
        client_codekyc as inv_code
        FROM client_test
        WHERE client_code != main_code
        and source_code  = p_source_id;
        
         -- AND inv_code NOT IN (p_excluded_inv) order by investor_name;
END;
