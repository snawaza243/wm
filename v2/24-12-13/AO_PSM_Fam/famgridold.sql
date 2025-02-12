create or replace PROCEDURE PSM_AO_GET_FAMILY_GRID (
        p_source_code    IN  investor_master.source_id%TYPE,
    p_exist IN  investor_master.inv_code%TYPE,
    p_cursor OUT SYS_REFCURSOR   
) AS
BEGIN
    -- Open a cursor to return the result of the select query
    OPEN p_cursor FOR

          SELECT 
        investor_master.investor_title as investor_title,
        investor_master.investor_name as investor_name,
        investor_master.gender as gender,       
        investor_master.mobile as mobile,   
        investor_master.email as email,              
        investor_master.pan as pan,
        investor_master.OUR_RELATIONSHIP as OUR_RELATIONSHIP,
        investor_master.KYC as KYC,
        investor_master.g_name as g_name,
        investor_master.g_pan as g_pan,
        investor_master.approved as approved,
        investor_master.AADHAR_CARD_NO as AADHAR_CARD_NO,
        '' as is_nominee,                     
        '' as nominee_per
        FROM investor_master
        WHERE source_id = p_source_code
          AND inv_code NOT IN (p_exist);

        --AND client_code != main_code;
END;