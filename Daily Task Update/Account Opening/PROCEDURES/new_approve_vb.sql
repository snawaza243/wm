CREATE OR REPLACE PROCEDURE PSM_UPDATE_INVESTOR_APPROVAL (
    P_CLIENT_CODE   VARCHAR2,
    P_USER_ID       VARCHAR2,
    P_APPROVE_DATE  DATE,
    P_ERRORMESSAGE  OUT VARCHAR2
)
AS
    CURSOR cur_client IS 
        SELECT CLIENT_CODEKYC, CLIENT_PAN, APPROVED_FLAG 
        FROM CLIENT_TEST 
        WHERE MAIN_CODE = P_CLIENT_CODE;
        
    V_CLIENT_CODEKYC  CLIENT_TEST.CLIENT_CODEKYC%TYPE;
    V_CLIENT_PAN      CLIENT_TEST.CLIENT_PAN%TYPE;
    V_APPROVED_FLAG   CLIENT_TEST.APPROVED_FLAG%TYPE;
    V_COUNT           NUMBER;
BEGIN
    P_ERRORMESSAGE := NULL;
    
    FOR rec IN cur_client LOOP
        -- Check for Duplicate PAN (Excluding current client_codekyc)
        SELECT COUNT(*) INTO V_COUNT
        FROM CLIENT_TEST
        WHERE UPPER(CLIENT_PAN) = UPPER(rec.CLIENT_PAN)
          AND CLIENT_CODEKYC <> rec.CLIENT_CODEKYC;
        
        IF V_COUNT > 0 THEN
            P_ERRORMESSAGE := 'This PAN No already exists';
            RETURN;
        END IF;
        
        -- Update based on APPROVED_FLAG
        IF UPPER(rec.APPROVED_FLAG) = 'YES' THEN
            -- Approve investor
            UPDATE INVESTOR_MASTER 
            SET APPROVED = 'YES' 
            WHERE INV_CODE = rec.CLIENT_CODEKYC;

            UPDATE CLIENT_TEST 
            SET APPROVED_FLAG = 'YES',
                APPROVED = 'YES',
                APPROVE_DT = P_APPROVE_DATE,
                APPROVE_USERID = P_USER_ID
            WHERE MAIN_CODE = P_CLIENT_CODE 
              AND CLIENT_CODEKYC = rec.CLIENT_CODEKYC;
        ELSE
            -- Revoke approval
            UPDATE CLIENT_TEST 
            SET APPROVED_FLAG = 'NO',
                APPROVED = NULL,
                APPROVE_DT = NULL,
                APPROVE_USERID = NULL
            WHERE MAIN_CODE = P_CLIENT_CODE 
              AND CLIENT_CODEKYC = rec.CLIENT_CODEKYC;

            UPDATE INVESTOR_MASTER 
            SET APPROVED = NULL 
            WHERE INV_CODE = rec.CLIENT_CODEKYC;
        END IF;
    END LOOP;

EXCEPTION
    WHEN OTHERS THEN
        P_ERRORMESSAGE := SQLERRM;
END PSM_UPDATE_INVESTOR_APPROVAL;
/
