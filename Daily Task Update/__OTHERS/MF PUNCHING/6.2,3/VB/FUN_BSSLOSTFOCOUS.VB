Public Sub TxtBusiCodeA_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim Rs_GetBranchCd As ADODB.Recordset
Dim b_cd() As String
Dim i As Integer
    If Len(TxtBusiCodeA.Text) <> 0 Then
        TxtBranchA.Text = ""
        
        If Len(Trim(Label32.Caption)) > 0 Then
            Set Rs_GetBranchCd = New ADODB.Recordset
            Rs_GetBranchCd.open "select i.branch_code from investor_master i where I.inv_code='" & Trim(Label32.Caption) & "' ", MyConn, adOpenDynamic, adLockOptimistic
            If Rs_GetBranchCd.EOF = False Then
                branch_code = Rs_GetBranchCd!branch_code
            End If
            Rs_GetBranchCd.Close
        End If
        Set Rs_GetBranchCd = Nothing
        
        If AllIndia = False Then
            If branch_code = "" Then
                TxtBusiCodeA.Text = ""
                MsgBox "Please Select Investor First", vbInformation
                Exit Sub
            End If
        End If
        
        
        
        Set rsEmp = New ADODB.Recordset
        If AllIndiaSearchFlag <> "ALL" Then
            rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where b.branch_code in (" & Branches & ") and e.payroll_id='" & Trim(TxtBusiCodeA.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018)", MyConn, adOpenForwardOnly
        Else
            rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeA.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018)", MyConn, adOpenForwardOnly
        End If
        If rsEmp.EOF = False Then
            cmbBusiBranch.Clear
            cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
            cmbBusiBranch.ListIndex = 0
            TxtRmName.Text = rsEmp(2)
            'MyRmCodeA = rsEmp(3)
            MyCurrentBranchCode = rsEmp(0)
            'Label9.Caption = rsEmp(1)
            Label29.Caption = rsEmp(2)
            
            If rsEmp.State = 1 Then rsEmp.Close
            'Call Show_Tr_Branches(Trim(TxtBusiCodeA.Text))
            ''''himanshu
            If valid_rm(branch_code, TxtBusiCodeA) = False Then Exit Sub
            i = 0
            If Tr_Branches <> "" Then
'                cmbBusiBranch.Clear
'                Set rsEmp = New ADODB.Recordset
'                rsEmp.open "Select branch_code,branch_name from branch_master b where branch_code in (" & Tr_Branches & ") order by branch_name", MyConn, adOpenForwardOnly
'                While Not rsEmp.EOF
'                    cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
'                    If rsEmp(0) = branch_code Then
'                        cmbBusiBranch.ListIndex = i
'                    End If
'                    i = i + 1
'                    rsEmp.MoveNext
'                Wend

                'cmbBusiBranch.ListIndex = 0

            End If
            
            If rsEmp.State = 1 Then rsEmp.Close
            Set rsEmp = Nothing
        Else
            MsgBox "Employee Should Be Of Sale Support or marketing", vbInformation
            TxtBranchA.Text = ""
            cmbBusiBranch.Clear
            TxtBusiCodeA.Text = ""
            MyRmCodeA = 0
            TxtRmName.Text = ""
            Label9.Caption = ""
            Label29.Caption = ""
            Label32.Caption = ""
            txtInvestorA.Text = ""
        End If
        Set rsEmp = Nothing
        
        
    End If
End Sub