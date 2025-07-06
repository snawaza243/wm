-- AM TESTING SQL

psm_ass_m_update_agent_master;
psm_am_insert_agent_master;
PSM_ANA_Get_AGENTLIST;




-- AM VALID DT FOR FRESH ENTRY OF AGENT fresh DT
SELECT COMMON_ID, rejection_status, punching_flag, verification_flag, tran_type, exist_code
FROM TB_DOC_UPLOAD WHERE COMMON_ID IS NOT NULL AND rejection_status > 0 AND punching_flag = 0 AND verification_flag >0 AND UPPER(TRIM(tran_type)) = 'ANA';

select * from branch_master where CITY_ID = 'C0914';

select * from city_master  where del_flag is null and city_name like'DEL%' order by city_name ;


select * from agent_master where agent_code = '30030925' ;
