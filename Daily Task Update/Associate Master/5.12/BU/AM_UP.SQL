CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AM_UPDATE_AGENT_MASTER (
    P_LOGGEDINUSER          IN VARCHAR2,
    P_EMPANELMENTTYPE       IN VARCHAR2,
    P_AGENTCODE             IN VARCHAR2,
    P_EXISTCODE             IN VARCHAR2,
    P_TITLE                 IN VARCHAR2,
    P_AGENTNAME             IN VARCHAR2,
    P_GENDER                IN VARCHAR2,
    P_SOURCEID              IN VARCHAR2,
    P_RMCODE                IN VARCHAR2,
    P_ADDRESS1              IN VARCHAR2,
    P_ADDRESS2              IN VARCHAR2,
    P_ADDRESS3              IN VARCHAR2,
    P_CITYID                IN VARCHAR2,
    P_LOCATIONID            IN VARCHAR2,
    P_MOBILE                IN NUMBER,
    P_PIN                   IN VARCHAR2,
    P_FAX                   IN VARCHAR2,
    P_CONTACTPER            IN VARCHAR2,
    P_EMAIL                 IN VARCHAR2,
    P_TDS                   IN NUMBER,
    P_ASSOCIATETYPE         IN VARCHAR2,
    P_ASSOCIATETYPECATEGORY IN VARCHAR2,
    P_CONTACTPERSONEMAILID  IN VARCHAR2,
    P_EMPANELMENTDATE       IN DATE,
    P_PHONE                 IN VARCHAR2,
    P_REMARKS               IN VARCHAR2,
    P_SUPERANA                      IN VARCHAR2,
    P_ONLINESUBSCRIPTIONCHECK       IN VARCHAR2,
    P_ONLINEPLATFORMBLOCK           IN VARCHAR2,
    P_OFFLINEPLATFORMBLOCK          IN VARCHAR2, 
    P_ONLINEPLATFORMBLOCKREMARK     IN VARCHAR2,
    P_OFFLINEPLATFORMBLOCKREMARK    IN VARCHAR2,
    P_AUDITDATE             IN DATE,
    P_AUDITCHECK            IN VARCHAR2,
    P_PAYMENTMODEID         IN NUMBER,
    P_ACCOUNTTYPEID         IN VARCHAR2,
    P_ACCNO                 IN VARCHAR2,
    P_AFFECTEDFROM          IN VARCHAR2,
    P_BANKID                IN VARCHAR2,
    P_BANKCITYID            IN VARCHAR2,
    P_BRANKBRANCHID         IN VARCHAR2,
    P_SMSFLAG               IN VARCHAR2,
    P_GSTINNO               IN VARCHAR2,
    P_DOB                   IN DATE,
    P_AGENTTYPE             IN VARCHAR2,
    P_PAN                   IN VARCHAR2,
    P_DIST                  IN VARCHAR2,
    P_AADHARCARDNO          IN VARCHAR2,
    P_POSPMARKING           IN VARCHAR2,
    P_POSPTYPE              IN VARCHAR2,
    P_POSPNOLI              IN VARCHAR2,
    P_POSPNOGI              IN VARCHAR2,
    P_POSPCERTIFIEDONLI     IN DATE,
    P_POSPVALIDTILLLI       IN DATE,
    P_POSPCERTIFIEDONGI     IN DATE,
    P_POSPVALIDTILLGI       IN DATE,
    P_VERIFIEDSTATUS        IN VARCHAR2,
    P_NEFTBANKNAME          IN VARCHAR2,
    P_NEFTBRANCH            IN VARCHAR2,
    P_NEFTIFSCCODE          IN VARCHAR2,
    P_NEFTNAME              IN VARCHAR2,
    P_CERTPASSED            IN VARCHAR2,
    P_CERTEXAMS             IN VARCHAR2,
    P_CERTREGNO             IN VARCHAR2,
    P_RES_ADD_1             IN VARCHAR2,
    P_RES_ADD_2             IN VARCHAR2,
    P_RES_ADD_STATE         IN VARCHAR2,
    P_RES_ADD_CITY          IN VARCHAR2,
    P_RES_ADD_PINCODE       IN VARCHAR2,
    P_DT_NUMBER             IN VARCHAR2,
    P_RESULT                OUT VARCHAR2
) IS

    V_AADHAR_EXISTS         NUMBER :=0; 
    VPAN                    NUMBER :=0;
    VMOBILE                 NUMBER :=0;
    VEMAIL                  NUMBER :=0;
    VG_PAN                  NUMBER :=0;
    ISVALIDPAN              VARCHAR2(10);
    ISVALID_GPAN            VARCHAR2(10);
    ISVALID_AADHAAR         VARCHAR2(10);
    ISVALIDMOBILE           NUMBER :=0;
    ISVALIDEMAIL            NUMBER :=0;
    V_MESSAGE               VARCHAR2(4000);  
    V_SOURCE_CODE           VARCHAR2(20);
    ISACCESS                NUMBER:=0;


    V_BANK_BRANCH_NAME VARCHAR2(100);
BEGIN
    -- Validate the PAN, Mobile, and Email formats first
    ISVALIDPAN      := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_PAN, 1, 10))));
    ISVALIDMOBILE   := VALIDATE_MOBILE(P_MOBILE);
    ISVALIDEMAIL    := VALIDATE_EMAIL(UPPER(P_EMAIL));

 IF P_BRANKBRANCHID IN ( 'Select Bank Branch', '') THEN
        V_BANK_BRANCH_NAME := NULL;
ELSE
    V_BANK_BRANCH_NAME  :=P_BRANKBRANCHID;
END IF;



    BEGIN -- Check ACCESS 
        SELECT COUNT(ROLE_ID)
        INTO ISACCESS
        FROM WEALTHMAKER.USERDETAILS_JI
        WHERE TRIM(LOGIN_ID) = TRIM(P_LOGGEDINUSER) 
        AND ROLE_ID = '212'; -- FOR PUNCHING 
        EXCEPTION WHEN NO_DATA_FOUND THEN ISACCESS := 0;
    END; 



    BEGIN -- Check for duplicate PAN 
        SELECT COUNT(PAN) 
        INTO VPAN FROM WEALTHMAKER.AGENT_MASTER
        WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_PAN, 1, 10))) AND EXIST_CODE != P_EXISTCODE;
        EXCEPTION WHEN NO_DATA_FOUND THEN VPAN := 0;
    END;

    BEGIN -- validate mobile no 
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.AGENT_MASTER
        WHERE TRIM(MOBILE) = TRIM(P_MOBILE) AND EXIST_CODE != P_EXISTCODE ;
        EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
    END;

    BEGIN -- validate  email 
        SELECT COUNT(EMAIL) 
        INTO VEMAIL 
        FROM WEALTHMAKER.AGENT_MASTER
        WHERE UPPER(EMAIL) = UPPER(P_EMAIL) AND EXIST_CODE != P_EXISTCODE;
        EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
    END;

    --IF ISACCESS = 0 THEN P_RESULT := 'Access: Only Punching Team Can Punch!'; RETURN;  END IF;


    -- If any validation fails, return an error message
    IF ISVALIDPAN = 'InValid' OR ISVALIDPAN = 'Miss' THEN
        P_RESULT := 'Invalid PAN (e.g. AAAAA9999A)';
        RETURN;
    END IF;

    IF ISVALIDPAN = 'Valid' AND VPAN > 0 THEN
        P_RESULT := 'Duplicate PAN';
        RETURN;
    END IF;

    IF ISVALIDMOBILE = 0 THEN
        P_RESULT := 'Invalid Mobile Number (e.g. 9999999999)';
        RETURN;
    END IF;

    IF ISVALIDMOBILE = 1  AND VMOBILE > 0 THEN
        P_RESULT :=  'Duplicate Mobile Number';
        RETURN;
    END IF;


    IF ISVALIDEMAIL = 0 THEN
        P_RESULT := 'Invalid Email'  ;
        RETURN;
     END IF;   


    IF ISVALIDEMAIL = 1  AND VEMAIL > 0 THEN
        P_RESULT := 'Duplicate Email'  ;
        RETURN;
    END IF;


    IF P_AADHARCARDNO IS NOT NULL THEN
        ISVALID_AADHAAR := PSM_VALIDATEAADHAAR(TRIM(P_AADHARCARDNO));

        IF ISVALID_AADHAAR = 'Invalid' OR ISVALID_AADHAAR = 'Miss' THEN 
            P_RESULT := 'Invalid Aadhaar Number (Must be 12-digit numeric)'; 
            RETURN;    

        ELSIF ISVALID_AADHAAR = 'Valid' THEN
            SELECT COUNT(AADHAR_CARD_NO)
            INTO V_AADHAR_EXISTS
            FROM AGENT_MASTER
            WHERE (AADHAR_CARD_NO) = USER_LOG(P_AADHARCARDNO)
            AND EXIST_CODE != P_EXISTCODE;

            IF V_AADHAR_EXISTS > 0 THEN 
                P_RESULT := 'Duplicate Aadhaar Number'; 
                RETURN;
            END IF;
        END IF;
    END IF;

    UPDATE AGENT_MASTER
    SET
        DOC_ID          = P_DT_NUMBER,
        LOGGEDUSERID    = P_LOGGEDINUSER,
	    PAIDFLAG        = P_EMPANELMENTTYPE,
        TITLE           = P_TITLE,
        AGENT_NAME      = P_AGENTNAME,
        GENDER          = P_GENDER,
        SOURCEID        = P_SOURCEID,
        RM_CODE         = P_RMCODE,
        ADDRESS1        = P_ADDRESS1,
        ADDRESS2        = P_ADDRESS2,
        ADDRESS3        = P_ADDRESS3,
        CITY_ID         = P_CITYID,
        LOCATION_ID     = P_LOCATIONID,
        MOBILE          = P_MOBILE,
        PINCODE         = P_PIN,
        FAX             = P_FAX,
        CONTACTPER      = P_CONTACTPER,
        EMAIL           = P_EMAIL,
        TDS             = P_TDS,
        SUB_BROKER_TYPE = P_ASSOCIATETYPE,
        CATEGORY_ID     = P_ASSOCIATETYPECATEGORY,
        CPEMAILID       = P_CONTACTPERSONEMAILID,
        MODIFY_DATE     = SYSDATE,
        MODIFY_USER     = P_LOGGEDINUSER, 
        PHONE           = P_PHONE,
        REMARK          = P_REMARKS,
        MASTER_ANA      = P_SUPERANA,
        ONLINE_SUBSCIPTION      = P_ONLINESUBSCRIPTIONCHECK ,
        ONLINE_BLOCK_AGENT      = P_ONLINEPLATFORMBLOCK,
        BLOCK_AGENT             = P_OFFLINEPLATFORMBLOCK,
        ONLINE_BLOCK_REMARK     = P_ONLINEPLATFORMBLOCKREMARK,
        OFFLINE_BLOCK_REMARK    = P_OFFLINEPLATFORMBLOCKREMARK,
        ANA_AUDITDATE           = P_AUDITDATE,
        ANA_AUDIT               = P_AUDITCHECK,
        PAYMENTMODEID           = P_PAYMENTMODEID,
        ACCTYPEID               = P_ACCOUNTTYPEID,
        ACCNO                   = P_ACCNO,
        AFFECTEDFROM            = P_AFFECTEDFROM ,
        BANKID                  = P_BANKID,
        CITY_NAME               = P_BANKCITYID,
        BANK_BRANCH_NAME        = V_BANK_BRANCH_NAME, --P_BRANKBRANCHID,
        SMS_FLAG                = P_SMSFLAG,
        GSTIN_NO                = P_GSTINNO,
        DOB                     = P_DOB,
        STATUSID                = P_AGENTTYPE,
        PAN                     = P_PAN,
        DIST                    = P_DIST,
        AADHAR_CARD_NO          = USER_LOG(P_AADHARCARDNO),
        POSP_MARKING            = P_POSPMARKING ,
        POSP_TYPE               = P_POSPTYPE ,
        POSP_NO_LI              = P_POSPNOLI,
        POSP_NO_GI              = P_POSPNOGI,
        POSP_CERTIFIED_ON_LI    = P_POSPCERTIFIEDONLI,
        POSP_VALID_TILL_LI      = P_POSPVALIDTILLLI,
        POSP_CERTIFIED_ON_GI    = P_POSPCERTIFIEDONGI,
        POSP_VALID_TILL_GI      = P_POSPVALIDTILLGI,
        VERIFIED_STATUS         = P_VERIFIEDSTATUS ,
        --NEFT_BANK_NAME = P_NEFTBANKNAME , BANK_BRANCH_NAME = P_NEFTBRANCH , --IFSC_CODE = P_NEFTIFSCCODE , --NAME_IN_BANK = P_NEFTNAME, 
        AMFICERT                = P_CERTPASSED,
        AMFIEXTYPEID            = P_CERTEXAMS, 
        AMFIID                  = P_CERTREGNO,
        AGENT_MASTER.R_ADDRESS1     = P_RES_ADD_1,
        AGENT_MASTER.R_ADDRESS2     = P_RES_ADD_2,
        AGENT_MASTER.R_STATE_NAME   = P_RES_ADD_STATE,
        AGENT_MASTER.R_CITY_NAME    = P_RES_ADD_CITY,
        AGENT_MASTER.R_PINCODE      = P_RES_ADD_PINCODE
    WHERE EXIST_CODE = P_EXISTCODE;

    IF SQL%ROWCOUNT > 0 THEN
        P_RESULT := 'Agent Updated Successfully --> (' || P_AGENTCODE || '/' || P_EXISTCODE || ')';
    ELSE
        P_RESULT := 'No records found to update';
    END IF;
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        P_RESULT := 'Error: ' || SQLERRM;

END PSM_AM_UPDATE_AGENT_MASTER;
/
