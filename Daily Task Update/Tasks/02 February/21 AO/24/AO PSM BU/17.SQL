create or replace PROCEDURE PSM_AO_MM_Valid_Insert (
    p_exist_client_code   IN VARCHAR2,
    p_mobile              IN VARCHAR2,
    p_pan                 IN VARCHAR2,
    p_email               IN VARCHAR2,
    p_gpan                IN VARCHAR2,
    p_aadhar_value        IN VARCHAR2,
    p_result              OUT SYS_REFCURSOR 
) AS
    v_generated_client_code client_test.client_code%TYPE;
    v_generated_inv_code investor_master.inv_code%type;
    v_aadhar_number         BOOLEAN;
    v_count                 NUMBER;
    nextinvkyc              NUMBER;
    v_pan_flag              NUMBER;

    -- Declare local variables
    VPAN                    NUMBER;
    VPAN2                    NUMBER;

    VMOBILE                 NUMBER;
    VEMAIL                  NUMBER;
    VG_PAN                  NUMBER;
    ISVALIDPAN              VARCHAR2(10);
    ISVALID_GPAN            VARCHAR2(10);
    ISVALIDMOBILE           NUMBER;
    ISVALIDEMAIL            NUMBER;
    v_message               VARCHAR2(4000); 
    V_SOURCE_CODE           VARCHAR2(20); 
    ISVALID_AADHAAR           NUMBER;
    V_AADHAR                NUMBER;
BEGIN
   -- Validate the PAN, Mobile, and Email formats first


IF p_mobile IS NOT NULL THEN -- VALIDATE MOBILE AND RETURN INVALID, DUPLICATE
    ISVALIDMOBILE := VALIDATE_MOBILE(p_mobile);
    IF ISVALIDMOBILE = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)' AS message FROM DUAL;
        RETURN;  
        
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE MOBILE = p_mobile 
        AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8));   
    ELSIF ISVALIDMOBILE = 1  AND VMOBILE > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number' AS message FROM DUAL;
        RETURN;
    END IF;
END IF;

IF p_email IS NOT NULL THEN -- VALIDATE EMAIL AND RETURN INVALID, DUPLICATE
    IF upper(p_email) NOT IN ('NA', 'NOT AVAILABLE', 'NONE', 'N/A') THEN
        ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(p_email));
        IF ISVALIDEMAIL = 0 THEN
            OPEN P_RESULT FOR SELECT 'Invalid Email' AS message FROM DUAL;
            RETURN;
            
            SELECT COUNT(EMAIL) 
            INTO VEMAIL 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE UPPER(EMAIL) = UPPER(p_email)
            AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8));  
        ELSIF ISVALIDEMAIL = 1  AND VEMAIL > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Email' AS message FROM DUAL;
        END IF;
    END IF;
END IF ;


IF p_pan IS NOT NULL THEN -- Validate PAN and return appropriate error messages
    ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(p_pan, 1, 10))));
    IF ISVALIDPAN IN ('InValid', 'Miss') THEN
        OPEN P_RESULT FOR 
        SELECT 'Invalid PAN (e.g., AAAAA9999A)' AS message FROM DUAL;
        RETURN;

        SELECT count(PAN) 
        INTO VPAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(p_pan, 1, 10)));
    ELSIF ISVALIDPAN = 'Valid' AND VPAN > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate PAN' AS message FROM DUAL;
        RETURN;
    END IF;
END IF;


IF p_gpan IS NOT NULL THEN  -- VALIDATE GUARDIAN PAN, return an error message
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(p_gpan, 1, 10))));
    IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN 
        OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL; 
        RETURN;

        SELECT COUNT(G_PAN) 
        INTO VG_PAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(G_PAN) = UPPER(p_gpan)
        AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8)); 
    ELSIF ISVALID_GPAN = 'Valid' and VG_PAN > 0 THEN 
        OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families) ' || VG_PAN AS message FROM DUAL; 
        RETURN; 
    END IF;
END IF;


IF p_aadhar_value IS NOT NULL THEN -- VALIDATE AADHAR NUMBER 
    ISVALID_AADHAAR := PSM_VALIDATEAADHAAR(TRIM(P_AADHAR_VALUE));

    IF ISVALID_AADHAAR = 'Invalid' OR ISVALID_AADHAAR = 'Miss' THEN 
        OPEN P_RESULT FOR SELECT 'Invalid Aadhaar Number (Must be 12-digit numeric)' AS message FROM DUAL;
        RETURN;

        SELECT COUNT(aadhar_card_no) 
        INTO V_AADHAR 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE aadhar_card_no = TRIM(SUBSTR(p_aadhar_value,1,12));
    ELSIF ISVALID_AADHAAR = 'Valid' AND V_AADHAR > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Aadhar Number' AS message FROM DUAL;
        RETURN;
    END IF;
END IF;


    OPEN p_result FOR 
    SELECT 'All Validation Pass | Mobile: ' || ISVALIDMOBILE || '/' || VMOBILE ||
    ' | Email: ' || ISVALIDEMAIL || '/' || VEMAIL ||
    ' | PAN: ' || ISVALIDPAN || '/' || VPAN ||
    ' | Guardian PAN: ' || ISVALID_GPAN || '/' || VG_PAN ||
    ' | Aadhar: ' || V_AADHAR AS message
    FROM dual;
END PSM_AO_MM_Valid_Insert;