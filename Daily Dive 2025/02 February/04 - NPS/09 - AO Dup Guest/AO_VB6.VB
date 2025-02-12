Dim Temp_Id() As String
Dim RSV As New Adodb.Recordset
Dim ApproveFlag As Integer
Dim RSV1 As New Adodb.Recordset
Dim RsOcc As New Adodb.Recordset
Dim MyOldBusiCode As String
Dim RsKyc As New Adodb.Recordset
Dim MyPrintSourceId As String
Dim MainAccountCode As String
Dim MyTrDate As Date
Dim RsAdvisory As New Adodb.Recordset
Dim MyPlan() As String
Dim RsSearch As New Adodb.Recordset
Dim flag As Boolean
Dim Temp_Id1() As String
Dim DupInvestor As String
Dim RsGridFill As New Adodb.Recordset
Dim CtC As String
Dim cmbYesNo As String
Dim cmbMaleFemale As String
Dim invCode() As String
Dim StrOcc As String
Dim rs_inv As New Adodb.Recordset
Dim StrInv As String
Dim CCKyc As String
Dim RsCommM As New Adodb.Recordset
Dim RsCommI As New Adodb.Recordset
Dim DoGrid As Integer
Dim lid As Long
Dim X As Boolean
Dim MyUpdateClient As String
Dim IsFamilyHead As Integer
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
Dim Rs_Rel As New Adodb.Recordset
Dim rs_Occ As New Adodb.Recordset
Dim clcode As New Adodb.Recordset
Dim MyBranch As String
Dim MyRmCode As String
Dim MyUpdateInvestor As String
Dim MyCityCode As String
Dim MyPerCityCode As String
Dim MyPerCityName As String
Dim City() As String
Dim PerCity() As String
Dim ClienCode() As String
Dim INVESTORCODE() As String
Dim Plan() As String
Dim MySchemeCode As String
Dim MyMutCode As String
Dim MySchemeName As String
Dim MyStateCode As String
Dim MyPerStateCode As String
Dim MyState() As String
Dim MyPerState() As String
Dim Country() As String
Dim PerCountry() As String
Dim ActCat() As String
Dim ActCat1 As Integer
Dim MyOccCode As String
Dim MyOcc() As String
Dim MyClientCode As String
Dim MyMainClientCode As String
Dim MySourceId As String
Dim MyInvestorSt As String
Dim MyTranCode As String
Dim AlreadyKyc As String
Dim rsDup As New Adodb.Recordset
Dim Rs_view As Adodb.Recordset
Dim StrCarStatus As String
Dim MyModelid As String
Dim MyMake() As String
Dim UPD_BAH As New Adodb.Command
Dim AdvFrRen As String
Dim lostpitch As Integer
Dim lostguest As Integer


Private Sub ChkPermanent_Click()
If ChkPermanent.Value = 1 Then
    txtaddress1_P.Text = txtaddress1.Text
    txtaddress2_P.Text = txtaddress2.Text
    txtpin_P.Text = txtPin.Text
    
    If CmbCountry.Text <> "" Then
        For i = 0 To CmbCountry_P.ListCount - 1
            PerCountry = Split(CmbCountry_P.List(i), "#")
            Country = Split(CmbCountry.Text, "#")
            If UCase(Trim(PerCountry(0))) = UCase(Trim(Country(0))) Then
                CmbCountry_P.ListIndex = i
                Exit For
            End If
        Next
        
        Call CmbCountry_P_Click
    End If
    
    If cmbstate.Text <> "" Then
        For i = 0 To cmbstate_P.ListCount - 1
            MyPerState = Split(cmbstate_P.List(i), "#")
            MyState = Split(cmbstate.Text, "#")
            If UCase(Trim(MyPerState(0))) = UCase(Trim(MyState(0))) Then
                cmbstate_P.ListIndex = i
                Exit For
            End If
        Next
        
        Call cmbstate_P_Click
    End If
    
    If cmbcity.Text <> "" Then
        For i = 0 To cmbcity_P.ListCount - 1
            PerCity = Split(cmbcity_P.List(i), "#")
            City = Split(cmbcity.Text, "#")
            If UCase(Trim(PerCity(1))) = UCase(Trim(City(1))) Then
                cmbcity_P.ListIndex = i
                Exit For
            End If
        Next
        
        Call cmbcity_P_Click
    End If
Else
    txtaddress1_P.Text = ""
    txtaddress2_P.Text = ""
    CmbCountry_P.ListIndex = -1
    cmbstate_P.ListIndex = -1
    cmbcity_P.ListIndex = -1
    txtpin_P.Text = ""
End If
End Sub

Private Sub cmbactcat_Click()
If cmbactcat.ListIndex <> -1 Then
    ActCat = Split(cmbactcat.Text, "#")
End If
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

Private Sub cmbcity_Change()
cmbcity_Click
End Sub

Private Sub cmbcity_Click()
If cmbcity.Text <> "" Then
    City = Split(cmbcity.Text, "#")
    MyCityCode = City(1)
    MyCityName = City(0)
End If
End Sub
Private Sub cmbCity_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbcity.MatchFound = False Then
      cmbcity.ListIndex = -1
End If
End Sub

Private Sub cmbcity_P_Change()
cmbcity_P_Click
End Sub

Private Sub cmbcity_P_Click()
If cmbcity_P.Text <> "" Then
    PerCity = Split(cmbcity_P.Text, "#")
    MyPerCityCode = City(1)
    MyPerCityName = City(0)
End If
End Sub

Private Sub cmbclientn_Change()
    If cmbclientn.Text = "NRI" Then
        txtMobile.MaxLength = 20
        txtPin.MaxLength = 20
    Else
        txtMobile.MaxLength = 10
        txtPin.MaxLength = 6
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



Private Sub CmbCountry_Change()
Call CmbCountry_Click
End Sub


Private Sub CmbCountry_Click()
Dim Str() As String
Dim MyCountry As String
If CmbCountry.Text <> "" Then
   Str = Split(CmbCountry.Text, "#")
   MyCountry = Str(1)
   If sql_ora = "SQL" Then
        st = "select distinct STATE_ID,STATE_NAME from state_master where  state_id is not null"
        If MyCountry <> "" Then
            st = st & " and country_id=" & MyCountry & " "
        End If
        st = st & " order by state_name"
    Else
        st = "select distinct state_id,state_name from state_master where  state_id is not null "
        If MyCountry <> "" Then
            st = st & " and country_id=" & MyCountry & " "
        End If
        st = st & " order by state_name"
    End If
    Set RSV = MyConn.Execute(st)
    cmbstate.Clear
    If Not RSV.EOF Then
        Do While Not RSV.EOF
            cmbstate.AddItem RSV(1) & Space(100) & "#" & RSV(0)
            RSV.MoveNext
        Loop
    End If
    RSV.Close
End If

End Sub


Private Sub CmbCountry_P_Change()
CmbCountry_P_Click
End Sub

Private Sub CmbCountry_P_Click()
Dim Str() As String
Dim MyPerCountry As String
If CmbCountry_P.Text <> "" Then
   Str = Split(CmbCountry_P.Text, "#")
   MyPerCountry = Str(1)
   If sql_ora = "SQL" Then
        st = "select distinct STATE_ID,STATE_NAME from state_master where  state_id is not null"
        If MyCountry <> "" Then
            st = st & " and country_id=" & MyPerCountry & " "
        End If
        st = st & " order by state_name"
    Else
        st = "select distinct state_id,state_name from state_master where  state_id is not null "
        If MyCountry <> "" Then
            st = st & " and country_id=" & MyPerCountry & " "
        End If
        st = st & " order by state_name"
    End If
    Set RSV = MyConn.Execute(st)
    cmbstate_P.Clear
    If Not RSV.EOF Then
        Do While Not RSV.EOF
            cmbstate_P.AddItem RSV(1) & Space(100) & "#" & RSV(0)
            RSV.MoveNext
        Loop
    End If
    RSV.Close
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

Private Sub cmbInvestor_Click()
If Left(UCase(cmbinvestor.Text), 10) <> "INDIVIDUAL" Then
    Label5(0).Caption = "Date Of Incorporation"
    'Label82.Visible = True
    'Label83.Visible = True
    'cmbnatureofbusiness.Visible = True
    'cmbnatureoftrust.Visible = True
Else
    Label5(0).Caption = "Date Of Birth"
    Label82.Visible = False
    Label83.Visible = False
    cmbnatureofbusiness.Visible = False
    cmbnatureoftrust.Visible = False
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
    If sql_ora = "SQL" Then
        st1 = "select distinct city_ID,CITY_NAME from city_master where (city_id is not null or city_id<>'')"
        If MyStateCode <> "" Then
            st1 = st1 & " and state_id=" & MyStateCode & " "
        End If
        st1 = st1 & " order by city_name"
    Else
        st1 = "select distinct city_id,city_name from city_master where city_id is not null"
        st1 = st1 & " and state_id=" & MyStateCode & " "
        st1 = st1 & " order by city_name"
    End If
    Set RSV1 = MyConn.Execute(st1)
    cmbcity.Clear
    If Not RSV1.EOF Then
        Do While Not RSV1.EOF
            cmbcity.AddItem RSV1(1) & Space(100) & "#" & RSV1(0)
            RSV1.MoveNext
        Loop
    End If
    RSV1.Close

End If
End Sub

Private Sub cmbstate_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbstate.MatchFound = False Then
      cmbstate.ListIndex = -1
End If
End Sub


Private Sub cmbstate_P_Change()
cmbstate_P_Click
End Sub

Private Sub cmbstate_P_Click()
If cmbstate_P.Text <> "" Then
    MyPerState = Split(cmbstate_P.Text, "#")
    MyPerStateCode = MyPerState(1)
    If sql_ora = "SQL" Then
        st1 = "select distinct city_ID,CITY_NAME from city_master where (city_id is not null or city_id<>'')"
        If MyPerStateCode <> "" Then
            st1 = st1 & " and state_id=" & MyPerStateCode & " "
        End If
        st1 = st1 & " order by city_name"
    Else
        st1 = "select distinct city_id,city_name from city_master where city_id is not null"
        st1 = st1 & " and state_id=" & MyPerStateCode & " "
        st1 = st1 & " order by city_name"
    End If
    Set RSV1 = MyConn.Execute(st1)
    cmbcity_P.Clear
    If Not RSV1.EOF Then
        Do While Not RSV1.EOF
            cmbcity_P.AddItem RSV1(1) & Space(100) & "#" & RSV1(0)
            RSV1.MoveNext
        Loop
    End If
    RSV1.Close
End If

End Sub
Private Sub cmbstate_P_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbstate_P.MatchFound = False Then
      cmbstate_P.ListIndex = -1
End If
End Sub

Private Sub cmbStatus_Change()
Call cmbStatus_Click
End Sub

Private Sub cmbStatus_Click()
If UCase(Trim(cmbstatus.Text)) = UCase(Trim("Others")) Then
    txtAcOther.Enabled = True
Else
    txtAcOther.Enabled = False
End If
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

If cmbstatus.Text = "NRI" Then
    cmbclientn.Text = "NRI"
Else
    cmbclientn.ListIndex = -1
End If
End Sub

Private Sub cmbstatus_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
If cmbstatus.MatchFound = False Then
      cmbstatus.ListIndex = -1
End If
End Sub

Private Sub cmbstatus1_Click()
If cmbstatus1.ListIndex = 1 Then
   lblreason.Visible = True
   TxtReason.Visible = True
Else
   lblreason.Visible = False
   TxtReason.Visible = False
End If
TxtReason.Text = ""
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

Private Sub cmbtitle_Spouse_Click()
If cmbtitle_Spouse.Text = "M/S." Then
    CmbMeritalStatus.ListIndex = 2
End If
End Sub

Private Sub cmdApprove_Click()
Dim cmd As New Adodb.Command
Dim UPD_CL_INV As New Adodb.Command
Dim ClCat As String
Dim invCat As String
Dim Str() As String
ApproveFlag = 1
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
Call cmdUpdate_Click
Set UPD_CL_INV.ActiveConnection = MyConn
UPD_CL_INV.CommandType = adCmdStoredProc
UPD_CL_INV.CommandText = "PRC_UPDATE_CLIENT_INVESTOR"
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("Main_Cd", adVarChar, adParamInput, 50, txtclientcode)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("login_id", adVarChar, adParamInput, 50, Glbloginid)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("client_cat", adVarChar, adParamInput, 50, ClCat)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("inv_Cd", adVarChar, adParamInput, 50, invCat)
UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("guest_Cd", adVarChar, adParamInput, 50, TguestCode)
UPD_CL_INV.Execute

'MyConn.Execute "update investor_master set approved='YES' where inv_code in (select client_codekyc from client_test where main_code='" & txtclientcode & "')"
MyConn.Execute "update client_test set APPROVED_FLAG='YES' , approved='YES',approve_dt= '" & Format(ServerDateTime, "DD-MMM-YYYY") & "',APPROVE_USERID='" & Glbloginid & "' where client_code=main_code AND main_code='" & txtclientcode & "'"
MyConn.Execute "update CLIENT_master set approved_flag='YES' where client_code in (select substr(client_codekyc,1,8) from client_test where main_code='" & txtclientcode & "')"

'To Approve Grid Investor
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
    '-----------------------------------Checking Duplicate pan------------------------------
        If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
            If SqlRet("select count(*) from client_test where client_codekyc <>'" & Trim(VSFCommGrdE.TextMatrix(i, 1)) & "' and upper(client_pan) = upper('" & Trim(VSFCommGrdE.TextMatrix(i, 5)) & "')") > 0 Then
                MsgBox "This Pan No is already Exist"
                Exit Sub
            End If
        End If
     '-------------------------------------------------------------------
        If VSFCommGrdE.TextMatrix(i, 0) <> "" Then
            If UCase(VSFCommGrdE.TextMatrix(i, 15)) = "YES" Then
                MyConn.Execute "update investor_master set approved='YES' where inv_code='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
                MyConn.Execute "update client_test set APPROVED_FLAG='YES' ,approved='YES',approve_dt= '" & Format(ServerDateTime, "DD-MMM-YYYY") & "',APPROVE_USERID='" & Glbloginid & "' where  main_code='" & txtclientcode & "' and client_codekyc='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
            Else
                MyConn.Execute "update client_test set APPROVED_FLAG='NO' ,approved=NULL,approve_dt= NULL,APPROVE_USERID=NULL where  main_code='" & txtclientcode & "' and client_codekyc='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
                MyConn.Execute "update investor_master set approved=null where inv_code='" & VSFCommGrdE.TextMatrix(i, 1) & "'"
            End If
        End If
     Next
End If
MsgBox "Main Account And All The Sub Account Has Been Approved", vbInformation, "Wealthmaker"
Call cmdReset_Click
End Sub

Private Sub cmdExit1_Click()
    Unload Me
End Sub

Private Sub cmdinvestor_Click()
frmactopeninv.Show
frmactopeninv.ZOrder
End Sub

Private Sub cmdmodifyar_Click()
str_test = " update transaction_st "
If optcash.Value = False Then
    str_test = str_test & " set MUT_CODE='" & MyMutCode & "',SCH_CODE='" & MySchemeCode & "',AMOUNT=" & Val(TxtAmount.Text) & ","
    str_test = str_test & " cheque_no=" & Trim(txtChqNo.Text) & ",CHEQUE_DATE=to_date('" & dtChqDate & "','dd/MM/yyyy'),BANK_NAME='" & Trim(cmbBankName.Text) & "',remark='ADVISORY',BANK_AC_NO='" & TxtRemark & "',LOGGEDUSERID='" & Glbloginid & "'"
    str_test = str_test & " where tran_code='" & MyTranCode & "'"
Else
    str_test = str_test & " set MUT_CODE='" & MyMutCode & "',SCH_CODE='" & MySchemeCode & "',AMOUNT=" & Val(TxtAmount.Text) & ","
    str_test = str_test & " remark='ADVISORY',BANK_AC_NO='" & TxtRemark & "',LOGGEDUSERID='" & Glbloginid & "'"
    str_test = str_test & " where tran_code='" & MyTranCode & "'"
End If
MyConn.Execute str_test
MsgBox "Your Advisory Package Has Been Modified Successfuly"
Call cmdReset_Click
End Sub

Private Sub cmdOk_Click()
MyConn.Execute ("DELETE FROM ACCOUNTS_CAR_DETAILS WHERE MAIN_CODE='" & MainAccountCode & "' AND MODEL_ID='" & MyModelid & "'")
MsgBox "The Row Has Been Deleted Sucessfully ", vbInformation
End Sub

Private Sub CmdOkStatus_Click()
Dim sql As String
Dim MyId As String
Dim LastStatus As String
LastStatus = SqlRet("select  ACCOUNT_STATUS from ACCOUNTSTATUS where main_Code='" & txtclientcode & "'")
If LastStatus = "APPROVED" Then
   MsgBox "The Status Of This Account Approved,You Can Not Change The Status Of This Account", vbInformation
   Exit Sub
End If
If cmbstatus1.ListIndex = -1 Then
    MsgBox "Please Select The Status First", vbInformation, "Wealthmaker"
    Exit Sub
End If
If txtclientcode.Text = "" Then
    MsgBox "Enter The MainCode", vbInformation, "Wealthmaker"
    Exit Sub
End If
If SqlRet("select count(*) from ACCOUNTSTATUS where main_Code='" & txtclientcode & "' and recd_date='" & Format(recdate, "dd-mmm-yyyy") & "' and ACCOUNT_STATUS='" & cmbstatus1.Text & "'") = 0 Then
    sql = "INSERT INTO ACCOUNTSTATUS(ACCOUNT_STATUS,USER_ID,TIMEST,RECD_DATE,MAIN_CODE,REASON,APPROVED_DT)"
    sql = sql & " values('" & cmbstatus1.Text & "','" & Glbloginid & "',sysdate,'" & Format(recdate, "dd-mmm-yyyy") & "','" & txtclientcode.Text & "','" & TxtReason.Text & "' "
    If cmbstatus1.ListIndex <> -1 Then
       If cmbstatus1.ListIndex = 2 Then
           sql = sql & ",SYSDATE)"
       Else
           sql = sql & " ,NULL) "
       End If
    Else
       sql = sql & " ,NULL) "
    End If
    MyConn.Execute sql
Else
    MyId = SqlRet("select id from ACCOUNTSTATUS where main_Code='" & txtclientcode & "' and recd_date='" & Format(recdate, "dd-mmm-yyyy") & "' and ACCOUNT_STATUS='" & cmbstatus1.Text & "'")
    sql = "UPDATE ACCOUNTSTATUS "
    sql = sql & " SET ACCOUNT_STATUS='" & cmbstatus1.Text & "',USER_ID='" & Glbloginid & "',RECD_DATE='" & Format(recdate, "dd-mmm-yyyy") & "',MAIN_CODE='" & txtclientcode.Text & "',reason='" & TxtReason & "' where id=" & MyId & ""
    MyConn.Execute sql
End If
frmstatus.Visible = False
If cmbstatus1.Text = "APPROVED" Then
    MsgBox SqlRet("select approved_code from ACCOUNTSTATUS where main_Code='" & txtclientcode & "' and recd_date='" & Format(recdate, "dd-mmm-yyyy") & "' and ACCOUNT_STATUS='APPROVED'"), vbInformation, "Wealthmaker"
End If
If cmbstatus1.ListIndex = 0 Then
    ApproveFlag = 1
    Call cmdUpdate_Click
End If
End Sub

Private Sub cmdprint_Click()
If MyTranCode = "" Then
    MsgBox " Ar Can Not Be Print Right Now Please Generate The AR "
    Exit Sub
End If
If MyPrintSourceId <> "" Then
    MySourceId = MyPrintSourceId
End If
sql = " CREATE OR REPLACE VIEW ARMFADV AS "
sql = sql & "   SELECT b.client_code, 'P' ar_type, t.tran_code, tr_date, "
sql = sql & "           cheque_date cheque_date, cheque_no cheque_no, t.bank_name, amount, "
sql = sql & "          b.client_code source_code, app_no, "
sql = sql & "          NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, "
sql = sql & "          NVL ((SELECT SUM (NVL (amt, 0)) "
sql = sql & "                  FROM payment_detail "
sql = sql & "                 WHERE tran_code = t.tran_code), 0) paidamt, '' asr, payment_mode, "
sql = sql & "          (select investor_name from investor_master where inv_code=t.client_code)inv, "
sql = sql & "           (SELECT MAX (client_name) "
sql = sql & "                                FROM client_master "
sql = sql & "                               WHERE client_code = t.source_code) client, "
sql = sql & "          exist_code AS existcode, address1 add1, address2 add2, '' loc, "
sql = sql & "          pincode pin, "
sql = sql & "          (SELECT MAX (city_name) "
sql = sql & "             FROM city_master "
sql = sql & "            WHERE city_id = (SELECT MAX (city_id) "
sql = sql & "                               FROM client_master "
sql = sql & "                              WHERE client_code = t.source_code)) ccity, "
sql = sql & "          mobile ph, email, 0 arn, '' subbroker, "
sql = sql & "          (SELECT rm_name "
sql = sql & "             FROM employee_master "
sql = sql & "            WHERE payroll_id = TO_CHAR (t.business_rmcode) "
sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rname, "
sql = sql & "          (SELECT payroll_id "
sql = sql & "             FROM employee_master "
sql = sql & "            WHERE payroll_id = TO_CHAR (t.business_rmcode) "
sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rcode, "
sql = sql & "          (SELECT branch_name "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) bname, "
sql = sql & "          (SELECT address1 "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) badd1, "
sql = sql & "          (SELECT address2 "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) badd2, "
sql = sql & "          (SELECT phone "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) bphone, "
sql = sql & "          (SELECT location_name "
sql = sql & "             FROM location_master "
sql = sql & "            WHERE location_id = (SELECT location_id "
sql = sql & "                                   FROM branch_master "
sql = sql & "                                  WHERE branch_code = t.busi_branch_code)) bloc, "
sql = sql & "          (SELECT city_name "
sql = sql & "             FROM city_master "
sql = sql & "            WHERE city_id = (SELECT city_id "
sql = sql & "                               FROM branch_master "
sql = sql & "                              WHERE branch_code = t.busi_branch_code)) bcity, "
sql = sql & "   (SELECT iss_name "
sql = sql & "             FROM iss_master "
sql = sql & "            WHERE iss_code = t.mut_code "
sql = sql & "              AND iss_code NOT IN ( "
sql = sql & "                              SELECT DISTINCT iss_code "
sql = sql & "                                         FROM iss_master "
sql = sql & "                                        WHERE prod_code IN ( "
sql = sql & "                                                   SELECT prod_code "
sql = sql & "                                                     FROM product_master "
sql = sql & "                                                    WHERE nature_code = "
sql = sql & "                                                                       'NT004'))) "
sql = sql & "                                                                       compmf, "
sql = sql & "          'Bajaj Capital Limited' compgroup, "
sql = sql & "    (SELECT longname "
sql = sql & "             FROM other_product "
sql = sql & "            WHERE osch_code = t.sch_code) schmf, "
sql = sql & "             "
sql = sql & "          (SELECT short_name "
sql = sql & "             FROM scheme_info "
sql = sql & "            WHERE sch_code = t.sch_code) sschmf, ('38387') userid "
sql = sql & "     FROM transaction_ST t, client_master b "
If MyPrintSourceId <> "" Then
    sql = sql & "    WHERE    tr_date = '" & Format(MyTrDate, "DD-MMM-YYYY") & "' and t.source_code = b.client_code "
Else
    sql = sql & "    WHERE    tr_date = '" & Format(ServerDateTime, "DD-MMM-YYYY") & "' and t.source_code = b.client_code "
End If
sql = sql & "      AND (asa <> 'C' OR asa IS NULL) "
sql = sql & " AND business_rmcode = " & txtbusicode & " "
sql = sql & "  AND source_code = " & MySourceId & " "
sql = sql & "      AND tran_code = " & MyTranCode & " "
MyConn.Execute sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "TEST", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\MFAr3.rpt"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.action = 1
End Sub
Private Sub cmdReset1_Click()
    cmdReset_Click
End Sub

Private Sub cmdSave_Click()
On Error GoTo err

'--------------Advisory Fresh/Renewal---------------------
If optFresh.Value = True Then
    AdvFrRen = "PURCHASE"
ElseIf OptRen.Value = True Then
    AdvFrRen = "REINVESTMENT"
End If

'---------------------------------------------------------

If MySourceId <> "" Then
    If txtbusicode.Text = "" Then
        MsgBox "Business Code Can Not Left Blank"
        SSTab1.Tab = 0
        txtbusicode.SetFocus
        Exit Sub
    End If
    If cmbplantype.Text = "" Then
       MsgBox "Plan Can Not Be Left Blank"
       Exit Sub
    End If
    If optcash.Value = False Then
        If cmbBankName.Text = "" Then
           MsgBox "Bank Name Can Not Be Left Blank"
           Exit Sub
        End If
    End If
    If txtChqNo.Text = "" Then
        MsgBox "Please Enter Cheque No.", vbInformation
        txtChqNo.SetFocus
        Exit Sub
    End If
    If optcash.Value = False Then
       If txtChqNo.Text = "" Then
           MsgBox "Chq No Can Not Be Left Blank"
           Exit Sub
       End If
    End If
Else
    MsgBox "You Have No KYC Acount ", vbInformation
    Exit Sub
End If
If SqlRet("select nvl(count(*),0) from transaction_st where TRIM(cheque_no)='" & Trim(txtChqNo.Text) & "' and remark= 'ADVISORY'") > 0 Then
    MsgBox "This Transaction Already Exist", vbInformation, "Wealthmaker"
    Exit Sub
End If
If optFresh.Value = True Then
    If SqlRet("select nvl(count(*),0) from transaction_st where client_code='" & MyInvestorSt & "' and remark= 'ADVISORY' and tran_type='PURCHASE'") > 0 Then
        MsgBox "Please Select Renewal Option", vbInformation, "Wealthmaker"
        Exit Sub
    End If
End If
If MySourceId = "" And txtclientcodeold.Text <> 0 And Len(txtclientcodeold.Text) >= 11 Then
    MySourceId = Mid(txtclientcodeold.Text, 1, 8)
    MyInvestorSt = txtclientcodeold.Text
End If
'MyConn.Execute ("update client_master set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/rrrr') where client_code='" & MySourceId & "'")
'MyConn.Execute ("update investor_master set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/rrrr') where source_id='" & MySourceId & "'")
str_test = " insert into transaction_sttemp"
str_test = str_test & " (INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,APP_NO,TRAN_TYPE,AMOUNT,"
str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,BANK_NAME,remark)"
str_test = str_test & " Values"
str_test = str_test & " ('1','" & Format(ServerDateTime, "DD-MMM-YYYY") & "'," & MySourceId & ","
str_test = str_test & " '" & MyMutCode & "','" & MySchemeCode & "',null,'" & AdvFrRen & "'," & Val(TxtAmount.Text) & "," & MyBranch & ","
str_test = str_test & " " & MySourceId & "," & MyRmCode & "," & Trim(txtbusicode.Text) & "," & MyBranch & ","
str_test = str_test & "" & Trim(txtChqNo.Text) & ",to_date('" & dtChqDate & "','dd/MM/yyyy'),'" & Trim(cmbBankName.Text) & "'"
str_test = str_test & " ,'ADVISORY')"
MyConn.Execute str_test

MyTranCode = SqlRet("select max(tran_code) from temp_tran where branch_code=" & MyBranch & " and substr(tran_code,1,2)='07' ")
If MyInvestorSt = "" Then
    MyInvestorSt = SqlRet("select min(inv_code) from investor_master where substr(inv_code,1,8)=" & MySourceId & " and kyc in ('YES','YESP')")
End If
str_test = " insert into transaction_st"
str_test = str_test & " (TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,APP_NO,TRAN_TYPE,AMOUNT,"
str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,BANK_NAME,remark,BANK_AC_NO,LOGGEDUSERID)"
str_test = str_test & " Values"
str_test = str_test & " (trim('" & MyTranCode & "'),'1','" & Format(ServerDateTime, "DD-MMM-YYYY") & "'," & MyInvestorSt & ","
str_test = str_test & " '" & MyMutCode & "','" & MySchemeCode & "',null,'" & AdvFrRen & "'," & Val(TxtAmount.Text) & "," & MyBranch & ","
str_test = str_test & " " & MySourceId & "," & MyRmCode & "," & Trim(txtbusicode.Text) & "," & MyBranch & ","
str_test = str_test & "" & Trim(txtChqNo.Text) & ",to_date('" & dtChqDate & "','dd/MM/yyyy'),'" & Trim(cmbBankName.Text) & "'"
str_test = str_test & " ,'ADVISORY','" & TxtRemark & "','" & Glbloginid & "')"
MyConn.Execute str_test
MyConn.Execute ("DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE='" & MyTranCode & "'")

cmdSave.Enabled = False
'*******************FOR BROK RECD_PAID -----------------------------------
Dim recd_paid As New Adodb.Command
Set recd_paid.ActiveConnection = MyConn
recd_paid.CommandType = adCmdStoredProc
recd_paid.CommandText = "Recd_paid_update"
recd_paid.Parameters.Append recd_paid.CreateParameter("tr_code", adVarChar, adParamInput, 50, MyTranCode)
recd_paid.Execute
'''''*********************************end------------------------------------
MsgBox "Transaction  " & MyTranCode & "  Has Been Saved Sucessfully"
Call cmdReset_Click
Exit Sub
err:
MsgBox err.Description
Resume
End Sub



Private Sub cmdStatus_Click()
Dim MyRs As New Adodb.Recordset
If txtclientcode.Text = "" Then
   MsgBox "First Enter The Client_Code", vbInformation, "Wealthhmaker"
   Exit Sub
End If
If SqlRet("SELECT COUNT(*) FROM CLIENT_tEST WHERE MAIN_CODE='" & txtclientcode.Text & "'") >= 1 Then
    frmstatus.Visible = True
    MyRs.open "SELECT * FROM ACCOUNTSTATUS WHERE ID=(SELECT MAX(ID) FROM ACCOUNTSTATUS WHERE MAIN_CODE='" & txtclientcode & "') ", MyConn, adOpenStatic
    If Not MyRs.EOF Then
        cmbstatus1.Text = IIf(IsNull(MyRs.Fields("account_status")), "", MyRs.Fields("account_status"))
        Label85.Caption = IIf(IsNull(MyRs.Fields("approved_code")), "", MyRs.Fields("Approved_code"))
        If UCase(cmbstatus1.Text) = "REJECTED" Then
           TxtReason.Visible = True
           lblreason.Visible = True
           TxtReason.Text = IIf(IsNull(MyRs.Fields("reason")), "", MyRs.Fields("reason"))
           
        Else
           TxtReason.Visible = False
           lblreason.Visible = False
        End If
        recdate.Text = MyRs.Fields("recd_date")
        MyRs.MoveNext
    End If
    MyRs.Close
End If
End Sub


Private Sub cmdGo_Click()
If TguestCode <> "" Then
    lostguest = 1
    picture4_fun
    If TguestCode.Text = "" Then
        MsgBox "Guest code not Found", vbOKOnly, "Wealthmaker"
        TguestCode.Text = ""
        'TguestCode.SetFocus
        Exit Sub
    End If
    If txtPitchBook.Text = "" Then
        MsgBox "Pitch book number not Found", vbOKOnly, "Wealthmaker"
        TguestCode.Text = ""
        'TguestCode.SetFocus
        Exit Sub
    End If
Else
    lostguest = 0
End If
If TguestCode.Text <> "" Then
  txtbusicode.Text = SqlRet("select nvl(emp_no,0) from bajaj_venue_booking where guest_cd='" & TguestCode & "'")
  Call txtbusicode_LostFocus
  TguestCode.Enabled = False
End If
End Sub

Private Sub cmdTransfer_Click()
On Error GoTo err1
    
Dim flag As Boolean
Dim rsEmp As Adodb.Recordset
Dim b_cd() As String

    
    If Len(txtbusicode.Text) <> 0 And Trim(txtbusicode.Text) <> "95829" Then
        MyBranch = ""
        MyRmCode = ""
        Set rsEmp = New Adodb.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.category_id in('2001','2018')", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            MyBranch = rsEmp(0)
            MyRmCode = rsEmp(3)
            Label32.Caption = rsEmp(1)
            Label55.Caption = rsEmp(2)
        Else
            MsgBox "RM Should Be Sale Support and Marketing "
            Label32.Caption = ""
            Label55.Caption = ""
            txtbusicode = ""
            Exit Sub
        End If
        rsEmp.Close
        Set rsEmp = Nothing
        
        Set rsEmp = New Adodb.Recordset
        rsEmp.open "select client_codekyc,main_code,business_code from client_test where client_code='" & Trim(txtclientcode.Text) & "'", MyConn, adOpenForwardOnly
        If Not rsEmp.EOF Then
            
            If rsEmp("BUSINESS_CODE") <> "95829" Then
                'MsgBox "Account Already Transferred", vbCritical
                rsEmp.Close
                Set rsEmp = Nothing
                Exit Sub
            End If
            
            MyConn.BeginTrans
            flag = True
            ''Account transfer log
            MyConn.Execute "insert into Account_transfer_log(CLIENT_CODE,OLD_BUSINESS_RM,NEW_BUSINESS_RM,LOGGEDUSERID) values('" & rsEmp("main_code") & "','" & rsEmp("business_code") & "','" & Trim(txtbusicode.Text) & "','" & Glbloginid & "')"
            '''''''''''''
            
            ''''''''''''''''''''''''''''''''''''''''
            MyConn.Execute "Update Client_master set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), rm_code=" & MyRmCode & ",Sourceid=" & MyBranch & " where Client_code=" & Left(rsEmp("client_codekyc"), 8)
            MyConn.Execute "Update Client_master@mf.bajajcapital set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), rm_code=" & MyRmCode & ",Sourceid=" & MyBranch & " where Client_code=" & Left(rsEmp("client_codekyc"), 8)
            MyConn.Execute "Update Investor_master set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'),rm_code=" & MyRmCode & ",Branch_code=" & MyBranch & " where inv_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "Update Investor_master@mf.bajajcapital set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'),rm_code=" & MyRmCode & ",Branch_code=" & MyBranch & " where inv_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "Update CLIENT_TEST set modify_date=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), BUSINESS_CODE=(SELECT PAYROLL_ID FROM EMPLOYEE_MASTER WHERE RM_CODE=" & MyRmCode & "),BRANCH_CODE=" & MyBranch & " where main_code='" & rsEmp("main_code") & "'"
    
    
            MyConn.Execute "UPDATE TRANSACTION_ST SET modify_talisma=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'),RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE TRANSACTION_ST@mf.bajajcapital SET RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE port_TRANSACTION_ST@mf.bajajcapital SET RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE TRANSACTION_STTEMP SET modify_talisma=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            MyConn.Execute "UPDATE transaction_mf_temp1 SET modify_talisma=to_date('" & Format(ServerDateTime, "dd/mm/yyyy") & "','dd/mm/RRRR'), RMCODE=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & " WHERE client_code in(select client_codekyc from client_test where main_code='" & rsEmp("main_code") & "')"
            ''''''''''''''''''''''''''''''''''''''''
            MyConn.CommitTrans
            flag = False
        Else
            'MsgBox "Account Not Found", vbCritical
        End If
        
        rsEmp.Close
        Set rsEmp = Nothing
    End If
    Exit Sub
err1:
    MsgBox err.Description
    If flag = True Then MyConn.RollbackTrans
End Sub

Private Sub DOB_LostFocus()
If IsDate(dob.Text) = False Then
    MsgBox "Enter Valid Date", vbInformation
    dob.SetFocus
    Exit Sub
End If
guardian_enable
End Sub
Private Sub guardian_enable()
If dob.Text <> "__/__/____" And IsDate(dob.Text) = True Then
    'If DateDiff("YYYY", Format(dob.Text, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
    If DateDiff("M", Format(dob.Text, "DD/MM/YYYY"), ServerDateTime) <= 216 Then
        'Left(UCase(cmbinvestor.Text), 10) = "INDIVIDUAL"
        If Left(UCase(cmbinvestor.Text), 3) = "HUF" Or Left(UCase(cmbinvestor.Text), 5) = "TRUST" Or Left(UCase(cmbinvestor.Text), 7) = "COMPANY" Or Left(UCase(cmbinvestor.Text), 7) = "SOCIETY" Then
            frameguardian.Enabled = False
        Else
            frameguardian.Enabled = True
        End If
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
'Private Sub Form_KeyPress(KeyAscii As Integer)
'    If KeyAscii = 13 Then
'        If flag = False Then SendKeys "{tab}"
'    End If
'End Sub

Private Sub Form_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then SendKeys "{Tab}"
End Sub

Private Sub Form_Resize()
         ' Position the scroll bars:
         HScroll1.Left = 0
         VScroll1.Top = 0
         If pic123.width > ScaleWidth Then
            HScroll1.Top = ScaleHeight - HScroll1.Height
         Else
            HScroll1.Top = ScaleHeight
         End If
         If pic123.Height > HScroll1.Top Then
            VScroll1.Left = ScaleWidth - VScroll1.width
            If pic123.width > VScroll1.Left Then
               HScroll1.Top = ScaleHeight - HScroll1.Height
            End If
         Else
            VScroll1.Left = ScaleWidth
         End If
         HScroll1.width = ScaleWidth
         If HScroll1.Top > 0 Then VScroll1.Height = HScroll1.Top
         ' Set the scroll bar ranges
         HScroll1.Max = pic123.width - VScroll1.Left
         VScroll1.Max = pic123.Height - HScroll1.Top
         HScroll1.SmallChange = Abs(HScroll1.Max \ 16) + 1
         HScroll1.LargeChange = Abs(HScroll1.Max \ 4) + 1
         VScroll1.SmallChange = Abs(VScroll1.Max \ 16) + 1
         VScroll1.LargeChange = Abs(VScroll1.Max \ 4) + 1
         HScroll1.ZOrder 0
         VScroll1.ZOrder 0
End Sub
Private Sub OptCarDetails_Click()
StrCarStatus = "YES"
'FrmcarDetails.Visible = True
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

Private Sub OptFresh_Click()
AdvFrRen = "PURCHASE"
End Sub

Private Sub OptNocarDetails_Click()
FrmcarDetails.Visible = False
StrCarStatus = "NO"
End Sub

Private Sub OptRen_Click()
AdvFrRen = "REINVESTMENT"
End Sub

Private Sub Picture4_Click()
lostguest = 0
picture4_fun
End Sub
Private Sub picture4_fun()
On Error Resume Next
selected_client_key = treeClient.SelectedItem.Key
Guest_AR = True
Guest_ARGen = False
Load frm_guestsearch
frm_guestsearch.client_master_flg = "Client" 'gaurav
frm_guestsearch.MyGuestCode = TguestCode.Text
'If lostguest = 1 Then
        frm_guestsearch.TguestCode.Text = TguestCode.Text
        frm_guestsearch.tempval.Text = 2
        frm_guestsearch.Command3_Click
        frm_guestsearch.Guestgrid_DblClick
        Unload frm_guestsearch
'Else
'frm_guestsearch.Show
'frm_guestsearch.ZOrder
'End If
'If txtbusicode <> "" Then
'txtbusicode_LostFocus
'End If
End Sub
Private Sub Picture5_Click()
lostpitch = 0
picture5_fun
End Sub

Private Sub picture5_fun()
On Error Resume Next
selected_client_key = treeClient.SelectedItem.Key
Guest_AR = True
Guest_ARGen = False
Load frm_guestsearch
frm_guestsearch.client_master_flg = "Client" 'gaurav
frm_guestsearch.Label1(44).Caption = "Pitch Book No."
frm_guestsearch.MyGuestCode = TguestCode.Text
If lostpitch = 1 Then
        frm_guestsearch.TguestCode.Text = txtPitchBook.Text
        frm_guestsearch.tempval.Text = 1
        frm_guestsearch.Command3_Click
        frm_guestsearch.Guestgrid_DblClick
        Unload frm_guestsearch
Else
frm_guestsearch.Show
frm_guestsearch.ZOrder
End If
If txtbusicode <> "" Then
txtbusicode_LostFocus
End If
End Sub

Private Sub txtaddharCardNo_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 8 Then
Else
    KeyAscii = 0
End If
End Sub



Private Sub txtaddress1_P_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub
Private Sub txtaddress1_Validate(Cancel As Boolean)
txtaddress1.Text = UCase(txtaddress1.Text)
End Sub


Private Sub txtaddress2_P_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub

Private Sub txtaddress2_Validate(Cancel As Boolean)
txtaddress2.Text = UCase(txtaddress2.Text)
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

Public Sub txtbusicode_LostFocus()
Dim rsEmp As Adodb.Recordset
Dim b_cd() As String
Dim RSEMP1 As New Adodb.Recordset
MyBranch = ""
MyRmCode = ""
    If Len(txtbusicode.Text) <> 0 And txtbusicode.Text <> "95829" And txtbusicode.Text <> "103914" Then
        Set rsEmp = New Adodb.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.category_id in('2001','2018')", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            MyBranch = rsEmp(0)
            MyRmCode = rsEmp(3)
            Label32.Caption = rsEmp(1)
            Label55.Caption = rsEmp(2)
        Else
            RSEMP1.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) ", MyConn, adOpenForwardOnly
            If RSEMP1.EOF = False Then
            MsgBox "RM Should be of Sale Support OR Sales", vbInformation
            Else
            MsgBox "RM Should be Active", vbInformation
            End If
            RSEMP1.Close
            Label32.Caption = ""
            Label55.Caption = ""
            If txtbusicode.Text <> "95829" Then
                txtbusicode = ""
            End If
            Exit Sub
        End If
        rsEmp.Close
        Set rsEmp = Nothing
    Else
        If txtbusicode.Text = "95829" Then
           txtbusicode = ""
        End If
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
txtclientname.Text = UCase(txtclientname)
End Sub

Private Function CheckDuplicate() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
        If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
            If SqlRet("select nvl(count(*),0) from client_test where client_pan='" & VSFCommGrdE.TextMatrix(i, 5) & "'") >= 1 Then
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
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
        If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
            If UCase(VSFCommGrdE.TextMatrix(i, 5)) = UCase(VSFCommGrdE.TextMatrix(i + 1, 5)) And Trim(UCase(VSFCommGrdE.TextMatrix(i, 5))) <> "" Then
                 DupFlag = False
                 Exit For
            End If
        End If
     Next
End If
CheckDuplicateInvestor = DupFlag
End Function

Private Function CheckDuplicateInvestorPan() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE.Rows > 1 Then
     For i = 1 To VSFCommGrdE.Rows - 1
        If VSFCommGrdE.TextMatrix(i, 2) <> "" Then
           If VSFCommGrdE.TextMatrix(i, 5) <> "" Then
               MyPan = VSFCommGrdE.TextMatrix(i, 5)
           Else
               MyPan = ""
           End If
           For J = 1 To VSFCommGrdE.Rows - 1
               If UCase(VSFCommGrdE.TextMatrix(J, 5)) = UCase(MyPan) And MyPan <> "" And i <> J Then
                    DupFlag = False
                    Exit For
               End If
           Next
           If DupFlag = False Then
              Exit For
           End If
        End If
     Next
End If
CheckDuplicateInvestorPan = DupFlag
End Function
Private Function CheckDuplicateMainCode1() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
        If VSFCommGrdE.TextMatrix(i, 2) <> "" And UCase(Trim(txtclientname.Text)) <> "" Then
            If UCase(VSFCommGrdE.TextMatrix(i, 2)) = UCase(Trim(txtclientname.Text)) And UCase(VSFCommGrdE.TextMatrix(i, 6)) = UCase(Trim(dob.Text)) Then
                 DupFlag = False
                 Exit For
            End If
        End If
     Next
End If
CheckDuplicateMainCode1 = DupFlag
End Function


Private Function CheckDuplicateMainCode() As Boolean
Dim DupFlag As Boolean
Dim MyPan As String
DupFlag = True
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
        If VSFCommGrdE.TextMatrix(i, 5) <> "" And UCase(Trim(txtclientpan.Text)) <> "" Then
            If UCase(VSFCommGrdE.TextMatrix(i, 5)) = UCase(Trim(txtclientpan.Text)) Then
                 DupFlag = False
                 Exit For
            End If
        End If
     Next
End If
CheckDuplicateMainCode = DupFlag
End Function

Private Sub txtclientpan_Validate(Cancel As Boolean)
txtclientpan.Text = UCase(txtclientpan.Text)
End Sub







Private Sub TxtEmail_o_GotFocus()
TxtEmail_o.Text = LCase(TxtEmail_o.Text)
End Sub

Private Sub TxtEmail_o_Validate(Cancel As Boolean)
TxtEmail_o.Text = LCase(TxtEmail_o.Text)
End Sub


Private Sub txtEMail_Validate(Cancel As Boolean)
txtEmail.Text = LCase(txtEmail.Text)
End Sub
'Private Sub TxtFatherSpouseName_GotFocus()
'If cmbtitle_Spouse.Text = "" Then
'    MsgBox "Please Choose Title First"
'    cmbtitle_Spouse.SetFocus
'    Exit Sub
'End If
'End Sub

Private Sub TxtFatherSpouseName_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub TxtFatherSpouseName_Validate(Cancel As Boolean)
TxtFatherSpouseName.Text = UCase(TxtFatherSpouseName)
End Sub

Private Sub txtguardiann_Validate(Cancel As Boolean)
txtguardiann.Text = UCase(txtguardiann.Text)
End Sub

Private Sub txtguardianname_Validate(Cancel As Boolean)
txtguardianname.Text = UCase(txtguardianname.Text)
End Sub

Private Sub txtguardianpan_Validate(Cancel As Boolean)
txtguardianpan.Text = UCase(txtguardianpan.Text)
End Sub

Private Sub txtoverseasadd_Validate(Cancel As Boolean)
txtoverseasadd.Text = UCase(txtoverseasadd.Text)
End Sub

Private Sub txtpans_Validate(Cancel As Boolean)
If txtpans.Text <> "" Then
    txtclientcode.Text = SqlRet("select NVL(main_code,0) from client_test where upper(Trim(client_pan))='" & Trim(UCase(txtpans)) & "'")
End If
End Sub

Private Sub txtpin_P_KeyPress(KeyAscii As Integer)
If cmbclientn.Text <> "NRI" Then
        If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
            KeyAscii = 0
        End If
        txtPin.MaxLength = 6
    Else
        txtPin.MaxLength = 20
    End If
End Sub


Private Sub txtPitchBook_LostFocus()
'If txtPitchBook.Text <> "" Then
'lostpitch = 1
'picture5_fun
'If TguestCode.Text = "" Then
'txtPitchBook.Text = ""
'txtPitchBook.SetFocus
'End If
'Else
'lostpitch = 0
'End If


End Sub

Private Sub TxtReason_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub




Private Sub TxtReferenceMob_KeyPress(Index As Integer, KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
End If
TxtReferenceMob(Index).MaxLength = 10
End Sub

Private Sub TxtReferenceName_KeyPress(Index As Integer, KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
If KeyAscii = 64 Or KeyAscii = 35 Or KeyAscii = 36 Or KeyAscii = 37 Or KeyAscii = 94 Or KeyAscii = 33 Or KeyAscii = 39 Or KeyAscii = 42 Then
    KeyAscii = 0
End If
End Sub

Private Sub TxtReferenceName_Validate(Index As Integer, Cancel As Boolean)
TxtReferenceName(Index).Text = UCase(TxtReferenceName(Index))
End Sub

Private Sub VSFCommGrdE_CellChanged(ByVal Row As Long, ByVal Col As Long)
'Call VSFCommGrdE_Click
End Sub

Private Sub VSFCommGrdE_DblClick()
If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 0) <> "" And VSFCommGrdE.Col = 0 Then
    For KCount_cOL = 0 To VSFCommGrdE.Cols - 1
        VSFCommGrdE.Col = KCount_cOL
        If VSFCommGrdE.CellBackColor = vbBlue Then
            VSFCommGrdE.CellBackColor = vbWhite
            VSFCommGrdE.CellForeColor = vbBlack
            VSFCommGrdE.CellFontBold = False
        Else
            VSFCommGrdE.CellBackColor = vbBlue
            VSFCommGrdE.CellForeColor = vbWhite
            VSFCommGrdE.CellFontBold = True
        End If
    Next
End If
End Sub
Private Sub VSFCommGrdE_KeyPressEdit(ByVal Row As Long, ByVal Col As Long, KeyAscii As Integer)
If Col = 18 Or Col = 21 Then
    If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 46 Or KeyAscii = 8 Then
    Else
        KeyAscii = 0
    End If
End If
End Sub

Private Sub VSFCommGrdE_ValidateEdit(ByVal Row As Long, ByVal Col As Long, Cancel As Boolean)
If VSFCommGrdE.Col = 6 Then
   If VSFCommGrdE.EditText = "  /  /    " Then Exit Sub
   If Not IsDate(Format(VSFCommGrdE.EditText, "dd/mm/yyyy")) Then
      MsgBox "Please enter Valid date!", vbInformation + vbOKOnly, strBajaj
      Cancel = True
   End If
End If

If VSFCommGrdE.Col = 20 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 20) = "NO" Then
        VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 21) = ""
    End If
End If

If VSFCommGrdE.Col = 21 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 21) <> "" Then
        If Val(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 21)) > 100 Then
            MsgBox "Nominee Percentage can not be greater than 100", vbInformation
            VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 21) = ""
            Cancel = True
        End If
    End If
End If

If VSFCommGrdE.Col = 5 Then
    If SqlRet("SELECT COUNT(*) FROM CLIENT_TEST WHERE CLIENT_PAN='" & VSFCommGrdE.EditText & "' AND MAIN_CODE<>'" & txtclientcode.Text & "'") > 0 Then
       MsgBox "Pls Check The Pan,It Is Already Exist In DataBase", vbInformation
       VSFCommGrdE.EditText = ""
    End If
End If
If VSFCommGrdE.Col = 5 Then
    If VSFCommGrdE.EditText <> "" Then
        If ValidatePan(VSFCommGrdE.EditText) = False Then
             MsgBox "Please Enter Valid Pan No"
             Cancel = True
        End If
    End If
End If

If VSFCommGrdE.Col = 19 Then
    If VSFCommGrdE.EditText = "" Then
        MsgBox "Gender Can Not Be Left Blank"
        Cancel = True
    End If
End If



'to validate aadhar card on investor by pankaj pundir on dated 29022016
If VSFCommGrdE.Col = 18 Then
    If SqlRet("SELECT COUNT(*) FROM CLIENT_TEST WHERE AADHAR_CARD_NO='" & VSFCommGrdE.EditText & "' AND MAIN_CODE<>'" & txtclientcode.Text & "'") > 0 Then
       MsgBox "Pls Check Aadhar Card No,It Is Already Exist In DataBase", vbInformation
       VSFCommGrdE.EditText = ""
       Cancel = True
    End If
End If

If VSFCommGrdE.Col = 13 Then
    If VSFCommGrdE.EditText <> "" Then
        If DateDiff("YYYY", Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 6), "DD/MM/YYYY"), ServerDateTime) < 18 Then
            If ValidatePan(VSFCommGrdE.EditText) = False Then
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
If VSFCommGrdE.Rows > 1 Then
    For i = 1 To VSFCommGrdE.Rows - 1
        If UCase(VSFCommGrdE.TextMatrix(i, 11)) <> "CL" Then
            If UCase(VSFCommGrdE.TextMatrix(i, 1)) <> "" Or UCase(VSFCommGrdE.TextMatrix(i, 2)) <> "" Then
                If UCase(VSFCommGrdE.TextMatrix(i, 6)) = "" Or Mid(UCase(VSFCommGrdE.TextMatrix(i, 7)), 1, 1) = "" Or Mid(UCase(VSFCommGrdE.TextMatrix(i, 7)), 1, 1) = " " Or Mid(UCase(VSFCommGrdE.TextMatrix(i, 9)), 1, 1) = " " Or Mid(UCase(VSFCommGrdE.TextMatrix(i, 9)), 1, 1) = "" Then
                     DupMan = False
                     Exit For
                End If
            End If
         End If
     Next
End If
CheckMandatory = DupMan
End Function
Private Sub cmdAdd_Click()
On Error Resume Next
Dim rs_city As New Adodb.Recordset
Dim cityId As String
Dim GetRecentCLCode As New Adodb.Recordset
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
Dim rs_max_inv As New Adodb.Recordset
Dim MAX_INV_CD As Long
Dim ExistClient As New Adodb.Recordset
Dim ExistTranCode As String
On Error GoTo errr
Dim ExistPan As New Adodb.Recordset
Dim rsDup As New Adodb.Recordset
Dim ClCat As String
Dim invCat As String
Dim rmipc As New Adodb.Recordset
Dim rsacces As New Adodb.Recordset
Dim dguest As New Adodb.Recordset

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
        If txtPin.Text = "" Then
            MsgBox "PIN No can Not Be left Blank", vbInformation
            SSTab1.Tab = 0
            txtPin.SetFocus
            Exit Sub
        End If
        If Len(txtPin.Text) < 6 Or Len(txtPin.Text) > 6 Then
            MsgBox "PIN No Should Be 6 Character", vbInformation
            SSTab1.Tab = 0
            txtPin.SetFocus
            Exit Sub
        End If
    End If
    If GlbroleId <> 22 Then
        If cmbclientn.Text <> "NRI" Then
            If txtMobile.Text = "" Then
                MsgBox "Mobile No can Not Be left Blank", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
            If (Len(txtMobile.Text) < 10 Or Len(txtMobile.Text) > 10) Then
                MsgBox "Mobile No Should Be 10 Character", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
        End If
    Else
        If txtMobile.Text = "" Then
            MsgBox "Mobile No can Not Be left Blank,It Can Be 0", vbInformation
            SSTab1.Tab = 0
            txtMobile.SetFocus
            Exit Sub
        End If
        If cmbclientn.Text <> "NRI" Then
            If (Len(txtMobile.Text) < 10 Or Len(txtMobile.Text) > 10) And txtMobile.Text <> "0" Then
                MsgBox "Mobile No Should Be 10 Character", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
        End If
    End If
    If cmbclientn.Text <> "NRI" Then
        If Len(txtMobile.Text) > 10 And Mid(txtMobile.Text, 1, 1) <> "9" And Mid(txtMobile.Text, 1, 1) <> "8" And Mid(txtMobile.Text, 1, 1) <> "7" And Mid(txtMobile.Text, 1, 1) <> "6" Then
            MsgBox "Mobile No Should Be Start 6 or 7 or 8 or 9", vbInformation
            SSTab1.Tab = 0
            txtMobile.SetFocus
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
    If UCase(txtEmail.Text) <> "NOT AVAILABLE" And UCase(txtEmail.Text) <> "N/A" And UCase(txtEmail.Text) <> "N A" And UCase(txtEmail.Text) <> "NILL" And UCase(txtEmail.Text) <> "NONE" And UCase(txtEmail.Text) <> "N-A" Then
        rsDup.open "Select client_code from client_test where  UPPER(EMAIL)='" & UCase(Trim(txtEmail.Text)) & "'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Account Is Already Existing in the Database ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    If txtMobile.Text <> "" And txtMobile.Text <> "0" Then
        If rsDup.State = 1 Then rsDup.Close
        rsDup.open "Select client_code from client_test where MOBILE_NO='" & Trim(txtMobile.Text) & "' and trim(upper(client_name)) like '%" & UCase(Trim(txtclientname.Text) & "%" & Trim(txtlstname)) & "%'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Account Is Already Existing in the Database ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    
    If txtMobile.Text <> "" And txtMobile.Text <> "0" And dob <> "__/__/____" Then
        If rsDup.State = 1 Then rsDup.Close
        rsDup.open "Select client_code from client_test where mobile_no='" & Trim(txtMobile.Text) & "' AND DOB=TO_DATE('" & Trim(dob) & "','DD-MM-YYYY')", MyConn, adOpenForwardOnly
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
    If UCase(txtEmail.Text) <> "NOT AVAILABLE" And UCase(txtEmail.Text) <> "N/A" And UCase(txtEmail.Text) <> "N A" And UCase(txtEmail.Text) <> "NILL" And UCase(txtEmail.Text) <> "NONE" And UCase(txtEmail.Text) <> "N-A" Then
        rsDup.open "Select client_code from client_master where  UPPER(EMAIL)='" & UCase(Trim(txtEmail.Text)) & "' and trim(upper(client_name)) like '%" & UCase(Trim(txtclientname.Text) & "%" & Trim(txtlstname)) & "%'", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Client Is Already Existing in the Database On Email and Client Name ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    If txtMobile.Text <> "" And txtMobile.Text <> "0" Then
        If rsDup.State = 1 Then rsDup.Close
        rsDup.open "Select client_code from client_master where  MOBILE=" & Trim(txtMobile.Text) & "", MyConn, adOpenForwardOnly
        If rsDup.EOF = False Then
            MsgBox "This Mobile Is Already Existing in Client the Database ", vbInformation
            rsDup.Close
            Set rsDup = Nothing
            Exit Sub
        End If
        rsDup.Close
    End If
    If txtMobile.Text <> "" And txtMobile.Text <> "0" Then
        rsDup.open "Select client_code from client_master where mobile='" & Trim(txtMobile.Text) & "'", MyConn, adOpenForwardOnly
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
             sql = sql & " g_name='" & txtguardianname.Text & "',g_nationality='" & txtguardiann.Text & "',g_pan='" & txtguardianpan.Text & "',add1='" & txtaddress1.Text & "',add2='" & txtaddress2 & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',state_id='" & MyStateCode & "',per_state_id='" & MyPerStateCode & "',pincode='" & txtPin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & txtSTD.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1 & "',tel2='" & TxtTel2.Text & "',mobile_no='" & txtMobile.Text & "',fax ='" & TxtFax.Text & "',email='" & txtEmail & "',"
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
             sql = sql & " '" & txtguardianname.Text & "','" & txtguardiann.Text & "','" & txtguardianpan.Text & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
             If txtSTD.Text = "" Then
                 sql = sql & "0 ,"
             Else
                 sql = sql & " '" & txtSTD.Text & "',"
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
             If txtMobile.Text = "" Then
                 sql = sql & "'',"
             Else
                 sql = sql & " '" & txtMobile.Text & "',"
             End If
             If TxtFax.Text = "" Then
                 sql = sql & " 0 ,"
             Else
                 sql = sql & " '" & TxtFax.Text & "',"
             End If
             sql = sql & " '" & txtEmail & "', "
             sql = sql & " '" & ActSt & "','" & News & "','" & sInfo & "','" & NIPan & "','" & NIAdd & "','" & NIPower & "','" & IPan & "','" & IAdd & "','" & txtlstname & "','" & Glbloginid & "','" & txtbusicode & "',SYSDATE," & txtclientcodeold.Text & "," & ActCat1 & ","
             If txtaddharCardNo.Text <> "" Then
                sql = sql & " " & txtaddharCardNo.Text & " "
             Else
                sql = sql & " '' "
             End If
             sql = sql & " )"
             'Insert New Acount In Client_test
             MyConn.Execute (sql)
             Set clcode = New Adodb.Recordset
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
             If txtMobile.Text <> "" Then
                sql = sql & " and mobile_no='" & txtMobile & "'"
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
        sql = sql & " '" & txtguardianname.Text & "','" & txtguardiann.Text & "','" & txtguardianpan.Text & "','" & txtaddress1.Text & "','" & txtaddress2.Text & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
        If txtSTD.Text = "" Then
            sql = sql & "0 ,"
        Else
            sql = sql & " " & txtSTD.Text & ","
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
        If txtMobile.Text = "" Then
            sql = sql & "'',"
        Else
            sql = sql & " '" & txtMobile.Text & "',"
        End If
        If TxtFax.Text = "" Then
            sql = sql & " 0 ,"
        Else
            sql = sql & " " & TxtFax.Text & ","
        End If
        sql = sql & " '" & txtEmail & "', "
        sql = sql & " '" & ActSt & "','" & News & "','" & sInfo & "','" & NIPan & "','" & NIAdd & "','" & NIPower & "','" & IPan & "','" & IAdd & "','" & txtlstname & "','" & Glbloginid & "','" & txtbusicode & "',SYSDATE," & ActCat1 & ","
        If txtaddharCardNo.Text <> "" Then
            sql = sql & " " & txtaddharCardNo.Text & " "
        Else
            sql = sql & " '' "
        End If
        sql = sql & ")"
        MyConn.Execute (sql)
        Set clcode = New Adodb.Recordset
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
        If txtMobile.Text <> "" Then
          sql = sql & " and mobile_no='" & txtMobile & "'"
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
        Ky = Ky & " '" & txtEmail & "','" & txtclientpan & "',"
        If dob.Text <> "__/__/____" Then
            Ky = Ky & " '" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        Else
            Ky = Ky & " null,"
        End If
        If txtMobile.Text <> "" Then
            Ky = Ky & " '" & txtMobile & "',"
        Else
            Ky = Ky & " '0',"
        End If
        If txtPin.Text <> "" Then
            Ky = Ky & " '" & txtPin & "',"
        Else
            Ky = Ky & " '',"
        End If
        Ky = Ky & " '" & Left(Trim(cmbgender.Text), 1) & "','" & CmbMeritalStatus.Text & "','" & CmbLeadSource.Text & "',"
        Ky = Ky & " '" & Trim(Mid(cmbcity.Text, 1, 50)) & "',"
        If txtSTD.Text <> "" Then
            Ky = Ky & " '" & txtSTD.Text & "',"
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
        If txtMobile.Text <> "" Then
            sql = sql & " AND MOBILE='" & txtMobile & "'"
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
        Ky = Ky & " '" & txtEmail & "','" & txtclientpan & "',"
        If dob.Text <> "__/__/____" Then
            Ky = Ky & " '" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        Else
            Ky = Ky & " null,"
        End If
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
        If TxtTel1.Text <> "" Then
            Ky = Ky & " '" & txtSTD.Text & "-" & TxtTel1.Text & "', "
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
            sql = sql & " '" & txtaddress1.Text & "','" & txtaddress2.Text & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
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
            If txtPin.Text <> "" Then
                Ky = Ky & " '" & txtPin & "',"
            Else
                Ky = Ky & " ' ',"
            End If
            Ky = Ky & " '" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "'," ''" & Left(Trim(cmbgender.Text), 1) & "' GENDER PANKAJ
            If TxtTel1.Text <> "" Then
            Ky = Ky & " '" & txtSTD.Text & "-" & TxtTel1.Text & "', "
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
            Set clcode = New Adodb.Recordset
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
            sql = sql & " add1='" & txtaddress1 & "',add2='" & txtaddress2 & "',city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',state_id='" & MyStateCode & "',per_state_id='" & MyPerStateCode & "',pincode='" & txtPin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & txtSTD.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1 & "',tel2='" & TxtTel2.Text & "',mobile_no='" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            
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
            sql = sql & " '" & txtaddress1 & "','" & txtaddress2 & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            If txtSTD.Text = "" Then
                sql = sql & "0 ,"
            Else
                sql = sql & " '" & txtSTD.Text & "',"
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
            If txtMobile.Text = "" Then
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
            If txtMobile.Text <> "" Then
                Ky = Ky & " mobile='" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            Else
                Ky = Ky & " mobile='',"
            End If
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                Ky = Ky & " AADHAR_CARD_NO=" & VSFCommGrdE.TextMatrix(i, 18) & ","
            Else
                Ky = Ky & " AADHAR_CARD_NO='',"
            End If
            If txtPin.Text <> "" Then
                Ky = Ky & " pincode='" & txtPin & "',"
            Else
                Ky = Ky & " pincode=' ',"
            End If
            Ky = Ky & " gender='" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "'," 'GENDER PANKAJ
            If TxtTel1.Text <> "" Then
                Ky = Ky & " phone1='" & txtSTD.Text & "-" & TxtTel1.Text & "', "
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
Dim RS As New Adodb.Recordset
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
cmdSave.Enabled = True
frameAdvisory.Enabled = True
Call cmdReset_Click
Exit Sub
errr:
MsgBox err.Description
Resume
End Sub

Private Sub cmdExit_Click()
    Unload Me
End Sub

Private Sub cmdnewocc_Click()
    occn = InputBox("Enter occupation", Occupation)
    MyConn.Execute "insert into occupation_master (occ_name,loggeduserid,timest)values('" & Trim(occn) & "','" & Glbloginid & "','" & Format(ServerDateTime, "DD-MMM-YYYY") & "')"
    MyConn.Execute "commit"
    If sql_ora = "SQL" Then
        oc = "select distinct OCC_ID,OCC_NAME from occupation_master where occ_id is not null AND OCC_ID NOT IN(35,33,34,31,32)"
    Else
        oc = "select distinct occ_id,occ_name from occupation_master where occ_id is not null AND OCC_ID NOT IN(35,33,34,31,32)"
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

Private Sub cmdReset_Click()
Dim i As Integer
txtPitchBook.Enabled = True
TguestCode.Enabled = True
txtPitchBook.Text = ""
TguestCode.Text = ""
txtbusicode.Text = ""
lostpitch = 0
lostguest = 0
txtaddharCardNo.Text = ""
VSFCommGrdK.Clear
ApproveFlag = 0
Call SetGrid1
TxtAmount.Text = ""
cmbacatg.ListIndex = -1
cmbinvestor.ListIndex = -1
TxtRemark.Text = ""
txtChqNo.Text = ""
txtpans.Text = ""
cmdadd.Enabled = True
Picture1.Enabled = True
txtbusicode.Enabled = True
cmdUpdate.Enabled = False
cmbplantype.ListIndex = -1
TxtAmount.Text = ""
txtchqamt.Text = ""
txtChqNo.Text = ""
cmbBankName.Text = ""
dtChqDate.Text = "__/__/____"
txtlstname.Text = ""
VSFCommGrdE.Enabled = True
dob.Text = "__/__/____"
txtclientcodeold.Text = ""
txtaddress1.Text = ""
txtaddress2.Text = ""
txtaddress1_P.Text = ""
txtaddress2_P.Text = ""
cmbkyc.Text = ""
txtclient.Text = ""
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
TxtFatherSpouseName.Text = ""
txtclientpan.Text = ""
txtdp_bid.Text = ""
txtEmail.Text = ""
TxtFax.Text = ""
txtguardiann.Text = ""
txtguardianname.Text = ""
txtguardianpan.Text = ""
txtMobile.Text = ""
txtoverseasadd.Text = ""
TxtTel1.Text = ""
TxtTel2.Text = ""
txtPin.Text = ""
txtstd1.Text = ""
cmbcity.ListIndex = -1
cmbstate.ListIndex = -1
cmbactcat.ListIndex = -1

CmbCountry_P.ListIndex = -1
cmbcity_P.ListIndex = -1
cmbstate_P.ListIndex = -1
txtSTD.Text = ""
txttitle.Text = ""
txttradingid.Text = ""
cmbcomminterval.Text = ""
cmbgender.Text = ""
CmbMeritalStatus.Text = ""
CmbLeadSource.Text = ""
cmbincome.Text = ""
cmbinvestment.Text = ""
cmbkyc.Text = ""
cmbkyc1.Text = ""
cmboccupation.ListIndex = -1
cmbprefcommmode.Text = ""
cmbresinri.Text = ""
cmbstatus.Text = ""
cmbtitle.Text = ""
cmbtitle_Spouse.Text = ""
txtstd1.Text = ""
imgp.Picture = LoadPicture("")
imgpan.Picture = LoadPicture("")
MyInvestorSt = ""
MySourceId = ""
MainAccountCode = ""
MyUpdateInvestor = ""
VSFCommGrd.Clear
Call SetGrid
cmdadd.Enabled = True
TxtReferenceName(0).Text = ""
TxtReferenceName(1).Text = ""
TxtReferenceName(2).Text = ""
TxtReferenceName(3).Text = ""
TxtReferenceName(4).Text = ""

TxtReferenceMob(0).Text = ""
TxtReferenceMob(1).Text = ""
TxtReferenceMob(2).Text = ""
TxtReferenceMob(3).Text = ""
TxtReferenceMob(4).Text = ""
End Sub

Public Sub cmdSearch_Click()
Call gridfill
Call SetGrid

'--------------Advisory Fresh/Renewal-----------------
If SqlRet("select count(*) from transaction_st where client_code='" & txtclientcodeold.Text & "' and remark='ADVISORY'") > 0 Then
    optFresh.Value = True
    OptRen.Value = False
Else
    OptRen.Value = True
    optFresh.Value = False
End If
'-----------------------------------------------------

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
    TxtFax.Text = IIf(IsNull(RsSearch.Fields("fax")), "", RsSearch.Fields("fax"))
    txtEmail.Text = IIf(IsNull(RsSearch.Fields("email")), "", RsSearch.Fields("email"))
    txtclientpan.Text = IIf(IsNull(RsSearch.Fields("pan")), "", RsSearch.Fields("pan"))
    If Not IsNull(RsSearch!dob) Then
        dob.Text = Format(RsSearch!dob, "DD/MM/YYYY")
    End If
    txtMobile.Text = IIf(IsNull(RsSearch.Fields("mobile")), "", RsSearch.Fields("mobile"))
    txtPin.Text = IIf(IsNull(RsSearch.Fields("pincode")), "", RsSearch.Fields("pincode"))
    For i = 0 To cmbgender.ListCount - 1
        If UCase(Trim(cmbgender.List(i))) = UCase(Trim(RsSearch!GENDER)) Then
            cmbgender.ListIndex = i
        End If
    Next
    For i = 0 To cmbcity.ListCount - 1
        City = Split(cmbcity.List(i), "#")
        If UCase(Trim(City(1))) = UCase(Trim(RsSearch!City_id)) Then
            cmbcity.ListIndex = i
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
    For i = 0 To cmbstatus.ListCount - 1
        If UCase(Trim(cmbstatus.List(i))) = UCase(Trim(RsSearch!status)) Then
        cmbstatus.ListIndex = i
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
Call txtbusicode_LostFocus
'Fill Extra Information ---------------------------------------------------------
Dim Rs_view As New Adodb.Recordset
Rs_view.open "select * from client_test   where  client_codekyc='" & txtclientcodeold & "'", MyConn, adOpenForwardOnly
If Not Rs_view.EOF Then
    txttradingid = IIf(IsNull(Rs_view.Fields("tradingid")), "", Rs_view.Fields("tradingid"))
    txtdp_bid = IIf(IsNull(Rs_view.Fields("benefi_id")), "", Rs_view.Fields("benefi_id"))
    cmbstatus.Text = IIf(IsNull(Rs_view.Fields("status")), "", Rs_view.Fields("status"))
    txtAcOther.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
    
         
    TxtAcOther1.Text = IIf(IsNull(Rs_view.Fields("other")), "", Rs_view.Fields("other"))
    cmbtitle.Text = IIf(IsNull(Rs_view.Fields("TITLE")), "", Rs_view.Fields("TITLE"))
    txtclientname.Text = IIf(IsNull(Rs_view.Fields("client_name")), "", Rs_view.Fields("client_name"))
    cmbclientn.Text = IIf(IsNull(Rs_view.Fields("nationality")), "", Rs_view.Fields("nationality"))
    
    cmbtitle_Spouse.Text = IIf(IsNull(Rs_view.Fields("TITLE_FATHER_SPOUSE")), "", Rs_view.Fields("TITLE_FATHER_SPOUSE"))
    TxtFatherSpouseName.Text = IIf(IsNull(Rs_view.Fields("FATHER_SPOUSE_NAME")), "", Rs_view.Fields("FATHER_SPOUSE_NAME"))
    
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
               StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!inv_Code & "|"
               rs_inv.MoveNext
           Loop
        End If
        rs_inv.Close
    End If
    VSFCommGrdE.ColComboList(0) = StrInv
    MainAccountCode = SqlRet("select Main_code from client_test where client_codekyc='" & Trim(txtclientcodeold.Text) & "'")
    If MainAccountCode <> "" Then
        RsGridFill.open "select a.client_name,a.client_codekyc,a.client_pan,a.dob,a.inv_kyc,a.IS_NOMINEE,a.g_name,a.g_pan,b.relation_name,b.relation_id,c.occ_name,c.occ_id,title,mobile_no,email from client_test a,relationship_master b,occupation_master c where main_code='" & MainAccountCode & "' and client_code<>'" & MainAccountCode & "' and a.relation_id=b.relation_id(+) and a.occ_id=c.occ_id(+) ", MyConn, adOpenForwardOnly
        i = 1
        If Not RsGridFill.EOF Then
            Do While Not RsGridFill.EOF
                VSFCommGrdE.TextMatrix(i, 0) = RsGridFill.Fields("client_name") & Space(50) & "#" & RsGridFill.Fields("client_codekyc")
                VSFCommGrdE.TextMatrix(i, 1) = RsGridFill.Fields("client_codekyc")
                VSFCommGrdE.TextMatrix(i, 2) = RsGridFill.Fields("client_name")
                VSFCommGrdE.TextMatrix(i, 3) = IIf(IsNull(RsGridFill.Fields("mobile_no")), "", RsGridFill.Fields("mobile_no"))
                VSFCommGrdE.TextMatrix(i, 4) = IIf(IsNull(RsGridFill.Fields("email")), "", RsGridFill.Fields("email"))
                VSFCommGrdE.TextMatrix(i, 5) = IIf(IsNull(RsGridFill.Fields("client_pan")), "", RsGridFill.Fields("client_pan"))
                VSFCommGrdE.TextMatrix(i, 6) = RsGridFill.Fields("dob")
                VSFCommGrdE.TextMatrix(i, 7) = RsGridFill.Fields("relation_name") & Space(50) & "#" & RsGridFill.Fields("relation_id")
                VSFCommGrdE.TextMatrix(i, 9) = RsGridFill.Fields("occ_name") & Space(50) & "#" & RsGridFill.Fields("occ_id")
                VSFCommGrdE.TextMatrix(i, 11) = IIf(IsNull(RsGridFill.Fields("inv_kyc")), "", RsGridFill.Fields("inv_kyc"))
                VSFCommGrdE.TextMatrix(i, 12) = IIf(IsNull(RsGridFill.Fields("g_name")), "", RsGridFill.Fields("g_name"))
                VSFCommGrdE.TextMatrix(i, 13) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
                VSFCommGrdE.TextMatrix(i, 17) = IIf(IsNull(RsGridFill.Fields("title")), "", RsGridFill.Fields("title"))
                VSFCommGrdE.TextMatrix(i, 20) = IIf(IsNull(RsGridFill.Fields("IS_NOMINEE")), "", RsGridFill.Fields("IS_NOMINEE"))
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
            VSFCommGrdE.TextMatrix(i, 6) = ""
            VSFCommGrdE.TextMatrix(i, 7) = ""
            VSFCommGrdE.TextMatrix(i, 9) = ""
            VSFCommGrdE.TextMatrix(i, 11) = ""
            VSFCommGrdE.TextMatrix(i, 12) = ""
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
Dim cmd As New Adodb.Command
Dim UPD_CL_INV As New Adodb.Command
Dim i As Integer
Dim Sql1 As String
Dim Str() As String
Dim ClCat As String
Dim invCat As String
Dim RS As New Adodb.Recordset
Dim rmipc As New Adodb.Recordset
Dim rsacces As New Adodb.Recordset
VSFCommGrdE_LeaveCell
''''''''''''''''''''''''''ADMIN UPDATE VALIDATION'''''''''''''''''''''''''''''''''''''''''''''''''''
If GlbroleId = 1 Then
    MsgBox "You are not authorised to update the details", vbInformation
    cmdUpdate.SetFocus
    Exit Sub
End If
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

'----------------------------------------------ClienMaster Validation-----------------------
If UCase(txtEmail.Text) <> "NOT AVAILABLE" And UCase(txtEmail.Text) <> "N/A" And UCase(txtEmail.Text) <> "N A" And UCase(txtEmail.Text) <> "NILL" And UCase(txtEmail.Text) <> "NONE" And UCase(txtEmail.Text) <> "N-A" Then
    If rsDup.State = 1 Then rsDup.Close
    rsDup.open "Select client_code from client_master where  upper(EMAIL)='" & UCase(Trim(txtEmail.Text)) & "' and client_code <> (select distinct source_code from client_test where main_code='" & txtclientcode.Text & "')", MyConn, adOpenForwardOnly 'and trim(upper(client_name)) like '%" & UCase(Trim(txtclientname.Text) & "%" & Trim(txtlstname)) & "%'
    If rsDup.EOF = False Then
        MsgBox "This Client Is Already Existing in the Database On Email", vbInformation
        rsDup.Close
        Set rsDup = Nothing
        Exit Sub
    End If
    rsDup.Close
End If
If txtMobile.Text <> "" And txtMobile.Text <> "0" Then
    rsDup.open "Select client_code from client_master where mobile='" & Trim(txtMobile.Text) & "' and client_code <> (select   distinct source_code from client_test where main_code='" & txtclientcode.Text & "')", MyConn, adOpenForwardOnly
    If rsDup.EOF = False Then
        MsgBox "Duplicate Mobile ", vbInformation
        rsDup.Close
        Set rsDup = Nothing
        Exit Sub
    End If
    rsDup.Close
End If
rsDup.open "Select client_code from client_master where address1||Address2='" & Trim(txtaddress1.Text) & Trim(txtaddress2.Text) & "' and city_id='" & MyCityCode & "' and sourceid=" & MyBranch & "and client_code <> (select  distinct  source_code from client_test where main_code='" & txtclientcode.Text & "')", MyConn, adOpenForwardOnly
If rsDup.EOF = False Then
    MsgBox "Duplicate Address", vbInformation
    rsDup.Close
    Set rsDup = Nothing
    Exit Sub
End If
rsDup.Close
Set rsDup = Nothing
'-------------------------------------------------------------------------------------------



rsacces.open "select * from access_control_account where role_id=" & GlbroleId, MyConn

If Not rsacces.EOF Or Not rsacces.BOF Then
If rsacces(2) <> 1 Then
MsgBox "Insufficient permission", vbInformation, "Wealthmaker"
rsacces.Close
Exit Sub
End If
Else
MsgBox "Insufficient permission", vbInformation, "Wealthmaker"
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
'rmipc.Close

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

For i = 1 To VSFCommGrdE.Rows - 1
If VSFCommGrdE.TextMatrix(i, 2) <> "" Then
If VSFCommGrdE.TextMatrix(i, 17) = "" Then
   MsgBox "Title can't be null"
   Exit Sub
   End If
   End If
Next

If txtclientpan.Text <> "" Then
    sql = "Select * from client_test where 1=1 "
    If txtclientpan.Text <> "" Then
       sql = sql & " and client_pan='" & txtclientpan.Text & "' AND MAIN_CODE <>'" & txtclientcode.Text & "' "
    End If
    RS.open sql, MyConn, adOpenForwardOnly
    If Not RS.EOF Then
         MsgBox "This Main PAN No is already Exist"
         Exit Sub
    End If
    RS.Close
End If
'---------------------------------------Mayank Garg------------------------------------------
If CheckDuplicateInvestor = False Then
   MsgBox "Check The Duplicacy Of Pan No of Investor"
   Exit Sub
End If

If CheckDuplicateInvestorPan = False Then
   MsgBox "Check The Duplicacy of Pan No of Investor"
   Exit Sub
End If


If CheckMandatory = False Then
   MsgBox "Fill All The Details of investor"
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
'--------------------------------------------------------------------------------------------

For i = 1 To VSFCommGrdE.Rows - 1
    If VSFCommGrdE.TextMatrix(i, 2) <> "" Then
            If txtclientcode.Text <> "" Then
                If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
                    'mobile no validation
                    If Len(Trim(VSFCommGrdE.TextMatrix(i, 3))) <> 10 Then
                        MsgBox "Mobile No " & Trim(VSFCommGrdE.TextMatrix(i, 3)) & " Should Be 10 Character ", vbInformation
                        VSFCommGrdE.TextMatrix(i, 3) = ""
                        Exit Sub
                    End If
                    
                    If Mid(Trim(VSFCommGrdE.TextMatrix(i, 3)), 1, 1) <> "9" And Mid(Trim(VSFCommGrdE.TextMatrix(i, 3)), 1, 1) <> "8" And Mid(Trim(VSFCommGrdE.TextMatrix(i, 3)), 1, 1) <> "6" And Mid(Trim(VSFCommGrdE.TextMatrix(i, 3)), 1, 1) <> "7" And Mid(Trim(VSFCommGrdE.TextMatrix(i, 3)), 1, 1) <> "6" Then
                        MsgBox "Mobile No " & Trim(VSFCommGrdE.TextMatrix(i, 3)) & " Should Be Start 6 or 7 or 8 or 9", vbInformation
                        VSFCommGrdE.TextMatrix(i, 3) = ""
                        Exit Sub
                    End If
                            
                    
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    If SqlRet("select count(*) from client_test where main_CODE <>'" & Trim(txtclientcode.Text) & "' and mobile_no = '" & Trim(VSFCommGrdE.TextMatrix(i, 3)) & "'") > 0 Then
                        MsgBox "Mobile No" & Trim(VSFCommGrdE.TextMatrix(i, 3)) & " is already Exist"
                        Exit Sub
                    End If
                End If
             End If
        
            If UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NOT AVAILABLE" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "N/A" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "N A" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NILL" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NONE" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "N-A" And UCase(Trim(VSFCommGrdE.TextMatrix(i, 4))) <> "NA" Then
                If txtclientcode.Text <> "" Then
                    If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
                        If SqlRet("select count(*) from client_test where main_CODE <>'" & Trim(txtclientcode.Text) & "' and email = '" & Trim(VSFCommGrdE.TextMatrix(i, 4)) & "'") > 0 Then
                            MsgBox "Email " & Trim(VSFCommGrdE.TextMatrix(i, 4)) & " is already Exist"
                            Exit Sub
                        End If
                    End If
                 End If
            End If
         
             If VSFCommGrdE.TextMatrix(i, 7) <> "" And VSFCommGrdE.TextMatrix(i, 7) <> "                                                  #" Then
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
                            If DateDiff("M", Format(VSFCommGrdE.TextMatrix(i, 6), "DD/MM/YYYY"), ServerDateTime) <= 216 Then
        '                        MsgBox "Enter Pan No Of Guardian Only", vbInformation, "Wealthmaker"
        '                        VSFCommGrdE.TextMatrix(i, 5) = ""
        '                        Exit Sub
                            End If
                        End If
                    End If
                  End If
              Else
                MsgBox "Relation can't be left blank", vbInformation, "Wealthmaker"
                Exit Sub
             End If
    End If
Next

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
    If txtPin.Text = "" Then
        MsgBox "PIN No can Not Be left Blank", vbInformation
        SSTab1.Tab = 0
        txtPin.SetFocus
        Exit Sub
    End If
    If cmbclientn.Text <> "NRI" Then
        If Len(txtPin.Text) < 6 Or Len(txtPin.Text) > 6 Then
            MsgBox "PIN No Should Be 6 Character", vbInformation
            SSTab1.Tab = 0
            txtPin.SetFocus
            Exit Sub
        End If
    End If
    If GlbroleId <> 22 Then
        If cmbclientn.Text <> "NRI" Then
            If (Len(txtMobile.Text) < 10 Or Len(txtMobile.Text) > 10) Then
                MsgBox "Mobile No Should Be 10 Character", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
            If Mid(txtMobile.Text, 1, 1) <> "9" And Mid(txtMobile.Text, 1, 1) <> "8" And Mid(txtMobile.Text, 1, 1) <> "7" And Mid(txtMobile.Text, 1, 1) <> "6" Then
                MsgBox "Mobile No Should Be Start 6 or 7 or 8 or 9", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
        End If
    Else
        If cmbclientn.Text <> "NRI" Then
            If (Len(txtMobile.Text) < 10 Or Len(txtMobile.Text) > 10) And txtMobile.Text <> "0" Then
                MsgBox "Mobile No Should Be 10 Character", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
            
            If txtMobile.Text <> "0" And Mid(txtMobile.Text, 1, 1) <> "9" And Mid(txtMobile.Text, 1, 1) <> "8" And Mid(txtMobile.Text, 1, 1) <> "7" And Mid(txtMobile.Text, 1, 1) <> "6" Then
                MsgBox "Mobile No Should Be Start 6 or 7 or 8 or 9", vbInformation
                SSTab1.Tab = 0
                txtMobile.SetFocus
                Exit Sub
            End If
        End If
    End If
End If
If txtEmail.Text <> "" Then
    If UCase(txtEmail.Text) = "NOT AVAILABLE" Or UCase(txtEmail.Text) = "N/A" Or UCase(txtEmail.Text) = "N A" Or UCase(txtEmail.Text) = "NILL" Or UCase(txtEmail.Text) = "NONE" Or UCase(txtEmail.Text) = "N-A" Then
    Else
        If ValidateEmail(txtEmail.Text) = False Then
            MsgBox "Please enter Valid Email Address"
            SSTab1.SetFocus
            txtEmail.SetFocus
            Exit Sub
        End If
    End If
Else
    MsgBox "Please Enter Either Email Address Or Enter Not Available OR N/A OR N A OR N-A OR NONE OR NILL"
End If
If MyBranch <> "10010097" And GlbroleId <> 22 Then
    If txtSTD.Text = "" And txtstd1.Text = "" And TxtTel1.Text = "" And TxtTel2.Text = "" And txtMobile.Text = "" Then
        MsgBox "Enter Atleast One of Your Contact Number", vbInformation
        SSTab1.Tab = 1
        Exit Sub
    End If
End If
If DateDiff("D", Format(dob.Text, "DD/MM/YYYY"), ServerDateTime) < 0 Then
    MsgBox "Date Of Birth Can Not be greater than Current Year", vbInformation
    dob.SetFocus
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

    cmdTransfer_Click
    
'-------------Update Basic InFormation Address City,State,Pin--------------------------------------
If ChkUpdateFaimily.Value = 1 Then
    If txtclientcode.Text <> "" Then
        Set UPD_CL_INV.ActiveConnection = MyConn
        UPD_CL_INV.CommandType = adCmdStoredProc
        UPD_CL_INV.CommandText = "UPDATE_BASIC_DETAILS_FAIMILY "
        UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("Main_Cd", adVarChar, adParamInput, 50, txtclientcode)
        UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("Address1", adVarChar, adParamInput, 50, txtaddress1.Text)
        UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("Address2", adVarChar, adParamInput, 50, txtaddress2.Text)
        UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("City", adVarChar, adParamInput, 50, Mid(cmbcity.Text, 1, 50))
        UPD_CL_INV.Parameters.Append UPD_CL_INV.CreateParameter("login_id", adVarChar, adParamInput, 50, Glbloginid)
        UPD_CL_INV.Execute
     End If
End If
'------------------------------------------------------------------------------------------------------
'Update Advisory Transaction In Transaction_st if businessCode change
If Trim(MyOldBusiCode) <> Trim(txtbusicode.Text) Then
   If MyOldBusiCode = "95829" Then
   Else
        sql = " update transaction_st set branch_code=" & MyBranch & ",rmcode=" & MyRmCode & ",business_rmcode=" & txtbusicode.Text & " where   sch_code in (select osch_code from other_product where prod_class_code='DT035')  "
        sql = sql & " AND substr(CLIENT_CODE,1,8)=" & MySourceId & " "
        MyConn.Execute (sql)
   End If
End If

MyConn.BeginTrans
'-------------------------------------------------------------------------------------------
''''''''''''''''''''''''Coment By Pankaj 27-march-2010'''''''''''''''''''''''''''''''''''''''''''''''
'If Glbloginid = 101877 Or Glbloginid = 38423 Or Glbloginid = 97117 Or Glbloginid = 95584 Or GlbroleId = 1 Or GlbroleId = 212 Then
 IsFamilyHead = SqlRet("select IS_FAMILY_HEAD (" & MyUpdateClient & ") from dual")
 If 1 = 1 Then
    'NEW CONDITION ADDED BY PANKAJ PUNDIR FOR FAMILYHEAD
    If IsFamilyHead = "1" Then
        Ky = " update client_master SET PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "', "
        Ky = Ky & "  PHONE=DECODE (STD1, '0', '', '', '', STD1) || DECODE (PHONE1, '0', '', '', '', PHONE1)|| '  '|| DECODE (STD2, '0', '', '', '', STD2)|| DECODE (PHONE2, '0', '', '', '', PHONE2) || '  '|| DECODE (STD3, '0', '', '', '', STD3)|| DECODE (PHONE3, '0', '', '', '', PHONE3),category_id='" & ClCat & "',investor_code='" & invCat & "',GUEST_CD='" & TguestCode & "', CLIENT_TITLE='" & cmbtitle.Text & "',TITLE_FATHER_SPOUSE='" & cmbtitle_Spouse.Text & "', CLIENT_LNAME='" & txtlstname & "', client_name='" & txtclientname & "',FATHER_SPOUSE_NAME='" & TxtFatherSpouseName.Text & "',address1='" & txtaddress1 & "',address2='" & txtaddress2 & "',"
        If TxtFax <> "" Then
            Ky = Ky & " fax='" & TxtFax & "',"
        Else
            Ky = Ky & " fax='0' ,"
        End If
        Ky = Ky & " email='" & txtEmail & "',pan='" & txtclientpan & "',dob='" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        If txtMobile.Text <> "" Then
            Ky = Ky & " mobile='" & txtMobile & "',"
        Else
            Ky = Ky & " mobile= '0',"
        End If
        If txtPin.Text <> "" Then
            Ky = Ky & " pincode='" & txtPin & "',"
        Else
            Ky = Ky & " pincode='',"
        End If
        Ky = Ky & " gender='" & Left(Trim(cmbgender.Text), 1) & "',MARITAL_STATUS='" & CmbMeritalStatus.Text & "',LEAD_SOURCE='" & CmbLeadSource.Text & "',"
        Ky = Ky & " city_name='" & Trim(Mid(cmbcity.Text, 1, 50)) & "',"
        If txtSTD.Text <> "" Then
            Ky = Ky & " std1='" & txtSTD.Text & "',"
        Else
            Ky = Ky & " std1='',"
        End If
        If txtstd1.Text <> "" Then
            Ky = Ky & "std2= '" & txtstd1.Text & "',"
        Else
            Ky = Ky & " std2='',"
        End If
        If TxtTel1.Text <> "" Then
            Ky = Ky & " phone1='" & TxtTel1.Text & "',"
        Else
            Ky = Ky & " phone1='',"
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & " phone2='" & TxtTel2.Text & "',"
        Else
            Ky = Ky & " phone2='',"
        End If
        Ky = Ky & " status='" & cmbstatus.Text & "',occ_id=" & MyOccCode & ",city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',"
        If cmbprefcommmode.Tag <> "" Then
            Ky = Ky & " comm_id=" & cmbprefcommmode.Tag & ","
        Else
            Ky = Ky & " comm_id =  0, "
        End If
        If cmbcomminterval.Tag <> "" Then
            Ky = Ky & " comm_int_id= " & cmbcomminterval.Tag & ","
        Else
            Ky = Ky & " comm_int_id = 0, "
        End If
        Ky = Ky & " kyc='YES',loggeduserid='" & Glbloginid & "',SOURCEID=" & MyBranch & ",RM_CODE=" & MyRmCode & ",act_cat=" & ActCat1 & " where client_code= " & Trim(Mid(MyUpdateClient, 1, 8)) & ""
        MyConn.Execute (Ky)
        
        'To Update Mf Client Master
        Ky = " update client_master@mf.bajajcapital SET PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "', "
        Ky = Ky & " PHONE=DECODE (STD1, '0', '', '', '', STD1) || DECODE (PHONE1, '0', '', '', '', PHONE1)|| '  '|| DECODE (STD2, '0', '', '', '', STD2)|| DECODE (PHONE2, '0', '', '', '', PHONE2) || '  '|| DECODE (STD3, '0', '', '', '', STD3)|| DECODE (PHONE3, '0', '', '', '', PHONE3),category_id='" & ClCat & "',investor_code='" & invCat & "',GUEST_CD='" & TguestCode & "', CLIENT_TITLE='" & cmbtitle.Text & "',TITLE_FATHER_SPOUSE='" & cmbtitle_Spouse.Text & "' ,CLIENT_LNAME='" & txtlstname & "', client_name='" & txtclientname & "',FATHER_SPOUSE_NAME='" & TxtFatherSpouseName.Text & "',address1='" & txtaddress1 & "',address2='" & txtaddress2 & "',"
        If TxtFax <> "" Then
            Ky = Ky & " fax='" & TxtFax & "',"
        Else
            Ky = Ky & " fax='0' ,"
        End If
        Ky = Ky & " email='" & txtEmail & "',pan='" & txtclientpan & "',dob='" & Format(dob.Text, "DD-MMM-YYYY") & "',"
        If txtMobile.Text <> "" Then
            Ky = Ky & " mobile='" & txtMobile & "',"
        Else
            Ky = Ky & " mobile= '0',"
        End If
        If txtPin.Text <> "" Then
            Ky = Ky & " pincode='" & txtPin & "',"
        Else
            Ky = Ky & " pincode='',"
        End If
        Ky = Ky & " gender='" & Left(Trim(cmbgender.Text), 1) & "',MARITAL_STATUS='" & CmbMeritalStatus.Text & "',LEAD_SOURCE='" & CmbLeadSource.Text & "',"
        Ky = Ky & " city_name='" & Trim(Mid(cmbcity.Text, 1, 50)) & "',"
        If txtSTD.Text <> "" Then
            Ky = Ky & " std1='" & txtSTD.Text & "',"
        Else
            Ky = Ky & " std1='',"
        End If
        If txtstd1.Text <> "" Then
            Ky = Ky & "std2= '" & txtstd1.Text & "',"
        Else
            Ky = Ky & " std2='',"
        End If
        If TxtTel1.Text <> "" Then
            Ky = Ky & " phone1='" & TxtTel1.Text & "',"
        Else
            Ky = Ky & " phone1='',"
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & " phone2='" & TxtTel2.Text & "',"
        Else
            Ky = Ky & " phone2='',"
        End If
        Ky = Ky & " status='" & cmbstatus.Text & "',occ_id=" & MyOccCode & ",city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',"
        If cmbprefcommmode.Tag <> "" Then
            Ky = Ky & " comm_id=" & cmbprefcommmode.Tag & ","
        Else
            Ky = Ky & " comm_id =  0, "
        End If
        If cmbcomminterval.Tag <> "" Then
            Ky = Ky & " comm_int_id= " & cmbcomminterval.Tag & ","
        Else
            Ky = Ky & " comm_int_id = 0, "
        End If
        Ky = Ky & " kyc='YES',loggeduserid='" & Glbloginid & "',SOURCEID=" & MyBranch & ",RM_CODE=" & MyRmCode & ",act_cat=" & ActCat1 & " where client_code= " & Trim(Mid(MyUpdateClient, 1, 8)) & ""
        MyConn.Execute (Ky)
    'COMMENT BY PANKAJ PUNDIR ON DATED 16012017 TO IDENTIFY FAMILY HEAD
    ElseIf IsFamilyHead = "0" Then
        Ky = " update client_master SET PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "', "
        Ky = Ky & " category_id='" & ClCat & "',investor_code='" & invCat & "',GUEST_CD='" & TguestCode & "', CLIENT_TITLE='" & cmbtitle.Text & "',TITLE_FATHER_SPOUSE='" & cmbtitle_Spouse.Text & "', CLIENT_LNAME='" & txtlstname & "', client_name='" & txtclientname & "',FATHER_SPOUSE_NAME='" & TxtFatherSpouseName.Text & "',address1='" & txtaddress1 & "',address2='" & txtaddress2 & "',"
        If TxtFax <> "" Then
            Ky = Ky & " fax='" & TxtFax & "',"
        Else
            Ky = Ky & " fax='0' ,"
        End If
        If txtMobile.Text <> "" Then
            Ky = Ky & " mobile='" & txtMobile & "',"
        Else
            Ky = Ky & " mobile= '0',"
        End If
        If txtPin.Text <> "" Then
            Ky = Ky & " pincode='" & txtPin & "',"
        Else
            Ky = Ky & " pincode='',"
        End If
        Ky = Ky & " gender='" & Left(Trim(cmbgender.Text), 1) & "',MARITAL_STATUS='" & CmbMeritalStatus.Text & "',LEAD_SOURCE='" & CmbLeadSource.Text & "',"
        Ky = Ky & " city_name='" & Trim(Mid(cmbcity.Text, 1, 50)) & "',"
        If txtSTD.Text <> "" Then
            Ky = Ky & " std1='" & txtSTD.Text & "',"
        Else
            Ky = Ky & " std1='',"
        End If
        If txtstd1.Text <> "" Then
            Ky = Ky & "std2= '" & txtstd1.Text & "',"
        Else
            Ky = Ky & " std2='',"
        End If
        If TxtTel1.Text <> "" Then
            Ky = Ky & " phone1='" & TxtTel1.Text & "',"
        Else
            Ky = Ky & " phone1='',"
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & " phone2='" & TxtTel2.Text & "',"
        Else
            Ky = Ky & " phone2='',"
        End If
        Ky = Ky & " status='" & cmbstatus.Text & "',occ_id=" & MyOccCode & " ,city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',"
        If cmbprefcommmode.Tag <> "" Then
            Ky = Ky & " comm_id=" & cmbprefcommmode.Tag & ","
        Else
            Ky = Ky & " comm_id =  0, "
        End If
        If cmbcomminterval.Tag <> "" Then
            Ky = Ky & " comm_int_id= " & cmbcomminterval.Tag & ","
        Else
            Ky = Ky & " comm_int_id = 0, "
        End If
        Ky = Ky & " kyc='YES',loggeduserid='" & Glbloginid & "',SOURCEID=" & MyBranch & ",RM_CODE=" & MyRmCode & ",act_cat=" & ActCat1 & " where client_code= " & Trim(Mid(MyUpdateClient, 1, 8)) & ""
        MyConn.Execute (Ky)
        
        'Update Mf Client Master
        
        Ky = " update client_master@mf.bajajcapital SET PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "', "
        Ky = Ky & " category_id='" & ClCat & "',investor_code='" & invCat & "',GUEST_CD='" & TguestCode & "', CLIENT_TITLE='" & cmbtitle.Text & "',TITLE_FATHER_SPOUSE='" & cmbtitle_Spouse.Text & "', CLIENT_LNAME='" & txtlstname & "', client_name='" & txtclientname & "',FATHER_SPOUSE_NAME='" & TxtFatherSpouseName.Text & "',address1='" & txtaddress1 & "',address2='" & txtaddress2 & "',"
        If TxtFax <> "" Then
            Ky = Ky & " fax='" & TxtFax & "',"
        Else
            Ky = Ky & " fax='0' ,"
        End If
        If txtMobile.Text <> "" Then
            Ky = Ky & " mobile='" & txtMobile & "',"
        Else
            Ky = Ky & " mobile= '0',"
        End If
        If txtPin.Text <> "" Then
            Ky = Ky & " pincode='" & txtPin & "',"
        Else
            Ky = Ky & " pincode='',"
        End If
        Ky = Ky & " gender='" & Left(Trim(cmbgender.Text), 1) & "',MARITAL_STATUS='" & CmbMeritalStatus.Text & "',LEAD_SOURCE='" & CmbLeadSource.Text & "',"
        Ky = Ky & " city_name='" & Trim(Mid(cmbcity.Text, 1, 50)) & "',"
        If txtSTD.Text <> "" Then
            Ky = Ky & " std1='" & txtSTD.Text & "',"
        Else
            Ky = Ky & " std1='',"
        End If
        If txtstd1.Text <> "" Then
            Ky = Ky & "std2= '" & txtstd1.Text & "',"
        Else
            Ky = Ky & " std2='',"
        End If
        If TxtTel1.Text <> "" Then
            Ky = Ky & " phone1='" & TxtTel1.Text & "',"
        Else
            Ky = Ky & " phone1='',"
        End If
        If TxtTel2.Text <> "" Then
            Ky = Ky & " phone2='" & TxtTel2.Text & "',"
        Else
            Ky = Ky & " phone2='',"
        End If
        Ky = Ky & " status='" & cmbstatus.Text & "',occ_id=" & MyOccCode & ",city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',"
        If cmbprefcommmode.Tag <> "" Then
            Ky = Ky & " comm_id=" & cmbprefcommmode.Tag & ","
        Else
            Ky = Ky & " comm_id =  0, "
        End If
        If cmbcomminterval.Tag <> "" Then
            Ky = Ky & " comm_int_id= " & cmbcomminterval.Tag & ","
        Else
            Ky = Ky & " comm_int_id = 0, "
        End If
        Ky = Ky & " kyc='YES',loggeduserid='" & Glbloginid & "',SOURCEID=" & MyBranch & ",RM_CODE=" & MyRmCode & ",act_cat=" & ActCat1 & " where client_code= " & Trim(Mid(MyUpdateClient, 1, 8)) & ""
        MyConn.Execute (Ky)
        
    End If
End If
sql = "update client_test set PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "', "
sql = sql & " NATURE_BUSINESS='" & cmbnatureofbusiness.Text & "',NATURE_TRUST='" & cmbnatureoftrust.Text & "',business_code=" & txtbusicode.Text & ",branch_code=" & MyBranch & ", clientCode_old=" & Val(txtclientcodeold.Text) & ",tradingid='" & txttradingid.Text & "',benefi_id='" & txtdp_bid.Text & "',status='" & cmbstatus.Text & "',other='" & txtAcOther.Text & "',occ_id=" & MyOccCode & ",others1='" & TxtAcOther1.Text & "',"
sql = sql & " title='" & cmbtitle.Text & "',TITLE_FATHER_SPOUSE='" & cmbtitle_Spouse.Text & "',client_name='" & txtclientname.Text & "',FATHER_SPOUSE_NAME='" & TxtFatherSpouseName.Text & "',nationality='" & cmbclientn.Text & "',client_pan='" & txtclientpan.Text & "',"
If txtaddharCardNo.Text <> "" Then
    sql = sql & " AADHAR_CARD_NO=" & txtaddharCardNo.Text & ","
Else
    sql = sql & " AADHAR_CARD_NO='',"
End If
sql = sql & " annual_income='" & cmbincome.Text & "',investement='" & cmbinvestment.Text & "',title_other='" & txttitle.Text & "',resident_nri='" & cmbresinri.Text & "',dob=TO_DATE('" & Trim(dob) & "','DD-MM-YYYY'),gender='" & cmbgender.Text & "',MARITAL_STATUS='" & CmbMeritalStatus.Text & "',LEAD_SOURCE='" & CmbLeadSource.Text & "',"
sql = sql & " a_enclosed_pan='" & StrEnclosedPanA & "',a_enclosed_60='" & StrEnclosed60A & "',a_enclosed_61='" & StrEnclosed61A & "',G_enclosed_pan='" & StrEnclosedPanG & "',g_enclosed_60='" & StrEnclosed60G & "',g_enclosed_61='" & StrEnclosed61G & "',a_kyc_status='" & cmbkyc.Text & "',g_kyc_status='" & cmbkyc1.Text & "',"
sql = sql & " g_name='" & txtguardianname.Text & "',g_nationality='" & txtguardiann.Text & "',g_pan='" & txtguardianpan.Text & "',add1='" & txtaddress1 & "',add2='" & txtaddress2 & "',city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',state_id='" & MyStateCode & "',per_state_id='" & MyPerStateCode & "',pincode='" & txtPin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & txtSTD.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1.Text & "',tel2='" & TxtTel2.Text & "',mobile_no='" & txtMobile.Text & "',fax='" & TxtFax.Text & "',email='" & txtEmail & "',"
sql = sql & " tele_Act_statement ='" & ActSt & "' ,tele_newsletter='" & News & "',tele_statutory_info='" & sInfo & "',de_ni_pan='" & NIPan & "',de_ni_addproof='" & NIAdd & "',de_ni_power='" & NIPower & "',de_i_pan='" & IPan & "',de_i_add_proof='" & IAdd & "',client_lastname='" & txtlstname & "',MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,act_cat=" & ActCat1 & ""
sql = sql & " where (client_code='" & txtclientcode.Text & "' or client_pan='" & txtpans.Text & "')"
MyConn.Execute (sql)
'If Glbloginid = 101877 Or Glbloginid = 38423 Or Glbloginid = 97117 Or Glbloginid = 95584 Or GlbroleId = 1 Or GlbroleId = 212 Or GlbroleId = 22 Then
If 1 = 1 Then
    sql = "update investor_master set BRANCH_CODE=" & MyBranch & ",RM_CODE=" & MyRmCode & ", status='" & cmbstatus.Text & "',OCCUPATION_ID=" & MyOccCode & ""
    sql = sql & " ,investor_title='" & cmbtitle.Text & "',investor_name='" & txtclientname.Text & "',pan='" & txtclientpan.Text & "',"
    If txtaddharCardNo.Text <> "" Then
        sql = sql & " AADHAR_CARD_NO=" & txtaddharCardNo.Text & ","
    Else
        sql = sql & " AADHAR_CARD_NO='',"
    End If
        
    sql = sql & " dob=TO_DATE('" & Trim(dob) & "','DD-MM-YYYY'),gender='" & Left(cmbgender.Text, 1) & "',"
    sql = sql & " address1='" & txtaddress1 & "',address2='" & txtaddress2 & "',city_id='" & MyCityCode & "',pincode='" & txtPin.Text & "',mobile='" & txtMobile.Text & "',MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE, fax='" & TxtFax.Text & "',email='" & txtEmail & "',phone1 = '" & txtSTD.Text & "-" & TxtTel1.Text & "',phone2='" & txtstd1.Text & "-" & TxtTel2.Text & "'"
    sql = sql & " where inv_code=" & MyUpdateInvestor & ""
    MyConn.Execute (sql)
    
    'To Update Investor Master
    sql = "update investor_master@MF.BAJAJCAPITAL set BRANCH_CODE=" & MyBranch & ",RM_CODE=" & MyRmCode & ", status='" & cmbstatus.Text & "',OCCUPATION_ID=" & MyOccCode & ""
    sql = sql & " ,investor_title='" & cmbtitle.Text & "',investor_name='" & txtclientname.Text & "',pan='" & txtclientpan.Text & "',"
    
    If txtaddharCardNo.Text <> "" Then
        sql = sql & " AADHAR_CARD_NO=" & txtaddharCardNo.Text & ","
    Else
        sql = sql & " AADHAR_CARD_NO='',"
    End If
    
    sql = sql & " dob=TO_DATE('" & Trim(dob) & "','DD-MM-YYYY'),gender='" & Left(cmbgender.Text, 1) & "'," '----INVESTOR_TITLE='" & cmbtitle.Text & "', INVESTOR_LNAME='" & txtlstname & "',
    sql = sql & " address1='" & txtaddress1 & "',address2='" & txtaddress2 & "',city_id='" & MyCityCode & "',pincode='" & txtPin.Text & "',mobile='" & txtMobile.Text & "',MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE, fax='" & TxtFax.Text & "',email='" & txtEmail & "',phone1 = '" & txtSTD.Text & "-" & TxtTel1.Text & "',phone2='" & txtstd1.Text & "-" & TxtTel2.Text & "'"
    sql = sql & " where inv_code=" & MyUpdateInvestor & ""
    MyConn.Execute (sql)
End If
MyConn.Execute ("update client_test set OPTCAR='" & StrCarStatus & "' where (main_code='" & txtclientcode.Text & "')")
'FOR GRID ENTRY(Now There May Be Two Type Of Client In Grid One New For Kyc One Already Kyc-------------
'If Account Is Already Is Not KYC Then It Shoud Be Inserted In client_test And Update In Investor Master
'If This Account Is  Kyc Then It Should Be Updated In client_test
'-------------------------------------------------------------------------------------------------------
For i = 1 To VSFCommGrdE.Rows - 1
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
    If VSFCommGrdE.TextMatrix(i, 1) <> "" Then
        AlreadyKyc = SqlRet("select nvl(count(*),0) from client_test where client_codekyc=" & VSFCommGrdE.TextMatrix(i, 1) & "")
        If AlreadyKyc > 0 Then 'It Means It Is KYC
            MySourceId = Mid(VSFCommGrdE.TextMatrix(i, 1), 1, 8)
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
            End If
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id1 = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
            End If
            'update client test-------mayank
            'GENDER PANKAJ
            sql = "update client_test set PER_PINCODE='" & txtpin_P.Text & "',OFFICE_EMAIL='" & TxtEmail_o.Text & "',PER_ADD1='" & txtaddress1_P.Text & "',PER_ADD2='" & txtaddress2_P.Text & "',REF_NAME1='" & TxtReferenceName(0).Text & "' , REF_NAME2='" & TxtReferenceName(1).Text & "',REF_NAME3='" & TxtReferenceName(2).Text & "',REF_NAME4='" & TxtReferenceName(3).Text & "',REF_NAME5='" & TxtReferenceName(4).Text & "',REF_MOB1='" & TxtReferenceMob(0).Text & "' , REF_MOB2='" & TxtReferenceMob(1).Text & "',REF_MOB3='" & TxtReferenceMob(2).Text & "',REF_MOB4='" & TxtReferenceMob(3).Text & "',REF_MOB5='" & TxtReferenceMob(4).Text & "',  "
            sql = sql & " GENDER='" & VSFCommGrdE.TextMatrix(i, 19) & "', BRANCH_CODE=" & MyBranch & ",  clientCode_old='" & MyUpdateClient & "',"
            If txttradingid.Text <> "" Then
                sql = sql & " tradingid='" & txttradingid.Text & "',"
            End If
            If txtdp_bid.Text <> "" Then
                sql = sql & " benefi_id='" & txtdp_bid.Text & "',"
            End If
            If Temp_Id(1) <> "" Then
                sql = sql & " occ_id=" & Temp_Id(1) & ","
            End If
            If Temp_Id1(1) <> "" Then
                sql = sql & " relation_id = " & Temp_Id1(1) & ","
            End If
            sql = sql & " inv_kyc='" & VSFCommGrdE.TextMatrix(i, 11) & "',IS_NOMINEE='" & VSFCommGrdE.TextMatrix(i, 20) & "',g_name='" & VSFCommGrdE.TextMatrix(i, 12) & "',g_pan= '" & VSFCommGrdE.TextMatrix(i, 13) & "',"
            sql = sql & " title='" & VSFCommGrdE.TextMatrix(i, 17) & "',client_name='" & VSFCommGrdE.TextMatrix(i, 2) & "'"
            
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                sql = sql & " ,AADHAR_CARD_NO=" & VSFCommGrdE.TextMatrix(i, 18) & ""
            End If
            
            If VSFCommGrdE.TextMatrix(i, 21) <> "" Then
                sql = sql & " ,NOMINEE_PER=" & VSFCommGrdE.TextMatrix(i, 21) & ""
            Else
                sql = sql & " ,NOMINEE_PER=''"
            End If
            
            sql = sql & " ,nationality='" & cmbclientn.Text & "',client_pan='" & VSFCommGrdE.TextMatrix(i, 5) & "',resident_nri='" & cmbresinri.Text & "',dob=TO_DATE('" & Trim(VSFCommGrdE.TextMatrix(i, 6)) & "','DD-MM-YYYY'),"
            sql = sql & " add1='" & Replace(txtaddress1, ",", "") & "',add2='" & Replace(txtaddress2, ",", "") & "',city_id='" & MyCityCode & "',per_city_id='" & MyPerCityCode & "',state_id='" & MyStateCode & "',per_state_id='" & MyPerStateCode & "',pincode='" & txtPin.Text & "',comm_id=" & CommMode & ",comm_inter_id=" & CommInterval & ",overseas_add='" & txtoverseasadd.Text & "',std1='" & txtSTD.Text & "',std2='" & txtstd1.Text & "',tel1='" & TxtTel1 & "',tel2='" & TxtTel2.Text & "',mobile_no='" & VSFCommGrdE.TextMatrix(i, 3) & "',fax ='" & TxtFax.Text & "',email='" & VSFCommGrdE.TextMatrix(i, 4) & "',"
            sql = sql & " MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,business_code=" & txtbusicode & ""
            sql = sql & " where client_codekyc='" & Trim(VSFCommGrdE.TextMatrix(i, 1)) & "'"
            If Len(Trim(VSFCommGrdE.TextMatrix(i, 16))) > 4 Then
                sql = sql & " AND client_code='" & Trim(VSFCommGrdE.TextMatrix(i, 16)) & "'"
            End If
            
            MyConn.Execute (sql)
            
            Ky = "update investor_master "
            Ky = Ky & " set investor_title='" & VSFCommGrdE.TextMatrix(i, 17) & "',MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE, investor_name='" & VSFCommGrdE.TextMatrix(i, 2) & "',address1='" & Replace(txtaddress1, ",", "") & "',address2='" & Replace(txtaddress2, ",", "") & "',"
            If TxtFax <> "" Then
                Ky = Ky & " fax='" & TxtFax & "',"
            Else
                Ky = Ky & " fax='0' ,"
            End If
            Ky = Ky & " email='" & VSFCommGrdE.TextMatrix(i, 4) & "',"
            If VSFCommGrdE.TextMatrix(i, 11) <> "CL" And VSFCommGrdE.TextMatrix(i, 5) <> "" Then
                Ky = Ky & " pan='" & VSFCommGrdE.TextMatrix(i, 5) & "',"
            End If
            If VSFCommGrdE.TextMatrix(i, 6) <> "" Then
                Ky = Ky & " dob=to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY'),"
            End If
            If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
                Ky = Ky & " mobile='" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            Else
                Ky = Ky & " mobile='',"
            End If
            If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                Ky = Ky & " AADHAR_CARD_NO=" & VSFCommGrdE.TextMatrix(i, 18) & ","
            Else
                Ky = Ky & " AADHAR_CARD_NO='',"
            End If
            If txtPin.Text <> "" Then
                Ky = Ky & " pincode='" & txtPin & "',"
            Else
                Ky = Ky & " pincode='',"
            End If
            Ky = Ky & " gender='" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "'," 'GENDER PANKAJ
            If TxtTel2.Text <> "" Then
                Ky = Ky & " phone='" & TxtTel2.Text & "' "
            Else
                Ky = Ky & " phone='' "
            End If
            Ky = Ky & " ,status='" & cmbstatus.Text & "',city_id='" & MyCityCode & "',"
            Ky = Ky & " kyc='YES',source_id=" & MySourceId & ",rm_code=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & ", "
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
                If Temp_Id(1) <> "" Then
                    Ky = Ky & "  REL_ID= " & Temp_Id(1) & ","
                End If
            Else
                Ky = Ky & " REL_ID=0 ,"
            End If
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                If Temp_Id(1) <> "" Then
                    Ky = Ky & " OCCUPATION_ID= " & Temp_Id(1) & ","
                End If
            Else
                Ky = Ky & " OCCUPATION_ID=0, "
            End If
            Ky = Ky & " G_NAME='" & Trim(VSFCommGrdE.TextMatrix(i, 12)) & "',G_pan='" & Trim(VSFCommGrdE.TextMatrix(i, 13)) & "'  "
            Ky = Ky & " where inv_code= " & Trim(VSFCommGrdE.TextMatrix(i, 1)) & " and inv_code<>" & MyInvestorSt & " "
            MyConn.Execute (Ky)
        ElseIf AlreadyKyc = 0 Then 'Insert Into client_test But Update Investor Master
            sql = "insert into client_test(PER_PINCODE,OFFICE_EMAIL,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,client_codekyc,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,relation_id,inv_kyc,IS_NOMINEE,g_name,g_pan,others1,"
            sql = sql & " title,client_name,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,"
            sql = sql & " add1,add2,city_id,per_city_id,state_id,per_state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,AADHAR_CARD_NO,NOMINEE_PER,fax,email,"
            sql = sql & " loggeduserid,business_code,timest,main_code) "
            sql = sql & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "'," & VSFCommGrdE.TextMatrix(i, 1) & "," & Val(MyBranch) & ", '" & MyUpdateClient & "','" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbstatus.Text & "','" & txtAcOther.Text & "',"
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                If Temp_Id(1) <> "" Then
                    sql = sql & " " & Temp_Id(1) & ","
                Else
                    sql = sql & " 0 ,"
                End If
            Else
                 sql = sql & " 0 ,"
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
            sql = sql & " '" & txtaddress1 & "','" & txtaddress2 & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            
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
            If VSFCommGrdE.TextMatrix(i, 3) = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            End If
            
            If VSFCommGrdE.TextMatrix(i, 18) = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 18) & ","
            End If
            
            If VSFCommGrdE.TextMatrix(i, 21) = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 21) & ","
            End If
            
            
            If TxtFax.Text = "" Then
                sql = sql & " '0' ,"
            Else
                sql = sql & " '" & TxtFax.Text & "',"
            End If
            sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 4) & "', "
            sql = sql & " '" & Glbloginid & "','" & txtbusicode & "','" & Format(ServerDateTime, "DD-MMM-YYYY") & "',"
            sql = sql & "'" & MainAccountCode & "')"
            MyConn.Execute (sql)
            
             Ky = "update investor_master "
             Ky = Ky & " set investor_title='" & VSFCommGrdE.TextMatrix(i, 17) & "', MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE, investor_name='" & VSFCommGrdE.TextMatrix(i, 2) & "',address1='" & Replace(txtaddress1, ",", "") & "',address2='" & Replace(txtaddress2, ",", "") & "',"
             If TxtFax <> "" Then
                 Ky = Ky & " fax='" & TxtFax & "',"
             Else
                 Ky = Ky & " fax='0' ,"
             End If
             Ky = Ky & " email='" & VSFCommGrdE.TextMatrix(i, 4) & "',pan='" & VSFCommGrdE.TextMatrix(i, 5) & "',dob=to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY'),"
             If VSFCommGrdE.TextMatrix(i, 3) <> "" Then
                 Ky = Ky & " mobile='" & VSFCommGrdE.TextMatrix(i, 3) & "',"
             Else
                 Ky = Ky & " mobile='',"
             End If
             If VSFCommGrdE.TextMatrix(i, 18) <> "" Then
                 Ky = Ky & " AADHAR_CARD_NO=" & VSFCommGrdE.TextMatrix(i, 18) & ","
             Else
                 Ky = Ky & " AADHAR_CARD_NO='',"
             End If
             If txtPin.Text <> "" Then
                 Ky = Ky & " pincode='" & txtPin & "',"
             Else
                 Ky = Ky & " pincode=' ',"
             End If
             Ky = Ky & " gender='" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "'," 'GENDER PANKAJ
             If TxtTel1.Text <> "" Then
                Ky = Ky & " phone1='" & txtSTD & "-" & TxtTel1.Text & "',"
             Else
                Ky = Ky & " phone1='',"
             End If
             If TxtTel2.Text <> "" Then
                Ky = Ky & " phone2='" & txtstd1 & "-" & TxtTel2.Text & "',"
             Else
                Ky = Ky & " phone2='',"
             End If
             Ky = Ky & " status='" & cmbstatus.Text & "',city_id='" & MyCityCode & "',"
             Ky = Ky & " kyc='YES',source_id=" & MySourceId & ",rm_code=" & MyRmCode & ",BRANCH_CODE=" & MyBranch & ", "
            If VSFCommGrdE.TextMatrix(i, 7) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 7), "#")
                If Temp_Id(1) <> "" Then
                    Ky = Ky & "  REL_ID= " & Temp_Id(1) & ","
                Else
                    Ky = Ky & " REL_ID=0 ,"
                End If
            Else
                Ky = Ky & " REL_ID=0 ,"
            End If
            If VSFCommGrdE.TextMatrix(i, 9) <> "" Then
                Temp_Id = Split(VSFCommGrdE.TextMatrix(i, 9), "#")
                If Temp_Id(1) <> "" Then
                    Ky = Ky & " OCCUPATION_ID= " & Temp_Id(1) & ","
                Else
                    Ky = Ky & "  OCCUPATION_ID=0, "
                End If
            Else
                Ky = Ky & "  OCCUPATION_ID=0, "
            End If
            Ky = Ky & " G_NAME='" & Trim(VSFCommGrdE.TextMatrix(i, 12)) & "',G_pan='" & Trim(VSFCommGrdE.TextMatrix(i, 13)) & "'  "
            Ky = Ky & " where inv_code= " & Trim(VSFCommGrdE.TextMatrix(i, 1)) & " "
            MyConn.Execute (Ky)
        End If
   End If
        If (VSFCommGrdE.TextMatrix(i, 1) = "" And VSFCommGrdE.TextMatrix(i, 2) <> "") Then  'Agar New Entry Karte Hai Grid Par
            sql = "insert into client_test(PER_PINCODE,OFFICE_EMAIL,REF_NAME1,REF_NAME2,REF_NAME3,REF_NAME4,REF_NAME5,REF_MOB1,REF_MOB2,REF_MOB3,REF_MOB4,REF_MOB5,BRANCH_CODE,clientCode_old,tradingid,benefi_id,status,other,OCC_ID,relation_id,inv_kyc,IS_NOMINEE,g_name,g_pan,others1,"
            sql = sql & " title,client_name,nationality,client_pan,annual_income,investement,title_other,resident_nri,dob,gender,"
            sql = sql & " add1,add2,city_id,per_city_id,state_id,per_state_id,pincode,comm_id,comm_inter_id,overseas_add,std1,std2,tel1,tel2,mobile_no,AADHAR_CARD_NO,NOMINEE_PER,fax,email,"
            sql = sql & " loggeduserid,business_code,timest,main_code) "
            sql = sql & " values('" & txtpin_P.Text & "','" & TxtEmail_o.Text & "','" & TxtReferenceName(0).Text & "','" & TxtReferenceName(1).Text & "','" & TxtReferenceName(2).Text & "','" & TxtReferenceName(3).Text & "','" & TxtReferenceName(4).Text & "' ,'" & TxtReferenceMob(0).Text & "','" & TxtReferenceMob(1).Text & "','" & TxtReferenceMob(2).Text & "','" & TxtReferenceMob(3).Text & "','" & TxtReferenceMob(4).Text & "'," & Val(MyBranch) & "," & MyUpdateClient & ",'" & txttradingid.Text & "','" & txtdp_bid.Text & "','" & cmbstatus.Text & "','" & txtAcOther.Text & "',"
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
            sql = sql & " '" & txtaddress1 & "','" & txtaddress2 & "','" & MyCityCode & "','" & MyPerCityCode & "','" & MyStateCode & "','" & MyPerStateCode & "','" & txtPin.Text & "'," & CommMode & "," & CommInterval & ",'" & txtoverseasadd.Text & "',"
            
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
            If VSFCommGrdE.TextMatrix(i, 3) = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 3) & "',"
            End If
            
            If VSFCommGrdE.TextMatrix(i, 18) = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 18) & ","
            End If
            
            If VSFCommGrdE.TextMatrix(i, 21) = "" Then
                sql = sql & "'',"
            Else
                sql = sql & " " & VSFCommGrdE.TextMatrix(i, 21) & ","
            End If
            
            If TxtFax.Text = "" Then
                sql = sql & " '0' ,"
            Else
                sql = sql & " '" & TxtFax.Text & "',"
            End If
            sql = sql & " '" & VSFCommGrdE.TextMatrix(i, 4) & "', "
            sql = sql & " '" & Glbloginid & "','" & txtbusicode & "','" & Format(ServerDateTime, "DD-MMM-YYYY") & "',"
            sql = sql & "'" & MainAccountCode & "')"
            'Insert New Acount In client_test
            MyConn.Execute (sql)
            '1 Client And 1 Investor hase Been Insert into client_master and investor_master
            'MySourceiD=ClientCode of new insert client in clientmaster
            '----------------------------------------ADD ALL GRID VALUE IN INVESTOR _MASTER --------------------
            Ky = "insert into investor_master (INVESTOR_CODE,investor_title,branch_Code,investor_name,address1,address2,fax,email,pan,dob,mobile,AADHAR_CARD_NO,pincode,gender,phone1,phone2,status,city_id,kyc,source_id,rm_code,rel_id,occupation_id)"
            Ky = Ky & " values(1,'" & VSFCommGrdE.TextMatrix(i, 17) & "','" & MyBranch & "', '" & VSFCommGrdE.TextMatrix(i, 2) & "','" & txtaddress1 & "','" & txtaddress2 & "',"
            If TxtFax <> "" Then
                Ky = Ky & " " & TxtFax & ","
            Else
                Ky = Ky & " 0 ,"
            End If
            'vinit  11-aug-2014
            Ky = Ky & " '" & VSFCommGrdE.TextMatrix(i, 4) & "','" & VSFCommGrdE.TextMatrix(i, 5) & "', to_date('" & VSFCommGrdE.TextMatrix(i, 6) & "','DD-MM-YYYY') ,"
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
            If txtPin.Text <> "" Then
                Ky = Ky & " '" & txtPin & "',"
            Else
                Ky = Ky & " ' ',"
            End If
            Ky = Ky & " '" & Left(Trim(VSFCommGrdE.TextMatrix(i, 19)), 1) & "',"
            If TxtTel1.Text <> "" Then
                Ky = Ky & " '" & txtSTD.Text & "-" & TxtTel1.Text & "', "
            Else
                Ky = Ky & " '',"
            End If
            If TxtTel2.Text <> "" Then
                Ky = Ky & " '" & txtstd1.Text & "-" & TxtTel2.Text & "', "
            Else
                Ky = Ky & " '',"
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
            Set clcode = New Adodb.Recordset
            Set clcode = MyConn.Execute("select 'AH'||max(TO_NUMBER(SUBSTR(client_code,3,LENGTH(CLIENT_CODE)))) CLIENT_CODE from client_test where  MAIN_CODE='" & MainAccountCode & "'")
            If Not clcode.EOF Then
                  sql = "   update client_test set client_codekyc=( select max(inv_code) from investor_master where source_id='" & MySourceId & "') "
                  sql = sql & "  where client_code='" & clcode(0) & "' "
                  MyConn.Execute sql
            End If
            If clcode.State = 1 Then clcode.Close
            Dim rs_max_inv As New Adodb.Recordset
            rs_max_inv.open "SELECT TO_NUMBER(MAX(SUBSTR(INV_CODE,9,13))) FROM investor_master WHERE SOURCE_ID=" & MySourceId, MyConn, adOpenForwardOnly
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
        ElseIf VSFCommGrdE.TextMatrix(i, 1) = "" And VSFCommGrdE.TextMatrix(i, 2) = "" Then
             Exit For
        End If
Next
'ENDING OF GRID ENTRY-----------------------------------------------------------------------------------------
'-------------------------------------------------------------------------------------------------------------
MsgBox "The Requested Record has been Updated Successfully", vbInformation
MyConn.CommitTrans
'----------------------------- Update Bajaj_ar_head by mayank--------------------------------------
'Cl_BAH = Trim(MyUpdateClient)

Set UPD_BAH.ActiveConnection = MyConn
UPD_BAH.CommandType = adCmdStoredProc
UPD_BAH.CommandText = "PRC_update_bajajarhead"
UPD_BAH.Parameters.Append UPD_BAH.CreateParameter("client_code", adVarChar, adParamInput, 50, MyUpdateClient)
UPD_BAH.Execute
Set UPD_BAH = Nothing
'--------------------------------------------------------------------------------------------------


''''''''''Save Car Details '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
iRow = 1
Dim CompanyCd As String
Dim Modelid As String
Do While VSFCommGrdK.TextMatrix(iRow, 0) <> ""
   If VSFCommGrdK.TextMatrix(iRow, 7) = "UPD" Then
       Str = Split(VSFCommGrdK.TextMatrix(iRow, 1), "#")
       Modelid = Str(1)
       If VSFCommGrdK.TextMatrix(iRow, 4) <> "" Then
            Str = Split(VSFCommGrdK.TextMatrix(iRow, 4), "#")
            CompanyCd = Str(1)
       End If
       Mysql = "update Accounts_Car_Details set model_id='" & Modelid & "'"
       Mysql = Mysql & ",company_Cd ='" & CompanyCd & "',login_id ='" & Glbloginid & "',"
       If VSFCommGrdK.TextMatrix(iRow, 3) = "  /    " Then
          Mysql = Mysql & "renewal_Date=NULL,"
       Else
          Mysql = Mysql & "renewal_Date=TO_DATE('" & (VSFCommGrdK.TextMatrix(iRow, 3)) & "','DD/MM/YYYY'),"
       End If
       If VSFCommGrdK.TextMatrix(iRow, 2) = "  /    " Then
          Mysql = Mysql & "manif_year=NULL,"
       Else
          Mysql = Mysql & "manif_year='" & (VSFCommGrdK.TextMatrix(iRow, 2)) & "',REMARKS='" & (VSFCommGrdK.TextMatrix(iRow, 6)) & "' where main_code='" & txtclientcode.Text & "'"
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
         Mysql = "SELECT * FROM Accounts_Car_Details WHERE Main_code='" & txtclientcode.Text & "' and model_id='" & Modelid & "' and company_cd='" & CompanyCd & "'  "
         RS.open Mysql, MyConn
         If RS.EOF Then
         Mysql = ""
               Mysql = "insert into Accounts_Car_Details(Main_Code,Model_id,company_cd,LOGIN_ID ,renewal_date,manif_year,REMARKS) values('"
               Mysql = Mysql & txtclientcode.Text & "','" & Modelid & "','" & CompanyCd & "','" & Glbloginid & "', "
               If VSFCommGrdK.TextMatrix(iRow, 3) = "  /    " Then
                  Mysql = Mysql & " NULL, "
               Else
                  Mysql = Mysql & " TO_DATE('" & (VSFCommGrdK.TextMatrix(iRow, 3)) & "','DD/MM/YYYY'),"
               End If
               If VSFCommGrdK.TextMatrix(iRow, 2) = "  /    " Then
                  Mysql = Mysql & " NULL) "
               Else
                  Mysql = Mysql & "'" & (VSFCommGrdK.TextMatrix(iRow, 2)) & "','" & (VSFCommGrdK.TextMatrix(iRow, 6)) & "')"
               End If
               MyConn.Execute Mysql
               RS.Close
        End If
   End If
   iRow = iRow + 1
Loop
If RS.State = 1 Then RS.Close
'--------------------------------------------------------------------------------------------------
If ApproveFlag = 0 Then
    Call cmdReset_Click
End If
cmdUpdate.Enabled = False
cmdadd.Enabled = True
Exit Sub
errr:
MsgBox err.Description
MyConn.RollbackTrans
Resume
End Sub


Public Sub cmdview_Click()
On Error Resume Next
Dim rsCity As New Adodb.Recordset
Dim rsState As New Adodb.Recordset
Dim RsPan As New Adodb.Recordset
Dim MyInvCode As String
Dim ClientCat As String
cmdadd.Enabled = False
cmdUpdate.Enabled = True
cmdSave.Enabled = True
Dim Rs_Relation As New Adodb.Recordset
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

Set Rs_view = New Adodb.Recordset
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
    
    For i = 0 To cmbactcat.ListCount - 1
        ActCat = Split(cmbactcat.List(i), "#")
        If UCase(Trim(ActCat(1))) = UCase(Trim(Rs_view!act_cat)) Then
            cmbactcat.ListIndex = i
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
    
    For i = 0 To CmbCountry.ListCount - 1
        Country = Split(CmbCountry.List(i), "#")
        If UCase(Trim(Country(1))) = MyCountryCode Then
            CmbCountry.ListIndex = i
            Exit For
        End If
    Next
    
    For i = 0 To CmbCountry_P.ListCount - 1
        PerCountry = Split(CmbCountry_P.List(i), "#")
        If UCase(Trim(PerCountry(1))) = MyPerCountryCode Then
            CmbCountry_P.ListIndex = i
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
    
    For i = 0 To cmbstate_P.ListCount - 1
        MyPerState = Split(cmbstate_P.List(i), "#")
        If UCase(Trim(MyPerState(1))) = UCase(Trim(Rs_view!per_state_id)) Then
            cmbstate_P.ListIndex = i
            Exit For
        End If
    Next
    
    Call cmbstate_Click
    For i = 0 To cmbcity.ListCount - 1
        City = Split(cmbcity.List(i), "#")
        If UCase(Trim(City(1))) = UCase(Trim(Rs_view!City_id)) Then
            cmbcity.ListIndex = i
            Exit For
        End If
    Next
    
    Call cmbstate_P_Click
    For i = 0 To cmbcity_P.ListCount - 1
        PerCity = Split(cmbcity_P.List(i), "#")
        If UCase(Trim(PerCity(1))) = UCase(Trim(Rs_view!per_City_id)) Then
            cmbcity_P.ListIndex = i
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
    For i = 0 To cmbacatg.ListCount - 1
        ClienCode = Split(cmbacatg.List(i), "#")
        If UCase(Trim(ClienCode(1))) = UCase(Trim(ClientCat)) Then
            cmbacatg.ListIndex = i
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
    i = 1
    If UCase(Trim(cmbstatus.Text)) = UCase(Trim("Others")) Then
    txtAcOther.Enabled = True
Else
    txtAcOther.Enabled = False
End If
RsGridFill.open "select a.client_name,a.client_code,a.client_codekyc,decode(approved_flag,'YES','YES','NO')approved_flag,a.client_pan,a.dob,a.inv_kyc,a.IS_NOMINEE,a.nominee_per,a.g_name,a.g_pan,b.relation_name,b.relation_id,c.occ_name,c.occ_id,title,mobile_no,email,AADHAR_CARD_NO,GENDER from client_test a,relationship_master b,occupation_master c where main_code=(SELECT MAIN_CODE FROM CLIENT_TEST WHERE CLIENT_CODE='" & txtclientcode.Text & "') and client_code<>MAIN_CODE   and a.relation_id=b.relation_id(+) and a.occ_id=c.occ_id(+) ", MyConn, adOpenForwardOnly
i = 1
If Not RsGridFill.EOF Then
    Do While Not RsGridFill.EOF
        VSFCommGrdE.TextMatrix(i, 0) = RsGridFill.Fields("client_name") & Space(50) & "#" & RsGridFill.Fields("client_codekyc")
        VSFCommGrdE.TextMatrix(i, 1) = RsGridFill.Fields("client_codekyc")
        VSFCommGrdE.TextMatrix(i, 2) = RsGridFill.Fields("client_name")
        VSFCommGrdE.TextMatrix(i, 3) = IIf(IsNull(RsGridFill.Fields("mobile_no")), "", RsGridFill.Fields("mobile_no"))
        VSFCommGrdE.TextMatrix(i, 4) = IIf(IsNull(RsGridFill.Fields("email")), "", RsGridFill.Fields("email"))
        VSFCommGrdE.TextMatrix(i, 5) = IIf(IsNull(RsGridFill.Fields("client_pan")), "", RsGridFill.Fields("client_pan"))
        VSFCommGrdE.TextMatrix(i, 6) = RsGridFill.Fields("dob")
        VSFCommGrdE.TextMatrix(i, 7) = RsGridFill.Fields("relation_name") & Space(50) & "#" & RsGridFill.Fields("relation_id")
        VSFCommGrdE.TextMatrix(i, 9) = RsGridFill.Fields("occ_name") & Space(50) & "#" & RsGridFill.Fields("occ_id")
        VSFCommGrdE.TextMatrix(i, 11) = IIf(IsNull(RsGridFill.Fields("inv_kyc")), "", RsGridFill.Fields("inv_kyc"))
        VSFCommGrdE.TextMatrix(i, 12) = IIf(IsNull(RsGridFill.Fields("g_name")), "", RsGridFill.Fields("g_name"))
        VSFCommGrdE.TextMatrix(i, 13) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
        VSFCommGrdE.TextMatrix(i, 13) = IIf(IsNull(RsGridFill.Fields("g_pan")), "", RsGridFill.Fields("g_pan"))
        VSFCommGrdE.TextMatrix(i, 15) = IIf(IsNull(RsGridFill.Fields("approved_flag")), "", RsGridFill.Fields("approved_flag"))
        VSFCommGrdE.TextMatrix(i, 16) = IIf(IsNull(RsGridFill.Fields("Client_Code")), "", RsGridFill.Fields("Client_Code"))
        VSFCommGrdE.TextMatrix(i, 17) = IIf(IsNull(RsGridFill.Fields("title")), "", RsGridFill.Fields("title"))
        VSFCommGrdE.TextMatrix(i, 18) = IIf(IsNull(RsGridFill.Fields("AADHAR_CARD_NO")), "", RsGridFill.Fields("AADHAR_CARD_NO"))
        VSFCommGrdE.TextMatrix(i, 19) = IIf(IsNull(RsGridFill.Fields("gender")), "", RsGridFill.Fields("gender"))
        VSFCommGrdE.TextMatrix(i, 20) = IIf(IsNull(RsGridFill.Fields("IS_NOMINEE")), "", RsGridFill.Fields("IS_NOMINEE"))
        VSFCommGrdE.TextMatrix(i, 21) = IIf(IsNull(RsGridFill.Fields("NOMINEE_PER")), "", RsGridFill.Fields("NOMINEE_PER"))
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
    VSFCommGrdE.TextMatrix(i, 6) = ""
    VSFCommGrdE.TextMatrix(i, 7) = ""
    VSFCommGrdE.TextMatrix(i, 9) = ""
    VSFCommGrdE.TextMatrix(i, 11) = ""
    VSFCommGrdE.TextMatrix(i, 12) = ""
    VSFCommGrdE.TextMatrix(i, 13) = ""
End If
RsGridFill.Close
'--------------------------------------Fill Car Details----------------------------------------------------------------------
sql = ""
sql = " select a.model_id,c.model_name,MANIF_YEAR,renewal_date,REMARKS,a.company_Cd,company_name, d.manuf_name,d.manuf_id from Accounts_Car_Details a ,bajaj_company_master b,bajaj_model_master c,bajaj_mnf_master d "
sql = sql & "where a.model_id=c.model_id and a.company_cd=b.company_Cd(+) and c.manuf_id=d.manuf_id  and main_code='" & txtclientcode.Text & "'"
RsGridFill.open sql, MyConn, adOpenForwardOnly
i = 1
If Not RsGridFill.EOF Then
    Do While Not RsGridFill.EOF
        VSFCommGrdK.TextMatrix(i, 0) = RsGridFill.Fields("manuf_name") & Space(50) & "#" & RsGridFill!manuf_id
        VSFCommGrdK.TextMatrix(i, 1) = RsGridFill.Fields("model_name") & Space(50) & "#" & RsGridFill!Model_id
        VSFCommGrdK.TextMatrix(i, 2) = IIf(IsNull(RsGridFill.Fields("MANIF_YEAR")), "", RsGridFill.Fields("MANIF_YEAR"))
        VSFCommGrdK.TextMatrix(i, 3) = RsGridFill.Fields("RENEWAL_DATE")
        VSFCommGrdK.TextMatrix(i, 4) = RsGridFill.Fields("company_name") & Space(50) & "#" & RsGridFill.Fields("Company_Cd")
        VSFCommGrdK.TextMatrix(i, 5) = RsGridFill.Fields("Company_cd")
        VSFCommGrdK.TextMatrix(i, 6) = IIf(IsNull(RsGridFill.Fields("REMARKS")), "", RsGridFill.Fields("REMARKS"))
        VSFCommGrdK.TextMatrix(i, 7) = "OLD"
        RsGridFill.MoveNext
        i = i + 1
    Loop
Else
    VSFCommGrdK.TextMatrix(i, 0) = ""
    VSFCommGrdK.TextMatrix(i, 1) = ""
    VSFCommGrdK.TextMatrix(i, 2) = ""
    VSFCommGrdK.TextMatrix(i, 3) = ""
    VSFCommGrdK.TextMatrix(i, 4) = ""
    VSFCommGrdK.TextMatrix(i, 5) = ""
    VSFCommGrdK.TextMatrix(i, 6) = ""
    VSFCommGrdK.TextMatrix(i, 7) = ""
End If
RsGridFill.Close
'------------------------------------------------------------------------------------------------------------
If MyUpdateInvestor <> "" Then
    StrInv = ""
    If rs_inv.State = 1 Then rs_inv.Close
    rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(MyUpdateInvestor, 8) & "' AND INV_CODE<>'" & MyUpdateInvestor & "' order by inv_code", MyConn, adOpenForwardOnly
    If Not rs_inv.EOF Then
       Do While Not rs_inv.EOF
           StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!inv_Code & "|"
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
    For i = 0 To cmbplantype.ListCount - 1
        MyPlan = Split(cmbplantype.List(i), "=")
        If UCase(Trim(MyPlan(1))) = UCase(Trim(RsAdvisory!sch_code)) Then
            cmbplantype.ListIndex = i
            Exit For
        End If
    Next
    TxtAmount.Text = RsAdvisory.Fields("amount")
    For i = 0 To cmbBankName.ListCount - 1
        If UCase(Trim(cmbBankName.List(i))) = UCase(Trim(RsAdvisory!bank_name)) Then
            cmbBankName.ListIndex = i
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
    
    Call cmdReset_Click
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
Private Sub DisplayImage(X As String)
'On Error Resume next
Dim RS As New Adodb.Recordset
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
Dim RS As New Adodb.Recordset
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
Dim rsacces As New Adodb.Recordset

rsacces.open "select * from access_control_account where role_id=" & GlbroleId, MyConn
If Not rsacces.EOF Or Not rsacces.BOF Then
    If rsacces(1) <> 1 Then
        cmdadd.Enabled = False
    End If
    If rsacces(2) <> 1 Then
        cmdUpdate.Enabled = False
    End If
Else
    cmdadd.Enabled = False
    cmdUpdate.Enabled = False
End If


'Date Validation
If dob.Text <> "__/__/____" Then
    If DateDiff("M", Format(dob.Text, "DD/MM/YYYY"), ServerDateTime) <= 216 Then
        'If Left(UCase(cmbinvestor.Text), 10) = "INDIVIDUAL" Then
        If Left(UCase(cmbinvestor.Text), 3) = "HUF" Or Left(UCase(cmbinvestor.Text), 5) = "TRUST" Or Left(UCase(cmbinvestor.Text), 7) = "COMPANY" Or Left(UCase(cmbinvestor.Text), 7) = "SOCIETY" Then
            frameguardian.Enabled = False
        Else
            frameguardian.Enabled = True
        End If
    Else
        frameguardian.Enabled = False
    End If
End If
If frameguardian.Enabled = True Then
    If txtguardianpan.Text <> "" Then
        If ValidatePan(txtguardianpan.Text) = False Then
           MsgBox "Plaes Enter Valid Pan No. of Guardian"
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
If IsDate(dob.Text) = False Then
    MsgBox "Please Enter Valid Date", vbInformation
    dob.SetFocus
    SSTab1.Tab = 0
    Exit Sub
End If
If DateDiff("D", Format(dob.Text, "DD/MM/YYYY"), ServerDateTime) < 0 Then
        MsgBox "Date Of Birth Can Not be grater than Current Year", vbInformation
        dob.SetFocus
    If MyBranch <> "10010097" Then
        SSTab1.Tab = 0
        Exit Sub
    End If
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

'ADD BY PANKAJ PUNDIR ON DATED 13052016 AS PER ANUP JI
If Left(UCase(cmbinvestor.Text), 10) = "INDIVIDUAL" Then
    If DateDiff("YYYY", Format(dob.Text, "DD/MM/YYYY"), ServerDateTime) < 18 Then
          If MyOccCode <> 3 Then
               MsgBox "Kindly Choose Occupation Correctly ", vbInformation
               SSTab1.Tab = 0
               Exit Sub
          End If
    End If
End If

'If Len(txtbusicode.Text) <> 5 Then
'    MsgBox "please enter 5 digit Business Code"
'    txtbusicode.SetFocus
'    Exit Sub
'End If
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
If txtEmail.Text = "" Then
    MsgBox "Email can not Left Balnk"
    Exit Sub
End If
If txtEmail.Text <> "" Then
    If UCase(txtEmail.Text) = "NOT AVAILABLE" Or UCase(txtEmail.Text) = "N/A" Or UCase(txtEmail.Text) = "N A" Or UCase(txtEmail.Text) = "NILL" Or UCase(txtEmail.Text) = "NONE" Or UCase(txtEmail.Text) = "N-A" Then
    Else
        If ValidateEmail(txtEmail.Text) = False Then
            MsgBox "Please enter Valid Email Address Or Enter Not Available OR N/A OR N A OR N-A OR NILL OR NONE"
            SSTab1.SetFocus
            txtEmail.SetFocus
            Exit Sub
        End If
    End If
Else
    MsgBox "Please Enter Either Email Address Or Enter Not Available OR N/A OR N A OR N-A OR NILL OR NONE"
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
If MyBranch <> "10010097" And GlbroleId <> 22 Then
    If txtSTD.Text = "" And txtstd1.Text = "" And TxtTel1.Text = "" And TxtTel2.Text = "" And txtMobile.Text = "" Then
        MsgBox "Enter Atleast One of Your Contact Number", vbInformation
        SSTab1.Tab = 0
        txtMobile.SetFocus
        Exit Sub
    End If
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
Sub HScroll1_Change()
   pic123.Left = -HScroll1.Value
End Sub
      
      
Sub VScroll1_Change()
   pic123.Top = -VScroll1.Value
End Sub
Private Sub Form_Load()

'------Advisory Fresh/Renewal------------------
optFresh.Value = True
OptRen.Value = False
'----------------------------------------------
Call SetGrid1
txtclientcode.Text = "AH"
ApproveFlag = 0
OptCarDetails.Value = True
If GlbroleId = 1 Or GlbroleId = 124 Or GlbroleId = 54 Or GlbroleId = 22 Then 'Or Glbloginid = 101877
    cmdnewocc.Enabled = True
Else
    cmdnewocc.Enabled = False
End If
If Glbloginid = 101877 Or Glbloginid = 43452 Or Glbloginid = 38423 Or Glbloginid = 46183 Or Glbloginid = 97117 Or Glbloginid = 95584 Or Glbloginid = 101877 Then
     CmdInvestor.Visible = True
     cmdapprove.Enabled = True
Else
     CmdInvestor.Visible = False
     cmdapprove.Enabled = False
End If
If GlbroleId = 13 Or GlbroleId = 78 Or GlbroleId = 76 Or GlbroleId = 212 Or GlbroleId = 1 Or GlbroleId = 242 Then
    Frameguest.Visible = True
Else
    Frameguest.Visible = False
End If
recdate.Text = Format(Date, "dd/mm/yyyy")
FillClientCate
FillInvestorCat
cmbacatg.ListIndex = 4
If sql_ora = "SQL" Then
    oc = "select distinct OCC_ID,OCC_NAME from occupation_master where occ_id is not null AND OCC_ID NOT IN(35,33,34,31,32)"
Else
    oc = "select distinct occ_id,occ_name from occupation_master where occ_id is not null AND OCC_ID NOT IN(35,33,34,31,32)"
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
SSTab1.TabEnabled(1) = False
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


cmbstatus.AddItem "Resident Individual", 0
cmbstatus.AddItem "Resident Minor", 1
cmbstatus.AddItem "NRI(Repatriable)", 2
cmbstatus.AddItem "NRI(Non-Repatriable)", 3
cmbstatus.AddItem "NRI Minor(Repatriable)", 4
cmbstatus.AddItem "NRI Minor(Non-Repatriable)", 5
cmbstatus.AddItem "Sole-Propriter", 6
cmbstatus.AddItem "Others", 7
cmbstatus.AddItem ""
cmbstatus.ListIndex = 7


cmbnatureofbusiness.AddItem "Manufacture", 0
cmbnatureofbusiness.AddItem "Distributer", 1
cmbnatureofbusiness.AddItem "Trader", 2
cmbnatureofbusiness.AddItem "Retail", 3
cmbnatureofbusiness.AddItem "Service Provider"
cmbnatureofbusiness.AddItem "Others"

cmbnatureoftrust.AddItem "Charitable"
cmbnatureoftrust.AddItem "Religious"



cmboccupation.AddItem ""
txtp.Text = ""
txtpan.Text = ""


cmbclientn.AddItem "RESIDENT"
cmbclientn.AddItem "INDIAN"
cmbclientn.AddItem "NRI"
cmbclientn.AddItem "NON NRI"
cmbclientn.ListIndex = 1

cmbgender.AddItem "Male", 0
cmbgender.AddItem "Female", 1
cmbgender.AddItem "Non-Ind", 2


CmbMeritalStatus.AddItem "Single", 0
CmbMeritalStatus.AddItem "Married", 1
CmbMeritalStatus.AddItem "None", 2

CmbLeadSource.AddItem "Online", 0
CmbLeadSource.AddItem "Reference", 1
CmbLeadSource.AddItem "Newspaper", 2
CmbLeadSource.AddItem "Other", 3

cmbkyc.AddItem "Yes", 0
cmbkyc.AddItem "NO", 1

cmbkyc1.AddItem "Yes", 0
cmbkyc1.AddItem "NO", 1

cmbresinri.AddItem "Resident", 0
cmbresinri.AddItem "NRI", 1
cmbresinri.ListIndex = 0

cmbtitle.Clear
cmbtitle_Spouse.Clear
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


cmbtitle_Spouse.Clear
cmbtitle_Spouse.AddItem "Mr."
cmbtitle_Spouse.AddItem "Mrs."
cmbtitle_Spouse.AddItem "Ms."
cmbtitle_Spouse.AddItem "M/S."
cmbtitle_Spouse.AddItem "Dr."
cmbtitle_Spouse.AddItem "Adv."
cmbtitle_Spouse.AddItem "Col."
cmbtitle_Spouse.AddItem "Maj."
cmbtitle_Spouse.AddItem "Gen."
cmbtitle_Spouse.AddItem "Brig."
cmbtitle_Spouse.AddItem "CDR."
cmbtitle_Spouse.AddItem "SQN LDR"
cmbtitle_Spouse.AddItem "CAPT."
cmbtitle_Spouse.AddItem "LT. COL."
cmbtitle_Spouse.AddItem "LT."
cmbtitle_Spouse.AddItem "Wg.Cdr."
cmbtitle_Spouse.AddItem "Maj. Gen."
cmbtitle_Spouse.AddItem "Master"
cmbtitle_Spouse.AddItem "Others"




If sql_ora = "SQL" Then
    st = "select distinct STATE_ID,STATE_NAME from state_master where state_id is not null and del_flag is null order by state_name"
Else
    st = "select distinct state_id,state_name from state_master where state_id is not null and del_flag is null order by state_name"
End If
Set RSV = MyConn.Execute(st)
cmbstate.Clear
If Not RSV.EOF Then
    Do While Not RSV.EOF
        cmbstate.AddItem RSV(1) & Space(100) & "#" & RSV(0)
        cmbstate_P.AddItem RSV(1) & Space(100) & "#" & RSV(0)
        RSV.MoveNext
    Loop
End If
RSV.Close

If sql_ora = "SQL" Then
    st = "select distinct COUNTRY_ID,trim(COUNTRY_NAME)COUNTRY_NAME from COUNTRY_master where COUNTRY_id is not null order by COUNTRY_name"
Else
    st = "select distinct COUNTRY_ID,trim(COUNTRY_NAME)COUNTRY_NAME from COUNTRY_master where COUNTRY_id is not null order by COUNTRY_name"
End If
Set RSV = MyConn.Execute(st)
CmbCountry.Clear
If Not RSV.EOF Then
    Do While Not RSV.EOF
        CmbCountry.AddItem RSV(1) & Space(100) & "#" & RSV(0)
        CmbCountry_P.AddItem RSV(1) & Space(100) & "#" & RSV(0)
        RSV.MoveNext
    Loop
End If
RSV.Close


 Set rsAmc = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
 cmbBankName.Clear
 While Not rsAmc.EOF
   cmbBankName.AddItem rsAmc(0)
   rsAmc.MoveNext
 Wend
 rsAmc.Close

If sql_ora = "SQL" Then
    st1 = "select distinct city_ID,CITY_NAME from city_master where (city_id is not null or city_id<>'') order by city_name"
Else
    st1 = "select distinct city_id,city_name from city_master where city_id is not null order by city_name"
End If
Set RSV1 = MyConn.Execute(st1)
If Not RSV1.EOF Then
    Do While Not RSV1.EOF
        cmbcity.AddItem RSV1(1) & Space(100) & "#" & RSV1(0)
        cmbcity_P.AddItem RSV1(1) & Space(100) & "#" & RSV1(0)
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
'---------------------------------Account category opened-----------------------------------------
Set RSV = MyConn.Execute("select * from FIXEDITEM where itemtype=38 ORDER BY ITEMid")
cmbactcat.Clear
While Not RSV.EOF
    cmbactcat.AddItem RSV("ITEMNAME") & Space(50) & "#" & RSV("ITEMSERIALNUMBER")
    RSV.MoveNext
Wend
RSV.Close
'-------------------------------------------------------------------------------------------------
Set RSV = MyConn.Execute("select * from FIXEDITEM where itemtype=14 ORDER BY ITEMNAME")
cmbcomminterval.Clear
While Not RSV.EOF
    cmbcomminterval.AddItem RSV("ITEMNAME")
    cmbcomminterval.Tag = RSV("ITEMSERIALNUMBER")
    RSV.MoveNext
Wend
RSV.Close
sql = " select DISTINCT a.iss_code mut_code,b.osch_code sch_code,B.OSCH_NAME SCH_NAME ,C.fiGure "
sql = sql & "from iss_master a, other_product b, upfront_recd c "
sql = sql & "where a.iss_code=b.iss_code and b.osch_code=c.sch_code and A.iss_code in ('MFIS02335','MFIS02336','MFIS02342') "
RSV.open (sql), MyConn, adOpenForwardOnly
While Not RSV.EOF
    cmbplantype.AddItem RSV(2) & Space(50) & "=" & RSV(1) & Space(25) & "=" & RSV(0) & Space(25) & "=" & RSV(3)
    RSV.MoveNext
Wend
RSV.Close
Set RSV = Nothing
Call SetGrid
cmbstatus1.AddItem "SENT FOR MERGING", 0
cmbstatus1.AddItem "REJECTED", 1
cmbstatus1.AddItem "APPROVED", 2
If Glbloginid = 101877 Or Glbloginid = 97120 Or Glbloginid = 97117 Or Glbloginid = 101877 Or Glbloginid = 95584 Then
    CmdStatus.Visible = True
Else
    CmdStatus.Visible = False
End If
If Glbloginid = 97117 Then
   ChkUpdateFaimily.Visible = True
Else
   ChkUpdateFaimily.Visible = False
End If
End Sub

Private Sub FillOccupation(cmb As ComboBox)
    On Error GoTo BOError
    Dim Get_occ As New Adodb.Recordset
    Set Get_occ = MyConn.Execute("select * from occupation_master  WHERE OCC_ID IS NOT NULL AND OCC_ID NOT IN(35,33,34,31,32) order by occ_name")
   If Not Get_occ.EOF Then
        cmb.Clear
        cmb.AddItem ""
        Do While Not Get_occ.EOF
            cmb.AddItem Get_occ("occ_name") & Space(40) & "#" & Get_occ("occ_id")
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
Dim P_RS As New Adodb.Recordset
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
    P_RS.open "select * from client_test where 1<>1", MyConn, adOpenStatic, adLockOptimistic
    P_RS.AddNew
    P_RS!clientcode_old = Val(txtclientcodeold.Text)
    P_RS!tradingid = txttradingid.Text
    P_RS!benefi_id = txtdp_bid.Text
    P_RS!status = cmbstatus.Text
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
    P_RS!dob = Format(dob.Text, "DD-MMM-YYYY")
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
    P_RS!fax = Val(TxtFax.Text)
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
    P_RS!Loggeduserid = Glbloginid
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
'frmpanmf.Show
txtpan.Text = ""
cd1.ShowOpen
'txtpan.Text = cd1.Filename
imgpan.Picture = LoadPicture(cd1.Filename)
End Sub
Private Sub lblpang_Click()
'frmpanmf.Show
'cd1.ShowOpen
'txtpan.Text = cd1.Filename
'imgpan.Picture = LoadPicture(cd1.Filename)
Call lblpana_Click
End Sub
'Private Sub optNo_Click()
'If optno.Value = True Then
'    txtclientcodeold.Enabled = False
'    MyExistRelFlag = 0
'End If
'End Sub
'Private Sub optYes_Click()
'If optyes.Value = True Then
'    txtclientcodeold.Enabled = True
'    MyExistRelFlag = 1
'End If
'End Sub
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
    frmtree_searchnew.cmbbranch.SetFocus
    frmtree_searchnew.ZOrder
End Sub




Private Sub Picture2_Click()
treeselected = "N"
Set frmtreeMf.currentForm = Nothing
Set frmtreeMf.currentForm = frmactopen
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
    frmtree_searchnew.cmbbranch.SetFocus
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
'If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
'    KeyAscii = 0
'End If
    If cmbclientn.Text = "NRI" Then
        txtMobile.MaxLength = 20
    Else
        txtMobile.MaxLength = 10
    End If
End Sub

Private Sub txtoverseasadd_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtpans_KeyPress(KeyAscii As Integer)
KeyAscii = Asc(UCase(Chr(KeyAscii)))
End Sub


Private Sub txtPin_KeyPress(KeyAscii As Integer)
    If cmbclientn.Text <> "NRI" Then
        If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
            KeyAscii = 0
        End If
        txtPin.MaxLength = 6
    Else
        txtPin.MaxLength = 20
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
Private Sub VSFCommGrd_LeaveCell()
If VSFCommGrd.Col = 2 And Len(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col)) > 10 Then
    VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col) = ""
    MsgBox "Please Enter Valid Pan No"
    Exit Sub
End If
If VSFCommGrd.Col = 2 And ValidatePan(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col)) = False Then
   VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col) = ""
   MsgBox "Please Enter Valid Pan No"
   Exit Sub
End If
If VSFCommGrd.Col = 7 And Len(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col)) > 10 Then
    VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col) = ""
    MsgBox "Please Enter Valid Pan No"
    Exit Sub
End If
If VSFCommGrd.Col = 7 And ValidatePan(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col)) = False Then
   VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col) = ""
   MsgBox "Please Enter Valid Pan No"
   Exit Sub
End If
If VSFCommGrd.Col = 3 And IsDate(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col)) = False Then
   MsgBox "Enter Valid Date"
   VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col) = ""
   Exit Sub
End If
If VSFCommGrd.Col = 3 And Format(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col), "DD/MM/YYYY") > Format(ServerDateTime, "DD/MM/YYYY") Then
   If DateDiff("d", Format(VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col), "DD/MM/YYYY"), ServerDateTime) < 0 Then
        MsgBox "Date can Not Be Greater Than Current date"
        VSFCommGrd.TextMatrix(VSFCommGrd.Row, VSFCommGrd.Col) = ""
   End If
   Exit Sub
End If
End Sub
Private Sub SetGrid()
Dim StrRel As String
Dim StrOcc As String
Dim Strtit As String
s = " S.No.|^Investor Name|^ PAN |^ DOB|^ Relation|^ Occupation |^ Guardian Name|^Guardian PAN|^Kyc "
VSFCommGrd.FormatString = s
VSFCommGrd.Cols = 9
VSFCommGrd.ColWidth(0) = 0
VSFCommGrd.ColWidth(1) = 2000
VSFCommGrd.ColWidth(2) = 1200
VSFCommGrd.ColWidth(3) = 1000
VSFCommGrd.ColWidth(4) = 1000
VSFCommGrd.ColWidth(5) = 2000
VSFCommGrd.ColWidth(6) = 2000
VSFCommGrd.ColWidth(7) = 1500
VSFCommGrd.ColWidth(8) = 500
VSFCommGrd.ColEditMask(3) = "##/##/####"
VSFCommGrd.ColComboList(4) = StrRel
VSFCommGrd.ColComboList(5) = StrOcc

VSFCommGrd.ColAlignment(0) = flexAlignLeftCenter
VSFCommGrd.ColAlignment(1) = flexAlignLeftCenter
VSFCommGrd.ColAlignment(2) = flexAlignLeftCenter
VSFCommGrd.ColAlignment(3) = flexAlignLeftCenter
VSFCommGrd.ColAlignment(4) = flexAlignLeftCenter
VSFCommGrd.ColAlignment(5) = flexAlignLeftCenter
StrInv = ""
If Len(txtclientcodeold.Text) >= 11 Then
    If rs_inv.State = 1 Then rs_inv.Close
    rs_inv.open "select investor_name,inv_code from investor_master where source_id = '" & Left(txtclientcodeold, 8) & "' AND INV_CODE<>'" & txtclientcodeold.Text & "' order by inv_code", MyConn, adOpenForwardOnly
    If Not rs_inv.EOF Then
       Do While Not rs_inv.EOF
           StrInv = StrInv & rs_inv!INVESTOR_name & Space(50) & "#" & rs_inv!inv_Code & "|"
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
i = 1
VSFCommGrdE.Clear

    
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

 cmbYesNo = "YES|NO"
 cmbMaleFemale = "Male|Female|Non-Ind"
 
 s1 = "Existing Investor|^InvestorCode|^Investor Name|^Mobile|^Email|^PAN|^DOB|^Relation|^RelationId|^Occupation|^OccId|^KYC|^Gaurdian Name|^Gaurdian PAN|^G KYC|^Approved|^Client_Code|^Title|^Aadhar Card No|^Gender|^IsNominee|^Allocation%"
 VSFCommGrdE.FormatString = s1
 VSFCommGrdE.ColWidth(0) = 2500 'ExistingInvestor
 VSFCommGrdE.ColWidth(1) = 0 'InvCode
 VSFCommGrdE.ColWidth(2) = 2000 'InvName
 VSFCommGrdE.ColWidth(3) = 1500 'Mobile
 VSFCommGrdE.ColWidth(4) = 3500 'Email
 VSFCommGrdE.ColWidth(5) = 1500 'Pan
 VSFCommGrdE.ColWidth(6) = 1500 'DOB
 VSFCommGrdE.ColWidth(7) = 1500 'Relation
 VSFCommGrdE.ColWidth(8) = 0 'Relationid
 VSFCommGrdE.ColWidth(9) = 2000 'Occupation
 VSFCommGrdE.ColWidth(10) = 0 'Occupationid
 VSFCommGrdE.ColWidth(11) = 800 'kyc
 VSFCommGrdE.ColWidth(12) = 2000 'GNAMEME
 VSFCommGrdE.ColWidth(13) = 2000 'GPAN
 VSFCommGrdE.ColWidth(14) = 2000 'KYCFLAG
 VSFCommGrdE.ColWidth(14) = 0
 VSFCommGrdE.ColWidth(15) = 1500 'Approved
 VSFCommGrdE.ColWidth(16) = 0 'Client_Code
 VSFCommGrdE.ColWidth(18) = 2000 'Adhar Card No
 VSFCommGrdE.ColWidth(19) = 1200 'Gender
 VSFCommGrdE.ColWidth(20) = 1300 'IsNominee
 VSFCommGrdE.ColWidth(21) = 1600 'Allocation%
 VSFCommGrdE.ColEditMask(6) = "##/##/####"
 VSFCommGrdE.ColComboList(0) = ""
 VSFCommGrdE.ColComboList(0) = StrInv
 VSFCommGrdE.ColComboList(7) = ""
 VSFCommGrdE.ColComboList(7) = StrRel
 VSFCommGrdE.ColComboList(9) = ""
 VSFCommGrdE.ColComboList(9) = StrOcc
 VSFCommGrdE.ColComboList(14) = ""
 VSFCommGrdE.ColComboList(14) = cmbYesNo
 VSFCommGrdE.ColComboList(15) = cmbYesNo
 VSFCommGrdE.ColComboList(11) = cmbYesNo
 VSFCommGrdE.ColComboList(17) = Strtit
 VSFCommGrdE.ColComboList(19) = cmbMaleFemale
 VSFCommGrdE.ColComboList(20) = cmbYesNo
 
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
 VSFCommGrdE.ColAlignment(12) = flexAlignLeftCenter  'expe
 VSFCommGrdE.ColAlignment(13) = flexAlignLeftCenter  'expe
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

Private Sub VSFCommGrdE_Click()
On Error Resume Next
If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 7) <> "" Then
    Temp_Id = Split(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 7), "#")
End If
If Temp_Id(1) <> 29 And Temp_Id(1) <> 30 And Temp_Id(1) <> 31 And Temp_Id(1) <> 32 Then
    If GlbroleId = 13 Or GlbroleId = 78 Or GlbroleId = 76 Then
    Else
        If (VSFCommGrdE.Col = 0 Or VSFCommGrdE.Col = 5 Or VSFCommGrdE.Col = 6 Or VSFCommGrdE.Col = 7 Or VSFCommGrdE.Col = 9) Then
            If Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 5)) = "" And VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 6) <> "" Then
                'Check If He/She Is Minor
                If DateDiff("M", Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 6), "DD/MM/YYYY"), ServerDateTime) <= 216 Then
                    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 12) <> "" And ValidatePan(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 13)) = False Then
'                        MsgBox "Please Enter Valid Pan No Of The Guardian ", vbInformation, strBajaj
'
'                        Exit Sub
                    ElseIf VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 12) = "" Then
                        MsgBox "Plese Enter Guardian Name ", vbInformation, strBajaj
                        
                        Exit Sub
                    ElseIf ValidatePan(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 13)) = False Then
'                        MsgBox "Please Enter Valid Pan No Of The Guardian ", vbInformation, strBajaj
'
'                        Exit Sub
                    End If
                End If
            Else '----------he enters any pan no.----------------
                If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 6) <> "" Then
                    If DateDiff("M", Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 6), "DD/MM/YYYY"), ServerDateTime) <= 216 Then
                        MsgBox "Enter Pan No Of Guardian Only", vbInformation, "Wealthmaker"
                        VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 5) = ""
                        
                        Exit Sub
                    End If
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
    If UCase(VSFCommGrdE.TextMatrix(i, 2)) = "" Or UCase(VSFCommGrdE.TextMatrix(i, 6)) = "" Or UCase(VSFCommGrdE.TextMatrix(i, 7)) = "" Or UCase(VSFCommGrdE.TextMatrix(i, 9)) = "" Then
         ValMan = False
    End If
End If
ValidatePreviousvalue = ValMan
End Function

Private Sub VSFCommGrdE_GotFocus()
    flag = True
End Sub

Private Sub VSFCommGrdE_LeaveCell()
On Error Resume Next
If VSFCommGrdE.Col <> 4 Then
    VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col) = UCase(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col))
Else
    VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col) = LCase(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col))
End If

If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 6) <> "" Then
    If VSFCommGrdE.Col = 6 And IsDate(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col)) = False Then
       MsgBox "Please Enter Valid Date"
       VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col) = ""
       Exit Sub
    End If
    If IsDate(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col)) = True Then
        If VSFCommGrdE.Col = 6 And CDate(Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col), "DD/MM/YYYY")) > CDate(Format(ServerDateTime, "DD/MM/YYYY")) Then
           If DateDiff("d", Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col), "DD/MM/YYYY"), ServerDateTime) < 0 Then
                MsgBox "Date can Not Be Greater Than Current date"
                VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col) = ""
           End If
           Exit Sub
        End If
        If VSFCommGrdE.Col = 6 And CDate(Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col), "DD/MM/YYYY")) > CDate(Format(ServerDateTime, "DD/MM/YYYY")) Then
           If DateDiff("YYYY", Format(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col), "DD/MM/YYYY"), ServerDateTime) < 18 Then
                MsgBox "Date can Not Be Greater Than Current date"
                VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, VSFCommGrdE.Col) = ""
           End If
           Exit Sub
        End If
   End If
End If
If VSFCommGrdE.Col = 0 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 0) <> "" Then
        invCode = Split(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 0), "#")
        VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 2) = Trim(invCode(0))
        VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 1) = Trim(invCode(1))
    End If
End If

If VSFCommGrdE.Col = 3 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3) <> "" Then
        If Len(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3)) <> 10 Then
            MsgBox "Mobile No Should Be 10 Character", vbInformation
            VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3) = ""
            Exit Sub
        End If
        
        If Mid(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3), 1, 1) <> "9" And Mid(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3), 1, 1) <> "8" And Mid(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3), 1, 1) <> "6" And Mid(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3), 1, 1) <> "7" And Mid(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3), 1, 1) <> "6" Then
            MsgBox "Mobile No Should Be Start 6 or 7 or 8 or 9", vbInformation
            VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 3) = ""
            Exit Sub
        End If
    End If
End If
'------------------------------------------------addhar card validation of investor by pankaj on dated 29022016
If VSFCommGrdE.Col = 18 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 18) <> "" Then
        If Len(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 18)) <> 12 Then
            MsgBox "Aadhar Card No should be 12 digit", vbInformation
            VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 18) = ""
            Exit Sub
        End If
        If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 18) = "000000000000" Then
            MsgBox "Please Enter a Valid Aadhar Card No", vbInformation
            VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 18) = ""
            Exit Sub
        End If
    End If
End If


If VSFCommGrdE.Col = 21 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 20) = "NO" Then
        VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 21) = ""
    End If
End If

If UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "NOT AVAILABLE" And UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "N/A" And UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "N A" And UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "NILL" And UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "NONE" And UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "N-A" And UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "NA" Then
    If VSFCommGrdE.Col = 4 Then
        If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4) <> "" Then
            If UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) <> "" Then
                If SqlRet("select count(validate_email('" & UCase(Trim(VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 4))) & "')) from dual") = 0 Then
                    MsgBox "Invalid Email " & Trim(VSFCommGrdE.TextMatrix(i, 4))
                    Exit Sub
                End If
            End If
        End If
    End If
End If
If VSFCommGrdE.Col = 19 Then
    If VSFCommGrdE.TextMatrix(VSFCommGrdE.Row, 19) = "" Then
        MsgBox "Gender Can Not Be Left Blank"
        Exit Sub
    End If
End If
End Sub
Private Sub VSFCommGrdE_LostFocus()
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
Dim rs_CLCategory As New Adodb.Recordset
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
Private Sub SetGrid1()
 Dim StrModel As String
 Dim StrMake As String
 Dim StrCompany As String
 StrModel = ""
 StrMake = ""
 Dim Rs_Model As New Adodb.Recordset
 Dim Rs_Make As New Adodb.Recordset
 Dim rs_Comp As New Adodb.Recordset
 If Rs_Model.State = 1 Then Rs_Model.Close
 Rs_Model.open "select model_id, model_name from bajaj_model_master", MyConn, adOpenForwardOnly
 If Not Rs_Model.EOF Then
   Do While Not Rs_Model.EOF
       StrModel = StrModel & Rs_Model!model_name & Space(50) & "#" & Rs_Model!Model_id & "|"
       Rs_Model.MoveNext
   Loop
 End If
 Rs_Model.Close
 
 '--------------------------Make---------------------------------------------------
If Rs_Make.State = 1 Then Rs_Make.Close
Rs_Make.open "select manuf_name,manuf_id from bajaj_mnf_master", MyConn, adOpenForwardOnly
If Not Rs_Make.EOF Then
   Do While Not Rs_Make.EOF
       StrMake = StrMake & Rs_Make!manuf_name & Space(50) & "#" & Rs_Make!manuf_id & "|"
       Rs_Make.MoveNext
   Loop
 End If
 Rs_Make.Close
 '---------------------------------------------------------------------------------
 
 StrCompany = ""
 If rs_Comp.State = 1 Then rs_Comp.Close
 rs_Comp.open "select Company_Cd,Company_Name from bajaj_Company_master", MyConn, adOpenForwardOnly
 If Not rs_Comp.EOF Then
    Do While Not rs_Comp.EOF
        StrCompany = StrCompany & rs_Comp!company_name & Space(50) & "#" & rs_Comp!COMPANY_CD & "|"
        rs_Comp.MoveNext
    Loop
 End If
 rs_Comp.Close
 
 VSFCommGrdK.Clear
 VSFCommGrdK.Cols = 7
 '     0        1          2          3          4          5           6     7
 s = "Make|^ModelName|^Manuf.Year|^DueDate|^InsCompany|^CompanyCd|^Remarks|^Upd"    'RenewalYear3
 VSFCommGrdK.FormatString = s
 VSFCommGrdK.ColWidth(0) = 1500 'Car Model Id
 VSFCommGrdK.ColWidth(1) = 2000 'Car Model Name
 VSFCommGrdK.ColWidth(2) = 1000 'Manifacture Year
 VSFCommGrdK.ColWidth(3) = 1000 'Renewal Month
 VSFCommGrdK.ColWidth(4) = 3000  'Ins Company
 VSFCommGrdK.ColWidth(5) = 0  'Company Cd
 VSFCommGrdK.ColWidth(6) = 2000  'Remarks
 VSFCommGrdK.ColWidth(7) = 0  'Upd
 VSFCommGrdK.ColEditMask(2) = "##/####"
 VSFCommGrdK.ColEditMask(3) = "##/##/####"
 VSFCommGrdK.ColComboList(0) = StrMake
 VSFCommGrdK.ColComboList(1) = StrModel
 VSFCommGrdK.ColComboList(4) = StrCompany
 VSFCommGrdK.ColAlignment(0) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(1) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(2) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(3) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(4) = flexAlignLeftCenter
 VSFCommGrdK.ColAlignment(5) = flexAlignLeftCenter   'chq
End Sub


Private Sub VSFCommGrdK_Click()
'If VSFCommGrdK.TextMatrix(Row, 3) <> "" Then
'    MyModelid = VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 3)
'End If
End Sub

Private Sub VSFCommGrdK_ComboDropDown(ByVal Row As Long, ByVal Col As Long)
If VSFCommGrdK.TextMatrix(Row, 7) <> "" Then
   VSFCommGrdK.TextMatrix(Row, 7) = "UPD"
End If
End Sub

Private Sub VSFCommGrdK_LeaveCell()
Dim invCode() As String
Dim Rs_Model As New Adodb.Recordset
Dim StrModel As String
'If VSFCommGrdK.Col = 1 Then
'    If VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 1) <> "" Then
'        invCode = Split(VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 1), "#")
'        VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 3) = Trim(invCode(1))
'    End If
'End If
If VSFCommGrdK.Col = 4 Then
    If VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 4) <> "" Then
        invCode = Split(VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 4), "#")
        VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 5) = Trim(invCode(1))
    End If
End If
If VSFCommGrdK.Col = 0 And VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 0) <> "" Then
    If VSFCommGrdK.TextMatrix(Row, 0) <> "" Then
        MyMake = Split(VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 0), "#")
        Rs_Model.open "select model_id, model_name from bajaj_model_master where manuf_id='" & MyMake(1) & "'", MyConn, adOpenForwardOnly
        If Not Rs_Model.EOF Then
          Do While Not Rs_Model.EOF
              StrModel = StrModel & Rs_Model!model_name & Space(50) & "#" & Rs_Model!Model_id & "|"
              Rs_Model.MoveNext
          Loop
        End If
        VSFCommGrdK.ColComboList(1) = StrModel
     Rs_Model.Close
    End If
End If
End Sub

Private Sub VSFCommGrdK_LostFocus()
If VSFCommGrdK.Col <> 4 Then
    Call VSFCommGrdK_LeaveCell
End If
End Sub





