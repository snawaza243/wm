Private Sub Form_Activate()
Dim Rs_rm As New ADODB.Recordset
    cmbBranch.Clear
    If GlbDataFilter = "72" Then
        Rs_rm.open "Select branch_code,branch_name from branch_master where category_id not in('1004','1005','1006') order by branch_name", MyConn, adOpenForwardOnly
    Else
        Rs_rm.open "Select branch_code,branch_name from branch_master where branch_code in (" & Branches & ") and category_id not in('1004','1005','1006') order by branch_name", MyConn, adOpenForwardOnly
    End If
    
    j = 0
    While Not Rs_rm.EOF
        cmbBranch.AddItem Rs_rm("branch_name")
        cmbBranch.List(j, 1) = Rs_rm("branch_code")
        j = j + 1
        Rs_rm.MoveNext
    Wend
    Rs_rm.Close
    Set Rs_rm = Nothing
    'cmbbranch.ListIndex = 0

End Sub