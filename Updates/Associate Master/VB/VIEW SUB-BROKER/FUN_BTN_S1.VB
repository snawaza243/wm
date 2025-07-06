Private Sub Search(StrSql As String)
On Error Resume Next
Set rsSB = New ADODB.Recordset
Dim i As Integer
Dim j As Integer
If rsSB.State = 1 Then rsSB.Close
rsSB.CursorLocation = adUseClient
rsSB.open StrSql, MyConn, adOpenForwardOnly, adLockReadOnly
If rsSB.Fields("address3") <> " " Then
    MyAddress3 = rsSB.Fields("address3")
Else
     MyAddress3 = ""
End If
Label7.Caption = " " & rsSB.RecordCount & " Record(s) Found "
Me.MousePointer = 11
If rsSB.RecordCount > 0 Then
    dataview.Rows = rsSB.RecordCount + 1
    With dataview
        For i = 0 To rsSB.RecordCount - 1
            For j = 0 To rsSB.Fields.count - 1
                .Row = i + 1
                .Col = j
                If Not IsNull(rsSB(j)) Then
                    .Text = rsSB(j)
                Else
                    .Text = ""
                End If
            Next
            rsSB.MoveNext
        Next
    End With
End If
If rsSB.State = 1 Then rsSB.Close
End Sub