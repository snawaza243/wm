Public Sub TxtBusiCodeM_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim Rs_GetBranchCd As ADODB.Recordset
Dim b_cd() As String
    If Len(TxtBusiCodeM.Text) <> 0 Then
        TxtBranchM.Text = ""
        
        If Len(Trim(Label42.Caption)) > 0 Then
            Set Rs_GetBranchCd = New ADODB.Recordset
            Rs_GetBranchCd.open "select i.branch_code from investor_master i where I.inv_code='" & Trim(Label42.Caption) & "'", MyConn, adOpenDynamic, adLockOptimistic
            If Rs_GetBranchCd.EOF = False Then
                branch_code = Rs_GetBranchCd!branch_code
            End If
            Rs_GetBranchCd.Close
        End If
        Set Rs_GetBranchCd = Nothing
        
        
        If AllIndia = False Then
            If branch_code = "" Then
                TxtBusiCodeM.Text = ""
                MsgBox "Please Select Investor First", vbInformation
                Exit Sub
            End If
        End If
        

        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeM.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018) ", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            cmbBusiBranchM.Clear
            cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
            cmbBusiBranchM.ListIndex = 0
            TxtRmNameM.Text = rsEmp(2)
            'MyRmCodeM = rsEmp(3)
            'Label14.Caption = rsEmp(1)
            Label30.Caption = rsEmp(2)
            Call Show_Tr_Branches(Trim(TxtBusiCodeM.Text))
            If valid_rm(branch_code, TxtBusiCodeM) = False Then Exit Sub
            If rsEmp.State = 1 Then rsEmp.Close
            
            
            If Tr_Branches <> "" Then
                cmbBusiBranchM.Clear
                Set rsEmp = New ADODB.Recordset
                rsEmp.open "Select branch_code,branch_name from branch_master b where branch_code in (" & Tr_Branches & ") order by branch_name", MyConn, adOpenForwardOnly
                While Not rsEmp.EOF
                    cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                    If rsEmp(0) = busi_br_tr Then
                        cmbBusiBranchM.ListIndex = i
                    End If
                    rsEmp.MoveNext
                Wend
                'cmbBusiBranchM.ListIndex = 0
            End If
            
            If busi_change_flag = True Then
                set_default_branch branch_code
            End If
            
            If rsEmp.State = 1 Then rsEmp.Close
            Set rsEmp = Nothing
            
        Else
            TxtBranchM.Text = ""
            MyRmCodeM = 0
            TxtBusiCodeM.Text = ""
            TxtRmNameM.Text = ""
            Label14.Caption = ""
            Label30.Caption = ""
        End If
        
        Set rsEmp = Nothing
    End If
End Sub

