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
            'Dim inv_cd As String
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


Set ARCODE = MyConn.Execute("select tran_code from transaction_mf_Temp1 where mut_code='" & MyAmcCode & "' and  app_no='" & TxtAppnoA.Text & "' AND (ASA<>'C' OR ASA IS NULL) ")
If ARCODE.EOF Then
Else
    MsgBox "Sorry,This App No in a Transaction Has Been Already Punched In This Company", vbInformation, "Wealthmaker"
    Exit Sub
End If
ARCODE.Close


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
sql = "select nvl(Upfront_recd_NONang_detail('" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & ",'" & cmbTranTypeA.Text & "','" & MySwitchSchCodeA & "'),0)+nvl(trail_recd_ang_detail('" & MyAmcCode & "','" & MySchCode & "',TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY')," & Val(TxtAmountA.Text) & "),0) from dual"
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
'MyBrokRecd = SqlRet("select nvl(Upfront_Ope_Recd_Temptran1_mf('" & MyPrintTranCode & "'),0) from dual")
'MyBrokPay = SqlRet("select nvl(Upfront_Ope_paid_Temptran2_mf('" & MyPrintTranCode & "'),0) from dual")
'MyMargin = (Val(MyBrokRecd) - Val(MyBrokPay)) * 11
'MyConn.Execute ("update transaction_mf_temp1 set brok_recd=" & MyBrokRecd & ",brok_payble=" & MyBrokPay & ",margin=" & MyBrokRecd & " - " & MyBrokPay & " where tran_code='" & MyPrintTranCode & "'")

If Val(Label32.Caption) <> 0 And TxtPanVarify.Text <> "" And IsMinor = False Then
    MyConn.Execute ("update investor_master set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,pan='" & TxtPanVarify.Text & "' where inv_code='" & Val(Label32.Caption) & "' and pan is null ")
    MyConn.Execute ("update client_test set MODIFY_USER='" & Glbloginid & "',MODIFY_DATE=SYSDATE,client_pan='" & TxtPanVarify.Text & "' where client_codekyc='" & Val(Label32.Caption) & "' and client_pan is null ")
End If


If cmbTranTypeA.Text = "PURCHASE" Then
    MyConn.Execute "CALL PRCINSERTWMTRANSIP('" & MyPrintTranCode & "',1)"
'    SIPADV = SqlRet("select BAJAJ_SIP_ADVANTAGE1_SIP_TEMP('" & MyPrintTranCode & "') from dual")
'    If SqlRet("SELECT BAJAJ_SIP_ADVANTAGE1_SIP_TEMP('" & MyPrintTranCode & "') FROM DUAL") > 0 Then
'        MyUniqueKey = SqlRet("select unique_id from wm_tran_sip where base_tran_code='" & MyPrintTranCode & "'")
'
'        If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then
'            SQL = "INSERT INTO transaction_mf_Temp1"
'            SQL = SQL & "( "
'            SQL = SQL & " CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
'            SQL = SQL & " APP_NO,SIP_START_DATE,Pan,FOLIO_NO,PAYMENT_MODE,BANK_NAME,CHEQUE_NO,CHEQUE_DATE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,SIP_ADV,BROK_RECD,BROK_PAYBLE,MARGIN,SIP_ID,MOVE_FLAG1,BASE_TRAN_CODE,SIP_END_DATE,sip_fr,dispatch "
'        Else
'            SQL = " INSERT INTO transaction_mf_Temp1"
'            SQL = SQL & "( "
'            SQL = SQL & " CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE, "
'            SQL = SQL & " APP_NO,SIP_START_DATE,Pan,FOLIO_NO,PAYMENT_MODE,AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,SIP_ADV,BROK_RECD,BROK_PAYBLE,MARGIN,SIP_ID,MOVE_FLAG1,BASE_TRAN_CODE,SIP_END_DATE,sip_fr,dispatch "
'        End If
'        SQL = SQL & ")VALUES(" & Val(Label32.Caption) & ",'" & TxtBusiCodeA & "','" & Glbloginid & "','" & TxtBusiCodeA & "'," & MyBranchCode & ",'" & TxtPanA & "','" & MyAmcCode & "','" & MySchCode & "', "
'        SQL = SQL & " TO_DATE('" & MyDate(ImEntryDtA) & "','MM/DD/YYYY'),'" & cmbTranTypeA.Text & "',"
'        SQL = SQL & " '" & TxtAppnoA.Text & "'"
'        If ImSipStartDtA.Text <> "__/__/____" Then
'            SQL = SQL & " ,TO_DATE('" & MyDate(ImSipStartDtA) & "','MM/DD/YYYY')"
'        Else
'            SQL = SQL & " ,null"
'        End If
'        SQL = SQL & " ,'" & TxtPanVarify.Text & "','" & TxtFolioNoA.Text & "',"
'        If optcheque.Value = True Then
'            paymode = "C"
'        ElseIf optdraft.Value = True Then
'            paymode = "D"
'        ElseIf optcash.Value = True Then
'            paymode = "H"
'        ElseIf OptEcs.Value = True Then
'            paymode = "E"
'        ElseIf OptOthers.Value = True Then
'            paymode = "R"
'        ElseIf OptRTGS.Value = True Then
'            paymode = "U"
'        ElseIf OptFT.Value = True Then
'            paymode = "B"
'        End If
'        SQL = SQL & "'" & paymode & "'"
'        If (optcheque.Value = True Or optdraft.Value = True Or OptEcs.Value = True Or OptRTGS.Value = True Or OptFT.Value = True) And OptsipF.Value = True Then  'vinod ECS 23/05/2005
'             SQL = SQL & ",'" & Trim(cmbBankName.Text) & "','" & Trim(txtChqNo.Text) & "',to_date('" & Trim(dtChqDate.Text) & "','DD-MM-YYYY'),"
'        Else
'            SQL = SQL & ","
'        End If
'        SQL = SQL & " " & Val(TxtAmountA.Text) * 11 & ""
'        SQL = SQL & ",'" & CmbSipStpA.Text & "'"
'        SQL = SQL & ",'" & lbl_lead_caption.Caption & "','" & TxtClientCodeA.Text & "','" & txtInvestorA & "'," & Val(ImnExp_Per.Text) & "," & Val(ImExpenses.Text) & ",'" & TxtAcHolderA.Text & "','" & MyFreq(1) & "','" & Trim(TxtInstallmens.Text) & "'," & Val(SIPADV) & " ," & Val(MyBrokRecd) * 11 & "," & Val(MyBrokPay) * 11 & "," & Val(MyMargin) & "," & MyUniqueKey & ",'ADVSIP','" & MyPrintTranCode & "'"
'        If ImSipEndDtA.Text <> "__/__/____" Then
'            SQL = SQL & " ,TO_DATE('" & MyDate(ImSipEndDtA) & "','MM/DD/YYYY')"
'        Else
'            SQL = SQL & " ,null"
'        End If
'        If OptsipF.Value = True Then
'            SQL = SQL & " ,'F'"
'        ElseIf OptsipF.Value = True Then
'            SQL = SQL & " ,'R'"
'        Else
'            SQL = SQL & " ,NULL"
'        End If
'        If optReg.Value = True Then
'            SQL = SQL & " ,'R'"
'        ElseIf optNFO.Value = True Then
'            SQL = SQL & " ,'N'"
'        End If
'        SQL = SQL & " )"
'        MyConn.Execute SQL
'    End If

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
'Comment by pankaj pundir on request anup ji on dated 12022016
'Call gridfill(1)
'If MsgBox("Do You Want To Print this Ar", vbYesNo + vbQuestion) = vbYes Then
'   Call cmdArPrint_Click
'End If
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


