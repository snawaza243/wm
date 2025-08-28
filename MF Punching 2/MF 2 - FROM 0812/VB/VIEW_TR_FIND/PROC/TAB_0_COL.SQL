SELECT 
    mf.investor_name,
    mf.bank_name,
    mf.SCH_CODE,
    mf.rm_name,
    mf.sip_type,
    mf.branch_name,
    mf.PANNO,
    amc.mut_name AS Amc_Name,
    sch.sch_name AS Scheme_Name,
    mf.TR_DATE,
    mf.TRAN_TYPE,
    mf.App_No,
    mf.PAYMENT_MODE,
    mf.CHEQUE_NO,
    mf.CHEQUE_DATE,
    mf.Amount,
    mf.lEAD_nO,
    mf.LEAD_NAME,
    mf.TRAN_code,
    b.branch_code,
    mf.BUSINESS_RMCODE,
    mf.frequency,
    mf.installments_no,
    mf.micro_investment,
    mf.target_switch_scheme
FROM 
    transaction_mf_Temp1 mf
    JOIN employee_master e ON TO_CHAR(mf.BUSINESS_RMCODE) = TO_CHAR(e.payroll_id)
    JOIN branch_master b ON mf.BUSI_BRANCH_CODE = b.branch_code
    JOIN mut_fund amc ON mf.MUT_CODE = amc.mut_code
    JOIN scheme_info sch ON mf.SCH_CODE = sch.sch_code
WHERE 
    mf.BUSINESS_RMCODE = 'your_value_here'
    AND TRUNC(mf.update_date) = TRUNC(SYSDATE)
ORDER BY 
    mf.TR_DATE, mf.rm_name;