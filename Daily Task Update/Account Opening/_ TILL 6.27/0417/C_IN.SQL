create or replace PROCEDURE PSM_AO_INSERT_CLIENT_DATA(
    P_DT_NUMBER                     IN VARCHAR2,
    P_EXIST_CLIENT_CODE             IN NUMBER,
    P_RM_BUSINESS_CODE              IN VARCHAR2,
    P_TAX_STATUS                    IN VARCHAR2,
    P_OCCUPATION                    IN NUMBER,
    P_STATUS_CAT                    IN VARCHAR2,
    P_CLIENT_CAT                    IN VARCHAR2,
    P_ACCOUNT_CAT                   IN NUMBER,
    P_SALUTATION1                   IN VARCHAR2,
    P_ACCOUNT_NAME                  IN VARCHAR2,
    P_SALUTATION2                   IN VARCHAR2,
    P_FATHER_NAME                   IN VARCHAR2,
    P_ACCOUNT_OTHER                 IN VARCHAR2,
    P_GENDER                        IN VARCHAR2,
    P_MARITAL_STATUS                IN VARCHAR2,
    P_NATIONALITY                   IN VARCHAR2,
    P_RESIDENT_NRI                  IN VARCHAR2,
    P_DOB                           IN DATE,
    P_ANNUAL_INCOME                 IN VARCHAR2,
    P_CLIENT_PAN                    IN VARCHAR2,
    P_LEAD_TYPE                     IN VARCHAR2,
    P_GUARDIAN_PERSON               IN VARCHAR2,
    P_GUARDIAN_PERSON_NATIONALITY   IN VARCHAR2,
    P_GUARDIAN_PERSON_PAN           IN VARCHAR2,
    P_MAILING_ADDRESS1              IN VARCHAR2,
    P_MAILING_ADDRESS2              IN VARCHAR2,
    P_MAILING_STATE                 IN VARCHAR2,
    P_MAILING_CITY                  IN VARCHAR2,
    P_MAILING_PINCODE               IN VARCHAR2,
    P_PERMANENT_ADDRESS1            IN VARCHAR2,
    P_PERMANENT_ADDRESS2            IN VARCHAR2,
    P_PERMANENT_STATE               IN VARCHAR2,
    P_PERMANENT_CITY                IN VARCHAR2,
    P_PERMANENT_PINCODE             IN VARCHAR2,
    P_NRI_ADDRESS                   IN VARCHAR2,
    P_FAX_VALUE                     IN VARCHAR2,
    P_AADHAR_VALUE                  IN NUMBER,
    P_EMAIL_VALUE                   IN VARCHAR2,
    P_EMAIL_OFFICIAL_VALUE          IN VARCHAR2,
    P_PHONE_OFFICE_STD_VALUE        IN VARCHAR2,
    P_PHONE_OFFICE_NUMBER_VALUE     IN VARCHAR2,
    P_PHONE_RES_STD_VALUE           IN VARCHAR2,
    P_PHONE_RES_NUMBER_VALUE        IN VARCHAR2,
    P_MOBILE_NO_VALUE               IN VARCHAR2,
    P_REFERENCE_NAME1_VALUE         IN VARCHAR2,
    P_REFERENCE_NAME2_VALUE         IN VARCHAR2,
    P_REFERENCE_NAME3_VALUE         IN VARCHAR2,
    P_REFERENCE_NAME4_VALUE         IN VARCHAR2,
    P_MOBILE_NO1_VALUE              IN NUMBER,
    P_MOBILE_NO2_VALUE              IN NUMBER,
    P_MOBILE_NO3_VALUE              IN NUMBER,
    P_MOBILE_NO4_VALUE              IN NUMBER,
    P_LOGGED_IN_USER                IN VARCHAR2,
    P_GUEST_CODE                    IN VARCHAR2,
    P_EMP_RM                        IN NUMBER,
    P_EMP_SRC                       IN NUMBER,
    P_RESULT                        OUT SYS_REFCURSOR 
) AS

    DCT_FLAG                    NUMBER := 0; -- Duplicate account in client_test
    DCM_FLAG                    NUMBER := 0;
    DT_FLAG                     NUMBER;
    MYCLIENTCODE                VARCHAR2(50);  
    MYMAINCLIENTCODE            VARCHAR2(50);  
    MYSOURCEID                  VARCHAR2(8);  
    V_UNIQUE_CLIENT_CODE        VARCHAR2(50);
    V_CHECK_DUPLICATE           NUMBER;
    SQL_QUERY                   VARCHAR2(4000);

    V_GENERATED_CLIENT_CODE     CLIENT_TEST.CLIENT_CODE%TYPE;
    V_INV_CODE                  INVESTOR_MASTER.INV_CODE%TYPE;
    V_MYSOURCEID                CLIENT_MASTER.CLIENT_CODE%TYPE;

    VCNT                        NUMBER; -- for inv pan validation
    CLIENT_MASTER_FLAG          NUMBER; -- Flag for CLIENT_MASTER validation
    CLIENT_TEST_FLAG            NUMBER; -- Flag for CLIENT_TEST validation

    -- DT AND GUEST CODE VALIDATION LCOAL VARIABLES
    GUEST_CD_EXIST_FLAG         NUMBER:=0;
    DUP_GUEST_FLAG              NUMBER:=0;




    -- Declare local variables
    VPAN                    NUMBER;
    VMOBILE                 NUMBER;
    VEMAIL                  NUMBER;
    VG_PAN                  NUMBER;
    ISVALIDPAN              VARCHAR2(10);
    ISVALID_GPAN            VARCHAR2(10);
    ISVALID_AADHAAR         VARCHAR2(10);

    ISVALIDMOBILE           NUMBER;
    ISVALIDEMAIL            NUMBER;
    V_MESSAGE               VARCHAR2(4000); -- To hold the result message
    V_SOURCE_CODE           VARCHAR2(20); -- To store the SOURCE_CODE of the current client
    ISVALIDAADAHR           NUMBER;
    V_AADHAR                NUMBER;

    V_P_STATUS_CAT           NUMBER;
BEGIN


IF P_STATUS_CAT IN ('1', '2', '3', '4', '5', '6') THEN
        V_P_STATUS_CAT := TO_NUMBER(P_STATUS_CAT);
    ELSE
        -- Map text values to their corresponding codes
        SELECT CASE 
                   WHEN P_STATUS_CAT = 'Individual' THEN 1
                   WHEN P_STATUS_CAT = 'HUF' THEN 2
                   WHEN P_STATUS_CAT = 'Company' THEN 3
                   WHEN P_STATUS_CAT = 'Trust' THEN 4
                   WHEN P_STATUS_CAT = 'Society' THEN 5
                   WHEN P_STATUS_CAT = 'Bajaj Capital Staff' THEN 6
                   ELSE NULL  -- Handle invalid values
               END
        INTO V_P_STATUS_CAT
        FROM DUAL;
END IF;


-- Validate CLIENT_MASTER, CLIENT_TEST
BEGIN
    BEGIN
        SELECT NVL(COUNT(*),0)
        INTO CLIENT_MASTER_FLAG
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = P_DT_NUMBER AND
        SUBSTR(INV_CODE, 1, 8) IN (SELECT CLIENT_CODE FROM CLIENT_MASTER);
        EXCEPTION WHEN NO_DATA_FOUND THEN CLIENT_MASTER_FLAG:=0;
    END;

    BEGIN
        SELECT NVL(COUNT(*),0)
        INTO CLIENT_TEST_FLAG
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = P_DT_NUMBER AND
        INV_CODE IN (SELECT CLIENT_CODEKYC FROM CLIENT_TEST);
        EXCEPTION WHEN NO_DATA_FOUND THEN CLIENT_TEST_FLAG:=0;
    END;
END;

IF LENGTH(TRIM(P_EXIST_CLIENT_CODE)) >= 11 THEN

    -- Check for Duplicate Account (DCT_FLAG)
    SELECT COUNT(CLIENT_CODEKYC) INTO DCT_FLAG
    FROM CLIENT_TEST
    WHERE CLIENT_CODEKYC = P_EXIST_CLIENT_CODE;

    -- If Account Exists, Return Message
    IF DCT_FLAG > 0 THEN
        OPEN P_RESULT FOR
        SELECT 'Account Of This Main Investor Is Already Exist' AS MESSAGE FROM DUAL;
        RETURN;

    ELSE   

        INSERT INTO CLIENT_TEST (
            CLIENT_CODE,
            CLIENTCODE_OLD,
            BUSINESS_CODE,
            KYC_STATUS,
            OCC_ID,
            STATUS,
            ACT_CAT,
            TITLE,
            CLIENT_NAME,
            TITLE_OTHER,
            OTHER,
            OTHERS1,
            GENDER,
            MARITAL_STATUS,
            NATIONALITY,
            RESIDENT_NRI,
            DOB,
            ANNUAL_INCOME,
            CLIENT_PAN,
            LEAD_TYPE,
            G_NAME,
            G_NATIONALITY,
            G_PAN,
            ADD1,
            ADD2,
            STATE_ID,
            CITY_ID,
            PINCODE,
            OVERSEAS_ADD,
            FAX,
            AADHAR_CARD_NO,
            EMAIL,
            STD1,
            TEL1,
            STD2,
            TEL2,
            MOBILE_NO,
            LOGGEDUSERID,
            TIMEST,
            CLIENT_CODEKYC,
            SOURCE_CODE,
            MAIN_CODE,
            BRANCH_CODE
        ) VALUES (
            NULL, -- Assuming CLIENT_CODE is auto-generated
            P_EXIST_CLIENT_CODE,
            P_RM_BUSINESS_CODE,
            NULL,
            P_OCCUPATION,
            P_STATUS_CAT,
            P_ACCOUNT_CAT,
            P_SALUTATION1,
            P_ACCOUNT_NAME,
            P_SALUTATION2,
            P_FATHER_NAME,
            P_ACCOUNT_OTHER,
            P_GENDER,
            P_MARITAL_STATUS,
            P_NATIONALITY,
            P_RESIDENT_NRI,
            P_DOB,
            P_ANNUAL_INCOME,
            P_CLIENT_PAN,
            P_LEAD_TYPE,
            P_GUARDIAN_PERSON,
            P_GUARDIAN_PERSON_NATIONALITY,
            P_GUARDIAN_PERSON_PAN,
            P_MAILING_ADDRESS1,
            P_MAILING_ADDRESS2,
            P_MAILING_STATE,
            P_MAILING_CITY,
            P_MAILING_PINCODE,
            P_NRI_ADDRESS,
            P_FAX_VALUE,
            P_AADHAR_VALUE,
            P_EMAIL_VALUE,
            P_PHONE_OFFICE_STD_VALUE,
            P_PHONE_OFFICE_NUMBER_VALUE,
            P_PHONE_RES_STD_VALUE,
            P_PHONE_RES_NUMBER_VALUE,
            P_MOBILE_NO_VALUE,
            P_LOGGED_IN_USER,
            SYSDATE,
            P_EXIST_CLIENT_CODE, --(SELECT concat(client_code, '001') FROM client_master WHERE pan = p_client_pan AND ROWNUM = 1 ),
            NULL,
            NULL,
            P_EMP_SRC
        ) RETURNING CLIENT_CODE INTO V_GENERATED_CLIENT_CODE;
        COMMIT; 

        UPDATE CLIENT_MASTER CM
        SET 
            CM.PER_STATE_ID         = P_PERMANENT_STATE,
            CM.PER_PINCODE          = P_PERMANENT_PINCODE, 
            CM.OFFICE_EMAIL         = P_EMAIL_OFFICIAL_VALUE,  
            CM.PER_ADD1             = P_PERMANENT_ADDRESS1,  
            CM.PER_ADD2             = P_PERMANENT_ADDRESS2,  
            CM.REF_NAME1            = P_REFERENCE_NAME1_VALUE,  
            CM.REF_NAME2            = P_REFERENCE_NAME2_VALUE,  
            CM.REF_NAME3            = P_REFERENCE_NAME3_VALUE,  
            CM.REF_NAME4            = P_REFERENCE_NAME4_VALUE,  
            CM.REF_MOB1             = P_MOBILE_NO1_VALUE,  
            CM.REF_MOB2             = P_MOBILE_NO2_VALUE,  
            CM.REF_MOB3             = P_MOBILE_NO3_VALUE,  
            CM.REF_MOB4             = P_MOBILE_NO4_VALUE,  
            CM.CLIENT_TITLE         = P_SALUTATION1,  
            CM.TITLE_FATHER_SPOUSE  = P_SALUTATION2,  
            CM.CATEGORY_ID          = P_CLIENT_CAT,  
            CM.INVESTOR_CODE        = P_STATUS_CAT,
            CM.GUEST_CD             = P_GUEST_CODE,  
            CM.CLIENT_NAME          = P_ACCOUNT_NAME,  
            CM.FATHER_SPOUSE_NAME   = P_FATHER_NAME,  
            CM.ADDRESS1             = P_MAILING_ADDRESS1,  
            CM.ADDRESS2             = P_MAILING_ADDRESS2,  
            CM.FAX                  = P_FAX_VALUE,  
            CM.EMAIL                = P_EMAIL_VALUE,  
            CM.PAN                  = P_CLIENT_PAN,  
            CM.DOB                  = P_DOB,  
            CM.MOBILE               = P_MOBILE_NO_VALUE,  
            CM.PINCODE              = P_MAILING_PINCODE,  
            CM.GENDER               = NVL (SUBSTR(P_GENDER, 1, 1), NULL),  
            CM.MARITAL_STATUS       = P_MARITAL_STATUS,  
            CM.LEAD_SOURCE          = P_LEAD_TYPE,  
            CM.CITY_NAME            = NVL ((SELECT CITY_NAME FROM CITY_MASTER WHERE CITY_ID = P_MAILING_CITY ), NULL ),  
            CM.STD1                 = NVL (SUBSTR(P_PHONE_OFFICE_STD_VALUE, 1, 5), NULL),  
            CM.STD2                 = NVL (SUBSTR(P_PHONE_RES_STD_VALUE, 1, 5), NULL),  
            CM.PHONE1               = P_PHONE_OFFICE_NUMBER_VALUE,  
            CM.PHONE2               = P_PHONE_RES_NUMBER_VALUE,  
            CM.STATUS               = P_TAX_STATUS,  
            CM.OCC_ID               = P_OCCUPATION,   
            CM.CITY_ID              = P_MAILING_CITY,  
            CM.PER_CITY_ID          = P_PERMANENT_CITY,  
            CM.SOURCEID             = P_EMP_SRC,  
            CM.RM_CODE              = P_EMP_RM,  
            CM.MODIFY_DATE          = SYSDATE,
            CM.MODIFY_USER          = P_LOGGED_IN_USER
        WHERE CM.CLIENT_CODE = SUBSTR(P_EXIST_CLIENT_CODE, 1, 8);

        UPDATE INVESTOR_MASTER IM
        SET
            IM.INVESTOR_TITLE       = P_SALUTATION1,                             
            IM.BRANCH_CODE          = P_EMP_SRC,                       
            IM.INVESTOR_NAME        = P_ACCOUNT_NAME,
            IM.G_NAME               = P_GUARDIAN_PERSON,
            IM.G_PAN                = P_GUARDIAN_PERSON_PAN,
            IM.MAR_STATUS           = P_MARITAL_STATUS,
            IM.ADDRESS1             = P_MAILING_ADDRESS1,                    
            IM.ADDRESS2             = P_MAILING_ADDRESS2,                    
            IM.FAX                  = P_FAX_VALUE,                  
            IM.EMAIL                = P_EMAIL_VALUE,                   
            IM.PAN                  = P_CLIENT_PAN,              
            IM.DOB                  = P_DOB,                  
            IM.MOBILE               = P_MOBILE_NO_VALUE,                 
            IM.AADHAR_CARD_NO       = P_AADHAR_VALUE,                   
            IM.PINCODE              = P_MAILING_PINCODE,                
            IM.GENDER               = NVL (SUBSTR(P_GENDER, 1, 1), NULL),                 
            IM.PHONE1               = P_PHONE_OFFICE_NUMBER_VALUE,                 
            IM.PHONE2               = P_PHONE_RES_NUMBER_VALUE,                 
            IM.STATUS               = P_TAX_STATUS,                
            IM.CITY_ID              = P_MAILING_CITY,                
            IM.RM_CODE              = P_EMP_RM,                             
            --IM.rel_id          =     null,                                
            IM.OCCUPATION_ID        = P_OCCUPATION, 
            IM.CATEGORY_ID          = P_CLIENT_CAT, 
            IM.MODIFY_DATE          = SYSDATE,
            IM.MODIFY_USER          = P_LOGGED_IN_USER
        WHERE INV_CODE = P_EXIST_CLIENT_CODE;
        COMMIT;
    
        UPDATE  CLIENT_TEST 
        SET     MAIN_CODE= V_GENERATED_CLIENT_CODE
        WHERE   CLIENT_CODE = V_GENERATED_CLIENT_CODE;
        
        -- UPDATE TB_COD_UPLAOD INV_CODE WITH INVESTOR CODE OF ADDED HEAD INVESTOR 
        -- UPDATE  TB_DOC_UPLOAD SET     INV_CODE        = P_EXIST_CLIENT_CODE WHERE   COMMON_ID       = P_DT_NUMBER;

        -- UPDATE INVESTOR_MASTER KYC = YEAS 
        UPDATE INVESTOR_MASTER SET APPROVED = NULL, KYC = 'YES'
        WHERE INV_CODE = V_INV_CODE;
        COMMIT;
        
        OPEN P_RESULT FOR
            SELECT 'Data Inserted successfully --> ' || V_MYSOURCEID || ' | '  || V_GENERATED_CLIENT_CODE || ' | ' || V_INV_CODE AS MESSAGE FROM DUAL;
            RETURN;

    END IF;
END IF;


-- Check if txtClientCodeOld length is >= 11 and ends with "001"
IF LENGTH(P_EXIST_CLIENT_CODE) >= 11 AND SUBSTR(P_EXIST_CLIENT_CODE, -3) = '001' THEN
    -- Extract the first 8 characters as MySourceId
    MYSOURCEID := SUBSTR(P_EXIST_CLIENT_CODE, 1, 8);

    -- Update CLIENT_MASTER for the main account
    UPDATE CLIENT_MASTER
    SET KYC = 'YES'
    WHERE CLIENT_CODE = MYSOURCEID;

    -- Update INVESTOR_MASTER
    UPDATE INVESTOR_MASTER
    SET KYC = 'YES'
    WHERE INV_CODE = P_EXIST_CLIENT_CODE;

    -- Update CLIENT_TEST
    UPDATE CLIENT_TEST
    SET CLIENT_CODEKYC = P_EXIST_CLIENT_CODE
    WHERE CLIENT_CODE = (SELECT CLIENT_CODE FROM CLIENT_TEST WHERE CLIENT_PAN = P_CLIENT_PAN AND CLIENT_NAME = P_ACCOUNT_NAME);

-- If txtClientCodeOld length is >= 11 and does NOT end with "001"
ELSIF LENGTH(P_EXIST_CLIENT_CODE) >= 11 AND SUBSTR(P_EXIST_CLIENT_CODE, -3) <> '001' THEN
    -- Extract the first 8 characters as MySourceId
    MYSOURCEID := SUBSTR(P_EXIST_CLIENT_CODE, 1, 8);

    -- Update CLIENT_MASTER
    UPDATE CLIENT_MASTER
    SET KYC = 'YES'
    WHERE CLIENT_CODE = MYSOURCEID;

    -- Update INVESTOR_MASTER
    UPDATE INVESTOR_MASTER 
    SET KYC = 'YES'
    WHERE INV_CODE = P_EXIST_CLIENT_CODE;

    -- Update CLIENT_TEST
    UPDATE CLIENT_TEST
    SET CLIENT_CODEKYC = P_EXIST_CLIENT_CODE
    WHERE CLIENT_CODE = (SELECT CLIENT_CODE FROM CLIENT_TEST WHERE CLIENT_PAN = P_CLIENT_PAN AND CLIENT_NAME = P_ACCOUNT_NAME);
    -- Commit the changes
    COMMIT;
END IF;


IF P_EXIST_CLIENT_CODE IS NULL OR P_EXIST_CLIENT_CODE = 0 THEN -- FRESH ACCOUNT OPENING

    INSERT INTO CLIENT_MASTER (
        PER_PINCODE, -- PER_PINCODE
        OFFICE_EMAIL, -- OFFICE_EMAIL
        PER_ADD1, -- PER_ADD1
        PER_ADD2, -- PER_ADD2
        REF_NAME1, -- REF_NAME1
        REF_NAME2, -- REF_NAME2
        REF_NAME3, -- REF_NAME3
        REF_NAME4, -- REF_NAME4
        REF_MOB1, -- REF_MOB1
        REF_MOB2, -- REF_MOB2
        REF_MOB3, -- REF_MOB3
        REF_MOB4, -- REF_MOB4
        CLIENT_TITLE, -- client_title
        TITLE_FATHER_SPOUSE, -- TITLE_FATHER_SPOUSE
        CLIENT_LNAME, -- CLIENT_LNAME
        CATEGORY_ID, -- category_id
        INVESTOR_CODE, -- investor_code
        GUEST_CD, -- GUEST_CD
        CLIENT_NAME, -- client_name
        FATHER_SPOUSE_NAME, -- FATHER_SPOUSE_NAME
        ADDRESS1, -- address1
        ADDRESS2, -- address2
        FAX, -- fax
        EMAIL, -- email
        PAN, -- pan
        DOB, -- dob
        MOBILE, -- mobile
        PINCODE, -- pincode
        GENDER, -- gender
        MARITAL_STATUS, -- MARITAL_STATUS
        LEAD_SOURCE, -- LEAD_SOURCE
        CITY_NAME, -- city_name
        STD1, -- std1
        STD2, -- std2
        PHONE1, -- phone1
        PHONE2, -- phone2
        STATUS, -- status
        OCC_ID, -- occ_id
        CITY_ID, -- city_id
        PER_CITY_ID, -- per_city_id
        PER_STATE_ID,
        COMM_ID, -- comm_id
        COMM_INT_ID, -- comm_int_id
        KYC, -- kyc
        SOURCEID, -- sourceid
        RM_CODE, -- rm_code
        LOGGEDUSERID, -- loggeduserid
        ACT_CAT, -- act_cat
        TIMEST
    ) VALUES (
        P_PERMANENT_PINCODE, -- PER_PINCODE
        P_EMAIL_OFFICIAL_VALUE, -- OFFICE_EMAIL
        P_PERMANENT_ADDRESS1, -- PER_ADD1
        P_PERMANENT_ADDRESS2, -- PER_ADD2
        P_REFERENCE_NAME1_VALUE, -- REF_NAME1
        P_REFERENCE_NAME2_VALUE, -- REF_NAME2
        P_REFERENCE_NAME3_VALUE, -- REF_NAME3
        P_REFERENCE_NAME4_VALUE, -- REF_NAME4
        P_MOBILE_NO1_VALUE, -- REF_MOB1
        P_MOBILE_NO2_VALUE, -- REF_MOB2
        P_MOBILE_NO3_VALUE, -- REF_MOB3
        P_MOBILE_NO4_VALUE, -- REF_MOB4
        P_SALUTATION1, -- client_title
        P_SALUTATION2, -- TITLE_FATHER_SPOUSE
        NULL, -- CLIENT_LNAME
        P_CLIENT_CAT, -- category_id
        V_P_STATUS_CAT, -- investor_code
        P_GUEST_CODE, -- GUEST_CD
        P_ACCOUNT_NAME, -- client_name
        P_FATHER_NAME, -- FATHER_SPOUSE_NAME
        P_MAILING_ADDRESS1, -- address1
        P_MAILING_ADDRESS2, -- address2
        P_FAX_VALUE, -- fax
        P_EMAIL_VALUE, -- email
        P_CLIENT_PAN, -- pan
        P_DOB, -- dob
        P_MOBILE_NO_VALUE, -- mobile
        P_MAILING_PINCODE, -- pincode
        NVL(SUBSTR(P_GENDER, 1, 1),         NULL), -- gender
        P_MARITAL_STATUS, -- MARITAL_STATUS
        P_LEAD_TYPE, -- LEAD_SOURCE
        NVL((SELECT CITY_NAME FROM CITY_MASTER WHERE CITY_ID = P_MAILING_CITY ), NULL), -- city_name
        NVL(SUBSTR(P_PHONE_OFFICE_STD_VALUE, 1, 5), NULL),                              -- std1
        NVL(SUBSTR(P_PHONE_RES_STD_VALUE, 1, 5), NULL),                                 -- std2
        P_PHONE_OFFICE_NUMBER_VALUE, -- phone1
        P_PHONE_RES_NUMBER_VALUE, -- phone2
        P_TAX_STATUS, -- status
        P_OCCUPATION, -- occ_id
        P_MAILING_CITY, -- city_id
        P_PERMANENT_CITY, -- per_city_id
        P_PERMANENT_STATE,
        NULL, -- comm_id
        NULL, -- comm_int_id
        'YES', -- kyc
        P_EMP_SRC, -- sourceid
        P_EMP_RM, -- rm_code
        P_LOGGED_IN_USER, -- loggeduserid
        P_ACCOUNT_CAT, -- act_cat
        SYSDATE
    );
    COMMIT;
    
    -- TAKING NEWLY CREATED ACCOUTN CLIENT_CODE FORM CLIENT_MASTER,IT WILL HELP TO FIND ADDED INVESTOR WIHT PASSING SOURCE_ID IN INVESTOR_MASTER
    SELECT MAX(CLIENT_CODE) INTO V_MYSOURCEID  
    FROM CLIENT_MASTER WHERE 
        UPPER(TRIM(CLIENT_NAME))    = UPPER(TRIM(P_ACCOUNT_NAME))       OR UPPER(TRIM(P_ACCOUNT_NAME)) IS NULL
        AND UPPER(TRIM(PAN))        = UPPER(TRIM(P_CLIENT_PAN))         OR UPPER(TRIM(P_CLIENT_PAN)) IS NULL
        AND UPPER(TRIM(ADDRESS1))   = UPPER(TRIM(P_MAILING_ADDRESS1))   OR UPPER(TRIM(P_MAILING_ADDRESS1)) IS NULL
        AND UPPER(TRIM(ADDRESS2))   = UPPER(TRIM(P_MAILING_ADDRESS2))   OR UPPER(TRIM(P_MAILING_ADDRESS2)) IS NULL
        AND UPPER(TRIM(MOBILE))     = UPPER(TRIM(P_MOBILE_NO_VALUE))    OR UPPER(TRIM(P_MOBILE_NO_VALUE)) IS NULL
        AND UPPER(TRIM(SOURCEID))   = UPPER(TRIM(P_EMP_SRC))            OR UPPER(TRIM(P_EMP_SRC)) IS NULL
        AND UPPER(TRIM(RM_CODE))    = UPPER(TRIM(P_EMP_RM))             OR UPPER(TRIM(P_EMP_RM)) IS NULL
    ;
    

    INSERT INTO CLIENT_TEST (
        CLIENT_CODE,
        CLIENTCODE_OLD,
        BUSINESS_CODE,
        KYC_STATUS,
        OCC_ID,
        STATUS,
        ACT_CAT,
        TITLE,
        CLIENT_NAME,
        TITLE_FATHER_SPOUSE,
        FATHER_SPOUSE_NAME,
        OTHERS1,
        GENDER,
        MARITAL_STATUS,
        NATIONALITY,
        RESIDENT_NRI,
        DOB,
        ANNUAL_INCOME,
        CLIENT_PAN,
        LEAD_SOURCE,
        G_NAME,
        G_NATIONALITY,
        G_PAN,
        GUEST_CODE,
        ADD1,
        ADD2,
        STATE_ID,
        CITY_ID,
        PINCODE,
        OVERSEAS_ADD,
        FAX,
        AADHAR_CARD_NO,
        EMAIL,
        STD1,
        TEL1,
        STD2,
        TEL2,
        MOBILE_NO,
        LOGGEDUSERID,
        TIMEST,
        CLIENT_CODEKYC,
        SOURCE_CODE,
        MAIN_CODE,
        BRANCH_CODE
    ) VALUES (
        NULL, -- Assuming CLIENT_CODE is auto-generated
        P_EXIST_CLIENT_CODE,
        P_RM_BUSINESS_CODE,
        'YES',
        P_OCCUPATION,
        P_TAX_STATUS ,
        P_ACCOUNT_CAT,
        P_SALUTATION1,
        P_ACCOUNT_NAME,
        P_SALUTATION2,
        P_FATHER_NAME,
        P_ACCOUNT_OTHER,
        P_GENDER,
        P_MARITAL_STATUS,
        P_NATIONALITY,
        P_RESIDENT_NRI,
        P_DOB,
        P_ANNUAL_INCOME,
        P_CLIENT_PAN,
        P_LEAD_TYPE,
        P_GUARDIAN_PERSON,
        P_GUARDIAN_PERSON_NATIONALITY,
        P_GUARDIAN_PERSON_PAN,
        P_GUEST_CODE,
        P_MAILING_ADDRESS1,
        P_MAILING_ADDRESS2,
        P_MAILING_STATE,
        P_MAILING_CITY,
        P_MAILING_PINCODE,
        P_NRI_ADDRESS,
        P_FAX_VALUE,
        P_AADHAR_VALUE,
        P_EMAIL_VALUE,
        P_PHONE_OFFICE_STD_VALUE,
        P_PHONE_OFFICE_NUMBER_VALUE,
        P_PHONE_RES_STD_VALUE,
        P_PHONE_RES_NUMBER_VALUE,
        P_MOBILE_NO_VALUE,
        P_LOGGED_IN_USER,
        SYSDATE,
        P_EXIST_CLIENT_CODE, 
        NULL, -- SOURCE_CODE WILL BE AUTO UPDATE ON CLIENT_CODEKYC UPDATION IN CILENT_TEST
        NULL, -- MAIN_CODE WILL BE UPDATE AFTER AH CODE GENERATED AND FOR CLIENT HEAD MAIN_CODE WILL BE UDPATED WITH GENERATD AH CLIENT_CODE 
        P_EMP_SRC
    ) RETURNING CLIENT_CODE INTO V_GENERATED_CLIENT_CODE;
    COMMIT; 
    
    INSERT INTO INVESTOR_MASTER (
        INV_CODE,             -- INV_CODE
        INVESTOR_TITLE,       -- Investor Title
        APPROVED,             -- APPROVE
        BRANCH_CODE,          -- Branch Code
        INVESTOR_NAME,        -- Investor Name
        G_NAME,
        G_PAN,
        MAR_STATUS,
        ADDRESS1,             -- Address Line 1
        ADDRESS2,             -- Address Line 2
        FAX,                  -- Fax Number
        EMAIL,                -- Email Address
        PAN,                  -- PAN Number
        DOB,                  -- Date of Birth
        MOBILE,               -- Mobile Number
        AADHAR_CARD_NO,       -- Aadhar Card Number
        PINCODE,              -- Pincode
        GENDER,               -- Gender
        PHONE1,               -- Primary Phone Number
        PHONE2,               -- Secondary Phone Number
        STATUS,               -- Status
        CITY_ID,              -- City ID
        KYC,                  -- KYC Status
        SOURCE_ID,            -- Source ID
        RM_CODE,              -- Relationship Manager Code
        REL_ID,               -- Relationship ID
        OCCUPATION_ID,         -- Occupation ID
        TIMEST,
        LOGGEDUSERID
    ) VALUES (
        NULL,                           -- INV_CODE
        P_SALUTATION1,                  -- investor_title
        NULL,                           -- APPROVE
        P_EMP_SRC,                      -- branch_Code
        P_ACCOUNT_NAME,                 -- investor_name
        P_GUARDIAN_PERSON,
        P_GUARDIAN_PERSON_PAN,
        P_MARITAL_STATUS,
        P_MAILING_ADDRESS1,             -- address1
        P_MAILING_ADDRESS2,             -- address2
        P_FAX_VALUE,                    -- fax
        P_EMAIL_VALUE,                  -- email
        P_CLIENT_PAN,                   -- pan
        P_DOB,                          -- dob
        P_MOBILE_NO_VALUE,              -- mobile
        P_AADHAR_VALUE,                 -- AADHAR_CARD_NO
        P_MAILING_PINCODE,              -- pincode
        NVL(SUBSTR(P_GENDER, 1, 1), NULL),    -- gender
        P_PHONE_OFFICE_NUMBER_VALUE,          -- phone1
        P_PHONE_RES_NUMBER_VALUE,             -- phone2
        P_STATUS_CAT,                   -- status
        P_MAILING_CITY,                 -- city_id
        'YES',                          -- kyc
        V_MYSOURCEID,                   -- source_id
        P_EMP_RM,                       -- rm_code
        NULL,                           -- rel_id
        P_OCCUPATION,                   -- occupation_id
        SYSDATE,
        P_LOGGED_IN_USER
    );
    COMMIT;
    
    -- TAKING INVESTOR_CODE OF NEW ADDED INVESTOR IN INVESTOR_MASTER AND HERE V_MYSOURCEID IS ADDED CLIENT_CODE OF CLIENT_MASTER
    SELECT MAX(INV_CODE) INTO V_INV_CODE
    FROM INVESTOR_MASTER WHERE SOURCE_ID= V_MYSOURCEID
    ;
    
    -- UPDATE CLIENT_TEST WITH CLIENT_CODE = MAIN_CODE, AND CLIENT_CODEKYC = INV_CODE
    UPDATE  CLIENT_TEST 
    SET     MAIN_CODE= V_GENERATED_CLIENT_CODE, 
            CLIENT_CODEKYC = V_INV_CODE
    WHERE   CLIENT_CODE = V_GENERATED_CLIENT_CODE;
    
    -- UPDATE TB_COD_UPLAOD INV_CODE WITH INVESTOR CODE OF ADDED HEAD INVESTOR 
    UPDATE  TB_DOC_UPLOAD 
    SET     INV_CODE        = V_INV_CODE                  
    WHERE   COMMON_ID       = P_DT_NUMBER;

    -- UPDATE INVESTOR_MASTER KYC = YEAS 
    UPDATE INVESTOR_MASTER SET APPROVED = NULL, KYC = 'YES'
    WHERE INV_CODE = V_INV_CODE;
    COMMIT;
    
    OPEN P_RESULT FOR
        SELECT 'Data Inserted successfully --> ' || V_MYSOURCEID || ' | '  || V_GENERATED_CLIENT_CODE || ' | ' || V_INV_CODE AS MESSAGE FROM DUAL;
        RETURN;
END IF;

END PSM_AO_INSERT_CLIENT_DATA;