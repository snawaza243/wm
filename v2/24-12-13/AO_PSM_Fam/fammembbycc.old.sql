create or replace PROCEDURE PSM_AO_GET_MEMBERDATABYCLIENTCODE (
    P_CLIENT_CODE IN VARCHAR2,
    P_MEMBERDATA OUT SYS_REFCURSOR
) AS
BEGIN
    -- Open the cursor to fetch the member data based on client code
    OPEN P_MEMBERDATA FOR
    SELECT 
        im.investor_title     as title,
        im.investor_name     as client_Name, 
        im.mobile     as MOBILE_NO, 
        im.email     as EMAIL, 
        im.pan     as CLIENT_PAN,
        im.aadhar_card_no     as aadhar_card_no,
        im.dob     as DOB, 
        im.rel_id     as relation_id, 

        im.g_name     as g_name, 
        im.g_pan     as G_PAN, 
        im.OCCUPATION_ID     as occ_id, 
        im.kyc     as kyc_status, 
        im.APPROVED     as approved_flag, 
        im.gender     as GENDER
    FROM investor_master im
    WHERE im.inv_code = P_CLIENT_CODE;

EXCEPTION
    WHEN NO_DATA_FOUND THEN 
        NULL;
    WHEN OTHERS THEN 
        RAISE;
END;