Private Sub Pic_Rem_Fld_Click()
List_xls_Fld = ""
For i = 0 To Lst_Exc_Fld.ListCount - 1
    List_xls_Fld = List_xls_Fld & Lst_Exc_Fld.List(i) & ","
Next
If List_xls_Fld <> "" Then List_xls_Fld = Mid(List_xls_Fld, 1, Len(List_xls_Fld) - 1)
For i = 0 To Lst_Map_Fld.ListCount - 1
     If Lst_Map_Fld.Selected(i) = True Then
        Split_GenFld = Split(Lst_Map_Fld.List(i), ">>")
        Exc_Fld = Split_GenFld(0)
        If countBrac(Exc_Fld) <= 1 Then
            If InStr(1, UCase(Back_xls_Fld), UCase(Exc_Fld)) > 0 Then
                If InStr(1, UCase(List_xls_Fld), UCase(Exc_Fld)) <= 0 Then
                    Lst_Exc_Fld.AddItem Exc_Fld
                End If
            End If
        Else
            Exc_Fld = Replace(Exc_Fld, "]", ",")
            Dim M As Integer, j As Integer, K As Integer
            Dim mstr As String
            M = InStr(1, Exc_Fld, ",")
            j = InStr(1, Exc_Fld, "AS ")
            Exc_Fld = Mid(Exc_Fld, 1, j - 2)
            For K = M To j
                mstr = Mid(Exc_Fld, K + 1, InStr(2, Exc_Fld, "[") - M)
                Exc_Fld = Replace(Exc_Fld, mstr, "")
                Exit For
            Next
            Exc_Fld = Mid(Exc_Fld, 1, Len(Exc_Fld) - 1)
            Exc_Fld = Replace(Exc_Fld, "[", "")
            split_Xls_Fld = Split(Exc_Fld, ",")
            For M = 0 To UBound(split_Xls_Fld)
                If InStr(1, UCase(Back_xls_Fld), UCase(split_Xls_Fld(M))) > 0 Then
                    If InStr(1, UCase(List_xls_Fld), UCase(split_Xls_Fld(M))) <= 0 Then
                        Lst_Exc_Fld.AddItem split_Xls_Fld(M)
                    End If
                End If
            Next
        End If
        Lst_Map_Fld.RemoveItem (i)
        Exit For
     End If
Next
End Sub