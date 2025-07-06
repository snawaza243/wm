
using Microsoft.Ajax.Utilities;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media.TextFormatting;
using WM.Controllers;
using System.Text.RegularExpressions;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Collections;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using System.Web.Mail;
using System.Configuration;
using MathNet.Numerics.Distributions;


namespace WM.Masters
{
    public partial class AccountOpening : Page
    {
        string genSourceID;
        string soruceID;
        protected void Page_Load(object sender, EventArgs e)
        {
            string baseUrl = ConfigurationManager.AppSettings["loginPage"];
            if (Session["LoginId"] == null)
            {
                Response.Redirect($"{baseUrl}");
                //Response.Redirect("~/index.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    //EnableDisablField(false);
                    PopulateInvestorBranchDropDown();
                    FillFamilyRelationList();
                    FillSalutationListDropDown();
                    fillOccupationList();
                    fillClientCategoryList();
                    fillCountryList();
                    fillStateList();
                    fillCityList();
                    fillBankMasterDetails();
                    ClientListFillCityList();
                    ClientListFillBranchList();
                    FillMutualFundDropdown();

                    AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                    AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                    AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                    AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");


                    if (Session["IsAddressCopied"] != null && (bool)Session["IsAddressCopied"])
                    {
                        CheckBox1.Checked = true;
                        var addressData = Session["PermanentAddress"] as dynamic;
                        if (addressData != null)
                        {
                            txtPAddress1.Text = addressData.Address1;
                            txtPAddress2.Text = addressData.Address2;
                            ddlPCountryList.SelectedValue = addressData.Country;
                            ddlPStateList.SelectedValue = addressData.State;
                            ddlPCityList.SelectedValue = addressData.City;
                            txtPPin.Text = addressData.Pin;
                            string currentAddCountry = ddlPCountryList.SelectedValue.ToString();
                            HandlePinCodeValidation(currentAddCountry, txtPPin);
                        }
                    }
                } 
            }
            if (Request.QueryString["stid"] != null)
            {
                string payrollId = Request.QueryString["stid"];
                //txtBusinessCode.Text = payrollId;
                //FillClientDataByAHNum(payrollId);
            }
        }


        protected void btnInsertAdvisoryTransaction_Click(object sender, EventArgs e)
        {
             
                try
                {
                    // Convert Source ID and Business Code to numeric values
                    int sourceId = string.IsNullOrEmpty(txtHeadSourceCode.Text) ? 0 : Convert.ToInt32(txtHeadSourceCode.Text);
                    int businessCode = string.IsNullOrEmpty(txtBusinessCode.Text) ? 0 : Convert.ToInt32(txtBusinessCode.Text);

                #region PlanType
                string planTypeFR = "";

                if (fresh.Checked)
                {
                    planTypeFR = "Fresh";
                }
                else if (renewal.Checked)
                {
                    planTypeFR = "Renewal";
                }
                #endregion

                string mutCode = "";  // First part (mut_code)
                string schemeCode = "";  // Second part (sch_code)
                string schName = "";  // Third part (SCH_NAME)
                string figure = "";   // Fourth part (figure)

                // Get the selected value from the dropdown
                string selectedPlanTypeValue = ddlMutualPlanType.SelectedValue;

                if (!string.IsNullOrEmpty(selectedPlanTypeValue))
                {
                    // Split the value using '$' as the delimiter
                    string[] parts = selectedPlanTypeValue.Split('$');
                    if (parts.Length >= 4)
                    {
                        mutCode = parts[0];    // First part (mut_code)
                        schemeCode = parts[1]; // Second part (sch_code)
                        schName = parts[2];    // Third part (SCH_NAME)
                        figure = parts[3];     // Fourth part (figure)
                    }
                }

                string chequeNo = string.IsNullOrEmpty(txtChequeNo.Text) ? null : txtChequeNo.Text;
                DateTime? chequeDate = DateTime.TryParse(txtChequeDate.Text, out var parsedChequeDate) ? (DateTime?)parsedChequeDate : null;
                string bankName = ddlMutualBank.SelectedItem.Text;
                double amount = string.IsNullOrEmpty(txtAmount.Text) ? 0 : Convert.ToDouble(txtAmount.Text);
                int rmCode = string.IsNullOrEmpty(txtBusinessCode.Text) ? 0 : Convert.ToInt32(txtBusinessCode.Text);
                int branchCode = string.IsNullOrEmpty(txtBusinessCode.Text) ? 0 : Convert.ToInt32(txtBusinessCode.Text);
                string loggedInUser = Session["LoginId"]?.ToString();
                string remark = string.IsNullOrEmpty(txtRemark.Text) ? null : txtRemark.Text;



                string[] partsChequeDraftName = ChequeLabel.Text.Split('<');
                string chequeDraftLabelDynamic = "";

                if (partsChequeDraftName.Length > 0)
                {
                    chequeDraftLabelDynamic = partsChequeDraftName[0]; // Extracts the text before '<'
                }


                // Field validation
                if (sourceId == 0)
                {
                    string message = "Source is empty, Load a client.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        return;
                    }

                    if (businessCode == 0)
                    {
                        string message = "Business Code is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        return;
                    }

                if (string.IsNullOrEmpty(selectedPlanTypeValue))
                {
                    string message = "Mutual Plan is empty.";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    ddlMutualPlanType.Focus();
                    return;
                }


                if (string.IsNullOrEmpty(bankName))
                {
                    string message = "Bank is empty.";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    ddlMutualBank.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtAmount.Text))
                {
                    string message = "Amount is empty.";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtAmount.Focus();
                    return;
                }


                if (!fresh.Checked && !renewal.Checked)
                {
                    string message = "Please select either Fresh or Renewal for Plan Type.";
                    ShowAlert(message);  
                    lblMessage.Text = message;  
                    lblMessage.CssClass = "message-label-error";
                    fresh.Focus();
                    return; 
                }

                if (!cheque.Checked && !draft.Checked)
                {
                    string message = "Please select either Cheque or Draft.";
                    ShowAlert(message);  
                    lblMessage.Text = message;  
                    lblMessage.CssClass = "message-label-error";
                    cheque.Focus();
                    return;  
                }

                if (cheque.Checked || draft.Checked)
                {
                    if (string.IsNullOrEmpty(txtChequeDate.Text))
                    {

                    string message = chequeDraftLabelDynamic + " date is empty.";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    txtChequeDate.Focus();
                    return;
                    }
                }



                if (cheque.Checked || draft.Checked)
                {


                    if (string.IsNullOrEmpty(chequeNo))
                    {
                        string message = chequeDraftLabelDynamic + " is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        lblMessage.CssClass = "message-label-error"; // Optional: Add error styling
                        txtChequeNo.Focus(); // Focus on the Cheque No input field
                        return; // Exit method
                    }
                }


                // Call the InsertAdvisoryTransaction method from the appropriate controller
                string insertResult = new WM.Controllers.AccountOpeningController().InsertAdvisoryTransaction(
                        sourceId,
                        businessCode,
                        planTypeFR,
                        chequeNo,
                        chequeDate,
                        bankName,
                        amount,
                        mutCode,
                        schemeCode,
                        rmCode,
                        branchCode,
                        loggedInUser,
                        remark
                    );

                    // Handle success or error message from insertResult
                    if (insertResult.Contains("Successfully"))
                    {
                        // Handle success
                        ShowAlert("Valid: " + insertResult);
                        lblMessage.Text = insertResult;
                        lblMessage.CssClass = "message-label-success";
                    }
                    else
                    {
                        // Handle error
                        ShowAlert(insertResult);
                        lblMessage.Text = insertResult;
                        lblMessage.CssClass = "message-label-error";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exception that occurs during the InsertAdvisoryTransaction call
                    string errorMessage = $"Error occurred: {ex.Message}";
                    lblMessage.CssClass = "message-label-error";
                    lblMessage.Text = errorMessage;

                    // Display alert for the exception
                    ShowAlert(errorMessage);
                }


            

        }

        protected void btnUpdateAdvisoryTransaction_Click(object sender, EventArgs e)
        {
            // Get data from the stored procedure
            DataTable dt = new AccountOpeningController().GetAdvisoryDataByAH(txtSearchClientCode.Text, txtSearchPan.Text);

            if (dt == null || dt.Rows.Count == 0)
            {
                ShowAlert("No Transaction AH Data found");
                return;
            }

            else if (dt.Rows.Count > 0)
            {

                DataRow row = dt.Rows[0];

                // Retrieve and handle the 'message' column or any other relevant column
                string MyTranCode = row["tran_code"] != DBNull.Value ? row["tran_code"].ToString() : string.Empty;
                string MyPrintSourceId = row["source_code"] != DBNull.Value ? row["source_code"].ToString() : string.Empty;
                DateTime MyTrDate;

                if (row["tr_date"] != DBNull.Value)
                {
                    MyTrDate = Convert.ToDateTime(row["tr_date"]);
                }
                else
                {
                    MyTrDate = DateTime.MinValue; // Or use a default date, such as DateTime.Now or another appropriate value
                }

                string BusinessCode = txtBusinessCode.Text;
                string MySourceId = txtHeadSourceCode.Text;


                try
                {
                    // Convert Source ID and Business Code to numeric values
                    string tranCode = MyTranCode;
                    string mutCode = "";  // First part (mut_code)
                    string schemeCode = "";  // Second part (sch_code)
                    string schName = "";  // Third part (SCH_NAME)
                    string figure = "";   // Fourth part (figure)

                    // Get the selected value from the dropdown
                    string selectedPlanTypeValue = ddlMutualPlanType.SelectedValue;

                    if (!string.IsNullOrEmpty(selectedPlanTypeValue))
                    {
                        // Split the value using '$' as the delimiter
                        string[] parts = selectedPlanTypeValue.Split('$');
                        if (parts.Length >= 4)
                        {
                            mutCode = parts[0];    // First part (mut_code)
                            schemeCode = parts[1]; // Second part (sch_code)
                            schName = parts[2];    // Third part (SCH_NAME)
                            figure = parts[3];     // Fourth part (figure)
                        }
                    }

                    // Get other input values
                    double amount = string.IsNullOrEmpty(txtAmount.Text) ? 0 : Convert.ToDouble(txtAmount.Text);
                    string chequeNo = string.IsNullOrEmpty(txtChequeNo.Text) ? null : txtChequeNo.Text;
                    DateTime? chequeDate = DateTime.TryParse(txtChequeDate.Text, out var parsedChequeDate) ? (DateTime?)parsedChequeDate : null;
                    string bankName = ddlMutualBank.SelectedItem.Text;
                    string remark = string.IsNullOrEmpty(txtRemark.Text) ? null : txtRemark.Text;
                    string loggedInUser = Session["LoginId"]?.ToString();

                    // Field validation
                    if (string.IsNullOrEmpty(tranCode))
                    {
                        string message = "Transaction Code is empty, Load the transaction first.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        return;
                    }

                    if (string.IsNullOrEmpty(selectedPlanTypeValue))
                    {
                        string message = "Mutual Plan is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        ddlMutualPlanType.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(txtAmount.Text))
                    {
                        string message = "Amount is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        txtAmount.Focus();
                        return;
                    }

                    if (string.IsNullOrEmpty(bankName) && (cheque.Checked || draft.Checked))
                    {
                        string message = "Bank is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        ddlMutualBank.Focus();
                        return;
                    }

                    if ((cheque.Checked || draft.Checked) && string.IsNullOrEmpty(bankName))
                    {
                        string message = "Mutual Bank is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        lblMessage.CssClass = "message-label-error";  // Optional: To show error styling
                        ddlMutualBank.Focus();
                        return;
                    }



                    if (string.IsNullOrEmpty(chequeNo) && !optCash.Checked)
                    {
                        string message = "Cheque No. is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        txtChequeNo.Focus();
                        return;
                    }


                    if (string.IsNullOrEmpty(txtChequeDate.Text) && !optCash.Checked)
                    {
                        string message = "Cheque Date is empty.";
                        ShowAlert(message);
                        lblMessage.Text = message;
                        txtChequeDate.Focus();
                        return;
                    }

                    if (optCash.Checked)
                    {

                        bankName = "Cash";
                        chequeNo = "";
                        chequeDate = null;

                        if (string.IsNullOrWhiteSpace(remark))
                        {
                            string message = "Remark is empty.";
                            ShowAlert(message);
                            lblMessage.Text = message;
                            txtRemark.Focus();
                            return;

                        }

                    }


                    // Call the UpdateAdvisoryTransaction method from the appropriate controller
                    string updateResult = new WM.Controllers.AccountOpeningController().UpdateAdvisoryTransaction(
                        tranCode,
                        mutCode,
                        schemeCode,
                        amount,
                        chequeNo,
                        chequeDate,
                        bankName,
                        remark,
                        loggedInUser,
                        optCash.Checked,
                        cheque.Checked,
                        draft.Checked
                    );

                    // Handle success or error message from updateResult
                    if (updateResult.Contains("Successfully"))
                    {
                        // Handle success
                        ShowAlert("Valid: " + updateResult);
                        lblMessage.Text = updateResult;
                        lblMessage.CssClass = "message-label-success";
                    }
                    else
                    {
                        // Handle error
                        ShowAlert(updateResult);
                        lblMessage.Text = updateResult;
                        lblMessage.CssClass = "message-label-error";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exception that occurs during the UpdateAdvisoryTransaction call
                    string errorMessage = $"Error occurred: {ex.Message}";
                    lblMessage.CssClass = "message-label-error";
                    lblMessage.Text = errorMessage;

                    // Display alert for the exception
                    ShowAlert(errorMessage);
                }

            }
        }

        private void FillAdvisoryDataByAH(string clientCode, string clientPan)
        {
            try
            {
                // Get data from the stored procedure
                DataTable dt = new AccountOpeningController().GetAdvisoryDataByAH(clientCode, clientPan);

                // Check if the DataTable is null or empty
                if (dt == null || dt.Rows.Count <= 0)
                {
                    txtClientAdvisoryInfo.Text = "Advisory data not found for this client";

                }
                else if (dt.Rows.Count > 0)
                {
                    // Get the first row of the result
                    DataRow row = dt.Rows[0];
                    // Retrieve and handle the 'message' column or any other relevant column
                    string message = row["message"] != DBNull.Value ? row["message"].ToString() : string.Empty;
                    // Check if the message contains valid data
                    if (message.Contains("Valid Data"))
                    {
                        ResetAdvisoryPackageFields();
                        txtClientAdvisoryInfo.Text = message;
                        SetAdvisoryFieldData(row);
                    }
                }


            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
               // ShowAlert("An error occurred: " + ex.Message);
            }
        }

        protected void btnPrintARReport_Click(object sender, EventArgs e)
        {
            // Get data from the stored procedure
            DataTable dt = new AccountOpeningController().GetAdvisoryDataByAH(txtSearchClientCode.Text, txtSearchPan.Text);

            if (dt == null || dt.Rows.Count == 0)
            {
                ShowAlert("No Transaction AH Data found");
                return;
            }
            DataRow row = dt.Rows[0];

            // Retrieve and handle the 'message' column or any other relevant column
            string message = row["message"] != DBNull.Value ? row["message"].ToString() : string.Empty;
            string MyTranCode = row["tran_code"] != DBNull.Value ? row["tran_code"].ToString() : string.Empty;
            string MyPrintSourceId = row["source_code"] != DBNull.Value ? row["source_code"].ToString() : string.Empty;
            DateTime MyTrDate;

            if (row["tr_date"] != DBNull.Value)
            {
                MyTrDate = Convert.ToDateTime(row["tr_date"]);
            }
            else
            {
                MyTrDate = DateTime.MinValue; // Or use a default date, such as DateTime.Now or another appropriate value
            }

            string BusinessCode = txtBusinessCode.Text;
            string MySourceId = txtHeadSourceCode.Text;

            try
            {
                /*
                // Validate the required fields before calling the stored procedure
                if (string.IsNullOrEmpty(txtTranCode.Text))
                {
                    ShowAlert("Transaction Code is empty.");
                    lblMessage.Text = "Transaction Code is empty.";
                    lblMessage.CssClass = "message-label-error";
                    txtTranCode.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtBusinessCode.Text))
                {
                    ShowAlert("Business Code is empty.");
                    lblMessage.Text = "Business Code is empty.";
                    lblMessage.CssClass = "message-label-error";
                    txtBusinessCode.Focus();
                    return;
                }

                // Convert input values
                string MyTranCode = txtTranCode.Text.Trim();
                string MyPrintSourceId = string.IsNullOrEmpty(txtPrintSourceId.Text) ? null : txtPrintSourceId.Text.Trim();
                DateTime MyTrDate = DateTime.Now; // Assuming the current date is used. You can adjust based on your needs
                string txtbusicode = txtBusinessCode.Text.Trim();
                string MySourceId = MyPrintSourceId;


                */
                
                
                
                DataTable reportData = new WM.Controllers.AccountOpeningController().GetARReport(MyTranCode, MyPrintSourceId, MyTrDate, txtBusinessCode.Text, MySourceId);

                // Check if any data is returned
                if (reportData.Rows.Count > 0)
                {
                    // Success: Data fetched and report is ready
                    // Proceed to show or print the report
                    LoadReportData(reportData);
                    CallPrintDiv();

             
                    // ShowAlert("Report data retrieved successfully.");
                    lblMessage.Text = "Report data retrieved successfully.";
                    lblMessage.CssClass = "message-label-success";

                    // You can now process the DataTable (reportData) to display or print the report as per your requirements
                    // Example: Bind the data to a GridView, export to Excel, or generate a PDF/print view
                }
                else
                {
                    // Handle the case where no data is found
                    ShowAlert("No data found for the provided criteria.");
                    lblMessage.Text = "No data found for the provided criteria.";
                    lblMessage.CssClass = "message-label-error";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the data retrieval process
                string errorMessage = $"Error occurred: {ex.Message}";
                ShowAlert(errorMessage);
                lblMessage.Text = errorMessage;
                lblMessage.CssClass = "message-label-error";
            }
        }

        private void LoadReportData(DataTable reportData)
        {
           
            if (reportData != null && reportData.Rows.Count > 0)
            {
                // Fill the header details (assuming first row has general info)
                DataRow headerRow = reportData.Rows[0];
                litDoctorName.Text = headerRow["client"].ToString();
                litAddress.Text = $"{headerRow["add1"]}, {headerRow["add2"]}, {headerRow["loc"]}, {headerRow["ccity"]}, {headerRow["pin"]}";
                litPhone.Text = headerRow["ph"].ToString();
                litEmail.Text = headerRow["email"].ToString();
                string cc = headerRow["client_code"].ToString();

                litClientNewOld.Text = cc;
                litTranDate.Text = headerRow["tr_date"].ToString();

                // Bind transaction details to Repeater
                rptTransactions.DataSource = reportData;
                rptTransactions.DataBind();

                // Calculate and display the total amount
                decimal totalAmount = 0;
                foreach (DataRow row in reportData.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["amount"]);
                }
                litTotal.Text = totalAmount.ToString("N2");

                // Footer details
                litPrintedBy.Text = headerRow["RNAME"].ToString();
                litBranch.Text = headerRow["BNAME"].ToString();
                litCompany.Text = headerRow["compmf"].ToString();
                litPrintedDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            }
            else
            {
                // Handle no data case
                litDoctorName.Text = "No data available";
                rptTransactions.DataSource = null;
                rptTransactions.DataBind();
            }
        }

        protected void ResetAdvisoryPackageFields()
        {
            // Reset Dropdowns
            ddlMutualPlanType.SelectedIndex = 0;
            ddlMutualBank.SelectedIndex = 0;

            // Reset TextBoxes
            txtRemark.Text = string.Empty;
            txtAmount.Text = string.Empty;
            txtChequeDate.Text = string.Empty;
            txtChequeNo.Text = string.Empty;

            // Reset Radio Buttons
            fresh.Checked = false;
            renewal.Checked = false;
            cheque.Checked = false;
            draft.Checked = false;

            // Reset Labels (if needed)
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = string.Empty;
        }

        protected void CallPrintDiv()
        {
            string divId = "arPrintDataForPrintWindow"; // The ID of the div to print
            string script = $"printDiv('{divId}');";
            ClientScript.RegisterStartupScript(this.GetType(), "PrintDivScript", script, true);
        }

        protected void PaymentTypeChanged(object sender, EventArgs e)
        {
            if (cheque.Checked)
            {
                ChequeLabel.Text = "Cheque No <span class='text-danger'>*</span>";
                ChequeDatedLabel.Text = "Cheque Dated <span class='text-danger'>*</span>";
                txtChequeDate.Focus();
                ddlMutualBank.Enabled = true;
                txtChequeDate.Enabled = true;
                txtChequeNo.Enabled = true;

            }
            else if (draft.Checked)
            {
                ChequeLabel.Text = "Draft No <span class='text-danger'>*</span>";
                ChequeDatedLabel.Text = "Draft Dated <span class='text-danger'>*</span>";
                
                txtChequeDate.Focus();
                ddlMutualBank.Enabled = true;
                txtChequeDate.Enabled = true;
                txtChequeNo.Enabled = true;

            }

            else if (optCash.Checked)
            {
                

                ddlMutualBank.Enabled = false;
                txtChequeDate.Enabled = false;
                txtChequeNo.Enabled = false;
            }
        }

        protected void cldDOB_TextChanged(object sender, EventArgs e)
        {
            // Validate and calculate age when DOB is changed
            DateTime dob;
            if (DateTime.TryParse(cldDOB.Text, out dob))
            {
                // Calculate the age in months
                int ageInMonths = CalculateAgeInMonths(dob);

                // Enable or disable guardian fields based on age
                if (ageInMonths < 216) // Less than 18 years (216 months)
                {
                    EnableGuardianFields(true);
                }
                else
                {
                    EnableGuardianFields(false);
                }
            }
            else
            {
                // Handle invalid date input if necessary
                // Optionally, you can display an error message here
            }
            cldDOB.Focus();
        }

        protected void addfamDOB_TextChanged(object sender, EventArgs e)
        {
            // Validate and calculate age when DOB is changed
            DateTime dob;
            string currentFabDate = addfamDOB.Text; // e.g., "16/12/1987"
            string dateFormat = "dd/MM/yyyy"; // Specify the expected format
            CultureInfo provider = CultureInfo.InvariantCulture; // Use invariant culture for consistent parsing

            if (DateTime.TryParseExact(currentFabDate, dateFormat, provider, DateTimeStyles.None, out dob))
            {
                // Calculate the age in months
                int ageInMonths = CalculateAgeInMonths(dob);

                // Enable or disable guardian fields based on age
                if (ageInMonths < 216) // Less than 18 years (216 months)
                {
                    EnableGuardianFields2(true);
                }
                else
                {
                    EnableGuardianFields2(false);
                }
            }

            else
            {
                // Handle invalid date input if necessary
                // Optionally, you can display an error message here
            }
            addfamDOB.Focus();
        }

        protected void ValidateGuardianByDOB(TextBox dobTextBox, bool mainOrFam)
        {
            // Validate and calculate age when DOB is changed
            DateTime dob;
            string currentFabDate = dobTextBox.Text; // e.g., "16/12/1987"
            string dateFormat = "dd/MM/yyyy"; // Specify the expected format
            CultureInfo provider = CultureInfo.InvariantCulture; // Use invariant culture for consistent parsing

            if (DateTime.TryParseExact(currentFabDate, dateFormat, provider, DateTimeStyles.None, out dob))
            {
                // Calculate the age in months
                int ageInMonths = CalculateAgeInMonths(dob);

                // Enable or disable guardian fields based on age
                if (ageInMonths < 216) // Less than 18 years (216 months)
                {
                    if (mainOrFam)
                    {
                        EnableGuardianFields(true);
                    }
                    else
                    {

                    EnableGuardianFields2(true);
                    }
                }
                else
                {
                    if (mainOrFam)
                    {
                        EnableGuardianFields(false);

                    }
                    else
                    {

                    EnableGuardianFields2(false);
                    }
                }
            }
            

            else
            {
                // Handle invalid date input if necessary
                // Optionally, you can display an error message here
            }
            dobTextBox.Focus();
        }

        private void EnableGuardianFields(bool enable)
        {
            // Enable or disable the guardian fields
            itfAOGuardianPerson.Enabled = enable;
            itfAOGuardianNationality.Enabled = enable;
            itfAOGuardianPANNO.Enabled = enable;

            if (enable)
            {
                // Add the 'required' attribute if fields are enabled
                itfAOGuardianPerson.Attributes.Add("required", "required");
                itfAOGuardianNationality.Attributes.Add("required", "required");
                itfAOGuardianPANNO.Attributes.Add("required", "required");
            }
            else
            {
                // Remove the 'required' attribute if fields are disabled
                itfAOGuardianPerson.Attributes.Remove("required");
                itfAOGuardianNationality.Attributes.Remove("required");
                itfAOGuardianPANNO.Attributes.Remove("required");

                // Optionally clear the fields when disabled
                itfAOGuardianPerson.Text = string.Empty;
                itfAOGuardianNationality.Text = string.Empty;
                itfAOGuardianPANNO.Text = string.Empty;
            }
        }


        private void EnableGuardianFields2(bool enable)
        {
            // Enable or disable the second set of guardian fields (Name and PAN)
            addfamGuardianName.Enabled = enable;
            addfamGuardianPan.Enabled = enable;

            if (enable)
            {
                // Add the 'required' attribute if fields are enabled
                addfamGuardianName.Attributes.Add("required", "required");
                addfamGuardianPan.Attributes.Add("required", "required");
            }
            else
            {
                // Remove the 'required' attribute if fields are disabled
                addfamGuardianName.Attributes.Remove("required");
                addfamGuardianPan.Attributes.Remove("required");

                // Optionally clear the fields when disabled
                addfamGuardianName.Text = string.Empty;
                addfamGuardianPan.Text = string.Empty;
            }
        }

        // Helper method to calculate the age in months
        private int CalculateAgeInMonths(DateTime dob)
        {
            DateTime today = DateTime.Today;
            int months = (today.Year - dob.Year) * 12 + today.Month - dob.Month;
            if (today.Day < dob.Day)
            {
                months--; // Adjust if the current day hasn't reached the birthday yet
            }
            return months;
        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
            {
                
                // Copy values from mailing address to permanent address
                txtPAddress1.Text = itfAOMAddress1.Text;
                txtPAddress2.Text = itfAOMAddress2.Text;

                AddSelectedItemToDropdown(ddlMailingCountryList, ddlPCountryList);
                AddSelectedItemToDropdown(ddlMailingStateList, ddlPStateList);
                AddSelectedItemToDropdown(ddlMailingCityList, ddlPCityList);

                txtPPin.Text = txtMailingPin.Text;

                // Save data to session
                Session["PermanentAddress"] = new
                {
                    Address1 = txtPAddress1.Text,
                    Address2 = txtPAddress2.Text,
                    Country = ddlPCountryList.SelectedValue,
                    State = ddlPStateList.SelectedValue,
                    City = ddlPCityList.SelectedValue,
                    Pin = txtPPin.Text
                };
                HandlePinCodeValidation(ddlMailingCountryList.SelectedItem.ToString(), txtMailingPin);
                HandlePinCodeValidation(ddlPCountryList.SelectedItem.ToString(), txtPPin);

                Session["IsAddressCopied"] = true;

             
                CheckBox1.Focus();

            }
            else
            {
                // Clear permanent address fields if unchecked
                ClearPermanentAddressFields();
                HandlePinCodeValidation(ddlMailingCountryList.SelectedItem.ToString(), txtMailingPin);
                HandlePinCodeValidation(ddlPCountryList.SelectedItem.ToString(), txtPPin);


                // Remove session data
                Session.Remove("PermanentAddress");
                Session["IsAddressCopied"] = false;

                CheckBox1.Focus();

            }
        }
        private void ClearPermanentAddressFields()
        {
            txtPAddress1.Text = string.Empty;
            txtPAddress2.Text = string.Empty;
            ddlPCountryList.SelectedIndex = -1;
            ddlPStateList.SelectedIndex = -1;
            ddlPCityList.SelectedIndex = -1;
            txtPPin.Text = string.Empty;
        }

        private void AddSelectedItemToDropdown(DropDownList sourceDropdown, DropDownList targetDropdown)
        {
            if (sourceDropdown.SelectedItem != null)
            {
                // Get selected item's text and value
                string selectedText = sourceDropdown.SelectedItem.Text;
                string selectedValue = sourceDropdown.SelectedItem.Value;

                // Check if the item already exists in the target dropdown
                ListItem newItem = new ListItem(selectedText, selectedValue);
                if (!targetDropdown.Items.Contains(newItem))
                {
                    targetDropdown.Items.Add(newItem);
                }

                // Set the newly added item as selected
                targetDropdown.SelectedValue = selectedValue;

                if(ddlMailingCountryList.Items.FindByValue(sourceDropdown.ToString()) != null)
                {
                    HandlePinCodeValidation(sourceDropdown.SelectedItem.ToString(), txtPPin);
                }
            }
        }


        private Boolean ValidateMobileFieldMinLength(TextBox uiField, int minLength = 10)
        {
            string fieldValue = uiField.Text;
            if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Length < minLength)
            {
                // Trigger alert and set focus on the corresponding field
                string script = $"alert('The value should be at least {minLength} digits.');";
                // script += $"document.getElementById('{fieldId}').focus();";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
                uiField.Focus();
                return true; // Exit after showing alert and focusing
            }
            return false;
        }



        #region InvestorList Functions
        private void FillExistingInvestorList(string src, string exist)
        {
            // Clear the dropdown list items
            ddladdfamExistingInvestor.Items.Clear();

            // Call the GetInvestorList method to fetch data
            DataTable dt = new WM.Controllers.AccountOpeningController().GetInvestorList(src, exist);

            if (dt.Rows.Count > 0)
            {
                // Add a new column to the DataTable to store the concatenated investor_name and inv_code
                dt.Columns.Add("DisplayText", typeof(string));

                // Loop through each row and concatenate investor_name and inv_code into DisplayText
                foreach (DataRow row in dt.Rows)
                {
                    row["DisplayText"] = row["investor_name"].ToString() + " (" + row["inv_code"].ToString() + ")";
                }

                // Bind the DataTable to the dropdown
                ddladdfamExistingInvestor.DataSource = dt;
                ddladdfamExistingInvestor.DataTextField = "DisplayText";  // Column that contains the concatenated value
                ddladdfamExistingInvestor.DataValueField = "inv_code";    // Column that holds the value
                ddladdfamExistingInvestor.DataBind();

                // Insert a default "Select Investor" item at the top of the list
                ddladdfamExistingInvestor.Items.Insert(0, new ListItem("Select Investor", ""));
            }
            else
            {
                // If no records are found, display a "No Investors Found" item
                ddladdfamExistingInvestor.Items.Insert(0, new ListItem("No Investors Found", ""));
            }
        }

        private void PopulateInvestorBranchDropDown()
        {
            try
            {
                // Check if LoginId exists in the session
                if (Session["LoginId"] == null)
                {
                    // If not, redirect to the index page (or any other page you prefer)
                    Response.Redirect("~/Index.aspx");
                    return; // Ensure further code execution stops here
                }

                // Get the LoginId from session
                string loginId = Session["LoginId"].ToString();

                // Create the controller instance
                WM.Controllers.AccountOpeningController controller = new WM.Controllers.AccountOpeningController();

                // Call the GetBranchListByLogin method and pass the loginId
                DataTable branchData = controller.GetBranchListByLogin(loginId);

                // Populate Branch dropdown
                InvestorBranchDropDown.DataSource = branchData;
                InvestorBranchDropDown.DataTextField = "BRANCH_NAME"; // Adjust field names if necessary
                InvestorBranchDropDown.DataValueField = "BRANCH_CODE";
                InvestorBranchDropDown.DataBind();

                // Insert a default item at the top of the dropdown list
                InvestorBranchDropDown.Items.Insert(0, new ListItem("Select Branch", ""));
            }
            catch (Exception ex)
            {
                // Handle exception
                // Log error or show user-friendly message
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Handles the Branch Dropdown selection change
        protected void InvestorBranchDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranch = InvestorBranchDropDown.SelectedValue;

            // Populate RM Dropdown based on the selected branch
            FillInvestorListBranchRM(selectedBranch);
        }

        // Fills the RM Dropdown
        private void FillInvestorListBranchRM(string branchCode)
        {
            try
            {
                // Null or empty check
                if (string.IsNullOrWhiteSpace(branchCode))
                {
                    InvestorListBranchRM.Items.Clear();
                    InvestorListBranchRM.Items.Insert(0, new ListItem("Select RM", ""));
                    return;
                }

                DataTable rmData = new WM.Controllers.AccountOpeningController().GetRMBySource(branchCode);

                // Check if data exists
                if (rmData != null && rmData.Rows.Count > 0)
                {
                    InvestorListBranchRM.DataSource = rmData;
                    InvestorListBranchRM.DataTextField = "RM_NAME";
                    InvestorListBranchRM.DataValueField = "RM_CODE";
                    InvestorListBranchRM.DataBind();
                }

                // Always insert default option
                InvestorListBranchRM.Items.Insert(0, new ListItem("Select RM", ""));
            }
            catch (Exception ex)
            {
                // Log the exception
                // Consider using a logging framework instead of client-side alert
                ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('Error loading RMs: {ex.Message}');", true);
            }
        }

        // Handles the Search Button click
        protected void InvestorListSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate search criteria (optional but recommended)
                if (AreAllSearchFieldsEmpty())
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please enter at least one search criteria!');", true);
                    return;
                }

                #region Vlaues for investors search
                // Trim all string inputs and handle nullable values
                int? branch = string.IsNullOrWhiteSpace(InvestorBranchDropDown.SelectedValue) ? (int?)null : int.Parse(InvestorBranchDropDown.SelectedValue.Trim());
                int? rm = string.IsNullOrWhiteSpace(InvestorListBranchRM.SelectedValue) ? (int?)null : int.Parse(InvestorListBranchRM.SelectedValue.Trim());
                DateTime? dob = string.IsNullOrWhiteSpace(InvestorListDOB.Text) ? (DateTime?)null : DateTime.Parse(InvestorListDOB.Text.Trim());
                int? clientCode = string.IsNullOrWhiteSpace(InvestorListClientCode.Text) ? (int?)null : int.Parse(InvestorListClientCode.Text.Trim());
                string clientName = string.IsNullOrWhiteSpace(InvestorListClientName.Text) ? null : InvestorListClientName.Text.Trim();
                long? mobile = string.IsNullOrWhiteSpace(InvestorMobileTextBox.Text) ? (long?)null : long.Parse(InvestorMobileTextBox.Text.Trim());
                string city = string.IsNullOrWhiteSpace(InvestorCityDropDown.SelectedValue) ? null : InvestorCityDropDown.SelectedValue.Trim();
                string phone = string.IsNullOrWhiteSpace(InvestorPhoneTextBox.Text) ? null : InvestorPhoneTextBox.Text.Trim();
                string accountCode = string.IsNullOrWhiteSpace(InvestorListAccountCode.Text) ? null : InvestorListAccountCode.Text.Trim();
                string pan = string.IsNullOrWhiteSpace(InvestorListPan.Text) ? null : InvestorListPan.Text.Trim();
                string investorName = string.IsNullOrWhiteSpace(InvestorListName.Text) ? null : InvestorListName.Text.Trim();
                string address1 = string.IsNullOrWhiteSpace(InvestorListAdd1.Text) ? null : InvestorListAdd1.Text.Trim();
                string address2 = string.IsNullOrWhiteSpace(InvestorListAdd2.Text) ? null : InvestorListAdd2.Text.Trim();

                #endregion

                DataTable resultData = new WM.Controllers.AccountOpeningController().SearchInvestors(
                    branch, rm, dob, clientCode, clientName, mobile, city, phone,
                    accountCode, pan, investorName, address1, address2
                );

                // Check if results exist
                if (resultData != null && resultData.Rows.Count > 0)
                {
                    InvestorDetailsGridView.DataSource = resultData;
                    InvestorDetailsGridView.DataBind();
                    // Optionally, show number of results found
                    InvestorDetailsLabel.Text = $"{resultData.Rows.Count} investors found.";
                }
                else
                {
                    // Clear grid and show no results message
                    InvestorDetailsGridView.DataSource = null;
                    InvestorDetailsGridView.DataBind();
                    InvestorDetailsLabel.Text = "No investors found matching the search criteria.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                ClientScript.RegisterStartupScript(this.GetType(), "Error", $"alert('Search Error: {ex.Message}');", true);
            }
        }

        // Check if all search fields are empty
        private bool AreAllSearchFieldsEmpty()
        {
            return string.IsNullOrWhiteSpace(InvestorBranchDropDown.SelectedValue) &&
                   string.IsNullOrWhiteSpace(InvestorListBranchRM.SelectedValue) &&
                   string.IsNullOrWhiteSpace(InvestorListDOB.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListClientCode.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListClientName.Text) &&
                   string.IsNullOrWhiteSpace(InvestorMobileTextBox.Text) &&
                   string.IsNullOrWhiteSpace(InvestorCityDropDown.SelectedValue) &&
                   string.IsNullOrWhiteSpace(InvestorPhoneTextBox.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListAccountCode.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListPan.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListName.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListAdd1.Text) &&
                   string.IsNullOrWhiteSpace(InvestorListAdd2.Text);
        }

        // Handles the Reset Button click
        protected void InvestorListReset_Click(object sender, EventArgs e)
        {
            ResetInvestorModel1();


        }

        protected void ResetInvestorModel1()
        {
            InvestorBranchDropDown.SelectedIndex = 0;
            InvestorListBranchRM.Items.Clear();
            InvestorListBranchRM.Items.Insert(0, new ListItem("Select RM", ""));

            // Clear text inputs
            InvestorListDOB.Text = string.Empty;
            InvestorListClientCode.Text = string.Empty;
            InvestorListClientName.Text = string.Empty;
            InvestorMobileTextBox.Text = string.Empty;
            InvestorCityDropDown.SelectedIndex = 0;
            InvestorPhoneTextBox.Text = string.Empty;
            InvestorListAccountCode.Text = string.Empty;
            InvestorListPan.Text = string.Empty;
            InvestorListName.Text = string.Empty;
            InvestorListAdd1.Text = string.Empty;
            InvestorListAdd2.Text = string.Empty;

            // Clear grid and label
            InvestorDetailsGridView.DataSource = null;
            InvestorDetailsGridView.DataBind();
            InvestorDetailsLabel.Text = string.Empty;

        }



            // GridView RowCommand event for selecting an investor
            protected void gvInvestorSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                string investorCode = e.CommandArgument?.ToString();
                if (!string.IsNullOrWhiteSpace(investorCode))
                {
                    HandleSelectedInvestor(investorCode);
                }
            }


        }

        // Handle InvestorCode selection and set field data
        private void HandleSelectedInvestor(string investorCode)
        {
            // Validate investor code
            if (string.IsNullOrWhiteSpace(investorCode))
            {
                // Log or handle invalid selection if needed
                ShowAlert("Invalid investor code");
                return;
            }

            // Store the selected investor code in session
            Session["SelectedInvestorCode"] = investorCode;

            try
            {
                //DataTable dt = new AccountOpeningController().GetClientDataByInvCode(investorCode);
                DataTable dt = new AccountOpeningController().GetInvestorHeadOrMember(investorCode);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string message = GetTextFieldValue(row,"message");

                    if (message.Contains("head"))
                    {
                        string currentAHByInvHead= GetTextFieldValue(row, "client_code");
                        string currentDbMain = GetTextFieldValue(row, "main_code");

                        ResetFormFields1();
                        FillClientDataByAHNum(currentAHByInvHead);
                        ResetInvestorModel1();
                        ClientScript.RegisterStartupScript(this.GetType(), "InvestorSelected", "closeInvestorListModal(); loadInvestorDetails();", true);
                    }
                    else if (message.Contains("member"))
                    {
                        ShowAlert("The Selected Investor Is Not The Head Of Family,Please Initiate Only Head Of Faimily As Main Investor");
                    }
                    else
                    {
                        ShowAlert(message);
                    }
                }
                else
                {
                    ShowAlert("No data found for the provided investor code");
                    return;
                }


            }
            catch (Exception ex)
            {
                ShowAlert("An error occurred while processing the investor code: " + ex.Message);
            }


        }





        #endregion

        protected void oneGuestSearch_Click(object sender, EventArgs e)
        {

            var guestCodeValue = txtGuestCode.Text.Trim();
            var bussinessCodeValue = txtBusinessCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(guestCodeValue))
            {
                ResetFormFields1();
                string emptyMsg = "Please provide guest code!";
                ShowAlert(emptyMsg);
                txtGuestCode.Focus();
            }
            else
            {
                FillBusinessByGuestCode(guestCodeValue);
                txtBusinessCode.Focus();
            }

            
        }

        private void FillBusinessByGuestCode(string guestCode)
        {
            try
            {
                var dt = new AccountOpeningController().GetGuestValidationData(guestCode);

                if (dt == null || dt.Rows.Count <= 0)
                {
                    ResetFormFields1();
                    string emptyMsg = "No Data!";
                    ShowAlert(emptyMsg);
                    txtGuestCode.Text = guestCode;
                    txtGuestCode.Focus();
                    lblHolderMessage.Text = emptyMsg;
                }
                if (dt?.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string message = dt.Rows[0]["message"].ToString();
                    string businessCode = dt.Rows[0]["BusinessCode"].ToString();


                    string validateMsg = "Operation Successful";
                    string onlyUsedMsg = "Guest Code Used";
                    
                    if (message.Contains(validateMsg) && !string.IsNullOrEmpty(businessCode))
                    {
                        string validAlertMsg = "Guest code is valid.";
                        //ShowAlert(validAlertMsg);
                        ResetFormFields1();
                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                        UpdateBusinessCodeDetails(businessCode);
                        txtBusinessCode.Focus();

                        // Assuming you have textboxes or labels to display these values
                        txtBusinessCode.Text = businessCode;
                        txtGuestCode.Text = guestCode;
                        lblMessage.Text = validAlertMsg;

                    }
                    else if (message.Contains(validateMsg) && string.IsNullOrEmpty(businessCode))
                    {
                        string validAlertMsg = "Guest code is valid, bussiness code not exist";
                        //ShowAlert(validAlertMsg);
                        ResetFormFields1();
                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                        txtGuestCode.Text = guestCode;
                        lblMessage.Text = validAlertMsg;

                    }
                    else  
                    {
                        ShowAlert(message);
                        ResetFormFields1();
                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                        txtGuestCode.Text = guestCode;



                    }
                   


                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = $"An error occurred while retrieving data: {ex.Message}";
            }
        }


        protected void txtBusinessCode_TextChanged(object sender, EventArgs e)
        {
            // Call the method that handles the logic for fetching the RM data
            UpdateBusinessCodeDetails(txtBusinessCode.Text.Trim());
        }

        private void UpdateBusinessCodeDetails(string businessCode)
        {
            

            // Create an instance of your controller or service that contains GetRMEmployee
            AccountOpeningController controller = new AccountOpeningController();

            // Fetch the data from the database
            DataTable rmData = controller.GetRMEmployee(businessCode);
            if (rmData.Rows.Count <= 0)
            {
                string emptyMsg = "RM data not found!";
                //ShowAlert(emptyMsg);
            }
            else
            {
                // Assuming the DataTable columns match the procedure output
                string bssRmNameVlaue = GetTextFieldValue(rmData.Rows[0], "EM_RM_NAME");
                string bssRmBranchVlaue = GetTextFieldValue(rmData.Rows[0], "EM_BRANCH_NAME_CODE");


                txtBusinessCodeName.Text = rmData.Rows[0]["EM_RM_NAME"].ToString();
                txtBusinessCodeBranch.Text = rmData.Rows[0]["EM_BRANCH_NAME_CODE"].ToString();
            }

        }

        private string GetSafeStringValue(DataRow row, string columnName)
        {
            return row[columnName] != DBNull.Value ? row[columnName].ToString() : string.Empty;
        }



        protected void txtSearchPan_TextChanged(object sender, EventArgs e)
        {
            // Get the TextBox control
            TextBox txtSearchPan = (TextBox)sender;

            // Validate input: Ensure length is <= 10 and check for special characters
            string input = txtSearchPan.Text;

            if (input.Length > 10)
            {
                // Clear the TextBox and show an alert if the length exceeds 10
                txtSearchPan.Text = string.Empty;
                string script = "alert('Input should not exceed 10 characters.');" +
                                "document.getElementById('" + txtSearchPan.ClientID + "').focus();";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "lengthAlert", script, true);
                return;
            }

            // Validate against special characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(input, "^[a-zA-Z0-9]*$"))
            {
                // Clear the TextBox and show an alert for special characters
                txtSearchPan.Text = string.Empty;
                string script = "alert('Only alphabetic and numeric characters are allowed.');" +
                                "document.getElementById('" + txtSearchPan.ClientID + "').focus();";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "charAlert", script, true);
            }
        }




        #region fillBankMasterDetails
        private void fillBankMasterDetails()
        {
            DataTable dt1 = new DataTable();
            dt1 = new WM.Controllers.AccountOpeningController().GetBankMasterDetails();
            AddDefaultItem(dt1, "BANK_NAME", "BANK_ID", "Select");

            ddlMutualBank.DataSource = dt1;
            ddlMutualBank.DataTextField = "BANK_NAME";
            ddlMutualBank.DataValueField = "BANK_ID";
            ddlMutualBank.DataBind();
        }
        #endregion



        protected void ViewButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("AccountOpeningView.aspx"); // Example redirect
        }






        public static void SelectValueFromRow(DataRow row, string columnName, DropDownList dropDownList)
        {
            // Get the value to select from the row
            string valueToSelect = !string.IsNullOrEmpty(row[columnName].ToString())
                ? row[columnName].ToString()
                : dropDownList.Items.Count > 0
                    ? dropDownList.Items[0].Value
                    : string.Empty;

            // Check if the value exists in the DropDownList items
            if (dropDownList.Items.FindByValue(valueToSelect) != null)
            {
                dropDownList.SelectedValue = valueToSelect;
            }
            else
            {
                // If the value is not in the list, set to the first item or handle as needed
                if (dropDownList.Items.Count > 0)
                {
                    dropDownList.SelectedIndex = -1; // Set to the first item
                }
                else
                {
                    dropDownList.SelectedValue = string.Empty; // Handle empty case if needed
                }
            }
        }



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


        public string GetDropDownValue(DataRow dataRow, string fieldName)
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
                    //fieldName = MapACCategory(fieldName);
                    // Return the field value if it's not null, otherwise return "-1"
                    return dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : "-1";
                }
                else
                {
                    throw new ArgumentException($"Field '{fieldName}' does not exist in the DataRow.");
                }
            }
            catch (Exception ex)
            {
                // Show the error message in the label if an error occurs
                lblMessage.Text = ex.Message;
                return "-1";  // Return "-1" in case of an error
            }
        }


        public string GetDropDownValue(DataRow dataRow, string fieldName, DropDownList dropdown)
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
                    string fieldValue = dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : "-1";

                    // Check if the field value is valid
                    if (fieldValue != "-1")
                    {
                        // Check if the item exists in the dropdown
                        ListItem existingItem = dropdown.Items.FindByValue(fieldValue);

                        if (existingItem != null)
                        {
                            // Select the existing item
                            dropdown.SelectedValue = fieldValue;
                        }
                        else
                        {
                            // Add the new item to the dropdown and select it
                            dropdown.Items.Add(new ListItem(fieldValue, fieldValue));
                            dropdown.SelectedValue = fieldValue;
                        }
                    }

                    return fieldValue;
                }
                else
                {
                    // Return "-1" if the field does not exist in the DataRow
                    lblMessage.Text = $"Field '{fieldName}' does not exist in the DataRow.";
                    return "-1";
                }
            }
            catch (Exception ex)
            {
                // Show the error message in the label if an error occurs
                lblMessage.Text = ex.Message;
                return "-1";  // Return "-1" in case of an error
            }
        }

        // Reusable function to select a value in any DropDownList or fall back to the first item if not found
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

        private int GetCountryCodeByStateCode(int stateCode)
        {
            int countryCode = 0;  // Default value if not found
            if (stateCode == 0)
            {
                return 0;
            }
            else
            {
                try
                {
                    // Fetch data from the controller
                    DataTable dt = new AccountOpeningController().GetCountriesByState(stateCode);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Assuming we need to get the first country_id
                        countryCode = Convert.ToInt32(dt.Rows[0]["country_id"]);
                    }
                    else
                    {
                        countryCode = 0;  // If no countries found
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    countryCode = 0;  // Return 0 in case of an error
                }
            }

            return countryCode;
        }

        protected void ValidatePinCode(DropDownList ddlMailingCountryList, TextBox txtMailingPin, string regPinBox)
        {
            // Get selected country and PIN code
            string selectedCountry = ddlMailingCountryList.SelectedValue; // Get selected country
            string pinCode = txtMailingPin.Text.Trim(); // Get entered PIN code

            // Register the control for event validation
            ClientScriptManager cs = Page.ClientScript;
            if (!cs.IsClientScriptBlockRegistered(regPinBox))
            {
                cs.RegisterForEventValidation(txtPPin.UniqueID, txtPPin.Text);
            }

            // Check if a country is selected
            if (string.IsNullOrEmpty(selectedCountry) || selectedCountry == "0")
            {
                ShowAlert("Please select a country before entering the PIN code.");
                ddlMailingCountryList.Focus(); // Set focus back to the country dropdown
                txtMailingPin.Text = string.Empty;
                return;
            }

            if (selectedCountry == "1") // If India is selected
            {
                // Validate for exactly 6 numeric digits
                if (pinCode.Length != 6 || !pinCode.All(char.IsDigit))
                {
                    ShowAlert("For India, the PIN code must be exactly 6 numeric digits.");
                    txtMailingPin.Focus(); // Set focus back to the PIN code field
                    return;
                }
            }
            else
            {
                // For other countries, validate for up to 20 alphanumeric characters
                if (pinCode.Length > 20)
                {
                    ShowAlert("For other countries, the PIN code must not exceed 20 characters.");
                    txtMailingPin.Focus(); // Set focus back to the PIN code field
                    return;
                }
            }

            // If validation passes
            //ShowAlert("PIN code is valid.");
            txtMailingPin.Focus();
        }

        protected void mailingPinValidate_TextChanged(object sender, EventArgs e)
        {
            ValidatePinCode(ddlMailingCountryList, txtMailingPin, "txtMailingPin");
        }

        protected void pPinValidate_TextChanged(object sender, EventArgs e)
        {
            ValidatePinCode(ddlPCountryList, txtPPin, "txtPPin");
        }


        private void HandlePinCodeValidation(string selectedCountry, TextBox txtMailingPin)
        {
            // You can set up the logic here to adjust other controls or perform actions
            if (selectedCountry.Equals("India", StringComparison.OrdinalIgnoreCase))
            {
                // Set the pin code length or other validation properties specific to India
                string msg = "Enter 6-digit PIN code";
                txtMailingPin.MaxLength = 6;
                txtMailingPin.Attributes["placeholder"] = msg;
                txtMailingPin.Attributes["onkeypress"] = "return event.charCode >= 48 && event.charCode <= 57";

                // Remove non-numeric characters and ensure only 6 digits server-side
                if (!string.IsNullOrEmpty(txtMailingPin.Text))
                {
                    txtMailingPin.Text = new string(txtMailingPin.Text.Where(char.IsDigit).ToArray());

                    // Ensure the length does not exceed 6 digits
                    if (txtMailingPin.Text.Length > 6)
                    {
                        txtMailingPin.Text = txtMailingPin.Text.Substring(0, 6);
                    }
                }

            }
            else
            {
                string msg = "Enter PIN code (Max 20 characters)";
                txtMailingPin.MaxLength = 20;
                txtMailingPin.Attributes["placeholder"] = msg;
                txtMailingPin.Attributes.Remove("onkeypress"); 
            }
        }
        protected void ddlMailingCountryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message
            try
            {
                int selectedCountryId = Convert.ToInt32(ddlMailingCountryList.SelectedValue);
                string selectedCountryName = ddlMailingCountryList.SelectedItem.ToString();
                lblMessage.Text = selectedCountryId.ToString();
                if (selectedCountryId != 0) // Check if a valid country is selected
                {
                     PopulateStateDropDownForAddress(selectedCountryId, ddlMailingStateList);
                    //fillStateList();
                    ddlMailingStateList.Focus();
                    ddlMailingCityList.Items.Clear();
                    HandlePinCodeValidation(selectedCountryName, txtMailingPin);

                }
                else
                {
                    // Handle case when "Select Country" (value 0) is selected
                    ddlMailingStateList.Items.Clear();
                    ddlMailingStateList.Items.Insert(0, new ListItem("Select State", ""));

                    ddlMailingCityList.Items.Clear();
                    ddlMailingCityList.Items.Insert(0, new ListItem("Select City", ""));

                }
            }
            catch (FormatException ex)
            {
                // Display error message in lblMessage
                lblMessage.Text = "Error: Invalid format for country ID.";
            }
            catch (Exception ex)
            {
                // Display generic error message in lblMessage
                lblMessage.Text = "Error: An unexpected error occurred while selecting the country.";
            }
        }

        protected void ddlMailingStateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message
            try
            {
                int selectedStateId = Convert.ToInt32(ddlMailingStateList.SelectedValue); // Convert Value to integer

                if (selectedStateId != 0) // Check if a valid state is selected
                {
                    // Call function to populate the city dropdown based on the selected state
                    PopulateCityDropDownForAddress(selectedStateId, ddlMailingCityList);
                    //fillCityList();
                    ddlMailingCityList.SelectedIndex = 0;
                    ddlMailingCityList.Focus();


                }
                else
                {
                    // Handle case when "Select State" (value 0) is selected
                    ddlMailingCityList.Items.Clear();
                    ddlMailingCityList.Items.Add(new ListItem("Select City", "0"));
                }
            }
            catch (FormatException ex)
            {
                // Display error message in lblMessage
                lblMessage.Text = "Error: Invalid format for state ID.";
            }
            catch (Exception ex)
            {
                // Display generic error message in lblMessage
                lblMessage.Text = "Error: An unexpected error occurred while selecting the state.";
            }
        }

        protected void ddlPCountryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message
            try
            {
                
                
                int selectedCountryId = Convert.ToInt32(ddlPCountryList.SelectedValue); // Convert Value to integer
                string selectedCountryName = ddlPCountryList.SelectedItem.ToString();
                lblMessage.Text = selectedCountryId.ToString();
                if (selectedCountryId != 0) // Check if a valid country is selected
                {
                    // Call function to populate the state dropdown based on the selected country
                    PopulateStateDropDownForAddress(selectedCountryId, ddlPStateList);
                    //fillStateList();
                    ddlPStateList.SelectedIndex = 0;
                    ddlPCityList.Items.Clear();
                    HandlePinCodeValidation(selectedCountryName, txtPPin);
                    ddlPStateList.Focus();
                }
                else
                {
                    // Handle case when "Select Country" (value 0) is selected
                    ddlPStateList.Items.Clear();
                    ddlPStateList.Items.Insert(0, new ListItem("Select State", ""));

                    ddlPCityList.Items.Clear();
                    ddlPCityList.Items.Insert(0, new ListItem("Select City", ""));

                }

                // Register items for event validation
                foreach (ListItem item in ddlPStateList.Items)
                {
                    ClientScript.RegisterForEventValidation(ddlPStateList.UniqueID, item.Value);
                }
            }
            catch (FormatException ex)
            {
                // Display error message in lblMessage
                lblMessage.Text = "Error: Invalid format for country ID.";
            }
            catch (Exception ex)
            {
                // Display generic error message in lblMessage
                lblMessage.Text = "Error: An unexpected error occurred while selecting the country.";
            }
        }

        protected void ddlPStateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message
            try
            {
                int selectedStateId = Convert.ToInt32(ddlPStateList.SelectedValue); // Convert Value to integer

                if (selectedStateId != 0) // Check if a valid state is selected
                {
                    // Call function to populate the city dropdown based on the selected state
                    PopulateCityDropDownForAddress(selectedStateId, ddlPCityList);
                    //fillCityList();
                    ddlPCityList.SelectedIndex = 0;
                    ddlPCityList.Focus();

                }
                else
                {
                    // Handle case when "Select State" (value 0) is selected
                    ddlPCityList.Items.Clear();
                    ddlPCityList.Items.Add(new ListItem("Select City", "0"));
                }
                // Register items for event validation
                foreach (ListItem item in ddlPCityList.Items)
                {
                    ClientScript.RegisterForEventValidation(ddlPCityList.UniqueID, item.Value);
                }
            }
            catch (FormatException ex)
            {
                // Display error message in lblMessage
                lblMessage.Text = "Error: Invalid format for state ID.";
            }
            catch (Exception ex)
            {
                // Display generic error message in lblMessage
                lblMessage.Text = "Error: An unexpected error occurred while selecting the state.";
            }
        }



        private void PopulateStateDropDownForAddress(int countryId, DropDownList ddlStateList)
        {
            DataTable dt = new AccountOpeningController().GetStatesByCountry(countryId);
            ddlStateList.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                ddlStateList.DataSource = dt;
                ddlStateList.DataTextField = "state_name";  // Column in your database for state name
                ddlStateList.DataValueField = "state_id";   // Column in your database for state id
                ddlStateList.DataBind();
                ddlStateList.Items.Insert(0, new ListItem("Select State", ""));


            }
        }

        private void PopulateCityDropDownForAddress(int stateId, DropDownList ddlCityList)
        {
            DataTable dt = new AccountOpeningController().GetCitiesByState(stateId);
            ddlCityList.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                ddlCityList.DataSource = dt;
                ddlCityList.DataTextField = "city_name";  // Column in your database for city name
                ddlCityList.DataValueField = "city_id";   // Column in your database for city id
                ddlCityList.DataBind();
                ddlCityList.Items.Insert(0, new ListItem("Select City", ""));

            }
            else
            {
                ddlCityList.Items.Insert(0, new ListItem("", ""));

            }
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

        private void SetDateFromTexBox(TextBox inputField)
        {
            string currecntDate = inputField.Text.ToString();
            if (System.DateTime.TryParse(currecntDate, out System.DateTime parsedDate))
            {
                inputField.Text = parsedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                inputField.Text = string.Empty;
            }
        }

        #region ResetButton
                                    protected void ResetFields(object sender, EventArgs e)
        {
            //ResetFormFields1();
            //RemoveStidFromUrl();
            // Reload the current URL (this will refresh the page)
            Response.Redirect(Request.RawUrl);

        }


        protected void ResetFamilyGrid()
        {
            // Hide the GridView
            familyGridView.Visible = false;

            // Clear the DataSource and rebind the GridView
            familyGridView.DataSource = null;
            familyGridView.DataBind();

        }
        protected void ResetFormFields1()
        {
            CheckBox1.Checked = false;
            ResetAdvisoryPackageFields();
           // txtClientCodeLabel.Attributes.CssStyle.Add("display", "block");
           // txtClientCode.Attributes.CssStyle.Add("display", "block");

             ResetFamSection();

            txtHeadSourceCode.Text = string.Empty;
            txtGuestCode.Text = string.Empty;

            btnApprove.Enabled = true;
            // btnSave.Enabled = true;
            //  btnUpdate.Enabled = false;


            lblHolderMessage.Text = string.Empty;
            CheckBox1.Checked = false;
            ReferenceMobileNo1.Text = string.Empty;
            ReferenceMobileNo2.Text = string.Empty;
            ReferenceMobileNo3.Text = string.Empty;
            ReferenceMobileNo4.Text = string.Empty;
            ReferenceName1.Text = string.Empty;
            ReferenceName2.Text = string.Empty;
            ReferenceName3.Text = string.Empty;
            ReferenceName4.Text = string.Empty;

            lblMessage.Text = string.Empty;
            // Reset TextBox fields
            txtDTNumber.Text = string.Empty;
            txtSearchAHNum.Text = string.Empty;
            txtSearchClientCode.Text = string.Empty;
            txtSearchPan.Text = string.Empty;

            // Reset ApprovalStatus label
            ApprovalStatus.Text = "";


            // Clear TextBox values
            txtClientCode.Text = string.Empty;
            txtBusinessCode.Text = string.Empty;
            txtBusinessCodeBranch.Text = string.Empty;
            txtBusinessCodeName.Text = string.Empty;

            // Reset DropDownList values to default (first item)
            ddlTaxStatus.SelectedIndex = -1;
            ddlOccupation.SelectedIndex = -1;
            ddlAOCategoryStatus.SelectedIndex = -1;
            ddlAOClientCategory.SelectedIndex = -1;
            ddlAOACCategory.SelectedIndex = -1;

            // Reset all DropDownLists to default (first) value
            ddlSalutation1.SelectedIndex = -1;
            ddlSalutation2.SelectedIndex = -1;
            ddlAOGender.SelectedIndex = -1;
            ddlAOMaritalStatus.SelectedIndex = -1;
            ddlAONationality.SelectedIndex = -1;
            ddlAOResidentNRI.SelectedIndex = -1;
            ddlAOAnnualIncome.SelectedIndex = -1;
            ddlLeadSource.SelectedIndex = -1;

            // Reset all TextBoxes to empty
            txtAccountName.Text = string.Empty;
            txtFatherSpouse.Text = string.Empty;
            txtOther1.Text = string.Empty;
            cldDOB.Text = string.Empty;
            itfAOClientPan.Text = string.Empty;

            // Reset Guardian Details
            itfAOGuardianPerson.Text = string.Empty;
            itfAOGuardianNationality.Text = string.Empty;
            itfAOGuardianPANNO.Text = string.Empty;

            // Reset Mailing Address
            itfAOMAddress1.Text = string.Empty;
            itfAOMAddress2.Text = string.Empty;
            ddlMailingCountryList.SelectedIndex = -1;
            ddlMailingStateList.SelectedIndex = -1;
            ddlMailingCityList.SelectedIndex = -1;
            txtMailingPin.Text = string.Empty;

            // Reset Permanent Address
            txtPAddress1.Text = string.Empty;
            txtPAddress2.Text = string.Empty;
            ddlPCountryList.SelectedIndex = -1;
            ddlPStateList.SelectedIndex = -1;
            ddlPCityList.SelectedIndex = -1;
            txtPPin.Text = string.Empty;

            // Reset Overseas Address
            txtOverseasAdd.Text = string.Empty;

            // Reset Additional Details
            txtFax.Text = string.Empty;
            txtAadhar.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtOfficialEmail.Text = string.Empty;

            // Reset Phone Numbers
            PhoneOfficeSTD.Text = string.Empty;
            PhoneOfficeNumber.Text = string.Empty;
            PhoneResSTD.Text = string.Empty;
            PhoneResNumber.Text = string.Empty;
            MobileNo.Text = string.Empty;

            familyGridView.Visible = false;
            familyGridView.DataSource = null;
            familyGridView.DataBind();

            // Reset Reference Details

            foreach (var textBox in this.Controls.OfType<TextBox>())
            {
                textBox.Text = string.Empty;
            }

            // Clear all GridView controls by setting their DataSource to null and rebinding
            foreach (var gridView in this.Controls.OfType<GridView>())
            {
                gridView.Visible = false;
                gridView.DataSource = null;
                gridView.DataBind();
            }

            foreach (var dropDown in this.Controls.OfType<DropDownList>())
            {
                dropDown.SelectedIndex = 0; // Sets to the first item, or use -1 to clear selection if allowed
            }


        }



        void ResetFormFieldsOnInsertion()
        {


            lblHolderMessage.Text = string.Empty;
            CheckBox1.Checked = false;
            ReferenceMobileNo1.Text = string.Empty;
            ReferenceMobileNo2.Text = string.Empty;
            ReferenceMobileNo3.Text = string.Empty;
            ReferenceMobileNo4.Text = string.Empty;
            ReferenceName1.Text = string.Empty;
            ReferenceName2.Text = string.Empty;
            ReferenceName3.Text = string.Empty;
            ReferenceName4.Text = string.Empty;

            lblMessage.Text = string.Empty;
            // Reset TextBox fields
            txtDTNumber.Text = string.Empty;
            txtSearchAHNum.Text = string.Empty;
            txtSearchClientCode.Text = string.Empty;
            txtSearchPan.Text = string.Empty;

            // Reset ApprovalStatus label
            ApprovalStatus.Text = string.Empty;


            // Clear TextBox values
            txtClientCode.Text = string.Empty;
            txtBusinessCode.Text = string.Empty;

            // Reset DropDownList values to default (first item)
            ddlTaxStatus.SelectedIndex = -1;
            ddlOccupation.SelectedIndex = -1;
            ddlAOCategoryStatus.SelectedIndex = -1;
            ddlAOClientCategory.SelectedIndex = -1;
            ddlAOACCategory.SelectedIndex = -1;

            // Reset all DropDownLists to default (first) value
            ddlSalutation1.SelectedIndex = -1;
            ddlSalutation2.SelectedIndex = -1;
            ddlAOGender.SelectedIndex = -1;
            ddlAOMaritalStatus.SelectedIndex = -1;
            ddlAONationality.SelectedIndex = -1;
            ddlAOResidentNRI.SelectedIndex = -1;
            ddlAOAnnualIncome.SelectedIndex = -1;
            ddlLeadSource.SelectedIndex = -1;

            // Reset all TextBoxes to empty
            txtAccountName.Text = string.Empty;
            txtFatherSpouse.Text = string.Empty;
            txtOther1.Text = string.Empty;
            cldDOB.Text = string.Empty;
            itfAOClientPan.Text = string.Empty;

            // Reset Guardian Details
            itfAOGuardianPerson.Text = string.Empty;
            itfAOGuardianNationality.Text = string.Empty;
            itfAOGuardianPANNO.Text = string.Empty;

            // Reset Mailing Address
            itfAOMAddress1.Text = string.Empty;
            itfAOMAddress2.Text = string.Empty;
            ddlMailingCountryList.SelectedIndex = -1;
            ddlMailingStateList.SelectedIndex = -1;
            ddlMailingCityList.SelectedIndex = -1;
            txtMailingPin.Text = string.Empty;

            // Reset Permanent Address
            txtPAddress1.Text = string.Empty;
            txtPAddress2.Text = string.Empty;
            ddlPCountryList.SelectedIndex = -1;
            ddlPStateList.SelectedIndex = -1;
            ddlPCityList.SelectedIndex = -1;
            txtPPin.Text = string.Empty;

            // Reset Overseas Address
            txtOverseasAdd.Text = string.Empty;

            // Reset Additional Details
            txtFax.Text = string.Empty;
            txtAadhar.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtOfficialEmail.Text = string.Empty;

            // Reset Phone Numbers
            PhoneOfficeSTD.Text = string.Empty;
            PhoneOfficeNumber.Text = string.Empty;
            PhoneResSTD.Text = string.Empty;
            PhoneResNumber.Text = string.Empty;
            MobileNo.Text = string.Empty;

            familyGridView.Visible = false;
            familyGridView.DataSource = null;
            familyGridView.DataBind();

            // Reset Reference Details

            foreach (var textBox in this.Controls.OfType<TextBox>())
            {
                textBox.Text = string.Empty;
            }

            // Clear all GridView controls by setting their DataSource to null and rebinding
            foreach (var gridView in this.Controls.OfType<GridView>())
            {
                gridView.Visible = false;
                gridView.DataSource = null;
                gridView.DataBind();
            }

            foreach (var dropDown in this.Controls.OfType<DropDownList>())
            {
                dropDown.SelectedIndex = 0; // Sets to the first item, or use -1 to clear selection if allowed
            }


        }


        public void ResetFamSectionOnUPdate()
        {
            ddlSalutation3.SelectedIndex = 0;
            addfamInvestorName.Text = string.Empty;
            addfamMobile.Text = string.Empty;
            addfamEmail.Text = string.Empty;
            addfamPan.Text = string.Empty;
            addfamAadharNumber.Text = string.Empty;
            addfamDOB.Text = string.Empty;
            addfamRelation.SelectedIndex = 0;
            addfamGuardianName.Text = string.Empty;
            addfamGuardianPan.Text = string.Empty;
            addfamOccupation.SelectedIndex = 0;
            addfamKYC.SelectedIndex = 0;
            addfamApproved.SelectedIndex = 0;
            addfamGender.SelectedIndex = 0;
            addfamIsNominee.SelectedIndex = 0;
            addfamAllocationPercentage.Text = string.Empty;
        }

        void ResetFamTextField()
        {
            ddlSalutation3.SelectedIndex = this.Controls.Count - 1;

            addfamInvestorName.Text = string.Empty;
            addfamMobile.Text = string.Empty;
            addfamEmail.Text = string.Empty;
            addfamPan.Text = string.Empty;
            addfamAadharNumber.Text = string.Empty;
            addfamDOB.Text = string.Empty;
            addfamRelation.SelectedIndex = 0;
            addfamGuardianName.Text = string.Empty;
            addfamGuardianPan.Text = string.Empty;
            addfamOccupation.SelectedIndex = 0;
            addfamKYC.SelectedIndex = 0;
            addfamApproved.SelectedIndex = 0;
            addfamGender.SelectedIndex = 0;
            addfamIsNominee.SelectedIndex = 0;
            addfamAllocationPercentage.Text = string.Empty;

        }


        void ResetFamSection()
        {
            ddladdfamExistingInvestor.Items.Clear();
            ddlSalutation3.SelectedIndex = 0;
            addfamInvestorName.Text = string.Empty;
            addfamMobile.Text = string.Empty;
            addfamEmail.Text = string.Empty;
            addfamPan.Text = string.Empty;
            addfamAadharNumber.Text = string.Empty;
            addfamDOB.Text = string.Empty;
            addfamRelation.SelectedIndex = 0;
            addfamGuardianName.Text = string.Empty;
            addfamGuardianPan.Text = string.Empty;
            addfamOccupation.SelectedIndex = 0;
            addfamKYC.SelectedIndex = 0;
            addfamApproved.SelectedIndex = 0;
            addfamGender.SelectedIndex = 0;
            addfamIsNominee.SelectedIndex = 0;
            addfamAllocationPercentage.Text = string.Empty;
        }


        #endregion


        private void RemoveStidFromUrl()
        {
            string currentUrl = Request.Url.AbsoluteUri;

            if (Request.QueryString["stid"] != null)
            {
                var uri = new Uri(currentUrl);
                var query = uri.Query;

                // Remove 'stid' parameter from the query string
                var queryParameters = HttpUtility.ParseQueryString(query);
                queryParameters.Remove("stid");

                // Reconstruct the URL without 'stid' parameter
                var baseUrl = currentUrl.Split('?')[0];
                var newQuery = queryParameters.ToString();
                var newUrl = newQuery.Length > 0 ? $"{baseUrl}?{newQuery}" : baseUrl;

                // Redirect to the URL without 'stid'
                Response.Redirect(newUrl);
            }
        }


        #region Mapping Functions

        #region MapANnualIncome


        private string MapAnnualIncome(string dbIncome)
        {
            string trimmedIncome = dbIncome.Trim().ToUpper();

            if (trimmedIncome.Contains("1") && trimmedIncome.Contains("LAC"))
            {
                return "1-5 Lac";
            }
            else if (trimmedIncome.Contains("5") && trimmedIncome.Contains("10"))
            {
                return "5-10 Lac";
            }
            else if (trimmedIncome.Contains("10") && trimmedIncome.Contains("25"))
            {
                return "10-25 Lac";
            }
            else if (trimmedIncome.Contains("25") && trimmedIncome.Contains("ABOVE"))
            {
                return "25 Lac and Above";
            }
            else if (trimmedIncome.Contains("SELECT") || trimmedIncome == "." || trimmedIncome == "B" || trimmedIncome == "W" || trimmedIncome == "11")
            {
                return "Other";
            }

            // Default to "Other" if no match is found
            return "Other";
        }


        #endregion

        #region MapACCategory

        private string MapACCategory(string dbACCategory)
        {
            string trimmedCategory = dbACCategory.Trim().ToUpper();

            if (trimmedCategory.Contains("CR") || trimmedCategory.Contains("CR."))
            {
                if (trimmedCategory.Contains(">5"))
                    return ">5 Cr";
                else if (trimmedCategory.Contains("2-5") || trimmedCategory.Contains("2 TO 5"))
                    return "2-5 Cr";
            }
            else if (trimmedCategory.Contains("LAC") || trimmedCategory.Contains("LACS") || trimmedCategory.Contains("L"))
            {
                if (trimmedCategory.Contains("50") && (trimmedCategory.Contains("ABOVE") || trimmedCategory.Contains("AND ABOVE")))
                    return "50 Lacs-2 Cr";
                else if (trimmedCategory.Contains("10") || trimmedCategory.Contains("5-10") || trimmedCategory.Contains("5 TO 10") || trimmedCategory.Contains("10-25"))
                    return "50 Lacs-2 Cr";
                else if (trimmedCategory.Contains("1-5") || trimmedCategory.Contains("1 TO 5") || trimmedCategory.Contains("5"))
                    return "<50 Lacs";
            }
            else if (trimmedCategory.Contains("RETAIL") || trimmedCategory.Contains("SELECT") || trimmedCategory.Contains("0"))
            {
                return "Retails";
            }

            // Default category if no match is found
            return "<50 Lacs";
        }
























        #endregion

        #region MapGender
        private string MapGender(string dbGender)
        {
            switch (dbGender.Trim().ToUpper())
            {
                case "MALE":
                case "M":
                case "MQ":
                case "M`":
                case "MALE.":
                case "MAL`E":
                case "MALEM":
                    return "Male";
                case "FEMALE":
                case "F":
                case "F`":
                    return "Female";
                case "NON-IND":
                    return "Other";
                default:
                    return "Other";
            }
        }





        #endregion

        #region MapTaxStatus
                                                                                                                            private string MapTextStatus(string dbTextStatus)
        {
            string trimmedStatus = dbTextStatus.Trim().ToUpper();

            if (trimmedStatus.Contains("INDIVIDUAL") || trimmedStatus.Contains("RESIDENT INDIVIDUAL"))
            {
                return "Resident Individual";
            }
            else if (trimmedStatus.Contains("MINOR"))
            {
                return "Resident Minor";
            }
            else if (trimmedStatus.Contains("NRI") && trimmedStatus.Contains("REPAR"))
            {
                return "NRI (Repatriable)";
            }
            else if (trimmedStatus.Contains("NRI") && !trimmedStatus.Contains("REPAR"))
            {
                return "NRI (Non-Repatriable)";
            }
            else if (trimmedStatus.Contains("SOLE-PROPRIETOR"))
            {
                return "Sole-Proprietor";
            }
            else
            {
                return "Others";
            }
        }

        #endregion

        #region MapNationality
        private string MapNationalityStatus(string dbNationalityStatus)
        {
            string trimmedStatus = dbNationalityStatus.Trim().ToUpper();

            if (trimmedStatus.Contains("RESIDENT"))
            {
                return "Resident";
            }
            else if (trimmedStatus.Contains("INDIAN"))
            {
                return "Indian";
            }
            else if (trimmedStatus == "NRI")
            {
                return "NRI";
            }
            else if (trimmedStatus.Contains("NON NRI"))
            {
                return "Non NRI";
            }
            else
            {
                // Default to "Other" or any default category if the value doesn't match known categories
                return "Other";
            }
        }

        #endregion

        #endregion

        #region GetOccupationList
        private void fillOccupationList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetOccupationList();


            ddlOccupation.DataSource = dt;
            ddlOccupation.DataTextField = "OCC_NAME";
            ddlOccupation.DataValueField = "OCC_ID";
            ddlOccupation.DataBind();
            ddlOccupation.Items.Insert(0, new ListItem("Select ", ""));


            addfamOccupation.DataSource = dt;
            addfamOccupation.DataTextField = "OCC_NAME";
            addfamOccupation.DataValueField = "OCC_ID";
            addfamOccupation.DataBind();
            addfamOccupation.Items.Insert(0, new ListItem("Select ", ""));




        }
        #endregion

        #region fillCountryList
        private void fillClientCategoryList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().Get_ClientCategory();
            ddlAOClientCategory.DataSource = dt;
            ddlAOClientCategory.DataTextField = "NAME";
            ddlAOClientCategory.DataValueField = "CATEGORY_ID";
            ddlAOClientCategory.DataBind();
            ddlAOClientCategory.Items.Insert(0, new ListItem("Select Category", ""));

            try
            {

                foreach (ListItem item in ddlAOClientCategory.Items)
                {
                    if (item.Text == "Retail")
                    {
                        item.Selected = true; // Mark "Retail" as selected
                        break; // Exit the loop once "Retail" is found and selected
                    }
                }
            }
            catch
            {
                ddlAOClientCategory.SelectedIndex = 0;
            }
        }




        #endregion

        #region fillCountryList
                                        private void fillCountryList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetCountryList();
            ddlMailingCountryList.DataSource = dt;
            ddlMailingCountryList.DataTextField = "COUNTRY_NAME";
            ddlMailingCountryList.DataValueField = "COUNTRY_ID";
            ddlMailingCountryList.DataBind();
            ddlMailingCountryList.Items.Insert(0, new ListItem("Select Country..", ""));



            DataTable dt2 = new WM.Controllers.AccountOpeningController().GetCountryList();
            ddlPCountryList.DataSource = dt2;
            ddlPCountryList.DataTextField = "COUNTRY_NAME";
            ddlPCountryList.DataValueField = "COUNTRY_ID";
            ddlPCountryList.DataBind();
            ddlPCountryList.Items.Insert(0, new ListItem("Select Country", ""));

        }
        #endregion


        #region fillCityList
        private void fillCityList()
        {
            DataTable dt1 = new WM.Controllers.AccountOpeningController().GetCityList();
            AddDefaultItem(dt1, "CITY_NAME", "CITY_ID", "Select City");

            ddlMailingCityList.DataSource = dt1;
            ddlMailingCityList.DataTextField = "CITY_NAME";
            ddlMailingCityList.DataValueField = "CITY_ID";
            ddlMailingCityList.DataBind();


            DataTable dt2 = new WM.Controllers.AccountOpeningController().GetCityList();
            AddDefaultItem(dt2, "CITY_NAME", "CITY_ID", "Select City");

            ddlPCityList.DataSource = dt2;
            ddlPCityList.DataTextField = "CITY_NAME";
            ddlPCityList.DataValueField = "CITY_ID";
            ddlPCityList.DataBind();


            DataTable dt3 = new WM.Controllers.AccountOpeningController().GetCityList();
            AddDefaultItem(dt3, "CITY_NAME", "CITY_ID", "Select City");

            InvestorCityDropDown.DataSource = dt3;
            InvestorCityDropDown.DataTextField = "CITY_NAME";
            InvestorCityDropDown.DataValueField = "CITY_ID";
            InvestorCityDropDown.DataBind();

        }
        #endregion


        #region fillStateList
        private void fillStateList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetStateList();
            AddDefaultItem(dt, "STATE_NAME", "STATE_ID", "Select State");

            ddlMailingStateList.DataSource = dt;
            ddlMailingStateList.DataTextField = "STATE_NAME";
            ddlMailingStateList.DataValueField = "STATE_ID";
            ddlMailingStateList.DataBind();


            DataTable dt2 = new WM.Controllers.AccountOpeningController().GetStateList();
            AddDefaultItem(dt2, "STATE_NAME", "STATE_ID", "Select State");
            ddlPStateList.DataSource = dt2;
            ddlPStateList.DataTextField = "STATE_NAME";
            ddlPStateList.DataValueField = "STATE_ID";
            ddlPStateList.DataBind();
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


        protected void ExitButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/welcome.aspx");
        }

        private DateTime? ParseDate(string dateText, string dateFormat = "dd/MM/yyyy")
        {
            if (string.IsNullOrWhiteSpace(dateText)) return null; // Handle empty or whitespace input

            // Use DateTime.TryParseExact to ensure proper date format handling
            return DateTime.TryParseExact(dateText, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
                ? parsedDate
                : (DateTime?)null;
        }

        public long ParseLongNumber(TextBox textBox)
        {
            return long.TryParse(textBox.Text.Trim(), out long parsedNumber) ? parsedNumber : 0;
        }

        private void SetDropdownValue(DataRow row, string columnName, DropDownList dropdown)
        {
            try
            {
                if (row[columnName] != DBNull.Value)
                {
                    dropdown.SelectedValue = row[columnName].ToString().ToUpper();
                }
            }
            catch
            {
            }
        }

        // Mapping function to normalize the database values to match dropdown options
        private string MapLeadSource(DataRow row, string dbColumnName)
        {
            // Check if the column exists and has a non-null value
            if (row.Table.Columns.Contains(dbColumnName) && row[dbColumnName] != DBNull.Value)
            {
                // Normalize the value to lowercase, trimmed string
                string dbValue = row[dbColumnName].ToString().Trim().ToLower();

                switch (dbValue)
                {
                    case "online":
                        return "Online";
                    case "referral":
                        return "Referral";
                    case "advertisement":
                    case "newspaper":
                        return "Advertisement";
                    case "other":
                    case "otther":
                    case "oot":
                    case "oother":
                    case "othre":
                    case "othjer":
                    case "othe":
                    case "otjher":
                    case "oither":
                    case "othr":
                    case "otheer":
                    case "other30":
                    case "otherre":
                    case "mr":
                    case "m":
                    case "s":
                    case "reference":
                    case "none":
                    case "toher":
                    case "othett":
                    case "ohter":
                    case "otehr":
                    case "otherq":
                    case "otherr":
                    case "othere":
                    case "otq":
                    case "boqpp0426j":
                    case "901007075911":
                    case "other ":
                    case "other   ":
                        return "Other";
                    default:
                        return "Other"; // Default to "Other" for any unmapped values
                }
            }

            // If column doesn't exist or value is null, default to "Other"
            return "Other";
        }


        protected void gvFamily_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Example of handling row commands
            if (e.CommandName == "Edit")
            {
                int index = Convert.ToInt32(e.CommandArgument); 
            }
            else if (e.CommandName == "Delete")
            {
                int index = Convert.ToInt32(e.CommandArgument); 
            }
        }

        public void fillFamilyGrid(string id, string exist)
        {
            DataTable familyData = new AccountOpeningController().GetFamilyGridDataBySourceCode(id, exist);

            if (familyData != null && familyData.Rows.Count > 0)
            {
                try
                {

                // Data exists, bind to GridView
                familyGridView.Visible = true;
                familyGridView.DataSource = familyData;
                familyGridView.DataBind();
                }
                catch(Exception ex)
                {

                }
            }

        }

        private void FillSalutationListDropDown()
        {
            // Call the method to get the data from the stored procedure
            DataTable dt = new WM.Controllers.AccountOpeningController().GetSalutationList();

            // Clear the dropdown list first
            ddlSalutation1.Items.Clear();
            if (dt != null && dt.Rows.Count > 0)
            {
                ddlSalutation1.DataSource = dt;
                ddlSalutation1.DataTextField = "Text";
                ddlSalutation1.DataValueField = "Value";
                ddlSalutation1.DataBind();
                ddlSalutation1.Items.Insert(0, new ListItem("Select", ""));
            }

            ddlSalutation2.Items.Clear();
            if (dt != null && dt.Rows.Count > 0)
            {
                ddlSalutation2.DataSource = dt;
                ddlSalutation2.DataTextField = "Text";
                ddlSalutation2.DataValueField = "Value";
                ddlSalutation2.DataBind();
                ddlSalutation2.Items.Insert(0, new ListItem("Select ", ""));
            }

            ddlSalutation3.Items.Clear();
            if (dt != null && dt.Rows.Count > 0)
            {
                ddlSalutation3.DataSource = dt;
                ddlSalutation3.DataTextField = "Text";
                ddlSalutation3.DataValueField = "Value";
                ddlSalutation3.DataBind();
                ddlSalutation3.Items.Insert(0, new ListItem("Select", ""));
            }
        }


        protected void AddMemberButton_Click(object sender, EventArgs e)
        {
            string accountName = addfamInvestorName.Text.Trim();
            string famMobile = string.IsNullOrEmpty(addfamMobile.Text) ? null : addfamMobile.Text;        

            if (string.IsNullOrEmpty(txtSearchClientCode.Text.Trim()))
            {
                string message = "First load family head data then insert";
                ShowAlert(message);
                lblMessage.Text = message;
                return;
            }

            if (string.IsNullOrEmpty(ddlSalutation3.Text))
            {
                string message = "Member title is empty";
                ShowAlert(message);
                lblMessage.Text = message;
                ddlSalutation3.Focus();
                return;
            }

            if (string.IsNullOrEmpty(addfamInvestorName.Text))
            {
                string message = "Member name is empty";
                ShowAlert(message);
                lblMessage.Text = message;
                addfamInvestorName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(addfamGender.SelectedValue.ToString()))
            {
                string message = "Member gender is empty";
                ShowAlert(message);
                lblMessage.Text = message;
                addfamGender.Focus();
                return;
            }

            if (ValidateMobileFieldMinLength(addfamMobile))
            {
                return;
            }

            else
            {

                btnInsertFamily2();
            }

        }


        protected void UpdateMemberButton_Click(object sender, EventArgs e)
        {
            string famClientCode = ddladdfamExistingInvestor.SelectedValue;
            //int headSource = Convert.ToInt32( txtHeadSourceCode.Text);


            if ( ValidateMobileFieldMinLength(addfamMobile)) return;
            btnUpdateFamilyByClientCode(famClientCode);
            
        }

        protected void ResetMemberButton_Click(object sender, EventArgs e)
        {
            ResetAddFamForm();
        }

        protected void btnInsertFamily2()
        {

            try
            {
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string selectedSalutation1 = ddlSalutation3.SelectedValue;
                string accountName = addfamInvestorName.Text.Trim();
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = txtSearchClientCode.Text;
                string famMobile = string.IsNullOrEmpty(addfamMobile.Text) ? null : addfamMobile.Text;
                string famPan = string.IsNullOrEmpty(addfamPan.Text) ? null : addfamPan.Text;
                string famEmail = string.IsNullOrEmpty(addfamEmail.Text) ? null : addfamEmail.Text;
                //DateTime? famDOB = DateTime.TryParse(addfamDOB.Text, out var parsedDateFam) ? (DateTime?)parsedDateFam : null;

                DateTime? famDOB = DateTime.TryParseExact(addfamDOB.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ? (DateTime?)parsedDate : null;

                int famRel = ParseInt(addfamRelation.SelectedValue);
                string famGName = string.IsNullOrEmpty(addfamGuardianName.Text) ? null : addfamGuardianName.Text;
                string famGPan = string.IsNullOrEmpty(addfamGuardianPan.Text) ? null : addfamGuardianPan.Text;
                int famOcc = ParseInt(addfamOccupation.SelectedValue);
                string famKYC = string.IsNullOrEmpty(addfamKYC.SelectedValue) ? null : addfamKYC.SelectedValue;
                string famApprove = string.IsNullOrEmpty(addfamApproved.SelectedValue) ? null : addfamApproved.SelectedValue;
                string famGender = string.IsNullOrEmpty(addfamGender.SelectedValue) ? null : addfamGender.SelectedValue;
                string famNom = string.IsNullOrEmpty(addfamIsNominee.SelectedValue) ? null : addfamIsNominee.SelectedValue;
                double famAllo = string.IsNullOrEmpty(addfamAllocationPercentage.Text) ? 0 : Convert.ToDouble(addfamAllocationPercentage.Text);
                long AadharValue = ParseLongNumber(addfamAadharNumber);







                string insertResult = new WM.Controllers.AccountOpeningController().InsertFamily2(
dtNumberValue,
existClientCodeValue,
businessCodeValue,
selectedSalutation1,
accountName,
loggedinUser,
clientCodeForMainValue,

famMobile,
famPan,
famEmail,

famDOB,
famRel,
famGName,
famGPan,
famOcc,
famKYC,
famApprove,
famGender,
famNom,
famAllo,
AadharValue

                );
                if (insertResult.Contains("Success"))
                {
                    ShowAlert(insertResult);
                    lblMessageAddFam.Text = insertResult;
                    ResetFamSectionOnUPdate();
                    itfAOGuardianPerson.Attributes.Remove("required");
                    itfAOGuardianNationality.Attributes.Remove("required");
                    itfAOGuardianPANNO.Attributes.Remove("required");

                    string famSrcId = txtHeadSourceCode.Text;
                    string famExistID = txtClientCode.Text;
                    HandleFamilyData(famSrcId, famExistID); 
                    AddMemberButton.Enabled = true;
                    UpdateMemberButton.Enabled = false;


                }

                else if (insertResult.Contains("Duplidate Data: "))
                {
                    ShowAlert(insertResult);
                    if (insertResult.Contains("pan"))
                    {
                        addfamPan.Focus();
                    }

                    if (insertResult.Contains("mobile"))
                    {
                        addfamMobile.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                string errorMessage = $"Kindly fill form properly.";
                ClientScript.RegisterStartupScript(this.GetType(), "insertFamExceptionAlert", $"alert('Kindly fill form properly.');", true);
            }
        }


        protected void btnUpdateFamilyByClientCode(string famClientCode)
        {
            try
            {
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);

                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string selectedSalutation1 = ddlSalutation3.SelectedValue;
                string accountName = addfamInvestorName.Text.Trim();
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = txtSearchClientCode.Text;

                string famMobile = string.IsNullOrEmpty(addfamMobile.Text) ? null : addfamMobile.Text;
                string famPan = string.IsNullOrEmpty(addfamPan.Text) ? null : addfamPan.Text;
                string famEmail = string.IsNullOrEmpty(addfamEmail.Text) ? null : addfamEmail.Text;

                //DateTime? famDOB = DateTime.TryParse(addfamDOB.Text, out var parsedDateFam) ? (DateTime?)parsedDateFam : null;
                DateTime? famDOB = DateTime.TryParseExact(addfamDOB.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ? (DateTime?)parsedDate : null;

                int famRel = ParseInt(addfamRelation.SelectedValue);
                string famGName = string.IsNullOrEmpty(addfamGuardianName.Text) ? null : addfamGuardianName.Text;
                string famGPan = string.IsNullOrEmpty(addfamGuardianPan.Text) ? null : addfamGuardianPan.Text;
                int famOcc = ParseInt(addfamOccupation.SelectedValue);
                string famKYC = string.IsNullOrEmpty(addfamKYC.SelectedValue) ? null : addfamKYC.SelectedValue;
                string famApprove = string.IsNullOrEmpty(addfamApproved.SelectedValue) ? null : addfamApproved.SelectedValue;
                string famGender = string.IsNullOrEmpty(addfamGender.SelectedValue) ? null : addfamGender.SelectedValue;

                string famNom = string.IsNullOrEmpty(addfamIsNominee.SelectedValue) ? null : addfamIsNominee.SelectedValue;
                double famAllo = string.IsNullOrEmpty(addfamAllocationPercentage.Text) ? 0 : Convert.ToDouble(addfamAllocationPercentage.Text);

                long AadharValue = ParseLongNumber(addfamAadharNumber);

                if (string.IsNullOrEmpty(txtSearchClientCode.Text.ToString()))
                {
                    string message = "This member have not a head account";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    return;
                }

                if (string.IsNullOrEmpty(ddladdfamExistingInvestor.SelectedValue.ToString()))
                {
                    string message = "First load existing member data then update";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    return;
                }


                if (string.IsNullOrEmpty(ddlSalutation3.Text))
                {
                    string message = "Member title is empty";
                    ShowAlert(message);
                    ddlSalutation3.Text = message;
                    ddlSalutation3.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(addfamInvestorName.Text))
                {
                    string message = "Member name is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    addfamInvestorName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(addfamGender.SelectedValue.ToString()))
                {
                    string message = "Member gender is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    addfamGender.Focus();
                    return;
                }




                string updateFamResult = new WM.Controllers.AccountOpeningController().UpdateFamilyByClientCode(
dtNumberValue,
existClientCodeValue,
businessCodeValue,
selectedSalutation1,
accountName,
loggedinUser,
clientCodeForMainValue,

famMobile,
famPan,
famEmail,

famDOB,
famRel,
famGName,
famGPan,
famOcc,
famKYC,
famApprove,
famGender,
famNom,
famAllo,
famClientCode,
AadharValue

                );
                if (updateFamResult.Contains("Success"))
                {
                    ShowAlert(updateFamResult);
                    lblMessageAddFam.Text = updateFamResult;
                    ResetFamSectionOnUPdate();

                    itfAOGuardianPerson.Attributes.Remove("required");
                    itfAOGuardianNationality.Attributes.Remove("required");
                    itfAOGuardianPANNO.Attributes.Remove("required");

                    string famSrcId = txtHeadSourceCode.Text;
                    string famExistID = txtClientCode.Text;
                    HandleFamilyData(famSrcId, famExistID);
                    
                    AddMemberButton.Enabled = true;
                    UpdateMemberButton.Enabled = false;


                }

                else if (updateFamResult.Contains("Duplidate Data: "))
                {
                    ShowAlert(updateFamResult);
                    if (updateFamResult.Contains("pan"))
                    {
                        addfamPan.Focus();
                    }

                    if (updateFamResult.Contains("mobile"))
                    {
                        addfamMobile.Focus();
                    }

                }
                else if (updateFamResult.Contains("Error"))
                {
                    ShowAlert(updateFamResult);
                }

            }
            catch (Exception ex)
            {
                string errorMessage = $"Kindly fill form properly.";
                ClientScript.RegisterStartupScript(this.GetType(), "updateFamExceptionAlert", $"alert('Kindly fill form properly');", true);
            }
        }

        protected void ResetAddFamForm()
        {
            // Reset TextBoxes
            addfamInvestorName.Text = string.Empty;
            addfamMobile.Text = string.Empty;
            addfamEmail.Text = string.Empty;
            addfamAadharNumber.Text = string.Empty;

            addfamPan.Text = string.Empty;
            addfamDOB.Text = string.Empty;
            addfamGuardianPan.Text = string.Empty;
            addfamGuardianName.Text = string.Empty;
            addfamAadharNumber.Text = string.Empty;
            addfamAllocationPercentage.Text = string.Empty;

            // Reset DropDownLists
            addfamRelation.SelectedIndex = 0;
            addfamKYC.SelectedIndex = 0;
            addfamApproved.SelectedIndex = 0;
            addfamGender.SelectedIndex = 0;
            addfamIsNominee.SelectedIndex = 0;
            ddladdfamExistingInvestor.SelectedIndex = 0;
            ddlSalutation3.SelectedIndex = 0;
        }

        #region FillFamilyRelationList
        private void FillFamilyRelationList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetRelaitonshipList();
            addfamRelation.DataSource = dt;
            addfamRelation.DataTextField = "RELATION_NAME";
            addfamRelation.DataValueField = "RELATION_ID";
            addfamRelation.DataBind();
            addfamRelation.Items.Insert(0, new ListItem("Select", ""));
        }
        #endregion

        private void FillExistingFamilyList()
        {

            ddladdfamExistingInvestor.Items.Clear();

            if (txtSearchClientCode.Text.Length > 0)
            {
                // Call the GetExistingFamilyList method to fetch data
                DataTable dt = new WM.Controllers.AccountOpeningController().GetFamilyByMainCode(txtSearchClientCode.Text);


                ddladdfamExistingInvestor.DataSource = dt;
                ddladdfamExistingInvestor.DataTextField = "CLIENT_NAME";
                ddladdfamExistingInvestor.DataValueField = "CLIENT_CODE";
                ddladdfamExistingInvestor.DataBind();
                ddladdfamExistingInvestor.Items.Insert(0, new ListItem("Members ", ""));
            }

        }

        private void SetOrAddDropdownValue(DataRow row, string columnName, DropDownList dropdown)
        {
            try
            {
                if (row[columnName] != DBNull.Value)
                {
                    // Get the value from DataRow and normalize to a consistent case
                    string rowValue = row[columnName].ToString().Trim();
                    string normalizedRowValue = rowValue.ToLower(); // Convert to lowercase for comparison

                    // Try to find a matching item in the dropdown
                    ListItem matchingItem = dropdown.Items.Cast<ListItem>()
                                                          .FirstOrDefault(i => i.Value.Trim().ToLower() == normalizedRowValue);

                    if (matchingItem != null)
                    {
                        // If the value exists, set it as the selected value
                        dropdown.SelectedValue = matchingItem.Value;
                    }
                    else
                    {
                        // If the value does not exist, add it to the dropdown and select it
                        dropdown.Items.Add(new ListItem(rowValue, rowValue));
                        dropdown.SelectedValue = rowValue;
                    }
                }
            }
            catch
            {
                // Optionally log or handle the error gracefully
            }
        }

        protected void ddladdfamExistingInvestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected client code from the dropdown
            string currentInvCode = ddladdfamExistingInvestor.SelectedValue;
            ResetFamTextField();

            if (string.IsNullOrEmpty(currentInvCode))
            {
                ResetFamTextField();
                AddMemberButton.Enabled = true;
                UpdateMemberButton.Enabled = false;
            }

            if (!string.IsNullOrEmpty(currentInvCode))
            {
                ResetFamTextField();
                AddMemberButton.Enabled = false;
                UpdateMemberButton.Enabled = true;
                // Call the PSMGetMemberDataByClientCode method to get the data
                DataTable memberData = new AccountOpeningController().GetMemberDataByClientCode(currentInvCode);

                if (memberData.Rows.Count > 0)
                {

                    DataRow row = memberData.Rows[0];
                    string famTitle = GetTextFieldValue(row, "investor_title");
                    ListItem item = ddlSalutation3.Items.FindByValue(famTitle);
                    // Find the matching dropdown item by its lowercase value
                    //ListItem item2 = ddlSalutation3.Items.Cast<ListItem>()
                    //                              .FirstOrDefault(i => i.Value.ToLower() == famTitle.ToLower());
                    SetOrAddDropdownValue(row, "investor_title", ddlSalutation3);


                    //ddlSalutation3.SelectedValue = GetTextFieldValue(row, "investor_title");

                    addfamInvestorName.Text = GetTextFieldValue(row, "investor_name");
                    addfamMobile.Text = GetTextFieldValue(row, "mobile");
                    addfamEmail.Text = GetTextFieldValue(row, "EMAIL");
                    addfamPan.Text = GetTextFieldValue(row, "pan");
                    addfamAadharNumber.Text = 
                        row["AADHAR_CARD_NO"] == DBNull.Value ? string.Empty : row["AADHAR_CARD_NO"].ToString();

                    GetSetDateField(row, "DOB", addfamDOB);

                    if (!string.IsNullOrEmpty(addfamDOB.Text))
                    {

                    ValidateGuardianByDOB(addfamDOB, false);
                    }
                    addfamGuardianName.Text = GetTextFieldValue(row, "g_name");
                    addfamGuardianPan.Text = GetTextFieldValue(row, "G_PAN");

                    #region Handle Member Gender
                    string currentGenderDB_value = GetTextFieldValue(row, "gender");
                   
                    // Get the first character of the gender value from the database
                    string genderChar = currentGenderDB_value.Length > 0 ? currentGenderDB_value[0].ToString().ToUpper() : "";

                   
                    // Loop through the items in the dropdown to find a match
                    bool matchFound = false;
                    foreach (ListItem gedner_item in addfamGender.Items)
                    {
                        // If the first character matches the dropdown item text's first character
                        if (gedner_item.Text.Length > 0 && gedner_item.Text[0].ToString().ToUpper() == genderChar)
                        {
                            addfamGender.SelectedValue = gedner_item.Value;  // Select the matching item
                            matchFound = true;
                            break;
                        }
                    }

                    // If no match was found, set the dropdown to the first index (default)
                    if (!matchFound)
                    {
                        addfamGender.SelectedIndex = 0;
                    }

                    #endregion

                    //SetDropdownValue(row, "GENDER", addfamGender);
                    SetDropdownValue(row, "KYC", addfamKYC);
                    SetDropdownValue(row, "approved", addfamApproved);
                    SetDropdownValue(row, "occ_id", addfamOccupation);
                    SetDropdownValue(row, "OUR_RELATIONSHIP", addfamRelation);
                    SetDropdownValue(row, "is_nominee", addfamIsNominee);
                    addfamAllocationPercentage.Text = row["nominee_per"] == DBNull.Value ? string.Empty : row["nominee_per"].ToString();

                }
                UpdateMemberButton.Enabled = true;
                ddladdfamExistingInvestor.Focus();

            }
        }
        void SetSelectedValue(DropDownList control, object value)
        {
            if (value != DBNull.Value)
            {
                control.SelectedValue = value.ToString();
            }
            else
            {
                control.SelectedIndex = 0;
            }
        }

        private void ShowAlert(string message)
        {
            // Handle null message by using a default value
            message = message ?? "No message provided";

            // Sanitize the message to handle special characters and prevent JavaScript errors
            string sanitizedMessage = message.Replace("'", "\\'").Replace("\n", "\\n");

            // Register the alert script
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "alert",
                $"alert('{sanitizedMessage}');",
                true
            );
        }

        private void SetOnlyDTFieldData(DataRow row)
        { 
        }

        public static string GetSetDataInNumberAndCleanInNumber(DataRow row, string columnName)
        {
            // Check if the column exists and is not null
            if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                string value = row[columnName].ToString();

                // Clean the data to keep only numbers
                string cleanedValue = Regex.Replace(value, @"[^0-9]", "");

                return cleanedValue;
            }

            // Return an empty string if no valid data
            return string.Empty;
        }
       
        protected string SetStringToTextBoxIfEmpty(TextBox currentTextBox, string newValue)
        {
            string currentValue = currentTextBox.Text.ToString();
            if (string.IsNullOrEmpty(currentValue))
            {
                return newValue;
            }
            else
            {
                return currentValue;
            }
        }
        private void SetFieldData(DataRow row)
        {
            fillCountryList();
            fillStateList();
            fillCityList();
            #region getting db value

            string db_message_value = GetTextFieldValue(row, "message");
            string db_doc_id_value = GetTextFieldValue(row, "doc_id");
            string db_AH_client_code_value = GetTextFieldValue(row, "client_code"); // AH107599
            string db_AH_main_code_vlaue = GetTextFieldValue(row, "main_code");
            string db_client_pan_value = GetTextFieldValue(row, "client_pan");
            string db_client_codekyc_value = GetTextFieldValue(row, "client_codekyc");
            string db_source_code_value = GetTextFieldValue(row, "source_code");
            string db_approved_value = GetTextFieldValue(row, "approved");
            string db_approved_flag_value = GetTextFieldValue(row, "approved_flag");
            string db_business_code_value = GetTextFieldValue(row, "business_code");
            string db_rm_name_value = GetTextFieldValue(row, "rm_name");
            string db_BRANCH_CODE_value = GetTextFieldValue(row, "BRANCH_CODE");
            string db_guest_code_value = GetTextFieldValue(row, "guest_code");
            string db_status_value = GetTextFieldValue(row, "status");
            string db_occ_id_value = GetTextFieldValue(row, "occ_id");
            string db_ct_status_value = GetTextFieldValue(row, "ct_status");
            string db_cm_category_id_value = GetTextFieldValue(row, "cm_category_id");
            string db_ct_act_cat_value = GetTextFieldValue(row, "ct_act_cat");
            string db_client_title_value = GetTextFieldValue(row, "title");
            string db_client_name_value = GetTextFieldValue(row, "client_name");
            string db_falther_title_value = GetTextFieldValue(row, "title_other");
            string db_father_name_value = GetTextFieldValue(row, "other");
            string db_others1_value = GetTextFieldValue(row, "others1");
            string db_gender_value = GetTextFieldValue(row, "gender");
            string db_marital_status_value = GetTextFieldValue(row, "marital_status");
            string db_nationality_value = GetTextFieldValue(row, "nationality");
            string db_resident_nri_value = GetTextFieldValue(row, "resident_nri");
            string db_dob_value = GetTextFieldValue(row, "dob");
            string db_annual_income_value = GetTextFieldValue(row, "annual_income");
            string db_lead_type_value = GetTextFieldValue(row, "lead_type");
            string db_g_name_value = GetTextFieldValue(row, "g_name");
            string db_g_nationality_value = GetTextFieldValue(row, "g_nationality");
            string db_g_pan_value = GetTextFieldValue(row, "g_pan");
            string db_add1_value = GetTextFieldValue(row, "add1");
            string db_add2_value = GetTextFieldValue(row, "add2");
            string db_state_id_value = GetTextFieldValue(row, "state_id");
            string db_pincode_value = GetTextFieldValue(row, "pincode");
            string db_city_id_value = GetTextFieldValue(row, "city_id");
            string db_per_add1_value = GetTextFieldValue(row, "per_add1");
            string db_per_add2_value = GetTextFieldValue(row, "per_add2");
            string db_per_state_id_value = GetTextFieldValue(row, "per_state_id");
            string db_per_city_id_value = GetTextFieldValue(row, "per_city_id");
            string db_per_pincode_value = GetTextFieldValue(row, "per_pincode");
            string db_overseas_add_value = GetTextFieldValue(row, "overseas_add");
            string db_aadhar_card_no_value = GetTextFieldValue(row, "aadhar_card_no");
            string db_email_value = GetTextFieldValue(row, "email");
            string db_FAX_value = GetTextFieldValue(row, "FAX");
            string db_office_email_value = GetTextFieldValue(row, "office_email");
            string db_tel1_value = GetTextFieldValue(row, "tel1");
            string db_tel2_value = GetTextFieldValue(row, "tel2");
            string db_std1_value = GetTextFieldValue(row, "std1");
            string db_std2_value = GetTextFieldValue(row, "std2");
            string db_mobile_no_value = GetTextFieldValue(row, "mobile_no");
            string db_ref_name1_value = GetTextFieldValue(row, "ref_name1");
            string db_ref_name2_value = GetTextFieldValue(row, "ref_name2");
            string db_ref_name3_value = GetTextFieldValue(row, "ref_name3");
            string db_ref_name4_value = GetTextFieldValue(row, "ref_name4");
            string db_ref_mob1_value = GetTextFieldValue(row, "ref_mob1");
            string db_ref_mob2_value = GetTextFieldValue(row, "ref_mob2");
            string db_ref_mob3_value = GetTextFieldValue(row, "ref_mob3");
            string db_ref_mob4_value = GetTextFieldValue(row, "ref_mob4");

            string db_mailing_add_city_value = GetTextFieldValue(row, "Mailing_City");
            string db_mailing_add_state_value = GetTextFieldValue(row, "Mailing_State");
            string db_mailing_add_count_value = GetTextFieldValue(row, "Mailing_Country");

            string db_perm_add_city_value = GetTextFieldValue(row, "Permanent_City");
            string db_perm_add_state_value = GetTextFieldValue(row, "Permanent_State");
            string db_perm_add_count_value = GetTextFieldValue(row, "Permanent_Country");
            #endregion

            string psmMsg = GetTextFieldValue(row, "message");

            #region Settign DB vlaue to Fields
            //ShowAlert(psmMsg);
            lblMessage.Text = psmMsg;
            txtDTNumber.Text = db_doc_id_value;
            txtGuestCode.Text = db_guest_code_value;
            //ApprovalStatus.Text = db_approved_flag_value;
            txtBusinessCode.Text = db_business_code_value;  // based on this fill name and branch
            txtBusinessCodeName.Text = db_business_code_value;
            txtBusinessCodeBranch.Text = db_business_code_value;

            //Search my AH, Pan or existing fields
            txtSearchClientCode.Text = db_AH_client_code_value;
            txtSearchPan.Text = db_client_pan_value;
            txtClientCode.Text = db_client_codekyc_value;
            ExistingClientCodeInv.Text = db_client_codekyc_value;

            lblHolderMessage.Text = psmMsg;
            txtHeadSourceCode.Text = db_source_code_value;
            ddlTaxStatus.SelectedValue = MapTextStatus(row["STATUS"].ToString().ToUpper());
            GetDropDownValue(row, "OCC_Id", ddlOccupation);
            GetDropDownValue(row, "CT_STATUS", ddlAOCategoryStatus);
            GetDropDownValue(row, "CM_category_id", ddlAOClientCategory);
            try
            {

                GetDropDownValue(row, "CT_ACT_CAT", ddlAOACCategory);
            }
            catch (Exception ex)
            {

                string currentChangedAcCat = MapACCategory(db_ct_act_cat_value);

                if (ddlAOACCategory.Items.FindByText(currentChangedAcCat) != null)
                {
                    ddlAOACCategory.SelectedValue = ddlAOACCategory.Items.FindByText(currentChangedAcCat).Value;
                }

            }

            GetDropDownValue(row, "title", ddlSalutation1);
            txtAccountName.Text = db_client_name_value;
            GetDropDownValue(row, "title_other", ddlSalutation2);
            txtFatherSpouse.Text = db_father_name_value;
            txtOther1.Text = db_others1_value;
            ddlAOGender.SelectedValue = MapGender(row["GENDER"].ToString());
            GetDropDownValue(row, "MARITAL_STATUS", ddlAOMaritalStatus);
            ddlAONationality.SelectedValue = row["nationality"].ToString().ToUpper();
            ddlAOResidentNRI.SelectedValue = row["RESIDENT_NRI"].ToString();
            GetSetDateField(row, "DOB", cldDOB);
            ddlAOAnnualIncome.SelectedValue = MapAnnualIncome(row["ANNUAL_INCOME"].ToString());
            itfAOClientPan.Text = db_client_pan_value;
            ddlLeadSource.SelectedValue = MapLeadSource(row, "lead_type");

            itfAOGuardianPerson.Text = db_g_name_value;
            itfAOGuardianNationality.Text = db_g_nationality_value;
            itfAOGuardianPANNO.Text = db_g_pan_value;

            itfAOMAddress1.Text = db_add1_value;
            itfAOMAddress2.Text = db_add2_value;
            ddlMailingCountryList.SelectedValue = db_mailing_add_count_value;
            //ddlMailingStateList.SelectedValue = db_mailing_add_state_value;
            //ddlMailingCityList.SelectedValue = db_mailing_add_city_value;
            //txtMailingPin.Text = db_pincode_value;
            txtPAddress1.Text = db_per_add1_value;
            txtPAddress2.Text = db_per_add2_value;
            ddlPCountryList.SelectedValue = db_perm_add_count_value;
            //ddlPStateList.SelectedValue = db_perm_add_state_value;
            //ddlPCityList.SelectedValue = db_perm_add_city_value;
            //txtPPin.Text = db_per_pincode_value;
            txtOverseasAdd.Text = db_overseas_add_value;
            txtFax.Text = GetSetDataInNumberAndCleanInNumber(row, "FAX"); // db_FAX_value
            txtAadhar.Text = GetSetDataInNumberAndCleanInNumber(row, "AADHAR_CARD_NO"); // db_aadhar_card_no_value
            txtEmail.Text = db_email_value;
            txtOfficialEmail.Text = db_office_email_value;
            PhoneOfficeSTD.Text = db_std1_value;
            PhoneResSTD.Text = db_std2_value;
            PhoneOfficeNumber.Text = GetSetDataInNumberAndCleanInNumber(row, "TEL1"); // db_tel1_value
            PhoneResNumber.Text = GetSetDataInNumberAndCleanInNumber(row, "TEL2"); // db_tel2_value
            MobileNo.Text = GetSetDataInNumberAndCleanInNumber(row, "MOBILE_NO"); // db_mobile_no_value
            ReferenceName1.Text = db_ref_name1_value;
            ReferenceName2.Text = db_ref_name2_value;
            ReferenceName3.Text = db_ref_name3_value;
            ReferenceName4.Text = db_ref_name4_value;
            ReferenceMobileNo1.Text = GetSetDataInNumberAndCleanInNumber(row, "ref_mob1");
            ReferenceMobileNo2.Text = GetSetDataInNumberAndCleanInNumber(row, "ref_mob2");
            ReferenceMobileNo3.Text = GetSetDataInNumberAndCleanInNumber(row, "ref_mob3");
            ReferenceMobileNo4.Text = GetSetDataInNumberAndCleanInNumber(row, "ref_mob4"); 



            #endregion

            #region Approval status check
            string clientCode = GetTextFieldValue(row, "client_code");
            string mainCode = GetTextFieldValue(row, "MAIN_CODE");
            string approved = GetTextFieldValue(row, "approved");
            string approvedClientHead = (clientCode == mainCode && approved == "YES") ? "Approved" : "Not Approved.";
            ApprovalStatus.Text = $"{(approvedClientHead.ToUpper().Contains("NOT") ? "<span style='color:black;'>Not Approved</span>" : "<span style='color:green;'>Approved</span>")}";
            btnApprove.Enabled = approvedClientHead.ToUpper().Contains("NOT") ? true : false;

            #endregion

            #region Hndle RM and Branch by Business Code
            if (!string.IsNullOrEmpty(db_business_code_value))
            {
                UpdateBusinessCodeDetails(db_business_code_value);
            }
            #endregion

            #region Handle Guardian by DOB
            if (!string.IsNullOrEmpty(cldDOB.Text)) {
                ValidateGuardianByDOB(cldDOB, true);
            }

            #endregion



            #region Mailing/Permanent Country, State, City Filter after set

            string currentDB_MCountryID = db_mailing_add_count_value;
            string currentDB_MStateID = db_mailing_add_state_value;
            string currentDB_MCityID = db_mailing_add_city_value;

            try
            {
                if (ddlMailingCountryList.Items.FindByValue(currentDB_MCountryID) != null)
                {
                    try
                    {
                        // Populate the state dropdown based on the selected country
                        PopulateStateDropDownForAddress(Convert.ToInt32(currentDB_MCountryID), ddlMailingStateList);

                        if (ddlMailingStateList.Items.FindByValue(currentDB_MStateID) != null)
                        {
                            try
                            {
                                // Set the state dropdown selected value
                                ddlMailingStateList.SelectedValue = db_mailing_add_state_value;

                                // Populate the city dropdown based on the selected state
                                PopulateCityDropDownForAddress(Convert.ToInt32(currentDB_MStateID), ddlMailingCityList);

                                if (ddlMailingCityList.Items.FindByValue(currentDB_MCityID) != null)
                                {
                                    try
                                    {
                                        // Set the city dropdown selected value
                                        ddlMailingCityList.SelectedValue = db_mailing_add_city_value;
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }



            string currentDB_P_CountryID = db_perm_add_count_value;
            string currentDB_P_StateID = db_perm_add_state_value;
            string currentDB_P_CityID = db_perm_add_city_value;

            try
            {
                if (ddlPCountryList.Items.FindByValue(currentDB_P_CountryID) != null)
                {
                    try
                    {
                        // Populate the state dropdown based on the selected country
                        PopulateStateDropDownForAddress(Convert.ToInt32(currentDB_P_CountryID), ddlPStateList);

                        if (ddlPStateList.Items.FindByValue(currentDB_P_StateID) != null)
                        {
                            try
                            {
                                // Set the state dropdown selected value
                                ddlPStateList.SelectedValue = db_perm_add_state_value;

                                try
                                {
                                    // Populate the city dropdown based on the selected state
                                    PopulateCityDropDownForAddress(Convert.ToInt32(currentDB_P_StateID), ddlPCityList);

                                    if (ddlPCityList.Items.FindByValue(currentDB_P_CityID) != null)
                                    {
                                        try
                                        {
                                            // Set the city dropdown selected value
                                            ddlPCityList.SelectedValue = db_perm_add_city_value;
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }

            #endregion

            #region Handle pin code by country
            if (ddlMailingCountryList.Items.Count > 0){ 
                HandlePinCodeValidation(ddlMailingCountryList.Items.ToString(), txtMailingPin);
                try
                {
                    txtMailingPin.Text = db_pincode_value;
                }
                catch(Exception ex)
                {
                    
                }
            }

            if (ddlPCountryList.Items.Count > 0)
            {
                HandlePinCodeValidation(ddlPCountryList.Items.ToString(), txtPPin);
                try
                {
                    txtPPin.Text = db_per_pincode_value;
                }
                catch (Exception ex)
                {

                }
            }

            #endregion

            #region Handle Existing Investor and Family Grid 
            string famSrcId = db_source_code_value;
            string famExistID = db_client_codekyc_value;
            if (!string.IsNullOrWhiteSpace(famSrcId) && !string.IsNullOrWhiteSpace(famExistID)) {
                HandleFamilyData(famSrcId, famExistID);
            }

            #endregion

            #region Handle Advisory and Fill 
            if (txtSearchClientCode.Text != null)
            {
                try
                {

                FillAdvisoryDataByAH(txtSearchClientCode.Text, txtSearchPan.Text);
                }catch(Exception ex)
                {

                }
            }
            #endregion

            txtAccountName.Focus();
        }


        protected void EnableDisablField(bool enable)
        {
            try
            {
                if (enable)
                {
                    txtDTNumber.Enabled = true;
                    txtGuestCode.Enabled = true;
                    txtSearchClientCode.Enabled = true;
                    txtSearchPan.Enabled = true;
                    //txtClientCode.Enabled = true;
                    ExistingClientCodeInv.Enabled = true;
                    ddlTaxStatus.Enabled = true;
                    ddlOccupation.Enabled = true;
                    ddlAOCategoryStatus.Enabled = true;
                    ddlAOClientCategory.Enabled = true;
                    ddlAOACCategory.Enabled = true;
                    ddlSalutation1.Enabled = true;
                    txtAccountName.Enabled = true;
                    ddlSalutation2.Enabled = true;
                    txtFatherSpouse.Enabled = true;
                    txtOther1.Enabled = true;
                    ddlAOGender.Enabled = true;
                    ddlAOMaritalStatus.Enabled = true;
                    ddlAONationality.Enabled = true;
                    ddlAOResidentNRI.Enabled = true;
                    cldDOB.Enabled = true;
                    ddlAOAnnualIncome.Enabled = true;
                    itfAOClientPan.Enabled = true;
                    ddlLeadSource.Enabled = true;
                    itfAOGuardianPerson.Enabled = true;
                    itfAOGuardianNationality.Enabled = true;
                    itfAOGuardianPANNO.Enabled = true;
                    itfAOMAddress1.Enabled = true;
                    itfAOMAddress2.Enabled = true;
                    ddlMailingCountryList.Enabled = true;
                    ddlMailingStateList.Enabled = true;
                    ddlMailingCityList.Enabled = true;
                    txtMailingPin.ReadOnly = true;
                    txtPAddress1.Enabled = true;
                    txtPAddress2.Enabled = true;
                    ddlPCountryList.Enabled = true;
                    ddlPStateList.Enabled = true;
                    ddlPCityList.Enabled = false;
                    txtPPin.ReadOnly = true;
                    CheckBox1.Enabled = true;
                    txtOverseasAdd.Enabled = true;
                    txtFax.Enabled = true;
                    txtAadhar.Enabled = true;
                    txtEmail.Enabled = true;
                    txtOfficialEmail.Enabled = true;
                    PhoneOfficeSTD.Enabled = true;
                    PhoneResSTD.Enabled = true;
                    PhoneOfficeNumber.Enabled = true;
                    PhoneResNumber.Enabled = true;
                    MobileNo.Enabled = true;
                    ReferenceName1.Enabled = true;
                    ReferenceName2.Enabled = true;
                    ReferenceName3.Enabled = true;
                    ReferenceName4.Enabled = true;
                    ReferenceMobileNo1.Enabled = true;
                    ReferenceMobileNo2.Enabled = true;
                    ReferenceMobileNo3.Enabled = true;
                    ReferenceMobileNo4.Enabled = true;
                }
                else if (!enable)
                {
                    btnSave.Enabled = false;
                    btnUpdate.Enabled = false;
                    btnApprove.Enabled = false;
                    txtDTNumber.Enabled = true;
                    txtGuestCode.Enabled = true;
                    txtSearchClientCode.Enabled = true;
                    txtSearchPan.Enabled = true;
                    txtClientCode.Enabled = true;
                    //ExistingClientCodeInv.Enabled = false;
                    //lblHolderMessage.Enabled = false;
                    //txtHeadSourceCode.Enabled = false;
                    ddlTaxStatus.Enabled = false;
                    ddlOccupation.Enabled = false;
                    ddlAOCategoryStatus.Enabled = false;
                    ddlAOClientCategory.Enabled = false;
                    ddlAOACCategory.Enabled = false;
                    ddlSalutation1.Enabled = false;
                    txtAccountName.Enabled = false;
                    ddlSalutation2.Enabled = false;
                    txtFatherSpouse.Enabled = false;
                    txtOther1.Enabled = false;
                    ddlAOGender.Enabled = false;
                    ddlAOMaritalStatus.Enabled = false;
                    ddlAONationality.Enabled = false;
                    ddlAOResidentNRI.Enabled = false;
                    cldDOB.Enabled = false;
                    ddlAOAnnualIncome.Enabled = false;
                    itfAOClientPan.Enabled = false;
                    ddlLeadSource.Enabled = false;
                    itfAOGuardianPerson.Enabled = false;
                    itfAOGuardianNationality.Enabled = false;
                    itfAOGuardianPANNO.Enabled = false;
                    itfAOMAddress1.Enabled = false;
                    itfAOMAddress2.Enabled = false;
                    ddlMailingCountryList.Enabled = false;
                    ddlMailingStateList.Enabled = false;
                    ddlMailingCityList.Enabled = false;
                    txtMailingPin.Enabled = false;
                    txtPAddress1.Enabled = false;
                    txtPAddress2.Enabled = false;
                    ddlPCountryList.Enabled = false;
                    ddlPStateList.Enabled = false;
                    ddlPCityList.Enabled = false;
                    txtPPin.Enabled = false;
                    CheckBox1.Enabled= false;
                    txtOverseasAdd.Enabled = false;
                    txtFax.Enabled = false;
                    txtAadhar.Enabled = false;
                    txtEmail.Enabled = false;
                    txtOfficialEmail.Enabled = false;
                    PhoneOfficeSTD.Enabled = false;
                    PhoneResSTD.Enabled = false;
                    PhoneOfficeNumber.Enabled = false;
                    PhoneResNumber.Enabled = false;
                    MobileNo.Enabled = false;
                    ReferenceName1.Enabled = false;
                    ReferenceName2.Enabled = false;
                    ReferenceName3.Enabled = false;
                    ReferenceName4.Enabled = false;
                    ReferenceMobileNo1.Enabled = false;
                    ReferenceMobileNo2.Enabled = false;
                    ReferenceMobileNo3.Enabled = false;
                    ReferenceMobileNo4.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                lblMessage.Text = $"An error occurred: {ex.Message}";
            }


        }

        private void HandleFamilyData(string famSrcId, string famExistID)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(famSrcId) && !string.IsNullOrWhiteSpace(famExistID))
                {
                    FillExistingInvestorList(famSrcId, famExistID);
                    fillFamilyGrid(famSrcId, famExistID);

                    if (ddladdfamExistingInvestor.Items.Count == 0)
                    {
                        ddladdfamExistingInvestor.Items.Add(new ListItem("", ""));
                    }

                    ddladdfamExistingInvestor.SelectedIndex = 0;
                    AddMemberButton.Enabled = true;
                    UpdateMemberButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                // Handle exception or log error
            }
        }

        private void SetAdvisoryFieldData(DataRow row)
        {
            try
            {
                // Show message from the database if available
                string DB_ADV_MSG = GetTextFieldValue(row, "message");
                string DB_MUT_CODE = GetTextFieldValue(row, "mut_code");
                string DB_SCH_CODE = GetTextFieldValue(row, "sch_code");
                string DB_BANK_NAME= GetTextFieldValue(row, "bank_name");
                string DB_REMARK = GetTextFieldValue(row, "bank_ac_no");
                string DB_AMT = GetTextFieldValue(row, "amount");
                string DB_PAYMENT_MODE = GetTextFieldValue(row, "payment_mode");
                string DB_CHEQUE_NO = GetTextFieldValue(row, "cheque_no");

                // Check and set cheque date
                if (row.Table.Columns.Contains("cheque_date") && row["cheque_date"] != DBNull.Value)
                {
                    GetSetDateField(row, "cheque_date",txtChequeDate);
                }

         
                string partialMatchValue = DB_MUT_CODE + "$" + DB_SCH_CODE;

                // Loop through the dropdown items to find a match
                foreach (ListItem item in ddlMutualPlanType.Items)
                {
                    if (item.Value.StartsWith(partialMatchValue))
                    {
                        ddlMutualPlanType.SelectedValue = item.Value; // Select the matching item
                        break; // Exit the loop once a match is found
                    }
                }

                ddlMutualBank.SelectedItem.Text = DB_BANK_NAME;
                txtRemark.Text = DB_REMARK;
                txtAmount.Text = DB_AMT;
                renewal.Checked = true;
                if(!string.IsNullOrEmpty(DB_PAYMENT_MODE) && DB_PAYMENT_MODE.ToUpper() == "C")
                {
                    if(DB_PAYMENT_MODE.ToUpper() == "C")
                    cheque.Checked = true;
                    ChequeLabel.Text = "Cheque No <span class='text-danger'>*</span>";
                    ChequeDatedLabel.Text = "Cheque Dated <span class='text-danger'>*</span>";

                }
                else
                {
                    draft.Checked = true;
                    ChequeLabel.Text = "Draft No <span class='text-danger'>*</span>";
                    ChequeDatedLabel.Text = "Draft Dated <span class='text-danger'>*</span>";

                }

                txtChequeNo.Text = DB_CHEQUE_NO;

            }
            catch (Exception ex)
            {
                // Handle any exceptions and provide feedback
                ShowAlert("An error occurred while setting advisory field data: " + ex.Message);
            }
        }

        private void FillClientDataByAHNum(string id)
        {
            try
            {
                DataTable dt = new AccountOpeningController().GetClientDataByID(id);
                if(dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string message = GetTextFieldValue(row, "message");
                    if (message.Contains("Valid Data"))
                    {
                        ResetFormFields1();
                        SetFieldData(row); 
                    }
                    else
                    {
                        ShowAlert("Not Valid: " + message);
                    }
                }
                else 
                {
                    ShowAlert("No Data!");
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowAlert("An error occurred: " + ex.Message);
            }
        }

        private void FillClientDataByDTNumber(string id)
        {
            try
            {
                DataTable dt = new AccountOpeningController().GetClientDataByDTNumber(id);                
                if (dt == null || dt.Rows.Count <= 0)
                {
                    string emptyMsg = "No Data!";
                    ShowAlert(emptyMsg);
                }

                else if (dt.Rows.Count > 0)
                {

                    DataRow row = dt.Rows[0];
                    string message = dt.Rows[0]["message"].ToString();

                    string DtNotMsg = "not exist";
                    string onlyDTmsg = "Valid Data: DT Exist";
                    string onlyInvMsg = "Only Investor";
                    string onlyINVCMMsg = "Only Investor and Client Master";
                    string onlyFullMsg = "Account Exist";
                    string INVALID_DT_DATA_MSG = "Invalid Data";

                    if (message.Contains(DtNotMsg))
                    {

                        ShowAlert(message);
                        txtDTNumber.Text = id;
                        txtBusinessCode.Text = string.Empty;
                        txtBusinessCodeName.Text = string.Empty;
                        txtBusinessCodeBranch.Text = string.Empty;
                        txtGuestCode.Text = string.Empty;

                    }
                    else if (message.Contains(onlyDTmsg))
                    {
                       // ShowAlert(message);
                        string rccDTValue = GetTextFieldValue(row, "COMMON_ID");
                        string rccDOCValue = GetTextFieldValue(row, "DOC_ID");
                        string rccBusiValue = GetTextFieldValue(row, "business_code");
                        string rccRmValue = GetTextFieldValue(row, "rm_name");
                        string rccRmBranchValue = GetTextFieldValue(row, "BRANCH_CODE");
                        string rccCommondValue = GetTextFieldValue(row, "guest_code");

                        string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                        string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");

                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");

                        txtDTNumber.Text = id;
                        txtBusinessCode.Text = !string.IsNullOrEmpty(rccBusiValue) ? rccBusiValue : string.Empty;
                        //txtBusinessCodeName.Text = !string.IsNullOrEmpty(rccRmValue) ? rccRmValue : string.Empty;
                        //txtBusinessCodeBranch.Text = !string.IsNullOrEmpty(rccRmBranchValue) ? rccRmBranchValue : string.Empty;
                        UpdateBusinessCodeDetails(rccBussRMCodeValue);
                        txtGuestCode.Text = !string.IsNullOrEmpty(rccCommondValue) ? rccCommondValue : string.Empty;
                        ApprovalStatus.Text = string.Empty;
                    }
                    
                    else if (message.Contains(onlyInvMsg))
                    {
                        string rccDTValue = GetTextFieldValue(row, "COMMON_ID");
                        string rccDOCValue = GetTextFieldValue(row, "DOC_ID");
                        string rccBusiValue = GetTextFieldValue(row, "business_code");
                        string rccRmValue = GetTextFieldValue(row, "rm_name");
                        string rccRmBranchValue = GetTextFieldValue(row, "BRANCH_CODE");
                        string rccCommondValue = GetTextFieldValue(row, "guest_code");

                        string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                        string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");


                        string rcvInvValue = GetTextFieldValue(row, "inv_code");


                        ResetFormFields1();
                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                        //HandleSelectedInvestor(rcvInvValue);

                        txtDTNumber.Text = !string.IsNullOrEmpty(rccDTValue) ? rccDTValue : string.Empty;
                        //txtBusinessCode.Text = !string.IsNullOrEmpty(rccBusiValue) ? rccBusiValue : string.Empty;
                        //txtBusinessCodeName.Text = !string.IsNullOrEmpty(rccRmValue) ? rccRmValue : string.Empty;
                        //txtBusinessCodeBranch.Text = !string.IsNullOrEmpty(rccRmBranchValue) ? rccRmBranchValue : string.Empty;
                        UpdateBusinessCodeDetails(rccBussRMCodeValue);
                        txtGuestCode.Text = !string.IsNullOrEmpty(rccCommondValue) ? rccCommondValue : string.Empty;

                    }


                   
                    else if (message.Contains(onlyFullMsg))
                    {

                        string rccDTValue = GetTextFieldValue(row, "COMMON_ID");
                        string rccDOCValue = GetTextFieldValue(row, "DOC_ID");
                        string rccBusiValue = GetTextFieldValue(row, "business_code");
                        string rccRmValue = GetTextFieldValue(row, "rm_name");
                        string rccRmBranchValue = GetTextFieldValue(row, "BRANCH_CODE");
                        string rccCommondValue = GetTextFieldValue(row, "guest_code");

                        string rcvInvValue = GetTextFieldValue(row, "inv_code");
                        string rcvCmValue = GetTextFieldValue(row, "cm_client_code");
                        string rcvCtValue = GetTextFieldValue(row, "ct_client_code");

                        string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                        string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");


                        ResetFormFields1();
                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                        if (rcvCtValue.Contains("AH"))
                        {
                            FillClientDataByAHNum(rcvCtValue);
                        }

                        txtDTNumber.Text = !string.IsNullOrEmpty(rccDTValue) ? rccDTValue : string.Empty;
                        //txtBusinessCode.Text = !string.IsNullOrEmpty(rccBusiValue) ? rccBusiValue : string.Empty;
                        //txtBusinessCodeName.Text = !string.IsNullOrEmpty(rccRmValue) ? rccRmValue : string.Empty;
                        //txtBusinessCodeBranch.Text = !string.IsNullOrEmpty(rccRmBranchValue) ? rccRmBranchValue : string.Empty;
                        UpdateBusinessCodeDetails(rccBussRMCodeValue);
                        txtGuestCode.Text = !string.IsNullOrEmpty(rccCommondValue) ? rccCommondValue : string.Empty;
                        
                    }


                    else
                    {

                        ShowAlert(message);
                        txtDTNumber.Text = id;
                        txtDTNumber.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception or showing an error message for better error handling
                // Example: Console.WriteLine(ex.Message);
            }

        }
       
        private void AutoSelectOrInsertDropdownItem(DropDownList dropdown, string itemText)
        {
            bool itemFound = false;

            // Check if the item already exists in the dropdown (case-insensitive comparison)
            foreach (ListItem item in dropdown.Items)
            {
                if (string.Equals(item.Text, itemText, StringComparison.OrdinalIgnoreCase))
                {
                    item.Selected = true;  // Set the item as selected
                    itemFound = true;      // Mark that the item was found
                    break;
                }
            }

            // If the item does not exist, insert it and set it as selected
            if (!itemFound)
            {
                ListItem newItem = new ListItem(itemText, itemText);
                dropdown.Items.Add(newItem);  // Add the new item
                newItem.Selected = true;      // Set the newly added item as selected
            }
        }

       

        protected void oneClientSearch_Click(object sender, EventArgs e)
        {
            string searchOneClientCode = txtSearchClientCode.Text.Trim();
            string searchOnePan = txtSearchPan.Text.Trim();
            string existingClientCode = txtClientCode.Text.Trim();
            string loginid = Session["LoginId"]?.ToString();

            string alertMsg = string.Empty;

            

            // Check if both Client Code and PAN are empty
            if (string.IsNullOrWhiteSpace(searchOneClientCode) && string.IsNullOrWhiteSpace(searchOnePan) && string.IsNullOrEmpty(existingClientCode))
            {
                ResetFormFields1();
                alertMsg = "Please provide AH Client Code or PAN!";
                ShowAlert(alertMsg);
                lblMessage.Text = alertMsg;
            }
            else
            {
                AccountOpeningController controller = new AccountOpeningController();

                // Call the method Getbycientcodepan using the controller instance
                DataTable result = controller.Getbycientcodepan(searchOneClientCode, searchOnePan, existingClientCode, loginid);

                if (result.Rows.Count > 0)
                {
                    string currectAHByIdPan = result.Rows[0]["client_code"].ToString();  // Correct column name to string
                    if (!string.IsNullOrEmpty(currectAHByIdPan))
                    {
                        FillClientDataByAHNum(currectAHByIdPan);
                    }
                }
                else
                {
                    ShowAlert("No Data Exist");
                }

            }
        }


        protected void oneClientSearchByDT_Click(object sender, EventArgs e)
        {
            string commonID = txtDTNumber.Text.Trim();
            if (string.IsNullOrWhiteSpace(commonID))
            {
                //ResetFormFields1();
                string emptyMsg = "Please provide DT Number!";
                lblMessage.Text = emptyMsg;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtAccountName.Text))
                {
                    ResetFormFields1();
                }
                
                FillClientDataByDTNumber(commonID);
            }

        }

        public void SelectDropDownByFieldValue(DataRow row, string fieldName, DropDownList dropDownControl, Func<int, int> controlFunction)
        {
            try
            {
                // Convert the field value to an integer
                int fieldValue = Convert.ToInt32(row[fieldName]);

                // Call the control function to get the related value (e.g., country code) based on the field value
                int result = controlFunction(fieldValue);

                // Assign the result to the DropDownList's SelectedValue, converting to string
                dropDownControl.SelectedValue = result.ToString();
            }
            catch
            {
                // If an error occurs (e.g., the value isn't found), set the DropDownList to its default state
                dropDownControl.SelectedIndex = -1;
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string taxStatus = ddlTaxStatus.SelectedValue;
                int occupationValue = ParseInt(ddlOccupation.SelectedValue);
                string statusCategoryValue = ddlAOCategoryStatus.SelectedValue;
                string clientCategoryValue = ddlAOClientCategory.SelectedValue;
                int accountCategoryValue = ParseInt(ddlAOACCategory.SelectedValue);
                string selectedSalutation1 = ddlSalutation1.SelectedValue;
                string accountName = txtAccountName.Text.Trim();
                string selectedSalutation2 = ddlSalutation2.SelectedValue;
                string accountFatherName = txtFatherSpouse.Text.Trim();
                string accountOtherValue = txtOther1.Text.Trim();
                string gender = ddlAOGender.SelectedValue;
                string maritalStatus = ddlAOMaritalStatus.SelectedValue;
                string nationality = ddlAONationality.SelectedValue;
                string residentNri = ddlAOResidentNRI.SelectedValue;
                //DateTime? dob = DateTime.TryParse(cldDOB.Text, out var parsedDate) ? (DateTime?)parsedDate : null;
                DateTime? dob = DateTime.TryParseExact(cldDOB.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ? (DateTime?)parsedDate : null;

                string annualIncome = ddlAOAnnualIncome.SelectedValue;
                string clientPan = itfAOClientPan.Text.Trim();
                string leadType = ddlLeadSource.SelectedValue;
                string guardianPerson = itfAOGuardianPerson.Text.Trim();
                string guardianPersonNationality = itfAOGuardianNationality.Text.Trim();
                string guardianPersonPan = itfAOGuardianPANNO.Text.Trim();
                string mailingAddress1 = itfAOMAddress1.Text.Trim();
                string mailingAddress2 = itfAOMAddress2.Text.Trim();
                string mailingState = ddlMailingStateList.SelectedValue;
                string mailingCity = ddlMailingCityList.SelectedValue;
                string mailingPinCode = txtMailingPin.Text.Trim();


                // Permanent Address
                string permanentAddress1 = txtPAddress1.Text.Trim();
                string permanentAddress2 = txtPAddress2.Text.Trim();
                string permanentState = ddlPStateList.SelectedValue;
                string permanentCity = ddlPCityList.SelectedValue;
                string PermanentPinCode = txtPPin.Text.Trim();


                // If permanent address is the same as mailing address
                bool PermanentAddressSameAsMailing = CheckBox1.Checked;
                if (PermanentAddressSameAsMailing)
                {
                    mailingAddress1 = permanentAddress1;
                    mailingAddress2 = permanentAddress2;
                    mailingState = permanentState;
                    mailingCity = permanentCity;
                    mailingPinCode = PermanentPinCode;
                }

                string NRIAddress = txtOverseasAdd.Text.Trim();
                string FaxValue = txtFax.Text.Trim();
                long AadharValue = ParseLongNumber(txtAadhar);
                string EmailValue = txtEmail.Text.Trim();
                string EmailOfficialValue = txtOfficialEmail.Text.Trim();
                string PhoneOfficeSTDValue = string.IsNullOrEmpty(PhoneOfficeSTD.Text.Trim()) ? null : PhoneOfficeSTD.Text.Trim();
                string PhoneOfficeNumberValue = string.IsNullOrEmpty(PhoneOfficeNumber.Text.Trim()) ? null : PhoneOfficeNumber.Text.Trim();
                string PhoneResSTDValue = string.IsNullOrEmpty(PhoneResSTD.Text.Trim()) ? null : PhoneResSTD.Text.Trim();
                string PhoneResNumberValue = string.IsNullOrEmpty(PhoneResNumber.Text.Trim()) ? null : PhoneResNumber.Text.Trim();
                string MobileNoValue = string.IsNullOrEmpty(MobileNo.Text.Trim()) ? null : MobileNo.Text.Trim();

                string ReferenceName1Value = ReferenceName1.Text.Trim();
                string ReferenceName2Value = ReferenceName2.Text.Trim();
                string ReferenceName3Value = ReferenceName3.Text.Trim();
                string ReferenceName4Value = ReferenceName4.Text.Trim();
                long MobileNo1Value = ParseLongNumber(ReferenceMobileNo1);
                long MobileNo2Value = ParseLongNumber(ReferenceMobileNo2);
                long MobileNo3Value = ParseLongNumber(ReferenceMobileNo3);
                long MobileNo4Value = ParseLongNumber(ReferenceMobileNo4);

                string guestCodeValue = txtGuestCode.Text.Trim();
                string loggedinUser = Session["LoginId"]?.ToString();


                #region Not Null Validaiton

                if (string.IsNullOrEmpty(txtDTNumber.Text))
                {
                    string message = "DT Number is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtDTNumber.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtBusinessCode.Text))
                {
                    string message = "Business Code is empty, enter valid DT";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtDTNumber.Focus();
                    return;  
                }

                if (string.IsNullOrEmpty(ddlSalutation1.Text))
                {
                    string message = "Client title is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    ddlSalutation1.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(accountName))
                {
                    string message = "Client Name is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtAccountName.Focus();
                    return; 
                }

                if (string.IsNullOrEmpty(clientPan))
                {
                    string message = "Client Pan is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    itfAOClientPan.Focus();
                    return; 
                }

                if (string.IsNullOrEmpty(cldDOB.Text))
                {
                    string message = "DOB is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    cldDOB.Focus();
                    return; 
                }

                if (string.IsNullOrEmpty(ddlAOGender.Text))
                {
                    string message = "Genter is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlAOGender.Focus();
                    return;  
                }

                if (string.IsNullOrEmpty(MobileNo.Text))
                {
                    string message = "Mobile Number is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    MobileNo.Focus();
                    return; 
                }

                if (string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue))
                {
                    string message = "Country is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlMailingCountryList.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(ddlMailingCityList.Text))
                {
                    string message = "City is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlMailingCityList.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtEmail.Text) || !txtEmail.Text.Contains("@"))
                {
                    string message = "Email is invalid";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    txtEmail.Focus();
                    return;
                }





                if (ValidateMobileFieldMinLength(MobileNo)){
                return;
                }
                if(ValidateMobileFieldMinLength(ReferenceMobileNo1)){
                return;
                }
                if(ValidateMobileFieldMinLength(ReferenceMobileNo2)){
                return;
                }
                if(ValidateMobileFieldMinLength(ReferenceMobileNo3)){
                return;
                }
                if (ValidateMobileFieldMinLength(ReferenceMobileNo4)) {
                    return;
                }

                #endregion

                #region Handle pin code by country
                if (ddlMailingCountryList.Items.Count > 0)
                {
                    HandlePinCodeValidation(ddlMailingCountryList.Items.ToString(), txtMailingPin);
                    try
                    {
                        mailingPinCode = txtMailingPin.Text;


                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (ddlPCountryList.Items.Count > 0)
                {
                    HandlePinCodeValidation(ddlPCountryList.Items.ToString(), txtPPin);
                    try
                    {
                        PermanentPinCode = txtPPin.Text ;
                    }
                    catch (Exception ex)
                    {

                    }
                }

                #endregion


                // Call the InsertClientData method from AccountOpeningController
                string insertResult = new WM.Controllers.AccountOpeningController().InsertClientData(

dtNumberValue,
existClientCodeValue,
businessCodeValue,

                    taxStatus,
                    occupationValue,
                    statusCategoryValue,
                    clientCategoryValue,
                    accountCategoryValue,
                    selectedSalutation1,
                    accountName,

                     selectedSalutation2, accountFatherName, accountOtherValue,
                    gender, maritalStatus, nationality, residentNri, dob, annualIncome, clientPan, leadType, guardianPerson,
                    guardianPersonNationality, guardianPersonPan, mailingAddress1, mailingAddress2, mailingState, mailingCity, mailingPinCode,
permanentAddress1,
permanentAddress2,
permanentState,
permanentCity,
PermanentPinCode,


NRIAddress,
FaxValue,
AadharValue,
EmailValue,
EmailOfficialValue,
PhoneOfficeSTDValue,
PhoneOfficeNumberValue,
PhoneResSTDValue,
PhoneResNumberValue,
MobileNoValue,
ReferenceName1Value,
ReferenceName2Value,
ReferenceName3Value,
ReferenceName4Value,
MobileNo1Value,
MobileNo2Value,
MobileNo3Value,
MobileNo4Value,
                    loggedinUser,
                    guestCodeValue

                );

                if (insertResult.Contains("no emp exist"))
                {

                    // Handle success or error message from insertResult
                    ClientScript.RegisterStartupScript(this.GetType(), "insertAlert", $"alert('RM Employee no valid');", true);
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";
                    //ResetFormFieldsOnInsertion();                    
                }
                if (insertResult.Contains("success"))
                {
                    ShowAlert(insertResult);

                    
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";
                    //ResetFormFieldsOnInsertion();
                    //
                    try
                    {
                        string NewAHCode = ExtractAHCodeAndReturn(insertResult);
                        FillClientDataByAHNum(NewAHCode);
                    } catch(Exception ex)
                    {

                    }
                }

                else if (insertResult.Contains("Duplidate Data: "))
                {
                    ShowAlert(insertResult);
                    if (insertResult.Contains("pan"))
                    {
                        itfAOClientPan.Focus();
                    }

                    if (insertResult.Contains("mobile"))
                    {
                        MobileNo.Focus();
                    }
                }
                else
                {
                    ShowAlert(insertResult);
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";

                }

            }
            catch (Exception ex)
            {
                // Handle any exception that occurs during the InsertClientData call
                string errorMessage = $"Kindly fill form properly. ";
                lblMessage.CssClass = "message-label-error";
                lblMessage.Text = errorMessage;

                // Display alert for the exception
                ClientScript.RegisterStartupScript(this.GetType(), "insertExceptionAlert", $"alert('Kindly fill form properly.');", true);
            }
        }

        public static string ExtractAHCodeAndReturn(string input)
        {
            // Use a regular expression to match "AH" followed by digits
            // Use fully qualified names to avoid ambiguity
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"AH\d+");
            System.Text.RegularExpressions.Match match = regex.Match(input);


            // Return the matched value if found, otherwise return an empty string
            return match.Success ? match.Value : string.Empty;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                #region Input Client Data for update
                string searchClientCodeValue = txtSearchClientCode.Text.Trim();
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string taxStatus = ddlTaxStatus.SelectedValue;
                int occupationValue = ParseInt(ddlOccupation.SelectedValue);
                string statusCategoryValue = ddlAOCategoryStatus.SelectedValue;
                string clientCategoryValue = ddlAOClientCategory.SelectedValue;
                int accountCategoryValue = ParseInt(ddlAOACCategory.SelectedValue);
                string selectedSalutation1 = ddlSalutation1.SelectedValue;
                string accountName = txtAccountName.Text.Trim();
                string selectedSalutation2 = ddlSalutation2.SelectedValue;
                string accountFatherName = txtFatherSpouse.Text.Trim();
                string accountOtherValue = txtOther1.Text.Trim();
                string gender = ddlAOGender.SelectedValue;
                string maritalStatus = ddlAOMaritalStatus.SelectedValue;
                string nationality = ddlAONationality.SelectedValue;
                string residentNri = ddlAOResidentNRI.SelectedValue;
                //DateTime? dob = DateTime.TryParse(cldDOB.Text, out var parsedDate) ? (DateTime?)parsedDate : null;
                DateTime? dob = DateTime.TryParseExact(cldDOB.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ? (DateTime?)parsedDate : null;

                string annualIncome = ddlAOAnnualIncome.SelectedValue;
                string clientPan = itfAOClientPan.Text.Trim();
                string leadType = ddlLeadSource.SelectedValue;
                string guardianPerson = itfAOGuardianPerson.Text.Trim();
                string guardianPersonNationality = itfAOGuardianNationality.Text.Trim();
                string guardianPersonPan = itfAOGuardianPANNO.Text.Trim();
                string mailingAddress1 = itfAOMAddress1.Text.Trim();
                string mailingAddress2 = itfAOMAddress2.Text.Trim();
                string mailingState = ddlMailingStateList.SelectedValue;
                string mailingCity = ddlMailingCityList.SelectedValue;
                string mailingPinCode = txtMailingPin.Text.Trim();


                // Permanent Address
                string permanentAddress1 = txtPAddress1.Text.Trim();
                string permanentAddress2 = txtPAddress2.Text.Trim();
                string permanentState = ddlPStateList.SelectedValue;
                string permanentCity = ddlPCityList.SelectedValue;
                string PermanentPinCode = txtPPin.Text;

                // If permanent address is the same as mailing address
                bool PermanentAddressSameAsMailing = CheckBox1.Checked;
                if (PermanentAddressSameAsMailing)
                {
                    mailingAddress1 = permanentAddress1;
                    mailingAddress2 = permanentAddress2;
                    mailingState = permanentState;
                    mailingCity = permanentCity;
                    mailingPinCode = PermanentPinCode;
                }

                string NRIAddress = txtOverseasAdd.Text.Trim();
                string FaxValue = txtFax.Text.Trim();
                long AadharValue = ParseLongNumber(txtAadhar);
                string EmailValue = txtEmail.Text.Trim();
                string EmailOfficialValue = txtOfficialEmail.Text.Trim();
                string PhoneOfficeSTDValue = string.IsNullOrEmpty(PhoneOfficeSTD.Text.Trim()) ? null : PhoneOfficeSTD.Text.Trim();
                string PhoneOfficeNumberValue = string.IsNullOrEmpty(PhoneOfficeNumber.Text.Trim()) ? null : PhoneOfficeNumber.Text.Trim();
                string PhoneResSTDValue = string.IsNullOrEmpty(PhoneResSTD.Text.Trim()) ? null : PhoneResSTD.Text.Trim();
                string PhoneResNumberValue = string.IsNullOrEmpty(PhoneResNumber.Text.Trim()) ? null : PhoneResNumber.Text.Trim();
                string MobileNoValue = string.IsNullOrEmpty(MobileNo.Text.Trim()) ? null : MobileNo.Text.Trim();

                string ReferenceName1Value = ReferenceName1.Text.Trim();
                string ReferenceName2Value = ReferenceName2.Text.Trim();
                string ReferenceName3Value = ReferenceName3.Text.Trim();
                string ReferenceName4Value = ReferenceName4.Text.Trim();
                long MobileNo1Value = ParseLongNumber(ReferenceMobileNo1);
                long MobileNo2Value = ParseLongNumber(ReferenceMobileNo2);
                long MobileNo3Value = ParseLongNumber(ReferenceMobileNo3);
                long MobileNo4Value = ParseLongNumber(ReferenceMobileNo4);
                string loggedinUser = Session["LoginId"].ToString();

                #endregion


                if (string.IsNullOrEmpty(txtClientCode.Text))
                {    
                    string message = "First load client data";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    return; // Exit the method early
                }
                if (string.IsNullOrEmpty(txtBusinessCode.Text))
                {
                    string message = "Business Code is empty, enter valid DT";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtDTNumber.Focus();
                    return;
                } 
                if (string.IsNullOrEmpty(accountName))
                {
                    string message = "Client Name is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtAccountName.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(clientPan))
                {
                    string message = "Client Pan is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    itfAOClientPan.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(cldDOB.Text))
                {
                    string message = "DOB is empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    cldDOB.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOGender.Text))
                {
                    string message = "Genter is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlAOGender.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(MobileNo.Text))
                {
                    string message = "Mobile Number is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    MobileNo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue))
                {
                    string message = "Country is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlMailingCountryList.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlMailingCityList.Text))
                {
                    string message = "City is Empty";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlMailingCityList.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    string message = "Email is invalid";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    txtEmail.Focus();
                    return;
                }

                if (ValidateMobileFieldMinLength(MobileNo))
                {
                    return;
                }
                if (ValidateMobileFieldMinLength(ReferenceMobileNo1))
                {
                    return;
                }
                if (ValidateMobileFieldMinLength(ReferenceMobileNo2))
                {
                    return;
                }
                if (ValidateMobileFieldMinLength(ReferenceMobileNo3))
                {
                    return;
                }
                if (ValidateMobileFieldMinLength(ReferenceMobileNo4))
                {
                    return;
                }

                string isUpdated = new WM.Controllers.AccountOpeningController().UpdateClientData(
                    searchClientCodeValue,

dtNumberValue,
existClientCodeValue,
businessCodeValue,



                    taxStatus, occupationValue, statusCategoryValue, clientCategoryValue,
                    accountCategoryValue, selectedSalutation1, accountName, selectedSalutation2, accountFatherName, accountOtherValue,
                    gender, maritalStatus, nationality, residentNri, dob, annualIncome, clientPan, leadType, guardianPerson,
                    guardianPersonNationality, guardianPersonPan, mailingAddress1, mailingAddress2, mailingState, mailingCity, mailingPinCode,
permanentAddress1,
permanentAddress2,
permanentState,
permanentCity,
PermanentPinCode,


NRIAddress,
FaxValue,
AadharValue,
EmailValue,
EmailOfficialValue,
PhoneOfficeSTDValue,
PhoneOfficeNumberValue,
PhoneResSTDValue,
PhoneResNumberValue,
MobileNoValue,
ReferenceName1Value,
ReferenceName2Value,
ReferenceName3Value,
ReferenceName4Value,
MobileNo1Value,
MobileNo2Value,
MobileNo3Value,
MobileNo4Value,
loggedinUser


                );

                if (isUpdated.ToUpper().Contains("SUCCESSFULLY"))
                {
                    ShowAlert(isUpdated);
                    lblMessage.Text = isUpdated;

                }
                
                else if (isUpdated.ToUpper().Contains("ACCESS")){
                    ShowAlert(isUpdated);
                    return;
                }
                
                else if (isUpdated.ToUpper().Contains("DUPLICATE"))
                {
                    ShowAlert(isUpdated);
                    if (isUpdated.ToUpper().Contains("PAN"))
                    {
                        itfAOClientPan.Focus();
                    }

                    if (isUpdated.ToUpper().Contains("MOBILE"))
                    {
                        MobileNo.Focus();
                    }
                    
                }
                else
                {
                    ShowAlert(isUpdated);
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Unexpected Error: {ex.Message}");
                lblMessage.Text = $"Error: {ex.Message}";
            }
        }


        // Helper methods for parsing
        private int ParseInt(string input) => int.TryParse(input, out int result) ? result : 0;

        private DateTime? ParseDate(string input) => DateTime.TryParse(input, out DateTime result) ? result : (DateTime?)null;



        #region Client List 


        #region ClientListFillCityList
        private void ClientListFillCityList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetCityList();

            ClientListddlMailingCityListV.DataSource = dt;
            ClientListddlMailingCityListV.DataTextField = "CITY_NAME";
            ClientListddlMailingCityListV.DataValueField = "CITY_ID";
            ClientListddlMailingCityListV.DataBind();
            ClientListddlMailingCityListV.Items.Insert(0, new ListItem("Select", ""));

        }
        #endregion

        #region ClientListFillBranchList
        private void ClientListFillBranchList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetBranchList();

            ClientListbranch.DataSource = dt;
            ClientListbranch.DataTextField = "BRANCH_NAME";
            ClientListbranch.DataValueField = "BRANCH_CODE";
            ClientListbranch.DataBind();
            ClientListbranch.Items.Insert(0, new ListItem("Select", ""));


            DataTable dt2 = new WM.Controllers.AccountOpeningController().GetBranchList();

            InvestorBranchDropDown.DataSource = dt2;
            InvestorBranchDropDown.DataTextField = "BRANCH_NAME";
            InvestorBranchDropDown.DataValueField = "BRANCH_CODE";
            InvestorBranchDropDown.DataBind();
            InvestorBranchDropDown.Items.Insert(0, new ListItem("Select", ""));

        }
        #endregion

        #region FillMutualFundDropdown
        private void FillMutualFundDropdown()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetMutualPlanType();
            // Clear any existing items in the dropdown
            ddlMutualPlanType.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    // Concatenate the column values with '#' for the dropdown value
                    string value = $"{row["mut_code"]}${row["sch_code"]}${row["SCH_NAME"]}${row["figure"]}";

                    // Use 'SCH_NAME' as the display text
                    string text = row["SCH_NAME"].ToString();

                    // Add the item to the dropdown
                    ddlMutualPlanType.Items.Add(new ListItem(text, value));
                }
            }

            // Insert the default "Select Mutual Fund" option at the top
            ddlMutualPlanType.Items.Insert(0, new ListItem("Select Mutual Fund", ""));
        }
        #endregion




        #region ClientListSearch_Click

        public string ConvertToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }



        protected void ClientListSearch_Click(object sender, EventArgs e)
        {
            // Retrieve input values directly, converting types as necessary
            string cityId = ClientListddlMailingCityListV.SelectedValue != "Select" ? ClientListddlMailingCityListV.SelectedValue : null;
            string clientCode = ClientListclientCode.Text.Trim();
            string name = ConvertToSentenceCase(ClientListclientName.Text.Trim());
            string mobile = string.IsNullOrWhiteSpace(ClientListmobile.Text) ? null : ClientListmobile.Text.Trim();
            string phone = string.IsNullOrWhiteSpace(ClientListphone.Text) ? null : ClientListphone.Text.Trim();
            string branchId = ClientListbranch.SelectedValue != "Select" ? ClientListbranch.SelectedValue : null;
            string businessCode = string.IsNullOrWhiteSpace(ClientListbusinessCd.Text) ? null : ClientListbusinessCd.Text.Trim();



            // Check if all fields are null or empty
            if (string.IsNullOrWhiteSpace(cityId) &&
                string.IsNullOrWhiteSpace(clientCode) &&
                string.IsNullOrWhiteSpace(name) &&
                string.IsNullOrWhiteSpace(mobile) &&
                string.IsNullOrWhiteSpace(phone) &&
                string.IsNullOrWhiteSpace(branchId) &&
                string.IsNullOrWhiteSpace(businessCode))

            {
                // Reset the search result and show an error message if no fields are provided
                ClientListResetGridView();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please provide at least one field for the search.');", true);

                ClientListlblMessage.Text = "Please provide at least one field for the search!";
                return;
            }

            // Call the controller to get the client list based on the provided fields
            DataTable dt = new AccountOpeningController().PSMGetClientList(cityId, clientCode, name, mobile, phone, branchId, businessCode);

            // Check if any records were returned
            if (dt == null || dt.Rows.Count < 1)
            {
                // Reset the grid and show "No records found" message
                ClientListResetGridView();
                ClientListlblMessage.Text = "No records found!";
            }
            else
            {
                // Display the total record count and bind the results to the grid
                ClientListlblMessage.Text = $"Total {dt.Rows.Count} {(dt.Rows.Count == 1 ? "record" : "records")} found!";
                ClientListclientGridView.Visible = true;
                ClientListclientGridView.DataSource = dt;
                ClientListclientGridView.DataBind();
            }
        }

        #endregion


        #region gvClientSearch_RowCommand
        protected void gvClientSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                string currentSelectedAh = e.CommandArgument.ToString();  // Use camel case for variable naming
                ResetFormFields1();
                ResetFamilyGrid();
                if (!string.IsNullOrEmpty(currentSelectedAh))
                {
                    FillClientDataByAHNum(currentSelectedAh);
                    if (!string.IsNullOrEmpty(txtSearchClientCode.Text))
                    {
                        ClientListResetMain();
                        ClientListlblMessage.Text = string.Empty;
                        ScriptManager.RegisterStartupScript(this, GetType(), "closeClientListModel", "closeClientListModal();", true);
                        //btnSave.Enabled = false;
                        //btnUpdate.Enabled = true;
                    }
                }
            }
        }
        #endregion


        protected void ClientListReset_Click(object sender, EventArgs e)
        {
            ClientListResetMain();
        }


        protected void ClientListResetMain()
        {
            // Reset all TextBox controls
            ClientListmobile.Text = string.Empty;
            ClientListphone.Text = string.Empty;
            ClientListclientCode.Text = string.Empty;
            ClientListbusinessCd.Text = string.Empty;
            ClientListclientName.Text = string.Empty;
            ClientListlblMessage.Text = string.Empty;

            if (ClientListbranch.Items.Count > 0)
                ClientListbranch.SelectedIndex = -1;

            if (ClientListddlMailingCityListV.Items.Count > 0)
                ClientListddlMailingCityListV.SelectedIndex = -1;

            ClientListclientGridView.DataSource = null;
            ClientListclientGridView.DataBind();
        }

        protected void ClientListResetGridView()
        {
            ClientListclientGridView.DataSource = null;
            ClientListclientGridView.DataBind();
        }


        #endregion

        protected void ApproveButton_Click(object sender, EventArgs e)
        {
            string CurrentAHToApprove = txtSearchClientCode.Text.Trim();
            string loggedInUser = Session["LoginId"]?.ToString();

            // Validate client code
            if (string.IsNullOrEmpty(CurrentAHToApprove))
            {
                ShowAlert("Please Enter AH Client Code to Approve.");
                return;
            }

            // Validate logged-in user
            if (string.IsNullOrEmpty(loggedInUser))
            {
                ShowAlert("User not logged in.");
                return;
            }

            // Approve client and handle the result
            string approvalMessage = new WM.Controllers.AccountOpeningController().ApproveClient(CurrentAHToApprove, loggedInUser);
            ShowAlert(approvalMessage);
            lblMessage.Text = approvalMessage;

            // Update client data if approved
            if (approvalMessage.ToUpper().Contains("CLIENT APPROVED"))
            {
                FillClientDataByAHNum(CurrentAHToApprove);
            }

        }

        protected void ResetFormFields1OnCreation(object sender, EventArgs e)
        {
            ResetFormFields1();

            //string clientCode = txtSearchClientCode.Text.Trim(); // Trim whitespace from the input
            //FillClientDataByAHNum(clientCode);

        }

    }
}