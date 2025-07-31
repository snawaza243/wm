Private Sub gridfill(X As Integer)
On Error GoTo err1
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
    If TxtAmount.Text <> "" Then
        StrSql = StrSql & " and abs(round(t.amount)) = " & Abs(Round(Val(TxtAmount.Text))) & ""
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
Else

    StrSql = " select inv.Investor_Name,inv.address1||','||inv.address2||','||inv.phone||','||inv.email address ,ct.city_name "
    StrSql = StrSql & " ,mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,"
    StrSql = StrSql & "Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.tran_type tran_type, mf.sip_type sip_type, CASE WHEN  mf.loggeduserid='MFONLINE' THEN 'Online' WHEN  mf.loggeduserid='Valuefy' THEN 'Online' ELSE 'Offline' end loggeduserid "
    StrSql = StrSql & " from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1 mf "
    StrSql = StrSql & " where  MF.MOVE_FLAG1 IS NULL AND MF.SIP_ID IS  NULL AND TRAN_TYPE NOT IN('REVERTAL') AND (MF.ASA<>'C' OR MF.ASA IS NULL) AND inv.city_id=ct.city_id(+) and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  "
    If cmbbranch.Text <> "" And cmbbranch.Text <> "ALL" And MyBranchCode <> "" Then
        StrSql = StrSql & " and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & MyBranchCode & ")"
    Else
        StrSql = StrSql & " and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & Branches & ")"
    End If
    If cmbcategory.Text <> "" And cmbcategory.Text <> "All" And MyBranchCat <> "" Then
         StrSql = StrSql & "      and b.branch_tar_cat='" & MyBranchCat & "' "
    End If
    If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" And MyRegionCode <> "" Then
        StrSql = StrSql & " and b.region_id ='" & MyRegionCode & "'"
    End If
    If CmbZone.Text <> "" And UCase(CmbZone.Text) <> "ALL" And MyZoneCode <> "" Then
        StrSql = StrSql & " and b.zone_id ='" & MyZoneCode & "'"
    End If
    If CmbRM.Text <> "" And UCase(CmbRM.Text) <> "ALL" And MyRmCode <> "" Then
        StrSql = StrSql & " and e.payroll_id ='" & MyRmCode & "'"
    End If
    If SRmCode <> "" Then
         StrSql = StrSql & " and e.rm_code=" & SRmCode & ""
    End If
    If OpTregulare.Value = True Then
        StrSql = StrSql & " AND (MF.SIP_TYPE='REGULAR' OR MF.SIP_TYPE IS NULL OR MF.SIP_TYPE ='STP')"
    End If
    If OptSip.Value = True Then
        StrSql = StrSql & " AND (MF.DISPATCH='N')"
    End If
    If Trim(txtAR.Text) <> "" Then
        StrSql = StrSql & " AND MF.tran_code='" & Trim(txtAR.Text) & "'"
    End If
    If optPMS.Value = True Then
        StrSql = StrSql & " AND ((sch.prd='DT027')) "
    End If
    If optATM.Value = True Then
        StrSql = StrSql & " AND NVL(ATM_FLAG,'0')='1' "
    End If
    If optTrail.Value = True Then
        StrSql = StrSql & " AND get_scheme_nature_trail_NEW(mf.sch_code,MF.TR_DATE)='T' and pms_identify(tran_code)='0' "
    End If
    
    If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
        StrSql = StrSql & " and mf.mut_code ='" & MyMutCode & "'"
    End If
    
    If OptReconcileTR.Value = True Then
        StrSql = StrSql & " and mf.rec_flag='Y'"
    ElseIf OptUnReconcileTr.Value = True Then
        StrSql = StrSql & " and (mf.rec_flag ='N' or mf.rec_flag is null)"
    ElseIf optPending.Value = True Then
        StrSql = StrSql & " and (mf.rec_flag ='N' or mf.rec_flag is null) and (processed is not null or remark_reco='REC0015')"
    End If
    
    If ChkCob.Value = 1 Then
        StrSql = StrSql & " and mf.cob_flag=1"
    End If
    
    If Trim(txtAR.Text) = "" Then
        If mskfromdt.Text <> "__/__/____" Then
            StrSql = StrSql & " and tr_date>=TO_DATE('" & mskfromdt1 & "','DD/MM/YYYY') "
        End If
        If msktodt.Text <> "__/__/____" Then
            StrSql = StrSql & " and tr_date<=TO_DATE('" & msktodt1 & "','DD/MM/YYYY') "
        End If
    End If
    
    StrSql = StrSql & " order by "
    If MyOrder1 = "" Then
        StrSql = StrSql & "  TR_Date,rm_name "
    Else
        StrSql = StrSql & "" & MyOrder1
    End If
    If Option1(3).Value = True Then
        StrSql = StrSql & " Desc"
    End If
    Set rsMap1 = New ADODB.Recordset
    rsMap1.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
End If
i = 1
j = 1
If X = 1 Then
    VSFCommGrdK.Clear
ElseIf X = 2 Then
    VSFCommGrdT.Clear
    VSFCommGrdK.Clear
End If
Call SetGrid
If X = 1 Then
    VSFCommGrdK.Rows = rsMap.RecordCount + 1
    If rsMap.EOF Then
        MsgBox "No record found corresponding to the searching criteria", vbInformation
        Exit Sub
    End If
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
Else
     VSFCommGrdT.Rows = rsMap1.RecordCount + 1
     LblCount = rsMap1.RecordCount
     While Not rsMap1.EOF
        VSFCommGrdT.TextMatrix(j, 0) = rsMap1("TRAN_code")
        VSFCommGrdT.TextMatrix(j, 1) = rsMap1("tr_date")
        VSFCommGrdT.TextMatrix(j, 2) = IIf(IsNull(rsMap1("Investor_Name")), "", rsMap1("Investor_Name"))
        VSFCommGrdT.TextMatrix(j, 3) = IIf(IsNull(rsMap1("Address")), "", rsMap1("address"))
        VSFCommGrdT.TextMatrix(j, 4) = IIf(IsNull(rsMap1("City_Name")), "", rsMap1("City_Name"))
        VSFCommGrdT.TextMatrix(j, 5) = IIf(IsNull(rsMap1("Mut_Name")), "", rsMap1("Mut_Name"))
        VSFCommGrdT.TextMatrix(j, 6) = IIf(IsNull(rsMap1("Sch_Name")), "", rsMap1("Sch_Name"))
        VSFCommGrdT.TextMatrix(j, 7) = IIf(IsNull(rsMap1("Amount")), 0, rsMap1("Amount"))
        VSFCommGrdT.TextMatrix(j, 8) = IIf(IsNull(rsMap1("Folio_no")), "", rsMap1("folio_no"))
        VSFCommGrdT.TextMatrix(j, 9) = IIf(IsNull(rsMap1("cheque_no")), 0, rsMap1("cheque_no"))
        VSFCommGrdT.TextMatrix(j, 10) = IIf(IsNull(rsMap1("app_no")), 0, rsMap1("app_no"))
        VSFCommGrdT.TextMatrix(j, 11) = IIf(IsNull(rsMap1("Rm_Name")), "", rsMap1("Rm_Name"))
        VSFCommGrdT.TextMatrix(j, 12) = IIf(IsNull(rsMap1("Branch_Name")), "", rsMap1("Branch_Name"))
        VSFCommGrdT.TextMatrix(j, 13) = IIf(IsNull(rsMap1("mut_code")), 0, rsMap1("mut_code"))
        VSFCommGrdT.TextMatrix(j, 14) = IIf(IsNull(rsMap1("sch_code")), 0, rsMap1("sch_code"))
        VSFCommGrdT.TextMatrix(j, 15) = IIf(IsNull(rsMap1("tran_type")), 0, rsMap1("tran_type"))
        VSFCommGrdT.TextMatrix(j, 16) = IIf(IsNull(rsMap1("sip_type")), 0, rsMap1("sip_type"))
        VSFCommGrdT.TextMatrix(j, 17) = IIf(IsNull(rsMap1("loggeduserid")), 0, rsMap1("loggeduserid"))
        
'        'FOR MISSING FLAG
'        VSFCommGrdT.Row = i
'        VSFCommGrdT.Col = 18
'        VSFCommGrdT.CellForeColor = vbWhite
'        Set VSFCommGrdT.CellPicture = Image2.Picture
'        If rsMap1.Fields("missing") <> "" Then
'            Set VSFCommGrdT.CellPicture = Image1.Picture
'            VSFCommGrdT.CellPictureAlignment = 4
'            VSFCommGrdT.Text = 1
'        Else
'            Set VSFCommGrdT.CellPicture = Image2.Picture
'            VSFCommGrdT.CellPictureAlignment = 4
'            VSFCommGrdT.Text = 0
'        End If
'        VSFCommGrdT.CellPictureAlignment = 4
'        '-----------------------------------FOR SOLUTION FLAG-----------------------------
'        VSFCommGrdT.Row = i
'        VSFCommGrdT.Col = 19
'        VSFCommGrdT.CellForeColor = vbWhite
'        Set VSFCommGrdT.CellPicture = Image2.Picture
'        If rsMap1.Fields("SOLVE") <> "" Then
'            VSFCommGrdT.Text = 1
'            Set VSFCommGrdT.CellPicture = Image1.Picture
'            VSFCommGrdT.CellPictureAlignment = 4
'        Else
'            VSFCommGrdT.Text = 0
'            Set VSFCommGrdT.CellPicture = Image2.Picture
'            VSFCommGrdT.CellPictureAlignment = 4
'        End If
'        VSFCommGrdT.CellPictureAlignment = 4
'        '--------------------------------------------------------------------------------
'        VSFCommGrdT.TextMatrix(i, 20) = IIf(IsNull(rsMap1("SOL_REF_AR")), "", rsMap1("SOL_REF_AR"))
'        On Error Resume Next
'        VSFCommGrdT.TextMatrix(i, 21) = rsMap1("SOL_DATE")
'        MyTotalAmount2 = MyTotalAmount2 + rsMap1("Amount")
'        MyTotalmargin2 = MyTotalmargin2 + Val(VSFCommGrdT.TextMatrix(i, 17))
'        MyTotalTrans2 = MyTotalTrans2 + i
        j = j + 1
        rsMap1.MoveNext
     Wend
   
'     LblAmount2.Caption = Round(MyTotalAmount2, 0)
'     LblMargin2.Caption = Round(MyTotalmargin2, 0)
'     LblTrans2.Caption = Round(MyTotalTrans2, 0)
     rsMap1.Close
     Set rsMap1 = Nothing
End If
DoEvents
Exit Sub
err1:
    MsgBox err.Description
    Resume
End Sub
