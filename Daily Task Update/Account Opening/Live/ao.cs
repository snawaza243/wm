
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media.TextFormatting;
using WM.Controllers;
using System.Text.RegularExpressions;
using System.Configuration;
using MathNet.Numerics.Distributions;
using System.Net.NetworkInformation;
using System.Windows.Input;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Services.Description;
using static NPOI.HSSF.Util.HSSFColor;
using System.Reflection;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Drawing;


namespace WM.Masters
{
    public partial class AccountOpening : Page
    {
        protected string currentLoginId;
        protected string currentRoleId;
        protected string tempRole = "261"; // Temporary role


        string genSourceID;
        string soruceID;

        PsmController psm_controller = new PsmController();
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
                    //EnableDisablField(false);
                    currentRoleId = "212";
                    if (currentRoleId == "212")
                    {
                        btnSave.Enabled = true;
                        btnUpdate.Enabled = true;
                        btnApprove.Enabled = true;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = false;
                        btnApprove.Enabled = false;

                    }

                    FILL_AC_CATEGORY();
                    PopulateInvestorBranchDropDown();
                    FillFamilyRelationList();
                    FillSalutationListDropDown();
                    fillOccupationList();
                    fillClientCategoryList();
                    fillCountryList();
                    fillStateList();
                    fillCityList();
                    ddlMailingStateList.Enabled = false;
                    ddlMailingCityList.Enabled = false;
                    ddlPStateList.Enabled = false;
                    ddlPCityList.Enabled = false;

                    fillBankMasterDetails();
                    ClientListFillCityList();
                    ClientListFillBranchList();
                    FillMutualFundDropdown();

                    AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                    AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                    AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                    AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");

                    /*
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
                    }*/


                    #region State city enable disable 
                    if (ddlPStateList.Items.Count > 1)
                    {
                        ddlPStateList.Enabled = true;

                    }else
                    {
                        ddlPStateList.Enabled = false;
                    }

                    if (ddlPCityList.Items.Count > 1)
                    {
                        ddlPCityList.Enabled = true;

                    }
                    else
                    {
                        ddlPCityList.Enabled = false;
                    }
                    #endregion

                    ddladdfamExistingInvestor.Enabled = false;

                    #region Test inline query
                    DataTable SQL_RESULT = CurrentSql("SELECT * FROM BRANCH_MASTER");
                    string currentSqlException = CurrentSqlExceptionMsg(SQL_RESULT);

                    #endregion


                   

                    #region BIND MULTI MEMBERS GRID

                    if (string.IsNullOrEmpty(ExistingClientCodeInv.Text))
                    {

                        BindInitialGrid(null);
                    }
                    else
                    {
                        #region Adding existing investor in inline grid new

                        int validRowCount = GetValidRowCount(ngfd_gvClients);

                        int validRowIndex = 0; // To count valid rows
                        int rowIndex = 1; // UI row number (starting from 1 for display)
                        if (validRowCount > 0)
                        {
                            foreach (GridViewRow row in ngfd_gvClients.Rows)
                            {
                                DropDownList ddlExistingInvestorNew = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                                TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                                if (ddlExistingInvestorNew != null)
                                {
                                    validRowIndex++; // Increment valid row count
                                    if (ddlExistingInvestorNew.Items.Count > 1 && !string.IsNullOrEmpty(txtInvestorCode.Text))
                                    {
                                        ddlExistingInvestorNew.SelectedValue = txtInvestorCode.Text;
                                        ddlExistingInvestorNew.Enabled = false;
                                    }
                                    else
                                    {
                                        ddlExistingInvestorNew.Enabled = false;
                                        //ddlExistingInvestorNew.Items.Insert(0, new ListItem("INVESTOR NOT EXIST", ""));
                                    }

                                    // Stop checking after reaching the valid row count
                                    if (validRowIndex >= validRowCount) break;
                                }
                                rowIndex++; // Increment the display row index
                            }
                        }
                        #endregion
                    }

                    #endregion


                    if (string.IsNullOrEmpty(ExistingClientCodeInv.Text))
                    {
                        btnSave.Enabled = true;
                        btnUpdate.Enabled = false;
                    }
                    else
                    {
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = true;                        
                    }

                    if (string.IsNullOrEmpty(txtSearchClientCode.Text))
                    {
                        btnApprove.Enabled = false;
                    }
                    else
                    {
                        btnApprove.Enabled = true;
                    }


                   


                    ON_LOAD_ADDRESS_SET();

                    string IS_AC_INV_FOUND = Session["AC_INV_FIND_INV"] as string;
                    string IS_AC_AH_FOUND = Session["AC_CL_FIND_AH"] as string;

                    if (!string.IsNullOrEmpty(IS_AC_INV_FOUND))
                    {
                        HandleSelectedInvestor(IS_AC_INV_FOUND);
                        Session["AC_INV_FIND_INV"] = null;
                        Session["AC_INV_FIND"] = true;


                    }
                    else if (!string.IsNullOrEmpty(IS_AC_AH_FOUND))
                    {
                        FillClientDataByAHNum(IS_AC_AH_FOUND);
                        Session["AC_CL_FIND_AH"] = null;
                        Session["AC_CL_FIND"] = false;
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

        protected void btnSearchClient(object sender, EventArgs e)
        {
            // Store a session variable
            Session["AC_FIND"] = "CLIENT";
            Response.Redirect("../Tree/frm_tree_mf");

        }

        protected void btnSearchInvestor(object sender, EventArgs e)
        {
            // Store a session variable
            Session["AC_FIND"] = "INVESTOR";
            Response.Redirect("../Tree/frm_tree_mf");
        }


        public void ON_LOAD_ADDRESS_SET()
        {
            #region ONLOAD SET MAIILNG PERMANENT ADDRESS

            if (ddlMailingCountryList.Items.Count > 1)
            {
                ddlMailingCountryList.SelectedValue = "1";
                HANDLE_MAILING_SCP_BY_C();
            }

            if (ddlMailingStateList.Items.Count > 1)
            {
                ddlMailingStateList.SelectedValue = "102";
                HANDLE_MAILING_SCP_BY_STATE();

            }

            if (ddlMailingCityList.Items.Count > 1)
            {
                ddlMailingCityList.SelectedValue = "C0914";
                txtAccountName.Focus();
            }

            /*
            if (ddlPCountryList.Items.Count > 1)
            {
                ddlPCountryList.SelectedValue = "1";
                HANDLE_PERMANENT_SCP_BY_COUNTRY();
            }

            if (ddlPStateList.Items.Count > 1)
            {
                ddlPStateList.SelectedValue = "102";
                HANDLE_PERMANENT_SCP_BY_STATE();

            }

            if (ddlPCityList.Items.Count > 1)
            {
                ddlPCityList.SelectedValue = "C0914";
                txtAccountName.Focus();
            }
            */
            #endregion

        }
        #region Add new family synamic grid
        private void BindInitialGrid(DataTable currenFam_dt = null, int totalRows = 20)
        {

            DataTable dt = new DataTable();

            // Define all columns for GridView
            dt.Columns.Add("ExistingInvestor", typeof(string));
            dt.Columns.Add("InvestorCode", typeof(string));
            dt.Columns.Add("Salutation", typeof(string));
            dt.Columns.Add("InvestorName", typeof(string));
            dt.Columns.Add("Mobile", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("PAN", typeof(string));
            dt.Columns.Add("Aadhar", typeof(string));
            dt.Columns.Add("DOB", typeof(string));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("GuardianPAN", typeof(string));
            dt.Columns.Add("GuardianName", typeof(string));
            dt.Columns.Add("Occupation", typeof(string));
            dt.Columns.Add("Relation", typeof(string));
            dt.Columns.Add("KYC", typeof(string));
            dt.Columns.Add("Approved", typeof(string));
            dt.Columns.Add("IsNominee", typeof(string));
            dt.Columns.Add("Allocation", typeof(string));

             

            if (currenFam_dt != null && currenFam_dt.Rows.Count > 0)
            {
                // Add existing data first
                foreach (DataRow row in currenFam_dt.Rows)
                {
                    //dt.ImportRow(row);

                    {
                        DataRow newRow = dt.NewRow();

                        #region FETCHED DB DATA

                        string inv_code = GetTextFieldValue(row, "InvestorCode");
                        string inv_title = GetTextFieldValue(row, "investor_title");
                        string inv_name = GetTextFieldValue(row, "investor_name");
                        string inv_gender = GetTextFieldValue(row, "gender");
                        string inv_mobile = GetTextFieldValue(row, "mobile");
                        string inv_dob = GetTextFieldValue(row, "dob");
                        string inv_email = GetTextFieldValue(row, "email");
                        string inv_pan = GetTextFieldValue(row, "pan");
                        string inv_rel = GetTextFieldValue(row, "OUR_RELATIONSHIP_ID");//(OUR_RELATIONSHIP, OUR_RELATIONSHIP_ID)
                        string inv_kyc = GetTextFieldValue(row, "KYC");
                        string inv_g_name = GetTextFieldValue(row, "g_name");
                        string inv_g_pan = GetTextFieldValue(row, "g_pan");
                        string inv_occ_id = GetTextFieldValue(row, "occ_id"); //(occ_id, occ_name)
                        string inv_approved = GetTextFieldValue(row, "approved");
                        string inv_AADHAR_CARD_NO = GetTextFieldValue(row, "AADHAR_CARD_NO");
                        string inv_is_nominee = GetTextFieldValue(row, "is_nominee");
                        string inv_nominee_per = GetTextFieldValue(row, "nominee_per");
                        #endregion

                        newRow["ExistingInvestor"] =  inv_code;
                        newRow["InvestorCode"]          = inv_code;
                        newRow["Salutation"] = ReturnedMatchedDropValue(row,"investor_title",ddlSalutation3); // A DROPDOWN
                        newRow["InvestorName"]          = inv_name;
                        newRow["Mobile"]                = inv_mobile;
                        newRow["Email"]                 = inv_email;
                        newRow["PAN"]                   = inv_pan;
                        newRow["Aadhar"]                = inv_AADHAR_CARD_NO;
                        newRow["DOB"]                   = ReturnDateString(inv_dob);
                        newRow["Gender"]                = inv_gender;
                        newRow["GuardianPAN"]           = inv_g_pan;
                        newRow["GuardianName"]          = inv_g_name;
                        newRow["Occupation"]            = (inv_occ_id=="0"? "": inv_occ_id);                       // A DROPDOWN
                        newRow["Relation"]              = (inv_rel == "0" ? "" : inv_rel); // A DROPDOWN
                        newRow["KYC"]                   = ReturnYESNOByYN(inv_kyc);         // A DROPDOWN
                        newRow["Approved"]              = ReturnYESNOByYN(inv_approved);    // A DROPDOWN
                        newRow["IsNominee"]             = ReturnYESNOByYN(inv_is_nominee);  // A DROPDOWN
                        newRow["Allocation"]            = inv_nominee_per;



                        dt.Rows.Add(newRow);
                    }
                }

                
                // Calculate remaining empty rows
                int remainingRows = totalRows - dt.Rows.Count;
                for (int i = 0; i < remainingRows; i++)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }
            else
            {
                // If no data, ensure 250 empty rows
                for (int i = 0; i < (totalRows); i++)
                {
                    dt.Rows.Add("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                }
            }

            // Bind to GridView
            ngfd_gvClients.DataSource = dt;
            ngfd_gvClients.DataBind();
        }

        protected void ngfd_gvClients_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Fetch controls
                DropDownList ddlExistingInvestor = (DropDownList)e.Row.FindControl("ngfd_ddlExistingInvestor");
                TextBox txtInvestorCode = (TextBox)e.Row.FindControl("ngfd_txtInvestorCode");
                DropDownList ddlSalutation = (DropDownList)e.Row.FindControl("ngfd_ddlSalutation");
                TextBox txtInvestorName = (TextBox)e.Row.FindControl("ngfd_txtInvestorName");
                TextBox txtMobile = (TextBox)e.Row.FindControl("ngfd_txtMobile");
                TextBox txtEmail = (TextBox)e.Row.FindControl("ngfd_txtEmail");
                TextBox txtPAN = (TextBox)e.Row.FindControl("ngfd_txtPAN");
                TextBox txtAadhar = (TextBox)e.Row.FindControl("ngfd_txtAadhar");
                TextBox txtDOB = (TextBox)e.Row.FindControl("ngfd_txtDOB");
                DropDownList ddlGender = (DropDownList)e.Row.FindControl("ngfd_ddlGender");
                TextBox txtGuardianPAN = (TextBox)e.Row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)e.Row.FindControl("ngfd_txtgName");
                DropDownList ddlOccupation = (DropDownList)e.Row.FindControl("ngfd_ddlOccupation");
                DropDownList ddlRelation = (DropDownList)e.Row.FindControl("ngfd_ddlRelation");
                DropDownList ddlKyc = (DropDownList)e.Row.FindControl("ngfd_ddlKyc");
                DropDownList ddlApproved = (DropDownList)e.Row.FindControl("ngfd_ddlApproved");
                DropDownList ddlIsNominee = (DropDownList)e.Row.FindControl("ngfd_ddlIsNominee");
                TextBox txtAllocation = (TextBox)e.Row.FindControl("ngfd_txtAllocation");

                DataRowView row = (DataRowView)e.Row.DataItem;

                // Bind values to dropdowns
                

                if (ddlSalutation != null)
                {
                    //DataTable dt = new WM.Controllers.AccountOpeningController().GetSalutationList();
                    //ddlSalutation.Items.Clear();

                    //if (dt != null && dt.Rows.Count > 0)
                    //{
                    //    ddlSalutation.DataSource = dt;
                    //    ddlSalutation.DataTextField = "Text";
                    //    ddlSalutation.DataValueField = "Value";
                    //    ddlSalutation.DataBind();
                    //    ddlSalutation.Items.Insert(0, new ListItem("Select", ""));
                    //}
                    //else
                    //{
                    //    ddlSalutation.Items.Insert(0, new ListItem("No Title", ""));
                    //}
                    ddlSalutation.SelectedValue = row["Salutation"].ToString();

                }

                if (ddlGender != null)
                {
                    ddlGender.SelectedValue = row["Gender"].ToString();
                }

                if (ddlOccupation != null)
                {
                    //DataTable dt = new WM.Controllers.AccountOpeningController().GetOccupationList();

                    //if (dt != null && dt.Rows.Count > 0)
                    //{
                    //    ddlOccupation.DataSource = dt; // Fetch from DB or predefined list
                    //    ddlOccupation.DataTextField = "OCC_NAME";
                    //    ddlOccupation.DataValueField = "OCC_ID";
                    //    ddlOccupation.DataBind();
                    //    ddlOccupation.Items.Insert(0, new ListItem("Select", ""));
                    //}
                    //else
                    //{
                    //    ddlSalutation.Items.Insert(0, new ListItem("No Occupation", ""));
                    //}
                    ddlOccupation.SelectedValue = row["Occupation"].ToString();

                }

                if (ddlRelation != null)
                {
                    //DataTable dt = new WM.Controllers.AccountOpeningController().GetRelaitonshipList();
                    //if (dt != null && dt.Rows.Count > 0)
                    //{
                    //    ddlRelation.DataSource = dt; // Fetch from DB or predefined list
                    //    ddlRelation.DataTextField = "RELATION_NAME";
                    //    ddlRelation.DataValueField = "RELATION_ID";
                    //    ddlRelation.DataBind();
                    //    ddlRelation.Items.Insert(0, new ListItem("Select", ""));
                    //}
                    //else
                    //{
                    //    ddlRelation.Items.Insert(0, new ListItem("No Relations", ""));
                    //}
                    ddlRelation.SelectedValue = row["Relation"].ToString();

                    
                }

                if (ddlKyc != null)
                {
                    ddlKyc.SelectedValue = row["KYC"].ToString();
                }

                if (ddlApproved != null)
                {
                    ddlApproved.SelectedValue = row["Approved"].ToString();
                }

                if (ddlIsNominee != null)
                {
                    ddlIsNominee.SelectedValue = row["IsNominee"].ToString();
                }

                // Bind textboxes
                if (txtInvestorCode != null)
                {
                    txtInvestorCode.Attributes["placeholder"] = "Investor Code";
                    txtInvestorCode.Attributes["title"] = "Investor Code";
                    txtInvestorCode.Text = row["InvestorCode"].ToString();
                }
                if (txtInvestorName != null)
                {
                    txtInvestorName.Attributes["placeholder"] = "Investor Name";
                    txtInvestorName.Attributes["title"] = "Investor Name";
                    txtInvestorName.Text = row["InvestorName"].ToString();
                }
                if (txtMobile != null)
                {
                    txtMobile.Attributes["placeholder"] = "Investor Mobile";
                    txtMobile.Attributes["title"] = "Investor Mobile";
                    txtMobile.Text = row["Mobile"].ToString();
                }
                if (txtEmail != null)
                {
                    txtEmail.Attributes["placeholder"] = "Investor Email";
                    txtEmail.Attributes["title"] = "Investor Email";
                    txtEmail.Text = row["Email"].ToString();
                }
                if (txtPAN != null)
                {
                    txtPAN.Attributes["placeholder"] = "Investor PAN";
                    txtPAN.Attributes["title"] = "Enter PAN in format: ABCDE1234F";
                    txtPAN.Attributes["oninput"] = "validatePanInput2(this)";
                    txtPAN.Text = row["PAN"].ToString();
                }
                if (txtAadhar != null)
                {
                    txtAadhar.Attributes["placeholder"] = "Investor Aadhar";
                    txtAadhar.Attributes["title"] = "Investor Aadhar (e.g. 12 Digits)";
                    txtAadhar.Text = row["Aadhar"].ToString();
                }
                if (txtDOB != null)
                {
                    txtDOB.Attributes["placeholder"] = "Investor DOB DD/MM/YYYY";
                    txtDOB.Attributes["title"] = "Enter Investor DOB DD/MM/YYYY";
                    txtDOB.Attributes["oninput"] = "formatDOBInput(this); validateGuardianRequirementMember(this);";
                    txtDOB.Text = row["DOB"].ToString(); ;
                }
                if (txtGuardianPAN != null)
                {
                    txtGuardianPAN.Attributes["placeholder"] = "Guardian PAN";
                    txtGuardianPAN.Attributes["title"] = "Enter Guardian PAN in format: ABCDE1234F";
                    txtGuardianPAN.Attributes["oninput"] = "validatePanInput2(this)";
                    txtGuardianPAN.Text = row["GuardianPAN"].ToString();
                }
                if (txtGuardianName != null)
                {
                    txtGuardianName.Attributes["placeholder"] = "Guardian Name";
                    txtGuardianName.Attributes["title"] = "Enter Guardian Name";
                    txtGuardianName.Text = row["GuardianName"].ToString();
                }
                if (txtAllocation != null)
                {
                    txtAllocation.Attributes["placeholder"] = "Allocation Percentage";
                    txtAllocation.Attributes["title"] = "Enter Allocation Percentage";
                    txtAllocation.Text = row["Allocation"].ToString();
                }


                if (txtDOB.Text != null)
                {
                    ValidateAge(txtDOB.Text, txtGuardianPAN, txtGuardianName);
                }


               


                if (ddlExistingInvestor != null)
                {
                    string famSrcId = txtHeadSourceCode.Text;
                    string famExistID = txtClientCode.Text;

                    DataTable dt = new WM.Controllers.AccountOpeningController().GetInvestorList(famSrcId, famExistID);

                    ddlExistingInvestor.Items.Clear();
                    ddlExistingInvestor.DataSource = dt;
                    ddlExistingInvestor.DataTextField = "investor_name";
                    ddlExistingInvestor.DataValueField = "inv_code";
                    ddlExistingInvestor.DataBind();

                    ddlExistingInvestor.Items.Insert(0, new ListItem("Select Investor", ""));
                    ddlExistingInvestor.Enabled = true;
                    if (ddlExistingInvestor.Items.Count > 1)
                    {
                        ddlExistingInvestor.Enabled = true;
                        ddlExistingInvestor.SelectedValue = row["ExistingInvestor"].ToString();
                    }
                    else
                    {
                        //ddlExistingInvestor.Items.Clear();
                        //ddlExistingInvestor.Items.Add(new ListItem("Investor Not Exist", ""));
                        //ddlExistingInvestor.SelectedIndex = 0;
                        ddlExistingInvestor.Enabled = true;
                    }
                }


            }
        }

        protected void ngfd_gvClients_RowDataBound_DOB(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txtDOB = (TextBox)e.Row.FindControl("ngfd_txtDOB");
                TextBox txtGuardianPAN = (TextBox)e.Row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)e.Row.FindControl("ngfd_txtgName");

                // Convert DOB from data source if available
                DataRowView row = (DataRowView)e.Row.DataItem;
                DateTime dob;
                if (DateTime.TryParse(row["DOB"].ToString(), out dob))
                {
                    txtDOB.Text = dob.ToString("dd-MM-yyyy");

                    // Check Age
                    if ((DateTime.Now - dob).TotalDays / 365.25 < 18)
                    {
                        txtGuardianPAN.Enabled = true;
                        txtGuardianName.Enabled = true;
                    }
                    else
                    {
                        txtGuardianPAN.Enabled = false;
                        txtGuardianName.Enabled = false;
                    }
                }
            }
        }

        protected void ngfd_ddladdfamExistingInvestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlExistingInvestor = (DropDownList)sender;
            string selected_inv = ddlExistingInvestor.SelectedValue;
            GridViewRow row = (GridViewRow)ddlExistingInvestor.NamingContainer;
            

            TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
            TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
            DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
            TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
            TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
            TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
            TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
            TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
            DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
            TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
            TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
            DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
            DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
            DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
            DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
            DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
            TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");

            string ddlExistingInvestorValue = ddlExistingInvestor.SelectedValue;
            string rowValue = row.RowIndex.ToString();
            string txtInvestorCodeValue = txtInvestorCode.Text;
            string txtInvestorNameValue = txtInvestorName.Text;
            string ddlSalutationValue = ddlSalutation.SelectedValue;
            string txtMobileValue = txtMobile.Text;
            string txtEmailValue = txtEmail.Text;
            string txtPANValue = txtPAN.Text;
            string txtAadharValue = txtAadhar.Text;
            string txtDOBValue = txtDOB.Text;
            string ddlGenderValue = ddlGender.SelectedValue;
            string txtGuardianPANValue = txtGuardianPAN.Text;
            string txtGuardianNameValue = txtGuardianName.Text;
            string ddlOccupationValue = ddlOccupation.SelectedValue;
            string ddlRelationValue = ddlRelation.SelectedValue;
            string ddlKycValue = ddlKyc.SelectedValue;
            string ddlApprovedValue = ddlApproved.SelectedValue;
            string ddlIsNomineeValue = ddlIsNominee.SelectedValue;
            string txtAllocationValue = txtAllocation.Text;
            
            #region Null handler
            /*
            // Bind values to dropdowns
            if (ddlExistingInvestor != null)
            {
                
            }

            if (ddlSalutation != null)
            {
                DataTable dt = new WM.Controllers.AccountOpeningController().GetSalutationList();
                ddlSalutation.Items.Clear();

                if (dt != null && dt.Rows.Count > 0)
                {
                    ddlSalutation.DataSource = dt;
                    ddlSalutation.DataTextField = "Text";
                    ddlSalutation.DataValueField = "Value";
                    ddlSalutation.DataBind();
                    ddlSalutation.Items.Insert(0, new ListItem("Select", ""));

                }
                else
                {

                    ddlSalutation.Items.Insert(0, new ListItem("No Title", ""));
                }
            }

            if (ddlGender != null)
            {
                //ddlGender.SelectedValue = row["Gender"].ToString();
            }

            if (ddlOccupation != null)
            {
                DataTable dt = new WM.Controllers.AccountOpeningController().GetOccupationList();

                if (dt != null && dt.Rows.Count > 0)
                {

                    ddlOccupation.DataSource = dt; // Fetch from DB or predefined list
                    ddlOccupation.DataTextField = "OCC_NAME";
                    ddlOccupation.DataValueField = "OCC_ID";
                    ddlOccupation.DataBind();
                    ddlOccupation.Items.Insert(0, new ListItem("Select", ""));
                }
                else
                {

                    ddlSalutation.Items.Insert(0, new ListItem("No Occupation", ""));
                }


            }

            if (ddlRelation != null)
            {
                DataTable dt = new WM.Controllers.AccountOpeningController().GetRelaitonshipList();
                if (dt != null && dt.Rows.Count > 0)

                {

                    ddlRelation.DataSource = dt; // Fetch from DB or predefined list
                    ddlRelation.DataTextField = "RELATION_NAME";
                    ddlRelation.DataValueField = "RELATION_ID";
                    ddlRelation.DataBind();
                    ddlRelation.Items.Insert(0, new ListItem("Select", ""));
                }
                else
                {
                    ddlRelation.Items.Insert(0, new ListItem("No Relations", ""));

                }
            }

            if (ddlKyc != null)
            {
                //ddlKyc.SelectedValue = row["KYC"].ToString();
            }

            if (ddlApproved != null)
            {
                //ddlApproved.SelectedValue = row["Approved"].ToString();
            }

            if (ddlIsNominee != null)
            {
                //ddlIsNominee.SelectedValue = row["IsNominee"].ToString();
            }

            // Bind textboxes
            if (txtInvestorCode != null)
            {
                //txtInvestorCode.Text = row["InvestorCode"].ToString();
            }
            if (txtInvestorName != null)
            {
                //txtInvestorName.Text = row["InvestorName"].ToString();
            }
            if (txtMobile != null)
            {
                //txtMobile.Text = row["Mobile"].ToString();
            }
            if (txtEmail != null)
            {
                //txtEmail.Text = row["Email"].ToString();
            }
            if (txtPAN != null)
            {
                //txtPAN.Attributes["placeholder"] = "Enter PAN (ABCDE1234F)";
                //txtPAN.Attributes["title"] = "Enter PAN in format: ABCDE1234F";
                //txtPAN.Attributes["oninput"] = "validatePanInput2(this)";
                //txtPAN.Text = row["PAN"].ToString();
            }
            if (txtAadhar != null)
            {
                //txtAadhar.Text = row["Aadhar"].ToString();
            }
            if (txtDOB != null || txtDOB == null)
            {
                //txtPAN.Attributes["placeholder"] = "DD/MM/YYYY";

                txtDOB.Attributes["oninput"] = "formatDOBInput(this); validateGuardianRequirementMember(this);";

                //txtDOB.Text =  Convert.ToDateTime(row["DOB"].ToString("dd/MM/yyyy");
            };
            if (txtGuardianPAN != null)
            {
                //txtGuardianPAN.Text = row["GuardianPAN"].ToString();
            }
            if (txtGuardianName != null)
            {
                //txtGuardianName.Text = row["GuardianName"].ToString();
            }
            if (txtAllocation != null)
            {
                //txtAllocation.Text = row["Allocation"].ToString();
            }
            */

            #endregion


            bool isDuplicate = false;
            foreach (GridViewRow row_2 in ngfd_gvClients.Rows)
            {
                TextBox txtExistingInvestorCode = (TextBox)row_2.FindControl("ngfd_txtInvestorCode");
                string invText = txtExistingInvestorCode.Text.Trim();
               

                if (txtExistingInvestorCode != null && invText == ddlExistingInvestorValue)
                {
                    isDuplicate = true;
                    break; // Exit loop if duplicate found
                }
            }

 
            if (string.IsNullOrEmpty(ddlExistingInvestorValue))
            {
                if (!string.IsNullOrEmpty(txtInvestorCodeValue))
                {
                    if (ddlExistingInvestorValue == txtInvestorCodeValue)
                    {
                        ddlExistingInvestor.SelectedValue = txtInvestorCodeValue;
                        return;
                    }
                    ddlExistingInvestor.SelectedIndex = 0;

                }

                else
                {
                    #region Reset MultiMemGrid
                    ddlExistingInvestor.SelectedIndex = 0;
                    txtInvestorCode.Text = string.Empty;
                    ddlSalutation.SelectedIndex = 0;
                    txtInvestorName.Text = string.Empty;
                    ddlGender.SelectedIndex = 0;
                    txtMobile.Text = string.Empty;
                    txtDOB.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtPAN.Text = string.Empty;
                    ddlRelation.SelectedIndex = 0;
                    ddlKyc.SelectedIndex = 0;
                    txtGuardianName.Text = string.Empty;
                    txtGuardianPAN.Text = string.Empty;
                    ddlOccupation.SelectedIndex = 0;
                    ddlApproved.SelectedIndex = 0;
                    txtAadhar.Text = string.Empty;
                    ddlIsNominee.SelectedIndex = 0;
                    txtAllocation.Text = string.Empty;
                    #endregion
                                        
                    return;
                }

            }

            else if (!string.IsNullOrEmpty(ddlExistingInvestorValue))
            {
                if(!string.IsNullOrEmpty(ddlExistingInvestorValue) && !string.IsNullOrEmpty(txtInvestorCodeValue))
                {
                    ddlExistingInvestor.SelectedValue = txtInvestorCodeValue;
                    return;
                }
                if (isDuplicate)
                {
                    if (ddlExistingInvestorValue == txtInvestorCodeValue)
                    {
                        ddlExistingInvestor.SelectedValue = txtInvestorCodeValue;
                        return;
                    }
                    ddlExistingInvestor.SelectedValue = txtInvestorCodeValue;
                    return;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtInvestorCodeValue))
                    {
                        if (ddlExistingInvestorValue == txtInvestorCodeValue)
                        {
                            ddlExistingInvestor.SelectedValue = txtInvestorCodeValue;
                            return;
                        }

                        ddlExistingInvestor.SelectedIndex = 0;
                        return;

                    }
                    else
                    {
                        DataTable memberData = new AccountOpeningController().GetMemberDataByClientCode(selected_inv);
                        if (memberData.Rows.Count > 0)
                        {
                            DataRow fetched_row = memberData.Rows[0];
                            #region Get DT Values 
                            string inv_code = GetTextFieldValue(fetched_row, "INVESTOR_CODE");
                            string inv_title = GetTextFieldValue(fetched_row, "investor_title");
                            string inv_name = GetTextFieldValue(fetched_row, "investor_name");
                            string inv_gender = GetTextFieldValue(fetched_row, "gender");
                            string inv_mobile = GetTextFieldValue(fetched_row, "mobile");
                            string inv_dob = GetTextFieldValue(fetched_row, "dob");
                            string inv_email = GetTextFieldValue(fetched_row, "email");
                            string inv_pan = GetTextFieldValue(fetched_row, "pan");
                            string inv_rel = GetTextFieldValue(fetched_row, "OUR_RELATIONSHIP");
                            string inv_kyc = GetTextFieldValue(fetched_row, "KYC");
                            string inv_g_name = GetTextFieldValue(fetched_row, "g_name");
                            string inv_g_pan = GetTextFieldValue(fetched_row, "g_pan");
                            string inv_occ_id = GetTextFieldValue(fetched_row, "occ_id");
                            string inv_approved = GetTextFieldValue(fetched_row, "approved");
                            string inv_AADHAR_CARD_NO = GetTextFieldValue(fetched_row, "AADHAR_CARD_NO");
                            string inv_is_nominee = GetTextFieldValue(fetched_row, "is_nominee");
                            string inv_nominee_per = GetTextFieldValue(fetched_row, "nominee_per");
                            #endregion

                            #region Set DT Values
                            txtInvestorCode.Text = inv_code;
                            SetOrAddDropdownValue(fetched_row, "investor_title", ddlSalutation);//ddlSalutation.SelectedValue = inv_title;
                            txtInvestorName.Text = inv_name;
                            ddlGender.SelectedValue = GetDropValueByFirstChar(inv_gender, ddlGender);
                            txtMobile.Text = inv_mobile;
                            GetSetDateField(fetched_row, "dob", txtDOB); //txtDOB.Text = inv_dob;
                            txtEmail.Text = inv_email;
                            txtPAN.Text = inv_pan;
                            ddlRelation.SelectedValue = (inv_rel == "0" || inv_rel == "" ? "" : inv_rel);
                            ddlKyc.SelectedValue = GetDropValueByFirstChar(inv_kyc, ddlKyc);
                            txtGuardianName.Text = inv_g_name;
                            txtGuardianPAN.Text = inv_g_pan;
                            ddlOccupation.SelectedValue = GetDropValueByFirstChar(inv_occ_id, ddlOccupation);
                            ddlApproved.SelectedValue = GetDropValueByFirstChar(inv_approved, ddlApproved);
                            txtAadhar.Text = inv_AADHAR_CARD_NO;
                            ddlIsNominee.SelectedValue = GetDropValueByFirstChar(inv_is_nominee, ddlIsNominee);
                            txtAllocation.Text = inv_nominee_per;

                            #endregion

                            ddlSalutation.Focus();
                        }
                    }                
                }
            }
        }

     
        private bool IsValueUniqueInGrid(GridView gridView, string columnName, string valueToCheck)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                Control control = row.FindControl(columnName);

                if (control is TextBox txtControl)
                {
                    if (txtControl.Text.Trim().Equals(valueToCheck.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Value exists, not unique
                    }
                }
                else if (control is DropDownList ddlControl)
                {
                    if (ddlControl.SelectedValue.Trim().Equals(valueToCheck.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Value exists, not unique
                    }
                }
            }

            return true; // Value is unique
        }

        private int GetValidInvestorCodeRowCount(GridView gridView, string columnName)
        {
            int validRowCount = 0;

            foreach (GridViewRow row in gridView.Rows)
            {
                TextBox txtInvestorCode = (TextBox)row.FindControl(columnName);

                // Check if the TextBox exists and has a non-empty value
                if (txtInvestorCode != null && !string.IsNullOrWhiteSpace(txtInvestorCode.Text))
                {
                    validRowCount++;
                }
            }

            return validRowCount;
        }



        protected void ngfd_txtDOB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", "showServerLoader();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowClientLoader", "showClientLoader();", true);


                TextBox txtDOB = (TextBox)sender;
                GridViewRow row = (GridViewRow)txtDOB.NamingContainer;

                TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");

                if(txtDOB.Text != null)
                {
                    //ValidateAge(txtDOB.Text, txtGuardianPAN, txtGuardianName);
                }
                /*
                DateTime dob;
                string currentFabDate = txtDOB.Text; // e.g., "16/12/1987"
                string dateFormat = "dd/MM/yyyy"; // Specify the expected format
                CultureInfo provider = CultureInfo.InvariantCulture; // Use invariant culture for consistent parsing

                if (DateTime.TryParseExact(currentFabDate, dateFormat, provider, DateTimeStyles.None, out dob))
                {
                    int age = (int)((DateTime.Now - dob).TotalDays / 365.25);

                    if (age < 18)
                    {
                        txtGuardianPAN.Enabled = true;
                        txtGuardianName.Enabled = true;
                    }
                    else
                    {
                        txtGuardianPAN.Enabled = false;
                        txtGuardianName.Enabled = false;
                        txtGuardianPAN.Text = "";
                        txtGuardianName.Text = "";
                    }
                }
                */
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", "hideServerLoader();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "HideClientLoader", "hideClientLoader();", true);

            }
        }
        #endregion


        public string GetDropValueByFirstChar(string inv_gender, DropDownList targetDdl)
        {
            if (!string.IsNullOrEmpty(inv_gender))
            {
                string firstChar = inv_gender.Substring(0, 1).ToUpper(); // Get the first character

                foreach (ListItem item in targetDdl.Items)
                {
                    if (item.Value.StartsWith(firstChar, StringComparison.OrdinalIgnoreCase))
                    {
                        return item.Value; // Return the first matched dropdown value
                    }
                }
            }
            return ""; // Return empty if no match found
        }

        public string ReturnYESNOByYN(string inv_gender)
        {
            string returnValue = "";
            if (!string.IsNullOrEmpty(inv_gender))
            {
                string firstChar = inv_gender.Substring(0, 1).ToUpper(); // Get the first character

                if (firstChar == "Y")
                {
                    returnValue = "YES";
                }
                else if(firstChar == "N")
                {
                    returnValue = "NO";
                }
                else if(firstChar == "O")
                {
                    returnValue = "OTHER";
                }
                else
                {
                    returnValue = "";
                }

            }
            return returnValue;
        }


        public static string ValidateAgeReturnGValidation(string dobText, TextBox guardianPan, TextBox guardianName)
        {
            DateTime dob;
            string dateFormat = "dd/MM/yyyy"; // Expected date format
            CultureInfo provider = CultureInfo.InvariantCulture;

            // Try parsing the input date
            if (DateTime.TryParseExact(dobText, dateFormat, provider, DateTimeStyles.None, out dob))
            {
                int age = (int)((DateTime.Now - dob).TotalDays / 365.25);

                if (age < 18 && (string.IsNullOrEmpty(guardianName.Text)))
                {
                    //guardianPan.Enabled = true;
                    //guardianName.Enabled = true;
                    return "Guardian name is requried for minors";
                }
                
            }

            return null;
        }


        public static void ValidateAge(string dobText, TextBox guardianPan, TextBox guardianName)
        {
            DateTime dob;
            string dateFormat = "dd/MM/yyyy"; // Expected date format
            CultureInfo provider = CultureInfo.InvariantCulture;

            // Try parsing the input date
            if (DateTime.TryParseExact(dobText, dateFormat, provider, DateTimeStyles.None, out dob))
            {
                int age = (int)((DateTime.Now - dob).TotalDays / 365.25);

                if (age < 18)
                {
                    //guardianPan.Enabled = true;
                    //guardianName.Enabled = true;

                    guardianName.Attributes["required"] = "required";
                }
                else
                {
                    //guardianPan.Enabled = false;
                    //guardianName.Enabled = false;
                    //guardianPan.Text = "";
                    //guardianName.Text = "";
                    guardianName.Attributes.Remove("required");
                }
            }
            else
            {
                // Handle invalid date format (optional)
               // guardianPan.Enabled = false;
                //guardianName.Enabled = false;
            }
        }
        public string CurrentSqlExceptionMsg(DataTable SQL_RESULT)
        {
            if (SQL_RESULT.Rows.Count == 1 && SQL_RESULT.Columns.Contains("Exception") && !string.IsNullOrEmpty(SQL_RESULT.Rows[0]["Exception"].ToString()))
            {
                string exceptionMsg = SQL_RESULT.Rows[0]["Exception"].ToString();
                return(exceptionMsg);
            }
            return null;
        }
        public static DataTable CurrentSql(string query)
        {
            DataTable resultTable = new DataTable();

            try
            {
                resultTable = new WM.Controllers.AccountOpeningController().ExecuteQuery(query);

            }
            catch (Exception ex)
            {
            }

            return resultTable;
        }


        #region FILL_AC_CATEGORY
        private void FILL_AC_CATEGORY()
        {
            DataTable dt = new WM.Controllers.AccountOpeningController().GetAccountCategoryList();

            if (dt.Rows.Count > 0)
            {
                
                ddlAOACCategory.DataSource = dt;
                ddlAOACCategory.DataTextField = "itemname";
                ddlAOACCategory.DataValueField = "itemserialnumber";
                ddlAOACCategory.DataBind();
                ddlAOACCategory.Items.Insert(0, new ListItem("Select AC Category", ""));
            }


        }
        #endregion

        public static string GetLastNumberFromString(string input)
        {
            MatchCollection matches = Regex.Matches(input, "\\d+");
            return matches.Count > 0 ? matches[matches.Count - 1].Value : string.Empty;
        }
        #region Advisory Package 
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
                string bankName = (ddlMutualBank.SelectedIndex == 0 ? null : ddlMutualBank.SelectedItem.Text);
                double amount = string.IsNullOrEmpty(txtAmount.Text) ? 0 : Convert.ToDouble(txtAmount.Text);
                int rmCode = string.IsNullOrEmpty(txtBusinessCode.Text) ? 0 : Convert.ToInt32(txtBusinessCode.Text);
                //int branchCode = string.IsNullOrEmpty(txtBusinessCode.Text) ? 0 : Convert.ToInt32(txtBusinessCode.Text);
                int branchCode = string.IsNullOrEmpty(txtBusinessCodeBranch.Text) ? 0 : Convert.ToInt32(GetLastNumberFromString(txtBusinessCodeBranch.Text));
                 
                string loggedInUser = Session["LoginId"]?.ToString();
                string remark = string.IsNullOrEmpty(txtRemark.Text) ? null : txtRemark.Text;



                string[] partsChequeDraftName = ChequeLabel.Text.Split('<');
                string chequeDraftLabelDynamic = "";

                if (partsChequeDraftName.Length > 0)
                {
                    chequeDraftLabelDynamic = partsChequeDraftName[0]; // Extracts the text before '<'
                }


                if (sourceId == 0 && ExistingClientCodeInv.Text != "0" && ExistingClientCodeInv.Text.Length >= 11)
                {
                    sourceId = Convert.ToInt32(ExistingClientCodeInv.Text.Substring(0, 8));
                }


                if (sourceId != 0)
                {
                    if (businessCode == 0)
                    {
                        string message = "Business Code Can Not Left Blank.";
                        ShowAlert(message);
                        return;
                    }

                    if (string.IsNullOrEmpty(selectedPlanTypeValue))
                    {
                        string message = "Plan Can Not Be Left Blank.";
                        ShowAlert(message);
                        ddlMutualPlanType.Focus();
                        return;
                    }

                    if(!optCash.Checked && !cheque.Checked && !draft.Checked)
                    {
                        ShowAlert("Cheque any payment option!");

                    }
                    if (!optCash.Checked)
                    {
                        if (string.IsNullOrEmpty(bankName))
                        {
                            string message = "Bank Name Can Not Be Left Blank.";
                            ShowAlert(message);
                            ddlMutualBank.Focus();
                            return;
                        }
                    }
                    if (cheque.Checked || draft.Checked)
                    {
                        if (string.IsNullOrEmpty(txtChequeDate.Text))
                        {
                            string message = chequeDraftLabelDynamic + " date is empty.";
                            ShowAlert(message);
                            txtChequeDate.Focus();
                            return;
                        }

                        if (string.IsNullOrEmpty(chequeNo))
                        {
                            string message = "Please Enter Cheque No "+ chequeDraftLabelDynamic ;
                            ShowAlert(message);
                            txtChequeNo.Focus();  
                            return;  
                        }
                    }

                    if ( optCash.Checked && string.IsNullOrEmpty(txtAmount.Text))
                    {
                        string message = "Amount is empty.";
                        ShowAlert(message);
                        //lblMessage.Text = message;
                        txtAmount.Focus();
                        return;
                    }


                    if (!fresh.Checked && !renewal.Checked)
                    {
                        string message = "Please select either Fresh or Renewal for Plan Type.";
                        ShowAlert(message);
                        fresh.Focus();
                        return;
                    }                  
 
                }
                else  
                {
                    string message = "You Have No KYC Account.";
                    ShowAlert(message);
                    return;
                }

                if (!string.IsNullOrEmpty(txtChequeNo.Text))
                {
                    string currChNo = txtChequeNo.Text.Trim();
 
                    DataTable dt1 = CurrentSql("select nvl(count(cheque_no),0) AS MESSAGE from transaction_st where TRIM(cheque_no)='" + currChNo + "' and remark= 'ADVISORY'");

                    if (dt1.Rows.Count > 0)
                    {
                        if (dt1.Rows[0]["MESSAGE"].ToString() != "0")
                        {
                            string message = "This Transaction Already Exist";
                            ShowAlert(message);
                            return;
                        }
                    }                   

                }
                if (fresh.Checked)
                {
                    DataTable dt2 = CurrentSql("select nvl(count(*),0) AS MESSAGE from transaction_st where client_code='" + ExistingClientCodeInv.Text + "' and remark= 'ADVISORY' and tran_type='PURCHASE'");

                    if (dt2.Rows.Count > 0)
                    {
                        if (dt2.Rows[0]["MESSAGE"].ToString() != "0")
                        {
                            string message = "Please Select Renewal Option";
                            ShowAlert(message);
                            return;
                        }
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
            //DataTable dt = new AccountOpeningController().GetAdvisoryDataByAH(txtSearchClientCode.Text, txtSearchPan.Text);
            DataTable dt = new AccountOpeningController().GetAdvisoryDataByAHINV(ExistingClientCodeInv.Text, txtSearchClientCode.Text);

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

        private void FillAdvisoryDataByAH(string clientCodeKyc, string clientCodeAH)
        {
            try
            {
                DataTable dt = new AccountOpeningController().GetAdvisoryDataByAHINV(clientCodeKyc, clientCodeAH);
                string sql = "";
                sql = @"SELECT 'Valid Data' as message, a.TRAN_CODE as TRAN_CODE, a.TR_DATE as TR_DATE, a.CLIENT_CODE as CLIENT_CODE, a.MUT_CODE as MUT_CODE, a.SCH_CODE as SCH_CODE, a.APP_NO as APP_NO, a.FOLIO_NO as FOLIO_NO, a.TRAN_TYPE as TRAN_TYPE, a.REF_TRAN_CODE as REF_TRAN_CODE, a.AMOUNT as AMOUNT, a.UNITS as UNITS, a.RATE as RATE, a.APP_DATE as APP_DATE, a.BRANCH_CODE as BRANCH_CODE, a.SOURCE_CODE as SOURCE_CODE, a.RE_INVT as RE_INVT, a.NEW_FROM_BROK_STAT as NEW_FROM_BROK_STAT, a.NAV_DATE as NAV_DATE, a.PAYMENT_MODE as PAYMENT_MODE, a.CHEQUE_DATE as CHEQUE_DATE, a.CHEQUE_NO as CHEQUE_NO, a.BANK_NAME as BANK_NAME, a.BANK_BRANCH as BANK_BRANCH, a.BANK_AC_NO as BANK_AC_NO, a.ASR as ASR, a.ASA as ASA, a.BROKER_ID as BROKER_ID, a.BROK_CAL as BROK_CAL, a.INVESTOR_TYPE as INVESTOR_TYPE, a.SWITCH_SCHEME as SWITCH_SCHEME, a.SWITCH_FOLIO as SWITCH_FOLIO, a.INTER as INTER, a.SWITCH_TRANCODE as SWITCH_TRANCODE, a.RMCODE as RMCODE, a.BILL_GEN as BILL_GEN, a.SYS_TRAN as SYS_TRAN, a.MANUAL_ARNO as MANUAL_ARNO, a.CHALLAN_FL as CHALLAN_FL, a.BUSINESS_RMCODE as BUSINESS_RMCODE, a.TRAN_ID as TRAN_ID, a.LOGGEDUSERID as LOGGEDUSERID, a.TIMEST as TIMEST, a.REMARK as REMARK, a.FLAG as FLAG, a.IMP_DATE as IMP_DATE, a.TRAN_UPDATE as TRAN_UPDATE, a.BUSI_BRANCH_CODE as BUSI_BRANCH_CODE, a.REG_DATE as REG_DATE, a.MODIFY_USER as MODIFY_USER, a.MODIFY_DATE as MODIFY_DATE, a.FAMILY_HEAD as FAMILY_HEAD, a.FPF_DATE as FPF_DATE, a.FPL_DATE as FPL_DATE, a.CH_RECO as CH_RECO, a.POSTED as POSTED, a.REG_TRANTYPE as REG_TRANTYPE, a.CROR_PLANNO as CROR_PLANNO, a.CROR_DATE as CROR_DATE, a.AUDIT_FPDATE as AUDIT_FPDATE, a.PLAN_TYPE as PLAN_TYPE, a.REG_SUBBROK as REG_SUBBROK, a.SEQ_NO as SEQ_NO, a.REG_TRAN_FLAG as REG_TRAN_FLAG, a.AUM_FLAG as AUM_FLAG, a.INV_NAME as INV_NAME, a.PORTFOLIO_FLAG as PORTFOLIO_FLAG, a.DUP_FLAG as DUP_FLAG, a.DUP_FLAG1 as DUP_FLAG1, a.DUP_FLAG2 as DUP_FLAG2, a.UNIQUE_ID as UNIQUE_ID, a.AUTOMAP_FLAG as AUTOMAP_FLAG, a.FP_STATUS as FP_STATUS, a.MODIFY_TALISMA as MODIFY_TALISMA, a.TRAN_SRC as TRAN_SRC, b.client_codekyc as b_client_codekyc, b.CLIENT_code   as  b_act_code, a.client_code        as a_client_code, client_pan FROM transaction_st a, client_test b WHERE remark = 'ADVISORY' AND";
                sql += " AND a.client_code = '" + clientCodeKyc + "' and rownum = 1";
                //DataTable dt = CurrentSql(sql);

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
            //DataTable dt = new AccountOpeningController().GetAdvisoryDataByAH(txtSearchClientCode.Text, txtSearchPan.Text);
            DataTable dt = new AccountOpeningController().GetAdvisoryDataByAHINV(ExistingClientCodeInv.Text, txtSearchClientCode.Text);

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
            string db_tranDate = row["tr_date"] != DBNull.Value ? row["tr_date"].ToString() : string.Empty;
            // Parse and format as DD-MM-YYYY
            string formattedTranDate = DateTime.TryParse(db_tranDate, out DateTime parsedDate)
                ? parsedDate.ToString("dd/MMyyyy")
                : string.Empty;

            DateTime MyTrDate;

            if (row["tr_date"] != DBNull.Value)
            {
                MyTrDate = Convert.ToDateTime(row["tr_date"]).Date;
            }
            else
            {
                MyTrDate = DateTime.MinValue; // Or use a default date, such as DateTime.Now or another appropriate value
            }

            string BusinessCode = txtBusinessCode.Text;
            string MySourceId = txtHeadSourceCode.Text;

            DateTime effectiveDate;

            // Check MyPrintSourceId and assign appropriate date
            if (!string.IsNullOrEmpty(MyPrintSourceId))
            {
                effectiveDate = MyTrDate; // Assign MyTrDate if MyPrintSourceId is present
            }
            else
            {
                effectiveDate = DateTime.Now; // Assign system date if MyPrintSourceId is empty
            }


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

                if(string.IsNullOrEmpty(MyTranCode))
                {
                    psm_controller.ShowAlert(this,"Transaction Code (MyTranCode) is missing. AR cannot be printed.");
                }



                // Fetch values from UI
               // string MyTranCode = txtMyTranCode.Text.Trim();
               // string MyPrintSourceId = txtMyPrintSourceId.Text.Trim();
               // DateTime MyTrDate = string.IsNullOrEmpty(txtMyTrDate.Text) ? DateTime.Now : Convert.ToDateTime(txtMyTrDate.Text);
                string txtbusicode = txtBusinessCode.Text.Trim();
              //  string MySourceId = txtMySourceId.Text.Trim();

                // Determine effective_date
                string effective_date = !string.IsNullOrEmpty(MyPrintSourceId) ? MyTrDate.ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy");

                // Construct SQL query
                string sqlQuery = $@"
    SELECT b.client_code, 'P' ar_type, t.tran_code, tr_date, 
          cheque_date cheque_date, cheque_no cheque_no, t.bank_name, amount, 
         b.client_code source_code, app_no, 
         NVL (upfront_ope_paid_temptran (t.tran_code), 0) paid_brok, 
         NVL ((SELECT SUM (NVL (amt, 0)) 
                 FROM payment_detail 
                WHERE tran_code = t.tran_code), 0) paidamt, '' asr, payment_mode, 
         (SELECT investor_name FROM investor_master WHERE inv_code = t.client_code) inv, 
         (SELECT MAX (client_name) FROM client_master WHERE client_code = t.source_code) client, 
         exist_code AS existcode, address1 add1, address2 add2, '' loc, 
         pincode pin, 
         (SELECT MAX (city_name) 
            FROM city_master 
           WHERE city_id = (SELECT MAX (city_id) 
                              FROM client_master 
                             WHERE client_code = t.source_code)) ccity, 
         mobile ph, email, 0 arn, '' subbroker, 
         (SELECT rm_name 
            FROM employee_master 
           WHERE payroll_id = TO_CHAR (t.business_rmcode) 
             AND (TYPE = 'A' OR TYPE IS NULL)) rname, 
         (SELECT payroll_id 
            FROM employee_master 
           WHERE payroll_id = TO_CHAR (t.business_rmcode) 
             AND (TYPE = 'A' OR TYPE IS NULL)) rcode, 
         (SELECT branch_name FROM branch_master WHERE branch_code = t.busi_branch_code) bname, 
         (SELECT address1 FROM branch_master WHERE branch_code = t.busi_branch_code) badd1, 
         (SELECT address2 FROM branch_master WHERE branch_code = t.busi_branch_code) badd2, 
         (SELECT phone FROM branch_master WHERE branch_code = t.busi_branch_code) bphone, 
         (SELECT location_name FROM location_master WHERE location_id = 
              (SELECT location_id FROM branch_master WHERE branch_code = t.busi_branch_code)) bloc, 
         (SELECT city_name FROM city_master WHERE city_id = 
              (SELECT city_id FROM branch_master WHERE branch_code = t.busi_branch_code)) bcity, 
         (SELECT iss_name FROM iss_master WHERE iss_code = t.mut_code 
             AND iss_code NOT IN (SELECT DISTINCT iss_code FROM iss_master 
                                       WHERE prod_code IN (SELECT prod_code FROM product_master 
                                                          WHERE nature_code = 'NT004'))) compmf, 
         'Bajaj Capital Limited' compgroup, 
         (SELECT longname FROM other_product WHERE osch_code = t.sch_code) schmf, 
         (SELECT short_name FROM scheme_info WHERE sch_code = t.sch_code) sschmf, ('38387') userid 
    FROM transaction_ST t, client_master b    
    WHERE t.source_code = b.client_code
          AND TRUNC(tr_date) = TO_DATE('{effective_date}', 'DD-MON-YYYY')
          AND tran_code = '{MyTranCode}'
          AND source_code = '{MySourceId}'
          AND business_rmcode = '{txtbusicode}'
";

                // Function call to execute SQL
                DataTable dt3 = CurrentSql(sqlQuery);


                //DataTable reportData = new WM.Controllers.AccountOpeningController().GetARReport(MyTranCode, MyPrintSourceId, MyTrDate, txtBusinessCode.Text, MySourceId);
                 
                if (dt3.Rows.Count > 0)
                {
                    DataRow row1 = dt3.Rows[0];
                     
                    LoadReportData(dt3);
                    CallPrintDiv(); 
                    lblMessage.Text = "Report data retrieved successfully.";
                    lblMessage.CssClass = "message-label-success"; 
                }
                else
                { 
                    ShowAlert("No data found for the provided criteria.");
                     
                }
            }
            catch (Exception ex)
            { 
                string errorMessage = $"Error occurred: {ex.Message}";
                ShowAlert(errorMessage);
                 
            }
        }


        public DataTable GetARMFADVView(string mySourceId, DateTime myTrDate, DateTime serverDateTime, string txtBusiCode, string myTranCode = null, string myPrintSourceId = null)
        {
            ;

            string sq1 = "SELECT source, branch_name, rm_name, rm_code " +
             "FROM employee_master e, branch_master b " +
             "WHERE B.BRANCH_CODE IN ('" + psm_controller.LogBranches() + "') " +
             "AND E.payroll_id = '" + txtBusinessCode.Text + "' " +
             "AND e.source = b.branch_code " +
             "AND (e.type = 'A' OR e.type IS NULL) " +
             "AND e.category_id IN ('2001', '2018')";

            DataTable  currentBrRm = CurrentSql(sq1);


            if (string.IsNullOrEmpty(myTranCode))
            {
                throw new ArgumentException("AR Cannot Be Printed Right Now. Please Generate The AR.");
            }

            if (!string.IsNullOrEmpty(myPrintSourceId))
            {
                mySourceId = myPrintSourceId;
            }

            string sql = @"
        CREATE OR REPLACE VIEW ARMFADV AS 
        SELECT 
            b.client_code, 
            'P' ar_type, 
            t.tran_code, 
            tr_date, 
            cheque_date, 
            cheque_no, 
            t.bank_name, 
            amount, 
            b.client_code source_code, 
            app_no, 
            NVL(upfront_ope_paid_temptran(t.tran_code), 0) paid_brok, 
            NVL((SELECT SUM(NVL(amt, 0)) FROM payment_detail WHERE tran_code = t.tran_code), 0) paidamt, 
            '' asr, 
            payment_mode, 
            (SELECT investor_name FROM investor_master WHERE inv_code = t.client_code) inv, 
            (SELECT MAX(client_name) FROM client_master WHERE client_code = t.source_code) client, 
            exist_code AS existcode, 
            address1 add1, 
            address2 add2, 
            '' loc, 
            pincode pin, 
            (SELECT MAX(city_name) FROM city_master WHERE city_id = 
                (SELECT MAX(city_id) FROM client_master WHERE client_code = t.source_code)) ccity, 
            mobile ph, 
            email, 
            0 arn, 
            '' subbroker, 
            (SELECT rm_name FROM employee_master WHERE payroll_id = TO_CHAR(t.business_rmcode) 
                AND (TYPE = 'A' OR TYPE IS NULL)) rname, 
            (SELECT payroll_id FROM employee_master WHERE payroll_id = TO_CHAR(t.business_rmcode) 
                AND (TYPE = 'A' OR TYPE IS NULL)) rcode, 
            (SELECT branch_name FROM branch_master WHERE branch_code = t.busi_branch_code) bname, 
            (SELECT address1 FROM branch_master WHERE branch_code = t.busi_branch_code) badd1, 
            (SELECT address2 FROM branch_master WHERE branch_code = t.busi_branch_code) badd2, 
            (SELECT phone FROM branch_master WHERE branch_code = t.busi_branch_code) bphone, 
            (SELECT location_name FROM location_master WHERE location_id = 
                (SELECT location_id FROM branch_master WHERE branch_code = t.busi_branch_code)) bloc, 
            (SELECT city_name FROM city_master WHERE city_id = 
                (SELECT city_id FROM branch_master WHERE branch_code = t.busi_branch_code)) bcity, 
            (SELECT iss_name FROM iss_master WHERE iss_code = t.mut_code 
                AND iss_code NOT IN (SELECT DISTINCT iss_code FROM iss_master 
                                    WHERE prod_code IN (SELECT prod_code FROM product_master 
                                                        WHERE nature_code = 'NT004'))) compmf, 
            'Bajaj Capital Limited' compgroup, 
            (SELECT longname FROM other_product WHERE osch_code = t.sch_code) schmf, 
            (SELECT short_name FROM scheme_info WHERE sch_code = t.sch_code) sschmf, 
            ('38387') userid 
        FROM transaction_ST t, client_master b 
    ";

            if (!string.IsNullOrEmpty(myPrintSourceId))
            {
                sql += $" WHERE tr_date = '{myTrDate:dd-MMM-yyyy}' AND t.source_code = b.client_code ";
            }
            else
            {
                sql += $" WHERE tr_date = '{serverDateTime:dd-MMM-yyyy}' AND t.source_code = b.client_code ";
            }

            sql += $" AND (asa <> 'C' OR asa IS NULL) ";
            sql += $" AND business_rmcode = {txtBusiCode} ";
            sql += $" AND source_code = {mySourceId} ";
            sql += $" AND tran_code = {myTranCode} ";

            return CurrentSql(sql);
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
                //txtChequeDate.Focus();
                ddlMutualBank.Enabled = true;
                txtChequeDate.Enabled = true;
                txtChequeNo.Enabled = true;

            }
            else if (draft.Checked)
            {
                ChequeLabel.Text = "Draft No <span class='text-danger'>*</span>";
                ChequeDatedLabel.Text = "Draft Dated <span class='text-danger'>*</span>";

                //txtChequeDate.Focus();
                ddlMutualBank.Enabled = true;
                txtChequeDate.Enabled = true;
                txtChequeNo.Enabled = true;

            }

            else if (optCash.Checked)
            {
                ChequeLabel.Text = "";
                ChequeDatedLabel.Text = "";

                ddlMutualBank.Enabled = false;
                txtChequeDate.Enabled = false;
                txtChequeNo.Enabled = false;
            }
      
        }


        #endregion
        protected void cldDOB_TextChanged(object sender, EventArgs e)
        {
            // Validate and calculate age when DOB is changed
            DateTime dob;
            DateTime? dob1 = DateTime.TryParseExact(cldDOB.Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ? (DateTime?)parsedDate : null;

            //if (DateTime.TryParse(cldDOB.Text, out dob))
            if (dob1 != null)

            {
                //int ageInMonths = CalculateAgeInMonths(dob);
                int? ageInMonths = CalculateAgeInMonthsNew(dob1);

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
              //  itfAOGuardianPerson.Attributes.Add("required", "required");
               // itfAOGuardianNationality.Attributes.Add("required", "required");
               // itfAOGuardianPANNO.Attributes.Add("required", "required");
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
                //addfamGuardianName.Attributes.Add("required", "required");
                //addfamGuardianPan.Attributes.Add("required", "required");
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

        private int? CalculateAgeInMonthsNew(DateTime? dob)
        {
            if (!dob.HasValue)
                return null; // Return null if dob is null

            DateTime today = DateTime.Today;
            int months = (today.Year - dob.Value.Year) * 12 + today.Month - dob.Value.Month;

            if (today.Day < dob.Value.Day)
            {
                months--; // Adjust if the current day hasn't reached the birthday yet
            }

            return months;
        }


        protected void chkSameAsMailing_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSameAsMailing.Checked)
            {
                // Debugging: Check if mailing address fields have values
                System.Diagnostics.Debug.WriteLine("Mailing Address 1: " + itfAOMAddress1.Text);
                System.Diagnostics.Debug.WriteLine("Mailing Address 2: " + itfAOMAddress2.Text);

                // Set Permanent address fields
                txtPAddress1.Text = itfAOMAddress1.Text;  // Hardcoded test
                txtPAddress2.Text = itfAOMAddress2.Text;

                ddlPCountryList.SelectedValue = ddlMailingCountryList.SelectedValue;
                ddlPStateList.SelectedValue = ddlMailingStateList.SelectedValue;
                ddlPCityList.SelectedValue = ddlMailingCityList.SelectedValue;
                txtPPin.Text = txtMailingPin.Text;

                // Disable fields
                txtPAddress1.Enabled = false;
                txtPAddress2.Enabled = false;
                ddlPCountryList.Enabled = false;
                ddlPStateList.Enabled = false;
                ddlPCityList.Enabled = false;
                txtPPin.Enabled = false;

                // Debugging: Check if values were set
                System.Diagnostics.Debug.WriteLine("Permanent Address 1 After: " + txtPAddress1.Text);
                System.Diagnostics.Debug.WriteLine("Permanent Address 2 After: " + txtPAddress2.Text);
            }
            else
            {
                // Reset fields
                txtPAddress1.Text = string.Empty;
                txtPAddress2.Text = string.Empty;
                ddlPCountryList.SelectedIndex = 0;
                ddlPStateList.SelectedIndex = 0;
                ddlPCityList.SelectedIndex = 0;
                txtPPin.Text = string.Empty;

                // Enable fields
                txtPAddress1.Enabled = true;
                txtPAddress2.Enabled = true;
                ddlPCountryList.Enabled = true;
                ddlPStateList.Enabled = true;
                ddlPCityList.Enabled = true;
                txtPPin.Enabled = true;
            }

            // Update Panel if inside one
            UpdatePanel pnl = this.Page.FindControl("UpdatePanel1") as UpdatePanel;
            if (pnl != null)
            {
                pnl.Update();
            }
        }


        protected void chkSameAsMailing_CheckedChanged_0(object sender, EventArgs e)
        {
            if (chkSameAsMailing.Checked)
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


                chkSameAsMailing.Focus();

                if (!string.IsNullOrEmpty(ddlPStateList.SelectedValue))
                {
                    ddlPStateList.Enabled = true;
                }
                else
                {
                    ddlPStateList.Enabled = false;

                }
                if (!string.IsNullOrEmpty(ddlPCityList.SelectedValue))
                {
                    ddlPCityList.Enabled = true;
                }
                else
                {
                    ddlPCityList.Enabled = false;
                }

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

                chkSameAsMailing.Focus();

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

                if (ddlMailingCountryList.Items.FindByValue(sourceDropdown.ToString()) != null)
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
            ddladdfamExistingInvestor.Items.Clear();

            DataTable dt = new WM.Controllers.AccountOpeningController().GetInvestorList(src, exist);

            if (dt.Rows.Count > 0)
            {
                #region Adding existing investor in inline grid new
                foreach (GridViewRow row in ngfd_gvClients.Rows)
                {
                    DropDownList ddlExistingInvestorNew = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                    TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");

                    if (ddlExistingInvestorNew != null)
                    {
                        try
                        {
                            string cinv = ddlExistingInvestorNew.Text;
                            //ddlExistingInvestorNew.SelectedIndex = 0;

                            ddlExistingInvestorNew.Items.Clear();
                            ddlExistingInvestorNew.DataSource = dt;
                            ddlExistingInvestorNew.DataTextField = "investor_name"; 
                            ddlExistingInvestorNew.DataValueField = "inv_code";   
                            ddlExistingInvestorNew.DataBind();

                            ddlExistingInvestorNew.Items.Insert(0, new ListItem("Select Investor", ""));
                            ddlExistingInvestorNew.Enabled = true;
                            ddlExistingInvestorNew.SelectedValue = txtInvestorCode.Text;

                        }
                        catch (Exception ex)
                        {
                            //ddlExistingInvestorNew.Items.Insert(0, new ListItem("Getting Error", ""));
                           // ddlExistingInvestorNew.Enabled = false;
                        }
                    }
                    else
                    {
                        ddlExistingInvestorNew.Enabled = false;

                        ddlExistingInvestorNew.Items.Insert(0, new ListItem("INVESTOR NOT EXIST", ""));

                    }
                }
                #endregion
                

                ddladdfamExistingInvestor.DataSource = dt;
                ddladdfamExistingInvestor.DataTextField = "investor_name"; 
                ddladdfamExistingInvestor.DataValueField = "inv_code";   
                ddladdfamExistingInvestor.DataBind();
                ddladdfamExistingInvestor.Items.Insert(0, new ListItem("Select Investor", ""));
                ddladdfamExistingInvestor.Enabled = true;

            }
            else
            {
                // If no records are found, display a "No Investors Found" item
                ddladdfamExistingInvestor.Enabled = true;


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


        protected bool validateOnlyNumericalInput(TextBox targetBox)
        {

            if (!string.IsNullOrEmpty(targetBox.Text) && !int.TryParse(targetBox.Text, out _))
            {
                return true;
            }
            return false;
        }

        protected bool ValidateOnlyDateInput(TextBox targetBox)
        {
            if (!string.IsNullOrEmpty(targetBox.Text) && !DateTime.TryParse(targetBox.Text, out _))
            {
                return true; // Invalid date format
            }
            return false; // Valid date format or empty input
        }

        // Handles the Search Button click
        protected void InvestorListSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate search criteria (optional but recommended)

                if (InvestorDetailsGridView.Rows.Count > 0)
                {
                    InvestorDetailsGridView.DataSource = null;
                    InvestorDetailsGridView.DataBind();
                    InvestorDetailsLabel.Text = string.Empty;

                }

                if (AreAllSearchFieldsEmpty())
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "Warning", "alert('Please enter at least one search criteria!');", true);
                    return;
                }

                if (ValidateOnlyDateInput(InvestorListDOB))
                {
                    InvestorListDOB.Focus();
                    ShowAlert("Investor DOB is not in proper formate");
                    return;
                }

               if(validateOnlyNumericalInput(InvestorListClientCode))
                {
                    InvestorListClientCode.Focus();
                    // If the input is not a valid number, return immediately
                    ShowAlert("Investor Client Code field reqruied only numerical.");
                    return;
                };


                if (validateOnlyNumericalInput(InvestorListAccountCode))
                {
                    InvestorListAccountCode.Focus();
                    // If the input is not a valid number, return immediately
                    ShowAlert("Investor Account Code reqruied only numerical.");
                    return;
                };

                if (validateOnlyNumericalInput(InvestorMobileTextBox))
                {
                    InvestorMobileTextBox.Focus();
                    // If the input is not a valid number, return immediately
                    ShowAlert("Investor Mobile field reqruied only numerical.");
                    return;
                };

                


                #region Vlaues for investors search
                // Trim all string inputs and handle nullable values
                int? branch = string.IsNullOrWhiteSpace(InvestorBranchDropDown.SelectedValue) ? (int?)null : int.Parse(InvestorBranchDropDown.SelectedValue.Trim());
                int? rm = string.IsNullOrWhiteSpace(InvestorListBranchRM.SelectedValue) ? (int?)null : int.Parse(InvestorListBranchRM.SelectedValue.Trim());
                //DateTime? dob = string.IsNullOrWhiteSpace(InvestorListDOB.Text) ? (DateTime?)null : DateTime.Parse(InvestorListDOB.Text.Trim());
                //DateTime? dob = string.IsNullOrWhiteSpace(InvestorListDOB.Text) ? (DateTime?)null : DateTime.ParseExact(InvestorListDOB.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string dob = string.IsNullOrWhiteSpace(InvestorListDOB.Text) ? null : InvestorListDOB.Text.Trim();



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
                    string message = GetTextFieldValue(row, "message");

                    if (message.Contains("ct head") )
                    {
                        string currentAHByInvHead = GetTextFieldValue(row, "client_code");
                        string currentDbMain = GetTextFieldValue(row, "main_code");

                        ResetFormFields1();
                        FillClientDataByAHNum(currentAHByInvHead);

                        if (!string.IsNullOrEmpty(txtSearchClientCode.Text))
                        {

                            ResetInvestorModel1();
                            ClientScript.RegisterStartupScript(this.GetType(), "InvestorSelected", "closeInvestorListModal(); loadInvestorDetails();", true);
                            return;
                        }
                    }
                    else if (message.Contains("inv head"))
                    {
                        string curentINVHead = GetTextFieldValue(row, "client_code");
                        //string currentDbMain = GetTextFieldValue(row, "main_code");

                        ResetFormFields1();
                        //FillClientDataByInvHead(curentINVHead);

                        if (!string.IsNullOrEmpty(ExistingClientCodeInv.Text))
                        {

                            ResetInvestorModel1();
                            ClientScript.RegisterStartupScript(this.GetType(), "InvestorSelected", "closeInvestorListModal(); loadInvestorDetails();", true);
                            return;
                        }
                    }
                    else if (message.Contains("member"))
                    {
                        ShowAlert("The Selected Investor Is Not The Head Of Family,Please Initiate Only Head Of Faimily As Main Investor");
                    }

                    else if (message.Contains("in cm and im"))
                    {
                        FillClientInvestor(investorCode);

                        if (!string.IsNullOrEmpty(txtClientCode.Text))
                        {
                            ResetInvestorModel1();
                            ClientScript.RegisterStartupScript(this.GetType(), "InvestorSelected", "closeInvestorListModal(); loadInvestorDetails();", true);
                            btnSave.Enabled = true;
                            btnUpdate.Enabled = false;
                        }
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
            string currentGuestValue = txtGuestCode.Text.Trim();
            string loginValue = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(currentGuestValue))
            {
                string isValidGC = IS_VALID_GUEST(currentGuestValue);
                if (isValidGC.ToUpper().Contains("PASS"))
                {
                    FetchedByGuestCode(currentGuestValue);
                    //FillClientByGuestCode(currentGuestValue, loginValue);
                }
                else
                {
                    RESET_GUEST_VALUE();
                    ShowAlert(isValidGC);
                    txtGuestCode.Text = currentGuestValue;
                }
            }
            else
            {
                ShowAlert("ENTER GUEST CODE ");
            }
        


    }

        public string IS_VALID_GUEST(string guestCode)
        {
            string returmMsg = "";
            DataTable dt = new AccountOpeningController().GetClientDetailsByGuest(guestCode, Session["LoginId"]?.ToString());
            if (dt.Rows.Count > 0)
            {
                DataRow row =dt.Rows[0];
                returmMsg = GetTextFieldValue(row, "message").ToUpper();
            }
            return returmMsg;
        }

        private void FetchedByGuestCode(string guestCode)
        {
            try
            {
                DataTable dt = new AccountOpeningController().GetClientDetailsByGuest(guestCode, Session["LoginId"]?.ToString());
                int dtRowCount = dt.Rows.Count;

                if (dtRowCount > 0)
                {
                    DataRow row = dt.Rows[0]; 

                    /*
            D.GUEST_CD          AS C1_GUEST_CODE,
            D.GUEST_NAME          AS C1_GUEST_NAME,
            D.MOBILE              AS C1_MOBILE_NO,
            D.EMP_NO              AS C1_EMP_NO,
            D.TELEPHONE         AS C1_TELEPHONE,
            B.SEX               AS C2_GUEST_SEX,
            B.MATRIALST         AS C2_GUEST_MATRIALST,
            CASE 
            WHEN UPPER(B.SEX) = UPPER('Male') THEN 'MR.'
            WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.'
            ELSE 'Mrs.'
            END AS TITLE,
            PITCH_BOOK_NO       AS C2_PITCH_NO,
            RESIADD1            AS C2_ADD1,
            RESIADD2            AS C2_ADD2,
            CITY                AS C2_CITY,
            cm.city_id          AS C2_CITY_ID,
            STATE               AS C2_STATE,
            sm.state_id         AS C2_STATE_CODE,
            sm.country_id       AS C2_COUNTRY_ID,
            RESIPINCODE         AS C2_PIN,
            B.DOB               AS C2_DOB,
            CORESSEMAIL         AS C2_EMAIL
                     */
                    string isPassGuest = GetTextFieldValue(row, "message").ToUpper();
                                         
                    string msg1 = GetTextFieldValue(row, "C1_MESSAGE").ToUpper();
                    string msg2= GetTextFieldValue(row, "C2_MESSAGE").ToUpper();

                    if(msg1 == "1" || msg2 == "1")
                    {
                        SET_FIELD_DATA_GUEST(row, guestCode);
                        txtGuestCode.Focus();
                    }
                    else
                    {
                        ShowAlert(isPassGuest);
                        ClearBusinessCodeFields();
                        PopulateDefaultDropdownValues();
                    }
                    
                }
                else
                {
                    // No data found for the given guest code
                    ResetFormFields1();
                    const string noDataMsg = "No Data!";
                    ShowAlert(noDataMsg);

                    txtGuestCode.Text = guestCode;
                    txtGuestCode.Focus();
                    lblHolderMessage.Text = noDataMsg;
                }


            }
            catch (Exception ex)
            {
                // Catch and display any errors that occur
                string alertMsg  = $"An error occurred while retrieving data: {ex.Message}";
                ShowAlert(alertMsg);
            }
        }

        public void SET_FIELD_DATA_GUEST(DataRow row, string guestCode)
        {
            string isPassGuest = GetTextFieldValue(row, "message").ToUpper();
            string c1Message = GetTextFieldValue(row, "C1_MESSAGE").ToUpper();

            string validateMsg = "VALID DATA IN C1".ToUpper();
            string validAlertMsg = "Guest code is valid".ToUpper();
            string usedCodeMsg = "Guest Code is Duplicate".ToUpper();
            string invalidCodeMsg = "This is not a valid Guest Code".ToUpper();


            string c1GuestCode = GetTextFieldValue(row, "C1_GUEST_CODE");
            string c1GuestName = GetTextFieldValue(row, "C1_GUEST_NAME");
            string c1MobileNo = GetTextFieldValue(row, "C1_MOBILE_NO");
            string c1Telephone = GetTextFieldValue(row, "C1_TELEPHONE");
            string c2Message = GetTextFieldValue(row, "C2_MESSAGE");
            string c2GuestSex = GetTextFieldValue(row, "C2_GUEST_SEX");
            string c2GuestMarital = GetTextFieldValue(row, "C2_GUEST_MATRIALST");
            string c2GuestTitle = GetTextFieldValue(row, "TITLE");
            string c1EmpNo = GetTextFieldValue(row, "C1_EMP_NO");


            string c2PitchNo = GetTextFieldValue(row, "C2_PITCH_NO");
            string c2Add1 = GetTextFieldValue(row, "C2_ADD1");
            string c2Add2 = GetTextFieldValue(row, "C2_ADD2");
            string c2City = GetTextFieldValue(row, "C2_CITY");
            string c2State = GetTextFieldValue(row, "C2_STATE");
            string db_mailing_add_city_value = GetTextFieldValue(row, "C2_CITY_ID");
            string db_mailing_add_state_value = GetTextFieldValue(row, "C2_STATE_CODE");
            string db_mailing_add_count_value = GetTextFieldValue(row, "C2_COUNTRY_ID");


            string c2Pin = GetTextFieldValue(row, "C2_PIN");
            string c2Dob = GetTextFieldValue(row, "C2_DOB");
            string c2Email = GetTextFieldValue(row, "C2_EMAIL");
            // Guest code is valid and business code exists
            PopulateDefaultDropdownValues();

            if (txtBusinessCode.Text != c1EmpNo)
            {

                UpdateBusinessCodeDetails(c1EmpNo);  // Using the EMP_NO as business code
                txtBusinessCode.Text = c1EmpNo;
            }


            // Populate form fields with the extracted values
            txtGuestCode.Text = guestCode;
            lblMessage.Text = validAlertMsg;
            txtBusinessCode.Focus();

            // Optionally, you can also fill in additional information
            txtAccountName.Text = c1GuestName;
            MobileNo.Text = c1MobileNo;
            PhoneResNumber.Text = c1Telephone;
            //ddlAOGender.Text = c2GuestSex;
            itfAOMAddress1.Text = c2Add1;
            itfAOMAddress2.Text = c2Add2;
            //ddlMailingCityList.Text = c2City;
            //ddlMailingStateList.Text = c2State;

            //txtPPin.Text = c2Pin;
            //cldDOB.Text = c2Dob;
            txtEmail.Text = c2Email;


            GetDropDownValue(row, "title", ddlSalutation1);

            ddlAOGender.SelectedValue = MapGender(row["C2_GUEST_SEX"].ToString());
            GetDropDownValue(row, "C2_GUEST_MATRIALST", ddlAOMaritalStatus);
            GetSetDateField(row, "C2_DOB", cldDOB);
            #region Handle Guardian by DOB
            if (!string.IsNullOrEmpty(cldDOB.Text))
            {
                ValidateGuardianByDOB(cldDOB, true);
            }

            #endregion



            string currentDB_MCountryID = db_mailing_add_count_value;
            string currentDB_MStateID = db_mailing_add_state_value;
            string currentDB_MCityID = db_mailing_add_city_value;

            try
            {
                if (ddlMailingCountryList.Items.FindByValue(currentDB_MCountryID) != null)
                {
                    ddlMailingCountryList.SelectedValue = currentDB_MCountryID;
                    try
                    {
                        // Populate the state dropdown based on the selected country
                        PopulateStateDropDownForAddress(Convert.ToInt32(currentDB_MCountryID), ddlMailingStateList);

                        if (ddlMailingStateList.Items.FindByValue(currentDB_MStateID) != null)
                        {
                            ddlMailingStateList.Enabled = true;
                            ddlMailingStateList.SelectedValue = db_mailing_add_state_value;
                            try
                            {
                                // Populate the city dropdown based on the selected state
                                PopulateCityDropDownForAddress(Convert.ToInt32(currentDB_MStateID), ddlMailingCityList);

                                if (ddlMailingCityList.Items.FindByValue(currentDB_MCityID) != null)
                                {
                                    ddlMailingCityList.Enabled = true;
                                    ddlMailingCityList.SelectedValue = db_mailing_add_city_value;
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



            if (ddlMailingCountryList.Items.Count > 0)
            {
                string currentMailingCoutnryName = ddlMailingCountryList.SelectedItem.ToString();

                HandlePinCodeValidation(currentMailingCoutnryName, txtMailingPin);
                try
                {
                    txtMailingPin.Text = c2Pin;
                }
                catch (Exception ex)
                {


                }
            }

        }

        public void RESET_GUEST_VALUE()
        {
            ClearBusinessCodeFields();
            PopulateDefaultDropdownValues();

            lblMessage.Text = string.Empty;
            txtAccountName.Text = string.Empty;
            MobileNo.Text = string.Empty;
            PhoneResNumber.Text = string.Empty;
            ddlAOGender.SelectedIndex = 0;
            txtPAddress1.Text = string.Empty;
            txtPAddress2.Text = string.Empty;
            ddlPCountryList.SelectedIndex = 0;
            try
            {
                ddlMailingCountryList.SelectedIndex = 0;
                ddlMailingStateList.SelectedIndex = 0;
                ddlMailingStateList.Enabled = false;

                ddlMailingCityList.SelectedIndex = 0;
                ddlMailingCityList.Enabled = false;
                txtMailingPin.Text = string.Empty;
            }
            catch (Exception ex)
            {

            }

            txtMailingPin.Text = string.Empty;
            cldDOB.Text = string.Empty;
            txtEmail.Text = string.Empty;
            ddlSalutation1.SelectedIndex = 0;

            ddlAOGender.SelectedIndex = 0;
            ddlAOMaritalStatus.SelectedIndex = 0;

            itfAOGuardianPerson.Enabled = false;
            itfAOGuardianNationality.Enabled = false;
            itfAOGuardianPANNO.Enabled = false;

        }
        private void PopulateDefaultDropdownValues()
        {
            AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
            AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
            AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
            AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
        }

     
        private void ClearBusinessCodeFields()
        {
            txtBusinessCode.Text = string.Empty;
            txtBusinessCodeName.Text = string.Empty;
            txtBusinessCodeBranch.Text = string.Empty;
            txtGuestCode.Text = string.Empty;
        }


       

        protected void txtBusinessCode_TextChanged(object sender, EventArgs e)
        {
            // Call the method that handles the logic for fetching the RM data
            UpdateBusinessCodeDetails(txtBusinessCode.Text.Trim());
        }


        private void UpdateClientBasicDetialsByGuest(string pitchNumber)
        {
            

        }

        private void UpdateBusinessCodeDetails(string businessCode)
        {
            // Create an instance of your controller or service that contains GetRMEmployee
            AccountOpeningController controller = new AccountOpeningController();

            // Fetch the data from the database
            DataTable rmData = controller.GetRMEmployee(businessCode);
            if(rmData.Rows.Count>0)
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

            ddlMutualBank.DataSource = dt1;
            ddlMutualBank.DataTextField = "BANK_NAME";
            ddlMutualBank.DataValueField = "BANK_ID";
            ddlMutualBank.DataBind();
            ddlMutualBank.Items.Insert(0, new ListItem("Select Bank", ""));

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
        public int GetDropDownValue0(DataRow dataRow, string fieldName, DropDownList dropdown)
        {
            try
            {
                // Clear message if it exists
                lblMessage.Text = string.Empty;

                // Check if the field exists in the DataRow
                if (dataRow.Table.Columns.Contains(fieldName))
                {
                    string fieldValue = dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : "-1";

                    // Check if the field value is valid
                    if (fieldValue != "-1")
                    {
                        // Find the item in the dropdown by matching the value case-insensitively
                        ListItem existingItem = dropdown.Items.Cast<ListItem>()
                            .FirstOrDefault(item => item.Value.ToLower() == fieldValue.ToLower()); // Normalize item values to lowercase for comparison

                        if (existingItem != null)
                        {
                            // Select the existing item
                           // dropdown.SelectedItem = existingItem;

                            // Return the index of the matched item
                            return dropdown.Items.IndexOf(existingItem);  // Returns the index of the matched item
                        }
                        else
                        {
                            // If item is not found, add it and select it
                            dropdown.Items.Add(new ListItem(fieldValue, fieldValue));
                            dropdown.SelectedValue = fieldValue;

                            // Return -1 if no match was found but a new item was added
                            return -1;
                        }
                    }

                    // Return -1 if the field value is invalid
                    return -1;
                }
                else
                {
                    // If field does not exist in the DataRow
                    lblMessage.Text = $"Field '{fieldName}' does not exist in the DataRow.";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                // Handle any exception that occurs
                lblMessage.Text = $"Error: {ex.Message}";
                return -1;  // Return -1 in case of an error
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


        private void HandlePinCodeValidation(string selectedCountryName, TextBox txtMailingPin)
        {
            /*
            string selectedCountry =  selectedCountryName.ToString();
            // You can set up the logic here to adjust other controls or perform actions
            if (selectedCountry.ToUpper().Contains("India".ToUpper()))
            {
                // Set the pin code length or other validation properties specific to India
                string msg = "Enter 6-digit PIN code";
                txtMailingPin.MaxLength = 6;
                txtMailingPin.Attributes["placeholder"] = msg;
                txtMailingPin.Attributes["onkeypress"] = "return event.charCode >= 48 && event.charCode <= 57";
                txtMailingPin.Enabled = true;
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
                txtMailingPin.Enabled = true;

            }

            */
        }
     
        
        protected void ddlMailingCountryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message
            if (!string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue.ToString()))
            {

                try
                {
                    HANDLE_MAILING_SCP_BY_C();
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

        public void HANDLE_MAILING_SCP_BY_C()
        {
            int selectedCountryId = Convert.ToInt32(ddlMailingCountryList.SelectedValue);
            string selectedCountryName = ddlMailingCountryList.SelectedItem.ToString();
            //lblMessage.Text = selectedCountryId.ToString();
            if (selectedCountryId > 0) // Check if a valid country is selected
            {
                PopulateStateDropDownForAddress(selectedCountryId, ddlMailingStateList);
                //fillStateList();

                if (ddlMailingStateList.Items.Count > 0)
                {
                    ddlMailingStateList.Enabled = true;
                    ddlMailingStateList.SelectedIndex = 0;
                    ddlMailingStateList.Focus();
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

                HandlePinCodeValidation(selectedCountryName, txtMailingPin);

            }

        }

        protected void ddlMailingStateList_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(ddlMailingStateList.SelectedValue.ToString()))
            {
                lblMessage.Text = "";  // Clear previous message
                try
                {
                    HANDLE_MAILING_SCP_BY_STATE();
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

        public void HANDLE_MAILING_SCP_BY_STATE()
        {
            int selectedStateId = Convert.ToInt32(ddlMailingStateList.SelectedValue); // Convert Value to integer

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



        protected void ddlPCountryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message

            if (!string.IsNullOrEmpty(ddlPCountryList.SelectedValue.ToString()))
            {
                try
                {
                    HANDLE_PERMANENT_SCP_BY_COUNTRY();
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
            else
            {
                if (ddlPStateList.Items.Count > 0)
                {
                    ddlPStateList.SelectedIndex = 0;
                }

                if (ddlPCityList.Items.Count > 0)
                {
                    ddlPCityList.SelectedIndex = 0;
                }
                ddlPStateList.Enabled = false;
                ddlPCityList.Enabled = false;
            }
          
        
        }

        protected void ddlPStateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";  // Clear previous message

            if (!string.IsNullOrEmpty(ddlPStateList.SelectedValue.ToString()))
            {
                try
                {
                    HANDLE_PERMANENT_SCP_BY_STATE();
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
                if (ddlPCityList.Items.Count > 0)
                {
                    ddlPCityList.SelectedIndex = 0;
                }
                ddlPCityList.Enabled = false;

            }


        }

        public void HANDLE_PERMANENT_SCP_BY_COUNTRY()
        {
            int selectedCountryId = Convert.ToInt32(ddlPCountryList.SelectedValue); // Convert Value to integer
            string selectedCountryName = ddlPCountryList.SelectedItem.ToString();
            lblMessage.Text = selectedCountryId.ToString();
            if (selectedCountryId > 0) // Check if a valid country is selected
            {
                // Call function to populate the state dropdown based on the selected country
                PopulateStateDropDownForAddress(selectedCountryId, ddlPStateList);
                //fillStateList();

                if (ddlPStateList.Items.Count > 0)
                {
                    ddlPStateList.Enabled = true;
                    ddlPStateList.SelectedIndex = 0;
                    ddlPStateList.Focus();
                }
                else
                {
                    ddlPStateList.Items.Insert(0, new ListItem("Select State", ""));
                    ddlPStateList.SelectedIndex = 0;
                    ddlPStateList.Enabled = false;
                }
                if (ddlPCityList.Items.Count > 0)
                {
                    ddlPCityList.SelectedIndex = 0;
                }
                ddlPCityList.Enabled = false;


                HandlePinCodeValidation(selectedCountryName, txtPPin);
            }


            // Register items for event validation
            foreach (ListItem item in ddlPStateList.Items)
            {
                ClientScript.RegisterForEventValidation(ddlPStateList.UniqueID, item.Value);
            }
        }

        public void HANDLE_PERMANENT_SCP_BY_STATE()
        {
            int selectedStateId = Convert.ToInt32(ddlPStateList.SelectedValue); // Convert Value to integer

            if (selectedStateId > 0) // Check if a valid state is selected
            {
                // Call function to populate the city dropdown based on the selected state
                PopulateCityDropDownForAddress(selectedStateId, ddlPCityList);
                //fillCityList();
                if (ddlPCityList.Items.Count > 0)
                {
                    ddlPCityList.SelectedIndex = 0;
                    ddlPCityList.Enabled = true;
                    ddlPCityList.Focus();
                }
                else
                {
                    ddlPCityList.Items.Add(new ListItem("Select City", "0"));
                    ddlPCityList.SelectedIndex = 0;
                    ddlPCityList.Enabled = false;
                }



            }

            // Register items for event validation
            foreach (ListItem item in ddlPCityList.Items)
            {
                ClientScript.RegisterForEventValidation(ddlPCityList.UniqueID, item.Value);
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

        private string ReturnDateString(string columnName)
        {
            if (System.DateTime.TryParse(columnName, out System.DateTime parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                return string.Empty;
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
            //Response.Redirect(Request.RawUrl);
            Response.Redirect("AccountOpening.aspx");

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
            chkSameAsMailing.Checked = false;
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
            chkSameAsMailing.Checked = false;
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
            chkSameAsMailing.Checked = false;
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


        private int GetMatchedIndex(DropDownList ddl, string value)
        {
            value = value.ToUpper(); // Convert to uppercase for consistency

            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Value.ToUpper() == value)
                {
                    return i; // Return matched index
                }
            }

            return 0; // Return index 0 if no match found
        }

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

            ddlMailingCityList.DataSource = dt1;
            ddlMailingCityList.DataTextField = "CITY_NAME";
            ddlMailingCityList.DataValueField = "CITY_ID";
            ddlMailingCityList.DataBind();
            ddlMailingCityList.Items.Insert(0, new ListItem("Select City", ""));



            DataTable dt2 = new WM.Controllers.AccountOpeningController().GetCityList();

            ddlPCityList.DataSource = dt2;
            ddlPCityList.DataTextField = "CITY_NAME";
            ddlPCityList.DataValueField = "CITY_ID";
            ddlPCityList.DataBind();
            ddlPCityList.Items.Insert(0, new ListItem("Select City", ""));



            DataTable dt3 = new WM.Controllers.AccountOpeningController().GetCityList();

            InvestorCityDropDown.DataSource = dt3;
            InvestorCityDropDown.DataTextField = "CITY_NAME";
            InvestorCityDropDown.DataValueField = "CITY_ID";
            InvestorCityDropDown.DataBind();
            InvestorCityDropDown.Items.Insert(0, new ListItem("Select City", ""));


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



            DataTable dt2 = new WM.Controllers.AccountOpeningController().GetStateList();

            ddlPStateList.DataSource = dt2;
            ddlPStateList.DataTextField = "STATE_NAME";
            ddlPStateList.DataValueField = "STATE_ID";
            ddlPStateList.DataBind();
            ddlPStateList.Items.Insert(0, new ListItem("Select State", ""));

        }
        #endregion


        #region AddDefaultItem




        #endregion


        protected void ExitButton_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/welcome.aspx");
            string baseUrl = "https://wealthmaker.in/login_new.aspx";
            if (Session["LoginId"] == null)
            {
                Response.Redirect($"{baseUrl}");
                //Response.Redirect("~/index.aspx");
            }
            else
            {
                string loginId = Session["LoginId"]?.ToString();
                string roleId = Session["roleId"]?.ToString();
                Response.Redirect($"~/welcome?loginid={loginId}&roleid={roleId}");

            }
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


        public long ParseLongNumberMMI(string textBox)
        {


            return long.TryParse(textBox.Trim(), out long parsedNumber) ? parsedNumber : 0;
        }

        private void SetDropdownValueKYC(DataRow row, string columnName, DropDownList dropdown)
        {
            try
            {
                if (row[columnName] != DBNull.Value)
                {
                    string value = row[columnName].ToString().Trim().ToUpper();
                    if (value.Length > 0)
                    {

                    string foundVlaue = "";
                        if(value == "N")
                        {
                            foundVlaue = "NO";
                        }
                        else if (value == "Y")
                        {
                            foundVlaue = "YES";
                        }
                        else
                        {
                            foundVlaue = "";
                        }
                        dropdown.SelectedValue = foundVlaue; // Set first character only
                    }
                }
            }
            catch
            {
                // Handle or log exception if necessary
            }
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
                    BindInitialGrid(familyData);

                    // Data exists, bind to GridView
                    familyGridView.Visible = true;
                    familyGridView.DataSource = familyData;
                    familyGridView.DataBind();
                }
                catch (Exception ex)
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
            string IS_PASS_MEM_VALIDATION = AddMemberValidate();
            if (IS_PASS_MEM_VALIDATION.ToUpper().Contains("PASS"))
            {
                ADD_MEMBERS_IN_GRID();
                ShowAlert("Member is valid to insert");
                //lblMessageAddFam.Text = insertResult;
                ResetFamSectionOnUPdate();
                //addfamGuardianName.Attributes.Remove("required");
                //addfamGuardianPan.Attributes.Remove("required");

                //string famSrcId = txtHeadSourceCode.Text;
                //string famExistID = txtClientCode.Text;
                //HandleFamilyData(famSrcId, famExistID);
                AddMemberButton.Enabled = true;
                UpdateMemberButton.Enabled = false;
            }
            else
            {
                ShowAlert(IS_PASS_MEM_VALIDATION);
            }

        }

        protected string AddMemberValidate()
        {
            string message = "";
            #region NULL FIELD VALIDATION
            if (string.IsNullOrEmpty(txtSearchClientCode.Text.Trim()))
            {
                return "First load family head data then insert";
                
            }

            if (string.IsNullOrEmpty(ddlSalutation3.Text))
            {
                ddlSalutation3.Focus();
                return "Member title is requried";
                 
            }

            if (string.IsNullOrEmpty(addfamInvestorName.Text))
            {
                addfamInvestorName.Focus();
                return "Member name is requried";
                
            }

            if (string.IsNullOrEmpty(addfamGender.SelectedValue.ToString()))
            {
                addfamGender.Focus();
                return "Member gender is requried";
                 
            }

            if (string.IsNullOrEmpty(addfamMobile.Text))
            { 
                addfamMobile.Focus();
                return "Member mobile is requried";
            }
            if (string.IsNullOrEmpty(addfamDOB.Text))
            {
                addfamDOB.Focus();
                return "Member DOB is requried";

            }

            if (addfamGuardianName.Enabled && string.IsNullOrEmpty(addfamGuardianName.Text))
            {
                addfamGuardianName.Focus();
                return "Guardian name is requried";

            }

            if (string.IsNullOrEmpty(addfamOccupation.Text))
            {
                addfamOccupation.Focus();
                return "Member Occupation is requried";

            }

            if (string.IsNullOrEmpty(addfamRelation.Text))
            {
                addfamRelation.Focus();
                return "Member Relation is requried";

            }


            if (ValidateMobileFieldMinLength(addfamMobile))
            {
                addfamMobile.Focus();
                return message;
            }

            #endregion

            else
            {
                string in_v_inv = ddladdfamExistingInvestor.SelectedValue;
                string in_v_exist = txtClientCode.Text;
                string in_v_log = Session["LoginId"]?.ToString();
                string in_v_client_main = txtSearchClientCode.Text;
                string in_v_mob = addfamMobile.Text;
                string in_v_pan = addfamPan.Text;
                string in_v_email = addfamEmail.Text;
                string in_v_g_pan = addfamGuardianPan.Text;
                string in_v_aadhar = addfamAadharNumber.Text.Trim();

                //InsertMember();
                DataTable memInValidation = new AccountOpeningController().ValidateAndInsertInvestor(in_v_exist, in_v_mob, in_v_pan, in_v_email, in_v_g_pan, in_v_aadhar);

                int mmInVRowCount = memInValidation.Rows.Count;
                if (mmInVRowCount > 0)
                {
                    DataRow row = memInValidation.Rows[0];
                    string msg = GetTextFieldValue(row, "message");
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        message = msg;
                    }
                }
            }
            return message;
        }

        protected void gvFamilyMembers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //int id = Convert.ToInt32(newMemGrid_IL_familyGridView.DataKeys[e.RowIndex].Value);
            //DataRow row = newMemGrid_IL_familyGridView.Rows.f//Find(id);
            //if (row != null)
            //{
            //    newMemGrid_IL_familyGridView.Rows.Remove(row);
            //}
            //BindGrid();
        }

        protected void ADD_MEMBERS_IN_GRID()
        {
            string nfds_name = "new_mem_list_to_insert_1";
            DataTable dt;

            // Step 1: Retrieve or Initialize DataTable from Session
            if (Session[nfds_name] != null)
            {
                dt = (DataTable)Session[nfds_name]; // Retrieve existing DataTable
            }
            else
            {
                dt = new DataTable();
                //dt.Columns.Add("inv_code", typeof(string));
                dt.Columns.Add("investor_title", typeof(string));
                dt.Columns.Add("investor_name", typeof(string));
                dt.Columns.Add("gender", typeof(string));
                dt.Columns.Add("mobile", typeof(string));
                dt.Columns.Add("email", typeof(string));
                dt.Columns.Add("pan", typeof(string));
                dt.Columns.Add("dob", typeof(string));

                dt.Columns.Add("OCC_NAME", typeof(string));
                dt.Columns.Add("OCC_ID", typeof(string));
                dt.Columns.Add("OUR_RELATIONSHIP", typeof(string));
                dt.Columns.Add("OUR_RELATIONSHIP_ID", typeof(string));
                dt.Columns.Add("KYC", typeof(string));
                dt.Columns.Add("g_name", typeof(string));
                dt.Columns.Add("g_pan", typeof(string));
                dt.Columns.Add("approved", typeof(string));
                dt.Columns.Add("AADHAR_CARD_NO", typeof(string));
                dt.Columns.Add("is_nominee", typeof(string));
                dt.Columns.Add("nominee_per", typeof(string));

                // Only extract data if Session is empty
                foreach (GridViewRow row in newMemGrid_IL_familyGridView.Rows)
                {
                    DataRow dr = dt.NewRow();

                   // dr["inv_code"] = ((Label)row.FindControl("newMemGrid_IL_lblInvCode"))?.Text.Trim() ?? string.Empty;
                    dr["investor_title"] = ((Label)row.FindControl("newMemGrid_IL_lblInvestorTitle"))?.Text.Trim() ?? string.Empty;
                    dr["investor_name"] = ((Label)row.FindControl("newMemGrid_IL_lblInvestorName"))?.Text.Trim() ?? string.Empty;
                    dr["gender"] = ((Label)row.FindControl("newMemGrid_IL_lblGender"))?.Text.Trim() ?? string.Empty;
                    dr["mobile"] = ((Label)row.FindControl("newMemGrid_IL_lblMobile"))?.Text.Trim() ?? string.Empty;
                    dr["email"] = ((Label)row.FindControl("newMemGrid_IL_lblEmail"))?.Text.Trim() ?? string.Empty;
                    dr["pan"] = ((Label)row.FindControl("newMemGrid_IL_lblPan"))?.Text.Trim() ?? string.Empty;
                    dr["dob"] = ((Label)row.FindControl("newMemGrid_IL_lblDOB"))?.Text.Trim() ?? string.Empty;

                    dr["OCC_NAME"] = ((Label)row.FindControl("newMemGrid_IL_lblOccupation"))?.Text.Trim() ?? string.Empty;
                    dr["OCC_ID"] = ((Label)row.FindControl("newMemGrid_IL_lblOccupationID"))?.Text.Trim() ?? string.Empty;
                    dr["OUR_RELATIONSHIP"] = ((Label)row.FindControl("newMemGrid_IL_lblRelation"))?.Text.Trim() ?? string.Empty;
                    dr["OUR_RELATIONSHIP_ID"] = ((Label)row.FindControl("newMemGrid_IL_lblRelationID"))?.Text.Trim() ?? string.Empty;
                    dr["KYC"] = ((Label)row.FindControl("newMemGrid_IL_lblKYC"))?.Text.Trim() ?? string.Empty;
                    dr["g_name"] = ((Label)row.FindControl("newMemGrid_IL_lblGuardianName"))?.Text.Trim() ?? string.Empty;
                    dr["g_pan"] = ((Label)row.FindControl("newMemGrid_IL_lblGuardianPan"))?.Text.Trim() ?? string.Empty;
                    dr["approved"] = ((Label)row.FindControl("newMemGrid_IL_lblApproved"))?.Text.Trim() ?? string.Empty;
                    dr["AADHAR_CARD_NO"] = ((Label)row.FindControl("newMemGrid_IL_lblAadhar"))?.Text.Trim() ?? string.Empty;
                    dr["is_nominee"] = ((Label)row.FindControl("newMemGrid_IL_lblIsNominee"))?.Text.Trim() ?? string.Empty;
                    dr["nominee_per"] = ((Label)row.FindControl("newMemGrid_IL_lblNomineePer"))?.Text.Trim() ?? string.Empty;
                    
                    
                    dt.Rows.Add(dr);
                }

                Session[nfds_name] = dt; // Save extracted data to session


            }

            // Step 2: Retrieve New Row Data from UI Fields
            DataRow newRow = dt.NewRow();
           // newRow["inv_code"] = ddladdfamExistingInvestor.SelectedValue;
            newRow["investor_title"] = ddlSalutation3.SelectedValue;
            newRow["investor_name"] = addfamInvestorName.Text.Trim();
            newRow["gender"] = addfamGender.SelectedValue;
            newRow["mobile"] = addfamMobile.Text.Trim();
            newRow["email"] = addfamEmail.Text.Trim();
            newRow["pan"] = addfamPan.Text.Trim();
            newRow["dob"] = addfamDOB.Text.Trim();

            newRow["OUR_RELATIONSHIP_ID"] = addfamRelation.SelectedValue;
            newRow["OUR_RELATIONSHIP"] = (string.IsNullOrEmpty(addfamRelation.SelectedValue)? null : addfamRelation.SelectedItem.Text);
            newRow["OCC_ID"] = addfamOccupation.SelectedValue;
            newRow["OCC_NAME"] = (string.IsNullOrEmpty(addfamOccupation.SelectedValue) ? null : addfamOccupation.SelectedItem.Text);


            newRow["KYC"] = addfamKYC.SelectedValue;
            newRow["g_name"] = addfamGuardianName.Text.Trim();
            newRow["g_pan"] = addfamGuardianPan.Text.Trim();
            newRow["approved"] = addfamApproved.SelectedValue;
            newRow["AADHAR_CARD_NO"] = addfamAadharNumber.Text.Trim();
            newRow["is_nominee"] = addfamIsNominee.SelectedValue;
            newRow["nominee_per"] = addfamAllocationPercentage.Text.Trim();

            dt.Rows.Add(newRow);

            // Step 4: Store Updated DataTable in Session & Bind to GridView
            Session[nfds_name] = dt;
            newMemGrid_IL_familyGridView.DataSource = dt;
            newMemGrid_IL_familyGridView.DataBind();
            newMemGrid_IL_familyGridView.Visible = true;

            if (lblAddingNewMemList.Visible == false)
            {
                lblAddingNewMemList.Visible = true;
            }
        }

        protected void UpdateMemberButton_Click(object sender, EventArgs e)
        {
            //string returnV = ValidateRequiredFieldsForFirstValidRow(ngfd_gvClients);
            //if (!string.IsNullOrEmpty(returnV))
            //{
            //    ShowAlert(returnV);
            //}
            //else
            //{
            //    // Show a message that no valid rows exist
            //    string msg = "No valid rows found for insertion.";
            //    ShowAlert(msg);
            //}

            /* get valid rows
            int validRows = GetValidRowCount(ngfd_gvClients);

            if (validRows > 0)
            {
                // Proceed with insertion logic only for valid rows
                //InsertValidRows();
                ShowAlert(validRows.ToString());
            }
            else
            {
                // Show a message that no valid rows exist
                string msg = "No valid rows found for insertion.";
                ShowAlert(msg);
            }
            */

            /* Its fuciont 
            string famClientCode = ddladdfamExistingInvestor.SelectedValue;
            if (!string.IsNullOrEmpty(famClientCode))
            {
            UPDATE_MEMBER_BY_INV(famClientCode);
            }
            //if (ValidateMobileFieldMinLength(addfamMobile)) return;
            //InsertMember();
           */
        }

        protected void ResetMemberButton_Click(object sender, EventArgs e)
        {
            ResetAddFamForm();

            if (newMemGrid_IL_familyGridView.Rows.Count > 0)
            {
                Session["new_mem_list_to_insert_1"] = null;
                newMemGrid_IL_familyGridView.Visible = false;
                newMemGrid_IL_familyGridView.DataSource = null;
                newMemGrid_IL_familyGridView.DataBind();
            }

        }

        public static DateTime? ConvertToDateExactPrsed(string dateString)
        {

            if (string.IsNullOrWhiteSpace(dateString))
                return null; // Return null if input is null or empty

            DateTime dob;
            string dateFormat = "dd/MM/yyyy"; // Expected date format
            CultureInfo provider = CultureInfo.InvariantCulture;

            // Try parsing the input date
            if (DateTime.TryParseExact(dateString, dateFormat, provider, DateTimeStyles.None, out dob))
            {
                return dob;
            }

            return null; // Return null if parsing fails
        }

        public static DateTime? ConvertToDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null; // Return null if input is null or empty

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out DateTime parsedDate))
            {
                return parsedDate;
            }

            return null; // Return null if parsing fails
        }


        private string ValidateRequiredFieldsForFirstValidRow(GridView gridView)
        {
            string returnmag = "";
            int validRowCount = GetValidRowCount(gridView);

            if (validRowCount == 0) return "No valid rows found.";

            int validRowIndex = 0; // To count valid rows
            int rowIndex = 1; // UI row number (starting from 1 for display)

            foreach (GridViewRow row in gridView.Rows)
            {
                DropDownList ddlExistingInvestor = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
                TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
                TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
                TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
                TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
                TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
                TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
                DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
                TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
                DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
                DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
                DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
                DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
                DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
                TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");

                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = txtSearchClientCode.Text;

                string famSelectedInv = ddlExistingInvestor.SelectedValue;
                string famInv = txtInvestorCode.Text;
                string selectedSalutation1 = ddlSalutation.SelectedValue;
                string accountName = txtInvestorName.Text;
                string famMobile = txtMobile.Text;
                string famEmail = txtEmail.Text;
                string famPan = txtPAN.Text;
                string famDOB_1 = txtDOB.Text;
                string famRel_1 = ddlRelation.SelectedValue;
                string famGName = txtGuardianName.Text;
                string famGPan = txtGuardianPAN.Text;
                string famOcc_1 = ddlOccupation.SelectedValue;
                string famKYC = ddlKyc.SelectedValue;
                string famApprove = ddlApproved.SelectedValue;
                string famGender = ddlGender.SelectedValue;
                string famNom = ddlIsNominee.SelectedValue;
                string famAllo_1 = txtAllocation.Text;
                string AadharValue_1 = txtAadhar.Text;


                DateTime? famDOB = ConvertToDateExactPrsed(famDOB_1);

                int famRel = ParseInt(famRel_1);
                int famOcc = ParseInt(famOcc_1);
                string famDOB_1aa = "14/02/2025"; // Input string in dd/MM/yyyy format

                DateTime? famDOBaa = ConvertToDate(famDOB_1aa);

                DateTime? famDOBaaa = ConvertToDate(famDOB_1);
                string isdobGValidated = ValidateAgeReturnGValidation(txtDOB.Text, txtGuardianPAN, txtGuardianName);


                double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);
                long AadharValue = ParseLongNumberMMI(AadharValue_1);


                // Check if the row contains any valid data
                bool hasData = (ddlSalutation != null && !string.IsNullOrEmpty(ddlSalutation.SelectedValue)) ||
                               (txtInvestorName != null && !string.IsNullOrEmpty(txtInvestorName.Text.Trim())) ||
                               (txtMobile != null && !string.IsNullOrEmpty(txtMobile.Text.Trim())) ||
                               (txtDOB != null && !string.IsNullOrEmpty(txtDOB.Text.Trim())) ||
                               (ddlGender != null && !string.IsNullOrEmpty(ddlGender.SelectedValue)) ||
                               (ddlOccupation != null && !string.IsNullOrEmpty(ddlOccupation.SelectedValue)) ||
                               (ddlRelation != null && !string.IsNullOrEmpty(ddlRelation.SelectedValue));

                if (hasData)
                {
                    validRowIndex++; // Increment valid row count

                    List<string> missingFields = new List<string>();

                    if (ddlSalutation == null || string.IsNullOrEmpty(ddlSalutation.SelectedValue)) missingFields.Add("Title");
                    if (txtInvestorName == null || string.IsNullOrEmpty(txtInvestorName.Text.Trim())) missingFields.Add("Investor Name");
                    if (txtMobile == null || string.IsNullOrEmpty(txtMobile.Text.Trim())) missingFields.Add("Mobile Number");
                    if (txtDOB == null || string.IsNullOrEmpty(txtDOB.Text.Trim())) missingFields.Add("Date of Birth (DOB)");
                    if (ddlGender == null || string.IsNullOrEmpty(ddlGender.SelectedValue)) missingFields.Add("Gender");
                    if (ddlOccupation == null || string.IsNullOrEmpty(ddlOccupation.SelectedValue)) missingFields.Add("Occupation");
                    if (ddlRelation == null || string.IsNullOrEmpty(ddlRelation.SelectedValue)) missingFields.Add("Relation");

                    if (missingFields.Count > 0)
                    {
                        return $"Member Row {rowIndex}: Missing field(s) -> {string.Join(", ", missingFields)}";
                    }

                    else if (!string.IsNullOrEmpty(isdobGValidated))
                    {
                        return $"Member Row {rowIndex}: Missing field(s) -> {string.Join(", ", isdobGValidated)}";

                    }

                    else
                    {

                        string IsValidatedMember = new WM.Controllers.AccountOpeningController().M_MULTI_VALIDATION(
                        #region MM IN VALUES
    dtNumberValue,
    existClientCodeValue,
    famInv,
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
                        #endregion
                    );

                        bool isSuccessMemInsert = IsValidatedMember.ToUpper().Contains("PASS") ? true : false;
                        bool isDuplicateMemInsert = IsValidatedMember.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                        if (!isSuccessMemInsert)
                        {

                             return $"Member Row {rowIndex}: {string.Join(", ", IsValidatedMember)}";;
                        }
                        else 
                        {
                            returnmag =  "VALID MEMBER DATA";
                        }


                    }

                    // Stop checking after reaching the valid row count
                    if (validRowIndex >= validRowCount) break;
                }
                rowIndex++; // Increment the display row index
            }
            
            
            return "MEMBER VALIDATION PASS";
        }

        private string MemberInsertUpdateOnHeadUpdate(GridView gridView)
        {
            string returnmag = "";
            int validRowCount = GetValidRowCount(gridView);

            if (validRowCount == 0) return "No valid rows found.";

            int validRowIndex = 0; // To count valid rows
            int rowIndex = 1; // UI row number (starting from 1 for display)

            foreach (GridViewRow row in gridView.Rows)
            {
                DropDownList ddlExistingInvestor = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
                TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
                TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
                TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
                TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
                TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
                TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
                DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
                TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
                DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
                DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
                DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
                DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
                DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
                TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");

                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text); // head inv
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = txtSearchClientCode.Text; // head ah 

                string famSelectedInv = ddlExistingInvestor.SelectedValue;
                string famInv = txtInvestorCode.Text;
                string selectedSalutation1 = ddlSalutation.SelectedValue;
                string accountName = txtInvestorName.Text;
                string famMobile = txtMobile.Text;
                string famEmail = txtEmail.Text;
                string famPan = txtPAN.Text;
                string famDOB_1 = txtDOB.Text;
                string famRel_1 = ddlRelation.SelectedValue;
                string famGName = txtGuardianName.Text;
                string famGPan = txtGuardianPAN.Text;
                string famOcc_1 = ddlOccupation.SelectedValue;
                string famKYC = ddlKyc.SelectedValue;
                string famApprove = ddlApproved.SelectedValue;
                string famGender = ddlGender.SelectedValue;
                string famNom = ddlIsNominee.SelectedValue;
                string famAllo_1 = txtAllocation.Text;
                string AadharValue_1 = txtAadhar.Text;


                DateTime? famDOB = ConvertToDateExactPrsed(famDOB_1);
                int famRel = ParseInt(famRel_1);
                int famOcc = ParseInt(famOcc_1);
                double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);
                long AadharValue = ParseLongNumberMMI(AadharValue_1);


                // Check if the row contains any valid data
                bool hasData = (ddlSalutation != null && !string.IsNullOrEmpty(ddlSalutation.SelectedValue)) ||
                               (txtInvestorName != null && !string.IsNullOrEmpty(txtInvestorName.Text.Trim())) ||
                               (txtMobile != null && !string.IsNullOrEmpty(txtMobile.Text.Trim())) ||
                               (txtDOB != null && !string.IsNullOrEmpty(txtDOB.Text.Trim())) ||
                               (ddlGender != null && !string.IsNullOrEmpty(ddlGender.SelectedValue)) ||
                               (ddlOccupation != null && !string.IsNullOrEmpty(ddlOccupation.SelectedValue)) ||
                               (ddlRelation != null && !string.IsNullOrEmpty(ddlRelation.SelectedValue));

                if (hasData)
                {
                    validRowIndex++; // Increment valid row count

                    List<string> missingFields = new List<string>();

                    if (ddlSalutation == null || string.IsNullOrEmpty(ddlSalutation.SelectedValue)) missingFields.Add("Title");
                    if (txtInvestorName == null || string.IsNullOrEmpty(txtInvestorName.Text.Trim())) missingFields.Add("Investor Name");
                    if (txtMobile == null || string.IsNullOrEmpty(txtMobile.Text.Trim())) missingFields.Add("Mobile Number");
                    if (txtDOB == null || string.IsNullOrEmpty(txtDOB.Text.Trim())) missingFields.Add("Date of Birth (DOB)");
                    if (ddlGender == null || string.IsNullOrEmpty(ddlGender.SelectedValue)) missingFields.Add("Gender");
                    if (ddlOccupation == null || string.IsNullOrEmpty(ddlOccupation.SelectedValue)) missingFields.Add("Occupation");
                    if (ddlRelation == null || string.IsNullOrEmpty(ddlRelation.SelectedValue)) missingFields.Add("Relation");

                    if (missingFields.Count > 0)
                    {
                        return $"Member Row {rowIndex}: Missing field(s) -> {string.Join(", ", missingFields)}";
                    }


                    else
                    {
                        if (string.IsNullOrEmpty(famInv))
                        {


                        string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
                        #region MM IN VALUES
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
                        #endregion
            );

                        bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false;
                        bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;
                            if (!isSuccessMemInsert)
                            {
                                ShowAlert(insertResult);
                            }
                        }
                        else
                        {
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
famInv,
AadharValue

                );
                            if (!updateFamResult.Contains("Success"))
                            {
                                ShowAlert(updateFamResult);
                            }
                        }



                    }

                    // Stop checking after reaching the valid row count
                    if (validRowIndex >= validRowCount) break;
                }
                rowIndex++; // Increment the display row index
            }
            return "MEMBER VALIDATION PASS";
        }


        private string MemberInsertOnHeadInsert(GridView gridView, string headAH, string headInv)
        {
            string returnmag = "";
            int validRowCount = GetValidRowCount(gridView);

            if (validRowCount == 0) return "No valid rows found.";

            int validRowIndex = 0; // To count valid rows
            int rowIndex = 1; // UI row number (starting from 1 for display)

            foreach (GridViewRow row in gridView.Rows)
            {
                DropDownList ddlExistingInvestor = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
                TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
                TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
                TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
                TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
                TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
                TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
                DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
                TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
                DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
                DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
                DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
                DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
                DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
                TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");

                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(headInv) ? 0 : Convert.ToInt64(headInv); // txtClientCode.Text
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = headAH; // txtSearchClientCode.Text;

                string famSelectedInv = ddlExistingInvestor.SelectedValue;
                string famInv = txtInvestorCode.Text;
                string selectedSalutation1 = ddlSalutation.SelectedValue;
                string accountName = txtInvestorName.Text;
                string famMobile = txtMobile.Text;
                string famEmail = txtEmail.Text;
                string famPan = txtPAN.Text;
                string famDOB_1 = txtDOB.Text;
                string famRel_1 = ddlRelation.SelectedValue;
                string famGName = txtGuardianName.Text;
                string famGPan = txtGuardianPAN.Text;
                string famOcc_1 = ddlOccupation.SelectedValue;
                string famKYC = ddlKyc.SelectedValue;
                string famApprove = ddlApproved.SelectedValue;
                string famGender = ddlGender.SelectedValue;
                string famNom = ddlIsNominee.SelectedValue;
                string famAllo_1 = txtAllocation.Text;
                string AadharValue_1 = txtAadhar.Text;


                DateTime? famDOB = ConvertToDateExactPrsed(famDOB_1);
                int famRel = ParseInt(famRel_1);
                int famOcc = ParseInt(famOcc_1);
                double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);
                long AadharValue = ParseLongNumberMMI(AadharValue_1);


                // Check if the row contains any valid data
                bool hasData = (ddlSalutation != null && !string.IsNullOrEmpty(ddlSalutation.SelectedValue)) ||
                               (txtInvestorName != null && !string.IsNullOrEmpty(txtInvestorName.Text.Trim())) ||
                               (txtMobile != null && !string.IsNullOrEmpty(txtMobile.Text.Trim())) ||
                               (txtDOB != null && !string.IsNullOrEmpty(txtDOB.Text.Trim())) ||
                               (ddlGender != null && !string.IsNullOrEmpty(ddlGender.SelectedValue)) ||
                               (ddlOccupation != null && !string.IsNullOrEmpty(ddlOccupation.SelectedValue)) ||
                               (ddlRelation != null && !string.IsNullOrEmpty(ddlRelation.SelectedValue));

                if (hasData)
                {
                    validRowIndex++; // Increment valid row count

                    List<string> missingFields = new List<string>();

                    if (ddlSalutation == null || string.IsNullOrEmpty(ddlSalutation.SelectedValue)) missingFields.Add("Title");
                    if (txtInvestorName == null || string.IsNullOrEmpty(txtInvestorName.Text.Trim())) missingFields.Add("Investor Name");
                    if (txtMobile == null || string.IsNullOrEmpty(txtMobile.Text.Trim())) missingFields.Add("Mobile Number");
                    if (txtDOB == null || string.IsNullOrEmpty(txtDOB.Text.Trim())) missingFields.Add("Date of Birth (DOB)");
                    if (ddlGender == null || string.IsNullOrEmpty(ddlGender.SelectedValue)) missingFields.Add("Gender");
                    if (ddlOccupation == null || string.IsNullOrEmpty(ddlOccupation.SelectedValue)) missingFields.Add("Occupation");
                    if (ddlRelation == null || string.IsNullOrEmpty(ddlRelation.SelectedValue)) missingFields.Add("Relation");

                    if (missingFields.Count > 0)
                    {
                        //return $"Member Row {rowIndex}: Missing field(s) -> {string.Join(", ", missingFields)}";
                    }


                    else
                    {
                        if (string.IsNullOrEmpty(famInv))
                        {


                            string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
                            #region MM IN VALUES
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
                            #endregion
                );

                            bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false;
                            bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                            if (!isSuccessMemInsert)
                            {
                                ShowAlert(insertResult);
                            }
                        }
                        



                    }

                    // Stop checking after reaching the valid row count
                    if (validRowIndex >= validRowCount) break;
                }
                rowIndex++; // Increment the display row index
            }
            return "MEMBER VALIDATION PASS";
        }

        private string InsertValiedatedMember_1(GridView gridView)
        {
            string returnmag = "";
            int validRowCount = GetValidRowCount(gridView);

            if (validRowCount == 0) return "No valid rows found.";

            int validRowIndex = 0; // To count valid rows
            int rowIndex = 1; // UI row number (starting from 1 for display)

            foreach (GridViewRow row in gridView.Rows)
            {
                DropDownList ddlExistingInvestor = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
                TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
                TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
                TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
                TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
                TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
                TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
                DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
                TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
                TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
                DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
                DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
                DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
                DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
                DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
                TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");

                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = txtSearchClientCode.Text;

                string famSelectedInv = ddlExistingInvestor.SelectedValue;
                string famInv = txtInvestorCode.Text;
                string selectedSalutation1 = ddlSalutation.SelectedValue;
                string accountName = txtInvestorName.Text;
                string famMobile = txtMobile.Text;
                string famEmail = txtEmail.Text;
                string famPan = txtPAN.Text;
                string famDOB_1 = txtDOB.Text;
                string famRel_1 = ddlRelation.SelectedValue;
                string famGName = txtGuardianName.Text;
                string famGPan = txtGuardianPAN.Text;
                string famOcc_1 = ddlOccupation.SelectedValue;
                string famKYC = ddlKyc.SelectedValue;
                string famApprove = ddlApproved.SelectedValue;
                string famGender = ddlGender.SelectedValue;
                string famNom = ddlIsNominee.SelectedValue;
                string famAllo_1 = txtAllocation.Text;
                string AadharValue_1 = txtAadhar.Text;


                DateTime? famDOB = ConvertToDate(famDOB_1);

                int famRel = ParseInt(famRel_1);
                int famOcc = ParseInt(famOcc_1);
                string famDOB_1aa = "14/02/2025"; // Input string in dd/MM/yyyy format

                DateTime? famDOBaa = ConvertToDate(famDOB_1aa);

                DateTime? famDOBaaa = ConvertToDate(famDOB_1);

                double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);
                long AadharValue = ParseLongNumberMMI(AadharValue_1);


                // Check if the row contains any valid data
                bool hasData = (ddlSalutation != null && !string.IsNullOrEmpty(ddlSalutation.SelectedValue)) ||
                               (txtInvestorName != null && !string.IsNullOrEmpty(txtInvestorName.Text.Trim())) ||
                               (txtMobile != null && !string.IsNullOrEmpty(txtMobile.Text.Trim())) ||
                               (txtDOB != null && !string.IsNullOrEmpty(txtDOB.Text.Trim())) ||
                               (ddlGender != null && !string.IsNullOrEmpty(ddlGender.SelectedValue)) ||
                               (ddlOccupation != null && !string.IsNullOrEmpty(ddlOccupation.SelectedValue)) ||
                               (ddlRelation != null && !string.IsNullOrEmpty(ddlRelation.SelectedValue));

                if (hasData)
                {
                    validRowIndex++; // Increment valid row count

                    List<string> missingFields = new List<string>();

                    if (ddlSalutation == null || string.IsNullOrEmpty(ddlSalutation.SelectedValue)) missingFields.Add("Title (Salutation)");
                    if (txtInvestorName == null || string.IsNullOrEmpty(txtInvestorName.Text.Trim())) missingFields.Add("Investor Name");
                    if (txtMobile == null || string.IsNullOrEmpty(txtMobile.Text.Trim())) missingFields.Add("Mobile Number");
                    if (txtDOB == null || string.IsNullOrEmpty(txtDOB.Text.Trim())) missingFields.Add("Date of Birth (DOB)");
                    if (ddlGender == null || string.IsNullOrEmpty(ddlGender.SelectedValue)) missingFields.Add("Gender");
                    if (ddlOccupation == null || string.IsNullOrEmpty(ddlOccupation.SelectedValue)) missingFields.Add("Occupation");
                    if (ddlRelation == null || string.IsNullOrEmpty(ddlRelation.SelectedValue)) missingFields.Add("Relation");

                    if (missingFields.Count > 0)
                    {
                        return $"Member Row {rowIndex}: Missing field(s) -> {string.Join(", ", missingFields)}";
                    }


                    else
                    {

                        string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
                        #region MM IN VALUES
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
                        #endregion
                    );

                        bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false;
                        bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                        if (!isSuccessMemInsert)
                        {
                            return insertResult;

                        }


                    }

                    // Stop checking after reaching the valid row count
                    if (validRowIndex >= validRowCount) break;
                }
                rowIndex++; // Increment the display row index
            }

            return "No valid rows with missing fields found.";
        }

        private int GetValidRowCount(GridView gridView)
        {
            int validRowCount = 0;

            foreach (GridViewRow row in gridView.Rows)
            {
                DropDownList ddlExistingInvSelected = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
                TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
                TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
                DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
                DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
                DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
                TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");

                bool hasTitle = ddlSalutation != null && !string.IsNullOrEmpty(ddlSalutation.SelectedValue);
                bool hasName = txtInvestorName != null && !string.IsNullOrEmpty(txtInvestorName.Text.Trim());
                bool hasDOB = txtDOB != null && !string.IsNullOrEmpty(txtDOB.Text.Trim());
                bool hasGender = ddlGender != null && !string.IsNullOrEmpty(ddlGender.SelectedValue);
                bool hasOccupation = ddlOccupation != null && !string.IsNullOrEmpty(ddlOccupation.SelectedValue);
                bool hasRelation = ddlRelation != null && !string.IsNullOrEmpty(ddlRelation.SelectedValue);
                bool hasMobile = txtMobile != null && !string.IsNullOrEmpty(txtMobile.Text.Trim());

                if (hasTitle || hasName || hasDOB || hasGender || hasOccupation || hasRelation || hasMobile)
                {
                    validRowCount++;
                }
            }

            return validRowCount;
        }


        protected void MM_INSERT_MULTI_GRID(string headAH, string headInv)
        {

            try
            {
                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(headInv);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = headAH; //txtSearchClientCode.Text;

                int anmrc = newMemGrid_IL_familyGridView.Rows.Count;
                int nmic = 0;

                foreach (GridViewRow row in newMemGrid_IL_familyGridView.Rows)
                {
                    /*

                    // dr["inv_code"] = ((Label)row.FindControl("newMemGrid_IL_lblInvCode"))?.Text.Trim() ?? string.Empty;
                    string selectedSalutation1 = ((Label)row.FindControl("newMemGrid_IL_lblTitle"))?.Text.Trim() ?? string.Empty;
                    string accountName = ((Label)row.FindControl("newMemGrid_IL_lblInvestorName"))?.Text.Trim() ?? string.Empty;
                    string famMobile = ((Label)row.FindControl("newMemGrid_IL_lblMobile"))?.Text.Trim() ?? string.Empty;
                    string famPan = ((Label)row.FindControl("newMemGrid_IL_lblPan"))?.Text.Trim() ?? string.Empty;
                    string famEmail = ((Label)row.FindControl("newMemGrid_IL_lblEmail"))?.Text.Trim() ?? string.Empty;
                    string famDOB_1 = ((Label)row.FindControl("newMemGrid_IL_lblDOB"))?.Text.Trim() ?? string.Empty;
                    string famRel_1 = ((Label)row.FindControl("newMemGrid_IL_lblRelationID"))?.Text.Trim() ?? string.Empty;
                    string famGName = ((Label)row.FindControl("newMemGrid_IL_lblGuardianName"))?.Text.Trim() ?? string.Empty;
                    string famGPan = ((Label)row.FindControl("newMemGrid_IL_lblGuardianPan"))?.Text.Trim() ?? string.Empty;
                    string famOcc_1 = ((Label)row.FindControl("newMemGrid_IL_lblOccupationID"))?.Text.Trim() ?? string.Empty;
                    string famKYC = ((Label)row.FindControl("newMemGrid_IL_lblKYC"))?.Text.Trim() ?? string.Empty;
                    string famApprove = ((Label)row.FindControl("newMemGrid_IL_lblApproved"))?.Text.Trim() ?? string.Empty;
                    string famGender = ((Label)row.FindControl("newMemGrid_IL_lblGender"))?.Text.Trim() ?? string.Empty;
                    string famNom = ((Label)row.FindControl("newMemGrid_IL_lblIsNoinee"))?.Text.Trim() ?? string.Empty;
                    string famAllo_1 = ((Label)row.FindControl("newMemGrid_IL_lblNomineePer"))?.Text.Trim() ?? string.Empty;
                    string AadharValue_1 = ((Label)row.FindControl("newMemGrid_IL_lblAadhaarCardNumber"))?.Text.Trim() ?? string.Empty;
                    */

                    DropDownList ddlExistingInvestor = (DropDownList)row.FindControl("ngfd_ddlExistingInvestor");
                    TextBox txtInvestorCode = (TextBox)row.FindControl("ngfd_txtInvestorCode");
                    DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
                    TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
                    TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
                    TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
                    TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
                    TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
                    TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
                    DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
                    TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
                    TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
                    DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
                    DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
                    DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
                    DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
                    DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
                    TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");

                    string famSelectedInv = ddlExistingInvestor.SelectedValue;
                    string famInv = txtInvestorCode.Text;
                    string selectedSalutation1 = ddlSalutation.SelectedValue ; 
                    string accountName = txtInvestorName.Text; 
                    string famMobile = txtMobile.Text; 
                    string famPan = txtEmail.Text; 
                    string famEmail = txtPAN.Text; 
                    string famDOB_1 = txtAadhar.Text; 
                    string famRel_1 = txtDOB.Text; 
                    string famGName = ddlGender.Text; 
                    string famGPan = txtGuardianPAN.Text; 
                    string famOcc_1 = txtGuardianName.Text; 
                    string famKYC = ddlOccupation.SelectedValue; 
                    string famApprove = ddlRelation.SelectedValue; 
                    string famGender = ddlKyc.SelectedValue; 
                    string famNom = ddlApproved.SelectedValue; 
                    string famAllo_1 = ddlIsNominee.SelectedValue;
                    string AadharValue_1 = txtAllocation.Text;

                    DateTime? famDOB = ConvertToDate(famDOB_1);

                    int famRel = ParseInt(famRel_1);
                    int famOcc = ParseInt(famOcc_1);
                    string famDOB_1aa = "14/02/2025"; // Input string in dd/MM/yyyy format

                    DateTime? famDOBaa = ConvertToDate(famDOB_1aa);

                    DateTime? famDOBaaa = ConvertToDate(famDOB_1);

                    double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);


                    long AadharValue = ParseLongNumberMMI(AadharValue_1);


                    string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
                    #region MM IN VALUES
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
                    #endregion
                );

                    bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false;
                    bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                    if (isSuccessMemInsert)
                    {
                        nmic += 1;

                    }

                    else if (isDuplicateMemInsert)
                    {
                        ShowAlert(insertResult);
                        if (insertResult.ToUpper().Contains("pan".ToUpper()))
                        {
                            if (insertResult.ToUpper().Contains("families".ToUpper()))
                            {
                                addfamGuardianPan.Focus();
                            }
                            else
                            {
                                addfamPan.Focus();
                            }
                        }

                        else if (insertResult.ToUpper().Contains("mobile".ToUpper()))
                        {
                            addfamMobile.Focus();
                        }

                        else if (insertResult.ToUpper().Contains("email".ToUpper()))
                        {
                            addfamEmail.Focus();
                        }
                        else if (insertResult.ToUpper().Contains("aadhar".ToUpper()))
                        {
                            addfamAadharNumber.Focus();
                        }
                    }
                    else
                    {
                        ShowAlert(insertResult);
                    }
                }
                if (anmrc == nmic)
                {
                    Session["new_mem_list_to_insert_1"] = null;
                    newMemGrid_IL_familyGridView.DataSource = null;
                    newMemGrid_IL_familyGridView.DataBind();
                    newMemGrid_IL_familyGridView.Visible = false;
                    lblAddingNewMemList.Visible = false;
                }



            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                string errorMessage = $"Kindly fill form properly.";
                ClientScript.RegisterStartupScript(this.GetType(), "insertFamExceptionAlert", $"alert('Kindly fill form properly.');", true);
            }
        }

        protected void INSERT_MEMBER_WITH_HEAD(string headAH, string headInv)
        {

            try
            {
                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(headInv);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = headAH; //txtSearchClientCode.Text;

                int anmrc = newMemGrid_IL_familyGridView.Rows.Count;
                int nmic = 0;

                foreach (GridViewRow row in newMemGrid_IL_familyGridView.Rows)
                {
                    // dr["inv_code"] = ((Label)row.FindControl("newMemGrid_IL_lblInvCode"))?.Text.Trim() ?? string.Empty;
                    string selectedSalutation1 = ((Label)row.FindControl("newMemGrid_IL_lblTitle"))?.Text.Trim() ?? string.Empty;
                    string accountName = ((Label)row.FindControl("newMemGrid_IL_lblInvestorName"))?.Text.Trim() ?? string.Empty;
                    string famMobile = ((Label)row.FindControl("newMemGrid_IL_lblMobile"))?.Text.Trim() ?? string.Empty;
                    string famPan = ((Label)row.FindControl("newMemGrid_IL_lblPan"))?.Text.Trim() ?? string.Empty;
                    string famEmail = ((Label)row.FindControl("newMemGrid_IL_lblEmail"))?.Text.Trim() ?? string.Empty;
                    string famDOB_1 = ((Label)row.FindControl("newMemGrid_IL_lblDOB"))?.Text.Trim() ?? string.Empty;
                    string famRel_1 = ((Label)row.FindControl("newMemGrid_IL_lblRelationID"))?.Text.Trim() ?? string.Empty;
                    string famGName = ((Label)row.FindControl("newMemGrid_IL_lblGuardianName"))?.Text.Trim() ?? string.Empty;
                    string famGPan = ((Label)row.FindControl("newMemGrid_IL_lblGuardianPan"))?.Text.Trim() ?? string.Empty;
                    string famOcc_1 = ((Label)row.FindControl("newMemGrid_IL_lblOccupationID"))?.Text.Trim() ?? string.Empty;
                    string famKYC = ((Label)row.FindControl("newMemGrid_IL_lblKYC"))?.Text.Trim() ?? string.Empty;
                    string famApprove = ((Label)row.FindControl("newMemGrid_IL_lblApproved"))?.Text.Trim() ?? string.Empty;
                    string famGender = ((Label)row.FindControl("newMemGrid_IL_lblGender"))?.Text.Trim() ?? string.Empty;
                    string famNom = ((Label)row.FindControl("newMemGrid_IL_lblIsNoinee"))?.Text.Trim() ?? string.Empty;
                    string famAllo_1 = ((Label)row.FindControl("newMemGrid_IL_lblNomineePer"))?.Text.Trim() ?? string.Empty;
                    string AadharValue_1 = ((Label)row.FindControl("newMemGrid_IL_lblAadhaarCardNumber"))?.Text.Trim() ?? string.Empty;

                    DateTime? famDOB = ConvertToDate(famDOB_1);

                    int famRel = ParseInt(famRel_1);
                    int famOcc = ParseInt(famOcc_1);
                    string famDOB_1aa = "14/02/2025"; // Input string in dd/MM/yyyy format

                    DateTime? famDOBaa = ConvertToDate(famDOB_1aa);

                    DateTime? famDOBaaa = ConvertToDate(famDOB_1);

                    double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);


                    long AadharValue = ParseLongNumberMMI(AadharValue_1);


                    string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
                    #region MM IN VALUES
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
                    #endregion
                );

                    bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false;
                    bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                    if (isSuccessMemInsert)
                    {
                        nmic += 1;
                        //ShowAlert(insertResult);
                        //lblMessageAddFam.Text = insertResult;
                        //ResetFamSectionOnUPdate();
                        //addfamGuardianName.Attributes.Remove("required");
                        //addfamGuardianPan.Attributes.Remove("required");

                        //string famSrcId = txtHeadSourceCode.Text;
                        //string famExistID = txtClientCode.Text;
                        //HandleFamilyData(famSrcId, famExistID);
                        //AddMemberButton.Enabled = true;
                        //UpdateMemberButton.Enabled = false;

                    }

                    else if (isDuplicateMemInsert)
                    {
                        ShowAlert(insertResult);
                        if (insertResult.ToUpper().Contains("pan".ToUpper()))
                        {
                            if (insertResult.ToUpper().Contains("families".ToUpper()))
                            {
                                addfamGuardianPan.Focus();
                            }
                            else
                            {
                                addfamPan.Focus();
                            }
                        }

                        else if (insertResult.ToUpper().Contains("mobile".ToUpper()))
                        {
                            addfamMobile.Focus();
                        }

                        else if (insertResult.ToUpper().Contains("email".ToUpper()))
                        {
                            addfamEmail.Focus();
                        }
                        else if (insertResult.ToUpper().Contains("aadhar".ToUpper()))
                        {
                            addfamAadharNumber.Focus();
                        }
                    }
                    else
                    {
                        ShowAlert(insertResult);
                    }
                }
                if (anmrc == nmic)
                {
                    Session["new_mem_list_to_insert_1"] = null;
                    newMemGrid_IL_familyGridView.DataSource = null;
                    newMemGrid_IL_familyGridView.DataBind();
                    newMemGrid_IL_familyGridView.Visible = false;
                    lblAddingNewMemList.Visible = false;
                }



            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                string errorMessage = $"Kindly fill form properly.";
                ClientScript.RegisterStartupScript(this.GetType(), "insertFamExceptionAlert", $"alert('Kindly fill form properly.');", true);
            }
        }

        protected void InsertMember()
        {
            try
            {
                string inv_code = ddladdfamExistingInvestor.SelectedValue;
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                long existClientCodeValue = string.IsNullOrEmpty(txtClientCode.Text) ? 0 : Convert.ToInt64(txtClientCode.Text);
                string businessCodeValue = string.IsNullOrEmpty(txtBusinessCode.Text) ? null : txtBusinessCode.Text;
                string loggedinUser = Session["LoginId"]?.ToString();
                string clientCodeForMainValue = txtSearchClientCode.Text;


                /*
                string selectedSalutation1 = ddlSalutation3.SelectedValue;
                string accountName = addfamInvestorName.Text.Trim();
                string famMobile = string.IsNullOrEmpty(addfamMobile.Text) ? null : addfamMobile.Text;
                string famPan = string.IsNullOrEmpty(addfamPan.Text) ? null : addfamPan.Text;
                string famEmail = string.IsNullOrEmpty(addfamEmail.Text) ? null : addfamEmail.Text;
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

                */


                int anmrc = newMemGrid_IL_familyGridView.Rows.Count;
                int nmic = 0;

                foreach (GridViewRow row in newMemGrid_IL_familyGridView.Rows)
                {
                    // dr["inv_code"] = ((Label)row.FindControl("newMemGrid_IL_lblInvCode"))?.Text.Trim() ?? string.Empty;
                    string selectedSalutation1 = ((Label)row.FindControl("newMemGrid_IL_lblTitle"))?.Text.Trim() ?? string.Empty;
                    string accountName = ((Label)row.FindControl("newMemGrid_IL_lblInvestorName"))?.Text.Trim() ?? string.Empty;
                    string famMobile = ((Label)row.FindControl("newMemGrid_IL_lblMobile"))?.Text.Trim() ?? string.Empty;
                    string famPan = ((Label)row.FindControl("newMemGrid_IL_lblPan"))?.Text.Trim() ?? string.Empty;
                    string famEmail = ((Label)row.FindControl("newMemGrid_IL_lblEmail"))?.Text.Trim() ?? string.Empty;
                    string famDOB_1 = ((Label)row.FindControl("newMemGrid_IL_lblDOB"))?.Text.Trim() ?? string.Empty;
                    string famRel_1 = ((Label)row.FindControl("newMemGrid_IL_lblRelationID"))?.Text.Trim() ?? string.Empty;
                    string famGName = ((Label)row.FindControl("newMemGrid_IL_lblGuardianName"))?.Text.Trim() ?? string.Empty;
                    string famGPan = ((Label)row.FindControl("newMemGrid_IL_lblGuardianPan"))?.Text.Trim() ?? string.Empty;
                    string famOcc_1 = ((Label)row.FindControl("newMemGrid_IL_lblOccupationID"))?.Text.Trim() ?? string.Empty;
                    string famKYC = ((Label)row.FindControl("newMemGrid_IL_lblKYC"))?.Text.Trim() ?? string.Empty;
                    string famApprove = ((Label)row.FindControl("newMemGrid_IL_lblApproved"))?.Text.Trim() ?? string.Empty;
                    string famGender = ((Label)row.FindControl("newMemGrid_IL_lblGender"))?.Text.Trim() ?? string.Empty;
                    string famNom = ((Label)row.FindControl("newMemGrid_IL_lblIsNoinee"))?.Text.Trim() ?? string.Empty;
                    string famAllo_1 = ((Label)row.FindControl("newMemGrid_IL_lblNomineePer"))?.Text.Trim() ?? string.Empty;
                    string AadharValue_1 = ((Label)row.FindControl("newMemGrid_IL_lblAadhaarCardNumber"))?.Text.Trim() ?? string.Empty;
                    //dr["OCC_NAME"] = ((Label)row.FindControl("newMemGrid_IL_lblOccupation"))?.Text.Trim() ?? string.Empty;
                    //dr["OUR_RELATIONSHIP"] = ((Label)row.FindControl("newMemGrid_IL_lblRelation"))?.Text.Trim() ?? string.Empty;

                    //DateTime? famDOB = DateTime.TryParseExact(famDOB_1, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) ? (DateTime?)parsedDate : null;
                    DateTime? famDOB = ConvertToDate(famDOB_1);

                    int famRel = ParseInt(famRel_1);
                    int famOcc = ParseInt(famOcc_1);
                    string famDOB_1aa = "14/02/2025"; // Input string in dd/MM/yyyy format

                    DateTime? famDOBaa = ConvertToDate(famDOB_1aa);

                    DateTime? famDOBaaa = ConvertToDate(famDOB_1);

                    double famAllo = string.IsNullOrEmpty(famAllo_1) ? 0 : Convert.ToDouble(famAllo_1);


                    long AadharValue = ParseLongNumberMMI(AadharValue_1);


                    string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
                #region MM IN VALUES
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
                #endregion 
                );

                bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false;
                bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                if (isSuccessMemInsert)
                {
                        nmic += 1;
                    ShowAlert(insertResult);
                    lblMessageAddFam.Text = insertResult;
                    ResetFamSectionOnUPdate();
                    addfamGuardianName.Attributes.Remove("required");
                    addfamGuardianPan.Attributes.Remove("required");

                    string famSrcId = txtHeadSourceCode.Text;
                    string famExistID = txtClientCode.Text;
                    HandleFamilyData(famSrcId, famExistID);
                    AddMemberButton.Enabled = true;
                    //UpdateMemberButton.Enabled = false;

                }

                else if (isDuplicateMemInsert)
                {
                    ShowAlert(insertResult);
                    if (insertResult.ToUpper().Contains("pan".ToUpper()))
                    {
                        if (insertResult.ToUpper().Contains("families".ToUpper()))
                        {
                            addfamGuardianPan.Focus();
                        }
                        else
                        {
                            addfamPan.Focus();
                        }
                    }

                    else if (insertResult.ToUpper().Contains("mobile".ToUpper()))
                    {
                        addfamMobile.Focus();
                    }

                    else if (insertResult.ToUpper().Contains("email".ToUpper()))
                    {
                        addfamEmail.Focus();
                    }
                    else if (insertResult.ToUpper().Contains("aadhar".ToUpper()))
                    {
                        addfamAadharNumber.Focus();
                    }

                }
                else
                {
                    ShowAlert(insertResult);
                }
                }
                if(anmrc == nmic)
                {
                    Session["new_mem_list_to_insert_1"] = null;
                    newMemGrid_IL_familyGridView.DataSource = null;
                    newMemGrid_IL_familyGridView.DataBind();
                }
                  


            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                string errorMessage = $"Kindly fill form properly.";
                ClientScript.RegisterStartupScript(this.GetType(), "insertFamExceptionAlert", $"alert('Kindly fill form properly.');", true);
            }
        }

        protected void InsertMember_Singel_Insert()
        {
            try
            {
                string inv_code = ddladdfamExistingInvestor.SelectedValue;
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

                string insertResult = new WM.Controllers.AccountOpeningController().InsertMemberWithMainCode(
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

                bool isSuccessMemInsert = insertResult.ToUpper().Contains("Success".ToUpper()) ? true : false ;
                bool isDuplicateMemInsert = insertResult.ToUpper().Contains("Duplicate".ToUpper()) ? true : false;

                if (isSuccessMemInsert)
                {
                    ShowAlert(insertResult);
                    lblMessageAddFam.Text = insertResult;
                    ResetFamSectionOnUPdate();
                    addfamGuardianName.Attributes.Remove("required");
                    addfamGuardianPan.Attributes.Remove("required");

                    string famSrcId = txtHeadSourceCode.Text;
                    string famExistID = txtClientCode.Text;
                    HandleFamilyData(famSrcId, famExistID);
                    AddMemberButton.Enabled = true;
                   // UpdateMemberButton.Enabled = false;

                }

                else if (isDuplicateMemInsert)
                {
                    ShowAlert(insertResult);
                    if (insertResult.ToUpper().Contains("pan".ToUpper()))
                    {
                        if (insertResult.ToUpper().Contains("families".ToUpper()))
                        {
                            addfamGuardianPan.Focus();
                        }
                        else
                        {
                            addfamPan.Focus();
                        }
                    }

                    else if (insertResult.ToUpper().Contains("mobile".ToUpper()))
                    {
                        addfamMobile.Focus();
                    }

                    else if (insertResult.ToUpper().Contains("email".ToUpper()))
                    {
                        addfamEmail.Focus();
                    }
                    else if (insertResult.ToUpper().Contains("aadhar".ToUpper()))
                    {
                        addfamAadharNumber.Focus();
                    }

                }
                else
                {
                    ShowAlert(insertResult);
                }

            }
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
                string errorMessage = $"Kindly fill form properly.";
                ClientScript.RegisterStartupScript(this.GetType(), "insertFamExceptionAlert", $"alert('Kindly fill form properly.');", true);
            }
        }


        protected void UPDATE_MEMBER_BY_INV(string famClientCode)
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
                    ddlSalutation3.Focus();
                    ShowAlert( "Member title is requried");return;

                }

                if (string.IsNullOrEmpty(addfamInvestorName.Text))
                {
                    addfamInvestorName.Focus();
                    ShowAlert( "Member name is requried");return;

                }

                if (string.IsNullOrEmpty(addfamGender.SelectedValue.ToString()))
                {
                    addfamGender.Focus();
                    ShowAlert( "Member gender is requried");return;

                }

                if (string.IsNullOrEmpty(addfamMobile.Text))
                {
                    addfamMobile.Focus();
                    ShowAlert( "Member mobile is requried");return;
                }
                if (string.IsNullOrEmpty(addfamDOB.Text))
                {
                    addfamDOB.Focus();
                    ShowAlert( "Member DOB is requried");return;

                }

                if (string.IsNullOrEmpty(addfamOccupation.Text))
                {
                    addfamOccupation.Focus();
                    ShowAlert( "Member Occupation is requried");return;

                }

                if (string.IsNullOrEmpty(addfamRelation.Text))
                {
                    addfamRelation.Focus();
                    ShowAlert( "Member Relation is requried");return;

                }

                if (ValidateMobileFieldMinLength(addfamMobile))
                {
                    addfamMobile.Focus();
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
                    //UpdateMemberButton.Enabled = false;


                }

                else if (updateFamResult.Contains("Duplicate"))
                {
                    ShowAlert(updateFamResult);
                    if (updateFamResult.ToUpper().Contains("pan".ToUpper()))
                    {
                        if (updateFamResult.ToUpper().Contains("families".ToUpper()))
                        {
                            addfamGuardianPan.Focus();
                        }
                        else
                        {
                            addfamPan.Focus();

                        }
                    }

                    if (updateFamResult.ToUpper().Contains("Mobile".ToUpper()))
                    {
                        addfamMobile.Focus();
                    }

                }
                else if (updateFamResult.Contains("Error"))
                {
                    ShowAlert(updateFamResult);
                }
                else
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
            if(ddladdfamExistingInvestor.Items.Count > 0)
            {

            ddladdfamExistingInvestor.SelectedIndex = 0;
            }
           
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

        private string ReturnedMatchedDropValue(DataRow row, string columnName, DropDownList dropdown)
        {
            try
            {
                if (row == null || string.IsNullOrEmpty(columnName) || dropdown == null)
                    return "";

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
                        // If the value exists, return the matched value
                        return matchingItem.Value;
                    }
                    else
                    {
                        // If the value does not exist, add it to the dropdown and return it
                        dropdown.Items.Add(new ListItem(rowValue, rowValue));
                        return rowValue;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return ""; // Default return for error cases
        }
        protected void ddladdfamExistingInvestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected client code from the dropdown
            string currentInvCode = ddladdfamExistingInvestor.SelectedValue;
            string currentInvCodeNew = "";

   
 
            string nnn = currentInvCodeNew.ToString();
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
                    string genderChar = "";
                    if (!string.IsNullOrWhiteSpace(currentGenderDB_value))
                    {
                        genderChar = currentGenderDB_value.Length > 0 ? currentGenderDB_value[0].ToString().ToUpper() : "";

                    }



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
                    SetDropdownValueKYC(row, "KYC", addfamKYC);


                    string CKFF = addfamKYC.SelectedValue;
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

        private void SetInvestorDetailsNew(GridViewRow row, DataRow member)
        {
            // Salutation
            DropDownList ddlSalutation = (DropDownList)row.FindControl("ngfd_ddlSalutation");
            if (ddlSalutation != null)
                ddlSalutation.SelectedValue = member["investor_title"].ToString();

            // Investor Name
            TextBox txtInvestorName = (TextBox)row.FindControl("ngfd_txtInvestorName");
            if (txtInvestorName != null)
                txtInvestorName.Text = member["investor_name"].ToString();

            // Mobile
            TextBox txtMobile = (TextBox)row.FindControl("ngfd_txtMobile");
            if (txtMobile != null)
                txtMobile.Text = member["mobile"].ToString();

            // Email
            TextBox txtEmail = (TextBox)row.FindControl("ngfd_txtEmail");
            if (txtEmail != null)
                txtEmail.Text = member["email"].ToString();

            // PAN
            TextBox txtPAN = (TextBox)row.FindControl("ngfd_txtPAN");
            if (txtPAN != null)
                txtPAN.Text = member["PAN"].ToString();

            // Aadhar
            TextBox txtAadhar = (TextBox)row.FindControl("ngfd_txtAadhar");
            if (txtAadhar != null)
                txtAadhar.Text = member["AADHAR_CARD_NO"].ToString();

            // DOB
            TextBox txtDOB = (TextBox)row.FindControl("ngfd_txtDOB");
            if (txtDOB != null)
                txtDOB.Text = Convert.ToDateTime(member["DOB"]).ToString("dd-MM-yyyy");

            // Gender
            DropDownList ddlGender = (DropDownList)row.FindControl("ngfd_ddlGender");
            if (ddlGender != null)
                ddlGender.SelectedValue = member["Gender"].ToString();

            // Guardian PAN
            TextBox txtGuardianPAN = (TextBox)row.FindControl("ngfd_txtgPAN");
            if (txtGuardianPAN != null)
                txtGuardianPAN.Text = member["g_pan"].ToString();

            // Guardian Name
            TextBox txtGuardianName = (TextBox)row.FindControl("ngfd_txtgName");
            if (txtGuardianName != null)
                txtGuardianName.Text = member["g_name"].ToString();

            // Occupation
            DropDownList ddlOccupation = (DropDownList)row.FindControl("ngfd_ddlOccupation");
            if (ddlOccupation != null)
                ddlOccupation.SelectedValue = member["occ_id"].ToString();

            // Relation
            DropDownList ddlRelation = (DropDownList)row.FindControl("ngfd_ddlRelation");
            if (ddlRelation != null)
                ddlRelation.SelectedValue = member["OUR_RELATIONSHIP"].ToString();

            /*
            // KYC
            DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
            if (ddlKyc != null)
                ddlKyc.SelectedValue = member["KYC"].ToString();

            // Approved
            DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
            if (ddlApproved != null)
                ddlApproved.SelectedValue = member["Approved"].ToString();

            // Is Nominee
            DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
            if (ddlIsNominee != null)
                ddlIsNominee.SelectedValue = member["is_nominee"].ToString();*/

            // KYC
            DropDownList ddlKyc = (DropDownList)row.FindControl("ngfd_ddlKyc");
            SetDropdownSelectedValue(ddlKyc, member["KYC"].ToString());

            // Approved
            DropDownList ddlApproved = (DropDownList)row.FindControl("ngfd_ddlApproved");
            SetDropdownSelectedValue(ddlApproved, member["Approved"].ToString());

            // Is Nominee
            DropDownList ddlIsNominee = (DropDownList)row.FindControl("ngfd_ddlIsNominee");
            SetDropdownSelectedValue(ddlIsNominee, member["is_nominee"].ToString());


            // Allocation
            TextBox txtAllocation = (TextBox)row.FindControl("ngfd_txtAllocation");
            if (txtAllocation != null)
                txtAllocation.Text = member["nominee_per"].ToString();
        }


        private void SetDropdownSelectedValue(DropDownList ddl, string value)
        {
            if (ddl != null && !string.IsNullOrEmpty(value))
            {
                // Trim spaces and match case-sensitive values
                ListItem matchedItem = ddl.Items.FindByValue(value.Trim());

                if (matchedItem != null)
                {
                    ddl.SelectedValue = matchedItem.Value;
                }
                else
                {
                    // If an exact match is not found, fallback to case-insensitive search
                    foreach (ListItem item in ddl.Items)
                    {
                        if (item.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                        {
                            ddl.SelectedValue = item.Value;
                            break;
                        }
                    }
                }
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

        private void SetFieldData(DataRow row)
        {
            btnSave.Enabled = false;
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

            //GetDropDownValue(row, "OCC_Id", ddlOccupation);
            ddlOccupation.SelectedValue = GetDropDownValueFromDbValue(db_occ_id_value, ddlOccupation);


            ddlTaxStatus.SelectedIndex = GetMatchedIndex(ddlTaxStatus, row["STATUS"].ToString());

            GetDropDownValue(row, "CT_STATUS", ddlAOCategoryStatus);
            GetDropDownValue(row, "CM_category_id", ddlAOClientCategory);

            ddlAOACCategory.SelectedValue = GetDropDownValueFromDbValue(db_ct_act_cat_value, ddlAOACCategory);

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
            if (!string.IsNullOrEmpty(cldDOB.Text))
            {
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
                        ddlMailingCountryList.SelectedValue = currentDB_MCountryID;

                       
                        PopulateStateDropDownForAddress(Convert.ToInt32(currentDB_MCountryID), ddlMailingStateList);

                        if (ddlMailingStateList.Items.FindByValue(currentDB_MStateID) != null)
                        { ddlMailingStateList.Enabled = true;
                            try
                            {
                                // Set the state dropdown selected value
                                ddlMailingStateList.SelectedValue = db_mailing_add_state_value;

                                // Populate the city dropdown based on the selected state
                                PopulateCityDropDownForAddress(Convert.ToInt32(currentDB_MStateID), ddlMailingCityList);
                                if (ddlMailingCityList.Items.Count > 0)
                                {
                                    ddlMailingCityList.Enabled = true;
                                }
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
                        ddlPCountryList.SelectedValue = currentDB_P_CountryID;
                        // Populate the state dropdown based on the selected country
                        PopulateStateDropDownForAddress(Convert.ToInt32(currentDB_P_CountryID), ddlPStateList);

                        if (ddlPStateList.Items.FindByValue(currentDB_P_StateID) != null)
                        {
                            ddlPStateList.Enabled = true;
                            try
                            {
                               
                                // Set the state dropdown selected value
                                ddlPStateList.SelectedValue = db_perm_add_state_value;

                                try
                                {
                                    // Populate the city dropdown based on the selected state
                                    PopulateCityDropDownForAddress(Convert.ToInt32(currentDB_P_StateID), ddlPCityList);
                                    if (ddlPCityList.Items.Count > 0)
                                    {
                                        ddlPCityList.Enabled = true;
                                    }
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
            if (ddlMailingCountryList.Items.Count > 0)
            {
                string currentMailingCoutnryName = ddlMailingCountryList.SelectedItem.ToString();

                HandlePinCodeValidation(currentMailingCoutnryName, txtMailingPin);
                try
                {
                    txtMailingPin.Text = db_pincode_value;
                }
                catch (Exception ex)
                {


                }
            }

            if (ddlPCountryList.Items.Count > 0)
            {
                string currentPCoutnryName = ddlPCountryList.SelectedItem.ToString();
                HandlePinCodeValidation(currentPCoutnryName, txtPPin);

                
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
            if (!string.IsNullOrWhiteSpace(famSrcId) && !string.IsNullOrWhiteSpace(famExistID))
            {
                HandleFamilyData(famSrcId, famExistID);
            }

            #endregion

            #region Handle Advisory and Fill 
            if (txtSearchClientCode.Text != null)
            {
                try
                {

                    FillAdvisoryDataByAH(ExistingClientCodeInv.Text, txtSearchClientCode.Text);
                }   
                catch (Exception ex)
                {

                }
            }


            #endregion

            #region ADVIOSRY BUTTON ENABLE DISABLE
            //if (string.IsNullOrEmpty(txtHeadSourceCode.Text))
            if (txtClientAdvisoryInfo.Text.ToUpper() == "VALID DATA")
            {
                btnAdvGenerate.Enabled = false;
                btnAdvUpdate.Enabled = true;
                btnAdvPrint.Enabled = true;
            }
            else
            {
                btnAdvGenerate.Enabled = true;
                btnAdvUpdate.Enabled = false;
                btnAdvPrint.Enabled = false;
            }

            #endregion

            txtAccountName.Focus();


            #region Button Enable/Disbale

            string currentAHHead = txtSearchClientCode.Text;
            if (!string.IsNullOrEmpty(currentAHHead) || currentAHHead != null)
            {
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;

            }
            else
            {
                btnSave.Enabled = true;

                btnUpdate.Enabled = false;
            }

            #region EXISTING, MEMBER SINGLE INSERT, UPDATE BUTTON ENABLED/DISEBLED
            if (!string.IsNullOrEmpty(ExistingClientCodeInv.Text))
            {
                AddMemberButton.Enabled = true;
            }
            else
            {
                AddMemberButton.Enabled = false;

            }

            // EXISTING INVESSSTOR DROP EN/DIS
            if (ddladdfamExistingInvestor.Items.Count>1)
            {
                ddladdfamExistingInvestor.Enabled = true;
            }
            else
            {
                ddladdfamExistingInvestor.Enabled = false;
            }

            #endregion

            #endregion

            if (!string.IsNullOrEmpty(txtHeadSourceCode.Text))
            {
                //btnAdvGenerate.Enabled = true;
                //btnAdvUpdate.Enabled = true;
                //btnAdvPrint.Enabled = true;

            }
             

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
                    chkSameAsMailing.Enabled = true;
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
                    chkSameAsMailing.Enabled = false;
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

                    if (ddladdfamExistingInvestor.Items.Count <= 0)
                    {
                        ddladdfamExistingInvestor.Items.Add(new ListItem("NOT EXISTING INVESTOR", ""));
                        UpdateMemberButton.Enabled = false;
                    }

                    ddladdfamExistingInvestor.SelectedIndex = 0;
                    AddMemberButton.Enabled = true;
                    //UpdateMemberButton.Enabled = false;
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
                string DB_BANK_NAME = GetTextFieldValue(row, "bank_name");
                string DB_REMARK = GetTextFieldValue(row, "REMARK");
                string DB_AMT = GetTextFieldValue(row, "amount");
                string DB_PAYMENT_MODE = GetTextFieldValue(row, "payment_mode");
                string DB_CHEQUE_NO = GetTextFieldValue(row, "cheque_no");

                // Check and set cheque date
                if (row.Table.Columns.Contains("cheque_date") && row["cheque_date"] != DBNull.Value)
                {
                    GetSetDateField(row, "cheque_date", txtChequeDate);
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
                if (!string.IsNullOrEmpty(DB_PAYMENT_MODE) && DB_PAYMENT_MODE.ToUpper() == "C")
                {
                    if (DB_PAYMENT_MODE.ToUpper() == "C")
                        cheque.Checked = true;
                  

                }
                else
                {
                    draft.Checked = true;
                  

                }

                if (cheque.Checked)
                {
                    ChequeLabel.Text = "Cheque No <span class='text-danger'>*</span>";
                    ChequeDatedLabel.Text = "Cheque Dated <span class='text-danger'>*</span>";
                    txtChequeDate.Focus();
                    ddlMutualBank.Enabled = true;
                    txtChequeDate.Enabled = true;
                    txtChequeNo.Enabled = true;
                    txtChequeNo.Text = DB_CHEQUE_NO;


                }
                else if (draft.Checked)
                {
                    ChequeLabel.Text = "Draft No <span class='text-danger'>*</span>";
                    ChequeDatedLabel.Text = "Draft Dated <span class='text-danger'>*</span>";

                    txtChequeDate.Focus();
                    ddlMutualBank.Enabled = true;
                    txtChequeDate.Enabled = true;
                    txtChequeNo.Enabled = true;
                    txtChequeNo.Text = DB_CHEQUE_NO;


                }

                else if (optCash.Checked)
                {
                    ChequeLabel.Text = "";
                    ChequeDatedLabel.Text = "";

                    ddlMutualBank.Enabled = false;
                    txtChequeDate.Enabled = false;
                    txtChequeNo.Enabled = false;
                }



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
                if (dt.Rows.Count > 0)
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

        private void FillClientInvestor(string id)
        {
            try
            {
                DataTable dt = new AccountOpeningController().GetClientDataByInv(id);
                if (dt.Rows.Count > 0)
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
                    string currectAHByIdPan = GetTextFieldValue(result.Rows[0], "client_code");
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

            if (!string.IsNullOrWhiteSpace(commonID))
            {
                SEARCH_BY_DT(commonID);

                /*
                 IF FOUND GUEST CODE THEN 
                IF VLAID GUEST THEN FILL BY GUEST AND AGAIN SET DT NUMBER WITH PREVIOUSE DT VALUE
                 */
            }
            else
            {
                ShowAlert("ENTER DT NUMBER");
            }

        }
        protected void SEARCH_BY_DT(string commonID)
        {
            try
            {
                DataTable dt = new AccountOpeningController().GetClientDataByDTNumber(commonID);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string message = dt.Rows[0]["message"].ToString();
                    string DtNotMsg = "not exist";
                    string onlyDTmsg = "Valid Data: DT Exist";

                    if (message.Contains(DtNotMsg))
                    {
                        // RESET FORM
                        ShowAlert(message);
                        txtDTNumber.Text = commonID;
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
                        string rccGuestCode = GetTextFieldValue(row, "guest_code");
                        string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                        string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");

                        AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                        AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                        AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                        AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");

                        txtDTNumber.Text = commonID;

                        if (!string.IsNullOrEmpty(rccGuestCode) && (txtGuestCode.Text.ToString() != "0"))
                        {
                            txtGuestCode.Text = !string.IsNullOrEmpty(rccGuestCode) ? rccGuestCode : string.Empty;

                            string isValidGC = IS_VALID_GUEST(rccGuestCode);
                            if (isValidGC.ToUpper().Contains("PASS"))
                            {
                                FetchedByGuestCode(rccGuestCode);
                                //FillClientByGuestCode(currentGuestValue, loginValue);
                            }
                            else
                            {
                                RESET_GUEST_VALUE();
                                ShowAlert(isValidGC);
                                txtGuestCode.Text = rccGuestCode;
                            }
                            //FillClientByGuestCode(rccGuestCode, Session["LoginId"]?.ToString());

                        }
                        else {

                            UpdateBusinessCodeDetails(rccBusiValue);// SET BUSS RM NAME AND BRANCH
                            txtBusinessCode.Text = rccBusiValue;
                        }



                        //ApprovalStatus.Text = string.Empty;
                    }

                    else
                    {

                        ShowAlert(message);
                        txtDTNumber.Text = commonID;
                        txtDTNumber.Focus();
                    }

                }
                else
                {


                    string emptyMsg = "No Data!";
                    ShowAlert(emptyMsg);
                }

            }

            catch (Exception ex)
            {
                Response.Redirect(Request.RawUrl);
                //ResetFormFields1();
                //ShowAlert(ex.Message);

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
            INSERT_CLIENT_DATA();

        }

        protected void INSERT_CLIENT_DATA()
        {
            try
            {
                #region INVESRT DATA GET
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
                bool PermanentAddressSameAsMailing = chkSameAsMailing.Checked;
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
                #endregion

                #region Not Null Validaiton

                if (string.IsNullOrEmpty(txtDTNumber.Text))
                {
                    string message = "DT Number is empty";
                    ShowAlert(message);
                    
                    txtDTNumber.Focus();
                    return;
                }


                if (string.IsNullOrEmpty(txtBusinessCode.Text))
                {
                    string message = "Business Code is empty, enter valid DT";
                    ShowAlert(message);
                    
                    txtDTNumber.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlOccupation.SelectedValue))
                {
                    string message = "Occupation is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    ddlOccupation.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOCategoryStatus.SelectedValue))
                {
                    string message = "Category status is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    ddlAOCategoryStatus.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOClientCategory.SelectedValue))
                {
                    string message = "Client category is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    ddlAOClientCategory.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(accountName))
                {
                    string message = "Client name is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtAccountName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(cldDOB.Text))
                {
                    string message = "DOB is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    cldDOB.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOGender.Text))
                {
                    string message = "Gender is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlAOGender.Focus();
                    return;
                }
                if (itfAOGuardianPerson.Enabled && string.IsNullOrEmpty(guardianPerson))
                {
                    string message = "Guardian name is required";
                    ShowAlert(message);
                    itfAOGuardianPerson.Focus();
                    return;
                }


                if (string.IsNullOrEmpty(MobileNo.Text))
                {
                    string message = "Mobile number is required";
                    ShowAlert(message);
                    MobileNo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue))
                {
                    string message = "Mailing Country is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlMailingCountryList.Focus();
                    return;
                }

                if (ddlMailingCityList.Items.Count > 0 && string.IsNullOrEmpty(ddlMailingCityList.Text))

                {
                    string message = "Mailing city is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    ddlMailingCityList.Focus();
                    return;
                }

                if (!string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue) && string.IsNullOrEmpty(txtMailingPin.Text))
                {

                    ShowAlert("Mailing pin code is reuried");
                    txtMailingPin.Focus();
                    return;
                }

                if (!string.IsNullOrEmpty(ddlPCountryList.SelectedValue) && string.IsNullOrEmpty(txtPPin.Text))
                {

                    PermanentPinCode = null;

                }


              

                if (!IsAllZeros(txtAadhar.Text))
                {
                    ShowAlert("Invalid Aadhaar Number (Must be 12-digit numeric)");
                    return;

                }
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    string message = "Email is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    txtEmail.Focus();
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

                #endregion


                #region Handle pin code by country
                /* not reqrueid
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
                      PermanentPinCode = txtPPin.Text;
                  }
                  catch (Exception ex)
                  {

                  }
              }


              if (!string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue) && string.IsNullOrEmpty(txtMailingPin.Text))
              {

                  ShowAlert("Mailing pin code is reuried");
                  txtMailingPin.Focus();
                  return;
              }



              if (!string.IsNullOrEmpty(ddlPCountryList.SelectedValue) && string.IsNullOrEmpty(txtPPin.Text))
              {

                  PermanentPinCode = null;

              }
              */

                #endregion


                string isClientValidationPass = new WM.Controllers.AccountOpeningController().ValidateUpdateClientData(
                   null,
dtNumberValue,
guestCodeValue,
existClientCodeValue,
businessCodeValue,
taxStatus,
occupationValue,
statusCategoryValue,
clientCategoryValue,
accountCategoryValue,
selectedSalutation1,
accountName.Trim().ToUpper(),
selectedSalutation2,
accountFatherName.Trim().ToUpper(),
accountOtherValue.Trim().ToUpper(),
gender,
maritalStatus,
nationality,
residentNri,
dob,
annualIncome,
clientPan,
leadType,
guardianPerson.Trim().ToUpper(),
guardianPersonNationality.Trim().ToUpper(),
guardianPersonPan,
mailingAddress1.Trim().ToUpper(),
mailingAddress2.Trim().ToUpper(),
mailingState,
mailingCity,
mailingPinCode,
permanentAddress1.Trim().ToUpper(),
permanentAddress2.Trim().ToUpper(),
permanentState,
permanentCity,
PermanentPinCode,
NRIAddress.Trim().ToUpper(),
FaxValue,
AadharValue,
EmailValue.Trim().ToUpper(),
EmailOfficialValue.Trim().ToUpper(),
PhoneOfficeSTDValue,
PhoneOfficeNumberValue,
PhoneResSTDValue,
PhoneResNumberValue,
MobileNoValue,
ReferenceName1Value.Trim().ToUpper(),
ReferenceName2Value.Trim().ToUpper(),
ReferenceName3Value.Trim().ToUpper(),
ReferenceName4Value.Trim().ToUpper(),
MobileNo1Value,
MobileNo2Value,
MobileNo3Value,
MobileNo4Value,
loggedinUser
                   );
                string isMemberValidattionPssed = "";
                if (!isClientValidationPass.ToUpper().Contains("PASS"))
                {
                    ShowAlert(isClientValidationPass);
                    return;
                }
                
                int validRowCount = GetValidRowCount(ngfd_gvClients);
                if (validRowCount > 0)
                {
                    isMemberValidattionPssed = ValidateRequiredFieldsForFirstValidRow(ngfd_gvClients);

                if (!isMemberValidattionPssed.ToUpper().Contains("PASS"))
                {

                    ShowAlert(isMemberValidattionPssed);
                    return;
                }
                }

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
accountName.Trim().ToUpper(),
selectedSalutation2, 
accountFatherName.Trim().ToUpper(), 
accountOtherValue,
gender, 
maritalStatus, 
nationality, 
residentNri, 
dob, 
annualIncome, 
clientPan, 
leadType, 
guardianPerson.Trim().ToUpper(),
guardianPersonNationality.Trim().ToUpper(), 
guardianPersonPan, 
mailingAddress1.Trim().ToUpper(), 
mailingAddress2.Trim().ToUpper(), 
mailingState, 
mailingCity, 
mailingPinCode,
permanentAddress1.Trim().ToUpper(),
permanentAddress2.Trim().ToUpper(),
permanentState,
permanentCity,
PermanentPinCode,
NRIAddress.Trim().ToUpper(),
FaxValue,
AadharValue,
EmailValue.Trim().ToUpper(),
EmailOfficialValue.Trim().ToUpper(),
PhoneOfficeSTDValue,
PhoneOfficeNumberValue,
PhoneResSTDValue,
PhoneResNumberValue,
MobileNoValue,
ReferenceName1Value.Trim().ToUpper(),
ReferenceName2Value.Trim().ToUpper(),
ReferenceName3Value.Trim().ToUpper(),
ReferenceName4Value.Trim().ToUpper(),
MobileNo1Value,
MobileNo2Value,
MobileNo3Value,
MobileNo4Value,
loggedinUser,
guestCodeValue

                );

                bool invalidEmpResult = insertResult.ToUpper().Contains("no emp exist".ToUpper());
                bool successResult = insertResult.ToUpper().Contains("success".ToUpper());
                bool accessResult = insertResult.ToUpper().Contains("access".ToUpper());
                bool duplicateResult = insertResult.ToUpper().Contains("duplicate".ToUpper());

                if (invalidEmpResult)
                {
                    string alertMsg = "Enter a valid DT for a valid business code";
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = "text-danger";
                    return;
                }
                else if (successResult)
                {
                    //string input = "Data Inserted successfully --> 42631437 | AH2484825 | 42631437001";
                    string[] parts = insertResult.Split(new string[] { "-->", "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 3)  
                    {
                        string value1 = parts[0].Trim(); // Data Inserted successfully
                        string value2 = parts[1].Trim(); // 42631437
                        string value3 = parts[2].Trim(); // AH2484825
                        string value4 = parts[3].Trim(); // 42631437001

                        int validRowForMemIn = GetValidRowCount(ngfd_gvClients);
                        if (validRowForMemIn > 0)
                        {
                            isMemberValidattionPssed = MemberInsertOnHeadInsert(ngfd_gvClients, value3, value4);
                        }

                        //if (newMemGrid_IL_familyGridView.Rows.Count > 0)
                        //{
                        //    //InsertMember();
                        //    INSERT_MEMBER_WITH_HEAD(value3, value4);
                        //}

                    }

                    ShowAlert(insertResult);
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = "text-success";
                    try
                    {

                        string NewAHCode = ExtractAHCodeAndReturn(insertResult);
                        FillClientDataByAHNum(NewAHCode);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (accessResult)
                {
                    ShowAlert(insertResult);
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = "text-danger";
                }
                else if (duplicateResult)
                {
                    ShowAlert(insertResult);
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = "text-warning";
                   
                    if (insertResult.ToUpper().Contains("pan".ToUpper()))
                    {
                        if (insertResult.ToUpper().Contains("families".ToUpper()))
                        {
                            itfAOGuardianPANNO.Focus();

                        }
                        else
                        {
                            itfAOClientPan.Focus();
                        }
                    }

                    if (insertResult.ToUpper().Contains("mobile".ToUpper()))
                    {
                        MobileNo.Focus();
                    }

                    if (insertResult.ToUpper().Contains("email".ToUpper()))
                    {
                        txtEmail.Focus();
                    }
                    if (insertResult.ToUpper().Contains("aadhar".ToUpper()))
                    {
                        txtAadhar.Focus();
                    }


                }

                else
                {
                    ShowAlert(insertResult);
                    lblMessage.Text = insertResult;
                    lblMessage.CssClass = "text-danger";

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
            UPDATE_CLIENT_DATA();
        }
        protected void UPDATE_CLIENT_DATA()
        {
            try
            {
                #region Input Client Data for update
                string searchClientCodeValue = txtSearchClientCode.Text.Trim();
                string dtNumberValue = string.IsNullOrEmpty(txtDTNumber.Text) ? null : txtDTNumber.Text;
                string guestCodeValue       = string.IsNullOrEmpty(txtGuestCode.Text) ? null : txtGuestCode.Text;
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
                bool PermanentAddressSameAsMailing = chkSameAsMailing.Checked;
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

                #region Validations
                if (string.IsNullOrEmpty(txtClientCode.Text) || string.IsNullOrEmpty(txtSearchClientCode.Text))
                {
                    string message = "First load client data";
                    ShowAlert(message);
                    return; 
                }
             

                if (string.IsNullOrEmpty(ddlOccupation.SelectedValue))
                {
                    string message = "Occupation is required";
                    ShowAlert(message); 
                    ddlOccupation.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOCategoryStatus.SelectedValue))
                {
                    string message = "Category status is required";
                    ShowAlert(message); 
                    ddlAOCategoryStatus.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOClientCategory.SelectedValue))
                {
                    string message = "Client category is required";
                    ShowAlert(message); 
                    ddlAOClientCategory.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(accountName))
                {
                    string message = "Client name is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    txtAccountName.Focus();
                    return;
                }
               
                if (string.IsNullOrEmpty(cldDOB.Text))
                {
                    string message = "DOB is required";
                    ShowAlert(message);
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message-label-error";
                    cldDOB.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlAOGender.Text))
                {
                    string message = "Gender is required";
                    ShowAlert(message);  
                    ddlAOGender.Focus();
                    return;
                }
                if (itfAOGuardianPerson.Enabled && string.IsNullOrEmpty(guardianPerson))
                {
                    string message = "Guardian name is required";
                    ShowAlert(message);
                    ddlAOGender.Focus();
                    return;
                }


                if (string.IsNullOrEmpty(MobileNo.Text))
                {
                    string message = "Mobile number is required";
                    ShowAlert(message); 
                    MobileNo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(ddlMailingCountryList.SelectedValue))
                {
                    string message = "Mailing Country is required";
                    ShowAlert(message); 
                    ddlMailingCountryList.Focus();
                    return;
                }
                


                if (!IsAllZeros(txtAadhar.Text))
                {
                    ShowAlert("Invalid Aadhaar Number (Must be 12-digit numeric)");
                    return;

                }
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    string message = "Email is required";
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

                #endregion


                #region CHECK VALID AND DUPLICATE
                string isClientValidationPass = new WM.Controllers.AccountOpeningController().ValidateUpdateClientData(
                    searchClientCodeValue,
dtNumberValue,
guestCodeValue,
existClientCodeValue,
businessCodeValue,
taxStatus,
occupationValue,
statusCategoryValue,
clientCategoryValue,
accountCategoryValue,
selectedSalutation1,
accountName.Trim().ToUpper(),
selectedSalutation2,
accountFatherName.Trim().ToUpper(),
accountOtherValue.Trim().ToUpper(),
gender,
maritalStatus,
nationality,
residentNri,
dob,
annualIncome,
clientPan,
leadType,
guardianPerson.Trim().ToUpper(),
guardianPersonNationality.Trim().ToUpper(),
guardianPersonPan,
mailingAddress1.Trim().ToUpper(),
mailingAddress2.Trim().ToUpper(),
mailingState,
mailingCity,
mailingPinCode,
permanentAddress1.Trim().ToUpper(),
permanentAddress2.Trim().ToUpper(),
permanentState,
permanentCity,
PermanentPinCode,
NRIAddress.Trim().ToUpper(),
FaxValue,
AadharValue,
EmailValue.Trim().ToUpper(),
EmailOfficialValue.Trim().ToUpper(),
PhoneOfficeSTDValue,
PhoneOfficeNumberValue,
PhoneResSTDValue,
PhoneResNumberValue,
MobileNoValue,
ReferenceName1Value.Trim().ToUpper(),
ReferenceName2Value.Trim().ToUpper(),
ReferenceName3Value.Trim().ToUpper(),
ReferenceName4Value.Trim().ToUpper(),
MobileNo1Value,
MobileNo2Value,
MobileNo3Value,
MobileNo4Value,
loggedinUser
                    );

                string isMemberValidattionPssed = "";
                if (!isClientValidationPass.ToUpper().Contains("PASS"))
                {

                    ShowAlert(isClientValidationPass);
                    return;
                }

                int validRowCount = GetValidRowCount(ngfd_gvClients);
                if (validRowCount > 0)
                {
                    isMemberValidattionPssed = ValidateRequiredFieldsForFirstValidRow(ngfd_gvClients);

                    if (!isMemberValidattionPssed.ToUpper().Contains("PASS"))
                    {

                        ShowAlert(isMemberValidattionPssed);
                        return;
                    }
                }

                #endregion

                #region isUpdated client data Controller
                string isUpdated = new WM.Controllers.AccountOpeningController().UpdateClientData(
searchClientCodeValue,
dtNumberValue,
guestCodeValue,
existClientCodeValue,
businessCodeValue,
taxStatus, 
occupationValue, 
statusCategoryValue, 
clientCategoryValue,
accountCategoryValue, 
selectedSalutation1, 
accountName.Trim().ToUpper(), 
selectedSalutation2, 
accountFatherName.Trim().ToUpper(), 
accountOtherValue.Trim().ToUpper(),
gender, 
maritalStatus, 
nationality, 
residentNri, 
dob, 
annualIncome, 
clientPan, 
leadType, 
guardianPerson.Trim().ToUpper(),
guardianPersonNationality.Trim().ToUpper(), 
guardianPersonPan, 
mailingAddress1.Trim().ToUpper(), 
mailingAddress2.Trim().ToUpper(), 
mailingState, 
mailingCity, 
mailingPinCode,
permanentAddress1.Trim().ToUpper(),
permanentAddress2.Trim().ToUpper(),
permanentState,
permanentCity,
PermanentPinCode,
NRIAddress.Trim().ToUpper(),
FaxValue,
AadharValue,
EmailValue.Trim().ToUpper(),
EmailOfficialValue.Trim().ToUpper(),
PhoneOfficeSTDValue,
PhoneOfficeNumberValue,
PhoneResSTDValue,
PhoneResNumberValue,
MobileNoValue,
ReferenceName1Value.Trim().ToUpper(),
ReferenceName2Value.Trim().ToUpper(),
ReferenceName3Value.Trim().ToUpper(),
ReferenceName4Value.Trim().ToUpper(),
MobileNo1Value,
MobileNo2Value,
MobileNo3Value,
MobileNo4Value,
loggedinUser


                );

                #endregion

                if (isUpdated.ToUpper().Contains("SUCCESSFULLY"))
                {
                    int validRowForMemIn = GetValidRowCount(ngfd_gvClients);
                    if (validRowForMemIn > 0)
                    {
                        isMemberValidattionPssed = MemberInsertUpdateOnHeadUpdate(ngfd_gvClients);
                    }


                   

                    ShowAlert(isUpdated);
                    try
                    {

                        FillClientDataByAHNum(txtSearchClientCode.Text);
                    }
                    catch (Exception ex) { }

                }

                else if (isUpdated.ToUpper().Contains("Access".ToUpper()))
                {
                    ShowAlert(isUpdated);
                    return;
                }
                else if (isUpdated.ToUpper().Contains("Duplicate".ToUpper()))
                {
                    if (isUpdated.ToUpper().Contains("families".ToUpper()))
                    {
                        ShowAlert(isUpdated);
                        itfAOGuardianPANNO.Focus();

                    }
                    else
                    {

                    ShowAlert(isUpdated);
                    if (isUpdated.ToUpper().Contains("pan".ToUpper()))
                    {
                        itfAOClientPan.Focus();
                        //ScriptManager.RegisterStartupScript(this, this.GetType(), "TabPress", "moveFocusToNextField();", true);

                    }

                    if (isUpdated.ToUpper().Contains("mobile".ToUpper()))
                    {
                        MobileNo.Focus();
                    }

                    if (isUpdated.ToUpper().Contains("email".ToUpper()))
                    {
                        txtEmail.Focus();
                    }
                    if (isUpdated.ToUpper().Contains("aadhar".ToUpper()))
                    {
                        txtAadhar.Focus();
                    }
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



        #region Client List  MODEL SEARCH


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