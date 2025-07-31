If X = 1 Then
    StrSql = " SELECT   FOLIO_NO, SCH_CODE, MAX(TR_DATE)TR_DATE, TRAN_TYPE, "
    StrSql = StrSql & "         MAX (INVESTOR_NAME) INVESTOR_NAME, MAX (ADDRESS) ADDRESS, "
    StrSql = StrSql & "         MAX (BROKER_CODE) BROKER_CODE, MAX (CITY_NAME) CITY_NAME, MAX(SCH_NAME)SCH_NAME, "
    StrSql = StrSql & "         MAX(MUT_CODE)MUT_CODE, MAX(MUT_NAME)MUT_NAME, MAX(RM_NAME)RM_NAME, MAX(BRANCH_NAME)BRANCH_NAME, MAX (APP_NO) APP_NO, "
    StrSql = StrSql & "         MAX (CHEQUE_NO) CHEQUE_NO, SUM (AMOUNT) AMOUNT, "
    StrSql = StrSql & "         MAX (BUSI_BRANCH_CODE) BUSI_BRANCH_CODE, "
    StrSql = StrSql & "         MAX (BUSINESS_RMCODE) BUSINESS_RMCODE,"
    
    If Optcams.Value = True Then
        StrSql = StrSql & " WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,TRAN_ID,MAX(UNQ_KEY),'CAMS') UNIQUE_TRAN"
    ElseIf OptKarvy = True Then
        StrSql = StrSql & " WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,MAX(TRAN_ID),MAX(UNQ_KEY),'KARVY') UNIQUE_TRAN"
    ElseIf OptKarvyCOB = True Then
        StrSql = StrSql & " WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,MAX(TR_DATE),TRAN_TYPE,MAX(TRAN_ID),UNQ_KEY,'KARVY COB') UNIQUE_TRAN"
    ElseIf OptCamsCOB.Value = True Then
        StrSql = StrSql & " WEALTHMAKER.GETTRANCODE_ALL(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,TRAN_ID,MAX(UNQ_KEY),'CAMS') UNIQUE_TRAN"
    End If
    
    StrSql = StrSql & " ,MAX(REG_TRANTYPE) REG_TRANTYPE,max(unq_key) unq_key"
    StrSql = StrSql & "   FROM ( "
    StrSql = StrSql & " SELECT t.tran_id,tran_type, t.inv_name Investor_Name,(i.ADDRESS1||','||i.ADDRESS2||','||i.EMAIL) address, "
    StrSql = StrSql & "         REG_SUBBROK broker_code,c.city_name, "
    StrSql = StrSql & "         t.sch_code,sch_name sch_name,t.mut_code,amc.mut_name mut_name, rm_name,  "
    StrSql = StrSql & "         branch_name,  "
    StrSql = StrSql & "         app_no,folio_no, cheque_no, "
    If OptKarvyCOB = True Then
        StrSql = StrSql & "    WEALTHMAKER.FN_GET_COB_CMV@MF.BAJAJCAPITAL(T.FOLIO_NO,T.SCH_CODE)     amount,  "
    Else
        StrSql = StrSql & "         amount,  "
    End If
    StrSql = StrSql & " t.tran_code,reg_date as tr_date, "
    StrSql = StrSql & "         busi_branch_code, business_rmcode,t.REG_TRANTYPE,t.unq_key unq_key"
    StrSql = StrSql & "    FROM employee_master e, "
    StrSql = StrSql & "         branch_master b, "
    StrSql = StrSql & "         mut_fund amc, "
    StrSql = StrSql & "         scheme_info sch, "
    StrSql = StrSql & "         TRANSACTION_ST@MF.BAJAJCAPITAL t, "
    StrSql = StrSql & "         investor_master i,CITY_MASTER C "
    StrSql = StrSql & "   WHERE I.CITY_ID=C.CITY_ID(+) "
    StrSql = StrSql & "     AND t.client_code = i.inv_code "
    StrSql = StrSql & "     AND to_char(t.rmcode) = e.rm_code "
    StrSql = StrSql & "     AND t. BRANCH_CODE = b.branch_code   "
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
    If txtamount.Text <> "" Then
        StrSql = StrSql & " and abs(round(t.amount)) = " & Abs(Round(Val(txtamount.Text))) & ""
    End If
    StrSql = StrSql & "  AND LPAD (t.mut_code, 2) = 'MF' "
    StrSql = StrSql & "     AND (t.asa <> 'C' OR t.asa IS NULL) "
    StrSql = StrSql & "                                    AND t.tran_type IN "
    StrSql = StrSql & "                                           ('PURCHASE', 'REINVESTMENT', "
    StrSql = StrSql & "                                            'SWITCH IN') "
    StrSql = StrSql & "ORDER BY "
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_Date,rm_name "
    End If
    StrSql = StrSql & " ) "
    If Optcams.Value = True Or OptCamsCOB.Value = True Then
        StrSql = StrSql & "GROUP BY FOLIO_NO, "
        StrSql = StrSql & "         SCH_CODE, "
        StrSql = StrSql & "         TR_DATE, "
        StrSql = StrSql & "         TRAN_TYPE,TRAN_ID "
    ElseIf OptKarvy = True Then
        StrSql = StrSql & "GROUP BY FOLIO_NO, "
        StrSql = StrSql & "         SCH_CODE, "
        StrSql = StrSql & "         TR_DATE, "
        StrSql = StrSql & "         TRAN_TYPE "
    ElseIf OptKarvyCOB = True Then
        StrSql = StrSql & "GROUP BY FOLIO_NO, "
        StrSql = StrSql & "         SCH_CODE, "
        StrSql = StrSql & "         TRAN_TYPE,UNQ_KEY"
    End If
    Set rsMap = New ADODB.Recordset
    rsMap.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
