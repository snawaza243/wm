DECLARE
    VPX_LOGID            VARCHAR2(1000);
    VPX_ROLEID           VARCHAR2(1000);
    VPX_PASS             VARCHAR2(1000);
    VPX_FOR              VARCHAR2(1000);
    VPX_FORM             VARCHAR2(1000);
    VPX_OUT_ROLES        VARCHAR2(1000);
    VPX_OUT_LID          VARCHAR2(1000);
    VPX_OUT_STAT         VARCHAR2(1000);
    VPX_OUT_MSG          VARCHAR2(1000);
    VPX_OUT_BTNS         VARCHAR2(1000);
    VPX_OUT_DATA_PERM    VARCHAR2(1000);
    VPX_OUT_TR_BR_NEW    VARCHAR2(1000);
    VPX_OUT_TR_BR_OLD    VARCHAR2(1000);
    VPX_OUT_SHOW_PROD    VARCHAR2(1000);
BEGIN
    -- Initialize input parameters with test data
    VPX_LOGID := '39339';      -- Replace with actual login ID
    VPX_ROLEID := '212';       -- Replace with actual role ID
    VPX_PASS := '212';         -- Replace with actual password
    VPX_FOR := 'SHOW_PRODUCT'; -- Options: 'ROLEPERMISSION', 'BUTTONPERMISSION', 'DATAPERMISSION', 'SHOW_PRODUCT', 'TR_BR_NEW', 'TR_BR_OLD'
    VPX_FORM := NULL;          -- Only needed for BUTTONPERMISSION
    
    -- Initialize output variables
    VPX_OUT_ROLES := NULL;
    VPX_OUT_LID := NULL;
    VPX_OUT_STAT := NULL;
    VPX_OUT_MSG := NULL;
    VPX_OUT_BTNS := NULL;
    VPX_OUT_DATA_PERM := NULL;
    VPX_OUT_TR_BR_NEW := NULL;
    VPX_OUT_TR_BR_OLD := NULL;
    VPX_OUT_SHOW_PROD := NULL;

    -- Call the procedure
    WEALTHMAKER.PSM_LOG_PERMISSION(
        PX_LOGID => VPX_LOGID,
        PX_ROLEID => VPX_ROLEID,
        PX_PASS => VPX_PASS,
        PX_FOR => VPX_FOR,
        PX_FORM => VPX_FORM,
        PX_OUT_ROLES => VPX_OUT_ROLES,
        PX_OUT_LID => VPX_OUT_LID,
        PX_OUT_STAT => VPX_OUT_STAT,
        PX_OUT_MSG => VPX_OUT_MSG,
        PX_OUT_BTNS => VPX_OUT_BTNS,
        PX_OUT_DATA_PERM => VPX_OUT_DATA_PERM,
        PX_OUT_TR_BR_NEW => VPX_OUT_TR_BR_NEW,
        PX_OUT_TR_BR_OLD => VPX_OUT_TR_BR_OLD,
        PX_OUT_SHOW_PROD => VPX_OUT_SHOW_PROD
    );

    -- Display the results
    DBMS_OUTPUT.PUT_LINE('=== PROCEDURE EXECUTION RESULTS ===');
    DBMS_OUTPUT.PUT_LINE('Status: ' || VPX_OUT_STAT);
    DBMS_OUTPUT.PUT_LINE('Message: ' || VPX_OUT_MSG);
    DBMS_OUTPUT.PUT_LINE('Login ID: ' || VPX_OUT_LID);
    
    -- Display output based on the function type
    CASE VPX_FOR
        WHEN 'ROLEPERMISSION' THEN
            DBMS_OUTPUT.PUT_LINE('Roles: ' || VPX_OUT_ROLES);
        WHEN 'BUTTONPERMISSION' THEN
            DBMS_OUTPUT.PUT_LINE('Buttons: ' || VPX_OUT_BTNS);
        WHEN 'DATAPERMISSION' THEN
            DBMS_OUTPUT.PUT_LINE('Data Permissions: ' || VPX_OUT_DATA_PERM);
        WHEN 'SHOW_PRODUCT' THEN
            DBMS_OUTPUT.PUT_LINE('Product Codes: ' || VPX_OUT_SHOW_PROD);
        WHEN 'TR_BR_NEW' THEN
            DBMS_OUTPUT.PUT_LINE('TR Branches New: ' || VPX_OUT_TR_BR_NEW);
        WHEN 'TR_BR_OLD' THEN
            DBMS_OUTPUT.PUT_LINE('TR Branches Old: ' || VPX_OUT_TR_BR_OLD);
        ELSE
            DBMS_OUTPUT.PUT_LINE('Roles: ' || VPX_OUT_ROLES);
            DBMS_OUTPUT.PUT_LINE('Buttons: ' || VPX_OUT_BTNS);
            DBMS_OUTPUT.PUT_LINE('Data Permissions: ' || VPX_OUT_DATA_PERM);
            DBMS_OUTPUT.PUT_LINE('Show Products: ' || VPX_OUT_SHOW_PROD);
            DBMS_OUTPUT.PUT_LINE('TR Branches New: ' || VPX_OUT_TR_BR_NEW);
            DBMS_OUTPUT.PUT_LINE('TR Branches Old: ' || VPX_OUT_TR_BR_OLD);
    END CASE;
    
    DBMS_OUTPUT.PUT_LINE('==================================');
END;
/