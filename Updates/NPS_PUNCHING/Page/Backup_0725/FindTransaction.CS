using System;
using System.Data;
using System.Reflection.Emit;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HSSF.Record;
using Oracle.ManagedDataAccess.Client;
using WM.Controllers; 
using Label = System.Web.UI.WebControls.ListItem; 
using System.Globalization;
using System.Web;



namespace WM.Tree
{
    public partial class FindTransaction : System.Web.UI.Page
    {
        PsmController pc= new PsmController();

        string GlbDataFilter = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            // get current log nad role id from session
            string loginId = Session["LoginId"]?.ToString();
            string roleId = Session["RoleId"]?.ToString();

            // chekc if null then redirect to welcome page using pmscontroller welcome page redireciton
            if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(roleId))
            {
                pc.RedirectToWelcomePage();
            }
            else
            {
                if (!IsPostBack)
                {
                    LoadSchemeDropdown();
                }
            }

        }

        public static void ParseQueryStringNPSTRFind(out string findTrFlag)
        {
            // string findTrFlag = string.Empty; // NPS_TR

            var queryString = HttpContext.Current.Request.QueryString;

            // SEARCH_TYPE=NPS_TR
            findTrFlag = queryString["SEARCH_TYPE"]; 
        }

        private void LoadSchemeDropdown()
        {
            ddlSchemas.Items.Clear(); 
            ddlSchemas.Items.Add(new ListItem("Scheme", ""));
            ddlSchemas.Items.Add(new ListItem("New Pension Scheme Tier 1", "OP#09971"));
            ddlSchemas.Items.Add(new ListItem("New Pension Scheme Tier 2", "OP#09972"));
            ddlSchemas.Items.Add(new ListItem("New Pension Scheme Tier 1+2", "OP#09973"));
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                NewSearchAR();
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }

        protected string GetUIStringValueToController(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return input;
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Add double-click event for JS function, passing the row itself
                e.Row.Attributes["ondblclick"] = "onGridRowDoubleClick(this);";

                // Make cursor pointer to indicate interactivity
                e.Row.Style["cursor"] = "pointer";


                /*DataRowView drv = e.Row.DataItem as DataRowView;
                if (drv != null && drv.Row.Table.Columns.Contains("Highlight"))
                {
                    bool highlight = false;
                    Boolean.TryParse(drv["Highlight"].ToString(), out highlight);

                    if (highlight)
                    {
                        e.Row.BackColor = System.Drawing.Color.Yellow;
                    }

                    // Add a data attribute for JavaScript to read the highlight value
                    e.Row.Attributes["data-highlight"] = drv["Highlight"].ToString();

                    // Add double-click event for JS function, passing the row itself
                    e.Row.Attributes["ondblclick"] = "onGridRowDoubleClick(this);";

                    // Make cursor pointer to indicate interactivity
                    e.Row.Style["cursor"] = "pointer";

                 // Add double-click event for JS function, passing the row itself
                e.Row.Attributes["ondblclick"] = "onGridRowDoubleClick(this);";

                // Make cursor pointer to indicate interactivity
                e.Row.Style["cursor"] = "pointer";
                }
                */
            }
        }





        public bool HasData(DataRow dr, string columnName)
        {
            return dr[columnName] != null && dr[columnName] != DBNull.Value;
        }

       
        private void NewSearchAR()
        {

            // store in a new variable to all the seasrch parameter vauel
            string strARNo = GetUIStringValueToController(txtARNo.Text);
            string strAppNo = GetUIStringValueToController(txtAppNo.Text);
            string strChequeNo = GetUIStringValueToController(txtChequeNo.Text);
            string strPranNo = GetUIStringValueToController(txtPranNo.Text);
            string strScheme = GetUIStringValueToController(ddlSchemas.SelectedValue);
            string strInvName = GetUIStringValueToController(txtInvName.Text);
            string strAnaExistCode = GetUIStringValueToController(txtAnaExistCode.Text);
            string strArFromDate = GetUIStringValueToController(txtArFromDate.Text);
            string strArToDate = GetUIStringValueToController(txtArToDate.Text);
            string strArBefore = rbtDepositBefore.Checked ? "1" : null;
            string strArAfter = rbtDepositAfter.Checked ? "1" : null;
            string strArBeforeAfter = rbtDepositBefore.Checked ? "1" : null;


            string StrSql = "";
            string strSearch = "";// "ConvertString(UCase(Trim(txtScheme.Text))";

            if(!rbtDepositBefore.Checked && !rbtDepositAfter.Checked) { 
                pc.ShowAlert(this, "Please check the deposit before or after radio button!");
                return; 
            }

            string GlbDataFilter = string.Empty;
            string globalFilterSQL = "SELECT DATAFILTER FROM userdetails_ji WHERE login_id = '"+pc.currentLoginID()+"' AND role_id = '"+pc.currentRoleID()+"' AND DATAFILTER = '72' AND ROWNUM = 1";
            DataTable tempDt = pc.ExecuteCurrentQueryMaster(globalFilterSQL, out int rnt1, out string iet1);

            if (rnt1 > 0 && string.IsNullOrEmpty(iet1))
            {
                GlbDataFilter = tempDt.Rows[0]["DATAFILTER"].ToString();
            }
            else
            {
                GlbDataFilter = string.Empty;
            }
            if (rbtDepositAfter.Checked)
            {
                
                if (string.IsNullOrEmpty(GlbDataFilter) && GlbDataFilter=="72")
                {
                    // User does not have the filter, show an error message
                    string msg = "You do not have permission to access this data!";
                    ARListlblMessage.Text = msg;
                    pc.ShowAlert(this, msg);
                    return;
                }
                #region If checkd after then search in transactionst else temp
                if (GlbDataFilter == "72")
                {
                    StrSql = "SELECT nvl((select investor_name from investor_master im where im.inv_code = client_code),'') as CLIENT_NAME, CASE WHEN SCH_CODE = 'OP#09971' THEN 'New Pension Scheme Tier 1' WHEN SCH_CODE = 'OP#09972' THEN 'New Pension Scheme Tier 2' WHEN SCH_CODE = 'OP#09973' THEN 'New Pension Scheme Tier 1+2' ELSE 'Unknown Scheme' END AS SCH_CODE1, TO_CHAR(TR_DATE, 'DD-MM-YY')  AS TR_DATE1, (select iss_name from iss_master where iss_code = transaction_st.mut_code) as MUT_CODE1, transaction_st.* from transaction_st where tran_code is not null ";
                    //StrSql = "SELECT * from transaction_st where ";

                }
                else
                {
                    StrSql = "SELECT nvl((select investor_name from investor_master im where im.inv_code = client_code),'') as CLIENT_NAME, CASE WHEN SCH_CODE = 'OP#09971' THEN 'New Pension Scheme Tier 1' WHEN SCH_CODE = 'OP#09972' THEN 'New Pension Scheme Tier 2' WHEN SCH_CODE = 'OP#09973' THEN 'New Pension Scheme Tier 1+2' ELSE 'Unknown Scheme' END AS SCH_CODE1, TO_CHAR(TR_DATE, 'DD-MM-YY')  AS TR_DATE1, (select iss_name from iss_master where iss_code = transaction_st.mut_code) as MUT_CODE1, transaction_st.* from transaction_st where (flag<>'NEWTRAN' OR FLAG IS NULL) and tran_code is not null ";
                    //StrSql = "SELECT * from transaction_st where (flag<>'NEWTRAN' OR FLAG IS NULL) and ";

                }

                #endregion
            }
            else
            {
                StrSql = "SELECT nvl((select investor_name from investor_master im where im.inv_code = client_code),'') as CLIENT_NAME, SELECT CASE WHEN SCH_CODE = 'OP#09971' THEN 'New Pension Scheme Tier 1' WHEN SCH_CODE = 'OP#09972' THEN 'New Pension Scheme Tier 2' WHEN SCH_CODE = 'OP#09973' THEN 'New Pension Scheme Tier 1+2' ELSE 'Unknown Scheme' END AS SCH_CODE1, TO_CHAR(TR_DATE, 'DD-MM-YY')  AS TR_DATE1, (select iss_name from iss_master where iss_code = transaction_st.mut_code) as MUT_CODE1, transaction_sttemp.* transaction_sttemp where tran_code is not null ";
            }


            // this vb in .cs    "If Trim(txtScheme.Text) = "" And Trim(txtappno.Text) = "" And Trim(txtarno.Text) = "" And Trim(txtchequeno.Text) = "" And txtmarno.Text = "" And txtArFromDate.Text = "__/__/____" And msktodate.Text = "__/__/____" And Trim(txtInvName.Text) = "" And Trim(txtANACD.Text) = "" Then"

            // check if all fields are null or empty

            if (string.IsNullOrWhiteSpace(txtARNo.Text) &&
                string.IsNullOrWhiteSpace(txtAppNo.Text) &&
                string.IsNullOrWhiteSpace(txtChequeNo.Text) &&
                string.IsNullOrWhiteSpace(txtPranNo.Text) &&
                string.IsNullOrWhiteSpace(ddlSchemas.SelectedValue) &&
                string.IsNullOrWhiteSpace(txtInvName.Text) &&
                string.IsNullOrEmpty(txtAnaExistCode.Text) &&
                string.IsNullOrEmpty(txtArFromDate.Text) &&
                string.IsNullOrEmpty(txtArToDate.Text))
            {
                // Reset the search result and show an error message if no fields are provided
                string msg = "Please provide at least one field for the search!";
                ARListlblMessage.Text = msg;
                pc.ShowAlert(this, msg);
                return;
            }

            if (!string.IsNullOrEmpty(strAppNo))
            {
                StrSql += " and app_no = '" + strAppNo + "' ";
            }

            if (!string.IsNullOrEmpty(strARNo))
            {
                StrSql += " and tran_code = '" + strARNo + "' ";
            }

            if (!string.IsNullOrEmpty(strChequeNo))
            {
                StrSql += "and cheque_no LIKE '%" + strChequeNo + "%' ";

            }

            // manual ar

            if (!string.IsNullOrEmpty(strPranNo))
            {
                StrSql += " and manual_arno = '" + strPranNo + "' ";
            }

            //If (currentForm.Name == "frmNPS"){}
            StrSql += " and sch_code in('OP#09971','OP#09972','OP#09973') ";

            if(!string.IsNullOrEmpty(strArFromDate) && !string.IsNullOrEmpty(strArToDate))
            {
                StrSql += " and tr_date >= to_date('" + strArFromDate + "', 'dd-MM-yyyy') ";
                StrSql += " and tr_date <= to_date('" + strArToDate + "', 'dd-MM-yyyy') ";
            }

            else if (!string.IsNullOrEmpty(strArFromDate) && string.IsNullOrEmpty(strArToDate))
            {
                StrSql += " and tr_date >= to_date('" + strArFromDate + "', 'dd-MM-yyyy') ";
            }
            if (string.IsNullOrEmpty(strArFromDate) && !string.IsNullOrEmpty(strArToDate))
            {
                StrSql += " and tr_date <= to_date('" + strArToDate + "', 'dd-MM-yyyy') ";
            }

            if(!string.IsNullOrEmpty(strScheme))
            {
                strSearch = ddlSchemas.SelectedItem.Text;
                StrSql += " and SCH_CODE = '" + strScheme + "' ";
                //StrSql += " (sch_code in (select sch_code from scheme_info,MUT_FUND where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) like '%" + strSearch + "%') or sch_code in (select osch_code from other_product o, product_master p,iss_master i where o.prod_class_code=p.prod_code and o.iss_code=i.iss_code AND UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) like '%" + strSearch + "%')) AND ";

            }

            if (!string.IsNullOrEmpty(txtInvName.Text))
            {

                StrSql += " and (client_code in (select distinct inv_code from investor_master where upper(investor_name) like '%" + txtInvName.Text.Trim().ToUpper() + "%')) ";

            }

            string txtANACD = txtAnaExistCode.Text.Trim();

            if (!string.IsNullOrEmpty(txtANACD)) {

                //StrSql += " and (source_code in (select agent_code from agent_master where trim(exist_code) ='" + txtANACD.Trim() + "')) ";
                StrSql += " and source_code ='" + txtANACD.Trim() + "' ";

            }

            string SRmCode = pc.psm_RMs();

            if (!string.IsNullOrEmpty(SRmCode)){
                //StrSql += " Branch_code in (" + pc.LogBranches() + ") order by tr_date";

            }
            else
            {
                //StrSql += " Branch_code in (" + pc.LogBranches() + ") and RMCODE IN (" + pc.psm_RMs() + ") order by tr_date";
            }

            StrSql += " and Branch_code in (" + pc.LogBranches() + ") and rownum < 9999 order by tr_date ";

            DataTable dt = pc.ExecuteCurrentQueryMaster(StrSql, out int rowNum, out string isException);

            //generateDynaDt(dt);
            if (rowNum>0 && isException == null)
            {
                ARListlblMessage.Text = "Total " + (rowNum>9995 ? $@"{rowNum.ToString()}+": rowNum.ToString()) + " records found!";
                ARListARGridView.Visible = true;
                ARListARGridView.DataSource = dt;
                ARListARGridView.DataBind();
            }
            else
            {
                string msg = "No records found!";
                pc.ShowAlert(this, msg);
                ARListlblMessage.Text = msg;
                ARListARGridView.Visible = false;
                ARListARGridView.DataSource = null;
                ARListARGridView.DataBind();
            }
        }


      
      
     
        protected void btnpReset_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtARNo.Text = "";
            ddlSchemas.SelectedIndex = 0;
            txtChequeNo.Text = "";
            txtInvName.Text = "";
            txtAppNo.Text = "";
            txtPranNo.Text = "";
            txtAnaExistCode.Text = "";
            txtArFromDate.Text = "";
            txtArToDate.Text = "";

            ARListARGridView.DataSource = null;
            ARListARGridView.DataBind();

            ARListlblMessage.Text = "";
        }

        protected void btnpExit_Click(object sender, EventArgs e)
        {
            ParseQueryStringNPSTRFind(out string findTrFlag);
            if (findTrFlag.Contains("NPS_TR"))
            {
                Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
            }
            else
            {
                pc.RedirectToWelcomePage();
                return;
            }



            /*
            // get this session valeu Session["AR_FIND_TR"] = "NPS";
            string currentSession = Session["AR_FIND_TR"]?.ToString();

            if(currentSession == "NPS")
            {
                Session["AR_FIND_TR"] = null;
            }
            else
            {
                pc.RedirectToWelcomePage();
            }*/

        }

        protected void PaginationARListARGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ARListARGridView.PageIndex = e.NewPageIndex; 
            NewSearchAR();
        }

       

        protected void ARListARGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectTransaction")
            {
                // Retrieve the TRAN_CODE from CommandArgument
                string tranCode = e.CommandArgument.ToString();

                // Set the text of the label
                ARListlblMessage.Text = " " + tranCode;
            }
        }


        protected void gvARSearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                string tranCode = e.CommandArgument.ToString();
                ParseQueryStringNPSTRFind(out string findTrFlag);
                if (findTrFlag.Contains("NPS_TR"))
                {
                    //Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
                }
                try
                {                      
                    if (!string.IsNullOrEmpty(tranCode))
                    {
                        DataTable finArData = new DataTable();
                        bool beforeCheck = rbtDepositBefore.Checked;
                        bool afterCheck = rbtDepositAfter.Checked;
                        if (!string.IsNullOrEmpty(tranCode))
                        {
                            // preapre a redire url wiht seletec tr and with before or after check
                            // //Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
                            // check if before or after is checked
                            
                            string redirectUrl = "~/Masters/NpsTransactionPunching.aspx?tranCode=" + tranCode;

                            // add before or after check to the url
                            if (beforeCheck) {
                                redirectUrl += "&check=before";
                            }
                            else if (afterCheck) {
                                redirectUrl += "&check=after";
                            }
                            else
                            {
                                pc.ShowAlert(this, "Please check the deposit before or after radio button!");
                                return;
                            }

                            // redirect to the url
                            Response.Redirect(redirectUrl, false);

                            // sample final url
                            // Response.Redirect("~/Masters/NpsTransactionPunching.aspx?tranCode=12345&check=before", false);

                            return;





                            if (beforeCheck)
                            {
                                finArData = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, true, Session["LoginId"]?.ToString());
                            }
                            else if (afterCheck)
                            {
                                finArData = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, false, Session["LoginId"]?.ToString());
                            }
                            else
                            {
                                pc.ShowAlert(this, "Please check the deposit before or after radio button!");
                                return;
                            }

                            int rowCount = finArData.Rows.Count > 0 ? 1 : 0;

                            if (rowCount > 0)
                            {
                                DataRow row = finArData.Rows[0];
                                string dbMsg = row["message"].ToString().ToUpper();

                                if (dbMsg.Contains("TRANSACTION DATA EXIST "))
                                {
                                    Session["AR_FIND_TR_ROW"] = row;
                                    Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
                                    return;
                                }
                                 
                                else
                                {
                                    pc.ShowAlert(this, dbMsg);
                                    return;
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    pc.ShowAlert(this, ex.Message);
                    return;
                }
            }
        }
    
    
    
    }
}
