CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_GET_COLUMN_NAMES (
    P_FRM       IN VARCHAR2,
    P_DATA_TYPE IN VARCHAR2,
    P_COLUMNS   OUT SYS_REFCURSOR
) AS
BEGIN

    IF LOWER(P_FRM) = 'due_and_paid' THEN
        IF LOWER(p_data_type) = 'due' THEN
            OPEN p_columns FOR
                SELECT 'policy_no' AS column_name FROM DUAL UNION ALL
                SELECT 'company_cd' FROM DUAL UNION ALL
                SELECT 'status_cd' FROM DUAL UNION ALL
                SELECT 'location' FROM DUAL UNION ALL
                SELECT 'cl_name' FROM DUAL UNION ALL
                SELECT 'prem_amt' FROM DUAL UNION ALL
                SELECT 'pay_mode' FROM DUAL UNION ALL
                SELECT 'sa' FROM DUAL UNION ALL
                SELECT 'due_date' FROM DUAL UNION ALL
                SELECT 'cl_add1' FROM DUAL UNION ALL
                SELECT 'cl_add2' FROM DUAL UNION ALL
                SELECT 'cl_add3' FROM DUAL UNION ALL
                SELECT 'cl_add4' FROM DUAL UNION ALL
                SELECT 'cl_add5' FROM DUAL UNION ALL
                SELECT 'cl_city' FROM DUAL UNION ALL
                SELECT 'cl_pin' FROM DUAL UNION ALL
                SELECT 'cl_phone1' FROM DUAL UNION ALL
                SELECT 'cl_phone2' FROM DUAL UNION ALL
                SELECT 'cl_mobile' FROM DUAL UNION ALL
                SELECT 'plan_name' FROM DUAL UNION ALL
                SELECT 'doc' FROM DUAL UNION ALL
                SELECT 'prem_freq' FROM DUAL UNION ALL
                SELECT 'ply_term' FROM DUAL UNION ALL
                SELECT 'ppt' FROM DUAL UNION ALL
                SELECT 'net_amount' FROM DUAL;

        ELSIF LOWER(p_data_type) in ('paid', 'reins') THEN
            OPEN p_columns FOR
                SELECT 'policy_no' AS column_name FROM DUAL UNION ALL
                SELECT 'company_cd' FROM DUAL UNION ALL
                SELECT 'prem_amt' FROM DUAL UNION ALL
                SELECT 'paid_date' FROM DUAL UNION ALL
                SELECT 'status_cd' FROM DUAL UNION ALL
                SELECT 'net_amount' FROM DUAL UNION ALL
                SELECT 'due_date' FROM DUAL UNION ALL
                SELECT 'next_due_date' FROM DUAL UNION ALL
                SELECT 'prem_freq' FROM DUAL UNION ALL
                SELECT 'pay_mode' FROM DUAL UNION ALL
                SELECT 'doc' FROM DUAL UNION ALL
                SELECT 'plan_name' FROM DUAL;

        ELSIF LOWER(p_data_type) = 'lapsed' THEN
            OPEN p_columns FOR
                SELECT 'policy_no' AS column_name FROM DUAL UNION ALL
                SELECT 'company_cd' FROM DUAL UNION ALL
                SELECT 'status_cd' FROM DUAL UNION ALL
                SELECT 'location' FROM DUAL UNION ALL
                SELECT 'cl_name' FROM DUAL UNION ALL
                SELECT 'prem_amt' FROM DUAL UNION ALL
                SELECT 'pay_mode' FROM DUAL UNION ALL
                SELECT 'sa' FROM DUAL UNION ALL
                SELECT 'cl_add1' FROM DUAL UNION ALL
                SELECT 'cl_add2' FROM DUAL UNION ALL
                SELECT 'cl_add3' FROM DUAL UNION ALL
                SELECT 'cl_add4' FROM DUAL UNION ALL
                SELECT 'cl_add5' FROM DUAL UNION ALL
                SELECT 'cl_city' FROM DUAL UNION ALL
                SELECT 'cl_pin' FROM DUAL UNION ALL
                SELECT 'cl_phone1' FROM DUAL UNION ALL
                SELECT 'cl_phone2' FROM DUAL UNION ALL
                SELECT 'cl_mobile' FROM DUAL UNION ALL
                SELECT 'due_date' FROM DUAL UNION ALL
                SELECT 'plan_name' FROM DUAL UNION ALL
                SELECT 'doc' FROM DUAL UNION ALL
                SELECT 'prem_freq' FROM DUAL UNION ALL
                SELECT 'ply_term' FROM DUAL UNION ALL
                SELECT 'ppt' FROM DUAL UNION ALL
                SELECT 'net_amount' FROM DUAL UNION ALL
                SELECT 'fup_date' FROM DUAL;

        ELSE
            OPEN p_columns FOR
                SELECT 'Invalid data type' AS column_name FROM DUAL;
        END IF;
    END IF;
END;
/
