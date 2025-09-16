Private Sub cmdreport_Click()
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

If CmbLetterType.Text = "A" Then
    CrystalReport1.ReportFileName = App.Path & "\reports\RenewalLetter_A11.RPT"
Else
    CrystalReport1.ReportFileName = App.Path & "\reports\RenewalLetter_B1.RPT"
End If


Dim monname As String
monname = Format(DTPicker1.Value, "MMM-YY")

'------------------------------------------------------
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "test", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ParameterFields(0) = " mon_name; " & monname & "  ; true "
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowExportBtn = True
    
CrystalReport1.action = 1
Call SaveLogIn(Glbloginid, "", Me.Name)

Exit Sub

End Sub

