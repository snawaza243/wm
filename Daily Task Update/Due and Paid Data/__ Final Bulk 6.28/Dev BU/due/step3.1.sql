-- Bulk update with values from psm_dap_temp_due1
UPDATE /*+ PARALLEL(4) */ bajaj_due_data b
SET (
    b.status_cd,
    b.last_update_dt,
    b.last_update
) = (
    SELECT 
        'LAPSED',
        TRUNC(SYSDATE),
        USER  -- Or replace with your login ID variable
    FROM dual
)
WHERE EXISTS (
    SELECT 1
    FROM psm_dap_temp_due1 d
    WHERE UPPER(TRIM(b.policy_no)) = UPPER(TRIM(d.policy_no))
    AND UPPER(TRIM(b.company_cd)) = UPPER(TRIM(d.company_cd))
    AND b.due_date = d.due_date
    AND b.importdatatype = 'DUEDATA'
    AND d.due_date <= TRUNC(SYSDATE)  -- Only update lapsed policies
);

COMMIT;



-- Alternative using MERGE (often faster for complex updates)
MERGE /*+ PARALLEL(4) */ INTO bajaj_due_data b
USING (
    SELECT 
        UPPER(TRIM(policy_no)) as policy_no,
        UPPER(TRIM(company_cd)) as company_cd,
        due_date
    FROM psm_dap_temp_due1
    WHERE due_date <= TRUNC(SYSDATE)
) d
ON (
    UPPER(TRIM(b.policy_no)) = d.policy_no
    AND UPPER(TRIM(b.company_cd)) = d.company_cd
    AND b.due_date = d.due_date
    AND b.importdatatype = 'DUEDATA'
)
WHEN MATCHED THEN UPDATE SET
    b.status_cd = 'LAPSED',
    b.last_update_dt = TRUNC(SYSDATE),
    b.last_update = USER;  -- Or your login ID variable