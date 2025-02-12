thie is complete fuciotn of save "Private Sub CmdSave_Click()
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

CmdSave.Enabled = False
'*******************FOR BROK RECD_PAID -----------------------------------
Dim recd_paid As New ADODB.Command
Set recd_paid.ActiveConnection = MyConn
recd_paid.CommandType = adCmdStoredProc
recd_paid.CommandText = "Recd_paid_update"
recd_paid.Parameters.Append recd_paid.CreateParameter("tr_code", adVarChar, adParamInput, 50, MyTranCode)
recd_paid.Execute
'''''*********************************end------------------------------------
MsgBox "Transaction  " & MyTranCode & "  Has Been Saved Sucessfully"
Call CMDRESET_Click
Exit Sub
err:
MsgBox err.Description
Resume
End Sub " so make a compelte procedure