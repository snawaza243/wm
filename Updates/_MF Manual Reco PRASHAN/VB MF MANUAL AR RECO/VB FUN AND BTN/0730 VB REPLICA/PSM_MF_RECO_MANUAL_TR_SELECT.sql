-- NEW MANUAL TR SEARCH


select inv.Investor_Name,inv.address1||','||inv.address2||','||inv.phone||','||inv.email address1 ,ct.city_name, 
 e.rm_code RMCODE, mf.FLAG, mf.BROKER_ID, mf.Sip_Amount, mf.remark,
mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,
tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,
Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.tran_type tran_type, mf.sip_type sip_type, 
CASE WHEN  mf.loggeduserid='MFONLINE' THEN 'Online' WHEN  mf.loggeduserid='Valuefy' THEN 'Online' ELSE 'Offline' end loggeduserid 
from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1 mf 
where  MF.MOVE_FLAG1 IS NULL AND MF.SIP_ID IS  NULL AND TRAN_TYPE NOT IN('REVERTAL') AND (MF.ASA<>'C' OR MF.ASA IS NULL) AND inv.city_id=ct.city_id(+) 
and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code 
and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code 
and b.branch_code in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='112650' AND ROLE_ID='29') 
 AND (MF.SIP_TYPE='REGULAR' OR MF.SIP_TYPE IS NULL OR MF.SIP_TYPE ='STP')   
 and (mf.rec_flag ='N' or mf.rec_flag is null) and tr_date>=TO_DATE('01-01-2025', 'DD-MM-YYYY') 
 and tr_date<=TO_DATE('10-01-2025', 'DD-MM-YYYY') ORDER BY TR_DATE