Dim RSV As New ADODB.Recordset
Dim RSV1 As New ADODB.Recordset
Dim RsOcc As New ADODB.Recordset
Dim MyOldBusiCode As String
Dim RsKyc As New ADODB.Recordset
Dim MyPrintSourceId As String
Dim MainAccountCode As String
Dim MyTrDate As Date
Dim RsAdvisory As New ADODB.Recordset
Dim MyPlan() As String
Dim RsSearch As New ADODB.Recordset
Dim flag As Boolean
Dim Temp_Id1() As String
Dim DupInvestor As String
Dim RsGridFill As New ADODB.Recordset
Dim CtC As String
Dim cmbYesNo As String
Dim invCode() As String
Dim StrOcc As String
Dim rs_inv As New ADODB.Recordset
Dim StrInv As String
Dim CCKyc As String
Dim RsCommM As New ADODB.Recordset
Dim RsCommI As New ADODB.Recordset
Dim DoGrid As Integer
Dim lid As Long
Dim X As Boolean
Dim MyUpdateClient As String
Dim StrRel As String
Dim occ As String
Dim Ky As String
Dim MyExistRelFlag As Integer
Dim StrEnclosedPanA As String
Dim StrEnclosed60A As String
Dim StrEnclosed61A As String
Dim StrEnclosedPanG As String
Dim StrEnclosed60G As String
Dim StrEnclosed61G As String
Dim ActSt As String
Dim News As String
Dim ARept As String
Dim sInfo As String
Dim NIPan As String
Dim NIAdd As String
Dim NIPower As String
Dim IPan As String
Dim IAdd As String
Dim CommMode As Integer
Dim CommInterval As Integer
Dim ctl As Control
Dim Rs_Rel As New ADODB.Recordset
Dim rs_Occ As New ADODB.Recordset
Dim rs_tit As New ADODB.Recordset
Dim clcode As New ADODB.Recordset
Dim MyBranch As String
Dim MyRmCode As String
Dim MyUpdateInvestor As String
Dim MyCityCode As String
Dim City() As String
Dim ClienCode() As String
Dim INVESTORCODE() As String
Dim Plan() As String
Dim MySchemeCode As String
Dim MyMutCode As String
Dim MySchemeName As String
Dim MyStateCode As String
Dim MyState() As String
Dim MyOccCode As String
Dim MyOcc() As String
Dim MyClientCode As String
Dim MySourceId As String
Dim MyInvestorSt As String
Dim MyTranCode As String
Dim AlreadyKyc As String
Dim rsDup As New ADODB.Recordset

Private Sub cmbBankName_DropDown()
Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
cmbBankName.Clear
While Not rsAmc.EOF
  cmbBankName.AddItem rsAmc(0)
  rsAmc.MoveNext
Wend
rsAmc.Close
End Sub

Private Sub cmbcity_Change()
cmbcity_Click
End Sub

Private Sub cmbcity_Click()
If cmbCity.Text <> "" Then
    City = Split(cmbCity.Text, "#")
    MyCityCode = City(1)
    MyCityName = City(0)
End If
End Sub
Private Sub cmbCity_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbCity.MatchFound = False Then
      cmbCity.ListIndex = -1
End If
End Sub

Private Sub cmbclientn_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbclientn.MatchFound = False Then
      cmbclientn.ListIndex = -1
End If
End Sub
Private Sub cmbcomminterval_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbcomminterval.MatchFound = False Then
      cmbcomminterval.ListIndex = -1
End If
End Sub

Private Sub cmbgender_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbgender.MatchFound = False Then
      cmbgender.ListIndex = -1
End If
End Sub

Private Sub cmbincome_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbincome.MatchFound = False Then
      cmbincome.ListIndex = -1
End If
End Sub
Private Sub cmbinvestment_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbinvestment.MatchFound = False Then
      cmbinvestment.ListIndex = -1
End If
End Sub

Private Sub cmbkyc_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbkyc.MatchFound = False Then
      cmbkyc.ListIndex = -1
End If
End Sub
Private Sub cmbkyc1_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbkyc1.MatchFound = False Then
      cmbkyc1.ListIndex = -1
End If
End Sub

Private Sub cmboccupation_Change()
Call cmboccupation_Click
End Sub

Private Sub cmboccupation_Click()
If cmboccupation.Text <> "" Then
    MyOcc = Split(cmboccupation.Text, "#")
    MyOccCode = MyOcc(1)
End If
If UCase(Trim(cmboccupation.Text)) = UCase(Trim("Others")) Then
    TxtAcOther1.Enabled = True
Else
    TxtAcOther1.Enabled = False
End If
End Sub

Private Sub cmboccupation_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmboccupation.MatchFound = False Then
      cmboccupation.ListIndex = -1
End If
End Sub

Private Sub cmbplantype_Click()
txtchqamt = ""
If cmbplantype.Text <> "" Then
    Plan = Split(cmbplantype.Text, "=")
    MySchemeCode = Trim(Plan(1))
    MyMutCode = Trim(Plan(2))
End If
End Sub

Private Sub cmbprefcommmode_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbprefcommmode.MatchFound = False Then
      cmbprefcommmode.ListIndex = -1
End If
End Sub

Private Sub cmbresinri_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbresinri.MatchFound = False Then
      cmbresinri.ListIndex = -1
End If
End Sub

Private Sub cmbstate_Change()
cmbstate_Click
End Sub

Private Sub cmbstate_Click()
If cmbstate.Text <> "" Then
    MyState = Split(cmbstate.Text, "#")
    MyStateCode = MyState(1)
End If
End Sub

Private Sub cmbstate_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbstate.MatchFound = False Then
      cmbstate.ListIndex = -1
End If
End Sub

Private Sub cmbStatus_Change()
Call cmbStatus_Click
End Sub

Private Sub cmbStatus_Click()
If UCase(Trim(cmbStatus.Text)) = UCase(Trim("Others")) Then
    txtAcOther.Enabled = True
Else
    txtAcOther.Enabled = False
End If
If UCase(Trim(cmbStatus.Text)) = UCase(Trim("individual")) Then
    chkiadd.Enabled = True
    chkipan.Enabled = True
    chknipan.Enabled = False
    chkniadd.Enabled = False
    chknipower.Enabled = False
Else
    chknipan.Enabled = True
    chkniadd.Enabled = True
    chknipower.Enabled = True
    chkiadd.Enabled = False
    chkipan.Enabled = False
End If
End Sub

Private Sub cmbstatus_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbStatus.MatchFound = False Then
      cmbStatus.ListIndex = -1
End If
End Sub

Private Sub cmbtitle_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbtitle.MatchFound = False Then
      cmbtitle.ListIndex = -1
End If
End Sub

Private Sub cmbtitle_LostFocus()
If UCase(Trim(cmbtitle.Text)) = UCase(Trim("others")) Then
    txttitle.Enabled = True
Else
    txttitle.Enabled = False
End If
End Sub


Private Sub cmdExit1_Click()
    Unload Me
End Sub


Private Sub cmdReset1_Click()
    CMDRESET_Click
End Sub



Private Sub DOB_LostFocus()
If DOB.Text <> "__/__/____" Then
If DateDiff("YYYY", Format(DOB.Text, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
    frameguardian.Enabled = True
Else
    frameguardian.Enabled = False
End If
End If
End Sub
Private Sub dtChqDate_LostFocus()
    If IsDate(dtChqDate.Text) = False Then
        MsgBox "Enter Valid Date", vbInformation
        dtChqDate.SetFocus
        Exit Sub
    End If
End Sub
Private Sub Form_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        If flag = False Then SendKeys "{tab}"
    End If
End Sub

Private Sub optcash_Click()
cmbBankName.Clear
cmbBankName.AddItem "Cash"
cmbBankName.Enabled = False
txtChqNo.Enabled = False
dtChqDate.Enabled = False
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub
Private Sub optcheque_Click()
Label111.Caption = "Cheque No."
Label222.Caption = "Cheque Dated."
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub
Private Sub optdraft_Click()
Label111.Caption = "Draft No."
Label222.Caption = "Draft Dated."
cmbBankName.Enabled = True
txtChqNo.Enabled = True
dtChqDate.Enabled = True
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub



Private Sub Picture4_Click()
On Error Resume Next
selected_client_key = treeClient.SelectedItem.Key
Guest_AR = True
Guest_ARGen = False
Load frm_guestsearch
frm_guestsearch.client_master_flg = "Client" 'gaurav
frm_guestsearch.MyGuestCode = TguestCode.Text
frm_guestsearch.Show
frm_guestsearch.ZOrder
End Sub




Private Sub TguestCode_LostFocus()
If TguestCode.Text <> "" Then
  txtbusicode.Text = SqlRet("select nvl(emp_no,0) from bajaj_venue_booking where guest_cd='" & TguestCode & "'")
  Call txtbusicode_KeyUp(55, 0)
End If
End Sub

Private Sub txtAmount_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
   KeyAscii = 0
End If
If KeyAscii = 13 Then
  SendKeys "{TAB}"
End If
End Sub


Private Sub txtbusicode_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim b_cd() As String
MyBranch = ""
MyRmCode = ""
    If Len(txtbusicode.Text) = 5 Then
        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.category_id in('2001','2018')", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            MyBranch = rsEmp(0)
            MyRmCode = rsEmp(3)
            Label32.Caption = rsEmp(1)
            Label55.Caption = rsEmp(2)
        Else
            MsgBox "Rm Should Be Sale Support And Marketing "
            Label32.Caption = ""
            Label55.Caption = ""
            txtbusicode = ""
            Exit Sub
        End If
        rsEmp.Close
        Set rsEmp = Nothing
    End If
End Sub

Private Sub txtClientCode_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtclientname_GotFocus()
    If cmbtitle.Text = "" Then
        MsgBox "Please Choose Title First"
        cmbtitle.SetFocus
        Exit Sub
    End If
End Sub

Private Sub txtclientname_Validate(Cancel As Boolean)
'Dim MyName() As String
'If InStr(txtclientname.Text, ".") = 0 Then
'    txtclientname.Text = Replace(txtclientname.Text, "  ", " ")
'    MyName = Split(txtclientname.Text, " ")
'    If UBound(MyName) = 1 Then
'        txtclientname = MyName(0)
'        txtlstname = MyName(1)
'    ElseIf UBound(MyName) = 2 Then
'        txtclientname = MyName(0)
'        txtlstname = MyName(1) & MyName(2)
'    End If
'End If
End Sub

Private Function CheckDuplicate() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
        If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
            If SqlRet("select nvl(count(*),0) from client_test where client_pan='" & VSFCommGrdE.TextMatrix(i, 3) & "'") >= 1 Then
                 DupFlag = False
                 Exit For
            End If
        End If
     Next
End If
CheckDuplicate = DupFlag
End Function

Private Function CheckDuplicateInvestor() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE1.Rows > 1 Then
    For i = 1 To VSFCommGrdE1.Rows - 1
        If VSFCommGrdE1.TextMatrix(i, 3) <> "" Then
            If UCase(VSFCommGrdE1.TextMatrix(i, 3)) = UCase(VSFCommGrdE1.TextMatrix(i + 1, 3)) Then
                 DupFlag = False
                 Exit For
            End If
        End If
     Next
End If
CheckDuplicateInvestor = DupFlag
End Function


Private Sub txtpans_Validate(Cancel As Boolean)
If txtpans.Text <> "" Then
    txtclientcode.Text = SqlRet("select NVL(main_code,0) from client_test where upper(Trim(client_pan))='" & Trim(UCase(txtpans)) & "'")
End If
End Sub




Private Sub VSFCommGrdE1_ValidateEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
If VSFCommGrdE1.Col = 4 Then
   If VSFCommGrdE1.EditText = "  /  /    " Then Exit Sub
   If Not IsDate(Format(VSFCommGrdE1.EditText, "dd/mm/yyyy")) Then
      MsgBox "Please enter Valid date!", vbInformation + vbOKOnly, strBajaj
      Cancel = True
   End If
End If
If VSFCommGrdE1.Col = 3 Then
    If VSFCommGrdE1.EditText <> "" Then
        If ValidatePan(VSFCommGrdE1.EditText) = False Then
             MsgBox "Please Enter Valid Pan No"
             Cancel = True
        End If
    End If
End If
If VSFCommGrdE1.Col = 11 Then
    If VSFCommGrdE1.EditText <> "" Then
        If DateDiff("YYYY", Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 4), "DD/MM/YYYY"), ServerDateTime) < 18 Then
            If ValidatePan(VSFCommGrdE1.EditText) = False Then
                  MsgBox "Please Enter Valid Pan No"
                  Cancel = True
             End If
        End If
    End If
End If

End Sub
Private Function CheckMandatory() As Boolean
Dim DupMan As Boolean
DupMan = True
If VSFCommGrdE1.Rows > 1 Then
    For i = 1 To VSFCommGrdE1.Rows - 1
        If UCase(VSFCommGrdE1.TextMatrix(i, 1)) <> "" Or UCase(VSFCommGrdE1.TextMatrix(i, 2)) <> "" Then
            If UCase(VSFCommGrdE1.TextMatrix(i, 4)) = "" Or Mid(UCase(VSFCommGrdE1.TextMatrix(i, 5)), 1, 1) = "" Or Mid(UCase(VSFCommGrdE1.TextMatrix(i, 5)), 1, 1) = " " Or Mid(UCase(VSFCommGrdE1.TextMatrix(i, 7)), 1, 1) = " " Or Mid(UCase(VSFCommGrdE1.TextMatrix(i, 7)), 1, 1) = "" Then
                 DupMan = False
                 Exit For
            End If
        End If
     Next
End If
CheckMandatory = DupMan
End Function
Private Function CheckDuplicateMainCode() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE1.Rows > 1 Then
    For i = 1 To VSFCommGrdE1.Rows - 1
        If VSFCommGrdE1.TextMatrix(i, 3) <> "" And UCase(Trim(txtclientpan.Text)) <> "" Then
            If UCase(VSFCommGrdE1.TextMatrix(i, 3)) = UCase(Trim(txtclientpan.Text)) Then
                 DupFlag = False
                 Exit For
            End If
        End If
     Next
End If
CheckDuplicateMainCode = DupFlag
End Function
Private Sub cmdExit_Click()
    Unload Me
End Sub

Private Sub cmdnewocc_Click()
    occn = InputBox("Enter occupation", Occupation)
    MyConn.Execute "insert into occupation_master (occ_name,loggeduserid,timest)values('" & Trim(occn) & "','" & Glbloginid & "','" & Format(ServerDateTime, "DD-MMM-YYYY") & "')"
    MyConn.Execute "commit"
    If sql_ora = "SQL" Then
        oc = "select distinct OCC_ID,OCC_NAME from occupation_master where occ_id is not null"
    Else
        oc = "select distinct occ_id,occ_name from occupation_master where occ_id is not null "
    End If
    Set RSV = MyConn.Execute(oc)
    If Not RSV.EOF Then
        Do While Not RSV.EOF
            cmboccupation.AddItem IIf(IsNull(RSV(1)), "", RSV(1))
            MyOccCode = RSV(0)
            RSV.MoveNext
        Loop
    End If
    RSV.Close
End Sub

Private Sub CMDRESET_Click()
Dim i As Integer
cmbacatg.ListIndex = -1
cmbinvestor.ListIndex = -1
txtpans.Text = ""
cmdAdd.Enabled = True
Picture1.Enabled = True
txtbusicode.Enabled = True
cmdUpdate.Enabled = False
txtlstname.Text = ""
VSFCommGrdE.Enabled = True
DOB.Text = "__/__/____"
txtclientcodeold.Text = ""
txtaddress1.Text = ""
txtaddress2.Text = ""
cmbkyc.Text = ""
cmbkyc1.Text = ""
chk_g_panproof.Value = 0
chk_g_form60.Value = 0
chk_g_form61.Value = 0
chkactst.Value = 0
chkactst.Value = 0
chknews.Value = 0
chksi.Value = 0
chkipan.Value = 0
chkiadd.Value = 0
chkniadd.Value = 0
chknipan.Value = 0
chknipower.Value = 0
chkpanproof.Value = 0
chkform60.Value = 0
chkform61.Value = 0
chkform60.Value = 0
chkform61.Value = 0
txtclientcode.Text = ""
cmbclientn.Text = ""
txtclientname.Text = ""
txtclientpan.Text = ""
txtdp_bid.Text = ""
txtEmail.Text = ""
txtFax.Text = ""
txtguardiann.Text = ""
txtguardianname.Text = ""
txtguardianpan.Text = ""
txtMobile.Text = ""
txtoverseasadd.Text = ""
TxtTel1.Text = ""
TxtTel2.Text = ""
txtPin.Text = ""
txtstd1.Text = ""
cmbCity.ListIndex = -1
cmbstate.ListIndex = -1
txtSTD.Text = ""
txttitle.Text = ""
txttradingid.Text = ""
cmbcomminterval.Text = ""
cmbgender.Text = ""
cmbincome.Text = ""
cmbinvestment.Text = ""
cmbkyc.Text = ""
cmbkyc1.Text = ""
cmboccupation.ListIndex = -1
cmbprefcommmode.Text = ""
cmbresinri.Text = ""
cmbStatus.Text = ""
cmbtitle.Text = ""
txtstd1.Text = ""
MyInvestorSt = ""
MySourceId = ""
MainAccountCode = ""
MyUpdateInvestor = ""
Call SetGrid
Call SetGrid1
cmdAdd.Enabled = True
End Sub

Public Sub cmdSearch_Click()
Call gridfill
Call SetGrid
Call SetGrid1
cmdUpdate.Enabled = False
sql = " SELECT a.category_id,a.investor_code,c.payroll_id,a.OCC_ID,b.city_id,a.std1,a.std2,a.status,b.investor_name,a.address1,a.address2,a.phone1,a.phone2,a.city_name,a.pincode,b.dob,b.mobile,b.email,b.gender,a.fax,b.pan "
sql = sql & "  FROM client_master a, investor_master b,employee_master c "
sql = sql & " WHERE a.client_code = SUBSTR (b.inv_code, 1, 8) AND inv_code='" & txtclientcodeold & "' AND a.rm_code=c.rm_code"
RsSearch.open sql, MyConn, adOpenForwardOnly
If Not RsSearch.EOF Then
    txtbusicode.Text = RsSearch.Fields("payroll_id")
    cmbclientname = RsSearch.Fields("investor_name")
    txtaddress1.Text = IIf(IsNull(RsSearch.Fields("address1")), "", RsSearch.Fields("address1"))
    txtaddress2.Text = IIf(IsNull(RsSearch.Fields("address2")), "", RsSearch.Fields("address2"))
    txtFax.Text = IIf(IsNull(RsSearch.Fields("fax")), "", RsSearch.Fields("fax"))
    txtEmail.Text = IIf(IsNull(RsSearch.Fields("email")), "", RsSearch.Fields("email"))
    txtclientpan.Text = IIf(IsNull(RsSearch.Fields("pan")), "", RsSearch.Fields("pan"))
    If Not IsNull(RsSearch!DOB) Then
        DOB.Text = Format(RsSearch!DOB, "DD/MM/YYYY")
    End If
    txtMobile.Text = IIf(IsNull(RsSearch.Fields("mobile")), "", RsSearch.Fields("mobile"))
    txtPin.Text = IIf(IsNull(RsSearch.Fields("pincode")), "", RsSearch.Fields("pincode"))
    For i = 0 To cmbgender.ListCount - 1
        If UCase(Trim(cmbgender.List(i))) = UCase(Trim(RsSearch!GENDER)) Then
            cmbgender.ListIndex = i
        End If
    Next
    For i = 0 To cmbCity.ListCount - 1
        City = Split(cmbCity.List(i), "#")
        If UCase(Trim(City(1))) = UCase(Trim(RsSearch!City_id)) Then
            cmbCity.ListIndex = i
            Exit For
        End If
    Next
    'investor  category
    For i = 0 To cmbinvestor.ListCount - 1
        INVESTORCODE = Split(cmbinvestor.List(i), "#")
        If UCase(Trim(INVESTORCODE(1))) = UCase(Trim(RsSearch!Investor_code)) Then
            cmbinvestor.ListIndex = i
            Exit For
        End If
    Next
    'Client  category
    For i = 0 To cmbacatg.ListCount - 1
        ClienCode = Split(cmbacatg.List(i), "#")
        If UCase(Trim(ClienCode(1))) = UCase(Trim(RsSearch!category_id)) Then
            cmbacatg.ListIndex = i
            Exit For
        End If
    Next
    For i = 0 To cmbStatus.ListCount - 1
        If UCase(Trim(cmbStatus.List(i))) = UCase(Trim(RsSearch!status)) Then
        cmbStatus.ListIndex = i
        End If
    Next
    txtSTD.Text = IIf(IsNull(RsSearch.Fields("std1")), "", RsSearch.Fields("std1"))
    txtstd1.Text = IIf(IsNull(RsSearch.Fields("std2")), "", RsSearch.Fields("std2"))
    TxtTel1.Text = IIf(IsNull(RsSearch.Fields("phone1")), "", RsSearch.Fields("phone1"))
    TxtTel2.Text = IIf(IsNull(RsSearch.Fields("phone2")), "", RsSearch.Fields("phone2"))
    On Error Resume Next
    For i = 0 To cmboccupation.ListCount - 1
        MyOcc = Split(cmboccupation.List(i), "#")
        If UCase(Trim(MyOcc(1))) = UCase(Trim(RsSearch!Occ_ID)) Then
            cmboccupation.ListIndex = i
            Exit For
        End If
    Next
End If
RsSearch.Close
Call txtbusicode_KeyUp(99, 0)
'Fill Extra Information ---------------------------------------------------------
Dim Rs_view As New ADODB.Recordset
Rs_view.open "select * from client_test   where  client_codekyc='" & txtclientcodeold & "'", MyConn, adOpenForwardOnly
If Not Rs_view.EOF Then
    txttradingid = IIf(IsNull(Rs_view.Fields("tradingid")), "", Rs_view.Fields("tradingid"))
    txtdp_bid = IIf(IsNull(Rs_view.Fields("benefi_id")), "", Rs_view.Fields("benefi_id"))
    cmbStatus.Text = IIf(IsNull(Rs_view.Fields("status")), "", Rs_view.Fields("status"))
    txtAcOther.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
    
         
    TxtAcOther1.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
    cmbtitle.Text = IIf(IsNull(Rs_view.Fields("TITLE")), "", Rs_view.Fields("TITLE"))
    txtclientname.Text = IIf(IsNull(Rs_view.Fields("client_name")), "", Rs_view.Fields("client_name"))
    cmbclientn.Text = IIf(IsNull(Rs_view.Fields("nationality")), "", Rs_view.Fields("nationality"))
    
    cmbincome.Text = IIf(IsNull(Rs_view.Fields("annual_income")), "", Rs_view.Fields("annual_income"))
    cmbinvestment.Text = IIf(IsNull(Rs_view.Fields("investement")), "", Rs_view.Fields("investement"))
    cmbresinri.Text = IIf(IsNull(Rs_view.Fields("resident_nri")), "", Rs_view.Fields("resident_nri"))
    
    StrEnclosedPanA = IIf(IsNull(Rs_view.Fields("a_enclosed_pan")), "", Rs_view.Fields("a_enclosed_pan"))
    If StrEnclosedPanA = "YES" Then
        chkpanproof.Value = 1
    Else
        chkpanproof.Value = 0
    End If
    StrEnclosed60A = IIf(IsNull(Rs_view.Fields("a_enclosed_60")), "", Rs_view.Fields("a_enclosed_60"))
    If StrEnclosed60A = "YES" Then
        chkform60.Value = 1
    Else
        chkform60.Value = 0
    End If
    StrEnclosed61A = IIf(IsNull(Rs_view.Fields("a_enclosed_61")), "", Rs_view.Fields("a_enclosed_61"))
    If StrEnclosed61A = "YES" Then
        chkform61.Value = 1
    Else
        chkform61.Value = 0
    End If
    StrEnclosedPanG = IIf(IsNull(Rs_view.Fields("G_enclosed_pan")), "", Rs_view.Fields("G_enclosed_pan"))
    If StrEnclosedPanG = "YES" Then
        chk_g_panproof.Value = 1
    Else
        chk_g_panproof.Value = 0
    End If
    StrEnclosed60G = IIf(IsNull(Rs_view.Fields("g_enclosed_60")), "", Rs_view.Fields("g_enclosed_60"))
    If StrEnclosed60G = "YES" Then
        chk_g_form60.Value = 1
    Else
        chk_g_form60.Value = 0
    End If
    StrEnclosed61G = IIf(IsNull(Rs_view.Fields("g_enclosed_61")), "", Rs_view.Fields("g_enclosed_61"))
    If StrEnclosed61G = "YES" Then
        chk_g_form61.Value = 1
    Else
        chk_g_form61.Value = 0
    End If

    cmbkyc.Text = IIf(IsNull(Rs_view.Fields("a_kyc_status")), "", Rs_view.Fields("a_kyc_status"))
    cmbkyc1.Text = IIf(IsNull(Rs_view.Fields("g_kyc_status")), "", Rs_view.Fields("g_kyc_status"))
    
    txtguardianname.Text = IIf(IsNull(Rs_view.Fields("g_name")), "", Rs_view.Fields("g_name"))
    txtguardiann.Text = IIf(IsNull(Rs_view.Fields("g_nationality")), "", Rs_view.Fields("g_nationality"))
    txtguardianpan.Text = IIf(IsNull(Rs_view.Fields("g_pan")), "", Rs_view.Fields("g_pan"))
    
    txtoverseasadd.Text = IIf(IsNull(Rs_view.Fields("overseas_add")), "", Rs_view.Fields("overseas_add"))
    ActSt = IIf(IsNull(Rs_view.Fields("tele_Act_statement")), "", Rs_view.Fields("tele_Act_statement"))
    If ActSt = "YES" Then
        chkactst.Value = 1
    Else
        chkactst.Value = 0
    End If
    News = IIf(IsNull(Rs_view.Fields("tele_newsletter")), "", Rs_view.Fields("tele_newsletter"))
    If News = "YES" Then
        chknews.Value = 1
    Else
        chknews.Value = 0
    End If
    sInfo = IIf(IsNull(Rs_view.Fields("tele_statutory_info")), "", Rs_view.Fields("tele_statutory_info"))
    If sInfo = "YES" Then
        chksi.Value = 1
    Else
        chksi.Value = 0
    End If
    NIPan = IIf(IsNull(Rs_view.Fields("de_ni_pan")), "", Rs_view.Fields("de_ni_pan"))
    If NIPan = "YES" Then
        chknipan.Value = 1
    Else
        chknipan.Value = 0
    End If
    NIAdd = IIf(IsNull(Rs_view.Fields("de_ni_addproof")), "", Rs_view.Fields("de_ni_addproof"))
    If NIAdd = "YES" Then
        chkniadd.Value = 1
    Else
        chkniadd.Value = 0
    End If
    NIPower = IIf(IsNull(Rs_view.Fields("de_ni_power")), "", Rs_view.Fields("de_ni_power"))
    If NIPower = "YES" Then
        chknipower.Value = 1
    Else
        chknipower.Value = 0
    End If
    IPan = IIf(IsNull(Rs_view.Fields("de_i_pan")), "", Rs_view.Fields("de_i_pan"))
    If IPan = "YES" Then
        chkipan.Value = 1
    Else
        chkipan.Value = 0
    End If
    IAdd = IIf(IsNull(Rs_view.Fields("de_i_add_proof")), "", Rs_view.Fields("de_i_add_proof"))
    If IAdd = "YES" Then
        chkiadd.Value = 1
    Else
        chkiadd.Value = 0
    End If
    '-----mayank grid fill-------------------
    StrInv = ""
    If Len(txtclientcodeold.Text) >= 11 Then
        If rs_inv.State = 1 Then rs_inv.Close
        rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(txtclientcodeold, 8) & "' AND INV_CODE<>'" & txtclientcodeold.Text & "' order by inv_code", MyConn, adOpenForwardOnly
        If Not rs_inv.EOF Then
           Do While Not rs_inv.EOF
               StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!INV_CODE & "|"
               rs_inv.MoveNext
           Loop
        End If
        rs_inv.Close
    End If
        VSFCommGrdE.ColComboList(0) = StrInv
    MainAccountCode = SqlRet("select Main_code from client_test where client_codekyc='" & Trim(txtclientcodeold.Text) & "'")
    If MainAccountCode <> "" Then
        RsGridFill.open "select a.client_name,a.client_codekyc,a.client_pan,a.dob,a.inv_kyc,a.g_name,a.g_pan,b.relation_name,b.relation_id,c.occ_name,c.occ_id,title from client_test a,relationship_master b,occupation_master c where main_code='" & MainAccountCode & "' and client_code<>'" & MainAccountCode & "' and a.relation_id=b.relation_id(+) and a.occ_id=c.occ_id(+) ", MyConn, adOpenForwardOnly
        i = 1
        If Not RsGridFill.EOF Then
            Do While Not RsGridFill.EOF
                VSFCommGrdE.TextMatrix(i, 0) = RsGridFill.Fields("client_name") & Space(50) & "#" & RsGridFill.Fields("client_codekyc")
                VSFCommGrdE.TextMatrix(i, 1) = RsGridFill.Fields("client_codekyc")
                VSFCommGrdE.TextMatrix(i, 2) = RsGridFill.Fields("client_name")
                VSFCommGrdE.TextMatrix(i, 3) = IIf(IsNull(RsGridFill.Fields("client_pan")), "", RsGridFill.Fields("client_pan"))
                VSFCommGrdE.TextMatrix(i, 4) = RsGridFill.Fields("dob")
                VSFCommGrdE.TextMatrix(i, 5) = RsGridFill.Fields("relation_name") & Space(50) & "#" & RsGridFill.Fields("relation_id")
                VSFCommGrdE.TextMatrix(i, 7) = RsGridFill.Fields("occ_name") & Space(50) & "#" & RsGridFill.Fields("occ_id")
                VSFCommGrdE.TextMatrix(i, 9) = IIf(IsNull(RsGridFill.Fields("inv_kyc")), "", RsGridFill.Fields("inv_kyc"))
                VSFCommGrdE.TextMatrix(i, 10) = IIf(IsNull(RsGridFill.Fields("g_name")), "", RsGridFill.Fields("g_name"))
                VSFCommGrdE.TextMatrix(i, 11) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
                VSFCommGrdE.TextMatrix(i, 13) = IIf(IsNull(RsGridFill.Fields("title")), "", RsGridFill.Fields("title"))
                RsGridFill.MoveNext
                i = i + 1
            Loop
        Else
            VSFCommGrdE.TextMatrix(i, 0) = ""
            VSFCommGrdE.TextMatrix(i, 1) = ""
            VSFCommGrdE.TextMatrix(i, 2) = ""
            VSFCommGrdE.TextMatrix(i, 3) = ""
            VSFCommGrdE.TextMatrix(i, 4) = ""
            VSFCommGrdE.TextMatrix(i, 5) = ""
            VSFCommGrdE.TextMatrix(i, 7) = ""
            VSFCommGrdE.TextMatrix(i, 9) = ""
            VSFCommGrdE.TextMatrix(i, 10) = ""
            VSFCommGrdE.TextMatrix(i, 11) = ""
            VSFCommGrdE.TextMatrix(i, 13) = ""
        End If
        RsGridFill.Close
    End If
'---------------------------------------------------------------------------------------
End If
'--------------------------------------------------------------------------------
End Sub

Private Sub cmdUpdate_Click()
On Error GoTo errr
Dim i As Integer
Dim Sql1 As String
Dim Str() As String
Dim ClCat As String
Dim invCat As String
'---------------------------------------Mayank Garg------------------------------------------
For i = 1 To VSFCommGrdE1.Rows - 1
If VSFCommGrdE1.TextMatrix(i, 2) <> "" Then
If VSFCommGrdE1.TextMatrix(i, 13) = "" Then
   MsgBox "Title can't be null"
   Exit Sub
   End If
   End If
Next

If CheckDuplicateInvestor = False Then
   MsgBox "Check The Duplicacy Of Pan No Of Investor"
   Exit Sub
End If
If CheckDuplicateMainCode = False Then
    MsgBox "Pan No Already Exist For Main Account"
   Exit Sub
End If
If CheckMandatory = False Then
   MsgBox "Fill All The Details of investor"
   Exit Sub
End If

Set RSV = MyConn.Execute("select main_code from client_test where client_code='" & txtclientcode & "'")
If Not RSV.EOF Then
    sql = " select a.client_codekyc,a.client_name,a.main_code,a.client_code from client_test a,client_test b where a.client_code=b.main_code and a.client_code=b.client_code "
    sql = " select client_codekyc,client_name,main_code,client_code from client_test where client_code=main_code"
    sql = sql & " and main_code = '" & RSV(0) & "' "
    If RSV1.State = 1 Then RSV1.Close
    RSV1.open sql, MyConn, adOpenForwardOnly
    If Not RSV1.EOF Then
        MyUpdateClient = RSV1(0)
    End If
    RSV1.Close
End If
RSV.Close
'FOR GRID ENTRY(Now There May Be Two Type Of Client In Grid One New For Kyc One Already Kyc-------------
'If Account Is Already Is Not KYC Then It Shoud Be Inserted In client_test And Update In Investor Master
'If This Account Is  Kyc Then It Should Be Updated In client_test
'-------------------------------------------------------------------------------------------------------
MyConn.BeginTrans
For i = 1 To VSFCommGrdE1.Rows - 1
    If VSFCommGrdE1.TextMatrix(i, 1) <> "" Then
        AlreadyKyc = SqlRet("select nvl(count(*),0) from client_test where client_codekyc=" & VSFCommGrdE1.TextMatrix(i, 1) & "")
        If AlreadyKyc > 0 Then 'It Means It Is KYC
            MySourceId = Mid(VSFCommGrdE1.TextMatrix(i, 1), 1, 8)
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
            End If
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id1 = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
            End If
            'update client test-------mayank
            sql = "update client_test set BRANCH_CODE=" & MyBranch & ",  clientCode_old='" & MyUpdateClient & "',tradingid='" & txttradingid.Text & "',benefi_id='" & txtdp_bid.Text & "',occ_id=" & Temp_Id(1) & ",relation_id = " & Temp_Id1(1) & ",inv_kyc='YES',g_name='" & VSFCommGrdE1.TextMatrix(i, 10) & "',g_pan= '" & VSFCommGrdE1.TextMatrix(i, 11) & "',"
            sql = sql & " title='" & VSFCommGrdE1.TextMatrix(i, 13) & "',client_name='" & VSFCommGrdE1.TextMatrix(i, 2) & "',nationality='" & cmbclientn.Text & "',client_pan='" & VSFCommGrdE1.TextMatrix(i, 3) & "',resident_nri='" & cmbresinri.Text & "',dob=TO_DATE('" & Trim(VSFCommGrdE1.TextMatrix(i, 4)) & "','DD-MM-YYYY'),"
            sql = sql & " add1='" & Replace(txtaddress1, ",", "") & "',add2='" & Replace(txtaddress2, ",", "") & "',city_id='" & MyCityCode & "',state_id='" & MyStateCode & "',pincode='" & txtPin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & txtSTD.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1 & "',tel2='" & TxtTel2.Text & "',mobile_no='" & txtMobile.Text & "',fax ='" & txtFax.Text & "',email='" & txtEmail & "',"
            sql = sql & " loggeduserid='" & Glbloginid & "',business_code=" & txtbusicode & ""
            sql = sql & " where client_codekyc='" & Trim(VSFCommGrdE1.TextMatrix(i, 1)) & "'"
            MyConn.Execute (sql)
            Ky = "update investor_master "
            Ky = Ky & " set investor_title='" & VSFCommGrdE1.TextMatrix(i, 13) & "',investor_name='" & VSFCommGrdE1.TextMatrix(i, 2) & "',address1='" & Replace(txtaddress1, ",", "") & "',address2='" & Replace(txtaddress2, ",", "") & "',"
            If txtFax <> "" Then
                Ky = Ky & " fax='" & txtFax & "',"
            Else
                Ky = Ky & " fax='0' ,"
            End If
            Ky = Ky & " email='" & txtEmail & "',pan='" & VSFCommGrdE1.TextMatrix(i, 3) & "',dob=to_date('" & VSFCommGrdE1.TextMatrix(i, 4) & "','DD-MM-YYYY'),"
            If txtMobile.Text <> "" Then
                Ky = Ky & " mobile='" & txtMobile & "',"
            Else
                Ky = Ky & " mobile='',"
            End If
            If txtPin.Text <> "" Then
                Ky = Ky & " pincode='" & txtPin & "',"
            Else
                Ky = Ky & " pincode='',"
            End If
            Ky = Ky & " gender='',"
            If TxtTel2.Text <> "" Then
                Ky = Ky & " phone='" & TxtTel2.Text & "' "
            Else
                Ky = Ky & " phone='' "
            End If
            Ky = Ky & " ,status='" & cmbStatus.Text & "',city_id='" & MyCityCode & "',"
            Ky = Ky & " kyc='YES',source_id=" & MySourceId & ",rm_code=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & ", "
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
                Ky = Ky & "  REL_ID= " & Temp_Id(1) & ","
            Else
                Ky = Ky & " REL_ID=0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
                Ky = Ky & " OCCUPATION_ID= " & Temp_Id(1) & ","
            Else
                Ky = Ky & " OCCUPATION_ID=0, "
            End If
            Ky = Ky & " G_NAME='" & Trim(VSFCommGrdE1.TextMatrix(i, 10)) & "',G_pan='" & Trim(VSFCommGrdE1.TextMatrix(i, 11)) & "'  "
            Ky = Ky & " where inv_code= " & Trim(VSFCommGrdE1.TextMatrix(i, 1)) & " "
            MyConn.Execute (Ky)
        ElseIf AlreadyKyc = 0 Then 'Insert Into client_test But Update Investor Master
            sql = "insert into client_test(client_codekyc,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,relation_id,inv_kyc,g_name,g_pan,others1,"
            sql = sql & " title,client_name,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,"
            sql = sql & " add1,add2,city_id,state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,fax,email,"
            sql = sql & " loggeduserid,business_code,timest,main_code) "
            sql = sql & " values(" & VSFCommGrdE1.TextMatrix(i, 1) & "," & Val(MyBranch) & ", '" & MyUpdateClient & "','" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbStatus.Text & "','" & txtAcOther.Text & "',"
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
                sql = sql & " " & Temp_Id(1) & ","
            Else
                 sql = sql & " 0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
                sql = sql & " " & Temp_Id(1) & ","
            Else
                 sql = sql & " 0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 9) <> "" Then
                sql = sql & " '" & VSFCommGrdE1.TextMatrix(i, 9) & "',"
            Else
                 sql = sql & " '',"
            End If
            sql = sql & " '" & VSFCommGrdE1.TextMatrix(i, 10) & "', '" & VSFCommGrdE1.TextMatrix(i, 11) & "',"
            sql = sql & " null,'" & VSFCommGrdE1.TextMatrix(i, 13) & "','" & VSFCommGrdE1.TextMatrix(i, 2) & "',null,'" & VSFCommGrdE1.TextMatrix(i, 3) & "',null,null,null,null,to_date('" & VSFCommGrdE1.TextMatrix(i, 4) & "','DD-MM-YYYY'),null,"
            sql = sql & " '" & txtaddress1 & "','" & txtaddress2 & "','" & MyCityCode & "','" & MyStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            
            If txtSTD.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & txtSTD.Text & "',"
            End If
            If txtstd1.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & txtstd1.Text & "',"
            End If
            If TxtTel1.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & Replace(TxtTel1.Text, "-", "") & "',"
            End If
            If TxtTel2.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & TxtTel2.Text & "',"
            End If
            If txtMobile.Text = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " '" & txtMobile.Text & "',"
            End If
            If txtFax.Text = "" Then
                sql = sql & " '0' ,"
            Else
                sql = sql & " '" & txtFax.Text & "',"
            End If
            sql = sql & " '" & txtEmail & "', "
            sql = sql & " '" & Glbloginid & "','" & txtbusicode & "','" & Format(ServerDateTime, "DD-MMM-YYYY") & "',"
            sql = sql & "'" & MainAccountCode & "')"
            MyConn.Execute (sql)
            
             Ky = "update investor_master "
             Ky = Ky & " set investor_title='" & VSFCommGrdE1.TextMatrix(i, 13) & "',investor_name='" & VSFCommGrdE1.TextMatrix(i, 2) & "',address1='" & Replace(txtaddress1, ",", "") & "',address2='" & Replace(txtaddress2, ",", "") & "',"
             If txtFax <> "" Then
                 Ky = Ky & " fax='" & txtFax & "',"
             Else
                 Ky = Ky & " fax='0' ,"
             End If
             Ky = Ky & " email='" & txtEmail & "',pan='" & VSFCommGrdE1.TextMatrix(i, 3) & "',dob=to_date('" & VSFCommGrdE1.TextMatrix(i, 4) & "','DD-MM-YYYY'),"
             If txtMobile.Text <> "" Then
                 Ky = Ky & " mobile='" & txtMobile & "',"
             Else
                 Ky = Ky & " mobile='',"
             End If
             If txtPin.Text <> "" Then
                 Ky = Ky & " pincode='" & txtPin & "',"
             Else
                 Ky = Ky & " pincode=' ',"
             End If
             Ky = Ky & " gender='',"
             If TxtTel2.Text <> "" Then
                 Ky = Ky & " phone='" & TxtTel2.Text & "' "
             Else
                 Ky = Ky & " phone='' "
             End If
             Ky = Ky & " ,status='" & cmbStatus.Text & "',city_id='" & MyCityCode & "',"
             Ky = Ky & " kyc='YES',source_id=" & MySourceId & ",rm_code=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & ", "
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
                Ky = Ky & "  REL_ID= " & Temp_Id(1) & ","
            Else
                Ky = Ky & " REL_ID=0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
                Ky = Ky & " OCCUPATION_ID= " & Temp_Id(1) & ","
            Else
                Ky = Ky & "  OCCUPATION_ID=0, "
            End If
            Ky = Ky & " G_NAME='" & Trim(VSFCommGrdE1.TextMatrix(i, 10)) & "',G_pan='" & Trim(VSFCommGrdE1.TextMatrix(i, 11)) & "'  "
            Ky = Ky & " where inv_code= " & Trim(VSFCommGrdE1.TextMatrix(i, 1)) & " "
            MyConn.Execute (Ky)
        End If
   End If
        If (VSFCommGrdE1.TextMatrix(i, 1) = "" And VSFCommGrdE1.TextMatrix(i, 2) <> "") Then  'Agar New Entry Karte Hai Grid Par
            sql = "insert into client_test(BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,relation_id,inv_kyc,g_name,g_pan,others1,"
            sql = sql & " title,client_name,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,"
            sql = sql & " add1,add2,city_id,state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,fax,email,"
            sql = sql & " loggeduserid,business_code,timest,main_code) "
            sql = sql & " values(" & Val(MyBranch) & "," & MyUpdateClient & ",'" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbStatus.Text & "','" & txtAcOther.Text & "',"
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
                sql = sql & " " & Temp_Id(1) & ","
            Else
                sql = sql & " 0,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
                sql = sql & " " & Temp_Id(1) & ","
            Else
                sql = sql & " 0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 9) <> "" Then
                sql = sql & " '" & VSFCommGrdE1.TextMatrix(i, 9) & "',"
            Else
                sql = sql & " '',"
            End If
            sql = sql & " '" & VSFCommGrdE1.TextMatrix(i, 10) & "', '" & VSFCommGrdE1.TextMatrix(i, 11) & "',"
            sql = sql & " null,'" & VSFCommGrdE1.TextMatrix(i, 13) & "','" & VSFCommGrdE1.TextMatrix(i, 2) & "',null,'" & VSFCommGrdE1.TextMatrix(i, 3) & "',null,null,null,null,to_date('" & VSFCommGrdE1.TextMatrix(i, 4) & "','DD-MM-YYYY'),null,"
            sql = sql & " '" & txtaddress1 & "','" & txtaddress2 & "','" & MyCityCode & "','" & MyStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            
            If txtSTD.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & txtSTD.Text & "',"
            End If
            If txtstd1.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & txtstd1.Text & "',"
            End If
            If TxtTel1.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & Replace(TxtTel1.Text, "-", "") & "',"
            End If
            If TxtTel2.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & TxtTel2.Text & "',"
            End If
            If txtMobile.Text = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " '" & txtMobile.Text & "',"
            End If
            If txtFax.Text = "" Then
                sql = sql & " '0' ,"
            Else
                sql = sql & " '" & txtFax.Text & "',"
            End If
            sql = sql & " '" & txtEmail & "', "
            sql = sql & " '" & Glbloginid & "','" & txtbusicode & "','" & Format(ServerDateTime, "DD-MMM-YYYY") & "',"
            sql = sql & "'" & MainAccountCode & "')"
            'Insert New Acount In client_test
            MyConn.Execute (sql)
            
            '1 Client And 1 Investor hase Been Insert into client_master and investor_master
            'MySourceiD=ClientCode of new insert client in clientmaster
            '----------------------------------------ADD ALL GRID VALUE IN INVESTOR _MASTER --------------------
            Ky = "insert into investor_master (INVESTOR_CODE,investor_title,branch_Code,investor_name,address1,address2,fax,email,pan,dob,mobile,pincode,gender,phone,status,city_id,kyc,source_id,rm_code,rel_id,occupation_id)"
            Ky = Ky & " values(1,'" & VSFCommGrdE1.TextMatrix(i, 13) & "'," & MyBranch & "', '" & VSFCommGrdE1.TextMatrix(i, 2) & "','" & txtaddress1 & "','" & txtaddress2 & "',"
            If txtFax <> "" Then
                Ky = Ky & " " & txtFax & ","
            Else
                Ky = Ky & " 0 ,"
            End If
            Ky = Ky & " '" & txtEmail & "','" & VSFCommGrdE1.TextMatrix(i, 3) & "','" & Format(DOB.Text, "DD-MMM-YYYY") & "',"
            If txtMobile.Text <> "" Then
                Ky = Ky & " '" & txtMobile & "',"
            Else
                Ky = Ky & " '',"
            End If
            If txtPin.Text <> "" Then
                Ky = Ky & " '" & txtPin & "',"
            Else
                Ky = Ky & " ' ',"
            End If
            Ky = Ky & " '" & Left(Trim(cmbgender.Text), 1) & "',"
            If TxtTel2.Text <> "" Then
                Ky = Ky & " '" & TxtTel2.Text & "' "
            Else
                Ky = Ky & " '' "
            End If
            Ky = Ky & " ,'" & cmbStatus.Text & "','" & MyCityCode & "',"
            Ky = Ky & " 'YESP'," & MySourceId & "," & MyRmCode & ", "
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
                Ky = Ky & " " & Temp_Id(1) & ","
            Else
                 Ky = Ky & " 0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
                Ky = Ky & " " & Temp_Id(1) & ""
            Else
                 Ky = Ky & " 0 "
            End If
            Ky = Ky & " )"
            MyConn.Execute (Ky)
            Set clcode = New ADODB.Recordset
            Set clcode = MyConn.Execute("select max(client_code) from client_test where  MAIN_CODE='" & MainAccountCode & "'")
            If Not clcode.EOF Then
                  sql = "   update client_test set client_codekyc=( select max(inv_code) from investor_master where source_id='" & MySourceId & "') "
                  sql = sql & "  where client_code='" & clcode(0) & "' "
                  MyConn.Execute sql
            End If
            If clcode.State = 1 Then clcode.Close
            Dim rs_max_inv As New ADODB.Recordset
            rs_max_inv.open "SELECT TO_NUMBER(MAX(SUBSTR(INV_CODE,9,13))) FROM investor_master WHERE SOURCE_ID=" & MySourceId, MyConn, adOpenForwardOnly
            If Not rs_max_inv.EOF Then
                MAX_INV_CD = rs_max_inv(0)
            End If
            
            If MAX_INV_CD >= 999 Then
                cnt = Format(MAX_INV_CD, "00000") 'Format(MAX_INV_CD + 1, "00000")
            Else
                cnt = Format(MAX_INV_CD, "000") 'Format(MAX_INV_CD + 1, "000")
            End If
            
            rs_max_inv.Close
            Set rs_max_inv = Nothing
            '--------------------------------------------INSERT INTO MF SERVER----------------------------------------------
            'Remove Comment
            Ky = "insert into investor_master@mf.bajajcapital  (INVESTOR_CODE,investor_title,branch_code,investor_name,address1,address2,fax,email,pan,dob,mobile,pincode,gender,phone,status,city_id,kyc,source_id,rm_code,INV_CODE,rel_id,occupation_id)"
            Ky = Ky & " values(1,'" & VSFCommGrdE1.TextMatrix(i, 13) & "'," & MyBranch & "', '" & VSFCommGrdE1.TextMatrix(i, 2) & "','" & txtaddress1 & "','" & txtaddress2 & "',"
            If txtFax <> "" Then
                Ky = Ky & " " & txtFax & ","
            Else
                Ky = Ky & " 0 ,"
            End If
            Ky = Ky & " '" & txtEmail & "','" & VSFCommGrdE1.TextMatrix(i, 3) & "','" & Format(DOB.Text, "DD-MMM-YYYY") & "',"
            If txtMobile.Text <> "" Then
                Ky = Ky & " '" & txtMobile & "',"
            Else
                Ky = Ky & " '',"
            End If
            If txtPin.Text <> "" Then
                Ky = Ky & " '" & txtPin & "',"
            Else
                Ky = Ky & " '',"
            End If
            Ky = Ky & " '" & Left(Trim(cmbgender.Text), 1) & "',"
            If TxtTel2.Text <> "" Then
                Ky = Ky & " '" & TxtTel2.Text & "' "
            Else
                Ky = Ky & " '' "
            End If
            Ky = Ky & ",'" & cmbStatus.Text & "','" & MyCityCode & "',"
            Ky = Ky & " 'YESP'," & MySourceId & "," & MyRmCode & ",'" & MySourceId & cnt & "',"
            If VSFCommGrdE1.TextMatrix(i, 5) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 5), "#")
                Ky = Ky & " " & Temp_Id(1) & ","
            Else
                Ky = Ky & " 0 ,"
            End If
            If VSFCommGrdE1.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE1.TextMatrix(i, 7), "#")
                Ky = Ky & " " & Temp_Id(1) & ""
            Else
                Ky = Ky & " 0 "
            End If
            Ky = Ky & " )"
            'On Error Resume next
            MyConn.Execute (Ky)
        ElseIf VSFCommGrdE1.TextMatrix(i, 1) = "" And VSFCommGrdE1.TextMatrix(i, 2) = "" Then
             Exit For
        End If
Next
'ENDING OF GRID ENTRY-----------------------------------------------------------------------------------------
'-------------------------------------------------------------------------------------------------------------
MsgBox "The Requested Record has been Updated Successfully", vbInformation
MyConn.CommitTrans
Call CMDRESET_Click
cmdUpdate.Enabled = False
cmdAdd.Enabled = True
Exit Sub
errr:
MsgBox err.Description
Resume
End Sub


Public Sub cmdview_Click()
Dim Rs_view As New ADODB.Recordset
Dim rsCity As New ADODB.Recordset
Dim rsState As New ADODB.Recordset
Dim MyInvCode As String
Dim ClientCat As String
cmdAdd.Enabled = False
cmdUpdate.Enabled = True
'cmdSave.Enabled = True
Dim Rs_Relation As New ADODB.Recordset
Call SetGrid
Call SetGrid1
cmdUpdate.Enabled = True
If Trim(txtclientcode.Text) = "" And Trim(txtpans.Text) = "" Then
    MsgBox "Please Enter Account Code or PAN", vbInformation
    txtclientcode.SetFocus
    Exit Sub
End If
If txtpans.Text <> "" Then
    Rs_view.open "select * from client_test   where  client_pan='" & txtpans & "'", MyConn, adOpenForwardOnly
Else
     'Rs_view.open "select * from client_test   where  client_code=trim('" & txtclientcode & "')", myconn, adOpenForwardOnly
     Rs_view.open "SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN (select MAIN_CODE from client_test   where  client_code=trim('" & txtclientcode & "')) AND CLIENT_CODE=MAIN_CODE", MyConn, adOpenForwardOnly
End If
If Not Rs_view.EOF Then
        txtclientcode.Text = Rs_view("main_code")
        txtclientcodeold.Text = IIf(IsNull(Rs_view.Fields("CLIENTCODE_OLD")), 0, Rs_view.Fields("CLIENTCODE_OLD"))
        MySourceId = Mid(Rs_view("client_codekyc"), 1, 8)
        MyOldBusiCode = Rs_view("business_code")
        MainAccountCode = SqlRet("select Main_code from client_test where client_code='" & Trim(txtclientcode.Text) & "'")
        'FOR IPC THIS CONDITION WILL NOT BE CHECKED
        If GlbroleId = 13 Or GlbroleId = 78 Or GlbroleId = 76 Then
        Else
            If SqlRet("select count(*) from client_test where main_code='" & MainAccountCode & "' and approve_dt is not null") >= 1 Then
                Label74.Caption = "ACCOUNT IS APPROVED"
            Else
                cmdUpdate.Enabled = True
                Label74.Caption = "ACCOUNT IS NOT APPROVED"
            End If
        End If
        MyUpdateInvestor = IIf(IsNull(Rs_view("client_codekyc")), Rs_view("clientcode_old"), Rs_view("client_codekyc"))
        MyInvestorSt = IIf(IsNull(Rs_view("client_codekyc")), Rs_view("clientcode_old"), Rs_view("client_codekyc"))
        txtbusicode.Text = IIf(IsNull(Rs_view.Fields("business_code")), "", Rs_view.Fields("business_code"))
        If Rs_view("resident_nri") = "NRI" Then
           cmbresinri.ListIndex = 1
        Else
           cmbresinri.ListIndex = 0
        End If
        Call txtbusicode_KeyUp(1, 1)
        txttradingid = IIf(IsNull(Rs_view.Fields("tradingid")), "", Rs_view.Fields("tradingid"))
        txtdp_bid = IIf(IsNull(Rs_view.Fields("benefi_id")), "", Rs_view.Fields("benefi_id"))
        cmbStatus.Text = IIf(IsNull(Rs_view.Fields("status")), "", Rs_view.Fields("status"))
        txtAcOther.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
        
             
        TxtAcOther1.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
        cmbtitle.Text = IIf(IsNull(Rs_view.Fields("TITLE")), "", Rs_view.Fields("TITLE"))
        txtclientname.Text = IIf(IsNull(Rs_view.Fields("client_name")), "", Rs_view.Fields("client_name"))
        cmbclientn.Text = IIf(IsNull(Rs_view.Fields("nationality")), "", Rs_view.Fields("nationality"))
        txtclientpan.Text = IIf(IsNull(Rs_view.Fields("client_pan")), "", Rs_view.Fields("client_pan"))
    
        cmbincome.Text = IIf(IsNull(Rs_view.Fields("annual_income")), "", Rs_view.Fields("annual_income"))
        cmbinvestment.Text = IIf(IsNull(Rs_view.Fields("investement")), "", Rs_view.Fields("investement"))
        txttitle.Text = IIf(IsNull(Rs_view.Fields("title_other")), "", Rs_view.Fields("title_other"))
        cmbresinri.Text = IIf(IsNull(Rs_view.Fields("resident_nri")), "", Rs_view.Fields("resident_nri"))
        On Error Resume Next
        DOB.Text = Format(Rs_view.Fields("dob"), "dd/mm/yyyy")
        cmbgender.Text = IIf(IsNull(Rs_view.Fields("GENDER")), "", Rs_view.Fields("GENDER"))
    
        StrEnclosedPanA = IIf(IsNull(Rs_view.Fields("a_enclosed_pan")), "", Rs_view.Fields("a_enclosed_pan"))
        If StrEnclosedPanA = "YES" Then
            chkpanproof.Value = 1
        Else
            chkpanproof.Value = 0
        End If
        StrEnclosed60A = IIf(IsNull(Rs_view.Fields("a_enclosed_60")), "", Rs_view.Fields("a_enclosed_60"))
        If StrEnclosed60A = "YES" Then
            chkform60.Value = 1
        Else
            chkform60.Value = 0
        End If
        StrEnclosed61A = IIf(IsNull(Rs_view.Fields("a_enclosed_61")), "", Rs_view.Fields("a_enclosed_61"))
        If StrEnclosed61A = "YES" Then
            chkform61.Value = 1
        Else
            chkform61.Value = 0
        End If
        StrEnclosedPanG = IIf(IsNull(Rs_view.Fields("G_enclosed_pan")), "", Rs_view.Fields("G_enclosed_pan"))
        If StrEnclosedPanG = "YES" Then
            chk_g_panproof.Value = 1
        Else
            chk_g_panproof.Value = 0
        End If
        StrEnclosed60G = IIf(IsNull(Rs_view.Fields("g_enclosed_60")), "", Rs_view.Fields("g_enclosed_60"))
        If StrEnclosed60G = "YES" Then
            chk_g_form60.Value = 1
        Else
            chk_g_form60.Value = 0
        End If
        StrEnclosed61G = IIf(IsNull(Rs_view.Fields("g_enclosed_61")), "", Rs_view.Fields("g_enclosed_61"))
        If StrEnclosed61G = "YES" Then
            chk_g_form61.Value = 1
        Else
            chk_g_form61.Value = 0
        End If
    
        cmbkyc.Text = IIf(IsNull(Rs_view.Fields("a_kyc_status")), "", Rs_view.Fields("a_kyc_status"))
    '    cmbkyc1.Text = "pankaj"
        cmbkyc1.Text = IIf(IsNull(Rs_view.Fields("g_kyc_status")), "", Rs_view.Fields("g_kyc_status"))
        
        txtguardianname.Text = IIf(IsNull(Rs_view.Fields("g_name")), "", Rs_view.Fields("g_name"))
        txtguardiann.Text = IIf(IsNull(Rs_view.Fields("g_nationality")), "", Rs_view.Fields("g_nationality"))
        txtguardianpan.Text = IIf(IsNull(Rs_view.Fields("g_pan")), "", Rs_view.Fields("g_pan"))
        txtaddress1 = IIf(IsNull(Rs_view.Fields("add1")), "", Rs_view.Fields("add1"))
        txtaddress2.Text = IIf(IsNull(Rs_view.Fields("add2")), "", Rs_view.Fields("add2"))
        On Error Resume Next
        For i = 0 To cmbCity.ListCount - 1
            City = Split(cmbCity.List(i), "#")
            If UCase(Trim(City(1))) = UCase(Trim(Rs_view!City_id)) Then
                cmbCity.ListIndex = i
                Exit For
            End If
        Next
        
        For i = 0 To cmbstate.ListCount - 1
            MyState = Split(cmbstate.List(i), "#")
            If UCase(Trim(MyState(1))) = UCase(Trim(Rs_view!state_id)) Then
                cmbstate.ListIndex = i
                Exit For
            End If
        Next
         On Error Resume Next
         For i = 0 To cmboccupation.ListCount - 1
            MyOcc = Split(cmboccupation.List(i), "#")
            If UCase(Trim(MyOcc(1))) = UCase(Trim(Rs_view!Occ_ID)) Then
                cmboccupation.ListIndex = i
                Exit For
            End If
        Next
        MyInvCode = SqlRet("select investor_code from client_master where client_code=" & MySourceId & "")
        ClientCat = SqlRet("select category_id from client_master where client_code=" & MySourceId & "")
        'investor  category
        For i = 0 To cmbinvestor.ListCount - 1
            INVESTORCODE = Split(cmbinvestor.List(i), "#")
            If UCase(Trim(INVESTORCODE(1))) = UCase(Trim(MyInvCode)) Then
                cmbinvestor.ListIndex = i
                Exit For
            End If
        Next
        'Client  category
        For i = 0 To cmbacatg.ListCount - 1
            ClienCode = Split(cmbacatg.List(i), "#")
            If UCase(Trim(ClienCode(1))) = UCase(Trim(ClientCat)) Then
                cmbacatg.ListIndex = i
                Exit For
            End If
        Next
    
        txtPin.Text = IIf(IsNull(Rs_view.Fields("pincode")), "", Rs_view.Fields("pincode"))
        CommMode = IIf(IsNull(Rs_view.Fields("comm_id")), 0, Rs_view.Fields("comm_id"))
        If RsCommM.State = 1 Then RsCommM.Close
        RsCommM.open "select * from fixeditem where itemtype=20 and itemserialnumber=" & CommMode & "", MyConn, adOpenForwardOnly
        If Not RsCommM.EOF Then
            cmbprefcommmode.Text = IIf(IsNull(RsCommM.Fields("itemname")), "", RsCommM.Fields("itemname"))
            cmbprefcommmode.Tag = IIf(IsNull(RsCommM.Fields("ITEMSERIALNUMBER")), 0, RsCommM.Fields("ITEMSERIALNUMBER"))
            RsCommM.Close
        End If
        CommInterval = IIf(IsNull(Rs_view.Fields("comm_inter_id")), 0, Rs_view.Fields("comm_inter_id"))
        RsCommI.open "select * from fixeditem where itemtype=14 and itemserialnumber=" & CommInterval & "", MyConn, adOpenForwardOnly
        If Not RsCommI.EOF Then
            cmbcomminterval.Text = IIf(IsNull(RsCommI.Fields("itemname")), "", RsCommI.Fields("itemname"))
            cmbcomminterval.Tag = IIf(IsNull(RsCommI.Fields("ITEMSERIALNUMBER")), 0, RsCommI.Fields("ITEMSERIALNUMBER"))
        End If
        RsCommI.Close
        txtoverseasadd.Text = IIf(IsNull(Rs_view.Fields("overseas_add")), "", Rs_view.Fields("overseas_add"))
        txtSTD.Text = IIf(IsNull(Rs_view.Fields("std1")), "", Rs_view.Fields("std1"))
        txtstd1.Text = IIf(IsNull(Rs_view.Fields("std2")), "", Rs_view.Fields("std2"))
        TxtTel1 = IIf(IsNull(Rs_view.Fields("tel1")), "", Rs_view.Fields("tel1"))
        TxtTel2.Text = IIf(IsNull(Rs_view.Fields("tel2")), "", Rs_view.Fields("tel2"))
        txtMobile.Text = IIf(IsNull(Rs_view.Fields("Mobile_no")), "", Rs_view.Fields("Mobile_no"))
        txtFax.Text = IIf(IsNull(Rs_view.Fields("fax")), "", Rs_view.Fields("fax"))
        txtEmail = Rs_view.Fields("Email")
    
        ActSt = IIf(IsNull(Rs_view.Fields("tele_Act_statement")), "", Rs_view.Fields("tele_Act_statement"))
        If ActSt = "YES" Then
            chkactst.Value = 1
        Else
            chkactst.Value = 0
        End If
        News = IIf(IsNull(Rs_view.Fields("tele_newsletter")), "", Rs_view.Fields("tele_newsletter"))
        If News = "YES" Then
            chknews.Value = 1
        Else
            chknews.Value = 0
        End If
        sInfo = IIf(IsNull(Rs_view.Fields("tele_statutory_info")), "", Rs_view.Fields("tele_statutory_info"))
        If sInfo = "YES" Then
            chksi.Value = 1
        Else
            chksi.Value = 0
        End If
        NIPan = IIf(IsNull(Rs_view.Fields("de_ni_pan")), "", Rs_view.Fields("de_ni_pan"))
        If NIPan = "YES" Then
            chknipan.Value = 1
        Else
            chknipan.Value = 0
        End If
        NIAdd = IIf(IsNull(Rs_view.Fields("de_ni_addproof")), "", Rs_view.Fields("de_ni_addproof"))
        If NIAdd = "YES" Then
            chkniadd.Value = 1
        Else
            chkniadd.Value = 0
        End If
        NIPower = IIf(IsNull(Rs_view.Fields("de_ni_power")), "", Rs_view.Fields("de_ni_power"))
        If NIPower = "YES" Then
            chknipower.Value = 1
        Else
            chknipower.Value = 0
        End If
        IPan = IIf(IsNull(Rs_view.Fields("de_i_pan")), "", Rs_view.Fields("de_i_pan"))
        If IPan = "YES" Then
            chkipan.Value = 1
        Else
            chkipan.Value = 0
        End If
        IAdd = IIf(IsNull(Rs_view.Fields("de_i_add_proof")), "", Rs_view.Fields("de_i_add_proof"))
        If IAdd = "YES" Then
            chkiadd.Value = 1
        Else
            chkiadd.Value = 0
        End If
        txtlstname = IIf(IsNull(Rs_view.Fields("client_lastname")), "", Rs_view.Fields("client_lastname"))
        'DisplayImage (txtclientcode.Text)
        'DisplayImage1 (txtclientcode.Text)
        Rs_view.Close
        i = 1
        If UCase(Trim(cmbStatus.Text)) = UCase(Trim("Others")) Then
        txtAcOther.Enabled = True
    Else
        txtAcOther.Enabled = False
    End If
    'RsGridFill.open "select a.client_name,a.client_codekyc,a.client_pan,a.dob,a.inv_kyc,a.g_name,a.g_pan,b.relation_name,b.relation_id,c.occ_name,c.occ_id from client_test a,relationship_master b,occupation_master c where main_code='" & txtclientcode.Text & "' and client_code<>'" & txtclientcode.Text & "' and a.relation_id=b.relation_id(+) and a.occ_id=c.occ_id(+) ", myconn, adOpenForwardOnly
    RsGridFill.open "select a.client_name,a.client_codekyc,a.client_pan,a.dob,a.inv_kyc,a.g_name,a.g_pan,b.relation_name,b.relation_id,c.occ_name,c.occ_id,title from client_test a,relationship_master b,occupation_master c where main_code=(SELECT MAIN_CODE FROM CLIENT_TEST WHERE CLIENT_CODE='" & txtclientcode.Text & "') and client_code<>MAIN_CODE   and a.relation_id=b.relation_id(+) and a.occ_id=c.occ_id(+) ", MyConn, adOpenForwardOnly
    i = 1
    If Not RsGridFill.EOF Then
        Do While Not RsGridFill.EOF
            VSFCommGrdE.TextMatrix(i, 0) = RsGridFill.Fields("client_name") & Space(50) & "#" & RsGridFill.Fields("client_codekyc")
            VSFCommGrdE.TextMatrix(i, 1) = RsGridFill.Fields("client_codekyc")
            VSFCommGrdE.TextMatrix(i, 2) = RsGridFill.Fields("client_name")
            VSFCommGrdE.TextMatrix(i, 3) = IIf(IsNull(RsGridFill.Fields("client_pan")), "", RsGridFill.Fields("client_pan"))
            VSFCommGrdE.TextMatrix(i, 4) = RsGridFill.Fields("dob")
            VSFCommGrdE.TextMatrix(i, 5) = RsGridFill.Fields("relation_name") & Space(50) & "#" & RsGridFill.Fields("relation_id")
            VSFCommGrdE.TextMatrix(i, 7) = RsGridFill.Fields("occ_name") & Space(50) & "#" & RsGridFill.Fields("occ_id")
            VSFCommGrdE.TextMatrix(i, 9) = IIf(IsNull(RsGridFill.Fields("inv_kyc")), "", RsGridFill.Fields("inv_kyc"))
            VSFCommGrdE.TextMatrix(i, 10) = IIf(IsNull(RsGridFill.Fields("g_name")), "", RsGridFill.Fields("g_name"))
            VSFCommGrdE.TextMatrix(i, 11) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
            VSFCommGrdE.TextMatrix(i, 13) = IIf(IsNull(RsGridFill.Fields("title")), "", RsGridFill.Fields("title"))
            RsGridFill.MoveNext
            i = i + 1
        Loop
    Else
        VSFCommGrdE.TextMatrix(i, 0) = ""
        VSFCommGrdE.TextMatrix(i, 1) = ""
        VSFCommGrdE.TextMatrix(i, 2) = ""
        VSFCommGrdE.TextMatrix(i, 3) = ""
        VSFCommGrdE.TextMatrix(i, 4) = ""
        VSFCommGrdE.TextMatrix(i, 5) = ""
        VSFCommGrdE.TextMatrix(i, 7) = ""
        VSFCommGrdE.TextMatrix(i, 9) = ""
        VSFCommGrdE.TextMatrix(i, 10) = ""
        VSFCommGrdE.TextMatrix(i, 11) = ""
        VSFCommGrdE.TextMatrix(i, 13) = ""
    End If
    RsGridFill.Close
    
    
    '---------------------------------------------------------------------------------------
    If MyUpdateInvestor <> "" Then
        StrInv = ""
        If rs_inv.State = 1 Then rs_inv.Close
        rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(MyUpdateInvestor, 8) & "' AND INV_CODE<>'" & MyUpdateInvestor & "' order by inv_code", MyConn, adOpenForwardOnly
        If Not rs_inv.EOF Then
           Do While Not rs_inv.EOF
               StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!INV_CODE & "|"
               rs_inv.MoveNext
           Loop
        End If
        rs_inv.Close
    End If
    VSFCommGrdE.ColComboList(0) = ""
    VSFCommGrdE.ColComboList(0) = StrInv
    '---------------------------------------------------------------------------------------------
    ''''''''''''''''''''''''''''''''''''''To Fill Investor For New Grid----------------------------
    If MyUpdateInvestor <> "" Then
        StrInv = ""
        If rs_inv.State = 1 Then rs_inv.Close
        rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(MyUpdateInvestor, 8) & "' AND INV_CODE<>'" & MyUpdateInvestor & "' and kyc is null order by inv_code", MyConn, adOpenForwardOnly
        If Not rs_inv.EOF Then
           Do While Not rs_inv.EOF
               StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!INV_CODE & "|"
               rs_inv.MoveNext
           Loop
        End If
        rs_inv.Close
    End If
    VSFCommGrdE1.ColComboList(0) = ""
    VSFCommGrdE1.ColComboList(0) = StrInv
    '-------------------------------------------------------------------------------
    If UCase(Trim(cmbStatus.Text)) = UCase(Trim("individual")) Then
        chkiadd.Enabled = True
        chkipan.Enabled = True
        chknipan.Enabled = False
        chkniadd.Enabled = False
        chknipower.Enabled = False
    Else
        chknipan.Enabled = True
        chkniadd.Enabled = True
        chknipower.Enabled = True
        chkiadd.Enabled = False
        chkipan.Enabled = False
    End If

Else
    MsgBox "The Requested Record Doesn't Exist", vbInformation
    Call CMDRESET_Click
End If
If GlbroleId <> 17 And GlbroleId <> 1 Then
    txtbusicode.Enabled = False
End If
Picture1.Enabled = False
End Sub
Private Sub DisplayImage(X As String)
'On Error Resume next
Dim RS As New ADODB.Recordset
Dim B() As Byte
'Dim C() As Byte
    RS.open "select * from client_test Where client_code = '" & txtclientcode.Text & "'", MyConn, adOpenForwardOnly
    B = IIf(IsNull(RS.Fields("photograph")), "", RS.Fields("photograph"))
    imgp.Picture = PictureFromBits(B)
    'C = RS.Fields("pan_image")
    'imgpan.Picture = PictureFromBits(C)
    RS.Close
    Set RS = Nothing
End Sub
Private Sub DisplayImage1(X As String)
'On Error Resume next
Dim RS As New ADODB.Recordset
Dim B() As Byte
    RS.open "select * from client_test Where client_code = '" & txtclientcode.Text & "'", MyConn, adOpenForwardOnly
    B = IIf(IsNull(RS.Fields("pan_image")), "", RS.Fields("pan_image"))
    imgpan.Picture = PictureFromBits(B)
    RS.Close
    Set RS = Nothing
End Sub
Private Sub Command4_Click()
Unload Me
End Sub

Private Sub cmdPrev_Click()
SSTab1.Tab = 0
End Sub

Private Sub cmdnext_Click()
'Date Validation
If DOB.Text <> "__/__/____" Then
    If DateDiff("YYYY", Format(DOB.Text, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
        frameguardian.Enabled = True
    Else
        frameguardian.Enabled = False
    End If
End If
'If SqlRet("select nvl(count(*),0) from investor_master where upper(pan)='" & UCase(txtguardianpan.Text) & "' ") > 0 Then
'    MsgBox "This PanNo Is Already Exist"
'    txtguardianpan.SetFocus
'    SSTab1.Tab = 0
'    Exit Sub
'End If
If frameguardian.Enabled = True Then
    If txtguardianpan.Text <> "" Then
        If ValidatePan(txtguardianpan.Text) = False Then
           MsgBox "Plaes Enter Valid Pan No. of Guardian"
           SSTab1.Tab = 0
           txtguardianpan.SetFocus
           Exit Sub
        End If
    End If
End If
If IsDate(DOB.Text) = False Then
    MsgBox "Please Enter Valid Date", vbInformation
    DOB.SetFocus
    SSTab1.Tab = 0
    Exit Sub
End If
If DateDiff("D", Format(DOB.Text, "DD/MM/YYYY"), ServerDateTime) < 0 Then
    MsgBox "Date Of Birth Can Not be grater than Current Year", vbInformation
    DOB.SetFocus
    SSTab1.Tab = 0
    Exit Sub
End If
If txtclientname.Text = "" Then
    MsgBox "Client Name Can Not Left blank"
    txtclientname.SetFocus
    Exit Sub
End If
If cmboccupation.Text = "" Then
    MsgBox "Occupation Can Not Left blank"
    cmboccupation.SetFocus
    Exit Sub
End If
If Len(txtbusicode.Text) <> 5 Then
    MsgBox "please enter 5 digit Business Code"
    txtbusicode.SetFocus
    Exit Sub
End If
If frameguardian.Enabled = False Then
    If txtclientpan.Text <> "" Then
        If ValidatePan(txtclientpan.Text) = False Then
           MsgBox "Please Enter Valid Pan No"
           SSTab1.Tab = 0
           txtclientpan.SetFocus
           Exit Sub
        End If
    End If
End If
If frameguardian.Enabled = True Then
    If txtguardianpan.Text <> "" Then
        If ValidatePan(txtguardianpan.Text) = False Then
           MsgBox "Plaes Enter Valid Pan No of guardian"
           SSTab1.Tab = 0
           txtguardianpan.SetFocus
           Exit Sub
        End If
    End If
End If
If DOB.Text = "__/__/____" Then
   MsgBox "Date Of Birth Can Not Be Left Blank"
   SSTab1.Tab = 0
   DOB.SetFocus
   Exit Sub
End If
If IsDate(DOB.Text) = False Then
    MsgBox "Please Enter Valid Date"
    SSTab1.Tab = 0
    DOB.SetFocus
    Exit Sub
End If
If cmbresinri.Text = "" Then
   MsgBox "Resident/NRI Can Not Left Blank"
   SSTab1.Tab = 0
   cmbresinri.SetFocus
   Exit Sub
End If
If cmbCity.Text = "" Then
   MsgBox "City Can Not Left Blank"
   SSTab1.Tab = 0
   cmbCity.SetFocus
   Exit Sub
End If
If cmbstate.Text = "" Then
   MsgBox "State Can Not Left Blank"
   SSTab1.Tab = 0
   cmbstate.SetFocus
   Exit Sub
End If
If txtEmail.Text = "" Then
    MsgBox "Email can not Left Balnk"
    Exit Sub
End If
If txtEmail.Text <> "" Then
    If UCase(txtEmail.Text) = "NOT AVAILABLE" Then
    Else
        If ValidateEmail(txtEmail.Text) = False Then
            MsgBox "Please enter Valid Email Address"
            SSTab1.SetFocus
            txtEmail.SetFocus
            Exit Sub
        End If
    End If
Else
    MsgBox "Please Enter Either Email Address Or Enter Not Available"
End If
'If txtp.Text = "" Then
'    If MsgBox("Do You Want to Upload Your Photograph", vbYesNo) = vbYes Then
'    Call Lblphoto_Click
'    End If
'End If
'If txtpan.Text = "" Then
'    If MsgBox("Do You Want to Upload Your PAN Card", vbYesNo) = vbYes Then
'    Call lblpana_Click
'    End If
'End If
If txtSTD.Text = "" And txtstd1.Text = "" And TxtTel1.Text = "" And TxtTel2.Text = "" And txtMobile.Text = "" Then
    MsgBox "Enter Atleast One of Your Contact Number", vbInformation
    SSTab1.Tab = 0
    txtMobile.SetFocus
    Exit Sub
End If
If Trim(txtaddress1.Text) = "" Then
    V = MsgBox("Please enter address", vbInformation, "Client Address")
    txtaddress1.SetFocus
    'validateData = False
    Exit Sub
End If
If Len(Trim(txtaddress1.Text) & Trim(txtaddress2.Text)) < 10 Then
    MsgBox "Please Enter Address with Minimum 10 Characters", vbInformation
    txtaddress1.SetFocus
    'validateData = False
    Exit Sub
End If

If Len(Trim(txtaddress1.Text)) > 45 Then
    MsgBox "First Part of Address Cannot be Greater than 45 Characters", vbInformation
    txtaddress1.SetFocus
    Exit Sub
End If
    
SSTab1.Tab = 1
End Sub


Private Sub Form_Activate()
    Me.WindowState = vbMaximized
End Sub
Private Sub Form_Load()
If GlbroleId = 1 Or GlbroleId = 17 Or GlbroleId = 54 Or GlbroleId = 22 Then 'Or Glbloginid = 96358
    cmdnewocc.Enabled = True
    cmdapprove.Enabled = True
Else
    cmdnewocc.Enabled = False
    cmdapprove.Enabled = False
End If


FillClientCate
FillInvestorCat
cmbacatg.ListIndex = 4
If sql_ora = "SQL" Then
    oc = "select distinct OCC_ID,OCC_NAME from occupation_master where occ_id is not null"
Else
    oc = "select distinct occ_id,occ_name from occupation_master where occ_id is not null "
End If
Set RSV = MyConn.Execute(oc)
If Not RSV.EOF Then
    Do While Not RSV.EOF
        cmboccupation.AddItem RSV(1) & Space(100) & "#" & RSV(0)
        RSV.MoveNext
    Loop
End If
RSV.Close


SSTab1.Tab = 0
SSTab1.TabEnabled(0) = False
frameguardian.Enabled = False
cmbincome.AddItem ""
cmbincome.AddItem "1 Lac", 0
cmbincome.AddItem "1-5 Lac", 1
cmbincome.AddItem "5-10 Lac", 2
cmbincome.AddItem "10-25 Lac", 3
cmbincome.AddItem "25 Lac and Above", 4

cmbinvestment.AddItem ""
cmbinvestment.AddItem "1-5 yrs", 0
cmbinvestment.AddItem "6-15 yrs", 1
cmbinvestment.AddItem "16-20 yrs", 2
cmbinvestment.AddItem "21-30 yrs", 3
cmbinvestment.AddItem "30 yrs and Above", 4

'cmbstatus.AddItem "Individual", 0
'cmbstatus.AddItem "HUF", 1
cmbStatus.AddItem "Minor Through Guardian", 0
cmbStatus.AddItem "Society/Club", 1
cmbStatus.AddItem "NRI", 2
cmbStatus.AddItem "AOP", 3
cmbStatus.AddItem "BOI", 4
cmbStatus.AddItem "Partnership", 5
'cmbstatus.AddItem "Company", 8
cmbStatus.AddItem "Body Corporate", 6
cmbStatus.AddItem "Others", 7
cmbStatus.AddItem ""
cmbStatus.ListIndex = 7

cmboccupation.AddItem ""


cmbclientn.AddItem "RESIDENT"
cmbclientn.AddItem "INDIAN"
cmbclientn.AddItem "NRI"
cmbclientn.AddItem "NON NRI"
cmbclientn.ListIndex = 1

cmbgender.AddItem "Male", 0
cmbgender.AddItem "Female", 1

cmbkyc.AddItem "Yes", 0
cmbkyc.AddItem "NO", 1

cmbkyc1.AddItem "Yes", 0
cmbkyc1.AddItem "NO", 1

cmbresinri.AddItem "Resident", 0
cmbresinri.AddItem "NRI", 1
cmbresinri.ListIndex = 0

cmbtitle.Clear
cmbtitle.AddItem "Mr."
cmbtitle.AddItem "Mrs."
cmbtitle.AddItem "Ms."
cmbtitle.AddItem "M/S."
cmbtitle.AddItem "Dr."
cmbtitle.AddItem "Adv."
cmbtitle.AddItem "Col."
cmbtitle.AddItem "Maj."
cmbtitle.AddItem "Gen."
cmbtitle.AddItem "Brig."
cmbtitle.AddItem "CDR."
cmbtitle.AddItem "SQN LDR"
cmbtitle.AddItem "CAPT."
cmbtitle.AddItem "LT. COL."
cmbtitle.AddItem "LT."
cmbtitle.AddItem "Wg.Cdr."
cmbtitle.AddItem "Maj. Gen."
cmbtitle.AddItem "Master"
cmbtitle.AddItem "Others"



   

If sql_ora = "SQL" Then
    st = "select distinct STATE_ID,STATE_NAME from state_master where state_id is not null order by state_name"
Else
    st = "select distinct state_id,state_name from state_master where state_id is not null order by state_name"
End If
Set RSV = MyConn.Execute(st)
If Not RSV.EOF Then
    Do While Not RSV.EOF
        cmbstate.AddItem RSV(1) & Space(100) & "#" & RSV(0)
        RSV.MoveNext
    Loop
End If
RSV.Close

If sql_ora = "SQL" Then
    st1 = "select distinct city_ID,CITY_NAME from city_master where (city_id is not null or city_id<>'') order by city_name"
Else
    st1 = "select distinct city_id,city_name from city_master where city_id is not null order by city_name"
End If
Set RSV1 = MyConn.Execute(st1)
If Not RSV1.EOF Then
    Do While Not RSV1.EOF
        cmbCity.AddItem RSV1(1) & Space(100) & "#" & RSV1(0)
        RSV1.MoveNext
    Loop
End If
RSV1.Close


    
Set RSV = MyConn.Execute("select * from FIXEDITEM where itemtype=20 ORDER BY ITEMNAME")
cmbprefcommmode.Clear
While Not RSV.EOF
    cmbprefcommmode.AddItem RSV("ITEMNAME")
    cmbprefcommmode.Tag = RSV("ITEMSERIALNUMBER")
    RSV.MoveNext
Wend
RSV.Close

Set RSV = MyConn.Execute("select * from FIXEDITEM where itemtype=14 ORDER BY ITEMNAME")
cmbcomminterval.Clear
While Not RSV.EOF
    cmbcomminterval.AddItem RSV("ITEMNAME")
    cmbcomminterval.Tag = RSV("ITEMSERIALNUMBER")
    RSV.MoveNext
Wend
RSV.Close
Call SetGrid
Call SetGrid1
End Sub

Private Sub FillOccupation(cmb As ComboBox)
    On Error GoTo BOError
    Dim Get_occ As New ADODB.Recordset
    Set Get_occ = MyConn.Execute("select * from occupation_master order by occ_name")
   If Not Get_occ.EOF Then
        cmb.Clear
        cmb.AddItem ""
        Do While Not Get_occ.EOF
            cmb.AddItem Get_occ("occ_name") & Space(40) & "#" & Get_occ("occ_id")
            'cmb.Tag = Get_occ("occ_id")
            Get_occ.MoveNext
        Loop
    End If
    Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
    Exit Sub
End Sub

Private Sub Lblphoto_Click()
txtp.Text = ""
cd1.ShowOpen
imgp.Picture = LoadPicture(cd1.Filename)
'txtp.Text = cd1.Filename
'If txtp.Text <> "" Then
'    Call AddImage(txtp.Text)
'Else
'    MsgBox "Upload Photograph"
'    Exit Sub
'End If
End Sub
Public Sub AddImage(sImage As String, sImage1 As String)
Dim P_RS As New ADODB.Recordset
On Error Resume Next
Dim B() As Byte
Dim c() As Byte
Dim lid As Long
Open sImage For Binary As #1
Open sImage1 For Binary As #2
ReDim B(1 To LOF(1))
ReDim c(2 To LOF(2))
Get 1, , B
Get 2, , c
'c = B
    'myconn.Execute "insert into  agent_pan_request (agent_code,pan,user_id,timest,pan_image,status) values(" & MyAgentCode1 & ",  '" & txtcrpan.Text & "'," & Glbloginid & " , '" & Format(Now, "dd-mmm-yyyy") & "', B ,'N')"
    P_RS.open "select * from client_test where 1<>1", MyConn, adOpenStatic, adLockOptimistic
'    P_RS!photograph = B
'    P_RS.Update
    P_RS.AddNew
    'P_RS!relationflag = MyExistRelFlag
    P_RS!clientcode_old = Val(txtclientcodeold.Text)
    P_RS!tradingid = txttradingid.Text
    P_RS!benefi_id = txtdp_bid.Text
    P_RS!status = cmbStatus.Text
    P_RS!Other = txtAcOther.Text
    P_RS!Occ_ID = MyOccCode
    P_RS!others1 = TxtAcOther1.Text
    P_RS!TITLE = cmbtitle.Text
    P_RS!client_name = ProperCase(cmbclientname.Text)
    P_RS!nationality = ProperCase(cmbclientn.Text)
    P_RS!client_pan = txtclientpan.Text
    P_RS!annual_income = cmbincome.Text
    P_RS!investement = cmbinvestment.Text
    P_RS!title_other = txttitle.Text
    P_RS!resident_nri = cmbresinri.Text
    P_RS!DOB = Format(DOB.Text, "DD-MMM-YYYY")
    P_RS!GENDER = cmbgender.Text
    P_RS!a_enclosed_pan = StrEnclosedPanA
    P_RS!a_enclosed_60 = StrEnclosed60A
    P_RS!a_enclosed_61 = StrEnclosed61A
    P_RS!G_enclosed_pan = StrEnclosedPanG
    P_RS!g_enclosed_60 = StrEnclosed60G
    P_RS!g_enclosed_61 = StrEnclosed61G
    P_RS!a_kyc_status = cmbkyc.Text
    P_RS!g_kyc_status = cmbkyc1.Text
    P_RS!g_name = ProperCase(txtguardianname.Text)
    P_RS!g_nationality = ProperCase(txtguardiann.Text)
    P_RS!g_pan = txtguardianpan.Text
    P_RS!add1 = txtaddress1
    P_RS!add2 = Replace(txtaddress2, ",", "")
    P_RS!City_id = MyCityCode
    P_RS!state_id = MyStateCode
    P_RS!pincode = txtPin.Text
    P_RS!Comm_Id = CommMode
    P_RS!comm_inter_id = CommInterval
    P_RS!overseas_add = txtoverseasadd.Text
    P_RS!std1 = Val(txtSTD.Text)
    P_RS!std2 = Val(txtstd1.Text)
    P_RS!tel1 = Val(TxtTel1.Text)
    P_RS!tel2 = Val(TxtTel2.Text)
    P_RS!mobile_no = txtMobile.Text
    P_RS!fax = Val(txtFax.Text)
    P_RS!Email = txtEmail.Text
    P_RS!tele_Act_statement = ActSt
    P_RS!tele_newsletter = News
    P_RS!tele_statutory_info = sInfo
    P_RS!de_ni_pan = NIPan
    P_RS!de_ni_addproof = NIAdd
    P_RS!de_ni_power = NIPower
    P_RS!de_i_pan = IPan
    P_RS!de_i_add_proof = IAdd
    P_RS!client_lastname = txtlstname.Text
    P_RS!photograph = B
    P_RS!pan_image = c
    P_RS!loggeduserid = Glbloginid
    P_RS!business_code = txtbusicode
    P_RS!timest = Format(ServerDateTime, "DD-MMM-YYYY")
    P_RS!branch_code = MyBranch
    P_RS.Update
    P_RS.Close
    Set RS = Nothing
    Close 1
    Close 2
End Sub



Private Sub lblpana_Click()
txtPAN.Text = ""
cd1.ShowOpen
imgpan.Picture = LoadPicture(cd1.Filename)
End Sub
Private Sub lblpang_Click()
Call lblpana_Click
End Sub
Private Sub Picture1_Click()
 treeselected = "N"
    Set frmtree_searchnew.currentForm = Nothing
    Set frmtree_searchnew.currentForm = frmactopen
    'frmtree_searchnew.treeName = "Treeclient"
    frmtree_searchnew.Cmbcat.Text = "INVESTOR"    'BY HIMANSHU
    frmtree_searchnew.Cmbcat.Enabled = False
    frmtree_searchnew.Caption = "Existing Investor Search"
    frmtree_searchnew.lblOF.Visible = True
    frmtree_searchnew.CmbClientBroker.Visible = True
    frmtree_searchnew.CmbClientBroker.ListIndex = 0
    frmtree_searchnew.Label7.Enabled = True
    frmtree_searchnew.txtCliSubName.Enabled = True
    'frmtree_search.chkIndia.Visible = True
    frmtree_searchnew.Show
    frmtree_searchnew.cmbBranch.SetFocus
    frmtree_searchnew.ZOrder
End Sub




Private Sub Picture2_Click()
treeselected = "N"
Set frmtreeMf.currentForm = Nothing
Set frmtreeMf.currentForm = frmactopeninv
frmtreeMf.treeName = "Treeclient"
Load frmtreeMf
frmtreeMf.Ar_type = ""
frmtreeMf.Cmbcat.ListIndex = 0
frmtreeMf.Combo2.ListIndex = 0
frmtreeMf.Cmbcat.Enabled = False
frmtreeMf.CmbClientBroker.Visible = True
frmtreeMf.CmbClientBroker.ListIndex = 0
frmtreeMf.lblOF.Visible = True
frmtreeMf.Show vbModal
End Sub


Private Sub Picture3_Click()
 treeselected = "N"
    Set frmtree_searchnew.currentForm = Nothing
    Set frmtree_searchnew.currentForm = frmactopen
    'frmtree_searchnew.treeName = "Treeclient"
    frmtree_searchnew.Cmbcat.Text = "INVESTOR"    'BY HIMANSHU
    frmtree_searchnew.Cmbcat.Enabled = False
    frmtree_searchnew.Caption = "Existing Investor Search"
    frmtree_searchnew.lblOF.Visible = True
    frmtree_searchnew.CmbClientBroker.Visible = True
    frmtree_searchnew.CmbClientBroker.ListIndex = 0
    frmtree_searchnew.Label7.Enabled = True
    frmtree_searchnew.txtCliSubName.Enabled = True
    'frmtree_search.chkIndia.Visible = True
    frmtree_searchnew.Show
    frmtree_searchnew.cmbBranch.SetFocus
    frmtree_searchnew.ZOrder
End Sub

Private Sub txtaddress1_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtaddress2_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtbusicode_GotFocus()
'Call cmdSearch_Click
End Sub

Private Sub txtbusicode_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
   KeyAscii = 0
End If
If KeyAscii = 13 Then
  SendKeys "{TAB}"
End If
End Sub

Public Sub txtbusicode_KeyUp(KeyCode As Integer, Shift As Integer)
Dim rsEmp As ADODB.Recordset
Dim b_cd() As String
MyBranch = ""
MyRmCode = ""
    If Len(txtbusicode.Text) = 5 Then
        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.category_id in('2001','2018')", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            MyBranch = rsEmp(0)
            MyRmCode = rsEmp(3)
            Label32.Caption = rsEmp(1)
            Label55.Caption = rsEmp(2)
        Else
            MsgBox "Rm Should Be Sale Support And Marketing "
            Label32.Caption = ""
            Label55.Caption = ""
            txtbusicode = ""
            Exit Sub
        End If
        rsEmp.Close
        Set rsEmp = Nothing
    End If
End Sub

Private Sub cmbclientname_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub



Private Sub txtclientcode_KeyDown(KeyCode As Integer, Shift As Integer)
If KeyCode = 13 Then
    Call cmdview_Click
End If
End Sub


Private Sub txtclientname_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtclientpan_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtFax_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
    MsgBox "Enter Digits only"
End If
End Sub

Private Sub txtguardiann_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub



Private Sub txtguardianname_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtguardianpan_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtlstname_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtMobile_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
End If
End Sub

Private Sub txtoverseasadd_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtpans_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtPin_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
End If
End Sub

Private Sub txtStd_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And keascii <> 8 Then
    KeyAscii = 0
    MsgBox "Enter Digits only"
End If
End Sub

Private Sub txtStd1_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
    MsgBox "Enter Digits only"
End If
End Sub
Private Sub TxtTel1_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
    MsgBox "Enter Digits only"
End If
End Sub
Private Sub TxtTel2_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
    MsgBox "Enter Digits only"
End If
End Sub

Private Sub txttitle_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub VSFCommGrd_KeyPress(KeyAscii As Integer)
If VSFCommGrd.Col = 2 Then
    KeyAscii = Asc(UCase(Chr(KeyAscii)))
End If
End Sub
Private Sub VSFCommGrd_KeyPressEdit(ByVal Row As Long, ByVal Col As Long, KeyAscii As Integer)
If Col = 2 Then
    KeyAscii = Asc(UCase(Chr(KeyAscii)))
End If
If Col = 1 Then
    KeyAscii = Asc(UCase(Chr(KeyAscii)))
End If
If Col = 7 Then
    KeyAscii = Asc(UCase(Chr(KeyAscii)))
End If
End Sub

Private Sub SetGrid()
Dim StrRel As String
Dim StrOcc As String
Dim Strtit As String
s = " S.No.|^Investor Name|^ PAN |^ DOB|^ Relation|^ Occupation |^ Guardian Name|^Guardian PAN|^Kyc|^Title "
StrInv = ""
If Len(txtclientcodeold.Text) >= 11 Then
    If rs_inv.State = 1 Then rs_inv.Close
    rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(txtclientcodeold, 8) & "' AND INV_CODE<>'" & txtclientcodeold.Text & "' order by inv_code", MyConn, adOpenForwardOnly
    If Not rs_inv.EOF Then
       Do While Not rs_inv.EOF
           StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!INV_CODE & "|"
           rs_inv.MoveNext
       Loop
    End If
    rs_inv.Close
End If
StrRel = ""
If Rs_Rel.State = 1 Then Rs_Rel.Close
Rs_Rel.open "select relation_id, relation_name from relationship_master", MyConn, adOpenForwardOnly
If Not Rs_Rel.EOF Then
   Do While Not Rs_Rel.EOF
       StrRel = StrRel & Rs_Rel!Relation_name & Space(50) & "#" & Rs_Rel!Relation_id & "|"
       Rs_Rel.MoveNext
   Loop
End If
Rs_Rel.Close
StrOcc = ""
If rs_Occ.State = 1 Then rs_Occ.Close
rs_Occ.open "select occ_name,occ_id from occupation_master", MyConn, adOpenForwardOnly
If Not rs_Occ.EOF Then
    Do While Not rs_Occ.EOF
        StrOcc = StrOcc & rs_Occ!occ_name & Space(50) & "#" & rs_Occ!Occ_ID & "|"
        rs_Occ.MoveNext
    Loop
End If
rs_Occ.Close



Strtit = ""
If rs_tit.State = 1 Then rs_tit.Close
rs_tit.open "select investor_title from investor_master where source_id = '" & Left(txtclientcodeold, 8) & "' AND INV_CODE<>'" & txtclientcodeold.Text & "' order by inv_code", MyConn, adOpenForwardOnly
If Not rs_tit.EOF Then
    Do While Not rs_tit.EOF
        Strtit = Strtit & rs_tit!investor_title & Space(50) & "#" & "|"
        rs_titOcc.MoveNext
    Loop
End If
rs_tit.Close



i = 1
VSFCommGrdE.Clear
cmbYesNo = "YES|NO"
 s1 = "Existing Investor|^InvestorCode|^Investor Name|^PAN|^DOB|^Relation|^RelationId|^Occupation|^OccId|^KYC|^Gaurdian Name|^Gaurdian PAN|^G KYC|^Title"
 VSFCommGrdE.FormatString = s1
 VSFCommGrdE.ColWidth(0) = 2500 'ExistingInvestor
 VSFCommGrdE.ColWidth(1) = 0 'InvCode
 VSFCommGrdE.ColWidth(2) = 2000 'InvName
 VSFCommGrdE.ColWidth(3) = 1500 'Pan
 VSFCommGrdE.ColWidth(4) = 1500 'DOB
 VSFCommGrdE.ColWidth(5) = 1500 'Relation
 VSFCommGrdE.ColWidth(6) = 0 'Relationid
 VSFCommGrdE.ColWidth(7) = 2000 'Occupation
 VSFCommGrdE.ColWidth(8) = 0 'Occupationid
 VSFCommGrdE.ColWidth(9) = 800 'kyc
 VSFCommGrdE.ColWidth(10) = 2000 'GNAMEME
 VSFCommGrdE.ColWidth(11) = 1500 'GPAN
 VSFCommGrdE.ColWidth(12) = 2000 'KYCFLAG
 VSFCommGrdE.ColWidth(12) = 6
 VSFCommGrdE.ColEditMask(4) = "##/##/####"
 VSFCommGrdE.ColComboList(0) = ""
 VSFCommGrdE.ColComboList(0) = StrInv
 VSFCommGrdE.ColComboList(5) = ""
 VSFCommGrdE.ColComboList(5) = StrRel
 VSFCommGrdE.ColComboList(7) = ""
 VSFCommGrdE.ColComboList(7) = StrOcc
 VSFCommGrdE.ColComboList(12) = ""
 VSFCommGrdE.ColComboList(12) = cmbYesNo
 VSFCommGrdE.ColComboList(9) = cmbYesNo
 VSFCommGrdE.ColComboList(13) = Strtit
 
 
 VSFCommGrdE.ColAlignment(0) = flexAlignLeftCenter  'inv
 VSFCommGrdE.ColAlignment(1) = flexAlignLeftCenter  'sch
 VSFCommGrdE.ColAlignment(2) = flexAlignLeftCenter   'amt
 VSFCommGrdE.ColAlignment(3) = flexAlignLeftCenter   'chq
 VSFCommGrdE.ColAlignment(4) = flexAlignLeftCenter  'tran
 VSFCommGrdE.ColAlignment(5) = flexAlignLeftCenter  'folio
 VSFCommGrdE.ColAlignment(6) = flexAlignLeftCenter  'date
 VSFCommGrdE.ColAlignment(7) = flexAlignLeftCenter  'mar
 VSFCommGrdE.ColAlignment(8) = flexAlignLeftCenter  'expe
 VSFCommGrdE.ColAlignment(9) = flexAlignLeftCenter  'mar
 VSFCommGrdE.ColAlignment(10) = flexAlignLeftCenter  'expe
 VSFCommGrdE.ColAlignment(11) = flexAlignLeftCenter  'expe

End Sub
Private Sub SetGrid1()
Dim StrRel As String
Dim StrOcc As String
Dim Strtit As String
s = " S.No.|^Investor Name|^ PAN |^ DOB|^ Relation|^ Occupation |^ Guardian Name|^Guardian PAN|^Kyc "
StrInv = ""
If Len(txtclientcodeold.Text) >= 11 Then
    If rs_inv.State = 1 Then rs_inv.Close
    rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(txtclientcodeold, 8) & "' AND INV_CODE<>'" & txtclientcodeold.Text & "' order by inv_code", MyConn, adOpenForwardOnly
    If Not rs_inv.EOF Then
       Do While Not rs_inv.EOF
           StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!INV_CODE & "|"
           rs_inv.MoveNext
       Loop
    End If
    rs_inv.Close
End If
StrRel = ""
If Rs_Rel.State = 1 Then Rs_Rel.Close
Rs_Rel.open "select relation_id, relation_name from relationship_master", MyConn, adOpenForwardOnly
If Not Rs_Rel.EOF Then
   Do While Not Rs_Rel.EOF
       StrRel = StrRel & Rs_Rel!Relation_name & Space(50) & "#" & Rs_Rel!Relation_id & "|"
       Rs_Rel.MoveNext
   Loop
End If
Rs_Rel.Close
StrOcc = ""
If rs_Occ.State = 1 Then rs_Occ.Close
rs_Occ.open "select occ_name,occ_id from occupation_master", MyConn, adOpenForwardOnly
If Not rs_Occ.EOF Then
    Do While Not rs_Occ.EOF
        StrOcc = StrOcc & rs_Occ!occ_name & Space(50) & "#" & rs_Occ!Occ_ID & "|"
        rs_Occ.MoveNext
    Loop
End If
rs_Occ.Close

Strtit = ""
Strtit = Strtit & "Mr." & "|"
Strtit = Strtit & "Mrs." & "|"
Strtit = Strtit & "Ms." & "|"
Strtit = Strtit & "M/S." & "|"
Strtit = Strtit & "Dr." & "|"
Strtit = Strtit & "Adv." & "|"
Strtit = Strtit & "Col." & "|"
Strtit = Strtit & "Maj." & "|"
Strtit = Strtit & "Gen." & "|"
Strtit = Strtit & "Brig." & "|"
Strtit = Strtit & "CDR." & "|"
Strtit = Strtit & "SQN LDR" & "|"
Strtit = Strtit & "CAPT." & "|"
Strtit = Strtit & "LT. COL." & "|"
Strtit = Strtit & "LT." & "|"
Strtit = Strtit & "Wg.Cdr." & "|"
Strtit = Strtit & "Maj. Gen." & "|"
Strtit = Strtit & "Master" & "|"
Strtit = Strtit & "Others" & "|"



i = 1
VSFCommGrdE1.Clear
cmbYesNo = "YES|NO"
 s1 = "Existing Investor|^InvestorCode|^Investor Name|^PAN|^DOB|^Relation|^RelationId|^Occupation|^OccId|^KYC|^Gaurdian Name|^Gaurdian PAN|^G KYC|^Title"
 VSFCommGrdE1.FormatString = s1
 VSFCommGrdE1.ColWidth(0) = 2500 'ExistingInvestor
 VSFCommGrdE1.ColWidth(1) = 0 'InvCode
 VSFCommGrdE1.ColWidth(2) = 2000 'InvName
 VSFCommGrdE1.ColWidth(3) = 1500 'Pan
 VSFCommGrdE1.ColWidth(4) = 1500 'DOB
 VSFCommGrdE1.ColWidth(5) = 1500 'Relation
 VSFCommGrdE1.ColWidth(6) = 0 'Relationid
 VSFCommGrdE1.ColWidth(7) = 2000 'Occupation
 VSFCommGrdE1.ColWidth(8) = 0 'Occupationid
 VSFCommGrdE1.ColWidth(9) = 800 'kyc
 VSFCommGrdE1.ColWidth(10) = 2000 'GNAMEME
 VSFCommGrdE1.ColWidth(11) = 1500 'GPAN
 VSFCommGrdE1.ColWidth(12) = 2000 'KYCFLAG
 VSFCommGrdE1.ColWidth(12) = 6
 VSFCommGrdE1.ColEditMask(4) = "##/##/####"
 VSFCommGrdE1.ColComboList(0) = ""
 VSFCommGrdE1.ColComboList(0) = StrInv
 VSFCommGrdE1.ColComboList(5) = ""
 VSFCommGrdE1.ColComboList(5) = StrRel
 VSFCommGrdE1.ColComboList(7) = ""
 VSFCommGrdE1.ColComboList(7) = StrOcc
 VSFCommGrdE1.ColComboList(12) = ""
 VSFCommGrdE1.ColComboList(12) = cmbYesNo
 VSFCommGrdE1.ColComboList(9) = cmbYesNo
 VSFCommGrdE1.ColComboList(13) = Strtit
 
 
 VSFCommGrdE1.ColAlignment(0) = flexAlignLeftCenter  'inv
 VSFCommGrdE1.ColAlignment(1) = flexAlignLeftCenter  'sch
 VSFCommGrdE1.ColAlignment(2) = flexAlignLeftCenter   'amt
 VSFCommGrdE1.ColAlignment(3) = flexAlignLeftCenter   'chq
 VSFCommGrdE1.ColAlignment(4) = flexAlignLeftCenter  'tran
 VSFCommGrdE1.ColAlignment(5) = flexAlignLeftCenter  'folio
 VSFCommGrdE1.ColAlignment(6) = flexAlignLeftCenter  'date
 VSFCommGrdE1.ColAlignment(7) = flexAlignLeftCenter  'mar
 VSFCommGrdE1.ColAlignment(8) = flexAlignLeftCenter  'expe
 VSFCommGrdE1.ColAlignment(9) = flexAlignLeftCenter  'mar
 VSFCommGrdE1.ColAlignment(10) = flexAlignLeftCenter  'expe
 VSFCommGrdE1.ColAlignment(11) = flexAlignLeftCenter  'expe

End Sub

Private Sub gridfill()
On Error GoTo err1
'Sql = " SELECT GUARDIAN_NAME ,GUARDIAN_PAN,i.pan,i.dob,inv.kyc,client_name,client_code,i.inv_code, inv.investor_name, i.rel_id, relation_name, i.occupation_id, "
'Sql = Sql & "       occ_name "
'Sql = Sql & "  FROM investor_master i, relationship_master r, occupation_master o,client_master cl,investor_detail inv "
'Sql = Sql & " WHERE i.occupation_id = o.occ_id(+) AND i.rel_id = r.relation_id and substr(i.INV_CODE,1,8)=cl.CLIENT_CODE "
'Sql = Sql & " and i.inv_code=inv.INV_CODE(+) "
'Sql = Sql & " and client_code=" & val(Mid(txtclientcodeold.Text, 1, 8)) & " "
'Set rsMap1 = New ADODB.Recordset
'rsMap1.open Sql, myconn, adOpenForwardOnly, adLockOptimistic
'If .State = 1 Then .Close
'.open "select investor_name,inv_code from investor_master where inv_code= '" & txtclientcodeold & "'", myconn, adOpenForwardOnly
'If Not .EOF Then
'   Do While Not .EOF
'       StrInv = StrInv & !investor_name & Space(50) & "#" & !inv_Code & "|"
'       .MoveNext
'   Loop
'End If
'.Close
'Set  = Nothing
'i = 1
'VSFCommGrdE.Clear
'Call SetGrid
'VSFCommGrdE.Rows = rsMap1.RecordCount + 1
'While Not rsMap1.EOF
'    VSFCommGrdE.TextMatrix(i, 0) = IIf(IsNull(rsMap1("Inv_Code")), "", rsMap1("Inv_Code"))
'    VSFCommGrdE.TextMatrix(i, 1) = IIf(IsNull(rsMap1("Investor_Name")), "", rsMap1("Investor_Name"))
'    VSFCommGrdE.TextMatrix(i, 2) = IIf(IsNull(rsMap1("Pan")), "", rsMap1("Pan"))
'    VSFCommGrdE.TextMatrix(i, 3) = Format(rsMap1("Dob"), "DD/MM/YYYY")
'    VSFCommGrdE.TextMatrix(i, 4) = IIf(IsNull(rsMap1("Relation_name")), "", rsMap1("Relation_name"))
'    VSFCommGrdE.TextMatrix(i, 5) = IIf(IsNull(rsMap1("Rel_id")), "", rsMap1("Rel_id"))
'    VSFCommGrdE.TextMatrix(i, 6) = IIf(IsNull(rsMap1("occ_name")), "", rsMap1("Occ_Name"))
'    VSFCommGrdE.TextMatrix(i, 7) = IIf(IsNull(rsMap1("occupation_id")), "", rsMap1("Occupation_id"))
'    VSFCommGrdE.TextMatrix(i, 8) = IIf(IsNull(rsMap1("GUARDIAN_NAME")), "", rsMap1("GUARDIAN_NAME"))
'    VSFCommGrdE.TextMatrix(i, 9) = IIf(IsNull(rsMap1("GUARDIAN_PAN")), "", rsMap1("GUARDIAN_PAN"))
'    VSFCommGrdE.TextMatrix(i, 10) = IIf(IsNull(rsMap1("Kyc")), "NO", rsMap1("Kyc"))
'    VSFCommGrdE.Row = i
    'VSFCommGrdE.Cols = 11
'    If rsMap1.Fields("kyc") <> "NO" Then
'        VSFCommGrdE.CellForeColor = vbWhite
'        Set VSFCommGrdE.CellPicture = Image22.Picture
'        VSFCommGrdE.CellPictureAlignment = flexPicAlignLeftCenter
'        VSFCommGrdE.Text = 1
'    Else
'        VSFCommGrdE.CellForeColor = vbWhite
'        Set VSFCommGrdE.CellPicture = Image2.Picture
'        VSFCommGrdE.CellPictureAlignment = flexPicAlignLeftCenter
'        VSFCommGrdE.Text = 0
'    End If
'    i = i + 1
'    rsMap1.MoveNext
'Wend
'If VSFCommGrdE.Rows > 2 Then
'    VSFCommGrdE.Row = 1
'    VSFCommGrdE.Col = 0
'End If
'rsMap1.Close
'Set rsMap1 = Nothing
DoEvents
Exit Sub
err1:
    MsgBox err.Description
    ''''Resume
End Sub

Private Sub VSFCommGrdE1_Click()
Dim Temp_Id() As String

If GlbroleId = 13 Or GlbroleId = 78 Or GlbroleId = 76 Then
Else
    If (VSFCommGrdE1.Col = 0 Or VSFCommGrdE1.Col = 3 Or VSFCommGrdE1.Col = 4 Or VSFCommGrdE1.Col = 5 Or VSFCommGrdE1.Col = 7) Then
        If Trim(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 3)) = "" And VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 4) <> "" Then
            If DateDiff("YYYY", Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 4), "DD/MM/YYYY"), ServerDateTime) < 18 Then
                If VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 10) <> "" And ValidatePan(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 11)) = False Then
                    MsgBox "Please Enter Valid Pan No Of The Guardian ", vbInformation, strBajaj
                    Exit Sub
                ElseIf VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 10) = "" Then
                    MsgBox "Plese Enter Guardian Name ", vbInformation, strBajaj
                    Exit Sub
                ElseIf ValidatePan(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 11)) = False Then
                    MsgBox "Please Enter Valid Pan No Of The Guardian ", vbInformation, strBajaj
                    Exit Sub
                End If
            End If
        Else '----------he enters any pan no.----------------
            If VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 4) <> "" Then
                If DateDiff("YYYY", Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 4), "DD/MM/YYYY"), ServerDateTime) < 18 Then
                    MsgBox "Enter Pan No Of Guardian Only", vbInformation, "Wealthmaker"
                    VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 3) = ""
                    Exit Sub
                End If
            End If
        End If
    End If
End If
End Sub

Private Function ValidatePreviousvalue() As Boolean
Dim ValMan As Boolean
ValMan = True
i = VSFCommGrdE.Row - 1
If VSFCommGrdE.Rows > 1 Then
    If UCase(VSFCommGrdE.TextMatrix(i, 2)) = "" Or UCase(VSFCommGrdE.TextMatrix(i, 4)) = "" Or UCase(VSFCommGrdE.TextMatrix(i, 5)) = "" Or UCase(VSFCommGrdE.TextMatrix(i, 7)) = "" Then
         ValMan = False
    End If
End If
ValidatePreviousvalue = ValMan
End Function

Private Sub VSFCommGrdE1_GotFocus()
    flag = True
End Sub

Private Sub VSFCommGrdE1_LeaveCell()
On Error Resume Next
VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col) = UCase(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col))

If VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 4) <> "" Then
    If VSFCommGrdE1.Col = 4 And IsDate(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col)) = False Then
       MsgBox "Please Enter Valid Date"
       VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col) = ""
       Exit Sub
    End If
    If IsDate(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col)) = True Then
        If VSFCommGrdE1.Col = 4 And CDate(Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col), "DD/MM/YYYY")) > CDate(Format(ServerDateTime, "DD/MM/YYYY")) Then
           If DateDiff("d", Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col), "DD/MM/YYYY"), ServerDateTime) < 0 Then
                MsgBox "Date can Not Be Greater Than Current date"
                VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col) = ""
           End If
           Exit Sub
        End If
        If VSFCommGrdE1.Col = 4 And CDate(Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col), "DD/MM/YYYY")) > CDate(Format(ServerDateTime, "DD/MM/YYYY")) Then
           If DateDiff("YYYY", Format(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col), "DD/MM/YYYY"), ServerDateTime) < 18 Then
                MsgBox "Date can Not Be Greater Than Current date"
                VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, VSFCommGrdE1.Col) = ""
           End If
           Exit Sub
        End If
   End If
End If
    
If VSFCommGrdE1.Col = 0 Then
    If VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 0) <> "" Then
        invCode = Split(VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 0), "#")
        VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 2) = Trim(invCode(0))
        VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 1) = Trim(invCode(1))
    End If
End If

'VSFCommGrdE1.TextMatrix(VSFCommGrdE1.Row, 13) = VSFCommGrdE1.ColComboList(VSFCommGrdE1.Col)

End Sub
Private Sub VSFCommGrdE1_LostFocus()
    flag = False
End Sub
'------------------------------------------Client Category And Investor Category-------------------------------

Private Sub cmbinvestor_KeyPress(KeyAscii As Integer)
    On Error GoTo BOError
    If KeyAscii = 13 Then
        cmbacatg.SetFocus
    Else
        AutoMatch cmbinvestor, KeyAscii
        KeyAscii = 0
    End If
    Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
    Exit Sub
End Sub

Private Sub FillInvestorCat()
    On Error GoTo BOError
    Set RSV = MyConn.Execute("select investor_code,investor_type from investortype ")
    If Not RSV.EOF Then
        cmbinvestor.Clear
        Do While Not RSV.EOF
            cmbinvestor.AddItem RSV("investor_type") & Space(50) & "#" & RSV("investor_code")
            RSV.MoveNext
        Loop
    End If
    cmbinvestor.ListIndex = 0
    Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
    Exit Sub
End Sub

Private Sub cmbacatg_KeyPress(KeyAscii As Integer)
    On Error GoTo BOError
    If KeyAscii = 13 Then
        Ddob.SetFocus
    Else
        AutoMatch cmbacatg, KeyAscii
        KeyAscii = 0
    End If
    Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
    Exit Sub
End Sub
Private Sub FillClientCate()
    On Error GoTo BOError
Dim rs_CLCategory As New ADODB.Recordset
    Set rs_CLCategory = MyConn.Execute("select category_id,name from client_category order by name")
    If Not rs_CLCategory.EOF Then
        cmbacatg.Clear
        Do While Not rs_CLCategory.EOF
            cmbacatg.AddItem rs_CLCategory("name") & Space(50) & "#" & rs_CLCategory("category_id")
            rs_CLCategory.MoveNext
        Loop
    End If
    Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
    Exit Sub
End Sub

