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
 
'Duplicate Cheque Number: VINIT 05-DEC-2015

'NSDL Branch on txtregistrationno
 
'ReqCode = Req(1)

'Busi_Branch_cd = ""
 
'Busi_Rm_Cd = SqlRet("select payroll_id from employee_master where payroll_id='" & txtrmbusicode & "'")
    
'Unload Me

'ClientBranchCode
'ClientRmCode

' Clear form

'index = 4 : corp, ar on update, checkSaveValidation, update nps_transaction

 

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