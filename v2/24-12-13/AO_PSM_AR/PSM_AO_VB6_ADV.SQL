convert thie into oracle sql 2006 "Private Sub cmdprint_Click()
If MyTranCode = "" Then
    MsgBox " Ar Can Not Be Print Right Now Please Generate The AR "
    Exit Sub
End If
If MyPrintSourceId <> "" Then
    MySourceId = MyPrintSourceId
End If

sql = " CREATE OR REPLACE VIEW ARMFADV AS "
sql = sql & "   SELECT b.client_code, 'P' ar_type, t.tran_code, tr_date, "
sql = sql & "           cheque_date cheque_date, cheque_no cheque_no, t.bank_name, amount, "
sql = sql & "          b.client_code source_code, app_no, "
sql = sql & "          NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, "
sql = sql & "          NVL ((SELECT SUM (NVL (amt, 0)) "
sql = sql & "                  FROM payment_detail "
sql = sql & "                 WHERE tran_code = t.tran_code), 0) paidamt, '' asr, payment_mode, "
sql = sql & "          (select investor_name from investor_master where inv_code=t.client_code)inv, "
sql = sql & "           (SELECT MAX (client_name) "
sql = sql & "                                FROM client_master "
sql = sql & "                               WHERE client_code = t.source_code) client, "
sql = sql & "          exist_code AS existcode, address1 add1, address2 add2, '' loc, "
sql = sql & "          pincode pin, "
sql = sql & "          (SELECT MAX (city_name) "
sql = sql & "             FROM city_master "
sql = sql & "            WHERE city_id = (SELECT MAX (city_id) "
sql = sql & "                               FROM client_master "
sql = sql & "                              WHERE client_code = t.source_code)) ccity, "
sql = sql & "          mobile ph, email, 0 arn, '' subbroker, "
sql = sql & "          (SELECT rm_name "
sql = sql & "             FROM employee_master "
sql = sql & "            WHERE payroll_id = TO_CHAR (t.business_rmcode) "
sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rname, "
sql = sql & "          (SELECT payroll_id "
sql = sql & "             FROM employee_master "
sql = sql & "            WHERE payroll_id = TO_CHAR (t.business_rmcode) "
sql = sql & "              AND (TYPE = 'A' OR TYPE IS NULL)) rcode, "
sql = sql & "          (SELECT branch_name "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) bname, "
sql = sql & "          (SELECT address1 "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) badd1, "
sql = sql & "          (SELECT address2 "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) badd2, "
sql = sql & "          (SELECT phone "
sql = sql & "             FROM branch_master "
sql = sql & "            WHERE branch_code = t.busi_branch_code) bphone, "
sql = sql & "          (SELECT location_name "
sql = sql & "             FROM location_master "
sql = sql & "            WHERE location_id = (SELECT location_id "
sql = sql & "                                   FROM branch_master "
sql = sql & "                                  WHERE branch_code = t.busi_branch_code)) bloc, "
sql = sql & "          (SELECT city_name "
sql = sql & "             FROM city_master "
sql = sql & "            WHERE city_id = (SELECT city_id "
sql = sql & "                               FROM branch_master "
sql = sql & "                              WHERE branch_code = t.busi_branch_code)) bcity, "
sql = sql & "   (SELECT iss_name "
sql = sql & "             FROM iss_master "
sql = sql & "            WHERE iss_code = t.mut_code "
sql = sql & "              AND iss_code NOT IN ( "
sql = sql & "                              SELECT DISTINCT iss_code "
sql = sql & "                                         FROM iss_master "
sql = sql & "                                        WHERE prod_code IN ( "
sql = sql & "                                                   SELECT prod_code "
sql = sql & "                                                     FROM product_master "
sql = sql & "                                                    WHERE nature_code = "
sql = sql & "                                                                       'NT004'))) "
sql = sql & "                                                                       compmf, "
sql = sql & "          'Bajaj Capital Limited' compgroup, "
sql = sql & "    (SELECT longname "
sql = sql & "             FROM other_product "
sql = sql & "            WHERE osch_code = t.sch_code) schmf, "
sql = sql & "             "
sql = sql & "          (SELECT short_name "
sql = sql & "             FROM scheme_info "
sql = sql & "            WHERE sch_code = t.sch_code) sschmf, ('38387') userid "
sql = sql & "     FROM transaction_ST t, client_master b "
If MyPrintSourceId <> "" Then
    sql = sql & "    WHERE    tr_date = '" & Format(MyTrDate, "DD-MMM-YYYY") & "' and t.source_code = b.client_code "
Else
    sql = sql & "    WHERE    tr_date = '" & Format(ServerDateTime, "DD-MMM-YYYY") & "' and t.source_code = b.client_code "
End If
sql = sql & "      AND (asa <> 'C' OR asa IS NULL) "
sql = sql & " AND business_rmcode = " & txtbusicode & " "
sql = sql & "  AND source_code = " & MySourceId & " "
sql = sql & "      AND tran_code = " & MyTranCode & " "
MyConn.Execute sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "TEST", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\MFAr3.rpt"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True
CrystalReport1.WindowShowPrintBtn = True
CrystalReport1.action = 1
End Sub"