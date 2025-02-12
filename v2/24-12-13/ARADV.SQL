 SELECT b.client_code, 'P' ar_type, t.tran_code, tr_date, 
          cheque_date cheque_date, cheque_no cheque_no, t.bank_name, amount, 
         b.client_code source_code, app_no, 
         NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, 
         NVL ((SELECT SUM (NVL (amt, 0)) 
                 FROM payment_detail 
                WHERE tran_code = t.tran_code), 0) paidamt, '' asr, payment_mode, 
         (select investor_name from investor_master where inv_code=t.client_code)inv, 
          (SELECT MAX (client_name) 
                               FROM client_master 
                              WHERE client_code = t.source_code) client, 
         exist_code AS existcode, address1 add1, address2 add2, '' loc, 
         pincode pin, 
         (SELECT MAX (city_name) 
            FROM city_master 
           WHERE city_id = (SELECT MAX (city_id) 
                              FROM client_master 
                             WHERE client_code = t.source_code)) ccity, 
         mobile ph, email, 0 arn, '' subbroker, 
         (SELECT rm_name 
            FROM employee_master 
           WHERE payroll_id = TO_CHAR (t.business_rmcode) 
             AND (TYPE = 'A' OR TYPE IS NULL)) rname, 
         (SELECT payroll_id 
            FROM employee_master 
           WHERE payroll_id = TO_CHAR (t.business_rmcode) 
             AND (TYPE = 'A' OR TYPE IS NULL)) rcode, 
         (SELECT branch_name 
            FROM branch_master 
           WHERE branch_code = t.busi_branch_code) bname, 
         (SELECT address1 
            FROM branch_master 
           WHERE branch_code = t.busi_branch_code) badd1, 
         (SELECT address2 
            FROM branch_master 
           WHERE branch_code = t.busi_branch_code) badd2, 
         (SELECT phone 
            FROM branch_master 
           WHERE branch_code = t.busi_branch_code) bphone, 
         (SELECT location_name 
            FROM location_master 
           WHERE location_id = (SELECT location_id 
                                  FROM branch_master 
                                 WHERE branch_code = t.busi_branch_code)) bloc, 
         (SELECT city_name 
            FROM city_master 
           WHERE city_id = (SELECT city_id 
                              FROM branch_master 
                             WHERE branch_code = t.busi_branch_code)) bcity, 
  (SELECT iss_name 
            FROM iss_master 
           WHERE iss_code = t.mut_code 
             AND iss_code NOT IN ( 
                             SELECT DISTINCT iss_code 
                                        FROM iss_master 
                                       WHERE prod_code IN ( 
                                                  SELECT prod_code 
                                                    FROM product_master 
                                                   WHERE nature_code = 
                                                                      'NT004'))) 
                                                                      compmf, 
         'Bajaj Capital Limited' compgroup, 
   (SELECT longname 
            FROM other_product 
           WHERE osch_code = t.sch_code) schmf, 
            
         (SELECT short_name 
            FROM scheme_info 
           WHERE sch_code = t.sch_code) sschmf, ('38387') userid 
    FROM transaction_ST t, client_master b "
If MyPrintSourceId <> "" Then
    Sql = Sql & "    WHERE    tr_date = '" & Format(MyTrDate, "DD-MMM-YYYY") & "' and t.source_code = b.client_code "
Else
    Sql = Sql & "    WHERE    tr_date = '" & Format(ServerDateTime, "DD-MMM-YYYY") & "' and t.source_code = b.client_code "
End If
Sql = Sql & "      AND (asa <> 'C' OR asa IS NULL) "
Sql = Sql & " AND business_rmcode = " & txtbusicode & " "
Sql = Sql & "  AND source_code = " & MySourceId & " "
Sql = Sql & "      AND tran_code = " & MyTranCode & " "
MyConn.Execute Sql
CrystalReport1.Reset
CrystalReport1.Connect = MyConn
CrystalReport1.LogOnServer "pdsodbc.dll", "TEST", "wealthmaker", "wealthmaker", DataBasePassword
CrystalReport1.ReportFileName = App.Path & "\Reports\MFAr3.rpt"
CrystalReport1.WindowState = crptMaximized
CrystalReport1.WindowShowPrintSetupBtn = True
CrystalReport1.WindowShowSearchBtn = True