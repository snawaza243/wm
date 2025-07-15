Private Sub load_Data(Flag_Local As Integer)
Dim ie As Integer
On Error GoTo err1
Set Rs_Exc_Fld = Rs_Map_Exl_Fld
Dim FileObj As filesystemobject, FS As Object
Set FileObj = CreateObject("Scripting.Filesystemobject")
Set fso = New filesystemobject
Set FS = FileObj.OpenTextFile(Map_Exl_Path)
MapField = FS.ReadLine
next_Stat:
Lst_Map_Fld.Clear
lst_already_Map.Clear
Lst_Back_Fld.Clear
Split_MAp_Data_Fld = Split(Map_DataBAse_FldName, "/")
Back_Data_Fld = ""
For i = 0 To UBound(Split_MAp_Data_Fld)
    Lst_Back_Fld.AddItem Split_MAp_Data_Fld(i)
Next
If Back_Data_Fld <> "" Then Back_Data_Fld = Mid(Back_Data_Fld, 1, Len(Back_Data_Fld) - 1)
delComma = Split(MapField, ",")
If MapField <> "" Then
    For i = 0 To UBound(delComma)
        delHash = Split(delComma(i), "#")
        Map_File_Data_Fld = Map_File_Data_Fld & delHash(0) & ","
        Map_File_Xls_Fld = Map_File_Xls_Fld & delHash(1) & ","
        If countBrac(delHash(1)) <= 1 Then
            ExcelFiled = Replace(Replace(delHash(1), "[", ""), "]", "")
            BAckField = delHash(0)
            Lst_Map_Fld.AddItem ExcelFiled & ">>" & BAckField
            lst_already_Map.AddItem ExcelFiled & ">>" & BAckField
        Else
            ExcelFiled = delHash(1)
            BAckField = delHash(0)
            Lst_Map_Fld.AddItem ExcelFiled & ">>" & BAckField
             lst_already_Map.AddItem ExcelFiled & ">>" & BAckField
        End If
    Next
End If
Lst_Exc_Fld.Clear
Back_xls_Fld = ""
For i = 0 To Rs_Exc_Fld.Fields.count - 1
    Lst_Exc_Fld.AddItem Rs_Exc_Fld.Fields(i).Name
    Back_xls_Fld = Back_xls_Fld & Rs_Exc_Fld.Fields(i).Name & ","
Next
If Back_xls_Fld <> "" Then Back_xls_Fld = Mid(Back_xls_Fld, 1, Len(Back_xls_Fld) - 1)
'###################################################################################3
'Lst_Exc_Fld
If Flag_Local = 1 Then
    Dim kCount As Integer
    Dim kCount1 As Integer
    For kCount = 0 To Lst_Map_Fld.ListCount - 1
        For kCount1 = 0 To Lst_Exc_Fld.ListCount - 1
            If Lst_Exc_Fld.List(kCount1) = Mid(Lst_Map_Fld.List(kCount), 1, InStr(Lst_Map_Fld.List(kCount), ">") - 1) Then
                Lst_Exc_Fld.RemoveItem (kCount1)
            End If
        Next
    Next
End If
'###################################################################################3
Exit Sub
err1:
   If err.Number = 62 Then
    GoTo next_Stat
   End If
End Sub