using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using WM.Controllers;
using Newtonsoft.Json;
using System.Windows.Interop;


namespace WM.Masters
{
    public partial class print_renewal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["LoginId"] = "121397";
            //Session["roleId"] = "212";
            hdnLoginId.Value = Session["LoginId"]?.ToString();
            hdnRoleId.Value = Session["roleId"]?.ToString();
        }

        [WebMethod]
        public static object ProcessReport(string type, string mon_year)
        {
            print_renewal pr = new print_renewal();
            PsmController pc = new PsmController();

            var result = pr.GetPrintRenewal(type, mon_year);


            if (result.data != null && result.data.Rows.Count > 0)
            {


                DataTable masterTranBySource = new DataTable();


                // Transaction data for type A by SOURCECODE1
                if (type == "A")
                {
                    foreach (DataRow row in result.data.Rows)
                    {
                        if (row["sourcecode1"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["sourcecode1"].ToString()))
                        {
                            string sourceCode = row["sourcecode1"].ToString();
                            DataTable singleResult = ProcessRenewalReport(type, mon_year, "2", sourceCode);

                            if (singleResult != null && singleResult.Rows.Count > 0)
                            {
                                masterTranBySource.Merge(singleResult, true, MissingSchemaAction.Add);
                            }
                        }
                    }
                }


                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    message = "Records found",
                    data = ConvertDataTableToList(result.data),
                    data2 = ConvertDataTableToList(masterTranBySource),
                    totalCount = result.data.Rows.Count
                });

            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    message = "No records found",
                    data = new List<object>(),
                    totalCount = 0
                });
            }

        }

 

        public static object ConvertDataTableToList(DataTable dt)
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



        public (bool success, string message, DataTable data) GetPrintRenewal(string letterType, string monYear)
        {
            string loginId = HttpContext.Current.Session["LoginId"]?.ToString();
            string role = HttpContext.Current.Session["roleId"]?.ToString();

            DataTable dt = new DataTable();
            string msg = "";
            msg = string.IsNullOrEmpty(msg) ? "No Data Found!" : msg;


            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_PRINT_REN_CALCT0", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("PX_LATER_TYPE", OracleDbType.Varchar2, 1).Value = letterType ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_MON_YEAR", OracleDbType.Varchar2, 7).Value = monYear ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_LOGE", OracleDbType.Varchar2, 50).Value = loginId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("PX_ROLE", OracleDbType.Varchar2, 50).Value = role ?? (object)DBNull.Value;

                    // Output parameter for the cursor
                    cmd.Parameters.Add("PX_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (!dt.Columns.Contains("MSG") && dt.Rows.Count > 0)
            {
                return (true, "Data found!", dt);
            }
            else
            {
                return (false, msg, null);

            }

            /*

            if (dt.Columns.Contains("MSG") && dt.Rows.Count > 0)
            {
                msg = dt.Rows[0]["MSG"]?.ToString();

                if (!string.IsNullOrEmpty(msg) && msg.StartsWith("SUCCESS"))
                {
                    return (true, msg, dt);
                }
                else
                {
                    return (false, msg, null);
                }
            }
            else
            {
                return (false, msg, null);

            }*/
        }




        public static DataTable ProcessRenewalReport(string laterType, string monYear, string mainSub, string subCode)
        {
            try
            {
                DataTable dt = new DataTable();
                
                PsmController pc = new PsmController();
                
                var parameters = new List<(string, OracleDbType, int, object, ParameterDirection)>
        {
            ("PX_LATER_TYPE", OracleDbType.Varchar2, 50, laterType ?? (object)DBNull.Value, ParameterDirection.Input),
            ("PX_MON_YEAR", OracleDbType.Varchar2, 7, monYear ?? (object)DBNull.Value, ParameterDirection.Input),
            ("PX_LOGE", OracleDbType.Varchar2, 50, pc.currentLoginID() ?? (object)DBNull.Value, ParameterDirection.Input),
            ("PX_MAIN_SUB", OracleDbType.Varchar2, 1, mainSub ?? (object)DBNull.Value, ParameterDirection.Input),
            ("PX_SUB_CODE", OracleDbType.Varchar2, 50, subCode ?? (object)DBNull.Value, ParameterDirection.Input),
            ("PX_ROLE", OracleDbType.Varchar2, 50, pc.currentRoleID() ?? (object)DBNull.Value, ParameterDirection.Input),
            ("PX_CURSOR", OracleDbType.RefCursor, 0, null, ParameterDirection.Output)
        };

                var result = pc.PSM_ExecuteProcedure("WEALTHMAKER.PSM_PRINT_REN_REPORT", parameters);

                if (result.success && result.data != null && result.data.Rows.Count > 0)
                {
                    dt = result.data;
                }
                else
                {
                    dt= null;
                }

                return dt;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    
    }
}