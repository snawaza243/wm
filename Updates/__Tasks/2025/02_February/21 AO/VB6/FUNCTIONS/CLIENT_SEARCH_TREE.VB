Dim setfoc As String
Public currentForm As Form
Public treeName As String
Public ReportFlag As Boolean
Dim SQLBranches As String
Public Ar_type As String 'jogi010907
Public Function findexactstrg(ByVal cmb As ComboBox, ByVal str1 As String)
    cmb.ListIndex = SendMessage(cmb.hwnd, CB_FINDSTRINGEXACT, -1, ByVal str1)
End Function
Private Function getBranchName(refcode As String) As String
Dim BRCODELIST As String
Dim qry As String
Dim rs_get_br As ADODB.Recordset
Dim rs_get_Loc As ADODB.Recordset
    If UCase(Left(refcode, 1)) = "R" Then
        qry = "select branch_code from branch_master where region_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
      ElseIf UCase(Left(refcode, 1)) = "Z" Then
        qry = "select branch_code from branch_master where zone_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "C" Then
        Set rs_get_Loc = New ADODB.Recordset
        qry = "select location_id from location_master where city_id='" & refcode & "'"
        Set rs_get_Loc = MyConn.Execute(qry)
        If Not rs_get_Loc.EOF Then
            Set rs_get_br = New ADODB.Recordset
            Do While Not rs_get_Loc.EOF
                qry = "select branch_code from branch_master where location_id='" & rs_get_Loc(0) & "'"
                Set rs_get_br = MyConn.Execute(qry)
                If Not rs_get_br.EOF Then
                    Do While Not rs_get_br.EOF
                        BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                        rs_get_br.MoveNext
                    Loop
                End If
                rs_get_Loc.MoveNext
            Loop
        End If
    ElseIf UCase(Left(refcode, 1)) = "L" Then
        qry = "select branch_code from branch_master where location_id='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf Left(refcode, 1) = "2" Then
        qry = "select source from employee_master where rm_code='" & refcode & "'"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    ElseIf Left(refcode, 1) = "1" Then
        BRCODELIST = BRCODELIST & "#" & refcode
    ElseIf Left(refcode, 1) = "7" Then
        qry = "select branch_code from branch_master"
        Set rs_get_br = MyConn.Execute(qry)
        If Not rs_get_br.EOF Then
            Do While Not rs_get_br.EOF
                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
                rs_get_br.MoveNext
            Loop
        End If
    End If
    getBranchName = BRCODELIST
End Function


Private Sub ShowFilterData()
Dim sql As String
sql = ""
If Trim(UCase(Cmbcat.Text)) = "CLIENT" Then
     sql = "Select CLIENT_name,CLIENT_CODE,a.add1,a.add2,C.CITY_NAME,a.tel1,client_pan,client_codekyc,rm_name,Branch_Name,a.LoggedUserId FROM CLIENT_test a,City_master c,employee_master emp,branch_master br where a.branch_code=br.branch_code and a.business_code=emp.payroll_id and  a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL "
    If currentForm.Name = "FrmAccountTransafer" Then
        sql = sql & " and a.client_code=a.main_code "
    End If
    If txtName.Text <> "" Then
        sql = sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(txtName.Text)), " ", "%") & "%'"
    End If
    If txtAdd1.Text <> "" Then
        sql = sql & " and upper(a.add1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'"
    End If
    If txtAdd2.Text <> "" Then
        sql = sql & " and upper(a.add2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'"
    End If
    If txtPhone.Text <> "" Then
        sql = sql & " and upper(a.tel1) like '%" & UCase(Trim(txtPhone.Text)) & "%'"
    End If
    If txtMobile.Text <> "" Then
        sql = sql & " and upper(a.mobile_no) like '%" & UCase(Trim(txtMobile.Text)) & "%'"
    End If
    If cmbCity.ListIndex <> -1 Then
        sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'"
    End If
    If txtPAN.Text <> "" Then
        sql = sql & " and a.client_pan='" & txtPAN.Text & "'"
    End If
    If txtahcode <> "" Then
       sql = sql & " and a.client_code='" & txtahcode.Text & "'"
    End If
    
    If txtPolicyNo <> "" Then
       sql = sql & " and a.DESIGNATION='" & txtPolicyNo.Text & "'"
    End If
    
    If TxtPrevPolicyNo <> "" Then
       sql = sql & " and a.TAX_BRACKET='" & TxtPrevPolicyNo.Text & "'"
    End If
    
    
    If txtCode.Text <> "" Then
        If CmbClientBroker.Text = "CLIENT" Then
            sql = sql & " and source_CODE in (select client_code from client_master where (upper(exist_code) like '%" & UCase(Trim(txtCode.Text)) & "%' OR TO_CHAR(client_code)='" & Trim(txtCode.Text) & "')) "
            sql = sql & " and source_CODE in (select client_code from client_master where TO_CHAR(client_code)='" & Trim(txtCode.Text) & "') "
        End If
    End If
    If currentForm.Name = "frmAccountUpdation" Then
        sql = sql & " and a.approved='YES'"
    End If
    If cmbBranch.ListIndex <> -1 Then
        sql = sql & " and a.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    Else
        sql = sql & " and a.BRANCH_CODE in(" & SQLBranches & ")"
    End If
End If


Populate_Data sql
End Sub

Private Sub chkIndia_Click()
    If chkIndia.Value = 1 Then
        cmbBranch.Enabled = False
        cmbCity.Enabled = False
    Else
        cmbBranch.Enabled = True
        cmbCity.Enabled = True
    End If
End Sub

Private Sub cmbbranch_Change()
Dim RsData As New ADODB.Recordset
    
    If strForm = "Client Transfer" Then
        cmbOldRM.Clear
    
        If cmbBranch.ListIndex <> -1 Then
            RsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code in (select distinct from_rm_code from client_master where from_sourceid =" & cmbBranch.List(cmbBranch.ListIndex, 1) & ") order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            RsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code in (select distinct from_rm_code from client_master where from_sourceid in(" & SQLBranches & ")) order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    
        j = 0
        While Not RsData.EOF
            cmbOldRM.AddItem RsData("RM_NAME")
            cmbOldRM.List(j, 1) = RsData("PAYROLL_ID")
            cmbOldRM.List(j, 2) = RsData("RM_CODE")
            j = j + 1
            RsData.MoveNext
        Wend
        RsData.Close
        Set RsData = Nothing
    End If
    txtRM.Text = ""
    
    cmbNewRM.Clear
    If cmbBranch.ListIndex <> -1 Then
        If SRmCode <> "" Then
            RsData.open "select * from employee_master where type='A' and source=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " and rm_code in (" & SRmCode & ") order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            RsData.open "select * from employee_master where type='A' and source=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    Else
        If SRmCode <> "" Then
            RsData.open "select * from employee_master where type='A' and source in (" & Branches & ") and rm_code in (" & SRmCode & ") order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            RsData.open "select * from employee_master where type='A' and source in (" & Branches & ") order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    End If
    
    'and category_id<>'2002'
    j = 0
    While Not RsData.EOF
        cmbNewRM.AddItem RsData("RM_NAME")
        cmbNewRM.List(j, 1) = RsData("PAYROLL_ID")
        cmbNewRM.List(j, 2) = RsData("RM_CODE")
        j = j + 1
        RsData.MoveNext
    Wend
    RsData.Close
End Sub

Private Sub cmbbranch_KeyDown(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
        'SendKeys "{TAB}"
    End If
End Sub
Private Sub cmbBranch_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbBranch.MatchFound = False Then
        cmbBranch.ListIndex = -1
    End If
End Sub

Private Sub cmbCity_KeyDown(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub
Private Sub cmbCity_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbCity.MatchFound = False Then
        cmbCity.ListIndex = -1
    End If
End Sub

Private Sub cmbOldRM_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbOldRM.MatchFound = False Then
        cmbOldRM.ListIndex = -1
    End If

End Sub

Private Sub cmbOldRM_LostFocus()
    If cmbOldRM.MatchFound = False Then
        cmbOldRM.ListIndex = -1
    End If
End Sub

Private Sub cmdExit_Click()
    Unload Me
End Sub

Private Sub cmdSelect_Click()
Dim Found As Boolean
        
        'MsgBox msfgClients.Row & " - " & (msfgClients.Row + msfgClients.RowSel) - msfgClients.Row
        'Exit Sub

        'r = msfgClients.Row
    If msfgClients.Rows > 1 Then
        Me.MousePointer = 11
        For i = msfgClients.Row To (msfgClients.Row + msfgClients.RowSel) - msfgClients.Row
                Found = False
                
                For j = 1 To frmRMTransfer1.ClientDetails.Rows - 1
                    If frmRMTransfer1.ClientDetails.TextMatrix(j, 4) = msfgClients.TextMatrix(i, 1) Then
                        Found = True
                        Exit For
                    End If
                Next j
                If Found = False Then
                        If frmRMTransfer1.ClientDetails.Rows >= 2 And frmRMTransfer1.ClientDetails.TextMatrix(1, 1) <> "" Then
                            frmRMTransfer1.ClientDetails.Rows = frmRMTransfer1.ClientDetails.Rows + 1
                        End If
                
                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 1) = msfgClients.TextMatrix(i, 0)
                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 4) = msfgClients.TextMatrix(i, 1)
                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 3) = msfgClients.TextMatrix(i, 5)
                        If Cmbcat.Text = "CLIENT" Then
                            Set rs_get_rm = MyConn.Execute("select C.rm_code,rm_name from client_master c,employee_master e where client_code=" & msfgClients.TextMatrix(i, 1) & " and c.rm_code=e.rm_code")
                        Else
                            Set rs_get_rm = MyConn.Execute("select C.rm_code,rm_name from agent_master c,employee_master e where agent_code=" & msfgClients.TextMatrix(i, 1) & " and c.rm_code=e.rm_code")
                        End If
                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 2) = rs_get_rm("rm_name")
                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 5) = rs_get_rm("rm_code")
                        frmRMTransfer1.ClientDetails.SetFocus
                End If
        Next i
        Me.Show
        Me.MousePointer = 0
    End If
End Sub

Private Sub cmdShow_Click()
On Error GoTo err
If Cmbcat.Text <> "" Then
    If txtName.Text = "" And txtAdd1.Text = "" And txtAdd2.Text = "" And txtPhone.Text = "" And txtMobile.Text = "" And cmbNewRM.ListIndex = -1 And txtPAN.Text = "" And txtahcode = "" And txtCode = "" Then
        If Cmbcat.Text = "INVESTOR" And txtCliSubName.Text = "" Then
            MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
            txtName.SetFocus
            Exit Sub
        ElseIf Cmbcat.Text = "CLIENT" Or Cmbcat.Text = "AGENT" Then
            MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
            txtName.SetFocus
            Exit Sub
        End If
    End If
    cmdShow.Enabled = False
    Me.MousePointer = 11
    msfgClients.Clear
    If chkIndia.Value = 1 Then
        AllIndiaSearch
    Else
        ShowFilterData
    End If
    set_grid
    Me.MousePointer = 0
    cmdShow.Enabled = True
Else
    MsgBox "Please Select Category ", vbInformation
    Cmbcat.SetFocus
End If


Exit Sub
err:
    MsgBox err.Description
'    ''resume
End Sub

Private Sub Form_Activate()
On Error GoTo err1
Dim RsData As New ADODB.Recordset
Dim j As Integer

Cmbcat.ListIndex = 0
Combo2.ListIndex = 0
CmbClientBroker.ListIndex = 0
SQLBranches = ""
If GlbDataFilter = "72" Then
    RsData.open "SELECT BRANCH_CODE FROM BRANCH_MASTER where (BRANCH_TYPE <>'Inactive' OR BRANCH_TYPE IS NULL) and CATEGORY_ID NOT IN('1004','1005','1006')", MyConn, adOpenForwardOnly
    While Not RsData.EOF
        SQLBranches = SQLBranches & RsData(0) & ","
        RsData.MoveNext
    Wend
    SQLBranches = Left(SQLBranches, Len(SQLBranches) - 1)
Else
    SQLBranches = Branches    'BRANCHES VARIABLE IS GLOBAL FOR BRANCH FILTERING PURPOSE
End If

cmbCity.Clear
If RsData.State = 1 Then RsData.Close
RsData.open "Select city_id,city_name from city_master order by city_name", MyConn, adOpenForwardOnly
j = 0
While Not RsData.EOF
    cmbCity.AddItem RsData("City_name")
    cmbCity.List(j, 1) = RsData("City_id")
    j = j + 1
    RsData.MoveNext
Wend
RsData.Close

'cmbBranch.Clear
'RsData.open "Select branch_code,branch_name from branch_master where branch_code in(" & SQLBranches & ") order by branch_name", MyConn, adOpenForwardOnly
'j = 0
'While Not RsData.EOF
'    cmbBranch.AddItem RsData("branch_name")
'    cmbBranch.List(j, 1) = RsData("branch_code")
'    j = j + 1
'    RsData.MoveNext
'Wend
'cmbBranch.ListIndex = 0
'RsData.Close

If currentForm.Name = "frmRMTransfer1" Then
    cmbOldRM.Clear
    RsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code in (select from_rm_code from client_master where from_sourceid in(" & SQLBranches & ")) order by RM_NAME", MyConn, adOpenForwardOnly
    j = 0
    While Not RsData.EOF
        cmbOldRM.AddItem RsData("RM_NAME")
        cmbOldRM.List(j, 1) = RsData("PAYROLL_ID")
        cmbOldRM.List(j, 2) = RsData("RM_CODE")
        j = j + 1
        RsData.MoveNext
    Wend
    RsData.Close
End If

cmbNewRM.Clear
If SRmCode <> "" Then
    RsData.open "select * from employee_master where type='A' and source in (" & Branches & ") and rm_code in (" & SRmCode & ") order by RM_NAME", MyConn, adOpenForwardOnly
Else
    RsData.open "select * from employee_master where type='A' and source in (" & Branches & ") order by RM_NAME", MyConn, adOpenForwardOnly
End If
j = 0
While Not RsData.EOF
    cmbNewRM.AddItem RsData("RM_NAME")
    cmbNewRM.List(j, 1) = RsData("PAYROLL_ID")
    cmbNewRM.List(j, 2) = RsData("RM_CODE")
    j = j + 1
    RsData.MoveNext
Wend
RsData.Close
Exit Sub
err1:
MsgBox err.Description
End Sub

Private Sub Form_KeyPress(KeyAscii As Integer)
    If KeyAscii = 13 Then
        SendKeys "{TAB}"
    End If
End Sub
Private Sub Form_Load()
Me.Icon = LoadPicture(App.Path & "\W.ICO")
server = ""
exeserver = ""
otherpara = ""
file = App.Path & "\mfi_server.srv"
Open file For Input As #1
Line Input #1, server
Line Input #1, expserver
If Not EOF(1) Then
Line Input #1, otherpara
End If
Close #1
para = Split(server, "#")
MSRClient.DataSourceName = "TEST"
MSRClient.UserName = DataBaseUser
MSRClient.Password = DataBasePassword
Cmbcat.Clear
Row = 1
txtCode.Text = ""
txtName.Text = ""

If UCase(currentForm.Name) = UCase("frmactopen") Or UCase(currentForm.Name) = UCase("frmactopeninv") Or UCase(currentForm.Name) = UCase("frmAccountUpdation") Or UCase(currentForm.Name) = UCase("FrmAccountTransafer") Or UCase(currentForm.Name) = UCase("frmIPO") Then
    Cmbcat.AddItem "CLIENT"
    Combo2.AddItem "ACCOUNT"
    Label12.Visible = False
    txtCode.Visible = False
ElseIf UCase(currentForm.Name) = UCase("frmAccountMerging") Then
    Cmbcat.AddItem "CLIENT"
    Combo2.AddItem "ACCOUNT"
    Label12.Visible = True
    txtCode.Visible = True
End If
'Cmbcat.ListIndex = 0
'Combo2.ListIndex = 0
'CmbClientBroker.ListIndex = 0
End Sub
Private Sub Form_Unload(Cancel As Integer)
    strForm = ""
    ReportFlag = False
End Sub


Private Sub msfgClients_dblClick()
On Error GoTo err1
'On Error resume Next
Dim rsexistcode As ADODB.Recordset
Dim rs_get_type As ADODB.Recordset, rs_get_rm As ADODB.Recordset, rs_get_invsrc As ADODB.Recordset
Dim i As Integer
Dim susPense As String
Dim Rs_IPO As ADODB.Recordset
Dim Rs_IPOinv As ADODB.Recordset
Dim Found As Boolean
If msfgClients.Rows > 1 Then
    AllIndia_Inv_code = ""
    Set rs_get_type = New ADODB.Recordset
    Me.MousePointer = vbHourglass
    susPense = ""
    R = msfgClients.Row
    If currentForm.Name = "frmtransactionmf" Then
        If frmtransactionmf.SSTab1.Tab = 0 Then
            frmtransactionmf.TxtClientCodeA = msfgClients.TextMatrix(R, 1)
            Me.MousePointer = vbNormal
            Unload Me
        ElseIf frmtransactionmf.SSTab1.Tab = 1 Then
            frmtransactionmf.TxtClientCodeM = msfgClients.TextMatrix(R, 1)
            Me.MousePointer = vbNormal
            Unload Me
        End If
        Exit Sub
    End If
    If currentForm.Name = "frmactopen" Then
           frmactopen.txtclientcode = msfgClients.TextMatrix(R, 1)
           frmactopen.cmdview_Click
           Me.MousePointer = vbNormal
           Unload Me
          Exit Sub
    End If
    '------------------------FOR ACCOUNT MERGING-------------------------------------------
    If currentForm.Name = "frmAccountMerging" Then
        For i = 1 To frmAccountMerging.msfgInvestors.Rows - 1
            If frmAccountMerging.msfgInvestors.TextMatrix(i, 9) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Account has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
            
            If Trim(frmAccountMerging.msfgInvestors.TextMatrix(i, 6)) <> Trim(msfgClients.TextMatrix(R, 5)) Then
                If Trim(frmAccountMerging.msfgInvestors.TextMatrix(i, 6)) <> "MF Data Import" And Trim(msfgClients.TextMatrix(R, 5)) <> "MF Data Import" And Trim(frmAccountMerging.msfgInvestors.TextMatrix(i, 6)) <> "Insurance Data Import" And Trim(msfgClients.TextMatrix(R, 5)) <> "Insurance Data Import" Then
                    If GlbDataFilter <> "72" And GlbroleId <> "17" And GlbroleId <> "124" Then
                        MsgBox "The Account should be of the same Branch ", vbInformation
                        Me.MousePointer = vbNormal
                        Exit Sub
                    End If
                End If
            End If
        Next
        With frmAccountMerging.msfgInvestors
           .Rows = .Rows + 1
            MaxRow = .Rows - 1
            .TextMatrix(MaxRow, 11) = ""
            .TextMatrix(MaxRow, 2) = msfgClients.TextMatrix(R, 0) 'Account Name
            .TextMatrix(MaxRow, 3) = msfgClients.TextMatrix(R, 2) 'Address1
            .TextMatrix(MaxRow, 4) = msfgClients.TextMatrix(R, 3) 'Address2
            .TextMatrix(MaxRow, 5) = msfgClients.TextMatrix(R, 4) 'City
            .TextMatrix(MaxRow, 6) = msfgClients.TextMatrix(R, 9) 'Branch
            .TextMatrix(MaxRow, 7) = msfgClients.TextMatrix(R, 6) 'Pan
            .TextMatrix(MaxRow, 9) = msfgClients.TextMatrix(R, 1)  'Client_Code
            .TextMatrix(MaxRow, 10) = msfgClients.TextMatrix(R, 8) 'RM
            .TextMatrix(MaxRow, 12) = msfgClients.TextMatrix(R, 7) 'Branch Name
            .TextMatrix(MaxRow, 14) = msfgClients.TextMatrix(R, 10) 'LoggedUserId
            Dim AppCount As Integer
            AppCount = SqlRet("select count(*) from client_test A where client_code='" & msfgClients.TextMatrix(R, 1) & "' and approved='YES' AND A.APPROVE_DT IS NOT NULL ")
            If AppCount > 0 Then
                 .TextMatrix(MaxRow, 13) = "YES"
            Else
                 .TextMatrix(MaxRow, 13) = "NO"
            End If
        End With
        Me.MousePointer = vbNormal
        Set rsCount = Nothing
        DoEvents
        msfgClients.SetFocus
        DoEvents
        'Unload Me
        Exit Sub
    End If
    
    '-----------------------frm ipo---------------------------------------------
    If currentForm.Name = "frmIPO" Then
         Set Rs_IPO = New ADODB.Recordset
         Rs_IPO.open "select client_codekyc from client_test where client_code='" & msfgClients.TextMatrix(R, 1) & "'", MyConn, adOpenForwardOnly
         If Not Rs_IPO.EOF Then
            Set Rs_IPOinv = New ADODB.Recordset
            Rs_IPOinv.open "select inv_code ,investor_name from investor_master where inv_code='" & Rs_IPO.Fields(0) & "' ", MyConn, adOpenForwardOnly
            If Not Rs_IPOinv.EOF Then
                frmIPO.txtINV_CD.Text = Rs_IPOinv.Fields(0)
                frmIPO.txtName.Text = Rs_IPOinv.Fields(1)
            End If
            Rs_IPOinv.Close
         End If
         Rs_IPO.Close
           frmIPO.txtahcode.Text = msfgClients.TextMatrix(R, 1)
           Me.MousePointer = vbNormal
           Unload Me
          Exit Sub
    End If
    '---------------------------------------------------------------------------
    If currentForm.Name = "frmactopeninv" Then
           frmactopeninv.txtclientcode = msfgClients.TextMatrix(R, 1)
           frmactopeninv.cmdview_Click
           Me.MousePointer = vbNormal
           Unload Me
          Exit Sub
    End If
    If currentForm.Name = "frmtransactionmf_modify" Then
        frmtransactionmf.TxtClientCodeM = msfgClients.TextMatrix(R, 1)
        Me.MousePointer = vbNormal
        Unload Me
        Exit Sub
    End If
    
    If currentForm.Name = "frmAccountUpdation" Then
           frmAccountUpdation.txtclientcode = msfgClients.TextMatrix(R, 1)
           frmAccountUpdation.cmdview_Click
           Me.MousePointer = vbNormal
           Unload Me
          Exit Sub
    End If
    If currentForm.Name = "FrmAccountTransafer" Then
           FrmAccountTransafer.txtclientcode = msfgClients.TextMatrix(R, 1)
           FrmAccountTransafer.cmdview_Click
           Me.MousePointer = vbNormal
           Unload Me
          Exit Sub
    End If
    
    
    
End If
Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
    Resume
End Sub
Private Sub lstname_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 27 Then
        lstname.Visible = False
        If setfoc = "txtinvname" Then
            txtInvName.SetFocus
            txtInvName.SelStart = Len(txtInvName)
        ElseIf setfoc = "txtclname" Then
            txtclname.SetFocus
            txtclname.SelStart = Len(txtclname)
        ElseIf setfoc = "txtclcode" Then
            txtClCode.SetFocus
            txtClCode.SelStart = Len(txtClCode)
        ElseIf setfoc = "txtagcode" Then
            txtAgCode.SetFocus
            txtAgCode.SelStart = Len(txtAgCode)
        End If
    ElseIf KeyCode = 13 Then
        lstname.Visible = False
        If setfoc = "txtinvname" Then
            txtInvName = lstname.Text
            txtInvName.SetFocus
            txtInvName.SelStart = Len(txtInvName)
        ElseIf setfoc = "txtclname" Then
            txtclname = lstname.Text
            txtclname.SetFocus
            txtclname.SelStart = Len(txtclname)
        ElseIf setfoc = "txtclcode" Then
            txtClCode = lstname.Text
            txtClCode.SetFocus
            txtClCode.SelStart = Len(txtClCode)
        ElseIf setfoc = "txtagcode" Then
            txtAgCode = lstname.Text
            txtAgCode.SetFocus
            txtAgCode.SelStart = Len(txtAgCode)
        End If
    End If
End Sub
Private Sub msfgClients_KeyPress(KeyAscii As Integer)
If KeyAscii = 13 Then
    Call msfgClients_dblClick
End If
End Sub

Private Sub txtAdd1_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub
Private Sub txtAdd2_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub
Private Sub Txtcode_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub
Private Sub txtname_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub
Private Sub Populate_Data(strQuery As String)
On Error GoTo err1
    MSRClient.sql = ""
    MSRClient.sql = strQuery
    MSRClient.Refresh
    If msfgClients.Rows = 1 And currentForm.Name = "FrmAccountTransafer" Then
        MsgBox "This Is Not Main Account", vbInformation, "Wealthmaker"
    End If
    Exit Sub
err1:
    'MsgBox Err.Description
End Sub
Private Sub set_grid()
On Error GoTo err1
    msfgClients.Row = 0
    msfgClients.ColWidth(0) = "2700"
    msfgClients.Text = "ClientName"
    msfgClients.CellFontBold = True
    msfgClients.Col = 1
    msfgClients.ColWidth(1) = "0"
    msfgClients.Text = "Code"
    msfgClients.CellFontBold = True
    msfgClients.Col = 2
    msfgClients.ColWidth(2) = "1600"
    msfgClients.Text = "Address1"
    msfgClients.CellFontBold = True
    msfgClients.Col = 3
    msfgClients.ColWidth(3) = "1600"
    msfgClients.Text = "Address2"
    msfgClients.CellFontBold = True
    msfgClients.Col = 4
    msfgClients.ColWidth(4) = "1100"
    msfgClients.Text = "City"
    msfgClients.CellFontBold = True
    msfgClients.Col = 5
    msfgClients.ColWidth(5) = "1100"
    msfgClients.Text = "Phone1"
    msfgClients.CellFontBold = True
    msfgClients.Col = 6
    msfgClients.ColWidth(6) = "1000"
    msfgClients.Text = "Pan"
    msfgClients.CellFontBold = True
    msfgClients.Col = 7
    msfgClients.ColWidth(7) = "1500"
    msfgClients.Text = "ClientCodeKyc"
    msfgClients.CellFontBold = True
    msfgClients.Col = 8
    msfgClients.ColWidth(8) = "1500"
    msfgClients.Text = "RM"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 9
    msfgClients.ColWidth(9) = "1500"
    msfgClients.Text = "Branch"
    msfgClients.CellFontBold = True
    
    msfgClients.Col = 10
    msfgClients.ColWidth(10) = "1000"
    msfgClients.Text = "LoggedUserId"
    msfgClients.CellFontBold = True
    
    msfgClients.SetFocus
    If msfgClients.Rows > 1 Then
        msfgClients.Row = 1
    End If
    msfgClients.Col = 0
    Exit Sub
err1:
    'MsgBox Err.Description
End Sub
Private Sub txtPhone_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub

Private Sub AllIndiaSearch()
Dim sql As String
sql = ""
If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
If Trim(txtCode.Text) = "" And Trim(txtName.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtPhone.Text) = "" Then
    'Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
Else
    If txtCode.Text <> "" Then
        sql = sql & " and inv_code like '%" & Trim(txtCode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    If txtName.Text <> "" Then
        sql = sql & " and upper(a.investor_name) like '" & UCase(Trim(txtName.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    If txtAdd1.Text <> "" Then
        sql = sql & " and upper(a.address1) like '%" & UCase(Trim(txtAdd1.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    If txtAdd2.Text <> "" Then
        sql = sql & " and upper(a.address2) like '%" & UCase(Trim(txtAdd2.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    If txtPhone.Text <> "" Then
        sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    If txtMobile.Text <> "" Then
        sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    'Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
    
End If
sql = sql & " ORDER BY upper(a.investor_name)"
End If


Populate_Data sql

End Sub

