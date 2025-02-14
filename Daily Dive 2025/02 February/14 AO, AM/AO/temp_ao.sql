psm_ao_insert_client_data;
psm_ao_update_client_data;
psm_ao_get_client_by_id;
psm_ao_mm_existing_inv_grid;
psm_ao_mm_insert;
psm_ao_mm_update_by_inv;
psm_ao_mm_existing_inv_grid;
psm_ao_mm_valid_insert;
PSM_AO_MM_GET_INV_TO_UP;


DELETE FROM CLIENT_TEST WHERE MAIN_CODE = 'AH2484801' AND client_codekyc NOT IN ('42631432002','42631432003');
select *  FROM INVESTOR_MASTER WHERE mobile = '9909757876';-- INV_CODE ='42631435002' ;


DELETE 
--select *
FROM INVESTOR_MASTER WHERE investor_master.inv_code = '42631435007';
commit;

--select * 
delete
FROM CLIENT_TEST WHERE client_test.client_codekyc = '42631435007';
commit;


DELETE from client_test where client_code = 'AH2484815';
delete from client_master where client_code = '42631432';
select * from tb_doc_upload where inv_code = '42631432001';




-- AO VALID DT
select common_id, punching_flag, verification_flag, rejection_status, inv_code, tran_type
from tb_doc_upload where common_id is not null and tran_type = 'AC' and punching_flag = '0' and rejection_status = '0'  and verification_flag = '1' and inv_code is null ;


-- AH2484812
--Data Inserted successfully --> 42631434 | AH2484813 | 42631434001
--Data Inserted successfully --> 42631435 | AH2484814 | 42631435001

SELECT * FROM INVESTOR_MASTER WHERE INV_CODE ='42631432004' ;
SELECT * FROM INVESTOR_MASTER WHERE investor_master.source_id = '42631432';
SELECT * FROM CLIENT_TEST WHERE client_test.source_code = '42631432';

SELECT kyc_status, client_code, clientcode_old, business_code, occ_id, status, act_cat, title, client_name, TITLE_FATHER_SPOUSE, FATHER_SPOUSE_NAME, others1, gender, marital_status, 
nationality, resident_nri, dob, annual_income, client_pan, lead_type, g_name, g_nationality, g_pan, add1, add2, state_id, city_id, pincode, overseas_add, fax, aadhar_card_no, 
email, std1, tel1, std2, tel2, mobile_no, loggeduserid, timest, client_codekyc, source_code, main_code, BRANCH_CODE
from client_test where client_code = 'AH2484813';


SELECT client_test.kyc_status from client_test;
delete from client_master where client_code = '42631432';
select * from tb_doc_upload where inv_code = '42631432001';
