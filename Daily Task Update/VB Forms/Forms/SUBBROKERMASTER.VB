Dim rsSubBroker As ADODB.Recordset
Dim rsCity As ADODB.Recordset
Dim rsLoc As ADODB.Recordset
Dim rsBank As ADODB.Recordset
Dim rsPmode As ADODB.Recordset
Dim rsAtype As ADODB.Recordset
Dim RSCAT As ADODB.Recordset
Dim rsEx As ADODB.Recordset
Dim rsStatus As ADODB.Recordset
Dim rsBB As ADODB.Recordset
Dim rsBr As ADODB.Recordset
Dim rsRm As ADODB.Recordset
Dim Ph As String
Dim VpospVerified As String
Dim VpospVerifiedBy As String
Dim VpospVerifiedDate As Date
Dim OldViewedSubbroker As String

Private Sub chkEnroll_Click()
   If chkEnroll.Value = 1 Then
        cmbExams.Enabled = True
        txtID.Enabled = True
        chkPassed.Enabled = True
        cmbExams.BackColor = vbWindowBackground
        txtID.BackColor = vbWindowBackground
    Else
        cmbExams.Enabled = False
        txtID.Enabled = False
        chkPassed.Enabled = False
        cmbExams.Text = ""
        txtID.Text = ""
        cmbExams.BackColor = vbButtonFace
        txtID.BackColor = vbButtonFace
    End If
End Sub
Private Sub chkEnroll_KeyPress(KeyAscii As Integer)
If chkEnroll.Value = 1 Then
 If KeyAscii = 13 Then
  cmbExams.SetFocus
End If
Else
    If KeyAscii = 13 Then
        cmdadd.SetFocus
     End If
  KeyAscii = 0
End If
End Sub
Private Sub chkPassed_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmdadd.SetFocus
End If
  KeyAscii = 0
End Sub

Private Sub ChkSMS_Click()
Dim ANASms As New ADODB.Command
If cmbExistCode.Text = "" Then
    MsgBox "Select ANA First", vbInformation
    ChkSMS.Value = 0
    Exit Sub
End If
If ChkSMS.Value = 1 Then
    Set ANASms.ActiveConnection = MyConn
    ANASms.CommandType = adCmdStoredProc
    ANASms.CommandText = "ACTIVE_SMS_AGENT"
    ANASms.Parameters.Append ANASms.CreateParameter("code1", adVarChar, adParamInput, 50, cmbExistCode)
    ANASms.Execute
    DoEvents
ElseIf ChkSMS.Value = 0 Then
    Set ANASms.ActiveConnection = MyConn
    ANASms.CommandType = adCmdStoredProc
    ANASms.CommandText = "DEACTIVE_SMS_AGENT"
    ANASms.Parameters.Append ANASms.CreateParameter("code1", adVarChar, adParamInput, 50, cmbExistCode)
    ANASms.Execute
    DoEvents
End If
End Sub

Private Sub ChkVerified_Click()
If ChkVerified.Value = 1 Then
    VpospVerified = "Y"
    VpospVerifiedBy = Glbloginid
    VpospVerifiedDate = ServerDateTime
Else
    VpospVerified = "N"
    VpospVerifiedBy = ""
    VpospVerifiedDate = ServerDateTime
End If
End Sub

Private Sub cmbAcType_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbPayMode.SetFocus
 Else
    AutoMatch cmbAcType, KeyAscii
    KeyAscii = 0
End If
End Sub


Private Sub cmbANA_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If CmbAna.MatchFound = False Then
        CmbAna.ListIndex = -1
    End If
End Sub


Private Sub cmbBank_Click()

Dim branchcode As String
Dim bname As String
Dim B() As String

    
    branchcode = ""
    bname = ""
    
    If Trim(CmbBank.Text) <> "" And Trim(CmbBank.Text) <> "-Select Branch-" Then
        B = Split(Trim(CmbBank.Text), "#")
        branchcode = Trim(B(1))
        bname = Trim(B(0))
    End If
Set rsBB = New ADODB.Recordset
 If rsBB.State = 1 Then rsBB.Close
     rsBB.CursorLocation = adUseClient
    rsBB.open "select branch from bank_branch_master where bank_id ='" & Trim(branchcode) & "'", MyConn, adOpenDynamic, adLockPessimistic
      'branch
      If rsBB.RecordCount > 0 Then
          cmbBB.Clear
         For i = 0 To rsBB.RecordCount - 1
           With cmbBB
           ' With cmbbankcity
             If Trim(rsBB(0)) <> "" Then
               .AddItem Trim(rsBB(0))
             End If
               rsBB.MoveNext
           End With
         Next
       rsBB.Close
      End If
      
End Sub

Private Sub cmbBank_DragOver(Source As Control, X As Single, Y As Single, State As Integer)
 'cmbBank.ToolTipText = cmbBank.SelText
 
End Sub

Private Sub cmbBank_DropDown()
 Call Bank
End Sub
Private Sub cmbBank_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbBB.SetFocus
 Else
    AutoMatch CmbBank, KeyAscii
    KeyAscii = 0
End If
End Sub

Private Sub cmbBank_Scroll()
 CmbBank.ToolTipText = CmbBank.Text
End Sub


Public Sub cmbbankcity_Click()
 Dim branchcode As String
Dim bname As String
Dim B() As String
    branchcode = ""
    bname = ""
    
    If Trim(CmbBank.Text) <> "" And Trim(CmbBank.Text) <> "-Select Branch-" Then
        B = Split(Trim(CmbBank.Text), "#")
        branchcode = Trim(B(1))
        bname = Trim(B(0))
    End If

Set rsBB = New ADODB.Recordset
 If rsBB.State = 1 Then rsBB.Close
     rsBB.CursorLocation = adUseClient
    rsBB.open "select branch,bank_BRANCHid from anabank_branch_master where bank_id ='" & Trim(branchcode) & "' and BRANCH_city='" & Trim(cmbbankcity.Text) & "'", MyConn, adOpenStatic, adLockPessimistic
    Debug.Print "select branch,bank_BRANCHid from anabank_branch_master where bank_id ='" & Trim(branchcode) & "'and BRANCH_city='" & Trim(cmbbankcity.Text) & "'"
      
      'branch
     ' If rsBB.RecordCount >= 0 Then
          cmbBB.Clear
       Do While Not rsBB.EOF
      '   For i = 0 To rsBB.RecordCount - 1
           With cmbBB
           ' With cmbbankcity
             If Trim(rsBB(0)) <> "" Then
               .AddItem Trim(rsBB(0)) & Space(50) & "#" & Trim(rsBB(1))
             End If
               rsBB.MoveNext
           End With
'       rsBB.MoveNext
       Loop
       rsBB.Close
     ' End If

End Sub

Private Sub cmbBB_Click()
Dim rsBank As New ADODB.Recordset
On Error GoTo BOError
Dim StrSql As String
Dim V() As String, v1() As String
Dim banbrcode As String
Dim bancode As String
If Trim(CmbBank.Text) <> "" Then
    V = Split(Trim(CmbBank.Text), "#")
    bancode = Trim(V(1))
End If
If Trim(cmbBB.Text) <> "" Then
    v1 = Split(Trim(cmbBB.Text), "#")
    banbrcode = Trim(v1(1))
End If
If banbrcode <> "" And bancode <> "" Then
    StrSql = "select a.BANK_NAME,b.BRANCH,b.bank_BRANCHid,b.branch_micrno,b.branch_address,b.BRANCH_PHONENO,b.branch_city,branch_pin,b.branch_contact_person,b.LOGGEDUSERID,b.ifsc_no from ANA_bank_master a,ANAbank_branch_master B where 1=1"
    StrSql = StrSql & " and a.BANK_ID=b.BANK_ID  AND BANK_BRANCHID=" & banbrcode & " and b.BANK_ID=" & bancode & ""
    StrSql = StrSql & " order by bank_name"
    If rsBank.State = 1 Then rsBank.Close
    rsBank.CursorLocation = adUseClient
    rsBank.open StrSql, MyConn, adOpenForwardOnly, adLockReadOnly
    If rsBank.EOF = False Then
        'Frame4.Visible = True comment by pankaj
        If IsNull(rsBank(0)) Then
            txt_BankName.Text = ""
        Else
            txt_BankName.Text = rsBank(0)
        End If
        If IsNull(rsBank(1)) Then
            txt_BankBranch.Text = ""
        Else
            txt_BankBranch.Text = rsBank(1)
        End If
        If IsNull(rsBank(4)) Then
            txt_address.Text = ""
        Else
            txt_address.Text = rsBank(4)
        End If
        If IsNull(rsBank(5)) Then
            txt_phone.Text = ""
        Else
            txt_phone.Text = rsBank(5)
        End If
        If IsNull(rsBank(7)) Then
            txt_pin.Text = ""
        Else
            txt_pin.Text = rsBank(7)
        End If
        If IsNull(rsBank(3)) Then
            txt_micrno.Text = ""
        Else
            txt_micrno.Text = rsBank(3)
        End If
        If IsNull(rsBank(3)) Then
            txt_micrno.Text = ""
        Else
            txt_micrno.Text = rsBank(3)
        End If
        If IsNull(rsBank(6)) Then
            txt_city.Text = ""
        Else
            txt_city.Text = rsBank(6)
        End If
        If IsNull(rsBank(8)) Then
            txt_contactper.Text = ""
        Else
            txt_contactper.Text = rsBank(8)
        End If
        
        If IsNull(rsBank("ifsc_no")) Then
            txtIFSC.Text = ""
        Else
            txtIFSC.Text = rsBank("ifsc_no")
        End If
    End If
End If
'' Call cmbbankcity_Click
    ''Call Search(StrSql)
Exit Sub
BOError:
      MsgBox err.Description, vbCritical, "Backoffice"
Exit Sub

''FRMANABANK_BRANCH_DETAILS.Show
End Sub

Private Sub cmbBB_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtAC.SetFocus
 Else
    AutoMatch cmbBB, KeyAscii
    KeyAscii = 0
End If
End Sub

Private Sub cmbcity_Click()
On Error GoTo BOError

Dim rsSID As ADODB.Recordset
Dim rsSName As ADODB.Recordset
 Set rsSID = New ADODB.Recordset
If rsSID.State = 1 Then rsSID.Close
 rsSID.CursorLocation = adUseClient
 rsSID.open "select state_id,CITY_ID from city_master where city_name='" & Trim(cmbcity) & "'", MyConn, adOpenDynamic, adLockPessimistic
  Set rsSName = New ADODB.Recordset
 If rsSName.State = 1 Then rsSName.Close
 rsSName.CursorLocation = adUseClient
 If Not rsSID.EOF Then
 rsSName.open "select state_name from state_master where state_id='" & Trim(rsSID(0)) & "'", MyConn, adOpenDynamic, adLockPessimistic
 If rsSName.RecordCount > 0 Then
   If (rsSName(0)) <> "" Then
    txtstate.Text = Trim(rsSName(0))
   End If
  If rsSName.State Then rsSName.Close
 End If
 End If
 Set rsLoc = New ADODB.Recordset
   If rsLoc.State = 1 Then rsLoc.Close
    rsLoc.CursorLocation = adUseClient
    If Not rsSID.EOF Then
      rsLoc.open "select location_name from location_master where city_id='" & rsSID(1) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsLoc.RecordCount > 0 Then
     cmbloc.Clear
      For i = 0 To rsLoc.RecordCount - 1
       If rsLoc(0) <> "" Then
         cmbloc.AddItem rsLoc(0)
       End If
         rsLoc.MoveNext
      Next
    Else
     cmbloc.Clear
     cmbloc.ListIndex = -1
    End If
    If rsLoc.State = 1 Then rsLoc.Close
    End If
   Set rsSName = Nothing
   Set rsLoc = Nothing
   Set rsSID = Nothing
Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
Exit Sub
End Sub
Private Sub cmbcity_DropDown()
 Call City
End Sub
Private Sub cmbcity_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
 cmbloc.SetFocus
Else
  AutoMatch cmbcity, KeyAscii
  KeyAscii = 0
End If
End Sub
Private Sub cmbDist_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  chkEnroll.SetFocus
 Else
    AutoMatch cmbDist, KeyAscii
    'KeyAscii = 0
End If
End Sub

Private Sub cmbExams_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtID.SetFocus
 Else
    AutoMatch cmbExams, KeyAscii
    KeyAscii = 0
End If
End Sub
Private Sub cmbExams_Validate(Cancel As Boolean)
If Trim(cmbExams.Text) = "" Then
   Cancel = True
   MsgBox "Plz. Select a Sub Broker Exams", vbCritical, "Alert"
End If
If Cancel = True Then
cmbExams.SetFocus
End If
End Sub
Private Sub cmbExistCode_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtName.SetFocus
End If
End Sub
Private Sub cmbloc_Click()
Dim rsSID As ADODB.Recordset
 Set rsSID = New ADODB.Recordset
If rsSID.State = 1 Then rsSID.Close
 rsSID.CursorLocation = adUseClient
 rsSID.open "select pincode from location_master where location_name='" & Trim(cmbloc) & "'", MyConn, adOpenDynamic, adLockPessimistic
 If rsSID.RecordCount > 0 Then
   If (rsSID(0)) <> "" Then
    txtpin.Text = Trim(rsSID(0))
   End If
  End If
End Sub
Private Sub cmbloc_KeyPress(KeyAscii As Integer)
   If KeyAscii = 13 Then
        txtpin.SetFocus
   Else
        AutoMatch cmbloc, KeyAscii
        KeyAscii = 0
   End If
End Sub

Private Sub cmbPayMode_Click()
If cmbPayMode.Text = "ECS" Then
    Frame4.Visible = True
    txt_BankName.Text = ""
    txt_BankBranch.Text = ""
    txt_address.Text = ""
    txt_phone.Text = ""
    txt_pin.Text = ""
    txt_contactper.Text = ""
    txt_city.Text = ""
    txt_micrno.Text = ""
Else
    Frame4.Visible = False
End If
End Sub

Private Sub cmbPayMode_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  dob.SetFocus
 Else
    AutoMatch cmbPayMode, KeyAscii
    KeyAscii = 0
End If
End Sub




Private Sub CMBPOSPTYPE_Click()
If CMBPOSPTYPE.Text = "Life" Then
    TXTPOSPNOLI.Enabled = True
    TXTPOSPNOGI.Enabled = False
    pospcertifieddateLI.Enabled = True
    pospcertifieddateGI.Enabled = False
    PospValidTillLI.Enabled = True
    PospValidTillGI.Enabled = False
    TXTPOSPNOGI.Text = ""
    pospcertifieddateGI.Text = "__/__/____"
    PospValidTillGI.Text = "__/__/____"
ElseIf CMBPOSPTYPE.Text = "General" Then
    TXTPOSPNOGI.Enabled = True
    TXTPOSPNOLI.Enabled = False
    pospcertifieddateGI.Enabled = True
    pospcertifieddateLI.Enabled = False
    PospValidTillGI.Enabled = True
    PospValidTillLI.Enabled = False
    TXTPOSPNOLI.Text = ""
    pospcertifieddateLI.Text = "__/__/____"
     PospValidTillLI.Text = "__/__/____"
ElseIf CMBPOSPTYPE.Text = "Both" Then
    TXTPOSPNOGI.Enabled = True
    TXTPOSPNOLI.Enabled = True
    pospcertifieddateGI.Enabled = True
    pospcertifieddateLI.Enabled = True
    PospValidTillLI.Enabled = True
    PospValidTillGI.Enabled = True
End If
End Sub

Private Sub cmbr_city_Click()
Dim cname_r As New ADODB.Recordset
    txtr_pin.Text = ""
    txtr_state.Text = ""
    Set cname_r = MyConn.Execute("select distinct state_id,city_id from city_master where city_name='" & Trim(cmbr_city.Text) & "'")
    If Not cname_r.EOF Then
        Set cname_r = MyConn.Execute("select state_name from state_master where state_id=" & cname_r(0))
        If Not cname_r.EOF Then
           txtr_state.Text = cname_r(0)
        End If
    End If
    '''txtr_pin.SetFocus
End Sub

Private Sub cmbr_city_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
         txtr_pin.SetFocus
Else
        AutoMatch cmbr_city, KeyAscii
        KeyAscii = 0
End If
End Sub

Private Sub cmbr_city_LostFocus()
If cmbr_city.Text = "" Then
txtr_state.Text = ""
End If
End Sub

Private Sub cmbrm_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtAdd1.SetFocus
 Else
   AutoMatch cmbrm, KeyAscii
   KeyAscii = 0
End If
End Sub
Private Sub cmbrm_Validate(Cancel As Boolean)
If Trim(cmbrm) = "" Then
   Cancel = True
   MsgBox "Plz. Select a Employee", vbCritical, "Alert"
End If
If Cancel = True Then
cmbrm.SetFocus
End If
End Sub
Private Sub cmbSCat_DropDown()
 Call SCategory
End Sub
Private Sub cmbSCat_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  SbDate.SetFocus
 Else
    AutoMatch cmbSCat, KeyAscii
    KeyAscii = 0
End If
End Sub
Private Sub cmbSource_Click()
 Dim i As Integer
 Set rsRm = New ADODB.Recordset
 If rsRm.State = 1 Then rsRm.Close
   rsRm.CursorLocation = adUseClient
   If Trim(SRmCode) <> "" Then
        rsRm.open "select rm_name,rm_code from employee_master where type='A' and source=(select branch_code from branch_master where branch_name='" & Trim(cmbSource.Text) & "') and rm_code=" & SRmCode & " order by rm_name", MyConn, adOpenDynamic, adLockReadOnly
   Else
        rsRm.open "select rm_name,rm_code from employee_master where type='A' and  source=(select max(branch_code) from branch_master where branch_name='" & Trim(cmbSource.Text) & "')", MyConn, adOpenDynamic, adLockReadOnly
   End If
   cmbrm.Clear
   If rsRm.RecordCount > 0 Then
     With cmbrm
         For i = 0 To rsRm.RecordCount - 1
           If rsRm(0) <> "" Then
             .AddItem rsRm(0)
           End If
             rsRm.MoveNext
         Next
       rsRm.Close
     End With
   End If
End Sub
Private Sub cmbsource_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbrm.SetFocus
 Else
    AutoMatch cmbSource, KeyAscii
    KeyAscii = 0
End If
End Sub
Private Sub cmbsource_Validate(Cancel As Boolean)
If Trim(cmbSource) = "" Then
    Cancel = True
    MsgBox "Plz. Select a Branch", vbCritical, "Alert"
End If
If Cancel = True Then
   cmbSource.SetFocus
End If
End Sub
Private Sub cmbStatus_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtpan.SetFocus
 Else
    AutoMatch CmbStatus, KeyAscii
    KeyAscii = 0
End If
End Sub
Private Sub cmbstype_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtCPEmail.SetFocus
End If
End Sub

Private Sub cmbtype_Click()
If CmbType.Text = "PAID" Then
    cmbExistCode.Enabled = False
Else
    cmbExistCode.Enabled = True
End If
End Sub

Private Sub cmdAdd_Click()
If Trim(txtName.Text) = "" Then
    MsgBox "Plz. Insert a Sub Broker Name", vbCritical, "Alert"
    txtName.SetFocus
    Exit Sub
End If
  
If ChkVerified.Value = 1 Then
    VpospVerified = "Y"
    VpospVerifiedBy = Glbloginid
    VpospVerifiedDate = ServerDateTime
Else
    VpospVerified = "N"
    VpospVerifiedBy = ""
    VpospVerifiedDate = "01-JAN-1850"
End If


If CMBPOSPTYPE = "Life" Or CMBPOSPTYPE = "Both" Then
    If pospcertifieddateLI.Text = "__/__/____" Then
       MsgBox "Please Enter POSP Certified start date for Life", vbCritical, "Alert"
       pospcertifieddateLI.SetFocus
       Exit Sub
    End If
End If
If CMBPOSPTYPE = "General" Or CMBPOSPTYPE = "Both" Then
    If pospcertifieddateGI.Text = "__/__/____" Then
       MsgBox "Please Enter POSP Certified start date for GI", vbCritical, "Alert"
       pospcertifieddateGI.SetFocus
       Exit Sub
    End If
End If


If pospcertifieddateLI.Text <> "__/__/____" Then
    If IsDate(pospcertifieddateLI.Text) = False Then
        MsgBox "Invalid Date Format of LI POSP Start Date", vbInformation, "BACKOFFICE"
        pospcertifieddateLI.SetFocus
        Exit Sub
    End If
End If
 
If pospcertifieddateGI.Text <> "__/__/____" Then
    If IsDate(pospcertifieddateGI.Text) = False Then
        MsgBox "Invalid Date Format of GI POSP Start Date", vbInformation, "BACKOFFICE"
        pospcertifieddateGI.SetFocus
        Exit Sub
    End If
End If

If PospValidTillLI.Text <> "__/__/____" Then
    If IsDate(PospValidTillLI.Text) = False Then
        MsgBox "Invalid Date Format of LI POSP Valid Till Date", vbInformation, "BACKOFFICE"
        PospValidTillLI.SetFocus
        Exit Sub
    End If
End If
 
If PospValidTillGI.Text <> "__/__/____" Then
    If IsDate(PospValidTillGI.Text) = False Then
        MsgBox "Invalid Date Format of GI POSP Valid Till Date", vbInformation, "BACKOFFICE"
        PospValidTillGI.SetFocus
        Exit Sub
    End If
End If
 
 
'PospValidTill can not be smaller than start date--------------------------------------------------------------
If CMBPOSPTYPE = "Life" Or CMBPOSPTYPE = "Both" Then
    If PospValidTillLI.Text <> "__/__/____" And pospcertifieddateLI.Text <> "__/__/____" Then
        If CDate(Format(pospcertifieddateLI, "DD/MM/YYYY")) > CDate(Format(PospValidTillLI, "DD/MM/YYYY")) Then
            MsgBox "POSP Start Date cannot be greater than POSP Valid Till Date for LI ", vbInformation
            PospValidTillLI = Format(Date, "dd/mm/yyyy")
            PospValidTillLI.SetFocus
            Exit Sub
        End If
    End If
 End If
 
If CMBPOSPTYPE = "General" Or CMBPOSPTYPE = "Both" Then
    If PospValidTillGI.Text <> "__/__/____" And pospcertifieddateGI.Text <> "__/__/____" Then
        If CDate(Format(pospcertifieddateGI, "DD/MM/YYYY")) > CDate(Format(PospValidTillGI, "DD/MM/YYYY")) Then
            MsgBox "POSP Start Date cannot be greater than POSP Valid Till Date for GI ", vbInformation
            PospValidTillGI = Format(Date, "dd/mm/yyyy")
            PospValidTillGI.SetFocus
            Exit Sub
        End If
    End If
End If
 


     
     
If Trim(cmbcity.Text) = "" Then
   MsgBox "Plz. Enter Sub Broker City.", vbCritical, "Alert"
   cmbcity.SetFocus
   Exit Sub
End If


If Len(txtMob.Text) <> 10 Then
   MsgBox "Plz. Enter Correct Mobile Number.", vbCritical, "Alert"
   txtMob.SetFocus
   Exit Sub
End If


 If Len(txtpin.Text) <> 6 Then
   MsgBox "Plz. Enter Correct 6 digit PIN Number.", vbCritical, "Alert"
   txtpin.SetFocus
   Exit Sub
End If


'txtr_add1

'txtAdd1

If Trim(txtAdd1.Text) = "" Then
   MsgBox "Plz. Enter Sub Broker Residential Address.", vbCritical, "Alert"
   txtAdd1.SetFocus
   Exit Sub
End If
     
     
     
If Trim(TxtEmail.Text) = "" Then
   MsgBox "Plz. Enter Sub Broker email Address.", vbCritical, "Alert"
   TxtEmail.SetFocus
   Exit Sub
End If

If IsDate(dob.Text) = False Then
   MsgBox "Plz. Enter Sub Broker Date of Birth.", vbCritical, "Alert"
   dob.SetFocus
   Exit Sub
End If

'--------------- PAN Validation By Vinay.
'--------------- dt. 05-Mar-2009
txtpan.Text = UCase(txtpan.Text)
If ValidatePan(txtpan.Text) = False Then
   MsgBox "Please enter a correct PAN Number.", vbCritical, "Alert"
   txtpan.SetFocus
   Exit Sub
End If
'----------------

If OptSB.Value = False Then
   MsgBox "Plz. select Sub Broker Type", vbCritical, "Alert"
   OptSB.SetFocus
   Exit Sub
End If
If Trim(cmbSource) = "" Then
   MsgBox "Plz. Select a Branch", vbCritical, "Alert"
   cmbSource.SetFocus
   Exit Sub
End If
 If Trim(cmbrm) = "" Then
    MsgBox "Plz. Select a Employee", vbCritical, "Alert"
    cmbrm.SetFocus
    Exit Sub
 End If
 If Trim(txtCPer.Text) = "" Then
    MsgBox "Plz. Insert  Contact Person Name", vbCritical, "Alert"
    txtCPer.SetFocus
    Exit Sub
 End If
 If Trim(cmbstype) = "" Then
    MsgBox "Plz. Select a Sub Broker Type", vbCritical, "Alert"
    cmbstype.SetFocus
    Exit Sub
 End If
'     If Trim(cmbSCat.Text) = "" Then
'        MsgBox "Plz. Select a Sub Broker Category", vbCritical, "Alert"
'        cmbSCat.SetFocus
'        Exit Sub
'     End If

 If Trim(txtpan.Text) = "" Then
    MsgBox "Plz. Select a PAN Number.", vbCritical, "Alert"
    txtpan.SetFocus
    Exit Sub
 End If

'txtPan
 If SbDate.Text = "__/__/____" Then
    MsgBox "Plz. Please Enter Enrollment Date", vbCritical, "Alert"
    SbDate.SetFocus
    Exit Sub
 End If
 If txtAC.Text <> "" Then
    If Len(txtAC.Text) > 20 Then
    MsgBox "Account No can not greater then 20", vbInformation, "Wealth Maker"
    txtAC.SetFocus
    Exit Sub
    End If
 End If
 
 
 
 If CmbType.Text = "PAID" Then
    'Me.Hide
    'Me.Show
    MsgBox "You have to fill some information about ANA before creating it.", vbInformation
    ANAcommingForm = Me.Name
    
    FrmAdvisorSubEntry.BorderStyle = 1
    FrmAdvisorSubEntry.TxtANAName.Text = UCase(txtName.Text)
    FrmAdvisorSubEntry.txtRM.Text = UCase(cmbrm.Text)
    FrmAdvisorSubEntry.TxtANABranch.Text = UCase(cmbSource.Text)
    FrmAdvisorSubEntry.txtRM.Visible = True
    FrmAdvisorSubEntry.CmbANARM.Visible = False
    
    
    FrmAdvisorSubEntry.TxtANACode.Enabled = False
    FrmAdvisorSubEntry.Picture1.Enabled = False
    FrmAdvisorSubEntry.CmbANARM.Enabled = False
    FrmAdvisorSubEntry.Picture2.Enabled = False
    FrmAdvisorSubEntry.Show vbModal
    Exit Sub
Else
    Call SubBrokerOffData
End If
     
    
      
End Sub
Private Sub cmdBNew_Click()

 FRMANA_BANKMASTER.Show
 FRMANA_BANKMASTER.ZOrder
End Sub

Private Sub cmdbranchnew_Click()
Dim branchcode As String
Dim bname As String
Dim B() As String
    branchcode = ""
    bname = ""
    
    
    If Trim(CmbBank.Text) = "" Or Trim(CmbBank.Text) = "-Select Branch-" Then
    MsgBox "Please select bank", vbInformation, "Wealth Maker"
    CmbBank.SetFocus
      Exit Sub
    End If
    If Trim(cmbbankcity.Text) = "" Or Trim(cmbbankcity.Text) = "-Select Branch-" Then
    MsgBox "Please select bank city", vbInformation, "Wealth Maker"
    cmbbankcity.SetFocus
      Exit Sub
    End If
    
    If Trim(CmbBank.Text) <> "" And Trim(CmbBank.Text) <> "-Select Branch-" Then
        B = Split(Trim(CmbBank.Text), "#")
        branchcode = Trim(B(1))
        bname = Trim(B(0))
    End If

FRMANA_BANK_BRANCH_MASTER.Show
FRMANA_BANK_BRANCH_MASTER.ZOrder
FRMANA_BANK_BRANCH_MASTER.TBankName.Text = bname
FRMANA_BANK_BRANCH_MASTER.txtBankid.Text = branchcode
FRMANA_BANK_BRANCH_MASTER.TBankBranch.Text = cmbbankcity.Text
End Sub

Private Sub cmdCity_Click()
Frmcity.Show
Frmcity.SSTab1.Tab = 1
Frmcity.ZOrder
End Sub
Private Sub cmdCity_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtMob.SetFocus
End If
  KeyAscii = 0
End Sub
Private Sub cmdExit_Click()
'cmbExistCode = ""
Unload Me
End Sub
Private Sub cmdFind_Click()
 frmAgentCode.Show
 frmAgentCode.ZOrder
End Sub

Private Sub cmdOk_Click()
On Error GoTo BOError
If Val(txtPh.Text) <> 0 Then
Phone.Visible = True
With Phone
.Col = 0
.Row = 0
.ColWidth(0) = 800
.Font.Bold = True
.Text = "STD"
.Col = 1
.ColWidth(1) = 1450
.Font.Bold = True
.Text = "Phone Number"
End With
With Phone
  .Rows = Val(txtPh.Text) + 1
  .SetFocus
End With
If KeyAscii = 13 Then
      Phone.SetFocus
     End If
      KeyAscii = 0
Else
    MsgBox "Please Enter Total No. of Phones", vbInformation, "BackOffice"
End If
Exit Sub
BOError:
      MsgBox err.Description, vbCritical, "Backoffice"
Exit Sub
End Sub
Private Sub cmdOK_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  Phone.SetFocus
End If
  KeyAscii = 0
End Sub

Private Sub cmdr_close_Click()
Frame_residence.Visible = False
TxtFax.SetFocus
End Sub

Private Sub CMDRESET_Click()
Dim X As Control
 For Each X In Controls
    If TypeOf X Is TextBox Then
       X = ""
    End If
    If TypeOf X Is ComboBox Then
     If X.Style = 0 Then
        X.Text = ""
      End If
      If X.Style = 2 Then
         X.ListIndex = -1
      End If
    End If
    If TypeOf X Is OptionButton Then
       X.Value = False
    End If
    If TypeOf X Is CheckBox Then
       X.Value = False
    End If
    If TypeOf X Is MSFlexGrid Then
       X.Clear
    End If
    If TypeOf X Is MaskEdBox Then
       X = "__/__/____"
    End If
 Next
 txtr_add1.Text = ""
 txtr_add2.Text = ""
 cmbr_city.Text = ""
 txtr_state.Text = ""
 txtr_pin.Text = ""
 cmbstype.ListIndex = -1
cmbSource.Enabled = False
cmbrm.Enabled = False
cmbExistCode.Enabled = True
txtSbCode.Enabled = True

CmbType.ListIndex = 0

txtpan.Enabled = True
Frame3.Enabled = True
txtName.Enabled = True
End Sub

Private Sub CMDRESIDENCE_Click()
Frame_residence.Visible = True
Frame_residence.Top = 2040
Frame_residence.Height = 2370
Frame_residence.width = 5775
Frame_residence.ZOrder
Dim RSV1 As New ADODB.Recordset
Set RSV1 = MyConn.Execute("select distinct city_name from city_master")
    If Not RSV1.EOF Then
        ''cmbr_city.Clear
        ''txtr_state.Text = ""
        While Not RSV1.EOF
            cmbr_city.AddItem UCase(RSV1("city_name"))
            RSV1.MoveNext
        Wend
    End If
    txtr_add1.SetFocus
End Sub

Private Sub cmdSNew_Click()
 Category.Show
 Category.ZOrder
End Sub
Private Sub cmdUpdate_Click()
Call cmdAdd_Click
End Sub
Private Sub cmdview_Click()
frmSubBrokerMaster.Visible = False
frmSubBrokerMasterView.Show

End Sub
Private Sub DOB_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  CmbStatus.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub DOB_LostFocus()
 If dob <> "" And dob <> "__/__/____" Then
  If Not IsValidDateString(dob, e1or2D_1or2M_2or4Y) Then
   MsgBox "Plz.. Insert Valid Date !", vbCritical, "Alert"
   dob.SetFocus
   Exit Sub
  End If
  If CDate(dob.Text) > Date Then
    MsgBox "Date Of Birth should be Less then Current Date", vbCritical, "Alert"
    dob.SetFocus
    Exit Sub
  End If
  If SbDate.Text <> "__/__/____" Then
  If CDate(dob.Text) > CDate(SbDate.Text) Then
    MsgBox "Date Of Birth should be Less then Joining date", vbCritical, "Alert"
    dob.SetFocus
    Exit Sub
  End If
  End If
End If
End Sub

'Private Function ValidatePan(str As String) As Boolean
'On Error GoTo BotError
'    Dim strTemp As String
'    strTemp = UCase(Trim(str))
'    If Trim(str) = "" Then
'        ValidatePan = False
'        Exit Function
'    End If
'
'    If (Asc(Mid(strTemp, 1, 1)) < 65 Or Asc(Mid(strTemp, 1, 1)) > 92) Or _
'    (Asc(Mid(strTemp, 2, 1)) < 65 Or Asc(Mid(strTemp, 2, 1)) > 92) Or _
'    (Asc(Mid(strTemp, 3, 1)) < 65 Or Asc(Mid(strTemp, 3, 1)) > 92) Or _
'    (Asc(Mid(strTemp, 4, 1)) < 65 Or Asc(Mid(strTemp, 4, 1)) > 92) Or _
'    (Asc(Mid(strTemp, 5, 1)) < 65 Or Asc(Mid(strTemp, 5, 1)) > 92) Then
'            ValidatePan = False
'            Exit Function
'    End If
'
'    If (Asc(Right(strTemp, 1)) < 65 Or Asc(Right(strTemp, 1)) > 92) _
'        Or (InStr("CPHFATBLJG", Mid(strTemp, 4, 1)) <= 0) _
'        Or (IsNumeric(Mid(UCase(strTemp), 6, 4)) = False) Then  'Or (Mid(UCase(txtPan.Text), 5, 1) = Left(UCase(txtlastname.Text), 1)) Then
'            ValidatePan = False
'            Exit Function
'    End If
'
'    'txtlastname
'    ValidatePan = True
'           Exit Function
'BotError:
'    If int_ErrorShow = 1 Then
'        MsgBox "An unhandled exception occured in the system, Please contact to the Administrator." & vbCrLf & "Error- " & err.Description, vbCritical, "Wealthmaker"
'    End If
'End Function
'
'    strTemp={AGENT_MASTER.PAN}
'    strTemp = UCase(Trim(strTemp))
'
'
'
'    If Trim(strTemp) = "" Then
'        check = False
'    Else
'        If (Asc(Mid(strTemp, 1, 1)) < 65 Or Asc(Mid(strTemp, 1, 1)) > 92) Or _
'        (Asc(Mid(strTemp, 2, 1)) < 65 Or Asc(Mid(strTemp, 2, 1)) > 92) Or _
'        (Asc(Mid(strTemp, 3, 1)) < 65 Or Asc(Mid(strTemp, 3, 1)) > 92) Or _
'        (Asc(Mid(strTemp, 4, 1)) < 65 Or Asc(Mid(strTemp, 4, 1)) > 92) Or _
'        (Asc(Mid(strTemp, 5, 1)) < 65 Or Asc(Mid(strTemp, 5, 1)) > 92) Then
'                check = False
'        Else
'            If (Asc(Right(strTemp, 1)) < 65 Or Asc(Right(strTemp, 1)) > 92) _
'                Or (InStr("CPHFATBLJG", Mid(strTemp, 4, 1)) <= 0) _
'                Or (IsNumeric(Mid(UCase(strTemp), 6, 4)) = False) Then
'                    check = False
'
'            End If
'        End If
'    End If
'
'    If check = True Then
'        formula = "Y"
'    Else
'        formula = "N"
'    End If
Private Sub Form_Load()
Dim rsANA As ADODB.Recordset
Dim i As Integer

ANAcommingForm = ""
my_TBLUID = ""


CMBPOSPMARKING.AddItem "N"
CMBPOSPMARKING.AddItem "Y"

CMBPOSPTYPE.AddItem "Life"
CMBPOSPTYPE.AddItem "General"
CMBPOSPTYPE.AddItem "Both"
CMBPOSPTYPE.AddItem ""

SbDate.Text = Format(ServerDateTime, "DD/MM/YYYY")
Image1.Picture = LoadPicture(App.Path & "\logo1.JPG")
    Me.Icon = LoadPicture(App.Path & "\W.ICO")


If GlbDataFilter = "72" Then
    Frm_audited.Visible = True
    cmdBNew.Visible = True
End If

CmbType.ListIndex = 0
ButtonPermission Me
 Set rsPmode = New ADODB.Recordset
  If rsPmode.State = 1 Then rsPmode.Close
     rsPmode.CursorLocation = adUseClient
      rsPmode.open "select itemname from fixeditem where itemtype='10'", MyConn, adOpenDynamic, adLockPessimistic
       cmbPayMode.Clear
       If rsPmode.RecordCount > 0 Then
          For i = 0 To rsPmode.RecordCount - 1
             With cmbPayMode
               If Trim(rsPmode(0)) <> "" Then
                  .AddItem Trim(rsPmode(0))
               End If
                  rsPmode.MoveNext
             End With
          Next
        rsPmode.Close
       End If
 Set rsAtype = New ADODB.Recordset
  If rsAtype.State = 1 Then rsAtype.Close
     rsAtype.CursorLocation = adUseClient
      rsAtype.open "select itemname from fixeditem where itemtype='7'", MyConn, adOpenDynamic, adLockPessimistic
       cmbAcType.Clear
       If rsAtype.RecordCount > 0 Then
          For i = 0 To rsAtype.RecordCount - 1
             With cmbAcType
               If Trim(rsAtype(0)) <> "" Then
                  .AddItem Trim(rsAtype(0))
               End If
                  rsAtype.MoveNext
             End With
          Next
        rsAtype.Close
       End If
 Set rsEx = New ADODB.Recordset
  If rsEx.State = 1 Then rsEx.Close
     rsEx.CursorLocation = adUseClient
      rsEx.open "select itemname from fixeditem where itemtype='6' ", MyConn, adOpenDynamic, adLockPessimistic
       cmbExams.Clear
       If rsEx.RecordCount > 0 Then
          For i = 0 To rsEx.RecordCount - 1
             With cmbExams
                If Trim(rsEx(0)) <> "" Then
                  .AddItem Trim(rsEx(0))
                End If
                  rsEx.MoveNext
             End With
          Next
         rsEx.Close
       End If
Set rsStatus = New ADODB.Recordset
  If rsStatus.State = 1 Then rsStatus.Close
     rsStatus.CursorLocation = adUseClient
      rsStatus.open "select itemname from fixeditem where itemtype='5' ", MyConn, adOpenDynamic, adLockPessimistic
       CmbStatus.Clear
       If rsStatus.RecordCount > 0 Then
          For i = 0 To rsStatus.RecordCount - 1
             With CmbStatus
               If Trim(rsStatus(0)) <> "" Then
                  .AddItem Trim(rsStatus(0))
               End If
                  rsStatus.MoveNext
             End With
          Next
       rsStatus.Close
       End If
       
    ''03/06/05********jawahar
    Set rsStype = New ADODB.Recordset
    If rsStype.State = 1 Then rsStype.Close
    rsStype.CursorLocation = adUseClient
    rsStype.open "select INVESTOR_TYPE from INVESTORTYPE  where Investor_code not in (13,14) ", MyConn, adOpenDynamic, adLockPessimistic
     cmbstype.Clear
       If rsStype.RecordCount > 0 Then
          For i = 0 To rsStype.RecordCount - 1
             With cmbstype
               If Trim(rsStype(0)) <> "" Then
                  .AddItem Trim(rsStype(0))
               End If
                  rsStype.MoveNext
             End With
          Next
        rsStype.Close
       End If
       ''END******
       
    Set rsANA = New ADODB.Recordset
    i = 0
    rsANA.open "select exist_code,agent_name from Agent_master where exist_code is not null and sourceid in (" & Branches & ")", MyConn, adOpenForwardOnly
    While Not rsANA.EOF
        CmbAna.AddItem rsANA(0)
        CmbAna.List(CmbAna.ListCount - 1, 1) = rsANA(1)
        rsANA.MoveNext
        'i = i + 1
    Wend
    rsANA.Close
    
    Set rsANA = Nothing
    
Call City
Call Bank
Call SCategory
cmbSource.Enabled = False
cmbrm.Enabled = False

If Glbloginid = "103578" Or Glbloginid = "41426" Or Glbloginid = "129080" Then
    LblOnlineBlock.Visible = True
    LblOfflineBlock.Visible = True
    ChkonlineBlock.Visible = True
    ChkOfflineBlock.Visible = True
Else
    LblOnlineBlock.Visible = False
    LblOfflineBlock.Visible = False
    ChkonlineBlock.Visible = False
    ChkOfflineBlock.Visible = False
End If
End Sub




Private Sub mskAffDate_LostFocus()
''mskAffDate.Text = "01" & Mid(mskAffDate.Text, 3)
End Sub

Private Sub OptSB_Click()
If OptSB.Value = True Then
cmbSource.Enabled = True
cmbrm.Enabled = True
Set rsBr = New ADODB.Recordset
 If rsBr.State = 1 Then rsBr.Close
   rsBr.CursorLocation = adUseClient
   If Trim(Branches) <> "" Then
    rsBr.open "select branch_name,branch_code from branch_master where branch_code in (" & Branches & ") order by branch_name", MyConn, adOpenDynamic, adLockReadOnly
   Else
    rsBr.open "select branch_name,branch_code from branch_master order by branch_name", MyConn, adOpenDynamic, adLockReadOnly
   End If
   cmbSource.Clear
   If rsBr.RecordCount > 0 Then
     cmbSource.Clear
     With cmbSource
         For i = 0 To rsBr.RecordCount - 1
           If Trim(rsBr(0)) <> "" Then
             .AddItem rsBr(0)
           End If
             '& "#" & rsBr(1)
             rsBr.MoveNext
         Next
     End With
   End If
  rsBr.Close
Else
cmbSource.Enabled = False
cmbrm.Enabled = False
End If
End Sub
Private Sub OptSB_KeyPress(KeyAscii As Integer)
If OptSB.Value = True Then
If KeyAscii = 13 Then
   Call OptSB_Click
  cmbSource.SetFocus
End If
  KeyAscii = 0
End If
End Sub
Private Sub Phone_KeyPress(KeyAscii As Integer)
If Phone.Row > 0 Then
 If KeyAscii = 8 Then
 If Len(Phone.TextMatrix(Phone.Row, Phone.Col)) >= 1 Then
 Phone.TextMatrix(Phone.Row, Phone.Col) = Mid(Phone.TextMatrix(Phone.Row, Phone.Col), 1, Len(Phone.TextMatrix(Phone.Row, Phone.Col)) - 1)
 End If
 Else
 If (KeyAscii < 48 Or KeyAscii > 57) Then
  KeyAscii = 0
 Else
 Phone.TextMatrix(Phone.Row, Phone.Col) = Phone.TextMatrix(Phone.Row, Phone.Col) + Chr(KeyAscii)
 End If
 End If
End If
If KeyAscii = 9 Then
     If Phone.Col < Phone.Cols Then
        Phone.Col = Phone.Col + 1
      ElseIf Phone.Row < Phone.Rows Then
        Phone.Row = Phone.Row + 1
     End If
End If
If KeyAscii = 13 Then
  txtstate.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
KeyAscii = 0
End If
End Sub
Sub Phoneno()
Ph = ""
  For i = 1 To Phone.Rows - 1
      If Trim(Phone.TextMatrix(i, 0)) <> "" Then
        Ph = Ph & "#" & (Phone.TextMatrix(i, 0) & "-" & Phone.TextMatrix(i, 1))
      End If
  Next
  If (Len(Ph) - 1) > 0 Then
  Ph = Right(Ph, (Len(Ph) - 1))
  End If
End Sub
Public Sub SubBrokerOffData()
Dim rs_get_source As ADODB.Recordset
Dim rs_get_source1 As New ADODB.Recordset
Dim rsCheck As New ADODB.Recordset
Dim rsSubType As New ADODB.Recordset
Dim MyBranchCode As String
Set rsSubBroker = New ADODB.Recordset
  
  
Dim chk_no As String
Dim Chkdt As String
Dim chbank As String

  
  
Dim B() As String
Dim bid As String

If Trim(CmbBank.Text) <> "" Then
    B = Split(Trim(CmbBank.Text), "#")
    bid = Trim(B(1))
End If


  
Dim bk() As String
Dim bkid As String

If Trim(cmbBB.Text) <> "" Then
    bk = Split(Trim(cmbBB.Text), "#")
    bkid = Trim(bk(1))
End If

If rsSubBroker.State = 1 Then rsSubBroker.Close
rsSubBroker.CursorLocation = adUseClient
rsSubBroker.open "select * from agent_master where agent_code='" & Trim(txtSCode) & "'", MyConn, adOpenDynamic, adLockPessimistic
If rsSubBroker.RecordCount > 0 Then
Dim yesup As String
yesup = MsgBox("Sub Broker already exist" & vbCrLf & "Name is : " & rsSubBroker!AGENT_NAME & vbCrLf & "Do You Want to Update", vbYesNo)
If yesup = vbYes Then
    rsCheck.open "SELECT COUNT(*) FROM AGENT_MASTER WHERE EXIST_CODE='" & cmbExistCode.Text & "' AND AGENT_CODE<>" & txtSCode.Text, MyConn, adOpenForwardOnly
    If rsCheck(0) > 0 Then
      MsgBox "This Subbroker Code Already Exists, Please Enter a Different one ", vbInformation
      If cmbExistCode.Enabled = True Then
          cmbExistCode.SetFocus
      End If
      rsCheck.Close
      Set rsCheck = Nothing
      Exit Sub
    End If
    rsCheck.Close
    Set rsCheck = Nothing

    If Trim(txtName) <> "" Then
       rsSubBroker!AGENT_NAME = Trim(txtName)
    End If
    If Trim(txtAdd1) <> "" Then
       rsSubBroker!address1 = Trim(txtAdd1)
    End If
    If Trim(txtAdd2) <> "" Then
       rsSubBroker!address2 = Trim(txtAdd2)
    End If
    rsSubBroker!address3 = Trim(txtadd3)
    
    ''ADDED BY JAWAHAR
   
   If txtr_add1.Text <> "" Then
       rsSubBroker!R_ADDRESS1 = txtr_add1.Text
   Else
       rsSubBroker!R_ADDRESS1 = ""
   End If
   If txtr_add2.Text <> "" Then
       rsSubBroker!R_ADDRESS2 = txtr_add2.Text
   Else
       rsSubBroker!R_ADDRESS2 = ""
   End If
   If cmbr_city.Text <> "" Then
        rsSubBroker!R_CITY_NAME = cmbr_city.Text
        Else
        rsSubBroker!R_CITY_NAME = ""
   End If
   If txtr_state.Text <> "" Then
       rsSubBroker!R_STATE_NAME = txtr_state.Text
   Else
       rsSubBroker!R_STATE_NAME = ""
   End If
   If txtr_pin.Text <> "" Then
       rsSubBroker!R_PINCODE = txtr_pin.Text
   Else
       rsSubBroker!R_PINCODE = ""
   End If
   
   If CHK_Online.Value = 1 Then
         rsSubBroker!online_subsciption = "Y"
   Else
         rsSubBroker!online_subsciption = "N"
   End If
   
   
'------------------------- Vinay Hatwal --------------------------------------------------------------------------------------
 '------------------------Updated on 1/May/2007 -------------------------------------------------------------------------------

 
''          If cmbPayMode.Text = "ECS" And txtMICR.Text = "" Then
''            MsgBox "First enter value for MICR Number"
''            txtMICR.SetFocus
''            Exit Sub
''          End If

If cmbPayMode.Text = "ECS" And cmbAcType.Text = "" Then
   MsgBox "Please select account Type", vbInformation, "Wealth maker"
   cmbAcType.SetFocus
   Exit Sub
End If
 
If cmbPayMode.Text = "ECS" And txtAC.Text = "" Then
   MsgBox "Please enter account Number", vbInformation, "Wealth maker"
   txtAC.SetFocus
   Exit Sub
 End If
 
 If cmbPayMode.Text = "ECS" And mskAffDate.Text = "__/__/____" Then
      MsgBox "Youn must enter the value of Affected Date"
      mskAffDate.SetFocus
      Exit Sub
 End If
 If mskAffDate.Text = "__/__/____" Then
   rsSubBroker!AFFECTEDFROM = Null
 Else
   rsSubBroker!AFFECTEDFROM = Format(Trim(mskAffDate.Text), "dd/mm/yyyy")
 End If
 If cmbPayMode.Text = "ECS" Then
    If CmbBank.Text = "" Then
      MsgBox "Youn must Select Bank."
      CmbBank.SetFocus
      Exit Sub
    End If
    If cmbbankcity.Text = "" Then
      MsgBox "Youn must Select Bank City."
      cmbbankcity.SetFocus
      Exit Sub
    End If
 
    If cmbBB.ListIndex = -1 Then
      MsgBox "Youn must Select Bank Branch."
      cmbBB.SetFocus
      Exit Sub
    End If
 End If
 '---------------------------------------------------------------------------------------------------------------------------
   
 If Trim(cmbBB.Text) <> "" Then
       rsSubBroker!Bank_branchid = bkid
 Else
       rsSubBroker!Bank_branchid = Null
 End If
   
 If chkaudit.Value = 1 Then
       rsSubBroker!ana_audit = "Y"
 Else
       rsSubBroker!ana_audit = "N"
 End If
      
 If mskauditdate.Text <> "" And mskauditdate.Text <> "__/__/____" And chkaudit.Value = 1 Then
       rsSubBroker!ana_auditDATE = mskauditdate.Text
 ElseIf chkaudit.Value = 1 Then
       rsSubBroker!ana_auditDATE = Date
 Else
       rsSubBroker!ana_auditDATE = Null
 End If
 
Set rsCity = New ADODB.Recordset
If rsCity.State = 1 Then rsCity.Close
  rsCity.CursorLocation = adUseClient
  rsCity.open "select city_id from city_master where city_name='" & Trim(cmbcity) & "'", MyConn, adOpenDynamic, adLockPessimistic
  If rsCity.RecordCount > 0 Then
        rsSubBroker!City_id = Trim(rsCity(0))
  End If
 
  Set rsLoc = New ADODB.Recordset
  If rsLoc.State = 1 Then rsLoc.Close
  rsLoc.CursorLocation = adUseClient
  rsLoc.open "select location_id from location_master where location_name='" & Trim(cmbloc) & "'", MyConn, adOpenDynamic, adLockPessimistic
  If rsLoc.RecordCount > 0 Then
     rsSubBroker!Location_ID = Trim(rsLoc(0))
  End If
  Call Phoneno
 If Trim(Ph) <> "" Then
 rsSubBroker!Phone = Trim(Ph)
 End If
 If txtMob <> "" Then
 rsSubBroker!Mobile = Trim(txtMob)
 End If
 If TxtFax <> "" Then
 rsSubBroker!fax = Trim(TxtFax)
 End If
 If Trim(TxtEmail) <> "" Then
 rsSubBroker!Email = Trim(TxtEmail)
 End If
 If Trim(txtCPer) <> "" Then
 rsSubBroker!contactper = Trim(txtCPer)
 End If
 If SbDate <> "" And SbDate <> "__/__/____" Then
   If IsNull(rsSubBroker!joiningdate) Then
     rsSubBroker!joiningdate = Trim(SbDate)
   End If
 End If
  Set RSCAT = New ADODB.Recordset
   If RSCAT.State = 1 Then RSCAT.Close
    RSCAT.CursorLocation = adUseClient
    RSCAT.open "select category from agent_category where name='" & Trim(cmbSCat) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If RSCAT.RecordCount > 0 Then
      rsSubBroker!category_id = Trim(RSCAT(0))
    End If
    Set rsStatus = New ADODB.Recordset
      If rsStatus.State = 1 Then rsStatus.Close
       rsStatus.CursorLocation = adUseClient
        rsStatus.open "select branch_code from branch_master where branch_name='" & Trim(cmbSource) & "'", MyConn, adOpenForwardOnly, adLockReadOnly
           If rsStatus.RecordCount > 0 Then
            If rsStatus(0) <> "" Then
                 rsSubBroker!SourceId = Trim(rsStatus(0))
                 MyBranchCode = Trim(rsStatus(0))
            End If
           End If
 If dob <> "" And dob <> "__/__/____" Then
 rsSubBroker!dob = Trim(dob)
 End If
 
 
''          ''''''''''''''''''''''''''''''''POSP DETAIL UPDATE''''''''''''''''''''''''''''''''''''''''''''''
If pospcertifieddateLI <> "" And pospcertifieddateLI <> "__/__/____" Then
   rsSubBroker!POSP_CERTIFIED_ON_LI = Trim(pospcertifieddateLI)
Else
   rsSubBroker!POSP_CERTIFIED_ON_LI = Null
End If

If pospcertifieddateGI <> "" And pospcertifieddateGI <> "__/__/____" Then
   rsSubBroker!POSP_CERTIFIED_ON_GI = Trim(pospcertifieddateGI)
Else
   rsSubBroker!POSP_CERTIFIED_ON_GI = Null
End If

If PospValidTillLI <> "" And PospValidTillLI <> "__/__/____" Then
   rsSubBroker!POSP_VALID_TILL_LI = Trim(PospValidTillLI)
Else
   rsSubBroker!POSP_VALID_TILL_LI = Null
End If

If PospValidTillGI <> "" And PospValidTillGI <> "__/__/____" Then
   rsSubBroker!POSP_VALID_TILL_GI = Trim(PospValidTillGI)
Else
   rsSubBroker!POSP_VALID_TILL_GI = Null
End If

If txtaadharcardno.Text <> "" Then
   rsSubBroker!AADHAR_CARD_NO = Trim(txtaadharcardno.Text)
End If

If TxtGstinno.Text <> "" Then
   rsSubBroker!GSTIN_NO = Trim(TxtGstinno.Text)
End If

If CMBPOSPMARKING.Text <> "" Then
   rsSubBroker!POSP_MARKING = Trim(CMBPOSPMARKING.Text)
End If

If CMBPOSPTYPE.Text <> "" Then
   rsSubBroker!Posp_type = Trim(CMBPOSPTYPE.Text)
End If

If TXTPOSPNOLI.Text <> "" Then
   rsSubBroker!POSP_NO_LI = Trim(TXTPOSPNOLI.Text)
Else
    rsSubBroker!POSP_NO_LI = Null
End If

If TXTPOSPNOGI.Text <> "" Then
   rsSubBroker!POSP_NO_GI = Trim(TXTPOSPNOGI.Text)
Else
   rsSubBroker!POSP_NO_GI = Null
End If


rsSubBroker!VERIFIED_STATUS = VpospVerified
rsSubBroker!VERIFIED_BY = VpospVerifiedBy
If VpospVerifiedDate = "01-JAN-1850" Then
    rsSubBroker!VERIFIED_DATE = Null
Else
    rsSubBroker!VERIFIED_DATE = VpospVerifiedDate
End If

If ChkonlineBlock.Value = 1 Then
    rsSubBroker!ONLINE_BLOCK_AGENT = 1
Else
    rsSubBroker!ONLINE_BLOCK_AGENT = 0
End If
    

If ChkOfflineBlock.Value = 1 Then
    rsSubBroker!BLOCK_AGENT = 1
Else
    rsSubBroker!BLOCK_AGENT = 0
End If
    


''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
   Set rsStatus = New ADODB.Recordset
   If rsStatus.State = 1 Then rsStatus.Close
    rsStatus.CursorLocation = adUseClient
    rsStatus.open "select itemserialnumber from fixeditem where itemname='" & Trim(CmbStatus) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsStatus.RecordCount > 0 Then
     If rsStatus(0) <> "" Then
      rsSubBroker!statusid = Trim(rsStatus(0))
     End If
    End If
 If txtpan <> "" Then
 rsSubBroker!pan = Trim(txtpan)
 End If
 If Trim(cmbDist) <> "" Then
 rsSubBroker!dist = Trim(cmbDist)
 End If
 
  If chkPassed = 1 Then
     pass = "Y"
  Else
     pass = "N"
  End If
 If Trim(pass) <> "" Then
 rsSubBroker!amficert = Trim(pass)
 End If
  Set rsEx = New ADODB.Recordset
   If rsEx.State = 1 Then rsEx.Close
    rsEx.CursorLocation = adUseClient
    rsEx.open "select itemserialnumber from fixeditem where itemname='" & Trim(cmbExams) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsEx.RecordCount > 0 Then
     If rsEx(0) <> "" Then
      rsSubBroker!amfiextypeid = Trim(rsEx(0))
     End If
    End If
 If txtID <> "" Then
    rsSubBroker!amfiid = Trim(txtID)
 End If
 
 If Trim(txtCPEmail) <> "" Then
 rsSubBroker!cpemailid = Trim(txtCPEmail)
 End If


If CmbBank.Text <> "" And cmbBB.Text <> "" Then
  Set rsBank = New ADODB.Recordset
   If rsBank.State = 1 Then rsBank.Close
    rsBank.open "select bank_id from ANA_bank_master where bank_id='" & bid & "'", MyConn, adOpenDynamic, adLockPessimistic  ''and branch='" & Trim(Mid(Trim(cmbBB), 1, Len(Trim(cmbBB)) - 3)) & "'"
    If rsBank.RecordCount > 0 Then
     If rsBank(0) <> "" Then
      rsSubBroker!bankid = Trim(rsBank(0))
     End If
    End If
  End If
  
  Set rsAtype = New ADODB.Recordset
   If rsAtype.State = 1 Then rsAtype.Close
    rsAtype.CursorLocation = adUseClient
    rsAtype.open "select itemserialnumber from fixeditem where itemname='" & Trim(cmbAcType) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsAtype.RecordCount > 0 Then
     If rsAtype(0) <> "" Then
      rsSubBroker!acctypeid = Trim(rsAtype(0))
     End If
    End If
 If Trim(txtWebSite) <> "" Then
 rsSubBroker!website = Trim(txtWebSite)
 End If
  Set rsStatus = New ADODB.Recordset
      If rsStatus.State = 1 Then rsStatus.Close
       rsStatus.CursorLocation = adUseClient
        rsStatus.open "select rm_code from employee_master where trim(RM_name)='" & Trim(cmbrm) & "' and source=" & MyBranchCode & "", MyConn, adOpenForwardOnly, adLockReadOnly
           If rsStatus.RecordCount > 0 Then
            If rsStatus(0) <> "" Then
                 rsSubBroker!RM_CODE = Trim(rsStatus(0))
            End If
           End If
  Set rsPmode = New ADODB.Recordset
   If rsPmode.State = 1 Then rsPmode.Close
    rsPmode.CursorLocation = adUseClient
    rsPmode.open "select itemserialnumber from fixeditem where itemname='" & Trim(cmbPayMode) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsPmode.RecordCount > 0 Then
     If rsPmode(0) <> "" Then
      rsSubBroker!paymentmodeid = Trim(rsPmode(0))
     End If
    End If
 If Trim(txtRem) <> "" Then
 rsSubBroker!remark = Trim(txtRem)
 End If
 If Trim(txtAC) <> "" Then
 rsSubBroker!accno = Trim(txtAC)
 End If
 If Trim(cmbExistCode) <> "" Then
 rsSubBroker!exist_code = Trim(cmbExistCode)
 End If
 If txtSbCode.Text = "" Then
        txtSbCode.Text = Trim(txtSCode.Text)
 End If
 If Trim(txtSbCode) <> "" Then
 rsSubBroker!agent_source = Trim(txtSbCode)
 End If
 If Trim(txtTds) <> "" Then
 rsSubBroker!tds = Trim(txtTds)
 End If
 If Trim(txtpin) <> "" Then
 rsSubBroker!pincode = Trim(txtpin)
 End If
 
 If Trim(cmbstype.Text) <> "" Then
   rsSubType.open "Select Investor_code from investortype where investor_type='" & Trim(cmbstype.Text) & "'", MyConn, adOpenForwardOnly
   rsSubBroker!sub_broker_type = rsSubType(0)
 End If
 
 rsSubType.Close
 Set rsSubType = Nothing
 
 If CmbAna.ListIndex <> -1 Then
   rsSubBroker!Master_ana = CmbAna.List(CmbAna.ListIndex, 0)
 End If
 
 rsSubBroker!MODIFY_USER = Trim(Glbloginid)
 rsSubBroker!MODIFY_DATE = ServerDateTime

 rsSubBroker.Update
 rsSubBroker.Requery
 MsgBox "Data Successfully Updated", vbInformation, "Update"
 Call CMDRESET_Click
End If
Else
 rsCheck.open "select count(*) from agent_master where exist_code='" & Trim(cmbExistCode.Text) & "'", MyConn, adOpenForwardOnly
 If rsCheck(0) > 0 Then
   MsgBox "This Subbroker Code Already Exists ", vbInformation
   If cmbExistCode.Enabled = True Then
   cmbExistCode.SetFocus
   End If
   rsCheck.Close
   Set rsCheck = Nothing
   Exit Sub
 End If
   rsCheck.Close
   Set rsCheck = Nothing
 
 rsSubBroker.AddNew
 If Trim(txtName) <> "" Then
 rsSubBroker!AGENT_NAME = Trim(txtName)
 End If
 If Trim(txtAdd1) <> "" Then
 rsSubBroker!address1 = Trim(txtAdd1)
 End If
 If Trim(txtadd3) <> "" Then
 rsSubBroker!address3 = Trim(txtadd3)
 End If
 If Trim(txtAdd2) <> "" Then
 rsSubBroker!address2 = Trim(txtAdd2)
 
 
  ''ADDED BY JAWAHAR
   
  If txtr_add1.Text <> "" Then
       rsSubBroker!R_ADDRESS1 = txtr_add1.Text
       Else
       rsSubBroker!R_ADDRESS1 = ""
   End If
   If txtr_add2.Text <> "" Then
       rsSubBroker!R_ADDRESS2 = txtr_add2.Text
       Else
       rsSubBroker!R_ADDRESS2 = ""
   End If
   If cmbr_city.Text <> "" Then
        rsSubBroker!R_CITY_NAME = cmbr_city.Text
        Else
        rsSubBroker!R_CITY_NAME = ""
   End If
   If txtr_state.Text <> "" Then
       rsSubBroker!R_STATE_NAME = txtr_state.Text
       Else
       rsSubBroker!R_STATE_NAME = ""
   End If
   If txtr_pin.Text <> "" Then
       rsSubBroker!R_PINCODE = txtr_pin.Text
       Else
       rsSubBroker!R_PINCODE = ""
   End If
 ''END
 
 
 
 End If
  Set rsCity = New ADODB.Recordset
   If rsCity.State = 1 Then rsCity.Close
    rsCity.CursorLocation = adUseClient
    rsCity.open "select city_id from city_master where city_name='" & Trim(cmbcity) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsCity.RecordCount > 0 Then
     If rsCity(0) <> "" Then
      rsSubBroker!City_id = Trim(rsCity(0))
     End If
     rsCity.Close
    End If
 Set rsLoc = New ADODB.Recordset
   If rsLoc.State = 1 Then rsLoc.Close
    rsLoc.CursorLocation = adUseClient
    rsLoc.open "select location_id from location_master where location_name='" & Trim(cmbloc) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsLoc.RecordCount > 0 Then
       rsSubBroker!Location_ID = Trim(rsLoc(0))
    End If
  Call Phoneno
 If Trim(Ph) <> "" Then
 rsSubBroker!Phone = Trim(Ph)
 End If
 If txtMob <> "" Then
 rsSubBroker!Mobile = Trim(txtMob)
 End If
 If TxtFax <> "" Then
 rsSubBroker!fax = Trim(TxtFax)
 End If
 If Trim(TxtEmail) <> "" Then
 rsSubBroker!Email = Trim(TxtEmail)
 End If
 If Trim(txtCPer) <> "" Then
 rsSubBroker!contactper = Trim(txtCPer)
 End If
 If SbDate <> "" And SbDate <> "__/__/____" Then
 rsSubBroker!joiningdate = Trim(SbDate)
 End If
 
''          ''''''''''''''''''''''''''''''''POSP DETAIL UPDATE''''''''''''''''''''''''''''''''''''''''''''''
If pospcertifieddateLI <> "" And pospcertifieddateLI <> "__/__/____" Then
   rsSubBroker!POSP_CERTIFIED_ON_LI = Trim(pospcertifieddateLI)
End If

If pospcertifieddateGI <> "" And pospcertifieddateGI <> "__/__/____" Then
   rsSubBroker!POSP_CERTIFIED_ON_GI = Trim(pospcertifieddateGI)
End If

If PospValidTillLI <> "" And PospValidTillLI <> "__/__/____" Then
   rsSubBroker!POSP_VALID_TILL_LI = Trim(PospValidTillLI)
End If

If PospValidTillGI <> "" And PospValidTillGI <> "__/__/____" Then
   rsSubBroker!POSP_VALID_TILL_GI = Trim(PospValidTillGI)
End If

If txtaadharcardno.Text <> "" Then
   rsSubBroker!AADHAR_CARD_NO = Trim(txtaadharcardno.Text)
End If

If CMBPOSPMARKING.Text <> "" Then
   rsSubBroker!POSP_MARKING = Trim(CMBPOSPMARKING.Text)
End If

If CMBPOSPTYPE.Text <> "" Then
   rsSubBroker!Posp_type = Trim(CMBPOSPTYPE.Text)
End If

If TXTPOSPNOLI.Text <> "" Then
   rsSubBroker!POSP_NO_LI = Trim(TXTPOSPNOLI.Text)
End If

If TXTPOSPNOGI.Text <> "" Then
   rsSubBroker!POSP_NO_GI = Trim(TXTPOSPNOGI.Text)
End If

'
'         '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

  Set RSCAT = New ADODB.Recordset
   If RSCAT.State = 1 Then RSCAT.Close
    RSCAT.CursorLocation = adUseClient
    RSCAT.open "select category from agent_category where name='" & Trim(cmbSCat) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If RSCAT.RecordCount > 0 Then
     If RSCAT(0) <> "" Then
      rsSubBroker!category_id = Trim(RSCAT(0))
     End If
     RSCAT.Close
    End If
     Set rsStatus = New ADODB.Recordset
      If rsStatus.State = 1 Then rsStatus.Close
       rsStatus.CursorLocation = adUseClient
        rsStatus.open "select branch_code from branch_master where branch_name='" & Trim(cmbSource) & "'", MyConn, adOpenForwardOnly, adLockReadOnly
           If rsStatus.RecordCount > 0 Then
            If rsStatus(0) <> "" Then
                 rsSubBroker!SourceId = Trim(rsStatus(0))
                 MyBranchCode = Trim(rsStatus(0))
            End If
           End If
 If dob <> "" And dob <> "__/__/____" Then
 rsSubBroker!dob = Trim(dob)
 End If
  Set rsStatus = New ADODB.Recordset
   If rsStatus.State = 1 Then rsStatus.Close
    rsStatus.CursorLocation = adUseClient
    rsStatus.open "select itemserialnumber from fixeditem where itemname='" & Trim(CmbStatus) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsStatus.RecordCount > 0 Then
     If rsStatus(0) <> "" Then
      rsSubBroker!statusid = Trim(rsStatus(0))
     End If
    End If
 If txtpan <> "" Then
 rsSubBroker!pan = Trim(txtpan)
 End If
 If Trim(cmbDist) <> "" Then
 rsSubBroker!dist = Trim(cmbDist)
 End If
  If chkPassed = 1 Then
     pass = "Y"
  Else
     pass = "N"
  End If
 If Trim(pass) <> "" Then
 rsSubBroker!amficert = Trim(pass)
 End If
  Set rsEx = New ADODB.Recordset
   If rsEx.State = 1 Then rsEx.Close
    rsEx.CursorLocation = adUseClient
    rsEx.open "select itemserialnumber from fixeditem where itemname='" & Trim(cmbExams) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsEx.RecordCount > 0 Then
     If rsEx(0) <> "" Then
      rsSubBroker!amfiextypeid = Trim(rsEx(0))
     End If
    End If
 If txtID <> "" Then
 rsSubBroker!amfiid = Trim(txtID)
 End If
 If Trim(txtCPEmail) <> "" Then
 rsSubBroker!cpemailid = Trim(txtCPEmail)
 End If
 
 
  If CmbBank.Text <> "" And cmbBB.Text <> "" Then
  Set rsBank = New ADODB.Recordset
   If rsBank.State = 1 Then rsBank.Close
    rsBank.CursorLocation = adUseClient
    rsBank.open "select bank_id from ANA_bank_master where bank_name='" & Trim(Mid(Trim(CmbBank), 1, (Len(Trim(CmbBank)) - 3))) & "' and branch='" & Trim(Mid(Trim(cmbBB), 1, Len(Trim(cmbBB)) - 3)) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsBank.RecordCount > 0 Then
     If rsBank(0) <> "" Then
      rsSubBroker!bankid = Trim(rsBank(0))
     End If
    End If
  End If
    
    
  Set rsAtype = New ADODB.Recordset
   If rsAtype.State = 1 Then rsAtype.Close
    rsAtype.CursorLocation = adUseClient
    rsAtype.open "select itemserialnumber from fixeditem where itemname='" & Trim(cmbAcType) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsAtype.RecordCount > 0 Then
     If rsAtype(0) <> "" Then
      rsSubBroker!acctypeid = Trim(rsAtype(0))
     End If
    End If
 If Trim(txtWebSite) <> "" Then
 rsSubBroker!website = Trim(txtWebSite)
 End If
 If Trim(cmbrm) <> "" Then
     Set rsStatus = New ADODB.Recordset
      If rsStatus.State = 1 Then rsStatus.Close
       rsStatus.CursorLocation = adUseClient
        rsStatus.open "select rm_code from employee_master where trim(RM_name)='" & Trim(cmbrm) & "' and source=" & MyBranchCode & "", MyConn, adOpenForwardOnly, adLockReadOnly
           If rsStatus.RecordCount > 0 Then
            If rsStatus(0) <> "" Then
                 rsSubBroker!RM_CODE = Trim(rsStatus(0))
            End If
           End If
 End If
  Set rsPmode = New ADODB.Recordset
   If rsPmode.State = 1 Then rsPmode.Close
    rsPmode.CursorLocation = adUseClient
    rsPmode.open "select itemserialnumber from fixeditem where itemname='" & Trim(cmbPayMode) & "'", MyConn, adOpenDynamic, adLockPessimistic
    If rsPmode.RecordCount > 0 Then
     If Trim(rsPmode(0)) <> "" Then
      rsSubBroker!paymentmodeid = Trim(rsPmode(0))
     End If
    End If
 If Trim(txtRem) <> "" Then
 rsSubBroker!remark = Trim(txtRem)
 End If
 If Trim(txtAC) <> "" Then
 rsSubBroker!accno = Trim(txtAC)
 End If
 If Trim(cmbExistCode) <> "" Then
 rsSubBroker!exist_code = Trim(cmbExistCode)
 End If
 If Trim(txtTds) <> "" Then
 rsSubBroker!tds = Trim(txtTds)
 End If
 If Trim(txtSbCode) <> "" Then
 rsSubBroker!agent_source = Trim(txtSbCode)
 End If
 If Trim(txtpin) <> "" Then
 rsSubBroker!pincode = Trim(txtpin)
 End If
 
 If Trim(cmbstype.Text) <> "" Then
   rsSubType.open "Select Investor_code from investortype where investor_type='" & Trim(cmbstype.Text) & "'", MyConn, adOpenForwardOnly
   rsSubBroker!sub_broker_type = rsSubType(0)
   rsSubType.Close
    Set rsSubType = Nothing
End If
 
 
 

 
 rsSubBroker!loggeduserid = Trim(Glbloginid)
            rsSubBroker!MODIFY_USER = Trim(Glbloginid)
 rsSubBroker!MODIFY_DATE = ServerDateTime
 
   '------------------------- Vinay Hatwal --------------------------------------------------------------------------------------
 '------------------------Updated on 1/May/2007 -------------------------------------------------------------------------------

 
''          If cmbPayMode.Text = "ECS" And txtMICR.Text = "" Then
''            MsgBox "First enter value for MICR Number"
''            txtMICR.SetFocus
''            Exit Sub
''          End If
''          If txtMICR.Text = "" Then
''            rsSubBroker!bank_micr_no = ""
''            Else
''            rsSubBroker!bank_micr_no = Trim(txtMICR.Text)
''          End If



If cmbPayMode.Text = "ECS" And cmbAcType.Text = "" Then
   MsgBox "Please select account Type", vbInformation, "Wealth maker"
   cmbAcType.SetFocus
   Exit Sub
 End If
If cmbPayMode.Text = "ECS" And txtAC.Text = "" Then
   MsgBox "Please enter account Number", vbInformation, "Wealth maker"
   txtAC.SetFocus
   Exit Sub
 End If

 If cmbPayMode.Text = "ECS" And mskAffDate.Text = "__/__/____" Then
   MsgBox "You must enter the value of Affected Date"
   mskAffDate.SetFocus
   Exit Sub
   
 End If
   If mskAffDate.Text = "__/__/____" Then
   rsSubBroker!AFFECTEDFROM = Null
   Else
   rsSubBroker!AFFECTEDFROM = Format(Trim(mskAffDate.Text), "dd/mm/yyyy")
 End If
 
 '---------------------------------------------------------------------------------------------------------------------------
   
   If Trim(cmbBB.Text) <> "" Then
       rsSubBroker!Bank_branchid = bkid
       Else
       rsSubBroker!Bank_branchid = Null
   End If
   
   If chkaudit.Value = 1 Then
       rsSubBroker!ana_audit = "Y"
       Else
       rsSubBroker!ana_audit = "N"
   End If
   
   If mskauditdate.Text <> "" And mskauditdate.Text <> "__/__/____" And chkaudit.Value = 1 Then
       rsSubBroker!ana_auditDATE = mskauditdate.Text
       ElseIf chkaudit.Value = 1 Then
       rsSubBroker!ana_auditDATE = Date
       Else
       rsSubBroker!ana_auditDATE = Null
   End If

   If CmbAna.ListIndex <> -1 Then
     rsSubBroker!Master_ana = CmbAna.List(CmbAna.ListIndex, 0)
   End If


 '---------------------------------------------------------------------------------------------------------------------------
 
 If CmbType.Text <> "PAID" Then
   rsSubBroker!exist_code = "FREE"
 End If
 
 
 rsSubBroker.Update
   If Trim(txtSbCode.Text) = "" Then
       Set rs_get_source = MyConn.Execute("select MAX(agent_code) from agent_master")
       sbcode = rs_get_source(0)
       txtSbCode = rs_get_source(0)
       Set rs_get_source = Nothing
   End If
 
 If ANACommingFromANASUBENT = "Yes" Then
   Dim re_local_dt As New ADODB.Recordset
   Dim str_test As String
   Dim ANACode_Local As String
   Dim pACK_Local As String
   
   Set rs_get_source = MyConn.Execute("select agent_code,rm_code,SOURCEID,AGENT_NAME,ADDRESS1,ADDRESS2,CITY_ID,PHONE,MOBILE,EMAIL,DOB,PAN from agent_master where agent_coDe=(select MAX(agent_code) from agent_master)")
   MyConn.Execute ("update ADVISORSUBENTRY set ANACODE='" & rs_get_source(0) & "' ,ANABRANCH='" & rs_get_source(2) & "',ANARM='" & rs_get_source(1) & "' WHERE UNIQUENO='" & my_TBLUID & "'")
   
   
   '------------------------------------------------------- VINAY HATWAL --------------------------------------------------------------------
   '----- DT.
   ' Auto Client Creation (Same as agent)
   str_test = " insert into client_master (CLIENT_NAME,ADDRESS1,ADDRESS2,CITY_ID,PHONE,EMAIL,PAN,RM_CODE,SOURCEID,CATEGORY_ID,CREATION_DATE,DOB,MOBILE,"
   str_test = str_test & " MODIFY_USER,MODIFY_DATE) Values "
   str_test = str_test & " ('" & rs_get_source("AGENT_NAME") & "','" & rs_get_source("ADDRESS1") & "','" & rs_get_source("ADDRESS2") & "','" & rs_get_source("CITY_ID") & "','" & rs_get_source("PHONE")
   str_test = str_test & "','" & rs_get_source("EMAIL") & "','" & rs_get_source("PAN") & "','" & rs_get_source("rm_code")
   str_test = str_test & "','" & rs_get_source("SOURCEID") & "','4003',sysdate,"
   str_test = str_test & " to_date('" & Format(rs_get_source("DOB"), "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & rs_get_source("MOBILE") & "','Auto',sysdate);"
   MyConn.Execute str_test
   
   Set re_local_dt = MyConn.Execute("select Max(client_code) from client_master  where client_name='" & rs_get_source("AGENT_NAME") & "'")
   ANACode_Local = re_local_dt(0) & "001"
   
   
   MyConn.Execute "update ADVISORSUBENTRY set client_code='" & ANACode_Local & "' WHERE UNIQUENO='" & my_TBLUID & "'"
   
   'MF Server
   str_test = " insert into client_master@mf.bajajcapital (client_code,CLIENT_NAME,ADDRESS1,ADDRESS2,CITY_ID,PHONE,EMAIL,PAN,RM_CODE,SOURCEID,CATEGORY_ID,CREATION_DATE,DOB,MOBILE,"
   str_test = str_test & " MODIFY_USER,MODIFY_DATE) Values "
   str_test = str_test & " ('" & re_local_dt(0) & "','" & rs_get_source("AGENT_NAME") & "','" & rs_get_source("ADDRESS1") & "','" & rs_get_source("ADDRESS2") & "','" & rs_get_source("CITY_ID") & "','" & rs_get_source("PHONE")
   str_test = str_test & "','" & rs_get_source("EMAIL") & "','" & rs_get_source("PAN") & "','" & rs_get_source("rm_code")
   str_test = str_test & "','" & rs_get_source("SOURCEID") & "','4003',sysdate,"
   str_test = str_test & " to_date('" & Format(rs_get_source("DOB"), "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & rs_get_source("MOBILE") & "','Auto',sysdate);"
   MyConn.Execute str_test
   
   ' Auto Investor Creation (Same as agent)
   str_test = " insert into investor_master (investor_code,INV_CODE,INVESTOR_NAME,ADDRESS1,ADDRESS2,CITY_ID,PHONE,EMAIL,PAN,SOURCE_ID,RM_CODE,BRANCH_CODE,"
   str_test = str_test & " MOBILE,DOB,LOGGEDUSERID,TIMEST)"
   str_test = str_test & " Values ('1','"
   str_test = str_test & ANACode_Local & "','" & rs_get_source("AGENT_NAME") & "','" & rs_get_source("ADDRESS1") & "',"
   str_test = str_test & "'" & rs_get_source("ADDRESS2") & "','" & rs_get_source("CITY_ID") & "','" & rs_get_source("PHONE")
   str_test = str_test & "','" & rs_get_source("EMAIL") & "',"
   str_test = str_test & "'" & rs_get_source("PAN") & "',(select Max(client_code) from client_master),'" & rs_get_source("rm_code") & "',"
   str_test = str_test & "'" & rs_get_source("SOURCEID") & "','" & rs_get_source("MOBILE") & "',to_date('" & Format(rs_get_source("DOB"), "dd/MM/yyyy") & "','dd/MM/yyyy'),"
   str_test = str_test & "'Auto',sysdate);"
   MyConn.Execute str_test
   
    ' Auto Investor Creation (Same as agent)
   str_test = " insert into investor_master@mf.bajajcapital (investor_code,INV_CODE,INVESTOR_NAME,ADDRESS1,ADDRESS2,CITY_ID,PHONE,EMAIL,PAN,SOURCE_ID,RM_CODE,BRANCH_CODE,"
   str_test = str_test & " MOBILE,DOB,LOGGEDUSERID,TIMEST)"
   str_test = str_test & " Values ('1','"
   str_test = str_test & ANACode_Local & "','" & rs_get_source("AGENT_NAME") & "','" & rs_get_source("ADDRESS1") & "',"
   str_test = str_test & "'" & rs_get_source("ADDRESS2") & "','" & rs_get_source("CITY_ID") & "','" & rs_get_source("PHONE")
   str_test = str_test & "','" & rs_get_source("EMAIL") & "',"
   str_test = str_test & "'" & rs_get_source("PAN") & "',(select Max(client_code) from client_master),'" & rs_get_source("rm_code") & "',"
   str_test = str_test & "'" & rs_get_source("SOURCEID") & "','" & rs_get_source("MOBILE") & "',to_date('" & Format(rs_get_source("DOB"), "dd/MM/yyyy") & "','dd/MM/yyyy'),"
   str_test = str_test & "'Auto',sysdate);"
   MyConn.Execute str_test
   
   ' Auto AR Creation
   Set rs_get_source = Nothing
   Set rs_get_source = MyConn.Execute("select * from ADVISORSUBENTRY where UNIQUENO='" & my_TBLUID & "'")
   
   If rs_get_source("BUSINESSPACK") = "SILVER" Then
       pACK_Local = "OP#09422"
   ElseIf rs_get_source("BUSINESSPACK") = "GOLD" Then
       pACK_Local = "OP#09423"
   ElseIf rs_get_source("BUSINESSPACK") = "PLATINUM" Then
       pACK_Local = "OP#09424"
   Else
       pACK_Local = "0"
   End If
   
   chk_no = IIf(IsNull(rs_get_source("paychqno")), "", rs_get_source("paychqno"))
   Chkdt = IIf(IsNull(rs_get_source("CHQDATE")), "", Format(CDate(rs_get_source("CHQDATE")), "dd/MM/yyyy"))
   chbank = IIf(IsNull(rs_get_source("BANKNAME")), "", rs_get_source("BANKNAME"))
   
   
   str_test = " insert into transaction_sttemp"
   str_test = str_test & " (INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,APP_NO,TRAN_TYPE,AMOUNT,BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,BANK_NAME)"
   str_test = str_test & " Values"
   str_test = str_test & " ('1',to_date('" & Format(rs_get_source("TRANSACTIONDATE"), "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & ANACode_Local & "',"
   str_test = str_test & " 'IS02216','" & pACK_Local & "',null,'PURCHASE','" & rs_get_source("AMOUNT") & "','" & rs_get_source("ANABRANCH") & "',"
   str_test = str_test & " lpad('" & ANACode_Local & "',8),'" & rs_get_source("ANARM") & "',(select max(payroll_id) from employee_master where rm_code='" & rs_get_source("ANARM") & "'),"
   str_test = str_test & " '" & rs_get_source("ANABRANCH") & "','" & chk_no & "',to_date('" & Chkdt & "','dd/MM/yyyy'),'" & chbank & "')"

   MyConn.Execute str_test
   
   ' Getting newly generated AR
   Set rs_get_source = Nothing
   Set rs_get_source = MyConn.Execute("select tran_code from transaction_sttemp where client_code ='" & ANACode_Local & "'")
   '----------Update Trancode-------------------------------------
   If Not rs_get_source.EOF Then
       Set rs_get_source1 = MyConn.Execute("update advisorsubentry set tran_code='" & rs_get_source(0) & "' where client_code ='" & ANACode_Local & "'")
   End If
   '-----------------------------------------------------------------
   
   'Calculating Recievable of AR
   Dim recd_paid As New ADODB.Command
   Set recd_paid.ActiveConnection = MyConn
   recd_paid.CommandType = adCmdStoredProc
   recd_paid.CommandText = "Recd_paid_update"
   recd_paid.Parameters.Append recd_paid.CreateParameter("tr_code", adVarChar, adParamInput, 50, rs_get_source(0))
   recd_paid.Execute
       
   'End ------------------------------------------------------- VINAY HATWAL --------------------------------------------------------------------
 End If
 MsgBox "Data Successfully Saved", vbInformation, "Save"
 'If ANACommingFromANASUBENT = "Yes" Then
   'If Trim(txtSbCode.Text) = "" Then
         Set rs_get_source = MyConn.Execute("select exist_code from agent_master where agent_code in (select MAX(agent_code) from agent_master)")
         cmbExistCode.Text = rs_get_source(0)
         Set rs_get_source = Nothing
         cmbExistCode.Enabled = True
         cmbExistCode.ForeColor = vbRed
         cmbExistCode.Locked = True
         
   'End If
 'Else
  'Call CmdReset_Click
 'End If
End If
End Sub
Private Sub Phone_KeyUp(KeyCode As Integer, Shift As Integer)
On Error Resume Next
Dim i As Integer
For i = 1 To Phone.Rows - 1
    If Len(Phone.TextMatrix(i, 0)) > 5 Then
      Phone.TextMatrix(Phone.Row, Phone.Col) = Left(Phone.Text, 5)
      Phone.SetFocus
    End If
    If Len(Phone.TextMatrix(i, 1)) > 10 Then
      MsgBox "Phone Number Should not Exceed 10 Digits ", vbInformation
      Phone.TextMatrix(Phone.Row, Phone.Col) = Left(Phone.Text, 10)
      Phone.SetFocus
    End If
Next
End Sub

Private Sub Picture1_Click(Index As Integer)
Dim i As Integer

Set frmtree_search.currentForm = Me
frmtree_search.treeName = ""
frmtree_search.Show
DoEvents

frmtree_search.Cmbcat.Text = "AGENT"
frmtree_search.Cmbcat.Enabled = False


frmtree_search.ZOrder
End Sub

Private Sub Picture7_Click(Index As Integer)
On Error GoTo err1
Dim openfile As String, ARL As String, FL As String
Dim strFile As String
Dim strPath As String
Dim strFiletype() As String
Dim l_file As String
Dim rsGetDocPath As New ADODB.Recordset
Dim bk() As String
Dim bkid As String

    If Trim(CmbDocument.Text) <> "" Then
        bk = Split(Trim(CmbDocument.Text), "#")
        bkid = Trim(bk(1))
    End If
    
    
    
    Me.MousePointer = 11
    rsGetDocPath.open "select doc_filename,doc_path from tb_doc_upload where COMMON_ID='" & bkid & "' AND exist_code='" & cmbExistCode.Text & "' and tran_sub_type ='" & Trim(Mid(CmbDocument.Text, 1, 20)) & "' and rownum=1", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
        strFile = rsGetDocPath("doc_filename")
        strPath = UCase(rsGetDocPath("doc_path"))
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
    
    ReDim strFiletype(1)
    strFiletype = Split(strFile, ".")
    l_file = "c:\doc\doc" & Glbloginid & "." & LCase(strFiletype(1))
    
    If FileExists(l_file) = True And OldViewedSubbroker <> cmbExistCode.Text Then
        Delete_File (l_file)
    End If
    Generate_File strPath, strFile, l_file
    
    ShellExecute Me.hwnd, vbNullString, "c:\doc\ftp1.bat", vbNullString, "c:", SW_SHOWNORMAL
    For i = 1 To 1000
        DoEvents
    Next i
    ShellExecute Me.hwnd, vbNullString, l_file, vbNullString, "c:", SW_SHOWNORMAL
    
    Me.MousePointer = 0
    
    OldViewedSubbroker = cmbExistCode.Text
    Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
End Sub


Private Sub SbDate_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtRem.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub SbDate_LostFocus()
If SbDate <> "" And SbDate <> "__/__/____" Then
  If Not IsValidDateString(SbDate, e1or2D_1or2M_2or4Y) Then
   MsgBox "Plz.. Insert Valid Date !", vbCritical, "Alert"
   SbDate.SetFocus
   Exit Sub
  End If
End If
End Sub



Private Sub txtaadharcardno_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  CmdOk.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub

Private Sub txtAC_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbAcType.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub txtadd1_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtAdd2.SetFocus
End If
End Sub
Private Sub txtAdd2_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbcity.SetFocus
End If
End Sub
Private Sub txtCPEmail_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbSCat.SetFocus
End If
End Sub
Private Sub txtCPEmail_LostFocus()
     If Trim(txtCPEmail.Text) <> "" Then
         st1 = InStr(1, txtCPEmail.Text, "@", vbTextCompare)
     If st1 <= 1 Then
         MsgBox "Please enter correct E-mail ID     ", vbInformation
         txtCPEmail.SetFocus
         Exit Sub
     End If
         st2 = InStr(1, txtCPEmail.Text, ".", vbTextCompare)
     If st2 <= 1 Then
         MsgBox "Please enter correct E-mail ID     ", vbInformation
         txtCPEmail.SetFocus
         Exit Sub
     End If
     End If
End Sub
Private Sub txtCPer_KeyPress(KeyAscii As Integer)
 If KeyAscii = 13 Then
        txtPh.SetFocus
     End If
     'KeyAscii = 0
If (KeyAscii > 48 And KeyAscii < 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub txtEMail_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtTds.SetFocus
End If
'KeyAscii = 0
End Sub
Private Sub txtEmail_LostFocus()
     If Trim(TxtEmail.Text) <> "" Then
         st1 = InStr(1, TxtEmail.Text, "@", vbTextCompare)
     If st1 <= 1 Then
         MsgBox "Please enter correct E-mail ID     ", vbInformation
         TxtEmail.SetFocus
         Exit Sub
     End If
             st2 = InStr(1, TxtEmail.Text, ".", vbTextCompare)
     If st2 <= 1 Then
         MsgBox "Please enter correct E-mail ID     ", vbInformation
         TxtEmail.SetFocus
         Exit Sub
     End If
     End If
End Sub
Private Sub txtFax_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  TxtEmail.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub txtID_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  chkPassed.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub txtID_Validate(Cancel As Boolean)
If Trim(txtID.Text) = "" Then
   Cancel = True
   MsgBox "Please Insert Reg. No.", vbCritical, "Alert"
End If
If Cancel = True Then
 txtID.SetFocus
End If
End Sub

Private Sub txtMob_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  TxtFax.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
'Private Sub txtname_KeyPress(KeyAscii As Integer)
'If KeyAscii = 13 Then
'  OptSB.SetFocus
'End If
'If (KeyAscii >= 48 And KeyAscii <= 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
'  KeyAscii = 0
'End If
'End Sub
Private Sub txtName_KeyUp(KeyCode As Integer, Shift As Integer)
txtCPer.Text = txtName.Text
End Sub
Private Sub txtName_Validate(Cancel As Boolean)
 If Trim(txtName.Text) = "" Then
    Cancel = True
    MsgBox "Plz. Insert a Sub Broker Name", vbCritical, "Alert"
 End If
 If Cancel = True Then
   txtName.SetFocus
 End If
End Sub
Private Sub txtPAN_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  cmbDist.SetFocus
End If
If KeyAscii >= 97 And KeyAscii <= 122 Then
    KeyAscii = KeyAscii - 32
End If
End Sub
Private Sub txtPh_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  CmdOk.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
  KeyAscii = 0
End If
End Sub
Private Sub txtPin_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        txtCPer.SetFocus
     End If
     If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 Then
         KeyAscii = 0
     End If
End Sub




Private Sub txtr_add1_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
        txtr_add2.SetFocus
End If
Call TextValidation(KeyAscii)
End Sub

Private Sub txtr_add2_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
        cmbr_city.SetFocus
End If
Call TextValidation(KeyAscii)
End Sub

Private Sub txtr_pin_KeyPress(KeyAscii As Integer)
 On Error GoTo BOError
    If KeyAscii = 13 Then
        cmdr_close.SetFocus
    Else
        Call NumberValidation(KeyAscii)
    End If
    Exit Sub
BOError:
    MsgBox err.Description, vbCritical, "Backoffice"
    Exit Sub
End Sub

Private Sub txtr_pin_LostFocus()
If txtr_pin.Text <> "" Then
 If Len(txtr_pin) < 6 Then
    MsgBox "Please Enter Valid PIN ", vbInformation, "Wealth Maker"
    ''txtoffice_pin.Text = ""
    txtr_pin.SetFocus
 Exit Sub
 End If
End If
If txtr_pin.Text <> "" And Val(txtr_pin.Text) < Val("110001") Then
        MsgBox "Please Enter Valid PIN ", vbInformation
        ''txtoffice_pin.Text =""
        txtr_pin.SetFocus
        Exit Sub
    End If
End Sub

Private Sub txtRem_KeyPress(KeyAscii As Integer)
On Error GoTo BOError
If txtSbCode.Enabled = True Then
If KeyAscii = 13 Then
  txtSbCode.SetFocus
End If
Else
    If KeyAscii = 13 Then
        CmbBank.SetFocus
    End If
    Call NameValidation(KeyAscii)
End If
Exit Sub
BOError:
        MsgBox err.Description, vbCritical, "BackOfice"
Exit Sub
End Sub
Private Sub txtSbCode_KeyPress(KeyAscii As Integer)
 If KeyAscii = 13 Then
  CmbBank.SetFocus
 End If
  KeyAscii = 0
End Sub
Private Sub txtstate_KeyPress(KeyAscii As Integer)
 If KeyAscii = 13 Then
    txtMob.SetFocus
 End If
 KeyAscii = 0
End Sub
Private Sub txtTds_KeyPress(KeyAscii As Integer)
 If KeyAscii = 13 Then
  cmbstype.SetFocus
End If
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 40 And KeyAscii <> 41 And KeyAscii <> 47 And KeyAscii <> 45 And KeyAscii <> 8 And KeyAscii <> 91 And KeyAscii <> 93 And KeyAscii <> 46 Then
  KeyAscii = 0
End If
End Sub
Private Sub txtTds_KeyUp(KeyCode As Integer, Shift As Integer)
    If Val(txtTds.Text) > 100 Then
        MsgBox "TDS Should not Exceed than 100(%)"
        txtTds.Text = ""
    End If
End Sub
Private Sub txtWebSite_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
  txtPh.SetFocus
End If
End Sub
Private Sub City()
 Set rsCity = New ADODB.Recordset
  If rsCity.State = 1 Then rsCity.Close
   rsCity.CursorLocation = adUseClient
    rsCity.open "select distinct city_name from city_master order by city_name", MyConn, adOpenForwardOnly, adLockReadOnly
    If rsCity.RecordCount > 0 Then
     cmbcity.Clear
     cmbbankcity.Clear
      For i = 0 To rsCity.RecordCount - 1
        cmbcity.AddItem rsCity(0)
        cmbbankcity.AddItem rsCity(0)
        rsCity.MoveNext
      Next
      rsCity.Close
    End If
End Sub
Private Sub Bank()
    Set rsBank = New ADODB.Recordset
    If rsBank.State = 1 Then rsBank.Close
    rsBank.CursorLocation = adUseClient
    rsBank.open "Select bank_id,bank_name from ana_bank_master order by bank_name", MyConn, adOpenForwardOnly, adLockReadOnly
    If rsBank.RecordCount > 0 Then
    CmbBank.Clear
    For i = 0 To rsBank.RecordCount - 1
        CmbBank.AddItem rsBank(1) & Space(80) & "#" & rsBank(0)
        rsBank.MoveNext
    Next
    rsBank.Close
    End If
End Sub
Private Sub SCategory()
  Set RSCAT = New ADODB.Recordset
  If RSCAT.State = 1 Then RSCAT.Close
     RSCAT.CursorLocation = adUseClient
'      RSCAT.open "select name from agent_category ", myconn, adOpenDynamic, adLockPessimistic
       RSCAT.open "select distinct businesspack from advisorsubentry", MyConn, adOpenDynamic, adLockPessimistic
       If RSCAT.RecordCount > 0 Then
         cmbSCat.Clear
          For i = 0 To RSCAT.RecordCount - 1
             With cmbSCat
                  .AddItem Trim(RSCAT(0))
                  RSCAT.MoveNext
             End With
          Next
        RSCAT.Close
       End If
End Sub
Private Sub txtWebSite_LostFocus()
  If Trim(txtWebSite) <> "" Then
    st1 = InStr(1, LCase(txtWebSite.Text), "www", vbTextCompare)
     If st1 < 1 Then
         MsgBox "Please enter correct Web Site    ", vbCritical, "Alert"
         txtWebSite.SetFocus
         Exit Sub
     End If
    st2 = InStr(1, txtWebSite.Text, ".", vbTextCompare)
     If st2 <> 4 And st2 > 4 Then
         MsgBox "Please enter correct Web Site     ", vbCritical, "Alert"
         txtWebSite.SetFocus
         Exit Sub
     End If
  End If
End Sub
