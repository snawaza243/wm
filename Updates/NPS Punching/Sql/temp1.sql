
select iss_name,iss_code from iss_master where iss_code='IS02520';

select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status;



DECLARE v_cursor SYS_REFCURSOR;
BEGIN
    v_cursor := PSM_GET_TABLE_COL('nps_nonecs_tbl_imp');
    -- You need to fetch data from v_cursor in PL/SQL
END;
/

delete PROCEDURE PSM_GET_TABLE_COL;



-- dt for ar
select inv_code, AR_CODE,  nvl(common_id, null) as dt,      nvl(VERIFICATION_FLAG, null) as vf,  nvl(PUNCHING_FLAG, null) as pf,  nvl(REJECTION_STATUS, null) as rf,  nvl(TRAN_TYPE, null) as tt 
from tb_doc_upload where common_id is not null --='40153220';
and tran_type= 'FI'
and VERIFICATION_FLAG = '1'
and AR_CODE is null or AR_CODE = '0'
;




update tb_doc_upload set inv_code = '41862817001' where common_id = '40153220'; commit ;
select * from investor_master where inv_code = '41862817001';

-- valid dt with vlaid branch
select nvl(common_id, null) as dt, nvl(ar_code,'0') as ar,nvl(INV_CODE,0) as inv , nvl(BUSI_BRANCH_CODE,0) as bss_branch ,nvl(BUSI_RM_CODE,0) as bss_rm 
from  tb_doc_upload where tran_type='FI' and BUSI_RM_CODE in ( Select e.payroll_id from employee_master e,branch_master b where e.payroll_id is not null and e.source=b.branch_code and (e.type='A' or e.type is null))
and common_id is not null ;



-- search schema
select sch_code, payment_mode from transaction_st where tran_code = '050146033269';

-- bss rm branch
Select payroll_id, category_id, rm_code from employee_master where payroll_id = '38387';

-- search in inv
select * from investor_master where inv_code = '40339790001';


-- select ecs noncse import tables
select * from nps_ecs_tbl_imp_bk;
select * from  nps_ecs_tbl_imp;

select iss_name,iss_code from iss_master where iss_code='IS02520';

SELECT PRAN_NO FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO is not null;





SELECT client_code , source_code
FROM transaction_sttemp 
WHERE LENGTH(client_code) = 8;

SELECT PRODUCTCODE FROM PRODUCT_MASTER;



desc transaction_st;

 

select * from transaction_sttemp where tran_code = '070840001133';

select * from transaction_st where tran_code = '070840001133'; commit;

select * from NPS_TRANSACTION where  tran_code = '070840001133'; commit;


--Your Duplicate Transaction No Is 070840001133 and Your Recpt No Is 12603691428889706



