    While Not rsMap.EOF

    
        StrSql = ""
        StrSql = " SELECT tran_code"
        StrSql = StrSql & "    FROM employee_master e, "
        StrSql = StrSql & "         branch_master b, "
        StrSql = StrSql & "         mut_fund amc, "
        StrSql = StrSql & "         scheme_info sch, "
        StrSql = StrSql & "         TRANSACTION_ST@MF.BAJAJCAPITAL t, "
        StrSql = StrSql & "         investor_master i,CITY_MASTER C "
        StrSql = StrSql & "   WHERE I.CITY_ID=C.CITY_ID(+) "
        StrSql = StrSql & "     AND t.client_code = i.inv_code "
        StrSql = StrSql & "     AND to_char(t.rmcode) = e.rm_code "
        StrSql = StrSql & "     AND t.BRANCH_CODE = b.branch_code   "
        StrSql = StrSql & "     AND t.mut_code = amc.mut_code AND t.sch_code = sch.sch_code "
        
        If Optcams.Value = True Then
            StrSql = StrSql & " AND  (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE='TICOB' AND DUP_FLAG2 IN(0,9))) "
        ElseIf OptKarvy.Value = True Then
            StrSql = StrSql & " AND  (DUP_FLAG2 = 0 OR (REG_TRAN_FLAG='TI' AND DUP_FLAG2 IN(0,9))) "
        End If
        
        If cmbbranchAr.Text <> "" And cmbbranchAr.Text <> "ALL" And MyBranchCode <> "" Then
             StrSql = StrSql & "      and b.BRANCH_CODE in (" & MyBranchCode & ") "
        Else
             StrSql = StrSql & "      and b.BRANCH_CODE in (" & Branches & ") "
        End If
        
        If SRmCode <> "" Then
             StrSql = StrSql & " and e.rm_code=" & SRmCode & ""
        End If
        
        If CmbAmcAR.Text <> "" And CmbAmcAR.Text <> "ALL" And MyMutCodeAr <> "" Then
            StrSql = StrSql & " and to_char(t.mut_code) ='" & MyMutCodeAr & "'"
        End If
        
        If OptReconcileAR.Value = True Then
            StrSql = StrSql & " and t.rec_flag ='Y'"
        ElseIf OptUnReconcileAR.Value = True Then
            StrSql = StrSql & " and (t.rec_flag ='N' or rec_flag is null)"
        End If
        
        
        
        
        If Optregular_st.Value = True Then
            StrSql = StrSql & " AND ((    UPPER (t.REG_TRANTYPE) NOT LIKE '%SYS%' AND UPPER (t.REG_TRANTYPE) NOT LIKE '%SIP%' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE '%S T P IN%' "
            StrSql = StrSql & " AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE '%S T P IN REJ%'"
            StrSql = StrSql & " AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE '%S T P IN REJ REVERSAL%'"
            StrSql = StrSql & " AND TRIM(UPPER (t.REG_TRANTYPE))NOT LIKE '%STP SWITCH IN%'"
            StrSql = StrSql & " AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE '%STPI%'"
            StrSql = StrSql & " AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE '%STPIR%'"
            StrSql = StrSql & " ) OR t.REG_TRANTYPE IS NULL) "
        End If
        If OptSip_St.Value = True Then
            StrSql = StrSql & " AND (( UPPER (t.REG_TRANTYPE) LIKE '%SYS%'"
            StrSql = StrSql & " OR UPPER (t.REG_TRANTYPE) LIKE '%SIP%'"
            StrSql = StrSql & " OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE '%S T P IN%'"
            StrSql = StrSql & " OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE '%S T P IN REJ%'"
            StrSql = StrSql & " OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE '%S T P IN REJ REVERSAL%'"
            StrSql = StrSql & " OR TRIM(UPPER (t.REG_TRANTYPE)) LIKE '%STP SWITCH IN%'"
            StrSql = StrSql & " OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE '%STPI%'"
            StrSql = StrSql & " OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE '%STPIR%')) "
        End If
        

        If OptKarvyCOB.Value = False Then
            If mskfromdt.Text <> "__/__/____" Then
                StrSql = StrSql & " and tr_date>=TO_DATE('" & mskfromdt & "','DD/MM/YYYY') "
            End If
            If msktodt.Text <> "__/__/____" Then
                StrSql = StrSql & " and tr_date<=TO_DATE('" & msktodt & "','DD/MM/YYYY') "
            End If
        End If
        
        If CmbSearch.ListIndex = 0 And TxtSearch <> "" Then
            StrSql = StrSql & " and ( t.CHEQUE_NO = '" & UCase(TxtSearch.Text) & "' or  t.CHEQUE_NO = '" & Val(TxtSearch.Text) & "' ) "
        ElseIf CmbSearch.ListIndex = 1 And TxtSearch <> "" Then
            StrSql = StrSql & " and t.folio_no = '" & UCase(TxtSearch.Text) & "'"
        ElseIf CmbSearch.ListIndex = 2 And TxtSearch <> "" Then
            StrSql = StrSql & " and t.app_no = '" & UCase(TxtSearch.Text) & "'"
        ElseIf CmbSearch.ListIndex = 4 And TxtSearch <> "" Then
            StrSql = StrSql & " and t.REG_SUBBROK = '" & UCase(TxtSearch.Text) & "'"
        ElseIf CmbSearch.ListIndex = 3 And TxtSearch <> "" Then
            StrSql = StrSql & " and (upper(t.pan1) = '" & UCase(TxtSearch.Text) & "' or upper(t.pan2) = '" & UCase(TxtSearch.Text) & "' or upper(t.pan3) = '" & UCase(TxtSearch.Text) & "')"
        End If
        
        If TxtInvestorName.Text <> "" Then
            StrSql = StrSql & " and upper(trim(T.inv_name)) like '%" & Replace(UCase(TxtInvestorName.Text), " ", "%") & "%'"
        End If
        
        If TxtAmount.Text <> "" Then
            StrSql = StrSql & " and abs(round(t.amount)) = " & Abs(Round(Val(TxtAmount.Text))) & ""
        End If
        
        StrSql = StrSql & "  AND LPAD (t.mut_code, 2) = 'MF' "
        StrSql = StrSql & "     AND (t.asa <> 'C' OR t.asa IS NULL) "
        StrSql = StrSql & "                                    AND t.tran_type IN "
        StrSql = StrSql & "                                           ('PURCHASE', 'REINVESTMENT', "
        StrSql = StrSql & "                                            'SWITCH IN') "
        StrSql = StrSql & " and t.sch_code='" & rsMap.Fields("sch_code") & "' and t.FOLIO_NO='" & rsMap.Fields("FOLIO_NO") & "' "
        If OptKarvyCOB.Value = False Then
            StrSql = StrSql & " AND T.TR_DATE=TO_DATE('" & rsMap.Fields("TR_DATE") & "','DD/MM/YYYY')"
        End If
        
        Set rsMapNew = New ADODB.Recordset
        rsMapNew.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
        strTranCode = ""
        While Not rsMapNew.EOF
            strTranCode = strTranCode & "'" & rsMapNew(0) & "',"
            rsMapNew.MoveNext
        Wend
        If Len(strTranCode) > 1 Then strTranCode = Left(strTranCode, Len(strTranCode) - 1)
        rsMapNew.Close
        Set rsMapNew = Nothing
        VSFCommGrdK.TextMatrix(i, 0) = strTranCode 'rsMap("UNIQUE_TRAN")
        VSFCommGrdK.TextMatrix(i, 1) = rsMap("tr_date")
        VSFCommGrdK.TextMatrix(i, 2) = IIf(IsNull(rsMap("Investor_Name")), "", rsMap("Investor_Name"))
        VSFCommGrdK.TextMatrix(i, 3) = IIf(IsNull(rsMap("Address")), "", rsMap("address"))
        VSFCommGrdK.TextMatrix(i, 4) = IIf(IsNull(rsMap("City_Name")), "", rsMap("City_Name"))
        VSFCommGrdK.TextMatrix(i, 5) = IIf(IsNull(rsMap("Mut_Name")), "", rsMap("Mut_Name"))
        VSFCommGrdK.TextMatrix(i, 6) = IIf(IsNull(rsMap("Sch_Name")), "", rsMap("Sch_Name"))
        VSFCommGrdK.TextMatrix(i, 7) = IIf(IsNull(rsMap("AMOUNT")), 0, rsMap("AMOUNT"))
        VSFCommGrdK.TextMatrix(i, 8) = IIf(IsNull(rsMap("Folio_no")), "", rsMap("folio_no"))
        VSFCommGrdK.TextMatrix(i, 9) = IIf(IsNull(rsMap("cheque_no")), 0, rsMap("cheque_no"))
        VSFCommGrdK.TextMatrix(i, 10) = IIf(IsNull(rsMap("app_no")), 0, rsMap("app_no"))
        VSFCommGrdK.TextMatrix(i, 11) = IIf(IsNull(rsMap("Rm_Name")), "", rsMap("Rm_Name"))
        VSFCommGrdK.TextMatrix(i, 12) = IIf(IsNull(rsMap("Branch_Name")), "", rsMap("Branch_Name"))
        VSFCommGrdK.TextMatrix(i, 13) = IIf(IsNull(rsMap("broker_Code")), "", rsMap("Broker_Code"))
        VSFCommGrdK.TextMatrix(i, 14) = IIf(IsNull(rsMap("mut_code")), 0, rsMap("mut_code"))
        VSFCommGrdK.TextMatrix(i, 15) = IIf(IsNull(rsMap("sch_code")), 0, rsMap("sch_code"))
        VSFCommGrdK.TextMatrix(i, 16) = IIf(IsNull(rsMap("reg_trantype")), 0, rsMap("reg_trantype"))
        VSFCommGrdK.TextMatrix(i, 17) = IIf(IsNull(rsMap("unq_key")), 0, rsMap("unq_key"))
        i = i + 1
        rsMap.MoveNext
     Wend
     rsMap.Close
     Set rsMap = Nothing
     
      'If MySelectedRow1 > 0 Then
          'VSFCommGrdT.Row = MySelectedRow1
          For i = 1 To VSFCommGrdK.Rows - 1
            For KCount_cOL = 1 To VSFCommGrdK.Cols - 2
                VSFCommGrdK.Row = i
                VSFCommGrdK.Col = KCount_cOL
                If KCount_cOL = 7 Then
                    If Abs(VSFCommGrdK.TextMatrix(i, KCount_cOL) - VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, KCount_cOL)) <= 5 Then
                        VSFCommGrdK.CellBackColor = vbGreen
                    Else
                        VSFCommGrdK.CellBackColor = vbCyan
                    End If
                Else
                    If KCount_cOL < 13 Then
                        If Trim(UCase(VSFCommGrdK.TextMatrix(i, KCount_cOL))) = Trim(UCase(VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, KCount_cOL))) Then
                            VSFCommGrdK.CellBackColor = vbGreen
                        Else
                            VSFCommGrdK.CellBackColor = vbCyan
                        End If
                    Else
                        VSFCommGrdK.Row = i
                        VSFCommGrdK.Col = KCount_cOL + 1
                        If Trim(UCase(VSFCommGrdK.TextMatrix(i, KCount_cOL + 1))) = Trim(UCase(VSFCommGrdT.TextMatrix(VSFCommGrdT.Row, KCount_cOL))) Then
                            VSFCommGrdK.CellBackColor = vbGreen
                        Else
                            VSFCommGrdK.CellBackColor = vbCyan
                        End If
                    End If
                End If
            Next
          Next
          
          For i = 1 To VSFCommGrdK.Rows - 1
                VSFCommGrdK.Row = i
                VSFCommGrdK.Col = 17
                VSFCommGrdK.CellBackColor = vbWhite
          Next
      'End If


