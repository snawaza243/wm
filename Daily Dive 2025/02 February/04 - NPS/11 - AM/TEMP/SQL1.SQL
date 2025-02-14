SELECT COMMON_ID, rejection_status, punching_flag, verification_flag, tran_type, exist_code
FROM TB_DOC_UPLOAD 
WHERE COMMON_ID IS NOT NULL
  AND rejection_status = 0
  AND punching_flag = 0
  AND verification_flag >0
  AND UPPER(TRIM(tran_type)) = 'ANA'
  --and exist_code is null
  --in (select exist_code from agent_master)
  ;