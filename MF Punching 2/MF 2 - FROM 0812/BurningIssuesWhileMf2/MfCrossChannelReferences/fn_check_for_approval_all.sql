create or replace function fn_check_for_approval_all (
   pdt_no varchar2
) return number is
   vflag              number;
   vgi_request        wealthmaker.tb_doc_upload.gi_request%type;
   vgi_request_status wealthmaker.tb_doc_upload.gi_request_status%type;
   vsch_code          wealthmaker.tb_doc_upload.sch_code%type;
   vfresh_renew       wealthmaker.tb_doc_upload.fresh_renew%type;
   vmain_pt_code      wealthmaker.bajaj_policy_type.main_pt_code%type;
begin
   begin
      select nvl(
         gi_request,
         0
      ),
             nvl(
                gi_request_status,
                0
             ),
             sch_code,
             fresh_renew
        into
         vgi_request,
         vgi_request_status,
         vsch_code,
         vfresh_renew
        from wealthmaker.tb_doc_upload
       where common_id = pdt_no
         and tran_type in ( 'LI',
                            'MF',
                            'FI',
                            'GI' )
         and rownum = 1;
   exception
      when others then
         vgi_request := 0;
   end;

   if
      vgi_request = 1
      and vgi_request_status = 0
   then
      vflag := 2;       /*Approval request for this transaction has already been raised.*/
   elsif
      vgi_request = 1
      and vgi_request_status = 1
   then
      vflag := 0;       /* Approval Request for this transaction has been approved.*/
   elsif
      vgi_request = 1
      and vgi_request_status = 2
   then
      vflag := 4;       /*Approval request for this transaction has rejected by Management.*/
   elsif vgi_request = 0 then
      vflag := 1;       /*Raise A Approval Request*/
   end if;

   return vflag;
end;