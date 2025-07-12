Public Sub CmdSave_Click()

'''''''''''''''''''''''''''''''''''''''''''''''''CROSS CHANNEL VALIDATION SUBBROKER--------------------------------------------------------
vApprovalFlag = ""
vApprovalFlag = SqlRet("select wealthmaker.fn_check_for_approval_all('" & txtdocID.Text & "')  from dual")
 
If vApprovalFlag = 2 Then
    MsgBox "Approval request for this transaction has already been raised.", vbInformation, strBajaj
    Exit Sub
End If

If vApprovalFlag = 4 Then
    MsgBox "Approval request for this transaction has been rejected by Management.", vbInformation, strBajaj
    Exit Sub
End If


If CmbSipStpA = "SIP" Then
   If ImSipEndDtA.Text = "__/__/____" Or ImSipStartDtA.Text = "__/__/____" Then
        MsgBox "Please Enter SIP End Date ", vbInformation, "Wealthmaker"
        ImSipEndDtA.SetFocus
        Exit Sub
   End If
End If

If ChkClose = 1 Then
    If TxtCloseSch.Text <> "" Then
        str1 = Split(TxtCloseSch.Text, "=")
        MyCloseSchCode = str1(1)
    End If
Else
    MyCloseSchCode = ""
End If



If Label32 <> "" And MySchCode <> "" And TxtAmountA <> "" Then
    If (SqlRet("select count(*) from transaction_mf_temp1 a,scheme_info b where   (asa <> 'C' OR asa IS NULL) and a.sch_code=b.sch_code AND A.DOC_ID IS NOT NULL  and CLIENT_CODE='" & Label32 & "' and tr_date>sysdate-90 and b.sch_code='" & MySchCode & "' and amount between " & TxtAmountA - 100 & " and " & TxtAmountA + 100)) >= 1 Then
            frmpopupduplicate.formtype = "MF"
            frmpopupduplicate.prem_amt = TxtAmountA
            frmpopupduplicate.CCODE = Label32
            frmpopupduplicate.sch_code = MySchCode
            frmpopupduplicate.Show
            frmpopupduplicate.ZOrder 0
    Else
            save_method
    End If
Else
    save_method
End If
End Sub
