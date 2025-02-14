select common_id, punching_flag, verification_flag, rejection_status, inv_code, tran_type
from tb_doc_upload
where common_id is not null
and tran_type = 'AC'
and punching_flag = '0'
and rejection_status = '0'
and verification_flag = '1'
and inv_code is null
;
