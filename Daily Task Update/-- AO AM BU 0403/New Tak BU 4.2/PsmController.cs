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
using System.Web.UI;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Office2016.Excel;


namespace WM.Controllers
{
    public class PsmController
    {
        OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);


        #region DDL DATA BINDING CONTROLS


        public DataTable get_BRANCH_RM(Models.PsmModel pm)
        {
            DataTable dtBrnachRM = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("PSM_BRANCH_RM_MASTER", con)) // Corrected procedure name
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding required parameters
                        cmd.Parameters.Add("P_REGIONID", OracleDbType.Varchar2).Value = pm.REGION_ID;
                        cmd.Parameters.Add("P_ZONE_ID", OracleDbType.Varchar2).Value = pm.ZONE_ID;
                        cmd.Parameters.Add("P_LOGINID", OracleDbType.Varchar2).Value = pm.LOGGEDUSERID;
                        cmd.Parameters.Add("P_ROLEID", OracleDbType.Varchar2).Value = pm.ROLE_ID;
                        cmd.Parameters.Add("P_BRANCH_CODE", OracleDbType.Varchar2).Value = pm.BRANCH_CODE;
                        cmd.Parameters.Add("P_RM_CODE", OracleDbType.Varchar2).Value = pm.RM_CODE;
                        cmd.Parameters.Add("P_LIST_TYPE", OracleDbType.Varchar2).Value = pm.LIST_TYPE;

                        // Output parameter for error message
                        cmd.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;

                        // Output cursor
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtBrnachRM);
                        }

                        // Fetch error message
                        string errorMessage = Convert.ToString(cmd.Parameters["P_ERRORMESSAGE"].Value);
                        //if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "null")
                        //{
                        //    throw new ArgumentException(errorMessage);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    // Log the error (optional)
                    Console.WriteLine("Error in PSM_BRANCH_RM: " + ex.Message);
                    throw;
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }

            return dtBrnachRM;
        }


        #endregion

       
        public string isException(DataTable dt)
        {
             return dt.Columns.Contains("Exception") ? dt.Rows[0]["Exception"]?.ToString() : null;

        }

        public DataTable ExecuteCurrentQuery(string query)
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

        public DataTable ExecuteCurrentQueryMaster(string query, out int rowCount, out string exception)
        {
            DataTable dt = new DataTable();
            rowCount = 0; // Default row count
            exception = null; // Default exception as null

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
                            rowCount = dt.Rows.Count; // Get the row count
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Capture exception message
                        exception = ex.Message;

                        // Create an empty DataTable with an Exception column
                        dt = new DataTable();
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
        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_SQL_RUN_QUERY", conn)) // Call the procedure
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        conn.Open();

                        // Pass the dynamic SQL query as input
                        cmd.Parameters.Add("p_query", OracleDbType.Clob).Value = query;

                        // Define the output cursor parameter
                        OracleParameter resultCursor = new OracleParameter("p_result", OracleDbType.RefCursor);
                        resultCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(resultCursor);

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader); // Load result into DataTable
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

        public DataTable LogBranchList(string loginId, string roleId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_BRANCHES", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for Login ID
                        command.Parameters.Add(new OracleParameter("P_LOGIN", OracleDbType.Varchar2)
                        {
                            Value = loginId,
                            Direction = ParameterDirection.Input
                        });

                        // Input parameter for Role ID
                        command.Parameters.Add(new OracleParameter("P_ROLEID", OracleDbType.Varchar2)
                        {
                            Value = roleId,
                            Direction = ParameterDirection.Input
                        });

                        // Output parameter for the cursor
                        command.Parameters.Add(new OracleParameter("P_BRANCHES", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        });

                        conn.Open(); // Open the connection before execution

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            // Convert DataTable to string array of branch codes
            return dt;
        }

        public string[] GetBranchCodes(string loginId, string roleId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_BRANCHES", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for Login ID
                        command.Parameters.Add(new OracleParameter("P_LOGIN", OracleDbType.Varchar2)
                        {
                            Value = loginId,
                            Direction = ParameterDirection.Input
                        });

                        // Input parameter for Role ID
                        command.Parameters.Add(new OracleParameter("P_ROLEID", OracleDbType.Varchar2)
                        {
                            Value = roleId,
                            Direction = ParameterDirection.Input
                        });

                        // Output parameter for the cursor
                        command.Parameters.Add(new OracleParameter("P_BRANCHES", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        });

                        conn.Open(); // Open the connection before execution

                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine("Error: " + ex.Message);
                return new string[0]; // Return an empty array in case of an error
            }

            // Convert DataTable to string array of branch codes
            return dt.AsEnumerable()
                     .Select(row => row["BRANCH_CODE"].ToString())
                     .ToArray();
        }

        public string LogBranches(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            string[] branchCodes = new string[0];
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                branchCodes = GetBranchCodes(currentLoginId, currentRoleId);
            }

            return string.Join(",", branchCodes);
        }


        public string psm_Branches(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                return string.Empty;
            }

            string sql = "SELECT BM.BRANCH_CODE, BM.BRANCH_NAME FROM BRANCH_MASTER BR INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID WHERE UJ.LOGIN_ID = '"+currentLoginId+"' AND UJ.ROLE_ID = '"+currentRoleId+"' ORDER BY BRANCH_NAME";

            DataTable dtBranchList = ExecuteCurrentQuery(sql);

            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("BRANCH_NAME") && dtBranchList.Columns.Contains("BRANCH_CODE"))
            {
                string[] branchCodes = dtBranchList.AsEnumerable()
                                                   .Select(row => row["BRANCH_CODE"].ToString())
                                                   .ToArray();
                return string.Join(",", branchCodes);
            }

            return string.Empty;  
        }

        public string psm_RMs(Func<string> psm_Branches = null)
        {
            // Get branches from the provided function
            string branches = psm_Branches.Invoke();

            if (string.IsNullOrEmpty(branches))
            {
                return string.Empty;
            }

            string sql = "SELECT RM_CODE, RM_NAME FROM employee_master WHERE source IN (" + branches + ")";

            DataTable dtBranchList = ExecuteCurrentQuery(sql);

            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("RM_CODE") && dtBranchList.Columns.Contains("RM_NAME"))
            {
                string[] RmCodes = dtBranchList.AsEnumerable()
                                               .Select(row => row["RM_CODE"].ToString())
                                               .ToArray();
                return string.Join(",", RmCodes);
            }

            return string.Empty;
        }



        public DataTable GetLogBranchList(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            DataTable dt = new DataTable();

            string[] branchCodes = new string[0];
            if (!string.IsNullOrEmpty(currentLoginId) && !string.IsNullOrEmpty(currentRoleId))
            {
                dt = LogBranchList(currentLoginId, currentRoleId);
            }

            return dt;
        }

        public void ShowAlert(Page page, string message)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page), "Page instance cannot be null.");
            }

            // Handle null message by using a default value
            message = message == null ? "" : message;

            // Sanitize the message to handle special characters and prevent JavaScript errors
            string sanitizedMessage = message.Replace("'", "\\'").Replace("\n", "\\n");

            // Check if the request is from an UpdatePanel (Partial PostBack)
            if (ScriptManager.GetCurrent(page)?.IsInAsyncPostBack == true)
            {
                // Use ScriptManager for UpdatePanel partial postbacks
                ScriptManager.RegisterStartupScript(
                    page,
                    page.GetType(),
                    "alert",
                    $"alert('{sanitizedMessage}');",
                    true
                );
            }
            else
            {
                // Use ClientScript for regular full postbacks
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "alert",
                    $"alert('{sanitizedMessage}');",
                    true
                );
            }
        }


        private void ClearTextBoxes(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Text = string.Empty;
                }
                else if (c.HasControls())
                {
                    ClearTextBoxes(c);
                }
            }
        }

        public string RowFoundMsg(DataTable dt)
        {
            string gettingRowsCount = dt.Rows.Count.ToString();
            string recordText = dt.Rows.Count == 1 ? "record" : "records";
            return gettingRowsCount + " " + recordText + " found!";
        }


        public string SafeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return HttpUtility.HtmlEncode(input);
        }

        public string SafeJavaScript(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return HttpUtility.JavaScriptStringEncode(input);
        }


        
        // redirect to welcome page with login id and roles roel or session
        public void RedirectToWelcomePage(string loginId = null, string roleId = null)
        {
            // Log the current session values for debugging
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            Console.WriteLine($"Current Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

            // Update session values only if the parameters are not null
            if (loginId != null)
            {
                HttpContext.Current.Session["LoginId"] = loginId;
            }
            if (roleId != null)
            {
                HttpContext.Current.Session["RoleId"] = roleId;
            }

            // Log the updated session values for debugging
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;
            Console.WriteLine($"Updated Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                HttpContext.Current.Response.Redirect("https://wealthmaker.in/login_new.aspx");
            }
            else
            {
                string welcomeUrl = $"~/welcome?loginid={HttpUtility.UrlEncode(currentLoginId)}&roleid={HttpUtility.UrlEncode(currentRoleId)}";
                HttpContext.Current.Response.Redirect(welcomeUrl);
            }
        }


        // add Page page
        public void ReloadCurrentPage(Page page)
        {
            try
            {
                if (page != null)
                {
                    if (ScriptManager.GetCurrent(page) != null && ScriptManager.GetCurrent(page).IsInAsyncPostBack)
                    {
                        // If it's an async postback, use JavaScript to refresh the page
                        ScriptManager.RegisterStartupScript(page, page.GetType(), "ReloadPageScript", "window.location.reload();", true);
                    }
                    else
                    {
                        // Regular postback, use Response.Redirect
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, false);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }

            catch (Exception ex)
            {
                 
            }
        }

      

        public List<string> GetTableColumns(string tableName)
        {
            List<string> columnNames = new List<string>();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_GET_TABLE_COLUMN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter (table name)
                        cmd.Parameters.Add("p_table_name", OracleDbType.Varchar2).Value = tableName.ToUpper();

                        // Output parameter (cursor)
                        cmd.Parameters.Add("v_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        conn.Open();
                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                columnNames.Add(reader.GetString(0)); // Fetch column names
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception properly instead of just printing
                System.Diagnostics.Debug.WriteLine("Error fetching table columns: " + ex.Message);
            }

            return columnNames; // Return the list even if it's empty in case of failure
        }
    }
}