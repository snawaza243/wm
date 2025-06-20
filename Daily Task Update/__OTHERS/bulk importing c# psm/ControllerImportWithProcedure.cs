using System;
using System.Data;
using Oracle.DataAccess.Client; // Oracle Data Provider for .NET
using System.Web.Configuration;

public class BulkInsertExecutor
{
    public bool ExecuteBulkInsert(string insertStatement, out string errorMessage)
    {
        errorMessage = string.Empty;
        bool success = false;

        using (OracleConnection conn = new OracleConnection(
            WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
        {
            try
            {
                conn.Open();
                
                using (OracleCommand cmd = new OracleCommand("EXECUTE_BULK_INSERT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    // Add parameters
                    cmd.Parameters.Add("p_insert_statement", OracleDbType.Clob).Value = insertStatement;
                    cmd.Parameters.Add("p_success", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;
                    
                    cmd.ExecuteNonQuery();
                    
                    // Get output parameters
                    success = Convert.ToInt32(cmd.Parameters["p_success"].Value) == 1;
                    errorMessage = cmd.Parameters["p_error_message"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error executing bulk insert: {ex.Message}";
                success = false;
            }
        }
        
        return success;
    }
}