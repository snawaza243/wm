using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Interop;
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
                    //fillRMListByBR1(null, null, loggedInUser);
                    fillCityList();


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
                    string existCode = ((Label)row.FindControl("lblExistCodeSearched")).Text;

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

    }
}