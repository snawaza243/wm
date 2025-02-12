create or replace PROCEDURE PSM_AO_ApproveClient(
    p_client_code IN VARCHAR2,     -- Input parameter for client code
    p_loggedin_user IN VARCHAR2,
    o_message OUT VARCHAR2        -- Output parameter for return message
) AS


BEGIN
    -- Update the client_test table
    UPDATE client_test
    SET 
        approve_dt = SYSDATE,           -- Set current date
        approve_userid = p_loggedin_user,          -- Set logged-in user
        approved_flag = 'YES',           -- Set approved flag to YES
        approved = 'YES'
    WHERE client_code = p_client_code;     -- Filter based on client code
    
    
    UPDATE INVESTOR_MASTER
    SET APPROVED = 'YES'
    WHERE INV_CODE = (SELECT CLIENT_CODEKYC FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code);     
    
    UPDATE CLIENT_MASTER
    SET APPROVED_FLAG = 'YES'
    WHERE CLIENT_CODE = (SELECT SOURCE_CODE FROM CLIENT_TEST WHERE CLIENT_CODE = p_client_code);
    
    

    -- Check if any rows were updated
    IF SQL%ROWCOUNT > 0 THEN
        o_message := 'Client Approved';          -- Set success message
    ELSE
        o_message := 'Not Approved'; -- Set message if no records were found
    END IF;

    COMMIT; -- Commit changes
EXCEPTION
    WHEN OTHERS THEN
        o_message := 'Error: ' || SQLERRM; -- Return error message in case of exceptions
END PSM_AO_ApproveClient;