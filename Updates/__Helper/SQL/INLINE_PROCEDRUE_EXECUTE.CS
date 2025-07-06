public DataTable ExecuteStoredProcedure(string procedureName, OracleParameter[] parameters, out int rowCount, out string exception)
{
    DataTable dt = new DataTable();
    rowCount = 0; // Default row count
    exception = null; // Default exception as null

    using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
    {
        using (OracleCommand cmd = new OracleCommand(procedureName, conn))
        {
            cmd.CommandType = CommandType.StoredProcedure; // Set command type to stored procedure
            
            // Add parameters if they exist
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            try
            {
                conn.Open();
                using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                {
                    adapter.Fill(dt); // Fill the DataTable with the result of the procedure
                    rowCount = dt.Rows.Count; // Get the row count
                }
            }
            catch (OracleException ex)
            {
                // Capture exception message
                exception = ex.Message;

                // Create an empty DataTable with an Exception column
                dt = new DataTable();
                dt.Columns.Add("Exception", typeof(string));
                dt.Rows.Add("Error: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }

    return dt;
}




// Create parameters for the stored procedure
OracleParameter[] parameters = new OracleParameter[]
{
    new OracleParameter("param1", OracleDbType.Varchar2) { Value = "value1" },
    new OracleParameter("param2", OracleDbType.Int32) { Value = 123 },
    new OracleParameter("outParam", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
};

// Execute the procedure
int rowCount;
string exception;
DataTable result = ExecuteStoredProcedure("YOUR_PROCEDURE_NAME", parameters, out rowCount, out exception);

// Process results
if (exception == null)
{
    // Success - use the result DataTable
}
else
{
    // Handle error
}