CREATE OR REPLACE PROCEDURE WEALTHMAKER.ADDRESSUPDATE_V2(
    P_INVCODE        VARCHAR2,
    PP_ADDRESS        VARCHAR2,
    PP_ADDRESS2       VARCHAR2,
    P_STATEID        VARCHAR2,
    P_CITYID         VARCHAR2,
    P_PINNO          VARCHAR2,
    P_MOBILE         VARCHAR2,
    P_PANNO          VARCHAR2,
    P_AADHARNO       VARCHAR2,
    P_DOB            VARCHAR2,
    P_EMAIL          VARCHAR2,
    P_USERID         VARCHAR2,   
    DBRESPONSE       OUT VARCHAR2
) 

AS
    V_INVCOUNT    NUMBER;
    V_MOBILECOUNT NUMBER;
    V_PANCOUNT    NUMBER;
    V_EMAILCOUNT  NUMBER;
    V_AADHARCOUNT NUMBER;
    V_DOB         DATE;  
BEGIN
   
    SELECT COUNT(*) INTO V_INVCOUNT
    FROM INVESTOR_MASTER 
    WHERE INV_CODE = P_INVCODE;

    IF V_INVCOUNT = 0 THEN
        DBRESPONSE := 'Error: Inv Code ('|| P_INVCODE ||') not found';
        RETURN;
    END IF;
    
    IF P_INVCODE LIKE '4%' AND P_MOBILE IS NOT NULL THEN
        IF WEALTHMAKER.CHECK_NUMBER(P_MOBILE) = 'N' THEN
            DBRESPONSE := 'Invalid Mobile No';
            RETURN;
        END IF;
    END IF;
    
    IF P_MOBILE IS NOT NULL THEN
        V_MOBILECOUNT:=0;
        SELECT COUNT(*) INTO V_MOBILECOUNT FROM INVESTOR_MASTER WHERE MOBILE=P_MOBILE AND INV_CODE NOT LIKE SUBSTR(P_INVCODE,1,8)||'%';
        IF V_MOBILECOUNT > 0 THEN
            DBRESPONSE := 'Error: Mobile('|| P_MOBILE ||') no already present!';
            RETURN;
        END IF;
    END IF;
    
    IF P_PANNO IS NOT NULL THEN
        IF WEALTHMAKER.VALIDATEPAN1(P_PANNO) = 0 THEN
            DBRESPONSE := 'Invalid PAN';
            RETURN;
        END IF;
    END IF;
    
    IF P_PANNO IS NOT NULL THEN
        V_PANCOUNT:=0;
        SELECT COUNT(*) INTO V_PANCOUNT FROM INVESTOR_MASTER WHERE UPPER(PAN)=P_PANNO AND INV_CODE NOT LIKE SUBSTR(P_INVCODE,1,8)||'%';
        IF V_PANCOUNT>0 THEN
            DBRESPONSE := 'PAN already exists!';
            RETURN;
        END IF;
    END IF;
    
    IF P_EMAIL IS NOT NULL THEN
        V_EMAILCOUNT:=0;
        IF REGEXP_REPLACE(UPPER(P_EMAIL), '\s+', '') NOT IN ('NOTAVAILABLE', 'NA', 'NILL', 'NONE', 'N-A') THEN
            SELECT COUNT(*) INTO V_EMAILCOUNT FROM INVESTOR_MASTER WHERE UPPER(EMAIL)=UPPER(P_EMAIL) AND INV_CODE NOT LIKE SUBSTR(P_INVCODE,1,8)||'%';
            IF V_EMAILCOUNT>0 THEN
                DBRESPONSE := 'EMAIL  already exists!';
                RETURN;
            END IF;    
       END IF;
    END IF;
    
    IF P_AADHARNO IS NOT NULL THEN
        V_AADHARCOUNT:=0;
        SELECT COUNT(*) INTO V_AADHARCOUNT FROM INVESTOR_MASTER WHERE AADHAR_CARD_NO=P_AADHARNO AND INV_CODE <>P_INVCODE;
        IF V_AADHARCOUNT>0 THEN
            DBRESPONSE := 'Adhar Card Number already Exist!';
            RETURN;
        END IF;
    END IF;
    
    BEGIN  
        V_DOB:=TO_DATE(P_DOB, 'dd/mm/yyyy');
    EXCEPTION WHEN OTHERS THEN
        V_DOB:=NULL;
    END;    
    
    IF P_INVCODE LIKE '3%' THEN
        UPDATE INVESTOR_MASTER
        SET
            MODIFY_USER     = P_USERID,
            MODIFY_DATE     = SYSDATE,
            AADHAR_CARD_NO  = P_AADHARNO,
            PAN             = P_PANNO,
            MOBILE          = P_MOBILE,
            EMAIL           = P_EMAIL,
            ADDRESS1        = PP_ADDRESS,
            ADDRESS2        = PP_ADDRESS2,
            PINCODE         = P_PINNO,
            CITY_ID         = P_CITYID,
            DOB             = V_DOB
            WHERE INV_CODE = P_INVCODE;
    ELSE
      
        
        UPDATE INVESTOR_MASTER SET
            MODIFY_USER     = P_USERID,
            MODIFY_DATE     = SYSDATE,
            AADHAR_CARD_NO  = P_AADHARNO,
            PAN             = P_PANNO,
            MOBILE          = P_MOBILE,
            EMAIL           = P_EMAIL,
            DOB             = V_DOB
            WHERE INV_CODE = P_INVCODE;
        
      
        
        UPDATE INVESTOR_MASTER
        SET
            MODIFY_USER     = P_USERID,
            MODIFY_DATE     = SYSDATE,
            ADDRESS1        = PP_ADDRESS,
            ADDRESS2        = PP_ADDRESS2,
            PINCODE         = P_PINNO,
            CITY_ID         = P_CITYID
            WHERE SOURCE_ID =SUBSTR(P_INVCODE,1,8);
        
       
        UPDATE CLIENT_TEST
        SET
            MODIFY_USER = P_USERID,
            MODIFY_DATE = SYSDATE,
            ADD1        = PP_ADDRESS,
            ADD2        = PP_ADDRESS2,
            PINCODE     = P_PINNO,
            CITY_ID     = P_CITYID,
            STATE_ID    = P_STATEID
            WHERE SOURCE_CODE = SUBSTR(P_INVCODE,1,8);
            
            UPDATE CLIENT_TEST 
            SET MODIFY_USER=P_USERID,
            MODIFY_DATE=SYSDATE,
            DOB = V_DOB 
            WHERE CLIENT_CODEKYC = P_INVCODE;
    END IF;
    
    IF WEALTHMAKER.IS_FAMILY_HEAD(P_INVCODE)=1 THEN
            UPDATE WEALTHMAKER.CLIENT_MASTER SET
            MODIFY_USER     = P_USERID,
            MODIFY_DATE     = SYSDATE,
            PAN             = P_PANNO,
            MOBILE          = P_MOBILE,
            EMAIL           = P_EMAIL,
            ADDRESS1        = PP_ADDRESS,
            ADDRESS2        = PP_ADDRESS2,
            PINCODE         = P_PINNO,
            CITY_ID         = P_CITYID,
            DOB             = V_DOB
            WHERE CLIENT_CODE = SUBSTR(P_INVCODE,1,8);
    END IF;
   
    COMMIT;
    DBRESPONSE := 'Success: Address update successfully';


EXCEPTION
    WHEN OTHERS THEN
        DBRESPONSE := 'Error: An unexpected error occurred: ' || SQLERRM;
END;
/