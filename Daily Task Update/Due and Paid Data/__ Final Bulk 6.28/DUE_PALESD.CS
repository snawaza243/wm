if (MyImportDataType == "LAPSEDDATA")
{
    DateTime MyLapsedDate = DateTime.Now;

    #region Select query for LAPSED
    string policyNo = row[Excel_policy_no].ToString().ToUpper().Trim();
    string companyCd = row[Excel_Comp].ToString().ToUpper().Trim();

    string sql = $@"
        SELECT policy_no, company_cd, MAX(due_date) AS due_date, MAX(mon_no), MAX(year_no),
               (
                   SELECT MAX(status_cd)
                   FROM bajaj_due_data
                   WHERE UPPER(TRIM(policy_no)) = UPPER(TRIM('{policyNo}'))
                     AND UPPER(TRIM(company_cd)) = '{companyCd}'
                     AND importdatatype = 'DUEDATA'
                     AND due_date = (
                         SELECT MAX(due_date)
                         FROM bajaj_due_data
                         WHERE UPPER(TRIM(policy_no)) = UPPER(TRIM('{policyNo}'))
                           AND UPPER(TRIM(company_cd)) = '{companyCd}'
                           AND due_date IS NOT NULL
                           AND importdatatype = 'DUEDATA'
                     )
               ) AS status_cd
        FROM bajaj_due_data a
        WHERE importdatatype = 'DUEDATA'
          AND UPPER(TRIM(policy_no)) = UPPER(TRIM('{policyNo}'))
          AND UPPER(TRIM(company_cd)) = '{companyCd}'
        GROUP BY policy_no, company_cd";
    #endregion

    DataTable dt3 = pc.ExecuteCurrentQueryMaster(sql, out int rn3, out string ie3);

    if (rn3 > 0 && ie3 == null)
    {
        #region If found lapsed data then update

        DateTime MyDueDate = DateTime.MinValue;

        object rawDueDate = dt3.Rows[0]["due_date"];
        if (rawDueDate != null && rawDueDate != DBNull.Value)
        {
            DateTime.TryParse(rawDueDate.ToString(), out MyDueDate);
        }

        if (MyLapsedDate >= MyDueDate)
        {
            string dueDateStr = Convert.ToDateTime(row["due_date"]).ToString("dd/MM/yyyy");

            string updateDueDataSql = $@"
                UPDATE bajaj_due_data
                SET status_cd = 'LAPSED',
                    last_update_dt = '{DateTime.Now:dd/MM/yyyy}',
                    last_update = '{logginId}'
                WHERE UPPER(TRIM(policy_no)) = UPPER(TRIM('{policyNo}'))
                  AND UPPER(TRIM(company_cd)) = '{companyCd}'
                  AND due_date = TO_DATE('{dueDateStr}', 'DD/MM/YYYY')
                  AND importdatatype = 'DUEDATA'";

            string updatePolicyMasterSql = $@"
                UPDATE policy_details_master
                SET last_status = 'L',
                    update_prog = '{ddlImportDataType.Text}',
                    update_user = '{logginId}',
                    update_date = TO_DATE('{ServerDateTime:dd-MMM-yyyy}', 'DD/MM/YYYY')
                WHERE UPPER(TRIM(policy_no)) = UPPER(TRIM('{policyNo}'))
                  AND UPPER(TRIM(company_cd)) = '{companyCd}'";

            pc.ExecuteCurrentQueryMaster(updateDueDataSql, out _, out _);
            pc.ExecuteCurrentQueryMaster(updatePolicyMasterSql, out _, out _);

            UpdateBajajStatus(policyNo, companyCd, "L", "LAPSSSSED DATA", month, year);
        }

        #endregion
    }
}
