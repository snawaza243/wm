Private Sub NPSECSIMP()
Dim Rs_GIApp As ADODB.Recordset
    Dim MyAR As String
    Dim MyDt As String
    Dim sql As String
    Dim MyApp As String
    sql = ""
    sql = "select REF_TRAN_CODE,TR_DATE,ECS_AMT,CONSUMER_CODE from NPS_ECS_TBL_IMP "
    Set Rs_GIApp = New ADODB.Recordset
    Rs_GIApp.open sql, MyConn, adOpenForwardOnly
    If Not Rs_GIApp.EOF Then
        While Not Rs_GIApp.EOF
            MyApp = ""
            MyApp = Rs_GIApp.Fields(0)
            If MyApp <> "" Then
                If SqlRet("select count(*) from TRANSACTION_ST where MANUAL_ARNO= '" & MyApp & "' and TO_DATE(TR_DATE,'DD/MM/RRRR')=TO_DATE('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR')") = 0 Then
                    Dim MyComIsuue As New ADODB.Command
                            MyComIsuue.ActiveConnection = MyConn
                            MyComIsuue.CommandType = adCmdStoredProc
                            MyComIsuue.CommandTimeout = 99999
                            MyComIsuue.CommandText = "NPS_ECSTRAN_GENERATE_IMP"
                            MyComIsuue.Parameters.Append MyComIsuue.CreateParameter("CONSUMERCODE", adChar, adParamInput, 50, MyApp)
                            MyComIsuue.Execute
                            Set MyComIsuue = Nothing
                            DoEvents
                Else
                    Call MarkinExcel(MyApp)
                End If
            End If
            Rs_GIApp.MoveNext
        Wend
    End If
    Rs_GIApp.Close
    Set Rs_GIApp = Nothing
End Sub