
SELECT busi_rm_code, busi_branch_code, common_id, EXIST_CODE, rejection_status FROM tb_doc_upload
WHERE tran_type = 'ANA' AND common_id /*= p_common_id */IS NOT NULL and (rejection_status is null or rejection_status = 0);

SELECT * FROM AGENT_MASTER WHERE PAN IS NOT NULL;