Dim RS As New ADODB.Recordset
Public currentForm As Form
Public treeName As String
Dim Req() As String
Private Sub cmdExcel_Click()
    FlexGrid_To_Excel MSFlexGrid1, MSFlexGrid1.Rows, MSFlexGrid1.Cols
End Sub
Private Sub cmdExit_Click()
    Unload Me
End Sub

Private Sub CmdOkG_Click()
FrmTransactionLog.Visible = False
End Sub

Public Sub cmdSearch_Click()
On Error Resume Next
Dim StrSql As String
Dim RS1 As New ADODB.Recordset
Dim strSearch As String
Dim AppNo As String, ARNO As String, chequeno As String, marno As String

MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM CD|BUSI_CD|B_CD|REM|PLAN_NO|PLAN_DT|DOC"
MSFlexGrid1.ColWidth(21) = 0
MSFlexGrid1.ColWidth(22) = 0
MSFlexGrid1.ColWidth(23) = 0
MSFlexGrid1.ColWidth(24) = 0
MSFlexGrid1.ColWidth(25) = 0
MSFlexGrid1.ColWidth(26) = 0
MSFlexGrid1.ColWidth(27) = 0
MSFlexGrid1.ColWidth(31) = 1000
    
    StrSql = ""
    strSearch = ConvertString(UCase(Trim(txtScheme.Text)))
    If optAfter.Value = True Then
        If GlbDataFilter = "72" Then
            StrSql = "select * from transaction_st where "
        Else
            StrSql = "select * from transaction_st where (flag<>'NEWTRAN' OR FLAG IS NULL) and "
        End If
    Else
        StrSql = "select * from transaction_sttemp where "
    End If
    If IsDate(mskfromdate.Text) = False And mskfromdate.Text <> "__/__/____" Then
        MsgBox "Please Enter Valid From Date", vbInformation
        mskfromdate = "__/__/____"
        mskfromdate.SetFocus
        Exit Sub
    End If
    If IsDate(msktodate.Text) = False And msktodate.Text <> "__/__/____" Then
        MsgBox "Please Enter Valid To Date", vbInformation
        msktodate = "__/__/____"
        msktodate.SetFocus
        Exit Sub
    End If
    If IsDate(mskfromdate.Text) = True And IsDate(mskfromdate.Text) = True And mskfromdate.Text <> "__/__/____" And msktodate.Text <> "__/__/____" Then
        If CDate(Format(mskfromdate.Text, "DD/MM/YYYY")) > CDate(Format(msktodate.Text, "DD/MM/YYYY")) Then
            MsgBox "From Date should be less then To Date", vbInformation
            mskfromdate = "__/__/____"
            msktodate = "__/__/____"
            mskfromdate.SetFocus
            Exit Sub
        End If
    End If
    If Trim(txtScheme.Text) = "" And Trim(txtappno.Text) = "" And Trim(txtarno.Text) = "" And Trim(txtchequeno.Text) = "" And txtmarno.Text = "" And mskfromdate.Text = "__/__/____" And msktodate.Text = "__/__/____" And Trim(txtInvName.Text) = "" And Trim(txtANACD.Text) = "" Then
        MsgBox "Please Enter Searching Parameter", vbInformation
        Exit Sub
    End If
    AppNo = ""
    cmdsearch.Enabled = False
    prgBar.Visible = True
    prgBar.Min = 0
    prgBar.Value = 0
    If txtappno.Text <> "" Then
        AppNo = Trim(txtappno.Text)
        StrSql = StrSql & "App_no='" & AppNo & "' and "
    End If
    If txtarno.Text <> "" Then
        ARNO = Trim(txtarno.Text)
        StrSql = StrSql & " tran_code = '" & ARNO & "' and "

    End If
    If txtchequeno.Text <> "" Then
        chequeno = Trim(txtchequeno.Text)
        StrSql = StrSql & "cheque_no LIKE '%" & chequeno & "%' and "
    End If
    If txtmarno.Text <> "" Then
        marno = Trim(txtmarno.Text)
        StrSql = StrSql & "manual_arno='" & marno & "' and "
    End If
    
    If EPF = True Then
        If frmPayment.Opt_client.Value = True Then
            StrSql = StrSql & " substr(Client_code,1,1)='4' and "
        Else
            StrSql = StrSql & " substr(Client_code,1,1)='3' and "
        End If
    End If
     '----------------NPS Tranactions-----------------------------------
    If currentForm.Name = "frmNPS" Then
        StrSql = StrSql & " sch_code in('OP#09971','OP#09972','OP#09973')  and "
    End If
    If currentForm.Name = "frmpaymentreced" Then
        StrSql = StrSql & " mut_code in(select iss_code from iss_master where prod_code='DT028')  and "
    End If
    '-------------------------------------------------------------------
    If mskfromdate.Text <> "__/__/____" And msktodate.Text <> "__/__/____" Then
        StrSql = StrSql & "tr_date >= to_date('" & mskfromdate.Text & "', 'dd-MM-yyyy') and "
        StrSql = StrSql & "tr_date <= to_date('" & msktodate.Text & "', 'dd-MM-yyyy') and "
    ElseIf mskfromdate.Text <> "__/__/____" And msktodate.Text = "__/__/____" Then
        StrSql = StrSql & "tr_date = to_date('" & mskfromdate.Text & "', 'dd-MM-yyyy') and "
    ElseIf msktodate.Text <> "__/__/____" And mskfromdate.Text = "__/__/____" Then
        StrSql = StrSql & "tr_date = to_date('" & msktodate.Text & "', 'dd-MM-yyyy') and "
    End If
    If RS.State = 1 Then RS.Close
    MSFlexGrid1.Clear
    'MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM_CD|BUSI_CD|B_CD|REM"
    MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM CD|BUSI_CD|B_CD|REM|PLAN_NO|PLAN_DT"
    MSFlexGrid1.ColWidth(21) = 0
    MSFlexGrid1.ColWidth(22) = 0
    MSFlexGrid1.ColWidth(23) = 0
    MSFlexGrid1.ColWidth(24) = 0
    MSFlexGrid1.ColWidth(25) = 0
    MSFlexGrid1.ColWidth(26) = 0
    MSFlexGrid1.ColWidth(27) = 0
    
    If txtScheme.Text <> "" Then
        StrSql = StrSql & " (sch_code in (select sch_code from scheme_info,MUT_FUND where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) like '%" & strSearch & "%') or sch_code in (select osch_code from other_product o, product_master p,iss_master i where o.prod_class_code=p.prod_code and o.iss_code=i.iss_code AND UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) like '%" & strSearch & "%')) AND "
    End If
    If txtInvName.Text <> "" Then
        StrSql = StrSql & " (client_code in (select distinct inv_code from investor_master where upper(investor_name) like '%" & Trim(UCase(txtInvName.Text)) & "%')) AND "
    End If
    If txtANACD.Text <> "" Then
        StrSql = StrSql & " (source_code in (select agent_code from agent_master where trim(exist_code) ='" & Trim(txtANACD.Text) & "')) AND "
    End If
    If SRmCode = "" Then
        StrSql = StrSql & " Branch_code in (" & Branches & ") order by tr_date"
    Else
        StrSql = StrSql & " Branch_code in (" & Branches & ") and RMCODE IN (" & SRmCode & ") order by tr_date"
    End If
    RS.open StrSql, MyConn, adOpenKeyset
    If RS.RecordCount <> 0 Then
        prgBar.Max = RS.RecordCount
    End If
    Label8.Caption = RS.RecordCount
    DoEvents
    MSFlexGrid1.Rows = 2
    MSFlexGrid1.FixedRows = 1
    MSFlexGrid1.FixedCols = 0
    MSFlexGrid1.Rows = 1
    MSFlexGrid1.AllowUserResizing = flexResizeBoth
    With MSFlexGrid1
        i = 0
        While Not RS.EOF
            prgBar.Value = prgBar.Value + 1
            MSFlexGrid1.Rows = MSFlexGrid1.Rows + 1
                .TextMatrix(i + 1, 0) = RS.Fields("tran_code")
                .TextMatrix(i + 1, 1) = Format(RS("tr_date"), "dd/mm/yyyy")
            clcode = RS.Fields("client_code")
            MSFlexGrid1.TextMatrix(i + 1, 23) = clcode
            MSFlexGrid1.TextMatrix(i + 1, 26) = RS("BUSINESS_RMCODE")
            If clcode <> "" Then
                If RS1.State = 1 Then RS1.Close
                strsql1 = "select investor_name from investor_master where inv_code='" & clcode & "'"
                RS1.open strsql1, MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 2) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            Else
                .TextMatrix(i + 1, 2) = ""
                RS1.Close
                Set RS1 = Nothing
            End If
            mtcode = RS.Fields("mut_code")
            MSFlexGrid1.TextMatrix(i + 1, 21) = mtcode
            If mtcode <> "" And Left(mtcode, 2) = "MF" And Left(mtcode, 4) <> "MFIS" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select mut_name from mut_fund where mut_code='" & mtcode & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 3) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            ''End If
            ''If mtcode <> "" And Left(mtcode, 2) <> "MF" Then
            Else
                If RS1.State = 1 Then RS1.Close
                RS1.open "select iss_name from iss_master where iss_code='" & mtcode & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 3) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
            SCHCODE = RS.Fields("sch_code")
            MSFlexGrid1.TextMatrix(i + 1, 22) = SCHCODE
            If SCHCODE <> "" And mtcode <> "" And (Left(mtcode, 2) = "IS" Or Left(mtcode, 4) = "MFIS") Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select longname||'-'||osch_name from other_product where osch_code='" & SCHCODE & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 4) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            Else
            End If
            If SCHCODE <> "" And Left(mtcode, 2) = "MF" And Left(mtcode, 4) <> "MFIS" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select SHORT_NAME from scheme_info where sch_code='" & SCHCODE & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 4) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
            branchcode = RS.Fields("branch_code")
            If branchcode <> "" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select branch_name from branch_master where branch_code='" & branchcode & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 5) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
                .TextMatrix(i + 1, 6) = RS.Fields("app_no")
                .TextMatrix(i + 1, 7) = Format(RS.Fields("app_date"), "dd/mm/yyyy")
                .TextMatrix(i + 1, 8) = RS.Fields("folio_no")
                .TextMatrix(i + 1, 9) = RS.Fields("tran_type")
                .TextMatrix(i + 1, 10) = RS.Fields("amount")
                .TextMatrix(i + 1, 11) = RS.Fields("units")
                .TextMatrix(i + 1, 12) = RS.Fields("rate")
                .TextMatrix(i + 1, 13) = Format(RS.Fields("nav_date"), "dd/mm/yyyy")
                .TextMatrix(i + 1, 14) = RS.Fields("payment_mode")
                .TextMatrix(i + 1, 15) = RS.Fields("cheque_no")
                .TextMatrix(i + 1, 16) = Format(RS.Fields("cheque_date"), "dd/mm/yyyy")
                .TextMatrix(i + 1, 17) = RS.Fields("bank_name")
            brokid = RS.Fields("broker_id")
            If brokid <> "" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select name from broker_master where broker_id='" & brokid & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 18) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
            rmcode1 = RS.Fields("rmcode")
            .TextMatrix(i + 1, 25) = RS.Fields("rmcode")
            
            .TextMatrix(i + 1, 29) = RS.Fields("CROR_PLANNO")
            .TextMatrix(i + 1, 30) = RS.Fields("plan_type")
            .TextMatrix(i + 1, 31) = RS.Fields("DOC_ID")
            
            If rmcode1 <> "" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select rm_name from employee_master where rm_code='" & rmcode1 & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 19) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
                .TextMatrix(i + 1, 20) = RS.Fields("manual_arno")
                .TextMatrix(i + 1, 27) = RS.Fields("busi_branch_code")
                .TextMatrix(i + 1, 28) = RS.Fields("Remark")
                
                .TextMatrix(i + 1, 24) = RS.Fields("broker_id")
                If .TextMatrix(i + 1, 9) = "REVERTAL" Then
                    .Row = i + 1
                    .Col = 0
                    .CellBackColor = vbYellow
                End If
            RS.MoveNext
            i = i + 1
            DoEvents
        Wend
    End With
    MSFlexGrid1.SetFocus
    Label7.Visible = True
    Label8.Caption = MSFlexGrid1.Rows - 1
    cmdsearch.Enabled = True
    prgBar.Visible = False
End Sub

Private Sub CmdViewLog_Click()
Dim sql As String
sql = ""
If txtarno.Text = "" Then
   MsgBox "Please Enter The Tran Code First To See The Log", vbInformation
   Exit Sub
End If
sql = "SELECT * FROM VIEWTRANSACTION_ST_STTEMP_LOG where tran_code='" & txtarno & "'"
Populate_Data sql
FrmTransactionLog.Visible = True
set_grid

End Sub


Private Sub Populate_Data(strQuery As String)
On Error GoTo err1
    MSRClient.sql = ""
    MSRClient.sql = strQuery
    MSRClient.Refresh
    Exit Sub
err1:
    'MsgBox Err.Description
End Sub
Private Sub Form_Load()
    Me.Icon = LoadPicture(App.Path & "\W.ICO")
    MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM CD|BUSI_CD|B_CD|REM|PLAN_NO|PLAN_DT"
    MSFlexGrid1.ColWidth(21) = 0
    MSFlexGrid1.ColWidth(22) = 0
    MSFlexGrid1.ColWidth(23) = 0
    MSFlexGrid1.ColWidth(24) = 0
    MSFlexGrid1.ColWidth(25) = 0
    MSFlexGrid1.ColWidth(26) = 0
    MSFlexGrid1.ColWidth(27) = 0
    If currentForm.Name = "FrmtransactionNew" Then
       frmcmdviewlog.Visible = True
       MSRClient.DataSourceName = "TEST"
       MSRClient.UserName = DataBaseUser
       MSRClient.Password = DataBasePassword
    Else
       frmcmdviewlog.Visible = False
    End If
End Sub


Public Sub MSFlexGrid1_DblClick()
On Error Resume Next
MyFlagMf = True
Dim RowSel As Long
Dim rsGet As New ADODB.Recordset
Dim Issuer(1) As String
Dim busi_code As String
Dim rs_Prod As New ADODB.Recordset

busi_code = ""
If MSFlexGrid1.Rows > 1 Then
    Me.MousePointer = 11
    RowSel = MSFlexGrid1.Row
If strForm = "frmreceivable" Then
frmreceivable.txtARcode.Text = ""
frmreceivable.txtinvestor.Text = ""
frmreceivable.txtcompany.Text = ""
frmreceivable.txtScheme.Text = ""
frmreceivable.TxtAmount.Text = ""

''---ADDED FOR ALL REAL STATE
''Set rs_Prod = myconn.Execute("select prod_class_code from other_product where osch_code='" & MSFlexGrid1.TextMatrix(RowSel, 22) & "'")
''If rs_Prod.EOF = False Then
''    If rs_Prod("prod_class_code") <> "DT028" Then
''    MsgBox "You can not enter receivable for product other then Real Estate.", vbInformation, "Wealth maker"
''    Me.MousePointer = 0
''    Exit Sub
''    ''Unload Me
''    Else
''    ''Unload Me
''    ''Me.MousePointer = 0
''    strForm = ""
''    ''Exit Sub
''    End If
''Else
''MsgBox "You can not enter receivable for product other then Real Estate.", vbInformation, "Wealth maker"
''Me.MousePointer = 0
''strForm = ""
''Exit Sub
''End If
''---END OF REAL STATE

''set rs_prod=myconn.Execute (select prod_class_code from other_product where
frmreceivable.grdDealRecd.Clear
frmreceivable.grdDealRecd.AllowUserResizing = flexResizeColumns
frmreceivable.grdDealRecd.Rows = 1
frmreceivable.grdDealRecd.Cols = 6
frmreceivable.grdDealRecd.FormatString = "  Type   | %/Rs   | Fee Rate  | Trail Input ||"
For i = 0 To frmreceivable.grdDealRecd.Cols - 3
    frmreceivable.grdDealRecd.Col = i
    frmreceivable.grdDealRecd.CellFontBold = True
    frmreceivable.grdDealRecd.ColWidth(i) = 1900
Next
frmreceivable.txtARcode = MSFlexGrid1.TextMatrix(RowSel, 0)
frmreceivable.txtARcode.Enabled = False
frmreceivable.txtinvestor.Text = MSFlexGrid1.TextMatrix(RowSel, 2)
frmreceivable.txtcompany.Text = MSFlexGrid1.TextMatrix(RowSel, 3)
frmreceivable.txtScheme.Text = MSFlexGrid1.TextMatrix(RowSel, 4)
frmreceivable.TxtAmount.Text = MSFlexGrid1.TextMatrix(RowSel, 10)
frmreceivable.cmbRecdDealBrok.ListIndex = 1
frmreceivable.cmbRecdFeeType.ListIndex = 0
Dim rsGetDealSlabs As New ADODB.Recordset
If rsGetDealSlabs.State = 1 Then rsGetDealSlabs.Close
rsGetDealSlabs.open "Select * from dealspecific_recd where tran_code='" & (MSFlexGrid1.TextMatrix(RowSel, 0)) & "' ORDER BY SLAB_ID", MyConn, adOpenForwardOnly, adLockReadOnly
i = 1
While Not rsGetDealSlabs.EOF
    frmreceivable.grdDealRecd.Rows = frmreceivable.grdDealRecd.Rows + 1
    frmreceivable.grdDealRecd.TextMatrix(i, 0) = rsGetDealSlabs("DEAL_BROK_TYPE")
    frmreceivable.grdDealRecd.TextMatrix(i, 1) = rsGetDealSlabs("PER_RS")
    frmreceivable.grdDealRecd.TextMatrix(i, 2) = rsGetDealSlabs("Figure")
    If rsGetDealSlabs("DEAL_BROK_TYPE") = "Trail" Or rsGetDealSlabs("DEAL_BROK_TYPE") = "Additional Trail" Then
        frmreceivable.grdDealRecd.TextMatrix(i, 3) = rsGetDealSlabs("DUE_AFTER") & "~" & rsGetDealSlabs("VALID_TILL") & "~" & rsGetDealSlabs("CAL_ON")
    End If
    frmreceivable.grdDealRecd.TextMatrix(i, 4) = rsGetDealSlabs("Slab_ID") 'slabId
    frmreceivable.grdDealRecd.TextMatrix(i, 5) = rsGetDealSlabs("tran_code")
    rsGetDealSlabs.MoveNext
    i = i + 1
Wend
rsGetDealSlabs.Close
Set rsGetDealSlabs = Nothing
Unload Me
Me.MousePointer = 0
strForm = ""
Exit Sub
End If
'-------------------------------------NPS----------------------------------------------
If currentForm.Name = "frmNPS" Then
    Dim VPOSPNO As String
    Dim VCorporateName As String
    rsGet.open "select Iss_name,prod_code from iss_master where iss_code='" & MSFlexGrid1.TextMatrix(RowSel, 21) & "'", MyConn, adOpenForwardOnly
    Issuer(0) = rsGet("iss_name")
    Issuer(1) = rsGet("Prod_code")
    rsGet.Close
    rsGet.open "Select Name from product_master where prod_code='" & Issuer(1) & "'", MyConn, adOpenForwardOnly
    frmNPS.cmbProd.Text = rsGet("Name")
    frmNPS.cmbProduct.Text = Issuer(0)
    rsGet.Close
    If Left(MSFlexGrid1.TextMatrix(RowSel, 21), 2) = "MF" And Left(MSFlexGrid1.TextMatrix(RowSel, 21), 4) <> "MFIS" Then
        rsGet.open "Select SCH_NAME NAME1,SHORT_NAME NAME2 from SCHEME_INFO where sch_code='" & MSFlexGrid1.TextMatrix(RowSel, 22) & "'", MyConn, adOpenForwardOnly
    Else
        rsGet.open "Select lONGName NAME1,OSCH_NAME NAME2 from other_product where osch_code='" & MSFlexGrid1.TextMatrix(RowSel, 22) & "'", MyConn, adOpenForwardOnly
    End If
    For i = 1 To frmNPS.lstlongname.ListItems.count
        If frmNPS.lstlongname.ListItems(i).Text = rsGet("NAME1") Then
            frmNPS.lstlongname.ListItems(i).Selected = True
            Exit For
        End If
    Next
    frmNPS.txtAmountInvest.Text = MSFlexGrid1.TextMatrix(RowSel, 10)
    If MSFlexGrid1.TextMatrix(RowSel, 14) = "C" Then
        frmNPS.optcheque.Value = True
    ElseIf MSFlexGrid1.TextMatrix(RowSel, 14) = "D" Then
        frmNPS.optdraft.Value = True
    ElseIf MSFlexGrid1.TextMatrix(RowSel, 14) = "H" Then
        frmNPS.optcash.Value = True
    End If
    frmNPS.lbtrancode.Caption = MSFlexGrid1.TextMatrix(RowSel, 0)
    frmNPS.Shortmutschfill
    
    Dim lst As ListItem
    frmNPS.lstlongname_ItemClick lst
    For i = 0 To frmNPS.lstSch.ListCount - 1
        If frmNPS.lstSch.List(i) = rsGet("NAME2") Then
            frmNPS.lstSch.Selected(i) = True
            Exit For
        End If
    Next
    rsGet.Close
    frmNPS.lstSch_Click
    frmNPS.txtChqNo.Text = MSFlexGrid1.TextMatrix(RowSel, 15)
    frmNPS.dtChqDate.Text = MSFlexGrid1.TextMatrix(RowSel, 16)
    frmNPS.txtrmbusicode = MSFlexGrid1.TextMatrix(RowSel, 26)
    frmNPS.txtAmountInvest.Text = MSFlexGrid1.TextMatrix(RowSel, 10)
    frmNPS.txtINV_CD.Text = MSFlexGrid1.TextMatrix(RowSel, 23)
    frmNPS.txtName.Text = MSFlexGrid1.TextMatrix(RowSel, 2)
    frmNPS.cmbBankName.Text = MSFlexGrid1.TextMatrix(RowSel, 17)
    frmNPS.txtPRAN.Text = MSFlexGrid1.TextMatrix(RowSel, 20)
    frmNPS.DtDate.Value = MSFlexGrid1.TextMatrix(RowSel, 1)
    frmNPS.txtrectno.Text = SqlRet("select UNIQUE_ID from TRANSACTION_ST where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'")
    VPOSPNO = SqlRet("select SUBSTR(UNIQUE_ID,3,7) from TRANSACTION_ST where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'")
    frmNPS.txtregistrationno.Text = VPOSPNO
    If VPOSPNO = "6036914" Then
        frmNPS.CboKRA.ListIndex = 0
    Else
        frmNPS.CboKRA.ListIndex = 1
    End If
    VCorporateName = SqlRet("select corporate_name from TRANSACTION_ST where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'")
    If VCorporateName = "" Then
        frmNPS.OptIndividual.Value = True
    Else
        frmNPS.OptCorporate.Value = True
        frmNPS.txtcorporatename = VCorporateName
    End If
    frmNPS.txtdocID.Text = SqlRet("select doc_ID from TRANSACTION_ST where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'")
    Dim RsData As New ADODB.Recordset
    RsData.open "select * from nps_transaction where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'", MyConn, adOpenKeyset
       For i = 0 To frmNPS.cboRequestType.ListCount - 1
        Req = Split(frmNPS.cboRequestType.List(i), "#")
        ReqCode = Req(1)
        If Req(1) = MSFlexGrid1.TextMatrix(RowSel, 6) Then
            frmNPS.cboRequestType.ListIndex = i
            Exit For
        End If
    Next
    If Not RsData.EOF Then
        If MSFlexGrid1.TextMatrix(RowSel, 10) = 0 And MSFlexGrid1.TextMatrix(RowSel, 6) = 12 Then
            frmNPS.TxtTire1.Text = 0
            frmNPS.TxtTire2.Text = 0
            frmNPS.txtpopregistration1.Text = 0
            frmNPS.txtpopregistration2.Text = 0
            frmNPS.txtServiceAmount.Text = 0
            frmNPS.txtAmountInvest.Text = 0
        Else
            frmNPS.TxtTire1.Text = RsData.Fields("amount1")
            frmNPS.TxtTire2.Text = RsData.Fields("amount2")
            frmNPS.txtpopregistration1.Text = RsData.Fields("reg_charge")
            frmNPS.txtpopregistration2.Text = RsData.Fields("tran_charge")
            frmNPS.txtServiceAmount.Text = RsData.Fields("servicetax")
            frmNPS.TxtRemark.Text = RsData.Fields("remark")
        End If
    End If
    Set RsData = Nothing
    
    For i = 0 To frmNPS.cboBranch.ListCount - 1
        Req = Split(frmNPS.cboBranch.List(i), "#")
        ReqCode = Req(2)
        If Trim(ReqCode) = Trim(MSFlexGrid1.TextMatrix(RowSel, 8)) Then
            frmNPS.cboBranch.ListIndex = i
            Exit For
        End If
    Next
    If rsGet.State = 1 Then rsGet.Close
    frmNPS.cmbBusiBranch.Clear
    rsGet.open "Select Branch_name from branch_master where branch_code=" & MSFlexGrid1.TextMatrix(RowSel, 27), MyConn, adOpenForwardOnly
    frmNPS.cmbBusiBranch.AddItem rsGet(0) & Space(100) & "#" & MSFlexGrid1.TextMatrix(RowSel, 27)
    frmNPS.cmbBusiBranch.ListIndex = 0
    rsGet.Close
    If MSFlexGrid1.TextMatrix(RowSel, 10) = 0 And MSFlexGrid1.TextMatrix(RowSel, 6) = 12 Then
    Else
        Call frmNPS.cboRequestType_Click
    End If
    Me.MousePointer = 0
    Unload Me
    Me.MousePointer = 0
    strForm = ""
    Exit Sub
End If
If currentForm.Name = "frmpaymentreced" Then
    frmpaymentreced.txtARcode.Text = MSFlexGrid1.TextMatrix(RowSel, 0)
    frmpaymentreced.lbldebit = Round(SqlRet("select brok_recd from bajaj_temptran_margin WHERE TRAN_CODE='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'"), 0)
    frmpaymentreced.lblcredit = Round(SqlRet("select sum(chq_amt) from PAYMENT_RECD_DETAILS WHERE TRAN_CODE='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'"), 0)
    frmpaymentreced.lblbalance = Val(frmpaymentreced.lbldebit) - Val(frmpaymentreced.lblcredit)
    frmpaymentreced.cmdview_Click
    Me.MousePointer = 0
    Unload Me
    Me.MousePointer = 0
    strForm = ""
    Exit Sub
End If
'--------------------------------------------------------------------------------------
If SIP_Flag = "SIP" Then
    SIP_Flag = ""
    frmSIPCheck.txtAR.Text = MSFlexGrid1.TextMatrix(RowSel, 0)
    frmSIPCheck.cmdview_Click
    Unload Me
    Exit Sub
End If
  
Dim tr As TreeView
    Set tr = currentForm.Controls(treeName)
    preSelectedCode = "MFIBR"
Dim Pcode As Long
    nodeValue = MSFlexGrid1.TextMatrix(RowSel, 23)
Dim K As Integer
 If AR = True Then
    If Trim(frmTransactionfind.currentForm.Name) = "frmArPrinting" Then
       frmArPrinting.trcode.Caption = Trim(MSFlexGrid1.TextMatrix(RowSel, 0))
       AR = False
       frmTransactionfind.optAfter.Enabled = True
       frmTransactionfind.optBefore.Enabled = True
    End If
 Else
    If MSFlexGrid1.TextMatrix(RowSel, 28) = "AutoSIP" Then
        'Me.MousePointer = 0
'        MsgBox "This AR is Generated through Automatic SIP Generation Program ", vbInformation
'        Exit Sub
    End If
 If EPF = False Then
        If MSFlexGrid1.TextMatrix(RowSel, 28) = "AutoSIP" Then
            Me.MousePointer = 0
            MsgBox "This AR is Generated through Automatic SIP Generation Program ", vbInformation
            FrmtransactionNew.cmdModifyNew1.Enabled = False
            FrmtransactionNew.cmdModifyNew.Enabled = False
        End If
        
        Set rs_get_invsrc = MyConn.Execute("Select source_id from investor_master where inv_code=" & nodeValue)
        If Left(rs_get_invsrc(0), 1) = 4 Then
            Set rs_get_rm = MyConn.Execute("select rm_code from client_master where client_code=" & rs_get_invsrc(0))
            If Not rs_get_rm.EOF Then
                Pcode = rs_get_rm(0)
                treeClientFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Key, Left(nodeValue, 8)
                treeInvestorFill currentForm.Controls(treeName), "C" & rs_get_invsrc(0), currentForm.Controls(treeName).Nodes("C" & rs_get_invsrc(0)).Child.Key
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Selected = True
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Bold = True
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Expanded = True
                If currentForm.Name = "FrmtransactionNew" Then
                    FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
                    FrmtransactionNew.userType = "INVESTOR"
                End If
            End If
        End If
        If Left(rs_get_invsrc(0), 1) = 3 Then
            Set rs_get_rm = MyConn.Execute("select rm_code from agent_master where agent_code=" & rs_get_invsrc(0))
            If Not rs_get_rm.EOF Then
                Pcode = rs_get_rm(0)
                treeAgentFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Next.Key, Left(nodeValue, 8)
                treeInvestorFill currentForm.Controls(treeName), "A" & rs_get_invsrc(0), currentForm.Controls(treeName).Nodes("A" & rs_get_invsrc(0)).Child.Key
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Selected = True
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Expanded = True
            End If
        End If
        
    With MSFlexGrid1
    If Left(.TextMatrix(RowSel, 21), 2) = "MF" And Left(.TextMatrix(RowSel, 21), 4) <> "MFIS" Then
        If MSFlexGrid1.TextMatrix(RowSel, 28) <> "AutoSIP" Then
            If optAfter.Value = True Then
                FrmtransactionNew.cmdModifyNew.Enabled = False
            Else
                FrmtransactionNew.cmdModifyNew.Enabled = True
            End If
        End If
        rsGet.open "Select Name from Product_master where nature_code='NT001'", MyConn, adOpenForwardOnly
        FrmtransactionNew.cmbProd.Text = rsGet("Name")
        rsGet.Close
        rsGet.open "select Mut_name from Mut_fund where mut_code='" & .TextMatrix(RowSel, 21) & "'", MyConn, adOpenForwardOnly
        Issuer(0) = rsGet("Mut_name")
        FrmtransactionNew.cmbProduct.Text = Issuer(0)
        rsGet.Close
        rsGet.open "Select sch_Name from scheme_info where sch_code='" & .TextMatrix(RowSel, 22) & "'", MyConn, adOpenForwardOnly
        For i = 1 To FrmtransactionNew.lstlongname.ListItems.count
            If FrmtransactionNew.lstlongname.ListItems(i).Text = rsGet("sch_name") Then
                FrmtransactionNew.lstlongname.ListItems(i).Selected = True
                Exit For
            End If
        Next
        rsGet.Close
        rsGet.open "Select SCH_NAME NAME1,SHORT_NAME NAME2 from SCHEME_INFO where sch_code='" & .TextMatrix(RowSel, 22) & "'", MyConn, adOpenForwardOnly
        'FrmtransactionNew.dtTran.Text = .TextMatrix(RowSel, 1)
        'FrmtransactionNew.dttranMF.Text = .TextMatrix(RowSel, 1)
        FrmtransactionNew.cmbFolio.Text = .TextMatrix(RowSel, 8)
        FrmtransactionNew.TxtAmount.Text = .TextMatrix(RowSel, 10)
        FrmtransactionNew.txtUnits.Text = .TextMatrix(RowSel, 11)
        'FrmtransactionNew.txtPrice.Text = .TextMatrix(RowSel, 12)
        If GlbroleId <> 1 Then
             FrmtransactionNew.dttranMF.Enabled = False
        End If
    Else
        
        rsGet.open "select Iss_name,prod_code from iss_master where iss_code='" & .TextMatrix(RowSel, 21) & "'", MyConn, adOpenForwardOnly
        Issuer(0) = rsGet("iss_name")
        Issuer(1) = rsGet("Prod_code")
        rsGet.Close
        rsGet.open "Select Name from product_master where prod_code='" & Issuer(1) & "'", MyConn, adOpenForwardOnly
        FrmtransactionNew.cmbProd.Text = rsGet("Name")
        FrmtransactionNew.cmbProduct.Text = Issuer(0)
        rsGet.Close
        If Left(.TextMatrix(RowSel, 21), 2) = "MF" And Left(.TextMatrix(RowSel, 21), 4) <> "MFIS" Then
            rsGet.open "Select SCH_NAME NAME1,SHORT_NAME NAME2 from SCHEME_INFO where sch_code='" & .TextMatrix(RowSel, 22) & "'", MyConn, adOpenForwardOnly
        Else
            rsGet.open "Select lONGName NAME1,OSCH_NAME NAME2 from other_product where osch_code='" & .TextMatrix(RowSel, 22) & "'", MyConn, adOpenForwardOnly
        End If
        For i = 1 To FrmtransactionNew.lstlongname.ListItems.count
            If FrmtransactionNew.lstlongname.ListItems(i).Text = rsGet("NAME1") Then
                FrmtransactionNew.lstlongname.ListItems(i).Selected = True
                Exit For
            End If
        Next
        
'        Dim lst As ListItem
'        FrmtransactionNew.lstlongname_ItemClick lst
'        For i = 0 To FrmtransactionNew.lstSch.ListCount - 1
'            If FrmtransactionNew.lstSch.List(i) = rsGet("NAME2") Then
'                FrmtransactionNew.lstSch.Selected(i) = True
'                Exit For
'            End If
'        Next
'        rsGet.Close


        FrmtransactionNew.dtOpTran.Text = .TextMatrix(RowSel, 1)
        FrmtransactionNew.txtOpAmount.Text = .TextMatrix(RowSel, 10)
        FrmtransactionNew.txtOpUnits.Text = .TextMatrix(RowSel, 11)
        FrmtransactionNew.txtOpPrice.Text = .TextMatrix(RowSel, 12)
        If GlbroleId <> 1 Then
            FrmtransactionNew.dtOpTran.Enabled = False
        End If
        
        
        If GlbroleId = 212 Then
            FrmtransactionNew.cmbProduct.Enabled = False
            FrmtransactionNew.lstlongname.Enabled = False
            FrmtransactionNew.dtOpTran.Enabled = False
        Else
            FrmtransactionNew.cmbProduct.Enabled = True
            FrmtransactionNew.lstlongname.Enabled = True
            FrmtransactionNew.dtOpTran.Enabled = True
        End If
    End If
    If .TextMatrix(RowSel, 14) = "C" Then
        FrmtransactionNew.optcheque.Value = True
    ElseIf .TextMatrix(RowSel, 14) = "D" Then
        FrmtransactionNew.optdraft.Value = True
    ElseIf .TextMatrix(RowSel, 14) = "H" Then
        FrmtransactionNew.optcash.Value = True
    ElseIf .TextMatrix(RowSel, 14) = "S" Then
        FrmtransactionNew.OptSip.Value = True
    End If
    FrmtransactionNew.lbtrancode.Caption = .TextMatrix(RowSel, 0)
    FrmtransactionNew.Shortmutschfill
    
    'Dim lst As ListItem
    FrmtransactionNew.lstlongname_ItemClick lst
    For i = 0 To FrmtransactionNew.lstSch.ListCount - 1
        If FrmtransactionNew.lstSch.List(i) = rsGet("NAME2") Then
            FrmtransactionNew.lstSch.Selected(i) = True
            Exit For
        End If
    Next
    rsGet.Close
    
    
    'FrmtransactionNew.lstSch.Selected(0) = True
    FrmtransactionNew.lstSch_Click
    FrmtransactionNew.cmbBankName.Text = .TextMatrix(RowSel, 17)
    FrmtransactionNew.cmbTranType.Text = .TextMatrix(RowSel, 9)
    FrmtransactionNew.cmbFolio.Text = .TextMatrix(RowSel, 8)
    FrmtransactionNew.txtChqNo.Text = .TextMatrix(RowSel, 15)
    FrmtransactionNew.dtChqDate.Text = .TextMatrix(RowSel, 16)
    FrmtransactionNew.txtmanualarno.Text = .TextMatrix(RowSel, 20)
    FrmtransactionNew.txtdocID.Text = .TextMatrix(RowSel, 31)
    FrmtransactionNew.cmbAppNo.Text = .TextMatrix(RowSel, 6)
    rsGet.open "Select Broker_id,name from broker_master where broker_id=" & .TextMatrix(RowSel, 24), MyConn, adOpenForwardOnly
    FrmtransactionNew.cmbBroker.Text = rsGet("Name") & "(" & rsGet("Broker_id") & ")"
    If rsGet.State = 1 Then rsGet.Close
    rsGet.open "Select remark from transaction_st where tran_code='" & .TextMatrix(RowSel, 0) & "'", MyConn, adOpenForwardOnly
    FrmtransactionNew.txtRem.Text = IIf(Trim(rsGet(0)) <> "", Trim(rsGet(0)), "")
    Dim rsIntro As New ADODB.Recordset
    Set rsIntro = MyConn.Execute("select * from transaction_intro where tran_code='" & .TextMatrix(RowSel, 0) & "'")
    'FrmtransactionNew.txtrmbusicode.Text = .TextMatrix(RowSel, 26) 'rsGet("payroll_id")
    If Not rsIntro.EOF Then
        FrmtransactionNew.cmbIntro.Text = rsIntro(2)
        FrmtransactionNew.txtIntroCode.Text = rsIntro(1)
'        If UCase(rsIntro(3)) = "YES" Then
'            FrmtransactionNew.optfp.Value = True
'        Else
'            FrmtransactionNew.optfp1.Value = True
'        End If

        If Not (IsNull(rsIntro("campaign"))) Then
            For i = 0 To FrmtransactionNew.cmbCampaign.ListCount - 1
                If rsIntro("campaign") = FrmtransactionNew.cmbCampaign.List(i, 1) Then
                    'FrmtransactionNew.cmbCampaign.Text = rsIntro("campaign")
                    FrmtransactionNew.cmbCampaign.ListIndex = i
                End If
            Next i
        End If
        If Not (IsNull(rsIntro("subcampaign"))) Then
            For i = 0 To FrmtransactionNew.cmbSubCampaign.ListCount - 1
                If rsIntro("subcampaign") = FrmtransactionNew.cmbSubCampaign.List(i, 1) Then
                    FrmtransactionNew.cmbSubCampaign.ListIndex = i
                End If
            Next i
            'FrmtransactionNew.cmbSubCampaign.Text = rsIntro("subcampaign")
        End If
        
    End If
    Set rsIntro = Nothing
    
    FrmtransactionNew.cmbBusiBranch.Clear
    FrmtransactionNew.treeClient_Click
    
    busi_code = .TextMatrix(RowSel, 26)
    FrmtransactionNew.txtrmbusicode.Text = busi_code 'rsGet("payroll_id")
    FrmtransactionNew.txtrmbusicode_LostFocus
    If rsGet.State = 1 Then rsGet.Close
    '
    busi_br_tr = .TextMatrix(RowSel, 27)
    rsGet.open "Select Branch_name from branch_master where branch_code=" & .TextMatrix(RowSel, 27), MyConn, adOpenForwardOnly
    'FrmtransactionNew.cmbBusiBranch.Text = rsGet(0) & Space(100) & "#" & .TextMatrix(RowSel, 27)
    FrmtransactionNew.cmbBusiBranch.AddItem rsGet(0) & Space(100) & "#" & .TextMatrix(RowSel, 27)
    FrmtransactionNew.cmbBusiBranch.ListIndex = 0
    
    'FrmtransactionNew.cmbBusiBranch.ListIndex = 0
    rsGet.Close
    FrmtransactionNew.txtRem.Text = .TextMatrix(RowSel, 28)
    busi_change_flag = False
    End With
    
    Me.MousePointer = 0
    
    FrmtransactionNew.Lbl_Leadno.Caption = ""
    rsGet.open "select * from tran_lead where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'", MyConn, adOpenForwardOnly
    If rsGet.EOF = False Then
        FrmtransactionNew.Lbl_Leadno.Caption = rsGet("lead_no")
    End If
    rsGet.Close
    Set rsGet = Nothing
    
    
    
    'Unload Me
    
    
    
    '------------------------------------------COMMENT BY PANKAJ-------------------------
'    If UCase(MSFlexGrid1.TextMatrix(RowSel, 30)) = "CROREPATI" Then
'        'frm_crorepati.cmb_cror.ListIndex = 0
'        frm_crorepati.cmb_cror_Click
'        frm_crorepati.optfp.Value = True
'        For i = 0 To frm_crorepati.cmb_planno.ListCount - 1
'            If MSFlexGrid1.TextMatrix(RowSel, 29) = frm_crorepati.cmb_planno.List(i) Then
'                frm_crorepati.cmb_planno.ListIndex = i
'                Exit For
'            End If
'        Next i
'    ElseIf UCase(MSFlexGrid1.TextMatrix(RowSel, 30)) = "COMPREHENSIVE" Then
'        'frm_crorepati.cmb_cror.ListIndex = 1
'        frm_crorepati.cmb_cror_Click
'        frm_crorepati.optfp.Value = True
'    Else
'        frm_crorepati.optfp.Value = False
'    End If
'    Unload Me
    '-----------------------------------------------------------------------------------
'    frm_crorepati.Left = 3500
'    frm_crorepati.Top = 4200
'    frm_crorepati.Show

If FrmtransactionNew.Label32.Caption <> "" Then
FrmtransactionNew.Change_bt_Click
End If

  Else
        If MSFlexGrid1.TextMatrix(RowSel, 28) = "AutoSIP" Then
            Me.MousePointer = 0
            MsgBox "This AR is Generated through Automatic SIP Generation Program ", vbInformation
        End If
  
    If frmPayment.Opt_client.Value = True Then
        Set rsGet = MyConn.Execute("select client_name,client_code from client_master where client_code=" & Left(MSFlexGrid1.TextMatrix(RowSel, 23), 8) & " ")
    Else
        Set rsGet = MyConn.Execute("select agent_name,agent_code from agent_master where agent_code=" & Left(MSFlexGrid1.TextMatrix(RowSel, 23), 8) & " ")
    End If
    If Not rsGet.EOF Then
      frmPayment.txtARcode = MSFlexGrid1.TextMatrix(RowSel, 0)
      frmPayment.txtARcode.Enabled = False
      frmPayment.Listclagname.Clear
      frmPayment.Listclagname.AddItem rsGet(0) & Space(80) & "#" & rsGet(1)
      frmPayment.Listclagname.Selected(0) = True
      frmPayment.AMT.Caption = MSFlexGrid1.TextMatrix(RowSel, 10)
      If rsGet.State = 1 Then rsGet.Close
      If rsGet1.State = 1 Then rsGet1.Close
      frmPayment.txttot = "0.00"
      frmPayment.txtPaid = "0.00"
      frmPayment.txtBal = "0.00"
      Set rsGet = MyConn.Execute("select sum(amt) from payment_detail where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "' GROUP BY TRAN_CODE")
      Set rsGet1 = MyConn.Execute("select TDS from payment_detail where tran_code='" & MSFlexGrid1.TextMatrix(RowSel, 0) & "'")
      frmPayment.txttot = Format(rsGet1(0), "0.00")
      frmPayment.txtPaid = Format(rsGet(0), "0.00")
      frmPayment.txtBal = Format(Format(rsGet1(0), "0.00") - Format(rsGet(0), "0.00"), "0.00")
      frmPayment.TxtAmount = frmPayment.txtBal.Text
      If frmPayment.txttot = "" Or frmPayment.txttot = "0.00" Then
        ShowSlab_Extra MSFlexGrid1.TextMatrix(MSFlexGrid1.Row, 0), MSFlexGrid1.TextMatrix(MSFlexGrid1.Row, 10)
      End If
      'If Left(MSFlexGrid1.TextMatrix(RowSel, 21), 2) = "MF" And frmPayment.Opt_client.Value = True Then
      'frmPayment.Fpay(1).Enabled = True
      'Else
      '  frmPayment.Fpay(1).Enabled = False
      'End If
      'by vinod to implement modify Slab
'''      If frmPayment.txtPaid = "" Or frmPayment.txtPaid = "0.00" Then
'''        frmPayment.Fpay(1).Enabled = True
'''      Else
'''        frmPayment.Fpay(1).Enabled = False
'''      End If
      'end
      frmPayment.Fpay(1).Enabled = True
      Unload Me
      EPF = False
      If rsGet.State = 1 Then rsGet.Close
      If rsGet1.State = 1 Then rsGet1.Close
    End If
    frmTransactionfind.optBefore.Enabled = True
  End If
 End If
 
  Unload Me
  Me.MousePointer = 0
  Set rsGet = Nothing
End If
End Sub

Private Sub ShowSlab_Extra(tran_cd As String, AMT As Double)
Dim ShSlab As New ADODB.Recordset
Dim fig As Single, FigOPe As Single
Dim CalFig As Double, CalOpe As Double

'If UCase(grid1.TextMatrix(Gridrow, 6)) = UCase("purchase") Or UCase(grid1.TextMatrix(Gridrow, 6)) = UCase("reinvestment") Then
    
    Set ShSlab = MyConn.Execute("select * from dealspecific_paid where tran_code='" & tran_cd & "'")
    If Not ShSlab.EOF Then
        While Not ShSlab.EOF
            'vinod 27/04/2005  DealSpecifice Slabs
            'ShowSlabs.TextMatrix(Gridrow, 0) = IIf((IsNull(ShSlab("slab_id")) Or ShSlab("slab_id") = ""), "", ShSlab("slab_id"))
            'ShowSlabs.TextMatrix(Gridrow, 1) = IIf((IsNull(ShSlab("paid_cal_on")) Or ShSlab("paid_cal_on") = ""), "", ShSlab("paid_cal_on"))
            'ShowSlabs.TextMatrix(Gridrow, 6) = IIf((IsNull(ShSlab("per_rs")) Or ShSlab("per_rs") = ""), "", ShSlab("per_rs"))
            'ShowSlabs.TextMatrix(Gridrow, 7) = IIf((IsNull(ShSlab("figure")) Or ShSlab("figure") = ""), "", ShSlab("figure"))
            If UCase(ShSlab("paid_cal_on")) = UCase("amount invested") Then
                fig = ShSlab("figure")
                If UCase(ShSlab("per_rs")) = UCase("percentage") Then
                    CalFig = CalFig + (CDbl(AMT) * fig) / 100
                    CalOpe = CalOpe + (CDbl(AMT) * FigOPe) / 100
                Else
                    CalFig = CalFig + CDbl(fig)
                    CalOpe = CalOpe + CDbl(CalOpe)
                End If
            Else
            End If
            
            ShSlab.MoveNext
        Wend
      
      frmPayment.txttot = Format((CalFig + CalOpe), "0.00")
      frmPayment.txtPaid = "0.00"
      frmPayment.txtBal = Format((CalFig + CalOpe), "0.00")
      frmPayment.TxtAmount = Format((CalFig + CalOpe), "0.00")
      
        'If MsgBox("Your Approximate Brokerage is :->> Brokergage (" & Format(CalFig, "0.00") & ") And BrokerageOpe(" & Format(CalOpe, "0.00") & ")" & vbCrLf _
        '    & "Would You like To Make an advance Payment Against That Transaction", vbYesNo, "WEALTH MAKER") = vbYes Then
        '    lbappbrok.Caption = Format(CalFig + CalOpe, "0.00")
        '    txtamt.Text = lbappbrok.Caption
        '    lbtranscoderes.Caption = Trim(grid1.TextMatrix(Gridrow, 15))
        '    stab.Tab = 3
        'End If
    Else
       Set ShSlab = MyConn.Execute("select * from UPFRONT_paid where tran_code='" & tran_cd & "'")
       If Not ShSlab.EOF Then
       While Not ShSlab.EOF
            'vinod 27/04/2005  DealSpecifice Slabs
            'ShowSlabs.TextMatrix(Gridrow, 0) = IIf((IsNull(ShSlab("slab_id")) Or ShSlab("slab_id") = ""), "", ShSlab("slab_id"))
            'ShowSlabs.TextMatrix(Gridrow, 1) = IIf((IsNull(ShSlab("paid_cal_on")) Or ShSlab("paid_cal_on") = ""), "", ShSlab("paid_cal_on"))
            'ShowSlabs.TextMatrix(Gridrow, 6) = IIf((IsNull(ShSlab("per_rs")) Or ShSlab("per_rs") = ""), "", ShSlab("per_rs"))
            'ShowSlabs.TextMatrix(Gridrow, 7) = IIf((IsNull(ShSlab("figure")) Or ShSlab("figure") = ""), "", ShSlab("figure"))
            If UCase(ShSlab("paid_cal_on")) = UCase("amount invested") Then
                fig = ShSlab("figure")
                If UCase(ShSlab("per_rs")) = UCase("percentage") Then
                    CalFig = CalFig + (CDbl(AMT) * fig) / 100
                    CalOpe = CalOpe + (CDbl(AMT) * FigOPe) / 100
                Else
                    CalFig = CalFig + CDbl(fig)
                    CalOpe = CalOpe + CDbl(CalOpe)
                End If
            Else
            End If
            
            ShSlab.MoveNext
        Wend
      
      frmPayment.txttot = Format((CalFig + CalOpe), "0.00")
      frmPayment.txtPaid = "0.00"
      frmPayment.txtBal = Format((CalFig + CalOpe), "0.00")
      frmPayment.TxtAmount = Format((CalFig + CalOpe), "0.00")
        
        'If MsgBox("Your Approximate Brokerage is :->> Brokergage (" & Format(CalFig, "0.00") & ") And BrokerageOpe(" & Format(CalOpe, "0.00") & ")" & vbCrLf _
        '    & "Would You like To Make an advance Payment Against That Transaction", vbYesNo, "WEALTH MAKER") = vbYes Then
        '    lbappbrok.Caption = Format(CalFig + CalOpe, "0.00")
        '    txtamt.Text = lbappbrok.Caption
        '    lbtranscoderes.Caption = Trim(grid1.TextMatrix(Gridrow, 15))
        '    stab.Tab = 3
       'End If
      End If
    End If
'End If
End Sub


Private Sub MSFlexGrid1_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        MSFlexGrid1_DblClick
    End If
End Sub
Private Sub mskfromdate_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        msktodate.SetFocus
    End If
End Sub
Private Sub msktodate_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        optBefore.SetFocus
    End If
End Sub
Private Sub optAfter_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        cmdsearch.SetFocus
    End If
End Sub
Private Sub optBefore_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        cmdsearch.SetFocus
    End If
End Sub
Private Sub txtappno_KeyPress(KeyAscii As Integer)
    If KeyAscii = 39 Then KeyAscii = 146
    If KeyAscii = 13 Then
        txtchequeno.SetFocus
    End If
End Sub
Private Sub txtARNo_KeyPress(KeyAscii As Integer)
    If KeyAscii = 39 Then KeyAscii = 146
    If KeyAscii = 13 Then
        txtappno.SetFocus
    End If
End Sub
Private Sub txtChequeNo_KeyPress(KeyAscii As Integer)
    If KeyAscii = 39 Then KeyAscii = 146
    If KeyAscii = 13 Then
        txtmarno.SetFocus
    End If
End Sub
Private Sub txtmarno_KeyPress(KeyAscii As Integer)
    If KeyAscii = 39 Then KeyAscii = 146
    If KeyAscii = 13 Then
        txtScheme.SetFocus
    End If
End Sub
Private Function getBranchName(refcode As String) As String
Dim BRCODELIST As String
Dim qry As String
Dim rs_get_br As ADODB.Recordset
Dim rs_get_Loc As ADODB.Recordset
    If UCase(Left(refcode, 1)) = "R" Then
        qry = "select branch_code from branch_master where region_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "Z" Then
        qry = "select branch_code from branch_master where zone_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "C" Then
        Set rs_get_Loc = New ADODB.Recordset
        qry = "select location_id from location_master where city_id='" & refcode & "'"
        Set rs_get_Loc = MyConn.Execute(qry)
        If Not rs_get_Loc.EOF Then
            Set rs_get_br = New ADODB.Recordset
            Do While Not rs_get_Loc.EOF
                qry = "select branch_code from branch_master where location_id='" & rs_get_Loc(0) & "'"
                Set rs_get_br = MyConn.Execute(qry)
                If Not rs_get_br.EOF Then
                    Do While Not rs_get_br.EOF
                        BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                        rs_get_br.MoveNext
                    Loop
                End If
                rs_get_Loc.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "L" Then
        qry = "select branch_code from branch_master where location_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf Left(refcode, 1) = "2" Then
        qry = "select source from employee_master where rm_code='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf Left(refcode, 1) = "1" Then
        BRCODELIST = BRCODELIST & "#" & refcode
    ElseIf Left(refcode, 1) = "7" Then
        qry = "select branch_code from branch_master"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    End If
    getBranchName = BRCODELIST
End Function
Private Sub txtScheme_KeyPress(KeyAscii As Integer)
    If KeyAscii = 39 Then KeyAscii = 146
    If KeyAscii = 13 Then
        mskfromdate.SetFocus
    End If
End Sub

Private Sub set_grid()
'On Error GoTo err1
    msfgClients.Row = 0
    msfgClients.ColWidth(0) = "1500"
    msfgClients.Text = "ArNo"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 1
    msfgClients.ColWidth(1) = "1500"
    msfgClients.Text = "ModifyDate"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 2
    msfgClients.ColWidth(2) = "0"
    msfgClients.Text = "LastUpdatedUserId"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 3
    msfgClients.ColWidth(3) = "1600"
    msfgClients.Text = "LastUser"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 4
    msfgClients.ColWidth(4) = "1600"
    msfgClients.Text = "ChangedField"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 5
    msfgClients.ColWidth(5) = "2000"
    msfgClients.Text = "PrevValue"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 6
    msfgClients.ColWidth(6) = "2000"
    msfgClients.Text = "ChangedValue"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 7
    msfgClients.ColWidth(7) = "0"
    msfgClients.Text = "CurrUpdatedUserId"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 8
    msfgClients.ColWidth(8) = "1200"
    msfgClients.Text = "CurrentUser"
    msfgClients.CellFontBold = True
    
    msfgClients.SetFocus
    If msfgClients.Rows > 1 Then
        msfgClients.Row = 1
    End If
    msfgClients.Col = 0
    Exit Sub
err1:
    'MsgBox err.Description
End Sub
