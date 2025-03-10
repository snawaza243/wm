using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Data.SqlClient;
using OfficeOpenXml;

namespace WM.Masters
{
    public partial class map_policy_number : Page
    {
        private PsmController psm_controller = new PsmController();
        private DataTable exportDT = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            //string baseUrl = ConfigurationManager.AppSettings["loginPage"];
            string baseUrl = "https://wealthmaker.in/login_new.aspx";
            string currentLogin = Session["LoginId"]?.ToString();

            if (currentLogin == null)
            {
                Response.Redirect($"{baseUrl}");
                //Response.Redirect("~/index.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    fillPolicyTypeList();
                    btnImport.Enabled = false;
                    btnExport.Enabled = false;
                    companySelect.Enabled = false;
                }
            }

        }


        protected void FileInput_Changed(object sender, EventArgs e)
        {
            
        }
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (!fileInput.HasFile)
            {
                ShowAlert("Kindly choose an excel file then press on upload!");
                fileInput.Focus();
                return;
            }

            if (fileInput.HasFile)
            {
                //string fileName = Guid.NewGuid().ToString() + "_" + fileInput.FileName; // Get Unique file name
                string fileName = "Map_Policy_" + fileInput.FileName;                     // Use the original file name
                string uploadPath = Server.MapPath("~/Uploads/");
                string filePath = Path.Combine(uploadPath, fileName);

                // Create uploads directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }


                try
                {
                    // Save the new uploaded file
                    fileInput.SaveAs(filePath);
                    Session["CurrentMapPolicyImportExcelFile"] = filePath;

                    // Load sheets into dropdown
                    LoadExcelSheets(filePath, ddlSheetList);

                    //  PopulateSheetsFromExcel(filePath);
                    lblFIleName.Text = "Uploaded File: " + fileInput.FileName;
                    lblMessage.CssClass = "text-dark";
                    string uploadedSuccessMsg = "File uploaded successfully";

                    ddlSheetList.Focus();
                    //ShowAlert(uploadedSuccessMsg);
                    lblMessage.Text = uploadedSuccessMsg;

                    companySelect.SelectedIndex = 0;
                    if (ddlSheetList.Items.Count > 0)
                    {
                        ddlSheetList.Enabled = true;
                        companySelect.Enabled = true;
                    }

                    if (GridView1.Rows.Count > 0 || gridPolicyData.Rows.Count > 0) {
                        GridView1.DataSource = null;
                        GridView1.DataBind();

                        gridPolicyData.DataSource = null;
                        gridPolicyData.DataBind();
                    }

                }
                catch (Exception ex)
                {
                    ShowAlert(ex.Message);
                    //lblMessage.Text = ex.Message;
                    lblMessage.CssClass = "text-dark";
                    ddlSheetList.Items.Clear();
                    ddlSheetList.Items.Add(new ListItem("Select Sheet", ""));
                    ddlSheetList.Enabled = false;
                    btnImport.Enabled = false;  
                    btnExport.Enabled = false;
                    companySelect.Enabled = false;
                    lblFIleName.Text = "File Name: <span class='text-danger'>*</span>";
                    if (companySelect.Items.Count >= 0)
                    {

                    companySelect.SelectedIndex = 0;
                    }


                    if (GridView1.Rows.Count > 0 || gridPolicyData.Rows.Count > 0)
                    {
                        GridView1.DataSource = null;
                        GridView1.DataBind();

                        gridPolicyData.DataSource = null;
                        gridPolicyData.DataBind();
                    }
                }
            }
        }

        // Load Excel sheets into dropdown
        public static void LoadExcelSheets(string filePath, DropDownList sheetDropDown)
        {

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
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

        private static string GetExcelConnectionString(string filePath)
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

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            string filePath = Session["CurrentMapPolicyImportExcelFile"]?.ToString();
            string selectedSheet = ddlSheetList.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(selectedSheet))
            {
                GridView1.DataSource = LoadSheetData(filePath, selectedSheet);
                GridView1.DataBind();
                lblMessage.Text = $"Selected sheet: " + selectedSheet;

            }

        }

        protected void ExcelSheetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filePath = Session["CurrentMapPolicyImportExcelFile"]?.ToString();
            string selectedSheet = ddlSheetList.SelectedValue.ToString();

            if (string.IsNullOrEmpty(selectedSheet))
            {
                btnImport.Enabled = false;
                btnExport.Enabled = false;
                gridPolicyData.DataSource = null;
                lblMessage.CssClass = "text-dark";

            }

            else if (!string.IsNullOrEmpty(selectedSheet))
            {

                btnImport.Enabled = true;
                lblMessage.CssClass = "text-dark";

            }



            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(selectedSheet))
            {
                try
                {

                    GridView1.DataSource = LoadSheetData(filePath, selectedSheet);
                    GridView1.DataBind();
                    string msg =  $"Selected sheet: " + selectedSheet + " have " + GridView1.Rows.Count + " row(s).";

                    if (GridView1.Rows.Count > 0){
                        lblMessage.Text = $"Selected sheet: " + selectedSheet + " have " + GridView1.Rows.Count + " row(s).";
                        btnImport.Focus();
                        companySelect.Enabled = true;
                    }
                    lblMessage.Text = $"Selected sheet: " + selectedSheet + " have " + GridView1.Rows.Count + " row(s).";

                }
                catch (Exception ex)
                {
                    ShowAlert("Error loading sheet data: " + ex.Message);

                    if (ex.Message.Contains("punctuation and that it is not too long"))
                    {
                        lblMessage.Text = ex.Message;
                        if (string.IsNullOrEmpty(filePath))
                        {
                            lblFIleName.Text = "Uploaded File: ";
                            ShowAlert("File doesn't exist, please upload a file.");
                        }
                        if (GridView1.Rows.Count > 0)
                        {

                            GridView1.DataSource = null;
                            GridView1.DataBind();
                        }

                        if (gridPolicyData.Rows.Count > 0)
                        {

                            gridPolicyData.DataSource = null;
                            gridPolicyData.DataBind();
                        }
                        btnImport.Enabled = false;
                        btnExport.Enabled = false;
                        companySelect.SelectedIndex = 0;
                        companySelect.Enabled = false;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    lblFIleName.Text = "Uploaded File: ";
                    ShowAlert("File doesn't exist, please upload a file.");
                }
                else if (string.IsNullOrEmpty(selectedSheet))
                {
                    if (GridView1.Rows.Count > 0)
                    {

                        GridView1.DataSource = null;
                        GridView1.DataBind();
                    }

                    if (gridPolicyData.Rows.Count > 0)
                    {

                        gridPolicyData.DataSource = null;
                        gridPolicyData.DataBind();
                    }
                    lblMessage.Text = $"Sheet not selected";
                }
            }
        }

        private void ShowAlert(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message.Replace("'", "\\'").Replace("\n", "\\n")}');", true);
        }

        protected void btnDownloadMapPolicySamplieFile_Click(object sender, EventArgs e)
        {
            string zipFilePath = Server.MapPath("~/SampleFile/IPOSampleUpdate.zip"); // Path to the ZIP file

            if (System.IO.File.Exists(zipFilePath))
            {
                Response.Clear();
                Response.ContentType = "application/zip"; // MIME type for ZIP files
                Response.AddHeader("Content-Disposition", $"attachment; filename={System.IO.Path.GetFileName(zipFilePath)}"); // Set the file name for download
                Response.TransmitFile(zipFilePath); // Efficiently sends the file to the response
                Response.End(); // Ends the response to ensure no further processing
            }
            else
            {
                lblMessage.Text = "The file 'IPOSmapleDataFormate.xlsx' does not exist in the SampleFile folder.";
                ShowAlert("The file does not exist. Please check the file path.");
            }
        }

        public static DataTable LoadSheetData(string filePath, string sheetName)
        {
            DataTable dtResult = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
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

            string connString = GetExcelConnectionString(filePath);
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

        #region fillPolicyTypeList
       

        private void fillPolicyTypeList()
        {
            try
            {

                // Get the data from the controller
                DataTable dt = new MapPolicyNumberController().GetCompanyList();

                // Combine the company name and company code in a new DataTable column
                DataTable dtWithCompanyAndCode = new DataTable();
                dtWithCompanyAndCode.Columns.Add("DisplayName", typeof(string));
                dtWithCompanyAndCode.Columns.Add("COMPANY_CD", typeof(string));

                // Populate the new DataTable with combined values
                foreach (DataRow row in dt.Rows)
                {
                    string displayName =  row["COMPANY_NAME"].ToString() + " - " + row["COMPANY_CD"].ToString();
                    dtWithCompanyAndCode.Rows.Add(displayName, row["COMPANY_CD"]);
                }

                // Bind the new DataTable to the dropdown list
                companySelect.DataSource = dtWithCompanyAndCode;
                companySelect.DataTextField = "DisplayName"; // Display the combined value
                companySelect.DataValueField = "COMPANY_CD"; // Value will still be the company code
                companySelect.DataBind();

                // Insert a default "Select Company" option
                companySelect.Items.Insert(0, new ListItem("Select Company", ""));
            }catch(Exception ex)
            {
                ShowAlert("Error in compnaies list data.");
            }
        }

        #endregion


        protected void btnImport_Click(object sender, EventArgs e)
        {
            string filePath = Server.MapPath("~/Uploads/") + Path.GetFileName(fileInput.FileName);
            fileInput.SaveAs(filePath);

            string excelConnString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=Yes;'";


            DataTable dtExcel = new DataTable();
            using (OleDbConnection excelConn = new OleDbConnection(excelConnString))
            {
                excelConn.Open();

                // **Get the First Sheet Name**
                DataTable dtSheets = excelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dtSheets != null && dtSheets.Rows.Count > 0)
                {
                    string firstSheetName = dtSheets.Rows[0]["TABLE_NAME"].ToString(); // Automatically picks the first sheet

                    // **Read data from the first sheet**
                    using (OleDbCommand cmd = new OleDbCommand($"SELECT * FROM [{firstSheetName}]", excelConn))
                    {
                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(dtExcel);
                    }
                }
                else
                {
                    throw new Exception("No sheets found in the uploaded Excel file.");
                }
            }

            using (OracleConnection oracleConn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                oracleConn.Open();

                // Clear previous data
                using (OracleCommand deleteCmd = new OracleCommand("DELETE FROM POLICY_MAP_TEMP1", oracleConn))
                {
                    deleteCmd.ExecuteNonQuery();
                }

                // Insert new data
                using (OracleCommand insertCmd = new OracleCommand("INSERT INTO POLICY_MAP_TEMP1 (POLICY_NO, COMPANY_CD) VALUES (:PolicyNo, :CompanyCd)", oracleConn))
                {
                    insertCmd.Parameters.Add(":PolicyNo", OracleDbType.Varchar2);
                    insertCmd.Parameters.Add(":CompanyCd", OracleDbType.Varchar2);

                    foreach (DataRow row in dtExcel.Rows)
                    {
                        if (!string.IsNullOrEmpty(row[0].ToString()))
                        {
                            string policyNo = row[0].ToString().Replace("-", "").Trim();
                            string companyCd = row[1].ToString().Trim();

                            // Assign values to parameters
                            insertCmd.Parameters[":PolicyNo"].Value = policyNo;
                            insertCmd.Parameters[":CompanyCd"].Value = companyCd;

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            // Generate and download report
            DataTable dtReport = GetPolicyData();
            ExportInexcelByEPP(dtReport);
            //ExportXML(dtReport);



        }
        private void ExportXML(DataTable dtReport)
        {
            if (dtReport.Rows.Count > 0)
            {
                // ✅ Set a name for the DataTable (important for XML serialization)
                dtReport.TableName = "PolicyData";

                string reportPath = Server.MapPath("~/Reports/policymap.xml");

                // ✅ Write DataTable to XML
                dtReport.WriteXml(reportPath);

                // ✅ Redirect to XML file to view/download
                Response.Redirect("~/Reports/policymap.xml");
            }
            else
            {
                // Handle case when no data is returned
                Response.Write("<script>alert('No data found.');</script>");
                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No records found.');", true);

            }
        }

        private void ExportInexcelByEPP(DataTable dtReport)
        {
            if (dtReport.Rows.Count > 0)
            {
                string fileName = "policymap.xlsx";
                string filePath = Server.MapPath("~/Reports/") + fileName;

                using (ExcelPackage excel = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("PolicyData");
                    worksheet.Cells["A1"].LoadFromDataTable(dtReport, true); // ✅ Load DataTable into Excel

                    FileInfo excelFile = new FileInfo(filePath);
                    excel.SaveAs(excelFile); // ✅ Save to file
                }

                // ✅ Prompt user to download the file
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AppendHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.TransmitFile(filePath);
                Response.End();
            }
            else
            {
                Response.Write("<script>alert('No data found.');</script>");
            }
        }

        private DataTable GetPolicyData()
        {
            DataTable dt = new DataTable();
            using (OracleConnection oracleConn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))

            {
                oracleConn.Open();
                string query = @"
                SELECT DISTINCT P.POLICY_NO, MAX(A.PREM_AMT) AS MAX_AMT, 
                    CASE MAX(A.PREM_FREQ) WHEN 1 THEN 'Y' WHEN 2 THEN 'HY' WHEN 4 THEN 'Q' WHEN 12 THEN 'M' END AS PREM_FREQ, 
                    MAX(A.NEXT_DUE_DT) AS NEXT_DUE_DT, MAX(A.COMPANY_CD) AS COMPANY_CD, 
                    R.REGION_NAME, Z.ZONE_NAME, E.RM_NAME, B.BRANCH_NAME, I.INVESTOR_NAME, 
                    I.ADDRESS1, I.ADDRESS2, C.CITY_NAME, S.STATE_NAME, I.MOBILE, I.PHONE 
                FROM POLICY_MAP_TEMP1 P
                JOIN BAJAJ_AR_HEAD A ON P.POLICY_NO = A.POLICY_NO
                JOIN INVESTOR_MASTER I ON A.CLIENT_CD = I.INV_CODE
                JOIN EMPLOYEE_MASTER E ON E.RM_CODE = I.RM_CODE
                JOIN CITY_MASTER C ON I.CITY_ID = C.CITY_ID
                JOIN STATE_MASTER S ON C.STATE_ID = S.STATE_ID
                JOIN BRANCH_MASTER B ON I.BRANCH_CODE = B.BRANCH_CODE
                JOIN REGION_MASTER R ON B.REGION_ID = R.REGION_ID
                JOIN ZONE_MASTER Z ON B.ZONE_ID = Z.ZONE_ID
                GROUP BY P.POLICY_NO, B.BRANCH_NAME, R.REGION_NAME, Z.ZONE_NAME, I.INVESTOR_NAME, 
                    I.ADDRESS1, I.ADDRESS2, I.MOBILE, I.PHONE, E.RM_NAME, C.CITY_NAME, S.STATE_NAME";

                using (OracleDataAdapter da = new OracleDataAdapter(query, oracleConn))
                {
                    da.Fill(dt);
                }
                 
            }
            return dt;
        }
        protected void btnImport_Click_0(object sender, EventArgs e)
        {
            try
            {
                if (Session["CurrentMapPolicyImportExcelFile"] == null || lblFIleName.Text.Contains("File Name"))
                {
                    ShowAlert("File does not exist kindly upload.");
                    fileInput.Focus();
                    lblMessage.Text = "Please select a file to upload.";
                    lblMessage.Text = string.Empty;
                    gridPolicyData.DataSource = null;
                    gridPolicyData.DataBind();
                    return;
                }
                if (ddlSheetList.Items.Count > 0 && string.IsNullOrEmpty(ddlSheetList.SelectedValue.ToString()))
                {
                    string alertMsg = "Choose a sheet";
                    ShowAlert(alertMsg);
                    fileInput.Focus();
                    lblMessage.Text = alertMsg;
                    lblMessage.Text = string.Empty;
                    ddlSheetList.Focus();
                    return;
                }
                else
                {
                    //ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", "showClientLoader();", true);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", "showServerLoader();", true);
                    try
                    {
                        string filePath = Session["CurrentMapPolicyImportExcelFile"]?.ToString();
                        string selectedSheet = ddlSheetList.SelectedValue;
                        string currrentCompVlaue = companySelect.SelectedValue.ToString();
                        DataTable rawData = ReadExcelData(filePath, selectedSheet);

                        var controller = new WM.Controllers.MapPolicyNumberController();
                        string isImported = controller.ImportExcelToDatabase(rawData, selectedSheet);
                        if (isImported.Contains("successfully"))
                        {
                            btnExport.Enabled = true;
                            //ShowAlert(isImported);
                            lblMessage.Text = isImported;
                            /*
                            string selectedCompanyString = companySelect.SelectedValue.ToString() == "Select Company" ? "all companies" : companySelect.SelectedItem.Text;
                            string lastString = string.IsNullOrEmpty(currrentCompVlaue) ? "." : " for <b class=\"text-success\">" + selectedCompanyString + "</b> company.";

                            DataTable exportDataFromTable = controller.GetPolicyDataForExport(currrentCompVlaue);
                            int rowCount = exportDataFromTable.Rows.Count;
                            if (rowCount > 0)
                            {
                                Session["MapPolicyExportData"] = exportDataFromTable;

                                string msg = isImported + " and exporting data fetched" + lastString;
                                btnExport.Enabled = true;
                                ShowAlert(msg);
                            }
                            else
                            {
                                ShowAlert("No data available for export.");
                            }
                            */


                        }
                        else
                        {
                            ShowAlert(isImported);
                            //lblMessage.Text = isImported;
                            gridPolicyData.DataSource = null;
                            gridPolicyData.DataBind();
                        }

                    }
                    finally
                    {
                        //ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", "hideClientLoader();", true);
                        ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", "hideClientFilterData(); ShowAlert('File Exported Successfully!');", true);

                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show error message
                lblMessage.Text = "An error occurred: " + ex.Message;
            }

        }



        private void BindGrid(DataTable dt)
        {
            
            gridPolicyData.DataSource = dt;
            gridPolicyData.DataBind();
        }

        // Handling Page Change Event
        protected void gridPolicyData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            string currrentCompVlaue = companySelect.SelectedValue.ToString();
            var controller = new WM.Controllers.MapPolicyNumberController();
            DataTable exportDataFromTable = controller.GetPolicyDataForExport(currrentCompVlaue);

            gridPolicyData.PageIndex = e.NewPageIndex;
            if(exportDataFromTable.Rows.Count > 0)
            {
                BindGrid(exportDataFromTable);
            }
            else
            {
                gridPolicyData.DataSource = null;
                gridPolicyData.DataBind();
            }
        }


        protected void btnExport_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", "showServerLoader();", true);
            try
            {
                var controller = new WM.Controllers.MapPolicyNumberController();
                string currentCompValue = companySelect.SelectedValue.ToString();
                DataTable exportDataFromTable = controller.GetPolicyDataForExport(currentCompValue);
                int rowCount = exportDataFromTable.Rows.Count;
                if (rowCount > 0)
                {
                    lblMessage.Text = rowCount + " row(s) data exported.";
                    
                    bool isExportedExcel = ExportGridToExcelDynamic(exportDataFromTable);
                    if (isExportedExcel)
                    {
                        ShowAlert("File Exported Successfully!" + exportDataFromTable.Rows.Count.ToString());
                    }
                    else
                    {
                        ShowAlert("Not Exported!");
                    }
                }
                else
                {
                    ShowAlert("No data available for export.");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message);
            }
            finally
            {
                //ScriptManager.RegisterStartupScript(this, GetType(), "ExportException", $"hideServerLoader(); ShowAlert('Data Exported Successfully!');", true);

            }
        }

        protected void btnExport_Click_0(object sender, EventArgs e)
        {
            var controller = new WM.Controllers.MapPolicyNumberController();
            string currrentCompValue = companySelect.SelectedValue.ToString();

            if (gridPolicyData.Rows.Count > 0)
            {
                string rowCount = gridPolicyData.Rows.Count.ToString();
                lblMessage.Text = rowCount + " row(s) data exported.";

                if (Convert.ToInt32(rowCount) > 0)
                {
                    try
                    {
                        // Show the server-side loader BEFORE starting export
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowLoader", "showServerLoader();", true);

                        bool isExportedExcel = true;// ExportGridToExcelDynamic(gridPolicyData);

                        if (isExportedExcel)
                        {
                            // Hide server loader after successful export
                            ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", "hideServerLoader(); ShowAlert('File Exported Successfully!');", true);
                        }
                        else
                        {
                            // Hide loader and show error message if export failed
                            ScriptManager.RegisterStartupScript(this, GetType(), "ExportError", "hideServerLoader(); ShowAlert('Error Exporting File');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ensure loader is hidden and show error message
                        ScriptManager.RegisterStartupScript(this, GetType(), "ExportException", $"hideServerLoader(); ShowAlert('Error: {ex.Message}');", true);
                    }
                }
            }
        }

        public bool ExportGridToExcelDynamic(DataTable dtPolicyData)
        {
            if (dtPolicyData != null && dtPolicyData.Rows.Count > 0)
            {
                try
                {
                    // Create a new workbook
                    using (var workbook = new XLWorkbook())
                    {
                        // Add a worksheet to the workbook
                        var worksheet = workbook.AddWorksheet("Policy Data");

                        // Add the header row (from DataTable columns) with borders
                        for (int col = 0; col < dtPolicyData.Columns.Count; col++)
                        {
                            var headerCell = worksheet.Cell(1, col + 1);
                            headerCell.Value = dtPolicyData.Columns[col].ColumnName;
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            headerCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Add the data rows (from DataTable rows) with borders
                        for (int row = 0; row < dtPolicyData.Rows.Count; row++)
                        {
                            for (int col = 0; col < dtPolicyData.Columns.Count; col++)
                            {
                                object cellValue = dtPolicyData.Rows[row][col];

                                // Replace NULL values with a space
                                string cellText = cellValue != DBNull.Value ? cellValue.ToString() : " ";

                                // Remove HTML encoded spaces if present
                                if (!string.IsNullOrEmpty(cellText) && cellText.Contains("&nbsp;"))
                                {
                                    cellText = cellText.Replace("&nbsp;", " ");
                                }

                                // Insert value into the worksheet
                                var cell = worksheet.Cell(row + 2, col + 1);
                                cell.Value = string.IsNullOrEmpty(cellText) ? " " : cellText;
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            }
                        }

                        // Prepare response for Excel download
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=PolicyData.xlsx");

                        // Write the Excel file to the Response stream
                        //using (var ms = new MemoryStream())
                        //{
                        //    workbook.SaveAs(ms);
                        //    ms.WriteTo(HttpContext.Current.Response.OutputStream);
                        //    ms.Close();
                        //}

                        using (var memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                            HttpContext.Current.Response.Buffer = false;
                            HttpContext.Current.Response.Flush();

                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.End(); // Important to stop further processing
                        }
                        // Notify user
                        ShowAlert("File Downloaded!");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                    return false;
                }
            }
            else
            {
                HttpContext.Current.Response.Write("No data to export.");
                return false;
            }
        }

        public bool ExportGridToExcel(System.Web.UI.WebControls.GridView gridPolicyData)
        {
            if (gridPolicyData.Rows.Count > 0)
            {
                try
                {
                    // Create a new workbook
                    using (var workbook = new XLWorkbook())
                    {
                        // Add a worksheet to the workbook
                        var worksheet = workbook.AddWorksheet("Policy Data");

                        // Add the header row (from GridView columns) with borders
                        for (int col = 0; col < gridPolicyData.Columns.Count; col++)
                        {
                            var headerCell = worksheet.Cell(1, col + 1);
                            headerCell.Value = gridPolicyData.Columns[col].HeaderText;
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Apply border
                            headerCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;  // Apply inside border
                        }

                        // Add the data rows (from GridView rows) with borders
                        for (int row = 0; row < gridPolicyData.Rows.Count; row++)
                        {
                            for (int col = 0; col < gridPolicyData.Columns.Count; col++)
                            {
                                // Get the value from the grid cell
                                string cellValue = gridPolicyData.Rows[row].Cells[col].Text;

                                // Check if the cell value contains "&nbsp;" and replace it with a regular space
                                if (!string.IsNullOrEmpty(cellValue) && cellValue.Contains("&nbsp;"))
                                {
                                    cellValue = cellValue.Replace("&nbsp;", " ");
                                }

                                // If the cell value is empty or after replacement is empty, set it to a space
                                if (string.IsNullOrEmpty(cellValue))
                                {
                                    worksheet.Cell(row + 2, col + 1).Value = " ";
                                }
                                else
                                {
                                    worksheet.Cell(row + 2, col + 1).Value = cellValue;
                                }

                                // Apply borders to each cell
                                var cell = worksheet.Cell(row + 2, col + 1);
                                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Apply border
                                cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;  // Apply inside border
                            }
                        }


                            //ScriptManager.RegisterStartupScript(this, GetType(), "HideLoader", "hideLoader();", true);
                        // Set the content type to be Excel file
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=PolicyData.xlsx");

                        // Write the Excel file to the Response stream
                        using (var ms = new MemoryStream())
                        {
                            workbook.SaveAs(ms);
                            ms.WriteTo(HttpContext.Current.Response.OutputStream);
                            ms.Close();
                        }

                        // End the response to trigger download
                        // HttpContext.Current.Response.End(); <-- Remove this line
                    }

                    ShowAlert("File Downloaded!");
                    return true; // Export successful
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    lblMessage.Text = "Error: " + ex.Message;
                    return false; // Return false if there's an error
                }
            }
            else
            {
                // Handle the case when there is no data to export
                HttpContext.Current.Response.Write("No data to export.");
                return false; // Return false if no data to export
            }
        }

        public void ExportGridToExcel0(System.Web.UI.WebControls.GridView gridPolicyData)
        {

            if (gridPolicyData.Rows.Count > 0)
            {
                // Create a new workbook
                using (var workbook = new XLWorkbook())
                {
                    // Add a worksheet to the workbook
                    var worksheet = workbook.AddWorksheet("Policy Data");
                    // Add the header row (from GridView columns) with borders
                    for (int col = 0; col < gridPolicyData.Columns.Count; col++)
                    {
                        var headerCell = worksheet.Cell(1, col + 1);
                        headerCell.Value = gridPolicyData.Columns[col].HeaderText;
                        headerCell.Style.Font.Bold = true;
                        headerCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Apply border
                        headerCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;  // Apply inside border
                    }

                    // Add the data rows (from GridView rows) with borders
                    for (int row = 0; row < gridPolicyData.Rows.Count; row++)
                    {
                        for (int col = 0; col < gridPolicyData.Columns.Count; col++)
                        {
                            // Get the value from the grid cell
                            string cellValue = gridPolicyData.Rows[row].Cells[col].Text;

                            // Check if the cell value contains "&nbsp;" and replace it with a regular space
                            if (!string.IsNullOrEmpty(cellValue) && cellValue.Contains("&nbsp;"))
                            {
                                // Replace all instances of "&nbsp;" with a space
                                cellValue = cellValue.Replace("&nbsp;", " ");
                            }

                            // If the cell value is empty or after replacement is empty, set it to a space
                            if (string.IsNullOrEmpty(cellValue))
                            {
                                worksheet.Cell(row + 2, col + 1).Value = " ";
                            }
                            else
                            {
                                // Otherwise, set the actual (or modified) value
                                worksheet.Cell(row + 2, col + 1).Value = cellValue;
                            }

                            // Apply borders to each cell
                            var cell = worksheet.Cell(row + 2, col + 1);
                            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin; // Apply border
                            cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;  // Apply inside border
                        }
                    }

                    // Set the content type to be Excel file
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=PolicyData.xlsx");

                    lblMessage.Text = "export new ddd2";

                    // Write the Excel file to the Response stream
                    using (var ms = new MemoryStream())
                    {
                        workbook.SaveAs(ms);
                        ms.WriteTo(HttpContext.Current.Response.OutputStream);
                        ms.Close();
                    }

                    lblMessage.Text = "export new ddd";
                    // End the response to trigger download
                    HttpContext.Current.Response.End();

                }

                ShowAlert("File Downloaded!");
            }
            else
            {
                // Handle the case when there is no data to export
                HttpContext.Current.Response.Write("No data to export.");
            }
        }
       
        
        #region Important function for the policy report generate grid
        protected void gvPolicyData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
        }

        #endregion

        #region ExitBtn
        protected void btnExit_Click(object sender, EventArgs e)
        {
            //string baseUrl = ConfigurationManager.AppSettings["loginPage"];
            string baseUrl = "https://wealthmaker.in/login_new.aspx";
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

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        protected void ResetAll()
        {
            // Clear the FileUpload control
            fileInput.Attributes.Clear();

            // Reset the DropDownList selection
            ddlSheetList.SelectedIndex = 0;

            companySelect.SelectedIndex = 0;
            // Clear the label text
            lblMessage.Text = string.Empty;
            lblFIleName.Text = "File Name: ";
            // Optionally, you can also clear the GridView
            gridPolicyData.DataSource = null;
            gridPolicyData.DataBind();

            GridView1.DataSource = null;
            GridView1.DataBind();
        }

    }

}
