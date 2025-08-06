Private Sub cmdUnconfirm_Click()
Dim cmMargin As New ADODB.Command
    'SAME BUTTON USED TO unCONFIRM BOTH PMS AND ATM TRANS.

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
        
        MyConn.Execute ("Update Transaction_mf_temp1 set remark_reco='" & Trim(TxtRemarks.Text) & "',REC_FLAG='N',RECO_DATE=null,REC_USER=null WHERE TRAN_CODE='" & MyTrCode & "'")
        MyConn.Execute ("Update Transaction_mf_temp1 set remark_reco='" & Trim(TxtRemarks.Text) & "',REC_FLAG='N',RECO_DATE=null,REC_USER=null WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
        
        
        MsgBox "Record Is Unconfirmed Sucessfully", vbInformation
        
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
        
        MyConn.Execute ("Update Transaction_mf_temp1 SET ATM_RECO_FLAG='N' WHERE TRAN_CODE='" & MyTrCode & "'")
        
        Set cmMargin.ActiveConnection = MyConn
        cmMargin.CommandType = adCmdStoredProc
        cmMargin.CommandText = "UPDATE_MF_MARGIN_TRAN"
        cmMargin.Parameters.Append cmMargin.CreateParameter("TRANCODE", adVarChar, adParamInput, 15, MyTrCode)
        cmMargin.Execute
        Set cmMargin = Nothing
        
        MsgBox "Record Is Unconfirmed Sucessfully", vbInformation
        
    End If
End Sub

