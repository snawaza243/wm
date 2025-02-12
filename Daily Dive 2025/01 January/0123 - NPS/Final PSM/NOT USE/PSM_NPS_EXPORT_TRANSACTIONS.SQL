create or replace PROCEDURE EXPORT_TRANSACTIONS(
    p_from_date IN DATE,
    p_to_date IN DATE,
    p_payment_mode IN VARCHAR2,
    p_folio_no IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
        SELECT 
            SUBSTR(a.unique_id, 3, 7) AS exitsr,
            manual_arno AS pran,
            investor_name,
            DECODE(payment_mode, 'C', 'CHEQUE', 'E', 'ECS', 'H', 'CASH', 'D', 'DRAFT', 'M', 'Corporate NON ECS') AS payment_mode,
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
            SUBSTR(A.UNIQUE_ID, 10, 17) AS RECEIPT_NO_10_17,
            CASE WHEN SUBSTR(UNIQUE_ID, 1, 2) = '11' THEN 58002 ELSE NULL END AS FC_REGISTRATION_NO,
            CASE 
                WHEN SUBSTR(UNIQUE_ID, 1, 2) = '11' THEN 'REGISTRATION'
                WHEN SUBSTR(UNIQUE_ID, 1, 2) = '12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION CONTRIBUTION'
                WHEN SUBSTR(UNIQUE_ID, 1, 2) = '12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION'
                WHEN SUBSTR(UNIQUE_ID, 1, 2) NOT IN ('11', '12') THEN 'OTHER CONTRIBUTION'
            END AS REMARKS,
            CASE 
                WHEN SUBSTR(UNIQUE_ID, 1, 2) = '11' THEN 'REGISTRATION'
                WHEN SUBSTR(UNIQUE_ID, 1, 2) = '12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION'
                WHEN SUBSTR(UNIQUE_ID, 1, 2) = '12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION'
                WHEN SUBSTR(UNIQUE_ID, 1, 2) NOT IN ('11', '12') THEN 'OTHER CONTRIBUTION'
            END AS REMARKS1,
            A.TRAN_CODE,
            REF_TRAN_CODE,
            CSF_TRANSACTION_ID
        FROM 
            transaction_st a
            JOIN investor_master b ON a.client_code = b.INV_CODE
            JOIN NPS_TRANSACTION c ON a.tran_code = c.tran_code
        WHERE 
            a.tr_date BETWEEN p_from_date AND p_to_date
            AND (p_payment_mode = 'all' OR payment_mode = p_payment_mode)
            AND (p_folio_no IS NULL OR folio_no = p_folio_no);
END;