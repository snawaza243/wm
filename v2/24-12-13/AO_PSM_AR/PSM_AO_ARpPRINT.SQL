CREATE OR REPLACE PROCEDURE PSM_AO_AR_PRINT(
    p_tr_date IN DATE,
    p_print_source_id IN VARCHAR2,
    p_busi_code IN VARCHAR2,
    p_source_id IN VARCHAR2,
    p_tran_code IN VARCHAR2
)
AS
BEGIN

        SELECT 
            b.client_code, 
            'P' AS ar_type, 
            t.tran_code, 
            t.tr_date, 
            t.cheque_date, 
            t.cheque_no, 
            t.bank_name, 
            t.amount, 
            b.client_code AS source_code, 
            t.app_no, 
            NVL(upfront_ope_paid_temptran(t.tran_code), 0) AS paid_brok, 
            NVL((
                SELECT SUM(NVL(amt, 0)) 
                FROM payment_detail 
                WHERE tran_code = t.tran_code
            ), 0) AS paidamt, 
            '' AS asr, 
            t.payment_mode, 
            (SELECT investor_name FROM investor_master WHERE inv_code = t.client_code) AS inv, 
            (
                SELECT MAX(client_name) 
                FROM client_master 
                WHERE client_code = t.source_code
            ) AS client, 
            t.exist_code AS existcode, 
            t.address1 AS add1, 
            t.address2 AS add2, 
            '' AS loc, 
            't.pincode' AS pin, 
            (
                SELECT MAX(city_name) 
                FROM city_master 
                WHERE city_id = (
                    SELECT MAX(city_id) 
                    FROM client_master 
                    WHERE client_code = t.source_code
                )
            ) AS ccity, 
            't.mobile' AS ph, 
            't.email' as email, 
            0 AS arn, 
            '' AS subbroker, 
            (
                SELECT rm_name 
                FROM employee_master 
                WHERE payroll_id = TO_CHAR(t.business_rmcode) 
                  AND (TYPE = 'A' OR TYPE IS NULL)
            ) AS rname, 
            (
                SELECT payroll_id 
                FROM employee_master 
                WHERE payroll_id = TO_CHAR(t.business_rmcode) 
                  AND (TYPE = 'A' OR TYPE IS NULL)
            ) AS rcode, 
            (
                SELECT branch_name 
                FROM branch_master 
                WHERE branch_code = t.busi_branch_code
            ) AS bname, 
            (
                SELECT address1 
                FROM branch_master 
                WHERE branch_code = t.busi_branch_code
            ) AS badd1, 
            (
                SELECT address2 
                FROM branch_master 
                WHERE branch_code = t.busi_branch_code
            ) AS badd2, 
            (
                SELECT phone 
                FROM branch_master 
                WHERE branch_code = t.busi_branch_code
            ) AS bphone, 
            (
                SELECT location_name 
                FROM location_master 
                WHERE location_id = (
                    SELECT location_id 
                    FROM branch_master 
                    WHERE branch_code = t.busi_branch_code
                )
            ) AS bloc, 
            (
                SELECT city_name 
                FROM city_master 
                WHERE city_id = (
                    SELECT city_id 
                    FROM branch_master 
                    WHERE branch_code = t.busi_branch_code
                )
            ) AS bcity, 
            (
                SELECT iss_name 
                FROM iss_master 
                WHERE iss_code = t.mut_code 
                  AND iss_code NOT IN (
                      SELECT DISTINCT iss_code 
                      FROM iss_master 
                      WHERE prod_code IN (
                          SELECT prod_code 
                          FROM product_master 
                          WHERE nature_code = 'NT004'
                      )
                  )
            ) AS compmf, 
            'Bajaj Capital Limited' AS compgroup, 
            (
                SELECT longname 
                FROM other_product 
                WHERE osch_code = t.sch_code
            ) AS schmf, 
            (
                SELECT short_name 
                FROM scheme_info 
                WHERE sch_code = t.sch_code
            ) AS sschmf, 
            '38387' AS userid
        FROM 
            transaction_ST t
        JOIN 
            client_master b 
        ON 
            t.source_code = b.client_code
        WHERE 
            t.tr_date = p_tr_date
            AND (
                P_print_source_id IS NOT NULL 
                AND t.source_code = b.client_code
            )
            AND (t.asa <> 'C' OR t.asa IS NULL)
            AND t.business_rmcode = P_busi_code
            AND t.source_code = P_source_id
            AND t.tran_code = P_tran_code;
END;
