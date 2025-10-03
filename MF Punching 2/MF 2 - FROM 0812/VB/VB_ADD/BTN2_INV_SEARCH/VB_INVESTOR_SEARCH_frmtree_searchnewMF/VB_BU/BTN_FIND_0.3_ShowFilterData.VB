Private Sub ShowFilterData()
Dim sql As String
sql = ""
If strForm <> "Client Transfer" Then
If Trim(UCase(Cmbcat.Text)) = "INVESTOR" Then
    ''''''''''''''27/3/2007''''''''''''''''''''''   Vinay Hatwal''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''' Client Search - Branch Category wise'''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' if branch is not in IAG-187 and WMG=188 then a RM can see all the clients in the branch''''''''''''''''''''''''''''''
    If currentForm.Name = "frmtransactionmf" Then   'jogi010907
        If CmbClientBroker.Text = "CLIENT" Then
            If SRmCode <> "" Then
                temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & Val(Branches)
                Set R = MyConn.Execute(temp_Sql)
                If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
                    sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,cm.GUEST_CD,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and  upper(a.kyc) IN ('YES', 'YESP')
                Else
                    sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,cm.GUEST_CD,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP')
                End If
            Else
                sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,cm.GUEST_CD,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP')
            End If
        ElseIf CmbClientBroker.Text = "SUB BROKER" Then
            If SRmCode <> "" Then
                temp_Sql = "select branch_tar_cat from branch_master where branch_code= " & Val(Branches)
                Set R = MyConn.Execute(temp_Sql)
                If R.Fields(0) = 187 Or R.Fields(0) = 188 Then
                    sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                Else
                    sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
                End If
            Else
                sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
        End If
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Else
        If CmbClientBroker.Text = "CLIENT" Then
            If SRmCode = "" Then
                sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,cm.GUEST_CD,a.pan  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and cm.city_id=C.city_id(+) AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP')
            Else
                sql = "Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,cm.GUEST_CD,a.pan  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND cm.city_id=C.city_id(+) and  INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP')
            End If
        ElseIf CmbClientBroker.Text = "SUB BROKER" Then
            If SRmCode = "" Then
                sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,a.pan  where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            Else
                sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,a.pan where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
            End If
        End If
    End If
    If CmbClientBroker.Text = "CLIENT" Then
        sql = sql & " and cm.client_code=a.source_id"
    End If
        
    If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by Pravesh Chandel on 18 nov 2005
        sql = sql & " and lpad(a.inv_code,1)=3"
    '---------------------------Block Agent by mayank----------------------------------------
        sql = sql & " and lpad(a.inv_code,8) not in(select agent_code from agent_master where block_agent='1')"
    '----------------------------------------------------------------------------------------
    ElseIf CmbClientBroker.Text = "CLIENT" Then
        sql = sql & " and lpad(a.inv_code,1)=4"
    End If
    
    
    If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtPan.Text) = "" And Trim(txtMobile.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 And txtCliSubName.Text = "" Then
      sql = sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
    Else
'        If txtCode.Text <> "" Then
'            If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
'                Sql = Sql & " and source_id in (select agent_code from agent_master where (upper(exist_code) like '%" & UCase(Trim(txtCode.Text)) & "%' OR TO_CHAR(agent_code)='" & Trim(txtCode.Text) & "')) "
'            ElseIf CmbClientBroker.Text = "CLIENT" Then
'                Sql = Sql & " and source_id in (select client_code from client_master where (upper(exist_code) like '%" & UCase(Trim(txtCode.Text)) & "%' OR TO_CHAR(client_code)='" & Trim(txtCode.Text) & "')) "
'            End If
'        End If
'
'        If txtCliSubName.Text <> "" Then
'            If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
'                Sql = Sql & " and source_id in (select agent_code from agent_master where upper(trim(agent_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
'            ElseIf CmbClientBroker.Text = "CLIENT" Then
'                Sql = Sql & " and source_id in (select client_code from client_master where upper(trim(client_name)) like '%" & Replace(UCase(Trim(txtCliSubName.Text)), " ", "%") & "%')"
'            End If
'        End If
        If Txtcode.Text <> "" Then
            If CmbClientBroker.Text = "SUB BROKER" Then  ''Added by vinod on 15 Dec 2005
                sql = sql & " and source_id in (select agent_code from agent_master where (upper(exist_code) = '" & UCase(Trim(Txtcode.Text)) & "' OR TO_CHAR(agent_code)='" & Trim(Txtcode.Text) & "')) "
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
                
        If Txtname.Text <> "" Then
            sql = sql & " and upper(a.investor_name) like '%" & Replace(UCase(Trim(Txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
        If txtPan.Text <> "" Then
            sql = sql & " and upper(a.pan) like '%" & UCase(Trim(txtPan.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If txtMobile.Text <> "" Then
            sql = sql & " and (a.mobile) = '" & UCase(Trim(txtMobile.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
ElseIf Trim(UCase(Cmbcat.Text)) = "CLIENT" Then
    If SRmCode = "" Then
        If GlbDataFilter = "72" Then
            sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP')
        Else
            sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'and cm.kyc in('YES','YESP') and upper(a.kyc) IN ('YES', 'YESP')
        End If
    Else
        sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 " 'AND cm.kyc in('YES','YESP')and upper(a.kyc) IN ('YES', 'YESP')
    End If
    If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtPan.Text) = "" And Trim(txtMobile.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 Then
        sql = sql & " and  B.BRANCH_CODE in(" & SQLBranches & ") "
    Else
      
        If Txtcode.Text <> "" Then
            sql = sql & " and (upper(a.exist_code) = '" & UCase(Trim(Txtcode.Text)) & "' or TO_CHAR(a.client_code)='" & Trim(Txtcode.Text) & "') "
        End If
        
        If Txtname.Text <> "" Then
            sql = sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(Txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
        If txtPan.Text <> "" Then
            sql = sql & " and upper(a.Pan) like '%" & UCase(Trim(txtPan.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If txtMobile.Text <> "" Then
            sql = sql & " and (a.mobile) = '" & UCase(Trim(txtMobile.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
'    If cmbsort.Text = "Name" Then
'        Sql = Sql & " ORDER BY upper(a.CLIENT_NAME)"
'    ElseIf cmbsort.Text = "Address1" Then
'        Sql = Sql & " ORDER BY upper(trim(a.Address1))"
'    ElseIf cmbsort.Text = "Address2" Then
'        Sql = Sql & " ORDER BY upper(trim(a.Address2))"
'    ElseIf cmbsort.Text = "City" Then
'        Sql = Sql & " ORDER BY upper(trim(C.CITY_NAME))"
'    ElseIf cmbsort.Text = "Phone" Then
'        Sql = Sql & " ORDER BY upper(trim(a.phone))"
'    End If
ElseIf Trim(UCase(Cmbcat.Text)) = "AGENT" Then
    If SRmCode = "" Then
        sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
    Else
        sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(" & SRmCode & ") AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
    End If
    If Trim(Txtcode.Text) = "" And Trim(Txtname.Text) = "" And Trim(txtAdd1.Text) = "" And Trim(txtAdd2.Text) = "" And Trim(txtPhone.Text) = "" And Trim(txtPan.Text) = "" And Trim(txtMobile.Text) = "" And cmbBranch.ListIndex = -1 And cmbCity.ListIndex = -1 Then
        sql = sql & " and  B.BRANCH_CODE in(" & SQLBranches & ")"
    Else
        If Txtcode.Text <> "" Then
            sql = sql & " and (upper(a.exist_code) = '" & UCase(Trim(Txtcode.Text)) & "' or to_char(a.agent_code)='" & Trim(Txtcode.Text) & "') "
        End If
        If Txtname.Text <> "" Then
            sql = sql & " and upper(a.agent_NAME) like '%" & Replace(UCase(Trim(Txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
        If txtPan.Text <> "" Then
            sql = sql & " and upper(a.Pan) like '%" & UCase(Trim(txtPan.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If txtMobile.Text <> "" Then
            sql = sql & " and (a.Mobile) = '" & UCase(Trim(txtMobile.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
   
    sql = "Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
    sql = sql & " and A.FROM_RM_CODE = " & cmbOldRM.List(cmbOldRM.ListIndex, 2) & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        If Txtcode.Text <> "" Then
            'Sql = Sql & " and CLIENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            sql = sql & " and upper(a.exist_code) = '" & UCase(Trim(Txtcode.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If Txtname.Text <> "" Then
            sql = sql & " and upper(a.CLIENT_NAME) like '%" & Replace(UCase(Trim(Txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
        If txtPan.Text <> "" Then
            sql = sql & " and upper(a.Pan) like '%" & UCase(Trim(txtPan.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If txtMobile.Text <> "" Then
            sql = sql & " and (a.Mobile) = '" & UCase(Trim(txtMobile.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If cmbCity.ListIndex <> -1 Then
            sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If cmbBranch.ListIndex <> -1 Then
            sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        Else
            sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
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
        sql = "Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 "
    sql = sql & " and A.FROM_RM_CODE = " & cmbOldRM.List(cmbOldRM.ListIndex, 2) & "" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        If Txtcode.Text <> "" Then
            'Sql = Sql & " and AGENT_code like '%" & Trim(txtcode.Text) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
            sql = sql & " and upper(a.exist_code) = '" & UCase(Trim(Txtcode.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If Txtname.Text <> "" Then
            sql = sql & " and upper(a.agent_NAME) like '%" & Replace(UCase(Trim(Txtname.Text)), " ", "%") & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
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
        If txtPan.Text <> "" Then
            sql = sql & " and upper(a.Pan) like '%" & UCase(Trim(txtPan.Text)) & "%'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If txtMobile.Text <> "" Then
            sql = sql & " and (a.mobile) = '" & UCase(Trim(txtMobile.Text)) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If cmbCity.ListIndex <> -1 Then
            sql = sql & " and a.city_id='" & cmbCity.List(cmbCity.ListIndex, 1) & "'" ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
        If cmbBranch.ListIndex <> -1 Then
            sql = sql & " and B.BRANCH_CODE=" & cmbBranch.List(cmbBranch.ListIndex, 1) ' and B.BRANCH_CODE in(" & SQLBranches & ")"
        Else
            sql = sql & " and B.BRANCH_CODE in(" & SQLBranches & ")"
        End If
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

