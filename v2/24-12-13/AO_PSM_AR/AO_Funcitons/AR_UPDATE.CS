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
Call CMDRESET_Click
End Sub