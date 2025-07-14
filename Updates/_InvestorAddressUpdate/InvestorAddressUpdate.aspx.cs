using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media.TextFormatting;
using WM.Controllers;

namespace WM.Tree
{
    public partial class InvestorAddressUpdate : System.Web.UI.Page
    {
        PsmController pc = new PsmController();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStates();
                BindCities();

                string currentInv = Session["PSM_IAU_INV_CODE"] as string;

                if (!string.IsNullOrEmpty(currentInv))
                {
                    BindInvOnload(currentInv);
                }
            }
        }

        private void BindInvOnload(string inv)
        {
            try
            {
                PsmController pc = new PsmController();
                DataTable invData = pc.PsmUidByInv(inv, "0", "frmARGeneral"); // action = "0" for GET

                if (invData != null && invData.Rows.Count > 0)
                {
                    DataRow row = invData.Rows[0];
                    hdnInvestorCode.Value     = row["INVESTOR_CODE"].ToString();
                    hdnInvestorName.Value     = row["INVESTOR_NAME"].ToString();
                    txtAddress1.Text    = row["INVESTOR_ADD1"].ToString();
                    txtAddress2.Text    = row["INVESTOR_ADD2"].ToString();
                    txtPIN.Text         = row["INVESTOR_PINCODE"].ToString();
                    txtMobile.Text      = row["INVESTOR_MOBILE"].ToString();
                    txtEmail.Text       = row["INVESTOR_EMAIL"].ToString();
                    txtAadhar.Text      = row["INVESTOR_AADHAR"].ToString();
                    txtPAN.Text         = row["INVESTOR_PAN"].ToString();

                    if (row["INVESTOR_DOB"] != DBNull.Value)
                        txtDOB.Text = Convert.ToDateTime(row["INVESTOR_DOB"]).ToString("dd/MM/yyyy");

                    ddlState.SelectedValue = row["INVESTOR_STATE_ID"].ToString();
                    ddlCity.SelectedValue = row["INVESTOR_CITY_ID"].ToString();
                }
                else
                {
                    pc.ShowAlert(this, "No data found for the given investor.");
                    return;
                }
            }
            catch (Exception ex)
            {
                new PsmController().ShowAlert(this, ex.Message);
                return;
            }
        }


        private void UpdateInvestorDetails(string inv)
        {
            try
            {
                PsmController pc = new PsmController();
                DataTable result = pc.PsmUidByInv(
                    invCode: inv,
                    action: "1",
                    type: "frmARGeneral",
                    add1: txtAddress1.Text.Trim(),
                    add2: txtAddress2.Text.Trim(),
                    pin: txtPIN.Text.Trim(),
                    state: ddlState.SelectedValue,
                    city: ddlCity.SelectedValue,
                    email: txtEmail.Text.Trim(),
                    mobile: txtMobile.Text.Trim(),
                    pan: txtPAN.Text.Trim(),
                    aadhar: txtAadhar.Text.Trim(),
                    dob: txtDOB.Text.Trim()
                );

                if (result != null && result.Rows.Count > 0)
                {
                    string msg = result.Rows[0]["MESSAGE"].ToString();
                    pc.ShowAlert(this, msg);
                }
                else
                {
                    pc.ShowAlert(this, "Update operation did not return any message.");
                }
            }
            catch (Exception ex)
            {
                new PsmController().ShowAlert(this, ex.Message);
            }
        }


        private void BindStates()
        {
            // TRY CATCH AND
            try
            {
                PsmController pc = new PsmController();
                DataTable stateMaster = pc.PsmStateMaster(null, null);
                dtBindToDdl(ddlState, stateMaster, "STATE_NAME", "STATE_ID", "Select State", "");
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }

        }

        private void BindCities()
        {
            // try catch
            try
            {
                PsmController pc = new PsmController();
                DataTable cityMaster = pc.PsmCityMaster(null,null, null);
                dtBindToDdl(ddlCity, cityMaster, "CITY_NAME", "CITY_ID", "Select City", "");

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }

        protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCityId = ddlCity.SelectedValue;

            try
            {
                if (!string.IsNullOrEmpty(selectedCityId))
                {
                    PsmController pc = new PsmController();

                    // Call with only city parameter
                    DataTable cityData = pc.PsmCityMaster(null, null, selectedCityId);

                    if (cityData != null && cityData.Rows.Count > 0)
                    {
                        string relatedStateId = cityData.Rows[0]["STATE_ID"].ToString();

                        // Select the corresponding state in ddlState
                        ddlState.SelectedValue = relatedStateId;
                    }
                }
            }
            catch (Exception ex)
            {
                PsmController pc = new PsmController();
                pc.ShowAlert(this, "Error while auto-selecting state: " + ex.Message);
            }
        }

        protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStateId = ddlState.SelectedValue;  

            try
            {
                if (!string.IsNullOrEmpty(selectedStateId))
                {
                    PsmController pc = new PsmController();

                    // Get cities filtered by selected state
                    DataTable cityData = pc.PsmCityMaster(null, selectedStateId, null);

                    // Bind cities to ddlCity
                    dtBindToDdl(ddlCity, cityData, "CITY_NAME", "CITY_ID", "Select City", "");
                }
                else
                {
                    BindCities();
                }
            }
            catch (Exception ex)
            {
                PsmController pc = new PsmController();
                pc.ShowAlert(this, "Error while loading cities: " + ex.Message);
            }
        }


        public void dtBindToDdl(DropDownList ddl, DataTable dt, string textField, string valueField, string defaultText = "Select", string defaultValue = "")
        {
            ddl.Items.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                // Check if specified columns exist
                if (!dt.Columns.Contains(textField) || !dt.Columns.Contains(valueField))
                {
                    ddl.Items.Clear();
                    ddl.Items.Add(new ListItem("Not matched dt col name", ""));
                    return;
                }

                ddl.DataSource = dt;
                ddl.DataTextField = textField;
                ddl.DataValueField = valueField;
                ddl.DataBind();

                if (!string.IsNullOrEmpty(defaultText))
                {
                    // Insert default item at the top
                    ddl.Items.Insert(0, new ListItem(defaultText, defaultValue));
                }
            }
            else
            {
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("No rows", ""));
            }
        }


        protected void calDOB_SelectionChanged(object sender, EventArgs e)
        {
            string valueDob = txtDOB.Text ;
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string invCode = hdnInvestorCode.Value.ToString();
                if (string.IsNullOrEmpty(invCode))
                {
                    new PsmController().ShowAlert(this, "Investor code is not set.");
                    return;
                }
                else
                {
                    UpdateInvestorDetails(invCode);
                    return;
                }
            }
            catch (Exception ex)
            {
                new PsmController().ShowAlert(this, ex.Message);
                return;
            }

            /*
            // Basic validation can be added here
            string address = txtAddress1.Text.Trim();
            string pin = txtPIN.Text.Trim();
            string city = ddlCity.SelectedValue;
            string state = ddlState.SelectedValue;
            string mobile = txtMobile.Text.Trim();
            string email = txtEmail.Text.Trim();
            string aadhar = txtAadhar.Text.Trim();
            string pan = txtPAN.Text.Trim();
            string dob = txtDOB.Text.Trim();

            // Save logic (DB insert/update)
            // Example: SaveToDatabase(address, pin, city, state, ...);
            // Show confirmation
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Investor detail updated successfully.');", true);
            */
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Remove("PSM_IAU_INV_CODE");
                pc.CloseWindow();
                return;
            }
            catch (Exception ex)
            {
                new PsmController().ShowAlert(this, ex.Message);
                return;
            }
        }

    }

}



