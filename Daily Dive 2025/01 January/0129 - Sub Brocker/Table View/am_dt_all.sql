CREATE OR REPLACE VIEW psm_tv_dt_am_1 AS
SELECT 
    'Valid DT for Associate Master' AS message,
    busi_rm_code AS tb_rm_code, 
    busi_branch_code AS tb_branch_code, 
    common_id AS tb_common_id, 
    rejection_status AS tb_reject, 
    punching_flag AS tb_punch, 
    verification_flag AS tb_verify, 
    tran_type AS tb_tr_type, 
    exist_code AS tb_exist_code, 
    NVL((SELECT exist_code FROM agent_master AM WHERE AM.exist_code = TDU.exist_code), 0) AS am_exist_code
    --tran_type = 'ANA'
FROM tb_doc_upload TDU
WHERE  tran_type is not null
AND common_id IS NOT NULL;