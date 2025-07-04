CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_ApproveClient(
    p_client_code    IN VARCHAR2,     -- Main client code
    p_loggedin_user  IN VARCHAR2,     -- Logged-in user
    o_message        OUT VARCHAR2     -- Output message
) AS
    -- Variable declarations
    v_count          NUMBER;
    v_main_code      VARCHAR2(50);
    v_pan           VARCHAR2(50);

    -- Cursor for processing related client records
    CURSOR cur_client IS
        SELECT CLIENT_CODEKYC, CLIENT_PAN, APPROVED 
        FROM CLIENT_TEST 
        WHERE MAIN_CODE = p_client_code;

BEGIN
    o_message := NULL;

    -- Approving main client and related sub-accounts
    UPDATE CLIENT_TEST
    SET 
        APPROVE_DT = SYSDATE,
        APPROVE_USERID = p_loggedin_user,
        APPROVED_FLAG = 'YES',
        APPROVED = 'YES'
    WHERE CLIENT_CODE = p_client_code
       OR MAIN_CODE = p_client_code;  -- Approving all related sub-accounts

    -- Update investor_master approval for related investors
    UPDATE INVESTOR_MASTER
    SET APPROVED = 'YES'
    WHERE INV_CODE IN (SELECT CLIENT_CODEKYC FROM CLIENT_TEST WHERE MAIN_CODE = p_client_code);

    -- Update CLIENT_MASTER approval
    UPDATE CLIENT_MASTER
    SET APPROVED_FLAG = 'YES'
    WHERE CLIENT_CODE IN (SELECT SUBSTR(CLIENT_CODEKYC, 1, 8) FROM CLIENT_TEST WHERE MAIN_CODE = p_client_code);

    -- Check for duplicate PAN before committing
    FOR rec IN cur_client LOOP
        IF rec.CLIENT_PAN IS NOT NULL THEN
            -- Check if this PAN exists in another client
            SELECT COUNT(*) INTO v_count
            FROM CLIENT_TEST
            WHERE UPPER(CLIENT_PAN) = UPPER(rec.CLIENT_PAN)
              AND CLIENT_CODEKYC <> rec.CLIENT_CODEKYC;

            IF v_count > 0 THEN
                o_message := 'This PAN No already exists!';
                ROLLBACK;
                RETURN;
            END IF;
        END IF;

        -- Approve investor if flagged as 'YES'
     /*   IF UPPER(rec.approved) = 'YES' THEN UPDATE INVESTOR_MASTER 
            SET APPROVED = 'YES'
            WHERE INV_CODE = rec.CLIENT_CODEKYC;

            UPDATE CLIENT_TEST
            SET APPROVED_FLAG = 'YES',
                APPROVED = 'YES',
                APPROVE_DT = SYSDATE,
                APPROVE_USERID = p_loggedin_user
            WHERE MAIN_CODE = p_client_code
              AND CLIENT_CODEKYC = rec.CLIENT_CODEKYC;
        ELSE
            -- Revoke approval
            UPDATE CLIENT_TEST
            SET APPROVED_FLAG = 'NO',
                APPROVED = NULL,
                APPROVE_DT = NULL,
                APPROVE_USERID = NULL
            WHERE MAIN_CODE = p_client_code
              AND CLIENT_CODEKYC = rec.CLIENT_CODEKYC;

            UPDATE INVESTOR_MASTER
            SET APPROVED = NULL
            WHERE INV_CODE = rec.CLIENT_CODEKYC;
        END IF; */


           UPDATE CLIENT_TEST
        SET APPROVED_FLAG = 'YES',
            APPROVED = 'YES',
            APPROVE_DT = SYSDATE,
            APPROVE_USERID = p_loggedin_user
        WHERE MAIN_CODE = p_client_code
          AND APPROVED = 'YES' ;


    END LOOP;




    -- Check if any records were updated
    IF SQL%ROWCOUNT > 0 THEN
        o_message := 'Main Account and All Sub-Accounts have been Successfully Approved';
        COMMIT;
    ELSE
        o_message := 'No records were updated!';
        ROLLBACK;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        o_message := 'Error: ' || SQLERRM;
        ROLLBACK;
END PSM_AO_ApproveClient;
/
