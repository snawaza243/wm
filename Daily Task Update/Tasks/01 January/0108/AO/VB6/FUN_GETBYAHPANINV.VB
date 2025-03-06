Public Sub cmdview_Click()
On Error Resume Next
Dim rsCity As New ADODB.Recordset
Dim rsState As New ADODB.Recordset
Dim RsPan As New ADODB.Recordset
Dim MyInvCode As String
Dim ClientCat As String
cmdAdd.Enabled = False
cmdUpdate.Enabled = True
CmdSave.Enabled = True
Dim Rs_Relation As New ADODB.Recordset
cmdUpdate.Enabled = True
Call SetGrid
Call SetGrid1
Dim MyCountryCode As String
Dim MyPerCountryCode As String
If Trim(txtclientcode.Text) = "" And Trim(txtpans.Text) = "" And txtclient.Text = "" Then
    MsgBox "Please Enter Account Code or PAN", vbInformation
    txtclientcode.SetFocus
    Exit Sub
End If

Set Rs_view = New ADODB.Recordset
If txtpans.Text <> "" Then
    If SRmCode = "" Then
        Rs_view.open "SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN(select MAIN_CODE from client_test   where  client_pan='" & txtpans & "'  AND BRANCH_CODE IN(" & Branches & ") UNION ALL select MAIN_CODE from client_test   where  client_pan='" & txtpans & "' AND BUSINESS_CODE='95829') AND CLIENT_CODE=MAIN_CODE", MyConn, adOpenForwardOnly
    Else
        Rs_view.open "SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN(select MAIN_CODE from client_test   where  client_pan='" & txtpans & "'  AND BRANCH_CODE IN(" & Branches & ") AND BUSINESS_CODE='" & Glbloginid & "' UNION ALL select MAIN_CODE from client_test   where  client_pan='" & txtpans & "' AND BUSINESS_CODE='95829') AND CLIENT_CODE=MAIN_CODE ", MyConn, adOpenForwardOnly
    End If
ElseIf txtclient.Text <> "" Then
    Rs_view.open "SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN(select MAIN_CODE from client_test   where  source_code='" & txtclient.Text & "'  AND BRANCH_CODE IN(" & Branches & ") UNION ALL select MAIN_CODE from client_test   where  client_pan='" & txtpans & "' AND BUSINESS_CODE='95829') AND CLIENT_CODE=MAIN_CODE", MyConn, adOpenForwardOnly
Else
    Rs_view.open "SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN (select MAIN_CODE from client_test   where  client_code=trim('" & txtclientcode & "')) AND CLIENT_CODE=MAIN_CODE", MyConn, adOpenForwardOnly
End If

'If Rs_view.State = 1 Then Rs_view.Close
If Not Rs_view.EOF Then
    txtclientcode.Text = Rs_view("main_code")
    txtclientcodeold.Text = IIf(IsNull(Rs_view.Fields("CLIENTCODE_OLD")), 0, Rs_view.Fields("CLIENTCODE_OLD"))
    MySourceId = Mid(Rs_view("client_codekyc"), 1, 8)
    MyOldBusiCode = Rs_view("business_code")
    
    MyCountryCode = SqlRet("select country_id from state_master where state_id=" & Rs_view.Fields("state_id") & "")
    MyPerCountryCode = SqlRet("select country_id from state_master where state_id=" & Rs_view.Fields("per_state_id") & "")
    
    MainAccountCode = SqlRet("select Main_code from client_test where client_code='" & Trim(txtclientcode.Text) & "'")
    If Rs_view.Fields("optcar") = "YES" Then
       OptCarDetails.Value = True
       Call OptCarDetails_Click
    Else
        OptNocarDetails.Value = True
    End If
    'FOR IPC THIS CONDITION WILL NOT BE CHECKED
    If GlbroleId = 13 Or GlbroleId = 78 Or GlbroleId = 76 Or GlbroleId = 212 Then
    Else
        If SqlRet("select count(*) from client_test where client_code='" & MainAccountCode & "' and approved='YES' ") >= 1 Then
            Label74.Caption = "APPROVED"
            If Glbloginid = 38387 Or Glbloginid = 101877 Or Glbloginid = 101877 Or Glbloginid = 97120 Or Glbloginid = 43452 Or Glbloginid = 38423 Or Glbloginid = 46183 Or Glbloginid = 97117 Or GlbroleId = 212 Or GlbroleId = 242 Then
                 cmdUpdate.Enabled = True
                 cmdapprove.Enabled = True
                 CmdInvestor.Visible = True
            Else
                 cmdUpdate.Enabled = False
                 cmdapprove.Enabled = False
                 CmdInvestor.Visible = False
            End If
        Else
            cmdUpdate.Enabled = True
            Label74.Caption = "NOT APPROVED"
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
    Call txtbusicode_LostFocus
    txttradingid = IIf(IsNull(Rs_view.Fields("tradingid")), "", Rs_view.Fields("tradingid"))
    txtdp_bid = IIf(IsNull(Rs_view.Fields("benefi_id")), "", Rs_view.Fields("benefi_id"))
    cmbstatus.Text = IIf(IsNull(Rs_view.Fields("status")), "", Rs_view.Fields("status"))
    txtAcOther.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
    
  

    TxtAcOther1.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
    cmbtitle.Text = IIf(IsNull(Rs_view.Fields("TITLE")), "", Rs_view.Fields("TITLE"))
    txtclientname.Text = IIf(IsNull(Rs_view.Fields("client_name")), "", Rs_view.Fields("client_name"))
    cmbclientn.Text = IIf(IsNull(Rs_view.Fields("nationality")), "", Rs_view.Fields("nationality"))
    txtclientpan.Text = IIf(IsNull(Rs_view.Fields("client_pan")), "", Rs_view.Fields("client_pan"))
    
    cmbtitle_Spouse.Text = IIf(IsNull(Rs_view.Fields("TITLE_FATHER_SPOUSE")), "", Rs_view.Fields("TITLE_FATHER_SPOUSE"))
    TxtFatherSpouseName.Text = IIf(IsNull(Rs_view.Fields("FATHER_SPOUSE_NAME")), "", Rs_view.Fields("FATHER_SPOUSE_NAME"))
    
    cmbincome.Text = IIf(IsNull(Rs_view.Fields("annual_income")), "", Rs_view.Fields("annual_income"))
    cmbinvestment.Text = IIf(IsNull(Rs_view.Fields("investement")), "", Rs_view.Fields("investement"))
    txttitle.Text = IIf(IsNull(Rs_view.Fields("title_other")), "", Rs_view.Fields("title_other"))
    cmbresinri.Text = IIf(IsNull(Rs_view.Fields("resident_nri")), "", Rs_view.Fields("resident_nri"))
    On Error Resume Next
    dob.Text = Format(Rs_view.Fields("dob"), "dd/mm/yyyy")
    cmbgender.Text = IIf(IsNull(Rs_view.Fields("GENDER")), "", Rs_view.Fields("GENDER"))
    
    TxtReferenceName(0).Text = IIf(IsNull(Rs_view.Fields("REF_NAME1")), "", Rs_view.Fields("REF_NAME1"))
    TxtReferenceMob(0).Text = IIf(IsNull(Rs_view.Fields("REF_MOB1")), "", Rs_view.Fields("REF_MOB1"))
    
    TxtReferenceName(1).Text = IIf(IsNull(Rs_view.Fields("REF_NAME2")), "", Rs_view.Fields("REF_NAME2"))
    TxtReferenceMob(1).Text = IIf(IsNull(Rs_view.Fields("REF_MOB2")), "", Rs_view.Fields("REF_MOB2"))
    
    TxtReferenceName(2).Text = IIf(IsNull(Rs_view.Fields("REF_NAME3")), "", Rs_view.Fields("REF_NAME3"))
    TxtReferenceMob(2).Text = IIf(IsNull(Rs_view.Fields("REF_MOB3")), "", Rs_view.Fields("REF_MOB3"))
    
    TxtReferenceName(3).Text = IIf(IsNull(Rs_view.Fields("REF_NAME4")), "", Rs_view.Fields("REF_NAME4"))
    TxtReferenceMob(3).Text = IIf(IsNull(Rs_view.Fields("REF_MOB4")), "", Rs_view.Fields("REF_MOB4"))
    
    TxtReferenceName(4).Text = IIf(IsNull(Rs_view.Fields("REF_NAME5")), "", Rs_view.Fields("REF_NAME5"))
    TxtReferenceMob(4).Text = IIf(IsNull(Rs_view.Fields("REF_MOB5")), "", Rs_view.Fields("REF_MOB5"))
    
    CmbMeritalStatus.Text = IIf(IsNull(Rs_view.Fields("MARITAL_STATUS")), "", Rs_view.Fields("MARITAL_STATUS"))
    CmbLeadSource.Text = IIf(IsNull(Rs_view.Fields("LEAD_SOURCE")), "", Rs_view.Fields("LEAD_SOURCE"))
    
    'SHOW GUEST CODE
    TguestCode.Text = SqlRet("SELECT GUEST_CD FROM CLIENT_MASTER WHERE CLIENT_CODE='" & MySourceId & "'")
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
    
    txtaddress1_P.Text = IIf(IsNull(Rs_view.Fields("PER_add1")), "", Rs_view.Fields("PER_add1"))
    txtaddress2_P.Text = IIf(IsNull(Rs_view.Fields("PER_add2")), "", Rs_view.Fields("PER_add2"))
    
    On Error Resume Next
    
    For I = 0 To cmbactcat.ListCount - 1
        ActCat = Split(cmbactcat.List(I), "#")
        If UCase(Trim(ActCat(1))) = UCase(Trim(Rs_view!act_cat)) Then
            cmbactcat.ListIndex = I
            Exit For
        End If
    Next
    
    
    On Error Resume Next
     For I = 0 To cmboccupation.ListCount - 1
        MyOcc = Split(cmboccupation.List(I), "#")
        If UCase(Trim(MyOcc(1))) = UCase(Trim(Rs_view!Occ_ID)) Then
            cmboccupation.ListIndex = I
            Exit For
        End If
    Next
    
    For I = 0 To CmbCountry.ListCount - 1
        Country = Split(CmbCountry.List(I), "#")
        If UCase(Trim(Country(1))) = MyCountryCode Then
            CmbCountry.ListIndex = I
            Exit For
        End If
    Next
    
    For I = 0 To CmbCountry_P.ListCount - 1
        PerCountry = Split(CmbCountry_P.List(I), "#")
        If UCase(Trim(PerCountry(1))) = MyPerCountryCode Then
            CmbCountry_P.ListIndex = I
            Exit For
        End If
    Next
    
    For I = 0 To cmbstate.ListCount - 1
        MyState = Split(cmbstate.List(I), "#")
        If UCase(Trim(MyState(1))) = UCase(Trim(Rs_view!state_id)) Then
            cmbstate.ListIndex = I
            Exit For
        End If
    Next
    
    For I = 0 To cmbstate_P.ListCount - 1
        MyPerState = Split(cmbstate_P.List(I), "#")
        If UCase(Trim(MyPerState(1))) = UCase(Trim(Rs_view!per_state_id)) Then
            cmbstate_P.ListIndex = I
            Exit For
        End If
    Next
    
    Call cmbstate_Click
    For I = 0 To cmbCity.ListCount - 1
        City = Split(cmbCity.List(I), "#")
        If UCase(Trim(City(1))) = UCase(Trim(Rs_view!City_id)) Then
            cmbCity.ListIndex = I
            Exit For
        End If
    Next
    
    Call cmbstate_P_Click
    For I = 0 To cmbcity_P.ListCount - 1
        PerCity = Split(cmbcity_P.List(I), "#")
        If UCase(Trim(PerCity(1))) = UCase(Trim(Rs_view!per_City_id)) Then
            cmbcity_P.ListIndex = I
            Exit For
        End If
    Next
    
    MyInvCode = SqlRet("select investor_code from client_master where client_code=" & MySourceId & "")
    ClientCat = SqlRet("select category_id from client_master where client_code=" & MySourceId & "")
    'investor  category
    For I = 0 To cmbinvestor.ListCount - 1
        INVESTORCODE = Split(cmbinvestor.List(I), "#")
        If UCase(Trim(INVESTORCODE(1))) = UCase(Trim(MyInvCode)) Then
            cmbinvestor.ListIndex = I
            Exit For
        End If
    Next
    
    If Left(UCase(cmbinvestor.Text), 10) <> "INDIVIDUAL" Then
        Label5(0).Caption = "Date Of Incorporation"
        Label82.Visible = True
        Label83.Visible = True
        cmbnatureofbusiness.Visible = True
        cmbnatureoftrust.Visible = True
        cmbnatureofbusiness = IIf(IsNull(Rs_view("nature_business")), "", Rs_view("nature_business"))
        cmbnatureoftrust = IIf(IsNull(Rs_view("NATURE_TRUST")), "", Rs_view("NATURE_TRUST"))
    Else
        Label5(0).Caption = "Date Of Birth"
        Label82.Visible = False
        Label83.Visible = False
        cmbnatureofbusiness.Visible = False
        cmbnatureoftrust.Visible = False
    End If
    
    'Client  category
    For I = 0 To cmbacatg.ListCount - 1
        ClienCode = Split(cmbacatg.List(I), "#")
        If UCase(Trim(ClienCode(1))) = UCase(Trim(ClientCat)) Then
            cmbacatg.ListIndex = I
            Exit For
        End If
    Next

    txtPin.Text = IIf(IsNull(Rs_view.Fields("pincode")), "", Rs_view.Fields("pincode"))
    txtpin_P.Text = IIf(IsNull(Rs_view.Fields("PER_PINCODE")), "", Rs_view.Fields("PER_PINCODE"))
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
    '----------------------------BY PANKAJ PUNDIR ON DATED 01032016---------------------------------------
    txtaddharCardNo.Text = IIf(IsNull(Rs_view.Fields("AADHAR_CARD_NO")), "", Rs_view.Fields("AADHAR_CARD_NO"))
    
    TxtFax.Text = IIf(IsNull(Rs_view.Fields("fax")), "", Rs_view.Fields("fax"))
    txtEmail = Rs_view.Fields("Email")
    TxtEmail_o = IIf(IsNull(Rs_view.Fields("OFFICE_EMAIL")), "", Rs_view.Fields("OFFICE_EMAIL"))
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
    Set Rs_view = Nothing
    I = 1
    If UCase(Trim(cmbstatus.Text)) = UCase(Trim("Others")) Then
    txtAcOther.Enabled = True
Else
    txtAcOther.Enabled = False
End If
RsGridFill.open "select a.client_name,a.client_code,a.client_codekyc,decode(approved_flag,'YES','YES','NO')approved_flag,a.client_pan,a.dob,a.inv_kyc,a.IS_NOMINEE,a.nominee_per,a.g_name,a.g_pan,b.relation_name,b.relation_id,c.occ_name,c.occ_id,title,mobile_no,email,AADHAR_CARD_NO,GENDER from client_test a,relationship_master b,occupation_master c where main_code=(SELECT MAIN_CODE FROM CLIENT_TEST WHERE CLIENT_CODE='" & txtclientcode.Text & "') and client_code<>MAIN_CODE   and a.relation_id=b.relation_id(+) and a.occ_id=c.occ_id(+) ", MyConn, adOpenForwardOnly
I = 1
If Not RsGridFill.EOF Then
    Do While Not RsGridFill.EOF
        VSFCommGrdE.TextMatrix(I, 0) = RsGridFill.Fields("client_name") & Space(50) & "#" & RsGridFill.Fields("client_codekyc")
        VSFCommGrdE.TextMatrix(I, 1) = RsGridFill.Fields("client_codekyc")
        VSFCommGrdE.TextMatrix(I, 2) = RsGridFill.Fields("client_name")
        VSFCommGrdE.TextMatrix(I, 3) = IIf(IsNull(RsGridFill.Fields("mobile_no")), "", RsGridFill.Fields("mobile_no"))
        VSFCommGrdE.TextMatrix(I, 4) = IIf(IsNull(RsGridFill.Fields("email")), "", RsGridFill.Fields("email"))
        VSFCommGrdE.TextMatrix(I, 5) = IIf(IsNull(RsGridFill.Fields("client_pan")), "", RsGridFill.Fields("client_pan"))
        VSFCommGrdE.TextMatrix(I, 6) = RsGridFill.Fields("dob")
        VSFCommGrdE.TextMatrix(I, 7) = RsGridFill.Fields("relation_name") & Space(50) & "#" & RsGridFill.Fields("relation_id")
        VSFCommGrdE.TextMatrix(I, 9) = RsGridFill.Fields("occ_name") & Space(50) & "#" & RsGridFill.Fields("occ_id")
        VSFCommGrdE.TextMatrix(I, 11) = IIf(IsNull(RsGridFill.Fields("inv_kyc")), "", RsGridFill.Fields("inv_kyc"))
        VSFCommGrdE.TextMatrix(I, 12) = IIf(IsNull(RsGridFill.Fields("g_name")), "", RsGridFill.Fields("g_name"))
        VSFCommGrdE.TextMatrix(I, 13) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
        VSFCommGrdE.TextMatrix(I, 13) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
        VSFCommGrdE.TextMatrix(I, 15) = IIf(IsNull(RsGridFill.Fields("approved_flag")), "", RsGridFill.Fields("approved_flag"))
        VSFCommGrdE.TextMatrix(I, 16) = IIf(IsNull(RsGridFill.Fields("Client_Code")), "", RsGridFill.Fields("Client_Code"))
        VSFCommGrdE.TextMatrix(I, 17) = IIf(IsNull(RsGridFill.Fields("title")), "", RsGridFill.Fields("title"))
        VSFCommGrdE.TextMatrix(I, 18) = IIf(IsNull(RsGridFill.Fields("AADHAR_CARD_NO")), "", RsGridFill.Fields("AADHAR_CARD_NO"))
        VSFCommGrdE.TextMatrix(I, 19) = IIf(IsNull(RsGridFill.Fields("gender")), "", RsGridFill.Fields("gender"))
        VSFCommGrdE.TextMatrix(I, 20) = IIf(IsNull(RsGridFill.Fields("IS_NOMINEE")), "", RsGridFill.Fields("IS_NOMINEE"))
        VSFCommGrdE.TextMatrix(I, 21) = IIf(IsNull(RsGridFill.Fields("NOMINEE_PER")), "", RsGridFill.Fields("NOMINEE_PER"))
        RsGridFill.MoveNext
        I = I + 1
    Loop
Else
    VSFCommGrdE.TextMatrix(I, 0) = ""
    VSFCommGrdE.TextMatrix(I, 1) = ""
    VSFCommGrdE.TextMatrix(I, 2) = ""
    VSFCommGrdE.TextMatrix(I, 3) = ""
    VSFCommGrdE.TextMatrix(I, 4) = ""
    VSFCommGrdE.TextMatrix(I, 5) = ""
    VSFCommGrdE.TextMatrix(I, 6) = ""
    VSFCommGrdE.TextMatrix(I, 7) = ""
    VSFCommGrdE.TextMatrix(I, 9) = ""
    VSFCommGrdE.TextMatrix(I, 11) = ""
    VSFCommGrdE.TextMatrix(I, 12) = ""
    VSFCommGrdE.TextMatrix(I, 13) = ""
End If
RsGridFill.Close
'--------------------------------------Fill Car Details----------------------------------------------------------------------
sql = ""
sql = " select a.model_id,c.model_name,MANIF_YEAR,renewal_date,REMARKS,a.company_Cd,company_name, d.manuf_name,d.manuf_id from Accounts_Car_Details a ,bajaj_company_master b,bajaj_model_master c,bajaj_mnf_master d "
sql = sql & "where a.model_id=c.model_id and a.company_cd=b.company_Cd(+) and c.manuf_id=d.manuf_id  and main_code='" & txtclientcode.Text & "'"
RsGridFill.open sql, MyConn, adOpenForwardOnly
I = 1
If Not RsGridFill.EOF Then
    Do While Not RsGridFill.EOF
        VSFCommGrdK.TextMatrix(I, 0) = RsGridFill.Fields("manuf_name") & Space(50) & "#" & RsGridFill!manuf_id
        VSFCommGrdK.TextMatrix(I, 1) = RsGridFill.Fields("model_name") & Space(50) & "#" & RsGridFill!Model_id
        VSFCommGrdK.TextMatrix(I, 2) = IIf(IsNull(RsGridFill.Fields("MANIF_YEAR")), "", RsGridFill.Fields("MANIF_YEAR"))
        VSFCommGrdK.TextMatrix(I, 3) = RsGridFill.Fields("RENEWAL_DATE")
        VSFCommGrdK.TextMatrix(I, 4) = RsGridFill.Fields("company_name") & Space(50) & "#" & RsGridFill.Fields("Company_Cd")
        VSFCommGrdK.TextMatrix(I, 5) = RsGridFill.Fields("Company_cd")
        VSFCommGrdK.TextMatrix(I, 6) = IIf(IsNull(RsGridFill.Fields("REMARKS")), "", RsGridFill.Fields("REMARKS"))
        VSFCommGrdK.TextMatrix(I, 7) = "OLD"
        RsGridFill.MoveNext
        I = I + 1
    Loop
Else
    VSFCommGrdK.TextMatrix(I, 0) = ""
    VSFCommGrdK.TextMatrix(I, 1) = ""
    VSFCommGrdK.TextMatrix(I, 2) = ""
    VSFCommGrdK.TextMatrix(I, 3) = ""
    VSFCommGrdK.TextMatrix(I, 4) = ""
    VSFCommGrdK.TextMatrix(I, 5) = ""
    VSFCommGrdK.TextMatrix(I, 6) = ""
    VSFCommGrdK.TextMatrix(I, 7) = ""
End If
RsGridFill.Close
'------------------------------------------------------------------------------------------------------------
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
'---------------------------------------------------------------------------------------
If UCase(Trim(cmbstatus.Text)) = UCase(Trim("individual")) Then
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
'-----------------------------fill advisory Details--------------------

sql = "select * from filladvisory where 1=1"
If txtclientcode.Text <> "" Then
    sql = sql & " and act_code='" & Trim(txtclientcode.Text) & "' "
End If
If txtpans.Text <> "" Then
    sql = sql & " and client_pan='" & Trim(txtpans.Text) & "' "
End If
RsAdvisory.open sql, MyConn, adforwardonly
If Not RsAdvisory.EOF Then
    '-------Fresh/Renewal----------
    OptRen.Value = True
    optFresh.Value = False
    For I = 0 To cmbplantype.ListCount - 1
        MyPlan = Split(cmbplantype.List(I), "=")
        If UCase(Trim(MyPlan(1))) = UCase(Trim(RsAdvisory!sch_code)) Then
            cmbplantype.ListIndex = I
            Exit For
        End If
    Next
    TxtAmount.Text = RsAdvisory.Fields("amount")
    For I = 0 To cmbBankName.ListCount - 1
        If UCase(Trim(cmbBankName.List(I))) = UCase(Trim(RsAdvisory!bank_name)) Then
            cmbBankName.ListIndex = I
            Exit For
        End If
    Next
    If RsAdvisory.Fields("payment_mode").Value = "C" Then
        optcheque.Value = True
    Else
        optdraft.Value = True
    End If
    TxtRemark.Text = RsAdvisory.Fields("bank_ac_no")
    txtChqNo.Text = RsAdvisory.Fields("cheque_no")
    dtChqDate.Text = RsAdvisory.Fields("cheque_date")
    MyTranCode = RsAdvisory.Fields("tran_code")
    MyPrintSourceId = RsAdvisory.Fields("source_code")
    MyTrDate = RsAdvisory.Fields("tr_date")
Else
    OptRen.Value = False
    optFresh.Value = True
    TxtAmount.Text = ""
    TxtRemark.Text = ""
    txtChqNo.Text = ""
    dtChqDate.Text = "__/__/____"
    MyTranCode = ""
    MyPrintSourceId = ""
    MyTrDate = "__/__/____"
End If
RsAdvisory.Close
'-----------------------------------------------------------------------
Else
    If txtpans.Text <> "" Then
        RsPan.open "select nvl(count(*),0) from client_test where client_pan=user_log('" & Trim(txtpans.Text) & "')", MyConn, adOpenForwardOnly
        If RsPan(0) = 0 Then
            MsgBox "The Requested Account Doesn't Exist", vbInformation
        Else
            MsgBox "The Requested Account is already mapped to some other RM", vbInformation
        End If
        RsPan.Close
    Else
        MsgBox "The Requested Account Doesn't Exist", vbInformation
    End If
    Set RsPan = Nothing
    
    Call CMDRESET_Click
End If
'CmdUpdate.Enabled = True
frameAdvisory.Enabled = True
If MyOldBusiCode <> "95829" Then
    If GlbroleId <> 218 And GlbroleId <> 124 And GlbroleId <> 1 And Glbloginid <> 97117 And GlbroleId <> 212 And GlbroleId <> 54 Then
        txtbusicode.Enabled = False
    End If
Else
    txtbusicode.Enabled = True
End If
Picture1.Enabled = False
End Sub
