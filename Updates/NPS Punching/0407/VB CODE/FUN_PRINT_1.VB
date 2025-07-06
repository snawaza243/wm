Public Sub SaveLogIn(User_Log As String, Session_Id_As_FormPath_in_Case_Wealthmaker As String, FormName As String)
On Error GoTo Desc
    Dim Str As String
    Dim a
    Dim lpaTh_v As String
    lpaTh_v = ""
    Dim rsModule As New ADODB.Recordset
    rsModule.open "SELECT * FROM BACKOFFICE_FORMS WHERE MAIN='" & FormName & "'", MyConn, adOpenForwardOnly
    If Not rsModule.EOF Then
        lpaTh_v = rsModule("main")
    End If
    GlbSaveLoginTime_vinay = Time
    rsModule.Close
    Set rsModule = Nothing
    'if CDate(text)<Cdate("1/2/2007")
    
    'Desc application_log
    'Name                             Null?  Type
    '-------------------------------- ------ -------------------------
    'USER_ID                          Yes    VARCHAR2(30)
    'SESSION_ID                       Yes    VARCHAR2(100)
    'INDATE                           Yes    DATE
    'INTIME                           Yes    VARCHAR2(25)
    'ENDDATE                          Yes    DATE
    'ENDTIME                          Yes    VARCHAR2(25)
    'MODULE                           Yes    VARCHAR2(50)
    'FORM_REPORT                      Yes    VARCHAR2(100) --to_date('" & Date & "','mm/dd/yyyy')
    Str = "insert into application_log (USER_ID,SESSION_ID,INDATE,INTIME,MODULE,FORM_REPORT) values "
    Str = Str & "('" & User_Log & "','" & Session_Id_As_FormPath_in_Case_Wealthmaker & "',to_date('" & Format(Date, "dd/MM/yyyy") & "','DD/MM/YYYY'),'" & GlbSaveLoginTime_vinay & "','WEALTHMAKER','" & Right(lpaTh_v, 100) & "')"
    a = MyConn.Execute(Str)
    Exit Sub
Desc:
    'Call OnErrorInLog_Vinay(Err.Description, "SaveLogIn", lpaTh_v)
    End Sub