using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;

namespace WM.Masters
{
    public partial class ClientMerge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindBranchDataToDropdown();
                BindCityDataToDropdown();
            }
        }

        private void BindBranchDataToDropdown()
        {
            var data = new ClientMergeController().GetBranchMaster();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow dataRow in data.Rows)
                {
                    Branch_Dropdown.Items.Add(new ListItem
                    {
                        Text = Convert.ToString(dataRow["branch_name"]),
                        Value = Convert.ToString(dataRow["branch_code"])
                    });

                    BranchSearch_Dropdown.Items.Add(new ListItem
                    {
                        Text = Convert.ToString(dataRow["branch_name"]),
                        Value = Convert.ToString(dataRow["branch_code"])
                    });
                }
            }
        }
        private void BindCityDataToDropdown()
        {
            var data = new ClientMergeController().GetCityData();

            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    CitySearch_Dropdown.Items.Add(new ListItem() { Text = Convert.ToString(row["city_name"]), Value = Convert.ToString(row["city_id"]) });
                }

            }
        }
        protected void Branch_Dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            string branchCode = Branch_Dropdown.Text;
            var data = new ClientMergeController().GetRmList(branchCode);
            bool isData = data != null && data.Rows.Count > 0;
            RM_Dropdown.Items.Clear();
            RM_Dropdown.Items.Add(new ListItem { Text = (isData ? "Select RM" : "Rm Not Found in this Branch"), Value = "" });
            if (isData)
            {
                foreach (DataRow dataRow in data.Rows)
                {
                    RM_Dropdown.Items.Add(new ListItem
                    {
                        Text = Convert.ToString(dataRow["rm_name"]),
                        Value = Convert.ToString(dataRow["rm_code"])
                    });
                }
            }

        }


        [WebMethod]
        public static string GetSearchData(string category, string branchCode, string cityCode, string rmCode, string clientCode, string clientName, string panNo, string phone, string mobile, string address1, string address2, string sortBy)
        {
            var searchResult = new ClientMergeController().ClientSearch(category, branchCode, cityCode, rmCode, clientCode, clientName, panNo, phone, mobile, address1, address2, sortBy);

            return JsonConvert.SerializeObject(new { data = searchResult }, Formatting.None); ;
        }

        [WebMethod]
        public static string MergeClient(List<ClientMergeModel> clients, ClientMergeModel mainClient)
        {
            DataTable investors = null;
            DateTime currentDate = DateTime.Now;
            string loggedUserId = Convert.ToString(HttpContext.Current.Session["LoginId"]);

            var dbResponse = new ClientMergeController().MergerClient(clients, mainClient, currentDate, loggedUserId, ref investors);

            return JsonConvert.SerializeObject(new { message = dbResponse, data = investors }, Formatting.None);
        }

        [WebMethod]
        public static string GetRMList(string branchCode)
        {
            List<dynamic> list = new List<dynamic>();
            var data = new ClientMergeController().GetRmList(branchCode);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["rm_name"]), value = Convert.ToString(row["rm_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);

        }
    }
}