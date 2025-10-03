Public Sub Show_Tr_Branches_new(strBusiRM As String)
Dim BrCode() As String
Dim GlCode() As String
Dim GDF As String
Dim RsData As New ADODB.Recordset
Dim RsData_filter As New ADODB.Recordset
Dim j, K As Integer
    Tr_Branches = ""
    sql = ""
    sql = " select branch_id  from userdetails_ji where login_id='" & strBusiRM & "' and datafilter in( "
    sql = sql & " select data_filter datafilter from datafilter_master where
 login_id in(select payroll_id from employee_master where type='A' and category_id=2001) 
and  login_id='" & strBusiRM & "' AND ROLE_DEFAULT='1' )"
    
    RsData_filter.open sql, MyConn, adOpenForwardOnly
    While Not RsData_filter.EOF
            Set RsData = MyConn.Execute("SELECT getBranch_TR('" & RsData_filter.Fields("branch_id") & "') FROM DUAL")
            If IsNull(RsData(0)) = False Then
                BrCode = Split(Mid(RsData(0), 2), "#")
                For j = LBound(BrCode) To UBound(BrCode)
                    Tr_Branches = Tr_Branches & BrCode(j) & ","
                Next
            End If
            RsData_filter.MoveNext
    Wend
    If Tr_Branches <> "" Then
        Tr_Branches = Left(Tr_Branches, Len(Tr_Branches) - 1)
    End If
    RsData_filter.Close
    If RsData.State = 1 Then RsData.Close
    Set RsData_filter = Nothing
    Set RsData = Nothing
End Sub
