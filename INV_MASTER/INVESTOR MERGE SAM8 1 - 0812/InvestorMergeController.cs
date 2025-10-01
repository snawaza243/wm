using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WM.Models;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Types;
using System.Drawing;
using System.Text.RegularExpressions;

namespace WM.Controllers
{
    public class InvestorMergeController
    {

        public DataTable GetRmList(string source)
        {
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                string query = @"Select rm_code,rm_name,source from employee_master where source=:source AND (TYPE='A' OR TYPE IS NULL) order by rm_name";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":source", source);
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable GetDDlSearchBranch()
        {
            DataTable dtBranch = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand command = new OracleCommand("PRC_INVMERGE_BRANCH", con))
                    {
                        OracleDataAdapter da;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["loginid"].ToString();
                        command.Parameters.Add("P_ROLE_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["roleid"].ToString();

                        // Output parameters
                        command.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                        command.Parameters["P_ERRORMESSAGE"].Size = 2000;
                        command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, 100).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(command);
                        da.Fill(dtBranch);
                        string message = Convert.ToString(command.Parameters["P_ERRORMESSAGE"].Value);
                        if (message != "null")
                        {
                            throw new ArgumentException(message);
                        }
                        else
                        {

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
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtBranch;







        }
        public DataTable GetDDlSearchcity()
        {
            DataTable dtCity = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand command = new OracleCommand("PRC_INVMERGE_CITY", con))
                    {
                        OracleDataAdapter da;
                        command.CommandType = CommandType.StoredProcedure;

                        // Output parameters
                        command.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                        command.Parameters["P_ERRORMESSAGE"].Size = 2000;
                        command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, 100).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(command);
                        da.Fill(dtCity);
                        string message = Convert.ToString(command.Parameters["P_ERRORMESSAGE"].Value);
                        if (message != "null")
                        {
                            throw new ArgumentException(message);
                        }
                        else
                        {

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
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtCity;



        }

        public DataTable GetDDlSearchRM()
        {
            DataTable dtRM = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand command = new OracleCommand("PRC_INVMERGE_RM", con))
                    {
                        OracleDataAdapter da;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["loginid"].ToString();
                        command.Parameters.Add("P_ROLE_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["roleid"].ToString();

                        // Output parameters
                        command.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                        command.Parameters["P_ERRORMESSAGE"].Size = 2000;
                        command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, 100).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(command);
                        da.Fill(dtRM);
                        string message = Convert.ToString(command.Parameters["P_ERRORMESSAGE"].Value);
                        if (message != "null")
                        {
                            throw new ArgumentException(message);
                        }
                        else
                        {

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
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dtRM;



        }

        private object DBNullString(string value)
        {
            return string.IsNullOrEmpty(value) ? DBNull.Value : (object)value;
        }
        public DataTable GetSearchForMerge(string category, Models.InvestorMergModel objIM)
        {
            DataTable dtMergeData = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand command = new OracleCommand("PRC_INVMERGE_FIND", con))
                    {
                        OracleDataAdapter da;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["loginid"].ToString();
                        command.Parameters.Add("P_ROLE_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["roleid"].ToString();
                       
                        command.Parameters.Add("P_CATEGORY", OracleDbType.Varchar2).Value = DBNullString(category);

                        if (objIM.CLIENTSUBBROKER == "")
                        {
                            command.Parameters.Add("P_CLIENT_SUBBROKER", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_CLIENT_SUBBROKER", OracleDbType.Varchar2).Value = objIM.CLIENTSUBBROKER;
                        }
                        if (objIM.CLIENT_NAME == "")
                        {
                            command.Parameters.Add("P_NAME", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_NAME", OracleDbType.Varchar2).Value = objIM.CLIENT_NAME;
                        }
                        //command.Parameters.Add("P_CLIENTCODE", OracleDbType.Varchar2).Value = objIM.CLIENT_CODE;
                        if (objIM.ADDRESS1 == "")
                        {
                            command.Parameters.Add("P_ADDRESS1", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_ADDRESS1", OracleDbType.Varchar2).Value = objIM.ADDRESS1;
                        }
                        if (objIM.ADDRESS2 == "")
                        {
                            command.Parameters.Add("P_ADDRESS2", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_ADDRESS2", OracleDbType.Varchar2).Value = objIM.ADDRESS2;
                        }
                        if (objIM.PHONE == "")
                        {
                            command.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_PHONE", OracleDbType.Varchar2).Value = objIM.PHONE;
                        }
                        if (objIM.MOBILE == "")
                        {
                            command.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_MOBILE", OracleDbType.Varchar2).Value = objIM.MOBILE;
                        }
                        if (objIM.PAN == "")
                        {
                            command.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = objIM.PAN;

                        }
                        if (objIM.CITY == "")
                        {
                            command.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_PAN", OracleDbType.Varchar2).Value = objIM.CITY;
                        }
                        if (objIM.BRANCH_NAME == "")
                        {
                            command.Parameters.Add("P_BRANCH", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_BRANCH", OracleDbType.Varchar2).Value = objIM.BRANCH_NAME;
                        }
                        if (objIM.RM == "")
                        {
                            command.Parameters.Add("P_RMCODE", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {

                            command.Parameters.Add("P_RMCODE", OracleDbType.Varchar2).Value = objIM.RM;
                        }
                        if (objIM.SORT == "")
                        {
                            command.Parameters.Add("P_SORT", OracleDbType.Varchar2).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("P_SORT", OracleDbType.Varchar2).Value = objIM.SORT;
                        }
                        // Output parameters
                        command.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                        command.Parameters["P_ERRORMESSAGE"].Size = 2000;
                        command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor, 100).Direction = ParameterDirection.Output;
                        da = new OracleDataAdapter(command);
                        da.Fill(dtMergeData);
                        string message = Convert.ToString(command.Parameters["P_ERRORMESSAGE"].Value);
                        if (message != "null")
                        {
                            throw new ArgumentException(message);
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
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
            return dtMergeData;



        }
        public DataTable GetClientData(string source)
        {
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                string query = @"SELECT DISTINCT CLIENT_NAME, im.SOURCE_ID FROM CLIENT_MASTER cm JOIN INVESTOR_MASTER im ON im.SOURCE_ID  = cm.CLIENT_CODE WHERE cm.RM_CODE = :source ORDER BY CLIENT_NAME";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":source", source);
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable GetInvestorList(string source)
        {
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                string query = @"
SELECT 
    NVL(SUM(fm.TotalCount), 0) AS TotalCount, 
    im.INV_CODE, 
    im.INVESTOR_NAME, 
    im.ADDRESS1, 
    im.ADDRESS2, 
    cm.CITY_NAME, 
    im.PINCODE, 
    im.KYC
FROM 
    INVESTOR_MASTER im
JOIN 
    CITY_MASTER cm 
    ON im.CITY_ID = cm.CITY_ID
LEFT JOIN 
    (
        SELECT 
            COUNT(*) AS TotalCount, 
            ts.CLIENT_CODE 
        FROM 
            TRANSACTION_ST ts
        GROUP BY 
            ts.CLIENT_CODE
        UNION ALL
        SELECT 
            COUNT(*) AS TotalCount, 
            ts.CLIENT_CODE 
        FROM 
            TRANSACTION_STTEMP ts
        GROUP BY 
            ts.CLIENT_CODE
    ) fm 
    ON fm.CLIENT_CODE = im.INV_CODE
WHERE 
    im.SOURCE_ID = :source
GROUP BY 
    im.INV_CODE, 
    im.INVESTOR_NAME, 
    im.ADDRESS1, 
    im.ADDRESS2, 
    cm.CITY_NAME, 
    im.PINCODE, 
    im.KYC
";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(":source", source);
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public DataTable GetBranchMaster()
        {
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                string query = @"
                select branch_code,branch_name from branch_master 
                where location_id in(
                    select location_id from location_master where city_id in (
                        select city_id from city_master
                    )
                )
                AND CATEGORY_ID<>1004 AND CATEGORY_ID<>1005 AND CATEGORY_ID<>1006 ORDER BY BRANCH_NAME
                ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    //command.Parameters.Add("@ClientCode", client.ClientCode);
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }


        public string InvestorMergeProcess(string MainCode, string MergedCode)
        {
            string ResultError = "";
            string ResultSuccess = "";
            string Result = "";

            OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
            if (connection.State == ConnectionState.Closed) { connection.Open(); }
            using (OracleCommand command = new OracleCommand("PRC_INVESTOR_MERGE", connection))
            {
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("P_MAIN_CODE", OracleDbType.Varchar2).Value = MainCode;
                command.Parameters.Add("P_TO_MERGE_CODE", OracleDbType.Varchar2).Value = MergedCode;
                command.Parameters.Add("P_LOGIN_ID", OracleDbType.Varchar2).Value = HttpContext.Current.Session["loginid"].ToString();


                // Output parameters
                command.Parameters.Add("P_ERRORMESSAGE", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                command.Parameters["P_ERRORMESSAGE"].Size = 2000;
                command.Parameters.Add("P_SUCCESS", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
                command.Parameters["P_SUCCESS"].Size = 2000;
                // Execute the command
                try
                {
                    command.ExecuteNonQuery();

                    ResultError = Convert.ToString(command.Parameters["P_ERRORMESSAGE"].Value);
                    ResultSuccess = Convert.ToString(command.Parameters["P_SUCCESS"].Value);

                    if (ResultSuccess == "SUCCESS")
                    {

                        Result = ResultSuccess;
                    }
                    else
                    {
                        Result = ResultError;
                    }


                }
                catch (OracleException ex)
                {




                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }


            return Result;
        }
    }
}