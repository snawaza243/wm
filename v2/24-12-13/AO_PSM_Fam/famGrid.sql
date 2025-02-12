create or replace PROCEDURE PSM_AO_GET_FAMILY_GRID (
    p_source_code    IN  investor_master.source_id%TYPE,
    p_exist IN  investor_master.inv_code%TYPE,
    p_cursor OUT SYS_REFCURSOR   
) AS
BEGIN
    -- Open a cursor to return the result of the select query
    OPEN p_cursor FOR

        SELECT 

        ct.investor_title as investor_title,
        ct.investor_name as investor_name,
        ct.gender as gender,       
        ct.mobile as mobile,   
        ct.email as email,              
        ct.pan as pan,
        ct.OUR_RELATIONSHIP as OUR_RELATIONSHIP,
        ct.KYC as KYC,
        ct.g_name as g_name,
        ct.g_pan as g_pan,
        ct.approved as approved,
        ct.AADHAR_CARD_NO as AADHAR_CARD_NO,
        ct.is_nominee as is_nominee,                     
        ct.nominee_per as nominee_per,

        
        
        FROM client_test ct

        WHERE ct.source_id = p_source_code
          AND inv_code NOT IN (p_exist);

        --AND client_code != main_code;
END;