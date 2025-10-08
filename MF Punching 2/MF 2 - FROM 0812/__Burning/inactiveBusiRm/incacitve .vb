Public Sub txtbusicode_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim b_cd() As String
Dim RSEMP1 As New ADODB.Recordset
MyBranch = ""
MyRmCode = ""
    If Len(txtbusicode.Text) <> 0 And txtbusicode.Text <> "95829" And txtbusicode.Text <> "103914" Then
        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.category_id in('2001','2018')", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            MyBranch = rsEmp(0)
            MyRmCode = rsEmp(3)
            Label32.Caption = rsEmp(1)
            Label55.Caption = rsEmp(2)
        Else
            RSEMP1.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where B.BRANCH_CODE IN (" & Branches & ") AND E.payroll_id='" & Trim(txtbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) ", MyConn, adOpenForwardOnly
            If RSEMP1.EOF = False Then
            MsgBox "RM Should be of Sale Support OR Sales", vbInformation
            Else
            MsgBox "RM Should be Active", vbInformation
            End If
            RSEMP1.Close
            Label32.Caption = ""
            Label55.Caption = ""
            If txtbusicode.Text <> "95829" Then
                txtbusicode = ""
            End If
            Exit Sub
        End If
        rsEmp.Close
        Set rsEmp = Nothing
    Else
        If txtbusicode.Text = "95829" Then
           txtbusicode = ""
        End If
    End If
End Sub