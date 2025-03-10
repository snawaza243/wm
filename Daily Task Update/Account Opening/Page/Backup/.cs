using DocumentFormat.OpenXml.Bibliography;
using NPOI.SS.Formula;
using System;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;

namespace WM.Masters
{
    public partial class associate_master : System.Web.UI.Page
    {
        protected string currentLoginId;
        protected string currentRoleId;
        protected string tempRole = "261"; // Temporary role

        protected void Page_Load(object sender, EventArgs e)
        {
            currentLoginId = Session["LoginId"] as string ?? null;
            currentRoleId = Session["RoleId"] as string ?? null;
            //currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            //currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            if (currentLoginId == null)
            {
                Session["loggedAngBranches"] = null;
                //Response.Redirect("~/index.aspx");
                Response.Redirect($"https://wealthmaker.in/login_new.aspx");
            }
            else
            {
                if (!IsPostBack)
                {

                    #region Role based access buttons
                    //string currentLoginId = Session["LoginId"] as string ?? null;
                    //string currentRoleId = Session["RoleId"] as string ?? null;
                    //string tmepRole = "261";
                    //string currentBranches = Branches();

                    //currentRoleId = "212";
                    //if (currentRoleId == "212")
                    //{
                    //    ddlBranchAM.Enabled = false;
                    //    selectEmployee.Enabled = false;
                    //    saveButton.Enabled = true;
                    //}

                    #endregion
                    FillTitle();
                    fillCountryList();
                    fillStateList();
                    ddlMailingCountryList_Fun("1", ddlMailingCountryList, ddlMailingStateList, ddlMailingCityList, txtMailingPin);
                    ddlMailingCountryList.SelectedValue = "1";
                    if (ddlMailingCountryList.SelectedValue == "1")
                    {
                        if (ddlMailingStateList.Items.Count > 0)
                        {
                            string currentState = "102"; // delhi
                            ddlMailingStateListFillCity(currentState);
                            ddlMailingStateList.SelectedValue = currentState;
                            ddlMailingStateList.Enabled= true;
                            ddlMailingStateListFillCity(currentState);

                            string nn = ddlMailingStateList.SelectedItem.Text;
                        }

                    }

                    ddlMailingCityList.SelectedIndex = 0;

                    //FillBranchList(logId);
                    FillPaymentModeID();
                    FillBankAccountTypeID();
                    FillBankBranchData();
                    FillBankMasterData();
                    //FilAllCityList(); 
                    AssociateSearchAllCityList();//  for agent view model, for bank city list
                    FillSuperANAAgentList();        // IN SUPER ANA MODEL ON LOAD 500 ANA AGENT
                    FillOtherTypeID();
                    FillExamList();
                    FillAssociateTypeList();
                    FillAssociateTypeListCat();
                    DisableAllFields();
                    FillRMList(null);
                    //FillLocationListByCityBranch(null,null, null);

                    if (ddlLocationAM.Items.Count > 1)
                    {
                        ddlLocationAM.Enabled = true;
                    }
                    else
                    {
                        ddlLocationAM.Enabled = false;
                    }

                    FillValidBranchList(); // only for branch list disable for 212
                    FillChannelBranches(); // Fol associate and super ana search model

                    txtAgentName.Focus();

                    if (string.IsNullOrEmpty(associateCode.Text))
                    {
                        upateButton.Enabled = false;
                        saveButton.Enabled = true;
                    }
                    else
                    {
                        upateButton.Enabled = true;
                        saveButton.Enabled = false;
                    }

                }

            }

            if (Request.QueryString["stid"] != null)
            {
                string payrollId = Request.QueryString["stid"];
                associateCode.Text = payrollId;
                FillAssociateDataByAgentCode(payrollId);
            }
        }


        protected void ddlresAddCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected SourceID
            string currentREsCity = ddlresAddCity.SelectedValue;

            //fillRMList(selectedSourceID);

            DataTable dt = new WM.Controllers.AssociateController().GetStateByCity(currentREsCity);
            int dtRowCount = dt.Rows.Count;

            if (dtRowCount > 0)
            {
                DataRow row = dt.Rows[0];
                //ddlRM.Enabled = true;
                //ddlRM.DataSource = dt;
                //ddlRM.DataTextField = "RM_NAME";
                //ddlRM.DataValueField = "RM_CODE";
                //ddlRM.DataBind();
                //ddlRM.Items.Insert(0, new ListItem("Select", ""));
                resAddState.Text = GetTextFieldValue(row, "state_name");
            }
            else
            {
                resAddState.Text = string.Empty;

            }

        }


        public string Branches(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            string[] branchCodes = new string[0];
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                WM.Controllers.AssociateController controller = new WM.Controllers.AssociateController();
                branchCodes = controller.GetBranchCodes(currentLoginId, currentRoleId);
            }

            Session["loggedAngBranches"] = string.Join(",", branchCodes);
            return string.Join(",", branchCodes);
        }

        public DataTable GetValidBranched(string currentLoginId = null, string currentRoleId = null)
        {
            // Retrieve session values if parameters are not provided
            if (string.IsNullOrEmpty(currentLoginId))
                currentLoginId = HttpContext.Current.Session["LoginId"] as string;

            if (string.IsNullOrEmpty(currentRoleId))
                currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            // Initialize an empty array to avoid returning null
            DataTable branchTable = new DataTable();

            // Ensure session values are not null before proceeding
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                // Create an instance of AssociateController
                WM.Controllers.AssociateController controller = new WM.Controllers.AssociateController();

                // Get branch codes from the controller and assign to branchCodes
                branchTable = controller.PSMGetBranches(currentLoginId, currentRoleId);
            }

            return branchTable;
        }

        protected void FillValidBranchList()
        {
            DataTable dt = GetValidBranched();
            int dtCount = dt.Rows.Count;

            if (dtCount>0)
            {

                ddlBranchAM.DataSource = dt;
                ddlBranchAM.DataTextField = "branch_name"; 
                ddlBranchAM.DataValueField = "branch_code";  
                ddlBranchAM.DataBind();
                ddlBranchAM.Items.Insert(0, new ListItem("Select", ""));
                ddlBranchAM.Enabled = false;
            }
        }


        public void FillChannelBranches(string currentLoginId = null, string currentRoleId = null)
        {
            // Retrieve session values if parameters are not provided
            if (string.IsNullOrEmpty(currentLoginId))
                currentLoginId = HttpContext.Current.Session["LoginId"] as string;

            if (string.IsNullOrEmpty(currentRoleId))
                currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            // Initialize an empty array to avoid returning null
            DataTable dt = new DataTable();

            // Ensure session values are not null before proceeding
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                // Create an instance of AssociateController
                WM.Controllers.AssociateController controller = new WM.Controllers.AssociateController();

                // Get branch codes from the controller and assign to branchCodes
                dt = controller.PSMGetChannewlBranches(currentLoginId, currentRoleId);
                int rowCount = dt.Rows.Count;

                if (rowCount > 0)
                {

                    branchsbl.DataSource = dt;
                    branchsbl.DataTextField = "branch_name";
                    branchsbl.DataValueField = "branch_code";
                    branchsbl.DataBind();
                    branchsbl.Items.Insert(0, new ListItem("Select Branch", ""));

                    // super ana search 
                    ddlSourceID.DataSource = dt;
                    ddlSourceID.DataTextField = "branch_name";
                    ddlSourceID.DataValueField = "branch_code";
                    ddlSourceID.DataBind();
                    ddlSourceID.Items.Insert(0, new ListItem("Select Branch", ""));
                }
            }

            ;
        }
        protected void ddlMailingCountryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string countryID = ddlMailingCountryList.SelectedValue.ToString();
            ddlMailingCountryList_Fun(countryID, ddlMailingCountryList, ddlMailingStateList, ddlMailingCityList, txtMailingPin);

            if (ddlMailingStateList.Items.Count > 0)
            {
                ddlMailingStateList.Focus();
            }

        }


        protected void ddlMailingCountryList_Fun(string countryID, DropDownList ddlMailingCountryList, DropDownList ddlMailingStateList, DropDownList ddlMailingCityList, TextBox txtMailingPin)
        {

            if (!string.IsNullOrEmpty(countryID))
            {

                try
                {
                    int selectedCountryId = Convert.ToInt32(countryID);
                    ddlMailingCountryList.SelectedValue = countryID;
                    string selectedCountryName = ddlMailingCountryList.SelectedItem.ToString();

                    if (selectedCountryId > 0) // Check if a valid country is selected
                    {
                        PopulateStateDropDownForAddress(selectedCountryId, ddlMailingStateList);

                        if (ddlMailingStateList.Items.Count > 0)
                        {
                            ddlMailingStateList.Enabled = true;
                            ddlMailingStateList.SelectedIndex = 0;
                            // ddlMailingStateList.Focus();
                        }
                        else
                        {
                            ddlMailingStateList.Items.Insert(0, new ListItem("Select State", ""));
                            ddlMailingStateList.SelectedIndex = 0;
                            ddlMailingStateList.Enabled = false;
                        }

                        if (ddlMailingCityList.Items.Count > 0)
                        {
                            ddlMailingCityList.SelectedIndex = 0;
                        }
                        ddlMailingCityList.Enabled = false;

                        if (ddlLocationAM.Items.Count > 0)
                        {
                            ddlLocationAM.SelectedIndex = 0;
                        }
                        ddlLocationAM.Enabled = false;
                        txtMailingPin.Text = string.Empty;

                        HandletxtMailingPinValidation(selectedCountryName, txtMailingPin);

                    }

                }
                catch (FormatException ex)
                {
                    // Display error message in lblMessage
                    //  lblMessage.Text = "Error: Invalid format for country ID.";
                }
                catch (Exception ex)
                {
                    // Display generic error message in lblMessage
                    lblMessage.Text = "Error: An unexpected error occurred while selecting the country.";
                }

            }
            else
            {
                if (ddlMailingStateList.Items.Count > 0)
                {
                    ddlMailingStateList.SelectedIndex = 0;
                }

                if (ddlMailingCityList.Items.Count > 0)
                {
                    ddlMailingCityList.SelectedIndex = 0;
                }
                ddlMailingStateList.Enabled = false;
                ddlMailingCityList.Enabled = false;


            }


        }

        protected void ddlMailingStateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentState = ddlMailingStateList.SelectedValue.ToString();
            ddlMailingStateListFillCity(currentState);
            
        }
        protected void ddlMailingCityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string currentCity = ddlMailingCityList.SelectedValue.ToString();
            ddlMailingCityListFillLocation(currentCity, null, null);

        }
        protected void ddlMailingLocationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentLocation = ddlLocationAM.SelectedValue.ToString();
            ddlMailingCityListFillLocation(null, currentLocation, null);

        }
       
        
        protected void ddlMailingStateListFillCity(string stateID)
        {
            if (!string.IsNullOrEmpty(stateID))
            {
                lblMessage.Text = "";  // Clear previous message
                try
                {
                    int selectedStateId = Convert.ToInt32(stateID); // Convert Value to integer

                    if (selectedStateId > 0) // Check if a valid state is selected
                    {
                        PopulateCityDropDownForAddress(selectedStateId, ddlMailingCityList);
                        //fillCityList();
                        if (ddlMailingCityList.Items.Count > 0)
                        {
                            ddlMailingCityList.SelectedIndex = 0;
                            ddlMailingCityList.Enabled = true;
                            ddlMailingCityList.Focus();
                        }
                        else
                        {
                            // Handle case when "Select State" (value 0) is selected
                            ddlMailingCityList.Items.Add(new ListItem("Select City", "0"));
                            ddlMailingCityList.SelectedIndex = 0;
                        }
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
            else
            {
                if (ddlMailingCityList.Items.Count > 0)
                {
                    ddlMailingCityList.SelectedIndex = 0;
                }
                ddlMailingCityList.Enabled = false;
            }

        }

        protected void ddlMailingCityListFillLocation(string cityId, string locationId, string pin)
        {
            if (!string.IsNullOrEmpty(cityId) || !string.IsNullOrEmpty(locationId) || !string.IsNullOrEmpty(pin) )
            {
                // Fetch data using all three parameters
                DataTable dt = new AssociateController().GetLocationList(cityId, locationId, pin);
                int dtRowsCount = dt.Rows.Count;

                if (dtRowsCount > 0)
                {
                    if (!string.IsNullOrEmpty(cityId))
                    {

                    ddlLocationAM.Enabled = true;
                    AddDefaultItem(dt, "LOCATION_NAME", "LOCATION_ID", "Select Location");

                    ddlLocationAM.DataSource = dt;
                    ddlLocationAM.DataTextField = "LOCATION_NAME";
                    ddlLocationAM.DataValueField = "LOCATION_ID";
                    ddlLocationAM.DataBind();
                    }
                    else if (!string.IsNullOrEmpty(locationId))
                    {
                        DataRow row = dt.Rows[0];
                        string pinByLoc = row["PINCODE"].ToString();

                        string prvPin = txtMailingPin.Text;
                        if (pinByLoc == "0" || pinByLoc == "")
                        {
                            txtMailingPin.Text = prvPin;
                        }
                        else
                        {
                            txtMailingPin.Text = pinByLoc;

                        }

                    }
                }

                else
                {
                    ddlLocationAM.Enabled = false;
                    ddlLocationAM.Items.Clear();
                    ddlLocationAM.Items.Insert(0, new ListItem("No Location", "0"));
                    txtMailingPin.Text = string.Empty;
                }

            }
            else
            {
                if (ddlLocationAM.Items.Count > 0)
                {
                    ddlLocationAM.SelectedIndex = 0;
                }
                ddlLocationAM.Enabled = false;
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

        }




        private void HandletxtMailingPinValidation(string selectedCountryName, TextBox txtMailingPin)
        {
            string selectedCountry = selectedCountryName.ToString();
            // You can set up the logic here to adjust other controls or perform actions
            if (selectedCountry.ToUpper().Contains("India".ToUpper()))
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


        public string GetTextFieldValue(DataRow dataRow, string fieldName)
        {
            try
            {
                if (dataRow.Table.Columns.Contains(fieldName))
                {
                    // Check if the field value is not null and return it, otherwise return null
                    return dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : null;
                }
                else
                {
                    //throw new ArgumentException($"Field '{fieldName}' does not exist in the DataRow.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // If the label is found, show the error message
                //lblMessage.Text = ex.Message;
                return null;
            }
        }


        public string GetDropDownValueFromDbValue(string dbValue, DropDownList dropDown)
        {
            // Check if the DropDownList has any items
            if (dropDown.Items.Count > 0)
            {
                // Try to find the item with the matching value
                ListItem item = dropDown.Items.FindByValue(dbValue);


                // If the item is found, return the dbValue as the selected value
                if (item != null)
                {
                    return item.Value;
                }
                else
                {
                    // If the item is not found, return the value of the first item (index 0)
                    return dropDown.Items[0].Value;
                }
            }
            else
            {
                // If no items exist in the dropdown, add a default item
                dropDown.Items.Add(new ListItem("No Data", "0"));

                // Return the index of the newly added item (index 0)
                return dropDown.Items[0].Value;
            }
        }


        public void SetDropDownSelectionByText(string dbValue, DropDownList dropDown)
        {
            // Check if the DropDownList has any items
            if (dropDown.Items.Count > 0)
            {
                // Try to find the item with the matching text (name)
                ListItem item = dropDown.Items.FindByText(dbValue);

                // If the item is found, set it as the selected item
                if (item != null)
                {
                    dropDown.SelectedItem.Selected = false;  // Deselect the current item
                    item.Selected = true;  // Select the found item
                }
                else
                {
                    // If the item is not found, select the first item (index 0)
                    dropDown.SelectedIndex = 0;
                }
            }
            else
            {
                // If no items exist in the dropdown, add a default item and select it
                dropDown.Items.Add(new ListItem("No Data", "0"));
                dropDown.SelectedIndex = 0;  // Select the first item (the added default item)
            }
        }


        public string SetDropDownSelectionByTextForce(DropDownList dropdown, string itemText)
        {
            if (string.IsNullOrEmpty(itemText))
                return string.Empty; // Return empty string if input is null or empty

            ListItem existingItem = dropdown.Items.FindByText(itemText);

            if (existingItem != null)
            {
                dropdown.ClearSelection();
                existingItem.Selected = true;
                return existingItem.Text; // Return the found item's text
            }
            else
            {
                dropdown.Items.Insert(1, new ListItem(itemText, itemText));
                dropdown.SelectedIndex = 1;
                return itemText; // Return the newly inserted item's text
            }
        }


        #region fillCountryList
        private void fillCountryList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetCountryList();
            ddlMailingCountryList.DataSource = dt;
            ddlMailingCountryList.DataTextField = "COUNTRY_NAME";
            ddlMailingCountryList.DataValueField = "COUNTRY_ID";
            ddlMailingCountryList.DataBind();
            ddlMailingCountryList.Items.Insert(0, new ListItem("Select Country..", ""));



        }
        #endregion


        #region fillStateList
        private void fillStateList()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetStateList();

            ddlMailingStateList.DataSource = dt;
            ddlMailingStateList.DataTextField = "STATE_NAME";
            ddlMailingStateList.DataValueField = "STATE_ID";
            ddlMailingStateList.DataBind();
            ddlMailingStateList.Items.Insert(0, new ListItem("Select State", ""));



        }
        #endregion

        #region fillCityList
        private void fillCityList()
        {
            DataTable dt1 = new WM.Controllers.AccountOpeningController().GetCityList();

            ddlMailingCityList.DataSource = dt1;
            ddlMailingCityList.DataTextField = "CITY_NAME";
            ddlMailingCityList.DataValueField = "CITY_ID";
            ddlMailingCityList.DataBind();
            ddlMailingCityList.Items.Insert(0, new ListItem("Select City", ""));




        }
        #endregion


        public void SelectDropDownItem(DropDownList ddl, string itemText)
        {
            ListItem item = ddl.Items.FindByText(itemText);
            if (ddl.Items.Count > 0)
            {

            if (item != null)
            {
                ddl.SelectedValue = item.Value;
            }
            else
            {
                ddl.SelectedIndex = 0;
            }
            }

        }

        private string GetDropDownValueIgnoreCase(string valueToCheck, DropDownList dropdown)
        {
            if (dropdown != null)
            {
                foreach (ListItem item in dropdown.Items)
                {
                    if (item.Value.Equals(valueToCheck, StringComparison.OrdinalIgnoreCase))
                    {
                        return item.Value; // Return matched value
                    }
                }
            }
            return string.Empty; // Return empty string if not found
        }

        private string GetDropDownTextIgnoreCase(string textToCheck, DropDownList dropdown)
        {
            if (dropdown != null)
            {
                foreach (ListItem item in dropdown.Items)
                {
                    if (item.Text.Equals(textToCheck, StringComparison.OrdinalIgnoreCase))
                    {
                        return item.Text; // Return matched text
                    }
                }
            }
            return string.Empty; // Return empty string if not found
        }



        public void SetFieldData(DataRow row)
        {
            FillMailingAddressCityList();

            fillCountryList();
            fillStateList();
            fillCityList();



            #region GET DB FIELD DATA 
            string DT_NUMBER_VALUE = GetTextFieldValue(row, "DT_NUMBER");
            string AGENT_CODE_VALUE = GetTextFieldValue(row, "AGENT_CODE");
            string EXIST_CODE_VALUE = GetTextFieldValue(row, "EXIST_CODE");
            string PAIDFLAG_VALUE = GetTextFieldValue(row, "PAIDFLAG");
            string AGENT_TITLE_VALUE = GetTextFieldValue(row, "TITLE");
            string AGENT_NAME_VALUE = GetTextFieldValue(row, "AGENT_NAME");
            string AGENT_GENDER_VALUE = GetTextFieldValue(row, "GENDER");

            string TIMEST_VALUE = GetTextFieldValue(row, "TIMEST");
            string SOURCEID_VALUE = GetTextFieldValue(row, "SOURCEID"); // RM BRANCH
            string RM_CODE_VALUE = GetTextFieldValue(row, "RM_CODE");
            string LOCATION_ID_VALUE = GetTextFieldValue(row, "LOCATION_ID");
            string ADDRESS1_VALUE = GetTextFieldValue(row, "ADDRESS1");
            string ADDRESS2_VALUE = GetTextFieldValue(row, "ADDRESS2");
            string ADDRESS3_VALUE = GetTextFieldValue(row, "ADDRESS3");
            string M_COUNTRY_ID_VALUE           = GetTextFieldValue(row, "M_COUNTRY_ID");
            string M_STATE_ID_VALUE             = GetTextFieldValue(row, "M_STATE_ID");
            string M_CITY_ID_VALUE              = GetTextFieldValue(row, "M_CITY_ID");
            string txtMailingPin_VALUE          = GetTextFieldValue(row, "PINCODE");
            string R_ADDRESS1_VALUE             = GetTextFieldValue(row, "RES_ADD_1");
            string R_ADDRESS2_VALUE             = GetTextFieldValue(row, "RES_ADD_2");
            string R_STATE_NAME_VALUE           = GetTextFieldValue(row, "RES_ADD_STATE");
            string R_CITY_NAME_VALUE            = GetTextFieldValue(row, "RES_ADD_CITY");
            string R_txtMailingPin_VALUE        = GetTextFieldValue(row, "RES_ADD_PIN");
            string MOBILE_VALUE = GetTextFieldValue(row, "MOBILE");
            string PHONE_VALUE = GetTextFieldValue(row, "PHONE");
            string FAX_VALUE = GetTextFieldValue(row, "FAX");
            string TDS_VALUE = GetTextFieldValue(row, "TDS");
            string EMAIL_VALUE = GetTextFieldValue(row, "EMAIL");
            string SUB_BROKER_TYPE_VALUE = GetTextFieldValue(row, "SUB_BROKER_TYPE"); // associate type
            string CATEGORY_ID_VALUE = GetTextFieldValue(row, "CATEGORY_ID");
            string CPEMAILID_VALUE = GetTextFieldValue(row, "CPEMAILID");
            string CONTACTPER_VALUE = GetTextFieldValue(row, "CONTACTPER");
            string REMARK_VALUE = GetTextFieldValue(row, "REMARK");
            string SOURCE_NAME_VALUE = GetTextFieldValue(row, "SOURCE_NAME");
            string SOURCE_VALUE = GetTextFieldValue(row, "SOURCEID");
            string MASTER_ANA_VALUE = GetTextFieldValue(row, "MASTER_ANA");
            string ONLINE_SUBSCIPTION_VALUE = GetTextFieldValue(row, "ONLINE_SUBSCIPTION");      // Y, N
            string BLOCK_AGENT_VALUE = GetTextFieldValue(row, "BLOCK_AGENT");                    // offline block check
            string OFFLINE_BLOCK_REMARK_VALUE = GetTextFieldValue(row, "OFFLINE_BLOCK_REMARK");  // offline block check remark
            string ONLINE_BLOCK_AGENT_VALUE = GetTextFieldValue(row, "ONLINE_BLOCK_AGENT");      // online block check
            string ONLINE_BLOCK_REMARK_VALUE = GetTextFieldValue(row, "ONLINE_BLOCK_REMARK");    // offline block check remark
            string ANA_AUDIT_VALUE = GetTextFieldValue(row, "ANA_AUDIT");
            string ANA_AUDITDATE_VALUE = GetTextFieldValue(row, "ANA_AUDITDATE");
            string PAYMENTMODEID_VALUE = GetTextFieldValue(row, "PAYMENTMODEID");
            string ACCTYPEID_VALUE = GetTextFieldValue(row, "ACCTYPEID");
            string ACCNO_VALUE = GetTextFieldValue(row, "ACCNO");
            string AFFECTEDFROM_VALUE = GetTextFieldValue(row, "AFFECTEDFROM");
            string BANKID_VALUE = GetTextFieldValue(row, "BANKID");
            string BANK_BRANCH_NAME_VALUE = GetTextFieldValue(row, "BANK_BRANCH_NAME");
            string CITY_NAME_VALUE = GetTextFieldValue(row, "CITY_NAME");
            string NEFT_BANK_NAME_VALUE = GetTextFieldValue(row, "NEFT_BANK_NAME");
            string IFSC_CODE_VALUE = GetTextFieldValue(row, "IFSC_CODE");
            string NAME_IN_BANK_VALUE = GetTextFieldValue(row, "NAME_IN_BANK");
            string SMS_FLAG_VALUE = GetTextFieldValue(row, "SMS_FLAG");
            string GSTIN_NO_VALUE = GetTextFieldValue(row, "GSTIN_NO");
            string DOB_VALUE = GetTextFieldValue(row, "DOB");
            string AGENT_TYPE_VALUE = GetTextFieldValue(row, "AGENT_TYPE");
            string PAN_VALUE = GetTextFieldValue(row, "PAN");
            string DIST_VALUE = GetTextFieldValue(row, "DIST");
            string POSP_MARKING_VALUE = GetTextFieldValue(row, "POSP_MARKING");
            string POSP_TYPE_VALUE = GetTextFieldValue(row, "POSP_TYPE");
            string POSP_NO_LI_VALUE = GetTextFieldValue(row, "POSP_NO_LI");
            string POSP_VALID_TILL_LI_VALUE = GetTextFieldValue(row, "POSP_VALID_TILL_LI");
            string POSP_CERTIFIED_ON_LI_VALUE = GetTextFieldValue(row, "POSP_CERTIFIED_ON_LI");
            string POSP_NO_GI_VALUE = GetTextFieldValue(row, "POSP_NO_GI");
            string POSP_CERTIFIED_ON_GI_VALUE = GetTextFieldValue(row, "POSP_CERTIFIED_ON_GI");
            string POSP_VALID_TILL_GI_VALUE = GetTextFieldValue(row, "POSP_VALID_TILL_GI");
            string AADHAR_CARD_NO_VALUE = GetTextFieldValue(row, "AADHAR_CARD_NO");
            string VERIFIED_STATUS_VALUE = GetTextFieldValue(row, "VERIFIED_STATUS");
            string AMFICERT_VALUE = GetTextFieldValue(row, "AMFICERT");
            string AMFIEXTYPEID_VALUE = GetTextFieldValue(row, "AMFIEXTYPEID");
            string AMFIID_VALUE = GetTextFieldValue(row, "AMFIID");
            string CITY_ID_VALUE = GetTextFieldValue(row, "CITY_ID");
            string BANK_BRANCHID_VALUE = GetTextFieldValue(row, "BANK_BRANCHID");
            string DOC_ID_VALUE = GetTextFieldValue(row, "DOC_ID");
            string CERTI_NAME_VALUE = GetTextFieldValue(row, "CERTI_NAME");
            #endregion


            #region SET DB DATA TO UI

            type.SelectedValue = GetDropDownValueFromDbValue(AGENT_TYPE_VALUE, type);
            txtDTNumber.Text = DT_NUMBER_VALUE;
            paymentMode.SelectedValue = GetDropDownValueFromDbValue(PAYMENTMODEID_VALUE, paymentMode);
            accountType.SelectedValue = GetDropDownValueFromDbValue(ACCTYPEID_VALUE, accountType);
            bankName.SelectedValue = GetDropDownValueFromDbValue(BANKID_VALUE, bankName);
            ddlBankCityAM.SelectedValue = GetDropDownValueFromDbValue(CITY_NAME_VALUE, ddlBankCityAM);
            //SetDropDownSelectionByText(CITY_NAME_VALUE, ddlBankCityAM);
            ddlTitle.SelectedValue = GetDropDownValueIgnoreCase(AGENT_TITLE_VALUE, ddlTitle);
            ddlGender.SelectedValue = GetDropDownValueIgnoreCase(AGENT_GENDER_VALUE, ddlGender);
            #region SELECT BANK NAME TEXT BY BANK_BRANCH_NAME_VALUE
            ListItem item = branchName.Items.FindByText(BANK_BRANCH_NAME_VALUE);
            if (item != null)
            {
                branchName.SelectedItem.Text = BANK_BRANCH_NAME_VALUE;
            }
            else
            {
                if (branchName.Items.Count > 0)
                {
                    branchName.SelectedIndex = 0;
                }
            }
            #endregion

            accountNo.Text = ACCNO_VALUE;
            neftBankName.Text = NEFT_BANK_NAME_VALUE;
            neftBranch.Text = BANK_BRANCH_NAME_VALUE;
            neftIFCSCode.Text = IFSC_CODE_VALUE;
            neftAccountNo.Text = ACCNO_VALUE;
            neftName.Text = NAME_IN_BANK_VALUE;

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


            #region ENABLE FIELD IF CHECKED

            onlinePlatformRemark.Text = ONLINE_BLOCK_REMARK_VALUE;

            offlinePlatformRemark.Text = OFFLINE_BLOCK_REMARK_VALUE;

            bool isOffineChec = (chkbOfflinePlaformBlock.Checked ? true : false);
            bool isOnlineChecked = (chkbOnlinePlaformBlock.Checked ? true : false);

          

            /*
            string BLOCK_AGENT_VALUE = GetTextFieldValue(row, "BLOCK_AGENT");                    // offline block check
            string OFFLINE_BLOCK_REMARK_VALUE = GetTextFieldValue(row, "OFFLINE_BLOCK_REMARK");  // offline block check remark
            string ONLINE_BLOCK_AGENT_VALUE = GetTextFieldValue(row, "ONLINE_BLOCK_AGENT");      // online block check
            string ONLINE_BLOCK_REMARK_VALUE = GetTextFieldValue(row, "ONLINE_BLOCK_REMARK");    // offline block check remark*/

            if (certPassedCheck.Checked)
            {
                ddlCertExam.Enabled = true;
            }
         

            if (audit.Checked)
            {
                auditDate.Enabled = true;
            }
            #endregion

            SetCheckboxValueYN(row, "VERIFIED_STATUS", verified);

            // Handle other fields
            contactPersionemailId.Text = CPEMAILID_VALUE;

            paymentMode.SelectedValue = GetDropDownValueFromDbValue(PAYMENTMODEID_VALUE, paymentMode);


            ddlAssociateType.SelectedValue = GetDropDownValueFromDbValue(SUB_BROKER_TYPE_VALUE,ddlAssociateType);
            empanelmentType.SelectedValue = GetDropDownValueFromDbValue(PAIDFLAG_VALUE, empanelmentType);
            associateName.Text = AGENT_NAME_VALUE;

            emailId.Text = EMAIL_VALUE;
            associateCode.Text = AGENT_CODE_VALUE;
            subBrokerExistCode.Text = EXIST_CODE_VALUE;
            ddlBranchAM.SelectedValue = GetDropDownValueFromDbValue(SOURCEID_VALUE,ddlBranchAM);

            //FillRMList(SOURCEID_VALUE);
            BindRmToDropdown(selectEmployee, SOURCEID_VALUE);
            selectEmployee.SelectedValue = GetDropDownValueFromDbValue(RM_CODE_VALUE, selectEmployee);

            #region RM list item by Branch

            /* DT : RM by Branch
                // Fetch RM details
                DataTable rmByRmList = new AssociateController().GetRMByRMCode(RM_CODE_VALUE);
                if (rmByRmList.Rows.Count > 0)
                {
                    DataRow RM_row = rmByRmList.Rows[0];
                    try
                    {
                        string newRMCode = RM_CODE_VALUE;
                        string newRMName = RM_row["RM_NAME"].ToString();
                        selectEmployee.Items.Insert(1, new ListItem(newRMName, newRMCode));
                        selectEmployee.SelectedIndex = 1;
                    }
                    catch
                    {
                        selectEmployee.SelectedIndex = -1;
                    }
                }*/

            #endregion

            address1.Text = ADDRESS1_VALUE;
            address2.Text = ADDRESS2_VALUE;
            address3.Text = ADDRESS3_VALUE;
            txtMailingPin.Text = txtMailingPin_VALUE;

            resAddAddress1.Text = R_ADDRESS1_VALUE;
            resAddAddress2.Text = R_ADDRESS2_VALUE;
            //ddlresAddCity.SelectedItem.Text = R_CITY_NAME_VALUE;
            SelectDropDownItem(ddlresAddCity, R_CITY_NAME_VALUE);
            resAddState.Text = R_STATE_NAME_VALUE;
            resAddPIN.Text = R_txtMailingPin_VALUE;




            mobile.Text = MOBILE_VALUE;
            fax.Text = FAX_VALUE;
            contactPerson.Text = CONTACTPER_VALUE;
            tds.Text = TDS_VALUE;
            phone.Text = PHONE_VALUE;
            remarks.Text = REMARK_VALUE;
            try
            {

                ddlAssociateTypeCategory.SelectedValue = GetDropDownValueFromDbValue(CATEGORY_ID_VALUE, ddlAssociateTypeCategory);
            }
            catch (Exception EX)
            {
                if (ddlAssociateTypeCategory.Items.Count > 0)
                {

                    ddlAssociateTypeCategory.SelectedIndex = 0;
                }
            }


            source.Text = SOURCE_NAME_VALUE;

            SelectDropdownValueIfExist(row, "MASTER_ANA", superAna);

      
            bool isChecked = SMS_FLAG_VALUE.Trim().ToUpper() == "1";
            sms.Checked = isChecked;
            gstin.Text = GSTIN_NO_VALUE;

            panGir.Text = PAN_VALUE;
            circleWardDist.Text = DIST_VALUE;

            // SetUITextField(row, "AADHAR_CARD_NO", aadharCard);
            aadharCard.Text = AADHAR_CARD_NO_VALUE;
            pospType.SelectedValue = GetDropDownValueFromDbValue(POSP_TYPE_VALUE, pospType);

            if (!string.IsNullOrEmpty(POSP_TYPE_VALUE))
            {
                pospType_Fun(POSP_TYPE_VALUE.ToUpper(), false);
            }



            aadharCard.Text = AADHAR_CARD_NO_VALUE;
            pospType.SelectedValue = GetDropDownValueFromDbValue(POSP_TYPE_VALUE, pospType);
            pospNoLi.Text = POSP_NO_LI_VALUE;
            pospNoGi.Text = POSP_NO_GI_VALUE;
            SetDropDownValueYN(row, "POSP_MARKING", pospMarking);
            document.SelectedValue = GetDropDownValueFromDbValue(DOC_ID_VALUE, document);

            #endregion

            #region Mailing Country, State, City Filter after set

            string currentDB_MCountryID = M_COUNTRY_ID_VALUE;
            string currentDB_MStateID = M_STATE_ID_VALUE;
            string currentDB_MCityID = M_CITY_ID_VALUE;

            try
            {
                if (ddlMailingCountryList.Items.FindByValue(currentDB_MCountryID) != null)
                {
                    try
                    {
                        // Populate the state dropdown based on the selected country
                        ddlMailingCountryList.SelectedValue = currentDB_MCountryID;


                        PopulateStateDropDownForAddress(Convert.ToInt32(currentDB_MCountryID), ddlMailingStateList);

                        if (ddlMailingStateList.Items.FindByValue(currentDB_MStateID) != null)
                        {
                            ddlMailingStateList.Enabled = true;
                            try
                            {
                                // Set the state dropdown selected value
                                ddlMailingStateList.SelectedValue = M_STATE_ID_VALUE;

                                // Populate the city dropdown based on the selected state
                                PopulateCityDropDownForAddress(Convert.ToInt32(currentDB_MStateID), ddlMailingCityList);

                                if (ddlMailingCityList.Items.FindByValue(currentDB_MCityID) != null)
                                {
                                    ddlMailingCityList.Enabled = true;
                                    try
                                    {
                                        // Set the city dropdown selected value
                                        ddlMailingCityList.SelectedValue = M_CITY_ID_VALUE;
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


            if (!string.IsNullOrEmpty(currentDB_MCityID))
            {
            ddlMailingCityListFillLocation(currentDB_MCityID, null, null);
                if (!string.IsNullOrEmpty(LOCATION_ID_VALUE))
                {
                ddlLocationAM.SelectedValue = GetDropDownValueFromDbValue(LOCATION_ID_VALUE, ddlLocationAM);
                    ddlLocationAM.Enabled = true;   
                }

            }

            if (!string.IsNullOrEmpty(txtMailingPin_VALUE))
            {
                txtMailingPin.Text = txtMailingPin_VALUE;
            }


            #endregion

            #region Handle pin code by country
            if (ddlMailingCountryList.Items.Count > 0)
            {
                string currentMailingCoutnryName = ddlMailingCountryList.SelectedItem.ToString();

                HandletxtMailingPinValidation(currentMailingCoutnryName, txtMailingPin);
                try
                {
                    txtMailingPin.Text = txtMailingPin_VALUE;
                }
                catch (Exception ex)
                {


                }
            }

            #endregion

 
          

            #region HANDLE CERTIFICATE SECTION 
            string certiYN = !string.IsNullOrEmpty(AMFICERT_VALUE) ? AMFICERT_VALUE.Trim().ToUpper() : null;
            string certiNameID = !string.IsNullOrEmpty(AMFIEXTYPEID_VALUE) ? AMFIEXTYPEID_VALUE.Trim() : null;
            string certiNumber = !string.IsNullOrEmpty(AMFIID_VALUE) ? AMFIID_VALUE.Trim() : null;

            if (certiYN == "Y")
            {
                certEnrolledCheck.Checked = true;
                ddlCertExam.Enabled = true;
                certPassedCheck.Enabled = true;


                if (certiNameID != null)
                {
                    try
                    {
                        ddlCertExam.SelectedValue = certiNameID;
                    } catch (Exception ex)
                    {
                        ddlCertExam.SelectedIndex = 0;
                    }
                }


                if (certiNumber != null)
                {
                    certPassedCheck.Checked = true;
                    certRegNo.Enabled = true;
                    certRegNo.Text = certiNumber;
                }



            }

            #endregion


            if (!string.IsNullOrEmpty(associateCode.Text))
            {
                saveButton.Enabled = false;
                upateButton.Enabled = true;

            }


            #region Role based control buttons

            //if (currentRoleId == "212")
            //{
            //    ddlBranchAM.Enabled = false;
            //    selectEmployee.Enabled = false;

            //    upateButton.Enabled = true;
            //    saveButton.Enabled = false;
            //}
            //else
            //{
            //    upateButton.Enabled = false;
            //    saveButton.Enabled = false;
            //}
            #endregion
        }

        // Helper method to set dropdown values if they exist
        private void SetDropdownValueIfExist(DropDownList ddl, string value)
        {
            if (ddl.Items.FindByValue(value) != null)
                ddl.SelectedValue = value;
            else
                ddl.SelectedIndex = -1;
        }



        private void FillTitle()
        {
            DataTable dt = new WM.Controllers.AssociateController().GetSalutationList();

            // Clear the dropdown list first
            ddlTitle.Items.Clear();
            if (dt != null && dt.Rows.Count > 0)
            {
                ddlTitle.DataSource = dt;
                ddlTitle.DataTextField = "Text";
                ddlTitle.DataValueField = "Value";
                ddlTitle.DataBind();
                ddlTitle.Items.Insert(0, new ListItem("Select", ""));
            }
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

                int dtRowCount = dt.Rows.Count;

                if (dtRowCount > 0)
                {
                    DataRow dataRow = dt.Rows[0];
                    ResetFields();
                    SetFieldData(dataRow);
                }

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
                DataTable dt = new AssociateController().GetClientDataByDTNumber(id);
                int dtRowCount = dt.Rows.Count;

                if (dtRowCount > 0)
                {
                    DataRow row = dt.Rows[0];
                    string message = GetTextFieldValue(row, "message");

                    bool isInvalid = message.ToUpper().Contains("INVALID DATA") ? true : false;
                    bool isValid = message.ToUpper().Contains("DATA FOUND") ? true : false;

                    ResetFields();
                    if (isValid)
                    {

                        #region DATA GET SET FOR VALID DT NUMBER
                        string branchCode = GetTextFieldValue(row, "tb_branch_code");
                        string roCode = GetTextFieldValue(row, "tb_rm_code");
                        string currentDt = GetTextFieldValue(row, "tb_common_id");
                        string rejectionStatus = GetTextFieldValue(row, "tb_reject");
                        string punchingFlag = GetTextFieldValue(row, "tb_punch");
                        string verificationFlag = GetTextFieldValue(row, "tb_verify");
                        string tranType = GetTextFieldValue(row, "tb_tr_type");
                        string existCode = GetTextFieldValue(row, "tb_exist_code");
                        string amExistCode = GetTextFieldValue(row, "am_exist_code");
                        string amAgentCode = GetTextFieldValue(row, "am_AGENT_code");

                        txtDTNumber.Text = currentDt;
                        ddlBranchAM.SelectedValue = branchCode;
                        BindRmToDropdown(selectEmployee, branchCode);
                        selectEmployee.SelectedValue = GetDropDownValueFromDbValue(roCode, selectEmployee);
                        string selectedSourceID = ddlBranchAM.SelectedValue;
                        if (selectedSourceID != "Select Branch" || selectedSourceID != "")
                        {
                            source.Text = ddlBranchAM.SelectedItem.Text;
                        }
                        else
                        {
                            source.Text = "";
                        }
                        if (ddlMailingCountryList.SelectedValue == "1")
                        {
                            if (ddlMailingStateList.Items.Count > 0)
                            {
                                string currentState = "102"; // delhi
                                ddlMailingStateListFillCity(currentState);
                                ddlMailingStateList.SelectedValue = currentState;
                                ddlMailingStateList.Enabled = true;
                                ddlMailingStateListFillCity(currentState);

                                string nn = ddlMailingStateList.SelectedItem.Text;
                            }

                        }

                        txtDTNumber.Focus();

                        /* auto fill                        

                        if (amAgentCode.Trim() == amAgentCode.Trim() && amAgentCode != "0")
                        {
                            FillAssociateDataByAgentCode(amAgentCode);
                            return;
                        }

                        else
                        {
                            txtDTNumber.Text = currentDt;
                            //lblMessage.Text = message;
                            //lblMessage.CssClass = "text-info";
                            ddlBranchAM.SelectedValue = branchCode;
                            
                            BindRmToDropdown(selectEmployee, branchCode);
                            selectEmployee.SelectedValue = roCode;

                            

                            //FillLocationListByCityBranch(branchCode);
                            string selectedSourceID = ddlBranchAM.SelectedValue;

                            if (selectedSourceID != "Select Branch" || selectedSourceID != "")
                            {
                                source.Text = ddlBranchAM.SelectedItem.Text;
                            }
                            else
                            {
                                source.Text = "";
                            }

                        } */

#endregion
                    }
                    
                    else
                    {
                        BindRmToDropdown(selectEmployee, null);

                        ResetFields();
                        ShowAlert(message);
                        txtDTNumber.Text = id;
                    }

                }
                else
                {

                    ResetFields();
                    string alertMsg = "No Data Found";
                    ShowAlert(alertMsg);
                    txtDTNumber.Text = id;
                }

            }
            catch (Exception ex)
            {

            }
        }

        // Function to bind RM data to DropDownList
        public void BindRmToDropdown(DropDownList selectEmployee, string sourceId)
        {
            DataTable dt_rm = new WM.Controllers.AssociateController().GetRmByBranchDTList(sourceId);

            if (dt_rm != null && dt_rm.Rows.Count > 0)
            {
                selectEmployee.DataSource = dt_rm;
                selectEmployee.DataTextField = "rm_name"; 
                selectEmployee.DataValueField = "payroll_id";  
                selectEmployee.DataBind();
            }
            else
            {
                selectEmployee.Items.Clear();
                selectEmployee.Items.Insert(0, new ListItem("Select", ""));
            }
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
                FillClientDataByDTNumber(commonID);
            }

        }



        protected void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                /*

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

                //// Get the text from the multiline TextBox
                //string[] addressMiltiline = address1.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                //// Initialize variables for each line
                //string addressLine1 = addressMiltiline.Length > 0 ? addressMiltiline[0] : string.Empty;
                //string addressLine2 = addressMiltiline.Length > 1 ? addressMiltiline[1] : string.Empty;
                //string addressLine3 = addressMiltiline.Length > 2 ? addressMiltiline[2] : string.Empty;

                string address1Value = address1.Text;
                string address2Value = address2.Text;

                string ddlCityAMValue = ddlMailingCityList.SelectedValue;
                string ddlLocationAMValue = ddlLocationAM.SelectedValue;
                long mobileNumberValue = long.TryParse(mobile.Text, out long mobileNumber) ? mobileNumber : 0;
                long txtMailingPinValue = long.TryParse(txtMailingPin.Text, out long pin) ? pin : 0;

                string resAddAddress1Value = !string.IsNullOrEmpty(resAddAddress1.Text) ? resAddAddress1.Text : string.Empty;
                string resAddAddress2Value = !string.IsNullOrEmpty(resAddAddress2.Text) ? resAddAddress2.Text : string.Empty;
                string resAddStateValue = !string.IsNullOrEmpty(resAddState.Text) ? resAddState.Text : string.Empty;
                string ddlresAddCityValue = !string.IsNullOrEmpty(ddlresAddCity.Text) ? ddlresAddCity.Text : string.Empty;
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

                if (!ValidateReqField(txtDTNumber.Text,                 "Enter DT number", txtDTNumber)) return;
                if (!ValidateReqField(empanelmentType.SelectedValue,    "Empanelment Type", empanelmentType)) return;
                if (!ValidateReqField(associateName.Text,               "Enter agent name", associateName)) return;
                if (!ValidateReqField(ddlBranchAM.SelectedValue,        "Select branch", ddlBranchAM)) return;
                if (!ValidateReqField(ddlMailingCityList.SelectedValue, "Select city", ddlMailingCityList)) return;
                if (!ValidateReqField(mobile.Text, "Enter mobile", mobile)) return;
                if (!ValidateReqField(emailId.Text, "Enter email", emailId)) return;
                if (!ValidateReqField(ddlAssociateTypeCategory.SelectedValue, "Select associate category", ddlAssociateTypeCategory)) return;
                if (!ValidateReqField(ddlAssociateType.SelectedValue, "Select associate type", ddlAssociateType)) return;
                if (!ValidateReqField(contactPerson.Text, "Enter contact person ", contactPerson)) return;
                


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

                */



                #region Parsing date fields 
                DateTime? auditDateValue = ParseDate(auditDate.Text);
                DateTime? empanelmentDateValue = ParseDate(empanelmentDate.Text);
                DateTime? affectedFromValue = ParseDate(affectedFrom.Text);
                DateTime? dobValue = ParseDate(dob.Text);
                DateTime? pospCertifiedOnLiValue = ParseDate(pospCertifiedLiOn.Text);
                DateTime? pospValidTillLiValue = ParseDate(pospCertifiedLiOnValidTill.Text);
                DateTime? pospCertifiedOnGiValue = ParseDate(pospCertifiedGiOn.Text);
                DateTime? pospValidTillGiValue = ParseDate(pospCertifiedGiOnValidTill.Text);

                #endregion

                #region Getting all UI stirng Data
                string empanelmentTypeValue = GetSetUiDropdownValue(empanelmentType);
                string associateCodeValue = GetSetUiStringValue(associateCode.Text);
                string subBrokerExistCodeValue = GetSetUiStringValue(subBrokerExistCode.Text);
                string associateTitleValue = GetSetUiStringValue(ddlTitle.SelectedValue);
                string associateNameValue = GetSetUiStringValue(associateName.Text);
                string associateGenderValue = GetSetUiStringValue(ddlGender.SelectedValue);
                string ddlBranchAMValue = GetSetUiDropdownValue(ddlBranchAM);
                string selectEmployeeValue = GetSetUiDropdownValue(selectEmployee);
                string address1Value = GetSetUiStringValue(address1.Text);
                string address2Value = GetSetUiStringValue(address2.Text);
                string address3Value = GetSetUiStringValue(address3.Text);

                string ddlCityAMValue = GetSetUiDropdownValue(ddlMailingCityList);
                string ddlLocationAMValue = GetSetUiDropdownValue(ddlLocationAM);
                long mobileNumberValue = GetSetUiLongValue(mobile);
                string txtMailingPinValue = GetSetUiStringValue(txtMailingPin.Text);
                string resAddAddress1Value = GetSetUiStringValue(resAddAddress1.Text);
                string resAddAddress2Value = GetSetUiStringValue(resAddAddress2.Text);
                string resAddStateValue = GetSetUiStringValue(resAddState.Text);
                string ddlresAddCityValue = (
                   ddlresAddCity.SelectedValue.ToString() != "" ? ddlresAddCity.SelectedItem.Text : string.Empty
               );
                string resAddPINValue = GetSetUiStringValue(resAddPIN.Text);
                string faxValue = GetSetUiStringValue(fax.Text);
                string contactPersonValue = GetSetUiStringValue(contactPerson.Text);
                string emailIdValue = GetSetUiStringValue(emailId.Text);
                decimal tdsValue = GetSetUiDecimalValue(tds.Text);
                string associateTypeValue = GetSetUiDropdownValue(ddlAssociateType);
                string associateCategoryValue = GetSetUiDropdownValue(ddlAssociateTypeCategory);
                string contactPersionEmailValue = GetSetUiStringValue(contactPersionemailId.Text);
                string phoneValue = GetSetUiStringValue(phone.Text);
                string remarksValue = GetSetUiStringValue(remarks.Text);
                string superAnaValue = GetSetUiDropdownValue(superAna);
                string onlinePlatformValue = GetSetUiCheckBoxValueZO(chkbOnlinePlaformBlock);
                string offlinePlatformValue = GetSetUiCheckBoxValueZO(chkbOfflinePlaformBlock);
                string onlinePlatformRemarkValue = GetSetUiStringValue(onlinePlatformRemark.Text);
                string offlinePlatformRemarkValue = GetSetUiStringValue(offlinePlatformRemark.Text);
                string onlineSubscriptionValue = GetSetUiCheckBoxValueYN(onlineSubscription);
                string auditValue = GetSetUiCheckBoxValueYN(audit);
                int paymentModeIdValue = GetSetUiIntValue(GetSetUiDropdownValue(paymentMode));
                string accountTypeValue = GetSetUiDropdownValue(accountType);
                string accountNoValue = GetSetUiStringValue(accountNo.Text);
                string bankNameValue = GetSetUiDropdownValue(bankName);
                string ddlBankCityAMValue = GetSetUiDropdownValue(ddlBankCityAM);
                string branchNameValue = (branchName.SelectedIndex == 0 ? "": branchName.SelectedItem.Text);

                string smsValue = GetSetUiCheckBoxValueZO(sms);
                string gstinValue = GetSetUiStringValue(gstin.Text);
                string typeValue = GetSetUiDropdownValue(type);
                string panGirValue = GetSetUiStringValue(panGir.Text);
                string circleWardDistValue = GetSetUiStringValue(circleWardDist.Text);
                string aadharCardValue = GetSetUiStringValue(aadharCard.Text);
                string pospMarkingValue = GetSetUiDropdownValue(pospMarking);
                string pospTypeValue = GetSetUiStringValue(pospType.Text);
                string pospNoLiValue = GetSetUiStringValue(pospNoLi.Text);
                string pospNoGiValue = GetSetUiStringValue(pospNoGi.Text);
                string verifiedValue = GetSetUiCheckBoxValueYN(verified);
                string neftBankNameValue = GetSetUiStringValue(neftBankName.Text);
                string neftBranchValue = GetSetUiStringValue(neftBranch.Text);
                string neftIFSCCodeValue = GetSetUiStringValue(neftIFCSCode.Text);
                string neftNameValue = GetSetUiStringValue(neftBankName.Text);
                string certPassedValue = GetSetUiCheckBoxValueYN(certPassedCheck);
                string certExamsValue = GetSetUiDropdownValue(ddlCertExam);
                string certRegValue = GetSetUiStringValue(certRegNo.Text);
                string loggedInUser = Session["LoginId"]?.ToString();
                string dtNumberValue = GetSetUiStringValue(txtDTNumber.Text);

                #endregion

                #region Input Requried Validation
                lblMessage.Text = "";
                if (!ValidateReqField(txtDTNumber.Text, "DT number is required", txtDTNumber)) return;
                if (!ValidateReqField(empanelmentType.SelectedValue, "Empanelment type is required", empanelmentType)) return;
                if (!ValidateReqField(ddlTitle.SelectedValue, "Agent title is required", ddlTitle)) return;

                if (!ValidateReqField(associateName.Text, "Associate Name is required", associateName)) return;
                if (!ValidateReqField(ddlBranchAM.SelectedValue, "Associate branch is required", ddlBranchAM)) return;
                if (!ValidateReqField(ddlMailingCityList.SelectedValue, "Associate city is required", ddlMailingCityList)) return;
                if (!ValidateReqField(mobile.Text, "Mobile Number is required", mobile)) return;
                if (!ValidateReqField(emailId.Text, "Email ID is required", emailId)) return;
                if (!ValidateReqField(ddlAssociateType.SelectedValue, "Associate type is required", ddlAssociateType)) return;
                //if (!ValidateReqField(ddlAssociateTypeCategory.SelectedValue, "Associate category is required", ddlAssociateTypeCategory)) return;
                if (!ValidateReqField(contactPerson.Text, "Contact person name is required", contactPerson)) return;
                if (!ValidateReqField(dob.Text, "DOB is required", dob)) return;
                if (!ValidateReqField(panGir.Text, "PAN is required", panGir)) return;
                if (!ValidateReqField(ddlGender.SelectedValue, "Agent gender is required", ddlGender)) return;

                if (!IsAllZeros(aadharCard.Text)) {
                    ShowAlert("Invalid Aadhaar Number (Must be 12-digit numeric)");
                    aadharCard.Focus();
                    return;
                }
                if (!ValidateReqFieldIfChecked(chkbOnlinePlaformBlock, onlinePlatformRemark.Text, "Please enter a remark for the online platform block", onlinePlatformRemark)) { onlinePlatformRemark.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(chkbOfflinePlaformBlock, offlinePlatformRemark.Text, "Please enter a remark for the online platform block", offlinePlatformRemark)) { offlinePlatformRemark.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(audit, auditDate.Text, "If audit, then audit date is requried", auditDate)) { auditDate.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(certEnrolledCheck, ddlCertExam.SelectedValue, "Choose enrolled exam name", ddlCertExam)) { ddlCertExam.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(certPassedCheck, certRegNo.Text, "If passed then enter registration number", certRegNo)) { certRegNo.Enabled = true; return; }

                #endregion

                else
                {

                    #region DO AGENT INSERTION

                    string isInserted = new WM.Controllers.AssociateController().InsertAgentMaster(
                            loggedInUser,
                            empanelmentTypeValue,
                            associateCodeValue,
                            subBrokerExistCodeValue,
                            associateTitleValue,
                            associateNameValue,
                            associateGenderValue,
                            ddlBranchAMValue,
                            selectEmployeeValue,
                            address1Value,
                            address2Value,
                            address3Value,

                            ddlCityAMValue,
                            ddlLocationAMValue,
                            mobileNumberValue,
                            txtMailingPinValue,
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
                            ddlresAddCityValue,
                            resAddPINValue,
                      dtNumberValue

                  );

                    #endregion


                    bool isAccess = isInserted.ToUpper().Contains("Access".ToUpper());
                    bool isInvalid = isInserted.ToUpper().Contains("Invalid".ToUpper());
                    bool isDuplicate = isInserted.ToUpper().Contains("Duplicate".ToUpper());
                    bool isSuccess = isInserted.ToUpper().Contains("Successful".ToUpper());



                    if (isAccess)
                    {
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isInserted);
                    }

                    else if (isInvalid)
                    {
                        bool isInvalidMOB = isInserted.ToUpper().Contains("MOBILE".ToUpper());
                        bool isInvalidEMAIL = isInserted.ToUpper().Contains("EMAIL".ToUpper());
                        bool isInvalidPAN = isInserted.ToUpper().Contains("PAN".ToUpper());
                        bool isInvalidAadhar = isInserted.ToUpper().Contains("AADHAR".ToUpper());



                        if (isInvalidMOB)
                        {
                            mobile.Focus();
                        }
                        else if (isInvalidEMAIL)
                        {
                            emailId.Focus();
                        }
                        else if (isInvalidPAN)
                        {
                            panGir.Focus();
                        }
                        else if (isInvalidAadhar)
                        {
                            aadharCard.Focus();
                        }

                        lblMessage.Text = isInserted;
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isInserted);
                    }

                    else if (isDuplicate)
                    {
                        bool isDupMOB = isInserted.ToUpper().Contains("MOBILE".ToUpper());
                        bool isDupEMAIL = isInserted.ToUpper().Contains("EMAIL".ToUpper());
                        bool isDupPAN = isInserted.ToUpper().Contains("PAN".ToUpper());
                        bool isDupAadhar = isInserted.ToUpper().Contains("AADHAR".ToUpper());



                        if (isDupMOB)
                        {
                            mobile.Focus();
                        }
                        else if (isDupEMAIL)
                        {
                            emailId.Focus();
                        }
                        else if (isDupPAN)
                        {
                            panGir.Focus();
                        }
                        else if (isDupAadhar)
                        {
                            aadharCard.Focus();
                        }

                        lblMessage.Text = isInserted;
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isInserted);
                    }

                    else if (isSuccess)
                    {
                        lblMessage.Text = isInserted;
                        string generatedAgentID = GetInsertedAgentID(isInserted);
                        FillAssociateDataByAgentCode(generatedAgentID);
                        lblMessage.CssClass = "text-success";
                        ShowAlert(isInserted);
                    }

                    else
                    {
                        lblMessage.Text = isInserted;
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isInserted);
                    }

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


        public string GetInsertedAgentID(string responseMessage)
        {
            if (!string.IsNullOrEmpty(responseMessage) && responseMessage.Contains("AGENT_CODE:"))
            {
                // Extract the numeric part after "AGENT_CODE:"
                string[] parts = responseMessage.Split(new string[] { "AGENT_CODE:" }, StringSplitOptions.None);
                if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out int agentId))
                {
                    return agentId.ToString(); // Successfully extracted AGENT_CODE
                }
            }
            return string.Empty; // Return 0 if extraction fails
        }


        public static string GetSetUiStringValue(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Trim() : string.Empty;
        }


        public static decimal GetSetUiDecimalValue(string input)
        {
            return decimal.TryParse(input, out decimal parsedValue) ? parsedValue : 0;
        }


        public static string GetSetUiDropdownValue(DropDownList dropdown)
        {
            return dropdown != null && !string.IsNullOrEmpty(dropdown.SelectedValue) ? dropdown.SelectedValue : string.Empty;
        }


        public static string GetSetUiDropdownText(DropDownList dropdown)
        {
            return dropdown != null && !string.IsNullOrEmpty(dropdown.SelectedItem.Text) ? dropdown.SelectedItem.Text : string.Empty;
        }


        public static long GetSetUiLongValue(TextBox textBox)
        {
            return textBox != null && long.TryParse(textBox.Text, out long parsedValue) ? parsedValue : 0;
        }

        public static string GetSetUiCheckBoxValueZO(CheckBox checkBox)
        {
            return checkBox != null && checkBox.Checked ? "1" : "0";
        }

        public static string GetSetUiCheckBoxValueYN(CheckBox checkBox)
        {
            return checkBox != null && checkBox.Checked ? "Y" : "N";
        }

        public static int GetSetUiIntValue(string input)
        {
            return int.TryParse(input, out int result) ? result : 0;
        }

        public bool IsAllZeros(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(input) & input.Length < 12)
            {
                return false;
            }
            return !input.All(c => c == '0'); // Returns true if all characters are '0'
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                #region Parsing date fields 
                DateTime? auditDateValue = ParseDate(auditDate.Text);
                DateTime? empanelmentDateValue = ParseDate(empanelmentDate.Text);
                DateTime? affectedFromValue = ParseDate(affectedFrom.Text);
                DateTime? dobValue = ParseDate(dob.Text);
                DateTime? pospCertifiedOnLiValue = ParseDate(pospCertifiedLiOn.Text);
                DateTime? pospValidTillLiValue = ParseDate(pospCertifiedLiOnValidTill.Text);
                DateTime? pospCertifiedOnGiValue = ParseDate(pospCertifiedGiOn.Text);
                DateTime? pospValidTillGiValue = ParseDate(pospCertifiedGiOnValidTill.Text);

                #endregion

                #region Getting all UI stirng Data
                string empanelmentTypeValue = GetSetUiDropdownValue(empanelmentType);
                string associateCodeValue = GetSetUiStringValue(associateCode.Text);
                string subBrokerExistCodeValue = GetSetUiStringValue(subBrokerExistCode.Text);
                string associateTitleValue = GetSetUiStringValue(ddlTitle.SelectedValue);
                string associateNameValue = GetSetUiStringValue(associateName.Text);
                string associateGenderValue = GetSetUiStringValue(ddlGender.SelectedValue);

                string ddlBranchAMValue = GetSetUiDropdownValue(ddlBranchAM);
                string selectEmployeeValue = GetSetUiDropdownValue(selectEmployee);
                string address1Value = GetSetUiStringValue(address1.Text);
                string address2Value = GetSetUiStringValue(address2.Text);
                string address3Value = GetSetUiStringValue(address3.Text);

                string ddlCityAMValue = GetSetUiDropdownValue(ddlMailingCityList);
                string ddlLocationAMValue = GetSetUiDropdownValue(ddlLocationAM);
                long mobileNumberValue = GetSetUiLongValue(mobile);
                string txtMailingPinValue = GetSetUiStringValue(txtMailingPin.Text);
                string resAddAddress1Value = GetSetUiStringValue(resAddAddress1.Text);
                string resAddAddress2Value = GetSetUiStringValue(resAddAddress2.Text);
                string resAddStateValue = GetSetUiStringValue(resAddState.Text);

                string currRecityID = ddlresAddCity.SelectedValue;
                string currReccityname = ddlresAddCity.SelectedItem.Text;
                string ddlresAddCityValue = (
                    ddlresAddCity.SelectedValue.ToString() != ""  ? ddlresAddCity.SelectedItem.Text : string.Empty
                );
                string resAddPINValue = GetSetUiStringValue(resAddPIN.Text);
                string faxValue = GetSetUiStringValue(fax.Text);
                string contactPersonValue = GetSetUiStringValue(contactPerson.Text);
                string emailIdValue = GetSetUiStringValue(emailId.Text);
                decimal tdsValue = GetSetUiDecimalValue(tds.Text);
                string associateTypeValue = GetSetUiDropdownValue(ddlAssociateType);
                string associateCategoryValue = GetSetUiDropdownValue(ddlAssociateTypeCategory);
                string contactPersionEmailValue = GetSetUiStringValue(contactPersionemailId.Text);
                string phoneValue = GetSetUiStringValue(phone.Text);
                string remarksValue = GetSetUiStringValue(remarks.Text);
                string superAnaValue = GetSetUiDropdownValue(superAna);
                string onlinePlatformValue = GetSetUiCheckBoxValueZO(chkbOnlinePlaformBlock);
                string offlinePlatformValue = GetSetUiCheckBoxValueZO(chkbOfflinePlaformBlock);
                string onlinePlatformRemarkValue = GetSetUiStringValue(onlinePlatformRemark.Text);
                string offlinePlatformRemarkValue = GetSetUiStringValue(offlinePlatformRemark.Text);
                string onlineSubscriptionValue = GetSetUiCheckBoxValueYN(onlineSubscription);
                string auditValue = GetSetUiCheckBoxValueYN(audit);
                int paymentModeIdValue = GetSetUiIntValue(GetSetUiDropdownValue(paymentMode));
                string accountTypeValue = GetSetUiDropdownValue(accountType);
                string accountNoValue = GetSetUiStringValue(accountNo.Text);
                string bankNameValue = GetSetUiDropdownValue(bankName);
                string ddlBankCityAMValue = GetSetUiDropdownValue(ddlBankCityAM);

                string branchNameValue = branchName.SelectedItem.Text;
                string branchNameValue2 = branchName.SelectedValue;

                string smsValue = GetSetUiCheckBoxValueZO(sms);
                string gstinValue = GetSetUiStringValue(gstin.Text);
                string typeValue = GetSetUiDropdownValue(type);
                string panGirValue = GetSetUiStringValue(panGir.Text);
                string circleWardDistValue = GetSetUiStringValue(circleWardDist.Text);
                string aadharCardValue = GetSetUiStringValue(aadharCard.Text);
                string pospMarkingValue = GetSetUiDropdownValue(pospMarking);
                string pospTypeValue = GetSetUiStringValue(pospType.Text);
                string pospNoLiValue = GetSetUiStringValue(pospNoLi.Text);
                string pospNoGiValue = GetSetUiStringValue(pospNoGi.Text);
                string verifiedValue = GetSetUiCheckBoxValueYN(verified);
                string neftBankNameValue = GetSetUiStringValue(neftBankName.Text);
                string neftBranchValue = GetSetUiStringValue(neftBranch.Text);
                string neftIFSCCodeValue = GetSetUiStringValue(neftIFCSCode.Text);
                string neftNameValue = GetSetUiStringValue(neftBankName.Text);
                string certPassedValue = GetSetUiCheckBoxValueYN(certPassedCheck);
                string certExamsValue = GetSetUiDropdownValue(ddlCertExam);
                string certRegValue = GetSetUiStringValue(certRegNo.Text);
                string loggedInUser = Session["LoginId"]?.ToString();
                string dtNumberValue = GetSetUiStringValue(txtDTNumber.Text);

                #endregion

                #region Input Requried Validation
                // if (!ValidateReqField(txtDTNumber.Text, "DT number is is required",             txtDTNumber)) return;
                if (!ValidateReqField(empanelmentType.SelectedValue, "Empanelment type is required", empanelmentType)) return;
                if (!ValidateReqField(ddlTitle.SelectedValue, "Agent title is required", ddlTitle)) return;
                if (!ValidateReqField(ddlGender.SelectedValue, "Agent gender is required", ddlGender)) return;

                if (!ValidateReqField(associateName.Text, "Associate Name is required", associateName)) return;
                if (!ValidateReqField(ddlBranchAM.SelectedValue, "Associate branch is required", ddlBranchAM)) return;
                if (!ValidateReqField(ddlMailingCityList.SelectedValue, "Associate city is required", ddlMailingCityList)) return;
                if (!ValidateReqField(mobile.Text, "Mobile Number is required", mobile)) return;
                if (!ValidateReqField(emailId.Text, "Email ID is required", emailId)) return;
                if (!ValidateReqField(ddlAssociateType.SelectedValue, "Associate type is required", ddlAssociateType)) return;
                //if (!ValidateReqField(ddlAssociateTypeCategory.SelectedValue, "Associate category is required", ddlAssociateTypeCategory)) return;
                if (!ValidateReqField(contactPerson.Text, "Contact person name is required", contactPerson)) return;
                if (!ValidateReqField(dob.Text, "DOB is required", dob)) return;
                if (!ValidateReqField(panGir.Text, "PAN is required", panGir)) return;
                if (!ValidateReqField(ddlGender.SelectedValue, "Agent gender is required", ddlGender)) return;



                if (!IsAllZeros(aadharCard.Text))
                {
                    ShowAlert("Invalid Aadhaar Number (Must be 12-digit numeric)");
                    aadharCard.Focus();
                    return;
                }
                if (!ValidateReqFieldIfChecked(chkbOnlinePlaformBlock, onlinePlatformRemark.Text, "Please enter a remark for the online platform block", onlinePlatformRemark)) { onlinePlatformRemark.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(chkbOfflinePlaformBlock, offlinePlatformRemark.Text, "Please enter a remark for the online platform block", offlinePlatformRemark)) { offlinePlatformRemark.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(audit, auditDate.Text, "If audit, then audit date is requried", auditDate)) { auditDate.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(certEnrolledCheck, ddlCertExam.SelectedValue, "Choose enrolled exam name", ddlCertExam)) { ddlCertExam.Enabled = true; return; }
                if (!ValidateReqFieldIfChecked(certPassedCheck, certRegNo.Text, "If passed then enter registration number", certRegNo)) { certRegNo.Enabled = true; return; }

                #endregion

                

                else
                {
                    string isUpdated = new WM.Controllers.AssociateController().UpdateAgentMaster(
                        loggedInUser,
                        empanelmentTypeValue,
                        associateCodeValue,
                        subBrokerExistCodeValue,
                        associateTitleValue,
                        associateNameValue,
                        associateGenderValue,
                        ddlBranchAMValue,
                        selectEmployeeValue,
                        address1Value,
                        address2Value,
                        address3Value,
                        ddlCityAMValue,
                        ddlLocationAMValue,
                        mobileNumberValue,
                        txtMailingPinValue,
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
                        ddlresAddCityValue,
                        resAddPINValue,
                        dtNumberValue
                    );


                    bool isAccess = isUpdated.ToUpper().Contains("Access".ToUpper());
                    bool isInvalid = isUpdated.ToUpper().Contains("Invalid".ToUpper());
                    bool isDuplicate = isUpdated.ToUpper().Contains("Duplicate".ToUpper());
                    bool isSuccess = isUpdated.ToUpper().Contains("Successful".ToUpper());



                    if (isAccess)
                    {
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isUpdated);
                    }

                    else if (isInvalid)
                    {
                        bool isInvalidMOB = isUpdated.ToUpper().Contains("MOBILE".ToUpper());
                        bool isInvalidEMAIL = isUpdated.ToUpper().Contains("EMAIL".ToUpper());
                        bool isInvalidPAN = isUpdated.ToUpper().Contains("PAN".ToUpper());
                        bool isInvalidAadhar = isUpdated.ToUpper().Contains("AADHAR".ToUpper());



                        if (isInvalidMOB)
                        {
                            mobile.Focus();
                        }
                        else if (isInvalidEMAIL)
                        {
                            emailId.Focus();
                        }
                        else if (isInvalidPAN)
                        {
                            panGir.Focus();
                        }
                        else if (isInvalidAadhar)
                        {
                            aadharCard.Focus();
                        }

                        lblMessage.Text = isUpdated;
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isUpdated);
                    }

                    else if (isDuplicate)
                    {
                        bool isDupMOB = isUpdated.ToUpper().Contains("MOBILE".ToUpper());
                        bool isDupEMAIL = isUpdated.ToUpper().Contains("EMAIL".ToUpper());
                        bool isDupPAN = isUpdated.ToUpper().Contains("PAN".ToUpper());
                        bool isDupAadhar = isUpdated.ToUpper().Contains("AADHAR".ToUpper());



                        if (isDupMOB)
                        {
                            mobile.Focus();
                        }
                        else if (isDupEMAIL)
                        {
                            emailId.Focus();
                        }
                        else if (isDupPAN)
                        {
                            panGir.Focus();
                        }
                        else if (isDupAadhar)
                        {
                            aadharCard.Focus();
                        }

                        lblMessage.Text = isUpdated;
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isUpdated);
                    }

                    else if (isSuccess)
                    {
                        lblMessage.Text = isUpdated;
                        FillAssociateDataByAgentCode(associateCodeValue);
                        lblMessage.CssClass = "text-success";
                        ShowAlert(isUpdated);
                    }

                    else
                    {
                        lblMessage.Text = isUpdated;
                        lblMessage.CssClass = "text-warning";
                        ShowAlert(isUpdated);
                    }

                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblMessage.CssClass = "text-danger";
            }
        }

        private bool ValidateReqField(string value, string alertMessage, Control controlToFocus)
        {
            if (string.IsNullOrEmpty(value.Trim()))
            {
                ShowAlert(alertMessage);
                controlToFocus.Focus();
                return false;
            }
            return true;
        }

        private bool ValidateReqFieldIfChecked(CheckBox checkBoxID, string reqruiedField, string alertMessage, Control controlToFocus)
        {

            if (checkBoxID.Checked && string.IsNullOrWhiteSpace(reqruiedField.Trim()))
            {
                ShowAlert(alertMessage);
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
        private void FillBranchList(string logId)
        {
            string roleID = Session["RoleID"] != null ? Session["RoleID"].ToString() : null;
            DataTable dt = new WM.Controllers.AssociateController().GetBranchMasterList(logId, roleID, null, null);

            int dtCount = dt.Rows.Count;
            if (dtCount > 0)
            {
                #region ON LOAD BRANCH FIELD
                ddlBranchAM.Items.Clear();
                ddlBranchAM.DataSource = dt;
                ddlBranchAM.DataTextField = "BRANCH_NAME";   // Text field for dropdown display
                ddlBranchAM.DataValueField = "BRANCH_CODE";  // Value field for dropdown value
                ddlBranchAM.DataBind();
                ddlBranchAM.Items.Insert(0, new ListItem("Select Branch", ""));
                #endregion

                #region ASSOCIATE SEARCH BRANCH
                branchsbl.DataSource = dt;
                branchsbl.DataTextField = "BRANCH_NAME";
                branchsbl.DataValueField = "BRANCH_CODE";
                branchsbl.DataBind();
                branchsbl.Items.Insert(0, new ListItem("Select Branch", ""));
                #endregion

                #region SUPER ANA SEARCH BRANCH
                ddlSourceID.DataSource = dt;
                ddlSourceID.DataTextField = "BRANCH_NAME";
                ddlSourceID.DataValueField = "BRANCH_CODE";
                ddlSourceID.DataBind();
                ddlSourceID.Items.Insert(0, new ListItem("Select Branch", ""));
                #endregion
            }
        }








        protected void ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSourceID = ddlBranchAM.SelectedValue;

            FillRMList(selectedSourceID);
            // BindRmToDropdown(selectEmployee, selectedSourceID);
            if (selectEmployee.Items.Count > 1)
            {
                selectEmployee.Focus();
            }

            if (selectedSourceID != "Select Branch" || selectedSourceID != "")
            {
                source.Text = ddlBranchAM.SelectedItem.Text;
            }
            else
            {
                source.Text = "";
            }
        }

        private void FillRMList(string sourceID)
        {
            if (sourceID == null)
            {
                selectEmployee.Enabled = false;
                selectEmployee.Items.Clear();
                selectEmployee.Items.Insert(0, new ListItem("Select", ""));
                selectEmployee.SelectedIndex = 0;

            }
            else
            {

                DataTable dt = new WM.Controllers.AssociateController().GetRMListBySourceBranch(sourceID);
                int dtRowsCount = dt.Rows.Count;
                if (dtRowsCount > 0)
                {
                    DataRow dtRows = dt.Rows[0];
                    selectEmployee.DataSource = dt;
                    selectEmployee.DataTextField = "AGENT_NAME";
                    selectEmployee.DataValueField = "RM_CODE";
                    selectEmployee.DataBind();
                    selectEmployee.Items.Insert(0, new ListItem("Select", ""));
                }
                else
                {
                    selectEmployee.Items.Clear();
                    selectEmployee.Items.Insert(0, new ListItem("Select", ""));
                    selectEmployee.SelectedIndex = 0;

                }
            }

        }

        private void FillCityMainList_0()
        {
            DataTable dt1 = new DataTable();

            dt1 = new WM.Controllers.AssociateController().GetCitylist();

            ddlMailingCityList.DataSource = dt1;
            ddlMailingCityList.DataTextField = "CITY_NAME";
            ddlMailingCityList.DataValueField = "CITY_ID";
            ddlMailingCityList.DataBind();
            ddlMailingCityList.Items.Insert(0, new ListItem("Select", ""));

        }


        private void FillMailingAddressCityList()
        {
            DataTable dt1 = new DataTable();

            dt1 = new WM.Controllers.AssociateController().GetCitylist();

            ddlMailingCityList.DataSource = dt1;
            ddlMailingCityList.DataTextField = "CITY_NAME";
            ddlMailingCityList.DataValueField = "CITY_ID";
            ddlMailingCityList.DataBind();
            ddlMailingCityList.Items.Insert(0, new ListItem("Select", ""));

        }

        private void AssociateSearchAllCityList()
        {
            DataTable dt2 = new DataTable();
            dt2 = new WM.Controllers.AssociateController().GetCitylist();

            citysbl.DataSource = dt2;
            citysbl.DataTextField = "CITY_NAME";
            citysbl.DataValueField = "CITY_ID";
            citysbl.DataBind();
            citysbl.Items.Insert(0, new ListItem("Select", ""));


            ddlresAddCity.DataSource = dt2;
            ddlresAddCity.DataTextField = "CITY_NAME";
            ddlresAddCity.DataValueField = "CITY_ID";
            ddlresAddCity.DataBind();
            ddlresAddCity.Items.Insert(0, new ListItem("Select", ""));

            ddlBankCityAM.DataSource = dt2;
            ddlBankCityAM.DataTextField = "CITY_NAME";
            ddlBankCityAM.DataValueField = "CITY_ID";
            ddlBankCityAM.DataBind();
            ddlBankCityAM.Items.Insert(0, new ListItem("Select", ""));

            
        }


        private void FilAllCityList()
        {
            DataTable dt2 = new DataTable();
            dt2 = new WM.Controllers.AssociateController().GetCitylist();

            citysbl.DataSource = dt2;
            citysbl.DataTextField = "CITY_NAME";
            citysbl.DataValueField = "CITY_ID";
            citysbl.DataBind();
            citysbl.Items.Insert(0, new ListItem("Select", ""));


            ddlBankCityAM.DataSource = dt2;
            ddlBankCityAM.DataTextField = "CITY_NAME";
            ddlBankCityAM.DataValueField = "CITY_ID";
            ddlBankCityAM.DataBind();
            ddlBankCityAM.Items.Insert(0, new ListItem("Select", ""));
        }


        private void FillLocationListByCityBranch(string p_city, string p_pin, string p_loc)
        {
            // Fetch data using all three parameters
            DataTable dt = new AssociateController().GetLocationList(p_city, p_loc, p_pin);
            int dtRowsCount = dt.Rows.Count;

            if (dtRowsCount > 0)
            {
                ddlLocationAM.Enabled = true;
                AddDefaultItem(dt, "LOCATION_NAME", "LOCATION_ID", "Select Location");

                ddlLocationAM.DataSource = dt;
                ddlLocationAM.DataTextField = "LOCATION_NAME";
                ddlLocationAM.DataValueField = "LOCATION_ID";
                ddlLocationAM.DataBind();
            }
            else
            {
                ddlLocationAM.Enabled = false;
                ddlLocationAM.Items.Clear();
                ddlLocationAM.Items.Insert(0, new ListItem("No Location", "0"));
            }
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
            DataTable dt = new AssociateController().GetAgentMasterDetails(Branches());
            superAna.DataSource = dt;
            superAna.DataTextField = "AGENT_NAME";
            superAna.DataValueField = "AGENT_CODE";
            superAna.DataBind();
            superAna.Items.Insert(0, new ListItem("Select", ""));



        }

        protected void RM_ddlSourceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected SourceID
            string selectedSourceID = ddlSourceID.SelectedValue;
            string logId = Session["LoginId"]?.ToString();

            //fillRMList(selectedSourceID);

            DataTable dt = new WM.Controllers.AgentController().SuperANASearchRMBySrc(logId, selectedSourceID, null);
            int dtRowCount = dt.Rows.Count;

            if (dtRowCount > 0)
            {
                ddlRM.Enabled = true;
                ddlRM.DataSource = dt;
                ddlRM.DataTextField = "RM_NAME";
                ddlRM.DataValueField = "RM_CODE";
                ddlRM.DataBind();
                ddlRM.Items.Insert(0, new ListItem("Select", ""));
            }
            else
            {
                ddlRM.Enabled = false;
                ddlRM.DataSource = null;
                ddlRM.DataBind();
                ddlRM.Items.Insert(0, new ListItem("No RM", ""));


            }

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
            bool isAdded = false;
            int existItem = 0;
            int uniqueItem = 0;

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

                    int totalCount = superAna.Items.Count;

                    if (!ifExist)
                    {
                        superAna.Items.Insert(0, new ListItem(agentName, agentCode));
                        superAna.Focus();
                        superAna.SelectedIndex = 0;
                        isAdded = true;
                        uniqueItem += 1;
                    }
                    else
                    {
                        existItem += 1;
                    }
                }
            }

            if (isAdded && uniqueItem > 0)
            {
                ShowAlert("Selected agent added successfully.");
            }

            if (!isAdded && existItem > 0)
            {
                ShowAlert("Selected agent already added.");
            }

            // If no item is selected, alert the user
            if (!itemSelected)
            {
                ShowAlert("Select any agent to add in Super ANA list");
            }

            ListItem selectItem = superAna.Items.FindByText("Select");
            if (selectItem != null)
            {
                superAna.Items.Remove(selectItem);
            }


            superAna.Items.Insert(0, new ListItem("Select", "0"));
            superAna.SelectedIndex = 1;


            //else
            //{
            //    // Close the modal if at least one item was added
            //    ScriptManager.RegisterStartupScript(this, GetType(), "btnCloseSuperAnaModal", "closeSuperAnaFindModal();", true);
            //}
        }

        // ExitSuperANAModelFields

        protected void ExitSuperANAModelFields(object sender, EventArgs e)
        {

            // call this with client side script manager "closeSuperAnaFindModal(); return false;" in htis fuciotn 
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeSuperAnaFindModal();", true);

            ResetSuperANAModelFun();
        }

        protected void ResetSuperANAModelFields(object sender, EventArgs e)
        {

            ResetSuperANAModelFun();
        }

        protected void ResetSuperANAModelFun()
        {
            // Reset the DropDownList

            if (ddlSourceID.Items.Count > 0)
            {
                ddlSourceID.SelectedIndex = 0;
            }
            if (ddlRM.Items.Count > 0)
            {
                ddlRM.SelectedIndex = 0;
                ddlRM.Enabled = false;

            }

            if (agentsGrid.Rows.Count > 0)
            {
                // Optionally, you may want to clear the GridView data if necessary
                agentsGrid.DataSource = null; // Clear the GridView data source
                agentsGrid.DataBind(); // Rebind the GridView to reflect the cleared data
            }
            btnSetAsMainAgent.Enabled = false;

            // Reset the TextBox controls
            txtAgentName.Text = string.Empty;
            txtExistCode.Text = string.Empty;

            m_sas_txtAgentCode.Text = string.Empty;
            m_sas_txtMobile.Text = string.Empty;
            m_sas_txtPhone.Text = string.Empty;
            m_sas_txtPan.Text = string.Empty;



            lblSuperAnamsg.Text = string.Empty;

        }
        private void FillBankBranchData()
        {
            DataTable dt2 = new DataTable();
            dt2 = new WM.Controllers.AssociateController().GET_BANK_BRANCH_Only();

            branchName.DataSource = dt2;
            branchName.DataTextField = "BRANCH_NAME";
            branchName.DataValueField = "BRANCH_NAME"; // NotFiniteNumberException exist in datatale 
            branchName.DataBind();
            branchName.Items.Insert(0, new ListItem("Select Bank Branch", ""));

        }

        private void FillBankMasterData()
        {
            DataTable dt1 = new DataTable();
            dt1 = new WM.Controllers.AssociateController().GetBankMasterDetails(null, null);
            //dt1 = new WM.Controllers.AssociateController().GET_BANK_BRANCH_NEW();
            bankName.DataSource = dt1;
            bankName.DataTextField = "BANK_NAME";
            bankName.DataValueField = "BANK_ID";
            bankName.DataBind();
            bankName.Items.Insert(0, new ListItem("Select Bank", ""));

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
            Response.Redirect(Request.RawUrl);

            //RemoveStidFromUrl();
            //ResetFields();
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
            resetDropdown(ddlMailingStateList);


            txtDTNumber.Text = string.Empty;
            resAddAddress1.Text = string.Empty;
            resAddAddress2.Text = string.Empty;
            resAddState.Text = string.Empty;
            ddlresAddCity.Text = string.Empty;
            resAddPIN.Text = string.Empty;

            // Reset date fields
            auditDate.Text = string.Empty;
            empanelmentDate.Text = string.Empty;
            affectedFrom.Text = string.Empty;
            dob.Text = string.Empty;


            pospCertifiedLiOn.Text = string.Empty;
            pospCertifiedLiOnValidTill.Text = string.Empty;
            pospCertifiedGiOn.Text = string.Empty;
            pospCertifiedGiOnValidTill.Text = string.Empty;

            // Reset dropdowns
            resetDropdown(empanelmentType);
            resetDropdown(ddlMailingStateList);
            resetDropdown(ddlBranchAM);
            resetDropdown(selectEmployee);
            resetDropdown(ddlMailingCityList);
            resetDropdown(ddlLocationAM);
            resetDropdown(ddlAssociateTypeCategory);

            associateCode.Text = string.Empty;
            subBrokerExistCode.Text = string.Empty;
            associateName.Text = string.Empty;
            address1.Text = string.Empty;
            address2.Text = string.Empty;
            address3.Text = string.Empty;


            mobile.Text = string.Empty;
            txtMailingPin.Text = string.Empty;

            fax.Text = string.Empty;
            contactPerson.Text = string.Empty;
            emailId.Text = string.Empty;
            tds.Text = string.Empty;
            resetDropdown(ddlAssociateType);


            contactPersionemailId.Text = string.Empty;
            phone.Text = string.Empty;
            remarks.Text = string.Empty;
            source.Text = "";
            onlinePlatformRemark.Text = string.Empty;
            offlinePlatformRemark.Text = string.Empty;

            resetDropdown(superAna);
            chkbOnlinePlaformBlock.Checked = false;
            chkbOfflinePlaformBlock.Checked = false;
            onlineSubscription.Checked = false;
            audit.Checked = false;

            paymentMode.SelectedIndex = 0;
            accountType.SelectedIndex = 0;
            accountNo.Text = string.Empty;

            resetDropdown(bankName);
            resetDropdown(ddlBankCityAM);
            resetDropdown(branchName);

            sms.Checked = false;
            gstin.Text = string.Empty;
            resetDropdown(type);
            panGir.Text = string.Empty;
            circleWardDist.Text = string.Empty;
            aadharCard.Text = string.Empty;

            resetDropdown(pospMarking);
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

            resetDropdown(ddlCertExam);

            resetDropdown(ddlCertExam);
            certRegNo.Text = string.Empty;
            lblMessage.Text = string.Empty;
        }


        private void resetDropdown(DropDownList dropDownList)
        {
            if (dropDownList.Items.Count > 0)
            {
                dropDownList.SelectedIndex = 0;
            }
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
            string AGENT_CODE = string.IsNullOrEmpty(agentCodesbl.Text.Trim()) ? null : agentCodesbl.Text.Trim();



            // Check if at least one field has a value
            return !string.IsNullOrEmpty(BRANCH_ID) ||
                   !string.IsNullOrEmpty(CITY_ID) ||
                   !string.IsNullOrEmpty(ASSOCIATE_CODE) ||
                   !string.IsNullOrEmpty(NAME) ||
                   !string.IsNullOrEmpty(MOBILE) ||
                   !string.IsNullOrEmpty(PHONE) ||
                   !string.IsNullOrEmpty(AGENT_CODE) ||


                   !string.IsNullOrEmpty(PAN_NO);
        }



        protected void btnAssListSearchsbl_Click(object sender, EventArgs e)
        {
            string BRANCH_ID = branchsbl.SelectedValue == "0" ? null : branchsbl.SelectedValue;
            string CITY_ID = citysbl.SelectedValue == "0" ? null : citysbl.SelectedValue;
            string ASSOCIATE_CODE = codesbl.Text.Trim(); // EXIST CODE
            string AGENT_CODE = agentCodesbl.Text.Trim();


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

            // Store search parameters in ViewState for pagination
            ViewState["BRANCH_ID"] = BRANCH_ID;
            ViewState["CITY_ID"] = CITY_ID;
            ViewState["ASSOCIATE_CODE"] = ASSOCIATE_CODE;
            ViewState["AGENT_CODE"] = AGENT_CODE;

            ViewState["NAME"] = NAME;
            ViewState["MOBILE"] = MOBILE;
            ViewState["PHONE"] = PHONE;
            ViewState["PAN_NO"] = PAN_NO;

            // Reset the page index to 0 when performing a new search
            agentListDetailsGridsbl.PageIndex = 0;

            BindGridForAgentSearchPaging(); // Call BindGrid with stored parameters
        }

        private void BindGridForAgentSearchPaging()
        {
            string BRANCH_ID = ViewState["BRANCH_ID"] as string;
            string CITY_ID = ViewState["CITY_ID"] as string;
            string ASSOCIATE_CODE = ViewState["ASSOCIATE_CODE"] as string;
            string AGENT_CODE = ViewState["AGENT_CODE"] as string;



            string NAME = ViewState["NAME"] as string;
            string MOBILE = ViewState["MOBILE"] as string;
            string PHONE = ViewState["PHONE"] as string;
            string PAN_NO = ViewState["PAN_NO"] as string;

            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            string currentBranched = Session["loggedAngBranches"].ToString();// Branches();

            DataTable dt = new AssociateController().GetAssociateListByCriteria(BRANCH_ID, CITY_ID, MOBILE, PHONE, ASSOCIATE_CODE, AGENT_CODE, PAN_NO, NAME, currentBranched);

            if (dt.Rows.Count > 0)
            {

                lblAssociateListMessage.Text = "Assocaite Searched: <span style=\"color:black;\"> Total " + dt.Rows.Count + (dt.Rows.Count == 1 ? " record found!" : " records found!</span>");
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


        protected void agentListDetailsGridsbl_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            agentListDetailsGridsbl.PageIndex = e.NewPageIndex;
            BindGridForAgentSearchPaging(); // Rebind data using stored search parameters
        }



        // Function to check if at least one search field is filled
        private bool IsSuperANASearchCriteriaFilled()
        {

            int? SOURCEID = ddlSourceID.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlSourceID.SelectedValue, out int sourceId) ? (int?)sourceId : null);
            int? RM_CODE = ddlRM.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlRM.SelectedValue, out int rmCode) ? (int?)rmCode : null);
            string AGENT_NAME = string.IsNullOrEmpty(txtAgentName.Text) ? null : txtAgentName.Text.Trim();
            string EXIST_CODE = string.IsNullOrEmpty(txtExistCode.Text) ? null : txtExistCode.Text.Trim();


            string AGENT_CODE = string.IsNullOrEmpty(m_sas_txtAgentCode.Text) ? null : m_sas_txtAgentCode.Text.Trim();
            string MOBILE_NO = string.IsNullOrEmpty(m_sas_txtMobile.Text) ? null : m_sas_txtMobile.Text.Trim();
            string PHONE_NO = string.IsNullOrEmpty(m_sas_txtPhone.Text) ? null : m_sas_txtPhone.Text.Trim();
            string PAN_CARD = string.IsNullOrEmpty(m_sas_txtPan.Text) ? null : m_sas_txtPan.Text.Trim();

            // Check if any of the fields have data
            if (
                SOURCEID != null ||
                RM_CODE != null ||
                AGENT_NAME != null ||
                EXIST_CODE != null ||
                AGENT_CODE != null ||
                MOBILE_NO != null ||
                PHONE_NO != null ||
                PAN_CARD != null
                
                )
            {
                return true;
            }
            return false;
        }

        private int? GetNullabledRopInt(DropDownList ddl)
        {
            return ddl.SelectedValue == "0" ? (int?)null :
                   (int.TryParse(ddl.SelectedValue, out int result) ? (int?)result : null);
        }


        protected void btnSearch_Click_SuperAna(object sender, EventArgs e)
        {
            // First, validate if at least one search field is filled
            if (!IsSuperANASearchCriteriaFilled())
            {
                string emptyFieldMsg = "Please fill at least one search field";
                if (agentsGrid.Rows.Count > 0)
                {
                    agentsGrid.DataSource = null;
                    agentsGrid.DataBind();
                }
                ShowAlert(emptyFieldMsg);
                lblSuperAnamsg.Text = emptyFieldMsg;

                return;
            }
            int? SOURCEID = ddlSourceID.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlSourceID.SelectedValue, out int sourceId) ? (int?)sourceId : null);
            int? RM_CODE = ddlRM.SelectedValue == "0" ? (int?)null : (int.TryParse(ddlRM.SelectedValue, out int rmCode) ? (int?)rmCode : null);
            string AGENT_NAME = string.IsNullOrEmpty(txtAgentName.Text) ? null : txtAgentName.Text.Trim();
            string EXIST_CODE = string.IsNullOrEmpty(txtExistCode.Text) ? null : txtExistCode.Text.Trim();

            string AGENT_CODE = string.IsNullOrEmpty(m_sas_txtAgentCode.Text) ? null : m_sas_txtAgentCode.Text.Trim();
            string MOBILE_NO  = string.IsNullOrEmpty(m_sas_txtMobile.Text) ? null : m_sas_txtMobile.Text.Trim();
            string PHONE_NO   = string.IsNullOrEmpty(m_sas_txtPhone.Text) ? null : m_sas_txtPhone.Text.Trim();
            string PAN_CARD   = string.IsNullOrEmpty(m_sas_txtPan.Text) ? null : m_sas_txtPan.Text.Trim();


            // Store search parameters in ViewState for pagination
            ViewState["VS_SAS_SOURCEID"] = SOURCEID;
            ViewState["VS_SAS_RM_CODE"] = RM_CODE;
            ViewState["VS_SAS_AGENT_NAME"] = AGENT_NAME;
            ViewState["VS_SAS_EXIST_CODE"] = EXIST_CODE;

            ViewState["VS_SAS_AGENT_CODE"] = AGENT_CODE;
            ViewState["VS_SAS_MOBILE_NO"] =  MOBILE_NO  ;
            ViewState["VS_SAS_PHONE_NO"] =   PHONE_NO   ;
            ViewState["VS_SAS_PAN_CARD"] =   PAN_CARD   ;


            // Reset the page index to 0 when performing a new search
            agentsGrid.PageIndex = 0;

            // Call BindGrid with stored parameters
            BindGridForSuperANASearchPaging();
        }


        private void BindGridForSuperANASearchPaging()
        {
            int? SOURCEID = ViewState["VS_SAS_SOURCEID"] as int?;
            int? RM_CODE = ViewState["VS_SAS_RM_CODE"] as int?;
            string AGENT_NAME = ViewState["VS_SAS_AGENT_NAME"] as string;
            string EXIST_CODE = ViewState["VS_SAS_EXIST_CODE"] as string;

            string AGENT_CODE = ViewState["VS_SAS_AGENT_CODE"] as string;
            string MOBILE_NO = ViewState["VS_SAS_MOBILE_NO"] as string;
            string PHONE_NO = ViewState["VS_SAS_PHONE_NO"] as string;
            string PAN_CARD = ViewState["VS_SAS_PAN_CARD"] as string;
            string currentBranches = Session["loggedAngBranches"].ToString(); //Branches();

            // Get the filtered data for the current page
            DataTable dt = new WM.Controllers.AgentController().GetFilteredAgentsData(SOURCEID, RM_CODE, AGENT_NAME, EXIST_CODE,MOBILE_NO,PHONE_NO,AGENT_CODE,PAN_CARD, currentBranches);

            // Get the total rows count for pagination
            int totalRowsCount = dt.Rows.Count;

            // Handle data binding and GridView visibility
            if (totalRowsCount > 0)
            {
                lblSuperAnamsg.Text = $"Agents Searched: <span style=\"color:black;\"> Total {totalRowsCount} Record{(totalRowsCount == 1 ? "" : "s")} found </span>";

                agentsGrid.Visible = true;
                agentsGrid.DataSource = dt;
                agentsGrid.DataBind();

                btnSetAsMainAgent.Enabled = true;

                // Set pagination controls
                agentsGrid.PageSize = 20;  // Set the page size to 20
            }
            else
            {
                ResetAssociateListGrid();
                lblSuperAnamsg.Text = "No records found!";
                agentsGrid.Visible = false;
                btnSetAsMainAgent.Enabled = false;

            }
        }

        protected void superANAAgentSearchListDetailsGridsbl_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Update the page index
            agentsGrid.PageIndex = e.NewPageIndex;

            // Re-bind the data with the updated page index
            BindGridForSuperANASearchPaging();
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
            pospType_Fun(selectedPospType.ToUpper(), true);



        }



        protected void pospType_Fun(string selectedPospType, bool focousMark)
        {
           
            
            // Enable/Disable fields based on the selected value of pospType
            if (selectedPospType == "LIFE")
            {
                EnableLifeFields();
                DisableGeneralFields();
                if (focousMark)
                {

                pospNoLi.Focus();
                }
            }
            else if (selectedPospType == "GENERAL")
            {
                EnableGeneralFields();
                DisableLifeFields();
                if (focousMark)
                {

                pospNoGi.Focus();
                }
            }
            else if (selectedPospType == "BOTH")
            {
                EnableLifeFields();
                EnableGeneralFields();
                if (focousMark)
                {

                pospNoLi.Focus();
                }
            }
            else
            {
                DisableAllFields();
                if (focousMark)
                {

                pospType.Focus();
                }
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

        protected void closeResAddModal(object sender, EventArgs e)
        {
            // Execute client-side script to close the modal
            ScriptManager.RegisterStartupScript(this, GetType(), "closeResAddModal", "closeResAddModal();", true);
        }


    }
}