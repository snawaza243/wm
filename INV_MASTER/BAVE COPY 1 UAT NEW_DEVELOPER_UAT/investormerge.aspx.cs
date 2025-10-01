using Microsoft.AspNet.FriendlyUrls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;

namespace WM.Masters
{
    public partial class investormerge : System.Web.UI.Page
    {
        string clientCode = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            var dataRM = new InvestorMergeController().GetDDlSearchRM();
            foreach (DataRow row in dataRM.Rows)
            {
                ddlRM.Items.Add(new ListItem { Text = Convert.ToString(row["RM_NAME"]), Value = Convert.ToString(row["PAYROLL_ID"]) });
            }

            if (!Page.IsPostBack)
            {
                BindBranchDataToDropDown();
                
                // Retrieve the CommandArgument value from the query string
                 clientCode = Request.QueryString["INMergeid"];
                string CLIENT_NAME = Request.QueryString["ClientName"];

                //if (!string.IsNullOrEmpty(clientCode))
                //{
                //    // Use the retrieved value as needed
                //    ddlClient.Items.Add(new ListItem(CLIENT_NAME, clientCode));
                //    ddlClient.SelectedItem.Value= clientCode;
                //    ddlClient.SelectedItem.Text = CLIENT_NAME;
                //    hdnClientID.Value = clientCode;

                //    btnshowData.Style["display"] = "inherit";
                //}

                if (!string.IsNullOrEmpty(clientCode))
                {
                    // Check if the clientCode already exists in the dropdown
                    if (ddlClient.Items.FindByValue(clientCode) == null)
                    {
                        // Add the new item if it doesn't already exist
                        ddlClient.Items.Add(new ListItem(CLIENT_NAME, clientCode));
                    }

                    // Select the added or existing item
                    ddlClient.SelectedValue = clientCode;

                    // Update the hidden field with the clientCode
                    hdnClientID.Value = clientCode;

                    // Display the button
                    btnshowData.Style["display"] = "inherit";
                }




            }
        }

        #region btnExit_Click
        protected void btnExit_Click(object sender, EventArgs e)
        {
            string loginId = Session["LoginId"]?.ToString();
            string roleId = Session["roleId"]?.ToString();
            Response.Redirect($"~/welcome?loginid={loginId}&roleid={roleId}");
        }
        #endregion

        private void BindBranchDataToDropDown()
        {
            var data = new InvestorMergeController().GetBranchMaster();
            foreach (DataRow row in data.Rows)
            {
                ddlBranch.Items.Add(new ListItem { Text = Convert.ToString(row["branch_name"]), Value = Convert.ToString(row["branch_code"]) });
            }
        }

        [WebMethod]
        public static string GetRMData(string code)
        {
            List<dynamic> list = new List<dynamic>();
            var data = new InvestorMergeController().GetRmList(code);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["rm_name"]), value = Convert.ToString(row["rm_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetClientList(string code)
        {
            List<dynamic> list = new List<dynamic>();
            var data = new InvestorMergeController().GetClientData(code);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["CLIENT_NAME"]), value = Convert.ToString(row["SOURCE_ID"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetInvestorForMerge(string code) {
            var investorsList = new InvestorMergeController().GetInvestorList(code);
            return JsonConvert.SerializeObject(new { data = investorsList }, Formatting.None);
        }

        [WebMethod]
        public static string UpdateInvestorMerge(string Mcode, string MergedCode)
        {
            var investorMergeStatus = new InvestorMergeController().InvestorMergeProcess(Mcode,MergedCode);
            return JsonConvert.SerializeObject(new { data = investorMergeStatus }, Formatting.None);

        }

        protected void ddlClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}