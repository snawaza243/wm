using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Collections.Generic;

namespace WM.Masters
{
    public partial class insurance_renewal_letter_printed : System.Web.UI.Page
    {
        private readonly OracleConnection _connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
        #region Page_Load
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
                    FillCompanyList();
                    FillYearMonths();
                }
            }

        }
        #endregion



        private void LoadReport()
        {
            // Define the path of the RDLC file (inside Reports folder)
            //ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/RenewalLetterReport.rdlc");

            // Fetch data from database
            DataTable dt = new DataTable();
            ReportDataSource rds = new ReportDataSource("RenewalLetterDataSet", dt);

            // Clear and add new data source
            //ReportViewer1.LocalReport.DataSources.Clear();
            //ReportViewer1.LocalReport.DataSources.Add(rds);

            // Refresh the report
            //ReportViewer1.LocalReport.Refresh();
        }
        #region Print with RDLC
        public void PrintReceiptWithRDLC()
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

        #region FillYearMonths
        private void FillYearMonths()
        {

            // Get the current year
            int currentYear = DateTime.Now.Year;

            // Populate the dropdown
            ddlYears.DataSource = new WM.Controllers.DueAndPaidDataImportingController().GetYearsList(15, 2030);
            ddlYears.DataBind();

            if (ddlYears.Items.Count > 1)
            {
                // Set the selected value to the current year
                ListItem currentYearItem = ddlYears.Items.FindByValue(currentYear.ToString());
                if (currentYearItem != null)
                {
                    ddlYears.ClearSelection();
                    currentYearItem.Selected = true;
                }
            }

            ddlYears.Items.Insert(0, new ListItem("Select Year", ""));




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

        #region Generate Policy List
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // Call JavaScript to show loader and disable UI
                ScriptManager.RegisterStartupScript(this, this.GetType(), "showLoader", "showLoader();", true);

                string companyCode = selectCompany.SelectedValue;
                string monthText = ddlMonths.SelectedValue;
                string yearText = ddlYears.SelectedValue;
                string policyNo = txtPolicyNumber.Text.Trim().ToString();
                string rem_flag = chckRemFlag.Checked ? "1" : "0";
                string loggedInUser = Session["LoginId"]?.ToString();

                // Validate input fields
                if (string.IsNullOrEmpty(companyCode) && string.IsNullOrEmpty(policyNo))
                {
                    ShowAlert("Company or Policy Number is required to fetch");
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "hideLoader", "hideLoader();", true);
                    return;
                }

                if (!string.IsNullOrEmpty(companyCode))
                {
                    if (string.IsNullOrEmpty(monthText) || string.IsNullOrEmpty(yearText))
                    {
                        ShowAlert("Select month-year.");
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "hideLoader", "hideLoader();", true);
                        return;
                    }
                }

                // Fetch data from the controller
                DataTable dt = new InsuranceRenewalLetterPrintedController().GenCompanyDataCMY(
                    companyCode,
                    string.IsNullOrEmpty(monthText) ? (int?)null : Convert.ToInt32(monthText),
                    string.IsNullOrEmpty(yearText) ? (int?)null : Convert.ToInt32(yearText),
                    string.IsNullOrEmpty(policyNo) ? null : policyNo,
                    rem_flag, loggedInUser);

                // Process the data and display results
                if (dt.Rows.Count > 0)
                {
                    string gettingRowsCount = dt.Rows.Count.ToString();
                    string recordText = dt.Rows.Count == 1 ? "record" : "records";
                    lblMessage.Text = gettingRowsCount + " " + recordText + " found";
                    lblMessage.CssClass = "text-success";
                    Session["dt"] = dt;

                    DataTable currentdtdata = Session["dt"] as DataTable;
                    BindGridView(currentdtdata);   
                    //PrintBulkByDT(dt);
                    
                    btnPrint.Enabled = true;


                     
                }
                else
                {
                    lblMessage.Text = "No record found.";
                    lblMessage.CssClass = "text-dark";
                    btnPrint.Enabled = false;
                    Session["dt"] = null;

                    BindGridView(null); 
                }


            }
            catch (Exception ex)
            {
                // Handle any exceptions
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideLoader", "hideLoader();", true);
                ShowAlert("An error occurred while fetching the data.");
            }
            finally
            {
                // Ensure the loader is hidden after the operation (successful or not)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideLoader", "hideLoader();", true);
            }
        
        
        
        }

        #endregion

        private string GetDateFromDateTimeString(string dateTimeString)
        {
            if (DateTime.TryParse(dateTimeString, out DateTime parsedDate))
            {
                return parsedDate.ToString("dd-MM-yyyy");
            }
            else
            {
                return string.Empty;
            }
        }

        #region Click and Print Single Policy  Report

        public class PrintModel
        {
            public string arBranchCode {get;set;}
            public string companyCode {get;set;}
            public string clientName {get;set;}
            public string address1 {get;set;}
            public string address2 {get;set;}
            public string clAdd3 {get;set;}
            public string clAdd4 {get;set;}
            public string clAdd5 {get;set;}
            public string phone1 {get;set;}
            public string phone2 {get;set;}
            public string cityName {get;set;}
            public string dueDateBase {get;set;}
            public string remFlag {get;set;}
            public string stateName {get;set;}
            public string companyName {get;set;}
            public string favourName {get;set;}
            public string branchName {get;set;}
            public string branchAdd1 {get;set;}
            public string branchAdd2 {get;set;}
            public string planName1 {get;set;}
            public string payMode {get;set;}
            public string policyNo {get;set;}
            public string pName {get;set;}
            public string iName {get;set;}
            public string premFreq {get;set;}
            public string bPremFreq {get;set;}
            public string planName {get;set;}
            public string sa {get;set;}
            public string premAmt {get;set;}
            public string monNo {get;set;}
            public string yearNo {get;set;}
            public string clPin {get;set;}
            public string pin1 {get;set;}
            public string invCode {get;set;}
            public string invCode1 {get;set;}
            public string importDataType {get;set;}
            public string dueDate { get; set; }
        }

        // Pagination event handler
        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            companyGridView.PageIndex = e.NewPageIndex;
            DataTable currentdtdata = Session["dt"] as DataTable;
            BindGridView(currentdtdata);  // Rebind the data
        }

        private void BindGridView(DataTable currentdtdata)
        {

            if (currentdtdata != null && currentdtdata.Rows.Count > 0)
            {

                companyGridView.DataSource = currentdtdata;
                companyGridView.Visible = true;
                companyGridView.DataBind();
            }
            else
            {
                // Handle no data scenario
                companyGridView.DataSource = null;
                companyGridView.Visible = false;
                companyGridView.DataBind();
                ShowAlert("No records found."); // Optional message
            }
        }
        protected void companyGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectPolicy")
            {

                // Get the row index from the CommandArgument
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = companyGridView.Rows[rowIndex];

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
                string dueDateBase = ((Label)row.FindControl("lblDueDate"))?.Text.Trim() ?? string.Empty;
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
                string sa = ((Label)row.FindControl("lblSumAssured"))?.Text.Trim() ?? string.Empty;
                string premAmt = ((Label)row.FindControl("lblPremAmount"))?.Text.Trim() ?? string.Empty;
                string monNo = ((Label)row.FindControl("lblMonNo"))?.Text.Trim() ?? string.Empty;
                string yearNo = ((Label)row.FindControl("lblYearNo"))?.Text.Trim() ?? string.Empty;
                string clPin = ((Label)row.FindControl("lblCLPin"))?.Text.Trim() ?? string.Empty;
                string pin1 = ((Label)row.FindControl("lblPin1"))?.Text.Trim() ?? string.Empty;
                string invCode = ((Label)row.FindControl("lblInvCode"))?.Text.Trim() ?? string.Empty;
                string invCode1 = ((Label)row.FindControl("lblInvCode1"))?.Text.Trim() ?? string.Empty;
                string importDataType = ((Label)row.FindControl("lblImportDataType"))?.Text.Trim() ?? string.Empty;
                string dueDate = GetDateFromDateTimeString(dueDateBase);

                #endregion

                DataTable currentdtdata = Session["dt"] as DataTable;

                if (currentdtdata != null && currentdtdata.Rows.Count > 0)
                {
                    PrintBulkByDT(currentdtdata, policyNo);
                }
                else
                {
                    // Handle scenarios when data is missing or empty
                    // You can log, show a message, or trigger fallback logic
                    ShowAlert("No data available to print.");
                }
                // Generate a report for each row
                /*
                GenerateAndPrintPage(
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
                */
            }
        }
        #endregion

        #region Print All Listing Policy Report


        protected void btnGenerateAll_Click(object sender, EventArgs e)
        {

            // fetch dt by Session and pass to PrintBulkByDT

            DataTable currentdtdata = Session["dt"] as DataTable;

            if (currentdtdata != null && currentdtdata.Rows.Count > 0)
            {
                PrintBulkByDT(currentdtdata);
            }
            else
            {
                // Handle scenarios when data is missing or empty
                // You can log, show a message, or trigger fallback logic
                ShowAlert("No data available to print.");
            }

            //GenerateAllByGrid();
        }

        protected void GenerateAllByGrid()
        {
            if (companyGridView.Rows.Count > 0)
            {
                var pageNo = 1;
                string htmlContent = "<html><head><style>";

                htmlContent += @"
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }
            .page {
                page-break-before: always;
                padding: 20px;
                width: 210mm;
                height: 293mm;
                margin: auto;
/* border: 1px solid #ccc; */
                
            }
            
            .client-header{
                width: 80%;
                margin-top:50px;
                margin-left: 200px;
            }
            .client-header {
                text-align: left;
                margin-bottom: 20px;
            }

            .client-footer {
                text-align: left;
            }
            .client-content {
                
            }
            .client-content table {
                width: 100%;
                border-collapse: collapse;
                margin-top:70%;
            }
             table, th, td {
                border: 1px solid black;
            }
            th, td {
                padding: 3px;
                text-align: left;
            }
            .client-footer p {
                font-size: 0.9rem;
            }
            
            .foot-info {
            margin-top: 5PX;
            display: flex;
            justify-content: space-between;
        }

/* Print specific styles */
    @media print {
        /* Remove any page margins */
        body {
            margin: 0;
            padding: 0;
        }

        /* Ensure page margins are controlled */
        @page {
            size: A4;
            margin: 0;
        }

        /* Hide page numbers, date/time, and other browser-specific content */
        .header, .footer {
            display: none; /* Hide header and footer during printing */
        }

        /* Hide unwanted elements like page numbers, date and time that might be added by the browser */
        .page-number, .page-date {
            display: none;
        }

        /* Hide any browser-specific header/footer */
        .content {
            margin-top: 0;
        }

        /* Hide borders or other print styles that are unnecessary */
        table {
            margin-top: 0;
        }
    }
        </style></head><body>";

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
                    string dueDateBase = ((Label)row.FindControl("lblDueDate"))?.Text.Trim() ?? string.Empty;

                    string dueDate = GetDateFromDateTimeString(dueDateBase);

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
                    string sa = ((Label)row.FindControl("lblSumAssured"))?.Text.Trim() ?? string.Empty;
                    string premAmt = ((Label)row.FindControl("lblPremAmount"))?.Text.Trim() ?? string.Empty;
                    string monNo = ((Label)row.FindControl("lblMonNo"))?.Text.Trim() ?? string.Empty;
                    string yearNo = ((Label)row.FindControl("lblYearNo"))?.Text.Trim() ?? string.Empty;
                    string clPin = ((Label)row.FindControl("lblCLPin"))?.Text.Trim() ?? string.Empty;
                    string pin1 = ((Label)row.FindControl("lblPin1"))?.Text.Trim() ?? string.Empty;
                    string invCode = ((Label)row.FindControl("lblInvCode"))?.Text.Trim() ?? string.Empty;
                    string invCode1 = ((Label)row.FindControl("lblInvCode1"))?.Text.Trim() ?? string.Empty;
                    string importDataType = ((Label)row.FindControl("lblImportDataType"))?.Text.Trim() ?? string.Empty;
                    #endregion

                    // Each block of content wrapped in a page div
                    htmlContent += $@"
                <div class=""page"">
                    <div class=""client-header"">
                        <p>{clientName}</p>
                        <p>{address1}</p>
                        <p>{address2} {clAdd3}</p>
                        <p>{clAdd4} {clAdd5}</p>
                        <p>{cityName} {stateName}</p>
                        <div class=""phones"">
                            <p><b>Tel No.:</b> {phone1} <b>Phone:</b> {phone2}</p>
                        </div>
                    </div>

                    <div class=""client-content"">
                        <table>
                            <tr><th colspan=""2"">Proposer Name</th><td colspan=""2"">{clientName}</td></tr>
                            <tr><th colspan=""2"">Life Insured Name</th><td colspan=""2"">{iName}</td></tr>
                            <tr><th colspan=""2"">Insurer</th><td colspan=""2"">{companyName}</td></tr>
                            <tr><th colspan=""2"">Plan Name</th><td colspan=""2"">{planName1}</td></tr>
                            <tr><th colspan=""2"">Policy Number</th><td colspan=""2"">{policyNo}</td></tr>
                            <tr><th>Payment Mode</th><td>{payMode}</td><th>Payment Frequency</th><td>{premFreq}</td></tr>
                            <tr><th>Basic Sum Assured</th><td>{sa}</td><th>Premium Due Date</th><td>{dueDate}</td></tr>
                            <tr><th>Premium Amount*</th><td>{premAmt}</td><th>Bajaj Capital Service Unit</th><td>{branchName}</td></tr>
                            <tr><th>Service Unit Address</th><td colspan=""3"">{branchAdd1} {branchAdd2}</td></tr>
                            <tr><th>Cheque in favor of</th><td colspan=""3"">{favourName}</td></tr>
                        </table>
                    </div>

                    <div class=""client-footer"">
                        <p style=""font-style: italic;"">* Please ensure to verify the exact premium amount with the Insurance Company on account of change in GST rates on Insurance Premium w.e.f. 1st Jul 2017</p>
                        <div class=""foot-info"">
                            <p><b>Ref No:</b> {invCode}</p>
                            <p>PAGE NO: {pageNo}</p>
                        </div>
                    </div>
                </div>";
                    pageNo += 1;
                }

                // Closing the HTML tags properly
                htmlContent += "</body></html>";

                // Encoding HTML and preparing print script 
                string printScript = $@"
var blob = new Blob([`{htmlContent}`], {{ type: 'text/html' }});
var url = URL.createObjectURL(blob);
var printWindow = window.open(url, '', 'height=600,width=800');
printWindow.onload = function() {{
    printWindow.print();
    URL.revokeObjectURL(url);
}};

// Wait for the print window to load before triggering print
            printWindow.onafterprint = function() {{
                // Close the print window after printing is finished
                printWindow.close();
            }};
            printWindow.print();
";


                string printScript0 = $@"
            var printWindow = window.open('', '', 'height=600,width=800');
            printWindow.document.open();
            printWindow.document.write(decodeURIComponent('{htmlContent}'));
            printWindow.document.close();
            printWindow.focus();

// Wait for the print window to load before triggering print
            printWindow.onafterprint = function() {{
                // Close the print window after printing is finished
                printWindow.close();
            }};
            printWindow.print();



        ";

                // Register the script to run in the browser
                ScriptManager.RegisterStartupScript(this, this.GetType(), "PrintScript", printScript, true);
            }


        }
        public string ReplaceSpecialCharacters(string input, string specialChars, char replaceWith)
        {
            // Null or empty check
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(specialChars))
            {
                return input; // Return original string if no replacement required
            }

            // Replace each special character in the string
            foreach (char specialChar in specialChars)
            {
                input = input.Replace(specialChar, replaceWith);
            }

            return input;
        }

        protected void PrintBulkByDT(DataTable dtMain, string policy = null)
        {
             DataTable dt = dtMain;
            if (dt.Rows.Count > 0)
            {
                var pageNo = 1;
                string htmlContent = "<html><head><style>";

                htmlContent += @"
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }
            .page {
                page-break-before: always;
                padding: 20px;
                width: 210mm;
                height: 293mm;
                margin: auto;
/* border: 1px solid #ccc; */
                
            }
            
            .client-header{
                width: 80%;
                margin-top:50px;
                margin-left: 200px;
            }
            .client-header {
                text-align: left;
                margin-bottom: 20px;
            }

            .client-footer {
                text-align: left;
            }
            .client-content {
                
            }
            .client-content table {
                width: 100%;
                border-collapse: collapse;
                margin-top:70%;
            }
             table, th, td {
                border: 1px solid black;
            }
            th, td {
                padding: 3px;
                text-align: left;
            }
            .client-footer p {
                font-size: 0.9rem;
            }
            
            .foot-info {
            margin-top: 5PX;
            display: flex;
            justify-content: space-between;
        }

/* Print specific styles */
    @media print {
        /* Remove any page margins */
        body {
            margin: 0;
            padding: 0;
        }

        /* Ensure page margins are controlled */
        @page {
            size: A4;
            margin: 0;
        }

        /* Hide page numbers, date/time, and other browser-specific content */
        .header, .footer {
            display: none; /* Hide header and footer during printing */
        }

        /* Hide unwanted elements like page numbers, date and time that might be added by the browser */
        .page-number, .page-date {
            display: none;
        }

        /* Hide any browser-specific header/footer */
        .content {
            margin-top: 0;
        }

        /* Hide borders or other print styles that are unnecessary */
        table {
            margin-top: 0;
        }
    }
        </style></head><body>"; 
                    
                if(policy != null)
                {
                    dt = dt.Select($"POLICY_NO = '{policy}'").CopyToDataTable();
                }
                
                foreach (DataRow row in dt.Rows)
                    
                    {

                    
                    #region All print letter fields value
                    string arBranchCode   = row["ar_branch_cd"].ToString();
                    string companyCode    = row["company_cd"].ToString();
                    string clientName     = row["client_name"].ToString();
                    string address1BASE       = row["address1"].ToString();
                    string address1       = ReplaceSpecialCharacters(address1BASE, "\\", '-');
                    string address2       = row["address2"].ToString();
                    string clAdd3         = row["cl_add3"].ToString();
                    string clAdd4         = row["cl_add4"].ToString();
                    string clAdd5         = row["cl_add5"].ToString();
                    string phone1         = row["cl_phone1"].ToString();
                    string phone2         = row["cl_phone2"].ToString();
                    string cityName       = row["city_name"].ToString();
                    string dueDateBase    = row["due_date"].ToString();
                    //string remFlag        = row["rem_flage"].ToString();
                    string stateName      = row["State_Name"].ToString();
                    string companyName    = row["Company_Name"].ToString();
                    string favourName     = row["Favour_Name"].ToString();
                    string branchName     = row["Branch_Name"].ToString();
                    string branchAdd1     = row["Branch_Add1"].ToString();
                    string branchAdd2     = row["Branch_Add2"].ToString();
                    string planName1      = row["Plan_Name1"].ToString();
                    string payMode        = row["Pay_Mode"].ToString();
                    string policyNo       = row["Policy_No"].ToString();
                    //string pName          = row["P_Name"].ToString();
                    string iName          = row["I_Name"].ToString();
                    string premFreq       = row["prem_freq"].ToString();
                    //string bPremFreq      = row["bprem_freq"].ToString();
                    //string planName       = row["plan_name"].ToString();
                    string sa             = row["sa"].ToString();
                    string premAmt        = row["prem_amt"].ToString();
                    //string monNo          = row["mon_no"].ToString();
                    //string yearNo         = row["year_no"].ToString();
                    //string clPin          = row["cl_pin"].ToString();
                    //string pin1           = row["pin1"].ToString();
                    string invCode        = row["inv_code"].ToString();
                    //string invCode1       = row["inv_code1"].ToString();
                    //string importDataType = row["importdatatype"].ToString();
                    string dueDate        = GetDateFromDateTimeString(dueDateBase);

                    #endregion

                    
                    // Each block of content wrapped in a page div

                    #region All print letter fields value
                    //string arBranchCode = "N/A";
                    //string companyCode = "N/A";
                    //string clientName = "N/A";
                    //string address1 = "N/A";
                    //string address2 = "N/A";
                    //string clAdd3 = "N/A";
                    //string clAdd4 = "N/A";
                    //string clAdd5 = "N/A";
                    //string phone1 = "N/A";
                    //string phone2 = "N/A";
                    //tring cityName = "N/A";
                    //string dueDateBase = "01/01/1900";
                    string remFlag = "0";
                    //string stateName = "N/A";
                    //string companyName = "N/A";
                    //string favourName = "N/A";
                    //string branchName = "N/A";
                    //string branchAdd1 = "N/A";
                    //string branchAdd2 = "N/A";
                    //string planName1 = "N/A";
                    //string payMode = "N/A";
                    //string policyNo = "N/A";
                    string pName = "N/A";
                    //string iName = "N/A";
                    //string premFreq = "N/A";
                    string bPremFreq = "N/A";
                    string planName = "N/A";
                    //string sa = "0";
                    //string premAmt = "0";
                    string monNo = "0";
                    string yearNo = "0";
                    string clPin = "N/A";
                    string pin1 = "N/A";
                    //string invCode = "N/A";
                    string invCode1 = "N/A";
                    string importDataType = "N/A";
                    //string dueDate = GetDateFromDateTimeString(dueDateBase);
                    #endregion

                    htmlContent += $@"
                <div class=""page"">
                    <div class=""client-header"">
                        <p>{clientName}</p>
                        <p>{address1}</p>
                        <p>{address2} {clAdd3}</p>
                        <p>{clAdd4} {clAdd5}</p>
                        <p>{cityName} {stateName}</p>
                        <div class=""phones"">
                            <p><b>Tel No.:</b> {phone1} <b>Phone:</b> {phone2}</p>
                        </div>
                    </div>

                    <div class=""client-content"">
                        <table>
                            <tr><th colspan=""2"">Proposer Name</th><td colspan=""2"">{clientName}</td></tr>
                            <tr><th colspan=""2"">Life Insured Name</th><td colspan=""2"">{iName}</td></tr>
                            <tr><th colspan=""2"">Insurer</th><td colspan=""2"">{companyName}</td></tr>
                            <tr><th colspan=""2"">Plan Name</th><td colspan=""2"">{planName1}</td></tr>
                            <tr><th colspan=""2"">Policy Number</th><td colspan=""2"">{policyNo}</td></tr>
                            <tr><th>Payment Mode</th><td>{payMode}</td><th>Payment Frequency</th><td>{premFreq}</td></tr>
                            <tr><th>Basic Sum Assured</th><td>{sa}</td><th>Premium Due Date</th><td>{dueDate}</td></tr>
                            <tr><th>Premium Amount*</th><td>{premAmt}</td><th>Bajaj Capital Service Unit</th><td>{branchName}</td></tr>
                            <tr><th>Service Unit Address</th><td colspan=""3"">{branchAdd1} {branchAdd2}</td></tr>
                            <tr><th>Cheque in favor of</th><td colspan=""3"">{favourName}</td></tr>
                        </table>
                    </div>

                    <div class=""client-footer"">
                        <p style=""font-style: italic;"">* Please ensure to verify the exact premium amount with the Insurance Company on account of change in GST rates on Insurance Premium w.e.f. 1st Jul 2017</p>
                        <div class=""foot-info"">
                            <p><b>Ref No:</b> {invCode}</p>
                            <p>PAGE NO: {pageNo}</p>
                        </div>
                    </div>
                </div>";
                    pageNo += 1;
                }

                // Closing the HTML tags properly
                htmlContent += "</body></html>";

                // Encoding HTML and preparing print script
                // string encodedHtmlContent = Uri.EscapeDataString(htmlContent);
                string printScript = $@"
var blob = new Blob([`{htmlContent}`], {{ type: 'text/html' }});
var url = URL.createObjectURL(blob);
var printWindow = window.open(url, '', 'height=600,width=800');
printWindow.onload = function() {{
    printWindow.print();
    URL.revokeObjectURL(url);
}};

// Wait for the print window to load before triggering print
            printWindow.onafterprint = function() {{
                // Close the print window after printing is finished
                printWindow.close();
            }};
            printWindow.print();
";


                string printScript0 = $@"
            var printWindow = window.open('', '', 'height=600,width=800');
            printWindow.document.open();
            printWindow.document.write(decodeURIComponent('{htmlContent}'));
            printWindow.document.close();
            printWindow.focus();

// Wait for the print window to load before triggering print
            printWindow.onafterprint = function() {{
                // Close the print window after printing is finished
                printWindow.close();
            }};
            printWindow.print();



        ";

                // Register the script to run in the browser
                ScriptManager.RegisterStartupScript(this, this.GetType(), "PrintScript", printScript, true);
            }
            else
            {
                ShowAlert("No records found to print.");
            }


        }


        #endregion

        #region Mark Reminder Button
        protected void btnMark_Click(object sender, EventArgs e)
        {
            string companyCode = selectCompany.SelectedValue;
            string monthText = ddlMonths.SelectedValue;
            string yearText = ddlYears.SelectedValue;
            string policyNo = txtPolicyNumber.Text;

            // Validate inputs
            if (string.IsNullOrEmpty(companyCode))
            {
                ShowAlert("Compmany requried for mark");
                return;
            }

            if (string.IsNullOrEmpty(companyCode) && (string.IsNullOrEmpty(monthText) || string.IsNullOrEmpty(yearText)))
            {
                ShowAlert("Compmany or Month/Year requried for mark");
                return;
            }

            try
            {
                // Call the UpdateReminderFlag method from the controller
                InsuranceRenewalLetterPrintedController controller = new InsuranceRenewalLetterPrintedController();
                DataTable dt = controller.UpdateReminderFlag(
                    companyCode,
                    string.IsNullOrEmpty(monthText) ? (int?)null : Convert.ToInt32(monthText),
                    string.IsNullOrEmpty(yearText) ? (int?)null : Convert.ToInt32(yearText),
                    string.IsNullOrEmpty(policyNo) ? null : policyNo
                );

                if (dt != null && dt.Rows.Count > 0)
                {
                    string msg = GetTextFieldValue(dt.Rows[0], "Message");
                    ShowAlert(msg); 
                    lblMessage.Text = msg;
                    lblMessage.CssClass = "text-success";

                    if (companyGridView.Rows.Count > 0)
                    {
                        companyGridView.DataSource = null;
                        companyGridView.DataBind();
                    }
                }
                else
                {
                    string msg = "No records updated.";
                    ShowAlert(msg); 
                    lblMessage.Text = msg;
                    lblMessage.CssClass = "text-dark"; 
                }


                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                // Handle exceptions and display a friendly error message
                string errorMsg = "An error occurred while updating the reminder flag. Please try again later.";
                ShowAlert(errorMsg);
                lblMessage.Text = errorMsg;
            }
            finally
            {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "hideLabel", "hideMessage();", true);
            }
        }

        #endregion

        #region Hepler Methods: Generate Report to Print

        public void GenerateAndPrintPage(
        #region Item parameters
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
            // Create dynamic HTML content
            if (companyGridView.Rows.Count > 0)
            {
                var pageNo = 1;
                string htmlContent = "<html><head><style>";

                htmlContent += @"
            body {
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                box-sizing: border-box;
            }
            .page {
                page-break-before: always;
                padding: 20px;
                width: 210mm;
                height: 293mm;
                margin: auto;
/* border: 1px solid #ccc; */
                
            }
            
            .client-header{
                width: 80%;
                margin-top:50px;
                margin-left: 200px;
            }
            .client-header {
                text-align: left;
                margin-bottom: 20px;
            }

            .client-footer {
                text-align: left;
            }
            .client-content {
                
            }
            .client-content table {
                width: 100%;
                border-collapse: collapse;
                margin-top:70%;
            }
             table, th, td {
                border: 1px solid black;
            }
            th, td {
                padding: 3px;
                text-align: left;
            }
            .client-footer p {
                font-size: 0.9rem;
            }
            
            .foot-info {
            margin-top: 5PX;
            display: flex;
            justify-content: space-between;
        }

/* Print specific styles */
    @media print {
        /* Remove any page margins */
        body {
            margin: 0;
            padding: 0;
        }

        /* Ensure page margins are controlled */
        @page {
            size: A4;
            margin: 0;
        }

        /* Hide page numbers, date/time, and other browser-specific content */
        .header, .footer {
            display: none; /* Hide header and footer during printing */
        }

        /* Hide unwanted elements like page numbers, date and time that might be added by the browser */
        .page-number, .page-date {
            display: none;
        }

        /* Hide any browser-specific header/footer */
        .content {
            margin-top: 0;
        }

        /* Hide borders or other print styles that are unnecessary */
        table {
            margin-top: 0;
        }
    }
        </style></head><body>";

                htmlContent += $@"
                <div class=""page"">
                    <div class=""client-header"">
                        <p>{clientName}</p>
                        <p>{address1}</p>
                        <p>{address2} {clAdd3}</p>
                        <p>{clAdd4} {clAdd5}</p>
                        <p>{cityName} {stateName}</p>
                        <div class=""phones"">
                            <p><b>Tel No.:</b> {phone1} <b>Phone:</b> {phone2}</p>
                        </div>
                    </div>

                    <div class=""client-content"">
                        <table>
                            <tr><th colspan=""2"">Proposer Name</th><td colspan=""2"">{clientName}</td></tr>
                            <tr><th colspan=""2"">Life Insured Name</th><td colspan=""2"">{iName}</td></tr>
                            <tr><th colspan=""2"">Insurer</th><td colspan=""2"">{companyName}</td></tr>
                            <tr><th colspan=""2"">Plan Name</th><td colspan=""2"">{planName1}</td></tr>
                            <tr><th colspan=""2"">Policy Number</th><td colspan=""2"">{policyNo}</td></tr>
                            <tr><th>Payment Mode</th><td>{payMode}</td><th>Payment Frequency</th><td>{premFreq}</td></tr>
                            <tr><th>Basic Sum Assured</th><td>{sa}</td><th>Premium Due Date</th><td>{dueDate}</td></tr>
                            <tr><th>Premium Amount*</th><td>{premAmt}</td><th>Bajaj Capital Service Unit</th><td>{branchName}</td></tr>
                            <tr><th>Service Unit Address</th><td colspan=""3"">{branchAdd1} {branchAdd2}</td></tr>
                            <tr><th>Cheque in favor of</th><td colspan=""3"">{favourName} </td></tr>
                        </table>
                    </div>

                    <div class=""client-footer"">
                        <p style=""font-style: italic;"">* Please ensure to verify the exact premium amount with the Insurance Company on account of change in GST rates on Insurance Premium w.e.f. 1st Jul 2017</p>

                        <div class=""foot-info"">
                            <p><b>Ref No:</b> {invCode}</p>
                            <p>PAGE NO: {pageNo}</p>
                        </div>
                    </div>
                </div>";


                // Closing the HTML tags properly
                htmlContent += "</body></html>";

                // Encoding HTML and preparing print script
                string encodedHtmlContent = Uri.EscapeDataString(htmlContent);
                string printScript = $@"
            var printWindow = window.open('', '', 'height=600,width=800');
            printWindow.document.open();
            printWindow.document.write(decodeURIComponent('{encodedHtmlContent}'));
            printWindow.document.close();
            printWindow.focus();

            // Wait for the print window to load before triggering print
            printWindow.onafterprint = function() {{
                // Close the print window after printing is finished
                printWindow.close();
            }};
            printWindow.print();
            ";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "PrintScript", printScript, true);
            }

        }

        private void GenerateReport0(
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

        #endregion

        protected void btnReset_Click(object sender, EventArgs e)
        {

            ResetMainForm();
        }

        protected void ResetMainForm()
        {
            if (selectCompany.Items.Count > 0)
            {
            selectCompany.SelectedIndex = 0;

            }
            if (ddlMonths.Items.Count > 0)
            {
                ddlMonths.SelectedIndex = 0;

            }



            lblMessage.Text = "";

            if(txtPolicyNumber.Text != null)
            {
            txtPolicyNumber.Text = string.Empty;


            }

            chckRemFlag.Checked = false;

            // Reset GridView
            if(companyGridView.Rows.Count > 0)
            {
                companyGridView.DataSource = null;
                companyGridView.DataBind();
            }

           

        }

        protected void btnExit_Click(object sender, EventArgs e)
        {

            if (Session["LoginId"] != null)
            {
                Response.Redirect("~/welcome.aspx");
            }
            else
            {
                string baseUrl = ConfigurationManager.AppSettings["loginPage"];
                if (Session["LoginId"] == null)
                {
                    Response.Redirect($"{baseUrl}");
                    //Response.Redirect("~/index.aspx");
                }
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

    }
}