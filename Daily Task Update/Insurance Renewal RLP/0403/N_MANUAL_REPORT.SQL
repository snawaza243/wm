create or replace PROCEDURE             PSM_IR_MANUAL(
    p_company_code   IN VARCHAR2,
    p_month          IN NUMBER,
    p_year           IN NUMBER,
    p_policy_number  IN VARCHAR2,
    p_rem_flag       IN VARCHAR2,
    p_log_in         IN VARCHAR2,
    p_cursor         OUT SYS_REFCURSOR
)
AS
    l_sql_query CLOB;
BEGIN
    -- Construct the base query
    l_sql_query := 'SELECT
                        ar_branch_cd,
                        company_cd,
                        DECODE (a.p_name, NULL, a.i_name, p_name) AS client_name,
                        DECODE (p_add1, NULL, iadd1, p_add1) AS address1,
                        DECODE (p_add2, NULL, iadd2, p_add2) AS address2,
                        NULL AS cl_add3,
                        NULL AS cl_add4,
                        NULL AS cl_add5,
                        DECODE (c.phone1, NULL, c.phone2, c.phone1) AS cl_phone1,
                        DECODE (c.mobile, NULL, c.mobile1, c.mobile) AS cl_phone2,
                        p_city AS city_name,
                        due_date,
                        rem_flage,
                        (SELECT state_name FROM state_master WHERE state_id = a.state_cd) || '' '' || DECODE (DECODE (p_pin, NULL, pincode, p_pin), NULL, ipin, DECODE (p_pin, NULL, pincode, p_pin))               AS state_name,
                        (SELECT company_name FROM bajaj_company_master WHERE company_cd = a.company_cd) AS company_name,
                        (SELECT favour_name FROM favour_master WHERE company_cd = a.company_cd) || '' '' ||    policy_no     AS favour_name,
                        (SELECT branch_name FROM branch_master WHERE branch_code = c.sourceid)          AS branch_name,
                        (SELECT address1 FROM branch_master WHERE branch_code = c.sourceid)             AS branch_add1,
                        (SELECT address2 FROM branch_master WHERE branch_code = c.sourceid)             AS branch_add2,
                        (SELECT PLAN FROM bajaj_plan_master WHERE plan_no = a.plan_no)                  AS plan_name1,
                        pay_mode,
                        policy_no,
                        p_name,
                        i_name,
                        CASE bprem_freq
                           WHEN 1 THEN ''ANNUALLY''
                           WHEN 12 THEN ''MONTHLY''
                           WHEN 4 THEN ''QUARTERLY''
                           WHEN 2 THEN ''SEMI-ANNUALLY''
                        END AS prem_freq,
                        prem_freq as bprem_freq,
                        plan_name,
                        CASE WHEN sa IS NULL THEN (SELECT MAX(sa) FROM bajaj_ar_head f WHERE f.company_cd = a.company_cd AND f.policy_no = a.policy_no) ELSE sa END AS sa,
                        prem_amt,
                        mon_no,
                        year_no,
                        DECODE (DECODE (p_pin, NULL, pincode, p_pin),
                                NULL, ipin,
                                DECODE (p_pin, NULL, pincode, p_pin)) AS cl_pin,
                        (SELECT MAX (pin)
                           FROM bajaj_ar_head
                          WHERE     UPPER (TRIM (company_cd)) = UPPER (TRIM (a.company_cd))
                                AND UPPER (TRIM (policy_no)) = UPPER (TRIM (a.policy_no))) AS pin1,
                        SUBSTR (inv_cd, 1, 8) AS inv_code,
                        inv_cd AS inv_code1,
                        importdatatype
                    FROM bajaj_due_data a
                    JOIN client_master c ON SUBSTR (a.inv_cd, 1, 8) = c.client_code
                    JOIN employee_master e ON c.rm_code = e.rm_code
                    WHERE UPPER (TRIM (pay_mode)) = ''NON ECS''
                      AND (cl_add1 IS NOT NULL OR cl_add2 IS NOT NULL OR cl_add3 IS NOT NULL)

                      AND e.TYPE = ''A''
                      AND (a.p_add1 IS NOT NULL OR cl_add1 IS NOT NULL)
                      AND p_city IS NOT NULL
                      AND prem_amt > 0
                      AND sourceid IN (
                          SELECT branch_code
                          FROM branch_master
                          WHERE branch_tar_cat IN (185, 184, 187, 283)
                            AND category_id NOT IN (1004, 1005, 1006)
                      )

                      AND importdatatype = ''DUEDATA''
                      AND status_cd = ''DUE''';

    l_sql_query := l_sql_query || ' AND rem_flage IS NULL';
    l_sql_query := l_sql_query || ' AND policy_no = ''' || p_policy_number || '''';
    IF p_company_code IS NOT NULL THEN
        l_sql_query := l_sql_query || ' AND company_cd = ''' || p_company_code || '''';
    END IF;
    l_sql_query := l_sql_query || ' AND ROWNUM = 1  ORDER BY cl_pin, client_code';



    OPEN p_cursor FOR l_sql_query;
END PSM_IR_MANUAL;
