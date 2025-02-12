using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WM.Models;
using System.Web.Configuration;

namespace WM.Controllers
{
    public class AgentController
    {
        private OracleConnection connection;

        public AgentController()
        {
            connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
        }


        #region GetBranchesByLogin
        public DataTable GetBranchesByLogin(string loginId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ANA_BMBYLOG", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for Login ID
                        OracleParameter loginIdParam = new OracleParameter
                        {
                            ParameterName = "p_login_id",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = string.IsNullOrEmpty(loginId) ? DBNull.Value : (object)loginId,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(loginIdParam);

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Ensure the connection is open before executing the command
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
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


        #region GetEmployeeListByBranchOrRM
        public DataTable GetEmployeeListByBranchOrRM(string srmCode, string branch, string loginId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ANA_RMBYBRLOG", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for SRM_CODE
                        OracleParameter srmCodeParam = new OracleParameter
                        {
                            ParameterName = "p_srm_code",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = string.IsNullOrEmpty(srmCode) ? DBNull.Value : (object)srmCode,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(srmCodeParam);

                        // Input parameter for Branch
                        OracleParameter branchParam = new OracleParameter
                        {
                            ParameterName = "p_branch",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = string.IsNullOrEmpty(branch) ? DBNull.Value : (object)branch,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(branchParam);

                        // Input parameter for Login ID
                        OracleParameter loginIdParam = new OracleParameter
                        {
                            ParameterName = "p_login_id",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = string.IsNullOrEmpty(loginId) ? DBNull.Value : (object)loginId,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(loginIdParam);

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Ensure the connection is open before executing the command
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
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
                    using (OracleCommand command = new OracleCommand("PSM_ANA_Get_Branch_Source", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_branch_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Ensure the connection is open before executing the command
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
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



        #region GetAgentListByRM
        public DataTable GetAgentListByRM(string rmCode)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ANA_Get_Agent_By_RM", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for RM_CODE
                        OracleParameter rmCodeParam = new OracleParameter
                        {
                            ParameterName = "p_rm_code",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = rmCode,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(rmCodeParam);

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_agent_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Ensure the connection is open before executing the command
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
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



        #region GetRMListBySource

        public DataTable GetRMListBySource(string sourceID)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ANA_GET_RM_BY_SRCBR", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for source ID
                        OracleParameter sourceParam = new OracleParameter
                        {
                            ParameterName = "p_source_id",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = sourceID,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(sourceParam);

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_branch_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Ensure the connection is open before executing the command
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
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



        #region Method to merge agent
        public bool MergeAgents(string mainAgentExistCode, string[] toMergeAgentsExistCodes)
        {
            bool isSuccess = false;

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_ANA_MergeAgent", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_main_agent_exist_code", OracleDbType.Varchar2).Value = mainAgentExistCode;

                    string mergedCodes = string.Join(",", toMergeAgentsExistCodes);
                    cmd.Parameters.Add("p_to_merge_agent_exist_codes", OracleDbType.Varchar2).Value = mergedCodes;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the procedure

                        isSuccess = true; // Indicate success if no exception occurred
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Oracle error: " + ex.Message);
                    }
                    catch (Exception ex)
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

            return isSuccess; // Return success status
        }
        #endregion




        public DataTable GetFilteredAgentsData(int? sourcID, int? rmCode, string agentName, string existCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_ANA_Get_AGENTLIST", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_sourceid", OracleDbType.Int64).Value = sourcID == 0 ? (object)DBNull.Value : sourcID;

                    cmd.Parameters.Add("p_rm_code", OracleDbType.Int64).Value = rmCode == 0 ? (object)DBNull.Value : rmCode;

                    cmd.Parameters.Add("p_agent_name", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(agentName) ? (object)DBNull.Value : agentName;

                    cmd.Parameters.Add("p_exist_code", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(existCode) ? (object)DBNull.Value : existCode;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_agents_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);  // Fill the DataTable with the result from the procedure
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



        #region AddAgent
        public string AddAgent(Agent agent)
        {
            string result = "";
            if (connection.State == ConnectionState.Closed) { connection.Open(); }
            using (OracleCommand cmd = new OracleCommand("Add_Agent", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("IN_AGENT_NAME", OracleDbType.Varchar2).Value = agent.AGENT_NAME ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_ADDRESS1", OracleDbType.Varchar2).Value = agent.ADDRESS1 ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_ADDRESS2", OracleDbType.Varchar2).Value = agent.ADDRESS2 ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_CITY_ID", OracleDbType.Varchar2).Value = agent.CITY_ID ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_PIN", OracleDbType.Varchar2).Value = agent.PIN ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_PHONE", OracleDbType.Varchar2).Value = agent.PHONE ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_MOBILE", OracleDbType.Varchar2).Value = agent.MOBILE ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_EMAIL", OracleDbType.Varchar2).Value = agent.EMAIL ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_JOINING_DATE", OracleDbType.Date).Value = agent.JOINING_DATE;
                cmd.Parameters.Add("IN_CATEGORY_ID", OracleDbType.Varchar2).Value = agent.CATEGORY_ID ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_TYPE", OracleDbType.Varchar2).Value = agent.AGENT_TYPE ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_AGENT_CODE", OracleDbType.Varchar2).Value = agent.AGENT_CODE ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_LOGGEDUSERID", OracleDbType.Varchar2).Value = agent.LOGGEDUSERID ?? (object)DBNull.Value;
                cmd.Parameters.Add("IN_TIMEST", OracleDbType.Date).Value = agent.TIMEST;

                OracleParameter outputParam = new OracleParameter("p_Result", OracleDbType.Varchar2, 100)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                try
                {
                    cmd.ExecuteNonQuery();
                    result = outputParam.Value.ToString();
                }
                catch (OracleException ex)
                {
                    result = ex.Message;
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

        #region GetAgentList
        public DataTable GetAgentList()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("Get_Agent_List", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;


                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_agent_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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

        #region GetAgentById
        public DataTable GetAgentById(string agentId)
        {
            DataTable dt = new DataTable();

            using (OracleCommand cmd = new OracleCommand("Get_Agent_ById", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IN_AgentId", OracleDbType.Varchar2).Value = agentId;

                OracleParameter cursorParam = new OracleParameter("p_AgentCursor", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(cursorParam);

                try
                {
                    connection.Open();
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
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

            return dt;
        }
        #endregion

        #region UpdateAgent
        public string UpdateAgent(Agent agent)
        {
            string result = "";

            using (OracleCommand cmd = new OracleCommand("Update_Agent", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmd.Parameters.Add("IN_agent_code", OracleDbType.Varchar2).Value = agent.AGENT_CODE;
                cmd.Parameters.Add("IN_agent_name", OracleDbType.Varchar2).Value = agent.AGENT_NAME;
                cmd.Parameters.Add("IN_address1", OracleDbType.Varchar2).Value = agent.ADDRESS1;
                cmd.Parameters.Add("IN_address2", OracleDbType.Varchar2).Value = agent.ADDRESS2;

                OracleParameter messageParam = new OracleParameter("p_message", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(messageParam);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    result = cmd.Parameters["p_message"].Value.ToString();
                }
                catch (OracleException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    result = "Error: " + ex.Message;
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

        #region GetAllReportingManagers
        public DataTable GetAllReportingManagers()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("Get_Reporting_Manager", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_report_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return dt;
        }
        #endregion
    }
}
