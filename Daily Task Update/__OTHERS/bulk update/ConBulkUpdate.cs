public bool ExecuteBulkUpdate(List<Tuple<string, string>> policies, 
    string fileName, string paymentMode, string updateProg, string updateUser)
{
    using (OracleConnection conn = new OracleConnection(
        WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
    {
        conn.Open();
        OracleTransaction transaction = conn.BeginTransaction();
        
        try
        {
            // 1. Clear temp table
            using (var cmd = new OracleCommand("TRUNCATE TABLE TEMP_POLICY_UPDATES", conn))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }
            
            // 2. Bulk insert to temp table
            using (var cmd = new OracleCommand(
                "INSERT INTO TEMP_POLICY_UPDATES (policy_no, company_cd) VALUES (:1, :2)", conn))
            {
                cmd.Transaction = transaction;
                cmd.ArrayBindCount = policies.Count;
                
                var policyNos = policies.Select(p => p.Item1).ToArray();
                var companyCds = policies.Select(p => p.Item2).ToArray();
                
                cmd.Parameters.Add(":1", OracleDbType.Varchar2, policyNos, ParameterDirection.Input);
                cmd.Parameters.Add(":2", OracleDbType.Varchar2, companyCds, ParameterDirection.Input);
                
                cmd.ExecuteNonQuery();
            }
            
            // 3. Execute bulk update
            using (var cmd = new OracleCommand("BULK_UPDATE_POLICIES", conn))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.Parameters.Add("p_file_name", OracleDbType.Varchar2).Value = fileName;
                cmd.Parameters.Add("p_payment_mode", OracleDbType.Varchar2).Value = paymentMode;
                cmd.Parameters.Add("p_update_prog", OracleDbType.Varchar2).Value = updateProg;
                cmd.Parameters.Add("p_update_user", OracleDbType.Varchar2).Value = updateUser;
                cmd.Parameters.Add("p_success", OracleDbType.Int32).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("p_error_message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
                
                cmd.ExecuteNonQuery();
                
                if (Convert.ToInt32(cmd.Parameters["p_success"].Value) != 1)
                {
                    throw new Exception(cmd.Parameters["p_error_message"].Value.ToString());
                }
            }
            
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            // Log error
            return false;
        }
    }
}