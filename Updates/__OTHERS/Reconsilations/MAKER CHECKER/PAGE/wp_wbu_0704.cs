using System; 
using System.Data; 
using System.Web.UI.WebControls; 
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration; 
using WM.Controllers;
using System.Web.UI;
using MathNet.Numerics.Distributions;
using NPOI.SS.Formula.Functions;
using System.Globalization;
using WM.Models;

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
                dtFrom.Text = "01/01/2025"; // DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                dtTo.Text = "01/03/2025"; // DateTime.Now.ToString("dd/MM/yyyy");
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
            go_Click(sender, e); // Rebind data
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "SelectRow")
                {
                    /*
                    string tranCode = e.CommandArgument?.ToString();

                    // Clear all row styles first
                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        row.BackColor = System.Drawing.Color.White;
                        row.CssClass = string.Empty;
                    }

                    // Loop to find the row with matching TRAN_CODE
                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        string rowTranCode = GridView1.DataKeys[row.RowIndex].Value.ToString();

                        if (rowTranCode == tranCode)
                        {
                            // Highlight the matching row
                            row.BackColor = System.Drawing.Color.LightBlue;
                            row.CssClass = "selected-row"; // Optional: CSS class
                            break;
                        }
                    }
                    
                    */


                    string tranCode = e.CommandArgument?.ToString();

                    // Store the selected TRAN_CODE in session
                    Session["TRCode"] = tranCode;

                    // Reload grid to trigger RowDataBound for styling
                    BindGrid(); // Replace this with your actual GridView data binding function

                    // Load transaction details
                    LoadTransactionDetails(tranCode);

                    // Update the UpdatePanel
                    UpdatePanelRTA.Update();

                    
                     
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the TRAN_CODE value
                string tranCode = DataBinder.Eval(e.Row.DataItem, "TRAN_CODE").ToString();

                // Set the row ID and data attribute for JavaScript
                e.Row.Attributes["id"] = "row_" + tranCode;
                e.Row.Attributes["data-trancode"] = tranCode;

                // Highlight selected row
                if (Session["TRCode"] != null && tranCode == Session["TRCode"].ToString())
                {
                    e.Row.CssClass = "selected-row";

                    // Register script to scroll to this row
                    string script = $"document.getElementById('row_{tranCode}').scrollIntoView({{ behavior: 'auto', block: 'center' }});";
                    // antoehr script to set that row backgroud crolor blue and and rest of the that row bg colro hwite
                    e.Row.BackColor = System.Drawing.Color.LightBlue;
                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        if (row.RowType == DataControlRowType.DataRow && row != e.Row)
                        {
                            row.BackColor = System.Drawing.Color.White; // Reset other rows
                            row.CssClass = string.Empty; // Remove the class from other rows
                        }
                    }
                    // Set the background color of the selected row
                    e.Row.BackColor = System.Drawing.Color.LightBlue; // Set the background color of the selected row

                    // Only apply style to selected row
                    e.Row.CssClass = "selected-row";


                    ScriptManager.RegisterStartupScript(this, this.GetType(), "scrollToRow", script, true);
                }
            }
        }


        #region AR SEARCH GRID

        private int SelectedRowIndex
        {
            get
            {
                return ViewState["SelectedRowIndex"] != null ? (int)ViewState["SelectedRowIndex"] : -1;
            }
            set
            {
                ViewState["SelectedRowIndex"] = value;
            }
        }


        protected void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)sender;

                if (!chk.Checked)
                {
                     
                    return;
                }
                GridViewRow row = (GridViewRow)chk.NamingContainer;

                // Uncheck others and reset their background
                foreach (GridViewRow r in GridView1.Rows)
                {
                    if (r.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox otherChk = (CheckBox)r.FindControl("chkSelect");
                        if (otherChk != null && r != row)
                        {
                            otherChk.Checked = false;
                            r.BackColor = System.Drawing.Color.White; // reset
                        }
                    }
                }

                foreach (GridViewRow r in GridView1.Rows)
                {
                    if (r.RowType == DataControlRowType.DataRow && r != row)
                    {
                        r.CssClass = string.Empty; // Remove the class from other rows
                    }
                }


                row.BackColor = chk.Checked ? System.Drawing.Color.LightBlue : System.Drawing.Color.White;

                row.CssClass = chk.Checked ? "selected-row" : string.Empty;







                int rowIndex = row.RowIndex;


                // If your grid has a header, adjust the index to skip it
                if (GridView1.HeaderRow != null && rowIndex >= 0)
                {
                    hfSelectedRow.Value = rowIndex.ToString();
                }
                SelectedRowIndex = row.RowIndex;

                GridView1.Focus();

                // Get values from the selected row
                HiddenField hfTranCode = (HiddenField)row.FindControl("hfTranCode");
                string tranCodetrap = hfTranCode.Value;
                 
                UpdatePanelRTA.Update();

                return;

            }

            catch (Exception ex)
            {
                // Handle any unexpected errors
                // Optionally, log the error or display a message to the user
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void HandlePostbackArSearchRowClick()
        {
            string eventTarget = Request["__EVENTTARGET"];
            string eventArgument = Request["__EVENTARGUMENT"];

            System.Diagnostics.Debug.WriteLine($"EventTarget: {eventTarget}");
            System.Diagnostics.Debug.WriteLine($"EventArgument: {eventArgument}");
            System.Diagnostics.Debug.WriteLine($"GridView UniqueID: {GridView1.UniqueID}");

            if (!string.IsNullOrEmpty(eventTarget) &&
                eventTarget.Equals(GridView1.UniqueID, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(eventArgument) &&
                eventArgument.StartsWith("RowDoubleClick:", StringComparison.OrdinalIgnoreCase)) // Changed to match your actual argument
            {
                int rowIndex = int.Parse(eventArgument.Split(':')[1]);
                //string tranCode = hftran1stcode.Value;
                //hftran1stcode.Value = rowIndex.ToString(); // Store the row index in the hidden field

                ProcessSelectedRow1(rowIndex);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Postback condition not met");
            }
        }

        private void ProcessSelectedRow1(int rowIndex)
        {
            try
            {
                GridViewRow row = GridView1.Rows[rowIndex];
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");

                if (chkSelect != null)
                {
                    // Check this checkbox
                    chkSelect.Checked = true;

                    // Uncheck all others manually (replicating logic)
                    foreach (GridViewRow r in GridView1.Rows)
                    {
                        if (r.RowIndex != rowIndex && r.RowType == DataControlRowType.DataRow)
                        {
                            CheckBox otherChk = (CheckBox)r.FindControl("chkSelect");
                            if (otherChk != null)
                            {
                                otherChk.Checked = false;
                                r.BackColor = System.Drawing.Color.White;
                            }
                        }
                    }

                    row.BackColor = System.Drawing.Color.LightBlue;

                    // Now call the CheckedChanged handler manually
                    chkSelect_CheckedChanged(chkSelect, EventArgs.Empty);
                    pc.ShowAlert(this, "this is sample");
                }

                UpdatePanelRTA.Update();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"ProcessSelectedRow1 error: {ex.Message}");
            }
        }



        protected void tableSearchResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add double click event to each row
                e.Row.Attributes["ondblclick"] = "onRowDoubleClick(this)";
                e.Row.Style["cursor"] = "pointer";

                // Find the checkbox control
                CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
                if (chkSelect != null)
                {
                    // Add client-side click handler to prevent event bubbling
                    chkSelect.Attributes["onclick"] = "event.stopPropagation();";
                }
            }
        }

        protected void tableSearchResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RowDoubleClick")
            {
                string[] args = e.CommandArgument.ToString().Split(':');
                if (args.Length == 2 && args[0] == "RowDoubleClick")
                {
                    int rowIndex = Convert.ToInt32(args[1]);
                    hfSelectedRow.Value = rowIndex.ToString();
                    ProcessSelectedRow1(rowIndex);
                     
                }
            }
        }

        #endregion







        protected void LoadTransactionDetails(string TranCode)
        {
            try
            {

                if (!string.IsNullOrEmpty(TranCode))
                {
                    Session["TRCode"] = TranCode;

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

            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }

        
        
        
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
                    pc.ShowAlert(this, "Audit Successfully Done.");
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