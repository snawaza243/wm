Public Sub Change_bt_Click(Index As Integer)
If Label32.Caption = "" Then
    MsgBox "Information not present", vbInformation, strBajaj
    Exit Sub
Else
    Set frminvaddress_update.currentForm = Nothing
    Set frminvaddress_update.currentForm = frmtransactionmf
    frminvaddress_update.Label32.Caption = Label32.Caption
    frminvaddress_update.Show
    frminvaddress_update.ZOrder 0
End If
End Sub

