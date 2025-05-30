Private Sub cmdAdd_Click()
On Error Resume Next
Dim rs_city As New ADODB.Recordset
Dim cityId As String
Dim GetRecentCLCode As New ADODB.Recordset
Dim GenClientCode As Long, OCid As Long
Dim MobNo As Double
Dim ArOcc() As String
Dim ParentCode As Long
Dim Scode As Long
Dim pmode As Long
Dim MFlag As String
Dim inv_Cate As Integer ''investor category
Dim fp As String
Dim FPFdate As String
Dim FPLdate As String
Dim res As String
Dim office_add1 As String
Dim office_add2 As String
Dim office_city As String
Dim office_state As String
Dim office_pin As String
Dim Perm_add1 As String
Dim Perm_add2 As String
Dim Perm_city As String
Dim Perm_state As String
Dim Perm_pin As String
Dim std_code1 As String
Dim std_code2 As String
Dim std_code3 As String
Dim phone1 As String
Dim phone2 As String
Dim phone3 As String
Dim phtype As String
Dim phtype2 As String
Dim phtype3 As String
Dim occ() As String
Dim min_number As String
Dim subCampaign_name  As String
Dim Campaign_name  As String
Dim i As Integer
Dim Str() As String
Dim rs_max_inv As New ADODB.Recordset
Dim MAX_INV_CD As Long
Dim ExistClient As New ADODB.Recordset
Dim ExistTranCode As String
On Error GoTo errr
Dim ExistPan As New ADODB.Recordset
Dim rsDup As New ADODB.Recordset
Dim ClCat As String
Dim invCat As String
Dim rmipc As New ADODB.Recordset
Dim rsacces As New ADODB.Recordset
Dim dguest As New ADODB.Recordset

If txtdocID <> "" Then
    If SqlRet("select check_dt_newinv('" & Trim(txtdocID.Text) & "') from dual") <= 0 Then
        MsgBox "Please Enter Valid DT Number"
        SSTab1.Tab = 0
        txtdocID.SetFocus
        Exit Sub
    End If
Else
    MsgBox "Please enter DT number for new account"
    SSTab1.Tab = 0
    txtdocID.SetFocus
    Exit Sub
End If

If dguest.State = 1 Then dguest.Close
    dguest.open "select count(*) from client_master where GUEST_CD='" & TguestCode.Text & "'", MyConn
    If dguest(0) >= 1 Then
        MsgBox "Guest code already in use", vbCritical, "Wealthmaker"
    Exit Sub
End If



rsacces.open "select * from access_control_account where role_id=" & GlbroleId, MyConn

If Not rsacces.EOF Or Not rsacces.BOF Then
    If rsacces(1) <> 1 Then
        MsgBox "Insufficient permission", vbCritical, "Wealthmaker"
        rsacces.Close
        Exit Sub
    End If
Else
    MsgBox "Insufficient permission", vbCritical, "Wealthmaker"
    rsacces.Close
    Exit Sub
End If

rmipc.open "select rm_channel('" & txtbusicode.Text & "') from dual", MyConn
If rmipc(0) = 185 Then
   If TguestCode.Text = "" Then
    MsgBox "Please Enter Guest code"
    rmipc.Close
    Exit Sub
   Else
     rmipc.Close
   End If
Else
   TguestCode.Text = ""
   rmipc.Close
End If


For i = 1 To VSFCommGrdE.Rows - 1
    If VSFCommGrdE.TextMatrix(i, 2) <> "" Then
         If VSFCommGrdE.TextMatrix(i, 17) = "" Then
              MsgBox "Title can't be null"
             Exit Sub
         End If
    End If
Next


For i = 1 To VSFCommGrdE.Rows - 1
If VSFCommGrdE.TextMatrix(i, 2) <> "" Then
    If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
        If SqlRet("select count(*) from client_test where upper(mobile_no) = '" & Trim(VSFCommGrdE.TextMatrix(i, 3)) & "'") > 0 Then
            MsgBox "Mobile No " & Trim(VSFCommGrdE.TextMatrix(i, 3)) & " is already Exist"
            Exit Sub
        End If
    End If
     
    If UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NOT AVAILABLE" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "N/A" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "N A" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NILL" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NONE" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "N-A" Then
           If VSFCommGrdE.TextMatrix(i, 4) <> "" Then
               If SqlRet("select count(*) from client_test where upper(email) = upper('" & Trim(VSFCommGrdE.TextMatrix(i, 4)) & "')") > 0 Then
                   MsgBox "Email " & Trim(VSFCommGrdE.TextMatrix(i, 4)) & " is already Exist"
                   Exit Sub
               End If
           End If
    End If
     
     
     
  If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
    Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
     If Temp_Id(1) <> 29 And Temp_Id(1) <> 30 And Temp_Id(1) <> 31 And Temp_Id(1) <> 32 Then
           If Trim(VSFCommGrdE.TextMatrix(i, 5)) = "" And VSFCommGrdE.TextMatrix(i, 6) <> "" Then
                'Check If He/She Is Minor
                If DateDiff("M", Format(VSFCommGrdE.TextMatrix(i, 6), "DD/MM/YYYY"), ServerDateTime) <= 216 Then
                    If VSFCommGrdE.TextMatrix(i, 12) <> "" And ValidatePan(VSFCommGrdE.TextMatrix(i, 13)) = False Then
'                        MsgBox "Please Enter Valid Pan No Of The Guardian ", vbInformation, strBajaj
'                        Exit Sub
                    ElseIf VSFCommGrdE.TextMatrix(i, 12) = "" Then
                        MsgBox "Plese Enter Guardian Name ", vbInformation, strBajaj
                        Exit Sub
                    ElseIf ValidatePan(VSFCommGrdE.TextMatrix(i, 13)) = False Then
'                        MsgBox "Please Enter Valid Pan No Of The Guardian ", vbInformation, strBajaj
'                        Exit Sub
                    End If
                End If
            Else '----------he enters any pan no.----------------
                If VSFCommGrdE.TextMatrix(i, 6) <> "" Then
                    'If DateDiff("YYYY", Format(VSFCommGrdE.TextMatrix(I, 6), "DD/MM/YYYY"), ServerDateTime) < 18 Then
                    If DateDiff("M", Format(VSFCommGrdE.TextMatrix(i, 6), "DD/MM/YYYY"), ServerDateTime) <= 216 Then
                        MsgBox "Enter Pan No Of Guardian Only", vbInformation, "Wealthmaker"
                        VSFCommGrdE.TextMatrix(i, 5) = ""
                        Exit Sub
                    End If
                End If
            End If
      End If
  End If
End If
Next


If cmbgender.Text = "" Then
    MsgBox "Gender can't be left blank", vbInformation
    Exit Sub
End If

If CmbMeritalStatus.Text = "" Then
    MsgBox "Marital Status can't be left blank", vbInformation
    Exit Sub
End If

If CmbLeadSource.Text = "" Then
    MsgBox "Lead Source can't be left blank", vbInformation
    Exit Sub
End If

If txtclientpan.Text <> "" Then
    sql = "Select * from client_test where 1=1 "
    If txtclientpan.Text <> "" Then
       sql = sql & " and client_pan='" & txtclientpan & "' "
    End If
    ExistClient.open sql, MyConn, adOpenForwardOnly
    If Not ExistClient.EOF Then
         MsgBox "This Pan No is already Exist"
         Exit Sub
    End If
    ExistClient.Close
End If

'---------------------------------by pankaj pundir on dated 29022016----------------------------------------------------------
If txtaddharCardNo.Text <> "" Then
    If Len(txtaddharCardNo.Text) <> 12 Then
       MsgBox "Minimum Length Of Aadhar Card No Should Be Greater or Equal To 12 digit", vbInformation, "Wealthmaker"
       txtaddharCardNo.SetFocus
       Exit Sub
    ElseIf txtaddharCardNo.Text = "000000000000" Then
       MsgBox "Please Enter A Valid adhar Card No", vbInformation, "Wealthmaker"
       txtaddharCardNo.SetFocus
       Exit Sub
    End If
    
    sql = "Select * from client_test where 1=1 "
    sql = sql & " and AADHAR_CARD_NO=" & txtaddharCardNo.Text & " "
    
    ExistClient.open sql, MyConn, adOpenForwardOnly
    If Not ExistClient.EOF Then
         MsgBox "This Aadhar Card No is already Exist"
         Exit Sub
    End If
    ExistClient.Close
End If
    
'---------------------------------------Check For Duplicate Account------------------------------
If Len(txtclientcodeold.Text) >= 11 Then
    sql = "Select * from client_test where client_codekyc='" & txtclientcodeold & "'"
    ExistClient.open sql, MyConn, adOpenForwardOnly
    If Not ExistClient.EOF Then
         MsgBox "Account Of This Main Investor Is Already Exist", vbInformation
         Exit Sub
    End If
    ExistClient.Close
End If
'---------------------------------------Mayank Garg------------------------------------------
If CheckDuplicateInvestor = False Then
   MsgBox "Check The Duplicacy Of Pan No Of Investor"
   Exit Sub
End If

If CheckDuplicateInvestorPan = False Then
   MsgBox "Check The Duplicacy of Pan No of Investor"
   Exit Sub
End If

If CheckDuplicateMainCode = False Then
    MsgBox "Pan No Already Exist For Main Account"
   Exit Sub
End If
If CheckDuplicateMainCode1 = False Then
    MsgBox "This Name Already Exist For Main Account"
   Exit Sub
End If
If CheckDuplicate = False Then
   MsgBox "Please Ensure Your Correct PAN No.", vbInformation
   Exit Sub
End If
If CheckMandatory = False Then
   MsgBox "Fill All The Details of investor"
   Exit Sub
End If
'--------------------------------------------------------------------------------------------
guardian_enable
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
    txtclientpan.Text = ""
    If txtguardianpan.Text <> "" Then
        If ValidatePan(txtguardianpan.Text) = False Then
           MsgBox "Plaes Enter Valid Pan No of guardian"
           SSTab1.Tab = 0
           txtguardianpan.SetFocus
           Exit Sub
        End If
    End If
    If txtguardianname.Text = "" Then
           MsgBox "Plaes Enter The Name Of Guardian", vbInformation, "Wealthmaker"
           SSTab1.Tab = 0
           txtguardianpan.SetFocus
           Exit Sub
    End If
End If
If MyBranch <> "10010097" Then
    If dob.Text = "__/__/____" Then
       MsgBox "Date Of Birth Can Not Be Left Blank"
       SSTab1.Tab = 0
       dob.SetFocus
       Exit Sub
    End If
    If IsDate(dob.Text) = False Then
        MsgBox "Please Enter Valid Date"
        SSTab1.Tab = 0
        dob.SetFocus
        Exit Sub
    End If
End If
If txtbusicode.Text = "" Then
    MsgBox "Please Enter Valid Business RM Code", vbInformation
    SSTab1.Tab = 0
    txtbusicode.SetFocus
    Exit Sub
End If
If cmbresinri.Text = "" Then
   MsgBox "Resident/NRI Can Not Left Blank"
   SSTab1.Tab = 0
   cmbresinri.SetFocus
   Exit Sub
End If
If cmbcity.Text = "" Then
   MsgBox "City Can Not Left Blank"
   SSTab1.Tab = 0
   cmbcity.SetFocus
   Exit Sub
End If
If cmbstate.Text = "" Then
   MsgBox "State Can Not Left Blank"
   SSTab1.Tab = 0
   cmbstate.SetFocus
   Exit Sub
End If
If MyBranch <> "10010097" Then
    If cmbclientn.Text <> "NRI" Then
        If txtpin.Text = "" Then
            MsgBox "PIN No can Not Be left Blank", vbInformation
            SSTab1.Tab = 0
            txtpin.SetFocus
            Exit Sub
        End If
        If Len(txtpin.Text) < 6 Or Len(txtpin.Text) > 6 Then
            MsgBox "PIN No Should Be 6 Character", vbInformation
            SSTab1.Tab = 0
            txtpin.SetFocus
            Exit Sub
        End If
    End If
    If GlbroleId <> 22 Then
        If cmbclientn.Text <> "NRI" Then
            If TxtMobile.Text = "" Then
                MsgBox "Mobile No can Not Be left Blank", vbInformation
                SSTab1.Tab = 0
                TxtMobile.SetFocus
                Exit Sub
            End If
            If (Len(TxtMobile.Text) < 10 Or Len(TxtMobile.Text) > 10) Then
                MsgBox "Mobile No Should Be 10 Character", vbInformation
                SSTab1.Tab = 0
                TxtMobile.SetFocus
                Exit Sub
            End If
        End If
    Else
        If TxtMobile.Text = "" Then
            MsgBox "Mobile No can Not Be left Blank,It Can Be 0", vbInformation
            SSTab1.Tab = 0
            TxtMobile.SetFocus
            Exit Sub
        End If
        If cmbclientn.Text <> "NRI" Then
            If (Len(TxtMobile.Text) < 10 Or Len(TxtMobile.Text) > 10) And TxtMobile.Text <> "0" Then
                MsgBox "Mobile No Should Be 10 Character", vbInformation
                SSTab1.Tab = 0
                TxtMobile.SetFocus
                Exit Sub
            End If
        End If
    End If
    If cmbclientn.Text <> "NRI" Then
        If Len(TxtMobile.Text) > 10 And Mid(TxtMobile.Text, 1, 1) <> "9" And Mid(TxtMobile.Text, 1, 1) <> "8" And Mid(TxtMobile.Text, 1, 1) <> "7" And Mid(TxtMobile.Text, 1, 1) <> "6" Then
            MsgBox "Mobile No Should Be Start 6 or 7 or 8 or 9", vbInformation
            SSTab1.Tab = 0
            TxtMobile.SetFocus
            Exit Sub
        End If
    End If
End If
Set RSV = MyConn.Execute("select ItemSerialNumber from FixedItem where upper(ItemName)='" & UCase(Trim(cmbprefcommmode.Text)) & "'")
If Not RSV.EOF Then
    CommMode = RSV(0)
End If
RSV.Close
Set RSV = MyConn.Execute("select ItemSerialNumber from FixedItem where upper(ItemName)='" & UCase(Trim(cmbcomminterval.Text)) & "'")
If Not RSV.EOF Then
    CommInterval = RSV(0)
End If
RSV.Close
If cmbactcat.Text <> "" Then
    ActCat = Split(cmbactcat.Text, "#")
    ActCat1 = ActCat(1)
Else
    ActCat1 = 0
End If
'----------------clientvatgory-------Investor Category---------------------------------------------
If cmbacatg.Text <> "" Then
    Str = Split(cmbacatg.Text, "#")
    ClCat = Str(1)
End If
If cmbinvestor.Text <> "" Then
    Str = Split(cmbinvestor.Text, "#")
    invCat = Str(1)
End If
If ClCat = "" Then
    MsgBox "Please Select Client category", vbInformation, "Wealthmaker"
    Exit Sub
End If
If invCat = "" Then
    MsgBox "Please Select Investor category", vbInformation, "Wealthmaker"
    Exit Sub
End If
'---------------------------------------------Validation ClientTest------------------------------------
    If UCase(TxtEmail.Text) <> "NOT AVAILABLE" And UCase(TxtEmail.Text) <> "N/A" And UCase(TxtEmail.Text) <> "N A" And UCase(TxtEmail.Text) <> "NILL" And UCase(TxtEmail.Text) <> "NONE" And UCase(TxtEmail.Text) <> "N-A" Then
        rsDup.open "Select client_code from client_test where  UPPER(EMAIL)='" & UCase(Trim(TxtEmail.Text)) & "'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Account Is Already Existing in the Database ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    If TxtMobile.Text <> "" And TxtMobile.Text <> "0" Then
        If rsDup.State = 1 Then rsDup.Close
        rsDup.open "Select client_code from client_test where MOBILE_NO='" & Trim(TxtMobile.Text) & "' and trim(upper(client_name)) like '%" & UCase(Trim(txtclientname.Text) & "%" & Trim(txtlstname)) & "%'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Account Is Already Existing in the Database ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    
    If TxtMobile.Text <> "" And TxtMobile.Text <> "0" And dob <> "__/__/____" Then
        If rsDup.State = 1 Then rsDup.Close
        rsDup.open "Select client_code from client_test where mobile_no='" & Trim(TxtMobile.Text) & "' AND DOB=TO_DATE('" & Trim(dob) & "','DD-MM-YYYY')", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "Duplicate Mobile In Account", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    
'End If
'------------------------------------------------------------------------------------------------------
If Len(txtclientcodeold.Text) < 11 Or txtclientcodeold.Text = "" Or Val(txtclientcodeold.Text) = 0 Then
    '----------------------------------------------ClienMaster Validation-----------------------
    If UCase(TxtEmail.Text) <> "NOT AVAILABLE" And UCase(TxtEmail.Text) <> "N/A" And UCase(TxtEmail.Text) <> "N A" And UCase(TxtEmail.Text) <> "NILL" And UCase(TxtEmail.Text) <> "NONE" And UCase(TxtEmail.Text) <> "N-A" Then
        rsDup.open "Select client_code from client_master where  UPPER(EMAIL)='" & UCase(Trim(TxtEmail.Text)) & "' and trim(upper(client_name)) like '%" & UCase(Trim(txtclientname.Text) & "%" & Trim(txtlstname)) & "%'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Client Is Already Existing in the Database On Email and Client Name ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    If TxtMobile.Text <> "" And TxtMobile.Text <> "0" Then
        If rsDup.State = 1 Then rsDup.Close
        rsDup.open "Select client_code from client_master where  MOBILE=" & Trim(TxtMobile.Text) & "", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Mobile Is Already Existing in Client the Database ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    If TxtMobile.Text <> "" And TxtMobile.Text <> "0" Then
        rsDup.open "Select client_code from client_master where mobile='" & Trim(TxtMobile.Text) & "'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "Duplicate Mobile ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    rsDup.open "Select client_code from client_master where address1||Address2='" & Trim(txtaddress1.Text) & Trim(txtaddress2.Text) & "' and city_id='" & MyCityCode & "' and sourceid=" & MyBranch, MyConn, adOpenForwardOnly
    If rsDup.EOF = False Then
        MsgBox "Duplicate Address", vbInformation
        rsDup.Close
        Set rsDup = Nothing
        Exit Sub
    End If
    rsDup.Close
    Set rsDup = Nothing
    '-------------------------------------------------------------------------------------------
End If
For Each ctl In Controls
    If TypeOf ctl Is CheckBox Then
        If chkpanproof.Value = 1 Then
            StrEnclosedPanA = "YES"
        Else
            StrEnclosedPanA = "NO"
        End If
        If chkform60.Value = 1 Then
            StrEnclosed60A = "YES"
        Else
            StrEnclosed60A = "NO"
        End If
        If chkform61.Value = 1 Then
            StrEnclosed61A = "YES"
        Else
            StrEnclosed61A = "NO"
        End If
        If chk_g_panproof.Value = 1 Then
            StrEnclosedPanG = "YES"
        Else
            StrEnclosedPanG = "NO"
        End If
        If chk_g_form60.Value = 1 Then
            StrEnclosed60G = "YES"
        Else
            StrEnclosed60G = "NO"
        End If
        If chk_g_form61.Value = 1 Then
            StrEnclosed61G = "YES"
        Else
            StrEnclosed61G = "NO"
        End If
        If chkactst.Value = 1 Then
            ActSt = "YES"
        Else
            ActSt = "NO"
        End If
        If chknews.Value = 1 Then
            News = "YES"
        Else
            News = "NO"
        End If
        If chksi.Value = 1 Then
            sInfo = "YES"
        Else
            sInfo = "NO"
        End If
        If chknipan.Value = 1 Then
            NIPan = "YES"
        Else
            NIPan = "NO"
        End If
        If chkniadd.Value = 1 Then
            NIAdd = "YES"
        Else
            NIAdd = "NO"
        End If
        If chknipower.Value = 1 Then
            NIPower = "YES"
        Else
            NIPower = "NO"
        End If
        If chkipan.Value = 1 Then
            IPan = "YES"
        Else
            IPan = "NO"
        End If
        If chkiadd.Value = 1 Then
            IAdd = "YES"
        Else
            IAdd = "NO"
        End If
    End If
Next ctl
MyConn.BeginTrans
If txtp.Text <> "" Or txtpan.Text <> "" Then
    'Call AddImage(txtp.Text, txtPAN.Text)
Else
    If Len(txtclientcodeold.Text) >= 11 Then 'FOR EXISTING INVESTOR WE CHECK KYC OR NON KYC IF KYC WE UPDATE CLIENT_TEST IF NOT WE INSERT CLIENT_TEST
         If RsKyc.State = 1 Then RsKyc.Close
         RsKyc.open " SELECT NVL(COUNT(*),0) FROM CLIENT_TEST WHERE CLIENT_CODEKYC=" & Trim(Left(txtclientcodeold.Text, 8)) & "", MyConn, adOpenForwardOnly
         If RsKyc(0) > 0 Then  'MEANS ACCOUNT ALRADY EXIST IN CLIENT_TEST SO UPDATE IT
             sql = "update client_test set PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "', "
             sql = sql & " REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "',"
             sql = sql & " NATURE_BUSINESS='" & cmbnatureofbusiness.Text & "',NATURE_TRUST='" & cmbnatureoftrust.Text & "',  branch_code=" & MyBranch & ", clientCode_old=" & Val(txtclientcodeold.Text) & ",tradingid='" & txttradingid.Text & "',benefi_id='" & txtdp_bid.Text & "',status='" & cmbstatus.Text & "',other='" & txtAcOther.Text & "',occ_id='" & MyOccCode & "',others1='" & TxtAcOther1.Text & "',"
             sql = sql & " title='" & cmbtitle.Text & "',TITLE_FATHER_SPOUSE='" & cmbtitle_Spouse.Text & "',client_name='" & txtclientname.Text & "',FATHER_SPOUSE_NAME='" & TxtFatherSpouseName.Text & "',nationality='" & cmbclientn.Text & "',client_pan='" & txtclientpan.Text & "',"
             
             If txtaddharCardNo.Text <> "" Then
                sql = sql & " AADHAR_CARD_NO=" & txtaddharCardNo.Text & ","
             Else
                sql = sql & " AADHAR_CARD_NO='',"
             End If
             
             sql = sql & " annual_income='" & cmbincome.Text & "',investement='" & cmbinvestment.Text & "',title_other='" & txttitle.Text & "',resident_nri='" & cmbresinri.Text & "',dob=TO_DATE('" & Trim(dob) & "','DD-MM-YYYY'),gender='" & cmbgender.Text & "',MARITAL_STATUS='" & CmbMeritalStatus.Text & "',LEAD_SOURCE='" & CmbLeadSource.Text & "',"
             
             sql = sql & " a_enclosed_pan='" & StrEnclosedPanA & "',a_enclosed_60='" & StrEnclosed60A & "',a_enclosed_61='" & StrEnclosed61A & "',G_enclosed_pan='" & StrEnclosedPanG & "',g_enclosed_60='" & StrEnclosed60G & "',g_enclosed_61='" & StrEnclosed61G & "',a_kyc_status='" & cmbkyc.Text & "',g_kyc_status='" & cmbkyc1.Text & "',"
             sql = sql & " g_name='" & txtguardianname.Text & "',g_nationality='" & txtguardiann.Text & "',g_pan='" & txtguardianpan.Text & "',add1='" & txtaddress1.Text & "',add2='" & txtaddress2 & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',state_id='" & MyStateCode & "',per_state_id='" & MyPerStateCode & "',pincode='" & txtpin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & TxtStd.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1 & "',tel2='" & TxtTel2.Text & "',mobile_no='" & TxtMobile.Text & "',fax ='" & TxtFax.Text & "',email='" & TxtEmail & "',"
             sql = sql & " tele_Act_statement ='" & ActSt & "' ,tele_newsletter='" & News & "',tele_statutory_info='" & sInfo & "',de_ni_pan='" & NIPan & "',de_ni_addproof='" & NIAdd & "',de_ni_power='" & NIPower & "',de_i_pan='" & IPan & "',de_i_add_proof='" & IAdd & "',client_lastname='" & txtlstname & "',loggeduserid='" & Glbloginid & "',business_code=" & txtbusicode & ",act_cat=" & ActCat1 & ""
             sql = sql & " where client_codekyc=" & Trim(txtclientcodeold.Text) & ""
             MyConn.Execute (sql)
             MyClientCode = SqlRet("select client_Code from client_test where substr(client_codekyc,1,8)=substr(" & Trim(txtclientcodeold.Text) & ",1,8) ")
             MyMainClientCode = SqlRet("select client_Code from client_test where substr(client_codekyc,1,8)=substr(" & Trim(txtclientcodeold.Text) & ",1,8) ")
             RsKyc.Close
        Else
             sql = "insert into client_test(PER_PINCODE,OFFICE_EMAIL,per_add1,per_add2,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,NATURE_BUSINESS,NATURE_TRUST,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,others1,"
             sql = sql & " title,TITLE_FATHER_SPOUSE,client_name,FATHER_SPOUSE_NAME,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,MARITAL_STATUS,LEAD_SOURCE,"
             sql = sql & " a_enclosed_pan,a_enclosed_60,a_enclosed_61,G_enclosed_pan,g_enclosed_60,g_enclosed_61,a_kyc_status,g_kyc_status,"
             sql = sql & " g_name,g_nationality,g_pan,add1,add2,city_id,per_city_id,state_id,per_state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,fax,email,"
             sql = sql & " tele_Act_statement,tele_newsletter,tele_statutory_info,de_ni_pan,de_ni_addproof,de_ni_power,de_i_pan,de_i_add_proof,client_lastname,loggeduserid,business_code,timest,client_codekyc,act_cat,AADHAR_CARD_NO) "
             sql = sql & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & txtaddress1_P.Text & "','" & txtaddress2_P.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "','" & cmbnatureofbusiness.Text & "','" & cmbnatureoftrust.Text & "'," & Val(MyBranch) & ", " & Val(txtclientcodeold.Text) & ",'" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbstatus.Text & "','" & txtAcOther.Text & "',"
             sql = sql & " '" & Trim(MyOccCode) & "','" & TxtAcOther1.Text & "','" & cmbtitle.Text & "','" & cmbtitle_Spouse.Text & "','" & txtclientname.Text & "','" & TxtFatherSpouseName.Text & "','" & cmbclientn.Text & "','" & txtclientpan.Text & "','" & cmbincome.Text & "','" & cmbinvestment.Text & "','" & txttitle.Text & "','" & cmbresinri.Text & "',to_date('" & Trim(dob.Text) & "','DD-MM-YYYY'),'" & cmbgender.Text & "','" & CmbMeritalStatus.Text & "','" & CmbLeadSource.Text & "',"
             sql = sql & " '" & StrEnclosedPanA & "','" & StrEnclosed60A & "','" & StrEnclosed61A & "','" & StrEnclosedPanG & "','" & StrEnclosed60G & "','" & StrEnclosed61G & "','" & cmbkyc.Text & "','" & cmbkyc1.Text & "',"
             sql = sql & " '" & txtguardianname.Text & "','" & txtguardiann.Text & "','" & txtguardianpan.Text & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtpin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
             If TxtStd.Text = "" Then
                 sql = sql & "0 ,"
             Else
                 sql = sql & " '" & TxtStd.Text & "',"
             End If
             If txtstd1.Text = "" Then
                 sql = sql & "0 ,"
             Else
                 sql = sql & " '" & txtstd1.Text & "',"
             End If
             If TxtTel1.Text = "" Then
                 sql = sql & "0 ,"
             Else
                 sql = sql & " '" & TxtTel1 & "',"
             End If
             If TxtTel2.Text = "" Then
                 sql = sql & "0 ,"
             Else
                 sql = sql & " '" & TxtTel2.Text & "',"
             End If
             If TxtMobile.Text = "" Then
                 sql = sql & "'',"
             Else
                 sql = sql & " '" & TxtMobile.Text & "',"
             End If
             If TxtFax.Text = "" Then
                 sql = sql & " 0 ,"
             Else
                 sql = sql & " '" & TxtFax.Text & "',"
             End If
             sql = sql & " '" & TxtEmail & "', "
             sql = sql & " '" & ActSt & "','" & News & "','" & sInfo & "','" & NIPan & "','" & NIAdd & "','" & NIPower & "','" & IPan & "','" & IAdd & "','" & txtlstname & "','" & Glbloginid & "','" & txtbusicode & "',SYSDATE," & txtclientcodeold.Text & "," & ActCat1 & ","
             If txtaddharCardNo.Text <> "" Then
                sql = sql & " " & txtaddharCardNo.Text & " "
             Else
                sql = sql & " '' "
             End If
             sql = sql & " )"
             'Insert New Acount In Client_test
             MyConn.Execute (sql)
             Set clcode = New ADODB.Recordset
             '-------Getting Last Generated Account Code -----------------
             sql = " select 'AH'||max(TO_NUMBER(SUBSTR(client_code,3,LENGTH(CLIENT_CODE)))) CLIENT_CODE from client_test where upper(trim(client_name))='" & UCase(Trim(txtclientname.Text)) & "' "
             If Trim(txtaddress1.Text <> "") Then
                 sql = sql & " and upper(Trim(add1))='" & UCase(Trim(txtaddress1.Text)) & "'"
             End If
             If Trim(txtaddress2.Text) <> "" Then
                 sql = sql & " and upper(Trim(add2))='" & UCase(Trim(txtaddress2.Text)) & "'"
             End If
             If txtbusicode.Text <> "" Then
                sql = sql & " and upper(Trim(business_code))='" & UCase(Trim(txtbusicode.Text)) & "'"
             End If
             If MyBranch <> "" Then
                sql = sql & " and upper(Trim(branch_code))='" & UCase(Trim(MyBranch)) & "'"
             End If
             If TxtMobile.Text <> "" Then
                sql = sql & " and mobile_no='" & TxtMobile & "'"
             End If
             If txtclientpan.Text <> "" Then
                sql = sql & " and upper(trim(client_pan))='" & UCase(Trim(txtclientpan.Text)) & "'"
             Else
                sql = sql & " and Client_pan is null "
             End If
             Set clcode = MyConn.Execute(sql)
             If Not clcode.EOF Then
                 MsgBox "Your Unique Client Code is :-->> " & clcode(0), vbInformation, "WEALTH MAKER"
                 lblclientcode.Caption = clcode(0)
                 MyClientCode = clcode(0)
                 MyMainClientCode = clcode(0)
                 DoEvents
             End If
             MyConn.Execute ("update client_test set OPTCAR='" & StrCarStatus & "' ,main_code='" & MyClientCode & "' where client_code='" & MyClientCode & "'")
        End If
  End If
End If
If RsKyc.State = 1 Then RsKyc.Close
'UPDATE CLIENT_MASTER AND INVESTOR MASTER FOR  EXISTING CLIENT (WEATHER IT ISKYC OR NOT)
If Len(txtclientcodeold.Text) >= 11 And Right(txtclientcodeold.Text, 3) = "001" Then 'Client_Master Ko Update In Both Case
        MySourceId = Trim(Left(txtclientcodeold.Text, 8)) 'UPDATE CLIENT_MASTER  FOR EXISTING CLIENT FOR MAIN ACCOUNT
        Ky = "update client_master "
        Ky = Ky & " set "
        Ky = Ky & " kyc='YES' where client_code = " & Trim(Left(txtclientcodeold.Text, 8)) & ""
        MyConn.Execute (Ky)
        '----------------------------------------------------------------------------
        sql = "update investor_master set KYC='YES'"
        sql = sql & " where  INV_CODE=" & Trim(txtclientcodeold.Text) & ""
        MyConn.Execute (sql)
        MyConn.Execute ("UPDATE CLIENT_TEST SET CLIENT_CODEKYC=" & Trim(txtclientcodeold.Text) & " WHERE CLIENT_CODE='" & MyClientCode & "'")
ElseIf Len(txtclientcodeold.Text) >= 11 And Right(txtclientcodeold.Text, 3) <> "001" Then
        MySourceId = Trim(Left(txtclientcodeold.Text, 8))
        '-----------------------------------UPDATE KYC-------------------------------------------------
        Ky = "update client_master "
        Ky = Ky & " set "
        Ky = Ky & " kyc='YES' where CLIENT_CODE=" & Trim(Mid(txtclientcodeold.Text, 1, 8)) & ""
        MyConn.Execute (Ky)
        '----------------------------------------------------------------------------
        sql = "update investor_master set KYC='YES'"
        sql = sql & " where  INV_CODE=" & Trim(txtclientcodeold.Text) & ""
        MyConn.Execute (sql)
        sql = "   update client_test set client_codekyc=" & Trim(txtclientcodeold.Text) & " where client_code='" & MyClientCode & "' "
        MyConn.Execute sql
End If 'INSERT INTO CLIENT_MASTER FOR NEW ACCOUNT
If txtclientcodeold.Text = "" Then
        sql = "insert into client_test(PER_PINCODE,OFFICE_EMAIL,PER_ADD1,PER_ADD2,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,NATURE_BUSINESS,NATURE_TRUST,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,others1,"
        sql = sql & " title,TITLE_FATHER_SPOUSE,client_name,FATHER_SPOUSE_NAME,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,MARITAL_STATUS,LEAD_SOURCE,"
        sql = sql & " a_enclosed_pan,a_enclosed_60,a_enclosed_61,G_enclosed_pan,g_enclosed_60,g_enclosed_61,a_kyc_status,g_kyc_status,"
        sql = sql & " g_name,g_nationality,g_pan,add1,add2,city_id,per_city_id,state_id,per_state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,fax,email,"
        sql = sql & " tele_Act_statement,tele_newsletter,tele_statutory_info,de_ni_pan,de_ni_addproof,de_ni_power,de_i_pan,de_i_add_proof,client_lastname,loggeduserid,business_code,timest,act_cat,AADHAR_CARD_NO) "
        sql = sql & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & txtaddress1_P.Text & "','" & txtaddress2_P.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "','" & cmbnatureofbusiness.Text & "','" & cmbnatureoftrust.Text & "'," & Val(MyBranch) & ", " & Val(txtclientcodeold.Text) & ",'" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbstatus.Text & "','" & txtAcOther.Text & "',"
        sql = sql & " " & Trim(MyOccCode) & ",'" & TxtAcOther1.Text & "','" & cmbtitle.Text & "','" & cmbtitle_Spouse.Text & "','" & txtclientname.Text & "','" & TxtFatherSpouseName.Text & "','" & cmbclientn.Text & "','" & txtclientpan.Text & "','" & cmbincome.Text & "','" & cmbinvestment.Text & "','" & txttitle.Text & "','" & cmbresinri.Text & "',"
        If dob.Text <> "__/__/____" Then
            sql = sql & " to_date('" & Trim(dob.Text) & "','DD-MM-YYYY')"
        Else
            sql = sql & " null"
        End If
        sql = sql & " ,'" & cmbgender.Text & "','" & CmbMeritalStatus.Text & "','" & CmbLeadSource.Text & "',"
        sql = sql & " '" & StrEnclosedPanA & "','" & StrEnclosed60A & "','" & StrEnclosed61A & "','" & StrEnclosedPanG & "','" & StrEnclosed60G & "','" & StrEnclosed61G & "','" & cmbkyc.Text & "','" & cmbkyc1.Text & "',"
        sql = sql & " '" & txtguardianname.Text & "','" & txtguardiann.Text & "','" & txtguardianpan.Text & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtpin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
        If TxtStd.Text = "" Then
            sql = sql & "0 ,"
        Else
            sql = sql & " " & TxtStd.Text & ","
        End If
        If txtstd1.Text = "" Then
            sql = sql & "0 ,"
        Else
            sql = sql & " " & txtstd1.Text & ","
        End If
        If TxtTel1.Text = "" Then
            sql = sql & "0 ,"
        Else
            sql = sql & " '" & TxtTel1 & "',"
        End If
        If TxtTel2.Text = "" Then
            sql = sql & "0 ,"
        Else
            sql = sql & " '" & TxtTel2.Text & "',"
        End If
        If TxtMobile.Text = "" Then
            sql = sql & "'',"
        Else
            sql = sql & " '" & TxtMobile.Text & "',"
        End If
        If TxtFax.Text = "" Then
            sql = sql & " 0 ,"
        Else
            sql = sql & " " & TxtFax.Text & ","
        End If
        sql = sql & " '" & TxtEmail & "', "
        sql = sql & " '" & ActSt & "','" & News & "','" & sInfo & "','" & NIPan & "','" & NIAdd & "','" & NIPower & "','" & IPan & "','" & IAdd & "','" & txtlstname & "','" & Glbloginid & "','" & txtbusicode & "',SYSDATE," & ActCat1 & ","
        If txtaddharCardNo.Text <> "" Then
            sql = sql & " " & txtaddharCardNo.Text & " "
        Else
            sql = sql & " '' "
        End If
        sql = sql & ")"
        MyConn.Execute (sql)
        Set clcode = New ADODB.Recordset
        sql = " select 'AH'||max(TO_NUMBER(SUBSTR(client_code,3,LENGTH(CLIENT_CODE)))) CLIENT_CODE from client_test where upper(trim(client_name))='" & UCase(Trim(txtclientname.Text)) & "' "
        If txtaddress1.Text <> "" Then
           sql = sql & " and upper(Trim(add1))='" & UCase(Trim(txtaddress1.Text)) & "'"
        End If
        If txtaddress2.Text <> "" Then
           sql = sql & " and upper(Trim(add2))='" & UCase(Trim(txtaddress2.Text)) & "'"
        End If
        If txtbusicode.Text <> "" Then
          sql = sql & " and upper(Trim(business_code))='" & UCase(Trim(txtbusicode.Text)) & "'"
        End If
        If MyBranch <> "" Then
          sql = sql & " and upper(Trim(branch_code))='" & UCase(Trim(MyBranch)) & "'"
        End If
        If TxtMobile.Text <> "" Then
          sql = sql & " and mobile_no='" & TxtMobile & "'"
        End If
        If txtclientpan.Text <> "" Then
          sql = sql & " and upper(trim(client_pan))='" & UCase(Trim(txtclientpan.Text)) & "'"
        Else
          sql = sql & " and Client_pan is null "
        End If
        Set clcode = MyConn.Execute(sql)
        If Not clcode.EOF Then
            MsgBox "Your Unique Client Code is :-->> " & clcode(0), vbInformation, "WEALTH MAKER"
            lblclientcode.Caption = clcode(0)
            MyClientCode = clcode(0)
            MyMainClientCode = clcode(0)
            DoEvents
        End If
        MyConn.Execute ("update client_test set OPTCAR='" & StrCarStatus & "' ,main_code='" & MyClientCode & "' where client_code='" & MyMainClientCode & "'")
        
        Ky = "insert into client_master (PER_PINCODE,OFFICE_EMAIL,PER_ADD1,PER_ADD2,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,client_title,TITLE_FATHER_SPOUSE,CLIENT_LNAME,category_id,investor_code,GUEST_CD,client_name,FATHER_SPOUSE_NAME,address1,address2,fax,email,pan,dob,mobile,pincode,gender,MARITAL_STATUS,LEAD_SOURCE,city_name,std1,std2,phone1,phone2,status,occ_id,city_id,per_city_id,comm_id,comm_int_id,kyc,sourceid,rm_code,loggeduserid,act_cat)"
        Ky = Ky & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & txtaddress1_P.Text & "','" & txtaddress2_P.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "','" & cmbtitle.Text & "','" & cmbtitle_Spouse.Text & "','" & txtlstname.Text & "','" & ClCat & "','" & invCat & "','" & TguestCode & "','" & txtclientname & "','" & TxtFatherSpouseName.Text & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "',"
        If TxtFax <> "" Then
            Ky = Ky & " " & TxtFax & ","
        Else
            Ky = Ky & " 0 ,"
        End If
        Ky = Ky & " '" & TxtEmail & "','" & txtclientpan & "',"
        If dob.Text <> "__/__/____" Then
            Ky = Ky & " '" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        Else
            Ky = Ky & " null,"
        End If
        If TxtMobile.Text <> "" Then
            Ky = Ky & " '" & TxtMobile & "',"
        Else
            Ky = Ky & " '0',"
        End If
        If txtpin.Text <> "" Then
            Ky = Ky & " '" & txtpin & "',"
        Else
            Ky = Ky & " '',"
        End If
        Ky = Ky & " '" & Left(Trim(cmbgender.Text), 1) & "','" & CmbMeritalStatus.Text & "','" & CmbLeadSource.Text & "',"
        Ky = Ky & " '" & Trim(Mid(cmbcity.Text, 1, 50)) & "',"
        If TxtStd.Text <> "" Then
            Ky = Ky & " '" & TxtStd.Text & "',"
        Else
            Ky = Ky & " '',"
        End If
        If txtstd1.Text <> "" Then
            Ky = Ky & " '" & txtstd1.Text & "',"
        Else
            Ky = Ky & " '',"
        End If
        If TxtTel1.Text <> "" Then
            Ky = Ky & " '" & TxtTel1.Text & "',"
        Else
            Ky = Ky & " '',"
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & "'" & TxtTel2.Text & "',"
        Else
            Ky = Ky & "'',"
        End If
        Ky = Ky & "'" & cmbstatus.Text & "'," & MyOccCode & ",'" & MyCityCode & "','" & MyPerCityCode & "',"
        If cmbprefcommmode.Tag <> "" Then
            Ky = Ky & " " & cmbprefcommmode.Tag & ","
        Else
            Ky = Ky & " 0, "
        End If
        If cmbcomminterval.Tag <> "" Then
            Ky = Ky & " " & cmbcomminterval.Tag & ","
        Else
            Ky = Ky & " 0, "
        End If
        Ky = Ky & " 'YESP'," & MyBranch & "," & MyRmCode & ",'" & Glbloginid & "'," & ActCat1 & ")"
        MyConn.Execute (Ky)
        sql = " select max(client_code) from client_master WHERE  upper(TRIM(client_name)) =  '" & UCase(Trim(txtclientname)) & "' "
        If txtclientpan.Text <> "" Then
            sql = sql & " AND UPPER(TRIM(pan))='" & Trim(txtclientpan.Text) & "' "
        End If
        If txtaddress1.Text <> "" Then
           sql = sql & " and upper(Trim(ADDRESS1))='" & UCase(Trim(txtaddress1.Text)) & "'"
        End If
        If txtaddress2.Text <> "" Then
           sql = sql & " and upper(Trim(ADDRESS2))='" & UCase(Trim(txtaddress2.Text)) & "'"
        End If
        If TxtMobile.Text <> "" Then
            sql = sql & " AND MOBILE='" & TxtMobile & "'"
        End If
        If MyBranch <> "" Then
            sql = sql & " AND SOURCEID=" & MyBranch & ""
        End If
        If MyRmCode <> "" Then
            sql = sql & " AND RM_CODE=" & MyRmCode & " "
        End If
        Set GetRecentCLCode = MyConn.Execute(sql)
        If Not GetRecentCLCode.EOF Then
            MySourceId = GetRecentCLCode(0)
        End If
        '------------------------------------------------------INSERT INTO INVESTOR MASTER SELF-------------------------------------
        Ky = "insert into investor_master (occupation_id,INVESTOR_CODE,investor_title,branch_code,investor_name,address1,address2,fax,email,pan,dob,mobile,pincode,gender,phone1,phone2,status,city_id,kyc,source_id,rm_code,AADHAR_CARD_NO)"
        Ky = Ky & " values(" & MyOccCode & ",1,'" & cmbtitle.Text & "','" & MyBranch & "', '" & txtclientname & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "',"
        If TxtFax <> "" Then
            Ky = Ky & " " & TxtFax & ","
        Else
            Ky = Ky & " 0 ,"
        End If
        Ky = Ky & " '" & TxtEmail & "','" & txtclientpan & "',"
        If dob.Text <> "__/__/____" Then
            Ky = Ky & " '" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        Else
            Ky = Ky & " null,"
        End If
        If TxtMobile.Text <> "" Then
            Ky = Ky & " '" & TxtMobile & "',"
        Else
            Ky = Ky & " '',"
        End If
        If txtpin.Text <> "" Then
            Ky = Ky & " '" & txtpin & "',"
        Else
            Ky = Ky & " ' ',"
        End If
        Ky = Ky & " '" & Left(Trim(cmbgender.Text), 1) & "',"
        If TxtTel1.Text <> "" Then
            Ky = Ky & " '" & TxtStd.Text & "-" & TxtTel1.Text & "', "
        Else
            Ky = Ky & " '', "
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & " '" & txtstd1.Text & "-" & TxtTel2.Text & "', "
        Else
            Ky = Ky & " '', "
        End If
        Ky = Ky & " '" & cmbstatus.Text & "','" & MyCityCode & "',"
        Ky = Ky & " 'YESP'," & MySourceId & "," & MyRmCode & ","
        If txtaddharCardNo.Text <> "" Then
            Ky = Ky & " " & txtaddharCardNo.Text & " "
        Else
            Ky = Ky & " '' "
        End If
        Ky = Ky & " )"
        MyConn.Execute (Ky)
        
        sql = "   update client_test set client_codekyc=( select max(inv_code) from investor_master where source_id='" & MySourceId & "') "
        sql = sql & "       where client_code='" & MyClientCode & "' "
        MyConn.Execute sql
        MyConn.Execute ("update client_test set main_code='" & MyClientCode & "' where client_code='" & MyClientCode & "'")
End If  'end of searching in clientmaster
If RsKyc.State = 1 Then RsKyc.Close
''''''''''''''''''''''''''----------------------------------------------SAVE THE ENTRY OF GRID---------------------------
''''''''''''''''''''''''''-insert Exist Kyc Investor INTO CLIENT_TEST AND UPDATE INVESTOR MASTER-----------------------
For i = 1 To VSFCommGrdE.Rows - 1
    If VSFCommGrdE.TextMatrix(i, 1) <> "" Then
        MySourceId = Mid(VSFCommGrdE.TextMatrix(i, 1), 1, 8)
    End If
    '----------------------------Checking Duplicate PAN of Grid implement by Shashi Ranjan--------------
     If VSFCommGrdE.TextMatrix(i, 1) = "" Then
        If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
            If SqlRet("select count(*) from client_test where upper(client_pan) = upper('" & Trim(VSFCommGrdE.TextMatrix(i, 5)) & "')") > 0 Then
                MsgBox "This Pan No is already Exist"
                Exit Sub
            End If
        End If
     Else
        If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
            If SqlRet("select count(*) from client_test where client_codekyc <>'" & Trim(VSFCommGrdE.TextMatrix(i, 1)) & "' and upper(client_pan) = upper('" & Trim(VSFCommGrdE.TextMatrix(i, 5)) & "')") > 0 Then
                MsgBox "This Pan No is already Exist"
                Exit Sub
            End If
        End If
     End If
     
     
   '---------------------------------------------------------------------------------------------------
   If VSFCommGrdE.TextMatrix(i, 1) = "" Then 'INVESTOR_CODE IS BLANK
        If VSFCommGrdE.TextMatrix(i, 2) <> "" Then
            sql = "insert into client_test(PER_PINCODE,OFFICE_EMAIL,PER_ADD1,PER_ADD2,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,relation_id,inv_kyc,IS_NOMINEE,g_name,g_pan,others1,"
            sql = sql & " title,client_name,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,"
            sql = sql & " add1,add2,city_id,per_city_id,state_id,per_state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,fax,email,"
            sql = sql & " loggeduserid,business_code,timest,main_code,AADHAR_CARD_NO,NOMINEE_PER) "
            sql = sql & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & txtaddress1_P.Text & "','" & txtaddress2_P.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "'," & Val(MyBranch) & ", " & Val(txtclientcodeold.Text) & ",'" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbstatus.Text & "','" & txtAcOther.Text & "',"
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                If Temp_Id(1) <> "" Then
                    sql = sql & " " & Temp_Id(1) & ","
                Else
                    sql = sql & " 0,"
                End If
            Else
                sql = sql & " 0,"
            End If
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
                If Temp_Id(1) <> "" Then
                    sql = sql & " " & Temp_Id(1) & ","
                Else
                    sql = sql & " 0 ,"
                End If
            Else
                sql = sql & " 0 ,"
            End If
            
            If VSFCommGrdE.TextMatrix(i, 11) <> "" Then
                sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 11) & "',"
            Else
                sql = sql & " '',"
            End If
            
            If VSFCommGrdE.TextMatrix(i, 20) <> "" Then
                sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 20) & "',"
            Else
                sql = sql & " '',"
            End If
            
            sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 12) & "', '" & VSFCommGrdE.TextMatrix(i, 13) & "',"
            sql = sql & " null,'" & VSFCommGrdE.TextMatrix(i, 17) & "','" & VSFCommGrdE.TextMatrix(i, 2) & "',null,'" & VSFCommGrdE.TextMatrix(i, 5) & "',null,null,null,null,to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY'),'" & VSFCommGrdE.TextMatrix(i, 19) & "'," 'GENDER PANKAJ
            sql = sql & " '" & txtaddress1.Text & "','" & txtaddress2.Text & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtpin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            If TxtStd.Text = "" Then
                sql = sql & "'0' ,"
            Else
                sql = sql & " '" & TxtStd.Text & "',"
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
            If TxtMobile.Text = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            End If
            If TxtFax.Text = "" Then
                sql = sql & " '0' ,"
            Else
                sql = sql & " '" & TxtFax.Text & "',"
            End If
            sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 4) & "', "
            sql = sql & " '" & Glbloginid & "','" & txtbusicode & "',SYSDATE,"
            sql = sql & "'" & MyClientCode & "',"
            
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 18) & ","
            Else
                sql = sql & " '',"
            End If
            
            If VSFCommGrdE.TextMatrix(i, 21) <> "" Then
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 21) & ""
            Else
                sql = sql & " ''"
            End If
            
            sql = sql & " )"
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Insert New Acount In Client_test
            MyConn.Execute (sql)
            ''''''''''''''''''''''''''''''1 Client And 1 Investor hase Been Insert into Client_master and Investor_Master
            'MySourceiD=ClientCode of new insert client in clientmaster
            '-------------------------------------------------ADD ALL GRID VALUE IN INVESTOR _MASTER --------------------
            Ky = "insert into investor_master (INVESTOR_CODE,investor_title,branch_code,investor_name,address1,address2,fax,email,pan,dob,mobile,AADHAR_CARD_NO,pincode,gender,phone1,phone2,status,city_id,kyc,source_id,rm_code,rel_id,occupation_id)"
            Ky = Ky & " values(1,'" & VSFCommGrdE.TextMatrix(i, 17) & "','" & MyBranch & "', '" & VSFCommGrdE.TextMatrix(i, 2) & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "',"
            If TxtFax <> "" Then
                Ky = Ky & " " & TxtFax & ","
            Else
                Ky = Ky & " 0 ,"
            End If
            Ky = Ky & " '" & VSFCommGrdE.TextMatrix(i, 4) & "','" & VSFCommGrdE.TextMatrix(i, 5) & "',to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY'),"
            If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
                Ky = Ky & " '" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            Else
                Ky = Ky & " '',"
            End If
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                Ky = Ky & " " & VSFCommGrdE.TextMatrix(i, 18) & ","
            Else
                Ky = Ky & " '',"
            End If
            If txtpin.Text <> "" Then
                Ky = Ky & " '" & txtpin & "',"
            Else
                Ky = Ky & " ' ',"
            End If
            Ky = Ky & " '" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "'," ''" & Left(Trim(cmbgender.Text), 1) & "' GENDER PANKAJ
            If TxtTel1.Text <> "" Then
            Ky = Ky & " '" & TxtStd.Text & "-" & TxtTel1.Text & "', "
            Else
                Ky = Ky & " '' ,"
            End If
            If TxtTel2.Text <> "" Then
                Ky = Ky & " '" & txtstd1.Text & "-" & TxtTel2.Text & "', "
            Else
                Ky = Ky & " '', "
            End If
            Ky = Ky & " '" & cmbstatus.Text & "','" & MyCityCode & "',"
            Ky = Ky & " 'YESP'," & MySourceId & "," & MyRmCode & ", "
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
                If Temp_Id(1) <> "" Then
                    Ky = Ky & " " & Temp_Id(1) & ","
                Else
                    Ky = Ky & " 0 ,"
                End If
            Else
                 Ky = Ky & " 0 ,"
            End If
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                If Temp_Id(1) <> "" Then
                    Ky = Ky & " " & Temp_Id(1) & ""
                Else
                    Ky = Ky & " 0 "
                End If
            Else
                 Ky = Ky & " 0 "
            End If
            Ky = Ky & " )"
            MyConn.Execute (Ky)
            Set clcode = New ADODB.Recordset
            Set clcode = MyConn.Execute("select 'AH'||max(TO_NUMBER(SUBSTR(client_code,3,LENGTH(CLIENT_CODE)))) CLIENT_CODE from client_test where  MAIN_CODE='" & MyClientCode & "'")
            If Not clcode.EOF Then
                  sql = "   update client_test set client_codekyc=( select max(inv_code) from investor_master where source_id='" & MySourceId & "') "
                  sql = sql & "  where client_code='" & clcode(0) & "' "
                  MyConn.Execute sql
            End If
            clcode.Close
            rs_max_inv.open "SELECT TO_NUMBER(MAX(SUBSTR(INV_CODE,9,13))) FROM INVESTOR_MASTER WHERE SOURCE_ID=" & MySourceId, MyConn, adOpenForwardOnly
            If Not rs_max_inv.EOF Then
                MAX_INV_CD = rs_max_inv(0)
            End If
            If MAX_INV_CD >= 999 Then
                cnt = Format(MAX_INV_CD, "00000")
            Else
                cnt = Format(MAX_INV_CD, "000")
            End If
            rs_max_inv.Close
            Set rs_max_inv = Nothing
        Else
             Exit For
        End If
    Else ''''''''''''''IF WE CHOOSE EXITING INVESTOR THEN IF IT IS MAIN INVESTOR(001) THEN IT IS UPDATED IN CLIENT MASTER
        If VSFCommGrdE.TextMatrix(i, 1) <> "" And Trim(Right(VSFCommGrdE.TextMatrix(i, 1), 3)) = "001" Then 'We Choose Existing Investor For Update
            Ky = "update client_master "
            Ky = Ky & " set  kyc='YES' where client_code= " & Trim(Mid(VSFCommGrdE.TextMatrix(i, 1), 1, 8)) & ""
            MyConn.Execute (Ky)
        Else
            Ky = "update client_master "
            Ky = Ky & " set  kyc='YES' where client_code= " & Trim(Mid(VSFCommGrdE.TextMatrix(i, 1), 1, 8)) & ""
            MyConn.Execute (Ky)
        End If
        'OTHER WISE INSERT IN CLIENT_TEST IF IT IS NOT KYC AND UPDATE CLIENT TEST IF IT IS ALREADY KYC AND UPDATE INVESTOR MASTER IN BOTH CASE
        AlreadyKyc = SqlRet("select nvl(count(*),0) from investor_master where inv_code=" & VSFCommGrdE.TextMatrix(i, 1) & " and kyc='YES'")
        If AlreadyKyc > 0 Then 'It Means It Is KYC
            MySourceId = Mid(VSFCommGrdE.TextMatrix(i, 1), 1, 8)
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
            End If
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id1 = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''update client test-------mayank
            'GENDER PANKAJ
            sql = "update client_test set PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "',GENDER='" & VSFCommGrdE.TextMatrix(i, 19) & "',clientCode_old=" & Val(txtclientcodeold.Text) & ",tradingid='" & txttradingid.Text & "',benefi_id='" & txtdp_bid.Text & "',"
            If Temp_Id(1) <> "" Then
                sql = sql & " occ_id=" & Temp_Id(1) & ","
            End If
            If Temp_Id1(1) <> "" Then
                sql = sql & " relation_id = " & Temp_Id1(1) & ","
            End If
            sql = sql & " inv_kyc='YES',IS_NOMINEE='" & VSFCommGrdE.TextMatrix(i, 20) & "',g_name='" & VSFCommGrdE.TextMatrix(i, 12) & "',g_pan= '" & VSFCommGrdE.TextMatrix(i, 13) & "',"
            sql = sql & " title='" & VSFCommGrdE.TextMatrix(i, 17) & "',client_name='" & VSFCommGrdE.TextMatrix(i, 2) & "',nationality='" & cmbclientn.Text & "',client_pan='" & VSFCommGrdE.TextMatrix(i, 5) & "',resident_nri='" & cmbresinri.Text & "',dob=TO_DATE('" & Trim(VSFCommGrdE.TextMatrix(i, 6)) & "','DD-MM-YYYY'),"
            sql = sql & " add1='" & txtaddress1 & "',add2='" & txtaddress2 & "',city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',state_id='" & MyStateCode & "',per_state_id='" & MyPerStateCode & "',pincode='" & txtpin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & TxtStd.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1 & "',tel2='" & TxtTel2.Text & "',mobile_no='" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                sql = sql & " AADHAR_CARD_NO=" & VSFCommGrdE.TextMatrix(i, 18) & ","
            Else
                sql = sql & " AADHAR_CARD_NO='',"
            End If
            
            If VSFCommGrdE.TextMatrix(i, 21) <> "" Then
                sql = sql & " NOMINEE_PER=" & VSFCommGrdE.TextMatrix(i, 21) & ","
            Else
                sql = sql & " NOMINEE_PER='',"
            End If
            
            sql = sql & " fax ='" & TxtFax.Text & "',email='" & VSFCommGrdE.TextMatrix(i, 4) & "',"
            sql = sql & " loggeduserid='" & Glbloginid & "',business_code=" & txtbusicode & ""
            sql = sql & " where client_codekyc='" & Trim(VSFCommGrdE.TextMatrix(i, 1)) & "'"
            MyConn.Execute (sql)
        Else
            sql = "insert into client_test(PER_PINCODE,OFFICE_EMAIL,PER_ADD1,PER_ADD2,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,others1,"
            sql = sql & " title,client_name,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,"
            sql = sql & " add1,add2,city_id,per_city_id,state_id,per_state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,AADHAR_CARD_NO,fax,email,"
            sql = sql & " loggeduserid,business_code,timest,main_code,client_codekyc) "
            sql = sql & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & txtaddress1_P.Text & "','" & txtaddress2_P.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "'," & Val(MyBranch) & ", " & Val(txtclientcodeold.Text) & ",'" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbstatus.Text & "','" & txtAcOther.Text & "',"
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                If Temp_Id(1) <> "" Then
                    sql = sql & " " & Temp_Id(1) & ","
                End If
            End If
            sql = sql & " null,'" & VSFCommGrdE.TextMatrix(i, 17) & "','" & VSFCommGrdE.TextMatrix(i, 2) & "',null,'" & VSFCommGrdE.TextMatrix(i, 5) & "',null,null,null,null,to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY'),'" & VSFCommGrdE.TextMatrix(i, 19) & "'," 'CHANGE PANKAJ
            sql = sql & " '" & txtaddress1 & "','" & txtaddress2 & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtpin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            If TxtStd.Text = "" Then
                sql = sql & "0 ,"
            Else
                sql = sql & " '" & TxtStd.Text & "',"
            End If
            If txtstd1.Text = "" Then
                sql = sql & "0 ,"
            Else
                sql = sql & " '" & txtstd1.Text & "',"
            End If
            If TxtTel1.Text = "" Then
                sql = sql & "0 ,"
            Else
                sql = sql & " '" & Replace(TxtTel1.Text, "-", "") & "',"
            End If
            If TxtTel2.Text = "" Then
                sql = sql & "0 ,"
            Else
                sql = sql & " '" & TxtTel2.Text & "',"
            End If
            If TxtMobile.Text = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            End If
            If txtaddharCardNo.Text = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 18) & ","
            End If
            If TxtFax.Text = "" Then
                sql = sql & " 0 ,"
            Else
                sql = sql & " " & TxtFax.Text & ","
            End If
            sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 4) & "', "
            sql = sql & " '" & Glbloginid & "','" & txtbusicode & "',SYSDATE,"
            sql = sql & "'" & MyClientCode & "'," & VSFCommGrdE.TextMatrix(i, 1) & ")"
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Insert New Acount In Client_test
            MyConn.Execute (sql)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Update ClientMAster
        End If
            Ky = "update investor_master "
            Ky = Ky & " set investor_title='" & VSFCommGrdE.TextMatrix(i, 17) & "',investor_name='" & VSFCommGrdE.TextMatrix(i, 2) & "',address1='" & Replace(txtaddress1, ",", "") & "',address2='" & Replace(txtaddress2, ",", "") & "',"
            If TxtFax <> "" Then
                Ky = Ky & " fax=" & TxtFax & ","
            Else
                Ky = Ky & " fax=0 ,"
            End If
            Ky = Ky & " email='" & VSFCommGrdE.TextMatrix(i, 4) & "',pan='" & VSFCommGrdE.TextMatrix(i, 5) & "',dob=to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY'),"
            If TxtMobile.Text <> "" Then
                Ky = Ky & " mobile='" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            Else
                Ky = Ky & " mobile='',"
            End If
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                Ky = Ky & " AADHAR_CARD_NO=" & VSFCommGrdE.TextMatrix(i, 18) & ","
            Else
                Ky = Ky & " AADHAR_CARD_NO='',"
            End If
            If txtpin.Text <> "" Then
                Ky = Ky & " pincode='" & txtpin & "',"
            Else
                Ky = Ky & " pincode=' ',"
            End If
            Ky = Ky & " gender='" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "'," 'GENDER PANKAJ
            If TxtTel1.Text <> "" Then
                Ky = Ky & " phone1='" & TxtStd.Text & "-" & TxtTel1.Text & "', "
            Else
                Ky = Ky & " phone1='', "
            End If
            If TxtTel2.Text <> "" Then
                Ky = Ky & " phone2='" & txtstd1.Text & "-" & TxtTel2.Text & "', "
            Else
                Ky = Ky & " phone2='', "
            End If
            Ky = Ky & " status='" & cmbstatus.Text & "',city_id='" & MyCityCode & "',"
            Ky = Ky & " kyc='YES',source_id=" & MySourceId & ",rm_code=" & MyRmCode & ", "
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
                Ky = Ky & "  REL_ID= " & Temp_Id(1) & ","
            Else
                Ky = Ky & " REL_ID=0 ,"
            End If
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                Ky = Ky & " OCCUPATION_ID= " & Temp_Id(1) & ""
            Else
                Ky = Ky & " 0 "
            End If
            Ky = Ky & " where inv_code= " & Trim(VSFCommGrdE.TextMatrix(i, 1)) & " "
            MyConn.Execute (Ky)
            Ky = " update client_master "
            Ky = Ky & " set kyc='YES' where client_code= " & Trim(Mid(VSFCommGrdE.TextMatrix(i, 1), 1, 8)) & ""
            MyConn.Execute (Ky)
    End If
Next
MyConn.CommitTrans
''''''''''Save Car Details '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
iRow = 1
Dim CompanyCd As String
Dim RS As New ADODB.Recordset
Dim Modelid As String
Do While VSFCommGrdK.TextMatrix(iRow, 0) <> ""
   If VSFCommGrdK.TextMatrix(iRow, 7) = "UPD" Then
       Str = Split(VSFCommGrdK.TextMatrix(iRow, 1), "#")
       Modelid = Str(1)
       If VSFCommGrdK.TextMatrix(iRow, 4) <> "" Then
            Str = Split(VSFCommGrdK.TextMatrix(iRow, 4), "#")
            CompanyCd = Str(1)
       End If
       Mysql = "update Accounts_Car_ Details set model_id='" & Modelid & "'"
       Mysql = Mysql & ",company_Cd ='" & CompanyCd & "',login_id ='" & Glbloginid & "',"
       If VSFCommGrdK.TextMatrix(iRow, 3) = "  /    " Then
          Mysql = Mysql & "renewal_year=NULL,"
       Else
          Mysql = Mysql & "renewal_year=TO_DATE('" & (VSFCommGrdK.TextMatrix(iRow, 3)) & "','MM/YYYY'),"
       End If
       If VSFCommGrdK.TextMatrix(iRow, 2) = "  /    " Then
          Mysql = Mysql & "manif_year=NULL,"
       Else
          Mysql = Mysql & "manif_year='" & (VSFCommGrdK.TextMatrix(iRow, 2)) & "',remarks='" & (VSFCommGrdK.TextMatrix(iRow, 6)) & "' where main_code=" & MyMainClientCode
       End If
       MyConn.Execute Mysql
   ElseIf VSFCommGrdK.TextMatrix(iRow, 7) = "OLD" Then
       ' No action for old Record Uday
   Else
         Str = Split(VSFCommGrdK.TextMatrix(iRow, 1), "#")
         Modelid = Str(1)
         If VSFCommGrdK.TextMatrix(iRow, 4) <> "" Then
            Str = Split(VSFCommGrdK.TextMatrix(iRow, 4), "#")
            CompanyCd = Str(1)
         End If
         Mysql = "SELECT * FROM Accounts_Car_Details WHERE Main_code='" & MainAccountCode & "' and model_id='" & Modelid & "' and company_cd='" & CompanyCd & "'  "
         RS.open Mysql, MyConn
         If RS.EOF Then
               Mysql = "insert into Accounts_Car_Details(Main_Code,Model_id,company_cd,LOGIN_ID ,renewal_date,manif_year,ahcode,remarks) values('"
               Mysql = Mysql & MyMainClientCode & "','" & Modelid & "','" & CompanyCd & "','" & Glbloginid & "', "
               If VSFCommGrdK.TextMatrix(iRow, 3) = "  /    " Then
                  Mysql = Mysql & " NULL, "
               Else
                  Mysql = Mysql & "TO_DATE('" & (VSFCommGrdK.TextMatrix(iRow, 3)) & "','DD/MM/YYYY'),"
               End If
               If VSFCommGrdK.TextMatrix(iRow, 2) = "  /    " Then
                  Mysql = Mysql & " NULL), "
               Else
                  Mysql = Mysql & "'" & VSFCommGrdK.TextMatrix(iRow, 2) & "',"
               End If
               Mysql = Mysql & "'" & MyClientCode & "','" & (VSFCommGrdK.TextMatrix(iRow, 6)) & "')"
               MyConn.Execute Mysql
               RS.Close
        End If
   End If
   iRow = iRow + 1
Loop
If RS.State = 1 Then RS.Close
'--------------------------------------------------------------------------------------------------
MyConn.Execute ("update tb_doc_upload set inv_code='" & GetRecentCLCode(0) & "001" & "'  where  verification_flag='1' and rejection_status='0'  and punching_flag='0' and common_id='" & txtdocID.Text & "'")
cmdadd.Enabled = False
CmdSave.Enabled = True
frameAdvisory.Enabled = True
Call CMDRESET_Click
Exit Sub
errr:
MsgBox err.Description
Resume
End Sub