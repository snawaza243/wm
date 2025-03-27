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
using NPOI.SS.Formula.PTG;
using System.Windows.Media.TextFormatting;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Security.Cryptography;


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
                    fillCityList();

                    btnAddSelectedRows2.Attributes["onclick"] = "return false;";
                    btnAddSelectedRows2.CssClass += " disabled";  

                    btnSelectAgentToMerge.Enabled = false;
                    btnRemoveSelected.Enabled = false;
                    btnMerge.Enabled = false;
                    btnSetAsMainAgent.Enabled = false;

                    Session["AgentData"] = null;
                    Session["toAgent"] = null;

                }
            }
            else
            {
                pc.RedirectToWelcomePage();
            }

        }

        #region Old merge

        
     

        #region ddlSourceID_SelectedIndexChanged
        protected void ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSourceID = ddlSourceID.SelectedValue;  
            string loggedInUser = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(selectedSourceID))  
            { 
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
                fillRMListByBR2(null, selectedSourceID2, loggedInUser);
            }
            else
            {
            }


        }

        #endregion
        private void fillBranchListByLogin(string loginId)
        {
            /* VB CODE

            SELECT COUNT(ROLE_ID) INTO GlbDataFilter FROM userdetails_ji WHERE ROLE_ID = '72' AND login_id = '102097' AND ROWNUM = 1;

            IF GlbDataFilter > 0 THEN
                Select branch_code,branch_name from branch_master where category_id not in('1004','1005','1006') order by branch_name
            else
                select branch_code,branch_name from branch_master where branch_code in (" & Branches & ") and category_id not in('1004','1005','1006') order by branch_name
             */

            string sql1 = "SELECT nvl(COUNT(ROLE_ID), 0) AS GlbDataFilter FROM userdetails_ji WHERE ROLE_ID = '72' AND login_id = '" + loginId + "' AND ROWNUM = 1";
            DataTable SQL1_dt_toList = pc.ExecuteCurrentQuery(sql1);

            string sql2_branchlist = "";
            DataTable sql2_dt_toList_branchlist = new DataTable();

            if (SQL1_dt_toList.Rows.Count > 0)
            {
                DataRow sql1dt_toList_row = SQL1_dt_toList.Rows[0];
                string GlbDataFilter = sql1dt_toList_row["GlbDataFilter"]?.ToString();

                if (!string.IsNullOrEmpty(GlbDataFilter) && GlbDataFilter != "0")
                {
                    sql2_branchlist = "Select branch_code, branch_name from branch_master where category_id not in('1004', '1005', '1006') order by branch_name";
                }
                else
                {
                    sql2_branchlist = "select branch_code,branch_name from branch_master where branch_code in (" + pc.LogBranches() + ") and category_id not in('1004','1005','1006') order by branch_name";

                }
            }
            sql2_dt_toList_branchlist = pc.ExecuteCurrentQuery(sql2_branchlist);

            if (sql2_dt_toList_branchlist.Rows.Count > 0)
            {
                ddlSourceID.DataSource = sql2_dt_toList_branchlist;
                ddlSourceID.DataTextField = "BRANCH_NAME";
                ddlSourceID.DataValueField = "BRANCH_CODE";
                ddlSourceID.DataBind();
                ddlSourceID.Items.Insert(0, new ListItem("Select Branch", ""));

                anameringBranch.DataSource = sql2_dt_toList_branchlist;
                anameringBranch.DataTextField = "BRANCH_NAME";
                anameringBranch.DataValueField = "BRANCH_CODE";
                anameringBranch.DataBind();
                anameringBranch.Items.Insert(0, new ListItem("Select Branch", ""));
            }
            else
            {
                ddlSourceID.Items.Insert(0, new ListItem("No Branch", ""));
                anameringBranch.Items.Insert(0, new ListItem("No Branch.", ""));
            }

        }

        private void fillCityList()
        {
            DataTable dt_toList = new WM.Controllers.AgentController().GetCityList();
            anameringCity.DataSource = dt_toList;
            anameringCity.DataTextField = "CITY_NAME";
            anameringCity.DataValueField = "CITY_ID";
            anameringCity.DataBind();
            anameringCity.Items.Insert(0, new ListItem("Select City", ""));

        }

        private void fillRMListByBR1(string srmCode, string branch, string loginId)
        {
            DataTable dt_toList = new WM.Controllers.AgentController().GetEmployeeListByBranchOrRM(srmCode, branch, loginId);

            if (dt_toList.Rows.Count > 0)
            {

                ddlRM.DataSource = dt_toList;
                ddlRM.DataTextField = "RM_NAME";
                ddlRM.DataValueField = "RM_CODE";
                ddlRM.DataBind();
                ddlRM.Items.Insert(0, new ListItem("Select", ""));
                ddlRM.Enabled = true;

                anameringRm.DataSource = dt_toList;
                anameringRm.DataTextField = "RM_NAME";
                anameringRm.DataValueField = "RM_CODE";
                anameringRm.DataBind();
                anameringRm.Items.Insert(0, new ListItem("Select", ""));
                anameringRm.Enabled = true;
            }
            else
            {
                ddlRM.Enabled = false;
                ddlRM.Items.Insert(0, new ListItem("Not RM", ""));

                anameringRm.Enabled = false;
                anameringRm.Items.Insert(0, new ListItem("Not RM", ""));
            }

        }

        private void fillRMListByBR2(string srmCode, string branch, string loginId)
        {
            DataTable dt_toList = new WM.Controllers.AgentController().GetEmployeeListByBranchOrRM(srmCode, branch, loginId);

            if (dt_toList.Rows.Count > 0)
            {
                // Bind the fetched data to the RM dropdown
                anameringRm.DataSource = dt_toList;
                anameringRm.DataTextField = "RM_NAME";
                anameringRm.DataValueField = "RM_CODE";
                anameringRm.DataBind();
                anameringRm.Items.Insert(0, new ListItem("Select", ""));
                anameringRm.Enabled = true;
            }
            else
            {
                anameringRm.DataSource = null;
                anameringRm.DataBind();
                anameringRm.Items.Insert(0, new ListItem("No RM", ""));
                anameringRm.Enabled = false;


            }

        }
        private void fillRMList(string sourceID)
        {
            DataTable dt_toList = new WM.Controllers.AgentController().GetRMListBySource(sourceID);
            ddlRM.DataSource = dt_toList;
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


                AgentFilterData();

                /*
                // Pass all parameters to the controller method
                DataTable dt_toList = new WM.Controllers.AgentController().GetFilteredAgentsDataANA(SOURCEID, RM_CODE, AGENT_NAME, EXIST_CODE,
                    OLD_RM_CODE, OldExistCode, mobile, phone, category, city, sortBy, ADDRESS1, ADDRESS2, pan);

                if (dt_toList != null && dt_toList.Rows.Count > 0)
                {

                    string agentSearchedMasterInfo = $"Total record(s) found: {dt_toList.Rows.Count}";
                    lblAgentCodeSearchedMasterInfo.Text = agentSearchedMasterInfo;
                    agentsGridSearchedMaster.Visible = true;
                    agentsGridSearchedMaster.DataSource = dt_toList;
                    agentsGridSearchedMaster.DataBind();
                    btnAddSelectedRows2.Enabled = true;

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

                    btnAddSelectedRows2.Enabled = false;
                }

                */
            }
        }


        protected void agentsGridSearchedMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            agentsGridSearchedMaster.PageIndex = e.NewPageIndex;
            AgentFilterData();
        }

        private void AgentFilterData()
        {
            string sql = "";

            if (anameringCategory.SelectedValue.ToUpper().Trim() == "AGENT")
            {
                //sql = @"Select a.AGENT_NAME as agent_name , a.AGENT_CODE as agent_code , a.ADDRESS1 as ADDRESS1, a.ADDRESS2 as ADDRESS2, a.MOBILE, a.phone, a.email, a.pan, c.city_name, b.Branch_name as Branch_name, b.branch_code as branch_code, e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E, agent_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.agent_name IS NOT NULL";

                sql = @"Select a.AGENT_NAME as agent_name , a.AGENT_CODE as agent_code, a.exist_code as exist_code , a.ADDRESS1 as ADDRESS1, a.ADDRESS2 as ADDRESS2, a.phone, a.mobile, a.pan, a.email, e.rm_name, b.Branch_name as Branch_name, b.branch_code as branch_code FROM Branch_master b,EMPLOYEE_MASTER E, agent_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.agent_name IS NOT NULL";

                // Apply Filters Based on User Input
                if (
                    string.IsNullOrWhiteSpace(anameringCity.SelectedValue) &&
                    string.IsNullOrWhiteSpace(anameringBranch.SelectedValue) &&
                    (string.IsNullOrWhiteSpace(anameringRm.SelectedValue) || anameringRm.SelectedValue == "0") &&
                    string.IsNullOrWhiteSpace(anameringName.Text) &&
                    string.IsNullOrWhiteSpace(anameringAddress1.Text) &&
                    string.IsNullOrWhiteSpace(anameringAddress2.Text) &&
                    string.IsNullOrWhiteSpace(anameringEmail.Text) &&

                    string.IsNullOrWhiteSpace(anameringPan.Text) &&
                    //string.IsNullOrWhiteSpace(txtDOB.Text) &&
                    string.IsNullOrWhiteSpace(anameringPhone.Text) &&
                    string.IsNullOrWhiteSpace(anameringOldCode.Text) &&
                    string.IsNullOrWhiteSpace(anameringClientCode.Text) &&
                    string.IsNullOrWhiteSpace(anameringMobile.Text))
                {
                    // No filters applied
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(anameringOldCode.Text))
                    {
                        sql += " AND A.agent_code ='" + anameringOldCode.Text.Trim() + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(anameringClientCode.Text))
                    {
                        sql += " AND A.exist_code ='" + anameringClientCode.Text.Trim() + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(anameringName.Text))
                    {
                        sql += " AND UPPER(a.agent_name) LIKE '" + anameringName.Text.Trim().ToUpper() + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(anameringAddress1.Text))
                    {
                        sql += " AND UPPER(a.address1) LIKE '%" + anameringAddress1.Text.Trim().ToUpper() + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(anameringAddress2.Text))
                    {
                        sql += " AND UPPER(a.address2) LIKE '%" + anameringAddress2.Text.Trim().ToUpper() + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(anameringCity.SelectedValue))
                    {
                        sql += " AND UPPER(a.CITY_ID) = '" + anameringCity.SelectedValue.Trim().ToUpper() + "'";
                    }

                    if (!string.IsNullOrWhiteSpace(anameringBranch.SelectedValue))
                    {
                        sql += " AND UPPER(a.SOURCEID) = '" + anameringBranch.SelectedValue.Trim().ToUpper() + "'";
                    }

                    if (!string.IsNullOrWhiteSpace(anameringEmail.Text))
                    {
                        sql += " AND UPPER(a.EMAIL) = '" + anameringEmail.Text.Trim().ToUpper() + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(anameringRm.Text))
                    {
                        sql += " AND UPPER(a.rm_code) = '" + anameringRm.SelectedValue.Trim().ToUpper() + "'";
                    }

                    if (!string.IsNullOrWhiteSpace(anameringPhone.Text))
                    {
                        sql += " AND UPPER(a.Phone) = '" + anameringPhone.Text.Trim().ToUpper() + "'";
                    }

                    if (!string.IsNullOrWhiteSpace(anameringMobile.Text))
                    {
                        sql += " AND UPPER(a.mobile) = '" + anameringMobile.Text.Trim().ToUpper() + "'";
                    }

                    if (!string.IsNullOrWhiteSpace(anameringPan.Text))
                    {
                        sql += " AND UPPER(a.pan) = '" + anameringPan.Text.Trim().ToUpper() + "'";
                    }

                    //if (!string.IsNullOrEmpty(txtDOB.Text))
                    //{
                    //    sql += " AND a.dob = TO_DATE('" + txtDOB.Text.Trim() + "', 'DD-MM-YYYY')";
                    //}

                    sql += "AND b.branch_code in(" + pc.LogBranches() + ") and b.branch_tar_cat in (186, 615, 308)";

                }

                string currentSort = GetOrderByClause(anameringSortBy.SelectedValue);
                string sortBy = !string.IsNullOrEmpty(currentSort) ? currentSort : "ORDER BY UPPER(a.agent_name)";

                sql += " " + sortBy; // Append ORDER BY clause to the SQL query

            }

            // Execute SQL Query
            DataTable dt_toList = pc.ExecuteCurrentQuery(sql);

            try
            {

                if (dt_toList.Rows.Count > 0 && string.IsNullOrEmpty(pc.isException(dt_toList)))
                {
                    string agentSearchedMasterInfo = $"Total record(s) found: {dt_toList.Rows.Count}";
                    lblAgentCodeSearchedMasterInfo.Text = agentSearchedMasterInfo;
                    agentsGridSearchedMaster.Visible = true;
                    agentsGridSearchedMaster.DataSource = dt_toList;

                    agentsGridSearchedMaster.DataBind();
                    btnAddSelectedRows2.Enabled = true;

                    btnAddSelectedRows2.Attributes["onclick"] = "return true;";
                    btnAddSelectedRows2.CssClass = btnAddSelectedRows2.CssClass.Replace("disabled", "").Trim();

                }
                else
                {

                    string msg = $"No records found!";
                    ShowAlert(msg);
                    lblAgentCodeSearchedMasterInfo.Text = msg;

                    agentsGridSearchedMaster.Visible = false;
                    agentsGridSearchedMaster.DataSource = null;
                    agentsGridSearchedMaster.DataBind();

                    btnAddSelectedRows2.Attributes["onclick"] = "return false;";
                    btnAddSelectedRows2.CssClass += " disabled"; // Add Bootstrap disabled class

                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
            }
        }


        public string GetOrderByClause(string orderByField)
        {
            switch (orderByField.ToLower())
            {
                case "name":
                    return " ORDER BY UPPER(a.agent_name)";
                case "add1":
                    return "ORDER BY UPPER(a.address1)";
                case "add2":
                    return "ORDER BY UPPER(a.address2)";
                case "phone":
                    return "ORDER BY trim(a.phone)";
                case "city":
                    return "ORDER BY a.city_id";
                default:
                    return ""; // Default order
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

        // ADD SEARCHED AGENT INTO THE MERGIN LIST
        protected void btnAddSelectedRows_Click(object sender, EventArgs e)
        {
            try
            {
                bool isCheckAnyMasterRowAgent = false;
                bool isOtherBranch = false;
                bool anyRowSelected = false;
                bool isAgentExist = false;

                foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
                {
                    CheckBox chkSelectSearchedMaster = (CheckBox)row.FindControl("chkSelectSearchedMaster");

                    if (chkSelectSearchedMaster != null && chkSelectSearchedMaster.Checked)
                    {
                        isCheckAnyMasterRowAgent = true;
                        break;
                    }
                }

                if (!isCheckAnyMasterRowAgent)
                {
                    pc.ShowAlert(this, "Select any agent to add in list!");
                    return;
                }

                else if (isCheckAnyMasterRowAgent)
                {
                    foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
                    {
                        CheckBox chkSelectSearchedMaster = (CheckBox)row.FindControl("chkSelectSearchedMaster");

                        if (chkSelectSearchedMaster != null && chkSelectSearchedMaster.Checked)
                        {
                            string newBranchCode = ((Label)row.FindControl("lblBranchCodeSearchedMaster")).Text;
                            string newAgentCode = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;


                            foreach (GridViewRow targetRow in agentsGrid.Rows)
                            {
                                string existingBranchCode = ((Label)targetRow.FindControl("lblBranchCodeSearched")).Text;
                                string existingAgentCode = ((Label)targetRow.FindControl("lblAgentCodeSearched")).Text;

                                if (newBranchCode != existingBranchCode)
                                {
                                    isOtherBranch = true;
                                    break;
                                }

                                if (newAgentCode == existingAgentCode)
                                {
                                    isAgentExist = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isOtherBranch)
                {
                    pc.ShowAlert(this, "All selected agents should belongs from the same branch!");
                    return;
                }

                if (isAgentExist)
                {
                    pc.ShowAlert(this, "Agent(s) already added!!");
                    return;
                }
                else
                {
                    int currentLitCount = agentsGrid.Rows.Count;

                    ADD_TO_SEARCHED_LIST();

                    int afterLitCount = agentsGrid.Rows.Count;

                    if (afterLitCount == currentLitCount)
                    {
                        pc.ShowAlert(this, "Agent(s) already exist!");
                        return;
                    }

                    if (afterLitCount > currentLitCount)
                    {
                        pc.ShowAlert(this, "Agent(s) added!");
                        return;
                    }
                }



            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;

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
            try
            {
                if (agentsGrid.Rows.Count >= 1)
                {
                    if (mainAgentsGrid.Rows.Count > 0)
                    {
                        mainAgentsGrid.DataSource = null;
                        mainAgentsGrid.DataBind();
                        AddSelectedAgentToMain(agentsGrid);
                    }
                    else
                    {
                        AddSelectedAgentToMain(agentsGrid);
                    }
                }

                if (mainAgentsGrid.Rows.Count > 0 && agentsToMergeGrid.Rows.Count > 0)
                {
                    btnMerge.Enabled = true;
                }
                else
                {
                    btnMerge.Enabled = false;

                }

                if (agentsToMergeGrid.Rows.Count > 0)
                {
                    btnRemoveSelected.Enabled = true;
                }
                else
                {
                    btnRemoveSelected.Enabled = false;
                }


            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }

        }

        protected void btnSelectAgentToMerge_Click(object sender, EventArgs e)
        {

            try
            {
                AddToListToMerge();

                if (mainAgentsGrid.Rows.Count > 0 && agentsToMergeGrid.Rows.Count > 0)
                {
                    btnMerge.Enabled = true;
                }
                else
                {
                    btnMerge.Enabled = false;

                }

                if (agentsToMergeGrid.Rows.Count > 0)
                {
                    btnRemoveSelected.Enabled = true;
                }
                else
                {
                    btnRemoveSelected.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;

            }
        }


        private DataTable ConvertGridToDataTable(GridView grid)
        {
            DataTable dt = new DataTable();

            // ✅ Create columns based on GridView headers
            foreach (TableCell headerCell in grid.HeaderRow.Cells)
            {
                dt.Columns.Add(headerCell.Text);
            }

            // ✅ Add rows from GridView to DataTable
            foreach (GridViewRow row in grid.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text.Trim();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public void ADD_TO_SEARCHED_LIST()
        {
            try
            {
                DataTable dt_toList;
                bool sourceIDExists = false;

                // Retrieve or Initialize DataTable
                if (Session["AgentData"] != null)
                {
                    dt_toList = (DataTable)Session["AgentData"];
                }
                else
                {
                    dt_toList = new DataTable();
                    dt_toList.Columns.Add("AGENT_NAME");
                    dt_toList.Columns.Add("AGENT_CODE");
                    dt_toList.Columns.Add("EXIST_CODE");
                    dt_toList.Columns.Add("ADDRESS1");
                    dt_toList.Columns.Add("ADDRESS2");
                    dt_toList.Columns.Add("BranchName");
                    dt_toList.Columns.Add("BranchCode");
                }

                string firstBranchCode = null;

                // Loop through searched agents
                foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
                {
                    CheckBox chkSelectSearchedMaster = (CheckBox)row.FindControl("chkSelectSearchedMaster");

                    if (chkSelectSearchedMaster != null && chkSelectSearchedMaster.Checked)
                    {
                        string newBranchCode = ((Label)row.FindControl("lblBranchCodeSearchedMaster")).Text;

                        // Check if this is the first selected agent
                        if (firstBranchCode == null)
                        {
                            firstBranchCode = newBranchCode;
                        }
                        else if (firstBranchCode != newBranchCode)
                        {
                            sourceIDExists = true;
                            break;
                        }

                        // Check for duplicate agent
                        string agentCode = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;
                        DataRow[] existingRows = dt_toList.Select("AGENT_CODE = '" + agentCode + "'");
                        if (existingRows.Length > 0)
                        {
                            continue; // Skip if agent is already in the grid
                        }

                        // Add new row to the DataTable
                        DataRow newRow = dt_toList.NewRow();
                        newRow["AGENT_CODE"] = agentCode;
                        newRow["EXIST_CODE"] = ((Label)row.FindControl("lblExistCodeSearchedMaster")).Text;
                        newRow["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameSearchedMaster")).Text;
                        newRow["ADDRESS1"] = ((Label)row.FindControl("lblAddress1SearchedMaster")).Text;
                        newRow["ADDRESS2"] = ((Label)row.FindControl("lblAddress2SearchedMaster")).Text;
                        newRow["BranchName"] = ((Label)row.FindControl("lblBranchNameSearchedMaster")).Text;
                        newRow["BranchCode"] = newBranchCode;
                        dt_toList.Rows.Add(newRow);
                    }
                }

                if (sourceIDExists)
                {
                    pc.ShowAlert(this, "All selected agents should belong to the same branch!");
                    return;
                }

                // Store DataTable in Session for persistence
                Session["AgentData"] = dt_toList;

                // Bind updated data to grid
                agentsGrid.DataSource = dt_toList;
                agentsGrid.DataBind();


                if (agentsGrid.Rows.Count > 1)
                {
                    btnSetAsMainAgent.Enabled = true;
                    btnSelectAgentToMerge.Enabled = true;
                }
                else
                {
                    btnSetAsMainAgent.Enabled = false;
                    btnSelectAgentToMerge.Enabled = false;
                }

                if (agentsToMergeGrid.Rows.Count > 0)
                {
                    btnRemoveSelected.Enabled = true;
                }
                else
                {
                    btnRemoveSelected.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
            }
        }

        public void ADD_TO_SEARCHED_LIST_old()
        {
            try
            {
                DataTable dt_toList;

                bool anyRowSelected = false;
                bool sourceIDExists = false;

                if (agentsGrid.DataSource == null && agentsGrid.Rows.Count == 0)
                {
                    dt_toList = new DataTable();
                    dt_toList.Columns.Add("AGENT_NAME");
                    dt_toList.Columns.Add("agent_code");
                    dt_toList.Columns.Add("EXIST_CODE");
                    dt_toList.Columns.Add("ADDRESS1");
                    dt_toList.Columns.Add("ADDRESS2");
                    dt_toList.Columns.Add("BranchName");
                    dt_toList.Columns.Add("BranchCode");
                }
                else
                {

                    dt_toList = ((DataView)agentsGrid.DataSource).ToTable();
                }

                foreach (GridViewRow row in agentsGridSearchedMaster.Rows)
                {
                    CheckBox chkSelectSearchedMaster = (CheckBox)row.FindControl("chkSelectSearchedMaster");

                    if (chkSelectSearchedMaster != null && chkSelectSearchedMaster.Checked)
                    {
                        string newBranchCode = ((Label)row.FindControl("lblBranchCodeSearchedMaster")).Text;

                        foreach (GridViewRow targetRow in agentsGrid.Rows)
                        {
                            // Get the SourceID from the target grid row
                            string existingBranchCode = ((Label)targetRow.FindControl("lblBranchCodeSearched")).Text;

                            // Compare the SourceID values, if found different branch agent then break and show alert that agent should be the same branch
                            if (newBranchCode != existingBranchCode)
                            {
                                sourceIDExists = true;
                                break;
                            }
                        }

                        if (sourceIDExists)
                        {
                            string msg = "All selected agents should belongs from the same branch!";
                            pc.ShowAlert(this, msg);
                            return;
                        }
                        else
                        {
                            DataRow newRow = dt_toList.NewRow();
                            newRow["Agent_Code"] = ((Label)row.FindControl("lblAgentCodeSearchedMaster")).Text;
                            newRow["Exist_Code"] = ((Label)row.FindControl("lblExistCodeSearchedMaster")).Text;
                            newRow["Agent_Name"] = ((Label)row.FindControl("lblAgentNameSearchedMaster")).Text;
                            newRow["Address1"] = ((Label)row.FindControl("lblAddress1SearchedMaster")).Text;
                            newRow["Address2"] = ((Label)row.FindControl("lblAddress2SearchedMaster")).Text;
                            newRow["BranchName"] = ((Label)row.FindControl("lblBranchNameSearchedMaster")).Text;
                            newRow["BranchCode"] = newBranchCode;
                            dt_toList.Rows.Add(newRow);
                        }
                    }
                }

                agentsGrid.DataSource = dt_toList.DefaultView;
                agentsGrid.DataBind();
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }

        }

        private void AddSelectedAgentToMain(GridView agentsGrid)
        {
            bool isCheck =false;

            foreach (GridViewRow row in agentsGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    isCheck = true;
                }
            }

            if (!isCheck) {
                pc.ShowAlert(this, "Choose any agent to set as main!");
                return;
            }

                    DataTable dt_toList = new DataTable();
            dt_toList.Columns.Add("AGENT_NAME");

            dt_toList.Columns.Add("agent_code");
            dt_toList.Columns.Add("EXIST_CODE");
            dt_toList.Columns.Add("ADDRESS1");
            dt_toList.Columns.Add("ADDRESS2");
            dt_toList.Columns.Add("BranchName");
            dt_toList.Columns.Add("BranchCode");



            foreach (GridViewRow row in agentsGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    string AgentCode = ((Label)row.FindControl("lblAgentCodeSearched")).Text;

                    // Check if the EXIST_CODE already exists in mainAgentsGrid
                    bool existsInToMerge = false;
                    foreach (GridViewRow mainRow in agentsToMergeGrid.Rows)
                    {
                        string mainAgentCode = ((Label)mainRow.FindControl("lblAgentCodeToMerge")).Text;
                        if (mainAgentCode == AgentCode)
                        {
                            existsInToMerge = true;
                            break;
                        }
                    }

                    if (existsInToMerge)
                    {
                        string msg = "Agent is already selected in to merge list.";
                        pc.ShowAlert(this, msg);
                        return;
                    }
                    else
                    {
                        DataRow dr = dt_toList.NewRow();
                        dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                        dr["Agent_Code"] = AgentCode;
                        dr["Exist_code"] = ((Label)row.FindControl("lblExistCodeSearched")).Text;
                        dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1Searched")).Text;
                        dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2Searched")).Text;
                        dr["BranchName"] = ((Label)row.FindControl("lblBranchNameSearched")).Text;
                        dr["BranchCode"] = ((Label)row.FindControl("lblBranchCodeSearched")).Text;

                        dt_toList.Rows.Add(dr);
                    }

                    // Break after adding the first selected agent (if needed)
                    break;
                }
            }

            mainAgentsGrid.DataSource = dt_toList.DefaultView;
            mainAgentsGrid.DataBind();


            //UpdatePanel1.Update();
        }



        public void AddToListToMerge()
        {
            try
            {
                DataTable dtToList;

                // Initialize DataTable based on GridView state
                if (agentsToMergeGrid.Rows.Count == 0 || agentsToMergeGrid.DataSource == null)
                {
                    dtToList = new DataTable();
                    dtToList.Columns.Add("AGENT_NAME");
                    dtToList.Columns.Add("AGENT_CODE");
                    dtToList.Columns.Add("EXIST_CODE");
                    dtToList.Columns.Add("ADDRESS1");
                    dtToList.Columns.Add("ADDRESS2");
                    dtToList.Columns.Add("BranchName");
                    dtToList.Columns.Add("BranchCode");
                }
                else
                {
                    dtToList = ((DataView)agentsToMergeGrid.DataSource).ToTable();
                }

                // Loop through agentsGrid to find selected agents
                foreach (GridViewRow row in agentsGrid.Rows)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                    if (chkSelect != null && chkSelect.Checked)
                    {
                        string agentCode = ((Label)row.FindControl("lblAgentCodeSearched")).Text;

                        // Check if agent already exists in the mainAgentsGrid or agentsToMergeGrid
                        bool agentInMain = false;
                        bool agentInToMerge = false;

                        foreach (GridViewRow mainRow in mainAgentsGrid.Rows)
                        {
                            string mainAgentCode = ((Label)mainRow.FindControl("lblAgentCodeMain")).Text;
                            if (mainAgentCode == agentCode)
                            {
                                agentInMain = true;
                                break;
                            }

                            
                        }

                        foreach (GridViewRow mainRow in agentsToMergeGrid.Rows)
                        {
                             

                            string toMergeAgentCode = ((Label)mainRow.FindControl("lblAgentCodeToMerge"))?.Text;
                            if (toMergeAgentCode == agentCode)
                            {
                                agentInToMerge = true;
                                break;
                            }
                        }

                        if (agentInMain)
                        {
                            pc.ShowAlert(this, "Agent is already selected as main.");
                            continue;  // Skip adding this agent
                        }
                        else if (agentInToMerge)
                        {
                            pc.ShowAlert(this, "Agent is already in merge list.");
                            return;  // Skip adding this agent
                        }
                        else
                        {
                            DataRow dr = dtToList.NewRow();
                            dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameSearched")).Text;
                            dr["AGENT_CODE"] = agentCode;
                            dr["EXIST_CODE"] = ((Label)row.FindControl("lblExistCodeSearched")).Text;
                            dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1Searched")).Text;
                            dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2Searched")).Text;
                            dr["BranchName"] = ((Label)row.FindControl("lblBranchNameSearched")).Text;
                            dr["BranchCode"] = ((Label)row.FindControl("lblBranchCodeSearched")).Text;

                            dtToList.Rows.Add(dr);
                        }
                    }
                }

                agentsToMergeGrid.DataSource = dtToList;
                agentsToMergeGrid.DataBind();


                if (!ScriptManager.GetCurrent(this).IsInAsyncPostBack)
                {
                    agentsToMergeGrid.DataSource = dtToList.DefaultView;
                    agentsToMergeGrid.DataBind();
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
            }
        }

        protected void MergeAgents(object sender, EventArgs e)
        {
            string mainAgentCode = GetMainAgentCode();
            string[] toMergeAgentCodes = GetAllToMergeAgentCodes(); // Fetch as an array
            string loginId = Session["LoginId"]?.ToString();


            //MergeAgentsCall(mainAgentCode, toMergeAgentCodes, loginId); // with procedure
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
            string mainAgentCode = GetMainAgentCode();
            string toMergeAgentCodes = string.Join(", ", GetAllToMergeAgentCodes());

            // Set the text for the label
            lblAgentCodes.Text = $"Main agent exist code: {mainAgentCode}<br/>All To Merge agent exist codes: {toMergeAgentCodes}";
        }

        public string GetMainAgentCode()
        {
            // Assume only one main agent agents; initialize a variable to hold the agent_CODE value
            string agentCodeMain = string.Empty;

            // Check the first row in the main agents grid (assuming it has at least one row)
            if (mainAgentsGrid.Rows.Count > 0)
            {
                // Find the Label control that holds the agent_CODE in the first row
                Label lblagentCode = (Label)mainAgentsGrid.Rows[0].FindControl("lblAgentCodeMain");

                if (lblagentCode != null)
                {
                    // Set the agent_CODE value
                    agentCodeMain = lblagentCode.Text;
                }
            }

            // Return the main agent code (can be empty if no code found)
            return agentCodeMain;
        }

        public string[] GetAllToMergeAgentCodes()
        {
            // Initialize a list to hold the agent_CODE values for the agents to merge
            List<string> agentCodesToMerge = new List<string>();

            // Iterate through each row in the agentsToMergeGrid (correct GridView for merging agents)
            foreach (GridViewRow row in agentsToMergeGrid.Rows)
            {
                // Find the Label control that holds the agent_CODE in the current row
                Label lblagentCode = (Label)row.FindControl("lblagentCodeToMerge");

                if (lblagentCode != null)
                {
                    // Add the agent_CODE value to the list
                    agentCodesToMerge.Add(lblagentCode.Text);
                }
            }

            // Convert the list to an array and return it
            return agentCodesToMerge.ToArray();
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {
            string login = Session["loginId"]?.ToString();
            Session["AgentData"] = null;
            Session["toAgent"] = null;
            if (!string.IsNullOrEmpty(login))
            {
                Response.Redirect(Request.Url.AbsoluteUri);
            }
            else
            {
                pc.RedirectToWelcomePage();
            }

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
            //anameringCategory.SelectedIndex = 0; // Set to "Select"
            anameringCategory.Enabled = false;
            anameringBranch.SelectedIndex = 0; // Set to "Select"
            if (anameringRm.Items.Count > 0)
            {
                anameringRm.SelectedIndex = 0;
            }

            anameringCity.SelectedIndex = 0; // Set to "Select"
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
            anameringEmail.Text = string.Empty;
            // Optionally, reset the checkboxes if they are part of the form
            foreach (GridViewRow row in agentsGrid.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null)
                {
                    chkSelect.Checked = false;
                }
            }

            lblAgentCodeSearchedMasterInfo.Text = string.Empty;

            anameringOldRm.Enabled = false;  // Disable Old RM dropdown

            agentsGridSearchedMaster.DataSource = null;
            agentsGridSearchedMaster.DataBind();

            btnAddSelectedRows2.Attributes["onclick"] = "return false;";
            btnAddSelectedRows2.CssClass += " disabled"; // Add Bootstrap disabled class

        }


        protected void ExitButton_Click(object sender, EventArgs e)
        {
            pc.RedirectToWelcomePage();

        }

        protected void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            try
            {
                // Retrieve existing session data or initialize a new DataTable
                DataTable dtToList = new DataTable();
                dtToList.Columns.Add("AGENT_NAME");
                dtToList.Columns.Add("AGENT_CODE");
                dtToList.Columns.Add("EXIST_CODE");
                dtToList.Columns.Add("ADDRESS1");
                dtToList.Columns.Add("ADDRESS2");
                dtToList.Columns.Add("BranchName");
                dtToList.Columns.Add("BranchCode");

               
              

                // Loop through agentsToMergeGrid to find non-selected agents
                foreach (GridViewRow row in agentsToMergeGrid.Rows)
                {
                    CheckBox chkSelect = row.FindControl("chkSelectRightDown") as CheckBox;

                    if (chkSelect == null || !chkSelect.Checked)
                    {
                        // Add non-selected agent's information to the new DataTable
                        DataRow dr = dtToList.NewRow();
                        dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameToMerge"))?.Text;
                        dr["AGENT_CODE"] = ((Label)row.FindControl("lblAgentCodeToMerge"))?.Text;
                        dr["EXIST_CODE"] = ((Label)row.FindControl("lblExistCodeToMerge"))?.Text;
                        dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1ToMerge"))?.Text;
                        dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2ToMerge"))?.Text;
                        dr["BranchName"] = ((Label)row.FindControl("lblBranchNameToMerge"))?.Text;
                        dr["BranchCode"] = ((Label)row.FindControl("lblBranchCodeToMerge"))?.Text;
                        dtToList.Rows.Add(dr);
                    }
                }

                // Update GridView and session
                agentsToMergeGrid.DataSource = dtToList.DefaultView;
                agentsToMergeGrid.DataBind();

                if (dtToList.Rows.Count == 0)
                {
                    Session["toAgent"] = null;
                }
                else
                {
                    Session["toAgent"] = dtToList;
                }

                // Enable or disable buttons based on grid states
                btnRemoveSelected.Enabled = dtToList.Rows.Count > 0;
                btnMerge.Enabled = mainAgentsGrid.Rows.Count > 0 && dtToList.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
            }
        }

        protected void btnRemoveSelected_Click_old(object sender, EventArgs e)
        {
            try
            {
                bool icheckedAnyToRemove = false;
                foreach (GridViewRow row in agentsToMergeGrid.Rows)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelectRightDown");
                    if (chkSelect != null && chkSelect.Checked)
                    {
                        icheckedAnyToRemove = true;
                        break;
                    }
                }

                if (icheckedAnyToRemove)
                {
                    pc.ShowAlert(this, "Pleace check any agent to removec!");
                    return;
                }


                DataTable dt_toList = new DataTable();
                dt_toList.Columns.Add("AGENT_NAME");
                dt_toList.Columns.Add("agent_code");
                dt_toList.Columns.Add("EXIST_CODE");
                dt_toList.Columns.Add("ADDRESS1");
                dt_toList.Columns.Add("ADDRESS2");
                dt_toList.Columns.Add("BranchName");
                dt_toList.Columns.Add("BranchCode");


                // Loop through agentsGrid to find the selected agent(s)

               
                foreach (GridViewRow row in agentsToMergeGrid.Rows)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelectRightDown");
                    if (!chkSelect.Checked)
                    {
                        // Add the selected agent's information to the DataTable
                        DataRow dr = dt_toList.NewRow();
                        dr["AGENT_NAME"] = ((Label)row.FindControl("lblAgentNameToMerge")).Text;
                        dr["AGENT_CODE"] = ((Label)row.FindControl("lblAgentCodeToMerge")).Text;
                        dr["EXIST_CODE"] = ((Label)row.FindControl("lblExistCodeToMerge")).Text;
                        dr["ADDRESS1"] = ((Label)row.FindControl("lblADDRESS1ToMerge")).Text;
                        dr["ADDRESS2"] = ((Label)row.FindControl("lblADDRESS2ToMerge")).Text;
                        dr["BranchName"] = ((Label)row.FindControl("lblBranchNameToMerge")).Text;
                        dr["BranchCode"] = ((Label)row.FindControl("lblBranchCodeToMerge")).Text;
                        dt_toList.Rows.Add(dr);
                    }
                }


                agentsToMergeGrid.DataSource = dt_toList.DefaultView;
                agentsToMergeGrid.DataBind();


              

                
                if (agentsToMergeGrid.Rows.Count > 0)
                {
                    btnRemoveSelected.Enabled = true;
                }
                else
                {
                    btnRemoveSelected.Enabled = false;
                }


                if (mainAgentsGrid.Rows.Count > 0 && agentsToMergeGrid.Rows.Count > 0)
                {
                    btnMerge.Enabled = true;
                }
                else
                {
                    btnMerge.Enabled = false;

                }

               

           



            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;

            }
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

        #region NEW MERGIN, ith helping func



        private OracleConnection MyConn;
        private string branch_cd;
        private string Rm_cd;
        private int mCount = 0;
        private DateTime ServerDateTime = DateTime.Now;

        private bool CheckDate(string date1, string date2)
        {
            return string.Equals(date1, date2, StringComparison.OrdinalIgnoreCase);
        }

        private string Left(string s, int len)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Length >= len ? s.Substring(0, len) : s;
        }



        public void MergeClients(string mainAgent, string[] agentsArray, string loginId)
        {
            bool flag = false;
            try
            {
                try
                {
                    #region get et main agent branch and rm
                    branch_cd = "";
                    Rm_cd = "";

                    string q0_MAIN_BR_RM = "SELECT sourceid, rm_code FROM AGENT_MASTER WHERE AGENT_CODE = '" + mainAgent + "'";
                    DataTable q0_MAIN_BR_RM_dt_toList = pc.ExecuteCurrentQuery(q0_MAIN_BR_RM);
                    if (q0_MAIN_BR_RM_dt_toList.Rows.Count > 0 && string.IsNullOrEmpty(pc.isException(q0_MAIN_BR_RM_dt_toList)))
                    {
                        DataRow q0_MAIN_BR_RM_ROW = q0_MAIN_BR_RM_dt_toList.Rows[0];
                        branch_cd = q0_MAIN_BR_RM_ROW["sourceid"].ToString();
                        Rm_cd = q0_MAIN_BR_RM_ROW["rm_code"].ToString();
                    }

                    #endregion

                    string Fam_Head = "";
                    string Members1 = "";
                    string Members2 = "";
                    string Members3 = "";

                    for (int i = 0; i < agentsArray.Length; i++)
                    {
                        string q1_MainAgent = "Select * from agent_master where agent_code='" + mainAgent + "'";
                        DataTable q1_MainAgent_dt_toList = pc.ExecuteCurrentQuery(q1_MainAgent);

                        string q2_ith_Agent = "Select * from agent_master where agent_code=" + agentsArray[i];
                        DataTable q2_ith_Agent_dt_toList = pc.ExecuteCurrentQuery(q2_ith_Agent);

                        string q3_ith_AgentInv = "select inv_code,investor_name from investor_master where source_id=" + agentsArray[i];
                        DataTable q3_ith_AgentInv_dt_toList = pc.ExecuteCurrentQuery(q3_ith_AgentInv);

                        flag = true;

                        #region UPDATE, INSERT AND DELETE OF THE SAME TO AGENT INVESTOR

                        foreach (DataRow rsDataRow in q3_ith_AgentInv_dt_toList.Rows)
                        {
                            string investorName = rsDataRow["investor_name"].ToString();
                            string cleanedInvestorName = investorName.Trim().ToUpper().Replace(".", "").Replace(" ", "");
                            string searchPattern = "%" + Left(cleanedInvestorName, 8) + "%";
                            string queryRsInvCheck = "Select inv_code from investor_master where source_id=" + mainAgent + " and substr(replace(replace(trim(upper(investor_name)),'.',''),' ',''),1,8) like '" + searchPattern + "' and instr(trim(upper(investor_name)),'HUF')=0";
                            DataTable rsInv_check = pc.ExecuteCurrentQuery(queryRsInvCheck);

                            string New_Inv_Code = "";
                            if (rsInv_check.Rows.Count > 0 && string.IsNullOrEmpty(pc.isException(rsInv_check)))
                            {
                                New_Inv_Code = rsInv_check.Rows[0]["inv_code"].ToString();
                            }
                            else
                            {
                                mCount = mCount + 1;
                                if (mCount >= 999)
                                {
                                    New_Inv_Code = mainAgent + mCount.ToString("00000");
                                }
                                else
                                {
                                    New_Inv_Code = mainAgent + mCount.ToString("000");
                                    string updateInvMaster = "update INVESTOR_MASTER set   SOURCE_ID=" + mainAgent + ",BRANCH_CODE=" + branch_cd + ",RM_CODE=" + Rm_cd + ",INV_CODE='" + New_Inv_Code + "' where INV_CODE=" + rsDataRow["INV_CODE"];
                                    DataTable updateInvMaster_dt_toList = pc.ExecuteCurrentQuery(updateInvMaster);

                                }
                            }


                            #region UPDATE ALL RELATED TABLES
                            string updateFpInvestor1 = "update fp_investor set familyhead_code='" + New_Inv_Code + "' where familyhead_code='" + rsDataRow["inv_code"] + "'";
                            pc.ExecuteCurrentQuery(updateFpInvestor1);

                            string updateFpInvestor2 = "update fp_investor set fam_mem1=replace(fam_mem1," + rsDataRow["inv_code"] + "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) + "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                            pc.ExecuteCurrentQuery(updateFpInvestor2);

                            string updateFpInvestor3 = "update fp_investor set fam_mem2=replace(fam_mem2," + rsDataRow["inv_code"] + "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) + "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                            pc.ExecuteCurrentQuery(updateFpInvestor3);

                            string updateFpInvestor4 = "update fp_investor set fam_mem3=replace(fam_mem3," + rsDataRow["inv_code"] + "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) + "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                            pc.ExecuteCurrentQuery(updateFpInvestor4);

                            string updateTransSt = "update TRANSACTION_ST set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransSt);

                            string updateTransMFTemp1 = "update TRANSACTION_MF_TEMP1  set client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + " where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransMFTemp1);

                            string updateTransSt_Bajaj = "update TRANSACTION_ST@mf.bajajcapital set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + " where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransSt_Bajaj);

                            string updateTransStTemp = "update TRANSACTION_STTEMP set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransStTemp);

                            string updateRedem = "update REDEM@mf.bajajcapital set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + " where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateRedem);

                            string updateInvFolio = "update INVESTOR_FOLIO@mf.bajajcapital set INVESTOR_CODE=" + New_Inv_Code + " where INVESTOR_CODE=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateInvFolio);

                            string updateInvMasterIPO = "update INVESTOR_MASTER_IPO set inv_code=" + New_Inv_Code + ",AGENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateInvMasterIPO);

                            string updateRevertal = "update REVERTAL_TRANSACTION  set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + " where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateRevertal);

                            string updateTransIPO = "update TRANSACTION_IPO set inv_code=" + New_Inv_Code + ",AGENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransIPO);

                            string updateTranPayout = "update TRAN_PAYOUT@mf.bajajcapital set inv_code=" + New_Inv_Code + " where inv_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTranPayout);

                            string updateBajajArHead = "update BAJAJ_AR_HEAD set CLIENT_CD=" + New_Inv_Code + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where CLIENT_CD=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateBajajArHead);

                            string updateTransNetBalance = "update TRAN_NET_BALANCE6@mf.bajajcapital set   CLIENT_CODE=" + New_Inv_Code + " where CLIENT_CODE=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransNetBalance);

                            string updateTransIPO2 = "update TRAN_IPO set inv_code=" + New_Inv_Code + ",CLIENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTransIPO2);

                            string updateTranLead = "update TRAN_LEAD set inv_code=" + New_Inv_Code + " where inv_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateTranLead);

                            string updateLeadDetail = "update LEADS.LEAD_DETAIL set inv_code=" + New_Inv_Code + " where inv_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateLeadDetail);

                            string updatePortTransSt = "update port_TRANSACTION_ST@mf.bajajcapital set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                                " where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updatePortTransSt);

                            string updateOnlineTransSt = "update online_transaction_st set   client_code=" + New_Inv_Code + ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd + " where client_code=" + rsDataRow["INV_CODE"];
                            pc.ExecuteCurrentQuery(updateOnlineTransSt);

                            string insertInvDelHist = "insert into Inv_Del_Hist_Agent_Merge (inv_code,new_inv_code,UpdateOn,UpdatedBy) values ('" + rsDataRow["INV_CODE"] + "','" + New_Inv_Code + "',to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'),'" + loginId + "')";
                            pc.ExecuteCurrentQuery(insertInvDelHist);

                            string updateTransStOnline = "update transaction_st_online    set client_code='" + New_Inv_Code + "'      where client_code='" + rsDataRow["INV_CODE"] + "'";
                            pc.ExecuteCurrentQuery(updateTransStOnline);

                            string updateOnlineClientReq = "update online_client_request    set inv_code='" + New_Inv_Code + "'         where inv_code='" + rsDataRow["INV_CODE"] + "'";
                            pc.ExecuteCurrentQuery(updateOnlineClientReq);

                            string updateOnlineClientReqHist = "update online_client_request_hist    set inv_code='" + New_Inv_Code + "'         where inv_code='" + rsDataRow["INV_CODE"] + "'";
                            pc.ExecuteCurrentQuery(updateOnlineClientReqHist);

                            string updateOnlineBusSum = "update online_business_summary  set client_codewm='" + New_Inv_Code + "'    where client_codewm='" + rsDataRow["INV_CODE"] + "'";
                            pc.ExecuteCurrentQuery(updateOnlineBusSum);

                            string updateOfflineBusSum = "update offline_business_summary set client_codewm='" + New_Inv_Code + "'    where client_codewm='" + rsDataRow["INV_CODE"] + "'";
                            DataTable updateOfflineBusSum_dt_toList = pc.ExecuteCurrentQuery(updateOfflineBusSum);


                            #endregion

                            #region INSERT AND DELETE SECTION
                            if (rsInv_check.Rows.Count > 0)
                            {
                                string insertClientInvMergeLog = "insert into client_inv_merge_log values('" + New_Inv_Code + "','" + rsDataRow["INV_CODE"] + "','" + loginId + "',sysdate)";
                                pc.ExecuteCurrentQuery(insertClientInvMergeLog);

                                string insertInvDel = "insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code=" + rsDataRow["INV_CODE"];
                                pc.ExecuteCurrentQuery(insertInvDel);

                                string deleteInvMaster = "Delete from INVESTOR_MASTER  where inv_code=" + rsDataRow["INV_CODE"];
                                pc.ExecuteCurrentQuery(deleteInvMaster);

                                string deleteInvMasterBajaj = "Delete from INVESTOR_MASTER@mf.bajajcapital  where inv_code=" + rsDataRow["INV_CODE"];
                                pc.ExecuteCurrentQuery(deleteInvMasterBajaj);
                            }

                            #endregion
                        }

                        #endregion

                        #region UPDATE INVESTOR ON MAING AGENT

                        string updateInvestorMasterAfter = "update INVESTOR_MASTER set BRANCH_CODE=" + branch_cd + ",RM_CODE=" + Rm_cd + ",modify_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" + " where source_id=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateInvestorMasterAfter);

                        string updateAgentMasterAfter = "update agent_MASTER   set sourceid=" + branch_cd + ",RM_CODE=" + Rm_cd + ",modify_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" + " where agent_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateAgentMasterAfter);

                        string updateTransactionStAfter = "update TRANSACTION_ST  set   branch_code=" + branch_cd + ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" + " where source_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateTransactionStAfter);

                        string updateTransactionMFTemp1After = "update TRANSACTION_MF_TEMP1      set   branch_code=" + branch_cd + ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateTransactionMFTemp1After);

                        string updateTransactionStBajajAfter = "update TRANSACTION_ST@mf.bajajcapital  set   branch_code=" + branch_cd + ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateTransactionStBajajAfter);

                        string updatePortTransStAfter = "update port_TRANSACTION_ST@mf.bajajcapital  set   branch_code=" + branch_cd + ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updatePortTransStAfter);

                        string updateTransactionStTempAfter = "update TRANSACTION_STTEMP        set   branch_code=" + branch_cd + ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" + " where source_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateTransactionStTempAfter);

                        string updateRedemAfter = "update REDEM set   branch_code=" + branch_cd + ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                        pc.ExecuteCurrentQuery(updateRedemAfter);

                        string updatePaymentDetail = "update PAYMENT_DETAIL  set agent_code=" + mainAgent + " where agent_code=" + agentsArray[i];
                        pc.ExecuteCurrentQuery(updatePaymentDetail);

                        string updateLedger = "update LEDGER          set AGENT_code=" + mainAgent + " where AGENT_code=" + agentsArray[i];
                        pc.ExecuteCurrentQuery(updateLedger);

                        string updateUpfrontPaid = "update upfront_paid set client_agent_code='" + mainAgent + "' where client_agent_code='" + agentsArray[i] + "'";
                        pc.ExecuteCurrentQuery(updateUpfrontPaid);

                        string updateAddIncentive = "update ADD_INCENTIVE_PAID set client_agent_code='" + mainAgent + "' where client_agent_code='" + agentsArray[i] + "'";
                        pc.ExecuteCurrentQuery(updateAddIncentive);

                        string updateSIPBrokerBilling = "update SIP_BROKER_BILLING1 set SOURCE_CODE='" + mainAgent + "' where SOURCE_CODE='" + agentsArray[i] + "'";
                        pc.ExecuteCurrentQuery(updateSIPBrokerBilling);

                        string updateSTPBrokerBilling = "update STP_BROKER_BILLING1 set SOURCE_CODE='" + mainAgent + "' where SOURCE_CODE='" + agentsArray[i] + "'";
                        pc.ExecuteCurrentQuery(updateSTPBrokerBilling);

                        string updateAdvisorSubEntry = "update ADVISORSUBENTRY set anacode='" + mainAgent + "' where anacode='" + agentsArray[i] + "'";
                        pc.ExecuteCurrentQuery(updateAdvisorSubEntry);

                        #endregion

                        #region CLEAN, SET AND UPDATE MAIN AGENT AND TO AGENT DATA

                        if (q1_MainAgent_dt_toList.Rows.Count > 0 && q2_ith_Agent_dt_toList.Rows.Count > 0)
                        {
                            DataRow rowClient = q1_MainAgent_dt_toList.Rows[0];
                            DataRow rowClient1 = q2_ith_Agent_dt_toList.Rows[0];

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
                            pc.ExecuteCurrentQuery(updateAgentMaster);
                        }

                        #endregion

                        #region INSERT LOG AND DETLE AGENT BY MAIN AND TO iTH AGENT
                        // Pankaj
                        string insertClientInvMergeLog2 = "insert into client_inv_merge_log values('" + mainAgent + "','" + agentsArray[i] + "','" + loginId + "',sysdate)";
                        pc.ExecuteCurrentQuery(insertClientInvMergeLog2);

                        string insertAgentDel = "insert into agent_del select * from agent_master where agent_code=" + agentsArray[i];
                        pc.ExecuteCurrentQuery(insertAgentDel);



                        string deleteAgentMaster = "Delete from agent_master where agent_code=" + agentsArray[i];
                        pc.ExecuteCurrentQuery(deleteAgentMaster);

                        string deleteAgentMasterBajaj = "Delete from agent_master@mf.bajajcapital where agent_code=" + agentsArray[i];
                        pc.ExecuteCurrentQuery(deleteAgentMasterBajaj);



                        // History of updations (investor wise)
                        string insertAgentDelHist = "insert into Agent_Del_Hist_Agent_Merge (agent_code,new_agent_code,UpdateOn,UpdatedBy) values ('" +
                  agentsArray[i] + "','" + mainAgent + "',to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'),'" + loginId + "')";
                        pc.ExecuteCurrentQuery(insertAgentDelHist);

                        #endregion

                        #region INSERT AND UPDATE FP INVESTOR HEAD AND MEMBER
                        string Members = "";
                        Fam_Head = "";
                        string queryRsHead = "Select * from fp_investor where substr(familyhead_code,1,8)=" + mainAgent +
                  " and (fp_type='Snapshot' or Fp_type='Comprehensive') order by familyhead_code desc ";
                        DataTable rsHead = pc.ExecuteCurrentQuery(queryRsHead);
                        if (rsHead.Rows.Count > 1)
                        {
                            Fam_Head = rsHead.Rows[0]["familyhead_code"].ToString();
                            Members1 = rsHead.Rows[0]["fam_mem1"].ToString();
                            string insertDupFpInvestor = "insert into dup_fp_investor select * from fp_investor where familyhead_code=" + Fam_Head;
                            pc.ExecuteCurrentQuery(insertDupFpInvestor);
                            string updateFpInvestorDup = "update fp_investor set fam_mem1=fam_mem1||'#'||'" + Members1 + "' where substr(familyhead_code,1,8)=" + mainAgent +
                                " and (fp_type='Snapshot' or Fp_type='Comprehensive')";
                            pc.ExecuteCurrentQuery(updateFpInvestorDup);
                        }
                        #endregion

                    }
                }
                catch (Exception err)
                {
                    flag = false;
                }

                if (flag)
                {
                    pc.ShowAlert(this, "Client(s) Merged Successfully.");
                    pc.ExecuteCurrentQuery("COMMIT");
                    ReetOnMerge();
                    return;
                }
            }
            catch (Exception err)
            {
                if (!flag)
                {
                    pc.ExecuteCurrentQuery("ROLLBACK");
                }

                pc.ShowAlert(this, "Client(s) Not Merged!: " + err.Message);
                return;
            }
        }
        public void ReetOnMerge()
        {

        }
        #endregion


    }




}
