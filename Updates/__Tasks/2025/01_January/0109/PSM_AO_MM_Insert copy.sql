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

-- Match CLIENT_CODE type
BEGIN

    

    
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

    SELECT
        MAX(inv_code) 
    INTO nextinvkyc
    FROM
        investor_master
    WHERE
        source_id = substr(p_exist_client_code, 1, 8);

    UPDATE client_test
    SET
        client_codekyc = nextinvkyc
    WHERE
        client_code = v_generated_client_code;

    UPDATE investor_master
    SET
        inv_code = nextinvkyc
    WHERE
        inv_code = v_generated_inv_code;

    OPEN p_result FOR 
SELECT 'Successfully member added: client code --> ' || v_generated_client_code || ' | Inv Code --> ' || nextinvkyc AS message
FROM dual;


    COMMIT;
END PSM_AO_MM_Insert;