Option Explicit
Dim cmbList As String
Dim SqlStr As String
Dim Count_Loop As Integer
Dim rs_Comp As ADODB.Recordset
Dim Rs_GetGRade As ADODB.Recordset
Dim mon_no As Integer
Dim year_no As Integer
Dim date1 As String, Date2 As String
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

Private Sub CmbMonth_Click()
Call cmbMonth_Change
End Sub

Private Sub cmdCancel_Click()
Unload Me
End Sub

Private Sub CMDRESET_Click()
For Count_Loop = 1 To VSGRADE.Rows - 1
    VSGRADE.TextMatrix(Count_Loop, 3) = ""
Next
End Sub
Private Sub CmdSave_Click()
If CmbMonth.Text = "" Then
    MsgBox "select  Valid Month", vbInformation
    Exit Sub
End If
If Cmbyear.Text = "" Then
    MsgBox "select  Valid year", vbInformation
    Exit Sub
End If
MyConn.Execute " delete  from iss_comp_grade where mon_no=" & CmbMonth.ListIndex + 1 & " and year_no=" & Cmbyear.Text & ""

For Count_Loop = 1 To VSGRADE.Rows - 1
    If VSGRADE.TextMatrix(Count_Loop, 3) <> "" Then
        SqlStr = "insert into iss_comp_grade(mon_no,year_no,iss_tag,iss_code,iss_name,grade) values(" & CmbMonth.ListIndex + 1 & "  "
        SqlStr = SqlStr & "," & Cmbyear.Text & " ,'" & VSGRADE.TextMatrix(Count_Loop, 0) & "','" & VSGRADE.TextMatrix(Count_Loop, 1) & "' "
        SqlStr = SqlStr & ",'" & VSGRADE.TextMatrix(Count_Loop, 2) & "' ,'" & VSGRADE.TextMatrix(Count_Loop, 3) & "')"

        MyConn.Execute SqlStr
    End If
''
'' '   If VSGRADE.TextMatrix(Count_Loop, 3) <> "" Then
''        SqlStr = "insert into iss_comp_grade(mon_no,year_no,iss_tag,iss_code,iss_name,grade) values(" & CmbMonth.ListIndex + 1 & "  "
''        SqlStr = SqlStr & "," & Cmbyear.Text & " ,'" & VSGRADE.TextMatrix(Count_Loop, 0) & "','" & VSGRADE.TextMatrix(Count_Loop, 1) & "' "
''        SqlStr = SqlStr & ",'" & VSGRADE.TextMatrix(Count_Loop, 2) & "' ,'A')"
''
''        myconn.Execute SqlStr
'' '   End If

Next

MsgBox "Saved Successfully", vbInformation

End Sub

Private Sub Form_Load()
Me.width = 8040
Me.Height = 5985

For Count_Loop = 1 To 12
    CmbMonth.AddItem MonthName(Count_Loop, True), Count_Loop - 1
Next


For Count_Loop = 0 To 5
    Cmbyear.AddItem Format(Now, "YYYY") - Count_Loop + 1, Count_Loop
Next




VSGRADE.Clear
VSGRADE.Cols = 4
VSGRADE.Rows = 2
VSGRADE.FixedRows = 1

VSGRADE.ColWidth(0) = 0
VSGRADE.ColWidth(1) = 0
VSGRADE.ColWidth(2) = 3500
VSGRADE.ColWidth(3) = 1200

VSGRADE.TextMatrix(0, 0) = "Tag"
VSGRADE.TextMatrix(0, 1) = "Company code"
VSGRADE.TextMatrix(0, 2) = "Name"
VSGRADE.TextMatrix(0, 3) = "Grade"





cmbList = "A|B"
VSGRADE.ColComboList(3) = cmbList

''''SqlStr = " select 'Issue' as tag ,iss_code,iss_name from iss_master where prod_code in (select prod_code from product_master where nature_code='NT003') and upper(iss_name) not like 'NOT IN%'"
''''SqlStr = SqlStr & " union all "
''''SqlStr = SqlStr & " select distinct 'UNIX' as tag, comp_cd as iss_code,company as iss_name from client_entire_unix"
''''SqlStr = SqlStr & " order by iss_name"
''''
''''
''''Set rs_Comp = New ADODB.Recordset
''''rs_Comp.open SqlStr, myconn, adOpenDynamic, adLockOptimistic
''''
''''Count_Loop = 1
''''Do While Not rs_Comp.EOF
''''    VSGRADE.TextMatrix(Count_Loop, 0) = rs_Comp!Tag
''''    VSGRADE.TextMatrix(Count_Loop, 1) = rs_Comp!iss_code
''''    VSGRADE.TextMatrix(Count_Loop, 2) = rs_Comp!iss_name & ""
''''    Count_Loop = Count_Loop + 1
''''    VSGRADE.AddItem ""
''''
''''    rs_Comp.MoveNext
''''Loop
''''rs_Comp.Close


Cmbyear.ListIndex = 0
'CmbMonth.ListIndex = 0


End Sub
