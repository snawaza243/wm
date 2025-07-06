using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;

using ExcelDataReader; // External library for reading Excel files
using System.Linq;
using OfficeOpenXml;
using System.Data.SqlClient;


namespace WM.Masters
{
    public partial class map_policy_number : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //fillPolicyTypeList();
            }

            //if (IsPostBack && Request.Params["__EVENTTARGET"] == btnCountSheets.UniqueID)
            //{
            //    btnCountSheets_Click(sender, e);
            //}
        }

        protected void btnCountSheets_Click(object sender, EventArgs e)
        {

        }

        //#region fillPolicyTypeList
        //private void fillPolicyTypeList()
        //{
        //    DataTable dt = new MapPolicyNumberController().GetCompanyList();
        //    AddDefaultItem(dt, "COMPANY_NAME", "COMPANY_CD", "Select");
        //    ddlCompanyName.DataSource = dt;
        //    ddlCompanyName.DataTextField = "COMPANY_NAME";
        //    ddlCompanyName.DataValueField = "COMPANY_CD";
        //    ddlCompanyName.DataBind();
        //}
        //#endregion



        //#region gvPolicySearch_RowCommand
        //protected void gvPolicySearch_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "SelectRow")
        //    {
        //        string policyNumber = e.CommandArgument.ToString();
        //        Response.Redirect("policy_details.aspx?policyNumber=" + policyNumber);
        //    }
        //}
        //#endregion


        // Button click event to trigger the Excel import

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                // Define the upload path and file path
                string uploadPath = Server.MapPath("~/Uploads/");
                string filePath = uploadPath + FileUpload1.FileName;

                // Ensure the directory exists; if not, create it
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Check if a file is uploaded
                if (FileUpload1.HasFile)
                {
                    // Save the uploaded file to the specified path
                    FileUpload1.SaveAs(filePath);

                    // Get the selected sheet name from dropdown (if applicable)
                    string selectedSheet = ddlSheetList.SelectedValue;

                    // Create an instance of the controller
                    var controller = new WM.Controllers.MapPolicyNumberController();

                    // Call method to import the Excel data into the Oracle database
                    string resultMessage = controller.ImportExcelToDatabase(filePath, selectedSheet);

                    // Display the result message
                    lblSheetMsg.Text = resultMessage;

                    // Check for successful import and bind data to the grid
                    if (resultMessage.Contains("successfully"))
                    {
                        exportData();
                        BindGrid();
                    }
                    else
                    {
                        lblExportMsg.Text = string.Empty;
                        gridPolicyData.DataSource = null;
                        gridPolicyData.DataBind();
                    }
                }
                else
                {
                    lblSheetMsg.Text = "Please select a file to upload.";
                    lblExportMsg.Text = string.Empty;
                    gridPolicyData.DataSource = null;
                    gridPolicyData.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Log the exception and show error message
                lblSheetMsg.Text = "An error occurred: " + ex.Message;
            }
        }


        protected void btnImport_Click_0(object sender, EventArgs e)
        {
            string filePath = Server.MapPath("~/Uploads/") + FileUpload1.FileName;

            if (FileUpload1.HasFile)
            {
                FileUpload1.SaveAs(filePath); // Save the file to the server

                // Get the selected sheet name (assuming you have a dropdown)
                string selectedSheet = ddlSheetList.SelectedValue;

                // Create an instance of the controller (if it's not static)
                var controller = new WM.Controllers.MapPolicyNumberController();

                // Import the Excel data into Oracle database by calling the method
                string resultMessage = controller.ImportExcelToDatabase(filePath, selectedSheet);

                lblSheetMsg.Text = resultMessage;

                if (resultMessage.Contains("successfully"))
                {
                    exportData();
                    BindGrid();
                }
                else
                {
                    lblExportMsg.Text = string.Empty;
                    gridPolicyData.DataSource = null;
                    gridPolicyData.DataBind();

                }
            }
            else
            {
                lblSheetMsg.Text = "Please select a file to upload.";
                lblExportMsg.Text = string.Empty;
                gridPolicyData.DataSource = null;
                gridPolicyData.DataBind();

            }
        }



        protected void btnExport_Click(object sender, EventArgs e)
        {
            exportData();
        }

        protected void exportData()
        {
            string result = new WM.Controllers.MapPolicyNumberController().GeneratePolicyReport();
            lblExportMsg.Text = result; // Use Literal to render HTML content
        }

        protected void btnShowData_Click(object sender, EventArgs e)
        {
            BindGrid();

        }



        private void BindGrid()
        {
            DataTable policyData = new WM.Controllers.MapPolicyNumberController().GetPolicyMapReport();
            gridPolicyData.DataSource = policyData;
            gridPolicyData.DataBind();
        }


        protected void gvPolicyData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRow")
            {
                // Get the CommandArgument value  
                string policyNo = e.CommandArgument.ToString();
                // Redirect to employee_master page with the policy number value 
                Response.Redirect("employee_master.aspx?stid=" + policyNo);
            }
        }

        #region ExitBtn
        protected void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/welcome.aspx");
        }
        #endregion

        protected void btnReset_Click(object sender, EventArgs e)
        {
            // Clear the FileUpload control
            FileUpload1.Attributes.Clear();

            // Reset the DropDownList selection
            ddlSheetList.SelectedIndex = 0;

            // Clear the label text
            lblSheetMsg.Text = string.Empty;
            lblExportMsg.Text = string.Empty;

            // Optionally, you can also clear the GridView
            gridPolicyData.DataSource = null;
            gridPolicyData.DataBind();
        }



    }

}
