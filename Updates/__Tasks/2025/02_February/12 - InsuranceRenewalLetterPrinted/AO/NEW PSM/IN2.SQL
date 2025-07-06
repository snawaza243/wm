create or replace PROCEDURE PSM_AO_INSERT_CLIENT_DATA(
    P_DT_NUMBER IN VARCHAR2,
    P_EXIST_CLIENT_CODE IN NUMBER,
    P_RM_BUSINESS_CODE IN VARCHAR2,
    P_TAX_STATUS IN VARCHAR2,
    P_OCCUPATION IN NUMBER,
    P_STATUS_CAT IN VARCHAR2,
    P_CLIENT_CAT IN VARCHAR2,
    P_ACCOUNT_CAT IN NUMBER,
    P_SALUTATION1 IN VARCHAR2,
    P_ACCOUNT_NAME IN VARCHAR2,
    P_SALUTATION2 IN VARCHAR2,
    P_FATHER_NAME IN VARCHAR2,
    P_ACCOUNT_OTHER IN VARCHAR2,
    P_GENDER IN VARCHAR2,
    P_MARITAL_STATUS IN VARCHAR2,
    P_NATIONALITY IN VARCHAR2,
    P_RESIDENT_NRI IN VARCHAR2,
    P_DOB IN DATE,
    P_ANNUAL_INCOME IN VARCHAR2,
    P_CLIENT_PAN IN VARCHAR2,
    P_LEAD_TYPE IN VARCHAR2,
    P_GUARDIAN_PERSON IN VARCHAR2,
    P_GUARDIAN_PERSON_NATIONALITY IN VARCHAR2,
    P_GUARDIAN_PERSON_PAN IN VARCHAR2,
    P_MAILING_ADDRESS1 IN VARCHAR2,
    P_MAILING_ADDRESS2 IN VARCHAR2,
    P_MAILING_STATE IN VARCHAR2,
    P_MAILING_CITY IN VARCHAR2,
    P_MAILING_PINCODE IN VARCHAR2,
    P_PERMANENT_ADDRESS1 IN VARCHAR2,
    P_PERMANENT_ADDRESS2 IN VARCHAR2,
    P_PERMANENT_STATE IN VARCHAR2,
    P_PERMANENT_CITY IN VARCHAR2,
    P_PERMANENT_PINCODE IN VARCHAR2,
    P_NRI_ADDRESS IN VARCHAR2,
    P_FAX_VALUE IN VARCHAR2,
    P_AADHAR_VALUE IN NUMBER,
    P_EMAIL_VALUE IN VARCHAR2,
    P_EMAIL_OFFICIAL_VALUE IN VARCHAR2,
    P_PHONE_OFFICE_STD_VALUE IN VARCHAR2,
    P_PHONE_OFFICE_NUMBER_VALUE IN VARCHAR2,
    P_PHONE_RES_STD_VALUE IN VARCHAR2,
    P_PHONE_RES_NUMBER_VALUE IN VARCHAR2,
    P_MOBILE_NO_VALUE IN VARCHAR2,
    P_REFERENCE_NAME1_VALUE IN VARCHAR2,
    P_REFERENCE_NAME2_VALUE IN VARCHAR2,
    P_REFERENCE_NAME3_VALUE IN VARCHAR2,
    P_REFERENCE_NAME4_VALUE IN VARCHAR2,
    P_MOBILE_NO1_VALUE IN NUMBER,
    P_MOBILE_NO2_VALUE IN NUMBER,
    P_MOBILE_NO3_VALUE IN NUMBER,
    P_MOBILE_NO4_VALUE IN NUMBER,
    P_LOGGED_IN_USER IN VARCHAR2,
    P_GUEST_CODE in varchar2,
    P_EMP_RM IN NUMBER,
    P_EMP_SRC IN NUMBER,
    P_RESULT OUT SYS_REFCURSOR 
) AS

    DCT_FLAG                    NUMBER := 0;
    DCM_FLAG                    NUMBER :=0;
    DT_FLAG                     NUMBER;
    MyClientCode                VARCHAR2(50);  
    MyMainClientCode            VARCHAR2(50);  
    MySourceId                  VARCHAR2(8);  
    v_unique_client_code        VARCHAR2(50);
    v_check_duplicate           NUMBER;
    sql_query                   VARCHAR2(4000);

    V_GENERATED_CLIENT_CODE client_test.client_code%type;
    v_inv_code investor_master.inv_code%type;
    V_MYSOURCEID client_master.client_code%type;


    VCNT                        NUMBER; -- for inv pan validation
    CLIENT_MASTER_FLAG          NUMBER; -- Flag for CLIENT_MASTER validation
    CLIENT_TEST_FLAG            NUMBER; -- Flag for CLIENT_TEST validation


    -- Declare local variables
    VPAN                    NUMBER;
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
    ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10))));
    ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE_NO_VALUE);
    ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(P_EMAIL_VALUE));
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GUARDIAN_PERSON_PAN, 1, 10))));
  --  ISVALIDAADAHR := VALIDATEAADHAR(TRIM(P_AADHAR_VALUE));

    BEGIN -- Check for duplicate PAN globally
        SELECT count(PAN) 
        INTO VPAN 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10)));
            --AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8)) AND INV_CODE != TRIM(P_EXIST_CLIENT_CODE);
        EXCEPTION 
        WHEN NO_DATA_FOUND THEN VPAN := 0;
    END;


    -- get family sourcecode  V_SOURCE_CODE


    BEGIN  -- validate mobile no of family sourcecode 
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE MOBILE = P_MOBILE_NO_VALUE ;        
        EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
    END;


    BEGIN -- Check if the same email exists for a different family (SOURCE_CODE)
        SELECT COUNT(EMAIL) 
        INTO VEMAIL 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(EMAIL) = UPPER(P_EMAIL_VALUE);
        EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
    END;

    -- Check if the same guardian pan exists for a different family (SOURCE_CODE)
    BEGIN
        SELECT COUNT(G_PAN) 
        INTO VG_PAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(G_PAN) = UPPER(P_GUARDIAN_PERSON_PAN);
        EXCEPTION WHEN NO_DATA_FOUND THEN VG_PAN := 0;
    END;




    -- CHECK DUPLICATE AADHAR FLAG
    BEGIN
        SELECT COUNT(aadhar_card_no) 
        INTO V_AADHAR 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE aadhar_card_no = TRIM(SUBSTR(P_AADHAR_VALUE,1,12));
        EXCEPTION WHEN NO_DATA_FOUND THEN V_AADHAR := 0;
    END;


    -- Validate CLIENT_MASTER
    BEGIN
        SELECT nvl(COUNT(*),0)
        INTO CLIENT_MASTER_FLAG
        FROM tb_doc_upload 
        WHERE 
        common_id = P_DT_NUMBER and
        SUBSTR(inv_code, 1, 8) IN (SELECT CLIENT_CODE FROM CLIENT_MASTER);
        EXCEPTION 
        WHEN NO_DATA_FOUND THEN CLIENT_MASTER_FLAG:=0;
    END;

    -- Validate CLIENT_TEST
    BEGIN
        SELECT nvl(COUNT(*),0)
        INTO CLIENT_TEST_FLAG
        FROM tb_doc_upload 
        WHERE 
        common_id = P_DT_NUMBER and
        inv_code IN (SELECT CLIENT_CODEKYC FROM CLIENT_TEST);
        EXCEPTION WHEN NO_DATA_FOUND THEN CLIENT_TEST_FLAG:=0;
    END;



    -- Validate DT Number and return appropriate error messages
    IF TRIM(P_DT_NUMBER) IS NOT NULL THEN
        IF DT_FLAG = 0 THEN
            OPEN P_RESULT FOR 
            SELECT 'Please Enter Valid DT Number' AS message 
            FROM DUAL;
            RETURN;  -- Exit the procedure here
        END IF;
    ELSE
        OPEN P_RESULT FOR 
        SELECT 'Please enter DT number for new account' AS message 
        FROM DUAL;
        RETURN;  
    END IF;

    -- Validate Business RM Code  
    IF TRIM(P_RM_BUSINESS_CODE) IS NULL THEN
        OPEN P_RESULT FOR
            SELECT 'Please Enter Valid Business RM Code' AS message FROM DUAL;
        RETURN;
    END IF;

    -- Validate PAN and return appropriate error messages
    IF P_CLIENT_PAN IS NOT NULL THEN
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

    -- VALIDATE MOBILE AND RETURN INVALID, DUPLICATE
    IF P_MOBILE_NO_VALUE IS NOT NULL THEN
        IF ISVALIDMOBILE = 0 THEN
            OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)' AS message FROM DUAL;
            RETURN;    
        ELSIF ISVALIDMOBILE = 1  AND VMOBILE > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number' AS message FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- VALIDATE EMAIL AND RETURN INVALID, DUPLICATE
    IF P_EMAIL_VALUE IS NOT NULL THEN
        IF P_EMAIL_VALUE NOT IN ('NA', 'NOT AVAILABLE') THEN
            IF ISVALIDEMAIL = 0 THEN
                OPEN P_RESULT FOR SELECT 'Invalid Email' AS message FROM DUAL;
                RETURN;

            ELSIF ISVALIDEMAIL = 1  AND VEMAIL > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Email' AS message FROM DUAL;
            END IF;
        END IF;
    END IF ;

    -- VALIDATE GUARDIAN PAN, return an error message
    IF P_GUARDIAN_PERSON_PAN IS NOT NULL THEN 
        IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
            OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
            RETURN;

        ELSIF ISVALID_GPAN = 'Valid' and VG_PAN > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families)' AS message FROM DUAL;
            RETURN;
        END IF;
    end if;

    -- VALIDATE AADHAR NUMBER   
    IF P_AADHAR_VALUE IS NOT NULL or P_AADHAR_VALUE = 0 THEN
        IF V_AADHAR > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Aadhar Number' AS message FROM DUAL;
            RETURN;
        END IF ;
    END IF;


IF LENGTH(TRIM(P_EXIST_CLIENT_CODE)) >= 11 THEN
    -- Check for Duplicate Account (DCT_FLAG)
    SELECT COUNT(*)
    INTO DCT_FLAG
    FROM client_test
    WHERE client_codekyc = P_EXIST_CLIENT_CODE;

    -- If Account Exists, Return Message
    IF DCT_FLAG > 0 THEN
        UPDATE CLIENT_TEST
    SET
        CLIENTCODE_OLD = P_EXIST_CLIENT_CODE,
        business_code = P_RM_BUSINESS_CODE,
        OCC_ID = P_OCCUPATION,
        STATUS = P_STATUS_CAT,
        ACT_CAT = P_ACCOUNT_CAT,
        TITLE = P_SALUTATION1,
        CLIENT_NAME = P_ACCOUNT_NAME,
        TITLE_OTHER = P_SALUTATION2,
        OTHER = P_FATHER_NAME,
        otherS1 = P_ACCOUNT_OTHER,
        GENDER = P_GENDER,
        MARITAL_STATUS = P_MARITAL_STATUS,
        NATIONALITY = P_NATIONALITY,
        RESIDENT_NRI = P_RESIDENT_NRI,
        DOB = P_DOB,
        ANNUAL_INCOME = P_ANNUAL_INCOME,
        CLIENT_PAN = P_CLIENT_PAN,
        LEAD_TYPE= P_LEAD_TYPE,
        G_NAME = P_GUARDIAN_PERSON,
        G_NATIONALITY = P_GUARDIAN_PERSON_NATIONALITY,
        G_PAN = P_GUARDIAN_PERSON_PAN,
        ADD1 = P_MAILING_ADDRESS1,
        ADD2 = P_MAILING_ADDRESS2,
        STATE_ID = P_MAILING_STATE,
        CITY_ID = P_MAILING_CITY,
        PINCODE = P_MAILING_PINCODE,
        OVERSEAS_ADD = P_NRI_ADDRESS ,
        FAX = P_FAX_VALUE ,
    AADHAR_CARD_NO = P_AADHAR_VALUE,
    EMAIL = P_EMAIL_VALUE ,
    STD1 = NVL (SUBSTR(P_PHONE_OFFICE_STD_VALUE, 1, 5), NULL) ,
    TEL1 = P_PHONE_OFFICE_NUMBER_VALUE,
    STD2 = NVL (SUBSTR(P_PHONE_RES_STD_VALUE, 1, 5), NULL) ,
    TEL2 = P_PHONE_RES_NUMBER_VALUE ,
    MOBILE_NO = P_MOBILE_NO_VALUE,
    LOGGEDUSERID = P_LOGGED_IN_USER,
    MODIFY_DATE  = SYSDATE,
    MODIFY_USER = P_LOGGED_IN_USER
    WHERE client_codekyc =  Trim(P_EXIST_CLIENT_CODE);
        COMMIT;
        OPEN P_RESULT FOR
            SELECT 'Account Of This Main Investor Is Already Exist' AS message FROM DUAL;
        RETURN;
    ELSE      
        INSERT INTO client_test (
        client_code,
        clientcode_old,
        business_code,
        occ_id,
        status,
        act_cat,
        title,
        client_name,
        title_other,
        other,
        others1,
        gender,
        marital_status,
        nationality,
        resident_nri,
        dob,
        annual_income,
        client_pan,
        lead_type,
        g_name,
        g_nationality,
        g_pan,
        add1,
        add2,
        state_id,
        city_id,
        pincode,
        overseas_add,
        fax,
        aadhar_card_no,
        email,
        std1,
        tel1,
        std2,
        tel2,
        mobile_no,
        loggeduserid,
        timest,
        client_codekyc,
        source_code,
        main_code,
        BRANCH_CODE
        ) VALUES (
        NULL, -- Assuming CLIENT_CODE is auto-generated
        p_exist_client_code,
        p_rm_business_code,
        p_occupation,
        p_status_cat,
        p_account_cat,
        p_salutation1,
        p_account_name,
        p_salutation2,
        p_father_name,
        p_account_other,
        p_gender,
        p_marital_status,
        p_nationality,
        p_resident_nri,
        p_dob,
        p_annual_income,
        p_client_pan,
        p_lead_type,
        p_guardian_person,
        p_guardian_person_nationality,
        p_guardian_person_pan,
        p_mailing_address1,
        p_mailing_address2,
        p_mailing_state,
        p_mailing_city,
        p_mailing_pincode,
        p_nri_address,
        p_fax_value,
        p_aadhar_value,
        p_email_value,
        p_phone_office_std_value,
        p_phone_office_number_value,
        p_phone_res_std_value,
        p_phone_res_number_value,
        p_mobile_no_value,
        p_logged_in_user,
        sysdate,
        P_EXIST_CLIENT_CODE, --(SELECT concat(client_code, '001') FROM client_master WHERE pan = p_client_pan AND ROWNUM = 1 ),
        NULL,
        NULL,
        P_EMP_SRC
        ) RETURNING client_code INTO v_generated_client_code;
        COMMIT; 

        UPDATE CLIENT_MASTER CM
    SET 
        CM.per_state_id =   P_PERMANENT_STATE,
        CM.PER_PINCODE     =         P_PERMANENT_PINCODE, 
        CM.OFFICE_EMAIL        =         P_EMAIL_OFFICIAL_VALUE,  
        CM.PER_ADD1        =         P_PERMANENT_ADDRESS1,  
        CM.PER_ADD2        =         P_PERMANENT_ADDRESS2,  
        CM.REF_NAME1       =         P_REFERENCE_NAME1_VALUE,  
        CM.REF_NAME2       =         P_REFERENCE_NAME2_VALUE,  
        CM.REF_NAME3       =         P_REFERENCE_NAME3_VALUE,  
        CM.REF_NAME4       =         P_REFERENCE_NAME4_VALUE,  
        CM.REF_MOB1        =         P_MOBILE_NO1_VALUE,  
        CM.REF_MOB2        =         P_MOBILE_NO2_VALUE,  
        CM.REF_MOB3        =         P_MOBILE_NO3_VALUE,  
        CM.REF_MOB4        =         P_MOBILE_NO4_VALUE,  
        CM.CLIENT_TITLE        =         P_SALUTATION1,  
        CM.TITLE_FATHER_SPOUSE     =         P_SALUTATION2,  
        CM.CLIENT_LNAME        =         NULL,  
        CM.CATEGORY_ID     =         P_CLIENT_CAT,  
        CM.INVESTOR_CODE       =         1,  
        CM.GUEST_CD        =         NULL,  
        CM.CLIENT_NAME     =         P_ACCOUNT_NAME,  
        CM.FATHER_SPOUSE_NAME      =         P_FATHER_NAME,  
        CM.ADDRESS1        =         P_MAILING_ADDRESS1,  
        CM.ADDRESS2        =         P_MAILING_ADDRESS2,  
        CM.FAX     =         P_FAX_VALUE,  
        CM.EMAIL       =         P_EMAIL_VALUE,  
        CM.PAN     =         P_CLIENT_PAN,  
        CM.DOB     =         P_DOB,  
        CM.MOBILE      =         P_MOBILE_NO_VALUE,  
        CM.PINCODE     =         P_MAILING_PINCODE,  
        CM.GENDER      =         NVL (SUBSTR(P_GENDER, 1, 1), NULL),  
        CM.MARITAL_STATUS      =         P_MARITAL_STATUS,  
        CM.LEAD_SOURCE     =         P_LEAD_TYPE,  
        CM.CITY_NAME       =         NVL ((SELECT CITY_NAME FROM CITY_MASTER WHERE CITY_ID = P_MAILING_CITY ), NULL ),  
        CM.STD1        =         NVL (SUBSTR(P_PHONE_OFFICE_STD_VALUE, 1, 5), NULL),  
        CM.STD2        =         NVL (SUBSTR(P_PHONE_RES_STD_VALUE, 1, 5), NULL),  
        CM.PHONE1      =         P_PHONE_OFFICE_NUMBER_VALUE,  
        CM.PHONE2      =         P_PHONE_RES_NUMBER_VALUE,  
        CM.STATUS      =         P_STATUS_CAT,  
        CM.OCC_ID      =         P_OCCUPATION,  
        CM.CITY_ID     =         P_MAILING_CITY,  
        CM.PER_CITY_ID     =         P_PERMANENT_CITY,  
        CM.COMM_ID     =         NULL,  
        CM.COMM_INT_ID     =         NULL,  
        CM.KYC     =         NULL,  
        CM.SOURCEID        =         P_EMP_SRC,  
        CM.RM_CODE     =         P_EMP_RM,  
        CM.LOGGEDUSERID        =         P_LOGGED_IN_USER,  
        CM.ACT_CAT     =         P_ACCOUNT_CAT,  
        CM.MODIFY_DATE  = SYSDATE,
        CM.MODIFY_USER = P_LOGGED_IN_USER
    WHERE CM.CLIENT_CODE = SUBSTR(P_EXIST_CLIENT_CODE,1,8);
        COMMIT;    

        UPDATE investor_master IM
 SET
    IM.investor_title          =     p_salutation1,                             
    IM.branch_Code         =     p_emp_src,                       
    IM.investor_name           =     P_ACCOUNT_NAME,
            im.g_name = P_GUARDIAN_PERSON,
        im.g_pan = P_GUARDIAN_PERSON_PAN,
        im.mar_status = P_MARITAL_STATUS,
    IM.address1            =     p_mailing_address1,                    
    IM.address2            =     p_mailing_address1,                    
    IM.fax         =     p_fax_value,                  
    IM.email           =     p_email_value,                   
    IM.pan         =     p_client_pan,              
    IM.dob         =     p_dob,                  
    IM.mobile          =     p_mobile_no_value,                 
    IM.AADHAR_CARD_NO          =     p_aadhar_value,                   
    IM.pincode         =     p_mailing_pincode,                
    IM.gender          =     NVL (SUBSTR(p_gender, 1, 1), NULL),                 
    IM.phone1          =     p_phone_office_number_value,                 
    IM.phone2          =     p_phone_res_number_value,                 
    IM.status          =     p_status_cat,                
    IM.city_id         =     p_mailing_city,                
    IM.kyc         =     null,                     
    --IM.source_id           =     (select client_code from client_master where pan = p_client_pan),  
    IM.rm_code         =     p_emp_rm,                             
    IM.rel_id          =     null,                                
    IM.occupation_id           =     p_occupation,                                  
    IM.timest          =     SYSDATE,
    IM.loggeduserid            =     p_logged_in_user
    WHERE INV_CODE = P_EXIST_CLIENT_CODE;
        COMMIT;

        update client_test 
        set  main_code= v_generated_client_code,
        client_codekyc = P_EXIST_CLIENT_CODE
        where client_code = v_generated_client_code;

        OPEN P_RESULT FOR
            SELECT 'Account created successfully --> ' || v_generated_client_code  AS message FROM DUAL;
        RETURN;

    END IF;
END IF;


-- Check if txtClientCodeOld length is >= 11 and ends with "001"
IF LENGTH(P_EXIST_CLIENT_CODE) >= 11 AND SUBSTR(P_EXIST_CLIENT_CODE, -3) = '001' THEN
    -- Extract the first 8 characters as MySourceId
    MySourceId := SUBSTR(P_EXIST_CLIENT_CODE, 1, 8);

    -- Update CLIENT_MASTER for the main account
    UPDATE client_master
    SET kyc = 'YES'
    WHERE client_code = MySourceId;

    -- Update INVESTOR_MASTER
    UPDATE investor_master
    SET kyc = 'YES'
    WHERE inv_code = P_EXIST_CLIENT_CODE;

    -- Update CLIENT_TEST
    UPDATE client_test
    SET client_codekyc = P_EXIST_CLIENT_CODE
    WHERE client_code = (select client_code from client_test where client_pan = p_client_pan and client_name = p_account_name);

-- If txtClientCodeOld length is >= 11 and does NOT end with "001"
ELSIF LENGTH(P_EXIST_CLIENT_CODE) >= 11 AND SUBSTR(P_EXIST_CLIENT_CODE, -3) <> '001' THEN
    -- Extract the first 8 characters as MySourceId
    MySourceId := SUBSTR(P_EXIST_CLIENT_CODE, 1, 8);

    -- Update CLIENT_MASTER
    UPDATE client_master
    SET kyc = 'YES'
    WHERE client_code = MySourceId;

    -- Update INVESTOR_MASTER
    UPDATE investor_master
    SET kyc = 'YES'
    WHERE inv_code = P_EXIST_CLIENT_CODE;

    -- Update CLIENT_TEST
    UPDATE client_test
    SET client_codekyc = P_EXIST_CLIENT_CODE
    WHERE client_code = (select client_code from client_test where client_pan = p_client_pan and client_name = p_account_name);

END IF;

-- Commit the changes
COMMIT;

IF P_EXIST_CLIENT_CODE IS NULL OR P_EXIST_CLIENT_CODE = 0 THEN 
        INSERT INTO client_master (
        per_pincode, -- PER_PINCODE
        office_email, -- OFFICE_EMAIL
        per_add1, -- PER_ADD1
        per_add2, -- PER_ADD2
        ref_name1, -- REF_NAME1
        ref_name2, -- REF_NAME2
        ref_name3, -- REF_NAME3
        ref_name4, -- REF_NAME4
        ref_mob1, -- REF_MOB1
        ref_mob2, -- REF_MOB2
        ref_mob3, -- REF_MOB3
        ref_mob4, -- REF_MOB4
        client_title, -- client_title
        title_father_spouse, -- TITLE_FATHER_SPOUSE
        client_lname, -- CLIENT_LNAME
        category_id, -- category_id
        investor_code, -- investor_code
        guest_cd, -- GUEST_CD
        client_name, -- client_name
        father_spouse_name, -- FATHER_SPOUSE_NAME
        address1, -- address1
        address2, -- address2
        fax, -- fax
        email, -- email
        pan, -- pan
        dob, -- dob
        mobile, -- mobile
        pincode, -- pincode
        gender, -- gender
        marital_status, -- MARITAL_STATUS
        lead_source, -- LEAD_SOURCE
        city_name, -- city_name
        std1, -- std1
        std2, -- std2
        phone1, -- phone1
        phone2, -- phone2
        status, -- status
        occ_id, -- occ_id
        city_id, -- city_id
        per_city_id, -- per_city_id
        per_state_id,
        comm_id, -- comm_id
        comm_int_id, -- comm_int_id
        kyc, -- kyc
        sourceid, -- sourceid
        rm_code, -- rm_code
        loggeduserid, -- loggeduserid
        act_cat, -- act_cat
        timest
        ) VALUES (
        UPPER(p_permanent_pincode), -- PER_PINCODE
        UPPER(p_email_official_value), -- OFFICE_EMAIL
        UPPER(p_permanent_address1), -- PER_ADD1
        UPPER(p_permanent_address2), -- PER_ADD2
        UPPER(p_reference_name1_value), -- REF_NAME1
        UPPER(p_reference_name2_value), -- REF_NAME2
        UPPER(p_reference_name3_value), -- REF_NAME3
        UPPER(p_reference_name4_value), -- REF_NAME4
        UPPER(p_mobile_no1_value), -- REF_MOB1
        UPPER(p_mobile_no2_value), -- REF_MOB2
        UPPER(p_mobile_no3_value), -- REF_MOB3
        UPPER(p_mobile_no4_value), -- REF_MOB4
        UPPER(p_salutation1), -- client_title
        UPPER(p_salutation2), -- TITLE_FATHER_SPOUSE
        NULL, -- CLIENT_LNAME
        UPPER(p_client_cat), -- category_id
        1, -- investor_code
        UPPER(P_GUEST_CODE), -- GUEST_CD
        UPPER(p_account_name), -- client_name
        UPPER(p_father_name), -- FATHER_SPOUSE_NAME
        UPPER(p_mailing_address1), -- address1
        UPPER(p_mailing_address2), -- address2
        UPPER(p_fax_value), -- fax
        UPPER(p_email_value), -- email
        UPPER(p_client_pan), -- pan
        UPPER(p_dob), -- dob
        UPPER(p_mobile_no_value), -- mobile
        UPPER(p_mailing_pincode), -- pincode
        UPPER(nvl(substr(p_gender, 1, 1), NULL)), -- gender
        UPPER(p_marital_status), -- MARITAL_STATUS
        UPPER(p_lead_type), -- LEAD_SOURCE
        UPPER(nvl(( SELECT city_name FROM city_master WHERE city_id = p_mailing_city), NULL)), -- city_name
        UPPER(nvl(substr(p_phone_office_std_value, 1, 5),NULL)), -- std1
        UPPER(nvl(substr(p_phone_res_std_value, 1, 5), NULL)), -- std2
        UPPER(p_phone_office_number_value), -- phone1
        UPPER(p_phone_res_number_value), -- phone2
        UPPER(P_TAX_STATUS), -- status
        UPPER(p_occupation), -- occ_id
        UPPER(p_mailing_city), -- city_id
        UPPER(p_permanent_city), -- per_city_id
        UPPER(p_permanent_state),
        NULL, -- comm_id
        NULL, -- comm_int_id
        '', -- kyc
        UPPER(p_emp_src), -- sourceid
        UPPER(p_emp_rm), -- rm_code
        UPPER(p_logged_in_user), -- loggeduserid
        UPPER(p_account_cat), -- act_cat
        sysdate
        );

        SELECT MAX(client_code) INTO V_MYSOURCEID  
        FROM client_master WHERE 
            UPPER(TRIM(client_name)) = UPPER(TRIM(p_account_name)) OR UPPER(TRIM(p_account_name)) IS NULL
            AND UPPER(TRIM(pan)) = UPPER(TRIM(P_CLIENT_PAN)) OR UPPER(TRIM(P_CLIENT_PAN)) IS NULL
            AND UPPER(TRIM(ADDRESS1)) = UPPER(TRIM(P_MAILING_ADDRESS1)) OR UPPER(TRIM(P_MAILING_ADDRESS1)) IS NULL
            AND UPPER(TRIM(ADDRESS2)) = UPPER(TRIM(P_MAILING_ADDRESS2)) OR UPPER(TRIM(P_MAILING_ADDRESS2)) IS NULL
            AND UPPER(TRIM(MOBILE)) = UPPER(TRIM(P_MOBILE_NO_VALUE)) OR UPPER(TRIM(P_MOBILE_NO_VALUE)) IS NULL
            AND UPPER(TRIM(SOURCEID)) = UPPER(TRIM(P_EMP_SRC)) OR UPPER(TRIM(P_EMP_SRC)) IS NULL
            AND UPPER(TRIM(RM_CODE)) = UPPER(TRIM(P_EMP_RM)) OR UPPER(TRIM(P_EMP_RM)) IS NULL;
            COMMIT;


        INSERT INTO client_test (
        client_code,
        clientcode_old,
        business_code,
        occ_id,
        status,
        act_cat,
        title,
        client_name,
        TITLE_FATHER_SPOUSE,
        FATHER_SPOUSE_NAME,
        others1,
        gender,
        marital_status,
        nationality,
        resident_nri,
        dob,
        annual_income,
        client_pan,
        lead_type,
        g_name,
        g_nationality,
        g_pan,
        add1,
        add2,
        state_id,
        city_id,
        pincode,
        overseas_add,
        fax,
        aadhar_card_no,
        email,
        std1,
        tel1,
        std2,
        tel2,
        mobile_no,
        loggeduserid,
        timest,
        client_codekyc,
        source_code,
        main_code,
        BRANCH_CODE
        ) VALUES (
        NULL, --  CLIENT_CODE is auto-generated
        UPPER(p_exist_client_code),
        UPPER(p_rm_business_code),
        UPPER(p_occupation),
        UPPER(p_status_cat),
        UPPER(p_account_cat),
        UPPER(p_salutation1),
        UPPER(p_account_name),
        UPPER(p_salutation2),
        UPPER(p_father_name),
        UPPER(p_account_other),
        UPPER(p_gender),
        UPPER(p_marital_status),
        UPPER(p_nationality),
        UPPER(p_resident_nri),
        UPPER(p_dob),
        UPPER(p_annual_income),
        UPPER(p_client_pan),
        UPPER(p_lead_type),
        UPPER(p_guardian_person),
        UPPER(p_guardian_person_nationality),
        UPPER(p_guardian_person_pan),
        UPPER(p_mailing_address1),
        UPPER(p_mailing_address2),
        UPPER(p_mailing_state),
        UPPER(p_mailing_city),
        UPPER(p_mailing_pincode),
        UPPER(p_nri_address),
        UPPER(p_fax_value),
        UPPER(p_aadhar_value),
        UPPER(p_email_value),
        UPPER(p_phone_office_std_value),
        UPPER(p_phone_office_number_value),
        UPPER(p_phone_res_std_value),
        UPPER(p_phone_res_number_value),
        UPPER(p_mobile_no_value),
        UPPER(p_logged_in_user),
        sysdate,
        P_EXIST_CLIENT_CODE, --(SELECT concat(client_code, '001') FROM client_master WHERE pan = p_client_pan AND ROWNUM = 1 ),
        NULL,
        NULL,
        P_EMP_SRC
        ) RETURNING client_code INTO v_generated_client_code;
        COMMIT; 

        INSERT INTO investor_master (
            inv_code,
            investor_title,       -- Investor Title
            branch_code,          -- Branch Code
            investor_name,        -- Investor Name
            g_name,
            g_pan,
            mar_status,
            address1,             -- Address Line 1
            address2,             -- Address Line 2
            fax,                  -- Fax Number
            email,                -- Email Address
            pan,                  -- PAN Number
            dob,                  -- Date of Birth
            mobile,               -- Mobile Number
            aadhar_card_no,       -- Aadhar Card Number
            pincode,              -- Pincode
            gender,               -- Gender
            phone1,               -- Primary Phone Number
            phone2,               -- Secondary Phone Number
            status,               -- Status
            city_id,              -- City ID
            kyc,                  -- KYC Status
            source_id,            -- Source ID
            rm_code,              -- Relationship Manager Code
            rel_id,               -- Relationship ID
            occupation_id,         -- Occupation ID
            timest,
            loggeduserid
        ) VALUES (
            NULL,
            UPPER(p_salutation1),               -- investor_title
            UPPER(p_emp_src),                -- branch_Code
            UPPER(p_account_name),               -- investor_name
            UPPER(P_GUARDIAN_PERSON),
            UPPER(P_GUARDIAN_PERSON_PAN),
            UPPER(p_marital_status), 
            UPPER(p_mailing_address1),               -- address1
            UPPER(p_mailing_address1),               -- address2
            UPPER(p_fax_value),                  -- fax
            UPPER(p_email_value),      -- email
            UPPER(p_client_pan),            -- pan
            UPPER(p_dob),                 -- dob
            UPPER(p_mobile_no_value),              -- mobile
            UPPER(p_aadhar_value),            -- AADHAR_CARD_NO
            UPPER(p_mailing_pincode),               -- pincode
            UPPER(nvl)(substr(p_gender, 1, 1), NULL),                  -- gender
            UPPER(p_phone_office_number_value),                 -- phone1
            UPPER(p_phone_res_number_value),                 -- phone2
            UPPER(p_status_cat),                 -- status
            UPPER(p_mailing_city),                 -- city_id
            UPPER(NULL),                 -- kyc
            UPPER(V_MYSOURCEID),             -- source_id
            UPPER(p_emp_rm),                 -- rm_code
            UPPER(NULL),                   -- rel_id
            UPPER(p_occupation),                    -- occupation_id
            UPPER(sysdate),
            UPPER(p_logged_in_user)
        );
        COMMIT;

         select max(inv_code) into v_inv_code
         from investor_master 
         where source_id= V_MYSOURCEID;

        update client_test 
        set  main_code= v_generated_client_code,
        client_codekyc = ( select max(inv_code) from investor_master where source_id= V_MYSOURCEID)
        where client_code = v_generated_client_code;

        update tb_doc_upload 
        set inv_code = v_inv_code
        where common_id = P_DT_NUMBER;

        update client_test
        set guest_code = P_GUEST_CODE  where client_codekyc = v_inv_code;

        update client_master
        set guest_cd = P_GUEST_CODE where client_code = substr(v_inv_code,1,8);
        
        update investor_master
        set mar_status = (select marital_status from client_master where client_code = substr(v_inv_code,1,8) and rownum = 1 );

        -- If no duplicates found, return the generated client code
        OPEN P_RESULT FOR
            SELECT 'Data Inserted successfully --> ' || V_MYSOURCEID || ' | '  || 
            v_generated_client_code || ' | ' || v_inv_code AS message FROM DUAL;
        RETURN;
END IF;



END PSM_AO_INSERT_CLIENT_DATA;