Private Sub ImSipEndDtA_Validate(Cancel As Boolean)
    If ImSipEndDtA.Text <> "__/__/____" Then
        If Not (IsDate(ImSipEndDtA.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipEndDtA.SetFocus
            Cancel = True
        End If
    End If
End Sub


