CREATE OR REPLACE PROCEDURE PSM_UID_BY_INV (
    P_LOGIN     IN  VARCHAR2 DEFAULT NULL,
    P_ROLE      IN  VARCHAR2 DEFAULT NULL,
    P_INV       IN  VARCHAR2 DEFAULT NULL,
    P_ACTION    IN  VARCHAR2 DEFAULT NULL, -- '0' = GET, '1' = UPDATE
    P_TYPE      IN  VARCHAR2 DEFAULT NULL,

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
-- VB AND .CS INFO
/*
Label32 = INV_CODE

*/



V_MSG       VARCHAR2(4000)  :=NULL;
V_TEMP1     VARCHAR2(100)   :=NULL;
V_IS_FAM_HEAD VARCHAR2(20)  :=NULL;


BEGIN

IF P_ACTION = '0' THEN
    IF P_TYPE = 'frmARGeneral' THEN
    
    OPEN P_CURSOR FOR
    select nvl(a.address1,' '),
    nvl(a.address2,' '),
    nvl(b.city_id,' '),
    nvl(to_char(c.state_id),''),
    nvl(a.mobile,0),
    nvl(a.email,' '),
    nvl(a.pincode,' '),
    nvl(upper(a.pan),' '),
    a.aadhar_card_no,
    A.DOB,investor_name  
    from investor_master a,
    city_master b, 
    state_master c 
    where a.city_id=b.city_id(+) 
    and b.state_id=c.state_id(+) and a.inv_code=P_INV;

ELSIF P_ACTION = '1' THEN

    IF P_ROLE = '1' THEN
        V_MSG:= 'You are not authorised to update the details.';
    END IF;
        
    IF SUBSTR(P_INV,1,1) = '4' THEN
        
        SELECT CHECK_NUMBER(P_MOBILE) INTO V_TEMP1 
        FROM DUAL;
        IF V_TEMP1 = 'N' THEN
            V_MSG:='Invalid Mobile No.';
        end if;
    end if;

    IF P_MOBILE IS NOT NULL THEN
        SELECT MOBILE INTO V_TEMP1
        FROM INVESTOR_MASTER WHERE MOBILE = P_MOBILE 
        AND INV_CODE NOT LIKE '%'||P_INV||'%';
        
        IF V_TEMP1 IS NOT NULL THEN
            V_MSG:='Mobile Number already present!';
        END IF;
    END IF;
            
    IF P_PAN IS NOT NULL THEN
        SELECT VALIDATEPAN1(P_PAN) INTO V_TEMP1
        FROM DUAL;
        
        IF V_TEMP1 = '0' THEN
            V_MSG:='Invalid PAN';
        ELSE
            SELECT PAN INTO V_TEMP1
            FROM INVESTOR_MASTER 
            WHERE PAN = P_PAN
            AND INV_CODE NOT LIKE '%'||P_INV||'%';
            
            IF V_TEMP1 IS NOT NULL THEN
                V_MSG:='PAN already exists!';
            end if;
        end if;
    end if;

    IF P_EMAIL IS NOT NULL THEN
        IF UPPER(P_EMAIL) NOT IN ('NOT AVAILABLE', 'N/A', 'N A', 'NILL', 'NONE', 'N-A')THEN
            SELECT EMAIL INTO V_TEMP1 FROM INVESTOR_MASTER
            WHERE UPPER(EMAIL) = UPPER(P_EMAIL)
            AND INV_CODE NOT LIKE '%'||P_INV||'%';
            
            IF V_TEMP1 IS NOT NULL THEN
                V_MSG:='EMAIL already exists!';
            end if;
        end if;
    END IF;

    IF P_AADHAR IS NOT NULL THEN
        SELECT PSM_VALIDATEAADHAAR(P_AADHAR) INTO V_TEMP1 FROM DUAL;
        
        IF V_TEMP1 IS NOT NULL AND V_TEMP1 IN ( 'Invalid', 'Miss') THEN
            V_MSG:='Invalid Aadhar';
            
        elsif v_temp1 is not null and v_temp1 = 'Valid' then
            select aadhar_card_no into v_temp1 from investor_master
            where aadhar_card_no = p_aadhar
            and inv_code <> p_inv;
            
            if v_temp1 is not null then
                v_msg:='Aadhar Card Number already Exist!';
            end if;
        end if;
            
    END IF;
         
    
    IF V_MSG IS NOT NULL THEN
        OPEN P_CURSOR FOR 
            SELECT V_MSG FROM DUAL
            RETURN;
    END IF;
    
    
    
    IF SUBSTR(P_INV,1,1) = '3' THEN
        UPDATE INVESTOR_MASTER SET 
        MODIFY_USER     = P_LOGIN,
        MODIFY_DATE     = SYSDATE,
        aadhar_card_no  = P_AADHAR,
        PAN             = P_PAN,
        MOBILE          = P_MOBILE,
        EMAIL           = P_EMAIL,
        ADDRESS1        = P_ADD1,
        ADDRESS2        = P_ADD2,
        PINCODE         = P_PIN,
        CITY_ID         = P_CITY_ID
        WHERE INV_CODE  = P_INV;
    ELSE
        UPDATE INVESTOR_MASTER SET
        MODIFY_USER     = P_LOGIN,
        MODIFY_DATE     = SYSDATE,
        AADHAR_CARD_NO  = P_AADHAR,
        PAN             = P_PAN,
        MOBILE          = P_MOBILE,
        EMAIL           = P_EMAIL
        WHERE INV_CODE  = P_INV;
        
        UPDATE INVESTOR_MASTER SET
        MODIFY_USER     = P_LOGIN,
        MODIFY_DATE     = SYSDATE,
        ADDRESS1        = P_ADD1,
        ADDRESS2        = P_ADD2, 
        PINCODE         = P_PIN,
        CITY_ID         = P_CITY_ID
        WHERE SOURCE_ID = SUBSTR(P_INV,1,8);
    END IF;
    
    UPDATE CLIENT_TEST SET
    MODIFY_USER = P_LOGIN,
    MODIFY_DATE = SYSDATE,
    AADHAR_CARD_NO = P_AADHAR,
    CLIENT_PAN = P_PAN,
    MOBILE_NO = P_MOBILE,
    EMAIL = P_EMAIL
    WHERE CLIENT_CODEKYC = P_INV;
    
    IF P_DOB IS NOT NULL THEN
        UPDATE INVESTOR_MASTER SET
        MODIFY_USER = P_LOGIN,
        MODIFY_DATE = SYSDATE,
        DOB = TO_DATE(P_DOB,'DD/MM/YYYY')
        WHERE INV_CODE = P_INV;
        
        UPDATE CLIENT_TEST SET
        MODIFY_USER = P_LOGIN,
        MODIFY_DATE = SYSDATE,
        DOB = TO_DATE(P_DOB,'DD/MM/YYYY/')
        WHERE CLIENT_CODEKYC = P_INV;
    
    select IS_FAMILY_HEAD(P_INV) INTO V_IS_FAM_HEAD 
    from dual;    
    
    IF V_IS_FAM_HEAD = '1' THEN
        UPDATE CLIENT_MASTER T SET
        MODIFY_USER = P_LOGIN,
        MODIFY_DATE = SYSDATE,
        PAN= P_PAN,
        MOBILE = P_MOBILE,
        EMAIL = P_EMAIL,
        ADDRESS1 = P_ADD1,
        ADDRESS2 = P_ADD2,
        PINCODE = P_PIN,
        CITY_ID = P_CITY_ID
        WHERE CLIENT_CODE = SUBSTR(P_INV,1,8);
        
        IF P_DOB IS NOT NULL THEN
            UPDATE CLIENT_MASTER T
            SET MODIFY_USER = P_LOGIN,
            MODIFY_DATE = SYSDATE,
            DOB = TO_DATE(P_DOB,'DD/MM/YYYY')
            WHERE CLIENT_CODE = SUBSTR(P_INV,1,8);
        END IF;
    END IF;    
        
        
 
    OPEN P_CURSOR FOR 
        SELECT 'Information updated' FROM DUAL
        RETURN;
    END IF;
       
END IF;            


END IF;   



























    IF P_INV IS NOT NULL THEN
        -- GET ACTION
        IF P_ACTION = '0' THEN
            OPEN P_CURSOR FOR
                SELECT 
                    IM.ADDRESS1,
                    IM.ADDRESS2,
                    IM.PINCODE,
                    IM.MOBILE AS MOBILE,
                    IM.EMAIL AS EMAIL,
                    IM.AADHAR_CARD_NO,
                    IM.DOB,
                    IM.PAN,
                    IM.CITY_ID,
                    CM.STATE_ID
                FROM INVESTOR_MASTER IM
                LEFT JOIN CITY_MASTER CM ON IM.CITY_ID = CM.CITY_ID
                WHERE IM.INV_CODE = P_INV;

        -- UPDATE ACTION
        ELSIF P_ACTION = '1' THEN

            IF P_TYPE = 'ACCOUNT' THEN
                UPDATE CLIENT_TEST 
                SET CLIENT_CODE = 'AH001'
                WHERE CLIENT_CODEKYC = P_INV;

                OPEN P_CURSOR FOR 
                    SELECT 'CLIENT TEST UPDATED BY INV CODE' AS MESSAGE FROM DUAL;

            ELSIF P_TYPE = 'NPS' THEN
                UPDATE NPS_TRANSACTION 
                SET REMARK = 'UPDATED BY INV'
                WHERE TRAN_CODE = P_INV;

                OPEN P_CURSOR FOR 
                    SELECT 'NPS UPDATED BY INV CODE' AS MESSAGE FROM DUAL;

            ELSIF P_TYPE = 'INVESTOR' THEN
                UPDATE INVESTOR_MASTER 
                SET 
                    ADDRESS1    = NVL(P_ADD1, ADDRESS1),
                    ADDRESS2    = NVL(P_ADD2, ADDRESS2),
                    PINCODE     = NVL(P_PIN, PINCODE),
                    EMAIL    = NVL(P_EMAIL, EMAIL),
                    MOBILE   = NVL(P_MOBILE, MOBILE),
                    PAN      = NVL(P_PAN, PAN),
                    AADHAR_CARD_NO   = NVL(P_AADHAR, AADHAR_CARD_NO),
                    --STATE_ID    = NVL(P_STATE_ID, STATE_ID),
                    CITY_ID     = NVL(P_CITY_ID, CITY_ID),
                    DOB         = NVL(TO_DATE(P_DOB, 'DD-MM-YYYY'), DOB)
                WHERE INV_CODE = P_INV;

                OPEN P_CURSOR FOR 
                    SELECT 'INVESTOR DETAILS UPDATED SUCCESSFULLY' AS MESSAGE FROM DUAL;
            END IF;
        END IF;
    END IF;
END;
/
