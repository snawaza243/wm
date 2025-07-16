
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
    If TxtSipAmount.Text = "" Or TxtSipAmount.Text = "0" Then
        MsgBox "Please Enter SIP Amount", vbInformation, "Wealthmaker"
        TxtSipAmount.SetFocus
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
    
    If MySchCode <> "OP#10826" Then
    
        If Left(Label32.Caption, 1) = "3" Then
                '-----------------------------CHECK IT IS MINOR OR NOT------------------------------------
                If Label32.Caption <> "" Then
                    dob = SqlRet("select NVL(dob,TO_DATE(SYSDATE)-10000) from INVESTOR_MASTER where INV_CODE=" & Label32.Caption & "")
                    If Format(dob, "dd/mm/yyyy") <> "__/__/____" And IsDate(dob) = True Then
                            If DateDiff("YYYY", Format(dob, "DD/MM/YYYY"), ServerDateTime) <= 18 Then
                                    IsMinor = True
                            Else
                                IsMinor = False
                            End If
                    End If
                End If
                '------------------------------------------------------------------------------------------
                
                If MySchCode <> "OP#10826" Then
                        If IsMinor = False Then
                        '--------------micro pan validate vinit 20 dec 2014
                            If micropanflag = 0 Then
                                 PAN1 = ""
                                 PAN1 = SqlRet("select UPPER(PAN) from investor_master where INV_CODE='" & Label32.Caption & "'")
                                 If PAN1 <> "" And PAN1 <> "0" Then
                                     If PAN1 <> UCase(TxtPanVarify.Text) Then
                                         MsgBox "PAN Entered Did Not Match With The Selected Investor's PAN ", vbCritical
                                         Exit Sub
                                         'End If
                                     End If
                                 End If
                             End If
                        Else
                        End If
                End If
        
        End If
    End If
    
    If MySchCode <> "OP#10826" Then
        If Left(Label32.Caption, 1) = "4" Then
            Dim AccountPan As Integer
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
    Else
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

sql = sql & ")VALUES(" & ChkAtmTransactionA.Value & ", " & Val(TxtSipAmount.Text) & "," & Val(Label32.Caption) & ",'" & TxtBusiCodeA & "','" & Glbloginid & "','" & TxtBusiCodeA & "'," & MyBranchCode & ",'" & TxtPanA & "','" & MyAmcCode & "','" & MySchCode & "', "
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
sql = sql & ",'" & lbl_lead_caption.Caption & "','" & TxtClientCodeA.Text & "','" & txtInvestorA & "'," & Val(ImnExp_Per.Text) & "," & Val(ImExpenses.Text) & ",'" & TxtAcHolderA.Text & "','" & MyFreq(1) & "','" & Trim(TxtInstallmens.Text) & "',SYSDATE "
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
If Val(Label32.Caption) <> 0 And TxtPanVarify.Text <> "" And IsMinor = False Then
    MyConn.Execute ("update investor_master set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,pan='" & TxtPanVarify.Text & "' where inv_code='" & Val(Label32.Caption) & "' and pan is null ")
    MyConn.Execute ("update client_test set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,client_pan='" & TxtPanVarify.Text & "' where client_codekyc='" & Val(Label32.Caption) & "' and client_pan is null ")
End If


If cmbTranTypeA.Text = "PURCHASE" Then
    MyConn.Execute "CALL PRCINSERTWMTRANSIP('" & MyPrintTranCode & "',1)"
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