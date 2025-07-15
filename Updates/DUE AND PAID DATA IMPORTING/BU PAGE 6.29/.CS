using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using Microsoft.Ajax.Utilities;
using System.Text;
using WM.Controllers;


namespace WM.Masters
{
    public partial class DueAndPaidDataImporting : Page
    {
        string uniqueFileName = "";
        PsmController psmController = new PsmController();
       
        protected void Page_Load(object sender, EventArgs e)
        {

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
                    excelSheetSelect.Enabled = false;
                    FillYearMonths();
                    FillDbFieldListDropDown();
                }
                
            }
        }

        protected void btnReloadPage_Click(object sender, EventArgs e)
        {
            // Reloads the current page
            Response.Redirect(Request.RawUrl);
        }



        protected void UploadButton_Click(object sender, EventArgs e)
        {

            if (fileInput.HasFile)
            {
                //string fileName = Guid.NewGuid().ToString() + "_" + fileInput.FileName; // Get Unique file name
                string fileName = "DueAndPaid_" + fileInput.FileName;                     // Use the original file name
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
                    fileInput.SaveAs(filePath);
                    Session["CurrentDAPExcelFile"] = filePath;
                    LoadExcelSheets(filePath, excelSheetSelect);
                    excelSheetSelect.Enabled = true;

                    lblFIleName.Text = "Uploaded File: " + fileInput.FileName;
                    ShowAlert("File Uploaded");
                    excelSheetSelect.Focus();
                }
                catch (Exception ex)
                {
                    ShowAlert(ex.Message);
                }
            }
        }

        protected void ExportImportedPolicyButton_Click(object sender, EventArgs e)
        {

            if (GridView1 == null || GridView1.Rows.Count == 0)
            {
                ShowMessage("No data available to export.");
                return;
            }


            try
            {
                // Disable paging to export all rows
                GridView1.AllowPaging = false;

                // Rebind GridView with data (if needed)
                // Example: GridView1.DataSource = GetDataSource(); GridView1.DataBind();

                // Set file name and content type
                string fileName = "ImportedPolicy.xls";
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";

                // Use StringWriter and HtmlTextWriter to render the GridView to Excel format
                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        // Add a style to format numbers properly in Excel
                        GridView1.HeaderRow.Style.Add("background-color", "#FFFFFF");
                        foreach (TableCell cell in GridView1.HeaderRow.Cells)
                        {
                            cell.Style.Add("background-color", "#D5D5D5");
                        }

                        foreach (GridViewRow row in GridView1.Rows)
                        {
                            row.BackColor = System.Drawing.Color.White;
                            foreach (TableCell cell in row.Cells)
                            {
                                cell.Attributes.Add("class", "text");
                            }
                        }

                        GridView1.RenderControl(hw);

                        // Write the rendered content to the response stream
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex); // Log the error for troubleshooting
                ShowMessage("An error occurred while exporting the data. Please try again.");
            }
            finally
            {
                try
                {
                    Response.End(); // Ensure the response is ended properly
                }
                catch (Exception endEx)
                {
                    LogError(endEx); // Log response ending issues
                }
            }
        }

        // Override to allow GridView rendering for export
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required to confirm that an HtmlForm control is rendered for the GridView
        }

        // Helper to log errors
        private void LogError(Exception ex)
        {
            // Log error (to file, database, etc.)
            System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
        }

        // Helper to display user messages
        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('{message.Replace("'", "\\'")}');", true);
        }


        protected void ExcelSheetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filePath = Session["CurrentDAPExcelFile"]?.ToString();
            string selectedSheet = excelSheetSelect.SelectedValue;

            lblsheetHeaderListHead.Text = selectedSheet + " Excel's fields :";

            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(selectedSheet))
            {
                try
                {
                    // Load sheet columns into dropdown
                    LoadSheetColumns(filePath, selectedSheet, headerDropDown);

                    monthSelect.Focus();

                    // Load sheet data into grid
                    GridView1.DataSource = LoadSheetData(filePath, selectedSheet);
                    GridView1.DataBind();


                    // Collect the column headers from the dropdown (assuming headerDropDown has the column names)
                    StringBuilder headerNames = new StringBuilder();
                    foreach (ListItem item in headerDropDown.Items)
                    {
                        if (item.Value != "")  // Avoid empty values if any
                        {
                            headerNames.Append(item.Text + "<br/>");  // Add <br/> for line breaks
                        }
                    }

                    // Set the label text to include the header labels with line breaks
                    lblsheetHeaderListBody.Text = " <br/>" + headerNames.ToString();

                    int gvRowCount = GridView1.Rows.Count;
                    int sheetColCount = headerDropDown.Items.Count - 1;

                    // Update header count
                    txtSheetHeaderCount.Text = gvRowCount.ToString() + " rows & " + sheetColCount + " columns";


                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "error", $"alert('Error loading sheet data: {ex.Message}')", true);
                }
            }
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

        // Load Excel sheets into dropdown
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

        // Load sheet data into grid
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



        public List<string> GetDatabaseFieldList(List<string> dropdownValues)
        {
            List<string> databaseFields = new List<string>();

            foreach (string value in dropdownValues)
            {
                // Split and take the second part as the database field
                string[] parts = value.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    databaseFields.Add(parts[1].Trim());
                }
            }

            return databaseFields;
        }

        public List<string> GetExcelFieldList(List<string> dropdownValues)
        {
            List<string> excelFields = new List<string>();

            foreach (string value in dropdownValues)
            {
                // Split and take the first part as the Excel field
                string[] parts = value.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    excelFields.Add(parts[0].Trim());
                }
            }

            return excelFields;
        }


        protected string GetCheckedRadioButtonText()
        {
            // Iterate through the controls in the container (e.g., Page or a specific Panel)
            foreach (Control control in Page.Controls)
            {
                string text = GetCheckedRadioButtonTextRecursive(control);
                if (!string.IsNullOrEmpty(text))
                {
                    return text; // Return the text of the checked RadioButton
                }
            }
            return string.Empty; // No RadioButton is checked
        }

        private string GetCheckedRadioButtonTextRecursive(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is RadioButton radioButton && radioButton.Checked)
                {
                    return radioButton.Text; // Return the text of the checked RadioButton
                }

                // Recursively check child controls
                if (control.HasControls())
                {
                    string text = GetCheckedRadioButtonTextRecursive(control);
                    if (!string.IsNullOrEmpty(text))
                    {
                        return text;
                    }
                }
            }
            return string.Empty;
        }

      


        public void ImportDatainDB()
        {
            string monthValue = monthSelect.SelectedValue ?? string.Empty;
            string yearValue = yearSelect.SelectedValue ?? string.Empty;
            string ddlImportDataTypeValue = ddlImportDataType.SelectedValue ?? string.Empty;
            string logginId = Session["LoginId"].ToString() ?? string.Empty;
            string filePath = Session["CurrentDAPExcelFile"]?.ToString();
            string selectedSheet = excelSheetSelect.SelectedValue;

            string selectedText = GetCheckedRadioButtonText();
            string chkDataTypeValue = selectedText.Substring(0, 1).ToUpper();

            string fileName = lblFIleName.Text;

            // Step 1: Collect database fields and dropdown values
            List<string> allDBFieldList = new List<string>();
            foreach (ListItem item in ddlDbField.Items)
            {
                allDBFieldList.Add(item.Text); // Collect all dropdown field names
            }

            List<string> dropdownValues = new List<string>();
            foreach (ListItem item in ddlMappedFieldList.Items)
            {
                dropdownValues.Add(item.Text); // Collect mapped fields (Excel Field >> DB Field)
            }

            // Step 2: Destructure dropdown values to get Excel fields and database fields
            List<string> excelFields = GetExcelFieldList(dropdownValues); // Extract Excel field names
            List<string> databaseFields = GetDatabaseFieldList(dropdownValues); // Extract database field names

            // Step 3: Import external Excel data

            if (ddlMappedFieldList.Items.Count == 0)
            {
                ShowAlert("Mapping is required");
                return;
            }
            DataTable excelData = LoadSheetData(filePath, selectedSheet); // Load data from the selected Excel sheet

            // Step 4: Create a new DataTable with fields matching ddlDbField
            DataTable mappedDataTable = new DataTable();
            foreach (string dbField in allDBFieldList)
            {
                mappedDataTable.Columns.Add(dbField, typeof(string)); // Add columns for all database fields
            }

            // Step 5: Map data from Excel to the new DataTable based on the mapping
            foreach (DataRow excelRow in excelData.Rows)
            {
                DataRow newRow = mappedDataTable.NewRow();

                for (int i = 0; i < databaseFields.Count; i++)
                {
                    string dbField = databaseFields[i]; // Database field name
                    string excelField = excelFields[i]; // Corresponding Excel field name

                    if (allDBFieldList.Contains(dbField) && excelData.Columns.Contains(excelField))
                    {
                        newRow[dbField] = excelRow[excelField]; // Map data from Excel to the new DataTable
                    }
                }
                mappedDataTable.Rows.Add(newRow); // Add the populated row to the DataTable
            }

            string MyImportDataType = "";
            string MyImport = "";


            if (!string.IsNullOrEmpty(ddlImportDataTypeValue))
            {
                if (ddlImportDataTypeValue == "DUE")
                {
                    MyImport = "D";
                    MyImportDataType = "DUEDATA";
                }
                if (ddlImportDataTypeValue == "LAPSED")
                {
                    MyImport = "L";
                    MyImportDataType = "LAPSEDDATA";
                }
                if (ddlImportDataTypeValue == "PAID")
                {
                    MyImport = "D";
                    MyImportDataType = "DUEDATA";
                }
                if (ddlImportDataTypeValue == "REINS")
                {
                    MyImport = "L";
                    MyImportDataType = "LAPSEDDATA";
                }
            }




            string insertionResult = "";
            string isDupUpdated = "";
            string insertionBDDForBPDResult = "";
            if (ddlImportDataTypeValue == "DUE" || ddlImportDataTypeValue == "LAPSED")
            {
                insertionResult = new DueAndPaidDataImportingController().InsertDueData_N(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
            }
            else if (ddlImportDataTypeValue == "PAID" || ddlImportDataTypeValue == "REINS")
            {
                insertionResult = new DueAndPaidDataImportingController().InsertPaidData_N(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
            }


            Label1.Text = insertionResult;
            if (insertionResult.ToUpper().Contains("success".ToUpper()))
            {

                isDupUpdated = new DueAndPaidDataImportingController().BDD_UpdateDuplicatePolicies( Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), MyImportDataType);
                string dupPolMsg = isDupUpdated.Contains("success") ? " and " + isDupUpdated : null;




                ShowAlert("Data Imported Status: " + insertionResult + dupPolMsg); 
            }


            if(insertionResult.Contains("Not found due data"))
            {
                if(ddlImportDataTypeValue == "DUE" || ddlImportDataTypeValue == "LAPSED")
                {
                insertionBDDForBPDResult = new DueAndPaidDataImportingController().InsertDueData_N_For_Paid(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
                }
                insertionResult = new DueAndPaidDataImportingController().InsertPaidData_N(mappedDataTable, Convert.ToInt32(monthValue), Convert.ToInt32(yearValue), ddlImportDataTypeValue, MyImportDataType, MyImport, chkDataTypeValue, logginId, filePath);
                ShowAlert(insertionResult);
            }
            else
            {
                ShowAlert("Data Imported Status: " + insertionResult);
            }
            ShowAlert(insertionResult);
        }

        // Load sheet columns into dropdown
        public static void LoadSheetColumns(string filePath, string sheetName, DropDownList columnDropDown)
        {

            DataTable dtColumns = LoadSheetData(filePath, sheetName);

            //  lblsheetHeaderListHead.Text = string.Empty;

            // Assuming you want to append the sheet and headers into a string
            StringBuilder sheetHeaderListHead = new StringBuilder();

            StringBuilder sheetHeaderListBody = new StringBuilder();

            var headers = dtColumns.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            // Display sheet name in TextBox (if there's a TextBox like 'sheetHeaderText')
            sheetHeaderListHead.AppendLine($"Sheet: {sheetName}");

            // Format headers as 'sheetName_columnName'
            var formattedHeaders = headers.Select(header => $"{sheetName}_{header}");
            sheetHeaderListBody.AppendLine($"Sheet: {formattedHeaders}");



            columnDropDown.Items.Clear();
            columnDropDown.Items.Add(new ListItem("-- Select Column --", ""));

            foreach (DataColumn column in dtColumns.Columns)
            {

                columnDropDown.Items.Add(new ListItem(column.ColumnName, column.ColumnName));

                // sheetHeaderListHead.InnerHtml += $"<p class=\"mb-2 fw-bold\">Sheet: {sheet}</p>";


            }
        }



        private string uploadedFilePath;

        // Method to upload the Excel file and handle sheets
        private void UploadExcelFile()
        {
            // Define the directory path
            string uploadDirectory = Server.MapPath("~/Uploads/");

            // Check if the directory exists, if not, create it
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            // Generate a unique file name to prevent overwriting existing files
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileInput.FileName;
            string filePath = Path.Combine(uploadDirectory, uniqueFileName);

            // Save the file path for further use
            uploadedFilePath = filePath;

            // Save the file to the server
            fileInput.SaveAs(filePath);

            // Get the sheet names from the uploaded Excel file
            var sheetNames = GetExcelSheetNames(filePath);

            // Bind sheet names to the dropdown
            excelSheetSelect.Items.Clear();
            excelSheetSelect.Items.Add(new ListItem("Select Sheet", ""));
            foreach (var sheet in sheetNames)
            {
                excelSheetSelect.Items.Add(new ListItem(sheet, sheet));
            }

            // Create a StringBuilder to hold the header information
            var sheetHeaderText = new StringBuilder();

            // Clear the header dropdown before adding new items
            headerDropDown.Items.Clear();
            headerDropDown.Items.Add(new ListItem("Select Header", ""));

            // Initialize counter for total headers
            int totalHeaderCount = 0;

            try
            {
                foreach (var sheet in sheetNames)
                {
                    try
                    {
                        // Get headers for each sheet
                        var headers = GetSheetHeaders(filePath, sheet);

                        // Display sheet name and headers in the TextBox
                        sheetHeaderText.AppendLine($"Sheet: {sheet}");
                        sheetHeaderListHead.InnerHtml += $"<p class=\"mb-2 fw-bold\">Sheet: {sheet}</p>";

                        // Format headers as 'sheetName_field'
                        var formattedHeaders = headers.Select(header => $"{sheet}_{header}");

                        // Add formatted headers to the sheetHeaderListBody
                        sheetHeaderListBody.InnerHtml += $"<p>{sheet}: {string.Join(", <br/>", formattedHeaders)}<br/></p>";

                        // Append formatted headers to the TextBox content
                        sheetHeaderText.AppendLine(string.Join(", ", formattedHeaders));
                        sheetHeaderText.AppendLine(); // Add a blank line between sheets

                        // Add formatted headers to the headerDropDown dropdown
                        foreach (var formattedHeader in formattedHeaders)
                        {
                            headerDropDown.Items.Add(new ListItem(formattedHeader, formattedHeader));

                        }

                        // Increment total header count
                        totalHeaderCount += headers.Count;
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors during header extraction for a specific sheet
                        sheetHeaderText.AppendLine($"Error reading headers for sheet {sheet}: {ex.Message}");
                    }
                }

                // Set the TextBox with the total number of headers from all sheets
                txtSheetHeaderCount.Text = totalHeaderCount.ToString();
            }
            catch (Exception ex)
            {
                // Log or display the error as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Show the headers in the TextBox
            headerTextBox.Text = sheetHeaderText.ToString();

            // Show an alert with the unique file name (if needed)
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('File uploaded successfully: {uniqueFileName}');", true);
        }

        // Method to get sheet names from an Excel file
        private List<string> GetExcelSheetNames(string filePath)
        {
            List<string> sheetNames = new List<string>();

            // Get the correct connection string based on the file extension
            string connString = GetExcelConnectionString(filePath);

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow row in dt.Rows)
                {
                    // Add the sheet name to the list
                    sheetNames.Add(row["TABLE_NAME"].ToString());
                }
            }

            return sheetNames;
        }

        // Method to get the correct connection string for Excel file

        private string GetExcelConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            string connString = "";

            try
            {
                // Check if filePath is null or empty
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return "Error: File path is empty or null.";
                }

                // Check for valid file extensions
                if (extension.Equals(".xls", StringComparison.OrdinalIgnoreCase)) // For older Excel format
                {
                    connString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
                }
                else if (extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase)) // For newer Excel format
                {
                    connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1';";
                }
                else
                {
                    throw new InvalidOperationException("Unsupported file extension. Only .xls and .xlsx are supported.");
                }
            }
            catch (InvalidOperationException ioEx)
            {
                // Handle specific case for invalid file extensions
                return $"Error: {ioEx.Message}";
            }
            catch (UnauthorizedAccessException uaEx)
            {
                // Handle case for file access permission issues
                return $"Error: Unauthorized access. {uaEx.Message}";
            }
            catch (IOException ioEx)
            {
                // Handle general input-output errors (e.g., file in use, file not found)
                return $"Error: IO issue occurred. {ioEx.Message}";
            }
            catch (Exception ex)
            {
                // Generic exception handling to catch all other errors
                return $"Error: An unexpected error occurred. {ex.Message}";
            }

            return connString; // Return the connection string if everything is fine
        }

        // Method to get headers from a selected sheet
        private List<string> GetSheetHeaders(string filePath, string sheetName)
        {
            List<string> headers = new List<string>();

            string connString = GetExcelConnectionString(filePath);

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}]";
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, conn);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Get the column names (headers)
                foreach (DataColumn column in dataTable.Columns)
                {
                    headers.Add(column.ColumnName);
                }
            }

            return headers;
        }
 
        protected void MapMatchedFields(DropDownList headerDropDown, DropDownList ddlDbField, DropDownList ddlAlreadyMappedFieldList)
        {
            var excelFields = headerDropDown.Items.Cast<ListItem>().Select(item => item.Value).ToList();
            var dbFields = ddlDbField.Items.Cast<ListItem>().Select(item => item.Value).ToList();

            var matchedFields = new List<string>();

            foreach (var excelField in excelFields)
            {
                foreach (var dbField in dbFields)
                {
                    // Modify the condition to ensure it matches only relevant criteria
                    if (Regex.IsMatch(excelField, ".*", RegexOptions.IgnoreCase) && Regex.IsMatch(dbField, ".*", RegexOptions.IgnoreCase))
                    {
                        matchedFields.Add($"{excelField} >> {dbField}");
                    }
                }
            }

            // Clear the target dropdown before adding items
            ddlAlreadyMappedFieldList.Items.Clear();

            if (matchedFields.Count > 0)
            {
                // Add matched items to the dropdown
                foreach (var matchedField in matchedFields)
                {
                    ddlAlreadyMappedFieldList.Items.Add(new ListItem(matchedField, matchedField));
                }
            }
            else
            {
                // Show an alert if no matches are found
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No matches found between Excel headers and database fields.');", true);
            }
        }
 
        protected void CopyListItems(DropDownList sourceDropDown, DropDownList targetDropDown)
        {
            // Clear existing items in the target dropdown to avoid duplicates
            targetDropDown.Items.Clear();

            // Iterate through each item in the source dropdown
            foreach (ListItem item in sourceDropDown.Items)
            {
                // Add the item to the target dropdown
                targetDropDown.Items.Add(new ListItem(item.Text, item.Value));
            }


        }

        // Method for removing the selected item from the mapped fields list
        protected void RemoveSelectedMapping(object sender, EventArgs e)
        {
            // Check if a mapping is selected in the dropdown
            if (ddlMappedFieldList.SelectedIndex == -1)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please select a field mapping to remove.');", true);
                return;
            }


            var value = ddlMappedFieldList.SelectedItem.Text;  // Use Text or Value instead of SelectedIndex

            string[] parts = value.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                headerDropDown.Items.Add(parts[0].Trim());  // Assuming you want to add the first part to headerDropDown
            }

            // Remove the selected item from the dropdown
            ddlMappedFieldList.Items.RemoveAt(ddlMappedFieldList.SelectedIndex);
            lblMappingMessage.Text = "Mapping removed: " + value;
            lblMappingMessage.CssClass = "text-info";


        }

        // Method for adding the selected fields to the mapped list

        protected void SetMatchedField(object sender, EventArgs e)
        {
            MapMatchedFields(headerDropDown, ddlDbField, ddlMappedFieldList);
        }

        protected void SetPreviousField(object sender, EventArgs e)
        {
            CopyListItems(ddlAlreadyMappedFieldList, ddlMappedFieldList);
        }
      
        protected void MapSelectedFields(object sender, EventArgs e)
        {
            // Get the selected values from the Excel and Database field dropdowns
            var selectedExcelField = headerDropDown.SelectedValue;
            var selectedDbField = ddlDbField.SelectedValue;

            int ex_index = headerDropDown.SelectedIndex;

            // Validate if both Excel Field and Database Field are selected
            if (string.IsNullOrEmpty(selectedExcelField) || string.IsNullOrEmpty(selectedDbField))
            {
                
                lblMappingMessage.Text = "Please select both Excel Field and Database Field before mapping.";
                lblMappingMessage.CssClass = "text-warning";
                return;
            }

            // Check if either the selectedExcelField or selectedDbField already exists in any mapping
            var isAlreadyMapped = ddlMappedFieldList.Items.Cast<ListItem>().Any(item =>
            {
                string[] parts = item.Text.Split(new[] { " >> " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    string mappedExcelField = parts[0].Trim();
                    string mappedDbField = parts[1].Trim();
                    return mappedExcelField == selectedExcelField || mappedDbField == selectedDbField;
                }
                return false;
            });

            if (isAlreadyMapped)
            {
                lblMappingMessage.Text = "This Excel field or DB field is already mapped with another field.";
                lblMappingMessage.CssClass = "text-warning";
                return;
            }

            // Add the new mapping to the dropdown list
            string mappedCurrentMappedItem = $"{selectedExcelField} >> {selectedDbField}";
            ddlMappedFieldList.Items.Add(new ListItem(mappedCurrentMappedItem, mappedCurrentMappedItem));
            lblMappingMessage.Text = "Mapping Added: " + mappedCurrentMappedItem.ToString();
            lblMappingMessage.CssClass = "text-success";

            // Remove the selectedExcelField from the headerDropDown
            ListItem headerItemToRemove = headerDropDown.Items.Cast<ListItem>().FirstOrDefault(item => item.Text == selectedExcelField);
            if (headerItemToRemove != null)
            {
                headerDropDown.Items.Remove(headerItemToRemove);
            }



            //ddlMappedFieldList.Items.Add(new ListItem($"{selectedExcelField} >> {selectedDbField}", $"{selectedExcelField}_{selectedDbField}"));
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Mapping added: {selectedExcelField} >> {selectedDbField}');", true);

        }

        protected void btnResetMappings_Click(object sender, EventArgs e)
        {
            headerDropDown.Items.Clear();
            ddlMappedFieldList.Items.Clear();

            string filePath = Session["CurrentDAPExcelFile"]?.ToString();
            string selectedSheet = excelSheetSelect.SelectedValue;
            LoadSheetColumns(filePath, selectedSheet, headerDropDown);

            lblMappingMessage.Text = "Mapped fields reset";
            lblMappingMessage.CssClass = "text-danger";

        }

        public void FillPreviousMapped()
        {
            try
            {

                // Define the file path for reading
                string currentImpDataName = ddlImportDataType.SelectedValue.ToString();
                string filePath = Server.MapPath("~/SampleFile/DuePaidMappedField" + currentImpDataName +".txt");

                // Clear existing items in the dropdown
                ddlAlreadyMappedFieldList.Items.Clear();

                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Read all lines from the file
                    string[] mappedFields = System.IO.File.ReadAllLines(filePath);

                    // Check if the file is not empty
                    if (mappedFields.Length > 0)
                    {
                        // Iterate over each line and add it to the dropdown
                        foreach (string field in mappedFields)
                        {
                            if (!string.IsNullOrWhiteSpace(field))
                            {
                                ddlAlreadyMappedFieldList.Items.Add(new ListItem(field, field));
                            }

                        }
                    }
                    else
                    {
                        // Add a "Not Saved" item if the file is empty
                        ddlAlreadyMappedFieldList.Items.Add(new ListItem("Not Saved", "Not Saved"));
                    }
                }
                else
                {
                    // Add a "Not Saved" item if the file does not exist
                    ddlAlreadyMappedFieldList.Items.Add(new ListItem("Not Saved", "Not Saved"));
                }
            }

            catch (Exception ex)
            {
                ddlAlreadyMappedFieldList.Items.Add(new ListItem("Not Saved", "Not Saved"));

            }
        }

        protected void btnLoadMappings_Click(object sender, EventArgs e)
        {

            if (headerDropDown.Items.Count>1 && ddlAlreadyMappedFieldList.Items.Count > 0)
            {
                ddlMappedFieldList.Items.Clear();
                CopyListItems(ddlAlreadyMappedFieldList, ddlMappedFieldList);


                foreach (ListItem item in ddlMappedFieldList.Items)
                {
                    string[] parts = item.ToString().Split(new[] { " >> " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        string excelfieldname = parts[0].Trim();
                        ListItem headerItemToRemove = headerDropDown.Items
                            .Cast<ListItem>()
                            .FirstOrDefault(headerItem => headerItem.Text.Trim() == excelfieldname);
                        if (headerItemToRemove != null)
                        {
                            headerDropDown.Items.Remove(headerItemToRemove);
                        }

                    }
                }

            }

            if (ddlAlreadyMappedFieldList.Items.Count < 1)
            {
                ShowAlert("Previous Mapping Not Exsit");
            }


        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Define the file path for saving
                string currentImpType = ddlImportDataType.SelectedValue.ToString();
                int currentMappenIten = ddlMappedFieldList.Items.Count;
                if(!string.IsNullOrEmpty(currentImpType) && currentMappenIten>1)
                {
                    string filePath = Server.MapPath("~/SampleFile/DuePaidMappedField"+ currentImpType+".txt");

                    // Ensure the directory exists
                    string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                    if (!System.IO.Directory.Exists(directoryPath))
                    {
                        System.IO.Directory.CreateDirectory(directoryPath);
                    }

                    // Collect all mapped fields from the dropdown list
                    List<string> mappedFields = new List<string>();
                    foreach (ListItem item in ddlMappedFieldList.Items)
                    {
                        mappedFields.Add(item.Text);
                    }

                    // Write the mapped fields to the file
                    System.IO.File.WriteAllLines(filePath, mappedFields);

                    // Call the function to load the saved mappings into the dropdown
                    FillPreviousMapped();

                    // Provide feedback to the user
                    lblMappingMessage.Text = "Mappings saved successfully.";
                    lblMappingMessage.CssClass = "text-success";
                }
                else
                {

                ShowAlert("Choose an import data type and set some mapping");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", $"alert('An error occurred: {ex.Message}');", true);

            }
        }


        #region FillYearMonths
        private void FillYearMonths()
        {

            // For months dropdown
            // ddlMonths.DataSource = new WM.Controllers.InsuranceCompanyMasterController().GetMonthsList();
            //  ddlMonths.DataBind();

            // Get the current year
            int currentYear = DateTime.Now.Year;

            // Populate the dropdown
            yearSelect.DataSource = new WM.Controllers.DueAndPaidDataImportingController().GetYearsList(15, 2030);
            yearSelect.DataBind();

            if(yearSelect.Items.Count > 1)
            {
                // Set the selected value to the current year
                ListItem currentYearItem = yearSelect.Items.FindByValue(currentYear.ToString());
                if (currentYearItem != null)
                {
                    yearSelect.ClearSelection();
                    currentYearItem.Selected = true;
                }
            }

        }
        #endregion

        protected void BrowseButton_Click(object sender, EventArgs e)
        {
            // Code to handle file browsing.
            // File upload handling can be done here.
            //if (fileInput.HasFile)
            //{
            //    string fileName = fileInput.FileName;
            //    // Handle file upload logic here.
            //}
        }

        protected void MapFieldsButton_Click(object sender, EventArgs e)
        {
            // Code to handle mapping fields.
            // Add your logic for mapping fields here.
        }

        protected void ImportButton_Click(object sender, EventArgs e)
        {
            // Validate all inputs
            if (!ValidateInput())
                return;

            // Proceed with data import if validation passes
            ImportDatainDB();
        }

        private bool ValidateInput()
        {
            if (Session["CurrentDAPExcelFile"] == null)
            {
                ShowAlert("First upload a file!");
                fileInput.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(excelSheetSelect.SelectedValue))
            {
                ShowAlert("Choose an Excel sheet!");
                excelSheetSelect.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(monthSelect.SelectedValue))
            {
                ShowAlert("Choose a month value!");
                monthSelect.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(yearSelect.SelectedValue))
            {
                ShowAlert("Choose a year value!");
                yearSelect.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(ddlImportDataType.SelectedValue))
            {
                ShowAlert("Choose an import data type!");
                ddlImportDataType.Focus();
                return false;
            }

           
            if (string.IsNullOrEmpty(ddlMappedFieldList.SelectedValue))
            {
                ShowAlert("Mapping is required!");
                btnMapFields.Focus();
                return false;
            }

            return true;
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{message}');", true);
        }

        protected void ExitButton_Click(object sender, EventArgs e)
        {
            // Code to handle exit functionality.
            // Redirect to another page or perform cleanup here.
            //Response.Redirect("~/welcome.aspx");
            //ScriptManager.RegisterStartupScript(this, GetType(), "closeModalScript", "closeMapFieldsModal();", true);

            psmController.RedirectToWelcomePage();
        }

        protected void MappingPlanButton_Click(object sender, EventArgs e)
        {
            // Code to handle mapping plan functionality.
            // Add your logic for mapping plan here.
        }

        private void FillDbFieldListDropDown()
        {
            // Call the method to get the data from the stored procedure
            DataTable dt = new WM.Controllers.DueAndPaidDataImportingController().GetDbFieldList();

            // Clear the dropdown list first
            ddlDbField.Items.Clear();

            // Check if the data returned is not null and has rows
            if (dt != null && dt.Rows.Count > 0)
            {
                // Bind the data to the dropdown list
                ddlDbField.DataSource = dt;
                ddlDbField.DataTextField = "Text";
                ddlDbField.DataValueField = "Value";
                ddlDbField.DataBind();

                // Insert a default "Select" option at the top of the list
                ddlDbField.Items.Insert(0, new ListItem("Select DB Field", ""));
            }
        }

        private void FillDbFieldListDropDownByType(string typeName)
        {
            // Get the selected import data type or default to the provided typeName
            string selectedImportDataType = ddlImportDataType.SelectedValue;
            string forName = !string.IsNullOrEmpty(selectedImportDataType) ? ddlImportDataType.SelectedItem.ToString() : typeName;

            // Clear and disable dropdown if no valid selection
            if (string.IsNullOrEmpty(selectedImportDataType))
            {
                ddlDbField.Items.Clear();
                ddlDbField.Enabled = false;
                return;
            }

            // Fetch the database fields list based on the type
            DataTable dbFields = new WM.Controllers.DueAndPaidDataImportingController().GetDbFieldListByType(typeName);

            // Clear the dropdown
            ddlDbField.Items.Clear();

            if (dbFields != null && dbFields.Rows.Count > 0)
            {
                // Bind the data to the dropdown
                ddlDbField.DataSource = dbFields;
                ddlDbField.DataTextField = "Text";
                ddlDbField.DataValueField = "Value";
                ddlDbField.DataBind();

                // Add default "Select" option
                ddlDbField.Items.Insert(0, new ListItem($"Select DB Field for {forName}", ""));
                ddlDbField.Enabled = true;
            }
            else
            {
                // No data: add placeholder and disable dropdown
                ddlDbField.Items.Add(new ListItem($"Select DB Field for {forName}", ""));
                ddlDbField.Enabled = false;
            }
        }

        protected void ddlDataTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

            string ddlValue = ddlImportDataType.SelectedValue.ToString();
            FillDbFieldListDropDownByType(ddlValue);


            if (!string.IsNullOrEmpty(ddlValue))
            {
                if (ddlValue == "DUE"){
                    chkOptDue.Checked = true;
                    chkOptPaid.Checked = false;
                    chkOptLap.Checked = false;
                    chkOptRein.Checked = false;

                    chkOptDue.Visible = true;
                    chkOptPaid.Visible = true;
                    //chkOptLap.Enabled = false;
                    //chkOptRein.Enabled = false;

                    chkOptLap.Visible = false;
                    chkOptRein.Visible = false;

                }
                if (ddlValue == "LAPSED")
                {
                   
                    chkOptLap.Checked = true;

                    chkOptDue.Checked = false;
                    chkOptPaid.Checked = false;
                    chkOptRein.Checked = false;


                    chkOptDue.Visible = false;
                    chkOptPaid.Visible = false;
                    chkOptLap.Visible = true;
                    chkOptRein.Visible = true;
                }
                if (ddlValue == "PAID")
                {
                    chkOptPaid.Checked = true;
                    chkOptDue.Checked = false;
                    chkOptLap.Checked = false;
                    chkOptRein.Checked = false;


                    chkOptDue.Visible = true;
                    chkOptPaid.Visible = true;
                    chkOptLap.Visible = false;
                    chkOptRein.Visible = false;
                }
                if (ddlValue == "REINS")
                { 
                    chkOptRein.Checked = true;

                    chkOptLap.Checked = false;

                    chkOptDue.Checked = false;
                    chkOptPaid.Checked = false;


                    chkOptDue.Visible = false;
                    chkOptPaid.Visible = false;
                    chkOptLap.Visible = true;
                    chkOptRein.Visible = true;
                }

                FillPreviousMapped();

            }

           


        }
    }
}
