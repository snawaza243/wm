Private Sub cmdShow_Click()
On Error Resume Next
Dim currentForm As Form
Dim rsGetDocPath As New ADODB.Recordset
Dim rsFind As New ADODB.Recordset
Dim rs_get_rm As New ADODB.Recordset
Dim rs_get_invsrc As New ADODB.Recordset
Dim rsnat As ADODB.Recordset
Dim sql As String
    
    rsGetDocPath.open "select a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
        If IsNull(rsGetDocPath("sch_code")) = False Then
            
            sql = "select osch_code sch_code,osch_name sch_name,longname Short_name,name,iss_name mut_name from other_product o, product_master p,iss_master i where osch_code='" & rsGetDocPath("sch_code") & "' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null)  "
            sql = sql & " union all "
            sql = sql & " select sch_code,sch_name,Short_name,'MF' name,mut_name from scheme_info s,mut_fund m where s.sch_code='" & rsGetDocPath("sch_code") & "' and s.mut_code=m.mut_code and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') And s.sch_code in (select sch_code from mp_scheme_master sm where sm.active_status='Active' )  "
            rsFind.open sql, MyConn, adOpenForwardOnly
            
            If SSTab1.Tab = 0 Then  ''add button
                Set rsnat = New ADODB.Recordset
                rsnat.open "select nvl(get_scheme_nature('" & UCase(rsGetDocPath("sch_code")) & "'),'') a from dual", MyConn, adOpenForwardOnly
                If rsnat("a") = "O" Then
                    frmtransactionmf.Label2(16).Caption = "Expenses agnst. Trail%"
                    frmtransactionmf.Label2(17).Caption = "Expenses agnst. Trail(Rs.)"
                    frmtransactionmf.MyNat = "O"
                Else
                    frmtransactionmf.Label2(16).Caption = "Expenses%"
                    frmtransactionmf.Label2(17).Caption = "Expenses(Rs.)"
                    frmtransactionmf.MyNat = "C"
                End If
                rsnat.Close
                Set rsnat = Nothing

                
                If frmtransactionmf.OptChkSwitch.Value = 0 Then
                    frmtransactionmf.CmbAmcA.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameA.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameA.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            frmtransactionmf.TxtSchemeA.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameA.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.CmbAmcA.SetFocus
                    If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                        frmtransactionmf.ChkAtmTransactionA.Value = 0
                        frmtransactionmf.ChkAtmTransactionA.Enabled = True
                    Else
                        frmtransactionmf.ChkAtmTransactionA.Value = 0
                        frmtransactionmf.ChkAtmTransactionA.Enabled = False
                    End If
                    
                Else
                    frmtransactionmf.CmbSwitchAmcA.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameA.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameA.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            'frmtransactionmf.lstlongnameA.Selected(I) = True
                            frmtransactionmf.TxtSwitchSchemeA.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameA.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.TxtSwitchSchemeA.SetFocus
                End If
            Else  ''modification
            
                Set rsnat = New ADODB.Recordset
                rsnat.open "select nvl(get_scheme_nature('" & UCase(rsGetDocPath("sch_code")) & "'),'') a from dual", MyConn, adOpenForwardOnly
                If rsnat("a") = "O" Then
                    frmtransactionmf.Label2(16).Caption = "Expenses agnst. Trail%"
                    frmtransactionmf.Label2(17).Caption = "Expenses agnst. Trail(Rs.)"
                    frmtransactionmf.MyNat = "O"
                Else
                    frmtransactionmf.Label2(16).Caption = "Expenses%"
                    frmtransactionmf.Label2(17).Caption = "Expenses(Rs.)"
                    frmtransactionmf.MyNat = "C"
                End If
                rsnat.Close
                Set rsnat = Nothing
                
                 If frmtransactionmf.OptChkSwitchM.Value = 0 Then
                    frmtransactionmf.CmbAmcM.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameM.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameM.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            frmtransactionmf.TxtSchemeM.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameM.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.CmbAmcM.SetFocus
                    
                    If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                        frmtransactionmf.ChkAtmTransactionM.Value = 0
                        frmtransactionmf.ChkAtmTransactionM.Enabled = True
                    Else
                        frmtransactionmf.ChkAtmTransactionM.Value = 0
                        frmtransactionmf.ChkAtmTransactionM.Enabled = False
                    End If
                Else
                    frmtransactionmf.CmbSwitchAmcM.Text = rsFind("mut_name")
                    For i = 0 To frmtransactionmf.lstlongnameM.ListCount - 1
                        If UCase(Trim(Mid(frmtransactionmf.lstlongnameM.List(i), 1, 99))) = UCase(rsFind("SCH_NAME")) Then
                            frmtransactionmf.TxtSwitchSchemeM.Text = UCase(Trim(frmtransactionmf.lstlongnameA.List(i))) 'frmtransactionmf.lstlongnameM.Text
                            Exit For
                        End If
                    Next
                    frmtransactionmf.TxtSwitchSchemeM.SetFocus
                End If
            End If

        Else
            CmbAmcA.ListIndex = -1
            CmbAmcA_Click
            frmtransactionmf.TxtSchemeA.Text = ""
        End If
        
        If IsNull(rsGetDocPath("Inv_code")) = False Then
        
            sql = "Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,'',a.pan " & _
                    "FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) " & _
                    "and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 and a.inv_code=" & rsGetDocPath("Inv_code")
            rs_get_invsrc.open sql, MyConn, adOpenForwardOnly
                    
            MyLoggedUserid = SqlRet("select loggeduserid from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
            MyMainCode = SqlRet("select main_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
            If MyLoggedUserid = "PROC" Then
               MyUpdProc = SqlRet("select NVL(UPD_PROC,'N') from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & " and dob is not null")
               If (Mid(rsGetDocPath("Inv_code"), 1, 1) <> 3) Then
               If MyUpdProc = "N" Or MyUpdProc = "0" Then
                  MsgBox "Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account", vbInformation
                  Unload Me
                  frmactopen.Show
                  frmactopen.ZOrder
                  frmactopen.txtclientcode = MyMainCode
                  frmactopen.cmdview_Click
                  Exit Sub
               End If
               End If
            End If
            If frmtransactionmf.SSTab1.Tab = 0 Then
                frmtransactionmf.TxtClientCodeA = Mid(rsGetDocPath("Inv_code"), 1, 8)
                frmtransactionmf.Label32.Caption = rsGetDocPath("Inv_code")
                frmtransactionmf.txtInvestorA = rs_get_invsrc("INVESTOR_NAME") ''name
                
                
                If Mid(rsGetDocPath("Inv_code"), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanA = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                    frmtransactionmf.TxtAcHolderA = SqlRet("select client_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
                    If frmtransactionmf.TxtAcHolderA.Text = "" Or Len(frmtransactionmf.TxtAcHolderA.Text) < 6 Then
                        MsgBox "Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same", vbInformation, "Wealthmaker"
                        Me.MousePointer = vbIconPointer
                        Exit Sub
                    End If
                Else
                    frmtransactionmf.TxtPanA = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtBusiCodeA.Text = rsGetDocPath("busi_rm_code")
                    'frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                End If
                Me.MousePointer = vbNormal
                'Unload Me
            ElseIf frmtransactionmf.SSTab1.Tab = 1 Then
                frmtransactionmf.TxtClientCodeM = Mid(rsGetDocPath("Inv_code"), 1, 8)
                frmtransactionmf.txtInvestorM = rs_get_invsrc("INVESTOR_NAME") ''name
                If Mid(rsGetDocPath("Inv_code"), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanM = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
                    frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                Else
                    frmtransactionmf.TxtPanA = rs_get_invsrc("PAN")
                    frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & rsGetDocPath("Inv_code") & "")
                    frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(rsGetDocPath("Inv_code"), 1, 8) & ")")
                End If
                frmtransactionmf.Label42.Caption = rsGetDocPath("Inv_code")
                Me.MousePointer = vbNormal
                Unload Me
            End If
            If frmtransactionmf.SSTab1.Tab = 0 Then
                Call frmtransactionmf.TxtBusiCodeA_LostFocus
            Else
                Call frmtransactionmf.TxtBusiCodeM_LostFocus
            End If
            
        Else
                frmtransactionmf.TxtClientCodeA = ""
                frmtransactionmf.Label32.Caption = ""
                frmtransactionmf.txtInvestorA = ""
                If Mid(rsGetDocPath("Inv_code"), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanA = ""
                    frmtransactionmf.TxtBusiCodeA = ""
                    frmtransactionmf.TxtAcHolderA = ""
                Else
                    frmtransactionmf.TxtPanA = ""
                    frmtransactionmf.TxtBusiCodeA = ""
                End If
        
        End If
        
        
        TxtBusiCodeA.Text = rsGetDocPath("busi_rm_code")
        ImEntryDtA.Text = rsGetDocPath("busi_tr_date")
        If GlbroleId = 212 Then
            CmbAmcA.Enabled = False
            TxtSchemeA.Enabled = False
            ImEntryDtA.Enabled = False
        Else
            CmbAmcA.Enabled = True
            TxtSchemeA.Enabled = True
            ImEntryDtA.Enabled = True
        End If
        If rsGetDocPath("expense") > 0 Then
            ImnExp_Per.Text = rsGetDocPath("expense")
        Else
            ImnExp_Per.Text = 0
        End If
        cmbBusiBranch.Text = rsGetDocPath("busi_branch_code") & Space(100) & "#" & rsGetDocPath("branch_name")
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
    Set rs_get_invsrc = Nothing
    Set rs_get_rm = Nothing
    Set rsFind = Nothing
    
    
     '''''''''''''''''''''''''''''''''''''''''''''''''CROSS CHANNEL VALIDATION--------------------------------------------------------
    rsGetDocPath.open "select a.busi_rm_code,a.busi_branch_code,a.busi_tr_date,a.sch_code,a.inv_code,b.branch_name,a.expense from tb_doc_upload a,branch_master b where a.busi_branch_code=b.branch_code and common_id='" & Trim(txtdocID.Text) & "' and tran_type='MF' and verification_flag='1' and rejection_status='0'  and punching_flag='0'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
            vApprovalFlag = SqlRet("select wealthmaker.fn_check_for_approval_all('" & txtdocID.Text & "')  from dual")
            'vApprovalFlag 2  Approval request for this transaction has already been raised.
            'vApprovalFlag 4  Approval request for this transaction has rejected by Management.
            
            If vApprovalFlag = 2 Then
                MsgBox "Approval request for this transaction has already been raised.", vbInformation, strBajaj
                Exit Sub
            End If
            
            If vApprovalFlag = 4 Then
                MsgBox "Approval request for this transaction has been rejected by Management.", vbInformation, strBajaj
                Exit Sub
            End If
        
            If Label32.Caption = "" Then
                MsgBox "Information not present", vbInformation, strBajaj
                Exit Sub
            End If
            
            Dim cmd As New ADODB.Command
            Dim vResult As String
            Set cmd.ActiveConnection = MyConn
            cmd.CommandType = adCmdStoredProc
            cmd.CommandText = "WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO"
            cmd.Parameters.Append cmd.CreateParameter("PCOMMON_ID", adVarChar, adParamInput, 15, txtdocID.Text)
            cmd.Parameters.Append cmd.CreateParameter("PSUB_CLIENT_CD", adVarChar, adParamInput, 15, Label32.Caption)
            cmd.Parameters.Append cmd.CreateParameter("PLOGIN_ID", adVarChar, adParamInput, 15, Glbloginid)
            cmd.Parameters.Append cmd.CreateParameter("PCNT", adDouble, adParamOutput, 100)
            cmd.Execute
            vResult = IIf(IsNull(cmd("PCNT")), 0, cmd("PCNT"))
            Set cmd = Nothing
            
            If vResult > 0 Then
                frmCrossChannelValidate.formtype = "MF"
                frmCrossChannelValidate.CommonId = txtdocID.Text
                frmCrossChannelValidate.loggeduserid = Glbloginid
                frmCrossChannelValidate.Show
                frmCrossChannelValidate.ZOrder 0
            Else
                If Label32.Caption <> "" Then
                    Change_bt_Click (0)
                End If
            End If
    Else
        MsgBox "Information not present", vbInformation, strBajaj
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
End Sub

