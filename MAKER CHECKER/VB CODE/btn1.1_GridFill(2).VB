Private Sub gridfill(X As Integer)
On Error GoTo err1
If X = 1 Then
    StrSql = " SELECT   FOLIO_NO, SCH_CODE, TR_DATE, TRAN_TYPE, "
    StrSql = StrSql & "         MAX (inv_name) Inv_Name, MAX (ADDRESS) ADDRESS, "
    StrSql = StrSql & "         MAX (BROKER_CODE) BROKER_CODE, MAX (CITY_NAME) CITY_NAME, max(sch_name)SCH_NAME, "
    StrSql = StrSql & "         MAX(MUT_CODE)MUT_CODE, MAX(MUT_NAME)MUT_NAME, MAX(RM_NAME)RM_NAME, MAX(BRANCH_NAME)BRANCH_NAME, MAX (APP_NO) APP_NO, "
    StrSql = StrSql & "         MAX (CHEQUE_NO) CHEQUE_NO, SUM (AMOUNT) AMOUNT, "
    StrSql = StrSql & "         MAX (BUSI_BRANCH_CODE) BUSI_BRANCH_CODE, "
    StrSql = StrSql & "         MAX (BUSINESS_RMCODE) BUSINESS_RMCODE,GETTRANCODE(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE)UNIQUE_TRAN "
    StrSql = StrSql & "   FROM ( "
    StrSql = StrSql & " SELECT  tran_type,t.inv_name,(i.ADDRESS1||','||i.ADDRESS2||','||i.EMAIL) address, "
    StrSql = StrSql & "         REG_SUBBROK broker_code,c.city_name, "
    StrSql = StrSql & "         t.sch_code,sch_name sch_name,t.mut_code,amc.mut_name mut_name, rm_name,  "
    StrSql = StrSql & "         branch_name,  "
    StrSql = StrSql & "         app_no,folio_no, cheque_no, "
    StrSql = StrSql & "         amount,  t.tran_code,reg_date as tr_date, "
    StrSql = StrSql & "         busi_branch_code, business_rmcode "
    StrSql = StrSql & "    FROM employee_master e, "
    StrSql = StrSql & "         branch_master b, "
    StrSql = StrSql & "         mut_fund amc, "
    StrSql = StrSql & "         scheme_info sch, "
    StrSql = StrSql & "         TRANSACTION_ST@MF.BAJAJCAPITAL t, "
    StrSql = StrSql & "         investor_master i,CITY_MASTER C "
    StrSql = StrSql & "   WHERE I.CITY_ID=C.CITY_ID(+)"
    StrSql = StrSql & "     AND t.client_code = i.inv_code "
    StrSql = StrSql & "     AND to_char(t.rmcode) = e.rm_code "
    StrSql = StrSql & "     AND t.BRANCH_CODE = b.branch_code   "
    StrSql = StrSql & "     AND t.mut_code = amc.mut_code AND t.sch_code = sch.sch_code AND (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE='TICOB' AND DUP_FLAG2 IN(0,9))) "
    
    
    
    If SRmCode <> "" Then
         StrSql = StrSql & " and e.rm_code=" & SRmCode & ""
    End If
    
    If MyTranCode <> "" Then
       StrSql = StrSql & " and t.tran_code IN(" & MyTempTrCode & ")"
    End If
    
    'StrSql = StrSql & "  AND LPAD (t.mut_code, 2) = 'MF' "
    
    StrSql = StrSql & "     AND (t.asa <> 'C' OR t.asa IS NULL) "
    StrSql = StrSql & "                                    AND t.tran_type IN "
    StrSql = StrSql & "                                           ('PURCHASE', 'REINVESTMENT', "
    StrSql = StrSql & "                                            'SWITCH IN') "
    StrSql = StrSql & "ORDER BY "
    If MyOrder = "" Then
        StrSql = StrSql & "  TR_Date,rm_name "
    End If
    StrSql = StrSql & " ) "
    StrSql = StrSql & "GROUP BY FOLIO_NO, "
    StrSql = StrSql & "         SCH_CODE, "
    StrSql = StrSql & "         TR_DATE, "
    StrSql = StrSql & "         TRAN_TYPE "

    
    Set rsMap = New ADODB.Recordset
    rsMap.open StrSql, MyConn, adOpenForwardOnly, adLockOptimistic
Else
    StrSql = " select inv.Investor_Name,inv.address1||','||inv.address2||','||inv.phone||','||inv.email address ,ct.city_name "
    StrSql = StrSql & " ,mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,"
    StrSql = StrSql & "Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE"
    StrSql = StrSql & " from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1 mf "
    StrSql = StrSql & " where  MF.MOVE_FLAG1 IS NULL AND MF.SIP_ID IS  NULL AND inv.city_id=ct.city_id and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  "
    
    If cmbbranch.Text <> "" And cmbbranch.Text <> "ALL" And MyBranchCode <> "" Then
        StrSql = StrSql & " and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & MyBranchCode & ")"
    Else
        StrSql = StrSql & " and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in (" & Branches & ")"
    End If
    If CmbRegion.Text <> "" And UCase(CmbRegion.Text) <> "ALL" And MyRegionCode <> "" Then
        StrSql = StrSql & " and b.region_id ='" & MyRegionCode & "'"
    End If

    If TxtAR.Text <> "" Then
        StrSql = StrSql & " and MF.TRAN_CODE ='" & TxtAR.Text & "'"
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

    If CmbAmcTR.Text <> "" And UCase(CmbAmcTR.Text) <> "ALL" Then
        StrSql = StrSql & " and mf.mut_code ='" & MyMutCode & "'"
    End If

    If OptManulaRecon.Value = True Then
        StrSql = StrSql & " and mf.rec_flag='Y' and mf.rec_user<>'Auto'"
    ElseIf OptAutoRecon.Value = True Then
        StrSql = StrSql & " and mf.rec_user='Auto' and mf.rec_flag='Y' "
    End If
    
    StrSql = StrSql & " and mf.audit_flag is null "
    
    If mskfromdt.Text <> "__/__/____" Then
        StrSql = StrSql & " and to_date(reco_date)>=TO_DATE('" & mskfromdt1 & "','DD/MM/YYYY') "
    End If
    If msktodt.Text <> "__/__/____" Then
        StrSql = StrSql & " and to_date(reco_date)<=TO_DATE('" & msktodt1 & "','DD/MM/YYYY') "
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
        VSFCommGrdK.TextMatrix(i, 0) = rsMap("UNIQUE_TRAN")
        VSFCommGrdK.TextMatrix(i, 1) = rsMap("tr_date")
        VSFCommGrdK.TextMatrix(i, 2) = IIf(IsNull(rsMap("Inv_Name")), "", rsMap("inv_name"))
        VSFCommGrdK.TextMatrix(i, 3) = IIf(IsNull(rsMap("Address")), "", rsMap("address"))
        VSFCommGrdK.TextMatrix(i, 4) = IIf(IsNull(rsMap("City_Name")), "", rsMap("City_Name"))
        VSFCommGrdK.TextMatrix(i, 5) = IIf(IsNull(rsMap("Mut_Name")), "", rsMap("Mut_Name"))
        VSFCommGrdK.TextMatrix(i, 6) = IIf(IsNull(rsMap("Sch_Name")), "", rsMap("Sch_Name"))
        VSFCommGrdK.TextMatrix(i, 7) = rsMap("Amount")
        VSFCommGrdK.TextMatrix(i, 8) = IIf(IsNull(rsMap("Folio_no")), "", rsMap("folio_no"))
        VSFCommGrdK.TextMatrix(i, 9) = IIf(IsNull(rsMap("cheque_no")), 0, rsMap("cheque_no"))
        VSFCommGrdK.TextMatrix(i, 10) = IIf(IsNull(rsMap("app_no")), 0, rsMap("app_no"))
        VSFCommGrdK.TextMatrix(i, 11) = IIf(IsNull(rsMap("Rm_Name")), "", rsMap("Rm_Name"))
        VSFCommGrdK.TextMatrix(i, 12) = IIf(IsNull(rsMap("Branch_Name")), "", rsMap("Branch_Name"))
        VSFCommGrdK.TextMatrix(i, 13) = IIf(IsNull(rsMap("broker_Code")), "", rsMap("Broker_Code"))
        VSFCommGrdK.TextMatrix(i, 14) = IIf(IsNull(rsMap("mut_code")), 0, rsMap("mut_code"))
        VSFCommGrdK.TextMatrix(i, 15) = IIf(IsNull(rsMap("sch_code")), 0, rsMap("sch_code"))
        i = i + 1
        rsMap.MoveNext
     Wend
     rsMap.Close
     Set rsMap = Nothing
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
        VSFCommGrdT.TextMatrix(j, 7) = rsMap1("Amount")
        VSFCommGrdT.TextMatrix(j, 8) = IIf(IsNull(rsMap1("Folio_no")), "", rsMap1("folio_no"))
        VSFCommGrdT.TextMatrix(j, 9) = IIf(IsNull(rsMap1("cheque_no")), 0, rsMap1("cheque_no"))
        VSFCommGrdT.TextMatrix(j, 10) = IIf(IsNull(rsMap1("app_no")), 0, rsMap1("app_no"))
        VSFCommGrdT.TextMatrix(j, 11) = IIf(IsNull(rsMap1("Rm_Name")), "", rsMap1("Rm_Name"))
        VSFCommGrdT.TextMatrix(j, 12) = IIf(IsNull(rsMap1("Branch_Name")), "", rsMap1("Branch_Name"))
        VSFCommGrdT.TextMatrix(j, 13) = IIf(IsNull(rsMap1("mut_code")), 0, rsMap1("mut_code"))
        VSFCommGrdT.TextMatrix(j, 14) = IIf(IsNull(rsMap1("sch_code")), 0, rsMap1("sch_code"))
        
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
