CREATE OR REPLACE PROCEDURE PSM_LOG_PERMISSION(
    PX_LOGID    IN VARCHAR2,
    PX_PASSWORD IN VARCHAR2,
    PX_ROLEID   IN VARCHAR2,
    PX_FOR      IN VARCHAR2 DEFAULT NULL,
    PX_CURSOR   OUT SYS_REFCURSOR
) AS 
    V_ROLE_COUNT NUMBER;
    V_ROLE_IDS VARCHAR2(4000);
    V_SPLIT_ROLE VARCHAR2(100);
    V_ITERATION NUMBER := 0;
    V_SPECIAL_CHAR VARCHAR2(1) := '|';
    V_DYNAMIC_VAR VARCHAR2(10) := 'RPT_';
BEGIN
    -- ROLEPERMISSION functionality
    IF PX_FOR = 'ROLEPERMISSION' THEN
        BEGIN
            -- First, validate user credentials and get role information
            SELECT Role_id INTO V_ROLE_IDS
            FROM user_master 
            WHERE login_id = PX_LOGID 
            AND login_pass = PX_PASSWORD
            AND status = '1' 
            AND LOGINVARIFY(PX_LOGID) = 1;
            
            -- If we reach here, record was found
            -- LogPrint "Record Found" equivalent would be handled in application layer
            
            -- Check if multiple roles exist (comma separated)
            IF INSTR(V_ROLE_IDS, ',') > 0 THEN
                -- Multiple roles - iterate through them
                FOR i IN (SELECT TRIM(REGEXP_SUBSTR(V_ROLE_IDS, '[^,]+', 1, LEVEL)) AS single_role
                         FROM DUAL
                         CONNECT BY LEVEL <= REGEXP_COUNT(V_ROLE_IDS, ',') + 1)
                LOOP
                    V_ITERATION := V_ITERATION + 1;
                    V_SPLIT_ROLE := i.single_role;
                    
                    -- Get role names for each role_id with special character concatenation
                    OPEN PX_CURSOR FOR
                    SELECT role_name,
                           role_name || V_SPECIAL_CHAR || V_DYNAMIC_VAR || V_ITERATION AS role_name_special,
                           V_ITERATION as iteration_number
                    FROM role_master 
                    WHERE role_id = V_SPLIT_ROLE;
                    
                END LOOP;
            ELSE
                -- Single role
                V_ITERATION := 1;
                
                -- Get role name with special character concatenation
                OPEN PX_CURSOR FOR
                SELECT role_name,
                       role_name || V_SPECIAL_CHAR || V_DYNAMIC_VAR || V_ITERATION AS role_name_special,
                       V_ITERATION as iteration_number
                FROM role_master 
                WHERE role_id = V_ROLE_IDS;
            END IF;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                -- No records found - equivalent to VB's Else block
                -- "You Are not Authorized" message would be handled in application
                OPEN PX_CURSOR FOR
                SELECT 'NOT_AUTHORIZED' as status FROM DUAL;
            WHEN TOO_MANY_ROWS THEN
                OPEN PX_CURSOR FOR
                SELECT 'MULTIPLE_RECORDS' as status FROM DUAL;
            WHEN OTHERS THEN
                OPEN PX_CURSOR FOR
                SELECT 'ERROR' as status FROM DUAL;
        END;
    END IF;
END PSM_LOG_PERMISSION;
/

-- Example of how to call this procedure:
/*
DECLARE
    V_CURSOR SYS_REFCURSOR;
    V_ROLE_NAME VARCHAR2(100);
    V_ROLE_SPECIAL VARCHAR2(150);
    V_ITER NUMBER;
BEGIN
    PSM_LOG_PERMISSION(
        PX_LOGID => 'user123',
        PX_PASSWORD => 'password123',
        PX_ROLEID => NULL,
        PX_FOR => 'ROLEPERMISSION',
        PX_CURSOR => V_CURSOR
    );
    
    LOOP
        FETCH V_CURSOR INTO V_ROLE_NAME, V_ROLE_SPECIAL, V_ITER;
        EXIT WHEN V_CURSOR%NOTFOUND;
        DBMS_OUTPUT.PUT_LINE('Role: ' || V_ROLE_NAME || ', Special: ' || V_ROLE_SPECIAL || ', Iteration: ' || V_ITER);
    END LOOP;
    CLOSE V_CURSOR;
END;
*/