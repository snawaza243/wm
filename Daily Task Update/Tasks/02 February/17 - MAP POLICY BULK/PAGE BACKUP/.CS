using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Web;

namespace WM.Masters
{
    public partial class map_policy_number : Page
    {
        protected void Page_Load(object sender, EventArgs e)
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
                if (!IsPostBack)
                {
                    // Fill company
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
                    ShowAlert(uploadedSuccessMsg);
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
                    lblMessage.Text = ex.Message;
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
                    lblMessage.Text = $"Selected sheet: " + selectedSheet;
                    if(GridView1.Rows.Count > 0){
                        companySelect.Enabled = true;
                    }
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
                #region Simple listing
               /*
                DataTable dt = new MapPolicyNumberController().GetCompanyList();
                companySelect.DataSource = dt;
                companySelect.DataTextField = "COMPANY_NAME";
                companySelect.DataValueField = "COMPANY_CD";
                companySelect.DataBind();
                companySelect.Items.Insert(0, new ListItem("Select Company", ""));
               */
                #endregion

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
                if (ddlSheetList.Items.Count>0 && string.IsNullOrEmpty(ddlSheetList.SelectedValue.ToString()))
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
                    string filePath = Session["CurrentMapPolicyImportExcelFile"]?.ToString();
                    string selectedSheet = ddlSheetList.SelectedValue;
                    string currrentCompVlaue = companySelect.SelectedValue.ToString();
                    DataTable rawData = ReadExcelData(filePath, selectedSheet);

                    var controller = new WM.Controllers.MapPolicyNumberController();

                    string isImported = controller.ImportExcelToDatabase(rawData, selectedSheet);
                    DataTable exportDataFromTable = controller.GetPolicyDataForExport(currrentCompVlaue);
                    int importDataCount = exportDataFromTable.Rows.Count;
                    


                    // Display the result message

                    // Check for successful import and bind data to the grid
                    if (isImported.Contains("successfully"))
                    {

                        // Correct the logic for selected company string
                        string selectedCompanyString = companySelect.SelectedValue.ToString() == "Select Company" ? "all companies" : companySelect.SelectedItem.Text;

                        // Fix the logic for lastString
                        string lastString = string.IsNullOrEmpty(currrentCompVlaue) ? "." : " for <b class=\"text-success\">" + selectedCompanyString + "</b> company.";

                        if (importDataCount > 0)
                        {
                            lblMessage.Text = isImported + " and exporting data fetched" + lastString;
                            lblMessage.CssClass = "text-success";

                            btnExport.Focus();
                            // Bind data to the grid
                            gridPolicyData.DataSource = exportDataFromTable;
                            gridPolicyData.DataBind();

                            // Enable the export button
                            btnExport.Enabled = true;
                        }

                        else
                        {
                            gridPolicyData.DataSource = null;
                            gridPolicyData.DataBind();
                            btnExport.Enabled = false;
                            lblMessage.Text = isImported + " but not fetched data for <b>" + selectedCompanyString + "</b> company to export.";
                            lblMessage.CssClass = "text-danger";


                        }

                    }
                    else
                    {
                        lblMessage.Text = isImported;
                        gridPolicyData.DataSource = null;
                        gridPolicyData.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {
                // Log the exception and show error message
                lblMessage.Text = "An error occurred: " + ex.Message;
            }
        }
   
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var controller = new WM.Controllers.MapPolicyNumberController();
            string currrentCompVlaue = companySelect.SelectedValue.ToString();
            DataTable exportDataFromTable = controller.GetPolicyDataForExport(currrentCompVlaue);
            if(exportDataFromTable.Rows.Count > 0)
            {
                string rowCount = exportDataFromTable.Rows.Count.ToString();
                lblMessage.Text = rowCount + " rows(s) data exported.";

                if (Convert.ToInt32(rowCount) > 0)
                {
                    try
                    {

                        ExportGridToExcel(gridPolicyData);
                    }
                    catch (Exception ex)
                    {
                        ShowAlert(ex.Message);
                    }

                }


            }
       
        
        
        }
        public void ExportGridToExcel(System.Web.UI.WebControls.GridView gridPolicyData)
        {
            try
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

                        // Write the Excel file to the Response stream
                        using (var ms = new MemoryStream())
                        {
                            workbook.SaveAs(ms);
                            ms.WriteTo(HttpContext.Current.Response.OutputStream);
                            ms.Close();
                        }

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
            catch (Exception ex)
            {
                ShowAlert(ex.Message);
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
