CREATE OR REPLACE PROCEDURE PSMM_INV_ADD_UP_SET_DATA (
    P_INV_CODE     IN VARCHAR2,
    P_MOBILE       IN VARCHAR2,
    P_PAN          IN VARCHAR2,
    P_EMAIL        IN VARCHAR2,
    P_AADHAR       IN VARCHAR2,
    P_ADDRESS1     IN VARCHAR2,
    P_ADDRESS2     IN VARCHAR2,
    P_PINCODE      IN VARCHAR2,
    P_CITY_ID      IN VARCHAR2,
    P_STATE_ID     IN VARCHAR2,
    P_DOB          IN VARCHAR2,
    P_LOGIN        IN VARCHAR2,
    P_ROLE         IN VARCHAR2,
    P_CURSOR       OUT SYS_REFCURSOR
) AS
    V_ERROR             VARCHAR2(4000);
    V_CHECK_DUPLICATE   NUMBER;
    V_CHECK_NUMBER      VARCHAR2(5);
    V_DOB               DATE;
    V_IS_FAMILY_HEAD    VARCHAR2(1);
BEGIN
    SAVEPOINT sp_find;

    /*
    OPEN P_CURSOR FOR 
    SELECT
        'P_INV_CODE: ' || P_INV_CODE || ', P_MOBILE: ' || P_MOBILE || ', P_PAN: ' || P_PAN || ', P_EMAIL: ' || P_EMAIL || ', P_AADHAR: ' 
        || P_AADHAR || ', P_ADDRESS1: ' || P_ADDRESS1 || ', P_ADDRESS2: ' || P_ADDRESS2 || ', P_PINCODE: ' || P_PINCODE || ', P_CITY_ID: ' 
        || P_CITY_ID || ', P_STATE_ID: ' || P_STATE_ID || ', P_DOB: ' || P_DOB || ', P_LOGIN: ' || P_LOGIN || ', P_ROLE: ' || P_ROLE AS msg FROM DUAL;
    RETURN;
    */


    -- Check if user is family head
    SELECT IS_FAMILY_HEAD(P_INV_CODE) INTO V_IS_FAMILY_HEAD FROM DUAL;

    -- Role Authorization
    IF P_ROLE = '1' THEN
        OPEN P_CURSOR FOR SELECT 'You are not authorised to update the details' msg FROM DUAL;
        RETURN;
    END IF;

    -- MOBILE Validation
    IF P_MOBILE IS NOT NULL THEN
        IF SUBSTR(P_INV_CODE,1,1) = '4' THEN
            SELECT check_number(P_MOBILE) INTO V_CHECK_NUMBER FROM DUAL;
            IF V_CHECK_NUMBER = 'N' THEN
                OPEN P_CURSOR FOR SELECT 'Invalid Mobile No' msg FROM DUAL;
                RETURN;
            END IF;
        END IF;

        SELECT COUNT(*) INTO V_CHECK_DUPLICATE
        FROM investor_master
        WHERE MOBILE = TRIM(P_MOBILE)
          AND inv_code NOT LIKE '%' || SUBSTR(P_INV_CODE,1,8) || '%';

        IF V_CHECK_DUPLICATE > 0 THEN
            OPEN P_CURSOR FOR SELECT 'Mobile Number already present!' msg FROM DUAL;
            RETURN;
        END IF;
    END IF;



    -- PAN Validation
    IF P_PAN IS NOT NULL THEN
        IF NOT REGEXP_LIKE(P_PAN, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
            OPEN P_CURSOR FOR SELECT 'Invalid PAN Format' msg FROM DUAL;
            RETURN;
        END IF;

        SELECT COUNT(*) INTO V_CHECK_DUPLICATE
        FROM investor_master
        WHERE UPPER(TRIM(PAN)) = UPPER(TRIM(P_PAN))
          AND inv_code NOT LIKE '%' || SUBSTR(P_INV_CODE,1,8) || '%';

        IF V_CHECK_DUPLICATE > 0 THEN
            OPEN P_CURSOR FOR SELECT 'PAN already present!' msg FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- EMAIL Validation
    IF P_EMAIL IS NOT NULL THEN
        IF UPPER(P_EMAIL) NOT IN ('NOT AVAILABLE', 'N/A', 'N A', 'NILL', 'NONE', 'N-A') THEN
            DECLARE ISVALIDEMAIL NUMBER;
            BEGIN
                ISVALIDEMAIL := VALIDATE_EMAIL(UPPER(P_EMAIL));
                IF ISVALIDEMAIL = 0 THEN
                    OPEN P_CURSOR FOR SELECT 'Invalid Email Format' msg FROM DUAL;
                    RETURN;
                END IF;

                SELECT COUNT(*) INTO V_CHECK_DUPLICATE
                FROM investor_master
                WHERE UPPER(TRIM(EMAIL)) = UPPER(TRIM(P_EMAIL))
                  AND inv_code NOT LIKE '%' || SUBSTR(P_INV_CODE,1,8) || '%';

                IF V_CHECK_DUPLICATE > 0 THEN
                    OPEN P_CURSOR FOR SELECT 'Email already present!' msg FROM DUAL;
                    RETURN;
                END IF;
            END;
        END IF;
    END IF;

    -- AADHAR Validation
    IF P_AADHAR IS NOT NULL THEN
        IF NOT REGEXP_LIKE(P_AADHAR, '^[0-9]{12}$') THEN
            OPEN P_CURSOR FOR SELECT 'Invalid Aadhar Format' msg FROM DUAL;
            RETURN;
        END IF;

        SELECT COUNT(*) INTO V_CHECK_DUPLICATE
        FROM investor_master
        WHERE AADHAR_CARD_NO = TRIM(P_AADHAR)
          AND inv_code NOT LIKE '%' || SUBSTR(P_INV_CODE,1,8) || '%';

        IF V_CHECK_DUPLICATE > 0 THEN
            OPEN P_CURSOR FOR SELECT 'Aadhar already present!' msg FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- DOB Validation
    IF P_DOB IS NOT NULL THEN
        IF NOT REGEXP_LIKE(P_DOB, '^\d{2}/\d{2}/\d{4}$') THEN
            OPEN P_CURSOR FOR SELECT 'Invalid Date of Birth Format' msg FROM DUAL;
            RETURN;
        END IF;

        V_DOB := TO_DATE(P_DOB, 'DD/MM/YYYY');
        IF V_DOB > SYSDATE THEN
            OPEN P_CURSOR FOR SELECT 'Date of Birth cannot be in the future' msg FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Update INVESTOR_MASTER
    IF SUBSTR(P_INV_CODE,1,1) = '3' THEN
        UPDATE INVESTOR_MASTER
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE,
            AADHAR_CARD_NO = TRIM(P_AADHAR), PAN = TRIM(P_PAN),
            MOBILE = TRIM(P_MOBILE), EMAIL = TRIM(P_EMAIL),
            ADDRESS1 = TRIM(P_ADDRESS1), ADDRESS2 = TRIM(P_ADDRESS2),
            PINCODE = TRIM(P_PINCODE), CITY_ID = P_CITY_ID
        WHERE inv_code = P_INV_CODE;
    ELSE
        OPEN P_CURSOR FOR SELECT 'SUCCESS: 2 I AM IN CLINET ' || SUBSTR(P_INV_CODE,1,1) as  msg FROM DUAL; RETURN;

        UPDATE INVESTOR_MASTER
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE,
            AADHAR_CARD_NO = TRIM(P_AADHAR), PAN = TRIM(P_PAN),
            MOBILE = TRIM(P_MOBILE), EMAIL = TRIM(P_EMAIL)
        WHERE inv_code = P_INV_CODE;

        UPDATE INVESTOR_MASTER
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE,
            ADDRESS1 = TRIM(P_ADDRESS1), ADDRESS2 = 'SASD',--TRIM(P_ADDRESS2),
            PINCODE = TRIM(P_PINCODE), CITY_ID = P_CITY_ID
        WHERE SOURCE_ID = SUBSTR(P_INV_CODE,1,8);

        UPDATE CLIENT_TEST
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE,
            ADD1 = TRIM(P_ADDRESS1), ADD2 = TRIM(P_ADDRESS2),
            PINCODE = TRIM(P_PINCODE), CITY_ID = P_CITY_ID, STATE_ID = P_STATE_ID
        WHERE SOURCE_CODE = SUBSTR(P_INV_CODE,1,8);
    END IF;

    -- Update CLIENT_TEST with personal info
    UPDATE CLIENT_TEST
    SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE,
        AADHAR_CARD_NO = TRIM(P_AADHAR), CLIENT_PAN = TRIM(P_PAN),
        MOBILE_NO = TRIM(P_MOBILE), EMAIL = TRIM(P_EMAIL)
    WHERE client_codekyc = P_INV_CODE;

    -- Update DOB if valid
    IF P_DOB IS NOT NULL THEN
        UPDATE INVESTOR_MASTER
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE, DOB = V_DOB
        WHERE inv_code = P_INV_CODE;

        UPDATE CLIENT_TEST
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE, DOB = V_DOB
        WHERE client_codekyc = P_INV_CODE;
    END IF;

    -- Update CLIENT_MASTER if family head
    IF V_IS_FAMILY_HEAD = '1' THEN
        UPDATE CLIENT_MASTER
        SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE,
            PAN = TRIM(P_PAN), MOBILE = TRIM(P_MOBILE),
            EMAIL = TRIM(P_EMAIL), ADDRESS1 = TRIM(P_ADDRESS1),
            ADDRESS2 = TRIM(P_ADDRESS2), PINCODE = TRIM(P_PINCODE), CITY_ID = P_CITY_ID
        WHERE client_code = SUBSTR(P_INV_CODE,1,8);

        IF P_DOB IS NOT NULL THEN
            UPDATE CLIENT_MASTER
            SET MODIFY_USER = P_LOGIN, MODIFY_DATE = SYSDATE, DOB = V_DOB
            WHERE client_code = SUBSTR(P_INV_CODE,1,8);
        END IF;
    END IF;

    COMMIT;
    OPEN P_CURSOR FOR SELECT 'SUCCESS: 2 Information Updated Successfully: ' || ADDRESS1 as  msg FROM investor_master where inv_code = P_INV_CODE;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO sp_find;
        V_ERROR := 'ERROR: ' || SQLERRM;
        OPEN P_CURSOR FOR SELECT V_ERROR msg FROM DUAL;
END;
/