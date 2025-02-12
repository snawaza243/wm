protected void chkHeaderSearchedMaster_CheckedChanged(object sender, EventArgs e)
{
    // Retrieve the source data for the main grid
    DataTable mainTable = GetDataFromMainGrid();

    foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
    {
        // Find the row checkbox and check if it is selected
        CheckBox rowCheckBox = (CheckBox)row.FindControl("chkSelectSearchedMaster");
        if (rowCheckBox != null && rowCheckBox.Checked)
        {
            // Get values from the master grid row
            string agentCode = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;
            string sourceID = ((Label)row.FindControl("lblSourceCodeSearchedMaster")).Text;
            string agentName = ((Label)row.FindControl("lblAgentNameSearchedMaster")).Text;
            string address1 = ((Label)row.FindControl("lblAddress1SearchedMaster")).Text;
            string address2 = ((Label)row.FindControl("lblAddress2SearchedMaster")).Text;

            // Check if a row with the same Agent_Code exists in the main grid with the same SourceID
            bool exists = mainTable.AsEnumerable().Any(row => 
                row.Field<string>("Agent_Code") == agentCode && 
                row.Field<string>("SourceID") == sourceID);

            if (!exists)
            {
                // Add the row to the main table
                DataRow newRow = mainTable.NewRow();
                newRow["Agent_Code"] = agentCode;
                newRow["Agent_Name"] = agentName;
                newRow["Address1"] = address1;
                newRow["Address2"] = address2;
                newRow["SourceID"] = sourceID;

                mainTable.Rows.Add(newRow);
            }
        }
    }

    // Rebind the main grid with the updated data
    mainAgentsGrid.DataSource = mainTable;
    mainAgentsGrid.DataBind();
}



private DataTable GetDataFromMainGrid()
{
    // Define the structure of the main grid's DataTable
    DataTable mainTable = new DataTable();
    mainTable.Columns.Add("Agent_Code", typeof(string));
    mainTable.Columns.Add("Agent_Name", typeof(string));
    mainTable.Columns.Add("Address1", typeof(string));
    mainTable.Columns.Add("Address2", typeof(string));
    mainTable.Columns.Add("SourceID", typeof(string));

    // Populate data from the main grid rows
    foreach (GridViewRow row in mainAgentsGrid.Rows)
    {
        DataRow dataRow = mainTable.NewRow();
        dataRow["Agent_Code"] = ((Label)row.FindControl("lblAgentNameMain")).Text;
        dataRow["Agent_Name"] = ((Label)row.FindControl("lblAgentNameMain")).Text;
        dataRow["Address1"] = ((Label)row.FindControl("lblAddress1Main")).Text;
        dataRow["Address2"] = ((Label)row.FindControl("lblAddress2Main")).Text;
        dataRow["SourceID"] = ((Label)row.FindControl("lblSourceCodeMain")).Text;

        mainTable.Rows.Add(dataRow);
    }

    return mainTable;
}
