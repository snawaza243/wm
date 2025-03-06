Dim userPermi As Boolean
Dim str2 As String
Dim str3 As String
Dim Btype As String
Dim Sqlstr_1 As String

Private Sub cmbCompny_Validate(Cancel As Boolean)
Dim rsPlan As New ADODB.Recordset
msql = "Select Plan_no,Plan from bajaj_plan_master where company_cd='" & Trim(Mid(cmbCompny.List(cmbCompny.ListIndex), 51)) & "'"
rsPlan.open (msql), MyConn
cmbPlan.Clear
If Not rsPlan.EOF Then
       Do While Not rsPlan.EOF
            cmbPlan.AddItem rsPlan("plan") & Space(50 - Len(rsPlan("plan"))) & "#" & rsPlan("plan_no")
        rsPlan.MoveNext
        Loop
End If
txtcompany.Text = Trim(Mid(cmbCompny.Text, 1, 50))
'cmbPlan.ListIndex = 0
rsPlan.Close
Set rsPlan = Nothing
End Sub
Private Sub cmdExit_Click()
Unload Me
End Sub

Private Sub cmbPlan_Click()
txtplan.Text = Trim(Mid(cmbPlan.Text, 1, 50))
End Sub

Private Sub cmbprint_Click()
On Error GoTo err
Dim sno As Integer
Dim rssave As New ADODB.Recordset
Dim rsMAx As New ADODB.Recordset
rsMAx.open "SELECT max(s_no) FROM BAJAJ_DDATA_REMINDER", MyConn
rssave.open "SELECT * FROM BAJAJ_DDATA_REMINDER", MyConn, adOpenDynamic, adLockOptimistic
If rsMAx.EOF = False Then
rssave.AddNew
    sno = rsMAx(0) + 1
    rssave("s_no") = sno
    If TXTCODE.Text = "" Then
        TXTCODE.Text = sno
    End If
    rssave("INV_CODE") = TXTCODE.Text
    rssave("POLICY_NO") = txtpolicy.Text
    rssave("cl_name") = txtcname.Text
    rssave("cl_add1") = txtadd1.Text
    rssave("cl_add2") = txtadd2.Text
    rssave("cl_add3") = txtadd3.Text
    rssave("cl_add4") = txtadd4.Text
    rssave("cl_add5") = txtadd5.Text
    rssave("cl_city") = txtcity.Text
    rssave("state_name") = txtstate.Text
    rssave("cl_pin") = txtpin.Text
    rssave("cl_phone1") = txtphone.Text
    rssave("cl_phone2") = txtphone2.Text
    rssave("company_name") = txtcompany.Text
    If CDate(Format(mskduedate.Text, "dd/mm/yyyy")) Then rssave("due_date") = Format(mskduedate.Text, "dd/mm/yyyy") & ""
    rssave("plan_name1") = txtplan.Text
    rssave("pay_mode") = txtpaymode.Text
    rssave("prem_freq") = txtpayfreq.Text
    rssave("sa") = txtsa.Text
    rssave("prem_amt") = tctpremamt.Text
    rssave("branch_name") = tctsu.Text
    rssave("branch_add1") = txtsuadd.Text
    rssave("favour_name") = txtchq.Text
    rssave.Update
End If

CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "test", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.WindowShowSearchBtn = True
Sel_Form = "{bajaj_due_data.s_no}=" & sno & ""
CrystalReport1.SelectionFormula = Sel_Form
CrystalReport1.ReportFileName = App.Path & "\lifereports\Manual_Insurance_Renewal Printed.rpt"
CrystalReport1.ParameterFields(0) = "company_name;" & Trim(Left(cmbCompny.Text, 80)) & ";true"
CrystalReport1.ParameterFields(1) = " guserid; " & Glbloginid & "  ; true "
CrystalReport1.WindowShowPrintSetupBtn = True
Call SaveLogIn(Glbloginid, "Transaction>Insurance>Reports", Me.Name)
CrystalReport1.action = 1
CMDRESET_Click
Exit Sub
err:
    MsgBox err.Description & err.Number
End Sub

Private Sub CMDRESET_Click()
    txtcompany.Visible = False
    txtplan.Visible = False
    txtcname.Text = ""
    txtadd1.Text = ""
    txtadd2.Text = ""
    txtadd3.Text = ""
    txtadd4.Text = ""
    txtadd5.Text = ""
    txtcity.Text = ""
    txtstate.Text = ""
    txtpin.Text = ""
    txtphone.Text = ""
    txtphone2.Text = ""
    txtcompany.Text = ""
    txtplan.Text = ""
    txtpaymode.Text = ""
    txtpayfreq.Text = ""
    txtsa.Text = ""
    tctpremamt.Text = ""
    tctsu.Text = ""
    txtsuadd.Text = ""
    txtchq.Text = ""
    cmbCompny.ListIndex = -1
    cmbPlan.ListIndex = -1
    mskduedate.Text = "__/__/____"
    TXTCODE.Text = ""
End Sub

Private Sub cmdSearch_Click()
CMDRESET_Click
On Error GoTo err
Dim RsSearch As New ADODB.Recordset
sql = " SELECT   ar_branch_cd, company_cd, cl_name, "
sql = sql & "            DECODE (c.address1, NULL, cl_add1, c.address1) cl_add1, "
sql = sql & "            DECODE (c.address2, NULL, cl_add2, c.address2) cl_add2, "
sql = sql & "            '' cl_add3,'' cl_add4,'' cl_add5, "
sql = sql & "            DECODE (c.phone1, NULL, c.phone2, c.phone1) cl_phone1, "
sql = sql & "            DECODE (c.mobile, NULL, c.mobile1, c.mobile) cl_phone2, "
sql = sql & "            city_name cl_city, due_date, rem_flage, "
sql = sql & "            (SELECT state_name "
sql = sql & "               FROM state_master "
sql = sql & "              WHERE state_id = a.state_cd) state_name, "
sql = sql & "            (SELECT company_name "
sql = sql & "               FROM bajaj_company_master "
sql = sql & "              WHERE company_cd = a.company_cd) company_name, "
sql = sql & "            (SELECT favour_name "
sql = sql & "               FROM favour_master "
sql = sql & "              WHERE company_cd = a.company_cd) favour_name, "
sql = sql & "            (SELECT branch_name "
sql = sql & "               FROM branch_master "
sql = sql & "              WHERE branch_code = c.sourceid) branch_name, "
sql = sql & "            (SELECT address1 "
sql = sql & "               FROM branch_master "
sql = sql & "              WHERE branch_code = c.sourceid) branch_add1, "
sql = sql & "            (SELECT address2 "
sql = sql & "               FROM branch_master "
sql = sql & "              WHERE branch_code = c.sourceid) branch_add2, "
sql = sql & "            (SELECT PLAN "
sql = sql & "               FROM bajaj_plan_master "
sql = sql & "              WHERE plan_no = a.plan_no) plan_name1, pay_mode, policy_no, "
sql = sql & "            p_name, i_name, "
sql = sql & "            CASE prem_freq "
sql = sql & "               WHEN '1' "
sql = sql & "                  THEN 'ANNUALLY' "
sql = sql & "               WHEN '12' "
sql = sql & "                  THEN 'MONTHLY' "
sql = sql & "               WHEN '4' "
sql = sql & "                  THEN 'QUARTERLY' "
sql = sql & "               WHEN '2' "
sql = sql & "                  THEN 'SEMI-ANNUALLY' "
sql = sql & "            END prem_freq, "
sql = sql & "            bprem_freq, plan_name, "
sql = sql & "            DECODE (sa, "
sql = sql & "                    NULL, (SELECT MAX (sa) "
sql = sql & "                             FROM bajaj_ar_head f "
sql = sql & "                            WHERE f.company_cd = a.company_cd "
sql = sql & "                              AND f.policy_no = a.policy_no), "
sql = sql & "                    sa "
sql = sql & "                   ) sa, "
sql = sql & "            prem_amt, mon_no, year_no, pincode cl_pin, "
sql = sql & "            (SELECT MAX (pin) "
sql = sql & "               FROM bajaj_ar_head "
sql = sql & "              WHERE UPPER (TRIM (company_cd)) = "
sql = sql & "                                             UPPER (TRIM (a.company_cd)) "
sql = sql & "                AND UPPER (TRIM (policy_no)) = UPPER (TRIM (a.policy_no))) "
sql = sql & "                                                                         pin1, "
sql = sql & "            SUBSTR (inv_cd, 1, 8) inv_code, inv_cd AS inv_code1, "
sql = sql & "            importdatatype "
sql = sql & "       FROM bajaj_due_data a, client_master c, employee_master e "
sql = sql & "      WHERE  "
sql = sql & "        c.rm_code = e.rm_code "
sql = sql & "        AND rem_flage IS NULL "
sql = sql & "        AND SUBSTR (a.inv_cd, 1, 8) = c.client_code "
sql = sql & "        AND prem_amt > 0 "
sql = sql & "        and upper(a.POLICY_NO)='" & UCase(Trim(txtpolicy.Text)) & "' "
sql = sql & "        AND sourceid IN ( "
sql = sql & "               SELECT branch_code "
sql = sql & "                 FROM branch_master "
sql = sql & "                WHERE branch_tar_cat <> 186 "
sql = sql & "                  AND category_id NOT IN (1004, 1005, 1006)) "
sql = sql & "   ORDER BY a.DUE_DATE"
RsSearch.open sql, MyConn, adOpenForwardOnly
If RsSearch.EOF = False Then
    cmbCompny.Visible = False
    cmbPlan.Visible = False
    txtcompany.Visible = True
    txtplan.Visible = True
    RsSearch.MoveLast
    TXTCODE.Text = RsSearch("inv_Code") & ""
    txtcname.Text = RsSearch("cl_name") & ""
    txtadd1.Text = RsSearch("cl_add1") & ""
    txtadd2.Text = RsSearch("cl_add2") & ""
    txtadd3.Text = RsSearch("cl_add3") & ""
    txtadd4.Text = RsSearch("cl_add4") & ""
    txtadd5.Text = RsSearch("cl_add5") & ""
    txtcity.Text = RsSearch("cl_city") & ""
    txtstate.Text = RsSearch("state_name") & ""
    txtpin.Text = RsSearch("cl_pin") & ""
    txtphone.Text = RsSearch("cl_phone1") & ""
    txtphone2.Text = RsSearch("cl_phone2") & ""
    txtcompany.Text = RsSearch("company_name") & ""
    If Not IsNull(RsSearch("due_date")) Then mskduedate.Text = Format(RsSearch("due_date"), "dd/mm/yyyy") & ""
    txtplan.Text = RsSearch("plan_name1") & ""
    txtpaymode.Text = RsSearch("pay_mode") & ""
    txtpayfreq.Text = RsSearch("prem_freq") & ""
    txtsa.Text = RsSearch("sa") & ""
    tctpremamt.Text = RsSearch("prem_amt") & ""
    tctsu.Text = RsSearch("branch_name") & ""
    txtsuadd.Text = RsSearch("branch_add1") & " " & RsSearch("branch_add2") & ""
    txtchq.Text = RsSearch("favour_name") & " " & RsSearch("policy_no") & ""
Else
    MsgBox "No data found", vbInformation
    cmbCompny.Visible = True
    cmbPlan.Visible = True
    txtcompany.Visible = False
    txtplan.Visible = False
End If
Exit Sub
err:
    MsgBox (err.Description & err.Number)
End Sub

Private Sub Command1_Click()
Unload Me
End Sub

Private Sub Form_Load()
Image1.Picture = LoadPicture(App.Path & "/LOGO1.JPG")
Me.Icon = LoadPicture(App.Path & "/W.ICO")
Me.Left = 20
Me.Top = 20
Me.width = 8385
Me.Height = 9180
Dim rsBrch As New ADODB.Recordset
Dim rsPrd As New ADODB.Recordset
Dim rsScm As New ADODB.Recordset
Dim rsCompny As New ADODB.Recordset
Dim rsPermi As New ADODB.Recordset
Dim str1 As String
ButtonPermission Me
Dim con
rsCompny.open "select company_cd,company_name from bajaj_company_master where catagory='L' order by company_name", MyConn
    i = 0
    cmbCompny.Clear
    While Not rsCompny.EOF
        cmbCompny.AddItem rsCompny!company_name & Space(50 - Len(rsCompny!company_name)) & rsCompny!COMPANY_CD
        rsCompny.MoveNext
    Wend
    rsCompny.Close
    Set rsCompny = Nothing
Dim rsCommtype As New ADODB.Recordset
rsCommtype.open ("Select comm_id,commission from BAJAJ_comm_type"), MyConn
If Not rsCommtype.EOF Then
    While Not rsCommtype.EOF
           str1 = str1 & rsCommtype("commission") & Space(50 - Len(rsCommtype("commission"))) & rsCommtype("comm_id") & "|"
        rsCommtype.MoveNext
        Wend
End If
str3 = "Premium Term     1|Annulize Premium 2|"
rsCommtype.Close
Dim rsType As New ADODB.Recordset

Set rsPermi = Nothing
Set rsCommtype = Nothing
End Sub



