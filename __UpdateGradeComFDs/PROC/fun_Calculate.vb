Private Sub CmdSave_Click()
If CmbMonth.Text = "" Then
    MsgBox "select  Valid Month", vbInformation
    Exit Sub
End If
If Cmbyear.Text = "" Then
    MsgBox "select  Valid year", vbInformation
    Exit Sub
End If
MyConn.Execute " delete  from iss_comp_grade where mon_no=" & CmbMonth.ListIndex + 1 & " and year_no=" & Cmbyear.Text & ""

For Count_Loop = 1 To VSGRADE.Rows - 1
    If VSGRADE.TextMatrix(Count_Loop, 3) <> "" Then
        SqlStr = "insert into iss_comp_grade(mon_no,year_no,iss_tag,iss_code,iss_name,grade) values(" & CmbMonth.ListIndex + 1 & "  "
        SqlStr = SqlStr & "," & Cmbyear.Text & " ,'" & VSGRADE.TextMatrix(Count_Loop, 0) & "','" & VSGRADE.TextMatrix(Count_Loop, 1) & "' "
        SqlStr = SqlStr & ",'" & VSGRADE.TextMatrix(Count_Loop, 2) & "' ,'" & VSGRADE.TextMatrix(Count_Loop, 3) & "')"

        MyConn.Execute SqlStr
    End If
''
'' '   If VSGRADE.TextMatrix(Count_Loop, 3) <> "" Then
''        SqlStr = "insert into iss_comp_grade(mon_no,year_no,iss_tag,iss_code,iss_name,grade) values(" & CmbMonth.ListIndex + 1 & "  "
''        SqlStr = SqlStr & "," & Cmbyear.Text & " ,'" & VSGRADE.TextMatrix(Count_Loop, 0) & "','" & VSGRADE.TextMatrix(Count_Loop, 1) & "' "
''        SqlStr = SqlStr & ",'" & VSGRADE.TextMatrix(Count_Loop, 2) & "' ,'A')"
''
''        myconn.Execute SqlStr
'' '   End If

Next

MsgBox "Saved Successfully", vbInformation

End Sub

