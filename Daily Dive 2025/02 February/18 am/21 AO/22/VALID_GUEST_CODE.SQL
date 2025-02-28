 
IF p_guest_cd IS NOT NULL THEN 
    -- Check if GUEST_CD exists in BAJAJ_VENUE_BOOKING
    BEGIN
        SELECT COUNT(GUEST_CD) 
        INTO GUEST_CD_EXIST_FLAG
        FROM BAJAJ_VENUE_BOOKING 
        WHERE GUEST_CD = TRIM(p_guest_cd);
        EXCEPTION WHEN NO_DATA_FOUND THEN GUEST_CD_EXIST_FLAG := 0;
    END;

    IF GUEST_CD_EXIST_FLAG = 0 THEN
        -- Guest Code does not exist in BAJAJ_VENUE_BOOKING
        OPEN o_cursor FOR 
        SELECT 'Guest Code does not exist' AS message FROM DUAL;
        RETURN;
    END IF;

    -- Check if GUEST_CD exists in CLIENT_MASTER
    BEGIN
        SELECT COUNT(GUEST_CD) 
        INTO DUP_GUEST_FLAG
        FROM CLIENT_MASTER 
        WHERE GUEST_CD = TRIM(p_guest_cd);
        EXCEPTION WHEN NO_DATA_FOUND THEN DUP_GUEST_FLAG := 0;
    END;
    
    IF DUP_GUEST_FLAG > 0 THEN
        -- Duplicate Guest Code found in CLIENT_MASTER
        OPEN o_cursor FOR 
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

        BEGIN -- Check for duplicate PAN globally
            SELECT count(PAN) 
            INTO VPAN 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE UPPER(TRIM(SUBSTR(PAN, 1, 10))) = UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10)));
            EXCEPTION WHEN NO_DATA_FOUND THEN VPAN := 0;
        END;
        IF VPAN > 0 THEN 
            OPEN P_RESULT FOR 
            SELECT 'Duplicate PAN' AS message 
            FROM DUAL;
            RETURN;
        END IF;
    END IF;
END IF;

IF P_GUARDIAN_PERSON_PAN IS NOT NULL THEN       -- VALIDATE GUARDIAN PAN, return an error message
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GUARDIAN_PERSON_PAN, 1, 10))));
    
    IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
        OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
        RETURN;

    ELSIF ISVALID_GPAN = 'Valid' THEN
    BEGIN
        SELECT COUNT(G_PAN) 
        INTO VG_PAN 
        FROM WEALTHMAKER.INVESTOR_MASTER
        WHERE UPPER(G_PAN) = UPPER(P_GUARDIAN_PERSON_PAN);
        EXCEPTION WHEN NO_DATA_FOUND THEN VG_PAN := 0;
    END;
    
    IF VG_PAN > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families)' AS message FROM DUAL;
        RETURN;
    END IF;
end if;

IF P_AADHAR_VALUE IS NOT NULL THEN              -- VALIDATE AADHAR NUMBER 
    ISVALID_AADHAAR := PSM_VALIDATEAADHAAR(TRIM(P_AADHAR_VALUE));

    IF ISVALID_AADHAAR = 'Invalid' OR ISVALID_AADHAAR = 'Miss' THEN 
        OPEN P_RESULT FOR 
        SELECT 'Invalid Aadhaar Number (Must be 12-digit numeric)' AS message FROM DUAL; 
        RETURN;
    ELSIF ISVALID_AADHAAR = 'valid' THEN 
        BEGIN
            SELECT COUNT(aadhar_card_no) 
            INTO V_AADHAR 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE aadhar_card_no = TRIM(SUBSTR(P_AADHAR_VALUE,1,12))
            and P_AADHAR_VALUE is not null
            and P_AADHAR_VALUE != 0
            ;
            EXCEPTION WHEN NO_DATA_FOUND THEN V_AADHAR := 0;
        END;

        IF V_AADHAR > 0 THEN 
        OPEN P_RESULT FOR 
            SELECT 'Duplicate Aadhaar Number' AS message FROM DUAL; 
            RETURN; 
        END IF;
    END IF;
END IF;

IF P_EMAIL_VALUE IS NOT NULL THEN
    ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(P_EMAIL_VALUE));
    IF P_EMAIL_VALUE NOT IN ('NA', 'NOT AVAILABLE', 'N/A','NONE') THEN
        IF ISVALIDEMAIL = 0 THEN
            OPEN P_RESULT FOR SELECT 'Invalid Email' AS message FROM DUAL;
            RETURN;

        ELSIF ISVALIDEMAIL = 1  THEN
            BEGIN -- Check if the same email exists for a different family (SOURCE_CODE)
                SELECT COUNT(EMAIL) 
                INTO VEMAIL 
                FROM WEALTHMAKER.INVESTOR_MASTER
                WHERE UPPER(EMAIL) = UPPER(P_EMAIL_VALUE);
                EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
            END;
        IF VEMAIL > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Email' AS message FROM DUAL;
        END IF;
    END IF;
END IF ;


IF P_MOBILE_NO_VALUE IS NOT NULL THEN
    ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE_NO_VALUE);

    IF ISVALIDMOBILE = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)' AS message FROM DUAL;
        RETURN;    
    ELSIF ISVALIDMOBILE = 1  THEN
        BEGIN  -- validate mobile no of family sourcecode 
            SELECT COUNT(MOBILE) 
            INTO VMOBILE 
            FROM WEALTHMAKER.INVESTOR_MASTER
            WHERE MOBILE = P_MOBILE_NO_VALUE ;        
            EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
        END;
        
        IF VMOBILE > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number' AS message FROM DUAL;
            RETURN;
    END IF;
END IF;

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


    -- VALIDATE EMAIL AND RETURN INVALID, DUPLICATE






