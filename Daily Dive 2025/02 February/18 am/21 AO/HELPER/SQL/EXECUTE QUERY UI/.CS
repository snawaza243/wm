
// APPLY INLINE QUERY

DataTable SQL_RESULT = MyQuery("SELECT * FROM BRANCH_MASTER");

if (SQL_RESULT.Rows.Count > 0)
{
    string sql = SQL_RESULT.Rows[0];
    atad
    Console.WriteLine(errorMsg);
}





public static DataTable MyQuery(string query)
{
    DataTable resultTable = new DataTable();

    try
    {
        resultTable = new WM.Controllers.AccountOpeningController().ExecuteQuery(query);

    }
    catch (Exception ex)
    {
        
    }

    return resultTable;
}

// CONTROLLER
   public DataTable ExecuteQuery(string query)
   {
       DataTable dt = new DataTable();

       using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
       {
           using (OracleCommand cmd = new OracleCommand(query, conn))
           {
               try
               {
                   conn.Open();
                   using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                   {
                       adapter.Fill(dt); // Fill the DataTable with the result of the query
                   }
               }
               catch (OracleException ex)
               {
                   // Handle exceptions and log them
                   Console.WriteLine("Error: " + ex.Message);
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

