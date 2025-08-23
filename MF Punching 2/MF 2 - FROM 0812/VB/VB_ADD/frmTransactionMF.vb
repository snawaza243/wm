Dim rs_get_trantype As New ADODB.Recordset
Dim RsData As New ADODB.Recordset
Dim rslead As New ADODB.Recordset
Dim ARCODE As ADODB.Recordset
Dim ARCODE1 As ADODB.Recordset
Dim MyCurrentBranchCode As String
Public MyNat As String
Dim ArLead() As String
Dim rsAmc As New ADODB.Recordset
Dim rsscheme As New ADODB.Recordset
Dim MyAmcCode As String
Dim MySchCode As String
Dim MyCloseSchCode As String
Dim MyCloseSchCodeM As String
Dim MySwitchSchCodeA As String
Dim MySwitchSchCodeM As String
Dim MyTranCode As String
Dim MyBranchCode As String
Dim MyRmCodeA As Long
Dim MyRmCodeM As Long
Dim ColumnIndex As Long
Dim Glb_L_SearchIndex As Integer
Dim Glb_Selected_row As Integer
Dim Glb_Flag_First_Time As Boolean
Dim MyOrder As String
Dim paymode As String
Dim MyPrintTranCode As String
Dim MyBrokRecd As Long
Dim MyMargin As Long
Dim MyBrokPay As Long
Dim MyRecdA As Long
Dim MyRecdM As Long
Dim MyUniqueKey As Long
Dim MyFreq() As String
Dim RsFreq As ADODB.Recordset
Dim MyFreq1() As String
Dim SIPADV  As Long
Dim BaseTranCode As String
Dim dob As Date
Dim Category As String
Dim IsMinor As Boolean
Dim branch_code As String
Dim asetflag As String
Dim inv_det As New ADODB.Recordset
Dim mob_ch As Integer
Dim MOB_NUM  As String
Dim micropanflag As Integer
Dim ModifyDocid As String
Dim MaxInstallmentNo   As Integer




Private Sub call_frmschemesearchM()
  strscheme = "frmtransactionmf_M"
  frmSchemeSearch.Show
  frmSchemeSearch.ZOrder
End Sub


Private Sub call_frmschemesearch()
  strscheme = "frmtransactionmf"
  frmSchemeSearch.Show
  frmSchemeSearch.ZOrder
End Sub

Private Sub CmbtxtCity_Change()
Dim state_dat As New ADODB.Recordset
      cboState.Clear
        state_dat.open "select * from state_master where state_id=(select state_id from city_master where city_name='" & CmbtxtCity.Text & "') order  by state_name", MyConn, adOpenForwardOnly, adLockReadOnly
        Do While Not state_dat.EOF
            cboState.AddItem state_dat!state_name
            state_dat.MoveNext
        Loop
    state_dat.Close
End Sub



Private Sub txtAdd1_Validate(Cancel As Boolean)
If Len(txtAdd1.Text) <= 0 Then
MsgBox "Please Enter a valid address", vbInformation, strBajaj
txtAdd1.Text = ""
    Exit Sub
    End If
End Sub
Private Sub txtadd2_Validate(Cancel As Boolean)
If Len(txtAdd2.Text) <= 0 Then
MsgBox "Please Enter a valid address", vbInformation, strBajaj
txtAdd2.Text = ""
    Exit Sub
    End If
End Sub

Private Sub txtIPin_Change()

End Sub

Private Sub txtIPin_Validate(Cancel As Boolean)

End Sub

Private Sub txtMobile_Change()
mob_ch = 1
End Sub

Private Sub txtMobile_Validate(Cancel As Boolean)


If txtMobile.Text <> "" Then
    If Len(txtMobile.Text) <> 10 Or Mid(txtMobile.Text, 1, 1) = 1 Or Mid(txtMobile.Text, 1, 1) = 2 Or Mid(txtMobile.Text, 1, 1) = 3 Or Mid(txtMobile.Text, 1, 1) = 4 Or Mid(txtMobile.Text, 1, 1) = 5 Then
    MsgBox "Please Enter a valid mobile number !", vbInformation, strBajaj
    'txtMobile.Text = ""
    Exit Sub
    End If
End If


End Sub

Private Sub txtPin_Validate(Cancel As Boolean)
If txtPin.Text = "" Or Len(txtPin.Text) <> 6 Then
    MsgBox "Please Enter a valid Pincode number !", vbInformation, strBajaj
    txtPin.Text = ""
    Exit Sub
    End If
End Sub

Private Sub txtEMail_Validate(Cancel As Boolean)
If txtEmail.Text <> "" Then
    If InStr(txtEmail.Text, "@") < 1 Or InStr(txtEmail.Text, ".") < 1 Then
    MsgBox "Please Enter a valid Email id !", vbInformation, strBajaj
    txtEmail.Text = ""
    Exit Sub
    End If
End If
End Sub
Private Sub txtMobile_KeyPress(KeyAscii As Integer)
If KeyAscii = 8 Then Exit Sub
If KeyAscii < 48 Or KeyAscii > 57 Then
KeyAscii = 0
Exit Sub
End If
End Sub

Private Sub gridfill(X As Integer)
On Error GoTo err1
If SSTab1.Tab = 0 Then
    StrSql = "select mf.investor_name,mf.bank_name,mf.SCH_CODE,rm_name,sip_type,branch_name,PANNO,amc.mut_name Amc_Name,sch_name Scheme_Name,TR_DATE,TRAN_TYPE,App_No,PAYMENT_MODE,CHEQUE_NO,CHEQUE_DATE,"
    StrSql = StrSql & "Amount,TRAN_TYPE,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.frequency,mf.installments_no,micro_investment,target_switch_scheme "
    StrSql = StrSql & " from employee_master e,branch_master b,mut_fund amc,scheme_info sch,transaction_mf_Temp1 mf "
    StrSql = StrSql & " where to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  "
    StrSql = StrSql & " and mf.MUT_CODE=amc.mut_code and mf.SCH_CODE=sch.sch_code and mf.BUSINESS_RMCODE=" & TxtBusiCodeA & " AND trunc(update_date) = to_date(SYSDATE)  order by "
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_DATE,rm_name "
    Else
        StrSql = StrSql & "" & MyOrder
    End If
    If Option1(1).Value = True Then
        StrSql = StrSql & " Desc"
    End If
    '
ElseIf SSTab1.Tab = 1 Then
    StrSql = "select * from temporaryBusiness where BUSI_BRANCH_CODE  in ( " & Branches & ")"
    If mskfromdt.Text <> "__/__/____" Then
        StrSql = StrSql & " and TR_DATE>=TO_DATE('" & mskfromdt & "','DD/MM/YYYY') "
    End If
    If msktodt.Text <> "__/__/____" Then
        StrSql = StrSql & " and TR_DATE<=TO_DATE('" & msktodt & "','DD/MM/YYYY') "
    End If
    If txtpans.Text <> "" Then
        StrSql = StrSql & " and PANNO='" & txtpans.Text & "' "
    End If
    If TxtClientS.Text <> "" Then
        StrSql = StrSql & " and SOURCE_CODE='" & TxtClientS.Text & "' "
    End If
    If txtTrNo.Text <> "" Then
        StrSql = StrSql & " and Tran_CODE='" & txtTrNo.Text & "'  "
    End If
    '---------------------------ANA code, Cheque, Application Search-------------------------------------------
    If Trim(TxtANACode.Text) <> "" Then
        StrSql = StrSql & " and source_CODE=(select agent_code from agent_master where upper(exist_code)=upper('" & Trim(TxtANACode.Text) & "'))  "
    End If
    If Trim(TxtChequeno1.Text) <> "" Then
        StrSql = StrSql & " and cheque_no='" & Trim(TxtChequeno1.Text) & "'"
    End If
    If Trim(txtappno.Text) <> "" Then
        StrSql = StrSql & " and app_no='" & Trim(txtappno.Text) & "'"
    End If
    '---------------------------------MASTER SIP SEARCH------------------------------------------------------------
    If Txtinstallments1.Text <> "" Then
        StrSql = StrSql & " and INSTALLMENTS_NO='" & Txtinstallments1.Text & "' "
    End If
    If CmbSipStpM.Text <> "" Then
       StrSql = StrSql & " and sip_type='" & CmbSipStpM.Text & "' "
    End If
    If Label42.Caption <> "" Then
        StrSql = StrSql & " and client_code='" & Label42.Caption & "' "
    End If
    If TxtAcHolderM.Text <> "" Then
        StrSql = StrSql & " and AC_HOLDER_CODE='" & TxtAcHolderM.Text & "' "
    End If
    If TxtBusiCodeM.Text <> "" Then
        StrSql = StrSql & " and BUSINESS_RMCODE='" & TxtBusiCodeM.Text & "' "
    End If
    If TxtPanM.Text <> "" Then
        StrSql = StrSql & " and PANNO='" & TxtPanM.Text & "' "
    End If
    If CmbFrequency1.ListIndex <> -1 Then
        MyFreq = Split(CmbFrequency1.Text, "#")
        StrSql = StrSql & " and frequency=" & MyFreq(1) & " "
    End If
    '--------------------------------------------------------------------------------------------------------------
    StrSql = StrSql & " order by  "
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_DATE,rm_name "
    Else
        StrSql = StrSql & "" & MyOrder
    End If
    If Option1(1).Value = True Then
        StrSql = StrSql & " Desc"
    End If
End If
Set rsMap = New ADODB.Recordset
rsMap.open StrSql, MyConn, adOpenDynamic, adLockOptimistic
i = 1
VSFCommGrdA.Clear
VSFCommGrdM.Clear
Call SetGrid
VSFCommGrdM.Rows = rsMap.RecordCount + 1
While Not rsMap.EOF
    If X = 1 Then
        VSFCommGrdA.TextMatrix(i, 0) = IIf(IsNull(rsMap("Investor_Name")), "", rsMap("Investor_Name"))
        VSFCommGrdA.TextMatrix(i, 1) = rsMap("Branch_Name")
        VSFCommGrdA.TextMatrix(i, 2) = IIf(IsNull(rsMap("PANNO")), "", rsMap("PANNO"))
        VSFCommGrdA.TextMatrix(i, 3) = rsMap("Amc_Name")
        VSFCommGrdA.TextMatrix(i, 4) = rsMap("Scheme_Name")
        VSFCommGrdA.TextMatrix(i, 5) = rsMap("TR_DATE")
        VSFCommGrdA.TextMatrix(i, 6) = IIf(IsNull(rsMap("TRAN_TYPE")), "", rsMap("TRAN_TYPE"))
        VSFCommGrdA.TextMatrix(i, 7) = IIf(IsNull(rsMap("App_No")), "", rsMap("App_No"))
        VSFCommGrdA.TextMatrix(i, 8) = IIf(IsNull(rsMap("PAYMENT_MODE")), "", rsMap("PAYMENT_MODE"))
        VSFCommGrdA.TextMatrix(i, 9) = IIf(IsNull(rsMap("CHEQUE_NO")), 0, rsMap("CHEQUE_NO"))
        VSFCommGrdA.TextMatrix(i, 10) = IIf(IsNull(rsMap("CHEQUE_DATE")), "", rsMap("CHEQUE_DATE"))
        VSFCommGrdA.TextMatrix(i, 11) = rsMap("Amount")
        VSFCommGrdA.TextMatrix(i, 12) = rsMap("Sip_TYPE")
        VSFCommGrdA.TextMatrix(i, 13) = IIf(IsNull(rsMap("lEAD_NO")), 0, rsMap("lEAD_nO"))
        VSFCommGrdA.TextMatrix(i, 14) = IIf(IsNull(rsMap("LEAD_nAME")), "", rsMap("LEAD_nAME"))
        VSFCommGrdA.TextMatrix(i, 15) = IIf(IsNull(rsMap("TRAN_code")), "", rsMap("TRAN_code"))
        VSFCommGrdA.TextMatrix(i, 16) = rsMap("branch_code")
        VSFCommGrdA.TextMatrix(i, 17) = rsMap("BUSINESS_RMCODE")
        VSFCommGrdA.TextMatrix(i, 18) = rsMap("SCH_CODE")
        VSFCommGrdA.TextMatrix(i, 19) = rsMap("Rm_NAme")
    Else
        VSFCommGrdM.TextMatrix(i, 0) = IIf(IsNull(rsMap("Investor_Name")), "", rsMap("Investor_Name"))
        VSFCommGrdM.TextMatrix(i, 1) = rsMap("Branch_Name")
        VSFCommGrdM.TextMatrix(i, 2) = IIf(IsNull(rsMap("PANNO")), "", rsMap("PANNO"))
        VSFCommGrdM.TextMatrix(i, 3) = rsMap("Amc_Name")
        VSFCommGrdM.TextMatrix(i, 4) = rsMap("Scheme_Name")
        VSFCommGrdM.TextMatrix(i, 5) = rsMap("TR_DATE")
        VSFCommGrdM.TextMatrix(i, 6) = IIf(IsNull(rsMap("TRAN_TYPE")), "", rsMap("TRAN_TYPE"))
        VSFCommGrdM.TextMatrix(i, 7) = IIf(IsNull(rsMap("App_No")), "", rsMap("App_No"))
        VSFCommGrdM.TextMatrix(i, 8) = IIf(IsNull(rsMap("PAYMENT_MODE")), "", rsMap("PAYMENT_MODE"))
        VSFCommGrdM.TextMatrix(i, 9) = IIf(IsNull(rsMap("CHEQUE_NO")), 0, rsMap("CHEQUE_NO"))
        VSFCommGrdM.TextMatrix(i, 10) = IIf(IsNull(rsMap("CHEQUE_DATE")), "", rsMap("CHEQUE_DATE"))
        VSFCommGrdM.TextMatrix(i, 11) = rsMap("Amount")
        VSFCommGrdM.TextMatrix(i, 12) = IIf(IsNull(rsMap("sip_TYPE")), "", rsMap("sip_TYPE"))
        VSFCommGrdM.TextMatrix(i, 13) = IIf(IsNull(rsMap("lEAD_nO")), 0, rsMap("lEAD_nO"))
        VSFCommGrdM.TextMatrix(i, 14) = IIf(IsNull(rsMap("LEAD_nAME")), "", rsMap("LEAD_nAME"))
        VSFCommGrdM.TextMatrix(i, 15) = IIf(IsNull(rsMap("TRAN_code")), "", rsMap("TRAN_code"))
        VSFCommGrdM.TextMatrix(i, 16) = rsMap("branch_code")
        VSFCommGrdM.TextMatrix(i, 17) = rsMap("BUSINESS_RMCODE")
        VSFCommGrdM.TextMatrix(i, 18) = rsMap("SCH_CODE")
        VSFCommGrdM.TextMatrix(i, 19) = rsMap("SOURCE_CODE")
        VSFCommGrdM.TextMatrix(i, 20) = IIf(IsNull(rsMap("Bank_Name")), "", rsMap("Bank_Name"))
        VSFCommGrdM.TextMatrix(i, 21) = IIf(IsNull(rsMap("Rm_Name")), "", rsMap("Rm_Name"))
        VSFCommGrdM.TextMatrix(i, 22) = IIf(IsNull(rsMap("folio_no")), "", rsMap("folio_no"))
        VSFCommGrdM.TextMatrix(i, 23) = IIf(IsNull(rsMap("doc_id")), "", rsMap("doc_id"))
        VSFCommGrdM.TextMatrix(i, 24) = IIf(IsNull(rsMap("micro_investment")), "", rsMap("micro_investment"))
        VSFCommGrdM.TextMatrix(i, 25) = IIf(IsNull(rsMap("target_switch_scheme")), "", rsMap("target_switch_scheme"))
        VSFCommGrdM.TextMatrix(i, 26) = IIf(IsNull(rsMap("target_scheme_name")), "", rsMap("target_scheme_name"))
        VSFCommGrdM.TextMatrix(i, 27) = IIf(IsNull(rsMap("switch_scheme_name")), "", rsMap("switch_scheme_name"))
        
    End If
    i = i + 1
    rsMap.MoveNext
    DoEvents
Wend
rsMap.Close
Set rsMap = Nothing
Exit Sub
err1:
    MsgBox err.Description
    'Resume
End Sub
Public Sub SelectText(obj As Object)
    obj.SelStart = 0
    obj.SelLength = Len(obj.Text)
Exit Sub
Errhandler:
    MsgBox err.Description, vbCritical
End Sub

Private Function valid_rm(ClientBranch As String, ctl As TextBox) As Boolean
Dim rsEmp As New ADODB.Recordset
valid_rm = True
If rsEmp.State = 1 Then rsEmp.Close
Set rsEmp = Nothing
End Function

Public Sub Change_bt_Click(Index As Integer)
If Label32.Caption = "" Then
    MsgBox "Information not present", vbInformation, strBajaj
    Exit Sub
Else
    Set frminvaddress_update.currentForm = Nothing
    Set frminvaddress_update.currentForm = frmtransactionmf
    frminvaddress_update.Label32.Caption = Label32.Caption
    frminvaddress_update.Show
    frminvaddress_update.ZOrder 0
End If
End Sub

Private Sub Check1_Click()

End Sub

Private Sub ChkClose_Click()
If TxtSchemeA.Text <> "" Then
    str1 = Split(TxtSchemeA.Text, "=")
    MySchCode = str1(1)
Else
    MySchCode = " "
End If

If ChkClose.Value = 1 Then
    If CmbSipStpA = "SIP" Or CmbSipStpA = "STP" Or SqlRet("select get_sch_status_close_end('" & MySchCode & "') from dual") = 0 Then
        MsgBox "Auto Switch on Maturity can be selected for Close Ended and NON-SIP transaction", vbInformation, "Wealthmaker"
        TxtCloseSch.Text = ""
        ChkClose.Value = 0
        Exit Sub
    End If
Else
    TxtCloseSch.Text = " "
End If

End Sub

Private Sub ChkCloseM_Click()
If TxtSchemeM.Text <> "" Then
    str1 = Split(TxtSchemeM.Text, "=")
    MySchCode = str1(1)
Else
    MySchCode = " "
End If

If ChkCloseM.Value = 1 Then
    If CmbSipStpM = "SIP" Or CmbSipStpM = "STP" Or SqlRet("select get_sch_status_close_end('" & MySchCode & "') from dual") = 0 Then
        MsgBox "Auto Switch on Maturity can be selected on Close Ended and NON-SIP transactions", vbInformation, "Wealthmaker"
        ChkCloseM.Value = 0
        TxtCloseSchM.Text = " "
        Exit Sub
    End If
Else
    TxtCloseSchM.Text = " "
End If
End Sub



Public Sub CmbAmcA_Change()
Call CmbAmcA_Click
TxtSchemeA.Text = ""
End Sub

Private Sub CmbSipStpA_Change()
ChkClose_Click
End Sub

Public Sub CmbSwitchAmcA_Change()
Call CmbSwitchAmcA_Click
TxtSwitchSchemeA.Text = ""
End Sub

Public Sub CmbAmcA_Click()
Dim StrAmc() As String
Dim LvwItem As ListItem
If rsAmc.State = 1 Then rsAmc.Close
rsAmc.open "select mut_code from mut_fund where upper(trim(mut_name))='" & UCase(Trim(CmbAmcA.Text)) & "'", MyConn
If rsAmc.EOF = True Then
    If rsAmc.State = 1 Then rsAmc.Close
    rsAmc.open "select iss_code from iss_master where upper(trim(iss_name))='" & UCase(Trim(CmbAmcA.Text)) & "'", MyConn
    If rsAmc.EOF = False Then
        MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select osch_code,osch_name from other_product where iss_code='" & MyAmcCode & "' order by osch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameA.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameA.AddItem rsscheme.Fields("osch_name") & Space(100) & "=" & rsscheme.Fields("osch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameA.Clear
    End If
Else
    If rsAmc.EOF = False Then
        MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select sch_code,sch_name from scheme_info where mut_code='" & rsAmc.Fields("mut_code") & "' order by sch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameA.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameA.AddItem rsscheme.Fields("sch_name") & Space(100) & "=" & rsscheme.Fields("sch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameA.Clear
    End If
End If
End Sub
Public Sub CmbSwitchAmcA_Click()
Dim StrAmc() As String
Dim LvwItem As ListItem
If rsAmc.State = 1 Then rsAmc.Close
rsAmc.open "select mut_code from mut_fund where upper(trim(mut_name))='" & UCase(Trim(CmbSwitchAmcA.Text)) & "'", MyConn
If rsAmc.EOF = True Then
    If rsAmc.State = 1 Then rsAmc.Close
    rsAmc.open "select iss_code from iss_master where upper(trim(iss_name))='" & UCase(Trim(CmbSwitchAmcA.Text)) & "'", MyConn
    If rsAmc.EOF = False Then
        'MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select osch_code,osch_name from other_product where iss_code='" & MyAmcCode & "' order by osch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameA.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameA.AddItem rsscheme.Fields("osch_name") & Space(100) & "=" & rsscheme.Fields("osch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameA.Clear
    End If
Else
    If rsAmc.EOF = False Then
        'MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select sch_code,sch_name from scheme_info where mut_code='" & rsAmc.Fields("mut_code") & "' order by sch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameA.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameA.AddItem rsscheme.Fields("sch_name") & Space(100) & "=" & rsscheme.Fields("sch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameA.Clear
    End If
End If
End Sub
Public Sub CmbAmcm_Change()
Call CmbAmcm_Click
TxtSchemeM.Text = ""
End Sub
Public Sub CmbSwitchAmcM_Change()
Call CmbSwitchAmcM_Click
TxtSwitchSchemeM.Text = ""
End Sub

Public Sub CmbAmcm_Click()
Dim StrAmc() As String
Dim LvwItem As ListItem
TxtSchemeM.Text = ""
If rsAmc.State = 1 Then rsAmc.Close
rsAmc.open "select mut_code from mut_fund where upper(trim(mut_name))='" & UCase(Trim(CmbAmcM.Text)) & "'", MyConn
If rsAmc.EOF = True Then
    If rsAmc.State = 1 Then rsAmc.Close
    rsAmc.open "select iss_code from iss_master where upper(trim(iss_name))='" & UCase(Trim(CmbAmcM.Text)) & "'", MyConn
    If rsAmc.EOF = False Then
        MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select osch_code,osch_name from other_product where iss_code='" & MyAmcCode & "' order by osch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameM.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameM.AddItem rsscheme.Fields("osch_name") & Space(100) & "=" & rsscheme.Fields("osch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameM.Clear
    End If
Else
    If rsAmc.EOF = False Then
        MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select sch_code,sch_name from scheme_info where mut_code='" & rsAmc.Fields("mut_code") & "' order by sch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameM.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameM.AddItem rsscheme.Fields("sch_name") & Space(100) & "=" & rsscheme.Fields("sch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameM.Clear
    End If
End If
End Sub

Public Sub CmbSwitchAmcM_Click()
Dim StrAmc() As String
Dim LvwItem As ListItem
If rsAmc.State = 1 Then rsAmc.Close
rsAmc.open "select mut_code from mut_fund where upper(trim(mut_name))='" & UCase(Trim(CmbSwitchAmcM.Text)) & "'", MyConn
If rsAmc.EOF = True Then
    If rsAmc.State = 1 Then rsAmc.Close
    rsAmc.open "select iss_code from iss_master where upper(trim(iss_name))='" & UCase(Trim(CmbSwitchAmcM.Text)) & "'", MyConn
    If rsAmc.EOF = False Then
        'MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select osch_code,osch_name from other_product where iss_code='" & MyAmcCode & "' order by osch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameM.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameM.AddItem rsscheme.Fields("osch_name") & Space(100) & "=" & rsscheme.Fields("osch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameM.Clear
    End If
Else
    If rsAmc.EOF = False Then
        'MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select sch_code,sch_name from scheme_info where mut_code='" & rsAmc.Fields("mut_code") & "' order by sch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameM.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameM.AddItem rsscheme.Fields("sch_name") & Space(100) & "=" & rsscheme.Fields("sch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameM.Clear
    End If
End If
End Sub

Private Sub Clear_Previously_Selected()
    On Error GoTo Bot
    Dim KCount_cOL As Integer
    If Glb_Flag_First_Time = False Then
        Exit Sub
    End If
    If Glb_Selected_row <> 0 Then
         VSFCommGrdM.Row = Glb_Selected_row
         For KCount_cOL = 1 To VSFCommGrdM.Cols - 1
            VSFCommGrdM.Col = KCount_cOL
            VSFCommGrdM.CellBackColor = vbWhite
            VSFCommGrdM.FontBold = True
            VSFCommGrdM.CellForeColor = vbBlack
            VSFCommGrdM.CellFontBold = False
        Next
    End If
    Exit Sub
Bot:
    'Call ShowErrorMessage(err.Description)
End Sub


Private Sub cmbBankName_DropDown()
Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
cmbBankName.Clear
While Not rsAmc.EOF
  cmbBankName.AddItem rsAmc(0)
  rsAmc.MoveNext
Wend
rsAmc.Close
End Sub

Private Sub cmbBankNameM_DropDown()
Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
cmbBankNameM.Clear
While Not rsAmc.EOF
  cmbBankNameM.AddItem rsAmc(0)
  rsAmc.MoveNext
Wend
rsAmc.Close
End Sub


Private Sub CmbFrequency_Click()
'vinit 31-oct-2015
ImSipEndDtA.Text = "__/__/____"

MaxInstallmentNo = SqlRet("select round(months_between(to_date('01/12/2099','DD/MM/RRRR'),to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'))) from dual")

If Opt99.Value = True And ImSipStartDtA.Text <> "__/__/____" Then
     ImSipEndDtA.Text = "01/12/2099"
End If
        
If CmbFrequency.ListIndex <> -1 And Opt99.Value = False Then
    MyFreq = Split(CmbFrequency.Text, "#")
    If MyFreq(1) = "208" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'), to_number(" & TxtInstallmens.Text & "/30))-1 as enddt from dual ")
        End If
    ElseIf MyFreq(1) = "173" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select WEALTHMAKER.add_weeks(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'), to_number(" & TxtInstallmens.Text & ")) as enddt  from dual ")
        End If
    ElseIf MyFreq(1) = "174" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),6* " & TxtInstallmens.Text & ")-1 as enddt from dual ")
        End If
    ElseIf MyFreq(1) = "175" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            If Val(TxtInstallmens.Text) < MaxInstallmentNo Then
                ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),1* " & TxtInstallmens.Text & ")-1  as enddt from dual ")
            Else
                ImSipEndDtA.Text = "01/12/2099"
            End If
        End If
    ElseIf MyFreq(1) = "176" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),3* " & TxtInstallmens.Text & ")-1  as enddt from dual ")
        End If
    ElseIf MyFreq(1) = "301" Then
        If (TxtInstallmens.Text <> "" Or Opt99.Value = True) And ImSipStartDtA.Text <> "__/__/____" Then
            ImSipEndDtA.Text = SqlRet("select add_months(to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'),12* " & TxtInstallmens.Text & ")-1  as enddt from dual ")
        End If
    End If
End If
End Sub

Private Sub CmbFrequency1_Click()

ImSipEndDtM.Text = "__/__/____"
'MyFreq1 = ""

MaxInstallmentNo = SqlRet("select round(months_between(to_date('01/12/2099','DD/MM/RRRR'),to_date('" & ImSipStartDtA.Text & "','DD/MM/RRRR'))) from dual")

If Opt991.Value = True And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = "01/12/2099"
End If


If CmbFrequency1.ListIndex <> -1 And Opt991.Value = False Then
    MyFreq1 = Split(CmbFrequency1.Text, "#")
    If MyFreq1(1) = "208" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'), to_number(" & Txtinstallments1.Text & "/30))-1 as enddt from dual ")
        End If
    ElseIf MyFreq1(1) = "173" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select to_date(add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'), to_number(" & Txtinstallments1.Text & "/4))-1) as enddt  from dual ")
        End If
    ElseIf MyFreq1(1) = "174" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),6* " & Txtinstallments1.Text & ")-1 as enddt from dual ")
        End If
    ElseIf MyFreq1(1) = "175" Then
'        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
'            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),1* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
'        End If
        
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            If Val(Txtinstallments1.Text) < MaxInstallmentNo Then
                ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),1* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
            Else
                ImSipEndDtM.Text = "01/12/2099"
            End If
        End If
    ElseIf MyFreq1(1) = "176" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),3* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
        End If
    ElseIf MyFreq1(1) = "301" Then
        If (Txtinstallments1.Text <> "" Or Opt991.Value = True) And ImSipStartDtM.Text <> "__/__/____" Then
            ImSipEndDtM.Text = SqlRet("select add_months(to_date('" & ImSipStartDtM.Text & "','DD/MM/RRRR'),12* " & Txtinstallments1.Text & ")-1  as enddt from dual ")
        End If
    End If
End If

 
End Sub


Private Sub CmbSipStpA_Click()
    If CmbSipStpA.Text = "SIP" Or CmbSipStpA.Text = "STP" Then
        CmbFrequency.Enabled = True
        Opt99.Enabled = True
        TxtInstallmens.Enabled = True
    Else
        CmbFrequency.ListIndex = -1
        TxtInstallmens.Text = ""
        CmbFrequency.Enabled = False
        Opt99.Enabled = False
        TxtInstallmens.Enabled = False
    End If
    
    If CmbSipStpA.Text = "STP" Then
        Frame11.Enabled = True
'        OptChkSwitch.Visible = False
'        TxtSwitchFolioA.SetFocus
        CmbFrequency.ListIndex = -1
'        OptOthers.Value = True
'        Frame11.Enabled = True
        Label6.Item(4).Enabled = True
        Label6.Item(5).Enabled = True
    End If
    
    If CmbSipStpA.Text = "SIP" Then
        LblSipType.Visible = True
        CmbSubSipA.Visible = True
        CmbSubSipA.ListIndex = 0
        Frame17.Visible = True
        OptsipR.Visible = True
        OptsipF.Visible = True
        OptsipF.Value = True
        Label8.Visible = True
        txtSipAmount.Visible = True
     Else
        LblSipType.Visible = False
        CmbSubSipA.Visible = False
        Frame17.Visible = False
        OptsipF.Value = True
        OptsipR.Visible = False
        OptsipF.Visible = False
        Label8.Visible = False
        txtSipAmount.Visible = False
    End If
     If CmbSipStpA.Text = "REGULAR" Then
        Frame22.Visible = True
     Else
        Frame22.Visible = False
    End If
 ChkClose_Click
End Sub

Private Sub CmbSipStpM_Click()
ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   
   Label2(8).Visible = False
   Label2(10).Visible = False

If CmbSipStpM.Text = "SIP" Or CmbSipStpM.Text = "STP" Then
    CmbFrequency1.Enabled = True
    Opt991.Enabled = True
    Txtinstallments1.Enabled = True
    ImSipStartDtM.Visible = True
   ImSipEndDtM.Visible = True
    Label2(8).Visible = True
   Label2(10).Visible = True
Else
    CmbFrequency1.ListIndex = -1
    Txtinstallments1.Text = ""
    CmbFrequency1.Enabled = False
    Opt991.Enabled = False
    Txtinstallments1.Enabled = False
    ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   Label2(8).Visible = False
   Label2(10).Visible = False
End If
If CmbSipStpM.Text = "STP" Then
   Frame3.Visible = False
   Frame12.Visible = True
   OptChkSwitchM.Visible = True
   TxtSwitchFolioM.SetFocus
End If
If CmbSipStpM.Text = "SIP" Then
    OptSIPRM.Visible = True
    OptSIPFM.Visible = True
    OptSIPFM.Value = True
    ImSipStartDtM.Visible = True
   ImSipEndDtM.Visible = True
    Label2(8).Visible = True
   Label2(10).Visible = True
Else
    OptSIPFM.Value = True
    OptSIPRM.Visible = False
    OptSIPFM.Visible = False
    ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   Label2(8).Visible = False
   Label2(10).Visible = False
End If
ChkCloseM_Click
End Sub




Private Sub cmbTranTypeA_Click()
CmbSipStpA.ListIndex = -1
ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
ImSipEndDtA.Text = "__/__/____"
CmbSubSipA.ListIndex = -1
CmbFrequency.ListIndex = -1
Opt99.Value = False
If cmbTranTypeA.Text = "PURCHASE" Then
   'Frame2.Visible = True
   OptChkSwitch.Value = 0
   Frame11.Enabled = False
   'Frame15.Enabled = False
   Frame11.Enabled = False
   Label6.Item(4).Enabled = False
   Label6.Item(5).Enabled = False
   
   CmbSipStpA.Enabled = True
   ImSipStartDtA.Enabled = True
   ImSipEndDtA.Enabled = True
   CmbSubSipA.Enabled = True
   CmbFrequency.Enabled = True
   Opt99.Enabled = True
   TxtSwitchFolioA.Text = ""
   TxtSwitchSchemeA.Text = ""
   'Frame11.BackColor = RGB(255, 255, 222)
   'Label6.Item(4).BackColor = RGB(255, 255, 222)
   'Label6.Item(5).BackColor = RGB(255, 255, 222)
ElseIf cmbTranTypeA.Text = "SWITCH IN" Then
   'Frame2.Visible = False
   Frame11.Enabled = True
   OptChkSwitch.Visible = False
   TxtSwitchFolioA.SetFocus
   CmbFrequency.ListIndex = -1
   OptOthers.Value = True
   'Frame15.Enabled = True
   Frame11.Enabled = True
   Label6.Item(4).Enabled = True
   Label6.Item(5).Enabled = True
   
   CmbSipStpA.Enabled = False
   ImSipStartDtA.Enabled = False
   ImSipEndDtA.Enabled = False
   CmbSubSipA.Enabled = False
   CmbFrequency.Enabled = False
   Opt99.Enabled = False
   'Frame11.BackColor = RGB(128, 128, 128)
   'Label6.Item(4).BackColor = RGB(128, 128, 128)
   'Label6.Item(5).BackColor = RGB(128, 128, 128)
End If
End Sub


Private Sub cmbTranTypeM_Change()
   ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
   ImSipEndDtM.Text = "__/__/____"
   CmbFrequency1.ListIndex = -1
    Opt991.Value = False
If cmbTranTypeM.Text = "PURCHASE" Then
   Frame3.Visible = True
   Frame12.Visible = False
   OptChkSwitchM.Visible = False
   
   
   
ElseIf cmbTranTypeM.Text = "SWITCH IN" Then
   Frame3.Visible = False
   Frame12.Visible = True
   OptChkSwitchM.Visible = True
   TxtSwitchFolioM.SetFocus
   
End If
End Sub

Private Sub cmbTranTypeM_Click()
   ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
   
   Label2(8).Visible = False
   Label2(10).Visible = False
   
   
   ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
   ImSipEndDtM.Text = "__/__/____"
   CmbFrequency1.ListIndex = -1
    Opt991.Value = False
If cmbTranTypeM.Text = "PURCHASE" Then
   Frame3.Visible = True
   Frame12.Visible = False
   OptChkSwitchM.Visible = False
   ImSipStartDtM.Visible = True
   ImSipEndDtM.Visible = True
   Label2(8).Visible = True
   Label2(10).Visible = True
   
   
ElseIf cmbTranTypeM.Text = "SWITCH IN" Then
   Frame3.Visible = False
   Frame12.Visible = True
   OptChkSwitchM.Visible = True
   TxtSwitchFolioM.SetFocus
   ImSipStartDtM.Visible = False
   ImSipEndDtM.Visible = False
    Label2(8).Visible = False
   Label2(10).Visible = False
End If
End Sub


Private Sub cmdArPrint_Click()
If MyPrintTranCode = "" Then
    MsgBox "Ar Can Not Be Print Right Now Please Generate The AR"
    Exit Sub
End If
If Mid(Trim(Label32.Caption), 1, 1) = "4" Then
    sql = " CREATE OR REPLACE VIEW armf AS SELECT b.client_code,'P' ar_type, t.tran_code, TR_DATE, CHEQUE_DATE cheque_date, "
    sql = sql & "          CHEQUE_NO cheque_no, t.bank_name, amount, b.client_code source_code, "
    sql = sql & "          app_no, NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, "
    sql = sql & "          NVL ((SELECT SUM (NVL (amt, 0)) "
    sql = sql & "                  FROM payment_detail "
    sql = sql & "                 WHERE tran_code = t.tran_code), 0) paidamt, '' asr, PAYMENT_MODE, "
    sql = sql & "          investor_name inv, (SELECT MAX (client_name) "
    sql = sql & "                                FROM client_master "
    sql = sql & "                               WHERE client_code = t.source_code) client, "
    sql = sql & "          exist_code as existcode, "
    sql = sql & "           address1  add1, "
    sql = sql & "          address2 add2, '' loc, "
    sql = sql & "          pincode pin, "
    sql = sql & "          (SELECT MAX (city_name) "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT MAX (city_id) "
    sql = sql & "                               FROM client_master "
    sql = sql & "                              WHERE client_code = t.source_code)) ccity, "
    sql = sql & "          mobile ph, "
    sql = sql & "           email, 0 arn, '' subbroker, "
    sql = sql & "          (SELECT rm_name "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rname, "
    sql = sql & "          (SELECT payroll_id "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rcode, "
    sql = sql & "          (SELECT branch_name "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bname, "
    sql = sql & "          (SELECT address1 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd1, "
    sql = sql & "          (SELECT address2 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd2, "
    sql = sql & "          (SELECT phone "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bphone, "
    sql = sql & "          (SELECT location_name "
    sql = sql & "             FROM location_master "
    sql = sql & "            WHERE location_id = (SELECT location_id "
    sql = sql & "                                   FROM branch_master "
    sql = sql & "                                  WHERE branch_code = t.BUSI_BRANCH_CODE)) bloc, "
    sql = sql & "          (SELECT city_name "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT city_id "
    sql = sql & "                               FROM branch_master "
    sql = sql & "                              WHERE branch_code = t.BUSI_BRANCH_CODE)) bcity, "
    sql = sql & "          (SELECT mut_name "
    sql = sql & "             FROM mut_fund "
    sql = sql & "            WHERE mut_code = t.MUT_CODE) compmf, "
    sql = sql & "          'Bajaj Capital Limited' compgroup, "
    sql = sql & "          (SELECT sch_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) schmf, "
    sql = sql & "          (SELECT short_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) sschmf  "
    sql = sql & " , ('" & Glbloginid & "') LOGGEDUSERID "
    sql = sql & "     FROM transaction_mf_Temp1 t,client_master b "
    sql = sql & "    WHERE t.source_code=b.client_code and TR_DATE = to_date('" & MyDate(ImEntryDtA) & "','mm/dd/yyyy')"
    sql = sql & "      AND (asa <> 'C' OR asa IS NULL) "
    sql = sql & "      AND BUSINESS_RMCODE = " & Val(TxtBusiCodeA.Text) & " "
    sql = sql & "      AND SOURCE_CODE = '" & TxtClientCodeA.Text & "' "
    sql = sql & "      AND tran_code = '" & MyPrintTranCode & "' "
Else
    sql = " CREATE OR REPLACE VIEW armf AS SELECT b.agent_code,'P' ar_type, t.tran_code, TR_DATE, CHEQUE_DATE cheque_date, "
    sql = sql & "          CHEQUE_NO cheque_no, t.bank_name, amount, b.agent_code source_code, "
    sql = sql & "          app_no, NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, "
    sql = sql & "          NVL ((SELECT SUM (NVL (amt, 0)) "
    sql = sql & "                  FROM payment_detail "
    sql = sql & "                 WHERE tran_code = t.tran_code), 0) paidamt, '' asr, PAYMENT_MODE, "
    sql = sql & "          investor_name inv, (SELECT MAX (agent_name) "
    sql = sql & "                                FROM agent_master "
    sql = sql & "                               WHERE agent_code = t.source_code) client, "
    sql = sql & "          exist_code as existcode, "
    sql = sql & "           address1  add1, "
    sql = sql & "          address2 add2, '' loc, "
    sql = sql & "          pincode pin, "
    sql = sql & "          (SELECT MAX (city_name) "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT MAX (city_id) "
    sql = sql & "                               FROM agent_master "
    sql = sql & "                              WHERE agent_code = t.source_code)) ccity, "
    sql = sql & "          mobile ph, "
    sql = sql & "           email, 0 arn, '' subbroker, "
    sql = sql & "          (SELECT rm_name "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rname, "
    sql = sql & "          (SELECT payroll_id "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rcode, "
    sql = sql & "          (SELECT branch_name "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bname, "
    sql = sql & "          (SELECT address1 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd1, "
    sql = sql & "          (SELECT address2 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd2, "
    sql = sql & "          (SELECT phone "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bphone, "
    sql = sql & "          (SELECT location_name "
    sql = sql & "             FROM location_master "
    sql = sql & "            WHERE location_id = (SELECT location_id "
    sql = sql & "                                   FROM branch_master "
    sql = sql & "                                  WHERE branch_code = t.BUSI_BRANCH_CODE)) bloc, "
    sql = sql & "          (SELECT city_name "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT city_id "
    sql = sql & "                               FROM branch_master "
    sql = sql & "                              WHERE branch_code = t.BUSI_BRANCH_CODE)) bcity, "
    sql = sql & "          (SELECT mut_name "
    sql = sql & "             FROM mut_fund "
    sql = sql & "            WHERE mut_code = t.MUT_CODE) compmf, "
    sql = sql & "          'Bajaj Capital Limited' compgroup, "
    sql = sql & "          (SELECT sch_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) schmf, "
    sql = sql & "          (SELECT short_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) sschmf  "
    sql = sql & " , ('" & Glbloginid & "') LOGGEDUSERID "
    sql = sql & "     FROM transaction_mf_Temp1 t,agent_master b "
    sql = sql & "    WHERE t.source_code=b.agent_code and TR_DATE = to_date('" & MyDate(ImEntryDtA) & "','mm/dd/yyyy')"
    sql = sql & "      AND (asa <> 'C' OR asa IS NULL) "
    sql = sql & "      AND BUSINESS_RMCODE = " & Val(TxtBusiCodeA.Text) & " "
    sql = sql & "      AND SOURCE_CODE = '" & TxtClientCodeA.Text & "' "
    sql = sql & "      AND tran_code = '" & MyPrintTranCode & "' "
End If
MyConn.Execute sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "TEST", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\MFAr1.rpt"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.action = 1
End Sub

Private Sub CmdArPrintM_Click()
If MyPrintTranCode = "" Then
   MsgBox "Double Click The Record Which You Want To Print"
   Exit Sub
End If
If Mid(Trim(TxtClientCodeM), 1, 1) = "4" Then
    sql = " CREATE OR REPLACE VIEW armf AS SELECT 'P' ar_type, t.tran_code, TR_DATE, CHEQUE_DATE cheque_date, "
    sql = sql & "          CHEQUE_NO cheque_no, t.bank_name, amount, b.client_code source_code, "
    sql = sql & "          app_no, NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, "
    sql = sql & "          NVL ((SELECT SUM (NVL (amt, 0)) "
    sql = sql & "                  FROM payment_detail "
    sql = sql & "                 WHERE tran_code = t.tran_code), 0) paidamt, '' asr, PAYMENT_MODE, "
    sql = sql & "          investor_name inv, (SELECT MAX (client_name) "
    sql = sql & "                                FROM client_master "
    sql = sql & "                               WHERE client_code = t.source_code) client, "
    sql = sql & "          exist_code as existcode, "
    sql = sql & "           address1  add1, "
    sql = sql & "          address2 add2, '' loc, "
    sql = sql & "          pincode pin, "
    sql = sql & "          (SELECT MAX (city_name) "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT MAX (city_id) "
    sql = sql & "                               FROM client_master "
    sql = sql & "                              WHERE client_code = t.source_code)) ccity, "
    sql = sql & "          mobile ph, "
    sql = sql & "           email, 0 arn, '' subbroker, "
    sql = sql & "          (SELECT rm_name "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rname, "
    sql = sql & "          (SELECT payroll_id "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rcode, "
    sql = sql & "          (SELECT branch_name "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bname, "
    sql = sql & "          (SELECT address1 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd1, "
    sql = sql & "          (SELECT address2 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd2, "
    sql = sql & "          (SELECT phone "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bphone, "
    sql = sql & "          (SELECT location_name "
    sql = sql & "             FROM location_master "
    sql = sql & "            WHERE location_id = (SELECT location_id "
    sql = sql & "                                   FROM branch_master "
    sql = sql & "                                  WHERE branch_code = t.BUSI_BRANCH_CODE)) bloc, "
    sql = sql & "          (SELECT city_name "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT city_id "
    sql = sql & "                               FROM branch_master "
    sql = sql & "                              WHERE branch_code = t.BUSI_BRANCH_CODE)) bcity, "
    sql = sql & "          (SELECT mut_name "
    sql = sql & "             FROM mut_fund "
    sql = sql & "            WHERE mut_code = t.MUT_CODE) compmf, "
    sql = sql & "          'Bajaj Capital Limited' compgroup, "
    sql = sql & "          (SELECT sch_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) schmf, "
    sql = sql & "          (SELECT short_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) sschmf  "
    sql = sql & " , ('" & Glbloginid & "') LOGGEDUSERID "
    sql = sql & "     FROM transaction_mf_Temp1 t,client_master b "
    sql = sql & "    WHERE t.source_code=b.client_code and TR_DATE = to_date('" & MyDate(ImEntryDtM) & "','mm/dd/yyyy')"
    sql = sql & "      AND MOVE_FLAG1 IS NULL "
    sql = sql & "      AND (asa <> 'C' OR asa IS NULL) "
    sql = sql & "      AND BUSINESS_RMCODE = " & Val(TxtBusiCodeM.Text) & " "
    sql = sql & "      AND SOURCE_CODE = '" & TxtClientCodeM.Text & "' "
    If MsgBox("Multiple Transactions Print?", vbYesNo) = vbNo Then
        sql = sql & "      AND tran_code = '" & MyPrintTranCode & "' "
    End If
Else
    sql = " CREATE OR REPLACE VIEW armf AS SELECT b.agent_code,'P' ar_type, t.tran_code, TR_DATE, CHEQUE_DATE cheque_date, "
    sql = sql & "          CHEQUE_NO cheque_no, t.bank_name, amount, b.agent_code source_code, "
    sql = sql & "          app_no, NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, "
    sql = sql & "          NVL ((SELECT SUM (NVL (amt, 0)) "
    sql = sql & "                  FROM payment_detail "
    sql = sql & "                 WHERE tran_code = t.tran_code), 0) paidamt, '' asr, PAYMENT_MODE, "
    sql = sql & "          investor_name inv, (SELECT MAX (agent_name) "
    sql = sql & "                                FROM agent_master "
    sql = sql & "                               WHERE agent_code = t.source_code) client, "
    sql = sql & "          exist_code as existcode, "
    sql = sql & "           address1  add1, "
    sql = sql & "          address2 add2, '' loc, "
    sql = sql & "          pincode pin, "
    sql = sql & "          (SELECT MAX (city_name) "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT MAX (city_id) "
    sql = sql & "                               FROM agent_master "
    sql = sql & "                              WHERE agent_code = t.source_code)) ccity, "
    sql = sql & "          mobile ph, "
    sql = sql & "           email, 0 arn, '' subbroker, "
    sql = sql & "          (SELECT rm_name "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rname, "
    sql = sql & "          (SELECT payroll_id "
    sql = sql & "             FROM employee_master "
    sql = sql & "            WHERE payroll_id = TO_CHAR (t.BUSINESS_RMCODE) "
    sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rcode, "
    sql = sql & "          (SELECT branch_name "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bname, "
    sql = sql & "          (SELECT address1 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd1, "
    sql = sql & "          (SELECT address2 "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) badd2, "
    sql = sql & "          (SELECT phone "
    sql = sql & "             FROM branch_master "
    sql = sql & "            WHERE branch_code = t.BUSI_BRANCH_CODE) bphone, "
    sql = sql & "          (SELECT location_name "
    sql = sql & "             FROM location_master "
    sql = sql & "            WHERE location_id = (SELECT location_id "
    sql = sql & "                                   FROM branch_master "
    sql = sql & "                                  WHERE branch_code = t.BUSI_BRANCH_CODE)) bloc, "
    sql = sql & "          (SELECT city_name "
    sql = sql & "             FROM city_master "
    sql = sql & "            WHERE city_id = (SELECT city_id "
    sql = sql & "                               FROM branch_master "
    sql = sql & "                              WHERE branch_code = t.BUSI_BRANCH_CODE)) bcity, "
    sql = sql & "          (SELECT mut_name "
    sql = sql & "             FROM mut_fund "
    sql = sql & "            WHERE mut_code = t.MUT_CODE) compmf, "
    sql = sql & "          'Bajaj Capital Limited' compgroup, "
    sql = sql & "          (SELECT sch_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) schmf, "
    sql = sql & "          (SELECT short_name "
    sql = sql & "             FROM scheme_info "
    sql = sql & "            WHERE sch_code = t.SCH_CODE) sschmf  "
    sql = sql & " , ('" & Glbloginid & "') LOGGEDUSERID "
    sql = sql & "     FROM transaction_mf_Temp1 t,agent_master b "
    sql = sql & "    WHERE t.source_code=b.agent_code and TR_DATE = to_date('" & MyDate(ImEntryDtM) & "','mm/dd/yyyy')"
    sql = sql & "      AND MOVE_FLAG1 IS NULL "
    sql = sql & "      AND (asa <> 'C' OR asa IS NULL) "
    sql = sql & "      AND BUSINESS_RMCODE = " & Val(TxtBusiCodeM.Text) & " "
    sql = sql & "      AND SOURCE_CODE = '" & TxtClientCodeM.Text & "' "
    If MsgBox("Multiple Transactions Print?", vbYesNo) = vbNo Then
        sql = sql & "      AND tran_code = '" & MyPrintTranCode & "' "
    End If
End If
MyConn.Execute sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "TEST", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\MFAr1.rpt"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.action = 1
End Sub

Private Sub cmdCancel_Click()
 fracancel.Visible = False
End Sub

Private Sub CmdClientSearch_Click()

TxtAcHolderA.Text = ""
 treeselected = "N"
 Set frmtree_searchnewMF.currentForm = Nothing
 Set frmtree_searchnewMF.currentForm = frmtransactionmf
 frmtree_searchnewMF.treeName = "Treeclient"
 If MyCurrentBranchCode <> "" Then
     BrCode = MyCurrentBranchCode
 End If
Load frmtree_searchnewMF
frmtree_searchnewMF.Ar_type = ""
frmtree_searchnewMF.Cmbcat.ListIndex = 1
frmtree_searchnewMF.Cmbcat.Enabled = False
frmtree_searchnewMF.CmbClientBroker.Visible = True
frmtree_searchnewMF.CmbClientBroker.ListIndex = 0
frmtree_searchnewMF.lblOF.Visible = True
frmtree_searchnewMF.Show vbModal

If Label32.Caption <> "" Then
    Change_bt_Click (0)
End If
End Sub
Private Sub CmdClientSearchM_Click()
TxtAcHolderA.Text = ""
 treeselected = "N"
 Set frmtree_searchnewMF.currentForm = Nothing
 Set frmtree_searchnewMF.currentForm = frmtransactionmf
 frmtree_searchnewMF.treeName = "Treeclient"
 If MyCurrentBranchCode <> "" Then
     BrCode = MyCurrentBranchCode
 End If
Load frmtree_searchnewMF
frmtree_searchnewMF.Ar_type = ""
frmtree_searchnewMF.Cmbcat.ListIndex = 1
frmtree_searchnewMF.Cmbcat.Enabled = False
frmtree_searchnewMF.CmbClientBroker.Visible = True
frmtree_searchnewMF.CmbClientBroker.ListIndex = 0
frmtree_searchnewMF.lblOF.Visible = True
frmtree_searchnewMF.Show vbModal
End Sub

Private Sub cmdExit_Click(Index As Integer)
If frmtransactionmf.TxtClientCodeA <> "" Then
        Lead_Form = "All"
        Load frm_leadtran_search
        frm_leadtran_search.Show
        'frm_leadtran_search.Visible = True
        'frm_leadtran_search.cmdSearch_Click
    Else
        MsgBox "Please Select Investor First", vbInformation
    End If
End Sub

Private Sub CmdExitA_Click(Index As Integer)
Unload Me
End Sub

Private Sub CmdExitM_Click()
Unload Me
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
     For kCount = Glb_L_SearchIndex To VSFCommGrdM.Rows - 1
        If InStr(1, UCase(VSFCommGrdM.TextMatrix(kCount, ColumnIndex)), UCase(txtFind.Text)) > 0 Then
            Glb_Comming_From_Search = True
            VSFCommGrdM.Row = kCount
            If kCount > 12 Then VSFCommGrdM.TopRow = kCount
            For KCount_cOL = 1 To VSFCommGrdM.Cols - 1
                Glb_Comming_From_Search = True
                VSFCommGrdM.Col = KCount_cOL
                VSFCommGrdM.CellBackColor = vbWhite
                VSFCommGrdM.FontBold = True
                VSFCommGrdM.CellForeColor = vbRed
                VSFCommGrdM.CellFontBold = True
            Next
            Glb_L_SearchIndex = kCount + 1
            Glb_Selected_row = kCount
            VSFCommGrdM.TopRow = kCount
            Exit Sub
       End If
    Next
    If kCount >= VSFCommGrdM.Rows - 1 Then
        MsgBox "No Such Record Found", vbInformation, "Search Completed"
        txtFind.Text = ""
        txtFind.SetFocus
        Glb_L_SearchIndex = 1
        Glb_Selected_row = 0
    End If
End Sub

Private Sub cmdOk_Click()
Dim Get_DetailLedger As ADODB.Recordset
Dim Get_paymentCode As ADODB.Recordset
Dim rsTran As New ADODB.Recordset
Dim rstran1 As New ADODB.Recordset
Dim Reason() As String
If IsDate(mdatecancel.Text) = False Then
    MsgBox "Invalid Cancel Date", vbInformation, "Wealthmaker"
    mdatecancel.SetFocus
    Exit Sub
End If
If CmbReson.Text = "" Then
    MsgBox "Select Any Reason", vbExclamation, "Wealthmaker"
    CmbReson.SetFocus
    Exit Sub
End If
If TxtReason.Text = "" Then
    MsgBox "Please Write Down The Remark .", vbInformation, "Wealthmaker"
    TxtReason.SetFocus
    Exit Sub
End If

'---------------------------------------------Should Not be Cancel if transaction is reconciled--------------------------------------
Set rsTran = MyConn.Execute("SELECT * FROM TRANSACTION_MF_TEMP1 WHERE TRAN_CODE='" & txtcanTrcode.Text & "'")
If Not rsTran.EOF Then
      If rsTran.Fields("rec_flag") = "Y" Then
         MsgBox "The AR is reconciled, It can not be canceled.", vbInformation, "Wealthmaker"
         mdatecancel.SetFocus
         Exit Sub
      End If
End If
Set rsTran = Nothing

'------------------------------------------------------------------------------------------------------------------------------------
If CDate(DateValue(mdatecancel.Text)) < Format(CDate(DateValue(Glbins_previousdate)), "DD-MM-YYYY") Or CDate(DateValue(mdatecancel.Text)) > Format(CDate(DateValue(Glbins_nextdate)), "DD-MM-YYYY") Then
    MsgBox "Security restrictions for date range"
    mdatecancel.SetFocus
    Exit Sub
Else
    Reason = Split(CmbReson.Text, "$")
    MyConn.Execute ("insert into tran_cancel(tran_code,cancel_date,remark,reason,T_TYPE,reason_type,cancel_by) values('" & txtcanTrcode.Text & "',to_date('" & mdatecancel.Text & "','dd-mm-yyyy'),'" & TxtReason.Text & "','" & Trim(Reason(0)) & "','MFTEMP','" & Trim(Reason(1)) & "','" & Glbloginid & "')")
    MyConn.Execute ("update TRANSACTION_MF_TEMP1 set asa='C',tran_type='REVERTAL' where tran_code='" & txtcanTrcode.Text & "'")
    'CANCEL THE SIP ADV ALSO
    rsTran.open "SELECT * FROM TRANSACTION_MF_TEMP1 WHERE SIP_ID IS NOT NULL AND BASE_TRAN_CODE='" & txtcanTrcode.Text & "'", MyConn, adOpenForwardOnly
    If Not rsTran.EOF Then
        MyConn.Execute ("update TRANSACTION_MF_TEMP1 set asa='C',tran_type='REVERTAL' where BASE_TRAN_CODE='" & txtcanTrcode.Text & "'")
    End If
End If
'-----------------------------New Cancellation Log by Mayank---------------------------------
Dim Can_Log As New ADODB.Command
Set Can_Log.ActiveConnection = MyConn
Can_Log.CommandType = adCmdStoredProc
Can_Log.CommandText = "PRC_DIRECTCLIENT_CANREC_GLB"
Can_Log.Parameters.Append Can_Log.CreateParameter("AR", adVarChar, adParamInput, 50, txtcanTrcode.Text)
Can_Log.Parameters.Append Can_Log.CreateParameter("USERID", adVarChar, adParamInput, 50, Glbloginid)
Can_Log.Execute
Set Can_Log = Nothing
'--------------------------------------------------------------------------------------------
MsgBox "Transaction Cancelled Successfully", vbInformation, "Wealthmaker"
txtcanTrcode.Text = ""
mdatecancel.Text = "__/__/____"
TxtReason.Text = ""
CmbReson.Clear
fracancel.Visible = False
End Sub

Private Sub CmdOkG_Click()
FrmTransactionLog.Visible = False
End Sub

Private Sub CMDRESET_Click()
Dim sql As String
TxtBusiCodeA = ""
CmbAmcA.ListIndex = -1
TxtAcHolderA.Text = ""
TxtBranchA.Text = ""
TxtPanA.Text = ""
TxtSchemeA.Text = ""
txtdocID.Text = ""
ImEntryDtA.Text = Format(Now, "DD/MM/YYYY")
cmbTranTypeA.Text = ""
TxtAppnoA.Text = ""
TxtFolioNoA.Text = ""
txtInvestorA.Text = ""
TxtClientCodeA.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = Format(Now, "DD/MM/YYYY")
TxtAmountA = ""
Lbl_Leadno.Caption = ""
lbl_lead_caption.Caption = ""
MyTranCode = ""
MyBranchCode = ""
MySchCode = ""
TxtRmName.Text = ""
MyCurrentBranchCode = ""
optcheque.Value = True
Label32.Caption = ""
Label29.Caption = ""
chkcobA.Value = False
chkSWPA.Value = False
ChkFreedomA.Value = False
Label9.Caption = ""
TxtSwitchFolioA.Text = ""
TxtSwitchSchemeA.Text = ""
TxtPanVarify.Text = ""
ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
ImSipEndDtA.Text = "__/__/____"
dtChqDate.Text = "__/__/____"
cmbBusiBranch.Clear
busi_br_tr = ""
busi_change_flag = True
If GlbroleId = "21" Then DIS_EXPENSE
SSTab1.Tab = 0
Frame21.Left = 5000
Frame21.Top = 1320
CmbSipStpA.ListIndex = -1
ImNExp_PER.Text = ""
ImExpenses.Text = ""
CmbFrequency.ListIndex = -1
TxtInstallmens.Text = ""
CmbSubSipA.ListIndex = -1
Cmbsubinv.ListIndex = 1
Cmbsubinvu.ListIndex = 1
txtSipAmount.Text = ""
ImSipEndDtA.Text = "__/__/____"
ImSipStartDtA.Text = "__/__/____"

Label29.Caption = ""
Label9.Caption = ""
Label32.Caption = ""
LblTranCode.Caption = ""
Me.MousePointer = 11
MSRClient.DataSourceName = "TEST"
MSRClient.UserName = DataBaseUser
MSRClient.Password = DataBasePassword
FrmTransactionLog.Visible = False

LblSipType.Visible = False
CmbSubSipA.Visible = False
        
ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
mskfromdt.Text = Format(Now, "DD/MM/YYYY")
msktodt.Text = Format(Now, "DD/MM/YYYY")

CmbSubSipA.AddItem "NORMAL", 0
CmbSubSipA.AddItem "MICROSIP", 1

Frame22.Visible = False
Frame17.Visible = False
Frame12.Visible = False
OptChkSwitch.Visible = False
OptChkSwitchM.Visible = False
 '-----------------Frequency-----------------------------
 fracancel.Visible = False
 CmbFrequency.Clear
 CmbFrequency1.Clear
 'ButtonPermission Me
 sql = "select itemname,itemserialnumber from fixeditem where itemtype=25"
 Set RsFreq = MyConn.Execute(sql)
 While Not RsFreq.EOF
     CmbFrequency.AddItem RsFreq(0) & Space(50) & "#" & RsFreq(1)
     CmbFrequency1.AddItem RsFreq(0) & Space(50) & "#" & RsFreq(1)
     RsFreq.MoveNext
 Wend
 For i = 0 To CmbFrequency.ListCount - 1
    MyFreq = Split(CmbFrequency.List(i), "#")
    If MyFreq(1) = 175 Then
        CmbFrequency.ListIndex = i
    End If
 Next
 RsFreq.Close
 Set RsFreq = Nothing
 '-------------------------------------------------------
 
 CmbAmcA.Clear
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbAmcA.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 CmbSwitchAmcA.Clear
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbSwitchAmcA.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 CmbSwitchAmcM.Clear
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbSwitchAmcM.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 SSTab1.Tab = 0
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 CmbAmcM.Clear
 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbAmcM.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 CmbAmcM.ListIndex = -1
 
 Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
 cmbBankName.Clear
 While Not rsAmc.EOF
   cmbBankName.AddItem rsAmc(0)
   rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 
 Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
 cmbBankNameM.Clear
 While Not rsAmc.EOF
   cmbBankNameM.AddItem rsAmc(0)
   rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 
 If CmbSipStpA.Text = "SIP" Then
    OptsipF.Value = True
 Else
    OptsipF.Value = False
 End If
 ImEntryDtA.Text = Format(Now, "DD/MM/YYYY")
 ImEntryDtM.Text = Format(Now, "DD/MM/YYYY")
 

 cmbTranTypeA.Text = "PURCHASE"
     

Call cmbTranTypeA_Click
Call SetGrid
CmbOrder.Clear
CmbOrder.AddItem "Select", 0
CmbOrder.AddItem "Rm Name", 1
CmbOrder.AddItem "Branch Name", 2
CmbOrder.AddItem "Amc Name", 3
CmbOrder.AddItem "Scheme Name", 4
CmbOrder.AddItem "Pay Mode", 5
CmbOrder.AddItem "TranDate", 6
CmbOrder.ListIndex = 0
optcheque.Value = True
optchequeM.Value = True
Me.MousePointer = 0

 
Cmbsubinv.ListIndex = 0
Cmbsubinvu.ListIndex = 0

ChkClose.Value = 0
TxtCloseSch.Text = ""
    
End Sub

Private Sub cmdResetM_Click()
    CmbSipStpM.ListIndex = -1
    ImnExp_Per1.Text = ""
    CmbCancel.ListIndex = 0
    TxtBusiCodeM = ""
    txtdocID.Text = ""
    cmbBankNameM.Text = ""
    TxtAcHolderM.Text = ""
    CmbAmcM.ListIndex = -1
    TxtBranchM.Text = ""
    TxtSchemeM.Text = ""
    ImEntryDtM.Text = Format(Now, "DD/MM/YYYY")
    cmbTranTypeM.Text = ""
    TxtAppnoM.Text = ""
    TxtFolioNoM.Text = ""
    txtappno.Text = ""
    busi_br_tr = ""
    busi_change_flag = True
    txtInvestorM.Text = ""
    TxtClientCodeM.Text = ""
    txtChqNoM.Text = ""
    TxtChequeno1.Text = ""
    dtChqDateM.Text = Format(Now, "DD/MM/YYYY")
    TxtAmountM = ""
    Lbl_Leadno.Caption = ""
    lbl_lead_caption.Caption = ""
    MyTranCode = ""
    MyBranchCode = ""
    MySchCode = ""
    TxtRmNameM.Text = ""
    MyCurrentBranchCode = ""
    Label42.Caption = ""
    Label30.Caption = ""
    Label14.Caption = ""
    optchequeM.Value = True
    Txtinstallments1.Text = ""
    CmbFrequency1.ListIndex = -1
    mskfromdt.Text = "__/__/____"
    msktodt.Text = "__/__/____"
    dtChqDateM.Text = "__/__/____"
    TxtPanM.Text = ""
    txtTrNo.Text = ""
    TxtANACode.Text = ""
    TxtClientS.Text = ""
    txtpans.Text = ""
    ImExpenses1.Text = ""
    VSFCommGrdM.Clear
    LblTranCode.Caption = ""
    Call SetGrid
    CmbSipStpA.ListIndex = 0
    LblRecoStatus.Caption = ""
    LblRemarks.Text = ""
    cmbBusiBranchM.Clear
    ImEntryDtM.Text = "__/__/____"
    dtDropDate.Text = "__/__/____"
    txtFind.Text = ""
    CmbOrder.ListIndex = -1
    CmbOrder.ListIndex = -1
    Cmbsubinv.ListIndex = 0
    Cmbsubinvu.ListIndex = 0
    ImSipStartDtM.Text = Format(Now, "DD/MM/YYYY")
    ImSipEndDtM.Text = "__/__/____"
    TxtSwitchSchemeM = ""
    TxtSwitchSchemeM.Text = ""
    TxtSwitchFolioM.Text = ""
    chkSWPM.Value = False
    ChkFreedomM.Value = False
End Sub
Private Sub ResetView_Click()
    ImnExp_Per1.Text = ""
    ImExpenses1.Text = ""
    cmbBankNameM.Text = ""
    CmbAmcM.ListIndex = -1
    TxtBranchM.Text = ""
    TxtPanM.Text = ""
    TxtSchemeM.Text = ""
    ImEntryDtM.Text = Format(Now, "DD/MM/YYYY")
    cmbTranTypeM.Text = ""
    TxtAppnoM.Text = ""
    TxtFolioNoM.Text = ""
    TxtClientCodeM.Text = ""
    txtChqNoM.Text = ""
    dtChqDateM.Text = Format(Now, "DD/MM/YYYY")
    TxtAmountM = ""
    Lbl_Leadno.Caption = ""
    lbl_lead_caption.Caption = ""
    MyTranCode = ""
    MyBranchCode = ""
    MySchCode = ""
    TxtRmNameM.Text = ""
    MyCurrentBranchCode = ""
    optchequeM.Value = True
    Txtinstallments1.Text = ""
    CmbFrequency1.ListIndex = -1
    dtChqDateM.Text = "__/__/____"
    TxtPanM.Text = ""
    Cmbsubinv.ListIndex = 0
    Cmbsubinvu.ListIndex = 0

End Sub

Public Sub CmdSave_Click()

'''''''''''''''''''''''''''''''''''''''''''''''''CROSS CHANNEL VALIDATION SUBBROKER--------------------------------------------------------
vApprovalFlag = ""
vApprovalFlag = SqlRet("select wealthmaker.fn_check_for_approval_all('" & txtdocID.Text & "')  from dual")
'vApprovalFlag 2  Approval request for this transaction has already been raised.
'vApprovalFlag 4  Approval request for this transaction has rejected by Management.

If vApprovalFlag = 2 Then
    MsgBox "Approval request for this transaction has already been raised.", vbInformation, strBajaj
    Exit Sub
End If

If vApprovalFlag = 4 Then
    MsgBox "Approval request for this transaction has been rejected by Management.", vbInformation, strBajaj
    Exit Sub
End If


If CmbSipStpA = "SIP" Then
   If ImSipEndDtA.Text = "__/__/____" Or ImSipStartDtA.Text = "__/__/____" Then
        MsgBox "Please Enter SIP End Date ", vbInformation, "Wealthmaker"
        ImSipEndDtA.SetFocus
        Exit Sub
   End If
End If

If ChkClose = 1 Then
    If TxtCloseSch.Text <> "" Then
        str1 = Split(TxtCloseSch.Text, "=")
        MyCloseSchCode = str1(1)
    End If
Else
    MyCloseSchCode = ""
End If



If Label32 <> "" And MySchCode <> "" And TxtAmountA <> "" Then
    If (SqlRet("select count(*) from transaction_mf_temp1 a,scheme_info b where   (asa <> 'C' OR asa IS NULL) and a.sch_code=b.sch_code AND A.DOC_ID IS NOT NULL  and CLIENT_CODE='" & Label32 & "' and tr_date>sysdate-90 and b.sch_code='" & MySchCode & "' and amount between " & TxtAmountA - 100 & " and " & TxtAmountA + 100)) >= 1 Then
            frmpopupduplicate.formtype = "MF"
            frmpopupduplicate.prem_amt = TxtAmountA
            frmpopupduplicate.CCODE = Label32
            frmpopupduplicate.sch_code = MySchCode
            frmpopupduplicate.Show
            frmpopupduplicate.ZOrder 0
    Else
            save_method
    End If
Else
    save_method
End If
End Sub



Public Sub save_method()
On Error GoTo err
Dim Str() As String
Dim str1() As String
Dim FpReasonId As Double
Dim FpPotential As String
Dim rsCheck As New ADODB.Recordset
Set ARCODE = New ADODB.Recordset
Dim chk1 As New ADODB.Recordset
Dim rsfpcheck As New ADODB.Recordset


'-------------------------------Micro Pan valadation (vinit) 20 dec 2014

If TxtAmountA.Text <> "" Then
    micropanflag = 0
    If Cmbsubinv.Text = "MICRO" And TxtAmountA.Text < 50000 Then
        micropanflag = 1
    Else
        micropanflag = 0
    End If
End If


If TxtSchemeA.Text <> "" Then
    str1 = Split(TxtSchemeA.Text, "=")
    MySchCode = str1(1)
End If

If txtdocID.Text <> "" Then
        If (SqlRet("select combo_plan_val('" & txtdocID.Text & "','" & MySchCode & "',0,'MF') from dual")) = 0 Then
             MsgBox "LI premimum for combo plan can't exceed 6.70 Lacs OR plan must be of combo"
             Exit Sub
        End If
End If

''''''''''''''''''''''''''''''''''''''''''''''''''validation of not to punch in Unallocated Branch and uploaded client , rm, branch-------------------------------------------------

If cmbBusiBranch.Text <> "" Then
   Str = Split(cmbBusiBranch.Text, "#")
   MyBranchCode = Str(1)
End If

If ValidateBranch(MyBranchCode) = False Then
    MsgBox "Transaction can not be punched in Unallocated Branch.", vbCritical
    cboBranch.SetFocus
    Exit Sub
End If

''''''''''''''''''''''''''validation Uploaded Client '''''''''''''''''''''''''''''''''''''''''''''''''''
If txtdocID.Text <> "" Then
    If ValidateUploadedClientDT(txtdocID.Text, Label32.Caption) = False Then
        Exit Sub
    End If
End If

''''''''''''''''''''''''''validation Uploaded Branch '''''''''''''''''''''''''''''''''''''''''''''''''''
If cmbBusiBranch.Text <> "" Then
    If ValidateUploadedBranchDT(txtdocID.Text, MyBranchCode, Trim(TxtBusiCodeA.Text)) = False Then
        Exit Sub
    End If
End If

'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

'rsfpcheck.open "select count(plan_no) from fp_investor where lpad(familyhead_code,8)='" & Mid(txtClientCD, 1, 8) & "'", MyConn
'If rsfpcheck(0) < 1 Then
'MsgBox "Please create Financial planning for the investor first", vbCritical, "Wealthmaker"
'rsfpcheck.Close
'Exit Sub
'End If
'rsfpcheck.Close

If txtdocID.Text <> "" And Label32.Caption <> "" Then
        If (SqlRet("SELECT fp_status_check1('" & Label32.Caption & "','" & txtdocID.Text & "') FROM dual")) = 0 Then
            MsgBox "Please create Financial planning for the investor first", vbCritical, "Wealthmaker"
            Exit Sub
        End If
End If


If txtdocID.Text <> "" Then
    chk1.open "select 1 from transaction_st where doc_id='" & txtdocID.Text & "'", MyConn, adOpenForwardOnly
    If Not chk1.EOF Or Not chk1.BOF Then
        If Not chk1(0) Then
            MsgBox "Document number already assigned.", vbOKOnly
            chk1.Close
            Exit Sub
        End If
    End If
chk1.Close
End If



If txtdocID.Text <> "" Then
    chk1.open "select 1 from transaction_sttemp where doc_id='" & txtdocID.Text & "'", MyConn, adOpenForwardOnly
    If Not chk1.EOF Or Not chk1.BOF Then
        If Not chk1(0) Then
            MsgBox "Document number already assigned.", vbOKOnly
            chk1.Close
            Exit Sub
        End If
    End If
chk1.Close
End If



If chkSaveValidation(True, False, "") = False Then
    Exit Sub
End If
If DateDiff("d", Format(ServerDateTime, "dd/mm/yyyy"), ImEntryDtA.Text) >= 1 Then
   MsgBox "You Can Not Punch Transaction In Advance ", vbInformation
   Exit Sub
End If
If dtChqDate.Text <> "" And dtChqDate.Text <> "__/__/____" Then
    If DateDiff("M", Format(ServerDateTime, "dd/mm/yyyy"), dtChqDate.Text) > 1 Then
       MsgBox "You Can Not Give The Cheque Greater Than One Month", vbInformation
       Exit Sub
    End If
End If
rsCheck.open "select count(*) from tb_doc_upload where common_id ='" & Trim(txtdocID.Text) & "' and tran_type='MF'", MyConn, adOpenForwardOnly
If rsCheck(0) = 0 Then
    MsgBox "Please Enter a valid DT Number", vbInformation
    rsCheck.Close
    Set rsCheck = Nothing
    Exit Sub
End If
rsCheck.Close
Set rsCheck = Nothing

If CmbFrequency.ListIndex <> -1 Then
    MyFreq = Split(CmbFrequency.Text, "#")
Else
    MyFreq(0) = ""
    MyFreq(0) = ""
End If

If UCase(Trim(cmbTranTypeA.Text)) = "PURCHASE" And optcheque.Value = False And optdraft.Value = False And OptEcs.Value = False And OptRTGS.Value = False And OptFT.Value = False Then
    MsgBox "Please Select Cheque/Draft/ECS/RTGS/Fund Transfer Payment Mode", vbInformation
    Exit Sub
End If


'If TxtPanA.Text <> "" Then
'    If ValidatePan(TxtPanA.Text) = False Then
'       MsgBox "Please Either Enter a Valid PAN Number Or Leave Blank", vbCritical, "Pan Entry"
'       TxtPanA.SetFocus
'       Exit Sub
'    End If
'End If
'pan no mandatory for all the transaction
'If TxtPanA.Text = "" And Val(TxtAmountA.Text) >= 3000 Then
'    If ValidatePan(TxtPanA.Text) = False Then
'       MsgBox "Please Either Enter a Valid PAN Number", vbCritical, "Pan Entry"
'       TxtPanA.SetFocus
'       Exit Sub
'    End If
'End If
If cmbTranTypeA.Text = "" Then
   MsgBox "Please Select Transaction Type", vbInformation, "Wealthmaker"
   cmbTranTypeA.SetFocus
   Exit Sub
End If
If UCase(Trim(cmbTranTypeA.Text)) = "PURCHASE" Then
If CmbSipStpA.Text = "" Then
   MsgBox "Please Select Sip Type", vbInformation, "Wealthmaker"
   CmbSipStpA.SetFocus
   Exit Sub
End If
End If
'-------------------------FREQ AND INSTALLMENTS----------------------------
If (CmbSipStpA.Text = "SIP" Or CmbSipStpA.Text = "STP") Then
    If CmbFrequency.ListIndex = -1 Or TxtInstallmens.Text = "" Then
        MsgBox "Please Enter Frequency Type and No. Of Installments ", vbInformation, "Wealthmaker"
        CmbSipStpA.SetFocus
        Exit Sub
    End If
End If
If (CmbSipStpA.Text = "SIP") Then
    If ImSipEndDtA.Text = "__/__/____" Then
        MsgBox "Please Enter SIP End Date ", vbInformation, "Wealthmaker"
        ImSipEndDtA.SetFocus
        Exit Sub
    End If
    If txtSipAmount.Text = "" Or txtSipAmount.Text = "0" Then
        MsgBox "Please Enter SIP Amount", vbInformation, "Wealthmaker"
        txtSipAmount.SetFocus
        Exit Sub
    End If
    If OptsipF.Value = False And OptsipR.Value = False Then
       MsgBox "Please select either Fresh or Renewal", vbInformation
       Exit Sub
    End If
End If
'--------------------------------------------------------------------------
If OptsipF.Value = True Then
    If optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True Then   'vinod ECS 23/05/2005
         If Trim(txtChqNo.Text) = "" Then
            If optcheque.Value = True Then
                MsgBox "Please Fill Cheque No. ", vbInformation
            ElseIf optdraft.Value = True Then
                MsgBox "Please Fill Draft No. ", vbInformation
            ElseIf OptEcs.Value = True Then
                MsgBox "Please Fill MICR No. ", vbInformation
            ElseIf OptRTGS.Value = True Then
                MsgBox "Please Fill UTR No. ", vbInformation
            ElseIf OptFT.Value = True Then
                MsgBox "Please Fill Bank A/c No. ", vbInformation
            End If
            txtChqNo.SetFocus
            Exit Sub
         End If
         If dtChqDate.Text = "__/__/____" Then
            If optcheque.Value = True Then
                MsgBox "Cheque Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf optdraft.Value = True Then
                MsgBox "Draft Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf OptEcs.Value = True Then
                MsgBox "ECS Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf OptRTGS.Value = True Then
                MsgBox "UTR Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf OptFT.Value = True Then
                MsgBox "Fund Transfer Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            End If
            
            dtChqDate.SetFocus
            Exit Sub
         End If
    End If
End If
'-------------------------------------maya
If cmbTranTypeA.Text = "SWITCH IN" And (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or optcash.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) Then
   MsgBox "Please Select 'Other' Option", vbInformation, "Wealthmaker"
   cmbTranTypeA.SetFocus
   Exit Sub
End If
'-------------------------------------

'If Inv_Code_FirstL = "3" Then
'    Dim df As New DataFlow()
'    If Convert.ToString(Session("Sch_code")) <> "OP#10826" Then
'        Dim isvalid As Boolean
'        isvalid = df.VERIFY_PAN_NOTMINER(txtpan.Text, txtInvCode.Text)
'        If (isvalid = False) Then
'            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "msg", "alert('PAN Entered Did Not Match With The Selected Investor PAN');", True)
'            Exit Sub
'        End If
'    End If
'End If
'
    
    

If txtInvestorA.Text = "" Then
    MsgBox "Please Fill Investor Name", vbInformation, "Wealthmaker"
    txtInvestorA.SetFocus
    Exit Sub
End If

If TxtBusiCodeA.Text = "" Then
   MsgBox "BusiCode Can Not Left Blank", vbInformation, "Wealthmaker"
   TxtBusiCodeA.SetFocus
   Exit Sub
End If
If CmbAmcA.Text = "" Then
   MsgBox "Select The AMC", vbInformation, "Wealthmaker"
   CmbAmcA.SetFocus
   Exit Sub
End If
If cmbTranTypeA.Text = "PURCHASE" Then
'    If TxtAppnoA.Text = "" Then
'       MsgBox "AppNo Can Not Be Left Blank", vbInformation, "Wealthmaker"
'       TxtAppnoA.SetFocus
'       Exit Sub
'    End If
    
    If TxtAppnoA.Text <> "" Then
        If Len(TxtAppnoA.Text) < 6 Then
           MsgBox "Minimum Length Of App No Should Be Greater or Equal To 6", vbInformation, "Wealthmaker"
           TxtAppnoA.SetFocus
           Exit Sub
        ElseIf TxtAppnoA.Text = "000000" Then
           MsgBox "Please Enter A Valid App No", vbInformation, "Wealthmaker"
           TxtAppnoA.SetFocus
           Exit Sub
        End If
    End If
End If

If TxtSchemeA.Text = "" Then
   MsgBox "Select The Scheme", vbInformation, "Wealthmaker"
   TxtSchemeA.SetFocus
   Exit Sub
End If


If ImEntryDtA.Text = "__/__/____" Then
   MsgBox "Transaction Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
   ImEntryDtA.SetFocus
   Exit Sub
End If

If Len(TxtClientCodeA.Text) < 8 Then
   MsgBox "Client Code Can Not Left Blank", vbInformation, "Wealthmaker"
   TxtClientCodeA.SetFocus
   Exit Sub
End If
If Val(TxtAmountA.Text) = 0 Then
   MsgBox "Amount Can Not Be Zero", vbInformation, "Wealthmaker"
   TxtAmountA.SetFocus
   Exit Sub
End If
If TxtAmountA.Text = "" Then
   MsgBox "Amount Can Not Left Blank", vbInformation, "Wealthmaker"
   TxtAmountA.SetFocus
   Exit Sub
End If
If cmbBusiBranch.Text = "" Then
   MsgBox "Branch Name Can Not Left Blank", vbInformation, "Wealthmaker"
   Exit Sub
End If

If cmbBusiBranch.Text <> "" Then
   Str = Split(cmbBusiBranch.Text, "#")
   MyBranchCode = Str(1)
End If
If TxtSchemeA.Text <> "" Then
    str1 = Split(TxtSchemeA.Text, "=")
    MySchCode = str1(1)
End If
If MySchCode = "" Then
   MsgBox "Select Scheme First", vbInformation, "Wealthmaker"
   Exit Sub
End If
If TxtSwitchSchemeA.Text <> "" Then
    str1 = Split(TxtSwitchSchemeA.Text, "=")
    MySwitchSchCodeA = str1(1)
End If

If cmbTranTypeA.Text = "SWITCH IN" Or CmbSipStpA.Text = "STP" Then
   If TxtSwitchSchemeA.Text = "" Then
        MsgBox "Select the Scheme,you have Switched From", vbInformation, "Wealthmaker"
        TxtSwitchSchemeA.SetFocus
        Exit Sub
   End If
   If TxtSwitchFolioA.Text = "" Then
        MsgBox "Select the Folio,you have Switched From", vbInformation, "Wealthmaker"
        TxtSwitchFolioA.SetFocus
        Exit Sub
   End If
    If MySchCode = MySwitchSchCodeA Then
        MsgBox "In case of Switch transaction, Switch from Scheme cannot be same as Switch to Scheme", vbCritical
        Exit Sub
    End If
    If chk1.State = 1 Then chk1.Close
    chk1.open "select count(distinct(mut_code)) cnt from scheme_info where sch_code in ('" & MySchCode & "','" & MySwitchSchCodeA & "')", MyConn, adOpenForwardOnly
    If chk1("cnt") > 1 Then
        MsgBox "In case of switch Transaction, Switch from Scheme and Switch to Scheme should be from one AMC only", vbCritical
        Exit Sub
    End If
End If


'''''''''''''''''''''''''''''Cross Checking Of Pan With Account Master''''''''''''''''''''''''''''
If (CmbSipStpA.Text = "SIP") And CmbSubSipA.Text = "MICROSIP" Then
Else
    
    '--------------micro pan validate vinit 20 dec 2014
    
    If MySchCode <> "OP#10826" Then
        If micropanflag = 0 Then
            If TxtPanVarify.Text = "" Or ValidatePan(TxtPanA.Text) = False Then
                If ValidatePan(TxtPanVarify.Text) = False Then
                   MsgBox "Please Either Enter a Valid PAN Number", vbCritical, "Pan Entry"
                   TxtPanVarify.SetFocus
                   Exit Sub
                End If
            End If
        End If
    End If
    
    
'    Dim AccountPan As String
'    AccountPan = ""
'    AccountPan = SqlRet("select NVL(max(client_pan),0) from client_test where client_codekyc='" & Label32.Caption & "'")
'    If ValidatePan(AccountPan) = True Then
'        If UCase(TxtPanVarify.Text) <> UCase(AccountPan) Then
'           MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbInformation
'           Exit Sub
'        End If
'    ElseIf ValidatePan(AccountPan) = False Then
'        'MyConn.Execute ("Update client_test set client_pan='" & TxtPanVarify.Text & "' where client_codekyc='" & Label32.Caption & "'")
'    End If
    If MySchCode <> "OP#10826" Then
    
        If Left(Label32.Caption, 1) = "3" Then
                '-----------------------------CHECK IT IS MINOR OR NOT------------------------------------
                If Label32.Caption <> "" Then
                    dob = SqlRet("select NVL(dob,TO_DATE(SYSDATE)-10000) from INVESTOR_MASTER where INV_CODE=" & Label32.Caption & "")
                    'Category = SqlRet("select nvl(investor_code,1) from client_master where client_code=" & Mid(Label32.Caption, 1, 8) & "")
                    If Format(dob, "dd/mm/yyyy") <> "__/__/____" And IsDate(dob) = True Then
                            If DateDiff("YYYY", Format(dob, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
                                'If Category = 1 Then
                                    IsMinor = True
                                'Else
                                '    IsMinor = False
                                'End If
                            Else
                                IsMinor = False
                            End If
                    End If
                End If
                '------------------------------------------------------------------------------------------
                
                If MySchCode <> "OP#10826" Then
                    
                
                        If IsMinor = False Then
            '                inv_cd = SqlRet("select inv_code from investor_master where upper(PAN)='" & UCase(TxtPanVarify.Text) & "'")
            '                If inv_cd <> "" And inv_cd <> "0" Then
            '                    If inv_cd <> Label32.Caption Then
            '                        MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbCritical
            '                        'If MsgBox("Do you want to overwrite the existing PAN entered in the Investor Master with the PAN provided in the Transaction", vbYesNo + vbQuestion) = vbNo Then
            '                        Exit Sub
            '                        'End If
            '                    End If
            '                End If
            
                        '--------------micro pan validate vinit 20 dec 2014
            
                            If micropanflag = 0 Then
                                 PAN1 = ""
                                 PAN1 = SqlRet("select UPPER(PAN) from investor_master where INV_CODE='" & Label32.Caption & "'")
                                 If PAN1 <> "" And PAN1 <> "0" Then
                                     If PAN1 <> UCase(TxtPanVarify.Text) Then
                                         MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbCritical
                                         'If MsgBox("Do you want to overwrite the existing PAN entered in the Investor Master with the PAN provided in the Transaction", vbYesNo + vbQuestion) = vbNo Then
                                         Exit Sub
                                         'End If
                                     End If
                                 End If
                             End If
                             
                             
                             
            '                AccountPan = SqlRet("select NVL(COUNT(*),0) from client_test where client_codekyc='" & Label32.Caption & "' AND (upper(CLIENT_PAN)='" & UCase(TxtPanVarify.Text) & "' OR CLIENT_PAN IS NULL)")
            '                If AccountPan = 0 Then
            '                    MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbInformation
            '                    Exit Sub
            '                End If
                        Else
            '                inv_cd = SqlRet("select inv_code from investor_master where upper(g_pan)='" & UCase(TxtPanVarify.Text) & "'")
            '                If inv_cd <> "" Then
            '                    If inv_cd <> Label32.Caption Then
            '                        MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbInformation
            '                        Exit Sub
            '                    End If
            '                End If
                            
            '                AccountPan = SqlRet("select NVL(COUNT(*),0) from client_test where client_codekyc='" & Label32.Caption & "' AND (upper(G_PAN)='" & UCase(TxtPanVarify.Text) & "' OR G_PAN IS NULL)")
            '                If AccountPan = 0 Then
            '                    MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbInformation
            '                    Exit Sub
            '                End If
                        End If
                End If
        
        End If
    End If
    
    If MySchCode <> "OP#10826" Then
        If Left(Label32.Caption, 1) = "4" Then
            Dim AccountPan As Integer
            'Dim inv_cd As String
            AccountPan = 0
            inv_cd = ""
            IsMinor = False
            Category = ""
            '-----------------------------CHECK IT IS MINOR OR NOT------------------------------------
            If Label32.Caption <> "" Then
                dob = SqlRet("select nvl(dob,to_date('01/01/1800','mm/dd/yyyy')) from client_test where client_codekyc=" & Label32.Caption & "")
                If dob = "01/01/1800" Then
                    MsgBox "Please fill Date of Birth for this investor"
                    Exit Sub
                End If
                Category = SqlRet("select nvl(investor_code,1) from client_master where client_code=" & Mid(Label32.Caption, 1, 8) & "")
                If Format(dob, "dd/mm/yyyy") <> "__/__/____" And IsDate(dob) = True Then
                        If DateDiff("YYYY", Format(dob, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
                            If Category = 1 Then
                                IsMinor = True
                            Else
                                IsMinor = False
                            End If
                        Else
                            IsMinor = False
                        End If
                End If
            End If
            '------------------------------------------------------------------------------------------
            If IsMinor = False Then
            
              '--------------micro pan validate vinit 20 dec 2014
            
                            If micropanflag = 0 Then
                            
                                    inv_cd = SqlRet("select client_codekyc from client_test where upper(CLIENT_PAN)='" & UCase(TxtPanVarify.Text) & "'")
                                    If inv_cd <> "" And inv_cd <> "0" Then
                                        If inv_cd <> Label32.Caption Then
                                            MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
                                            Exit Sub
                                        End If
                                    End If
                                 
                                    AccountPan = SqlRet("select NVL(COUNT(*),0) from client_test where client_codekyc='" & Label32.Caption & "' AND (upper(CLIENT_PAN)='" & UCase(TxtPanVarify.Text) & "' OR CLIENT_PAN IS NULL)")
                                    If AccountPan = 0 Then
                                        MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
                                        Exit Sub
                                    End If
                            End If
            Else
    '            inv_cd = SqlRet("select client_codekyc from client_test where upper(g_pan)='" & UCase(TxtPanVarify.Text) & "'")
    '            If inv_cd <> "" Then
    '                If inv_cd <> Label32.Caption Then
    '                    MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
    '                    Exit Sub
    '                End If
    '            End If
    
    '--------------micro pan validate vinit 20 dec 2014
            
                If micropanflag = 0 Then
                    PAN1 = ""
                    PAN1 = SqlRet("select NVL(upper(g_pan),0) from client_test where client_codekyc='" & Label32.Caption & "'")
                    If PAN1 <> "" And PAN1 <> "0" Then
                        If PAN1 <> UCase(TxtPanVarify.Text) Then
                            MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN", vbCritical
                            Exit Sub
                        End If
                    End If
                    
                    AccountPan = SqlRet("select NVL(COUNT(*),0) from client_test where client_codekyc='" & Label32.Caption & "' AND (upper(G_PAN)='" & UCase(TxtPanVarify.Text) & "' OR G_PAN IS NULL)")
                    If AccountPan = 0 Then
                        MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbCritical
                        Exit Sub
                    End If
                End If
            End If
        End If
        
        If TxtPanA.Text = "" Or ValidatePan(TxtPanA.Text) = False Then
           TxtPanA.Text = TxtPanVarify.Text
        End If
        
        End If
    End If
'''''''''''''''''''''''''''''end Cross Checking Of Pan With Account Master''''''''''''''''''''''''


If ChkAtmTransactionA.Value = 1 Then
    If get_ATM_scheme_amt_condition(MySchCode, Val(TxtAmountA)) = False Then
        MsgBox "Minimum Amount condition to have Reliance ATM Card is not being fulfilled ", vbCritical
        Exit Sub
    End If
End If

If cmbTranTypeA.Text = "PURCHASE" Then
    Dim sql As String
    If CmbSipStpA.Text = "SIP" And OptEcs.Value = True Then
'        sql = "select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and SCH_CODE='" & MySchCode & "'"
'        sql = sql & " and amount=" & Val(TxtAmountA) & " "
    Else
    'HIMANSHU
'        sql = "select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and SCH_CODE='" & MySchCode & "'"
'        sql = sql & "  and amount=" & Val(TxtAmountA) & " "   'AND CHEQUE_NO='" & txtChqNo.Text & "'
'        Set ARCODE = MyConn.Execute(sql)
'
'        If ARCODE.EOF Then
'        Else
'            MsgBox "Sorry,This Transaction Has Been Already Punched", vbInformation, "Wealthmaker"
'            Exit Sub
'        End If
'        ARCODE.Close
        
        'vinit 19 dec 215
        'Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where bank_name = '" & Trim(cmbBankName.Text) & "' and mut_code = '" & MyAmcCode & "' and sch_code = '" & MySchCode & "' and busi_branch_code = " & MyBranchCode & "  and (cheque_no='" & Trim(txtChqNo.Text) & "') AND (ASA<>'C' OR ASA IS NULL) ")
        'If Not ARCODE.EOF Then
        '    MsgBox "Duplicate Record ", vbInformation
        '    Exit Sub
        'End If
        
        'HIMANSHU
        
    End If
End If
Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and  app_no='" & TxtAppnoA.Text & "' AND (ASA<>'C' OR ASA IS NULL) ")
If ARCODE.EOF Then
Else
    MsgBox "Sorry,This App No in a Transaction Has Been Already Punched In This Company", vbInformation, "Wealthmaker"
    Exit Sub
End If
ARCODE.Close

If Not (CmbSipStpA.Text = "SIP" And OptEcs.Value = True) Then
   ' Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where BUSINESS_RMCODE='" & TxtBusiCodeA & "'  AND (ASA<>'C' OR ASA IS NULL)") 'and CHEQUE_NO='" & txtChqNo.Text & "'
   ' If ARCODE.EOF Then
   ' Else
   '     MsgBox "Sorry,This Application Has Been Already Punched", vbInformation, "Wealthmaker"
   '     Exit Sub
   ' End If
   ' ARCODE.Close
End If

ArLead = Split(Lbl_Leadno.Caption, ":")
'---------------------------Pankaj--------------------------------------------------------
    Flag_Lead = False
    incode = Val(Label32.Caption)
    FP_Form = "TR"
    FP_Flag = "Save"
    cr_planno = ""
    plan_type = ""
    If frm_crorepati.optfp.Value = True Then
        FpPotential = 1
    ElseIf frm_crorepati.OPTNONFP.Value = True Then
        FpPotential = 0
    Else
        FpPotential = 0
        FpReasonId = 294
    End If
    
    If frm_crorepati.cmb_Reason.Text <> "" Then
        Str = Split(frm_crorepati.cmb_Reason.Text, "#")
        FpReasonId = Str(1)
    Else
        FpReasonId = 0
    End If
    
    If FpPotential = 1 Then
        FpReasonId = 0
    End If
    
    If frm_crorepati.OPTNONFP.Value = False And frm_crorepati.optfp.Value = False Then
        frm_crorepati.Left = 3500
        frm_crorepati.optfp.Value = True
        frm_crorepati.cror_lbl2(0).Visible = False
        frm_crorepati.Top = 4200
        frm_crorepati.Show
        frm_crorepati.ZOrder
        frm_crorepati.Form_Activate
        Exit Sub
    Else
        Unload frm_crorepati
    End If
    

'---------------------------------------------Received ------------------------------------------
MyRecdA = 0
'If MyNat = "C" Then
'    If Mid(TxtClientCodeA, 1, 1) = "3" Then
'         MyRecdA = SqlRet("select nvl(Upfront_Ope_RECD_Temptran2_MSH('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & " , 1 ," & MyCurrentBranchCode & "),0) from dual ")
'    Else
'         MyRecdA = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MSH('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & " , 1 ," & MyCurrentBranchCode & "),0) from dual ")
'    End If
'Else  'open ended
'    If Mid(TxtClientCodeA, 1, 1) = "3" Then
'         MyRecdA = SqlRet("select nvl(Upfront_Ope_RECD_Temptran2_MS('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & " , 1 ," & MyCurrentBranchCode & "),0) from dual ")
'    Else
'         MyRecdA = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MS('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & " , 1 ," & MyCurrentBranchCode & "),0) from dual ")
'    End If
'End If
'asetflag = ""
'asetflag = SqlRet("select nvl(get_scheme_nature_trail_dt('" & UCase(MySchCode) & "','',''),'A') a from dual")
'
'If Trim(asetflag) = "" Then
'    MsgBox "Asset category for this Scheme has not been defined. So transaction cannot be punched corresponding to this Scheme", vbInformation
'    Exit Sub
'End If

'sql = "select nvl(get_recd_mf('" & asetflag & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY'),'" & MyCurrentBranchCode & "','" & TxtClientCodeA.Text & "'," & Val(TxtAmountA.Text) & ",'" & MyAmcCode & "','" & MySchCode & "','" & cmbTranTypeA.Text & "'),0) from dual"
sql = "select nvl(Upfront_recd_NONang_detail('" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & ",'" & cmbTranTypeA.Text & "','" & MySwitchSchCodeA & "'),0)+nvl(trail_recd_ang_detail('" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & "),0) from dual"

'Upfront_recd_ang_detail(MUT_CODE1 VARCHAR2,SCH_CODE1 VARCHAR2,TR_DATE1 date,AMOUNT1 number,var_tr_type VARCHAR2,VAR_SWITCH_SCHEME VARCHAR2)
'trail_recd_ang_detail(MUT_CODE1 VARCHAR2,SCH_CODE1 VARCHAR2,TR_DATE1 date,AMOUNT1 number)


MyRecdA = 0
MyRecdA = SqlRet(sql)

If Val(ImExpenses.Text) > MyRecdA And MyRecdA <> 0 Then
   MsgBox "Payable cannot be greater than Receivable", vbInformation, "Wealthmaker"
   Exit Sub
End If

MyConn.BeginTrans
sql = ""
'--------------------------------------------------------------------------------------------------
If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then
    sql = " INSERT INTO transaction_mf_Temp1"
    sql = sql & "( "
    sql = sql & " ATM_FLAG,SIP_AMOUNT,CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
    sql = sql & " APP_NO,SIP_START_DATE,Pan,FOLIO_NO,SWITCH_FOLIO,SWITCH_SCHEME,PAYMENT_MODE,BANK_NAME,CHEQUE_NO,CHEQUE_DATE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,TIMEST,SIP_END_DATE,sip_fr,dispatch,doc_id,micro_investment,target_switch_scheme,cob_flag,SWP_flag,FREEDOM_SIP_FLAG "
Else
    sql = " INSERT INTO transaction_mf_Temp1"
    sql = sql & "( "
    sql = sql & " ATM_FLAG,SIP_AMOUNT,CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
    sql = sql & " APP_NO,SIP_START_DATE,Pan,FOLIO_NO,SWITCH_FOLIO,SWITCH_SCHEME,PAYMENT_MODE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,TIMEST,SIP_END_DATE,sip_fr,dispatch,doc_id,micro_investment,target_switch_scheme,cob_flag,SWP_flag,FREEDOM_SIP_FLAG "
End If

sql = sql & ")VALUES(" & ChkAtmTransactionA.Value & ", " & Val(txtSipAmount.Text) & "," & Val(Label32.Caption) & ",'" & TxtBusiCodeA & "','" & Glbloginid & "','" & TxtBusiCodeA & "'," & MyBranchCode & ",'" & TxtPanA & "','" & MyAmcCode & "','" & MySchCode & "', "
sql = sql & " TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY'),'" & cmbTranTypeA.Text & "',"
sql = sql & " '" & TxtAppnoA.Text & "'"
If ImSipStartDtA.Text <> "__/__/____" Then
    sql = sql & " ,TO_DATE('" & MyDate(ImSipStartDtA) & "','MM/DD/YYYY')"
Else
    sql = sql & " ,null"
End If
sql = sql & ",'" & TxtPanVarify.Text & "','" & TxtFolioNoA.Text & "','" & TxtSwitchFolioA.Text & "','" & MySwitchSchCodeA & "',"
If optcheque.Value = True Then
    paymode = "C"
ElseIf optdraft.Value = True Then
    paymode = "D"
ElseIf optcash.Value = True Then
    paymode = "H"
ElseIf OptEcs.Value = True Then
    paymode = "E"
ElseIf OptOthers.Value = True Then
    paymode = "R"
ElseIf OptRTGS.Value = True Then
    paymode = "U"
ElseIf OptFT.Value = True Then
    paymode = "B"
End If
sql = sql & "'" & paymode & "'"
If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then   'vinod ECS 23/05/2005
     sql = sql & ",'" & Trim(cmbBankName.Text) & "','" & Trim(txtChqNo.Text) & "',to_date('" & Trim(dtChqDate.Text) & "','DD-MM-YYYY'),"
Else
    sql = sql & ","
End If
sql = sql & " " & Val(TxtAmountA.Text) & ""
sql = sql & ",'" & CmbSipStpA.Text & "'"
sql = sql & ",'" & lbl_lead_caption.Caption & "','" & TxtClientCodeA.Text & "','" & txtInvestorA & "'," & Val(ImNExp_PER.Text) & "," & Val(ImExpenses.Text) & ",'" & TxtAcHolderA.Text & "','" & MyFreq(1) & "','" & Trim(TxtInstallmens.Text) & "',SYSDATE "
If ImSipEndDtA.Text <> "__/__/____" Then
    sql = sql & " ,TO_DATE('" & MyDate(ImSipEndDtA) & "','MM/DD/YYYY')"
Else
    sql = sql & " ,null"
End If
If OptsipF.Value = True Then
    sql = sql & " ,'F'"
ElseIf OptsipR.Value = True Then
    sql = sql & " ,'R'"
Else
    sql = sql & " ,NULL"
End If
If optReg.Value = True Then
    sql = sql & " ,'R'"
ElseIf optNFO.Value = True Then
    sql = sql & " ,'N'"
End If
sql = sql & ",'" & Trim(txtdocID.Text) & "','" & Cmbsubinv.Text & "','" & MyCloseSchCode & "','" & chkcobA.Value & "','" & chkSWPA.Value & "','" & ChkFreedomA.Value & "')"

MyConn.Execute sql
MsgBox "Current transaction has been recorded successfully.", vbInformation, "WEALTHMAKER"
Set ARCODE1 = New ADODB.Recordset
sql = ""
sql = sql & " select max(tran_code) from transaction_mf_Temp1 where BUSINESS_RMCODE='" & TxtBusiCodeA & "' "
If TxtPanA.Text <> "" Then
    sql = sql & " and PANNO='" & TxtPanA & "' "
End If
If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then
    If txtChqNo.Text <> "" Then
        sql = sql & " and CHEQUE_NO='" & txtChqNo.Text & "'"
    End If
End If
If TxtAppnoA.Text <> "" Then
    sql = sql & " and APP_NO='" & TxtAppnoA.Text & "'"
End If
Set ARCODE1 = MyConn.Execute(sql)
If Not ARCODE1.EOF Then
    a = InputBox("System Generated AR No.", "WealthMaker", ARCODE1(0))
    MsgBox "Your ARNo is :-->> " & ARCODE1(0), vbInformation, "WEALTH MAKER"
    
    lbtrancode.Caption = ARCODE1(0)
    MyPrintTranCode = ARCODE1(0)
    
    DoEvents
End If
ARCODE1.Close

MyBrokRecd = 0
MyBrokPay = 0
MyMargin = 0
'MyBrokRecd = SqlRet("select nvl(Upfront_Ope_Recd_Temptran1_mf('" & MyPrintTranCode & "'),0) from dual")
'MyBrokPay = SqlRet("select nvl(Upfront_Ope_paid_Temptran2_mf('" & MyPrintTranCode & "'),0) from dual")
'MyMargin = (Val(MyBrokRecd) - Val(MyBrokPay)) * 11
'MyConn.Execute ("update transaction_mf_temp1 set brok_recd=" & MyBrokRecd & ",brok_payble=" & MyBrokPay & ",margin=" & MyBrokRecd & " - " & MyBrokPay & " where tran_code='" & MyPrintTranCode & "'")

If Val(Label32.Caption) <> 0 And TxtPanVarify.Text <> "" And IsMinor = False Then
    MyConn.Execute ("update investor_master set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,pan='" & TxtPanVarify.Text & "' where inv_code='" & Val(Label32.Caption) & "' and pan is null ")
    MyConn.Execute ("update client_test set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,client_pan='" & TxtPanVarify.Text & "' where client_codekyc='" & Val(Label32.Caption) & "' and client_pan is null ")
End If


If cmbTranTypeA.Text = "PURCHASE" Then
    MyConn.Execute "CALL PRCINSERTWMTRANSIP('" & MyPrintTranCode & "',1)"
'    SIPADV = SqlRet("select BAJAJ_SIP_ADVANTAGE1_SIP_TEMP('" & MyPrintTranCode & "') from dual")
'    If SqlRet("SELECT BAJAJ_SIP_ADVANTAGE1_SIP_TEMP('" & MyPrintTranCode & "') FROM DUAL") > 0 Then
'        MyUniqueKey = SqlRet("select unique_id from wm_tran_sip where base_tran_code='" & MyPrintTranCode & "'")
'
'        If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then
'            SQL = "INSERT INTO transaction_mf_Temp1"
'            SQL = SQL & "( "
'            SQL = SQL & " CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
'            SQL = SQL & " APP_NO,SIP_START_DATE,Pan,FOLIO_NO,PAYMENT_MODE,BANK_NAME,CHEQUE_NO,CHEQUE_DATE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,SIP_ADV,BROK_RECD,BROK_PAYBLE,MARGIN,SIP_ID,MOVE_FLAG1,BASE_TRAN_CODE,SIP_END_DATE,sip_fr,dispatch "
'        Else
'            SQL = " INSERT INTO transaction_mf_Temp1"
'            SQL = SQL & "( "
'            SQL = SQL & " CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
'            SQL = SQL & " APP_NO,SIP_START_DATE,Pan,FOLIO_NO,PAYMENT_MODE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,SIP_ADV,BROK_RECD,BROK_PAYBLE,MARGIN,SIP_ID,MOVE_FLAG1,BASE_TRAN_CODE,SIP_END_DATE,sip_fr,dispatch "
'        End If
'        SQL = SQL & ")VALUES(" & Val(Label32.Caption) & ",'" & TxtBusiCodeA & "','" & Glbloginid & "','" & TxtBusiCodeA & "'," & MyBranchCode & ",'" & TxtPanA & "','" & MyAmcCode & "','" & MySchCode & "', "
'        SQL = SQL & " TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY'),'" & cmbTranTypeA.Text & "',"
'        SQL = SQL & " '" & TxtAppnoA.Text & "'"
'        If ImSipStartDtA.Text <> "__/__/____" Then
'            SQL = SQL & " ,TO_DATE('" & MyDate(ImSipStartDtA) & "','MM/DD/YYYY')"
'        Else
'            SQL = SQL & " ,null"
'        End If
'        SQL = SQL & " ,'" & TxtPanVarify.Text & "','" & TxtFolioNoA.Text & "',"
'        If optcheque.Value = True Then
'            paymode = "C"
'        ElseIf optdraft.Value = True Then
'            paymode = "D"
'        ElseIf optcash.Value = True Then
'            paymode = "H"
'        ElseIf OptEcs.Value = True Then
'            paymode = "E"
'        ElseIf OptOthers.Value = True Then
'            paymode = "R"
'        ElseIf OptRTGS.Value = True Then
'            paymode = "U"
'        ElseIf OptFT.Value = True Then
'            paymode = "B"
'        End If
'        SQL = SQL & "'" & paymode & "'"
'        If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then  'vinod ECS 23/05/2005
'             SQL = SQL & ",'" & Trim(cmbBankName.Text) & "','" & Trim(txtChqNo.Text) & "',to_date('" & Trim(dtChqDate.Text) & "','DD-MM-YYYY'),"
'        Else
'            SQL = SQL & ","
'        End If
'        SQL = SQL & " " & Val(TxtAmountA.Text) * 11 & ""
'        SQL = SQL & ",'" & CmbSipStpA.Text & "'"
'        SQL = SQL & ",'" & lbl_lead_caption.Caption & "','" & TxtClientCodeA.Text & "','" & txtInvestorA & "'," & Val(ImnExp_Per.Text) & "," & Val(ImExpenses.Text) & ",'" & TxtAcHolderA.Text & "','" & MyFreq(1) & "','" & Trim(TxtInstallmens.Text) & "'," & Val(SIPADV) & " ," & Val(MyBrokRecd) * 11 & "," & Val(MyBrokPay) * 11 & "," & Val(MyMargin) & "," & MyUniqueKey & ",'ADVSIP','" & MyPrintTranCode & "'"
'        If ImSipEndDtA.Text <> "__/__/____" Then
'            SQL = SQL & " ,TO_DATE('" & MyDate(ImSipEndDtA) & "','MM/DD/YYYY')"
'        Else
'            SQL = SQL & " ,null"
'        End If
'        If OptsipF.Value = True Then
'            SQL = SQL & " ,'F'"
'        ElseIf OptsipF.Value = True Then
'            SQL = SQL & " ,'R'"
'        Else
'            SQL = SQL & " ,NULL"
'        End If
'        If optReg.Value = True Then
'            SQL = SQL & " ,'R'"
'        ElseIf optNFO.Value = True Then
'            SQL = SQL & " ,'N'"
'        End If
'        SQL = SQL & " )"
'        MyConn.Execute SQL
'    End If

End If


'-----------------pankaj kumar--------------------------------------------------------------
If FpPotential <> "" Then
    If Right(Label32.Caption, 1) = 1 Then
        MyConn.Execute "UPDATE CLIENT_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,FP_POTENTIAL='" & FpPotential & "',FP_REASON_ID='" & FpReasonId & "' where client_code=" & Mid(Label32.Caption, 1, 8) & " "
        MyConn.Execute "UPDATE INVESTOR_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,FP_POTENTIAL='" & FpPotential & "',FP_REASON_ID='" & FpReasonId & "' where inv_code=" & Label32.Caption & " "
    Else
        MyConn.Execute "UPDATE INVESTOR_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,FP_POTENTIAL='" & FpPotential & "',FP_REASON_ID='" & FpReasonId & "' where inv_code=" & Label32.Caption & " "
    End If
    MyConn.Execute "UPDATE CLIENT_TEST SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,FP_POTENTIAL='" & FpPotential & "',FP_REASON_ID='" & FpReasonId & "' where CLIENT_CODEKYC=" & Label32.Caption & " "
End If
'-------------------------------------------------------------------------------------------
MyConn.CommitTrans
''---------------------FOR PER APP RECD PAID-----------------------------------
Dim ins_mar As New ADODB.Command
Set ins_mar.ActiveConnection = MyConn
ins_mar.CommandType = adCmdStoredProc
ins_mar.CommandText = "UPDATE_MF_MARGIN_TRAN"
ins_mar.Parameters.Append ins_mar.CreateParameter("TRANCODE", adVarChar, adParamInput, 14, MyPrintTranCode)
ins_mar.Execute
Set ins_mar = Nothing
''------------------------------------------------------------------------------
ChkAtmTransactionA.Value = 0
'Comment by pankaj pundir on request anup ji on dated 12022016
'Call gridfill(1)
'If MsgBox("Do You Want To Print this Ar", vbYesNo + vbQuestion) = vbYes Then
'   Call cmdArPrint_Click
'End If
ChkClose.Value = 0
CMDRESET_Click
cmdResetM_Click
Exit Sub
err:
MsgBox err.Description
Resume
MyConn.RollbackTrans
Resume
End Sub


Private Sub cmdModify_Click()
On Error GoTo err
Dim Str() As String
Dim str1() As String
Dim sql As String
Dim rsCheck As New ADODB.Recordset
Set ARCODE = New ADODB.Recordset

''''''''''''''''''''''''''ADMIN UPDATE VALIDATION'''''''''''''''''''''''''''''''''''''''''''''''''''
If GlbroleId = 1 Then
    MsgBox "You are not authorised to update the Transaction Details", vbInformation
    CmdModify.SetFocus
    Exit Sub
End If
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

If CmbSipStpM = "SIP" Then
   If ImSipEndDtM.Text = "__/__/____" Or ImSipStartDtM.Text = "__/__/____" Then
        MsgBox "Please Enter SIP End Date ", vbInformation, "Wealthmaker"
        ImSipEndDtM.SetFocus
        Exit Sub
   End If
End If

If ChkCloseM = 1 Then
    If TxtCloseSchM.Text <> "" Then
        str1 = Split(TxtCloseSchM.Text, "=")
        MyCloseSchCodeM = str1(1)
    End If
Else
    MyCloseSchCodeM = ""
End If


If Cmbsubinvu = "NORMAL" Or Cmbsubinvu = "MICRO" Then
Else
   MsgBox "Please Select Correct Sub investor type "
End If


'-------------------------------Micro Pan valadation (vinit) 20 dec 2014

If TxtAmountM.Text <> "" Then
    micropanflag = 0
    If Cmbsubinvu.Text = "MICRO" And TxtAmountM < 50000 Then
        micropanflag = 1
    Else
        micropanflag = 0
    End If
End If


If TxtSchemeM.Text <> "" Then
    str1 = Split(TxtSchemeM.Text, "=")
    MySchCode = str1(1)
End If

If TxtSwitchSchemeM.Text <> "" Then
    str1 = Split(TxtSwitchSchemeM.Text, "=")
    MySwitchSchCodeM = str1(1)
End If

If cmbTranTypeM.Text = "SWITCH IN" Or CmbSipStpM.Text = "STP" Then
    
   If TxtSwitchSchemeM.Text = "" Then
        MsgBox "Select the Scheme,you have Switched From", vbInformation, "Wealthmaker"
        TxtSwitchSchemeM.SetFocus
        Exit Sub
   End If
   If TxtSwitchFolioM.Text = "" Then
        MsgBox "Select the Folio,you have Switched From", vbInformation, "Wealthmaker"
        TxtSwitchFolioM.SetFocus
        Exit Sub
   End If
   
    If MySchCode = MySwitchSchCodeM Then
        MsgBox "In case of Switch transaction, Switch from Scheme cannot be same as Switch to Scheme", vbCritical
        Exit Sub
    End If
    If rsCheck.State = 1 Then rsCheck.Close
    rsCheck.open "select count(distinct(mut_code)) cnt from scheme_info where sch_code in ('" & MySchCode & "','" & MySwitchSchCodeM & "')", MyConn, adOpenForwardOnly
    If rsCheck("cnt") > 1 Then
        MsgBox "In case of switch Transaction, Switch from Scheme and Switch to Scheme should be from one AMC only", vbCritical
        Exit Sub
    End If
    
End If

If ModifyDocid <> "NA" Then
    If txtTrNo <> "" Then
           If (SqlRet("select combo_plan_val((select distinct common_id from tb_doc_upload where ar_code='" & txtTrNo.Text & "'),'" & MySchCode & "',0,'MF') from dual")) = 0 Then
                  MsgBox "LI premimum for combo plan can't exceed 6.70 Lacs OR plan mut be of combo"
                  Exit Sub
            End If
    End If
End If

If MyTranCode = "" Then
   MsgBox "Double Click The Record You Want To Update"
   Exit Sub
End If
If chkSaveValidation(False, True, MyTranCode) = False Then 'vinod 16/02/2006
    Exit Sub
End If
If DateDiff("d", Format(ServerDateTime, "dd/mm/yyyy"), ImEntryDtM.Text) >= 1 Then
   MsgBox "You Can Not Punch Transaction In Advance ", vbInformation
   Exit Sub
End If
If dtChqDateM.Text <> "__/__/____" Then
    If DateDiff("M", Format(ServerDateTime, "dd/mm/yyyy"), dtChqDateM.Text) > 1 Then
       MsgBox "You Can Not Give The Cheque Date Greater Than One Month", vbInformation
       Exit Sub
    End If
End If


ModifyDocid = SqlRet("SELECT NVL(DOC_ID,'NA') FROM WEALTHMAKER.TRANSACTION_MF_TEMP1 WHERE TRAN_CODE='" & txtTrNo & "'")

If ModifyDocid <> "NA" Then
    If rsCheck.State = 1 Then rsCheck.Close
    rsCheck.open "select count(*) from tb_doc_upload where common_id ='" & Trim(txtdocID.Text) & "' and tran_type='MF'", MyConn, adOpenForwardOnly
    If rsCheck(0) = 0 Then
        MsgBox "Please Enter a valid DT Number", vbInformation
        rsCheck.Close
        Set rsCheck = Nothing
        Exit Sub
    End If
    rsCheck.Close
    Set rsCheck = Nothing
End If

If LblRecoStatus.Caption = "Confirmed" And GlbroleId <> 29 And GlbroleId <> 1 And GlbroleId <> 21 Then
    MsgBox "Reconciled transactions cannot be modified ", vbCritical
    Exit Sub
End If

If CmbFrequency.ListIndex <> -1 Then
    MyFreq = Split(CmbFrequency.Text, "#")
Else
    MyFreq(0) = ""
    MyFreq(1) = ""
End If
If cmbTranTypeM.Text = "" Then
   MsgBox "Please Select Transaction Type", vbInformation, "Wealthmaker"
   cmbTranTypeM.SetFocus
   Exit Sub
End If
If CmbSipStpM.Text = "" Then
   MsgBox "Please Select SIP Type", vbInformation, "Wealthmaker"
   CmbSipStpM.SetFocus
   Exit Sub
End If
'-------------------------FREQ AND INSTALLMENTS----------------------------
If (CmbSipStpM.Text = "SIP" Or CmbSipStpM.Text = "STP") Then
    If CmbFrequency1.ListIndex = -1 Or Txtinstallments1.Text = "" Then
        MsgBox "Please Enter Frequency Type and No. Of Installments ", vbInformation, "Wealthmaker"
        CmbSipStpM.SetFocus
        Exit Sub
    End If
End If

'-----------------vinit 22 dec 2014
'--------------------------------------------------------------------------

If micropanflag = 0 Then

        If TxtPanM.Text <> "" Then
            If ValidatePan(TxtPanM.Text) = False Then
               MsgBox "Please Either Enter a Valid PAN Number ", vbCritical, "Pan Entry"
               TxtPanM.SetFocus
               Exit Sub
            End If
        End If
        If TxtPanM.Text = "" Then
            If ValidatePan(TxtPanM.Text) = False Then
               MsgBox "Please Either Enter a Valid PAN Number", vbCritical, "Pan Entry"
               TxtPanM.SetFocus
               Exit Sub
            End If
        End If
End If

'If TxtAppnoM.Text = "" Then
'   MsgBox "App No Can Not Be Left Blank", vbInformation, "Wealthmaker"
'   TxtAppnoM.SetFocus
'   Exit Sub
'End If
If TxtAppnoM.Text <> "" Then
    If Len(TxtAppnoM.Text) < 6 Then
       MsgBox "Minimum Length Of App No Should Be Greater or Equal To 6", vbInformation, "Wealthmaker"
       TxtAppnoM.SetFocus
       Exit Sub
    ElseIf TxtAppnoM.Text = "000000" Then
       MsgBox "Please Enter A Valid App No", vbInformation, "Wealthmaker"
       TxtAppnoM.SetFocus
       Exit Sub
    End If
End If
If TxtBusiCodeM.Text = "" Then
   MsgBox "BusiCode Can Not Left Blank"
   TxtBusiCodeM.SetFocus
   Exit Sub
End If
If txtInvestorM.Text = "" Then
    MsgBox "Please Fill Investor Name"
    txtInvestorM.SetFocus
    Exit Sub
End If
If Len(TxtClientCodeM.Text) < 8 Then
   MsgBox "Client Code Can Not Be Left Blank"
   Exit Sub
End If
If CmbAmcM.Text = "" Then
   MsgBox "Select The AMC", vbInformation, "Wealthmaker"
   CmbAmcM.SetFocus
   Exit Sub
End If
If (CmbSipStpM.Text = "SIP") Then
    If OptSIPFM.Value = False And OptSIPRM.Value = False Then
       MsgBox "Please select either Fresh of Renewal", vbInformation
       Exit Sub
    End If
End If
If ImEntryDtM.Text = "__/__/____" Then
   MsgBox "Transaction Date Can Not Be Left Blank"
   ImEntryDtM.SetFocus
   Exit Sub
End If
If OptSIPFM.Value = True Then
    If optchequeM.Value = True Or optdraftM.Value = True Or OptEcsM.Value = True Or OptRTGSM.Value = True Or OptFTM.Value = True Then  'vinod ECS 23/05/2005
         If Trim(txtChqNoM.Text) = "" Then
            If optchequeM.Value = True Then
                MsgBox "Please Fill Cheque No. ", vbInformation
            ElseIf optdraftM.Value = True Then
                MsgBox "Please Fill Draft No. ", vbInformation
            ElseIf OptEcsM.Value = True Then
                MsgBox "Please Fill Bank A/c No. ", vbInformation
            ElseIf OptRTGSM.Value = True Then
                MsgBox "Please Fill UTR No. ", vbInformation
            ElseIf OptFTM.Value = True Then
                MsgBox "Please Fill Bank A/c No. ", vbInformation
            End If
            txtChqNoM.SetFocus
            Exit Sub
         End If
         If dtChqDateM.Text = "__/__/____" Then
            If optcheque.Value = True Then
                MsgBox "Cheque Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf optdraft.Value = True Then
                MsgBox "Draft Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf OptEcs.Value = True Then
                MsgBox "ECS Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf OptRTGS.Value = True Then
                MsgBox "UTR Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            ElseIf OptFT.Value = True Then
                MsgBox "Fund Transfer Date Can Not Be Left Blank", vbInformation, "Wealthmaker"
            End If
            dtChqDateM.SetFocus
         Exit Sub
         End If
    End If
End If

'-------------------------------------------------maya
If cmbTranTypeM.Text = "SWITCH IN" And (optchequeM.Value = True Or optdraftM.Value = True Or OptEcsM.Value = True Or optcashM.Value = True Or OptRTGSM.Value = True Or OptFTM.Value = True) Then
   MsgBox "Please Select 'Other' Option", vbInformation, "Wealthmaker"
   cmbTranTypeM.SetFocus
   Exit Sub
End If
'-------------------------------------------------
If Val(TxtAmountM.Text) = 0 Then
   MsgBox "Amount Can Not Zero"
   TxtAmountM.SetFocus
   Exit Sub
End If
If TxtAmountM.Text = "" Then
   MsgBox "Amount Can Not Left Blank"
   TxtAmountM.SetFocus
   Exit Sub
End If
If cmbBusiBranchM.Text <> "" Then
   Str = Split(cmbBusiBranchM.Text, "#")
   MyBranchCode = Str(1)
End If
If TxtSchemeM.Text <> "" Then
    str1 = Split(TxtSchemeM.Text, "=")
    MySchCode = str1(1)
End If


If MySchCode = "" Or TxtSchemeM.Text = "" Then
   MsgBox "Scheme Cant Be Left Blank", vbInformation, "Wealthmaker"
   Exit Sub
End If
 
If ChkAtmTransactionM.Value = 1 Then
    If get_ATM_scheme_amt_condition(MySchCode, Val(TxtAmountM)) = False Then
        MsgBox "Minimum Amount condition to have Reliance ATM Card is not being fulfilled ", vbCritical
        Exit Sub
    End If
End If

'-------------------------------------------Duplicate Check Condition-----------------------------------
If cmbTranTypeA.Text = "PURCHASE" Then
    If BaseTranCode = "0" Or BaseTranCode = "" Then
        If CmbSipStpM.Text = "SIP" And OptEcsM.Value = True Then
           ' Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' AND SCH_CODE='" & MySchCode & "' and amount=" & Val(TxtAmountM) & " AND TRAN_CODE <> '" & MyTranCode & "'")
        Else
            'vinit 19 dec 2015
          '  sql = "select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' AND SCH_CODE='" & MySchCode & "' "
            
                'vinit 5-dec-2015
                'If txtChqNoM.Text <> "" And txtChqNoM.Text <> 0 Then
                '    sql = sql & " AND CHEQUE_NO='" & txtChqNoM.Text & "' "
                'End If
                
            
           ' sql = sql & "  and amount=" & Val(TxtAmountM) & " AND TRAN_CODE <> '" & MyTranCode & "' AND (ASA<>'C' OR ASA IS NULL)"
           ' Set ARCODE = MyConn.Execute(sql)
            
            'If ARCODE.EOF Then
            'Else
            '    MsgBox "Sorry,This Transaction Has Been Already Punched", vbInformation, "Wealthmaker"
            '    Exit Sub
            'End If
            'ARCODE.Close
        End If
        
        Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and  app_no='" & TxtAppnoM.Text & "' AND TRAN_CODE <> '" & MyTranCode & "' AND MOVE_FLAG1 IS NULL AND SIP_ID IS NULL AND (ASA<>'C' OR ASA IS NULL)")
        If ARCODE.EOF Then
        Else
            MsgBox "Sorry,This App No. in Transaction has Been Already Punched for this Company", vbInformation, "Wealthmaker"
            Exit Sub
        End If
        ARCODE.Close
        'vinit 19 dec 2015
        'If Not (CmbSipStpM.Text = "SIP" And OptEcsM.Value = True) Then
        '    Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where BUSINESS_RMCODE='" & TxtBusiCodeM & "'  and amount=" & Val(TxtAmountM) & " AND TRAN_CODE <> '" & MyTranCode & "' AND (ASA<>'C' OR ASA IS NULL)")  'and CHEQUE_NO='" & txtChqNoM.Text & "'
        '    If ARCODE.EOF Then
        '    Else
        '        MsgBox "Sorry,This Transaction Has Been Already Punched", vbInformation, "Wealthmaker"
        '        Exit Sub
        '    End If
        'End If
    Else
        If CmbSipStpM.Text = "SIP" And OptEcsM.Value = True Then
            'Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' AND SCH_CODE='" & MySchCode & "'  and amount=" & Val(TxtAmountM) & " AND TRAN_CODE <> '" & MyTranCode & "' AND TRAN_CODE<>'" & BaseTranCode & "'")
        Else
        'vinit 19 dec 2015
         '   Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' AND SCH_CODE='" & MySchCode & "'  and amount=" & Val(TxtAmountM) & " AND TRAN_CODE <> '" & MyTranCode & "' AND TRAN_CODE<>'" & BaseTranCode & "' AND (ASA<>'C' OR ASA IS NULL)")  'AND CHEQUE_NO='" & txtChqNoM.Text & "'
         '   If ARCODE.EOF Then
         '   Else
         '       MsgBox "Sorry,This Transaction Has Been Already Punched", vbInformation, "Wealthmaker"
         '       Exit Sub
         '   End If
         '   ARCODE.Close
        End If
        
        Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and  app_no='" & TxtAppnoM.Text & "' AND TRAN_CODE <> '" & MyTranCode & "' AND TRAN_CODE<>'" & BaseTranCode & "' AND (ASA<>'C' OR ASA IS NULL) ")
        If ARCODE.EOF Then
        Else
            MsgBox "Sorry,This App No. in  Transaction Has Been Already Punched for This Company", vbInformation, "Wealthmaker"
            Exit Sub
        End If
        ARCODE.Close
        
        'vinit 19 dec 2015
        'If Not (CmbSipStpM.Text = "SIP" And OptEcsM.Value = True) Then
        '    Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where BUSINESS_RMCODE='" & TxtBusiCodeM & "' and amount=" & Val(TxtAmountM) & " AND TRAN_CODE <> '" & MyTranCode & "' AND TRAN_CODE<>'" & BaseTranCode & "' AND (ASA<>'C' OR ASA IS NULL)")  'and CHEQUE_NO='" & txtChqNoM.Text & "'
        '    If ARCODE.EOF Then
        '    Else
        '        MsgBox "Sorry,This Transaction Has Been Already Punched", vbInformation, "Wealthmaker"
        '        Exit Sub
        '    End If
        'End If
    End If
    '----------------------------------------------------------------------------------------------------
End If
MyConn.BeginTrans
If MyTranCode = "" Then
    MsgBox "Please Double Click at the record you want to update", vbInformation
Else
'---------------------------------------------Received ------------------------------------------
    MyRecdM = 0
'    If MyNat = "C" Then
'        If Mid(TxtClientCodeM, 1, 1) = "3" Then
'             MyRecdM = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MSH('" & TxtClientCodeM.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY')," & Val(TxtAmountM.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'        Else
'             MyRecdM = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MSH('" & TxtClientCodeM.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY')," & Val(TxtAmountM.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'        End If
'    Else
'        If Mid(TxtClientCodeM, 1, 1) = "3" Then
'             MyRecdM = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MS('" & TxtClientCodeM.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY')," & Val(TxtAmountM.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'        Else
'             MyRecdM = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MS('" & TxtClientCodeM.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY')," & Val(TxtAmountM.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'        End If
'    End If

'    asetflag = SqlRet("select nvl(get_scheme_nature_trail('" & UCase(MySchCode) & "'),'') a from dual")
    
'    sql = "select get_recd_mf('" & asetflag & "',TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY'),'" & MyBranchCode & "','" & TxtClientCodeM.Text & "'," & Val(TxtAmountM.Text) & ",'" & MyAmcCode & "','" & MySchCode & "','" & cmbTranTypeM.Text & "') from dual"
    
    sql = "select nvl(Upfront_recd_NONang('" & MyTranCode & "'),0)+nvl(trail_recd_ang('" & MyTranCode & "'),0) from dual"
    
    MyRecdM = SqlRet(sql)

    If Val(ImExpenses1.Text) > MyRecdM And MyRecdM <> 0 Then
       MsgBox "Payable Can Not Be Greater Than Receivable", vbInformation
       Exit Sub
    End If
'--------------------------------------------------------------------------------------------------
    sql = ""
    ',RMCODE=" & MyRmCodeM & "
    sql = "UPDATE transaction_mf_Temp1 SET target_switch_scheme='" & MyCloseSchCodeM & "',ATM_FLAG=" & ChkAtmTransactionM.Value & ", AC_HOLDER_CODE='" & TxtAcHolderM.Text & "',EXP_RATE=" & Val(ImnExp_Per1.Text) & ",EXP_AMOUNT=" & Val(ImExpenses1.Text) & ", CLIENT_CODE=" & Val(Label42.Caption) & ",INVESTOR_NAME='" & txtInvestorM & "',UPDATE_DATE= tO_DATE(TO_CHAR(SYSDATE,'mm/dd/yyyy hh12:mi:ss AM'),'mm/dd/yyyy hh12:mi:ss AM'),MODIFY_DATE= tO_DATE(TO_CHAR(SYSDATE,'mm/dd/yyyy'),'mm/dd/yyyy'),MODIFY_USER=" & Glbloginid & ", BUSINESS_RMCODE=" & TxtBusiCodeM & ",CLIENT_OWNER=" & TxtBusiCodeM & ", BUSI_BRANCH_CODE=" & MyBranchCode & ""
    sql = sql & " ,PANNO='" & TxtPanM & "',MUT_CODE='" & MyAmcCode & "',SCH_CODE='" & UCase(MySchCode) & "',TR_DATE=TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY'), "
    sql = sql & " TRAN_TYPE='" & cmbTranTypeM.Text & "',APP_NO='" & TxtAppnoM.Text & "',FOLIO_NO='" & TxtFolioNoM.Text & "'"
    
    If ImSipStartDtM.Text <> "__/__/____" Then
        sql = sql & " ,sip_start_date = TO_DATE('" & MyDate(ImSipStartDtM) & "','MM/DD/YYYY')"
    End If
    
    If ImSipEndDtM.Text <> "__/__/____" Then
        sql = sql & " ,sip_end_date = TO_DATE('" & MyDate(ImSipEndDtM) & "','MM/DD/YYYY')"
    End If
    
    If cmbTranTypeM.Text = "SWITCH IN" Or CmbSipStpM.Text = "STP" Then
        sql = sql & ",SWITCH_SCHEME='" & MySwitchSchCodeM & "',SWITCH_FOLIO='" & TxtSwitchFolioM.Text & "'"
    End If
    
    If optchequeM.Value = True Then
        paymode = "C"
    ElseIf optdraftM.Value = True Then
        paymode = "D"
    ElseIf optcashM.Value = True Then
        paymode = "H"
    ElseIf OptEcsM.Value = True Then
        paymode = "E"
    ElseIf OptOthersm.Value = True Then
        paymode = "R"
    ElseIf OptRTGSM.Value = True Then
        paymode = "U"
    ElseIf OptFTM.Value = True Then
        paymode = "B"
    End If
    sql = sql & ",PAYMENT_MODE='" & paymode & "',"
'    If CmbCancel.ListIndex = 1 Then
'        Sql = Sql & "ASA='C',"
'    End If
    If (optchequeM.Value = True Or optdraftM.Value = True Or OptEcsM.Value = True Or OptRTGSM.Value = True Or OptFTM.Value = True) And OptSIPFM.Value = True Then 'vinod ECS 23/05/2005
         sql = sql & " bank_name='" & Trim(cmbBankNameM.Text) & "',CHEQUE_NO='" & Trim(txtChqNoM.Text) & "',CHEQUE_DATE=to_date('" & Trim(dtChqDateM.Text) & "','DD-MM-YYYY'),"
    End If
    sql = sql & " AMOUNT= " & Val(TxtAmountM.Text) & ", "
    '-------------------------Frequency & Installments----------------------------
    If Trim(CmbSipStpM.Text) = "SIP" Or Trim(CmbSipStpM.Text) = "STP" Then
        sql = sql & " frequency= " & MyFreq1(1) & ", "
        sql = sql & " installments_no= " & Txtinstallments1.Text & ", "
    End If
    '------------------------------------------------------------------------------
    sql = sql & " SIP_TYPE='" & CmbSipStpM.Text & "'"
    If OptSIPFM.Value = True Then
        sql = sql & ", SIP_fr='F'"
    ElseIf OptSIPRM.Value = True Then
        sql = sql & ", SIP_fr='R'"
    Else
        sql = sql & ", SIP_fr=NULL"
    End If
    If optRegM.Value = True Then
        sql = sql & ", dispatch='R'"
    ElseIf optNFOM.Value = True Then
        sql = sql & ", dispatch='N'"
    End If
    
    sql = sql & ",LEAD_NAME='" & TxtLeadNameM & "',SOURCE_CODE='" & TxtClientCodeM.Text & "',doc_id='" & Trim(txtdocID.Text) & "',micro_investment='" & Cmbsubinvu.Text & "',cob_flag='" & chkcobM.Value & "',FREEDOM_SIP_FLAG='" & ChkFreedomM.Value & "',swp_flag='" & chkSWPM.Value & "'  WHERE TRAN_CODE='" & MyTranCode & "' "
    MyConn.Execute sql
End If

sql = ""
sql = "UPDATE WM_TRAN_SIP SET AMOUNT_SIP=" & Val(TxtAmountM.Text) & ""
If Txtinstallments1.Text <> "" Then
    sql = sql & " , TOTAL_SIP=" & Txtinstallments1.Text & " "
End If
sql = sql & " where base_tran_code='" & MyTranCode & "'"
MyConn.Execute sql

'MyBrokRecd = SqlRet("select nvl(Upfront_Ope_Recd_Temptran1_mf('" & MyTranCode & "'),0) from dual")
'MyBrokPay = SqlRet("select nvl(Upfront_Ope_paid_Temptran2_mf('" & MyTranCode & "'),0) from dual")
'MyMargin = (Val(MyBrokRecd) - Val(MyBrokPay)) * 11
'MyConn.Execute ("update transaction_mf_temp1 set brok_recd=" & MyBrokRecd & ",brok_payble=" & MyBrokPay & ",margin=" & MyBrokRecd & " - " & MyBrokPay & " where tran_code='" & MyTranCode & "'")

If Val(Label42.Caption) <> 0 And Val(TxtAmountM.Text) > 3000 Then
    MyConn.Execute ("update investor_master set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,pan='" & TxtPanM.Text & "' where inv_code='" & Val(Label42.Caption) & "' and pan is null ")
End If
busi_change_flag = True
busi_br_tr = ""
If cmbTranTypeA.Text = "PURCHASE" Then
    MyConn.Execute "CALL PRCINSERTWMTRANSIP('" & MyTranCode & "',2)"
    If GlbroleId = 29 Then
        MyConn.Execute "CALL PRCINSERTWMTRANSIP_HIS('" & MyTranCode & "','" & GlbroleId & "')"
    End If
    
'    SIPADV = SqlRet("select BAJAJ_SIP_ADVANTAGE1_SIP_TEMP('" & MyTranCode & "') from dual")
'    If SqlRet("SELECT BAJAJ_SIP_ADVANTAGE1_SIP_TEMP('" & MyTranCode & "') FROM DUAL") > 0 Then
'        MyUniqueKey = SqlRet("select unique_id from wm_tran_sip where base_tran_code='" & MyTranCode & "'")
'        If SqlRet("select * from transaction_mf_Temp1 A where SIP_ID='" & MyUniqueKey & "'") > 0 Then
'            SQL = ""
'            ',RMCODE=" & MyRmCodeM & "
'            SQL = "UPDATE transaction_mf_Temp1 SET brok_recd=" & MyBrokRecd * 11 & ",brok_payble=" & MyBrokPay * 11 & ",margin=" & MyMargin & ",SIP_ADV=" & SIPADV & ", AC_HOLDER_CODE='" & TxtAcHolderM.Text & "',EXP_RATE=" & Val(ImnExp_Per1.Text) & ",EXP_AMOUNT=" & Val(ImExpenses1.Text) & ", CLIENT_CODE=" & Val(Label42.Caption) & ",INVESTOR_NAME='" & txtInvestorM & "',UPDATE_DATE= tO_DATE(TO_CHAR(SYSDATE,'mm/dd/yyyy hh12:mi:ss AM'),'mm/dd/yyyy hh12:mi:ss AM'),MODIFY_DATE= tO_DATE(TO_CHAR(SYSDATE,'mm/dd/yyyy'),'mm/dd/yyyy'),MODIFY_USER=" & Glbloginid & ", BUSINESS_RMCODE=" & TxtBusiCodeM & ",CLIENT_OWNER=" & TxtBusiCodeM & ", BUSI_BRANCH_CODE=" & MyBranchCode & ""
'            SQL = SQL & " ,PANNO='" & TxtPanM & "',MUT_CODE='" & MyAmcCode & "',SCH_CODE='" & UCase(MySchCode) & "',TR_DATE=TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY'), "
'            SQL = SQL & " TRAN_TYPE='" & cmbTranTypeM.Text & "',"
'            If optchequeM.Value = True Then
'                paymode = "C"
'            ElseIf optdraftM.Value = True Then
'                paymode = "D"
'            ElseIf optcashM.Value = True Then
'                paymode = "H"
'            ElseIf OptEcsM.Value = True Then
'                paymode = "E"
'            ElseIf OptOthersm.Value = True Then
'                paymode = "R"
'            ElseIf OptRTGSM.Value = True Then
'                paymode = "U"
'            ElseIf OptFTM.Value = True Then
'                paymode = "B"
'            End If
'            SQL = SQL & "PAYMENT_MODE='" & paymode & "',"
'            If (optchequeM.Value = True Or optdraftM.Value = True Or OptEcsM.Value = True Or OptRTGSM.Value = True Or OptFTM.Value = True) And OptSIPFM.Value = True Then 'vinod ECS 23/05/2005
'                 SQL = SQL & " bank_name='" & Trim(cmbBankNameM.Text) & "',CHEQUE_NO='" & Trim(txtChqNoM.Text) & "',CHEQUE_DATE=to_date('" & Trim(dtChqDateM.Text) & "','DD-MM-YYYY'),"
'            End If
'            SQL = SQL & " AMOUNT= " & Val(TxtAmountM.Text) & ", "
'            '-------------------------Frequency & Installments----------------------------
'            If Trim(CmbSipStpM.Text) = "SIP" Or Trim(CmbSipStpM.Text) = "STP" Then
'                SQL = SQL & " frequency= " & MyFreq1(1) & ", "
'                SQL = SQL & " installments_no= " & Txtinstallments1.Text & ", "
'            End If
'            '------------------------------------------------------------------------------
'            SQL = SQL & " SIP_TYPE='" & CmbSipStpM.Text & "'"
'            If OptSIPFM.Value = True Then
'                SQL = SQL & ", SIP_fr='F'"
'            ElseIf OptSIPRM.Value = True Then
'                SQL = SQL & ", SIP_fr='R'"
'            Else
'                SQL = SQL & ", SIP_fr=NULL"
'            End If
'            SQL = SQL & ",LEAD_NAME='" & TxtLeadNameM & "',SOURCE_CODE='" & TxtClientCodeM.Text & "'  WHERE SIP_ID='" & MyUniqueKey & "' "
'            MyConn.Execute SQL
'       Else
'            If (optchequeM.Value = True Or optdraftM.Value = True Or OptEcsM.Value = True Or OptRTGSM.Value = True Or OptFTM.Value = True) And OptSIPFM.Value = True Then
'                SQL = "INSERT INTO transaction_mf_Temp1"
'                SQL = SQL & "( "
'                SQL = SQL & " CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
'                SQL = SQL & " APP_NO,FOLIO_NO,PAYMENT_MODE,BANK_NAME,CHEQUE_NO,CHEQUE_DATE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,SIP_ADV,BROK_RECD,BROK_PAYBLE,MARGIN,SIP_ID,MOVE_FLAG1,BASE_TRAN_CODE,sip_fr "
'            Else
'                SQL = " INSERT INTO transaction_mf_Temp1"
'                SQL = SQL & "( "
'                SQL = SQL & " CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
'                SQL = SQL & " APP_NO,FOLIO_NO,PAYMENT_MODE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,SIP_ADV,REF_TRAN_CODE,BROK_RECD,BROK_PAYBLE,MARGIN,SIP_ID,MOVE_FLAG1,BASE_TRAN_CODE,sip_fr "
'            End If
'            SQL = SQL & ")VALUES(" & Val(Label42.Caption) & ",'" & TxtBusiCodeM & "','" & Glbloginid & "','" & TxtBusiCodeM & "'," & MyBranchCode & ",'" & TxtPanM & "','" & MyAmcCode & "','" & MySchCode & "', "
'            SQL = SQL & " TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY'),'" & cmbTranTypeM.Text & "',"
'            SQL = SQL & " '" & TxtAppnoM.Text & "','" & TxtFolioNoM.Text & "',"
'            If optchequeM.Value = True Then
'                paymode = "C"
'            ElseIf optdraftM.Value = True Then
'                paymode = "D"
'            ElseIf optcashM.Value = True Then
'                paymode = "H"
'            ElseIf OptEcsM.Value = True Then
'                paymode = "E"
'            ElseIf OptOthersm.Value = True Then
'                paymode = "R"
'            ElseIf OptRTGSM.Value = True Then
'                paymode = "U"
'            ElseIf OptFTM.Value = True Then
'                paymode = "B"
'            End If
'            SQL = SQL & "'" & paymode & "'"
'            If (optchequeM.Value = True Or optdraftM.Value = True Or OptEcsM.Value = True Or OptRTGSM.Value = True Or OptFTM.Value = True) And OptSIPFM.Value = True Then  'vinod ECS 23/05/2005
'                 SQL = SQL & ",'" & Trim(cmbBankNameM.Text) & "','" & Trim(txtChqNoM.Text) & "',to_date('" & Trim(dtChqDateM.Text) & "','DD-MM-YYYY'),"
'            Else
'                SQL = SQL & ","
'            End If
'            SQL = SQL & " " & Val(TxtAmountM.Text) * 11 & ""
'            SQL = SQL & ",'" & CmbSipStpM.Text & "'"
'            SQL = SQL & ",'" & TxtLeadNameM & "','" & TxtClientCodeM.Text & "','" & txtInvestorM & "'," & Val(ImnExp_Per1.Text) & "," & Val(ImExpenses1.Text) & ",'" & TxtAcHolderM.Text & "','" & MyFreq(1) & "','" & Trim(Txtinstallments1.Text) & "'," & Val(SIPADV) & " ," & Val(MyBrokRecd) * 11 & "," & Val(MyBrokPay) * 11 & "," & Val(MyMargin) & "," & MyUniqueKey & ",'ADVSIP','" & MyPrintTranCode & "'"
'            If OptSIPFM.Value = True Then
'                SQL = SQL & ",'F'"
'            ElseIf OptSIPRM.Value = True Then
'                SQL = SQL & ",'R'"
'            Else
'                SQL = SQL & ",NULL"
'            End If
'            SQL = SQL & ")"
'            MyConn.Execute SQL
'       End If
'    End If
End If

If dtDropDate.Text <> "__/__/____" Then
   MyConn.Execute "update wm_tran_sip set SIP_DEACTIVATION_DATE=TO_DATE('" & dtDropDate & "','DD/MM/YYYY') where base_tran_code='" & MyTranCode & "'"
Else
   MyConn.Execute "update wm_tran_sip set SIP_DEACTIVATION_DATE=NULL where base_tran_code='" & MyTranCode & "'"
End If
MyConn.CommitTrans
''---------------------FOR PER APP RECD PAID-----------------------------------
Dim ins_mar As New ADODB.Command
Set ins_mar.ActiveConnection = MyConn
ins_mar.CommandType = adCmdStoredProc
ins_mar.CommandText = "UPDATE_MF_MARGIN_TRAN"
ins_mar.Parameters.Append ins_mar.CreateParameter("TRANCODE", adVarChar, adParamInput, 14, MyTranCode)
ins_mar.Execute
Set ins_mar = Nothing
''------------------------------------------------------------------------------
MsgBox "Record Has Been SucessFully Updated", vbInformation, "Wealthamker"
ChkAtmTransactionM.Value = 0
Call gridfill(2)
Call cmdResetM_Click
Exit Sub
err:
MsgBox err.Description
Resume
End Sub

Private Sub cmdSelectSch_Click()
If cmbTranTypeA.Text = "" Then
    MsgBox "Please Select Transaction Type First"
    cmbTranTypeA.SetFocus
    Exit Sub
End If
  frmSchemeSearch.searchschflag = 0
  call_frmschemesearch
End Sub

Private Sub cmdSelectSchClose_Click()
If ChkClose.Value = 1 Then
    frmSchemeSearch.searchschflag = 2
    Call call_frmschemesearch
End If
End Sub

Private Sub cmdSelectSchCloseM_Click()
If ChkCloseM.Value = 1 Then
    frmSchemeSearch.searchschflag = 5
    call_frmschemesearchM
End If
End Sub

Private Sub cmdSelectSchm_Click()
If OptChkSwitchM = 0 Then
    frmSchemeSearch.searchschflag = 3
    
Else
    frmSchemeSearch.searchschflag = 4
End If
 call_frmschemesearchM
End Sub

Private Sub cmdShow_Click()
On Error Resume Next
Dim currentForm As Form
Dim rsGetDocPath As New ADODB.Recordset
Dim rsFind As New ADODB.Recordset
Dim rs_get_rm As New ADODB.Recordset
Dim rs_get_invsrc As New ADODB.Recordset
Dim rsnat As ADODB.Recordset
Dim sql As String
    
    rsGetDocPath.open "select a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
        If IsNull(rsGetDocPath("sch_code")) = False Then
            
            sql = "select osch_code sch_code,osch_name sch_name,longname Short_name,name,iss_name mut_name from other_product o, product_master p,iss_master i where osch_code='" & rsGetDocPath("sch_code") & "' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null)  "
            sql = sql & " union all "
            sql = sql & " select sch_code,sch_name,Short_name,'MF' name,mut_name from scheme_info s,mut_fund m where s.sch_code='" & rsGetDocPath("sch_code") & "' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') And s.sch_code in (select sch_code from mp_scheme_master sm where sm.active_status='Active' )  "
            rsFind.open sql, MyConn, adOpenForwardOnly
            
            If SSTab1.Tab = 0 Then  ''add button
                Set rsnat = New ADODB.Recordset
                rsnat.open "select nvl(get_scheme_nature('" & UCase(rsGetDocPath("sch_code")) & "'),'') a from dual", MyConn, adOpenForwardOnly
                If rsnat("a") = "O" Then
                    frmtransactionmf.Label2(16).Caption = "Expenses agnst. Trail%"
                    frmtransactionmf.Label2(17).Caption = "Expenses agnst. Trail(Rs.)"
                    frmtransactionmf.MyNat = "O"
                Else
                    frmtransactionmf.Label2(16).Caption = "Expenses%"
                    frmtransactionmf.Label2(17).Caption = "Expenses(Rs.)"
                    frmtransactionmf.MyNat = "C"
                End If
                rsnat.Close
                Set rsnat = Nothing

                
                If frmtransactionmf.OptChkSwitch.Value = 0 Then
                    frmtransactionmf.CmbAmcA.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameA.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameA.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            frmtransactionmf.TxtSchemeA.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameA.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.CmbAmcA.SetFocus
                    If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                        frmtransactionmf.ChkAtmTransactionA.Value = 0
                        frmtransactionmf.ChkAtmTransactionA.Enabled = True
                    Else
                        frmtransactionmf.ChkAtmTransactionA.Value = 0
                        frmtransactionmf.ChkAtmTransactionA.Enabled = False
                    End If
                    
                Else
                    frmtransactionmf.CmbSwitchAmcA.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameA.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameA.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            'frmtransactionmf.lstlongnameA.Selected(I) = True
                            frmtransactionmf.TxtSwitchSchemeA.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameA.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.TxtSwitchSchemeA.SetFocus
                End If
            Else  ''modification
            
                Set rsnat = New ADODB.Recordset
                rsnat.open "select nvl(get_scheme_nature('" & UCase(rsGetDocPath("sch_code")) & "'),'') a from dual", MyConn, adOpenForwardOnly
                If rsnat("a") = "O" Then
                    frmtransactionmf.Label2(16).Caption = "Expenses agnst. Trail%"
                    frmtransactionmf.Label2(17).Caption = "Expenses agnst. Trail(Rs.)"
                    frmtransactionmf.MyNat = "O"
                Else
                    frmtransactionmf.Label2(16).Caption = "Expenses%"
                    frmtransactionmf.Label2(17).Caption = "Expenses(Rs.)"
                    frmtransactionmf.MyNat = "C"
                End If
                rsnat.Close
                Set rsnat = Nothing
                
                 If frmtransactionmf.OptChkSwitchM.Value = 0 Then
                    frmtransactionmf.CmbAmcM.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameM.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameM.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            frmtransactionmf.TxtSchemeM.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameM.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.CmbAmcM.SetFocus
                    
                    If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                        frmtransactionmf.ChkAtmTransactionM.Value = 0
                        frmtransactionmf.ChkAtmTransactionM.Enabled = True
                    Else
                        frmtransactionmf.ChkAtmTransactionM.Value = 0
                        frmtransactionmf.ChkAtmTransactionM.Enabled = False
                    End If
                Else
                    frmtransactionmf.CmbSwitchAmcM.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameM.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameM.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            frmtransactionmf.TxtSwitchSchemeM.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameM.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.TxtSwitchSchemeM.SetFocus
                End If
            End If

        Else
            CmbAmcA.ListIndex = -1
            CmbAmcA_Click
            frmtransactionmf.TxtSchemeA.Text = ""
        End If
        
        If IsNull(rsGetDocPath("Inv_code")) = False Then
        
            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,'',a.pan " & _
                    "FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) " & _
                    "and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 and a.inv_code=" & rsGetDocPath("Inv_code")
            rs_get_invsrc.open sql, MyConn, adOpenForwardOnly
                    
            MyLoggedUserid = SqlRet("select loggeduserid from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
            MyMainCode = SqlRet("select main_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
            If MyLoggedUserid = "PROC" Then
               MyUpdProc = SqlRet("select NVL(UPD_PROC,'N') from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & " and dob is not null")
               If (Mid(rsGetDocPath("Inv_code"), 1, 1) <> 3) Then
               If MyUpdProc = "N" Or MyUpdProc = "0" Then
                  MsgBox "Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account", vbInformation
                  Unload Me
                  frmactopen.Show
                  frmactopen.ZOrder
                  frmactopen.txtclientcode = MyMainCode
                  frmactopen.cmdview_Click
                  Exit Sub
               End If
               End If
            End If
            If frmtransactionmf.SSTab1.Tab = 0 Then
                frmtransactionmf.TxtClientCodeA = Mid(rsGetDocPath("Inv_code"), 1, 8)
                frmtransactionmf.Label32.Caption = rsGetDocPath("Inv_code")
                frmtransactionmf.txtInvestorA = rs_get_invsrc("INVESTOR_NAME") ''name
                
                
                If Mid(rsGetDocPath("Inv_code"), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanA = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                    frmtransactionmf.TxtAcHolderA = SqlRet("select client_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
                    If frmtransactionmf.TxtAcHolderA.Text = "" Or Len(frmtransactionmf.TxtAcHolderA.Text) < 6 Then
                        MsgBox "Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same", vbInformation, "Wealthmaker"
                        Me.MousePointer = vbIconPointer
                        Exit Sub
                    End If
                Else
                    frmtransactionmf.TxtPanA = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtBusiCodeA.Text = rsGetDocPath("busi_rm_code")
                    'frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                End If
                Me.MousePointer = vbNormal
                'Unload Me
            ElseIf frmtransactionmf.SSTab1.Tab = 1 Then
                frmtransactionmf.TxtClientCodeM = Mid(rsGetDocPath("Inv_code"), 1, 8)
                frmtransactionmf.txtInvestorM = rs_get_invsrc("INVESTOR_NAME") ''name
                If Mid(rsGetDocPath("Inv_code"), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanM = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
                    frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                Else
                    frmtransactionmf.TxtPanA = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
                    frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                End If
                frmtransactionmf.Label42.Caption = rsGetDocPath("Inv_code")
                Me.MousePointer = vbNormal
                Unload Me
            End If
            If frmtransactionmf.SSTab1.Tab = 0 Then
                Call frmtransactionmf.TxtBusiCodeA_LostFocus
            Else
                Call frmtransactionmf.TxtBusiCodeM_LostFocus
            End If
            
        Else
                frmtransactionmf.TxtClientCodeA = ""
                frmtransactionmf.Label32.Caption = ""
                frmtransactionmf.txtInvestorA = ""
                If Mid(rsGetDocPath("Inv_code"), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanA = ""
                    frmtransactionmf.TxtBusiCodeA = ""
                    frmtransactionmf.TxtAcHolderA = ""
                Else
                    frmtransactionmf.TxtPanA = ""
                    frmtransactionmf.TxtBusiCodeA = ""
                End If
        
        End If
        
        
        TxtBusiCodeA.Text = rsGetDocPath("busi_rm_code")
        ImEntryDtA.Text = rsGetDocPath("busi_tr_date")
        If GlbroleId = 212 Then
            CmbAmcA.Enabled = False
            TxtSchemeA.Enabled = False
            ImEntryDtA.Enabled = False
        Else
            CmbAmcA.Enabled = True
            TxtSchemeA.Enabled = True
            ImEntryDtA.Enabled = True
        End If
        If rsGetDocPath("expense") > 0 Then
            ImNExp_PER.Text = rsGetDocPath("expense")
        Else
            ImNExp_PER.Text = 0
        End If
        cmbBusiBranch.Text = rsGetDocPath("busi_branch_code") & Space(100) & "#" & rsGetDocPath("branch_name")
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
    Set rs_get_invsrc = Nothing
    Set rs_get_rm = Nothing
    Set rsFind = Nothing
    
    
     '''''''''''''''''''''''''''''''''''''''''''''''''CROSS CHANNEL VALIDATION--------------------------------------------------------
    rsGetDocPath.open "select a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
            vApprovalFlag = SqlRet("select wealthmaker.fn_check_for_approval_all('" & txtdocID.Text & "')  from dual")
            'vApprovalFlag 2  Approval request for this transaction has already been raised.
            'vApprovalFlag 4  Approval request for this transaction has rejected by Management.
            
            If vApprovalFlag = 2 Then
                MsgBox "Approval request for this transaction has already been raised.", vbInformation, strBajaj
                Exit Sub
            End If
            
            If vApprovalFlag = 4 Then
                MsgBox "Approval request for this transaction has been rejected by Management.", vbInformation, strBajaj
                Exit Sub
            End If
        
            If Label32.Caption = "" Then
                MsgBox "Information not present", vbInformation, strBajaj
                Exit Sub
            End If
            
            Dim cmd As New ADODB.Command
            Dim vResult As String
            Set cmd.ActiveConnection = MyConn
            cmd.CommandType = adCmdStoredProc
            cmd.CommandText = "WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO"
            cmd.Parameters.Append cmd.CreateParameter("PCOMMON_ID", adVarChar, adParamInput, 15, txtdocID.Text)
            cmd.Parameters.Append cmd.CreateParameter("PSUB_CLIENT_CD", adVarChar, adParamInput, 15, Label32.Caption)
            cmd.Parameters.Append cmd.CreateParameter("PLOGIN_ID", adVarChar, adParamInput, 15, Glbloginid)
            cmd.Parameters.Append cmd.CreateParameter("PCNT", adDouble, adParamOutput, 100)
            cmd.Execute
            vResult = IIf(IsNull(cmd("PCNT")), 0, cmd("PCNT"))
            Set cmd = Nothing
            
            If vResult > 0 Then
                frmCrossChannelValidate.formtype = "MF"
                frmCrossChannelValidate.CommonId = txtdocID.Text
                frmCrossChannelValidate.loggeduserid = Glbloginid
                frmCrossChannelValidate.Show
                frmCrossChannelValidate.ZOrder 0
            Else
                If Label32.Caption <> "" Then
                    Change_bt_Click (0)
                End If
            End If
    Else
        MsgBox "Information not present", vbInformation, strBajaj
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
End Sub

Public Sub SendForApproval()
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String
If Label32.Caption = "" Then
    MsgBox "Information not present", vbInformation, strBajaj
    Exit Sub
End If
Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.PRC_VALIDATE_CROSS_CHANNEL"
cmd.Parameters.Append cmd.CreateParameter("PCOMMON_ID", adVarChar, adParamInput, 15, txtdocID.Text)
cmd.Parameters.Append cmd.CreateParameter("PSUB_CLIENT_CD", adVarChar, adParamInput, 15, Label32.Caption)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 100)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing



If vResult = 0 Then
        MyConn.Execute "UPDATE WEALTHMAKER.TB_DOC_UPLOAD SET GI_REQUEST=1,REQUEST_BY='" & Glbloginid & "',REQUEST_DATE=SYSDATE,REQUEST_REMARKS='" & vErrMsg & "' where common_id='" & txtdocID.Text & "'"
        MsgBox "Request has been Raised Successfully", vbInformation
        Exit Sub
End If
End Sub
Private Sub CmdSwitchSchsearch_Click()
If cmbTranTypeA.Text = "" Then
    MsgBox "Please Select Transaction Type First"
    cmbTranTypeA.SetFocus
    Exit Sub
End If
frmSchemeSearch.searchschflag = 1
'OptChkSwitch.Value = 1
Call call_frmschemesearch
'OptChkSwitch.Value = 0
End Sub

Private Sub CmdTranCancel_Click()
fracancel.ZOrder
txtcanTrcode = txtTrNo.Text
mdatecancel = Format(ServerDateTime, "DD/MM/YYYY")
If txtcanTrcode = "" Then
    MsgBox "First Search The Ar To Wich You want To Cancel", vbInformation
    Exit Sub
End If
fracancel.Visible = True
txtcanTrcode.Enabled = False
Set rsReason = MyConn.Execute("select reason,reason_type from ar_reason where upper(trim(purpose))='C' order by reason")
CmbReson.Clear
While Not rsReason.EOF
    CmbReson.AddItem rsReason(0) & Space(50) & "$" & rsReason(1)
    rsReason.MoveNext
Wend
End Sub

Private Sub cmdview_Click()
cmdview.Enabled = False
FrmTransactionLog.Visible = False
Call gridfill(2)
cmdview.Enabled = True
ResetView_Click
Cmbsubinv.ListIndex = 0
Cmbsubinvu.ListIndex = 0


End Sub

Private Sub CmdViewLog_Click()
Dim sql As String
sql = ""
If LblTranCode.Caption = "" Then
   MsgBox "Please Select Any Transaction First", vbInformation
   Exit Sub
End If
sql = "SELECT * FROM VIEWTRANSACTION_MF_TEMP1_LOG where tran_code='" & Trim(LblTranCode.Caption) & "'"
Populate_Data sql
set_grid
SSTab1.Tab = 1
FrmTransactionLog.Visible = True
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


Private Sub set_grid()
On Error GoTo err1
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
    msfgClients.ColWidth(4) = "2000"
    msfgClients.Text = "ChangedField"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 5
    msfgClients.ColWidth(5) = "3000"
    msfgClients.Text = "PrevValue"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 6
    msfgClients.ColWidth(6) = "3000"
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




Private Sub Command1_Click()
TxtInstallmens.Enabled = True
TxtInstallmens.Locked = False
End Sub

Private Sub dtChqDate_Validate(Cancel As Boolean)
    If dtChqDate.Text <> "__/__/____" Then
        If Not (IsDate(dtChqDate.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            dtChqDate.SetFocus
            Cancel = True
        End If
    End If
End Sub



Private Sub dtChqDateM_Validate(Cancel As Boolean)
    If dtChqDateM.Text <> "__/__/____" Then
        If Not (IsDate(dtChqDateM.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            dtChqDateM.SetFocus
            Cancel = True
        End If
    End If
End Sub


Private Sub Form_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then SendKeys "{Tab}"


End Sub

Sub Form_Resize()
         ' Position the scroll bars:
         HScroll1.Left = 0
         VScroll1.Top = 0
         If Picture1.width > ScaleWidth Then
            HScroll1.Top = ScaleHeight - HScroll1.Height
         Else
            HScroll1.Top = ScaleHeight
         End If
         If Picture1.Height > HScroll1.Top Then
            VScroll1.Left = ScaleWidth - VScroll1.width
            If Picture1.width > VScroll1.Left Then
               HScroll1.Top = ScaleHeight - HScroll1.Height
            End If
         Else
            VScroll1.Left = ScaleWidth
         End If
         HScroll1.width = ScaleWidth
         If HScroll1.Top > 0 Then VScroll1.Height = HScroll1.Top
         ' Set the scroll bar ranges
         HScroll1.Max = Picture1.width - VScroll1.Left
         VScroll1.Max = Picture1.Height - HScroll1.Top
         HScroll1.SmallChange = Abs(HScroll1.Max \ 16) + 1
         HScroll1.LargeChange = Abs(HScroll1.Max \ 4) + 1
         VScroll1.SmallChange = Abs(VScroll1.Max \ 16) + 1
         VScroll1.LargeChange = Abs(VScroll1.Max \ 4) + 1
         HScroll1.ZOrder 0
         VScroll1.ZOrder 0
End Sub



Private Sub Frame12_Click()
    Frame12.ZOrder
End Sub

Private Sub Frame3_Click()
    Frame3.ZOrder
End Sub

Sub HScroll1_Change()
   Picture1.Left = -HScroll1.Value
End Sub
      
      
Private Sub ImSipEndDtA_Validate(Cancel As Boolean)
    If ImSipEndDtA.Text <> "__/__/____" Then
        If Not (IsDate(ImSipEndDtA.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipEndDtA.SetFocus
            Cancel = True
        End If
    End If
End Sub


Private Sub ImSipEndDtM_Validate(Cancel As Boolean)
    If ImSipEndDtM.Text <> "__/__/____" Then
        If Not (IsDate(ImSipEndDtM.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipEndDtM.SetFocus
            Cancel = True
        End If
    End If
End Sub


Private Sub ImSipStartDtA_Validate(Cancel As Boolean)
    If ImSipStartDtA.Text <> "__/__/____" Then
        If Not (IsDate(ImSipStartDtA.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipStartDtA.SetFocus
            Cancel = True
        End If
    End If
End Sub

Private Sub ImSipStartDtM_Validate(Cancel As Boolean)
    If ImSipStartDtM.Text <> "__/__/____" Then
        If Not (IsDate(ImSipStartDtM.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipStartDtM.SetFocus
            Cancel = True
        End If
    End If
End Sub


Private Sub Opt991_Click()
    Txtinstallments1.Text = "99"
    CmbFrequency1_Click
End Sub

Private Sub Opt991_GotFocus()
Txtinstallments1.Text = "99"
    CmbFrequency1_Click
End Sub

Private Sub Picture7_Click(Index As Integer)
On Error GoTo err1
Dim openfile As String, ARL As String, FL As String
Dim strFile As String
Dim strPath As String
Dim strFiletype() As String
Dim l_file As String
Dim rsGetDocPath As New ADODB.Recordset

    '    If lbtrancode.Caption = "lbtrancode" Then
    '        MsgBox "Please Select a transaction to view documents ", vbInformation
    '        Exit Sub
    '    End If
        
    'ARL = "C:\Program Files\Adobe\Reader 9.0\Reader\AcroRd32.exe" ', vbNormalFocus
    
    Me.MousePointer = 11

    rsGetDocPath.open "select doc_filename,doc_path from tb_doc_upload where common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
        strFile = rsGetDocPath("doc_filename")
        strPath = UCase(rsGetDocPath("doc_path"))
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
    
    ReDim strFiletype(1)
    strFiletype = Split(strFile, ".")
    l_file = "c:\doc\doc" & Glbloginid & "." & LCase(strFiletype(1))
    
    'FL = "c:\doc\doc1.pdf"
    '
    'openfile = FL & "/" & ARL
    
    'Shell openfile, vbNormalFocus
    'App_doc_path & "/branch.pdf"
    'App_doc_path & "/branch.pdf"


    Generate_File strPath, strFile, l_file
    
    ShellExecute Me.hwnd, vbNullString, "c:\doc\ftp1.bat", vbNullString, "c:", SW_SHOWNORMAL
    For i = 1 To 1000
        DoEvents
    Next i
    ShellExecute Me.hwnd, vbNullString, l_file, vbNullString, "c:", SW_SHOWNORMAL
    'ShellExecute Me.hwnd, vbNullString, "d:\doc\abc.jpg", vbNullString, "c:", SW_SHOWNORMAL
    
    Me.MousePointer = 0
    Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
    
End Sub



Private Sub TxtAmountA_Validate(Cancel As Boolean)
Dim a As String
Dim B As String
Dim count As Integer
If TxtAmountA.Text = "" Then
Exit Sub
End If
count = 0
For i = 1 To Len(TxtAmountA.Text)
      If Asc(Mid$(TxtAmountA.Text, i, 1)) < 48 Or Asc(Mid$(TxtAmountA.Text, i, 1)) > 57 Then
      If Asc(Mid$(TxtAmountA.Text, i, 1)) <> 46 Then
       MsgBox ("Enter Valid number")
       Exit Sub
       Else
       count = count + 1
       End If
       End If
Next i
If count > 1 Then
MsgBox ("Enter Valid number")
Exit Sub
End If

If count = 1 Then
a = Left(TxtAmountA.Text, (InStr(TxtAmountA.Text, ".")) - 1)
B = Right(TxtAmountA.Text, Len(TxtAmountA.Text) - ((InStr(TxtAmountA.Text, "."))))
If (InStr(TxtAmountA.Text, ".")) = 1 Then
a = 0
End If
If Len(TxtAmountA.Text) = (InStr(TxtAmountA.Text, ".")) Then
B = 0
End If

If MsgBox("Rupees " & rupe(a) & " and " & rupe(B) & " Paise Only", vbOKCancel) = vbCancel Then
TxtAmountA.SetFocus
TxtAmountA.Text = ""
End If
Else
If MsgBox("Rupees " & rupe(TxtAmountA.Text) & " Only", vbOKCancel) = vbCancel Then
TxtAmountA.SetFocus
TxtAmountA.Text = ""
End If

End If
End Sub

Public Function rupe(ByVal dblValue As Double) As String
Static ones(0 To 9) As String
Static teens(0 To 9) As String
Static tens(0 To 9) As String
Static thousands(0 To 4) As String
Dim i, X, Y As Integer, nPosition As Integer
Dim nDigit As Integer, bAllZeros As Integer
Dim strResult As String, strTemp As String, strTemp1 As String
Dim tmpBuff As String


ones(0) = "zero"
ones(1) = "one"
ones(2) = "two"
ones(3) = "three"
ones(4) = "four"
ones(5) = "five"
ones(6) = "six"
ones(7) = "seven"
ones(8) = "eight"
ones(9) = "nine"

teens(0) = "ten"
teens(1) = "eleven"
teens(2) = "twelve"
teens(3) = "thirteen"
teens(4) = "fourteen"
teens(5) = "fifteen"
teens(6) = "sixteen"
teens(7) = "seventeen"
teens(8) = "eighteen"
teens(9) = "nineteen"

tens(0) = ""
tens(1) = "ten"
tens(2) = "twenty"
tens(3) = "thirty"
tens(4) = "forty"
tens(5) = "fifty"
tens(6) = "sixty"
tens(7) = "seventy"
tens(8) = "eighty"
tens(9) = "ninty"

thousands(0) = ""
thousands(1) = "thousand"
thousands(2) = "million"
thousands(3) = "billion"
thousands(4) = "trillion"

'Trap errors
On Error GoTo rupeError

'Get fractional part
strResult = Format((dblValue - Int(dblValue)) * 100)
'Convert rest to string and process each digit
strTemp = CStr(Int(dblValue))

'Iterate through string
For i = Len(strTemp) To 1 Step -1
'Get value of this digit
nDigit = Val(Mid$(strTemp, i, 1))
'Get column position
nPosition = (Len(strTemp) - i) + 1
'Action depends on 1's, 10's or 100's column
Select Case (nPosition Mod 3)
Case 1 '1's position
bAllZeros = False
If i = 1 Then
tmpBuff = ones(nDigit) & " "
ElseIf Mid$(strTemp, i - 1, 1) = "1" Then
tmpBuff = teens(nDigit) & " "
i = i - 1 'Skip tens position
ElseIf nDigit > 0 Then
tmpBuff = ones(nDigit) & " "
Else
'If next 10s & 100s columns are also
'zero, then don't show 'thousands'
bAllZeros = True
If i > 1 Then
If Mid$(strTemp, i - 1, 1) <> "0" Then
bAllZeros = False
End If
End If
If i > 2 Then
If Mid$(strTemp, i - 2, 1) <> "0" Then
bAllZeros = False
End If
End If
tmpBuff = ""
End If
If bAllZeros = False And nPosition > 1 Then
tmpBuff = tmpBuff & thousands(nPosition / 3) & " "
End If
strResult = tmpBuff & strResult
Case 2 'Tens position
If nDigit > 0 Then
strResult = tens(nDigit) & " " & strResult
End If
Case 0 'Hundreds position
If nDigit > 0 Then
strResult = ones(nDigit) & " hundred " & strResult
End If
End Select
Next i
'Convert first letter to upper case
If Len(strResult) > 0 Then
strResult = UCase$(Left$(strResult, 1)) & Mid$(strResult, 2)
End If

Endrupe:

'Return result
Y = Len(strResult)
strTemp1 = Mid(strResult, 1, Y - 1)

rupe = strTemp1
Exit Function

rupeError:
strResult = "#Error#"
Resume Endrupe
End Function














Private Sub Txtinstallments1_Change()
Opt991.Value = False
End Sub

Private Sub Txtinstallments1_LostFocus()
CmbFrequency1_Click
Opt991.Value = False
'If Val(Txtinstallments1.Text) > 99 Then
'    MsgBox "Installments can't be greater then 99", vbCritical, "Wealthmaker"
'    Txtinstallments1.Text = ""
'    Txtinstallments1.SetFocus
'    Exit Sub
'End If

End Sub

Private Sub TxtPanM_Validate(Cancel As Boolean)
If ValidatePan(TxtPanM.Text) = False Then
    MsgBox "Please Either Enter a Valid PAN Number", vbCritical, "Pan Entry"
    TxtPanM.SetFocus
    Exit Sub
End If
End Sub



Private Sub TxtSchemeA_Change()
ChkClose_Click
End Sub

Private Sub TxtSchemeM_Change()
ChkCloseM_Click
End Sub

Sub VScroll1_Change()
   Picture1.Top = -VScroll1.Value
End Sub


Private Sub Form_KeyUp(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyF1 Then
        If SSTab1.Tab = 0 Then
            cmdSelectSch_Click
        Else
            cmdSelectSchm_Click
        End If
    End If
    If KeyCode = vbKeyF2 Then
        SSTab1.Tab = 1
    End If
    If KeyCode = vbKeyF3 Then
        If SSTab1.Tab = 0 Then
            CmdClientSearch_Click
        Else
            CmdClientSearchM_Click
        End If
    End If
'    If KeyCode = vbKeyF4 Then
'        pic_newinvestor_Click
'    End If
End Sub


Private Sub Form_Activate()
'On Error Resume Next
'SSTab1.Tab = 0
'Frame21.Left = 5000
'Frame21.Top = 1320
'TxtBusiCodeA.SetFocus
'LblTranCode.Caption = ""
End Sub

Private Sub DIS_EXPENSE()
    Label2(16).Visible = False
    Label2(17).Visible = False
    ImNExp_PER.Visible = False
    ImExpenses.Visible = False
    ImExpenses1.Visible = False
    ImnExp_Per1.Visible = False
    Label2(1).Visible = False
    Label2(2).Visible = False
End Sub

Private Sub Form_Load()
Dim sql As String
Label29.Caption = ""
Label9.Caption = ""
Label32.Caption = ""
LblTranCode.Caption = ""
Me.MousePointer = 11
MSRClient.DataSourceName = "TEST"
MSRClient.UserName = DataBaseUser
MSRClient.Password = DataBasePassword
FrmTransactionLog.Visible = False

Me.Top = 0
Me.Left = 0
Me.Height = main.Height - 900
'If GlbroleId = "21" Then DIS_EXPENSE

LblSipType.Visible = False
CmbSubSipA.Visible = False
        
ImSipStartDtA.Text = Format(Now, "DD/MM/YYYY")
mskfromdt.Text = Format(Now, "DD/MM/YYYY")
msktodt.Text = Format(Now, "DD/MM/YYYY")

CmbSubSipA.AddItem "NORMAL", 0
CmbSubSipA.AddItem "MICROSIP", 1

        
Cmbsubinv.AddItem "NORMAL", 0
Cmbsubinv.AddItem "MICRO", 1
Cmbsubinv.ListIndex = 0

Cmbsubinvu.AddItem "NORMAL", 0
Cmbsubinvu.AddItem "MICRO", 1
Cmbsubinvu.ListIndex = 0

Frame22.Visible = False
Frame17.Visible = False
Frame12.Visible = False
OptChkSwitch.Visible = False
OptChkSwitchM.Visible = False
 '-----------------Frequency-----------------------------
 fracancel.Visible = False
 CmbFrequency.Clear
 CmbFrequency1.Clear
 'ButtonPermission Me
 sql = "select itemname,itemserialnumber from fixeditem where itemtype=25"
 Set RsFreq = MyConn.Execute(sql)
 While Not RsFreq.EOF
     CmbFrequency.AddItem RsFreq(0) & Space(50) & "#" & RsFreq(1)
     CmbFrequency1.AddItem RsFreq(0) & Space(50) & "#" & RsFreq(1)
     RsFreq.MoveNext
 Wend
 For i = 0 To CmbFrequency.ListCount - 1
    MyFreq = Split(CmbFrequency.List(i), "#")
    If MyFreq(1) = 175 Then
        CmbFrequency.ListIndex = i
    End If
 Next
 RsFreq.Close
 Set RsFreq = Nothing
 '-------------------------------------------------------
 
 CmbAmcA.Clear
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbAmcA.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 CmbSwitchAmcA.Clear
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbSwitchAmcA.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 CmbSwitchAmcM.Clear
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbSwitchAmcM.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 SSTab1.Tab = 0
 sql = " Select mut_name from mut_fund  "
 sql = sql & "UNION ALL "
 sql = sql & "select B.ISS_NAME from iss_master b, other_product a "
 sql = sql & "where a.iss_code=b.iss_code and B.prod_code in ('DT020','DT027') "

 CmbAmcM.Clear
 Set rsAmc = MyConn.Execute(sql)
 While Not rsAmc.EOF
     CmbAmcM.AddItem rsAmc.Fields("Mut_Name")
     rsAmc.MoveNext
 Wend
 rsAmc.Close
 CmbAmcM.ListIndex = -1
 
 Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
 cmbBankName.Clear
 While Not rsAmc.EOF
   cmbBankName.AddItem rsAmc(0)
   rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 
 Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
 cmbBankNameM.Clear
 While Not rsAmc.EOF
   cmbBankNameM.AddItem rsAmc(0)
   rsAmc.MoveNext
 Wend
 rsAmc.Close
 
 CmbSipStpA.AddItem "REGULAR"
 CmbSipStpA.AddItem "SIP"
 CmbSipStpA.AddItem "STP"
 CmbSipStpA.ListIndex = 0
 
 CmbSipStpM.AddItem "REGULAR"
 CmbSipStpM.AddItem "SIP"
 CmbSipStpM.AddItem "STP"
 CmbSipStpM.ListIndex = 0
 
 If CmbSipStpA.Text = "SIP" Then
    OptsipF.Value = True
 Else
    OptsipF.Value = False
 End If
 ImEntryDtA.Text = Format(Now, "DD/MM/YYYY")
 ImEntryDtM.Text = Format(Now, "DD/MM/YYYY")
 

 cmbTranTypeA.AddItem "PURCHASE"
 cmbTranTypeA.AddItem "SWITCH IN"
 cmbTranTypeA.Text = "PURCHASE"
  
ImSipStartDtM.Text = Format(Now, "DD/MM/YYYY")
ImSipEndDtM.Text = "__/__/____"
ImSipStartDtM.Visible = False
ImSipEndDtM.Visible = False

Label2(8).Visible = False
Label2(10).Visible = False

Call cmbTranTypeA_Click
Call SetGrid
CmbOrder.Clear
CmbOrder.AddItem "Select", 0
CmbOrder.AddItem "Rm Name", 1
CmbOrder.AddItem "Branch Name", 2
CmbOrder.AddItem "Amc Name", 3
CmbOrder.AddItem "Scheme Name", 4
CmbOrder.AddItem "Pay Mode", 5
CmbOrder.AddItem "TranDate", 6
CmbOrder.ListIndex = 0
optcheque.Value = True
optchequeM.Value = True
Me.MousePointer = 0
searchschflag = 0
End Sub
Private Sub SetGrid()
 s = "Investor|^Branch|^PANNo|^AMC |^Scheme|^TranDate|^TranType|^AppNo|^Mode|^ChqNo|^ChqDt|^Amoount|^SIP/STP|^Lead No|^LeadName|^TranCode|^BranchCode|^BusiCode|^SchCode|^Rm"
 VSFCommGrdA.FormatString = s
 VSFCommGrdA.ColWidth(0) = 2200
 VSFCommGrdA.ColWidth(1) = 0
 VSFCommGrdA.ColWidth(2) = 1700
 VSFCommGrdA.ColWidth(3) = 1800
 VSFCommGrdA.ColWidth(4) = 1800
 VSFCommGrdA.ColWidth(5) = 1200
 VSFCommGrdA.ColWidth(6) = 1400
 
 VSFCommGrdA.ColWidth(7) = 1400
 VSFCommGrdA.ColWidth(8) = 800
 VSFCommGrdA.ColWidth(9) = 800
 VSFCommGrdA.ColWidth(10) = 1200
 VSFCommGrdA.ColWidth(11) = 1200

 VSFCommGrdA.ColWidth(12) = 1000
 VSFCommGrdA.ColWidth(13) = 1000
 VSFCommGrdA.ColWidth(14) = 1000
 VSFCommGrdA.ColWidth(15) = 0
 VSFCommGrdA.ColWidth(16) = 0
 VSFCommGrdA.ColWidth(17) = 0

 
 s1 = "Investor|^Branch|^PANNo|^AMC |^Scheme|^TranDate|^TranType|^AppNo|^Mode|^ChqNo|^ChqDt|^Amount|^SIP/STP|^Lead No|^LeadName|^TranCode|^BranchCode|^BusiCode|^SchCode|^ClientCode|^BankName|^Rm|^FolioNo|^docID|^Micro Investment|^Target Scheme Code|^Target Scheme Name|^Switch Scheme Name"
 VSFCommGrdM.FormatString = s1
 VSFCommGrdM.ColWidth(0) = 1500
 VSFCommGrdM.ColWidth(1) = 0
 VSFCommGrdM.ColWidth(2) = 1800
 VSFCommGrdM.ColWidth(3) = 1800
 VSFCommGrdM.ColWidth(4) = 1800
 VSFCommGrdM.ColWidth(5) = 1300
 VSFCommGrdM.ColWidth(6) = 1400
 
 VSFCommGrdM.ColWidth(7) = 1400
 VSFCommGrdM.ColWidth(8) = 800
 VSFCommGrdM.ColWidth(9) = 800
 VSFCommGrdM.ColWidth(10) = 1200
 VSFCommGrdM.ColWidth(11) = 1000

 VSFCommGrdM.ColWidth(12) = 1000
 VSFCommGrdM.ColWidth(13) = 1000
 VSFCommGrdM.ColWidth(14) = 1000
 VSFCommGrdM.ColWidth(15) = 0
 VSFCommGrdM.ColWidth(16) = 0
 VSFCommGrdM.ColWidth(17) = 0
 VSFCommGrdM.ColWidth(18) = 0
 VSFCommGrdM.ColWidth(19) = 0
 VSFCommGrdM.ColWidth(22) = 600
 VSFCommGrdM.ColWidth(23) = 600
 VSFCommGrdM.ColWidth(24) = 600
 VSFCommGrdM.ColWidth(25) = 600
 VSFCommGrdM.ColWidth(26) = 1600
 VSFCommGrdM.ColWidth(27) = 1600
End Sub

Private Sub ImEntryDtA_Validate(Cancel As Boolean)
If ImEntryDtA.Text <> "__/__/____" Then
'    If CDate(ImEntryDtA.Text) > Format(ServerDateTime, "dd-mm-yyyy") Then
'        MsgBox "TranDate Can Not Be Greater Date"
'        Cancel = True
'    End If
'    If CDate(ImEntryDtA.Text) < #8/1/2009# Then
'        MsgBox "Punching is not Allowed Before 31 July"
'        Cancel = True
'    End If
End If
End Sub


Private Sub ImEntryDtM_Validate(Cancel As Boolean)
If ImEntryDtM.Text <> "__/__/____" Then
    If CDate(ImEntryDtM.Text) > Format(ServerDateTime, "dd-mm-yyyy") Then
        MsgBox "TranDate Can Not Be Greater Date"
        Cancel = True
    End If
    If CDate(ImEntryDtM.Text) < #8/1/2009# Then
        MsgBox "Punching is not Allowed Before 31 July"
        Cancel = True
    End If
End If
End Sub
Private Sub ImExpenses_Change()
   If Val(TxtAmountA.Text) <> 0 Then ImNExp_PER.Text = Round(Val(ImExpenses.Text) / Val(TxtAmountA.Text) * 100, 4)
End Sub

Private Sub ImExpenses_GotFocus()
'If val(ImExpenses.Text) > MyRecdA Then
'    MsgBox "Payble CanNot Be Greater Than Receivable"
'    ImExpenses.Text = 0
'    ImnExp_Per.Text = 0
'End If
End Sub

Private Sub ImExpenses1_Change()
If TxtAmountM.Text <> "" Then
   If TxtAmountM.Text <> 0 Then ImnExp_Per1.Text = Round(Val(ImExpenses1.Text) / Val(TxtAmountM.Text) * 100, 4)
End If
End Sub

Private Sub ImExpenses1_GotFocus()
'If val(ImExpenses1.text) > MyRecdM Then
'    MsgBox "Payble CanNot Be Greater Than Receivable"
'    val(ImExpenses1.text) = 0
'    val(ImnExp_Per1.text) = 0
'End If
End Sub

Private Sub ImNExp_PER_Change()
'ImExpenses.Text = val(Round(val(TxtAmountA.Text) * val(ImnExp_Per.Text) * 0.01, 2))
End Sub

Private Sub ImnExp_Per_KeyPress(KeyAscii As Integer)
'    If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 And KeyAscii <> 46 Then
'        KeyAscii = 0
'    End If
End Sub
Private Sub ImnExp_Per_LostFocus()
ImExpenses.Text = Val(Round(Val(TxtAmountA.Text) * Val(ImNExp_PER.Text) * 0.01, 4))
End Sub

Private Sub ImNExp_PER1_Change()
'ImExpenses1.Text = val(Round(val(TxtAmountM.Text) * val(ImnExp_Per1.Text) * 0.01, 2))
End Sub



Private Sub ImnExp_Per1_LostFocus()
ImExpenses1.Text = Val(Round(Val(TxtAmountM.Text) * Val(ImnExp_Per1.Text) * 0.01, 4))
End Sub


Private Sub ImSipStartDtA_LostFocus()
CmbFrequency_Click
End Sub


Private Sub lstlongnameA_Click()
If OptChkSwitch.Value = 0 And frmSchemeSearch.searchschflag = 0 Then
    TxtSchemeA.Text = lstlongnameA.Text
Else
    TxtSwitchSchemeA.Text = lstlongnameA.Text
End If
End Sub
Private Sub lstlongnamem_Click()
TxtSchemeM.Text = lstlongnameM.Text
Dim str1() As String
Dim rsnat As ADODB.Recordset

    If TxtSchemeM.Text <> "" Then
        str1 = Split(TxtSchemeM.Text, "=")
        MySchCode = str1(1)
    End If


    Set rsnat = New ADODB.Recordset
    rsnat.open "select nvl(get_scheme_nature('" & MySchCode & "'),'') a from dual", MyConn, adOpenForwardOnly
    If rsnat("a") = "O" Then
        frmtransactionmf.Label2(2).Caption = "Expense Trail%"
        frmtransactionmf.Label2(1).Caption = "Expense Trail(Rs.)"
        MyNat = "O"
    Else
        frmtransactionmf.Label2(2).Caption = "Expense%"
        frmtransactionmf.Label2(1).Caption = "Expense(Rs.)"
        MyNat = "C"
    End If
    rsnat.Close
    Set rsnat = Nothing
    
    
End Sub

Private Sub Opt99_Click()
    TxtInstallmens.Text = "99"
    CmbFrequency_Click
End Sub

Private Sub optcash_Click()
cmbBankName.Clear
cmbBankName.AddItem "Cash"
Label6(2).Caption = "Application No"
cmbBankName.Enabled = False
txtChqNo.Enabled = False
dtChqDate.Enabled = False
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = False
Label31(5).Visible = False
Label31(6).Visible = False
If cmbTranTypeA.Text = "SWITCH IN" Then
   OptOthers.Value = True
End If
End Sub

Private Sub optcashM_Click()
cmbBankNameM.Clear
cmbBankNameM.AddItem "Cash"
Label33.Caption = "Application No"
cmbBankNameM.Enabled = False
txtChqNoM.Enabled = False
dtChqDateM.Enabled = False
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(55).Visible = False
Label31(66).Visible = False
End Sub

Private Sub optcheque_Click()
Label11.Caption = "Cheque No."
Label12.Caption = "Cheque Dated."
Label6(2).Caption = "Application No"
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
cmbBankName.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = True
Label31(5).Visible = True
Label31(6).Visible = True
If cmbTranTypeA.Text = "SWITCH IN" Then
   OptOthers.Value = True
End If
End Sub

Private Sub optchequeM_Click()
Label111.Caption = "Cheque No."
Label122.Caption = "Cheque Dated."
Label33.Caption = "Application No"
cmbBankNameM.Enabled = True
txtChqNoM.Enabled = True
dtChqDateM.Enabled = True
cmbBankNameM.Text = ""
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(4).Visible = True
Label31(55).Visible = True
Label31(66).Visible = True
End Sub

Private Sub OptChkSwitch_Click()
lstlongnameA.Clear
End Sub

Private Sub OptChkSwitchM_Click()
lstlongnameM.Clear
End Sub


Private Sub optdraft_Click()
Label11.Caption = "Draft No."
Label12.Caption = "Draft Dated."
Label6(2).Caption = "Application No"
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
cmbBankName.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = True
Label31(5).Visible = True
Label31(6).Visible = True
If cmbTranTypeA.Text = "SWITCH IN" Then
   OptOthers.Value = True
End If
End Sub

Private Sub optdraftM_Click()
Label111.Caption = "Draft No."
Label122.Caption = "Draft Dated."
Label33.Caption = "Application No"
cmbBankNameM.Enabled = True
txtChqNoM.Enabled = True
dtChqDateM.Enabled = True
cmbBankNameM.Text = ""
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(55).Visible = True
Label31(66).Visible = True
End Sub

Private Sub OptECS_Click()
Label11.Caption = "MICR No."
Label12.Caption = "Dated."
Label6(2).Caption = "Application No"
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
cmbBankName.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = True
Label31(5).Visible = True
Label31(6).Visible = True
If cmbTranTypeA.Text = "SWITCH IN" Then
   OptOthers.Value = True
End If
End Sub

Private Sub OptEcsM_Click()
Label111.Caption = "MICR No."
Label122.Caption = "Dated."
Label33.Caption = "Application No"
cmbBankNameM.Enabled = True
txtChqNoM.Enabled = True
dtChqDateM.Enabled = True
cmbBankNameM.Text = ""
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(55).Visible = True
Label31(66).Visible = True
End Sub

Private Sub OptFT_Click()
Label11.Caption = "Bank A/C No."
Label12.Caption = "Fund Transfer Date"
Label6(2).Caption = "Application No"
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
cmbBankName.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = True
Label31(5).Visible = True
Label31(6).Visible = True
If cmbTranTypeA.Text = "SWITCH IN" Then
   OptOthers.Value = True
End If

End Sub

Private Sub OptFTM_Click()
Label111.Caption = "Bank A/C No."
Label122.Caption = "Fund Transfer Date"
Label33.Caption = "Application No"
cmbBankNameM.Enabled = True
txtChqNoM.Enabled = True
dtChqDateM.Enabled = True
cmbBankNameM.Text = ""
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(4).Visible = True
Label31(55).Visible = True
Label31(66).Visible = True
End Sub

Private Sub OptOthers_Click()
'Label11.Caption = "FDR No."
'Label12.Caption = "Renewal Date."
Label6(2).Caption = "Folio No"
cmbBankName.Enabled = False
txtChqNo.Enabled = False
dtChqDate.Enabled = False
cmbBankName.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = False
Label31(5).Visible = True
Label31(6).Visible = True
End Sub

Private Sub OptOthersm_Click()
Label33.Caption = "Folio No"
Label33.Caption = "Application No"
cmbBankNameM.Enabled = False
txtChqNoM.Enabled = False
dtChqDateM.Enabled = False
cmbBankNameM.Text = ""
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(55).Visible = False
Label31(66).Visible = False
End Sub


Private Sub OptRTGS_Click()
Label11.Caption = "UTR No."
Label12.Caption = "UTR Date"
Label6(2).Caption = "Application No"
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
cmbBankName.Text = ""
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
Label31(4).Visible = True
Label31(5).Visible = True
Label31(6).Visible = True
If cmbTranTypeA.Text = "SWITCH IN" Then
   OptOthers.Value = True
End If
End Sub

Private Sub OptRTGSM_Click()
Label111.Caption = "UTR No."
Label122.Caption = "UTR Date"
Label33.Caption = "Application No"
cmbBankNameM.Enabled = True
txtChqNoM.Enabled = True
dtChqDateM.Enabled = True
cmbBankNameM.Text = ""
txtChqNoM.Text = ""
dtChqDateM.Text = "__/__/____"
Label31(4).Visible = True
Label31(55).Visible = True
Label31(66).Visible = True

End Sub

Private Sub SSTab1_Click(PreviousTab As Integer)
'If PreviousTab = 1 Then
'   TxtBusiCodeA.SetFocus
'ElseIf PreviousTab = 2 Then
'    TxtBusiCodeA.SetFocus
'End If

If SSTab1.Tab = 0 Then
    Frame21.Left = 5000
    Frame21.Top = 1320
Else
    Frame21.Left = 9240
    Frame21.Top = 3120
End If

'If PreviousTab = 0 Then
'    ImEntryDtA.Text = "__/__/____"
'    ImEntryDtM.Text=
'Else
'
'End If
End Sub

Private Sub TxtAcHolderA_KeyDown(KeyCode As Integer, Shift As Integer)
If KeyCode = 46 Or KeyCode = 8 Then
   KeyCode = 0
End If
End Sub

Private Sub TxtAcHolderA_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub


Private Sub TxtAcHolderM_KeyDown(KeyCode As Integer, Shift As Integer)
If KeyCode = 46 Or KeyCode = 8 Then
   KeyCode = 0
End If
End Sub

Private Sub TxtAcHolderM_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub


Private Sub TxtAmountA_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 46 Or KeyAscii = 8 Then
Else
    KeyAscii = 0
End If
If KeyAscii = 13 Then
   SendKeys "{TAB}"
End If
End Sub


Private Sub TxtAmountM_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 46 Or KeyAscii = 8 Then
Else
   KeyAscii = 0
End If
If KeyAscii = 13 Then
  SendKeys "{TAB}"
End If
End Sub

Private Sub TxtAmountM_LostFocus()
'MyRecdM = 0
'Dim str() As String
'If Mid(TxtClientCodeM, 1, 1) = "3" Then
'    If TxtSchemeM.Text <> "" Then
'        str = Split(TxtSchemeM.Text, "=")
'        MySchCode = str(1)
'    End If
'    If TxtBusiCodeM.Text <> "" And CmbAmcM.Text <> "" And TxtSchemeM.Text <> "" And ImEntryDtM.Value <> "__/__/____" Then
'        ImExpenses1.Text = SqlRet("select nvl(Upfront_Ope_Paid_Temptran2_MSH('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & val(TxtAmountA.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'        MyRecdM = SqlRet("select nvl(Upfront_Ope_Paid_Temptran2_MSH('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & val(TxtAmountA.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'    End If
'Else
'    If TxtSchemeM.Text <> "" Then
'        str = Split(TxtSchemeM.Text, "#")
'        MySchCode = str(1)
'    End If
'    If TxtBusiCodeM.Text <> "" And CmbAmcM.Text <> "" And TxtSchemeM.Text <> "" And ImEntryDtM.Value <> "__/__/____" Then
'        ImExpenses1.Text = SqlRet("select nvl(Upfront_Ope_Recd_Temptran2_MSH('" & TxtClientCodeM.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtM) & "','MM/DD/YYYY')," & val(TxtAmountM.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'        MyRecdM = SqlRet("select nvl(Upfront_Ope_recd_Temptran2_MSH('" & TxtClientCodeA.Text & "' ,'" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & val(TxtAmountA.Text) & " , 1 ," & MyBranchCode & "),0) from dual ")
'    End If
'End If
End Sub





Private Sub TxtAppnoA_KeyPress(KeyAscii As Integer)
  If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
        KeyAscii = 0
    End If
End Sub


Private Sub TxtAppnoM_KeyPress(KeyAscii As Integer)
  If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
        KeyAscii = 0
  End If
End Sub


Private Sub TxtBranchA_KeyDown(KeyCode As Integer, Shift As Integer)
KeyCode = 0
End Sub

Private Sub TxtBranchA_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub
Private Sub TxtBranchM_KeyDown(KeyCode As Integer, Shift As Integer)
KeyCode = 0
End Sub

Private Sub TxtBranchM_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub

Private Sub TxtBusiCodeA_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
   KeyAscii = 0
End If
If KeyAscii = 13 Then
  SendKeys "{TAB}"
End If
End Sub

Public Sub TxtBusiCodeA_KeyUp(KeyCode As Integer, Shift As Integer)
'Dim rsEmp As ADODB.Recordset
'Dim b_cd() As String
'    If Len(TxtBusiCodeA.Text) <> 0 Then
'        TxtBranchA.Text = ""
'        Set rsEmp = New ADODB.Recordset
'        If AllIndiaSearchFlag <> "ALL" Then
'            rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where b.branch_code in (" & Branches & ") and e.payroll_id='" & Trim(TxtBusiCodeA.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018)", MyConn, adOpenForwardOnly
'        Else
'            rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeA.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018)", MyConn, adOpenForwardOnly
'        End If
'        If rsEmp.EOF = False Then
'            TxtBranchA.Text = rsEmp(1) & Space(100) & "#" & rsEmp(0)
'            TxtRMName.Text = rsEmp(2)
'            MyRmCodeA = rsEmp(3)
'            MyCurrentBranchCode = rsEmp(0)
'            Label9.Caption = rsEmp(1)
'            Label29.Caption = rsEmp(2)
'        Else
'            MsgBox "Employee Should Be Of Sale Support or marketing"
'            TxtBranchA.Text = ""
'            TxtBusiCodeA.Text = ""
'            MyRmCodeA = 0
'            TxtRMName.Text = ""
'            Label9.Caption = ""
'            Label29.Caption = ""
'            Label32.Caption = ""
'            txtInvestorA.Text = ""
'        End If
'        rsEmp.Close
'        Set rsEmp = Nothing
'    End If
End Sub

Public Sub TxtBusiCodeA_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim Rs_GetBranchCd As ADODB.Recordset
Dim b_cd() As String
Dim i As Integer
    If Len(TxtBusiCodeA.Text) <> 0 Then
        TxtBranchA.Text = ""
        
        If Len(Trim(Label32.Caption)) > 0 Then
            Set Rs_GetBranchCd = New ADODB.Recordset
            Rs_GetBranchCd.open "select i.branch_code from investor_master i where I.inv_code='" & Trim(Label32.Caption) & "' ", MyConn, adOpenDynamic, adLockOptimistic
            If Rs_GetBranchCd.EOF = False Then
                branch_code = Rs_GetBranchCd!branch_code
            End If
            Rs_GetBranchCd.Close
        End If
        Set Rs_GetBranchCd = Nothing
        
        If AllIndia = False Then
            If branch_code = "" Then
                TxtBusiCodeA.Text = ""
                MsgBox "Please Select Investor First", vbInformation
                Exit Sub
            End If
        End If
        
        
        
        Set rsEmp = New ADODB.Recordset
        If AllIndiaSearchFlag <> "ALL" Then
            rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where b.branch_code in (" & Branches & ") and e.payroll_id='" & Trim(TxtBusiCodeA.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018)", MyConn, adOpenForwardOnly
        Else
            rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeA.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018)", MyConn, adOpenForwardOnly
        End If
        If rsEmp.EOF = False Then
            cmbBusiBranch.Clear
            cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
            cmbBusiBranch.ListIndex = 0
            TxtRmName.Text = rsEmp(2)
            'MyRmCodeA = rsEmp(3)
            MyCurrentBranchCode = rsEmp(0)
            'Label9.Caption = rsEmp(1)
            Label29.Caption = rsEmp(2)
            
            If rsEmp.State = 1 Then rsEmp.Close
            'Call Show_Tr_Branches(Trim(TxtBusiCodeA.Text))
            ''''himanshu
            If valid_rm(branch_code, TxtBusiCodeA) = False Then Exit Sub
            i = 0
            If Tr_Branches <> "" Then
'                cmbBusiBranch.Clear
'                Set rsEmp = New ADODB.Recordset
'                rsEmp.open "Select branch_code,branch_name from branch_master b where branch_code in (" & Tr_Branches & ") order by branch_name", MyConn, adOpenForwardOnly
'                While Not rsEmp.EOF
'                    cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
'                    If rsEmp(0) = branch_code Then
'                        cmbBusiBranch.ListIndex = i
'                    End If
'                    i = i + 1
'                    rsEmp.MoveNext
'                Wend

                'cmbBusiBranch.ListIndex = 0

            End If
            
            If rsEmp.State = 1 Then rsEmp.Close
            Set rsEmp = Nothing
        Else
            MsgBox "Employee Should Be Of Sale Support or marketing", vbInformation
            TxtBranchA.Text = ""
            cmbBusiBranch.Clear
            TxtBusiCodeA.Text = ""
            MyRmCodeA = 0
            TxtRmName.Text = ""
            Label9.Caption = ""
            Label29.Caption = ""
            Label32.Caption = ""
            txtInvestorA.Text = ""
        End If
        Set rsEmp = Nothing
        
        
    End If
End Sub

Private Sub TxtBusiCodeM_Change()
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
    KeyAscii = 0
End If
If KeyAscii = 13 Then
   SendKeys "{TAB}"
 End If
End Sub

Private Sub TxtBusiCodeM_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
    KeyAscii = 0
End If
If KeyAscii = 13 Then
   SendKeys "{TAB}"
End If
End Sub

Public Sub TxtBusiCodeM_KeyUp(KeyCode As Integer, Shift As Integer)
busi_change_flag = True
'Dim rsEmp As ADODB.Recordset
'Dim b_cd() As String
'    If Len(TxtBusiCodeM.Text) >= 5 Then
'        TxtBranchM.Text = ""
'        Set rsEmp = New ADODB.Recordset
'        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeM.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018) ", MyConn, adOpenForwardOnly
'        If rsEmp.EOF = False Then
'            TxtBranchM.Text = rsEmp(1) & Space(100) & "#" & rsEmp(0)
'            TxtRmNameM.Text = rsEmp(2)
'            MyRmCodeM = rsEmp(3)
'            Label14.Caption = rsEmp(1)
'            Label30.Caption = rsEmp(2)
'        Else
'            TxtBranchM.Text = ""
'            MyRmCodeM = 0
'            TxtBusiCodeM.Text = ""
'            TxtRmNameM.Text = ""
'            Label14.Caption = ""
'            Label30.Caption = ""
'        End If
'        rsEmp.Close
'        Set rsEmp = Nothing
'    End If
End Sub



Public Sub TxtBusiCodeM_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim Rs_GetBranchCd As ADODB.Recordset
Dim b_cd() As String
    If Len(TxtBusiCodeM.Text) <> 0 Then
        TxtBranchM.Text = ""
        
        If Len(Trim(Label42.Caption)) > 0 Then
            Set Rs_GetBranchCd = New ADODB.Recordset
            Rs_GetBranchCd.open "select i.branch_code from investor_master i where I.inv_code='" & Trim(Label42.Caption) & "'", MyConn, adOpenDynamic, adLockOptimistic
            If Rs_GetBranchCd.EOF = False Then
                branch_code = Rs_GetBranchCd!branch_code
            End If
            Rs_GetBranchCd.Close
        End If
        Set Rs_GetBranchCd = Nothing
        
        
        If AllIndia = False Then
            If branch_code = "" Then
                TxtBusiCodeM.Text = ""
                MsgBox "Please Select Investor First", vbInformation
                Exit Sub
            End If
        End If
        

        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeM.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018) ", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            cmbBusiBranchM.Clear
            cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
            cmbBusiBranchM.ListIndex = 0
            TxtRmNameM.Text = rsEmp(2)
            'MyRmCodeM = rsEmp(3)
            'Label14.Caption = rsEmp(1)
            Label30.Caption = rsEmp(2)
            Call Show_Tr_Branches(Trim(TxtBusiCodeM.Text))
            If valid_rm(branch_code, TxtBusiCodeM) = False Then Exit Sub
            If rsEmp.State = 1 Then rsEmp.Close
            
            
            If Tr_Branches <> "" Then
                cmbBusiBranchM.Clear
                Set rsEmp = New ADODB.Recordset
                rsEmp.open "Select branch_code,branch_name from branch_master b where branch_code in (" & Tr_Branches & ") order by branch_name", MyConn, adOpenForwardOnly
                While Not rsEmp.EOF
                    cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                    If rsEmp(0) = busi_br_tr Then
                        cmbBusiBranchM.ListIndex = i
                    End If
                    rsEmp.MoveNext
                Wend
                'cmbBusiBranchM.ListIndex = 0
            End If
            
            If busi_change_flag = True Then
                set_default_branch branch_code
            End If
            
            If rsEmp.State = 1 Then rsEmp.Close
            Set rsEmp = Nothing
            
        Else
            TxtBranchM.Text = ""
            MyRmCodeM = 0
            TxtBusiCodeM.Text = ""
            TxtRmNameM.Text = ""
            Label14.Caption = ""
            Label30.Caption = ""
        End If
        
        Set rsEmp = Nothing
    End If
End Sub

Private Sub set_default_branch(br_cd As String)
Dim i As Integer
Dim branch_cd() As String
    For i = 0 To cmbBusiBranchM.ListCount - 1
        branch_cd = Split(cmbBusiBranchM.List(i), "#")
        If Trim(branch_cd(1)) = br_cd Then
            cmbBusiBranchM.ListIndex = i
            Exit For
        End If
    Next i
End Sub

'Private Sub txtChqNo_KeyPress(KeyAscii As Integer)
'    If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
'        KeyAscii = 0
'    End If
'End Sub


Private Sub TxtClientCodeA_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub
Private Sub TxtClientCodeM_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub


Private Sub TxtInstallmens_KeyPress(KeyAscii As Integer)
    If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
        KeyAscii = 0
    End If
    Opt99.Value = False
'    If Val(TxtInstallmens.Text) > 99 Then
'    MsgBox "Installments can't be greater then 99", vbCritical, "Wealthmaker"
'    TxtInstallmens.Text = ""
'    TxtInstallmens.SetFocus
'    Exit Sub
'End If
End Sub


Private Sub TxtInstallmens_LostFocus()
CmbFrequency_Click
End Sub


Private Sub txtInvestorA_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub
Private Sub txtInvestorM_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub
Private Sub TxtPanA_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub TxtPanM_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtpans_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub TxtPanVarify_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub TxtRmName_KeyDown(KeyCode As Integer, Shift As Integer)
KeyCode = 0
End Sub

Private Sub TxtRmName_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub

Private Sub TxtRmNameM_KeyDown(KeyCode As Integer, Shift As Integer)
KeyCode = 0
End Sub

Private Sub TxtRmNameM_KeyPress(KeyAscii As Integer)
KeyAscii = 0
End Sub





Private Sub txtTrNo_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub




Private Sub VSFCommGrdM_Click()
On Error Resume Next
If VSFCommGrdM.Rows > 1 And VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 1) <> "" Then
    If ChkCloseM.Value = 1 Then
        ChkCloseM.Value = 0
        TxtCloseSchM.Text = ""
    End If
    
    MyTranCode = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 15)
    
    If GlbroleId = 212 Then
        CmbAmcM.Enabled = False
        TxtSchemeM.Enabled = False
        ImEntryDtM.Enabled = False
    Else
        CmbAmcM.Enabled = True
        TxtSchemeM.Enabled = True
        ImEntryDtM.Enabled = True
    End If
        
    txtTrNo.Text = MyTranCode
    txtdocID.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 23)
    TxtBranchM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 1) & Space(100) & "#" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 16)
    Label42.Caption = SqlRet("select nvl(client_code,0) from transaction_mf_temp1 where tran_code='" & MyTranCode & "'")
    cmbBusiBranchM.Clear
    TxtBusiCodeM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 17)
    TxtBusiCodeM_LostFocus
    busi_br_tr = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 16)
    
    cmbBusiBranchM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 1) & Space(100) & "#" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 16)
    busi_change_flag = False
    
    
    TxtRmNameM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 21)
    txtInvestorM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 0)
    TxtPanM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 2)
    CmbAmcM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 3)
    For i = 0 To lstlongnameM.ListCount - 1
        If UCase(Trim(Mid(lstlongnameM.List(i), 1, 99))) = UCase(Trim(VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 4))) Then
            lstlongnameM.Selected(i) = True
            TxtSchemeM.Text = lstlongnameA.Text
            Exit For
        End If
    Next
    TxtSchemeM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 4) & Space(100) & "=" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 18)
    
    TxtCloseSchM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 26) & Space(100) & "=" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 25)
    If TxtCloseSchM.Text <> "" Then
        ChkCloseM.Value = 1
    End If
    
    ImEntryDtM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 5)
    cmbTranTypeM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 6)
    TxtAppnoM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 7)
    TxtFolioNoM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 22)
    If VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "C" Then
       optchequeM.Value = True
       Call optchequeM_Click
    ElseIf VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "D" Then
       optdraftM.Value = True
       Call optdraftM_Click
    ElseIf VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "H" Then
       optcashM.Value = True
       optcashM_Click
    ElseIf VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "E" Then
       OptEcsM.Value = True
       OptEcsM_Click
    ElseIf VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "R" Then
       OptOthersm.Value = True
       OptOthersm_Click
    ElseIf VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "U" Then
       OptRTGSM.Value = True
       OptRTGSM_Click
    ElseIf VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 8) = "B" Then
       OptFTM.Value = True
       OptFTM_Click
    End If
    txtChqNoM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 9)
    On Error Resume Next
    dtChqDateM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 10)
    TxtAmountM = Val(VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 11))
    CmbSipStpM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 12)
    
    '--------------------Frequency & Installments(Maya)------------------------------
        If CmbSipStpM.Text = "SIP" Or CmbSipStpM.Text = "STP" Then
            Set RsFreq = New ADODB.Recordset
            sql = "select frequency,installments_no,sip_fr from transaction_mf_temp1 where tran_code='" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 15) & "'"
            RsFreq.open sql, MyConn, adOpenForwardOnly
            If Not RsFreq.EOF Then
                Txtinstallments1.Text = RsFreq.Fields("installments_no")
                If Txtinstallments1.Text = "99" Then
                    Opt991.Value = True
                End If
                If RsFreq.Fields("sip_fr") = "F" Then
                    OptSIPFM.Value = True
                ElseIf RsFreq.Fields("sip_fr") = "R" Then
                    OptSIPRM.Value = True
                Else
                    OptSIPFM.Value = False
                    OptSIPRM.Value = False
                End If
                For i = 0 To CmbFrequency1.ListCount - 1
                     MyFreq = Split(CmbFrequency1.List(i), "#")
                     If Trim(MyFreq(1)) = Trim(RsFreq.Fields("frequency")) Then
                        CmbFrequency1.ListIndex = i
                     End If
                Next
            End If
            RsFreq.Close
            Set RsFreq = Nothing
        End If
    '---------------------------------------------------------------------------
    '--------------------Reconcilation Status/ Remarks Dispaly------------------------------
        Set RsFreq = New ADODB.Recordset
        sql = "select rec_flag,remark_reco,dispatch,ATM_FLAG,cob_flag,FREEDOM_SIP_FLAG,SWP_FLAG,SWITCH_SCHEME,SWITCH_FOLIO from transaction_mf_temp1 where tran_code='" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 15) & "'"
        RsFreq.open sql, MyConn, adOpenForwardOnly
        If Not RsFreq.EOF Then
            If RsFreq.Fields("ATM_FLAG") = 1 Then
               ChkAtmTransactionM.Value = 1
            Else
                ChkAtmTransactionM.Value = 0
            End If
            
            If RsFreq.Fields("cob_flag") = 1 Then
               chkcobM.Value = 1
            Else
                chkcobM.Value = 0
            End If
            
            'BY PANKAJ PUNDIR ON DATED 02052016
            If RsFreq.Fields("SWP_FLAG") = 1 Then
               chkSWPM.Value = 1
            Else
                chkSWPM.Value = 0
            End If
            
            If RsFreq.Fields("FREEDOM_SIP_FLAG") = 1 Then
                ChkFreedomM.Value = 1
            Else
                ChkFreedomM.Value = 0
            End If
            
            If Not IsNull(RsFreq.Fields("dispatch")) Then
                If RsFreq.Fields("dispatch") = "R" Then
                    optRegM.Value = True
                Else
                    optNFOM.Value = True
                End If
            Else
                optRegM.Value = True
            End If
            LblRemarks.Text = IIf(IsNull(RsFreq.Fields("remark_reco")), "", RsFreq.Fields("remark_reco"))
            If RsFreq.Fields("rec_flag") = "Y" Then
                LblRecoStatus.Caption = "Confirmed"
                LblRecoStatus.AutoSize = True
            Else
                LblRecoStatus.Caption = "Logged-In"
                LblRecoStatus.AutoSize = True
            End If
            'un comment by pankaj on dated 12032016
            If cmbTranTypeM.Text = "SWITCH IN" Then
                TxtSwitchFolioM.Text = RsFreq("SWITCH_FOLIO")
                'rsscheme.Fields ("sch_name") & Space(100) & "=" & rsscheme.Fields("sch_code")
                TxtSwitchSchemeM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 27) & Space(100) & "=" & RsFreq("SWITCH_SCHEME")
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End If
        RsFreq.Close
        Set RsFreq = Nothing
       
    '---------------------------------------------------------------------------
    Lbl_Leadno = "Lead No:" & VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 13)
    lbl_lead_caption = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 14)
    
    BaseTranCode = SqlRet("select tran_code from transaction_mf_temp1 where base_tran_code='" & MyTranCode & "'")
    '--------------------AR Serch for log-------------------------------
    LblTranCode.Caption = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 15)
    '--------------------------------------------------------------------
    ''cmbTranTypeA.Text = SqlRet("SELECT TRAN_TYPE FROM TRANSACTION_MF_TEMP1 WHERE TRAN_CODE='" & MyTranCode & "'")
    MySchCode = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 18)
    If get_ATM_scheme(MySchCode) = True Then
        ChkAtmTransactionM.Enabled = True
    Else
        ChkAtmTransactionM.Enabled = False
    End If
    MyBranchCode = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 16)
    Label14.Caption = SqlRet("select branch_name from branch_master where branch_code=" & MyBranchCode & "")
    'Call TxtBusiCodeM_LostFocus
    Label30.Caption = SqlRet("select rm_name from employee_master where payroll_id='" & TxtBusiCodeM & "'")
    lbtrancode1 = MyTranCode
    MyPrintTranCode = MyTranCode
    

    TxtAcHolderM.Text = SqlRet("select nvl(AC_HOLDER_CODE,0) from transaction_mf_temp1 where tran_code='" & MyTranCode & "'")
    
    MyRmCodeM = SqlRet("select nvl(RM_CODE,0) from TRANSACTION_MF_TEMP1 where tran_code='" & MyTranCode & "'")
    
    ImnExp_Per1.Text = SqlRet("select nvl(exp_rate,0) from transaction_mf_temp1 where tran_code='" & MyTranCode & "'")
    ImExpenses1.Text = SqlRet("select nvl(exp_amount,0) from transaction_mf_temp1 where tran_code='" & MyTranCode & "'")
    TxtClientCodeM = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 19)
    cmbBankNameM.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 20)
    frmtransactionmf.Lbl_Leadno.Caption = ""
End If
ColumnIndex = VSFCommGrdM.Col
Clear_Previously_Selected
Glb_L_SearchIndex = 1
Glb_Selected_row = 0
Label6(2).Caption = ""
Label6(2).Caption = "Searched By--->" & VSFCommGrdM.TextMatrix(0, VSFCommGrdM.Col)
Label6(0).Caption = VSFCommGrdM.TextMatrix(0, VSFCommGrdM.Col)

If Label42.Caption = "" Then
    MsgBox "Information not present", vbInformation, strBajaj
    Exit Sub
Else
    Set frminvaddress_update.currentForm = Nothing
    Set frminvaddress_update.currentForm = frmtransactionmf
    frminvaddress_update.Label32.Caption = Label42.Caption
    frminvaddress_update.Show
    frminvaddress_update.ZOrder 0
End If

If VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 24) <> "" Then
    Cmbsubinvu.Text = VSFCommGrdM.TextMatrix(VSFCommGrdM.Row, 24)
    Frame23.Visible = True
Else
    Cmbsubinvu.ListIndex = 0
End If

End Sub
Private Sub CmbOrder_Click()
MyOrder = ""
If CmbOrder.ListIndex = 1 Then
   MyOrder = "Rm_Name"
ElseIf CmbOrder.ListIndex = 2 Then
    MyOrder = "Branch_Name"
ElseIf CmbOrder.ListIndex = 3 Then
    MyOrder = "Amc_Name"
ElseIf CmbOrder.ListIndex = 4 Then
    MyOrder = "Scheme_Name"
ElseIf CmbOrder.ListIndex = 5 Then
    MyOrder = "PAYMENT_MODE"
ElseIf CmbOrder.ListIndex = 6 Then
    MyOrder = "TR_DATE"
End If
End Sub

Public Function chkSaveValidation(FrSave As Boolean, FrMody As Boolean, tranCode As String) As Boolean
Dim rs_get_chk_folio As ADODB.Recordset, rs_get_first_date As ADODB.Recordset, rs_get_facevalue As ADODB.Recordset
Dim tempFol As String
Dim pre As String
Dim MyOriginalDate As Date
If tranCode <> "" Then
    MyOriginalDate = SqlRet("select tr_date from transaction_mf_temp1 where tran_code='" & tranCode & "'")
End If
If FrSave = True And FrMody = False Then
    If (CDate(Format(ImEntryDtA.Text, "dd-mm-yyyy")) < CDate(Format(Glbins_previousdate, "DD-MM-YYYY"))) Or (CDate(Format(ImEntryDtA.Text, "dd-mm-yyyy")) > CDate(Format(Glbins_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
End If
If FrSave = False And FrMody = True Then
    If (CDate(Format(MyOriginalDate, "dd-mm-yyyy")) < CDate(Format(Glbup_previousdate, "DD-MM-YYYY"))) Or (CDate(Format(MyOriginalDate, "dd-mm-yyyy")) > CDate(Format(Glbup_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
    
    If MyConn.Execute("select count(*) from WEALTHMAKER.ANATRANDETAILTABLE_NEW_ALL_VB  where tran_code='" & tranCode & "'")(0) > 0 Then
        MsgBox "ANG Bills generated for this transaction. You can not modify the payout of this AR. ", vbInformation
        chkSaveValidation = False
        Exit Function
    End If
End If
chkSaveValidation = True
End Function
