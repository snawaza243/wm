create or replace PROCEDURE PSM_AO_UPDATE_MEMBERBYCLIENTCODE(
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


    ct_flag INT;

BEGIN
    -- Check if the client_codekyc exists in the table
    SELECT NVL(COUNT(*), 0) INTO ct_flag
    FROM client_test
    WHERE client_codekyc = P_UPDATEBYCLIENTCODE;

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
            APPROVED_FLAG = P_APPROVE,                    -- Update the approval flag
            GENDER = P_GENDER,                            -- Update the gender
            MODIFY_DATE = SYSDATE,                        -- Set modify date to current system date
            MODIFY_USER = P_LOGGED_IN_USER,               -- Set modify user to the user performing the update
            AADHAR_CARD_NO = P_AADHAR_VALUE
        WHERE client_codekyc = P_UPDATEBYCLIENTCODE;  -- Condition to identify the correct record
    ELSE
        -- Insert a new record into CLIENT_TEST if no record exists
        INSERT INTO CLIENT_TEST (
            CLIENT_CODE, CLIENTCODE_OLD, BUSINESS_CODE, TITLE, CLIENT_NAME, LOGGEDUSERID, MAIN_CODE,
            MOBILE_NO, EMAIL, CLIENT_PAN, DOB, RELATION_ID, G_NAME, G_PAN, OCC_ID, KYC_STATUS, APPROVED_FLAG, GENDER, AADHAR_CARD_NO,
            TIMEST
        )
        VALUES (
            NULL,  -- Assuming CLIENT_CODE is auto-generated
            P_EXIST_CLIENT_CODE, P_RM_BUSINESS_CODE, P_SALUTATION1, P_ACCOUNT_NAME, P_LOGGED_IN_USER, P_CLIENT_CODE_IN_MAIN,
            P_MOBILE, P_EMAIL, P_PAN, P_DOB, P_RELATION, P_GNAME, P_GPAN, P_OCCUPATION, P_KYC, P_APPROVE, P_GENDER, P_AADHAR_VALUE,
            SYSDATE
        ) RETURN client_code into v_client_code ;
        COMMIT;
    END IF;

    -- Update client_codekyc in CLIENT_TEST using the highest inv_code from investor_master
    UPDATE client_test
    SET client_codekyc = P_UPDATEBYCLIENTCODE
    WHERE client_code = v_client_code;


    COMMIT;

     OPEN P_RESULT FOR
    SELECT v_client_code, CLIENT_NAME
    FROM CLIENT_TEST
    WHERE CLIENT_CODE = v_client_code;  -- Return the record with the updated client code

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        -- Handle the case where no record was found for the given client code
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20001, 'No record found with the provided client code.');
    WHEN OTHERS THEN
        -- Handle any other exceptions
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20002, 'An error occurred during updating member.');
END PSM_AO_UPDATE_MEMBERBYCLIENTCODE;