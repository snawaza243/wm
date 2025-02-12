
 select  mut_code, sch_code,  bank_name, bank_ac_no, amount, payment_mode, , cheque_no, cheque_date, tran_code, source_code, tr_date, act_code
, 
 from filladvisory where 1=1 
 and act_code='P_CLIENT_CODE' 
 and client_pan='P_CLINET_PAN' OR NULL "