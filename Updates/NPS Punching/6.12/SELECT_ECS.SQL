SELECT 
    SUBSTR(a.unique_id, 3, 7) AS exitsr,
    manual_arno AS pran,
    investor_name,
    DECODE(payment_mode, 'C', 'CHEQUE', 'E', 'ECS', 'H', 'CASH', 'D', 'DRAFT') AS payment_mode,
    cheque_no,
    TO_CHAR(tr_date, 'dd-Mon-yyyy') AS ecs_date,
    bank_name,
    a.unique_id,
    c.amount1,
    c.amount2,
    reg_charge,
    tran_charge,
    ROUND(((NVL(c.reg_charge, 0) + DECODE(NVL(c.amount2, 0), 0, 0, NVL(c.reg_charge, 0))) * 0.18), 2) AS service,
    (NVL(c.amount1, 0) + NVL(c.amount2, 0)) - (reg_charge + tran_charge + ROUND(((NVL(c.reg_charge, 0) + DECODE(NVL(c.amount2, 0), 0, 0, NVL(c.reg_charge, 0))) * 0.18), 2)) AS amt_inv,
    SUBSTR(a.unique_id, 10, 17) AS receipt_no_10_17,
    CASE WHEN SUBSTR(unique_id, 1, 2) = '11' THEN 58002 ELSE NULL END AS fc_registration_no,
    CASE 
        WHEN SUBSTR(unique_id,1,2) = '11' THEN 'REGISTRATION' 
        WHEN SUBSTR(unique_id,1,2) = '12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION CONTRIBUTION'
        WHEN SUBSTR(unique_id,1,2) = '12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION'
        ELSE 'OTHER CONTRIBUTION' 
    END AS remarks,
    CASE 
        WHEN SUBSTR(unique_id,1,2) = '11' THEN 'REGISTRATION' 
        WHEN SUBSTR(unique_id,1,2) = '12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION' 
        WHEN SUBSTR(unique_id,1,2) = '12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION'
        ELSE 'OTHER CONTRIBUTION' 
    END AS remarks1,
    A.tran_code,
    REF_TRAN_CODE,
    CSF_TRANSACTION_ID
FROM transaction_st a
JOIN investor_master b ON a.client_code = b.inv_code
JOIN NPS_TRANSACTION c ON a.tran_code = c.tran_code
WHERE TO_CHAR(a.tr_date, 'dd/mm/yyyy') = '01/06/2024'
    AND payment_mode = 'E'
    AND folio_no = '6036914'; -- based on CboKRAImport = '1'
