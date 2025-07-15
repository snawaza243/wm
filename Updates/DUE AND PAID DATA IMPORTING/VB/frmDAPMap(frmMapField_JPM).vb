Option Explicit
Dim i As Integer
Dim Rs_Exc_Fld As New ADODB.Recordset
Dim ExcelFiled As String, BAckField As String
Dim fno As Integer
Dim AddFileName As String
Dim fso As New filesystemobject
Dim Exc_Fld As String, Back_Fld As String
Dim Split_GenFld() As String
Dim Split_BackFld() As String
Dim AddToFile As String
Dim Split_MAp_Data_Fld() As String
Dim MapField As String
Dim delComma() As String
Dim delHash() As String
Dim DelBrac() As String
Dim Map_File_Xls_Fld As String
Dim Map_File_Data_Fld As String
Dim Back_Data_Fld As String, Back_xls_Fld As String, List_xls_Fld As String
Dim split_Xls_Fld() As String
Dim SqlStr As String
Private Sub cmdExit_Click()
Unload Me
End Sub
Private Sub CmdSave_Click()
If Lst_Map_Fld.ListCount <= 0 Then
    If MsgBox("Mapping Is Not Done,Do You Want To Save the Blank File?", vbYesNo + vbDefaultButton2) = vbNo Then
    Unload Me
        Exit Sub
    End If
End If
fno = FreeFile
'fso.DeleteFile AddFileName
Open Map_Exl_Path For Output As #1
AddToFile = ""
For i = 0 To Lst_Map_Fld.ListCount - 1
     Split_GenFld = Split(Lst_Map_Fld.List(i), ">>")
     Exc_Fld = Split_GenFld(0)
     Back_Fld = Split_GenFld(1)
     If countBrac(Exc_Fld) > 0 Then
        AddToFile = AddToFile & Back_Fld & "#" & Exc_Fld & ","
     Else
        AddToFile = AddToFile & Back_Fld & "#[" & Exc_Fld & "],"
     End If
Next
If AddToFile <> "" Then AddToFile = Mid(AddToFile, 1, Len(AddToFile) - 1)
Print #1, AddToFile
Close #1
MsgBox "Mapped Successfully"
End Sub
Private Sub Command1_Click()
    load_Data (0)
    Lst_Map_Fld.Clear
End Sub
Private Sub Command2_Click()
Call Form_Load
End Sub
Private Sub Form_Load()
load_Data (1)
End Sub
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
Private Function countBrac(ExcelStr As String) As String
Dim K As Integer, l As Integer
K = 0
If InStr(1, ExcelStr, "[") > 0 Then
     K = K + 1
     l = InStr(1, ExcelStr, "[")
End If
If InStr(l + 1, ExcelStr, "[") > 0 Then
    K = K + 1
End If
countBrac = K
End Function



