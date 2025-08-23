Private Sub CmdTranCancel_Click()
fracancel.ZOrder
txtcanTrcode = txtTrNo.Text
mdatecancel = Format(ServerDateTime, "DD/MM/YYYY")
If txtcanTrcode = "" Then
    MsgBox "First Search The Ar To Wich You want To Cancel", vbInformation
    Exit Sub
End If
fracancel.Visible = True
txtcanTrcode.Enabled = False
Set rsReason = MyConn.Execute("select reason,reason_type from ar_reason where upper(trim(purpose))='C' order by reason")
CmbReson.Clear
While Not rsReason.EOF
    CmbReson.AddItem rsReason(0) & Space(50) & "$" & rsReason(1)
    rsReason.MoveNext
Wend
End Sub

