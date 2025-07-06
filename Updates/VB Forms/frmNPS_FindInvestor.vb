Dim setfoc As String
Dim Panc As String
Dim RsPan As New ADODB.Recordset
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
'kyc
Dim Sql As String
    Sql = ""
    If strForm <> "Client Transfer" Then
        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
        ''''''''''''''27/3/2007''''''''''''''''''''''   Vinay Hatwal''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ''''''''''''''''''''''''''' Client Search - Branch Category wise'''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ' if branch is not in IAG-187 and WMG=188 then a RM can see all the clients in the branch''''''''''''''''''''''''''''''
            If currentForm.Name = "FrmtransactionNew" Or currentForm.Name = "frmAR" Or currentForm.Name = "frmARGeneral" Or currentForm.Name = "frmAR_Renewal" Then  'jogi010907
                If CmbClientBroker.Text = "CLIENT" Then
                    If SRmCode <> "" Then
                        temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & Val(Branches)
                        Set R = MyConn.Execute(temp_Sql)
                        If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
                            Sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode ,to_char(cm.DOB,'dd/mm/rrrr') DOB,cm.OCC_ID FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c,client_test t where t.client_codekyc(+)=a.inv_code and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP')"
                            'sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP')"
                        Else
                            Sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode ,to_char(cm.DOB,'dd/mm/rrrr') DOB,cm.OCC_ID FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c,client_test t where t.client_codekyc=a.inv_code and  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257" '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                            'sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257" '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                        End If
                    Else
                        Sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode ,to_char(cm.DOB,'dd/mm/rrrr') DOB,cm.OCC_ID FROM client_test t,Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where t.client_codekyc(+)=a.inv_code and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                       ' sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                    End If
                ElseIf CmbClientBroker.Text = "SUB BROKER" Then
                    If SRmCode <> "" Then
                        temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & Val(Branches)
                        Set R = MyConn.Execute(temp_Sql)
                        If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
                            Sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                        Else
                            Sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                        End If
                    Else
                        Sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                    End If
                End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Else
                If CmbClientBroker.Text = "CLIENT" Then
                    If SRmCode = "" Then
                        Sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm,client_test t where  t.client_codekyc(+)=a.inv_code and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and cm.city_id=C.city_id(+) AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP') "
                    Else
                        Sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm,client_test t where t.client_codekyc(+)=a.inv_Code and  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and  INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP') "
                    End If
                ElseIf CmbClientBroker.Text = "SUB BROKER" Then
                    If SRmCode = "" Then
                        Sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c  where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                    Else
                        Sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                    End If
                End If
            End If
            If CmbClientBroker.Text = "CLIENT" Then
                Sql = Sql & " and cm.client_code=a.source_id"
            End If
                
            If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by Pravesh Chandel on 18 nov 2005
                Sql = Sql & " and lpad(a.inv_code,1)=3"
                '------------------Block agent by mayank---------------------------------
                Sql = Sql & " and lpad(a.inv_code,8)not in (select agent_code from agent_master where block_agent='1')"
                '--------------------------------------------------------------
            ElseIf CmbClientBroker.Text = "CLIENT" Then
                Sql = Sql & " and lpad(a.inv_code,1)=4"
            End If
            If Trim(txtcode.Text) = "" And Trim(txtname.Text) = "" And Trim(txtadd1.Text) = "" And Trim(txtadd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(TxtMobile.Text) = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbbranch.ListIndex = -1 And cmbcity.ListIndex = -1 And txtCliSubName.Text = "" Then
              Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            Else
'                If txtcode.Text <> "" Then
'                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
'                        Sql = Sql & " and source_id in (select agent_code from agent_master where upper(exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%')"
'                    ElseIf CmbClientBroker.Text = "CLIENT" Then
'                        Sql = Sql & " and source_id in (select client_code from client_master where upper(exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%')"
'                    End If
'                End If
                
'                If txtcode.Text <> "" Then
'                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
'                        Sql = Sql & " and (source_id in (select agent_code from agent_master where upper(exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%') OR TO_CHAR(SOURCE_ID)='" & Trim(txtcode.Text) & "') "
'                    ElseIf CmbClientBroker.Text = "CLIENT" Then
'                        Sql = Sql & " and (source_id in (select client_code from client_master where upper(exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%') OR TO_CHAR(SOURCE_ID)='" & Trim(txtcode.Text) & "') "
'                    End If
'                End If

                If txtcode.Text <> "" Then
                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
                        Sql = Sql & " and source_id in (select agent_code from agent_master where (upper(exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%' OR TO_CHAR(agent_code)='" & Trim(txtcode.Text) & "')) "
                    ElseIf CmbClientBroker.Text = "CLIENT" Then
                        Sql = Sql & " and source_id in (select client_code from client_master where ( TO_CHAR(client_code)='" & Trim(txtcode.Text) & "')) " 'upper(exist_code) like '%" & UCase(Trim(txtCode.Text)) & "%' OR
                    End If
                End If
                
                If txtCliSubName.Text <> "" Then
                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
                        Sql = Sql & " and source_id in (select agent_code from agent_master where upper(trim(agent_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
                    ElseIf CmbClientBroker.Text = "CLIENT" Then
                        Sql = Sql & " and source_id in (select client_code from client_master where upper(trim(client_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
                    End If
                End If
                
                If txtname.Text <> "" Then
                    Sql = Sql & " and upper(a.investor_name) like '%" & Replace(UCase(Trim(txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If CmbClientBroker.Text = "SUB BROKER" Then
                    If txtadd1.Text <> "" Then
                        Sql = Sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtadd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If txtadd2.Text <> "" Then
                        Sql = Sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtadd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If cmbcity.ListIndex <> -1 Then
                        Sql = Sql & " and a.city_id='" & cmbcity.List(cmbcity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                ElseIf CmbClientBroker.Text = "CLIENT" Then
                    If txtadd1.Text <> "" Then
                        Sql = Sql & " and upper(cm.address1) like '%" & Replace(UCase(Trim(txtadd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If txtadd2.Text <> "" Then
                        Sql = Sql & " and upper(cm.address2) like '%" & Replace(UCase(Trim(txtadd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If cmbcity.ListIndex <> -1 Then
                        Sql = Sql & " and cm.city_id='" & cmbcity.List(cmbcity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                End If
                If txtPhone.Text <> "" Then
                    Sql = Sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtMobile.Text <> "" Then
                    Sql = Sql & " and upper(a.mobile) like '%" & UCase(Trim(TxtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    Sql = Sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtMobile.Text <> "" Then
                    Sql = Sql & " and upper(a.mobile) like '%" & UCase(Trim(TxtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbbranch.ListIndex <> -1 Then
                    Sql = Sql & " and B.BRANCH_CODE=" & cmbbranch.List(cmbbranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If

            End If
            
            If cmbNewRM.ListIndex <> -1 Then
                Sql = Sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            
            
            
'''            If cmbsort.Text = "Name" Then
'''                sql = sql & " ORDER BY upper(a.investor_name)"
'''            ElseIf cmbsort.Text = "Address1" Then
'''                sql = sql & " ORDER BY upper(trim(a.Address1))"
'''            ElseIf cmbsort.Text = "Address2" Then
'''                sql = sql & " ORDER BY upper(trim(a.Address2))"
'''            ElseIf cmbsort.Text = "City" Then
'''                sql = sql & " ORDER BY upper(trim(C.CITY_NAME))"
'''            ElseIf cmbsort.Text = "Phone" Then
'''                sql = sql & " ORDER BY upper(trim(a.phone))"
'''            End If
            
            'Sql = Sql & " ORDER BY upper(a.investor_name)"
        ElseIf Trim(UCase(Cmbcat.Text)) = "CLIENT" Then
            If SRmCode = "" Then
            If GlbDataFilter = "72" Then
                Sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL "
            Else
                Sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
            Else
                Sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
            If Trim(txtcode.Text) = "" And Trim(txtname.Text) = "" And Trim(txtadd1.Text) = "" And Trim(txtadd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(TxtMobile.Text) = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbbranch.ListIndex = -1 And cmbcity.ListIndex = -1 Then
                Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ") "
            Else
'                If txtcode.Text <> "" Then
'                    'Sql = Sql & " and CLIENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                    Sql = Sql & " and upper(a.exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%'"
'                End If
                
                If txtcode.Text <> "" Then
                    If Cmbcat.Text = "CLIENT" Then
                        Sql = Sql & " and TO_CHAR(a.client_code)='" & Trim(txtcode.Text) & "' "
                    Else
                        Sql = Sql & " and (upper(a.exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%') "
                    End If
                End If
                
                If txtname.Text <> "" Then
                    Sql = Sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd1.Text <> "" Then
                    Sql = Sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtadd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd2.Text <> "" Then
                    Sql = Sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtadd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    Sql = Sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtMobile.Text <> "" Then
                    Sql = Sql & " and upper(a.mobile) like '%" & UCase(Trim(TxtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    Sql = Sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbcity.ListIndex <> -1 Then
                    Sql = Sql & " and a.city_id='" & cmbcity.List(cmbcity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbbranch.ListIndex <> -1 Then
                    Sql = Sql & " and B.BRANCH_CODE=" & cmbbranch.List(cmbbranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ") "
                End If
                If currentForm.Name = "frmInvestorMerge" Then
                    Sql = Sql & " and A.RM_CODE = " & txtRM.Text & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If currentForm.Name = "frmClientMerging" And txtRM.Text <> "" Then
                    Sql = Sql & " and A.RM_CODE = " & txtRM.Text & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                
            End If
            If cmbNewRM.ListIndex <> -1 Then
                Sql = Sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
'            If cmbsort.Text = "Name" Then
'                Sql = Sql & " ORDER BY upper(a.CLIENT_NAME)"
'            ElseIf cmbsort.Text = "Address1" Then
'                Sql = Sql & " ORDER BY upper(trim(a.Address1))"
'            ElseIf cmbsort.Text = "Address2" Then
'                Sql = Sql & " ORDER BY upper(trim(a.Address2))"
'            ElseIf cmbsort.Text = "City" Then
'                Sql = Sql & " ORDER BY upper(trim(C.CITY_NAME))"
'            ElseIf cmbsort.Text = "Phone" Then
'                Sql = Sql & " ORDER BY upper(trim(a.phone))"
'            End If
        ElseIf Trim(UCase(Cmbcat.Text)) = "AGENT" Then
            If SRmCode = "" Then
                Sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            Else
                Sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
            If Trim(txtcode.Text) = "" And Trim(txtname.Text) = "" And Trim(txtadd1.Text) = "" And Trim(txtadd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(TxtMobile.Text) = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbbranch.ListIndex = -1 And cmbcity.ListIndex = -1 Then
                Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            Else
                If txtcode.Text <> "" Then
                    'Sql = Sql & " and AGENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    Sql = Sql & " and (upper(a.exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%' or to_char(a.agent_code)='" & Trim(txtcode.Text) & "') "
                End If
                If txtname.Text <> "" Then
                    Sql = Sql & " and upper(a.agent_NAME) like '%" & Replace(UCase(Trim(txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd1.Text <> "" Then
                    Sql = Sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtadd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd2.Text <> "" Then
                    Sql = Sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtadd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    Sql = Sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtMobile.Text <> "" Then
                    Sql = Sql & " and upper(a.Mobile) like '%" & UCase(Trim(TxtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    Sql = Sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbcity.ListIndex <> -1 Then
                    Sql = Sql & " and a.city_id='" & cmbcity.List(cmbcity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbbranch.ListIndex <> -1 Then
                    Sql = Sql & " and B.BRANCH_CODE=" & cmbbranch.List(cmbbranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If currentForm.Name = "frmInvestorMerge" Then
                    Sql = Sql & " and A.RM_CODE = " & txtRM.Text & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                
            End If
             If cmbNewRM.ListIndex <> -1 Then
                Sql = Sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            Sql = Sql & " ORDER BY upper(a.agent_NAME)"
        End If
    
    Else
    
'***********************************
        If Trim(UCase(Cmbcat.Text)) = "CLIENT" Then
            'If SRmCode = "" Then
            
                Sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'            Else
'                Sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND CLIENT_name IS NOT NULL"
            'End If
            Sql = Sql & " and A.FROM_RM_CODE = " & cmbOldRM.List(cmbOldRM.ListIndex, 2) & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            
            'If Trim(txtcode.Text) = "" And Trim(txtName.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And cmbBranch.ListIndex = -1 And Cmbcity.ListIndex = -1 Then
            '    Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            'Else
                If txtcode.Text <> "" Then
                    'Sql = Sql & " and CLIENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    Sql = Sql & " and upper(a.exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtname.Text <> "" Then
                    Sql = Sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd1.Text <> "" Then
                    Sql = Sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtadd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd2.Text <> "" Then
                    Sql = Sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtadd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    Sql = Sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtMobile.Text <> "" Then
                    Sql = Sql & " and upper(a.Mobile) like '%" & UCase(Trim(TxtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    Sql = Sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbcity.ListIndex <> -1 Then
                    Sql = Sql & " and a.city_id='" & cmbcity.List(cmbcity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbbranch.ListIndex <> -1 Then
                    Sql = Sql & " and B.BRANCH_CODE=" & cmbbranch.List(cmbbranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
            'End If
            'Sql = Sql & " ORDER BY upper(a.CLIENT_NAME)"
             If cmbNewRM.ListIndex <> -1 Then
                Sql = Sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            If cmbSort.Text = "Name" Then
                Sql = Sql & " ORDER BY upper(a.CLIENT_NAME)"
            ElseIf cmbSort.Text = "Address1" Then
                Sql = Sql & " ORDER BY upper(trim(a.Address1))"
            ElseIf cmbSort.Text = "Address2" Then
                Sql = Sql & " ORDER BY upper(trim(a.Address2))"
            ElseIf cmbSort.Text = "City" Then
                Sql = Sql & " ORDER BY upper(trim(C.CITY_NAME))"
            ElseIf cmbSort.Text = "Phone" Then
                Sql = Sql & " ORDER BY upper(trim(a.phone))"
            End If
            
        ElseIf Trim(UCase(Cmbcat.Text)) = "AGENT" Then
            'If SRmCode = "" Then
                
                Sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            'Else
            '    Sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and A.FROM_SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND AGENT_name IS NOT NULL"
            'End If
            Sql = Sql & " and A.FROM_RM_CODE = " & cmbOldRM.List(cmbOldRM.ListIndex, 2) & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            
            'If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 Then
            '    Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            'Else
                
                If txtcode.Text <> "" Then
                    'Sql = Sql & " and AGENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    Sql = Sql & " and upper(a.exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtname.Text <> "" Then
                    Sql = Sql & " and upper(a.agent_NAME) like '%" & Replace(UCase(Trim(txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd1.Text <> "" Then
                    Sql = Sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtadd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtadd2.Text <> "" Then
                    Sql = Sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtadd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    Sql = Sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtMobile.Text <> "" Then
                    Sql = Sql & " and upper(a.mobile) like '%" & UCase(Trim(TxtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    Sql = Sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbcity.ListIndex <> -1 Then
                    Sql = Sql & " and a.city_id='" & cmbcity.List(cmbcity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbbranch.ListIndex <> -1 Then
                    Sql = Sql & " and B.BRANCH_CODE=" & cmbbranch.List(cmbbranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    Sql = Sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
            'End If
             If cmbNewRM.ListIndex <> -1 Then
                Sql = Sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            Sql = Sql & " ORDER BY upper(a.agent_NAME)"
        End If
    
    
'***********************************
    End If
    'txtRM.Text = ""
    Populate_Data Sql
End Sub

Private Sub chkIndia_Click()
    If chkIndia.Value = 1 Then
        cmbbranch.Enabled = False
        cmbcity.Enabled = False
    Else
        cmbbranch.Enabled = True
        cmbcity.Enabled = True
    End If
End Sub

Private Sub cmbbranch_Change()
Dim rsData As New ADODB.Recordset
    
    If strForm = "Client Transfer" Then
        cmbOldRM.Clear
    
        If cmbbranch.ListIndex <> -1 Then
            rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code in (select distinct from_rm_code from client_master where from_sourceid =" & cmbbranch.List(cmbbranch.ListIndex, 1) & ") order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code in (select distinct from_rm_code from client_master where from_sourceid in(" & SQLBranches & ")) order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    
        j = 0
        While Not rsData.EOF
            cmbOldRM.AddItem rsData("RM_NAME")
            cmbOldRM.List(j, 1) = rsData("PAYROLL_ID")
            cmbOldRM.List(j, 2) = rsData("RM_CODE")
            j = j + 1
            rsData.MoveNext
        Wend
        rsData.Close
        Set rsData = Nothing
    End If
    txtRM.Text = ""
    
    cmbNewRM.Clear
    If cmbbranch.ListIndex <> -1 Then
        If SRmCode <> "" Then
            rsData.open "select * from employee_master where type='A' and source=" & cmbbranch.List(cmbbranch.ListIndex, 1) & " and rm_code in (" & SRmCode & ") order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            rsData.open "select * from employee_master where type='A' and source=" & cmbbranch.List(cmbbranch.ListIndex, 1) & " order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    Else
        If SRmCode <> "" Then
            rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") and rm_code in (" & SRmCode & ") order by RM_NAME", MyConn, adOpenForwardOnly
        Else
            rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") order by RM_NAME", MyConn, adOpenForwardOnly
        End If
    End If
    
    'and category_id<>'2002'
    j = 0
    While Not rsData.EOF
        cmbNewRM.AddItem rsData("RM_NAME")
        cmbNewRM.List(j, 1) = rsData("PAYROLL_ID")
        cmbNewRM.List(j, 2) = rsData("RM_CODE")
        j = j + 1
        rsData.MoveNext
    Wend
    rsData.Close
End Sub

Private Sub cmbbranch_KeyDown(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
        'SendKeys "{TAB}"
    End If
End Sub
Private Sub cmbBranch_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbbranch.MatchFound = False Then
        cmbbranch.ListIndex = -1
    End If
End Sub

Private Sub cmbCity_KeyDown(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub
Private Sub cmbCity_KeyUp(KeyCode As MSForms.ReturnInteger, Shift As Integer)
    If cmbcity.MatchFound = False Then
        cmbcity.ListIndex = -1
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
'kyc
If Len(Branches) = 8 Then
If cmbbranch.ListIndex = -1 Then
        MsgBox "Please Select Atleast One Branch ", vbExclamation
        cmbbranch.SetFocus
        Exit Sub
    End If
End If
    If Cmbcat.Text <> "" Then
        If cmbbranch.ListIndex = -1 And cmbcity.ListIndex = -1 And txtcode.Text = "" And txtname.Text = "" And txtadd1.Text = "" And txtadd2.Text = "" And txtPhone.Text = "" And TxtMobile.Text = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbNewRM.ListIndex = -1 Then
            If Cmbcat.Text = "INVESTOR" And txtCliSubName.Text = "" Then
                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
                txtname.SetFocus
                Exit Sub
            ElseIf Cmbcat.Text = "CLIENT" Or Cmbcat.Text = "AGENT" Then
                MsgBox "Please Enter Atleast One Searching Criteria !", vbExclamation
                txtname.SetFocus
                Exit Sub
            End If
        End If
        cmdShow.Enabled = False
        Me.MousePointer = 11
        msfgClients.Clear
        If txtname.Text = "" Then
            If (TxtPanNo.Text <> "" Or txtAccountCode.Text <> "") And (txtname.Text = "" And txtadd1.Text = "" And txtadd2.Text = "" And txtPhone.Text = "" And TxtMobile.Text = "" And txtcode.Text = "") Then
                chkIndia.Value = 1
                AllIndiaSearchFlag = "ALL"
            Else
                AllIndiaSearchFlag = "SPECIFIC"
                chkIndia.Value = 0
            End If
        Else
            If (txtname.Text <> "" And ImEntryDt.Text <> "__/__/____") And (txtadd1.Text = "" And txtadd2.Text = "" And txtPhone.Text = "" And TxtMobile.Text = "" And txtcode.Text = "" And TxtPanNo.Text = "" And txtAccountCode.Text = "") Then
                chkIndia.Value = 1
                AllIndiaSearchFlag = "ALL"
            Else
                AllIndiaSearchFlag = "SPECIFIC"
                chkIndia.Value = 0
            End If
        End If
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
    End If
End Sub

Private Sub Command1_Click()

End Sub

Private Sub Form_Activate()
On Error GoTo err1
Dim rsData As New ADODB.Recordset
Dim j As Integer
    SQLBranches = ""
    If GlbDataFilter = "72" Then
        rsData.open "SELECT BRANCH_CODE FROM BRANCH_MASTER where (BRANCH_TYPE <>'Inactive' OR BRANCH_TYPE IS NULL)", MyConn, adOpenForwardOnly
        While Not rsData.EOF
            SQLBranches = SQLBranches & rsData(0) & ","
            rsData.MoveNext
        Wend
        SQLBranches = Left(SQLBranches, Len(SQLBranches) - 1)
    Else
        SQLBranches = Branches    'BRANCHES VARIABLE IS GLOBAL FOR BRANCH FILTERING PURPOSE
    End If
    cmbcity.Clear
    If rsData.State = 1 Then rsData.Close
    rsData.open "Select city_id,city_name from city_master order by city_name", MyConn, adOpenForwardOnly
    j = 0
    While Not rsData.EOF
        cmbcity.AddItem rsData("City_name")
        cmbcity.List(j, 1) = rsData("City_id")
        j = j + 1
        rsData.MoveNext
    Wend
    rsData.Close
    cmbbranch.Clear
    rsData.open "Select branch_code,branch_name from branch_master where branch_code in(" & SQLBranches & ") order by branch_name", MyConn, adOpenForwardOnly
    j = 0
    While Not rsData.EOF
        cmbbranch.AddItem rsData("branch_name")
        cmbbranch.List(j, 1) = rsData("branch_code")
        j = j + 1
        rsData.MoveNext
    Wend
    cmbbranch.ListIndex = 0
    rsData.Close
    
    If currentForm.Name = "frmRMTransfer1" Then
        cmbOldRM.Clear
        rsData.open "Select RM_CODE,RM_NAME,PAYROLL_ID from EMPLOYEE_MASTER where rm_code in (select from_rm_code from client_master where from_sourceid in(" & SQLBranches & ")) order by RM_NAME", MyConn, adOpenForwardOnly
        j = 0
        While Not rsData.EOF
            cmbOldRM.AddItem rsData("RM_NAME")
            cmbOldRM.List(j, 1) = rsData("PAYROLL_ID")
            cmbOldRM.List(j, 2) = rsData("RM_CODE")
            j = j + 1
            rsData.MoveNext
        Wend
        rsData.Close
    End If

    cmbNewRM.Clear
    If SRmCode <> "" Then
        rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") and rm_code in (" & SRmCode & ") order by RM_NAME", MyConn, adOpenForwardOnly
    Else
        rsData.open "select * from employee_master where type='A' and source in (" & Branches & ") order by RM_NAME", MyConn, adOpenForwardOnly
    End If
    'and category_id<>'2002'
    j = 0
    While Not rsData.EOF
        cmbNewRM.AddItem rsData("RM_NAME")
        cmbNewRM.List(j, 1) = rsData("PAYROLL_ID")
        cmbNewRM.List(j, 2) = rsData("RM_CODE")
        j = j + 1
        rsData.MoveNext
    Wend
    rsData.Close

    
    
  ''''Added by pravesh
    If currentForm.Name = "frmAR" Then
        For j = 0 To cmbbranch.ListCount - 1
            If cmbbranch.List(j, 1) = frmAR.BrCode Then
                cmbbranch.ListIndex = j
                Exit For
            End If
        Next
    End If
    If currentForm.Name = "frmAR_Renewal" Then 'jogi010907
        For j = 0 To cmbbranch.ListCount - 1
            If cmbbranch.List(j, 1) = frmAR_Renewal.BrCode Then
                cmbbranch.ListIndex = j
                Exit For
            End If
        Next
    End If
    
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
'Image1.Picture = LoadPicture(App.Path & "\logo1.JPG")
Me.Icon = LoadPicture(App.Path & "\W.ICO")
''added new
server = ""
exeserver = ""
ImEntryDt.Text = "__/__/____"
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
'MSRClient.Connect = "192.168.0.21"
MSRClient.DataSourceName = "test"
MSRClient.UserName = DataBaseUser
MSRClient.Password = DataBasePassword
'MSRClient.UserName = "datatest"
'MSRClient.Password = "testdata"
Cmbcat.Clear
Row = 1
txtcode.Text = ""
txtname.Text = ""
If UCase(currentForm.Name) = "FRMUPDATECLIENT" Then
    Cmbcat.AddItem "CLIENT"
    Cmbcat.AddItem "INVESTOR"
Else
    Cmbcat.AddItem "CLIENT"
    Cmbcat.AddItem "AGENT"
    Cmbcat.AddItem "INVESTOR"
End If
If UCase(currentForm.Name) = "FRMAR" Or UCase(currentForm.Name) = "FRMARGENERAL" Or UCase(currentForm.Name) = "FRMTRANSACTIONNEW" Or UCase(currentForm.Name) = "FRMACTOPEN" Then
   lblAccode.Visible = True
   lblPan.Visible = True
   TxtPanNo.Visible = True
   txtAccountCode.Visible = True
Else
   lblAccode.Visible = False
   lblPan.Visible = False
   TxtPanNo.Visible = False
   txtAccountCode.Visible = False
End If
End Sub



Private Sub Form_Unload(Cancel As Integer)
    strForm = ""
    ReportFlag = False
End Sub


Private Sub msfgClients_dblClick()
On Error GoTo err1
On Error Resume Next
Dim rsexistcode As ADODB.Recordset
Dim rs_get_type As ADODB.Recordset, rs_get_rm As ADODB.Recordset, rs_get_invsrc As ADODB.Recordset
Dim i As Integer
Dim susPense As String
Dim Found As Boolean
If msfgClients.Rows > 1 Then
    AllIndia_Inv_code = ""
    Set rs_get_type = New ADODB.Recordset
    Me.MousePointer = vbHourglass
    susPense = ""
    R = msfgClients.Row
    '-------comment by mayank----------------
    If MyInsured = "INSURED" Then
        If currentForm.Name <> "frmactopen" Then
            If Mid(msfgClients.TextMatrix(R, 1), 1, 1) = "4" And SqlRet("SELECT BRANCH_TAR_CAT FROM BRANCH_MASTER WHERE BRANCH_CODE IN (SELECT BRANCH_CODE FROM INVESTOR_MASTER WHERE INV_CODE='" & msfgClients.TextMatrix(R, 1) & "')") <> 186 Then
                If msfgClients.TextMatrix(R, 9) = "YES" Or msfgClients.TextMatrix(R, 9) = "YESP" Then
                Else
                    MsgBox "Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same", vbInformation, "Wealthmaker"
                    Me.MousePointer = vbIconPointer
                    Exit Sub
                End If
             End If
        End If
    End If
   
  If currentForm.Name <> "frmactopen" Then
    If UCase(Trim(currentForm.Name)) = UCase(Trim("frmtransactionnew")) Then
            If Mid(msfgClients.TextMatrix(R, 1), 1, 1) = "4" And SqlRet("SELECT BRANCH_TAR_CAT FROM BRANCH_MASTER WHERE BRANCH_CODE IN (SELECT BRANCH_CODE FROM INVESTOR_MASTER WHERE INV_CODE='" & msfgClients.TextMatrix(R, 1) & "')") <> 186 Then
                If msfgClients.TextMatrix(R, 9) = "YES" Or msfgClients.TextMatrix(R, 9) = "YESP" Then
                Else
                    MsgBox "Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same", vbInformation, "Wealthmaker"
                    Me.MousePointer = vbIconPointer
                    Exit Sub
                End If
         End If
    End If
  End If
    '----------comment by mayank------------------
    If strForm = "Client Transfer" Then
        For i = 1 To frmRMTransfer1.ClientDetails.Rows - 1
            If frmRMTransfer1.ClientDetails.TextMatrix(i, 4) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Client has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
        Next
        If frmRMTransfer1.ClientDetails.Rows >= 2 And frmRMTransfer1.ClientDetails.TextMatrix(1, 1) <> "" Then
            frmRMTransfer1.ClientDetails.Rows = frmRMTransfer1.ClientDetails.Rows + 1
        End If
        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 1) = msfgClients.TextMatrix(R, 0)
        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 4) = msfgClients.TextMatrix(R, 1)
        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 3) = msfgClients.TextMatrix(R, 5)
        If Cmbcat.Text = "CLIENT" Then
            Set rs_get_rm = MyConn.Execute("select C.rm_code,rm_name from client_master c,employee_master e where client_code=" & msfgClients.TextMatrix(R, 1) & " and c.rm_code=e.rm_code")
        Else
            Set rs_get_rm = MyConn.Execute("select C.rm_code,rm_name from agent_master c,employee_master e where agent_code=" & msfgClients.TextMatrix(R, 1) & " and c.rm_code=e.rm_code")
        End If
        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 2) = rs_get_rm("rm_name")
        frmRMTransfer1.ClientDetails.TextMatrix(frmRMTransfer1.ClientDetails.Rows - 1, 5) = rs_get_rm("rm_code")
        frmRMTransfer1.ClientDetails.SetFocus
        Me.MousePointer = vbNormal
        Me.Show
        Exit Sub
    End If
    If currentForm.Name = "frmInvestorMerge" Then
        Found = False
        For i = 0 To frmInvestorMerge.cmbClient.ListCount - 1
            If frmInvestorMerge.cmbClient.List(i, 1) = msfgClients.TextMatrix(msfgClients.Row, 1) Then
                frmInvestorMerge.cmbClient.ListIndex = i
                frmInvestorMerge.cmbClient_Change
                Found = True
                Exit For
            End If
        Next i
        If Found = False Then
            MsgBox "This Client is not Having Duplicate Investors ", vbInformation
        Else
            Unload Me
        End If
        Me.MousePointer = vbNormal
        Exit Sub
    End If
    If currentForm.Name = "frmClientInvestmentReport" Then
        For i = 1 To frmClientInvestmentReport.ClientDetails.Rows - 1
            If frmClientInvestmentReport.ClientDetails.TextMatrix(i, 4) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Client has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
        Next
        If frmClientInvestmentReport.ClientDetails.Rows >= 2 And frmClientInvestmentReport.ClientDetails.TextMatrix(1, 1) <> "" Then
            frmClientInvestmentReport.ClientDetails.Rows = frmClientInvestmentReport.ClientDetails.Rows + 1
        End If
        frmClientInvestmentReport.ClientDetails.TextMatrix(frmClientInvestmentReport.ClientDetails.Rows - 1, 1) = msfgClients.TextMatrix(R, 0)
        frmClientInvestmentReport.ClientDetails.TextMatrix(frmClientInvestmentReport.ClientDetails.Rows - 1, 4) = msfgClients.TextMatrix(R, 1)
        frmClientInvestmentReport.ClientDetails.TextMatrix(frmClientInvestmentReport.ClientDetails.Rows - 1, 3) = msfgClients.TextMatrix(R, 4)
        If Cmbcat.Text = "CLIENT" Then
            Set rs_get_rm = MyConn.Execute("select C.rm_code,rm_name from client_master c,employee_master e where client_code=" & msfgClients.TextMatrix(R, 1) & " and c.rm_code=e.rm_code")
        Else
            Set rs_get_rm = MyConn.Execute("select C.rm_code,rm_name from agent_master c,employee_master e where agent_code=" & msfgClients.TextMatrix(R, 1) & " and c.rm_code=e.rm_code")
        End If
        frmClientInvestmentReport.ClientDetails.TextMatrix(frmClientInvestmentReport.ClientDetails.Rows - 1, 2) = frmtree_search.msfgClients.TextMatrix(R, 2) & " " & frmtree_search.msfgClients.TextMatrix(R, 3)
        frmClientInvestmentReport.ClientDetails.TextMatrix(frmClientInvestmentReport.ClientDetails.Rows - 1, 5) = rs_get_rm("rm_code")
        Me.MousePointer = vbNormal
        Me.Show
        frmtree_search.msfgClients.SetFocus
        DoEvents
        Exit Sub
    End If
    
    If currentForm.Name = "frmClientMerging" Then
    Dim MaxRow As Integer
    Dim rsCount As New ADODB.Recordset
    
        For i = 1 To frmClientMerging.msfgInvestors.Rows - 1
            If frmClientMerging.msfgInvestors.TextMatrix(i, 9) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Client has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
            
            If Trim(frmClientMerging.msfgInvestors.TextMatrix(i, 6)) <> Trim(msfgClients.TextMatrix(R, 5)) Then
                If Trim(frmClientMerging.msfgInvestors.TextMatrix(i, 6)) <> "MF Data Import" And Trim(msfgClients.TextMatrix(R, 5)) <> "MF Data Import" Then
                    MsgBox "The Clients should be of the same Branch ", vbInformation
                    Me.MousePointer = vbNormal
                    Exit Sub
                End If
            End If
        Next
        
        With frmClientMerging.msfgInvestors
        
          .Rows = .Rows + 1
            MaxRow = .Rows - 1
            
            rsCount.open "select sum(cnt) cnt from(select count(*) cnt from transaction_st where source_code=" & msfgClients.TextMatrix(R, 1) & " and (ASA<>'C' or ASA is null) and (flag<>'NEWTRAN' or flag is null) " _
                & " Union All " _
                & "select count(*) cnt from transaction_sttemp where source_code=" & msfgClients.TextMatrix(R, 1) & " and (ASA<>'C' or ASA is null) " _
                & " Union All " _
                & "select count(*) cnt from bajaj_ar_head where lpad(client_cd,8)=" & msfgClients.TextMatrix(R, 1) & " and (status_cd<>'B' or status_cd is null)) ", MyConn, adOpenForwardOnly
                
            .TextMatrix(MaxRow, 0) = rsCount("cnt")
            rsCount.Close
            rsCount.open "Select last_tran_dt1 from client_master where client_code=" & msfgClients.TextMatrix(R, 1), MyConn, adOpenForwardOnly
            If Not IsNull(rsCount("last_tran_dt1")) Then
            .TextMatrix(MaxRow, 11) = Format(rsCount("last_tran_dt1"), "dd-MMM-yy")
            Else
            .TextMatrix(MaxRow, 11) = ""
            End If
            rsCount.Close
            
            .TextMatrix(MaxRow, 2) = msfgClients.TextMatrix(R, 0)
            .TextMatrix(MaxRow, 3) = msfgClients.TextMatrix(R, 2)
            .TextMatrix(MaxRow, 4) = msfgClients.TextMatrix(R, 3)
            .TextMatrix(MaxRow, 5) = msfgClients.TextMatrix(R, 4)
            .TextMatrix(MaxRow, 6) = msfgClients.TextMatrix(R, 5)
            .TextMatrix(MaxRow, 7) = msfgClients.TextMatrix(R, 6)
            rsCount.open "Select pincode,creation_date from client_master where client_code=" & msfgClients.TextMatrix(R, 1), MyConn, adOpenForwardOnly
            If Not (IsNull(rsCount("pincode"))) Then .TextMatrix(MaxRow, 8) = rsCount("pincode")
            If Not (IsNull(rsCount("creation_date"))) Then .TextMatrix(MaxRow, 1) = Format(rsCount("creation_date"), "dd-MMM-yy")
            rsCount.Close
            .TextMatrix(MaxRow, 9) = msfgClients.TextMatrix(R, 1)
            .TextMatrix(MaxRow, 10) = msfgClients.TextMatrix(R, 7)
            
            
'''            .Rows = .Rows + 1
'''            MaxRow = .Rows - 1
'''            rsCount.open "select sum(cnt) cnt from(select count(*) cnt from transaction_st where source_code=" & msfgClients.TextMatrix(r, 1) _
'''                & " Union All " _
'''                & "select count(*) cnt from transaction_sttemp where source_code=" & msfgClients.TextMatrix(r, 1) & ")", myconn, adOpenForwardOnly
'''            .TextMatrix(MaxRow, 0) = rsCount("cnt")
'''            rsCount.Close
'''            .TextMatrix(MaxRow, 2) = msfgClients.TextMatrix(r, 0)
'''            .TextMatrix(MaxRow, 3) = msfgClients.TextMatrix(r, 2)
'''            .TextMatrix(MaxRow, 4) = msfgClients.TextMatrix(r, 3)
'''            .TextMatrix(MaxRow, 5) = msfgClients.TextMatrix(r, 4)
'''            .TextMatrix(MaxRow, 6) = msfgClients.TextMatrix(r, 5)
'''            .TextMatrix(MaxRow, 7) = msfgClients.TextMatrix(r, 6)
'''            rsCount.open "Select pincode,creation_date from client_master where client_code=" & msfgClients.TextMatrix(r, 1), myconn, adOpenForwardOnly
'''            If Not (IsNull(rsCount("pincode"))) Then .TextMatrix(MaxRow, 8) = rsCount("pincode")
'''            If Not (IsNull(rsCount("creation_date"))) Then .TextMatrix(MaxRow, 1) = Format(rsCount("creation_date"), "dd-MMM-yy")
'''            rsCount.Close
'''            .TextMatrix(MaxRow, 9) = msfgClients.TextMatrix(r, 1)
'''            .TextMatrix(MaxRow, 10) = msfgClients.TextMatrix(r, 7)
'''
        End With
        Me.MousePointer = vbNormal
        Set rsCount = Nothing
        DoEvents
        msfgClients.SetFocus
        DoEvents
        'Unload Me
        Exit Sub
    End If
    
    If currentForm.Name = "frmIPO" Then
        frmIPO.txtINV_CD.Text = msfgClients.TextMatrix(R, 1)
        frmIPO.txtname.Text = msfgClients.TextMatrix(R, 0)
        frmIPO.txtDPID.Text = ""
        frmIPO.TxtDPname.Text = ""
        frmIPO.txtclient.Text = ""
        
        If rs_get_type.State = 1 Then rs_get_type.Close
        rs_get_type.open "select t.investor_type,dp_id,dp_name,client_id,deposit_type from investor_master i,investortype t where i.investor_code=t.investor_code(+) and inv_code=" & msfgClients.TextMatrix(R, 1), MyConn, adOpenForwardOnly
        If Not (IsNull(rs_get_type(1))) Then frmIPO.txtDPID.Text = rs_get_type(1)
        If Not (IsNull(rs_get_type(2))) Then frmIPO.TxtDPname.Text = rs_get_type(2)
        If Not (IsNull(rs_get_type(3))) Then frmIPO.txtclient.Text = rs_get_type(3)
        If Not (IsNull(rs_get_type(0))) Then frmIPO.cmbinvestor.Text = rs_get_type(0)
        If Not (IsNull(rs_get_type(4))) Then
            If rs_get_type(4) = "N" Then
                frmIPO.optNDSL.Value = True
            Else
                frmIPO.optCDSL.Value = True
            End If
        Else
            frmIPO.optNDSL.Value = True
        End If
        
        rs_get_type.Close
        Unload Me
        Exit Sub
    End If
    '----------------------NPS_-----------------
    If currentForm.Name = "frmNPS" Then
        frmNPS.txtINV_CD.Text = msfgClients.TextMatrix(R, 1)
        frmNPS.txtname.Text = msfgClients.TextMatrix(R, 0)
        rs_get_type.Close
        Unload Me
        Exit Sub
    End If
    '------------------------------------------------------
     '----------------------jt ana rate master mayank-----------------
    
    If currentForm.Name = "frmJTANAratemaster" Then
            Dim Br_ANA As String
            Dim Br_Cat As String
            Br_ANA = SqlRet("select branch_code from investor_master where inv_code='" & msfgClients.TextMatrix(R, 1) & "' ")
            Br_Cat = SqlRet("select branch_tar_cat from branch_master where branch_code='" & Br_ANA & "'")
            If Br_Cat = "186" Then
                frmJTANAratemaster.cmbinvestor.Clear
                frmJTANAratemaster.cmbinvestor.AddItem Left(msfgClients.TextMatrix(R, 0), 60) & Space(60 - Len(Left(msfgClients.TextMatrix(R, 0), 60))) & "#" & msfgClients.TextMatrix(R, 1)
                frmJTANAratemaster.cmbinvestor.Text = frmJTANAratemaster.cmbinvestor.List(0)
                
                
                
                EXISTCODE = ""
                Set rsexistcode = MyConn.Execute("select exist_code,agent_name,agent_code from agent_master where agent_code=substr('" & msfgClients.TextMatrix(R, 1) & "',1,8)")
                If Not IsNull(rsexistcode("exist_code")) Then
                    EXISTCODE = rsexistcode("exist_code")
                Else
                    EXISTCODE = "Not Exist"
                End If
                If rsexistcode.EOF = False Then
                    frmJTANAratemaster.cmbsuperana.Clear
                    frmJTANAratemaster.cmbsuperana.AddItem Left(rsexistcode(1), 60) & " (" & rsexistcode(0) & ")" & Space(60 - Len(Left(rsexistcode(1), 60))) & "#" & rsexistcode(2)
                    frmJTANAratemaster.cmbsuperana.Text = frmJTANAratemaster.cmbsuperana.List(0)
                End If
            Else
                MsgBox "Plz. Select Investor of ANG Channel", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
            Me.MousePointer = vbNormal
            Unload Me
          Exit Sub
    End If
   
    '------------------------------------------------------
    
    If currentForm.Name = "frmFP" Then
    Dim rsFP As New ADODB.Recordset
    Dim rs_inv As New ADODB.Recordset
    Dim strsql1 As String
    Dim strsql2 As String
    frmFP.txtFdate.Text = "__/__/____"
    frmFP.txtTdate.Text = "__/__/____"
     frmFP.CmbStatus.Text = ""
        frmFP.txtINV_CD.Text = msfgClients.TextMatrix(R, 1)
        frmFP.txtname.Text = msfgClients.TextMatrix(R, 0)
        
strsql1 = ""
Dim Fam_Head As String
If Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
    strsql1 = "select family_head from investor_master where source_id='" & Left(Trim(msfgClients.TextMatrix(R, 1)), 8) & "' and fpf_date is not null"
    If rs_inv.State = 1 Then rs_inv.Close
    rs_inv.open strsql1, MyConn, adOpenForwardOnly
    If rs_inv.EOF = False Then
        If msfgClients.TextMatrix(R, 1) <> rs_inv("family_head") Then
            Fam_Head = rs_inv("family_head")
            rs_inv.Close
            rs_inv.open "select Investor_name from investor_master where inv_code=" & Fam_Head, MyConn, adOpenForwardOnly
            MsgBox "Financial Planning Already done for the Family Head " & vbCrLf & "Code : " & Fam_Head & " Name : " & rs_inv("investor_name"), vbInformation
            rs_inv.Close
            Set rs_inv = Nothing
            Me.MousePointer = 0
            Exit Sub
        End If
    End If
End If

strsql1 = ""
strsql1 = "select investor_name,inv_code from investor_master where source_id='" & Left(Trim(msfgClients.TextMatrix(R, 1)), 8) & "' and inv_code <>'" & (msfgClients.TextMatrix(R, 1)) & "'"
If rs_inv.State = 1 Then rs_inv.Close
rs_inv.open strsql1, MyConn, adOpenStatic, adLockOptimistic, adCmdText
If rs_inv.RecordCount > 0 Then
    frmFP.Lstinv.Clear
With frmFP.Lstinv
For i = 0 To rs_inv.RecordCount - 1
 .AddItem rs_inv(0) & Space(100) & "#" & rs_inv(1)
'''''                    lstscheme.AddItem rs_scheme(0) & Space(80 - Len(rs_scheme(0))) & rs_scheme(1) & Space(10 - Len(rs_scheme(1))) & rs_scheme(2)
 rs_inv.MoveNext
Next
End With
Else
    frmFP.Lstinv.Clear
End If
    rs_inv.Close
    Set rs_inv = Nothing
''End If
        
    
        If rsFP.State = 1 Then rsFP.Close
        rsFP.open "select FPF_date,fpl_date,status,FAMILY_HEAD,implemented_date,audit_date from investor_master where inv_code=" & msfgClients.TextMatrix(R, 1), MyConn, adOpenForwardOnly
        If rsFP.EOF = False Then
            If Not (IsNull(rsFP("fpf_date"))) Then
                frmFP.txtFdate.Text = Format(rsFP("fpf_date"), "dd/mm/yyyy")
            End If
            If Not (IsNull(rsFP("fpl_date"))) Then
                frmFP.txtTdate.Text = Format(rsFP("fpl_date"), "dd/mm/yyyy")
            End If
            If Not (IsNull(rsFP("status"))) Then
                frmFP.CmbStatus.SelText = rsFP("status")
            End If
            If Not (IsNull(rsFP("audit_date"))) Then
                frmFP.MSK_AUDIT_DATE.Text = Format(rsFP("AUDIT_DATE"), "dd/mm/yyyy")
                frmFP.chkaudit.Value = 1
            Else
                frmFP.MSK_AUDIT_DATE.Text = "__/__/____"
                frmFP.chkaudit.Value = 0
            End If
            
            If Not (IsNull(rsFP("implemented_date"))) Then
                frmFP.chkaudit.Enabled = True
                frmFP.MSK_AUDIT_DATE.Enabled = True
            Else
                frmFP.chkaudit.Enabled = False
                frmFP.MSK_AUDIT_DATE.Enabled = False
            End If
            
        End If
        
        If IsNull(rsFP("status")) Then
            frmFP.CmbStatus.Text = "Snapshot Finalized"
        Else
            If role_name = "FINANCIAL PLANNING" Or GlbDataFilter = "72" Or role_name = "FPM (AUDIT)" Then
                frmFP.CmbStatus.Enabled = True
                If frmFP.chkaudit.Enabled = True Then
                    frmFP.chkaudit.Enabled = True
                    frmFP.MSK_AUDIT_DATE.Enabled = True
                End If
            Else
                frmFP.CmbStatus.Enabled = False
                frmFP.chkaudit.Enabled = False
                frmFP.MSK_AUDIT_DATE.Enabled = False
            End If
        End If
        
        
        
    strsql2 = ""
    strsql2 = "select inv_code from investor_master where FAMILY_HEAD='" & Trim(msfgClients.TextMatrix(R, 1)) & "'"
    If rs_inv.State = 1 Then rs_inv.Close
    rs_inv.open strsql2, MyConn, adOpenStatic, adLockOptimistic, adCmdText
    If rs_inv.EOF = False Then
    If rs_inv.RecordCount > 0 Then
    For j = 0 To rs_inv.RecordCount - 1
        For i = 0 To frmFP.Lstinv.ListCount - 1
        If IsNull(rsFP("FPF_DATE")) Then
            frmFP.Lstinv.Selected(i) = True
        Else
        
            If Trim(Right(frmFP.Lstinv.List(i), 11)) = Trim(rs_inv(0)) Then
                frmFP.Lstinv.Selected(i) = True
            End If
        End If
        Next
        rs_inv.MoveNext
    Next
    End If
    Else
    For i = 0 To frmFP.Lstinv.ListCount - 1
        If IsNull(rsFP("FPF_DATE")) Then
            frmFP.Lstinv.Selected(i) = True
        End If
    Next
    End If
    
    
        
        rsFP.Close
        Set rsFP = Nothing
        If frmFP.txtFdate.Text <> "__/__/____" And IsDate(frmFP.txtFdate.Text) Then
            frmFP.txtFdate.Enabled = False
        Else
            frmFP.txtFdate.Enabled = True
        End If
        Unload Me
        Exit Sub
    End If
    
    If currentForm.Name = "frmUpdateRecdSlabs" Then
        frmUpdateRecdSlabs.txtname.Text = msfgClients.TextMatrix(R, 0)
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        frmUpdateRecdSlabs.txtCD.Text = rsexistcode(0)
        frmUpdateRecdSlabs.txtAgCode.Text = msfgClients.TextMatrix(R, 1)
        frmUpdateRecdSlabs.cmbMutFund1.Enabled = True
        frmUpdateRecdSlabs.lstlongname1.Enabled = True
        frmUpdateRecdSlabs.lstSch1.Enabled = True
        Unload Me
        Exit Sub
    End If
    
    If currentForm.Name = "frmPayment" Then
        For i = 0 To frmPayment.Listclagname.ListCount - 1
        Dim clagname As Variant
        clagname = Split(frmPayment.Listclagname.List(i), "#")
            If clagname(1) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Client/Agent has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
        Next
            frmPayment.Listclagname.AddItem msfgClients.TextMatrix(R, 0) & Space(80) & "#" & msfgClients.TextMatrix(R, 1)
          For i = 0 To frmPayment.Listclagname.ListCount - 1
            frmPayment.Listclagname.Selected(i) = True
          Next
        frmPayment.Listclagname.SetFocus
        Me.MousePointer = vbNormal
        Me.Show
        Exit Sub
    End If
    
    If currentForm.Name = "frmjv" Then
    Dim clname As Variant
    If Cmbcat.Text = "CLIENT" Then
        For i = 0 To frmjv.clientlist.ListCount - 1
        clname = Split(frmjv.clientlist.List(i), "#")
            If clname(1) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Client/Agent has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
        Next
            frmjv.clientlist.AddItem msfgClients.TextMatrix(R, 0) & Space(80) & "#" & msfgClients.TextMatrix(R, 1)
          For i = o To frmjv.clientlist.ListCount - 1
            frmjv.clientlist.Selected(i) = True
          Next
        frmjv.clientlist.SetFocus
      Else
        For i = 0 To frmjv.agentlist.ListCount - 1
        clname = Split(frmjv.agentlist.List(i), "#")
            If clname(1) = msfgClients.TextMatrix(R, 1) Then
                MsgBox "This Client/Agent has already been Added ", vbInformation
                Me.MousePointer = vbNormal
                Exit Sub
            End If
        Next
            frmjv.clientlist.AddItem msfgClients.TextMatrix(R, 0) & Space(80) & "#" & msfgClients.TextMatrix(R, 1)
          For i = 0 To frmjv.clientlist.ListCount - 1
            frmjv.clientlist.Selected(i) = True
          Next
        frmjv.clientlist.SetFocus
      End If
        Me.MousePointer = vbNormal
        Me.Show
        Exit Sub
    End If
    
    
    If currentForm.Name = "frmSynchronization" Then  ' VISHAL
      If UCase(Trim(Cmbcat.Text)) = "INVESTOR" Then
         nodeValue = msfgClients.TextMatrix(R, 1)
         For j = 1 To currentForm.Controls(treeName).Rows - 1
             If currentForm.Controls(treeName).TextMatrix(j, 1) = "" Then
                 If Trim(currentForm.Controls(treeName).TextMatrix(j, 2)) = "Revertal Transactions" Then
                   startPos = j + 1
                   Exit For
                End If
             End If
         Next
         For M = startPos To currentForm.Controls(treeName).Rows - 1
             If currentForm.Controls(treeName).TextMatrix(M, 1) = "" Then
                 ENDPOS = M
                 Exit For
             End If
         Next
         For l = startPos To ENDPOS
             If currentForm.Controls(treeName).TextMatrix(l, 1) <> "" And currentForm.Controls(treeName).TextMatrix(l, 10) = "Y" And Left(currentForm.Controls(treeName).TextMatrix(l, 12), 3) <> "INV" Then
                 currentForm.Controls(treeName).TextMatrix(l, 12) = "INV#" & CStr(nodeValue)
             End If
         Next
         '''''''''''''''''''''''''''''''
         For j = 1 To currentForm.Controls(treeName).Rows - 1
             If currentForm.Controls(treeName).TextMatrix(j, 1) = "" Then
                 If Trim(currentForm.Controls(treeName).TextMatrix(j, 2)) = "New Transactions" Then
                   startPos = j + 1
                   Exit For
                End If
             End If
         Next
         For M = startPos To currentForm.Controls(treeName).Rows - 1
             If currentForm.Controls(treeName).TextMatrix(M, 1) = "" Then
                 ENDPOS = M
                 Exit For
             End If
         Next
         For l = startPos To ENDPOS
             If currentForm.Controls(treeName).TextMatrix(l, 1) <> "" And currentForm.Controls(treeName).TextMatrix(l, 10) = "Y" And Left(currentForm.Controls(treeName).TextMatrix(l, 12), 3) <> "INV" Then
                 currentForm.Controls(treeName).TextMatrix(l, 12) = "INV#" & CStr(nodeValue)
             End If
         Next
       End If
      Me.MousePointer = vbNormal
      Unload Me
      Exit Sub
  End If
    '' end here
     '''ADDED BY PRAVESH ON 24 MAY FOR lIFE iNSURANCE fORM
        If currentForm.Name = "frmAR" And Ar_type = "" Then 'jogi010907
            If MyOpt <> 1 Then
                'frmAR.FrmConfirmBoth.Visible = True
                frmAR.FrmConfirmBoth.Move frmAR.txtInsured.Left, frmAR.txtInsured.Top - 200
                frmAR.txtInsured1.Text = ""
                frmAR.txtProposer1.Text = ""
            End If
            frmAR.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            frmAR.txtInsured.Text = msfgClients.TextMatrix(R, 0)
            frmAR.txtProposer.Text = msfgClients.TextMatrix(R, 0)
            frmAR.txtadd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR.txtadd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR.txtIadd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR.txtIadd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR.txtPhone.Text = msfgClients.TextMatrix(R, 6)
            frmAR.txtPhone.Text = msfgClients.TextMatrix(R, 11)
            frmAR.fillData
            frmAR.txtClientCD.Enabled = False
            If Left(msfgClients.TextMatrix(R, 1), 1) <> "3" Then
                frmAR.TguestCode.Text = msfgClients.TextMatrix(R, 8)
            End If
            '-----Mayank--------------------
            If Mid(frmAR.txtClientCD, 1, 1) = "3" Then
                If RsPan.State = 1 Then RsPan.Close
                RsPan.open "select * from agent_master where agent_code=" & Mid(frmAR.txtClientCD, 1, 8) & "", MyConn
                Panc = IIf(IsNull(RsPan.Fields("pan")), "", RsPan.Fields("pan"))
                MyAgentCode1 = Mid(frmAR.txtClientCD, 1, 8)
                If ValidatePan(Panc) = False Then
                     MsgBox "Please enter a valid PAN Card Number and Upload Scan image Of PAN Card of ANA", vbCritical, "Pan Entry"
                     frmpan_request.Show vbModal
                     'Exit Sub
                 End If
                 RsPan.Close
            End If
            Me.MousePointer = vbNormal
            Unload Me
        End If
        If currentForm.Name = "frmAR_Renewal" And Ar_type = "" Then 'jogi010907
            frmAR_Renewal.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            frmAR_Renewal.Lbl_P_Code.Caption = msfgClients.TextMatrix(R, 1)
            
            frmAR_Renewal.txtInsured.Text = msfgClients.TextMatrix(R, 0)
            frmAR_Renewal.txtProposer.Text = msfgClients.TextMatrix(R, 0)
            frmAR_Renewal.txtadd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR_Renewal.txtadd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR_Renewal.txtIadd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR_Renewal.txtIadd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR_Renewal.txtPhone.Text = msfgClients.TextMatrix(R, 6)
            frmAR_Renewal.fillData
            frmAR_Renewal.txtClientCD.Enabled = False
            Me.MousePointer = vbNormal
            Unload Me
        End If
        
        If currentForm.Name = "frmactopen" Then
            frmactopen.txtclientcodeold.Text = msfgClients.TextMatrix(R, 1)
            frmactopen.txtclientname.Text = msfgClients.TextMatrix(R, 0)
            frmactopen.txtaddress1.Text = msfgClients.TextMatrix(R, 2)
            frmactopen.txtaddress2.Text = msfgClients.TextMatrix(R, 3)
            frmactopen.TxtTel1.Text = msfgClients.TextMatrix(R, 6)
            If RsPan.State = 1 Then RsPan.Close
            RsPan.open "select * from investor_master where inv_code=" & msfgClients.TextMatrix(R, 1) & "", MyConn, adOpenForwardOnly
            If Not RsPan.EOF Then
                  If IsNull(RsPan.Fields("dob")) Then
                  Else
                    frmactopen.dob = RsPan.Fields("dob")
                  End If
                  frmactopen.txtclientpan = IIf(IsNull(RsPan.Fields("pan")), "", RsPan.Fields("pan"))
                  frmactopen.TxtFax = IIf(IsNull(RsPan.Fields("fax")), "", RsPan.Fields("fax"))
                  frmactopen.TxtEmail = IIf(IsNull(RsPan.Fields("email")), "", RsPan.Fields("email"))
                  frmactopen.TxtMobile = IIf(IsNull(RsPan.Fields("mobile")), "", RsPan.Fields("mobile"))
                  frmactopen.cmbgender = IIf(IsNull(RsPan.Fields("gender")), " ", RsPan.Fields("gender"))
                  frmactopen.txtpin = IIf(IsNull(RsPan.Fields("pincode")), "", RsPan.Fields("pincode"))
                  If frmactopen.cmbcity.ListCount > 0 Then
                        For i = 0 To frmactopen.cmbcity.ListCount - 1
                              City = Split(frmactopen.cmbcity.List(i), "#")
                              If UCase(Trim(City(1))) = UCase(Trim(RsPan!City_id)) Then
                                frmactopen.cmbcity.ListIndex = i
                                Exit For
                              End If
                        Next
                  End If
                  If frmactopen.cmboccupation.ListCount > 0 And Not IsNull(UCase(Trim(RsPan!OCCUPATION_ID))) Then
                     For i = 0 To frmactopen.cmboccupation.ListCount - 1
                        If frmactopen.cmboccupation.List(i) <> "" Then
                            MyOcc = Split(frmactopen.cmboccupation.List(i), "#")
                            If UCase(Trim(MyOcc(1))) = UCase(Trim(RsPan!OCCUPATION_ID)) Then
                                frmactopen.cmboccupation.ListIndex = i
                                Exit For
                            End If
                        End If
                     Next
                  End If
            End If
            RsPan.Close
            frmactopen.txtclientcodeold.Enabled = False
            frmactopen.cmdSearch_Click
            Me.MousePointer = vbNormal
            Unload Me
        End If
        
        If currentForm.Name = "frmAR" And Ar_type = "P_NAME" Then 'jogi010907
        frmAR.Lbl_P_Code.Caption = msfgClients.TextMatrix(R, 1)
        frmAR.txtProposer.Text = msfgClients.TextMatrix(R, 0)
        
        frmAR.txtadd1.Text = msfgClients.TextMatrix(R, 2)
        frmAR.txtadd2.Text = msfgClients.TextMatrix(R, 3)
        
        frmAR.txtPhone.Text = msfgClients.TextMatrix(R, 6)
        frmAR.ImPDOB.Text = msfgClients.TextMatrix(R, 11)
        frmAR.ImNDOB.Text = msfgClients.TextMatrix(R, 11)
        
        For i = 0 To frmAR.cboNOccupation.ListCount - 1
            If Val(Mid(frmAR.cboNOccupation.List(i), 35)) = Val("" & msfgClients.TextMatrix(R, 12)) Then
                frmAR.cboNOccupation.ListIndex = i
                frmAR.cboPOccupation.ListIndex = i
                Exit For
            End If
        Next
        
        If Left(msfgClients.TextMatrix(R, 1), 1) <> "3" Then
            frmAR.TguestCode.Text = msfgClients.TextMatrix(R, 8)
        End If
        'frmAR.fillData
        Me.MousePointer = vbNormal
        Unload Me
    
    End If
    If currentForm.Name = "frmAR_Renewal" And Ar_type = "P_NAME" Then 'jogi010907
        frmAR_Renewal.Lbl_P_Code.Caption = msfgClients.TextMatrix(R, 1)
        frmAR_Renewal.txtProposer.Text = msfgClients.TextMatrix(R, 0)
        
        frmAR_Renewal.txtadd1.Text = msfgClients.TextMatrix(R, 2)
        frmAR_Renewal.txtadd2.Text = msfgClients.TextMatrix(R, 3)
        
        frmAR_Renewal.txtPhone.Text = msfgClients.TextMatrix(R, 6)
        'frmAR.fillData
        Me.MousePointer = vbNormal
        Unload Me
    
    End If
    If currentForm.Name = "frmAR" And Ar_type = "N_NAME" Then  'jogi010907
        frmAR.Lbl_N_Code.Caption = msfgClients.TextMatrix(R, 1)
        frmAR.txtNominee.Text = msfgClients.TextMatrix(R, 0)
        If msfgClients.TextMatrix(R, 11) <> "" Then frmAR.ImNDOB.Text = msfgClients.TextMatrix(R, 11)
        'frmAR.fillData
        Me.MousePointer = vbNormal
        Unload Me
    
    End If
    
    If currentForm.Name = "frmAR_Renewal" And Ar_type = "N_NAME" Then 'jogi010907
        frmAR_Renewal.Lbl_N_Code.Caption = msfgClients.TextMatrix(R, 1)
        frmAR_Renewal.txtNominee.Text = msfgClients.TextMatrix(R, 0)
        
        'frmAR.fillData
        Me.MousePointer = vbNormal
        Unload Me
    
    End If
    
         
     If currentForm.Name = "frmARGeneral" Then
            frmARGeneral.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            frmARGeneral.txtClientCD.Enabled = False
            frmARGeneral.txtadd1.Text = msfgClients.TextMatrix(R, 2)
            frmARGeneral.txtProposer.Text = msfgClients.TextMatrix(R, 0)
            frmARGeneral.txtadd2.Text = msfgClients.TextMatrix(R, 3)
            frmARGeneral.txtIadd1.Text = msfgClients.TextMatrix(R, 2)
            frmARGeneral.txtIadd2.Text = msfgClients.TextMatrix(R, 3)
            frmARGeneral.txtPhone.Text = msfgClients.TextMatrix(R, 6)
            frmARGeneral.fillData
            '-----Mayank--------------------
            If Mid(frmARGeneral.txtClientCD, 1, 1) = "3" Then
                RsPan.open "select * from agent_master where agent_code=" & Mid(frmARGeneral.txtClientCD, 1, 8) & "", MyConn
                Panc = IIf(IsNull(RsPan.Fields("pan")), "", RsPan.Fields("pan"))
                MyAgentCode1 = Mid(frmARGeneral.txtClientCD, 1, 8)
                If ValidatePan(Panc) = False Then
                    MsgBox "Please enter a valid PAN Card Number and attach a Scan Copy Of PAN Card of ANA", vbCritical, "Pan Entry"
                    frmpan_request.Show vbModal
                    'Exit Sub
                End If
                RsPan.Close
            End If
            '---------------------------------
            Me.MousePointer = vbNormal
        Unload Me
     '' Exit Sub
    End If
    
    
    
    
    
       If currentForm.Name = "frm_MF_AUM_rpt" Then
       '' Dim EXISTCODE As String
         EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frm_MF_AUM_rpt.CmbClientSBroker.Clear
            frm_MF_AUM_rpt.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frm_MF_AUM_rpt.CmbClientSBroker.Text = frm_MF_AUM_rpt.CmbClientSBroker.List(0)
        End If
            Me.MousePointer = vbNormal
        Unload Me
          Exit Sub
    End If
    
    
    ''by Jawahar for transaction report on 03 Feb 06
    If currentForm.Name = "frmtstatementrpt" Then
      ''  Dim EXISTCODE As String
         EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmtstatementrpt.CmbClientSBroker.Clear
            frmtstatementrpt.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmtstatementrpt.CmbClientSBroker.Text = frmtstatementrpt.CmbClientSBroker.List(0)
        End If
            Me.MousePointer = vbNormal
        Unload Me
          Exit Sub
    End If
    If currentForm.Name = "frmtstatementrptop" Then
    EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmtstatementrptop.CmbClientSBroker.Clear
            frmtstatementrptop.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmtstatementrptop.CmbClientSBroker.Text = frmtstatementrptop.CmbClientSBroker.List(0)
        End If

            Me.MousePointer = vbNormal
        Unload Me
      Exit Sub
    End If
    
    
    
    ''FRM_FP_STATEMENT
    
    If currentForm.Name = "FRM_FP_STATEMENT" Then
    EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            FRM_FP_STATEMENT.CmbClientSBroker.Clear
            FRM_FP_STATEMENT.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            FRM_FP_STATEMENT.CmbClientSBroker.Text = FRM_FP_STATEMENT.CmbClientSBroker.List(0)
        End If

            Me.MousePointer = vbNormal
        Unload Me
      Exit Sub
    End If
    
    
    
    
    
    
    
    
    
     '''FRMTSTATEOP_STAX
    
    If currentForm.Name = "frmtstateop_stax" Then
    EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmtstateop_stax.CmbClientSBroker.Clear
            frmtstateop_stax.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmtstateop_stax.CmbClientSBroker.Text = frmtstatementrptop.CmbClientSBroker.List(0)
        End If

            Me.MousePointer = vbNormal
        Unload Me
      Exit Sub
    End If
    
    '''frmtstatemf_withstax
    
    If currentForm.Name = "frmtstatemf_withstax" Then
    EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmtstatemf_withstax.CmbClientSBroker.Clear
            frmtstatemf_withstax.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmtstatemf_withstax.CmbClientSBroker.Text = frmtstatementrptop.CmbClientSBroker.List(0)
        End If

            Me.MousePointer = vbNormal
        Unload Me
      Exit Sub
    End If
    
    If currentForm.Name = "frmbussummery" Then
          EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmbussummery.CmbClientSBroker.Clear
            frmbussummery.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmbussummery.CmbClientSBroker.Text = frmbussummery.CmbClientSBroker.List(0)
        End If
            Me.MousePointer = vbNormal
        Unload Me
          Exit Sub
    End If
    
    If currentForm.Name = "frmBrokerBill_Statement" Then
          EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        ElseIf Left(msfgClients.TextMatrix(R, 1), 1) = "4" Then
            Set rsexistcode = MyConn.Execute("select exist_code from client_master where client_code='" & msfgClients.TextMatrix(R, 1) & "'")
        End If
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmBrokerBill_Statement.CmbClientSBroker.Clear
            frmBrokerBill_Statement.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmBrokerBill_Statement.CmbClientSBroker.Text = frmBrokerBill_Statement.CmbClientSBroker.List(0)
        End If
            Me.MousePointer = vbNormal
            Unload Me
          Exit Sub
    End If
    
    If currentForm.Name = "frmSubrokerSIPBillReport" Then
        EXISTCODE = ""
        Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmSubrokerSIPBillReport.CmbClientSBroker.Clear
            frmSubrokerSIPBillReport.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmSubrokerSIPBillReport.CmbClientSBroker.Text = frmSubrokerSIPBillReport.CmbClientSBroker.List(0)
        End If
            Me.MousePointer = vbNormal
            Unload Me
          Exit Sub
    End If
    
    If currentForm.Name = "frmANASummaryReport" Then
        EXISTCODE = ""
        Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        If Not IsNull(rsexistcode("exist_code")) Then
            EXISTCODE = rsexistcode("exist_code")
        Else
            EXISTCODE = "Not Exist"
        End If
        If rsexistcode.EOF = False Then
            frmANASummaryReport.CmbClientSBroker.Clear
            frmANASummaryReport.CmbClientSBroker.AddItem msfgClients.TextMatrix(R, 0) & " Exist Code- " & EXISTCODE & Space(50) & Trim(msfgClients.TextMatrix(R, 1))
            frmANASummaryReport.CmbClientSBroker.Text = frmANASummaryReport.CmbClientSBroker.List(0)
        End If
            Me.MousePointer = vbNormal
            Unload Me
          Exit Sub
    End If
    
    If ReportFlag = True Then
    If currentForm.Caption = f(0).Caption Then
            For i = 0 To f(0).cmbAgent.ListCount - 1
             If Mid(f(0).cmbAgent.List(i), 51) = msfgClients.TextMatrix(R, 1) Then
                f(0).cmbAgent.ListIndex = i
                Exit For
             End If
            Next
            Me.MousePointer = vbNormal
            Unload f(1)
            Unload f(2)
            ReportFlag = False
        Unload Me
      Exit Sub
    End If

    If currentForm.Caption = f(1).Caption Then
            For i = 0 To f(1).cmbAgent.ListCount - 1
             If Mid(f(1).cmbAgent.List(i), 51) = msfgClients.TextMatrix(R, 1) Then
                f(1).cmbAgent.ListIndex = i
                Exit For
             End If
            Next
            Me.MousePointer = vbNormal
            Unload f(0)
            Unload f(2)
            ReportFlag = False
        Unload Me
      Exit Sub
    End If

    If currentForm.Caption = f(2).Caption Then
           For i = 0 To f(2).cmbAgent.ListCount - 1
             If Mid(f(2).cmbAgent.List(i), 51) = msfgClients.TextMatrix(R, 1) Then
                f(2).cmbAgent.ListIndex = i
                Exit For
             End If
            Next
            Me.MousePointer = vbNormal
            Unload f(0)
            Unload f(1)
            ReportFlag = False
        Unload Me
      Exit Sub
    End If

  End If
    
    
    
'''''''''''''''''''end here
If currentForm.Name <> "frmAR" And currentForm.Name <> "frmactopen" And currentForm.Name <> "frmARGeneral" And currentForm.Name <> "FrmVenDayBook" And currentForm.Name <> "frmAR_Renewal" Then 'jogi010907
If chkIndia.Value = 0 Then
    AllIndia = False
    Dim tr As TreeView
    Set tr = currentForm.Controls(treeName)
    preSelectedCode = "MFIBR"
    Dim Fcode As String
    Dim Pcode As Long
    nodeValue = msfgClients.TextMatrix(R, 1)
    If currentForm.Name = "FrmtransactionNew" Then FrmtransactionNew.cmbBusiBranch.Clear
    Dim Get_rm As New ADODB.Recordset
    If UCase(Trim(Cmbcat.Text)) = "CLIENT" Then
        Set Get_rm = MyConn.Execute("select rm_code from client_master where client_code=" & nodeValue)
        If Not Get_rm.EOF Then
            Pcode = Get_rm(0)
        End If
        treeClientFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Key, msfgClients.TextMatrix(R, 1) 'BY HIMANSHU LATEST
        currentForm.Controls(treeName).Nodes("C" & CStr(nodeValue)).Selected = True
        currentForm.Controls(treeName).Nodes("C" & CStr(nodeValue)).Bold = True
        currentForm.Controls(treeName).Nodes("C" & CStr(nodeValue)).Expanded = True
        If currentForm.Name = "FrmtransactionNew" Then
            FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
            FrmtransactionNew.userType = "CLIENT"
        End If
    End If
    If UCase(Trim(Cmbcat.Text)) = "AGENT" Then
        Set Get_rm = MyConn.Execute("select rm_code from agent_master where agent_code=" & nodeValue)
        If Not Get_rm.EOF Then
            Pcode = Get_rm(0)
        End If
        treeAgentFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & CStr(Pcode)).Child.Next.Key, Left(msfgClients.TextMatrix(R, 1), 8) 'BY HIMANSHU LATEST  06-SEP
        currentForm.Controls(treeName).Nodes("A" & CStr(nodeValue)).Selected = True
        currentForm.Controls(treeName).Nodes("A" & CStr(nodeValue)).Bold = True
        currentForm.Controls(treeName).Nodes("A" & CStr(nodeValue)).Expanded = True
        If currentForm.Name = "FrmtransactionNew" Then
            FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
            FrmtransactionNew.userType = "AGENT"
        End If
    End If
    If UCase(Trim(Cmbcat.Text)) = "INVESTOR" Then
        Set rs_get_invsrc = MyConn.Execute("Select source_id from investor_master where inv_code=" & nodeValue)
         If Left(rs_get_invsrc(0), 1) = 4 Then
            Set rs_get_rm = MyConn.Execute("select rm_code from client_master where client_code=" & rs_get_invsrc(0))
            If Not rs_get_rm.EOF Then
                Pcode = rs_get_rm(0)
                    treeClientFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Key, Left(msfgClients.TextMatrix(R, 1), 8) 'BY HIMANSHU LATEST
                    treeInvestorFill currentForm.Controls(treeName), "C" & rs_get_invsrc(0), currentForm.Controls(treeName).Nodes("C" & rs_get_invsrc(0)).Child.Key
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Selected = True
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Bold = True
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Expanded = True
                If currentForm.Name = "FrmtransactionNew" Then
                    FrmtransactionNew.userCode = FrmtransactionNew.treeClient.SelectedItem.Key
                    FrmtransactionNew.userType = "INVESTOR"
                End If
            End If
        End If
        If Left(rs_get_invsrc(0), 1) = 3 Then
            Set rs_get_rm = MyConn.Execute("select rm_code from agent_master where agent_code=" & rs_get_invsrc(0))
            If Not rs_get_rm.EOF Then
                Pcode = rs_get_rm(0)
                treeAgentFill currentForm.Controls(treeName), "R" & CStr(Pcode), currentForm.Controls(treeName).Nodes("R" & Pcode).Child.Next.Key, Left(msfgClients.TextMatrix(R, 1), 8)
                treeInvestorFill currentForm.Controls(treeName), "A" & rs_get_invsrc(0), currentForm.Controls(treeName).Nodes("A" & rs_get_invsrc(0)).Child.Key
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Selected = True
                currentForm.Controls(treeName).Nodes("I" & CStr(nodeValue)).Expanded = True
            End If
        End If
    End If
Else
    AllIndia = True
    AllIndia_Inv_code = msfgClients.TextMatrix(R, 1)
    FrmtransactionNew.userCode = "I" & msfgClients.TextMatrix(R, 1)
    FrmtransactionNew.userType = "INVESTOR"
End If
    If susPense = "" Then
    End If
    Unload Me
End If
    
If currentForm.Name = "frmUpdateClient" Then
    Debug.Print frmUpdateClient.treeClient.SelectedItem.Key, 1
    DoEvents
    frmUpdateClient.treeClient_Click
End If
If currentForm.Name = "frmupdatedinvestor" Then
    Debug.Print frmupdatedinvestor.treeClient.SelectedItem.Key, 1
    DoEvents
    frmupdatedinvestor.treeClient_Click
End If
If AllIndia = False Then
    If currentForm.Name = "FrmtransactionNew" Then
        FrmtransactionNew.treeClient_Click
        DoEvents
        If FrmtransactionNew.cmbBankName.Enabled = True Then
            FrmtransactionNew.cmbBankName.SetFocus
        End If
        DoEvents
    End If
End If
If currentForm.Name = "frmPaySlabs" Then
    frmPaySlabs.treeClient_Click
    DoEvents
End If
  
End If
Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
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
    MSRClient.Sql = ""
    
    MSRClient.Sql = strQuery
    MSRClient.Refresh
    
    Exit Sub
err1:
    MsgBox err.Description
   ' Resume
End Sub
Private Sub set_grid()
On Error GoTo err1
    msfgClients.Row = 0
    msfgClients.ColWidth(0) = "2700"
    msfgClients.Text = "Name"
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
    msfgClients.Text = "Branch"
    msfgClients.CellFontBold = True
    msfgClients.Col = 6
    msfgClients.ColWidth(6) = "1000"
    msfgClients.Text = "Phone"
    msfgClients.CellFontBold = True
    msfgClients.Col = 7
    msfgClients.ColWidth(7) = "2000"
    msfgClients.Text = "RM"
    msfgClients.CellFontBold = True
    msfgClients.Col = 8
    msfgClients.ColWidth(8) = "2000"
    msfgClients.Text = "Guest Code"
    msfgClients.Col = 9
    msfgClients.ColWidth(9) = "1500"
    msfgClients.Text = "Accont Status"
    msfgClients.CellFontBold = True
    msfgClients.Col = 10
    msfgClients.ColWidth(10) = "1500"
    msfgClients.Text = "Account Holder"
    msfgClients.CellFontBold = True
     msfgClients.Col = 11
    msfgClients.ColWidth(10) = "1500"
    msfgClients.Text = "DOB"
    msfgClients.CellFontBold = True
     msfgClients.Col = 12
    msfgClients.ColWidth(10) = "1500"
    msfgClients.Text = "Occupation"
    msfgClients.CellFontBold = True
    msfgClients.SetFocus
    If msfgClients.Rows > 1 Then
        msfgClients.Row = 1
    End If
    msfgClients.Col = 0
    Exit Sub
err1:
    'MsgBox err.Description
End Sub
Private Sub txtPhone_KeyDown(KeyCode As Integer, Shift As Integer)
    If KeyCode = 13 Then
        'Call cmdshow_Click
    End If
End Sub

Private Sub AllIndiaSearch()
Dim Sql As String
        Sql = ""
        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
            Sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.guest_cd,a.kyc,T.CLIENT_CODE FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,CLIENT_TEST T,client_master cm  where  cm.client_code=a.source_id and T.CLIENT_CODEKYC=A.INV_CODE AND a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
            If TxtPanNo.Text <> "" Then
                Sql = Sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            End If
            If txtAccountCode.Text <> "" Then
                Sql = Sql & " and upper(t.CLIENT_CODE) like '%" & UCase(Trim(txtAccountCode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            End If
            If txtname.Text <> "" Then
                 Sql = Sql & " and upper(a.investor_name) like '" & UCase(Trim(txtname.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            End If
            If ImEntryDt.Text <> "__/__/____" Then
                 Sql = Sql & " and a.dob=to_date('" & ImEntryDt.Text & "','dd-mm-yyyy')"
            End If
            Sql = Sql & " ORDER BY upper(a.investor_name)"
        End If
        Populate_Data Sql
End Sub
'------------------------------------------------------------------------------------------------
'Private Sub AllIndiaSearch()
'Dim sql As String
'        sql = ""
'        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
'            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,T.CLIENT_CODE FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,CLIENT_TEST T where T.CLIENT_CODEKYC=A.INV_CODE AND a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL"
'            If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtPhone.Text) = "" Then
'            Else
'                If Txtcode.Text <> "" Then
'                    sql = sql & " and inv_code like '%" & Trim(Txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If Txtname.Text <> "" Then
'                    sql = sql & " and upper(a.investor_name) like '" & UCase(Trim(Txtname.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtAdd1.Text <> "" Then
'                    sql = sql & " and upper(a.address1) like '%" & UCase(Trim(txtAdd1.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtAdd2.Text <> "" Then
'                    sql = sql & " and upper(a.address2) like '%" & UCase(Trim(txtAdd2.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtPhone.Text <> "" Then
'                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtmobile.Text <> "" Then
'                    sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtmobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If TxtPanNo.Text <> "" Then
'                    sql = sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'                If txtAccountCode.Text <> "" Then
'                    sql = sql & " and upper(a.CLIENT_CODE) like '%" & UCase(Trim(txtAccountCode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                End If
'            End If
'            sql = sql & " ORDER BY upper(a.investor_name)"
'        End If
'        Populate_Data sql
'End Sub
'------------------------------------------------------------------------------------------------
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
'''    MSRClient.DataSourceName = "test"
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
