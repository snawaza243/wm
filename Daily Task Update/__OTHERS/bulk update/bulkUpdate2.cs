public bool ExecuteBulkUpdateInChunks(List<Tuple<string, string>> policies, 
    string fileName, string paymentMode, string updateProg, string updateUser)
{
    const int chunkSize = 100; // Number of policies per UPDATE statement
    var chunks = policies.Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index / chunkSize)
                        .Select(x => x.Select(v => v.Value).ToList())
                        .ToList();

    using (OracleConnection conn = new OracleConnection(
        WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
    {
        conn.Open();
        OracleTransaction transaction = conn.BeginTransaction();
        
        try
        {
            foreach (var chunk in chunks)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("UPDATE POLICY_DETAILS_MASTER SET");
                sb.AppendLine("    FILE_NAME = :fileName,");
                sb.AppendLine("    PAYMENT_MODE = :paymentMode,");
                sb.AppendLine("    UPDATE_PROG = :updateProg,");
                sb.AppendLine("    UPDATE_USER = :updateUser,");
                sb.AppendLine("    UPDATE_DATE = SYSDATE");
                sb.AppendLine("WHERE ");
                
                for (int i = 0; i < chunk.Count; i++)
                {
                    if (i > 0) sb.Append(" OR ");
                    sb.Append($"(UPPER(TRIM(POLICY_no)) = :pno{i} AND UPPER(TRIM(COMPANY_CD)) = :ccd{i})");
                }
                
                using (var cmd = new OracleCommand(sb.ToString(), conn))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.Add("fileName", OracleDbType.Varchar2).Value = fileName;
                    cmd.Parameters.Add("paymentMode", OracleDbType.Varchar2).Value = paymentMode;
                    cmd.Parameters.Add("updateProg", OracleDbType.Varchar2).Value = updateProg;
                    cmd.Parameters.Add("updateUser", OracleDbType.Varchar2).Value = updateUser;
                    
                    for (int i = 0; i < chunk.Count; i++)
                    {
                        cmd.Parameters.Add($"pno{i}", OracleDbType.Varchar2).Value = chunk[i].Item1;
                        cmd.Parameters.Add($"ccd{i}", OracleDbType.Varchar2).Value = chunk[i].Item2;
                    }
                    
                    cmd.ExecuteNonQuery();
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