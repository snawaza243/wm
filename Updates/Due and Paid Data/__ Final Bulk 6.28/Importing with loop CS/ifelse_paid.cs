     else if (chkOptPaid.Checked)
            {
                foreach (DataRow row in excelData.Rows)
                {
                    if (PsmController.IsContainAndNotNull(row, Excel_Comp))
                    {
                        if (ddlImportDataType.SelectedIndex == 2 || ddlImportDataType.SelectedIndex == 3) // paid and reinstate
                        {
                            string checkLapSql = $@"
SELECT POLICY_NO FROM BAJAJ_DUE_DATA 
WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
AND MON_NO ={Convert.ToInt32(month)} 
AND YEAR_NO={year} 
AND IMPORTDATATYPE='{MyImportDataType}'";

                            DataTable lapTable = pc.ExecuteCurrentQueryMaster(checkLapSql, out int lapRowNum, out string lapException);
                            if (!string.IsNullOrEmpty(lapException))
                            {
                                pc.ShowAlert(this, lapException);
                                return;
                            }
                            if (lapRowNum <= 0)
                            {
                                StringBuilder sqlInsert = new StringBuilder();
                                sqlInsert.Append($"Insert into BAJAJ_DUE_DATA ({dataBaseField},Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG) Values(");

                                for (int Count_Loop = 0; Count_Loop < mskDbFld.Count; Count_Loop++)
                                {
                                    string fieldName = mskExFld[Count_Loop]; // Matching Column Found or not

                                    string fieldValue = row[fieldName].ToString();
                                    if (fieldValue.Contains("'"))
                                    {
                                        fieldValue = fieldValue.Replace("'", "");
                                    }

                                    if (excelData.Columns[fieldName].DataType == typeof(DateTime))
                                    {
                                        DateTime dtValue;
                                        if (DateTime.TryParse(fieldValue, out dtValue))
                                        {
                                            sqlInsert.Append($"To_date('{dtValue.ToString("dd-MMM-yyyy")}','dd/mm/rrrr'),");
                                        }
                                        else
                                        {
                                            sqlInsert.Append("NULL,");
                                        }
                                    }
                                    else
                                    {
                                        sqlInsert.Append($"'{fieldValue.Trim()}',");
                                    }
                                }

                                sqlInsert.Append($"{month}, {year}, '{logginId}', TO_DATE('{curDate}', 'dd/mm/rrrr'), '{MyImportDataType}', NULL, 'FORCE FULL')");
                                string finalSql = sqlInsert.ToString().Replace("''", "Null");

                                /*
                                 Insert into BAJAJ_DUE_DATA 
                                (COMPANY_CD,DOC,            FREQ_UPD,       DUE_DATE,       POLICY_NO,      STATUS_CD,  Mon_no, Year_No,UserId,  Import_Dt,                             ImportDataType, NEWINSERT,  FORCE_FLAG) Values
                                ('BAJAJ A', '20-Mar-2018',  'Half Yearly',  '20-Mar-2025',  '343900831',    'Paid',     1,      2021,   '38387', TO_DATE('19-May-2025', 'dd/mm/rrrr'),  'DUEDATA',      NULL, '     FORCE FULL')
                                 */





                                pc.ExecuteCurrentQueryMaster(finalSql, out int rn1, out string ie1);

                                if (!string.IsNullOrEmpty(ie1))
                                {
                                    pc.ShowAlert(this, ie1);
                                    return;
                                }

                                insertCount += 1;
                            }
                        }









            string checkPaidSql = $@"
SELECT * FROM BAJAJ_PAID_DATA 
WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
AND MON_NO={month} 
AND YEAR_NO={year} 
AND IMPORTDATATYPE='{MyImportDataType}'";

                        DataTable paidTable = pc.ExecuteCurrentQueryMaster(checkPaidSql, out int paidRowNum, out string paidException);

                        if (!string.IsNullOrEmpty(paidException))
                        {
                            pc.ShowAlert(this, paidException);
                            return;
                        }
                        if (paidRowNum <= 0 && paidException != null)
                        {
                            int xy = 0;
                            string headerSql = $@"
SELECT POLICY_NO,COMPANY_CD,LAST_STATUS FROM POLICY_DETAILS_MASTER 
WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";

                            DataTable headerTable = pc.ExecuteCurrentQueryMaster(headerSql, out int headerRowNum, out string headerException);
                            if (headerRowNum > 0)
                            {
                                if (headerTable.Rows[0]["last_status"].ToString().ToUpper() == "L" &&
                                    row[Excel_Status].ToString().ToUpper() == "PAID")
                                {
                                    string updateSql = $@"
UPDATE POLICY_DETAILS_MASTER SET LAST_STATUS='R',
UPDATE_PROG='{ddlImportDataType.Text}',
UPDATE_USER='{logginId}',
UPDATE_DATE=TO_DATE('{curDate}','dd/mm/rrrr') 
WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";

                                    pc.ExecuteCurrentQueryMaster(updateSql, out _, out _);
                                }
                            }
                            else
                            {
                                string insertSql = $@"
INSERT INTO POLICYNOTINHEADER(POLICY_NO,COMPANY_CD) VALUES
(('{row[Excel_policy_no].ToString().ToUpper().Trim()}'),
'{row[Excel_Comp].ToString().ToUpper().Trim()}')";

                                pc.ExecuteCurrentQueryMaster(insertSql, out int insertSqlrn, out string insertSqlie);

                                if (!string.IsNullOrEmpty(insertSqlie))
                                {
                                    pc.ShowAlert(this, insertSqlie);
                                    return;
                                }

                            }

                            string maxDateSql = $@"
SELECT MAX(TO_DATE('01/{month}/{year}','dd/mm/rrrr')) from_dt 
FROM BAJAJ_DUE_DATA 
WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
AND STATUS_CD='LAPSED'";

                            DataTable maxDateTable = pc.ExecuteCurrentQueryMaster(maxDateSql, out int maxDateRowNum, out string maxDateException);

                            if (!string.IsNullOrEmpty(maxDateException))
                            {
                                pc.ShowAlert(this, maxDateException);
                                return;
                            }

                            string updateDueSql;
                            if (maxDateRowNum > 0 && maxDateTable.Rows[0][0] != DBNull.Value)
                            {
                                DateTime maxDate = Convert.ToDateTime(maxDateTable.Rows[0][0]);
                                updateDueSql = $@"
UPDATE BAJAJ_DUE_DATA set 
STATUS_CD='{row[Excel_Status].ToString().ToUpper().Trim()}',
LAST_UPDATE_DT='{ServerDateTime.ToString("dd-MMM-yyyy")}',
LAST_UPDATE='{logginId}' 
WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
AND IMPORTDATATYPE='{MyImportDataType}' 
AND TO_DATE('01/{month}/{year}','dd/mm/rrrr') > TO_DATE('{curDate}', 'dd/mm/rrrr')
AND TO_DATE('01/{month}/{year}','dd/mm/rrrr') <= LAST_DAY(TO_DATE('01/{month}/{year}','dd/mm/rrrr')";

                            }
                            else
                            {
                                updateDueSql = $@"
update bajaj_due_Data set 
status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',
last_update_dt=TO_DATE('{curDate}', 'dd/mm/rrrr')',
last_update='{logginId}' 
WHERE UPPER(TRIM(POLICY_no))=upper(trim('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
and importdatatype='{MyImportDataType}' 
AND mon_no ={month} and year_no={year}";
                            }

                            pc.ExecuteCurrentQueryMaster(updateDueSql, out xy, out string xyIe);

                            if (!string.IsNullOrEmpty(xyIe))
                            {
                                pc.ShowAlert(this, xyIe);
                                return;
                            }
                            if (row[Excel_Status].ToString().ToUpper().Trim() == "PAID" && xy > 0)
                            {
                                StringBuilder paidInsert = new StringBuilder();
                                paidInsert.Append($"Insert into BAJAJ_PAID_DATA ({dataBaseField},Mon_no,Year_No,UserId,Import_Dt,importdatatype) Values(");

                                for (int Count_Loop = 0; Count_Loop < mskDbFld.Count; Count_Loop++)

                                {
                                    string fieldName = mskExFld[Count_Loop]; // Matching Column Found or not

                                    string fieldValue = row[fieldName].ToString();

                                    if (fieldValue.Contains("'"))
                                    {
                                        fieldValue = fieldValue.Replace("'", "");
                                    }

                                    if (excelData.Columns[fieldName].DataType == typeof(DateTime))

                                    {
                                        DateTime dtValue;
                                        if (DateTime.TryParse(fieldValue, out dtValue))
                                        {
                                            paidInsert.Append($"'{dtValue.ToString("dd-MMM-yyyy")}',");
                                        }
                                        else
                                        {
                                            paidInsert.Append("NULL,");
                                        }
                                    }
                                    else
                                    {
                                        paidInsert.Append($"'{fieldValue.Trim()}',");
                                    }
                                }

                                paidInsert.Append($"{month}, {year}, '{logginId}', '{ServerDateTime.ToString("dd-MMM-yyyy")}', '{MyImportDataType}')");
                                string finalPaidSql = paidInsert.ToString().Replace("''", "Null");
                                pc.ExecuteCurrentQueryMaster(finalPaidSql, out int paidSqlRn, out string paidSqlIe);

                                if (!string.IsNullOrEmpty(paidSqlIe))
                                {
                                    pc.ShowAlert(this, paidSqlIe);
                                    return;
                                }
                                Rec_Count++;

                                string updateStatusSql = $"update policy_details_master set last_status='A',UPDATE_PROG='{ddlImportDataType.Text}',UPDATE_USER='{logginId}',UPDATE_DATE=to_date('{ServerDateTime.ToString("dd/MM/yyyy")}','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                                pc.ExecuteCurrentQueryMaster(updateStatusSql, out _, out _);

                                UpdateBajajStatus(row[Excel_policy_no]?.ToString(), row[Excel_Comp]?.ToString(), "F", "PAID DATA", month, year);
                                //lblCount.Text = Rec_Count.ToString();
                            }
                        }
                        else
                        {
                            string updatePaidSql = $@"
UPDATE BAJAJ_PAID_DATA SET STATUS_CD='{row[Excel_Status].ToString().ToUpper().Trim()}',
LAST_UPDATE_DT='{DateTime.Now.ToString("dd-MMM-yyyy")}',
LAST_UPDATE='{logginId}' 
WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
AND MON_NO={month} and year_no={year} 
AND IMPORTDATATYPE='{MyImportDataType}'";
                            pc.ExecuteCurrentQueryMaster(updatePaidSql, out int rnup, out string ieup);

                            //rec1++;
                            //lblRec1.Text = rec1.ToString();
                        }
                    }

                    //Rec_Count_exl++;
                    //lblRecCountExl.Text = Rec_Count_exl.ToString();
                    impCount += 1;
                }

                // Handle duplicate policies
                string dupPolicySql = $@"select policy_no from (select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_paid_Data b where upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = {month} and year_no={year} and importdatatype='{MyImportDataType}' group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(A.company_cd)>1)";
                DataTable dupPolicyTable = pc.ExecuteCurrentQueryMaster(dupPolicySql, out int dupPolicyRowNum, out string dupPolicyException);

                StringBuilder dupPolicyStr = new StringBuilder();
                pc.ExecuteCurrentQueryMaster("DELETE FROM DUP_POLICY", out _, out _);

                foreach (DataRow dupRow in dupPolicyTable.Rows)
                {
                    string insertDupSql = $"INSERT INTO DUP_POLICY VALUES('{dupRow["policy_no"]}')";
                    pc.ExecuteCurrentQueryMaster(insertDupSql, out _, out _);
                    dupPolicyStr.Append($"'{dupRow["policy_no"]}',");
                }

                if (dupPolicyStr.Length > 0)
                {
                    dupPolicyStr.Length--; // Remove last comma
                    string updateDupSql = $"update bajaj_paid_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no ={month} and year_no={year} and importdatatype='{MyImportDataType}'";
                    int recDel = 0;
                    pc.ExecuteCurrentQueryMaster(updateDupSql, out recDel, out _);
                    Rec_Count -= recDel;
                }

                // Update employee and investor codes
                string updateEmpSql = $@"update bajaj_paid_data A set A.emp_no =
        (select MAX(B.emp_no) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1),
        inv_cd=(select MAX(B.client_Cd) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1)
        where mon_no= {month} and year_no= {year} and dup_Rec is null and importdatatype='{MyImportDataType}'";
                pc.ExecuteCurrentQueryMaster(updateEmpSql, out _, out _);

                pc.ShowAlert(this, "Records Imported Successfully");

                return;

            }
