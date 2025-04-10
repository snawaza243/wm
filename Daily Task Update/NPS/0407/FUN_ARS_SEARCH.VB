Public Sub cmdSearch_Click()
On Error Resume Next
Dim StrSql As String
Dim RS1 As New ADODB.Recordset
Dim strSearch As String
Dim AppNo As String, ARNO As String, chequeno As String, marno As String

MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM CD|BUSI_CD|B_CD|REM|PLAN_NO|PLAN_DT|DOC"
MSFlexGrid1.ColWidth(21) = 0
MSFlexGrid1.ColWidth(22) = 0
MSFlexGrid1.ColWidth(23) = 0
MSFlexGrid1.ColWidth(24) = 0
MSFlexGrid1.ColWidth(25) = 0
MSFlexGrid1.ColWidth(26) = 0
MSFlexGrid1.ColWidth(27) = 0
MSFlexGrid1.ColWidth(31) = 1000
    
    StrSql = ""
    strSearch = ConvertString(UCase(Trim(txtScheme.Text)))
    If optAfter.Value = True Then
        If GlbDataFilter = "72" Then
            StrSql = "select * from transaction_st where "
        Else
            StrSql = "select * from transaction_st where (flag<>'NEWTRAN' OR FLAG IS NULL) and "
        End If
    Else
        StrSql = "select * from transaction_sttemp where "
    End If
    If IsDate(mskfromdate.Text) = False And mskfromdate.Text <> "__/__/____" Then
        MsgBox "Please Enter Valid From Date", vbInformation
        mskfromdate = "__/__/____"
        mskfromdate.SetFocus
        Exit Sub
    End If
    If IsDate(msktodate.Text) = False And msktodate.Text <> "__/__/____" Then
        MsgBox "Please Enter Valid To Date", vbInformation
        msktodate = "__/__/____"
        msktodate.SetFocus
        Exit Sub
    End If
    If IsDate(mskfromdate.Text) = True And IsDate(mskfromdate.Text) = True And mskfromdate.Text <> "__/__/____" And msktodate.Text <> "__/__/____" Then
        If CDate(Format(mskfromdate.Text, "DD/MM/YYYY")) > CDate(Format(msktodate.Text, "DD/MM/YYYY")) Then
            MsgBox "From Date should be less then To Date", vbInformation
            mskfromdate = "__/__/____"
            msktodate = "__/__/____"
            mskfromdate.SetFocus
            Exit Sub
        End If
    End If
    If Trim(txtScheme.Text) = "" And Trim(txtappno.Text) = "" And Trim(txtarno.Text) = "" And Trim(txtchequeno.Text) = "" And txtmarno.Text = "" And mskfromdate.Text = "__/__/____" And msktodate.Text = "__/__/____" And Trim(txtInvName.Text) = "" And Trim(txtANACD.Text) = "" Then
        MsgBox "Please Enter Searching Parameter", vbInformation
        Exit Sub
    End If
    AppNo = ""
    cmdsearch.Enabled = False
    prgBar.Visible = True
    prgBar.Min = 0
    prgBar.Value = 0
    If txtappno.Text <> "" Then
        AppNo = Trim(txtappno.Text)
        StrSql = StrSql & "App_no='" & AppNo & "' and "
    End If
    If txtarno.Text <> "" Then
        ARNO = Trim(txtarno.Text)
        StrSql = StrSql & " tran_code = '" & ARNO & "' and "

    End If
    If txtchequeno.Text <> "" Then
        chequeno = Trim(txtchequeno.Text)
        StrSql = StrSql & "cheque_no LIKE '%" & chequeno & "%' and "
    End If
    If txtmarno.Text <> "" Then
        marno = Trim(txtmarno.Text)
        StrSql = StrSql & "manual_arno='" & marno & "' and "
    End If
    
    If EPF = True Then
        If frmPayment.Opt_client.Value = True Then
            StrSql = StrSql & " substr(Client_code,1,1)='4' and "
        Else
            StrSql = StrSql & " substr(Client_code,1,1)='3' and "
        End If
    End If
     '----------------NPS Tranactions-----------------------------------
    If currentForm.Name = "frmNPS" Then
        StrSql = StrSql & " sch_code in('OP#09971','OP#09972','OP#09973')  and "
    End If
    If currentForm.Name = "frmpaymentreced" Then
        StrSql = StrSql & " mut_code in(select iss_code from iss_master where prod_code='DT028')  and "
    End If
    '-------------------------------------------------------------------
    If mskfromdate.Text <> "__/__/____" And msktodate.Text <> "__/__/____" Then
        StrSql = StrSql & "tr_date >= to_date('" & mskfromdate.Text & "', 'dd-MM-yyyy') and "
        StrSql = StrSql & "tr_date <= to_date('" & msktodate.Text & "', 'dd-MM-yyyy') and "
    ElseIf mskfromdate.Text <> "__/__/____" And msktodate.Text = "__/__/____" Then
        StrSql = StrSql & "tr_date = to_date('" & mskfromdate.Text & "', 'dd-MM-yyyy') and "
    ElseIf msktodate.Text <> "__/__/____" And mskfromdate.Text = "__/__/____" Then
        StrSql = StrSql & "tr_date = to_date('" & msktodate.Text & "', 'dd-MM-yyyy') and "
    End If
    If RS.State = 1 Then RS.Close
    MSFlexGrid1.Clear
    'MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM_CD|BUSI_CD|B_CD|REM"
    MSFlexGrid1.FormatString = " AR Number       | AR Date          | Investor Name     | Issuer Name        | Scheme Name     | Branch Name      | App No    | App Date   | Folio No    | Tran Type   | Amount    | Unit     | Rate     | Nav Date   | Payment Mode | Cheque No   | Cheque Date    | Bank Name       | Broker Name      | Rm Name       | Manual ARNo  |Mut_CD|Sch_CD|Inv_CD|Brok_ID|RM CD|BUSI_CD|B_CD|REM|PLAN_NO|PLAN_DT"
    MSFlexGrid1.ColWidth(21) = 0
    MSFlexGrid1.ColWidth(22) = 0
    MSFlexGrid1.ColWidth(23) = 0
    MSFlexGrid1.ColWidth(24) = 0
    MSFlexGrid1.ColWidth(25) = 0
    MSFlexGrid1.ColWidth(26) = 0
    MSFlexGrid1.ColWidth(27) = 0
    
    If txtScheme.Text <> "" Then
        StrSql = StrSql & " (sch_code in (select sch_code from scheme_info,MUT_FUND where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) like '%" & strSearch & "%') or sch_code in (select osch_code from other_product o, product_master p,iss_master i where o.prod_class_code=p.prod_code and o.iss_code=i.iss_code AND UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) like '%" & strSearch & "%')) AND "
    End If
    If txtInvName.Text <> "" Then
        StrSql = StrSql & " (client_code in (select distinct inv_code from investor_master where upper(investor_name) like '%" & Trim(UCase(txtInvName.Text)) & "%')) AND "
    End If
    If txtANACD.Text <> "" Then
        StrSql = StrSql & " (source_code in (select agent_code from agent_master where trim(exist_code) ='" & Trim(txtANACD.Text) & "')) AND "
    End If
    If SRmCode = "" Then
        StrSql = StrSql & " Branch_code in (" & Branches & ") order by tr_date"
    Else
        StrSql = StrSql & " Branch_code in (" & Branches & ") and RMCODE IN (" & SRmCode & ") order by tr_date"
    End If
    RS.open StrSql, MyConn, adOpenKeyset
    If RS.RecordCount <> 0 Then
        prgBar.Max = RS.RecordCount
    End If
    Label8.Caption = RS.RecordCount
    DoEvents
    MSFlexGrid1.Rows = 2
    MSFlexGrid1.FixedRows = 1
    MSFlexGrid1.FixedCols = 0
    MSFlexGrid1.Rows = 1
    MSFlexGrid1.AllowUserResizing = flexResizeBoth
    With MSFlexGrid1
        i = 0
        While Not RS.EOF
            prgBar.Value = prgBar.Value + 1
            MSFlexGrid1.Rows = MSFlexGrid1.Rows + 1
                .TextMatrix(i + 1, 0) = RS.Fields("tran_code")
                .TextMatrix(i + 1, 1) = Format(RS("tr_date"), "dd/mm/yyyy")
            clcode = RS.Fields("client_code")
            MSFlexGrid1.TextMatrix(i + 1, 23) = clcode
            MSFlexGrid1.TextMatrix(i + 1, 26) = RS("BUSINESS_RMCODE")
            If clcode <> "" Then
                If RS1.State = 1 Then RS1.Close
                strsql1 = "select investor_name from investor_master where inv_code='" & clcode & "'"
                RS1.open strsql1, MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 2) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            Else
                .TextMatrix(i + 1, 2) = ""
                RS1.Close
                Set RS1 = Nothing
            End If
            mtcode = RS.Fields("mut_code")
            MSFlexGrid1.TextMatrix(i + 1, 21) = mtcode
            If mtcode <> "" And Left(mtcode, 2) = "MF" And Left(mtcode, 4) <> "MFIS" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select mut_name from mut_fund where mut_code='" & mtcode & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 3) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            ''End If
            ''If mtcode <> "" And Left(mtcode, 2) <> "MF" Then
            Else
                If RS1.State = 1 Then RS1.Close
                RS1.open "select iss_name from iss_master where iss_code='" & mtcode & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 3) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
            SCHCODE = RS.Fields("sch_code")
            MSFlexGrid1.TextMatrix(i + 1, 22) = SCHCODE
            If SCHCODE <> "" And mtcode <> "" And (Left(mtcode, 2) = "IS" Or Left(mtcode, 4) = "MFIS") Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select longname||'-'||osch_name from other_product where osch_code='" & SCHCODE & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 4) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            Else
            End If
            If SCHCODE <> "" And Left(mtcode, 2) = "MF" And Left(mtcode, 4) <> "MFIS" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select SHORT_NAME from scheme_info where sch_code='" & SCHCODE & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 4) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
            branchcode = RS.Fields("branch_code")
            If branchcode <> "" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select branch_name from branch_master where branch_code='" & branchcode & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 5) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
                .TextMatrix(i + 1, 6) = RS.Fields("app_no")
                .TextMatrix(i + 1, 7) = Format(RS.Fields("app_date"), "dd/mm/yyyy")
                .TextMatrix(i + 1, 8) = RS.Fields("folio_no")
                .TextMatrix(i + 1, 9) = RS.Fields("tran_type")
                .TextMatrix(i + 1, 10) = RS.Fields("amount")
                .TextMatrix(i + 1, 11) = RS.Fields("units")
                .TextMatrix(i + 1, 12) = RS.Fields("rate")
                .TextMatrix(i + 1, 13) = Format(RS.Fields("nav_date"), "dd/mm/yyyy")
                .TextMatrix(i + 1, 14) = RS.Fields("payment_mode")
                .TextMatrix(i + 1, 15) = RS.Fields("cheque_no")
                .TextMatrix(i + 1, 16) = Format(RS.Fields("cheque_date"), "dd/mm/yyyy")
                .TextMatrix(i + 1, 17) = RS.Fields("bank_name")
            brokid = RS.Fields("broker_id")
            If brokid <> "" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select name from broker_master where broker_id='" & brokid & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 18) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
            rmcode1 = RS.Fields("rmcode")
            .TextMatrix(i + 1, 25) = RS.Fields("rmcode")
            
            .TextMatrix(i + 1, 29) = RS.Fields("CROR_PLANNO")
            .TextMatrix(i + 1, 30) = RS.Fields("plan_type")
            .TextMatrix(i + 1, 31) = RS.Fields("DOC_ID")
            
            If rmcode1 <> "" Then
                If RS1.State = 1 Then RS1.Close
                RS1.open "select rm_name from employee_master where rm_code='" & rmcode1 & "'", MyConn, adOpenForwardOnly
                    .TextMatrix(i + 1, 19) = RS1.Fields(0)
                    RS1.Close
                    Set RS1 = Nothing
            End If
                .TextMatrix(i + 1, 20) = RS.Fields("manual_arno")
                .TextMatrix(i + 1, 27) = RS.Fields("busi_branch_code")
                .TextMatrix(i + 1, 28) = RS.Fields("Remark")
                
                .TextMatrix(i + 1, 24) = RS.Fields("broker_id")
                If .TextMatrix(i + 1, 9) = "REVERTAL" Then
                    .Row = i + 1
                    .Col = 0
                    .CellBackColor = vbYellow
                End If
            RS.MoveNext
            i = i + 1
            DoEvents
        Wend
    End With
    MSFlexGrid1.SetFocus
    Label7.Visible = True
    Label8.Caption = MSFlexGrid1.Rows - 1
    cmdsearch.Enabled = True
    prgBar.Visible = False
End Sub