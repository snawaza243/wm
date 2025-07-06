CREATE OR REPLACE PROCEDURE WEALTHMAKER.psm_am_insert_agent_master(

    p_loggedinuser          IN VARCHAR2,
    p_empanelmenttype       IN VARCHAR2,
    p_agentcode             IN VARCHAR2,
    p_existcode             IN VARCHAR2,
    p_title                 IN VARCHAR2,F
    p_agentname             IN VARCHAR2,
    p_gender                IN VARCHAR2,
    p_sourceid              IN VARCHAR2,
    p_rmcode                IN VARCHAR2,
    p_address1              IN VARCHAR2,
    p_address2              IN VARCHAR2,
    p_address3              IN VARCHAR2,
    p_cityid                IN VARCHAR2,
    p_locationid            IN VARCHAR2,
    p_mobile                IN NUMBER,
    p_pin                   IN NUMBER,
    p_fax                   IN VARCHAR2,
    p_contactper            IN VARCHAR2,
    p_email                 IN VARCHAR2,
    p_tds                   IN NUMBER,
    p_associatetype         IN VARCHAR2,
    p_associatetypecategory IN VARCHAR2,
    p_contactpersonemailid  IN VARCHAR2,
    p_empanelmentdate       IN DATE,
    p_phone                 IN VARCHAR2,
    p_remarks               IN VARCHAR2,
    p_superana              IN VARCHAR2,
    p_onlinesubscriptioncheck       IN VARCHAR2,
    p_offlineplatformblock          IN VARCHAR2,
    p_onlineplatformblock           IN VARCHAR2,
    p_onlineplatformblockremark     IN VARCHAR2,
    p_offlineplatformblockremark    IN VARCHAR2,
    p_auditdate             IN DATE,
    p_auditcheck            IN VARCHAR2,
    p_paymentmodeid         IN NUMBER,
    p_accounttypeid         IN VARCHAR2,
    p_accno                 IN VARCHAR2,
    p_affectedfrom          IN DATE,
    p_bankid                IN VARCHAR2,
    p_bankcityid            IN VARCHAR2,
    p_brankbranchid         IN VARCHAR2,
    p_smsflag               IN VARCHAR2,
    p_gstinno               IN VARCHAR2,
    p_dob                   IN DATE,
    p_agenttype             IN VARCHAR2,
    p_pan                   IN VARCHAR2,
    p_dist                  IN VARCHAR2,
    p_aadharcardno          IN VARCHAR2,
    p_pospmarking           IN VARCHAR2,
    p_posptype              IN VARCHAR2,
    p_pospnoli              IN VARCHAR2,
    p_pospnogi              IN VARCHAR2,
    p_pospcertifiedonli     IN DATE,
    p_pospvalidtillli       IN DATE,
    p_pospcertifiedongi     IN DATE,
    p_pospvalidtillgi       IN DATE,
    p_verifiedstatus        IN VARCHAR2,
    p_neftbankname          IN VARCHAR2,
    p_neftbranch            IN VARCHAR2,
    p_neftifsccode          IN VARCHAR2,
    p_neftname              IN VARCHAR2,
    p_certpassed            IN VARCHAR2,
    p_certexams             IN VARCHAR2,
    p_certregno             IN VARCHAR2,
    p_res_add_1             IN VARCHAR2,
    p_res_add_2             IN VARCHAR2,
    p_res_add_state         IN VARCHAR2,
    p_res_add_city          IN VARCHAR2,
    p_res_add_pincode       IN VARCHAR2,
    p_dt_number             IN VARCHAR2,
    
    p_result OUT SYS_REFCURSOR
) AS
    agent_flag NUMBER := 0;
    pan_flag NUMBER := 0;
    dt_flag NUMBER := 0;
    new_agent_code VARCHAR2(2000);

    error_message           VARCHAR2(4000);
    v_aadhar_exists         NUMBER      :=0; 
    vpan                    NUMBER      :=0;
    vmobile                 NUMBER      :=0;
    vemail                  NUMBER      :=0;
    vg_pan                  NUMBER      :=0;
    isvalidpan              VARCHAR2(10);
    isvalid_gpan            VARCHAR2(10);
    isvalid_aadhaar         VARCHAR2(10);

    isvalidmobile           NUMBER      :=0;
    isvalidemail            NUMBER      :=0;
    v_message               VARCHAR2(4000); 

    new_aadhar_enc          VARCHAR2(200)   :=NULL;  
    v_source_code           VARCHAR2(20); 
    isaccess                NUMBER          :=0;
    v_bank_branch_name      VARCHAR2(100);

BEGIN
    -- Validate the PAN, Mobile, and Email formats first
    isvalidpan      := validatepan(UPPER(TRIM(substr(p_pan, 1, 10))));
    isvalidmobile   := validate_mobile(p_mobile);
    isvalidemail    := validate_email(UPPER(p_email));


 IF p_brankbranchid IN ( 'Select Bank Branch', '') THEN
        v_bank_branch_name := NULL;
ELSE
    v_bank_branch_name  :=p_brankbranchid;
END IF;


    BEGIN -- Check ACCESS 
        SELECT COUNT(role_id)
        INTO isaccess
        FROM wealthmaker.userdetails_ji
        WHERE TRIM(login_id) = TRIM(p_loggedinuser) 
        AND role_id = '212'; -- FOR PUNCHING 
        EXCEPTION WHEN no_data_found THEN isaccess := 0;
    END; 

    BEGIN -- Check for duplicate aadhar 
        SELECT COUNT(aadhar_card_no)
        INTO v_aadhar_exists
        FROM agent_master
        WHERE TRIM(aadhar_card_no) = TRIM(p_aadharcardno);
        EXCEPTION WHEN no_data_found THEN v_aadhar_exists := 0;
    END;

    BEGIN -- Check for duplicate PAN 
        SELECT COUNT(pan) 
        INTO vpan FROM wealthmaker.agent_master
        WHERE UPPER(TRIM(substr(pan, 1, 10))) = UPPER(TRIM(substr(p_pan, 1, 10)));
        EXCEPTION WHEN no_data_found THEN vpan := 0;
    END;

    BEGIN -- validate mobile no 
        SELECT COUNT(mobile) 
        INTO vmobile 
        FROM wealthmaker.agent_master
        WHERE TRIM(mobile) = TRIM(p_mobile) ;
        EXCEPTION WHEN no_data_found THEN vmobile := 0;
    END;

    BEGIN -- validate  email 
        SELECT COUNT(email) 
        INTO vemail 
        FROM wealthmaker.agent_master
        WHERE UPPER(email) = UPPER(p_email) ;
        EXCEPTION WHEN no_data_found THEN vemail := 0;
    END;

    BEGIN -- Check for duplicate aadhar 
        SELECT COUNT(aadhar_card_no)
        INTO v_aadhar_exists
        FROM agent_master
        WHERE (aadhar_card_no) = user_log(p_aadharcardno);
        EXCEPTION WHEN no_data_found THEN v_aadhar_exists := 0;
    END;





    --IF ISACCESS = 0 THEN OPEN P_RESULT FOR SELECT 'Access: Only Punching Team Can Punch!' AS MESSAGE FROM DUAL; RETURN; end if;

    IF isvalidpan = 'InValid' OR isvalidpan = 'Miss' THEN
        OPEN p_result FOR SELECT 'Invalid PAN (e.g. AAAAA9999A)' AS message FROM dual;
        RETURN;
    END IF;

    IF isvalidpan = 'Valid' AND vpan > 0 THEN
        OPEN p_result FOR SELECT 'Duplicate PAN'AS message FROM dual;
        RETURN;
    END IF;

    IF isvalidmobile = 0 THEN
        OPEN p_result FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)'AS message FROM dual;
        RETURN;
    END IF;

    IF isvalidmobile = 1  AND vmobile > 0 THEN
         OPEN p_result FOR SELECT 'Duplicate Mobile Number'AS message FROM dual;
        RETURN;
    END IF;


    IF isvalidemail = 0 THEN
        OPEN p_result FOR SELECT 'Invalid Email'  AS message FROM dual;
        RETURN;
     END IF;   


    IF isvalidemail = 1  AND vemail > 0 THEN
        OPEN p_result FOR SELECT 'Duplicate Email'  AS message FROM dual;
        RETURN;
    END IF;

    IF p_aadharcardno IS NOT NULL THEN
        isvalid_aadhaar := psm_validateaadhaar(TRIM(p_aadharcardno));

        IF isvalid_aadhaar = 'Invalid' OR isvalid_aadhaar = 'Miss' THEN 
            OPEN p_result FOR SELECT 'Invalid Aadhaar Number (Must be 12-digit numeric)' AS message FROM dual; 
            RETURN;    

        ELSIF isvalid_aadhaar = 'Valid' AND v_aadhar_exists > 0 THEN 

                OPEN p_result FOR SELECT 'Duplicate Aadhaar Number' AS message FROM dual; 
                RETURN;
        END IF;
    END IF;

    IF p_aadharcardno IS NOT NULL THEN
        isvalid_aadhaar := psm_validateaadhaar(TRIM(p_aadharcardno));

        IF isvalid_aadhaar = 'Invalid' OR isvalid_aadhaar = 'Miss' THEN 
            OPEN p_result FOR SELECT 'Invalid Aadhaar Number (Must be 12-digit numeric)' FROM dual; 
            RETURN;    

        ELSIF isvalid_aadhaar = 'Valid' THEN
            SELECT COUNT(aadhar_card_no)
            INTO v_aadhar_exists
            FROM agent_master
            WHERE (aadhar_card_no) = user_log(p_aadharcardno);

            IF v_aadhar_exists > 0 THEN 
                OPEN p_result FOR SELECT 'Duplicate Aadhaar Number' AS message FROM dual; 
                RETURN;
            END IF;
        END IF;

    END IF;

    IF p_aadharcardno IS NOT NULL THEN
        SELECT user_log(p_aadharcardno) INTO new_aadhar_enc FROM dual;
    END IF;


BEGIN

    INSERT INTO agent_master (
        loggeduserid,               paidflag,               agent_name,               sourceid,               rm_code,              address1,             address2,         address3,            
        city_id,                    location_id,            mobile,                   pincode,                fax,                  contactper,           email,   
        tds  ,                      sub_broker_type ,       category_id,              cpemailid ,             timest ,              phone     ,           remark     ,            
        master_ana,                 online_subsciption ,    online_block_agent,       block_agent,            online_block_remark,  offline_block_remark, ana_auditdate,
        ana_audit,                  paymentmodeid,          acctypeid,                accno,                  affectedfrom,         bankid,               city_name,            
        bank_branch_name /*BANK_BRANCHID*/,              sms_flag,               gstin_no,                 dob,                    statusid /*AGENT_TYPE*/,           pan,                  dist,             
        aadhar_card_no,             posp_marking ,          posp_type ,               posp_no_li,             posp_no_gi,           posp_certified_on_li,  posp_valid_till_li,          
        posp_certified_on_gi,       posp_valid_till_gi,     verified_status ,         /*NEFT_BANK_NAME,         BANK_BRANCH_NAME,     IFSC_CODE,            NAME_IN_BANK,*/           
        amficert,                   amfiextypeid,           amfiid,                   
        agent_master.r_address1,  agent_master.r_address2,  agent_master.r_state_name, agent_master.r_city_name, agent_master.r_pincode, doc_id, title, gender

    ) VALUES (
      p_loggedinuser,              p_empanelmenttype,       UPPER(p_agentname),       p_sourceid,             p_rmcode,             UPPER(p_address1),            UPPER(p_address2),     UPPER(p_address3),
      p_cityid,                    p_locationid,            p_mobile,                 p_pin,                  p_fax,                UPPER(p_contactper),          p_email,          
      p_tds,                       p_associatetype,         p_associatetypecategory,  p_contactpersonemailid, sysdate,              p_phone,                p_remarks,      
      p_superana,                  p_onlinesubscriptioncheck, p_offlineplatformblock, p_onlineplatformblock, p_onlineplatformblockremark, p_offlineplatformblockremark, p_auditdate,   
      p_auditcheck,                p_paymentmodeid,         p_accounttypeid,          p_accno,                p_affectedfrom,       p_bankid,               p_bankcityid,   
      v_bank_branch_name, /*P_BRANKBRANCHID,*/             p_smsflag,               p_gstinno,                p_dob,                  p_agenttype,          p_pan,                  p_dist,     
      new_aadhar_enc,        p_pospmarking,           p_posptype,               p_pospnoli,             p_pospnogi,           p_pospcertifiedonli,    p_pospvalidtillli,     
      p_pospcertifiedongi,         p_pospvalidtillgi,       p_verifiedstatus,       /*  P_NEFTBANKNAME,         P_NEFTBRANCH,         P_NEFTIFSCCODE,         P_NEFTNAME,  */ 
      p_certpassed,                p_certexams,             p_certregno,              
      UPPER(p_res_add_1),          UPPER(p_res_add_2),      UPPER(p_res_add_state),   UPPER(p_res_add_city),   TRIM(p_res_add_pincode), TRIM(p_dt_number), p_title, p_gender
    ) RETURNING agent_code INTO new_agent_code;-- Retrieve the new agent code

    COMMIT;

    UPDATE agent_master 
    SET doc_id =  TRIM(p_dt_number)
    WHERE UPPER(agent_name) = UPPER(p_agentname)
    AND mobile = p_mobile
    AND UPPER(pan) = UPPER(p_pan);

    --UPDATE tb_doc_upload SET punching_flag = '1' WHERE common_id = p_dt_number AND tran_type = 'ANA';

    OPEN p_result FOR
           SELECT 'Agent created successfully with EXIST_CODE: ' || (SELECT exist_code FROM agent_master WHERE agent_code = new_agent_code) || ' and ' || 'AGENT_CODE: ' || new_agent_code AS message FROM dual;
    RETURN;

    EXCEPTION
        WHEN OTHERS THEN
            error_message := 'Defined Error: ' || sqlerrm;

            OPEN p_result FOR
                SELECT error_message AS message FROM dual;
            RETURN;
    END;
END psm_am_insert_agent_master;
/
