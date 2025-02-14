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
    -- Validate the PAN, Mobile, and Email formats first
    ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(p_pan, 1, 10))));
    ISVALIDMOBILE := VALIDATE_MOBILE(p_mobile);
    ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(p_email));
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(p_gpan, 1, 10))));

    BEGIN -- Check for duplicate PAN globally
        SELECT count(PAN) 
        INTO VPAN 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(p_pan, 1, 10)));
        EXCEPTION WHEN NO_DATA_FOUND THEN VPAN := 0;
    END;


    BEGIN  -- validate mobile no of family sourcecode 
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE MOBILE = p_mobile 
        AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8));     
        EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
    END;

    BEGIN -- Check if the same email exists for a different family (SOURCE_CODE)
        SELECT COUNT(EMAIL) 
        INTO VEMAIL 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(EMAIL) = UPPER(p_email)
        AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8));     

        EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
    END;

    BEGIN -- Check if the same guardian pan exists for a different family (SOURCE_CODE)
        SELECT COUNT(G_PAN) 
        INTO VG_PAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(G_PAN) = UPPER(p_gpan)
        AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8)); 



        EXCEPTION WHEN NO_DATA_FOUND THEN VG_PAN := 0;
    END;

    BEGIN -- CHECK DUPLICATE AADHAR FLAG
        SELECT COUNT(aadhar_card_no) 
        INTO V_AADHAR 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE aadhar_card_no = TRIM(SUBSTR(p_aadhar_value,1,12));
        EXCEPTION WHEN NO_DATA_FOUND THEN V_AADHAR := 0;
    END;


    IF p_mobile IS NOT NULL THEN -- VALIDATE MOBILE AND RETURN INVALID, DUPLICATE
        IF ISVALIDMOBILE = 0 THEN
            OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)' AS message FROM DUAL;
            RETURN;    
        ELSIF ISVALIDMOBILE = 1  AND VMOBILE > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number' AS message FROM DUAL;
            RETURN;
        END IF;
    END IF;

    IF p_email IS NOT NULL THEN -- VALIDATE EMAIL AND RETURN INVALID, DUPLICATE
        IF p_email NOT IN ('NA', 'NOT AVAILABLE') THEN
            IF ISVALIDEMAIL = 0 THEN
                OPEN P_RESULT FOR SELECT 'Invalid Email' AS message FROM DUAL;
                RETURN;

            ELSIF ISVALIDEMAIL = 1  AND VEMAIL > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Email' AS message FROM DUAL;
            END IF;
        END IF;
    END IF ;

   IF p_pan IS NOT NULL THEN -- Validate PAN and return appropriate error messages
        IF ISVALIDPAN IN ('InValid', 'Miss') THEN
            OPEN P_RESULT FOR 
            SELECT 'Invalid PAN (e.g., AAAAA9999A)' AS message 
            FROM DUAL;
            RETURN;
        ELSIF ISVALIDPAN = 'Valid' AND VPAN > 0 THEN
            OPEN P_RESULT FOR 
            SELECT 'Duplicate PAN' AS message 
            FROM DUAL;
            RETURN;
        END IF;
    END IF;


    IF p_aadhar_value IS NOT NULL THEN -- VALIDATE AADHAR NUMBER 
        IF V_AADHAR > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Aadhar Number' AS message FROM DUAL;
            RETURN;
        END IF ;
    END IF;


    IF p_gpan IS NOT NULL THEN  -- VALIDATE GUARDIAN PAN, return an error message
        IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
            OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
            RETURN;

        ELSIF ISVALID_GPAN = 'Valid' and VG_PAN > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families)' AS message FROM DUAL;
            RETURN;
        END IF;
    end if;


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
            kyc_status,
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
            im.modify_date,
            im.modify_user,
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
            p_kyc,                                     -- insert the KYC status
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
              --  nationality = (SELECT nationality FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  resident_nri = (SELECT resident_nri FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
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
            SELECT 'Successfully member added: client code --> ' || v_generated_client_code || ' | Inv Code --> ' || nextinvkyc AS message
            FROM dual;
        COMMIT;

    ELSE 
        OPEN p_result FOR 
        SELECT 'Invalid client head' AS message
        FROM dual;
    end if ;
END PSM_AO_MM_Insert;