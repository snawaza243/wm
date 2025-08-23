Private Sub Clear_Previously_Selected()
    On Error GoTo Bot
    Dim KCount_cOL As Integer
    If Glb_Flag_First_Time = False Then
        Exit Sub
    End If
    If Glb_Selected_row <> 0 Then
         VSFCommGrdM.Row = Glb_Selected_row
         For KCount_cOL = 1 To VSFCommGrdM.Cols - 1
            VSFCommGrdM.Col = KCount_cOL
            VSFCommGrdM.CellBackColor = vbWhite
            VSFCommGrdM.FontBold = True
            VSFCommGrdM.CellForeColor = vbBlack
            VSFCommGrdM.CellFontBold = False
        Next
    End If
    Exit Sub
Bot:
    'Call ShowErrorMessage(err.Description)
End Sub


