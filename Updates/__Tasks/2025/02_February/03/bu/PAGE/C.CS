using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WM.Models;
using System.Web.Configuration;
using System.Web.ModelBinding;
using Oracle.ManagedDataAccess.Types;

namespace WM.Controllers
{
    public class AssociateController
    {
        private OracleConnection connection;
        public AssociateController()
        {
            connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
        }

        #region GetStateForCityList
        public DataTable GetStateForCityList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("psm_ass_m_statefcity", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Output parameter for the SYS_REFCURSOR
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_result",  // Parameter name as defined in the procedure
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



        #region GetRmByBranchDTList
        public DataTable GetRmByBranchDTList(string sourceId)
        {
            DataTable dt = new DataTable();

            try
            {
                // Establishing connection to the database
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    // Create the OracleCommand for the stored procedure
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_RM_BY_BRANCHDT", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for source_id
                        OracleParameter sourceParam = new OracleParameter
                        {
                            ParameterName = "p_source_id",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = sourceId,  // Pass the sourceId value here
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

                        // Using OracleDataAdapter to fill the DataTable with the result set
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


        #region GetClientDataByDTNumber
        #region GetDocumentCodes
        public DataTable GetClientDataByDTNumber(string documentId)
        {
            DataTable dtDocumentCodes = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_AM_GET_BY_DT_ID", con))
                    {
                        OracleDataAdapter da;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_common_id", OracleDbType.Varchar2).Value = documentId;
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        da = new OracleDataAdapter(cmd);
                        da.Fill(dtDocumentCodes);
                    }
                }
                catch (Exception ex)
                {
                    // ErrorHandler.ErrorLog("Get Document Codes", ex);
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

            return dtDocumentCodes;
        }
        #endregion

        public DataTable GetClientDataByDTNumber_n(string clientID)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_ASS_M_CLIENT_DT", conn))
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


        #region InsertAgentMaster
        public string InsertAgentMaster(
            string loggedInUser,
            string empanelmentType,
            string agentCode,
            string existCode,
            string agentName,
            string sourceId,
            string rmCode,
            string address1,
            string address2,
            string cityId,
            string locationId,
            long mobile, 
            string pin,  
            string fax,
            string contactPerson,
            string email,
            decimal tds,
            string associateType,
            string associateCategory,
            string contactPersonEmailId,
            DateTime? empanelmentDate,
            string phone,
            string remarks,
            string superANA,
            string onlineSubscriptionCheck,
            string onlinePlatformBlock,
            string offlinePlatformBlock,
            string onlinePlatformBlockRemark,
            string offlinePlatformBlockRemark,
            DateTime? auditDate,
            string auditCheck,
            int paymentModeId,
            string accountTypeId,
            string accNo,
            DateTime? affectedFrom, // Adjusted to match VARCHAR2 type
            string bankId,
            string bankCityID,
            string brankBranchID,
            string smsFlag,
            string gstinNo,
            DateTime? dob,
            string agentType,
            string pan,
            string dist,
            string aadharCardNo,
            string pospMarking,
            string pospType,
            string pospNoLi,
            string pospNoGi,
            DateTime? pospCertifiedOnLi,
            DateTime? pospValidTillLi,
            DateTime? pospCertifiedOnGi,
            DateTime? pospValidTillGi,
            string verifiedStatus,
            string neftBankName,
            string neftBranch,
            string neftIfscCode,
            string neftName,
            string ceftPassed,
            string certExams,
            string certRegNo,
            string resAddAddress1Value,
            string resAddAddress2Value,
            string resAddStateValue,
            string resAddCityValue,
            string resAddPINValue,
            string dtNumberValue
        )
        {
            string result;
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_ASS_M_INSERT_AGENT_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters with corrected types
                    cmd.Parameters.Add("P_LOGGEDINUSER", OracleDbType.Varchar2).Value = loggedInUser ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMPANELMENTTYPE", OracleDbType.Varchar2).Value = empanelmentType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AGENTCODE", OracleDbType.Varchar2).Value = agentCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EXISTCODE", OracleDbType.Varchar2).Value = existCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AGENTNAME", OracleDbType.Varchar2).Value = agentName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SOURCEID", OracleDbType.Varchar2).Value = sourceId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RMCODE", OracleDbType.Varchar2).Value = rmCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ADDRESS1", OracleDbType.Varchar2).Value = address1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ADDRESS2", OracleDbType.Varchar2).Value = address2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CITYID", OracleDbType.Varchar2).Value = cityId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOCATIONID", OracleDbType.Varchar2).Value = locationId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Int64).Value = mobile;  
                    cmd.Parameters.Add("P_PIN", OracleDbType.Varchar2).Value = pin; 
                    cmd.Parameters.Add("P_FAX", OracleDbType.Varchar2).Value = fax ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CONTACTPER", OracleDbType.Varchar2).Value = contactPerson ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = email ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_TDS", OracleDbType.Decimal).Value = tds;
                    cmd.Parameters.Add("P_ASSOCIATETYPE", OracleDbType.Varchar2).Value = associateType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ASSOCIATETYPECATEGORY", OracleDbType.Varchar2).Value = associateCategory ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CONTACTPERSONEMAILID", OracleDbType.Varchar2).Value = contactPersonEmailId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMPANELMENTDATE", OracleDbType.Date).Value = empanelmentDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = phone ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REMARKS", OracleDbType.Varchar2).Value = remarks ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SUPERANA", OracleDbType.Varchar2).Value = superANA ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ONLINESUBSCRIPTIONCHECK", OracleDbType.Varchar2).Value = onlineSubscriptionCheck ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ONLINEPLATFORMBLOCK", OracleDbType.Varchar2).Value = onlinePlatformBlock ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OFFLINEPLATFORMBLOCK", OracleDbType.Varchar2).Value = offlinePlatformBlock ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ONLINEPLATFORMBLOCKREMARK", OracleDbType.Varchar2).Value = onlinePlatformBlockRemark ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OFFLINEPLATFORMBLOCKREMARK", OracleDbType.Varchar2).Value = offlinePlatformBlockRemark ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AUDITDATE", OracleDbType.Date).Value = auditDate ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AUDITCHECK", OracleDbType.Varchar2).Value = auditCheck ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAYMENTMODEID", OracleDbType.Int32).Value = paymentModeId;
                    cmd.Parameters.Add("P_ACCOUNTTYPEID", OracleDbType.Varchar2).Value = accountTypeId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCNO", OracleDbType.Varchar2).Value = accNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AFFECTEDFROM", OracleDbType.Date).Value = affectedFrom ?? (object)DBNull.Value; // Adjusted to VARCHAR2
                    cmd.Parameters.Add("P_BANKID", OracleDbType.Varchar2).Value = bankId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BANKCITYID", OracleDbType.Varchar2).Value = bankCityID ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BRANKBRANCHID", OracleDbType.Varchar2).Value = brankBranchID ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SMSFLAG", OracleDbType.Varchar2).Value = smsFlag ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GSTINNO", OracleDbType.Varchar2).Value = gstinNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value = dob ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AGENTTYPE", OracleDbType.Varchar2).Value = agentType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = pan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DIST", OracleDbType.Varchar2).Value = dist ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHARCARDNO", OracleDbType.Varchar2).Value = aadharCardNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPMARKING", OracleDbType.Varchar2).Value = pospMarking ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPTYPE", OracleDbType.Varchar2).Value = pospType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPNOLI", OracleDbType.Varchar2).Value = pospNoLi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPNOGI", OracleDbType.Varchar2).Value = pospNoGi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPCERTIFIEDONLI", OracleDbType.Date).Value = pospCertifiedOnLi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPVALIDTILLLI", OracleDbType.Date).Value = pospValidTillLi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPCERTIFIEDONGI", OracleDbType.Date).Value = pospCertifiedOnGi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPVALIDTILLGI", OracleDbType.Date).Value = pospValidTillGi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_VERIFIEDSTATUS", OracleDbType.Varchar2).Value = verifiedStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTBANKNAME", OracleDbType.Varchar2).Value = neftBankName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTBRANCH", OracleDbType.Varchar2).Value = neftBranch ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTIFSCCODE", OracleDbType.Varchar2).Value = neftIfscCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTNAME", OracleDbType.Varchar2).Value = neftName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CERTPASSED", OracleDbType.Varchar2).Value = ceftPassed ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CERTEXAMS", OracleDbType.Varchar2).Value = certExams ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CERTREGNO", OracleDbType.Varchar2).Value = certRegNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_1", OracleDbType.Varchar2).Value = resAddAddress1Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_2", OracleDbType.Varchar2).Value = resAddAddress2Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_STATE", OracleDbType.Varchar2).Value = resAddStateValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_CITY", OracleDbType.Varchar2).Value = resAddCityValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_PINCODE", OracleDbType.Varchar2).Value = resAddPINValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    /*
                                        try
                                        {
                                            conn.Open();
                                            cmd.ExecuteNonQuery(); // Execute the command

                                            // Retrieve the message from the RefCursor in P_RESULT
                                            using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                                            {
                                                if (reader.Read())
                                                {
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

                                        */

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Execute the command

                    // Retrieve the message from the RefCursor in P_RESULT
                    using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                    {
                        if (reader.Read())
                        {
                            result = reader["message"].ToString();
                        }
                        else
                        {
                            result = "No message retrieved.";
                        }
                    }
                }
            }
            return result;
        }
        #endregion



        #region UpdateAgentMaster
        public string UpdateAgentMaster(
            string loggedInUser,
            string empanelmentType,
            string agentCode,
            string existCode,
            string agentName,
            string sourceId,
            string rmCode,
            string address1,
            string address2,
            string cityId,
            string locationId,
            long mobile,
            string pin,
            string fax,
            string contactPerson,
            string email,
            decimal tds,
            string associateType,
            string associateCategory,

            string contactPersonEmailId,
            DateTime? empanelmentDate,
            string phone,
            string remarks,
            string superANA,
            string onlineSubscriptionCheck,
            string onlinePlatformBlock,
            string offlinePlatformBlock,
            string onlinePlatformBlockRemark,
            string offlinePlatformBlockRemark,
            DateTime? auditDate,
            string auditCheck,
            int paymentModeId,
            string accountTypeId,
            string accNo,
            DateTime? affectedFrom,
            string bankId,
            string bankCityID,
            string brankBranchID,

            string smsFlag,
            string gstinNo,
            DateTime? dob,
            string agentType,
            string pan,
            string dist,

            string aadharCardNo,
            string pospMarking,
            string pospType,
            string pospNoLi,
            string pospNoGi,
            DateTime? pospCertifiedOnLi,
            DateTime? pospValidTillLi,
            DateTime? pospCertifiedOnGi,
            DateTime? pospValidTillGi,

            string verifiedStatus,
            string neftBankName,
            string neftBranch,
            string neftIfscCode,
            string neftName,

            string ceftPassed,
            string certExams,
            string certRegNo,
            string resAddAddress1Value,
            string resAddAddress2Value,
            string resAddStateValue,
            string resAddCityValue,
            string resAddPINValue,

             string dtNumberValue
            )
        {
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                string resultMessage = string.Empty;

                using (OracleCommand cmd = new OracleCommand("PSM_ASS_M_UPDATE_AGENT_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters and handle nulls
                    cmd.Parameters.Add("P_LOGGEDINUSER", OracleDbType.Varchar2).Value = loggedInUser ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_EMPANELMENTTYPE", OracleDbType.Varchar2).Value                = empanelmentType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AGENTCODE", OracleDbType.Varchar2).Value                      = agentCode;
                    cmd.Parameters.Add("P_EXISTCODE", OracleDbType.Varchar2).Value                      = existCode;
                    cmd.Parameters.Add("P_AGENTNAME", OracleDbType.Varchar2).Value                      = agentName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SOURCEID", OracleDbType.Varchar2).Value                       = sourceId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RMCODE", OracleDbType.Varchar2).Value                         = rmCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ADDRESS1", OracleDbType.Varchar2).Value                       = address1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ADDRESS2", OracleDbType.Varchar2).Value                       = address2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CITYID", OracleDbType.Varchar2).Value                         = cityId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOCATIONID", OracleDbType.Varchar2).Value                     = locationId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Decimal).Value                          = mobile;
                    cmd.Parameters.Add("P_PIN", OracleDbType.Varchar2).Value                            = pin;
                    cmd.Parameters.Add("P_FAX", OracleDbType.Varchar2).Value                            = fax ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CONTACTPER", OracleDbType.Varchar2).Value                     = contactPerson ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value                          = email ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_TDS", OracleDbType.Decimal).Value                             = tds;
                    cmd.Parameters.Add("P_ASSOCIATETYPE", OracleDbType.Varchar2).Value                  = associateType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ASSOCIATETYPECATEGORY", OracleDbType.Varchar2).Value          = (object)associateCategory ?? DBNull.Value;
                    cmd.Parameters.Add("P_CONTACTPERSONEMAILID", OracleDbType.Varchar2).Value           = contactPersonEmailId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMPANELMENTDATE", OracleDbType.Date).Value                    = empanelmentDate.HasValue ? (object)empanelmentDate.Value : DBNull.Value;
                    cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value                          = phone ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_REMARKS", OracleDbType.Varchar2).Value                        = remarks ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SUPERANA", OracleDbType.Varchar2).Value                       = superANA ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ONLINESUBSCRIPTIONCHECK", OracleDbType.Varchar2).Value        = onlineSubscriptionCheck ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ONLINEPLATFORMBLOCK", OracleDbType.Varchar2).Value            = onlinePlatformBlock ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OFFLINEPLATFORMBLOCK", OracleDbType.Varchar2).Value           = offlinePlatformBlock ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ONLINEPLATFORMBLOCKREMARK", OracleDbType.Varchar2).Value      = onlinePlatformBlockRemark ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_OFFLINEPLATFORMBLOCKREMARK", OracleDbType.Varchar2).Value     = offlinePlatformBlockRemark ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AUDITDATE", OracleDbType.Date).Value                          = auditDate.HasValue ? (object)auditDate.Value : DBNull.Value;
                    cmd.Parameters.Add("P_AUDITCHECK", OracleDbType.Varchar2).Value                     = auditCheck ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAYMENTMODEID", OracleDbType.Int32).Value                     = paymentModeId;
                    cmd.Parameters.Add("P_ACCOUNTTYPEID", OracleDbType.Varchar2).Value                  = accountTypeId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ACCNO", OracleDbType.Varchar2).Value                          = accNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AFFECTEDFROM", OracleDbType.Date).Value                       = affectedFrom.HasValue ? (object)affectedFrom.Value : DBNull.Value;
                    cmd.Parameters.Add("P_BANKID", OracleDbType.Varchar2).Value                         = bankId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BANKCITYID", OracleDbType.Varchar2).Value                     = bankCityID ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BRANKBRANCHID", OracleDbType.Varchar2).Value                  = brankBranchID ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SMSFLAG", OracleDbType.Varchar2).Value                        = smsFlag ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_GSTINNO", OracleDbType.Varchar2).Value                        = gstinNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOB", OracleDbType.Date).Value                                = dob.HasValue ? (object)dob.Value : DBNull.Value;
                    cmd.Parameters.Add("P_AGENTTYPE", OracleDbType.Varchar2).Value                      = agentType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value                            = pan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DIST", OracleDbType.Varchar2).Value                           = dist ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHARCARDNO", OracleDbType.Varchar2).Value                   = aadharCardNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPMARKING", OracleDbType.Varchar2).Value                    = pospMarking ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPTYPE", OracleDbType.Varchar2).Value                       = pospType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPNOLI", OracleDbType.Varchar2).Value                       = pospNoLi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPNOGI", OracleDbType.Varchar2).Value                       = pospNoGi ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_POSPCERTIFIEDONLI", OracleDbType.Date).Value                  = pospCertifiedOnLi.HasValue ? (object)pospCertifiedOnLi.Value : DBNull.Value;
                    cmd.Parameters.Add("P_POSPVALIDTILLLI", OracleDbType.Date).Value                    = pospValidTillLi.HasValue ? (object)pospValidTillLi.Value : DBNull.Value;
                    cmd.Parameters.Add("P_POSPCERTIFIEDONGI", OracleDbType.Date).Value                  = pospCertifiedOnGi.HasValue ? (object)pospCertifiedOnGi.Value : DBNull.Value;
                    cmd.Parameters.Add("P_POSPVALIDTILLGI", OracleDbType.Date).Value                    = pospValidTillGi.HasValue ? (object)pospValidTillGi.Value : DBNull.Value;
                    cmd.Parameters.Add("P_VERIFIEDSTATUS", OracleDbType.Varchar2).Value                 = verifiedStatus ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTBANKNAME", OracleDbType.Varchar2).Value                   = neftBankName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTBRANCH", OracleDbType.Varchar2).Value                     = neftBranch ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTIFSCCODE", OracleDbType.Varchar2).Value                   = neftIfscCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_NEFTNAME", OracleDbType.Varchar2).Value                       = neftName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CERTPASSED", OracleDbType.Varchar2).Value                     = ceftPassed ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CERTEXAMS", OracleDbType.Varchar2).Value                      = certExams ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CERTREGNO", OracleDbType.Varchar2).Value                      = certRegNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_1", OracleDbType.Varchar2).Value                      = resAddAddress1Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_2", OracleDbType.Varchar2).Value                      = resAddAddress2Value ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_STATE", OracleDbType.Varchar2).Value                  = resAddStateValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_CITY", OracleDbType.Varchar2).Value                   = resAddCityValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_RES_ADD_PINCODE", OracleDbType.Varchar2).Value                = resAddPINValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value                      = dtNumberValue ?? (object)DBNull.Value;









                    OracleParameter resultParam = new OracleParameter("P_RESULT", OracleDbType.Varchar2, 2000) // Adjust size as needed
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(resultParam);


                    try
                    {
                        // Execute the stored procedure
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Get the result message from the OUT parameter
                        resultMessage = resultParam.Value.ToString();

                    }
                    catch (Exception ex)
                    {
                        resultMessage = $"Error: {ex.Message}";
                    }
                }
                return resultMessage;
            }

        }
        #endregion

        #region GetAssociateTypeListCat
        public DataTable GetAssociateTypeListCat()
        {
            DataTable dtCity = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_AM_GET_ASCT_TYPE_CAT", con))
                    {
                        OracleDataAdapter da;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(cmd);
                        da.Fill(dtCity);
                    }
                }
                catch (Exception ex)
                {
                    //ErrorHandler.ErrorLog("State Master", ex);
                    //Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtCity;
        }
        #endregion

        #region GetAssociateTypeList
        public DataTable GetAssociateTypeList()
        {
            DataTable dtCity = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_ASS_M_Get_AssociateType", con))
                    {
                        OracleDataAdapter da;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(cmd);
                        da.Fill(dtCity);
                    }
                }
                catch (Exception ex)
                {
                    //ErrorHandler.ErrorLog("State Master", ex);
                    //Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtCity;
        }
        #endregion

        #region GetBranchList
        public DataTable GetBranchMasterList(string login, string mark1, string mark2)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_AM_BRANCH_MASTER_3", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add(new OracleParameter("p_login", OracleDbType.Varchar2)).Value = GetDbNullIfEmpty(login);
                        command.Parameters.Add(new OracleParameter("p_mark1", OracleDbType.Varchar2)).Value = GetDbNullIfEmpty(mark1);
                        command.Parameters.Add(new OracleParameter("p_mark2", OracleDbType.Varchar2)).Value = GetDbNullIfEmpty(mark2);

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
                // Optionally, you can rethrow the exception or handle it differently
            }

            return dt;
        }
        #endregion

        #region GetRMListBySourceBranch
        public DataTable GetRMListBySourceBranch(string sourceID)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ANA_SR_BR", conn))
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

        #region GetRMByRMCode
        public DataTable GetRMByRMCode(string rmCode)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_RM_BY_RM_CODE", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for RM_CODE
                        OracleParameter rmCodeParam = new OracleParameter
                        {
                            ParameterName = "P_RM_CODE",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = rmCode,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(rmCodeParam);

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "P_RM_BY_RM_CODE",
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

        #region GetCitylist
        public DataTable GetCitylist()
        {
            DataTable dtCity = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_EMP_M__GET_CITY", con))
                    {
                        OracleDataAdapter da;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(cmd);
                        da.Fill(dtCity);
                    }
                }
                catch (Exception ex)
                {
                    //ErrorHandler.ErrorLog("State Master", ex);
                    //Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtCity;
        }
        #endregion


        #region GetCitylist
        public DataTable GetCityByStatelist(string stateId)
        {
            DataTable dtCity = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_ASS_M__CITYBYSTATE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add input parameter for state ID
                        cmd.Parameters.Add("P_STATE_ID", OracleDbType.Varchar2).Value = stateId;

                        // Add output parameter for the cursor
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Fill the DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtCity);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (uncomment the error log below if applicable)
                    // ErrorHandler.ErrorLog("State Master", ex);
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtCity;
        }
        #endregion


        #region GetStateNamesByCity
        public DataTable GetStateByCity(string cityId)
        {
            // Create a DataTable to store the result
            DataTable stateTable = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();  // Open the database connection

                    using (OracleCommand cmd = new OracleCommand("PSM_GET_STATE_NAME_BY_CITY", con))
                    {
                        // Specify that this is a stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter for city_id
                        cmd.Parameters.Add("p_city_id", OracleDbType.Varchar2).Value = cityId;

                        // Output parameter for the SYS_REFCURSOR (used for result set)
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_result",  // The cursor parameter name as defined in the stored procedure
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(cursorParam);

                        // Execute the command and get the SYS_REFCURSOR
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            // Fill the DataTable with the results
                            adapter.Fill(stateTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the error as needed
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    // Ensure the connection is closed
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }

            // Return the DataTable containing the state names and state IDs
            return stateTable;
        }
        #endregion



        #region GetStateNameByCity
        public string GetStateNameByCity(string cityId)
        {
            string stateName = string.Empty;

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_GET_STATE_NAME_BY_CITY", con))
                    {
                        // Specify that this is a stored procedure
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter for city_id
                        cmd.Parameters.Add("p_city_id", OracleDbType.Varchar2).Value = cityId;

                        // Output parameter for state_name
                        cmd.Parameters.Add("p_state_name", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;

                        // Execute the command
                        cmd.ExecuteNonQuery();

                        // Get the output parameter value (state name)
                        stateName = cmd.Parameters["p_state_name"].Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors (logging or otherwise)
                    // ErrorHandler.ErrorLog("Get State Name By City", ex);
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

            return stateName;
        }
        #endregion

        #region GetLocationListBySourceBranch
        public DataTable GetLocationListBySourceBranch(string sourceID)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_Get_Location_List", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameter for source ID
                        OracleParameter sourceParam = new OracleParameter
                        {
                            ParameterName = "p_city_by_branch",
                            OracleDbType = OracleDbType.Varchar2,
                            Value = sourceID,
                            Direction = ParameterDirection.Input
                        };
                        command.Parameters.Add(sourceParam);

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "location_cursor",
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

        #region GetLocationList
        public DataTable GetLocationList()
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_Get_Location_List", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Define the OUT parameter to receive the cursor
                        OracleParameter refCursorParam = new OracleParameter();
                        refCursorParam.OracleDbType = OracleDbType.RefCursor;
                        refCursorParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(refCursorParam);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();

                        // Retrieve the cursor
                        using (OracleDataReader reader = ((OracleRefCursor)refCursorParam.Value).GetDataReader())
                        {
                            // Load the data into a DataTable                          
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ErrorHandler.ErrorLog("State Master", ex);
                    //Console.WriteLine("Error: " + ex.Message);
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

        #region GetAgentMasterDetails for Super ANA
        public DataTable GetAgentMasterDetails()
        {
            DataTable dtAgentMasterDetails = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_GET_AGENT_MASTER_LIST", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Define the output parameter
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Fill the DataTable with the result from the procedure
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtAgentMasterDetails);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors (logging or otherwise)
                    // ErrorHandler.ErrorLog("Get Agent Master Details", ex);
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

            return dtAgentMasterDetails;
        }
        #endregion


        private object GetDbNullIfEmpty(object input)
        {
            if (input == null)
            {
                return DBNull.Value;
            }

            if (input is string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                {
                    return DBNull.Value;
                }
                return str;
            }

            if (input is int intValue)
            {
                if (intValue == 0)
                {
                    return DBNull.Value;
                }
                return intValue;
            }

            if (input is decimal decimalValue)
            {
                if (decimalValue == 0)
                {
                    return DBNull.Value;
                }
                return decimalValue;
            }

            if (input is double doubleValue)
            {
                if (doubleValue == 0)
                {
                    return DBNull.Value;
                }
                return doubleValue;
            }

            if (input is bool boolValue)
            {
                return boolValue ? "1" : "0"; // Convert `true` to 1, `false` to 0 for Oracle
            }

            return input;
        }

        #region GetBankMasterDetails
        public DataTable GetBankMasterDetails(string bankBranchMark, string branchByBank)
        {
            DataTable dtBankMasterDetails = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_AM_GET_BANK_BRANCH", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.Add("p_bank_branch_mark", OracleDbType.Varchar2).Value = GetDbNullIfEmpty(bankBranchMark);
                        cmd.Parameters.Add("p_branch_by_bank", OracleDbType.Varchar2).Value = GetDbNullIfEmpty(branchByBank);
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Fill DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtBankMasterDetails);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error (uncomment and implement logging)
                    // ErrorHandler.ErrorLog("Bank Master Details", ex);
                    // Console.WriteLine("Error: " + ex.Message);
                }
            }

            return dtBankMasterDetails;
        }
        #endregion


        #region GetExamList
        public DataTable GetExamList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_GET_EXAMLIST", conn))
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

        #region GetPaymentModeID
        public DataTable GetPaymentModeID()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_GET_PAYMODEID", conn))
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

        #region GetBankAccountTypeID
        public DataTable GetBankAccountTypeID()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_GET_BANKACCTYPEID", conn))
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

        #region GetBankAccountTypeID
        public DataTable GetOtherTypeID()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_ASS_M_GET_OTHERDETAILSTYPE", conn))
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
                    using (OracleCommand command = new OracleCommand("PSM_GET_BRANCH_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "P_BRANCH_LIST",
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
                    using (OracleCommand command = new OracleCommand("PSM_GET_STATE_LIST", conn))
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

     
        #region GetAssociateDataByAssociateCode
        public DataTable GetAssociateDataByAssociateCode(Agent agent)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AM_GET_AGENT_BY_ID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_agent_code", OracleDbType.Varchar2).Value = agent.AGENT_CODE ?? (object)DBNull.Value;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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


        // Function to handle DBNull checking
        public object HandleStringDbValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            return value;
        }



        #region PSMGetAssociateList
        public DataTable GetAssociateListByCriteria(
            string branchID,
            string cityID,
            string mobileNO,
            string phoneNO,
            string assCode,
            string agentCode,
            string panNO,
            string assName

            )
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_AM_AGENT_BY_ID ", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command using the GetDbValue function
                    cmd.Parameters.Add("P_BRANCH_ID", OracleDbType.Varchar2).Value = HandleStringDbValue(branchID);
                    cmd.Parameters.Add("P_CITY_ID", OracleDbType.Varchar2).Value = HandleStringDbValue(cityID);
                    cmd.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = HandleStringDbValue(mobileNO);
                    cmd.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = HandleStringDbValue(phoneNO);
                    cmd.Parameters.Add("P_ASSOCIATE_CODE", OracleDbType.Varchar2).Value = HandleStringDbValue(assCode);
                    cmd.Parameters.Add("P_AGENT_CODE", OracleDbType.Varchar2).Value = HandleStringDbValue(agentCode);

                    cmd.Parameters.Add("P_PAN_NO", OracleDbType.Varchar2).Value = HandleStringDbValue(panNO);
                    cmd.Parameters.Add("P_ASSOCIATE_NAME", OracleDbType.Varchar2).Value = HandleStringDbValue(assName);


                    cmd.Parameters.Add("P_ASSOCIATE_LIST", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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
                            conn.Close();
                    }
                }
            }
            return dt;
        }
        #endregion

        #region GetExamList
        public DataTable GetExamListDefault()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("Get_Exam_List", conn)) // Assuming you have a stored procedure named Get_Exam_List
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add output parameter
                    cmd.Parameters.Add("p_exam_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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
                        // Handle exception
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            return dt;
        }
        #endregion

        private DataTable ExecuteListProcedure(string procedureName)
        {
            DataTable dt = new DataTable();

            using (OracleCommand cmd = new OracleCommand(procedureName, connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter cursorParam = new OracleParameter("p_Cursor", OracleDbType.RefCursor)
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
    }
}
