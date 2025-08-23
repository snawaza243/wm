Private Sub TxtAppnoM_KeyPress(KeyAscii As Integer)
  If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
        KeyAscii = 0
  End If
End Sub

