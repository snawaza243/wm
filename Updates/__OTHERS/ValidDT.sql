select common_id from tb_doc_upload
where tb_doc_upload.PUNCHING_FLAG='0'
and tb_doc_upload.REJECTION_STATUS = '0'
and tb_doc_upload.VERIFICATION_FLAG= '1'
and tran_type = 'MF'