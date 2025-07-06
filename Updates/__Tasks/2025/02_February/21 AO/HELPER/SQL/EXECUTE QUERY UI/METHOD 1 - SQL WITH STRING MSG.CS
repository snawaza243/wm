

// APPLY INLINE QUERY
string errorMsg;
DataTable SQL_RESULT = ExecuteQuery("SELECT * FROM BRANCH_MASTER", out errorMsg);

if (!string.IsNullOrEmpty(errorMsg))
{
    Console.WriteLine(errorMsg);
}


// .CS FUNCTION
public static DataTable MyQuery(string query, out string errorMessage)
{
    DataTable resultTable = new DataTable();
    errorMessage = string.Empty;

    try
    {
        resultTable = new WM.Controllers.AccountOpeningController().ExecuteQuery(query, errorMessage);
    }
    catch (Exception ex)
    {
        errorMessage = "Error executing SQL query: " + ex.Message;
    }

    return resultTable;
}
// CONTROLLER
public static DataTable ExecuteQuery(string query, out string errorMessage)
    {
        DataTable resultTable = new DataTable();
        errorMessage = string.Empty;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(resultTable);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Error executing SQL query: " + ex.Message;
            Console.WriteLine(errorMessage); // Ensure error message is captured and displayed
        }

        return resultTable;
    }