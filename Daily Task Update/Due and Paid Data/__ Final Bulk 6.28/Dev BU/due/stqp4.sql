-- STEP 1: Update BAJAJ_AR_HEAD status in bulk
UPDATE BAJAJ_AR_HEAD h
SET status_cd = '&status_cd'
WHERE EXISTS (
    SELECT 1 
    FROM psm_dap_temp_due1 d
    WHERE h.POLICY_NO = d.policy_no
    AND h.COMPANY_CD = d.company_cd
    AND TO_CHAR(h.SYS_AR_DT,'MON-YYYY') = '&month_year'
    AND h.status_cd = '&current_status'
);

-- STEP 2: Bulk insert into BAJAJ_AR_DETAILS for updated records
INSERT INTO BAJAJ_AR_DETAILS
(SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON)
SELECT 
    h.SYS_AR_NO,
    LAST_DAY(h.SYS_AR_DT),
    '&status_cd',
    '&import_type' || ' ' || TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),
    h.SYS_AR_DT,
    SYSDATE
FROM 
    BAJAJ_AR_HEAD h
JOIN 
    psm_dap_temp_due1 d ON h.POLICY_NO = d.policy_no AND h.COMPANY_CD = d.company_cd
WHERE 
    TO_CHAR(h.SYS_AR_DT,'MON-YYYY') = '&month_year'
    AND h.status_cd = '&status_cd'
    AND NOT EXISTS (
        SELECT 1 
        FROM BAJAJ_AR_DETAILS d2
        WHERE d2.SYS_AR_NO = h.SYS_AR_NO
        AND d2.STATUS_DT = LAST_DAY(h.SYS_AR_DT)
    );

COMMIT;