 SELECT MAIN_CODE , client_name, approved, approved_flag FROM CLIENT_TEST WHERE main_code = 'AH023602' ;
 
 update client_test set approved_flag  = 'NO', APPROVED = 'NO' where main_code = 'AH023602' ;
 
 
 SELECT COMMON_ID FROM TB_DOC_UPLOAD WHERE CHECK_DT_NEWINV(COMMON_ID) > 0 AND ROWNUM <100;
 
 
 SELECT COMMON_ID, GUEST_CD, inv_code, PUNCHING_FLAG, rejection_status, tran_type, verification_flag
FROM TB_DOC_UPLOAD  
WHERE COMMON_ID  IS NOT NULL
AND tran_type= 'AC' AND rejection_status = '0' AND verification_flag = '1' AND PUNCHING_FLAG = '0' 
AND GUEST_CD  is null 
AND INV_CODE IS NULL;

