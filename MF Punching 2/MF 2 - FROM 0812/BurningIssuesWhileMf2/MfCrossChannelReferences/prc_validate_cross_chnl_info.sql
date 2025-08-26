create or replace procedure prc_validate_cross_chnl_info (
   pcommon_id     varchar2,
   psub_client_cd varchar2,
   plogin_id      varchar2,
   pcnt           out integer
) as
   vpan           wealthmaker.investor_master.pan%type;
   vmobile        varchar2(20);
   vemail         wealthmaker.investor_master.email%type;
   vcnt           number(5) := 0;
   vcntgroupid    number(5) := 0;
   vlastclient    wealthmaker.investor_master.inv_code%type;
   vcntunalocated number(2) := 0;
   vgroupid       wealthmaker.subbroker_grp_mapping.group_id%type;
   isvalidpan     varchar2(10);
   isvalidmobile  number(1);
   isvalidemail   number(1);
   vcntmaininfo   number(2) := 0;
begin
   if psub_client_cd like '4%' then   /* no validation for client */
      pcnt := 0;
      return;
   end if;
   delete from wealthmaker.cross_channel_investor_info
    where common_id = pcommon_id
      and loggeduserid = plogin_id;

   commit;
   select upper(trim(substr(
      pan,
      1,
      10
   ))),
          mobile,
          upper(email),
          validatepan(upper(trim(substr(
             pan,
             1,
             10
          )))),
          validate_mobile(mobile),
          validate_email(upper(email))
     into
      vpan,
      vmobile,
      vemail,
      isvalidpan,
      isvalidmobile,
      isvalidemail
     from wealthmaker.investor_master
    where inv_code = psub_client_cd;

   if
      isvalidpan = 'InValid'
      and isvalidmobile = 0
      and isvalidemail = 0
   then
      pcnt := 0;
      return;
   end if;
          
          ----------------------------------------------------------------------------------INSERT ALL INFO ------------------------------------------------------------------------------------------
   if isvalidpan = 'Valid' then
      insert into wealthmaker.cross_channel_investor_info (
         common_id,
         search_key,
         inv_code,
         client_name,
         rm_name,
         pan,
         email,
         mobile,
         branch_name,
         zone_name,
         region_name,
         channel,
         loggeduserid
      )
         select pcommon_id,
                'PAN',
                client_codekyc,
                client_name,
                rm_name,
                client_pan,
                email,
                mobile_no,
                branch_name,
                zone_name,
                region_name,
                (
                   select itemname
                     from wealthmaker.fixeditem
                    where itemserialnumber in (
                      select branch_tar_cat
                        from wealthmaker.branch_master
                       where branch_code = t.branch_code
                   )
                ) itemname,
                plogin_id
           from wealthmaker.temp_client_details@mf.bajajcapital t
          where client_pan = vpan
            and substr(
            client_codekyc,
            1,
            8
         ) <> substr(
            psub_client_cd,
            1,
            8
         )
            and length(client_codekyc) >= 11;
   end if;

   vcntmaininfo := 0;
   select count(*)
     into vcntmaininfo
     from wealthmaker.cross_channel_investor_info
    where common_id = pcommon_id;

   if
      isvalidmobile = 1
      and isvalidemail = 1
      and vcntmaininfo = 0
   then
      insert into wealthmaker.cross_channel_investor_info (
         common_id,
         search_key,
         inv_code,
         client_name,
         rm_name,
         pan,
         email,
         mobile,
         branch_name,
         zone_name,
         region_name,
         channel,
         loggeduserid
      )
         select pcommon_id,
                'MOBILE AND EMAIL',
                client_codekyc,
                client_name,
                rm_name,
                client_pan,
                email,
                mobile_no,
                branch_name,
                zone_name,
                region_name,
                (
                   select itemname
                     from wealthmaker.fixeditem
                    where itemserialnumber in (
                      select branch_tar_cat
                        from wealthmaker.branch_master
                       where branch_code = t.branch_code
                   )
                ) itemname,
                plogin_id
           from wealthmaker.temp_client_details@mf.bajajcapital t
          where mobile_no = vmobile
            and email = vemail
            and substr(
            client_codekyc,
            1,
            8
         ) <> substr(
            psub_client_cd,
            1,
            8
         )
            and length(client_codekyc) >= 11;
   end if;

   vcntmaininfo := 0;
   select count(*)
     into vcntmaininfo
     from wealthmaker.cross_channel_investor_info
    where common_id = pcommon_id;

   if
      isvalidmobile = 1
      and vcntmaininfo = 0
   then
      insert into wealthmaker.cross_channel_investor_info (
         common_id,
         search_key,
         inv_code,
         client_name,
         rm_name,
         pan,
         email,
         mobile,
         branch_name,
         zone_name,
         region_name,
         channel,
         loggeduserid
      )
         select pcommon_id,
                'MOBILE',
                client_codekyc,
                client_name,
                rm_name,
                client_pan,
                email,
                mobile_no,
                branch_name,
                zone_name,
                region_name,
                (
                   select itemname
                     from wealthmaker.fixeditem
                    where itemserialnumber in (
                      select branch_tar_cat
                        from wealthmaker.branch_master
                       where branch_code = t.branch_code
                   )
                ) itemname,
                plogin_id
           from wealthmaker.temp_client_details@mf.bajajcapital t
          where mobile_no = vmobile
            and substr(
            client_codekyc,
            1,
            8
         ) <> substr(
            psub_client_cd,
            1,
            8
         )
            and length(client_codekyc) >= 11;
   end if;
          
          
          /* FLAGGING OF THOSE CASES WHICH IS APPROVED */
   update wealthmaker.cross_channel_investor_info t
      set
      approved = wealthmaker.fn_get_trf_approval(
         pcommon_id,
         psub_client_cd,
         t.inv_code
      )
    where common_id = pcommon_id
      and loggeduserid = plogin_id;

   commit;
   select count(*)
     into vcnt
     from wealthmaker.cross_channel_investor_info
    where common_id = pcommon_id
      and loggeduserid = plogin_id;
   pcnt := nvl(
      vcnt,
      0
   );
end;