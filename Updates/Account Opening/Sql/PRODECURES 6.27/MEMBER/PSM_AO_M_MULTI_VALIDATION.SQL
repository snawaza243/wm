CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_M_MULTI_VALIDATION  (
    P_DT_NUMBER           IN VARCHAR2,
    P_EXIST_CLIENT_CODE   IN NUMBER,
    P_EXISTING_MEMBER_INV IN NUMBER,
    P_HEAD_NATIONALITY    IN VARCHAR2,

    P_RM_BUSINESS_CODE    IN VARCHAR2,
    P_SALUTATION1         IN VARCHAR2,
    P_ACCOUNT_NAME        IN VARCHAR2,
    P_LOGGED_IN_USER      IN VARCHAR2,
    P_CLIENT_CODE_IN_MAIN IN VARCHAR2,
    P_MOBILE              IN VARCHAR2,
    P_PAN                 IN VARCHAR2,
    P_EMAIL               IN VARCHAR2,
    P_DOB                 DATE,
    P_RELATION            IN NUMBER,
    P_GNAME               IN VARCHAR2,
    P_GPAN                IN VARCHAR2,
    P_OCCUPATION          IN NUMBER,
    P_KYC                 IN VARCHAR2,
    P_APPROVE             IN VARCHAR2,
    P_GENDER              IN VARCHAR2,
    P_NOM                 IN VARCHAR2,
    P_ALLO                IN NUMBER,
    P_AADHAR_VALUE        IN VARCHAR2,
    P_RESULT              OUT SYS_REFCURSOR  -- Cursor to return results
) AS
    V_GENERATED_CLIENT_CODE CLIENT_TEST.CLIENT_CODE%TYPE;
    V_GENERATED_INV_CODE INVESTOR_MASTER.INV_CODE%TYPE;
    V_AADHAR_NUMBER         BOOLEAN;
    V_COUNT                 NUMBER;
    NEXTINVKYC              NUMBER;
    V_PAN_FLAG              NUMBER;

    -- Declare local variables
    VPAN                    NUMBER;
    VPAN2                    NUMBER;

    VMOBILE                 NUMBER;
    VEMAIL                  NUMBER;
    VG_PAN                  NUMBER;
    ISVALIDPAN              VARCHAR2(10);
    ISVALID_GPAN            VARCHAR2(10);
    ISVALID_AADHAAR         VARCHAR2(12);

    ISVALIDMOBILE           NUMBER;
    ISVALIDEMAIL            NUMBER;
    V_MESSAGE               VARCHAR2(4000); -- To hold the result message
    V_SOURCE_CODE           VARCHAR2(20); -- To store the SOURCE_CODE of the current client
    ISVALIDAADAHR           NUMBER;
    V_AADHAR                NUMBER;

    CT_FLAG                 NUMBER;

    V_PARSED_DOB            DATE;        
    V_AGE                   NUMBER;               
BEGIN


IF TRIM(P_EXISTING_MEMBER_INV) IS NULL THEN
    BEGIN
        IF TRIM(P_MOBILE) IS NOT NULL AND UPPER(P_HEAD_NATIONALITY) = 'NO' THEN -- VALIDATE AND RETURN INVALID, DUPLICATE
            ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE);

            IF ISVALIDMOBILE = 0 THEN
                OPEN P_RESULT FOR SELECT 'Member Save: Invalid Mobile Number (e.g. 9999999999)' AS MESSAGE FROM DUAL;
                RETURN;  
                  
            ELSIF ISVALIDMOBILE = 1  THEN
                SELECT NVL(COUNT(MOBILE) ,0) INTO VMOBILE 
                FROM WEALTHMAKER.INVESTOR_MASTER WHERE MOBILE = TRIM(P_MOBILE)                
                AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8)); 
                  
                IF VMOBILE > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Save: Duplicate Mobile Number' AS MESSAGE FROM DUAL; 
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF TRIM(P_EMAIL) IS NOT NULL THEN -- VALIDATE EMAIL AND RETURN INVALID, DUPLICATE
            IF UPPER(P_EMAIL) NOT IN ('NA', 'NOT AVAILABLE', 'N/A', 'NONE') THEN
                ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(TRIM(P_EMAIL)));

                IF ISVALIDEMAIL = 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Save: Invalid Email' AS MESSAGE FROM DUAL;
                    RETURN;

                ELSIF ISVALIDEMAIL = 1  THEN 
                    SELECT NVL(COUNT(EMAIL),0) INTO VEMAIL 
                    FROM WEALTHMAKER.INVESTOR_MASTER WHERE UPPER(EMAIL) = UPPER(TRIM(P_EMAIL))
                   
                    AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8));

                    IF VEMAIL > 0 THEN
                        OPEN P_RESULT FOR SELECT 'Member Save: Duplicate Email' AS MESSAGE FROM DUAL;
                        RETURN;
                    END IF;
                END IF;
            END IF;
        END IF ;

        IF TRIM(P_PAN) IS NOT NULL THEN -- Validate PAN and return appropriate error messages
           ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_PAN, 1, 10))));

            IF ISVALIDPAN IN ('InValid', 'Miss') THEN
                OPEN P_RESULT FOR SELECT 'Member Save: Invalid PAN (e.g., AAAAA9999A)' AS MESSAGE  FROM DUAL;
                RETURN;

            ELSIF ISVALIDPAN = 'Valid' THEN 
                SELECT NVL(COUNT(PAN),0) INTO VPAN 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_PAN, 1, 10)));

                IF VPAN > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Save: Duplicate PAN' AS MESSAGE  FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF TRIM(P_AADHAR_VALUE) IS NOT NULL THEN 
            ISVALID_AADHAAR := PSM_VALIDATEAADHAAR(TRIM(P_AADHAR_VALUE));

            IF ISVALID_AADHAAR = 'Invalid' OR ISVALID_AADHAAR = 'Miss' THEN 
                OPEN P_RESULT FOR SELECT 'Member Save: Invalid Aadhaar Number (Must be 12-digit numeric)' AS MESSAGE FROM DUAL;  
                RETURN;

            ELSIF ISVALID_AADHAAR = 'Valid' THEN          
                    SELECT NVL(COUNT(AADHAR_CARD_NO),0) INTO V_AADHAR 
                    FROM WEALTHMAKER.INVESTOR_MASTER
                    WHERE AADHAR_CARD_NO = TRIM(SUBSTR(P_AADHAR_VALUE,1,12)) 
                    AND P_AADHAR_VALUE IS NOT NULL AND P_AADHAR_VALUE != 0;

                IF V_AADHAR > 0 THEN 
                    OPEN P_RESULT FOR SELECT 'Member Save: Duplicate Aadhaar Number' AS MESSAGE FROM DUAL; 
                    RETURN; 
                END IF;
            END IF;
        END IF;

        IF TRIM(P_GPAN) IS NOT NULL THEN  -- VALIDATE GUARDIAN PAN, return an error message
            ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GPAN, 1, 10))));

            IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN 
                OPEN P_RESULT FOR SELECT 'Member Save: Invalid Guardian PAN (e.g. AAAAA9999A)' AS MESSAGE FROM DUAL; 
                RETURN;

            ELSIF ISVALID_GPAN = 'Valid' THEN
                SELECT NVL(COUNT(G_PAN),0) INTO VG_PAN 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE UPPER(G_PAN) = UPPER(P_GPAN)
                AND SOURCE_ID != TRIM(SUBSTR(P_EXIST_CLIENT_CODE,1,8)); 

                IF VG_PAN > 0 THEN 
                    OPEN P_RESULT FOR SELECT 'Member Save: Duplicate Guardian PAN (Cannot be shared across different families) ' || VG_PAN AS MESSAGE FROM DUAL; 
                    RETURN; 
                END IF;
            END IF;
        END IF;

        OPEN P_RESULT FOR 
        SELECT 'MEMBER INSERT VALIDATION PASS | Mobile: ' || ISVALIDMOBILE || '/' || VMOBILE ||
        ' | Email: ' || ISVALIDEMAIL || '/' || VEMAIL ||
        ' | PAN: ' || ISVALIDPAN || '/' || VPAN ||
        ' | Guardian PAN: ' || ISVALID_GPAN || '/' || VG_PAN ||
        ' | Aadhar: ' || V_AADHAR AS MESSAGE FROM DUAL;
    END;
END IF;

IF TRIM(P_EXISTING_MEMBER_INV) IS NOT NULL THEN
    BEGIN
        SELECT NVL(COUNT(CLIENT_CODEKYC),0) INTO CT_FLAG
        FROM CLIENT_TEST WHERE CLIENT_CODEKYC = P_EXISTING_MEMBER_INV;

        IF TRIM(P_MOBILE) IS NOT NULL AND UPPER(P_HEAD_NATIONALITY)  = 'NO' THEN -- VALIDATE MOBILE AND RETURN INVALID, DUPLICATE
            ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE);
            
            IF ISVALIDMOBILE = 0 THEN
                OPEN P_RESULT FOR SELECT 'Member Update: Invalid Mobile Number (e.g. 9999999999)' || P_HEAD_NATIONALITY AS MESSAGE FROM DUAL;
                RETURN;        

            ELSIF ISVALIDMOBILE = 1  THEN
                SELECT NVL(COUNT(MOBILE),0) 
                INTO VMOBILE 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE MOBILE = TRIM(P_MOBILE)  
                AND SOURCE_ID != TRIM(SUBSTR(P_EXISTING_MEMBER_INV,1,8));  --AND INV_CODE != TRIM(P_EXISTING_MEMBER_INV);

                IF VMOBILE > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Update: Duplicate Mobile Number' AS MESSAGE FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF TRIM(P_EMAIL) IS NOT NULL THEN -- VALIDATE EMAIL AND RETURN INVALID, DUPLICATE
            IF UPPER(P_EMAIL) NOT IN ('NA', 'NOT AVAILABLE', 'N/A', 'NONE') THEN
                ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(TRIM(P_EMAIL)));

                IF ISVALIDEMAIL = 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Update: Invalid Email' AS MESSAGE FROM DUAL;
                    RETURN;

                ELSIF ISVALIDEMAIL = 1  THEN
                    SELECT NVL(COUNT(EMAIL) ,0) INTO VEMAIL 
                    FROM WEALTHMAKER.INVESTOR_MASTER
                    WHERE UPPER(EMAIL) = UPPER(TRIM(P_EMAIL)) 
                    AND SOURCE_ID != TRIM(SUBSTR(P_EXISTING_MEMBER_INV,1,8));

                    IF VEMAIL > 0 THEN
                        OPEN P_RESULT FOR SELECT 'Member Update: Duplicate Email' AS MESSAGE FROM DUAL;
                        RETURN;
                    END IF;
                END IF;
            END IF;
        END IF ;

        IF TRIM(P_PAN) IS NOT NULL THEN -- Validate PAN and return appropriate error messages
            ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_PAN, 1, 10))));

            IF ISVALIDPAN IN ('InValid', 'Miss') THEN
                OPEN P_RESULT FOR SELECT 'Member Update: Invalid PAN (e.g., AAAAA9999A)' AS MESSAGE  FROM DUAL;
                RETURN;
                
            ELSIF ISVALIDPAN = 'Valid' THEN 
                SELECT NVL(COUNT(PAN),0)  INTO VPAN 
                FROM WEALTHMAKER.INVESTOR_MASTER WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_PAN, 1, 10)))
                AND SOURCE_ID != TRIM(SUBSTR(P_EXISTING_MEMBER_INV,1,8));


                IF VPAN > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Update: Duplicate Member PAN (Cannot be shared across different families)' AS MESSAGE FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF P_AADHAR_VALUE IS NOT NULL THEN 
            ISVALID_AADHAAR := PSM_VALIDATEAADHAAR(TRIM(P_AADHAR_VALUE));

            IF ISVALID_AADHAAR = 'Invalid' OR ISVALID_AADHAAR = 'Miss' THEN 
                OPEN P_RESULT FOR SELECT 'Member Update: Invalid Aadhaar Number (Must be 12-digit numeric)' AS MESSAGE FROM DUAL;  
                RETURN;

            ELSIF ISVALID_AADHAAR = 'Valid' THEN          
                    SELECT NVL(COUNT(AADHAR_CARD_NO),0) INTO V_AADHAR 
                    FROM WEALTHMAKER.INVESTOR_MASTER
                    WHERE AADHAR_CARD_NO = TRIM(SUBSTR(P_AADHAR_VALUE,1,12)) 
                    AND INV_CODE != TRIM(P_EXISTING_MEMBER_INV)
                    AND P_AADHAR_VALUE IS NOT NULL AND P_AADHAR_VALUE != 0;

                IF V_AADHAR > 0 THEN 
                    OPEN P_RESULT FOR SELECT 'Member Update: Duplicate Aadhaar Number' AS MESSAGE FROM DUAL; RETURN; 
                END IF;
            END IF;
        END IF;

        IF TRIM(P_GPAN) IS NOT NULL THEN  -- VALIDATE GUARDIAN PAN, return an error message
            ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GPAN, 1, 10))));  

            IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
                OPEN P_RESULT FOR SELECT 'Member Update: Invalid Guardian PAN (e.g. AAAAA9999A)' AS MESSAGE FROM DUAL;
                RETURN;

            ELSIF ISVALID_GPAN = 'Valid' THEN 
                SELECT NVL(COUNT(G_PAN),0) INTO VG_PAN FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE UPPER(G_PAN) = UPPER(TRIM(P_GPAN))
                AND SOURCE_ID != TRIM(SUBSTR(P_EXISTING_MEMBER_INV,1,8));

                IF VG_PAN > 0  OR VPAN2 > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Member Update: Duplicate Guardian PAN (Cannot be shared across different families)' AS MESSAGE FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        OPEN P_RESULT FOR 
        SELECT 'MEMBER UPDATE VALIDATION PASS | Mobile: ' || ISVALIDMOBILE || '/' || VMOBILE ||
        ' | Email: ' || ISVALIDEMAIL || '/' || VEMAIL ||
        ' | PAN: ' || ISVALIDPAN || '/' || VPAN ||
        ' | Guardian PAN: ' || ISVALID_GPAN || '/' || VG_PAN ||
        ' | Aadhar: ' || V_AADHAR AS MESSAGE FROM DUAL;
    END;
END IF;


END PSM_AO_M_MULTI_VALIDATION;
/
