Private Sub CmdsaveRemaek_Click()
If MyTrCode = "" Then
   MsgBox "Please double click the record,you want to remark", vbInformation
   Exit Sub
End If
MyConn.Execute ("Update Transaction_mf_temp1 set  remark_reco='" & TxtRemarks.Text & "'   WHERE TRAN_CODE='" & MyTrCode & "'")
MyConn.Execute ("Update Transaction_mf_temp1 set remark_reco='" & TxtRemarks.Text & "' WHERE BASE_TRAN_CODE='" & MyTrCode & "'")
MsgBox "The AR has been remarked sucessfully", vbInformation


End Sub



