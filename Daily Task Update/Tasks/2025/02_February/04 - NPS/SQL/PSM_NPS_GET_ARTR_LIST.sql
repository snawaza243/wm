create or replace PROCEDURE PSM_NPS_GET_ARTR_LIST (
    p_ar_no            IN VARCHAR2 DEFAULT NULL,
    p_app_no           IN VARCHAR2 DEFAULT NULL,
    p_cheque_no        IN VARCHAR2 DEFAULT NULL,
    p_pran_no          IN VARCHAR2 DEFAULT NULL,
    p_scheme           IN VARCHAR2 DEFAULT NULL,
    p_investor_name    IN VARCHAR2 DEFAULT NULL,
    p_ana_exist_code   IN VARCHAR2 DEFAULT NULL,
    p_ar_from_date     IN VARCHAR2 DEFAULT NULL,
    p_ar_to_date       IN VARCHAR2 DEFAULT NULL,
    p_before_st        IN VARCHAR2 DEFAULT NULL,
    p_result_cursor    OUT SYS_REFCURSOR
) AS

BEGIN
    -- Check if the p_before_st parameter is '1'
    IF p_before_st = '1' THEN
        OPEN p_result_cursor FOR
            SELECT 
                CASE 
                    WHEN SCH_CODE = 'OP#09971' THEN 'New Pension Scheme Tier 1'
                    WHEN SCH_CODE = 'OP#09972' THEN 'New Pension Scheme Tier 2'
                    WHEN SCH_CODE = 'OP#09973' THEN 'New Pension Scheme Tier 1+2'
                    ELSE 'Unknown Scheme'
                    END AS SCH_CODE,
                TO_CHAR(TR_DATE, 'DD-MM-YY')  AS TR_DATE1, 
                
                transaction_sttemp.*
            FROM 
                transaction_sttemp
            WHERE 
                (p_ar_no IS NULL OR tran_code = p_ar_no) 
                AND (p_app_no IS NULL OR app_no = p_app_no) 
                AND (p_cheque_no IS NULL OR cheque_no LIKE '%' || p_cheque_no || '%')
                AND (p_pran_no IS NULL OR manual_arno = p_pran_no)
                AND (p_scheme IS NULL OR sch_code LIKE '%' || UPPER(p_scheme) || '%')
                AND (p_investor_name IS NULL OR UPPER(p_investor_name) LIKE '%' || UPPER((select investor_name from investor_master where inv_code = transaction_sttemp.CLIENT_CODE)) || '%')
                AND (p_ana_exist_code IS NULL OR CLIENT_CODE = p_ana_exist_code)
                AND (p_ar_from_date IS NULL OR tr_date >= TO_DATE(p_ar_from_date, 'DD-MM-YYYY'))
                AND (p_ar_to_date IS NULL OR tr_date <= TO_DATE(p_ar_to_date, 'DD-MM-YYYY'))
                AND sch_code IN ('OP#09971', 'OP#09972', 'OP#09973')
           --     AND mut_code IN (SELECT iss_code FROM iss_master WHERE prod_code = 'DT028')
                and rownum <=1500
            ORDER BY tr_date;

    ELSE
        OPEN p_result_cursor FOR
            SELECT 
                CASE 
                    WHEN SCH_CODE = 'OP#09971' THEN 'New Pension Scheme Tier 1'
                    WHEN SCH_CODE = 'OP#09972' THEN 'New Pension Scheme Tier 2'
                    WHEN SCH_CODE = 'OP#09973' THEN 'New Pension Scheme Tier 1+2'
                    ELSE 'Unknown Scheme'
                END AS SCH_CODE,
                
                TO_CHAR(TR_DATE, 'DD-MM-YY')  AS TR_DATE1, 
                transaction_st.*
            FROM 
                transaction_st 
            WHERE 
                (p_ar_no IS NULL OR tran_code = p_ar_no) 
                AND (p_app_no IS NULL OR app_no = p_app_no) 
                AND (p_cheque_no IS NULL OR cheque_no LIKE '%' || p_cheque_no || '%')
                AND (p_pran_no IS NULL OR manual_arno = p_pran_no)
                AND (p_scheme IS NULL OR sch_code LIKE '%' || UPPER(p_scheme) || '%')
                AND (p_investor_name IS NULL OR UPPER(p_investor_name) LIKE '%' || UPPER(INV_NAME) || '%')
                AND (p_ana_exist_code IS NULL OR CLIENT_CODE = p_ana_exist_code)
                AND (p_ar_from_date IS NULL OR tr_date >= TO_DATE(p_ar_from_date, 'DD-MM-YYYY'))
                AND (p_ar_to_date IS NULL OR tr_date <= TO_DATE(p_ar_to_date, 'DD-MM-YYYY'))
                AND sch_code IN ('OP#09971', 'OP#09972', 'OP#09973')
             --   AND mut_code IN (SELECT iss_code FROM iss_master WHERE prod_code = 'DT028')
                and rownum <=1500

            ORDER BY tr_date;
    END IF;
END PSM_NPS_GET_ARTR_LIST;