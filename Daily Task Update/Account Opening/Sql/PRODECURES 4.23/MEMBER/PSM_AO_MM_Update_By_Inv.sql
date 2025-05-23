CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_MM_Update_By_Inv( 
    P_DT_NUMBER                IN VARCHAR2,  -- Input parameter for some detail number
    P_EXIST_CLIENT_CODE        IN NUMBER,    -- Existing client code (used for updating CLIENTCODE_OLD)
    P_RM_BUSINESS_CODE         IN VARCHAR2,  -- Business code related to the RM
    P_SALUTATION1              IN VARCHAR2,  -- Salutation for the client (e.g., Mr., Ms.)
    P_ACCOUNT_NAME             IN VARCHAR2,  -- Name of the account holder
    P_LOGGED_IN_USER           IN VARCHAR2,  -- User performing the update
    P_CLIENT_CODE_IN_MAIN      IN VARCHAR2,  -- Main client code
    P_MOBILE                   IN VARCHAR2,  -- Mobile number of the client
    P_PAN                      IN VARCHAR2,  -- PAN (Permanent Account Number) of the client
    P_EMAIL                    IN VARCHAR2,  -- Email of the client
    P_DOB                      IN DATE,      -- Date of Birth of the client
    P_RELATION                 IN NUMBER,    -- Relation ID
    P_GNAME                    IN VARCHAR2,  -- Guardian name
    P_GPAN                     IN VARCHAR2,  -- Guardian PAN
    P_OCCUPATION               IN NUMBER,    -- Occupation ID
    P_KYC                      IN VARCHAR2,  -- KYC status
    P_APPROVE                  IN VARCHAR2,  -- Approval status
    P_GENDER                   IN VARCHAR2,  -- Gender of the client
    P_NOM                      IN VARCHAR2,  -- Nominee details
    P_ALLO                     IN NUMBER,    -- Allocation percentage or amount
    P_UPDATEBYCLIENTCODE       IN VARCHAR2,  -- Client code to identify the record to be updated
    P_AADHAR_VALUE             IN VARCHAR2,
    P_RESULT                   OUT SYS_REFCURSOR  -- Cursor to return results
    
) AS
    v_updated_client_name CLIENT_TEST.CLIENT_NAME%TYPE;  -- Variable to hold the updated client name
    v_client_code CLIENT_TEST.client_code%TYPE;  -- Variable to hold the updated client name

    ct_flag                 NUMBER;
BEGIN

     BEGIN
        SELECT COUNT(client_codekyc) INTO ct_flag
        FROM client_test WHERE client_codekyc = P_UPDATEBYCLIENTCODE;
        EXCEPTION WHEN NO_DATA_FOUND THEN ct_flag := 0;
    END;

    IF P_UPDATEBYCLIENTCODE IS NOT NULL THEN
        UPDATE investor_master im
        SET 
            im.investor_title = P_SALUTATION1,                  -- Update the title/salutation
            im.investor_name = P_ACCOUNT_NAME,                  -- Update the client name
            im.mobile = P_MOBILE,                               -- Update the mobile number
            im.EMAIL = P_EMAIL,                                 -- Update the email
            im.pan = P_PAN,                                     -- Update the client PAN
            im.DOB = P_DOB,                                     -- Update the date of birth
            im.REL_ID = P_RELATION,                             -- Update the relation ID
            im.G_NAME = P_GNAME,                                -- Update the guardian name
            im.G_PAN = P_GPAN,                                  -- Update the guardian PAN
            im.OCCUPATION_ID = P_OCCUPATION,                    -- Update the occupation ID
            im.APPROVED = P_APPROVE,                            -- Update the approval flag
            im.GENDER = P_GENDER,                               -- Update the gender
            im.MODIFY_DATE = SYSDATE,                           -- Set modify date to current system date
            im.MODIFY_USER = P_LOGGED_IN_USER,                  -- Set modify user to the user performing the update
            im.AADHAR_CARD_NO = P_AADHAR_VALUE,


            branch_code = (SELECT branch_code FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            address1 = (SELECT address1 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            address2 = (SELECT address2 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            fax = (SELECT fax FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            pincode = (SELECT pincode FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            phone = (SELECT phone1 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            phone1 = (SELECT phone1 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            phone2 = (SELECT phone2 FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            status = (SELECT status FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            city_id = (SELECT city_id FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code)
            --source_id = (SELECT source_id FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code),
            --rm_code = (SELECT rm_code FROM INVESTOR_MASTER WHERE INV_CODE = p_exist_client_code)
        WHERE im.inv_code = P_UPDATEBYCLIENTCODE;
        -- Commit the transaction
        COMMIT;

        IF ct_flag > 0 THEN
            -- Update the client_test table with the new details if record exists
            UPDATE CLIENT_TEST
            SET 
                CLIENTCODE_OLD = P_EXIST_CLIENT_CODE,         -- Update the old client code field
               -- BUSINESS_CODE = P_RM_BUSINESS_CODE,           -- Update the business code
                TITLE = P_SALUTATION1,                        -- Update the title/salutation
                CLIENT_NAME = P_ACCOUNT_NAME,                 -- Update the client name
                MOBILE_NO = P_MOBILE,                         -- Update the mobile number
                EMAIL = P_EMAIL,                              -- Update the email
                CLIENT_PAN = P_PAN,                           -- Update the client PAN
                DOB = P_DOB,                                  -- Update the date of birth
                RELATION_ID = P_RELATION,                     -- Update the relation ID
                G_NAME = P_GNAME,                             -- Update the guardian name
                G_PAN = P_GPAN,                               -- Update the guardian PAN
                OCC_ID = P_OCCUPATION,                        -- Update the occupation ID
                KYC_STATUS = 'Y',                           -- Update the KYC status
                INV_KYC = P_KYC,
                approved = P_APPROVE,                    -- Update the approval flag
                GENDER = P_GENDER,                            -- Update the gender
                MODIFY_DATE = SYSDATE,                        -- Set modify date to current system date
                MODIFY_USER = P_LOGGED_IN_USER,               -- Set modify user to the user performing the update
                is_nominee = P_NOM,
                nominee_per = P_ALLO,
                AADHAR_CARD_NO = P_AADHAR_VALUE,

                -- head data
                  --/ clientcode_old = (SELECT clientcode_old FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
                --business_code = (SELECT business_code FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
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
                tel2 = (SELECT tel2 FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main)
             --   mobile_no = (SELECT mobile_no FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   loggeduserid = (SELECT loggeduserid FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   timest = (SELECT timest FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   client_codekyc = (SELECT client_codekyc FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
             --   source_code = (SELECT source_code FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
              --  main_code = (SELECT main_code FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main),
            --BRANCH_CODE = (SELECT BRANCH_CODE FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code_in_main)  

            WHERE client_codekyc = P_UPDATEBYCLIENTCODE;  -- Condition to identify the correct record

            select client_code into v_client_code from client_test where client_codekyc = P_UPDATEBYCLIENTCODE;
        ELSE
            -- Insert a new record into CLIENT_TEST if no record exists
            INSERT INTO CLIENT_TEST (
                CLIENT_CODE, CLIENTCODE_OLD, BUSINESS_CODE, TITLE, CLIENT_NAME, LOGGEDUSERID, MAIN_CODE,
                MOBILE_NO, EMAIL, CLIENT_PAN, DOB, RELATION_ID, G_NAME, G_PAN, OCC_ID, KYC_STATUS, INV_KYC,  APPROVED, GENDER, AADHAR_CARD_NO,
                TIMEST, is_nominee,  nominee_per
            )
            VALUES (
                NULL,  -- Assuming CLIENT_CODE is auto-generated
                P_EXIST_CLIENT_CODE, P_RM_BUSINESS_CODE, P_SALUTATION1, P_ACCOUNT_NAME, P_LOGGED_IN_USER, P_CLIENT_CODE_IN_MAIN,
                P_MOBILE, P_EMAIL, P_PAN, P_DOB, P_RELATION, P_GNAME, P_GPAN, P_OCCUPATION, 'Y' , P_KYC, P_APPROVE, P_GENDER, P_AADHAR_VALUE,
                SYSDATE,  p_nom, p_allo
            ) RETURN client_code into v_client_code ;
            COMMIT;

                -- Update client_codekyc in CLIENT_TEST using the highest inv_code from investor_master
            UPDATE client_test
            SET client_codekyc = P_UPDATEBYCLIENTCODE
            WHERE client_code = v_client_code;


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
        WHERE CLIENT_CODE = v_client_code;





        END IF;


        COMMIT;
        OPEN p_result FOR 
            SELECT 'Member Updated Successfully' /* || client_name || ' (' || v_client_code || ' / ' || (SELECT inv_code FROM investor_master WHERE inv_code = P_UPDATEBYCLIENTCODE)|| ')' */ AS message
            FROM client_test WHERE client_code = v_client_code;
        COMMIT;
    ELSE
                OPEN p_result FOR 
            SELECT 'Invalid member ID' AS message
            FROM client_test WHERE client_code = v_client_code;
    end if;

END PSM_AO_MM_Update_By_Inv;
/
