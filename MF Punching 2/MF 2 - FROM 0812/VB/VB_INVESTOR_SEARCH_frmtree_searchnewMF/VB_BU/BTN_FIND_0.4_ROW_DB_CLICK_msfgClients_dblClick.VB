Private Sub msfgClients_dblClick()
On Error GoTo err1
'On Error resume Next
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
    '--------------------------------------Restrict To Update Account Details------------
    If currentForm.Name = "frmtransactionmf" Then
       MyLoggedUserid = SqlRet("select loggeduserid from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
       MyMainCode = SqlRet("select main_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
       If MyLoggedUserid = "PROC" Then
          MyUpdProc = SqlRet("select NVL(UPD_PROC,'N') from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & " and dob is not null")
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
    '--------------------------------------------------------------------------------------
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
        End With
        Me.MousePointer = vbNormal
        Set rsCount = Nothing
        DoEvents
        msfgClients.SetFocus
        DoEvents
        'Unload Me
        Exit Sub
    End If
    If currentForm.Name = "frmtransactionmf" Then
        If frmtransactionmf.SSTab1.Tab = 0 Then
            frmtransactionmf.TxtClientCodeA = Mid(msfgClients.TextMatrix(R, 1), 1, 8)
            frmtransactionmf.Label32.Caption = msfgClients.TextMatrix(R, 1)
            frmtransactionmf.txtInvestorA = msfgClients.TextMatrix(R, 0)
            If Mid(msfgClients.TextMatrix(R, 1), 1, 1) = "4" Then
                frmtransactionmf.TxtPanA = msfgClients.TextMatrix(R, 9)
                'frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
                frmtransactionmf.TxtAcHolderA = SqlRet("select client_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
                If frmtransactionmf.TxtAcHolderA.Text = "" Or Len(frmtransactionmf.TxtAcHolderA.Text) < 6 Then
                    MsgBox "Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same", vbInformation, "Wealthmaker"
                    Me.MousePointer = vbIconPointer
                    Exit Sub
                End If
            Else
                frmtransactionmf.TxtPanA = msfgClients.TextMatrix(R, 8)
                'frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
            End If
            Me.MousePointer = vbNormal
            Unload Me
        ElseIf frmtransactionmf.SSTab1.Tab = 1 Then
            frmtransactionmf.TxtClientCodeM = Mid(msfgClients.TextMatrix(R, 1), 1, 8)
            frmtransactionmf.txtInvestorM = msfgClients.TextMatrix(R, 0)
            If Mid(msfgClients.TextMatrix(R, 1), 1, 1) = "4" Then
                frmtransactionmf.TxtPanM = msfgClients.TextMatrix(R, 9)
                frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
                frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
            Else
                frmtransactionmf.TxtPanA = msfgClients.TextMatrix(R, 8)
                frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
                frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
            End If
            frmtransactionmf.Label42.Caption = msfgClients.TextMatrix(R, 1)
            Me.MousePointer = vbNormal
            Unload Me
        End If
        If frmtransactionmf.SSTab1.Tab = 0 Then
            'Call frmtransactionmf.TxtBusiCodeA_LostFocus
        Else
            'Call frmtransactionmf.TxtBusiCodeM_LostFocus
        End If
        Exit Sub
    End If
    If currentForm.Name = "frmIPO" Then
        frmIPO.txtINV_CD.Text = msfgClients.TextMatrix(R, 1)
        frmIPO.Txtname.Text = msfgClients.TextMatrix(R, 0)
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
    
    
    If currentForm.Name = "frmFP" Then
    Dim rsFP As New ADODB.Recordset
    Dim rs_inv As New ADODB.Recordset
    Dim strsql1 As String
    Dim strsql2 As String
    frmFP.txtFdate.Text = "__/__/____"
    frmFP.txtTdate.Text = "__/__/____"
     frmFP.cmbstatus.Text = ""
        frmFP.txtINV_CD.Text = msfgClients.TextMatrix(R, 1)
        frmFP.Txtname.Text = msfgClients.TextMatrix(R, 0)
        
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
                frmFP.cmbstatus.SelText = rsFP("status")
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
            frmFP.cmbstatus.Text = "Snapshot Finalized"
        Else
            If role_name = "FINANCIAL PLANNING" Or GlbDataFilter = "72" Or role_name = "FPM (AUDIT)" Then
                frmFP.cmbstatus.Enabled = True
                If frmFP.chkaudit.Enabled = True Then
                    frmFP.chkaudit.Enabled = True
                    frmFP.MSK_AUDIT_DATE.Enabled = True
                End If
            Else
                frmFP.cmbstatus.Enabled = False
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
        frmUpdateRecdSlabs.Txtname.Text = msfgClients.TextMatrix(R, 0)
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        Else
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
                frmAR.FrmConfirmBoth.Visible = True
                frmAR.FrmConfirmBoth.Move frmAR.txtInsured.Left, frmAR.txtInsured.Top - 200
                frmAR.txtInsured1.Text = ""
                frmAR.txtProposer1.Text = ""
            End If
            frmAR.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            frmAR.txtInsured.Text = msfgClients.TextMatrix(R, 0)
            frmAR.txtProposer.Text = msfgClients.TextMatrix(R, 0)
            frmAR.txtAdd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR.txtAdd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR.txtIadd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR.txtIadd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR.txtPhone.Text = msfgClients.TextMatrix(R, 6)
            frmAR.fillData
            frmAR.txtClientCD.Enabled = False
            If Left(msfgClients.TextMatrix(R, 1), 1) <> "3" Then
                frmAR.TguestCode.Text = msfgClients.TextMatrix(R, 8)
            End If
           'frmAR.Lbl_LeadSearch_Click
            Me.MousePointer = vbNormal
            Unload Me
        '' Exit Sub
        End If
        If currentForm.Name = "frmAR_Renewal" And Ar_type = "" Then 'jogi010907
            frmAR_Renewal.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            frmAR_Renewal.Lbl_P_Code.Caption = msfgClients.TextMatrix(R, 1)
            
            frmAR_Renewal.txtInsured.Text = msfgClients.TextMatrix(R, 0)
            frmAR_Renewal.txtProposer.Text = msfgClients.TextMatrix(R, 0)
            frmAR_Renewal.txtAdd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR_Renewal.txtAdd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR_Renewal.txtIadd1.Text = msfgClients.TextMatrix(R, 2)
            frmAR_Renewal.txtIadd2.Text = msfgClients.TextMatrix(R, 3)
            frmAR_Renewal.txtPhone.Text = msfgClients.TextMatrix(R, 6)
            frmAR_Renewal.fillData
            frmAR_Renewal.txtClientCD.Enabled = False
            'frmAR.Lbl_LeadSearch_Click
            Me.MousePointer = vbNormal
            Unload Me
        '' Exit Sub
        End If
        If currentForm.Name = "frmAR" And Ar_type = "P_NAME" Then 'jogi010907
        frmAR.Lbl_P_Code.Caption = msfgClients.TextMatrix(R, 1)
        frmAR.txtProposer.Text = msfgClients.TextMatrix(R, 0)
        
        frmAR.txtAdd1.Text = msfgClients.TextMatrix(R, 2)
        frmAR.txtAdd2.Text = msfgClients.TextMatrix(R, 3)
        
        frmAR.txtPhone.Text = msfgClients.TextMatrix(R, 6)
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
        
        frmAR_Renewal.txtAdd1.Text = msfgClients.TextMatrix(R, 2)
        frmAR_Renewal.txtAdd2.Text = msfgClients.TextMatrix(R, 3)
        
        frmAR_Renewal.txtPhone.Text = msfgClients.TextMatrix(R, 6)
        'frmAR.fillData
        Me.MousePointer = vbNormal
        Unload Me
    
    End If
    If currentForm.Name = "frmAR" And Ar_type = "N_NAME" Then  'jogi010907
        frmAR.Lbl_N_Code.Caption = msfgClients.TextMatrix(R, 1)
        frmAR.txtNominee.Text = msfgClients.TextMatrix(R, 0)
        
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
    'By joginder  for venue day book form
    If currentForm.Name = "FrmVenDayBook" Then
            FrmVenDayBook.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            FrmVenDayBook.txtclname.Text = msfgClients.TextMatrix(R, 0)
            FrmVenDayBook.fillData
            Me.MousePointer = vbNormal
        Unload Me
     '' Exit Sub
    End If
    'end Change by joginder
         
     If currentForm.Name = "frmARGeneral" Then
            frmARGeneral.txtClientCD.Text = msfgClients.TextMatrix(R, 1)
            frmARGeneral.txtClientCD.Enabled = False
            frmARGeneral.txtAdd1.Text = msfgClients.TextMatrix(R, 2)
            frmARGeneral.txtProposer.Text = msfgClients.TextMatrix(R, 0)
            frmARGeneral.txtAdd2.Text = msfgClients.TextMatrix(R, 3)
            frmARGeneral.txtIadd1.Text = msfgClients.TextMatrix(R, 2)
            frmARGeneral.txtIadd2.Text = msfgClients.TextMatrix(R, 3)
            frmARGeneral.txtPhone.Text = msfgClients.TextMatrix(R, 6)
            frmARGeneral.fillData
            Me.MousePointer = vbNormal
        Unload Me
     '' Exit Sub
    End If
    
    
    
    
    
       If currentForm.Name = "frm_MF_AUM_rpt" Then
       '' Dim EXISTCODE As String
         EXISTCODE = ""
        If Left(msfgClients.TextMatrix(R, 1), 1) = "3" Then
            Set rsexistcode = MyConn.Execute("select exist_code from agent_master where agent_code='" & msfgClients.TextMatrix(R, 1) & "'")
        Else
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
        Else
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
        Else
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
        Else
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
        Else
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
        Else
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
        Else
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
        Else
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
If currentForm.Name <> "frmAR" And currentForm.Name <> "frmARGeneral" And currentForm.Name <> "FrmVenDayBook" And currentForm.Name <> "frmAR_Renewal" Then 'jogi010907
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
        'treeClientFill currentForm.Controls(treeName), "R" & CStr(pcode), currentForm.Controls(treeName).Nodes("R" & pcode).Child.Key  'BY HIMANSHU
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
        'treeAgentFill currentForm.Controls(treeName), "R" & CStr(pcode), currentForm.Controls(treeName).Nodes("R" & CStr(pcode)).Child.Key, Left(msfgClients.TextMatrix(r, 1), 8) 'BY HIMANSHU LATEST  06-SEP
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
                    'treeClientFill currentForm.Controls(treeName), "R" & CStr(pcode), currentForm.Controls(treeName).Nodes("R" & pcode).Child.Key
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
End If
    If susPense = "" Then
    End If
    Me.MousePointer = vbNormal
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
    If currentForm.Name = "FrmtransactionNew" Then
        FrmtransactionNew.treeClient_Click
        DoEvents
        If FrmtransactionNew.cmbBankName.Enabled = True Then
            FrmtransactionNew.cmbBankName.SetFocus
        End If
        DoEvents
    End If
    If currentForm.Name = "frmPaySlabs" Then
        frmPaySlabs.treeClient_Click
        DoEvents
    End If
  
End If
'ankit
'If currentForm.Name = "FrmtransactionNew" Then
'Call FrmtransactionNew.Lbl_LeadSearch_Click
'ElseIf currentForm.Name = "frmAR" Then
'Call frmAR.Lbl_LeadSearch_Click
'ElseIf currentForm.Name = "frmARGeneral" Then
'Call frmARGeneral.Lbl_LeadSearch_Click
'End If

Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
    Resume
End Sub
