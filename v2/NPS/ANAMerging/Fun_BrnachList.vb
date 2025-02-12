Private Sub cmbbranch_Change()
Dim rsData As New ADODB.Recordset

    cmbOldRM.Clear
    If SRmCode = "" Then
        If cmbbranch.ListIndex <> -1 Then
            rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where source=" & cmbbranch.List(cmbbranch.ListIndex, 1) & " and (type='A' or type is null) order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where source in(" & Branches & ") and (type='A' or type is null) order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    Else
        rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code=" & SRmCode & " order by RM_NAME", MyConn, adOpenForwardOnly
    End If
    
    j = 0
    While Not rsData.EOF
        cmbOldRM.AddItem rsData("RM_NAME")
        cmbOldRM.List(j, 1) = rsData("PAYROLL_ID")
        cmbOldRM.List(j, 2) = rsData("RM_CODE")
        j = j + 1
        rsData.MoveNext
    Wend
    rsData.Close
    Set rsData = Nothing

End Sub