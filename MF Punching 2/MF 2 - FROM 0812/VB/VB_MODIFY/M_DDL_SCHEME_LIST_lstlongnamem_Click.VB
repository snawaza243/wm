Private Sub lstlongnamem_Click()
TxtSchemeM.Text = lstlongnameM.Text
Dim str1() As String
Dim rsnat As ADODB.Recordset

    If TxtSchemeM.Text <> "" Then
        str1 = Split(TxtSchemeM.Text, "=")
        MySchCode = str1(1)
    End If


    Set rsnat = New ADODB.Recordset
    rsnat.open "select nvl(get_scheme_nature('" & MySchCode & "'),'') a from dual", MyConn, adOpenForwardOnly
    If rsnat("a") = "O" Then
        frmtransactionmf.Label2(2).Caption = "Expense Trail%"
        frmtransactionmf.Label2(1).Caption = "Expense Trail(Rs.)"
        MyNat = "O"
    Else
        frmtransactionmf.Label2(2).Caption = "Expense%"
        frmtransactionmf.Label2(1).Caption = "Expense(Rs.)"
        MyNat = "C"
    End If
    rsnat.Close
    Set rsnat = Nothing
    
    
End Sub

