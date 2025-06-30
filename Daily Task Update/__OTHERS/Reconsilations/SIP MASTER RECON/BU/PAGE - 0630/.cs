using System;
using System.Data;
using System.Globalization;
using System.Runtime.Remoting.Channels;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;

namespace WM.Masters
{
    public partial class sip_master_reconciliation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                fillBranchList("");
                fillRegionList();
                 //fillZoneList("");
                fillRMList();
                FillChannelList();
                FillAMCList();

            }
            ToggleReconcileButton();

        }
        private void ToggleReconcileButton()
        {
            reconcileBtn.Enabled = GridRta.Rows.Count > 0;
        }

        #region btnReset_Click
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        #endregion

        #region btnReset2_Click
        protected void btnReset2_Click(object sender, EventArgs e)
        {
            tarnCode.Text = string.Empty;
            tranCodeGrid.DataSource = null;
            tranCodeGrid.DataBind();

            // Reset DropDownLists
            amcSelectrta.SelectedIndex = 0;
            if (chequeNoSelect.Items.FindByValue("001") != null)
            {
                chequeNoSelect.SelectedValue = "001"; // Set default value as per your logic
            }

            fillBranchList("2");

            // Clear TextBoxes
            dateFromRta.Text = string.Empty;
            dateToRta.Text = string.Empty;
            chequeNo.Text = string.Empty;
            txtInvestorName.Text = string.Empty;
            txtAmount.Text = string.Empty;
            remark.Text = string.Empty;

            // Reset RadioButtons
            RadioButton1.Checked = false;
            RadioButton2.Checked = false;

            // Reset RadioButtonList
            RadioButtonList1.ClearSelection();

            // Optional: Clear GridView if needed
            GridRta.DataSource = null;
            GridRta.DataBind();
        }
        #endregion

        #region ClearFields
        private void ClearFields()
        {
            //// Reset DropDownLists to default selection
            //branchSelect.SelectedIndex = 0; // "Please Select Branch"
            //regionSelect.SelectedIndex = 0; // "Select Region"
            //zoneSelect.SelectedIndex = 0;   // "Select Zone"
            //rmSelect.SelectedIndex = 0;     // "Please Select RM"
            //channelSelect.SelectedIndex = 0; // Assuming the first item is the default
            amcSelect.SelectedIndex = 0;    // Assuming the first item is the default

            fillBranchList("1");

            fillRegionList();
            fillRMList();
            FillChannelList();

            zoneSelect.Items.Clear();
        
            // Reset Text Fields
            arNumber.Text = string.Empty;
            TextBox3.Text = string.Empty;
            folioNo.Text = string.Empty;
            pan.Text = string.Empty;
            clientCode.Text = string.Empty;
            sipStartDate.Text = string.Empty;
            tarnCode.Text = string.Empty;
            lblRowCount.Text = string.Empty;
            dateFrom.Text = string.Empty; dateTo.Text = string.Empty ; 

            // Reset Checkboxes
            pms.Checked = false;
            cob.Checked = false;

            // Reset Radio Buttons and Radio Button Lists
            RadioButtonList2.ClearSelection();
            // RadioButtonList2.Items.Clear();
            c.Checked = false;
            k.Checked = false;
            cCob.Checked = false;
            kCob.Checked = false;
            rblReconciliationType.ClearSelection();
            //  rblReconciliationType.Items.Clear();
            //op1.Checked = false;
            //op2.Checked = false;
            Option1Radio.Checked = false;

            // Clear GridView Data
            tableSearchResults.DataSource = null; // Remove any data bound to the GridView
            tableSearchResults.DataBind();        // Refresh the GridView to reflect changes
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



        #region fillBranchList
        private void fillBranchList(string check = "")
        {
                sip_master_reconciliationController controller = new sip_master_reconciliationController();
                DataTable dt = controller.GetBranchList();

            if (check == "1")
            {
                branchSelect.DataSource = dt;
                branchSelect.DataTextField = "branch_name";
                branchSelect.DataValueField = "branch_code";
                branchSelect.DataBind();
                branchSelect.Items.Insert(0, new ListItem("Select Branch", ""));
            }
            else if(check == "2")
            {
                branchSelectrta.DataSource = dt;
                branchSelectrta.DataTextField = "branch_name";
                branchSelectrta.DataValueField = "branch_code";
                branchSelectrta.DataBind();

                branchSelectrta.Items.Insert(0, new ListItem("Select Branch", ""));
            }
            else
            {
                branchSelect.DataSource = dt;
                branchSelect.DataTextField = "branch_name";
                branchSelect.DataValueField = "branch_code";
                branchSelect.DataBind();
                branchSelect.Items.Insert(0, new ListItem("Select Branch", ""));

                branchSelectrta.DataSource = dt;
                branchSelectrta.DataTextField = "branch_name";
                branchSelectrta.DataValueField = "branch_code";
                branchSelectrta.DataBind();

                branchSelectrta.Items.Insert(0, new ListItem("Select Branch", ""));

            }
        }
        #endregion

        #region fillRegionList
        private void fillRegionList(string regionName = "")
        {
            // Get the region list from the controller
            DataTable dt = new RegionZoneController().GetRegions(regionName);

            if (dt != null && dt.Rows.Count > 0)
            {
                // Set the data source for the DropDownList
                regionSelect.DataSource = dt;
                regionSelect.DataTextField = "REGION_NAME"; // Adjust according to your data field
                regionSelect.DataValueField = "REGION_ID";  // Adjust according to your data field
                regionSelect.DataBind();
            }
            else
            {
                // If no regions are found, you may want to clear the DropDownList
                regionSelect.Items.Clear();
            }

            // Optionally, add a default item like "Select Region"
            regionSelect.Items.Insert(0, new ListItem("Select Region", ""));
        }
        #endregion

        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRegionId = regionSelect.SelectedValue;
            FillZoneList(selectedRegionId);
        }

        #region FillZoneList
        private void FillZoneList(string regionId)
        {
            // Assume branches are stored in a session or other variable
            //string branches = (string)Session["Branches"];

            // Get the zones list from the controller
            DataTable dt = new sip_master_reconciliationController().GetZonesByRegion(regionId);

            if (dt != null && dt.Rows.Count > 0)
            {
                // Clear existing items and set the data source for the DropDownList
                zoneSelect.DataSource = dt;
                zoneSelect.DataTextField = "ZONE_NAME";
                zoneSelect.DataValueField = "ZONE_ID";
                zoneSelect.DataBind();
            }
            else
            {
                // Clear the dropdown if no zones are found
                zoneSelect.Items.Clear();
            }

            zoneSelect.Items.Insert(0, new ListItem("", ""));
            zoneSelect.Items.Insert(1, new ListItem("All", ""));
        }
        #endregion

        protected void ddlzone_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedbranchbyzoneId = zoneSelect.SelectedValue;
            FillbranchbyzoneList(selectedbranchbyzoneId);
        }

        protected void FillbranchbyzoneList(string selectedbranchbyzoneId)
        {

            // Call the controller method to get branch details
            DataTable dt = new sip_master_reconciliationController().GetBranchbyzoneDetails(selectedbranchbyzoneId);

            branchSelect.DataSource = dt;
            branchSelect.DataTextField = "Branch_name";
            branchSelect.DataValueField = "Branch_code";
            branchSelect.DataBind();

            branchSelectrta.DataSource = dt;
            branchSelectrta.DataTextField = "Branch_name";
            branchSelectrta.DataValueField = "Branch_code";
            branchSelectrta.DataBind();

            branchSelect.Items.Insert(0, new ListItem("", ""));
            branchSelectrta.Items.Insert(0, new ListItem("", ""));

            branchSelect.Items.Insert(1, new ListItem("All", ""));
            branchSelectrta.Items.Insert(1, new ListItem("All", ""));

        }

        protected void ddlRmFill_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItemSerial = branchSelect.SelectedValue;

            // Call the controller method to get branch details
            // DataTable dt = new MfManualReconciliationController().GetBranchDetails(selectedItemSerial);
            DataTable dt = new WM.Controllers.MfManualReconciliationController().GetRM(selectedItemSerial);
            rmSelect.DataSource = dt;
            rmSelect.DataTextField = "RM_NAME";
            rmSelect.DataValueField = "payroll_id";
            rmSelect.DataBind();
            rmSelect.Items.Insert(0, new ListItem("", ""));
            rmSelect.Items.Insert(1, new ListItem("All", ""));
        }

        //#region fillZoneList
        //private void fillZoneList(string zoneName)
        //{
        //    // Get the list of zones from the controller
        //    DataTable dt = new RegionZoneController().GetZones(zoneName);

        //    if (dt != null)
        //    {
        //        // Bind the data to the DropDownList
        //        zoneSelect.DataSource = dt;
        //        zoneSelect.DataTextField = "ZONE_NAME"; // Adjust according to your data field
        //        zoneSelect.DataValueField = "ZONE_ID";  // Adjust according to your data field
        //        zoneSelect.DataBind();
        //    }

        //    // Optionally, add a default item like "Select Zone"
        //    zoneSelect.Items.Insert(0, new ListItem("Select Zone", ""));
        //}
        //#endregion

        #region fillRmList
        private void fillRMList()

        {
            DataTable dt = new WM.Controllers.EmployeeController().GetRM();
            //AddDefaultItemRM(dt);
            rmSelect.DataSource = dt;
            rmSelect.DataTextField = "RM_NAME";
            rmSelect.DataValueField = "RM_CODE";
            rmSelect.DataBind();

            rmSelect.Items.Insert(0, new ListItem("Select RM", ""));
        }

        #endregion



        #region FillChannelList
        private void FillChannelList()
        {
            // Get the list of Channels from the controller
            DataTable dt = new WM.Controllers.sip_master_reconciliationController().GetChannelList();


            // Bind the data to the DropDownList
            channelSelect.DataSource = dt;
            channelSelect.DataTextField = "itemname"; // Corresponding to itemname in the procedure
            channelSelect.DataValueField = "itemserialnumber";   // Corresponding to itemserialnumber in the procedure
            channelSelect.DataBind();
            channelSelect.Items.Insert(0, new ListItem("Select Channel", ""));
        }
        #endregion

        protected void ddlBranchCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItemSerial = channelSelect.SelectedValue;

            // Call the controller method to get branch details
            DataTable dt = new sip_master_reconciliationController().GetBranchDetails(selectedItemSerial);

            branchSelect.DataSource = dt;
            branchSelect.DataTextField = "Branch_name";
            branchSelect.DataValueField = "Branch_code";
            branchSelect.DataBind();

            branchSelectrta.DataSource = dt;
            branchSelectrta.DataTextField = "Branch_name";
            branchSelectrta.DataValueField = "Branch_code";
            branchSelectrta.DataBind();

            branchSelect.Items.Insert(0, new ListItem("", ""));
            branchSelectrta.Items.Insert(0, new ListItem("", ""));

            branchSelect.Items.Insert(1, new ListItem("All", ""));
            branchSelectrta.Items.Insert(1, new ListItem("All", ""));


            DataTable rdt = new sip_master_reconciliationController().Getregionbychannel(selectedItemSerial);

            regionSelect.DataSource = rdt;
            regionSelect.DataTextField = "REGION_NAME"; // Adjust according to your data field
            regionSelect.DataValueField = "REGION_ID";  // Adjust according to your data field
            regionSelect.DataBind();
          
            regionSelect.Items.Insert(0, new ListItem("", ""));
            regionSelect.Items.Insert(1, new ListItem("All", ""));

            DataTable zdt = new sip_master_reconciliationController().Getzonebychannel(selectedItemSerial);

            zoneSelect.DataSource = zdt;
            zoneSelect.DataTextField = "ZONE_NAME"; // Adjust according to your data field
            zoneSelect.DataValueField = "ZONE_ID";  // Adjust according to your data field
            zoneSelect.DataBind();
          
            zoneSelect.Items.Insert(0, new ListItem("", ""));
            zoneSelect.Items.Insert(1, new ListItem("All", ""));


        }


        #region FillAMCList
        private void FillAMCList()
        {
            // Get the list of AMCs from the controller
            DataTable dt = new WM.Controllers.sip_master_reconciliationController().GetAMCList(); // Ensure you have the correct controller reference


            // Bind the data to the DropDownList
            amcSelect.DataSource = dt;
            amcSelect.DataTextField = "MUT_NAME"; // Corresponding to MUT_NAME in the procedure
            amcSelect.DataValueField = "MUT_CODE";   // Corresponding to MUT_CODE in the procedure
            amcSelect.DataBind();
            amcSelectrta.DataSource = dt;
            amcSelectrta.DataTextField = "MUT_NAME"; // Corresponding to MUT_NAME in the procedure
            amcSelectrta.DataValueField = "MUT_CODE";   // Corresponding to MUT_CODE in the procedure
            amcSelectrta.DataBind();

            amcSelect.Items.Insert(0, new ListItem("Select Amc", ""));
            amcSelectrta.Items.Insert(0, new ListItem("Select Amc", ""));
        }
        #endregion

        private string tranCodetra
        {
            get { return ViewState["tranCodetra"] as string ?? string.Empty; }
            set { ViewState["tranCodetra"] = value; }
        }

        private int SelectedRowIndex
        {
            get
            {
                return ViewState["SelectedRowIndex"] != null ? (int)ViewState["SelectedRowIndex"] : -1;
            }
            set
            {
                ViewState["SelectedRowIndex"] = value;
            }
        }

        protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                // Find the row where the checkbox is checked
                CheckBox chk = (CheckBox)sender;
                GridViewRow row = (GridViewRow)chk.NamingContainer;

                int rowIndex = row.RowIndex;

                // If your grid has a header, adjust the index to skip it
                if (tableSearchResults.HeaderRow != null && rowIndex >= 0)
                {
                    hfSelectedRow.Value = rowIndex.ToString();
                }
                SelectedRowIndex = row.RowIndex;
             
                tableSearchResults.Focus();

                // Get values from the selected row
                HiddenField hfTranCode = (HiddenField)row.FindControl("hfTranCode");
                string tranCodetrap = hfTranCode.Value;
                tranCodetra = tranCodetrap.ToString();

                string tranDateStr = ((Label)row.FindControl("lblTranDate")).Text;
                string tranType = ((Label)row.FindControl("lblTrnType")).Text;
                string investorName = ((Label)row.FindControl("lblInvestorName")).Text;
                string amcCode = ((Label)row.FindControl("lblAMC")).Text;
                string branch = ((Label)row.FindControl("lblbranchco")).Text;
                string amount = ((Label)row.FindControl("lblAmount")).Text;
                string chqno = ((Label)row.FindControl("lblChqNo")).Text;
                string registrar = ((Label)row.FindControl("lblregis")).Text;
                string cobfl = ((Label)row.FindControl("lblCOBFL")).Text;

                if (registrar == "C" && cobfl == "0")
                {
                    c.Checked = true;  // Select "C"
                }
                else if (registrar == "K" && cobfl == "0")
                {
                    k.Checked = true;  // Select "K"
                }
                else if (registrar == "C" && cobfl == "1")
                {
                    cCob.Checked = true;  // Select "C COB"
                }
                else if (registrar == "K" && cobfl == "1")
                {
                    kCob.Checked = true;  // Select "K COB"
                }


                if (DateTime.TryParseExact(tranDateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tranDate))
                {
                    // Calculate the "From" and "To" dates, safely adjusting the day if needed.
                    DateTime fromDate = tranDate.AddMonths(-1); // One month before
                    DateTime toDate = tranDate.AddMonths(1);    // One month after

                    // Ensure the day remains valid if the target month has fewer days.
                    fromDate = new DateTime(fromDate.Year, fromDate.Month, Math.Min(tranDate.Day, DateTime.DaysInMonth(fromDate.Year, fromDate.Month)));
                    toDate = new DateTime(toDate.Year, toDate.Month, Math.Min(tranDate.Day, DateTime.DaysInMonth(toDate.Year, toDate.Month)));

                    // Assign the formatted dates to the textboxes.
                    dateFromRta.Text = fromDate.ToString("dd/MM/yyyy");
                    dateToRta.Text = toDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    // Handle invalid date parsing.
                    dateFromRta.Text = string.Empty;
                    dateToRta.Text = string.Empty;
                }


                rblReconciliationType.SelectedValue = tranType.ToLower() == "reconciled" ? "reconciled-rta" : "unreconciled-rta";
                amcSelectrta.SelectedValue = amcCode;
              //  branchSelectrta.SelectedValue = branch;
                txtAmount.Text = amount;
                txtInvestorName.Text = investorName;
                chequeNoSelect.SelectedValue = "001";
                chequeNo.Text = chqno;

            }

            catch (Exception ex)
            {
                // Handle any unexpected errors
                // Optionally, log the error or display a message to the user
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        protected void ddlChequeNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if a row has been selected
            if (SelectedRowIndex >= 0 && SelectedRowIndex < tableSearchResults.Rows.Count)
            {
                // Get the selected GridViewRow
                GridViewRow row = tableSearchResults.Rows[SelectedRowIndex];

                // Get the selected field from the DropDownList
                string selectedField = chequeNoSelect.SelectedItem.ToString();

                // Retrieve the value for the selected field
                string fieldValue = string.Empty;
                switch (selectedField)
                {
                    case "CHEQUE_NO":
                        fieldValue = ((Label)row.FindControl("lblChqNo")).Text;
                        break;
                    case "FOLIO_NO":
                        fieldValue = ((Label)row.FindControl("lblFolioNo")).Text;
                        break;
                    case "APP_NO":
                        fieldValue = ((Label)row.FindControl("lblAppNoModify")).Text;
                        break;
                    case "PANNO":
                        fieldValue = ((Label)row.FindControl("lblpanNo")).Text;
                        break;
                    case "BROKER_ID":
                        fieldValue = ((Label)row.FindControl("lblbrokerCode")).Text;
                        break;
                    default:
                        fieldValue = string.Empty;
                        break;
                }

                // Set the retrieved value in the TextBox
                chequeNo.Text = fieldValue;
            }
            else
            {
                chequeNo.Text = "Please select a row first.";
            }
        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SipMasterModel searchModel = new SipMasterModel
            {
                Channelid = decimal.TryParse(channelSelect.SelectedValue, out decimal result) ? result : (decimal?)null,
                ARNumber = arNumber.Text,
                Branch = branchSelect.SelectedValue,
                Region = regionSelect.SelectedValue,
                RM = rmSelect.SelectedValue,
                Zone = zoneSelect.SelectedValue,
                TranType = RadioButtonList2.SelectedValue,
                ReconciliationStatus = rblReconciliationType.SelectedValue,
                COB = cob.Checked ? "1" : null,
                PMS = pms.Checked ? "1" : null,
                DateFrom = string.IsNullOrEmpty(dateFrom.Text) ? (System.DateTime?)null :
            DateTime.TryParseExact(dateFrom.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateFrom)
            ? parsedDateFrom // Store as DateTime object
            : (System.DateTime?)null,

                DateTo = string.IsNullOrEmpty(dateTo.Text) ? (System.DateTime?)null :
            DateTime.TryParseExact(dateTo.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTo)
            ? parsedDateTo // Store as DateTime object
            : (System.DateTime?)null,
                AMC = amcSelect.SelectedValue
            };
            if (string.IsNullOrWhiteSpace(searchModel.ARNumber) &&
       string.IsNullOrWhiteSpace(searchModel.Branch) &&
       string.IsNullOrWhiteSpace(searchModel.Region) &&
       string.IsNullOrWhiteSpace(searchModel.RM) &&
       string.IsNullOrWhiteSpace(searchModel.Zone) &&
       string.IsNullOrWhiteSpace(searchModel.TranType) &&
       searchModel.DateFrom == null &&
       searchModel.DateTo == null &&
       string.IsNullOrWhiteSpace(searchModel.AMC))
            {
                // Alert user if no criteria are provided
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Please provide at least one search criterion.');", true);
                return;
            }

            DataTable dt = new WM.Controllers.sip_master_reconciliationController().GetTransactions(searchModel);
            tableSearchResults.Visible = true;
            tableSearchResults.DataSource = dt;
            tableSearchResults.DataBind();
            tableSearchResults.Focus();

            lblRowCount.Text = dt.Rows.Count.ToString();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Clear any previous content and headers
            Response.Clear();
            Response.Buffer = true;

            // Set the response type to Excel
            Response.AddHeader("content-disposition", "attachment;filename=SipMasterData.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            // Hide the CheckBox column during export (optional, if needed)
            GridRta.Columns[0].Visible = false; // Adjust column index based on CheckBox location

            // Use a StringWriter and HtmlTextWriter to write the GridView data
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // Remove paging to export all data
                    GridRta.AllowPaging = false;

                    // Optionally rebind data if needed
                    // BindGridData();

                    // Render the GridTransaction content
                    GridRta.RenderControl(hw);

                    // Output the rendered HTML as Excel-compatible content
                    string excelData = sw.ToString();
                    Response.Output.Write(excelData);
                    Response.Flush();
                    Response.End();
                }
            }

            // Re-show the CheckBox column after export (optional)
            GridRta.Columns[0].Visible = true;

            // Optionally re-enable paging if it was enabled before
            GridRta.AllowPaging = true;
            // Rebind data if needed to show paginated data again
            // BindGridData();
        }

        protected void btnExport1_Click(object sender, EventArgs e)
        {
            // Clear any previous content and headers
            Response.Clear();
            Response.Buffer = true;

            // Set the response type to Excel
            Response.AddHeader("content-disposition", "attachment;filename=WealthmakerSIPData.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            // Hide the CheckBox column during export
            tableSearchResults.Columns[0].Visible = false;  // Assuming the CheckBox is in the first column (index 0)

            // Use a StringWriter and HtmlTextWriter to write the GridView data
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    // Remove paging to export all data
                    tableSearchResults.AllowPaging = false;

                    // Optionally rebind data if needed
                    // BindGridData();

                    // Render the GridView content
                    tableSearchResults.RenderControl(hw);

                    // Output the rendered HTML as Excel-compatible content
                    string excelData = sw.ToString();
                    Response.Output.Write(excelData);
                    Response.Flush();
                    Response.End();
                }
            }

            // Re-show the CheckBox column after export
            tableSearchResults.Columns[0].Visible = true;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for rendering GridView in an Excel-friendly format
        }

        protected void btnSearchrta_Click(object sender, EventArgs e)
        {
            string reconciliationStatus = RadioButton1.Checked ? "Y" : (RadioButton2.Checked ? "N" : null);

            SipMasterModel searchModel = new SipMasterModel
            {
                Branch = branchSelectrta.SelectedValue,
                AMC = amcSelectrta.SelectedValue,
                ReconciliationStatus = reconciliationStatus,

                DateFrom = string.IsNullOrEmpty(dateFromRta.Text) ? (System.DateTime?)null : System.DateTime.ParseExact(dateFromRta.Text, "dd/MM/yyyy", null),
                DateTo = string.IsNullOrEmpty(dateToRta.Text) ? (System.DateTime?)null : System.DateTime.ParseExact(dateToRta.Text, "dd/MM/yyyy", null),
                TranType = RadioButtonList1.SelectedValue,

                ChequeNo = chequeNoSelect.SelectedValue,
                InvestorName = txtInvestorName.Text,
                Amount = string.IsNullOrEmpty(txtAmount.Text) ? (decimal?)null : decimal.Parse(txtAmount.Text),
                docNo = chequeNo.Text,
            };

            // Validate that at least one search criterion is provided
            if (string.IsNullOrWhiteSpace(searchModel.Branch) &&
                string.IsNullOrWhiteSpace(searchModel.AMC) &&
                searchModel.DateFrom == null &&
                searchModel.DateTo == null &&
                string.IsNullOrWhiteSpace(searchModel.ChequeNo) &&
                string.IsNullOrWhiteSpace(searchModel.InvestorName) &&
                searchModel.Amount == null &&
                string.IsNullOrWhiteSpace(searchModel.docNo))
            {
                // Display an alert if no criteria are provided
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "alert('Please provide at least one search criterion.');", true);
                return;
            }

            DataTable transactions = new WM.Controllers.sip_master_reconciliationController().GetTransactionsRta(searchModel);

            GridRta.Visible = true;
            GridRta.DataSource = transactions;
            GridRta.DataBind();
            GridRta.Focus();
        }

        protected void btnSearchsip_Click(object sender, EventArgs e)
        {
            SipMasterModel searchModel = new SipMasterModel
            {
                FolioNo = string.IsNullOrEmpty(folioNo.Text) ? (string)null : folioNo.Text,
                Amount = string.IsNullOrEmpty(TextBox3.Text) ? (decimal?)null : decimal.Parse(TextBox3.Text),
                PAN = string.IsNullOrEmpty(pan.Text) ? (string)null : pan.Text,
                ClientCode = string.IsNullOrEmpty(clientCode.Text) ? (string)null : clientCode.Text,
                SIPStartDate = string.IsNullOrEmpty(sipStartDate.Text) ? (System.DateTime?)null : System.DateTime.ParseExact(sipStartDate.Text, "dd/MM/yyyy", null)
            };

            DataTable transactions = new WM.Controllers.sip_master_reconciliationController().GetTransactionsSIP(searchModel);
            GridSIPTransactions.Visible = true;
            GridSIPTransactions.DataSource = transactions;
            GridSIPTransactions.DataBind();
        }

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

        private decimal amrtaparsed
        {
            get { return (decimal)(ViewState["amrtaparsed"] ?? 0); }
            set { ViewState["amrtaparsed"] = value; }
        }

        private string tranCoderta
        {
            get { return ViewState["tranCoderta"] as string ?? string.Empty; }
            set { ViewState["tranCoderta"] = value; }
        }

        protected void chkSelectrta_CheckedChanged(object sender, EventArgs e)
        {
            // Find the row where the checkbox is checked
            CheckBox chk = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chk.NamingContainer;
            GridRta.Focus();

            // Get values from the selected row
            HiddenField hfTranCoderta = (HiddenField)row.FindControl("hfTranCoderta");
            string tranCodertap = hfTranCoderta.Value;
            tranCoderta = tranCodertap.ToString();

            string amrta = ((Label)row.FindControl("lblmount")).Text;
            amrtaparsed = decimal.Parse(((Label)row.FindControl("lblmount")).Text);

        }

        protected void btnsiprerta_Click(object sender, EventArgs e)
        {
            string loginId = Session["LoginId"]?.ToString();

            string result = new sip_master_reconciliationController().ReconcileTransactions(tranCodetra, amrtaparsed, tranCoderta , loginId);

            // Show the result
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{result}');", true);
            reconcileBtn.Focus();
        }

        protected void CmdSaveRemark_Click(object sender, EventArgs e)
        {
            // Store TRAN_CODE in ViewState
            //string myTrCode = tranCoderta;


            try
            {
                string remarkText = remark.Text;

                if (remarkText.Length > 0)
                {
                    sip_master_reconciliationController controller = new sip_master_reconciliationController();
                    controller.UpdateRemarkReco(tranCodetra, remarkText);
                    // txtRemarks.Focus();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this ,this.GetType(), "alert", "alert('First Give Any Remark Before Save It.');", true);
                    remark.Focus();
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('The record has been remarked successfully.');", true);
                remark.Focus();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                remark.Focus();
            }
        }
    }
}



