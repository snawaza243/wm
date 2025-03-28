Private Sub Get_Comp_TextFilePath()
If OptDue.Value = True Then
    TxtFile_Name = "Field_Parameter_due.txt"
ElseIf OptPaid.Value = True Then
    TxtFile_Name = "Field_Parameter_paid.txt"
End If
End Sub