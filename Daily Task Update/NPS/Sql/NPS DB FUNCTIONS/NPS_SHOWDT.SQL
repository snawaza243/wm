SELECT 
    COALESCE(ar_code, '0') AS ar_code,
    COALESCE(ar_name, '0') AS ar_name,
    COALESCE(BUSI_BRANCH_CODE, 0) AS BUSI_BRANCH_CODE,
    COALESCE(BUSI_RM_CODE, 0) AS BUSI_RM_CODE
FROM 
    tb_doc_upload
WHERE 
    tran_type = 'FI'
    AND common_id IS NOT NULL
    AND INV_CODE IS NULL;