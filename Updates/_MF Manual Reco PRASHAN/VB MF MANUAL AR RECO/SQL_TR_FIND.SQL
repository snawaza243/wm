
select inv.Investor_Name,inv.address1||','||inv.address2||','||inv.phone||','||inv.email address ,ct.city_name 
 ,mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,
Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.tran_type tran_type, mf.sip_type sip_type, CASE WHEN  mf.loggeduserid='MFONLINE' THEN 'Online' WHEN  mf.loggeduserid='Valuefy' THEN 'Online' ELSE 'Offline' end loggeduserid 
 from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1 mf 
 where  MF.MOVE_FLAG1 IS NULL AND MF.SIP_ID IS  NULL AND TRAN_TYPE NOT IN('REVERTAL') AND (MF.ASA<>'C' OR MF.ASA IS NULL) AND inv.city_id=ct.city_id(+) and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  
    --If cmbbranch.Text <> "" And cmbbranch.Text <> "ALL" And MyBranchCode <> "" Then
    and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & MyBranchCode & ")
    --Else  and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & Branches & ")" 
    --If cmbcategory.Text <> "" And cmbcategory.Text <> "All" And MyBranchCat <> "" Then
    and b.branch_tar_cat='" & MyBranchCat & "' 
     
    --If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" And MyRegionCode <> "" Then
         and b.region_id ='" & MyRegionCode & "'
    
    --If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" And MyZoneCode <> "" Then
    and b.zone_id ='" & MyZoneCode & "'
    
    --If CmbRM.Text <> "" And UCase(CmbRM.Text) <> "ALL" And MyRmCode <> "" Then
     and e.payroll_id ='" & MyRmCode & "'
  
    --If SRmCode <> "" Then
     and e.rm_code=" & SRmCode & "
    
    --If OpTregulare.Value = True Then
     AND (MF.SIP_TYPE='REGULAR' OR MF.SIP_TYPE IS NULL OR MF.SIP_TYPE ='STP')
    
    --If OptSip.Value = True Then
     AND (MF.DISPATCH='N')
    
    --If Trim(txtAR.Text) <> "" Then
         AND MF.tran_code='" & Trim(txtAR.Text) & "'
    
    --If optPMS.Value = True Then
         AND ((sch.prd='DT027')) 
    
    --If optATM.Value = True Then
         AND NVL(ATM_FLAG,'0')='1' 
    
    --If optTrail.Value = True Then
         AND get_scheme_nature_trail_NEW(mf.sch_code,MF.TR_DATE)='T' and pms_identify(tran_code)='0' 
    
    
    --If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
         and mf.mut_code ='" & MyMutCode & "'
    
    
    --If OptReconcileTR.Value = True Then
         and mf.rec_flag='Y' 
    --ElseIf OptUnReconcileTr.Value = True Then
         and (mf.rec_flag ='N' or mf.rec_flag is null)
    --ElseIf optPending.Value = True Then
         and (mf.rec_flag ='N' or mf.rec_flag is null) and (processed is not null or remark_reco='REC0015')
    
    
    --If ChkCob.Value = 1 Then
         and mf.cob_flag=1
    
    
    --If Trim(txtAR.Text) = "" Then
    --    If mskfromdt.Text <> "__/__/____" Then
             and tr_date>=TO_DATE('" & mskfromdt1 & "','DD/MM/YYYY') 
        
    --    If msktodt.Text <> "__/__/____" Then
             and tr_date<=TO_DATE('" & msktodt1 & "','DD/MM/YYYY') 
        
    
    
     order by 
    --If MyOrder1 = "" Then
          TR_Date,rm_name ;
    --Else        " & MyOrder1
    
    --If Option1(3).Value = True Then Desc"
    
    Set rsMap1 = New ADODB.Recordset