using DocumentFormat.OpenXml.Office.Word;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Input;
using System.Windows.Interop;
using WM.Controllers;
using WM.Models;

using Oracle.ManagedDataAccess.Client;
using System.Configuration;


namespace WM.Masters
{
    public partial class ANA_Merging : System.Web.UI.Page
    {
        PsmController pc = new PsmController();

        protected void Page_Load(object sender, EventArgs e)
        {
            string loggedInUser = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(loggedInUser))
            {
                if (!IsPostBack)
                {
                    fillBranchListByLogin(loggedInUser);
                    //fillSourceList();
                    //fillRMListByBR1(null, null, loggedInUser);
                    fillCityList();
                }
            }
            else
            {
                pc.RedirectToWelcomePage();
            }

        }

        #region Old merge

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

        #region ddlSourceID_SelectedIndexChanged
        protected void ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSourceID = ddlSourceID.SelectedValue; // This will give Branch Code
            string loggedInUser = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(selectedSourceID)) // Check if the selected value is not null or empty
            {
                // Call the fill function with the selected branch and logged-in user
                fillRMListByBR1(null, selectedSourceID, loggedInUser);

                anameringBranch.SelectedValue = selectedSourceID;
                fillRMListByBR2(null, selectedSourceID, loggedInUser);

            }
            else
            {
            }


        }

        #endregion


        #region ddlSourceIDAnameringBranch_SelectedIndexChanged
        protected void ddlSourceIDAnameringBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSourceID2 = anameringBranch.SelectedValue.ToString(); // This will give Branch Code
            string loggedInUser = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(selectedSourceID2)) // Check if the selected value is not null or empty
            {
                // Call the fill function with the selected branch and logged-in user
                fillRMListByBR2(null, selectedSourceID2, loggedInUser);
            }
            else
            {
            }


        }

        #endregion
        private void fillBranchListByLogin(string loginId)
        {
            // Fetch data using the controller
            DataTable dt = new WM.Controllers.AgentController().GetBranchesByLogin(loginId);

            if (dt.Rows.Count > 0)
            {
                ddlSourceID.DataSource = dt;
                ddlSourceID.DataTextField = "BRANCH_NAME";
                ddlSourceID.DataValueField = "BRANCH_CODE";
                ddlSourceID.DataBind();
                ddlSourceID.Items.Insert(0, new ListItem("Select", "0"));

                anameringBranch.DataSource = dt;
                anameringBranch.DataTextField = "BRANCH_NAME";
                anameringBranch.DataValueField = "BRANCH_CODE";
                anameringBranch.DataBind();
                anameringBranch.Items.Insert(0, new ListItem("Select", "0"));
            }
            else
            {
                anameringBranch.Items.Insert(0, new ListItem("Not any branch exist", ""));

            }



        }

        private void fillCityList()
        {
            DataTable dt = new WM.Controllers.AgentController().GetCityList();

            anameringCity.DataSource = dt;
            anameringCity.DataTextField = "CITY_NAME";
            anameringCity.DataValueField = "CITY_ID";
            anameringCity.DataBind();
            anameringCity.Items.Insert(0, new ListItem("Select City", "0"));

        }

        private void fillRMListByBR1(string srmCode, string branch, string loginId)
        {
            DataTable dt = new WM.Controllers.AgentController().GetEmployeeListByBranchOrRM(srmCode, branch, loginId);

            if (dt.Rows.Count > 0)
            {

                ddlRM.DataSource = dt;
                ddlRM.DataTextField = "RM_NAME";
                ddlRM.DataValueField = "RM_CODE";
                ddlRM.DataBind();
                ddlRM.Items.Insert(0, new ListItem("Select", ""));
            }
            else
            {
                ddlRM.Items.Insert(0, new ListItem("Not RM exist", ""));
            }

        }

        private void fillRMListByBR2(string srmCode, string branch, string loginId)
        {
            DataTable dt = new WM.Controllers.AgentController().GetEmployeeListByBranchOrRM(srmCode, branch, loginId);

            // Bind the fetched data to the RM dropdown
            anameringRm.DataSource = dt;
            anameringRm.DataTextField = "RM_NAME";
            anameringRm.DataValueField = "RM_CODE";
            anameringRm.DataBind();
            anameringRm.Items.Insert(0, new ListItem("Select", "0"));

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


        protected void btnModelReset_Click(object sender, EventArgs e)
        {
            ResetFormFields();
        }

        #region btnSearch_Click
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Existing parameters
            int? SOURCEID = GetNullableInt(anameringBranch.SelectedValue.ToString());
            int? RM_CODE = GetNullableInt(anameringRm.SelectedValue.ToString());
            string AGENT_NAME = GetNullableString(anameringName.Text);
            string EXIST_CODE = GetNullableString(anameringClientCode.Text);

            // New parameters 
            int? OLD_RM_CODE = GetNullableInt(anameringOldRm.SelectedValue.ToString());
            int? OldExistCode = GetNullableInt(anameringOldCode.Text.ToString());
            long? mobile = GetNullableLong(anameringMobile.Text);
            string phone = GetNullableString(anameringPhone.Text.ToString());
            string category = GetNullableString(anameringCategory.SelectedValue.ToString());
            string city = GetNullableString(anameringCity.SelectedValue.ToString());
            string sortBy = GetNullableString(anameringSortBy.SelectedValue.ToString());
            string ADDRESS1 = GetNullableString(anameringAddress1.Text.ToString());
            string ADDRESS2 = GetNullableString(anameringAddress2.Text.ToString());
            string pan = GetNullableString(anameringPan.Text.ToString());

            // Check for at least one field to be filled
            if ((string.IsNullOrEmpty(anameringBranch.SelectedValue) || anameringBranch.SelectedValue.ToString() == "0") &&
                (string.IsNullOrEmpty(anameringCity.SelectedValue) || anameringCity.SelectedValue.ToString() == "0") &&
                (string.IsNullOrEmpty(anameringRm.SelectedValue) || anameringRm.SelectedValue.ToString() == "0") &&
                string.IsNullOrEmpty(AGENT_NAME) &&
                string.IsNullOrEmpty(EXIST_CODE) &&
                string.IsNullOrEmpty(anameringOldCode.Text) &&

                string.IsNullOrEmpty(pan) &&
                string.IsNullOrEmpty(phone) &&
                string.IsNullOrEmpty(anameringMobile.Text) &&

                string.IsNullOrEmpty(pan) &&


                string.IsNullOrEmpty(ADDRESS1) &&
                string.IsNullOrEmpty(ADDRESS2)

                )

            {
                // Alert user to fill at least one field
                string msg = "Please fill at least one field to search.";
                ShowAlert(msg);
                return;
            }
            else
            {
                // Pass all parameters to the controller method
                DataTable dt = new WM.Controllers.AgentController().GetFilteredAgentsDataANA(SOURCEID, RM_CODE, AGENT_NAME, EXIST_CODE,
                    OLD_RM_CODE, OldExistCode, mobile, phone, category, city, sortBy, ADDRESS1, ADDRESS2, pan);

                if (dt != null && dt.Rows.Count > 0)
                {

                    string agentSearchedMasterInfo = $"Total record(s) found: {dt.Rows.Count}";
                    lblAgentCodeSearchedMasterInfo.Text = agentSearchedMasterInfo;

                    agentsGridSearchedMaster.Visible = true;
                    agentsGridSearchedMaster.DataSource = dt;
                    agentsGridSearchedMaster.DataBind();
                }
                else
                {

                    // Show alert if no records found

                    string msg = $"No records found!";
                    ShowAlert(msg);
                    lblAgentCodeSearchedMasterInfo.Text = msg;

                    agentsGridSearchedMaster.Visible = false;
                    agentsGridSearchedMaster.DataSource = null;
                    agentsGridSearchedMaster.DataBind();
                }
            }
        }
        #endregion

        #endregion


        public int? GetNullableInt(string selectedValue)
        {
            // Return null if the selected value is "0"
            if (selectedValue == "0")
            {
                return null;
            }

            // Try parsing the selected value as an integer
            if (int.TryParse(selectedValue, out int parsedValue))
            {
                return parsedValue; // Return the parsed integer as nullable
            }

            // If parsing fails, return null
            return null;
        }

        public string GetNullableString(string input)
        {
            // If the input is null or empty, return null
            return string.IsNullOrEmpty(input) ? null : input;
        }


        public long? GetNullableLong(string inputText)
        {
            // Try to parse the input text into a long value
            if (long.TryParse(inputText, out long parsedValue))
            {
                // Return the parsed long value if successful
                return parsedValue;
            }
            else
            {
                // Return null if parsing fails (invalid input)
                return null;
            }
        }
        protected void btnAddSelectedRows_Click(object sender, EventArgs e)
        {
            string notSelectedMsg = "Agent not selected to add!";
            bool anyRowSelected = false;

            if (agentsGridSearchedMaster.Rows.Count <= 0)
            {
                ShowAlert(notSelectedMsg);
                return;
            }

            else
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
                        anyRowSelected = true; // Mark that at least one row is selected

                        // Get the SourceID from the selected row
                        string newSourceID = ((Label)row.FindControl("lblSourceCodeSearchedMaster")).Text;

                        // Check if the SourceID already exists in the target grid (agentsGrid)
                        bool sourceIDExists = false;
                        foreach (GridViewRow targetRow in agentsGrid.Rows)
                        {
                            // Get the SourceID from the target grid row
                            string existingSourceID = ((Label)targetRow.FindControl("lblSourceCodeSearched")).Text;

                            // Compare the SourceID values
                            if (newSourceID != existingSourceID)
                            {
                                sourceIDExists = true;
                                break;
                            }
                        }

                        // If SourceID does not match, show a message and skip adding the row
                        if (sourceIDExists)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('The branch should be the same for all selected rows.');", true);
                            return; // Exit the method early to prevent adding the row
                        }

                        // Create a new row for the selected data
                        DataRow newRow = selectedRows.NewRow();
                        newRow["Agent_Code"] = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;
                        newRow["Exist_Code"] = ((Label)row.FindControl("lblExistCodeSearchedMaster")).Text;
                        newRow["Agent_Name"] = ((Label)row.FindControl("lblAgentNameSearchedMaster")).Text;
                        newRow["Address1"] = ((Label)row.FindControl("lblAddress1SearchedMaster")).Text;
                        newRow["Address2"] = ((Label)row.FindControl("lblAddress2SearchedMaster")).Text;
                        newRow["SourceID"] = newSourceID;

                        // Add the row to the DataTable
                        selectedRows.Rows.Add(newRow);
                    }
                }

                if (!anyRowSelected)
                {
                    ShowAlert(notSelectedMsg);
                    return;
                }
                else if (anyRowSelected)
                {
                    ShowAlert("Selected agent added!");


                }

                // Now bind the selected rows to the target GridView (agentsGrid)
                agentsGrid.DataSource = selectedRows;
                agentsGrid.DataBind();
            }

        }

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
            dt.Columns.Add("AGENT_NAME");
            dt.Columns.Add("EXIST_CODE");
            dt.Columns.Add("ADDRESS1");
            dt.Columns.Add("ADDRESS2");
            dt.Columns.Add("sourceid");


            foreach (GridViewRow row in agentsGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    string existCode = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;

                    // Check if the EXIST_CODE already exists in mainAgentsGrid
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
                        dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                        dr["EXIST_CODE"] = existCode;
                        dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1Searched")).Text;
                        dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2Searched")).Text;
                        dr["sourceid"] = ((Label)row.FindControl("lblSourceCodeSearched")).Text;

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
                dt.Columns.Add("AGENT_NAME");
                dt.Columns.Add("EXIST_CODE");
                dt.Columns.Add("ADDRESS1");
                dt.Columns.Add("ADDRESS2");
                dt.Columns.Add("sourceid");

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

                    // Check if the EXIST_CODE already exists in mainAgentsGrid
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
                        dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                        dr["EXIST_CODE"] = existCode;
                        dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1Searched")).Text;
                        dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2Searched")).Text;
                        dr["sourceid"] = ((Label)row.FindControl("lblSourceCodeSearched")).Text;

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
            string mainAgentCode = GetMainAgentExistCode();
            string[] toMergeAgentCodes = GetAllToMergeAgentExistCodes(); // Fetch as an array
            string loginId  = Session["LoginId"]?.ToString();


            MergeAgentsCall(mainAgentCode, toMergeAgentCodes, loginId);
            MergeClients(mainAgentCode, toMergeAgentCodes, loginId);

        }

        // This method will be called when the user confirms the merge
        //protected void btnMerge_Click(object sender, EventArgs e)
        //{
        //    // Call the merge agents logic here
        //    MergeAgentsCall();

        //    // Show success message after merging
        //    ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('Agents Merged Successfully');", true);
        //}

        protected void MergeAgentsCall(string mainAgentCode, string[] toMergeAgentCodes, string login)
        {
            bool responseResult;

            //string mainAgentCode = GetMainAgentExistCode();
            //string[] toMergeAgentCodes = GetAllToMergeAgentExistCodes(); // Fetch as an array

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
            // Assume only one main agent exists; initialize a variable to hold the EXIST_CODE value
            string existCodeMain = string.Empty;

            // Check the first row in the main agents grid (assuming it has at least one row)
            if (mainAgentsGrid.Rows.Count > 0)
            {
                // Find the Label control that holds the EXIST_CODE in the first row
                Label lblExistCode = (Label)mainAgentsGrid.Rows[0].FindControl("lblExistCodeMain");

                if (lblExistCode != null)
                {
                    // Set the EXIST_CODE value
                    existCodeMain = lblExistCode.Text;
                }
            }

            // Return the main agent code (can be empty if no code found)
            return existCodeMain;
        }

        public string[] GetAllToMergeAgentExistCodes()
        {
            // Initialize a list to hold the EXIST_CODE values for the agents to merge
            List<string> existCodesToMerge = new List<string>();

            // Iterate through each row in the agentsToMergeGrid (correct GridView for merging agents)
            foreach (GridViewRow row in agentsToMergeGrid.Rows)
            {
                // Find the Label control that holds the EXIST_CODE in the current row
                Label lblExistCode = (Label)row.FindControl("lblExistCodeToMerge");

                if (lblExistCode != null)
                {
                    // Add the EXIST_CODE value to the list
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



        protected void ResetFormFields()
        {
            // Reset the dropdowns
            //anameringCategory.SelectedIndex = 0; // Set to "Select"
            anameringBranch.SelectedIndex = 0; // Set to "Select"
            anameringRm.SelectedIndex = 0;

            anameringCity.SelectedIndex = 0; // Set to "Select"
            //anameringRm.SelectedIndex = 0; // Set to "Select branch first"
            //anameringOldRm.SelectedIndex = 0; // Set to "Select"
            anameringSortBy.SelectedIndex = 0; // Set to "Select"

            // Reset the textboxes
            anameringName.Text = string.Empty;
            anameringClientCode.Text = string.Empty; // exist code
            anameringOldCode.Text = string.Empty; // agent code
            anameringAddress1.Text = string.Empty;
            anameringAddress2.Text = string.Empty;
            anameringPan.Text = string.Empty;
            anameringPhone.Text = string.Empty;
            anameringMobile.Text = string.Empty;

            // Optionally, reset the checkboxes if they are part of the form
            foreach (GridViewRow row in agentsGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null)
                {
                    chkSelect.Checked = false;
                }
            }

            // If you want to disable/enable any fields (for example, the disabled dropdowns)
            anameringCategory.Enabled = true;  // Enable category dropdown
            anameringOldRm.Enabled = false;  // Disable Old RM dropdown

            agentsGridSearchedMaster.DataSource = null;
            agentsGridSearchedMaster.DataBind();
        }


        protected void ExitButton_Click(object sender, EventArgs e)
        {
            string loginId = Session["LoginId"]?.ToString();
            string roleId = Session["roleId"]?.ToString();
            Response.Redirect($"~/welcome?loginid={loginId}&roleid={roleId}");
        }

        protected void btnRemoveSelected_Click(object sender, EventArgs e)
        {

            DataTable dt;


            dt = new DataTable();
            dt.Columns.Add("AGENT_NAME");
            dt.Columns.Add("EXIST_CODE");

            dt.Columns.Add("ADDRESS1");
            dt.Columns.Add("ADDRESS2");
            dt.Columns.Add("sourceid");



            // Loop through agentsGrid to find the selected agent(s)
            foreach (GridViewRow row in agentsToMergeGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelectRightDown");
                if (!chkSelect.Checked)
                {
                    // Add the selected agent's information to the DataTable
                    DataRow dr = dt.NewRow();
                    dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameToMerge")).Text;
                    dr["EXIST_CODE"] = ((Label)row.FindControl("lblExistCodeToMerge")).Text;
                    dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1ToMerge")).Text;
                    dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2ToMerge")).Text;
                    dr["sourceid"] = ((Label)row.FindControl("lblSourceCodeToMerge")).Text;


                    dt.Rows.Add(dr);
                }
            }

            // Bind the DataTable to the agentsToMergeGrid
            agentsToMergeGrid.DataSource = dt.DefaultView;
            agentsToMergeGrid.DataBind();


        }



        protected void chkHeaderSearchedMaster_CheckedChanged(object sender, EventArgs e)

        {
            // Cast the sender to a CheckBox
            CheckBox headerCheckBox = (CheckBox)sender;

            // Iterate through each row in the GridView
            foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
            {
                // Find the checkbox in the row
                CheckBox rowCheckBox = (CheckBox)row.FindControl("chkSelectSearchedMaster");

                if (rowCheckBox != null)
                {
                    // Set the row checkbox's checked state to match the header checkbox
                    rowCheckBox.Checked = headerCheckBox.Checked;
                    agentsGridSearchedMaster.Focus();
                }
            }
        }

        #endregion

        #region NEW MERGIN


        #region helping func for new merge
        private OracleConnection MyConn;
        private string branch_cd;
        private string Rm_cd;
        private int mCount = 0;
        private DateTime ServerDateTime = DateTime.Now;

 
        private DataTable ExecuteQuery(string query, OracleTransaction trans = null)
        {
            DataTable dt = new DataTable();
            OracleCommand cmd = new OracleCommand(query, MyConn);
            if (trans != null)
                cmd.Transaction = trans;
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        // Executes a non-query SQL command. Returns the number of rows affected.
        private int ExecuteNonQuery(string query, OracleTransaction trans = null)
        {
            OracleCommand cmd = new OracleCommand(query, MyConn);
            if (trans != null)
                cmd.Transaction = trans;
            return cmd.ExecuteNonQuery();
        }

        // A dummy validation function. Returns true if validation passes.
        private bool ValidateInput()
        {
            // Dummy implementation. Replace with actual validation logic if needed.
            return true;
        }

        // Dummy implementation to set branch_cd and Rm_cd.
        private void Find_RM()
        {
            // In the original VB code, this would set the RM details.
            branch_cd = "1";
            Rm_cd = "1";
        }

        // Checks if two date strings (in dd/MM/yyyy format) are the same.
        private bool CheckDate(string date1, string date2)
        {
            return string.Equals(date1, date2, StringComparison.OrdinalIgnoreCase);
        }

        // Returns the leftmost substring of the specified length.
        private string Left(string s, int len)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Length >= len ? s.Substring(0, len) : s;
        }

        // Dummy implementation to (re)configure grid controls.
        private void SetGrid()
        {
            // Implementation for grid configuration goes here.
        }

        #endregion

        public void MergeClients(string mainAgent, string[] agentsArray, string loginId)
        {
            try
            {
                
                branch_cd = "";
                Rm_cd = "";
                //Find_RM();

                string Fam_Head = "";
                string Members1 = "";
                string Members2 = "";
                string Members3 = "";

                for (int i = 1; i < agentsArray.Length; i++)
                {
                    string queryRsClient = "Select * from agent_master where agent_code='" + mainAgent + "'";
                    DataTable rsClient = pc.ExecuteCurrentQuery(queryRsClient);


                    return;

                    string queryRsClient1 = "Select * from agent_master where agent_code=" + agentsArray[i];
                    DataTable rsclient1 = ExecuteQuery(queryRsClient1);

                    string queryRsData = "select inv_code,investor_name from investor_master where source_id=" + agentsArray[i];
                    DataTable RsData = ExecuteQuery(queryRsData);

                    OracleTransaction trans = MyConn.BeginTransaction();
                    bool flag = true;

                    foreach (DataRow rsDataRow in RsData.Rows)
                    {
                        string investorName = rsDataRow["investor_name"].ToString();
                        string cleanedInvestorName = investorName.Trim().ToUpper().Replace(".", "").Replace(" ", "");
                        string searchPattern = "%" + Left(cleanedInvestorName, 8) + "%";
                        string queryRsInvCheck = "Select inv_code from investor_master where source_id=" + mainAgent +
                            " and substr(replace(replace(trim(upper(investor_name)),'.',''),' ',''),1,8) like '" + searchPattern +
                            "' and instr(trim(upper(investor_name)),'HUF')=0";
                        DataTable rsInv_check = ExecuteQuery(queryRsInvCheck, trans);

                        string New_Inv_Code = "";
                        if (rsInv_check.Rows.Count > 0)
                        {
                            New_Inv_Code = rsInv_check.Rows[0]["inv_code"].ToString();
                        }
                        else
                        {
                            mCount = mCount + 1;
                            if (mCount >= 999)
                                New_Inv_Code = mainAgent + mCount.ToString("00000");
                            else
                                New_Inv_Code = mainAgent + mCount.ToString("000");
                            string updateInvMaster = "update INVESTOR_MASTER       set   SOURCE_ID=" + mainAgent +
                                ",BRANCH_CODE=" + branch_cd + ",RM_CODE=" + Rm_cd + ",INV_CODE='" + New_Inv_Code +
                                "' where INV_CODE=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(updateInvMaster, trans);
                        }

                        string updateFpInvestor1 = "update fp_investor set familyhead_code='" + New_Inv_Code +
                            "' where familyhead_code='" + rsDataRow["inv_code"] + "'";
                        ExecuteNonQuery(updateFpInvestor1, trans);

                        string updateFpInvestor2 = "update fp_investor set fam_mem1=replace(fam_mem1," + rsDataRow["inv_code"] +
                            "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) +
                            "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                        ExecuteNonQuery(updateFpInvestor2, trans);

                        string updateFpInvestor3 = "update fp_investor set fam_mem2=replace(fam_mem2," + rsDataRow["inv_code"] +
                            "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) +
                            "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                        ExecuteNonQuery(updateFpInvestor3, trans);

                        string updateFpInvestor4 = "update fp_investor set fam_mem3=replace(fam_mem3," + rsDataRow["inv_code"] +
                            "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) +
                            "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                        ExecuteNonQuery(updateFpInvestor4, trans);

                        string updateTransSt = "update TRANSACTION_ST        set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransSt, trans);

                        string updateTransMFTemp1 = "update TRANSACTION_MF_TEMP1  set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransMFTemp1, trans);

                        string updateTransSt_Bajaj = "update TRANSACTION_ST@mf.bajajcapital        set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransSt_Bajaj, trans);

                        string updateTransStTemp = "update TRANSACTION_STTEMP    set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransStTemp, trans);

                        string updateRedem = "update REDEM@mf.bajajcapital                 set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateRedem, trans);

                        string updateInvFolio = "update INVESTOR_FOLIO@mf.bajajcapital        set INVESTOR_CODE=" + New_Inv_Code +
                            " where INVESTOR_CODE=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateInvFolio, trans);

                        string updateInvMasterIPO = "update INVESTOR_MASTER_IPO   set      inv_code=" + New_Inv_Code +
                            ",AGENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateInvMasterIPO, trans);

                        string updateRevertal = "update REVERTAL_TRANSACTION  set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateRevertal, trans);

                        string updateTransIPO = "update TRANSACTION_IPO       set      inv_code=" + New_Inv_Code +
                            ",AGENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransIPO, trans);

                        string updateTranPayout = "update TRAN_PAYOUT@mf.bajajcapital           set      inv_code=" + New_Inv_Code +
                            " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTranPayout, trans);

                        string updateBajajArHead = "update BAJAJ_AR_HEAD         set     CLIENT_CD=" + New_Inv_Code +
                            ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where CLIENT_CD=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateBajajArHead, trans);

                        string updateTransNetBalance = "update TRAN_NET_BALANCE6@mf.bajajcapital      set   CLIENT_CODE=" + New_Inv_Code +
                            " where CLIENT_CODE=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransNetBalance, trans);

                        string updateTransIPO2 = "update TRAN_IPO              set      inv_code=" + New_Inv_Code +
                            ",CLIENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransIPO2, trans);

                        string updateTranLead = "update TRAN_LEAD             set      inv_code=" + New_Inv_Code +
                            " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTranLead, trans);

                        string updateLeadDetail = "update LEADS.LEAD_DETAIL     set      inv_code=" + New_Inv_Code +
                            " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateLeadDetail, trans);

                        string updatePortTransSt = "update port_TRANSACTION_ST@mf.bajajcapital        set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updatePortTransSt, trans);

                        string updateOnlineTransSt = "update online_transaction_st   set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateOnlineTransSt, trans);

                        string insertInvDelHist = "insert into Inv_Del_Hist_Agent_Merge (inv_code,new_inv_code,UpdateOn,UpdatedBy) values ('" +
                            rsDataRow["INV_CODE"] + "','" + New_Inv_Code + "',to_date('" + DateTime.Now.ToString("dd/MM/yyyy") +
                            "','dd/MM/yyyy'),'" + loginId + "')";
                        ExecuteNonQuery(insertInvDelHist, trans);

                        string updateTransStOnline = "update transaction_st_online    set client_code='" + New_Inv_Code +
                            "'      where client_code='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateTransStOnline, trans);

                        string updateOnlineClientReq = "update online_client_request    set inv_code='" + New_Inv_Code +
                            "'         where inv_code='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOnlineClientReq, trans);

                        string updateOnlineClientReqHist = "update online_client_request_hist    set inv_code='" + New_Inv_Code +
                            "'         where inv_code='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOnlineClientReqHist, trans);

                        string updateOnlineBusSum = "update online_business_summary  set client_codewm='" + New_Inv_Code +
                            "'    where client_codewm='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOnlineBusSum, trans);

                        string updateOfflineBusSum = "update offline_business_summary set client_codewm='" + New_Inv_Code +
                            "'    where client_codewm='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOfflineBusSum, trans);

                        if (rsInv_check.Rows.Count > 0)
                        {
                            string insertClientInvMergeLog = "insert into client_inv_merge_log values('" + New_Inv_Code +
                                "','" + rsDataRow["INV_CODE"] + "','" + loginId + "',sysdate)";
                            ExecuteNonQuery(insertClientInvMergeLog, trans);
                            string insertInvDel = "insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(insertInvDel, trans);
                            string deleteInvMaster = "Delete from INVESTOR_MASTER  where inv_code=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(deleteInvMaster, trans);
                            string deleteInvMasterBajaj = "Delete from INVESTOR_MASTER@mf.bajajcapital  where inv_code=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(deleteInvMasterBajaj, trans);
                        }
                    }

                    string updateInvestorMasterAfter = "update INVESTOR_MASTER           set BRANCH_CODE=" + branch_cd +
                        ",RM_CODE=" + Rm_cd + ",modify_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where source_id=" + mainAgent;
                    ExecuteNonQuery(updateInvestorMasterAfter, trans);

                    string updateAgentMasterAfter = "update agent_MASTER             set sourceid=" + branch_cd +
                        ",RM_CODE=" + Rm_cd + ",modify_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where agent_code=" + mainAgent;
                    ExecuteNonQuery(updateAgentMasterAfter, trans);

                    string updateTransactionStAfter = "update TRANSACTION_ST            set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionStAfter, trans);

                    string updateTransactionMFTemp1After = "update TRANSACTION_MF_TEMP1      set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionMFTemp1After, trans);

                    string updateTransactionStBajajAfter = "update TRANSACTION_ST@mf.bajajcapital            set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionStBajajAfter, trans);

                    string updatePortTransStAfter = "update port_TRANSACTION_ST@mf.bajajcapital            set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updatePortTransStAfter, trans);

                    string updateTransactionStTempAfter = "update TRANSACTION_STTEMP        set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionStTempAfter, trans);

                    string updateRedemAfter = "update REDEM                     set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateRedemAfter, trans);

                    string updatePaymentDetail = "update PAYMENT_DETAIL            set agent_code=" + mainAgent +
                        " where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(updatePaymentDetail, trans);

                    string updateLedger = "update LEDGER                    set AGENT_code=" + mainAgent +
                        " where AGENT_code=" + agentsArray[i];
                    ExecuteNonQuery(updateLedger, trans);

                    string updateUpfrontPaid = "update upfront_paid set client_agent_code='" + mainAgent +
                        "' where client_agent_code='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateUpfrontPaid, trans);

                    string updateAddIncentive = "update ADD_INCENTIVE_PAID set client_agent_code='" + mainAgent +
                        "' where client_agent_code='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateAddIncentive, trans);

                    string updateSIPBrokerBilling = "update SIP_BROKER_BILLING1 set SOURCE_CODE='" + mainAgent +
                        "' where SOURCE_CODE='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateSIPBrokerBilling, trans);

                    string updateSTPBrokerBilling = "update STP_BROKER_BILLING1 set SOURCE_CODE='" + mainAgent +
                        "' where SOURCE_CODE='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateSTPBrokerBilling, trans);

                    string updateAdvisorSubEntry = "update ADVISORSUBENTRY set anacode='" + mainAgent +
                        "' where anacode='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateAdvisorSubEntry, trans);


                    if (rsClient.Rows.Count > 0 && rsclient1.Rows.Count > 0)
                    {
                        DataRow rowClient = rsClient.Rows[0];
                        DataRow rowClient1 = rsclient1.Rows[0];

                        if ((rowClient["PHONE"] == DBNull.Value || string.IsNullOrWhiteSpace(rowClient["PHONE"].ToString())) && rowClient1["PHONE"] != DBNull.Value)
                        {
                            rowClient["PHONE"] = rowClient1["PHONE"];
                        }

                        if (rowClient["EMAIL"] == DBNull.Value && rowClient1["EMAIL"] != DBNull.Value)
                        {
                            rowClient["EMAIL"] = rowClient1["EMAIL"];
                        }
                        
                        if (rowClient["MOBILE"] == DBNull.Value && rowClient1["MOBILE"] != DBNull.Value && rowClient1["MOBILE"].ToString() != "0")
                        {
                            rowClient["MOBILE"] = rowClient1["MOBILE"];
                        }
                        
                        if (rowClient["PINCODE"] == DBNull.Value && rowClient1["PINCODE"] != DBNull.Value)
                        {
                            rowClient["PINCODE"] = rowClient1["PINCODE"];
                        }
                        
                        if (rowClient["CITY_ID"] == DBNull.Value && rowClient1["CITY_ID"] != DBNull.Value)
                        {
                            rowClient["CITY_ID"] = rowClient1["CITY_ID"];
                        }
                        
                        if (rowClient["DOB"] == DBNull.Value && rowClient1["DOB"] != DBNull.Value)
                        {
                            rowClient["DOB"] = rowClient1["DOB"];
                        }
                        
                        if (rowClient["EXIST_CODE"] == DBNull.Value && rowClient1["EXIST_CODE"] != DBNull.Value)
                        {
                            rowClient["EXIST_CODE"] = rowClient1["EXIST_CODE"];
                        }
                        
                        if (rowClient["TDS"] == DBNull.Value && rowClient1["TDS"] != DBNull.Value)
                        {
                            rowClient["TDS"] = rowClient1["TDS"];
                        }
                        
                        if (rowClient["INTRODUCER"] == DBNull.Value && rowClient1["INTRODUCER"] != DBNull.Value)
                        {
                            rowClient["INTRODUCER"] = rowClient1["INTRODUCER"];
                        }

                        if (rowClient1["JOININGDATE"] != DBNull.Value)
                        {
                            string clientJoining = rowClient["JOININGDATE"] != DBNull.Value ? Convert.ToDateTime(rowClient["JOININGDATE"]).ToString("dd/MM/yyyy") : "";
                            string client1Joining = Convert.ToDateTime(rowClient1["JOININGDATE"]).ToString("dd/MM/yyyy");
                            if (!CheckDate(clientJoining, client1Joining))
                            {
                                if (rowClient1.Table.Columns.Contains("creation_date") && rowClient1["creation_date"] != DBNull.Value)
                                    rowClient["JOININGDATE"] = rowClient1["creation_date"];
                                else
                                    rowClient["JOININGDATE"] = rowClient1["JOININGDATE"];
                            }
                        }
                        if (rowClient["JOININGDATE"] == DBNull.Value && rowClient1["JOININGDATE"] != DBNull.Value)
                        {
                            rowClient["JOININGDATE"] = rowClient1["JOININGDATE"];
                        }
                        if (rowClient1["LAST_TRAN_DT1"] != DBNull.Value)
                        {
                            string clientLastTran = rowClient["LAST_TRAN_DT1"] != DBNull.Value ? Convert.ToDateTime(rowClient["LAST_TRAN_DT1"]).ToString("dd/MM/yyyy") : "";
                            string client1LastTran = Convert.ToDateTime(rowClient1["LAST_TRAN_DT1"]).ToString("dd/MM/yyyy");
                            if (!CheckDate(client1LastTran, clientLastTran))
                            {
                                rowClient["LAST_TRAN_DT1"] = rowClient1["LAST_TRAN_DT1"];
                            }
                        }
                        if (rowClient["LAST_TRAN_DT1"] == DBNull.Value && rowClient1["LAST_TRAN_DT1"] != DBNull.Value)
                        {
                            rowClient["LAST_TRAN_DT1"] = rowClient1["LAST_TRAN_DT1"];
                        }

                        // Update the adjusted rsClient record back to the database.
                        string updateAgentMaster = "update agent_master set " +
                            "PHONE=" + (rowClient["PHONE"] != DBNull.Value ? "'" + rowClient["PHONE"].ToString() + "'" : "null") + "," +
                            "EMAIL=" + (rowClient["EMAIL"] != DBNull.Value ? "'" + rowClient["EMAIL"].ToString() + "'" : "null") + "," +
                            "MOBILE=" + (rowClient["MOBILE"] != DBNull.Value ? rowClient["MOBILE"].ToString() : "null") + "," +
                            "PINCODE=" + (rowClient["PINCODE"] != DBNull.Value ? rowClient["PINCODE"].ToString() : "null") + "," +
                            "CITY_ID=" + (rowClient["CITY_ID"] != DBNull.Value ? rowClient["CITY_ID"].ToString() : "null") + "," +
                            "DOB=" + (rowClient["DOB"] != DBNull.Value ? "TO_DATE('" + Convert.ToDateTime(rowClient["DOB"]).ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy')" : "null") + "," +
                            "EXIST_CODE=" + (rowClient["EXIST_CODE"] != DBNull.Value ? rowClient["EXIST_CODE"].ToString() : "null") + "," +
                            "TDS=" + (rowClient["TDS"] != DBNull.Value ? rowClient["TDS"].ToString() : "null") + "," +
                            "INTRODUCER=" + (rowClient["INTRODUCER"] != DBNull.Value ? rowClient["INTRODUCER"].ToString() : "null") +
                            (rowClient["JOININGDATE"] != DBNull.Value ? ",JOININGDATE=TO_DATE('" + Convert.ToDateTime(rowClient["JOININGDATE"]).ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy')" : "") +
                            (rowClient["LAST_TRAN_DT1"] != DBNull.Value ? ",LAST_TRAN_DT1=TO_DATE('" + Convert.ToDateTime(rowClient["LAST_TRAN_DT1"]).ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy')" : "") +
                            " where agent_code=" + mainAgent;
                        ExecuteNonQuery(updateAgentMaster, trans);
                    }

                    // Pankaj
                    string insertClientInvMergeLog2 = "insert into client_inv_merge_log values('" + mainAgent + "','" + agentsArray[i] + "','" + loginId + "',sysdate)";
                    ExecuteNonQuery(insertClientInvMergeLog2, trans);
                    
                    string insertAgentDel = "insert into agent_del select * from agent_master where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(insertAgentDel, trans);
                    
                    string deleteAgentMaster = "Delete from agent_master where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(deleteAgentMaster, trans);
                    
                    string deleteAgentMasterBajaj = "Delete from agent_master@mf.bajajcapital where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(deleteAgentMasterBajaj, trans);

                    // History of updations (investor wise)
                    string insertAgentDelHist = "insert into Agent_Del_Hist_Agent_Merge (agent_code,new_agent_code,UpdateOn,UpdatedBy) values ('" +
                        agentsArray[i] + "','" + mainAgent + "',to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'),'" + loginId + "')";
                    ExecuteNonQuery(insertAgentDelHist, trans);

             
                    trans.Commit();
                     
                    string Members = "";
                    Fam_Head = "";
                    string queryRsHead = "Select * from fp_investor where substr(familyhead_code,1,8)=" + mainAgent +
                        " and (fp_type='Snapshot' or Fp_type='Comprehensive') order by familyhead_code desc ";
                    DataTable rsHead = ExecuteQuery(queryRsHead);
                    if (rsHead.Rows.Count > 1)
                    {
                        Fam_Head = rsHead.Rows[0]["familyhead_code"].ToString();
                        Members1 = rsHead.Rows[0]["fam_mem1"].ToString();
                        string insertDupFpInvestor = "insert into dup_fp_investor select * from fp_investor where familyhead_code=" + Fam_Head;
                        ExecuteNonQuery(insertDupFpInvestor);
                        string updateFpInvestorDup = "update fp_investor set fam_mem1=fam_mem1||'#'||'" + Members1 + "' where substr(familyhead_code,1,8)=" + mainAgent +
                            " and (fp_type='Snapshot' or Fp_type='Comprehensive')";
                        ExecuteNonQuery(updateFpInvestorDup);
                    }
                    flag = false;
                    
                }

                pc.ShowAlert(this, "Client(s) Merged Successfully");
                //SetGrid();
            }
            catch (Exception err)
            {
                pc.ShowAlert(this, err.Message);
                return;
            }
            finally
            {
                if (MyConn != null && MyConn.State == ConnectionState.Open)
                    MyConn.Close();
            }
        }


        #endregion


    }




}
