ElseIf OptPaid.Value = True Then
    Do While Not rsExcel.EOF
        If rsExcel("" & Excel_Comp & "").Value & "" <> "" Then
            If (CmbDataType.ListIndex = 2 Or CmbDataType.ListIndex = 3) Then 'paid and reinstate
                If rslap.State = 1 Then rslap.Close
                rslap.open "select POLICY_NO  from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " AND IMPORTDATATYPE='" & MyImportDataType & "'", MyConn
                If rslap.EOF Then
                    SqlStr = "Insert into BAJAJ_DUE_DATA (" & dataBaseField & ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG)  Values("
                    For Count_Loop = 0 To rsExcel.Fields.count - 1
                       If InStr(1, rsExcel(Count_Loop), "'") > 0 Then
                           Field_value = Replace(rsExcel(Count_Loop), "'", "")
                           SqlStr = SqlStr & "'" & Field_value & "',"
                       Else
                           If rsExcel(Count_Loop).Type = 7 Then
                               SqlStr = SqlStr & "'" & Format(rsExcel(Count_Loop), "dd-mmm-yyyy") & "',"
                           Else
                               SqlStr = SqlStr & "'" & Trim(rsExcel(Count_Loop)) & "',"
                           End If
                       End If
                    Next
                    SqlStr = SqlStr & " " & CmbMonth.ListIndex + 1 & "," & TxtYear.Text & ",'" & Glbloginid & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & MyImportDataType & "' "
                    SqlStr = SqlStr & ",NULL,'FORCE FULL')"
                    SqlStr = Replace(SqlStr, "''", "Null")
                    MyConn.Execute SqlStr
                    DoEvents
                 End If
            End If
            SqlStr = " select * from bajaj_paid_Data WHERE upper(trim(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'  and mon_no=" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' "
            Set Rs_Chk_Excel = New ADODB.Recordset
            Rs_Chk_Excel.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
            
            If Rs_Chk_Excel.EOF = True Then
                Dim xy As Integer
                xy = 0
                If RsHeader.State = 1 Then RsHeader.Close
                RsHeader.open "Select policy_no,company_cd,last_status from policy_details_master WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' ", MyConn, adOpenStatic, adLockOptimistic
                If RsHeader.EOF = False Then
                    If UCase(RsHeader.Fields("last_status")) = "L" Then
                          If UCase(rsExcel("" & Excel_Status & "")) = "PAID" Then
                               SqlStr = " update policy_details_master set last_status='R',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=to_date('" & Format(ServerDateTime, "DD/MM/YYYY") & "','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' "
                               MyConn.Execute SqlStr
                          End If
                    End If
                Else
                    MyConn.Execute ("insert into PolicyNotInHeader(policy_no,company_Cd) values(('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "'),'" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "')")
                End If
           
                If rslap.State = 1 Then rslap.Close
                rslap.open "select max(to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')) from_dt from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and status_cd='LAPSED'", MyConn
                If Not IsNull(rslap(0)) Then
                    SqlStr = "update bajaj_due_Data set status_Cd='" & UCase(Trim(rsExcel("" & Excel_Status & ""))) & "',last_update_dt='" & Format(ServerDateTime, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and importdatatype='" & MyImportDataType & "' AND to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')>'" & Format(rslap(0), "dd-MMM-yyyy") & "' and to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')<=last_day(to_date('01-" & CmbMonth.Text & "-" & TxtYear.Text & "','dd-mm-yyyy')) "
                Else
                    SqlStr = "update bajaj_due_Data set status_Cd='" & UCase(Trim(rsExcel("" & Excel_Status & ""))) & "',last_update_dt='" & Format(ServerDateTime, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=upper(trim('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and importdatatype='" & MyImportDataType & "' AND mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & ""
                End If
                MyConn.Execute SqlStr, xy
                If UCase(Trim(rsExcel("" & Excel_Status & ""))) = "PAID" And xy > 0 Then  'Jis Month Ka Paid Data Hai Agara Us Month Kak Due Data Update Ho gaya
                    SqlStr = "Insert into BAJAJ_PAID_DATA (" & dataBaseField & ",Mon_no,Year_No,UserId,Import_Dt,importdatatype)  Values("
                    For Count_Loop = 0 To rsExcel.Fields.count - 1
                        If InStr(1, rsExcel(Count_Loop), "'") > 0 Then
                            Field_value = Replace(rsExcel(Count_Loop), "'", "")
                            SqlStr = SqlStr & "'" & Field_value & "',"
                        Else
                            If rsExcel(Count_Loop).Type = 7 Then
                                SqlStr = SqlStr & "'" & Format(rsExcel(Count_Loop), "dd-mmm-yyyy") & "',"
                            Else
                                SqlStr = SqlStr & "'" & Trim(rsExcel(Count_Loop)) & "',"
                            End If
                        End If
                    Next
                    SqlStr = SqlStr & "" & CmbMonth.ListIndex + 1 & "," & TxtYear.Text & ",'" & Glbloginid & "','" & Format(ServerDateTime, "dd-mmm-yyyy") & "','" & MyImportDataType & "' "
                    SqlStr = SqlStr & ")"
                    SqlStr = Replace(SqlStr, "''", "Null")
                    Rec_Count = Rec_Count + 1
                    MyConn.Execute SqlStr
                    MyConn.Execute "update policy_details_master set last_status='A',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=to_date('" & Format(ServerDateTime, "DD/MM/YYYY") & "','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "'"
                    Call update_bajajar_status(UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)), UCase(Trim(rsExcel("" & Excel_Comp & "").Value)), "F", "PAID DATA")
                    Label5.Caption = Rec_Count
                    DoEvents

                End If
            Else
                MyConn.Execute SqlStr
                SqlStr = " update bajaj_paid_Data set status_Cd='" & UCase(Trim(rsExcel("" & Excel_Status & ""))) & "',last_update_dt='" & Format(Now, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('" & UCase(Trim(rsExcel("" & Excel_policy_no & "").Value)) & "')) and upper(trim(company_cd))= '" & UCase(Trim(rsExcel("" & Excel_Comp & "").Value)) & "' and mon_no=" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' "
                MyConn.Execute SqlStr
                rec1 = rec1 + 1
                Label7 = rec1
                DoEvents
            End If
            Rs_Chk_Excel.Close
            DoEvents
        End If
        Label9 = Rec_Count_exl
        Rec_Count_exl = Rec_Count_exl + 1
        rsExcel.MoveNext
    Loop
    SqlStr = " select  policy_no from ( select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_paid_Data b where"
    SqlStr = SqlStr & " upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = " & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "'"
    SqlStr = SqlStr & " group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(A.company_cd)>1)"
    Set Rs_Dup_policy = New ADODB.Recordset
    Rs_Dup_policy.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
    Dup_Policy_Str = ""
    MyConn.Execute "DELETE FROM DUP_POLICY"
    Do While Not Rs_Dup_policy.EOF
        MyConn.Execute "INSERT INTO DUP_POLICY VALUES('" & Rs_Dup_policy!policy_no & "')"
        Dup_Policy_Str = Dup_Policy_Str & "'" & Rs_Dup_policy!policy_no & "'" & ","
        Rs_Dup_policy.MoveNext
    Loop
    If Dup_Policy_Str <> "" Then Dup_Policy_Str = Left(Dup_Policy_Str, Len(Dup_Policy_Str) - 1)
    Rec_Del = 0
    If Dup_Policy_Str <> "" Then
        MyConn.Execute " update bajaj_paid_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no =" & CmbMonth.ListIndex + 1 & " and year_no=" & TxtYear.Text & " and importdatatype='" & MyImportDataType & "' ", Rec_Del
    End If
    Rec_Count = Rec_Count - Rec_Del
    MyConn.Execute " update bajaj_paid_data A set A.emp_no " & _
    " =(select MAX(B.emp_no) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1), " & _
    " inv_cd=(select MAX(B.client_Cd) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no  AND B.FRESH_RENEWAL=1) " & _
    "where mon_no= " & CmbMonth.ListIndex + 1 & " and year_no= " & TxtYear.Text & " and dup_Rec is null and importdatatype='" & MyImportDataType & "'"
End If 'End Of Main If