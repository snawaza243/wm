private void ProcessLapsedPolicies(List<Tuple<string, string>> policies, string loginId,
                                 int month, int year, string currentDateFormatted,
                                 string importType, DateTime serverDateTime)
{
    if (policies.Count == 0) return;

    using (OracleConnection conn = new OracleConnection(connectionString))
    {
        conn.Open();
        OracleTransaction transaction = conn.BeginTransaction();

        try
        {
            // 1. Load policies into temp table
            BulkInsertToTempTable(conn, transaction, "TEMP_LAPSED_POLICIES", policies);

            // 2. Get max due dates in single query
            DataTable lapsedPolicies = GetLapsedPoliciesData(conn, transaction);

            if (lapsedPolicies.Rows.Count == 0)
            {
                transaction.Commit();
                return;
            }

            // 3. Process lapsed policies
            ProcessLapsedUpdates(conn, transaction, lapsedPolicies, loginId, 
                                currentDateFormatted, importType, serverDateTime);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            // Log error
            throw;
        }
    }
}

private DataTable GetLapsedPoliciesData(OracleConnection conn, OracleTransaction transaction)
{
    string sql = @"SELECT b.policy_no, b.company_cd, b.due_date, b.status_cd
                   FROM (
                       SELECT policy_no, company_cd, MAX(due_date) as max_due_date
                       FROM bajaj_due_data 
                       WHERE importdatatype = 'DUEDATA'
                       AND (policy_no, company_cd) IN (
                           SELECT policy_no, company_cd FROM TEMP_LAPSED_POLICIES
                       )
                       GROUP BY policy_no, company_cd
                   ) max_dates
                   JOIN bajaj_due_data b ON b.policy_no = max_dates.policy_no 
                                       AND b.company_cd = max_dates.company_cd
                                       AND b.due_date = max_dates.max_due_date";

    using (OracleCommand cmd = new OracleCommand(sql, conn))
    {
        cmd.Transaction = transaction;
        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
        {
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}

private void ProcessLapsedUpdates(OracleConnection conn, OracleTransaction transaction,
                                DataTable lapsedPolicies, string loginId,
                                string currentDateFormatted, string importType,
                                DateTime serverDateTime)
{
    // 1. Prepare lapsed policy data
    var lapsedUpdates = new List<Tuple<string, string, DateTime>>();
    DateTime currentDate = DateTime.Now;

    foreach (DataRow dr in lapsedPolicies.Rows)
    {
        string policyNo = dr["policy_no"].ToString();
        string companyCd = dr["company_cd"].ToString();
        DateTime dueDate = Convert.ToDateTime(dr["due_date"]);

        if (currentDate >= dueDate)
        {
            lapsedUpdates.Add(Tuple.Create(policyNo, companyCd, dueDate));
        }
    }

    if (lapsedUpdates.Count == 0) return;

    // 2. Update bajaj_due_data (status to LAPSED)
    string updateDueDataSql = @"UPDATE bajaj_due_data SET 
                              status_Cd = 'LAPSED',
                              last_update_dt = TO_DATE(:currentDate, 'DD/MM/YYYY'),
                              last_update = :loginId
                              WHERE (policy_no, company_cd, due_date) IN (
                                  SELECT policy_no, company_cd, due_date 
                                  FROM TEMP_LAPSED_UPDATES
                              ) AND importdatatype = 'DUEDATA'";

    // 3. Update policy_details_master
    string updatePolicyMasterSql = @"UPDATE policy_details_master SET 
                                   last_status = 'L',
                                   UPDATE_PROG = :importType,
                                   UPDATE_USER = :loginId,
                                   UPDATE_DATE = TO_DATE(:serverDate, 'DD-MON-YYYY')
                                   WHERE (policy_no, company_cd) IN (
                                       SELECT policy_no, company_cd 
                                       FROM TEMP_LAPSED_UPDATES
                                   )";

    // Load updates to temp table
    BulkInsertLapsedUpdates(conn, transaction, lapsedUpdates);

    // Execute updates with parameters
    ExecuteUpdateWithParams(conn, transaction, updateDueDataSql, new {
        currentDate = currentDateFormatted,
        loginId
    });

    ExecuteUpdateWithParams(conn, transaction, updatePolicyMasterSql, new {
        importType,
        loginId,
        serverDate = serverDateTime.ToString("dd-MMM-yyyy")
    });
}

private void BulkInsertLapsedUpdates(OracleConnection conn, OracleTransaction transaction,
                                   List<Tuple<string, string, DateTime>> updates)
{
    // Clear temp table first
    using (var cmd = new OracleCommand("TRUNCATE TABLE TEMP_LAPSED_UPDATES", conn))
    {
        cmd.Transaction = transaction;
        cmd.ExecuteNonQuery();
    }

    // Use array binding for bulk insert
    using (var cmd = new OracleCommand(
        "INSERT INTO TEMP_LAPSED_UPDATES (policy_no, company_cd, due_date) VALUES (:1, :2, :3)", conn))
    {
        cmd.Transaction = transaction;
        cmd.ArrayBindCount = updates.Count;
        
        var policyNos = updates.Select(u => u.Item1).ToArray();
        var companyCds = updates.Select(u => u.Item2).ToArray();
        var dueDates = updates.Select(u => u.Item3).ToArray();
        
        cmd.Parameters.Add(":1", OracleDbType.Varchar2, policyNos, ParameterDirection.Input);
        cmd.Parameters.Add(":2", OracleDbType.Varchar2, companyCds, ParameterDirection.Input);
        cmd.Parameters.Add(":3", OracleDbType.Date, dueDates, ParameterDirection.Input);
        
        cmd.ExecuteNonQuery();
    }
}