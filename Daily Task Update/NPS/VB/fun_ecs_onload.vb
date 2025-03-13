Private Sub Form_Load()
cmbCompany.Clear
OptNPS.Value = True
'OptLife.Value = False
OptGeneral.Value = False
Dim Rs_company As New ADODB.Recordset
'OptLife.Value = True
Rs_company.open "select iss_name,iss_code from iss_master where iss_code='IS02520'", MyConn, adOpenDynamic, adLockOptimistic
cmbCompany.Clear

Do While Not Rs_company.EOF
    cmbCompany.AddItem Rs_company!iss_name & Space(60) & "#" & Rs_company!iss_code
    Rs_company.MoveNext
Loop
cmbCompany.ListIndex = 0
Rs_company.Close
Set Rs_Status = New ADODB.Recordset
Rs_Status.open " select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status", MyConn, adOpenDynamic, adLockReadOnly
CmbStatus.Clear
Do While Not Rs_Status.EOF
   CmbStatus.AddItem Rs_Status!status & Space(60) & "#" & Rs_Status!status_cd
   Rs_Status.MoveNext
Loop
If CmbStatus.ListCount > 0 Then CmbStatus.ListIndex = 0
Rs_Status.Close

End Sub