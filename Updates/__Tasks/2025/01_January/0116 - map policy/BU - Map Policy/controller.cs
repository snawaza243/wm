using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web;
using System.Web.Configuration;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using Excel = Microsoft.Office.Interop.Excel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // If you also want to support .xlsx
using NPOI.HSSF.UserModel; // for .xls
using NPOI.SS.UserModel;
using System.IO;

namespace WM.Controllers
{
    public class MapPolicyNumberController : System.Web.UI.Page
    {
        private readonly string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;


        // Helper method to get the connection string for the Excel file
        private string GetExcelConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            string connString = "";

            if (extension == ".xls")
            {
                connString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
            }
            else if (extension == ".xlsx")
            {
                connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1';";
            }

            return connString;
        }

   

public string GeneratePolicyReport()
    {
        // Step 1: Retrieve data from the database
        DataTable policyData = GetPolicyMapReport(); // Fetch data from the database

        // Step 2: Create NPOI objects
        IWorkbook workbook = new HSSFWorkbook(); // For creating .xls file
        ISheet sheet = workbook.CreateSheet("Policy Report");

        // Step 3: Add headers
        IRow headerRow = sheet.CreateRow(0);
        for (int i = 0; i < policyData.Columns.Count; i++)
        {
            headerRow.CreateCell(i).SetCellValue(policyData.Columns[i].ColumnName);
        }

        // Step 4: Add data
        for (int i = 0; i < policyData.Rows.Count; i++)
        {
            DataRow dr = policyData.Rows[i];
            IRow row = sheet.CreateRow(i + 1);
            for (int j = 0; j < policyData.Columns.Count; j++)
            {
                row.CreateCell(j).SetCellValue(dr[j].ToString());
            }
        }

        // Step 5: Define the directory where the file will be saved
        string folderPath = Server.MapPath("~/Reports");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Step 6: Save the file as .xls
        string savePath = Path.Combine(folderPath, "policymap.xls");
        using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(fileStream);
        }

        // Step 7: Return a success message with the download link
        var reportUrl = ResolveUrl("~/Reports/policymap.xls");
        return $"Policy report generated successfully. You can download it from: <a href='{reportUrl}'>{reportUrl}</a>";
    }


    public string GeneratePolicyReport1()
        {
            // Step 1: Retrieve data from the database
            DataTable policyData = GetPolicyMapReport(); // Fetch data from the database

            // Step 2: Create an HSSFWorkbook for .xls (Excel 97-2003) format
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Policy Report");

            // Step 3: Define the headers in the first row
            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("POLICY_NO");
            headerRow.CreateCell(1).SetCellValue("MAX_AMT");
            headerRow.CreateCell(2).SetCellValue("PREM_FREQ");
            headerRow.CreateCell(3).SetCellValue("NEXT_DUE_DT");
            headerRow.CreateCell(4).SetCellValue("COMPANY_CD");
            headerRow.CreateCell(5).SetCellValue("REGION_NAME");
            headerRow.CreateCell(6).SetCellValue("ZONE_NAME");
            headerRow.CreateCell(7).SetCellValue("RM_NAME");
            headerRow.CreateCell(8).SetCellValue("BRANCH_NAME");
            headerRow.CreateCell(9).SetCellValue("INVESTOR_NAME");
            headerRow.CreateCell(10).SetCellValue("ADDRESS1");
            headerRow.CreateCell(11).SetCellValue("ADDRESS2");
            headerRow.CreateCell(12).SetCellValue("CITY_NAME");
            headerRow.CreateCell(13).SetCellValue("STATE_NAME");
            headerRow.CreateCell(14).SetCellValue("MOBILE");
            headerRow.CreateCell(15).SetCellValue("PHONE");

            // Step 4: Load the data into the sheet starting from the second row
            for (int i = 0; i < policyData.Rows.Count; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                row.CreateCell(0).SetCellValue(policyData.Rows[i]["POLICY_NO"].ToString());
                row.CreateCell(1).SetCellValue(policyData.Rows[i]["MAX_AMT"].ToString());
                row.CreateCell(2).SetCellValue(policyData.Rows[i]["PREM_FREQ"].ToString());
                row.CreateCell(3).SetCellValue(policyData.Rows[i]["NEXT_DUE_DT"].ToString());
                row.CreateCell(4).SetCellValue(policyData.Rows[i]["COMPANY_CD"].ToString());
                row.CreateCell(5).SetCellValue(policyData.Rows[i]["REGION_NAME"].ToString());
                row.CreateCell(6).SetCellValue(policyData.Rows[i]["ZONE_NAME"].ToString());
                row.CreateCell(7).SetCellValue(policyData.Rows[i]["RM_NAME"].ToString());
                row.CreateCell(8).SetCellValue(policyData.Rows[i]["BRANCH_NAME"].ToString());
                row.CreateCell(9).SetCellValue(policyData.Rows[i]["INVESTOR_NAME"].ToString());
                row.CreateCell(10).SetCellValue(policyData.Rows[i]["ADDRESS1"].ToString());
                row.CreateCell(11).SetCellValue(policyData.Rows[i]["ADDRESS2"].ToString());
                row.CreateCell(12).SetCellValue(policyData.Rows[i]["CITY_NAME"].ToString());
                row.CreateCell(13).SetCellValue(policyData.Rows[i]["STATE_NAME"].ToString());
                row.CreateCell(14).SetCellValue(policyData.Rows[i]["MOBILE"].ToString());
                row.CreateCell(15).SetCellValue(policyData.Rows[i]["PHONE"].ToString());
            }

            // Step 5: Define the report file path
            string reportsFolder = Server.MapPath("~/Reports");

            // Check if the Reports directory exists, if not, create it
            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder); // Create directory if it doesn't exist
            }

            string reportName = "PolicyReport.xls"; // Use .xls extension for Excel 97-2003 format
            string filePath = Path.Combine(reportsFolder, reportName);

            // Delete the old file if it exists
            if (File.Exists(filePath))
            {
                File.Delete(filePath); // Delete the existing file
            }

            // Step 6: Save the file to the Reports folder
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream); // Save the workbook to the file stream
            }

            // Construct the URL using ResolveUrl
            var reportUrl = ResolveUrl("~/Reports/" + Path.GetFileName(filePath));

            // Step 7: Prepare the response to download the file
            return $"Policy report generated successfully. You can download it from: <a href='{reportUrl}'>{reportUrl}</a>";
        }


        public string GeneratePolicyReport0()
        {
            // Step 1: Retrieve data from the database
            DataTable policyData = GetPolicyMapReport(); // Fetch data from the database
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context here
            // Step 2: Create Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Policy Report");

                // Step 3: Define the headers based on your SQL query
                worksheet.Cells[1, 1].Value = "POLICY_NO";
                worksheet.Cells[1, 2].Value = "MAX_AMT";
                worksheet.Cells[1, 3].Value = "PREM_FREQ";
                worksheet.Cells[1, 4].Value = "NEXT_DUE_DT";
                worksheet.Cells[1, 5].Value = "COMPANY_CD";
                worksheet.Cells[1, 6].Value = "REGION_NAME";
                worksheet.Cells[1, 7].Value = "ZONE_NAME";
                worksheet.Cells[1, 8].Value = "RM_NAME";
                worksheet.Cells[1, 9].Value = "BRANCH_NAME";
                worksheet.Cells[1, 10].Value = "INVESTOR_NAME";
                worksheet.Cells[1, 11].Value = "ADDRESS1";
                worksheet.Cells[1, 12].Value = "ADDRESS2";
                worksheet.Cells[1, 13].Value = "CITY_NAME";
                worksheet.Cells[1, 14].Value = "STATE_NAME";
                worksheet.Cells[1, 15].Value = "MOBILE";
                worksheet.Cells[1, 16].Value = "PHONE";

                // Step 4: Load the data into the worksheet starting from the second row
                worksheet.Cells["A2"].LoadFromDataTable(policyData, false); // Load data without headers


                // Step 5: Define the report file path
                string reportName = "PolicyReport.xlsx";
                string filePath = Path.Combine(Server.MapPath("~/Reports"), reportName);

                // Delete the old file if it exists
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Delete the existing file
                }


                // Construct the URL using ResolveUrl
                var reportUrl = ResolveUrl("~/Reports/" + Path.GetFileName(filePath));

                // Step 4: Prepare the response to download the file
                var stream = new MemoryStream();
                package.SaveAs(stream); // Save the package to the stream
                stream.Position = 0; // Reset the stream position to the beginning


                return $"Policy report generated successfully. You can download it from: <a href='{reportUrl}'>{reportUrl}</a>";
            }
        }

        public DataTable GetPolicyMapReport()
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_MPN_GetPolicyMapReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Define the cursor parameter
                    OracleParameter cursorParam = new OracleParameter
                    {
                        ParameterName = "p_cursor",
                        OracleDbType = OracleDbType.RefCursor,
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(cursorParam);

                    con.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        private void GenerateExcelReport(DataTable data, string filePath)
        {
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Report");
                ws.Cells["A1"].LoadFromDataTable(data, true);
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                package.SaveAs(new FileInfo(filePath));
            }
        }


        bool impStatus = false;

        public string ImportExcelToDatabase(string filePath, string selectedSheet)
        {
            string confirmationMessage = string.Empty;

            try
            {
                // Step 1: Read data from Excel file
                DataTable excelData = ReadExcelData(filePath, selectedSheet);

                if (excelData == null || excelData.Rows.Count == 0)
                {
                    return "No data found in the selected sheet.";
                }

                // Step 2: Insert data into the Oracle database
                using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    con.Open();

                    // Clear existing data
                    using (OracleCommand deleteCommand = new OracleCommand("DELETE FROM POLICY_MAP_TEMP1", con))
                    {
                        deleteCommand.ExecuteNonQuery();
                    }

                    confirmationMessage = "Policy Number - Company Code<br/>";  // Initialize the message header

                    // Insert the Excel data row by row
                    foreach (DataRow row in excelData.Rows)
                    {
                        // Check if first column is not empty
                        if (!string.IsNullOrWhiteSpace(row[0].ToString()))
                        {
                            using (OracleCommand cmd = new OracleCommand("PSM_MPN_InsertExcelPolicyData", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Parameter for policy number
                                string policyNo = row[0].ToString().Replace("-", "");
                                cmd.Parameters.Add("p_policy_no", OracleDbType.Varchar2).Value = policyNo;

                                // Build the confirmation message for policy number
                                confirmationMessage += $"Policy No: {policyNo}";

                                // Additional parameter check for the second column
                                if (excelData.Columns.Count > 1 && !string.IsNullOrWhiteSpace(row[1].ToString()))
                                {
                                    string companyCode = row[1].ToString();
                                    cmd.Parameters.Add("p_column2", OracleDbType.Varchar2).Value = companyCode;

                                    // Append company code to confirmation message
                                    confirmationMessage += $" - Company Code: {companyCode}<br/>";
                                }
                                else
                                {
                                    confirmationMessage += " - Company Code: N/A<br/>"; // Handle case when company code is not present
                                }

                                // Execute the command
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    impStatus = true;

                    confirmationMessage = "Data inserted successfully!<br/>" + confirmationMessage; // Add success message with details
                }
            }
            catch (OracleException oracleEx)
            {
                confirmationMessage = "Database error: " + oracleEx.Message;
            }
            catch (Exception ex)
            {
                confirmationMessage = "Error: " + ex.Message;
            }

            return confirmationMessage;
        }

        // Import Excel data and insert it into POLICY_MAP_TEMP1
        //[HttpPost]
        public string ImportExcelToDatabase0(string filePath, string selectedSheet)
        {
            string confirmationMessage = string.Empty;

            try
            {
                // Step 1: Read data from Excel file
                DataTable excelData = ReadExcelData(filePath, selectedSheet);

                if (excelData == null || excelData.Rows.Count == 0)
                {
                    return "No data found in the selected sheet.";
                }

                // Step 2: Insert data into the Oracle database
                using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    con.Open();

                    // Clear existing data
                    using (OracleCommand deleteCommand = new OracleCommand("DELETE FROM POLICY_MAP_TEMP1", con))
                    {
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Insert the Excel data row by row
                    foreach (DataRow row in excelData.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(row[0].ToString())) // Check if first column is not empty
                        {
                            using (OracleCommand cmd = new OracleCommand("PSM_MPN_InsertExcelPolicyData", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Parameter for policy number
                                cmd.Parameters.Add("p_policy_no", OracleDbType.Varchar2).Value = row[0].ToString().Replace("-", "");
                                // Additional parameter check
                                if (excelData.Columns.Count > 1 && !string.IsNullOrWhiteSpace(row[1].ToString()))
                                {
                                    cmd.Parameters.Add("p_column2", OracleDbType.Varchar2).Value = row[1].ToString();
                                }

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    confirmationMessage = "Data inserted successfully!";
                }
            }
            catch (OracleException oracleEx)
            {
                confirmationMessage = "Database error: " + oracleEx.Message;
            }
            catch (Exception ex)
            {
                confirmationMessage = "Error: " + ex.Message;
            }

            return confirmationMessage;
        }


        private DataTable ReadExcelData(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();
            string connString = GetExcelConnectionString(filePath);

            // Format the sheet name properly
            string formattedSheetName = FormatSheetName(sheetName);

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    // Use the formatted sheet name
                    OleDbDataAdapter da = new OleDbDataAdapter($"SELECT * FROM {formattedSheetName}", conn);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    // Handle exceptions and log error if needed
                    throw new Exception($"Error reading Excel data: {ex.Message}");
                }
            }

            return dt;
        }

        // Helper method to format the sheet name
        private string FormatSheetName(string sheetName)
        {
            // Trim any leading or trailing spaces
            sheetName = sheetName.Trim();

            // Check if the sheet name ends with a dollar sign
            if (!sheetName.EndsWith("$"))
            {
                sheetName += "$"; // Append $ if not present
            }

            // Wrap the sheet name in square brackets if it contains spaces or special characters
            if (sheetName.Contains(" ") || sheetName.Contains("'") || sheetName.Contains("["))
            {
                sheetName = $"[{sheetName}]"; // Wrap in brackets if necessary
            }

            return sheetName;
        }



        public void ImportExcelData(string filePath, string sheetName, string companyCode)
        {
            string excelFilePath = Path.GetFullPath(filePath);

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("PSM_MPN_ImportExcelData", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding input parameters for the procedure
                        cmd.Parameters.Add("p_filename", OracleDbType.Varchar2).Value = excelFilePath;
                        cmd.Parameters.Add("p_sheet_name", OracleDbType.Varchar2).Value = sheetName;
                        cmd.Parameters.Add("p_company_code", OracleDbType.Varchar2).Value = companyCode;


                        // Execute the procedure
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (OracleException ex)
                {
                    // Handle Oracle-specific errors
                    Console.WriteLine("Oracle Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle general errors
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }










        #region GetCompanyList
        public DataTable GetCompanyList()
        {
            DataTable dtCompanyList = new DataTable();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_MPN_GET_COMPANY_MASTER", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding the OUT parameter for the cursor
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Execute the command and fill the DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtCompanyList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error message
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return dtCompanyList;
        }
        #endregion



















        #region GetExcelSheetNames
        public DataTable GetExcelSheetNames(string filePath)
        {
            DataTable dtSheets = new DataTable();
            try
            {
                // Connection string for Excel file
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0;HDR=YES;'";
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    dtSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                }
            }
            catch (Exception ex)
            {
                // Log the error message
                Console.WriteLine("Error: " + ex.Message);
            }

            return dtSheets;
        }
        #endregion

        #region UploadFile
        public void UploadFile(HttpPostedFile file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);

                if (Path.GetExtension(fileName).Equals(".xls", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(fileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    file.SaveAs(filePath);

                    // Process the Excel file
                    ProcessExcelFile(filePath);

                    // Redirect to another page or update the view as needed
                    Response.Redirect("SuccessPage.aspx");
                }
                else
                {
                    // Set an error message to be displayed on the view
                    ViewState["Message"] = "Please upload an Excel file.";
                }
            }
            else
            {
                // Set an error message to be displayed on the view
                ViewState["Message"] = "No file uploaded.";
            }
        }
        #endregion

        #region ProcessExcelFile
        private void ProcessExcelFile(string filePath)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(filePath);
            Excel._Worksheet xlWorkSheet = xlWorkBook.Sheets[1];
            Excel.Range xlRange = xlWorkSheet.UsedRange;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                con.Open();

                // Call stored procedure to clear existing data
                using (OracleCommand cmd = new OracleCommand("DELETE_POLICY_MAP_TEMP1", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                // Insert data from Excel into temporary table
                for (int row = 2; row <= xlRange.Rows.Count; row++)
                {
                    if (xlRange.Cells[row, 1] != null && xlRange.Cells[row, 1].Value2 != null)
                    {
                        string policyNo = xlRange.Cells[row, 1].Value2.ToString().Replace("-", "").Trim();
                        string companyCd = xlRange.Cells[row, 2].Value2.ToString().Trim();

                        using (OracleCommand cmd = new OracleCommand("INSERT_POLICY_MAP_TEMP1", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new OracleParameter("P_POLICY_NO", policyNo));
                            cmd.Parameters.Add(new OracleParameter("P_COMPANY_CD", companyCd));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Execute stored procedure to get policy data
                using (OracleCommand cmd = new OracleCommand("GET_POLICY_DATA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("OUT_POLICY_DATA_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        // Save the data to an Excel file or use it as needed
                        SaveToExcel(dr);
                    }
                }
            }

            // Cleanup
            xlWorkBook.Close(false);
            xlApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlRange);
        }
        #endregion

        #region SaveToExcel
        private void SaveToExcel(OracleDataReader dr)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Excel._Worksheet xlWorkSheet = xlWorkBook.Sheets[1];

            int row = 1;

            // Header
            for (int i = 0; i < dr.FieldCount; i++)
            {
                xlWorkSheet.Cells[row, i + 1] = dr.GetName(i);
            }

            // Data
            while (dr.Read())
            {
                row++;
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    xlWorkSheet.Cells[row, i + 1] = dr.GetValue(i).ToString();
                }
            }

            string savePath = Server.MapPath("~/App_Data/Reports/policymap.xlsx");
            xlWorkBook.SaveAs(savePath);
            xlWorkBook.Close(false);
            xlApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
        }
        #endregion
    }
}
