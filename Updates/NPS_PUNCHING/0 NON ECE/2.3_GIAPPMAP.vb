Private Sub GIAPPMAP()
    Dim Rs_GIApp As ADODB.Recordset
    Dim MyAR As String
    Dim MyDt As String
    Dim sql As String
    Dim MyApp As String
    sql = "select app_no,login_dt,policy_no from Bajaj_PolicyInfo_Data_gen "
    Set Rs_GIApp = New ADODB.Recordset
    Rs_GIApp.open sql, MyConn, adOpenForwardOnly
    If Not Rs_GIApp.EOF Then
        While Not Rs_GIApp.EOF
            MyApp = ""
            MyApp = Rs_GIApp.Fields(0)
            If MyApp <> "" Then
                If SqlRet("select count(*) from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and app_no='" & Rs_GIApp.Fields(0) & "'") > 1 Then
                ElseIf SqlRet("select count(*) from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and ptype<>0 and app_no='" & Rs_GIApp.Fields(0) & "'") = 1 Then
                    MyAR = ""
                    MyDt = ""
                    MyAR = SqlRet("select sys_ar_no from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and ptype<>0 and app_no='" & Rs_GIApp.Fields(0) & "' ")
                    MyDt = SqlRet("select sys_ar_dt from bajaj_ar_head where company_cd= '" & Comp_Cd & "' and ptype<>0 and app_no='" & Rs_GIApp.Fields(0) & "' ")
                    
                    MyConn.Execute "Insert into gicovernoteimport select * from bajaj_ar_head  where sys_ar_no='" & MyAR & "'"
                    MyConn.Execute "Update bajaj_ar_head set status_cd ='A', login_dt=to_date('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR'),ply_issue_dt=add_months(to_date('" & Rs_GIApp.Fields(1) & "','DD/MM/RRRR'),12)-1 , policy_no='" & Rs_GIApp.Fields(2) & "',modified_user='AutoI' where company_cd= '" & Comp_Cd & "' and app_no='" & Rs_GIApp.Fields(0) & "' and ptype<>0 and modified_user<>'AutoI'"
                    
                    sql = ""
    '                sql = " UPDATE BAJAJ_AR_DETAILS SET STATUS_CD='A'  "
    '                sql = sql & " WHERE SYS_AR_NO = '" & MyAR & "' AND STATUS_DT = (SELECT MAX (STATUS_DT) "
    '                sql = sql & "                      FROM BAJAJ_AR_DETAILS "
    '                sql = sql & "                     WHERE SYS_AR_NO = '" & MyAR & "' AND STATUS_CD not in( 'A','F') ) "
                    sql = "insert into bajaj_ar_details (sys_ar_no,status_dt,status_cd,remarks,sys_ar_dt,userid,status_update_on,reason_cd) values"
                    sql = sql & "('" & MyAR & "',to_date(sysdate,'DD/MM/RRRR'),'A','Imported data',"
                    sql = sql & "to_date('" & MyDt & "','DD/MM/RRRR'),'AutoI',sysdate,'A001'"
                    sql = sql & ")"
                    MyConn.Execute (sql)
                Else
                    'MsgBox "This cover Note No. does not Exist in Our Database:" & MyApp
                    Call MarkinExcel(MyApp)
                End If
            End If
            Rs_GIApp.MoveNext
        Wend
    End If
    Rs_GIApp.Close
    Set Rs_GIApp = Nothing
End Sub