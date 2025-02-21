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
AND UJ.LOGIN_ID = '130835'
and uj.ROLE_ID = '212';

-- 120459






select * from USERDETAILS_JI where login_id = '38387';
select * from branch_master where branch_code = '10010193';

SELECT * FROM BRANCH_MASTER BR where branch_tar_cat in (186, 615, 308)
--(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615) -- branch_tar_cat - 186, 615, 308 corporate partner , -- only ang : 189, 231, 186
and branch_code is not null AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = '38387' AND ROLE_ID is not null);

    
select sourceid  from agent_master;
    
    


-- AM 19
PSM_AM_AGENT_LIST5H;
PSM_AM_GET_AGENT_BY_ID;


-- ao 19
psm_ao_;


-- valid dt for ao
select common_id,tran_type, inv_code from tb_doc_upload where 
tran_type = 'AC' and 
verification_flag = '1' and punching_flag = '0' and rejection_status= '0' and inv_code is null ;



select * from tb_doc_upload where 
tran_type = 'ANA' and verification_flag = '1' and punching_flag = '0' and rejection_status= '0'
;
SELECT aadhar_card_no 
FROM WEALTHMAKER.INVESTOR_MASTER
WHERE aadhar_card_no IS NOT NULL 
AND LENGTH(aadhar_card_no) = 12;

desc agent_master;
    
    
    
    
    
    