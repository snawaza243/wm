-- Get head client where dt exist

select client_code, client_codekyc from client_test 
where client_codekyc in (select inv_code from tb_doc_upload where substr(inv_code, 9,11) = '001' and common_id is not null)
and client_code = main_code;