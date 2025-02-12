create or replace PROCEDURE PSM_IRP_SEARCH_BDD_By_COMPOL(
    p_policy_no IN VARCHAR2,
    p_company   IN VARCHAR2,
    p_cursor    OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
        SELECT 
            a.ar_branch_cd, 
            a.company_cd, 
            a.cl_name, 
            DECODE(c.address1, NULL, a.cl_add1, c.address1) AS cl_add1, 
            DECODE(c.address2, NULL, a.cl_add2, c.address2) AS cl_add2, 
            NULL AS cl_add3, 
            NULL AS cl_add4, 
            NULL AS cl_add5, 
            DECODE(c.phone1, NULL, c.phone2, c.phone1) AS cl_phone1, 
            DECODE(c.mobile, NULL, c.mobile1, c.mobile) AS cl_phone2, 
            c.city_name AS cl_city, 
            a.due_date, 
            a.rem_flage, 
            (SELECT state_name FROM state_master WHERE state_id = a.state_cd) AS state_name, 
            (SELECT company_name FROM bajaj_company_master WHERE company_cd = a.company_cd) AS company_name, 
            (SELECT favour_name FROM favour_master WHERE company_cd = a.company_cd) AS favour_name, 
            (SELECT branch_name FROM branch_master WHERE branch_code = c.sourceid) AS branch_name, 
            (SELECT address1 FROM branch_master WHERE branch_code = c.sourceid) AS branch_add1, 
            (SELECT address2 FROM branch_master WHERE branch_code = c.sourceid) AS branch_add2, 
            (SELECT PLAN FROM bajaj_plan_master WHERE plan_no = a.plan_no) AS plan_name1, 
            a.pay_mode, 
            a.policy_no, 
            a.p_name, 
            a.i_name, 
            CASE a.prem_freq 
                WHEN '1' THEN 'ANNUALLY' 
                WHEN '12' THEN 'MONTHLY' 
                WHEN '4' THEN 'QUARTERLY' 
                WHEN '2' THEN 'SEMI-ANNUALLY' 
            END AS prem_freq, 
            a.bprem_freq, 
            a.plan_name, 
            DECODE(a.sa, NULL, (SELECT MAX(f.sa) FROM bajaj_ar_head f WHERE f.company_cd = a.company_cd AND f.policy_no = a.policy_no), a.sa) AS sa, 
            a.prem_amt, 
            a.mon_no, 
            a.year_no, 
            c.pincode AS cl_pin, 
            (SELECT MAX(pin) FROM bajaj_ar_head WHERE UPPER(TRIM(company_cd)) = UPPER(TRIM(a.company_cd)) AND UPPER(TRIM(policy_no)) = UPPER(TRIM(a.policy_no))) AS pin1, 
            SUBSTR(a.inv_cd, 1, 8) AS inv_code, 
            a.inv_cd AS inv_code1, 
            a.importdatatype
        FROM 
            bajaj_due_data a
        JOIN 
            client_master c 
            ON SUBSTR(a.inv_cd, 1, 8) = c.client_code
        JOIN 
            employee_master e 
            ON c.rm_code = e.rm_code
        WHERE rownum = 1
            and a.rem_flage IS NULL
            AND a.prem_amt > 0
            AND UPPER(a.policy_no) = UPPER(p_policy_no)
            AND (p_company IS NULL OR a.company_cd = p_company)
            AND a.branch_cd IN (
                SELECT b.branch_code 
                FROM branch_master b
                WHERE 
                    b.branch_tar_cat <> 186 
                    AND b.category_id NOT IN (1004, 1005, 1006)
            )
        ORDER BY 
            a.due_date;
END PSM_IRP_SEARCH_BDD_By_COMPOL;