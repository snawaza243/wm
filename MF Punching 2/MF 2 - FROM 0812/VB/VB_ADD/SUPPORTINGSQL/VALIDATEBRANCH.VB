Public Function ValidateBranch(pBranch_code As String) As Boolean
Dim DupFlag As Boolean
DupFlag = True
If SqlRet("select count(*) from wealthmaker.branch_master where branch_CODE=" & pBranch_code & " and BRANCH_NAME LIKE 'UNALLO%'") = 0 Then
Else
    DupFlag = False
    Exit Function
End If
ValidateBranch = DupFlag
End Function
