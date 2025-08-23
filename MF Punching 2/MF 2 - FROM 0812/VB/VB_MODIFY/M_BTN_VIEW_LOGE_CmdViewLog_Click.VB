Private Sub CmdViewLog_Click()
Dim sql As String
sql = ""
If LblTranCode.Caption = "" Then
   MsgBox "Please Select Any Transaction First", vbInformation
   Exit Sub
End If
sql = "SELECT * FROM VIEWTRANSACTION_MF_TEMP1_LOG where tran_code='" & Trim(LblTranCode.Caption) & "'"
Populate_Data sql
set_grid
SSTab1.Tab = 1
FrmTransactionLog.Visible = True
End Sub

