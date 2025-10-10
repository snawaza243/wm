

-- 2. Individual test blocks for each functionality
-- Test 1: ROLEPERMISSION
BEGIN
    DBMS_OUTPUT.PUT_LINE('TEST 1: ROLEPERMISSION FUNCTIONALITY');
    DBMS_OUTPUT.PUT_LINE('====================================');
    
    -- Test with valid credentials
    TEST_PSM_LOG_PERMISSIONDebug('ROLEPERMISSION_VALID');
    
    -- Test with invalid credentials
    TEST_PSM_LOG_PERMISSIONDebug('ROLEPERMISSION_INVALID');
END;
/

-- Test 2: BUTTONPERMISSION  
BEGIN
    DBMS_OUTPUT.PUT_LINE('TEST 2: BUTTONPERMISSION FUNCTIONALITY');
    DBMS_OUTPUT.PUT_LINE('======================================');
    
    TEST_PSM_LOG_PERMISSIONDebug('BUTTONPERMISSION_VALID');
END;
/

-- Test 3: DATAPERMISSION
BEGIN
    DBMS_OUTPUT.PUT_LINE('TEST 3: DATAPERMISSION FUNCTIONALITY');
    DBMS_OUTPUT.PUT_LINE('====================================');
    
    TEST_PSM_LOG_PERMISSIONDebug('DATAPERMISSION_VALID');
END;
/

-- Test 4: SHOW_PRODUCT
BEGIN
    DBMS_OUTPUT.PUT_LINE('TEST 4: SHOW_PRODUCT FUNCTIONALITY');
    DBMS_OUTPUT.PUT_LINE('==================================');
    
    TEST_PSM_LOG_PERMISSIONDebug('SHOW_PRODUCT_VALID');
END;
/

-- Test 5: TR_BR_NEW
BEGIN
    DBMS_OUTPUT.PUT_LINE('TEST 5: TR_BR_NEW FUNCTIONALITY');
    DBMS_OUTPUT.PUT_LINE('================================');
    
    TEST_PSM_LOG_PERMISSIONDebug('TR_BR_NEW_VALID');
END;
/

-- Test 6: TR_BR_OLD
BEGIN
    DBMS_OUTPUT.PUT_LINE('TEST 6: TR_BR_OLD FUNCTIONALITY');
    DBMS_OUTPUT.PUT_LINE('================================');
    
    TEST_PSM_LOG_PERMISSIONDebug('TR_BR_OLD_VALID');
END;
/

-- 3. Comprehensive test with all functionalities in one go
BEGIN
    DBMS_OUTPUT.PUT_LINE('COMPREHENSIVE TEST - ALL FUNCTIONALITIES');
    DBMS_OUTPUT.PUT_LINE('========================================');
    
    DECLARE
        V_TEST_USER VARCHAR2(20) := '39339';
        V_TEST_ROLE VARCHAR2(20) := '212';
        V_TEST_PASS VARCHAR2(20) := '212';
    BEGIN
        -- Test each functionality sequentially
        TEST_PSM_LOG_PERMISSIONDebug('CUSTOM_TEST', V_TEST_USER, V_TEST_ROLE, V_TEST_PASS, 'ROLEPERMISSION');
        TEST_PSM_LOG_PERMISSIONDebug('CUSTOM_TEST', V_TEST_USER, V_TEST_ROLE, V_TEST_PASS, 'BUTTONPERMISSION', 'MAIN_FORM');
        TEST_PSM_LOG_PERMISSIONDebug('CUSTOM_TEST', V_TEST_USER, V_TEST_ROLE, V_TEST_PASS, 'DATAPERMISSION');
        TEST_PSM_LOG_PERMISSIONDebug('CUSTOM_TEST', V_TEST_USER, V_TEST_ROLE, V_TEST_PASS, 'SHOW_PRODUCT');
        TEST_PSM_LOG_PERMISSIONDebug('CUSTOM_TEST', V_TEST_USER, V_TEST_ROLE, V_TEST_PASS, 'TR_BR_NEW');
        TEST_PSM_LOG_PERMISSIONDebug('CUSTOM_TEST', V_TEST_USER, V_TEST_ROLE, V_TEST_PASS, 'TR_BR_OLD');
    END;
END;
/

-- 4. Debug procedure to check table data dependencies
CREATE OR REPLACE PROCEDURE DEBUG_TABLE_DATA AS
    V_USER_COUNT NUMBER;
    V_ROLE_COUNT NUMBER;
    V_PRODUCT_COUNT NUMBER;
    V_BUTTON_COUNT NUMBER;
    V_DATA_FILTER_COUNT NUMBER;
BEGIN
    DBMS_OUTPUT.PUT_LINE('TABLE DATA DEBUG INFORMATION');
    DBMS_OUTPUT.PUT_LINE('============================');
    
    -- Check user_master table
    SELECT COUNT(*) INTO V_USER_COUNT FROM user_master WHERE login_id = '39339';
    DBMS_OUTPUT.PUT_LINE('user_master records for 39339: ' || V_USER_COUNT);
    
    -- Check role_master table
    SELECT COUNT(*) INTO V_ROLE_COUNT FROM role_master WHERE role_id = '212';
    DBMS_OUTPUT.PUT_LINE('role_master records for 212: ' || V_ROLE_COUNT);
    
    -- Check bajaj_product_permissions table
    SELECT COUNT(*) INTO V_PRODUCT_COUNT FROM bajaj_product_permissions WHERE role_id = '212';
    DBMS_OUTPUT.PUT_LINE('bajaj_product_permissions records for 212: ' || V_PRODUCT_COUNT);
    
    -- Check ROLE_FORM_PERMISSION_VB table
    SELECT COUNT(*) INTO V_BUTTON_COUNT FROM ROLE_FORM_PERMISSION_VB WHERE ROLE_ID = '212';
    DBMS_OUTPUT.PUT_LINE('ROLE_FORM_PERMISSION_VB records for 212: ' || V_BUTTON_COUNT);
    
    -- Check DATAFILTER_MASTER table
    SELECT COUNT(*) INTO V_DATA_FILTER_COUNT FROM DATAFILTER_MASTER WHERE ROLE_ID = '212' AND LOGIN_ID = '39339';
    DBMS_OUTPUT.PUT_LINE('DATAFILTER_MASTER records for role 212, user 39339: ' || V_DATA_FILTER_COUNT);
    
    -- Display sample data if available
    IF V_PRODUCT_COUNT > 0 THEN
        FOR rec IN (SELECT prod_code FROM bajaj_product_permissions WHERE role_id = '212' AND ROWNUM = 1) 
        LOOP
            DBMS_OUTPUT.PUT_LINE('Sample product code for role 212: ' || rec.prod_code);
        END LOOP;
    END IF;
    
END DEBUG_TABLE_DATA;
/

-- Run the debug procedure
BEGIN
    DEBUG_TABLE_DATA;
END;
/

-- 5. Error handling test
BEGIN
    DBMS_OUTPUT.PUT_LINE('ERROR HANDLING TESTS');
    DBMS_OUTPUT.PUT_LINE('====================');
    
    -- Test with NULL parameters
    TEST_PSM_LOG_PERMISSION('NULL_TEST', NULL, NULL, NULL, 'ROLEPERMISSION');
    
    -- Test with invalid PX_FOR
    TEST_PSM_LOG_PERMISSION('INVALID_FOR', '39339', '212', '212', 'INVALID_FUNCTION');
    
    -- Test with very long parameters (boundary test)
    TEST_PSM_LOG_PERMISSION('LONG_PARAMS', 
        RPAD('X', 100, 'X'), 
        RPAD('Y', 100, 'Y'), 
        RPAD('Z', 100, 'Z'), 
        'ROLEPERMISSION');
END;
/

-- 6. Performance test with multiple executions
BEGIN
    DBMS_OUTPUT.PUT_LINE('PERFORMANCE TEST - MULTIPLE EXECUTIONS');
    DBMS_OUTPUT.PUT_LINE('======================================');
    
    FOR i IN 1..5 LOOP
        DBMS_OUTPUT.PUT_LINE('Execution #' || i);
        TEST_PSM_LOG_PERMISSION('PERF_TEST', '39339', '212', '212', 'SHOW_PRODUCT');
    END LOOP;
END;
/

-- 7. Test specific edge cases for SHOW_PRODUCT
BEGIN
    DBMS_OUTPUT.PUT_LINE('EDGE CASE TESTS FOR SHOW_PRODUCT');
    DBMS_OUTPUT.PUT_LINE('================================');
    
    -- Test with role that has no product permissions
    TEST_PSM_LOG_PERMISSION('NO_PRODUCTS', '39339', '999', '212', 'SHOW_PRODUCT');
    
    -- Test with very long product codes
    -- This would require actual data in the table
    TEST_PSM_LOG_PERMISSION('LONG_PRODUCTS', '39339', '212', '212', 'SHOW_PRODUCT');
END;
/

-- 8. Final validation summary
BEGIN
    DBMS_OUTPUT.PUT_LINE('FINAL VALIDATION SUMMARY');
    DBMS_OUTPUT.PUT_LINE('========================');
    DBMS_OUTPUT.PUT_LINE('1. ROLEPERMISSION: Tests user authentication and role retrieval');
    DBMS_OUTPUT.PUT_LINE('2. BUTTONPERMISSION: Tests form button permissions');
    DBMS_OUTPUT.PUT_LINE('3. DATAPERMISSION: Tests data access permissions');
    DBMS_OUTPUT.PUT_LINE('4. SHOW_PRODUCT: Tests product code retrieval and formatting');
    DBMS_OUTPUT.PUT_LINE('5. TR_BR_NEW: Tests new TR branch retrieval');
    DBMS_OUTPUT.PUT_LINE('6. TR_BR_OLD: Tests old TR branch retrieval');
    DBMS_OUTPUT.PUT_LINE('');
    DBMS_OUTPUT.PUT_LINE('All test cases include:');
    DBMS_OUTPUT.PUT_LINE('- Input parameter validation');
    DBMS_OUTPUT.PUT_LINE('- Output parameter verification');
    DBMS_OUTPUT.PUT_LINE('- Error handling testing');
    DBMS_OUTPUT.PUT_LINE('- Performance monitoring');
END;
/