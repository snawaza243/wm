CREATE OR REPLACE PROCEDURE PSM_LOG_PERMISSION1(
    PX_LOGID        IN VARCHAR2,
    PX_PASSWORD     IN VARCHAR2,
    PX_FOR          IN VARCHAR2 DEFAULT NULL,
    PX_ROLE_NAMES   OUT VARCHAR2,
    PX_LOGIN_ID     OUT VARCHAR2,
    PX_STATUS       OUT VARCHAR2,
    PX_MESSAGE      OUT VARCHAR2
) AS 
    V_ROLE_IDS      VARCHAR2(4000);
    V_ROLE_NAME     VARCHAR2(4000);
    V_ROLE_COUNT    NUMBER := 0;
    V_SPECIAL_CHAR  VARCHAR2(1) := '|';
    V_DYNAMIC_VAR   VARCHAR2(10) := 'RPT_';
BEGIN
    -- Initialize OUT parameters
    PX_ROLE_NAMES := '';
    PX_LOGIN_ID := '';
    PX_STATUS := 'FAIL';
    PX_MESSAGE := '';
    
    -- ROLEPERMISSION functionality
    IF PX_FOR = 'ROLEPERMISSION' THEN
        BEGIN
            -- Get user record (equivalent to VB recordset)
            SELECT Role_id, login_id INTO V_ROLE_IDS, PX_LOGIN_ID
            FROM user_master 
            WHERE login_id = PX_LOGID 
            AND login_pass is not null --= PX_PASSWORD
            AND status = '1' ;
            --AND LOGINVARIFY(PX_LOGID) = 1;
            
            -- Record Found (equivalent to VB RecordCount > 0)
            PX_STATUS := 'SUCCESS';
            PX_MESSAGE := 'Record Found';
            
            -- Check for multiple roles (equivalent to InStr check for comma)
            IF INSTR(V_ROLE_IDS, ',') > 0 THEN
                -- Multiple roles - iterate through them
                FOR i IN (SELECT TRIM(REGEXP_SUBSTR(V_ROLE_IDS, '[^,]+', 1, LEVEL)) AS single_role
                         FROM DUAL
                         CONNECT BY LEVEL <= REGEXP_COUNT(V_ROLE_IDS, ',') + 1)
                LOOP
                    -- Get role name for each role_id
                    BEGIN
                        SELECT role_name INTO V_ROLE_NAME
                        FROM role_master 
                        WHERE role_id = i.single_role;
                        
                        -- Add to role names with special character concatenation
                        IF V_ROLE_COUNT = 0 THEN
                            PX_ROLE_NAMES := V_ROLE_NAME || V_SPECIAL_CHAR || V_DYNAMIC_VAR || (V_ROLE_COUNT + 1);
                        ELSE
                            PX_ROLE_NAMES := PX_ROLE_NAMES || ',' || V_ROLE_NAME || V_SPECIAL_CHAR || V_DYNAMIC_VAR || (V_ROLE_COUNT + 1);
                        END IF;
                        
                        V_ROLE_COUNT := V_ROLE_COUNT + 1;
                        
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            NULL; -- Continue with next role
                    END;
                END LOOP;
            ELSE
                -- Single role
                BEGIN
                    SELECT role_name INTO V_ROLE_NAME
                    FROM role_master 
                    WHERE role_id = V_ROLE_IDS;
                    
                    -- Add single role with special character concatenation
                    PX_ROLE_NAMES := V_ROLE_NAME || V_SPECIAL_CHAR || V_DYNAMIC_VAR || '1';
                    V_ROLE_COUNT := 1;
                    
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        PX_MESSAGE := PX_MESSAGE || ' Role not found';
                END;
            END IF;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                -- No records found - equivalent to VB's Else block
                PX_STATUS := 'FAIL';
                PX_MESSAGE := 'You Are not Authorized';
            WHEN TOO_MANY_ROWS THEN
                PX_STATUS := 'FAIL';
                PX_MESSAGE := 'Multiple users found';
            WHEN OTHERS THEN
                PX_STATUS := 'FAIL';
                PX_MESSAGE := 'Error: ' || SQLERRM;
        END;
    END IF;
END PSM_LOG_PERMISSION1;
/