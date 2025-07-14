using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;  
using System.Windows.Input;
using Microsoft.Ajax.Utilities;
 

namespace WM.Tree
{
    public partial class playground : System.Web.UI.Page
    {
        PsmController pc = new PsmController();
        protected string currentLoginId;
        protected string currentRoleId;
        protected string AC_FIND_TYPE = null;

        string GlbDataFilter = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pc.currentLoginID()))
            {
                Session["loggedAngBranches"] = null;
                //pc.RedirectToWelcomePage();
            }
            else
            {
                GlbDataFilter = pc.currentRoleID();
                currentLoginId = pc.currentLoginID();
                currentRoleId = pc.currentRoleID();

                if (!IsPostBack)
                {
                    Session["Validated"] = null; // Clear session on fresh load

                    string listDBLikeWealth = $@"
SELECT username AS name, 
username AS value 
FROM all_users WHERE UPPER(username) LIKE 'WEALTHMAKER%' ORDER BY username

";
                    DataTable dbList = pc.ExecuteCurrentQueryMaster(listDBLikeWealth, out int rc1, out string ex1);

                    if (rc1 > 0 && string.IsNullOrEmpty(ex1))
                    {
                        ddlLanguageList.Items.Clear();
                        ddlLanguageList.Items.Add(new ListItem("-- Select --", ""));  
                        ddlLanguageList.Items.Add(new ListItem("Query", "sql"));  
                        ddlLanguageList.Items.Add(new ListItem("Humanize - Hindi", "hin"));  
                        ddlLanguageList.Items.Add(new ListItem("Humanize - English", "eng"));  
                        ddlLanguageList.Items.Add(new ListItem("Humanize - Hinglish", "heng"));  
                        ddlLanguageList.SelectedValue  ="sql"; // Default selection


                        foreach (DataRow row in dbList.Rows)
                        {
                            string name = row["name"].ToString();
                            string value = row["value"].ToString();

                            ddlDataBaseList.Items.Add(new ListItem(name, value));
                        }

                        string currentDb = "WEALTHMAKER";
                        if (ddlDataBaseList.Items.Count > 0)
                        {
                            ddlDataBaseList.SelectedValue = currentDb;
                        }

                        if (!string.IsNullOrEmpty(ddlDataBaseList.SelectedValue)){
                            PsmController.FillDropDown("all_tables", "table_name", "table_name", $@"WHERE owner = '{currentDb}'", ddlTablesList , null);
                        }
                    }

                }

                else
                {
                    Session["loggedAngBranches"] = null;

                    string dtStateName = "SearchedDtOne";
                    DataTable dt = ViewState[dtStateName] as DataTable;
                    if (dt != null)
                    {
                        BindGrid(dt, null, dtStateName); // Rebind the grid on IsPostBack
                    }
                }

                hdnSessionValid.Value = (Session["Validated"] != null && (bool)Session["Validated"]).ToString().ToLower();

            }
        }

        protected void Fun_FillTableByDb(string dbName)
        {
            try
            {
                if (string.IsNullOrEmpty(dbName))
                {
                    pc.ShowAlert(this, "DB is null");
                    return;
                }

            }catch(Exception e)
            {
                pc.ShowAlert(this,e.Message);
                return;
            }


            string currentDb = "WEALTHMAKER";
            if (ddlDataBaseList.Items.Count > 0)
            {
                ddlDataBaseList.SelectedValue = currentDb;
            }

            if (!string.IsNullOrEmpty(ddlDataBaseList.SelectedValue))
            {
                PsmController.FillDropDown("all_tables", "table_name", "table_name", $@"WHERE owner = '{currentDb}'", ddlTablesList, null);
            }
        }


        #region Grid section

        private void BindGrid(DataTable dt, string ex,string dtStateName)
         {

            globalGridOne.Columns.Clear();

            if (dt == null || dt.Rows.Count == 0)
            {
                txtMessage.Text = ex; 
                ViewState[dtStateName] = dtStateName; // Store the DataTable in ViewState for paging
                globalGridOne.Visible = false;
                pnlSearchInGrid.Visible = false;

                if (dtStateName != null)
                {
                    txtMessage.ForeColor = System.Drawing.Color.Red;
                    txtMessage.BorderColor = System.Drawing.Color.Red;
                    pc.ShowAlert(this, "No data found");
                }
                globalGridOne.DataSource = null;
                globalGridOne.DataBind();
                txtSearchInGrid.Visible = false;
                return;
            }

            ViewState[dtStateName] = dt;
            txtMessage.Text = "✔ Total " + dt.Rows.Count + (dt.Rows.Count == 1 ? " record" : " records") + " found!";
            txtSearchInGrid.Visible = true;

            txtMessage.ForeColor = System.Drawing.Color.Teal;
            txtMessage.BorderColor = System.Drawing.Color.Teal;
            pnlSearchInGrid.Visible = true;

            globalGridOne.Visible = true;

            /*
            // First column: Select button
            TemplateField selectField = new TemplateField
            {
                HeaderText = "Select"
            };
            selectField.ItemTemplate = new SelectLinkButtonTemplate("code"); // Pass column name to bind
            selectField.ItemStyle.Width = Unit.Pixel(100);
            selectField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            selectField.ItemStyle.BorderStyle = BorderStyle.Solid;
            selectField.ItemStyle.BorderWidth = Unit.Pixel(1);
            selectField.HeaderStyle.BorderStyle = BorderStyle.Solid;
            selectField.HeaderStyle.BorderWidth = Unit.Pixel(1);
            globalGridOne.Columns.Add(selectField);
            */

            // Add rest of the dynamic columns
            foreach (DataColumn col in dt.Columns)
            {
                BoundField bf = new BoundField
                {
                    DataField = col.ColumnName,
                    HeaderText = col.ColumnName.Replace("_", " ").FormatInvariant()
                };
                bf.ItemStyle.Width = Unit.Pixel(150);
                bf.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                bf.ItemStyle.BorderStyle = BorderStyle.Solid;
                bf.ItemStyle.BorderWidth = Unit.Pixel(1);
                bf.HeaderStyle.BorderStyle = BorderStyle.Solid;
                bf.HeaderStyle.BorderWidth = Unit.Pixel(1);

                globalGridOne.Columns.Add(bf);
            }


            ViewState[dtStateName] = dt; // Store the DataTable in ViewState for paging
            globalGridOne.Visible = true;
            globalGridOne.DataSource = dt;
            globalGridOne.DataBind();

        }

        protected void globalGridOne_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                globalGridOne.PageIndex = e.NewPageIndex;
                string dtStateName = "SearchedDtOne";
                DataTable dt = ViewState[dtStateName] as DataTable;

                if (dt != null)
                {
                    BindGrid(dt, null, dtStateName); // Rebind the grid on page index change
                }
            }
            catch ( Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;

            }
            
        }


        protected void globalGridOne_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = globalGridOne.Rows[rowIndex];

                string selectedValue = row.Cells[0].Text; // Example: getting first column's value
                                                          // Do something with selectedValue
            }

            if (e.CommandName == "DeleteRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                // Perform delete logic here
            }
        }

        protected void globalGridOne_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["ondblclick"] = "this.style.backgroundColor='#ffffcc';";
                e.Row.Style.Add("cursor", "pointer");
            }
        }



        protected void globalGridOne_Sorting(object sender, GridViewSortEventArgs e)
        {
            string dtStateName = "SearchedDtOne";
            DataTable dt = ViewState[dtStateName] as DataTable;
            if (dt != null)
            {
                string sortDirection = GetSortDirection(e.SortExpression);
                dt.DefaultView.Sort = e.SortExpression + " " + sortDirection;

                globalGridOne.DataSource = dt;
                globalGridOne.DataBind();
            }
        }
        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            string lastSortColumn = ViewState["SortExpression"] as string;
            string lastDirection = ViewState["SortDirection"] as string;

            if (lastSortColumn == column && lastDirection == "ASC")
            {
                sortDirection = "DESC";
            }

            ViewState["SortExpression"] = column;
            ViewState["SortDirection"] = sortDirection;

            return sortDirection;
        }

        protected void txtSearchInGrid_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearchInGrid.Text.Trim().ToLower();
            string dtStateName = "SearchedDtOne";
            DataTable dt = ViewState[dtStateName] as DataTable;

            if (dt != null)
            {
                DataView dv = dt.DefaultView;
                string filter = "";

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.DataType == typeof(string))
                    {
                        if (filter.Length > 0)
                            filter += " OR ";

                        filter += $"{col.ColumnName} LIKE '%{keyword.Replace("'", "''")}%'"; // Safe filter
                    }
                }

                dv.RowFilter = filter;
                globalGridOne.DataSource = dv;
                globalGridOne.DataBind();
            }
        }


        #endregion






        #region Button and OnChange Function
        // Fetch confidential data only if session is validated
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["Validated"] != null && (bool)Session["Validated"])
                {
                    FetchConfidentialData();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertInvalid", "alert('Access denied. Please enter the correct passcode.');", true);
                }
            }
            catch (Exception ex)
            {
                // pc.ShowAlert(this, ex.Message); // Optional if you have a custom alert system
                ScriptManager.RegisterStartupScript(this, GetType(), "alertError", $"alert('{ex.Message}');", true);
            }
        }

        // Triggered when hidden validation button is posted back
        protected void btnValidatePasscode_Click(object sender, EventArgs e)
        {
            string enteredPasscode = hdnPasscode.Value;

            if (IsValidPasscode(enteredPasscode))
            {
                Session["Validated"] = true;

                // Re-trigger the main button click after successful validation
                ScriptManager.RegisterStartupScript(this, GetType(), "retryClick", "__doPostBack('" + btnSearch.UniqueID + "', '');", true);
            }
            else
            {
                Session["Validated"] = null;

                // Show invalid message
                ScriptManager.RegisterStartupScript(this, GetType(), "alertInvalid", "alert('Invalid passcode. Please try again.');", true);
            }

            // Clear hidden field
            hdnPasscode.Value = string.Empty;

            // Refresh session check value for client-side
            hdnSessionValid.Value = "false";
        }

        // Replace with real validation logic (ideally check a hash, DB, or config)
        private bool IsValidPasscode(string input)
        {
            return input == "sam123"; // Replace this with secure logic
        }



        protected void FetchConfidentialData()
        {
            try
            {
                string query = txtQuery.Text.Trim();
                if (query.EndsWith(";"))
                {
                    query = query.Substring(0, query.Length - 1);
                }

                if (!query.ToUpper().Contains("WHERE"))
                {
                    query += " WHERE 1=1 and rownum<500";
                }

                if (!string.IsNullOrEmpty(query))
                {
                    DataTable dt = pc.ExecuteCurrentQueryMaster(query, out int rc, out string ex);
                    string dtStateName = "SearchedDtOne";
                    if (rc > 0 && string.IsNullOrEmpty(ex))
                    {
                        BindGrid(dt, null, dtStateName); // Rebind the grid with the new data
                    }
                    else
                    {
                        BindGrid(null, ex, dtStateName); // Rebind the grid with the new data
                    }
                }
                else
                {
                    pc.ShowAlert(this, "Please enter a query."); return;
                }

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }

        }
 

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtQuery.Text = string.Empty;
                //ddlConnectionList.SelectedIndex = 0;
                //ddlDataBaseList.SelectedIndex = 0;
                ddlTablesList.SelectedIndex = 0;
                //ddlLanguageList.SelectedIndex = 0;
                txtSearchInGrid.Text = string.Empty;
                Session["Validated"] = null;
                Session["loggedAngBranches"] = null;


                ViewState["SearchedDtOne"] = null;
                txtMessage.ForeColor = System.Drawing.Color.Black;
                txtMessage.BorderColor = System.Drawing.Color.Black;
                txtMessage.Text = string.Empty;
                globalGridOne.Visible = false;
                pnlSearchInGrid.Visible = false;
                globalGridOne.DataSource = null;
                globalGridOne.DataBind();
                txtSearchInGrid.Visible = false;
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            } 

        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearchInGrid.Text = string.Empty;
                Session["Validated"] = null;
                Session["loggedAngBranches"] = null;
                ViewState["SearchedDtOne"] = null;
                pc.RedirectToWelcomePage();
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }
        }

        protected void ddlConnectionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pc.ShowAlert(this, "Clieck!");
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }
        }

        protected void ddlDataBaseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //pc.ShowAlert(this, ddlDataBaseList.ID);
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }
        }

        protected void ddlTablesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //pc.ShowAlert(this, ddlTablesList.ID);
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }
        }

        protected void ddlLanguageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //pc.ShowAlert(this, ddlLanguageList.ID);
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message); return;
            }
        }


        #endregion


    }
}
