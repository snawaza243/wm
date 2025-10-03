Private Sub dtChqDate_Validate(Cancel As Boolean)
    If dtChqDate.Text <> "__/__/____" Then
        If Not (IsDate(dtChqDate.Text)) Then
            MsgBox "Please enter Valid Date", vbCritical
            dtChqDate.SetFocus
            Cancel = True
        End If
    End If
End Sub



