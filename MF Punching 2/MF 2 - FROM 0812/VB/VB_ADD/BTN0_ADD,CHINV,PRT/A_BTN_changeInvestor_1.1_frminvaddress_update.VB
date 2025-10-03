Dim mob_ch As Integer
Dim con_1 As New ADODB.Recordset
Dim con_2 As New ADODB.Recordset
Dim state_dat As New ADODB.Recordset
Dim city_dat As New ADODB.Recordset
Dim inv_det As New ADODB.Recordset
Dim IsFamilyHead As Integer
Dim Sql1 As String
Dim MOB_NUM As String
Dim AADHAR_NUM As String
Dim AADHAR_CH As Integer

Dim PAN_NUM As String
Dim PAN_CH As Integer

Dim EMAIL_NUM As String
Dim EMAIL_CH As Integer
Dim InvName As String
Public currentForm As Form

Private Sub cboState_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
txtPin.SetFocus
End If
End Sub

Private Sub cmbTxtCity_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
cboState.SetFocus
End If
End Sub

Private Sub exit_bt_Click()
If UCase(Label2.Caption) = "FRMAR" Then
    frmAR.txtNominee.SetFocus
    frmAR.WindowState = 2
End If

If UCase(Label2.Caption) = "FRMARGENERAL" Then
    frmARGeneral.txtProposer.SetFocus
    frmARGeneral.WindowState = 2
End If
Unload Me
End Sub



Private Sub txtaadharcardno_Change()
AADHAR_CH = 1
End Sub

Private Sub txtaadharcardno_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
    KeyAscii = 0
End If
End Sub

Private Sub txtadd1_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    txtAdd2.SetFocus
End If
End Sub

Private Sub txtEmail_Change()
EMAIL_CH = 1
End Sub

Private Sub txtEMail_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    upload_bt.SetFocus
End If
End Sub

Private Sub TxtPan_Change()
PAN_CH = 1
End Sub

Private Sub upload_bt_Click()
Dim s As String
Dim s1 As String
Dim s2 As String
Dim strState_id As String
    'Label32 -->investor code
    'label2 --> form name
    
     ''''''''''''''''''''''''''ADMIN UPDATE VALIDATION'''''''''''''''''''''''''''''''''''''''''''''''''''
    If GlbroleId = 1 Then
        V = MsgBox("You are not authorised to update the details.", vbInformation, "Client Master")
        upload_bt.SetFocus
        Exit Sub
    End If
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    
    If Mid(Label32.Caption, 1, 1) = 4 Then
        If SqlRet("select check_number('" & Trim(txtMobile.Text) & "') from dual") = "N" Then
            MsgBox "Invalid Mobile No", vbInformation
            txtMobile.SetFocus
            Exit Sub
        End If
    End If
   
    If mob_ch = 1 And MOB_NUM <> txtMobile.Text Then
        con_2.open "select mobile from investor_master where mobile='" & txtMobile.Text & "' and inv_code not like '%" & Mid(Label32.Caption, 1, 8) & "%'", MyConn, adOpenForwardOnly
        If Not con_2.EOF Then
            MsgBox "Mobile Number already present!", vbInformation, strBajaj
            con_2.Close
            Exit Sub
        End If
        con_2.Close
    End If
    
    If PAN_CH = 1 And UCase(PAN_NUM) <> UCase(txtPan.Text) Then
        If SqlRet("SELECT VALIDATEPAN1('" & txtPan.Text & "') FROM DUAL") = 0 Then
            MsgBox "Invalid PAN", vbInformation, strBajaj
            Exit Sub
        End If
    End If
    
    If PAN_CH = 1 And UCase(PAN_NUM) <> UCase(txtPan.Text) Then
        If con_2.State = 1 Then con_2.Close
        con_2.open "select PAN from investor_master where upper(PAN)='" & Trim(UCase(txtPan.Text)) & "' and inv_code not like '%" & Mid(Label32.Caption, 1, 8) & "%'", MyConn, adOpenForwardOnly
        If Not con_2.EOF Then
            MsgBox "PAN already exists!", vbInformation, strBajaj
            con_2.Close
            Exit Sub
        End If
        con_2.Close
    End If
    
      
    If EMAIL_CH = 1 And UCase(EMAIL_NUM) <> UCase(txtEmail.Text) Then
        If UCase(txtEmail.Text) <> "NOT AVAILABLE" And UCase(txtEmail.Text) <> "N/A" And UCase(txtEmail.Text) <> "N A" And UCase(txtEmail.Text) <> "NILL" And UCase(txtEmail.Text) <> "NONE" And UCase(txtEmail.Text) <> "N-A" Then
            con_2.open "select EMAIL from investor_master where upper(EMAIL)='" & UCase(txtEmail.Text) & "' and inv_code not like '%" & Mid(Label32.Caption, 1, 8) & "%'", MyConn, adOpenForwardOnly
            If Not con_2.EOF Then
                MsgBox "EMAIL already exists!", vbInformation, strBajaj
                con_2.Close
                Exit Sub
            End If
            con_2.Close
       End If
    End If
    
    '---------------------------------------by pankaj pundir on dated 01032016----------------------------------------------------------
    If AADHAR_CH = 1 And AADHAR_NUM <> txtaadharcardno.Text And AADHAR_CARD_NO <> "" Then
        con_2.open "select AADHAR_CARD_NO from investor_master where AADHAR_CARD_NO=" & txtaadharcardno.Text & " and inv_code <>" & Label32.Caption & "", MyConn, adOpenForwardOnly
        If Not con_2.EOF Then
            MsgBox "Aadhar Card Number already Exist!", vbInformation, strBajaj
            con_2.Close
            Exit Sub
        End If
        con_2.Close
    End If
    '-----------------------------------------------------------------------------------------------------------------------------------
    s = CmbtxtCity.Text
    s1 = cboState.Text
    
    con_1.open "select city_id from city_master where city_name='" & s & "'", MyConn, adOpenForwardOnly
    If Not con_1.EOF Then
      s2 = con_1!City_id
    End If
    con_1.Close
    
    con_1.open "select state_id from state_master where state_name = '" & s1 & "'", MyConn, adOpenForwardOnly
    If Not con_1.EOF Then
      strState_id = con_1!state_id
    End If
    con_1.Close
    

    If Mid(Label32.Caption, 1, 1) = 3 Then
        MyConn.Execute "UPDATE INVESTOR_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,AADHAR_CARD_NO='" & txtaadharcardno.Text & "',PAN=TRIM('" & txtPan.Text & "'), MOBILE='" & txtMobile.Text & "',email='" & txtEmail.Text & "' ,address1='" & txtAdd1.Text & "',address2='" & txtAdd2.Text & "',pincode='" & txtPin.Text & "',city_id='" & s2 & "'  WHERE inv_code = '" & Label32.Caption & "'"
    Else
        MyConn.Execute "UPDATE INVESTOR_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,AADHAR_CARD_NO='" & txtaadharcardno.Text & "',PAN=TRIM('" & txtPan.Text & "'),MOBILE='" & txtMobile.Text & "',email='" & txtEmail.Text & "' WHERE inv_code=" & Label32.Caption & ""
        MyConn.Execute "UPDATE INVESTOR_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,address1='" & txtAdd1.Text & "',address2='" & txtAdd2.Text & "',pincode='" & txtPin.Text & "',city_id='" & s2 & "' WHERE SOURCE_ID = '" & Mid(Label32.Caption, 1, 8) & "'"
        MyConn.Execute "UPDATE client_test SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,add1='" & txtAdd1.Text & "',add2='" & txtAdd2.Text & "',pincode='" & txtPin.Text & "',city_id='" & s2 & "',state_id='" & strState_id & "' WHERE SOURCE_CODE = '" & Mid(Label32.Caption, 1, 8) & "'"
    End If

    MyConn.Execute "UPDATE client_test SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,AADHAR_CARD_NO='" & txtaadharcardno.Text & "',CLIENT_PAN=TRIM('" & txtPan.Text & "'), MOBILE_no='" & txtMobile.Text & "',email='" & txtEmail.Text & "' WHERE client_codekyc=" & Label32.Caption & ""
    
          
    If msktodate <> "__/__/____" Then
      MyConn.Execute "UPDATE INVESTOR_MASTER SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,dob=TO_DATE('" & msktodate.Text & "','DD/MM/YYYY')  WHERE inv_code = '" & Label32.Caption & "'"
      MyConn.Execute "UPDATE CLIENT_TEST SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,dob=TO_DATE('" & msktodate.Text & "','DD/MM/YYYY')  WHERE client_codekyc = '" & Label32.Caption & "'"
    End If
       
        
    If IsFamilyHead = "1" Then
        MyConn.Execute " UPDATE CLIENT_MASTER T SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,PAN=TRIM('" & txtPan.Text & "'),MOBILE='" & txtMobile.Text & "',email='" & txtEmail.Text & "',address1='" & txtAdd1.Text & "',address2='" & txtAdd2.Text & "',pincode='" & txtPin.Text & "', city_id='" & s2 & "' WHERE  client_code=" & Mid(Label32.Caption, 1, 8) & ""
        If msktodate <> "__/__/____" Then
              MyConn.Execute " UPDATE CLIENT_MASTER T SET MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,dob=TO_DATE('" & msktodate.Text & "','DD/MM/YYYY') WHERE  client_code=" & Mid(Label32.Caption, 1, 8) & " "
        End If
    End If
    
    If currentForm.Name = "frmARGeneral" Then
        frmARGeneral.txtmobileTran = txtMobile.Text
    End If
    MsgBox "Information updated", vbInformation, strBajaj
End Sub
Private Sub Form_Activate()
If currentForm.Name <> "frmARGeneral" Then
    Sql1 = "select nvl(a.address1,' '),nvl(a.address2,' '),nvl(b.city_name,' '),nvl(c.state_name,' '),nvl(a.mobile,0),nvl(a.email,' '),nvl(a.pincode,' '),nvl(upper(a.pan),' '),a.aadhar_card_no,A.DOB,investor_name  from investor_master a,city_master b, state_master c where a.city_id=b.city_id(+) and b.state_id=c.state_id(+) and a.inv_code='" & Label32.Caption & "'"
    If inv_det.State = 1 Then inv_det.Close
    inv_det.open (Sql1), MyConn
    If (inv_det.EOF = False) Then
        txtAdd1.Text = inv_det(0)
        txtAdd2.Text = inv_det(1)
        txtPin.Text = inv_det(6)
        CmbtxtCity.Text = inv_det(2)
        cboState.Text = inv_det(3)
        txtEmail.Text = inv_det(5)
        txtMobile.Text = inv_det(4)
        MOB_NUM = inv_det(4)
        txtPan.Text = inv_det(7)
        InvName = inv_det.Fields("INVESTOR_NAME")
        txtaadharcardno.Text = IIf(IsNull("" & inv_det(8)), "", "" & inv_det(8))
        AADHAR_NUM = IIf(IsNull("" & inv_det(8)), "", "" & inv_det(8))
        PAN_NUM = IIf(IsNull("" & inv_det(7)), "", "" & inv_det(7))
        EMAIL_NUM = IIf(IsNull("" & inv_det(5)), "", "" & inv_det(5))
        If inv_det(9) <> "" Then
            msktodate.Text = Format("" & inv_det(9), "DD/MM/YYYY")
        End If
        IsFamilyHead = SqlRet("select IS_FAMILY_HEAD ('" & Label32.Caption & "') from dual")
    End If
    inv_det.Close
End If
End Sub

Private Sub Form_Load()
CmbtxtCity.Clear
city_dat.open "select * from city_master order by city_name", MyConn, adOpenForwardOnly, adLockReadOnly
Do While Not city_dat.EOF
    CmbtxtCity.AddItem city_dat!city_name
    city_dat.MoveNext
Loop
city_dat.Close


If currentForm.Name = "frmARGeneral" Then
    If Len(frmARGeneral.txtClientCD.Text) >= 10 Then
        Sql1 = "select nvl(a.address1,' '),nvl(a.address2,' '),nvl(b.city_name,' '),nvl(c.state_name,' '),nvl(a.mobile,0),nvl(a.email,' '),nvl(a.pincode,' '),nvl(upper(a.pan),' '),a.aadhar_card_no,A.DOB,investor_name  from investor_master a,city_master b, state_master c where a.city_id=b.city_id(+) and b.state_id=c.state_id(+) and a.inv_code='" & frmARGeneral.txtClientCD.Text & "'"
        If inv_det.State = 1 Then inv_det.Close
        inv_det.open (Sql1), MyConn
        If (inv_det.EOF = False) Then
            txtAdd1.Text = inv_det(0)
            txtAdd2.Text = inv_det(1)
            txtPin.Text = inv_det(6)
            CmbtxtCity.Text = inv_det(2)
            cboState.Text = inv_det(3)
            txtEmail.Text = inv_det(5)
            txtMobile.Text = inv_det(4)
            MOB_NUM = inv_det(4)
            txtPan.Text = inv_det(7)
            InvName = inv_det.Fields("INVESTOR_NAME")
            txtaadharcardno.Text = IIf(IsNull("" & inv_det(8)), "", "" & inv_det(8))
            AADHAR_NUM = IIf(IsNull("" & inv_det(8)), "", "" & inv_det(8))
            PAN_NUM = IIf(IsNull("" & inv_det(7)), "", "" & inv_det(7))
            EMAIL_NUM = IIf(IsNull("" & inv_det(5)), "", "" & inv_det(5))
            If inv_det(9) <> "" Then
                msktodate.Text = Format("" & inv_det(9), "DD/MM/YYYY")
            End If
            IsFamilyHead = SqlRet("select IS_FAMILY_HEAD ('" & Label32.Caption & "') from dual")
        End If
        inv_det.Close
    End If
End If
End Sub
    
    Private Sub CmbtxtCity_Click()
    cboState.Clear
    state_dat.open "select * from state_master where state_id in(select state_id from city_master where city_name='" & CmbtxtCity.Text & "') order by state_name", MyConn, adOpenForwardOnly, adLockReadOnly
    Do While Not state_dat.EOF
         cboState.AddItem state_dat!state_name
         state_dat.MoveNext
    Loop
    state_dat.Close
    cboState.Text = cboState.List(0)
    End Sub
    
    Private Sub CmbtxtCity_Validate(Cancel As Boolean)
    If Len(CmbtxtCity.Text) <= 0 Then
    MsgBox "Please Enter a valid city", vbInformation, strBajaj
    CmbtxtCity.Text = ""
        Exit Sub
        End If
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
Private Sub txtMobile_Change()
mob_ch = 1
End Sub

Private Sub txtMobile_Validate(Cancel As Boolean)
If txtMobile.Text <> "" Then
    If Len(txtMobile.Text) <> 10 Or Mid(txtMobile.Text, 1, 1) = 1 Or Mid(txtMobile.Text, 1, 1) = 2 Or Mid(txtMobile.Text, 1, 1) = 3 Or Mid(txtMobile.Text, 1, 1) = 4 Or Mid(txtMobile.Text, 1, 1) = 5 Then
    MsgBox "Please Enter a valid mobile number !", vbInformation, strBajaj
    txtMobile.Text = ""
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


Private Sub txtAdd2_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
If KeyAscii = 13 Then
    CmbtxtCity.SetFocus
End If
End Sub

Private Sub txtMobile_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    txtEmail.SetFocus
End If
If KeyAscii = 8 Then Exit Sub
    If KeyAscii < 48 Or KeyAscii > 57 Then
        KeyAscii = 0
    Exit Sub
End If
End Sub


Private Sub txtPin_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    txtMobile.SetFocus
End If
If KeyAscii = 8 Then Exit Sub
If KeyAscii < 48 Or KeyAscii > 57 Then
    KeyAscii = 0
    Exit Sub
End If
End Sub

