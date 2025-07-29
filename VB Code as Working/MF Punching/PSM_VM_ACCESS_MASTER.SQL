-- THIS PROCEDRUE IS USED TO PROVIDE THE MASTER ACCESS TO THE VM
-- IT WILL RETURN TWO OUT PARAMETERS WITH 0/1 FOR TRUE/FALSE, AND MESSAGE STRING

CREATE OR REPLACE PROCEDURE PSM_VM_ACCESS_MASTER(
    P_LOGIN_ID          IN VARCHAR2,
    P_ROLE_ID           IN VARCHAR2,
    P_MODULE_ACTION     IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
   )
 AS
    v_has_access        NUMBER := 0;
    v_message           VARCHAR2(200);
    v_valid_roles       VARCHAR2(100);
BEGIN

        -- Determine which roles to check based on button type
    IF P_MODULE_ACTION = 'ao_approve' THEN
        v_valid_roles := '212,22,218';   
    ELSIF P_MODULE_ACTION = 'ao_insert' THEN
        v_valid_roles := '212';                 
    ELSIF P_MODULE_ACTION = 'ao_update' THEN
        v_valid_roles := '212,218';          
               
    ELSIF P_MODULE_ACTION = 'ao_bss' THEN
        v_valid_roles := '212,22'; 

    ELSIF P_MODULE_ACTION = 'nps_save' THEN
        v_valid_roles := '212'; 

    ELSIF P_MODULE_ACTION = 'nps_update' THEN
        v_valid_roles := '212'; 

    ELSE
        v_valid_roles := '-1';          
    END IF;

    
    SELECT COUNT(*) INTO v_has_access
    FROM userdetails_ji 
    WHERE LOGIN_ID = p_login 
    AND role_id IN (
      --v_valid_roles
      SELECT * FROM TABLE(WEALTHMAKER.data_SPLIT(v_valid_roles,','))
    );



    -- Set appropriate message
    IF v_has_access > 0 THEN
        v_message := 'You have ' || p_button_id || ' access with role ' || p_role;
    ELSE 
        v_message := 'You does not have required role access';
        --for ' || p_button_id || 
        --            ' (required roles: ' || v_valid_roles || ')';
    END IF;
    
    -- Return both status and message
    OPEN p_cursor FOR
    SELECT 
        CASE WHEN v_has_access > 0 THEN 'true' ELSE 'false' END AS status,
        v_message AS message
    FROM dual;
 
EXCEPTION
    WHEN OTHERS THEN
    RAISE_APPLICATION_ERROR(-number, 'error-text');

        -- Handle any exceptions and set the message
        P_STATUS := 0;
        P_MESSAGE := 'Error: ' || SQLERRM;
END PSM_VM_ACCESS_MASTER;
    