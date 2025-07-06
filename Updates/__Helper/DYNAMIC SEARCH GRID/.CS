protected void cmdSearch_Click(object sender, EventArgs e)
{
    // Define the DataTable structure
    DataTable dtGrid = new DataTable();

    // Add columns (same as your defined structure)
    string[] columns = {
        "tran_code", "tr_date", "investor_name", "issuer_name", "scheme_name",
        "branch_name", "app_no", "app_date", "folio_no", "tran_type", "amount",
        "units", "rate", "nav_date", "payment_mode", "cheque_no", "cheque_date",
        "bank_name", "broker_name", "rm_name", "manual_arno", "mut_code",
        "sch_code", "client_code", "broker_id", "rmcode", "BUSINESS_RMCODE",
        "busi_branch_code", "Remark", "CROR_PLANNO", "plan_type", "DOC_ID"
    };

    foreach (string col in columns)
    {
        dtGrid.Columns.Add(col);
    }

    // Fetch data from database
    string query = "SELECT * FROM transaction_st"; // Replace with actual query
    DataTable dt = GetData(query);

    if (dt.Rows.Count > 0)
    {
        gvDynamic.Columns.Clear(); // Clear existing columns

        // Create dynamic BoundField columns based on defined structure
        foreach (string col in columns)
        {
            BoundField bf = new BoundField();
            bf.DataField = col;
            bf.HeaderText = col.Replace("_", " "); // Format header
            gvDynamic.Columns.Add(bf);
        }

        // Bind data
        gvDynamic.DataSource = dt;
        gvDynamic.DataBind();
    }
    else
    {
        gvDynamic.DataSource = null;
        gvDynamic.DataBind();
    }
}

// Method to fetch data from the database
private DataTable GetData(string query)
{
    DataTable dt = new DataTable();
    string connectionString = "your_connection_string_here"; // Replace with actual DB connection string
    using (OracleConnection con = new OracleConnection(connectionString))
    {
        using (OracleCommand cmd = new OracleCommand(query, con))
        {
            using (OracleDataAdapter sda = new OracleDataAdapter(cmd))
            {
                sda.Fill(dt);
            }
        }
    }
    return dt;
}
