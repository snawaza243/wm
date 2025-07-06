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
