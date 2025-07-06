
Private Sub update_bajajar_status(ply_no As String, Comp_Cd As String, status_cd As String, import_type As String)
On Error GoTo err
Dim RS As New ADODB.Recordset
Dim rschk As New ADODB.Recordset
RS.open "SELECT * FROM BAJAJ_AR_HEAD WHERE POLICY_NO='" & ply_no & "' AND COMPANY_CD='" & Comp_Cd & "' AND TO_CHAR(SYS_AR_DT,'MON-YYYY')= '" & UCase(CmbMonth.Text) & "-" & TxtYear.Text & "' and status_cd='" & status_cd & "'", MyConn
If RS.EOF = False Then
    rschk.open "SELECT * FROM BAJAJ_AR_DETAILS WHERE SYS_AR_NO='" & RS("sys_aR_no") & "' and status_dt=last_day(to_date('" & RS("sys_aR_dt") & "','dd/mm/yyyy'))", MyConn
    If rschk.EOF = True Then
         MyConn.Execute ("update bajaj_AR_head set status_cd='" & status_cd & "' where sys_aR_no='" & RS("sys_aR_no") & "'")
         MyConn.Execute "INSERT INTO BAJAJ_AR_DETAILS (SYS_AR_NO,STATUS_DT,STATUS_CD,REMARKS,SYS_AR_DT,STATUS_UPDATE_ON)  VALUES('" & RS("sys_aR_no") & "',last_day('" & Format(RS("sys_aR_dt"), "DD-MMM-YYYY") & "'),'" & status_cd & "','" & import_type & "'||' '||sysdate ,'" & Format(RS("sys_aR_dt"), "DD-MMM-YYYY") & "' ,SYSDATE)"
    End If
End If
Set RS = Nothing
Set rschk = Nothing
Exit Sub
err:
    MsgBox "error in while status updating", vbInformation
End Sub