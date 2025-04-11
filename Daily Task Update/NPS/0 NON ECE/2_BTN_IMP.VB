Private Sub pic_import_Click()
'On Error GoTo BOError
Chk_OPENFORM = True
If lstsheet.ListCount = 0 Then
   MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
If lstsheet.Text <> "" Then
  For i = 0 To lstsheet.ListCount - 1
    If lstsheet.Selected(i) = True Then
      GoTo mm
    End If
  Next
  MsgBox "Please First Select Sheet", vbInformation, "Main Form"
   Exit Sub
End If
mm:
filepath = TxtFileName.Text   'lblfile.Caption
Me.MousePointer = vbHourglass
For i = 0 To lstsheet.ListCount - 1
     If lstsheet.Selected(i) = True Then
        sheetName = lstsheet.List(i)
     End If
Next
Me.MousePointer = vbHourglass
If Nps_Importing_flag = "ECS" Then
    Call ImportTransaction
Else
    Call ImportNonECSTransaction
End If
Me.MousePointer = vbNormal
If OptLife.Value = True Then
ElseIf OptGeneral.Value = True Then
    Call GIAPPMAP
    MsgBox "All Records Updated Successfully", vbInformation
ElseIf OptNPS.Value = True Then
    If Nps_Importing_flag = "ECS" Then
        Call NPSECSIMP
    Else
        Call NPSNONECSIMP
    End If
    MsgBox "All Records Inserted Successfully", vbInformation
End If
MousePointer = vbNormal
Exit Sub
End Sub