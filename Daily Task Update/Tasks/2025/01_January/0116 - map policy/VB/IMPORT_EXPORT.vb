Private Sub cmdImport_Click()
Dim query, branch As String
Dim RS As New ADODB.Recordset
Dim REF_NO, tot, Not_Imp As Integer
Dim flag As Boolean
'On Error GoTo err
Set XLW = XL.Workbooks.open(Filename)
If importExcelcon.State = adStateOpen Then importExcelcon.Close:   Set importExcelcon = Nothing
importExcelcon.open "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" & Filename & ";Extended Properties=""Excel 8.0;HDR=Yes;"";"
Set RsImport = New ADODB.Recordset
Set RsImport = importExcelcon.Execute("Select * from [" & lstsheet.Text & "$] ")
MyConn.Execute "delete from POLICY_MAP_TEMP1"
If RS.State = 1 Then RS.Close
RS.open "select * from POLICY_MAP_TEMP1", MyConn, adOpenDynamic, adLockOptimistic
While RsImport.EOF = False
    tot = tot + 1
    If Trim(RsImport(0)) <> "" Then
        RS.AddNew
        RS(0) = Replace(Trim(RsImport(0)), "-", "")
        If Lst_Exc_Fld.ListCount > 1 Then
           RS(1) = Trim(RsImport(1))
        End If
        RS.Update
    End If
    RsImport.MoveNext
Label3.Caption = tot
DoEvents
Wend
If RS.State = 1 Then RS.Close

sql = "SELECT DISTINCT(P.POLICY_NO) AS POLICY_NO,max(a.PREM_AMT) max_amt,decode(max(a.PREM_FREQ),1,'Y',2,'HY',4,'Q',12,'M') PREM_FREQ,max(a.NEXT_DUE_DT) NEXT_DUE_DT,MAX(A.COMPANY_CD) COMPANY_CD,R.REGION_NAME REGION_NAME,Z.ZONE_NAME ZONE_NAME, E.RM_NAME RM_NAME, B.BRANCH_NAME BRANCH_NAME,I.INVESTOR_NAME INVESTOR_NAME, I.ADDRESS1 ADDRESS1, I.ADDRESS2 ADDRESS2,C.CITY_NAME CITY_NAME,S.STATE_NAME STATE_NAME ,I.MOBILE MOBILE, I.PHONE PHONE from branch_master b,zone_master z,POLICY_MAP_TEMP1 p,bajaj_ar_head a,investor_master i,employee_master e,region_master r,STATE_MASTER S, CITY_MASTER C Where a.FRESH_RENEWAL in ('1','5') and I.CITY_ID=C.CITY_ID AND C.STATE_ID=S.STATE_ID AND e.payroll_id=to_char(a.emp_no) and TRIMZERO(upper(Trim(p.POLICY_NO))) = upper(trim(a.POLICY_NO)) "
sql = "SELECT DISTINCT(P.POLICY_NO) AS POLICY_NO,max(a.PREM_AMT) max_amt,decode(max(a.PREM_FREQ),1,'Y',2,'HY',4,'Q',12,'M') PREM_FREQ,max(a.NEXT_DUE_DT) NEXT_DUE_DT,MAX(A.COMPANY_CD) COMPANY_CD,R.REGION_NAME REGION_NAME,Z.ZONE_NAME ZONE_NAME, E.RM_NAME RM_NAME, B.BRANCH_NAME BRANCH_NAME,I.INVESTOR_NAME INVESTOR_NAME, I.ADDRESS1 ADDRESS1, I.ADDRESS2 ADDRESS2,C.CITY_NAME CITY_NAME,S.STATE_NAME STATE_NAME ,I.MOBILE MOBILE, I.PHONE PHONE from branch_master b,zone_master z,POLICY_MAP_TEMP1 p,bajaj_ar_head a,investor_master i,employee_master e,region_master r,STATE_MASTER S, CITY_MASTER C Where I.CITY_ID=C.CITY_ID AND C.STATE_ID=S.STATE_ID AND e.RM_CODE=I.RM_CODE and TRIMZERO(upper(Trim(p.POLICY_NO))) = TRIMZERO(upper(trim(a.POLICY_NO))) "
If Lst_Exc_Fld.ListCount > 1 Then
   sql = sql & " and Trim(UPPER(p.COMPANY_CD)) = upper(Trim(a.COMPANY_CD))"
End If
If cmbCompany.Text <> "" Then
    sql = sql & " And a.company_CD ='" & Mid(cmbCompany.Text, 81) & "' "
End If
sql = sql & " And a.CLIENT_CD = i.inv_Code And I.BRANCH_CODE = B.BRANCH_CODE and  b.region_id=r.region_id and b.zone_id=z.zone_id   group by p.policy_no,p.company_cd,b.BRANCH_NAME,r.region_name,z.zone_name,i.INVESTOR_NAME,i.address1,i.address2,i.mobile,i.phone,e.rm_name,C.CITY_NAME,S.STATE_NAME"
RS.open sql, MyConn
If RS.EOF = False Then
    If Dir(App.Path & "\Reports\policymap.xls") <> "" Then
        Kill App.Path & "\Reports\policymap.xls"
        RS.save App.Path & "\Reports\policymap.xls", adPersistXML
    Else
        RS.save App.Path & "\Reports\policymap.xls", adPersistXML
    End If
    XLW.Close
    Set XLW = Nothing
    RsImport.Close
    importExcelcon.Close
    Set RsImport = Nothing
    Dim xx As Long
    xx = ShellExecute(Me.hwnd, "Open", App.Path & "\Reports\policymap.xls", 0&, 0&, 1)
Else
    MsgBox "No Record found against policy no."
End If


End Sub