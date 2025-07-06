IF P_IMP_TYPE = 'LAPSED' THEN
        -- Step 2: Bulk insert with proper data typing
INSERT INTO psm_dap_temp_due1_lap
(policy_no, company_cd, due_date, mon_no, year_no, status_cd, days_lapsed)
SELECT 
    d.policy_no,
    d.company_cd,
    MAX(a.due_date),
    MAX(a.mon_no),
    MAX(a.year_no),
    (
        SELECT MAX(b.status_cd)
        FROM bajaj_due_data b
        WHERE b.policy_no = d.policy_no
        AND b.company_cd = d.company_cd
        AND b.importdatatype = 'DUEDATA'
        AND b.due_date = (
SELECT MAX(c.due_date)
                FROM bajaj_due_data c
                join psm_dap_temp_due1 e on e.policy_no = c.policy_no
                WHERE
                    UPPER(TRIM(c.policy_no)) = e.policy_no
                    AND UPPER(TRIM(c.company_cd)) = e.company_cd 
                    AND c.cdue_date IS NOT NULL
                    AND c.importdatatype = 'DUEDATA'
        
        )
    ) as status_cd ,
    TRUNC(SYSDATE) - MAX(a.due_date)
FROM 
    psm_dap_temp_due1 d
JOIN 
    bajaj_due_data a ON a.policy_no = d.policy_no
                     AND a.company_cd = d.company_cd
WHERE 
    a.importdatatype = 'DUEDATA'
    AND a.due_date <= TRUNC(SYSDATE)
GROUP BY 
    d.policy_no, 
    d.company_cd;

COMMIT;

END IF;