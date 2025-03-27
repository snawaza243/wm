'********************************************************************************************
' SubBrokerOffData
' 
' Purpose:
' This subroutine handles the creation and updating of Sub Broker data in the database. 
' It performs various validations, updates existing records, or inserts new records 
' into the `agent_master` table and related tables. It also handles additional operations 
' such as auto client creation, investor creation, and AR (Account Receivable) creation.
'
' Parameters:
' None
'
' Variables:
' - rs_get_source, rs_get_source1, rsCheck, rsSubType, rsCity, rsLoc, RSCAT, rsStatus, rsEx, rsBank, rsAtype, rsPmode:
'   ADODB.Recordset objects used for database queries and updates.
' - MyBranchCode: String to store the branch code.
' - chk_no, Chkdt, chbank: Strings to store check details.
' - B(), bk(): Arrays to store parsed bank and branch details.
' - bid, bkid: Strings to store bank and branch IDs.
' - yesup: String to store user confirmation for updating existing records.
' - pass: String to store AMFI certification status.
' - ANACode_Local, pACK_Local: Strings for auto-generated client and AR codes.
' - re_local_dt: ADODB.Recordset for local data retrieval.
' - str_test: String to store SQL queries for execution.
'
' Workflow:
' 1. Validates input fields such as bank details, account type, and affected date.
' 2. Checks if the Sub Broker already exists in the database.
'    - If exists, prompts the user for update confirmation.
'    - Updates the record with new data if confirmed.
' 3. If the Sub Broker does not exist:
'    - Validates the existence of the Sub Broker code.
'    - Inserts a new record into the `agent_master` table.
' 4. Updates related fields such as address, phone, email, category, and payment details.
' 5. Handles additional operations:
'    - Auto client creation in `client_master`.
'    - Auto investor creation in `investor_master`.
'    - Auto AR creation in `transaction_sttemp`.
' 6. Executes stored procedures for receivable calculations.
' 7. Displays success messages upon completion.
'
' Notes:
' - The subroutine uses ADODB for database operations.
' - It assumes the existence of specific database tables and fields.
' - It includes legacy comments and updates by different developers.
'
' Error Handling:
' - The subroutine does not include explicit error handling. Ensure proper error handling 
'   mechanisms are implemented in the calling code or within this subroutine.
'
' Dependencies:
' - Requires a valid ADODB.Connection object (`MyConn`) for database operations.
' - Relies on specific database schema and stored procedures.
'
' Author(s):
' - Original Author: Unknown
' - Updates by: Vinay Hatwal (1/May/2007), Jawahar (specific sections)
'
' Last Updated:
' - Date: 1/May/2007
' - By: Vinay Hatwal
'
'********************************************************************************************
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