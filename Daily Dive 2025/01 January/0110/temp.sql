PSM_AO_MM_Update_By_Inv



select ct.client_code, ct.source_code, ct.client_codekyc from client_test ct 
where ct.source_code  = substr((select im.inv_code from investor_master im where im.inv_code = '41116814002'),1,8)
and ct.client_code = ct.main_code;


PSM_AO_GetStatesByCountry;


desc filladvisory;


select act_code from filladvisory where 1=1 and act_code in (select client_code from client_test where client_code = main_code);

--AH085425
--40983217001

--AH1400501

select title, client_code, main_code, client_name, dob, client_codekyc from client_test where client_code = 'AH1379460';


select investor_title, GENDER, investor_name, dob from investor_master where inv_code = '42049476001';

select client_title, client_name, dob from client_master where client_code = '42049476';




select * from investor_master where inv_code = '41116814002';
select client_codekyc from client_test where main_code = 'AH665277';
