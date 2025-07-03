using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Models;
using System.IO;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Web.Services.Description;
using WM.Controllers;

namespace WM.Masters
{
    public partial class MakerChecker : System.Web.UI.Page
    {
        PsmController pc = new PsmController();
        DataTable dtbr = new DataTable();
        string rta_tran_code;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LoginId"] = "112650";
            Session["roleId"] = "29";

            if (!IsPostBack)
            {
                BindGrid();
                dtFrom.Text = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                dtTo.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        public void BindGrid()
        {
            ListItem item = new ListItem
            {
                Text = "ALL",
                Value = "ALL"
            };

            DataTable dt = new DataTable();
            dt = new WM.Controllers.MakerCheckerController().GetRegionList(pc.currentLoginID(), pc.currentRoleID(), "", "", "", "");

            regionIDC.DataSource = dt;
            regionIDC.DataTextField = "REGION_NAME";
            regionIDC.DataValueField = "REGION_ID";
            regionIDC.DataBind();
            regionIDC.Items.Insert(0, item);
        }

        protected void regionIDC_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem itemzone = new ListItem
            {
                Text = "ALL",
                Value = "ALL"
            };

            DataTable dtzone = new DataTable();
            dtzone = new WM.Controllers.MakerCheckerController().GetZoneByRegionId(regionIDC.SelectedValue.ToString());

            zone.DataSource = dtzone;
            zone.DataTextField = "ZONE_NAME";
            zone.DataValueField = "ZONE_ID";
            zone.DataBind();
            zone.Items.Insert(0, itemzone);

            ListItem itemAMC = new ListItem
            {
                Text = "ALL",
                Value = "ALL"
            };
            DataTable dt = new DataTable();
            dt = new WM.Controllers.BrokerageController().GetAMCList();
            amc.DataSource = dt;
            amc.DataTextField = "mut_name";
            amc.DataValueField = "mut_code";
            amc.DataBind();
            amc.Items.Insert(0, itemAMC);
        }

        protected void zone_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem itembr = new ListItem
            {
                Text = "ALL",
                Value = "ALL"
            };

            DataTable dtbr = new DataTable();
            dtbr = new WM.Controllers.MakerCheckerController().GetBranchNameCode(Session["LoginId"].ToString(), Session["roleId"].ToString());

            branch.DataSource = dtbr;
            branch.DataTextField = "BRANCH_NAME";
            branch.DataValueField = "BRANCH_CODE";
            branch.DataBind();
            branch.Items.Insert(0, itembr);
        }

        protected void branch_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem itembr = new ListItem
            {
                Text = "ALL",
                Value = "ALL"
            };

            DataTable dtbr = new DataTable();
            dtbr = new WM.Controllers.MakerCheckerController().GetRMByBranchCode(Session["LoginId"].ToString(), Session["roleId"].ToString(), branch.SelectedValue.ToString());

            rm.DataSource = dtbr;
            rm.DataTextField = "RM_NAME";
            rm.DataValueField = "PAYROLL_ID";
            rm.DataBind();
            rm.Items.Insert(0, itembr);
        }

        protected void go_Click(object sender, EventArgs e)
        {
            try
            {
                #region Validation and searching
                // before getin any record firslt reset all the grids and session
                Session["TRCode"] = null;
                Session["RTATranCode"] = null;
                GridView1.DataSource = null;
                GridView1.DataBind();
                GridView2.DataSource = null;
                GridView2.DataBind();


                Models.MFMakerChecker objMFMakerChecker = new Models.MFMakerChecker();
                objMFMakerChecker.LOGIN_ID = Session["LoginId"].ToString();
                objMFMakerChecker.ROLE_ID = Session["roleId"].ToString();

                objMFMakerChecker.BRANCH_CD = (branch.Text != "ALL" && branch.Text != "") ? branch.SelectedValue : string.Empty;
                objMFMakerChecker.REGION_ID = (regionIDC.Text != "ALL" && regionIDC.Text != "") ? regionIDC.SelectedValue : string.Empty;
                objMFMakerChecker.ZONE_ID = (zone.Text != "ALL" && zone.Text != "") ? zone.SelectedValue : string.Empty;
                objMFMakerChecker.RM_ID = (rm.Text != "ALL" && rm.Text != "") ? rm.SelectedValue : string.Empty;
                objMFMakerChecker.AR_NO = (txtAR.Text != "") ? txtAR.Text : string.Empty;
                objMFMakerChecker.AMC = (amc.Text != "ALL" && amc.Text != "") ? amc.SelectedValue : string.Empty;

                objMFMakerChecker.AUTO_REC = autoReconciled.Checked ? "1" : string.Empty;
                objMFMakerChecker.MANUAL_REC = manualReconciled.Checked ? "1" : string.Empty;

                objMFMakerChecker.DATEFROM = (string.IsNullOrEmpty(txtAR.Text)) ? dtFrom.Text.ToString() : null;
                objMFMakerChecker.DATETO = (string.IsNullOrEmpty(dtTo.Text)) ? dtFrom.Text.ToString() : null;

                dtbr = new WM.Controllers.MakerCheckerController().GetInvestorMC(objMFMakerChecker);


                GridView1.DataSource = dtbr;
                GridView1.DataBind();

                UpdatePanelRTA.Update();

                #endregion
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }

        protected void unreconcile_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TRCode"] == null || Session["RTATranCode"] == null)
                {
                    pc.ShowAlert(this, "No transaction selected for unreconciliation.");
                    return;
                }

                string status = new WM.Controllers.MakerCheckerController().UnreconcileRecordMC(
                    Session["TRCode"].ToString(),
                    Session["RTATranCode"].ToString());

                string message = status == "ok"
                    ? "Unreconciled Successfully Done."
                    : "Some Issue Please contact Admin.";

                pc.ShowAlert(this, message);

                // Refresh the grid after unreconciliation
                go_Click(sender, e);
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
            }
        }

        protected void autoReconciled_CheckedChanged(object sender, EventArgs e)
        {
            if (autoReconciled.Checked)
            {
                manualReconciled.Checked = false;
            }
        }

        protected void manualReconciled_CheckedChanged(object sender, EventArgs e)
        {
            if (manualReconciled.Checked)
            {
                autoReconciled.Checked = false;
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            go_Click(sender, e); // Rebind data with current filters
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                // Get the clicked row
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

                // Clear previous selection
                foreach (GridViewRow r in GridView1.Rows)
                {
                    r.CssClass = "";
                    LinkButton lnk = (LinkButton)r.FindControl("lnkSelect");
                    if (lnk != null)
                    {
                        lnk.Text = "Select";
                        lnk.CssClass = "btn btn-sm btn-outline-primary";
                    }
                }

                // Highlight current row
                row.CssClass = "highlighted";
                LinkButton lnkSelect = (LinkButton)row.FindControl("lnkSelect");
                if (lnkSelect != null)
                {
                    lnkSelect.Text = "Selected";
                    lnkSelect.CssClass = "btn btn-sm btn-primary selected-link";
                }

                // Rest of your existing RowCommand code
                string TranCode = e.CommandArgument.ToString();
                Session["TRCode"] = TranCode;

                if (!string.IsNullOrEmpty(TranCode))
                {
                    using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                    {
                        try
                        {
                            con.Open();
                            using (OracleCommand cmd = new OracleCommand(
                                "select rta_tran_code from transaction_mf_temp1 where tran_code='" + TranCode + "'", con))
                            {
                                rta_tran_code = cmd.ExecuteScalar()?.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            pc.ShowAlert(this, "Error retrieving RTA transaction: " + ex.Message);
                            return;
                        }
                    }

                    DataTable dtRTA = new DataTable();
                    Models.MFMakerChecker objMFMakerChecker = new Models.MFMakerChecker();
                    objMFMakerChecker.AR_NO = TranCode;

                    dtRTA = new WM.Controllers.MakerCheckerController().GetRTADetails(objMFMakerChecker);
                    Session["RTATranCode"] = rta_tran_code;

                    GridView2.DataSource = dtRTA;
                    GridView2.DataBind();
                }
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the TRAN_CODE for this row
                string tranCode = DataBinder.Eval(e.Row.DataItem, "TRAN_CODE").ToString();

                // Check if this is the selected row
                if (Session["TRCode"] != null && tranCode == Session["TRCode"].ToString())
                {
                    e.Row.CssClass = "highlighted";
                    LinkButton lnkSelect = (LinkButton)e.Row.FindControl("lnkSelect");
                    if (lnkSelect != null)
                    {
                        lnkSelect.Text = "Selected";
                        lnkSelect.CssClass = "btn btn-sm btn-primary selected-link";
                    }
                }
            }
        }

        /*
             OnRowCommand="GridView1_RowCommand" 
    OnPageIndexChanging="GridView1_PageIndexChanging"
    CssClass="grid-view table table-hover"
    DataKeyNames="TRAN_CODE"
    OnRowDataBound="GridView1_RowDataBound"
    OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
         */
        protected void AuditAR_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["TRCode"] == null)
                {
                    lblMessage.Text = "No transaction selected for audit.";
                    lblMessage.CssClass = "text-danger";
                    return;
                }

                Models.MFMakerChecker objMFMakerChecker = new Models.MFMakerChecker();
                objMFMakerChecker.AR_NO = Session["TRCode"].ToString();

                string status = new WM.Controllers.MakerCheckerController().AuditAR(objMFMakerChecker);

                if (status == "ok")
                {
                    //lblMessage.Text = "Audit Successfully Done.";
                    //lblMessage.CssClass = "text-success";

                    pc.ShowAlert(this, "Audit Successfully Done.");

                    // Better approach to refresh the RTA transactions grid
                    string TranCode = Session["TRCode"].ToString();
                    DataTable dtRTA = new DataTable();
                    Models.MFMakerChecker rtaObj = new Models.MFMakerChecker();
                    rtaObj.AR_NO = TranCode;

                    dtRTA = new WM.Controllers.MakerCheckerController().GetRTADetails(rtaObj);
                    GridView2.DataSource = dtRTA;
                    GridView2.DataBind();
                }
                else
                {
                    lblMessage.Text = "Some Issue Please contact Admin.";
                    lblMessage.CssClass = "text-danger";
                }
            }

            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }

        protected void reset_Click(object sender, EventArgs e)
        {

            try
            {

                Session["RTATranCode"] = null;
                Session["TRCode"] = null;

                Response.Redirect(Request.Url.AbsoluteUri);

            }

            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }
    }
}