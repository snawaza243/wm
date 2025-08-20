Private Sub gridfill(X As Integer)
On Error GoTo err1
If X = 1 Then
    StrSql = " SELECT   FOLIO_NO, SCH_CODE, TR_DATE, TRAN_TYPE, "
    StrSql = StrSql & "         MAX (INVESTOR_NAME) INVESTOR_NAME, MAX (ADDRESS) ADDRESS, "
    StrSql = StrSql & "         MAX (BROKER_CODE) BROKER_CODE, MAX (CITY_NAME) CITY_NAME, MAX(SCH_NAME)SCH_NAME, "
    StrSql = StrSql & "         MAX(MUT_CODE)MUT_CODE, MAX(MUT_NAME)MUT_NAME, MAX(RM_NAME)RM_NAME, MAX(BRANCH_NAME)BRANCH_NAME, MAX (APP_NO) APP_NO, "
    StrSql = StrSql & "         MAX (CHEQUE_NO) CHEQUE_NO, SUM (AMOUNT) AMOUNT, "
    StrSql = StrSql & "         MAX (BUSI_BRANCH_CODE) BUSI_BRANCH_CODE, "
    StrSql = StrSql & "         MAX (BUSINESS_RMCODE) BUSINESS_RMCODE,"
    If Optcams.Value = True Then
        StrSql = StrSql & " GETTRANCODE_NEW(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,tran_id)UNIQUE_TRAN ,"
    Else
        StrSql = StrSql & " GETTRANCODE(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE)UNIQUE_TRAN ,"
    End If
    StrSql = StrSql & " max(reg_trantype) reg_trantype,max(unq_key)unq_key"
    StrSql = StrSql & "   FROM ( "
    StrSql = StrSql & " SELECT t.tran_id,tran_type, t.inv_name Investor_Name,(i.ADDRESS1||','||i.ADDRESS2||','||i.EMAIL) address, "
    StrSql = StrSql & "         REG_SUBBROK broker_code,c.city_name, "
    StrSql = StrSql & "         t.sch_code,sch_name sch_name,t.mut_code,amc.mut_name mut_name, rm_name,  "
    StrSql = StrSql & "         branch_name,  "
    StrSql = StrSql & "         app_no,folio_no, cheque_no, "
    StrSql = StrSql & "         amount,  t.tran_code,reg_date as tr_date, "
    StrSql = StrSql & "         busi_branch_code, business_rmcode, t.reg_trantype,t.unq_key"
    StrSql = StrSql & "    FROM employee_master e, "
    StrSql = StrSql & "         branch_master b, "
    StrSql = StrSql & "         mut_fund amc, "
    StrSql = StrSql & "         scheme_info sch, "
    StrSql = StrSql & "         TRANSACTION_ST@MF.BAJAJCAPITAL t, "
    StrSql = StrSql & "         investor_master i,CITY_MASTER C "
    StrSql = StrSql & "   WHERE I.CITY_ID=C.CITY_ID(+) "
    StrSql = StrSql & "     AND t.client_code = i.inv_code "
    StrSql = StrSql & "     AND to_char(t.rmcode) = e.rm_code "
    StrSql = StrSql & "     AND t. BUSI_BRANCH_CODE = b.branch_code   "
    StrSql = StrSql & "     AND t.mut_code = amc.mut_code AND t.sch_code = sch.sch_code AND DUP_FLAG2=0 "
    
    If cmbbranchAr.Text <> "" And cmbbranchAr.Text <> "ALL" And MyBranchCode <> "" Then
         StrSql = StrSql & "      and BUSI_BRANCH_CODE in (" & MyBranchCode & ") "
    Else
         StrSql = StrSql & "      and BUSI_BRANCH_CODE in (" & Branches & ") "
    End If
    If SRmCode <> "" Then
         StrSql = StrSql & " and e.rm_code=" & SRmCode & ""
    End If
    If cmbcategory.Text <> "" And cmbcategory.Text <> "All" And MyBranchCat <> "" Then
         StrSql = StrSql & "      and b.branch_tar_cat='" & MyBranchCat & "' "
    End If
    
   
    If CmbAmcAR.Text <> "" And CmbAmcAR.Text <> "ALL" And MyMutCodeAr <> "" Then
        StrSql = StrSql & " and to_char(t.mut_code) ='" & MyMutCodeAr & "'"
    End If
    
    If OptReconcileAR.Value = True Then
        StrSql = StrSql & " and t.rec_flag ='Y'"
    ElseIf OptUnReconcileAR.Value = True Then
        StrSql = StrSql & " and (t.rec_flag ='N' or rec_flag is null)"
    End If
    If Optregular_st.Value = True Then
        StrSql = StrSql & " AND ((UPPER(t.REG_TRANTYPE) NOT LIKE '%SYS%' AND UPPER(t.REG_TRANTYPE) NOT LIKE '%SIP%') OR t.REG_TRANTYPE IS NULL)"
    End If
    If OptSip_St.Value = True Then
        StrSql = StrSql & " AND ((UPPER(t.REG_TRANTYPE)  LIKE '%SYS%' OR UPPER(t.REG_TRANTYPE)  LIKE '%SIP%')) "
    End If
  
    If mskfromdt.Text <> "__/__/____" Then
        StrSql = StrSql & " and tr_date>=TO_DATE('" & mskfromdt & "','DD/MM/YYYY') "
    End If
    If msktodt.Text <> "__/__/____" Then
        StrSql = StrSql & " and tr_date<=TO_DATE('" & msktodt & "','DD/MM/YYYY') "
    End If
    If CmbSearch.ListIndex = 0 And TxtSearch <> "" Then
        StrSql = StrSql & " and ( t.CHEQUE_NO = '" & UCase(TxtSearch.Text) & "' or  t.CHEQUE_NO = '" & Val(TxtSearch.Text) & "' ) "
    ElseIf CmbSearch.ListIndex = 1 And TxtSearch <> "" Then
        StrSql = StrSql & " and t.folio_no = '" & UCase(TxtSearch.Text) & "'"
    ElseIf CmbSearch.ListIndex = 2 And TxtSearch <> "" Then
        StrSql = StrSql & " and t.app_no = '" & UCase(TxtSearch.Text) & "'"
    ElseIf CmbSearch.ListIndex = 4 And TxtSearch <> "" Then
        StrSql = StrSql & " and t.REG_SUBBROK = '" & UCase(TxtSearch.Text) & "'"
    ElseIf CmbSearch.ListIndex = 3 And TxtSearch <> "" Then
        StrSql = StrSql & " and (upper(t.pan1) = '" & UCase(TxtSearch.Text) & "' or upper(t.pan2) = '" & UCase(TxtSearch.Text) & "' or upper(t.pan3) = '" & UCase(TxtSearch.Text) & "')"
    End If

    If TxtInvestorName.Text <> "" Then
        StrSql = StrSql & " and upper(trim(T.inv_name)) like '%" & Replace(UCase(TxtInvestorName.Text), " ", "%") & "%'"
    End If
    If TxtAmount.Text <> "" Then
        StrSql = StrSql & " and abs(round(t.amount)) = " & Abs(Round(Val(TxtAmount.Text))) & ""
    End If
    StrSql = StrSql & "  AND LPAD (t.mut_code, 2) = 'MF' "
    StrSql = StrSql & "     AND (t.asa <> 'C' OR t.asa IS NULL) "
    StrSql = StrSql & "                                    AND t.tran_type IN "
    StrSql = StrSql & "                                           ('PURCHASE', 'REINVESTMENT', "
    StrSql = StrSql & "                                            'SWITCH IN') "
    StrSql = StrSql & "ORDER BY "
    
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_Date,rm_name "
    End If
    StrSql = StrSql & " ) "
    StrSql = StrSql & "GROUP BY FOLIO_NO, "
    StrSql = StrSql & "         SCH_CODE, "
    StrSql = StrSql & "         TR_DATE, "
    If Optcams.Value = True Then
        StrSql = StrSql & "         TRAN_TYPE,tran_id "
    Else
        StrSql = StrSql & "         TRAN_TYPE"
    End If
    
    Set rsMap = New ADODB.Recordset
    rsMap.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
ElseIf X = 2 Then
    StrSql = " select inv.Investor_Name,inv.address1||','||inv.address2||','||inv.phone||','||inv.email address ,ct.city_name "
    StrSql = StrSql & " ,mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,"
    StrSql = StrSql & "Amount,Sip_Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.INSTALLMENTS_NO,RETURNSTATUS(MF.TRAN_CODE)STATUS, CASE WHEN  mf.loggeduserid='MFONLINE' THEN 'Online' WHEN  mf.loggeduserid='Valuefy' THEN 'Online' ELSE 'Offline' end loggeduserid "
    StrSql = StrSql & " from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1  mf "
    StrSql = StrSql & " where  MF.SIP_TYPE='SIP' AND AMOUNT>0 AND (MF.ASA<>'C' OR MF.ASA IS NULL) and move_flag1 is null and sip_id is null AND inv.city_id=ct.city_id(+) and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  "
    
    
    
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
    
    '------------------VINIT 06 JUN 2015
    
    If TxtAR.Text <> "" Then
         StrSql = StrSql & "  and mf.tran_code='" & TxtAR.Text & "' "
    End If
    
    '------------------VINIT 06 JUN 2015
    
    
    If OptSip.Value = True Then
        StrSql = StrSql & " AND (MF.SIP_TYPE='SIP' AND (SIP_FR='F' OR SIP_FR IS NULL))"
    End If
    If optrenewal.Value = True Then
        StrSql = StrSql & " AND (SIP_FR='R')"
    End If
    
    If chkPMS.Value = 1 Then
        StrSql = StrSql & " AND ((sch.prd='DT027') or sch.sch_code in(select sch_code From scheme_info where nature='ETF')) "
    End If
    
    If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
        StrSql = StrSql & " and mf.mut_code ='" & MyMutCode & "'"
    End If
    
    If OptReconcileTR.Value = True Then
        StrSql = StrSql & " and nvl(mf.rec_flag,'N')='Y'"
    ElseIf OptUnReconcileTr.Value = True Then
        StrSql = StrSql & " and nvl(mf.rec_flag,'N')='N'"
    End If
    
    If ChkCob.Value = 1 Then
        StrSql = StrSql & " and mf.cob_flag='1'"
    End If
    
    If mskfromdt.Text <> "__/__/____" Then
        StrSql = StrSql & " and tr_date>=TO_DATE('" & mskfromdt1 & "','DD/MM/YYYY') "
    End If
    If msktodt.Text <> "__/__/____" Then
        StrSql = StrSql & " and tr_date<=TO_DATE('" & msktodt1 & "','DD/MM/YYYY') "
    End If


    If OptBaseRecon.Value = True Then
        StrSql = StrSql & " and RETURNSTATUS(MF.TRAN_CODE)='BASE RECONCILE'"
    End If
    If OptSipReco.Value = True Then
        StrSql = StrSql & " and RETURNSTATUS(MF.TRAN_CODE)='SIP RECONCILE'"
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
    Set rsMap1 = New ADODB.Recordset
    rsMap1.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
Else
    StrSql = " SELECT A.SEQ_NO,A.MUT_CODE,B.MUT_NAME,A.SCH_CODE,C.SCH_NAME,A.FOLIO_NO,IHNO_TRXNNO,START_DATE SIP_START_DATE,END_DATE SIP_END_DATE, "
    StrSql = StrSql & " SIPREGDATE,SIP_OPTION,AMOUNT_SIP,TOTAL_SIP,RM_NAME,PAYROLL_ID,BRANCH_CODE,BRANCH_NAME FROM  "
    StrSql = StrSql & " TRAN_SIP_FEED A, "
    StrSql = StrSql & " MUT_FUND B, "
    StrSql = StrSql & " SCHEME_INFO C, "
    StrSql = StrSql & " EMPLOYEE_MASTER E, "
    StrSql = StrSql & " BRANCH_MASTER BR "
    StrSql = StrSql & " WHERE A.MUT_CODE=B.MUT_CODE AND A.SCH_CODE=C.SCH_CODE AND BASE_TRAN_CODE IS NULL "
    StrSql = StrSql & " AND A.CLIENT_RM_CODE=E.RM_CODE AND A.CLIENT_BRANCH_CODE=BR.BRANCH_CODE and ok_flag='OK' "
    
    
    If txtSipFoliono.Text <> "" Then
        StrSql = StrSql & "  AND A.FOLIO_NO='" & txtSipFoliono.Text & "'"
    End If
    If txtSipAmount.Text <> "" Then
        StrSql = StrSql & "  AND A.AMOUNT_SIP='" & Round(Val(txtSipAmount.Text)) & "'"
    End If
    If TxtSipPan.Text <> "" Then
        StrSql = StrSql & "  AND (upper(A.pan)='" & UCase(TxtSipPan.Text) & "' or upper(A.joint1_pan)='" & UCase(TxtSipPan.Text) & "' or upper(A.joint2_pan)='" & UCase(TxtSipPan.Text) & "')"
    End If
    If MaskSipEdBox.Text <> "__/__/____" Then
        StrSql = StrSql & " and START_DATE BETWEEN ADD_MONTHS(TO_DATE('" & MaskSipEdBox & "','DD/MM/RRRR'),-2) AND ADD_MONTHS(TO_DATE('" & MaskSipEdBox & "','DD/MM/RRRR'),2) "
    End If
    If cmbcategory.Text <> "" And cmbcategory.Text <> "All" And MyBranchCat <> "" Then
         StrSql = StrSql & "      and br.branch_tar_cat='" & MyBranchCat & "' "
    End If
    If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" And MyRegionCode <> "" Then
        StrSql = StrSql & " and br.region_id ='" & MyRegionCode & "'"
    End If
    If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" And MyZoneCode <> "" Then
        StrSql = StrSql & " and br.zone_id ='" & MyZoneCode & "'"
    End If
    If CmbRM.Text <> "" And UCase(CmbRM.Text) <> "ALL" And MyRmCode <> "" Then
        StrSql = StrSql & " and e.payroll_id ='" & MyRmCode & "'"
    End If
    If SRmCode <> "" Then
         StrSql = StrSql & " and e.rm_code=" & SRmCode & ""
    End If
    If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
        StrSql = StrSql & " and a.mut_code ='" & MyMutCode & "'"
    End If
    Set rsMap1 = New ADODB.Recordset
    rsMap1.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
End If
i = 1
j = 1
K = 1
If X = 1 Then
    VSFCommGrdS.Clear
ElseIf X = 2 Then
    VSFCommGrdT.Clear
Else
    VSFCommGrdK.Clear
End If
Call SetGrid
If X = 1 Then
    VSFCommGrdS.Rows = rsMap.RecordCount + 1
    If rsMap.EOF Then
        MsgBox "No record found corresponding to the searching criteria", vbInformation
        Exit Sub
    End If
    While Not rsMap.EOF
        VSFCommGrdS.TextMatrix(i, 0) = rsMap("UNIQUE_TRAN")
        VSFCommGrdS.TextMatrix(i, 1) = rsMap("tr_date")
        VSFCommGrdS.TextMatrix(i, 2) = IIf(IsNull(rsMap("Investor_Name")), "", rsMap("Investor_Name"))
        VSFCommGrdS.TextMatrix(i, 3) = IIf(IsNull(rsMap("Address")), "", rsMap("address"))
        VSFCommGrdS.TextMatrix(i, 4) = IIf(IsNull(rsMap("City_Name")), "", rsMap("City_Name"))
        VSFCommGrdS.TextMatrix(i, 5) = IIf(IsNull(rsMap("Mut_Name")), "", rsMap("Mut_Name"))
        VSFCommGrdS.TextMatrix(i, 6) = IIf(IsNull(rsMap("Sch_Name")), "", rsMap("Sch_Name"))
        VSFCommGrdS.TextMatrix(i, 7) = rsMap("Amount")
        VSFCommGrdS.TextMatrix(i, 8) = IIf(IsNull(rsMap("Folio_no")), "", rsMap("folio_no"))
        VSFCommGrdS.TextMatrix(i, 9) = IIf(IsNull(rsMap("cheque_no")), 0, rsMap("cheque_no"))
        VSFCommGrdS.TextMatrix(i, 10) = IIf(IsNull(rsMap("app_no")), 0, rsMap("app_no"))
        VSFCommGrdS.TextMatrix(i, 11) = IIf(IsNull(rsMap("Rm_Name")), "", rsMap("Rm_Name"))
        VSFCommGrdS.TextMatrix(i, 12) = IIf(IsNull(rsMap("Branch_Name")), "", rsMap("Branch_Name"))
        VSFCommGrdS.TextMatrix(i, 13) = IIf(IsNull(rsMap("broker_Code")), "", rsMap("Broker_Code"))
        VSFCommGrdS.TextMatrix(i, 14) = IIf(IsNull(rsMap("mut_code")), 0, rsMap("mut_code"))
        VSFCommGrdS.TextMatrix(i, 15) = IIf(IsNull(rsMap("sch_code")), 0, rsMap("sch_code"))
        VSFCommGrdS.TextMatrix(i, 16) = IIf(IsNull(rsMap("reg_trantype")), 0, rsMap("reg_trantype"))
        VSFCommGrdS.TextMatrix(i, 17) = IIf(IsNull(rsMap("unq_key")), 0, rsMap("unq_key"))
        i = i + 1
        rsMap.MoveNext
     Wend
     rsMap.Close
     Set rsMap = Nothing
ElseIf X = 2 Then
     VSFCommGrdT.Rows = rsMap1.RecordCount + 1
     LblCount = rsMap1.RecordCount
     While Not rsMap1.EOF
        VSFCommGrdT.TextMatrix(j, 0) = rsMap1("TRAN_code")
        VSFCommGrdT.TextMatrix(j, 1) = rsMap1("tr_date")
        VSFCommGrdT.TextMatrix(j, 2) = IIf(IsNull(rsMap1("Investor_Name")), "", rsMap1("Investor_Name"))
        VSFCommGrdT.TextMatrix(j, 3) = IIf(IsNull(rsMap1("Address")), "", rsMap1("address"))
        VSFCommGrdT.TextMatrix(j, 4) = IIf(IsNull(rsMap1("City_Name")), "", rsMap1("City_Name"))
        VSFCommGrdT.TextMatrix(j, 5) = IIf(IsNull(rsMap1("Mut_Name")), "", rsMap1("Mut_Name"))
        VSFCommGrdT.TextMatrix(j, 6) = IIf(IsNull(rsMap1("Sch_Name")), "", rsMap1("Sch_Name"))
        VSFCommGrdT.TextMatrix(j, 7) = IIf(IsNull(rsMap1("Amount")), 0, rsMap1("Amount"))
        VSFCommGrdT.TextMatrix(j, 8) = IIf(IsNull(rsMap1("Folio_no")), "", rsMap1("folio_no"))
        VSFCommGrdT.TextMatrix(j, 9) = IIf(IsNull(rsMap1("cheque_no")), 0, rsMap1("cheque_no"))
        VSFCommGrdT.TextMatrix(j, 10) = IIf(IsNull(rsMap1("app_no")), 0, rsMap1("app_no"))
        VSFCommGrdT.TextMatrix(j, 11) = IIf(IsNull(rsMap1("Rm_Name")), "", rsMap1("Rm_Name"))
        VSFCommGrdT.TextMatrix(j, 12) = IIf(IsNull(rsMap1("Branch_Name")), "", rsMap1("Branch_Name"))
        VSFCommGrdT.TextMatrix(j, 13) = IIf(IsNull(rsMap1("mut_code")), 0, rsMap1("mut_code"))
        VSFCommGrdT.TextMatrix(j, 14) = IIf(IsNull(rsMap1("sch_code")), 0, rsMap1("sch_code"))
        VSFCommGrdT.TextMatrix(j, 15) = IIf(IsNull(rsMap1("Sip_Amount")), 0, rsMap1("Sip_Amount"))
        VSFCommGrdT.TextMatrix(j, 16) = IIf(IsNull(rsMap1("INSTALLMENTS_NO")), 0, rsMap1("INSTALLMENTS_NO"))
        VSFCommGrdT.TextMatrix(j, 17) = IIf(IsNull(rsMap1("STATUS")), 0, rsMap1("STATUS"))
        VSFCommGrdT.TextMatrix(j, 18) = IIf(IsNull(rsMap1("loggeduserid")), 0, rsMap1("loggeduserid"))
        j = j + 1
        rsMap1.MoveNext
     Wend
     rsMap1.Close
     Set rsMap1 = Nothing
Else
    VSFCommGrdS.Rows = rsMap1.RecordCount + 1
     LblCount = rsMap1.RecordCount
     While Not rsMap1.EOF
        VSFCommGrdK.TextMatrix(K, 0) = rsMap1("seq_no")
        VSFCommGrdK.TextMatrix(K, 1) = IIf(IsNull(rsMap1("Mut_Name")), "", rsMap1("Mut_Name"))
        VSFCommGrdK.TextMatrix(K, 2) = IIf(IsNull(rsMap1("Sch_Name")), "", rsMap1("Sch_Name"))
        VSFCommGrdK.TextMatrix(K, 3) = IIf(IsNull(rsMap1("Amount_Sip")), 0, rsMap1("Amount_Sip"))
        VSFCommGrdK.TextMatrix(K, 4) = IIf(IsNull(rsMap1("Folio_no")), "", rsMap1("folio_no"))
        VSFCommGrdK.TextMatrix(K, 5) = rsMap1("SIP_START_DATE")
        VSFCommGrdK.TextMatrix(K, 6) = rsMap1("SIP_END_DATE")
        VSFCommGrdK.TextMatrix(K, 7) = rsMap1("SipRegDate")
        VSFCommGrdK.TextMatrix(K, 8) = IIf(IsNull(rsMap1("SIP_OPTION")), "", rsMap1("SIP_OPTION"))
        VSFCommGrdK.TextMatrix(K, 9) = IIf(IsNull(rsMap1("TOTAL_SIP")), 0, rsMap1("TOTAL_SIP"))
        VSFCommGrdK.TextMatrix(K, 10) = IIf(IsNull(rsMap1("RM_NAME")), 0, rsMap1("RM_NAME"))
        VSFCommGrdK.TextMatrix(K, 11) = IIf(IsNull(rsMap1("BRANCH_NAME")), 0, rsMap1("BRANCH_NAME"))
        VSFCommGrdK.TextMatrix(K, 12) = IIf(IsNull(rsMap1("MUT_CODE")), 0, rsMap1("MUT_CODE"))
        VSFCommGrdK.TextMatrix(K, 13) = IIf(IsNull(rsMap1("sch_code")), 0, rsMap1("sch_code"))
        VSFCommGrdK.TextMatrix(K, 14) = IIf(IsNull(rsMap1("PAYROLL_ID")), 0, rsMap1("PAYROLL_ID"))
        VSFCommGrdK.TextMatrix(K, 15) = IIf(IsNull(rsMap1("BRANCH_CODE")), 0, rsMap1("BRANCH_CODE"))
        K = K + 1
        rsMap1.MoveNext
     Wend
     rsMap1.Close
     Set rsMap1 = Nothing
End If
DoEvents
Exit Sub
err1:
    MsgBox err.Description
    Resume
End Sub
