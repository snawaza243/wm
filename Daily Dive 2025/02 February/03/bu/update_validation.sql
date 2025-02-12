 
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
BEGIN
    -- Validate the PAN, Mobile, and Email formats first
    ISVALIDPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10))));
    ISVALIDMOBILE := VALIDATE_MOBILE(P_MOBILE_NO_VALUE);
    ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(P_EMAIL_VALUE));
    ISVALID_GPAN := VALIDATEPAN(UPPER(TRIM(SUBSTR(P_GUARDIAN_PERSON_PAN, 1, 10))));
    
    -- Validate DT NUMBER FLAG VALUE
    
    BEGIN -- Check for duplicate PAN globally
        SELECT check_dt_newinv(P_DT_NUMBER) into DT_FLAG  FROM dual;
        EXCEPTION WHEN NO_DATA_FOUND THEN DT_FLAG := 0;
    END;

    BEGIN -- Check for duplicate PAN globally
        SELECT count(pan) INTO VPAN FROM WEALTHMAKER.investor_master
        WHERE UPPER(TRIM(SUBSTR(pan, 1, 10))) = UPPER(TRIM(SUBSTR(P_CLIENT_PAN, 1, 10)));
        EXCEPTION WHEN NO_DATA_FOUND THEN VPAN := 0;
    END;
    
    

    
      -- validate mobile no of family sourcecode 
    BEGIN
        SELECT COUNT(MOBILE) 
        INTO VMOBILE 
        FROM WEALTHMAKER.investor_master
        WHERE MOBILE = P_MOBILE_NO_VALUE ;
        EXCEPTION WHEN NO_DATA_FOUND THEN VMOBILE := 0;
    END;
    
    -- Check if the same email exists for a different family (SOURCE_CODE)
    BEGIN
        SELECT COUNT(EMAIL) 
        INTO VEMAIL 
        FROM WEALTHMAKER.investor_master
        WHERE UPPER(EMAIL) = UPPER(P_EMAIL_VALUE);
        EXCEPTION WHEN NO_DATA_FOUND THEN VEMAIL := 0;
    END;

    -- Check if the same guardian pan exists for a different family (SOURCE_CODE)
    BEGIN
        SELECT COUNT(G_PAN) 
        INTO VG_PAN 
        FROM WEALTHMAKER.investor_master
        WHERE UPPER(G_PAN) = UPPER(P_GUARDIAN_PERSON_PAN);
        EXCEPTION WHEN NO_DATA_FOUND THEN VG_PAN := 0;
    END;
        

    -- If any validation fails, return an error message
    IF ISVALIDPAN = 'InValid' OR ISVALIDPAN = 'Miss' THEN
        OPEN P_RESULT FOR SELECT 'Invalid PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
        RETURN;
    end if;
    
    IF ISVALIDPAN = 'Valid' and VPAN > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate PAN' AS message FROM DUAL;
        RETURN;
    end if;
    
    IF ISVALIDMOBILE = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Mobile Number (e.g. 9999999999)' AS message FROM DUAL;
        RETURN;
    END IF;
    
    IF ISVALIDMOBILE = 1  AND VMOBILE > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Mobile Number (Cannot be shared across different families)' AS message FROM DUAL;
        RETURN;
    END IF;

 
    IF ISVALIDEMAIL = 0 THEN
        OPEN P_RESULT FOR SELECT 'Invalid Email' AS message FROM DUAL;
        RETURN;
     end if;   


    IF ISVALIDEMAIL = 1  AND VEMAIL > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Email (Cannot be shared across different families)' AS message FROM DUAL;
    END IF;

    -- If any validation fails, return an error message
    IF P_GUARDIAN_PERSON_PAN IS NOT NULL THEN
    IF ISVALID_GPAN = 'InValid' OR ISVALID_GPAN = 'Miss' THEN
        OPEN P_RESULT FOR SELECT 'Invalid Guardian PAN (e.g. AAAAA9999A)' AS message FROM DUAL;
        RETURN;
        END IF;
    end if;
    
    IF P_GUARDIAN_PERSON_PAN IS NOT NULL THEN
    IF ISVALID_GPAN = 'Valid' and VG_PAN > 0 THEN
        OPEN P_RESULT FOR SELECT 'Duplicate Guardian PAN (Cannot be shared across different families)' AS message FROM DUAL;
        RETURN;
        END IF;
    end if;

-- Validate DT number is not null and valid
IF TRIM(P_DT_NUMBER) IS NULL THEN
    OPEN P_RESULT FOR
        SELECT 'Please enter DT number for new account' AS message FROM DUAL;
    RETURN;  -- Exit the procedure here
ELSIF DT_FLAG = 0 THEN
    OPEN P_RESULT FOR
        SELECT 'Please Enter Valid DT Number' AS message FROM DUAL;
    RETURN;  -- Exit the procedure here
END IF;


-- Validate CLIENT_MASTER
SELECT nvl(COUNT(*),0)
INTO CLIENT_MASTER_FLAG
FROM tb_doc_upload 
WHERE 
common_id = P_DT_NUMBER and
SUBSTR(inv_code, 1, 8) IN (SELECT CLIENT_CODE FROM CLIENT_MASTER);

-- Validate CLIENT_TEST
SELECT nvl(COUNT(*),0)
INTO CLIENT_TEST_FLAG
FROM tb_doc_upload 
WHERE 
common_id = P_DT_NUMBER and
inv_code IN (SELECT CLIENT_CODEKYC FROM CLIENT_TEST);


-- Validate Business RM Code  
IF TRIM(P_RM_BUSINESS_CODE) IS NULL THEN
    OPEN P_RESULT FOR
        SELECT 'Please Enter Valid Business RM Code' AS message FROM DUAL;
    RETURN;
END IF;