psm_am_branch_master_3


ppsm_ao_update_client_data;
psm_ao_update_client_data;

SELECT LOCATION_ID, LOCATION_NAME, city_id, pincode FROM LOCATION_MASTER;
    
select itemname from fixeditem where itemtype='10';     -- payment mode
select itemname from fixeditem where itemtype='7';      -- ACCOUNT TYPE
select itemname from fixeditem where itemtype='6';      -- EXAM TYPE
select itemname from fixeditem where itemtype='5';      -- TYPE 
select pincode, location_name from location_master where location_name ='patna';
select state_id, state_name, country_id from state_master ;
    
--city_id in ('C1586', 'C0087', 'C0182') , NEW DELHI	C0087, NEW DELHI	C1586
select agent_name from agent_master where city_id = 'C1586';
select city_name, CITY_ID, DEL_FLAG from city_master where city_name like upper('%DELHI%'); --city_id in ('C1586', 'C0087', 'C0182')


-- loginid = 130835, 
SELECT br.*
--, (select login_id from USERDETAILS_JI where BRANCH_ID = BR.BRANCH_CODE and rownum = 1) as loginid
FROM BRANCH_MASTER BR
where branch_tar_cat in (186, 615, 308)
and branch_code is not null 
AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = '130835'); AND ROLE_ID is not null);


SELECT BR.*
FROM BRANCH_MASTER BR
INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID
WHERE BR.branch_tar_cat IN (186, 615, 308)
AND UJ.LOGIN_ID = '120459'
and uj.ROLE_ID = '212'
AND BR.BRANCH_CODE = '10071189' 
;


-- 130835 RARHUL
-- 120459

10071189




select * from USERDETAILS_JI where login_id = '38387';
select * from branch_master where branch_code = '10010193';

SELECT * FROM BRANCH_MASTER BR where branch_tar_cat in (186, 615, 308)
--(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615) -- branch_tar_cat - 186, 615, 308 corporate partner , -- only ang : 189, 231, 186
and branch_code is not null AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = '38387' AND ROLE_ID is not null);

    
select sourceid  from agent_master;




    
    


-- AM 19
PSM_AM_AGENT_LIST5H;
PSM_AM_GET_AGENT_BY_ID;
PSM_GET_BRANCHES


-- ao 19
psm_ao_;


-- valid dt for ao
select common_id,tran_type, inv_code from tb_doc_upload where 
tran_type = 'AC' and 
verification_flag = '1' and punching_flag = '0' and rejection_status= '0' and inv_code is null ;


-- valid dt for am
select COMMON_ID, EXIST_CODE from tb_doc_upload where 
tran_type = 'ANA' and verification_flag = '1' and punching_flag = '0' and rejection_status= '0'

;
update agent_master set doc_id = '60009215' where exist_code = '538238';
SELECT aadhar_card_no 
FROM WEALTHMAKER.INVESTOR_MASTER
WHERE aadhar_card_no IS NOT NULL 
AND LENGTH(aadhar_card_no) = 12;

desc agent_master;



-- am 20
PSM_EMP_M__GET_CITY;
PSM_GET_BRANCHES;
PSM_AM_GET_CH_BRANCHES;
PSM_AM_AGENT_BY_ID;
PSM_AM_GET_AGENT_BY_ID;
PSM_ANA_Get_AGENTLIST;
PSM_AM_INSERT_AGENT_MASTER;
pam_am_update_agent_master;
PSM_AM_UPDATE_AGENT_MASTER;
PSM_AM_GET_BY_DT_ID;
PSM_AM_RM_BY_BRANCHDT




select * from agent_master WHERE EXIST_CODE = '538237';

delete from agent_master where agent_code = '30058592';
commit;
  SELECT * FROM BRANCH_MASTER 
    WHERE BRANCH_CODE IN (SELECT BRANCH_ID  FROM WEALTHMAKER.USERDETAILS_JI WHERE LOGIN_ID = '38387' AND ROLE_ID = '261' )
    and branch_code = '10071189';

  SELECT * FROM BRANCH_MASTER 
    WHERE BRANCH_CODE IN (SELECT BRANCH_ID  FROM WEALTHMAKER.USERDETAILS_JI WHERE LOGIN_ID = '38387' AND ROLE_ID = '212' )
    --AND branch_tar_cat in (186, 615, 308) 
    and branch_tar_cat = '615'
    --branch_code = '10071189';
    
    
    SELECT BRANCH_ID, LOGIN_ID, ROLE_ID  FROM WEALTHMAKER.USERDETAILS_JI WHERE branch_tar_cat = '615';-- = '10071189';
    
    
     SELECT branch_tar_cat FROM BRANCH_MASTER 
    WHERE  branch_code = '10071189'; 
    
    
SELECT branch_tar_cat, branch_name, branch_code , (SELECT LOGIN_ID FROM WEALTHMAKER.USERDETAILS_JI WHERE branch_id = branch_code and rownum = 1)
FROM branch_master 
WHERE  branch_code IN (
    SELECT branch_id 
    FROM WEALTHMAKER.USERDETAILS_JI 
    --WHERE LOGIN_ID = '120459' AND ROLE_ID = '212'
)
and branch_tar_cat = '615'
;

SELECT DISTINCT ROLE_ID FROM WEALTHMAKER.USERDETAILS_JI WHERE LOGIN_ID = '120459' ;

AND ROLE_ID = '212'

SELECT br.branch_code, br.branch_name, uj.ROLE_ID, uj.LOGIN_ID, br.branch_tar_cat
FROM BRANCH_MASTER BR
INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID
WHERE BR.branch_tar_cat IN (186, 615, 308)
--AND UJ.LOGIN_ID = '120459' and uj.ROLE_ID = '212'
--AND BR.BRANCH_CODE = '10071189' 
and br.branch_tar_cat = '615'
;
60014162

select sysdate from dual;
    
    
select * from agent_master where agent_code = '30058592';

select common_id from tb_doc_upload WHERE EXIST_CODE = '538237';

DESC AGENT_MASTER;


select * from agent_master where agent_code = '30508716';


-- dt = 60018625, rm = 126394
select * from branch_master where branch_code = '10010656';

select source from employee_master where payroll_id = '126394';


 SELECT a.rm_name, a.rm_code, a.payroll_id,a.source
    FROM employee_master a
    WHERE a.source = '10010656' and 
    a.payroll_id  = '126394' and type='A'
    and category_id in (2001, 2018)


-- valid dt for am
select COMMON_ID, EXIST_CODE from tb_doc_upload where 
tran_type = 'ANA' and verification_flag = '1' and punching_flag = '0' and rejection_status= '0'

;