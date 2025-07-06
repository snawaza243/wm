Private Sub CmdMark_Click()
If MsgBox("You are going to Mark", vbInformation + vbOKCancel, strBajaj) = vbCancel Then Exit Sub
If CmbCompany.Text = "" Then
Else
    comp = Split(CmbCompany.Text, "#")
    MyCompCode = comp(1)
End If
If MyCompCode <> "" Then
   MyConn.Execute "Update bajaj_due_data set rem_flage='YES' WHERE COMPANY_CD='" & MyCompCode & "' AND MON_NO=" & MyMonth & " AND YEAR_NO=" & MyYear & " AND upper(trim(PAY_MODE))='NON ECS' AND IMPORTDATATYPE='DUEDATA'"
Else
   MyConn.Execute "Update bajaj_due_data set rem_flage='YES' WHERE   MON_NO=" & MyMonth & " AND YEAR_NO=" & MyYear & " AND upper(trim(PAY_MODE='NON ECS')) AND IMPORTDATATYPE='DUEDATA' "
End If
End Sub