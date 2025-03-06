create or replace PROCEDURE PSM_AO_MM_GETBYCC (
    P_CLIENT_CODE IN VARCHAR2,
    P_MEMBERDATA OUT SYS_REFCURSOR
) AS
BEGIN
    -- Open the cursor to fetch the member data based on client code
    OPEN P_MEMBERDATA FOR
        SELECT 
        ct.title as investor_title,
        ct.client_name as investor_name,
        ct.gender as gender,       
        ct.mobile_no as mobile,  
        ct.dob  as dob,
        ct.email as email,              
        ct.client_pan as pan,
        ct.relation_id as OUR_RELATIONSHIP,
        ct.kyc_status as KYC,
        ct.g_name as g_name,
        ct.g_pan as g_pan,
        ct.occ_id as occ_id,
        ct.approved as approved,
        ct.AADHAR_CARD_NO as AADHAR_CARD_NO,
        ct.is_nominee as is_nominee,                     
        ct.nominee_per as nominee_per        
        FROM client_test ct
    WHERE ct.client_codekyc = P_CLIENT_CODE;

EXCEPTION
    WHEN NO_DATA_FOUND THEN 
        NULL;
    WHEN OTHERS THEN 
        RAISE;
END;