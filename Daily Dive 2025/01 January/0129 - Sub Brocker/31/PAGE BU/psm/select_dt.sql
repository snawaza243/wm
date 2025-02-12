SELECT 
busi_rm_code as tb_rm_code, 
busi_branch_code as tb_branch_code, 
common_id as tb_common_id, 
rejection_status as tb_reject, 
punching_flag as tb_punch, 
verification_flag as tb_verify , 
tran_type as tb_tr_type, 
exist_code as tb_exist_code, 
nvl((select exist_code from agent_master where exist_code = tb_doc_upload.exist_code), 0 ) as am_exist_code
    FROM tb_doc_upload
    WHERE tran_type  = 'ANA' 
    AND common_id is not null;