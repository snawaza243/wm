Private Function valid_rm(ClientBranch As String, ctl As TextBox) As Boolean
Dim rsEmp As New ADODB.Recordset
valid_rm = True
If rsEmp.State = 1 Then rsEmp.Close
Set rsEmp = Nothing
End Function

