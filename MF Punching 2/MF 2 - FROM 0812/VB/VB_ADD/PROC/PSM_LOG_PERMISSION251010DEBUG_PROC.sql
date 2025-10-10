-- =============================================
-- TEST AND DEBUG CODE FOR PSM_LOG_PERMISSION
-- =============================================

-- 1. First, let's create a test wrapper procedure for better debugging
CREATE OR REPLACE PROCEDURE TEST_PSM_LOG_PERMISSIONDebug(
    P_TEST_CASE IN VARCHAR2,
    P_LOGID IN VARCHAR2 DEFAULT NULL,
    P_ROLEID IN VARCHAR2 DEFAULT NULL,
    P_PASS IN VARCHAR2 DEFAULT NULL,
    Pp_FOR IN VARCHAR2 DEFAULT NULL,
    PP_FORM IN VARCHAR2 DEFAULT NULL
) AS
    V_ROLES VARCHAR2(4000);
    V_LID VARCHAR2(100);
    V_STAT VARCHAR2(20);
    V_MSG VARCHAR2(1000);
    V_BTNS VARCHAR2(4000);
    V_DATA_PERM VARCHAR2(4000);
    V_TR_BR_NEW VARCHAR2(4000);
    V_TR_BR_OLD VARCHAR2(4000);
    V_SHOW_PROD VARCHAR2(4000);
    P_FOR   VARCHAR2(4000);
    P_FORM  VARCHAR2(4000);
    
    -- Test data variables
    V_TEST_LOGID VARCHAR2(20);
    V_TEST_ROLEID VARCHAR2(20);
    V_TEST_PASS VARCHAR2(20);
BEGIN
    DBMS_OUTPUT.PUT_LINE('=========================================');
    DBMS_OUTPUT.PUT_LINE('TEST CASE: ' || P_TEST_CASE);
    DBMS_OUTPUT.PUT_LINE('=========================================');
    
    -- Set test data based on test case
    CASE P_TEST_CASE
        WHEN 'ROLEPERMISSION_VALID' THEN
            V_TEST_LOGID := '39339';
            V_TEST_ROLEID := '212';
            V_TEST_PASS := '212';
            P_FOR := 'ROLEPERMISSION';
            
        WHEN 'ROLEPERMISSION_INVALID' THEN
            V_TEST_LOGID := 'INVALID_USER';
            V_TEST_ROLEID := '999';
            V_TEST_PASS := 'WRONG_PASS';
            P_FOR := 'ROLEPERMISSION';
            
        WHEN 'BUTTONPERMISSION_VALID' THEN
            V_TEST_LOGID := '39339';
            V_TEST_ROLEID := '212';
            V_TEST_PASS := '212';
            P_FOR := 'BUTTONPERMISSION';
            P_FORM := 'MAIN_FORM'; -- Replace with actual form name
            
        WHEN 'DATAPERMISSION_VALID' THEN
            V_TEST_LOGID := '39339';
            V_TEST_ROLEID := '212';
            V_TEST_PASS := '212';
            P_FOR := 'DATAPERMISSION';
            
        WHEN 'SHOW_PRODUCT_VALID' THEN
            V_TEST_LOGID := '39339';
            V_TEST_ROLEID := '212';
            V_TEST_PASS := '212';
            P_FOR := 'SHOW_PRODUCT';
            
        WHEN 'TR_BR_NEW_VALID' THEN
            V_TEST_LOGID := '39339';
            V_TEST_ROLEID := '212';
            V_TEST_PASS := '212';
            P_FOR := 'TR_BR_NEW';
            
        WHEN 'TR_BR_OLD_VALID' THEN
            V_TEST_LOGID := '39339';
            V_TEST_ROLEID := '212';
            V_TEST_PASS := '212';
            P_FOR := 'TR_BR_OLD';
            
        ELSE
            -- Use provided parameters
            V_TEST_LOGID := P_LOGID;
            V_TEST_ROLEID := P_ROLEID;
            V_TEST_PASS := P_PASS;
    END CASE;
    
    -- Display input parameters
    DBMS_OUTPUT.PUT_LINE('Input Parameters:');
    DBMS_OUTPUT.PUT_LINE('  PX_LOGID: ' || V_TEST_LOGID);
    DBMS_OUTPUT.PUT_LINE('  PX_ROLEID: ' || V_TEST_ROLEID);
    DBMS_OUTPUT.PUT_LINE('  PX_PASS: ' || V_TEST_PASS);
    DBMS_OUTPUT.PUT_LINE('  PX_FOR: ' || P_FOR);
    DBMS_OUTPUT.PUT_LINE('  PX_FORM: ' || P_FORM);
    DBMS_OUTPUT.PUT_LINE('-----------------------------------------');
    
    -- Execute the main procedure
    BEGIN
        PSM_LOG_PERMISSION(
            PX_LOGID         => V_TEST_LOGID,
            PX_ROLEID        => V_TEST_ROLEID,
            PX_PASS          => V_TEST_PASS,
            PX_FOR           => P_FOR,
            PX_FORM          => P_FORM,
            PX_OUT_ROLES     => V_ROLES,
            PX_OUT_LID       => V_LID,
            PX_OUT_STAT      => V_STAT,
            PX_OUT_MSG       => V_MSG,
            PX_OUT_BTNS      => V_BTNS,
            PX_OUT_DATA_PERM => V_DATA_PERM,
            PX_OUT_TR_BR_NEW => V_TR_BR_NEW,
            PX_OUT_TR_BR_OLD => V_TR_BR_OLD,
            PX_OUT_SHOW_PROD => V_SHOW_PROD
        );
        
        -- Display results
        DBMS_OUTPUT.PUT_LINE('RESULTS:');
        DBMS_OUTPUT.PUT_LINE('  Status: ' || V_STAT);
        DBMS_OUTPUT.PUT_LINE('  Message: ' || V_MSG);
        DBMS_OUTPUT.PUT_LINE('  Login ID: ' || V_LID);
        
        -- Display output based on PX_FOR type
        CASE P_FOR
            WHEN 'ROLEPERMISSION' THEN
                DBMS_OUTPUT.PUT_LINE('  Roles: ' || V_ROLES);
                
            WHEN 'BUTTONPERMISSION' THEN
                DBMS_OUTPUT.PUT_LINE('  Buttons: ' || V_BTNS);
                
            WHEN 'DATAPERMISSION' THEN
                DBMS_OUTPUT.PUT_LINE('  Data Permissions: ' || V_DATA_PERM);
                
            WHEN 'SHOW_PRODUCT' THEN
                DBMS_OUTPUT.PUT_LINE('  Product Codes: ' || V_SHOW_PROD);
                
            WHEN 'TR_BR_NEW' THEN
                DBMS_OUTPUT.PUT_LINE('  TR Branches New: ' || V_TR_BR_NEW);
                
            WHEN 'TR_BR_OLD' THEN
                DBMS_OUTPUT.PUT_LINE('  TR Branches Old: ' || V_TR_BR_OLD);
                
            ELSE
                DBMS_OUTPUT.PUT_LINE('  All Outputs:');
                DBMS_OUTPUT.PUT_LINE('    Roles: ' || V_ROLES);
                DBMS_OUTPUT.PUT_LINE('    Buttons: ' || V_BTNS);
                DBMS_OUTPUT.PUT_LINE('    Data Perm: ' || V_DATA_PERM);
                DBMS_OUTPUT.PUT_LINE('    Show Prod: ' || V_SHOW_PROD);
                DBMS_OUTPUT.PUT_LINE('    TR BR New: ' || V_TR_BR_NEW);
                DBMS_OUTPUT.PUT_LINE('    TR BR Old: ' || V_TR_BR_OLD);
        END CASE;
        
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_OUTPUT.PUT_LINE('ERROR in procedure execution:');
            DBMS_OUTPUT.PUT_LINE('  Error Code: ' || SQLCODE);
            DBMS_OUTPUT.PUT_LINE('  Error Message: ' || SQLERRM);
    END;
    
    DBMS_OUTPUT.PUT_LINE('=========================================');
    DBMS_OUTPUT.PUT_LINE('');
    
END TEST_PSM_LOG_PERMISSIONDebug;
/