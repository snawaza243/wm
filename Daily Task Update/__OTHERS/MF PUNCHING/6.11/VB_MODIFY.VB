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
        Else
           
        End If
        
        Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and  app_no='" & TxtAppnoM.Text & "' AND TRAN_CODE <> '" & MyTranCode & "' AND MOVE_FLAG1 IS NULL AND SIP_ID IS NULL AND (ASA<>'C' OR ASA IS NULL)")
        If ARCODE.EOF Then
        Else
            MsgBox "Sorry,This App No. in Transaction has Been Already Punched for this Company", vbInformation, "Wealthmaker"
            Exit Sub
        End If
        ARCODE.Close
       
    Else
        If CmbSipStpM.Text = "SIP" And OptEcsM.Value = True Then
        Else
       
        End If
        
        Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and  app_no='" & TxtAppnoM.Text & "' AND TRAN_CODE <> '" & MyTranCode & "' AND TRAN_CODE<>'" & BaseTranCode & "' AND (ASA<>'C' OR ASA IS NULL) ")
        If ARCODE.EOF Then
        Else
            MsgBox "Sorry,This App No. in  Transaction Has Been Already Punched for This Company", vbInformation, "Wealthmaker"
            Exit Sub
        End If
        ARCODE.Close
        
      
    End If
    '----------------------------------------------------------------------------------------------------
End If
MyConn.BeginTrans
If MyTranCode = "" Then
    MsgBox "Please Double Click at the record you want to update", vbInformation
Else
'---------------------------------------------Received ------------------------------------------
    MyRecdM = 0
    
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