-- Get current roll ID by login id
BEGIN
    SELECT role_id 
    INTO currentRole 
    FROM USERDETAILS_Ji where LOGIN_ID = P_LOGGEDIN_USER AND ROLE_ID  = 212 AND ROWNUM = 1;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        currentRole := 0; 
END;