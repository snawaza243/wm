using System;
using System.Web;
using System.Linq;
using System.Data;
using System.Web.UI;
using WM.Controllers;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using DocumentFormat.OpenXml.Bibliography;
using NPOI.SS.Formula.Functions;
using System.Web.Services;

namespace WM.Masters
{
    public partial class update_grade_for_fd_com : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["LoginId"] = "121397";
            //Session["roleId"] = "212";
            hdnLoginId.Value = Session["LoginId"]?.ToString();
            hdnRoleId.Value = Session["roleId"]?.ToString();

        }

        [System.Web.Services.WebMethod]
        public static object GetDataOnChange(string mon, string year)
        {
            PsmController pc = new PsmController();

            var parameters = new List<(string, OracleDbType, int, object, ParameterDirection)>
{
    ("PX_LOGIN", OracleDbType.Varchar2, 50, HttpContext.Current.Session["LoginId"]?.ToString(), ParameterDirection.Input),
    ("PX_ROLE", OracleDbType.Varchar2, 50, HttpContext.Current.Session["roleId"]?.ToString(), ParameterDirection.Input),
    ("PX_MONTH", OracleDbType.Varchar2, 2, mon, ParameterDirection.Input),   
    ("PX_YEAR", OracleDbType.Varchar2, 4, year, ParameterDirection.Input),  
    ("PX_CURSOR", OracleDbType.RefCursor, 0, null, ParameterDirection.Output)
};

            var helper = new PsmController(); 
            var result = helper.PSM_ExecuteProcedure("PSM_UPD_GRD_FDCOM_MON_CHANGE", parameters);

            if (result.success && result.data != null && result.data.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    message = "Records found",
                    data = pc.ConvertDataTableToList(result.data), 
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


        [System.Web.Services.WebMethod]
        public static object DeleteGradeRecord(string mon, string year)
        {
            try
            {
                PsmController pc = new PsmController();
                string result = pc.PSM_DeleteTable(
                    frm: "update_grade_for_fd_com",
                    tb: "ISS_COMP_GRADE",
                    col1: mon,
                    col2: year
                );

                return JsonConvert.SerializeObject(new
                {
                    success = !string.IsNullOrEmpty(result) && result.StartsWith("SUCCESS", StringComparison.OrdinalIgnoreCase),
                    message = result
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "ERROR: " + ex.Message
                });
            }
        }


        [WebMethod]
        public static object SaveGradeData(string mon,string year,string issTag,string issCode,string issName,string grade)
        {
            try
            {
                PsmController pc = new PsmController();

                var parameters = new List<(string, OracleDbType, int, object, ParameterDirection)>
        {
            ("PX_LOGIN", OracleDbType.Varchar2, 50, pc.currentLoginID(), ParameterDirection.Input),
            ("PX_ROLE", OracleDbType.Varchar2, 50, pc.currentRoleID(), ParameterDirection.Input),
            ("PX_MONTH", OracleDbType.Varchar2, 2, mon, ParameterDirection.Input),
            ("PX_YEAR", OracleDbType.Varchar2, 4, year, ParameterDirection.Input),
            ("PX_ISS_TAG", OracleDbType.Varchar2, 50, issTag, ParameterDirection.Input),
            ("PX_ISS_CODE", OracleDbType.Varchar2, 50, issCode, ParameterDirection.Input),
            ("PX_ISS_NAME", OracleDbType.Varchar2, 100, issName, ParameterDirection.Input),
            ("PX_GRADE", OracleDbType.Varchar2, 10, grade, ParameterDirection.Input),
            ("PX_CURSOR", OracleDbType.RefCursor, 0, null, ParameterDirection.Output)
        };

                var result = pc.PSM_ExecuteProcedure("PSM_UPD_GRD_FDCOM_SAVE", parameters);

                if (result.success && result.data != null && result.data.Rows.Count > 0)
                {
                    string msg = result.data.Rows[0]["MSG"].ToString();

                    bool isSuccess = msg.StartsWith("SUCCESS", StringComparison.OrdinalIgnoreCase);

                    return JsonConvert.SerializeObject(new
                    {
                        success = isSuccess,
                        message = msg,
                        data = pc.ConvertDataTableToList(result.data),
                        totalCount = result.data.Rows.Count
                    });
                }
                else if (result.success)
                {
                    // Procedure executed but no rows returned
                    return JsonConvert.SerializeObject(new
                    {
                        success = true,
                        message = "No data returned",
                        data = new List<object>(),
                        totalCount = 0
                    });
                }
                else
                {
                    // Procedure failed
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "Error: " + result.message,
                        data = new List<object>()
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions here
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Exception: " + ex.Message,
                    data = new List<object>()
                });
            }
        }




    }
}