Private Sub CmdNfoReconcile_Click()
If MyTrCode = "" Then
    MsgBox "First Double Click The Record , You Want To Map", vbInformation
    Exit Sub
End If
If TxtRemarks.Text = "" Then
    MsgBox "Remarks ", vbInformation
    Exit Sub
End If
MyConn.Execute ("Update  Transaction_mf_temp1 set  remark_reco='" & TxtRemarks.Text & "' , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE TRAN_CODE='" & MyTrCode & "'")
MyConn.Execute ("Update  Transaction_mf_temp1 set remark_reco='" & TxtRemarks.Text & "' , REC_FLAG='Y',RECO_DATE=TO_DATE(SYSDATE),REC_USER='" & Glbloginid & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
MsgBox "Record Is Mapped Sucessfully", vbInformation
End Sub

