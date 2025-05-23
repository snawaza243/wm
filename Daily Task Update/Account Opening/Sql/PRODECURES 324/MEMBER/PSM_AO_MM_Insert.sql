create or replace PROCEDURE PSM_AO_MM_Insert (
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
    p_dob                 DATE,
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
    p_result              OUT SYS_REFCURSOR  -- Cursor to return results
) AS
    v_generated_client_code client_test.client_code%TYPE;
    v_generated_inv_code investor_master.inv_code%type;
    v_aadhar_number         BOOLEAN;
    v_count                 NUMBER;
    nextinvkyc              NUMBER;
    v_pan_flag              NUMBER;

    -- Declare local variables
    VPAN                    NUMBER;
    VPAN2                    NUMBER;

    VMOBILE                 NUMBER;
    VEMAIL                  NUMBER;
    VG_PAN                  NUMBER;
    ISVALIDPAN              VARCHAR2(10);
    ISVALID_GPAN            VARCHAR2(10);

    ISVALIDMOBILE           NUMBER;
    ISVALIDEMAIL            NUMBER;
    v_message               VARCHAR2(4000); -- To hold the result message
    V_SOURCE_CODE           VARCHAR2(20); -- To store the SOURCE_CODE of the current client
    ISVALIDAADAHR           NUMBER;
    V_AADHAR                NUMBER;
BEGIN
    IF p_exist_client_code IS NOT NULL THEN 
        INSERT INTO client_test (
            client_code,
            clientcode_old,
            business_code,
            title,
            client_name,
            loggeduserid,
            main_code,
            mobile_no,
            email,
            client_pan,
            dob,
            relation_id,
            g_name,
            g_pan,
            occ_id,
            INV_KYC,
            approved,
            gender,
            aadhar_card_no,
            timest,
            is_nominee, 
            nominee_per
        ) VALUES (
            NULL,  -- Assuming CLIENT_CODE is auto-generated
            p_exist_client_code,
            p_rm_business_code,
            p_salutation1,
            p_account_name,
            p_logged_in_user,
            p_client_code_in_main,
            p_mobile,
            p_email,
            p_pan,
            p_dob,
            p_relation,
            p_gname,
            p_gpan,
            p_occupation,
            p_kyc,
            p_approve,
            p_gender,
            p_aadhar_value,
            sysdate,
            p_nom, 
            p_allo
        ) RETURN client_code INTO v_generated_client_code;

        COMMIT;
        INSERT INTO investor_master im (
            im.inv_code,
            im.investor_title,
            im.investor_name,
            im.mobile,
            im.email,
            im.pan,
            im.dob,
            im.rel_id,
            im.g_name,
            im.g_pan,
            im.occupation_id,
            im.kyc,
            im.approved,
            im.gender,
            im.timest,
            im.loggeduserid,
            im.aadhar_card_no,
            im.source_id
        ) VALUES (
            NULL,
            p_salutation1,                  -- insert the title/salutation
            p_account_name,                  -- insert the client name
            p_mobile,                               -- insert the mobile number
            p_email,                                 -- insert the email
            p_pan,                                     -- insert the client PAN
            p_dob,                                     -- insert the date of birth
            p_relation,                             -- insert the relation ID
            p_gname,                                -- insert the guardian name
            p_gpan,                                  -- insert the guardian PAN
            p_occupation,                    -- insert the occupation ID
            'YES',                                     -- insert the KYC status
            p_approve,                            -- insert the approval flag
            substr(p_gender,1,1),                               -- insert the gender
            sysdate,                           -- Set modify date to current system date
            p_logged_in_user,                  -- Set modify user to the user performing the insert
            p_aadhar_value,
            substr(p_exist_client_code, 1,8)

        ) RETURNING inv_code INTO v_generated_inv_code;

        COMMIT;

        SELECT MAX(inv_code) 
        INTO nextinvkyc
        FROM investor_master WHERE source_id = substr(p_exist_client_code, 1, 8);

        UPDATE client_test 
        SET client_codekyc = nextinvkyc
        WHERE client_code = v_generated_client_code;

        UPDATE investor_master
        SET inv_code = nextinvkyc
        WHERE inv_code = v_generated_inv_code;


        -- UPDATE MEMEBR LEFT DATA WITH HEAD DATA
        UPDATE CLIENT_TEST
            SET
               --/ clientcode_old = (SELECT clientcode_old FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                business_code = (SELECT business_code FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- occ_id = (SELECT occ_id FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- status = (SELECT status FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- act_cat = (SELECT act_cat FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- title = (SELECT title FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- client_name = (SELECT client_name FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- title_other = (SELECT title_other FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- other = (SELECT other FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- others1 = (SELECT others1 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  gender = (SELECT gender FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  marital_status = (SELECT marital_status FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                nationality = (SELECT nationality FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                resident_nri = (SELECT resident_nri FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  dob = (SELECT dob FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  annual_income = (SELECT annual_income FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  client_pan = (SELECT client_pan FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                lead_type = (SELECT lead_type FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  g_name = (SELECT g_name FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                g_nationality = (SELECT g_nationality FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  g_pan = (SELECT g_pan FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                add1 = (SELECT add1 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                add2 = (SELECT add2 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                state_id = (SELECT state_id FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                city_id = (SELECT city_id FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                pincode = (SELECT pincode FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                --overseas_add = (SELECT overseas_add FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
               -- fax = (SELECT fax FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  aadhar_card_no = (SELECT aadhar_card_no FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  email = (SELECT email FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                std1 = (SELECT std1 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                tel1 = (SELECT tel1 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                std2 = (SELECT std2 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                tel2 = (SELECT tel2 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   mobile_no = (SELECT mobile_no FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   loggeduserid = (SELECT loggeduserid FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   timest = (SELECT timest FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   client_codekyc = (SELECT client_codekyc FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   source_code = (SELECT source_code FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  main_code = (SELECT main_code FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                BRANCH_CODE = (SELECT BRANCH_CODE FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main)        
        WHERE CLIENT_CODE = v_generated_client_code;
        COMMIT;

        UPDATE INVESTOR_MASTER
            SET
            branch_code = (SELECT branch_code FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            address1 = (SELECT address1 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            address2 = (SELECT address2 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            fax = (SELECT fax FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            pincode = (SELECT pincode FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            phone = (SELECT phone1 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            phone1 = (SELECT phone1 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            phone2 = (SELECT phone2 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            status = (SELECT status FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            city_id = (SELECT city_id FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            source_id = (SELECT source_id FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            rm_code = (SELECT rm_code FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code)
                WHERE INV_CODE = nextinvkyc;
        COMMIT;

        OPEN p_result FOR 
            SELECT 'Successfully member added ' /* || v_generated_client_code || ' | Inv Code --> ' || nextinvkyc*/ AS message
            FROM dual;
        COMMIT;

    ELSE 
        OPEN p_result FOR 
        SELECT 'Invalid client head' AS message
        FROM dual;
    end if ;
END PSM_AO_MM_Insert;