using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Models;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Web.UI.HtmlControls;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Linq.Expressions;
using Microsoft.Ajax.Utilities;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Office.Interop.Excel;

namespace WM.Masters
{
    public partial class MF_Trail : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnImpExe_Click(object sender, EventArgs e)
        {
            importexcel();
        }
        private void importexcel()
        {
            try
            {
                string path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MTrail", "opp_log.txt");
                string login_id = Session["LoginId"]?.ToString();
                string role_id = Session["RoleId"]?.ToString();
                File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "  If satrted here  ");

                string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    if (Execle_uploader.PostedFile == null)
                    {
                        Status_uploader.Text = "Please upload the file ";
                        Status_uploader.CssClass = "text-danger";
                        return;
                    }

                    File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "if started here ");
                    string strpath = Execle_uploader.PostedFile.FileName;
                    if ((!Path.GetExtension(strpath).ToUpper().Equals(".XLS") && !Path.GetExtension(strpath).ToUpper().Equals(".XLSX")) || String.IsNullOrEmpty(Path.GetFileNameWithoutExtension(strpath)))
                    {
                        Status_uploader.Text = "Please select a valid Excel File with .xls Extension!";
                        Status_uploader.CssClass = "text-danger";
                        return;
                    }

                    System.Data.DataTable dt = new System.Data.DataTable();
                    Status_uploader.Text = "";
                    Execle_uploader.PostedFile.SaveAs(HttpContext.Current.Server.MapPath("~/MTrail/" + strpath));
                    string path = HttpContext.Current.Server.MapPath("~/MTrail/" + strpath);
                    File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "parse started");
                    string message = string.Empty;
                    dt = Parse(path, ref message);
                    File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "parse Ended");

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        Status_uploader.Text = message ?? "Error encountered while reading data from the Excel file.";
                        Status_uploader.CssClass = "text-danger";
                        return;
                    }

                    try
                    {
                        if (connection.State != ConnectionState.Open) connection.Open();

                        foreach (DataRow row in dt.Rows)
                        {
                            string foliono = row[0]?.ToString();
                            File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "table enrty started");

                            string insstr = "INSERT INTO IMPORT_FOLIOS_TEMP (FOLIO_NUMBER, LOGIN_ID, ROLE_ID, TIMEST) VALUES (" + foliono + "," + login_id + " ," + role_id + ", SYSDATE)";

                            OracleCommand cmd = new OracleCommand(insstr, connection);
                            File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "Table enrty started");

                            cmd.ExecuteNonQuery();
                            File.AppendAllText(path1, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "cmd executed");
                        }

                        Status_uploader.Text = "File Uploaded Sucessfully";
                        Status_uploader.CssClass = "text-success";
                        btnExport.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        Status_uploader.Text = ex.Message;
                        Status_uploader.CssClass = "text-danger";
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open) connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Status_uploader.Text = ex.Message;
                Status_uploader.CssClass = "text-danger";
            }
        }
        static System.Data.DataTable Parse(string fileName, ref string message)
        {
            string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MTrail", "opp_log.txt");
            string extension = Path.GetExtension(fileName).ToLower();
            string connectionString;

            File.AppendAllText(path2, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "Case started");

            switch (extension)
            {
                case ".xls":
                    //connectionString = $"Provider=Microsoft.Jet.OLEDB.12.0;Data Source={fileName};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\"";
                    break;
                case ".xlsx":
                    connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fileName};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\"";
                    break;
                default:
                    throw new NotSupportedException("Unsupported file format.");
            }
            File.AppendAllText(path2, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "Case started");

            System.Data.DataTable data = new System.Data.DataTable();
            File.AppendAllText(path2, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + "Try satrted");
            try
            {
                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();

                    // Retrieve the sheet names
                    System.Data.DataTable schemaTable = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (schemaTable == null || schemaTable.Rows.Count == 0)
                    {
                        throw new Exception("No sheets found in the Excel file.");
                    }

                    // Assume we want to read the first sheet
                    string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString();
                    //string query = $"SELECT * FROM [{sheetName}]";
                    string query = $"SELECT * FROM [{sheetName}] ";

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, con))
                    {
                        adapter.Fill(data);
                        File.AppendAllText(path2, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + data + "Case started");
                    }
                    // After filling the DataTable, iterate over each row and check if it's empty
                    for (int i = data.Rows.Count - 1; i >= 0; i--)
                    {
                        bool isEmpty = true;
                        foreach (var item in data.Rows[i].ItemArray)
                        {
                            if (item != DBNull.Value && !string.IsNullOrWhiteSpace(item.ToString()))
                            {
                                isEmpty = false;
                                break;
                            }
                        }

                        // If the row is empty, delete it
                        if (isEmpty)
                        {
                            data.Rows[i].Delete();
                        }
                    }

                    data.AcceptChanges(); // Finalize changes

                }
            }
            catch (OleDbException ex)
            {
                // Log the exception details
                Console.WriteLine($"OLE DB Error: {ex.Message}");
                File.AppendAllText(path2, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + ex.Message + "exception ole db");
                message = ex.Message;
            }
            catch (Exception ex)
            {
                // Log any other exception details
                Console.WriteLine($"General Error: {ex.Message}");
                message = ex.Message;
            }
            File.AppendAllText(path2, Environment.NewLine + JsonConvert.SerializeObject(DateTime.Now) + data + "Returning datatable");
            return data;
        }

        private void Exporting_func()
        {
            bool isValidFromDate = DateTime.TryParseExact(dtFrom.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime fromDate);
            bool isValidToDate = DateTime.TryParseExact(dtTO.Text.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime toDate);

            if (!isValidFromDate)
            {
                FromDateValidator.ErrorMessage = "Enter valid from date in dd/mm/yyy format";
                FromDateValidator.IsValid = false;
                return;
            }
            if (!isValidToDate)
            {
                ToDateValidator.ErrorMessage = "Enter valid to date in dd/mm/yyy format";
                ToDateValidator.IsValid = false;
                return;
            }

            if (!(fromDate < toDate))
            {
                lblMessage.Text = "From date should be earlier than to date.";
                return;
            }

            OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
            if (ConnectionState.Closed == connection.State) { connection.Open(); }
            using (OracleCommand command = new OracleCommand("PRC_CALULATE_FOLIO_TRAIL", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("PFROMDATE", OracleDbType.Date).Value = fromDate;
                command.Parameters.Add("PTODATE", OracleDbType.Date).Value = toDate;
                command.Parameters.Add("PUSERID", OracleDbType.Varchar2).Value = Session["LoginId"].ToString();
                command.Parameters.Add("PROLE_ID", OracleDbType.Int32).Value = Convert.ToInt32(Session["RoleId"]);
                command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, 100).Direction = ParameterDirection.Output;

                using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    adapter.Fill(dt);
                    ExportToCsv(dt);
                }
            }

        }
        public void ExportToCsv(System.Data.DataTable dt)
        {
            // Start building the XML for the Excel file
            StringBuilder excelXml = new StringBuilder();

            // XML headers and workbook declaration
            excelXml.AppendLine("<?xml version=\"1.0\"?>");
            excelXml.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
            excelXml.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            excelXml.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            excelXml.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");

            // Start worksheet
            excelXml.AppendLine("<Worksheet ss:Name=\"Sheet1\">");
            excelXml.AppendLine("<Table>");

            // Add the header row
            excelXml.AppendLine("<Row>");
            foreach (DataColumn column in dt.Columns)
            {
                excelXml.AppendLine($"<Cell><Data ss:Type=\"String\">{column.ColumnName}</Data></Cell>");
            }
            excelXml.AppendLine("</Row>");

            // Add the data rows
            foreach (DataRow row in dt.Rows)
            {
                excelXml.AppendLine("<Row>");
                foreach (DataColumn column in dt.Columns)
                {
                    excelXml.AppendLine($"<Cell><Data ss:Type=\"String\">{row[column].ToString()}</Data></Cell>");
                }
                excelXml.AppendLine("</Row>");
            }

            // Close table and worksheet
            excelXml.AppendLine("</Table>");
            excelXml.AppendLine("</Worksheet>");

            // Close workbook
            excelXml.AppendLine("</Workbook>");

            // Convert to bytes
            byte[] excelBytes = Encoding.UTF8.GetBytes(excelXml.ToString());

            // Send the file to the client
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=Trail_file.xls");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"; // Excel 2003 XML format
            HttpContext.Current.Response.OutputStream.Write(excelBytes, 0, excelBytes.Length);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();




        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            Exporting_func();
            return;
            //if (dtFrom.Text != "" & dtTO.Text != "")
            //{
            //    Exporting_func();
            //}
            //else
            //{
            //    Status_uploader.Text = "Please Import the file first";
            //}

        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            dtTO.Text = string.Empty;
            dtFrom.Text = string.Empty;
            Status_uploader.Text = string.Empty;


        }


    }
}