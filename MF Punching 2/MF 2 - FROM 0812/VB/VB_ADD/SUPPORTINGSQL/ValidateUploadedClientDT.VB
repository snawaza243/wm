Public Function ValidateUploadedClientDT(ByVal PPDT_NO As String, ByVal PClient_cd As String) As Boolean
ValidateUploadedClientDT = True
Dim cmd As New ADODB.Command
Dim vResult As String
Dim vErrMsg As String

Set cmd.ActiveConnection = MyConn
cmd.CommandType = adCmdStoredProc
cmd.CommandText = "WEALTHMAKER.FN_VALIDATE_CLIENT_DT"
cmd.Parameters.Append cmd.CreateParameter("PDT_NO", adVarChar, adParamInput, 15, PPDT_NO)
cmd.Parameters.Append cmd.CreateParameter("PCLIENT_CODE", adVarChar, adParamInput, 15, PClient_cd)
cmd.Parameters.Append cmd.CreateParameter("RESULT", adDouble, adParamOutput)
cmd.Parameters.Append cmd.CreateParameter("MESSAGE", adVarChar, adParamOutput, 200)
cmd.Execute
vResult = IIf(IsNull(cmd("RESULT")), "", cmd("RESULT"))
vErrMsg = IIf(IsNull(cmd("MESSAGE")), "", cmd("MESSAGE"))
Set cmd = Nothing
If vResult = 0 Then
    ValidateUploadedClientDT = False
    MsgBox vErrMsg, vbInformation
End If
End Function

