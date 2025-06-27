CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_MM_UPDATE_BY_INV( 
    P_DT_NUMBER                IN VARCHAR2,
    P_EXIST_CLIENT_CODE        IN NUMBER,
    P_RM_BUSINESS_CODE         IN VARCHAR2,
    P_SALUTATION1              IN VARCHAR2,
    P_ACCOUNT_NAME             IN VARCHAR2,
    P_LOGGED_IN_USER           IN VARCHAR2,
    P_CLIENT_CODE_IN_MAIN      IN VARCHAR2,
    P_MOBILE                   IN VARCHAR2,
    P_PAN                      IN VARCHAR2,
    P_EMAIL                    IN VARCHAR2,
    P_DOB                      IN DATE,
    P_RELATION                 IN NUMBER,
    P_GNAME                    IN VARCHAR2,
    P_GPAN                     IN VARCHAR2,
    P_OCCUPATION               IN NUMBER,
    P_KYC                      IN VARCHAR2,
    P_APPROVE                  IN VARCHAR2,
    P_GENDER                   IN VARCHAR2,
    P_NOM                      IN VARCHAR2,
    P_ALLO                     IN NUMBER,
    P_UPDATEBYCLIENTCODE       IN VARCHAR2,
    P_AADHAR_VALUE             IN VARCHAR2,
    P_RESULT                   OUT SYS_REFCURSOR
) AS
    V_GENERATED_CLIENT_CODE     CLIENT_TEST.CLIENT_CODE%TYPE;
    V_HEAD_AH         CLIENT_TEST.CLIENT_CODE%TYPE;
    CT_FLAG           NUMBER := 0;

    -- Variables to hold values from INVESTOR_MASTER
    V_BRANCH_CODE     INVESTOR_MASTER.BRANCH_CODE%TYPE;
    V_ADDRESS1        INVESTOR_MASTER.ADDRESS1%TYPE;
    V_ADDRESS2        INVESTOR_MASTER.ADDRESS2%TYPE;
    V_FAX             INVESTOR_MASTER.FAX%TYPE;
    V_PINCODE         INVESTOR_MASTER.PINCODE%TYPE;
    V_PHONE1          INVESTOR_MASTER.PHONE1%TYPE;
    V_PHONE2          INVESTOR_MASTER.PHONE2%TYPE;
    V_STATUS          INVESTOR_MASTER.STATUS%TYPE;
    V_CITY_ID         INVESTOR_MASTER.CITY_ID%TYPE;

    -- Variables from CLIENT_TEST
    V_NATIONALITY     CLIENT_TEST.NATIONALITY%TYPE;
    V_RESIDENT_NRI    CLIENT_TEST.RESIDENT_NRI%TYPE;
    V_LEAD_TYPE       CLIENT_TEST.LEAD_TYPE%TYPE;
    V_G_NATIONALITY   CLIENT_TEST.G_NATIONALITY%TYPE;
    V_ADD1            CLIENT_TEST.ADD1%TYPE;
    V_ADD2            CLIENT_TEST.ADD2%TYPE;
    V_STATE_ID        CLIENT_TEST.STATE_ID%TYPE;
    V_CT_CITY_ID      CLIENT_TEST.CITY_ID%TYPE;
    V_STD1            CLIENT_TEST.STD1%TYPE;
    V_TEL1            CLIENT_TEST.TEL1%TYPE;
    V_STD2            CLIENT_TEST.STD2%TYPE;
    V_TEL2            CLIENT_TEST.TEL2%TYPE;
    V_MAIN_CODE       CLIENT_TEST.MAIN_CODE%TYPE;
    V_BRANCH_CODE_CT  CLIENT_TEST.BRANCH_CODE%TYPE;

    V_AADHAR_VALUE VARCHAR2(100) := NULL;
    
    V_FULL_GENDER     CLIENT_TEST.GENDER%TYPE;
BEGIN
    
/*
    IF P_AADHAR_VALUE IS NOT NULL AND LENGTH(TRIM(TO_CHAR(P_AADHAR_VALUE))) = 12 THEN
        SELECT WEALTHMAKER.USER_LOG(TRIM(TO_CHAR(P_AADHAR_VALUE)))
        INTO V_AADHAR_VALUE
        FROM DUAL;
    END IF;
*/
    
    IF TRIM(P_GENDER) IS NOT NULL THEN
        IF SUBSTR(UPPER(P_GENDER),1,2) = 'F' THEN
            V_FULL_GENDER := 'FEMALE';
        ELSIF SUBSTR(UPPER(P_GENDER),1,2) = 'M' THEN
            V_FULL_GENDER := 'MALE';
        ELSE
            V_FULL_GENDER := 'OTHERS';
        END IF;
    END IF;
        
    -- Check if the record exists in CLIENT_TEST
    SELECT COUNT(*) INTO CT_FLAG
    FROM CLIENT_TEST
    WHERE CLIENT_CODEKYC = P_UPDATEBYCLIENTCODE;

    -- Get head account
    BEGIN
        SELECT CLIENT_CODE INTO V_HEAD_AH
        FROM CLIENT_TEST
        WHERE CLIENT_CODEKYC = P_EXIST_CLIENT_CODE
        AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            V_HEAD_AH := NULL;
    END;

    -- Fetch values OF HEAD from INVESTOR_MASTER
    BEGIN
        SELECT BRANCH_CODE, ADDRESS1, ADDRESS2, FAX, PINCODE, PHONE1, PHONE1, PHONE2, STATUS, CITY_ID
        INTO V_BRANCH_CODE, V_ADDRESS1, V_ADDRESS2, V_FAX, V_PINCODE, V_PHONE1, V_PHONE1, V_PHONE2, V_STATUS, V_CITY_ID
        FROM INVESTOR_MASTER
        WHERE INV_CODE = P_EXIST_CLIENT_CODE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
    END;

    -- Fetch values OF HEAD from CLIENT_TEST
    BEGIN
        SELECT NATIONALITY, RESIDENT_NRI, LEAD_TYPE, G_NATIONALITY, ADD1, ADD2, STATE_ID, 
        CITY_ID, PINCODE, STD1, TEL1, STD2, TEL2, MAIN_CODE, BRANCH_CODE
        INTO V_NATIONALITY, V_RESIDENT_NRI, V_LEAD_TYPE, V_G_NATIONALITY, V_ADD1, V_ADD2, V_STATE_ID,
        V_CT_CITY_ID, V_PINCODE, V_STD1, V_TEL1, V_STD2, V_TEL2, V_MAIN_CODE, V_BRANCH_CODE_CT
        FROM CLIENT_TEST
        WHERE CLIENT_CODEKYC = P_EXIST_CLIENT_CODE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
    END;

    IF P_UPDATEBYCLIENTCODE IS NOT NULL THEN
        -- Update investor_master
        UPDATE INVESTOR_MASTER
        SET 
            INVESTOR_TITLE  = P_SALUTATION1,
            INVESTOR_NAME   = P_ACCOUNT_NAME,
            MOBILE          = P_MOBILE,
            EMAIL           = P_EMAIL,
            PAN             = P_PAN,
            DOB             = P_DOB,
            REL_ID          = P_RELATION,
            G_NAME          = P_GNAME,
            G_PAN           = P_GPAN,
            OCCUPATION_ID   = P_OCCUPATION,
            APPROVED        = P_APPROVE,
            GENDER          = P_GENDER,
            MODIFY_DATE     = SYSDATE,
            MODIFY_USER     = P_LOGGED_IN_USER,
            AADHAR_CARD_NO  = P_AADHAR_VALUE,
            BRANCH_CODE     = V_BRANCH_CODE,
            ADDRESS1        = V_ADDRESS1,
            ADDRESS2        = V_ADDRESS2,
            FAX             = V_FAX,
            PINCODE         = V_PINCODE,
            PHONE           = V_PHONE1,
            PHONE1          = V_PHONE1,
            PHONE2          = V_PHONE2,
            STATUS          = V_STATUS,
            CITY_ID         = V_CITY_ID
        WHERE INV_CODE = P_UPDATEBYCLIENTCODE;

        IF CT_FLAG > 0 THEN
            -- Update existing CLIENT_TEST
            UPDATE CLIENT_TEST
            SET 
                CLIENTCODE_OLD  = P_EXIST_CLIENT_CODE,
                BUSINESS_CODE   = P_RM_BUSINESS_CODE,
                TITLE           = P_SALUTATION1,
                CLIENT_NAME     = P_ACCOUNT_NAME,
                MOBILE_NO       = P_MOBILE,
                EMAIL           = P_EMAIL,
                CLIENT_PAN      = P_PAN,
                DOB             = P_DOB,
                RELATION_ID     = P_RELATION,
                G_NAME          = P_GNAME,
                G_PAN           = P_GPAN,
                OCC_ID          = P_OCCUPATION,
                KYC_STATUS      = P_KYC,
                INV_KYC         = P_KYC,
                APPROVED        = P_APPROVE,
                GENDER          = V_FULL_GENDER,
                MODIFY_DATE     = SYSDATE,
                MODIFY_USER     = P_LOGGED_IN_USER,
                IS_NOMINEE      = P_NOM,
                NOMINEE_PER     = P_ALLO,
                AADHAR_CARD_NO  = P_AADHAR_VALUE,
                NATIONALITY     = V_NATIONALITY,
                RESIDENT_NRI    = V_RESIDENT_NRI,
                LEAD_TYPE       = V_LEAD_TYPE,
                G_NATIONALITY   = V_G_NATIONALITY,
                ADD1            = V_ADD1,
                ADD2            = V_ADD2,
                STATE_ID        = V_STATE_ID,
                CITY_ID         = V_CT_CITY_ID,
                PINCODE         = V_PINCODE,
                STD1            = V_STD1,
                TEL1            = V_TEL1,
                STD2            = V_STD2,
                TEL2            = V_TEL2,
                MAIN_CODE       = V_MAIN_CODE
            WHERE CLIENT_CODEKYC = P_UPDATEBYCLIENTCODE;  

        ELSE
            -- Insert new CLIENT_TEST
            INSERT INTO CLIENT_TEST (
                CLIENT_CODE, CLIENTCODE_OLD, BUSINESS_CODE, TITLE, CLIENT_NAME, LOGGEDUSERID, MAIN_CODE,
                MOBILE_NO, EMAIL, CLIENT_PAN, DOB, RELATION_ID, G_NAME, G_PAN, OCC_ID, KYC_STATUS, INV_KYC, APPROVED,
                GENDER, AADHAR_CARD_NO, TIMEST, IS_NOMINEE, NOMINEE_PER, 
                LEAD_TYPE , G_NATIONALITY , ADD1 , ADD2 ,  STATE_ID , CITY_ID , PINCODE , STD1 , TEL1 , STD2 , TEL2 , BRANCH_CODE 
            )
            VALUES (
                NULL, P_EXIST_CLIENT_CODE, P_RM_BUSINESS_CODE, P_SALUTATION1, P_ACCOUNT_NAME, P_LOGGED_IN_USER, V_MAIN_CODE,
                P_MOBILE, P_EMAIL, P_PAN, P_DOB, P_RELATION, P_GNAME, P_GPAN, P_OCCUPATION, P_KYC, P_KYC, P_APPROVE,
                V_FULL_GENDER, P_AADHAR_VALUE, SYSDATE, P_NOM, P_ALLO,
                V_LEAD_TYPE, V_G_NATIONALITY, V_ADD1, V_ADD2, V_STATE_ID, V_CT_CITY_ID, V_PINCODE, V_STD1, V_TEL1, V_STD2, V_TEL2, V_BRANCH_CODE_CT
            )
            RETURNING CLIENT_CODE INTO V_GENERATED_CLIENT_CODE;

            -- Set client_codekyc
            UPDATE CLIENT_TEST
            SET CLIENT_CODEKYC = P_UPDATEBYCLIENTCODE
            WHERE CLIENT_CODE = V_GENERATED_CLIENT_CODE;
        END IF;

        OPEN P_RESULT FOR 
            SELECT 'Member Updated Successfully' AS MESSAGE
            FROM DUAL;

    ELSE
        OPEN P_RESULT FOR 
            SELECT 'Invalid member ID' AS MESSAGE
            FROM DUAL;
    END IF;

END PSM_AO_MM_UPDATE_BY_INV;
/
