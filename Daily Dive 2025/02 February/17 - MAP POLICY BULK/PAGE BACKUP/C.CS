using System;
using System.Data;
using System.IO;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Client;



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

        bool impStatus = false;
       
        public string ImportExcelToDatabase(DataTable excelData, string selectedSheet)
        {
            string confirmationMessage = string.Empty;
            int countInserted = 0;
            try
            {
                // Step 1: Read data from Excel file
                if (excelData == null || excelData.Rows.Count == 0)
                {
                    return "No data found in the selected sheet.";
                }

                // Step 2: Insert data into the Oracle database
                using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    con.Open();

                    string tableClean = CleanTable("POLICY_MAP_TEMP1");

                    // Insert the Excel data row by row
                    foreach (DataRow row in excelData.Rows)
                    {
                        // Check if first column is not empty
                        if (!string.IsNullOrWhiteSpace(row[0].ToString()))
                        {
                            using (OracleCommand cmd = new OracleCommand("PSM_MPN_InsertExcelPolicyData", con))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                string policyNo = row[0].ToString().Replace("-", "");
                                cmd.Parameters.Add("p_policy_no", OracleDbType.Varchar2).Value = policyNo;
                                if (excelData.Columns.Count > 1 && !string.IsNullOrWhiteSpace(row[1].ToString()))
                                {
                                    string companyCode = row[1].ToString();
                                    cmd.Parameters.Add("p_column2", OracleDbType.Varchar2).Value = companyCode;
                                }
                                else
                                {
                                    cmd.Parameters.Add("p_column2", OracleDbType.Varchar2).Value = DBNull.Value;

                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        countInserted += 1;
                    }
                    confirmationMessage = + countInserted + " rows inserted successfully!";
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

        #region GetPolicyData for export
        public DataTable GetPolicyDataForExport(string companyCd)
        {
            DataTable dtPolicyData = new DataTable();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("psm_mpn_export_data", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding the input parameter for company_cd
                        cmd.Parameters.Add("p_company_cd", OracleDbType.Varchar2).Value = companyCd;

                        // Adding the OUT parameter for the cursor
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Execute the command and fill the DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtPolicyData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error message
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return dtPolicyData;
        }
        #endregion

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

        #region CleanTable
        public string CleanTable(string tableName)
        {
            string resultMessage = string.Empty;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("psm_clean_table", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter for the table name
                        cmd.Parameters.Add("p_table_name", OracleDbType.Varchar2).Value = tableName;

                        // Output parameter for the message
                        cmd.Parameters.Add("p_message", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

                        // Execute the command
                        cmd.ExecuteNonQuery();

                        // Retrieve the result message from the output parameter
                        resultMessage = cmd.Parameters["p_message"].Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Log the error message
                    Console.WriteLine("Error: " + ex.Message);
                    resultMessage = "Error: " + ex.Message;
                }
            }

            return resultMessage;
        }
        #endregion

    }
}
