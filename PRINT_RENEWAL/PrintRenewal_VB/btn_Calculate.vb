Private Sub cmdCalc_Click()
On Error Resume Next
mon_no = month(DTPicker1.Value)
year_no = CStr(Format(DTPicker1.Value, "YYYY"))

date1 = "01/" & Format(mon_no, "00") & "/" & year_no
If mon_no = 1 Or mon_no = 3 Or mon_no = 5 Or mon_no = 7 Or mon_no = 8 Or mon_no = 10 Or mon_no = 12 Then
  Date2 = "31/" & Format(mon_no, "00") & "/" & year_no
ElseIf mon_no = 4 Or mon_no = 6 Or mon_no = 9 Or mon_no = 11 Then
  Date2 = "30/" & Format(mon_no, "00") & "/" & year_no
ElseIf mon_no = 2 And year_no Mod 4 = 0 Then
  Date2 = "29/" & Format(mon_no, "00") & "/" & year_no
Else
  Date2 = "28/" & Format(mon_no, "00") & "/" & year_no
End If


'SELECT DISTINCT ([client_cd]), max([Name]), max([add1]), max([add2]), max([City]), max([Pin]), max([CATEG]), max([CO_CODE]), max(ARDate)
'FROM rem
'GROUP BY [client_cd];
If CmbLetterType.Text = "A" Then
    SqlStr = " select a.client_name,a.address1,a.address2,a.city_id,b.grade,A.sourcecode, A.investor_name,A.amount,A.mat_period,A.Tran_code,A.ardate,a.mut_name,a.cheque_no,a.cheque_Date,a.bank_name,a.renewaldate,a.city_name,a.pincode   "
    SqlStr = SqlStr & " from ms_renvel_op_new A,iss_comp_grade B  where A.iss_code=B.iss_code  "
    SqlStr = SqlStr & " and a.renewaldate >= '" & Format(date1, "dd-mmm-yyyy") & "' and a.renewaldate <= '" & Format(Date2, "dd-mmm-yyyy") & "'"
    SqlStr = SqlStr & " and B.mon_no=" & mon_no & " and b.year_no=" & year_no & " "
    SqlStr = SqlStr & " and B.grade='" & CmbLetterType.Text & "'   "
    MyConn.Execute "truncate table RenewalLetter_Vw_sub"
    MyConn.Execute " insert into RenewalLetter_Vw_sub " & SqlStr

    SqlStr = " select max(A.sourcecode) as sourcecode1,max(A.client_name) as client_name1,max(A.address1) as add1,max(A.address2) as add2,max(A.city_id) as city_cd, max(A.mut_name) as mutname,max(A.ardate) as ardate,max(a.city_name) as city_name,max(a.pincode) as pincode "
    SqlStr = SqlStr & " from  RenewalLetter_Vw_sub A  where 1=1"
    'Sqlstr = Sqlstr & " and a.renewaldate >= '" & Format(Date1, "dd-mmm-yyyy") & "' and a.renewaldate <= '" & Format(Date2, "dd-mmm-yyyy") & "'"
    'Sqlstr = Sqlstr & " and B.grade='" & CmbLetterType.Text & "' "
    'Sqlstr = Sqlstr & " and B.mon_no=" & Mon_No & " and b.year_no=" & Year_No & " "
    'Sqlstr = Sqlstr & " and a.naturecode='NT003'  "
    SqlStr = SqlStr & " group by sourcecode"
    
    MyConn.Execute "truncate table RenewalLetter_Vw"
    MyConn.Execute " insert into RenewalLetter_Vw " & SqlStr
End If

If CmbLetterType.Text = "B" Then
    SqlStr = " select a.client_name,a.address1,a.address2,a.city_id,b.grade,A.sourcecode, A.investor_name,A.amount,A.mat_period,A.Tran_code,A.ardate,a.mut_name,a.cheque_no,a.cheque_Date,a.bank_name,a.renewaldate,a.city_name,a.pincode   "
    SqlStr = SqlStr & " from ms_renvel_op_new A,iss_comp_grade B  where A.iss_code=B.iss_code  "
    SqlStr = SqlStr & " and a.renewaldate >= '" & Format(date1, "dd-mmm-yyyy") & "' and a.renewaldate <= '" & Format(Date2, "dd-mmm-yyyy") & "'"
    SqlStr = SqlStr & " and B.mon_no=" & mon_no & " and b.year_no=" & year_no & " "
    SqlStr = SqlStr & " and B.grade='" & CmbLetterType.Text & "'   "
    MyConn.Execute "truncate table RenewalLetter_Vw_sub_B"
    MyConn.Execute "  insert into RenewalLetter_Vw_sub_B  " & SqlStr

End If
MsgBox "Calculated Successfully", vbInformation

Exit Sub
End Sub

