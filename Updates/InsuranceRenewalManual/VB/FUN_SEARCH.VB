Private Sub cmdSearch_Click()
CMDRESET_Click
On Error GoTo err
Dim RsSearch As New ADODB.Recordset
Sql = " SELECT   ar_branch_cd, company_cd, cl_name, "
Sql = Sql & "            DECODE (c.address1, NULL, cl_add1, c.address1) cl_add1, "
Sql = Sql & "            DECODE (c.address2, NULL, cl_add2, c.address2) cl_add2, "
Sql = Sql & "            '' cl_add3,'' cl_add4,'' cl_add5, "
Sql = Sql & "            DECODE (c.phone1, NULL, c.phone2, c.phone1) cl_phone1, "
Sql = Sql & "            DECODE (c.mobile, NULL, c.mobile1, c.mobile) cl_phone2, "
Sql = Sql & "            city_name cl_city, due_date, rem_flage, "
Sql = Sql & "            (SELECT state_name "
Sql = Sql & "               FROM state_master "
Sql = Sql & "              WHERE state_id = a.state_cd) state_name, "
Sql = Sql & "            (SELECT company_name "
Sql = Sql & "               FROM bajaj_company_master "
Sql = Sql & "              WHERE company_cd = a.company_cd) company_name, "
Sql = Sql & "            (SELECT favour_name "
Sql = Sql & "               FROM favour_master "
Sql = Sql & "              WHERE company_cd = a.company_cd) favour_name, "
Sql = Sql & "            (SELECT branch_name "
Sql = Sql & "               FROM branch_master "
Sql = Sql & "              WHERE branch_code = c.sourceid) branch_name, "
Sql = Sql & "            (SELECT address1 "
Sql = Sql & "               FROM branch_master "
Sql = Sql & "              WHERE branch_code = c.sourceid) branch_add1, "
Sql = Sql & "            (SELECT address2 "
Sql = Sql & "               FROM branch_master "
Sql = Sql & "              WHERE branch_code = c.sourceid) branch_add2, "
Sql = Sql & "            (SELECT PLAN "
Sql = Sql & "               FROM bajaj_plan_master "
Sql = Sql & "              WHERE plan_no = a.plan_no) plan_name1, pay_mode, policy_no, "
Sql = Sql & "            p_name, i_name, "
Sql = Sql & "            CASE prem_freq "
Sql = Sql & "               WHEN '1' "
Sql = Sql & "                  THEN 'ANNUALLY' "
Sql = Sql & "               WHEN '12' "
Sql = Sql & "                  THEN 'MONTHLY' "
Sql = Sql & "               WHEN '4' "
Sql = Sql & "                  THEN 'QUARTERLY' "
Sql = Sql & "               WHEN '2' "
Sql = Sql & "                  THEN 'SEMI-ANNUALLY' "
Sql = Sql & "            END prem_freq, "
Sql = Sql & "            bprem_freq, plan_name, "
Sql = Sql & "            DECODE (sa, "
Sql = Sql & "                    NULL, (SELECT MAX (sa) "
Sql = Sql & "                             FROM bajaj_ar_head f "
Sql = Sql & "                            WHERE f.company_cd = a.company_cd "
Sql = Sql & "                              AND f.policy_no = a.policy_no), "
Sql = Sql & "                    sa "
Sql = Sql & "                   ) sa, "
Sql = Sql & "            prem_amt, mon_no, year_no, pincode cl_pin, "
Sql = Sql & "            (SELECT MAX (pin) "
Sql = Sql & "               FROM bajaj_ar_head "
Sql = Sql & "              WHERE UPPER (TRIM (company_cd)) = "
Sql = Sql & "                                             UPPER (TRIM (a.company_cd)) "
Sql = Sql & "                AND UPPER (TRIM (policy_no)) = UPPER (TRIM (a.policy_no))) "
Sql = Sql & "                                                                         pin1, "
Sql = Sql & "            SUBSTR (inv_cd, 1, 8) inv_code, inv_cd AS inv_code1, "
Sql = Sql & "            importdatatype "
Sql = Sql & "       FROM bajaj_due_data a, client_master c, employee_master e "
Sql = Sql & "      WHERE  "
Sql = Sql & "        c.rm_code = e.rm_code "
Sql = Sql & "        AND rem_flage IS NULL "
Sql = Sql & "        AND SUBSTR (a.inv_cd, 1, 8) = c.client_code "
Sql = Sql & "        AND prem_amt > 0 "
Sql = Sql & "        and upper(a.POLICY_NO)='" & UCase(Trim(txtpolicy.Text)) & "' "
Sql = Sql & "        AND sourceid IN ( "
Sql = Sql & "               SELECT branch_code "
Sql = Sql & "                 FROM branch_master "
Sql = Sql & "                WHERE branch_tar_cat <> 186 "
Sql = Sql & "                  AND category_id NOT IN (1004, 1005, 1006)) "
Sql = Sql & "   ORDER BY a.DUE_DATE"
RsSearch.open Sql, MyConn, adOpenForwardOnly
If RsSearch.EOF = False Then
    cmbCompny.Visible = False
    cmbPlan.Visible = False
    txtcompany.Visible = True
    txtplan.Visible = True
    RsSearch.MoveLast
    txtcode.Text = RsSearch("inv_Code") & ""
    txtcname.Text = RsSearch("cl_name") & ""
    txtadd1.Text = RsSearch("cl_add1") & ""
    txtadd2.Text = RsSearch("cl_add2") & ""
    txtadd3.Text = RsSearch("cl_add3") & ""
    txtadd4.Text = RsSearch("cl_add4") & ""
    txtadd5.Text = RsSearch("cl_add5") & ""
    txtcity.Text = RsSearch("cl_city") & ""
    txtstate.Text = RsSearch("state_name") & ""
    txtpin.Text = RsSearch("cl_pin") & ""
    txtPhone.Text = RsSearch("cl_phone1") & ""
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