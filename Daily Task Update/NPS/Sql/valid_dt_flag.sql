select 

nvl(common_id, null)            as dt, 
nvl(VERIFICATION_FLAG, null)    as vf, 
nvl(PUNCHING_FLAG, null)        as pf,
nvl(REJECTION_STATUS, null)     as rf,
nvl(TRAN_TYPE, null)            as tt
 from tb_doc_upload;