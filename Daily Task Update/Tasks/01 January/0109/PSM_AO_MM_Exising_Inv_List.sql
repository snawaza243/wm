create or replace PROCEDURE PSM_AO_MM_Existing_Inv_List (
    p_source_id    IN  investor_master.source_id%TYPE,
    p_excluded_inv IN  investor_master.inv_code%TYPE,
    p_investor_list OUT SYS_REFCURSOR
)
IS
BEGIN
    OPEN p_investor_list FOR
        SELECT 
        investor_name as investor_name, 
        inv_code as inv_code
        FROM investor_master
        WHERE  source_id  = p_source_id and inv_code not in p_excluded_inv
        order by investor_name;
END;

