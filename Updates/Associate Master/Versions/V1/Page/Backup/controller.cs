using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WM.Models;
using System.Web.Configuration;
using System.Web.ModelBinding;
using Oracle.ManagedDataAccess.Types;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using System.Web.UI.WebControls;
using NPOI.SS.Formula.Functions;
using System.Globalization;




namespace WM.Controllers
{
    public class AccountOpeningController
    {
        OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);



        #region ValidateAndInsertInvestor
        public DataTable ValidateAndInsertInvestor(
            string existClientCode, string mobile, string pan, string email, string gpan, string aadharValue)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_MM_Valid_Insert", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input Parameters
                        command.Parameters.Add("p_exist_client_code", OracleDbType.Varchar2).Value = existClientCode;
                        command.Parameters.Add("p_mobile", OracleDbType.Varchar2).Value = mobile;
                        command.Parameters.Add("p_pan", OracleDbType.Varchar2).Value = pan;
                        command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email;
                        command.Parameters.Add("p_gpan", OracleDbType.Varchar2).Value = gpan;
                        command.Parameters.Add("p_aadhar_value", OracleDbType.Varchar2).Value = aadharValue;

                        // Output Cursor Parameter
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_result",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion


        #region GetAccountCategoryList
        public DataTable GetAccountCategoryList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_GET_AC_CAT", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion


        #region GetGuestValidationData
        public DataTable GetClientDetailsByGuest(string guestCode, string loggedInUser)
        {
            string message = string.Empty; // Variable to hold the output message
            DataTable clientData = new DataTable(); // DataTable to store the result set

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_CLIENT_BY_GUEST", conn)) // Stored procedure name
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("p_guest_cd", OracleDbType.Varchar2).Value = guestCode;
                    cmd.Parameters.Add("p_login_id", OracleDbType.Varchar2).Value = loggedInUser;

                    // Output parameter (RefCursor)
                    OracleParameter refCursorParam = new OracleParameter("o_cursor", OracleDbType.RefCursor);
                    refCursorParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(refCursorParam);

                    try
                    {
                        conn.Open(); // Open the connection to the database
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            // Fill the DataTable with the result from the cursor
                            da.Fill(clientData);
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur
                        message = "Error: " + ex.Message;
                    }
                }
            }

            // You can return the message or the DataTable depending on the requirement
            // Here we return the message, but if needed you could return clientData instead
            return clientData;
        }


        public DataTable GetGuestValidationData(string guestCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GUESTVALIDAIOTN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter (Guest Code)
                    cmd.Parameters.Add("P_GUEST_CODE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(guestCode) ? (object)DBNull.Value : guestCode;

                    // Add output parameters (Business Code and Message)
                    cmd.Parameters.Add("P_BUSI_CODE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the output values
                        string businessCode = cmd.Parameters["P_BUSI_CODE"].Value.ToString();
                        string message = cmd.Parameters["P_MESSAGE"].Value.ToString();

                        // Split the businessCode string by comma
                        string[] businessDetails = businessCode.Split(new string[] { ", " }, StringSplitOptions.None);

                        // Ensure that there are enough parts in the split string
                        string businessCodeValue = businessDetails.Length > 0 ? businessDetails[0] : string.Empty;
                        string pitchBookNo = businessDetails.Length > 1 ? businessDetails[1] : string.Empty;
                        string guestCodeNo = businessDetails.Length > 2 ? businessDetails[2] : string.Empty;
                        string guestName = businessDetails.Length > 3 ? businessDetails[3] : string.Empty;
                        string mobile = businessDetails.Length > 4 ? businessDetails[4] : string.Empty;
                        string telephone = businessDetails.Length > 5 ? businessDetails[5] : string.Empty;

                        // Add columns to the DataTable
                        dt.Columns.Add("BusinessCode", typeof(string));
                        dt.Columns.Add("PitchBookNo", typeof(string));
                        dt.Columns.Add("GuestCode", typeof(string));
                        dt.Columns.Add("GuestName", typeof(string));
                        dt.Columns.Add("Mobile", typeof(string));
                        dt.Columns.Add("Telephone", typeof(string));
                        dt.Columns.Add("Message", typeof(string));

                        // Add the result row to the DataTable
                        dt.Rows.Add(businessCodeValue, pitchBookNo, guestCodeNo, guestName, mobile, telephone, message);

                    }
                    catch (OracleException ex)
                    {
                        // Handle exceptions by setting the message accordingly
                        Console.WriteLine("Error: " + ex.Message);
                        dt.Columns.Add("Message", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
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

            return dt;
        }
       
        
        #endregion




        public DataTable GetInvestorHeadOrMember(string invCode)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("psm_ao_inv_check_head", conn)) // Stored procedure name
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_inv_code", OracleDbType.Varchar2, 20).Value = invCode ?? (object)DBNull.Value;

                    // Output parameter for the cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public string[] GlbloginidHardCoded()
        {
            // List to store the Glbloginid and GlbroleId values
            List<string> values = new List<string>();

            // Add hardcoded values for Glbloginid and GlbroleId
            values.Add("38387");
            values.Add("101877");
            values.Add("97120");
            values.Add("43452");
            values.Add("38423");
            values.Add("46183");
            values.Add("97117");
            values.Add("212");
            values.Add("242");

            // Convert the list to an array and return it
            return values.ToArray();
        }

        public string[] GetBranchByLoginId(string loginId)
        {
            List<string> branchIds = new List<string>(); // List to store branch IDs

            if (!string.IsNullOrWhiteSpace(loginId))
            {
                // Query to fetch Branch ID based on Login ID
                string query = $"SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID = '{loginId}'";

                DataTable result = ExecuteQuery(query); // Call ExecuteQuery to fetch data

                // If data is returned, add the branch IDs to the list
                foreach (DataRow row in result.Rows)
                {
                    branchIds.Add(row["BRANCH_ID"].ToString()); // Add branch ID to list
                }
            }

            return branchIds.ToArray(); // Convert the list to an array and return
        }

        public DataTable Getbycientcodepan(string searchOneClientCode, string searchOnePan, string existingClientCode, string loginid)
        {
            DataTable result = new DataTable();

            string SRmCode = "";
            string[] Branches = GetBranchByLoginId(loginid);  // Get branches for the logged-in user
            string[] Glbloginid = GlbloginidHardCoded();  // Get the hardcoded Glbloginid values

            if (!string.IsNullOrWhiteSpace(searchOnePan))
            {
                string query = string.Empty;

                if (string.IsNullOrWhiteSpace(SRmCode))
                {
                    // If SRmCode is empty, query based on Branches
                    query = $"SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN (SELECT MAIN_CODE FROM client_test WHERE client_pan = '{searchOnePan}' AND BRANCH_CODE IN ({string.Join(",", Branches)}) UNION ALL SELECT MAIN_CODE FROM client_test WHERE client_pan = '{searchOnePan}' AND BUSINESS_CODE = '95829') AND CLIENT_CODE = MAIN_CODE";
                }
                else
                {
                    // If SRmCode is not empty, query based on Glbloginid and Branches
                    query = $"SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN (SELECT MAIN_CODE FROM client_test WHERE client_pan = '{searchOnePan}' AND BRANCH_CODE IN ({string.Join(",", Branches)}) AND BUSINESS_CODE IN ({string.Join(",", Glbloginid)}) UNION ALL SELECT MAIN_CODE FROM client_test WHERE client_pan = '{searchOnePan}' AND BUSINESS_CODE = '95829') AND CLIENT_CODE = MAIN_CODE";
                }

                result = ExecuteQuery(query); // Execute the query
            }
            else if (!string.IsNullOrWhiteSpace(searchOneClientCode))
            {
                // Query based on Client Code
                string query = $"SELECT * FROM CLIENT_TEST WHERE MAIN_CODE IN (SELECT MAIN_CODE FROM client_test WHERE client_code = '{searchOneClientCode.Trim()}') AND CLIENT_CODE = MAIN_CODE";
                result = ExecuteQuery(query);
            }
            else if (!string.IsNullOrWhiteSpace(existingClientCode))
            {
                // Query based on Existing Client Code
                string query = $@"
        SELECT * 
        FROM CLIENT_TEST 
        WHERE MAIN_CODE IN (
            SELECT MAIN_CODE 
            FROM CLIENT_TEST 
            WHERE source_code = SUBSTR('{existingClientCode.Trim()}', 1, 8)
        ) 
        AND CLIENT_CODE = MAIN_CODE";
                result = ExecuteQuery(query);
            }

            return result;  // Return the result
        }


        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the result of the query
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Handle exceptions and log them
                        Console.WriteLine("Error: " + ex.Message);
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
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

            return dt;
        }


        public DataTable GetFamilyGridDataBySourceCode(string sourceCode, string exist)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_MM_Existing_Inv_Grid", conn)) // Change to your stored procedure name
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_source_code", OracleDbType.Varchar2, 8).Value = sourceCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_exist", OracleDbType.Varchar2, 11).Value = exist ?? (object)DBNull.Value;

                    cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        #region GetInvestorList
        public DataTable GetInvestorList(string sourceId, string excludedInvestorCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_MM_Existing_Inv_List", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the input parameter for source_id
                    cmd.Parameters.Add("p_source_id", OracleDbType.Varchar2, 8).Value = sourceId ?? (object)DBNull.Value;

                    // Add the input parameter for excluded inv_code
                    cmd.Parameters.Add("p_excluded_inv", OracleDbType.Varchar2, 13).Value = excludedInvestorCode ?? (object)DBNull.Value;

                    // Add the output parameter for the cursor
                    cmd.Parameters.Add("p_investor_list", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the data returned from the cursor
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion


       


        #region InvestorGetCityList

        public DataTable InvestorGetCityList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_GET_CITY_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }

        #endregion


        #region GetBranchListByLogin
        public DataTable GetBranchListByLogin(string loginId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_BRANCHBYLOGIN", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Adding input parameter for LOGIN_ID
                        OracleParameter loginIdParam = new OracleParameter
                        {
                            ParameterName = "P_LOGIN_ID",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = loginId,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(loginIdParam);

                        // Adding output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_result",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion



        public DataTable SearchInvestors(
            int? branch, int? rm, string dob, int? clientCode, string clientName,
            long? mobile, string city, string phone, string accountCode,
            string pan, string investorName, string address1, string address2)
        {
            DataTable resultTable = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_AO_SEARCH_INVESTORS", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Pass all parameters to the stored procedure
                        cmd.Parameters.Add("P_BRANCH", OracleDbType.Int32).Value = branch ?? (object)DBNull.Value;
                        cmd.Parameters.Add("P_RM", OracleDbType.Int32).Value = rm ?? (object)DBNull.Value;
                        cmd.Parameters.Add("P_DOB", OracleDbType.Varchar2).Value = dob ?? (object)DBNull.Value;
                        cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Int32).Value = clientCode ?? (object)DBNull.Value;
                        cmd.Parameters.Add("P_CLIENT_NAME", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(clientName) ? (object)DBNull.Value : clientName.Trim();
                        cmd.Parameters.Add("P_MOBILE", OracleDbType.Int64).Value = mobile ?? (object)DBNull.Value;
                        cmd.Parameters.Add("P_CITY", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(city) ? (object)DBNull.Value : city.Trim();
                        cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim();
                        cmd.Parameters.Add("P_ACCOUNT_CODE", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(accountCode) ? (object)DBNull.Value : accountCode.Trim();
                        cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(pan) ? (object)DBNull.Value : pan.Trim();
                        cmd.Parameters.Add("P_INVESTOR_NAME", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(investorName) ? (object)DBNull.Value : investorName.Trim();
                        cmd.Parameters.Add("P_ADDRESS1", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(address1) ? (object)DBNull.Value : address1.Trim();
                        cmd.Parameters.Add("P_ADDRESS2", OracleDbType.Varchar2).Value = string.IsNullOrWhiteSpace(address2) ? (object)DBNull.Value : address2.Trim();

                        // Output parameter for the result cursor
                        cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Execute and fill the result table
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(resultTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or rethrow as needed
                throw new Exception("Error while searching investors: " + ex.Message);
            }

            return resultTable;
        }



        #region GetRMBySource
        public DataTable GetRMBySource(string source)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_RM_BY_SRC", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter (Source)
                    cmd.Parameters.Add("P_SRC", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(source) ? (object)DBNull.Value : source;

                    // Add output parameter for the ref cursor
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            // Prepare the DataTable schema
                            dt.Columns.Add("RM_CODE", typeof(string));
                            dt.Columns.Add("RM_NAME", typeof(string));

                            // Read the data from the cursor and load it into the DataTable
                            while (reader.Read())
                            {
                                dt.Rows.Add(reader["RM_CODE"], reader["RM_NAME"]);
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Handle exceptions and set an error message
                        Console.WriteLine("Error: " + ex.Message);
                        dt.Columns.Add("Message", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);
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

            return dt;
        }
        #endregion


    

        #region GetMutualPlanType
        public DataTable GetMutualPlanType()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_get_mutual_fund_data", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Output parameter for cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "cur_result",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion






        public string ApproveClient(string clientCode, string loggedInUser)
        {
            string message = string.Empty; // Variable to hold the output message

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_ApproveClient", conn)) // Your stored procedure name
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("p_client_code", OracleDbType.Varchar2).Value = clientCode;
                    cmd.Parameters.Add("p_loggedin_user", OracleDbType.Varchar2).Value = loggedInUser;

                    // Output parameter
                    cmd.Parameters.Add("o_message", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open(); // Open the database connection
                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the output message
                        message = cmd.Parameters["o_message"].Value.ToString();
                    }
                    catch (Exception ex)
                    {
                        // Handle exception, log it if necessary
                        message = "Error: " + ex.Message;
                    }
                }
            }

            return message; // Return the output message
        }




        public DataTable GetFamilyGridDataByMainCode(string code)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_FAMILY_GRIDBYMAIN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_main_code", OracleDbType.Varchar2).Value = code;
                    cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }





        #region GetEmployee
        public DataTable GetEmployee(string payrollID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PASM_EMP_M_GET_EMPLOYEE_BY_PAYROLL_ID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_payroll_id", OracleDbType.Varchar2).Value = payrollID;
                    cmd.Parameters.Add("p_employee_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    Console.WriteLine(payrollID);

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion



        #region GetRMEmployee
        public DataTable GetRMEmployee(string payrollId)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_RM_BY_BUSSCODE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_payroll_id", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(payrollId) ? (object)DBNull.Value : payrollId;
                    cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the result from the RefCursor
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Database error: " + ex.Message); // Log the error
                                                                            // Optionally, rethrow or handle as needed
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unexpected error: " + ex.Message); // Log the error
                                                                              // Optionally, rethrow or handle as needed
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close(); // Ensure the connection is closed
                        }
                    }
                }
            }

            return dt;
        }
        #endregion


        #region INSERT CLIENT DATA

        public string InsertClientData(
            string dtNumberValue,
            long existClientCodeValue,
            string businessCodeValue,

              string taxStatus,
           int occupationValue,
           string statusCategoryValue,
           string clientCategoryValue,
           int accountCategoryValue,
           string selectedSalutation1,
     string accountName,

      string selectedSalutation2,
           string accountFatherName,
           string accountOtherValue,
           string gender,
           string maritalStatus,
           string nationality,
           string residentNri,
           DateTime? dob,
           string annualIncome,
           string clientPan,
           string leadType,
           string guardianPerson,
           string guardianPersonNationality,
           string guardianPersonPan,
           string mailingAddress1,
           string mailingAddress2,
           string mailingState,
           string mailingCity,
           string mailingPinCode,

           string permanentAddress1,
           string permanentAddress2,
           string permanentState,
           string permanentCity,
           string PermanentPinCode,
           string NRIAddress,
           string FaxValue,
           long AadharValue,
           string EmailValue,
           string EmailOfficialValue,
           string PhoneOfficeSTDValue,
           string PhoneOfficeNumberValue,
           string PhoneResSTDValue,
           string PhoneResNumberValue,
           string MobileNoValue,
           string ReferenceName1Value,
           string ReferenceName2Value,
           string ReferenceName3Value,
           string ReferenceName4Value,
           long MobileNo1Value,
           long MobileNo2Value,
           long MobileNo3Value,
           long MobileNo4Value,

     string loggedinUser,
     string guestCodeValue
 )
        {
            string result;

            long em_rm = 0;
            long em_src = 0;

            Employee employee = new Employee
            {
                PAYROLL_ID = businessCodeValue.Trim()
            };

            // Get the employee list
            DataTable dt = new EmployeeController().GetEmployee(employee);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                em_rm = Convert.ToInt64(row["RM_CODE"]);

                em_src = Convert.ToInt64(row["SOURCE"]);


            }
            else
            {
                // Handle case where no employee data is returned
                // Optionally clear the form fields or display a message
            }





            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {


                using (OracleCommand cmd = new OracleCommand("PSM_AO_INSERT_CLIENT_DATA", conn)) // Make sure the name is correct
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EXIST_CLIENT_CODE", OracleDbType.Int64).Value = existClientCodeValue;
                    cmd.Parameters.Add("P_RM_BUSINESS_CODE", OracleDbType.Varchar2).Value = businessCodeValue ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_TAX_STATUS", OracleDbType.Varchar2).Value = taxStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OCCUPATION", OracleDbType.Int32).Value = occupationValue;
                    cmd.Parameters.Add("P_STATUS_CAT", OracleDbType.Varchar2).Value = statusCategoryValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CAT", OracleDbType.Varchar2).Value = clientCategoryValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_CAT", OracleDbType.Varchar2).Value = accountCategoryValue;
                    cmd.Parameters.Add("P_SALUTATION1", OracleDbType.Varchar2).Value = selectedSalutation1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_NAME", OracleDbType.Varchar2).Value = accountName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SALUTATION2", OracleDbType.Varchar2).Value = selectedSalutation2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FATHER_NAME", OracleDbType.Varchar2).Value = accountFatherName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_OTHER", OracleDbType.Varchar2).Value = accountOtherValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GENDER", OracleDbType.Varchar2).Value = gender ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MARITAL_STATUS", OracleDbType.Varchar2).Value = maritalStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NATIONALITY", OracleDbType.Varchar2).Value = nationality ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RESIDENT_NRI", OracleDbType.Varchar2).Value = residentNri ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = dob ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ANNUAL_INCOME", OracleDbType.Varchar2).Value = annualIncome ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_PAN", OracleDbType.Varchar2).Value = clientPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LEAD_TYPE", OracleDbType.Varchar2).Value = leadType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON", OracleDbType.Varchar2).Value = guardianPerson ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON_NATIONALITY", OracleDbType.Varchar2).Value = guardianPersonNationality ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON_PAN", OracleDbType.Varchar2).Value = guardianPersonPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_ADDRESS1", OracleDbType.Varchar2).Value = mailingAddress1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_ADDRESS2", OracleDbType.Varchar2).Value = mailingAddress2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_STATE", OracleDbType.Varchar2).Value = mailingState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_CITY", OracleDbType.Varchar2).Value = mailingCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_PINCODE", OracleDbType.Varchar2).Value = mailingPinCode ?? (object)DBNull.Value; 
                    cmd.Parameters.Add("P_PERMANENT_ADDRESS1", OracleDbType.Varchar2).Value = permanentAddress1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_ADDRESS2", OracleDbType.Varchar2).Value = permanentAddress2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_STATE", OracleDbType.Varchar2).Value = permanentState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_CITY", OracleDbType.Varchar2).Value = permanentCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_PINCODE", OracleDbType.Varchar2).Value = PermanentPinCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NRI_ADDRESS", OracleDbType.Varchar2).Value = NRIAddress ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FAX_VALUE", OracleDbType.Varchar2).Value = FaxValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHAR_VALUE", OracleDbType.Varchar2).Value = (AadharValue == 0 ? (object)DBNull.Value : AadharValue);
                    cmd.Parameters.Add("P_EMAIL_VALUE", OracleDbType.Varchar2).Value = EmailValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL_OFFICIAL_VALUE", OracleDbType.Varchar2).Value = EmailOfficialValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_OFFICE_STD_VALUE", OracleDbType.Varchar2).Value = PhoneOfficeSTDValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_OFFICE_NUMBER_VALUE", OracleDbType.Int64).Value = PhoneOfficeNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_RES_STD_VALUE", OracleDbType.Varchar2).Value = PhoneResSTDValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_RES_NUMBER_VALUE", OracleDbType.Int64).Value = PhoneResNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE_NO_VALUE", OracleDbType.Int64).Value = MobileNoValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME1_VALUE", OracleDbType.Varchar2).Value = ReferenceName1Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME2_VALUE", OracleDbType.Varchar2).Value = ReferenceName2Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME3_VALUE", OracleDbType.Varchar2).Value = ReferenceName3Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME4_VALUE", OracleDbType.Varchar2).Value = ReferenceName4Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE_NO1_VALUE", OracleDbType.Int64).Value = (MobileNo1Value== 0 ? (object)DBNull.Value : MobileNo1Value );
                    cmd.Parameters.Add("P_MOBILE_NO2_VALUE", OracleDbType.Int64).Value = (MobileNo2Value== 0 ? (object)DBNull.Value : MobileNo2Value );
                    cmd.Parameters.Add("P_MOBILE_NO3_VALUE", OracleDbType.Int64).Value = (MobileNo3Value== 0 ? (object)DBNull.Value : MobileNo3Value );
                    cmd.Parameters.Add("P_MOBILE_NO4_VALUE", OracleDbType.Int64).Value = (MobileNo4Value== 0 ? (object)DBNull.Value : MobileNo4Value);

                    cmd.Parameters.Add("P_LOGGED_IN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUEST_CODE", OracleDbType.Varchar2).Value = guestCodeValue ?? (object)DBNull.Value;


                    cmd.Parameters.Add("P_EMP_RM", OracleDbType.Int64).Value = em_rm;
                    cmd.Parameters.Add("P_EMP_SRC", OracleDbType.Int64).Value = em_src;
                    // Define the output parameter
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the command

                        // Retrieve the message from the RefCursor in P_RESULT
                        using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming the message is in the first column
                                result = reader["message"].ToString();
                            }
                            else
                            {
                                result = "No message retrieved from the procedure.";
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        result = "Error inserting client data: " + ex.Message;
                    }
                    catch (Exception ex) // General exception handling
                    {
                        result = "An unexpected error occurred: " + ex.Message;
                    }

                }
            }
            return result;
        }


        #endregion


        #region UPDATE CLIENT  DATA

        public string ValidateUpdateClientData(
        #region Prameters
    string searchClientCodeValue,
    string dtNumberValue,
    string guestCodeValue,
    long existClientCodeValue,
    string businessCodeValue,
    string taxStatus,
    int occupationValue,
    string statusCategoryValue,
    string clientCategoryValue,
    int accountCategoryValue,
    string selectedSalutation1,
    string accountName,
    string selectedSalutation2,
    string accountFatherName,
    string accountOtherValue,
    string gender,
    string maritalStatus,
    string nationality,
    string residentNri,
    DateTime? dob,
    string annualIncome,
    string clientPan,
    string leadType,
    string guardianPerson,
    string guardianPersonNationality,
    string guardianPersonPan,
    string mailingAddress1,
    string mailingAddress2,
    string mailingState,
    string mailingCity,
    string mailingPinCode,

    string permanentAddress1,
    string permanentAddress2,
    string permanentState,
    string permanentCity,
    string PermanentPinCode,
    string NRIAddress,
    string FaxValue,
    long AadharValue,
    string EmailValue,
    string EmailOfficialValue,
    string PhoneOfficeSTDValue,
    string PhoneOfficeNumberValue,
    string PhoneResSTDValue,
    string PhoneResNumberValue,
    string MobileNoValue,
    string ReferenceName1Value,
    string ReferenceName2Value,
    string ReferenceName3Value,
    string ReferenceName4Value,
    long MobileNo1Value,
    long MobileNo2Value,
    long MobileNo3Value,
    long MobileNo4Value,
    string loggedinUser
        #endregion
)
        {

            #region Get Set EMP data by buss data
            string result = "";
            long em_rm = 0;
            long em_src = 0;
            Employee employee = new Employee();
            if (!string.IsNullOrEmpty(businessCodeValue))
            {
                employee = new Employee
                {
                    PAYROLL_ID = businessCodeValue.Trim()
                };
            }

            // Get the employee list
            DataTable dt = new EmployeeController().GetEmployee(employee);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                em_rm = Convert.ToInt64(row["RM_CODE"]);

                em_src = Convert.ToInt64(row["SOURCE"]);


            }
            else
            {
            }
            #endregion

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_C_VALIDATE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    #region PARAMTETER
                    cmd.Parameters.Add("P_SEARCH_CLIENT_CODE", OracleDbType.Varchar2).Value = searchClientCodeValue ?? (object)DBNull.Value; // HEAD AH

                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUEST_CODE", OracleDbType.Varchar2).Value = guestCodeValue ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_EXIST_CLIENT_CODE", OracleDbType.Int64).Value = (existClientCodeValue==0?(object)DBNull.Value: existClientCodeValue);// HEAD INV
                    cmd.Parameters.Add("P_RM_BUSINESS_CODE", OracleDbType.Varchar2).Value = businessCodeValue ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_TAX_STATUS", OracleDbType.Varchar2).Value = taxStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OCCUPATION", OracleDbType.Int32).Value = occupationValue;
                    cmd.Parameters.Add("P_STATUS_CAT", OracleDbType.Varchar2).Value = statusCategoryValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CAT", OracleDbType.Varchar2).Value = clientCategoryValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_CAT", OracleDbType.Varchar2).Value = accountCategoryValue;
                    cmd.Parameters.Add("P_SALUTATION1", OracleDbType.Varchar2).Value = selectedSalutation1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_NAME", OracleDbType.Varchar2).Value = accountName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SALUTATION2", OracleDbType.Varchar2).Value = selectedSalutation2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FATHER_NAME", OracleDbType.Varchar2).Value = accountFatherName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_OTHER", OracleDbType.Varchar2).Value = accountOtherValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GENDER", OracleDbType.Varchar2).Value = gender ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MARITAL_STATUS", OracleDbType.Varchar2).Value = maritalStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NATIONALITY", OracleDbType.Varchar2).Value = nationality ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RESIDENT_NRI", OracleDbType.Varchar2).Value = residentNri ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = dob ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ANNUAL_INCOME", OracleDbType.Varchar2).Value = annualIncome ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_PAN", OracleDbType.Varchar2).Value = clientPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LEAD_TYPE", OracleDbType.Varchar2).Value = leadType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON", OracleDbType.Varchar2).Value = guardianPerson ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON_NATIONALITY", OracleDbType.Varchar2).Value = guardianPersonNationality ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON_PAN", OracleDbType.Varchar2).Value = guardianPersonPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_ADDRESS1", OracleDbType.Varchar2).Value = mailingAddress1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_ADDRESS2", OracleDbType.Varchar2).Value = mailingAddress2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_STATE", OracleDbType.Varchar2).Value = mailingState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_CITY", OracleDbType.Varchar2).Value = mailingCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_PINCODE", OracleDbType.Int64).Value = mailingPinCode ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_PERMANENT_ADDRESS1", OracleDbType.Varchar2).Value = permanentAddress1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_ADDRESS2", OracleDbType.Varchar2).Value = permanentAddress2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_STATE", OracleDbType.Varchar2).Value = permanentState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_CITY", OracleDbType.Varchar2).Value = permanentCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_PINCODE", OracleDbType.Varchar2).Value = PermanentPinCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NRI_ADDRESS", OracleDbType.Varchar2).Value = NRIAddress ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FAX_VALUE", OracleDbType.Varchar2).Value = FaxValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHAR_VALUE", OracleDbType.Varchar2).Value = (AadharValue == 0) ? (object)DBNull.Value : AadharValue;
                    cmd.Parameters.Add("P_EMAIL_VALUE", OracleDbType.Varchar2).Value = EmailValue.ToUpper() ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL_OFFICIAL_VALUE", OracleDbType.Varchar2).Value = EmailOfficialValue.ToUpper() ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_OFFICE_STD_VALUE", OracleDbType.Varchar2).Value = PhoneOfficeSTDValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_OFFICE_NUMBER_VALUE", OracleDbType.Varchar2).Value = PhoneOfficeNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_RES_STD_VALUE", OracleDbType.Varchar2).Value = PhoneResSTDValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_RES_NUMBER_VALUE", OracleDbType.Varchar2).Value = PhoneResNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE_NO_VALUE", OracleDbType.Int64).Value = MobileNoValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME1_VALUE", OracleDbType.Varchar2).Value = ReferenceName1Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME2_VALUE", OracleDbType.Varchar2).Value = ReferenceName2Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME3_VALUE", OracleDbType.Varchar2).Value = ReferenceName3Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME4_VALUE", OracleDbType.Varchar2).Value = ReferenceName4Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE_NO1_VALUE", OracleDbType.Int64).Value = (MobileNo1Value == 0) ? (object)DBNull.Value : MobileNo1Value;
                    cmd.Parameters.Add("P_MOBILE_NO2_VALUE", OracleDbType.Int64).Value = (MobileNo2Value == 0) ? (object)DBNull.Value : MobileNo2Value;
                    cmd.Parameters.Add("P_MOBILE_NO3_VALUE", OracleDbType.Int64).Value = (MobileNo3Value == 0) ? (object)DBNull.Value : MobileNo3Value;
                    cmd.Parameters.Add("P_MOBILE_NO4_VALUE", OracleDbType.Int64).Value = (MobileNo4Value == 0) ? (object)DBNull.Value : MobileNo4Value;

                    cmd.Parameters.Add("P_LOGGED_IN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_EMP_RM", OracleDbType.Int64).Value = (em_rm == 0) ? (object)DBNull.Value : em_rm;
                    cmd.Parameters.Add("P_EMP_SRC", OracleDbType.Int64).Value = (em_src == 0) ? (object)DBNull.Value : em_src;

                    #endregion
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Retrieve the RefCursor from the output parameter
                        OracleRefCursor refCursor = (OracleRefCursor)cmd.Parameters["P_RESULT"].Value;

                        // Use GetDataReader to read the cursor
                        using (OracleDataReader reader = refCursor.GetDataReader())
                        {
                            if (reader.Read())
                            {
                                result = reader["message"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = "Error: " + ex.Message;
                    }
                    finally
                    {
                        conn.Close();
                    }


                }
            }
            return result;
        }
        
        
        
        public string UpdateClientData(
        #region Prameters
            string searchClientCodeValue,
            string dtNumberValue,
            string guestCodeValue,
            long existClientCodeValue,
            string businessCodeValue,
            string taxStatus,
            int occupationValue,
            string statusCategoryValue,
            string clientCategoryValue,
            int accountCategoryValue,
            string selectedSalutation1,
            string accountName,
            string selectedSalutation2,
            string accountFatherName,
            string accountOtherValue,
            string gender,
            string maritalStatus,
            string nationality,
            string residentNri,
            DateTime? dob,
            string annualIncome,
            string clientPan,
            string leadType,
            string guardianPerson,
            string guardianPersonNationality,
            string guardianPersonPan,
            string mailingAddress1,
            string mailingAddress2,
            string mailingState,
            string mailingCity,
            string mailingPinCode,

            string permanentAddress1,
            string permanentAddress2,
            string permanentState,
            string permanentCity,
            string PermanentPinCode,
            string NRIAddress,
            string FaxValue,
            long AadharValue,
            string EmailValue,
            string EmailOfficialValue,
            string PhoneOfficeSTDValue,
            string PhoneOfficeNumberValue,
            string PhoneResSTDValue,
            string PhoneResNumberValue,
            string MobileNoValue,
            string ReferenceName1Value,
            string ReferenceName2Value,
            string ReferenceName3Value,
            string ReferenceName4Value,
            long MobileNo1Value,
            long MobileNo2Value,
            long MobileNo3Value,
            long MobileNo4Value,
            string loggedinUser
        #endregion
       )
        {

            #region Get Set EMP data by buss data
            string result = "";
            long em_rm = 0;
            long em_src = 0;
            Employee employee = new Employee();
            if (!string.IsNullOrEmpty(businessCodeValue))
            {
                employee = new Employee
                {
                    PAYROLL_ID = businessCodeValue.Trim()
                };
            }

            // Get the employee list
            DataTable dt = new EmployeeController().GetEmployee(employee);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                em_rm = Convert.ToInt64(row["RM_CODE"]);

                em_src = Convert.ToInt64(row["SOURCE"]);


            }
            else
            {
            }
            #endregion

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_UPDATE_CLIENT_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    #region PARAMTETER
                    cmd.Parameters.Add("P_SEARCH_CLIENT_CODE", OracleDbType.Varchar2).Value = searchClientCodeValue ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUEST_CODE", OracleDbType.Varchar2).Value = guestCodeValue ?? (object)DBNull.Value;

                    
                    cmd.Parameters.Add("P_EXIST_CLIENT_CODE", OracleDbType.Int64).Value = existClientCodeValue;
                    cmd.Parameters.Add("P_RM_BUSINESS_CODE", OracleDbType.Varchar2).Value = businessCodeValue ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_TAX_STATUS", OracleDbType.Varchar2).Value = taxStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OCCUPATION", OracleDbType.Int32).Value = occupationValue;
                    cmd.Parameters.Add("P_STATUS_CAT", OracleDbType.Varchar2).Value = statusCategoryValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CAT", OracleDbType.Varchar2).Value = clientCategoryValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_CAT", OracleDbType.Varchar2).Value = accountCategoryValue;
                    cmd.Parameters.Add("P_SALUTATION1", OracleDbType.Varchar2).Value = selectedSalutation1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_NAME", OracleDbType.Varchar2).Value = accountName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SALUTATION2", OracleDbType.Varchar2).Value = selectedSalutation2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FATHER_NAME", OracleDbType.Varchar2).Value = accountFatherName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_OTHER", OracleDbType.Varchar2).Value = accountOtherValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GENDER", OracleDbType.Varchar2).Value = gender ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MARITAL_STATUS", OracleDbType.Varchar2).Value = maritalStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NATIONALITY", OracleDbType.Varchar2).Value = nationality ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RESIDENT_NRI", OracleDbType.Varchar2).Value = residentNri ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = dob ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ANNUAL_INCOME", OracleDbType.Varchar2).Value = annualIncome ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_PAN", OracleDbType.Varchar2).Value = clientPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LEAD_TYPE", OracleDbType.Varchar2).Value = leadType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON", OracleDbType.Varchar2).Value = guardianPerson ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON_NATIONALITY", OracleDbType.Varchar2).Value = guardianPersonNationality ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GUARDIAN_PERSON_PAN", OracleDbType.Varchar2).Value = guardianPersonPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_ADDRESS1", OracleDbType.Varchar2).Value = mailingAddress1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_ADDRESS2", OracleDbType.Varchar2).Value = mailingAddress2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_STATE", OracleDbType.Varchar2).Value = mailingState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_CITY", OracleDbType.Varchar2).Value = mailingCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MAILING_PINCODE", OracleDbType.Int64).Value = mailingPinCode ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_PERMANENT_ADDRESS1", OracleDbType.Varchar2).Value = permanentAddress1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_ADDRESS2", OracleDbType.Varchar2).Value = permanentAddress2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_STATE", OracleDbType.Varchar2).Value = permanentState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_CITY", OracleDbType.Varchar2).Value = permanentCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PERMANENT_PINCODE", OracleDbType.Varchar2).Value = PermanentPinCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NRI_ADDRESS", OracleDbType.Varchar2).Value = NRIAddress ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FAX_VALUE", OracleDbType.Varchar2).Value = FaxValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHAR_VALUE", OracleDbType.Varchar2).Value = (AadharValue == 0) ? (object)DBNull.Value : AadharValue;
                    cmd.Parameters.Add("P_EMAIL_VALUE", OracleDbType.Varchar2).Value = EmailValue.ToUpper() ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL_OFFICIAL_VALUE", OracleDbType.Varchar2).Value = EmailOfficialValue.ToUpper() ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_OFFICE_STD_VALUE", OracleDbType.Varchar2).Value = PhoneOfficeSTDValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_OFFICE_NUMBER_VALUE", OracleDbType.Varchar2).Value = PhoneOfficeNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_RES_STD_VALUE", OracleDbType.Varchar2).Value = PhoneResSTDValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE_RES_NUMBER_VALUE", OracleDbType.Varchar2).Value = PhoneResNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE_NO_VALUE", OracleDbType.Int64).Value = MobileNoValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME1_VALUE", OracleDbType.Varchar2).Value = ReferenceName1Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME2_VALUE", OracleDbType.Varchar2).Value = ReferenceName2Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME3_VALUE", OracleDbType.Varchar2).Value = ReferenceName3Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REFERENCE_NAME4_VALUE", OracleDbType.Varchar2).Value = ReferenceName4Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE_NO1_VALUE", OracleDbType.Int64).Value = ( MobileNo1Value == 0) ? (object)DBNull.Value : MobileNo1Value;
                    cmd.Parameters.Add("P_MOBILE_NO2_VALUE", OracleDbType.Int64).Value = ( MobileNo2Value == 0) ? (object)DBNull.Value : MobileNo2Value;
                    cmd.Parameters.Add("P_MOBILE_NO3_VALUE", OracleDbType.Int64).Value = ( MobileNo3Value == 0) ? (object)DBNull.Value : MobileNo3Value;
                    cmd.Parameters.Add("P_MOBILE_NO4_VALUE", OracleDbType.Int64).Value = ( MobileNo4Value == 0) ? (object)DBNull.Value : MobileNo4Value;

                    cmd.Parameters.Add("P_LOGGED_IN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_EMP_RM", OracleDbType.Int64).Value = (em_rm == 0) ? (object)DBNull.Value : em_rm;
                    cmd.Parameters.Add("P_EMP_SRC", OracleDbType.Int64).Value = (em_src == 0) ? (object)DBNull.Value : em_src;

                    #endregion
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Retrieve the RefCursor from the output parameter
                        OracleRefCursor refCursor = (OracleRefCursor)cmd.Parameters["P_RESULT"].Value;

                        // Use GetDataReader to read the cursor
                        using (OracleDataReader reader = refCursor.GetDataReader())
                        {
                            if (reader.Read())
                            {
                                result = reader["message"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = "Error: " + ex.Message;
                    }
                    finally
                    {
                        conn.Close();
                    }


                }
            }
            return result;
        }


        #endregion

        #region GetStatesByCountry
        public DataTable GetStatesByCountry(int countryID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GetStatesByCountry", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter for country ID
                    cmd.Parameters.Add("p_country_id", OracleDbType.Int32).Value = countryID;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_states_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion

        #region GetCitiesByState
        public DataTable GetCitiesByState(int stateID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GetCitiesByState", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter for state ID
                    cmd.Parameters.Add("p_state_id", OracleDbType.Int32).Value = stateID;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_cities_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion




        #region GetCountriesByState
        public DataTable GetCountriesByState(int stateID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GetCountriesByState", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter for state ID
                    cmd.Parameters.Add("p_state_id", OracleDbType.Int32).Value = stateID;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_countries_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion


        #region GetBankMasterDetails
        public DataTable GetBankMasterDetails()
        {
            DataTable dtBankMasterDetails = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_BANKMASTERALL", con))
                    {
                        OracleDataAdapter da;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(cmd);
                        da.Fill(dtBankMasterDetails);
                    }
                }
                catch (Exception ex)
                {
                    // ErrorHandler.ErrorLog("Bank Master Details", ex);
                    // Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtBankMasterDetails;
        }
        #endregion




        #region PSMGetClientList

        #region PSMGetClientList
        public DataTable PSMGetClientList(string cityId, string clientCode, string name, string mobile, string phone, string branchId, string businessCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_CLIENTLIST", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("P_BRANCH_ID", OracleDbType.Varchar2).Value = branchId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = mobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CITY_ID", OracleDbType.Varchar2).Value = cityId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = phone ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Varchar2).Value = clientCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BUSINESS_CD", OracleDbType.Varchar2).Value = businessCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_NAME", OracleDbType.Varchar2).Value = name ?? (object)DBNull.Value;

                    // Cursor to open procedure
                    cmd.Parameters.Add("P_CLIENT_LIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion



        #endregion

        #region GetClientData
        public DataTable GetClientDataByIDPan(string clientCode, string clientPan)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_CC_BYAHPAN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(clientCode) ? (object)DBNull.Value : clientCode.ToUpper();
                    cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(clientPan) ? (object)DBNull.Value : clientPan.ToUpper();

                    // Define the output parameter as a cursor
                    cmd.Parameters.Add("OUT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }


        #endregion


        #region GetClientDataByInv
        public DataTable GetClientDataByInv(string invCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_CLIENT_BY_INV", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("P_INV_CODE", OracleDbType.Varchar2).Value = invCode;
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion

        #region GetClientDataBTAHID
        public DataTable GetClientDataByID(string clientID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_CLIENT_BY_ID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Varchar2).Value = clientID;
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion



        #region GetClientDataByDTNumber
        public DataTable GetClientDataByDTNumber(string clientID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_GET_CLIENTBYDT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Varchar2).Value = clientID;
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion































        #region SearchFamilyGrid
        public DataTable SearchFamilyGrid(string sourceCode)
        {
            DataTable dt = new DataTable();

            // Connection string from web.config
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                // Create the command for the stored procedure
                using (OracleCommand cmd = new OracleCommand("PSM_AO_SEARCH_FAMILY_GRID", conn))
                {
                    // Set the command type to stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the input parameter for the source code
                    cmd.Parameters.Add("P_SOURCE_CODE", OracleDbType.Varchar2).Value = sourceCode ?? (object)DBNull.Value as object;

                    // Add the output parameter for the result cursor
                    cmd.Parameters.Add("P_RESULT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        // Open the connection
                        conn.Open();

                        // Use OracleDataAdapter to fill the DataTable with the result set from the procedure
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Handle or log the exception
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        // Ensure the connection is closed
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }
        #endregion

        #region GetSalutationList
        public DataTable GetSalutationList()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_SALUTATION_LIST", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion





        #region GetOccupationList

        public DataTable GetOccupationList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_OCCUPATION_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }

        #endregion


        #region Get_ClientCategory
        public DataTable Get_ClientCategory()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_Get_ClientCategory", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion

        #region GetCountryList
        public DataTable GetCountryList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_Get_Country_List", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion





        #region GetBranchList
        public DataTable GetBranchList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_BRANCH_MASTER_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_result",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion

        #region GetCityList
        public DataTable GetCityList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_CITY_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion

        #region GetStateList
        public DataTable GetStateList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_MASTER_STATE", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion


        #region Fetch Methods

        public DataTable CheckFormNameExists(string formIndex)
        {
            DataTable dt = null;

            using (OracleCommand cmd = new OracleCommand("USP_GET_FORMNAME_VB", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.Add("P_INDEX", OracleDbType.Varchar2).Value = formIndex;
                // Output parameters             
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                // Execute the command
                try
                {
                    OracleDataAdapter oda = new OracleDataAdapter();
                    DataSet ds = new DataSet();
                    oda.SelectCommand = cmd;
                    oda.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            return dt;
                        }
                    }

                }
                catch (OracleException ex)
                {
                    //ErrorHandler.ErrorLog("City Master", ex);
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dt;
        }
        public DataTable GetModules()
        {
            DataTable dt = null;

            using (OracleCommand cmd = new OracleCommand("USP_GET_MODULE_VB", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters

                // Output parameters             
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                // Execute the command
                try
                {
                    OracleDataAdapter oda = new OracleDataAdapter();
                    DataSet ds = new DataSet();
                    oda.SelectCommand = cmd;
                    oda.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            return dt;
                        }
                    }

                }
                catch (OracleException ex)
                {
                    //ErrorHandler.ErrorLog("City Master", ex);
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dt;
        }
        public DataTable GetSubModules(string module)
        {
            DataTable dt = null;

            using (OracleCommand cmd = new OracleCommand("USP_GET_SUBMODULE_VB", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.Add("P_MODULE", OracleDbType.Varchar2).Value = module;
                // Output parameters             
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                // Execute the command
                try
                {
                    OracleDataAdapter oda = new OracleDataAdapter();
                    DataSet ds = new DataSet();
                    oda.SelectCommand = cmd;
                    oda.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            return dt;
                        }
                    }

                }
                catch (OracleException ex)
                {
                    //ErrorHandler.ErrorLog("City Master", ex);
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dt;
        }
        public DataTable GetFormDescription(string formIndex)
        {
            DataTable dt = null;

            using (OracleCommand cmd = new OracleCommand("USP_GET_FORM_DESC_VB", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.Add("P_INDEX", OracleDbType.Varchar2).Value = formIndex;
                // Output parameters             
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                // Execute the command
                try
                {
                    OracleDataAdapter oda = new OracleDataAdapter();
                    DataSet ds = new DataSet();
                    oda.SelectCommand = cmd;
                    oda.Fill(ds);
                    if (ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            return dt;
                        }
                    }

                }
                catch (OracleException ex)
                {
                    //ErrorHandler.ErrorLog("City Master", ex);
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return dt;
        }


        #endregion

        #region Save Methods
        public string SaveNewForm(AddNewFormModel obj)
        {
            string result = "failure";
            DataTable dt = null;
            if (connection.State == ConnectionState.Closed) { connection.Open(); }

            using (OracleCommand cmd = new OracleCommand("USP_SAVE_FORMNAME_VB", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Input parameters
                cmd.Parameters.Add("P_MAIN", OracleDbType.Varchar2).Value = obj.Main;
                cmd.Parameters.Add("P_MODULE", OracleDbType.Varchar2).Value = obj.Module;
                cmd.Parameters.Add("P_SUBMODULE", OracleDbType.Varchar2).Value = obj.SubModule;
                cmd.Parameters.Add("P_DESC", OracleDbType.Varchar2).Value = obj.Description;
                cmd.Parameters.Add("P_SUBFORM", OracleDbType.Varchar2).Value = obj.SubForm;
                cmd.Parameters.Add("P_INDEX", OracleDbType.Varchar2).Value = obj.FormIndex;


                cmd.Parameters.Add("P_FORM_NAME", OracleDbType.Varchar2).Value = obj.FormIndex;
                cmd.Parameters.Add("P_FORM_DESC", OracleDbType.Varchar2).Value = obj.FormPath;
                cmd.Parameters.Add("P_VIEW_BUTTON", OracleDbType.Int16).Value = obj.ViewButton;
                cmd.Parameters.Add("P_ADD_BUTTON", OracleDbType.Int16).Value = obj.AddButton;
                cmd.Parameters.Add("P_UPDATE_BUTTON", OracleDbType.Int16).Value = obj.UpdateButton;
                cmd.Parameters.Add("P_PRINT_BUTTON", OracleDbType.Int16).Value = obj.PrintButton;
                cmd.Parameters.Add("P_LOGGEDUSERID", OracleDbType.Varchar2).Value = obj.LoggedUserId;


                // Output parameters
                cmd.Parameters.Add("P_ERROR_MESSAGE", OracleDbType.Char, 2000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                // Execute the command
                try
                {
                    OracleDataAdapter oda = new OracleDataAdapter();
                    DataSet ds = new DataSet();
                    oda.SelectCommand = cmd;
                    oda.Fill(ds);
                    // Access output parameters                   
                    Oracle.ManagedDataAccess.Types.OracleString msg = ((Oracle.ManagedDataAccess.Types.OracleString)cmd.Parameters["P_ERROR_MESSAGE"].Value);
                    if (msg.IsNull)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                result = "success";
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException(msg.ToString());
                    }

                    // Process output parameters as needed
                }
                catch (OracleException ex)
                {
                    //ErrorHandler.ErrorLog("City Master", ex);
                    throw;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }
        #endregion




        #region Family Details





        public DataTable GetRelaitonshipList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_GET_RELATIONSHIPLIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }


        public DataTable GetExistingFamilyList(string clientCode, int sourceCode)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AO_FAM_EXISTINGMEM", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the input parameters to the command
                        command.Parameters.Add("p_client_code", OracleDbType.Varchar2).Value = clientCode;
                        command.Parameters.Add("p_source_code", OracleDbType.Int16).Value = sourceCode;

                        // Add the output cursor parameter
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }





        private object GetFirstCharOrDBNull(string input)
        {
            string VALUE = !string.IsNullOrEmpty(input) ? input[0].ToString() : null;
            return !string.IsNullOrEmpty(VALUE) ? VALUE : (object)DBNull.Value;
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



        public string M_MULTI_VALIDATION(
        string dtNumberValue,
        long existClientCodeValue,
        string existingMemberInv,
        string businessCodeValue,
        string selectedSalutation1,
        string accountName,
        string loggedinUser,
        string clientCodeForMainValue,

        string famMobile,
        string famPan,
        string famEmail,

        DateTime? famDOB,
        int famRel,
        string famGName,
        string famGPan,
        int famOcc,
        string famKYC,
        string famApprove,
        string famGender,
        string famNom,
        double famAllo,
        long aadharValue


        )
        {
            string result;
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_M_MULTI_VALIDATION ", conn)) // Make sure the name is correct
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EXIST_CLIENT_CODE", OracleDbType.Int64).Value = (existClientCodeValue==0) ? (object)DBNull.Value: existClientCodeValue;
                    cmd.Parameters.Add("p_existing_member", OracleDbType.Varchar2).Value = existingMemberInv ?? (object)DBNull.Value;

                    
                    cmd.Parameters.Add("P_RM_BUSINESS_CODE", OracleDbType.Varchar2).Value = businessCodeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SALUTATION1", OracleDbType.Varchar2).Value = selectedSalutation1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_NAME", OracleDbType.Varchar2).Value = accountName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGGED_IN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CODE_IN_MAIN", OracleDbType.Varchar2).Value = clientCodeForMainValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = famMobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = famPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = famEmail ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = famDOB ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RELATION", OracleDbType.Int32).Value = famRel;
                    cmd.Parameters.Add("P_GNAME", OracleDbType.Varchar2).Value = famGName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GPAN", OracleDbType.Varchar2).Value = famGPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OCCUPATION", OracleDbType.Int32).Value = famOcc;
                    cmd.Parameters.Add("P_KYC", OracleDbType.Varchar2).Value = GetFirstCharOrDBNull(famKYC);
                    cmd.Parameters.Add("P_APPROVE", OracleDbType.Varchar2).Value = famApprove ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GENDER", OracleDbType.Varchar2).Value = GetFirstCharOrDBNull(famGender);
                    cmd.Parameters.Add("P_NOM", OracleDbType.Varchar2).Value = famNom ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ALLO", OracleDbType.Double).Value = (famAllo == 0) ? (object)DBNull.Value : famAllo;

                    cmd.Parameters.Add("P_AADHAR_VALUE", OracleDbType.Varchar2).Value = (aadharValue == 0) ? (object)DBNull.Value : aadharValue;









                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open(); // Open the connection

                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the result from the RefCursor
                        using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                        {
                            if (reader.Read()) // Check if there's a row in the cursor
                            {
                                result = reader.GetString(0); // Retrieve the message from the cursor
                            }
                            else
                            {
                                result = "No result returned from the procedure.";
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        result = "Error inserting client data: " + ex.Message; // Handle Oracle exceptions
                    }
                    catch (Exception ex)
                    {
                        result = "An unexpected error occurred: " + ex.Message; // Handle general exceptions
                    }
                    finally
                    {
                        conn.Close(); // Ensure the connection is closed
                    }

                }
            }
            return result;
        }

        public string InsertMemberWithMainCode(
        string dtNumberValue,
        long existClientCodeValue,
        string businessCodeValue,
        string selectedSalutation1,
        string accountName,
        string loggedinUser,
        string clientCodeForMainValue,

        string famMobile,
        string famPan,
        string famEmail,

        DateTime? famDOB,
        int famRel,
        string famGName,
        string famGPan,
        int famOcc,
        string famKYC,
        string famApprove,
        string famGender,
        string famNom,
        double famAllo,
        long aadharValue


        )
        {
            string result;
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_MM_Insert", conn)) // Make sure the name is correct
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EXIST_CLIENT_CODE", OracleDbType.Int64).Value = existClientCodeValue;
                    cmd.Parameters.Add("P_RM_BUSINESS_CODE", OracleDbType.Varchar2).Value = businessCodeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SALUTATION1", OracleDbType.Varchar2).Value = selectedSalutation1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_NAME", OracleDbType.Varchar2).Value = accountName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGGED_IN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CODE_IN_MAIN", OracleDbType.Varchar2).Value = clientCodeForMainValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = famMobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = famPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = famEmail ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = famDOB ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RELATION", OracleDbType.Int32).Value = famRel;
                    cmd.Parameters.Add("P_GNAME", OracleDbType.Varchar2).Value = famGName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GPAN", OracleDbType.Varchar2).Value = famGPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OCCUPATION", OracleDbType.Int32).Value = famOcc;
                    cmd.Parameters.Add("P_KYC", OracleDbType.Varchar2).Value = GetFirstCharOrDBNull(famKYC);
                    cmd.Parameters.Add("P_APPROVE", OracleDbType.Varchar2).Value = famApprove ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GENDER", OracleDbType.Varchar2).Value = GetFirstCharOrDBNull(famGender);
                    cmd.Parameters.Add("P_NOM", OracleDbType.Varchar2).Value = famNom ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ALLO", OracleDbType.Double).Value = ( famAllo == 0) ? (object)DBNull.Value : famAllo;

                    cmd.Parameters.Add("P_AADHAR_VALUE", OracleDbType.Varchar2).Value = (aadharValue == 0) ? (object)DBNull.Value : aadharValue;









                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open(); // Open the connection

                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the result from the RefCursor
                        using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                        {
                            if (reader.Read()) // Check if there's a row in the cursor
                            {
                                result = reader.GetString(0); // Retrieve the message from the cursor
                            }
                            else
                            {
                                result = "No result returned from the procedure.";
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        result = "Error inserting client data: " + ex.Message; // Handle Oracle exceptions
                    }
                    catch (Exception ex)
                    {
                        result = "An unexpected error occurred: " + ex.Message; // Handle general exceptions
                    }
                    finally
                    {
                        conn.Close(); // Ensure the connection is closed
                    }

                }
            }
            return result;
        }



        public string UpdateFamilyByClientCode(
        string dtNumberValue,
        long existClientCodeValue,
        string businessCodeValue,
        string selectedSalutation1,
        string accountName,
        string loggedinUser,
        string clientCodeForMainValue,

        string famMobile,
        string famPan,
        string famEmail,

        DateTime? famDOB,
        int famRel,
        string famGName,
        string famGPan,
        int famOcc,
        string famKYC,
        string famApprove,
        string famGender,
        string famNom,
        double famAllo,
        string updateByClientCode,
        long aadharValue

        )
        {
            string result;
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_MM_Update_By_Inv", conn)) // Make sure the name is correct
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EXIST_CLIENT_CODE", OracleDbType.Int64).Value = existClientCodeValue;
                    cmd.Parameters.Add("P_RM_BUSINESS_CODE", OracleDbType.Varchar2).Value = businessCodeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SALUTATION1", OracleDbType.Varchar2).Value = selectedSalutation1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCOUNT_NAME", OracleDbType.Varchar2).Value = accountName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGGED_IN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CLIENT_CODE_IN_MAIN", OracleDbType.Varchar2).Value = clientCodeForMainValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = famMobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = famPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = famEmail ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = famDOB ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RELATION", OracleDbType.Int32).Value = famRel;
                    cmd.Parameters.Add("P_GNAME", OracleDbType.Varchar2).Value = famGName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GPAN", OracleDbType.Varchar2).Value = famGPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OCCUPATION", OracleDbType.Int32).Value = famOcc;
                    cmd.Parameters.Add("P_KYC", OracleDbType.Varchar2).Value = GetFirstCharOrDBNull(famKYC);
                    cmd.Parameters.Add("P_APPROVE", OracleDbType.Varchar2).Value = famApprove ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GENDER", OracleDbType.Varchar2).Value = GetFirstCharOrDBNull(famGender);
                    cmd.Parameters.Add("P_NOM", OracleDbType.Varchar2).Value = famNom ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ALLO", OracleDbType.Double).Value = (famAllo == 0 ? (object)DBNull.Value: famAllo);
                    cmd.Parameters.Add("P_UPDATEBYCLIENTCODE", OracleDbType.Varchar2).Value = updateByClientCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHAR_VALUE", OracleDbType.Varchar2).Value = (aadharValue == 0) ? (object)DBNull.Value : aadharValue;








                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open(); // Open the connection

                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the result from the RefCursor
                        using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                        {
                            if (reader.Read()) // Check if there's a row in the cursor
                            {
                                result = reader.GetString(0); // Retrieve the message from the cursor
                            }
                            else
                            {
                                result = "No result returned from the procedure.";
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        result = "Error inserting client data: " + ex.Message; // Handle Oracle exceptions
                    }
                    catch (Exception ex)
                    {
                        result = "An unexpected error occurred: " + ex.Message; // Handle general exceptions
                    }
                    finally
                    {
                        conn.Close(); // Ensure the connection is closed
                    }

                }
            }
            return result;
        }



        #endregion


        #region 
        public DataTable GetFamilyByMainCode(
            string clientCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_MEMBERBYMAIN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_client_main_code", OracleDbType.Varchar2).Value = clientCode ?? (object)DBNull.Value;

                    // Cursor to open procedure
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }



        #region GetMemberDataByClientCode
        public DataTable GetMemberDataByClientCode(string clientCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_MM_GET_INV_TO_UP", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add the input parameter for client code
                    cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Varchar2).Value = clientCode ?? (object)DBNull.Value;

                    // Cursor to open procedure
                    cmd.Parameters.Add("P_MEMBERDATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the data returned from the cursor
                        }
                    }
                    catch (OracleException ex)
                    {
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

            return dt;
        }
        #endregion

        #endregion


        #region AdvTran

        public string InsertAdvisoryTransaction(
            int sourceId,
            int businessCode,
            string planType,
            string chequeNo,
            DateTime? chequeDate,
            string bankName,
            double amount,
            string mutCode,
            string schemeCode,
            int rmCode,
            int branchCode,
            string loggedInUser,
            string remark
        )
        {
            string result;

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_AR_INSERT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_SOURCE_ID", OracleDbType.Int32).Value = sourceId;
                    cmd.Parameters.Add("P_BUSI_CODE", OracleDbType.Int32).Value = businessCode;
                    cmd.Parameters.Add("P_PLAN_TYPE", OracleDbType.Varchar2).Value = planType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CHEQUE_NO", OracleDbType.Varchar2).Value = chequeNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CHQ_DATE", OracleDbType.Date).Value = chequeDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BANK_NAME", OracleDbType.Varchar2).Value = bankName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AMOUNT", OracleDbType.Double).Value = amount;
                    cmd.Parameters.Add("P_MUT_CODE", OracleDbType.Varchar2).Value = mutCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SCHEME_CODE", OracleDbType.Varchar2).Value = schemeCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RM_CODE", OracleDbType.Int32).Value = rmCode;
                    cmd.Parameters.Add("P_BRANCH", OracleDbType.Int32).Value = branchCode;
                    cmd.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = loggedInUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REMARK", OracleDbType.Varchar2).Value = remark ?? (object)DBNull.Value;

                    // OUT parameter to capture success/error message
                    OracleParameter outMessage = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000);
                    outMessage.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outMessage);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the message from the OUT parameter
                        result = outMessage.Value.ToString();
                    }
                    catch (OracleException ex)
                    {
                        result = "Error inserting advisory transaction: " + ex.Message;
                    }
                    catch (Exception ex)
                    {
                        result = "An unexpected error occurred: " + ex.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

            return result;
        }





        public string UpdateAdvisoryTransaction(
            string tranCode,
            string mutCode,
            string schemeCode,
            double amount,
            string chequeNo,
            DateTime? chequeDate,
            string bankName,
            string remark,
            string loggedInUser,
            bool isCash,
            bool isCheque,
            bool isDraft
        )
        {
            string result;

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_AR_UPDATE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_TRAN_CODE", OracleDbType.Varchar2).Value = tranCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MUT_CODE", OracleDbType.Varchar2).Value = mutCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SCHEME_CODE", OracleDbType.Varchar2).Value = schemeCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AMOUNT", OracleDbType.Double).Value = amount;
                    cmd.Parameters.Add("P_CHEQUE_NO", OracleDbType.Varchar2).Value = chequeNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CHEQUE_DATE", OracleDbType.Date).Value = chequeDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BANK_NAME", OracleDbType.Varchar2).Value = bankName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REMARK", OracleDbType.Varchar2).Value = remark ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGGED_USER_ID", OracleDbType.Varchar2).Value = loggedInUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_IS_CASH", OracleDbType.Boolean).Value = isCash;
                    cmd.Parameters.Add("P_IS_CHEQUE", OracleDbType.Boolean).Value = isCheque;
                    cmd.Parameters.Add("P_IS_DRAFT", OracleDbType.Boolean).Value = isDraft;

                    // OUT parameter to capture success/error message
                    OracleParameter outMessage = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 4000);
                    outMessage.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outMessage);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the stored procedure

                        // Retrieve the message from the OUT parameter
                        result = outMessage.Value.ToString();
                    }
                    catch (OracleException ex)
                    {
                        result = "Error updating advisory transaction: " + ex.Message;
                    }
                    catch (Exception ex)
                    {
                        result = "An unexpected error occurred: " + ex.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }

            return result;
        }



        // Fill AR By Client Code

        #region GetFillAdvisoryDetails

        
        public DataTable GetAdvisoryDataByAHINV(string clientCodeKYC, string clientCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_AR_BY_AHINV", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("p_client_codekyc", OracleDbType.Varchar2).Value = clientCodeKYC;
                    cmd.Parameters.Add("p_client_code", OracleDbType.Varchar2).Value = (object)clientCode ?? DBNull.Value;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the query result
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message); // Handle or log the exception
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close(); // Ensure the connection is closed
                        }
                    }
                }
            }

            return dt; // Return the filled DataTable
        }
       

        public DataTable GetAdvisoryDataByAH(string clientCode, string clientPan)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_AR_FILLBYAH", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("P_CLIENT_CODE", OracleDbType.Varchar2).Value = clientCode;
                    cmd.Parameters.Add("P_CLIENT_PAN", OracleDbType.Varchar2).Value = (object)clientPan ?? DBNull.Value;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the query result
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message); // Handle or log the exception
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close(); // Ensure the connection is closed
                        }
                    }
                }
            }

            return dt; // Return the filled DataTable
        }
        #endregion


        #region GetARReport
        public DataTable GetARReport(string MyTranCode, string MyPrintSourceId, DateTime MyTrDate, string txtbusicode, string MySourceId)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AO_AR_PRINT", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("MyTranCode", OracleDbType.Varchar2).Value = MyTranCode;
                    cmd.Parameters.Add("MyPrintSourceId", OracleDbType.Varchar2).Value = (object)MyPrintSourceId ?? DBNull.Value;
                    cmd.Parameters.Add("MyTrDate", OracleDbType.Date).Value = MyTrDate;
                    cmd.Parameters.Add("txtbusicode", OracleDbType.Varchar2).Value = txtbusicode;
                    cmd.Parameters.Add("MySourceId", OracleDbType.Varchar2).Value = MySourceId;

                    // Add output parameter for the REF CURSOR
                    cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the query result from the REF CURSOR
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message); // Handle or log the exception
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close(); // Ensure the connection is closed
                        }
                    }
                }
            }

            // Capture any changes made to MySourceId from the stored procedure
            //MySourceId = cmd.Parameters["MySourceId"].Value.ToString();

            return dt; // Return the filled DataTable
        }
        #endregion


        #endregion





    }
}