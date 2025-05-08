CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_MM_Update_By_Inv( 
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
    v_generated_client_code     CLIENT_TEST.client_code%TYPE;
    v_head_ah         CLIENT_TEST.client_code%TYPE;
    ct_flag           NUMBER := 0;

    -- Variables to hold values from INVESTOR_MASTER
    v_branch_code     INVESTOR_MASTER.branch_code%TYPE;
    v_address1        INVESTOR_MASTER.address1%TYPE;
    v_address2        INVESTOR_MASTER.address2%TYPE;
    v_fax             INVESTOR_MASTER.fax%TYPE;
    v_pincode         INVESTOR_MASTER.pincode%TYPE;
    v_phone1          INVESTOR_MASTER.phone1%TYPE;
    v_phone2          INVESTOR_MASTER.phone2%TYPE;
    v_status          INVESTOR_MASTER.status%TYPE;
    v_city_id         INVESTOR_MASTER.city_id%TYPE;

    -- Variables from CLIENT_TEST
    v_nationality     CLIENT_TEST.nationality%TYPE;
    v_resident_nri    CLIENT_TEST.resident_nri%TYPE;
    v_lead_type       CLIENT_TEST.lead_type%TYPE;
    v_g_nationality   CLIENT_TEST.g_nationality%TYPE;
    v_add1            CLIENT_TEST.add1%TYPE;
    v_add2            CLIENT_TEST.add2%TYPE;
    v_state_id        CLIENT_TEST.state_id%TYPE;
    v_ct_city_id      CLIENT_TEST.city_id%TYPE;
    v_std1            CLIENT_TEST.std1%TYPE;
    v_tel1            CLIENT_TEST.tel1%TYPE;
    v_std2            CLIENT_TEST.std2%TYPE;
    v_tel2            CLIENT_TEST.tel2%TYPE;
    v_main_code       CLIENT_TEST.main_code%TYPE;
    v_branch_code_ct  CLIENT_TEST.branch_code%TYPE;

    V_AADHAR_VALUE VARCHAR2(100) := NULL;
BEGIN
    
/*
    IF P_AADHAR_VALUE IS NOT NULL AND LENGTH(TRIM(TO_CHAR(P_AADHAR_VALUE))) = 12 THEN
        SELECT WEALTHMAKER.USER_LOG(TRIM(TO_CHAR(P_AADHAR_VALUE)))
        INTO V_AADHAR_VALUE
        FROM DUAL;
    END IF;
*/
    
    
    -- Check if the record exists in CLIENT_TEST
    SELECT COUNT(*) INTO ct_flag
    FROM client_test
    WHERE client_codekyc = P_UPDATEBYCLIENTCODE;

    -- Get head account
    BEGIN
        SELECT client_code INTO v_head_ah
        FROM client_test
        WHERE client_codekyc = P_EXIST_CLIENT_CODE
        AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            v_head_ah := NULL;
    END;

    -- Fetch values OF HEAD from INVESTOR_MASTER
    BEGIN
        SELECT branch_code, address1, address2, fax, pincode, phone1, phone1, phone2, status, city_id
        INTO v_branch_code, v_address1, v_address2, v_fax, v_pincode, v_phone1, v_phone1, v_phone2, v_status, v_city_id
        FROM INVESTOR_MASTER
        WHERE INV_CODE = P_EXIST_CLIENT_CODE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
    END;

    -- Fetch values OF HEAD from CLIENT_TEST
    BEGIN
        SELECT nationality, resident_nri, lead_type, g_nationality, add1, add2, state_id, 
        city_id, pincode, std1, tel1, std2, tel2, main_code, branch_code
        INTO v_nationality, v_resident_nri, v_lead_type, v_g_nationality, v_add1, v_add2, v_state_id,
        v_ct_city_id, v_pincode, v_std1, v_tel1, v_std2, v_tel2, v_main_code, v_branch_code_ct
        FROM CLIENT_TEST
        WHERE CLIENT_CODEkyc = P_EXIST_CLIENT_CODE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL;
    END;

    IF P_UPDATEBYCLIENTCODE IS NOT NULL THEN
        -- Update investor_master
        UPDATE investor_master
        SET 
            investor_title = P_SALUTATION1,
            investor_name = P_ACCOUNT_NAME,
            mobile = P_MOBILE,
            email = P_EMAIL,
            pan = P_PAN,
            dob = P_DOB,
            rel_id = P_RELATION,
            g_name = P_GNAME,
            g_pan = P_GPAN,
            occupation_id = P_OCCUPATION,
            approved = P_APPROVE,
            gender = P_GENDER,
            modify_date = SYSDATE,
            modify_user = P_LOGGED_IN_USER,
            aadhar_card_no = P_AADHAR_VALUE,
            branch_code = v_branch_code,
            address1 = v_address1,
            address2 = v_address2,
            fax = v_fax,
            pincode = v_pincode,
            phone = v_phone1,
            phone1 = v_phone1,
            phone2 = v_phone2,
            status = v_status,
            city_id = v_city_id
        WHERE inv_code = P_UPDATEBYCLIENTCODE;

        IF ct_flag > 0 THEN
            -- Update existing CLIENT_TEST
            UPDATE CLIENT_TEST
            SET 
                clientcode_old = P_EXIST_CLIENT_CODE,
                business_code = P_RM_BUSINESS_CODE,
                title = P_SALUTATION1,
                client_name = P_ACCOUNT_NAME,
                mobile_no = P_MOBILE,
                email = P_EMAIL,
                client_pan = P_PAN,
                dob = P_DOB,
                relation_id = P_RELATION,
                g_name = P_GNAME,
                g_pan = P_GPAN,
                occ_id = P_OCCUPATION,
                kyc_status = P_KYC,
                inv_kyc = P_KYC,
                approved = P_APPROVE,
                gender = P_GENDER,
                modify_date = SYSDATE,
                modify_user = P_LOGGED_IN_USER,
                is_nominee = P_NOM,
                nominee_per = P_ALLO,
                aadhar_card_no = P_AADHAR_VALUE,
                nationality = v_nationality,
                resident_nri = v_resident_nri,
                lead_type = v_lead_type,
                g_nationality = v_g_nationality,
                add1 = v_add1,
                add2 = v_add2,
                state_id = v_state_id,
                city_id = v_ct_city_id,
                pincode = v_pincode,
                std1 = v_std1,
                tel1 = v_tel1,
                std2 = v_std2,
                tel2 = v_tel2,
                main_code = v_main_code
            WHERE client_codekyc = P_UPDATEBYCLIENTCODE;  

        ELSE
            -- Insert new CLIENT_TEST
            INSERT INTO CLIENT_TEST (
                CLIENT_CODE, CLIENTCODE_OLD, BUSINESS_CODE, TITLE, CLIENT_NAME, LOGGEDUSERID, MAIN_CODE,
                MOBILE_NO, EMAIL, CLIENT_PAN, DOB, RELATION_ID, G_NAME, G_PAN, OCC_ID, KYC_STATUS, INV_KYC, APPROVED,
                GENDER, AADHAR_CARD_NO, TIMEST, IS_NOMINEE, NOMINEE_PER,                
                
                lead_type , g_nationality , add1 , add2 ,  state_id , city_id , pincode , std1 , tel1 , std2 , tel2 , branch_code 
            )
            VALUES (
                NULL, P_EXIST_CLIENT_CODE, P_RM_BUSINESS_CODE, P_SALUTATION1, P_ACCOUNT_NAME, P_LOGGED_IN_USER, v_main_code,
                P_MOBILE, P_EMAIL, P_PAN, P_DOB, P_RELATION, P_GNAME, P_GPAN, P_OCCUPATION, P_KYC, P_KYC, P_APPROVE,
                P_GENDER, P_AADHAR_VALUE, SYSDATE, P_NOM, P_ALLO,                
                
                v_lead_type, v_g_nationality, v_add1, v_add2, v_state_id, v_ct_city_id, v_pincode, v_std1, v_tel1, v_std2, v_tel2, v_branch_code_ct
            )
            RETURNING client_code INTO v_generated_client_code;

            -- Set client_codekyc
            UPDATE client_test
            SET client_codekyc = P_UPDATEBYCLIENTCODE
            WHERE client_code = v_generated_client_code;
        END IF;

        OPEN P_RESULT FOR 
            SELECT 'Member Updated Successfully' AS message
            FROM dual;

    ELSE
        OPEN P_RESULT FOR 
            SELECT 'Invalid member ID' AS message
            FROM dual;
    END IF;

END PSM_AO_MM_Update_By_Inv;
/
