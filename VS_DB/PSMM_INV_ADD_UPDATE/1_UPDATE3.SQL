CREATE OR REPLACE PROCEDURE PSMM_INV_ADD_UP_SET_DATA (
    P_INV_CODE        IN VARCHAR2,
    P_MOBILE          IN VARCHAR2,
    P_PAN             IN VARCHAR2,
    P_EMAIL           IN VARCHAR2,
    P_AADHAR          IN VARCHAR2,
    P_ADDRESS1        IN VARCHAR2,
    P_ADDRESS2        IN VARCHAR2,
    P_PINCODE         IN VARCHAR2,
    P_CITY_ID         IN VARCHAR2,
    P_STATE_ID        IN VARCHAR2,
    P_DOB             IN VARCHAR2,
    P_LOGIN           IN VARCHAR2,
    P_ROLE            IN VARCHAR2,
    P_CURSOR          OUT SYS_REFCURSOR
) AS
    V_ERROR             VARCHAR2(4000);
    V_CHECK_DUPLICATE   NUMBER;
    V_CHECK_NUMBER      VARCHAR2(5);
    V_DOB               DATE;
    V_IS_FAMILY_HEAD    VARCHAR2(1);
    V_SOURCE_CODE       VARCHAR2(8);
    V_SOURCE_ID         VARCHAR2(8);
    V_INV_PREFIX        VARCHAR2(8);
BEGIN 
    -- Initialize variables
    V_INV_PREFIX := SUBSTR(P_INV_CODE, 1, 8);
    
    IF P_INV_CODE IS NOT NULL AND LENGTH(P_INV_CODE) > 8 THEN
        V_SOURCE_CODE := V_INV_PREFIX;
        V_SOURCE_ID := V_INV_PREFIX;
    END IF;
    
    SAVEPOINT sp_find;

    -- Get family head status once
    SELECT IS_FAMILY_HEAD(P_INV_CODE) INTO V_IS_FAMILY_HEAD FROM DUAL;

    -- Role check
    IF P_ROLE = '1' THEN 
        OPEN P_CURSOR FOR SELECT 'You are not authorised to update the details' msg FROM DUAL;
        RETURN;
    END IF;

    -- Mobile validation for INV_CODE starting with 4
    IF SUBSTR(P_INV_CODE,1,1) = '4' AND P_MOBILE IS NOT NULL THEN    
        SELECT check_number(P_MOBILE) INTO V_CHECK_NUMBER FROM DUAL;
        IF V_CHECK_NUMBER = 'N' THEN
            OPEN P_CURSOR FOR SELECT 'Invalid Mobile No' msg FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Single query for all duplicate checks
    BEGIN
        -- Check mobile duplicate
        IF P_MOBILE IS NOT NULL THEN 
            SELECT COUNT(*) INTO V_CHECK_DUPLICATE 
            FROM investor_master 
            WHERE MOBILE = P_MOBILE 
            AND inv_code NOT LIKE '%' || V_INV_PREFIX || '%'
            AND ROWNUM = 1;
            
            IF V_CHECK_DUPLICATE > 0 THEN
                OPEN P_CURSOR FOR SELECT 'Mobile Number already present!' msg FROM DUAL;
                RETURN;
            END IF;
        END IF;

        -- PAN Validation and duplicate check
        IF P_PAN IS NOT NULL THEN
            IF NOT REGEXP_LIKE(P_PAN, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                OPEN P_CURSOR FOR SELECT 'Invalid PAN Format' msg FROM DUAL;
                RETURN;
            ELSE
                SELECT COUNT(*) INTO V_CHECK_DUPLICATE 
                FROM investor_master 
                WHERE UPPER(TRIM(PAN)) = UPPER(TRIM(P_PAN)) 
                AND inv_code NOT LIKE '%' || V_INV_PREFIX || '%'
                AND ROWNUM = 1;
                
                IF V_CHECK_DUPLICATE > 0 THEN
                    OPEN P_CURSOR FOR SELECT 'PAN already present!' msg FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        -- Email Validation and duplicate check
        IF P_EMAIL IS NOT NULL THEN
            DECLARE
                V_EMAIL_UPPER VARCHAR2(100) := UPPER(TRIM(P_EMAIL));
                ISVALIDEMAIL NUMBER;
            BEGIN
                IF V_EMAIL_UPPER NOT IN ('NOT AVAILABLE', 'N/A', 'N A', 'NILL', 'NONE', 'N-A') THEN
                    ISVALIDEMAIL := VALIDATE_EMAIL(V_EMAIL_UPPER);
                    IF ISVALIDEMAIL = 0 THEN
                        OPEN P_CURSOR FOR SELECT 'Invalid Email Format' AS msg FROM DUAL; 
                        RETURN;
                    ELSIF ISVALIDEMAIL > 0 THEN
                        SELECT COUNT(*) INTO V_CHECK_DUPLICATE 
                        FROM investor_master 
                        WHERE UPPER(TRIM(EMAIL)) = V_EMAIL_UPPER 
                        AND inv_code NOT LIKE '%' || V_INV_PREFIX || '%'
                        AND ROWNUM = 1;
                        
                        IF V_CHECK_DUPLICATE > 0 THEN
                            OPEN P_CURSOR FOR SELECT 'Email already present!' msg FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;
                END IF;
            END;
        END IF;

        -- Aadhar Validation and duplicate check
        IF P_AADHAR IS NOT NULL THEN
            IF NOT REGEXP_LIKE(P_AADHAR, '^[0-9]{12}$') THEN
                OPEN P_CURSOR FOR SELECT 'Invalid Aadhar Format' msg FROM DUAL;
                RETURN;
            ELSE
                SELECT COUNT(*) INTO V_CHECK_DUPLICATE 
                FROM investor_master 
                WHERE AADHAR_CARD_NO = TRIM(P_AADHAR) 
                AND inv_code NOT LIKE '%' || V_INV_PREFIX || '%'
                AND ROWNUM = 1;
                
                IF V_CHECK_DUPLICATE > 0 THEN
                    OPEN P_CURSOR FOR SELECT 'Aadhar already present!' msg FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;
    END;

    -- DOB Validation (consolidated)
    IF P_DOB IS NOT NULL THEN
        BEGIN
            IF NOT REGEXP_LIKE(P_DOB, '^\d{2}/\d{2}/\d{4}$') THEN
                OPEN P_CURSOR FOR SELECT 'Invalid Date of Birth Format' msg FROM DUAL;
                RETURN;
            END IF;
            
            V_DOB := TO_DATE(P_DOB, 'DD/MM/YYYY');
            IF V_DOB > SYSDATE THEN
                OPEN P_CURSOR FOR SELECT 'Date of Birth cannot be in the future' msg FROM DUAL;
                RETURN;
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                OPEN P_CURSOR FOR SELECT 'Invalid Date of Birth' msg FROM DUAL;
                RETURN;
        END;
    END IF;

    -- Main update logic
    IF SUBSTR(P_INV_CODE,1,1) = '3' THEN
        -- Single update for type '3'
        UPDATE INVESTOR_MASTER 
        SET MODIFY_USER = P_LOGIN, 
            MODIFY_DATE = SYSDATE, 
            AADHAR_CARD_NO = TRIM(P_AADHAR), 
            PAN = TRIM(P_PAN), 
            MOBILE = TRIM(P_MOBILE), 
            EMAIL = TRIM(P_EMAIL), 
            ADDRESS1 = TRIM(P_ADDRESS1), 
            ADDRESS2 = TRIM(P_ADDRESS2), 
            PINCODE = TRIM(P_PINCODE), 
            CITY_ID = P_CITY_ID,
            DOB = CASE WHEN P_DOB IS NOT NULL THEN TO_DATE(P_DOB,'DD/MM/YYYY') ELSE DOB END
        WHERE inv_code = P_INV_CODE;
    ELSE
        -- Update investor_master (personal data)
        UPDATE INVESTOR_MASTER 
        SET MODIFY_USER = P_LOGIN, 
            MODIFY_DATE = SYSDATE, 
            AADHAR_CARD_NO = TRIM(P_AADHAR), 
            PAN = TRIM(P_PAN), 
            MOBILE = TRIM(P_MOBILE), 
            EMAIL = TRIM(P_EMAIL),
            DOB = CASE WHEN P_DOB IS NOT NULL THEN TO_DATE(P_DOB,'DD/MM/YYYY') ELSE DOB END
        WHERE inv_code = P_INV_CODE;

        -- Update investor_master (common data)
        UPDATE INVESTOR_MASTER 
        SET MODIFY_USER = P_LOGIN, 
            MODIFY_DATE = SYSDATE, 
            ADDRESS1 = TRIM(P_ADDRESS1), 
            ADDRESS2 = TRIM(P_ADDRESS2), 
            PINCODE = TRIM(P_PINCODE), 
            CITY_ID = P_CITY_ID
        WHERE SOURCE_ID = V_INV_PREFIX;

        -- Update client_test (personal data)
        UPDATE CLIENT_TEST 
        SET MODIFY_USER = P_LOGIN, 
            MODIFY_DATE = SYSDATE, 
            AADHAR_CARD_NO = TRIM(P_AADHAR), 
            CLIENT_PAN = TRIM(P_PAN), 
            MOBILE_NO = TRIM(P_MOBILE), 
            EMAIL = TRIM(P_EMAIL),
            DOB = CASE WHEN P_DOB IS NOT NULL THEN TO_DATE(P_DOB,'DD/MM/YYYY') ELSE DOB END
        WHERE client_codekyc = P_INV_CODE;
        
        -- Update client_test (common data)
        UPDATE CLIENT_TEST 
        SET MODIFY_USER = P_LOGIN, 
            MODIFY_DATE = SYSDATE, 
            ADD1 = TRIM(P_ADDRESS1), 
            ADD2 = TRIM(P_ADDRESS2), 
            PINCODE = TRIM(P_PINCODE), 
            CITY_ID = P_CITY_ID, 
            STATE_ID = P_STATE_ID
        WHERE SOURCE_CODE = V_INV_PREFIX;
    END IF;

    -- Family Head updates
    IF V_IS_FAMILY_HEAD = '1' THEN
        UPDATE CLIENT_MASTER 
        SET MODIFY_USER = P_LOGIN, 
            MODIFY_DATE = SYSDATE, 
            PAN = TRIM(P_PAN), 
            MOBILE = TRIM(P_MOBILE), 
            EMAIL = TRIM(P_EMAIL), 
            ADDRESS1 = TRIM(P_ADDRESS1), 
            ADDRESS2 = TRIM(P_ADDRESS2), 
            PINCODE = TRIM(P_PINCODE), 
            CITY_ID = P_CITY_ID,
            DOB = CASE WHEN P_DOB IS NOT NULL THEN TO_DATE(P_DOB,'DD/MM/YYYY') ELSE DOB END
        WHERE client_code = V_INV_PREFIX;
    END IF;

    -- Single commit at the end
    COMMIT;

    --OPEN P_CURSOR FOR SELECT 'SUCCESS: Information Updated Successfully' msg FROM DUAL;
      
    OPEN P_CURSOR FOR 
        --SELECT 'SUCCESS: P_INV_CODE: ' || P_INV_CODE || ', P_MOBILE: ' || P_MOBILE || ', P_PAN: ' || P_PAN || ', P_EMAIL: ' || P_EMAIL || ', P_AADHAR: ' || P_AADHAR || ', P_ADDRESS1: ' || P_ADDRESS1 || ', P_ADDRESS2: ' || P_ADDRESS2 || ', P_PINCODE: ' || P_PINCODE || ', P_CITY_ID: ' || P_CITY_ID || ', P_STATE_ID: ' || P_STATE_ID || ', P_DOB: ' || P_DOB || ', P_LOGIN: ' || P_LOGIN || ', P_ROLE: ' || P_ROLE AS msg FROM DUAL;
        SELECT  'SUCCESS: ADD1: '|| NVL(A.ADDRESS1,' ')          || 'ADD2: ' ||NVL(A.ADDRESS2,' ')   || 'CITY NAME: ' || NVL(B.CITY_NAME,' ') || 'STATE NAME:' || NVL(C.STATE_NAME,' ')  ||'MOBILE: ' || NVL(A.MOBILE,0)     || 'EMAIL: ' ||NVL(A.EMAIL,' ') || 'PINCODE: ' ||NVL(A.PINCODE,' ')        ||'PAN: ' ||NVL(UPPER(A.PAN),' ')   ||'ASDF: ' || A.AADHAR_CARD_NO || 'ASDF: ' || A.DOB,INVESTOR_NAME         ||'ASDF: ' || INV_CODE  ||'ASDF: '  || A.CITY_ID  AS MSG FROM INVESTOR_MASTER A,CITY_MASTER B, STATE_MASTER C WHERE A.CITY_ID=B.CITY_ID(+) AND B.STATE_ID=C.STATE_ID(+) AND A.INV_CODE=P_INV_CODE;
    RETURN;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO sp_find;
        V_ERROR := 'ERROR:' || SQLERRM;
        OPEN P_CURSOR FOR SELECT V_ERROR msg FROM DUAL;
END;
/