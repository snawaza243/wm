create or replace PROCEDURE PSM_AO_MM_Update_By_Inv(
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
  
        -- Declare local variables
    VPAN                    NUMBER;
    VPAN2                   NUMBER;
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
    ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_PAN, 1, 10))));
    ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE);
    ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(P_EMAIL));
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GPAN, 1, 10))));
    
    -- Check if the client_codekyc exists in the table
    BEGIN
        SELECT COUNT(client_codekyc) INTO ct_flag
        FROM client_test WHERE client_codekyc = P_UPDATEBYCLIENTCODE;
        EXCEPTION WHEN NO_DATA_FOUND THEN ct_flag := 0;
    END;


    BEGIN -- Check for duplicate PAN globally
        SELECT count(PAN)  INTO VPAN 
        FROM WEALTHMAKER.INVESTOR_MASTER WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_PAN, 1, 10)))
        AND INV_CODE != TRIM(P_UPDATEBYCLIENTCODE);
        EXCEPTION WHEN NO_DATA_FOUND THEN VPAN := 0;
    END;
    
    BEGIN -- Check for duplicate PAN globally
        SELECT count(PAN)  INTO VPAN2
        FROM WEALTHMAKER.INVESTOR_MASTER WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_GPAN, 1, 10)))
        AND INV_CODE != TRIM(P_EXIST_CLIENT_CODE);
        EXCEPTION WHEN NO_DATA_FOUND THEN VPAN2 := 0;
    END;

    BEGIN  -- validate mobile no of family sourcecode 
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE MOBILE = P_MOBILE 
        AND SOURCE_ID != TRIM(SUBSTR(P_UPDATEBYCLIENTCODE,1,8));  --AND INV_CODE != TRIM(P_UPDATEBYCLIENTCODE);
        EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
    END;

    
    BEGIN -- Check if the same email exists for a different family (SOURCE_CODE)
        SELECT COUNT(EMAIL) 
        INTO VEMAIL 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(EMAIL) = UPPER(P_EMAIL)
        AND SOURCE_ID != TRIM(SUBSTR(P_UPDATEBYCLIENTCODE,1,8));
        EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
    END;

    -- Check if the same guardian pan exists for a different family (SOURCE_CODE)
    BEGIN
        SELECT COUNT(G_PAN) 
        INTO VG_PAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(G_PAN) = UPPER(P_GPAN)
        AND SOURCE_ID != TRIM(SUBSTR(P_UPDATEBYCLIENTCODE,1,8))
        
        ;
        EXCEPTION WHEN NO_DATA_FOUND THEN VG_PAN := 0;
    END;


 
    
    -- CHECK DUPLICATE AADHAR FLAG
    BEGIN
        SELECT COUNT(aadhar_card_no) 
        INTO V_AADHAR 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE aadhar_card_no = TRIM(SUBSTR(P_AADHAR_VALUE,1,12))
        AND INV_CODE != TRIM(P_UPDATEBYCLIENTCODE);

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
                return;
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
        IF V_AADHAR > 0  and P_AADHAR_VALUE not in ('0')THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Aadhar Number' AS message FROM DUAL;
            RETURN;
        END IF ;
    END IF;
  
    
    IF p_gpan IS NOT NULL THEN  -- VALIDATE GUARDIAN PAN, return an error message
        IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
            OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
            RETURN;
        
        ELSIF ISVALID_GPAN = 'Valid' and VG_PAN > 0  OR VPAN2 > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families)' AS message FROM DUAL;
            RETURN;
        END IF;
    end if;
     
    IF P_UPDATEBYCLIENTCODE IS NOT NULL THEN
        -- Update the investor_master table with the provided input values
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
            im.kyc = P_KYC,                                     -- Update the KYC status
            im.APPROVED = P_APPROVE,                            -- Update the approval flag
            im.GENDER = P_GENDER,                               -- Update the gender
            im.MODIFY_DATE = SYSDATE,                           -- Set modify date to current system date
            im.MODIFY_USER = P_LOGGED_IN_USER,                  -- Set modify user to the user performing the update
            im.AADHAR_CARD_NO = P_AADHAR_VALUE
        WHERE im.inv_code = P_UPDATEBYCLIENTCODE;
        -- Commit the transaction
        COMMIT;

        IF ct_flag > 0 THEN
            -- Update the client_test table with the new details if record exists
            UPDATE CLIENT_TEST
            SET 
                CLIENTCODE_OLD = P_EXIST_CLIENT_CODE,         -- Update the old client code field
                BUSINESS_CODE = P_RM_BUSINESS_CODE,           -- Update the business code
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
                KYC_STATUS = P_KYC,                           -- Update the KYC status
                approved = P_APPROVE,                    -- Update the approval flag
                GENDER = P_GENDER,                            -- Update the gender
                MODIFY_DATE = SYSDATE,                        -- Set modify date to current system date
                MODIFY_USER = P_LOGGED_IN_USER,               -- Set modify user to the user performing the update
                is_nominee = P_NOM,
                nominee_per = P_ALLO,
                AADHAR_CARD_NO = P_AADHAR_VALUE
            WHERE client_codekyc = P_UPDATEBYCLIENTCODE;  -- Condition to identify the correct record

            select client_code into v_client_code from client_test where client_codekyc = P_UPDATEBYCLIENTCODE;
        ELSE
            -- Insert a new record into CLIENT_TEST if no record exists
            INSERT INTO CLIENT_TEST (
                CLIENT_CODE, CLIENTCODE_OLD, BUSINESS_CODE, TITLE, CLIENT_NAME, LOGGEDUSERID, MAIN_CODE,
                MOBILE_NO, EMAIL, CLIENT_PAN, DOB, RELATION_ID, G_NAME, G_PAN, OCC_ID, KYC_STATUS, APPROVED, GENDER, AADHAR_CARD_NO,
                TIMEST, is_nominee,  nominee_per
            )
            VALUES (
                NULL,  -- Assuming CLIENT_CODE is auto-generated
                P_EXIST_CLIENT_CODE, P_RM_BUSINESS_CODE, P_SALUTATION1, P_ACCOUNT_NAME, P_LOGGED_IN_USER, P_CLIENT_CODE_IN_MAIN,
                P_MOBILE, P_EMAIL, P_PAN, P_DOB, P_RELATION, P_GNAME, P_GPAN, P_OCCUPATION, P_KYC, P_APPROVE, P_GENDER, P_AADHAR_VALUE,
                SYSDATE,  p_nom, p_allo
            ) RETURN client_code into v_client_code ;
            COMMIT;
            
                -- Update client_codekyc in CLIENT_TEST using the highest inv_code from investor_master
            UPDATE client_test
            SET client_codekyc = P_UPDATEBYCLIENTCODE
            WHERE client_code = v_client_code;

        END IF;

        
        COMMIT;
        OPEN p_result FOR 
            SELECT 'Successfully Updated: ' || client_name || ' (' || v_client_code || ' / ' || (SELECT inv_code FROM investor_master WHERE inv_code = P_UPDATEBYCLIENTCODE)|| ')' AS message
            FROM client_test WHERE client_code = v_client_code;
        COMMIT;
    ELSE
                OPEN p_result FOR 
            SELECT 'Invalid member ID' AS message
            FROM client_test WHERE client_code = v_client_code;
    end if;
 
END PSM_AO_MM_Update_By_Inv;