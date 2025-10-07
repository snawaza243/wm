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
using System.Web.UI.WebControls; 
using System.IO;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Globalization;
using MathNet.Numerics.Distributions;
using System.Text;
using NPOI.SS.Formula.Functions;
using Newtonsoft.Json;

namespace WM.Controllers
{


    public class PsmController
    {
        OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
        public static string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;


        #region PSM MODAL: INVESTOR ADDRESS UDPATE

        public (bool Success, string Message, Dictionary<string, object> Data) PSMM_INV_ADD_UP_GET_DATA(string invCode)
        {
            string loginId = currentLoginID();
            string roleId = currentRoleID();
            var data = new Dictionary<string, object>();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_ADD_UP_GET_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_INV_CODE", OracleDbType.Varchar2).Value = invCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("P_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        using (var reader = ((OracleRefCursor)outCursor.Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                // Always return the message
                                string message = reader["MSG"]?.ToString() ?? "No message returned";
                                bool isSuccess = message.StartsWith("SUCCESS:");

                                // Collect all returned fields dynamically
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string colName = reader.GetName(i);
                                    object colValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    data[colName] = colValue;
                                }

                                return (isSuccess, message, data);
                            }
                        }

                        return (false, "No response received from procedure", data);
                    }
                    catch (OracleException ex)
                    {
                        string errorMessage = $"Database Error: {ex.Message}";
                        return (false, errorMessage, null);
                    }
                    catch (Exception ex)
                    {
                        return (false, $"System Error: {ex.Message}", null);
                    }
                }
            }
        }

        public DataTable PSMM_INV_ADD_UP_GET_DATA1(string invCode)
        {
            string loginId = currentLoginID();
            string roleId = currentRoleID();
            var dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_ADD_UP_GET_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_INV_CODE", OracleDbType.Varchar2).Value = invCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("P_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        return dt;
                    }
                    catch (OracleException ex)
                    {
                        // Optionally log error
                        return null;
                    }
                    catch (Exception ex)
                    {
                        // Optionally log error
                        return null;
                    }
                }
            }
        }


        public (bool Success, string Message) PSMM_INV_ADD_UP_UP_DATA(string invCode,string mobile,string pan,string email,string aadhar,string address1,string address2,string pincode,string cityId,string stateId,string dob)
        {
            string loginId = currentLoginID();
            string role = currentRoleID();
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_ADD_UP_SET_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_INV_CODEX", OracleDbType.Varchar2).Value = invCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MOBILEX", OracleDbType.Varchar2).Value = mobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PANX", OracleDbType.Varchar2).Value = pan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_EMAILX", OracleDbType.Varchar2).Value = email ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AADHARX", OracleDbType.Varchar2).Value = aadhar ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ADDRESS1X", OracleDbType.Varchar2).Value = address1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ADDRESS2X", OracleDbType.Varchar2).Value = address2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PINCODEX", OracleDbType.Varchar2).Value = pincode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CITY_IDX", OracleDbType.Varchar2).Value = cityId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_STATE_IDX", OracleDbType.Varchar2).Value = stateId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DOBX", OracleDbType.Varchar2).Value = dob ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGINX", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ROLEX", OracleDbType.Varchar2).Value = role ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("P_CURSORX", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        using (var reader = ((OracleRefCursor)outCursor.Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                string message = reader["MSG"].ToString();
                                bool isSuccess = message.StartsWith("SUCCESS:");
                                return (isSuccess, message);
                            }
                        }

                        return (false, "No response received from procedure");
                    }
                    catch (OracleException ex)
                    {
                        string errorMessage = $"Database Error: {ex.Message}";
                        return (false, errorMessage);
                    }
                    catch (Exception ex)
                    {
                        return (false, $"System Error: {ex.Message}");
                    }
                }
            }
        }

        #endregion


        #region PSM MODAL: CROSS CHANNEL VALIDATION

        //using Oracle.ManagedDataAccess.Types; // add this namespace at top
        public (bool Success, int Count) PSMM_CROSS_CH_VALIDATE(string frmType, string commonId, string subClientCd)
        {
            string loginId = currentLoginID();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_CROSS_CH_VALIDATE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_FRM_TYPE", OracleDbType.Varchar2).Value = frmType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_COMMON_ID", OracleDbType.Varchar2).Value = commonId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SUB_CLIENT_CD", OracleDbType.Varchar2).Value = subClientCd ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;

                    // Output parameter
                    var outCnt = new OracleParameter("P_CNT", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCnt);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        int count = 0;
                        if (outCnt.Value != DBNull.Value)
                        {
                            // Convert OracleDecimal to int safely
                            count = ((OracleDecimal)outCnt.Value).ToInt32();
                        }

                        bool success = count > 0;
                        return (success, count);
                    }
                    catch (OracleException ex)
                    {
                        // Log or handle error as needed
                        return (false, -1);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle error as needed
                        return (false, -1);
                    }
                }
            }
        }


        public (bool Success, string Message, Dictionary<string, object> Data) PSMM_CROSS_CH_GET_DATA(string commonId, string invCode)
        {
            string logId = currentLoginID();
            var data = new Dictionary<string, object>();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_CROSS_CH_GET_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_COMMONID", OracleDbType.Varchar2).Value = commonId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOG", OracleDbType.Varchar2).Value = logId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_INVCODE", OracleDbType.Varchar2).Value = invCode ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("P_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        using (var reader = ((OracleRefCursor)outCursor.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    // Collect all returned fields dynamically
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        string colName = reader.GetName(i);
                                        object colValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                        data[colName] = colValue;
                                    }
                                }

                                return (true, "SUCCESS: Data fetched successfully", data);
                            }
                        }

                        return (false, "No data returned from procedure", data);
                    }
                    catch (OracleException ex)
                    {
                        string errorMessage = $"Database Error: {ex.Message}";
                        return (false, errorMessage, null);
                    }
                    catch (Exception ex)
                    {
                        return (false, $"System Error: {ex.Message}", null);
                    }
                }
            }
        }

        public DataTable PSMM_CROSS_CH_GET_DATA1(string commonId, string invCode)
        {
            string logId = currentLoginID();
            var dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_CROSS_CH_GET_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_COMMONID", OracleDbType.Varchar2).Value = commonId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOG", OracleDbType.Varchar2).Value = logId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_INVCODE", OracleDbType.Varchar2).Value = invCode ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("P_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();

                        // Use OracleDataAdapter to fill DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        return dt; // return the filled DataTable
                    }
                    catch (OracleException ex)
                    {
                        // Log or handle error
                        return null;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle error
                        return null;
                    }
                }
            }
        }


        #endregion


        #region PSM MODAL: INVESTOR SEARCH TREE MASTER
        public DataTable PSMM_INV_SEARCH_TREE_FIND(string pxBranch, string pxCat, string pxCity, string pxCode, string pxName, string pxAdd1, string pxAdd2, string pxPhone, string pxPan, string pxMobile, string pxNewRm, string pxAhCode, string pxClientSubName, string pxClientBroker, string pxStrForm, string pxCurrentForm, string pxRm, string pxOldRm, string pxSort)
        {
            string loginId = currentLoginID();
            string roleId = currentRoleID();
            var dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_SEARCH_TREE_FIND", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("PX_BRANCH", OracleDbType.Varchar2).Value = pxBranch ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_CAT", OracleDbType.Varchar2).Value = pxCat ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_CITY", OracleDbType.Varchar2).Value = pxCity ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_CODE", OracleDbType.Varchar2).Value = pxCode ?? (object)DBNull.Value; // CLIENT/ SUB_BROKER CODE
                    cmd.Parameters.Add("PX_NAME", OracleDbType.Varchar2).Value = pxName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_ADD1", OracleDbType.Varchar2).Value = pxAdd1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_ADD2", OracleDbType.Varchar2).Value = pxAdd2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_PHONE", OracleDbType.Varchar2).Value = pxPhone ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_PAN", OracleDbType.Varchar2).Value = pxPan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_MOBILE", OracleDbType.Varchar2).Value = pxMobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_NEW_RM", OracleDbType.Varchar2).Value = pxNewRm ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_AH_CODE", OracleDbType.Varchar2).Value = pxAhCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_CLIENT_SUB_NAME", OracleDbType.Varchar2).Value = pxClientSubName ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_CLIENT_BROKER", OracleDbType.Varchar2).Value = pxClientBroker ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_LOG_ID", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_ROLE_ID", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_STR_FORM", OracleDbType.Varchar2).Value = pxStrForm ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_CURRENT_FORM", OracleDbType.Varchar2).Value = pxCurrentForm ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_RM", OracleDbType.Varchar2).Value = pxRm ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_OLD_RM", OracleDbType.Varchar2).Value = pxOldRm ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_SORT", OracleDbType.Varchar2).Value = pxSort ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("PX_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        return dt;
                    }
                    catch (OracleException ex)
                    {
                        // Log Oracle-specific errors
                        // Logger.Error("Oracle error in PSMM_INV_SEARCH_TREE_FIND", ex);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        // Log general errors
                        // Logger.Error("General error in PSMM_INV_SEARCH_TREE_FIND", ex);
                        return null;
                    }
                }
            }
        }

        public (bool Success, string Message, DataTable Data) PSMM_INV_SEARCH_ROW_CLICK(string PX_CUR_FORM = null, string PX_INV_CODE = null, string PX_INDEX = null, string PX_STR_FORM = null, string PX_FCT_CD_CODE = null, string PX_CAT = null, string PX_FIM_CMB_CLIENT = null, string PX_FCIR_CD = null, string PX_CM_INVESTORS = null, string PX_CM_BRANCH_NAMES = null, string PX_INV_BRANCH_NAME = null, string PX_FAR_AR_TYPE = null, string PX_FAR_AR_MY_OPT = null, string PX_FARREN_AR_TYPE = null, string PX_FPAY_CL_CD = null, string PX_FJV_CL_CD = null, string PX_FJV_AG_CD = null, string PX_FSYNC_REV_TR = null, string PX_FRM_SYNC_TM1 = null, string PX_FRM_SYNC_TM10 = null, string PX_FRM_SYNC_TM12 = null,      string V_chkIndia = null)
        {
            DataTable data = new DataTable();
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            using (OracleCommand cmd = new OracleCommand("WEALTHMAKER.PSMM_INV_SEARCH_ROW_DB_CLICK", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;

                // Add input parameters
                cmd.Parameters.Add("PX_CUR_FORM", OracleDbType.Varchar2).Value = PX_CUR_FORM ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_INV_CODE", OracleDbType.Varchar2).Value = PX_INV_CODE ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_INDEX", OracleDbType.Varchar2).Value = PX_INDEX ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_STR_FORM", OracleDbType.Varchar2).Value = PX_STR_FORM ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FCT_CD_CODE", OracleDbType.Varchar2).Value = PX_FCT_CD_CODE ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_CAT", OracleDbType.Varchar2).Value = PX_CAT ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FIM_CMB_CLIENT", OracleDbType.Varchar2).Value = PX_FIM_CMB_CLIENT ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FCIR_CD", OracleDbType.Varchar2).Value = PX_FCIR_CD ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_CM_INVESTORS", OracleDbType.Varchar2).Value = PX_CM_INVESTORS ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_CM_BRANCH_NAMES", OracleDbType.Varchar2).Value = PX_CM_BRANCH_NAMES ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_INV_BRANCH_NAME", OracleDbType.Varchar2).Value = PX_INV_BRANCH_NAME ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FAR_AR_TYPE", OracleDbType.Varchar2).Value = PX_FAR_AR_TYPE ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FAR_AR_MY_OPT", OracleDbType.Varchar2).Value = PX_FAR_AR_MY_OPT ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FARREN_AR_TYPE", OracleDbType.Varchar2).Value = PX_FARREN_AR_TYPE ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FPAY_CL_CD", OracleDbType.Varchar2).Value = PX_FPAY_CL_CD ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FJV_CL_CD", OracleDbType.Varchar2).Value = PX_FJV_CL_CD ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FJV_AG_CD", OracleDbType.Varchar2).Value = PX_FJV_AG_CD ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FSYNC_REV_TR", OracleDbType.Varchar2).Value = PX_FSYNC_REV_TR ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FRM_SYNC_TM1", OracleDbType.Varchar2).Value = PX_FRM_SYNC_TM1 ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FRM_SYNC_TM10", OracleDbType.Varchar2).Value = PX_FRM_SYNC_TM10 ?? (object)DBNull.Value;
                cmd.Parameters.Add("PX_FRM_SYNC_TM12", OracleDbType.Varchar2).Value = PX_FRM_SYNC_TM12 ?? (object)DBNull.Value;
                cmd.Parameters.Add("V_chkIndia", OracleDbType.Varchar2).Value = V_chkIndia ?? (object)DBNull.Value;

                // Output parameter
                cmd.Parameters.Add("PX_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["PX_CURSOR"].Value).GetDataReader())
                    {
                        if (reader.HasRows)
                        {
                            data.Load(reader);

                            if (data.Rows.Count > 0)
                            {
                                DataRow row = data.Rows[0];

                                // ✅ Check if MSG column exists
                                if (data.Columns.Contains("MSG"))
                                {
                                    string msgValue = row["MSG"]?.ToString() ?? "";

                                    if (msgValue.StartsWith("SUCCESS:", StringComparison.OrdinalIgnoreCase))
                                    {
                                        return (true, msgValue, data);
                                    }
                                    else
                                    {
                                        return (false, msgValue, null);
                                    }
                                }
                                else
                                {
                                    return (false, "Missing 'MSG' column in result set.", null);
                                }
                            }
                            else
                            {
                                return (false, "No rows returned from procedure.", null);
                            }
                        }
                        else
                        {
                            return (false, "No data returned from procedure.", null);
                        }
                    }
                }
                catch (OracleException ex)
                {
                    return (false, $"Database Error: {ex.Message}", null);
                }
                catch (Exception ex)
                {
                    return (false, $"System Error: {ex.Message}", null);
                }
            }
        }



        #endregion PSM MODAL: INVESTOR SEARCH TREE


        #region PSM MODAL: SCHEME SEARCH

        public DataTable PSMM_SCHEME_SEARCH_GET(string pxSchStr)
        {
            string loginId = currentLoginID();
            string roleId = currentRoleID();
            var dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSMM_SCH_SEARCH_GET", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("PX_SCH_STR", OracleDbType.Varchar2).Value = pxSchStr ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_LOG", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_ROLE", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;

                    // Output cursor
                    var outCursor = new OracleParameter("PX_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        return dt;
                    }
                    catch (OracleException ex)
                    {
                        // Log Oracle-specific errors
                        // Logger.Error("Oracle error in PSMM_SCHEME_SEARCH_GET", ex);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        // Log general errors
                        // Logger.Error("General error in PSMM_SCHEME_SEARCH_GET", ex);
                        return null;
                    }
                }
            }
        }

        public DataTable PSMM_SHEME_SEARCH_GET_ROW_DB(
    string pxFindState,
    string pxFrmStr,
    string pxAmcCol = null,
    string pxCol1 = null,
    string pxCol2 = null,
    string pxCol3 = null,
    string pxCol4 = null,
    string pxCol5 = null,
    string pxCol6 = null,
    string pxCol7 = null,
    string pxOther1 = null,
    string pxOther2 = null)
        {
            string loginId = currentLoginID();
            string roleId = currentRoleID();
            var dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSMM_SCH_SEARCH_GET_ROW_DB", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("PX_Find_state", OracleDbType.Varchar2).Value = pxFindState ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_FRM_STR", OracleDbType.Varchar2).Value = pxFrmStr ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_AMC_COL", OracleDbType.Varchar2).Value = pxAmcCol ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL1", OracleDbType.Varchar2).Value = pxCol1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL2", OracleDbType.Varchar2).Value = pxCol2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL3", OracleDbType.Varchar2).Value = pxCol3 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL4", OracleDbType.Varchar2).Value = pxCol4 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL5", OracleDbType.Varchar2).Value = pxCol5 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL6", OracleDbType.Varchar2).Value = pxCol6 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_COL7", OracleDbType.Varchar2).Value = pxCol7 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_OTHER1", OracleDbType.Varchar2).Value = pxOther1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_OTHER2", OracleDbType.Varchar2).Value = pxOther2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_LOG", OracleDbType.Varchar2).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_ROLE", OracleDbType.Varchar2).Value = roleId ?? (object)DBNull.Value;

                    var outCursor = new OracleParameter("PX_CURSOR", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outCursor);

                    try
                    {
                        conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }

                        return dt;
                    }
                    catch (OracleException ex)
                    {
                        // Log Oracle-specific errors
                        // Logger.Error("Oracle error in PSMM_SHEME_SEARCH_GET_ROW_DB", ex);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        // Log general errors
                        // Logger.Error("General error in PSMM_SHEME_SEARCH_GET_ROW_DB", ex);
                        return null;
                    }
                }
            }
        }
        #endregion

        #region WEB METHOD HELPTING FUNCTION

        public object ConvertDataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            return list;
        }



        public string DataTableToJsonWithSchema(DataTable dt)
        {
            try
            {
                // Check if the DataTable is null or has no rows
                if (dt == null || dt.Rows.Count == 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "ERROR: No data returned",
                        data = (object)null,
                        schema = (object)null,
                        totalCount = 0,
                        columns = 0
                    });
                }

                // Check if 'msg' column exists — this means it's not actual data
                if (dt.Columns.Contains("msg"))
                {
                    string msg = dt.Rows[0]["msg"]?.ToString().ToUpper();

                    // If msg is present and not empty
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            success = false,
                            message = "ERROR: " + msg,
                            data = (object)null,
                            schema = (object)null,
                            totalCount = 0,
                            columns = 0
                        });
                    }
                }

                // If no 'msg' column or it's empty, treat as successful data
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                List<object> schema = new List<object>();

                // Schema info
                foreach (DataColumn col in dt.Columns)
                {
                    schema.Add(new
                    {
                        columnName = col.ColumnName,
                        dataType = col.DataType.Name,
                        isNullable = col.AllowDBNull
                    });
                }

                // Row data
                foreach (DataRow dr in dt.Rows)
                {
                    var row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row[col.ColumnName] = dr[col] == DBNull.Value ? null : dr[col];
                    }
                    rows.Add(row);
                }

                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    message = "SUCCESS: Data retrieved successfully",
                    data = rows,
                    schema = schema,
                    totalCount = rows.Count,
                    columns = dt.Columns.Count
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "ERROR: " + ex.Message,
                    data = (object)null,
                    schema = (object)null,
                    totalCount = 0,
                    columns = 0
                });
            }
        }


        #endregion

        #region TABLE MASTER AND LIST MASTER

        public string PSM_DeleteTable(string frm,string tb,string col1 = null,string col2 = null,string col3 = null,string col4 = null,string col5 = null)
        {
            try
            {
                string msg = "";

                var parameters = new List<(string, OracleDbType, int, object, ParameterDirection)>
        {
            ("PX_LOGIN", OracleDbType.Varchar2, 50, currentLoginID(), ParameterDirection.Input),
            ("PX_ROLE", OracleDbType.Varchar2, 50, currentRoleID(), ParameterDirection.Input),
            ("PX_FRM_NAME", OracleDbType.Varchar2, 50, frm, ParameterDirection.Input),
            ("PX_TB_NAME", OracleDbType.Varchar2, 50, tb, ParameterDirection.Input),
            ("PX_COL1", OracleDbType.Varchar2, 50, col1, ParameterDirection.Input),
            ("PX_COL2", OracleDbType.Varchar2, 50, col2, ParameterDirection.Input),
            ("PX_COL3", OracleDbType.Varchar2, 50, col3, ParameterDirection.Input),
            ("PX_COL4", OracleDbType.Varchar2, 50, col4, ParameterDirection.Input),
            ("PX_COL5", OracleDbType.Varchar2, 50, col5, ParameterDirection.Input),
            ("PX_CURSOR", OracleDbType.RefCursor, 0, null, ParameterDirection.Output)
        };

                var result = PSM_ExecuteProcedure("PSM_LOG_DELETE_TABLE", parameters);

                if (result.success && result.data != null && result.data.Rows.Count > 0)
                {
                    msg = result.data.Rows[0][0]?.ToString() ?? "";
                    bool isSuccess = msg.StartsWith("SUCCESS:", StringComparison.OrdinalIgnoreCase);

                    return msg; 
                }

                return "ERROR: Unable to delete record.";
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        public (bool success, string message, DataTable data) PSM_ExecuteProcedure(string procedureName, List<(string paramName, OracleDbType paramType, int size, object value, ParameterDirection direction)> parameters)
        {

            /* Hot to use?
             
            
var parameters = new List<(string, OracleDbType, int, object, ParameterDirection)>
{
    ("PX_LATER_TYPE", OracleDbType.Varchar2, 1, letterType, ParameterDirection.Input),
    ("PX_MON_YEAR", OracleDbType.Varchar2, 7, monYear, ParameterDirection.Input),
    ("PX_LOGE", OracleDbType.Varchar2, 50, HttpContext.Current.Session["LoginId"]?.ToString(), ParameterDirection.Input),
    ("PX_ROLE", OracleDbType.Varchar2, 50, HttpContext.Current.Session["roleId"]?.ToString(), ParameterDirection.Input),
    ("PX_CURSOR", OracleDbType.RefCursor, 0, null, ParameterDirection.Output)
};

var helper = new PsmController();
var result = helper.PSM_ExecuteProcedure("PROC_NEW_DATA", parameters);

if (result.success)
{
    DataTable data = result.data;
    // Process your data
}
else
{
    string errorMsg = result.message;
}

             */
            DataTable dt = new DataTable();
            string msg = "No Data Found!";

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters dynamically
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                var oracleParam = new OracleParameter(param.paramName, param.paramType, param.size)
                                {
                                    Value = param.value ?? DBNull.Value,
                                    Direction = param.direction
                                };
                                cmd.Parameters.Add(oracleParam);
                            }
                        }

                        conn.Open();

                        // Check if procedure has a RefCursor output
                        bool hasRefCursor = parameters?.Exists(p => p.paramType == OracleDbType.RefCursor && p.direction == ParameterDirection.Output) ?? false;

                        if (hasRefCursor)
                        {
                            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                            {
                                da.Fill(dt);
                            }
                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    return (true, "Data found!", dt);
                }
                else
                {
                    return (false, msg, null);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }




        public DataTable PSM_LoggedTableList(string proc, string p_for, string p_by, string p_by_value)
        {
            string p_log_id = currentLoginID();
            string p_role_id = currentRoleID();

            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(proc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("P_FOR", OracleDbType.Varchar2).Value = p_for;
                    cmd.Parameters.Add("P_BY", OracleDbType.Varchar2).Value = p_by;
                    cmd.Parameters.Add("P_BY_VALUE", OracleDbType.Varchar2).Value = p_by_value;
                    cmd.Parameters.Add("P_LOG_ID", OracleDbType.Varchar2).Value = p_log_id;
                    cmd.Parameters.Add("P_ROLE_ID", OracleDbType.Varchar2).Value = p_role_id;

                    // Add output cursor
                    cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        if (conn.State != ConnectionState.Open) conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        
                        {
                            da.Fill(dt); // Load data from cursor into DataTable
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                        conn.Dispose();
                    }
                }
            }

            return dt;
        }


        public DataTable PSM_LogRMList(string proc, string p_for, string p_by, string p_by_value)
        {
            string p_log_id = currentLoginID();
            string p_role_id = currentRoleID();

            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(proc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("P_FOR", OracleDbType.Varchar2).Value = p_for;
                    cmd.Parameters.Add("P_BY", OracleDbType.Varchar2).Value = p_by;
                    cmd.Parameters.Add("P_BY_VALUE", OracleDbType.Varchar2).Value = p_by_value;
                    cmd.Parameters.Add("P_LOG_ID", OracleDbType.Varchar2).Value = p_log_id;
                    cmd.Parameters.Add("P_ROLE_ID", OracleDbType.Varchar2).Value = p_role_id;

                    // Add output cursor
                    cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        if (conn.State != ConnectionState.Open)conn.Open();

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Load data from cursor into DataTable
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                        conn.Dispose();
                    }
                }
            }

            return dt;
        }



        #endregion


        #region Text Formation

        public string RemoveAllSpaces(string input)
        {
            return string.IsNullOrEmpty(input)
                ? input
                : System.Text.RegularExpressions.Regex.Replace(input, @"\s+", "").ToLower();
        }

        public string ReplaceMultipleSpaceIntoSingleSpace(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return System.Text.RegularExpressions.Regex.Replace(input.Trim(), @"\s+", " ").ToLower();
        }


        #endregion



        #region DDL DropDown Helper Functions

        public void SetDataTableTODdl(DataTable data, DropDownList ddl, string col_name, string col_value, bool addDefault = true)
        {
            if (data.Rows.Count > 0 && data.Columns.Contains(col_name) && data.Columns.Contains(col_value))
            {
                ddl.DataSource = data;
                ddl.DataTextField = col_name;
                ddl.DataValueField = col_value;
                ddl.DataBind();

                if (addDefault)
                {
                    ddl.Items.Insert(0, new ListItem("", ""));
                }
            }
            else
            {
                ddl.Items.Clear();
            }
        }


        public void DdlItemFindByVlaue2(DropDownList ddl, string value, int elseSet, bool doEnable = true)
        {
            if (ddl.Items.FindByValue(value) != null)
            {
                ddl.SelectedValue = value;
                if (doEnable)
                {
                    ddl.Enabled = true;
                }

                ddl.Enabled = false;

            }
            else
            {
                if (elseSet != -1)
                {
                    ddl.SelectedIndex = elseSet;
                }
                else
                {
                    ddl.SelectedIndex = 0;  
                }

                if (doEnable)
                {
                    ddl.Enabled = true;
                }
            }

        }

        public void DdlItemFindByVlaue1(DropDownList ddl, string value )
        {
            if (ddl.Items.FindByValue(value) != null)
            {
                ddl.SelectedValue = value;
            }
        }

        #endregion




        #region DataTable Helpler

        public string GetRowStringDbNull(DataRow row, string columnName)
        {
            /* Working of this function
             * --> It will only handle if that row column have not data 
             * --> Bt this function will hanld even if the passign column name not exist in that datarow
            */

            if (row[columnName] != DBNull.Value)
            {
                return row[columnName].ToString();
            }
            return string.Empty;  
        }

        public static string GetRowStringDbNullIfExist(DataRow row, string columnName)
        {
            /* Working of this function
             * --> This funciton will handle if the row column have not any value
             * --> Also this function will hanld even if the passign column name not exist in that datarow
            */
            
            if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                return row[columnName].ToString();
            }
            return string.Empty;  
        }


        #endregion

        #region Validate pan

        public bool ValidatePan(string input)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                    return false;

                string pan = input.Trim().ToUpper();

                if (pan.Length != 10)
                    return false;

                // Check first 5 characters are letters
                for (int i = 0; i < 5; i++)
                {
                    if (!char.IsLetter(pan[i]) || !char.IsUpper(pan[i]))
                        return false;
                }

                // Check next 4 characters are digits
                for (int i = 5; i < 9; i++)
                {
                    if (!char.IsDigit(pan[i]))
                        return false;
                }

                // Check last character is an uppercase letter
                if (!char.IsLetter(pan[9]) || !char.IsUpper(pan[9]))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                // Optional: log or show error message
                Console.WriteLine("Unhandled error: " + ex.Message);
                return false;
            }
        }






        #endregion

        #region Page and Window


        public void CloseWindow(string redirectUrl = null)
        {
            Page page = HttpContext.Current.Handler as Page;
            string script = null;

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                script = $@"
        <script type='text/javascript'>
            window.onload = function () {{
                window.location.href = '{redirectUrl}';
                window.close();
            }};
        </script>";
            }
            else
            {
                script = $@"
        <script type='text/javascript'>
            window.onload = function () {{ 
                window.close();
            }};
        </script>";
            }

            // Detect if the postback is partial (inside UpdatePanel)
            if (ScriptManager.GetCurrent(page) != null && ScriptManager.GetCurrent(page).IsInAsyncPostBack)
            {
                // Partial postback
                ScriptManager.RegisterStartupScript(page, page.GetType(), "CloseWindowScript", script, false);
            }
            else
            {
                // Full postback
                page.ClientScript.RegisterStartupScript(page.GetType(), "CloseWindowScript", script, false);
            }
        }


        public void CloseWindowFullPostback(Page page, string redirectUrl = null)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = "DefaultPage.aspx";
            }

            string script = $@"
        <script type='text/javascript'>
            window.onload = function () {{
                window.location.href = '{redirectUrl}';
                window.close();
            }};
        </script>";

            page.ClientScript.RegisterStartupScript(page.GetType(), "CloseWindowScriptFull", script, false);
        }

        public void CloseWindowPartialPostback(Page page, string redirectUrl = null)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = "DefaultPage.aspx";
            }

            string script = $@"
        <script type='text/javascript'>
            window.onload = function () {{
                window.close();
            }};
        </script>";

            ScriptManager.RegisterStartupScript(page, page.GetType(), "CloseWindowScriptPartial", script, false);
        }



        public (bool, string) OpenUrl(string url, string openMode = "popup")
        {
            /*How to use this function 
            var result = PSMController.OpenUrl("../Tree/frm_tree_mf?SEARCH_TYPE=mf_view_inv", "popup");
            if (result.Item1)
            { 
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenDynamicWindow", result.Item2, true);
            }
            else
            { 
                pc.ShowAlert(this, result.Item2);
                return;
            }
             */
            
            try
            {
                string script = string.Empty;
                if (string.IsNullOrWhiteSpace(url))
                {
                    script = "URL cannot be empty.";
                    return (false, script);
                }
                switch (openMode.ToLower())
                {
                    case "same":
                        script = $"window.location.href = '{url}';";
                        break;

                    case "new":
                        script = $"window.open('{url}', '_blank');";
                        break;

                    case "popup":
                        script = $"window.open('{url}', '_blank', 'width=1000,height=700,resizable=yes');";
                        break;

                    default:
                        script = $"window.open('{url}', '_blank', 'width=1000,height=700,resizable=yes');";
                        break;
                }
                return (true, script);

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        #endregion

        #region Common Interface functoins

        public void InvestorAddressUpdatePopup(string inv_code)
        {
            Page page = HttpContext.Current.Handler as Page;

            string script = @"
        <script type='text/javascript'>
            window.onload = function () {
                window.open(
                    'InvestorAddressUpdate.aspx',
                    'InvestorPopup',
                    'width=700,height=350,resizable=yes,scrollbars=yes'
                );
            };
        </script>";

            HttpContext.Current.Session["PSM_IAU_INV_CODE"] = inv_code;
            RunClientScript(page, "PSM_IAU_POPUP_WINDOW", script);
        }





        // investor address udpate  update investor by inv_code
        public DataTable PsmUidByInv(
    string invCode, string action, string type, 
    string add1 = null, string add2 = null, string pin = null,
    string state = null, string city = null, string email = null, string mobile = null,
    string pan = null, string aadhar = null, string dob = null
)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_UID_BY_INV", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add input parameters
                        cmd.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = currentLoginID();
                        cmd.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = currentRoleID();
                        cmd.Parameters.Add("P_INV", OracleDbType.Varchar2).Value = invCode;
                        cmd.Parameters.Add("P_ACTION", OracleDbType.Varchar2).Value = action;
                        cmd.Parameters.Add("P_TYPE", OracleDbType.Varchar2).Value = type;

                        cmd.Parameters.Add("P_ADD1", OracleDbType.Varchar2).Value = DBNullString(add1);
                        cmd.Parameters.Add("P_ADD2", OracleDbType.Varchar2).Value = DBNullString(add2);
                        cmd.Parameters.Add("P_PIN", OracleDbType.Varchar2).Value = DBNullString(pin);
                        cmd.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = DBNullString(email);
                        cmd.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = DBNullString(mobile);
                        cmd.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = DBNullString(pan);
                        cmd.Parameters.Add("P_AADHAR", OracleDbType.Varchar2).Value = DBNullString(aadhar);
                        cmd.Parameters.Add("P_STATE_ID", OracleDbType.Varchar2).Value = DBNullString(state);
                        cmd.Parameters.Add("P_CITY_ID", OracleDbType.Varchar2).Value = DBNullString(city);
                        cmd.Parameters.Add("P_DOB", OracleDbType.Varchar2).Value = DBNullString(dob);

                        // Add output cursor
                        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        conn.Open();

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);  // Fill the result into DataTable
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PsmUidByInv: " + ex.Message);
            }

            return dt;
        }


        #endregion


        #region TABLE MASTER LIST



        public bool CleanTable(string tableName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(tableName))
            {
                errorMessage = "Table name cannot be empty.";
                return false;
            }

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    conn.Open();

                    // Use TRUNCATE for better performance (cannot rollback)
                    using (OracleCommand cmd = new OracleCommand($"TRUNCATE TABLE {tableName}", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Error cleaning table {tableName}: {ex.Message}";
                    return false;
                }
            }
        }

        public bool BulkInsertToOracleDAP(DataTable data, string destinationTable, out string error)
        {
            error = null;

            try
            {
                using (var conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    conn.Open();

                    using (var bulkCopy = new OracleBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = destinationTable;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.BulkCopyTimeout = 300; // <-- Extend timeout to 5 mins

                        foreach (DataColumn col in data.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.NotifyAfter = 1000;
                        bulkCopy.OracleRowsCopied += (s, e) =>
                        {
                        };

                        bulkCopy.WriteToServer(data);
                    }

                    conn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }




        public bool BulkInsertToTable(DataTable dt, string destinationTableName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (dt == null || dt.Rows.Count == 0)
            {
                errorMessage = "DataTable is null or empty.";
                return false;
            }

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    conn.Open();

                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = destinationTableName;

                        foreach (DataColumn col in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.WriteToServer(dt);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = "Bulk insert failed: " + ex.Message;
                    return false;
                }
            }
        }

        public bool BulkInsertToTable_Upcoming(DataTable dt, string destinationTableName, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate DataTable
            if (dt == null || dt.Rows.Count == 0)
            {
                errorMessage = "DataTable is null or empty.";
                return false;
            }

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    conn.Open();

                    // Get destination table columns
                    var destinationColumns = new List<string>();
                    using (var cmd = new OracleCommand($"SELECT column_name FROM all_tab_columns WHERE table_name = '{destinationTableName.ToUpper()}'", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            destinationColumns.Add(reader.GetString(0));
                        }
                    }

                    // Validate column matching
                    var missingColumns = new List<string>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (!destinationColumns.Contains(col.ColumnName.ToUpper()))
                        {
                            missingColumns.Add(col.ColumnName);
                        }
                    }

                    if (missingColumns.Count > 0)
                    {
                        errorMessage = $"Destination table doesn't contain these columns: {string.Join(", ", missingColumns)}";
                        return false;
                    }

                    // Perform bulk insert
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = destinationTableName;

                        foreach (DataColumn col in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.WriteToServer(dt);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = "Bulk insert failed: " + ex.Message;
                    return false;
                }
            }
        }

        public DataTable GetColumnsName(string frmName, string dataType)
        {
            DataTable dt = new DataTable();
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_GET_COLUMN_NAMES", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("P_FRM", OracleDbType.Varchar2).Value = frmName;
                        command.Parameters.Add("P_DATA_TYPE", OracleDbType.Varchar2).Value = dataType;
                        command.Parameters.Add("P_COLUMNS", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error in GetColumnsName: " + ex.Message);
            }
            return dt;
        }

        public DataTable BranchMasterTable()
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand com = new OracleCommand("PSM_BRANCH_MASTER", conn))
                    {
                        string curerntLog = currentLoginID();
                        string currentRole = currentRoleID();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.Add("p_log_id", OracleDbType.Varchar2).Value = curerntLog;
                        com.Parameters.Add("p_role_id", OracleDbType.Varchar2).Value = currentRole;
                        com.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(com))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BranchMasterTable: " + ex.Message);
            }
        }

        // BranchMasterTable will return all the record of branch_master
        // but in new dt fucoint i have to return only htis "WHERE 1 = 1 AND B.BRANCH_CODE IN(SELECT BRANCH_ID FROM WEALTHMAKER.USERDETAILS_JI WHERE LOGIN_ID=P_LOG_ID AND ROLE_ID=P_ROLE_ID)"

        public DataTable LoggedBranchMaster()
        {
            try
            {
                DataTable dt = new DataTable();
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand com = new OracleCommand("PSM_LOGGED_BRANCH_MASTER", conn))
                    {
                        string curerntLog = currentLoginID();
                        string currentRole = currentRoleID();
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.Add("p_log_id", OracleDbType.Varchar2).Value = curerntLog;
                        com.Parameters.Add("p_role_id", OracleDbType.Varchar2).Value = currentRole;
                        com.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(com))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BranchMasterTable: " + ex.Message);
            }
        }


        public void setDdlLoggedBranchList(DropDownList ddl, bool setDefault = false)
        {
            
            DataTable dt = LoggedBranchMaster();
            List<string> branchCodes = dt.AsEnumerable().Select(row => row.Field<string>("BRANCH_CODE")).ToList();



            AddDdlItemByItemsList(ddl, branchCodes, setDefault);
        }


        public DataTable BankMaster()
        {
            DataTable dt = new DataTable();
            try
            {
                // Create a connection string variable
                string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

                // Create a connection to the Oracle database
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    // Create a command to execute the stored procedure
                    using (OracleCommand command = new OracleCommand("PSM_BANK_MASTER", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Define the cursor parameter
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "P_CURSOR",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        // Open the connection
                        conn.Open();

                        // Use OracleDataAdapter to fill the DataTable
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                //Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        
        
         


        #endregion

        #region DDL GET SET

        public void AddDdlItemByItemsList(DropDownList ddl, List<string> items, bool setDefault = false)
        {
            ddl.Items.Clear();  

            if (setDefault)
            {
                ddl.Items.Add(new ListItem("--Select--", "")); // Optional default item
            }

            foreach (string item in items)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    ddl.Items.Add(new ListItem(item, item.Replace(" ", "_")));
                }
            }
        }


        public List<string> DdlValueToList(DropDownList ddl)
        {
            /* How to use this function
             * 
             * List<string> ddlValues = DdlValueToList(myDropDownList);
             * 
             * This will return a list of values from the DropDownList.
             */


            List<string> result = new List<string>();

            foreach (ListItem item in ddl.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Value))
                {
                    result.Add(item.Value.Trim());
                }
            }

            return result;
        }


        public List<string> DdlTextToList(DropDownList ddl)
        {
            /* How to use this function
             * 
             * List<string> ddlTexts = DdlTextToList(myDropDownList);
             * 
             * This will return a list of texts from the DropDownList.
             */


            List<string> result = new List<string>();

            foreach (ListItem item in ddl.Items)
            {
                if (!string.IsNullOrWhiteSpace(item.Value))
                {
                    result.Add(item.Text.Trim());
                }
            }

            return result;
        }

        public void ListToDdl(DropDownList ddl, List<string> textList, List<string> valueList = null, bool checkMatch = false)
        {
            /* How to use this function 
             * 
             * List<string> textList = new List<string> { "Item 1", "Item 2", "Item 3" };
             * List<string> valueList = new List<string> { "Value1", "Value2", "Value3" };
             * ListToDdl(dbFieldFromDb1, textList, valueList, checkMatch: true);
             * 
             * List<string> textList = new List<string> { "Item 1", "Item 2", "Item 3" };
             * ListToDdl(dbFieldFromDb1, textList, checkMatch: false);
             */

            ddl.Items.Clear(); // Clear any existing items

            // If checkMatch is true, ensure the lists match in length and content
            if (checkMatch)
            {
                // Ensure both lists are of the same size or handle mismatch
                int maxLength = Math.Max(textList.Count, valueList?.Count ?? 0);

                for (int i = 0; i < maxLength; i++)
                {
                    string text = (i < textList.Count) ? textList[i].Trim() : null;
                    string value = (i < valueList.Count) ? valueList[i].Trim() : null;

                    // If either text or value is null or whitespace, skip this item
                    if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value))
                    {
                        ListItem item = new ListItem(text, value);
                        ddl.Items.Add(item);
                    }
                    else
                    {
                        // Add a blank or placeholder item if mismatch is detected
                        ddl.Items.Add(new ListItem(text ?? "", value ?? ""));
                    }
                }
            }
            else
            {
                // Regular adding when not checking for match
                if (valueList == null)
                {
                    foreach (string text in textList)
                    {
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            ListItem item = new ListItem(text.Trim(), text.Trim());
                            ddl.Items.Add(item); // Add Text and Value as same
                        }
                    }
                }
                else
                {
                    if (textList.Count == valueList.Count)
                    {
                        for (int i = 0; i < textList.Count; i++)
                        {
                            string text = textList[i].Trim();
                            string value = valueList[i].Trim();

                            if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value))
                            {
                                ListItem item = new ListItem(text, value);
                                ddl.Items.Add(item);
                            }
                            else
                            {
                                // Add a blank item if text or value is empty
                                ddl.Items.Add(new ListItem("", ""));
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Text and Value lists must have the same number of items.");
                    }
                }
            }
        }

        public DropDownList CreateDropDownFromList(List<string> sourceList)
        {
            /* How to use this function
             * 
             * List<string> sourceList = new List<string> { "Item 1", "Item 2", "Item 3" };
             * DropDownList ddl = CreateDropDownFromList(sourceList);
             * 
             * This will create a DropDownList with the items from the source list.
             */
            DropDownList ddl = new DropDownList();

            foreach (string item in sourceList)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    ddl.Items.Add(new ListItem(item.Trim(), item.Trim()));
                }
            }

            return ddl;
        }

        public DropDownList CreateDropDownFromMask(string csvItems)
        {
            /* How to use this function
             * 
             * string csvItems = "Item 1, Item 2, Item 3";
             * DropDownList ddl = CreateDropDownFromMask(csvItems);
             * 
             * This will create a DropDownList with the items from the CSV string.
             */
            DropDownList ddl = new DropDownList();

            if (!string.IsNullOrWhiteSpace(csvItems))
            {
                string[] items = csvItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in items)
                {
                    string trimmed = item.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        ddl.Items.Add(new ListItem(trimmed, trimmed));
                    }
                }
            }

            return ddl;
        }


        public List<string> GetSplitedMaskList(List<string> inputList, int index)
        {
            /* How to use this function
             * 
             * List<string> inputList = new List<string> { "Field1 --> DBField1", "Field2 --> DBField2" };
             * int index = 0; // 0 for Excel field, 1 for DB field
             * List<string> result = GetSplitedMaskList(inputList, index);
             * 
             * This will return a list of either Excel or DB fields based on the index.
             */
            List<string> extractedParts = new List<string>();

            foreach (string item in inputList)
            {
                // Split the item based on '-->'
                string[] parts = item.Split(new string[] { "-->" }, StringSplitOptions.None);

                // If the split resulted in exactly two parts, process them
                if (parts.Length == 2)
                {
                    // If index is valid, add the corresponding part
                    if (index == 0) // Excel field (part before -->)
                    {
                        extractedParts.Add(parts[0].Trim());
                    }
                    else if (index == 1) // DB field (part after -->)
                    {
                        extractedParts.Add(parts[1].Trim());
                    }
                    else
                    {
                        // If an invalid index is passed, you can handle it or add an empty string
                        extractedParts.Add(string.Empty);
                    }
                }
            }

            return extractedParts;
        }


        public string[] GetMaskedSplitedString(string mask)
        {
            return mask.Split(new string[] { "-->" }, StringSplitOptions.None);
        }


        public List<string[]> GetListSplitWithCommaOfMaskedString(string maskedString)
        {
            List<string[]> result = new List<string[]>();

            if (!string.IsNullOrWhiteSpace(maskedString))
            {
                // Split by comma to get individual mask items
                string[] items = maskedString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in items)
                {
                    // Split each item by '-->' and add if valid
                    var parts = item.Split(new[] { "-->" }, StringSplitOptions.None);
                    if (parts.Length == 2)
                    {
                        result.Add(new string[] { parts[0].Trim(), parts[1].Trim() });
                    }
                }
            }

            return result;
        }


        public List<string> GetCommaSplittedListOfMask(string input)
        {
            /* How to use this function
             * 
             * string input = "Item 1, Item 2, Item 3";
             * List<string> result = GetCommaSplittedListOfMask(input);
             * 
             * This will return a list of items from the comma-separated string.
             */
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return input
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrEmpty(item))
                .ToList();
        }



        public List<T> ListToList<T>(List<T> listA)
        {
            /* How to use this function
             * List<int> listA = new List<int> { 1, 2, 3, 4, 5 };
             * 
             * Call the function to copy listA to listB
             * List<int> listB = ListToList(listA);
             */

            List<T> listB = new List<T>();
            foreach (var item in listA)
            {
                listB.Add(item);  // Or apply a transformation here if needed
            }

            return listB;
        }


        public static void SafeSelectDropdownByValue(DropDownList ddl, string valueToSelect)
        {
            if (ddl == null || ddl.Items.Count == 0)
                return;

            string targetValue = valueToSelect?.Trim();

            ListItem foundItem = ddl.Items.FindByValue(targetValue);

            if (foundItem != null)
            {
                ddl.ClearSelection();
                foundItem.Selected = true;
            }
            else
            {
                ddl.SelectedIndex = 0;
            }
        }



        public static void SafeSelectDropdownByText(DropDownList ddl, string valueToSelect)
        {
            if (ddl == null || ddl.Items.Count == 0)
                return;

            string target = valueToSelect?.Trim().ToUpper();

            ListItem foundItem = ddl.Items.FindByText(target);

            if (foundItem != null)
            {
                ddl.ClearSelection();
                foundItem.Selected = true;
            }
            else
            {
                ddl.SelectedIndex = 0;
            }
        }



        public void SetDdlByText(string input, DropDownList ddl, bool doInsert = false)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                ListItem matchedItem = ddl.Items
                    .Cast<ListItem>()
                    .FirstOrDefault(item =>
                        string.Equals(item.Value.ToUpper(), input.ToUpper(), StringComparison.OrdinalIgnoreCase));

                if (matchedItem != null)
                {
                    ddl.ClearSelection();
                    matchedItem.Selected = true;
                }

                if (doInsert)
                {
                    ddl.Items.Add(new ListItem(input, input));
                    ddl.SelectedValue = input;
                }
            }
            else
            {
                if (ddl.Items.Count > 0)
                {
                    ddl.SelectedIndex = -1;
                }
            }
        }





        #endregion

        #region DB and UI NullHandler

        public static bool IsContainAndNotNull(DataRow row, string columnName)
        {
            if (row == null || row.Table == null)
                return false;

            return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value;
        }

        public static bool IsContainInRow(DataRow row, string columnName)
        {
            if (row == null || row.Table == null)
                return false;

            return row.Table.Columns.Contains(columnName);
        }


        public static string GetPaymentModeFullName(string code)
        {
            switch (code.ToUpper())
            {
                case "C":
                    return "Cheque";
                case "D":
                    return "Draft";
                case "H":
                    return "Cash";
                case "E":
                    return "ECS";
                case "M":
                    return "Corporate NECS";
                case "R":
                    return "Others";
                default:
                    return "";
            }
        }



        public static DateTime ParseDateMaster(object value, string format = "dd/MM/yyyy", DateTime? fallback = null)
        {
            if (value == null || value == DBNull.Value)
                return fallback ?? DateTime.MinValue;

            string dateStr = value.ToString()?.Trim();

            if (string.IsNullOrEmpty(dateStr))
                return fallback ?? DateTime.MinValue;

            if (DateTime.TryParseExact(
                    dateStr,
                    format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
            {
                return parsedDate;
            }

            return fallback ?? DateTime.MinValue;
        }


        public decimal DBSqlGetDecimalParameterValue(SqlParameterCollection parameters, string parameterName, decimal elsePart = 0)
        {
            var value = parameters[parameterName]?.Value;
            return value != DBNull.Value ? Convert.ToDecimal(value) : 0;
        }

        public decimal DBOrcGetDecimalParameterValue(OracleParameterCollection parameters, string parameterName, decimal elsePart = 0) 
        {
            var value = parameters[parameterName]?.Value;
            return value != DBNull.Value ? Convert.ToDecimal(value) : elsePart;
        }

        public static int DBOrcGetInt32ParameterValue(OracleParameterCollection parameters, string parameterName, int elsePart = 0)
        {
            var value = parameters[parameterName]?.Value;
            return value != DBNull.Value ? Convert.ToInt32(value) : elsePart;
        }

        public static long DBOrcGetInt64ParameterValue(OracleParameterCollection parameters, string parameterName, long elsePart = 0L)
        {
            var value = parameters[parameterName]?.Value;
            return value != DBNull.Value ? Convert.ToInt64(value) : elsePart;
        }

        public static string DBOrcGetStringParameterValue(OracleParameterCollection parameters, string parameterName, string elsePart = "")
        {
            var value = parameters[parameterName]?.Value;
            return value != DBNull.Value ? value.ToString() : (string.IsNullOrEmpty(elsePart)? string.Empty: elsePart);
        }


        public string ToUpperTrimString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty; // Return empty string if input is null or empty
            }

            return input.ToUpper().Trim(); // Convert to uppercase and trim any whitespace
        }

        public bool isNullString(string value)
        {
            /* How to use this function
             * 
             * string value = "NULL";
             * bool isNull = isNullString(value);
             * 
             * This will return true if the value is null, "NULL", "N/A", or "NOT APPLICABLE".
             */
            return string.IsNullOrWhiteSpace(value) || value.ToUpper() == "NULL" || value.ToUpper() == "N/A" || value.ToUpper() == "NOT APPLICABLE";
        }

        public bool isNotNullString(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.ToUpper() != "NULL" && value.ToUpper() != "N/A" && value.ToUpper() != "NOT APPLICABLE";
        }



        public string ReplaceThis(string input, string replaceThis, string replaceWith = "")
        {

            /*
             * How to use this function
             * string cleaned1 = ReplaceSpecialCharacters("[Policy-No]", "[,],-", "");  // Output: PolicyNo
             * string cleaned2 = ReplaceSpecialCharacters("[Policy-No]", "[,],-", "_");  // Output: _Policy_No_
             */





            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(replaceThis))
                return input;

            // Split the replace list by comma
            string[] charsToReplace = replaceThis.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in charsToReplace)
            {
                string toReplace = item.Trim();
                if (!string.IsNullOrEmpty(toReplace))
                {
                    input = input.Replace(toReplace, replaceWith);
                }
            }

            return input;
        }



        public object NullHandlerStringUiToDb(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? DBNull.Value : (object)value;
        }

        public string SafeParseString(string input)
        {
            string value = "";

            if (!string.IsNullOrEmpty(input))
            {
                value = input;
            }

            return value;
        }

        public long SafeParseLong(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (long.TryParse(input, out long result))
            {
                return result;
            }

            return 0;
        }

        public double SafeParseDouble(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (double.TryParse(input, out double result))
            {
                return result;
            }

            return 0;
        }


        public decimal SafeParsedecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (decimal.TryParse(input, out decimal result))
            {
                return result;
            }

            return 0;
        }

        public int SafeParseInd(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (int.TryParse(input, out int result))
            {
                return result;
            }

            return 0;
        }


        public string SafeParseCheckBoxZeroOne(bool isChecked)
        {
            return isChecked ? "1" : "0";
        }

        public string SafeParseCheckBoxYesNo(bool isChecked)
        {
            return isChecked ? "Y" : "N";
        }



        #endregion

        #region FILL FUNCTIONS

        // WEALTHMAKER.PSM_FILL_DDL_MASTER
        // A FUCIOTN TO RETRIEVE DATA FROM 'WEALTHMAKER.PSM_FILL_DDL_MASTER' PROCEDURE AND RETURN DATATABELR
        //public DataTable PSM_DDL_FILL_MASTER(string procedureName = "WEALTHMAKER.PSM_FILL_DDL_MASTER", string[] parameters, out int rowCount, out string exception)
        public DataTable PSM_DDL_FILL_MASTER(PsmModel pm, DropDownList ddl = null, bool isFirst = true)
        {
            pm.procedureName = "WEALTHMAKER.PSM_FILL_DDL_MASTER";
            //pm.parameters = new string[] { pm.LOGGEDUSERID, pm.ROLE_ID, pm.REGION_ID, pm.ZONE_ID, pm.BRANCH_CODE, pm.RM_CODE, pm.LIST_TYPE };
            

            DataTable dt = new DataTable();
            pm.rowCount = 0; // Default row count
            pm.exception = null; // Default exception as null

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(pm.procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Set command type to stored procedure

                    // Add parameters if they exist
                    if (pm.parameters != null && pm.parameters.Length > 0)
                    {
                        for (int i = 0; i < pm.parameters.Length; i++)
                        {
                            cmd.Parameters.Add("P_VALUE" + (i + 1), OracleDbType.Varchar2).Value = pm.parameters[i];
                        }
                    }

                    // add two currors param
                    cmd.Parameters.Add("P_CURSOR1", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("P_CURSOR2", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {                      

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {

                            adapter.Fill(dt); // Fill the DataTable with the result of the procedure
                            pm.rowCount = dt.Rows.Count; // Get the row count
                        }

                        string text = "FIELD_NAME";
                        string value = "FIELD_VALUE";
                        if (dt != null && dt.Rows.Count > 0 &&
                            dt.Columns.Contains(text) && dt.Columns.Contains(value))
                        {
                            pm.ddlText = dt.AsEnumerable()
                                           .Select(row => row[text].ToString())
                                           .ToArray();
                            pm.ddlValues = dt.AsEnumerable()
                                             .Select(row => row[value].ToString())
                                             .ToArray();

                            if (ddl != null)
                            {
                                ddl.DataSource = dt;
                                ddl.DataTextField = text;
                                ddl.DataValueField = value;
                                ddl.DataBind();
                                if (isFirst)
                                {
                                    ddl.Items.Insert(0, new ListItem("Select", ""));
                                }
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Capture exception message
                        pm.exception = ex.Message;

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

        public static void FillDropDownWithLogBranch(string tableName, string textField, string valueField, string whereClause, DropDownList ddlToSet, Page page = null, Label messageLabel = null)
        {
            PsmController pc = new PsmController();

            try
            {
                // Build the SQL query
                string sql = $@"
            SELECT {textField} AS name, 
                   {valueField} AS value 
            FROM {tableName}";

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    sql += $" {whereClause}";
                }

                // Add ORDER BY on the textField (display field)
                sql += $" ORDER BY {textField}";


                // make an object of PsmController

                DataTable dt = pc.ExecuteCurrentQueryMaster(sql, out int rowCount, out string isException);

                if (dt != null && dt.Rows.Count > 0 && string.IsNullOrEmpty(isException))
                {
                    ddlToSet.DataSource = dt;
                    ddlToSet.DataTextField = "name";
                    ddlToSet.DataValueField = "value";
                    ddlToSet.DataBind();
                    // Add default "Select" option at the top
                    ddlToSet.Items.Insert(0, new ListItem("Select", ""));

                }
                else
                {
                    ddlToSet.Items.Clear();
                    ddlToSet.Items.Add(new ListItem("-- No data found --", ""));

                    if (page != null)
                        pc.ShowAlert(page, "No data found for the selected input.");
                    if (messageLabel != null)
                        messageLabel.Text = "No data found.";
                }
            }
            catch (Exception ex)
            {
                ddlToSet.Items.Clear();

                if (page != null)
                    pc.ShowAlert(page, "Error: " + ex.Message);
                if (messageLabel != null)
                    messageLabel.Text = "Error: " + ex.Message;
            }
        }

        public static void FillDropDown(string tableName, string textField, string valueField, string whereClause, DropDownList ddlToSet, Page page = null, Label messageLabel = null)
        {
            PsmController pc = new PsmController();

            try
            {
                // Build the SQL query
                string sql = $@"
            SELECT {textField} AS name, 
                   {valueField} AS value 
            FROM {tableName}";

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    sql += $" {whereClause}";
                }

                // Add ORDER BY on the textField (display field)
                sql += $" ORDER BY {textField}";


                // make an object of PsmController

                DataTable dt = pc.ExecuteCurrentQueryMaster(sql, out int rowCount, out string isException);

                if (dt != null && dt.Rows.Count > 0 && string.IsNullOrEmpty(isException))
                {
                    ddlToSet.DataSource = dt;
                    ddlToSet.DataTextField = "name";
                    ddlToSet.DataValueField = "value";
                    ddlToSet.DataBind();
                    // Add default "Select" option at the top
                    ddlToSet.Items.Insert(0, new ListItem("Select", ""));
                    ddlToSet.Enabled = true;

                }
                else
                {
                    ddlToSet.Items.Clear();

                    if (page != null)
                    {
                        ddlToSet.Items.Add(new ListItem("-- No data found --", ""));
                        pc.ShowAlert(page, "No data found for the selected input.");
                    }
                    if (messageLabel != null)
                    { messageLabel.Text = "No data found."; }

                    ddlToSet.Enabled = false;

                }
            }
            catch (Exception ex)
            {
                ddlToSet.Items.Clear();

                if (page != null)
                    pc.ShowAlert(page, "Error: " + ex.Message);
                if (messageLabel != null)
                    messageLabel.Text = "Error: " + ex.Message;
            }
        }


        #endregion

        #region Datatable Functions

        public string DtToUpperTrim(DataRow row, string columnName)
        {
            // Check if the column exists and has a value
            if (row != null && row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                // Retrieve the value, convert to string, trim, and convert to uppercase
                return row[columnName].ToString().ToUpper().Trim();
            }

            // Return empty string if column doesn't exist or value is DBNull
            return string.Empty;
        }

        public bool Dt_DoesColumnExistInRow(DataRow row, string columnName)
        {
            // Check if the row's table contains the column, ignoring case
            return row.Table.Columns.Cast<DataColumn>()
                                     .Any(col => string.Equals(col.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
        }


        public static List<string> GetDataTableHeaders(DataTable currentSheetData, object outputTarget = null)
        {
            /* How  TO Use 
            
            Get Header:
            var headers = PsmController.GetExcelHeadersFromDataTable(currentDataTable);

            Get and Set Headers to DDl
            var headers = PsmController.GetExcelHeadersFromDataTable(currentDataTable, myDropDownList);

            Get and Set Headers to Text fields of Label
            var headers = PsmController.GetExcelHeadersFromDataTable(currentDataTable, "out");
            lblHeaders.Text = string.Join(", ", headers);

             */
            
            
            List<string> headers = new List<string>();

            // Collect column names
            foreach (DataColumn column in currentSheetData.Columns)
            {
                headers.Add(column.ColumnName);
            }

            // If outputTarget is a DropDownList, populate it
            if (outputTarget is DropDownList ddl)
            {
                ddl.Items.Clear();
                foreach (string header in headers)
                {
                    ddl.Items.Add(new ListItem(header));
                }
            }
            // If outputTarget is the string "out", you might handle it elsewhere or return a string
            else if (outputTarget is string str && str.Equals("out", StringComparison.OrdinalIgnoreCase))
            {
                // You could optionally return a comma-separated version or store it elsewhere
                string headerString = string.Join(", ", headers); 
            }

            // Always return the list of headers
            return headers;
        }


        #endregion

        #region Global Mapping Functions

        public static void MaskToList(
     string mappingString,
     out List<string> upExFld,
     out List<string> upDbFld)
        {
            upExFld = new List<string>();
            upDbFld = new List<string>();

            if (string.IsNullOrWhiteSpace(mappingString))
                return;

            var mappings = mappingString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var map in mappings)
            {
                var pair = map.Split(new[] { "-->" }, StringSplitOptions.None);
                if (pair.Length == 2)
                {
                    string excelField = pair[0]
                        .Replace("[", "")
                        .Replace("]", "")
                        .Replace("-", "")
                        .Trim();

                    string dbField = pair[1].Trim();

                    if (!string.IsNullOrEmpty(excelField) && !string.IsNullOrEmpty(dbField))
                    {
                        upExFld.Add(excelField);
                        upDbFld.Add(dbField);
                    }
                }
            }
        }

        public bool GetMappedFields(string mappingFor, string mappingType,out string getMask, out List<string> mappedExcelFields, out List<string> mappedDbFields)
        {

            /* --== How to use this function ==--
             * 
             * List<string> excelFieldFromDb = new List<string>();
             * List<string> dbFieldFromDb = new List<string>();
             * string mappingFor = "DUE_AND_DATA";
             * string mappingType = "DUE";
             * string isMaskFound = pc.GetMappedFields(mappingFor, mappingType, out excelFieldFromDb, out dbFieldFromDb);
             */

            getMask = string.Empty;
            mappedExcelFields = new List<string>();
            mappedDbFields = new List<string>();


            mappedExcelFields = new List<string>();
            mappedDbFields = new List<string>();

            string getOldMapped = $@"
        SELECT 
            FIELD_NAME AS TEXT, 
            FIELD_VALUE AS VALUE 
        FROM PSM_GLOBAL_WORKING_DATA 
        WHERE UPPER(DATA_TYPE_1) = 'MAPPED_MASK' 
            AND UPPER(DATA_TYPE_2) = UPPER('{mappingFor}')
            AND UPPER(DATA_TYPE_3) = UPPER('{mappingType}')
            AND ROWNUM = 1
            ";
            DataTable dt = ExecuteCurrentQueryMaster(getOldMapped, out int rc, out string ie);

            if (rc > 0 && string.IsNullOrEmpty(ie) && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                // You can choose either TEXT or VALUE, or merge both if needed
                string mappingsRaw = row["TEXT"]?.ToString(); // or row["VALUE"]
                getMask = row["TEXT"]?.ToString();
                if (!string.IsNullOrEmpty(mappingsRaw))
                {
                    string[] mappings = mappingsRaw.Split(',');
                    foreach (string pair in mappings)
                    {
                        string cleaned = pair.Trim();
                        if (cleaned.Contains("-->"))
                        {
                            int arrowIndex = cleaned.IndexOf("-->");
                            string excelField = cleaned.Substring(0, arrowIndex).Trim().TrimStart('[').TrimEnd(']');
                            string dbField = cleaned.Substring(arrowIndex + 3).Trim();

                            mappedExcelFields.Add(excelField);
                            mappedDbFields.Add(dbField);
                        }
                    }

                    return mappedExcelFields.Count > 0 && mappedDbFields.Count > 0;
                }
            }

            return false;
        }


        #endregion

        #region EXCEL HANDLER FUNCTIONS

        public bool BulkInsertToOracle(DataTable data, string destinationTable, out string error)
        {
            error = null;

            try
            {
                using (var conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    conn.Open();

                    using (var bulkCopy = new OracleBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = destinationTable;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.BulkCopyTimeout = 300; // <-- Extend timeout to 5 mins

                        foreach (DataColumn col in data.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.NotifyAfter = 1000;
                        bulkCopy.OracleRowsCopied += (s, e) =>
                        {
                            Console.WriteLine($"{e.RowsCopied} rows copied.");
                        };

                        bulkCopy.WriteToServer(data);
                    }

                    conn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }



        public (bool, string) DeletFromTable(string tableName)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new OracleCommand($"DELETE FROM {tableName}", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close(); // ✅ Explicit close for clarity (even though using handles it)
                }

                return (true, "Temporary table cleared successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error clearing temporary table: {ex.Message}");
            }
        }

        // 1. EXCEL CONNECTION
        public static string GetExcelConnectionString(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            string connectionString = string.Empty;

            // Ensure backslashes are escaped for use in connection strings
            string safePath = filePath.Replace(@"\", @"\\");

            if (extension == ".xls")
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={safePath};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
            }
            else if (extension == ".xlsx")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={safePath};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1';";
            }

            return connectionString;
        }

        // 2. LOAD EXCEL SHEETS IN DDL
        public static List<ListItem> GetExcelSheetList(string filePath, DropDownList ddlToSet = null)
        {
            List<ListItem> items = new List<ListItem>
    {
        new ListItem("Select Sheet", "")
    };

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                DataTable dtSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dtSheets != null && dtSheets.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSheets.Rows)
                    {
                        string sheetName = row["TABLE_NAME"].ToString();
                        sheetName = sheetName.Replace("$", "").Replace("'", "");
                        items.Add(new ListItem(sheetName, sheetName));
                    }
                }
                else
                {
                    items.Add(new ListItem("No sheets found", ""));
                }
            }

            // Optionally bind to DropDownList
            if (ddlToSet != null)
            {
                ddlToSet.Items.Clear();
                ddlToSet.Items.AddRange(items.ToArray());
            }

            return items;
        }

        public static List<string> GetExcelHeaders(string filePath, string sheetName, DropDownList ddlToSet = null)
        {
            List<string> headers = new List<string>();

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";

                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                using (OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    DataTable schemaTable = reader.GetSchemaTable();

                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string columnName = row["ColumnName"].ToString();
                        headers.Add(columnName);
                    }
                }
            }

            // Optional: Set to DropDownList
            if (ddlToSet != null)
            {
                ddlToSet.Items.Clear();
                ddlToSet.Items.Add(new ListItem("Select Column", ""));

                foreach (var header in headers)
                {
                    ddlToSet.Items.Add(new ListItem(header, header));
                }
            }

            return headers;
        }


        public static List<string> GetExcelHeadersFromDataTable(DataTable table, DropDownList ddlToSet = null)
        {
            List<string> headers = new List<string>();

            if (table == null || table.Columns.Count == 0)
                return headers;

            foreach (DataColumn col in table.Columns)
            {
                headers.Add(col.ColumnName);
            }
 

            headers.Sort();
            // Optionally bind to DropDownList
            if (ddlToSet != null)
            {
                ddlToSet.Enabled=true;
                ddlToSet.Items.Clear();
                ddlToSet.Items.Add(new ListItem("Select Column", ""));

                foreach (var header in headers)
                {
                    ddlToSet.Items.Add(new ListItem(header, header));
                }
            }else
            {
                ddlToSet.Enabled = false;
            }






           
            return headers;
        }


        // 3. LOAD EXCEL DATA
        public static DataTable LoadSheetData(string filePath, string sheetName, GridView gridToSet= null)
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

                        // Set the DataTable as the DataSource for the GridView
                        if (gridToSet != null)
                        {
                            gridToSet.DataSource = dtResult;
                            gridToSet.DataBind();
                        }
                    }
                }
            }

            return dtResult;
        }

        public DataTable ReadExcelUsingClosedXML2(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();

            try
            {
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    IXLWorksheet worksheet = workbook.Worksheet(sheetName);

                    if (worksheet == null)
                        throw new Exception($"Sheet '{sheetName}' not found.");

                    // Get the first row (headers)
                    var headerRow = worksheet.FirstRowUsed();
                    var headerCells = headerRow.CellsUsed();

                    // Create DataTable columns (using header names)
                    foreach (var cell in headerCells)
                    {
                        dt.Columns.Add(cell.Value.ToString());
                    }

                    // Get all data rows (skip header)
                    var dataRows = worksheet.RowsUsed().Skip(1);

                    // Loop through each row
                    foreach (var row in dataRows)
                    {
                        DataRow dataRow = dt.NewRow();

                        for (int i = 0; i < headerCells.Count(); i++)
                        {
                            var cell = row.Cell(i + 1); // 1-based index

                            // Handle empty cells (replace null with empty string)
                            dataRow[i] = cell.IsEmpty() ? "" : cell.Value.ToString();
                        }

                        dt.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading Excel: {ex.Message}", ex);
            }

            return dt;
        }

        public DataTable ReadExcelUsingClosedXML(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();

            try
            {
                // Load the Excel file
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    // Get the worksheet by name
                    IXLWorksheet worksheet = workbook.Worksheet(sheetName);

                    if (worksheet == null)
                    {
                        throw new Exception($"Sheet '{sheetName}' not found in the Excel file.");
                    }

                    // Get the first row (header row) to create DataTable columns
                    var firstRow = worksheet.FirstRowUsed();
                    var headerCells = firstRow.CellsUsed();

                    // Add columns to DataTable (with header names)
                    foreach (var cell in headerCells)
                    {
                        dt.Columns.Add(cell.Value.ToString());
                    }

                    // Get all data rows (skip the header row)
                    var dataRows = worksheet.RowsUsed().Skip(1); // Skip(1) to exclude the header row

                    // Loop through all data rows and add to DataTable
                    foreach (var row in dataRows)
                    {
                        DataRow dataRow = dt.NewRow();
                        for (int i = 0; i < headerCells.Count(); i++)
                        {
                            var cell = row.Cell(i + 1); // Columns are 1-based in ClosedXML
                            dataRow[i] = cell.Value.ToString(); // Convert to string (handles all types)
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading Excel file: {ex.Message}", ex);
            }

            return dt;
        }

        public DataTable ReadExcelUsingClosedXMLSheetIndex(string filePath, int sheetIndex)
        {
            DataTable dt = new DataTable();

            try
            {
                using (XLWorkbook workbook = new XLWorkbook(filePath))
                {
                    // Get all worksheets
                    var worksheets = workbook.Worksheets.ToList();

                    // Validate index
                    if (sheetIndex < 0 || sheetIndex >= worksheets.Count)
                    {
                        throw new Exception($"Sheet index {sheetIndex} is out of range. The file has {worksheets.Count} sheet(s).");
                    }

                    // Get the worksheet by index
                    IXLWorksheet worksheet = worksheets[sheetIndex];

                    // Get the first row (headers)
                    var firstRow = worksheet.FirstRowUsed();
                    var headerCells = firstRow.CellsUsed();

                    // Add columns to DataTable
                    foreach (var cell in headerCells)
                    {
                        dt.Columns.Add(cell.Value.ToString());
                    }

                    // Read all data rows (excluding the header)
                    var dataRows = worksheet.RowsUsed().Skip(1);

                    foreach (var row in dataRows)
                    {
                        DataRow dataRow = dt.NewRow();
                        for (int i = 0; i < headerCells.Count(); i++)
                        {
                            var cell = row.Cell(i + 1);  
                            dataRow[i] = cell.Value.ToString() ?? string.Empty;
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading Excel file: {ex.Message}", ex);
            }

            return dt;
        }

        public static DataTable GetExcelSheetData(string filePath, string sheetName, out DataTable fetchData , GridView gridToSet = null, Label lblMesage = null)
        {
            DataTable dtResult = new DataTable();

            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn))
                {
                    adapter.Fill(dt);
                }
            }
            fetchData = dt;

            if (gridToSet != null)
            {
                if (dt.Rows.Count > 0)
                {
                    if (lblMesage != null)
                    {
                        lblMesage.Text = $"Selected sheet: {sheetName}, has {dt.Rows.Count} rows.";
                    } 

                    gridToSet.DataSource = dt;
                    gridToSet.DataBind();
                }
                else
                {
                    if (lblMesage != null)
                    {
                        lblMesage.Text = $"";
                    }
                    gridToSet.DataSource = null;
                    gridToSet.DataBind();
                }
            }
            
            return dt;
        }


        public void ExportToExcelByDT(DataTable dataToExport)
        {
            if (dataToExport != null && dataToExport.Rows.Count > 0)
            {
                try
                {
                    // Create a new workbook
                    using (var workbook = new XLWorkbook())
                    {
                        // Add a worksheet to the workbook
                        var worksheet = workbook.AddWorksheet("Policy Data");

                        // Add the header row (from DataTable columns) with borders
                        for (int col = 0; col < dataToExport.Columns.Count; col++)
                        {
                            var headerCell = worksheet.Cell(1, col + 1);
                            headerCell.Value = dataToExport.Columns[col].ColumnName;
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            headerCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        }

                        // Add the data rows (from DataTable rows) with borders
                        for (int row = 0; row < dataToExport.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataToExport.Columns.Count; col++)
                            {
                                object cellValue = dataToExport.Rows[row][col];

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

                        using (var memoryStream = new MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                            HttpContext.Current.Response.Buffer = false;
                            HttpContext.Current.Response.End();

                            /*
                            HttpContext.Current.Response.Flush();
                            HttpContext.Current.Response.SuppressContent = true;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                             */
                        }
                        
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                HttpContext.Current.Response.Write("No data to export.");
            }
        }

        public void ExportToExcelByDT2(DataTable dt, string fileName)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Sheet1");
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    byte[] data = stream.ToArray();

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                    HttpContext.Current.Response.BinaryWrite(data);
                    HttpContext.Current.Response.End();
                }
            }
        }


        public void ExportToCsvExcel(System.Data.DataTable dt, string fileName = "exported_file.xls")
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
            // file name take form from a varibaer

            HttpContext.Current.Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel"; // Excel 2003 XML format
            HttpContext.Current.Response.OutputStream.Write(excelBytes, 0, excelBytes.Length);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();

        }


        #endregion

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

        #region Inline SQL Functions



        public class BulkInsertExecutor
        {
            public bool ExecuteBulkInsert(string insertStatement, out string errorMessage)
            {
                errorMessage = string.Empty;
                bool success = false;

                using (OracleConnection conn = new OracleConnection(
                    WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    try
                    {
                        conn.Open();

                        using (OracleCommand cmd = new OracleCommand("PSM_EXECUTE_BULK_INSERT", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            cmd.Parameters.Add("p_insert_statement", OracleDbType.Clob).Value = insertStatement;
                            cmd.Parameters.Add("p_success", OracleDbType.Int32).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("p_error_message", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            // Get output parameters
                            success = Convert.ToInt32(cmd.Parameters["p_success"].Value) == 1;
                            errorMessage = cmd.Parameters["p_error_message"].Value.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"Error executing bulk insert: {ex.Message}";
                        success = false;
                    }
                }

                return success;
            }
        }


        public bool ExecuteNonQueryMaster(string query, out string exception)
        {
            exception = null; // Default to no exception

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute COMMIT or other non-query commands
                        return true; // Return success
                    }
                    catch (OracleException ex)
                    {
                        exception = ex.Message; // Capture exception message
                        return false; // Return failure
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
        }


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


        public DataTable ExecuteStoredProcedureMaster(string procedureName, OracleParameter[] parameters, out int rowCount, out string exception)
        {
            DataTable dt = new DataTable();
            rowCount = 0; // Default row count
            exception = null; // Default exception as null

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Set command type to stored procedure

                    // Add parameters if they exist
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the result of the procedure
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


        public DataTable ExecuteParameterizedQuery(string sql, Dictionary<string, object> parameters, out int rowCount, out string errorMessage)
        {
            DataTable resultTable = new DataTable();
            rowCount = 0;
            errorMessage = null;

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        // Add parameters to the command
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.Add(new OracleParameter(param.Key, param.Value ?? DBNull.Value));
                            }
                        }

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(resultTable);
                            rowCount = resultTable.Rows.Count;
                        }
                    }
                }
                catch (OracleException ex)
                {
                    errorMessage = $"Oracle Error {ex.Number}: {ex.Message}";
                    // Log the error if needed
                }
                catch (Exception ex)
                {
                    errorMessage = $"General Error: {ex.Message}";
                    // Log the error if needed
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return resultTable;
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

        public DataTable ExecuteCurrentQuery3(string query, out int rowCount, out string exception, out bool isExecuted)
        {
            DataTable dt = new DataTable();
            rowCount = 0;
            exception = null;
            isExecuted = false; // Default to false

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                            rowCount = dt.Rows.Count;
                            isExecuted = true; // Set to true only if fill succeeds
                        }
                    }
                    catch (OracleException ex)
                    {
                        exception = ex.Message;

                        dt = new DataTable();
                        dt.Columns.Add("Exception", typeof(string));
                        dt.Rows.Add("Error: " + ex.Message);

                        isExecuted = false; // Execution failed
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




        public static string SqlRetString(string query)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value && result != null ? result.ToString() : string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DateTime SqlRetDate(string query)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value && result != null ? Convert.ToDateTime(result) : DateTime.MinValue;
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static int SqlRetInt(string query)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public static decimal SqlRetDecimal(string query)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0m;
                }
            }
            catch
            {
                return 0m;
            }
        }

        public static bool SqlRetBool(string query)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value && result != null && Convert.ToBoolean(result);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool SqlExecuteInsert(string query)
        {
            using (OracleConnection conn = new OracleConnection(connectionString))
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery(); // INSERT/UPDATE/DELETE
                return rowsAffected > 0;
            }
        }


        #endregion

        #region GetCityList

        public DataTable PsmCityMaster(string country, string state, string city)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_CITY_MASTER", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters with default NULL values if input is null or empty
                        command.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = currentLoginID();
                        command.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = currentRoleID();
                        command.Parameters.Add("P_COUNTRY", OracleDbType.Varchar2).Value = DBNullString(country);
                        command.Parameters.Add("P_STATE", OracleDbType.Varchar2).Value = DBNullString(state);
                        command.Parameters.Add("P_CITY", OracleDbType.Varchar2).Value = DBNullString(city);

                        // Output cursor
                        command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Open connection
                        conn.Open();

                        // Fill DataTable using adapter
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in PsmCityMaster: " + ex.Message);
            }

            return dt;
        }
       
        
        public DataTable PsmListCountryStateCity(string getByValue = null, string getBy = null, string orderBy = null)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_LIST_COUNTRY_STATE_CITY", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add("P_GET_BY_VALUE", OracleDbType.Varchar2).Value = DBNullString(getByValue);
                        command.Parameters.Add("P_GET_BY", OracleDbType.Varchar2).Value = DBNullString(getBy);
                        command.Parameters.Add("P_ORDER_BY", OracleDbType.Varchar2).Value = DBNullString(orderBy);

                        // Output parameter
                        command.Parameters.Add("P_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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
                throw new Exception("Error in PsmListCountryStateCity: " + ex.Message);
            }

            return dt;
        }

        public DataTable PsmStateMaster(string country, string state)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_STATE_MASTER", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = currentLoginID();
                        command.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = currentRoleID();
                        command.Parameters.Add("P_COUNTRY", OracleDbType.Varchar2).Value = DBNullString(country);
                        command.Parameters.Add("P_STATE", OracleDbType.Varchar2).Value = DBNullString(state);

                        // Output parameter
                        command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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
                throw new Exception("Error in PsmStateMaster: " + ex.Message);
            }

            return dt;
        }



        public static object DBNullString(string value)
        {
            return string.IsNullOrEmpty(value) ? DBNull.Value : (object)value;
        }


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

        #region Login ID and Role ID Cresidential, Access and Branch Codes

        public readonly HashSet<string> AuthorizedRolesMF = new HashSet<string> { "212", "241" };


        public (bool IsAuthorized, string Message) CheckAccess(string buttonType)
        {
            string loginId = currentLoginID();
            string currentRole = currentRoleID();



            try
            {
                using (var conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = new OracleCommand("psm_ao_role_master", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        cmd.Parameters.Add("p_login", OracleDbType.Varchar2).Value = loginId;
                        cmd.Parameters.Add("p_role", OracleDbType.Varchar2).Value = (object)currentRole ?? DBNull.Value;
                        cmd.Parameters.Add("p_button_id", OracleDbType.Varchar2).Value = buttonType.ToLower();

                        // Output parameter
                        var resultCursor = new OracleParameter("p_cursor", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(resultCursor);

                        cmd.ExecuteNonQuery();

                        using (var reader = ((OracleRefCursor)resultCursor.Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                bool isAuthorized = reader["status"].ToString() == "true";
                                string message = reader["message"].ToString();
                                return (isAuthorized, message);
                            }
                        }
                    }
                }

                return (false, "No authorization data received");
            }
            catch (OracleException ex)
            {
                return (false, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"System error: {ex.Message}");
            }
        }

        public (bool IsAuthorized, string Message) CheckAccessMaster(string moduleAction)
        {
            string loginId = currentLoginID();
            string currentRole = currentRoleID();

            try
            {
                using (var conn = new OracleConnection(connectionString))
                {
                    conn.Open();

                    using (var cmd = new OracleCommand("PSM_ACCESS_MASTER", conn)) // Match name with actual proc
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_LOGIN", OracleDbType.Varchar2).Value = loginId;
                        cmd.Parameters.Add("P_ROLE", OracleDbType.Varchar2).Value = (object)currentRole ?? DBNull.Value;
                        cmd.Parameters.Add("P_MODULE_ACTION", OracleDbType.Varchar2).Value = moduleAction.ToLower();

                        var resultCursor = new OracleParameter("P_CURSOR", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(resultCursor);

                        cmd.ExecuteNonQuery();

                        using (var reader = ((OracleRefCursor)resultCursor.Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                bool isAuthorized = string.Equals(reader["STATUS"].ToString(), "true", StringComparison.OrdinalIgnoreCase);
                                string message = reader["MESSAGE"].ToString();
                                return (isAuthorized, message);
                            }
                        }
                    }
                }

                return (false, "No authorization data received");
            }
            catch (OracleException ex)
            {
                return (false, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"System error: {ex.Message}");
            }
        }



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
        public string currentLoginID()
        {
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;

            return currentLoginId;
        }

        public string currentRoleID()
        {
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            return currentRoleId;
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

        public string GlbloginidHardCoded()
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

            // Join the list into a single string separated by commas and return it
            return string.Join(",", values);
        }



        public string psm_Branches(string currentLoginId = null, string currentRoleId = null)
        {
            currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                return string.Empty;
            }

            string sql = "SELECT BM.BRANCH_CODE, BM.BRANCH_NAME FROM BRANCH_MASTER BR INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID WHERE UJ.LOGIN_ID = '" + currentLoginId + "' AND UJ.ROLE_ID = '" + currentRoleId + "' ORDER BY BRANCH_NAME";

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
            //string branches = psm_Branches.Invoke();
            string currentLoginId = HttpContext.Current.Session["LoginId"] as string;
            string currentRoleId = HttpContext.Current.Session["RoleId"] as string;

            if (string.IsNullOrEmpty(currentLoginId) || string.IsNullOrEmpty(currentRoleId))
            {
                return string.Empty;
            }

            string sql = "SELECT BM.BRANCH_CODE, BM.BRANCH_NAME FROM BRANCH_MASTER BM INNER JOIN USERDETAILS_JI UJ ON BM.BRANCH_CODE = UJ.BRANCH_ID WHERE UJ.LOGIN_ID = '" + currentLoginId + "' AND UJ.ROLE_ID = '" + currentRoleId + "' ORDER BY BRANCH_NAME";

            DataTable dtBranchList = ExecuteCurrentQuery(sql);

            string currentBranchCode = string.Empty;
            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("BRANCH_NAME") && dtBranchList.Columns.Contains("BRANCH_CODE"))
            {
                string[] branchCodes = dtBranchList.AsEnumerable()
                                                   .Select(row => row["BRANCH_CODE"].ToString())
                                                   .ToArray();
                currentBranchCode = string.Join(",", branchCodes);
            }

 

            sql = "SELECT RM_CODE, RM_NAME FROM employee_master WHERE source IN (" + currentBranchCode + ")";

             dtBranchList = ExecuteCurrentQuery(sql);
            string currentRmCodes = string.Empty;
            if (dtBranchList != null && dtBranchList.Rows.Count > 0 &&
                dtBranchList.Columns.Contains("RM_CODE") && dtBranchList.Columns.Contains("RM_NAME"))
            {
                string[] RmCodes = dtBranchList.AsEnumerable()
                                               .Select(row => row["RM_CODE"].ToString())
                                               .ToArray();
                currentRmCodes = string.Join(",", RmCodes);
            }

            return currentRmCodes;
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

        #endregion

        #region Helper Functions

        public DateTime? ParseDateWithParamEW(string dateText, string arNumberText)
        {
            // If dateText is null/empty OR arNumberText is NOT null/empty, return null
            if (string.IsNullOrEmpty(dateText) || !string.IsNullOrEmpty(arNumberText))
            {
                return null;
            }

            // Try parse exact with format dd/MM/yyyy
            if (DateTime.TryParseExact(dateText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            // If parse failed, return null
            return null;
        }


        public static DateTime? ParseNullableDateEW(string inputDate)
        {
            if (string.IsNullOrEmpty(inputDate))
                return null;

            if (DateTime.TryParseExact(inputDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                return result;

            return null;
        }

        public static decimal? ParseNullableDecimal(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            if (decimal.TryParse(input, out decimal result))
                return result;

            return null; // or throw if invalid value is unacceptable
        }


        public static decimal ParseNullableDecimalNE(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            if (decimal.TryParse(input, out decimal result))
                return result;

            return 0; // or throw if invalid value is unacceptable
        }





        public void RunClientScript(Page page, string scriptKey, string clientScript)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page), "Page instance cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(clientScript))
            {
                return; // Nothing to run
            }

            // Sanitize scriptKey to avoid collisions or invalid keys
            scriptKey = string.IsNullOrWhiteSpace(scriptKey) ? Guid.NewGuid().ToString() : scriptKey;

            if (ScriptManager.GetCurrent(page)?.IsInAsyncPostBack == true)
            {
                // Partial postback: use ScriptManager
                ScriptManager.RegisterStartupScript(
                    page,
                    page.GetType(),
                    scriptKey,
                    clientScript.EndsWith(";") ? clientScript : clientScript + ";",
                    true
                );
            }
            else
            {
                // Full postback: use ClientScript
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    scriptKey,
                    clientScript.EndsWith(";") ? clientScript : clientScript + ";",
                    true
                );
            }
        }


        public void CloseWindow(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page), "Page instance cannot be null.");
            }

            string script = "window.close();";

            // Handle ScriptManager-aware registration
            if (ScriptManager.GetCurrent(page)?.IsInAsyncPostBack == true)
            {
                ScriptManager.RegisterStartupScript(
                    page,
                    page.GetType(),
                    "closeWindow",
                    script,
                    true
                );
            }
            else
            {
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "closeWindow",
                    script,
                    true
                );
            }
        }



        public void ShowAlertAndCloseWindow(Page page, string message)
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
                    $"alert('{sanitizedMessage}'); window.close();",
                    true
                );
            }
            else
            {
                // Use ClientScript for regular full postbacks
                page.ClientScript.RegisterStartupScript(
                    page.GetType(),
                    "alert",
                    $"alert('{sanitizedMessage}'); window.close();",
                    true
                );
            }
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


        public void ShowAlert2(string message)
        {
            Page page = HttpContext.Current?.Handler as Page;

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
            //Console.WriteLine($"Current Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

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
            //Console.WriteLine($"Updated Session LoginId: {currentLoginId}, RoleId: {currentRoleId}");

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
        public void ReloadCurrentPage(Page page, bool doRun = false)
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
                        HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl, doRun);
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
        #endregion


    }
}