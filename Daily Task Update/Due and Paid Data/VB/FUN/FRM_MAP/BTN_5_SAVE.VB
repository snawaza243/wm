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