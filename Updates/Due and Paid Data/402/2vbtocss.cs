else if (chkOptPaid.Checked)
{
    foreach (DataRow row in excelData.Rows)
    {
        if (!string.IsNullOrEmpty(row[Excel_Comp]?.ToString()))
        {
            if (ddlImportDataType.SelectedIndex == 2 || ddlImportDataType.SelectedIndex == 3) // paid and reinstate
            {
                string checkLapSql = $"select POLICY_NO from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and mon_no ={monthValue} and year_no={yearValue} AND IMPORTDATATYPE='{MyImportDataType}'";
                DataTable lapTable = pc.ExecuteCurrentQueryMaster(checkLapSql, out int lapRowNum, out string lapException);

                if (lapRowNum <= 0)
                {
                    StringBuilder sqlInsert = new StringBuilder();
                    sqlInsert.Append($"Insert into BAJAJ_DUE_DATA ({dataBaseField},Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG) Values(");

                    foreach (DataColumn col in excelData.Columns)
                    {
                        string fieldValue = row[col.ColumnName].ToString();
                        if (fieldValue.Contains("'"))
                        {
                            fieldValue = fieldValue.Replace("'", "");
                        }

                        if (col.DataType == typeof(DateTime))
                        {
                            DateTime dtValue;
                            if (DateTime.TryParse(fieldValue, out dtValue))
                            {
                                sqlInsert.Append($"'{dtValue.ToString("dd-MMM-yyyy")}',");
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

                    sqlInsert.Append($"{monthValue}, {yearValue}, '{logginId}', '{ServerDateTime.ToString("dd-MMM-yyyy")}', '{MyImportDataType}', NULL, 'FORCE FULL')");
                    string finalSql = sqlInsert.ToString().Replace("''", "Null");
                    pc.ExecuteCurrentQueryMaster(finalSql, out _, out _);
                }
            }

            string checkPaidSql = $"select * from bajaj_paid_Data WHERE upper(trim(POLICY_no))=upper(trim('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and mon_no={monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}'";
            DataTable paidTable = pc.ExecuteCurrentQueryMaster(checkPaidSql, out int paidRowNum, out string paidException);

            if (paidRowNum <= 0)
            {
                int xy = 0;
                string headerSql = $"Select policy_no,company_cd,last_status from policy_details_master WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                DataTable headerTable = pc.ExecuteCurrentQueryMaster(headerSql, out int headerRowNum, out string headerException);

                if (headerRowNum > 0)
                {
                    if (headerTable.Rows[0]["last_status"].ToString().ToUpper() == "L" &&
                        row[Excel_Status].ToString().ToUpper() == "PAID")
                    {
                        string updateSql = $"update policy_details_master set last_status='R',UPDATE_PROG='{ddlImportDataType.Text}',UPDATE_USER='{logginId}',UPDATE_DATE=to_date('{ServerDateTime.ToString("dd/MM/yyyy")}','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                        pc.ExecuteCurrentQueryMaster(updateSql, out _, out _);
                    }
                }
                else
                {
                    string insertSql = $"insert into PolicyNotInHeader(policy_no,company_Cd) values(('{row[Excel_policy_no].ToString().ToUpper().Trim()}'),'{row[Excel_Comp].ToString().ToUpper().Trim()}')";
                    pc.ExecuteCurrentQueryMaster(insertSql, out _, out _);
                }

                string maxDateSql = $"select max(to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')) from_dt from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and status_cd='LAPSED'";
                DataTable maxDateTable = pc.ExecuteCurrentQueryMaster(maxDateSql, out int maxDateRowNum, out string maxDateException);

                string updateDueSql;
                if (maxDateRowNum > 0 && maxDateTable.Rows[0][0] != DBNull.Value)
                {
                    DateTime maxDate = Convert.ToDateTime(maxDateTable.Rows[0][0]);
                    updateDueSql = $"update bajaj_due_Data set status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',last_update_dt='{ServerDateTime.ToString("dd-MMM-yyyy")}',last_update='{logginId}' WHERE UPPER(TRIM(POLICY_no))=upper(trim('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and importdatatype='{MyImportDataType}' AND to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')>'{maxDate.ToString("dd-MMM-yyyy")}' and to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')<=last_day(to_date('01-{monthValue}-{yearValue}','dd-mm-yyyy'))";
                }
                else
                {
                    updateDueSql = $"update bajaj_due_Data set status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',last_update_dt='{ServerDateTime.ToString("dd-MMM-yyyy")}',last_update='{logginId}' WHERE UPPER(TRIM(POLICY_no))=upper(trim('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and importdatatype='{MyImportDataType}' AND mon_no ={monthValue} and year_no={yearValue}";
                }

                pc.ExecuteCurrentQueryMaster(updateDueSql, out xy, out _);

                if (row[Excel_Status].ToString().ToUpper().Trim() == "PAID" && xy > 0)
                {
                    StringBuilder paidInsert = new StringBuilder();
                    paidInsert.Append($"Insert into BAJAJ_PAID_DATA ({dataBaseField},Mon_no,Year_No,UserId,Import_Dt,importdatatype) Values(");

                    foreach (DataColumn col in excelData.Columns)
                    {
                        string fieldValue = row[col.ColumnName].ToString();
                        if (fieldValue.Contains("'"))
                        {
                            fieldValue = fieldValue.Replace("'", "");
                        }

                        if (col.DataType == typeof(DateTime))
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

                    paidInsert.Append($"{monthValue}, {yearValue}, '{logginId}', '{ServerDateTime.ToString("dd-MMM-yyyy")}', '{MyImportDataType}')");
                    string finalPaidSql = paidInsert.ToString().Replace("''", "Null");
                    pc.ExecuteCurrentQueryMaster(finalPaidSql, out _, out _);

                    Rec_Count++;

                    string updateStatusSql = $"update policy_details_master set last_status='A',UPDATE_PROG='{ddlImportDataType.Text}',UPDATE_USER='{logginId}',UPDATE_DATE=to_date('{ServerDateTime.ToString("dd/MM/yyyy")}','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                    pc.ExecuteCurrentQueryMaster(updateStatusSql, out _, out _);

                    UpdateBajajStatus(row[Excel_policy_no]?.ToString(), row[Excel_Comp]?.ToString(), "F", "PAID DATA", monthValue, yearValue);
                    //lblCount.Text = Rec_Count.ToString();
                }
            }
            else
            {
                string updatePaidSql = $"update bajaj_paid_Data set status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',last_update_dt='{DateTime.Now.ToString("dd-MMM-yyyy")}',last_update='{logginId}' WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and mon_no={monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}'";
                pc.ExecuteCurrentQueryMaster(updatePaidSql, out _, out _);

                rec1++;
                //lblRec1.Text = rec1.ToString();
            }
        }

        //Rec_Count_exl++;
        //lblRecCountExl.Text = Rec_Count_exl.ToString();
    }

    // Handle duplicate policies
    string dupPolicySql = $"select policy_no from (select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_paid_Data b where upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = {monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}' group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(A.company_cd)>1)";
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
        string updateDupSql = $"update bajaj_paid_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no ={monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}'";
        int recDel = 0;
        pc.ExecuteCurrentQueryMaster(updateDupSql, out recDel, out _);
        Rec_Count -= recDel;
    }

    // Update employee and investor codes
    string updateEmpSql = $@"update bajaj_paid_data A set A.emp_no =
(select MAX(B.emp_no) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1),
inv_cd=(select MAX(B.client_Cd) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1)
where mon_no= {monthValue} and year_no= {yearValue} and dup_Rec is null and importdatatype='{MyImportDataType}'";
    pc.ExecuteCurrentQueryMaster(updateEmpSql, out _, out _);

     


}

