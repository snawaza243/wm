Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
'Call SaveLogIn(Glbloginid, "", Me.Name)
Call SaveLogOut(Glbloginid, Me.Name)
End Sub