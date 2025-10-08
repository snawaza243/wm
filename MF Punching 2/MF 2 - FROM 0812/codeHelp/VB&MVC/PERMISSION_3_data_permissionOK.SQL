CREATE OR REPLACE PROCEDURE PSM_LOG_PERMISSION(
    PX_LOGID    IN VARCHAR2,
    PX_PASS     IN VARCHAR2,
    PX_FOR      IN VARCHAR2 DEFAULT NULL,
    PX_FORM     IN VARCHAR2 DEFAULT NULL,
    PX_OUT_ROLES OUT VARCHAR2,
    PX_OUT_LID OUT VARCHAR2,
    PX_OUT_STAT OUT VARCHAR2,
    PX_OUT_MSG OUT VARCHAR2,
    PX_OUT_BTNS OUT VARCHAR2,
    PX_OUT_DATA_PERM OUT VARCHAR2  -- New output for data permission
) AS 
    -- Special characters for concatenation patterns
    LV_KV_SEP VARCHAR2(1) := '#';   -- Key-Value separator
    LV_PAIR_SEP VARCHAR2(1) := '@'; -- Pair separator
    LV_ITEM_SEP VARCHAR2(1) := ','; -- Item separator
    LV_SP_CHR VARCHAR2(1) := '|';   -- Special character
    LV_DYN_VAR VARCHAR2(10) := 'RPT_'; -- Dynamic variable prefix
    
    -- Local variables for ROLEPERMISSION
    LV_RP_ROLE_IDS VARCHAR2(4000);
    LV_RP_ROLE_NM VARCHAR2(4000);
    LV_RP_ROLE_ID VARCHAR2(100);
    LV_RP_COUNT NUMBER := 0;
    
    -- Local variables for BUTTONPERMISSION
    LV_BP_VIEW VARCHAR2(1);
    LV_BP_ADD VARCHAR2(1);
    LV_BP_UPD VARCHAR2(1);
    LV_BP_PRINT VARCHAR2(1);
    LV_BP_MAIN_FORM VARCHAR2(100);
    LV_BP_FRM_CNT NUMBER := 0;
    
    -- Local variables for DATAPERMISSION
    LV_DP_UP_PRE_DUR VARCHAR2(20);
    LV_DP_UP_NEXT_DUR VARCHAR2(20);
    LV_DP_IN_PRE_DUR VARCHAR2(20);
    LV_DP_IN_NEXT_DUR VARCHAR2(20);
    LV_DP_UP_PRE_TYPE VARCHAR2(20);
    LV_DP_UP_NEXT_TYPE VARCHAR2(20);
    LV_DP_IN_PRE_TYPE VARCHAR2(20);
    LV_DP_IN_NEXT_TYPE VARCHAR2(20);
    LV_DP_DATA_FILTER VARCHAR2(4000);
    LV_DP_IS_LOCKED VARCHAR2(1) := 'N';
    LV_DP_IS_LOCKED_LI VARCHAR2(1) := 'N';
    LV_DP_INS_PREV_DATE VARCHAR2(20);
    LV_DP_INS_NEXT_DATE VARCHAR2(20);
    LV_DP_UP_PREV_DATE VARCHAR2(20);
    LV_DP_UP_NEXT_DATE VARCHAR2(20);
    LV_DP_FIN_YR_FR VARCHAR2(20);
    LV_DP_FIN_YR_TO VARCHAR2(20);
    LV_DP_SERVER_DATE DATE;
BEGIN
    -- Initialize OUT parameters
    PX_OUT_ROLES := '';
    PX_OUT_LID := '';
    PX_OUT_STAT := 'FAIL';
    PX_OUT_MSG := '';
    PX_OUT_BTNS := '';
    PX_OUT_DATA_PERM := '';
    
    -- Get server date
    SELECT SYSDATE INTO LV_DP_SERVER_DATE FROM DUAL;
    
    -- ROLEPERMISSION functionality
    IF PX_FOR = 'ROLEPERMISSION' THEN
        BEGIN
            -- Get user record
            SELECT Role_id, login_id INTO LV_RP_ROLE_IDS, PX_OUT_LID
            FROM user_master 
            WHERE login_id = PX_LOGID 
            AND login_pass = PX_PASS
            AND status = '1' 
            AND LOGINVARIFY(PX_LOGID) = 1;
            
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
                            PX_OUT_ROLES := LV_RP_ROLE_NM || LV_KV_SEP || LV_RP_ROLE_ID || 
                                          LV_SP_CHR || LV_DYN_VAR || (LV_RP_COUNT + 1);
                        ELSE
                            PX_OUT_ROLES := PX_OUT_ROLES || LV_ITEM_SEP || LV_RP_ROLE_NM || LV_KV_SEP || LV_RP_ROLE_ID || 
                                          LV_SP_CHR || LV_DYN_VAR || (LV_RP_COUNT + 1);
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
                    PX_OUT_ROLES := LV_RP_ROLE_NM || LV_KV_SEP || LV_RP_ROLE_ID || 
                                  LV_SP_CHR || LV_DYN_VAR || '1';
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
                WHERE RFP.ROLE_ID = PX_PASS
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
        BEGIN
            -- Get role data permissions
            SELECT 
                UP_PRE_DUR, UP_NEXT_DUR, IN_PRE_DUR, IN_NEXT_DUR,
                UP_PRE_DUR_TYPE, UP_NEXT_DUR_TYPE, IN_PRE_DUR_TYPE, IN_NEXT_DUR_TYPE
            INTO 
                LV_DP_UP_PRE_DUR, LV_DP_UP_NEXT_DUR, LV_DP_IN_PRE_DUR, LV_DP_IN_NEXT_DUR,
                LV_DP_UP_PRE_TYPE, LV_DP_UP_NEXT_TYPE, LV_DP_IN_PRE_TYPE, LV_DP_IN_NEXT_TYPE
            FROM ROLE_MASTER 
            WHERE ROLE_ID = PX_PASS;  -- Using PX_PASS as role_id
            
            -- Calculate date ranges based on types
            LV_DP_INS_PREV_DATE := PSM_CALC_DATE_RANGE(LV_DP_IN_PRE_DUR, LV_DP_IN_PRE_TYPE, 'PREV', LV_DP_SERVER_DATE);
            LV_DP_INS_NEXT_DATE := PSM_CALC_DATE_RANGE(LV_DP_IN_NEXT_DUR, LV_DP_IN_NEXT_TYPE, 'NEXT', LV_DP_SERVER_DATE);
            LV_DP_UP_PREV_DATE := PSM_CALC_DATE_RANGE(LV_DP_UP_PRE_DUR, LV_DP_UP_PRE_TYPE, 'PREV', LV_DP_SERVER_DATE);
            LV_DP_UP_NEXT_DATE := PSM_CALC_DATE_RANGE(LV_DP_UP_NEXT_DUR, LV_DP_UP_NEXT_TYPE, 'NEXT', LV_DP_SERVER_DATE);
            
            -- Get data filter
            BEGIN
                SELECT DATA_FILTER INTO LV_DP_DATA_FILTER
                FROM DATAFILTER_MASTER 
                WHERE ROLE_ID = PX_PASS 
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
    END IF;
END PSM_LOG_PERMISSION;
/

-- Supporting function for date calculations
CREATE OR REPLACE FUNCTION PSM_CALC_DATE_RANGE(
    P_DAYS IN VARCHAR2,
    P_TYPE IN VARCHAR2,
    P_DIRECTION IN VARCHAR2,
    P_SERVER_DATE IN DATE
) RETURN VARCHAR2 IS
    V_RESULT_DATE DATE;
    V_DAYS_NUMBER NUMBER;
    V_FIN_YR_FR DATE;
    V_FIN_YR_TO DATE;
    V_TEMP_DATE DATE;
BEGIN
    V_DAYS_NUMBER := TO_NUMBER(P_DAYS);
    V_RESULT_DATE := P_SERVER_DATE;
    
    IF P_DIRECTION = 'PREV' THEN
        -- Previous date calculation
        IF P_TYPE = '1' THEN -- Days
            V_RESULT_DATE := P_SERVER_DATE - V_DAYS_NUMBER;
        ELSIF P_TYPE = '2' THEN -- Months
            V_RESULT_DATE := ADD_MONTHS(P_SERVER_DATE, -V_DAYS_NUMBER);
        ELSIF P_TYPE = '3' THEN -- Years
            V_RESULT_DATE := ADD_MONTHS(P_SERVER_DATE, -V_DAYS_NUMBER * 12);
        END IF;
    ELSE
        -- Next date calculation (subtract 1 day as per VB logic)
        IF P_TYPE = '1' THEN -- Days
            V_RESULT_DATE := P_SERVER_DATE + (V_DAYS_NUMBER - 1);
        ELSIF P_TYPE = '2' THEN -- Months
            V_RESULT_DATE := ADD_MONTHS(P_SERVER_DATE, V_DAYS_NUMBER - 1);
        ELSIF P_TYPE = '3' THEN -- Years
            -- Get financial year dates
            BEGIN
                SELECT DATE_FR, DATE_TO INTO V_FIN_YR_FR, V_FIN_YR_TO
                FROM FINANCIAL_YEAR
                WHERE ROWNUM = 1;
                
                V_RESULT_DATE := ADD_MONTHS(V_FIN_YR_TO, (V_DAYS_NUMBER - 1) * 12);
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    V_RESULT_DATE := ADD_MONTHS(P_SERVER_DATE, (V_DAYS_NUMBER - 1) * 12);
            END;
        END IF;
    END IF;
    
    -- Return in DD/MM/YYYY format
    RETURN TO_CHAR(V_RESULT_DATE, 'DD/MM/YYYY');
EXCEPTION
    WHEN OTHERS THEN
        RETURN TO_CHAR(P_SERVER_DATE, 'DD/MM/YYYY');
END PSM_CALC_DATE_RANGE;
/




/*
-- DATAPERMISSION Call
DECLARE
    V_ROLES VARCHAR2(4000);
    V_LID VARCHAR2(100);
    V_STAT VARCHAR2(20);
    V_MSG VARCHAR2(1000);
    V_BTNS VARCHAR2(4000);
    V_DATA_PERM VARCHAR2(4000);
BEGIN
    PSM_LOG_PERMISSION(
        PX_LOGID => '39339',           -- Login ID
        PX_PASS => '212',            -- Role ID
        PX_FOR => 'DATAPERMISSION',      -- Function type
        PX_FORM => NULL,                 -- Not needed for data permission
        PX_OUT_ROLES => V_ROLES,
        PX_OUT_LID => V_LID,
        PX_OUT_STAT => V_STAT,
        PX_OUT_MSG => V_MSG,
        PX_OUT_BTNS => V_BTNS,
        PX_OUT_DATA_PERM => V_DATA_PERM  -- Output: Data permissions
    );
    
    DBMS_OUTPUT.PUT_LINE('Status: ' || V_STAT);
    DBMS_OUTPUT.PUT_LINE('Message: ' || V_MSG);
    DBMS_OUTPUT.PUT_LINE('Data Permissions: ' || V_DATA_PERM);
END;
/
*/

/*Status: SUCCESS
Message: No button restrictions
Roles:
Button Permissions: VIEW#1@ADD#1@UPDATE#1@PRINT#1
Status: SUCCESS
Message: Data permissions found
Data Permissions: UP_PRE_DUR#30@UP_NEXT_DUR#30@IN_PRE_DUR#30@IN_NEXT_DUR#30@UP_PRE_TYPE#2@UP_NEXT_TYPE#2@IN_PRE_TYPE#2@IN_NEXT_TYPE#2@DATA_FILTER#@USER_LOCKED#N@LI_LOCKED#N@INS_PREV_DATE#08/04/2023@INS_NEXT_DATE#08/03/2028@UP_PREV_DATE#08/04/2023@UP_NEXT_DATE#08/03/2028
*/
