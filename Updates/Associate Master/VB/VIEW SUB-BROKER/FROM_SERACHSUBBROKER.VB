Dim rsSB As ADODB.Recordset
Dim Ph As Variant
Dim Pcode As Variant
Sub DisplaySB()
Set rsSB = New ADODB.Recordset
Dim i As Integer
Dim j As Integer
With dataview
    .Cols = 63
    .Row = 0
    .Col = 0
    .ColWidth(0) = 0
    .Font.Bold = True
    .Text = "Sub Broker Code"
    .Col = 1
    .ColWidth(1) = 1500
    .Font.Bold = True
    .Text = "Sub Broker Name"
    .Col = 2
    .ColWidth(2) = 1500
    .Font.Bold = True
    .Text = "First Address"
    .Col = 3
    .ColWidth(3) = 1500
    .Font.Bold = True
    .Text = "Second Address"
    .Col = 4
    .ColWidth(4) = 0
    .Font.Bold = True
    .Text = "City"
    .Col = 5
    .ColWidth(5) = 1500
    .Font.Bold = True
    .Text = "Phone"
    .Col = 6
    .ColWidth(6) = 1500
    .Font.Bold = True
    .Text = "Mobile"
    .Col = 7
    .ColWidth(7) = 1500
    .Font.Bold = True
    .Text = "Fax"
    .Col = 8
    .ColWidth(8) = 1500
    .Font.Bold = True
    .Text = "Email"
    .Col = 9
    .ColWidth(9) = 1500
    .Font.Bold = True
    .Text = "Contact Person"
    .Col = 10
    .ColWidth(10) = 1500
    .Font.Bold = True
    .Text = "Enrollment Date"
    .Col = 11
    .ColWidth(11) = 0
    .Font.Bold = True
    .Text = "Category"
    .Col = 12
    .ColWidth(12) = 1500
    .Font.Bold = True
    .Text = "Source Code"
    .Col = 13
    .ColWidth(13) = 1100
    .Font.Bold = True
    .Text = "Date Of Birth"
    .Col = 14
    .ColWidth(14) = 900
    .Font.Bold = True
    .Text = "Status Code"
    .Col = 15
    .ColWidth(15) = 1500
    .Font.Bold = True
    .Text = "Pan No."
    .Col = 16
    .ColWidth(16) = 1500
    .Font.Bold = True
    .Text = "Dist"
    .Col = 17
    .ColWidth(17) = 1000
    .Font.Bold = True
    .Text = "AMFI Certificate"
    .Col = 18
    .ColWidth(18) = 1100
    .Font.Bold = True
    .Text = "Registration No."
    .Col = 19
    .ColWidth(19) = 900
    .Font.Bold = True
    .Text = "AMFI ID"
    .Col = 20
    .ColWidth(20) = 1200
    .Font.Bold = True
    .Text = "Contact Person Email"
    .Col = 21
    .ColWidth(21) = 0
    .Font.Bold = True
    .Text = "Bank Code"
    .Col = 22
    .ColWidth(22) = 1200
    .Font.Bold = True
    .Text = "Account Type"
    .Col = 23
    .ColWidth(23) = 1100
    .Font.Bold = True
    .Text = "Web Site"
    .Col = 24
    .ColWidth(24) = 0
    .Font.Bold = True
    .Text = "Employee Code"
    .Col = 25
    .ColWidth(25) = 1200
    .Font.Bold = True
    .Text = "Payment Mode"
    .Col = 26
    .ColWidth(26) = 1200
    .Font.Bold = True
    .Text = "Remark"
    .Col = 27
    .ColWidth(27) = 1200
    .Font.Bold = True
    .Text = "Account No"
    .Col = 28
    .ColWidth(28) = 0
    '.Font.Bold = True
'    .Text = "Loged user code"
    .Col = 29
    .ColWidth(29) = 1000
    .Font.Bold = True
    .Text = "Exist Code"
    .Col = 30
    .ColWidth(30) = 1500
    .Font.Bold = True
    .Text = "Sub Broker Source"
    .Col = 31
    .ColWidth(31) = 1500
    .Font.Bold = True
    .Text = "TDS Percentage"
    .Col = 32
    .ColWidth(32) = 0
     .Col = 33
    .ColWidth(33) = 0
    .Col = 34
    .ColWidth(34) = 0
    .Col = 35
    .ColWidth(35) = 1300
    .Font.Bold = True
    .Text = "Loged user code"
    .Col = 36
    .ColWidth(36) = 2500
    .Font.Bold = True
    .Text = "Creation Time"
    .Col = 37
    .ColWidth(37) = 2500
    .Font.Bold = True
    .Text = "Pin Code"
    .Col = 38
    .ColWidth(38) = 2500
    .Font.Bold = True
    .Text = "Sub Broker Type"
    
    .Text = "Residence Address1"
    .Col = 45
    .ColWidth(45) = 0
    .Font.Bold = True
    .Text = "Residence Address2"
    .Col = 46
    .ColWidth(46) = 0
    .Font.Bold = True
    .Text = "Residence City"
    .Col = 47
    .ColWidth(47) = 0
    .Text = "Residence State"
    .Col = 48
    .ColWidth(48) = 0
    .Text = "Residence Pin"
    .Col = 49
    .ColWidth(49) = 0
    .Col = 50
    .ColWidth(50) = 0
    .Text = "MICR No."
    End With
If rsSB.State = 1 Then rsSB.Close
rsSB.CursorLocation = adUseClient
rsSB.open "select * from agent_master", MyConn, adOpenForwardOnly, adLockReadOnly
If rsSB.RecordCount > 0 Then
    dataview.Rows = rsSB.RecordCount + 2
    With dataview
        For i = 0 To rsSB.RecordCount - 1
            For j = 0 To rsSB.Fields.count - 2
                .Row = i + 1
                .Col = j
                If IsNull(rsSB(j)) = True Then
                .Text = ""
                Else
                .Text = rsSB(j)
                End If
            Next
            rsSB.MoveNext
        Next
    End With
End If
If rsSB.State = 1 Then rsSB.Close
End Sub
Private Sub cboCity_KeyPress(KeyAscii As MSForms.ReturnInteger)
    If KeyAscii = 13 Then SendKeys "{Tab}"
End Sub
Private Sub cboCity_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cboCity.MatchFound = False Then
        cboCity.ListIndex = -1
    End If
End Sub
Private Sub cmdExit_Click()
Unload Me
End Sub

Private Sub CMDRESET_Click()
    txtMobile.Text = ""
    Txtcode.Text = ""
    Txtname.Text = ""
    txtPan.Text = ""
    txtPhone.Text = ""
    cboCity.ListIndex = -1
    dataview.Clear
    dataview.FixedRows = 1
    dataview.FixedCols = 0
    dataview.Rows = 2
    dataview.Cols = 2
    Call Drawgrid
    Label7.Caption = ""
End Sub
Private Sub cmdSearch_Click()
Dim StrSql As String
If cboCity.Text = "" And Txtcode.Text = "" And Txtname.Text = "" And txtMobile.Text = "" And txtPhone.Text = "" And txtPan.Text = "" And cmbBr.Text = "" Then
    MsgBox "Please Select/Enter Searching Parameter", vbInformation
    cboCity.SetFocus
    Exit Sub
End If
    StrSql = "select * from agent_master where 1=1"
    If cboCity.ListIndex <> -1 Then
        StrSql = StrSql & " and city_id='" & cboCity.List(cboCity.ListIndex, 1) & "'"
    End If
    If cmbBr.ListIndex <> -1 Then
        StrSql = StrSql & " and sourceid='" & cmbBr.List(cmbBr.ListIndex, 1) & "'"
    End If
    If Txtcode.Text <> "" Then
        StrSql = StrSql & " and EXIST_CODE like '%" & Trim(Txtcode.Text) & "%'"
    End If
    If Txtname.Text <> "" Then
        StrSql = StrSql & " and Upper(agent_name) like '%" & UCase(Trim(Txtname.Text)) & "%'"
    End If
    If txtMobile.Text <> "" Then
        StrSql = StrSql & " and mobile like '%" & Trim(txtMobile.Text) & "%'"
    End If
    If txtPhone.Text <> "" Then
        StrSql = StrSql & " and phone like '%" & Trim(txtPhone.Text) & "%'"
    End If
    If txtPan.Text <> "" Then
        StrSql = StrSql & " and Upper(pan) like '%" & UCase(Trim(txtPan.Text)) & "%'"
    End If
    'vinod security Purpose 3 may
      If Trim(SRmCode) <> "" Then
        StrSql = StrSql & " and sourceid in (" & Branches & ") and (rm_code=" & SRmCode & ")"
      Else
        StrSql = StrSql & " and sourceid in (" & Branches & ")"
      End If
    'vinod security Purpose 3 may
    StrSql = StrSql & " order by agent_name"
    Call Search(StrSql)
    Me.MousePointer = 0
    dataview.SetFocus
End Sub
Private Sub Search(StrSql As String)
On Error Resume Next
Set rsSB = New ADODB.Recordset
Dim i As Integer
Dim j As Integer
If rsSB.State = 1 Then rsSB.Close
rsSB.CursorLocation = adUseClient
rsSB.open StrSql, MyConn, adOpenForwardOnly, adLockReadOnly
If rsSB.Fields("address3") <> " " Then
    MyAddress3 = rsSB.Fields("address3")
Else
     MyAddress3 = ""
End If
Label7.Caption = " " & rsSB.RecordCount & " Record(s) Found "
Me.MousePointer = 11
If rsSB.RecordCount > 0 Then
    dataview.Rows = rsSB.RecordCount + 1
    With dataview
        For i = 0 To rsSB.RecordCount - 1
            For j = 0 To rsSB.Fields.count - 1
                .Row = i + 1
                .Col = j
                If Not IsNull(rsSB(j)) Then
                    .Text = rsSB(j)
                Else
                    .Text = ""
                End If
            Next
            rsSB.MoveNext
        Next
    End With
End If
If rsSB.State = 1 Then rsSB.Close
End Sub
Private Sub dataview_DblClick()
Dim i As Integer
Dim rsCity As ADODB.Recordset
Dim rsState As ADODB.Recordset
Dim RSCAT As ADODB.Recordset
Dim rsExam As ADODB.Recordset
Dim rsBank As ADODB.Recordset
Dim rsLoc As ADODB.Recordset
Dim rsEx As ADODB.Recordset
Dim rsSr As ADODB.Recordset
Dim rsRm As ADODB.Recordset
Dim MyRs As ADODB.Recordset
Dim rsagent As New ADODB.Recordset
Dim Mycat As String
Dim rsSubType As New ADODB.Recordset
frmSubBrokerMaster.Frame4.Visible = False
Dim j As Integer
    frmSubBrokerMaster.OptSB.Value = True
    With dataview
       For i = 0 To .Cols - 1
            If i = 0 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtSCode = .TextMatrix(.RowSel, i)
                 Mycat = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtSCode = ""
               End If
               '----change here----
                Set MyRs = New ADODB.Recordset
                MyRs.open "select b.anastatus,a.activationdate from advisorsubentry a,advisorstatus b where a.ANACODE='" & .TextMatrix(.RowSel, i) & "' and  a.STATUS=b.ANASTATUS_ID ", MyConn, adOpenForwardOnly
                If Not MyRs.EOF Then
                    frmSubBrokerMaster.Label82.Caption = MyRs(0)
                    If Not IsNull(MyRs(1)) Then
                        frmSubBrokerMaster.LblActivationDt.Caption = MyRs(1)
                    End If
                Else
                    frmSubBrokerMaster.Label82.Caption = "NOT DEFINED"
                    frmSubBrokerMaster.LblActivationDt.Caption = "NOT ACTIVE"
                End If
                MyRs.Close
                Set MyRs = Nothing
                End If
            If i = 1 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.Txtname = .TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.Txtname = ""
               End If
            End If
            If i = 2 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtAdd1 = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtAdd1 = ""
               End If
            End If
            If i = 3 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.txtAdd2 = .TextMatrix(.RowSel, i)
                  frmSubBrokerMaster.txtadd3 = MyAddress3
               Else
                  frmSubBrokerMaster.txtAdd2 = ""
                  frmSubBrokerMaster.txtadd3 = ""
               End If
            End If
            If i = 4 Then
               Set rsCity = New ADODB.Recordset
               If rsCity.State = 1 Then rsCity.Close
                  rsCity.CursorLocation = adUseClient
                 rsCity.open "select city_name,state_id,pincode from city_master where city_id='" & Trim(.TextMatrix(.RowSel, i)) & "'", MyConn, adOpenDynamic, adLockPessimistic
               If rsCity.RecordCount > 0 Then
                If rsCity(0) <> "" Then
                 frmSubBrokerMaster.cmbCity = Trim(rsCity(0))
                End If
               End If
               Set rsState = New ADODB.Recordset
               If rsState.State = 1 Then rsState.Close
                  rsState.CursorLocation = adUseClient
              If Not rsCity.EOF Then
                 rsState.open "select state_name from state_master where state_id='" & rsCity(1) & "'", MyConn, adOpenDynamic, adLockPessimistic
               If rsState.RecordCount > 0 Then
                If rsState(0) <> "" Then
                 frmSubBrokerMaster.txtstate = Trim(rsState(0))
                End If
               Else
                 frmSubBrokerMaster.txtstate = ""
               End If
              End If
            End If
            If i = 28 Then
               Set rsLoc = New ADODB.Recordset
               If rsLoc.State = 1 Then rsLoc.Close
                  rsLoc.CursorLocation = adUseClient
                 rsLoc.open "select location_name,pincode from location_master where location_id='" & Trim(.TextMatrix(.RowSel, i)) & "'", MyConn, adOpenDynamic, adLockPessimistic
               If rsLoc.RecordCount > 0 Then
                If rsLoc(0) <> "" Then
                 frmSubBrokerMaster.cmbloc = Trim(rsLoc(0))
                End If
                 If rsLoc(1) <> "" Then
                 frmSubBrokerMaster.txtPin = Trim(rsLoc(1))
                 End If
               Else
                 frmSubBrokerMaster.cmbloc = ""
               End If
            End If
            If i = 5 Then
                If .TextMatrix(.RowSel, i) <> "" Then
                 Ph = Split(.TextMatrix(.RowSel, i), "#")
                 frmSubBrokerMaster.Phone.Rows = UBound(Ph) + 2
                 frmSubBrokerMaster.Phone.TextMatrix(0, 0) = "STD"
                 frmSubBrokerMaster.Phone.TextMatrix(0, 1) = "Phone"
                For j = 0 To UBound(Ph)
                  If InStr(1, Ph(j), "-", vbTextCompare) > 0 Then
                    Pcode = Split(Ph(j), "-")
                    frmSubBrokerMaster.Phone.TextMatrix(j + 1, 0) = Pcode(0)
                    frmSubBrokerMaster.Phone.TextMatrix(j + 1, 1) = Pcode(1)
                 Else
                    If Ph(j) <> "" Then
                    Pcode = Split(Ph(j), "-")
                    frmSubBrokerMaster.Phone.TextMatrix(j + 1, 0) = ""
                    frmSubBrokerMaster.Phone.TextMatrix(j + 1, 1) = Pcode(0)
                    End If
                 End If
                Next
               End If
            End If
            If i = 6 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                frmSubBrokerMaster.txtMob = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtMob = ""
               End If
            End If
            If i = 7 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.TxtFax = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.TxtFax = ""
               End If
            End If
            If i = 8 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtEmail = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtEmail = ""
               End If
            End If
            If i = 9 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtCPer = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtCPer = ""
               End If
            End If
            If i = 10 Then
             If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.SbDate = Format(.TextMatrix(.RowSel, i), "dd/mm/yyyy")
             End If
            End If
            '----change here----
            If i = 11 Then
              Set RSCAT = New ADODB.Recordset
                If RSCAT.State = 1 Then RSCAT.Close
                RSCAT.open "select businesspack from advisorsubentry where ANACODE='" & Mycat & "'", MyConn, adOpenForwardOnly
                If RSCAT.RecordCount > 0 Then
                  If RSCAT(0) <> "" Then
                    frmSubBrokerMaster.cmbSCat = Trim(RSCAT(0))
                  End If
                Else
                    frmSubBrokerMaster.cmbSCat = ""
                End If
            End If
            If i = 12 Then
              Set rsSr = New ADODB.Recordset
                If rsSr.State = 1 Then rsSr.Close
                    rsSr.open "select branch_name,branch_code from branch_master where branch_code='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                If rsSr.RecordCount > 0 Then
                  If rsSr(0) <> "" And rsSr(1) <> "" Then
                    frmSubBrokerMaster.cmbSource = Trim(rsSr(0))
                  End If
                Else
                    frmSubBrokerMaster.cmbSource = ""
                End If
            End If
            If i = 13 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.dob = Format(.TextMatrix(.RowSel, i), "dd/mm/yyyy")
               End If
            End If
            If i = 14 Then
              Set rsSr = New ADODB.Recordset
                If rsSr.State = 1 Then rsSr.Close
                   If .TextMatrix(.RowSel, i) = "" Then
                    .TextMatrix(.RowSel, i) = 0
                   End If
                    rsSr.open "select itemname from fixeditem where itemserialnumber='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                If rsSr.RecordCount > 0 Then
                   If rsSr(0) <> "" Then
                    frmSubBrokerMaster.cmbstatus = Trim(rsSr(0))
                   End If
                Else
                    frmSubBrokerMaster.cmbstatus = ""
                End If
            End If
            If i = 15 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtPan = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtPan = ""
               End If
            End If
            If i = 16 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.cmbDist = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.cmbDist = ""
               End If
            End If
            If i = 17 Then
                If Trim(.TextMatrix(.RowSel, i)) = "Y" Then
                 frmSubBrokerMaster.chkPassed = 1
                Else
                 frmSubBrokerMaster.chkPassed = 0
                End If
            End If
            If i = 18 Then
              Set rsEx = New ADODB.Recordset
                If rsEx.State = 1 Then rsEx.Close
                If .TextMatrix(.RowSel, i) = "" Then
                    .TextMatrix(.RowSel, i) = 0
                End If
                    rsEx.open "select itemname from fixeditem where itemserialnumber='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                If rsEx.RecordCount > 0 Then
                  If rsEx(0) <> "" Then
                    frmSubBrokerMaster.cmbExams = Trim(rsEx(0))
                  End If
                Else
                    frmSubBrokerMaster.cmbExams = ""
                End If
            End If
            If i = 19 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtID = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtID = ""
               End If
            End If
            If i = 20 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtCPEmail = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtCPEmail = ""
               End If
            End If
            
            If i = 21 Then
              Set rsBank = New ADODB.Recordset
                If rsBank.State = 1 Then rsBank.Close
                    rsBank.open "select bank_name,bank_id from ana_bank_master where bank_id='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                    ''rsBankbranch.open "select bank_name,branch from bank_master where bank_id='" & .TextMatrix(.RowSel, i) & "'", myconn, adOpenDynamic, adLockPessimistic
                If rsBank.RecordCount > 0 Then
                  If rsBank(0) <> "" Then
                    frmSubBrokerMaster.CmbBank = Trim(rsBank(0)) & Space(80) & "#" & rsBank(1)
                  End If
''                  If rsBank(1) <> "" Then
''                    frmSubBrokerMaster.cmbBB = Trim(rsBank(1))
''                  End If
                Else
                    frmSubBrokerMaster.CmbBank.ListIndex = -1
                    frmSubBrokerMaster.cmbBB.ListIndex = -1
                End If
            End If
            
''            If i = 21 Then
''              Set rsBank = New ADODB.Recordset
''                If rsBank.State = 1 Then rsBank.Close
''                    rsBank.open "select bank_name,branch from ANA_bank_master where bank_id='" & .TextMatrix(.RowSel, i) & "'", myconn, adOpenDynamic, adLockPessimistic
''                If rsBank.RecordCount > 0 Then
''                  If rsBank(0) <> "" Then
''                    frmSubBrokerMaster.cmbBank.Text = Trim(rsBank(0))
''                  End If
''                  If rsBank(1) <> "" Then
''                    frmSubBrokerMaster.cmbbankcity.Text = Trim(rsBank(1))
''                  End If
''                Else
''                    frmSubBrokerMaster.cmbBank.ListIndex = -1
''                    frmSubBrokerMaster.cmbBB.ListIndex = -1
''                End If
''            End If
            If i = 22 Then
              Set rsExam = New ADODB.Recordset
                If rsExam.State = 1 Then rsExam.Close
                If .TextMatrix(.RowSel, i) = "" Then
                    .TextMatrix(.RowSel, i) = 0
                End If
                    rsExam.open "select itemname from fixeditem where itemserialnumber='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                If rsExam.RecordCount > 0 Then
                  If rsExam(0) <> "" Then
                    frmSubBrokerMaster.cmbAcType = Trim(rsExam(0))
                  End If
                Else
                    frmSubBrokerMaster.cmbAcType = ""
                End If
            End If
            If i = 23 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtWebSite = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtWebSite = ""
               End If
            End If
            If i = 24 Then
              Set rsRm = New ADODB.Recordset
                If rsRm.State = 1 Then rsRm.Close
                    rsRm.open "select rm_name,rm_code from employee_master where rm_code='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                If rsRm.RecordCount > 0 Then
                  If rsRm(0) <> "" And rsRm(1) <> "" Then
                    frmSubBrokerMaster.cmbrm = Trim(rsRm(0))
                  End If
                Else
                    frmSubBrokerMaster.cmbrm = ""
                End If
            End If
            If i = 25 Then
              Set rsExam = New ADODB.Recordset
                If rsExam.State = 1 Then rsExam.Close
                If .TextMatrix(.RowSel, i) = "" Then
                   .TextMatrix(.RowSel, i) = 0
                End If
                    rsExam.open "select itemname from fixeditem where itemserialnumber='" & .TextMatrix(.RowSel, i) & "'", MyConn, adOpenDynamic, adLockPessimistic
                If rsExam.RecordCount > 0 Then
                  If rsExam(0) <> "" Then
                    frmSubBrokerMaster.cmbPayMode = Trim(rsExam(0))
                  End If
                Else
                    frmSubBrokerMaster.cmbPayMode = ""
                End If
            End If
            If i = 26 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtRem = Trim(.TextMatrix(.RowSel, i))
               Else
                 frmSubBrokerMaster.txtRem = ""
               End If
            End If
            If i = 27 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtAC = Trim(.TextMatrix(.RowSel, i))
               Else
                 frmSubBrokerMaster.txtAC = ""
               End If
            End If
            If i = 29 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.cmbExistCode = Trim(.TextMatrix(.RowSel, i))
               Else
                 frmSubBrokerMaster.cmbExistCode = ""
               End If
            End If
            If i = 30 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtSbCode = Trim(.TextMatrix(.RowSel, i))
               Else
                 frmSubBrokerMaster.txtSbCode = ""
               End If
            End If
            If i = 31 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtTds = Trim(.TextMatrix(.RowSel, i))
               Else
                 frmSubBrokerMaster.txtTds = ""
               End If
            End If
             If i = 37 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtPin = Trim(.TextMatrix(.RowSel, i))
               Else
                 frmSubBrokerMaster.txtPin = ""
              End If
             End If
             If i = 38 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                    rsSubType.open "Select investor_type from investortype where investor_code='" & Trim(.TextMatrix(.RowSel, i)) & "'", MyConn, adOpenForwardOnly
                    frmSubBrokerMaster.cmbstype = rsSubType(0)
                    rsSubType.Close
                    Set rsSubType = Nothing
               Else
                 frmSubBrokerMaster.cmbstype.ListIndex = -1
               End If
               
'               '               '''''''fill posp information'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
               If rsagent.State = 1 Then rsagent.Close
               rsagent.open "select * from agent_master where agent_code='" & Mycat & "'", MyConn, adOpenForwardOnly
               If Not rsagent.EOF Then
                    If rsagent.Fields("POSP_CERTIFIED_ON_LI") <> "" And rsagent.Fields("POSP_CERTIFIED_ON_LI") <> "__/__/____" Then
                       frmSubBrokerMaster.pospcertifieddateLI = rsagent.Fields("POSP_CERTIFIED_ON_LI")
                    End If
                    
                    If rsagent.Fields("POSP_CERTIFIED_ON_GI") <> "" And rsagent.Fields("POSP_CERTIFIED_ON_GI") <> "__/__/____" Then
                       frmSubBrokerMaster.pospcertifieddateGI = rsagent.Fields("POSP_CERTIFIED_ON_GI")
                    End If
                    
                    frmSubBrokerMaster.TXTIFSCCODE = IIf(rsagent!IFSC_CODE <> "", rsagent!IFSC_CODE, "")
                    frmSubBrokerMaster.txtbankname = IIf(rsagent!NEFT_BANK_NAME <> "", rsagent!NEFT_BANK_NAME, "")
                    frmSubBrokerMaster.TXTAGENTBANKNAME = IIf(rsagent!NAME_IN_BANK <> "", rsagent!NAME_IN_BANK, "")
                    frmSubBrokerMaster.txtbankbranchname = IIf(rsagent!BANK_BRANCH_NAME <> "", rsagent!BANK_BRANCH_NAME, "")
                    frmSubBrokerMaster.txtAccountNo = IIf(rsagent!accno <> "", rsagent!accno, "")
                    
                    If rsagent.Fields("POSP_VALID_TILL_LI") <> "" And rsagent.Fields("POSP_VALID_TILL_LI") <> "__/__/____" Then
                       frmSubBrokerMaster.PospValidTillLI = rsagent!POSP_VALID_TILL_LI
                    End If
                    
                    If rsagent.Fields("POSP_VALID_TILL_GI") <> "" And rsagent.Fields("POSP_VALID_TILL_GI") <> "__/__/____" Then
                       frmSubBrokerMaster.PospValidTillGI = rsagent!POSP_VALID_TILL_GI
                    End If

                    If rsagent.Fields("AADHAR_CARD_NO") <> "" Then
                       frmSubBrokerMaster.txtaadharcardno = fn_User_unlog(rsagent!AADHAR_CARD_NO)
                    End If
                    
                    If rsagent.Fields("GSTIN_NO") <> "" Then
                       frmSubBrokerMaster.TxtGstinno = rsagent!GSTIN_NO
                    End If


                    If rsagent.Fields("POSP_MARKING") <> "" Then
                       If rsagent!POSP_MARKING = "N" Then
                            frmSubBrokerMaster.CMBPOSPMARKING.ListIndex = 0
                       ElseIf rsagent!POSP_MARKING = "Y" Then
                            frmSubBrokerMaster.CMBPOSPMARKING.ListIndex = 1
                       End If
                    End If
                    
                    If rsagent.Fields("POSP_TYPE") <> "" Then
                       If rsagent!Posp_type = "Life" Then
                            frmSubBrokerMaster.CMBPOSPTYPE.ListIndex = 0
                       ElseIf rsagent!Posp_type = "General" Then
                            frmSubBrokerMaster.CMBPOSPTYPE.ListIndex = 1
                       ElseIf rsagent!Posp_type = "Both" Then
                            frmSubBrokerMaster.CMBPOSPTYPE.ListIndex = 2
                       End If
                    End If
                    
                    If rsagent.Fields("POSP_NO_LI") <> "" Then
                       frmSubBrokerMaster.TXTPOSPNOLI.Text = rsagent!POSP_NO_LI
                    End If
                    
                    If rsagent.Fields("POSP_NO_GI") <> "" Then
                       frmSubBrokerMaster.TXTPOSPNOGI.Text = rsagent!POSP_NO_GI
                    End If
                    
                    If rsagent.Fields("VERIFIED_STATUS") <> "" Then
                        If rsagent.Fields("VERIFIED_STATUS") = "Y" Then
                            frmSubBrokerMaster.ChkVerified.Value = 1
                        Else
                            frmSubBrokerMaster.ChkVerified.Value = 0
                        End If
                    End If
                    
                    If rsagent.Fields("ONLINE_BLOCK_AGENT") <> "" Then
                        If rsagent.Fields("ONLINE_BLOCK_AGENT") = "1" Then
                            frmSubBrokerMaster.ChkonlineBlock.Value = 1
                        Else
                            frmSubBrokerMaster.ChkonlineBlock.Value = 0
                        End If
                    End If
                    
                    If rsagent.Fields("BLOCK_AGENT") <> "" Then
                        If rsagent.Fields("BLOCK_AGENT") = "1" Then
                            frmSubBrokerMaster.ChkOfflineBlock.Value = 1
                        Else
                            frmSubBrokerMaster.ChkOfflineBlock.Value = 0
                        End If
                    End If
                    
                    '---------------------------DUCUMENT FILL------------------------------------------------------
                    
                    sql = "SELECT TRAN_SUB_TYPE,COMMON_ID  FROM WEALTHMAKER.TB_DOC_UPLOAD T WHERE EXIST_CODE='" & rsagent!exist_code & "'  AND TRAN_SUB_TYPE IN ('ECS','ASSOCIATE','FORM','PHOTO','PAN','IDPROOF','ADDRESS','ADHAR','MARKSHEET','BANKCOPY')"
                    sql = sql & " AND DOC_UPLOAD_DATE=(SELECT MAX(DOC_UPLOAD_DATE) FROM WEALTHMAKER.TB_DOC_UPLOAD T1 WHERE T1.EXIST_CODE=T.EXIST_CODE"
                    sql = sql & "  AND T.TRAN_SUB_TYPE= T1.TRAN_SUB_TYPE)"
                    rsSubType.open sql, MyConn, adOpenForwardOnly
                    frmSubBrokerMaster.CmbDocument.Clear
                    For j = 0 To rsSubType.RecordCount - 1
                        frmSubBrokerMaster.CmbDocument.AddItem rsSubType(0) & Space(50) & "#" & Trim(rsSubType(1))
                        rsSubType.MoveNext
                    Next
                    rsSubType.Close
                    Set rsSubType = Nothing
                    '----------------------------------------------------------------------------------------------
                    
               End If
               rsagent.Close
'               ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
   
            End If
            
            
            If i = 45 Then
              If .TextMatrix(.RowSel, i) <> "" Then
                 frmSubBrokerMaster.txtr_add1 = .TextMatrix(.RowSel, i)
               Else
                 frmSubBrokerMaster.txtr_add1 = ""
               End If
            End If
            If i = 46 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.txtr_add2 = .TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.txtr_add2 = ""
               End If
            End If
            If i = 47 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.cmbr_city = .TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.cmbr_city = ""
               End If
            End If
            If i = 48 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.txtr_state = .TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.txtr_state = ""
               End If
            End If
            If i = 49 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.txtr_pin = .TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.txtr_pin = ""
               End If
            End If
            '---------------------------  Vinay Kumar -----------------------------------------------------------------------------------
            '------------------------Updated on 1/May/2007------------------------------------------------------------------------------------
            
            If i = 58 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.txtMICR.Text = .TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.txtMICR.Text = ""
               End If
            End If
            
            If i = 59 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.mskAffDate.Text = Format(.TextMatrix(.RowSel, i), "dd/mm/yyyy") '.TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.mskAffDate.Text = "__/__/____"
               End If
            End If
            
            If i = 60 Then
               If .TextMatrix(.RowSel, i) <> "" And .TextMatrix(.RowSel, i) = "Y" Then
                  frmSubBrokerMaster.chkaudit.Value = 1
               Else
                  frmSubBrokerMaster.chkaudit.Value = 0
               End If
            End If
            
            
            If i = 61 Then
               If .TextMatrix(.RowSel, i) <> "" Then
                  frmSubBrokerMaster.mskauditdate.Text = Format(.TextMatrix(.RowSel, i), "dd/mm/yyyy") '.TextMatrix(.RowSel, i)
               Else
                  frmSubBrokerMaster.mskauditdate.Text = "__/__/____"
               End If
            End If
           
            If i = 62 Then
               Set rsBankbranch = New ADODB.Recordset
               If rsBankbranch.State = 1 Then rsBankbranch.Close
                    rsBankbranch.CursorLocation = adUseClient
                    rsBankbranch.open "select BRANCH,BANK_BRANCHID,branch_city FROM ANABANK_BRANCH_MASTER where BANK_BRANCHid='" & Trim(.TextMatrix(.RowSel, i)) & "'", MyConn, adOpenDynamic, adLockPessimistic
               If rsBankbranch.RecordCount > 0 Then
                    If rsBankbranch(0) <> "" Then
                         frmSubBrokerMaster.cmbBB.List(0) = rsBankbranch(0) & Space(50) & "#" & rsBankbranch(1)
                         frmSubBrokerMaster.cmbBB.ListIndex = 0
                         frmSubBrokerMaster.cmbbankcity.Text = rsBankbranch(2)
                    End If
               Else
                    frmSubBrokerMaster.cmbBB.ListIndex = -1
               End If
            End If
            
             If i = 63 Then
               If .TextMatrix(.RowSel, i) <> "N" Then
                  frmSubBrokerMaster.CHK_Online.Value = 1
               Else
                  frmSubBrokerMaster.CHK_Online.Value = 0
               End If
            End If
            
             If i = 67 Then
                frmSubBrokerMaster.CmbAna.ListIndex = -1
                frmSubBrokerMaster.CmbAna.Text = .TextMatrix(.RowSel, i)
            End If
            
            '-------------------------------------------------------------------------------------------------------------------------------
        
        Next
    End With
    With frmSubBrokerMaster
             .width = main.ScaleWidth
             .Height = main.ScaleHeight
             .Left = 0
             .Top = 0
             .ZOrder
     End With
     frmSubBrokerMaster.Visible = True
     '------------------------------vinit 11 jan15
     If frmSubBrokerMaster.chkaudit.Value = 1 And GlbroleId <> 10 And GlbroleId <> 212 Then
         frmSubBrokerMaster.cmbSource.Enabled = False
         frmSubBrokerMaster.cmbrm.Enabled = False
         'frmSubBrokerMaster.cmbExistCode.Enabled = False
         frmSubBrokerMaster.txtSbCode.Enabled = False
    
            If frmSubBrokerMaster.chkaudit.Value = 1 And GlbroleId <> 10 Then
               frmSubBrokerMaster.Frame3.Enabled = False
               frmSubBrokerMaster.Frame5.Enabled = False
            Else
               frmSubBrokerMaster.Frame3.Enabled = True
               frmSubBrokerMaster.Frame5.Enabled = True
               frmSubBrokerMaster.txtPan.Enabled = False
               frmSubBrokerMaster.Txtname.Enabled = False
            End If
    
        
        If MyConn.Execute("select count(*) from subupdatedright where busi_code='" & Glbloginid & "'")(0) <= 0 And GlbroleId <> 10 Then
            frmSubBrokerMaster.txtPan.Enabled = False
            frmSubBrokerMaster.Frame3.Enabled = False
            frmSubBrokerMaster.Txtname.Enabled = False
        Else
            frmSubBrokerMaster.txtPan.Enabled = True
            frmSubBrokerMaster.Frame3.Enabled = True
            frmSubBrokerMaster.Txtname.Enabled = True
            frmSubBrokerMaster.txtPan.Enabled = False
            frmSubBrokerMaster.Txtname.Enabled = False
        End If
    End If
    
'    txtPan
'Frame3
'txtName
    
    Unload Me
End Sub

Private Sub dataview_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then dataview_DblClick
End Sub
Private Sub Form_Activate()
Dim i As Long
Dim rsCity As New ADODB.Recordset
Dim rsBr As New ADODB.Recordset
    cboCity.Clear
    rsCity.CursorLocation = adUseClient
    rsCity.open "select * from city_master order by city_name", MyConn, adOpenForwardOnly, adLockReadOnly
    i = 0
    While Not rsCity.EOF
        cboCity.AddItem rsCity(1)
        cboCity.List(i, 1) = rsCity(0)
        rsCity.MoveNext
        i = i + 1
    Wend
    rsCity.Close
    Set rsCity = Nothing
    cmbBr.Clear
    rsBr.CursorLocation = adUseClient
    rsBr.open "select Branch_name,branch_code from branch_master where branch_code in (" & Branches & ") order by branch_name", MyConn, adOpenForwardOnly, adLockReadOnly
    i = 0
    While Not rsBr.EOF
        cmbBr.AddItem rsBr(0)
        cmbBr.List(i, 1) = rsBr(1)
        rsBr.MoveNext
        i = i + 1
    Wend
    rsBr.Close
    Set rsBr = Nothing
End Sub
Private Sub Form_Load()
    Me.Top = (main.Height - Me.Height) / 2 - 400
    Me.Left = (main.width - Me.width) / 2
    Drawgrid
End Sub
Private Sub Drawgrid()
dataview.Clear
With dataview
    .Cols = 69
    .Row = 0
    .Col = 0
    .ColWidth(0) = 0
    .Font.Bold = True
    .Text = "Sub Broker Code"
    .Col = 1
    .ColWidth(1) = 1500
    .Font.Bold = True
    .Text = "Sub Broker Name"
    .Col = 2
    .ColWidth(2) = 1500
    .Font.Bold = True
    .Text = "First Address"
    .Col = 3
    .ColWidth(3) = 1500
    .Font.Bold = True
    .Text = "Second Address"
    .Col = 4
    .ColWidth(4) = 0
    .Font.Bold = True
    .Text = "City"
    .Col = 5
    .ColWidth(5) = 1500
    .Font.Bold = True
    .Text = "Phone"
    .Col = 6
    .ColWidth(6) = 1500
    .Font.Bold = True
    .Text = "Mobile"
    .Col = 7
    .ColWidth(7) = 1500
    .Font.Bold = True
    .Text = "Fax"
    .Col = 8
    .ColWidth(8) = 1500
    .Font.Bold = True
    .Text = "Email"
    .Col = 9
    .ColWidth(9) = 1500
    .Font.Bold = True
    .Text = "Contact Person"
    .Col = 10
    .ColWidth(10) = 1500
    .Font.Bold = True
    .Text = "Enrollment Date"
    .Col = 11
    .ColWidth(11) = 0
    .Font.Bold = True
    .Text = "Category"
    .Col = 12
    .ColWidth(12) = 0
    .Font.Bold = True
    .Text = "Source Code"
    .Col = 13
    .ColWidth(13) = 1100
    .Font.Bold = True
    .Text = "Date Of Birth"
    .Col = 14
    .ColWidth(14) = 900
    .Font.Bold = True
    .Text = "Status Code"
    .Col = 15
    .ColWidth(15) = 1500
    .Font.Bold = True
    .Text = "Pan No."
    .Col = 16
    .ColWidth(16) = 1500
    .Font.Bold = True
    .Text = "Dist"
    .Col = 17
    .ColWidth(17) = 1000
    .Font.Bold = True
    .Text = "AMFI Certificate"
    .Col = 18
    .ColWidth(18) = 1100
    .Font.Bold = True
    .Text = "Registration No."
    .Col = 19
    .ColWidth(19) = 900
    .Font.Bold = True
    .Text = "AMFI ID"
    .Col = 20
    .ColWidth(20) = 1200
    .Font.Bold = True
    .Text = "Contact Person Email"
    .Col = 21
    .ColWidth(21) = 0
    .Font.Bold = True
    .Text = "Bank Code"
    .Col = 22
    .ColWidth(22) = 1200
    .Font.Bold = True
    .Text = "Account Type"
    .Col = 23
    .ColWidth(23) = 1100
    .Font.Bold = True
    .Text = "Web Site"
    .Col = 24
    .ColWidth(24) = 1500
    .Font.Bold = True
    .Text = "Employee Code"
    .Col = 25
    .ColWidth(25) = 1200
    .Font.Bold = True
    .Text = "Payment Mode"
    .Col = 26
    .ColWidth(26) = 1200
    .Font.Bold = True
    .Text = "Remark"
    .Col = 27
    .ColWidth(27) = 1200
    .Font.Bold = True
    .Text = "Account No"
    .Col = 28
    .ColWidth(28) = 0
    .Col = 29
    .ColWidth(29) = 1000
    .Font.Bold = True
    .Text = "Exist Code"
    .Col = 30
    .ColWidth(30) = 1500
    .Font.Bold = True
    .Text = "Sub Broker Source"
    .Col = 31
    .ColWidth(31) = 1500
    .Font.Bold = True
    .Text = "TDS Percentage"
    .Col = 32
    .ColWidth(32) = 0
    .Col = 33
    .ColWidth(33) = 0
    .Col = 34
    .ColWidth(34) = 0
    .Col = 35
    .ColWidth(35) = 1300
    .Font.Bold = True
    .Text = "Loged user code"
    .Col = 36
    .ColWidth(36) = 2500
    .Font.Bold = True
    .Text = "Creation Time"
    .Col = 37
    .ColWidth(37) = 0
    .Font.Bold = True
    .Text = "Pin Code"
    .Col = 38
    .ColWidth(38) = 1500
    .Font.Bold = True
    .Text = "Sub Broker Type"
    .Col = 39
    .ColWidth(39) = 0
    
    .Text = "Residence Address1"
    .Col = 40
    .ColWidth(40) = 1500
    .Font.Bold = True
    .Text = "Residence Address2"
    .Col = 41
    .ColWidth(41) = 1500
    .Font.Bold = True
    .Text = "Residence City"
    .Col = 42
    .ColWidth(42) = 1500
    .Text = "Residence State"
    .Col = 43
    .ColWidth(43) = 1500
    .Text = "Residence Pin"
    .Col = 44
    .ColWidth(44) = 1500
    .ColWidth(45) = 1
    .ColWidth(46) = 1
    .ColWidth(47) = 1
    .ColWidth(48) = 1
    .ColWidth(49) = 1
    .ColWidth(50) = 1
    .ColWidth(51) = 1
    .ColWidth(52) = 1
    .ColWidth(53) = 1
    .ColWidth(54) = 1
    .ColWidth(55) = 1
    .ColWidth(56) = 1
    .ColWidth(57) = 1
    .Col = 58
    .ColWidth(58) = 1500
    .Text = "MICR No."
    .Col = 59
    .ColWidth(59) = 1500
    .Text = "Affected From"
    
     .Col = 60
    .ColWidth(60) = 1500
    .Text = "ana_audit"
    .Col = 61
    .ColWidth(61) = 1500
    .Text = "ANA_AuditDate"
    .Col = 62
    .ColWidth(62) = 1500
    .Text = "Bank_BranchID"
     .Col = 63
    .ColWidth(63) = 0
    .Text = "Online_Flag"
    
End With
End Sub


Private Sub Txtcode_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then SendKeys "{Tab}"
    'Call PhoneValidation(KeyAscii)
    If (KeyAscii >= 48 And KeyAscii <= 57) Or (KeyAscii >= 97 And KeyAscii <= 122) Or (KeyAscii >= 65 And KeyAscii <= 90) Or KeyAscii = 8 Then
    Else
        KeyAscii = 0
    End If
End Sub
Private Sub txtMobile_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then SendKeys "{Tab}"
    Call PhoneValidation(KeyAscii)
End Sub
Private Sub txtname_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then SendKeys "{Tab}"
    Call NameValidation(KeyAscii)
End Sub
Private Sub txtPAN_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then SendKeys "{Tab}"
End Sub
Private Sub txtPhone_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then SendKeys "{Tab}"
    Call PhoneValidation(KeyAscii)
End Sub
