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

        // creat loginis and roel id session with 123 and 212
        //Session["LoginId"] = "123";
        //Session["roleId"] = "212";

       


        DataTable dtbr = new DataTable();
        string rta_tran_code;
        //string login_id = "38387";
        //string role_id = "261";
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

        public void BindGrid() {
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
           
            Models.MFMakerChecker objMFMakerChecker = new Models.MFMakerChecker();
            objMFMakerChecker.LOGIN_ID = Session["LoginId"].ToString();
            objMFMakerChecker.ROLE_ID = Session["roleId"].ToString();
            if (branch.Text != "ALL" && branch.Text!="")
            {
                objMFMakerChecker.BRANCH_CD = branch.SelectedValue;
            }
            else
            {
                objMFMakerChecker.BRANCH_CD = string.Empty;
            }
            if (regionIDC.Text != "ALL" && regionIDC.Text != "")
            {
                objMFMakerChecker.REGION_ID = regionIDC.SelectedValue;
            }
            else
            {
                objMFMakerChecker.REGION_ID =string.Empty;
            }
            if (zone.Text != "ALL" && zone.Text != "")
            {
                objMFMakerChecker.ZONE_ID = zone.SelectedValue;
            }
            else
            {
                objMFMakerChecker.ZONE_ID = string.Empty;
            }
            if (rm.Text != "ALL" && rm.Text != "")
            {
                objMFMakerChecker.RM_ID = rm.SelectedValue;
            }
            else
            {
                objMFMakerChecker.RM_ID = string.Empty;
            }
            if (txtAR.Text != "")
            {
                objMFMakerChecker.AR_NO = txtAR.Text;
            }
            else
            {
                objMFMakerChecker.AR_NO = string.Empty;
            }
            if (amc.Text != "ALL" && amc.Text != "")
            {
                objMFMakerChecker.AMC = amc.SelectedValue;
            }
            else
            {
                objMFMakerChecker.AMC = string.Empty;
            }
            if(autoReconciled.Checked==true)
            {
                objMFMakerChecker.AUTO_REC = "1";
            }
            else
            {
                objMFMakerChecker.AUTO_REC = string.Empty;
            }

            if (manualReconciled.Checked == true)
            {
                objMFMakerChecker.MANUAL_REC = "1";
            }
            else
            {
                objMFMakerChecker.MANUAL_REC = string.Empty;
            }
            objMFMakerChecker.DATEFROM = (string.IsNullOrEmpty(txtAR.Text)) ? dtFrom.Text.ToString() : null;
            objMFMakerChecker.DATETO = (string.IsNullOrEmpty(dtTo.Text)) ? dtFrom.Text.ToString() : null;


            dtbr = new WM.Controllers.MakerCheckerController().GetInvestorMC(objMFMakerChecker);
            GridView1.DataSource = dtbr;
            GridView1.DataBind();
           
        }

        protected void unreconcile_Click(object sender, EventArgs e)
        {
            try
            {
                string status = new WM.Controllers.MakerCheckerController().UnreconcileRecordMC(Session["TRCode"].ToString(),Session["RTATranCode"].ToString());
                string returned = string.Empty;
                if (status == "ok")
                {
                    returned =  "Unreconciled Successfully Done.";
                }
                else
                {
                    returned = "Some Issue Please contact Admin.";
                }

                if(!string.IsNullOrEmpty(returned))
                {
                    pc.ShowAlert(this, returned);
                    return;
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }

        protected void autoReconciled_CheckedChanged(object sender, EventArgs e)
        {
            if(manualReconciled.Checked==true)
            {
                manualReconciled.Checked = false;
            }
        }

        protected void manualReconciled_CheckedChanged(object sender, EventArgs e)
        {
            if (autoReconciled.Checked == true)
            {
                autoReconciled.Checked = false;
            }
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //// Set the new page index
            //GridView1.PageIndex = e.NewPageIndex;

            //// Rebind the data to reflect the new page
            //DataTable dtbr = new DataTable();
            //Models.MFMakerChecker objMFMakerChecker = new Models.MFMakerChecker();
            //objMFMakerChecker.LOGIN_ID = Session["LoginId"].ToString();
            //objMFMakerChecker.ROLE_ID = Session["roleId"].ToString();
            //if (branch.Text != "ALL" && branch.Text != "")
            //{
            //    objMFMakerChecker.BRANCH_CD = branch.SelectedValue;
            //}
            //else
            //{
            //    objMFMakerChecker.BRANCH_CD = string.Empty;
            //}
            //if (regionIDC.Text != "ALL" && regionIDC.Text != "")
            //{
            //    objMFMakerChecker.REGION_ID = regionIDC.SelectedValue;
            //}
            //else
            //{
            //    objMFMakerChecker.REGION_ID = string.Empty;
            //}
            //if (zone.Text != "ALL" && zone.Text != "")
            //{
            //    objMFMakerChecker.ZONE_ID = zone.SelectedValue;
            //}
            //else
            //{
            //    objMFMakerChecker.ZONE_ID = string.Empty;
            //}
            //if (rm.Text != "ALL" && rm.Text != "")
            //{
            //    objMFMakerChecker.RM_ID = rm.SelectedValue;
            //}
            //else
            //{
            //    objMFMakerChecker.RM_ID = string.Empty;
            //}
            //if (txtAR.Text != "")
            //{
            //    objMFMakerChecker.AR_NO = txtAR.Text;
            //}
            //else
            //{
            //    objMFMakerChecker.AR_NO = string.Empty;
            //}
            //if (amc.Text != "ALL" && amc.Text != "")
            //{
            //    objMFMakerChecker.AMC = amc.SelectedValue;
            //}
            //else
            //{
            //    objMFMakerChecker.AMC = string.Empty;
            //}
            //if (autoReconciled.Checked == true)
            //{
            //    objMFMakerChecker.AUTO_REC = "1";
            //}
            //else
            //{
            //    objMFMakerChecker.AUTO_REC = string.Empty;
            //}

            //if (manualReconciled.Checked == true)
            //{
            //    objMFMakerChecker.MANUAL_REC = "1";
            //}
            //else
            //{
            //    objMFMakerChecker.MANUAL_REC = string.Empty;
            //}
            //objMFMakerChecker.DATEFROM = dtFrom.Text.ToString();
            ////objMFMakerChecker.DATEFROM= DateTime.ParseExact(dtFrom.Text.ToString(), "dd/MM/yyyy", CultureInfo.CurrentCulture).ToString();
            //objMFMakerChecker.DATETO = dtTo.Text.ToString();
            ////objMFMakerChecker.DATETO = DateTime.ParseExact(dtTo.Text.ToString(), "dd/MM/yyyy", CultureInfo.CurrentCulture).ToString();


            //dtbr = new WM.Controllers.MakerCheckerController().GetInvestorMC(objMFMakerChecker);
            //GridView1.DataSource = dtbr;
            //GridView1.DataBind();
        }

   

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                // Get the CommandArgument value  
                string TranCode = e.CommandArgument.ToString();
                Session["TRCode"] = TranCode;
                if (TranCode != "")
                {
                    using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                    {
                        try
                        {
                            con.Open();

                            using (OracleCommand cmd = new OracleCommand("select rta_tran_code from transaction_mf_temp1 where tran_code='" + TranCode + "'", con))
                            {
                                 rta_tran_code = cmd.ExecuteScalar().ToString();
                            }
                        }
                        catch
                        {

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
                else
                {

                }


            }
        }

        protected void AuditAR_Click(object sender, EventArgs e)
        {
            if (GridView2.Rows.Count > 0)
            {
                Models.MFMakerChecker objMFMakerChecker = new Models.MFMakerChecker();
                objMFMakerChecker.AR_NO = Session["TRCode"].ToString();
                string status = new WM.Controllers.MakerCheckerController().AuditAR(objMFMakerChecker);
                if (status == "ok")
                {
                    lblMessage.Text = "Audit Successfully Done.";
                }
                else
                {
                    lblMessage.Text = "Some Issue Please contact Admin.";
                }
            }
            else
            {
                lblMessage.Text = "No Row Found For Audit.";
                lblMessage.CssClass = "text-danger";
            }
            

        }

        protected void reset_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/masters/MakerChecker.aspx");
        }
    }
}