Dim Form_name As String
Dim MyMonth As Integer
Dim MyYear As Integer
Dim comp() As String
Dim MyCompCode As String
Private Sub CmbMonth_Click()
MyMonth = CmbMonth.ListIndex + 1
End Sub
Private Sub Cmbyear_Click()
    MyYear = CmbYear.List(CmbYear.ListIndex)
End Sub

Private Sub CmdMark_Click()
If MsgBox("You are going to Mark", vbInformation + vbOKCancel, strBajaj) = vbCancel Then Exit Sub
If cmbCompany.Text = "" Then
Else
    comp = Split(cmbCompany.Text, "#")
    MyCompCode = comp(1)
End If
If MyCompCode <> "" Then
   MyConn.Execute "Update bajaj_due_data set rem_flage='YES' WHERE COMPANY_CD='" & MyCompCode & "' AND MON_NO=" & MyMonth & " AND YEAR_NO=" & MyYear & " AND upper(trim(PAY_MODE))='NON ECS' AND IMPORTDATATYPE='DUEDATA'"
Else
   MyConn.Execute "Update bajaj_due_data set rem_flage='YES' WHERE   MON_NO=" & MyMonth & " AND YEAR_NO=" & MyYear & " AND upper(trim(PAY_MODE='NON ECS')) AND IMPORTDATATYPE='DUEDATA' "
End If
End Sub
Private Sub CMDRESET_Click()
Form_Load
End Sub
Private Sub Command1_Click()
On Error GoTo errr
If cmbCompany.Text = "" Then
Else
    comp = Split(cmbCompany.Text, "#")
    MyCompCode = comp(1)
End If
If MsgBox("You are going to Generate Renewal of " & Trim(Left(cmbCompany.Text, 80)), vbInformation + vbOKCancel, strBajaj) = vbCancel Then Exit Sub
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
CrystalReport1.ParameterFields(0) = "company_name;" & Trim(Left(cmbCompany.Text, 80)) & ";true"
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


Private Sub Command3_Click()
Unload Me
End Sub

Private Sub Form_Load()
On Error GoTo errr
Me.Icon = LoadPicture(App.Path & "\w.ico")
Form_name = "Transaction>Insurance>Reports>" & Me.Caption
Dim RS As New ADODB.Recordset
RS.open "select company_name,company_cd from bajaj_company_master where catagory='L' order by company_name", MyConn
cmbCompany.Clear
Do While Not RS.EOF
    cmbCompany.AddItem RS(0) & Space(60) & "#" & RS(1)
    RS.MoveNext
Loop
RS.Close
Set RS = Nothing
CmbMonth.Clear
For i = 1 To 12
    CmbMonth.AddItem MonthName(i)
Next
CmbYear.Clear
For i = 0 To 6 Step 1
    CmbYear.AddItem year(Now) - i + 1
Next
Exit Sub
errr:
MsgBox err.Description
End Sub

Private Sub Form_Unload(Cancel As Integer)
Call SaveLogOut(Glbloginid, Me.Name)
End Sub
