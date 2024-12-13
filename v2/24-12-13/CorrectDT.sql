-- chack your dt number is valid or not (your tested dt is not valid '20222011')
SELECT nvl(
check_dt_newinv('20222011') , '')
FROM dual;


-- it will give valid dt number (not existing investor)
SELECT common_id, inv_code
FROM tb_doc_upload 
WHERE verification_flag = '1' 
  AND rejection_status = '0' 
  AND tran_type = 'AC' 
  AND punching_flag = '0' 
  AND inv_code IS NULL;
  
-- it will give valid dt number (existing investor)
SELECT common_id, inv_code
FROM tb_doc_upload 
WHERE verification_flag = '1' 
  AND rejection_status = '0' 
  AND tran_type = 'AC' 
  AND punching_flag = '0' 
  AND inv_code IS not null;
  
--  get fresh and valid dt number investor not exist
select * from psm_tv_dt;

psm_ao_get_city_list;