Public Sub Show_Tr_Branches(strBusiRM As String)
Dim BrCode() As String
Dim GlCode() As String
Dim GDF As String
Dim RsData As New ADODB.Recordset
Dim RsData_filter As New ADODB.Recordset
Dim j, K As Integer
    Tr_Branches = ""
    RsData_filter.open "select data_filter datafilter from datafilter_master where login_id in(select payroll_id from employee_master where type='A' and category_id=2018) and  login_id='" & strBusiRM & "' AND ROLE_DEFAULT='1' ", MyConn, adOpenForwardOnly 'and role_id in ('5','37')
    If Not RsData_filter.EOF Then
        If Left(RsData_filter("datafilter"), 1) <> "2" And Left(RsData_filter("datafilter"), 1) <> "7" Then
            Tr_Branches = ""
            GlCode = Split(Mid(RsData_filter("datafilter"), 1), "#")
            For K = LBound(GlCode) To UBound(GlCode)
                Set RsData = MyConn.Execute("SELECT getBranch_TR('" & GlCode(K) & "') FROM DUAL")
                If IsNull(RsData(0)) = False Then
                    BrCode = Split(Mid(RsData(0), 2), "#")
                    For j = LBound(BrCode) To UBound(BrCode)
                        Tr_Branches = Tr_Branches & BrCode(j) & ","
                    Next
                End If
             Next
            If Tr_Branches <> "" Then
                Tr_Branches = Left(Tr_Branches, Len(Tr_Branches) - 1)
            End If
        End If
            '        If Left(RsData_filter("datafilter"), 1) = "2" Then
            '            BrCode = Split(Mid(RsData(0), 2), "#")
            '            Tr_Branches = ""
            '            For J = LBound(BrCode) To UBound(BrCode)
            '                Tr_Branches = Tr_Branches & BrCode(J) & ","
            '            Next
            '            Tr_Branches = Left(Tr_Branches, Len(Tr_Branches) - 1)
            '        End If
    End If
    RsData_filter.Close
    If RsData.State = 1 Then RsData.Close
    Set RsData_filter = Nothing
    Set RsData = Nothing
End Sub

