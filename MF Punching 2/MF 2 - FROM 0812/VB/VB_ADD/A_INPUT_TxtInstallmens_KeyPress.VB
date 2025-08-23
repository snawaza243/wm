Private Sub TxtInstallmens_KeyPress(KeyAscii As Integer)
    If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
        KeyAscii = 0
    End If
    Opt99.Value = False
'    If Val(TxtInstallmens.Text) > 99 Then
'    MsgBox "Installments can't be greater then 99", vbCritical, "Wealthmaker"
'    TxtInstallmens.Text = ""
'    TxtInstallmens.SetFocus
'    Exit Sub
'End If
End Sub


