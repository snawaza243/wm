Private Sub cmdAdd_Click()
If Trim(Txtname.Text) = "" Then
    MsgBox "Plz. Insert a Sub Broker Name", vbCritical, "Alert"
    Txtname.SetFocus
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


 If Len(txtPin.Text) <> 6 Then
   MsgBox "Plz. Enter Correct 6 digit PIN Number.", vbCritical, "Alert"
   txtPin.SetFocus
   Exit Sub
End If


'txtr_add1

'txtAdd1

If Trim(txtadd1.Text) = "" Then
   MsgBox "Plz. Enter Sub Broker Residential Address.", vbCritical, "Alert"
   txtadd1.SetFocus
   Exit Sub
End If
     
     
     
If Trim(txtEmail.Text) = "" Then
   MsgBox "Plz. Enter Sub Broker email Address.", vbCritical, "Alert"
   txtEmail.SetFocus
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
    FrmAdvisorSubEntry.TxtANAName.Text = UCase(Txtname.Text)
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