CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_SIP2_SET_RECONSILE(
    MYTRCODE            IN VARCHAR2, 
    MYRTAAMOUNT         IN NUMBER,
    MYRTATRDATE         IN VARCHAR2,
    MYRTAFOLIO          IN VARCHAR2,
    MYRTATRCODE         IN VARCHAR2,
    MYDISPATCH          IN VARCHAR2,
    GLBLOGINID          IN VARCHAR2,
    P_ROLE_ID           IN VARCHAR2,
    P_CURSOR            OUT SYS_REFCURSOR    
)  
AS
    V_ERROR_MSG         VARCHAR2(4000);
    MYRTATRCODE_1       VARCHAR2(4000);
    MYMAILID            VARCHAR2(50);
    V_COUNT             NUMBER;
    V_SQL               VARCHAR2(4000);
BEGIN
    -- Validate input parameters
    IF MYTRCODE IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'First Double Click The Record , You Want To Map' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    IF MYRTATRCODE IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'First Double Click The Record Of RTA Transaction,To Which Folio You Want To Map' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;


    -- Prepare RTA transaction code
    IF LENGTH(MYRTATRCODE) > 4000 THEN 
        MYRTATRCODE_1 := SUBSTR(MYRTATRCODE,1,4000);
    ELSE
        MYRTATRCODE_1 := MYRTATRCODE;
    END IF;
    MYRTATRCODE_1 := REPLACE(MYRTATRCODE_1, '''', '');

    -- Begin transaction
    SAVEPOINT START_TRANSACTION;
    IF MYDISPATCH = 'N' THEN
        UPDATE TRANSACTION_MF_TEMP1 SET  AMOUNT=MYRTAAMOUNT , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER=GLBLOGINID, RTA_TRAN_CODE=REPLACE(MYRTATRCODE, '''', '') WHERE TRAN_CODE=MYTRCODE;
        UPDATE TRANSACTION_MF_TEMP1 SET AMOUNT=MYRTAAMOUNT, REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER=GLBLOGINID,RTA_TRAN_CODE=REPLACE(MYRTATRCODE, '''', '') WHERE BASE_TRAN_CODE=MYTRCODE;
        --EXECUTE IMMEDIATE 'UPDATE Transaction_st@MF.BAJAJCAPITAL  SET REC_FLAG = ''Y'', HO_TRAN_CODE = ''' || MYTRCODE || ''' WHERE tran_code IN (' || MYRTATRCODE_1 || ')';
    
        FOR code IN (
                SELECT REGEXP_SUBSTR(MYRTATRCODE_1, '[^,]+', 1, LEVEL) AS TRAN_CODE
                FROM dual
                CONNECT BY REGEXP_SUBSTR(MYRTATRCODE_1, '[^,]+', 1, LEVEL) IS NOT NULL
            )
            LOOP
                UPDATE transaction_st@MF.BAJAJCAPITAL
                SET REC_FLAG = 'Y',
                HO_TRAN_CODE = MYTRCODE
                WHERE TRAN_CODE = code.TRAN_CODE; 
        END LOOP;
    
    
    
    ELSE
        UPDATE TRANSACTION_MF_TEMP1 SET  AMOUNT=MYRTAAMOUNT, REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER=GLBLOGINID ,RTA_TRAN_CODE=REPLACE(MYRTATRCODE, '''', '') WHERE TRAN_CODE= MYTRCODE;
        UPDATE TRANSACTION_MF_TEMP1 SET AMOUNT=MYRTAAMOUNT,  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER=GLBLOGINID,RTA_TRAN_CODE=REPLACE(MYRTATRCODE, '''', '') WHERE BASE_TRAN_CODE=MYTRCODE;
        --EXECUTE IMMEDIATE 'UPDATE Transaction_st@MF.BAJAJCAPITAL SET REC_FLAG = ''Y'', HO_TRAN_CODE = ''' || MYTRCODE || ''' WHERE tran_code IN (' || MYRTATRCODE_1 || ')';
        
        FOR code IN (
                SELECT REGEXP_SUBSTR(MYRTATRCODE_1, '[^,]+', 1, LEVEL) AS TRAN_CODE
                FROM dual
                CONNECT BY REGEXP_SUBSTR(MYRTATRCODE_1, '[^,]+', 1, LEVEL) IS NOT NULL
            )
            LOOP
                UPDATE transaction_st@MF.BAJAJCAPITAL
                SET REC_FLAG = 'Y',
                HO_TRAN_CODE = MYTRCODE
                WHERE TRAN_CODE = code.TRAN_CODE; 
        END LOOP;

    END IF;


    -- Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    -- Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    -- Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")

    IF GLBLOGINID = '39339' THEN 
        MYMAILID := 'anamikat@bajajcapital.com';
    ELSIF GLBLOGINID = '112649' THEN 
        MYMAILID := 'rajeshb@bajajcapital.com';
    END IF;

    IF MYMAILID IS NOT NULL THEN
        SEND_MAIL(
            RECP => MYMAILID,
            FROM_ID => 'wealthmaker@bajajcapital.com',
            MSG => '',
            MSG1 => '',
            MSG2 => 'Base SIP Reconciled',
            MSG3 => '',
            SUB => 'Reco Update ' || MYTRCODE
        );
    END IF;
    
    OPEN P_CURSOR FOR
        SELECT 'SUCCESS: Base SIP Registration Confirmed Sucessfully with REC_FLAG=' || REC_FLAG AS MESSAGE
        FROM TRANSACTION_MF_TEMP1 
        WHERE TRAN_CODE = MYTRCODE;
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO START_TRANSACTION;
        V_ERROR_MSG := SQLERRM;
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || V_ERROR_MSG AS MESSAGE FROM DUAL;
END;
/