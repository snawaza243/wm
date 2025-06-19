public void ProcessBulkPaidData(DataTable excelData, int month, int year, string MyImportDataType, 
                               string logginId, string curDate, string ddlImportDataType, 
                               DateTime ServerDateTime, CheckBox chkOptPaid)
{
    if (!chkOptPaid.Checked) return;

    const int batchSize = 1000;
    int totalRecords = excelData.Rows.Count;
    int processedRecords = 0;
    int insertCount = 0;
    int updateCount = 0;

    // Pre-calculate values that don't change
    bool isPaidOrReinstate = ddlImportDataType.SelectedIndex == 2 || ddlImportDataType.SelectedIndex == 3;
    string currentDateFormatted = ServerDateTime.ToString("dd-MMM-yyyy");

    // Process records in batches
    for (int i = 0; i < totalRecords; i += batchSize)
    {
        int endIndex = Math.Min(i + batchSize, totalRecords);
        var batch = excelData.AsEnumerable().Skip(i).Take(endIndex - i);

        // Lists for batch operations
        var dueDataToInsert = new List<DataRow>();
        var paidDataToInsert = new List<DataRow>();
        var paidDataToUpdate = new List<DataRow>();
        var policiesToReinstate = new List<Tuple<string, string>>();
        var policiesNotFound = new List<Tuple<string, string>>();
        var lapsedPolicies = new List<Tuple<string, string, string>>(); // policyNo, companyCd, status

        foreach (DataRow row in batch)
        {
            if (!PsmController.IsContainAndNotNull(row, Excel_Comp)) continue;

            string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
            string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();
            string status = row[Excel_Status]?.ToString().Trim().ToUpper();

            // Check for due data (only for paid/reinstate)
            if (isPaidOrReinstate)
            {
                string checkDueSql = $@"SELECT COUNT(1) FROM BAJAJ_DUE_DATA 
                                    WHERE UPPER(TRIM(POLICY_NO)) = :policyNo
                                    AND UPPER(TRIM(COMPANY_CD)) = :companyCd
                                    AND MON_NO = :month 
                                    AND YEAR_NO = :year
                                    AND IMPORTDATATYPE = :importType";

                var dueParams = new Dictionary<string, object>
                {
                    { "policyNo", policyNo },
                    { "companyCd", companyCd },
                    { "month", month },
                    { "year", year },
                    { "importType", MyImportDataType }
                };

                DataTable dueTable = pc.ExecuteParameterizedQuery(checkDueSql, dueParams, out int dueRowCount, out string dueError);

                if (dueRowCount <= 0 && dueError == null)
                {
                    dueDataToInsert.Add(row);
                }
            }

            // Check paid data
            string checkPaidSql = $@"SELECT COUNT(1) FROM BAJAJ_PAID_DATA 
                                   WHERE UPPER(TRIM(POLICY_NO)) = :policyNo
                                   AND UPPER(TRIM(COMPANY_CD)) = :companyCd
                                   AND MON_NO = :month 
                                   AND YEAR_NO = :year
                                   AND IMPORTDATATYPE = :importType";

            var paidParams = new Dictionary<string, object>
            {
                { "policyNo", policyNo },
                { "companyCd", companyCd },
                { "month", month },
                { "year", year },
                { "importType", MyImportDataType }
            };

            DataTable paidTable = pc.ExecuteParameterizedQuery(checkPaidSql, paidParams, out int paidRowCount, out string paidError);

            if (paidRowCount <= 0 && paidError == null)
            {
                paidDataToInsert.Add(row);
                
                // Check policy header
                string headerSql = $@"SELECT POLICY_NO, COMPANY_CD, LAST_STATUS 
                                   FROM POLICY_DETAILS_MASTER 
                                   WHERE UPPER(TRIM(POLICY_NO)) = :policyNo
                                   AND UPPER(TRIM(COMPANY_CD)) = :companyCd";

                var headerParams = new Dictionary<string, object>
                {
                    { "policyNo", policyNo },
                    { "companyCd", companyCd }
                };

                DataTable headerTable = pc.ExecuteParameterizedQuery(headerSql, headerParams, out int headerRowCount, out string headerError);

                if (headerRowCount > 0)
                {
                    string lastStatus = headerTable.Rows[0]["LAST_STATUS"]?.ToString().ToUpper();
                    if (lastStatus == "L" && status == "PAID")
                    {
                        policiesToReinstate.Add(Tuple.Create(policyNo, companyCd));
                    }
                }
                else
                {
                    policiesNotFound.Add(Tuple.Create(policyNo, companyCd));
                }

                // Check for lapsed status
                if (status == "PAID")
                {
                    lapsedPolicies.Add(Tuple.Create(policyNo, companyCd, status));
                }
            }
            else
            {
                paidDataToUpdate.Add(row);
            }
        }

        // Process batch operations
        if (dueDataToInsert.Count > 0 && isPaidOrReinstate)
        {
            insertCount += BulkInsertDueData(dueDataToInsert, month, year, MyImportDataType, 
                                           logginId, curDate, dataBaseField, mskDbFld, mskExFld);
        }

        if (policiesToReinstate.Count > 0)
        {
            BulkReinstatePolicies(policiesToReinstate, logginId, ddlImportDataType.Text, curDate);
        }

        if (policiesNotFound.Count > 0)
        {
            BulkInsertMissingPolicies(policiesNotFound);
        }

        if (lapsedPolicies.Count > 0)
        {
            BulkUpdateLapsedPolicies(lapsedPolicies, month, year, MyImportDataType, 
                                   logginId, currentDateFormatted);
        }

        if (paidDataToInsert.Count > 0)
        {
            insertCount += BulkInsertPaidData(paidDataToInsert, month, year, MyImportDataType, 
                                            logginId, currentDateFormatted, dataBaseField, 
                                            mskDbFld, mskExFld);
        }

        if (paidDataToUpdate.Count > 0)
        {
            updateCount += BulkUpdatePaidData(paidDataToUpdate, month, year, MyImportDataType, 
                                            logginId, currentDateFormatted);
        }

        processedRecords += endIndex - i;
        UpdateProgress(processedRecords, totalRecords);
    }

    // Process duplicate policies
    ProcessDuplicatePolicies(month, year, MyImportDataType);

    // Update employee and investor codes
    UpdateEmployeeInvestorCodes(month, year, MyImportDataType);

    string returnMsg = $"Total {totalRecords} record(s) processed: {insertCount} inserted, {updateCount} updated.";
    pc.ShowAlert(this, returnMsg);
}

private int BulkInsertDueData(List<DataRow> rows, int month, int year, string importDataType, 
                            string loginId, string currentDate, string dataBaseField, 
                            List<string> dbFields, List<string> excelFields)
{
    StringBuilder sqlBuilder = new StringBuilder();
    sqlBuilder.AppendLine("INSERT ALL");

    foreach (DataRow row in rows)
    {
        sqlBuilder.Append("INTO BAJAJ_DUE_DATA (");
        sqlBuilder.Append(dataBaseField);
        sqlBuilder.Append(", Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT, FORCE_FLAG) VALUES (");

        for (int i = 0; i < dbFields.Count; i++)
        {
            string fieldName = excelFields[i];
            string fieldValue = row[fieldName]?.ToString().Trim();

            if (string.IsNullOrEmpty(fieldValue))
            {
                sqlBuilder.Append("NULL,");
                continue;
            }

            fieldValue = fieldValue.Replace("'", "''");

            if (row.Table.Columns[fieldName].DataType == typeof(DateTime))
            {
                if (DateTime.TryParse(fieldValue, out DateTime dtValue))
                {
                    sqlBuilder.Append($"TO_DATE('{dtValue.ToString("dd-MMM-yyyy")}','DD-MON-YYYY'),");
                }
                else
                {
                    sqlBuilder.Append("NULL,");
                }
            }
            else
            {
                fieldValue = fieldValue.Replace(",", "");
                sqlBuilder.Append($"'{fieldValue}',");
            }
        }

        sqlBuilder.Append($"{month}, {year}, '{loginId}', TO_DATE('{currentDate}','DD/MM/YYYY'), ");
        sqlBuilder.Append($"'{importDataType}', NULL, 'FORCE FULL') ");
    }

    sqlBuilder.AppendLine("SELECT 1 FROM DUAL");
    string sql = sqlBuilder.ToString();

    pc.ExecuteCurrentQueryMaster(sql, out int rowsAffected, out string error);
    return rowsAffected;
}

private void BulkReinstatePolicies(List<Tuple<string, string>> policies, string loginId, 
                                 string importType, string currentDate)
{
    string policyConditions = string.Join(" OR ", policies.Select(p => 
        $"(UPPER(TRIM(POLICY_NO)) = UPPER(TRIM('{p.Item1}')) " +
        $"AND UPPER(TRIM(COMPANY_CD)) = UPPER(TRIM('{p.Item2}'))"));

    string sql = $@"UPDATE POLICY_DETAILS_MASTER SET 
                   LAST_STATUS='R',
                   UPDATE_PROG='{importType}',
                   UPDATE_USER='{loginId}',
                   UPDATE_DATE=TO_DATE('{currentDate}','DD/MM/YYYY') 
                   WHERE {policyConditions}";

    pc.ExecuteCurrentQueryMaster(sql, out _, out _);
}

private void BulkInsertMissingPolicies(List<Tuple<string, string>> policies)
{
    // First clear the table
    pc.ExecuteCurrentQueryMaster("DELETE FROM POLICYNOTINHEADER", out _, out _);

    // Then insert all missing policies in one operation
    StringBuilder sqlBuilder = new StringBuilder();
    sqlBuilder.AppendLine("INSERT ALL");

    foreach (var policy in policies)
    {
        sqlBuilder.AppendLine($"INTO POLICYNOTINHEADER(POLICY_NO, COMPANY_CD) VALUES ('{policy.Item1}', '{policy.Item2}')");
    }

    sqlBuilder.AppendLine("SELECT 1 FROM DUAL");
    string sql = sqlBuilder.ToString();

    pc.ExecuteCurrentQueryMaster(sql, out _, out _);
}

private void BulkUpdateLapsedPolicies(List<Tuple<string, string, string>> policies, int month, int year, 
                                    string importDataType, string loginId, string currentDate)
{
    // Get max dates for all policies in one query
    string policyConditions = string.Join(" OR ", policies.Select(p => 
        $"(UPPER(TRIM(POLICY_NO)) = UPPER(TRIM('{p.Item1}')) " +
        $"AND UPPER(TRIM(COMPANY_CD)) = UPPER(TRIM('{p.Item2}'))"));

    string maxDateSql = $@"SELECT POLICY_NO, COMPANY_CD, MAX(TO_DATE('01/{month}/{year}','DD/MM/YYYY')) as max_date
                        FROM BAJAJ_DUE_DATA 
                        WHERE STATUS_CD='LAPSED' AND ({policyConditions})
                        GROUP BY POLICY_NO, COMPANY_CD";

    DataTable maxDateTable = pc.ExecuteCurrentQueryMaster(maxDateSql, out _, out _);

    // Build update statements
    var updates = new List<string>();
    var statusUpdates = new List<string>();

    foreach (DataRow dr in maxDateTable.Rows)
    {
        string policyNo = dr["POLICY_NO"].ToString();
        string companyCd = dr["COMPANY_CD"].ToString();
        object maxDateObj = dr["max_date"];

        if (maxDateObj == DBNull.Value) continue;

        DateTime maxDate;
        if (!DateTime.TryParse(maxDateObj.ToString(), out maxDate)) continue;

        string status = policies.First(p => p.Item1 == policyNo && p.Item2 == companyCd).Item3;

        if (maxDate > DateTime.MinValue)
        {
            updates.Add($@"UPDATE BAJAJ_DUE_DATA SET 
                        STATUS_CD='{status}',
                        LAST_UPDATE_DT=TO_DATE('{currentDate}','DD-MON-YYYY'),
                        LAST_UPDATE='{loginId}' 
                        WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{policyNo}')) 
                        AND UPPER(TRIM(COMPANY_CD))=UPPER(TRIM('{companyCd}')) 
                        AND IMPORTDATATYPE='{importDataType}' 
                        AND TO_DATE('01/{month}/{year}','DD/MM/YYYY') > TO_DATE('{currentDate}', 'DD/MM/YYYY')");
        }
        else
        {
            updates.Add($@"UPDATE BAJAJ_DUE_DATA SET 
                        STATUS_CD='{status}',
                        LAST_UPDATE_DT=TO_DATE('{currentDate}','DD-MON-YYYY'),
                        LAST_UPDATE='{loginId}' 
                        WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{policyNo}')) 
                        AND UPPER(TRIM(COMPANY_CD))=UPPER(TRIM('{companyCd}')) 
                        AND IMPORTDATATYPE='{importDataType}' 
                        AND MON_NO={month} AND YEAR_NO={year}");
        }

        statusUpdates.Add($@"UPDATE POLICY_DETAILS_MASTER SET 
                            LAST_STATUS='A',
                            UPDATE_PROG='{ddlImportDataType.Text}',
                            UPDATE_USER='{loginId}',
                            UPDATE_DATE=TO_DATE('{currentDate}','DD-MON-YYYY') 
                            WHERE UPPER(TRIM(POLICY_NO))=UPPER(TRIM('{policyNo}')) 
                            AND UPPER(TRIM(COMPANY_CD))=UPPER(TRIM('{companyCd}'))");
    }

    // Execute updates in batches
    if (updates.Count > 0)
    {
        ExecuteBatchSql(updates);
        ExecuteBatchSql(statusUpdates);
    }
}

private int BulkInsertPaidData(List<DataRow> rows, int month, int year, string importDataType, 
                             string loginId, string currentDate, string dataBaseField, 
                             List<string> dbFields, List<string> excelFields)
{
    StringBuilder sqlBuilder = new StringBuilder();
    sqlBuilder.AppendLine("INSERT ALL");

    foreach (DataRow row in rows)
    {
        sqlBuilder.Append("INTO BAJAJ_PAID_DATA (");
        sqlBuilder.Append(dataBaseField);
        sqlBuilder.Append(", Mon_no, Year_No, UserId, Import_Dt, ImportDataType) VALUES (");

        for (int i = 0; i < dbFields.Count; i++)
        {
            string fieldName = excelFields[i];
            string fieldValue = row[fieldName]?.ToString().Trim();

            if (string.IsNullOrEmpty(fieldValue))
            {
                sqlBuilder.Append("NULL,");
                continue;
            }

            fieldValue = fieldValue.Replace("'", "''");

            if (row.Table.Columns[fieldName].DataType == typeof(DateTime))
            {
                if (DateTime.TryParse(fieldValue, out DateTime dtValue))
                {
                    sqlBuilder.Append($"TO_DATE('{dtValue.ToString("dd-MMM-yyyy")}','DD-MON-YYYY'),");
                }
                else
                {
                    sqlBuilder.Append("NULL,");
                }
            }
            else
            {
                fieldValue = fieldValue.Replace(",", "");
                sqlBuilder.Append($"'{fieldValue}',");
            }
        }

        sqlBuilder.Append($"{month}, {year}, '{loginId}', TO_DATE('{currentDate}','DD-MON-YYYY'), '{importDataType}') ");
    }

    sqlBuilder.AppendLine("SELECT 1 FROM DUAL");
    string sql = sqlBuilder.ToString();

    pc.ExecuteCurrentQueryMaster(sql, out int rowsAffected, out string error);
    return rowsAffected;
}

private int BulkUpdatePaidData(List<DataRow> rows, int month, int year, string importDataType, 
                             string loginId, string currentDate)
{
    var policyUpdates = new Dictionary<string, List<Tuple<string, string>>>();

    foreach (DataRow row in rows)
    {
        string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
        string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();
        string status = row[Excel_Status]?.ToString().Trim().ToUpper();

        if (!policyUpdates.ContainsKey(status))
        {
            policyUpdates[status] = new List<Tuple<string, string>>();
        }
        policyUpdates[status].Add(Tuple.Create(policyNo, companyCd));
    }

    int totalUpdates = 0;

    foreach (var kvp in policyUpdates)
    {
        string status = kvp.Key;
        var policies = kvp.Value;

        string policyConditions = string.Join(" OR ", policies.Select(p => 
            $"(UPPER(TRIM(POLICY_NO)) = UPPER(TRIM('{p.Item1}')) " +
            $"AND UPPER(TRIM(COMPANY_CD)) = UPPER(TRIM('{p.Item2}'))"));

        string sql = $@"UPDATE BAJAJ_PAID_DATA SET 
                       STATUS_CD='{status}',
                       LAST_UPDATE_DT=TO_DATE('{currentDate}','DD-MON-YYYY'),
                       LAST_UPDATE='{loginId}' 
                       WHERE ({policyConditions})
                       AND MON_NO={month} 
                       AND YEAR_NO={year} 
                       AND IMPORTDATATYPE='{importDataType}'";

        pc.ExecuteCurrentQueryMaster(sql, out int rowsAffected, out string error);
        totalUpdates += rowsAffected;
    }

    return totalUpdates;
}

private void ProcessDuplicatePolicies(int month, int year, string importDataType)
{
    // First clear the table
    pc.ExecuteCurrentQueryMaster("DELETE FROM DUP_POLICY", out _, out _);

    // Find duplicates
    string dupPolicySql = $@"INSERT INTO DUP_POLICY
                           SELECT policy_no FROM (
                               SELECT a.policy_no, a.sys_ar_Dt, a.company_Cd 
                               FROM bajaj_Ar_head a, bajaj_paid_Data b 
                               WHERE upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) 
                               AND a.Policy_No = B.Policy_No 
                               AND mon_no = {month} 
                               AND year_no = {year} 
                               AND importdatatype = '{importDataType}' 
                               GROUP BY a.policy_no, a.sys_ar_dt, a.company_Cd 
                               HAVING count(a.policy_no) > 1 
                               AND count(a.sys_ar_dt) > 1 
                               AND count(A.company_cd) > 1
                           )";

    pc.ExecuteCurrentQueryMaster(dupPolicySql, out int dupCount, out _);

    if (dupCount > 0)
    {
        string updateDupSql = $@"UPDATE bajaj_paid_Data 
                               SET dup_rec = 'Y' 
                               WHERE policy_no IN (SELECT POLICY_NO FROM DUP_POLICY) 
                               AND mon_no = {month} 
                               AND year_no = {year} 
                               AND importdatatype = '{importDataType}'";
        pc.ExecuteCurrentQueryMaster(updateDupSql, out int recDel, out _);
    }
}

private void UpdateEmployeeInvestorCodes(int month, int year, string importDataType)
{
    string updateSql = $@"UPDATE bajaj_paid_data A SET 
                        A.emp_no = (
                            SELECT MAX(B.emp_no) 
                            FROM bajaj_ar_head B 
                            WHERE upper(trim(B.Company_cd)) = upper(trim(A.Company_cd)) 
                            AND B.policy_no = A.policy_no 
                            AND B.FRESH_RENEWAL = 1
                        ),
                        inv_cd = (
                            SELECT MAX(B.client_Cd) 
                            FROM bajaj_ar_head B 
                            WHERE upper(trim(B.Company_cd)) = upper(trim(A.Company_cd)) 
                            AND B.policy_no = A.policy_no 
                            AND B.FRESH_RENEWAL = 1
                        )
                        WHERE mon_no = {month} 
                        AND year_no = {year} 
                        AND dup_Rec IS NULL 
                        AND importdatatype = '{importDataType}'";
    
    pc.ExecuteCurrentQueryMaster(updateSql, out _, out _);
}

private void ExecuteBatchSql(List<string> sqlStatements)
{
    const int sqlBatchSize = 50; // Adjust based on your database's maximum allowed batch size
    for (int i = 0; i < sqlStatements.Count; i += sqlBatchSize)
    {
        int endIndex = Math.Min(i + sqlBatchSize, sqlStatements.Count);
        var batch = sqlStatements.Skip(i).Take(endIndex - i);

        string combinedSql = string.Join(";", batch);
        pc.ExecuteCurrentQueryMaster(combinedSql, out _, out _);
    }
}