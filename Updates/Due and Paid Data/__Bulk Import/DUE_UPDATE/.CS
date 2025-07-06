private int BulkUpdateDueData(List<DataRow> rows, int month, int year, string importDataType,
                            string loginId, string fileName, string importType,
                            DateTime serverDateTime, string currentDateFormatted)
{
    if (importDataType != "DUEDATA") return 0;

    // Group updates by type
    var policyUpdates = new Dictionary<string, List<Tuple<string, string>>>
    {
        ["BASE"] = new List<Tuple<string, string>>(),
        ["PREM_FREQ"] = new List<Tuple<string, string>>(),
        // Other update types...
    };

    // Populate the lists (same as your current code)
    foreach (DataRow row in rows)
    {
        string policyNo = row[Excel_policy_no]?.ToString().Trim().ToUpper();
        string companyCd = row[Excel_Comp]?.ToString().Trim().ToUpper();

        policyUpdates["BASE"].Add(Tuple.Create(policyNo, companyCd));

        if (PsmController.IsContainAndNotNull(row, Excel_Payment_Mode))
        {
            policyUpdates["PREM_FREQ"].Add(Tuple.Create(policyNo, companyCd));
        }
        // Other conditions...
    }

    int totalUpdates = 0;
    
    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        conn.Open();
        OracleTransaction transaction = conn.BeginTransaction();
        
        try
        {
            // 1. Process BASE updates
            if (policyUpdates["BASE"].Count > 0)
            {
                // Load data to temp table
                BulkInsertToTempTable(conn, transaction, "TEMP_POLICY_UPDATES_BASE", policyUpdates["BASE"]);
                
                // Execute update
                string sql = @"UPDATE POLICY_DETAILS_MASTER p
                              SET FILE_NAME = :fileName,
                                  PAYMENT_MODE = :paymentMode,
                                  UPDATE_PROG = :updateProg,
                                  UPDATE_USER = :updateUser,
                                  UPDATE_DATE = TO_DATE(:updateDate, 'DD-MON-YYYY')
                              WHERE EXISTS (
                                  SELECT 1 FROM TEMP_POLICY_UPDATES_BASE t
                                  WHERE p.POLICY_no = t.policy_no
                                  AND p.COMPANY_CD = t.company_cd
                              )";
                
                totalUpdates += ExecuteUpdateWithParams(conn, transaction, sql, 
                    new {
                        fileName,
                        paymentMode = Excel_Payment_Mode,
                        updateProg = importType,
                        updateUser = loginId,
                        updateDate = currentDateFormatted
                    });
                
                // Clear temp table
                ClearTempTable(conn, transaction, "TEMP_POLICY_UPDATES_BASE");
            }

            // 2. Process PREM_FREQ updates (and other types similarly)
            if (policyUpdates["PREM_FREQ"].Count > 0)
            {
                // Similar pattern as above
            }

            transaction.Commit();
            return totalUpdates;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            // Log error
            return 0;
        }
    }
}

// Helper methods
private void BulkInsertToTempTable(OracleConnection conn, OracleTransaction transaction, 
                                 string tableName, List<Tuple<string, string>> data)
{
    // Clear table first
    ClearTempTable(conn, transaction, tableName);
    
    // Use array binding for efficient bulk insert
    using (var cmd = new OracleCommand(
        $"INSERT INTO {tableName} (policy_no, company_cd) VALUES (:1, :2)", conn))
    {
        cmd.Transaction = transaction;
        cmd.ArrayBindCount = data.Count;
        
        var policyNos = data.Select(p => p.Item1).ToArray();
        var companyCds = data.Select(p => p.Item2).ToArray();
        
        cmd.Parameters.Add(":1", OracleDbType.Varchar2, policyNos, ParameterDirection.Input);
        cmd.Parameters.Add(":2", OracleDbType.Varchar2, companyCds, ParameterDirection.Input);
        
        cmd.ExecuteNonQuery();
    }
}

private void ClearTempTable(OracleConnection conn, OracleTransaction transaction, string tableName)
{
    using (var cmd = new OracleCommand($"DELETE FROM {tableName}", conn))
    {
        cmd.Transaction = transaction;
        cmd.ExecuteNonQuery();
    }
}

private int ExecuteUpdateWithParams(OracleConnection conn, OracleTransaction transaction, 
                                  string sql, dynamic parameters)
{
    using (var cmd = new OracleCommand(sql, conn))
    {
        cmd.Transaction = transaction;
        
        // Add parameters dynamically
        var props = ((object)parameters).GetType().GetProperties();
        foreach (var prop in props)
        {
            cmd.Parameters.Add(prop.Name, GetOracleDbType(prop.PropertyType)).Value = prop.GetValue(parameters);
        }
        
        return cmd.ExecuteNonQuery();
    }
}

private OracleDbType GetOracleDbType(Type type)
{
    if (type == typeof(string)) return OracleDbType.Varchar2;
    // Add other type mappings as needed
    return OracleDbType.Varchar2;
}