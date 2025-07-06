Private Sub ShowFilterData()
'kyc
Dim sql As String
    sql = ""
    If strForm <> "Client Transfer" Then
        If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
            If currentForm.Name = "FrmtransactionNew" Or currentForm.Name = "frmAR" Or currentForm.Name = "frmARGeneral" Or currentForm.Name = "frmAR_Renewal" Then  'jogi010907
                If CmbClientBroker.Text = "CLIENT" Then
                    If SRmCode <> "" Then
                        temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & Val(Branches)
                        Set R = MyConn.Execute(temp_Sql)
                        If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
                            sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode ,to_char(cm.DOB,'dd/mm/rrrr') DOB,cm.OCC_ID FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c,client_test t where t.client_codekyc(+)=a.inv_code and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP')"
                            'sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP')"
                        Else
                            sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode ,to_char(cm.DOB,'dd/mm/rrrr') DOB,cm.OCC_ID FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c,client_test t where t.client_codekyc=a.inv_code and  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257" '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                            'sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257" '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                        End If
                    Else
                        sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode ,to_char(cm.DOB,'dd/mm/rrrr') DOB,cm.OCC_ID FROM client_test t,Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where t.client_codekyc(+)=a.inv_code and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                       ' sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " '  and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP') "
                    End If
                ElseIf CmbClientBroker.Text = "SUB BROKER" Then
                    If SRmCode <> "" Then
                        temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & Val(Branches)
                        Set R = MyConn.Execute(temp_Sql)
                        If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
                            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                        Else
                            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                        End If
                    Else
                        sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                    End If
                End If
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Else
                If CmbClientBroker.Text = "CLIENT" Then
                    If SRmCode = "" Then
                        sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code AcHolderCode  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm,client_test t where  t.client_codekyc(+)=a.inv_code and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and cm.city_id=C.city_id(+) AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP') "
                    Else
                        sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,a.phone,e.rm_name,cm.GUEST_CD,a.kyc,t.client_code  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm,client_test t where t.client_codekyc(+)=a.inv_Code and  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and  INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP') "
                    End If
                ElseIf CmbClientBroker.Text = "SUB BROKER" Then
                    If SRmCode = "" Then
                        sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c  where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                    Else
                        sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                    End If
                End If
            End If
            If CmbClientBroker.Text = "CLIENT" Then
                sql = sql & " and cm.client_code=a.source_id"
            End If
                
            If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by Pravesh Chandel on 18 nov 2005
                sql = sql & " and lpad(a.inv_code,1)=3"
                '------------------Block agent by mayank---------------------------------
                sql = sql & " and lpad(a.inv_code,8)not in (select agent_code from agent_master where block_agent='1')"
                '--------------------------------------------------------------
            ElseIf CmbClientBroker.Text = "CLIENT" Then
                sql = sql & " and lpad(a.inv_code,1)=4"
            End If
            If Trim(Txtcode.Text) = "" And Trim(txtName.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtMobile.Text) = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 And txtCliSubName.Text = "" Then
              sql = sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
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

                If Txtcode.Text <> "" Then
                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
                        sql = sql & " and source_id in (select agent_code from agent_master where (upper(exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%' OR TO_CHAR(agent_code)='" & Trim(Txtcode.Text) & "')) "
                    ElseIf CmbClientBroker.Text = "CLIENT" Then
                        sql = sql & " and source_id in (select client_code from client_master where ( TO_CHAR(client_code)='" & Trim(Txtcode.Text) & "')) " 'upper(exist_code) like '%" & UCase(Trim(txtCode.Text)) & "%' OR
                    End If
                End If
                
                If txtCliSubName.Text <> "" Then
                    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
                        sql = sql & " and source_id in (select agent_code from agent_master where upper(trim(agent_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
                    ElseIf CmbClientBroker.Text = "CLIENT" Then
                        sql = sql & " and source_id in (select client_code from client_master where upper(trim(client_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
                    End If
                End If
                
                If txtName.Text <> "" Then
                    sql = sql & " and upper(a.investor_name) like '%" & Replace(UCase(Trim(txtName.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If CmbClientBroker.Text = "SUB BROKER" Then
                    If txtAdd1.Text <> "" Then
                        sql = sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If txtAdd2.Text <> "" Then
                        sql = sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If cmbCity.ListIndex <> -1 Then
                        sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                ElseIf CmbClientBroker.Text = "CLIENT" Then
                    If txtAdd1.Text <> "" Then
                        sql = sql & " and upper(cm.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If txtAdd2.Text <> "" Then
                        sql = sql & " and upper(cm.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                    If cmbCity.ListIndex <> -1 Then
                        sql = sql & " and cm.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    End If
                End If
                If txtPhone.Text <> "" Then
                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtMobile.Text <> "" Then
                    sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    sql = sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtMobile.Text <> "" Then
                    sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbBranch.ListIndex <> -1 Then
                    sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If

            End If
            
            If cmbNewRM.ListIndex <> -1 Then
                sql = sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
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
                sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL "
            Else
                sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
            Else
                sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
            If Trim(Txtcode.Text) = "" And Trim(txtName.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtMobile.Text) = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 Then
                sql = sql & " and  B.BRANCH_CODE in(" & SQLBranches & ") "
            Else
'                If txtcode.Text <> "" Then
'                    'Sql = Sql & " and CLIENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
'                    Sql = Sql & " and upper(a.exist_code) like '%" & UCase(Trim(txtcode.Text)) & "%'"
'                End If
                
                If Txtcode.Text <> "" Then
                    If Cmbcat.Text = "CLIENT" Then
                        sql = sql & " and TO_CHAR(a.client_code)='" & Trim(Txtcode.Text) & "' "
                    Else
                        sql = sql & " and (upper(a.exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%') "
                    End If
                End If
                
                If txtName.Text <> "" Then
                    sql = sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(txtName.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd1.Text <> "" Then
                    sql = sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd2.Text <> "" Then
                    sql = sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtMobile.Text <> "" Then
                    sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    sql = sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbCity.ListIndex <> -1 Then
                    sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbBranch.ListIndex <> -1 Then
                    sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ") "
                End If
                If currentForm.Name = "frmInvestorMerge" Then
                    sql = sql & " and A.RM_CODE = " & txtRM.Text & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If currentForm.Name = "frmClientMerging" And txtRM.Text <> "" Then
                    sql = sql & " and A.RM_CODE = " & txtRM.Text & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                
            End If
            If cmbNewRM.ListIndex <> -1 Then
                sql = sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
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
                sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            Else
                sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
            If Trim(Txtcode.Text) = "" And Trim(txtName.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtMobile.Text) = "" And Trim(TxtPanNo.Text) = "" And Trim(txtAccountCode.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 Then
                sql = sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            Else
                If Txtcode.Text <> "" Then
                    'Sql = Sql & " and AGENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    sql = sql & " and (upper(a.exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%' or to_char(a.agent_code)='" & Trim(Txtcode.Text) & "') "
                End If
                If txtName.Text <> "" Then
                    sql = sql & " and upper(a.agent_NAME) like '%" & Replace(UCase(Trim(txtName.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd1.Text <> "" Then
                    sql = sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd2.Text <> "" Then
                    sql = sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtMobile.Text <> "" Then
                    sql = sql & " and upper(a.Mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    sql = sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbCity.ListIndex <> -1 Then
                    sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbBranch.ListIndex <> -1 Then
                    sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If currentForm.Name = "frmInvestorMerge" Then
                    sql = sql & " and A.RM_CODE = " & txtRM.Text & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                
            End If
             If cmbNewRM.ListIndex <> -1 Then
                sql = sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            sql = sql & " ORDER BY upper(a.agent_NAME)"
        End If
    
    Else
    
'***********************************
        If Trim(UCase(Cmbcat.Text)) = "CLIENT" Then
            'If SRmCode = "" Then
            
                sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
'            Else
'                Sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND CLIENT_name IS NOT NULL"
            'End If
            sql = sql & " and A.FROM_RM_CODE = " & cmbOldRM.List(cmbOldRM.ListIndex, 2) & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            
            'If Trim(txtcode.Text) = "" And Trim(txtName.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And cmbBranch.ListIndex = -1 And Cmbcity.ListIndex = -1 Then
            '    Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            'Else
                If Txtcode.Text <> "" Then
                    'Sql = Sql & " and CLIENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    sql = sql & " and upper(a.exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtName.Text <> "" Then
                    sql = sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(txtName.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd1.Text <> "" Then
                    sql = sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd2.Text <> "" Then
                    sql = sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtMobile.Text <> "" Then
                    sql = sql & " and upper(a.Mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    sql = sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbCity.ListIndex <> -1 Then
                    sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbBranch.ListIndex <> -1 Then
                    sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
            'End If
            'Sql = Sql & " ORDER BY upper(a.CLIENT_NAME)"
             If cmbNewRM.ListIndex <> -1 Then
                sql = sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            If cmbSort.Text = "Name" Then
                sql = sql & " ORDER BY upper(a.CLIENT_NAME)"
            ElseIf cmbSort.Text = "Address1" Then
                sql = sql & " ORDER BY upper(trim(a.Address1))"
            ElseIf cmbSort.Text = "Address2" Then
                sql = sql & " ORDER BY upper(trim(a.Address2))"
            ElseIf cmbSort.Text = "City" Then
                sql = sql & " ORDER BY upper(trim(C.CITY_NAME))"
            ElseIf cmbSort.Text = "Phone" Then
                sql = sql & " ORDER BY upper(trim(a.phone))"
            End If
            
        ElseIf Trim(UCase(Cmbcat.Text)) = "AGENT" Then
            'If SRmCode = "" Then
                
                sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            'Else
            '    Sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,a.phone,e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and A.FROM_SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND AGENT_name IS NOT NULL"
            'End If
            sql = sql & " and A.FROM_RM_CODE = " & cmbOldRM.List(cmbOldRM.ListIndex, 2) & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            
            'If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 Then
            '    Sql = Sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
            'Else
                
                If Txtcode.Text <> "" Then
                    'Sql = Sql & " and AGENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                    sql = sql & " and upper(a.exist_code) like '%" & UCase(Trim(Txtcode.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtName.Text <> "" Then
                    sql = sql & " and upper(a.agent_NAME) like '%" & Replace(UCase(Trim(txtName.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd1.Text <> "" Then
                    sql = sql & " and upper(a.address1) like '%" & Replace(UCase(Trim(txtAdd1.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtAdd2.Text <> "" Then
                    sql = sql & " and upper(a.address2) like '%" & Replace(UCase(Trim(txtAdd2.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtPhone.Text <> "" Then
                    sql = sql & " and upper(a.Phone) like '%" & UCase(Trim(txtPhone.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If txtMobile.Text <> "" Then
                    sql = sql & " and upper(a.mobile) like '%" & UCase(Trim(txtMobile.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If TxtPanNo.Text <> "" Then
                    sql = sql & " and upper(a.PAN) like '%" & UCase(Trim(TxtPanNo.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbCity.ListIndex <> -1 Then
                    sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
                If cmbBranch.ListIndex <> -1 Then
                    sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
                Else
                    sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
                End If
            'End If
             If cmbNewRM.ListIndex <> -1 Then
                sql = sql & " and e.rm_code= " & cmbNewRM.List(cmbNewRM.ListIndex, 2)
            End If
            sql = sql & " ORDER BY upper(a.agent_NAME)"
        End If
    
    
'***********************************
    End If
    'txtRM.Text = ""
    Populate_Data sql
End Sub