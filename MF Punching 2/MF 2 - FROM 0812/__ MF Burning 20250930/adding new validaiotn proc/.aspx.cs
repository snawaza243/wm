using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;
using System.Globalization;
using System.Linq;
using Microsoft.Ajax.Utilities;
using Oracle.ManagedDataAccess.Client;
using System.Web.Services.Description;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web.Services;
using WM.Transaction;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Optimization;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using NPOI.SS.Formula.Functions;
using System.Web.UI.HtmlControls;
using System.Web;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Oracle.ManagedDataAccess.Types;


namespace WM.Masters
{
    public partial class Mf_Punching : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                // lgin and roel id session
                //Session["LoginId"] = "121397";
                //Session["roleId"] = "212";

                RedirectToWelcomePage();

                FrmTransactionLog.Visible = false;

                lblMessage.Text = string.Empty;

                txtSIPStartDate.Text = DateTime.Now.ToString("dd/MM/yyyy").Replace('-', '/');
                txtSIPStartDateupdate.Text = DateTime.Now.ToString("dd/MM/yyyy").Replace('-','/');

                if (tableSearchResults.Visible && tableSearchResults.Rows.Count > 0)
                {
                    // Register the focus script for the GridView after it has been rendered
                    ScriptManager.RegisterStartupScript(this, GetType(), "focusGrid",
                        $"setTimeout(function() {{ document.getElementById('{tableSearchResults.ClientID}').focus(); }}, 100);", true);
                }

                if (!IsPostBack)
                {

                    txtInstallmentsNos.Enabled = false;
                    ddlFrequency.Enabled = false;
                    rdo99Years.Enabled = false;
                    BindBranchDataToDropdown();
                    BindCityDataToDropdown();
                    FillAMCList();
                    BindBranchDropdown();
                    fillBankList();
                    fillCityList();
                    fillStateList();
                    FillReasonDropdown();
                    UpdateStateDropdownmodal();
                    mdatecancel.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    cheque.Checked = true;
                    cheque_view.Checked = true;
                    if (ddlSipStp != null && hdnSipStpValue != null)
                    {
                        hdnSipStpValue.Value = ddlSipStp.SelectedValue;
                    }


                }

                else
                {
                    if (hdnPopupVisible1.Value.Contains("true"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showPopup3();", true);
                    }

                    if (hdnPopupVisible2.Value.Contains("true"))
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showPopup", "showPopup4();", true);
                    }
                }


                if (ddlSipStp != null && hdnSipStpValue != null)
                {
                    hdnSipStpValue.Value = ddlSipStp.SelectedValue;
                }

                // Redirected from DMS System :-

                bool hasArno = !string.IsNullOrEmpty(Request.QueryString["arno"]);
                bool hasAudit = !string.IsNullOrEmpty(Request.QueryString["Audit"]);
                string arno = Request.QueryString["arno"];

                if (hasArno && arno != null)
                {
                    UpdatePanelSearchAR.Visible = false;
                    additionalInfo.Visible = false;
                    UpdatePanelgrid.Visible = false;
                    UpdatePanelFillByDT.Visible = false;
                    trNo.Text = arno;

                    // Simulate Search Button Click
                    btnSearch_Click(btnSearchAR, EventArgs.Empty);

                    // Wait for grid to bind & then auto-click first row (optional)
                    if (tableSearchResults.Rows.Count > 0)
                    {
                        // Find the "Select" button in the first row
                        GridViewRow row = tableSearchResults.Rows[0];
                        Button selectBtn = row.FindControl("Button1") as Button;

                        if (selectBtn != null)
                        {
                            // Panel3.Visible = true;
                            string commandArg = selectBtn.CommandArgument;
                            tableSearchResultsTran_RowCommand(selectBtn, new GridViewCommandEventArgs(selectBtn, new CommandEventArgs("SelectRow", commandArg)));

                        }
                    }
                }
            }
        }

        public void RedirectToWelcomePage(string loginId = null, string roleId = null)
        {
            // Log the current session values for debugging
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            Console.WriteLine($"Current Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

            // Update session values only if the parameters are not null
            if (loginId != null)
            {
                HttpContext.Current.Session["LoginId"] = loginId;
            }
            if (roleId != null)
            {
                HttpContext.Current.Session["RoleId"] = roleId;
            }

            // Log the updated session values for debugging
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            Console.WriteLine($"Updated Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                HttpContext.Current.Response.Redirect("https://wealthmaker.in/login_new.aspx");
            }

        }


        #region BindBranchDropdown
        private void BindBranchDropdown()
        {
            MfPunchingController controller = new MfPunchingController();
            DataTable dt = controller.GetBranchList();


            branch.DataSource = dt;
            branch.DataTextField = "branch_name";
            branch.DataValueField = "branch_code"; // Set the value to branch_code
            branch.DataBind();


            // Add a default item for selection
            branch.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select Branch--", ""));
        }
        #endregion

        #region FillReasonDropdown
        private void FillReasonDropdown()
        {
            DataTable dt = new MfPunchingController().GetDistinctReasons();
            CmbReson.DataSource = dt;
            CmbReson.DataTextField = "REASON";
            CmbReson.DataValueField = "REASON"; // Or use a different field for value if needed
            CmbReson.DataBind();
            CmbReson.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- Select Reason --", ""));
        }
        #endregion

        #region FillAMCList
        private void FillAMCList()
        {
            // Get the list of AMCs from the controller
            DataTable dt = new WM.Controllers.MfPunchingController().GetAMCList(); // Ensure you have the correct controller reference

            // Bind the data to the DropDownList
            amcSelect.DataSource = dt;
            amcSelect.DataTextField = "MUT_NAME"; // Corresponding to MUT_NAME in the procedure
            amcSelect.DataValueField = "MUT_CODE";   // Corresponding to MUT_CODE in the procedure
            amcSelect.DataBind();
            amcSelect.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));

            amc.DataSource = dt;
            amc.DataTextField = "MUT_NAME"; // Corresponding to MUT_NAME in the procedure
            amc.DataValueField = "MUT_CODE";   // Corresponding to MUT_CODE in the procedure
            amc.DataBind();
            amc.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));
        }
        #endregion

        #region fillbank
        private void fillBankList()
        {
            DataTable dt = new DataTable();

            dt = new WM.Controllers.MfPunchingController().Getbankdropdown();
            bankName.DataSource = dt;
            bankName.DataTextField = "Bank_name";
            bankName.DataValueField = "Bank_Id";
            bankName.DataBind();
            bankName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));

            ddlBankName.DataSource = dt;
            ddlBankName.DataTextField = "Bank_name";
            ddlBankName.DataValueField = "Bank_Id";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));
        }
        #endregion

        #region fillCity
        private void fillCityList()
        {
            DataTable dt = new WM.Controllers.MfPunchingController().GetCityDropdown();

            ddlCity.DataSource = dt; // Assume 'cityDropdown' is your dropdown control ID
            ddlCity.DataTextField = "CITY_NAME";
            ddlCity.DataValueField = "CITY_ID";
            ddlCity.DataBind();
            ddlCity.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));

            if (dt != null && DropDownCity != null)
            {
                DropDownCity.DataSource = dt;
                DropDownCity.DataTextField = "CITY_NAME";
                DropDownCity.DataValueField = "CITY_ID";
                DropDownCity.DataBind();
                DropDownCity.Items.Insert(0, new ListItem("--Select city--", ""));

                //string selectedCity = DropDownCity.SelectedItem.Text;
                //UpdateStateDropdownmodal(selectedCity);
            }
            ddlCityADD.DataSource = dt;
            ddlCityADD.DataTextField = "CITY_NAME";
            ddlCityADD.DataValueField = "CITY_ID";
            ddlCityADD.DataBind();
            ddlCityADD.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));

            ddlCityADD3.DataSource = dt;
            ddlCityADD3.DataTextField = "CITY_NAME";
            ddlCityADD3.DataValueField = "CITY_ID";
            ddlCityADD3.DataBind();
            ddlCityADD3.Items.Insert(0, new System.Web.UI.WebControls.ListItem("", ""));
        }
        private void fillStateList()
        {
            DataTable dt = new MfPunchingController().GetStateList();

            ddlStateADD.DataSource = dt; // Assume 'cityDropdown' is your dropdown control ID
            ddlStateADD.DataTextField = "State_NAME";
            ddlStateADD.DataValueField = "State_ID";
            ddlStateADD.DataBind();
            ddlStateADD.Items.Insert(0, new ListItem("Select State", ""));
        }
        #endregion

        //#region statedropdownlist
        //protected void dropdownCity_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string selectedCity = DropDownCity.SelectedItem.Text;
        //    UpdateStateDropdownmodal(selectedCity);
        //}
        //#endregion

        #region
        protected void UpdateStateDropdownmodal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup4", "$('#addresModal').modal('show');", true);

            //  string selectedCity = ddlCityADD.SelectedItem.ToString();
            MfPunchingController controller = new MfPunchingController();
            DataTable dt = controller.GetStateList();

            //ddlStateADD.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                DropDownState.DataSource = dt;
                DropDownState.DataTextField = "STATE_NAME";
                DropDownState.DataValueField = "state_id";
                DropDownState.DataBind();
            }
            else
            {
                // Clear the dropdown if no zones are found
                DropDownState.Items.Clear();
            }
        }
        #endregion

        #region btnSearch_Click
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //  UpdatePanel2.Update();

            // Check if at least one search criteria is filled
            if (string.IsNullOrWhiteSpace(fromDate.Text) &&
                string.IsNullOrWhiteSpace(toDate.Text) &&
                string.IsNullOrWhiteSpace(TextBox15.Text) &&
                string.IsNullOrWhiteSpace(trNo.Text) &&
                string.IsNullOrWhiteSpace(uniqueClientCode.Text) &&
                string.IsNullOrWhiteSpace(chequeNo.Text) &&
                string.IsNullOrWhiteSpace(anaCode.Text) &&
                string.IsNullOrWhiteSpace(appNo.Text))
            {
                // Show normal alert
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('Please choose at least one criteria to search.');", true);
                return; // Exit the method if no criteria are selected
            }

            // Create an instance of the TransactionFilter model with data from the UI controls
            TransactionFilter filter = new TransactionFilter
            {
                FromDate = string.IsNullOrWhiteSpace(fromDate.Text)
    ? (DateTime?)null
    : DateTime.ParseExact(fromDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),

                ToDate = string.IsNullOrWhiteSpace(toDate.Text)
    ? (DateTime?)null
    : DateTime.ParseExact(toDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                OrderBy = orderBy.SelectedValue == "" ? null : orderBy.SelectedValue,
                OrderDirection = ascending.Checked ? "ASC" : descending.Checked ? "DESC" : null,
                PANNo = string.IsNullOrWhiteSpace(TextBox15.Text) ? null : TextBox15.Text.Trim(),
                TRNo = string.IsNullOrWhiteSpace(trNo.Text) ? null : trNo.Text.Trim(),
                UniqueClientCode = string.IsNullOrWhiteSpace(uniqueClientCode.Text) ? null : uniqueClientCode.Text.Trim(),
                ChequeNo = string.IsNullOrWhiteSpace(chequeNo.Text) ? null : chequeNo.Text.Trim(),
                anaExistCode = string.IsNullOrWhiteSpace(anaCode.Text) ? null : anaCode.Text.Trim(),
                AppNo = string.IsNullOrWhiteSpace(appNo.Text) ? null : appNo.Text.Trim()
            };

            // Get the filtered transaction list
            DataTable dt = new MfPunchingController().GetTransactionList(filter);

            if (dt.Rows.Count > 0)
            {
                // If data exists, show the GridView and bind the data
                tableSearchResults.Visible = true;
                tableSearchResults.DataSource = dt;
                tableSearchResults.DataBind();
                Label20.Text = ""; // Clear any previous message

                //gridview1.Visible = true;
                //gridview1.DataSource = dt;
                //gridview1.DataBind();

                //gridview1.Focus();
            }
            else
            {
                // If no data exists, hide the GridView and display a message
                tableSearchResults.Visible = false;
                Label20.Text = "No Data Available";
            }

            UpdatePanelgrid.Update();

            ScriptManager.RegisterStartupScript(this, GetType(), "focusGrid",
        $"setTimeout(function() {{ document.getElementById('{tableSearchResults.ClientID}').focus(); }}, 100);", true);


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


        private string baseTranCode
        {
            get { return ViewState["baseTranCode"] as string ?? string.Empty; }
            set { ViewState["baseTranCode"] = value; }
        }

        private string iNVCODEMODIFY
        {
            get { return ViewState["iNVCODEMODIFY"] as string ?? string.Empty; }
            set { ViewState["iNVCODEMODIFY"] = value; }
        }


        protected void tableSearchResultsTran_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            #region Authorization Check and Popup Display
            bool isAuthorized = pc.AuthorizedRolesMF.Contains(pc.currentRoleID());

            if (!isAuthorized)
            {

                string script = @"
        hidePopup4();
        alert('You are not authorized to access this functionality.');
        console.warn('Unauthorized access attempt with role: ' + '" + pc.currentRoleID() + @"');
    ";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "BlockUnauthorizedAccess", script, true);


            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAutoSwitchPanel", "showAutoSwitchPanel();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAuthorizedPopup", "showPopup4();", true);
                hdnPopupVisible2.Value = "true"; // Set the hidden field to indicate the popup is visible
            }
            #endregion

            txtCanTrcode.Text = trNo.Text.ToString();

            if (e.CommandName == "SelectRow")
            {

                Button btnSelect = (Button)e.CommandSource;
                GridViewRow row = (GridViewRow)btnSelect.NamingContainer;

                MfPunchingController mfc1 = new MfPunchingController();
                string currentVieAtLogStatus = mfc1.GET_MF_VIEW_AR_LOGGED_STATUS(null, ((Label)row.FindControl("lblARNO")).Text, null);
                lblMfViewLogStatus.Text = currentVieAtLogStatus;
                //INVESTOR POP-UP FIELD'S FILLING 
                txtAddressADD3.Text = ((Label)row.FindControl("lblAddress1")).Text;
                txtAddressADD23.Text = ((Label)row.FindControl("lblAddress2")).Text;
                txtPinADD3.Text = ((Label)row.FindControl("lblPIN")).Text;
                txtMobileADD3.Text = ((Label)row.FindControl("lblMobile")).Text;
                txtPanADD3.Text = ((Label)row.FindControl("lblPAN")).Text;
                txtEmailADD3.Text = ((Label)row.FindControl("lblEmail")).Text;
                txtAadharADD3.Text = ((Label)row.FindControl("lblAdhno")).Text;
                // Find the Label control in the row and get its text
                Label lblDOB = (Label)row.FindControl("lblDOB");

                if (lblDOB != null && DateTime.TryParseExact(lblDOB.Text, "dd/MM/yyyy",
                                                             CultureInfo.InvariantCulture,
                                                             DateTimeStyles.None,
                                                             out DateTime joiningDate))
                {
                    // If parsing succeeds, discard the time part and set the text in dd/MM/yyyy format
                    txtDOBADD3.Text = joiningDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    // If parsing fails or the label is not found, set the text to empty
                    txtDOBADD3.Text = string.Empty;
                }


                Label lblCity = (Label)row.FindControl("lblCity");
                if (lblCity != null)
                {
                    ListItem cityItem = ddlCityADD3.Items.FindByText(lblCity.Text);
                    if (cityItem != null)
                    {
                        ddlCityADD3.SelectedIndex = ddlCityADD3.Items.IndexOf(cityItem);
                        UpdateStateDropdownview(cityItem.Text); // Call the UpdateStateDropdown method if needed
                    }
                }

                // Set the form field values
                accountHolderV.Text = ((Label)row.FindControl("lblInvestorName")).Text;
                businessCodeV.Text = ((Label)row.FindControl("lblbusicodem")).Text;
                panNo.Text = ((Label)row.FindControl("lblPANTEMP")).Text;
                RadioButton9.Checked = ((Label)row.FindControl("lblSIPSTPview")).Text.Trim().ToUpper() == "REGULAR";
                SchemeName.Text = ((Label)row.FindControl("lblSchemeName")).Text + ";;" + ((Label)row.FindControl("lblSchemeCode")).Text;
                TextBox13.Text = ((Label)row.FindControl("lblAppNoModify")).Text;
                TextBox11.Text = ((Label)row.FindControl("lblTranDate")).Text;
                TextBox3.Text = ((Label)row.FindControl("lblChqDt")).Text;
                BranchNameView.Text = ((Label)row.FindControl("lblBranchName")).Text;

                string freq = ((Label)row.FindControl("lblFREQUENCY")).Text;
                if (!string.IsNullOrEmpty(freq))
                {

                    frequency.SelectedValue = freq;
                }
                installmentsNos.Text = ((Label)row.FindControl("lblinstallments_no")).Text;

                txtSIPStartDateupdate.Text = ((Label)row.FindControl("lblSIP_Start_Date")).Text;
                txtSIPEndDateupdate.Text = ((Label)row.FindControl("lblSIP_End_Date")).Text;

                HiddenField baseTranCode1 = (HiddenField)row.FindControl("hfBaseTranCode");
                string tranCodetrap = baseTranCode1.Value;
                baseTranCode = tranCodetrap.ToString();

                HiddenField INVCODEMOD = (HiddenField)row.FindControl("hfINVCODEMOD");
                string INVCODEM = INVCODEMOD.Value;
                iNVCODEMODIFY = INVCODEM.ToString();

                remarks.Text = ((Label)row.FindControl("lblRemark")).Text;
                TextBox12.Text = ((Label)row.FindControl("lblFolioNo")).Text;
                switchSchemeName.Text = ((Label)row.FindControl("lblSwitchSchemeName")).Text + ";;" + ((Label)row.FindControl("lblSwitchScheme")).Text;
                TextBox14.Text = ((Label)row.FindControl("lblAmount")).Text;
                TextBox2.Text = ((Label)row.FindControl("lblChqNo")).Text;
                holderCodeV.Text = ((Label)row.FindControl("lblHOLDERCODE")).Text;
                trNo.Text = ((Label)row.FindControl("lblARNO")).Text;
                txtClientCode.Text = ((Label)row.FindControl("lblCLIENT")).Text;

                // Set the selected values in dropdowns if they exist
                if (amcSelect.Items.FindByValue(((Label)row.FindControl("lblAMCview")).Text) != null)
                {
                    amcSelect.SelectedValue = ((Label)row.FindControl("lblAMCview")).Text;
                }
                if (ddlCity.Items.FindByValue(((Label)row.FindControl("lblCity")).Text) != null)
                {
                    ddlCity.SelectedValue = ((Label)row.FindControl("lblCity")).Text;
                }
                if (DropDownList4.Items.FindByValue(((Label)row.FindControl("lblTranType")).Text) != null)
                {
                    string curTranType = ((Label)row.FindControl("lblTranType")).Text;
                    DropDownList4.SelectedValue = curTranType;
                }
                string bankNameText = ((Label)row.FindControl("lblBankName")).Text;

                ListItem selectedItem = bankName.Items.FindByText(bankNameText);
                if (selectedItem != null)
                {
                    bankName.SelectedValue = selectedItem.Value; // Set the selected value of the dropdown
                }
                if (sipStp.Items.FindByValue(((Label)row.FindControl("lblSIPSTPview")).Text) != null)
                {
                    sipStp.SelectedValue = ((Label)row.FindControl("lblSIPSTPview")).Text;
                }
                //          dropDat.Text = DateTime.TryParseExact(((Label)row.FindControl("lblDOB")).Text, "dd/MM/yyyy",
                //System.Globalization.CultureInfo.InvariantCulture,
                //System.Globalization.DateTimeStyles.None,
                //out DateTime dob1)
                //? dob1.ToString("dd/MM/yyyy")
                //: string.Empty;

                // Additional fields
                txtAddress.Text = ((Label)row.FindControl("lblAddress1")).Text;
                txtAddress2.Text = ((Label)row.FindControl("lblAddress2")).Text;
                txtPin.Text = ((Label)row.FindControl("lblPIN")).Text;
                txtMobile.Text = ((Label)row.FindControl("lblMobile")).Text;
                txtEmail.Text = ((Label)row.FindControl("lblEmail")).Text;
                txtDOB.Text = DateTime.TryParseExact(((Label)row.FindControl("lblDOB")).Text, "dd/MM/yyyy",
                               System.Globalization.CultureInfo.InvariantCulture,
                               System.Globalization.DateTimeStyles.None,
                               out DateTime dob)
                               ? dob.ToString("dd/MM/yyyy")
                               : string.Empty;

                if (DropDownList4.SelectedValue.ToString() == "SWITCH IN")
                {
                    lblswisch.Visible = true;
                    SwitchChebox.Visible = true;

                    switchSchemeName.Enabled = true;
                    TextBox12.Enabled = true;
                }
                else
                {
                    lblswisch.Visible = false;
                    SwitchChebox.Visible = false;

                    switchSchemeName.Enabled = false;
                    TextBox12.Enabled = false;
                }

                // sipstp code
                string sipstpvaluedo = sipStp.SelectedValue;

                sipstpupdate(sipstpvaluedo);

                // disable/enable payment methods based on transaction type
                DisablePaymentMethods(DropDownList4.SelectedValue);

                 
                string m_ar_data_sip_sub_type = ((Label)row.FindControl("lblM_FIND_AR_SIP_SUP_TYPE")).Text;

                if(sipstpvaluedo.Trim().ToUpper() == "SIP" && !string.IsNullOrEmpty(m_ar_data_sip_sub_type))
                {
                    ddlM_SIPSubType.SelectedValue = m_ar_data_sip_sub_type;
                }




            }

            UpdatePanel2.Update();

        }

        protected void trantypechangeINupdate(object sender, EventArgs e)
        {
            try
            {
                string currentTranType = DropDownList4.SelectedValue.ToString();
                DisablePaymentMethods(currentTranType);
                if (currentTranType == "SWITCH IN")
                {
                    lblswisch.Visible = true;
                    SwitchChebox.Visible = true;
                    switchSchemeName.Enabled = true;
                    TextBox12.Enabled = true;

                }
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message;
                return;
            }
        }

        protected void DisablePaymentMethods(string mode)
        {
            bool disable = mode == "SWITCH IN";

            // Set enabled/disabled state for all radio buttons
            cheque_view.Enabled = !disable;
            draft_view.Enabled = !disable;
            ecs_view.Enabled = !disable;
            cash_view.Enabled = !disable;
            others_view.Enabled = !disable;
            rtgs_view.Enabled = !disable;
            fund_view.Enabled = !disable;

            // TextBox controls: clear if disabling, keep value if enabling
            TextBox2.Enabled = !disable;
            if (disable) TextBox2.Text = "";

            TextBox3.Enabled = !disable;
            if (disable) TextBox3.Text = "";

            TextBox4.Enabled = !disable;
            if (disable) TextBox4.Text = "";

            TextBox5.Enabled = !disable;
            if (disable) TextBox5.Text = "";

            TextBox6.Enabled = !disable;
            if (disable) TextBox6.Text = "";

            TextBox7.Enabled = !disable;
            if (disable) TextBox7.Text = "";

            TextBox8.Enabled = !disable;
            if (disable) TextBox8.Text = "";

            TextBox9.Enabled = !disable;
            if (disable) TextBox9.Text = "";

            TextBox10.Enabled = !disable;
            if (disable) TextBox10.Text = "";

            TextBox17.Enabled = !disable;
            if (disable) TextBox17.Text = "";

            TextBox18.Enabled = !disable;
            if (disable) TextBox18.Text = "";

            TextBox19.Enabled = !disable;
            if (disable) TextBox19.Text = "";

            TextBox20.Enabled = !disable;
            if (disable) TextBox20.Text = "";

            TextBox21.Enabled = !disable;
            if (disable) TextBox21.Text = "";
        }


        #region btnReset_Click
        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearFields();
        }


        private void ClearFields()
        {

            // Clear all fields if no data is found
            hdnSelectedPaymentMethod_view.Value = null;
            hdnSelectedPaymentMethod_view.Value = "cheque_view";

            string Payscript = "initializePaymentView();";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "initializePaymentView", Payscript, true);

            switchSchemeName.Text = "";
            lblWaning2.Visible = false;
            lblWaning2.Text = string.Empty;
            txtClientCode.Text = "";
            accountHolderV.Text = "";
            businessCodeV.Text = "";
            panNo.Text = "";
            TextBox11.Text = "";
            TextBox12.Text = "";
            TextBox13.Text = "";
            TextBox14.Text = "";
            dropDat.Text = "";
            frequency.SelectedIndex = 0;
            installmentsNos.Text = "";
            cobCase.Checked = false;
            swpCase.Checked = false;
            freedomSip.Checked = false;
            or99Years.Checked = false;

            txtSIPStartDateupdate.Text = "";
            txtSIPEndDateupdate.Text = "";

            // Uncheck radio buttons
            cheque_view.Checked = true;
            draft_view.Checked = false;
            ecs_view.Checked = false;
            cash_view.Checked = false;
            others_view.Checked = false;
            rtgs_view.Checked = false;
            fund_view.Checked = false;
            SwitchChebox.Checked = false;

            TextBox2.Text = "";  // Cheque No
            TextBox3.Text = "";  // Cheque Dated
            TextBox4.Text = "";  // Draft No
            TextBox5.Text = "";  // Draft Date
            TextBox6.Text = "";  // RTGS Transaction No
            TextBox7.Text = "";  // RTGS Date
            TextBox8.Text = "";  // Fund Transfer No
            TextBox9.Text = "";  // Fund Transfer Date
            TextBox10.Text = ""; // ECS Reference No
            TextBox17.Text = ""; // ECS Date
            TextBox18.Text = ""; // Cash Amount
            TextBox19.Text = ""; // Cash Payment Date
            TextBox20.Text = ""; // Others Reference No
            TextBox21.Text = ""; // Payment Date
            SchemeName.Text = "";

            remarks.Text = "";
            expensesPercent.Text = "";
            expensesRs.Text = "";
            autoSwitchOnMaturity.Checked = false;
            recoStatus.Text = "";

            txtCanTrcode.Text = "";


            // Reset the dropdown list to the first item
            CmbReson.SelectedIndex = 0;

            // Clear the remarks textbox
            txtremark.Text = "";

            // Reset date fields
            fromDate.Text = "";
            toDate.Text = "";

            // Reset dropdown
            orderBy.SelectedIndex = 0;

            // Reset radio buttons
            ascending.Checked = false;
            descending.Checked = false;

            // Reset text fields
            TextBox15.Text = "";  // PAN No.
            trNo.Text = "";       // TR No.
            uniqueClientCode.Text = "";  // Unique Client Code
            anaCode.Text = "";     // ANA Code
            chequeNo.Text = "";    // Cheque No
            appNo.Text = "";       // App No

            //  chequeNo.Text = "";
            holderCodeV.Text = "";
            RadioButton9.Checked = false;
            //  RadioButton3.Checked = false;

            amcSelect.ClearSelection();
            DropDownList4.ClearSelection();
            bankName.ClearSelection();
            sipStp.ClearSelection();

            if (hdnSelectedPaymentMethod_view != null)
            {
                hdnSelectedPaymentMethod_view.Value = "";  // Clear the value on first load
            }
            // Show cheque details (only cheque-related fields)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showChequeDetailsOnlyView", "showChequeDetailsOnlyView();", true);


            // Clear the data source and rebind the grid
            tableSearchResults.DataSource = null;
            tableSearchResults.DataBind();

            gridview1.DataSource = null;
            gridview1.DataBind();

            gvTransactionLog.DataSource = null;
            gvTransactionLog.DataBind();

            UpdatePanelSearchAR.Update();
            UpdatePanelgrid.Update();

        }
        #endregion

        #region statedropdownlist
        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCity = ddlCityADD.SelectedItem.Text;
            UpdateStateDropdown(selectedCity);
        }


        protected void UpdateStateDropdown(string selectedCity)
        {

            //  string selectedCity = ddlCityADD.SelectedItem.ToString();
            MfPunchingController controller = new MfPunchingController();
            DataTable dt = controller.GetStatesByCity(selectedCity);

            //ddlStateADD.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                // Clear existing items and set the data source for the DropDownList
                ddlStateADD.DataSource = dt;
                ddlStateADD.DataTextField = "STATE_NAME";
                ddlStateADD.DataValueField = "state_id";
                ddlStateADD.DataBind();

            }
            else
            {
                // Clear the dropdown if no zones are found
                ddlStateADD.Items.Clear();
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "showAddressUpOnCityChange", "showPopup3();", true);

        }
        #endregion

        #region statedropdownlistView
        protected void ddlCity3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCity3 = ddlCityADD3.SelectedItem.Text;
            UpdateStateDropdownview(selectedCity3);
        }

        protected void UpdateStateDropdownview(string selectedCity3)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup4", "showPopup4();", true);

            //  string selectedCity = ddlCityADD.SelectedItem.ToString();
            MfPunchingController controller = new MfPunchingController();
            DataTable dt = controller.GetStatesByCity(selectedCity3);

            //ddlStateADD.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                ddlStateADD3.DataSource = dt;
                ddlStateADD3.DataTextField = "STATE_NAME";
                ddlStateADD3.DataValueField = "state_id";
                ddlStateADD3.DataBind();
            }
            else
            {
                // Clear the dropdown if no zones are found
                ddlStateADD3.Items.Clear();
            }
        }
        #endregion

        #region CancelTranM
        protected void btnOk_Click(object sender, EventArgs e)
        {


            string reasonText = txtremark.Text;
            string[] reasonDetails = CmbReson.SelectedValue.Split('$');
            string reason = reasonDetails[0];

            //  txtCanTrcode.Text = trNo.Text;
            string transactionCode = txtCanTrcode.Text;
            DateTime cancelDate = DateTime.Now;
            mdatecancel.Text = DateTime.Now.ToString("dd/MM/yyyy");

            if (string.IsNullOrWhiteSpace(transactionCode))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First provide any transaction to cancle');", true);
                return;
            }

            try
            {

                MfPunchingController controller = new MfPunchingController();
                string result = controller.CancelTransaction(transactionCode, cancelDate, reason, reasonText);

                // Show success message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Transaction Cancelled Successfully');", true);

                // Clear form fields
                txtCanTrcode.Text = string.Empty;
                mdatecancel.Text = "__/__/____";
                txtremark.Text = string.Empty;
                CmbReson.SelectedIndex = -1; // Clear combo box selection
                CancelPopup.Visible = false; // Hide the frame or panel
            }
            catch (Exception ex)
            {
                // Handle the error, log it, and show a message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Error cancelling the transaction: " + ex.Message + "');", true);
            }
        }
        #endregion

        #region printbutton
        protected void btn_PrintModify(object sender, EventArgs e)
        {
            try
            {
                // Get user's choice from hidden field (Set via JavaScript confirm box)
                string multiplePrintOption = hdnPrintOption.Value;

                // Connection String
                string connStr = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

                using (OracleConnection con = new OracleConnection(connStr))
                {
                    con.Open();

                    // ✅ 1. Construct SQL Query exactly as provided
                    StringBuilder sql = new StringBuilder();

                    sql.Append("CREATE OR REPLACE VIEW NEWarmf AS SELECT b.client_code, 'P' ar_type, t.tran_code, TR_DATE, CHEQUE_DATE cheque_date, ");
                    sql.Append("CHEQUE_NO cheque_no, t.bank_name, amount, b.client_code source_code, ");
                    sql.Append("app_no, NVL(upfront_ope_paid_temptran(t.tran_code), 0) paid_brok, ");
                    sql.Append("NVL((SELECT SUM(NVL(amt, 0)) FROM payment_detail WHERE tran_code = t.tran_code), 0) paidamt, '' asr, PAYMENT_MODE, ");
                    sql.Append("investor_name inv, (SELECT MAX(agent_name) FROM agent_master WHERE agent_code = t.source_code) client, ");
                    sql.Append("exist_code as existcode, address1 add1, address2 add2, '' loc, pincode pin, ");
                    sql.Append("(SELECT MAX(city_name) FROM city_master WHERE city_id = ");
                    sql.Append("(SELECT MAX(city_id) FROM agent_master WHERE agent_code = t.source_code)) ccity, ");
                    sql.Append("mobile ph, email, 0 arn, '' subbroker, ");
                    sql.Append("(SELECT rm_name FROM employee_master WHERE payroll_id = TO_CHAR(t.BUSINESS_RMCODE) ");
                    sql.Append("AND (TYPE = 'A' OR TYPE IS NULL)) rname, ");
                    sql.Append("(SELECT payroll_id FROM employee_master WHERE payroll_id = TO_CHAR(t.BUSINESS_RMCODE) ");
                    sql.Append("AND (TYPE = 'A' OR TYPE IS NULL)) rcode, ");
                    sql.Append("(SELECT branch_name FROM branch_master WHERE branch_code = t.BUSI_BRANCH_CODE) bname, ");
                    sql.Append("(SELECT address1 FROM branch_master WHERE branch_code = t.BUSI_BRANCH_CODE) badd1, ");
                    sql.Append("(SELECT address2 FROM branch_master WHERE branch_code = t.BUSI_BRANCH_CODE) badd2, ");
                    sql.Append("(SELECT phone FROM branch_master WHERE branch_code = t.BUSI_BRANCH_CODE) bphone, ");
                    sql.Append("(SELECT location_name FROM location_master WHERE location_id = ");
                    sql.Append("(SELECT location_id FROM branch_master WHERE branch_code = t.BUSI_BRANCH_CODE)) bloc, ");
                    sql.Append("(SELECT city_name FROM city_master WHERE city_id = ");
                    sql.Append("(SELECT city_id FROM branch_master WHERE branch_code = t.BUSI_BRANCH_CODE)) bcity, ");
                    sql.Append("(SELECT mut_name FROM mut_fund WHERE mut_code = t.MUT_CODE) compmf, ");
                    sql.Append("'Bajaj Capital Limited' compgroup, ");
                    sql.Append("(SELECT sch_name FROM scheme_info WHERE sch_code = t.SCH_CODE) schmf, ");
                    sql.Append("(SELECT short_name FROM scheme_info WHERE sch_code = t.SCH_CODE) sschmf, ");
                    sql.Append("'" + Session["LoginId"].ToString() + "' LOGGEDUSERID ");
                    sql.Append("FROM transaction_mf_Temp1 t, client_master b ");
                    sql.Append("WHERE t.source_code = b.client_code ");

                    // ✅ Correct Date Handling (Convert Date from `TextBox` into Oracle `TO_DATE()`)
                    DateTime parsedDate;
                    if (!DateTime.TryParseExact(TextBox11.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        Response.Write("<script>alert('Invalid Date Format! Please enter a valid date in dd/MM/yyyy format.');</script>");
                        return;
                    }
                    //  sql.Append("AND TR_DATE = TO_DATE('" + parsedDate.ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy') ");

                    sql.Append("AND MOVE_FLAG1 IS NULL ");
                    sql.Append("AND (asa <> 'C' OR asa IS NULL) ");
                    sql.Append("AND BUSINESS_RMCODE = " + Convert.ToInt32(businessCodeV.Text.Trim()) + " ");
                    sql.Append("AND SOURCE_CODE = '" + txtClientCode.Text.Trim().Substring(0, Math.Min(8, txtClientCode.Text.Trim().Length)) + "' ");

                    // ✅ Handle Multiple Transactions Condition (Yes/No Confirmation)
                    if (multiplePrintOption == "No")
                    {
                        sql.Append("AND tran_code = '" + trNo.Text.Trim() + "' ");
                    }

                    using (OracleCommand cmd = new OracleCommand(sql.ToString(), con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // ✅ Fetch Data
                    string fetchSQL = "SELECT * FROM NEWarmf";
                    OracleDataAdapter da = new OracleDataAdapter(fetchSQL, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ Bind Data to GridView
                    gridview1.DataSource = dt;
                    gridview1.DataBind();

                    // Trigger the print function after updating the GridView
                    ScriptManager.RegisterStartupScript(this, GetType(), "TriggerPrint", "triggerPrintGrid();", true);
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }

        private string GenerateTableRows(DataTable dt)
        {
            string rows = "";
            int srNo = 1;
            foreach (DataRow row in dt.Rows)
            {
                rows += $"<tr>" +
                        $"<td>{srNo++}</td>" +
                        $"<td>{row["TRAN_CODE"]}</td>" +
                        $"<td>{row["INVESTOR_NAME"]}</td>" +
                        $"<td>{row["SCH_NAME"]}</td>" +
                        $"<td>{row["APP_NO"]}</td>" +
                        //$"<td>{row["PayMode"]}</td>" +
                        $"<td>{row["CHEQUE_NO"]}</td>" +
                        $"<td>{row["BANK_NAME"]}</td>" +
                        $"<td>{row["AMOUNT"]}</td>" +
                        $"</tr>";
            }
            return rows;
        }
        #endregion

        #region printbutton
        protected void btn_PrintAR(object sender, EventArgs e)
        {
            // Load data and generate the HTML table rows
            TransactionFilter filter = new TransactionFilter
            {
                TRNo = trNo.Text
            };

            DataTable dt = new MfPunchingController().GetMfDataByTranCode(filter);// Replace with your actual data fetching method
            string tableRows = GenerateTableRowsAR(dt);

            // Add the generated table rows to the panel
            printableArea.Controls.Add(new LiteralControl(tableRows));

            // Call the JavaScript function to open a new tab and print
            ClientScript.RegisterStartupScript(this.GetType(), "PrintPopup", "printDiv();", true);
        }

        private string GenerateTableRowsAR(DataTable dt)
        {
            string rows = "";
            int srNo = 1;
            foreach (DataRow row in dt.Rows)
            {
                rows += $"<tr>" +
                        $"<td>{srNo++}</td>" +
                        $"<td>{row["TRAN_CODE"]}</td>" +
                        $"<td>{row["INVESTOR_NAME"]}</td>" +
                        $"<td>{row["SCH_NAME"]}</td>" +
                        $"<td>{row["APP_NO"]}</td>" +
                        //$"<td>{row["PayMode"]}</td>" +
                        $"<td>{row["CHEQUE_NO"]}</td>" +
                        $"<td>{row["BANK_NAME"]}</td>" +
                        $"<td>{row["AMOUNT"]}</td>" +
                        $"</tr>";
            }
            return rows;
        }
        #endregion

        #region CmdViewLog_Click
        protected void CmdViewLog_Click(object sender, EventArgs e)
        {
            string tranCode = trNo.Text.Trim();


            if (string.IsNullOrEmpty(tranCode))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select Any Transaction First');", true);
                return;
            }


            if (string.IsNullOrEmpty(txtClientCode.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Select Any Transaction First');", true);
                return;
            }

            // Fetch data from the controller
            MfPunchingController controller = new MfPunchingController();
            DataTable dt = controller.GetTransactionLog(tranCode);

            if (dt.Rows.Count > 0)
            {
                gvTransactionLog.DataSource = dt;
                gvTransactionLog.DataBind();

                // Make the transaction log panel visible
                FrmTransactionLog.Visible = true;
            }
            else
            {
                // Handle the case where no data is found
                FrmTransactionLog.Visible = false;
                // Optionally, you can display a message indicating no data was found
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No transaction logs found for the provided code.');", true);
            }

        }
        #endregion

        #region btnShowA_Click
        protected void btnShowA_Click(object sender, EventArgs e)
        {
            try
            {

                UpdatePanelFillByDT.Update();
                UpdatePanelDTNumber.Update();
                bool isAuthorized = pc.AuthorizedRolesMF.Contains(pc.currentRoleID());


                string dtNumber = dtNumberA.Text.Trim();
                if (!string.IsNullOrEmpty(dtNumber) && dtNumber.All(char.IsDigit) && dtNumber.Length <= 10)
                {
                    #region Clear Add Fields

                    accountHolder.Text = "";
                    applicationNo.Text = "";
                    folioNoo.Text = "";
                    scheme.Text = "";
                    businessCode.Text = "";
                    txtPanADD.Text = "";
                    holderCode.Text = "";
                    transactionDate.Text = "";
                    txtAddressADD.Text = "";
                    txtAddressADD2.Text = "";
                    txtPinADD.Text = "";
                    txtMobileADD.Text = "";
                    txtEmailADD.Text = "";
                    cheque.Checked = false;
                    regular.Checked = false;
                    invcode.Text = "";
                    RMNAMEP.Text = "";
                    amountt.Text = "";
                    txtSearchSchemeDetails.Text = "";
                    txtAadharADD.Text = "";
                    txtAadhar.Text = "";
                    txtDOB.Text = "";
                    txtDOBADD.Text = "";
                    hdncountryid.Value = "";
                    //RadioButton3.Checked = false;
                    chkAutoSwitchOnMaturity.Checked = false;
                    chkCOBCase.Checked = false;
                    chkSWPCase.Checked = false;
                    chkFreedomSIP.Checked = false;
                    rdo99Years.Checked = false;
                    amc.ClearSelection();
                    transactionType.ClearSelection();
                    branch.ClearSelection();
                    ddlSipStp.ClearSelection();
                    lblMessage.Text = "";

                    // Show only cheque details
                    txtChequeNo.Visible = true;
                    txtChequeDated.Visible = true;

                    // Optionally, set the value of the hidden field
                    // Clear the hidden payment method field (optional)
                    if (hdnSelectedPaymentMethod != null)
                    {
                        hdnSelectedPaymentMethod.Value = "";  // Clear the value on first load
                    }


                    txtChequeNo.Text = string.Empty;
                    txtChequeDated.Text = string.Empty;
                    txtDraftNo.Text = string.Empty;
                    txtDraftDate.Text = string.Empty;
                    txtRtgsNo.Text = string.Empty;
                    txtRtgsDate.Text = string.Empty;
                    txtNeftNo.Text = string.Empty;
                    txtNeftDate.Text = string.Empty;
                    txtEcsReference.Text = string.Empty;
                    txtEcsDate.Text = string.Empty;
                    txtCashAmount.Text = string.Empty;
                    txtCashDate.Text = string.Empty;
                    txtOthersReference.Text = string.Empty;
                    txtOthersDate.Text = string.Empty;
                    txtExpensesPercent.Text = string.Empty;
                    txtExpensesRs.Text = string.Empty;
                    txtSearchDetails.Text = string.Empty;

                    // Reset DropDownLists
                    ddlBankName.SelectedIndex = 0;
                    ddlSipStp.SelectedIndex = 0;
                    iNSTYPE.SelectedIndex = 0;
                    siptype.SelectedIndex = 0;
                    ddlCityADD.SelectedIndex = 0;
                    ddlStateADD.SelectedIndex = 0;
                    ddlCity.SelectedIndex = 0;
                    ddlState.ClearSelection();

                    // Uncheck all RadioButtons
                    cheque.Checked = true;
                    draft.Checked = false;
                    ecs.Checked = false;
                    cash.Checked = false;
                    others.Checked = false;
                    rtgs.Checked = false;
                    neft.Checked = false;

                    renewal.Checked = false;

                    // Uncheck CheckBox
                    chkAutoSwitchOnMaturity.Checked = false;

                    formSwitchFolio.Text = "";
                    formSwitchScheme.Text = "";

                    ddlSipStp.SelectedIndex = 0;
                    iNSTYPE.SelectedIndex = 0;
                    siptype.SelectedIndex = 0;
                    ddlFrequency.SelectedIndex = 0;

                    // Reset TextBox fields to empty
                    sipamount.Text = string.Empty;
                    txtInstallmentsNos.Text = string.Empty;
                    txtSIPEndDate.Text = string.Empty;
                    pann.Text = string.Empty;

                    // Reset CheckBoxes to unchecked state
                    chkCOBCase.Checked = false;
                    chkSWPCase.Checked = false;
                    chkFreedomSIP.Checked = false;

                    // Reset RadioButtons to unchecked state
                    renewal.Checked = false;
                    rdo99Years.Checked = false;

                    // Hide warnings and reset read-only textboxes
                    lblWarning.Visible = false;
                    lblWarning.Text = string.Empty;

                    // dtNumberA.Text = dtNumber;

                    #endregion

                    MfPunchingController controller = new MfPunchingController();
                    DataTable dt = controller.GetTransactionDetailsByDT(dtNumber);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        string statusMessage = row["STATUS_MESSAGE"].ToString();
                        if (!string.IsNullOrEmpty(statusMessage) && statusMessage != "DT is valid")
                        {
                            pc.ShowAlert(this, statusMessage);
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                            return;
                        }


                        hdncountryid.Value = row["country_id"]?.ToString();
                        string inv_code = row["INV_CODE"]?.ToString();
                        invcode.Text = inv_code;
                        businessCode.Text = row["BUSI_RM_CODE"].ToString();
                        accountHolder.Text = row["INVESTOR_NAME"].ToString();
                        holderCode.Text = row["EXIST_CODE"].ToString();
                        transactionDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                        cheque.Checked = true;
                        regular.Checked = true;

                        scheme.Text = $"{row["SCH_NAME"]};;{row["SCH_CODE"]}";
                        RMNAMEP.Text = row["RM_NAME"].ToString();

                        if (amc.Items.FindByValue(row["MUT_CODE"].ToString()) != null)
                        {
                            amc.SelectedValue = row["MUT_CODE"].ToString();
                            amc.Enabled = false;
                        }
                        if (transactionType.Items.FindByValue(row["TRAN_TYPE"].ToString()) != null)
                        {
                            transactionType.SelectedItem.Text = row["TRAN_TYPE"].ToString();
                        }

                        ListItem branchItem = branch.Items.FindByText(row["branch_name"].ToString());
                        if (branchItem != null && !string.IsNullOrEmpty(branchItem.Text))
                        {
                            branch.SelectedIndex = branch.Items.IndexOf(branchItem);
                        }

                        branch.Enabled = false;

                        // cross channel validation
                        string strValue = ValidateCrossChannelInfo(dtNumber, invcode.Text, pc.currentLoginID());
                        if (strValue == "SHOW_CROSS_CHANNEL_INFO")
                        {
                            GetSetCrossChannelData(dtNumber, invcode.Text, isAuthorized);
                        }
                        else if (strValue == "SHOW_ADDRESS_UPDATE")
                        {
                            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
                            GetSetInvAddressData(inv_code, isAuthorized);
                            hdnPopupVisible1.Value = "true";
                        }
                        else
                        {
                            pc.ShowAlert(this, strValue);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                        pc.ShowAlert(this, "Please enter a valid DT Number.");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                    pc.ShowAlert(this, "DT Number must be numeric and have a maximum of 10 digits.");
                }
            }
            catch(Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }
        public void GetSetCrossChannelData(string dt, string inv, bool isAuth, bool open=true)
        {
            try
            {
                DataTable data = new TransactionInvestmentController().GetCrissChannelData(dt, pc.currentLoginID(), inv);

                if (data != null && data.Rows.Count > 0)
                {
                    gvCrossChannel.DataSource = data;
                    gvCrossChannel.DataBind();
                    gvCrossChannel.Visible = true;
                }

                else
                {
                    pc.ShowAlert(this, "No cross-channel data found.");  
                }

                if (isAuth)
                {
                    if (open)
                    {
                        gvCrossChannel.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CrossChannelShow", "ShowCrossChannelModal();", true);
                    }
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }
        public void GetSetInvAddressData(string inv_code, bool isAuth, bool open = true)
        {
            try
            {
                PsmController pc = new PsmController();
                DataTable data = pc.PSMM_INV_ADD_UP_GET_DATA1(inv_code);

                if (data != null && data.Rows.Count > 0)
                {
                    DataRow row = data.Rows[0];

                    string msg = row["MSG"]?.ToString() ?? "";
                    string inv_code_val = row["V_INV_CODE"]?.ToString() ?? "";
                    string inv_name = row["V_INV_NAME"]?.ToString() ?? "";
                    string address1 = row["V_ADD1"]?.ToString() ?? "";
                    string address2 = row["V_ADD2"]?.ToString() ?? "";
                    string city = row["V_CITY_NAME"]?.ToString() ?? "";
                    string state = row["V_STATE_NAME"]?.ToString() ?? "";
                    string mobile = row["V_MOBILE"]?.ToString() ?? "";
                    string email = row["V_EMAIL"]?.ToString() ?? "";
                    string pin = row["V_PIN"]?.ToString() ?? "";
                    string pan = row["V_PAN"]?.ToString() ?? "";
                    string aadhar = row["V_AADHAR"]?.ToString() ?? "";
                    string dobRaw = row["V_DOB"]?.ToString() ?? "";
                    string isHead = row["V_IS_HEAD"]?.ToString() ?? "";

                    // Optional: Convert DOB to dd/MM/yyyy format
                    string dob = AnyDatToDMY(dobRaw).Replace('-','/');

                    // You can now use these values to set UI fields or process further
                    txtAddressADD.Text = address1;
                    txtAddressADD2.Text = address2;
                    txtPinADD.Text = pin;
                    txtMobileADD.Text = mobile;
                    txtPanADD.Text = pan;
                    txtEmailADD.Text = email;
                    txtAadharADD.Text = aadhar;
                    txtDOBADD.Text = dob;

                    // Optional: set city/state dropdowns
                    ListItem cityItem = ddlCityADD.Items.FindByText(city);
                    if (cityItem != null)
                    {
                        ddlCityADD.SelectedIndex = ddlCityADD.Items.IndexOf(cityItem);
                        UpdateStateDropdown(cityItem.Text);
                    }

                    // Optionally handle state as well
                    // ListItem stateItem = ddlStateADD.Items.FindByText(state);
                    // if (stateItem != null) ddlStateADD.SelectedIndex = ddlStateADD.Items.IndexOf(stateItem);
                }
                else
                {
                    pc.ShowAlert(this,"No investor data found for the given code.");
                }

                if (isAuth)
                {
                    if (open)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                    }
                }
            }
            catch (Exception ex)
            {

                pc.ShowAlert(this, "System error occurred while setting investor data: " + ex.Message);
                return;
            }
        }

        public static string AnyDatToDMY(string rawDate)
        {
            if (string.IsNullOrWhiteSpace(rawDate))
                return "";

            // Try parse using any supported formats
            if (DateTime.TryParse(rawDate, out DateTime parsed))
            {
                return parsed.ToString("dd/MM/yyyy");
            }

            // If fails, return empty string or "Invalid Date"
            return "";
        }

        protected void btnShowA_Click_old(object sender, EventArgs e)
        {

            UpdatePanelFillByDT.Update();
            UpdatePanelDTNumber.Update();

            #region Authorization Check and Popup Display
            bool isAuthorized = pc.AuthorizedRolesMF.Contains(pc.currentRoleID());

            if (!isAuthorized)
            {

                string script = @"
        hidePopup4();
        alert('You are not authorized to access this functionality.');
        console.warn('Unauthorized access attempt with role: ' + '" + pc.currentRoleID() + @"');
    ";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "BlockUnauthorizedAccess", script, true);

            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
                hdnPopupVisible1.Value = "true"; // Set the hidden field to indicate the popup is visible
            }

            #endregion

            string dtNumber = dtNumberA.Text.Trim();


            // Validation: Check if dtNumber is numeric and has a maximum length of 10 digits
            if (!string.IsNullOrEmpty(dtNumber) && dtNumber.All(char.IsDigit) && dtNumber.Length <= 10)
            {

                MfPunchingController controller = new MfPunchingController();
                DataTable dt = controller.GetTransactionDetailsByDT(dtNumber);

                accountHolder.Text = "";
                applicationNo.Text = "";
                folioNoo.Text = "";
                scheme.Text = "";
                businessCode.Text = "";
                txtPanADD.Text = "";
                holderCode.Text = "";
                transactionDate.Text = "";
                txtAddressADD.Text = "";
                txtAddressADD2.Text = "";
                txtPinADD.Text = "";
                txtMobileADD.Text = "";
                txtEmailADD.Text = "";
                cheque.Checked = false;
                regular.Checked = false;
                invcode.Text = "";
                RMNAMEP.Text = "";
                amountt.Text = "";
                txtSearchSchemeDetails.Text = "";
                txtAadharADD.Text = "";
                txtAadhar.Text = "";
                txtDOB.Text = "";
                txtDOBADD.Text = "";
                hdncountryid.Value = "";
                //RadioButton3.Checked = false;
                chkAutoSwitchOnMaturity.Checked = false;
                chkCOBCase.Checked = false;
                chkSWPCase.Checked = false;
                chkFreedomSIP.Checked = false;
                rdo99Years.Checked = false;
                amc.ClearSelection();
                transactionType.ClearSelection();
                branch.ClearSelection();
                ddlSipStp.ClearSelection();
                lblMessage.Text = "";

                // Show only cheque details
                txtChequeNo.Visible = true;
                txtChequeDated.Visible = true;

                // Optionally, set the value of the hidden field
                // Clear the hidden payment method field (optional)
                if (hdnSelectedPaymentMethod != null)
                {
                    hdnSelectedPaymentMethod.Value = "";  // Clear the value on first load
                }


                txtChequeNo.Text = string.Empty;
                txtChequeDated.Text = string.Empty;
                txtDraftNo.Text = string.Empty;
                txtDraftDate.Text = string.Empty;
                txtRtgsNo.Text = string.Empty;
                txtRtgsDate.Text = string.Empty;
                txtNeftNo.Text = string.Empty;
                txtNeftDate.Text = string.Empty;
                txtEcsReference.Text = string.Empty;
                txtEcsDate.Text = string.Empty;
                txtCashAmount.Text = string.Empty;
                txtCashDate.Text = string.Empty;
                txtOthersReference.Text = string.Empty;
                txtOthersDate.Text = string.Empty;
                txtExpensesPercent.Text = string.Empty;
                txtExpensesRs.Text = string.Empty;
                txtSearchDetails.Text = string.Empty;

                // Reset DropDownLists
                ddlBankName.SelectedIndex = 0;
                ddlSipStp.SelectedIndex = 0;
                iNSTYPE.SelectedIndex = 0;
                siptype.SelectedIndex = 0;
                ddlCityADD.SelectedIndex = 0;
                ddlStateADD.SelectedIndex = 0;
                ddlCity.SelectedIndex = 0;
                ddlState.ClearSelection();

                // Uncheck all RadioButtons
                cheque.Checked = true;
                draft.Checked = false;
                ecs.Checked = false;
                cash.Checked = false;
                others.Checked = false;
                rtgs.Checked = false;
                neft.Checked = false;

                renewal.Checked = false;

                // Uncheck CheckBox
                chkAutoSwitchOnMaturity.Checked = false;

                formSwitchFolio.Text = "";
                formSwitchScheme.Text = "";

                ddlSipStp.SelectedIndex = 0;
                iNSTYPE.SelectedIndex = 0;
                siptype.SelectedIndex = 0;
                ddlFrequency.SelectedIndex = 0;

                // Reset TextBox fields to empty
                sipamount.Text = string.Empty;
                txtInstallmentsNos.Text = string.Empty;
                txtSIPEndDate.Text = string.Empty;
                pann.Text = string.Empty;

                // Reset CheckBoxes to unchecked state
                chkCOBCase.Checked = false;
                chkSWPCase.Checked = false;
                chkFreedomSIP.Checked = false;

                // Reset RadioButtons to unchecked state
                renewal.Checked = false;
                rdo99Years.Checked = false;

                // Hide warnings and reset read-only textboxes
                lblWarning.Visible = false;
                lblWarning.Text = string.Empty;

                // dtNumberA.Text = dtNumber;


                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];



                    // Check the STATUS_MESSAGE column for any status-related messages
                    string statusMessage = row["STATUS_MESSAGE"].ToString();
                    if (!string.IsNullOrEmpty(statusMessage) && statusMessage != "DT is valid")
                    {
                        // Display the status message and exit for invalid DT
                        lblMessage.Text = statusMessage;
                        lblMessage.CssClass = "text-danger"; // Optional: Add CSS class for styling the message
                        lblMessage.Focus();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                        return;
                    }

                    string countryid = row["country_id"] == DBNull.Value ? null : row["country_id"].ToString();

                    hdncountryid.Value = countryid;

                    invcode.Text = row["INV_CODE"].ToString();

                    txtAadharADD.Text = row["AADHAR_CARD_NO"].ToString();

                    txtPinADD.Text = row["PINCODE"].ToString();

                    businessCode.Text = row["BUSI_RM_CODE"].ToString();
                    accountHolder.Text = row["INVESTOR_NAME"].ToString();
                    holderCode.Text = row["EXIST_CODE"].ToString();
                    transactionDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    cheque.Checked = true;
                    regular.Checked = true;

                    txtAddressADD.Text = row["ADDRESS1"].ToString();
                    txtAddressADD2.Text = row["ADDRESS2"].ToString();
                    txtMobileADD.Text = row["MOBILE"].ToString();
                    txtPanADD.Text = row["PAN"].ToString();
                    txtEmailADD.Text = row["EMAIL"].ToString();

                    // Try parsing the date in the expected format (it could include time in the data like "4/6/1997 12:00")
                    if (DateTime.TryParse(row["DOB"].ToString(), out DateTime joiningDate))
                    {
                        // If parsing succeeds, discard the time part and set the text in dd/MM/yyyy format
                        txtDOBADD.Text = joiningDate.Date.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        // If parsing fails, set the text to empty
                        txtDOBADD.Text = string.Empty;
                    }

                    scheme.Text = $"{row["SCH_NAME"]};;{row["SCH_CODE"]}";
                    RMNAMEP.Text = row["RM_NAME"].ToString();

                    if (amc.Items.FindByValue(row["MUT_CODE"].ToString()) != null)
                    {
                        amc.SelectedValue = row["MUT_CODE"].ToString();
                        amc.Enabled = false;
                    }
                    if (transactionType.Items.FindByValue(row["TRAN_TYPE"].ToString()) != null)
                    {
                        transactionType.SelectedItem.Text = row["TRAN_TYPE"].ToString();
                    }
                    ListItem cityItem = ddlCityADD.Items.FindByText(row["CITY_NAME"].ToString());
                    if (cityItem != null && !string.IsNullOrWhiteSpace(cityItem.Text))
                    {
                        ddlCityADD.SelectedIndex = ddlCityADD.Items.IndexOf(cityItem);
                        UpdateStateDropdown(cityItem.Text);
                    }

                    //HiddenField countryid = (HiddenField)FindControl("countryid");
                    //if (countryid != null)
                    //{
                    //    countryid.Value = row["country_id"].ToString();
                    //}

                    ListItem branchItem = branch.Items.FindByText(row["branch_name"].ToString());
                    if (branchItem != null && !string.IsNullOrEmpty(branchItem.Text))
                    {
                        branch.SelectedIndex = branch.Items.IndexOf(branchItem);
                    }

                    branch.Enabled = false;


                }
                else
                {
                    // Handle case where no data is found for the DT number
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                    lblMessage.Text = "Please enter a valid DT Number.";
                    lblMessage.Focus();

                }
            }
            else
            {
                // Validation failed
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "hidePopup3();", true);
                lblMessage.Text = "DT Number must be numeric and have a maximum of 10 digits.";
                lblMessage.Focus();
            }


        }
       
        #endregion

        #region btnResetClick
        protected void btnResetClick(object sender, EventArgs e)
        {
            ClearFieldsADD();
        }


        private void ClearFieldsADD()
        {
            // Clear all fields if no data is found
            string script = @"
        console.log('🚨 Clearing sessionStorage...');
        sessionStorage.clear(); // Clear all sessionStorage data
        console.log('✅ sessionStorage has been cleared.');
        ResetPaymentMethod();
    ";

            // Register the script for execution on the client side
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearSessionStorage", script, true);

            accountHolder.Text = "";
            applicationNo.Text = "";
            folioNoo.Text = "";
            scheme.Text = "";
            businessCode.Text = "";
            txtPanADD.Text = "";
            holderCode.Text = "";
            transactionDate.Text = "";
            txtAddressADD.Text = "";
            txtAddressADD2.Text = "";
            txtPinADD.Text = "";
            txtMobileADD.Text = "";
            txtEmailADD.Text = "";
            cheque.Checked = false;
            regular.Checked = false;
            invcode.Text = "";
            RMNAMEP.Text = "";
            amountt.Text = "";
            txtSearchSchemeDetails.Text = "";
            txtAadharADD.Text = "";
            txtAadhar.Text = "";
            txtDOB.Text = "";
            txtDOBADD.Text = "";

            //RadioButton3.Checked = false;
            chkAutoSwitchOnMaturity.Checked = false;
            chkCOBCase.Checked = false;
            chkSWPCase.Checked = false;
            chkFreedomSIP.Checked = false;
            rdo99Years.Checked = false;
            amc.ClearSelection();
            transactionType.ClearSelection();
            branch.ClearSelection();
            ddlSipStp.ClearSelection();
            lblMessage.Text = "";


            hdnSelectedPaymentMethod.Value = null;
            hdnSelectedPaymentMethod.Value = "cheque";

            string Payscript = "initializePaymentMethod();";
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "initializePaymentMethod", Payscript, true);

            txtChequeNo.Text = string.Empty;
            txtChequeDated.Text = string.Empty;
            txtDraftNo.Text = string.Empty;
            txtDraftDate.Text = string.Empty;
            txtRtgsNo.Text = string.Empty;
            txtRtgsDate.Text = string.Empty;
            txtNeftNo.Text = string.Empty;
            txtNeftDate.Text = string.Empty;
            txtEcsReference.Text = string.Empty;
            txtEcsDate.Text = string.Empty;
            txtCashAmount.Text = string.Empty;
            txtCashDate.Text = string.Empty;
            txtOthersReference.Text = string.Empty;
            txtOthersDate.Text = string.Empty;
            txtExpensesPercent.Text = string.Empty;
            txtExpensesRs.Text = string.Empty;
            txtSearchDetails.Text = string.Empty;

            // Reset DropDownLists
            ddlBankName.SelectedIndex = 0;
            ddlSipStp.SelectedIndex = 0;
            iNSTYPE.SelectedIndex = 0;
            siptype.SelectedIndex = 0;
            ddlCityADD.SelectedIndex = 0;
            ddlStateADD.SelectedIndex = 0;
            ddlCity.SelectedIndex = 0;
            ddlState.ClearSelection();

            // Uncheck all RadioButtons
            cheque.Checked = true;
            draft.Checked = false;
            ecs.Checked = false;
            cash.Checked = false;
            others.Checked = false;
            rtgs.Checked = false;
            neft.Checked = false;
            fresh.Checked = false;
            renewal.Checked = false;

            // Uncheck CheckBox
            chkAutoSwitchOnMaturity.Checked = false;

            formSwitchFolio.Text = "";
            formSwitchScheme.Text = "";

            ddlSipStp.SelectedIndex = 0;
            iNSTYPE.SelectedIndex = 0;
            siptype.SelectedIndex = 0;
            ddlFrequency.SelectedIndex = 0;

            // Reset TextBox fields to empty
            sipamount.Text = string.Empty;
            txtInstallmentsNos.Text = string.Empty;
            txtSIPEndDate.Text = string.Empty;
            pann.Text = string.Empty;

            // Reset CheckBoxes to unchecked state
            chkCOBCase.Checked = false;
            chkSWPCase.Checked = false;
            chkFreedomSIP.Checked = false;

            // Reset RadioButtons to unchecked state
            fresh.Checked = false;
            renewal.Checked = false;
            rdo99Years.Checked = false;

            // Hide warnings and reset read-only textboxes
            lblWarning.Visible = false;
            lblWarning.Text = string.Empty;

            dtNumberA.Text = "";
        }
        #endregion

        #region btnAdd_Change
        protected void btnAdd_Change(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dtNumberA.Text.Trim()))
            {
                // Show an alert if dtNumberA is empty
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Information is not present');", true);
            }
            else
            {
                // Panel2.Visible = true;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
            }
        }

        #endregion


        protected void btnGo_Click(object sender, EventArgs e)
        {
            string searchTerm = txtGo.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                FillSchemeGrid(searchTerm);
            }

            // Keep the popup open after postback
            SchemeSearchPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepPopupOpen", "showSchemeSearchPanel();", true);
        }

        protected void btnSelectScheme_Click(object sender, EventArgs e)
        {

            SchemeSearchPanel.Style["display"] = "none"; // Close the panel

        }

        private void FillSchemeGrid(string schemeCode)
        {
            // Call the controller to get the scheme details
            DataTable dt = new MfPunchingController().GetSchemeDetails(schemeCode);

            // Bind the data to the GridView
            SchemeGrid.DataSource = dt;
            SchemeGrid.DataBind();


        }


        #region btnSearchScheme
        protected void btnSearchSchemeADD(object sender, EventArgs e)
        {
            hfSearchClicked.Value = "1";
            SchemeSearchPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSchemeSearchPanel", "showSchemeSearchPanel();", true);
        }
        #endregion

        #region btnfindSchemeButton
        protected void btnfindSchemeButton(object sender, EventArgs e)
        {
            SchemeSearchPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSchemeSearchPanel", "showSchemeSearchPanel();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepPopupOpen", "showPopup3();", true);
        }
        #endregion

        #region btnSearchSchemeDetails_Click
        protected void btnSearchSchemeDetails_Click(object sender, EventArgs e)
        {
            txtSearchDetails.Text = amc.SelectedItem?.Text ?? string.Empty;

            SchemeSearchPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showSchemeDetailsPanel", "showSchemeDetailsPanel();", true);
        }
        #endregion

        #region btn_SearchFormSwitchScheme
        protected void btn_SearchFormSwitchScheme(object sender, EventArgs e)
        {
            hfSearchClicked.Value = "0";

            txtGo.Text = amc.SelectedItem?.Text ?? string.Empty;

            SchemeSearchPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSchemeSearchPanel", "showSchemeSearchPanel();", true);
        }
        #endregion

        protected void btnSearchDetails_Click(object sender, EventArgs e)
        {
            string searchDetails = txtSearchDetails.Text.Trim();
            if (!string.IsNullOrEmpty(searchDetails))
            {
                FillDetailsGrid(searchDetails);
            }

            // Keep the popup open after postback
            SchemeDetailsPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepDetailsPopupOpen", "showSchemeDetailsPanel();", true);
        }

        private void FillDetailsGrid(string details)
        {
            // Call the controller to get the details based on the input
            DataTable dt = new MfPunchingController().GetSchemeDetails(details);

            // Bind the data to the DetailsGrid
            DetailsGrid.DataSource = dt;
            DetailsGrid.DataBind();
        }

        protected void btnSelectDetail_Click(object sender, EventArgs e)
        {
            // Hide the panel after selection
            SchemeDetailsPanel.Style["display"] = "none";
        }



        #region btnautoSwitch
        protected void btnautoSwitch(object sender, EventArgs e)
        {
            SchemeSearchPanel.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSchemeSearchPanel", "showSchemeSearchPanel();", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepPopupOpen", "showPopup3();", true);
        }
        #endregion

        //#region btnfindSchemeButton
        //protected void btn_SearchFormSwitchScheme(object sender, EventArgs e)
        //{
        //    SchemeSearchPanel.Visible = true;
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSchemeSearchPanel", "showSchemeSearchPanel();", true);
        //}
        //#endregion

        private string schcodeadetail;

        protected void detailsSearchResultsClient_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectDetailRow")
            {
                // Get the reference of the clicked button
                Button btnSelect = (Button)e.CommandSource;

                // Retrieve the row that contains the button clicked
                GridViewRow row = (GridViewRow)btnSelect.NamingContainer;

                // Now you have the correct row, and you can retrieve values from the row and populate the text fields
                string schCode = ((Label)row.FindControl("lblDetailName")).Text;

                schcodeadetail = ((Label)row.FindControl("lblDetailCode")).Text;
                Session["SchemeCodeA"] = schcodeadetail;
                // Set the values to the respective text boxes
                txtSearchSchemeDetails.Text = ((Label)row.FindControl("lblDetailName")).Text + ";;" + ((Label)row.FindControl("lblDetailCode")).Text;

            }
        }

        private string schcodead;

        protected void tableSearchResultsClient_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                Button btnSelect = (Button)e.CommandSource;
                GridViewRow row = (GridViewRow)btnSelect.NamingContainer;

                // Get values from the row
                string amcName = ((Label)row.FindControl("lblAMCName")).Text;
                string selectedAmcValue = amc.SelectedItem.Text.Trim();

                // Normalize spaces (replace multiple spaces with a single space) and convert to lowercase for comparison
                string amcNameCh = System.Text.RegularExpressions.Regex.Replace(amcName, @"\s+", " ").ToLower();
                string selectedAmcValueCh = System.Text.RegularExpressions.Regex.Replace(selectedAmcValue, @"\s+", " ").ToLower();

                string schemeName = ((Label)row.FindControl("lblSchemeName")).Text;
                schcodead = ((Label)row.FindControl("lblSchemeCode")).Text;
                string combinedText = schemeName + ";;" + schcodead;

                string FromschemeTo = scheme.Text.ToString();
                string schemeToChe = System.Text.RegularExpressions.Regex.Replace(combinedText, @"\s+", " ").ToLower();
                string FromschemeToChe = System.Text.RegularExpressions.Regex.Replace(FromschemeTo, @"\s+", " ").ToLower();

                // Compare the values
                if (amcNameCh != selectedAmcValueCh)
                {
                    // Show the error message if the AMC names are different
                    string message = "Switch Scheme's AMC can't be different as the From Scheme";

                    // Use double quotes to enclose the message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage",
                        "alert(\"" + message + "\");", true);
                    return;
                }

                if (schemeToChe == FromschemeToChe)
                {
                    // Show the error message if the AMC names are different
                    string message = "Switch Scheme can't be exact same as From Scheme";

                    // Use double quotes to enclose the message
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage",
                        "alert(\"" + message + "\");", true);
                    return;
                }





                // Store the values in hidden fields for later use in Page_Load
                hiddenSchemeName.Value = schemeName;
                hiddenCombinedText.Value = combinedText;
                hiddenAMCName.Value = amcName;



                if (hfSearchClicked.Value == "1")
                {
                    scheme.Text = combinedText;
                }
                else
                {
                    formSwitchScheme.Text = combinedText;
                }


                // JavaScript to prompt the user which TextBox to fill
                //    string script = $@"
                //var fillSwitchScheme = confirm('Do You Want to fill formSwitchScheme textbox?');
                //if (fillSwitchScheme) {{
                //    document.getElementById('{selectedOption.ClientID}').value = 'formSwitchScheme';
                //}} else {{
                //    var fillScheme = confirm('Do You Want to fill scheme textbox?');
                //    if (fillScheme) {{
                //        document.getElementById('{selectedOption.ClientID}').value = 'scheme';
                //    }}
                //}}
                //__doPostBack('{selectedOption.UniqueID}', '');";

                //    ClientScript.RegisterStartupScript(this.GetType(), "FillTextBox", script, true);


            }
        }

        protected void btnfindSchemeButton_Click(object sender, EventArgs e)
        {
            SchemeSearchPanel2.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowSchemeSearchPanel2", "document.getElementById('" + SchemeSearchPanel2.ClientID + "').style.display='block';", true);
        }

        protected void btnSearchScheme_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearchScheme.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                FillSchemeGrid2(searchTerm);
            }

            SchemeSearchPanel2.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepPanelOpen", "document.getElementById('" + SchemeSearchPanel2.ClientID + "').style.display='block';", true);
        }

        protected void SchemeGrid2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            UpdatePanel2.Update();

            if (e.CommandName == "SelectScheme")
            {
                // Get the reference of the clicked button
                Button btnSelect = (Button)e.CommandSource;

                // Retrieve the row that contains the button clicked
                GridViewRow row = (GridViewRow)btnSelect.NamingContainer;


                string schemeCode = ((Label)row.FindControl("lblSchemeCode2")).Text;
                string schemeName = ((Label)row.FindControl("lblSchemeName2")).Text;

                string switchflag = SwitchChebox.Checked ? "1" : "0";


                if (switchflag == "1")
                {
                    switchSchemeName.Text = schemeName + ";;" + schemeCode;
                }
                else
                {
                    SchemeName.Text = schemeName + ";;" + schemeCode;
                }


                amcSelect.SelectedItem.Text = ((Label)row.FindControl("lblAMCName2")).Text;

                SchemeSearchPanel2.Style["display"] = "none";
            }
        }

        private void FillSchemeGrid2(string schemeCode)
        {
            DataTable dt = new MfPunchingController().GetSchemeDetails(schemeCode);
            SchemeGrid2.DataSource = dt;
            SchemeGrid2.DataBind();
        }

        // Validate PAN input (e.g. 'AAAAA9999A')
        private bool ValidatePanInput(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan))
                return false;  // Return false for empty or null inputs

            string pattern = @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$";
            return Regex.IsMatch(pan.Trim(), pattern);  // Use Trim to remove any extra spaces
        }


        // Validate Mobile input (must be 10 digits, starting with 6-9)
        private bool ValidateMobileInput(string mobile)
        {
            string countryId = hdncountryid.Value ?? ""; // Fetch country ID from HiddenField

            if (string.IsNullOrWhiteSpace(mobile))
                return true;

            string pattern = countryId == "1"
                ? @"^[6-9]\d{9}$"  // India: 10-digit number starting with 6-9
                : @"^\d+$";         // International: Any sequence of digits (no length restriction)

            return Regex.IsMatch(mobile, pattern);
        }

        // Validate PIN input based on country
        private bool ValidatePinInput(string pin)
        {
            string countryId = hdncountryid.Value ?? ""; // Fetch country ID from HiddenField

            if (string.IsNullOrWhiteSpace(pin))
                return true;

            string pattern = countryId == "1"
                ? @"^\d{6}$"       // India: Exactly 6 numeric digits
                : @"^[a-zA-Z0-9]{1,10}$"; // International: Alphanumeric, up to 10 characters

            return Regex.IsMatch(pin, pattern);
        }


        // Validate Aadhaar input (must be exactly 12 digits)
        private bool ValidateAadhaarInput(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar))
                return true;

            string pattern = @"^\d{12}$";
            return Regex.IsMatch(aadhaar, pattern);
        }

        // Validate Email input
        private bool ValidateEmailInput(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, pattern);
        }

        protected void btnPanelAddressUpdate_Click(object sender, EventArgs e)
        {
            string loggedInUser = Session["LoginId"]?.ToString();

            string invcd = Invcdtxt.Text.Trim();

            if (string.IsNullOrEmpty(invcd))
            {
                // Trigger JavaScript alert and show the modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please load the investor details first.'); openAddressModal();", true);
                return;
            }

            try
            {
                // Validate and fetch input values
                decimal invCode = decimal.Parse(Invcdtxt.Text); // Ensure invCode is numeric
                string address1 = string.IsNullOrWhiteSpace(AddTextAddress1.Text) ? null : AddTextAddress1.Text;
                string address2 = string.IsNullOrWhiteSpace(AddTextAddress2.Text) ? null : AddTextAddress2.Text;
                string cityId = DropDownCity.SelectedValue != "0" ? DropDownCity.SelectedValue : null;
                string stateId = DropDownState.SelectedValue != "0" ? DropDownState.SelectedValue : null;
                string pincode = string.IsNullOrWhiteSpace(TextPin.Text) ? null : TextPin.Text;
                // string mobile = string.IsNullOrWhiteSpace(TextMobile.Text) ? null : TextMobile.Text;
                string pan = string.IsNullOrWhiteSpace(TextPan.Text) ? null : TextPan.Text;
                string aadhar = string.IsNullOrWhiteSpace(TextAadhar.Text) ? null : TextAadhar.Text;
                decimal? mobile = string.IsNullOrWhiteSpace(TextMobile.Text)
                    ? (decimal?)null : decimal.Parse(TextMobile.Text);

                if (!string.IsNullOrWhiteSpace(pan) && !ValidatePanInput(pan))
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage",
     "alert('Invalid PAN format.'); openAddressModal();", true);

                    return;
                }

                if (mobile.HasValue && !ValidateMobileInput(TextMobile.Text))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid Mobile number format.'); openAddressModal();", true);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(pincode) && !ValidatePinInput(pincode))
                {
                    // Trigger JavaScript alert and show the modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid PIN code format.'); openAddressModal();", true);
                    return;
                }

                DateTime? dob = null;
                if (DateTime.TryParseExact(TextDob.Text, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDOB))
                {
                    dob = parsedDOB;
                }
                else
                {
                    // Trigger JavaScript alert and show the modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage",
                        "alert('Invalid DOB format.'); openAddressModal();", true);
                    return;
                }

                // Call the controller method to update details
                MfPunchingController controller = new MfPunchingController();
                controller.UpdateInvestorDetails(
                    invCode, aadhar, pan, mobile, null,
                    address1, address2, pincode, cityId, stateId, dob, loggedInUser
                );


                // After successfully saving the changes
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeModal",
                    "$('#addresModal').modal('hide'); alert('Address details updated successfully');", true);
                return;
            }
            catch (OracleException oracleEx)
            {
                string currentOrEx = oracleEx.Message;
                ShowAlert(currentOrEx);
            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
            }
        }

        public static string ReduceRepeatedValue(string input)
        {
            if (!string.IsNullOrWhiteSpace(input) && input.Contains(","))
            {
                input = input.Replace(",", "").Trim();
            }

            if (string.IsNullOrWhiteSpace(input))
                return input;

            int len = input.Length;

          


            for (int i = 1; i <= len / 2; i++)
            {
                string part = input.Substring(0, i);
                int repeatCount = len / i;
                string rebuilt = string.Concat(Enumerable.Repeat(part, repeatCount));

                // If rebuilt length matches and equals input — it's a full repeat
                if (rebuilt.Length == len && rebuilt == input)
                {
                    return part;
                }

                // OR: input starts with a repeated pattern multiple times
                if (input.StartsWith(part) && input.Replace(part, "") == "")
                {
                    return part;
                }
            }

            return input;
        }

        // update investor data new , up inv data new, up_inv_data, inv_add_up
        protected void btnPanel2Update_Click(object sender, EventArgs e)
        {
            #region Fetch address pannel data 
            string aadhar_temp = txtAadharADD.Text;
            string pan_temp = txtPanADD.Text;
            string email_temp = txtEmailADD.Text;
            string address1_temp = txtAddressADD.Text;
            string address2_temp = txtAddressADD2.Text;
            string pincode_temp = txtPinADD.Text;
            string cityId_temp = ddlCityADD.SelectedValue != "0" ? ddlCityADD.SelectedValue : null;
            string stateId_temp = ddlStateADD.SelectedValue != "0" ? ddlStateADD.SelectedValue : null;
            string mobileStr_temp = txtMobileADD.Text;
            string dob_temp = txtDOBADD.Text;

            #endregion

            #region Filter address pannel data
            string aadhar = ReduceRepeatedValue(txtAadharADD.Text);
            string pan = ReduceRepeatedValue(txtPanADD.Text);
            string email = ReduceRepeatedValue(txtEmailADD.Text);
            string address1 = ReduceRepeatedValue(txtAddressADD.Text);
            string address2 = ReduceRepeatedValue(txtAddressADD2.Text);
            string pincode = ReduceRepeatedValue(txtPinADD.Text);
            string cityId = ddlCityADD.SelectedValue != "0" ? ddlCityADD.SelectedValue : null;
            string stateId = ddlStateADD.SelectedValue != "0" ? ddlStateADD.SelectedValue : null;
            string mobileStr = ReduceRepeatedValue(txtMobileADD.Text);
            string dob = ReduceRepeatedValue(txtDOBADD.Text);
            #endregion

            #region Set filterd data to the address pannel


            txtAadharADD.Text = aadhar;
            txtPanADD.Text = pan;
            txtEmailADD.Text = email;
            txtAddressADD.Text = address1;
            txtAddressADD2.Text = address2;
            txtPinADD.Text = pincode;
            txtMobileADD.Text = mobileStr;
            txtDOBADD.Text = dob;

            #endregion

            //ddlStateADD.SelectedValue = stateId;
            //ddlCityADD.SelectedValue = cityId;


            string invcd = invcode.Text.Trim();
            string bscod = businessCode.Text.Trim();

            if (string.IsNullOrEmpty(invcd) && string.IsNullOrEmpty(bscod))
            {
                GetSetInvAddressData(invcd, true, true);
                pc.ShowAlert(this, "First load the investor details to update.");
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "showPopup3();", true);
                return;
            }

            try
            {
                PsmController pc_inv_add_up = new PsmController();
                var result = pc_inv_add_up.PSMM_INV_ADD_UP_UP_DATA(invcd, mobileStr, pan, email, aadhar, address1, address2, pincode, cityId, stateId, dob);

                if(result.Success && result.Message.StartsWith("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    pc.ShowAlert(this, result.Message);
                    GetSetInvAddressData(invcd, true, true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "showPopup3();", true);
                    pc.ShowAlert(this, "Update failed: " + (result.Message ?? "No message received."));
                    return;
                }

                hdnPopupVisible1.Value = "false";

                //ClientScript.RegisterStartupScript(this.GetType(), "closePopup", "hidePopup3();", true);
            }
            catch (OracleException oracleEx)
            {
                string message = Regex.Replace(oracleEx.Message, @"ORA-\d{5}:\s*", "");
                pc.ShowAlert(this, message);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "showPopup3();", true);
                GetSetInvAddressData(invcd, true, true);
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "popup", "showPopup3();", true);
                GetSetInvAddressData(invcd, true, true);
                return ;
            }
        }


        protected void btnPanel2Update_Click_oldtill230925(object sender, EventArgs e)
        {
            //UpdatePanelDTNumber.Update();
            // UpdatePanelFillByDT.Update();



            txtAddressADD.Text = txtAddressADD.Text.Replace(",", "").Trim();
            txtAddressADD2.Text = txtAddressADD2.Text.Replace(",", "").Trim();
            txtPinADD.Text = txtPinADD.Text.Replace(",", "").Trim();
            txtMobileADD.Text = txtMobileADD.Text.Replace(",", "").Trim();
            txtPanADD.Text = txtPanADD.Text.Replace(",", "").Trim();
            txtEmailADD.Text = txtEmailADD.Text.Replace(",", "").Trim();
            txtAadharADD.Text = txtAadharADD.Text.Replace(",", "").Trim();
            txtDOBADD.Text = txtDOBADD.Text.Replace(",", "").Trim();
            hdncountryid.Value = hdncountryid.Value.Replace(",", "").Trim();

            string loggedInUser = Session["LoginId"]?.ToString();

            string roleId = Session["roleId"]?.ToString();

            if (roleId != "212")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('You are not Authorized to update details.'); showPopup3();", true);
                return;
            }

            string invcd = invcode.Text.Trim();
            string bscod = businessCode.Text.Trim();

            if (string.IsNullOrEmpty(invcd) && string.IsNullOrEmpty(bscod))
            {
                // Trigger JavaScript alert and show the panel
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First Load the Investor details to update investor details.'); showPopup3();", true);
                return;
            }

            try
            {
                // Validate and fetch input values
                string aadhar = txtAadharADD.Text;
                decimal invCode = decimal.Parse(invcode.Text); // Ensure invCode is numeric
                                                               // string aadhar = string.IsNullOrWhiteSpace(txtAadharADD.Text) ? null : txtAadharADD.Text;
                string pan = txtPanADD.Text;

                if (!string.IsNullOrEmpty(pan) && !ValidatePanInput(pan))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid PAN format.'); showPopup3();", true);
                    return;
                }

                decimal? mobile = string.IsNullOrWhiteSpace(txtMobileADD.Text)
                    ? (decimal?)null : decimal.Parse(txtMobileADD.Text);

                if (mobile.HasValue && !ValidateMobileInput(txtMobileADD.Text))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid Mobile number format.'); showPopup3();", true);
                    return;
                }

                string email = string.IsNullOrWhiteSpace(txtEmailADD.Text) ? null : txtEmailADD.Text;
                if (!string.IsNullOrWhiteSpace(email) && !ValidateEmailInput(email))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid Email format.'); showPopup3();", true);
                    return;
                }

                string address1 = string.IsNullOrWhiteSpace(txtAddressADD.Text) ? null : txtAddressADD.Text;
                string address2 = string.IsNullOrWhiteSpace(txtAddressADD2.Text) ? null : txtAddressADD2.Text;
                string pincode = string.IsNullOrWhiteSpace(txtPinADD.Text) ? null : txtPinADD.Text;
                if (!string.IsNullOrWhiteSpace(pincode) && !ValidatePinInput(pincode))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid PIN code format.'); showPopup3();", true);
                    return;
                }

                string cityId = ddlCityADD.SelectedValue != "0" ? ddlCityADD.SelectedValue : null;
                string stateId = ddlStateADD.SelectedValue != "0" ? ddlStateADD.SelectedValue : null;

                DateTime? dob = null;
                if (DateTime.TryParseExact(txtDOBADD.Text, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDOB))
                {
                    // If successfully parsed, assign to the dob variable
                    dob = parsedDOB;
                }
                else
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage",
                        "alert('Invalid DOB format.'); showPopup3();", true);
                    return;
                }

                // Call the controller method to update details
                MfPunchingController controller = new MfPunchingController();
                controller.UpdateInvestorDetails(
                    invCode, aadhar, pan, mobile, email,
                    address1, address2, pincode, cityId, stateId, dob, loggedInUser
                );

                ClientScript.RegisterStartupScript(this.GetType(), "HidePopupScript", "hidePopup3();", true);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Investor details updated successfully');", true);
                hdnPopupVisible1.Value = "false"; // Hide the popup after successful update
            }
            catch (OracleException oracleEx)
            {
                string currenctOrEx = Regex.Replace(oracleEx.Message, @"ORA-\d{5}:\s*", "");
                ShowAlert(currenctOrEx);
                ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "showPopup3();", true);
            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "showPopup3();", true);
            }


        }

   

        private void ShowAlert(string message)
        {
            // Handle null message by using a default value
            message = message ?? "No message provided";

            // Sanitize the message to handle special characters and prevent JavaScript errors
            string sanitizedMessage = message.Replace("'", "\\'").Replace("\n", "\\n");

            // Register the alert script
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "alert",
                "alert('" + sanitizedMessage.Replace("'", "\\'") + "');",
                true
            );

        }


        protected void btnPanel3Update_Click(object sender, EventArgs e)
        {
            string loggedInUser = Session["LoginId"]?.ToString();

            string roleId = Session["roleId"]?.ToString();

            if (roleId != "212")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('You are not Authorized to update details.'); showPopup4();", true);
                return;
            }

            string invcd = txtClientCode.Text.Trim();
            string bscod = businessCodeV.Text.Trim();

            if (string.IsNullOrEmpty(invcd) && string.IsNullOrEmpty(bscod))
            {
                // Trigger JavaScript alert
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First Load the Investor details to update investor details.');", true);
                return;
            }

            try
            {
                // Validate and fetch input values
                decimal invCode = decimal.Parse(txtClientCode.Text); // Ensure invCode is numeric
                string aadhar = string.IsNullOrWhiteSpace(txtAadharADD3.Text) ? null : txtAadharADD3.Text;
                string pan = string.IsNullOrWhiteSpace(txtPanADD3.Text) ? null : txtPanADD3.Text;

                if (!ValidatePanInput(pan))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid PAN format.'); showPopup4();", true);
                    return;
                }

                decimal? mobile = string.IsNullOrWhiteSpace(txtMobileADD3.Text)
                    ? (decimal?)null : decimal.Parse(txtMobileADD3.Text);

                if (mobile.HasValue && !ValidateMobileInput(txtMobileADD3.Text))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid Mobile number format.'); showPopup4();", true);
                    return;
                }

                string email = string.IsNullOrWhiteSpace(txtEmailADD3.Text) ? null : txtEmailADD3.Text;
                if (!string.IsNullOrWhiteSpace(email) && !ValidateEmailInput(email))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid Email format.'); showPopup4();", true);
                    return;
                }

                string address1 = string.IsNullOrWhiteSpace(txtAddressADD3.Text) ? null : txtAddressADD3.Text;
                string address2 = string.IsNullOrWhiteSpace(txtAddressADD23.Text) ? null : txtAddressADD23.Text;
                string pincode = string.IsNullOrWhiteSpace(txtPinADD3.Text) ? null : txtPinADD3.Text;

                if (!string.IsNullOrWhiteSpace(pincode) && !ValidatePinInput(pincode))
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid PIN code format.'); showPopup4();", true);
                    return;
                }

                string cityId = ddlCityADD3.SelectedValue != "0" ? ddlCityADD3.SelectedValue : null;
                string stateId = ddlStateADD3.SelectedValue != "0" ? ddlStateADD3.SelectedValue : null;

                DateTime? dob = null;
                if (DateTime.TryParseExact(txtDOBADD3.Text, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDOB))
                {
                    // If successfully parsed, assign to the dob variable
                    dob = parsedDOB;
                }
                else
                {
                    // Trigger JavaScript alert and show the panel
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Invalid DOB format.'); showPopup4();", true);
                    return;
                }

                // Call the controller method to update details
                MfPunchingController controller = new MfPunchingController();
                controller.UpdateInvestorDetails(
                    invCode, aadhar, pan, mobile, email,
                    address1, address2, pincode, cityId, stateId, dob, loggedInUser
                );

                // On success, hide the popup and show success message
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "alert('Investor details updated successfully'); hidePopup4();", true);
                hdnPopupVisible2.Value = "false"; // Hide the popup after successful update


            }
            catch (OracleException oracleEx)
            {
                string currenctOrEx = oracleEx.Message;
                ShowAlert(currenctOrEx);
            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
            }
        }



        //protected void btnPanel3Update_Click(object sender, EventArgs e)
        //{
        //    string loggedInUser = Session["LoginId"]?.ToString();

        //    string invcd = txtClientCode.Text.Trim();
        //    string bscod = businessCodeV.Text.Trim();

        //    if (string.IsNullOrEmpty(invcd) && string.IsNullOrEmpty(bscod))
        //    {
        //        // Trigger JavaScript alert
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('First Load the Investor details to update investor details.');", true);
        //        return;
        //    }


        //    try
        //    {
        //        // Validate and fetch input values
        //        decimal invCode = decimal.Parse(txtClientCode.Text); // Ensure invCode is numeric
        //        string aadhar = string.IsNullOrWhiteSpace(txtAadharADD3.Text) ? null : txtAadharADD3.Text;
        //        string pan = string.IsNullOrWhiteSpace(txtPanADD3.Text) ? null : txtPanADD3.Text;

        //        decimal? mobile = string.IsNullOrWhiteSpace(txtMobileADD3.Text)
        //            ? (decimal?)null : decimal.Parse(txtMobileADD3.Text);

        //        string email = string.IsNullOrWhiteSpace(txtEmailADD3.Text) ? null : txtEmailADD3.Text;
        //        string address1 = string.IsNullOrWhiteSpace(txtAddressADD3.Text) ? null : txtAddressADD3.Text;
        //        string address2 = string.IsNullOrWhiteSpace(txtAddressADD23.Text) ? null : txtAddressADD23.Text;
        //        string pincode = string.IsNullOrWhiteSpace(txtPinADD3.Text) ? null : txtPinADD3.Text;

        //        string cityId = ddlCityADD3.SelectedValue != "0" ? ddlCityADD3.SelectedValue : null;
        //        string stateId = ddlStateADD3.SelectedValue != "0" ? ddlStateADD3.SelectedValue : null;

        //        DateTime? dob = null;
        //        if (DateTime.TryParseExact(txtDOBADD3.Text, "dd/MM/yyyy",
        //System.Globalization.CultureInfo.InvariantCulture,
        //System.Globalization.DateTimeStyles.None,
        //out DateTime parsedDOB))
        //        {
        //            // If successfully parsed, assign to the dob variable
        //            dob = parsedDOB;
        //        }
        //        else
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
        //     "alert('Invalid DOB format.');", true);

        //        }

        //        // Call the controller method to update details
        //        MfPunchingController controller = new MfPunchingController();
        //        controller.UpdateInvestorDetails(
        //            invCode, aadhar, pan, mobile, email,
        //            address1, address2, pincode, cityId, stateId, dob, loggedInUser
        //        );

        //        // ClientScript.RegisterStartupScript(this.GetType(), "HidePopupScript", "hidePopup3();", true);



        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
        //            "alert('Investor details updated successfully');", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Error alert
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage",
        //            $"alert('Error: {ex.Message}');", true);
        //    }
        //}

        protected void ShowMessage(string message)
        {
            string script = $"alert('{message}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowMessageScript", script, true);
        }


        public void LoadDuplicateTransactions(string clientCode, string schemeCode, decimal premAmount, Panel popupDuplicateCheck, GridView gvDuplicateTransactions)
        {
            string connStr = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();

                string query = @"
            SELECT NVL(inv_code, 0) AS ClientCode, 
                   NVL(i.investor_name, 0) AS ClientName,
                   NVL(mobile, 0) AS Mobile, 
                   NVL(tr_date, TO_DATE('01-01-1900', 'dd-MM-yyyy')) AS ARDate, 
                   NVL(b.sch_name, 0) AS SchemeName, 
                   NVL(b.sch_code, 0) AS SchemeCode, 
                   NVL(amount, 0) AS Amount, 
                   NVL(tran_code, 0) AS ARNumber, 
                   NVL(doc_id, 0) AS DTNumber, 
                   NVL(cheque_no, 0) AS ChequeNumber 
            FROM transaction_mf_temp1 a
            JOIN scheme_info b ON a.sch_code = b.sch_code
            JOIN investor_master i ON i.inv_code = a.client_code
            WHERE A.DOC_ID IS NOT NULL  
                AND CLIENT_CODE = :clientCode  
                AND tr_date > SYSDATE - 90  
                AND b.sch_code = :schemeCode
                AND amount BETWEEN :amountMin AND :amountMax";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add("clientCode", OracleDbType.Varchar2).Value = clientCode;
                    cmd.Parameters.Add("schemeCode", OracleDbType.Varchar2).Value = schemeCode;
                    cmd.Parameters.Add("amountMin", OracleDbType.Decimal).Value = premAmount - 100;
                    cmd.Parameters.Add("amountMax", OracleDbType.Decimal).Value = premAmount + 100;

                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvDuplicateTransactions.DataSource = dt;
                        gvDuplicateTransactions.DataBind();
                        popupDuplicate.Visible = true;
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowDPopup", "showDPopup();", true);
                    }
                    else
                    {
                        popupDuplicate.Visible = false;
                    }
                }
            }
        }


        private bool CheckForDuplicateTransactions(string clientCode, string schemeCode, decimal premAmount)
        {
            bool hasDuplicate = false;
            string connStr = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connStr))
            {
                con.Open();
                string query = @"
            SELECT COUNT(*)
            FROM transaction_mf_temp1 a
            JOIN scheme_info b ON a.sch_code = b.sch_code
            WHERE (asa <> 'C' OR asa IS NULL)
              AND A.DOC_ID IS NOT NULL
              AND CLIENT_CODE = :clientCode
              AND tr_date > SYSDATE - 90
              AND b.sch_code = :schemeCode
              AND amount BETWEEN :amountMin AND :amountMax";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    cmd.Parameters.Add("clientCode", OracleDbType.Varchar2).Value = clientCode;
                    cmd.Parameters.Add("schemeCode", OracleDbType.Varchar2).Value = schemeCode;
                    cmd.Parameters.Add("amountMin", OracleDbType.Decimal).Value = premAmount - 100;
                    cmd.Parameters.Add("amountMax", OracleDbType.Decimal).Value = premAmount + 100;

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    hasDuplicate = (count >= 1);
                }
            }

            return hasDuplicate;
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            // User chose to continue, update the hidden field
            hfContinueTransaction.Value = "1";

            // Proceed with transaction
            Save_Method(); // btnContinue_Click
        }



        #region cmdAddTransaction
        protected void btnAddClick(object sender, EventArgs e)
        {

            MfPunchingController controller = new MfPunchingController();

            string loggedInUser = Session["LoginId"]?.ToString();
            string roleId = Session["roleId"]?.ToString();

            #region Validations
            if (roleId != "212")
            {
                pc.ShowAlert(this, "You are not authorized to punch the transaction");
                return;
            }

            // Validate required fields
            // Assuming 'pann' is a TextBox control
            if (string.IsNullOrWhiteSpace(dtNumberA.Text))
            {
                lblWarning.Text = "Please Select Investor to add";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }


            /* old pan validation 
            string checkinstype = iNSTYPE.Text.ToString();
            if (string.IsNullOrWhiteSpace(pann.Text)  && (checkinstype != "MICROSIP" && checkinstype != "MICRO"))
            {
                lblWarning.Text = "Please Provide Pan Number";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

             string a_Pan1 = pann.Text.Substring(0, Math.Min(10, pann.Text.Length));
             string a_Pan2 = txtPanADD.Text.Substring(0, Math.Min(10, txtPanADD.Text.Length));


            if (a_Pan1.ToUpper() != a_Pan2.ToUpper() && checkinstype != "MICRO" && checkinstype != "MICROSIP")
            {
                lblWarning.Text = "Pan Number does not match Investor's Pan No.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }
            */


            var (isTrue, errorMsg) = SamValidationSave();
            if (!isTrue)
            {
                pc.ShowAlert(this, errorMsg);
                return;

            }


            #region Validaion 2


            if (string.IsNullOrWhiteSpace(amc.SelectedValue))
            {
                lblWarning.Text = "Please select an AMC.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            // Validate Scheme TextBox
            if (string.IsNullOrWhiteSpace(scheme.Text))
            {
                lblWarning.Text = "Please provide a Scheme.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(amountt.Text))
            {
                lblWarning.Text = "Please provide an Amount.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(applicationNo.Text))
            {
                if (applicationNo.Text.Length < 6)
                {
                    lblWarning.Text = "Minimum Length Of App No Should Be Greater or Equal To 6.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
                else if (applicationNo.Text == "000000")
                {
                    lblWarning.Text = "Please Enter A Valid App No.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
            }

            // Validate Business Code
            if (string.IsNullOrWhiteSpace(businessCode.Text))
            {
                lblWarning.Text = "Business Code Cannot Be Left Blank.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            // Validate Investor Name
            if (string.IsNullOrWhiteSpace(accountHolder.Text))
            {
                lblWarning.Text = "Please Fill Investor Name.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            // Validate Client Code
            /*
            if (holderCode.Text.Length < 8)
            {
                lblWarning.Text = "Client Code Cannot Be Left Blank.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }*/

            // Validate ddlSipStp selection
            if (transactionType.SelectedValue == "PURCHASE")
            {
                if (string.IsNullOrWhiteSpace(ddlSipStp.SelectedValue))
                {
                    lblWarning.Text = "Please select a value from SIP/STP.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
            }

            // Check if SIP is selected
            if (ddlSipStp.SelectedValue == "SIP" && transactionType.SelectedValue == "PURCHASE")
            {
                // Validate SIP-specific fields
                if (string.IsNullOrWhiteSpace(txtSIPStartDate.Text))
                {
                    lblWarning.Text = "Please provide a SIP Start Date.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSIPEndDate.Text))
                {
                    lblWarning.Text = "Please provide a SIP End Date.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlFrequency.SelectedValue))
                {
                    lblWarning.Text = "Please select a Frequency.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtInstallmentsNos.Text))
                {
                    lblWarning.Text = "Please provide the number of Installments.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(sipamount.Text))
                {
                    lblWarning.Text = "Please provide a SIP Amount.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
            }

            // Check if STP is selected
            if (ddlSipStp.SelectedValue == "STP")
            {
                //   transactionType.SelectedValue = "SWITCH IN";
                // Validate STP-specific fields
                if (string.IsNullOrWhiteSpace(ddlFrequency.SelectedValue))
                {
                    lblWarning.Text = "Please select a Frequency.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtInstallmentsNos.Text))
                {
                    lblWarning.Text = "Please provide the number of Installments.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSIPEndDate.Text))
                {
                    lblWarning.Text = "Please provide a SIP End Date.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }


                if (ddlSipStp.SelectedValue == "STP")
                {
                    if (string.IsNullOrWhiteSpace(formSwitchScheme.Text))
                    {
                        lblWarning.Text = "Please provide a Form Switch/STP Scheme.";
                        lblWarning.Visible = true;
                        lblWarning.Focus();
                        return;
                    }
                }


            }


            #endregion

            #region Validation 3
            // Validate payment method fields
            string selectedPaymentMethod = null;
            string paymentMode = null;
            string chequeNo = null;
            DateTime? chequeDate = null;

            if (cheque.Checked)
            {
                selectedPaymentMethod = "cheque";
                paymentMode = "C";
                chequeNo = txtChequeNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtChequeDated.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtChequeDated.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (draft.Checked)
            {
                selectedPaymentMethod = "draft";
                paymentMode = "D";
                chequeNo = txtDraftNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtDraftDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtDraftDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (ecs.Checked)
            {
                selectedPaymentMethod = "ecs";
                paymentMode = "E";
                chequeNo = txtEcsReference.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtEcsDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtEcsDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (cash.Checked)
            {
                selectedPaymentMethod = "cash";
                paymentMode = "H";
                chequeNo = txtCashAmount.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtCashDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtCashDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (others.Checked)
            {
                selectedPaymentMethod = "others";
                paymentMode = "R";
                chequeNo = txtOthersReference.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtOthersDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtOthersDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (rtgs.Checked)
            {
                selectedPaymentMethod = "rtgs";
                paymentMode = "U";
                chequeNo = txtRtgsNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtRtgsDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtRtgsDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (neft.Checked)
            {
                selectedPaymentMethod = "neft";
                paymentMode = "B";
                chequeNo = txtNeftNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtNeftDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtNeftDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            if (transactionType.SelectedValue == "PURCHASE")
            {
                if (string.IsNullOrEmpty(selectedPaymentMethod))
                {
                    lblWarning.Text = "Please select a payment method.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }


                switch (selectedPaymentMethod)
                {
                    case "cheque":
                        if (string.IsNullOrWhiteSpace(txtChequeNo.Text) || string.IsNullOrWhiteSpace(txtChequeDated.Text))
                        {
                            lblWarning.Text = "Please fill in all cheque details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "draft":
                        if (string.IsNullOrWhiteSpace(txtDraftNo.Text) || string.IsNullOrWhiteSpace(txtDraftDate.Text))
                        {
                            lblWarning.Text = "Please fill in all draft details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "rtgs":
                        if (string.IsNullOrWhiteSpace(txtRtgsNo.Text) || string.IsNullOrWhiteSpace(txtRtgsDate.Text))
                        {
                            lblWarning.Text = "Please fill in all RTGS details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "neft":
                        if (string.IsNullOrWhiteSpace(txtNeftNo.Text) || string.IsNullOrWhiteSpace(txtNeftDate.Text))
                        {
                            lblWarning.Text = "Please fill in all NEFT details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "ecs":
                        if (string.IsNullOrWhiteSpace(txtEcsReference.Text) || string.IsNullOrWhiteSpace(txtEcsDate.Text))
                        {
                            lblWarning.Text = "Please fill in all ECS details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "cash":
                        if (string.IsNullOrWhiteSpace(txtCashAmount.Text) || string.IsNullOrWhiteSpace(txtCashDate.Text))
                        {
                            lblWarning.Text = "Please fill in all cash payment details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "others":
                        if (string.IsNullOrWhiteSpace(txtOthersReference.Text) || string.IsNullOrWhiteSpace(txtOthersDate.Text))
                        {
                            lblWarning.Text = "Please fill in all other payment details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    default:
                        lblWarning.Text = "Invalid payment method selected.";
                        lblWarning.Visible = true;
                        lblWarning.Focus();
                        return;
                }
            }

            #endregion

            #region Getting Values

            // Handle decimal values with null checks
            decimal sipAmount = string.IsNullOrWhiteSpace(sipamount.Text) ? 0 : decimal.Parse(sipamount.Text);
            string clientCode = invcode.Text;
            string businessRmCode = businessCode.Text;
            //  string loggedUserId = txtLoggedUserId.Text;
            string clientOwner = businessCode.Text;
            string busiBranchCode = branch.Text;
            string panno = pann.Text;
            string mutCode = amc.SelectedValue;
            string schCode = scheme.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

            // Parse DateTime values with null checks
            DateTime trDate = string.IsNullOrWhiteSpace(transactionDate.Text)
       ? DateTime.Now
       : DateTime.TryParseExact(transactionDate.Text, "dd/MM/yyyy",
           System.Globalization.CultureInfo.InvariantCulture,
           System.Globalization.DateTimeStyles.None, out var parsedTrDate)
       ? parsedTrDate
       : DateTime.Now; // Default to now if parsing fails

            string tranType = transactionType.SelectedValue;
            string appNo = applicationNo.Text;

            DateTime sipStartDate = string.IsNullOrWhiteSpace(txtSIPStartDate.Text)
      ? DateTime.Now
      : DateTime.TryParseExact(txtSIPStartDate.Text, "dd/MM/yyyy",
          System.Globalization.CultureInfo.InvariantCulture,
          System.Globalization.DateTimeStyles.None, out var parsedSipStartDate)
      ? parsedSipStartDate
      : DateTime.Now; // Default to now if parsing fails

            string pan = pann.Text;
            string folioNo = folioNoo.Text;
            string switchFolio = formSwitchFolio.Text;

            string switchScheme;
            if (string.IsNullOrWhiteSpace(formSwitchScheme.Text))
            {
                switchScheme = null;
            }
            else
            {
                switchScheme = formSwitchScheme.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

            }

            string bankName = ddlBankName.SelectedItem.ToString();


            // Handle cheque date with null checks
            decimal amount = string.IsNullOrWhiteSpace(amountt.Text) ? 0 : decimal.Parse(amountt.Text);


            string sipType = ddlSipStp.SelectedValue;

            //  string leadName = txtLeadName.Text;
            string sourceCode = invcode.Text.Length >= 8 ? invcode.Text.Substring(0, 8) : invcode.Text;

            string investorName = accountHolder.Text;

            decimal expRate = string.IsNullOrWhiteSpace(txtExpensesPercent.Text) ? 0 : decimal.Parse(txtExpensesPercent.Text);
            decimal expAmount = string.IsNullOrWhiteSpace(txtExpensesRs.Text) ? 0 : decimal.Parse(txtExpensesRs.Text);
            string acHolderCode = holderCode.Text;
            string frequency = ddlFrequency.SelectedValue;

            int installmentsNo = string.IsNullOrWhiteSpace(txtInstallmentsNos.Text) ? 0 : int.Parse(txtInstallmentsNos.Text);

            DateTime timestamp = DateTime.Now; // Current timestamp

            DateTime? sipEndDate = string.IsNullOrWhiteSpace(txtSIPEndDate.Text)
          ? (DateTime?)null
          : DateTime.TryParseExact(txtSIPEndDate.Text, "dd/MM/yyyy",
              System.Globalization.CultureInfo.InvariantCulture,
              System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
          ? parsedDate
          : (DateTime?)null;

            string sipFr = null;
            if (ddlSipStp.SelectedValue == "SIP")
            {

                if (fresh.Checked)
                {
                    sipFr = "F";
                }
                else if (renewal.Checked)
                {
                    sipFr = "R";
                }
            }

            string microflag = null;

            if (ddlSipStp.SelectedValue == "SIP")
            {
                microflag = siptype.SelectedValue;
            }

            string dispatch = null;

            if (regular.Checked)
                dispatch = "R";
            else if (nfo.Checked)
                dispatch = "N";

            string docId = dtNumberA.Text;

            string microInvestment = null;

            microInvestment = iNSTYPE.SelectedValue;


            string cobFlag = chkCOBCase.Checked ? "1" : "0";
            string swpFlag = chkSWPCase.Checked ? "1" : "0";
            string freedomSipFlag = chkFreedomSIP.Checked ? "1" : "0";

            string targetSwitchScheme;
            if (string.IsNullOrWhiteSpace(txtSearchSchemeDetails.Text))
            {
                targetSwitchScheme = null;
            }
            else
            {
                targetSwitchScheme = txtSearchSchemeDetails.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

            }

            #endregion

            // Check if duplicate transactions exist
            bool hasDuplicate = CheckForDuplicateTransactions(clientCode, schCode, amount);

            if (hasDuplicate)
            {
                // Load duplicates and show popup
                LoadDuplicateTransactions(clientCode, schCode, amount, popupDuplicate, gvDuplicateTransactions);

                // Set the hidden field to 0 (waiting for user input)
                hfContinueTransaction.Value = "0";

                return; // Stop execution until user selects an option
            }

            #endregion
            Save_Method(); // btnAddClick

        }
        #endregion


        public bool validateMinorDob(string dobText)
        {

            TextBox guardianPan;
            TextBox guardianName;
            string ddlOccupation = null;

            DateTime dob;
            string dateFormat = "dd/MM/yyyy"; // Expected date format
            CultureInfo provider = CultureInfo.InvariantCulture;

            // Try parsing the input date
            if (DateTime.TryParseExact(dobText, dateFormat, provider, DateTimeStyles.None, out dob))
            {
                int age = (int)((DateTime.Now - dob).TotalDays / 365.25);

                //if (ddlOccupation == "33" || ddlOccupation == "1" || ddlOccupation == "12" || ddlOccupation == "32")
                //{
                //    return false;
                //}

                if (age < 18)
                {
                    return false;
                }

            }

            return true;
        }


        public static string SplitSchemeFromDoubleColon(string stringValue, bool getValue = true)
        {
            // Axis ELSS Tax Saver Fund - Growth;;AX012
            if (string.IsNullOrEmpty(stringValue))
                return string.Empty;

            string[] parts = stringValue.Split(new string[] { ";;" }, StringSplitOptions.None);

            if (parts.Length != 2)
                return string.Empty; // Or handle error

            return getValue ? parts[1] : parts[0];
        }



        public (bool isValid, string errorMessage) SamValidationSave()
        {
            string CmbSipStpA = ddlSipStp.Text.ToString();      // REGULAR, SIP, STP
            string CmbSubSipA = siptype.Text.ToString();        // NORMAL, MICROSIP
            string Cmbsubinv = iNSTYPE.SelectedValue.ToString();    // MICRO, NORMAL

            string FreshRenA = fresh.Checked ? fresh.Text.ToUpper() : renewal.Text.ToUpper();       // Fresh, Renewal
            string TranType = transactionType.SelectedValue.ToString();                            // PURCHASE, SWITCH IN

            string TxtPanA = txtPanADD.Text.Length >= 10 ? txtPanADD.Text.Substring(0, 10).ToUpper() : txtPanADD.Text.ToUpper(); // Pan from address popup that opened after dt entered
            string TxtPanVarify = pann.Text.Length >= 10 ? pann.Text.Substring(0, 10).ToUpper() : pann.Text.ToUpper();

            string TxtAmountA = amountt.Text.ToString();
            string TxtSchemeA = scheme.ToString();
            string TxtCloseSch = txtSearchSchemeDetails.ToString();
            string TxtSwitchSchemeA = formSwitchScheme.ToString();

            string MySchCode = SplitSchemeFromDoubleColon(TxtSchemeA, true);
            string MyCloseSchCode = SplitSchemeFromDoubleColon(TxtCloseSch, true);
            string MySwitchScheme = SplitSchemeFromDoubleColon(TxtSwitchSchemeA, true);

            string txtdocID = dtNumberA.Text.ToString();
            string ImSipEndDtA = txtSIPEndDate.Text.ToString();

            #region Validation fron CmdSave_Click

            string vApprovalFlag = PsmController.SqlRetString($"SELECT wealthmaker.fn_check_for_approval_all('{txtdocID}') FROM dual");
            if (vApprovalFlag == "2")
            {
                return (false, "Approval request for this transaction has already been raised.");
            }
            if (vApprovalFlag == "4")
            {
                return (false, "Approval request for this transaction has been rejected by Management.");
            }



            if (CmbSipStpA == "SIP")
            {
                if (string.IsNullOrWhiteSpace(ImSipEndDtA))
                {
                    //return (false, "Please Enter SIP End Date");
                }
                if (string.IsNullOrWhiteSpace(txtSIPStartDate.Text))
                {
                    //return (false, "Please Enter SIP Start Date");
                }
            }

            // set TxtCloseSchCode to MyCloseSchCode if it is not empty

            /* Duplicate Transaction Check
            
            If Label32 <> "" And MySchCode <> "" And TxtAmountA <> "" Then
                If (SqlRet("select count(*) from transaction_mf_temp1 a,scheme_info b where   (asa <> 'C' OR asa IS NULL) and a.sch_code=b.sch_code AND A.DOC_ID IS NOT NULL  and CLIENT_CODE='" & Label32 & "' and tr_date>sysdate-90 and b.sch_code='" & MySchCode & "' and amount between " & TxtAmountA - 100 & " and " & TxtAmountA + 100)) >= 1 Then
                        frmpopupduplicate.formtype = "MF"
                        frmpopupduplicate.prem_amt = TxtAmountA
                        frmpopupduplicate.CCODE = Label32
                        frmpopupduplicate.sch_code = MySchCode
                        frmpopupduplicate.Show
                        frmpopupduplicate.ZOrder 0
                Else
                        save_method
                End If
            Else
                save_method
            End If
            
             */





            #endregion


            string Label32 = invcode.Text.ToString();
            int micropanflag = 0;


            if (!string.IsNullOrWhiteSpace(TxtAmountA))
            {
                if (Cmbsubinv == "MICRO" && decimal.TryParse(TxtAmountA, out decimal amt) && amt < 50000)
                {
                    micropanflag = 1;
                }
            }

            #region Pan validation

            if ((CmbSipStpA == "SIP" && CmbSubSipA == "MICROSIP"))
            {

            }
            // If not SIP+MICROSIP, validate PAN
            else
            {
                if (MySchCode != "OP#10826")
                {
                    if (micropanflag == 0)
                    {
                        if (string.IsNullOrWhiteSpace(TxtPanVarify) || !pc.ValidatePan(TxtPanA))
                        {
                            if (!pc.ValidatePan(TxtPanVarify))
                            {
                                pann.Focus();
                                return (false, "Please Either Enter a Valid PAN Number");
                            }
                        }
                    }
                    // Minor check for codes starting with 3
                    if (!string.IsNullOrWhiteSpace(Label32) && Label32.StartsWith("3"))
                    {
                        DateTime dob = PsmController.SqlRetDate($"SELECT NVL(dob, SYSDATE - 10000) FROM INVESTOR_MASTER WHERE INV_CODE='{Label32}'");
                        bool IsMinor = dob != DateTime.MinValue && (DateTime.Now.Year - dob.Year) <= 18;

                        if (!IsMinor && micropanflag == 0)
                        {
                            string PAN1 = PsmController.SqlRetString($"SELECT UPPER(PAN) FROM investor_master WHERE INV_CODE='{Label32}'");
                            if (!string.IsNullOrWhiteSpace(PAN1) && PAN1 != "0")
                            {
                                if (PAN1 != TxtPanVarify.ToUpper())
                                {
                                    return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                                }
                            }
                        }
                    }

                    // Minor check for codes starting with 4
                    if (!string.IsNullOrWhiteSpace(Label32) && Label32.StartsWith("4"))
                    {
                        DateTime dob = PsmController.SqlRetDate($"SELECT NVL(dob, TO_DATE('01/01/1800','MM/DD/YYYY')) FROM client_test WHERE client_codekyc='{Label32}'");
                        if (dob == DateTime.Parse("01/01/1800"))
                        {
                            return (false, "Please fill Date of Birth for this investor");
                        }

                        string category = PsmController.SqlRetString($"SELECT NVL(investor_code,1) FROM client_master WHERE client_code='{Label32.Substring(0, 8)}'");
                        bool IsMinor = (DateTime.Now.Year - dob.Year) <= 18 && category == "1";

                        if (!IsMinor && micropanflag == 0)
                        {
                            string inv_cd = PsmController.SqlRetString($"SELECT client_codekyc FROM client_test WHERE upper(CLIENT_PAN)='{TxtPanVarify}'");
                            if (!string.IsNullOrWhiteSpace(inv_cd) && inv_cd != "0" && inv_cd != Label32)
                            {
                                return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                            }

                            int AccountPan = PsmController.SqlRetInt($"SELECT NVL(COUNT(*),0) FROM client_test WHERE client_codekyc='{Label32}' AND (upper(CLIENT_PAN)='{TxtPanVarify}' OR CLIENT_PAN IS NULL)");
                            if (AccountPan == 0)
                            {
                                return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                            }
                        }
                        else if (IsMinor && micropanflag == 0)
                        {
                            string PAN1 = PsmController.SqlRetString($"SELECT NVL(upper(g_pan),0) FROM client_test WHERE client_codekyc='{Label32}'");
                            if (!string.IsNullOrWhiteSpace(PAN1) && PAN1 != "0")
                            {
                                if (PAN1 != TxtPanVarify)
                                {
                                    return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                                }
                            }

                            int AccountPan = PsmController.SqlRetInt($"SELECT NVL(COUNT(*),0) FROM client_test WHERE client_codekyc='{Label32}' AND (upper(G_PAN)='{TxtPanVarify}' OR G_PAN IS NULL)");
                            if (AccountPan == 0)
                            {
                                return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(TxtPanA) || !pc.ValidatePan(TxtPanA))
                    {
                        txtPanADD.Text = TxtPanVarify;
                    }
                }
            }

            // Extra validation
            if (CmbSubSipA == "MICROSIP" && string.IsNullOrWhiteSpace(TxtPanVarify))
            {
                return (false, "Please Provide Pan Number");
            }

            #endregion

            return (true, string.Empty);
        }


        private void Save_Method()
        {


            MfPunchingController controller = new MfPunchingController();

            string loggedInUser = pc.currentLoginID();
            string roleId = pc.currentRoleID();
            if (roleId != "212")
            {
                pc.ShowAlert(this, "You are not authorized to punch the transaction");
                return;
            }




            // Validate required fields
            // Assuming 'pann' is a TextBox control
            if (string.IsNullOrWhiteSpace(dtNumberA.Text))
            {
                lblWarning.Text = "Please Select Investor to add";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            string checkinstype = iNSTYPE.Text.ToString();
            if (string.IsNullOrWhiteSpace(pann.Text) && checkinstype != "MICRO")
            {
                lblWarning.Text = "Please Provide Pan Number";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }


            // useing sam validation for pan number

            /* old pan validation
            // Assuming 'txtPanADD' is another TextBox control for comparison
            if (!pann.Text.Substring(0, Math.Min(10, pann.Text.Length))
         .Equals(txtPanADD.Text.Substring(0, Math.Min(10, txtPanADD.Text.Length)), StringComparison.OrdinalIgnoreCase) && checkinstype != "MICRO" && txtPanADD.Text == null)
            {
                lblWarning.Text = "Pan Number does not match Investor's Pan No.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }
            */

            if (string.IsNullOrWhiteSpace(amc.SelectedValue))
            {
                lblWarning.Text = "Please select an AMC.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            // Validate Scheme TextBox
            if (string.IsNullOrWhiteSpace(scheme.Text))
            {
                lblWarning.Text = "Please provide a Scheme.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(amountt.Text))
            {
                lblWarning.Text = "Please provide an Amount.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            if (!string.IsNullOrWhiteSpace(applicationNo.Text))
            {
                if (applicationNo.Text.Length < 6)
                {
                    lblWarning.Text = "Minimum Length Of App No Should Be Greater or Equal To 6.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
                else if (applicationNo.Text == "000000")
                {
                    lblWarning.Text = "Please Enter A Valid App No.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
            }

            // Validate Business Code
            if (string.IsNullOrWhiteSpace(businessCode.Text))
            {
                lblWarning.Text = "Business Code Cannot Be Left Blank.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            // Validate Investor Name
            if (string.IsNullOrWhiteSpace(accountHolder.Text))
            {
                lblWarning.Text = "Please Fill Investor Name.";
                lblWarning.Visible = true;
                lblWarning.Focus();
                return;
            }

            // Validate Client Code
            //if (holderCode.Text.Length < 8)
            //{
            //    lblWarning.Text = "Client Code Cannot Be Left Blank.";
            //    lblWarning.Visible = true;
            //    lblWarning.Focus();
            //    return;
            //}

            // Validate ddlSipStp selection
            if (transactionType.SelectedValue == "PURCHASE")
            {
                if (string.IsNullOrWhiteSpace(ddlSipStp.SelectedValue))
                {
                    lblWarning.Text = "Please select a value from SIP/STP.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
            }

            // Check if SIP is selected
            if (ddlSipStp.SelectedValue == "SIP")
            {
                // Validate SIP-specific fields
                if (string.IsNullOrWhiteSpace(txtSIPStartDate.Text))
                {
                    lblWarning.Text = "Please provide a SIP Start Date.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSIPEndDate.Text))
                {
                    lblWarning.Text = "Please provide a SIP End Date.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlFrequency.SelectedValue))
                {
                    lblWarning.Text = "Please select a Frequency.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtInstallmentsNos.Text))
                {
                    lblWarning.Text = "Please provide the number of Installments.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(sipamount.Text))
                {
                    lblWarning.Text = "Please provide a SIP Amount.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }
            }

            // Check if STP is selected
            if (ddlSipStp.SelectedValue == "STP" && transactionType.SelectedValue == "PURCHASE")
            {
                //   transactionType.SelectedValue = "SWITCH IN";
                // Validate STP-specific fields
                if (string.IsNullOrWhiteSpace(ddlFrequency.SelectedValue))
                {
                    lblWarning.Text = "Please select a Frequency.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtInstallmentsNos.Text))
                {
                    lblWarning.Text = "Please provide the number of Installments.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtSIPEndDate.Text))
                {
                    lblWarning.Text = "Please provide a SIP End Date.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }

                //if (transactionType.SelectedValue == "PURCHASE")
                //{
                //    lblWarning.Text = "Please select SWITCH IN AS TRANSACTION TYPE.";
                //    lblWarning.Visible = true;
                //    lblWarning.Focus();
                //    return;
                //}

                if (ddlSipStp.SelectedValue == "STP")
                {
                    if (string.IsNullOrWhiteSpace(formSwitchScheme.Text))
                    {
                        lblWarning.Text = "Please provide a Form Switch/STP Scheme.";
                        lblWarning.Visible = true;
                        lblWarning.Focus();
                        return;
                    }
                }


            }

            // Proceed with further logic if validation passes
            // Your logic to handle search based on SIP/STP, frequency, installments, and amounts goes here

            // Validate payment method fields
            string selectedPaymentMethod = null;
            string paymentMode = null;
            string chequeNo = null;
            DateTime? chequeDate = null;

            if (cheque.Checked)
            {
                selectedPaymentMethod = "cheque";
                paymentMode = "C";
                chequeNo = txtChequeNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtChequeDated.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtChequeDated.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (draft.Checked)
            {
                selectedPaymentMethod = "draft";
                paymentMode = "D";
                chequeNo = txtDraftNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtDraftDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtDraftDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (ecs.Checked)
            {
                selectedPaymentMethod = "ecs";
                paymentMode = "E";
                chequeNo = txtEcsReference.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtEcsDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtEcsDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (cash.Checked)
            {
                selectedPaymentMethod = "cash";
                paymentMode = "H";
                chequeNo = txtCashAmount.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtCashDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtCashDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (others.Checked)
            {
                selectedPaymentMethod = "others";
                paymentMode = "R";
                chequeNo = txtOthersReference.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtOthersDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtOthersDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (rtgs.Checked)
            {
                selectedPaymentMethod = "rtgs";
                paymentMode = "U";
                chequeNo = txtRtgsNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtRtgsDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtRtgsDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (neft.Checked)
            {
                selectedPaymentMethod = "neft";
                paymentMode = "B";
                chequeNo = txtNeftNo.Text;
                chequeDate = string.IsNullOrWhiteSpace(txtNeftDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(txtNeftDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            if (transactionType.SelectedValue == "PURCHASE")
            {
                if (string.IsNullOrEmpty(selectedPaymentMethod))
                {
                    lblWarning.Text = "Please select a payment method.";
                    lblWarning.Visible = true;
                    lblWarning.Focus();
                    return;
                }


                switch (selectedPaymentMethod)
                {
                    case "cheque":
                        if (string.IsNullOrWhiteSpace(txtChequeNo.Text) || string.IsNullOrWhiteSpace(txtChequeDated.Text))
                        {
                            lblWarning.Text = "Please fill in all cheque details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "draft":
                        if (string.IsNullOrWhiteSpace(txtDraftNo.Text) || string.IsNullOrWhiteSpace(txtDraftDate.Text))
                        {
                            lblWarning.Text = "Please fill in all draft details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "rtgs":
                        if (string.IsNullOrWhiteSpace(txtRtgsNo.Text) || string.IsNullOrWhiteSpace(txtRtgsDate.Text))
                        {
                            lblWarning.Text = "Please fill in all RTGS details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "neft":
                        if (string.IsNullOrWhiteSpace(txtNeftNo.Text) || string.IsNullOrWhiteSpace(txtNeftDate.Text))
                        {
                            lblWarning.Text = "Please fill in all NEFT details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "ecs":
                        if (string.IsNullOrWhiteSpace(txtEcsReference.Text) || string.IsNullOrWhiteSpace(txtEcsDate.Text))
                        {
                            lblWarning.Text = "Please fill in all ECS details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "cash":
                        if (string.IsNullOrWhiteSpace(txtCashAmount.Text) || string.IsNullOrWhiteSpace(txtCashDate.Text))
                        {
                            lblWarning.Text = "Please fill in all cash payment details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    case "others":
                        if (string.IsNullOrWhiteSpace(txtOthersReference.Text) || string.IsNullOrWhiteSpace(txtOthersDate.Text))
                        {
                            lblWarning.Text = "Please fill in all other payment details.";
                            lblWarning.Visible = true;
                            lblWarning.Focus();
                            return;
                        }
                        break;

                    default:
                        lblWarning.Text = "Invalid payment method selected.";
                        lblWarning.Visible = true;
                        lblWarning.Focus();
                        return;
                }
            }

            // Proceed with adding the transaction
            // Your transaction logic goes here...


            // Add other field validations as needed...

            // Create variables to hold input values
            // string atmFlag = txtAtmFlag.Text;

            // Handle decimal values with null checks
            decimal sipAmount = string.IsNullOrWhiteSpace(sipamount.Text) ? 0 : decimal.Parse(sipamount.Text);
            string clientCode = invcode.Text;
            string businessRmCode = businessCode.Text;
            //  string loggedUserId = txtLoggedUserId.Text;
            string clientOwner = businessCode.Text;
            string busiBranchCode = branch.Text;
            string panno = pann.Text;
            string mutCode = amc.SelectedValue;
            string schCode = scheme.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

            // Parse DateTime values with null checks
            DateTime trDate = string.IsNullOrWhiteSpace(transactionDate.Text)
       ? DateTime.Now
       : DateTime.TryParseExact(transactionDate.Text, "dd/MM/yyyy",
           System.Globalization.CultureInfo.InvariantCulture,
           System.Globalization.DateTimeStyles.None, out var parsedTrDate)
       ? parsedTrDate
       : DateTime.Now; // Default to now if parsing fails

            string tranType = transactionType.SelectedValue;
            string appNo = applicationNo.Text;

            DateTime sipStartDate = string.IsNullOrWhiteSpace(txtSIPStartDate.Text)
      ? DateTime.Now
      : DateTime.TryParseExact(txtSIPStartDate.Text, "dd/MM/yyyy",
          System.Globalization.CultureInfo.InvariantCulture,
          System.Globalization.DateTimeStyles.None, out var parsedSipStartDate)
      ? parsedSipStartDate
      : DateTime.Now; // Default to now if parsing fails

            string pan = pann.Text;
            string folioNo = folioNoo.Text;
            string switchFolio = formSwitchFolio.Text;

            string switchScheme;
            if (string.IsNullOrWhiteSpace(formSwitchScheme.Text))
            {
                switchScheme = null;
            }
            else
            {
                switchScheme = formSwitchScheme.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

            }

            string bankName = ddlBankName.SelectedItem.ToString();


            // Handle cheque date with null checks
            decimal amount = string.IsNullOrWhiteSpace(amountt.Text) ? 0 : decimal.Parse(amountt.Text);


            string sipType = ddlSipStp.SelectedValue;

            //  string leadName = txtLeadName.Text;
            string sourceCode = invcode.Text.Length >= 8 ? invcode.Text.Substring(0, 8) : invcode.Text;

            string investorName = accountHolder.Text;

            decimal expRate = string.IsNullOrWhiteSpace(txtExpensesPercent.Text) ? 0 : decimal.Parse(txtExpensesPercent.Text);
            decimal expAmount = string.IsNullOrWhiteSpace(txtExpensesRs.Text) ? 0 : decimal.Parse(txtExpensesRs.Text);
            string acHolderCode = holderCode.Text;
            string frequency = ddlFrequency.SelectedValue;

            int installmentsNo = string.IsNullOrWhiteSpace(txtInstallmentsNos.Text) ? 0 : int.Parse(txtInstallmentsNos.Text);

            DateTime timestamp = DateTime.Now; // Current timestamp

            DateTime? sipEndDate = string.IsNullOrWhiteSpace(txtSIPEndDate.Text)
          ? (DateTime?)null
          : DateTime.TryParseExact(txtSIPEndDate.Text, "dd/MM/yyyy",
              System.Globalization.CultureInfo.InvariantCulture,
              System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
          ? parsedDate
          : (DateTime?)null;

            string sipFr = null;
            if (ddlSipStp.SelectedValue == "SIP")
            {

                if (fresh.Checked)
                {
                    sipFr = "F";
                }
                else if (renewal.Checked)
                {
                    sipFr = "R";
                }
            }

            string microflag = null;

            if (ddlSipStp.SelectedValue == "SIP")
            {
                microflag = siptype.SelectedValue;
            }

            string dispatch = null;

            if (regular.Checked)
                dispatch = "R";
            else if (nfo.Checked)
                dispatch = "N";

            string docId = dtNumberA.Text;

            string microInvestment = null;

            microInvestment = iNSTYPE.SelectedValue;


            string cobFlag = chkCOBCase.Checked ? "1" : "0";
            string swpFlag = chkSWPCase.Checked ? "1" : "0";
            string freedomSipFlag = chkFreedomSIP.Checked ? "1" : "0";

            string targetSwitchScheme;
            if (string.IsNullOrWhiteSpace(txtSearchSchemeDetails.Text))
            {
                targetSwitchScheme = null;
            }
            else
            {
                targetSwitchScheme = txtSearchSchemeDetails.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

            }

            // Call the controller method to insert the transaction
            var (success, message, ret_tr_code) = controller.AddTransaction(
                 sipAmount, clientCode, businessRmCode, clientOwner,
                busiBranchCode, panno, mutCode, schCode, trDate, tranType, appNo,
                sipStartDate, pan, folioNo, switchFolio, switchScheme, paymentMode,
                bankName, chequeNo, chequeDate, amount, sipType,
                sourceCode, investorName, expRate, expAmount, acHolderCode,
                frequency, installmentsNo, timestamp, sipEndDate, sipFr,
                dispatch, docId, microInvestment, targetSwitchScheme, cobFlag,
                swpFlag, freedomSipFlag, loggedInUser, microflag
            );

            if (success)
            {
                lblWarning.Visible = true;
                lblWarning.Text = "Transaction added successfully. Transaction Code Generated : " + " " + ret_tr_code;
                lblWarning.CssClass = "text-success";  // Optional: Apply CSS class for success
                string script = $@"
                 alert('Transaction added successfully. \n Transaction Code Generated: {ret_tr_code}');
                 window.location.reload();
                 sessionStorage.setItem('selectedSipStp', 'REGULAR'); // Override stored value
                 document.getElementById('{siptype.ClientID}').value = 'REGULAR'; // Set dropdown
                 ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "TransactionAlert", script, true);
                lblWarning.Focus();

                ClearFieldsADD();
                formSwitchFolio.Text = "";
                formSwitchScheme.Text = "";
                siptype.SelectedValue = "REGULAR";
            }
            else
            {
                lblWarning.Visible = true;
                lblWarning.Text = message;  // Display the error message
                lblWarning.CssClass = "text-danger";  // Optional: Apply CSS class for error
                ShowAlert(message);
                lblWarning.Focus();
            }
        }

        PsmController pc = new PsmController();

        public DateTime? ParseDate(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            return null;
        }

        #region btnModClick
        protected void btnModClick(object sender, EventArgs e)
        {
            string roleId = Session["roleId"]?.ToString();

            #region Validation 
            if (roleId != "212" && roleId != "29")
            {
                pc.ShowAlert(this, "You are not authorized to update the transaction");
                return;
            }

            string loggedInUser = Session["LoginId"]?.ToString();

            // Validate Application Number
            if (!string.IsNullOrWhiteSpace(TextBox13.Text))
            {
                if (TextBox13.Text.Length < 6)
                {
                    lblWaning2.Text = "Minimum Length Of App No Should Be Greater or Equal To 6.";
                    lblWaning2.Visible = true;
                    TextBox13.Focus();
                    return;
                }
                else if (TextBox13.Text == "000000")
                {
                    lblWaning2.Text = "Please Enter A Valid App No.";
                    lblWaning2.Visible = true;
                    TextBox13.Focus();
                    return;
                }
            }

            // Validate Business Code
            if (string.IsNullOrWhiteSpace(businessCodeV.Text))
            {
                lblWaning2.Text = "Business Code Cannot Be Left Blank.";
                lblWaning2.Visible = true;
                businessCodeV.Focus();
                return;
            }

            // Validate Investor Name
            if (string.IsNullOrWhiteSpace(accountHolderV.Text))
            {
                lblWaning2.Text = "Please Fill Investor Name.";
                lblWaning2.Visible = true;
                lblWaning2.Focus();
                return;
            }

            // Validate Client Code
            //if (holderCodeV.Text.Length < 8)
            //{
            //    lblWaning2.Text = "Client Code Must be of 8 Digits.";
            //    lblWaning2.Visible = true;
            //    lblWaning2.Focus();
            //    return;
            //}

            // Validate AMC Selection
            if (string.IsNullOrWhiteSpace(amcSelect.SelectedValue))
            {
                lblWaning2.Text = "Select The AMC.";
                lblWaning2.Visible = true;
                lblWaning2.Focus();
                return;
            }



            // Validate Transaction Date
            if (TextBox11.Text == "__/__/____")
            {
                lblWaning2.Text = "Transaction Date Cannot Be Left Blank.";
                lblWaning2.Visible = true;
                lblWaning2.Focus();
                return;
            }

            // Validate payment method fields
            string selectedPaymentMethod = null;
            string paymentMode = null;
            string chequeNo = null;
            DateTime? chequeDate = null;

            string dateStr = TextBox3.Text;

            chequeDate = ParseDate(dateStr);

            // Check the selected payment method using the correct IDs
            if (cheque_view.Checked)
            {
                selectedPaymentMethod = "cheque";
                paymentMode = "C";
                chequeNo = TextBox2.Text; // Corresponds to txtChequeNo
                chequeDate = string.IsNullOrWhiteSpace(dateStr)
                    ? (DateTime?)null
                    : DateTime.ParseExact(dateStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); 
            }
            else if (draft_view.Checked)
            {
                selectedPaymentMethod = "draft";
                paymentMode = "D";
                chequeNo = TextBox4.Text; // Corresponds to txtDraftNo
                chequeDate = string.IsNullOrWhiteSpace(TextBox5.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(TextBox5.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Corresponds to txtDraftDate
            }
            else if (ecs_view.Checked)
            {
                selectedPaymentMethod = "ecs";
                paymentMode = "E";
                chequeNo = TextBox10.Text; // Corresponds to txtEcsReference
                chequeDate = string.IsNullOrWhiteSpace(TextBox17.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(TextBox17.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Corresponds to txtEcsDate
            }
            else if (cash_view.Checked)
            {
                selectedPaymentMethod = "cash";
                paymentMode = "H";
                chequeNo = TextBox18.Text; // Corresponds to txtCashAmount
                chequeDate = string.IsNullOrWhiteSpace(TextBox19.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(TextBox19.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Corresponds to txtCashDate
            }
            else if (others_view.Checked)
            {
                selectedPaymentMethod = "others";
                paymentMode = "R";
                chequeNo = TextBox20.Text; // Corresponds to txtOthersReference
                chequeDate = string.IsNullOrWhiteSpace(TextBox21.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(TextBox21.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Corresponds to txtOthersDate
            }
            else if (rtgs_view.Checked)
            {
                selectedPaymentMethod = "rtgs";
                paymentMode = "U";
                chequeNo = TextBox6.Text; // Corresponds to txtRtgsNo
                chequeDate = string.IsNullOrWhiteSpace(TextBox7.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(TextBox7.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Corresponds to txtRtgsDate
            }
            else if (fund_view.Checked)
            {
                selectedPaymentMethod = "neft"; // Assuming 'fund' is equivalent to 'neft'
                paymentMode = "B";
                chequeNo = TextBox8.Text; // Corresponds to txtNeftNo
                chequeDate = string.IsNullOrWhiteSpace(TextBox9.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(TextBox9.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); // Corresponds to txtNeftDate
            }

            if (string.IsNullOrEmpty(selectedPaymentMethod))
            {
                lblWaning2.Text = "Please select a payment method.";
                lblWaning2.Visible = true;
                return;
            }

            if (DropDownList4.SelectedValue.ToString() == "PURCHASE")
            {
                if (string.IsNullOrEmpty(selectedPaymentMethod))
                {
                    lblWaning2.Text = "Please select a payment method.";
                    lblWaning2.Visible = true;
                    return;
                }

                switch (selectedPaymentMethod)
                {
                    case "cheque":
                        if (string.IsNullOrWhiteSpace(TextBox2.Text) || string.IsNullOrWhiteSpace(TextBox3.Text))
                        {
                            lblWaning2.Text = "Please fill in all cheque details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    case "draft":
                        if (string.IsNullOrWhiteSpace(TextBox4.Text) || string.IsNullOrWhiteSpace(TextBox5.Text))
                        {
                            lblWaning2.Text = "Please fill in all draft details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    case "rtgs":
                        if (string.IsNullOrWhiteSpace(TextBox6.Text) || string.IsNullOrWhiteSpace(TextBox7.Text))
                        {
                            lblWaning2.Text = "Please fill in all RTGS details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    case "neft":
                        if (string.IsNullOrWhiteSpace(TextBox8.Text) || string.IsNullOrWhiteSpace(TextBox9.Text))
                        {
                            lblWaning2.Text = "Please fill in all NEFT details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    case "ecs":
                        if (string.IsNullOrWhiteSpace(TextBox10.Text) || string.IsNullOrWhiteSpace(TextBox17.Text))
                        {
                            lblWaning2.Text = "Please fill in all ECS details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    case "cash":
                        if (string.IsNullOrWhiteSpace(TextBox18.Text) || string.IsNullOrWhiteSpace(TextBox19.Text))
                        {
                            lblWaning2.Text = "Please fill in all cash payment details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    case "others":
                        if (string.IsNullOrWhiteSpace(TextBox20.Text) || string.IsNullOrWhiteSpace(TextBox21.Text))
                        {
                            lblWaning2.Text = "Please fill in all other payment details.";
                            lblWaning2.Visible = true;
                            lblWaning2.Focus();
                            return;
                        }
                        break;

                    default:
                        lblWaning2.Text = "Invalid payment method selected.";
                        lblWaning2.Visible = true;
                        lblWaning2.Focus();
                        return;
                }
            }


            #endregion

            MfPunchingController controller = new MfPunchingController();

            #region Input 
            decimal sipAmount = string.IsNullOrWhiteSpace(sipamount.Text) ? 0 : decimal.Parse(sipamount.Text);
            string clientCode = txtClientCode.Text; // iNVCODEMODIFY;
            string businessRmCode = businessCodeV.Text; // Updated ID
            string branchNameUpdate = BranchNameView.Text;

            string clientOwner = businessCodeV.Text; // Updated ID
            string busiBranchCode = branch.Text; // Ensure 'branch' ID is correctly defined in your UI
            string panno = panNo.Text; // Updated ID
            string mutCode = amcSelect.SelectedValue; // Updated ID
            string schCode = SchemeName.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1]; // This gets the value after ';;'
            string tranCode = trNo.Text;
            // Parse DateTime values with null checks
            DateTime trDate;

            if (!string.IsNullOrWhiteSpace(transactionDate.Text) &&
                DateTime.TryParseExact(transactionDate.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedTrDate))
            {
                trDate = parsedTrDate;
            }
            else
            {
                trDate = DateTime.Now; // Default to now if parsing fails
            }
            string tranType = DropDownList4.SelectedValue; // Updated ID
            string appNo = TextBox13.Text; // Ensure 'applicationNo' ID is correctly defined in your UI

            DateTime sipStartDate;

            if (!string.IsNullOrWhiteSpace(txtSIPStartDateupdate.Text) &&
                DateTime.TryParseExact(txtSIPStartDateupdate.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedSipStartDate))
            {
                sipStartDate = parsedSipStartDate;
            }
            else
            {
                sipStartDate = DateTime.Now; // Default to now if parsing fails
            }
            string pan = panNo.Text; // Updated ID
            string folioNo = TextBox12.Text; // Updated ID
            string switchFolio = formSwitchFolio.Text; // Ensure 'formSwitchFolio' ID is correctly defined in your UI
            string switchScheme = switchSchemeName.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1]; ; // Ensure 'formSwitchScheme' ID is correctly defined in your UI
            string closeSchCode = searchBox.Text;
            string bankNam = bankName.SelectedItem.Text.ToString(); // Updated ID

            // Handle cheque date with null checks
            decimal amount = string.IsNullOrWhiteSpace(TextBox14.Text) ? 0 : decimal.Parse(TextBox14.Text); // Updated ID
            string sipType = sipStp.SelectedValue; // Updated ID

            string sourceCode = invcode.Text;
            string investorName = accountHolderV.Text; // Updated ID

            decimal expRate = string.IsNullOrWhiteSpace(txtExpensesPercent.Text) ? 0 : decimal.Parse(txtExpensesPercent.Text);
            decimal expAmount = string.IsNullOrWhiteSpace(txtExpensesRs.Text) ? 0 : decimal.Parse(txtExpensesRs.Text);
            string acHolderCode = holderCodeV.Text; // Updated ID
            string frequenc = frequency.SelectedValue; // Updated ID

            int installmentsNo = string.IsNullOrWhiteSpace(installmentsNos.Text) ? 0 : int.Parse(installmentsNos.Text); // Updated ID

            DateTime timestamp = DateTime.Now; // Current timestamp

            DateTime? sipEndDate = null;

            if (!string.IsNullOrWhiteSpace(txtSIPEndDateupdate.Text) &&
                DateTime.TryParseExact(txtSIPEndDateupdate.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedSipEndDate))
            {
                sipEndDate = parsedSipEndDate;
            }

            DateTime? tran_date = null;

            if (!string.IsNullOrWhiteSpace(TextBox11.Text) &&
                DateTime.TryParseExact(TextBox11.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedtran_date))
            {
                tran_date = parsedtran_date;
            }

            DateTime? dropDate = null;

            if (!string.IsNullOrWhiteSpace(dropDat.Text) &&
                DateTime.TryParseExact(dropDat.Text, "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime parsedDropDate))
            {
                dropDate = parsedDropDate;
            }

            string sipFr = null;
            if (or99Years.Checked) // Updated ID
            {
                sipFr = "R";
            }
            else if (RadioButton10.Checked) // Ensure this ID is correctly defined
            {
                sipFr = "N";
            }
            else if (!or99Years.Checked && !RadioButton10.Checked) // Ensure this ID is correctly defined
            {
                sipFr = siptype.SelectedValue; // Ensure 'siptype' ID is correctly defined
            }
            string dispatch = null;
            string docId = dtNumberA.Text; // Ensure 'dtNumberA' ID is correctly defined

            string microInvestment = iNSTYPE.SelectedValue; // Ensure 'iNSTYPE' ID is correctly defined
            string targetSwitchScheme = txtSearchDetails.Text; // Ensure 'txtSearchDetails' ID is correctly defined
            string cobFlag = cobCase.Checked ? "1" : "0";
            string swpFlag = swpCase.Checked ? "1" : "0";
            string freedomSipFlag = freedomSip.Checked ? "1" : "0";

            string M_SIP_SUB_TYPE = "";

            if (sipStp.SelectedValue == "SIP")
            {
                M_SIP_SUB_TYPE = ddlM_SIPSubType.SelectedValue;
            }

            #endregion

            // Call the controller method to insert the transaction
            var (success, message) = controller.ModifyTransaction(
                tranCode, closeSchCode, sipAmount, clientCode, businessRmCode, clientOwner,
                 mutCode, busiBranchCode, panno, schCode, trDate, tranType, appNo,
                sipStartDate, pan, folioNo, switchFolio, switchScheme, paymentMode,
                bankNam, chequeNo, chequeDate, amount, sipType,
                sourceCode, investorName, expRate, expAmount, acHolderCode,
                frequenc, installmentsNo, timestamp, sipEndDate, sipFr,
                dispatch, docId, microInvestment, targetSwitchScheme, cobFlag,
                swpFlag, freedomSipFlag, baseTranCode, dropDate, loggedInUser, tran_date, M_SIP_SUB_TYPE
            );

            if (success)
            {
                //lblWaning2.Visible = true;
                //lblWaning2.Text = "Transaction Modified successfully.";
                //lblWaning2.CssClass = "text-success";  // Optional: Apply CSS class for success
                //string script = $"alert('Transaction Modified successfully.');";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "TransactionAlert", script, true);
                //ClearFields();
                //lblWaning2.Focus();

                string script = $@"
        alert('Transaction Modified successfully.');
        window.location.href = window.location.href;
    ";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertAndReload", script, true);
                return;
            }
            else
            {
                lblWaning2.Visible = true;
                lblWaning2.Text = message;  // Display the error message
                lblWaning2.CssClass = "text-danger";  // Optional: Apply CSS class for error
                lblWaning2.Focus();
            }
        }
        #endregion


        [WebMethod]
        public static string SearchInvestorData(MFQuery query)
        {
            var searchResult = new MfPunchingController().MFClientSearch(query);
            return JsonConvert.SerializeObject(new { data = searchResult }, Formatting.None);
        }

        public class MFQuery
        {
            public string Category { get; set; }
            public string BranchCode { get; set; }
            public string CityCode { get; set; }
            public string RmCode { get; set; }
            public string ClientName { get; set; }
            public string InvCode { get; set; }
            public string Phone { get; set; }
            public string PanNo { get; set; }
            public string Mobile { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string SortBy { get; set; }
            public string Email { get; set; }
            public string DOB { get; set; }
            public string AccountCode { get; set; }

        }

        private void BindBranchDataToDropdown()
        {
            var data = new TransactionInvestmentController().GetBranchMaster();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow dataRow in data.Rows)
                {
                    BranchSearch_Dropdown.Items.Add(new ListItem
                    {
                        Text = Convert.ToString(dataRow["branch_name"]),
                        Value = Convert.ToString(dataRow["branch_code"])
                    });
                }
            }
        }

        private void BindCityDataToDropdown()
        {
            var data = new TransactionInvestmentController().GetCityList();

            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    CitySearch_Dropdown.Items.Add(new ListItem() { Text = Convert.ToString(row["city_name"]), Value = Convert.ToString(row["city_id"]) });
                }

            }
        }

        [WebMethod]
        public static string GetRMList(string branchCode)
        {
            List<dynamic> list = new List<dynamic>();
            var data = new TransactionInvestmentController().GetRmList(branchCode);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["rm_name"]), value = Convert.ToString(row["rm_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);

        }

        protected void ddlsipstpInupdate(object Sender, EventArgs e)
        {
            string sipstpvalueUP = sipStp.SelectedValue;
            sipstpupdate(sipstpvalueUP);
        }

        public void sipstpupdate(string sipstpvalue)
        {

            lblM_SIPSubType.Visible = false;
            ddlM_SIPSubType.Visible = false;
            if (sipstpvalue == "REGULAR")
            {
                frequency.Enabled = false;
                installmentsNos.Enabled = false;
                or99Years.Enabled = false;
                txtSIPStartDateupdate.Visible = false;
                txtSIPEndDateupdate.Visible = false;
                lblSIPStartDateupdate.Visible = false;
                lblSIPEndDateupdate.Visible = false;


                // Hide payment method options
                cheque_view.Visible = true;
                draft_view.Visible = true;
                ecs_view.Visible = true;
                cash_view.Visible = true;
                others_view.Visible = true;
                rtgs_view.Visible = true;
                fund_view.Visible = true;

                // Disable and clear all TextBoxes
                TextBox2.Enabled = true;
                TextBox3.Enabled = true;
                TextBox4.Enabled = true;
                TextBox5.Enabled = true;
                TextBox6.Enabled = true;
                TextBox7.Enabled = true;
                TextBox8.Enabled = true;
                TextBox9.Enabled = true;
                TextBox10.Enabled = true;
                TextBox17.Enabled = true;
                TextBox18.Enabled = true;
                TextBox19.Enabled = true;
                TextBox20.Enabled = true;
                TextBox21.Enabled = true;

                freshINupdate.Visible = false;
                renewalInUpdate.Visible = false;

                lblswisch.Visible = false;
                SwitchChebox.Visible = false;

                switchSchemeName.Enabled = false;
                TextBox12.Enabled = false;

                if (DropDownList4.SelectedValue.ToString() == "SWITCH IN")
                {
                    lblswisch.Visible = true;
                    SwitchChebox.Visible = true;

                    switchSchemeName.Enabled = true;
                    TextBox12.Enabled = true;
                }
            }
            else if (sipstpvalue == "SIP")
            {
                lblM_SIPSubType.Visible= true;
                ddlM_SIPSubType.Visible = true;
                txtSIPStartDateupdate.Visible = true;
                txtSIPEndDateupdate.Visible = true;
                lblSIPStartDateupdate.Visible = true;
                lblSIPEndDateupdate.Visible = true;

                frequency.Enabled = true;
                installmentsNos.Enabled = true;
                or99Years.Enabled = true;
                txtSIPEndDateupdate.Enabled = false;

                // Hide payment method options
                cheque_view.Visible = true;
                draft_view.Visible = true;
                ecs_view.Visible = true;
                cash_view.Visible = true;
                others_view.Visible = true;
                rtgs_view.Visible = true;
                fund_view.Visible = true;

                // Disable and clear all TextBoxes
                TextBox2.Enabled = true;
                TextBox3.Enabled = true;
                TextBox4.Enabled = true;
                TextBox5.Enabled = true;
                TextBox6.Enabled = true;
                TextBox7.Enabled = true;
                TextBox8.Enabled = true;
                TextBox9.Enabled = true;
                TextBox10.Enabled = true;
                TextBox17.Enabled = true;
                TextBox18.Enabled = true;
                TextBox19.Enabled = true;
                TextBox20.Enabled = true;
                TextBox21.Enabled = true;

                freshINupdate.Visible = true;
                renewalInUpdate.Visible = true;


                lblswisch.Visible = false;
                SwitchChebox.Visible = false;

                switchSchemeName.Enabled = false;
                TextBox12.Enabled = false;
            }
            else
            {
                // Hide payment method options
                cheque_view.Visible = false;
                draft_view.Visible = false;
                ecs_view.Visible = false;
                cash_view.Visible = false;
                others_view.Visible = false;
                rtgs_view.Visible = false;
                fund_view.Visible = false;

                // Disable and clear all TextBoxes
                TextBox2.Enabled = false;
                TextBox3.Enabled = false;
                TextBox4.Enabled = false;
                TextBox5.Enabled = false;
                TextBox6.Enabled = false;
                TextBox7.Enabled = false;
                TextBox8.Enabled = false;
                TextBox9.Enabled = false;
                TextBox10.Enabled = false;
                TextBox17.Enabled = false;
                TextBox18.Enabled = false;
                TextBox19.Enabled = false;
                TextBox20.Enabled = false;
                TextBox21.Enabled = false;

                freshINupdate.Visible = true;
                renewalInUpdate.Visible = true;


                txtSIPStartDateupdate.Visible = true;
                txtSIPEndDateupdate.Visible = true;
                lblSIPStartDateupdate.Visible = true;
                lblSIPEndDateupdate.Visible = true;

                frequency.Enabled = true;
                installmentsNos.Enabled = true;
                or99Years.Enabled = true;
                txtSIPEndDateupdate.Enabled = false;

                lblswisch.Visible = true;
                SwitchChebox.Visible = true;

                switchSchemeName.Enabled = true;
                TextBox12.Enabled = true;

            }



        }

        #region Sam Edition


        protected void btnInvSearch2ForTr_Click(object sender, EventArgs e)
        {
            try
            {


                string delimiter = "##@@##";
                string[] investorData = hdnInvCodeInModify.Value.Split(new string[] { delimiter }, StringSplitOptions.None);

                if (investorData.Length >= 15)
                {
                    /*
                    string investorName = investorData[0];
                    string invCode = investorData[1];
                    string mobile = investorData[2];
                    string address1 = investorData[3];
                    string address2 = investorData[4];
                    string cityId = investorData[5];
                    string stateId = investorData[6];
                    string pincode = investorData[7];
                    string pan = investorData[8];
                    string aadharNo = investorData[9];
                    string dob = investorData[10];
                    string ahClientCode = investorData[11];
                    string businessCodeInMod = investorData[12];
                    string rmNameInMod = investorData[13];
                    string branchCode = investorData[14];
                    */

                    #region Row data on index;

                    string investorName = investorData[0];
                    string invCode = investorData[1];
                    string mobile = investorData[2];
                    string address1 = investorData[3];
                    string address2 = investorData[4];
                    string cityId = investorData[5];
                    string stateId = investorData[6];
                    string pincode = investorData[7];
                    string pan = investorData[8];
                    string aadharNo = investorData[9];
                    string dob = investorData[10];
                    string ahClientCode = investorData[11];
                    string businessCodeInMod = investorData[12];
                    string rmNameInMod = investorData[13];
                    string branchCode = investorData[14];
                    #endregion

                    string sql = "";

                    if (invCode.Length > 1 && invCode.Substring(1, 1) == "4")
                    {
                        sql = $@"
Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257";
                    }
                    else
                    {
                        sql = $@"
Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL),e.rm_name,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c 
where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ";
                    }

                    sql += $@" AND A.INV_CODE = {invCode}";
                    DataTable dt = pc.ExecuteCurrentQueryMaster(sql, out int rc, out string ex);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // Common fields in both queries
                        string newInvName = row["INVESTOR_name"]?.ToString();
                        string newInvCode = row["INV_code"]?.ToString();
                        string newInvAddress1 = row["address1"]?.ToString();
                        string newInvAddress2 = row["address2"]?.ToString();
                        string newInvCityName = row["city_name"]?.ToString();
                        string newInvBranchName = row["Branch_name"]?.ToString();
                        string newInvclientType = row[6]?.ToString(); // DECODE(a.client_type, ...)
                        string newInvrmName = row["rm_name"]?.ToString();
                        string newInvpan = row["pan"]?.ToString();

                        //txtClientCodeA.Text = invCode,Substring(1,8); // Add: Inv Source Code 
                        txtClientCode.Text = newInvCode;    // view inv code
                        accountHolderV.Text = newInvName;   // view inv name
                                                            //pann.Text = newInvpan;

                        string getFirstKey = invCode.Substring(0, 1);

                        string clientSrc = invCode.Substring(0, 8);

                        if (invCode.Length > 1 && invCode.Substring(0, 1) == "4")
                        {
                            panNo.Text = newInvpan; // view: inv pan

                            DataTable dt1 = pc.ExecuteCurrentQueryMaster(
        $@"SELECT client_code 
       FROM client_test 
       WHERE client_codekyc = {newInvCode}",
        out int rc1,
        out string ie1
    );
                            holderCodeV.Text = dt1.Rows[0]?.ToString();  // view: ah code

                            DataTable dt2 = pc.ExecuteCurrentQueryMaster(
       $@"SELECT payroll_id 
       FROM employee_master 
       WHERE rm_code = (
           SELECT rm_code 
           FROM client_master where client_code = '{clientSrc}'
       )",
       out int rc2,
       out string ie2
    );
                            businessCodeV.Text = dt2.Rows[0]?.ToString();  // view bss code
                        }
                        else
                        {
                            DataTable dt1 = pc.ExecuteCurrentQueryMaster(
         $@"SELECT client_code 
       FROM client_test 
       WHERE client_codekyc = {newInvCode}",
         out int rc1,
         out string ie1
     );
                            holderCodeV.Text = dt1.Rows[0]?.ToString();  // view: ah code

                            DataTable dt2 = pc.ExecuteCurrentQueryMaster(
       $@"SELECT payroll_id 
       FROM employee_master 
       where rm_code=(select rm_code from agent_master where agent_code = '{clientSrc}'
       )",
       out int rc2,
       out string ie2
    );
                            businessCodeV.Text = dt2.Rows[0]?.ToString();  // view bss code
                        }


                    }
                    /*
                     If frmtransactionmf.SSTab1.Tab = 0 Then
                frmtransactionmf.TxtClientCodeA = Mid(msfgClients.TextMatrix(R, 1), 1, 8)
                frmtransactionmf.Label32.Caption = msfgClients.TextMatrix(R, 1)
                frmtransactionmf.txtInvestorA = msfgClients.TextMatrix(R, 0)
                If Mid(msfgClients.TextMatrix(R, 1), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanA = msfgClients.TextMatrix(R, 9)
                    'frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
                    frmtransactionmf.TxtAcHolderA = SqlRet("select client_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
                    If frmtransactionmf.TxtAcHolderA.Text = "" Or Len(frmtransactionmf.TxtAcHolderA.Text) < 6 Then
                        MsgBox "Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same", vbInformation, "Wealthmaker"
                        Me.MousePointer = vbIconPointer
                        Exit Sub
                    End If
                Else
                    frmtransactionmf.TxtPanA = msfgClients.TextMatrix(R, 8)
                    'frmtransactionmf.TxtBusiCodeA = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
                End If
                Me.MousePointer = vbNormal
                Unload Me



                      //txtClientCodeA.Text = invCode,Substring(1,8); // Add: Inv Source Code 
                        invcode.Text = newInvCode;          // Add: Inv Code
                        accountHolder.Text = newInvName;    // Add: Inv Name

                        txtClientCode.Text = newInvCode;    // view inv code
                        accountHolderV.Text = newInvName;   // view inv name
                        panNo.Text = newInvpan; // view: inv pan
                        //pann.Text = newInvpan;

                        if (invCode.Length > 1 && invCode.Substring(1, 1) == "4")
                        {
                            DataTable dt2 = pc.ExecuteCurrentQueryMaster(
        $@"SELECT payroll_id 
           FROM employee_master 
           WHERE rm_code = (
               SELECT rm_code 
               FROM client_master 
               WHERE client_code = '{newInvCode.Substring(1, 8)}'
           )",
        out int rc2,
        out string ie2
    );
                            businessCode.Text = dt2.Rows[0]?.ToString();  // add bss code

                            DataTable dt3 = pc.ExecuteCurrentQueryMaster(
                                $@"SELECT client_code 
           FROM client_test 
           WHERE client_codekyc = '{newInvCode}'",
                                out int rc3,
                                out string ie3
                            );
                            holderCode.Text = dt3.Rows[0]?.ToString();  // add AH code
                            if(string.IsNullOrEmpty(holderCode.Text) ||  holderCode.Text.Length <6)
                            {
                                pc.ShowAlert(this, "Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same");
                                return;
                            }
                        }
                        else
                        {
                            DataTable dt4 = pc.ExecuteCurrentQueryMaster(
       $@"SELECT payroll_id 
           FROM employee_master 
           WHERE rm_code = (
               SELECT rm_code 
               FROM agent_master 
               WHERE agent_code = '{newInvCode.Substring(1, 8)}'
           )",
       out int rc2,
       out string ie2
    );
                            businessCode.Text = dt4.Rows[0]?.ToString();  // add bss code


                        }


                     */
                    /*
    If currentForm.Name = "frmtransactionmf" Then

            ElseIf frmtransactionmf.SSTab1.Tab = 1 Then
                frmtransactionmf.TxtClientCodeM = Mid(msfgClients.TextMatrix(R, 1), 1, 8)
                frmtransactionmf.txtInvestorM = msfgClients.TextMatrix(R, 0)
                If Mid(msfgClients.TextMatrix(R, 1), 1, 1) = "4" Then
                    frmtransactionmf.TxtPanM = msfgClients.TextMatrix(R, 9)
                    frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
                    frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from client_master where client_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
                Else
                    frmtransactionmf.TxtPanA = msfgClients.TextMatrix(R, 8)
                    frmtransactionmf.TxtAcHolderM = SqlRet("select client_code from client_test where client_codekyc=" & msfgClients.TextMatrix(R, 1) & "")
                    frmtransactionmf.TxtBusiCodeM = SqlRet("select payroll_id from employee_master where rm_code=(select rm_code from agent_master where agent_code=" & Mid(msfgClients.TextMatrix(R, 1), 1, 8) & ")")
                End If
                frmtransactionmf.Label42.Caption = msfgClients.TextMatrix(R, 1)
                Me.MousePointer = vbNormal
                Unload Me
            End If
            If frmtransactionmf.SSTab1.Tab = 0 Then
                'Call frmtransactionmf.TxtBusiCodeA_LostFocus
            Else
                'Call frmtransactionmf.TxtBusiCodeM_LostFocus
            End If
            Exit Sub
        End If







Public Sub TxtBusiCodeM_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim Rs_GetBranchCd As ADODB.Recordset
Dim b_cd() As String
    If Len(TxtBusiCodeM.Text) <> 0 Then
        TxtBranchM.Text = ""
        
        If Len(Trim(Label42.Caption)) > 0 Then
            Set Rs_GetBranchCd = New ADODB.Recordset
            Rs_GetBranchCd.open "select i.branch_code from investor_master i where I.inv_code='" & Trim(Label42.Caption) & "'", MyConn, adOpenDynamic, adLockOptimistic
            If Rs_GetBranchCd.EOF = False Then
                branch_code = Rs_GetBranchCd!branch_code
            End If
            Rs_GetBranchCd.Close
        End If
        Set Rs_GetBranchCd = Nothing
        
        
        If AllIndia = False Then
            If branch_code = "" Then
                TxtBusiCodeM.Text = ""
                MsgBox "Please Select Investor First", vbInformation
                Exit Sub
            End If
        End If
        

        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeM.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018) ", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            cmbBusiBranchM.Clear
            cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
            cmbBusiBranchM.ListIndex = 0
            TxtRmNameM.Text = rsEmp(2)
            'MyRmCodeM = rsEmp(3)
            'Label14.Caption = rsEmp(1)
            Label30.Caption = rsEmp(2)
            Call Show_Tr_Branches(Trim(TxtBusiCodeM.Text))
            If valid_rm(branch_code, TxtBusiCodeM) = False Then Exit Sub
            If rsEmp.State = 1 Then rsEmp.Close
            
            
            If Tr_Branches <> "" Then
                cmbBusiBranchM.Clear
                Set rsEmp = New ADODB.Recordset
                rsEmp.open "Select branch_code,branch_name from branch_master b where branch_code in (" & Tr_Branches & ") order by branch_name", MyConn, adOpenForwardOnly
                While Not rsEmp.EOF
                    cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                    If rsEmp(0) = busi_br_tr Then
                        cmbBusiBranchM.ListIndex = i
                    End If
                    rsEmp.MoveNext
                Wend
                'cmbBusiBranchM.ListIndex = 0
            End If
            
            If busi_change_flag = True Then
                set_default_branch branch_code
            End If
            
            If rsEmp.State = 1 Then rsEmp.Close
            Set rsEmp = Nothing
            
        Else
            TxtBranchM.Text = ""
            MyRmCodeM = 0
            TxtBusiCodeM.Text = ""
            TxtRmNameM.Text = ""
            Label14.Caption = ""
            Label30.Caption = ""
        End If
        
        Set rsEmp = Nothing
    End If
End Sub







                     */

                    /*


                    MfPunchingController mfc = new MfPunchingController();
                    string curInvValue = txtClientCode.Text;
                    string curAhValue = holderCodeV.Text;
                    DataTable newInvData = mfc.NewInvDataForTrInvUpdate(curInvValue, "", invCode);


                    IM.MOBILE,
                    IM.INV_CODE,
                    IM.ADDRESS1,
                    IM.ADDRESS2,
                    IM.EMAIL,
                    IM.DOB,
                    IM.PINCODE,
                    IM.CITY_ID, 
                    IM.AADHAR_CARD_NO,
                    IM.PAN


                    if (newInvData != null && newInvData.Rows.Count > 0)
                    {
                        DataRow row = newInvData.Rows[0];

                        string newMobile = row["MOBILE"].ToString();
                        string newInvCode = row["INV_CODE"].ToString();
                        string newAddress1 = row["ADDRESS1"].ToString();
                        string newAddress2 = row["ADDRESS2"].ToString();
                        string newEmail = row["EMAIL"].ToString();
                        string newDob = row["DOB"].ToString();
                        string newPincode = row["PINCODE"].ToString();
                        string newCityId = row["CITY_ID"].ToString();
                        string newAadhar = row["AADHAR_CARD_NO"].ToString();
                        string newPan = row["PAN"].ToString();


                    }

                    */
                }
                else
                {
                    // Handle unexpected format or show error
                }


                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }

        //btnSearchInv2
        protected void btnStoreData_Click(object sender, EventArgs e)
        {
            try
            {
                var result = pc.OpenUrl("../Tree/frm_tree_mf?SEARCH_TYPE=mf_view_inv", "popup");
                if (result.Item1)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDynamicWindow", result.Item2, true);
                }
                else
                {
                    pc.ShowAlert(this, result.Item2);
                    return;
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }

        protected void btnRetriveInvSearchData_Click(object sender, EventArgs e)
        {
            try
            {
                getSetSearchInvInView();
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }
        }


        public void getSetSearchInvInView()
        {
            DataTable filledData = Session["FilledData"] as DataTable;
            if (filledData != null && filledData.Rows.Count > 0)
            {
                DataRow row = filledData.Rows[0];
                string INVESTOR_nameValue = row["INVESTOR_name"]?.ToString();
                string INV_codeValue = row["INV_code"]?.ToString();
                string branch_codeValue = row["branch_code"]?.ToString();
                string address1Value = row["address1"]?.ToString();
                string address2Value = row["address2"]?.ToString();
                string city_nameValue = row["city_name"]?.ToString();
                string Branch_nameValue = row["Branch_name"]?.ToString();
                string client_typeValue = row["client_type"]?.ToString();
                string rm_nameValue = row["rm_name"]?.ToString();
                string bss_codeValue = row["bss_code"]?.ToString();
                string guest_codeValue = row["guest_code"]?.ToString();
                string ah_codeValue = row["ah_code"]?.ToString();
                string panValue = row["pan"]?.ToString();


                txtClientCode.Text = INV_codeValue;       // view: inv code
                accountHolderV.Text = INVESTOR_nameValue; // view: inv name

                if (INV_codeValue.Substring(0, 1) == "4")
                {
                    panNo.Text = panValue;                    // view: inv pan
                    holderCodeV.Text = ah_codeValue;          // view: ah code
                    businessCodeV.Text = bss_codeValue;       // view: bss code
                }
                else
                {
                    //pann.Text = panValue;
                    holderCodeV.Text = ah_codeValue;          // view: ah code
                    businessCodeV.Text = bss_codeValue;       // view: bss code
                }

                string newBranchCode = branch_codeValue;
                BranchNameView.Text = Branch_nameValue;

                if (newBranchCode != null)
                {
                    string sql1 = $@"
SELECT SOURCE,BRANCH_NAME,RM_NAME,RM_CODE 
FROM EMPLOYEE_MASTER E,BRANCH_MASTER B WHERE E.PAYROLL_ID='{newBranchCode}' 
AND E.SOURCE=B.BRANCH_CODE AND (E.TYPE='A' OR E.TYPE IS NULL) AND E.CATEGORY_ID IN(2001,2018)

";
                }

                Session["FilledData"] = null;
                return;
                /*
Public Sub TxtBusiCodeM_LostFocus()
Dim rsEmp As ADODB.Recordset
Dim Rs_GetBranchCd As ADODB.Recordset
Dim b_cd() As String
    If Len(TxtBusiCodeM.Text) <> 0 Then
        TxtBranchM.Text = ""
        
        If Len(Trim(Label42.Caption)) > 0 Then
            Set Rs_GetBranchCd = New ADODB.Recordset
            Rs_GetBranchCd.open "select i.branch_code from investor_master i where I.inv_code='" & Trim(Label42.Caption) & "'", MyConn, adOpenDynamic, adLockOptimistic
            If Rs_GetBranchCd.EOF = False Then
                branch_code = Rs_GetBranchCd!branch_code
            End If
            Rs_GetBranchCd.Close
        End If
        Set Rs_GetBranchCd = Nothing        
        
        If AllIndia = False Then
            If branch_code = "" Then
                TxtBusiCodeM.Text = ""
                MsgBox "Please Select Investor First", vbInformation
                Exit Sub
            End If
        End If
        

        Set rsEmp = New ADODB.Recordset
        rsEmp.open "Select source,branch_name,rm_name,rm_code from employee_master e,branch_master b where e.payroll_id='" & Trim(TxtBusiCodeM.Text) & "' and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018) ", MyConn, adOpenForwardOnly
        If rsEmp.EOF = False Then
            cmbBusiBranchM.Clear
            cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
            cmbBusiBranchM.ListIndex = 0
            TxtRmNameM.Text = rsEmp(2)
            'MyRmCodeM = rsEmp(3)
            'Label14.Caption = rsEmp(1)
            Label30.Caption = rsEmp(2)
            Call Show_Tr_Branches(Trim(TxtBusiCodeM.Text))
            If valid_rm(branch_code, TxtBusiCodeM) = False Then Exit Sub
            If rsEmp.State = 1 Then rsEmp.Close
            
            
            If Tr_Branches <> "" Then
                cmbBusiBranchM.Clear
                Set rsEmp = New ADODB.Recordset
                rsEmp.open "Select branch_code,branch_name from branch_master b where branch_code in (" & Tr_Branches & ") order by branch_name", MyConn, adOpenForwardOnly
                While Not rsEmp.EOF
                    cmbBusiBranchM.AddItem rsEmp(1) & Space(100) & "#" & rsEmp(0)
                    If rsEmp(0) = busi_br_tr Then
                        cmbBusiBranchM.ListIndex = i
                    End If
                    rsEmp.MoveNext
                Wend
                'cmbBusiBranchM.ListIndex = 0
            End If
            
            If busi_change_flag = True Then
                set_default_branch branch_code
            End If
            
            If rsEmp.State = 1 Then rsEmp.Close
            Set rsEmp = Nothing
            
        Else
            TxtBranchM.Text = ""
            MyRmCodeM = 0
            TxtBusiCodeM.Text = ""
            TxtRmNameM.Text = ""
            Label14.Caption = ""
            Label30.Caption = ""
        End If
        
        Set rsEmp = Nothing
    End If
End Sub



                 */

            }
            else
            {
                pc.ShowAlert(this, "No record found!");
                return;
            }
        }



        #region Cross Channel Validation

        public string ValidateCrossChannelInfo(string commonId, string label32, string loginId)
        {
            string message = string.Empty;
            string remark = string.Empty;

            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("psm_mf2_v_cross_channel_temp", connection))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        cmd.Parameters.Add("p_common_id", OracleDbType.Varchar2).Value = commonId;
                        cmd.Parameters.Add("p_label32", OracleDbType.Varchar2).Value = label32 ?? string.Empty;
                        cmd.Parameters.Add("p_login_id", OracleDbType.Varchar2).Value = loginId ?? string.Empty;

                        // Output cursor
                        var cursorParam = new OracleParameter("p_cursor", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(cursorParam);

                        connection.Open();
                        cmd.ExecuteNonQuery();

                        // Read single row from cursor
                        using (var reader = ((OracleRefCursor)cursorParam.Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                remark = reader["message"].ToString();
                            }
                        }

                        message = remark;
                    }
                    catch (Exception ex)
                    {
                        message = "Error: " + ex.Message;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }
            }

            return message;
        }


        private bool CrossChannelValidation(string dtNUmber, string invCode)
        {
            string loggedUserId = Convert.ToString(Session["LoginId"]);
            var (count, crossChannelValidateMessage) = new TransactionInvestmentController().CrossChannelValidateInfo(dtNUmber, invCode, loggedUserId);

            string message = HttpUtility.JavaScriptStringEncode(crossChannelValidateMessage);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ConsoleMessage", $"console.log('{message}');", true);

            if (count > 0)
            {
                var data = new TransactionInvestmentController().GetCrissChannelData(dtNUmber, loggedUserId, invCode);
                gvCrossChannel.DataSource = data;
                gvCrossChannel.DataBind();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "CrossChannelAlert", $"ShowCrossChannelModal()", true);
                return false;
            }

            return true;
        }

        protected void BtnCrossChannelApproval_Click(object sender, EventArgs e)
        {
            try
            {
                string docId = dtNumberA.Text;
                string invCode = invcode.Text;
                CrossChannel2(docId, invCode);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "CrossChannelHideAlert", "HideCrossChannelModal();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "CrossChannelHideAlert", "HideCrossChannelModal();", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
                return;
            }

        }


        public void CrossChannel2(string docId, string invCode)
        { 
            string loggedUserId = Convert.ToString(Session["LoginId"]);
            var (count, remarks, message) = new TransactionInvestmentController().ValidateCrossChannelInfo(docId, invCode);
            if (count == 0)
            {
                var dbMessage = new TransactionInvestmentController().UpdateCrossChannelApproval(docId, loggedUserId, remarks);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", $"alert('{dbMessage}');", true);
            }

        }

        protected void BtnContinueCross_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "CrossChannelHideAlert", "HideCrossChannelModal();", true);

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowPopup3", "showPopup3();", true);
            GetSetInvAddressData(invcode.Text, true);
            hdnPopupVisible1.Value = "true";

            HdnIsCrossContinue.Value = "yes";
            //BtnViewDtNumber_Click(this, null);

        }

        #endregion
        #endregion
    }
}
