create or replace procedure wealthmaker.prc_calulate_folio_trail (
   pfromdate date,
   ptodate   date,
   puserid   varchar2,
   prole_id  number,
   p_cursor  out sys_refcursor
) as
begin
  open p_cursor for 
   select * from ( select feed_seq, bc_no, amc_code, folio, investor_name, product_code, units, amount, subbroker_code, trade_dt, trail_amt, trail_paid_from_dt, trail_paid_to_dt, null days_fee_paid_from_dt, null days_fee_paid_to_dt, trail_rate, trail_payable_amt, flag, ps_seq, trxn_id, trail_type, days_fee_paid, inv_code, sch_code, flag_cal, sub_cd, flag_rate, temp, sub_code_last, client_rmcode, brok_code, ter_location, tr_location_upd, import_company, tran_type, net_commission_amount, source_gst_state_code, company_gst_state_code, cgst_rate, cgst_amount, sgst_rate, sgst_amount, utgst_rate, utgst_amount, igst_rate, igst_amount, gst_calculation_flag, mut_code, mf_ho_gst_state_code
                  from mftrail.trail_feed@mf.bajajcapital union all
                    select feed_seq, bc_no,amc_code,folio,investor_name,product_code,units,amount,subbroker_code,trade_dt,trail_amt,trail_paid_from_dt,trail_paid_to_dt,days_fee_paid_from_dt,days_fee_paid_to_dt,trail_rate,trail_payable_amt,flag,ps_seq,trxn_id,trail_type,days_fee_paid,inv_code,sch_code,flag_cal,sub_cd,flag_rate,temp,sub_code_last,client_rmcode,brok_code,ter_location,tr_location_upd,import_company,tran_type,net_commission_amount,source_gst_state_code,company_gst_state_code,cgst_rate,cgst_amount,sgst_rate,sgst_amount,utgst_rate,utgst_amount,igst_rate,igst_amount,gst_calculation_flag,mut_code, mf_ho_gst_state_code
                    from ( select * from mftrail.trail_feed_new@mf.bajajcapital union all select * from wealthmaker.trail_feed_jan18_dec19@archivedb )
                            )
    where folio in ( select ft.folio_number from wealthmaker.import_folios_temp ft where ft.login_id = puserid and ft.role_id = prole_id )
    and trail_paid_from_dt >= to_date(pfromdate, 'DD-MM-YYYY')
    and trail_paid_to_dt <= to_date(ptodate, 'DD-MM-YYYY');

  delete from wealthmaker.import_folios_temp where login_id = puserid and role_id = prole_id;
  commit;
end;
/