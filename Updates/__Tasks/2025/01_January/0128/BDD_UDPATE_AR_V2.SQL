CREATE OR REPLACE PROCEDURE PSM_DAP_Update_AR_Details(
    p_uba_ply_no      IN VARCHAR2,
    p_uba_comp_cd     IN VARCHAR2,
    p_uba_status_cd   IN VARCHAR2,
    p_uba_month       IN VARCHAR2,
    p_uba_year        IN VARCHAR2,
    p_uba_import_type IN VARCHAR2
)
IS
    v_sys_ar_no      VARCHAR2(50)   := NULL;
    v_sys_ar_dt      DATE;
    v_exists         NUMBER         := 0;
BEGIN
    -- Find record in BAJAJ_AR_HEAD
    BEGIN
        SELECT SYS_AR_NO, SYS_AR_DT
        INTO v_sys_ar_no, v_sys_ar_dt
        FROM BAJAJ_AR_HEAD
        WHERE POLICY_NO = p_uba_ply_no
          AND COMPANY_CD = p_uba_comp_cd
          AND TO_CHAR(SYS_AR_DT, 'MON-YYYY') = p_uba_month || '-' || p_uba_year
          AND STATUS_CD = p_uba_status_cd;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            v_sys_ar_no := NULL;
            v_sys_ar_dt := NULL;
    END;

    IF v_sys_ar_no IS NOT NULL THEN
        BEGIN
            -- Check if record exists in BAJAJ_AR_DETAILS
            BEGIN
                SELECT COUNT(*)
                INTO v_exists
                FROM BAJAJ_AR_DETAILS
                WHERE SYS_AR_NO = v_sys_ar_no
                  AND STATUS_DT = LAST_DAY(v_sys_ar_dt);
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    v_exists := 0;
            END;

            IF v_exists = 0 THEN
                -- Update status in BAJAJ_AR_HEAD
                UPDATE BAJAJ_AR_HEAD
                SET STATUS_CD = p_uba_status_cd
                WHERE SYS_AR_NO = v_sys_ar_no;

                -- Insert record into BAJAJ_AR_DETAILS
                INSERT INTO BAJAJ_AR_DETAILS (
                    SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON
                )
                VALUES (
                    v_sys_ar_no,
                    LAST_DAY(v_sys_ar_dt),
                    p_uba_status_cd,
                    p_uba_import_type || ' ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'),
                    v_sys_ar_dt,
                    SYSDATE
                );
            END IF;
        END;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Log or re-raise the exception
        RAISE;
END PSM_DAP_Update_AR_Details;
/
