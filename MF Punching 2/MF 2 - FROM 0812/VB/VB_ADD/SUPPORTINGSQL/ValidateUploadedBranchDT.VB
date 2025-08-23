Public Function ValidateUploadedBranchDT(ByVal PPDT_NO As String, ByVal PBranch_cd As String, ByVal PPayroll_id As String) As Boolean
ValidateUploadedBranchDT = True
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String

Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.FN_VALIDATE_BRANCH_RM_DT"
cmd.Parameters.Append cmd.CreateParameter("PDT_NO", adVarChar, adParamInput, 15, PPDT_NO)
cmd.Parameters.Append cmd.CreateParameter("PBRANCH_CODE", adVarChar, adParamInput, 15, PBranch_cd)
cmd.Parameters.Append cmd.CreateParameter("PPAYROLL_ID", adVarChar, adParamInput, 15, PPayroll_id)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 200)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing
If vResult = 0 Then
    ValidateUploadedBranchDT = False
    MsgBox vErrMsg, vbInformation
End If
End Function

