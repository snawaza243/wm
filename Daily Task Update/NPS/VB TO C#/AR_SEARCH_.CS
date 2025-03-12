using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication
{
    public partial class SearchPage : System.Web.UI.Page
    {
        // Default values for global variables normally provided elsewhere
        private string GlbDataFilter = "72";
        private bool EPF = false;
        private string currentFormName = "frmDefault"; // Default, can be "frmNPS" or "frmpaymentreced"
        private string Branches = "'Branch1','Branch2'"; // Example branch codes for SQL IN clause
        private string SRmCode = ""; // Default RM code
        
        // Connection string from Web.config (ensure to have this key defined)
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            // On Error Resume Next equivalent is try-catch blocks around code lines where needed.
            // Local variables declaration
            string StrSql = "";
            string strSearch = "";
            string AppNo = "", ARNO = "", chequeno = "", marno = "";
            string clcode = "";
            string mtcode = "";
            string SCHCODE = "";
            string branchcode = "";
            string brokid = "";
            string rmcode1 = "";
            
            // Set default control values or states for date textboxes etc.
            // (Assume mskfromdate, msktodate, txtScheme, txtappno, txtarno, txtchequeno, txtmarno, txtInvName, txtANACD are TextBox controls defined in the .aspx page)
            
            // Set default format string for GridView if needed - GridView will auto-generate columns
            // (Original MSFlexGrid formatting information is not directly applicable to GridView)

            // Local default SQL building
            strSearch = txtScheme.Text.Trim().ToUpper();

            if (optAfter.Checked) // optAfter is assumed as a RadioButton control
            {
                if (GlbDataFilter == "72")
                {
                    StrSql = "select * from transaction_st where ";
                }
                else
                {
                    StrSql = "select * from transaction_st where (flag<>'NEWTRAN' OR FLAG IS NULL) and ";
                }
            }
            else
            {
                StrSql = "select * from transaction_sttemp where ";
            }
            
            // Validate from date
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(mskfromdate.Text, out fromDate) && mskfromdate.Text.Trim() != "__/__/____")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Enter Valid From Date');", true);
                mskfromdate.Text = "__/__/____";
                mskfromdate.Focus();
                return;
            }
            // Validate to date
            if (!DateTime.TryParse(msktodate.Text, out toDate) && msktodate.Text.Trim() != "__/__/____")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Enter Valid To Date');", true);
                msktodate.Text = "__/__/____";
                msktodate.Focus();
                return;
            }
            // Validate date range
            if (DateTime.TryParse(mskfromdate.Text, out fromDate) && DateTime.TryParse(msktodate.Text, out toDate) && mskfromdate.Text.Trim() != "__/__/____" && msktodate.Text.Trim() != "__/__/____")
            {
                // Format dates as dd/MM/yyyy and compare
                try
                {
                    DateTime dtFrom = DateTime.ParseExact(mskfromdate.Text, "dd/MM/yyyy", null);
                    DateTime dtTo = DateTime.ParseExact(msktodate.Text, "dd/MM/yyyy", null);
                    if (dtFrom > dtTo)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('From Date should be less then To Date');", true);
                        mskfromdate.Text = "__/__/____";
                        msktodate.Text = "__/__/____";
                        mskfromdate.Focus();
                        return;
                    }
                }
                catch (Exception)
                {
                    // If formatting fails, ignore and let later processing catch the error.
                }
            }
            if (txtScheme.Text.Trim() == "" && txtappno.Text.Trim() == "" && txtarno.Text.Trim() == "" && txtchequeno.Text.Trim() == ""
                && txtmarno.Text.Trim() == "" && mskfromdate.Text.Trim() == "__/__/____" && msktodate.Text.Trim() == "__/__/____"
                && txtInvName.Text.Trim() == "" && txtANACD.Text.Trim() == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please Enter Searching Parameter');", true);
                return;
            }
            
            AppNo = "";
            cmdSearch.Enabled = false;
            prgBar.Visible = true;
            prgBar.Minimum = 0;
            prgBar.Value = 0;
            
            if (txtappno.Text.Trim() != "")
            {
                AppNo = txtappno.Text.Trim();
                StrSql += "App_no='" + AppNo + "' and ";
            }
            if (txtarno.Text.Trim() != "")
            {
                ARNO = txtarno.Text.Trim();
                StrSql += " tran_code = '" + ARNO + "' and ";
            }
            if (txtchequeno.Text.Trim() != "")
            {
                chequeno = txtchequeno.Text.Trim();
                StrSql += "cheque_no LIKE '%" + chequeno + "%' and ";
            }
            if (txtmarno.Text.Trim() != "")
            {
                marno = txtmarno.Text.Trim();
                StrSql += "manual_arno='" + marno + "' and ";
            }
            
            if (EPF)
            {
                // Assume frmPayment.Opt_client is represented by a RadioButton control optClient
                if (optClient.Checked)
                {
                    StrSql += " substr(Client_code,1,1)='4' and ";
                }
                else
                {
                    StrSql += " substr(Client_code,1,1)='3' and ";
                }
            }
            // ----------------NPS Transactions-----------------------------------
            if (currentFormName == "frmNPS")
            {
                StrSql += " sch_code in('OP#09971','OP#09972','OP#09973')  and ";
            }
            if (currentFormName == "frmpaymentreced")
            {
                StrSql += " mut_code in(select iss_code from iss_master where prod_code='DT028')  and ";
            }
            // -------------------------------------------------------------------
            if (mskfromdate.Text.Trim() != "__/__/____" && msktodate.Text.Trim() != "__/__/____")
            {
                StrSql += "tr_date >= to_date('" + mskfromdate.Text + "', 'dd-MM-yyyy') and ";
                StrSql += "tr_date <= to_date('" + msktodate.Text + "', 'dd-MM-yyyy') and ";
            }
            else if (mskfromdate.Text.Trim() != "__/__/____" && msktodate.Text.Trim() == "__/__/____")
            {
                StrSql += "tr_date = to_date('" + mskfromdate.Text + "', 'dd-MM-yyyy') and ";
            }
            else if (msktodate.Text.Trim() != "__/__/____" && mskfromdate.Text.Trim() == "__/__/____")
            {
                StrSql += "tr_date = to_date('" + msktodate.Text + "', 'dd-MM-yyyy') and ";
            }
            
            // Remove any remaining open records if applicable (not needed in ADO.NET)
            // Clear gridview data
            gvResults.DataSource = null;
            gvResults.DataBind();
            
            // Reset grid column format info not applicable for GridView.            
            if (txtScheme.Text.Trim() != "")
            {
                StrSql += " (sch_code in (select sch_code from scheme_info,MUT_FUND where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) like '%" + strSearch + "%') or sch_code in (select osch_code from other_product o, product_master p,iss_master i where o.prod_class_code=p.prod_code and o.iss_code=i.iss_code AND UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) like '%" + strSearch + "%')) AND ";
            }
            if (txtInvName.Text.Trim() != "")
            {
                StrSql += " (client_code in (select distinct inv_code from investor_master where upper(investor_name) like '%" + txtInvName.Text.Trim().ToUpper() + "%')) AND ";
            }
            if (txtANACD.Text.Trim() != "")
            {
                StrSql += " (source_code in (select agent_code from agent_master where trim(exist_code) ='" + txtANACD.Text.Trim() + "')) AND ";
            }
            if (string.IsNullOrEmpty(SRmCode))
            {
                StrSql += " Branch_code in (" + Branches + ") order by tr_date";
            }
            else
            {
                StrSql += " Branch_code in (" + Branches + ") and RMCODE IN (" + SRmCode + ") order by tr_date";
            }
            
            // Execute main SQL and get result as DataTable
            DataTable dtMain = ExecMySQL(StrSql);
            if (dtMain != null && dtMain.Rows.Count > 0)
            {
                prgBar.Maximum = dtMain.Rows.Count;
            }
            Label8.Text = (dtMain != null ? dtMain.Rows.Count.ToString() : "0");
            
            // Create a DataTable to mimic grid data similar to MSFlexGrid
            DataTable dtGrid = new DataTable();
            // Add columns corresponding to grid indices as per original VB code
            dtGrid.Columns.Add("tran_code");      // col0
            dtGrid.Columns.Add("tr_date");          // col1
            dtGrid.Columns.Add("investor_name");    // col2
            dtGrid.Columns.Add("issuer_name");      // col3 (mut_fund or iss_master return)
            dtGrid.Columns.Add("scheme_name");      // col4
            dtGrid.Columns.Add("branch_name");      // col5
            dtGrid.Columns.Add("app_no");           // col6
            dtGrid.Columns.Add("app_date");         // col7
            dtGrid.Columns.Add("folio_no");         // col8
            dtGrid.Columns.Add("tran_type");        // col9
            dtGrid.Columns.Add("amount");           // col10
            dtGrid.Columns.Add("units");            // col11
            dtGrid.Columns.Add("rate");             // col12
            dtGrid.Columns.Add("nav_date");         // col13
            dtGrid.Columns.Add("payment_mode");     // col14
            dtGrid.Columns.Add("cheque_no");        // col15
            dtGrid.Columns.Add("cheque_date");      // col16
            dtGrid.Columns.Add("bank_name");        // col17
            dtGrid.Columns.Add("broker_name");      // col18
            dtGrid.Columns.Add("rm_name");          // col19
            dtGrid.Columns.Add("manual_arno");      // col20
            dtGrid.Columns.Add("mut_code");         // col21
            dtGrid.Columns.Add("sch_code");         // col22
            dtGrid.Columns.Add("client_code");      // col23
            dtGrid.Columns.Add("broker_id");        // col24
            dtGrid.Columns.Add("rmcode");           // col25
            dtGrid.Columns.Add("BUSINESS_RMCODE");  // col26
            dtGrid.Columns.Add("busi_branch_code"); // col27
            dtGrid.Columns.Add("Remark");           // col28
            dtGrid.Columns.Add("CROR_PLANNO");      // col29
            dtGrid.Columns.Add("plan_type");        // col30
            dtGrid.Columns.Add("DOC_ID");           // col31

            // Loop variables
            int i = 0;
            // Loop through each record in dtMain and populate dtGrid
            foreach (DataRow dr in dtMain.Rows)
            {
                prgBar.Value = prgBar.Value + 1;
                DataRow newRow = dtGrid.NewRow();
                try
                {
                    // col0: transaction code
                    newRow["tran_code"] = dr["tran_code"].ToString();
                }
                catch { }

                try
                {
                    // col1: transaction date formatted as dd/mm/yyyy
                    DateTime trDate;
                    if (DateTime.TryParse(dr["tr_date"].ToString(), out trDate))
                        newRow["tr_date"] = trDate.ToString("dd/MM/yyyy");
                    else
                        newRow["tr_date"] = "";
                }
                catch { }

                try
                {
                    // clcode from field client_code used later
                    clcode = dr["client_code"].ToString();
                    newRow["client_code"] = clcode;
                }
                catch { }

                try
                {
                    // col26: BUSINESS_RMCODE field
                    newRow["BUSINESS_RMCODE"] = dr["BUSINESS_RMCODE"].ToString();
                }
                catch { }

                try
                {
                    // col2: investor_name retrieved using subquery if clcode not empty
                    if (!string.IsNullOrEmpty(clcode))
                    {
                        string strsql1 = "select investor_name from investor_master where inv_code='" + clcode + "'";
                        DataTable dtRS1 = ExecMySQL(strsql1);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                        {
                            newRow["investor_name"] = dtRS1.Rows[0][0].ToString();
                        }
                        else
                        {
                            newRow["investor_name"] = "";
                        }
                    }
                    else
                    {
                        newRow["investor_name"] = "";
                    }
                }
                catch { }

                try
                {
                    // col21: mut_code
                    mtcode = dr["mut_code"].ToString();
                    newRow["mut_code"] = mtcode;
                }
                catch { }

                try
                {
                    // col3: issuer_name from mut_fund or iss_master depending on mut_code value
                    if (!string.IsNullOrEmpty(mtcode) && mtcode.Length >= 2 && mtcode.Substring(0, 2) == "MF" && (mtcode.Length < 4 || mtcode.Substring(0, 4) != "MFIS"))
                    {
                        string sqlQuery = "select mut_name from mut_fund where mut_code='" + mtcode + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["issuer_name"] = dtRS1.Rows[0][0].ToString();
                        else
                            newRow["issuer_name"] = "";
                    }
                    else
                    {
                        string sqlQuery = "select iss_name from iss_master where iss_code='" + mtcode + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["issuer_name"] = dtRS1.Rows[0][0].ToString();
                        else
                            newRow["issuer_name"] = "";
                    }
                }
                catch { }

                try
                {
                    // col22: sch_code
                    SCHCODE = dr["sch_code"].ToString();
                    newRow["sch_code"] = SCHCODE;
                }
                catch { }

                try
                {
                    // col4: scheme_name based on SCHCODE and mut_code conditions
                    if (!string.IsNullOrEmpty(SCHCODE) && !string.IsNullOrEmpty(mtcode) && ((mtcode.Length>=2 && mtcode.Substring(0, 2)=="IS") || (mtcode.Length>=4 && mtcode.Substring(0, 4)=="MFIS")))
                    {
                        string sqlQuery = "select longname||'-'||osch_name from other_product where osch_code='" + SCHCODE + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["scheme_name"] = dtRS1.Rows[0][0].ToString();
                    }
                    else if (!string.IsNullOrEmpty(SCHCODE) && !string.IsNullOrEmpty(mtcode) && mtcode.Length >= 2 && mtcode.Substring(0, 2) == "MF" && (mtcode.Length < 4 || mtcode.Substring(0, 4) != "MFIS"))
                    {
                        string sqlQuery = "select SHORT_NAME from scheme_info where sch_code='" + SCHCODE + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["scheme_name"] = dtRS1.Rows[0][0].ToString();
                    }
                    else
                    {
                        newRow["scheme_name"] = "";
                    }
                }
                catch { }

                try
                {
                    // col5: branch_name from branch_master using branch_code
                    branchcode = dr["branch_code"].ToString();
                    if (!string.IsNullOrEmpty(branchcode))
                    {
                        string sqlQuery = "select branch_name from branch_master where branch_code='" + branchcode + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["branch_name"] = dtRS1.Rows[0][0].ToString();
                        else
                            newRow["branch_name"] = "";
                    }
                    else
                    {
                        newRow["branch_name"] = "";
                    }
                }
                catch { }

                try
                {
                    // col6: App_no
                    newRow["app_no"] = dr["app_no"].ToString();
                }
                catch { }
                
                try
                {
                    // col7: App_date formatted
                    DateTime appDate;
                    if (DateTime.TryParse(dr["app_date"].ToString(), out appDate))
                        newRow["app_date"] = appDate.ToString("dd/MM/yyyy");
                    else
                        newRow["app_date"] = "";
                }
                catch { }
                
                try
                {
                    // col8: folio_no
                    newRow["folio_no"] = dr["folio_no"].ToString();
                }
                catch { }
                
                try
                {
                    // col9: tran_type
                    newRow["tran_type"] = dr["tran_type"].ToString();
                }
                catch { }
                
                try
                {
                    // col10: amount
                    newRow["amount"] = dr["amount"].ToString();
                }
                catch { }
                
                try
                {
                    // col11: units
                    newRow["units"] = dr["units"].ToString();
                }
                catch { }
                
                try
                {
                    // col12: rate
                    newRow["rate"] = dr["rate"].ToString();
                }
                catch { }
                
                try
                {
                    // col13: nav_date formatted
                    DateTime navDate;
                    if (DateTime.TryParse(dr["nav_date"].ToString(), out navDate))
                        newRow["nav_date"] = navDate.ToString("dd/MM/yyyy");
                    else
                        newRow["nav_date"] = "";
                }
                catch { }
                
                try
                {
                    // col14: payment_mode
                    newRow["payment_mode"] = dr["payment_mode"].ToString();
                }
                catch { }
                
                try
                {
                    // col15: cheque_no
                    newRow["cheque_no"] = dr["cheque_no"].ToString();
                }
                catch { }
                
                try
                {
                    // col16: cheque_date formatted
                    DateTime chequeDate;
                    if (DateTime.TryParse(dr["cheque_date"].ToString(), out chequeDate))
                        newRow["cheque_date"] = chequeDate.ToString("dd/MM/yyyy");
                    else
                        newRow["cheque_date"] = "";
                }
                catch { }
                
                try
                {
                    // col17: bank_name
                    newRow["bank_name"] = dr["bank_name"].ToString();
                }
                catch { }
                
                try
                {
                    // col18: broker_name from broker_master using broker_id field (brokid)
                    brokid = dr["broker_id"].ToString();
                    newRow["broker_id"] = brokid; // also assign broker_id in col24 later
                    if (!string.IsNullOrEmpty(brokid))
                    {
                        string sqlQuery = "select name from broker_master where broker_id='" + brokid + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["broker_name"] = dtRS1.Rows[0][0].ToString();
                        else
                            newRow["broker_name"] = "";
                    }
                    else
                    {
                        newRow["broker_name"] = "";
                    }
                }
                catch { }
                
                try
                {
                    // col25: rmcode
                    rmcode1 = dr["rmcode"].ToString();
                    newRow["rmcode"] = rmcode1;
                }
                catch { }
                
                try
                {
                    // col29: CROR_PLANNO
                    newRow["CROR_PLANNO"] = dr["CROR_PLANNO"].ToString();
                }
                catch { }
                
                try
                {
                    // col30: plan_type
                    newRow["plan_type"] = dr["plan_type"].ToString();
                }
                catch { }
                
                try
                {
                    // col31: DOC_ID
                    newRow["DOC_ID"] = dr["DOC_ID"].ToString();
                }
                catch { }
                
                try
                {
                    // col19: rm_name from employee_master using rmcode1
                    if (!string.IsNullOrEmpty(rmcode1))
                    {
                        string sqlQuery = "select rm_name from employee_master where rm_code='" + rmcode1 + "'";
                        DataTable dtRS1 = ExecMySQL(sqlQuery);
                        if (dtRS1 != null && dtRS1.Rows.Count > 0)
                            newRow["rm_name"] = dtRS1.Rows[0][0].ToString();
                        else
                            newRow["rm_name"] = "";
                    }
                    else
                    {
                        newRow["rm_name"] = "";
                    }
                }
                catch { }
                
                try
                {
                    // col20: manual_arno
                    newRow["manual_arno"] = dr["manual_arno"].ToString();
                }
                catch { }
                
                try
                {
                    // col27: busi_branch_code
                    newRow["busi_branch_code"] = dr["busi_branch_code"].ToString();
                }
                catch { }
                
                try
                {
                    // col28: Remark
                    newRow["Remark"] = dr["Remark"].ToString();
                }
                catch { }
                
                // col24 already set as broker_id above

                // Highlight row logic: if tran_type equals "REVERTAL"
                // In GridView, we will mark a flag in the DataTable so that in RowDataBound event we can highlight.
                // Here we add an extra column "Highlight" to dtGrid (if not already added) and set it.
                if (newRow["tran_type"].ToString() == "REVERTAL")
                {
                    // We add a temporary column "Highlight" if not exist.
                    if (!dtGrid.Columns.Contains("Highlight"))
                    {
                        dtGrid.Columns.Add("Highlight", typeof(bool));
                    }
                    newRow["Highlight"] = true;
                }
                else
                {
                    if (dtGrid.Columns.Contains("Highlight"))
                        newRow["Highlight"] = false;
                }
                
                dtGrid.Rows.Add(newRow);
                i = i + 1;
            }
            
            // Bind the DataTable to GridView
            gvResults.DataSource = dtGrid;
            gvResults.DataBind();
            
            gvResults.Focus();
            Label7.Visible = true;
            Label8.Text = (dtGrid.Rows.Count).ToString();
            cmdSearch.Enabled = true;
            prgBar.Visible = false;
        }
        
        /// <summary>
        /// Executes the provided SQL query and returns a DataTable.
        /// </summary>
        /// <param name="sql">The SQL query string</param>
        /// <returns>DataTable containing query results</returns>
        private DataTable ExecMySQL(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = 60;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In VB code On Error Resume Next would skip errors.
                // Here we catch exceptions and continue.
            }
            return dt;
        }
        
        // Event for GridView row data bound to highlight rows that meet criteria
        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Check if this is a data row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;
                if (drv != null && drv.Row.Table.Columns.Contains("Highlight"))
                {
                    bool highlight = false;
                    Boolean.TryParse(drv["Highlight"].ToString(), out highlight);
                    if (highlight)
                    {
                        e.Row.BackColor = System.Drawing.Color.Yellow;
                    }
                }
            }
        }
        
        // The following are definitions for the controls used on the page.
        // They are assumed to be declared in the .aspx page and linked via the designer.
        protected TextBox txtScheme;
        protected TextBox txtappno;
        protected TextBox txtarno;
        protected TextBox txtchequeno;
        protected TextBox txtmarno;
        protected TextBox mskfromdate;
        protected TextBox msktodate;
        protected TextBox txtInvName;
        protected TextBox txtANACD;
        protected RadioButton optAfter;
        protected RadioButton optClient;
        protected Button cmdSearch;
        protected ProgressBar prgBar; // Assume prgBar is a custom user control or replaced by a Literal showing progress
        protected Label Label7;
        protected Label Label8;
        protected GridView gvResults;
    }
}
  
// A dummy ProgressBar control for demonstration purposes.
public class ProgressBar : WebControl
{
    public int Minimum { get; set; }
    public int Maximum { get; set; }
    public int Value { get; set; }
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        writer.Write("Progress: " + Value + " of " + Maximum);
        writer.RenderEndTag();
    }
}
