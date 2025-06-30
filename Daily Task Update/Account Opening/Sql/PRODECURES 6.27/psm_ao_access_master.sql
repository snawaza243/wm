CREATE OR REPLACE PROCEDURE psm_ao_role_master(
    p_login     IN VARCHAR2,
    p_role      IN VARCHAR2,
    p_button_id IN VARCHAR2,
    p_cursor    OUT SYS_REFCURSOR)
AS
    v_has_access NUMBER := 0;
    v_message    VARCHAR2(200);
    v_valid_roles VARCHAR2(100);
BEGIN
     
    IF p_button_id = 'ao_approve' THEN
        v_valid_roles := '212,22'; 
    ELSIF p_button_id = 'ao_insert' or p_button_id = 'ao_update'THEN
        v_valid_roles := '212'; 
    ELSIF p_button_id = 'ao_bss' THEN  -- change bussiness code
        v_valid_roles := '22';  
    ELSE
        v_valid_roles := '-1';      
    END IF;

    -- Check if user has any of the required roles
    SELECT COUNT(*) INTO v_has_access
    FROM userdetails_ji 
    WHERE LOGIN_ID = p_login 
    AND role_id IN (
      v_valid_roles
    );

    -- Set  message
    IF v_has_access > 0 THEN
        v_message := 'User has ' || p_button_id || ' access with role ' || p_role;
    ELSE 
        v_message := 'User does not have required role for ' || p_button_id || 
                    ' (required roles: ' || v_valid_roles || ')';
    END IF;
    
    -- Return both status and message
    OPEN p_cursor FOR
    SELECT 
        CASE WHEN v_has_access > 0 THEN 'true' ELSE 'false' END AS status,      
        v_message AS message
    FROM dual;
END;
/