create or replace PROCEDURE             PSM_AM_INSERT_AGENT_MASTER(

    P_LOGGEDINUSER IN VARCHAR2,
    P_EMPANELMENTTYPE IN VARCHAR2,
    P_AGENTCODE IN VARCHAR2,
    P_EXISTCODE IN VARCHAR2,
    P_TITLE         IN VARCHAR2,
    P_AGENTNAME IN VARCHAR2,
    P_GENDER                IN VARCHAR2,
    P_SOURCEID IN VARCHAR2,
    P_RMCODE IN VARCHAR2,
    P_ADDRESS1 IN VARCHAR2,
    P_ADDRESS2 IN VARCHAR2,
    P_ADDRESS3                  IN VARCHAR2,
    P_CITYID IN VARCHAR2,
    P_LOCATIONID IN VARCHAR2,
    P_MOBILE IN NUMBER,
    P_PIN IN NUMBER,
    P_FAX IN VARCHAR2,
    P_CONTACTPER IN VARCHAR2,
    P_EMAIL IN VARCHAR2,
    P_TDS IN NUMBER,
    P_ASSOCIATETYPE IN VARCHAR2,
    P_ASSOCIATETYPECATEGORY IN VARCHAR2,
    P_CONTACTPERSONEMAILID IN VARCHAR2,
    P_EMPANELMENTDATE IN DATE,
    P_PHONE IN VARCHAR2,
    P_REMARKS IN VARCHAR2,
    P_SUPERANA IN VARCHAR2,
    P_ONLINESUBSCRIPTIONCHECK IN VARCHAR2,
    P_OFFLINEPLATFORMBLOCK IN VARCHAR2,
    P_ONLINEPLATFORMBLOCK IN VARCHAR2,
    P_ONLINEPLATFORMBLOCKREMARK IN VARCHAR2,
    P_OFFLINEPLATFORMBLOCKREMARK IN VARCHAR2,
    P_AUDITDATE IN DATE,
    P_AUDITCHECK IN VARCHAR2,
    P_PAYMENTMODEID IN NUMBER,
    P_ACCOUNTTYPEID IN VARCHAR2,
    P_ACCNO IN VARCHAR2,
    P_AFFECTEDFROM IN DATE,
    P_BANKID IN VARCHAR2,
    P_BANKCITYID IN VARCHAR2,
    P_BRANKBRANCHID IN VARCHAR2,
    P_SMSFLAG IN VARCHAR2,
    P_GSTINNO IN VARCHAR2,
    P_DOB IN DATE,
    P_AGENTTYPE IN VARCHAR2,
    P_PAN IN VARCHAR2,
    P_DIST IN VARCHAR2,
    P_AADHARCARDNO IN VARCHAR2,
    P_POSPMARKING IN VARCHAR2,
    P_POSPTYPE IN VARCHAR2,
    P_POSPNOLI IN VARCHAR2,
    P_POSPNOGI IN VARCHAR2,
    P_POSPCERTIFIEDONLI IN DATE,
    P_POSPVALIDTILLLI IN DATE,
    P_POSPCERTIFIEDONGI IN DATE,
    P_POSPVALIDTILLGI IN DATE,
    P_VERIFIEDSTATUS IN VARCHAR2,
    P_NEFTBANKNAME IN VARCHAR2,
    P_NEFTBRANCH IN VARCHAR2,
    P_NEFTIFSCCODE IN VARCHAR2,
    P_NEFTNAME IN VARCHAR2,
    P_CERTPASSED IN VARCHAR2,
    P_CERTEXAMS IN VARCHAR2,
    P_CERTREGNO IN VARCHAR2,
    P_RES_ADD_1 IN VARCHAR2,
    P_RES_ADD_2 IN VARCHAR2,
    P_RES_ADD_STATE IN VARCHAR2,
    P_RES_ADD_CITY IN VARCHAR2,
    P_RES_ADD_PINCODE IN VARCHAR2,
    P_DT_NUMBER IN VARCHAR2,
    
    P_RESULT OUT SYS_REFCURSOR
) AS
    AGENT_FLAG NUMBER := 0;
    PAN_FLAG NUMBER := 0;
    DT_FLAG NUMBER := 0;
    NEW_AGENT_CODE VARCHAR2(2000);


    ERROR_MESSAGE VARCHAR2(4000);


    V_AADHAR_EXISTS         NUMBER :=0; 
    VPAN                    NUMBER :=0;
    VMOBILE                 NUMBER :=0;
    VEMAIL                  NUMBER :=0;
    VG_PAN                  NUMBER :=0;
    ISVALIDPAN              VARCHAR2(10);
    ISVALID_GPAN            VARCHAR2(10);

    ISVALIDMOBILE           NUMBER :=0;
    ISVALIDEMAIL            NUMBER :=0;
    v_message               VARCHAR2(4000);  
    V_SOURCE_CODE           VARCHAR2(20); 
    ISACCESS                NUMBER:=0;

    v_bank_branch_name      VARCHAR2(100);

BEGIN
    -- Validate the PAN, Mobile, and Email formats first
    ISVALIDPAN      := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_PAN, 1, 10))));
    ISVALIDMOBILE   := VALIDATE_MOBILE(P_MOBILE);
    ISVALIDEMAIL    := VALIDATE_EMAIL(UPPER(P_EMAIL));


 IF P_BRANKBRANCHID IN ( 'Select Bank Branch', '') THEN
        v_bank_branch_name := NULL;
ELSE
    v_bank_branch_name  :=P_BRANKBRANCHID;
END IF;


    BEGIN -- Check ACCESS 
        SELECT COUNT(ROLE_ID)
        INTO ISACCESS
        FROM WEALTHMAKER.USERDETAILS_JI
        WHERE TRIM(LOGIN_ID) = TRIM(P_LOGGEDINUSER) 
        AND ROLE_ID = '212'; -- FOR PUNCHING 
        EXCEPTION WHEN NO_DATA_FOUND THEN ISACCESS := 0;
    END; 

    BEGIN -- Check for duplicate aadhar 
        SELECT COUNT(AADHAR_CARD_NO)
        INTO V_AADHAR_EXISTS
        FROM AGENT_MASTER
        WHERE TRIM(AADHAR_CARD_NO) = TRIM(P_AADHARCARDNO);
        EXCEPTION WHEN NO_DATA_FOUND THEN V_AADHAR_EXISTS := 0;
    END;

    BEGIN -- Check for duplicate PAN 
        SELECT count(PAN) 
        INTO VPAN FROM WEALTHMAKER.AGENT_MASTER
        WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_PAN, 1, 10)));
        EXCEPTION WHEN NO_DATA_FOUND THEN VPAN := 0;
    END;

    BEGIN -- validate mobile no 
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.AGENT_MASTER
        WHERE TRIM(MOBILE) = TRIM(P_MOBILE) ;
        EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
    END;

    BEGIN -- validate  email 
        SELECT COUNT(EMAIL) 
        INTO VEMAIL 
        FROM WEALTHMAKER.AGENT_MASTER
        WHERE UPPER(EMAIL) = UPPER(P_EMAIL) ;
        EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
    END;



    IF ISACCESS = 0 THEN OPEN P_RESULT FOR SELECT 'Access: Only Punching Team Can Punch!' AS MESSAGE FROM DUAL; RETURN; end if;

    IF ISVALIDPAN = 'InValid' OR ISVALIDPAN = 'Miss' THEN
        OPEN P_RESULT FOR SELECT 'Invalid PAN (e.g. AAAAA9999A)' AS MESSAGE FROM DUAL;
        RETURN;
    end if;

    IF ISVALIDPAN = 'Valid' and VPAN > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate PAN'AS MESSAGE FROM DUAL;
        RETURN;
    end if;

    IF ISVALIDMOBILE = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)'AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    IF ISVALIDMOBILE = 1  AND VMOBILE > 0 THEN
         OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number'AS MESSAGE FROM DUAL;
        RETURN;
    END IF;


    IF ISVALIDEMAIL = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Email'  AS MESSAGE FROM DUAL;
        RETURN;
     end if;   


    IF ISVALIDEMAIL = 1  AND VEMAIL > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Email'  AS MESSAGE FROM DUAL;
        return;
    END IF;


begin

    INSERT INTO AGENT_MASTER (
        loggeduserid,               PAIDFLAG,               AGENT_NAME,               SOURCEID,               RM_CODE,              ADDRESS1,             ADDRESS2,         ADDRESS3,            
        CITY_ID,                    LOCATION_ID,            MOBILE,                   PINCODE,                FAX,                  CONTACTPER,           EMAIL,   
        TDS  ,                      SUB_BROKER_TYPE ,       category_id,              CPEMAILID ,             TIMEST ,              PHONE     ,           REMARK     ,            
        master_ana,                 ONLINE_SUBSCIPTION ,    ONLINE_BLOCK_AGENT,       BLOCK_AGENT,            online_block_remark,  offline_block_remark, ANA_AUDITDATE,
        ANA_AUDIT,                  PAYMENTMODEID,          ACCTYPEID,                ACCNO,                  AFFECTEDFROM,         BANKID,               CITY_NAME,            
        BANK_BRANCH_NAME /*BANK_BRANCHID*/,              SMS_FLAG,               GSTIN_NO,                 DOB,                    AGENT_TYPE,           PAN,                  DIST,             
        AADHAR_CARD_NO,             POSP_MARKING ,          POSP_TYPE ,               POSP_NO_LI,             POSP_NO_GI,           POSP_CERTIFIED_ON_LI,  POSP_VALID_TILL_LI,          
        POSP_CERTIFIED_ON_GI,       POSP_VALID_TILL_GI,     VERIFIED_STATUS ,         /*NEFT_BANK_NAME,         BANK_BRANCH_NAME,     IFSC_CODE,            NAME_IN_BANK,*/           
        AMFICERT,                   AMFIEXTYPEID,           AMFIID,                   
        AGENT_MASTER.r_address1,  AGENT_MASTER.r_address2,  AGENT_MASTER.r_state_name, AGENT_MASTER.r_city_name, AGENT_MASTER.r_pincode, DOC_ID, TITLE, GENDER

    ) VALUES (
      P_LOGGEDINUSER,              P_EMPANELMENTTYPE,       upper(P_AGENTNAME),              P_SOURCEID,             P_RMCODE,             upper(P_ADDRESS1),            upper(P_ADDRESS2),     UPPER(P_ADDRESS3),
      P_CITYID,                    P_LOCATIONID,            P_MOBILE,                 P_PIN,                  P_FAX,                upper(P_CONTACTPER),          upper(P_EMAIL),          
      P_TDS,                       P_ASSOCIATETYPE,         P_ASSOCIATETYPECATEGORY,  upper(P_CONTACTPERSONEMAILID), SYSDATE,              P_PHONE,                P_REMARKS,      
      P_SUPERANA,                  P_ONLINESUBSCRIPTIONCHECK, P_OFFLINEPLATFORMBLOCK, P_ONLINEPLATFORMBLOCK, P_ONLINEPLATFORMBLOCKREMARK, P_OFFLINEPLATFORMBLOCKREMARK, P_AUDITDATE,   
      P_AUDITCHECK,                P_PAYMENTMODEID,         P_ACCOUNTTYPEID,          P_ACCNO,                P_AFFECTEDFROM,       P_BANKID,               P_BANKCITYID,   
      v_bank_branch_name, /*P_BRANKBRANCHID,*/             P_SMSFLAG,               P_GSTINNO,                P_DOB,                  P_AGENTTYPE,          P_PAN,                  P_DIST,     
      trim(P_AADHARCARDNO),        P_POSPMARKING,           P_POSPTYPE,               P_POSPNOLI,             P_POSPNOGI,           P_POSPCERTIFIEDONLI,    P_POSPVALIDTILLLI,     
      P_POSPCERTIFIEDONGI,         P_POSPVALIDTILLGI,       P_VERIFIEDSTATUS,       /*  P_NEFTBANKNAME,         P_NEFTBRANCH,         P_NEFTIFSCCODE,         P_NEFTNAME,  */ 
      P_CERTPASSED,                P_CERTEXAMS,             P_CERTREGNO,              
      upper(P_RES_ADD_1),          upper(P_RES_ADD_2),      upper(P_RES_ADD_STATE),   upper(P_RES_ADD_CITY),   trim(P_RES_ADD_PINCODE), trim(P_DT_NUMBER), P_TITLE, P_GENDER
    ) RETURNING AGENT_CODE INTO NEW_AGENT_CODE;-- Retrieve the new agent code

    COMMIT;

    UPDATE AGENT_MASTER 
    SET DOC_ID =  trim(P_DT_NUMBER)
    WHERE UPPER(AGENT_NAME) = upper(P_AGENTNAME)
    AND MOBILE = P_MOBILE
    AND UPPER(PAN) = UPPER(P_PAN);

    UPDATE TB_DOC_UPLOAD
    SET PUNCHING_FLAG = '1'
    WHERE COMMON_ID = P_DT_NUMBER
    AND TRAN_TYPE = 'ANA';

    -- Return success message
    OPEN P_RESULT FOR
           SELECT 'Agent inserted successfully with AGENT_CODE: ' || NEW_AGENT_CODE AS MESSAGE FROM dual;
    RETURN;

    EXCEPTION
        WHEN OTHERS THEN
            -- Capture the error message
            ERROR_MESSAGE := 'Defined Error: ' || SQLERRM;


            -- Return the error message as a result
            OPEN P_RESULT FOR
                SELECT ERROR_MESSAGE AS MESSAGE FROM DUAL;
            RETURN;
    end;
END PSM_AM_INSERT_AGENT_MASTER;