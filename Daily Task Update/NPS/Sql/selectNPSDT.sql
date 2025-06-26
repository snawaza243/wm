select 
nvl(ar_code, null)              as ar,
inv_code                        as inv,
nvl(common_id, null)            as dt,  
nvl(VERIFICATION_FLAG, null)    as vf,  
nvl(PUNCHING_FLAG, null)        as pf, 
nvl(REJECTION_STATUS, null)     as rf,  
nvl(TRAN_TYPE, null)            as tt,
nvl(BUSI_BRANCH_CODE,0)         as bss_branch,
nvl(BUSI_RM_CODE,0)             as bss_rm

from tb_doc_upload where common_id is not null and tran_type = 'FI' and PUNCHING_FLAG = '0' and VERIFICATION_FLAG = '1'

