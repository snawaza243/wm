using DocumentFormat.OpenXml.Office.Word;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using WM.Models;

namespace WM.Controllers
{
    public class ClientMergeController
    {
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
                AND CATEGORY_ID not in (1004,1005,1006) ORDER BY BRANCH_NAME";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
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
        public DataTable GetCityData(string cityId = null)
        {
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {

                string query = @"select distinct city_id, city_name, state_id from city_master order by city_name asc";

                if (cityId != null) query = @"select distinct city_id, city_name, state_id from city_master where city_id=:cityId order by city_name asc";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    if (cityId != null) command.Parameters.Add("city_id", cityId);

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public DataTable ClientSearch(string category, string branchCode, string cityCode, string rmCode, string clientCode, string clientName, string panNo, string phone, string mobile, string address1, string address2, string sortBy)
        {
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                //string query = @"Select CATNAME_name, CATNAME_CODE, a.address1, a.address2, C.CITY_NAME, b.Branch_name, b.Branch_Code, a.phone, a.mobile, 
                //                e.rm_name, e.rm_Code, kyc, CREATION_DATE, last_tran_dt1 
                //                FROM Branch_master b, EMPLOYEE_MASTER E, CATNAME_master a, City_master c 
                //                where E.source = b.branch_code AND a.RM_CODE = E.RM_CODE AND a.city_id = C.city_id(+)";
                /*
                string query = @"Select c.client_name, c.client_CODE, c.address1, c.address2, C.CITY_NAME, b.Branch_name, b.Branch_Code, c.phone, c.mobile, 
                                e.rm_name, e.rm_Code, c.kyc, c.CREATION_DATE, TO_CHAR(c.last_tran_dt1,'DD-MM-YYYY') AS last_tran_dt1,
                                nvl(ct.approved,'NO') as Approved
                                FROM client_master c
                                join EMPLOYEE_MASTER e on c.rm_code = e.rm_code
                                join Branch_master b on e.source = b.branch_code
                                join city_master cm on c.city_id = cm.city_id
                                left join(
                                    select Approved, client_codekyc from client_test
                                )ct ON substr(ct.client_codekyc, 1, 8) = c.client_code
                                where rownum < 100
                                ";

                if (!string.IsNullOrEmpty(branchCode)) query += " and upper(b.BRANCH_CODE) like '%" + branchCode.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(cityCode)) query += " and upper(cm.CITY_ID) like '%" + cityCode.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(rmCode)) query += " and upper(c.RM_CODE) like '%" + rmCode.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(clientCode)) query += " and c.Client_Code like '%" + clientCode + "%'";
                if (!string.IsNullOrEmpty(clientName)) query += " and upper(c.Client_NAME) like '%" + clientName.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(phone)) query += " and upper(c.phone) like '%" + phone.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(mobile)) query += " and upper(c.mobile) like '%" + mobile.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(panNo)) query += " and upper(c.PAN) like '%" + panNo.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(address1)) query += " and upper(c.address1) like '%" + address1.ToUpper() + "%'";
                if (!string.IsNullOrEmpty(address2)) query += " and upper(c.address2) like '%" + address2.ToUpper() + "%'";

                //query += " and rownum < 100";

                if (!string.IsNullOrEmpty(sortBy)) query += " order by " + sortBy + "";

                query = query.Replace("CATNAME", category);
                */

                sortBy = sortBy.Replace("CATNAME", category);



                using (OracleCommand command = new OracleCommand("search_client_v1", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_BranchCode", OracleDbType.Varchar2).Value = branchCode;
                    command.Parameters.Add("p_CityId", OracleDbType.Varchar2).Value = cityCode;
                    command.Parameters.Add("p_RmCode", OracleDbType.Varchar2).Value = rmCode;
                    command.Parameters.Add("p_ClientCode", OracleDbType.Varchar2).Value = clientCode;
                    command.Parameters.Add("p_ClientName", OracleDbType.Varchar2).Value = clientName;
                    command.Parameters.Add("p_Phone", OracleDbType.Varchar2).Value = phone;
                    command.Parameters.Add("p_Mobile", OracleDbType.Varchar2).Value = mobile;
                    command.Parameters.Add("p_Pan", OracleDbType.Varchar2).Value = panNo;
                    command.Parameters.Add("p_Address1", OracleDbType.Varchar2).Value = address1;
                    command.Parameters.Add("p_Address2", OracleDbType.Varchar2).Value = address2;
                    command.Parameters.Add("p_OrderBy", OracleDbType.Varchar2).Value = sortBy;
                    command.Parameters.Add("p_Category", OracleDbType.Varchar2).Value = category;

                    command.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public string MergerClient(List<ClientMergeModel> client, ClientMergeModel mainClient, DateTime modifyDate, string loggedUserId, ref DataTable dt)
        {
            string dnResponseMessage = string.Empty;
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand("ClientMerge_v1", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add the XML data as a parameter
                        OracleParameter xmlParam = new OracleParameter("p_xml", OracleDbType.Clob);
                        xmlParam.Value = CreateXMl(client);
                        cmd.Parameters.Add(xmlParam);

                        cmd.Parameters.Add("MainClientCode", OracleDbType.Int64).Value = mainClient.ClientCode;
                        cmd.Parameters.Add("ModifyDate", OracleDbType.Date).Value = modifyDate;
                        cmd.Parameters.Add("LoggedUserId", OracleDbType.Varchar2).Value = loggedUserId;

                        // Add the OUT parameter to capture the confirmation message
                        var confirmMsgParam = new OracleParameter("p_message", OracleDbType.Varchar2, 500)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(confirmMsgParam);

                        // Define the OUT parameter to receive the cursor
                        OracleParameter refCursorParam = new OracleParameter();
                        refCursorParam.OracleDbType = OracleDbType.RefCursor;
                        refCursorParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(refCursorParam);

                        // Execute the stored procedure
                        var count = cmd.ExecuteNonQuery();
                        dnResponseMessage = confirmMsgParam.Value.ToString();

                        // Retrieve the cursor
                        if (!dnResponseMessage.Contains("Error") && refCursorParam.Value != null && refCursorParam.Value is OracleRefCursor)
                        {
                            using (OracleDataReader reader = ((OracleRefCursor)refCursorParam.Value).GetDataReader())
                            {
                                if (reader != null)
                                {
                                    dt = new DataTable();
                                    dt.Load(reader);
                                }
                                
                            }
                        }

                        
                        return dnResponseMessage;
                    }
                }
                catch (Exception ex)
                {
                    dnResponseMessage = $"Error: {ex.Message}";
                }
                finally
                {
                    connection.Close();
                }
            }

            return dnResponseMessage;
        }
        private string CreateXMl(List<ClientMergeModel> list)
        {
            StringBuilder xml = new StringBuilder("<Records>");
            if (list != null && list.Count() > 0)
            {
                foreach (ClientMergeModel model in list)
                {
                    xml.Append("<Record>");
                    xml.Append($"<ClientCode>{model.ClientCode}</ClientCode>");
                    xml.Append("</Record>");
                }
            }
            xml.Append("</Records>");
            string completeXml = xml.ToString();
            return completeXml;
        }

    }
}