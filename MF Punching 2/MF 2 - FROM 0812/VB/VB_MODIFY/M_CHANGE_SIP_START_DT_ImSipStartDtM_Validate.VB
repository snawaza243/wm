Private Sub ImSipStartDtM_Validate(Cancel As Boolean)
    If ImSipStartDtM.Text <> "__/__/____" Then
        If Not (IsDate(ImSipStartDtM.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            ImSipStartDtM.SetFocus
            Cancel = True
        End If
    End If
End Sub


