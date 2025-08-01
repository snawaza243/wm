Private Sub cmdFind_Click()
Dim kCount As Integer
    Dim KCount_cOL As Integer
    Dim Column_name As Integer
    Dim KCount_Row As Integer
    Glb_Flag_First_Time = True
    If Trim(txtFind.Text) = "" Then
        Glb_L_SearchIndex = 1
        Exit Sub
    End If
     Clear_Previously_Selected
     For kCount = Glb_L_SearchIndex To VSFCommGrdK.Rows - 1
        If InStr(1, UCase(VSFCommGrdK.TextMatrix(kCount, ColumnIndex)), UCase(txtFind.Text)) > 0 Then
            Glb_Comming_From_Search = True
            VSFCommGrdK.Row = kCount
            If kCount > 12 Then VSFCommGrdK.TopRow = kCount
            For KCount_cOL = 1 To VSFCommGrdK.Cols - 1
                Glb_Comming_From_Search = True
                VSFCommGrdK.Col = KCount_cOL
                VSFCommGrdK.CellBackColor = vbWhite
                VSFCommGrdK.FontBold = True
                VSFCommGrdK.CellForeColor = vbRed
                VSFCommGrdK.CellFontBold = True
            Next
            Glb_L_SearchIndex = kCount + 1
            Glb_Selected_row = kCount
            VSFCommGrdK.TopRow = kCount
            Exit Sub
       End If
    Next
    If kCount >= VSFCommGrdK.Rows - 1 Then
        MsgBox "No Such Record Found", vbInformation, "Search Completed"
        txtFind.Text = ""
        txtFind.SetFocus
        Glb_L_SearchIndex = 1
        Glb_Selected_row = 0
    End If
End Sub

