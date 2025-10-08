CREATE OR REPLACE PROCEDURE PSM_LOG_PERMISSION(
    PX_LOGID    IN VARCHAR2,
    PX_PASS     IN VARCHAR2,
    PX_FOR      IN VARCHAR2 DEFAULT NULL,
    PX_FORM     IN VARCHAR2 DEFAULT NULL,
    PX_OUT_ROLES OUT VARCHAR2,
    PX_OUT_LID OUT VARCHAR2,
    PX_OUT_STAT OUT VARCHAR2,
    PX_OUT_MSG OUT VARCHAR2,
    PX_OUT_BTNS OUT VARCHAR2
) AS 
    -- Local variables for ROLEPERMISSION
    LV_RP_ROLE_IDS VARCHAR2(4000);
    LV_RP_ROLE_NM VARCHAR2(4000);
    LV_RP_ROLE_ID VARCHAR2(100);
    LV_RP_COUNT NUMBER := 0;
    LV_RP_SP_CHR VARCHAR2(1) := '|';
    LV_RP_DYN_VAR VARCHAR2(10) := 'RPT_';
    
    -- Local variables for BUTTONPERMISSION
    LV_BP_VIEW VARCHAR2(1);
    LV_BP_ADD VARCHAR2(1);
    LV_BP_UPD VARCHAR2(1);
    LV_BP_PRINT VARCHAR2(1);
    LV_BP_MAIN_FORM VARCHAR2(100);
    LV_BP_FRM_CNT NUMBER := 0;
BEGIN
    -- Initialize OUT parameters
    PX_OUT_ROLES := '';
    PX_OUT_LID := '';
    PX_OUT_STAT := 'FAIL';
    PX_OUT_MSG := '';
    PX_OUT_BTNS := '';
    
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
                        
                        -- Add to role names with format: role_name#role_id|RPT_iteration
                        IF LV_RP_COUNT = 0 THEN
                            PX_OUT_ROLES := LV_RP_ROLE_NM || '#' || LV_RP_ROLE_ID || 
                                          LV_RP_SP_CHR || LV_RP_DYN_VAR || (LV_RP_COUNT + 1);
                        ELSE
                            PX_OUT_ROLES := PX_OUT_ROLES || ',' || LV_RP_ROLE_NM || '#' || LV_RP_ROLE_ID || 
                                          LV_RP_SP_CHR || LV_RP_DYN_VAR || (LV_RP_COUNT + 1);
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
                    
                    -- Add single role with format: role_name#role_id|RPT_1
                    PX_OUT_ROLES := LV_RP_ROLE_NM || '#' || LV_RP_ROLE_ID || 
                                  LV_RP_SP_CHR || LV_RP_DYN_VAR || '1';
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
                            -- Build button permissions string
                            PX_OUT_BTNS := 
                                'VIEW:' || LV_BP_REC.VIEW_BUTTON || '|' ||
                                'ADD:' || LV_BP_REC.ADD_BUTTON || '|' ||
                                'UPDATE:' || LV_BP_REC.UPDATE_BUTTON || '|' ||
                                'PRINT:' || LV_BP_REC.PRINT_BUTTON;
                            
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
                PX_OUT_BTNS := 'VIEW:1|ADD:1|UPDATE:1|PRINT:1'; -- All buttons enabled
            END IF;
            
        EXCEPTION
            WHEN OTHERS THEN
                PX_OUT_STAT := 'FAIL';
                PX_OUT_MSG := 'Error in button permission: ' || SQLERRM;
        END;
    END IF;
END PSM_LOG_PERMISSION;
/

/*

-- For ROLEPERMISSION
DECLARE
    V_ROLES VARCHAR2(4000);
    V_LID VARCHAR2(100);
    V_STAT VARCHAR2(20);
    V_MSG VARCHAR2(1000);
    V_BTNS VARCHAR2(4000);
BEGIN
    PSM_LOG_PERMISSION(
        PX_LOGID => '39339',
        PX_PASS => 'password123',
        PX_FOR => 'ROLEPERMISSION',
        PX_FORM => NULL,
        PX_OUT_ROLES => V_ROLES,
        PX_OUT_LID => V_LID,
        PX_OUT_STAT => V_STAT,
        PX_OUT_MSG => V_MSG,
        PX_OUT_BTNS => V_BTNS
    );
END;
/
*/