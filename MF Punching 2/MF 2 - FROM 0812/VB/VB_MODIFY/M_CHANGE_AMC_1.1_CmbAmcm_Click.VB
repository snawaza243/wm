Public Sub CmbAmcm_Click()
Dim StrAmc() As String
Dim LvwItem As ListItem
TxtSchemeM.Text = ""
If rsAmc.State = 1 Then rsAmc.Close
rsAmc.open "select mut_code from mut_fund where upper(trim(mut_name))='" & UCase(Trim(CmbAmcM.Text)) & "'", MyConn
If rsAmc.EOF = True Then
    If rsAmc.State = 1 Then rsAmc.Close
    rsAmc.open "select iss_code from iss_master where upper(trim(iss_name))='" & UCase(Trim(CmbAmcM.Text)) & "'", MyConn
    If rsAmc.EOF = False Then
        MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select osch_code,osch_name from other_product where iss_code='" & MyAmcCode & "' order by osch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameM.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameM.AddItem rsscheme.Fields("osch_name") & Space(100) & "=" & rsscheme.Fields("osch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameM.Clear
    End If
Else
    If rsAmc.EOF = False Then
        MyAmcCode = rsAmc.Fields(0)
        If rsscheme.State = 1 Then rsscheme.Close
        rsscheme.open "select sch_code,sch_name from scheme_info where mut_code='" & rsAmc.Fields("mut_code") & "' order by sch_name", MyConn, adOpenStatic, adLockOptimistic
        lstlongnameM.Clear
        If rsscheme.RecordCount > 0 Then rsscheme.MoveFirst
        Do While Not rsscheme.EOF
            lstlongnameM.AddItem rsscheme.Fields("sch_name") & Space(100) & "=" & rsscheme.Fields("sch_code")
            rsscheme.MoveNext
        Loop
        rsscheme.Close
        rsAmc.Close
    Else
        lstlongnameM.Clear
    End If
End If
End Sub

