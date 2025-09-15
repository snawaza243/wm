CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_RECO_M_RECONCILE2(
    MyTrCode            IN VARCHAR2, 
    MyRtaAmount         IN NUMBER,
    MyRtaTrDate         IN VARCHAR2,
    MyRtaFolio          IN VARCHAR2,
    MyRtaTrCode         IN VARCHAR2,
    MyDispatch          IN VARCHAR2,
    Glbloginid          IN VARCHAR2,
    P_TR_TRAN_TYPE      IN VARCHAR2,
    P_ROLE_ID           IN VARCHAR2,
    P_CURSOR            OUT SYS_REFCURSOR    
)  
AS
    V_ERROR_MSG         VARCHAR2(4000);
    MyRtaTrCode_1       VARCHAR2(4000);
    MyMailId            VARCHAR2(50);
    v_count             NUMBER;
    v_sql               VARCHAR2(4000);
BEGIN
    -- Validate input parameters
    IF MyTrCode IS NULL OR MyRtaTrCode IS NULL OR Glbloginid IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Mandatory parameters (TRAN_CODE, RTA_TRAN_CODE, LOGIN_ID) cannot be null' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Prepare RTA transaction code
    IF LENGTH(MyRtaTrCode) > 4000 THEN 
        MyRtaTrCode_1 := SUBSTR(MyRtaTrCode,1,4000);
    ELSE
        MyRtaTrCode_1 := MyRtaTrCode;
    END IF;
    
    MyRtaTrCode_1 := REPLACE(MyRtaTrCode_1, '''', '');

    -- Begin transaction
    SAVEPOINT START_TRANSACTION;
    
    -- Process updates based on dispatch type
    IF MyDispatch = 'N' THEN
        -- Non-dispatch case updates
        UPDATE Transaction_mf_temp1 
        SET amount = MyRtaAmount, 
            REC_FLAG = 'Y',
            RECO_DATE = SYSDATE,
            REC_USER = Glbloginid,
            RTA_TRAN_CODE = MyRtaTrCode_1  
        WHERE TRAN_CODE = MyTrCode;
       
        UPDATE Transaction_mf_temp1 
        SET amount = MyRtaAmount, 
            REC_FLAG = 'Y',
            RECO_DATE = SYSDATE,
            REC_USER = Glbloginid,
            RTA_TRAN_CODE = MyRtaTrCode_1
        WHERE BASE_TRAN_CODE = MyTrCode;
       
        -- Remote update using dynamic SQL to avoid ORA-22804
        v_sql := 'UPDATE Transaction_st@MF.BAJAJCAPITAL 
                 SET REC_FLAG = ''Y'',
                     HO_TRAN_CODE = ''' || MyTrCode || '''
                 WHERE tran_code IN (''' || MyRtaTrCode || ''')';
        EXECUTE IMMEDIATE v_sql;
        
        -- Verify update count
        SELECT COUNT(*) INTO v_count 
        FROM Transaction_mf_temp1 
        WHERE TRAN_CODE = MyTrCode AND REC_FLAG = 'Y';
        
        IF v_count = 0 THEN
            RAISE_APPLICATION_ERROR(-20002, 'No records updated in Transaction_mf_temp1');
        END IF;
    ELSE 
        -- Dispatch case updates
        IF P_TR_TRAN_TYPE = 'TRAILACTUAL' THEN 
            -- Trail transaction type updates
            UPDATE Transaction_mf_temp1 
            SET amount = MyRtaAmount,
                folio_no = MyRtaFolio,
                REC_FLAG = 'Y',
                RECO_DATE = SYSDATE,
                REC_USER = Glbloginid,
                RTA_TRAN_CODE = MyRtaTrCode_1
            WHERE TRAN_CODE = MyTrCode;
            
            UPDATE Transaction_mf_temp1 
            SET amount = MyRtaAmount,
                folio_no = MyRtaFolio,
                REC_FLAG = 'Y',
                RECO_DATE = SYSDATE,
                REC_USER = Glbloginid,
                RTA_TRAN_CODE = MyRtaTrCode_1
            WHERE BASE_TRAN_CODE = MyTrCode;

            -- Remote update
            v_sql := 'UPDATE Transaction_st@MF.BAJAJCAPITAL 
                     SET REC_FLAG = ''Y'',
                         HO_TRAN_CODE = ''' || MyTrCode || '''
                     WHERE tran_code IN (''' || MyRtaTrCode || ''')';
            EXECUTE IMMEDIATE v_sql;
        ELSE
            -- Regular transaction type updates
            UPDATE Transaction_mf_temp1 
            SET amount = MyRtaAmount,
                folio_no = MyRtaFolio,
                tr_date = NVL(TO_DATE(MyRtaTrDate, 'dd/mm/rrrr'), NULL),
                REC_FLAG = 'Y',
                RECO_DATE = SYSDATE,
                REC_USER = Glbloginid,
                RTA_TRAN_CODE = MyRtaTrCode_1
            WHERE TRAN_CODE = MyTrCode;

            UPDATE Transaction_mf_temp1 
            SET amount = MyRtaAmount,
                folio_no = MyRtaFolio,
                REC_FLAG = 'Y',
                RECO_DATE = SYSDATE,
                REC_USER = Glbloginid,
                RTA_TRAN_CODE = MyRtaTrCode_1
            WHERE BASE_TRAN_CODE = MyTrCode;

            -- Remote update
            v_sql := 'UPDATE Transaction_st@MF.BAJAJCAPITAL 
                     SET REC_FLAG = ''Y'',
                         HO_TRAN_CODE = ''' || MyTrCode || '''
                     WHERE tran_code IN (''' || MyRtaTrCode || ''')';
            EXECUTE IMMEDIATE v_sql;
        END IF;
    END IF;

    -- Send notification email if configured
    IF Glbloginid = '39339' THEN 
        MyMailId := 'anamikat@bajajcapital.com';
    ELSIF Glbloginid = '112649' THEN 
        MyMailId := 'rajeshb@bajajcapital.com';
    END IF;

    IF MyMailId IS NOT NULL THEN
        WEALTHMAKER.SEND_MAIL(
            recp => MyMailId,
            from_id => 'wealthmaker@bajajcapital.com',
            msg => '',
            msg1 => '',
            msg2 => 'AR Reconciled',
            msg3 => '',
            sub => 'Reco Update ' || MyTrCode
        );
    END IF;
    
    -- Return success message with record status
    OPEN P_CURSOR FOR
    SELECT 'SUCCESS: Record mapped successfully with REC_FLAG=' || REC_FLAG AS MESSAGE
    FROM Transaction_mf_temp1 
    WHERE TRAN_CODE = MyTrCode;
    
    COMMIT;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO START_TRANSACTION;
        V_ERROR_MSG := SQLERRM;
        
        -- Log the error (implement your logging mechanism here)
        -- INSERT INTO error_log VALUES (SYSDATE, 'PSM_MF_RECO_M_RECONCILE4', V_ERROR_MSG);
        
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || V_ERROR_MSG AS MESSAGE FROM DUAL;
END;
/