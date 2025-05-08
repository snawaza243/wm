CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_MM_Insert (
    p_dt_number           IN VARCHAR2,
    p_exist_client_code   IN NUMBER,
    p_rm_business_code    IN VARCHAR2,
    p_salutation1         IN VARCHAR2,
    p_account_name        IN VARCHAR2,
    p_logged_in_user      IN VARCHAR2,
    p_client_code_in_main IN VARCHAR2,
    p_mobile              IN VARCHAR2,
    p_pan                 IN VARCHAR2,
    p_email               IN VARCHAR2,
    p_dob                 IN DATE,
    p_relation            IN NUMBER,
    p_gname               IN VARCHAR2,
    p_gpan                IN VARCHAR2,
    p_occupation          IN NUMBER,
    p_kyc                 IN VARCHAR2,
    p_approve             IN VARCHAR2,
    p_gender              IN VARCHAR2,
    p_nom                 IN VARCHAR2,
    p_allo                IN NUMBER,
    p_aadhar_value        IN VARCHAR2,
    p_result              OUT SYS_REFCURSOR
) AS
    v_generated_client_code client_test.client_code%TYPE;
    v_generated_inv_code investor_master.inv_code%TYPE;

    v_head_ah client_test.client_code%TYPE;
    nextinvkyc NUMBER;

    -- Cached data from main client
    v_branch_code         client_test.branch_code%TYPE;
    v_nationality         client_test.nationality%TYPE;
    v_resident_nri        client_test.resident_nri%TYPE;
    v_lead_type           client_test.lead_type%TYPE;
    v_g_nationality       client_test.g_nationality%TYPE;
    v_add1                client_test.add1%TYPE;
    v_add2                client_test.add2%TYPE;
    v_state_id            client_test.state_id%TYPE;
    v_city_id             client_test.city_id%TYPE;
    v_pincode             client_test.pincode%TYPE;
    v_std1                client_test.std1%TYPE;
    v_tel1                client_test.tel1%TYPE;
    v_std2                client_test.std2%TYPE;
    v_tel2                client_test.tel2%TYPE;

    -- Cached data from existing investor
    v_inv_branch_code     investor_master.branch_code%TYPE;
    v_inv_address1        investor_master.address1%TYPE;
    v_inv_address2        investor_master.address2%TYPE;
    v_inv_fax             investor_master.fax%TYPE;
    v_inv_pincode         investor_master.pincode%TYPE;    
    v_inv_phone           investor_master.phone%TYPE;
    v_inv_phone1          investor_master.phone1%TYPE;
    v_inv_phone2          investor_master.phone2%TYPE;
    v_inv_status          investor_master.status%TYPE;
    v_inv_city_id         investor_master.city_id%TYPE;
    v_inv_source_id       investor_master.source_id%TYPE;
    v_inv_rm_code         investor_master.rm_code%TYPE;

BEGIN

 

        -- Fetch head values from client_test table 
        SELECT nationality, resident_nri, lead_type, g_nationality,
               add1, add2, state_id, city_id, pincode, std1, tel1, 
               std2, tel2, branch_code, client_code
        INTO v_nationality, v_resident_nri, v_lead_type, v_g_nationality,
             v_add1, v_add2, v_state_id, v_city_id, v_pincode, v_std1, v_tel1,
             v_std2, v_tel2, v_branch_code, v_head_ah
        FROM client_test
        WHERE client_codekyc = p_exist_client_code;



        -- Fetch head values from the investor_master table
        SELECT  branch_code, address1, address2, fax, 
                pincode, phone, phone1, phone2, status, 
                city_id, source_id, rm_code
        INTO    v_inv_branch_code, v_inv_address1, v_inv_address2, v_inv_fax, 
                v_inv_pincode, v_inv_phone, v_inv_phone1, v_inv_phone2, v_inv_status, 
                v_inv_city_id, v_inv_source_id, v_inv_rm_code
        FROM investor_master
        WHERE inv_code = p_exist_client_code;



    IF p_exist_client_code IS NOT NULL THEN
        -- Insert into CLIENT_TEST
        INSERT INTO client_test (
            client_code, clientcode_old, business_code, title, client_name,
            loggeduserid, mobile_no, email, client_pan,
            dob, relation_id, g_name, g_pan, occ_id, INV_KYC, approved,
            gender, aadhar_card_no, timest, is_nominee, nominee_per,

            nationality, resident_nri, lead_type, g_nationality, add1, add2, 
            state_id, city_id, pincode, std1, tel1, std2, tel2, branch_code, main_code
        )
        VALUES (
            NULL, p_exist_client_code, p_rm_business_code, p_salutation1, p_account_name, 
            p_logged_in_user, p_mobile, p_email, p_pan, p_dob, p_relation, p_gname,
            p_gpan, p_occupation, p_kyc, p_approve,
            p_gender, p_aadhar_value, SYSDATE, p_nom, p_allo,

            v_nationality, v_resident_nri, v_lead_type, v_g_nationality, v_add1, v_add2, 
            v_state_id, v_city_id, v_pincode, v_std1, v_tel1, v_std2, v_tel2, v_branch_code, v_head_ah
        )
        RETURNING client_code INTO v_generated_client_code;

        -- Insert into INVESTOR_MASTER
        INSERT INTO investor_master (
            inv_code, investor_title, investor_name, mobile, email, pan,
            dob, rel_id, g_name, g_pan, occupation_id, kyc, approved,
            gender, timest, loggeduserid, aadhar_card_no, source_id,

            branch_code, address1, address2, fax, pincode, phone, 
            phone1, phone2, status, city_id, rm_code
        )
        VALUES (
            NULL, p_salutation1, p_account_name, p_mobile, p_email, p_pan,
            p_dob, p_relation, p_gname, p_gpan, p_occupation, 'YES',
            p_approve, SUBSTR(p_gender, 1, 1), SYSDATE, p_logged_in_user,
            p_aadhar_value, SUBSTR(p_exist_client_code, 1, 8),

            v_inv_branch_code, v_inv_address1, v_inv_address2, v_inv_fax, v_inv_pincode, v_inv_phone, 
            v_inv_phone1, v_inv_phone2, v_inv_status, v_inv_city_id, v_inv_rm_code
        )
        RETURNING inv_code INTO v_generated_inv_code;

        -- Get latest inv_code for that source
        SELECT MAX(inv_code)
        INTO nextinvkyc
        FROM investor_master
        WHERE source_id = SUBSTR(p_exist_client_code, 1, 8);

        -- Update KYC code and main code
        UPDATE client_test
        SET client_codekyc = nextinvkyc,
            main_code = (SELECT client_code FROM client_test WHERE client_codekyc = p_exist_client_code)
        WHERE client_code = v_generated_client_code;

        UPDATE investor_master
        SET inv_code = nextinvkyc
        WHERE inv_code = v_generated_inv_code;
       

        -- Final output
        OPEN p_result FOR
        SELECT 'Successfully member added' AS message FROM dual;

        COMMIT;

    ELSE
        OPEN p_result FOR
        SELECT 'Invalid client head' AS message FROM dual;
    END IF;
END PSM_AO_MM_Insert;
/
