
create or replace PROCEDURE PSM_AO_MM_Existing_Inv_Grid (
    p_source_code    IN  client_test.source_code%TYPE,
    p_exist IN  client_test.client_codekyc%TYPE,
    p_cursor OUT SYS_REFCURSOR   
) AS
BEGIN
    -- Open a cursor to return the result of the select query
    OPEN p_cursor FOR

        SELECT 
        ct.title as investor_title,
        ct.client_name as investor_name,
        ct.gender as gender,       
        ct.mobile_no as mobile,   
        ct.email as email,              
        ct.client_pan as pan,
        nvl((select  RELATION_NAME from relationship_masteR WHERE  RELATION_ID = ct.relation_id),'') as OUR_RELATIONSHIP,
        ct.kyc_status as KYC,
        ct.g_name as g_name,
        ct.g_pan as g_pan,
        ct.approved as approved,
        ct.AADHAR_CARD_NO as AADHAR_CARD_NO,
        ct.is_nominee as is_nominee,                     
        ct.nominee_per as nominee_per        
        FROM client_test ct

        WHERE ct.source_code = p_source_code
        and ct.client_code != ct.main_code;
END;
