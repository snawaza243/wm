Private Sub ImSipEndDtM_Validate(Cancel As Boolean)
    If ImSipEndDtM.Text <> "__/__/____" Then
        If Not (IsDate(ImSipEndDtM.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipEndDtM.SetFocus
            Cancel = True
        End If
    End If
End Sub


