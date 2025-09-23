// a controler fuciont
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Drawing;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
[ScriptService]
public class PSM_WebMethod : System.Web.Services.WebService
{
      public DataTable PSM_LoggedTableList(string proc, string p_for, string p_by, string p_by_value)
  {
      string p_log_id = currentLoginID();
      string p_role_id = currentRoleID();

        // take log and role from session context
        if (HttpContext.Current.Session["LogID"] != null)
        {
            p_log_id = HttpContext.Current.Session["LogID"].ToString();
        }
        if (HttpContext.Current.Session["RoleID"] != null)
        {
            p_role_id = HttpContext.Current.Session["RoleID"].ToString();
        }

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


    [WebMethod]
    public static string GetDropdownData(string psm, string for_x, string by_y, string y, string get_name, string get_code)
    {
        string proc = "WEALTHMAKER." + psm;
        PsmController pc = new PsmController();
        List<dynamic> list = new List<dynamic>();
        var data = PSM_LoggedTableList(proc, for_x, by_y, y);
        foreach (DataRow row in data.Rows)
        {
            list.Add(new { text = Convert.ToString(row[get_name]), value = Convert.ToString(row[get_code]) });
        }
        var outPut = new { data = list };
        return JsonConvert.SerializeObject(outPut, Formatting.None);
    }
}
