Private Sub cmdFind_Click()
 Dim kCount As Integer
    Dim KCount_cOL As Integer
    Dim Column_name As Integer
    Dim KCount_Row As Integer
    Glb_Flag_First_Time = True
    If Trim(TxtFind.Text) = "" Then
        Glb_L_SearchIndex = 1
        Exit Sub
    End If
     Clear_Previously_Selected
     For kCount = Glb_L_SearchIndex To VSFCommGrdM.Rows - 1
        If InStr(1, UCase(VSFCommGrdM.TextMatrix(kCount, ColumnIndex)), UCase(TxtFind.Text)) > 0 Then
            Glb_Comming_From_Search = True
            VSFCommGrdM.Row = kCount
            If kCount > 12 Then VSFCommGrdM.TopRow = kCount
            For KCount_cOL = 1 To VSFCommGrdM.Cols - 1
                Glb_Comming_From_Search = True
                VSFCommGrdM.Col = KCount_cOL
                VSFCommGrdM.CellBackColor = vbWhite
                VSFCommGrdM.FontBold = True
                VSFCommGrdM.CellForeColor = vbRed
                VSFCommGrdM.CellFontBold = True
            Next
            Glb_L_SearchIndex = kCount + 1
            Glb_Selected_row = kCount
            VSFCommGrdM.TopRow = kCount
            Exit Sub
       End If
    Next
    If kCount >= VSFCommGrdM.Rows - 1 Then
        MsgBox "No Such Record Found", vbInformation, "Search Completed"
        TxtFind.Text = ""
        TxtFind.SetFocus
        Glb_L_SearchIndex = 1
        Glb_Selected_row = 0
    End If
End Sub

