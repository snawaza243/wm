-- Validate DT Number and return appropriate error messages
IF TRIM(P_DT_NUMBER) IS NOT NULL THEN

        SELECT NVL(COUNT(COMMON_ID),0)
        INTO DT_FLAG
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_DT_NUMBER)
        AND tran_type= 'AC' AND rejection_status = '0' AND verification_flag = '1' AND PUNCHING_FLAG = '0';

    IF DT_FLAG = 0 THEN
        OPEN P_RESULT FOR 
        SELECT 'Please Enter Valid DT Number' AS message 
        FROM DUAL;
        RETURN;  
    END IF;
ELSE
    --OPEN P_RESULT FOR SELECT 'Please enter DT number for new account' AS message FROM DUAL; RETURN;
    ;
END IF;

-- Validate Business RM Code  
IF TRIM(P_RM_BUSINESS_CODE) IS NULL THEN
    OPEN P_RESULT FOR
        SELECT 'Valid Business RM Required' AS message FROM DUAL;
    RETURN;
END IF;

-- VALIDATE GUESTCODE, PAN, GPAN, AADHAR, EMAIL MOBILE
IF P_GUEST_CODE IS NOT NULL THEN 
    SELECT NVL(COUNT(GUEST_CD),0) 
    INTO GUEST_CD_EXIST_FLAG
    FROM BAJAJ_VENUE_BOOKING 
    WHERE GUEST_CD = TRIM(P_GUEST_CODE);

    IF GUEST_CD_EXIST_FLAG = 0 THEN
        -- Guest Code does not exist in BAJAJ_VENUE_BOOKING
        OPEN P_RESULT FOR 
        SELECT 'Guest Code does not exist' AS message FROM DUAL;
        RETURN;
    END IF;

    SELECT NVL(COUNT(GUEST_CD),0) 
    INTO DUP_GUEST_FLAG
    FROM CLIENT_MASTER 
    WHERE GUEST_CD = TRIM(P_GUEST_CODE);

    IF DUP_GUEST_FLAG > 0 THEN
        -- Duplicate Guest Code found in CLIENT_MASTER
        OPEN P_RESULT FOR 
        SELECT 'Duplicate Guest Code' AS message FROM DUAL;
        RETURN;
    END IF;
END IF;

IF P_CLIENT_PAN IS NOT NULL THEN
    ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10))));
    IF ISVALIDPAN IN ('InValid', 'Miss') THEN
        OPEN P_RESULT FOR 
        SELECT 'Invalid PAN (e.g., AAAAA9999A)' AS message 
        FROM DUAL;
        RETURN;
    ELSIF ISVALIDPAN = 'Valid' THEN
        SELECT NVL(count(PAN),0)
        INTO VPAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10)));

        IF VPAN > 0 THEN 
            OPEN P_RESULT FOR 
            SELECT 'Duplicate PAN' AS message 
            FROM DUAL;
            RETURN;
        END IF;
    END IF;
END IF;

IF P_GUARDIAN_PERSON_PAN IS NOT NULL THEN       
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GUARDIAN_PERSON_PAN, 1, 10))));

    IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
        OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
        RETURN;
    ELSIF ISVALID_GPAN = 'Valid' THEN

            SELECT NVL(COUNT(G_PAN),0)
            INTO VG_PAN 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE UPPER(G_PAN) = UPPER(P_GUARDIAN_PERSON_PAN);

        IF VG_PAN > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families)' AS message FROM DUAL;
            RETURN;
        END IF;
    END IF;
end if;

IF P_AADHAR_VALUE IS NOT NULL THEN 
    ISVALID_AADHAAR := PSM_VALIDATEAADHAAR(TRIM(P_AADHAR_VALUE));

    IF ISVALID_AADHAAR = 'Invalid' OR ISVALID_AADHAAR = 'Miss' THEN 
        OPEN P_RESULT FOR SELECT 'Invalid Aadhaar Number (Must be 12-digit numeric)' AS message FROM DUAL;  RETURN;
    ELSIF ISVALID_AADHAAR = 'Valid' THEN          
            SELECT NVL(COUNT(aadhar_card_no),0)
            INTO V_AADHAR 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE aadhar_card_no = TRIM(SUBSTR(P_AADHAR_VALUE,1,12))
            and P_AADHAR_VALUE is not null
            and P_AADHAR_VALUE != 0;
        IF V_AADHAR > 0 THEN 
        OPEN P_RESULT FOR 
            SELECT 'Duplicate Aadhaar Number' AS message FROM DUAL; 
            RETURN; 
        END IF;
    END IF;
END IF;

IF P_EMAIL_VALUE IS NOT NULL THEN
    ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(P_EMAIL_VALUE));
    IF UPPER(P_EMAIL_VALUE) NOT IN ('NA', 'NOT AVAILABLE', 'N/A','NONE') THEN
        IF ISVALIDEMAIL = 0 THEN
            OPEN P_RESULT FOR SELECT 'Invalid Email' AS message FROM DUAL;
            RETURN;
        ELSIF ISVALIDEMAIL = 1  THEN
                SELECT NVL(COUNT(EMAIL),0) 
                INTO VEMAIL 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE UPPER(EMAIL) = UPPER(P_EMAIL_VALUE);
            IF VEMAIL > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Email' AS message FROM DUAL;
            END IF;
        END IF;
    END IF ;
END IF;

IF P_MOBILE_NO_VALUE IS NOT NULL AND UPPER(P_NATIONALITY) NOT IN ('NRI') THEN
    ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE_NO_VALUE);

    IF ISVALIDMOBILE = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)' AS message FROM DUAL;
        RETURN;    
    ELSIF ISVALIDMOBILE = 1  THEN
            SELECT NVL(COUNT(MOBILE),0)
            INTO VMOBILE 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE MOBILE = P_MOBILE_NO_VALUE ;        

        IF VMOBILE > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number..' AS message FROM DUAL;
            RETURN;
        END IF;
    END IF;
END IF;
