Private Sub AllIndiaSearch()
Dim sql As String
sql = ""


If Trim(UCase(CmbClientBroker.Text)) = "CLIENT" Then
    sql = "Select INVESTOR_name,INV_code,'' address1,'' address2,'' city_name,b.Branch_name,'' phone,e.rm_name,cm.guest_cd,a.pan,T.CLIENT_CODE FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,CLIENT_TEST T,client_master cm  where  cm.client_code=a.source_id and T.CLIENT_CODEKYC=A.INV_CODE AND a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
    If txtPan.Text <> "" Then
        sql = sql & " and upper(a.PAN) = '" & UCase(Trim(txtPan.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
'    If txtahcode.Text <> "" Then
'        sql = sql & " and upper(t.CLIENT_CODE) like '%" & UCase(Trim(txtahcode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'    End If
    If txtahcode.Text <> "" Then
        sql = sql & " and upper(t.CLIENT_CODE) = '" & UCase(Trim(txtahcode.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    sql = sql & " ORDER BY upper(a.investor_name)"
Else  'CASE OF SUBBROKER
    sql = "Select INVESTOR_name,INV_code,'' address1,'' address2,'' city_name,b.Branch_name,'' phone,e.rm_name,'' guest_cd,a.pan,T.CLIENT_CODE FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,CLIENT_TEST T  where T.CLIENT_CODEKYC=A.INV_CODE AND a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
    If txtPan.Text <> "" Then
        sql = sql & " and upper(a.PAN) = '" & UCase(Trim(txtPan.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    If txtahcode.Text <> "" Then
        sql = sql & " and upper(t.CLIENT_CODE) = '" & UCase(Trim(txtahcode.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
    End If
    sql = sql & " ORDER BY upper(a.investor_name)"


End If


Populate_Data sql
End Sub
'Dim sql As String
'
'        sql = ""
'        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
'            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
'
'            If Trim(txtCode.Text) = "" And Trim(TxtName.Text) = "" And Trim(TxtAdd1.Text) = "" And Trim(TxtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtPhone.Text) = "" And Trim(TxtPan.Text) = "" Then
'                'Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
'            Else
'                If txtCode.Text <> "" Then
'                    sql = sql & " and inv_code like '%" & Trim(txtCode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If TxtName.Text <> "" Then
'                    sql = sql & " and upper(a.investor_name) like '" & UCase(Trim(TxtName.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If TxtAdd1.Text <> "" Then
'                    sql = sql & " and upper(a.address1) like '%" & UCase(Trim(TxtAdd1.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If TxtAdd2.Text <> "" Then
'                    sql = sql & " and upper(a.address2) like '%" & UCase(Trim(TxtAdd2.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtPhone.Text <> "" Then
'                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If TxtPan.Text <> "" Then
'                    sql = sql & " and upper(a.Pan) like '%" & UCase(Trim(TxtPan.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtMobile.Text <> "" Then
'                    sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                'Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
'
'            End If
'            sql = sql & " ORDER BY upper(a.investor_name)"
'        End If
'        Populate_Data sql
        
'End Sub

'''Dim setfoc As String
'''Public currentForm As Form
'''Public treeName As String
'''Public ReportFlag As Boolean
'''Dim SQLBranches As String
'''Public Ar_type As String 'jogi010907
'''Public Function findexactstrg(ByVal cmb As ComboBox, ByVal str1 As String)
'''    cmb.ListIndex = SendMessage(cmb.hwnd, CB_FINDSTRINGEXACT, -1, ByVal str1)
'''End Function
'''Private Function getBranchName(refcode As String) As String
'''Dim BRCODELIST As String
'''Dim qry As String
'''Dim rs_get_br As ADODB.Recordset
'''Dim rs_get_Loc As ADODB.Recordset
'''    If UCase(Left(refcode, 1)) = "R" Then
'''        qry = "select branch_code from branch_master where region_id='" & refcode & "'"
'''        Set rs_get_br = myconn.Execute(qry)
'''        If Not rs_get_br.EOF Then
'''            Do While Not rs_get_br.EOF
'''                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
'''                rs_get_br.MoveNext
'''            Loop
'''        End If
'''      ElseIf UCase(Left(refcode, 1)) = "Z" Then
'''        qry = "select branch_code from branch_master where zone_id='" & refcode & "'"
'''        Set rs_get_br = myconn.Execute(qry)
'''        If Not rs_get_br.EOF Then
'''            Do While Not rs_get_br.EOF
'''                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
'''                rs_get_br.MoveNext
'''            Loop
'''        End If
'''    ElseIf UCase(Left(refcode, 1)) = "C" Then
'''        Set rs_get_Loc = New ADODB.Recordset
'''        qry = "select location_id from location_master where city_id='" & refcode & "'"
'''        Set rs_get_Loc = myconn.Execute(qry)
'''        If Not rs_get_Loc.EOF Then
'''            Set rs_get_br = New ADODB.Recordset
'''            Do While Not rs_get_Loc.EOF
'''                qry = "select branch_code from branch_master where location_id='" & rs_get_Loc(0) & "'"
'''                Set rs_get_br = myconn.Execute(qry)
'''                If Not rs_get_br.EOF Then
'''                    Do While Not rs_get_br.EOF
'''                        BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
'''                        rs_get_br.MoveNext
'''                    Loop
'''                End If
'''                rs_get_Loc.MoveNext
'''            Loop
'''        End If
'''    ElseIf UCase(Left(refcode, 1)) = "L" Then
'''        qry = "select branch_code from branch_master where location_id='" & refcode & "'"
'''        Set rs_get_br = myconn.Execute(qry)
'''        If Not rs_get_br.EOF Then
'''            Do While Not rs_get_br.EOF
'''                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
'''                rs_get_br.MoveNext
'''            Loop
'''        End If
'''    ElseIf Left(refcode, 1) = "2" Then
'''        qry = "select source from employee_master where rm_code='" & refcode & "'"
'''        Set rs_get_br = myconn.Execute(qry)
'''        If Not rs_get_br.EOF Then
'''            Do While Not rs_get_br.EOF
'''                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
'''                rs_get_br.MoveNext
'''            Loop
'''        End If
'''    ElseIf Left(refcode, 1) = "1" Then
'''        BRCODELIST = BRCODELIST & "#" & refcode
'''    ElseIf Left(refcode, 1) = "7" Then
'''        qry = "select branch_code from branch_master"
'''        Set rs_get_br = myconn.Execute(qry)
'''        If Not rs_get_br.EOF Then
'''            Do While Not rs_get_br.EOF
'''                BRCODELIST = BRCODELIST & "#" & rs_get_br(0)
'''                rs_get_br.MoveNext
'''            Loop
'''        End If
'''    End If
'''    getBranchName = BRCODELIST
'''End Function
'''Private Sub ShowFilterData()
'''Dim sql As String
'''    sql = ""
'''        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
'''            ''''''''''''''''''''''''''' Client Search - Branch Category wise'''''''''''''''''''''''''''''''''''''''''''''''''''''''
'''            ' if branch is not in IAG-187 and WMG=188 then a RM can see all the clients in the branch''''''''''''''''''''''''''''''
'''            If currentForm.Name = "FrmtransactionNew" Then
'''                If CmbClientBroker.Text = "CLIENT" Then
'''                    If SRmCode <> "" Then
'''                        temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & val(Branches)
'''                        Set R = myconn.Execute(temp_Sql)
'''                        If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
'''                            sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                        Else
'''                            sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                        End If
'''                    Else
'''                        ''sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                        sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+)  and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                        ''and upper(INVESTOR_name||address1||address2||phone) like upper('%'" & Txtname & "'%'" & txtAdd1 & "'%'" & txtAdd2 & "'%'" & txtPhone & "'%')
'''                    End If
'''                ElseIf CmbClientBroker.Text = "SUB BROKER" Then
'''                    If SRmCode <> "" Then
'''                        temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & val(Branches)
'''                        Set R = myconn.Execute(temp_Sql)
'''                        If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
'''                            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                        Else
'''                            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                        End If
'''                    Else
'''                        sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'''                    End If
'''                End If
'''            End If
'''    '******************************************************************************************
'''    End If
'''
'''
'''   ''**************ADDED BY JAWAHAR
'''            If CmbClientBroker.Text = "CLIENT" Then
'''                sql = sql & " and cm.client_code=a.source_id"
'''            End If
'''            If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by Pravesh Chandel on 18 nov 2005
'''                sql = sql & " and lpad(a.inv_code,1)=3"
'''            ElseIf CmbClientBroker.Text = "CLIENT" Then
'''                sql = sql & " and lpad(a.inv_code,1)=4"
'''            End If
'''
'''            If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And cmbCity.ListIndex = -1 And txtCliSubName.Text = "" Then
'''                sql = sql & " and  B.BRANCH_CODE in(" & Branches & ")"
'''            Else
'''                If Txtcode.Text <> "" Then
'''                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
'''                        sql = sql & " and source_id in (select agent_code from agent_master where (upper(exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%' OR TO_CHAR(agent_code)='" & Trim(Txtcode.Text) & "')) "
'''                    ElseIf CmbClientBroker.Text = "CLIENT" Then
'''                        sql = sql & " and source_id in (select client_code from client_master where (upper(exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%' OR TO_CHAR(client_code)='" & Trim(Txtcode.Text) & "')) "
'''                    End If
'''                End If
'''
'''                If txtCliSubName.Text <> "" Then
'''                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
'''                        sql = sql & " and source_id in (select agent_code from agent_master where upper(trim(agent_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
'''                    ElseIf CmbClientBroker.Text = "CLIENT" Then
'''                        sql = sql & " and source_id in (select client_code from client_master where upper(trim(client_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
'''                    End If
'''                End If
'''                If Txtname.Text <> "" Then
'''                    sql = sql & " and upper(a.investor_name) like '%" & Replace(UCase(Trim(Txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''''                    If CmbClientBroker.Text = "CLIENT" Then
'''''                    sql = sql & " and upper(A.INVESTOR_name||CM.address1||CM.address2||A.phone) like upper('%" & Txtname & "%" & txtAdd1 & "%" & txtAdd2 & "%" & txtPhone & "%')"
'''''                    ElseIf CmbClientBroker.Text = "SUB BROKER" Then
'''''                    sql = sql & " and upper(A.INVESTOR_name||a.address1||a.address2||A.phone) like upper('%" & Txtname & "%" & txtAdd1 & "%" & txtAdd2 & "%" & txtPhone & "%')"
'''''                    End If
'''                End If
'''                If CmbClientBroker.Text = "SUB BROKER" Then
'''                    If txtAdd1.Text <> "" Then
'''                        sql = sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                    End If
'''                    If txtAdd2.Text <> "" Then
'''                        sql = sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                    End If
'''                    If cmbCity.ListIndex <> -1 Then
'''                        sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                    End If
'''                ElseIf CmbClientBroker.Text = "CLIENT" Then
'''                    If txtAdd1.Text <> "" Then
'''                        sql = sql & " and upper(cm.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                    End If
'''                    If txtAdd2.Text <> "" Then
'''                        sql = sql & " and upper(cm.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                    End If
'''                    If cmbCity.ListIndex <> -1 Then
'''                        sql = sql & " and cm.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                    End If
'''                End If
'''                If txtPhone.Text <> "" Then
'''                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                End If
'''''                If cmbBranch.ListIndex <> -1 Then
'''''                    sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''''                Else
'''''                    sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
'''''                End If
'''            End If
'''            If cmbNewRM.ListIndex <> -1 Then
'''                sql = sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
'''            End If
'''    Populate_Data sql
'''End Sub
'''Private Sub chkIndia_Click()
'''    If chkIndia.Value = 1 Then
'''        cmbBranch.Enabled = False
'''        cmbCity.Enabled = False
'''    Else
'''        cmbBranch.Enabled = True
'''        cmbCity.Enabled = True
'''    End If
'''End Sub
'''Private Sub cmbbranch_Change()
'''Dim rsData As New ADODB.Recordset
'''
'''    txtRM.Text = ""
'''    cmbNewRM.Clear
'''    If cmbBranch.ListIndex <> -1 Then
'''        If SRmCode <> "" Then
'''            rsData.open "select * from employee_master where type='A' and source=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " and rm_code in (" & SRmCode & ") order by RM_NAME", myconn, adOpenForwardOnly
'''        Else
'''            rsData.open "select * from employee_master where type='A' and source=" & cmbBranch.List(cmbBranch.ListIndex, 1) & " order by RM_NAME", myconn, adOpenForwardOnly
'''        End If
'''    Else
'''        If SRmCode <> "" Then
'''            rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") and rm_code in (" & SRmCode & ") order by RM_NAME", myconn, adOpenForwardOnly
'''        Else
'''            rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") order by RM_NAME", myconn, adOpenForwardOnly
'''        End If
'''    End If
'''
'''    'and category_id<>'2002'
'''    J = 0
'''    While Not rsData.EOF
'''        cmbNewRM.AddItem rsData("RM_NAME")
'''        cmbNewRM.List(J, 1) = rsData("PAYROLL_ID")
'''        cmbNewRM.List(J, 2) = rsData("RM_CODE")
'''        J = J + 1
'''        rsData.MoveNext
'''    Wend
'''    rsData.Close
'''End Sub
'''Private Sub cmbBranch_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
'''    If cmbBranch.MatchFound = False Then
'''        cmbBranch.ListIndex = -1
'''    End If
'''End Sub
'''Private Sub cmbCity_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
'''    If cmbCity.MatchFound = False Then
'''        cmbCity.ListIndex = -1
'''    End If
'''End Sub
'''Private Sub CmbClientBroker_Click()
'''If CmbClientBroker.Text = "SUB BROKER" Then
'''Label16.Visible = True
'''Else
'''Label16.Visible = False
'''End If
'''End Sub
'''
'''Private Sub CmdExit_Click()
'''    Unload Me
'''End Sub
'''
'''Private Sub cmdSelect_Click()
'''Dim Found As Boolean
'''    If msfgClients.Rows > 1 Then
'''        Me.MousePointer = 11
'''        For i = msfgClients.Row To (msfgClients.Row + msfgClients.RowSel) - msfgClients.Row
'''                Found = False
'''
'''                For J = 1 To frmRMTransfer1.ClientDetails.Rows - 1
'''                    If frmRMTransfer1.ClientDetails.TextMatrix(J, 4) = msfgClients.TextMatrix(i, 1) Then
'''                        Found = True
'''                        Exit For
'''                    End If
'''                Next J
'''                If Found = False Then
'''                        If frmRMTransfer1.ClientDetails.Rows >= 2 And frmRMTransfer1.ClientDetails.TextMatrix(1, 1) <> "" Then
'''                            frmRMTransfer1.ClientDetails.Rows = frmRMTransfer1.ClientDetails.Rows + 1
'''                        End If
'''
'''                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 1) = msfgClients.TextMatrix(i, 0)
'''                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 4) = msfgClients.TextMatrix(i, 1)
'''                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 3) = msfgClients.TextMatrix(i, 5)
'''                        If Cmbcat.Text = "CLIENT" Then
'''                            Set rs_get_rm = myconn.Execute("select C.rm_code,rm_name from client_master c,employee_master e where client_code=" & msfgClients.TextMatrix(i, 1) & " and c.rm_code=e.rm_code")
'''                        Else
'''                            Set rs_get_rm = myconn.Execute("select C.rm_code,rm_name from agent_master c,employee_master e where agent_code=" & msfgClients.TextMatrix(i, 1) & " and c.rm_code=e.rm_code")
'''                        End If
'''                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 2) = rs_get_rm("rm_name")
'''                        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 5) = rs_get_rm("rm_code")
'''                        frmRMTransfer1.ClientDetails.SetFocus
'''                End If
'''        Next i
'''        Me.Show
'''        Me.MousePointer = 0
'''    End If
'''End Sub
'''
'''Private Sub cmdShow_Click()
'''''MsgBox Len(Branches)
'''    If cmbBranch.ListIndex = -1 And Len(Branches) = 8 Then
'''        MsgBox "Please Select Atleast One Branch ", vbExclamation
'''        cmbBranch.SetFocus
'''        Exit Sub
'''    End If
'''    If Cmbcat.Text <> "" Then
'''        If cmbCity.ListIndex = -1 And Txtcode.Text = "" And Txtname.Text = "" And txtAdd1.Text = "" And txtAdd2.Text = "" And txtPhone.Text = "" Then
'''            If Cmbcat.Text = "INVESTOR" And txtCliSubName.Text = "" Then
'''                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
'''                Txtname.SetFocus
'''                Exit Sub
'''            ElseIf Cmbcat.Text = "CLIENT" Or Cmbcat.Text = "AGENT" Then
'''                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
'''                Txtname.SetFocus
'''                Exit Sub
'''            End If
'''        End If
'''        cmdShow.Enabled = False
'''        Me.MousePointer = 11
'''        msfgClients.Clear
'''        If chkIndia.Value = 1 Then
'''            AllIndiaSearch
'''        Else
'''            ShowFilterData
'''        End If
'''        Set_Grid
'''        'MsgBox msfgClients.Rows - 1 & " Records Shown ! ", vbInformation
'''        Me.MousePointer = 0
'''        cmdShow.Enabled = True
'''    Else
'''        MsgBox "Please Select Category ", vbInformation
'''        Cmbcat.SetFocus
'''    End If
'''End Sub
'''
'''Private Sub Command1_Click()
'''
'''End Sub
'''
'''Private Sub Form_Activate()
'''On Error GoTo err1
'''Dim rsData As New ADODB.Recordset
'''Dim J As Integer
'''    SQLBranches = ""
'''
''''''    If GlbDataFilter = "72" Then
''''''        rsData.open "SELECT BRANCH_CODE FROM BRANCH_MASTER", myconn, adOpenForwardOnly
''''''        While Not rsData.EOF
''''''            SQLBranches = SQLBranches & rsData(0) & ","
''''''            rsData.MoveNext
''''''        Wend
''''''        SQLBranches = Left(SQLBranches, Len(SQLBranches) - 1)
''''''    Else
''''''        SQLBranches = Branches    'BRANCHES VARIABLE IS GLOBAL FOR BRANCH FILTERING PURPOSE
''''''    End If
'''    cmbBranch.Clear
'''    If rsData.State = 1 Then rsData.Close
'''    rsData.open "Select branch_code,branch_name from branch_master where branch_code in(" & Branches & ") order by branch_name", myconn, adOpenForwardOnly
'''    J = 0
'''    While Not rsData.EOF
'''        cmbBranch.AddItem rsData("branch_name")
'''        cmbBranch.List(J, 1) = rsData("branch_code")
'''        J = J + 1
'''        rsData.MoveNext
'''    Wend
'''    cmbBranch.ListIndex = 0
'''    rsData.Close
'''    ''************* for city
'''    cmbCity.Clear
'''    If rsData.State = 1 Then rsData.Close
'''    rsData.open "Select city_id,city_name from city_master order by city_name", myconn, adOpenForwardOnly
'''    J = 0
'''    While Not rsData.EOF
'''        cmbCity.AddItem rsData("City_name")
'''        cmbCity.List(J, 1) = rsData("City_id")
'''        J = J + 1
'''        rsData.MoveNext
'''    Wend
'''    rsData.Close
'''    ''*****************for rm
'''    cmbNewRM.Clear
'''    If SRmCode <> "" Then
'''     rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") and rm_code in (" & SRmCode & ") order by RM_NAME", myconn, adOpenForwardOnly
'''    Else
'''     rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") order by RM_NAME", myconn, adOpenForwardOnly
'''    End If
'''    'and category_id<>'2002'
'''    J = 0
'''    While Not rsData.EOF
'''        cmbNewRM.AddItem rsData("RM_NAME")
'''        cmbNewRM.List(J, 1) = rsData("PAYROLL_ID")
'''        cmbNewRM.List(J, 2) = rsData("RM_CODE")
'''        J = J + 1
'''        rsData.MoveNext
'''    Wend
'''    rsData.Close
'''    Exit Sub
'''
'''err1:
'''    MsgBox err.Description
'''
'''End Sub
'''
'''Private Sub Form_KeyPress(KeyAscii As Integer)
'''    If KeyAscii = 13 Then
'''        SendKeys "{TAB}"
'''    End If
'''End Sub
'''
'''Private Sub Form_Load()
''''Image1.Picture = LoadPicture(App.Path & "\logo1.JPG")
'''Me.Icon = LoadPicture(App.Path & "\W.ICO")
'''''added new
'''server = ""
'''exeserver = ""
'''otherpara = ""
'''file = App.Path & "\mfi_server.srv"
'''Open file For Input As #1
'''Line Input #1, server
'''Line Input #1, expserver
'''If Not EOF(1) Then
'''   Line Input #1, otherpara
'''End If
'''Close #1
'''para = Split(server, "#")
'''    'MSRClient.Connect = "192.168.0.21"
'''    MSRClient.DataSourceName = "TEST"
'''    MSRClient.UserName = "wealthmaker"
'''    MSRClient.Password = DataBasePassword
'''    'MSRClient.UserName = "datatest"
'''    'MSRClient.Password = "testdata"
'''    Cmbcat.Clear
'''    Row = 1
'''    Txtcode.Text = ""
'''    Txtname.Text = ""
'''    If UCase(currentForm.Name) = "FRMUPDATECLIENT" Then
'''        Cmbcat.AddItem "CLIENT"
'''        Cmbcat.AddItem "INVESTOR"
'''    Else
'''        Cmbcat.AddItem "CLIENT"
'''        Cmbcat.AddItem "AGENT"
'''        Cmbcat.AddItem "INVESTOR"
'''    End If
'''End Sub
'''Private Sub Form_Unload(Cancel As Integer)
'''    strForm = ""
'''    ReportFlag = False
'''End Sub
'''
'''Private Sub msfgClients_dblClick()
'''On Error GoTo err1
'''Dim rs_get_type As ADODB.Recordset
'''Dim i As Integer
'''Dim susPense As String
'''Dim Found As Boolean
'''If msfgClients.Rows > 1 Then
'''    AllIndia_Inv_code = ""
'''    Set rs_get_type = New ADODB.Recordset
'''    Me.MousePointer = vbHourglass
'''    susPense = ""
'''    R = msfgClients.Row
'''
'''
'''
'''
'''
'''If currentForm.Name <> "frmAR" And currentForm.Name <> "frmARGeneral" And currentForm.Name <> "FrmVenDayBook" Then
'''If chkIndia.Value = 0 Then
'''    AllIndia = False
'''Dim tr As TreeView
'''    Set tr = currentForm.Controls(treeName)
'''    preSelectedCode = "MFIBR"
'''Dim Fcode As String
'''Dim Pcode As Long
'''    nodeValue = msfgClients.TextMatrix(R, 1)
'''    If currentForm.Name = "FrmtransactionNew" Then FrmtransactionNew.cmbBusiBranch.Clear
'''Dim Get_rm As New ADODB.Recordset
'''    If UCase(Trim(Cmbcat.Text)) = "CLIENT" Then
'''        Set Get_rm = myconn.Execute("select rm_code from client_master where client_code=" & nodeValue)
'''        If Not Get_rm.EOF Then
'''            Pcode = Get_rm(0)
'''        End If
'''        'treeClientFill currentForm.Controls(treeName), "R" & CStr(pcode), currentForm.Controls(treeName).Nodes("R" & pcode).Child.Key  'BY HIMANSHU
'''        treeClientFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Key, msfgClients.TextMatrix(R, 1) 'BY HIMANSHU LATEST
'''        currentForm.Controls(treeName).Nodes("C" & CStr(nodeValue)).Selected = True
'''        currentForm.Controls(treeName).Nodes("C" & CStr(nodeValue)).Bold = True
'''        currentForm.Controls(treeName).Nodes("C" & CStr(nodeValue)).Expanded = True
'''        If currentForm.Name = "FrmtransactionNew" Then
'''            FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
'''            FrmtransactionNew.userType = "CLIENT"
'''        End If
'''    End If
'''    If UCase(Trim(Cmbcat.Text)) = "AGENT" Then
'''        Set Get_rm = myconn.Execute("select rm_code from agent_master where agent_code=" & nodeValue)
'''        If Not Get_rm.EOF Then
'''            Pcode = Get_rm(0)
'''        End If
'''        'treeAgentFill currentForm.Controls(treeName), "R" & CStr(pcode), currentForm.Controls(treeName).Nodes("R" & CStr(pcode)).Child.Key, Left(msfgClients.TextMatrix(r, 1), 8) 'BY HIMANSHU LATEST  06-SEP
'''        treeAgentFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & CStr(Pcode)).Child.Next.Key, Left(msfgClients.TextMatrix(R, 1), 8) 'BY HIMANSHU LATEST  06-SEP
'''        currentForm.Controls(treeName).Nodes("A" & CStr(nodeValue)).Selected = True
'''        currentForm.Controls(treeName).Nodes("A" & CStr(nodeValue)).Bold = True
'''        currentForm.Controls(treeName).Nodes("A" & CStr(nodeValue)).Expanded = True
'''        If currentForm.Name = "FrmtransactionNew" Then
'''            FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
'''            FrmtransactionNew.userType = "AGENT"
'''        End If
'''    End If
'''    If UCase(Trim(Cmbcat.Text)) = "INVESTOR" Then
'''        Set rs_get_invsrc = myconn.Execute("Select source_id from investor_master where inv_code=" & nodeValue)
'''        If Left(rs_get_invsrc(0), 1) = 4 Then
'''            Set rs_get_rm = myconn.Execute("select rm_code from client_master where client_code=" & rs_get_invsrc(0))
'''            If Not rs_get_rm.EOF Then
'''                Pcode = rs_get_rm(0)
'''                    'treeClientFill currentForm.Controls(treeName), "R" & CStr(pcode), currentForm.Controls(treeName).Nodes("R" & pcode).Child.Key
'''                    treeClientFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Key, Left(msfgClients.TextMatrix(R, 1), 8) 'BY HIMANSHU LATEST
'''                    treeInvestorFill currentForm.Controls(treeName), "C" & rs_get_invsrc(0), currentForm.Controls(treeName).Nodes("C" & rs_get_invsrc(0)).Child.Key
'''                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Selected = True
'''                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Bold = True
'''                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Expanded = True
'''                If currentForm.Name = "FrmtransactionNew" Then
'''                    FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
'''                    FrmtransactionNew.userType = "INVESTOR"
'''                End If
'''            End If
'''        End If
'''        If Left(rs_get_invsrc(0), 1) = 3 Then
'''            Set rs_get_rm = myconn.Execute("select rm_code from agent_master where agent_code=" & rs_get_invsrc(0))
'''            If Not rs_get_rm.EOF Then
'''                Pcode = rs_get_rm(0)
'''                treeAgentFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Next.Key, Left(msfgClients.TextMatrix(R, 1), 8)
'''                treeInvestorFill currentForm.Controls(treeName), "A" & rs_get_invsrc(0), currentForm.Controls(treeName).Nodes("A" & rs_get_invsrc(0)).Child.Key
'''                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Selected = True
'''                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Expanded = True
'''            End If
'''        End If
'''    End If
'''Else
'''    AllIndia = True
'''    AllIndia_Inv_code = msfgClients.TextMatrix(R, 1)
'''End If
'''    If susPense = "" Then
'''    End If
'''    Me.MousePointer = vbNormal
'''    Unload Me
'''End If
'''
'''
'''    If currentForm.Name = "FrmtransactionNew" Then
'''        FrmtransactionNew.treeClient_Click
'''        DoEvents
'''        If FrmtransactionNew.cmbBankName.Enabled = True Then
'''            FrmtransactionNew.cmbBankName.SetFocus
'''        End If
'''        DoEvents
'''    End If
'''End If
'''
'''
'''
'''Exit Sub
'''err1:
'''    Me.MousePointer = 0
'''    MsgBox err.Description
'''End Sub
'''Private Sub lstname_KeyDown(KeyCode As Integer, Shift As Integer)
'''    If KeyCode = 27 Then
'''        lstname.Visible = False
'''        If setfoc = "txtinvname" Then
'''            txtInvName.SetFocus
'''            txtInvName.SelStart = Len(txtInvName)
'''        ElseIf setfoc = "txtclname" Then
'''            txtclname.SetFocus
'''            txtclname.SelStart = Len(txtclname)
'''        ElseIf setfoc = "txtclcode" Then
'''            txtClcode.SetFocus
'''            txtClcode.SelStart = Len(txtClcode)
'''        ElseIf setfoc = "txtagcode" Then
'''            txtAgCode.SetFocus
'''            txtAgCode.SelStart = Len(txtAgCode)
'''        End If
'''    ElseIf KeyCode = 13 Then
'''        lstname.Visible = False
'''        If setfoc = "txtinvname" Then
'''            txtInvName = lstname.Text
'''            txtInvName.SetFocus
'''            txtInvName.SelStart = Len(txtInvName)
'''        ElseIf setfoc = "txtclname" Then
'''            txtclname = lstname.Text
'''            txtclname.SetFocus
'''            txtclname.SelStart = Len(txtclname)
'''        ElseIf setfoc = "txtclcode" Then
'''            txtClcode = lstname.Text
'''            txtClcode.SetFocus
'''            txtClcode.SelStart = Len(txtClcode)
'''        ElseIf setfoc = "txtagcode" Then
'''            txtAgCode = lstname.Text
'''            txtAgCode.SetFocus
'''            txtAgCode.SelStart = Len(txtAgCode)
'''        End If
'''    End If
'''End Sub
'''Private Sub msfgClients_KeyPress(KeyAscii As Integer)
'''If KeyAscii = 13 Then
'''    Call msfgClients_dblClick
'''End If
'''End Sub
'''
'''Private Sub Populate_Data(strQuery As String)
'''On Error GoTo err1
'''    MSRClient.sql = ""
'''    MSRClient.sql = strQuery
'''    MSRClient.Refresh
'''    Exit Sub
'''err1:
'''    'MsgBox Err.Description
'''End Sub
'''Private Sub Set_Grid()
'''On Error GoTo err1
'''    msfgClients.Row = 0
'''    msfgClients.ColWidth(0) = "2700"
'''    msfgClients.Text = "Name"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 1
'''    msfgClients.ColWidth(1) = "0"
'''    msfgClients.Text = "Code"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 2
'''    msfgClients.ColWidth(2) = "1600"
'''    msfgClients.Text = "Address1"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 3
'''    msfgClients.ColWidth(3) = "1600"
'''    msfgClients.Text = "Address2"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 4
'''    msfgClients.ColWidth(4) = "1100"
'''    msfgClients.Text = "City"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 5
'''    msfgClients.ColWidth(5) = "1100"
'''    msfgClients.Text = "Branch"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 6
'''    msfgClients.ColWidth(6) = "1000"
'''    msfgClients.Text = "Phone"
'''    msfgClients.CellFontBold = True
'''    msfgClients.Col = 7
'''    msfgClients.ColWidth(7) = "2000"
'''    msfgClients.Text = "RM"
'''    msfgClients.CellFontBold = True
'''    msfgClients.SetFocus
'''    If msfgClients.Rows > 1 Then
'''        msfgClients.Row = 1
'''    End If
'''    msfgClients.Col = 0
'''    Exit Sub
'''err1:
'''    'MsgBox Err.Description
'''End Sub
'''
'''Private Sub AllIndiaSearch()
'''Dim sql As String
'''        sql = ""
'''        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
'''            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
'''
'''            If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" Then
'''                'Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
'''            Else
'''                If Txtcode.Text <> "" Then
'''                    sql = sql & " and inv_code like '%" & Trim(Txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                End If
'''                If Txtname.Text <> "" Then
'''                    sql = sql & " and upper(a.investor_name) like '" & UCase(Trim(Txtname.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                End If
'''                If txtAdd1.Text <> "" Then
'''                    sql = sql & " and upper(a.address1) like '%" & UCase(Trim(txtAdd1.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                End If
'''                If txtAdd2.Text <> "" Then
'''                    sql = sql & " and upper(a.address2) like '%" & UCase(Trim(txtAdd2.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                End If
'''                If txtPhone.Text <> "" Then
'''                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'''                End If
'''
'''                'Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
'''
'''            End If
'''            sql = sql & " ORDER BY upper(a.investor_name)"
'''        End If
'''        Populate_Data sql
'''
'''End Sub
'''
'''
