-- Option 1: Using UPDATE with EXISTS (best for most cases)
UPDATE /*+ PARALLEL(4) */ policy_details_master p
SET 
    p.last_status = 'L',
    p.update_prog = 'IMPORT_TYPE',
    p.update_user = 'LOGIN_ID',
    p.update_date = TO_DATE('25-JUN-2025', 'DD-MON-YYYY')
WHERE EXISTS (
    SELECT 1
    FROM psm_dap_temp_due1 d
    WHERE UPPER(TRIM(p.policy_no)) = UPPER(TRIM(d.policy_no))
    AND UPPER(TRIM(p.company_cd)) = UPPER(TRIM(d.company_cd))
);

-- Option 2: Using MERGE (best for very large datasets)
MERGE /*+ PARALLEL(4) */ INTO policy_details_master p
USING (
    SELECT 
        UPPER(TRIM(policy_no)) as policy_no,
        UPPER(TRIM(company_cd)) as company_cd
    FROM psm_dap_temp_due1
) d
ON (
    UPPER(TRIM(p.policy_no)) = d.policy_no
    AND UPPER(TRIM(p.company_cd)) = d.company_cd
)
WHEN MATCHED THEN UPDATE SET
    p.last_status = 'L',
    p.update_prog = 'IMPORT_TYPE',
    p.update_user = 'LOGIN_ID',
    p.update_date = TO_DATE('25-JUN-2025', 'DD-MON-YYYY');

COMMIT;