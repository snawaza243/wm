Private Sub CmdMap_Click()
Dim MyRtaTrCode_1  As String
Dim adSendMail As New ADODB.Command
Dim MyMailId As String
Dim rsCheck As New ADODB.Recordset
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If
If MyRtaTrCode = "" Then
    MsgBox "First Double Click The Record , To Which You Want To Map", vbInformation
    Exit Sub
End If
MyRtaTrCode_1 = MyRtaTrCode
If Len(MyRtaTrCode) > 4000 Then
    MyRtaTrCode_1 = Left(MyRtaTrCode, 4000)
End If

If MyDispatch = "N" Then
    MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & " , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
    MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
Else
    If optTrail.Value = False Then
        MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "' , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'") ',tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr')
        MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "' ,  REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'") 'tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr'),
        MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
    Else
        MyConn.Execute ("Update Transaction_mf_temp1 set  amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "' ,tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr'), REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE TRAN_CODE='" & MyTrCode & "'") ',tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr')
        
        MyConn.Execute ("Update Transaction_mf_temp1 set amount=" & Val(MyRtaAmount) & ",folio_no='" & MyRtaFolio & "', REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "',RTA_TRAN_CODE='" & Replace(MyRtaTrCode_1, "'", "") & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'") 'tr_date=to_date('" & MyRtaTrDate & "', 'dd/mm/rrrr'),
        
        MyConn.Execute ("Update Transaction_st@MF.BAJAJCAPITAL set REC_FLAG='Y',HO_TRAN_CODE='" & MyTrCode & "' where tran_code in (" & MyRtaTrCode & ")")
    End If
End If
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
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg2", adVarChar, adParamInput, 50, "AR Reconciled")
        adSendMail.Parameters.Append adSendMail.CreateParameter("msg3", adVarChar, adParamInput, 50, "")
        adSendMail.Parameters.Append adSendMail.CreateParameter("Sub", adVarChar, adParamInput, 50, "Reco Update " & MyTrCode)
        
        'adSendMail.Execute
        Set adSendMail = Nothing
        '-------------------------
        rsCheck.open "select rec_flag from transaction_mf_temp1 WHERE TRAN_CODE='" & MyTrCode & "'", MyConn, adOpenForwardOnly
        MsgBox rsCheck("Rec_flag")
        rsCheck.Close
        Set rsCheck = Nothing
        
MsgBox "Record Is Mapped Sucessfully", vbInformation

End Sub

