Private Sub gridfill(X As Integer)
On Error GoTo err1
If SSTab1.Tab = 0 Then
    StrSql = "select mf.investor_name,mf.bank_name,mf.SCH_CODE,rm_name,sip_type,branch_name,PANNO,amc.mut_name Amc_Name,sch_name Scheme_Name,TR_DATE,TRAN_TYPE,App_No,PAYMENT_MODE,CHEQUE_NO,CHEQUE_DATE,"
    StrSql = StrSql & "Amount,TRAN_TYPE,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.frequency,mf.installments_no,micro_investment,target_switch_scheme "
    StrSql = StrSql & " from employee_master e,branch_master b,mut_fund amc,scheme_info sch,transaction_mf_Temp1 mf "
    StrSql = StrSql & " where to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  "
    StrSql = StrSql & " and mf.MUT_CODE=amc.mut_code and mf.SCH_CODE=sch.sch_code and mf.BUSINESS_RMCODE=" & TxtBusiCodeA & " AND trunc(update_date) = to_date(SYSDATE)  order by "
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_DATE,rm_name "
    Else
        StrSql = StrSql & "" & MyOrder
    End If
    If Option1(1).Value = True Then
        StrSql = StrSql & " Desc"
    End If
    '
ElseIf SSTab1.Tab = 1 Then
    StrSql = "select * from temporaryBusiness where BUSI_BRANCH_CODE  in ( " & Branches & ")"
    If mskfromdt.Text <> "__/__/____" Then
        StrSql = StrSql & " and TR_DATE>=TO_DATE('" & mskfromdt & "','DD/MM/YYYY') "
    End If
    If msktodt.Text <> "__/__/____" Then
        StrSql = StrSql & " and TR_DATE<=TO_DATE('" & msktodt & "','DD/MM/YYYY') "
    End If
    If TxtPanS.Text <> "" Then
        StrSql = StrSql & " and PANNO='" & TxtPanS.Text & "' "
    End If
    If TxtClientS.Text <> "" Then
        StrSql = StrSql & " and SOURCE_CODE='" & TxtClientS.Text & "' "
    End If
    If txtTrNo.Text <> "" Then
        StrSql = StrSql & " and Tran_CODE='" & txtTrNo.Text & "'  "
    End If
    '---------------------------ANA code, Cheque, Application Search-------------------------------------------
    If Trim(TxtANACode.Text) <> "" Then
        StrSql = StrSql & " and source_CODE=(select agent_code from agent_master where upper(exist_code)=upper('" & Trim(TxtANACode.Text) & "'))  "
    End If
    If Trim(TxtChequeno1.Text) <> "" Then
        StrSql = StrSql & " and cheque_no='" & Trim(TxtChequeno1.Text) & "'"
    End If
    If Trim(TxtAppno.Text) <> "" Then
        StrSql = StrSql & " and app_no='" & Trim(TxtAppno.Text) & "'"
    End If
    '---------------------------------MASTER SIP SEARCH------------------------------------------------------------
    If Txtinstallments1.Text <> "" Then
        StrSql = StrSql & " and INSTALLMENTS_NO='" & Txtinstallments1.Text & "' "
    End If
    If CmbSipStpM.Text <> "" Then
       StrSql = StrSql & " and sip_type='" & CmbSipStpM.Text & "' "
    End If
    If Label42.Caption <> "" Then
        StrSql = StrSql & " and client_code='" & Label42.Caption & "' "
    End If
    If TxtAcHolderM.Text <> "" Then
        StrSql = StrSql & " and AC_HOLDER_CODE='" & TxtAcHolderM.Text & "' "
    End If
    If TxtBusiCodeM.Text <> "" Then
        StrSql = StrSql & " and BUSINESS_RMCODE='" & TxtBusiCodeM.Text & "' "
    End If
    If TxtPanM.Text <> "" Then
        StrSql = StrSql & " and PANNO='" & TxtPanM.Text & "' "
    End If
    If CmbFrequency1.ListIndex <> -1 Then
        MyFreq = Split(CmbFrequency1.Text, "#")
        StrSql = StrSql & " and frequency=" & MyFreq(1) & " "
    End If
    '--------------------------------------------------------------------------------------------------------------
    StrSql = StrSql & " order by  "
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_DATE,rm_name "
    Else
        StrSql = StrSql & "" & MyOrder
    End If
    If Option1(1).Value = True Then
        StrSql = StrSql & " Desc"
    End If
End If
Set rsMap = New ADODB.Recordset
rsMap.open StrSql, MyConn, adOpenDynamic, adLockOptimistic
i = 1
VSFCommGrdA.Clear
VSFCommGrdM.Clear
Call SetGrid
VSFCommGrdM.Rows = rsMap.RecordCount + 1
While Not rsMap.EOF
    If X = 1 Then
        VSFCommGrdA.TextMatrix(i, 0) = IIf(IsNull(rsMap("Investor_Name")), "", rsMap("Investor_Name"))
        VSFCommGrdA.TextMatrix(i, 1) = rsMap("Branch_Name")
        VSFCommGrdA.TextMatrix(i, 2) = IIf(IsNull(rsMap("PANNO")), "", rsMap("PANNO"))
        VSFCommGrdA.TextMatrix(i, 3) = rsMap("Amc_Name")
        VSFCommGrdA.TextMatrix(i, 4) = rsMap("Scheme_Name")
        VSFCommGrdA.TextMatrix(i, 5) = rsMap("TR_DATE")
        VSFCommGrdA.TextMatrix(i, 6) = IIf(IsNull(rsMap("TRAN_TYPE")), "", rsMap("TRAN_TYPE"))
        VSFCommGrdA.TextMatrix(i, 7) = IIf(IsNull(rsMap("App_No")), "", rsMap("App_No"))
        VSFCommGrdA.TextMatrix(i, 8) = IIf(IsNull(rsMap("PAYMENT_MODE")), "", rsMap("PAYMENT_MODE"))
        VSFCommGrdA.TextMatrix(i, 9) = IIf(IsNull(rsMap("CHEQUE_NO")), 0, rsMap("CHEQUE_NO"))
        VSFCommGrdA.TextMatrix(i, 10) = IIf(IsNull(rsMap("CHEQUE_DATE")), "", rsMap("CHEQUE_DATE"))
        VSFCommGrdA.TextMatrix(i, 11) = rsMap("Amount")
        VSFCommGrdA.TextMatrix(i, 12) = rsMap("Sip_TYPE")
        VSFCommGrdA.TextMatrix(i, 13) = IIf(IsNull(rsMap("lEAD_NO")), 0, rsMap("lEAD_nO"))
        VSFCommGrdA.TextMatrix(i, 14) = IIf(IsNull(rsMap("LEAD_nAME")), "", rsMap("LEAD_nAME"))
        VSFCommGrdA.TextMatrix(i, 15) = IIf(IsNull(rsMap("TRAN_code")), "", rsMap("TRAN_code"))
        VSFCommGrdA.TextMatrix(i, 16) = rsMap("branch_code")
        VSFCommGrdA.TextMatrix(i, 17) = rsMap("BUSINESS_RMCODE")
        VSFCommGrdA.TextMatrix(i, 18) = rsMap("SCH_CODE")
        VSFCommGrdA.TextMatrix(i, 19) = rsMap("Rm_NAme")
    Else
        VSFCommGrdM.TextMatrix(i, 0) = IIf(IsNull(rsMap("Investor_Name")), "", rsMap("Investor_Name"))
        VSFCommGrdM.TextMatrix(i, 1) = rsMap("Branch_Name")
        VSFCommGrdM.TextMatrix(i, 2) = IIf(IsNull(rsMap("PANNO")), "", rsMap("PANNO"))
        VSFCommGrdM.TextMatrix(i, 3) = rsMap("Amc_Name")
        VSFCommGrdM.TextMatrix(i, 4) = rsMap("Scheme_Name")
        VSFCommGrdM.TextMatrix(i, 5) = rsMap("TR_DATE")
        VSFCommGrdM.TextMatrix(i, 6) = IIf(IsNull(rsMap("TRAN_TYPE")), "", rsMap("TRAN_TYPE"))
        VSFCommGrdM.TextMatrix(i, 7) = IIf(IsNull(rsMap("App_No")), "", rsMap("App_No"))
        VSFCommGrdM.TextMatrix(i, 8) = IIf(IsNull(rsMap("PAYMENT_MODE")), "", rsMap("PAYMENT_MODE"))
        VSFCommGrdM.TextMatrix(i, 9) = IIf(IsNull(rsMap("CHEQUE_NO")), 0, rsMap("CHEQUE_NO"))
        VSFCommGrdM.TextMatrix(i, 10) = IIf(IsNull(rsMap("CHEQUE_DATE")), "", rsMap("CHEQUE_DATE"))
        VSFCommGrdM.TextMatrix(i, 11) = rsMap("Amount")
        VSFCommGrdM.TextMatrix(i, 12) = IIf(IsNull(rsMap("sip_TYPE")), "", rsMap("sip_TYPE"))
        VSFCommGrdM.TextMatrix(i, 13) = IIf(IsNull(rsMap("lEAD_nO")), 0, rsMap("lEAD_nO"))
        VSFCommGrdM.TextMatrix(i, 14) = IIf(IsNull(rsMap("LEAD_nAME")), "", rsMap("LEAD_nAME"))
        VSFCommGrdM.TextMatrix(i, 15) = IIf(IsNull(rsMap("TRAN_code")), "", rsMap("TRAN_code"))
        VSFCommGrdM.TextMatrix(i, 16) = rsMap("branch_code")
        VSFCommGrdM.TextMatrix(i, 17) = rsMap("BUSINESS_RMCODE")
        VSFCommGrdM.TextMatrix(i, 18) = rsMap("SCH_CODE")
        VSFCommGrdM.TextMatrix(i, 19) = rsMap("SOURCE_CODE")
        VSFCommGrdM.TextMatrix(i, 20) = IIf(IsNull(rsMap("Bank_Name")), "", rsMap("Bank_Name"))
        VSFCommGrdM.TextMatrix(i, 21) = IIf(IsNull(rsMap("Rm_Name")), "", rsMap("Rm_Name"))
        VSFCommGrdM.TextMatrix(i, 22) = IIf(IsNull(rsMap("folio_no")), "", rsMap("folio_no"))
        VSFCommGrdM.TextMatrix(i, 23) = IIf(IsNull(rsMap("doc_id")), "", rsMap("doc_id"))
        VSFCommGrdM.TextMatrix(i, 24) = IIf(IsNull(rsMap("micro_investment")), "", rsMap("micro_investment"))
        VSFCommGrdM.TextMatrix(i, 25) = IIf(IsNull(rsMap("target_switch_scheme")), "", rsMap("target_switch_scheme"))
        VSFCommGrdM.TextMatrix(i, 26) = IIf(IsNull(rsMap("target_scheme_name")), "", rsMap("target_scheme_name"))
        VSFCommGrdM.TextMatrix(i, 27) = IIf(IsNull(rsMap("switch_scheme_name")), "", rsMap("switch_scheme_name"))
        
    End If
    i = i + 1
    rsMap.MoveNext
    DoEvents
Wend
rsMap.Close
Set rsMap = Nothing
Exit Sub
err1:
    MsgBox err.Description
    'Resume
End Sub