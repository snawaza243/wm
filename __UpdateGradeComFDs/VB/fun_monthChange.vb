Private Sub cmbMonth_Change()
Call CMDRESET_Click
mon_no = CmbMonth.ListIndex + 1
year_no = Cmbyear.Text

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

SqlStr = " select distinct '' as tag ,iss_code,mut_name from ms_renvel_op where  (naturecode='NT003' or PCODE='DT010') and renewaldate>='" & Format(date1, "dd-mmm-yyyy") & "' and  renewaldate<='" & Format(Date2, "dd-mmm-yyyy") & "'  order by mut_name"

Set rs_Comp = New ADODB.Recordset
rs_Comp.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic

Count_Loop = 1
Do While Not rs_Comp.EOF
    VSGRADE.TextMatrix(Count_Loop, 0) = rs_Comp!Tag & ""
    VSGRADE.TextMatrix(Count_Loop, 1) = rs_Comp!iss_code
    VSGRADE.TextMatrix(Count_Loop, 2) = rs_Comp!mut_name & ""
    Count_Loop = Count_Loop + 1
    VSGRADE.AddItem ""
    
    rs_Comp.MoveNext
Loop
rs_Comp.Close




SqlStr = " select iss_Code,grade from iss_comp_grade where mon_no=" & CmbMonth.ListIndex + 1 & " and Year_no =" & Cmbyear.Text & "   "
Set Rs_GetGRade = New ADODB.Recordset
Rs_GetGRade.open SqlStr, MyConn, adOpenDynamic, adLockOptimistic
If Rs_GetGRade.EOF = False Then Rs_GetGRade.MoveFirst

Count_Loop = 1
For Count_Loop = 1 To VSGRADE.Rows - 1
    If Not Rs_GetGRade.EOF Then
        Rs_GetGRade.MoveFirst
        Rs_GetGRade.find "iss_Code='" & VSGRADE.TextMatrix(Count_Loop, 1) & "'"
        If Not Rs_GetGRade.EOF Then
            VSGRADE.TextMatrix(Count_Loop, 3) = Rs_GetGRade!grade
        Else
            VSGRADE.TextMatrix(Count_Loop, 3) = ""
        End If
        Rs_GetGRade.MoveFirst
    End If
Next

Rs_GetGRade.Close

End Sub

