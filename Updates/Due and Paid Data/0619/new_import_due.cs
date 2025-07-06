public void ProcessBulkDueData(DataTable excelData, int month, int year, string MyImportDataType, 
                              string logginId, string curDate, string ddlImportDataType, 
                              DateTime ServerDateTime, CheckBox chkOptDue)
{
    if (!chkOptDue.Checked) return;

    // Create batches for processing
    const int batchSize = 1000;
    int totalRecords = excelData.Rows.Count;
    int processedRecords = 0;
    int insertCount = 0;
    int updateCount = 0;

    // Create lists for batch processing
    var insertRecords = new List<DataRow>();
    var updateRecords = new List<DataRow>();
    var lapsedPolicies = new List<Tuple<string, string>>();

    // Pre-calculate values that don't change
    string fileNameNe = "DuePaidMappedField" + ddlImportDataType + ".txt";
    string currentDateFormatted = ServerDateTime.ToString("dd-MMM-yyyy");

    // Process records in batches
    for (int i = 0; i < totalRecords; i += batchSize)
    {
        int endIndex = Math.Min(i + batchSize, totalRecords);
        var batch = excelData.AsEnumerable().Skip(i).Take(endIndex - i);

        // Clear batch lists
        insertRecords.Clear();
        updateRecords.Clear();
        lapsedPolicies.Clear();

        // Categorize records
        foreach (DataRow row in batch)
        {
            if (!PsmController.IsContainAndNotNull(row, Excel_Comp)) continue;

            string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
            string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();

            // Check if record exists
            string checkSql = $@"SELECT COUNT(1) FROM BAJAJ_DUE_DATA 
                              WHERE UPPER(TRIM(POLICY_NO)) = :policyNo
                              AND UPPER(TRIM(COMPANY_CD)) = :companyCd
                              AND MON_NO = :month 
                              AND YEAR_NO = :year
                              AND IMPORTDATATYPE = :importType";

            var parameters = new Dictionary<string, object>
            {
                { "policyNo", policyNo },
                { "companyCd", companyCd },
                { "month", month },
                { "year", year },
                { "importType", MyImportDataType }
            };

            DataTable dt = pc.ExecuteParameterizedQuery(checkSql, parameters, out int rowCount, out string error);

            if (rowCount <= 0 && error == null)
            {
                insertRecords.Add(row);
                if (MyImportDataType == "LAPSEDDATA")
                {
                    lapsedPolicies.Add(Tuple.Create(policyNo, companyCd));
                }
            }
            else
            {
                updateRecords.Add(row);
            }
        }

        // Process inserts in bulk
        if (insertRecords.Count > 0)
        {
            insertCount += BulkInsertDueData(insertRecords, month, year, MyImportDataType, 
                                          logginId, curDate, dataBaseField, mskDbFld, mskExFld);
        }

        // Process lapsed policies in bulk
        if (lapsedPolicies.Count > 0)
        {
            ProcessLapsedPolicies(lapsedPolicies, logginId, month, year, currentDateFormatted, 
                                ddlImportDataType, ServerDateTime);
        }

        // Process updates in bulk
        if (updateRecords.Count > 0)
        {
            updateCount += BulkUpdateDueData(updateRecords, month, year, MyImportDataType, 
                                           logginId, fileNameNe, ddlImportDataType, 
                                           ServerDateTime, currentDateFormatted);
        }

        processedRecords += endIndex - i;
        UpdateProgress(processedRecords, totalRecords); // Optional progress reporting
    }

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
        sqlBuilder.Append(", Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT) VALUES (");

        for (int i = 0; i < dbFields.Count; i++)
        {
            string fieldName = excelFields[i];
            string fieldValue = row[fieldName]?.ToString().Trim();

            if (string.IsNullOrEmpty(fieldValue))
            {
                sqlBuilder.Append("NULL,");
                continue;
            }

            if (fieldValue.Contains("'"))
            {
                fieldValue = fieldValue.Replace("'", "''");
            }

            if (row.Table.Columns[fieldName].DataType == typeof(DateTime))
            {
                if (DateTime.TryParse(fieldValue, out DateTime dtValue))
                {
                    sqlBuilder.Append($"TO_DATE('{dtValue.ToString("dd/MMM/yyyy")}','DD/MON/YYYY'),");
                }
                else
                {
                    sqlBuilder.Append("NULL,");
                }
            }
            else
            {
                if (fieldValue.Contains(","))
                {
                    fieldValue = fieldValue.Replace(",", "");
                }
                sqlBuilder.Append($"'{fieldValue}',");
            }
        }

        sqlBuilder.Append($"{month}, {year}, '{loginId}', TO_DATE('{currentDate}','DD/MM/YYYY'), ");
        sqlBuilder.Append($"'{importDataType}', 'BAL') ");
    }

    sqlBuilder.AppendLine("SELECT 1 FROM DUAL");
    string sql = sqlBuilder.ToString();

    pc.ExecuteCurrentQueryMaster(sql, out int rowsAffected, out string error);
    return rowsAffected;
}

private void ProcessLapsedPolicies(List<Tuple<string, string>> policies, string loginId, 
                                 int month, int year, string currentDateFormatted, 
                                 string importType, DateTime serverDateTime)
{
    // Get max due dates for all policies in one query
    string policyConditions = string.Join(" OR ", policies.Select(p => 
        $"(UPPER(TRIM(POLICY_NO)) = UPPER(TRIM('{p.Item1}')) " +
        $"AND UPPER(TRIM(COMPANY_CD)) = UPPER(TRIM('{p.Item2}'))"));

    string sql = $@"SELECT policy_no, company_cd, MAX(due_date) as max_due_date, 
                   MAX(status_cd) as status_cd
                   FROM bajaj_due_data 
                   WHERE importdatatype = 'DUEDATA' AND ({policyConditions})
                   GROUP BY policy_no, company_cd";

    DataTable dt = pc.ExecuteCurrentQueryMaster(sql, out int rowCount, out string error);

    if (rowCount <= 0) return;

    // Build bulk update statements
    var updates = new List<string>();
    var policyUpdates = new List<string>();
    DateTime currentDate = DateTime.Now;

    foreach (DataRow dr in dt.Rows)
    {
        string policyNo = dr["policy_no"].ToString();
        string companyCd = dr["company_cd"].ToString();
        object dueDateObj = dr["max_due_date"];

        if (dueDateObj == DBNull.Value || dueDateObj == null) continue;

        DateTime dueDate;
        if (!DateTime.TryParse(dueDateObj.ToString(), out dueDate)) continue;

        if (currentDate >= dueDate)
        {
            updates.Add($@"UPDATE bajaj_due_Data SET status_Cd='LAPSED',
                        last_update_dt=TO_DATE('{currentDateFormatted}','DD/MM/YYYY'),
                        last_update='{loginId}' 
                        WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{policyNo}')) 
                        AND upper(trim(company_cd))= '{companyCd}' 
                        AND due_date=TO_DATE('{dueDate.ToString("dd/MM/yyyy")}','DD/MM/YYYY') 
                        AND importdatatype='DUEDATA'");

            policyUpdates.Add($@"UPDATE policy_details_master SET last_status='L',
                              UPDATE_PROG='{importType}',
                              UPDATE_USER='{loginId}',
                              UPDATE_DATE=TO_DATE('{serverDateTime.ToString("dd-MMM-yyyy")}','DD-MON-YYYY') 
                              WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{policyNo}')) 
                              AND upper(trim(company_cd))= '{companyCd}'");
        }
    }

    // Execute updates in batches
    if (updates.Count > 0)
    {
        ExecuteBatchSql(updates);
        ExecuteBatchSql(policyUpdates);
    }
}

private int BulkUpdateDueData(List<DataRow> rows, int month, int year, string importDataType,
                            string loginId, string fileName, string importType,
                            DateTime serverDateTime, string currentDateFormatted)
{
    if (importDataType != "DUEDATA") return 0;

    // Group updates by type to minimize SQL statements
    var policyUpdates = new Dictionary<string, List<Tuple<string, string>>>
    {
        ["BASE"] = new List<Tuple<string, string>>(),
        ["PREM_FREQ"] = new List<Tuple<string, string>>(),
        ["DOC"] = new List<Tuple<string, string>>(),
        ["PLY_TERM"] = new List<Tuple<string, string>>(),
        ["MOBILE"] = new List<Tuple<string, string>>(),
        ["SA"] = new List<Tuple<string, string>>(),
        ["PREM_AMT"] = new List<Tuple<string, string>>()
    };

    foreach (DataRow row in rows)
    {
        string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
        string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();

        // Base update (always performed)
        policyUpdates["BASE"].Add(Tuple.Create(policyNo, companyCd));

        // Conditional updates
        if (PsmController.IsContainAndNotNull(row, Excel_Payment_Mode))
        {
            policyUpdates["PREM_FREQ"].Add(Tuple.Create(policyNo, companyCd));
        }
        // Add other conditions similarly...
    }

    // Execute updates
    int totalUpdates = 0;

    // Base update
    if (policyUpdates["BASE"].Count > 0)
    {
        string policyConditions = BuildPolicyConditions(policyUpdates["BASE"]);
        string sql = $@"UPDATE POLICY_DETAILS_MASTER SET 
                       FILE_NAME='{fileName}', 
                       PAYMENT_MODE='{Excel_Payment_Mode}',
                       UPDATE_PROG='{importType}',
                       UPDATE_USER='{loginId}',
                       UPDATE_DATE=TO_DATE('{currentDateFormatted}','DD-MON-YYYY') 
                       WHERE {policyConditions}";

        pc.ExecuteCurrentQueryMaster(sql, out int rowsAffected, out string error);
        totalUpdates += rowsAffected;
    }

    // Add other update types similarly...

    return totalUpdates;
}

private string BuildPolicyConditions(List<Tuple<string, string>> policies)
{
    return string.Join(" OR ", policies.Select(p => 
        $"(UPPER(TRIM(POLICY_no))=UPPER(TRIM('{p.Item1}')) " +
        $"AND UPPER(TRIM(COMPANY_CD))= '{p.Item2}')"));
}

private void ExecuteBatchSql(List<string> sqlStatements)
{
    const int batchSize = 100;
    for (int i = 0; i < sqlStatements.Count; i += batchSize)
    {
        int endIndex = Math.Min(i + batchSize, sqlStatements.Count);
        var batch = sqlStatements.Skip(i).Take(endIndex - i);

        string combinedSql = string.Join(";", batch);
        pc.ExecuteCurrentQueryMaster(combinedSql, out _, out _);
    }
}