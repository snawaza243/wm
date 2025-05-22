Private Sub CmdSave_Click(Index As Integer)
Dim Busi_Branch_cd As String
Dim Busi_Rm_Cd As String
Dim ClientBranchCode As String
Dim ClientRmCode As String
Dim br_cd() As String
Dim paymode As String
Dim rsGet As New ADODB.Recordset
Dim invCode As String
Dim rsTran As New ADODB.Recordset
Dim dipo_type As String * 1
Dim Pay_type As String * 1
Dim str_test As String
Dim MyTranCode As String
Dim MyGSTNO As String
Dim MyTranCode1 As String
Dim Rs_Rect As ADODB.Recordset
Dim MySecReq As String
Dim Dup_RectNo As Variant
Dim Vclientcategory As String
Dim i As Integer
Dim Xmin As Variant
Dim Xmax As Variant
If cmbproduct.Text = "" Then
   Exit Sub
End If
Dim j As Variant
Dim X As Variant
Dim Y As Variant
Dim M As Integer
Dim Z As Variant

'If Index = 0 Then
'    If GlbroleId = "212" Or GlbroleId = "1" Then
'    Else
'        MsgBox "Only Punching Team can punch the transaction.", vbInformation
'        Exit Sub
'    End If
'ElseIf Index = 4 Then
'    If GlbroleId = "146" Or GlbroleId = "1" Then
'    Else
'        MsgBox "Only NPS Team can modify the transaction.", vbInformation
'        Exit Sub
'    End If
'End If

If txtdocID.Text = "" Then
    MsgBox "DT No can not be left blank", vbInformation
    Exit Sub
End If

'FATCA VALIDATION
If Index = 0 Then
    If OptCorporate.Value = True Then
        If txtcorporatename.Text = "" Then
            MsgBox "Corporate name cannot be left blank.", vbInformation
            txtcorporatename.SetFocus
            Exit Sub
        End If
    End If
    
    If chkUnfreeze.Value = 0 Then
        If txtPRAN.Text <> " " Then
            If SqlRet("SELECT COUNT(*) FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO='" & txtPRAN.Text & "'") >= 1 Then
                 MsgBox "FATCA for this PRAN is non compliant.Please contact product team for the same", vbInformation
                 Exit Sub
            End If
        End If
    Else
        MyConn.Execute "delete from NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO=trim('" & txtPRAN.Text & "')"
    End If
End If

'VINIT 05-DEC-2015
If Index = 0 Or Index = 4 Then
    Vclientcategory = SqlRet("select category_id from client_master where client_code='" & Mid(txtINV_CD.Text, 1, 8) & "' ")
    If Vclientcategory <> "4004" Then
        If lbtrancode.Caption = "0" Then
            rsGet.open "select count(*) from ( select TRAN_CODE from transaction_st where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) )", MyConn, adOpenForwardOnly
            If rsGet(0) > 0 Then
                MsgBox "Duplicate Cheque Number !", vbInformation
                Exit Sub
            End If
            rsGet.Close
         Else
            If ReqCode = "11" Then
                rsGet.open "select count(*) from ( select TRAN_CODE from transaction_st where TRAN_CODE <> " & lbtrancode.Caption & " and  mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) AND REF_TRAN_CODE IS NULL Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) AND TRAN_CODE <> " & lbtrancode.Caption & ")", MyConn, adOpenForwardOnly
                If rsGet(0) > 0 Then
                    MsgBox "Duplicate Cheque Number !", vbInformation
                    Exit Sub
                End If
                rsGet.Close
            Else
                rsGet.open "select count(*) from ( select TRAN_CODE from transaction_st where TRAN_CODE <> " & lbtrancode.Caption & " and  mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) AND TRAN_CODE <> " & lbtrancode.Caption & ")", MyConn, adOpenForwardOnly
                If rsGet(0) > 0 Then
                    MsgBox "Duplicate Cheque Number !", vbInformation
                    Exit Sub
                End If
                rsGet.Close
            End If
        End If
    End If
End If
'-----------------
    
If Index <> 3 And Index <> 1 Then
    If txtregistrationno.Text = "" Then
       MsgBox "Please Select NSDL Branch First", vbInformation
       Exit Sub
    End If
End If
If cboRequestType.Text <> "" Then
    Req = Split(cboRequestType.Text, "#")
    ReqCode = Req(1)
End If

Busi_Branch_cd = ""
If cmbBusiBranch.Text <> "" Then
    br_cd = Split(cmbBusiBranch.Text, "#")
    Busi_Branch_cd = br_cd(1)
End If
Busi_Rm_Cd = SqlRet("select payroll_id from employee_master where payroll_id='" & txtrmbusicode & "'")
If Index = 1 Then
    Unload Me
    Exit Sub
End If
If txtINV_CD.Text <> "" Then
    If rsGet.State = 1 Then rsGet.Close
    rsGet.open "select rm_code,branch_code from investor_master where inv_code='" & txtINV_CD.Text & "'", MyConn, adOpenKeyset
    If Not rsGet.EOF Then
       ClientBranchCode = rsGet.Fields("branch_code")
       ClientRmCode = rsGet.Fields("rm_code")
    End If
End If
If rsGet.State = 1 Then rsGet.Close
If Index = 3 Then
    Call Clear
End If
If Index = 4 Then   ''Modification
    If OptCorporate.Value = True Then
        If txtcorporatename.Text = "" Then
            MsgBox "Corporate name cannot be left blank.", vbInformation
            txtcorporatename.SetFocus
            Exit Sub
        End If
    End If
    If lbtrancode.Caption = "0" Then
        MsgBox "Please Select a Transaction to Modify", vbInformation
        Exit Sub
    End If
    If chkSaveValidation(False, True) = False Then
        Exit Sub
    End If
    Busi_Branch_cd = ""
    br_cd = Split(cmbBusiBranch.Text, "#")
    Busi_Branch_cd = br_cd(1)
    paymode = ""
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
    ElseIf OptCorNECS.Value = True Then
        paymode = "M"
    End If
    rsTran.open "select * from transaction_st where tran_code='" & lbtrancode.Caption & "'", MyConn, adOpenDynamic, adLockPessimistic
    MyTranCode = rsTran.Fields("Tran_code")
    rsTran("tr_date") = DtDate.Value
    rsTran("client_Code") = Trim(txtINV_CD.Text)
    rsTran("source_code") = Left(Trim(txtINV_CD.Text), 8)
    rsTran("BUSI_BRANCH_CODE") = Busi_Branch_cd
    rsTran("BUSINESS_RMCODE") = Busi_Rm_Cd
    rsTran("mut_code") = MutCode
    rsTran("sch_code") = SCHCODE
    rsTran("amount") = Trim(txtAmountInvest.Text)
    rsTran("folio_no") = Trim(txtregistrationno.Text)
    rsTran("app_no") = ReqCode
    rsTran("PAYMENT_MODE") = paymode
    If paymode <> "M" Then
        rsTran("CHEQUE_DATE") = dtChqDate.Text
        rsTran("cheque_no") = Trim(txtChqNo.Text)
        rsTran("BANK_NAME") = cmbBankName.Text
    End If
    rsTran("manual_arno") = Trim(txtPRAN.Text)
    rsTran("corporate_name") = Trim(txtcorporatename.Text)
    rsTran("unique_id") = txtrectno.Text
    rsTran("MODIFY_USER") = Glbloginid
    rsTran("MODIFY_DATE") = Format(ServerDateTime, "dd/mm/yyyy")
    rsTran.Update
    rsTran.Close
    
    MyConn.Execute "update nps_transaction set amount1=" & Val(TxtTire1.Text) & ",amount2=" & Val(TxtTire2.Text) & ",REG_CHARGE=" & Val(txtpopregistration1.Text) & ",Tran_CHARGE=" & Val(txtpopregistration2.Text) & ",SERVICETAX=" & Val(txtServiceAmount.Text) & ",remark='" & TxtRemark.Text & "' where tran_code='" & Trim(lbtrancode.Caption) & "'"
    MsgBox "Transaction Updated Sucessfully", vbInformation
End If 
If Index = 0 Then   ''save
    If chkSaveValidation(True, False) = False Then
        Exit Sub
    End If
        
    If Vclientcategory <> "4004" Then
        rsGet.open "select count(*) from transaction_st where cheque_no='" & Trim(txtChqNo.Text) & "' and trim(bank_name)='" & Trim(cmbBankName.Text) & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')", MyConn, adOpenForwardOnly
        If rsGet(0) > 0 Then
            MsgBox "Duplicate Cheque Number !", vbInformation
            Exit Sub
        End If
        rsGet.Close
     End If
    
    rsGet.open "select count(*) from transaction_st where CLIENT_code='" & Trim(txtINV_CD.Text) & "' and sch_code='" & SCHCODE & "' and app_no = '" & ReqCode & "' and amount= '" & Trim(txtAmountInvest.Text) & "' and cheque_no='" & Trim(txtChqNo.Text) & "' And Trim(bank_name) = '" & Trim(cmbBankName.Text) & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')", MyConn, adOpenForwardOnly
    If rsGet(0) > 0 Then
        MsgBox "Duplicate Transaction !", vbInformation
        Exit Sub
    End If
    rsGet.Close
  
'---------------------------------------------------------------------------------------------------------------
    paymode = ""
    If optcheque.Value = True Then
        paymode = "C"
    ElseIf optdraft.Value = True Then
        paymode = "D"
    ElseIf optcash.Value = True Then
        paymode = "H"
    ElseIf OptCorNECS.Value = True Then
        paymode = "M"
    End If
    If optcheque.Value = True Or optdraft.Value = True Then
        str_test = " insert into transaction_sttemp"
        str_test = str_test & " (CORPORATE_NAME,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "','1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & "" & Trim(txtChqNo.Text) & ",to_date('" & dtChqDate & "','dd/MM/yyyy')"
        str_test = str_test & " ,'NPS','" & Trim(txtdocID.Text) & "')"
    Else
        str_test = ""
        str_test = " insert into transaction_sttemp"
        str_test = str_test & " (CORPORATE_NAME,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,remark,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "','1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & " 'NPS','" & Trim(txtdocID.Text) & "')"
    End If
    MyConn.Execute str_test
    
    MyTranCode = SqlRet("select max(tran_code) from temp_tran where branch_code=" & ClientBranchCode & " and substr(tran_code,1,2)='07' ")
    
    MyGSTNO = ""
    
    MyGSTNO = SqlRet("select invoice_no from transaction_sttemp where tran_code='" & MyTranCode & "'")
    
    If optcheque.Value = True Or optdraft.Value = True Then
        str_test = " insert into transaction_st"
        str_test = str_test & " (invoice_no,CORPORATE_NAME,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & MyGSTNO & "','" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "',trim('" & MyTranCode & "'),'1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD.Text & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & "" & Trim(txtChqNo.Text) & ",to_date('" & dtChqDate & "','dd/MM/yyyy')"
        str_test = str_test & " ,'NPS','" & Glbloginid & "','" & Trim(txtdocID.Text) & "')"
    Else
        str_test = " insert into transaction_st"
        str_test = str_test & " (invoice_no,CORPORATE_NAME,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,remark,LOGGEDUSERID,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & MyGSTNO & "','" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "',trim('" & MyTranCode & "'),'1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD.Text & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & " 'NPS','" & Glbloginid & "','" & Trim(txtdocID.Text) & "')"
    End If
    MyConn.Execute str_test
    str_test = "insert into nps_transaction(tran_code,amount1,amount2,reg_charge,tran_charge,SERVICETAX,remark)"
    str_test = str_test & " Values(trim('" & MyTranCode & "')," & Val(TxtTire1.Text) & "," & Val(TxtTire2.Text) & ","
    str_test = str_test & " " & Val(txtpopregistration1.Text) & "," & Val(txtpopregistration2.Text) & "," & Val(txtServiceAmount.Text) & ",'" & TxtRemark.Text & "')"
    MyConn.Execute str_test
    txtrectno.Text = SqlRet("select unique_id from transaction_st where tran_code='" & MyTranCode & "'")
    MsgBox "Your Transaction No Is " & MyTranCode & " and Your Recpt No IS " & txtrectno.Text & " "
    '--------------------------DOUBLE TRANSACTION OF CONTRIBUTION WHEN REGISTRATION----------------------
    If ReqCode = "11" And OptIndividual.Value = True Then
        sql = ""
        sql = " insert into transaction_sttemp (CORPORATE_NAME,ref_tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,doc_id) select CORPORATE_NAME,tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,0, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,'' from transaction_sttemp where tran_code='" & MyTranCode & "'"
        MyConn.Execute sql
        MyTranCode1 = SqlRet("select max(tran_code) from temp_tran where branch_code=" & ClientBranchCode & " and substr(tran_code,1,2)='07' ")
        sql = " update tb_doc_upload set ar_code='" & MyTranCode1 & "' where common_id='" & Trim(txtdocID.Text) & "'"
        MyConn.Execute sql
        sql = ""
        sql = " insert into transaction_st (ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id) select ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,12,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id from transaction_sttemp where tran_code='" & MyTranCode1 & "'"
        MyConn.Execute sql
        str_test = "insert into nps_transaction(tran_code,amount1,amount2,reg_charge,tran_charge,SERVICETAX,remark)"
        str_test = str_test & " Values(trim('" & MyTranCode1 & "'),0,0,"
        str_test = str_test & " 0,0,0,'" & TxtRemark.Text & "')"
        MyConn.Execute str_test
        MyConn.Execute ("DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE='" & MyTranCode1 & "'")
        MySecReq = SqlRet("select unique_id from transaction_st where tran_code='" & MyTranCode1 & "'")
        MsgBox "Your Duplicate Transaction No Is " & MyTranCode1 & " and Your Recpt No IS " & MySecReq & " "
    End If
    MyConn.Execute ("DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE='" & MyTranCode & "'")
    Call Clear
End If
Set rsGet = Nothing
Set rsTran = Nothing

Dim recd_paid As New ADODB.Command
Set recd_paid.ActiveConnection = MyConn
recd_paid.CommandType = adCmdStoredProc
recd_paid.CommandText = "Recd_paid_update"
recd_paid.Parameters.Append recd_paid.CreateParameter("tr_code", adVarChar, adParamInput, 50, MyTranCode)
recd_paid.Execute
            
End Sub