
    StrSql = " select inv.Investor_Name,inv.address1||','||inv.address2||','||inv.phone||','||inv.email address ,ct.city_name "
    StrSql = StrSql & " ,mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,"
    StrSql = StrSql & "Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.tran_type tran_type, mf.sip_type sip_type, CASE WHEN  mf.loggeduserid='MFONLINE' THEN 'Online' WHEN  mf.loggeduserid='Valuefy' THEN 'Online' ELSE 'Offline' end loggeduserid "
    StrSql = StrSql & " from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1 mf "
    StrSql = StrSql & " where  MF.MOVE_FLAG1 IS NULL AND MF.SIP_ID IS  NULL AND TRAN_TYPE NOT IN('REVERTAL') AND (MF.ASA<>'C' OR MF.ASA IS NULL) AND inv.city_id=ct.city_id(+) and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  "
    If cmbbranch.Text <> "" And cmbbranch.Text <> "ALL" And MyBranchCode <> "" Then
        StrSql = StrSql & " and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & MyBranchCode & ")"
    Else
        StrSql = StrSql & " and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & Branches & ")"
    End If
    If cmbcategory.Text <> "" And cmbcategory.Text <> "All" And MyBranchCat <> "" Then
         StrSql = StrSql & "      and b.branch_tar_cat='" & MyBranchCat & "' "
    End If
    If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" And MyRegionCode <> "" Then
        StrSql = StrSql & " and b.region_id ='" & MyRegionCode & "'"
    End If
    If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" And MyZoneCode <> "" Then
        StrSql = StrSql & " and b.zone_id ='" & MyZoneCode & "'"
    End If
    If CmbRM.Text <> "" And UCase(CmbRM.Text) <> "ALL" And MyRmCode <> "" Then
        StrSql = StrSql & " and e.payroll_id ='" & MyRmCode & "'"
    End If
    If SRmCode <> "" Then
         StrSql = StrSql & " and e.rm_code=" & SRmCode & ""
    End If
    If OpTregulare.Value = True Then
        StrSql = StrSql & " AND (MF.SIP_TYPE='REGULAR' OR MF.SIP_TYPE IS NULL OR MF.SIP_TYPE ='STP')"
    End If
    If OptSip.Value = True Then
        StrSql = StrSql & " AND (MF.DISPATCH='N')"
    End If
    If Trim(txtAR.Text) <> "" Then
        StrSql = StrSql & " AND MF.tran_code='" & Trim(txtAR.Text) & "'"
    End If
    If optPMS.Value = True Then
        StrSql = StrSql & " AND ((sch.prd='DT027')) "
    End If
    If optATM.Value = True Then
        StrSql = StrSql & " AND NVL(ATM_FLAG,'0')='1' "
    End If
    If optTrail.Value = True Then
        StrSql = StrSql & " AND get_scheme_nature_trail_NEW(mf.sch_code,MF.TR_DATE)='T' and pms_identify(tran_code)='0' "
    End If
    
    If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
        StrSql = StrSql & " and mf.mut_code ='" & MyMutCode & "'"
    End If
    
    If OptReconcileTR.Value = True Then
        StrSql = StrSql & " and mf.rec_flag='Y'"
    ElseIf OptUnReconcileTr.Value = True Then
        StrSql = StrSql & " and (mf.rec_flag ='N' or mf.rec_flag is null)"
    ElseIf optPending.Value = True Then
        StrSql = StrSql & " and (mf.rec_flag ='N' or mf.rec_flag is null) and (processed is not null or remark_reco='REC0015')"
    End If
    
    If ChkCob.Value = 1 Then
        StrSql = StrSql & " and mf.cob_flag=1"
    End If
    
    If Trim(txtAR.Text) = "" Then
        If mskfromdt.Text <> "__/__/____" Then
            StrSql = StrSql & " and tr_date>=TO_DATE('" & mskfromdt1 & "','DD/MM/YYYY') "
        End If
        If msktodt.Text <> "__/__/____" Then
            StrSql = StrSql & " and tr_date<=TO_DATE('" & msktodt1 & "','DD/MM/YYYY') "
        End If
    End If
    
    StrSql = StrSql & " order by "
    If MyOrder1 = "" Then
        StrSql = StrSql & "  TR_Date,rm_name "
    Else
        StrSql = StrSql & "" & MyOrder1
    End If
    If Option1(3).Value = True Then
        StrSql = StrSql & " Desc"
    End If