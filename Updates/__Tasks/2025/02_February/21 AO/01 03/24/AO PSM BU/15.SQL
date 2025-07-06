SELECT EMAIL, PAN, MOBILE, G_PAN, aadhar_card_no FROM INVESTOR_MASTER 
WHERE EMAIL IS NOT NULL 
AND PAN IS NOT NULL
AND MOBILE IS NOT NULL
AND G_PAN IS NOT NULL;

 

select guest_cd from client_master where mobile = '9785975122';

select * from investor_master where aadhar_card_no = '428910184789';
select * from client_test where client_codekyc = '41078645002';