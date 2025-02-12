using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using WM.Controllers;
using OfficeOpenXml;
using Excel = Microsoft.Office.Interop.Excel;
using ListItem = System.Web.UI.WebControls.ListItem;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.ComponentModel.Design;
using System.Windows.Interop;
using Microsoft.Ajax.Utilities;
using System.Data.OleDb;
using System.Linq;


namespace WM.Masters
{
    public partial class NpsTransactionPunching : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            string baseUrl = "https://wealthmaker.in/login_new.aspx";

            if (Session["LoginId"] == null)
            {
                Response.Redirect($"{baseUrl}");
                //Response.Redirect("~/index.aspx");
            }
            else
            {

                if (!IsPostBack)
                {


                    FillSchemaCode();
                    //FillCRABranchList();
                    //fillCRABrnach();
                    fillBranchMasterList();
                    fillRequestType();
                    FillBankMasterList();
                    //RegisterToggleCorporateNameScript();
                    //cboKRA_SelectedIndexChanged();
                    try
                    {
                        string loginSessionValue = Session["LoginId"]?.ToString() ?? string.Empty;

                        DataTable loginSessionData = new WM.Controllers.NpsTransactionPunchingController().GET_PSM_LOGIN_DATA(loginSessionValue);
                        if (loginSessionData != null && loginSessionData.Rows.Count > 0)
                        {

                            // Permission logic
                            //string glbLoginId = loginSessionData.Rows[0]["LOGIN_ID"].ToString();
                            //string glbRoleId  = loginSessionData.Rows[0]["ROLE_ID"].ToString();

                            // Define a static array of valid ROLE_ID values
                            string[] glbLoginIdsForBoth = { "115514", "46183", "114678" };
                            string[] glbLoginIdsForExport = { "1", "91" };

                            string[] glbLoginIdsForSaveModify = { "212", };

                            // Check if any row in the DataTable contains a matching ROLE_ID
                            bool glbLoginIdValuesForBoth = loginSessionData.AsEnumerable().Any(row => glbLoginIdsForBoth.Contains(row["ROLE_ID"].ToString()));
                            bool glbLoginIdValuesForExport = loginSessionData.AsEnumerable().Any(row => glbLoginIdsForExport.Contains(row["ROLE_ID"].ToString()));
                            bool glbLoginIdValuesForSaveModity = loginSessionData.AsEnumerable().Any(row => glbLoginIdsForExport.Contains(row["ROLE_ID"].ToString()));

                            if (glbLoginIdValuesForBoth)
                            {
                                btnImportEcs.Enabled = true;
                                btnImportCorporateNonEsc.Enabled = true;
                            }
                            else
                            {
                                btnImportEcs.Enabled = false;
                                btnImportCorporateNonEsc.Enabled = false;

                            }

                            //if (glbLoginIdValuesForExport) { btnExportToExcel.Enabled = true; }



                            if (glbLoginIdValuesForSaveModity)
                            {
                                btnSave.Enabled = true;
                                btnModify.Enabled = true;

                            }


                        }


                        btnImportEcs.Enabled = false;
                        btnImportCorporateNonEsc.Enabled = false;

                        // Set Date and Time fields
                        txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                        txtTime.Text = DateTime.Now.ToString("HH:mm");

                        // Default options
                        //OptIndividual.Checked = true;
                        //optExECS.Checked = true;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                        Console.WriteLine("Error in Page_Load: " + ex.Message);
                    }
                }
            }
        }

        private void RegisterToggleCorporateNameScript()
        {
            string script = @"
        <script type='text/javascript'>
            //
        </script>";

            ClientScript.RegisterStartupScript(this.GetType(), "scriptName", script);
        }


        protected void cboKRA_SelectedIndexChanged_SelectedIndexChanged(object sender, EventArgs e)
        {

            cboKRA_SelectedIndexChanged();
        }


        protected void cboKRA_SelectedIndexChanged()
        {
            //int selectedIndex = Convert.ToInt32(ddlCra.SelectedValue);
            int selectedIndex = 0;

            NpsTransactionPunchingController controller = new NpsTransactionPunchingController();
            DataTable branchData = controller.NTPCraGetBranchData(selectedIndex);

            ddlCraBranch.Items.Clear();
            foreach (DataRow row in branchData.Rows)
            {
                string branchName = row["branch_name"].ToString();
                string branchCode = row["branch_code"].ToString();
                string nsdlNo = row["NSDL_NO"].ToString();
                //ddlCraBranch.Items.Add($"{branchName.PadRight(80)} #{branchCode.PadRight(10)} #{nsdlNo}");
                ddlCraBranch.Items.Add($"{branchName.PadRight(80)}");

            }

            if (ddlCraBranch.Items.Count > 0)
            {
                ddlCraBranch.SelectedIndex = 0;
            }
            ddlCraBranch.Focus();
        }






        public void PrintButton_Click(object sender, EventArgs e)
        {

            string receiptNo = txtReceiptNo.Text;

            printData(receiptNo);

        }

        public void printData(string receiptNo)
        {

            if (string.IsNullOrEmpty(receiptNo))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please Enter Receipt No First');", true);
                return;
            }

            try
            {
                DataTable receiptData = new WM.Controllers.NpsTransactionPunchingController().PrintReceiptData(receiptNo);
                if (receiptData != null && receiptData.Rows.Count > 0)
                {
                    DataRow row = receiptData.Rows[0];

                    // Populate all required fields for modal and print sheet
                    popSaveBUSINESS_RMCODE.Text = GetSafeString(row["BUSINESS_RMCODE"]);
                    popSaveTRAN_ID.Text = GetSafeString(row["TRAN_CODE"]);
                    popSaveLOGGEDUSERID.Text = GetSafeString(row["LOGGEDUSERID"]);
                    popSaveAMOUNT1.Text = GetSafeDecimal(row["AMOUNT"]).ToString("N2");
                    popSaveAMOUNT2.Text = GetSafeDecimal(row["AMT1"]).ToString("N2");
                    popSaveTRANCODE.Text = GetSafeString(row["PAYMENT_MODE"]);
                    popSaveTRAN_SRC.Text = GetSafeString(row["TRAN_SRC"]);
                    popSaveREG_TRANTYPE.Text = GetSafeString(row["REG_TRANTYPE"]);
                    popSaveREG_recieptnoT.Text = GetSafeString(row["UNIQUE_ID"]);
                    popTRDATE.Text = GetSafeString(row["TR_DATE"]);

                    // Show the modal and trigger print
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModalAndPrint", @"
                $('#printSaveRecietDataModel').modal('show');
                setTimeout(function() { 
                    printGrid(); 
                }, 500);", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Fields found are filled !.');", true);
                }
            }
            catch (Exception ex)
            {
                LogError(ex); // Assuming you have a logging mechanism
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error occurred while fetching receipt data. Please try again.');", true);
            }


        }
        // Helper methods for safe data conversion
        private string GetSafeString(object value)
        {
            return value == DBNull.Value ? string.Empty : value.ToString().Trim();
        }

        private decimal GetSafeDecimal(object value)
        {
            if (value == DBNull.Value) return 0;
            return decimal.TryParse(value.ToString(), out decimal result) ? result : 0;
        }

        private void LogError(Exception ex)
        {
            // Implement your error logging mechanism here
            System.Diagnostics.Debug.WriteLine($"Error in PrintButton_Click: {ex.Message}");
            // You might want to log to a file or database
        }





        #region btnCmdShow_Click
        protected void btnCmdShow(object sender, EventArgs e)
        {
            string dtNumberValue = txtDtNumber.Text.Trim();

            int tempDataCount = 0;
            int stDataCount = 0;

            try
            {
                // Check if the DT number is empty or null
                if (string.IsNullOrWhiteSpace(dtNumberValue))
                {
                    string msg = "Enter DT Number";
                    ShowAlert(msg);
                    return;
                }

                string dtNumber = txtDtNumber.Text.Trim();
                string aRNumber = dtNumberValue;  // It seems like you're using DT number here for AR number, which is fine for now
                bool onlyDtFlag = false;
                // Call the procedure with null for AR number and false for Beforemark
                DataTable dt = new NpsTransactionPunchingController().GET_AR_BY_DTTS(dtNumberValue, null, false, Session["LoginId"]?.ToString());

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string dbMsg = row["message"].ToString();

                    // Check for valid DT number
                    if (dbMsg.Contains("Validity: DT is valid"))
                    {

                        // Extract values from the result for later use
                        string arFromDBValue = row["ar_code"].ToString();
                        string ivFromDBValue = row["inv_code"].ToString();
                        string bbFromDBValue = row["busi_branch_code"].ToString();
                        string brFromDBValue = row["busi_rm_code"].ToString();
                        string dtFromDBValue = row["common_id"].ToString();
                        string ivnFromDBValue = row["INV_NAME"].ToString();


                        if (dbMsg.Contains("Validity: DT is valid") && string.IsNullOrEmpty(arFromDBValue))
                        {
                            ShowAlert("Only DT Exist but Transaction not exist");
                            onlyDtFlag = true;
                        }
                        // If AR exists, proceed with checking transaction in temp and st tables
                        if (!string.IsNullOrEmpty(arFromDBValue))
                        {
                            // Check for AR in temp transaction table
                            DataTable dtIfArExistForTempTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, arFromDBValue, true, Session["LoginId"]?.ToString());
                            DataTable dtIfArExistForStTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, arFromDBValue, false, Session["LoginId"]?.ToString());


                            tempDataCount = dtIfArExistForTempTran.Rows.Count > 0 ? 1: 0;
                            stDataCount = dtIfArExistForStTran.Rows.Count > 0 ? 1 : 0 ;


                            if (tempDataCount == 0) // No data in temp
                            {
                                // Check for AR in main transaction table
                                //DataTable dtIfArExistForStTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, arFromDBValue, false);

                                if (stDataCount > 0) // Data exists in st
                                {
                                    string dbMsg1 = dtIfArExistForStTran.Rows[0]["message"].ToString();
                                    if (dbMsg1.Contains("Validity: Transaction data exist in st"))
                                    {

                                        lblArNo.Text = arFromDBValue;
                                        txtInvestorCode.Text = ivFromDBValue;
                                        txtBusinessRm.Text = brFromDBValue;
                                        ddlBusinessBranch.SelectedValue = bbFromDBValue;
                                        txtNameOfSubscriber.Text = ivnFromDBValue;
                                        DataRow row3 = dtIfArExistForStTran.Rows[0];
                                        //ShowAlert(dbMsg1);
                                        SetARFieldData(row3);
                                        btnSave.Enabled = false;
                                        btnModify.Enabled = true;
                                        txtDtNumber.Text = dtNumber;
                                        txtBusinessRm.Enabled = false;

                                    }
                                }
                                else
                                {
                                   // ShowAlert(dbMsg);

                                    ShowAlert("No data in transaction st");
                                }
                            }
                            
                        }
                    } 

                    else if (dbMsg.Contains("Validity: DT is valid") && ( stDataCount ==0))
                    {
                        ShowAlert("Only DT exist but not transaction");
                    }
                    else
                    {
                        ShowAlert(dbMsg);
                    }

                }
                else
                {
                    ShowAlert("No data found for the given DT number");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or show error message to the user
                ShowAlert(ex.Message);
            }
        }

        #endregion

        private void FillBankMasterList()
        {
            // Create an instance of the controller that retrieves bank master data
            var bankController = new WM.Controllers.NpsTransactionPunchingController();

            // Fetch the bank master list from the database
            DataTable bankMasterList = bankController.GetBankMasterList();
            ddlBankName.DataSource = bankMasterList;
            ddlBankName.DataTextField = "BANK_NAME";
            ddlBankName.DataValueField = "BRANCH_CODE";
            ddlBankName.DataBind();

            // Insert a default item at the top of the dropdown list
            ddlBankName.Items.Insert(0, new ListItem("--Select Bank--", "0"));
        }


        #region  FillCRABranchList
        private void FillCRABranchList()
        {
            // Fetch the branch master list
            DataTable dt = new WM.Controllers.NpsTransactionPunchingController().GetCRABranchMasterList();

            // Bind the fetched data to the dropdown
            ddlCraBranch.DataSource = dt;
            ddlCraBranch.DataTextField = "BRANCH_NAME";   // Text field for dropdown display
            ddlCraBranch.DataValueField = "BRANCH_CODE";  // Value field for dropdown value
            ddlCraBranch.DataBind();

            // Insert a default "Select" option at the top
            ddlCraBranch.Items.Insert(0, new ListItem("Select", "0"));


        }
        #endregion

        private void FillSchemaCode()
        {
            // Get the list of scheme codes from the database
            DataTable schemeCodeList = new WM.Controllers.NpsTransactionPunchingController().GetSchemeCodeList();

            if (schemeCodeList != null && schemeCodeList.Rows.Count > 0)
            {
                // Clear existing items before binding
                ddlScheme.Items.Clear();

                // Iterate through the DataTable and add items with custom text
                foreach (DataRow row in schemeCodeList.Rows)
                {
                    string schemeCode = row["SCH_CODE"].ToString();
                    string displayText;

                    // Set the display text based on the scheme code
                    switch (schemeCode)
                    {
                        case "OP#09971":
                            displayText = "New Pension Scheme Tier 1";
                            break;
                        case "OP#09972":
                            displayText = "New Pension Scheme Tier 2";
                            break;
                        case "OP#09973":
                            displayText = "New Pension Scheme Tier 1+2";
                            break;
                        default:
                            displayText = "Unknown Scheme"; // Fallback text for any unhandled codes
                            break;
                    }

                    // Add the item to the dropdown list
                    ddlScheme.Items.Add(new ListItem(displayText, schemeCode));
                }
            }

            // Add a default item to the dropdown list
            ddlScheme.Items.Insert(0, new ListItem("--Select Scheme--", "0"));
        }



     

        private void fillRequestType()
        {
            // Populate Request Types dropdown from database
            DataTable requestTypeList = new WM.Controllers.NpsTransactionPunchingController().GetRequestTypeList();
            ddlRequestType.DataSource = requestTypeList;
            ddlRequestType.DataTextField = "REQUEST_NAME";  // Ensure the field name matches the column in your DataTable
            ddlRequestType.DataValueField = "REQUEST_CODE"; // Use the corresponding code field (if needed)
            ddlRequestType.DataBind();
            ddlRequestType.Items.Insert(0, new ListItem("--Select Request Type--", "0"));

        }

        #region fillCRABrnach
        private void fillCRABrnach()
        {
            int craBranchCode = 0;
            DataTable dt = new WM.Controllers.NpsTransactionPunchingController().NTPCraGetBranchData(craBranchCode);

            ddlCraBranch.DataSource = dt;
            ddlCraBranch.DataTextField = "BRANCH_NAME";
            ddlCraBranch.DataValueField = "BRANCH_CODE";
            ddlCraBranch.DataBind();
        }
        #endregion


        #region  FillCRABranchList
        private void fillBranchMasterList()
        {

            DataTable dt = new WM.Controllers.NpsTransactionPunchingController().GetBranchMasterList();
            AddDefaultItem(dt, "BRANCH_NAME", "BRANCH_CODE", "Select");
            ddlBusinessBranch.DataSource = dt;
            ddlBusinessBranch.DataTextField = "BRANCH_NAME";
            ddlBusinessBranch.DataValueField = "BRANCH_CODE";
            ddlBusinessBranch.DataBind();
        }
        #endregion



        #region AddDefaultItem

        private void AddDefaultItem(DataTable dt, string textField, string valueField, string defaultText)
        {
            DataRow row = dt.NewRow();
            row[textField] = defaultText;

            if (dt.Columns[valueField].DataType == typeof(string))
            {
                row[valueField] = string.Empty;
            }
            else
            {
                row[valueField] = DBNull.Value;
            }

            dt.Rows.InsertAt(row, 0);
        }
        #endregion




        protected void btnExportTransaction_Click(object sender, EventArgs e)
        {
            // Capture form input using the appropriate control IDs
            string fromDateText = fromDate.Text ?? string.Empty; // Get the text from the TextBox
            string toDateText = toDate.Text ?? string.Empty;     // Get the text from the TextBox

            // Parse the input dates as DateTime objects
            DateTime fromDateValue;
            DateTime toDateValue;

            if (!DateTime.TryParseExact(fromDateText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fromDateValue) ||
                !DateTime.TryParseExact(toDateText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out toDateValue))
            {
                //lblMessage.Text = "Please provide valid From Date and To Date.";
                return;
            }

            string transactionType = rblTransactionType.SelectedValue;  // Get the selected value from RadioButtonList
            string craValue = cra.SelectedValue;                        // Get the selected value from DropDownList

            try
            {
                // Call the controller method directly
                NpsTransactionPunchingController controller = new NpsTransactionPunchingController();
                DataTable transactionData = controller.ExportTransaction(fromDateValue.ToString("yyyy-MM-dd"), toDateValue.ToString("yyyy-MM-dd"), transactionType, craValue);

                // Export the result to Excel
                if (transactionData != null && transactionData.Rows.Count > 0)
                {
                    ExportToExcel(transactionData);
                }
                else
                {
                    //  lblMessage.Text = "No transaction data available for the selected criteria.";
                }
            }
            catch (Exception ex)
            {
                // lblMessage.Text = "An error occurred: " + ex.Message;
            }
        }

        private void ExportToExcel(DataTable dataTable)
        {
            // Create Excel application and workbook
            var objXL = new Excel.Application();
            var wbXL = objXL.Workbooks.Add();
            var wsXL = (Excel.Worksheet)objXL.ActiveSheet;
            wsXL.Name = "NPS_tran";

            // Add column headers from the DataTable
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                wsXL.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
            }

            // Add rows from the DataTable
            int row = 2;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    wsXL.Cells[row, i + 1] = dataRow[i].ToString();
                }
                row++;
            }

            // Make Excel application visible
            objXL.Visible = true;

            // Optionally: Save the file, or handle as needed
            wbXL.SaveAs("ExportedFile.xlsx");
        }



        #region Buttons


        protected void btnShow_Click(object sender, EventArgs e)
        {
            // Handle the Show button click event
        }



        protected void btnSearchInvestor_Click(object sender, EventArgs e)
        {
            // Handle the Search Investor button click event
        }



        protected void btnAddNewBank_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Masters/addnewbank.aspx"); // Redirect to home page or any other page

        }


        protected void SaveButton_Click(object sender, EventArgs e)
        {
            int index = 0;
            string Busi_Branch_cd = string.Empty;
            string Busi_Rm_Cd = string.Empty;
            string ClientBranchCode = string.Empty;
            string ClientRmCode = string.Empty;
            string[] br_cd;
            string paymode = string.Empty;
            string invCode = string.Empty;
            string dipo_type = string.Empty;
            string Pay_type = string.Empty;
            string str_test = string.Empty;
            string MyTranCode = string.Empty;
            string MyGSTNO = string.Empty;
            string MyTranCode1 = string.Empty;
            string MySecReq = string.Empty;
            object Dup_RectNo = null;
            string Vclientcategory = string.Empty;
            int i;
            object Xmin = null;
            object Xmax = null;


            // Permission logic
            string glbLoginId = Session["Glbloginid"]?.ToString() ?? string.Empty;
            string GlbroleId = Session["GlbroleId"]?.ToString() ?? string.Empty;

            string selectedCorporateValue = ddlType.SelectedValue;
            string selectedCorporateName = corporateName.Text; // .Text gives the actual value from the TextBox
            string selecteScheme = ddlScheme.SelectedValue;
            string currentAR = lblArNo.Text.ToString();
            string currentDT = txtDtNumber.Text.ToString();


            
            /*
             Punching id

            index ; 0   : GlbroleId = 212, 1,261
            index ; 4   : GlbroleId = 146, 1

            
             */




            if (!string.IsNullOrEmpty(currentAR))
            {
                ShowAlert("Transaction already exist.");
                return;
            }

            if (txtDtNumber.Text == "" || string.IsNullOrEmpty(txtDtNumber.Text))
            {
                string msg = "DT No cannot be left blank";
                ShowAlert(msg);
                txtDtNumber.Focus();
                return;
            }

            if (ddlProductClass.Text == "")
            {
                string msg = "New Pension Scheme is empty";
                ShowAlert(msg);
                ddlProductClass.Focus();
                return;
            }

            if (selectedCorporateValue == "1" && string.IsNullOrEmpty(selectedCorporateName))
            {
                ShowAlert("Corporate name is required.");
                corporateName.Focus();
                return;
            }
           
            if (chkZeroCommission.Checked)
            {
                if (txtPran.Text.Trim() != "")
                {
                    //if (SqlRet($"SELECT COUNT(*) FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO='{ar.Text}'") >= 1)

                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('FATCA for this PRAN is non-compliant. Please contact product team for the same.');", true);
                    return;
                }
            }
            
            if (string.IsNullOrEmpty(selecteScheme))
            {
                ShowAlert("Scheme is required.");
                ddlScheme.Focus();
                return;
            }


            InsertUpdate(null);


        }

        public void ShowConfirmation(string message)
        {
            string script = $@"
        if (confirm('{message}')) {{
            // User clicked 'Yes'. You can add client-side logic here if needed.
        }} else {{
            // User clicked 'No'. You can add client-side logic here if needed.
            return;
        }}
    ";
            ClientScript.RegisterStartupScript(this.GetType(), "confirmationAlert", script, true);
        }



        public void ShowAlert(string message)
        {
            string script = $"alert('{message}');";
            ClientScript.RegisterStartupScript(this.GetType(), "validationAlert", script, true);
        }


        protected void ModifyButton_Click(object sender, EventArgs e)
        {
            string markVlaue = lblArNo.Text.ToString();
            string selectedCorporateValue = ddlType.SelectedValue;
            string selectedCorporateName = corporateName.Text; // .Text gives the actual value from the TextBox
            string selecteScheme = ddlScheme.SelectedValue;


            if (string.IsNullOrEmpty(markVlaue) || markVlaue == "0")
            {
                ShowAlert("Load a transasction");
                lblArNo.Focus();
            }

            if (selectedCorporateValue == "1" && string.IsNullOrEmpty(selectedCorporateName))
            {
                ShowAlert("Corporate name is required.");
                return;
            }

            if (string.IsNullOrEmpty(selecteScheme) || selecteScheme == "0")
            {
                ShowAlert("Scheme is required.");
                ddlScheme.Focus();
                return;
            }

            //ShowConfirmation("Do you want to prceed?");

            //bool userResponse = bool.TryParse(hdnUserResponse.Value, out bool result) && result;

            //if (userResponse)
            //{
                
            //    ShowAlert("Clicked on yes");
            //    Response.Write("You clicked Yes!");
            //}
            //else
            //{
               
            //    ShowAlert("Cliekc on No");
            //    Response.Write("You clicked No!");
            //}




            InsertUpdate(lblArNo.Text.ToString());
        }





        private void InsertTransaction(string paymode, string Busi_Branch_cd, string Busi_Rm_Cd)
        {
            // SQL insert logic for saving transaction
        }

        private void UpdateTransaction(string paymode, string Busi_Branch_cd, string Busi_Rm_Cd)
        {
            // SQL update logic for modifying transaction
        }

        private bool ValidateSave()
        {
            // Implement validation checks for saving
            return true;
        }

        private bool ValidateModification()
        {
            // Implement validation checks for modifying
            return true;
        }

        private int SqlRet(string query)
        {
            // Implement method for executing query and returning an integer result.
            return 0;
        }




        protected void ExitButton_Click(object sender, EventArgs e)
        {

            //Response.Redirect("~/welcome.aspx");

            
            string baseUrl = "https://wealthmaker.in/login_new.aspx";
            //Response.Redirect($"{baseUrl}");

            if (Session["LoginId"] == null)
            {
                Response.Redirect($"{baseUrl}");
                //Response.Redirect("~/index.aspx");
            }
            else
            {
                Response.Redirect("~/welcome.aspx");
            }

            
        }

        #endregion

        protected void ResetButton_Click(object sender, EventArgs e)
        {
            ResetMain();
        }
        protected void ResetMain()
        {
            // DropDownList
            arPopReset();
            lblMessage.Text = string.Empty;
            btnShow.Enabled = true;
            lblArNo.Text = string.Empty;
            ddlProductClass.SelectedIndex = 0;
            ddlScheme.SelectedIndex = 0;
            ddlCra.SelectedIndex = 0;
            ddlCraBranch.SelectedIndex = 0;
            ddlBusinessBranch.SelectedIndex = 0;
            ddlRequestType.SelectedIndex = 0;
            ddlBankName.SelectedIndex = 0;

            // RadioButtonList
            ddlType.ClearSelection();
            ddlPaymentMethod.ClearSelection();
            rblTransactionType.ClearSelection();

            // TextBox
            txtDtNumber.Text = "";
            txtInvestorCode.Text = "";
            txtPopSpRegNo.Text = "";
            txtBusinessRm.Text = "";
            txtReceiptNo.Text = "";
            txtChequeNo.Text = "";
            txtChequeDated.Text = "";
            txtPran.Text = "";
            txtAmountReceivedTire1.Text = "";
            txtAmountReceivedTire2.Text = "";
            txtPopRegistrationChargesOneTime.Text = "";
            txtPopRegistrationCharges.Text = "";
            txtGst.Text = "";
            txtCollectionAmount.Text = "";
            txtAmountInvested.Text = "";
            txtAmountInvestedAdditional.Text = "";
            txtRemark.Text = "";
            txtNameOfSubscriber.Text = "";
            txtDate.Text = "";
            txtTime.Text = "";
            txtInvestorCode.Text = "";
            newBank.Text = "New Bank";

            // Label
            lblArNo.Text = "0";
            txtInvestorCode.Text = string.Empty;

            // CheckBox
            chkZeroCommission.Checked = false;

            // Set date and time pickers to default values
            fromDate.Text = "";

            toDate.Text = "";
            txtChequeDated.Text = ""; // If using date picker control

            // Button enable states
            //  btnSave.Enabled = true; // Enable Save button
            //  btnModify.Enabled = false; // Enable Modify button
            // btnImportCorporateNonEsc.Enabled = true;
            // btnImportEcs.Enabled = true;
            //   btnPrint.Enabled = false;
        }

        protected string GetSelectedTransactionType()
        {
            // Check if a transaction type is selected
            if (rblTransactionType.SelectedItem != null)
            {
                // Return the selected value
                return rblTransactionType.SelectedValue;
            }
            return string.Empty; // Return an empty string if none is selected
        }

        protected void arPopReset_Click(object sender, EventArgs e)
        {
            arPopReset();
        }

        protected void arPopReset()
        {
            ARListlblMessage.Text = string.Empty;
            arPopArNo.Text = string.Empty;
            arPopAppNo.Text = string.Empty;
            arPopChequeNo.Text = string.Empty;
            arPopPranNo.Text = string.Empty;
            arPopDdlSchemas.SelectedIndex = -1;
            arPopInvName.Text = string.Empty;
            arPopAnaExistCode.Text = string.Empty;
            arPopArFromDate.Text = string.Empty;
            arPopArToDate.Text = string.Empty;

            // Reset RadioButton selections
            arPopRadioBefore.Checked = false;
            arPopRadioAfter.Checked = false;

            // Clear GridView
            ARListARGridView.DataSource = null; // Clear the data source
            ARListARGridView.DataBind(); // Rebind to refresh the GridView
        }


        public void arPopResetGrid_Click()
        {
            // ARListlblMessage.Text= string.Empty;
            ARListARGridView.DataSource = null; // Clear the data source
            ARListARGridView.DataBind(); // Rebind to refresh the GridView
        }


        private DateTime? ParseDate(string dateText, string dateFormat = "dd/MM/yyyy")
        {
            if (string.IsNullOrWhiteSpace(dateText)) return null; // Handle empty or whitespace input

            // Use DateTime.TryParseExact to ensure proper date format handling
            return DateTime.TryParseExact(dateText, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
                ? parsedDate
                : (DateTime?)null;
        }


        #region Handle Date with Formate

        private void SetDateValue(DataRow row, string columnName, string dateFormat, System.Web.UI.WebControls.TextBox uiField)
        {
            if (DateTime.TryParse(row[columnName]?.ToString(), out DateTime dateValue))
            {
                uiField.Text = dateValue.ToString(dateFormat); // Set the date in the specified format
            }
            else
            {
                uiField.Text = string.Empty; // If date parsing fails, clear the field
            }
        }

        #endregion



        public string GetTextFieldValue(DataRow dataRow, string fieldName)
        {
            try
            {

                if (lblMessage.Text != null)
                {
                    lblMessage.Text = "";
                }

                // Check if the field exists in the DataRow
                if (dataRow.Table.Columns.Contains(fieldName))
                {
                    // Check if the field value is not null and return it, otherwise return null
                    return dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : null;
                }
                else
                {
                    throw new ArgumentException($"Field '{fieldName}' does not exist in the DataRow.");
                }
            }
            catch (Exception ex)
            {
                // If the label is found, show the error message
                lblMessage.Text = ex.Message;
                return null;
            }
        }

        private void SelectValueInDropdown(DropDownList dropdown, string valueToSelect)
        {
            if (dropdown == null || dropdown.Items.Count == 0) return;

            // Check if the value exists in the DropDownList
            ListItem item = dropdown.Items.FindByValue(valueToSelect);
            if (item != null)
            {
                // If the value exists, select it
                dropdown.SelectedValue = valueToSelect;
            }
            else
            {
                // If the value doesn't exist, select a fallback value (e.g., the first item)
                dropdown.SelectedIndex = 0; // Select the first item as default
            }
        }
        public static class DateTimeParser
        {
            // Function to parse a date string
            public static DateTime? ParseDate(string dateString, string format = "dd/MM/yyyy")
            {
                if (string.IsNullOrWhiteSpace(dateString))
                {
                    return null; // Return null if the input string is null or empty
                }

                if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate; // Return parsed date if successful
                }

                // Handle invalid date format
                throw new FormatException("Invalid date format. Expected format is: " + format);
            }

            // Function to parse a time string
            public static DateTime? ParseTime(string timeString, string format = "HH:mm")
            {
                if (string.IsNullOrWhiteSpace(timeString))
                {
                    return null; // Return null if the input string is null or empty
                }

                if (DateTime.TryParseExact(timeString, format, null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                {
                    return parsedTime; // Return parsed time if successful
                }

                // Handle invalid time format
                throw new FormatException("Invalid time format. Expected format is: " + format);
            }
        }


        private void SetARPopFieldData(DataRow row)
        {
            lblArNo.Text = GetTextFieldValue(row, "TRAN_CODE");
            txtInvestorCode.Text = GetTextFieldValue(row, "TRAN_CODE");
            txtBusinessRm.Text = GetTextFieldValue(row, "BUSINESS_RMCODE");

            txtRemark.Text = GetTextFieldValue(row, "REMARK");
            txtAmountInvested.Text = GetTextFieldValue(row, "");

            ddlPaymentMethod.SelectedValue = GetTextFieldValue(row, "PAYMENT_MODE");

            txtChequeNo.Text = GetTextFieldValue(row, "CHEQUE_NO");
            txtChequeDated.Text = GetTextFieldValue(row, "CHEQUE_DATE");
            ddlBankName.SelectedValue = GetTextFieldValue(row, "BANK_NAME");

        }

        private DropDownList GetDdlBankName()
        {
            return ddlBankName;
        }

        private void SetARFieldData(DataRow row)
        {
            // Extract values into variables
            string corporateNameValue = GetTextFieldValue(row, "CORPORATE_NAME");
            string docId = GetTextFieldValue(row, "DOC_ID");
            string tranCode = GetTextFieldValue(row, "TRAN_CODE");
            string clientCode = GetTextFieldValue(row, "CLIENT_CODE");
            string schCode = row["SCH_CODE"]?.ToString();
            string folioNo = GetTextFieldValue(row, "FOLIO_NO");
            string businessRmCode = GetTextFieldValue(row, "BUSINESS_RMCODE");
            string businessBranchCode = row["BUSI_BRANCH_CODE"]?.ToString();
            string uniqueId = row["UNIQUE_ID"]?.ToString();
            string paymentMode = row["PAYMENT_MODE"]?.ToString();
            string chequeNo = GetTextFieldValue(row, "CHEQUE_NO");


            DateTime? chequeDate = row["CHEQUE_DATE"] != DBNull.Value ? (DateTime?)row["CHEQUE_DATE"] : null;

            GetSetDateField(row, "CHEQUE_DATE", txtChequeDated);

            string bankName = row["BANK_NAME"]?.ToString();
            string appNo = row["APP_NO"]?.ToString();
            DateTime? trDate = row["TR_DATE"] != DBNull.Value ? (DateTime?)row["TR_DATE"] : null;
            string InvNameValue = GetTextFieldValue(row, "INV_NAME");
            string manualArNo = GetTextFieldValue(row, "manual_arno");
            string ntAmount1 = GetTextFieldValue(row, "AMOUNT1");
            string ntAmount2 = GetTextFieldValue(row, "AMOUNT2");
            string ntRegCharge = GetTextFieldValue(row, "REG_CHARGE");
            string ntTranCharge = GetTextFieldValue(row, "Tran_CHARGE");
            string ntServiceTax = GetTextFieldValue(row, "SERVICETAX");
            string amountInvested = GetTextFieldValue(row, "AMOUNT");
            string remark = GetTextFieldValue(row, "REMARK");

            // Assign values to controls
            SelectValueInDropdown(ddlType, row["INVESTOR_TYPE"]?.ToString());
            corporateName.Text = corporateNameValue;
            txtDtNumber.Text =    docId;
            lblArNo.Text = tranCode;
            txtInvestorCode.Text = clientCode;
            SelectValueInDropdown(ddlScheme, schCode);
            txtPopSpRegNo.Text = folioNo;
            txtBusinessRm.Text = businessRmCode;
            SelectValueInDropdown(ddlBusinessBranch, businessBranchCode);
            txtReceiptNo.Text = uniqueId;
            SelectValueInDropdown(ddlPaymentMethod, paymentMode);
            txtChequeNo.Text = chequeNo;
            //txtChequeDated.Text = chequeDate.HasValue ? chequeDate.Value.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrEmpty(bankName))
            {
                if (ddlBankName.Items.FindByValue(bankName) != null)
                {
                    ddlBankName.SelectedValue = bankName;
                }
                else
                {
                    ddlBankName.Items.Add(new ListItem(bankName, bankName));
                    ddlBankName.SelectedValue = bankName;
                }
            }
            else
            {
                ddlBankName.SelectedIndex = -1;
            }

            SelectValueInDropdown(ddlRequestType, appNo);
            //txtDate.Text = trDate.HasValue ? trDate.Value.ToString("dd/MM/yyyy") : string.Empty;

            GetSetDateField(row, "TR_DATE", txtDate);

            txtTime.Text = trDate.HasValue ? trDate.Value.ToString("HH:mm") : string.Empty;
            string currentSubName = txtNameOfSubscriber.Text;
            txtPran.Text = manualArNo;
            txtNameOfSubscriber.Text = string.IsNullOrEmpty(txtNameOfSubscriber.Text) ? InvNameValue : currentSubName;
            txtAmountReceivedTire1.Text = ntAmount1;
            txtAmountReceivedTire2.Text = ntAmount2;
            txtPopRegistrationChargesOneTime.Text = ntRegCharge;
            txtPopRegistrationCharges.Text = ntTranCharge;
            txtGst.Text = ntServiceTax;
            txtCollectionAmount.Text = string.Empty; // Update with actual key if available
            txtAmountInvested.Text = amountInvested;
            txtRemark.Text = remark;

            lblMessage.Text = "Found Data Filled";
        }


        private void GetSetDateField(DataRow row, string columnName, TextBox inputField)
        {
            if (System.DateTime.TryParse(row[columnName]?.ToString(), out System.DateTime parsedDate))
            {
                inputField.Text = parsedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                inputField.Text = string.Empty;
            }
        }


        private void SetARFieldData1(DataRow row)
        {

            //txtInvestorCode.Text = dr["AR_CODE"].ToString();
            //txtBusinessRm.Text = dr["BUSI_RM_CODE"].ToString();
            //ddlBusinessBranch.SelectedValue = dr["BUSI_BRANCH_CODE"].ToString();
            //txtNameOfSubscriber.Text = dr["INV_NAME"].ToString();
            //SelectValueInDropdown(ddlProductClass, row["ISS_CODE"].ToString());
            SelectValueInDropdown(ddlType, row["INVESTOR_TYPE"].ToString());

            corporateName.Text = GetTextFieldValue(row, "CORPORATE_NAME");
            txtDtNumber.Text = GetTextFieldValue(row, "DOC_ID");
            lblArNo.Text = GetTextFieldValue(row, "TRAN_CODE");
            txtInvestorCode.Text = GetTextFieldValue(row, "CLIENT_CODE");
            SelectValueInDropdown(ddlScheme, row["SCH_CODE"].ToString());
            // ddlCra.SelectedValue = GetTextFieldValue(row, "TRAN_CODE");
            // SelectValueInDropdown(ddlCraBranch, row["BRANCH_CODE"].ToString());




            txtPopSpRegNo.Text = GetTextFieldValue(row, "FOLIO_NO");
            txtBusinessRm.Text = GetTextFieldValue(row, "BUSINESS_RMCODE");
            SelectValueInDropdown(ddlBusinessBranch, row["BUSI_BRANCH_CODE"].ToString());
            txtReceiptNo.Text = (row["UNIQUE_ID"].ToString());

            SelectValueInDropdown(ddlPaymentMethod, row["PAYMENT_MODE"].ToString());
            txtChequeNo.Text = GetTextFieldValue(row, "CHEQUE_NO");

            DateTime? chequeDate = row["CHEQUE_DATE"] != DBNull.Value ? (DateTime?)row["CHEQUE_DATE"] : null;

            if (chequeDate.HasValue)
            {
                // Set the date part in the desired format
                txtChequeDated.Text = chequeDate.Value.ToString("dd/MM/yyyy"); // Adjust the format as needed
            }
            else
            {
                // Handle the case when CHEQUE_DATE is null or empty
                txtChequeDated.Text = string.Empty;
            }

            if (row["BANK_NAME"] != DBNull.Value && !string.IsNullOrEmpty(row["BANK_NAME"].ToString()))
            {
                string bankName = row["BANK_NAME"].ToString();

                // Check if the value exists in the dropdown list
                if (ddlBankName.Items.FindByValue(bankName) != null)
                {
                    ddlBankName.SelectedValue = bankName; // Set the selected value
                }
                else
                {
                    // Add the new item and set it as the selected item
                    ddlBankName.Items.Add(new ListItem(bankName, bankName));
                    ddlBankName.SelectedValue = bankName;
                }
            }
            else
            {
                // Handle cases where BANK_NAME is null or empty by deselecting any item
                ddlBankName.SelectedIndex = -1;
            }

            SelectValueInDropdown(ddlRequestType, row["APP_NO"].ToString());
            DateTime? trDateTime = row["TR_DATE"] != DBNull.Value ? (DateTime?)row["TR_DATE"] : null;

            if (trDateTime.HasValue)
            {
                // Set the date part
                txtDate.Text = trDateTime.Value.ToString("dd/MM/yyyy"); // Adjust the format as needed

                // Set the time part
                txtTime.Text = trDateTime.Value.ToString("HH:mm"); // Adjust the format as needed
            }
            else
            {
                // Handle the case when TR_DATE is null or empty
                txtDate.Text = string.Empty;
                txtTime.Text = string.Empty;
            }
            txtNameOfSubscriber.Text = GetTextFieldValue(row, "TS_INV_NAME") == null ? GetTextFieldValue(row, "DC_INV_NAME") : GetTextFieldValue(row, "TS_INV_NAME");
            txtPran.Text = GetTextFieldValue(row, "manual_arno");
            //CheckBox1.Checked = row[""].ToString().Equals(); // Unfreez


            txtAmountReceivedTire1.Text = GetTextFieldValue(row, "NT_AMOUNT1");
            txtAmountReceivedTire2.Text = GetTextFieldValue(row, "NT_AMOUNT2");
            txtPopRegistrationChargesOneTime.Text = GetTextFieldValue(row, "NT_REG_CHARGE");
            txtPopRegistrationCharges.Text = GetTextFieldValue(row, "NT_Tran_CHARGE");
            txtGst.Text = GetTextFieldValue(row, "NT_SERVICETAX");
            txtCollectionAmount.Text = GetTextFieldValue(row, "");
            txtAmountInvested.Text = GetTextFieldValue(row, "AMOUNT");

            txtRemark.Text = GetTextFieldValue(row, "REMARK");

            lblMessage.Text = "Found Data Filled";


        }

        private void InsertUpdate(string ui)
        {
            if (ui == "0")
            {
                ui = null;
            }
            try
            {
                string mark = ui;
                string prodValue = string.IsNullOrWhiteSpace(ddlProductClass.SelectedValue) ? string.Empty : ddlProductClass.SelectedValue;
                string investorTypeValue = string.IsNullOrWhiteSpace(ddlType.SelectedValue) ? string.Empty : ddlType.SelectedValue;
                string corporateNameValue = string.IsNullOrWhiteSpace(corporateName.Text) ? string.Empty : corporateName.Text;
                string dtNumberValue = string.IsNullOrWhiteSpace(txtDtNumber.Text) ? string.Empty : txtDtNumber.Text;
                string tranCodeValue = string.IsNullOrWhiteSpace(lblArNo.Text) ? string.Empty : lblArNo.Text;
                long invertorCodeValue = string.IsNullOrWhiteSpace(txtInvestorCode.Text) ? 0 : Convert.ToInt64(txtInvestorCode.Text);
                string schemeCodeValue = string.IsNullOrWhiteSpace(ddlScheme.SelectedValue) ? string.Empty : ddlScheme.SelectedValue;
                string craValue = string.IsNullOrWhiteSpace(ddlCra.SelectedValue) ? string.Empty : ddlCra.SelectedValue;
                int craBranchValue = string.IsNullOrWhiteSpace(ddlCraBranch.SelectedValue) ? 0 : Convert.ToInt32(ddlCraBranch.SelectedValue);
                string txtPopSpRegNoFolioValue = string.IsNullOrWhiteSpace(txtPopSpRegNo.Text) ? string.Empty : txtPopSpRegNo.Text;
                long businessRmValue = string.IsNullOrWhiteSpace(txtBusinessRm.Text) ? 0 : Convert.ToInt64(txtBusinessRm.Text);
                long businessBranchValue = string.IsNullOrWhiteSpace(ddlBusinessBranch.SelectedValue) ? 0 : Convert.ToInt64(ddlBusinessBranch.SelectedValue);
                string receiptNoUniueValue = string.IsNullOrWhiteSpace(txtReceiptNo.Text) ? string.Empty : txtReceiptNo.Text;
                char paymentModeValue = string.IsNullOrWhiteSpace(ddlPaymentMethod.SelectedValue) ? '\0' : Convert.ToChar(ddlPaymentMethod.SelectedValue);
                string chequeNoValue = string.IsNullOrWhiteSpace(txtChequeNo.Text) ? string.Empty : txtChequeNo.Text;
                string bankNameValue = ddlBankName.SelectedItem == null ? string.Empty : ddlBankName.SelectedItem.ToString();
                string appNoValue = string.IsNullOrWhiteSpace(ddlRequestType.SelectedValue) ? string.Empty : ddlRequestType.SelectedValue;
                DateTime? chequeDateValue = DateTimeParser.ParseDate(txtChequeDated.Text);
                DateTime? dateValue = DateTimeParser.ParseDate(txtDate.Text);
                DateTime? timeValue = DateTimeParser.ParseTime(txtTime.Text, "HH:mm");
                DateTime? combinedDateTime = null;

                if (dateValue.HasValue && timeValue.HasValue)
                {
                    // Combine date and time into one DateTime variable
                    combinedDateTime = new DateTime(
                        dateValue.Value.Year,
                        dateValue.Value.Month,
                        dateValue.Value.Day,
                        timeValue.Value.Hour,
                        timeValue.Value.Minute,
                        timeValue.Value.Second
                    );
                }

                // Use combinedDateTime as needed
                if (combinedDateTime.HasValue)
                {
                    // Do something with combinedDateTime.Value
                }
                else
                {
                    // Handle the case where the date or time was not provided
                    //Console.WriteLine("Date or time input was invalid or missing.");
                }

                string subInvNameValue = string.IsNullOrWhiteSpace(txtNameOfSubscriber.Text) ? string.Empty : txtNameOfSubscriber.Text;
                string praManualARNoValue = string.IsNullOrWhiteSpace(txtPran.Text) ? string.Empty : txtPran.Text;
                string unfreezValue = CheckBox1.Checked ? "Y" : "N";
                decimal amountT1Value = string.IsNullOrWhiteSpace(txtAmountReceivedTire1.Text) ? 0 : decimal.Parse(txtAmountReceivedTire1.Text);
                decimal amountT2Value = string.IsNullOrWhiteSpace(txtAmountReceivedTire2.Text) ? 0 : decimal.Parse(txtAmountReceivedTire2.Text);
                decimal regharge1Value = string.IsNullOrWhiteSpace(txtPopRegistrationChargesOneTime.Text) ? 0 : decimal.Parse(txtPopRegistrationChargesOneTime.Text);
                decimal regharge2Value = string.IsNullOrWhiteSpace(txtPopRegistrationCharges.Text) ? 0 : decimal.Parse(txtPopRegistrationCharges.Text);
                decimal gstTaxValue = string.IsNullOrWhiteSpace(txtGst.Text) ? 0 : decimal.Parse(txtGst.Text);
                decimal amountCollectionValue = string.IsNullOrWhiteSpace(txtCollectionAmount.Text) ? 0 : decimal.Parse(txtCollectionAmount.Text);
                decimal amountInvestedValue = string.IsNullOrWhiteSpace(txtAmountInvested.Text) ? 0 : decimal.Parse(txtAmountInvested.Text);
                decimal amountInvested2Value = string.IsNullOrWhiteSpace(txtAmountInvestedAdditional.Text) ? 0 : decimal.Parse(txtAmountInvestedAdditional.Text);
                string remarkValue = txtRemark.Text.ToString();
                string zeroComValue = chkZeroCommission.Checked ? "Y" : "N";
                string loggedinUser = Session["LoginId"]?.ToString();


                if (string.IsNullOrEmpty(schemeCodeValue))
                {
                    ShowAlert("Choose any pension schema");
                    ddlScheme.Focus();
                    return;
                }
                else
                {


                    if (schemeCodeValue == "OP#09971")
                    {
                        //txtAmountReceivedTire1 ==> amountT1Value
                        //txtAmountReceivedTire2 ==> amountT2Value
                        // Only t1 is required
                        if (string.IsNullOrWhiteSpace(txtAmountReceivedTire1.Text) || txtAmountReceivedTire1.Text ==  "0")
                        {
                            ShowAlert("Please enter a value for Tier 1.");
                            txtAmountReceivedTire1.Focus();
                            return;
                        }
                        else
                        {
                            amountT2Value = 0;

                        }
                    }

                    if (schemeCodeValue == "OP#09972")
                    {
                        // Only t2 is required
                        if (string.IsNullOrWhiteSpace(txtAmountReceivedTire2.Text) || txtAmountReceivedTire2.Text == "0")
                        {
                            ShowAlert("Please enter a value for Tier 2.");
                            txtAmountReceivedTire2.Focus();

                            return;
                        }
                        else
                        {
                            amountT1Value = 0;

                        }
                    }

                    if (schemeCodeValue == "OP#09973")
                    {
                        // Both t1 and t2 are required
                        if (string.IsNullOrWhiteSpace(txtAmountReceivedTire1.Text) || string.IsNullOrWhiteSpace(txtAmountReceivedTire2.Text))
                        {
                            ShowAlert("Please enter values for both Tier 1 and Tier 2.");
                            txtAmountReceivedTire1.Focus();

                            return;
                        }
                    }
                }

                string insertResult = new WM.Controllers.NpsTransactionPunchingController().InsertClientData(
                mark,
                prodValue,
                investorTypeValue,
                corporateNameValue,
                dtNumberValue,
                tranCodeValue,
                invertorCodeValue,
                schemeCodeValue,
                craValue,
                craBranchValue,
                txtPopSpRegNoFolioValue,
                businessRmValue,
                businessBranchValue,
                receiptNoUniueValue,
                paymentModeValue,
                chequeNoValue,
                bankNameValue,
                appNoValue,
                chequeDateValue,
                dateValue,
                timeValue,
                combinedDateTime,
                subInvNameValue,
                praManualARNoValue,
                unfreezValue,
                amountT1Value,
                amountT2Value,
                regharge1Value,
                regharge2Value,
                gstTaxValue,
                amountCollectionValue,
                amountInvestedValue,
                amountInvested2Value,
                remarkValue,
                zeroComValue,
                loggedinUser
                    );

                lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";

                lblMessage.Text = insertResult;
                lblMessage.Focus();
                //ClientScript.RegisterStartupScript(this.GetType(), "punchingAlert", $"alert('{insertResult}');", true);


                if (insertResult.Contains("successful"))
                {
                    string currentAR = lblArNo.Text.ToString();
                    string currentDT = txtDtNumber.Text.ToString();

                    if (!string.IsNullOrEmpty(currentAR))
                    {
                        // If the AR number or DT number is empty, we need to alert that the transaction was updated
                        if (insertResult.Contains("Updation successful"))
                        {
                            ResetMain();
                            if (!string.IsNullOrEmpty(currentAR))
                            {

                                DataTable dt = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, currentAR, false, Session["LoginId"]?.ToString());
                                if (dt.Rows.Count > 0)
                                {
                                    DataRow row = dt.Rows[0];
                                    SetARFieldData(row);
                                    ShowAlert(insertResult);
                                    lblMessage.Text = insertResult;
                                    lblMessage.Focus();

                                    string currentDT1 = currentDT;


                                }
                            }


                        }

                        else if (insertResult.Contains("Insertion successful"))
                        {
                            ShowAlert(insertResult);
                            lblMessage.Text = insertResult;

                        }
                    }

                }
                else
                {
                    ShowAlert(insertResult);
                }

            }
            catch (Exception ex)
            {
                // Handle any exception that occurs during the InsertClientData call
                string errorMessage = $"Error: Kindly fill form properly. {ex.Message}";
                lblMessage.CssClass = "message-label-error";
                lblMessage.Text = errorMessage;

                // Display alert for the exception
                ClientScript.RegisterStartupScript(this.GetType(), "punchingExceptionAlert", $"alert('{errorMessage}');", true);
            }


        }

        private void ShowSuccessModal(string message)
        {
            // Generate a unique name for localStorage
            string uniqueName = Guid.NewGuid().ToString();

            // Register JavaScript to show modal and handle session/localStorage
            string script = $@"
        if (!localStorage.getItem('{uniqueName}')) {{
            $('#modalMessage').text('{message}');
            $('#successModal').modal('show');
            localStorage.setItem('{uniqueName}', 'shown');
            
            // Auto-hide the modal after 3 seconds
            setTimeout(function() {{
                $('#successModal').modal('hide');
                sessionStorage.setItem('{uniqueName}_session', 'none');
            }}, 3000);
        }}
    ";

            // Inject the script into the page
            ShowAlert(message);

            ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);

            ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessModal", script, true);
        }


   




        #region btnSearch_Click

        protected void arPopSearch_Click(object sender, EventArgs e)
        {
            if (ARListARGridView.DataSource != null)
            {
                ARListARGridView.DataSource = null;
                ARListARGridView.DataBind(); // Rebind to refresh the GridView
                arPopSearch();
            }
            else
            {
                arPopSearch();


            }

        }

        protected void arPopSearch()
        {
            try
            {
                // Collect input values from the fields on the page
                string arNo = string.IsNullOrWhiteSpace(arPopArNo.Text) ? null : arPopArNo.Text.Trim();
                string appNo = string.IsNullOrWhiteSpace(arPopAppNo.Text) ? null : arPopAppNo.Text.Trim();
                string chequeNo = string.IsNullOrWhiteSpace(arPopChequeNo.Text) ? null : arPopChequeNo.Text.Trim();
                string pranNo = string.IsNullOrWhiteSpace(arPopPranNo.Text) ? null : arPopPranNo.Text.Trim();
                string scheme = string.IsNullOrWhiteSpace(arPopDdlSchemas.SelectedValue.ToString()) ? null : arPopDdlSchemas.SelectedValue.ToString();
                string invName = string.IsNullOrWhiteSpace(arPopInvName.Text) ? null : arPopInvName.Text.Trim();
                string anaExistCode = string.IsNullOrWhiteSpace(arPopAnaExistCode.Text) ? null : arPopAnaExistCode.Text.Trim();
                DateTime? arFromDate = string.IsNullOrWhiteSpace(arPopArFromDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(arPopArFromDate.Text, "dd/MM/yyyy", null);

                DateTime? arToDate = string.IsNullOrWhiteSpace(arPopArToDate.Text)
                    ? (DateTime?)null
                    : DateTime.ParseExact(arPopArToDate.Text, "dd/MM/yyyy", null);

                string arBefore = arPopRadioBefore.Checked ? "1" : null;

                // Check if all fields are null or empty
                if (string.IsNullOrWhiteSpace(arNo) &&
                    string.IsNullOrWhiteSpace(appNo) &&
                    string.IsNullOrWhiteSpace(chequeNo) &&
                    string.IsNullOrWhiteSpace(pranNo) &&
                    string.IsNullOrWhiteSpace(scheme) &&
                    string.IsNullOrWhiteSpace(invName) &&
                    string.IsNullOrEmpty(anaExistCode) &&
                    arFromDate == null && arToDate == null)
                {
                    // Reset the search result and show an error message if no fields are provided
                    string msg = "Please provide at least one field for the search!";
                    ARListlblMessage.Text = msg;
                    ShowAlert(msg);
                    arPopArNo.Focus();
                    return;
                }

                // Check if date fields are null and no checkbox is selected
                if (!arPopRadioBefore.Checked && !arPopRadioAfter.Checked) // Replace with actual checkbox variables
                {
                    // Reset the search result and show an error message if no fields are provided

                    string msg = "Please check at least one (Before/After) field for the search!";
                    ARListlblMessage.Text = msg;
                    ShowAlert(msg);
                    arPopRadioBefore.Focus();
                    return;
                }


                // Check for invalid date range (if To date is before From date)
                if (arFromDate.HasValue && arToDate.HasValue && arFromDate > arToDate)
                {
                    // Show error message if the "From" date is later than the "To" date
                    ARListlblMessage.Text = "The 'From' date cannot be later than the 'To' date!";
                    return;
                }






                // Call the controller method with the collected inputs
                DataTable dt = new WM.Controllers.NpsTransactionPunchingController().SearchARDetails(
                    arNo, appNo, chequeNo, pranNo, scheme,

                    invName, anaExistCode, arFromDate, arToDate, arBefore

                    );


                // Check if any records were returned
                if (dt == null || dt.Rows.Count < 1)
                {
                    // Reset the grid and show "No records found" message
                    arPopResetGrid_Click();
                    string msg = "No records found!";
                    ShowAlert(msg);

                    ARListlblMessage.Text = msg;
                }
                else
                {
                    // Display the total record count and bind the results to the grid
                    ARListlblMessage.Text = $"Total {dt.Rows.Count} {(dt.Rows.Count == 1 ? "record" : "records")} found!";
                    ARListARGridView.Visible = true;
                    ARListARGridView.DataSource = dt;
                    ARListARGridView.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or show an error message to the user
                //ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('An error occurred: {ex.Message}');", true);
                ARListlblMessage.Text = $"An error occurred: {ex.Message}'";

            }
        }

        #endregion

        protected void ARListARGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectTransaction")
            {
                // Retrieve the TRAN_CODE from CommandArgument
                string tranCode = e.CommandArgument.ToString();

                // Set the text of the label
                ARListlblMessage.Text = " " + tranCode;
                lblArNo.Text = tranCode;
            }
        }

       

        #region ARSearchList

        #region gvARSearch_RowCommand
        protected void gvARSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                // Retrieve the TRAN_CODE from CommandArgument
                string tranCode = e.CommandArgument.ToString();

                ResetMain();

                try
                {

                    bool beforeAfterFlags = false;

                    beforeAfterFlags  = arPopRadioBefore.Checked ? true : false;

                    if (!string.IsNullOrEmpty(tranCode))
                    {

                        DataTable dtIfArExistForTempTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, true, Session["LoginId"]?.ToString());
                        DataTable dtIfArExistForStTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, false, Session["LoginId"]?.ToString());

                        int temDataCount = dtIfArExistForTempTran.Rows.Count > 0 ? 1 : 0;
                        int stDataCount = dtIfArExistForStTran.Rows.Count > 0 ? 1 : 0;


                        // Check for AR in temp transaction table
                        if (temDataCount == 0) // No data in temp
                        {
                            if (stDataCount == 1) // Data exists in st
                            {
                                string dbMsg1 = dtIfArExistForStTran.Rows[0]["message"].ToString();
                                if (dbMsg1.Contains("Validity: Transaction data exist in st"))
                                {
                                    DataRow row3 = dtIfArExistForStTran.Rows[0];
                                    SetARFieldData(row3);
                                    ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);
                                    arPopReset();
                                    btnSave.Enabled = false;
                                    btnModify.Enabled = true;
                                    txtBusinessRm.Enabled = false;


                                }
                            }
                            else
                            {

                                ShowAlert("No data exist!");

                            }
                        }
                        else
                        {
                            // Data exists in temp, extract values as needed
                            string dbMsg1 = dtIfArExistForTempTran.Rows[0]["message"].ToString();
                            if (dbMsg1.Contains("Validity: Transaction data exist in temp"))
                            {
                                DataRow row3 = dtIfArExistForTempTran.Rows[0];
                                //ShowAlert(dbMsg1);
                                SetARFieldData(row3);
                                ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);
                                arPopReset();
                                btnSave.Enabled = false;
                                btnModify.Enabled = true;
                            }
                        }
                    }
                   
                }

                catch (Exception ex)
                {
                    //btnSave.Enabled = true;
                    //btnModify.Enabled = false;

                }



            }
        }
        #endregion



        #endregion


        private void ProcessTransaction()
        {
            // Initialize variables
            double lbtrancode = 0;
            double TranAmount = 0;
            double TxtTire1 = 0;
            double TxtTire2 = 0;
            double txtpopregistration1 = 0;
            double txtpopregistration2 = 0;
            string txtregistrationno = txtPopSpRegNo.Text.ToString();
            string cboRequestType = ddlRequestType.SelectedValue.ToString();
            string VarScheme = ddlScheme.SelectedValue.ToString();
            DateTime DtDate = DateTime.MinValue;

            // Try parsing values from the controls
            if (!double.TryParse(lblArNo.Text.ToString(), out lbtrancode))
            {
                // Handle invalid lbtrancode (log, alert, or default to zero)
                lbtrancode = 0;
            }

            if (!double.TryParse(txtAmountInvested.Text.ToString(), out TranAmount))
            {
                // Handle invalid TranAmount (log, alert, or default to zero)
                TranAmount = 0;
            }

            if (!double.TryParse(txtAmountReceivedTire1.Text.ToString(), out TxtTire1))
            {
                // Handle invalid TxtTire1 (log, alert, or default to zero)
                TxtTire1 = 0;
            }

            if (!double.TryParse(txtAmountReceivedTire2.Text.ToString(), out TxtTire2))
            {
                // Handle invalid TxtTire2 (log, alert, or default to zero)
                TxtTire2 = 0;
            }

            if (!double.TryParse(txtPopRegistrationChargesOneTime.Text.ToString(), out txtpopregistration1))
            {
                // Handle invalid txtpopregistration1 (log, alert, or default to zero)
                txtpopregistration1 = 0;
            }

            if (!double.TryParse(txtPopRegistrationCharges.Text.ToString(), out txtpopregistration2))
            {
                // Handle invalid txtpopregistration2 (log, alert, or default to zero)
                txtpopregistration2 = 0;
            }

            if (!DateTime.TryParse(txtDate.Text.ToString(), out DtDate))
            {
                // Handle invalid DtDate (log, alert, or default to DateTime.MinValue)
                DtDate = DateTime.MinValue;
            }

            // Skip processing if certain conditions are met
            if (lbtrancode > 0 && cboRequestType == "12" && TranAmount == 0)
            {
                return;
            }

            // Check for empty registration number
            if (string.IsNullOrEmpty(txtregistrationno))
            {
                // Display an error message or handle as needed
                return;
            }

            // Handle enabling/disabling fields based on request type and scheme
            if (cboRequestType == "11" || cboRequestType == "12")
            {
                if (VarScheme == "OP#09971")
                {
                    txtAmountReceivedTire1.Enabled = true;
                    txtAmountReceivedTire2.Enabled = false;
                }
                else if (VarScheme == "OP#09972")
                {
                    txtAmountReceivedTire2.Enabled = true;
                    txtAmountReceivedTire1.Enabled = false;
                }
            }

            // Handle pop registration calculations
            if (cboRequestType == "11" && TxtTire1 == 0)
            {
                txtpopregistration1 = 0;
            }
            else
            {
                if (cboRequestType == "11" || cboRequestType == "12")
                {
                    double FreshContri = TxtTire1 * 0.0025;
                    FreshContri = Math.Max(20, Math.Min(FreshContri, 25000));

                    if (TxtTire1 > 0)
                    {
                        txtpopregistration1 = DtDate >= new DateTime(2017, 11, 1) ? (200 + FreshContri) : (100 + FreshContri);
                    }
                    else
                    {
                        txtpopregistration1 = 0;
                    }

                    // Handle specific conditions for TxtTire2
                    if (VarScheme == "OP#09972" || VarScheme == "OP#09973")
                    {
                        txtAmountReceivedTire2.Enabled = true;
                        txtpopregistration2 = TxtTire2 == 0 ? 0 : 0; // Modify this logic as needed
                    }
                }
            }

            // Additional calculations for TxtTire2
            if (TxtTire2 != 0 && cboRequestType == "11")
            {
                txtpopregistration2 = Math.Max(20, TxtTire2 * 0.0025);
            }

            // Update the values back to the controls if necessary
            txtPopRegistrationChargesOneTime.Text = txtpopregistration1.ToString("F2");
            txtPopRegistrationCharges.Text = txtpopregistration2.ToString("F2");
        }


        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("NPS_tran");

                    // Set headers
                    string[] headers = new string[] {
                    "PoP NO", "RRAN No", "Client Name", "Payment Mode", "Cheque Number",
                    "Date", "Bank Name", "Receipt no", "Tier 1", "Tier 2",
                    "Reg Charges", "Tran Charges", "GST", "Amount Invested",
                    "Receipt no(10-17)", "Fc Registration No", "Remarks",
                    "Remarks1", "Tran_Code", "Ref_Tran_Code", "CSF_TRANSACTION_ID"
                };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    // Get data from database
                    using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand("PKG_NPS_EXPORT_PRA.GET_NPS_TRANSACTIONS_PRA", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            cmd.Parameters.Add("p_from_date", OracleDbType.Varchar2).Value = fromDate.Text;
                            cmd.Parameters.Add("p_to_date", OracleDbType.Varchar2).Value = toDate.Text;
                            cmd.Parameters.Add("p_transaction_type", OracleDbType.Varchar2).Value = rblTransactionType.SelectedValue;
                            cmd.Parameters.Add("p_cra_type", OracleDbType.Varchar2).Value = cra.SelectedValue;
                            cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                int row = 2;
                                while (reader.Read())
                                {
                                    for (int col = 0; col < reader.FieldCount; col++)
                                    {
                                        worksheet.Cells[row, col + 1].Value = reader[col]?.ToString() ?? "";
                                    }
                                    row++;
                                }
                            }
                        }
                    }

                    // Format and autofit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Prepare response
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=NPS_Transaction_Export.xlsx");

                    // Write to response stream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        package.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                    }
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                // Log error and show user-friendly message
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('An error occurred while exporting data. Please try again.');", true);
            }
        }



        #region NPSECS Model
        // Submit Button Click Event

        protected void UploadButton_Click(object sender, EventArgs e)
        {

            if (!NpsEcsFileInput.HasFile)
            {
                ShowAlert("Kindly choose an excel file then press on upload!");
                NpsEcsFileInput.Focus();
                return;
            }

            if (NpsEcsFileInput.HasFile)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + NpsEcsFileInput.FileName;
                string uploadPath = Server.MapPath("~/Uploads/");
                string filePath = Path.Combine(uploadPath, fileName);

                // Create uploads directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                try
                {
                    NpsEcsFileInput.SaveAs(filePath);
                    Session["CurrentNpsEcsExcelFile"] = filePath;

                    // Load sheets into dropdown
                    LoadExcelSheets(filePath, npsEcsDdlSheetlist);

                    //  PopulateSheetsFromExcel(filePath);
                    npsEcsLblFileName.Text = "Uploaded File: " + NpsEcsFileInput.FileName;
                    npsEcsDdlSheetlist.Focus();
                    ShowAlert("File uploaded successfully");

                }
                catch (Exception ex)
                {

                    ShowAlert(ex.Message);
                }
            }
        }

        public static void LoadExcelSheets(string filePath, DropDownList sheetDropDown)
        {

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString_N(filePath)))
            {
                conn.Open();
                DataTable dtSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                sheetDropDown.Items.Clear();
                sheetDropDown.Items.Add(new ListItem("-- Select Sheet --", ""));

                foreach (DataRow row in dtSheets.Rows)
                {
                    string sheetName = row["TABLE_NAME"].ToString();
                    // Remove $ from sheet name that Excel adds
                    sheetName = sheetName.Replace("$", "").Replace("'", "");
                    sheetDropDown.Items.Add(new ListItem(sheetName, sheetName));
                }
            }
        }

        protected void NpsEcsExcelSheetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filePath = Session["CurrentNpsEcsExcelFile"]?.ToString();
            string selectedSheet = npsEcsDdlSheetlist.SelectedValue;
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(selectedSheet))
            {
                try
                {


                    GridView1.DataSource = LoadSheetData(filePath, selectedSheet);
                    GridView1.DataBind();
                    npsEcsSuccessMessage.Text = $"Selected sheet: " + selectedSheet;
                    npsEcsSuccessMessage.Visible = true;

                }
                catch (Exception ex)
                {
                    ShowAlert("Error loading sheet data: " + ex.Message);
                    npsEcsSuccessMessage.Text = ex.Message;
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                }
            }
            else
            {
                ShowAlert("File or sheet not present. ");
            }
        }

        public static DataTable LoadSheetData(string filePath, string sheetName)
        {
            DataTable dtResult = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString_N(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(dtResult);
                    }
                }
            }

            return dtResult;
        }



        private static string GetExcelConnectionString_N(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            string connectionString = string.Empty;

            if (extension == ".xls")
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
            }
            else if (extension == ".xlsx")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1';";
            }

            return connectionString;
        }


        protected void NpsEcsSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Get user inputs
                string selectedValue1 = npsEcsDdlCompany.SelectedValue;
                string inputFieldValue = npsEcsInputField.Text;
                string selectedValue2 = npsEcsDdlSheetlist.SelectedValue;


                // Validate session file
                if (Session["CurrentNpsEcsExcelFile"] == null)
                {
                    ShowAlert("File does not exist kindly upload.");
                    npsEcsLblFileName.Focus();
                    return;
                }


                string selectedFilePath = Session["CurrentIPOExcelFile"]?.ToString();

                string selectedSheet = npsEcsDdlSheetlist.SelectedValue;

                DataTable excelData = GetGridViewData(selectedFilePath, selectedSheet);
                string getCompany = npsEcsDdlCompany.SelectedValue;
                string getLoginID = Session["LoginId"]?.ToString();


              
                try
                {
                    //int importSummary = WM.Controllers.NpsTransactionPunchingController,ImportDataToDatabase(excelData, getCompany, getLoginID);
                    int importSummary = 1;
                    ShowAlert($"Total inserted row(s): {importSummary}");
                    lblMessage.Text = $"Total inserted row(s): {importSummary}";
                }
                catch (Exception ex)
                {
                    // Handle unexpected errors
                    lblMessage.Text = $"An error occurred: {ex.Message}";
                    ShowAlert("An error occurred while importing data. Please try again.");
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                npsEcsSuccessMessage.Visible = true;
                npsEcsSuccessMessage.Text = $"An error occurred: {ex.Message}";
                npsEcsSuccessMessage.CssClass = "text-danger"; // Highlight error in red
            }
        }

        // Reset Button Click Event
        protected void NpsEcsReset_Click(object sender, EventArgs e)
        {
            Session["CurrentNpsEcsExcelFile"] = null;
            GridView1.DataSource = null;
            GridView1.DataBind();
            npsEcsSuccessMessage.Text = string.Empty;

            // Clear all inputs
            npsEcsDdlSheetlist.SelectedIndex = 0;
            npsEcsInputField.Text = string.Empty;
            npsEcsDdlCompany.SelectedIndex = 0;
            npsEcsLblFileName.Text = string.Empty;
            // Hide success/error message
            npsEcsSuccessMessage.Visible = false;
        }

        // Other Utility Methods (if needed)
        protected void NpsEcsExit_Click(object sender, EventArgs e)
        {
            // Inject JavaScript to close the modal
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeModalNpsEcsModel();", true);
        }


        private DataTable GetGridViewData(string filePath, string sheetName)
        {
            // Use the ReadExcelData method to get the initial DataTable
            DataTable rawData = ReadExcelData(filePath, sheetName);
            DataTable cleanData = new DataTable();

            // Check if rawData has columns
            if (rawData.Columns.Count > 0)
            {
                // Trim spaces from column names and add them to cleanData
                foreach (DataColumn column in rawData.Columns)
                {
                    string trimmedColumnName = column.ColumnName.Trim(); // Trim spaces
                    cleanData.Columns.Add(trimmedColumnName);
                }

                // Add rows to cleanData
                foreach (DataRow row in rawData.Rows)
                {
                    DataRow newRow = cleanData.NewRow();
                    foreach (DataColumn column in rawData.Columns)
                    {
                        newRow[column.ColumnName.Trim()] = row[column]; // Keep the trimmed column name
                    }
                    cleanData.Rows.Add(newRow);
                }
            }

            return cleanData;
        }

        private DataTable ReadExcelData(string filePath, string sheetName)
        {
            DataTable dataTable = new DataTable();

            string connString = GetExcelConnectionString_N(filePath);
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {

                    // Select all data from the sheet

                    // Ensure that the sheetName does not include the $ symbol and append it only if necessary
                    string query = $"SELECT * FROM [{sheetName.TrimEnd('$')}$]";

                    //string query = $"SELECT * FROM [{sheetName}$]";

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, conn);
                    dataAdapter.Fill(dataTable);

                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;

                }
            }

            return dataTable;
        }



        #endregion
    }
}