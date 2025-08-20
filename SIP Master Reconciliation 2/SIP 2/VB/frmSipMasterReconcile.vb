Dim rsfindtr As New ADODB.Recordset
Dim rs_get_trantype As New ADODB.Recordset
Dim RsData As New ADODB.Recordset
Dim rslead As New ADODB.Recordset
Dim ARCODE As ADODB.Recordset
Dim MyRmName As String
Dim MyBusiCode As String
Dim strmsg As String
Dim MySipFolio As String
Dim MyBranch As String
Dim ARCODE1 As ADODB.Recordset
Dim rsMap As New ADODB.Recordset
Dim rsMap1 As New ADODB.Recordset
Dim ArLead() As String
Dim rsAmc As New ADODB.Recordset
Dim rsscheme As New ADODB.Recordset
Dim MyAmcCode As String
Dim MySchCode As String
Dim MyTranCode As String
Dim rs_branch As ADODB.Recordset
Dim ColumnIndex As Long
Dim Glb_L_SearchIndex As Integer
Dim Glb_L_SearchIndex2 As Integer
Dim Glb_Selected_row As Integer
Dim Glb_Flag_First_Time As Boolean
Dim MyOrder As String
Dim KCount_cOL As Integer
Dim MyTotalAmount1 As Double
Dim MyTotalAmount2 As Double
Dim MyTotalTrans1 As Long
Dim MyTotalTrans2 As Long
Dim MyTotalMargin1 As Double
Dim MyTotalmargin2 As Double
Dim MySelectedRow As Integer
Dim MyCurrRow As Integer
Dim MyCurrRowk As Integer
Dim MySelectedRow1 As Integer
Dim MyCurrRow1 As Integer
Dim ColumnIndex1 As Long
Dim ColumnIndex2 As Long
Dim Glb_L_SearchIndex1 As Integer
Dim Glb_Selected_row1 As Integer
Dim Glb_Selected_row2 As Integer
Dim Glb_Flag_First_Time1 As Boolean
Dim MyOrder1 As String

Dim paymode As String
Dim cmbList As String
Dim DoGrid As Integer
Dim MyPayble As Currency
Dim strbranch() As String
Dim MyBranchCode As String
Dim MyBranchCat As String
Dim MyRegionCode As String
Dim MyZoneCode As String
Dim MyRmCode As String
Dim MyMutCode As String
Dim MyMutCodeAr As String
Dim StrBranchAr() As String
Dim MyBranchCodeAr As String

Dim MyAppNo As String
Dim MyChqNo As String
Dim MyPan As String
Dim MyFolio As String
Dim MyAmount As Long
Dim MyBroker As String
Dim MyInvestorName As String
Dim MyTrCode As String
Dim MyTrDate As Date
Dim MyRtaTrCode As String
Dim MyRtaTrDate As String
Dim MyRtaAmount As String
Dim MyDispatch As String
Dim MyProdCode As String
Dim MySipFolioNo As String
Dim MySipStartDate As Date
Dim MySIPAmount As Long
Dim MasterId  As Long
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
    
'    If OptReconcileTR.Value = True Then
'        StrSql = StrSql & " and mf.rec_flag='Y' AND MF.SIP_REC_FLAG='Y'"
'    ElseIf OptUnReconcileTr.Value = True Then
'        StrSql = StrSql & " and ((mf.rec_flag ='N' or mf.rec_flag is null OR RTA_TRAN_CODE IS NULL) and (mf.SIP_rec_flag ='N' or mf.SIP_rec_flag is null))"
'    End If
    
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
Public Sub SelectText(obj As Object)
    obj.SelStart = 0
    obj.SelLength = Len(obj.Text)
Exit Sub
Errhandler:
    MsgBox err.Description, vbCritical
End Sub

Private Sub Clear_Previously_Selected()
    On Error GoTo Bot
    Dim KCount_cOL As Integer
    If Glb_Flag_First_Time = False Then
        Exit Sub
    End If
    If Glb_Selected_row <> 0 Then
         VSFCommGrdT.Row = Glb_Selected_row
         For KCount_cOL = 1 To VSFCommGrdT.Cols - 1
            VSFCommGrdT.Col = KCount_cOL
            VSFCommGrdT.CellBackColor = vbWhite
            VSFCommGrdT.FontBold = True
            VSFCommGrdT.CellForeColor = vbBlack
            VSFCommGrdT.CellFontBold = False
        Next
    End If
    Exit Sub
Bot:
    'Call ShowErrorMessage(err.Description)
End Sub
Private Sub Clear_Previously_Selected1()
    On Error GoTo Bot
    Dim KCount_cOL As Integer
    If Glb_Flag_First_Time1 = False Then
        Exit Sub
    End If
    If Glb_Selected_row1 <> 0 Then
         VSFCommGrdT.Row1 = Glb_Selected_row1
         For KCount_Col1 = 1 To VSFCommGrdT.Cols - 1
            VSFCommGrdT.Col = KCount_Col1
            VSFCommGrdT.CellBackColor = vbWhite
            VSFCommGrdT.FontBold = True
            VSFCommGrdT.CellForeColor = vbBlack
            VSFCommGrdT.CellFontBold = False
        Next
    End If
    Exit Sub
Bot:
    'Call ShowErrorMessage(err.Description)
End Sub
Private Sub Clear_Previously_Selected2()
    On Error GoTo Bot
    Dim KCount_cOL As Integer
    If Glb_Flag_First_Time1 = False Then
        Exit Sub
    End If
    If Glb_Selected_row2 <> 0 Then
         VSFCommGrdK.Row1 = Glb_Selected_row2
         For KCount_Col1 = 1 To VSFCommGrdK.Cols - 1
            VSFCommGrdK.Col = KCount_Col1
            VSFCommGrdK.CellBackColor = vbWhite
            VSFCommGrdK.FontBold = True
            VSFCommGrdK.CellForeColor = vbBlack
            VSFCommGrdK.CellFontBold = False
        Next
    End If
    Exit Sub
Bot:
    'Call ShowErrorMessage(err.Description)
End Sub
Private Sub cmdArPrint_Click()
If lbtrancode.Caption = "" Then
    MsgBox "Ar Can Not Be Print Right Now"
    Exit Sub
End If
'Sql = " create or replace view transaction_mfview as "
'Sql = Sql & " SELECT   mf.investor_name, mf.bank_name, mf.client_code, mf.sch_code, "
'Sql = Sql & "         rm_name, branch_name, panno, amc.mut_name amc_name, "
'Sql = Sql & "         sch_name scheme_name, tr_date, tran_type, app_no, payment_mode,  cheque_no, "
'Sql = Sql & "          CHEQUE_DATE, amount, sip_type, lead_no, lead_name, tran_code, branch_code, "
'Sql = Sql & "         BUSINESS_RMCODE "
'Sql = Sql & "    FROM wealthmaker.employee_master e, "
'Sql = Sql & "         wealthmaker.branch_master b, "
'Sql = Sql & "         wealthmaker.mut_fund amc, "
'Sql = Sql & "         wealthmaker.scheme_info sch, "
'Sql = Sql & "         wealthmaker.TRANSACTION_MF_TEMP1 mf "
'Sql = Sql & "   WHERE TO_CHAR (mf.BUSINESS_RMCODE) = TO_CHAR (e.payroll_id) "
'Sql = Sql & "     AND mf.BUSI_BRANCH_CODE = b.branch_code "
'Sql = Sql & "     AND mf.mut_code = amc.mut_code "
'Sql = Sql & "     AND mf.sch_code = sch.sch_code "
'Sql = Sql & "ORDER BY tr_date, rm_name "
'myconn.Execute Sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "test", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\Ar1.rpt"
CrystalReport1.SelectionFormula = "{transaction_mfview.tran_code}='" & lbtrancode.Caption & "'"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.action = 1
End Sub

Private Sub CmdArPrintM_Click()
If MyTranCode = "" Then
    MsgBox "Double Click The Record You Want To Print "
    Exit Sub
End If
'Sql = " create or replace view transaction_mfview as "
'Sql = Sql & " SELECT   mf.investor_name, mf.bank_name, mf.client_code, mf.sch_code, "
'Sql = Sql & "         rm_name, branch_name, panno, amc.mut_name amc_name, "
'Sql = Sql & "         sch_name scheme_name, tr_date, tran_type, app_no, payment_mode,  cheque_no, "
'Sql = Sql & "          CHEQUE_DATE, amount, sip_type, lead_no, lead_name, tran_code, branch_code, "
'Sql = Sql & "         BUSINESS_RMCODE "
'Sql = Sql & "    FROM wealthmaker.employee_master e, "
'Sql = Sql & "         wealthmaker.branch_master b, "
'Sql = Sql & "         wealthmaker.mut_fund amc, "
'Sql = Sql & "         wealthmaker.scheme_info sch, "
'Sql = Sql & "         wealthmaker.TRANSACTION_MF_TEMP1 mf "
'Sql = Sql & "   WHERE TO_CHAR (mf.BUSINESS_RMCODE) = TO_CHAR (e.payroll_id) "
'Sql = Sql & "     AND mf.BUSI_BRANCH_CODE = b.branch_code "
'Sql = Sql & "     AND mf.mut_code = amc.mut_code "
'Sql = Sql & "     AND mf.sch_code = sch.sch_code "
'Sql = Sql & "ORDER BY tr_date, rm_name "
'myconn.Execute Sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "test", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\LiContest.rpt"
CrystalReport1.SelectionFormula = "{transaction_mfview.tran_code}='" & MyTranCode & "'"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.action = 1
End Sub
Private Sub CmbAmcTR_Change()
Call CmbAmcTR_Click
End Sub
Private Sub CmbAmcTR_Click()
CmbAmcAR.ListIndex = CmbAmcTR.ListIndex
End Sub
Private Sub cmbbranch_Change()
Dim b_id() As String
b_id = Split(cmbbranch.Text, "#")
CmbRM.Clear
Dim RsCompany As New ADODB.Recordset
Dim Mysql As String
Mysql = "select rm_name,payroll_id from employee_master where 1=1"
If cmbbranch.Text <> "All" Then
     Mysql = Mysql & " and source=" & b_id(1) & ""
End If
If SRmCode <> "" Then
     Mysql = Mysql & " and rm_code=" & SRmCode & ""
End If
Mysql = Mysql & " and source in (" & Branches & ")"
Mysql = Mysql & " and type='A' and category_id in (2001,2018) order by rm_name"
CmbRM.Clear
RsCompany.open Mysql, MyConn
Do While Not RsCompany.EOF
     CmbRM.AddItem RsCompany("rm_name") & Space(90) & "#" & RsCompany("payroll_id")
     RsCompany.MoveNext
Loop
RsCompany.Close
Set RsCompany = Nothing
If cmbbranchAr.ListIndex <> -1 Then
    cmbbranchAr.ListIndex = cmbbranch.ListIndex
End If
End Sub

Private Sub cmbbranch_Click()
Call cmbbranch_Change
If cmbbranchAr.ListIndex <> -1 Then
    cmbbranchAr.ListIndex = cmbbranch.ListIndex
End If
End Sub


Private Sub CmbFind1_Click()
 Dim kCount As Integer
    Dim KCount_cOL As Integer
    Dim Column_name As Integer
    Dim KCount_Row As Integer
    Glb_Flag_First_Time1 = True
    If Trim(TxtFind1.Text) = "" Then
        Glb_L_SearchIndex1 = 1
        Exit Sub
    End If
     Clear_Previously_Selected1
     For kCount = Glb_L_SearchIndex To VSFCommGrdT.Rows - 1
        If InStr(1, UCase(VSFCommGrdT.TextMatrix(kCount, ColumnIndex1)), UCase(TxtFind1.Text)) > 0 Then
            Glb_Comming_From_Search = True
            VSFCommGrdT.Row = kCount
            If kCount > 12 Then VSFCommGrdT.TopRow = kCount
            For KCount_cOL = 1 To VSFCommGrdT.Cols - 1
                Glb_Comming_From_Search = True
                VSFCommGrdT.Col = KCount_cOL
                VSFCommGrdT.CellBackColor = vbWhite
                VSFCommGrdT.FontBold = True
                VSFCommGrdT.CellForeColor = vbRed
                VSFCommGrdT.CellFontBold = True
            Next
            Glb_L_SearchIndex = kCount + 1
            Glb_Selected_row = kCount
            VSFCommGrdT.TopRow = kCount
            Exit Sub
       End If
    Next
    If kCount >= VSFCommGrdT.Rows - 1 Then
        MsgBox "No Such Record Found", vbInformation, "Search Completed"
        TxtFind1.Text = ""
        TxtFind1.SetFocus
        Glb_L_SearchIndex = 1
        Glb_Selected_row = 0
    End If
End Sub

Private Sub cmbRegion_Change()
Dim RS As New ADODB.Recordset, Mysql As String
MyRegionCode = ""
If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" Then
    strbranch = Split(CmbRegion.Text, "#")
    MyRegionCode = strbranch(1)
End If
If CmbRegion.Text = "All" Then
    CmbZone.Clear
    cmbbranchAr.Clear
    cmbbranch.Clear
    Mysql = "select branch_name,branch_code from branch_master where 1=1"
    If GlbDataFilter <> "72" Then Mysql = Mysql & " and branch_code in (" & Branches & ")"
    Mysql = Mysql & " AND category_id not in ('1004','1005','1006') order by branch_name"
    RS.open Mysql, MyConn
    Do While Not RS.EOF
        cmbbranch.AddItem RS(0) & Space(80) & "#" & RS(1)
        cmbbranchAr.AddItem RS(0) & Space(80) & "#" & RS(1)
        RS.MoveNext
    Loop
    RS.Close
    cmbbranch.AddItem "All", 0
    cmbbranch.ListIndex = 0
    cmbbranchAr.AddItem "All", 0
    cmbbranchAr.ListIndex = 0
    Set RS = Nothing
    CmbZone.AddItem "All", 0
    CmbZone.ListIndex = 0
Else
    CmbZone.Clear
    Mysql = "select distinct zone_name,z.zone_id from branch_master b, zone_master z where b.zone_id=z.zone_id and b.region_id='" & MyRegionCode & "'"
    If GlbDataFilter <> "72" Then Mysql = Mysql & " and branch_code in (" & Branches & ")"
    Mysql = Mysql & " and category_id not in ('1004','1005','1006') order by zone_name"
    RS.open Mysql, MyConn, adOpenForwardOnly
    CmbZone.Clear
    cmbbranch.Clear
    Do While Not RS.EOF
        CmbZone.AddItem RS(0) & Space(80) & "#" & RS(1)
        RS.MoveNext
    Loop
    RS.Close
    Set RS = Nothing
    Reg_id = MyRegionCode
    Zon_id = ""
    b_id = ""
    CmbZone.AddItem "All", 0
    CmbZone.ListIndex = 0
End If
End Sub

Private Sub cmbRegion_Click()
Call cmbRegion_Change
End Sub

Private Sub CmbSearch_Change()
Call CmbSearch_Click
End Sub

Private Sub CmbSearch_Click()
If MyTrCode <> "" Then
    If CmbSearch.ListIndex = 0 Then
        TxtSearch.Text = MyChqNo
    ElseIf CmbSearch.ListIndex = 1 Then
        If Opt1.Value = True Then
            TxtSearch.Text = MySipFolio
        Else
            TxtSearch.Text = MyFolio
        End If
    ElseIf CmbSearch.ListIndex = 2 Then
        TxtSearch.Text = MyAppNo
    ElseIf CmbSearch.ListIndex = 3 Then
        TxtSearch.Text = MyPan
    ElseIf CmbSearch.ListIndex = 4 Then
        TxtSearch.Text = MyBroker
    End If
End If
End Sub






Private Sub CmbZone_Change()
Dim RS As New ADODB.Recordset, Mysql As String
Dim RS1 As New ADODB.Recordset, mysql1 As String
Dim Str() As String



MyRegionCode = ""
If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" Then
    strbranch = Split(CmbRegion.Text, "#")
    MyRegionCode = strbranch(1)
End If

MyZoneCode = ""
If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" Then
    strbranch = Split(CmbZone.Text, "#")
    MyZoneCode = strbranch(1)
End If


Mysql = "select branch_name,branch_code from branch_master where 1=1"
If GlbDataFilter <> "72" Then Mysql = Mysql & " and branch_code in (" & Branches & ")"
If CmbRegion.Text <> "All" Then Mysql = Mysql & " and Region_id='" & MyRegionCode & "'"
If CmbZone.Text <> "All" Then Mysql = Mysql & " and zone_id='" & MyZoneCode & "'"
'If Cmb_branch_cat.Text <> "All" Then Mysql = Mysql & " and branch_tar_cat='" & Trim(Mid(Cmb_branch_cat.Text, 52)) & "'"
Mysql = Mysql & " and category_id not in ('1004','1005','1006') order by branch_name"
RS.open Mysql, MyConn
cmbbranch.Clear
cmbbranchAr.Clear
Do While Not RS.EOF
    cmbbranch.AddItem RS(0) & Space(80) & "#" & RS(1)
    cmbbranchAr.AddItem RS(0) & Space(80) & "#" & RS(1)
    RS.MoveNext
Loop
RS.Close
cmbbranch.AddItem "All", 0
cmbbranch.ListIndex = 0
cmbbranchAr.AddItem "All", 0
cmbbranchAr.ListIndex = 0
Set RS = Nothing
End Sub


Private Sub CmbZone_Click()
Call CmbZone_Change
End Sub

Private Sub cmdConfirm_Click()
    If MyTrCode = "" Then
        MsgBox "First Double Click The Record , You Want To Map", vbInformation
        Exit Sub
    End If
    If TxtRemarks.Text = "" Then
        MsgBox "Please enter Remarks", vbInformation
        TxtRemarks.SetFocus
        Exit Sub
    End If
    
    MyConn.Execute ("Update  Transaction_mf_temp1 set remark_reco='" & Trim(TxtRemarks.Text) & "',REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update  Transaction_mf_temp1 set remark_reco='" & Trim(TxtRemarks.Text) & "',REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    
    
    MsgBox "Record Is confirmed Sucessfully", vbInformation


End Sub

Private Sub CmdExport_Click()
Call exportGRIDAll(VSFCommGrdT, "Commision", "Commision", "Sheet1")
End Sub

Private Sub CmdExport1_Click()
Call exportGRIDAll(VSFCommGrdS, "Commision", "Commision", "Sheet1")
End Sub

Private Sub cmdFind_Click()
Dim kCount As Integer
    Dim KCount_cOL As Integer
    Dim Column_name As Integer
    Dim KCount_Row As Integer
    Glb_Flag_First_Time = True
    If Trim(txtFind.Text) = "" Then
        Glb_L_SearchIndex = 1
        Exit Sub
    End If
     Clear_Previously_Selected
     For kCount = Glb_L_SearchIndex To VSFCommGrdS.Rows - 1
        If InStr(1, UCase(VSFCommGrdS.TextMatrix(kCount, ColumnIndex)), UCase(txtFind.Text)) > 0 Then
            Glb_Comming_From_Search = True
            VSFCommGrdS.Row = kCount
            If kCount > 12 Then VSFCommGrdS.TopRow = kCount
            For KCount_cOL = 1 To VSFCommGrdS.Cols - 1
                Glb_Comming_From_Search = True
                VSFCommGrdS.Col = KCount_cOL
                VSFCommGrdS.CellBackColor = vbWhite
                VSFCommGrdS.FontBold = True
                VSFCommGrdS.CellForeColor = vbRed
                VSFCommGrdS.CellFontBold = True
            Next
            Glb_L_SearchIndex = kCount + 1
            Glb_Selected_row = kCount
            VSFCommGrdS.TopRow = kCount
            Exit Sub
       End If
    Next
    If kCount >= VSFCommGrdS.Rows - 1 Then
        MsgBox "No Such Record Found", vbInformation, "Search Completed"
        txtFind.Text = ""
        txtFind.SetFocus
        Glb_L_SearchIndex = 1
        Glb_Selected_row = 0
    End If
End Sub

Private Sub CMDRESET_Click()
    TxtBusiCodeA = ""
    CmbAmcA.ListIndex = -1
    TxtBranchA.Text = ""
    TxtPanA.Text = ""
    TxtSchemeA.Text = ""
    ImEntryDtF.Text = Format(Now, "DD/MM/YYYY")
    cmbTranTypeA.Text = ""
    TxtAppnoA.Text = ""
    txtInvestorA.Text = ""
    TxtClientCodeA.Text = ""
    txtChqNo.Text = ""
    dtChqDate.Text = Format(Now, "DD/MM/YYYY")
    TxtAmountA = ""
    CmbSipStpA.Text = ""
    Lbl_Leadno.Caption = ""
    lbl_lead_caption.Caption = ""
    MyTranCode = ""
    MyBranchCode = ""
    TxtBusiCodeM = ""
End Sub

Private Sub cmdfindtran_Click()
Dim findsql As String
Dim X As Integer
VSFlexGrid1_reset
findsql = ""
findsql = "select nvl(HO_TRAN_CODE,0)HO_TRAN_CODE,branch_name from TRANSACTION_ST@mf.bajajcapital a,branch_master b where a.branch_code=b.branch_code and TRAN_CODE='" & txtfindtran.Text & "'"
rsfindtr.open findsql, MyConn, adOpenForwardOnly, adLockOptimistic
X = 1
While Not rsfindtr.EOF   'Or rsfindtr.BOF
        VSFlexGrid1.TextMatrix(X, 0) = IIf(IsNull(rsfindtr("HO_TRAN_CODE")), "", rsfindtr("HO_TRAN_CODE"))
        VSFlexGrid1.TextMatrix(X, 1) = IIf(IsNull(rsfindtr("branch_name")), "", rsfindtr("branch_name"))
        X = X + 1
        rsfindtr.MoveNext
Wend
rsfindtr.Close
End Sub

Private Sub cmdGo_Click()
MyBranchCode = ""
If cmbbranchAr.Text <> "" And UCase(cmbbranchAr.Text) <> "ALL" Then
    strbranch = Split(cmbbranchAr.Text, "#")
    MyBranchCode = strbranch(1)
End If

MyMutCodeAr = ""
If CmbAmcAR.Text <> "" And UCase(CmbAmcAR.Text) <> "ALL" Then
    strbranch = Split(CmbAmcAR.Text, "#")
    MyMutCodeAr = strbranch(1)
End If


CmdGo.Enabled = False
MyTotalAmount1 = 0
MyTotalMargin1 = 0
MyTotalTrans1 = 0
Call gridfill(1)
CmdGo.Enabled = True
End Sub

Private Sub CmdGo1_Click()
cmbbranchAr.ListIndex = cmbbranch.ListIndex
MyTrCode = ""
MyBranchCode = ""
If cmbbranch.Text <> "" And UCase(cmbbranch.Text) <> "ALL" Then
    strbranch = Split(cmbbranch.Text, "#")
    MyBranchCode = strbranch(1)
End If

If Trim(cmbcategory.Text) <> "" And Trim(cmbcategory.Text) <> "-Select Branch-" And UCase(Trim(cmbcategory.Text)) <> "ALL" Then
    strbranch = Split(Trim(cmbcategory.Text), "#")
    MyBranchCat = Trim(strbranch(1))
End If

MyRegionCode = ""
If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" Then
    strbranch = Split(CmbRegion.Text, "#")
    MyRegionCode = strbranch(1)
End If

MyZoneCode = ""
If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" Then
    strbranch = Split(CmbZone.Text, "#")
    MyZoneCode = strbranch(1)
End If

MyRmCode = ""
If CmbRM.Text <> "" And UCase(CmbRM.Text) <> "ALL" Then
    strbranch = Split(CmbRM.Text, "#")
    MyRmCode = strbranch(1)
End If

MyMutCode = ""
If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
    strbranch = Split(CmbAmcTR.Text, "#")
    MyMutCode = strbranch(1)
End If
If chkPMS.Value = 1 Then
    cmdMapPMS.Visible = True
    cmdUnmapPMS.Visible = True
Else
    cmdMapPMS.Visible = False
    cmdUnmapPMS.Visible = False
End If

CmdGo1.Enabled = False
MyTotalAmount2 = 0
MyTotalmargin2 = 0
MyTotalTrans2 = 0
Call gridfill(2)
CmdGo1.Enabled = True
End Sub

'Private Sub CmdMissingReport_Click()
'On Error GoTo err
'Dim cnt As Integer
'cnt = 0
'If VSFCommGrdT.Rows > 1 Then
'    For i = 1 To VSFCommGrdT.Rows - 1
'        Screen.MousePointer = vbHourglass
'        If VSFCommGrdT.TextMatrix(i, 18) <> "" Then
'            If VSFCommGrdT.TextMatrix(i, 18) = 1 Then
'                MyConn.Execute "Update TRANSACTION_MF_TEMP1 set MISSING ='YES' where TRAN_CODE='" & VSFCommGrdT.TextMatrix(i, 10) & "'"
'                MyConn.Execute "insert into temp_mail (tran_code) values('" & VSFCommGrdT.TextMatrix(i, 10) & "')"
'            Else
'                MyConn.Execute "Update TRANSACTION_MF_TEMP1 set MISSING =NULL where TRAN_CODE='" & VSFCommGrdT.TextMatrix(i, 10) & "'"
'                MyConn.Execute "delete from  temp_mail  where tran_code='" & VSFCommGrdT.TextMatrix(i, 10) & "'"
'            End If
'        End If
'        If VSFCommGrdT.TextMatrix(i, 19) <> "" Then
'            If VSFCommGrdT.TextMatrix(i, 19) = 1 Then
'                MyConn.Execute "Update TRANSACTION_MF_TEMP1 set solve ='YES',sol_ref_Ar='" & VSFCommGrdT.TextMatrix(i, 20) & "',sol_date=sysdate where TRAN_CODE='" & VSFCommGrdT.TextMatrix(i, 10) & "'"
'            Else
'                MyConn.Execute "Update TRANSACTION_MF_TEMP1 set solve =NULL,sol_ref_Ar=null,sol_date=null where TRAN_CODE='" & VSFCommGrdT.TextMatrix(i, 10) & "'"
'            End If
'        End If
'        Screen.MousePointer = vbNormal
'    Next
'    '-------------------------MAIL------------------------
'    cnt = SqlRet("select count(*) from transaction_mf_temp1 where tran_code in(select tran_code from temp_mail) and mflag IS NULL ")
'    Call SendMails
'    '-------------------------------------------------
'    MsgBox cnt & " Missing Transactions have been reported to HO. For further follows, write to Operations Group - shashir@bajajcapital.com", vbInformation
'End If
'MyConn.Execute "update transaction_mf_temp1 set mflag='YES' where tran_code in(select tran_code from temp_mail)"
'MyConn.Execute ("delete from  temp_mail ")
'Exit Sub
'err:
'    MsgBox err.Description
'    'resume
'End Sub
Private Sub SendMails()
On Error GoTo Mah
Dim Str() As String
Dim rsType As New ADODB.Recordset
Dim RsSplit As New ADODB.Recordset
Dim count As Integer
Dim Mysql As String
Dim rsRm As New ADODB.Recordset
Dim rcp As String, MailFlg As Boolean
Dim rmvFl As New filesystemobject
count = 0
'l6.Caption = 0
rsType.CursorLocation = adUseClient
RsSplit.CursorLocation = adUseClient
rsType.open "select distinct rm_name,business_rmcode,branch_name from tempmail where tran_code in(select tran_code from temp_mail) and mflag IS NULL ", MyConn, adOpenForwardOnly
While Not rsType.EOF
    MyRmName = rsType.Fields("rm_name")
    MyBusiCode = rsType.Fields("business_rmcode")
    MyBranch = rsType.Fields("branch_name")
    rcp = "shashir@bajajcapital.com"
    'rcp = "pankajpundir@bajajcapital.com"
    Set poSendMail = New clsSendMail
    If poSendMail.IsValidEmailAddress(rcp) = True Then
       MailFlg = True
       With poSendMail
            .SMTPHostValidation = VALIDATE_NONE         ' Optional, default = VALIDATE_HOST_DNS
            .EmailAddressValidation = VALIDATE_SYNTAX   ' Optional, default = VALIDATE_SYNTAX
            .Delimiter = ";"                            ' Optional, default = ";" (semicolon)
            .SMTPHost = "mail.bajajcapital.com"                 ' Required the fist time, optional thereafter
            .From = "wealthmaker@bajajcapital.com"          ' Required the fist time, optional thereafter
            .FromDisplayName = "Wealthmaker"                ' Optional, saved after first use
            .Recipient = rcp    ' Required, separate multiple entries with delimiter character"
            .RecipientDisplayName = rcp ' Optional, separate multiple entries with delimiter character"
            .BccRecipient = "nidhic@bajajcapital.com" ';jijyo@bajajcapital.com
            .CcRecipient = "harshn@bajajcapital.com;harishs@bajajcapital.com;anjaneyakg@bajajcapital.com"
            .Subject = "Missing MF Transactions Reported on " & ServerDateTime & " by " & MyRmName & "( " & MyBusiCode & " ) - " & MyBranch & "" ' Optional
             Call MessageZonal
            .Message = strmsg                             ' Optional
            '.Attachment = App.Path & "\reports\siprenewal.jpg"           ' Optional, separate multiple entries with delimiter character
            .AsHTML = True                           ' Optional, default = FALSE, send mail as html or plain text
            .ContentBase = ""                           ' Optional, default = Null String, reference base for embedded links
            .EncodeType = MyEncodeType                  ' Optional, default = MIME_ENCODE
            .Priority = etPriority                      ' Optional, default = PRIORITY_NORMAL
            .Receipt = False                         ' Optional, default = FALSE
            .UseAuthentication = True             ' Optional, default = FALSE
            .UsePopAuthentication = True           ' Optional, default = FALSE
            .UserName = "troubleticket"                     ' Optional, default = Null String
            .Password = "bajajcap"                     ' Optional, default = Null String, value is NOT saved
            .POP3Host = "mail.bajajcapital.com"
            .MaxRecipients = 100                        ' Optional, default = 100, recipient count before error is raised
            .ConnectTimeout = 100                      ' Optional, default = 10
            .ConnectRetry = 10                         ' Optional, default = 5
            .MessageTimeout = 100                      ' Optional, default = 60
            .SMTPPort = 587                            ' Optional, default = 25
            .send                                       ' Required
            .Disconnect                               ' Optional, use when sending bulk mail
      End With
    End If
    Set poSendMail = Nothing
    count = count + 1
    DoEvents
    rsType.MoveNext
   ' myconn.Execute ("UPDATE temp_mail SET MFLAG='YES' WHERE rm_name='" & MyRmName & "'")
     'MsgBox "Email has been sent to " & MyRmName
Wend
    Exit Sub
Mah:
    PrintToTrace " -- Sip Renewal Mail Error " & err.Description
End Sub
Private Sub MessageZonal()
Dim MyTotalPmt As Long
Dim MyTotalMargin As Long
Dim flage As Boolean
Dim StrMsg1 As String
Dim rsVar1 As New ADODB.Recordset
Dim sql As String, rsVar As New ADODB.Recordset
Dim add2 As String, City As String, pin As String, Folio As String, scheme As String, nav As String
strmsg = "select rownum,a.* from tempmail a,temp_mail b where a.tran_code = b.tran_code  and a.rm_name='" & MyRmName & "' and a.mflag IS NULL "
If rsVar.State = 1 Then rsVar.Close
rsVar.open strmsg, MyConn, adOpenStatic, adLockReadOnly
StrMsg1 = "select sum(amount)sum from tempmail a,temp_mail b where a.tran_code = b.tran_code and a.rm_name='" & MyRmName & "' and a.mflag IS NULL "
If rsVar1.State = 1 Then rsVar1.Close
rsVar1.open StrMsg1, MyConn, adOpenStatic, adLockReadOnly
'---------------------------------------------------------------------------------------------
strmsg = " <HTML> "
strmsg = strmsg & "<HEAD> "
strmsg = strmsg & "<META HTTP-EQUIV=""Content-Type"" CONTENT=""text/html; charset=windows-1252""> "
strmsg = strmsg & "<META NAME=""Generator"" CONTENT=""Microsoft FrontPage 5.0""> "
strmsg = strmsg & "<TITLE>Dear J Kodeeswari </TITLE> "
strmsg = strmsg & "</HEAD> "
strmsg = strmsg & "<BODY> "
strmsg = strmsg & "<FONT FACE=""Arial"" SIZE=4><P align=""center""><B><U>Missing MF Temporary Transaction</B></U></P> "
strmsg = strmsg & "<FONT FACE=""Arial"" SIZE=2><P>Following are the Missing Transactions Reported by  <B><U>" & MyRmName & " ( " & MyBusiCode & " )</B></U> ( <B>" & MyBranch & " )</B></P> "
strmsg = strmsg & "<TABLE BORDER CELLSPACING=0 CELLPADDING=2 VALIGN=CENTER BGCOLOR=FFFFE1 BORDERCOLOR=ECE9D8 > "
strmsg = strmsg & "<tr><TD WIDTH=""1%"" BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>S No.</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%"" BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>TR No.</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""3%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>TR Date</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>PAN</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>Account No.</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>Client Code</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>ANA Code</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>Investor Code</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>Investor</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""10%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>AMC/Scheme</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""3%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>Amount</STRONG></FONT></TD> "
strmsg = strmsg & "<TD WIDTH=""2%""  BGCOLOR=#333399 ALIGN=LEFT><FONT  COLOR=#FFFFE1 size=1 ><STRONG>SIP Type</STRONG></FONT></TD></tr> "

While Not rsVar.EOF
        strmsg = strmsg & "<TR> "
        strmsg = strmsg & "<TD WIDTH=""1%"" BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("rownum") & "</FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%"" BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("tran_code") & "</FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""3%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("tr_date") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & UCase(rsVar.Fields("panno")) & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("ac_holder_code") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & Mid(rsVar.Fields("client_codec"), 1, 8) & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("exist_code") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("client_code") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("investor_name") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""10%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > " & rsVar.Fields("scheme_name") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""3%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("amount") & " </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""2%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 >" & rsVar.Fields("sip_type") & " </FONT></TD> </tr>"
        rsVar.MoveNext
        DoEvents
Wend
        strmsg = strmsg & "<TR> "
        strmsg = strmsg & "<TD WIDTH=""1%"" BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 ></FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""2%"" BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 ></FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""3%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""10%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 ><STRONG><B>TOTAL</B> </STRONG></FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""4%"" BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 ><B>" & rsVar1.Fields("sum") & "</B></FONT></TD> "
        strmsg = strmsg & "<TD WIDTH=""2%""  BGCOLOR=#e1f5ff ALIGN=LEFT><FONT  COLOR=#000000 size=1 > </FONT></TD> </tr>"
        strmsg = strmsg & "</table> "
        strmsg = strmsg & "</BODY> "
        strmsg = strmsg & "</HTML> "
        rsVar.Close
        Set rsVar = Nothing
End Sub

Private Sub CmdMap_Click()
Dim adSendMail As New ADODB.Command
Dim rsCheck As New ADODB.Recordset

If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If
If MyRtaTrCode = "" Then
    MsgBox "First Double Click The Record Of RTA Transaction,To Which Folio You Want To Map ", vbInformation
    Exit Sub
End If
'If Len(MasterId) = 0 Then
'    MsgBox "First Select The Record Of Sip MAster , To Which You Want To Map", vbInformation
'    Exit Sub
'End If
'
'-------------------------------------------------------------------------------------------------
If MyDispatch = "N" Then
    MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
Else
    MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & " ,  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
End If
'--------------------------------------------------------------------------------------------------
'MyConn.Execute ("Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")

        '-------temp send mail
        If Glbloginid = "39339" Then
            MyMailId = "anamikat@bajajcapital.com"
        ElseIf Glbloginid = "112649" Then
            MyMailId = "rajeshb@bajajcapital.com"
        End If

        Set adSendMail.ActiveConnection = MyConn
        adSendMail.CommandType = adCmdStoredProc
        adSendMail.CommandText = "SEND_MAIL"
        
        adSendMail.Parameters.Append adSendMail.CreateParameter("recp", adVarChar, adParamInput, 50, MyMailId)
        adSendMail.Parameters.Append adSendMail.CreateParameter("from_id", adVarChar, adParamInput, 50, "wealthmaker@bajajcapital.com")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg1", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg2", adVarChar, adParamInput, 50, "Base SIP Reconciled")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg3", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("Sub", adVarChar, adParamInput, 50, "Reco Update " & MyTrCode)
        
        'adSendMail.Execute
        Set adSendMail = Nothing
        '-------------------------
        rsCheck.open "select rec_flag from transaction_mf_temp1 WHERE TRAN_CODE='" & MyTrCode & "'", MyConn, adOpenForwardOnly
        MsgBox rsCheck("Rec_flag")
        rsCheck.Close
        Set rsCheck = Nothing

MsgBox "Base SIP Registration Confirmed Sucessfully", vbInformation
End Sub

Private Sub cmdMapPMS_Click()
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If

'If Len(MasterId) = 0 Then
'    MsgBox "First Select The Record Of Sip MAster , To Which You Want To Map", vbInformation
'    Exit Sub
'End If
'
'-------------------------------------------------------------------------------------------------
If MyDispatch = "N" Then
    MyConn.Execute ("Update Transaction_mf_temp1 set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    'MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
Else
    MyConn.Execute ("Update Transaction_mf_temp1 set   REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set   REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    'MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
End If
'--------------------------------------------------------------------------------------------------
'MyConn.Execute ("Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")
MsgBox "Base SIP Registration Confirmed Sucessfully", vbInformation
End Sub


Private Sub CmdMasterReconcile_Click()
Dim adSendMail As New ADODB.Command
Dim rsCheck As New ADODB.Recordset
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If

If Len(MasterId) = 0 Then
    MsgBox "First Select The Record Of Sip MAster , To Which You Want To Map", vbInformation
    Exit Sub
End If
MyConn.Execute ("Update Transaction_mf_temp1 set  SIP_REC_FLAG='Y',SIP_RECO_DATE=TO_DATE(SYSDATE),SIP_REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
MyConn.Execute ("Update TRAN_SIP_FEED SET SIP_REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',SIP_REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")

        '-------temp send mail
        If Glbloginid = "39339" Then
            MyMailId = "anamikat@bajajcapital.com"
        ElseIf Glbloginid = "112649" Then
            MyMailId = "rajeshb@bajajcapital.com"
        End If

        Set adSendMail.ActiveConnection = MyConn
        adSendMail.CommandType = adCmdStoredProc
        adSendMail.CommandText = "SEND_MAIL"
        
        adSendMail.Parameters.Append adSendMail.CreateParameter("recp", adVarChar, adParamInput, 50, MyMailId)
        adSendMail.Parameters.Append adSendMail.CreateParameter("from_id", adVarChar, adParamInput, 50, "wealthmaker@bajajcapital.com")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg1", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg2", adVarChar, adParamInput, 50, "SIP Reconciled")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg3", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("Sub", adVarChar, adParamInput, 50, "Reco Update " & MyTrCode)
        
        'adSendMail.Execute
        Set adSendMail = Nothing
        '-------------------------
        rsCheck.open "select rec_flag from transaction_mf_temp1 WHERE TRAN_CODE='" & MyTrCode & "'", MyConn, adOpenForwardOnly
        MsgBox rsCheck("Rec_flag")
        rsCheck.Close
        Set rsCheck = Nothing
        
MsgBox "SIP Registration Confirmed Sucessfully", vbInformation
End Sub

Private Sub CmdNfoReconcile_Click()
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If
If TxtRemarks.Text = "" Then
    MsgBox "Remarks ", vbInformation
    Exit Sub
End If
MyConn.Execute ("Update  Transaction_mf_temp1 set  remark_reco='" & TxtRemarks.Text & "' , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
MyConn.Execute ("Update  Transaction_mf_temp1 set remark_reco='" & TxtRemarks.Text & "' , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
MsgBox "Record Is Mapped Sucessfully", vbInformation
End Sub

Private Sub cmdReset1_Click()
On Error Resume Next
cmbbranchAr.ListIndex = -1
MyRtaTrCode = ""
CmbOrder.ListIndex = -1
Option1(0).Value = True
If VSFCommGrdS.Rows > 0 Then
    VSFCommGrdS.Clear
    VSFCommGrdS.Row = MySelectedRow
    For KCount_cOL = 0 To VSFCommGrdS.Cols - 1
         VSFCommGrdS.Col = KCount_cOL
          VSFCommGrdS.CellBackColor = vbWhite
          VSFCommGrdS.CellForeColor = vbBlack
          VSFCommGrdS.CellFontBold = False
    Next
End If
CmdMap.Visible = True
CmdNfoReconcile.Visible = False
End Sub

Private Sub cmdReset2_Click()
On Error Resume Next
cmbbranch.ListIndex = 0
CmbOrder1.ListIndex = 0
OptUnReconcileTr.Value = True
LblCount.Caption = ""
Option1(2).Value = True
If VSFCommGrdT.Rows > 0 Then
    VSFCommGrdT.Clear
    VSFCommGrdT.Row = MySelectedRow1
    For KCount_cOL = 0 To VSFCommGrdT.Cols - 1
         VSFCommGrdT.Col = KCount_cOL
          VSFCommGrdT.CellBackColor = vbWhite
          VSFCommGrdT.CellForeColor = vbBlack
          VSFCommGrdT.CellFontBold = False
    Next
End If
MyTrCode = ""
CmdMap.Visible = True
CmdNfoReconcile.Visible = False
End Sub

'Private Sub cmdSave_Click()
'On Error GoTo err
'Dim str() As String
'For DoGrid = 1 To VSFCommGrdS.Rows - 1
'    If VSFCommGrdS.TextMatrix(DoGrid, 10) <> "" And VSFCommGrdS.TextMatrix(DoGrid, 11) <> "" Then
'        If Val(VSFCommGrdS.TextMatrix(DoGrid, 11)) <> 0 Then
'            MyConn.Execute "delete from MFDEALSPECIFIC_PAID where tran_code='" & VSFCommGrdS.TextMatrix(DoGrid, 14) & "'"
'            MyConn.Execute "delete from DEALSPECIFIC_PAID where tran_code='" & VSFCommGrdS.TextMatrix(DoGrid, 14) & "' and flag='YES'"
'            sql = "Insert Into MFDEALSPECIFIC_PAID(tran_code, receivable, expense,margin, RATETYPE, rate, USERID, TIMEST)"
'            sql = sql & " VALUES('" & VSFCommGrdS.TextMatrix(DoGrid, 14) & "'," & Val(VSFCommGrdS.TextMatrix(DoGrid, 9)) & ","
'            sql = sql & " " & Val(VSFCommGrdS.TextMatrix(DoGrid, 12)) & "," & Val(VSFCommGrdS.TextMatrix(DoGrid, 13)) & ","
'            sql = sql & " '" & VSFCommGrdS.TextMatrix(DoGrid, 10) & "'," & Val(VSFCommGrdS.TextMatrix(DoGrid, 11)) & ","
'            sql = sql & " '" & Glbloginid & "',to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/yyyy'))"
'            MyConn.Execute sql
'
'            sql = "INSERT INTO DEALSPECIFIC_PAID (PAID_CAL_ON,DEAL_BROK_TYPE,PER_RS,FIGURE,TRAN_CODE,LOGGEDUSERID,TIMEST,FLAG)"
'            sql = sql & " VALUES('Amount Invested','Upfront','" & VSFCommGrdS.TextMatrix(DoGrid, 10) & "'," & Val(VSFCommGrdS.TextMatrix(DoGrid, 11)) & ",'" & VSFCommGrdS.TextMatrix(DoGrid, 14) & "',"
'            sql = sql & "'" & Glbloginid & "',to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/yyyy'),'YES')"
'            MyConn.Execute sql
'        Else
'            MyConn.Execute "delete from MFDEALSPECIFIC_PAID where tran_code='" & VSFCommGrdS.TextMatrix(DoGrid, 14) & "'"
'            MyConn.Execute "delete from DEALSPECIFIC_PAID where tran_code='" & VSFCommGrdS.TextMatrix(DoGrid, 14) & "' and flag='YES'"
'        End If
'        '*******************FOR BROK RECD_PAID
'        Dim recd_paid As New ADODB.Command
'        Set recd_paid.ActiveConnection = MyConn
'        recd_paid.CommandType = adCmdStoredProc
'        recd_paid.CommandText = "Recd_paid_update"
'        recd_paid.Parameters.Append recd_paid.CreateParameter("tr_code", adVarChar, adParamInput, 50, Trim(VSFCommGrdS.TextMatrix(DoGrid, 14)))
'        recd_paid.Execute
'        '''''******************************end
'    End If
'Next
'MsgBox "The Record Has Been Saved Successfully"
'Exit Sub
'err:
'MsgBox err.Description
'End Sub
Private Sub cmdview_Click()
Call gridfill(2)
End Sub

Private Sub CmdSIPReste_Click()
txtSipAmount.Text = ""
txtSipClientCode.Text = ""
txtSipFoliono.Text = ""
TxtSipPan.Text = ""
If VSFCommGrdS.Rows > 0 Then
    VSFCommGrdS.Clear
    VSFCommGrdS.Row = MySelectedRow
    For KCount_cOL = 0 To VSFCommGrdS.Cols - 1
         VSFCommGrdS.Col = KCount_cOL
          VSFCommGrdS.CellBackColor = vbWhite
          VSFCommGrdS.CellForeColor = vbBlack
          VSFCommGrdS.CellFontBold = False
    Next
End If
End Sub

Private Sub cmdUnmapPMS_Click()
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If

'If Len(MasterId) = 0 Then
'    MsgBox "First Select The Record Of Sip MAster , To Which You Want To Map", vbInformation
'    Exit Sub
'End If
'
'-------------------------------------------------------------------------------------------------
If MyDispatch = "N" Then
    MyConn.Execute ("Update Transaction_mf_temp1 set  REC_FLAG=null,RECO_DATE=null,REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set REC_FLAG=null,RECO_DATE=null,REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    'MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
Else
    MyConn.Execute ("Update Transaction_mf_temp1 set   REC_FLAG=null,RECO_DATE=null,REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set   REC_FLAG=null,RECO_DATE=null,REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    'MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
End If
'--------------------------------------------------------------------------------------------------
'MyConn.Execute ("Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")
MsgBox "Base SIP Registration Confirmed Sucessfully", vbInformation
End Sub

Private Sub Command1_Click()
Call gridfill(3)
End Sub

Private Sub CmdsaveRemaek_Click()
If MyTrCode = "" Then
   MsgBox "Please double click the record,you want to remark", vbInformation
   Exit Sub
End If
MyConn.Execute ("Update  Transaction_mf_temp1 set  remark_reco='" & TxtRemarks.Text & "'   WHERE TRAN_CODE='" & MyTrCode & "'")
MyConn.Execute ("Update  Transaction_mf_temp1 set remark_reco='" & TxtRemarks.Text & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
MsgBox "The record has been remarked sucessfully"
End Sub





Private Sub EmdExit_Click()
Unload Me
End Sub

Private Sub Form_Load()
mskfromdt.Text = Format(Now, "DD/MM/YYYY")
msktodt.Text = Format(Now, "DD/MM/YYYY")
MySelectedRow = -1
mskfromdt1.Text = Format(Now, "DD/MM/YYYY")
msktodt1.Text = Format(Now, "DD/MM/YYYY")
Call SetGrid
Opt1.Value = True
FrmSipMaster.Top = 3720
FrmRta.Top = 6120
Dim RsData As New ADODB.Recordset
CmbOrder.Clear
CmbOrder.AddItem "Select", 0
CmbOrder.AddItem "Investor Name", 1
CmbOrder.AddItem "Scheme Name", 2
CmbOrder.AddItem "Amount", 3
CmbOrder.AddItem "Cheque No", 4
CmbOrder.AddItem "Trans Type", 5
CmbOrder.AddItem "Trans Date", 6
CmbOrder.AddItem "Folio No", 7
CmbOrder.ListIndex = 0

CmbSearch.AddItem "Cheque No", 0
CmbSearch.AddItem "Folio No", 1
CmbSearch.AddItem "App No", 2
CmbSearch.AddItem "PAN No", 3
CmbSearch.AddItem "Broker Code", 4
CmbSearch.ListIndex = 0


OptUnReconcileTr.Value = True
OptUnReconcileAR.Value = True

CmbOrder1.Clear
CmbOrder1.AddItem "Select", 0
CmbOrder1.AddItem "Investor Name", 1
CmbOrder1.AddItem "Scheme Name", 2
CmbOrder1.AddItem "Amount", 3
CmbOrder1.AddItem "Cheque No", 4
CmbOrder1.AddItem "Tran Type", 5
CmbOrder1.AddItem "Tran Date", 6
CmbOrder1.AddItem "RM Name", 7
CmbOrder1.ListIndex = 0
VSFlexGrid1_reset
'If GlbroleId = 17 Or GlbroleId = 1 Then
'CmdMissingReport.Enabled = True
'Else
'    CmdMissingReport.Enabled = False
'End If
If Glbloginid = "38558" Then
    cmdConfirm.Visible = True
Else
    cmdConfirm.Visible = False
End If

Set rs_branch = New ADODB.Recordset
Mysql = ""
Mysql = "select itemname,itemserialnumber from fixeditem where itemtype='26' order by itemname"
If rs_branch.State = 1 Then RS.Close
rs_branch.open Mysql, MyConn
cmbcategory.Clear
Do While Not rs_branch.EOF
        cmbcategory.AddItem rs_branch(0) & Space(80) & "#" & rs_branch(1)
        rs_branch.MoveNext
Loop
rs_branch.Close
Set rs_branch = Nothing
cmbcategory.AddItem "All", 0
cmbcategory.ListIndex = 0

'------------------------------------TR BRANCH FILL---------------------------------------------------------------------
Set rs_branch = New ADODB.Recordset
If rs_branch.State = 1 Then rs_branch.Close
If Trim(Branches) <> "" Then
    rs_branch.open "select branch_name,branch_code from branch_master where branch_code in (" & Branches & ") and category_id not in ('1004','1005','1006') order by branch_name", MyConn, adOpenForwardOnly, adLockReadOnly
Else
    rs_branch.open "select branch_name,branch_code from branch_master and category_id not in ('1004','1005','1006')  order by branch_name", MyConn, adOpenForwardOnly, adLockReadOnly
End If
If rs_branch.RecordCount > 0 Then
    For i = 0 To rs_branch.RecordCount - 1
        cmbbranch.AddItem rs_branch(0) & Space(80) & "#" & rs_branch(1)
        cmbbranchAr.AddItem rs_branch(0) & Space(80) & "#" & rs_branch(1)
        rs_branch.MoveNext
    Next
End If
cmbbranch.AddItem "ALL", 0
cmbbranchAr.AddItem "ALL", 0
cmbbranchAr.ListIndex = 0
rs_branch.Close
Set rs_branch = Nothing

'region fill
Mysql = "select region_name,region_id from region_master where region_id in (select region_id from branch_master where branch_code in (" & Branches & ")) order by region_name"
RsData.open Mysql, MyConn, adOpenForwardOnly
CmbRegion.Clear
Do While Not RsData.EOF
     CmbRegion.AddItem RsData(0) & Space(50 - Len(RsData(0))) & "#" & RsData(1)
     RsData.MoveNext
Loop
RsData.Close
CmbRegion.AddItem "All", 0
CmbRegion.ListIndex = 0
Set RsData = Nothing
    
'AMC fill
Mysql = "select MUT_NAME,MUT_CODE from MUT_FUND order by MUT_name"
RsData.open Mysql, MyConn, adOpenForwardOnly
CmbAmcTR.Clear
Do While Not RsData.EOF
     CmbAmcTR.AddItem RsData(0) & Space(80) & "#" & RsData(1)
     CmbAmcAR.AddItem RsData(0) & Space(80) & "#" & RsData(1)
     RsData.MoveNext
Loop
RsData.Close
CmbAmcTR.AddItem "All", 0
CmbAmcTR.ListIndex = 0
CmbAmcAR.AddItem "All", 0
CmbAmcAR.ListIndex = 0
Set RsData = Nothing
'------------------------------------------------------------------------------------------------------------------------
'------------------------------------AR BRANCH FILL---------------------------------------------------------------------
'Set rs_branch = New ADODB.Recordset
'If rs_branch.State = 1 Then rs_branch.Close
'If Trim(Branches) <> "" Then
'    rs_branch.open "select branch_name,branch_code from branch_master where branch_code in (" & Branches & ") and category_id not in ('1004','1005','1006') order by branch_name", MyConn, adOpenForwardOnly, adLockReadOnly
'Else
'    rs_branch.open "select branch_name,branch_code from branch_master and category_id not in ('1004','1005','1006')  order by branch_name", MyConn, adOpenForwardOnly, adLockReadOnly
'End If
'cmbbranchAr.Clear
'If rs_branch.RecordCount > 0 Then
'    With cmbbranchAr
'        For i = 0 To rs_branch.RecordCount - 1
'            .AddItem rs_branch(0) & Space(80) & "#" & rs_branch(1)
'            rs_branch.MoveNext
'        Next
'    End With
'End If
'cmbbranchAr.AddItem "ALL", 0
'cmbbranchAr.ListIndex = 0
'rs_branch.Close
'Set rs_branch = Nothing

Optcams.Value = True
End Sub

Private Sub VSFlexGrid1_reset()
VSFlexGrid1.Clear
VSFlexGrid1.Cols = 2
VSFlexGrid1.FormatString = "Tran Code|^Branch Name"
VSFlexGrid1.ColWidth(0) = 1000
VSFlexGrid1.ColWidth(1) = 2500
End Sub
Private Sub CmbOrder_Click()
MyOrder = ""
If CmbOrder.ListIndex = 1 Then
   MyOrder = "Investor_Name"
ElseIf CmbOrder.ListIndex = 2 Then
    MyOrder = "Scheme_Name"
ElseIf CmbOrder.ListIndex = 3 Then
    MyOrder = "Amount"
ElseIf CmbOrder.ListIndex = 4 Then
    MyOrder = "Cheque_No"
ElseIf CmbOrder.ListIndex = 5 Then
    MyOrder = "Tran_Type"
ElseIf CmbOrder.ListIndex = 6 Then
    MyOrder = "tr_date"
ElseIf CmbOrder.ListIndex = 7 Then
    MyOrder = "Folio_No"
End If
End Sub
Private Sub CmbOrder1_Click()
MyOrder1 = ""
If CmbOrder1.ListIndex = 1 Then
   MyOrder1 = "Investor_Name"
ElseIf CmbOrder1.ListIndex = 2 Then
    MyOrder1 = "Scheme_Name"
ElseIf CmbOrder1.ListIndex = 3 Then
    MyOrder1 = "Amount"
ElseIf CmbOrder1.ListIndex = 4 Then
    MyOrder1 = "Cheque_no"
ElseIf CmbOrder1.ListIndex = 5 Then
    MyOrder1 = "Tran_Type"
ElseIf CmbOrder1.ListIndex = 6 Then
    MyOrder1 = "tr_date"
ElseIf CmbOrder1.ListIndex = 7 Then
    MyOrder1 = "rm_name"
End If
End Sub

Private Sub SetGrid()
 s = "TranCode|^TrDate|^InvestorName|^Address|^City|^Amc|^Scheme|^Amount|^FolioNo|^ChequeNO|^AppNo|^RM|^Branch|^BrokerCode|^MutCode|^SchCode|^Reg Tran Type|^Unique Key"
 VSFCommGrdS.FormatString = s
 VSFCommGrdS.ColWidth(0) = 1200 'TranCode
 VSFCommGrdS.ColWidth(1) = 1200 'Date
 VSFCommGrdS.ColWidth(2) = 2500 'InvName
 VSFCommGrdS.ColWidth(3) = 2500 'Address
 VSFCommGrdS.ColWidth(4) = 1200 'city
 VSFCommGrdS.ColWidth(5) = 2000 'amc
 VSFCommGrdS.ColWidth(6) = 5000 'scheme
 VSFCommGrdS.ColWidth(7) = 1500 'amt
 VSFCommGrdS.ColWidth(8) = 1200 'fol_no
 VSFCommGrdS.ColWidth(9) = 1500 'chq_no
 VSFCommGrdS.ColWidth(10) = 1500 'app_no
 VSFCommGrdS.ColWidth(11) = 1500 'rm
 VSFCommGrdS.ColWidth(12) = 1500 'branch
 VSFCommGrdS.ColWidth(13) = 1200 'subcode
 VSFCommGrdS.ColWidth(14) = 0 'mutcode
 VSFCommGrdS.ColWidth(15) = 0 'mutcode
 VSFCommGrdS.ColWidth(16) = 1500 'branch
 VSFCommGrdS.ColWidth(17) = 1200 'subcode
  'VSFCommGrdS.ColWidth(16) = 1500 'Trcode
 VSFCommGrdS.ColEditMask(1) = "##/##/####"
 VSFCommGrdS.ColAlignment(0) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(1) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(2) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(3) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(4) = flexAlignRightCenter
 VSFCommGrdS.ColAlignment(5) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(6) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(7) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(8) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(9) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(10) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(11) = flexAlignLeftCenter
 VSFCommGrdS.ColAlignment(12) = flexAlignLeftCenter
 
 
 s1 = "TranCode|^TrDate|^InvestorName|^Address|^City|^Amc|^Scheme|^Amount|^FolioNo|^ChequeNO|^AppNo|^RM|^Branch|^MutCode|^SchCode|^Sip Amount|^Total Sip|^Status|^Flag"
 VSFCommGrdT.FormatString = s1
 VSFCommGrdT.ColWidth(0) = 1200 'TranCode
 VSFCommGrdT.ColWidth(1) = 1200 'Date
 VSFCommGrdT.ColWidth(2) = 2500 'InvName
 VSFCommGrdT.ColWidth(3) = 2500 'Address
 VSFCommGrdT.ColWidth(4) = 1200 'city
 VSFCommGrdT.ColWidth(5) = 2000 'amc
 VSFCommGrdT.ColWidth(6) = 5000 'scheme
 VSFCommGrdT.ColWidth(7) = 1500 'amt
 VSFCommGrdT.ColWidth(8) = 1200 'fol_no
 VSFCommGrdT.ColWidth(9) = 1500 'chq_no
 VSFCommGrdT.ColWidth(10) = 1500 'app_no
 VSFCommGrdT.ColWidth(11) = 1500 'rm
 VSFCommGrdT.ColWidth(12) = 1500 'branch
 VSFCommGrdT.ColWidth(13) = 0 'mutcode
 VSFCommGrdT.ColWidth(14) = 0 'schcode
 VSFCommGrdT.ColWidth(15) = 1500 'Sip Amount
 VSFCommGrdT.ColWidth(16) = 1000 'Total Sip
 VSFCommGrdT.ColWidth(17) = 2000 'Status
 VSFCommGrdT.ColWidth(18) = 1500 'Online Flag
 VSFCommGrdT.ColEditMask(1) = "##/##/####"
 VSFCommGrdT.ColAlignment(0) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(1) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(2) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(3) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(4) = flexAlignRightCenter
 VSFCommGrdT.ColAlignment(5) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(6) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(7) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(8) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(9) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(10) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(11) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(12) = flexAlignLeftCenter
 VSFCommGrdT.ColAlignment(15) = flexAlignLeftCenter
 
 s2 = "SEQ_NO|^Amc|^Scheme|^Amount|^FolioNo|^Sip_Start_Date|^Sip_End_Date|^SipRegDate|^Sip_Option|^Total Sip|^RM|^Branch|^MutCode|^SchCode|^Payroll_Id|^Branch_Code"
 VSFCommGrdK.FormatString = s2
 VSFCommGrdK.ColWidth(0) = 1200 'Seq_No
 VSFCommGrdK.ColWidth(1) = 2000 'amc
 VSFCommGrdK.ColWidth(2) = 5000 'scheme
 VSFCommGrdK.ColWidth(3) = 1500 'amt
 VSFCommGrdK.ColWidth(4) = 1200 'fol_no
 VSFCommGrdK.ColWidth(5) = 1300 'StartDate
 VSFCommGrdK.ColWidth(6) = 1300 'EndDate
 VSFCommGrdK.ColWidth(7) = 1300 'SipRegDate
 VSFCommGrdK.ColWidth(8) = 1200 'SipOption
 VSFCommGrdK.ColWidth(9) = 800 'TotalSip
 VSFCommGrdK.ColWidth(10) = 1500 'rm
 VSFCommGrdK.ColWidth(11) = 1500 'branch
 VSFCommGrdK.ColWidth(12) = 0 'mutcode
 VSFCommGrdK.ColWidth(13) = 0 'schcode
 VSFCommGrdK.ColWidth(14) = 0 'Payroll_id
 VSFCommGrdK.ColWidth(15) = 0 'Branch_Code
 VSFCommGrdK.ColEditMask(5) = "##/##/####" 'StartDate
 VSFCommGrdK.ColEditMask(6) = "##/##/####" 'EndDate
 VSFCommGrdK.ColEditMask(7) = "##/##/####" 'SipregDate
 
 VSFCommGrdK.ColAlignment(0) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(1) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(2) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(3) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(4) = flexAlignRightCenter
 VSFCommGrdK.ColAlignment(5) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(6) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(7) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(8) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(9) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(10) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(11) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(12) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(13) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(14) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(15) = flexAlignLeftCenter
End Sub


Private Sub mskfromdt1_LostFocus()
mskfromdt.Text = mskfromdt1.Text
End Sub


Private Sub msktodt1_LostFocus()
msktodt.Text = msktodt1.Text
End Sub

Private Sub Opt1_Click()
FrmSipMaster.Top = 3720
FrmRta.Top = 6120
End Sub

Private Sub Opt2_Click()
FrmSipMaster.Top = 7200
FrmRta.Top = 3720
End Sub


Private Sub txtfindtran_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub TxtRemarks_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub



Private Sub VSFCommGrdK_Click()
ColumnIndex2 = VSFCommGrdK.Col
Clear_Previously_Selected2
Glb_L_SearchIndex2 = 1
Glb_Selected_row2 = 0
Label6(0).Caption = "" & VSFCommGrdK.TextMatrix(0, VSFCommGrdK.Col)

MyCurrRowk = VSFCommGrdK.Row
If VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 0) <> "" Then
      If MySelectedRow > 0 Then
          VSFCommGrdK.Row = MySelectedRow
          For KCount_cOL = 0 To VSFCommGrdK.Cols - 1
               VSFCommGrdK.Col = KCount_cOL
                VSFCommGrdK.CellBackColor = vbWhite
                VSFCommGrdK.CellForeColor = vbBlack
                VSFCommGrdK.CellFontBold = False
          Next
      End If
      VSFCommGrdK.Row = MyCurrRowk
      For KCount_cOL = 0 To VSFCommGrdK.Cols - 1
        VSFCommGrdK.Col = KCount_cOL
        If VSFCommGrdK.CellBackColor = vbBlue Then
            VSFCommGrdK.CellBackColor = vbWhite
            VSFCommGrdK.CellForeColor = vbBlack
            VSFCommGrdK.CellFontBold = False
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdK.CellForeColor = vbWhite
            End If
        Else
            VSFCommGrdK.CellBackColor = vbBlue
            VSFCommGrdK.CellForeColor = vbWhite
            MySelectedRow = VSFCommGrdK.Row
            VSFCommGrdK.CellFontBold = True
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdK.CellForeColor = vbBlue
            End If
        End If
        If KCount_cOL = 18 Then
            If VSFCommGrdK.CellPicture = Image1.Picture Then
                Exit Sub
            End If
            If VSFCommGrdK.CellPicture = Image2.Picture Then
                Set VSFCommGrdK.CellPicture = Image22.Picture
                VSFCommGrdK.CellPictureAlignment = flexPicAlignCenterCenter
                VSFCommGrdK.Text = 1
            Else
                 Set VSFCommGrdK.CellPicture = Image2.Picture
                 VSFCommGrdK.CellPictureAlignment = flexPicAlignCenterCenter
                 VSFCommGrdK.Text = 0
            End If
        End If
    Next
End If
MySipFolio = VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 4)
MasterId = VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 0)
End Sub

Private Sub VSFCommGrdS_Click()
On Error Resume Next
ColumnIndex = VSFCommGrdS.Col
Clear_Previously_Selected1
Glb_L_SearchIndex1 = 1
Glb_Selected_row1 = 0
Label6(0).Caption = "" & VSFCommGrdS.TextMatrix(0, VSFCommGrdS.Col)

MyCurrRow = VSFCommGrdS.Row
If VSFCommGrdS.TextMatrix(VSFCommGrdS.Row, 0) <> "" Then
      If MySelectedRow > 0 Then
          VSFCommGrdS.Row = MySelectedRow
          For KCount_cOL = 0 To VSFCommGrdS.Cols - 1
               VSFCommGrdS.Col = KCount_cOL
                VSFCommGrdS.CellBackColor = vbWhite
                VSFCommGrdS.CellForeColor = vbBlack
                VSFCommGrdS.CellFontBold = False
          Next
      End If
      VSFCommGrdS.Row = MyCurrRow
      For KCount_cOL = 0 To VSFCommGrdS.Cols - 1
        VSFCommGrdS.Col = KCount_cOL
        If VSFCommGrdS.CellBackColor = vbBlue Then
            VSFCommGrdS.CellBackColor = vbWhite
            VSFCommGrdS.CellForeColor = vbBlack
            VSFCommGrdS.CellFontBold = False
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdS.CellForeColor = vbWhite
            End If
        Else
            VSFCommGrdS.CellBackColor = vbBlue
            VSFCommGrdS.CellForeColor = vbWhite
            MySelectedRow = VSFCommGrdS.Row
            'VSFCommGrdS.TextMatrix(VSFCommGrdS.Row, 16) = MyTrCode
            VSFCommGrdS.CellFontBold = True
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdS.CellForeColor = vbBlue
            End If
        End If
        If KCount_cOL = 18 Then
            If VSFCommGrdS.CellPicture = Image1.Picture Then
                Exit Sub
            End If
            If VSFCommGrdS.CellPicture = Image2.Picture Then
                Set VSFCommGrdS.CellPicture = Image22.Picture
                VSFCommGrdS.CellPictureAlignment = flexPicAlignCenterCenter
                VSFCommGrdS.Text = 1
            Else
                 Set VSFCommGrdS.CellPicture = Image2.Picture
                 VSFCommGrdS.CellPictureAlignment = flexPicAlignCenterCenter
                 VSFCommGrdS.Text = 0
            End If
        End If
    Next
End If
MyRtaTrCode = ""
MyRtaTrDate = ""
MyRtaTrCode = VSFCommGrdS.TextMatrix(VSFCommGrdS.Row, 0)
MyRtaTrDate = VSFCommGrdS.TextMatrix(VSFCommGrdS.Row, 1)
MyRtaAmount = Val(VSFCommGrdS.TextMatrix(VSFCommGrdS.Row, 7))
If Opt2.Value = True Then
    txtSipFoliono.Text = VSFCommGrdS.TextMatrix(VSFCommGrdS.Row, 8)
End If
End Sub




Private Sub VSFCommGrdT_AfterScroll(ByVal OldTopRow As Long, ByVal OldLeftCol As Long, ByVal NewTopRow As Long, ByVal NewLeftCol As Long)
    VSFCommGrdS.TopRow = VSFCommGrdT.TopRow
    VSFCommGrdS.LeftCol = VSFCommGrdT.LeftCol
End Sub
Private Sub VSFCommGrdT_DblClick()
MyCurrRow1 = VSFCommGrdT.Row
If VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 0) <> "" Then
      If MySelectedRow1 > 0 Then
          VSFCommGrdT.Row = MySelectedRow1
          For KCount_cOL = 0 To VSFCommGrdT.Cols - 1
               VSFCommGrdT.Col = KCount_cOL
                VSFCommGrdT.CellBackColor = vbWhite
                VSFCommGrdT.CellForeColor = vbBlack
                VSFCommGrdT.CellFontBold = False
          Next
      End If
      VSFCommGrdT.Row = MyCurrRow1
      For KCount_cOL = 0 To VSFCommGrdT.Cols - 1
        VSFCommGrdT.Col = KCount_cOL
        If VSFCommGrdT.CellBackColor = vbBlue Then
            VSFCommGrdT.CellBackColor = vbWhite
            VSFCommGrdT.CellForeColor = vbBlack
            VSFCommGrdT.CellFontBold = False
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdT.CellForeColor = vbWhite
            End If
        Else
            VSFCommGrdT.CellBackColor = vbBlue
            VSFCommGrdT.CellForeColor = vbWhite
            VSFCommGrdT.CellFontBold = True
            MySelectedRow1 = VSFCommGrdT.Row
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdT.CellForeColor = vbBlue
            End If
        End If
'        If KCount_cOL = 18 Then
'            If VSFCommGrdT.CellPicture = Image1.Picture Then
'                Exit Sub
'            End If
'            If VSFCommGrdT.CellPicture = Image2.Picture Then
'                Set VSFCommGrdT.CellPicture = Image22.Picture
'                VSFCommGrdT.CellPictureAlignment = flexPicAlignCenterCenter
'                VSFCommGrdT.Text = 1
'            Else
'                 Set VSFCommGrdT.CellPicture = Image2.Picture
'                 VSFCommGrdT.CellPictureAlignment = flexPicAlignCenterCenter
'                 VSFCommGrdT.Text = 0
'            End If
'        End If
    Next
'End If
MyAppNo = ""
MyChqNo = ""
MyPan = ""
MyAmount = 0
MyTrCode = ""
MyBroker = 0
TxtInvestorName.Text = ""
MyDispatch = ""
MyTrCode = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 0)
TxtInvestorName.Text = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 2)
MyTrDate = CDate(Format(VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 1), "dd/mm/yyyy"))
If IsNull(MyTrDate) Then
Else
    mskfromdt.Text = MyTrDate - 30
    msktodt.Text = MyTrDate + 30
End If
TxtAmount.Text = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 7)
TxtSearch.Text = VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, 9)
If MyTrCode <> "" Then
    MyConn.Execute ("Update  Transaction_mf_temp1 set PROCESSED=nvl(PROCESSED,0)+1 where tran_code='" & MyTrCode & "' ")
End If
Dim MyRs As New ADODB.Recordset
cmdConfirm.Enabled = False
MyRs.open "select pms_identify('" & MyTrCode & "') from dual", MyConn, adOpenForwardOnly
If MyRs(0) = "1" Then
    cmdConfirm.Enabled = True
    CmdMap.Enabled = False
Else
    cmdConfirm.Enabled = False
    CmdMap.Enabled = True
End If
MyRs.Close

MyRs.open "Select client_code,Sip_Start_Date,App_no,CHEQUE_NO,panno,folio_no,amount,remark_reco,source_code,DISPATCH,SCH_CODE,Sip_Amount,NVL(cob_flag,0)cob_flag,WEALTHMAKER.FN_IDENTIFY_REGISTRAR@MF.BAJAJCAPITAL(mut_code) registrar,mut_code from  Transaction_mf_temp1 where tran_code='" & MyTrCode & "'", MyConn, adOpenForwardOnly, adLockReadOnly
If Not MyRs.EOF Then
    MyAppNo = IIf(IsNull(MyRs.Fields("app_no")), "", MyRs.Fields("app_no"))
    MyChqNo = IIf(IsNull(MyRs.Fields("CHEQUE_NO")), "", MyRs.Fields("CHEQUE_NO"))
    MyPan = IIf(IsNull(MyRs.Fields("panno")), "", MyRs.Fields("panno"))
    MyAmount = IIf(IsNull(MyRs.Fields("Amount")), 0, MyRs.Fields("Amount"))
    MySIPAmount = IIf(IsNull(MyRs.Fields("Sip_Amount")), 0, MyRs.Fields("Sip_Amount"))
    If Opt1.Value = True Then
        TxtSipPan.Text = MyPan
        txtSipClientCode.Text = MyRs.Fields("client_code")
        txtSipFoliono.Text = IIf(IsNull(MyRs.Fields("folio_NO")), "", MyRs.Fields("folio_NO"))
        txtSipAmount.Text = MySIPAmount
    End If
    If Opt2.Value = True Then
        txtSipAmount.Text = MyAmount
    End If
    MyBroker = SqlRet("select exist_code from agent_master where agent_code='" & MyRs.Fields("source_code") & "'")
    MyDispatch = IIf(IsNull(MyRs.Fields("DISPATCH")), "", MyRs.Fields("DISPATCH"))
    MaskSipEdBox.Text = IIf(IsNull(MyRs.Fields("sip_start_date")), "__/__/____", MyRs.Fields("sip_start_date"))
    MyProdCode = ""
    TxtRemarks.Text = ""
    TxtRemarks.Text = IIf(IsNull(MyRs.Fields("REMARK_RECO")), "", MyRs.Fields("REMARK_RECO"))
    If MyRs.Fields("registrar") = "C" And MyRs.Fields("cob_flag") = "0" Then
        Optcams.Value = True
    ElseIf MyRs.Fields("registrar") = "K" And MyRs.Fields("cob_flag") = "0" Then
        OptKarvy.Value = True
    ElseIf MyRs.Fields("registrar") = "C" And MyRs.Fields("cob_flag") = "1" Then
        OptCamsCOB.Value = True
    ElseIf MyRs.Fields("registrar") = "K" And MyRs.Fields("cob_flag") = "1" Then
        OptKarvyCOB.Value = True
    End If
Else
    TxtRemarks.Text = ""
    MyDispatch = ""
End If
CmbSearch.ListIndex = 0
MyRs.Close
End If
End Sub

Private Sub cmbcategory_Click()
Dim Rs_Cat As New ADODB.Recordset
Dim StrSql As String
Dim Mycat1() As String
If cmbcategory.Text <> "" And cmbcategory.ListIndex <> -1 Then
    Mycat1 = Split(cmbcategory.Text, "#")
End If
'----branch------
StrSql = ""
StrSql = "select branch_name,branch_code from branch_master where 1=1 "
    If GlbDataFilter <> "72" Then StrSql = StrSql & " and branch_code in (" & Branches & ")"
    If SRmCode <> "" Then StrSql = StrSql & " and BRANCH_CODE=(SELECT SOURCE FROM EMPLOYEE_MASTER WHERE RM_CODE='" & SRmCode & "')"
    If cmbcategory.Text <> "All" Then StrSql = StrSql & " and branch_tar_cat='" & Trim(Mycat1(1)) & "'"
    If CmbRegion.Text <> "All" Then StrSql = StrSql & " and region_id='" & Trim(Right(CmbRegion.Text, 3)) & "'"
    If UCase(Trim(CmbZone.Text)) <> "ALL" And CmbZone.ListIndex <> -1 Then StrSql = StrSql & " and zone_id='" & Trim(Right(CmbZone.Text, 3)) & "'"
    StrSql = StrSql & " and category_id not in ('1004','1005','1006') order by branch_name"
    Rs_Cat.open StrSql, MyConn
    cmbbranch.Clear
    cmbbranchAr.Clear
    Do While Not Rs_Cat.EOF
        cmbbranch.AddItem Rs_Cat(0) & Space(80) & "#" & Rs_Cat(1)
        cmbbranchAr.AddItem Rs_Cat(0) & Space(80) & "#" & Rs_Cat(1)
        Rs_Cat.MoveNext
    Loop
    Rs_Cat.Close
    Set Rs_Cat = Nothing
    cmbbranch.AddItem "All", 0
    cmbbranch.ListIndex = 0
    cmbbranchAr.AddItem "All", 0
    cmbbranchAr.ListIndex = 0
End Sub






