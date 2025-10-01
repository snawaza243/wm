using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Web.Configuration;
using System.Web;

public class OracleHelper
{
    // Generic method to call any Oracle procedure with dynamic parameters
    public (bool success, string message, DataTable data) ExecuteProcedure(string procedureName, List<(string paramName, OracleDbType paramType, int size, object value, ParameterDirection direction)> parameters)
    {
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

    public static object UserFun()
    {

        var parameters = new List<(string, OracleDbType, int, object, ParameterDirection)>
    {
        ("PX_LATER_TYPE", OracleDbType.Varchar2, 1, letterType, ParameterDirection.Input),
        ("PX_MON_YEAR", OracleDbType.Varchar2, 7, monYear, ParameterDirection.Input),
        ("PX_LOGE", OracleDbType.Varchar2, 50, HttpContext.Current.Session["LoginId"]?.ToString(), ParameterDirection.Input),
        ("PX_ROLE", OracleDbType.Varchar2, 50, HttpContext.Current.Session["roleId"]?.ToString(), ParameterDirection.Input),
        ("PX_CURSOR", OracleDbType.RefCursor, 0, null, ParameterDirection.Output)
    };

        var helper = new OracleHelper();
        var result = helper.ExecuteProcedure("PSM_PRINT_REN_CALCT", parameters);

        if (result.success)
        {
            DataTable data = result.data;
            // Process your data
        }
        else
        {
            string errorMsg = result.message;
        }
    }
}