public void AddToMerge()
{
    DataTable dt;

    if (agentsToMergeGrid.DataSource == null || agentsToMergeGrid.Rows.Count == 0)
    {
        dt = new DataTable();
        dt.Columns.Add("AGENT_NAME");
        dt.Columns.Add("agent_code");
        dt.Columns.Add("EXIST_CODE");
        dt.Columns.Add("ADDRESS1");
        dt.Columns.Add("ADDRESS2");
        dt.Columns.Add("BranchName");
        dt.Columns.Add("BranchCode");
    }
    else
    {
        dt = ((DataView)agentsToMergeGrid.DataSource).ToTable();
    }

    // Loop through agentsGrid to find the selected agent(s)
    foreach (GridViewRow row in agentsGrid.Rows)
    {
        CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
        if (chkSelect != null && chkSelect.Checked)
        {
            string agentCode = ((Label)row.FindControl("lblAgentCodeSearched")).Text;

            // Check if the agent_CODE already agents in mainAgentsGrid
            bool agentsInMain = false;
            foreach (GridViewRow mainRow in mainAgentsGrid.Rows)
            {
                string mainAgentCode = ((Label)mainRow.FindControl("lblAgentCodeMain")).Text;
                if (mainAgentCode == agentCode)
                {
                    agentsInMain = true;
                    break;
                }
            }

            if (agentsInMain)
            {
                string msg = "Agent is already selected as main.";
                pc.ShowAlert(this, msg);
                //return;
            }
            else
            {
                DataRow dr = dt.NewRow();
                dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                dr["agent_code"] = agentCode;
                dr["exist_code"] = ((Label)row.FindControl("lblExistCodeSearched")).Text; ;
                dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1Searched")).Text;
                dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2Searched")).Text;
                dr["BranchName"] = ((Label)row.FindControl("lblBranchNameSearched")).Text;
                dr["BranchCode"] = ((Label)row.FindControl("lblBranchCodeSearched")).Text;
                dt.Rows.Add(dr);
            }
        }
    }

    // Bind the DataTable to the agentsToMergeGrid
    agentsToMergeGrid.DataSource = dt.DefaultView;
    agentsToMergeGrid.DataBind();
}
