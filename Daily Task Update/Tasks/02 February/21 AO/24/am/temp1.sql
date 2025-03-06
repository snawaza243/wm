PSM_AM_AGENT_SEARCH;
PSM_AM_GET_AGENT_BY_ID;
PSM_ANA_Get_AGENTLIST;
PSM_AM_UPDATE_AGENT_MASTER;
PSM_AM_INSERT_AGENT_MASTER;
PSM_SALUTATION_LIST;


select MOBILE, EMAIL, PAN, title,gender, RM_CODE, BANK_BRANCH_NAME from agent_master WHERE AGENT_CODE = '30508719';

select common_id from tb_doc_upload where punching_flag = '0'
and verification_flag = '1' and rejection_status = '0' and tran_type = 'ANA';