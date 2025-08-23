Private Sub TxtAmountM_KeyPress(KeyAscii As Integer)
If (KeyAscii >= 48 And KeyAscii <= 57) Or KeyAscii = 46 Or KeyAscii = 8 Then
Else
   KeyAscii = 0
End If
If KeyAscii = 13 Then
  SendKeys "{TAB}"
End If
End Sub

