select 

nvl(common_id, null)            as dt, 
nvl(VERIFICATION_FLAG, null)    as vf, 
nvl(PUNCHING_FLAG, null)        as pf,
nvl(REJECTION_STATUS, null)     as rf,
nvl(TRAN_TYPE, null)            as tt
 from tb_doc_upload;

 SELECT busi_branch_code,busi_rm_code,  COMMON_ID FROM TB_DOC_UPLOAD
WHERE punching_flag = '0'
AND verification_flag = '1'
AND rejection_status = '0'
AND TRAN_TYPE= 'FI'

;