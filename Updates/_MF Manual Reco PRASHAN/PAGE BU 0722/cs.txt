using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using WM.Controllers;
using WM.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Configuration;

namespace WM.Masters
{
    public partial class MfManualReconciliation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillAMCList();
                FillChannelList();
                fillRMListUNFILTERD();
                fillbranchlkist("");
                FillRegionList();
            }
            //ToggleReconcileButton();
            UpdatePanelFirst.Update();


        }

        private void ToggleReconcileButton()
        {
            btnReconcile.Enabled = GridView1.Rows.Count > 0;
        }

        #region FillChannelList
        private void FillChannelList()
        {
            // Get the list of Channels from the controller
            DataTable dt = new WM.Controllers.MfManualReconciliationController().GetChannelList();


            // Bind the data to the DropDownList
            ddlChannel.DataSource = dt;
            ddlChannel.DataTextField = "itemname"; // Corresponding to itemname in the procedure
            ddlChannel.DataValueField = "itemserialnumber";   // Corresponding to itemserialnumber in the procedure
            ddlChannel.DataBind();
            ddlChannel.Items.Insert(0, new ListItem("", ""));
            ddlChannel.Items.Insert(1, new ListItem("All", ""));
        }
        #endregion

        protected void ddlBranchCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItemSerial = ddlChannel.SelectedValue;

            // Call the controller method to get branch details
            DataTable dt = new MfManualReconciliationController().GetBranchDetails(selectedItemSerial);

            ddlBranch.DataSource = dt;
            ddlBranch.DataTextField = "Branch_name";
            ddlBranch.DataValueField = "Branch_code";
            ddlBranch.DataBind();

            DropDownList2.DataSource = dt;
            DropDownList2.DataTextField = "Branch_name";
            DropDownList2.DataValueField = "Branch_code";
            DropDownList2.DataBind();

            ddlBranch.Items.Insert(0, new ListItem("", ""));
            DropDownList2.Items.Insert(0, new ListItem("", ""));

            ddlBranch.Items.Insert(1, new ListItem("All", ""));
            DropDownList2.Items.Insert(1, new ListItem("All", ""));


            DataTable rdt = new MfManualReconciliationController().Getregionbychannel(selectedItemSerial);

            ddlRegion.DataSource = rdt;
            ddlRegion.DataTextField = "REGION_NAME"; // Adjust according to your data field
            ddlRegion.DataValueField = "REGION_ID";  // Adjust according to your data field
            ddlRegion.DataBind();

            ddlRegion.Items.Insert(0, new ListItem("", ""));
            ddlRegion.Items.Insert(1, new ListItem("All", ""));

            DataTable zdt = new MfManualReconciliationController().Getzonebychannel(selectedItemSerial);

            ddlZone.DataSource = zdt;
            ddlZone.DataTextField = "ZONE_NAME"; // Adjust according to your data field
            ddlZone.DataValueField = "ZONE_ID";  // Adjust according to your data field
            ddlZone.DataBind();

            ddlZone.Items.Insert(0, new ListItem("", ""));
            ddlZone.Items.Insert(1, new ListItem("All", ""));


        }

        #region branch drop down bind

        private void fillbranchlkist(string check = "")
        {
                DataTable dt = new DataTable();
                dt = new WM.Controllers.MfManualReconciliationController().Getbranchdropdown();

            if (check == "1")
            {
                ddlBranch.DataSource = dt;
                ddlBranch.DataTextField = "Branch_name";
                ddlBranch.DataValueField = "Branch_code";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("", ""));
                ddlBranch.Items.Insert(1, new ListItem("All", ""));


                DropDownList2.DataSource = dt;
                DropDownList2.DataTextField = "Branch_name";
                DropDownList2.DataValueField = "Branch_code";
                DropDownList2.DataBind();

                DropDownList2.Items.Insert(0, new ListItem("", ""));

                DropDownList2.Items.Insert(1, new ListItem("All", ""));
            }
            else if (check == "2")
            {
                DropDownList2.DataSource = dt;
                DropDownList2.DataTextField = "Branch_name";
                DropDownList2.DataValueField = "Branch_code";
                DropDownList2.DataBind();

                DropDownList2.Items.Insert(0, new ListItem("", ""));

                DropDownList2.Items.Insert(1, new ListItem("All", ""));
            }
            else
            {
                ddlBranch.DataSource = dt;
                ddlBranch.DataTextField = "Branch_name";
                ddlBranch.DataValueField = "Branch_code";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("", ""));
                ddlBranch.Items.Insert(1, new ListItem("All", ""));

                DropDownList2.DataSource = dt;
                DropDownList2.DataTextField = "Branch_name";
                DropDownList2.DataValueField = "Branch_code";
                DropDownList2.DataBind();

                DropDownList2.Items.Insert(0, new ListItem("", ""));

                DropDownList2.Items.Insert(1, new ListItem("All", ""));

            }
        }
        #endregion 

        #region FillAMCList
        private void FillAMCList()
        {
            // Get the list of AMCs from the controller
            DataTable dt = new WM.Controllers.MfManualReconciliationController().GetAMCList(); // Ensure you have the correct controller reference


            // Bind the data to the DropDownList
            ddlAMC.DataSource = dt;
            ddlAMC.DataTextField = "MUT_NAME"; // Corresponding to MUT_NAME in the procedure
            ddlAMC.DataValueField = "MUT_CODE";   // Corresponding to MUT_CODE in the procedure
            ddlAMC.DataBind();
            DropDownList1.DataSource = dt;
            DropDownList1.DataTextField = "MUT_NAME"; // Corresponding to MUT_NAME in the procedure
            DropDownList1.DataValueField = "MUT_CODE";   // Corresponding to MUT_CODE in the procedure
            DropDownList1.DataBind();

            ddlAMC.Items.Insert(0, new ListItem("", ""));
            DropDownList1.Items.Insert(0, new ListItem("", ""));

            ddlAMC.Items.Insert(1, new ListItem("All", ""));
            DropDownList1.Items.Insert(1, new ListItem("All", ""));
        }
        #endregion



        #region fillRegionList
        private void FillRegionList()
        {
            // Get the region list from the controller
            DataTable dt = new MfManualReconciliationController().GetRegion();

            if (dt != null && dt.Rows.Count > 0)
            {
                // Set the data source for the DropDownList
                ddlRegion.DataSource = dt;
                ddlRegion.DataTextField = "REGION_NAME"; // Adjust according to your data field
                ddlRegion.DataValueField = "REGION_ID";  // Adjust according to your data field
                ddlRegion.DataBind();

                //  DropDownList2.DataTextField = ddlBranch.SelectedItem.Text.Trim();
                //  DropDownList2.DataValueField = ddlBranch.SelectedValue;
            }
            else
            {
                // If no regions are found, you may want to clear the DropDownList
                ddlRegion.Items.Clear();
            }

            ddlRegion.Items.Insert(0, new ListItem("", ""));
            ddlRegion.Items.Insert(1, new ListItem("All", ""));
        }
        #endregion

        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRegionId = ddlRegion.SelectedValue;
            FillZoneList(selectedRegionId);
        }

        #region FillZoneList
        private void FillZoneList(string regionId)
        {
            // Assume branches are stored in a session or other variable
            //string branches = (string)Session["Branches"];

            // Get the zones list from the controller
            DataTable dt = new MfManualReconciliationController().GetZonesByRegion(regionId);

            if (dt != null && dt.Rows.Count > 0)
            {
                // Clear existing items and set the data source for the DropDownList
                ddlZone.DataSource = dt;
                ddlZone.DataTextField = "ZONE_NAME";
                ddlZone.DataValueField = "ZONE_ID";
                ddlZone.DataBind();
            }
            else
            {
                // Clear the dropdown if no zones are found
                ddlZone.Items.Clear();
            }

            ddlZone.Items.Insert(0, new ListItem("", ""));
            ddlZone.Items.Insert(1, new ListItem("All", ""));
        }
        #endregion

        protected void ddlzone_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedbranchbyzoneId = ddlZone.SelectedValue;
            FillbranchbyzoneList(selectedbranchbyzoneId);
        }

        protected void FillbranchbyzoneList(string selectedbranchbyzoneId)   {
          
            // Call the controller method to get branch details
            DataTable dt = new MfManualReconciliationController().GetBranchbyzoneDetails(selectedbranchbyzoneId);

            ddlBranch.DataSource = dt;
            ddlBranch.DataTextField = "Branch_name";
            ddlBranch.DataValueField = "Branch_code";
            ddlBranch.DataBind();

            DropDownList2.DataSource = dt;
            DropDownList2.DataTextField = "Branch_name";
            DropDownList2.DataValueField = "Branch_code";
            DropDownList2.DataBind();

            ddlBranch.Items.Insert(0, new ListItem("", ""));
            DropDownList2.Items.Insert(0, new ListItem("", ""));

            ddlBranch.Items.Insert(1, new ListItem("All", ""));
            DropDownList2.Items.Insert(1, new ListItem("All", ""));

        }

        protected void ddlRmFill_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItemSerial = ddlBranch.SelectedValue;

            // Call the controller method to get branch details
            // DataTable dt = new MfManualReconciliationController().GetBranchDetails(selectedItemSerial);
            DataTable dt = new WM.Controllers.MfManualReconciliationController().GetRM(selectedItemSerial);
            ddlRM.DataSource = dt;
            ddlRM.DataTextField = "RM_NAME";
            ddlRM.DataValueField = "payroll_id";
            ddlRM.DataBind();
            ddlRM.Items.Insert(0, new ListItem("", ""));
            ddlRM.Items.Insert(1, new ListItem("All", ""));
        }

        #region fillRmList
        private void fillRMListUNFILTERD()

        {
            DataTable dt = new WM.Controllers.MfManualReconciliationController().GetRMUNFILTERED();
            ddlRM.DataSource = dt;
            ddlRM.DataTextField = "RM_NAME";
            ddlRM.DataValueField = "payroll_id";
            ddlRM.DataBind();
            ddlRM.Items.Insert(0, new ListItem("", ""));
            ddlRM.Items.Insert(1, new ListItem("All", ""));
        }

        #endregion

        #region btnExit_Click
        protected void btnExit_Click(object sender, EventArgs e)
        {
            string loginId = Session["LoginId"]?.ToString();
            string roleId = Session["roleId"]?.ToString();
            Response.Redirect($"~/welcome?loginid={loginId}&roleid={roleId}");
        }
        #endregion

        protected void btnSearchtrn_Click(object sender, EventArgs e)
        {
            SipMasterModel searchModel = new SipMasterModel
            {
                TranCode = string.IsNullOrEmpty(tarnCode.Text) ? (string)null : tarnCode.Text,
            };

            DataTable transactions = new WM.Controllers.sip_master_reconciliationController().GetBranchName(searchModel);
            tranCodeGrid.DataSource = transactions;
            tranCodeGrid.DataBind();
        }


        #region btnReset_Click
        protected void btnReset_Click(object sender, EventArgs e)
        {
            // Reset DropDownLists to the first item
            ddlRM.Items.Clear();
            ddlAMC.SelectedIndex = 0;
            ddlZone.Items.Clear();

            lblRowCount.Text = string.Empty;

            
          //  FillAMCList();
            FillChannelList();
            fillbranchlkist("1");
            FillRegionList();

            // Clear TextBoxes
          //  txtDateFrom.Text = string.Empty;
          //  txtDateTo.Text = string.Empty;
            txtAR.Text = string.Empty;
            txtARNo.Text = string.Empty;

            // Reset RadioButtonLists to the first item

            //rblReconciliation.SelectedIndex = -1;
            rblTranType.SelectedIndex = -1;
            rblRegistrar.SelectedIndex = -1;


            // Uncheck CheckBox
            cbCOB.Checked = false;

            // Optionally reset GridView (if needed)
            GridTransaction.DataSource = null;
            GridTransaction.DataBind();
        }
        #endregion

        protected void Button1_Click(object sender , EventArgs e)
        {
            Clearfieldsofrta();
        }

        #region Button1_Click
        protected void Clearfieldsofrta()
        {
            tarnCode.Text = string.Empty;
            tranCodeGrid.DataSource = null;
            tranCodeGrid.DataBind();

            // Clear all TextBox controls
            dateFromRta.Text = string.Empty;
            dateToRta.Text = string.Empty;
            txtChequeSearch.Text = string.Empty;
            txtInvestorName.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtSearching.Text = string.Empty;
            txtRemarks.Text = string.Empty;

            // Clear all DropDownList controls
            DropDownList1.SelectedIndex = 0;
            fillbranchlkist("1");

           // fillbranchlkist("2");
            ddlChequeNo.SelectedIndex = 0;

            // Clear all RadioButtonList selections and items
            rblReconciliationType.ClearSelection();
            //rblReconciliationType.Items.Clear();

            RadioButtonList1.ClearSelection();
            //RadioButtonList1.Items.Clear();

            // Clear GridView
            GridView1.DataSource = null;
            GridView1.DataBind();
        }
        #endregion

        #region btnGo_Click
        protected void btnGo_Click(object sender, EventArgs e)
        {
            // Extract values from the input fields
            //string channel = ddlChannel.SelectedValue;
            string region = ddlRegion.SelectedValue;
            string zone = ddlZone.SelectedValue;
            string branch = ddlBranch.SelectedValue;
            string rm = ddlRM.SelectedValue;

            string dateFrom = txtDateFrom.Text.Trim();
            string dateTo = txtDateTo.Text.Trim();

            try
            {
                // Convert dates to MM-dd-yyyy format
                if (!string.IsNullOrWhiteSpace(dateFrom))
                {
                    dateFrom = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", null).ToString("MM/dd/yyyy");
                }

                if (!string.IsNullOrWhiteSpace(dateTo))
                {
                    dateTo = DateTime.ParseExact(dateTo, "dd/MM/yyyy", null).ToString("MM/dd/yyyy");
                }
            }
            catch (FormatException ex)
            {
                // Handle invalid date format
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Invalid date format. Please use dd-MM-yyyy format.');", true);
                return;
            }

            // Now dateFrom and dateTo are in MM-dd-yyyy format

            string amc = ddlAMC.SelectedValue;
            string ar = txtAR.Text.Trim();

            string reconciliationStatus = rblReconciliation.SelectedValue;

            string cobFlag = cbCOB.Checked ? "1" : null;

            string arNo = txtARNo.Text.Trim();

            string tranType = rblTranType.SelectedValue;
            string registrar = rblRegistrar.SelectedValue;

            // Check if at least one criterion is selected
            if (string.IsNullOrWhiteSpace(region) && string.IsNullOrWhiteSpace(zone) &&
                string.IsNullOrWhiteSpace(branch) && string.IsNullOrWhiteSpace(rm) &&
                string.IsNullOrWhiteSpace(dateFrom) && string.IsNullOrWhiteSpace(dateTo) &&
                string.IsNullOrWhiteSpace(amc) && string.IsNullOrWhiteSpace(ar) &&
                string.IsNullOrWhiteSpace(arNo) && string.IsNullOrWhiteSpace(tranType)
               )
            {
                // Display alert if no criteria are provided
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Please provide at least one search criterion.');", true);
                return;
            }

            // Call a method to perform the action (e.g., populate a grid or display results)
            PerformSearch(region, zone, branch, rm, dateFrom, dateTo, amc, ar, reconciliationStatus, cobFlag, arNo, tranType, registrar);
        }

        private void PerformSearch(string region, string zone, string branch, string rm,
                                   string dateFrom, string dateTo, string amc, string ar, string reconciliationStatus, string cobFlag
                                  , string arNo, string tranType, string registrar)
        {
            DataTable dt = new MfManualReconciliationController().GetClientDetails(region, zone, branch, rm, dateFrom, dateTo, amc, ar, reconciliationStatus, cobFlag, arNo, tranType, registrar);

            // Bind the retrieved data to the GridView
            GridTransaction.Visible = true;
            GridTransaction.DataSource = dt;
            GridTransaction.DataBind();
           // GridTransaction.Focus();

            // Update the row count in the Label
            lblRowCount.Text = dt.Rows.Count.ToString();
        }
        #endregion

        #region btnSearch_Click
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Get values from form
            DateTime? dateFrom = null;
            DateTime? dateTo = null;

            // Convert dd/MM/yyyy to MM/dd/yyyy
            if (!string.IsNullOrEmpty(dateFromRta.Text))
            {
                // Try parsing the input date in dd/MM/yyyy format first
                DateTime tempDate;
                if (DateTime.TryParseExact(dateFromRta.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out tempDate))
                {
                    // Convert to MM/dd/yyyy format
                    dateFrom = tempDate; // Assign the parsed date
                }
                else
                {
                    // Return or handle error if invalid date format for DateFrom
                    return; // Or another error handling action as required
                }
            }

            if (!string.IsNullOrEmpty(dateToRta.Text))
            {
                // Try parsing the input date in dd/MM/yyyy format first
                DateTime tempDate;
                if (DateTime.TryParseExact(dateToRta.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out tempDate))
                {
                    // Convert to MM/dd/yyyy format
                    dateTo = tempDate; // Assign the parsed date
                }
                else
                {
                    // Return or handle error if invalid date format for DateTo
                    return; // Or another error handling action as required
                }
            }

            string status = rblReconciliationType.SelectedValue;
            string amc = DropDownList1.SelectedValue;
            string branch = DropDownList2.SelectedValue;
            string chequeType = ddlChequeNo.SelectedValue;
            string chequeSearch = txtChequeSearch.Text;
            string investorName = txtInvestorName.Text;
            string amount = txtAmount.Text;
            string tranType = RadioButtonList1.SelectedValue;
            string searchText = txtSearching.Text;

            // Validation: Ensure at least one search criterion is provided
            if (dateFrom == null && dateTo == null &&
                string.IsNullOrWhiteSpace(amc) &&
                string.IsNullOrWhiteSpace(branch) && string.IsNullOrWhiteSpace(chequeType) &&
                string.IsNullOrWhiteSpace(chequeSearch) && string.IsNullOrWhiteSpace(investorName) &&
                string.IsNullOrWhiteSpace(amount) &&
                string.IsNullOrWhiteSpace(searchText))
            {
                // Display alert if no criteria are provided
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Please provide at least one search criterion.');", true);
                return;
            }

            PerformSearch2(dateFrom, dateTo, status, amc, branch, chequeType, chequeSearch, investorName, amount, tranType, searchText);
            // Call the method to search transactions
           // btnSearch.Focus();

        }

        private void PerformSearch2(DateTime? dateFrom, DateTime? dateTo, string status, string amc,
                                  string branch, string chequeType, string chequeSearch, string investorName, string amount
                                 , string tranType, string searchText)
        {
            DataTable dt = new MfManualReconciliationController().SearchTransactions(dateFrom, dateTo, status, amc, branch, chequeType, chequeSearch, investorName, amount, tranType, searchText);

            // Bind results to GridView or any display control
            GridView1.Visible = true;
            GridView1.DataSource = dt;
            GridView1.DataBind();
           // GridView1.Focus();
        }
        #endregion

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Clear any previous content and headers
            Response.Clear();
            Response.Buffer = true;

            // Set the response type to Excel
            Response.AddHeader("content-disposition", "attachment;filename=TransactionData.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            // Hide the CheckBox column during export (optional, if needed)
            GridTransaction.Columns[0].Visible = false; // Adjust column index based on CheckBox location

            // Use a StringWriter and HtmlTextWriter to write the GridView data
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // Remove paging to export all data
                    GridTransaction.AllowPaging = false;

                    // Optionally rebind data if needed
                    // BindGridData();

                    // Render the GridTransaction content
                    GridTransaction.RenderControl(hw);

                    // Output the rendered HTML as Excel-compatible content
                    string excelData = sw.ToString();
                    Response.Output.Write(excelData);
                    Response.Flush();
                    Response.End();
                }
            }

            // Re-show the CheckBox column after export (optional)
            GridTransaction.Columns[0].Visible = true;

            // Optionally re-enable paging if it was enabled before
            GridTransaction.AllowPaging = true;
            // Rebind data if needed to show paginated data again
            // BindGridData();
        }
        protected void btnExport1_Click(object sender, EventArgs e)
        {
            // Clear any previous content and headers
            Response.Clear();
            Response.Buffer = true;

            // Set the response type to Excel
            Response.AddHeader("content-disposition", "attachment;filename=RTATransactionData.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            // Hide the CheckBox column during export
            GridView1.Columns[0].Visible = false;  // Assuming the CheckBox is in the first column (index 0)

            // Use a StringWriter and HtmlTextWriter to write the GridView data
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // Remove paging to export all data
                    GridView1.AllowPaging = false;

                    // Optionally rebind data if needed
                    // BindGridData();

                    // Render the GridView content
                    GridView1.RenderControl(hw);

                    // Output the rendered HTML as Excel-compatible content
                    string excelData = sw.ToString();
                    Response.Output.Write(excelData);
                    Response.Flush();
                    Response.End();
                }
            }

            // Re-show the CheckBox column after export
            GridView1.Columns[0].Visible = true;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for rendering GridView in an Excel-friendly format
        }


        private string tranCodetra
        {
            get { return ViewState["tranCodetra"] as string ?? string.Empty; }
            set { ViewState["tranCodetra"] = value; }
        }

        //protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        //{
        //    // Find the row where the checkbox is checked
        //    CheckBox chk = (CheckBox)sender;
        //    GridViewRow row = (GridViewRow)chk.NamingContainer;

        //    // Uncheck all other checkboxes
        //    foreach (GridViewRow gridRow in GridTransaction.Rows)
        //    {
        //        CheckBox otherChk = (CheckBox)gridRow.FindControl("chkSelect");
        //        if (otherChk != null && otherChk != chk)
        //        {
        //            otherChk.Checked = false;  // Uncheck other checkboxes
        //        }
        //    }

        //    SelectedRowIndex = row.RowIndex;
        //    // GridTransaction.Focus();

        //    // Save the selected row index in hidden field
        //    int selectedIndex = row.RowIndex;
        //    hfSelectedRow.Value = selectedIndex.ToString();

        //    Clearfieldsofrta();

        //    // Get values from the selected row
        //    HiddenField hfTranCode = (HiddenField)row.FindControl("hfTranCode");
        //    string tranCodetrap = hfTranCode.Value;
        //    tranCodetra = tranCodetrap.ToString();

           
        //    string tranDateStr = ((Label)row.FindControl("lblTrnDate")).Text;
        //    string tranType = ((Label)row.FindControl("llTrnType")).Text;
        //    string investorName = ((Label)row.FindControl("lblInvestorNam")).Text;
        //    string amcCode = ((Label)row.FindControl("lblAC")).Text;
        //    string branch = ((Label)row.FindControl("lblbranhna")).Text;
        //    string amount = ((Label)row.FindControl("lblmount")).Text;
        //    string siptype = ((Label)row.FindControl("lblSipType")).Text;
        //    string chqno = ((Label)row.FindControl("lblCqNo")).Text;
        //    // Get the values from the labels
        //    string registrar = ((Label)row.FindControl("lblregis")).Text;
        //    string cobfl = ((Label)row.FindControl("lblCOBFL")).Text;

        //    // Set the radio button based on the conditions
        //    if (registrar == "C" && cobfl == "0")
        //    {
        //        rblRegistrar.SelectedValue = "c";  // Select "C"
        //    }
        //    else if (registrar == "K" && cobfl == "0")
        //    {
        //        rblRegistrar.SelectedValue = "k";  // Select "K"
        //    }
        //    else if (registrar == "C" && cobfl == "1")
        //    {
        //        rblRegistrar.SelectedValue = "ccob";  // Select "C COB"
        //    }
        //    else if (registrar == "K" && cobfl == "1")
        //    {
        //        rblRegistrar.SelectedValue = "kcob";  // Select "K COB"
        //    }


        //    // Populate the form fields
        //    DateTime tranDate;
        //    string inputFormat = "dd/MM/yyyy"; // Specify the expected input format
        //    if (DateTime.TryParseExact(tranDateStr, inputFormat, null, System.Globalization.DateTimeStyles.None, out tranDate))
        //    {
        //        // Set the text in dd/MM/yyyy format with one-month adjustments
        //        dateFromRta.Text = tranDate.AddMonths(-1).ToString("dd/MM/yyyy"); // One month before
        //        dateToRta.Text = tranDate.AddMonths(1).ToString("dd/MM/yyyy");   // One month after
        //    }
        //    else
        //    {
        //        // Handle the error: tranDateStr is not a valid date
        //        dateFromRta.Text = string.Empty;
        //        dateToRta.Text = string.Empty;
        //    }

        //    // Assuming same date for "Date From" and "Date To", you can modify it
        //    rblReconciliationType.SelectedValue = tranType.ToLower() == "reconciled" ? "reconciled-rta" : "unreconciled-rta";
        //    RadioButtonList1.SelectedValue = siptype.ToLower() == "regular" ? "regular" : "sip";
        //    // Validate and set the selected value for DropDownList1
        //    if (DropDownList1.Items.FindByValue(amcCode) != null)
        //    {
        //        DropDownList1.SelectedValue = amcCode;
        //    }
        //    else
        //    {
        //        // Handle the case where the value is not found (optional)
        //        DropDownList1.SelectedIndex = 0; // Set to the first option (blank or default)
        //    }

        //    // Validate and set the selected value for DropDownList2
        //    if (DropDownList2.Items.FindByValue(branch) != null)
        //    {
        //        DropDownList2.SelectedValue = branch;
        //    }
        //    else
        //    {
        //        // Handle the case where the value is not found (optional)
        //        DropDownList2.SelectedIndex = 0; // Set to the first option (blank or default)
        //    }

        //    txtAmount.Text = amount;
        //    txtInvestorName.Text = investorName;
        //    ddlChequeNo.SelectedValue = "001";
        //    txtChequeSearch.Text = chqno;

        //}

        //protected void ddlChequeNo_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    // Check if a row has been selected
        //    if (SelectedRowIndex >= 0 && SelectedRowIndex < GridTransaction.Rows.Count)
        //    {
        //        // Get the selected GridViewRow
        //        GridViewRow row = GridTransaction.Rows[SelectedRowIndex];

        //        // Get the selected field from the DropDownList
        //        string selectedField = ddlChequeNo.SelectedItem.ToString();

        //        // Retrieve the value for the selected field
        //        string fieldValue = string.Empty;
        //        switch (selectedField)
        //        {
        //            case "CHEQUE_NO":
        //                fieldValue = ((Label)row.FindControl("lblCqNo")).Text;
        //                break;
        //            case "FOLIO_NO":
        //                fieldValue = ((Label)row.FindControl("lblFoloNo")).Text;
        //                break;
        //            case "APP_NO":
        //                fieldValue = ((Label)row.FindControl("lblAppoModify")).Text;
        //                break;
        //            case "PANNO":
        //                fieldValue = ((Label)row.FindControl("lblpanNo")).Text;
        //                break;
        //            case "BROKER_ID":
        //                fieldValue = ((Label)row.FindControl("lblbrokerCode")).Text;
        //                break;
        //            default:
        //                fieldValue = string.Empty;
        //                break;
        //        }

        //        // Set the retrieved value in the TextBox
        //        txtChequeSearch.Text = fieldValue;
        //    }
        //    else
        //    {
        //        txtChequeSearch.Text = "Please select a row first.";
        //    }
        //}

        
        protected void btnReconcile_Click(object sender, EventArgs e)
        {
            string rawCode = hfSelectedTranCode.Value;

            if (rawCode != null && rawCode != "")
            {

                
                decimal rawAmount = 0;
                decimal.TryParse(hfSelectedAmount.Value, out rawAmount);
                string tran1stcode = hftran1stcode.Value;

                string loginId = Session["LoginId"]?.ToString();
                string result = new MfManualReconciliationController().ReconcileTransactions(tran1stcode, rawAmount, rawCode, loginId);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{result}');", true);
               // btnReconcile.Focus();
            }
            // Show the result
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"Check any transaction for reconcilation first');", true);
           // btnReconcile.Focus();
        }

        protected void CmdSaveRemark_Click(object sender, EventArgs e)
        {
            // Store TRAN_CODE in ViewState
            //string myTrCode = tranCoderta;


            try
            {
                string remarkText = txtRemarks.Text;
                string tran1stcode = hftran1stcode.Value;

                // Check if transaction code is selected
                if (string.IsNullOrEmpty(tran1stcode))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert1", "alert('First select any transaction to be remarked.');", true);
                    return;
                }

                // Check if remark is entered
                if (string.IsNullOrWhiteSpace(remarkText))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert2", "alert('Please enter a remark before saving.');", true);
                    return;
                }

                // Update remark
                MfManualReconciliationController controller = new MfManualReconciliationController();
                controller.UpdateRemarkReco(tran1stcode, remarkText);

                // Show success message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert3", "alert('The record has been remarked successfully.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert4", $"alert('Error: {ex.Message}');", true);
            }

        }


        #region ConfirmPMS

        protected void cmdConfirm_Click(object sender, EventArgs e)
        {
            string tran1stcode = hftran1stcode.Value;
            
            string remarks = txtRemarks.Text.Trim();
            string user = Session["LoginId"]?.ToString(); // Assuming `Glbloginid` is stored in Session

            bool optPMS = rblTranType.SelectedValue == "pms";
            bool optATM = rblTranType.SelectedValue == "atm";

            if (!optATM && !optPMS)
            {
                ShowMessage("Select Either PMS or ATM Tran Type");
            }

            if (optPMS)
            {
                if (string.IsNullOrEmpty(tran1stcode))
                {
                    ShowMessage("First Select The Record You Want To Map");
                    return;
                }

            }

            if (optATM)
            {
                if (string.IsNullOrEmpty(tran1stcode))
                {
                    ShowMessage("First Select The Record You Want To Map");
                    return;
                }
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("PRA_PMS_TransactionReco", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.Add("p_tran_code", OracleDbType.Varchar2).Value = tran1stcode;
                        cmd.Parameters.Add("p_remarks", OracleDbType.Varchar2).Value = optPMS ? remarks : (object)DBNull.Value;
                        cmd.Parameters.Add("p_user", OracleDbType.Varchar2).Value = user;
                        cmd.Parameters.Add("p_opt_pms", OracleDbType.Boolean).Value = optPMS;
                        cmd.Parameters.Add("p_opt_atm", OracleDbType.Boolean).Value = optATM;

                        // Execute procedure
                        cmd.ExecuteNonQuery();
                    }
                }

                // Only show success message if execution completes without exceptions
                ShowMessage("Record is confirmed successfully");
            }
            catch (Exception ex)
            {
                // Show error message if an exception occurs
                ShowMessage("Error: " + ex.Message);
            }
        }

        #endregion


        

       

        #region UnconfirmPMS
        protected void cmdUnconfirm_Click(object sender, EventArgs e)
        {
            string tran1stcode = hftran1stcode.Value;
            
            string remarks = txtRemarks.Text.Trim();
            string user = Session["LoginId"]?.ToString(); // Assuming `Glbloginid` is stored in Session

            bool optPMS = rblTranType.SelectedValue == "pms";
            bool optATM = rblTranType.SelectedValue == "atm";

            if (!optATM && !optPMS)
            {
                ShowMessage("Select Either PMS or ATM Tran Type");
            }

            if (optPMS)
            {
                if (string.IsNullOrEmpty(tran1stcode))
                {
                    ShowMessage("First Select The Record You Want To Unmap");
                    return;
                }

            }

            if (optATM)
            {
                if (string.IsNullOrEmpty(tran1stcode))
                {
                    ShowMessage("First Select The Record You Want To Unmap");
                    return;
                }
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("PRA_PMS_TransactionUnReco", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.Add("p_tran_code", OracleDbType.Varchar2).Value = tran1stcode;
                        cmd.Parameters.Add("p_remarks", OracleDbType.Varchar2).Value = optPMS ? remarks : (object)DBNull.Value;
                        cmd.Parameters.Add("p_user", OracleDbType.Varchar2).Value = user;
                        cmd.Parameters.Add("p_opt_pms", OracleDbType.Boolean).Value = optPMS;
                        cmd.Parameters.Add("p_opt_atm", OracleDbType.Boolean).Value = optATM;

                        // Execute procedure
                        cmd.ExecuteNonQuery();
                    }
                }

                // Only show success message if execution completes without exceptions
                ShowMessage("Record is unconfirmed successfully");
            }
            catch (Exception ex)
            {
                // Show error message if an exception occurs
                ShowMessage("Error: " + ex.Message);
            }
        }



        #endregion

        private void ShowMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", $"alert('{message}');", true);
        }

    }
}