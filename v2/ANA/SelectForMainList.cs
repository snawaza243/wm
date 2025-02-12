    }
    protected void btnAddSelectedRows_Click(object sender, EventArgs e)
    {
        // Create a DataTable to hold the selected rows data
        DataTable selectedRows = new DataTable();
        selectedRows.Columns.Add("Agent_Code");
        selectedRows.Columns.Add("Exist_Code");
        selectedRows.Columns.Add("Agent_Name");
        selectedRows.Columns.Add("Address1");
        selectedRows.Columns.Add("Address2");
        selectedRows.Columns.Add("SourceID");

        // Loop through each row in the source GridView (agentsGridSearchedMaster)
        foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
        {
            // Find the checkbox in the current row
            CheckBox chkSelectSearchedMaster = (CheckBox)row.FindControl("chkSelectSearchedMaster");

            // If the checkbox is checked, add the row's data to the DataTable
            if (chkSelectSearchedMaster != null && chkSelectSearchedMaster.Checked)
            {
                string currentMasterSrc = ((Label)row.FindControl("lblSourceCodeSearchedMaster")).Text;
                
                DataRow newRow = selectedRows.NewRow();
                newRow["Agent_Code"] = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;
                newRow["Exist_Code"] = ((Label)row.FindControl("lblExistCodeSearchedMaster")).Text;
                newRow["Agent_Name"] = ((Label)row.FindControl("lblAgentNameSearchedMaster")).Text;
                newRow["Address1"] = ((Label)row.FindControl("lblAddress1SearchedMaster")).Text;
                newRow["Address2"] = ((Label)row.FindControl("lblAddress2SearchedMaster")).Text;
                newRow["SourceID"] = ((Label)row.FindControl("lblSourceCodeSearchedMaster")).Text;

                selectedRows.Rows.Add(newRow);
            }
        }

        // Now bind the selected rows to the target GridView (agentsGrid)
        agentsGrid.DataSource = selectedRows;
        agentsGrid.DataBind();
    }
