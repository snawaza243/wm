Private Sub branch_fill()
Dim rsEmp As Adodb.Recordset
Dim b_cd() As String

Set rsEmp = New Adodb.Recordset
rsEmp.open "Select * from employee_master where payroll_id='" & txtrmbusicode.Text & "' and type='A'", MyConn, adOpenForwardOnly
If Not rsEmp.EOF Then
    If rsEmp.Fields("category_id") <> "2001" And rsEmp.Fields("category_id") <> "2018" Then
        MsgBox "Rm Should be Sales and Support Only "
        txtrmbusicode.Text = ""
        cmbBusiBranch.Clear
        Exit Sub
    End If
End If
rsEmp.Close
Set rsEmp = Nothing
    If Len(txtrmbusicode.Text) = 5 Or Len(txtrmbusicode.Text) = 6 Then
        cmbBusiBranch.Clear
        If strbranch <> "" Then
            cmbBusiBranch.AddItem strbranch
            cmbBusiBranch.ListIndex = 0
        End If
        Set rsEmp = New Adodb.Recordset
        rsEmp.open "Select source,branch_name from employee_master e,branch_master b where e.payroll_id='" & Trim(txtrmbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null)", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            If strbranch <> "" Then
                b_cd = Split(strbranch, "#")
                If b_cd(1) <> rsEmp(0) Then
                    cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                    cmbBusiBranch.ListIndex = 0
                End If
            Else
                cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                cmbBusiBranch.ListIndex = 0
            End If
        End If
        rsEmp.Close
        Set rsEmp = Nothing
    End If
End Sub