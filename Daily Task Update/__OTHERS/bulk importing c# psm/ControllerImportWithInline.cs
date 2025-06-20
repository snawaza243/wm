public bool ExecuteBulkInsertInChunks(List<string> insertChunks, out string errorMessage)
{
    errorMessage = string.Empty;
    bool success = true;

    using (OracleConnection conn = new OracleConnection(
        WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
    {
        conn.Open();
        OracleTransaction transaction = conn.BeginTransaction();
        
        try
        {
            foreach (var chunk in insertChunks)
            {
                using (OracleCommand cmd = new OracleCommand(chunk, conn))
                {
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            errorMessage = $"Error executing bulk insert: {ex.Message}";
            success = false;
        }
    }
    
    return success;
}