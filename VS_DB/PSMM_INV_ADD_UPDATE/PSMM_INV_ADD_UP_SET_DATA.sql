CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSMM_INV_ADD_UP_SET_DATA(
    P_INV_CODEX     VARCHAR2,
    P_MOBILEX       VARCHAR2,
    P_PANX          VARCHAR2,
    P_EMAILX        VARCHAR2,
    P_AADHARX       VARCHAR2,
    P_ADDRESS1X     VARCHAR2,
    P_ADDRESS2X     VARCHAR2,
    P_PINCODEX      VARCHAR2,
    P_CITY_IDX      VARCHAR2,
    P_STATE_IDX     VARCHAR2,
    P_DOBX          VARCHAR2,
    P_LOGINX        VARCHAR2, 
    P_ROLEX         VARCHAR2,
    P_CURSORX       OUT SYS_REFCURSOR

) 

AS
    V_ERROR             VARCHAR2(4000);

    V_INVCOUNT    NUMBER;
    V_MOBILECOUNT NUMBER;
    V_PANCOUNT    NUMBER;
    V_EMAILCOUNT  NUMBER;
    V_AADHARCOUNT NUMBER;
    V_DOB         DATE; 

BEGIN


       -- Role check
    IF P_ROLEX = '1' THEN 
        OPEN P_CURSORX FOR SELECT 'You are not authorised to update the details' msg FROM DUAL;
        RETURN;
    END IF;

    SELECT COUNT(*) INTO V_INVCOUNT
    FROM INVESTOR_MASTER 
    WHERE INV_CODE = P_INV_CODEX;

    IF V_INVCOUNT = 0 THEN
        OPEN P_CURSORX FOR SELECT 'Error: Inv Code ('|| P_INV_CODEX ||') not found' msg from dual;
        RETURN;
    END IF;

    IF P_INV_CODEX LIKE '4%' AND P_MOBILEX IS NOT NULL THEN
        IF WEALTHMAKER.CHECK_NUMBER(P_MOBILEX) = 'N' THEN
            OPEN P_CURSORX FOR SELECT 'Invalid Mobile No' msg from dual;
            RETURN;
        END IF;
    END IF;

    IF P_MOBILEX IS NOT NULL THEN
        V_MOBILECOUNT:=0;
        SELECT COUNT(*) INTO V_MOBILECOUNT FROM INVESTOR_MASTER WHERE MOBILE=P_MOBILEX AND INV_CODE NOT LIKE SUBSTR(P_INV_CODEX,1,8)||'%';
        IF V_MOBILECOUNT > 0 THEN
            OPEN P_CURSORX FOR SELECT 'Error: Mobile('|| P_MOBILEX ||') no already present!' msg from dual;
            RETURN;
        END IF;
    END IF;

    IF P_PANX IS NOT NULL THEN
        IF WEALTHMAKER.VALIDATEPAN1(P_PANX) = 0 THEN
            OPEN P_CURSORX FOR SELECT 'Invalid PAN' msg from dual;
            RETURN;
        END IF;
    END IF;

    IF P_PANX IS NOT NULL THEN
        V_PANCOUNT:=0;
        SELECT COUNT(*) INTO V_PANCOUNT FROM INVESTOR_MASTER WHERE UPPER(PAN)=P_PANX AND INV_CODE NOT LIKE SUBSTR(P_INV_CODEX,1,8)||'%';
        IF V_PANCOUNT>0 THEN
            OPEN P_CURSORX FOR SELECT 'PAN already exists!' msg from dual;
            RETURN;
        END IF;
    END IF;

    IF P_EMAILX IS NOT NULL THEN
        V_EMAILCOUNT:=0;
        IF REGEXP_REPLACE(UPPER(P_EMAILX), '\s+', '') NOT IN ('NOTAVAILABLE', 'NA', 'NILL', 'NONE', 'N-A') THEN
            SELECT COUNT(*) INTO V_EMAILCOUNT FROM INVESTOR_MASTER WHERE UPPER(EMAIL)=UPPER(P_EMAILX) AND INV_CODE NOT LIKE SUBSTR(P_INV_CODEX,1,8)||'%';
            IF V_EMAILCOUNT>0 THEN
                OPEN P_CURSORX FOR SELECT 'EMAIL  already exists!' msg from dual;
                RETURN;
            END IF;    
       END IF;
    END IF;

    IF P_AADHARX IS NOT NULL THEN
        V_AADHARCOUNT:=0;
        SELECT COUNT(*) INTO V_AADHARCOUNT FROM INVESTOR_MASTER WHERE AADHAR_CARD_NO=P_AADHARX AND INV_CODE <>P_INV_CODEX;
        IF V_AADHARCOUNT>0 THEN
            OPEN P_CURSORX FOR SELECT 'Aadhar Card Number already Exist' msg from dual;
            RETURN;
        END IF;
    END IF;

    BEGIN  
        V_DOB:=TO_DATE(P_DOBX, 'dd/mm/yyyy');
    EXCEPTION WHEN OTHERS THEN
        V_DOB:=NULL;
    END;    

    IF P_INV_CODEX LIKE '3%' THEN
        UPDATE INVESTOR_MASTER
        SET
            MODIFY_USER     = P_LOGINX,
            MODIFY_DATE     = SYSDATE,
            AADHAR_CARD_NO  = P_AADHARX,
            PAN             = P_PANX,
            MOBILE          = P_MOBILEX,
            EMAIL           = P_EMAILX,
            ADDRESS1        = P_ADDRESS1X,
            ADDRESS2        = P_ADDRESS2X,
            PINCODE         = P_PINCODEX,
            CITY_ID         = P_CITY_IDX,
            DOB             = V_DOB
            WHERE INV_CODE = P_INV_CODEX;
    ELSE


        UPDATE INVESTOR_MASTER SET
            MODIFY_USER     = P_LOGINX,
            MODIFY_DATE     = SYSDATE,
            AADHAR_CARD_NO  = P_AADHARX,
            PAN             = P_PANX,
            MOBILE          = P_MOBILEX,
            EMAIL           = P_EMAILX,
            DOB             = V_DOB
            WHERE INV_CODE = P_INV_CODEX;

        UPDATE INVESTOR_MASTER
        SET
            MODIFY_USER     = P_LOGINX,
            MODIFY_DATE     = SYSDATE,
            ADDRESS1        = P_ADDRESS1X,
            ADDRESS2        = P_ADDRESS2X,
            PINCODE         = P_PINCODEX,
            CITY_ID         = P_CITY_IDX
            WHERE SOURCE_ID =SUBSTR(P_INV_CODEX,1,8);


        UPDATE CLIENT_TEST
        SET
            MODIFY_USER = P_LOGINX,
            MODIFY_DATE = SYSDATE,
            ADD1        = P_ADDRESS1X,
            ADD2        = P_ADDRESS2X,
            PINCODE     = P_PINCODEX,
            CITY_ID     = P_CITY_IDX,
            STATE_ID    = P_STATE_IDX
            WHERE SOURCE_CODE = SUBSTR(P_INV_CODEX,1,8);

            UPDATE CLIENT_TEST 
            SET MODIFY_USER=P_LOGINX,
            MODIFY_DATE=SYSDATE,
            DOB = V_DOB 
            WHERE CLIENT_CODEKYC = P_INV_CODEX;
    END IF;

    IF WEALTHMAKER.IS_FAMILY_HEAD(P_INV_CODEX)=1 THEN
            UPDATE WEALTHMAKER.CLIENT_MASTER SET
            MODIFY_USER     = P_LOGINX,
            MODIFY_DATE     = SYSDATE,
            PAN             = P_PANX,
            MOBILE          = P_MOBILEX,
            EMAIL           = P_EMAILX,
            ADDRESS1        = P_ADDRESS1X,
            ADDRESS2        = P_ADDRESS2X,
            PINCODE         = P_PINCODEX,
            CITY_ID         = P_CITY_IDX,
            DOB             = V_DOB
            WHERE CLIENT_CODE = SUBSTR(P_INV_CODEX,1,8);
    END IF;

    COMMIT;
    OPEN P_CURSORX FOR SELECT 'SUCCESS: Address update successfully' msg from dual;


EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO sp_find;
        V_ERROR := 'ERROR:' || SQLERRM;
        OPEN P_CURSORX FOR SELECT V_ERROR msg FROM DUAL;
END;
/