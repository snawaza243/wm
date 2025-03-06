CREATE OR REPLACE PROCEDURE PSM_DAP_Update_AR_Details (
    p_ua_ply_no IN VARCHAR2,
    p_ua_Comp_Cd IN VARCHAR2,
    p_ua_status_cd IN VARCHAR2,
    p_ua_import_type IN VARCHAR2
) IS
    -- Declare local variables
    v_sys_ar_no BAJAJ_AR_HEAD.sys_ar_no%TYPE;
    v_sys_ar_dt BAJAJ_AR_HEAD.sys_ar_dt%TYPE;
    v_record_exists number := 0;
BEGIN
    -- Step 1: Check if record exists in BAJAJ_AR_HEAD
    SELECT sys_ar_no, sys_ar_dt
    INTO v_sys_ar_no, v_sys_ar_dt
    FROM BAJAJ_AR_HEAD
    WHERE POLICY_NO = p_ua_ply_no
      AND COMPANY_CD = p_ua_Comp_Cd
      AND TO_CHAR(SYS_AR_DT, 'MON-YYYY') = UPPER(TO_CHAR(SYSDATE, 'MON-YYYY'))
      AND status_cd = p_ua_status_cd;

    -- Step 2: If record exists, check for details in BAJAJ_AR_DETAILS
    BEGIN
        -- Check for matching record in BAJAJ_AR_DETAILS
        SELECT 1
        INTO v_record_exists
        FROM BAJAJ_AR_DETAILS
        WHERE SYS_AR_NO = v_sys_ar_no
          AND status_dt = LAST_DAY(TO_DATE(v_sys_ar_dt, 'DD/MM/YYYY'));
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            v_record_exists := 0;  -- No matching record found
    END;

    -- Step 3: If record does not exist in BAJAJ_AR_DETAILS, insert new record
    IF v_record_exists = 0 THEN
        -- Update BAJAJ_AR_HEAD
        UPDATE BAJAJ_AR_HEAD
        SET status_cd = p_ua_status_cd
        WHERE sys_ar_no = v_sys_ar_no;

        -- Insert into BAJAJ_AR_DETAILS
        INSERT INTO BAJAJ_AR_DETAILS (
            SYS_AR_NO,
            STATUS_DT,
            STATUS_CD,
            REMARKS,
            SYS_AR_DT,
            STATUS_UPDATE_ON
        ) VALUES (
            v_sys_ar_no,
            LAST_DAY(TO_DATE(v_sys_ar_dt, 'DD-MMM-YYYY')),
            p_ua_status_cd,
            p_ua_import_type || ' ' || SYSDATE,
            TO_DATE(v_sys_ar_dt, 'DD-MMM-YYYY'),
            SYSDATE
        );
    END IF;

    -- Commit the transaction
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        -- Rollback in case of any errors
        ROLLBACK;
        RAISE;
END PSM_DAP_Update_AR_Details;
/
