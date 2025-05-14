Private Sub cmdprint_Click()
If txtrectno.Text = "" Then
    MsgBox "Please Enter Receipt No First"
    Exit Sub
End If
Dim query As String
Dim CreateQuery As String
query = ""
CreateQuery = ""
query = "select * from npstranreceipt_view"
query = query & " where unique_id='" & txtrectno.Text & "'"
CRPT1.Reset
CRPT1.Connect = MyConn.ConnectionString
CreateQuery = "create or replace view npstranreceipt_view1 as " & query
MyConn.Execute CreateQuery
CRPT1.ReportFileName = App.Path & "\reports\NPSTranReceipt.rpt"
CRPT1.ParameterFields(2) = "guserid;" & Glbloginid & ";true"
CRPT1.ParameterFields(3) = "rundate ; " & Date & " ;true"
CRPT1.WindowShowPrintSetupBtn = True
CRPT1.WindowState = crptMaximized
Call SaveLogIn(Glbloginid, "Reports", Me.Name)
CRPT1.WindowMaxButton = True
CRPT1.WindowShowPrintBtn = True
CRPT1.WindowShowSearchBtn = True
CRPT1.action = 1
Call SaveLogIn(Glbloginid, "", Me.Name)
Exit Sub
errr:
    MsgBox "There is Some Error in Processing AS: " & vbCrLf & err.Description, vbExclamation, strBajaj
    Resume
End Sub