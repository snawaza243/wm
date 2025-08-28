SELECT 
    Investor_Name,
    Branch_Name,
    PANNO,
    Amc_Name,
    Scheme_Name,
    TR_DATE,
    TRAN_TYPE,
    App_No,
    PAYMENT_MODE,
    CHEQUE_NO,
    CHEQUE_DATE,
    Amount,
    sip_TYPE,
    lEAD_nO,
    LEAD_nAME,
    TRAN_code,
    branch_code,
    BUSINESS_RMCODE,
    SCH_CODE,
    SOURCE_CODE,
    Bank_Name,
    Rm_Name,
    folio_no,
    doc_id,
    micro_investment,
    target_switch_scheme,
    target_scheme_name,
    switch_scheme_name
FROM 
    temporaryBusiness
WHERE 
    BUSI_BRANCH_CODE IN ('your_branch_list_here')
ORDER BY 
    TR_DATE, rm_name;