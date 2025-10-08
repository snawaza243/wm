CREATE OR REPLACE PROCEDURE PSM_LOG_PERMISSION(
    PX_LOGID            IN VARCHAR2,
    PX_ROLEID           IN VARCHAR2,
    PX_PASS             IN VARCHAR2,
    PX_FOR              IN VARCHAR2 DEFAULT NULL,
    PX_FORM             IN VARCHAR2 DEFAULT NULL,
    PX_OUT_ROLES        OUT VARCHAR2,
    PX_OUT_LID          OUT VARCHAR2,
    PX_OUT_STAT         OUT VARCHAR2,
    PX_OUT_MSG          OUT VARCHAR2,
    PX_OUT_BTNS         OUT VARCHAR2,
    PX_OUT_DATA_PERM    OUT VARCHAR2, 
    PX_OUT_TR_BR_NEW    OUT VARCHAR2,
    PX_OUT_TR_BR_OLD    OUT VARCHAR2,
    PX_OUT_SHOW_PROD    OUT VARCHAR2

) AS 
    -- Special characters for concatenation patterns
    LV_KV_SEP       VARCHAR2(1) := '#'; -- Key-Value separator
    LV_PAIR_SEP     VARCHAR2(1) := '@'; -- Pair separator
    LV_ITEM_SEP     VARCHAR2(1) := ','; -- Item separator
    LV_SP_CHR       VARCHAR2(1) := '|'; -- Special character
    LV_DYN_VAR      VARCHAR2(10) := 'RPT_'; -- Dynamic variable prefix
    LV_SERVER_DATE  DATE;

    
BEGIN
    -- Initialize OUT parameters
    PX_OUT_ROLES        := '';
    PX_OUT_LID          := '';
    PX_OUT_STAT         := 'FAIL';
    PX_OUT_MSG          := '';
    PX_OUT_BTNS         := '';
    PX_OUT_DATA_PERM    := '';
    PX_OUT_TR_BR_NEW    :='';
    PX_OUT_TR_BR_OLD    :='';
    PX_OUT_SHOW_PROD    :='';
    
    -- Get server date
    SELECT SYSDATE INTO LV_SERVER_DATE FROM DUAL;
    
    -- ROLEPERMISSION functionality
    IF PX_FOR = 'ROLEPERMISSION' THEN
        DECLARE -- Local variables for ROLEPERMISSION
            LV_RP_ROLE_IDS  VARCHAR2(4000);
            LV_RP_ROLE_NM   VARCHAR2(4000);
            LV_RP_ROLE_ID   VARCHAR2(100);
            LV_RP_COUNT     NUMBER := 0;
        BEGIN
            SELECT Role_id, login_id INTO LV_RP_ROLE_IDS, PX_OUT_LID FROM user_master 
            WHERE login_id = PX_LOGID AND login_pass = PX_PASS
            AND status = '1' AND LOGINVARIFY(PX_LOGID) = 1;
            
            -- Record Found
            PX_OUT_STAT := 'SUCCESS';
            PX_OUT_MSG := 'Record Found';
            
            -- Check for multiple roles
            IF INSTR(LV_RP_ROLE_IDS, ',') > 0 THEN
                -- Multiple roles - iterate through them
                FOR LV_RP_I IN (
                    SELECT TRIM(REGEXP_SUBSTR(LV_RP_ROLE_IDS, '[^,]+', 1, LEVEL)) AS SINGLE_ROLE
                    FROM DUAL
                    CONNECT BY LEVEL <= REGEXP_COUNT(LV_RP_ROLE_IDS, ',') + 1
                )
                LOOP
                    -- Get role name and role_id for each role_id
                    BEGIN
                        SELECT role_id, role_name INTO LV_RP_ROLE_ID, LV_RP_ROLE_NM
                        FROM role_master 
                        WHERE role_id = LV_RP_I.SINGLE_ROLE;
                        
                        -- Build role item with pattern: name#id|prefix_iteration
                        IF LV_RP_COUNT = 0 THEN
                            PX_OUT_ROLES := LV_RP_ROLE_NM || LV_KV_SEP || LV_RP_ROLE_ID;
                        ELSE
                            PX_OUT_ROLES := PX_OUT_ROLES || LV_PAIR_SEP || LV_RP_ROLE_NM || LV_KV_SEP || LV_RP_ROLE_ID;
                        END IF;
                        
                        LV_RP_COUNT := LV_RP_COUNT + 1;
                        
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL; -- Continue with next role
                    END;
                END LOOP;
            ELSE
                -- Single role
                BEGIN
                    SELECT role_id, role_name INTO LV_RP_ROLE_ID, LV_RP_ROLE_NM
                    FROM role_master 
                    WHERE role_id = LV_RP_ROLE_IDS;
                    
                    -- Build single role with pattern: name#id|prefix_1
                    PX_OUT_ROLES := LV_RP_ROLE_NM || LV_KV_SEP || LV_RP_ROLE_ID;
                    LV_RP_COUNT := 1;
                    
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        PX_OUT_MSG := PX_OUT_MSG || ' Role not found';
                END;
            END IF;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                -- No records found
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'You Are not Authorized';
            WHEN TOO_MANY_ROWS THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Multiple users found';
            WHEN OTHERS THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Error: ' || SQLERRM;
        END;
    
    -- BUTTONPERMISSION functionality
    ELSIF PX_FOR = 'BUTTONPERMISSION' THEN
        DECLARE -- Local variables for BUTTONPERMISSION
            LV_BP_VIEW      VARCHAR2(1);
            LV_BP_ADD       VARCHAR2(1);
            LV_BP_UPD       VARCHAR2(1);
            LV_BP_PRINT     VARCHAR2(1);
            LV_BP_MAIN_FORM VARCHAR2(100);
            LV_BP_FRM_CNT   NUMBER := 0;
        BEGIN
            -- Iterate through role form permissions
            FOR LV_BP_REC IN (
                SELECT 
                    RFP.FORM_NAME, 
                    RFP.VIEW_BUTTON, 
                    RFP.ADD_BUTTON, 
                    RFP.UPDATE_BUTTON, 
                    RFP.PRINT_BUTTON
                FROM ROLE_FORM_PERMISSION_VB RFP
                WHERE RFP.ROLE_ID = PX_ROLEID
            )
            LOOP
                -- Check if any button permission is granted
                IF LV_BP_REC.VIEW_BUTTON = '1' OR 
                   LV_BP_REC.ADD_BUTTON = '1' OR 
                   LV_BP_REC.UPDATE_BUTTON = '1' OR 
                   LV_BP_REC.PRINT_BUTTON = '1' THEN
                    
                    -- Get main form name
                    BEGIN
                        SELECT MAIN INTO LV_BP_MAIN_FORM
                        FROM BACKOFFICE_FORMS
                        WHERE FORMNAME = LV_BP_REC.FORM_NAME;
                        
                        -- Check if the current form matches the main form
                        IF LV_BP_MAIN_FORM = PX_FORM THEN
                            -- Build button permissions with pattern: key#value@key#value@key#value
                            PX_OUT_BTNS := 
                                'VIEW' || LV_KV_SEP || LV_BP_REC.VIEW_BUTTON || 
                                LV_PAIR_SEP || 'ADD' || LV_KV_SEP || LV_BP_REC.ADD_BUTTON || 
                                LV_PAIR_SEP || 'UPDATE' || LV_KV_SEP || LV_BP_REC.UPDATE_BUTTON || 
                                LV_PAIR_SEP || 'PRINT' || LV_KV_SEP || LV_BP_REC.PRINT_BUTTON;
                            
                            PX_OUT_STAT := 'SUCCESS';
                            PX_OUT_MSG := 'Button permissions found';
                            EXIT; -- Exit loop after finding matching form
                        END IF;
                        
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL; -- Continue to next record
                    END;
                END IF;
                
                LV_BP_FRM_CNT := LV_BP_FRM_CNT + 1;
            END LOOP;
            
            -- If no button permissions found
            IF PX_OUT_BTNS IS NULL OR PX_OUT_BTNS = '' THEN
                PX_OUT_STAT := 'SUCCESS';
                PX_OUT_MSG := 'No button restrictions';
                -- Default all buttons enabled with pattern: key#value@key#value@key#value
                PX_OUT_BTNS := 
                    'VIEW' || LV_KV_SEP || '1' || 
                    LV_PAIR_SEP || 'ADD' || LV_KV_SEP || '1' || 
                    LV_PAIR_SEP || 'UPDATE' || LV_KV_SEP || '1' || 
                    LV_PAIR_SEP || 'PRINT' || LV_KV_SEP || '1';
            END IF;
            
        EXCEPTION
            WHEN OTHERS THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Error in button permission: ' || SQLERRM;
        END;
    
    -- DATAPERMISSION functionality
    ELSIF PX_FOR = 'DATAPERMISSION' THEN
        DECLARE-- Local variables for DATAPERMISSION
            LV_DP_UP_PRE_DUR    VARCHAR2(20);
            LV_DP_UP_NEXT_DUR   VARCHAR2(20);
            LV_DP_IN_PRE_DUR    VARCHAR2(20);
            LV_DP_IN_NEXT_DUR   VARCHAR2(20);
            LV_DP_UP_PRE_TYPE   VARCHAR2(20);
            LV_DP_UP_NEXT_TYPE  VARCHAR2(20);
            LV_DP_IN_PRE_TYPE   VARCHAR2(20);
            LV_DP_IN_NEXT_TYPE  VARCHAR2(20);
            LV_DP_DATA_FILTER   VARCHAR2(4000);
            LV_DP_IS_LOCKED     VARCHAR2(1) := 'N';
            LV_DP_IS_LOCKED_LI  VARCHAR2(1) := 'N';
            LV_DP_INS_PREV_DATE VARCHAR2(20);
            LV_DP_INS_NEXT_DATE VARCHAR2(20);
            LV_DP_UP_PREV_DATE  VARCHAR2(20);
            LV_DP_UP_NEXT_DATE  VARCHAR2(20);
            LV_DP_FIN_YR_FR     VARCHAR2(20);
            LV_DP_FIN_YR_TO     VARCHAR2(20);
        BEGIN
            -- Get role data permissions
            SELECT 
                UP_PRE_DUR, UP_NEXT_DUR, IN_PRE_DUR, IN_NEXT_DUR,
                UP_PRE_DUR_TYPE, UP_NEXT_DUR_TYPE, IN_PRE_DUR_TYPE, IN_NEXT_DUR_TYPE
            INTO 
                LV_DP_UP_PRE_DUR, LV_DP_UP_NEXT_DUR, LV_DP_IN_PRE_DUR, LV_DP_IN_NEXT_DUR,
                LV_DP_UP_PRE_TYPE, LV_DP_UP_NEXT_TYPE, LV_DP_IN_PRE_TYPE, LV_DP_IN_NEXT_TYPE
            FROM ROLE_MASTER 
            WHERE ROLE_ID = PX_ROLEID;  -- Using PX_ROLEID as role_id
            
            -- Calculate date ranges based on types
            LV_DP_INS_PREV_DATE := PSM_CALC_DATE_RANGE(LV_DP_IN_PRE_DUR, LV_DP_IN_PRE_TYPE, 'PREV', LV_SERVER_DATE);
            LV_DP_INS_NEXT_DATE := PSM_CALC_DATE_RANGE(LV_DP_IN_NEXT_DUR, LV_DP_IN_NEXT_TYPE, 'NEXT', LV_SERVER_DATE);
            LV_DP_UP_PREV_DATE := PSM_CALC_DATE_RANGE(LV_DP_UP_PRE_DUR, LV_DP_UP_PRE_TYPE, 'PREV', LV_SERVER_DATE);
            LV_DP_UP_NEXT_DATE := PSM_CALC_DATE_RANGE(LV_DP_UP_NEXT_DUR, LV_DP_UP_NEXT_TYPE, 'NEXT', LV_SERVER_DATE);
            
            -- Get data filter
            BEGIN
                SELECT DATA_FILTER INTO LV_DP_DATA_FILTER
                FROM DATAFILTER_MASTER 
                WHERE ROLE_ID = PX_ROLEID 
                AND LOGIN_ID = PX_LOGID;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    LV_DP_DATA_FILTER := '';
            END;
            
            -- Check if user is locked
            BEGIN
                SELECT 'Y' INTO LV_DP_IS_LOCKED
                FROM USER_LOCKED 
                WHERE USERID = PX_LOGID
                AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    LV_DP_IS_LOCKED := 'N';
            END;
            
            -- Check if LI module is blocked
            BEGIN
                SELECT 'Y' INTO LV_DP_IS_LOCKED_LI
                FROM RDC_BLOCK 
                WHERE MODULE = 'LI' 
                AND BLOCKED = 'Y'
                AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    LV_DP_IS_LOCKED_LI := 'N';
            END;
            
            -- Build data permission output with pattern
            PX_OUT_DATA_PERM := 
                'UP_PRE_DUR' || LV_KV_SEP || LV_DP_UP_PRE_DUR || 
                LV_PAIR_SEP || 'UP_NEXT_DUR' || LV_KV_SEP || LV_DP_UP_NEXT_DUR || 
                LV_PAIR_SEP || 'IN_PRE_DUR' || LV_KV_SEP || LV_DP_IN_PRE_DUR || 
                LV_PAIR_SEP || 'IN_NEXT_DUR' || LV_KV_SEP || LV_DP_IN_NEXT_DUR || 
                LV_PAIR_SEP || 'UP_PRE_TYPE' || LV_KV_SEP || LV_DP_UP_PRE_TYPE || 
                LV_PAIR_SEP || 'UP_NEXT_TYPE' || LV_KV_SEP || LV_DP_UP_NEXT_TYPE || 
                LV_PAIR_SEP || 'IN_PRE_TYPE' || LV_KV_SEP || LV_DP_IN_PRE_TYPE || 
                LV_PAIR_SEP || 'IN_NEXT_TYPE' || LV_KV_SEP || LV_DP_IN_NEXT_TYPE || 
                LV_PAIR_SEP || 'DATA_FILTER' || LV_KV_SEP || LV_DP_DATA_FILTER || 
                LV_PAIR_SEP || 'USER_LOCKED' || LV_KV_SEP || LV_DP_IS_LOCKED || 
                LV_PAIR_SEP || 'LI_LOCKED' || LV_KV_SEP || LV_DP_IS_LOCKED_LI || 
                LV_PAIR_SEP || 'INS_PREV_DATE' || LV_KV_SEP || LV_DP_INS_PREV_DATE || 
                LV_PAIR_SEP || 'INS_NEXT_DATE' || LV_KV_SEP || LV_DP_INS_NEXT_DATE || 
                LV_PAIR_SEP || 'UP_PREV_DATE' || LV_KV_SEP || LV_DP_UP_PREV_DATE || 
                LV_PAIR_SEP || 'UP_NEXT_DATE' || LV_KV_SEP || LV_DP_UP_NEXT_DATE;
            
            PX_OUT_STAT := 'SUCCESS';
            PX_OUT_MSG := 'Data permissions found';
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Role not found for data permission';
            WHEN OTHERS THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Error in data permission: ' || SQLERRM;
        END;

    -- SHOW_PRODUCT functionality
    ELSIF PX_FOR = 'SHOW_PRODUCT' THEN
        DECLARE
            LV_SP_PROD_CODES VARCHAR2(4000);
            LV_SP_FINAL_PROD VARCHAR2(4000);
        BEGIN
            -- Get product codes for the given role
            SELECT prod_code INTO LV_SP_PROD_CODES
            FROM bajaj_product_permissions 
            WHERE role_id = PX_ROLEID;
            
            IF LV_SP_PROD_CODES IS NOT NULL THEN
                -- Remove last character (equivalent to Left(rsData1(0), Len(rsData1(0)) - 1) in VB)
                LV_SP_FINAL_PROD := SUBSTR(LV_SP_PROD_CODES, 1, LENGTH(LV_SP_PROD_CODES) - 1);
                
                -- Replace # with ',' and wrap each product code with single quotes
                -- Equivalent to: "'" & Replace(Products, "#", "','") & "'"
                LV_SP_FINAL_PROD := '''' || REPLACE(LV_SP_FINAL_PROD, '#', ''',''') || '''';
                
                -- Set output parameters
                PX_OUT_SHOW_PROD := LV_SP_FINAL_PROD;  
                PX_OUT_STAT := 'SUCCESS';
                PX_OUT_MSG := 'Product codes retrieved successfully';
            ELSE
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'No product codes found for this role';
            END IF;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'No product permissions found for role: ' || PX_ROLEID;
            WHEN OTHERS THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Error in SHOW_PRODUCT: ' || SQLERRM;
        END;
    
    -- TR_BR_NEW functionality
    ELSIF PX_FOR = 'TR_BR_NEW' THEN
        BEGIN
            DECLARE
                LV_TR_BRANCHES VARCHAR2(4000) := '';
                LV_TR_BR_CODE VARCHAR2(4000);
                LV_TR_BR_TEMP VARCHAR2(4000);
                LV_TR_J NUMBER := 0;
            BEGIN
                -- Get branch IDs from userdetails_ji with data filter conditions
                FOR LV_TR_REC IN (
                    SELECT UD.BRANCH_ID
                    FROM USERDETAILS_JI UD
                    WHERE UD.LOGIN_ID = PX_LOGID
                    AND UD.DATAFILTER IN (
                        SELECT DF.DATA_FILTER
                        FROM DATAFILTER_MASTER DF
                        WHERE DF.LOGIN_ID IN (
                            SELECT EM.PAYROLL_ID 
                            FROM EMPLOYEE_MASTER EM 
                            WHERE EM.TYPE = 'A' 
                            AND EM.CATEGORY_ID = 2001
                        )
                        AND DF.LOGIN_ID = PX_LOGID
                        AND DF.ROLE_DEFAULT = '1'
                    )
                )
                LOOP
                    -- Call GETBRANCH_TR function for each branch_id
                    BEGIN
                        SELECT GETBRANCH_TR(LV_TR_REC.BRANCH_ID) INTO LV_TR_BR_CODE FROM DUAL;
                        
                        IF LV_TR_BR_CODE IS NOT NULL THEN
                            -- Remove first character and split by #
                            LV_TR_BR_TEMP := SUBSTR(LV_TR_BR_CODE, 2);
                            
                            -- Split the string by # and concatenate with comma
                            FOR LV_TR_I IN (
                                SELECT TRIM(REGEXP_SUBSTR(LV_TR_BR_TEMP, '[^#]+', 1, LEVEL)) AS BRANCH_PART
                                FROM DUAL
                                CONNECT BY LEVEL <= REGEXP_COUNT(LV_TR_BR_TEMP, '#') + 1
                            )
                            LOOP
                                IF LV_TR_BRANCHES IS NULL THEN
                                    LV_TR_BRANCHES := LV_TR_I.BRANCH_PART;
                                ELSE
                                    LV_TR_BRANCHES := LV_TR_BRANCHES || ',' || LV_TR_I.BRANCH_PART;
                                END IF;
                            END LOOP;
                        END IF;
                        
                    EXCEPTION
                        WHEN OTHERS THEN
                            NULL; -- Continue to next branch
                    END;
                END LOOP;
                
                -- Set output parameters
                PX_OUT_TR_BR_NEW := LV_TR_BRANCHES;
                PX_OUT_STAT := 'SUCCESS';
                PX_OUT_MSG := 'TR Branches retrieved';
                
            EXCEPTION
                WHEN OTHERS THEN
                    PX_OUT_STAT := 'FAIL';
                    PX_OUT_MSG := 'Error in TR_BR_NEW: ' || SQLERRM;
            END;
        END;
    -- TR_BR_OLD functionality
    ELSIF PX_FOR = 'TR_BR_OLD' THEN
        BEGIN
            DECLARE
                LV_TRO_BRANCHES VARCHAR2(4000) := '';
                LV_TRO_BR_CODE VARCHAR2(4000);
                LV_TRO_BR_TEMP VARCHAR2(4000);
                LV_TRO_J NUMBER := 0;
            BEGIN
                -- Get branch IDs from userdetails_ji with data filter conditions
                FOR LV_TRO_REC IN (
                    SELECT UD.BRANCH_ID
                    FROM USERDETAILS_JI UD
                    WHERE UD.LOGIN_ID = PX_LOGID
                    AND UD.DATAFILTER IN (
                        SELECT DF.DATA_FILTER
                        FROM DATAFILTER_MASTER DF
                        WHERE DF.LOGIN_ID IN (
                            SELECT EM.PAYROLL_ID 
                            FROM EMPLOYEE_MASTER EM 
                            WHERE EM.TYPE = 'A' 
                            AND EM.CATEGORY_ID = 2001
                        )
                        AND DF.LOGIN_ID = PX_LOGID
                        AND DF.ROLE_DEFAULT = '1'
                    )
                )
                LOOP
                    -- Call GETBRANCH_TR function for each branch_id
                    BEGIN
                        SELECT GETBRANCH_TR(LV_TRO_REC.BRANCH_ID) INTO LV_TRO_BR_CODE FROM DUAL;
                        
                        IF LV_TRO_BR_CODE IS NOT NULL THEN
                            -- Remove first character and split by #
                            LV_TRO_BR_TEMP := SUBSTR(LV_TRO_BR_CODE, 2);
                            
                            -- Split the string by # and concatenate with comma
                            FOR LV_TRO_I IN (
                                SELECT TRIM(REGEXP_SUBSTR(LV_TRO_BR_TEMP, '[^#]+', 1, LEVEL)) AS BRANCH_PART
                                FROM DUAL
                                CONNECT BY LEVEL <= REGEXP_COUNT(LV_TRO_BR_TEMP, '#') + 1
                            )
                            LOOP
                                IF LV_TRO_BRANCHES IS NULL THEN
                                    LV_TRO_BRANCHES := LV_TRO_I.BRANCH_PART;
                                ELSE
                                    LV_TRO_BRANCHES := LV_TRO_BRANCHES || ',' || LV_TRO_I.BRANCH_PART;
                                END IF;
                            END LOOP;
                        END IF;
                        
                    EXCEPTION
                        WHEN OTHERS THEN
                            NULL; -- Continue to next branch
                    END;
                END LOOP;
                
                -- Set output parameters
                PX_OUT_TR_BR_OLD := LV_TRO_BRANCHES;
                PX_OUT_STAT := 'SUCCESS';
                PX_OUT_MSG := 'TR Branches Old retrieved';
                
            EXCEPTION
                WHEN OTHERS THEN
                    PX_OUT_STAT := 'FAIL';
                    PX_OUT_MSG := 'Error in TR_BR_OLD: ' || SQLERRM;
            END;
        END;
    END IF;
END PSM_LOG_PERMISSION;
/