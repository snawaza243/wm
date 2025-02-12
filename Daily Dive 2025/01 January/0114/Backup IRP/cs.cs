using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;

using Microsoft.Reporting.WebForms;


namespace WM.Masters
{
    public partial class insurance_renewal_letter_printed : System.Web.UI.Page
    {
        private readonly OracleConnection _connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillCompanyList();
                FillYearMonths();
            }

        }
        #endregion

        protected void btnPrint_Click_Report(object sender, EventArgs e)
        {
            // Call the PrintReceipt method to generate and download the report
            PrintReceipt();
        }

        public void PrintReceipt()
        {
            // Step 1: Get the DataTable by calling the GetItemList method
            DataTable dt = GetItemList();

            // Step 2: Set up the RDLC report (load and configure it)
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/CompanyReport.rdlc"); // Load your RDLC report

            // Step 3: Clear existing data sources (if any)
            localReport.DataSources.Clear();

            // Step 4: Add the DataTable as a ReportDataSource to the report
            ReportDataSource rds = new ReportDataSource("DataSet1", dt); // Ensure "YourDataSetName" matches your RDLC's dataset name
            localReport.DataSources.Add(rds);

            // Step 5: Render the report into a PDF
            string mimeType, encoding, fileNameExtension;
            string[] streams;

            // Use fully qualified type to avoid ambiguity
             Microsoft.Reporting.WebForms.Warning[] warnings;

            byte[] renderedBytes = localReport.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings
            );

            // Step 6: Send the PDF file as a response for download
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=CompanyReport.pdf");
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }

        public DataTable GetItemList()
        {
            DataTable dt = new DataTable();

            // Define the columns for the DataTable
            dt.Columns.Add("COMPANY_CD", typeof(string));
            dt.Columns.Add("STATUS_CD", typeof(string));
            dt.Columns.Add("LOCATION", typeof(string));
            dt.Columns.Add("POLICY_NO", typeof(string));
            dt.Columns.Add("CL_NAME", typeof(string));

            // Populate the DataTable with data from the GridView
            foreach (GridViewRow row in companyGridView.Rows)
            {
                DataRow dr = dt.NewRow();

                // Retrieve and assign values from the GridView row
                dr["COMPANY_CD"] = ((Label)row.FindControl("lblCompanyCode")).Text;
                dr["STATUS_CD"] = ((Label)row.FindControl("lblStatus")).Text;
                dr["LOCATION"] = ((Label)row.FindControl("lblLocation")).Text;
                dr["POLICY_NO"] = ((Label)row.FindControl("lblPolicyNo")).Text;
                dr["CL_NAME"] = ((Label)row.FindControl("lblClientName")).Text;

                // Add the populated DataRow to the DataTable
                dt.Rows.Add(dr);
            }

            return dt;
        }


        #region FillYearMonths
        private void FillYearMonths()
        {

            // For months dropdown
            // ddlMonths.DataSource = new WM.Controllers.InsuranceCompanyMasterController().GetMonthsList();
            //  ddlMonths.DataBind();

            // For years dropdown
            ddlYears.DataSource = new WM.Controllers.InsuranceRenewalLetterPrintedController().GetYearsList(45);
            ddlYears.DataBind();

        }
        #endregion

        #region FillCompanyList
        private void FillCompanyList()
        {
            DataTable dt = new WM.Controllers.InsuranceRenewalLetterPrintedController().GetCompanyListByCategoryL();
            AddDefaultItem(dt, "COMPANY_NAME", "COMPANY_CD", "Select");

            selectCompany.DataSource = dt;
            selectCompany.DataTextField = "COMPANY_NAME";
            selectCompany.DataValueField = "COMPANY_CD";
            selectCompany.DataBind();
        }
        #endregion


        #region AddDefaultItem
        private void AddDefaultItem(DataTable dt, string textField, string valueField, string defaultText)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                // If DataTable contains rows, add the default item
                DataRow dr = dt.NewRow();
                dr[textField] = defaultText;
                dr[valueField] = DBNull.Value;  // Use DBNull.Value or empty string as needed

                // Insert the default row at the first position
                dt.Rows.InsertAt(dr, 0);
            }
            else
            {
                // If DataTable is empty or null, handle accordingly (optional)
                Console.WriteLine("No data available to add a default item.");
            }
        }
        #endregion



        protected void companyGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectPolicy")
            {

                // Get the row index from the CommandArgument
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = companyGridView.Rows[rowIndex];

                // Retrieve values from the selected row
                string companyCode = row.Cells[1].Text;

                // Perform your action with the selected row data
                // For example, you might want to redirect to a details page or perform an update
                //Response.Redirect($"Details.aspx?CompanyCode={companyCode}");
            }
        }


        protected void btnGenerateAll_Click(object sender, EventArgs e)
        {
            if (companyGridView.Rows.Count > 0)
            {

            foreach (GridViewRow row in companyGridView.Rows)
            {
                #region All print letter fields value
                string arBranchCode = ((Label)row.FindControl("lblARBranchCode"))?.Text.Trim() ?? string.Empty;
                string companyCode = ((Label)row.FindControl("lblCompanyCode"))?.Text.Trim() ?? string.Empty;
                string clientName = ((Label)row.FindControl("lblClientName"))?.Text.Trim() ?? string.Empty;
                string address1 = ((Label)row.FindControl("lblAddress1"))?.Text.Trim() ?? string.Empty;
                string address2 = ((Label)row.FindControl("lblAddress2"))?.Text.Trim() ?? string.Empty;
                string clAdd3 = ((Label)row.FindControl("lblCLAdd3"))?.Text.Trim() ?? string.Empty;
                string clAdd4 = ((Label)row.FindControl("lblCLAdd4"))?.Text.Trim() ?? string.Empty;
                string clAdd5 = ((Label)row.FindControl("lblCLAdd5"))?.Text.Trim() ?? string.Empty;
                string phone1 = ((Label)row.FindControl("lblPhone1"))?.Text.Trim() ?? string.Empty;
                string phone2 = ((Label)row.FindControl("lblPhone2"))?.Text.Trim() ?? string.Empty;
                string cityName = ((Label)row.FindControl("lblCityName"))?.Text.Trim() ?? string.Empty;
                string dueDate = ((Label)row.FindControl("lblDueDate"))?.Text.Trim() ?? string.Empty;
                string remFlag = ((Label)row.FindControl("lblRemFlag"))?.Text.Trim() ?? string.Empty;
                string stateName = ((Label)row.FindControl("lblStateName"))?.Text.Trim() ?? string.Empty;
                string companyName = ((Label)row.FindControl("lblCompanyName"))?.Text.Trim() ?? string.Empty;
                string favourName = ((Label)row.FindControl("lblFavourName"))?.Text.Trim() ?? string.Empty;
                string branchName = ((Label)row.FindControl("lblBranchName"))?.Text.Trim() ?? string.Empty;
                string branchAdd1 = ((Label)row.FindControl("lblBranchAdd1"))?.Text.Trim() ?? string.Empty;
                string branchAdd2 = ((Label)row.FindControl("lblBranchAdd2"))?.Text.Trim() ?? string.Empty;
                string planName1 = ((Label)row.FindControl("lblPlanName1"))?.Text.Trim() ?? string.Empty;
                string payMode = ((Label)row.FindControl("lblPayMode"))?.Text.Trim() ?? string.Empty;
                string policyNo = ((Label)row.FindControl("lblPolicyNo"))?.Text.Trim() ?? string.Empty;
                string pName = ((Label)row.FindControl("lblPName"))?.Text.Trim() ?? string.Empty;
                string iName = ((Label)row.FindControl("lblIName"))?.Text.Trim() ?? string.Empty;
                string premFreq = ((Label)row.FindControl("lblPremFreq"))?.Text.Trim() ?? string.Empty;
                string bPremFreq = ((Label)row.FindControl("lblBPremFreq"))?.Text.Trim() ?? string.Empty;
                string planName = ((Label)row.FindControl("lblPlanName"))?.Text.Trim() ?? string.Empty;
                string sa = ((Label)row.FindControl("lblSA"))?.Text.Trim() ?? string.Empty;
                string premAmt = ((Label)row.FindControl("lblPremAmt"))?.Text.Trim() ?? string.Empty;
                string monNo = ((Label)row.FindControl("lblMonNo"))?.Text.Trim() ?? string.Empty;
                string yearNo = ((Label)row.FindControl("lblYearNo"))?.Text.Trim() ?? string.Empty;
                string clPin = ((Label)row.FindControl("lblCLPin"))?.Text.Trim() ?? string.Empty;
                string pin1 = ((Label)row.FindControl("lblPin1"))?.Text.Trim() ?? string.Empty;
                string invCode = ((Label)row.FindControl("lblInvCode"))?.Text.Trim() ?? string.Empty;
                string invCode1 = ((Label)row.FindControl("lblInvCode1"))?.Text.Trim() ?? string.Empty;
                string importDataType = ((Label)row.FindControl("lblImportDataType"))?.Text.Trim() ?? string.Empty;

                #endregion
                // Generate a report for each row
                GenerateReport(
                    arBranchCode,
                    companyCode,
                    clientName,
                    address1,
                    address2,
                    clAdd3,
                    clAdd4,
                    clAdd5,
                    phone1,
                    phone2,
                    cityName,
                    dueDate,
                    remFlag,
                    stateName,
                    companyName,
                    favourName,
                    branchName,
                    branchAdd1,
                    branchAdd2,
                    planName1,
                    payMode,
                    policyNo,
                    pName,
                    iName,
                    premFreq,
                    bPremFreq,
                    planName,
                    sa,
                    premAmt,
                    monNo,
                    yearNo,
                    clPin,
                    pin1,
                    invCode,
                    invCode1,
                    importDataType

                    );
            }
            }

        }

        private void GenerateReport(
        #region Item paramter
            string arBranchCode,
            string companyCode,
            string clientName,
            string address1,
            string address2,
            string clAdd3,
            string clAdd4,
            string clAdd5,
            string phone1,
            string phone2,
            string cityName,
            string dueDate,
            string remFlag,
            string stateName,
            string companyName,
            string favourName,
            string branchName,
            string branchAdd1,
            string branchAdd2,
            string planName1,
            string payMode,
            string policyNo,
            string pName,
            string iName,
            string premFreq,
            string bPremFreq,
            string planName,
            string sa,
            string premAmt,
            string monNo,
            string yearNo,
            string clPin,
            string pin1,
            string invCode,
            string invCode1,
            string importDataType
#endregion
            )
        {
            // Create an HTML layout matching the format
            string htmlContent = $@"
        <html>
            <body>
                <table>
                    <tr>
                        <td>Client Name:</td>
                        <td>{clientName}</td>
                    </tr>
                    <tr>
                        <td>Address:</td>
                        <td>{address1}</td>
                    </tr>
                    <!-- Add other rows here -->
                </table>
            </body>
        </html>";

            // Show in a new window or render directly
            Response.Clear();
            Response.ContentType = "application/vnd.ms-word";
            Response.AddHeader("content-disposition", "attachment;filename=Report.doc");
            Response.Write(htmlContent);
            Response.End();
        }




        // Generate list 
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {   // Retrieve parameters from form controls
                string companyCode = selectCompany.SelectedValue;
                string monthText = ddlMonths.SelectedValue;
                string yearText = ddlYears.SelectedValue;
                string policytNumber = txtPolicyNumber.Text.Trim().ToString();
                string rem_flag = chckRemFlag.Checked ? "1" : "0";
                string loggedInUser = Session["LoginId"]?.ToString();


                // Get the employee list
                DataTable dt = new InsuranceRenewalLetterPrintedController().GenCompanyDataCMY(companyCode, monthText, yearText, policytNumber, rem_flag, loggedInUser);
                //lblMessage.Text = $"Selected company {companyCode}, month {monthText} and year {yearText}";

                // Bind data to GridView
                companyGridView.Visible = true;
                companyGridView.DataSource = dt;
                companyGridView.DataBind();
            }
            catch (Exception ex)
            {
                // lblMessage.Text = "An error occurred: " + ex.Message;
            }
        }


        protected void btnReset_Click(object sender, EventArgs e)
        {
            selectCompany.SelectedValue = string.Empty;
            ddlMonths.SelectedValue = string.Empty;
            lblMessage.Text = "";

            // Reset GridView
            companyGridView.DataSource = null;
            companyGridView.DataBind();

        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/welcome.aspx");
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

        protected void btnMark_Click(object sender, EventArgs e)
        {
            string companyCode = selectCompany.SelectedValue;
            string monthText = ddlMonths.SelectedValue;
            string yearText = ddlYears.SelectedValue;
            string policyNo = txtPolicyNumber.Text;

            if (string.IsNullOrEmpty(companyCode) || (string.IsNullOrEmpty(monthText) && string.IsNullOrEmpty(policyNo)) || string.IsNullOrEmpty(yearText) || companyGridView == null)
            {
                lblMessage.Text = "Please select all values.";
                lblMessage.Visible = true;
                selectCompany.Focus();
            }


            else
            {
                // Call the UpdateReminderFlag method from the controller
                InsuranceRenewalLetterPrintedController controller = new InsuranceRenewalLetterPrintedController();
                DataTable dt = controller.UpdateReminderFlag(companyCode, monthText, int.TryParse(yearText, out int year) ? year : (int?)null, string.IsNullOrEmpty(policyNo) ? null : policyNo);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Display the message from the DataTable
                    string msg = GetTextFieldValue(dt.Rows[0], "Message");
                    ShowAlert(msg);
                    lblMessage.Text = msg;
                }
                else
                {
                    string msg = "No records updated";
                    ShowAlert(msg);
                    lblMessage.Text = msg;
                }

                lblMessage.Visible = true;

                // Optionally hide the label after some time
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideLabel", "hideMessage();", true);
            }

        }




        #region GenerateRenewalLetter

        public void GenerateRenewalLetterByGridOrPolicy(GridView policyGrid, string currentPolicyNo)
        {
            int rowIndex = Convert.ToInt32(currentPolicyNo);
            GridViewRow row = companyGridView.Rows[rowIndex];

            //string companyCode = row.Cells[1].Text;

            #region All print letter fields value
            string arBranchCode = ((Label)row.FindControl("lblARBranchCode"))?.Text.Trim() ?? string.Empty;
            string companyCode = ((Label)row.FindControl("lblCompanyCode"))?.Text.Trim() ?? string.Empty;
            string clientName = ((Label)row.FindControl("lblClientName"))?.Text.Trim() ?? string.Empty;
            string address1 = ((Label)row.FindControl("lblAddress1"))?.Text.Trim() ?? string.Empty;
            string address2 = ((Label)row.FindControl("lblAddress2"))?.Text.Trim() ?? string.Empty;
            string clAdd3 = ((Label)row.FindControl("lblCLAdd3"))?.Text.Trim() ?? string.Empty;
            string clAdd4 = ((Label)row.FindControl("lblCLAdd4"))?.Text.Trim() ?? string.Empty;
            string clAdd5 = ((Label)row.FindControl("lblCLAdd5"))?.Text.Trim() ?? string.Empty;
            string phone1 = ((Label)row.FindControl("lblPhone1"))?.Text.Trim() ?? string.Empty;
            string phone2 = ((Label)row.FindControl("lblPhone2"))?.Text.Trim() ?? string.Empty;
            string cityName = ((Label)row.FindControl("lblCityName"))?.Text.Trim() ?? string.Empty;
            string dueDate = ((Label)row.FindControl("lblDueDate"))?.Text.Trim() ?? string.Empty;
            string remFlag = ((Label)row.FindControl("lblRemFlag"))?.Text.Trim() ?? string.Empty;
            string stateName = ((Label)row.FindControl("lblStateName"))?.Text.Trim() ?? string.Empty;
            string companyName = ((Label)row.FindControl("lblCompanyName"))?.Text.Trim() ?? string.Empty;
            string favourName = ((Label)row.FindControl("lblFavourName"))?.Text.Trim() ?? string.Empty;
            string branchName = ((Label)row.FindControl("lblBranchName"))?.Text.Trim() ?? string.Empty;
            string branchAdd1 = ((Label)row.FindControl("lblBranchAdd1"))?.Text.Trim() ?? string.Empty;
            string branchAdd2 = ((Label)row.FindControl("lblBranchAdd2"))?.Text.Trim() ?? string.Empty;
            string planName1 = ((Label)row.FindControl("lblPlanName1"))?.Text.Trim() ?? string.Empty;
            string payMode = ((Label)row.FindControl("lblPayMode"))?.Text.Trim() ?? string.Empty;
            string policyNo = ((Label)row.FindControl("lblPolicyNo"))?.Text.Trim() ?? string.Empty;
            string pName = ((Label)row.FindControl("lblPName"))?.Text.Trim() ?? string.Empty;
            string iName = ((Label)row.FindControl("lblIName"))?.Text.Trim() ?? string.Empty;
            string premFreq = ((Label)row.FindControl("lblPremFreq"))?.Text.Trim() ?? string.Empty;
            string bPremFreq = ((Label)row.FindControl("lblBPremFreq"))?.Text.Trim() ?? string.Empty;
            string planName = ((Label)row.FindControl("lblPlanName"))?.Text.Trim() ?? string.Empty;
            string sa = ((Label)row.FindControl("lblSA"))?.Text.Trim() ?? string.Empty;
            string premAmt = ((Label)row.FindControl("lblPremAmt"))?.Text.Trim() ?? string.Empty;
            string monNo = ((Label)row.FindControl("lblMonNo"))?.Text.Trim() ?? string.Empty;
            string yearNo = ((Label)row.FindControl("lblYearNo"))?.Text.Trim() ?? string.Empty;
            string clPin = ((Label)row.FindControl("lblCLPin"))?.Text.Trim() ?? string.Empty;
            string pin1 = ((Label)row.FindControl("lblPin1"))?.Text.Trim() ?? string.Empty;
            string invCode = ((Label)row.FindControl("lblInvCode"))?.Text.Trim() ?? string.Empty;
            string invCode1 = ((Label)row.FindControl("lblInvCode1"))?.Text.Trim() ?? string.Empty;
            string importDataType = ((Label)row.FindControl("lblImportDataType"))?.Text.Trim() ?? string.Empty;

            #endregion




        }

        public class ReportGenerateItems
        {
            public string arBranchCode{get;set;}
            public string companyCode{get;set;}
            public string clientName{get;set;}
            public string address1{get;set;}
            public string address2{get;set;}
            public string clAdd3{get;set;}
            public string clAdd4{get;set;}
            public string clAdd5{get;set;}
            public string phone1{get;set;}
            public string phone2{get;set;}
            public string cityName{get;set;}
            public string dueDate{get;set;}
            public string remFlag{get;set;}
            public string stateName{get;set;}
            public string companyName{get;set;}
            public string favourName{get;set;}
            public string branchName{get;set;}
            public string branchAdd1{get;set;}
            public string branchAdd2{get;set;}
            public string planName1{get;set;}
            public string payMode{get;set;}
            public string policyNo{get;set;}
            public string pName{get;set;}
            public string iName{get;set;}
            public string premFreq{get;set;}
            public string bPremFreq{get;set;}
            public string planName{get;set;}
            public string sa{get;set;}
            public string premAmt{get;set;}
            public string monNo{get;set;}
            public string yearNo{get;set;}
            public string clPin{get;set;}
            public string pin1{get;set;}
            public string invCode{get;set;}
            public string invCode1{get;set;}
            public string importDataType { get; set; }
        }
        public void GenerateRenewalLetter(int month, int year, string companyCode, string companyName, string loginId)
        {
            try
            {
                string selectionFormula = $"{{bajaj_due_data.mon_no}}={month} and {{bajaj_due_data.year_no}}={year} and {{bajaj_due_data.importdatatype}}='DUEDATA'";
                if (!string.IsNullOrEmpty(companyCode))
                {
                    selectionFormula += $" and {{bajaj_Due_Data.Company_Cd}}='{companyCode}'";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:", ex.Message);
            }
        }
        #endregion

        #region GetCompanyList
        public DataTable GetCompanyList()
        {
            DataTable dt = new DataTable();

            using (OracleCommand cmd = new OracleCommand("Get_Company_List", _connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("company_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                try
                {
                    _connection.Open();
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:", ex.Message);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        _connection.Close();
                    }
                }
            }

            return dt;
        }
        #endregion
    }
}
