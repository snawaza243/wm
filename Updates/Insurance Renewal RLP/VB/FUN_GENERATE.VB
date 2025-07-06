Private Sub Command1_Click()
On Error GoTo errr
If CmbCompany.Text = "" Then
Else
    comp = Split(CmbCompany.Text, "#")
    MyCompCode = comp(1)
End If
If MsgBox("You are going to Generate Renewal of " & Trim(Left(CmbCompany.Text, 80)), vbInformation + vbOKCancel, strBajaj) = vbCancel Then Exit Sub
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "test", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.WindowShowSearchBtn = True
Sel_Form = ""
If MyCompCode <> "" Then
    Sel_Form = "{bajaj_due_data.mon_no}=" & MyMonth & " and {bajaj_due_data.year_no}=" & MyYear & " And {bajaj_Due_Data.Company_Cd}='" & MyCompCode & "' And {bajaj_Due_Data.importdatatype}='DUEDATA' "
Else
    Sel_Form = "{bajaj_due_data.mon_no}=" & MyMonth & " and {bajaj_due_data.year_no}=" & MyYear & " And {bajaj_Due_Data.importdatatype}='DUEDATA'"
    'Sel_Form = "{bajaj_due_data.mon_no}='" & MyMonth & "' and {bajaj_due_data.year_no}='" & MyYear & "' And {bajaj_Due_Data.importdatatype}='DUEDATA'"
End If
CrystalReport1.SelectionFormula = Sel_Form
CrystalReport1.ReportFileName = App.Path & "\lifereports\Insurance_Renewal Printed.rpt"
CrystalReport1.ParameterFields(0) = "company_name;" & Trim(Left(CmbCompany.Text, 80)) & ";true"
CrystalReport1.ParameterFields(1) = " guserid; " & Glbloginid & "  ; true "
CrystalReport1.WindowShowPrintSetupBtn = True
Call SaveLogIn(Glbloginid, "Transaction>Insurance>Reports", Me.Name)
CrystalReport1.action = 1
CmdMark.Enabled = True
Exit Sub
errr:
MsgBox err.Description
Resume
'MakeDSN "RENEW_EXCEL", "Microsoft Excel Driver (*.xls)", txtFileName.Text, 3
End Sub