select nvl(ar_code,'0'), nvl(INV_CODE,0), nvl(BUSI_BRANCH_CODE,0), nvl(BUSI_RM_CODE,0) , common_id
from  tb_doc_upload where tran_type='FI' and common_id IS NOT NULL
and ar_code in (select tran_code from transaction_st WHERE SCH_CODE IN ('OP#09971', 'OP#09972', 'OP#09973' ))
and ar_code in (select TRAN_CODE from nps_transaction)
AND common_id IS NULL
;




select nvl(ar_code,'0'), nvl(INV_CODE,0), nvl(BUSI_BRANCH_CODE,0), nvl(BUSI_RM_CODE,0) , common_id
from  tb_doc_upload where tran_type='FI' and common_id IS NOT NULL
and ar_code in (select tran_code from transaction_st WHERE SCH_CODE IN ('OP#09971', 'OP#09972', 'OP#09973' ))
and ar_code in (select TRAN_CODE from nps_transaction)
--AND common_id IS NULL
and ar_code = '050042011237'
;

select common_id from tb_doc_upload where ar_code is not null = '050042011237';

PSM_NPS_GET_ARTR_LIST



-- 40246786 AT = 050104226247