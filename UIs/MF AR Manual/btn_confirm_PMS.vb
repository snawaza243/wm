Private Sub cmdConfirm_Click()
Dim cmMargin As New ADODB.Command
Dim adSendMail As New ADODB.Command
    'SAME BUTTON USED TO CONFIRM BOTH PMS AND ATM TRANS.

    If optPMS.Value = True Then
        If MyTrCode = "" Then
            MsgBox "First Double Click The Record , You Want To Map", vbInformation
            Exit Sub
        End If
        If TxtRemarks.Text = "" Then
            MsgBox "Please enter Remarks", vbInformation
            TxtRemarks.SetFocus
            Exit Sub
        End If
        
        MyConn.Execute ("Update Transaction_mf_temp1 set remark_reco='" & Trim(TxtRemarks.Text) & "',REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
        MyConn.Execute ("Update Transaction_mf_temp1 set remark_reco='" & Trim(TxtRemarks.Text) & "',REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
        
        
        MsgBox "Record Is confirmed Sucessfully", vbInformation
    ElseIf optATM.Value = True Then
        If MyTrCode = "" Then
            MsgBox "First Double Click The Record , You Want To Map", vbInformation
            Exit Sub
        End If
'        If TxtRemarks.Text = "" Then
'            MsgBox "Please enter Remarks", vbInformation
'            TxtRemarks.SetFocus
'            Exit Sub
'        End If
        
        MyConn.Execute ("Update Transaction_mf_temp1 SET ATM_RECO_FLAG='Y' WHERE TRAN_CODE='" & MyTrCode & "'")
        Set cmMargin.ActiveConnection = MyConn
        cmMargin.CommandType = adCmdStoredProc
        cmMargin.CommandText = "UPDATE_MF_MARGIN_TRAN"
        cmMargin.Parameters.Append cmMargin.CreateParameter("TRANCODE", adVarChar, adParamInput, 15, MyTrCode)
        cmMargin.Execute
        Set cmMargin = Nothing
        
        '-------temp send mail
        If Glbloginid = "39339" Then
            MyMailId = "anamikat@bajajcapital.com"
        ElseIf Glbloginid = "112649" Then
            MyMailId = "rajeshb@bajajcapital.com"
        End If

        Set adSendMail.ActiveConnection = MyConn
        adSendMail.CommandType = adCmdStoredProc
        adSendMail.CommandText = "SEND_MAIL"
        
        adSendMail.Parameters.Append adSendMail.CreateParameter("recp", adVarChar, adParamInput, 50, MyMailId)
        adSendMail.Parameters.Append adSendMail.CreateParameter("from_id", adVarChar, adParamInput, 50, "wealthmaker@bajajcapital.com")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg1", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg2", adVarChar, adParamInput, 50, "PMS/ATM Reconciled")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg3", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("Sub", adVarChar, adParamInput, 50, "Reco Update " & MyTrCode)
        
        'adSendMail.Execute
        Set adSendMail = Nothing
        '-------------------------
        
        
        
        MsgBox "Record Is confirmed Sucessfully", vbInformation
    
    End If

End Sub

