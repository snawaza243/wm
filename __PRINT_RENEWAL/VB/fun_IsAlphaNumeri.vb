Public Function IsAlphanumeric(KeyAscii As Integer) As Boolean
    If Not ((KeyAscii >= 65 And KeyAscii <= 92) Or (KeyAscii >= 97 And KeyAscii <= 122) Or (Chr(KeyAscii) >= 0 And Chr(KeyAscii) <= 9)) And KeyAscii <> 8 Then
           IsAlphanumeric = False
           Exit Function
    End If
    IsAlphanumeric = True
End Function