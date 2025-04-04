PSM_AM_AGENT_BY_ID;
PSM_AM_BRANCH_MASTER_3;
psm_ass_m_update_agent_master;


select branch_tar_cat, branch_name from branch_master where BRANCH_TAR_CAT = 308;
--upper(branch_name) like '% PARTNER%';
--(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615);
-- branch_tar_cat - 186, 615, corporate partner     :  only ang : 189, 231, 186

SELECT busi_rm_code, busi_branch_code, common_id, EXIST_CODE FROM tb_doc_upload
WHERE tran_type = 'ANA' AND common_id /*= p_common_id */IS NOT NULL AND EXIST_CODE = '82106'
--!= 'N/A'
and (rejection_status is null or rejection_status = 0);


select exist_code, agent_code from agent_master where exist_code is null;

exist_code = '501550';



