Private Sub pic_add_fld_Click()

If Lst_Exc_Fld.SelCount = 0 Then
    MsgBox "Please Select Excel Filed Name", vbInformation
    Exit Sub
End If

If Lst_Back_Fld.SelCount = 0 Then
    MsgBox "Please Select BackOffice Field Name", vbInformation
    Exit Sub
End If
ExcelFiled = ""
For i = 0 To Lst_Exc_Fld.ListCount - 1
    If Lst_Exc_Fld.SelCount = 1 Then
        If Lst_Exc_Fld.Selected(i) = True Then
            ExcelFiled = Lst_Exc_Fld.List(i)
            Exit For
        End If
    Else
        If Lst_Exc_Fld.Selected(i) = True Then
            ExcelFiled = ExcelFiled & "[" & Lst_Exc_Fld.List(i) & "] & ' ' & "
        End If
    End If
Next
For i = 0 To Lst_Back_Fld.ListCount - 1
    If Lst_Back_Fld.Selected(i) = True Then
        BAckField = Lst_Back_Fld.List(i)
        Exit For
    End If
Next
If Lst_Exc_Fld.SelCount > 1 Then
    ExcelFiled = Mid(ExcelFiled, 1, Len(ExcelFiled) - 9)
    ExcelFiled = ExcelFiled & " AS " & BAckField
End If
Dim kCount As Integer
For kCount = 0 To Lst_Map_Fld.ListCount - 1
    If StrConv(Mid(Lst_Map_Fld.List(kCount), InStr(Lst_Map_Fld.List(kCount), ">") + 2), vbUpperCase) = StrConv(BAckField, vbUpperCase) Then
        MsgBox "Mapping to more then one column from excel to same database field is not allowed." & vbCrLf & _
              "Excel--[" & Mid(Lst_Map_Fld.List(kCount), 1, InStr(Lst_Map_Fld.List(kCount), ">") - 1) & "]  ----------- Mapped To-------- > Backoffice--[" & Mid(Lst_Map_Fld.List(kCount), InStr(Lst_Map_Fld.List(kCount), ">") + 2) & "] "
        Exit Sub
    End If
    If StrConv(Mid(Lst_Map_Fld.List(kCount), 1, InStr(Lst_Map_Fld.List(kCount), ">") - 1), vbUpperCase) = StrConv(ExcelFiled, vbUpperCase) Then
        MsgBox "This [" & Mid(Lst_Map_Fld.List(kCount), 1, InStr(Lst_Map_Fld.List(kCount), ">") - 1) & "] excel field is already mapped with [" & Mid(Lst_Map_Fld.List(kCount), InStr(Lst_Map_Fld.List(kCount), ">") + 2) & "]"
        Exit Sub
    End If
Next
Lst_Map_Fld.AddItem ExcelFiled & ">>" & BAckField
Dim M, K As Integer
M = Lst_Exc_Fld.SelCount
For K = 1 To M
    For i = 0 To Lst_Exc_Fld.ListCount - 1
        If Lst_Exc_Fld.Selected(i) = True Then
            Lst_Exc_Fld.RemoveItem (i)
            Exit For
        End If
    Next
Next
End Sub