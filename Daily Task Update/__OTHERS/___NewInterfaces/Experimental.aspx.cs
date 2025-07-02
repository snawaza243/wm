using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;




namespace WM.Tree
{
    public partial class Experimental : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

  
            if (!IsPostBack)
            {
                BindGrid();
                lblModalTitle.Text = "Add New Item";
            }
        }

        private void BindGrid()
        {
            // Sample data binding
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Description");
            dt.Columns.Add("CreatedDate", typeof(DateTime));

            // Add sample rows
            for (int i = 1; i <= 15; i++)
            {
                dt.Rows.Add(i, $"Item {i}", $"Description for item {i}", DateTime.Now.AddDays(-i));
            }

            gvData.DataSource = dt;
            gvData.DataBind();
        }

       
        protected void btnSubmitBasic_Click(object sender, EventArgs e)
        {
            // Process basic form submission
            pnlStatus.Visible = true;
            pnlStatus.CssClass = "status-message success";
            lblStatus.Text = "Basic form submitted successfully!";
        }

        protected void btnResetBasic_Click(object sender, EventArgs e)
        {
            // Reset basic form controls
            txtName.Text = string.Empty;
            ddlOptions.SelectedIndex = 0;
            rbOption1.Checked = false;
            rbOption2.Checked = false;
            txtDate.Text = string.Empty;
            txtNumber.Text = string.Empty;

            pnlStatus.Visible = true;
            pnlStatus.CssClass = "status-message success";
            lblStatus.Text = "Basic form reset successfully!";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Implement search functionality
            BindGrid();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            // Prepare modal for adding new item
            txtModalName.Text = string.Empty;
            txtModalDescription.Text = string.Empty;
            ddlModalType.SelectedIndex = 0;
            chkModalActive.Checked = true;
            txtModalDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            lblModalTitle.Text = "Add New Item";

            // Script to open modal will be called from client side after postback
            hfModalState.Value = "open";
            ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openPersistentModal();", true);
        }

        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                // Handle edit command
                int id = Convert.ToInt32(e.CommandArgument);
                EditItem(id);
            }
            else if (e.CommandName == "Delete")
            {
                // Handle delete command
                int id = Convert.ToInt32(e.CommandArgument);
                DeleteItem(id);
            }
        }



        protected void gvData_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvData.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvData_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int rowIndex = e.RowIndex;

            // Example: get ID from DataKeys if set
            string id = gvData.DataKeys[rowIndex].Value.ToString();

            // Delete the row using your logic
            DeleteRow(id);

            // Rebind the data
            BindGrid();
        }

        // Optional Delete Function
        private void DeleteRow(string id)
        {
            // Your DB delete logic here
        }

        private void EditItem(int id)
        {
            // Load item data for editing
            txtModalName.Text = $"Item {id}";
            txtModalDescription.Text = $"Description for item {id}";
            ddlModalType.SelectedIndex = id % 3;
            chkModalActive.Checked = (id % 2 == 0);
            txtModalDate.Text = DateTime.Now.AddDays(-id).ToString("yyyy-MM-dd");

            lblModalTitle.Text = $"Edit Item {id}";
            ViewState["EditingId"] = id;

            // Open modal
            hfModalState.Value = "open";
            ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openPersistentModal();", true);
        }

        private void DeleteItem(int id)
        {
            // In a real application, delete from database
            BindGrid();

            pnlStatus.Visible = true;
            pnlStatus.CssClass = "status-message success";
            lblStatus.Text = $"Item {id} deleted successfully!";
        }

        
        protected void btnModalSave_Click(object sender, EventArgs e)
        {
            // Save modal data
            if (ViewState["EditingId"] != null)
            {
                // Update existing item
                int id = (int)ViewState["EditingId"];
                pnlStatus.Visible = true;
                pnlStatus.CssClass = "status-message success";
                lblStatus.Text = $"Item {id} updated successfully!";
                ViewState.Remove("EditingId");
            }
            else
            {
                // Add new item
                pnlStatus.Visible = true;
                pnlStatus.CssClass = "status-message success";
                lblStatus.Text = "New item added successfully!";
            }

            // Refresh grid
            BindGrid();

            // Close modal
            hfModalState.Value = "closed";
        }
    }
}