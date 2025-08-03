
CREATE OR REPLACE PSM_MF_RECO_MANUAL_SET_GRID(
P_X         IN VARCHAR2,
P_TR_REG    IN VARCHAR2;
P_BR        IN VARCHAR2;
P_BRANCH    IN VARCHAR2;
P_AMC       IN VARCHAR2;
P_STATUS    IN VARCHAR2; -- RTA: RECO, UNRECO 
P_TRAN_TYPE IN VARCHAR2; -- RTA: REGULAR, SIP 


P_LOG_ID       IN VARCHAR2;
P_ROLE_ID      IN VARCHAR2;

P_CURSOR    OUT SYS_REFCURSOR

) AS 

V_DT_COUNT      NUMBER;

V_ERRR      VARCHAR2(100);
V_QUERY     VARCHAR2(4000);
V_SRmCode     VARCHAR2(5) :=NULL;

BEGIN 

IF P_X = '1' Then
    IF V_DT_COUNT <= 0 THEN 
        OPEN P_CURSOR FOR 
            SELECT 'No record found corresponding to the searching criteria' FROM DUAL;
            RETURN; 
    ELSE
      
        V_QUERY :='';
        V_QUERY :=' SELECT tran_code';
        V_QUERY := V_QUERY || '    FROM employee_master e, ';
        V_QUERY := V_QUERY || '         branch_master b, ';
        V_QUERY := V_QUERY || '         mut_fund amc, ';
        V_QUERY := V_QUERY || '         scheme_info sch, ';
        V_QUERY := V_QUERY || '         TRANSACTION_ST@MF.BAJAJCAPITAL t, ';
        V_QUERY := V_QUERY || '         investor_master i,CITY_MASTER C ';
        V_QUERY := V_QUERY || '   WHERE I.CITY_ID=C.CITY_ID(+) ';
        V_QUERY := V_QUERY || '     AND t.client_code = i.inv_code ';
        V_QUERY := V_QUERY || '     AND to_char(t.rmcode) = e.rm_code ';
        V_QUERY := V_QUERY || '     AND t.BRANCH_CODE = b.branch_code   ';
        V_QUERY := V_QUERY || '     AND t.mut_code = amc.mut_code AND t.sch_code = sch.sch_code';
        
        IF P_TR_REG = 'c' Then
            V_QUERY := V_QUERY || ' AND  (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE=''TICOB'' AND DUP_FLAG2 IN(0,9))) ';
        
        ELSIF P_TR_REG = 'k' THEN
            V_QUERY := V_QUERY || '  AND  (DUP_FLAG2 = 0 OR (REG_TRAN_FLAG=''TI'' AND DUP_FLAG2 IN(0,9)))  ';
        END IF;
        
        IF P_BRANCH <>'' AND P_BRANCH <>'' 'ALL' THEN
            V_QUERY := V_QUERY || ' and b.BRANCH_CODE in ( ''' || P_BRANCH || ''')';
        ELSE
            V_QUERY := V_QUERY || ' and b.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='''||P_LOG_ID||''' AND ROLE_ID='''||P_ROLE_ID||''') ';
        END IF;
        
        If V_SRmCode <> "" Then
             V_QUERY := V_QUERY || ' and e.rm_code='''||V_SRmCode||''' ';
        End If

        IF P_AMC <>'' AND P_AMC <>'' 'ALL' THEN
            V_QUERY := V_QUERY || ' and to_char(t.mut_code) = ''' || P_AMC || '''';
        END IF;

        IF P_STATUS IS NOT NULL THEN
            IF UPPER(P_STATUS) = 'Y' THEN
                V_QUERY := V_QUERY || ' and t.rec_flag =''Y'' ';
            ELSIF UPPER(P_STATUS) = 'N' THEN
                V_QUERY := V_QUERY || ' and (t.rec_flag =''N'' or rec_flag is null) ';
            END IF;
        END IF;
        
        
        
        IF P_TRAN_TYPE IS NOT NULL THEN
            IF UPPER(P_TRAN_TYPE) = 'REGULAR' THEN
                V_QUERY := V_QUERY || ' AND ((    UPPER (t.REG_TRANTYPE) NOT LIKE ''%SYS%'' AND UPPER (t.REG_TRANTYPE) NOT LIKE ''%SIP%'' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%S T P IN%'' ';
                V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ%''';
                V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%S T P IN REJ REVERSAL%''';
                V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE))NOT LIKE ''%STP SWITCH IN%''';
                V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%STPI%''';

                V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%REDEMPTION%''';
                V_QUERY := V_QUERY || ' AND TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';

                V_QUERY := V_QUERY || ' AND TRIM(UPPER (t.REG_TRANTYPE)) NOT LIKE ''%STPIR%''';
                V_QUERY := V_QUERY || ' ) OR t.REG_TRANTYPE IS NULL) ';

            ELSIF UPPER(P_TRAN_TYPE) = 'SIP' THEN
                V_QUERY := V_QUERY || ' AND (( UPPER (t.REG_TRANTYPE) LIKE ''%SYS%''';
                V_QUERY := V_QUERY || ' OR UPPER (t.REG_TRANTYPE) LIKE ''%SIP%''';
                V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%S T P IN%''';
                V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%S T P IN REJ%''';
                V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%S T P IN REJ REVERSAL%''';

                V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE)) LIKE ''%STP SWITCH IN%''';
                V_QUERY := V_QUERY || ' OR TRIM(UPPER(t.REG_TRANTYPE)) NOT LIKE ''%PRE-REJECTION%''';

                V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%STPI%''';
                V_QUERY := V_QUERY || ' OR TRIM(UPPER (t.REG_TRANTYPE))  LIKE ''%STPIR%'')) '; 
            END IF;
        END IF;
    
        IF P_TR_REG NOT IN ('kcob') THEN
            IF P_DATE_FROM IS NOT NULL THEN
                V_QUERY := V_QUERY || ' AND TR_DATE >= TO_DATE(''' || TO_CHAR(P_DATE_FROM, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
            END IF;

            IF P_DATE_TO IS NOT NULL THEN
                V_QUERY := V_QUERY || ' AND TR_DATE <= TO_DATE(''' || TO_CHAR(P_DATE_TO, 'DD-MM-YYYY') || ''', ''DD-MM-YYYY'')';
            END IF;
        END IF; 


        IF P_CHEQUE_TYPE IS NOT NULL AND P_CHEQUE_SEARCH IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND (';
            IF P_CHEQUE_TYPE = '001' THEN
                V_QUERY := V_QUERY || ' t.CHEQUE_NO = ''' || P_CHEQUE_SEARCH || '''';
            ELSIF P_CHEQUE_TYPE = '002' THEN
                V_QUERY := V_QUERY || ' t.FOLIO_NO = ''' || P_CHEQUE_SEARCH || '''';
            ELSIF P_CHEQUE_TYPE = '003' THEN
                V_QUERY := V_QUERY || ' t.APP_NO = ''' || P_CHEQUE_SEARCH || '''';
            ELSIF P_CHEQUE_TYPE = '004' THEN
            V_QUERY := V_QUERY || ' (UPPER(t.PAN1) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN2) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN3) = UPPER(''' || P_CHEQUE_SEARCH || ''')) ';
            ELSIF P_CHEQUE_TYPE = '005' THEN
                V_QUERY := V_QUERY || ' t.REG_SUBBROK = ''' || P_CHEQUE_SEARCH || '''';
            END IF;
            V_QUERY := V_QUERY || ')';
        END IF;


        IF P_INVESTOR_NAME IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND UPPER(TRIM(t.INV_NAME)) LIKE ''%' || REPLACE(UPPER(P_INVESTOR_NAME),' ', '%') || '%''';
        END IF;

        IF P_AMOUNT IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND abs(round(t.amount)) = ' || ABS(ROUND(TO_NUMBER(P_AMOUNT)));
        END IF;
        
        V_QUERY := V_QUERY || '  AND LPAD (t.mut_code, 2) = ''MF'' ';
        V_QUERY := V_QUERY || '     AND (t.asa <> 'C' OR t.asa IS NULL) ';
        V_QUERY := V_QUERY || '                                    AND t.tran_type IN ';
        V_QUERY := V_QUERY || '                                           (''PURCHASE'', ''REINVESTMENT'', ';
        V_QUERY := V_QUERY || '                                            ''SWITCH IN'') ';
        V_QUERY := V_QUERY || ' and t.sch_code='''||V_TR_SCH_CODE||''' and t.FOLIO_NO='''||V_TR_FOLIO||''' ';
        
        
        IF P_TR_REG NOT IN ('kcob') THEN
            V_QUERY := V_QUERY || ' AND T.TR_DATE=TO_DATE('''||V_TR_DATE||'''';
        End If
    END IF;

ELSE

END IF;


    END;




Call SetGrid
If X = 1 Then
    VSFCommGrdK.Rows = rsMap.RecordCount + 1
    If rsMap.EOF Then
        MsgBox "No record found corresponding to the searching criteria", vbInformation
        Exit Sub
    End If
    While Not rsMap.EOF

  
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
  