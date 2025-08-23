Private Sub ImEntryDtM_Validate(Cancel As Boolean)
If ImEntryDtM.Text <> "__/__/____" Then
    If CDate(ImEntryDtM.Text) > Format(ServerDateTime, "dd-mm-yyyy") Then
        MsgBox "TranDate Can Not Be Greater Date"
        Cancel = True
    End If
    If CDate(ImEntryDtM.Text) < #8/1/2009# Then
        MsgBox "Punching is not Allowed Before 31 July"
        Cancel = True
    End If
End If
End Sub
