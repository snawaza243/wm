CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_UID_BY_INV (
    P_LOGIN     IN  VARCHAR2 DEFAULT NULL,
    P_ROLE      IN  VARCHAR2 DEFAULT NULL,
    P_INV       IN  VARCHAR2 DEFAULT NULL,
    P_ACTION    IN  VARCHAR2 DEFAULT NULL, -- '0' = GET, '1' = UPDATE
    P_TYPE      IN  VARCHAR2 DEFAULT NULL, -- frmARGeneral

    P_ADD1      IN VARCHAR2 DEFAULT NULL,
    P_ADD2      IN VARCHAR2 DEFAULT NULL,
    P_PIN       IN VARCHAR2 DEFAULT NULL,
    P_EMAIL     IN VARCHAR2 DEFAULT NULL,
    P_MOBILE    IN VARCHAR2 DEFAULT NULL,
    P_PAN       IN VARCHAR2 DEFAULT NULL,
    P_AADHAR    IN VARCHAR2 DEFAULT NULL,
    P_STATE_ID  IN VARCHAR2 DEFAULT NULL,
    P_CITY_ID   IN VARCHAR2 DEFAULT NULL,
    P_DOB       IN VARCHAR2 DEFAULT NULL,

    P_CURSOR    OUT SYS_REFCURSOR   
)
AS
    V_MSG           VARCHAR2(4000)  := NULL;
    V_TEMP1         VARCHAR2(100)   := NULL;
    V_COUNT         NUMBER;
    V_IS_FAM_HEAD   VARCHAR2(20)    := NULL;
BEGIN
    IF P_ACTION = '0' THEN -- GET
        IF P_TYPE = 'frmARGeneral' THEN    
            OPEN P_CURSOR FOR
                SELECT NVL(A.inv_code,NULL) AS INVESTOR_CODE,
                NVL(A.ADDRESS1,'')          AS INVESTOR_ADD1,
                NVL(A.ADDRESS2,'')          AS INVESTOR_ADD2,
                NVL(B.CITY_ID,'')           AS INVESTOR_CITY_ID, 
                NVL(TO_CHAR(C.STATE_ID),'') AS INVESTOR_STATE_ID,
                NVL(A.EMAIL,'')             AS INVESTOR_EMAIL,
                A.MOBILE                    AS INVESTOR_MOBILE,
                NVL(A.PINCODE,'')           AS INVESTOR_PINCODE,
                NVL(UPPER(A.PAN),'')        AS INVESTOR_PAN,
                A.AADHAR_CARD_NO            AS INVESTOR_AADHAR,
                A.DOB                       AS INVESTOR_DOB,
                INVESTOR_NAME               AS INVESTOR_NAME
                FROM INVESTOR_MASTER A,
                CITY_MASTER B, 
                STATE_MASTER C 
                WHERE A.CITY_ID=B.CITY_ID(+) 
                AND B.STATE_ID=C.STATE_ID(+) 
                AND A.INV_CODE=P_INV;
        END IF;
    ELSIF P_ACTION = '1' THEN -- UPDATE
        -- Authorization check
        IF P_ROLE = '1' THEN
            V_MSG:= 'You are not authorised to update the details.';
        END IF;
            
        -- Mobile validation for investor type 4
        IF SUBSTR(P_INV,1,1) = '4' THEN
            SELECT CHECK_NUMBER(P_MOBILE) INTO V_TEMP1 FROM DUAL;
            IF V_TEMP1 = 'N' THEN
                V_MSG:= NVL(V_MSG, '') || ' Invalid Mobile No.';
            END IF;
        END IF;


        -- Mobile uniqueness check
        IF P_MOBILE IS NOT NULL THEN
            BEGIN
                SELECT 1 INTO V_COUNT
                FROM DUAL
                WHERE EXISTS (
                    SELECT 1 FROM INVESTOR_MASTER 
                    WHERE MOBILE = P_MOBILE 
                    --AND INV_CODE != P_INV AND ROWNUM =1
                    AND source_id != substr(P_INV,1,8)
                );
                V_MSG:= NVL(V_MSG, '') || ' Mobile Number already exists!';
            EXCEPTION
                WHEN NO_DATA_FOUND THEN NULL;
            END;
        END IF;


        -- PAN validation
        IF P_PAN IS NOT NULL THEN
            SELECT VALIDATEPAN1(P_PAN) INTO V_TEMP1 FROM DUAL;
            
            IF V_TEMP1 = '0' THEN
                V_MSG:= NVL(V_MSG, '') || ' Invalid PAN';
            ELSE
                BEGIN
                    SELECT PAN INTO V_TEMP1
                    FROM INVESTOR_MASTER 
                    WHERE PAN = P_PAN
                    AND INV_CODE != P_INV AND ROWNUM =1;                     
                    V_MSG:= NVL(V_MSG, '') || ' PAN already exists!';
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN NULL;
                END;
            END IF;
        END IF; 
        -- Email validation
        IF P_EMAIL IS NOT NULL THEN
            IF UPPER(P_EMAIL) NOT IN ('NOT AVAILABLE', 'N/A', 'N A', 'NILL', 'NONE', 'N-A') THEN             
                BEGIN
                    SELECT EMAIL INTO V_TEMP1 FROM INVESTOR_MASTER
                    WHERE UPPER(EMAIL) = UPPER(P_EMAIL)
                    AND INV_CODE != P_INV AND ROWNUM = 1;
                    V_MSG:= NVL(V_MSG, '') || ' Email already exists!';
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN NULL;
                END;
            END IF;
        END IF;
   
       
        -- Aadhar validation
        IF P_AADHAR IS NOT NULL THEN
            SELECT PSM_VALIDATEAADHAAR(P_AADHAR) INTO V_TEMP1 FROM DUAL;
            
            IF V_TEMP1 IN ('Invalid', 'Miss') THEN
                V_MSG:= NVL(V_MSG, '') || ' Invalid Aadhar';
            ELSIF V_TEMP1 = 'Valid' THEN
                BEGIN
                    SELECT AADHAR_CARD_NO INTO V_TEMP1 
                    FROM INVESTOR_MASTER
                    WHERE AADHAR_CARD_NO = P_AADHAR
                    AND INV_CODE != P_INV AND ROWNUM =1;
                    
                    V_MSG:= NVL(V_MSG, '') || ' Aadhar already exists!';
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN NULL;
                END;
            END IF;
        END IF;
        
    
             
        -- Return errors if any
        IF V_MSG IS NOT NULL THEN
            OPEN P_CURSOR FOR 
                SELECT V_MSG AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
        
        -- Update investor data based on type
        IF SUBSTR(P_INV,1,1) = '3' THEN
            UPDATE INVESTOR_MASTER SET 
            MODIFY_USER     = P_LOGIN,
            MODIFY_DATE     = SYSDATE,
            AADHAR_CARD_NO  = P_AADHAR,
            PAN             = P_PAN,
            MOBILE          = P_MOBILE,
            EMAIL           = P_EMAIL,
            ADDRESS1        = P_ADD1,
            ADDRESS2        = P_ADD2,
            PINCODE         = P_PIN,
            CITY_ID         = P_CITY_ID
            WHERE INV_CODE  = P_INV;
        ELSE
            -- Update investor master
            UPDATE INVESTOR_MASTER SET
            MODIFY_USER     = P_LOGIN,
            MODIFY_DATE     = SYSDATE,
            AADHAR_CARD_NO  = P_AADHAR,
            PAN             = P_PAN,
            MOBILE          = P_MOBILE,
            EMAIL           = P_EMAIL
            WHERE INV_CODE  = P_INV;
            
            -- Update address for non-type 3 investors
            UPDATE INVESTOR_MASTER SET
            MODIFY_USER     = P_LOGIN,
            MODIFY_DATE     = SYSDATE,
            ADDRESS1        = P_ADD1,
            ADDRESS2        = P_ADD2, 
            PINCODE         = P_PIN,
            CITY_ID         = P_CITY_ID
            WHERE SOURCE_ID = SUBSTR(P_INV,1,8);
        END IF;
        
        -- Update client test table
        UPDATE CLIENT_TEST SET
        MODIFY_USER     = P_LOGIN,
        MODIFY_DATE     = SYSDATE,
        AADHAR_CARD_NO  = P_AADHAR,
        CLIENT_PAN      = P_PAN,
        MOBILE_NO       = P_MOBILE,
        EMAIL           = P_EMAIL
        WHERE CLIENT_CODEKYC = P_INV;
        
        -- Handle DOB updates
        IF P_DOB IS NOT NULL THEN
            UPDATE INVESTOR_MASTER SET
            MODIFY_USER = P_LOGIN,
            MODIFY_DATE = SYSDATE,
            DOB         = TO_DATE(P_DOB,'DD/MM/YYYY')
            WHERE INV_CODE = P_INV;
            
            UPDATE CLIENT_TEST SET
            MODIFY_USER = P_LOGIN,
            MODIFY_DATE = SYSDATE,
            DOB         = TO_DATE(P_DOB,'DD/MM/YYYY')
            WHERE CLIENT_CODEKYC = P_INV;
        
            -- Check if family head
            SELECT IS_FAMILY_HEAD(P_INV) INTO V_IS_FAM_HEAD FROM DUAL;    
            
            IF V_IS_FAM_HEAD = '1' THEN
                UPDATE CLIENT_MASTER T SET
                MODIFY_USER = P_LOGIN,
                MODIFY_DATE = SYSDATE,
                PAN         = P_PAN,
                MOBILE      = P_MOBILE,
                EMAIL       = P_EMAIL,
                ADDRESS1    = P_ADD1,
                ADDRESS2    = P_ADD2,
                PINCODE     = P_PIN,
                CITY_ID     = P_CITY_ID
                WHERE CLIENT_CODE = SUBSTR(P_INV,1,8);
                
                UPDATE CLIENT_MASTER T
                SET MODIFY_USER = P_LOGIN,
                MODIFY_DATE     = SYSDATE,
                DOB             = TO_DATE(P_DOB,'DD/MM/YYYY')
                WHERE CLIENT_CODE = SUBSTR(P_INV,1,8);
            END IF;
        END IF;            
     
        OPEN P_CURSOR FOR 
            SELECT 'Information updated successfully!' AS MESSAGE FROM DUAL;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        V_MSG := 'Error: ' || SQLERRM;
        OPEN P_CURSOR FOR 
            SELECT V_MSG AS MESSAGE FROM DUAL;
END PSM_UID_BY_INV;
/
