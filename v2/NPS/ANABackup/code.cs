using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace WM.Masters
{
    public partial class ANA_Merging : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoginId"] == null)
            {
                Response.Redirect("~/index.aspx");
            }
            else
            {

                if (!IsPostBack)
                {
                    string loggedInUser = Session["LoginId"]?.ToString();
                    fillBranchListByLogin(loggedInUser);
                    //fillSourceList();
                    fillRMListByBR(null, null, loggedInUser);
                }
            }

        }


        #region fillSourceList
        private void fillSourceList()
        {

            DataTable dt = new WM.Controllers.AgentController().GetBranchList();

            ddlSourceID.DataSource = dt;
            ddlSourceID.DataTextField = "BRANCH_NAME";
            ddlSourceID.DataValueField = "BRANCH_CODE";
            ddlSourceID.DataBind();
            ddlSourceID.Items.Insert(0, new ListItem("Select", "0"));

        }
        #endregion

        protected void ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSourceID = ddlSourceID.SelectedValue; // This will give Branch Code
            string loggedInUser = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(selectedSourceID)) // Check if the selected value is not null or empty
            {
                // Call the fill function with the selected branch and logged-in user
                fillRMListByBR(null, selectedSourceID, loggedInUser);
            }
            else
            {
            }

        }

        private void fillBranchListByLogin(string loginId)
        {
            // Fetch data using the controller
            DataTable dt = new WM.Controllers.AgentController().GetBranchesByLogin(loginId);

            // Bind the fetched data to the Branch dropdown
            ddlSourceID.DataSource = dt;
            ddlSourceID.DataTextField = "BRANCH_NAME";  // Update to match the column name for branch name
            ddlSourceID.DataValueField = "BRANCH_CODE"; // Update to match the column name for branch code
            ddlSourceID.DataBind();

            // Insert a default option at the top of the dropdown
            ddlSourceID.Items.Insert(0, new ListItem("Select", "0"));
        }



        private void fillRMListByBR(string srmCode, string branch, string loginId)
        {
            DataTable dt = new WM.Controllers.AgentController().GetEmployeeListByBranchOrRM( srmCode,  branch,  loginId);

            // Bind the fetched data to the RM dropdown
            ddlRM.DataSource = dt;
            ddlRM.DataTextField = "RM_NAME";  // Update to match the column name for employee name
            ddlRM.DataValueField = "RM_CODE"; // Update to match the column name for employee code
            ddlRM.DataBind();

            // Insert a default option at the top of the dropdown
            ddlRM.Items.Insert(0, new ListItem("Select", "0"));

        }
        private void fillRMList(string sourceID)
        {
            DataTable dt = new WM.Controllers.AgentController().GetRMListBySource(sourceID);
            ddlRM.DataSource = dt;
            ddlRM.DataTextField = "AGENT_NAME";  // Update to your actual column name
            ddlRM.DataValueField = "RM_CODE";    // Update to your actual column name
            ddlRM.DataBind();
            ddlRM.Items.Insert(0, new ListItem("Select", "0"));
        }

      

        #region btnSearch_Click
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Create and populate Agent object with the search filters

            int? SOURCEID = ddlSourceID.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlSourceID.SelectedValue, out int sourceId) ? (int?)sourceId : null);

            int? RM_CODE = ddlRM.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlRM.SelectedValue, out int rmCode) ? (int?)rmCode : null);


            string AGENT_NAME = string.IsNullOrEmpty(txtAgentName.Text) ? null : txtAgentName.Text.Trim();

            string EXIST_CODE = string.IsNullOrEmpty(txtExistCode.Text) ? null : txtExistCode.Text.Trim();

            // Check if at least one input field is populated
            if (SOURCEID == null && RM_CODE == null && string.IsNullOrEmpty(AGENT_NAME) && string.IsNullOrEmpty(EXIST_CODE))
            {
                ShowAlert("Kindly select a branch or enter an Agent Name or Exist Code.");
                return;
            }

            // Get the filtered agent data
            DataTable dt = new WM.Controllers.AgentController().GetFilteredAgentsData(SOURCEID, RM_CODE, AGENT_NAME, EXIST_CODE);

            if (dt != null && dt.Rows.Count > 0)
            {

                // Bind data to GridView
                
                agentsGrid.Visible = true;
                agentsGrid.DataSource = dt;
                agentsGrid.DataBind();
            }
            else
            {
                ResetAllGridMain();
                // Show alert if no records found
                ShowAlert("No records found.");
            }
        }
        #endregion

        // Function to display alert messages
        protected void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showalert", $"alert('{message}');", true);
        }

        protected void chkHeader_CheckedChanged(object sender, EventArgs e)
        {
            // Find the header checkbox
            CheckBox chkHeader = (CheckBox)sender;

            // Loop through each row in the GridView
            foreach (GridViewRow row in agentsGrid.Rows)
            {
                // Find the checkbox in the current row
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");

                if (chkSelect != null)
                {
                    // Set the row checkbox's checked status based on the header checkbox
                    chkSelect.Checked = chkHeader.Checked;
                }
            }
        }


        protected void chkHeaderRightDown_CheckedChanged(object sender, EventArgs e)
        {
            // Find the header checkbox
            CheckBox chkHeaderRightDown = (CheckBox)sender;

            // Loop through each row in the GridView
            foreach (GridViewRow row in agentsToMergeGrid.Rows)
            {
                // Find the checkbox in the current row
                CheckBox chkSelectRightDown = (CheckBox)row.FindControl("chkSelectRightDown");

                if (chkSelectRightDown != null)
                {
                    // Set the row checkbox's checked status based on the header checkbox
                    chkSelectRightDown.Checked = chkHeaderRightDown.Checked;
                }
            }
        }



        protected void btnSetAsMainAgent_Click(object sender, EventArgs e)
        {
            if (agentsGrid.Rows.Count >= 1)
            {
                // Check if mainAgentsGrid already has data
                if (mainAgentsGrid.Rows.Count > 0)
                {

                    mainAgentsGrid.DataSource = null;
                    mainAgentsGrid.DataBind();
                    AddSelectedAgentToMain(agentsGrid);

                }
                else
                {
                    // Directly set the selected agent as the main agent if none exists
                    AddSelectedAgentToMain(agentsGrid);
                }
            }
        }


        private void AddSelectedAgentToMain(GridView agentsGrid)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Agent_Name");
            dt.Columns.Add("Exist_Code");
            dt.Columns.Add("Address1");
            dt.Columns.Add("Address2");

            foreach (GridViewRow row in agentsGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    string existCode = ((Label)row.FindControl("lblExistCodeSearched")).Text;

                    // Check if the Exist_Code already exists in mainAgentsGrid
                    bool existsInToMerge = false;
                    foreach (GridViewRow mainRow in agentsToMergeGrid.Rows)
                    {
                        string mainExistCode = ((Label)mainRow.FindControl("lblExistCodeToMerge")).Text;
                        if (mainExistCode == existCode)
                        {
                            existsInToMerge = true;
                            break;
                        }
                    }

                    if (existsInToMerge)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Agent is already selected in to merge list.');", true);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Agent_Name"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                        dr["Exist_Code"] = existCode;
                        dr["Address1"] = ((Label)row.FindControl("lblAddress1Searched")).Text;
                        dr["Address2"] = ((Label)row.FindControl("lblAddress2Searched")).Text;
                        dt.Rows.Add(dr);
                    }

                    // Break after adding the first selected agent (if needed)
                    break;
                }
            }

            mainAgentsGrid.DataSource = dt.DefaultView;
            mainAgentsGrid.DataBind();
        }

        protected void btnSelectAgentToMerge_Click(object sender, EventArgs e)
        {
            DataTable dt;

            // Check if agentsToMergeGrid already has data
            if (agentsToMergeGrid.DataSource == null || agentsToMergeGrid.Rows.Count == 0)
            {
                dt = new DataTable();
                dt.Columns.Add("Agent_Name");
                dt.Columns.Add("Exist_Code");
                dt.Columns.Add("Address1");
                dt.Columns.Add("Address2");
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
                    string existCode = ((Label)row.FindControl("lblExistCodeSearched")).Text;

                    // Check if the Exist_Code already exists in mainAgentsGrid
                    bool existsInMain = false;
                    foreach (GridViewRow mainRow in mainAgentsGrid.Rows)
                    {
                        string mainExistCode = ((Label)mainRow.FindControl("lblExistCodeMain")).Text;
                        if (mainExistCode == existCode)
                        {
                            existsInMain = true;
                            break;
                        }
                    }

                    if (existsInMain)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Agent is already selected as main.');", true);
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr["Agent_Name"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                        dr["Exist_Code"] = existCode;
                        dr["Address1"] = ((Label)row.FindControl("lblAddress1Searched")).Text;
                        dr["Address2"] = ((Label)row.FindControl("lblAddress2Searched")).Text;
                        dt.Rows.Add(dr);
                    }
                }
            }

            // Bind the DataTable to the agentsToMergeGrid
            agentsToMergeGrid.DataSource = dt.DefaultView;
            agentsToMergeGrid.DataBind();
        }

        protected void MergeAgents(object sender, EventArgs e)
        {

            MergeAgentsCall();

        }

        // This method will be called when the user confirms the merge
        //protected void btnMerge_Click(object sender, EventArgs e)
        //{
        //    // Call the merge agents logic here
        //    MergeAgentsCall();

        //    // Show success message after merging
        //    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Agents Merged Successfully');", true);
        //}

        protected void MergeAgentsCall()
        {
            bool responseResult;

            string mainAgentCode = GetMainAgentExistCode();
            string[] toMergeAgentCodes = GetAllToMergeAgentExistCodes(); // Fetch as an array

            // Call the controller function to merge the agents
            responseResult = new WM.Controllers.AgentController().MergeAgents(mainAgentCode, toMergeAgentCodes);

            if (responseResult)
            {
                // If successful, show success alert
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Agents Merged Successfully');", true);

            }
            else
            {
                // If the merging failed, show failure alert
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Error Merging Agents');", true);
            }
        }





        protected void ShowAgentExist_Click(object sender, EventArgs e)
        {
            // Get the main agent exist code and merge agent exist codes
            string mainAgentCode = GetMainAgentExistCode();
            string toMergeAgentCodes = string.Join(", ", GetAllToMergeAgentExistCodes());

            // Set the text for the label
            lblAgentCodes.Text = $"Main agent exist code: {mainAgentCode}<br/>All To Merge agent exist codes: {toMergeAgentCodes}";
        }

        public string GetMainAgentExistCode()
        {
            // Assume only one main agent exists; initialize a variable to hold the Exist_Code value
            string existCodeMain = string.Empty;

            // Check the first row in the main agents grid (assuming it has at least one row)
            if (mainAgentsGrid.Rows.Count > 0)
            {
                // Find the Label control that holds the Exist_Code in the first row
                Label lblExistCode = (Label)mainAgentsGrid.Rows[0].FindControl("lblExistCodeMain");

                if (lblExistCode != null)
                {
                    // Set the Exist_Code value
                    existCodeMain = lblExistCode.Text;
                }
            }

            // Return the main agent code (can be empty if no code found)
            return existCodeMain;
        }

        public string[] GetAllToMergeAgentExistCodes()
        {
            // Initialize a list to hold the Exist_Code values for the agents to merge
            List<string> existCodesToMerge = new List<string>();

            // Iterate through each row in the agentsToMergeGrid (correct GridView for merging agents)
            foreach (GridViewRow row in agentsToMergeGrid.Rows)
            {
                // Find the Label control that holds the Exist_Code in the current row
                Label lblExistCode = (Label)row.FindControl("lblExistCodeToMerge");

                if (lblExistCode != null)
                {
                    // Add the Exist_Code value to the list
                    existCodesToMerge.Add(lblExistCode.Text);
                }
            }

            // Convert the list to an array and return it
            return existCodesToMerge.ToArray();
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {

            ResetMain();

        }

        protected void ResetMain()
        {
            // Reset dropdowns to their default values
            ddlSourceID.SelectedIndex = 0;
            ddlRM.SelectedIndex = 0;

            txtAgentName.Text = string.Empty;
            txtExistCode.Text = string.Empty;

            lblAgentCodes.Text = string.Empty;

            // Clear the agentsGrid (searched section)
            agentsGrid.DataSource = null;
            agentsGrid.DataBind();

            // Clear the mainAgentsGrid (sent to agent section)
            mainAgentsGrid.DataSource = null;
            mainAgentsGrid.DataBind();

            // Clear the agentsToMergeGrid (agents to merge section)
            agentsToMergeGrid.DataSource = null;
            agentsToMergeGrid.DataBind();

        }

        protected void ResetAllGridMain()
        {
  
            // Clear the agentsGrid (searched section)
            agentsGrid.DataSource = null;
            agentsGrid.DataBind();

            // Clear the mainAgentsGrid (sent to agent section)
            mainAgentsGrid.DataSource = null;
            mainAgentsGrid.DataBind();

            // Clear the agentsToMergeGrid (agents to merge section)
            agentsToMergeGrid.DataSource = null;
            agentsToMergeGrid.DataBind();

        }

        protected void ExitButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/welcome.aspx");
        }

        protected void btnRemoveSelected_Click(object sender, EventArgs e)
        {

            DataTable dt;


            dt = new DataTable();
            dt.Columns.Add("Agent_Name");
            dt.Columns.Add("Exist_Code");

            dt.Columns.Add("Address1");
            dt.Columns.Add("Address2");


            // Loop through agentsGrid to find the selected agent(s)
            foreach (GridViewRow row in agentsToMergeGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelectRightDown");
                if (!chkSelect.Checked)
                {
                    // Add the selected agent's information to the DataTable
                    DataRow dr = dt.NewRow();
                    dr["Agent_Name"] = ((Label)row.FindControl("lblAgentNameToMerge")).Text;
                    dr["Exist_Code"] = ((Label)row.FindControl("lblExistCodeToMerge")).Text;
                    dr["Address1"] = ((Label)row.FindControl("lblAddress1ToMerge")).Text;
                    dr["Address2"] = ((Label)row.FindControl("lblAddress2ToMerge")).Text;
                    dt.Rows.Add(dr);
                }
            }

            // Bind the DataTable to the agentsToMergeGrid
            agentsToMergeGrid.DataSource = dt.DefaultView;
            agentsToMergeGrid.DataBind();


        }
    }
}

