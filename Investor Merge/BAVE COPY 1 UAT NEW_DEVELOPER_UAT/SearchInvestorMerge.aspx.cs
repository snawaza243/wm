using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;


namespace WM.Masters
{
    public partial class SearchInvestorMerge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var data = new InvestorMergeController().GetDDlSearchBranch();             
                foreach (DataRow row in data.Rows)
                {
                   ddlBranchName.Items.Add(new ListItem { Text = Convert.ToString(row["branch_name"]), Value = Convert.ToString(row["branch_code"]) });
                }
     
                var dataCity = new InvestorMergeController().GetDDlSearchcity();
                foreach (DataRow row in dataCity.Rows)
                {
                    ddlCity.Items.Add(new ListItem { Text = Convert.ToString(row["CITY_NAME"]), Value = Convert.ToString(row["CITY_ID"]) });
                }

                var dataRM = new InvestorMergeController().GetDDlSearchRM();
                foreach (DataRow row in dataRM.Rows)
                {
                    ddlRM.Items.Add(new ListItem { Text = Convert.ToString(row["RM_NAME"]), Value = Convert.ToString(row["PAYROLL_ID"]) });
                }
            }

        }
        protected void btnFind_Click(object sender, EventArgs e)
        {
            DataTable ResponseResult ;

            Models.InvestorMergModel objIM = new Models.InvestorMergModel();
            if (txtClientSubbrokerCode.Text == "" && txtName.Text == "" && txtAdd1.Text == "" && txtAdd2.Text == "" && txtMobile.Text == "" && txtPAN.Text == "" && ddlCity.SelectedItem.Text == "Select City" && ddlBranchName.SelectedItem.Text == "Select Branch" && ddlRM.SelectedItem.Text == "Select RM" && ddlSort.SelectedItem.Text == "Select Sorting")
            {
                lblMessage.Text = "Please select Something for search.";
                lblMessage.CssClass = "text-danger";
            }
            else
            {
                objIM.CLIENTSUBBROKER = txtClientSubbrokerCode.Text.Trim().ToUpper(); ;
                objIM.CLIENT_NAME = txtName.Text.Trim().ToUpper();
                objIM.ADDRESS1 = txtAdd1.Text.Trim().ToUpper(); ;
                objIM.ADDRESS2 = txtAdd2.Text.Trim().ToUpper(); ;
                objIM.MOBILE = txtMobile.Text.Trim().ToUpper(); ;
                objIM.PAN = txtPAN.Text.Trim().ToUpper(); ;
                if (ddlCity.SelectedItem.Text == "Select City")
                { objIM.CITY = ""; }
                else { objIM.CITY = ddlCity.SelectedValue; }
                if (ddlBranchName.SelectedItem.Text == "Select Branch")
                { objIM.BRANCH_NAME = ""; }
                else { objIM.BRANCH_NAME = ddlBranchName.SelectedValue; }
                if (ddlRM.SelectedItem.Text == "Select RM")
                { objIM.RM = ""; }
                else { objIM.RM = ddlRM.SelectedValue; }
                if (ddlSort.SelectedItem.Text == "Select Sorting")
                { objIM.SORT = ""; }
                else { objIM.SORT = ddlSort.SelectedItem.Text; }
                ResponseResult = new WM.Controllers.InvestorMergeController().GetSearchForMerge(objIM);
                if (ResponseResult.Rows.Count > 0)
                {
                    lblMessage.Text = "";
                    gvInvMergeFind.DataSource = ResponseResult;
                    gvInvMergeFind.DataBind();
                }
                else
                {
                    gvInvMergeFind.DataSource = new DataTable();
                    gvInvMergeFind.DataBind();
                    lblMessage.Text = "No Matched Record Found.";
                    lblMessage.CssClass = "text-danger";
                }
            }
        }
        protected void gvInvMergeFind_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                // Get the CommandArgument value                 

                string commandArgs = e.CommandArgument.ToString();
                string[] args = commandArgs.Split(',');

                // Assuming the first part is ID and the second part is Name
                string CLIENT_CODE = args[0];
                string CLIENT_NAME = args[1];

                // Redirect to addnewstate page with the state id value 
                Response.Redirect("investormerge.aspx?INMergeid=" + CLIENT_CODE + "&ClientName=" + CLIENT_NAME);
            }
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect("searchinvestormerge.aspx");
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("investormerge.aspx");
        }
    }
}