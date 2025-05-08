CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_UPDATE_CLIENT_DATA(
    P_SEARCH_CLIENT_CODE        IN VARCHAR2,
    P_DT_NUMBER                 IN VARCHAR2,
    P_GUEST_CODE                IN VARCHAR2,
    P_EXIST_CLIENT_CODE         IN NUMBER,
    P_RM_BUSINESS_CODE          IN VARCHAR2,
    P_TAX_STATUS                IN VARCHAR2,
    P_OCCUPATION                IN NUMBER,
    P_STATUS_CAT                IN VARCHAR2,
    P_CLIENT_CAT                IN VARCHAR2,
    P_ACCOUNT_CAT               IN NUMBER,
    P_SALUTATION1               IN VARCHAR2,
    P_ACCOUNT_NAME              IN VARCHAR2,
    P_SALUTATION2               IN VARCHAR2,
    P_FATHER_NAME               IN VARCHAR2,
    P_ACCOUNT_OTHER             IN VARCHAR2,
    P_GENDER                    IN VARCHAR2,
    P_MARITAL_STATUS            IN VARCHAR2,
    P_NATIONALITY               IN VARCHAR2,
    P_RESIDENT_NRI              IN VARCHAR2,
    P_DOB                       DATE,
    P_ANNUAL_INCOME             IN VARCHAR2,
    P_CLIENT_PAN                IN VARCHAR2,
    P_LEAD_TYPE                 IN VARCHAR2,
    P_GUARDIAN_PERSON           IN VARCHAR2,
    P_GUARDIAN_PERSON_NATIONALITY IN VARCHAR2,
    P_GUARDIAN_PERSON_PAN       IN VARCHAR2,
    P_MAILING_ADDRESS1          IN VARCHAR2,
    P_MAILING_ADDRESS2          IN VARCHAR2,
    P_MAILING_STATE             IN VARCHAR2,
    P_MAILING_CITY              IN VARCHAR2,
    P_MAILING_PINCODE           IN VARCHAR2,
    P_PERMANENT_ADDRESS1        IN VARCHAR2,
    P_PERMANENT_ADDRESS2        IN VARCHAR2,
    P_PERMANENT_STATE           IN VARCHAR2,
    P_PERMANENT_CITY            IN VARCHAR2,
    P_PERMANENT_PINCODE         IN VARCHAR2,
    P_NRI_ADDRESS               IN VARCHAR2,
    P_FAX_VALUE                 IN VARCHAR2,
    P_AADHAR_VALUE              IN NUMBER,
    P_EMAIL_VALUE               IN VARCHAR2,
    P_EMAIL_OFFICIAL_VALUE      IN VARCHAR2,
    P_PHONE_OFFICE_STD_VALUE    IN VARCHAR2,
    P_PHONE_OFFICE_NUMBER_VALUE IN VARCHAR2,
    P_PHONE_RES_STD_VALUE       IN VARCHAR2,
    P_PHONE_RES_NUMBER_VALUE    IN VARCHAR2,
    P_MOBILE_NO_VALUE           IN VARCHAR2,
    P_REFERENCE_NAME1_VALUE     IN VARCHAR2,
    P_REFERENCE_NAME2_VALUE     IN VARCHAR2,
    P_REFERENCE_NAME3_VALUE     IN VARCHAR2,
    P_REFERENCE_NAME4_VALUE     IN VARCHAR2,
    P_MOBILE_NO1_VALUE          IN NUMBER,
    P_MOBILE_NO2_VALUE          IN NUMBER,
    P_MOBILE_NO3_VALUE          IN NUMBER,
    P_MOBILE_NO4_VALUE          IN NUMBER,
    P_LOGGED_IN_USER            IN VARCHAR2,
    P_EMP_RM                    IN NUMBER,
    P_EMP_SRC                   IN NUMBER,
    P_RESULT OUT                SYS_REFCURSOR
) AS

    DCT_FLAG                    NUMBER;
    V_P_STATUS_CAT              NUMBER;
BEGIN



    IF LENGTH(TRIM(P_EXIST_CLIENT_CODE)) >= 11 THEN
        -- Check if account exists (DCT_FLAG)
        SELECT COUNT(*)
        INTO DCT_FLAG
        FROM CLIENT_TEST
        WHERE CLIENT_CODEKYC = P_EXIST_CLIENT_CODE;

        -- If account exists, proceed with the update
        IF DCT_FLAG > 0 THEN
            UPDATE CLIENT_TEST
            SET
                CLIENTCODE_OLD          = P_EXIST_CLIENT_CODE,
                guest_code              = P_GUEST_CODE,  
                BUSINESS_CODE           = P_RM_BUSINESS_CODE,
                OCC_ID                  = P_OCCUPATION,
                STATUS                  = P_TAX_STATUS ,
                ACT_CAT                 = P_ACCOUNT_CAT,
                TITLE                   = P_SALUTATION1,
                CLIENT_NAME             = P_ACCOUNT_NAME,
                TITLE_FATHER_SPOUSE     = P_SALUTATION2,
                FATHER_SPOUSE_NAME      = P_FATHER_NAME,
                OTHERS1                 = P_ACCOUNT_OTHER,
                GENDER                  = P_GENDER,
                MARITAL_STATUS          = P_MARITAL_STATUS,
                NATIONALITY             = P_NATIONALITY,
                RESIDENT_NRI            = P_RESIDENT_NRI,
                DOB                     = P_DOB,
                ANNUAL_INCOME           = P_ANNUAL_INCOME,
                CLIENT_PAN              = P_CLIENT_PAN,
                LEAD_SOURCE               = P_LEAD_TYPE,
                G_NAME                  = P_GUARDIAN_PERSON,
                G_NATIONALITY           = P_GUARDIAN_PERSON_NATIONALITY,
                G_PAN                   = P_GUARDIAN_PERSON_PAN,
                ADD1                    = P_MAILING_ADDRESS1,
                ADD2                    = P_MAILING_ADDRESS2,
                STATE_ID                = P_MAILING_STATE,
                CITY_ID                 = P_MAILING_CITY,
                PINCODE                 = P_MAILING_PINCODE,
                OVERSEAS_ADD            = P_NRI_ADDRESS ,
                FAX                     = P_FAX_VALUE ,
                AADHAR_CARD_NO          = P_AADHAR_VALUE,
                EMAIL                   = P_EMAIL_VALUE ,
                STD1                    = NVL (SUBSTR(P_PHONE_OFFICE_STD_VALUE, 1, 5), NULL) ,
                TEL1                    = P_PHONE_OFFICE_NUMBER_VALUE,
                STD2                    = NVL (SUBSTR(P_PHONE_RES_STD_VALUE, 1, 5), NULL) ,
                TEL2                    = P_PHONE_RES_NUMBER_VALUE ,
                MOBILE_NO               = P_MOBILE_NO_VALUE,
                MODIFY_DATE             = SYSDATE,
                MODIFY_USER             = P_LOGGED_IN_USER,
                
                PER_STATE_ID         = P_PERMANENT_STATE,
                PER_PINCODE          = P_PERMANENT_PINCODE,  
                PER_ADD1             = P_PERMANENT_ADDRESS1,  
                PER_ADD2             = P_PERMANENT_ADDRESS2,
                PER_CITY_ID          = P_PERMANENT_CITY
            WHERE CLIENT_CODE = P_SEARCH_CLIENT_CODE;
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
            WHERE CM.CLIENT_CODE = (SELECT SUBSTR(CLIENT_CODEKYC, 1, 8) FROM CLIENT_TEST WHERE CLIENT_CODE = P_SEARCH_CLIENT_CODE);

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
                IM.OCCUPATION_ID        = P_OCCUPATION, 
                IM.CATEGORY_ID          = P_CLIENT_CAT, 
                IM.MODIFY_DATE          = SYSDATE,
                IM.MODIFY_USER          = P_LOGGED_IN_USER
            WHERE INV_CODE = (SELECT CLIENT_CODEKYC FROM CLIENT_TEST WHERE CLIENT_CODE = P_SEARCH_CLIENT_CODE);

            COMMIT;
            OPEN P_RESULT FOR SELECT 'Client Data Updated Successfully!' AS MESSAGE FROM DUAL; 
        ELSE
            OPEN P_RESULT FOR SELECT 'Account Does not exist!' AS MESSAGE FROM DUAL;
        END IF;
    ELSE
        -- If P_EXIST_CLIENT_CODE length is invalid, return error message
        OPEN P_RESULT FOR
            SELECT 'Invalid Client' AS MESSAGE FROM DUAL;
    END IF;
END;
/
