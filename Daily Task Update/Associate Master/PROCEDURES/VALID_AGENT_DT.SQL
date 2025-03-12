SELECT COMMON_ID FROM TB_DOC_UPLOAD
WHERE PUNCHING_FLAG = '0'
AND verification_flag = '1'
AND TRAN_TYPE = 'ANA'
AND rejection_status = '0'