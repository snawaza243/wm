create or replace PROCEDURE PSM_AO_MM_GETBYCC (
    P_CLIENT_CODE IN VARCHAR2,
    P_MEMBERDATA OUT SYS_REFCURSOR
) AS

v_ct_flag number;
v_im_flag number;
BEGIN

    select nvl(count(INV_CODE), 0)
    from investor_master WHERE INV_CODE = '41005971006';

    select nvl(count(CLIENT_CODEKYC), 0)
    INTO v_ct_flag
    from CLIENT_TEST WHERE CLIENT_CODEKYC = '41005971006';
    
    IF v_ct_flag = 0 THEN
    OPEN P_MEMBERDATA FOR
        SELECT 
        IM.title as investor_title,
        IM.client_name as investor_name,
        IM.gender as gender,       
        IM.mobile_no as mobile,  
        IM.dob  as dob,
        IM.email as email,              
        IM.client_pan as pan,
        IM.relation_id as OUR_RELATIONSHIP,
        IM.kyc_status as KYC,
        IM.g_name as g_name,
        IM.g_pan as g_pan,
        IM.occ_id as occ_id,
        IM.approved as approved,
        IM.AADHAR_CARD_NO as AADHAR_CARD_NO,
        IM.is_nominee as is_nominee,                     
        IM.nominee_per as nominee_per        
        FROM INVESTOR_MASTER IM
    WHERE IM.INV_CODE = P_CLIENT_CODE;
    ELSE
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