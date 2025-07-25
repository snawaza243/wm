
Dim rs_get_br As ADODB.Recordset
Dim rs_get_Loc As ADODB.Recordset
    If UCase(Left(refcode, 1)) = "R" Then
        qry = "select branch_code from branch_master where region_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
      ElseIf UCase(Left(refcode, 1)) = "Z" Then
        qry = "select branch_code from branch_master where zone_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "C" Then
        Set rs_get_Loc = New ADODB.Recordset
        qry = "select location_id from location_master where city_id='" & refcode & "'"
        Set rs_get_Loc = MyConn.Execute(qry)
        If Not rs_get_Loc.EOF Then
            Set rs_get_br = New ADODB.Recordset
            Do While Not rs_get_Loc.EOF
                qry = "select branch_code from branch_master where location_id='" & rs_get_Loc(0) & "'"
                Set rs_get_br = MyConn.Execute(qry)
                If Not rs_get_br.EOF Then
                    Do While Not rs_get_br.EOF
                        BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                        rs_get_br.MoveNext
                    Loop
                End If
                rs_get_Loc.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "L" Then
        qry = "select branch_code from branch_master where location_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf Left(refcode, 1) = "2" Then
        qry = "select source from employee_master where rm_code='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf Left(refcode, 1) = "1" Then
        BRCODELIST = BRCODELIST & "#" & refcode
    ElseIf Left(refcode, 1) = "7" Then
        qry = "select branch_code from branch_master"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    End If
    getBranchName = BRCODELIST
End Function
