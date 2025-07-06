select common_id, inv_code, tdu.VERIFICATION_FLAG, tdu.REJECTION_STATUS, tdu.PUNCHING_FLAG, tdu.tran_type 
from tb_doc_upload tdu
where tdu.VERIFICATION_FLAG = '1'
and tdu.REJECTION_STATUS = '0'
and tdu.PUNCHING_FLAG = '0' 
and tdu.tran_type != 'MF'