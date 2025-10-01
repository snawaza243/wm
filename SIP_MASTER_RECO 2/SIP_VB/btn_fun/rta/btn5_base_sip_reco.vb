Private Sub CmdMap_Click()
Dim adSendMail As New ADODB.Command
Dim rsCheck As New ADODB.Recordset

If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If
If MyRtaTrCode = "" Then
    MsgBox "First Double Click The Record Of RTA Transaction,To Which Folio You Want To Map ", vbInformation
    Exit Sub
End If
'If Len(MasterId) = 0 Then
'    MsgBox "First Select The Record Of Sip MAster , To Which You Want To Map", vbInformation
'    Exit Sub
'End If
'
'-------------------------------------------------------------------------------------------------
If MyDispatch = "N" Then
    MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
Else
    MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & " ,  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
End If
'--------------------------------------------------------------------------------------------------
'MyConn.Execute ("Update Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update  Transaction_mf_temp1@test.bajajcapital set  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
'MyConn.Execute ("Update TRAN_SIP_FEED@TEST.BAJAJCAPITAL SET REC_USER='" & Glbloginid & "' ,BASE_TRAN_CODE='" & MyTrCode & "',REC_DATE=TO_DATE(SYSDATE) WHERE SEQ_NO=" & MasterId & "")

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
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg2", adVarChar, adParamInput, 50, "Base SIP Reconciled")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg3", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("Sub", adVarChar, adParamInput, 50, "Reco Update " & MyTrCode)
        
        'adSendMail.Execute
        Set adSendMail = Nothing
        '-------------------------
        rsCheck.open "select rec_flag from transaction_mf_temp1 WHERE TRAN_CODE='" & MyTrCode & "'", MyConn, adOpenForwardOnly
        MsgBox rsCheck("Rec_flag")
        rsCheck.Close
        Set rsCheck = Nothing

MsgBox "Base SIP Registration Confirmed Sucessfully", vbInformation
End Sub

