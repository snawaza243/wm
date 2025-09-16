Public Sub SaveLogOut(User_Log As String, FormName As String)
On Error Resume Next
    Dim Str As String
    Dim a
    Dim lpaTh_v As String
    Dim rsModule As New ADODB.Recordset
    rsModule.open "SELECT * FROM BACKOFFICE_FORMS WHERE MAIN='" & FormName & "'", MyConn, adOpenForwardOnly
    If Not rsModule.EOF Then
        lpaTh_v = rsModule("main")
    End If
    rsModule.Close
    Set rsModule = Nothing
        
    Str = "update application_log set ENDDATE=to_date('" & Format(Date, "dd/MM/yyyy") & "','DD/MM/YYYY'),ENDTIME='" & Time & "'"
    Str = Str & " where USER_ID='" & User_Log & "' and FORM_REPORT='" & Right(lpaTh_v, 100) & "' and intime='" & GlbSaveLoginTime_vinay & "'"
    a = MyConn.Execute(Str)
End Sub