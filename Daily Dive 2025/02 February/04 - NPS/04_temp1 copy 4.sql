-- SEARCH VALID DT FOR NPS
SELECT ar_code, INV_CODE, BUSI_BRANCH_CODE, BUSI_RM_CODE, common_id, rejection_status, verification_flag, punching_flag, 
(SELECT  TRAN_CODE FROM TRANSACTION_ST WHERE TRANSACTION_ST.TRAN_CODE = tb_doc_upload.AR_CODE) AS TST_TR,
sch_code
--INTO tb_f_ar, tb_f_inv, tb_f_bss_br, tb_f_bss_rm, tb_f_common, tb_f_rej, tb_f_ver, tb_f_pun, tb_f_sch
FROM tb_doc_upload 


WHERE tran_type = 'FI' AND sch_code IN ('OP#09971', 'OP#09972', 'OP#09973') AND common_id IS NOT NULL
AND AR_CODE IS NULL
AND rejection_status = '0'
AND verification_flag = '1'
AND punching_flag = '0'
;





















select tb_doc_upload.sch_code from tb_doc_upload;

select common_id, tst.tran_code from tb_doc_upload tdu
left join transaction_st tst on tst.tran_code = tdu.ar_code
where tdu.common_id is not null and tst.tran_code is not null
AND tst.sch_code IN ('OP#09971', 'OP#09972', 'OP#09973')
;








select nvl(ar_code,'0'),nvl(INV_CODE,'0'),nvl(BUSI_BRANCH_CODE,'0'),nvl(BUSI_RM_CODE,'0'), 
rejection_status, verification_flag, punching_flag, common_id from  tb_doc_upload where tran_type='FI' and common_id IS NOT NULL;

select inv_code,investor_name from investor_master where inv_code= '41901044001';

select CLIENT_CODE from transaction_stTEMP WHERE TRAN_CODE = '050048037701';

select AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK from nps_transaction where TRAN_CODE = '050140302124';
--40812153004, 050099012042