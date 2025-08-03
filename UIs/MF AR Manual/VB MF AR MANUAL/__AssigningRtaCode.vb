Private Sub VSFCommGrdK_Click()
On Error Resume Next
ColumnIndex = VSFCommGrdK.Col
Clear_Previously_Selected1
Glb_L_SearchIndex1 = 1
Glb_Selected_row1 = 0
Label6(0).Caption = "" & VSFCommGrdK.TextMatrix(0, VSFCommGrdK.Col)

MyCurrRow = VSFCommGrdK.Row
If VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 0) <> "" Then
      If MySelectedRow > 0 Then
          VSFCommGrdK.Row = MySelectedRow
          For KCount_cOL = 0 To VSFCommGrdK.Cols - 1
               VSFCommGrdK.Col = KCount_cOL
                VSFCommGrdK.CellBackColor = vbWhite
                VSFCommGrdK.CellForeColor = vbBlack
                VSFCommGrdK.CellFontBold = False
          Next
      End If
      VSFCommGrdK.Row = MyCurrRow
      For KCount_cOL = 0 To VSFCommGrdK.Cols - 1
        VSFCommGrdK.Col = KCount_cOL
        If VSFCommGrdK.CellBackColor = vbBlue Then
            VSFCommGrdK.CellBackColor = vbWhite
            VSFCommGrdK.CellForeColor = vbBlack
            VSFCommGrdK.CellFontBold = False
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdK.CellForeColor = vbWhite
            End If
        Else
            VSFCommGrdK.CellBackColor = vbBlue
            VSFCommGrdK.CellForeColor = vbWhite
            MySelectedRow = VSFCommGrdK.Row
            'VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 16) = MyTrCode
            VSFCommGrdK.CellFontBold = True
            If KCount_cOL = 18 Or KCount_cOL = 19 Then
                VSFCommGrdK.CellForeColor = vbBlue
            End If
        End If
        If KCount_cOL = 18 Then
            If VSFCommGrdK.CellPicture = Image1.Picture Then
                Exit Sub
            End If
            If VSFCommGrdK.CellPicture = Image2.Picture Then
                Set VSFCommGrdK.CellPicture = Image22.Picture
                VSFCommGrdK.CellPictureAlignment = flexPicAlignCenterCenter
                VSFCommGrdK.Text = 1
            Else
                 Set VSFCommGrdK.CellPicture = Image2.Picture
                 VSFCommGrdK.CellPictureAlignment = flexPicAlignCenterCenter
                 VSFCommGrdK.Text = 0
            End If
        End If
    Next
End If
MyRtaTrCode = ""
MyRtaTrDate = ""
MyRtaAmount = ""
MyRtaTrCode = VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 0)
MyRtaTrDate = VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 1)
MyRtaAmount = Val(VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 7))
MyRtaFolio = Val(VSFCommGrdK.TextMatrix(VSFCommGrdK.Row, 8))
End Sub