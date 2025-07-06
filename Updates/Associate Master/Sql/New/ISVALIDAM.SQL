CREATE OR REPLACE PROCEDURE PSM_AM_IS_VALID (
    P_LOGGEDINUSER          IN VARCHAR2,
    P_ROLEID                IN VARCHAR2,
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
    ISVALIDMOBILE           NUMBER :=0;
    ISVALIDEMAIL            NUMBER :=0;
    V_MESSAGE               VARCHAR2(4000);  
    V_SOURCE_CODE           VARCHAR2(20);
    ISACCESS                NUMBER:=0;

BEGIN


IF P_EXISTCODE IS NOT NULL THEN
BEGIN
    -- Validate the PAN, Mobile, and Email formats first
    ISVALIDPAN      := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_PAN, 1, 10))));
    ISVALIDMOBILE   := VALIDATE_MOBILE(P_MOBILE);
    ISVALIDEMAIL    := VALIDATE_EMAIL(UPPER(P_EMAIL));

    BEGIN -- Check ACCESS 
        SELECT COUNT(ROLE_ID)
        INTO ISACCESS
        FROM WEALTHMAKER.USERDETAILS_JI
        WHERE TRIM(LOGIN_ID) = TRIM(P_LOGGEDINUSER) 
        AND ROLE_ID = TRIM(P_ROLEID); -- FOR PUNCHING 212 
        EXCEPTION WHEN NO_DATA_FOUND THEN ISACCESS := 0;
    END; 

    BEGIN -- Check for duplicate aadhar 
        SELECT COUNT(AADHAR_CARD_NO)
        INTO V_AADHAR_EXISTS
        FROM AGENT_MASTER
        WHERE AADHAR_CARD_NO = P_AADHARCARDNO AND EXIST_CODE != P_EXISTCODE;
        EXCEPTION WHEN NO_DATA_FOUND THEN V_AADHAR_EXISTS := 0;
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
    
    

    IF ISACCESS = 0 OR ISACCESS > 0  THEN 
        P_RESULT := 'Access: Only Punching Team Can Punch!' ||  P_LOGGEDINUSER || P_ROLEID  ; 
        RETURN; 
    END IF;


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
    
    ELSE
        P_RESULT := 'AGENT PASS --> (' || P_AGENTCODE || '/' || P_EXISTCODE || ')';
    ELSE
        P_RESULT := 'No records found to update';
    END IF;
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        P_RESULT := 'Error: ' || SQLERRM;

END PSM_AM_IS_VALID;