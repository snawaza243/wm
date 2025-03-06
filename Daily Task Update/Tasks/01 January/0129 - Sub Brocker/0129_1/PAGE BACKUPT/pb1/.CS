using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WM.Controllers;
using WM.Models;

namespace WM.Masters
{
    public partial class associate_master : System.Web.UI.Page
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

                    FillCityMainList();

                    FillStateForCity();

                    FillBranchList();
                    FillCityList();
                    FillSuperANAAgentList();
                    SuperAnafillSourceList();
                    //fillBankCityList();
                    FillBankMasterDetails();
                    FillPaymentModeID();
                    FillBankAccountTypeID();
                    FillOtherTypeID();

                    FillExamList();

                    FillAssociateTypeList();
                    FillAssociateTypeListCat();




                    fillAgentViewBranchList();
                    fillAgentViewCityList();
                    DisableAllFields();

                }

            }

            if (Request.QueryString["stid"] != null)
            {
                string payrollId = Request.QueryString["stid"];
                associateCode.Text = payrollId;
                FillAssociateDataByAgentCode(payrollId);
            }


        }


        public void SetFieldData(DataTable dt)
        {
            FillCityMainList();
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                string selectedPaymentMode = Convert.ToString(row["PAYMENTMODEID"]);
                string selectedAccType = Convert.ToString(row["ACCTYPEID"]);
                type.SelectedValue = string.IsNullOrEmpty(row["AGENT_TYPE"]?.ToString()) ? type.Items[0].Value : row["AGENT_TYPE"].ToString();

                string selectedBank = Convert.ToString(row["BANKID"]);
                string cityName = Convert.ToString(row["CITY_NAME"]);

                txtDTNumber.Text = row["DT_NUMBER"] != DBNull.Value ? row["DT_NUMBER"].ToString() : string.Empty;


                // Set dropdown values
                SetDropdownValueIfExist(paymentMode, selectedPaymentMode);
                SetDropdownValueIfExist(accountType, selectedAccType);
                SetDropdownValueIfExist(bankName, selectedBank);
                SetDropdownValueIfExist(ddlBankCityAM, cityName);

                // Set text values with null checks
                accountNo.Text = row["ACCNO"] != DBNull.Value ? row["ACCNO"].ToString() : string.Empty;
                neftBankName.Text = row["NEFT_BANK_NAME"] != DBNull.Value ? row["NEFT_BANK_NAME"].ToString() : string.Empty;
                neftBranch.Text = row["BANK_BRANCH_NAME"] != DBNull.Value ? row["BANK_BRANCH_NAME"].ToString() : string.Empty;
                neftIFCSCode.Text = row["IFSC_CODE"] != DBNull.Value ? row["IFSC_CODE"].ToString() : string.Empty;
                neftAccountNo.Text = row["ACCNO"] != DBNull.Value ? row["ACCNO"].ToString() : string.Empty;
                neftName.Text = row["NAME_IN_BANK"] != DBNull.Value ? row["NAME_IN_BANK"].ToString() : string.Empty;

                // Set date fields
                SetDateValue(row, "AFFECTEDFROM", "dd/MM/yyyy", affectedFrom);
                SetDateValue(row, "TIMEST", "dd/MM/yyyy", empanelmentDate);
                SetDateValue(row, "ANA_AUDITDATE", "dd/MM/yyyy", auditDate);
                SetDateValue(row, "DOB", "dd/MM/yyyy", dob);
                SetDateValue(row, "POSP_CERTIFIED_ON_GI", "dd/MM/yyyy", pospCertifiedGiOn);
                SetDateValue(row, "POSP_VALID_TILL_GI", "dd/MM/yyyy", pospCertifiedGiOnValidTill);
                SetDateValue(row, "POSP_CERTIFIED_ON_LI", "dd/MM/yyyy", pospCertifiedLiOn);
                SetDateValue(row, "POSP_VALID_TILL_LI", "dd/MM/yyyy", pospCertifiedLiOnValidTill);

                // Set checkbox values
                SetCheckboxValueYN(row, "AMFICERT", certPassedCheck);
                SetCheckboxValueYN(row, "ANA_AUDIT", audit);
                SetCheckboxValueYN(row, "ONLINE_SUBSCIPTION", onlineSubscription);
                SetCheckboxValueYN(row, "BLOCK_AGENT", chkbOfflinePlaformBlock);
                SetCheckboxValueYN(row, "ONLINE_BLOCK_AGENT", chkbOnlinePlaformBlock);
                SetCheckboxValueYN(row, "VERIFIED_STATUS", verified);

                // Handle other fields
                contactPersionemailId.Text = row["CPEMAILID"] != DBNull.Value ? row["CPEMAILID"].ToString() : string.Empty;
                ddlAssociateType.SelectedValue = row["SUB_BROKER_TYPE"] != DBNull.Value ? row["SUB_BROKER_TYPE"].ToString() : string.Empty;
                empanelmentType.SelectedValue = row["PAIDFLAG"] != DBNull.Value ? row["PAIDFLAG"].ToString() : string.Empty;
                associateName.Text = row["AGENT_NAME"] != DBNull.Value ? row["AGENT_NAME"].ToString() : string.Empty;
                emailId.Text = row["EMAIL"] != DBNull.Value ? row["EMAIL"].ToString() : string.Empty;
                associateCode.Text = row["AGENT_CODE"] != DBNull.Value ? row["AGENT_CODE"].ToString() : string.Empty;
                subBrokerExistCode.Text = row["EXIST_CODE"].ToString();
                ddlBranchAM.SelectedValue = row["SOURCEID"].ToString();

                string gettingSourceIDBranch = row["SOURCEID"] != DBNull.Value ? row["SOURCEID"].ToString() : null;
                FillRMList(gettingSourceIDBranch);
                FillLocationListByCityBranch(gettingSourceIDBranch);

                string gettingRMCode = row["RM_CODE"] != DBNull.Value ? row["RM_CODE"].ToString() : null;

                // Fetch RM details
                DataTable rmByRmList = new AssociateController().GetRMByRMCode(gettingRMCode);
                if (rmByRmList.Rows.Count > 0)
                {
                    DataRow RM_row = rmByRmList.Rows[0];
                    try
                    {
                        string newRMCode = gettingRMCode;
                        string newRMName = RM_row["RM_NAME"].ToString();
                        selectEmployee.Items.Insert(1, new ListItem(newRMName, newRMCode));
                        selectEmployee.SelectedIndex = 1;
                    }
                    catch
                    {
                        selectEmployee.SelectedIndex = -1;
                    }
                }

                string firstAddress = row["ADDRESS1"] != DBNull.Value ? row["ADDRESS1"].ToString() : string.Empty;
                string secondAddress = row["ADDRESS2"] != DBNull.Value ? row["ADDRESS2"].ToString() : string.Empty;
                address1.Text = $"{firstAddress}\n{secondAddress}";
                pinCode.Text = row["PINCODE"].ToString();



                resAddAddress1.Text = row["RES_ADD_1"] != DBNull.Value ? row["RES_ADD_1"].ToString() : string.Empty;
                resAddAddress2.Text = row["RES_ADD_2"] != DBNull.Value ? row["RES_ADD_2"].ToString() : string.Empty;
                resAddState.Text = row["RES_ADD_STATE"] != DBNull.Value ? row["RES_ADD_STATE"].ToString() : string.Empty;
                resAddCity.Text = row["RES_ADD_CITY"] != DBNull.Value ? row["RES_ADD_CITY"].ToString() : string.Empty;
                resAddPIN.Text = row["RES_ADD_PIN"] != DBNull.Value ? row["RES_ADD_PIN"].ToString() : string.Empty;


                FillCityMainList();

                FillStateNameBySelectedCity(row["CITY_ID"].ToString());
                ddlCityAM.SelectedValue = row["CITY_ID"] != DBNull.Value ? row["CITY_ID"].ToString() : string.Empty;


                //ddlLocationAM.SelectedValue = row["LOCATION_ID"].ToString();
                if (row["LOCATION_ID"] != DBNull.Value && ddlLocationAM.Items.FindByValue(row["LOCATION_ID"].ToString()) != null)
                {
                    ddlLocationAM.SelectedValue = row["LOCATION_ID"].ToString();
                }
                else
                {
                    ddlLocationAM.SelectedIndex = 0; // Select the first item
                }

                mobile.Text = row["MOBILE"] != DBNull.Value ? row["MOBILE"].ToString() : string.Empty;
                fax.Text = row["FAX"] != DBNull.Value ? row["FAX"].ToString() : string.Empty;
                contactPerson.Text = row["CONTACTPER"] != DBNull.Value ? row["CONTACTPER"].ToString() : string.Empty;
                tds.Text = row["TDS"] != DBNull.Value ? row["TDS"].ToString() : string.Empty;
                phone.Text = row["PHONE"] != DBNull.Value ? row["PHONE"].ToString() : string.Empty;
                remarks.Text = row["REMARK"] != DBNull.Value ? row["REMARK"].ToString() : string.Empty;


                //ddlAssociateTypeCategory.SelectedValue = row["CATEGORY_ID"].ToString();
                if (row["CATEGORY_ID"] != DBNull.Value && ddlAssociateTypeCategory.Items.FindByValue(row["CATEGORY_ID"].ToString()) != null)
                {
                    ddlAssociateTypeCategory.SelectedValue = row["CATEGORY_ID"].ToString();
                }
                else
                {
                    ddlAssociateTypeCategory.SelectedIndex = 0; // Select the first item if null or not found
                }

                source.Text = row["SOURCEID"] != DBNull.Value ? row["SOURCEID"].ToString() : string.Empty;

                SelectDropdownValueIfExist(row, "MASTER_ANA", superAna);
                onlinePlatformRemark.Text = row["ONLINE_BLOCK_REMARK"] != DBNull.Value ? row["ONLINE_BLOCK_REMARK"].ToString() : string.Empty;
                offlinePlatformRemark.Text = row["OFFLINE_BLOCK_REMARK"] != DBNull.Value ? row["OFFLINE_BLOCK_REMARK"].ToString() : string.Empty;

                // Branch name
                string branchNameGetting = row["BANK_BRANCH_NAME"].ToString();
                branchName.Items.Insert(1, new ListItem(branchNameGetting, branchNameGetting));
                branchName.SelectedValue = row["BANK_BRANCHID"].ToString();

                bool isChecked = row["SMS_FLAG"].ToString().Trim().ToUpper() == "1";
                sms.Checked = isChecked;
                gstin.Text = row["GSTIN_NO"].ToString();

                type.SelectedValue = string.IsNullOrEmpty(row["AGENT_TYPE"]?.ToString()) ? type.Items[0].Value : row["AGENT_TYPE"].ToString();

                panGir.Text = row["PAN"].ToString();
                circleWardDist.Text = row["DIST"].ToString();

                // SetUITextField(row, "AADHAR_CARD_NO", aadharCard);
                aadharCard.Text = row["AADHAR_CARD_NO"] != DBNull.Value ? row["AADHAR_CARD_NO"].ToString() : string.Empty;
                // Handle POSP_TYPE
                if (row["POSP_TYPE"] != DBNull.Value && pospType.Items.FindByValue(row["POSP_TYPE"].ToString()) != null)
                {
                    pospType.SelectedValue = row["POSP_TYPE"].ToString();
                }
                else
                {
                    pospType.SelectedIndex = 0; // Default to the first item if null or not found
                }

                // Handle POSP_NO_LI
                pospNoLi.Text = row["POSP_NO_LI"] != DBNull.Value ? row["POSP_NO_LI"].ToString() : string.Empty;

                // Handle POSP_NO_GI
                pospNoGi.Text = row["POSP_NO_GI"] != DBNull.Value ? row["POSP_NO_GI"].ToString() : string.Empty;

                SetDropDownValueYN(row, "POSP_MARKING", pospMarking);

                // Handle DOC_ID
                if (row["DOC_ID"] != DBNull.Value && document.Items.FindByValue(row["DOC_ID"].ToString()) != null)
                {
                    document.SelectedValue = row["DOC_ID"].ToString();
                }
                else
                {
                    document.SelectedIndex = 0; // Default to the first item if null or not found
                }

            }
            else
            {
                // Optionally clear the form fields or display a message
            }
        }

        // Helper method to set dropdown values if they exist
        private void SetDropdownValueIfExist(DropDownList ddl, string value)
        {
            if (ddl.Items.FindByValue(value) != null)
                ddl.SelectedValue = value;
            else
                ddl.SelectedIndex = -1;
        }





        private void FillAssociateDataByAgentCode(string id)
        {
            try
            {
                Agent associate = new Agent
                {
                    AGENT_CODE = id
                };

                DataTable dt = new AssociateController().GetAssociateDataByAssociateCode(associate);

                SetFieldData(dt);
            }
            catch (Exception ex)
            {
                // Display an alert with the error message
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('An error occurred: {ex.Message}');", true);
                lblMessage.Text = ex.Message;
            }
        }
        private void SetUITextField(DataRow row, string fieldName, TextBox uiControl)
        {
            try
            {
                // Check if the specified field exists in the row and its value is not null or DBNull
                if (row[fieldName] != null && row[fieldName] != DBNull.Value)
                {
                    // Set the UI control's text to the field value
                    uiControl.Text = row[fieldName].ToString();
                }
                else
                {
                    // Set a message if the value is null or not available
                    uiControl.Text = "Field value not available";
                }
            }
            catch (Exception ex)
            {
                // Handle any errors and set a fallback message in the UI
                uiControl.Text = "Error retrieving field value";

                // Optionally log the exception if needed
                // LogError(ex); // Uncomment if you want to log the error
            }
        }
        private void SetDropDownValueYN(DataRow row, string columnName, DropDownList uiField)
        {
            string value = row[columnName]?.ToString(); // Get the value from the DataRow based on the column name

            if (value == "Y" || value == "N")
            {
                uiField.SelectedValue = value; // Set the dropdown's selected value if it's "Y" or "N"
            }
            else
            {
                uiField.SelectedIndex = -1; // Deselect if the value is invalid
            }
        }
        protected void SelectDropdownValueIfExist(DataRow row, string fieldName, DropDownList ddl, Label lblMessage = null)
        {
            // Retrieve the value from the DataRow using the field name
            string fieldValue = row[fieldName]?.ToString();

            // Check if the value exists in the DropDownList
            if (!string.IsNullOrEmpty(fieldValue) && ddl.Items.FindByValue(fieldValue) != null)
            {
                // If the value exists, select it
                ddl.SelectedValue = fieldValue;
            }
            else
            {
                // If the value doesn't exist, select a fallback value (e.g., first item)
                if (ddl.Items.Count > 0)
                {
                    ddl.SelectedIndex = 0; // Select the first item as default
                }

                // Optionally show a message if provided
                if (lblMessage != null)
                {
                    lblMessage.Text = $"Value '{fieldValue}' not found in the dropdown. Default value selected.";
                }
            }
        }
        private void SetDropdownValue(DataRow row, string columnName, DropDownList dropdown)
        {
            try
            {
                if (row[columnName] != DBNull.Value)
                {
                    dropdown.SelectedValue = row[columnName].ToString();
                }
            }
            catch
            {
                // Optionally log the error or handle the exception
            }
        }
        private void SetDateValue(DataRow row, string columnName, string dateFormat, TextBox uiField)
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
        private void SetCheckboxValueYN(DataRow row, string columnName, CheckBox checkbox)
        {
            string columnValue = row[columnName]?.ToString().Trim().ToUpper();

            if (columnValue == "Y" || columnValue == "1")
            {
                checkbox.Checked = true; // Check the checkbox
            }

            else if (columnValue == "N" || columnValue == "0")
            {
                checkbox.Checked = false; // Uncheck the checkbox
            }
            else
            {
                checkbox.Checked = false; // Default case if the value is neither "Y" nor "N"
            }
        }

        private void FillClientDataByDTNumber(string id)
        {
            try
            {
                // Call the controller to get client data
                DataTable dt = new AssociateController().GetClientDataByDTNumber(id);

                if (dt == null || dt.Rows.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "dtNumberNotAlert", "alert('Provide valid DT number!');", true);
                }
                else
                {

                    string branchCode = dt.Rows[0]["busi_branch_code"]?.ToString();
                    string roCode = dt.Rows[0]["busi_rm_code"]?.ToString();
                    string currentDt = dt.Rows[0]["common_id"]?.ToString();




                    ddlBranchAM.SelectedValue = branchCode;

                    BindRmToDropdown(selectEmployee, branchCode);
                    selectEmployee.SelectedValue = roCode;
                    txtDTNumber.Text = currentDt;

                    FillLocationListByCityBranch(branchCode);



                    //Console.WriteLine($"Branch Code: {branchCode}, RO Code: {roCode}");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log them or show an error message)
                Console.WriteLine($"Error occurred: {ex.Message}");
                // Consider logging to a file, database, or monitoring system for production use
            }
        }

        // Function to bind RM data to DropDownList
        public void BindRmToDropdown(DropDownList selectEmployee, string sourceId)
        {
            // Get the data from the database
            DataTable dt_rm = new WM.Controllers.AssociateController().GetRmByBranchDTList(sourceId);

            // Check if the DataTable is not null and contains rows
            if (dt_rm != null && dt_rm.Rows.Count > 0)
            {
                selectEmployee.DataSource = dt_rm;
                selectEmployee.DataTextField = "rm_name";  // TextField corresponds to RM name
                selectEmployee.DataValueField = "payroll_id"; // ValueField corresponds to RM code
                selectEmployee.DataBind();
            }
            else
            {
                // If no records are found, clear the items and add a 'No RM found' item
                selectEmployee.Items.Clear();
                selectEmployee.Items.Insert(0, new ListItem("No RM found", ""));
            }

            // Always insert a default "Select" item at the top of the dropdown
            selectEmployee.Items.Insert(0, new ListItem("Select", ""));
        }


        protected void oneClientSearchByDT_Click(object sender, EventArgs e)
        {
            string commonID = txtDTNumber.Text.Trim();
            if (string.IsNullOrWhiteSpace(commonID))
            {
                ResetFields();
                ClientScript.RegisterStartupScript(this.GetType(), "dtNumberNotAlert", "alert('Please provide DT Number!');", true);

                lblMessage.Text = "Please provide DT Number";
            }
            else
            {
                ResetFields();
                FillClientDataByDTNumber(commonID);
            }

        }



        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {

                // Parsing date fields using a helper function to handle null or invalid values
                DateTime? auditDateValue = ParseDate(auditDate.Text);
                DateTime? empanelmentDateValue = ParseDate(empanelmentDate.Text);
                DateTime? affectedFromValue = ParseDate(affectedFrom.Text);
                DateTime? dobValue = ParseDate(dob.Text);

                DateTime? pospCertifiedOnLiValue = ParseDate(pospCertifiedLiOn.Text);
                DateTime? pospValidTillLiValue = ParseDate(pospCertifiedLiOnValidTill.Text);
                DateTime? pospCertifiedOnGiValue = ParseDate(pospCertifiedGiOn.Text);
                DateTime? pospValidTillGiValue = ParseDate(pospCertifiedGiOnValidTill.Text);

                // Safely parse numeric fields with fallback values if parsing fails
                string empanelmentTypeValue = empanelmentType.SelectedValue;
                string associateCodeValue = associateCode.Text;
                string subBrokerExistCodeValue = subBrokerExistCode.Text;
                string associateNameValue = associateName.Text;
                string ddlBranchAMValue = ddlBranchAM.SelectedValue;
                string selectEmployeeValue = selectEmployee.SelectedValue;

                // Get the text from the multiline TextBox
                string[] addressMiltiline = address1.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                // Initialize variables for each line
                string addressLine1 = addressMiltiline.Length > 0 ? addressMiltiline[0] : string.Empty;
                string addressLine2 = addressMiltiline.Length > 1 ? addressMiltiline[1] : string.Empty;
                string addressLine3 = addressMiltiline.Length > 2 ? addressMiltiline[2] : string.Empty;

                string address1Value = addressLine1;
                string address2Value = addressLine2;

                string ddlCityAMValue = ddlCityAM.SelectedValue;
                string ddlLocationAMValue = ddlLocationAM.SelectedValue;
                long mobileNumberValue = long.TryParse(mobile.Text, out long mobileNumber) ? mobileNumber : 0;
                long pinCodeValue = long.TryParse(pinCode.Text, out long pin) ? pin : 0;

                string resAddAddress1Value = !string.IsNullOrEmpty(resAddAddress1.Text) ? resAddAddress1.Text : string.Empty;
                string resAddAddress2Value = !string.IsNullOrEmpty(resAddAddress2.Text) ? resAddAddress2.Text : string.Empty;
                string resAddStateValue = !string.IsNullOrEmpty(resAddState.Text) ? resAddState.Text : string.Empty;
                string resAddCityValue = !string.IsNullOrEmpty(resAddCity.Text) ? resAddCity.Text : string.Empty;
                string resAddPINValue = !string.IsNullOrEmpty(resAddPIN.Text) ? resAddPIN.Text : string.Empty;

                string faxValue = fax.Text;
                string contactPersonValue = contactPerson.Text;
                string emailIdValue = emailId.Text;
                decimal tdsValue = decimal.TryParse(tds.Text, out decimal tdsParsed) ? tdsParsed : 0;

                string associateTypeValue = ddlAssociateType.SelectedValue;
                string associateCategoryValue = ddlAssociateTypeCategory.SelectedValue;

                string contactPersionEmailValue = contactPersionemailId.Text;
                string phoneValue = phone.Text;
                string remarksValue = remarks.Text;
                string superAnaValue = superAna.SelectedValue;
                string onlinePlatformValue = chkbOnlinePlaformBlock.Checked ? "1" : "0";
                string offlinePlatformValue = chkbOfflinePlaformBlock.Checked ? "1" : "0";

                string onlinePlatformRemarkValue = onlinePlatformRemark.Text;
                string offlinePlatformRemarkValue = offlinePlatformRemark.Text;

                string onlineSubscriptionValue = onlineSubscription.Checked ? "Y" : "N";
                string auditValue = audit.Checked ? "Y" : "N";

                int paymentModeIdValue = int.TryParse(paymentMode.SelectedValue, out int paymentModeId) ? paymentModeId : 0;
                string accountTypeValue = accountType.SelectedValue;
                string accountNoValue = accountNo.Text;

                string bankNameValue = bankName.SelectedValue;
                string ddlBankCityAMValue = ddlBankCityAM.SelectedValue;
                string branchNameValue = branchName.SelectedValue;
                string smsValue = sms.Checked ? "1" : "0";
                string gstinValue = gstin.Text;
                string typeValue = type.SelectedValue;
                string panGirValue = panGir.Text;
                string circleWardDistValue = circleWardDist.Text;
                string aadharCardValue = aadharCard.Text;
                string pospMarkingValue = pospMarking.SelectedValue;

                string pospTypeValue = pospType.Text;
                string pospNoLiValue = pospNoLi.Text;
                string pospNoGiValue = pospNoGi.Text;
                string verifiedValue = verified.Checked ? "Y" : "N";

                string neftBankNameValue = neftBankName.Text;
                string neftBranchValue = neftBranch.Text;
                string neftIFSCCodeValue = neftIFCSCode.Text;
                string neftNameValue = neftBankName.Text;

                string certPassedValue = certPassedCheck.Checked ? "Y" : "N";
                string certExamsValue = ddlCertExam.SelectedValue;
                string certRegValue = certRegNo.Text;
                string loggedInUser = Session["LoginId"]?.ToString();

                string dtNumberValue = txtDTNumber.Text.ToString();

                if (!ValidateField(txtDTNumber.Text, "Enter DT number", txtDTNumber)) return;
                if (!ValidateField(empanelmentType.SelectedValue, "Empanelment Type", empanelmentType)) return;
                if (!ValidateField(associateName.Text, "Enter agent name", associateName)) return;
                if (!ValidateField(ddlBranchAM.SelectedValue, "Select branch", ddlBranchAM)) return;
                if (selectEmployee.Items != null)
                {
                    if (!ValidateField(selectEmployee.SelectedValue, "Select RM", selectEmployee)) return;

                }

                if (!ValidateField(ddlCityAM.SelectedValue, "Select city", ddlCityAM)) return;
                if (!ValidateField(mobile.Text, "Enter mobile", mobile)) return;
                if (!ValidateField(emailId.Text, "Enter email", emailId)) return;
                if (!ValidateField(ddlAssociateTypeCategory.SelectedValue, "Select associate category", ddlAssociateTypeCategory)) return;
                if (!ValidateField(ddlAssociateType.SelectedValue, "Select associate type", ddlAssociateType)) return;

                if (!ValidateField(contactPerson.Text, "Enter contact person ", contactPerson)) return;
             //   if (!ValidateField(panGir.Text, "Enter Valid Pan", panGir)) return;



                // Check if the checkbox is checked and corresponding text is empty
                if (chkbOnlinePlaformBlock.Checked && string.IsNullOrWhiteSpace(onlinePlatformRemarkValue))
                {
                    // Show an alert if the remark text is empty
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a remark for the online platform.');", true);
                    onlinePlatformRemark.Focus(); // Set focus to the text box
                    return; // Exit the method if validation fails
                }

                if (chkbOfflinePlaformBlock.Checked && string.IsNullOrWhiteSpace(offlinePlatformRemarkValue))
                {
                    // Show an alert if the remark text is empty
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a remark for the offline platform.');", true);
                    offlinePlatformRemark.Focus(); // Set focus to the text box
                    return; // Exit the method if validation fails
                }

                else
                {


                    string insertResult = new WM.Controllers.AssociateController().InsertAgentMaster(
                            loggedInUser,
                            empanelmentTypeValue,
                            associateCodeValue,
                            subBrokerExistCodeValue,
                            associateNameValue,
                            ddlBranchAMValue,
                            selectEmployeeValue,
                            address1Value,
                            address2Value,
                            ddlCityAMValue,
                            ddlLocationAMValue,
                            mobileNumberValue,
                            pinCodeValue,
                            faxValue,
                            contactPersonValue,
                            emailIdValue,
                            tdsValue,
                            associateTypeValue,
                            associateCategoryValue,
                            contactPersionEmailValue,
                            empanelmentDateValue,
                            phoneValue,

                            remarksValue,

                            superAnaValue,
                            onlineSubscriptionValue,
                            onlinePlatformValue,
                            offlinePlatformValue,
                            onlinePlatformRemarkValue,
                            offlinePlatformRemarkValue,




                            auditDateValue,
                            auditValue,

                            paymentModeIdValue,
                            accountTypeValue,
                            accountNoValue,
                            affectedFromValue,
                            bankNameValue,
                            ddlBankCityAMValue,
                            branchNameValue,

                            smsValue,
                            gstinValue,
                            dobValue,
                            typeValue,
                            panGirValue,
                            circleWardDistValue,

                            aadharCardValue,
                            pospMarkingValue,
                            pospTypeValue,
                            pospNoLiValue,
                            pospNoGiValue,
                            pospCertifiedOnLiValue,
                            pospValidTillLiValue,
                            pospCertifiedOnGiValue,
                            pospValidTillGiValue,
                            verifiedValue,
                            neftBankNameValue,
                            neftBranchValue,
                            neftIFSCCodeValue,
                            neftNameValue,
                            certPassedValue,
                            certExamsValue,
                            certRegValue,

                            resAddAddress1Value,
                            resAddAddress2Value,
                            resAddStateValue,
                            resAddCityValue,
                            resAddPINValue,
                      dtNumberValue

                  );



                        ClientScript.RegisterStartupScript(this.GetType(), "insertAlert", $"alert('{insertResult}');", true);
                 /*
                    
                    if (insertResult.Contains("pan"))
                    {
                        panGir.Focus();
                        lblMessage.Text = insertResult;
                        lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";

                    }

                    if (insertResult.Contains("success"))
                    {


                        // Handle success or error message from insertResult
                        ClientScript.RegisterStartupScript(this.GetType(), "insertAlert", $"alert('{insertResult}');", true);
                        lblMessage.Text = insertResult;
                        lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";
                        //ResetFormFieldsOnInsertion();                    
                    }
                    if (insertResult.Contains("Defined Error"))
                    {
                        // Handle success or error message from insertResult
                        ClientScript.RegisterStartupScript(this.GetType(), "insertAlert", $"alert('{insertResult}');", true);
                        lblMessage.Text = insertResult;
                        lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";
                        //ResetFormFieldsOnInsertion();
                    }

                    */
              
                }


            }
            catch (Exception ex)
            {
                // Log and display any exception in an alert
                ClientScript.RegisterStartupScript(this.GetType(), "insertExceptionAlert", $"alert('Exception: {ex.Message}');", true);

                // Update the label with the exception message
                lblMessage.Text = $"Error: {ex.Message}";
                lblMessage.CssClass = "message-label-error"; // Error message style
            }
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Parsing date fields using a helper function to handle null or invalid values
                DateTime? auditDateValue = ParseDate(auditDate.Text);
                DateTime? empanelmentDateValue = ParseDate(empanelmentDate.Text);
                DateTime? affectedFromValue = ParseDate(affectedFrom.Text);
                DateTime? dobValue = ParseDate(dob.Text);

                DateTime? pospCertifiedOnLiValue = ParseDate(pospCertifiedLiOn.Text);
                DateTime? pospValidTillLiValue = ParseDate(pospCertifiedLiOnValidTill.Text);
                DateTime? pospCertifiedOnGiValue = ParseDate(pospCertifiedGiOn.Text);
                DateTime? pospValidTillGiValue = ParseDate(pospCertifiedGiOnValidTill.Text);

                // Safely parse numeric fields with fallback values if parsing fails
                string empanelmentTypeValue = empanelmentType.SelectedValue;
                string associateCodeValue = associateCode.Text;
                string subBrokerExistCodeValue = subBrokerExistCode.Text;
                string associateNameValue = associateName.Text;
                string ddlBranchAMValue = ddlBranchAM.SelectedValue;
                string selectEmployeeValue = selectEmployee.SelectedValue;

                // Get the text from the multiline TextBox
                string[] addressMiltiline = address1.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                // Initialize variables for each line
                string addressLine1 = addressMiltiline.Length > 0 ? addressMiltiline[0] : string.Empty;
                string addressLine2 = addressMiltiline.Length > 1 ? addressMiltiline[1] : string.Empty;
                string addressLine3 = addressMiltiline.Length > 2 ? addressMiltiline[2] : string.Empty;

                string address1Value = addressLine1;
                string address2Value = addressLine2;

                string ddlCityAMValue = ddlCityAM.SelectedValue;
                string ddlLocationAMValue = ddlLocationAM.SelectedValue;
                long mobileNumberValue = long.TryParse(mobile.Text, out long mobileNumber) ? mobileNumber : 0;
                long pinCodeValue = long.TryParse(pinCode.Text, out long pin) ? pin : 0;

                string resAddAddress1Value = !string.IsNullOrEmpty(resAddAddress1.Text) ? resAddAddress1.Text : string.Empty;
                string resAddAddress2Value = !string.IsNullOrEmpty(resAddAddress2.Text) ? resAddAddress2.Text : string.Empty;
                string resAddStateValue = !string.IsNullOrEmpty(resAddState.Text) ? resAddState.Text : string.Empty;
                string resAddCityValue = !string.IsNullOrEmpty(resAddCity.Text) ? resAddCity.Text : string.Empty;
                string resAddPINValue = !string.IsNullOrEmpty(resAddPIN.Text) ? resAddPIN.Text : string.Empty;

                string faxValue = fax.Text;
                string contactPersonValue = contactPerson.Text;
                string emailIdValue = emailId.Text;
                decimal tdsValue = decimal.TryParse(tds.Text, out decimal tdsParsed) ? tdsParsed : 0;

                string associateTypeValue = ddlAssociateType.SelectedValue;
                string associateCategoryValue = ddlAssociateTypeCategory.SelectedValue;

                string contactPersionEmailValue = contactPersionemailId.Text;
                string phoneValue = phone.Text;
                string remarksValue = remarks.Text;
                string superAnaValue = superAna.SelectedValue;
                string onlinePlatformValue = chkbOnlinePlaformBlock.Checked ? "1" : "0";
                string offlinePlatformValue = chkbOfflinePlaformBlock.Checked ? "1" : "0";

                string onlinePlatformRemarkValue = onlinePlatformRemark.Text;
                string offlinePlatformRemarkValue = offlinePlatformRemark.Text;

                string onlineSubscriptionValue = onlineSubscription.Checked ? "Y" : "N";
                string auditValue = audit.Checked ? "Y" : "N";

                int paymentModeIdValue = int.TryParse(paymentMode.SelectedValue, out int paymentModeId) ? paymentModeId : 0;
                string accountTypeValue = accountType.SelectedValue;
                string accountNoValue = accountNo.Text;

                string bankNameValue = bankName.SelectedValue;
                string ddlBankCityAMValue = ddlBankCityAM.SelectedValue;
                string branchNameValue = branchName.SelectedValue;
                string smsValue = sms.Checked ? "1" : "0";
                string gstinValue = gstin.Text;
                string typeValue = type.SelectedValue;
                string panGirValue = panGir.Text;
                string circleWardDistValue = circleWardDist.Text;
                string aadharCardValue = aadharCard.Text;
                string pospMarkingValue = pospMarking.SelectedValue;

                string pospTypeValue = pospType.Text;
                string pospNoLiValue = pospNoLi.Text;
                string pospNoGiValue = pospNoGi.Text;
                string verifiedValue = verified.Checked ? "Y" : "N";

                string neftBankNameValue = neftBankName.Text;
                string neftBranchValue = neftBranch.Text;
                string neftIFSCCodeValue = neftIFCSCode.Text;
                string neftNameValue = neftBankName.Text;

                string certPassedValue = certPassedCheck.Checked ? "Y" : "N";
                string certExamsValue = ddlCertExam.SelectedValue;
                string certRegValue = certRegNo.Text;
                string loggedInUser = Session["LoginId"]?.ToString();
                string dtNumberValue = txtDTNumber.Text.ToString();


                if (!ValidateField(ddlBranchAM.SelectedValue, "Select branch", ddlBranchAM)) return;
                //  if (!ValidateField(selectEmployee.SelectedValue, "Select RM", selectEmployee)) return;
                if (!ValidateField(ddlCityAM.SelectedValue, "Select city", ddlCityAM)) return;
                if (!ValidateField(mobile.Text, "Enter mobile", mobile)) return;
                if (!ValidateField(emailId.Text, "Enter email", emailId)) return;
                if (!ValidateField(ddlAssociateType.SelectedValue, "Select associate type", ddlAssociateType)) return;
                if (!ValidateField(contactPerson.Text, "Enter contact person email", contactPerson)) return;

                // Check if the checkbox is checked and corresponding text is empty
                if (chkbOnlinePlaformBlock.Checked && string.IsNullOrWhiteSpace(onlinePlatformRemarkValue))
                {
                    // Show an alert if the remark text is empty
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a remark for the online platform.');", true);
                    onlinePlatformRemark.Focus(); // Set focus to the text box
                    return; // Exit the method if validation fails
                }

                if (chkbOfflinePlaformBlock.Checked && string.IsNullOrWhiteSpace(offlinePlatformRemarkValue))
                {
                    // Show an alert if the remark text is empty
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a remark for the offline platform.');", true);
                    offlinePlatformRemark.Focus(); // Set focus to the text box
                    return; // Exit the method if validation fails
                }



                else
                {
                    string isUpdated = new WM.Controllers.AssociateController().UpdateAgentMaster(
                        loggedInUser,
                        empanelmentTypeValue,
                        associateCodeValue,
                        subBrokerExistCodeValue,
                        associateNameValue,
                        ddlBranchAMValue,
                        selectEmployeeValue,
                        address1Value,
                        address2Value,
                        ddlCityAMValue,
                        ddlLocationAMValue,
                        mobileNumberValue,
                        pinCodeValue,
                        faxValue,
                        contactPersonValue,
                        emailIdValue,
                        tdsValue,
                        associateTypeValue,
                        associateCategoryValue,
                        contactPersionEmailValue,
                        empanelmentDateValue,
                        phoneValue,

                        remarksValue,

                        superAnaValue,
                        onlineSubscriptionValue,
                        onlinePlatformValue,
                        offlinePlatformValue,
                        onlinePlatformRemarkValue,
                        offlinePlatformRemarkValue,




                        auditDateValue,
                        auditValue,

                        paymentModeIdValue,
                        accountTypeValue,
                        accountNoValue,
                        affectedFromValue,
                        bankNameValue,
                        ddlBankCityAMValue,
                        branchNameValue,

                        smsValue,
                        gstinValue,
                        dobValue,
                        typeValue,
                        panGirValue,
                        circleWardDistValue,

                        aadharCardValue,
                        pospMarkingValue,
                        pospTypeValue,
                        pospNoLiValue,
                        pospNoGiValue,
                        pospCertifiedOnLiValue,
                        pospValidTillLiValue,
                        pospCertifiedOnGiValue,
                        pospValidTillGiValue,
                        verifiedValue,
                        neftBankNameValue,
                        neftBranchValue,
                        neftIFSCCodeValue,
                        neftNameValue,
                        certPassedValue,
                        certExamsValue,
                        certRegValue,

                        resAddAddress1Value,
                        resAddAddress2Value,
                        resAddStateValue,
                        resAddCityValue,
                        resAddPINValue,
                        dtNumberValue
                    );

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "updateSuccess", "alert('" + isUpdated + "');", true);

                    lblMessage.Text = isUpdated;
                    lblMessage.CssClass = "text-success";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblMessage.CssClass = "text-danger";
            }
        }

        private bool ValidateField(string value, string alertMessage, Control controlToFocus)
        {
            if (string.IsNullOrEmpty(value))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "validationError", $"alert('{alertMessage}');", true);
                controlToFocus.Focus();
                return false;
            }
            return true;
        }



        protected void certEnrolledCheck_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or disable the dropdown based on the checkbox state
            ddlCertExam.Enabled = certEnrolledCheck.Checked;
            certEnrolledCheck.Focus();
        }

        protected void certPassedCheck_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or disable the dropdown based on the checkbox state
            certRegNo.Enabled = certPassedCheck.Checked;
            certPassedCheck.Focus();
        }


        public string GetAadharCardValue(TextBox aadharCard)
        {
            // Extract the text from the TextBox
            string aadharCardValue = aadharCard.Text.ToString();

            // Remove any non-digit characters
            aadharCardValue = new string(aadharCardValue.Where(char.IsDigit).ToArray());

            // Ensure the value is exactly 12 digits
            if (aadharCardValue.Length > 12)
            {
                aadharCardValue = aadharCardValue.Substring(0, 12);
            }
            else if (aadharCardValue.Length < 12)
            {
                // Handle the case where the number is less than 12 digits
                aadharCardValue = string.Empty; // or handle it as needed
            }

            return aadharCardValue;
        }
        private void ShowAlert(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message.Replace("'", "\\'").Replace("\n", "\\n")}');", true);
        }
        private DateTime? ParseDate(string dateText, string dateFormat = "dd/MM/yyyy")
        {
            if (string.IsNullOrWhiteSpace(dateText)) return null; // Handle empty or whitespace input

            // Use DateTime.TryParseExact to ensure proper date format handling
            return DateTime.TryParseExact(dateText, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
                ? parsedDate
                : (DateTime?)null;
        }
        private void FillBranchList()
        {
            // Fetch the branch master list
            DataTable dt = new WM.Controllers.AssociateController().GetBranchMasterList();

            // Bind the fetched data to the dropdown
            ddlBranchAM.DataSource = dt;
            ddlBranchAM.DataTextField = "BRANCH_NAME";   // Text field for dropdown display
            ddlBranchAM.DataValueField = "BRANCH_CODE";  // Value field for dropdown value
            ddlBranchAM.DataBind();

            // Insert a default "Select" option at the top
            ddlBranchAM.Items.Insert(0, new ListItem("Select", ""));


        }


        public void FillStateForCity()
        {
            DataTable dt = new WM.Controllers.AssociateController().GetStateForCityList();

            ddlState.DataSource = dt;
            ddlState.DataTextField = "text";

            ddlState.DataValueField = "value";

            ddlState.DataBind();

            ddlState.Items.Insert(0, new ListItem("Select", "0"));
        }

        private void FillCityByStateList(string stateId)
        {

            if (string.IsNullOrWhiteSpace(stateId) || stateId == '0'.ToString())
            {
                FillCityMainList();
            }
            if (!string.IsNullOrWhiteSpace(stateId))
            {
                ddlCityAM.Items.Clear();
                try
                {
                    // Retrieve the city list based on the state ID
                    DataTable cityDataTable = new WM.Controllers.AssociateController().GetCityByStatelist(stateId);
                    ddlCityAM.DataSource = cityDataTable;
                    ddlCityAM.DataTextField = "CITY_NAME";
                    ddlCityAM.DataValueField = "CITY_ID";
                    ddlCityAM.DataBind();
                    ddlCityAM.Items.Insert(0, new ListItem("Select", ""));
                }
                catch (Exception ex)
                {

                }
            }
        }




        protected void ddlStateID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string stateCode = ddlState.SelectedValue.ToString();
            FillCityByStateList(stateCode);
            ddlCityAM.Focus();

        }
        protected void ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Get selected SourceID
            string selectedSourceID = ddlBranchAM.SelectedValue;

            // Call FillRMList to populate RM dropdown based on selected SourceID
            FillRMList(selectedSourceID);
            FillLocationListByCityBranch(selectedSourceID);

            BindRmToDropdown(selectEmployee, selectedSourceID);
            selectEmployee.Focus();
            source.Text = selectedSourceID;
            neftAccountNo.Text = accountNo.Text;
        }
        private void FillRMList(string sourceID)
        {
            DataTable dt = new WM.Controllers.AssociateController().GetRMListBySourceBranch(sourceID);
            selectEmployee.DataSource = dt;
            selectEmployee.DataTextField = "AGENT_NAME";
            selectEmployee.DataValueField = "RM_CODE";
            selectEmployee.DataBind();
            selectEmployee.Items.Insert(0, new ListItem("Select", ""));
        }
        private void FillCityMainList()
        {
            DataTable dt1 = new DataTable();

            dt1 = new WM.Controllers.AssociateController().GetCitylist();

            ddlCityAM.DataSource = dt1;
            ddlCityAM.DataTextField = "CITY_NAME";
            ddlCityAM.DataValueField = "CITY_ID";
            ddlCityAM.DataBind();
            ddlCityAM.Items.Insert(0, new ListItem("Select", ""));

        }
        private void FillCityList()
        {
            //DataTable dt1 = new DataTable();
            //dt1 = new WM.Controllers.AssociateController().GetCitylist();
            //ddlCityAM.DataSource = dt1;
            //ddlCityAM.DataTextField = "CITY_NAME";
            //ddlCityAM.DataValueField = "CITY_ID";
            //ddlCityAM.DataBind();
            //ddlCityAM.Items.Insert(0, new ListItem("Select", ""));

            DataTable dt2 = new DataTable();
            dt2 = new WM.Controllers.AssociateController().GetCitylist();
            //AddDefaultItem(dt2, "CITY_NAME", "CITY_ID", "Select");

            ddlBankCityAM.DataSource = dt2;
            ddlBankCityAM.DataTextField = "CITY_NAME";
            ddlBankCityAM.DataValueField = "CITY_ID";
            ddlBankCityAM.DataBind();
            ddlBankCityAM.Items.Insert(0, new ListItem("Select", ""));
        }
        protected void ddlCityAM_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (ddlCityAM.SelectedValue == "0")
            //{
            //    state.Text = "City not selected";

            //}
            
            //string selecteCity_Code = ddlCityAM.SelectedValue;

            //FillStateNameBySelectedCity(selecteCity_Code);
            //ddlCityAM.Focus();
        }
        private void FillStateNameBySelectedCity(string sourceID)
        {
            try
            {
                string stateNameSelectedCity = new WM.Controllers.AssociateController().GetStateNameByCity(sourceID);

                state.Text = stateNameSelectedCity;

                DataTable dt = new WM.Controllers.AssociateController().GetStateByCity(sourceID);
                DataRow row = dt.Rows[0];
                ddlState.SelectedValue = row["state_id"].ToString();
                FillCityByStateList(row["state_id"].ToString());
            }
            catch (Exception ex)
            {
                // Handle any errors, for example, log the error or display a message
                Console.WriteLine("Error: " + ex.Message);
                state.Text = "Error fetching state name.";
            }
        }

        private void FillLocationListByCityBranch(string cityBranch)
        {
            DataTable dt = new AssociateController().GetLocationListBySourceBranch(cityBranch);

            AddDefaultItem(dt, "LOCATION_NAME", "LOCATION_ID", "Select Location");

            ddlLocationAM.DataSource = dt;
            ddlLocationAM.DataTextField = "LOCATION_NAME";
            ddlLocationAM.DataValueField = "LOCATION_ID";
            ddlLocationAM.DataBind();
        }
        private void FillAssociateTypeListCat()
        {
            DataTable dt1 = new DataTable();

            dt1 = new WM.Controllers.AssociateController().GetAssociateTypeListCat();

            ddlAssociateTypeCategory.DataSource = dt1;
            ddlAssociateTypeCategory.DataTextField = "CATEGORY_NAME";
            ddlAssociateTypeCategory.DataValueField = "CATEGORY_ID";
            ddlAssociateTypeCategory.DataBind();
            ddlAssociateTypeCategory.Items.Insert(0, new ListItem("Select", ""));

        }
        private void FillAssociateTypeList()
        {
            DataTable dt1 = new DataTable();

            dt1 = new WM.Controllers.AssociateController().GetAssociateTypeList();

            ddlAssociateType.DataSource = dt1;
            ddlAssociateType.DataTextField = "INVESTOR_TYPE";
            ddlAssociateType.DataValueField = "INVESTOR_CODE";
            ddlAssociateType.DataBind();
            ddlAssociateType.Items.Insert(0, new ListItem("Select", ""));

        }
        private void FillSuperANAAgentList()
        {
            DataTable dt = new AssociateController().GetAgentMasterDetails();
            superAna.DataSource = dt;
            superAna.DataTextField = "AGENT_NAME";
            superAna.DataValueField = "AGENT_CODE";
            superAna.DataBind();
            superAna.Items.Insert(0, new ListItem("Select", ""));
            superAna.Items.Insert(2, new ListItem("SANA", "SANA"));  // Inserting "SANA, null"
            superAna.Items.Insert(2, new ListItem("LSANA", "LSANA"));  // Inserting "LSANA"



        }
        private void SuperAnafillSourceList()
        {

            DataTable dt = new WM.Controllers.AgentController().GetBranchList();

            ddlSourceID.DataSource = dt;
            ddlSourceID.DataTextField = "BRANCH_NAME";
            ddlSourceID.DataValueField = "BRANCH_CODE";
            ddlSourceID.DataBind();
            ddlSourceID.Items.Insert(0, new ListItem("Select", ""));

        }
        protected void RM_ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected SourceID
            string selectedSourceID = ddlSourceID.SelectedValue;

            // Call fillRMList to populate RM dropdown based on selected SourceID
            fillRMList(selectedSourceID);
        }
        private void fillRMList(string sourceID)
        {
            DataTable dt = new WM.Controllers.AgentController().GetRMListBySource(sourceID);
            ddlRM.DataSource = dt;
            ddlRM.DataTextField = "AGENT_NAME";  // Update to your actual column name
            ddlRM.DataValueField = "RM_CODE";    // Update to your actual column name
            ddlRM.DataBind();
            ddlRM.Items.Insert(0, new ListItem("Select", ""));
        }
        protected void btnSearch_Click_SuperAna(object sender, EventArgs e)
        {
            int? SOURCEID = ddlSourceID.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlSourceID.SelectedValue, out int sourceId) ? (int?)sourceId : null);

            int? RM_CODE = ddlRM.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlRM.SelectedValue, out int rmCode) ? (int?)rmCode : null);

            string AGENT_NAME = string.IsNullOrEmpty(txtAgentName.Text) ? null : txtAgentName.Text.Trim();

            string EXIST_CODE = string.IsNullOrEmpty(txtExistCode.Text) ? null : txtExistCode.Text.Trim();

            DataTable dt = new WM.Controllers.AgentController().GetFilteredAgentsData(SOURCEID, RM_CODE, AGENT_NAME, EXIST_CODE);

            // Bind data to GridView
            agentsGrid.Visible = true;
            agentsGrid.DataSource = dt;
            agentsGrid.DataBind();
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
        protected void btnSetSuperAgent_Click(object sender, EventArgs e)
        {
            bool itemSelected = false; // Flag to track if any item is selected

            // Loop through all the rows in the GridView
            foreach (GridViewRow row in agentsGrid.Rows)
            {
                // Find the checkbox control in the current row
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");

                // Check if the checkbox is checked
                if (chkSelect != null && chkSelect.Checked)
                {
                    itemSelected = true; // Set flag to true if an item is selected

                    // Retrieve the values from the labels
                    string agentCode = ((Label)row.FindControl("lblAgentCodeSearched")).Text;
                    string agentName = ((Label)row.FindControl("lblAgentNameSearched")).Text;

                    // Check if the agent already exists in the dropdown
                    bool ifExist = superAna.Items.Cast<ListItem>().Any(item => item.Value.Equals(agentCode));

                    if (!ifExist)
                    {
                        superAna.Items.Insert(0, new ListItem(agentName, agentCode)); // Insert item if it does not exist
                        superAna.Focus();
                    }
                }
            }

            // If no item is selected, alert the user
            if (!itemSelected)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please select any agent to add in list');", true);
            }
            else
            {
                // Close the modal if at least one item was added
                ScriptManager.RegisterStartupScript(this, GetType(), "btnCloseSuperAnaModal", "closeSuperAnaFindModal();", true);
            }
        }
        protected void ResetSuperANAModelFields(object sender, EventArgs e)
        {
            // Reset the DropDownList
            ddlRM.SelectedValue = "0"; // Set to "Select"

            // Reset the TextBox controls
            txtAgentName.Text = string.Empty; // Clear Agent Name
            txtExistCode.Text = string.Empty; // Clear Exist Code

            // Optionally, you may want to clear the GridView data if necessary
            agentsGrid.DataSource = null; // Clear the GridView data source
            agentsGrid.DataBind(); // Rebind the GridView to reflect the cleared data
        }
        private void FillBankMasterDetails()
        {
            DataTable dt1 = new DataTable();
            dt1 = new WM.Controllers.AssociateController().GetBankMasterDetails();
            bankName.DataSource = dt1;
            bankName.DataTextField = "BANK_NAME";
            bankName.DataValueField = "BANK_ID";
            bankName.DataBind();
            bankName.Items.Insert(0, new ListItem("Select", ""));


            DataTable dt2 = new DataTable();
            dt2 = new WM.Controllers.AssociateController().GetBankMasterDetails();

            branchName.DataSource = dt2;
            branchName.DataTextField = "BRANCH";
            branchName.DataValueField = "BRANCH_CODE";
            branchName.DataBind();
            branchName.Items.Insert(0, new ListItem("Select", ""));

        }
        private void FillPaymentModeID()
        {
            DataTable dt = new WM.Controllers.AssociateController().GetPaymentModeID();

            paymentMode.DataSource = dt;
            paymentMode.DataTextField = "itemname";
            paymentMode.DataValueField = "itemserialnumber";
            paymentMode.DataBind();
            paymentMode.Items.Insert(0, new ListItem("Select", ""));

        }
        private void FillExamList()
        {
            DataTable dt = new WM.Controllers.AssociateController().GetExamList();

            ddlCertExam.DataSource = dt;
            ddlCertExam.DataTextField = "itemname";
            ddlCertExam.DataValueField = "itemserialnumber";
            ddlCertExam.DataBind();
            ddlCertExam.Items.Insert(0, new ListItem("Select", ""));
        }
        private void FillBankAccountTypeID()
        {

            DataTable dt = new WM.Controllers.AssociateController().GetBankAccountTypeID();

            accountType.DataSource = dt;
            accountType.DataTextField = "itemname";
            accountType.DataValueField = "itemserialnumber";
            accountType.DataBind();
            accountType.Items.Insert(0, new ListItem("Select", ""));

        }
        private void FillOtherTypeID()
        {

            DataTable dt = new WM.Controllers.AssociateController().GetOtherTypeID();

            type.DataSource = dt;
            type.DataTextField = "itemname";
            type.DataValueField = "itemserialnumber";
            type.DataBind();
            type.Items.Insert(0, new ListItem("Select", "0"));

        }
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
        protected void btnAssociateList(object sender, EventArgs e)
        {
            Response.Redirect("~/Masters/associate_list.aspx");

        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            RemoveStidFromUrl();
            ResetFields();
        }
        protected void btnNewBranchClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Masters/AddNewBranchType.aspx");

        }
        protected void btnNewBankClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Masters/addnewbank.aspx");

        }
        protected void btnNewCityClick(object sender, EventArgs e)
        {
            Response.Redirect("~/Masters/addnewcity.aspx");

        }
        protected void btnExitAssociatePage_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/welcome.aspx");

        }
        private void ResetFields()
        {
            ddlState.SelectedIndex = -1;
            txtDTNumber.Text = string.Empty;
            resAddAddress1.Text = string.Empty;
            resAddAddress2.Text = string.Empty;
            resAddState.Text = string.Empty;
            resAddCity.Text = string.Empty;
            resAddPIN.Text = string.Empty;

            // Reset date fields
            auditDate.Text = string.Empty;
            empanelmentDate.Text = string.Empty;
            affectedFrom.Text = string.Empty;
            dob.Text = string.Empty;

            state.Text = string.Empty;

            pospCertifiedLiOn.Text = string.Empty;
            pospCertifiedLiOnValidTill.Text = string.Empty;
            pospCertifiedGiOn.Text = string.Empty;
            pospCertifiedGiOnValidTill.Text = string.Empty;

            // Reset dropdowns
            empanelmentType.SelectedIndex = 0;
            associateCode.Text = string.Empty;
            subBrokerExistCode.Text = string.Empty;
            associateName.Text = string.Empty;
            ddlBranchAM.SelectedIndex = 0;
            selectEmployee.SelectedIndex = 0;

            // Reset multiline TextBox
            address1.Text = string.Empty;

            // Reset other fields
            ddlCityAM.SelectedIndex = 0;
            ddlLocationAM.SelectedIndex = 0;
            mobile.Text = string.Empty;
            pinCode.Text = string.Empty;

            fax.Text = string.Empty;
            contactPerson.Text = string.Empty;
            emailId.Text = string.Empty;
            tds.Text = string.Empty;
            ddlAssociateType.SelectedIndex = 0;
            ddlAssociateTypeCategory.SelectedIndex = 0;


            contactPersionemailId.Text = string.Empty;
            phone.Text = string.Empty;
            remarks.Text = string.Empty;
            source.Text = string.Empty;
            onlinePlatformRemark.Text = string.Empty;
            offlinePlatformRemark.Text = string.Empty;

            superAna.SelectedIndex = 0;
            chkbOnlinePlaformBlock.Checked = false;
            chkbOfflinePlaformBlock.Checked = false;
            onlineSubscription.Checked = false;
            audit.Checked = false;

            paymentMode.SelectedIndex = 0;
            accountType.SelectedIndex = 0;
            accountNo.Text = string.Empty;

            bankName.SelectedIndex = 0;
            ddlBankCityAM.SelectedIndex = 0;
            branchName.SelectedIndex = 0;

            sms.Checked = false;
            gstin.Text = string.Empty;
            type.SelectedIndex = 0;
            panGir.Text = string.Empty;
            circleWardDist.Text = string.Empty;
            aadharCard.Text = string.Empty;

            pospMarking.SelectedIndex = 0;
            pospType.Text = string.Empty;
            pospNoLi.Text = string.Empty;
            pospNoGi.Text = string.Empty;

            verified.Checked = false;

            neftBankName.Text = string.Empty;
            neftBranch.Text = string.Empty;
            neftIFCSCode.Text = string.Empty;
            neftName.Text = string.Empty;
            neftAccountNo.Text = string.Empty;

            certEnrolledCheck.Checked = false;

            certPassedCheck.Checked = false;
            ddlCertExam.SelectedIndex = 0;
            certRegNo.Text = string.Empty;
            lblMessage.Text = string.Empty;
        }
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
        private void fillAgentViewBranchList()
        {

            DataTable dt = new WM.Controllers.AssociateController().GetBranchMasterList();

            branchsbl.DataSource = dt;
            branchsbl.DataTextField = "BRANCH_NAME";
            branchsbl.DataValueField = "BRANCH_CODE";
            branchsbl.DataBind();
            branchsbl.Items.Insert(0, new ListItem("Select", ""));

        }
        private void fillAgentViewCityList()
        {
            DataTable dt = new WM.Controllers.IssuerController().GetCitylist();
            citysbl.DataSource = dt;
            citysbl.DataTextField = "CITY_NAME";
            citysbl.DataValueField = "CITY_ID";
            citysbl.DataBind();
            citysbl.Items.Insert(0, new ListItem("Select", ""));

        }
        public string ConvertToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        public bool IsSearchCriteriaFilled()
        {

        

            string BRANCH_ID = branchsbl.SelectedValue == "0" ? null : branchsbl.SelectedValue;
            string CITY_ID = citysbl.SelectedValue == "0" ? null : citysbl.SelectedValue;
            string ASSOCIATE_CODE = codesbl.Text.Trim();
            string NAME = ConvertToSentenceCase(namesbl.Text.Trim());
            string MOBILE = string.IsNullOrEmpty(mobilesbl.Text.Trim()) ? null : mobilesbl.Text.Trim();
            string PHONE = string.IsNullOrEmpty(phonesbl.Text.Trim()) ? null : phonesbl.Text.Trim();
            string PAN_NO = string.IsNullOrEmpty(panNosbl.Text.Trim()) ? null : panNosbl.Text.Trim();

            // Check if at least one field has a value
            return !string.IsNullOrEmpty(BRANCH_ID) ||
                   !string.IsNullOrEmpty(CITY_ID) ||
                   !string.IsNullOrEmpty(ASSOCIATE_CODE) ||
                   !string.IsNullOrEmpty(NAME) ||
                   !string.IsNullOrEmpty(MOBILE) ||
                   !string.IsNullOrEmpty(PHONE) ||
                   !string.IsNullOrEmpty(PAN_NO);
        }

        protected void btnAssListSearchsbl_Click(object sender, EventArgs e)
        {
            string BRANCH_ID = branchsbl.SelectedValue == "0" ? null : branchsbl.SelectedValue;
            string CITY_ID = citysbl.SelectedValue == "0" ? null : citysbl.SelectedValue;
            string ASSOCIATE_CODE = codesbl.Text.Trim();
            string NAME = namesbl.Text.Trim();
            string MOBILE = string.IsNullOrEmpty(mobilesbl.Text.Trim()) ? null : mobilesbl.Text.Trim();
            string PHONE = string.IsNullOrEmpty(phonesbl.Text.Trim()) ? null : phonesbl.Text.Trim();
            string PAN_NO = string.IsNullOrEmpty(panNosbl.Text.Trim()) ? null : panNosbl.Text.Trim();

            ResetAssociateListGrid();

            if (!IsSearchCriteriaFilled())
            {
                ShowAlert("Need at least one field value");
                return;
            }

            else
            {
                DataTable dt = new AssociateController().GetAssociateListByCriteria(BRANCH_ID, CITY_ID, MOBILE, PHONE, ASSOCIATE_CODE, PAN_NO, NAME);
                if (dt.Rows.Count > 0)
                {
                    lblAssociateListMessage.Text = "Total " + dt.Rows.Count + (dt.Rows.Count == 1 ? " record found!" : " records found!");
                    agentListDetailsGridsbl.Visible = true;
                    agentListDetailsGridsbl.DataSource = dt;
                    agentListDetailsGridsbl.DataBind();
                }
                else
                {
                    ResetAssociateListGrid();
                    lblAssociateListMessage.Text = "No records found!";
                }
            }

        }
    
        
        protected void btnAssListReset_Click(object sender, EventArgs e)
        {
            // Clear the search input fields
            branchsbl.SelectedIndex = 0; // Assuming the first item is "Select"
            citysbl.SelectedIndex = 0; // Assuming the first item is "Select"
            mobilesbl.Text = string.Empty;
            phonesbl.Text = string.Empty;
            codesbl.Text = string.Empty;
            panNosbl.Text = string.Empty;
            namesbl.Text = string.Empty;

            ResetAssociateListGrid();




        }
        protected void ResetAssociateListMain()
        {
            // Clear the search input fields
            branchsbl.SelectedIndex = 0; // Assuming the first item is "Select"
            citysbl.SelectedIndex = 0; // Assuming the first item is "Select"
            mobilesbl.Text = string.Empty;
            phonesbl.Text = string.Empty;
            codesbl.Text = string.Empty;
            panNosbl.Text = string.Empty;
            namesbl.Text = string.Empty;

            // Clear the GridView
            agentListDetailsGridsbl.DataSource = null;
            agentListDetailsGridsbl.DataBind();
            agentListDetailsGridsbl.Visible = false;
        }
        protected void ResetAssociateListGrid()
        {
            if (agentListDetailsGridsbl.Rows.Count > 0)
            {
                lblAssociateListMessage.Text = string.Empty;
                agentListDetailsGridsbl.DataSource = null;
                agentListDetailsGridsbl.DataBind();
                agentListDetailsGridsbl.Visible = false;
            }
        }
        protected void btnSelect_Click_1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            GridViewRow row = btn.NamingContainer as GridViewRow;

            if (btn != null && row != null)
            {
                string agentCode = agentListDetailsGridsbl.DataKeys[row.RowIndex]["AGENT_CODE"].ToString();
                FillAssociateDataByAgentCode(agentCode);
            }
        }
        protected void btnAssListSelect_Click(object sender, EventArgs e)
        {
            string selectedAgentCode = string.Empty;

            // Loop through each row in the GridView
            foreach (GridViewRow row in agentListDetailsGridsbl.Rows)
            {
                // Find the CheckBox control
                CheckBox chk = row.FindControl("chkSelect") as CheckBox;

                if (chk != null && chk.Checked)
                {
                    // Retrieve the Agent Code from the DataKey
                    selectedAgentCode = agentListDetailsGridsbl.DataKeys[row.RowIndex]["AGENT_CODE"].ToString();
                    break; // Only one should be selected, so we can stop the loop
                }
            }

            if (string.IsNullOrEmpty(selectedAgentCode))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Please select any agent');", true);

            }

            else if (!string.IsNullOrEmpty(selectedAgentCode))
            {
                // Process the selected agent code (For example, filling the associate data)
                FillAssociateDataByAgentCode(selectedAgentCode);

                ScriptManager.RegisterStartupScript(this, GetType(), "closeModel", "closeAssociateListModal();", true);
            }
            else
            {
                // Optionally, show a message if no agent was selected
                lblMessage.Text = string.Empty;
            }
        }
        protected void gvAgentSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                string agentCode = e.CommandArgument.ToString();
                ResetFields();

                try
                {
                    // Fill data based on the agent code
                    FillAssociateDataByAgentCode(agentCode);
                    if (associateCode.Text != string.Empty)
                    {
                        ResetAssociateListMain();
                        lblAssociateListMessage.Text = string.Empty;
                    }

                    // Close the modal automatically after a delay
                    // Register a client-side script to close the modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CloseAgentModal", "closeAssociateListModal();", true);
                }
                catch (Exception ex)
                {
                    // Handle exception if any error occurs
                }
            }
        }
        protected void pospType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPospType = pospType.SelectedValue;
            pospType.Focus();

            // Enable/Disable fields based on the selected value of pospType
            if (selectedPospType == "Life")
            {
                EnableLifeFields();
                DisableGeneralFields();
                pospNoLi.Focus(); // Replace with the ID of the life field
            }
            else if (selectedPospType == "General")
            {
                EnableGeneralFields();
                DisableLifeFields();
                pospNoGi.Focus(); // Replace with the ID of the general field
            }
            else if (selectedPospType == "Both")
            {
                EnableLifeFields();
                EnableGeneralFields();
                pospNoLi.Focus();
            }
            else
            {
                DisableAllFields(); // For "Select POSP Type" or empty selection
                pospType.Focus();
            }
        }



        private void EnableLifeFields()
        {
            pospNoLi.Enabled = true;
            pospCertifiedLiOn.Enabled = true;
            pospCertifiedLiOnValidTill.Enabled = true;
        }
        private void DisableLifeFields()
        {
            pospNoLi.Enabled = false;
            pospCertifiedLiOn.Enabled = false;
            pospCertifiedLiOnValidTill.Enabled = false;

            // Clear Life fields
            pospNoLi.Text = "";
            pospCertifiedLiOn.Text = "";
            pospCertifiedLiOnValidTill.Text = "";
        }
        private void EnableGeneralFields()
        {
            pospNoGi.Enabled = true;
            pospCertifiedGiOn.Enabled = true;
            pospCertifiedGiOnValidTill.Enabled = true;
        }
        private void DisableGeneralFields()
        {
            pospNoGi.Enabled = false;
            pospCertifiedGiOn.Enabled = false;
            pospCertifiedGiOnValidTill.Enabled = false;

            // Clear General fields
            pospNoGi.Text = "";
            pospCertifiedGiOn.Text = "";
            pospCertifiedGiOnValidTill.Text = "";
        }
        private void DisableAllFields()
        {
            DisableLifeFields();
            DisableGeneralFields();
        }

    }
}