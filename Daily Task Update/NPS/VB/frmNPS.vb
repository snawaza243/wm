
Dim flgreset As Boolean
Dim NatureCode As String, treeselected As String, brokercode As String, Prodcode As String, prevselectednode As String
Dim MutCode As String, findTran As String, slabId As String
Public userType As String
Public SCHCODE As String, userCode As String, curTranCode As String, findFlag
Public br_no As String
Public parnode_cl As String
Public chk_mind As Boolean
Dim Intrate As Double
Dim statusflag As Integer
Dim dispatch_message As String
Dim brokerage_slab_message As String
Public VChkZeroCommission As String
Dim printflag As Boolean
Public Rs_Chk As ADODB.Recordset
Dim rs_PayDetail As ADODB.Recordset
Dim ctr As Integer, ntcode As String, upp_id As String, add_incentive_id As String, dealspecific_id As String
Dim st1 As String
Dim RowNumbforPaySlab As Integer
Dim THRUSLAB As Boolean
Dim ModifiedTrancode As String
Dim NewTranafterModi As String
Dim FlagModify As Boolean
Dim KeyAscii As Integer
Public strbranch As String
Dim FreshContri As Long
Dim min_unit As Long
Dim max_unit As Long
Dim min_price As Double
Dim max_price As Double
Dim Incr_val As Integer
Dim Fix_value As Double
Dim Pay_opt As String * 1
Dim Req() As String
Dim ReqCode As String
Dim VarScheme As String
Dim sch() As String
Dim Nsdl_branch_COde As String

Private Sub Clear()
    txtcorporatename.Text = ""
    txtChqNo.Text = ""
    dtChqDate.Text = "__/__/____"
    cmbBankName.Text = ""
    strbranch = ""
    lbtrancode.Caption = "0"
    txtAmountInvest.Text = ""
    cmbBusiBranch.Clear
    txtINV_CD.Text = ""
    Txtname.Text = ""
    txtrmbusicode.Text = ""
    optcheque.Value = True
    Txtname.Text = ""
    cboBranch.ListIndex = -1
    cmbBankName.ListIndex = -1
    cmbBusiBranch.ListIndex = -1
    txtrectno.Text = ""
    txtPRAN.Text = ""
    txtregistrationno.Text = ""
    cboRequestType.ListIndex = -1
    DtDate.Value = Date
    DtTime.Text = Format(Time, "hh:mm")
    TxtTire1.Text = 0
    TxtTire2.Text = 0
    TxtRemark.Text = ""
    txtpopregistration1.Text = 0
    txtpopregistration2.Text = 0
    txtAmountInvest.Text = 0
    txtServiceAmount.Text = 0
    TxtCollection.Text = 0
    expdate.Text = "__/__/____"
    expdate_to.Text = "__/__/____"
    CmdSave.Item(0).Enabled = True
    ReqCode = ""
    txtdocID.Text = ""
End Sub

Private Sub btnexp_Click()
Dim objXL As New Excel.Application
Dim wbXL As New Excel.Workbook
Dim wsXL As New Excel.Worksheet
Dim intRow As Integer ' counter
Dim intCol As Integer ' counter
Dim rsexp As New ADODB.Recordset
Dim vSql As String
Dim X As Integer
Dim Y As Integer
If Not IsObject(objXL) Then
    MsgBox "You need Microsoft Excel to use this function", _
       vbExclamation, "Print to Excel"
    Exit Sub
End If
On Error Resume Next
objXL.Visible = True
Set wbXL = objXL.Workbooks.Add
Set wsXL = objXL.ActiveSheet
wsXL.Name = "NPS_tran"
If optExECS.Value = True Then
    vSql = " select substr(a.unique_id,3,7)exitsr,manual_arno pran,investor_name,decode(payment_mode,'C','CHEQUE','E','ECS','H','CASH','D','DRAFT')payment_mode,cheque_no,to_char(tr_date,'dd-Mon-yyyy') ecs_date,bank_name,a.unique_id, c.amount1,c.amount2,reg_charge,tran_charge,Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2) service,(NVL(c.amount1,0) + NVL(c.amount2,0))-(reg_charge+tran_charge+Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2)) amt_inv, "
    vSql = vSql & " SUBSTR(A.UNIQUE_ID,10,17) RECEIPT_NO_10_17,CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 58002 ELSE NULL END FC_REGISTRATION_NO, "
    vSql = vSql & "          CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 'REGISTRATION' WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN('11','12') THEN 'OTHER CONTRIBUTION' END REMARKS, "
    vSql = vSql & "          CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 'REGISTRATION' WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN('11','12') THEN 'OTHER CONTRIBUTION' END REMARKS1, "
    vSql = vSql & "                     A.TRAN_CODE,REF_TRAN_CODE,CSF_TRANSACTION_ID  "
    vSql = vSql & " from transaction_st a,investor_master b,NPS_TRANSACTION c  Where a.tran_code = c.tran_code  and a.client_code=b.INV_CODE   and to_char(a.tr_date,'dd/mm/rrrr')='" & expdate.Text & "' and payment_mode='E'"
    If CboKRAImport.ListIndex = 1 Then
        vSql = vSql & " and folio_no='6036914'"
    ElseIf CboKRAImport.ListIndex = 2 Then
        vSql = vSql & " and folio_no='1171966'"
    End If
    rsexp.open vSql, MyConn
ElseIf OptNonecs.Value = True Then
    If expdate_to.Text = "__/__/____" Then
        MsgBox "To Date can not be blank", vbCritical, "BACKOFFICE"
        expdate_to.SetFocus
    End If
    vSql = "select substr(a.unique_id,3,7)exitsr,manual_arno pran,investor_name,decode(payment_mode,'C','CHEQUE','E','ECS','H','CASH','D','DRAFT', 'M','Corporate NON ECS')payment_mode,cheque_no,to_char(tr_date,'dd-Mon-yyyy') ecs_date,bank_name,a.unique_id, c.amount1,c.amount2,reg_charge,tran_charge,Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2) service,(NVL(c.amount1,0) + NVL(c.amount2,0))-(reg_charge+tran_charge+Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2)) amt_inv,"
    vSql = vSql & " SUBSTR(A.UNIQUE_ID,10,17) RECEIPT_NO_10_17,CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 58002 ELSE NULL END FC_REGISTRATION_NO, "
    vSql = vSql & "          CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 'REGISTRATION' WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN('11','12') THEN 'OTHER CONTRIBUTION' END REMARKS, "
    vSql = vSql & "          CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 'REGISTRATION' WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN('11','12') THEN 'OTHER CONTRIBUTION' END REMARKS1, "
    vSql = vSql & "                     A.TRAN_CODE,REF_TRAN_CODE,CSF_TRANSACTION_ID  "
    vSql = vSql & " from transaction_st a,investor_master b,NPS_TRANSACTION c  Where a.tran_code = c.tran_code  and  a.client_code =b.inv_code   and a.tr_date between  '" & Format(expdate.Text, "dd-MMM-yyyy") & "' and '" & Format(expdate_to.Text, "dd-MMM-yyyy") & "' and payment_mode<>'E'"
    If CboKRAImport.ListIndex = 1 Then
        vSql = vSql & " and folio_no='6036914'"
    ElseIf CboKRAImport.ListIndex = 2 Then
        vSql = vSql & " and folio_no='1171966'"
    End If
    rsexp.open vSql, MyConn
Else
    If expdate_to.Text = "__/__/____" Then
        MsgBox "To Date can not be blank", vbCritical, "BACKOFFICE"
        expdate_to.SetFocus
    End If
    vSql = "select substr(a.unique_id,3,7)exitsr,manual_arno pran,investor_name,decode(payment_mode,'C','CHEQUE','E','ECS','H','CASH','D','DRAFT', 'M','Corporate NON ECS')payment_mode,cheque_no,to_char(tr_date,'dd-Mon-yyyy') ecs_date,bank_name,a.unique_id, c.amount1,c.amount2,reg_charge,tran_charge,Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2) service,(NVL(c.amount1,0) + NVL(c.amount2,0))-(reg_charge+tran_charge+Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2)) amt_inv,"
    vSql = vSql & " SUBSTR(A.UNIQUE_ID,10,17) RECEIPT_NO_10_17,CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 58002 ELSE NULL END FC_REGISTRATION_NO, "
    vSql = vSql & "          CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 'REGISTRATION' WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN('11','12') THEN 'OTHER CONTRIBUTION' END REMARKS, "
    vSql = vSql & "          CASE WHEN SUBSTR(UNIQUE_ID,1,2) ='11' THEN 'REGISTRATION' WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NOT NULL THEN 'REGISTRATION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) ='12' AND REF_TRAN_CODE IS NULL THEN 'NORMAL CONTRIBUTION' "
    vSql = vSql & "                     WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN('11','12') THEN 'OTHER CONTRIBUTION' END REMARKS1, "
    vSql = vSql & "                     A.TRAN_CODE,REF_TRAN_CODE,CSF_TRANSACTION_ID"
    vSql = vSql & " from transaction_st a,investor_master b,NPS_TRANSACTION c  Where a.tran_code = c.tran_code  and  a.client_code =b.inv_code   and a.tr_date between  '" & Format(expdate.Text, "dd-MMM-yyyy") & "' and '" & Format(expdate_to.Text, "dd-MMM-yyyy") & "'"
    If CboKRAImport.ListIndex = 1 Then
        vSql = vSql & " and folio_no='6036914'"
    ElseIf CboKRAImport.ListIndex = 2 Then
        vSql = vSql & " and folio_no='1171966'"
    End If
    rsexp.open vSql, MyConn
End If
wsXL.Cells(1, 1).Value = "PoP NO"
wsXL.Cells(1, 2).Value = "RRAN No"
wsXL.Cells(1, 3).Value = "Client Name"
wsXL.Cells(1, 4).Value = "Payment Mode"
wsXL.Cells(1, 5).Value = "Cheque Number"
wsXL.Cells(1, 6).Value = "Date"
wsXL.Cells(1, 7).Value = "Bank Name"
wsXL.Cells(1, 8).Value = "Receipt no"
wsXL.Cells(1, 9).Value = "Tier 1"
wsXL.Cells(1, 10).Value = "Tier 2"
wsXL.Cells(1, 11).Value = "Reg Charges"
wsXL.Cells(1, 12).Value = "Tran Charges"
wsXL.Cells(1, 13).Value = "GST"
wsXL.Cells(1, 14).Value = "Amount Invested"
wsXL.Cells(1, 15).Value = "Receipt no(10-17)"
wsXL.Cells(1, 16).Value = "Fc Registration No"
wsXL.Cells(1, 17).Value = "Remarks"
wsXL.Cells(1, 18).Value = "Remarks1"
wsXL.Cells(1, 19).Value = "Tran_Code"
wsXL.Cells(1, 20).Value = "Ref_Tran_Code"
wsXL.Cells(1, 21).Value = "CSF_TRANSACTION_ID"

If Not rsexp.EOF Or Not rsexp.BOF Then
For X = 1 To rsexp.RecordCount
    For Y = 0 To 20
        wsXL.Cells(X + 1, Y + 1).Value = rsexp(Y)
    Next
    rsexp.MoveNext
Next
rsexp.Close
Else
rsexp.Close
End If
End Sub

Private Sub cboBranch_Click()
If cboBranch.ListIndex <> -1 Then
    Req = Split(cboBranch.Text, "#")
    Nsdl_branch_COde = Req(1)
    txtregistrationno.Text = Req(2)
End If
End Sub

Private Sub cboKRA_Click()
    Dim vSql As String
    Dim rs_get_data As New ADODB.Recordset
    If CboKRA.ListIndex = 0 Then
        vSql = "select branch_code,branch_name,NSDL_NO from branch_master where nsdl_no is not null AND BRANCH_CODE=10010044"
    Else
        vSql = "select branch_code,branch_name,KARVY_NSDL_NO NSDL_NO from branch_master where KARVY_NSDL_NO is not null AND BRANCH_CODE=10010044"
    End If
    rs_get_data.open vSql, MyConn, adOpenStatic
    cboBranch.Clear
    If Not rs_get_data.EOF Then
        Do While Not rs_get_data.EOF
            cboBranch.AddItem rs_get_data.Fields(1) & Space(80) & "#" & rs_get_data.Fields(0) & Space(10) & "#" & rs_get_data.Fields(2)
            rs_get_data.MoveNext
        Loop
    End If
    cboBranch.ListIndex = 0
    Set rs_get_data = Nothing
End Sub

Public Sub cboRequestType_Click()
Dim MyReqType As Double
Dim TranAmount As Double
Dim v_servicetax As Double

If CDate(DtDate) < CDate("01/06/2015") Then
    v_servicetax = 0.1236
ElseIf CDate(DtDate) >= CDate("01/06/2015") And CDate(DtDate) < CDate("15/11/2015") Then
    v_servicetax = 0.14
ElseIf CDate(DtDate) >= CDate("15/11/2015") And CDate(DtDate) < CDate("01/06/2016") Then
    v_servicetax = 0.145
ElseIf CDate(DtDate) >= CDate("01/06/2017") And CDate(DtDate) < CDate("01/07/2017") Then
    v_servicetax = 0.15
ElseIf CDate(DtDate) >= CDate("01/07/2017") Then
    v_servicetax = 0.18
End If

If cboRequestType.Text = "" Then
    Exit Sub
End If

If Val(lbtrancode.Caption) > 0 Then
   MyReqType = SqlRet("select nvl(app_no,0) from transaction_st where tran_code='" & lbtrancode.Caption & "'")
   TranAmount = SqlRet("select amount from transaction_st where tran_code='" & lbtrancode.Caption & "'")
End If

'11,12,21 ke alawa sab me

If Val(lbtrancode.Caption) > 0 And MyReqType = 12 And TranAmount = 0 Then '12 Contribution
Else
    If txtregistrationno.Text = "" Then
       MsgBox "Please Select NSDL Branch First", vbInformation
       Exit Sub
    End If
    Req = Split(cboRequestType.Text, "#")
    ReqCode = Req(1)
        
    If ReqCode = "11" Or ReqCode = "12" Then   '11 SUBSCRIBER REGISTRATION
        If VarScheme = "TIRE1" Then
            TxtTire1.Enabled = True
            TxtTire2.Enabled = False
        ElseIf VarScheme = "TIRE2" Then
            TxtTire2.Enabled = True
            TxtTire1.Enabled = False
        End If
    End If
    If ReqCode = "11" And Val(TxtTire1.Text) = 0 Then '11 SUBSCRIBER REGISTRATION
        txtpopregistration1.Text = "0"
    Else
        If ReqCode = "11" Or ReqCode = "12" Then   'We do not have to calculate pop registration charge other then 11,12,21
            FreshContri = Val(TxtTire1.Text) * 0.0025
            If FreshContri < 20 Then
               FreshContri = 20
            End If
            If FreshContri >= 25000 Then
               FreshContri = 25000
            End If
            If Val(TxtTire1.Text) > 0 Then
                If CDate(DtDate) >= CDate("01/11/2017") Then
                    txtpopregistration1.Text = 200 + FreshContri
                Else
                    txtpopregistration1.Text = 100 + FreshContri
                End If
            Else
                txtpopregistration1.Text = 0
            End If
            If VarScheme = "TIRE2" Or VarScheme = "TIRE1-2" Then
                TxtTire2.Enabled = True
                If Val(TxtTire2.Text) = 0 Then
                    txtpopregistration2.Text = "0"
                Else
                    txtpopregistration2.Text = "0"
                End If
            End If
        End If
    End If
    If Val(TxtTire2) <> 0 And ReqCode = "11" Then '11 SUBSCRIBER REGISTRATION
        txtpopregistration2.Text = Val(TxtTire2.Text) * 0.0025
        If Val(txtpopregistration2.Text) < 20 Then
            txtpopregistration2.Text = 20
         End If
    End If
    If (ReqCode <> "11" And Val(TxtTire1) <> 0) Then  '11 SUBSCRIBER REGISTRATION
         If ReqCode = "11" Or ReqCode = "12" Then  'We do not have to calculate pop registration charge other then 11,12,21
            txtpopregistration1.Text = Val(TxtTire1.Text) * 0.0025
            If Val(txtpopregistration1.Text) < 20 Then
               txtpopregistration1.Text = 20
            End If
            If Val(txtpopregistration1.Text) >= 25000 Then
               txtpopregistration1.Text = 25000
            End If
         End If
    End If
    If (ReqCode <> "11" And Val(TxtTire2) <> 0) Then  '11 SUBSCRIBER REGISTRATION
         txtpopregistration2.Text = Val(TxtTire2.Text) * 0.0025
         If Val(txtpopregistration2.Text) < 20 Then
            txtpopregistration2.Text = 20
         End If
         If Val(txtpopregistration2.Text) >= 25000 Then
            txtpopregistration2.Text = 25000
         End If
    End If
    If (ReqCode <> "11" And Val(TxtTire2) <> 0) And VarScheme = "TIRE1-2" Then
    End If
    If (ReqCode <> "11" And ReqCode <> "12") Then   '11 SUBSCRIBER REGISTRATION   '12 Contribution
        TxtCollection.Visible = True
        Label22.Visible = True
        Label19.Caption = "Misclenious collection"
        Label22.Caption = "Collection Amount"
        TxtTire2.Text = "0"
        txtpopregistration1.Text = "0"
        txtpopregistration2.Text = "0"
        txtAmountInvest = "0"
        TxtTire2.Enabled = False
        txtServiceAmount.Text = Round((Val(TxtTire1.Text) * v_servicetax), 2)
        txtAmountInvest = Round(((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - ((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(txtServiceAmount.Text)))), 2) 'added by pankaj pundir on 30112018
    Else
        TxtCollection.Text = "0"
        TxtCollection.Visible = False
        Label22.Visible = False
        Label22.Caption = "Amount Invested"
        Label19.Caption = "Amount Invested"
        MiscAmt.Visible = False
        txtAmountInvest.Visible = True
        If VarScheme = "TIRE1" Or VarScheme = "TIRE1-2" Then
            TxtTire1.Enabled = True
        End If
        If (ReqCode <> "11") Then
            txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
        Else
            txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
        End If
        txtAmountInvest = Round(((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - ((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(txtServiceAmount.Text)))), 2)
    End If
    If (ReqCode <> "11") Then
        If ReqCode = "11" Or ReqCode = "12" Then
            If Val(TxtTire1.Text) > 0 Then
                txtpopregistration1.Text = Val(TxtTire1.Text) * 0.0025
                If Val(txtpopregistration1.Text) < 20 Then
                   txtpopregistration1.Text = 20
                End If
                If Val(txtpopregistration1.Text) >= 25000 Then
                   txtpopregistration1.Text = 25000
                End If
            Else
                 txtpopregistration1.Text = 0
            End If
             
            txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
            txtAmountInvest.Text = Abs(Round((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - (Val(txtServiceAmount.Text) + Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)), 2))
            
        End If
    End If
    If Val(TxtCollection.Text) > 0 Then
        txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(TxtCollection.Text)) * v_servicetax), 2)
        txtAmountInvest.Text = Round((Val(TxtCollection.Text)) - (Val(txtServiceAmount.Text) + Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)), 2)
    End If
End If
If OptCorporate.Value = True And ReqCode = 11 Then
    txtpopregistration1.Text = Round((Val(TxtTire1.Text) / (100 + v_servicetax * 100)) * 100, 2)
    txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text)) * v_servicetax), 2)
    txtAmountInvest.Text = 0
End If
End Sub

Private Sub ChkZeroCommission_Click()
If ChkZeroCommission.Value = 1 Then
    VChkZeroCommission = "Y"
Else
    VChkZeroCommission = "N"
End If
End Sub

Public Sub cmbProd_Change()
On Error Resume Next
Dim rs_get_nature As ADODB.Recordset, rs_get_product As ADODB.Recordset, rs_get_trantype As ADODB.Recordset
If cmbProd.Text <> "" Then
      cmbproduct.Clear
      lstSch.Clear
        lstlongname.ListItems.Clear
      Set rs_get_nature = MyConn.Execute("Select nature_code,prod_code from product_master where name like '" & Trim(cmbProd.Text) & "%'")
      If rs_get_nature.EOF Then Exit Sub
      NatureCode = rs_get_nature(0)
      Prodcode = rs_get_nature(1)
      If NatureCode = "NT001" Then
          Set rs_get_product = MyConn.Execute("Select mut_name,mut_code from mut_fund order by mut_name")
          cmbStru.Enabled = True
          CmbType.Enabled = True
          cmbNature.Enabled = True
          cmbStru.Visible = True
          cmbStru.BackColor = vbWhite
          CmbType.BackColor = vbWhite
          cmbNature.BackColor = vbWhite
          lblStru.Visible = True
          lblStru.Caption = "Structure"
          lblType.Caption = "Plan"
          lblNature.Caption = "Asset Class"
          cmbFolio.BackColor = vbWhite
          cmbFolio.Enabled = True
          Frame3.Enabled = True
          frameOP.Visible = False
          Frame4.Visible = True
          dttranMF.Text = Format(Date, "dd/mm/yyyy")
      Else
          If Trim(GlbroleId) = 1 Then
            Set rs_get_product = MyConn.Execute("Select iss_name from iss_master where iss_code in (Select distinct issuermf_code from product_class_issuer_mf where prod_code='" & rs_get_nature(1) & "') order by iss_name")
          Else
            Set rs_get_product = MyConn.Execute("Select iss_name from iss_master where iss_code in (Select distinct issuermf_code from product_class_issuer_mf where prod_code='" & rs_get_nature(1) & "') and iss_code in (SELECT iss_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) or city_id='ALL')) order by iss_name")
          End If
          
          Frame3.Enabled = False
          Frame4.Visible = False
          frameOP.Visible = True
            txtOpUnits.Enabled = False   'vinod 11/05/2005
            txtOpPrice.Enabled = False  'vinod 11/05/2005
            dtOpMat.Enabled = False
            dtOpTran.Text = Format(Date, "dd/mm/yyyy")
            txtOpUnits.Text = ""
            txtOpPrice.Text = ""
            dtOpMat.Text = "__/__/____"
            Label9.Visible = True
            dtOpMat.Visible = True
            txtOpAmount.Text = ""
          If NatureCode <> "NT005" And NatureCode <> "NT002" Then
              lblOPUnits.Caption = "Months"
              lblOPRate.Caption = "Interest Rate"
              lbfaceval.Visible = False
              txtfaceval.Visible = False
              txtfaceval.Enabled = False
              lbmatperiod.Visible = False
              txtmatperiod.Visible = False
              txtmatperiod.Enabled = False
              Label9.Visible = True
              dtOpMat.Visible = True
          Else
              lblOPUnits.Caption = "Units/Bond"
              lbfaceval.Visible = True
              txtfaceval.Visible = True
              txtfaceval.Enabled = False
              lbmatperiod.Visible = True
              txtmatperiod.Visible = True
              txtmatperiod.Enabled = False
              Label9.Visible = False
              dtOpMat.Visible = False
          End If
      End If
      While Not rs_get_product.EOF
          cmbproduct.AddItem rs_get_product(0)
          rs_get_product.MoveNext
      Wend
      cmbproduct.ListIndex = 0
      Set rs_get_trantype = MyConn.Execute("Select tran_type_desc from tran_type_master a,nature_tran_master b where a.tran_type_code=b.tran_type_code and nature_code='" & NatureCode & "' ")
      While Not rs_get_trantype.EOF
          cmbTranType.AddItem rs_get_trantype(0)
          rs_get_trantype.MoveNext
      Wend
      cmbTranType.Text = "PURCHASE"
End If
If UCase(cmbProd.Text) = "MF" Then
  OptSip.Visible = True
  optStp.Visible = True
Else
  OptSip.Visible = False
  optStp.Visible = False
End If
If NatureCode = "NT001" Then
CmdStatus.Enabled = False
Else
CmdStatus.Enabled = True
End If
If NatureCode = "NT003" Or NatureCode = "NT004" Then
    OptOthers.Enabled = True
Else
    OptOthers.Enabled = False
End If
End Sub

Public Sub cmbproduct_Change()
Dim rs_get_mutCode As ADODB.Recordset
SCHCODE = ""
If cmbproduct.Text <> "" Then
    If NatureCode = "NT001" Then
        Set rs_get_mutCode = MyConn.Execute("Select mut_code from mut_fund where mut_name='" & Trim(cmbproduct.Text) & "'")
        If Frame3.Visible = True Then
              'FillFolioNo
        End If
    Else
        Set rs_get_mutCode = MyConn.Execute("Select iss_code from iss_master a,product_class_issuer_mf b where trim(iss_name)='" & Trim(cmbproduct.Text) & "' and a.iss_code=b.issuermf_code and b.prod_code='" & Prodcode & "'")
    End If
    If Not rs_get_mutCode.EOF Then
          MutCode = rs_get_mutCode(0)
          mutschfill
     End If
End If
End Sub



Private Sub mutschfill()
On Error Resume Next
Dim sqlSch As String
Dim rs_get_sch As ADODB.Recordset
Dim DuplicateLongName As String
lstSch.Clear
lstSchm.Clear
sqlSch = ""
If NatureCode = "NT001" Then
    sqlSch = "Select sch_name,short_name from scheme_info where scheme_info.mut_code='" & MutCode & "' "
    If cmbStru.Text <> "" Then
        sqlSch = sqlSch & " and upper(type1)='" & Trim(cmbStru.Text) & "'"
    End If
    If CmbType.Text <> "" Then
        sqlSch = sqlSch & " and upper(type3)='" & Trim(CmbType.Text) & "'"
    End If
    If Trim(cmbNature.Text) <> "" Then
      If UCase(Trim(cmbNature.Text)) = UCase(Trim("Liquid")) Then
         sqlSch = sqlSch & " and upper(nature)= 'SHORT TERM DEBT'"
      ElseIf UCase(Trim(cmbNature.Text)) = UCase(Trim("Balanced")) Then
        sqlSch = sqlSch & " and upper(nature)='EQUITY & DEBT'"
      Else
        sqlSch = sqlSch & " and upper(nature)='" & UCase(Trim(cmbNature.Text)) & "'"
      End If
    End If
Else
    sqlSch = "Select DISTINCT longname,osch_name from other_product where other_product.iss_code='" & MutCode & "' ORDER BY LONGNAME"

End If
Set rs_get_sch = MyConn.Execute(sqlSch)
lstlongname.ListItems.Clear
lstSchm.Clear
DuplicateLongName = ""
While Not rs_get_sch.EOF
    If Trim(DuplicateLongName) <> Trim(rs_get_sch(0)) Then
        lstlongname.ListItems.Add , , rs_get_sch(0)
    End If
    DuplicateLongName = Trim(rs_get_sch(0))
    lstSchm.AddItem rs_get_sch(1)
    rs_get_sch.MoveNext
Wend
changeListWidth lstSch
'Added by vinod for fund Merging
If NatureCode = "NT001" Then
Dim rsFund As New ADODB.Recordset
Dim rsMFund As New ADODB.Recordset
   Set rsFund = MyConn.Execute("select fund_to,fund_with from FUND_MARGE where fund_to='" & MutCode & "' or fund_with='" & MutCode & "'")
    If Not rsFund.EOF Then
        cmbfund.Clear
            While Not rsFund.EOF
               Set rsMFund = MyConn.Execute("select mut_code,mut_name from mut_fund where mut_Code in ('" & rsFund(0) & "','" & rsFund(1) & "')")
                While Not rsMFund.EOF
                    cmbfund.AddItem rsMFund(1) & Space(80) & "#" & rsMFund(0)
                    rsMFund.MoveNext
                Wend
                rsFund.MoveNext
            Wend
    Else
        cmbfund.Clear
    End If
End If
Set rsMFund = Nothing
Set rsFund = Nothing
Call Shortmutschfill
End Sub

Private Sub CmbProduct_Click()
    cmbproduct_Change
End Sub

Private Sub CmdEcsTran_Click()
    ChkZeroCommission.Value = 0
    frmnpsecsimp.Show
    frmnpsecsimp.ZOrder
    frmnpsecsimp.Label2 = "NPS ECS Transaction Importing"
    Nps_Importing_flag = "ECS"
End Sub

Private Sub CmdNonEcsTran_Click()
frmnpsecsimp.Show
frmnpsecsimp.ZOrder
frmnpsecsimp.Label2 = "NPS Corporate NON ECS Transaction Importing"
Nps_Importing_flag = "NON ECS"

If ChkZeroCommission.Value = 1 Then
    VChkZeroCommission = "Y"
Else
    VChkZeroCommission = "N"
End If
End Sub

Private Sub cmdprint_Click()
If txtrectno.Text = "" Then
    MsgBox "Please Enter Receipt No First"
    Exit Sub
End If
Dim query As String
Dim CreateQuery As String
query = ""
CreateQuery = ""
query = "select * from npstranreceipt_view"
query = query & " where unique_id='" & txtrectno.Text & "'"
CRPT1.Reset
CRPT1.Connect = MyConn.ConnectionString
CreateQuery = "create or replace view npstranreceipt_view1 as " & query
MyConn.Execute CreateQuery
CRPT1.ReportFileName = App.Path & "\reports\NPSTranReceipt.rpt"
CRPT1.ParameterFields(2) = "guserid;" & Glbloginid & ";true"
CRPT1.ParameterFields(3) = "rundate ; " & Date & " ;true"
CRPT1.WindowShowPrintSetupBtn = True
CRPT1.WindowState = crptMaximized
Call SaveLogIn(Glbloginid, "Reports", Me.Name)
CRPT1.WindowMaxButton = True
CRPT1.WindowShowPrintBtn = True
CRPT1.WindowShowSearchBtn = True
CRPT1.action = 1
Call SaveLogIn(Glbloginid, "", Me.Name)
Exit Sub
errr:
    MsgBox "There is Some Error in Processing AS: " & vbCrLf & err.Description, vbExclamation, strBajaj
    Resume
End Sub

Private Sub CmdSave_Click(Index As Integer)
Dim Busi_Branch_cd As String
Dim Busi_Rm_Cd As String
Dim ClientBranchCode As String
Dim ClientRmCode As String
Dim br_cd() As String
Dim paymode As String
Dim rsGet As New ADODB.Recordset
Dim invCode As String
Dim rsTran As New ADODB.Recordset
Dim dipo_type As String * 1
Dim Pay_type As String * 1
Dim str_test As String
Dim MyTranCode As String
Dim MyGSTNO As String
Dim MyTranCode1 As String
Dim Rs_Rect As ADODB.Recordset
Dim MySecReq As String
Dim Dup_RectNo As Variant
Dim Vclientcategory As String
Dim i As Integer
Dim Xmin As Variant
Dim Xmax As Variant
If cmbproduct.Text = "" Then
   Exit Sub
End If
Dim j As Variant
Dim X As Variant
Dim Y As Variant
Dim M As Integer
Dim Z As Variant

'If Index = 0 Then
'    If GlbroleId = "212" Or GlbroleId = "1" Then
'    Else
'        MsgBox "Only Punching Team can punch the transaction.", vbInformation
'        Exit Sub
'    End If
'ElseIf Index = 4 Then
'    If GlbroleId = "146" Or GlbroleId = "1" Then
'    Else
'        MsgBox "Only NPS Team can modify the transaction.", vbInformation
'        Exit Sub
'    End If
'End If

If txtdocID.Text = "" Then
    MsgBox "DT No can not be left blank", vbInformation
    Exit Sub
End If

'FATCA VALIDATION
If Index = 0 Then
    If OptCorporate.Value = True Then
        If txtcorporatename.Text = "" Then
            MsgBox "Corporate name cannot be left blank.", vbInformation
            txtcorporatename.SetFocus
            Exit Sub
        End If
    End If
    
    If chkUnfreeze.Value = 0 Then
        If txtPRAN.Text <> " " Then
            If SqlRet("SELECT COUNT(*) FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO='" & txtPRAN.Text & "'") >= 1 Then
                 MsgBox "FATCA for this PRAN is non compliant.Please contact product team for the same", vbInformation
                 Exit Sub
            End If
        End If
    Else
        MyConn.Execute "delete from NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO=trim('" & txtPRAN.Text & "')"
    End If
End If

'VINIT 05-DEC-2015
If Index = 0 Or Index = 4 Then
    Vclientcategory = SqlRet("select category_id from client_master where client_code='" & Mid(txtINV_CD.Text, 1, 8) & "' ")
    If Vclientcategory <> "4004" Then
        If lbtrancode.Caption = "0" Then
            rsGet.open "select count(*) from ( select TRAN_CODE from transaction_st where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) )", MyConn, adOpenForwardOnly
            If rsGet(0) > 0 Then
                MsgBox "Duplicate Cheque Number !", vbInformation
                Exit Sub
            End If
            rsGet.Close
         Else
            If ReqCode = "11" Then
                rsGet.open "select count(*) from ( select TRAN_CODE from transaction_st where TRAN_CODE <> " & lbtrancode.Caption & " and  mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) AND REF_TRAN_CODE IS NULL Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) AND TRAN_CODE <> " & lbtrancode.Caption & ")", MyConn, adOpenForwardOnly
                If rsGet(0) > 0 Then
                    MsgBox "Duplicate Cheque Number !", vbInformation
                    Exit Sub
                End If
                rsGet.Close
            Else
                rsGet.open "select count(*) from ( select TRAN_CODE from transaction_st where TRAN_CODE <> " & lbtrancode.Caption & " and  mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" & Trim(txtChqNo.Text) & "' and tr_date >= add_months(sysdate,-6) AND TRAN_CODE <> " & lbtrancode.Caption & ")", MyConn, adOpenForwardOnly
                If rsGet(0) > 0 Then
                    MsgBox "Duplicate Cheque Number !", vbInformation
                    Exit Sub
                End If
                rsGet.Close
            End If
        End If
    End If
End If
'-----------------
    
If Index <> 3 And Index <> 1 Then
    If txtregistrationno.Text = "" Then
       MsgBox "Please Select NSDL Branch First", vbInformation
       Exit Sub
    End If
End If
If cboRequestType.Text <> "" Then
    Req = Split(cboRequestType.Text, "#")
    ReqCode = Req(1)
End If

Busi_Branch_cd = ""
If cmbBusiBranch.Text <> "" Then
    br_cd = Split(cmbBusiBranch.Text, "#")
    Busi_Branch_cd = br_cd(1)
End If
Busi_Rm_Cd = SqlRet("select payroll_id from employee_master where payroll_id='" & txtrmbusicode & "'")
If Index = 1 Then
    Unload Me
    Exit Sub
End If
If txtINV_CD.Text <> "" Then
    If rsGet.State = 1 Then rsGet.Close
    rsGet.open "select rm_code,branch_code from investor_master where inv_code='" & txtINV_CD.Text & "'", MyConn, adOpenKeyset
    If Not rsGet.EOF Then
       ClientBranchCode = rsGet.Fields("branch_code")
       ClientRmCode = rsGet.Fields("rm_code")
    End If
End If
If rsGet.State = 1 Then rsGet.Close
If Index = 3 Then
    Call Clear
End If
If Index = 4 Then   ''Modification
    If OptCorporate.Value = True Then
        If txtcorporatename.Text = "" Then
            MsgBox "Corporate name cannot be left blank.", vbInformation
            txtcorporatename.SetFocus
            Exit Sub
        End If
    End If
    If lbtrancode.Caption = "0" Then
        MsgBox "Please Select a Transaction to Modify", vbInformation
        Exit Sub
    End If
    If chkSaveValidation(False, True) = False Then
        Exit Sub
    End If
    Busi_Branch_cd = ""
    br_cd = Split(cmbBusiBranch.Text, "#")
    Busi_Branch_cd = br_cd(1)
    paymode = ""
    If optcheque.Value = True Then
        paymode = "C"
    ElseIf optdraft.Value = True Then
        paymode = "D"
    ElseIf optcash.Value = True Then
        paymode = "H"
    ElseIf OptEcs.Value = True Then
        paymode = "E"
    ElseIf OptOthers.Value = True Then
        paymode = "R"
    ElseIf OptCorNECS.Value = True Then
        paymode = "M"
    End If
    rsTran.open "select * from transaction_st where tran_code='" & lbtrancode.Caption & "'", MyConn, adOpenDynamic, adLockPessimistic
    MyTranCode = rsTran.Fields("Tran_code")
    rsTran("tr_date") = DtDate.Value
    rsTran("client_Code") = Trim(txtINV_CD.Text)
    rsTran("source_code") = Left(Trim(txtINV_CD.Text), 8)
    rsTran("BUSI_BRANCH_CODE") = Busi_Branch_cd
    rsTran("BUSINESS_RMCODE") = Busi_Rm_Cd
    rsTran("mut_code") = MutCode
    rsTran("sch_code") = SCHCODE
    rsTran("amount") = Trim(txtAmountInvest.Text)
    rsTran("folio_no") = Trim(txtregistrationno.Text)
    rsTran("app_no") = ReqCode
    rsTran("PAYMENT_MODE") = paymode
    If paymode <> "M" Then
        rsTran("CHEQUE_DATE") = dtChqDate.Text
        rsTran("cheque_no") = Trim(txtChqNo.Text)
        rsTran("BANK_NAME") = cmbBankName.Text
    End If
    rsTran("manual_arno") = Trim(txtPRAN.Text)
    rsTran("corporate_name") = Trim(txtcorporatename.Text)
    rsTran("unique_id") = txtrectno.Text
    rsTran("MODIFY_USER") = Glbloginid
    rsTran("MODIFY_DATE") = Format(ServerDateTime, "dd/mm/yyyy")
    rsTran.Update
    rsTran.Close
    
    MyConn.Execute "update nps_transaction set amount1=" & Val(TxtTire1.Text) & ",amount2=" & Val(TxtTire2.Text) & ",REG_CHARGE=" & Val(txtpopregistration1.Text) & ",Tran_CHARGE=" & Val(txtpopregistration2.Text) & ",SERVICETAX=" & Val(txtServiceAmount.Text) & ",remark='" & TxtRemark.Text & "' where tran_code='" & Trim(lbtrancode.Caption) & "'"
    MsgBox "Transaction Updated Sucessfully", vbInformation
End If
If Index = 0 Then   ''save
    If chkSaveValidation(True, False) = False Then
        Exit Sub
    End If
        
    If Vclientcategory <> "4004" Then
        rsGet.open "select count(*) from transaction_st where cheque_no='" & Trim(txtChqNo.Text) & "' and trim(bank_name)='" & Trim(cmbBankName.Text) & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')", MyConn, adOpenForwardOnly
        If rsGet(0) > 0 Then
            MsgBox "Duplicate Cheque Number !", vbInformation
            Exit Sub
        End If
        rsGet.Close
     End If
    
    rsGet.open "select count(*) from transaction_st where CLIENT_code='" & Trim(txtINV_CD.Text) & "' and sch_code='" & SCHCODE & "' and app_no = '" & ReqCode & "' and amount= '" & Trim(txtAmountInvest.Text) & "' and cheque_no='" & Trim(txtChqNo.Text) & "' And Trim(bank_name) = '" & Trim(cmbBankName.Text) & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')", MyConn, adOpenForwardOnly
    If rsGet(0) > 0 Then
        MsgBox "Duplicate Transaction !", vbInformation
        Exit Sub
    End If
    rsGet.Close
  
'---------------------------------------------------------------------------------------------------------------
    paymode = ""
    If optcheque.Value = True Then
        paymode = "C"
    ElseIf optdraft.Value = True Then
        paymode = "D"
    ElseIf optcash.Value = True Then
        paymode = "H"
    ElseIf OptCorNECS.Value = True Then
        paymode = "M"
    End If
    If optcheque.Value = True Or optdraft.Value = True Then
        str_test = " insert into transaction_sttemp"
        str_test = str_test & " (CORPORATE_NAME,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "','1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & "" & Trim(txtChqNo.Text) & ",to_date('" & dtChqDate & "','dd/MM/yyyy')"
        str_test = str_test & " ,'NPS','" & Trim(txtdocID.Text) & "')"
    Else
        str_test = ""
        str_test = " insert into transaction_sttemp"
        str_test = str_test & " (CORPORATE_NAME,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,remark,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "','1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & " 'NPS','" & Trim(txtdocID.Text) & "')"
    End If
    MyConn.Execute str_test
    
    MyTranCode = SqlRet("select max(tran_code) from temp_tran where branch_code=" & ClientBranchCode & " and substr(tran_code,1,2)='07' ")
    
    MyGSTNO = ""
    
    MyGSTNO = SqlRet("select invoice_no from transaction_sttemp where tran_code='" & MyTranCode & "'")
    
    If optcheque.Value = True Or optdraft.Value = True Then
        str_test = " insert into transaction_st"
        str_test = str_test & " (invoice_no,CORPORATE_NAME,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & MyGSTNO & "','" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "',trim('" & MyTranCode & "'),'1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD.Text & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & "" & Trim(txtChqNo.Text) & ",to_date('" & dtChqDate & "','dd/MM/yyyy')"
        str_test = str_test & " ,'NPS','" & Glbloginid & "','" & Trim(txtdocID.Text) & "')"
    Else
        str_test = " insert into transaction_st"
        str_test = str_test & " (invoice_no,CORPORATE_NAME,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT,"
        str_test = str_test & " BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,remark,LOGGEDUSERID,doc_id)"
        str_test = str_test & " Values"
        str_test = str_test & " ('" & MyGSTNO & "','" & txtcorporatename.Text & "','" & txtPRAN.Text & "','" & cmbBankName.Text & "','" & txtregistrationno.Text & "','" & ReqCode & "','" & paymode & "',trim('" & MyTranCode & "'),'1','" & Format(DtDate, "DD-MMM-YYYY") & "'," & txtINV_CD.Text & ","
        str_test = str_test & " '" & MutCode & "','" & SCHCODE & "','PURCHASE'," & Val(txtAmountInvest.Text) & "," & ClientBranchCode & ","
        str_test = str_test & " " & Mid(txtINV_CD, 1, 8) & "," & ClientRmCode & "," & Trim(txtrmbusicode.Text) & "," & Busi_Branch_cd & ","
        str_test = str_test & " 'NPS','" & Glbloginid & "','" & Trim(txtdocID.Text) & "')"
    End If
    MyConn.Execute str_test
    str_test = "insert into nps_transaction(tran_code,amount1,amount2,reg_charge,tran_charge,SERVICETAX,remark)"
    str_test = str_test & " Values(trim('" & MyTranCode & "')," & Val(TxtTire1.Text) & "," & Val(TxtTire2.Text) & ","
    str_test = str_test & " " & Val(txtpopregistration1.Text) & "," & Val(txtpopregistration2.Text) & "," & Val(txtServiceAmount.Text) & ",'" & TxtRemark.Text & "')"
    MyConn.Execute str_test
    txtrectno.Text = SqlRet("select unique_id from transaction_st where tran_code='" & MyTranCode & "'")
    MsgBox "Your Transaction No Is " & MyTranCode & " and Your Recpt No IS " & txtrectno.Text & " "
    '--------------------------DOUBLE TRANSACTION OF CONTRIBUTION WHEN REGISTRATION----------------------
    If ReqCode = "11" And OptIndividual.Value = True Then
        sql = ""
        sql = " insert into transaction_sttemp (CORPORATE_NAME,ref_tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,doc_id) select CORPORATE_NAME,tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,0, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,'' from transaction_sttemp where tran_code='" & MyTranCode & "'"
        MyConn.Execute sql
        MyTranCode1 = SqlRet("select max(tran_code) from temp_tran where branch_code=" & ClientBranchCode & " and substr(tran_code,1,2)='07' ")
        sql = " update tb_doc_upload set ar_code='" & MyTranCode1 & "' where common_id='" & Trim(txtdocID.Text) & "'"
        MyConn.Execute sql
        sql = ""
        sql = " insert into transaction_st (ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id) select ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,12,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id from transaction_sttemp where tran_code='" & MyTranCode1 & "'"
        MyConn.Execute sql
        str_test = "insert into nps_transaction(tran_code,amount1,amount2,reg_charge,tran_charge,SERVICETAX,remark)"
        str_test = str_test & " Values(trim('" & MyTranCode1 & "'),0,0,"
        str_test = str_test & " 0,0,0,'" & TxtRemark.Text & "')"
        MyConn.Execute str_test
        MyConn.Execute ("DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE='" & MyTranCode1 & "'")
        MySecReq = SqlRet("select unique_id from transaction_st where tran_code='" & MyTranCode1 & "'")
        MsgBox "Your Duplicate Transaction No Is " & MyTranCode1 & " and Your Recpt No IS " & MySecReq & " "
    End If
    MyConn.Execute ("DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE='" & MyTranCode & "'")
    Call Clear
End If
Set rsGet = Nothing
Set rsTran = Nothing

Dim recd_paid As New ADODB.Command
Set recd_paid.ActiveConnection = MyConn
recd_paid.CommandType = adCmdStoredProc
recd_paid.CommandText = "Recd_paid_update"
recd_paid.Parameters.Append recd_paid.CreateParameter("tr_code", adVarChar, adParamInput, 50, MyTranCode)
recd_paid.Execute
            
End Sub
Private Sub cmdSelectSch_Click()
    strscheme = "frmNPS"
    frmSchemeSearch.Show
    frmSchemeSearch.ZOrder
End Sub


Private Sub cmdShow_Click()
Dim rstb As New ADODB.Recordset
Dim clinfo As New ADODB.Recordset
Dim i As Integer
Dim reqq As Variant
Dim reqcod As String
rstb.open "select nvl(ar_code,'0'),nvl(INV_CODE,0),nvl(BUSI_BRANCH_CODE,0),nvl(BUSI_RM_CODE,0) from  tb_doc_upload where tran_type='FI' and common_id ='" & txtdocID.Text & "'", MyConn

If Not rstb.EOF And Not rstb.BOF Then
If rstb(1) <> 0 Then
clinfo.open "select inv_code,investor_name from investor_master where inv_code='" & rstb(1) & "'", MyConn
txtINV_CD = clinfo(0)
Txtname = clinfo(1)
clinfo.Close
End If
txtrmbusicode = rstb(3)
cboBranch.ListIndex = 0

branch_fill
rstb.Close
Else
rstb.Close
End If
End Sub

Private Sub dtChqDate_Validate(Cancel As Boolean)
Dim dtf As String
Dim DtL As String
If Trim(dtChqDate.Text) <> "" And dtChqDate.Text <> "__/__/____" Then
    dtf = DateAdd("D", -30, Format(CDate(Date), "dd/MM/yyyy"))
    DtL = DateAdd("D", 30, Format(CDate(Date), "dd/MM/yyyy"))
    
    If Format(dtChqDate.Text, "dd/mm/yyyy") < CDate(Format(dtf, "dd/mm/yyyy")) Or Format(dtChqDate.Text, "dd/mm/yyyy") > CDate(Format(DtL, "dd/mm/yyyy")) Then
        MsgBox "Cheque/Draft Date Should be with in One Month", vbExclamation, "Alert"
        dtChqDate.Text = "__/__/____"
        dtChqDate.SetFocus
        Exit Sub
    End If
End If
End Sub


Private Sub Form_KeyUp(KeyCode As Integer, Shift As Integer)
    If KeyCode = vbKeyF1 Then
        cmdSelectSch_Click
    End If
    If KeyCode = vbKeyF2 Then
        Picture4_Click
    End If
    If KeyCode = vbKeyF3 Then
        Picture1_Click 0
    End If
End Sub

Private Sub Form_Load()
    Dim rs_get_data As New ADODB.Recordset
    Me.Top = (main.Height - Me.Height) / 2 - 600
    Me.Left = (main.width - Me.width) / 2
    Me.Icon = LoadPicture(App.Path & "\W.ICO")
    
    '======================ECS Trnsaction Importing Permission================================'
    If Glbloginid = "115514" Or Glbloginid = "46183" Or GlbroleId = "1" Or Glbloginid = "114678" Then
        CmdEcsTran.Enabled = True
        CmdNonEcsTran.Enabled = True
    Else
        CmdEcsTran.Enabled = False
    End If
    
    If GlbroleId = "1" Or GlbroleId = "91" Then
        expfrm.Enabled = True
    End If
    
    '========================================================================================='
    
    cmbProd.Clear
    cmbProd.AddItem "New Pension Scheme"
    cmbProd.Text = "New Pension Scheme"
    cmbProd_Change
    CmdSave.Item(0).Enabled = True
    CmdSave.Item(4).Enabled = False
    Set rs_get_data = MyConn.Execute("Select distinct bank_name from bank_master order by bank_name")
    cmbBankName.Clear
    While Not rs_get_data.EOF
        cmbBankName.AddItem rs_get_data(0)
        rs_get_data.MoveNext
    Wend
    Set rs_get_data = Nothing
    CboKRA.AddItem "NSDL", 0
    CboKRA.AddItem "KARVY", 1
    CboKRA.ListIndex = 0
    
    CboKRAImport.AddItem "ALL", 0
    CboKRAImport.AddItem "NSDL", 1
    CboKRAImport.AddItem "KARVY", 2
    CboKRAImport.ListIndex = 0
    
    rs_get_data.open "select request_code,request_name from request_master order by request_code", MyConn, adOpenStatic
    cboRequestType.Clear
    If Not rs_get_data.EOF Then
        Do While Not rs_get_data.EOF
            cboRequestType.AddItem rs_get_data.Fields(1) & Space(80) & "#" & rs_get_data.Fields(0)
            rs_get_data.MoveNext
        Loop
    End If
    Set rs_get_data = Nothing
    DtDate.Value = Date
    DtTime.Text = Format(Time, "hh:mm")
    
    OptIndividual.Value = True
    optExECS.Value = True
End Sub



Private Function chkSaveValidation(FrSave As Boolean, FrMody As Boolean) As Boolean
Dim rs_get_chk_folio As ADODB.Recordset, rs_get_first_date As ADODB.Recordset, rs_get_facevalue As ADODB.Recordset
Dim tempFol As String
Dim pre As String
Dim dtf, DtL As String
Dim MyRs_validate As ADODB.Recordset
Dim MyRs_validate1 As ADODB.Recordset
If cmbProd.Text = "" Then
     MsgBox "Please select a Product Class.", vbInformation
      chkSaveValidation = False
      Exit Function
End If
If MutCode = "" Then
      MsgBox "Please select a Product.", vbInformation
      chkSaveValidation = False
      Exit Function
End If
If SCHCODE = "" Then
      MsgBox "Please select a Scheme.", vbInformation
      chkSaveValidation = False
      Exit Function
      
End If
If txtINV_CD.Text = "" Then
    MsgBox "Please select a Investor.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If ReqCode = "" Then
    MsgBox "Please Select a Request Id.", vbInformation
    chkSaveValidation = False
    Exit Function
End If

If Not IsDate(DtDate) Then
     MsgBox "Please enter a correct Transaction Date.", vbInformation
     chkSaveValidation = False
     Exit Function
End If
If Format(CDate(DtDate), "dd/mm/yyyy") < "01/01/1980" Then
    MsgBox "Please enter a correct transaction date.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If txtAmountInvest.Text = "" Then
    MsgBox "Please enter amount.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If Not IsNumeric(txtAmountInvest.Text) Then
    MsgBox "Please enter Correct Amount.", vbInformation
    chkSaveValidation = False
    Exit Function
End If
If txtrmbusicode.Text = "" Or Len(txtrmbusicode) < 5 Then
    MsgBox "Not a Valid RM Business Code", vbCritical, "BACKOFFICE"
    txtrmbusicode.SetFocus
    chkSaveValidation = False
    Exit Function
End If
Dim GEt_busiRMCode As New ADODB.Recordset
Set GEt_busiRMCode = MyConn.Execute("select rm_code from employee_master where payroll_id='" & Trim(txtrmbusicode.Text) & "'")
If GEt_busiRMCode.EOF Then
    MsgBox "Not a Valid RM Business Code", vbCritical, "BACKOFFICE"
    txtrmbusicode.SetFocus
    chkSaveValidation = False
    Exit Function
End If
If FrSave = True And FrMody = False Then
    If (CDate(Format(DtDate, "dd-mm-yyyy")) < CDate(Format(Glbins_previousdate, "DD-MM-YYYY"))) Or (CDate(Format(DtDate, "dd-mm-yyyy")) > CDate(Format(Glbins_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
End If
If FrSave = False And FrMody = True Then
    If (CDate(Format(DtDate, "dd-mm-yyyy")) < CDate(Format(Glbup_previousdate, "DD-MM-YYYY"))) Or (CDate(Format(DtDate, "dd-mm-yyyy")) > CDate(Format(Glbup_nextdate, "DD-MM-YYYY"))) Then
        MsgBox "Security restrictions for date range"
        chkSaveValidation = False
        Exit Function
    End If
End If
If CheckDate(DtDate, Format(ServerDateTime, "dd/mm/yyyy")) = False Then
    MsgBox "Transaction Date Cannot Be Greater than Current Date ", vbInformation
    chkSaveValidation = False
    Exit Function
End If

If OptCorNECS.Value = False And optcheque.Value = False And optdraft.Value = False And OptEcs.Value = False And optcash.Value = False And OptOthers.Value = False Then
    MsgBox "Please Select a Payment Mode.", vbExclamation
    chkSaveValidation = False
    Exit Function
End If

If optcheque.Value = True Then
    If Trim(cmbBankName.Text) = "" Then
        MsgBox "Please Select a Bank Name", vbExclamation, "Alert"
        cmbBankName.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a Cheque Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Cheque Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
If optdraft.Value = True Then
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a Draft Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Draft Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
If OptEcs.Value = True Then
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a MCR Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
If OptOthers.Value = True Then
    If Trim(txtChqNo.Text) = "" Then
        MsgBox "Please Insert a FDR Number", vbExclamation, "Alert"
        txtChqNo.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    If Trim(dtChqDate.Text) = "" Or Trim(dtChqDate.Text) = "__/__/____" Then
        MsgBox "Please Insert a Renewal Date", vbExclamation, "Alert"
        dtChqDate.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    
    If Trim(dtChqDate.Text) <> "" And dtChqDate.Text <> "__/__/____" Then
        dtf = "01/" & Format(CDate(Date), "mm/yyyy")
        DtL = DateAdd("M", 1, Format(CDate(dtf), "dd/MM/yyyy")) - 1
        If CDate(dtChqDate.Text) < CDate(dtf) Or CDate(dtChqDate.Text) > CDate(DtL) Then
            MsgBox "Cheque/Draft Date Should be in Current Month", vbExclamation, "Alert"
            dtChqDate.Text = "__/__/____"
            dtChqDate.SetFocus
            chkSaveValidation = False
            Exit Function
        End If
    End If
End If
'End If
If cmbBusiBranch.Text = "" Then
    MsgBox "Please Select Business Branch ", vbInformation
    cmbBusiBranch.SetFocus
    chkSaveValidation = False
    Exit Function
End If
'-------------------My Validations--------------------------------
If CmdSave.Item(0) = True Then
        Set MyRs_validate = New ADODB.Recordset
        Mysql = "select * from transaction_st where client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
        MyRs_validate.open Mysql, MyConn, adOpenForwardOnly
        If Not MyRs_validate.EOF Then
            If MyRs_validate.Fields("sch_code") = "OP#09971" Then
            End If
        End If
        MyRs_validate.Close
        Set MyRs_validate = Nothing
End If

Set MyRs_validate = New ADODB.Recordset
Mysql = "select * from transaction_st where client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971','OP#09973') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
MyRs_validate.open Mysql, MyConn, adOpenForwardOnly
If Not MyRs_validate.EOF Then
Else
    If SCHCODE = "OP#09972" Then
        MsgBox "Not Allowed in this Scheme Please Select Scheme Tier1 or Tier1+2"
        lstlongname.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
    
End If
MyRs_validate.Close
Set MyRs_validate = Nothing

If Not (OptCorporate.Value = True And ReqCode = 11) Then
    Set MyRs_validate = New ADODB.Recordset
    
    If lbtrancode.Caption = "0" Then
        Mysql = "select * from transaction_st where client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971','OP#09972','OP#09973') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
    Else
        Mysql = "select * from transaction_st where tran_code<>" & lbtrancode.Caption & " and client_code='" & txtINV_CD.Text & "' and sch_code in ('OP#09971','OP#09972','OP#09973') and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
    End If
    
    MyRs_validate.open Mysql, MyConn, adOpenForwardOnly
    If Not MyRs_validate.EOF Then
        If Trim(MyRs_validate.Fields("sch_code")) = "OP#09971" And (SCHCODE = "OP#09971" Or SCHCODE = "OP#09972") Then
            If Trim(MyRs_validate.Fields("manual_arno")) <> Trim(txtPRAN.Text) Then
                MsgBox "Please Enter Same PRAN No. "
                txtPRAN.SetFocus
                chkSaveValidation = False
                Exit Function
            End If
        ElseIf Trim(MyRs_validate.Fields("sch_code")) = "OP#09973" And SCHCODE = "OP#09973" Then
            If Trim(MyRs_validate.Fields("manual_arno")) <> Trim(txtPRAN.Text) Then
                MsgBox "Please Enter Same PRAN No. "
                txtPRAN.SetFocus
                chkSaveValidation = False
                Exit Function
            End If
        End If
    Else
        Set MyRs_validate1 = New ADODB.Recordset
        If lbtrancode.Caption = "0" Then
            Mysql = "select * from transaction_st where manual_arno='" & txtPRAN.Text & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
        Else
            Mysql = "select * from transaction_st where tran_code<>" & lbtrancode.Caption & " and manual_arno='" & txtPRAN.Text & "' and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN')"
        End If
        MyRs_validate1.open Mysql, MyConn, adOpenForwardOnly
        If Not MyRs_validate.EOF Then
            MsgBox "Please Enter Different PRAN No. "
            txtPRAN.SetFocus
            chkSaveValidation = False
            Exit Function
        End If
        MyRs_validate1.Close
        Set MyRs_validate1 = Nothing
    End If
    MyRs_validate.Close
    Set MyRs_validate = Nothing
End If
If SCHCODE = "OP#09973" And ReqCode = "11" Then
    If (Trim(TxtTire1.Text) = 0 Or Trim(TxtTire1.Text) = "") Or (Trim(TxtTire2.Text) = 0 Or Trim(TxtTire2.Text) = "") Then
        MsgBox "Please Enter Tier1 and Tier2 Amount in this Scheme"
        TxtTire1.SetFocus
        lstlongname.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If

If ReqCode = "11" And SCHCODE = "OP#09973" Then
    If (Val(Trim(TxtTire1.Text)) < 500) Or (Val(Trim(TxtTire2.Text)) < 1000) Then
        MsgBox "Please Enter Correct Amount in this Scheme"
        TxtTire1.SetFocus
        lstlongname.SetFocus
        chkSaveValidation = False
        Exit Function
    End If
End If
'vinod  11/05/2005
chkSaveValidation = True
End Function
Private Sub Image1_Click()
frmBank.Show
frmBank.ZOrder
End Sub



Public Sub lstlongname_ItemClick(ByVal Item As MSComctlLib.ListItem)
    Call Shortmutschfill
End Sub
Public Sub Shortmutschfill()
Dim sqlSch As String
Dim sql As String
Dim rs_get_sch As ADODB.Recordset
lstSch.Clear
sqlSch = ""
sql = ""
If Trim(GlbroleId) = 1 Then
If NatureCode = "NT001" Then
    sqlSch = "Select short_name from scheme_info where scheme_info.mut_code='" & MutCode & "' and sch_name='" & lstlongname.SelectedItem.Text & "'"
    If cmbStru.Text <> "" Then
        sqlSch = sqlSch & " and upper(type1)='" & Trim(cmbStru.Text) & "'"
    End If
    If CmbType.Text <> "" Then
        sqlSch = sqlSch & " and upper(type3)='" & Trim(CmbType.Text) & "'"
    End If
    If Trim(cmbNature.Text) <> "" Then
      If UCase(Trim(cmbNature.Text)) = UCase(Trim("Liquid")) Then
         sqlSch = sqlSch & " and upper(nature)= 'SHORT TERM DEBT'"
      ElseIf UCase(Trim(cmbNature.Text)) = UCase(Trim("Balanced")) Then
        sqlSch = sqlSch & " and upper(nature)='EQUITY & DEBT'"
      Else
        sqlSch = sqlSch & " and upper(nature)='" & UCase(Trim(cmbNature.Text)) & "'"
      End If
    End If
Else
    sqlSch = "Select osch_name,osch_code from other_product where other_product.iss_code='" & MutCode & "' and longname='" & lstlongname.SelectedItem.Text & "'"
End If
Else
If NatureCode = "NT001" Then
    sql = "Select sch_code from scheme_info where scheme_info.mut_code='" & MutCode & "' and TRIM(sch_name)='" & Trim(lstlongname.SelectedItem.Text) & "'"
    If cmbStru.Text <> "" Then
        sql = sql & " and upper(type1)='" & Trim(cmbStru.Text) & "'"
    End If
    If CmbType.Text <> "" Then
        sql = sql & " and upper(type3)='" & Trim(CmbType.Text) & "'"
    End If
    If Trim(cmbNature.Text) <> "" Then
      If UCase(Trim(cmbNature.Text)) = UCase(Trim("Liquid")) Then
         sql = sql & " and upper(nature)= 'SHORT TERM DEBT'"
      ElseIf UCase(Trim(cmbNature.Text)) = UCase(Trim("Balanced")) Then
        sql = sql & " and upper(nature)='EQUITY & DEBT'"
      Else
        sql = sql & " and upper(nature)='" & UCase(Trim(cmbNature.Text)) & "'"
      End If
    End If
    sqlSch = "SELECT shortname,sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) or city_id='ALL') and iss_code='" & MutCode & "' and TRIM(schname)='" & Trim(lstlongname.SelectedItem.Text) & "' and sch_code in (" & sql & ")"
Else
    sqlSch = "SELECT shortname,sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (" & Branches & "))) or city_id='ALL') and iss_code='" & MutCode & "' and TRIM(schname)='" & Trim(lstlongname.SelectedItem.Text) & "'"
End If
End If
Set rs_get_sch = MyConn.Execute(sqlSch)
lstSch.Clear
If Not rs_get_sch.EOF Then
    While Not rs_get_sch.EOF
        lstSch.AddItem rs_get_sch(0) & Space(100) & "=" & rs_get_sch(1)
        rs_get_sch.MoveNext
    Wend
Else
    MsgBox "Permission Denied", vbCritical, "Wealth Maker"
End If
If lstSch.List(0) <> "" Then
    sch = Split(lstSch.List(0), "=")
    SCHCODE = sch(1)
    If SCHCODE = "OP#09971" Then
        TxtTire1.Enabled = True
        TxtTire2.Enabled = False
        VarScheme = "TIRE1"
        TxtTire2.Text = 0
        txtpopregistration2.Text = 0
        txtAmountInvest.Text = 0
        txtServiceAmount.Text = 0
        TxtCollection.Text = 0
        Call cboRequestType_Click
    ElseIf SCHCODE = "OP#09972" Then
        TxtTire2.Enabled = True
        TxtTire1.Enabled = False
        VarScheme = "TIRE2"
        TxtTire1.Text = 0
        txtpopregistration1.Text = 0
        txtAmountInvest.Text = 0
        txtServiceAmount.Text = 0
        TxtCollection.Text = 0
        Call cboRequestType_Click
    ElseIf SCHCODE = "OP#09973" Then
        TxtTire2.Enabled = True
        TxtTire1.Enabled = True
        VarScheme = "TIRE1-2"
        Call cboRequestType_Click
    End If
End If
End Sub

Public Sub lstSch_Click()
Dim rs_get_sch_code As ADODB.Recordset
Dim Rs_IPO As ADODB.Recordset
Dim i As Integer, j As Integer
Dim matPeriod As String
Dim mm As Integer
Dim faceVal As Long
On Error Resume Next
For i = 0 To lstSch.ListCount - 1
     If lstSch.Selected(i) = True Then
         Exit For
     End If
Next

If NatureCode = "NT001" Then
    If sql_ora = "SQL" Then
         Set rs_get_sch_code = MyConn.Execute("Select sch_code from scheme_info where short_name='" & Trim(lstSch.List(i)) & "' and mut_code='" & MutCode & "'")
    Else
         Set rs_get_sch_code = MyConn.Execute("Select sch_code from scheme_info where trim(short_name)='" & Trim(lstSch.List(i)) & "' and mut_code='" & MutCode & "'")
    End If
Else
    If NatureCode = "NT006" Then
        Set rs_get_sch_code = MyConn.Execute("Select osch_code,int_rate,facevalue,mat_period from other_product where osch_name='" & Trim(lstSch.List(i)) & "' and longname='" & Trim(lstlongname.SelectedItem) & "' and iss_code='" & MutCode & "'")     'chvv
        txtOpPrice.Text = IIf(rs_get_sch_code("int_rate") <> "", rs_get_sch_code("int_rate"), 0)
        If rs_get_sch_code("facevalue") <> "" Then
        faceVal = rs_get_sch_code("facevalue")
        txtfaceval.Text = faceVal
        End If
        If IsNumeric(txtOpAmount.Text) And Trim(txtOpAmount.Text) <> "" And Trim(txtOpAmount.Text) <> 0 And Trim(txtOpPrice.Text) <> 0 Then
            txtOpUnits.Text = Format((txtOpAmount.Text / txtOpPrice.Text), "0.00")
        End If
        txtmatperiod.Text = IIf(rs_get_sch_code("mat_period") <> "", rs_get_sch_code("mat_period"), 0)
    Else
        Set rs_get_sch_code = MyConn.Execute("Select osch_code,int_rate,mat_period from other_product where osch_name='" & Trim(lstSch.List(i)) & "' and longname='" & Trim(lstlongname.SelectedItem) & "' and iss_code='" & MutCode & "'")      'chvv
        matPeriod = rs_get_sch_code("mat_period")
        txtOpUnits.Text = matPeriod
        txtOpPrice.Text = rs_get_sch_code("int_rate")
        If dtOpTran.Text <> "00/00/0000" Or dtOpTran.Text <> "" Then
            If txtOpUnits.Text <> "" Then
                mm = Trim(txtOpUnits.Text)
                dtOpMat.Text = DateAdd("m", mm, Format(dtOpTran.Text, "dd/mm/yyyy"))
                dtOpMat.Text = Format(dtOpMat.Text, "dd/mm/yyyy")
                dtOpMat.Enabled = False
            End If
        End If
    End If
End If
SCHCODE = rs_get_sch_code(0)
min_unit = 0
max_unit = 0
min_price = 0
max_price = 0
Incr_val = 0
Fix_value = 0
Pay_opt = ""
txtPrice.Locked = False
txtPrice.Text = 0
Set Rs_IPO = MyConn.Execute("Select * from ipo_info where osch_code ='" & SCHCODE & "'")
If Not Rs_IPO.EOF Then
    min_unit = Rs_IPO("min_unit")
    max_unit = Rs_IPO("max_unit")
    min_price = Rs_IPO("min_price")
    max_price = Rs_IPO("max_price")
    Incr_val = Rs_IPO("Incremental_Unit")
    Fix_value = Rs_IPO("Fix_price")
    Pay_opt = Rs_IPO("Payment_method")
End If
Rs_IPO.Close
If Pay_opt = "Y" Then
    Frame1.Visible = True
    optMethod2.Value = True
Else
    Frame1.Visible = False
    optMethod2.Value = False
    optMethod1.Value = False
End If

Set Rs_IPO = Nothing
Set rs_get_sch_code = Nothing
End Sub
Private Sub optcash_Click()
      cmbBankName.Text = ""
     cmbBankName.Enabled = False
     txtChqNo.Enabled = False
     dtChqDate.Enabled = False
     txtChqNo.Text = ""
     dtChqDate.Text = "__/__/____"
End Sub

Private Sub optCDSL_Click()
    If optCDSL.Value = True Then
        txtDPID.Text = ""
        txtDPID.Locked = True
        txtclient.MaxLength = 16
    End If
End Sub

Private Sub optcheque_Click()
 Label11.Caption = "Cheque No."
 Label12.Caption = "Cheque Dated."
 cmbBankName.Enabled = True
 txtChqNo.Enabled = True
 dtChqDate.Enabled = True
 cmbBankName.Text = ""
 txtChqNo.Text = ""
 dtChqDate.Text = "__/__/____"
End Sub


Private Sub OptCorNECS_Click()
cmbBankName.Text = ""
cmbBankName.Enabled = False
txtChqNo.Enabled = False
dtChqDate.Enabled = False
txtChqNo.Text = ""
dtChqDate.Text = "__/__/____"
End Sub

Private Sub OptCorporate_Click()
lblcorporate.Visible = True
txtcorporatename.Visible = True
txtcorporatename.SetFocus
End Sub

Private Sub optdraft_Click()
 Label11.Caption = "Draft No."
 Label12.Caption = "Draft Dated."
 cmbBankName.Enabled = True
 txtChqNo.Enabled = True
 dtChqDate.Enabled = True
 cmbBankName.Text = ""
 txtChqNo.Text = ""
 dtChqDate.Text = "__/__/____"
End Sub


Private Sub OptECS_Click()
 Label11.Caption = "MCR No."
 Label12.Caption = "Dated."
 cmbBankName.Enabled = True
 txtChqNo.Enabled = True
 dtChqDate.Enabled = True
 cmbBankName.Text = ""
 txtChqNo.Text = ""
 dtChqDate.Text = "__/__/____"
End Sub
Private Sub optMethod1_Click()
    If optMethod1.Value = True Then
        txtPrice.Text = Fix_value
        txtPrice.Locked = True
        TxtAmount.Text = ""
    End If
End Sub

Private Sub optMethod2_Click()
    If optMethod2.Value = True Then
        txtPrice.Text = ""
        txtPrice.Locked = False
        TxtAmount.Text = ""
    End If
End Sub


Private Sub optNDSL_Click()
    If optNDSL.Value = True Then
        txtDPID.Text = "IN"
        txtDPID.Locked = False
        txtDPID.MaxLength = 8
        txtclient.MaxLength = 8
    End If
End Sub

Private Sub optExALL_Click()
    If optExALL.Value = False Then
        expdate_to.Visible = False
        expdate.Visible = True
    Else
        expdate_to.Visible = True
        expdate.Visible = True
    End If
End Sub

Private Sub optExECS_Click()
    If optExECS.Value = True Then
        expdate_to.Visible = False
        expdate.Visible = True
    Else
        expdate_to.Visible = True
        expdate.Visible = True
    End If
End Sub

Private Sub optindividual_Click()
lblcorporate.Visible = False
txtcorporatename.Visible = False
txtcorporatename.Text = ""
End Sub

Private Sub OptNonecs_Click()
If OptNonecs.Value = False Then
    expdate_to.Visible = False
    expdate.Visible = True
Else
    expdate_to.Visible = True
    expdate.Visible = True
End If
End Sub

Private Sub OptOthers_Click()
 Label11.Caption = "FDR No."
 Label12.Caption = "Renewal Date."
 cmbBankName.Enabled = False
 txtChqNo.Enabled = True
 dtChqDate.Enabled = True
 cmbBankName.Text = ""
 txtChqNo.Text = ""
 dtChqDate.Text = "__/__/____"
End Sub


Private Sub pic_newinvestor_Click()
    frmupdatedinvestor.Show
    frmupdatedinvestor.ZOrder
End Sub

Private Sub Picture1_Click(Index As Integer)
    treeselected = "N"
    Set frmtree_searchnew.currentForm = Nothing
    Set frmtree_searchnew.currentForm = frmNPS
    frmtree_search.treeName = "Treeclient"
    frmtree_searchnew.Cmbcat.Text = "INVESTOR"   'BY HIMANSHU
    frmtree_searchnew.Cmbcat.Enabled = False
    
    frmtree_searchnew.lblOF.Visible = True
    frmtree_searchnew.CmbClientBroker.Visible = True
    frmtree_searchnew.CmbClientBroker.ListIndex = 0
    frmtree_searchnew.Label7.Enabled = True
    frmtree_searchnew.txtCliSubName.Enabled = True
    'frmtree_search.chkIndia.Visible = True
    frmtree_searchnew.Show
    frmtree_searchnew.cmbBranch.SetFocus
    frmtree_searchnew.ZOrder
End Sub

Private Sub Picture2_Click()
 frmactopen.Show
 frmactopen.ZOrder
End Sub

Private Sub Picture3_Click()
 'cmdSave.Item(0).Enabled = False
    treeselected = "N"
    Set frmTransactionfind.currentForm = Nothing
    Set frmTransactionfind.currentForm = frmNPS
    frmTransactionfind.Label4.Caption = "PRAN No."
    EPF = False
    frmTransactionfind.Show
    frmTransactionfind.ZOrder
End Sub

Private Sub Picture4_Click()
    FRMTRAN_IPO_SEARCH.Show
    FRMTRAN_IPO_SEARCH.ZOrder
End Sub

Private Sub Picture6_Click()
    frmUpdateClient.Show
    frmUpdateClient.ZOrder
End Sub
Private Sub txtAmount_KeyUp(KeyCode As Integer, Shift As Integer)
    If Val(txtPrice.Text) <> "0" Then
    End If
End Sub
Private Sub txtPrice_KeyUp(KeyCode As Integer, Shift As Integer)
    TxtAmount.Text = Val(txtPrice.Text) * Val(txtShares.Text)
End Sub
Private Sub txtPrice_Validate(Cancel As Boolean)
    If min_price <> 0 And max_price <> 0 And Val(txtPrice.Text) <> 0 And optMethod1.Value = False Then
        If Val(txtPrice.Text) < min_price Or Val(txtPrice.Text) > max_price Then
            MsgBox "Price can be From " & min_price & " To " & max_price, vbInformation
            txtPrice.Text = "0"
            Cancel = True
        End If
    End If

End Sub

Private Sub Picture7_Click(Index As Integer)
On Error GoTo err1
Dim openfile As String, ARL As String, FL As String
Dim strFile As String
Dim strPath As String
Dim strFiletype() As String
Dim l_file As String
Dim rsGetDocPath As New ADODB.Recordset

      
    Me.MousePointer = 11

    rsGetDocPath.open "select doc_filename,doc_path from tb_doc_upload where common_id='" & Trim(txtdocID.Text) & "' and tran_type='FI'", MyConn, adOpenForwardOnly
    If Not rsGetDocPath.EOF Then
        strFile = rsGetDocPath("doc_filename")
        strPath = UCase(rsGetDocPath("doc_path"))
    End If
    rsGetDocPath.Close
    Set rsGetDocPath = Nothing
    
    ReDim strFiletype(1)
    strFiletype = Split(strFile, ".")
    l_file = "c:\doc\doc" & Glbloginid & "." & LCase(strFiletype(1))
    
   

    Generate_File strPath, strFile, l_file
    
    ShellExecute Me.hwnd, vbNullString, "c:\doc\ftp1.bat", vbNullString, "c:", SW_SHOWNORMAL
    For i = 1 To 1000
        DoEvents
    Next i
    ShellExecute Me.hwnd, vbNullString, l_file, vbNullString, "c:", SW_SHOWNORMAL
    
    Me.MousePointer = 0
    Exit Sub
err1:
    Me.MousePointer = 0
    MsgBox err.Description
    
End Sub



Private Sub txtChqNo_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
End If
End Sub

Private Sub TxtCollection_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
End If
End Sub

Private Sub TxtCollection_LostFocus()
Call cboRequestType_Click
End Sub
Private Sub txtPRAN_Validate(Cancel As Boolean)
    If chkUnfreeze.Value = 0 Then
        If txtPRAN.Text <> " " Then
            If SqlRet("SELECT COUNT(*) FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO='" & txtPRAN.Text & "'") >= 1 Then
                 MsgBox "FATCA for this PRAN is non compliant.Please contact product team for the same", vbInformation
                 Exit Sub
            End If
        End If
    Else
        MyConn.Execute "delete from NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO=trim('" & txtPRAN.Text & "')"
    End If
End Sub

Private Sub txtrmbusicode_KeyUp(KeyCode As Integer, Shift As Integer)
branch_fill
End Sub

Private Sub branch_fill()
Dim rsEmp As ADODB.Recordset
Dim b_cd() As String

Set rsEmp = New ADODB.Recordset
rsEmp.open "Select * from employee_master where payroll_id='" & txtrmbusicode.Text & "' and type='A'", MyConn, adOpenForwardOnly
If Not rsEmp.EOF Then
    If rsEmp.Fields("category_id") <> "2001" And rsEmp.Fields("category_id") <> "2018" Then
        MsgBox "Rm Should be Sales and Support Only "
        txtrmbusicode.Text = ""
        cmbBusiBranch.Clear
        Exit Sub
    End If
End If
rsEmp.Close
Set rsEmp = Nothing
    If Len(txtrmbusicode.Text) = 5 Or Len(txtrmbusicode.Text) = 6 Then
        cmbBusiBranch.Clear
        If strbranch <> "" Then
            cmbBusiBranch.AddItem strbranch
            cmbBusiBranch.ListIndex = 0
        End If
        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name from employee_master e,branch_master b where e.payroll_id='" & Trim(txtrmbusicode.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null)", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            If strbranch <> "" Then
                b_cd = Split(strbranch, "#")
                If b_cd(1) <> rsEmp(0) Then
                    cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                    cmbBusiBranch.ListIndex = 0
                End If
            Else
                cmbBusiBranch.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                cmbBusiBranch.ListIndex = 0
            End If
        End If
        rsEmp.Close
        Set rsEmp = Nothing
    End If
End Sub
Private Sub txtShares_KeyPress(KeyAscii As Integer)
    NumberValidation KeyAscii
End Sub


Private Sub txtShares_KeyUp(KeyCode As Integer, Shift As Integer)
    TxtAmount.Text = Val(txtPrice.Text) * Val(txtShares.Text)
End Sub


Private Sub txtShares_Validate(Cancel As Boolean)
    If min_unit <> 0 And max_unit <> 0 And Val(txtShares.Text) <> 0 Then
        If Val(txtShares.Text) < min_unit Or Val(txtShares.Text) > max_unit Then
            MsgBox "Number of Shares can be From " & min_unit & " To " & max_unit, vbInformation
            txtShares.Text = "0"
            Cancel = True
        End If
    End If
    If Incr_val <> 0 Then
        If (Val(txtShares.Text) Mod Incr_val) > 0 Then
            MsgBox "Number of Shares should be in Multiples of " & Incr_val, vbInformation
            txtShares.Text = "0"
            Cancel = True
        End If
    End If
End Sub


Private Sub TxtTire1_KeyPress(KeyAscii As Integer)
If (KeyAscii < 48 Or KeyAscii > 57) And KeyAscii <> 8 Then
    KeyAscii = 0
End If
End Sub

Private Sub TxtTire1_LostFocus()
Call cboRequestType_Click
End Sub


Private Sub TxtTire2_LostFocus()
Call cboRequestType_Click
End Sub
