using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO; 
using System.Text;
using WM.Controllers;
using NPOI.SS.Formula.PTG;


namespace WM.Masters
{
    public partial class DueAndPaidDataImporting : Page
    {
        PsmController pc = new PsmController();

        private DateTime ServerDateTime = DateTime.Now;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (pc.currentLoginID() == null)
            {
                pc.RedirectToWelcomePage();
            }
            else
            {
                if (!IsPostBack)
                {
                    excelSheetSelect.Enabled = false;
                    FillYearMonths();
                     
                }
            }
        }

     

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (fileInput.HasFile)
            {
                string fileName = "DueAndPaid_" + fileInput.FileName;                     // Use the original file name
                string uploadPath = Server.MapPath("~/Uploads/");
                string filePath = Path.Combine(uploadPath, fileName);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                try
                {
                    fileInput.SaveAs(filePath);
                    Session["CurrentDAPExcelFile"] = filePath;
                    PsmController.GetExcelSheetList(filePath,excelSheetSelect).ToArray();
                    excelSheetSelect.Enabled = (excelSheetSelect.Items.Count > 0) ? true : false;

                    #region Craft the field name to show in the page 
                    string fileNameDisplay = fileInput.FileName;
                    string displayName = fileNameDisplay;
                    if (fileNameDisplay.Length > 20)
                    {
                        string start = fileNameDisplay.Substring(0, 9);
                        string end = fileNameDisplay.Substring(fileNameDisplay.Length - 10, 10);
                        displayName = start + "..." + end;
                    }
                    lblFIleName.Text = "Uploaded File: " + displayName;
                }
                catch (Exception ex)
                {
                    pc.ShowAlert(this, ex.Message);
                }
            }
        }

        protected void ExcelSheetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filePath = Session["CurrentDAPExcelFile"]?.ToString();
            string selectedSheet = excelSheetSelect.SelectedValue;

            // lblsheetHeaderListBody

            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(selectedSheet))
            {
                try
                {
                    DataTable isFetched = null;

                    PsmController.GetExcelSheetData(filePath, selectedSheet, out isFetched, null, lblMessage);
                    lblsheetHeaderListHead.Text = selectedSheet + " Excel's fields :";

                    if (isFetched.Rows.Count == 0)
                    {
                        pc.ShowAlert(this, "No data found in the selected sheet.");
                        lblMessage.Text = "No data found in the selected sheet.";
                        Session["SheetData"] = null;

                        GridView1.DataSource = null;
                        GridView1.DataBind();
                        return;
                    }
                    else
                    {

                        Session["SheetData"] = isFetched;
                        txtSheetHeaderCount.Text = isFetched.Rows.Count + " rows & " + isFetched.Columns.Count + " columns";
                        DataTable currentSheetData = (DataTable)Session["SheetData"];

                        GridView1.DataSource = currentSheetData;
                        GridView1.DataBind();




                        #region Do the mached fields validation
                        var getHeaders = PsmController.GetDataTableHeaders(currentSheetData, "out");
                        lblsheetHeaderListBody.Text = string.Join(", <br />", getHeaders);
                        #endregion

                        string mappingType = ddlImportDataType.SelectedValue;
                        if (!string.IsNullOrEmpty(mappingType))
                        {

                            Fun_FetchMapped(filePath, selectedSheet, "DUE_AND_PAID", mappingType, ddlFillPrevious);
                        }

                        monthSelect.Focus();
                    }
                }
                catch (Exception ex)
                {
                    pc.ShowAlert(this, ex.Message);
                }
            }
            else
            {
                txtSheetHeaderCount.Text = "";
                lblsheetHeaderListHead.Text = "";
                lblsheetHeaderListBody.Text = "";
                Session["SheetData"] = null;
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
        }


        protected void ddlDataTypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

            string ddlValue = ddlImportDataType.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(ddlValue))
            {
                #region CheckBox Validation
                if (ddlValue == "DUE")
                {
                    chkOptDue.Checked = true;
                    chkOptPaid.Checked = false;
                    chkOptLap.Checked = false;
                    chkOptRein.Checked = false;

                    chkOptDue.Visible = true;
                    chkOptPaid.Visible = true;
                    //chkOptLap.Enabled = false;
                    //chkOptRein.Enabled = false;

                    chkOptLap.Visible = false;
                    chkOptRein.Visible = false;

                }
                if (ddlValue == "LAPSED")
                {

                    chkOptLap.Checked = true;

                    chkOptDue.Checked = false;
                    chkOptPaid.Checked = false;
                    chkOptRein.Checked = false;


                    chkOptDue.Visible = false;
                    chkOptPaid.Visible = false;
                    chkOptLap.Visible = true;
                    chkOptRein.Visible = true;
                }
                if (ddlValue == "PAID")
                {
                    chkOptPaid.Checked = true;
                    chkOptDue.Checked = false;
                    chkOptLap.Checked = false;
                    chkOptRein.Checked = false;


                    chkOptDue.Visible = true;
                    chkOptPaid.Visible = true;
                    chkOptLap.Visible = false;
                    chkOptRein.Visible = false;
                }
                if (ddlValue == "REINS")
                {
                    chkOptRein.Checked = true;

                    chkOptLap.Checked = false;

                    chkOptDue.Checked = false;
                    chkOptPaid.Checked = false;


                    chkOptDue.Visible = false;
                    chkOptPaid.Visible = false;
                    chkOptLap.Visible = true;
                    chkOptRein.Visible = true;
                }
                #endregion

                if (!string.IsNullOrEmpty(ddlValue))
                {
                    string filePath = Session["CurrentDAPExcelFile"]?.ToString();

                    Fun_FetchMapped(filePath, excelSheetSelect.SelectedValue, "DUE_AND_PAID", ddlValue, ddlFillPrevious);
                }

            }
        }

        protected void Map_Click(object sender, EventArgs e)
        {
            try
            {
                if (pc.currentLoginID() != null)
                {
                    string redirectUrl = "../Tree/global_mapping_interface?mapping_type=due_and_paid";
                    string script = $"window.open('{redirectUrl}', '_blank', 'width=1000,height=700,resizable=yes');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenPopup", script, true);
                    return;
                }
                else
                {
                    pc.RedirectToWelcomePage();
                    return;
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }


        protected void Map_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (pc.currentLoginID() != null)
                {
                    string redirectUrl = "../Tree/global_mapping_interface?mapping_type=dua_and_paid";
                    Response.Redirect(redirectUrl);
                    return;
                }
                else
                {
                    pc.RedirectToWelcomePage();
                    return;
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
            finally
            {
                //ScriptManager.RegisterStartupScript(this, GetType(), "ExportException", $"hideServerLoader(); pc.ShowAlert(this, 'Data Exported Successfully!');", true);

            }
        }


        protected void MappingPlanButton_Click(object sender, EventArgs e)
        {
            // Code to handle mapping plan functionality.
            // Add your logic for mapping plan here.
        }


        protected void ImportButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                {
                    return;
                }
                else
                {
                    ImportDatainDB();
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
        }


        protected void ExportImportedPolicyButton_Click(object sender, EventArgs e)
        {

            if (GridView1 == null || GridView1.Rows.Count == 0)
            {
                pc.ShowAlert(this, "No data available to export.");
                return;
            }

            try
            {
                GridView1.AllowPaging = false;
                string fileName = "ImportedPolicy.xls";
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={fileName}");
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";

                using (System.IO.StringWriter sw = new System.IO.StringWriter())
                {
                    using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
                    {
                        // Add a style to format numbers properly in Excel
                        GridView1.HeaderRow.Style.Add("background-color", "#FFFFFF");
                        foreach (TableCell cell in GridView1.HeaderRow.Cells)
                        {
                            cell.Style.Add("background-color", "#D5D5D5");
                        }

                        foreach (GridViewRow row in GridView1.Rows)
                        {
                            row.BackColor = System.Drawing.Color.White;
                            foreach (TableCell cell in row.Cells)
                            {
                                cell.Attributes.Add("class", "text");
                            }
                        }

                        GridView1.RenderControl(hw);
                        // Write the rendered content to the response stream
                        Response.Output.Write(sw.ToString());
                        Response.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "An error occurred while exporting the data. Please try again.");
            }
            finally
            {
                try
                {
                    Response.End(); // Ensure the response is ended properly
                }
                catch (Exception endEx)
                {
                    pc.ShowAlert(this, endEx.Message);
                }
            }
        }


        protected void btnReloadPage_Click(object sender, EventArgs e)
        {
            pc.ReloadCurrentPage(this);
            return;
        }

        protected void ExitButton_Click(object sender, EventArgs e)
        {
            pc.RedirectToWelcomePage();
        }




        #region Helping funciotns

        protected string GetCheckedRadioButtonText()
        {
            // Iterate through the controls in the container (e.g., Page or a specific Panel)
            foreach (Control control in Page.Controls)
            {
                string text = GetCheckedRadioButtonTextRecursive(control);
                if (!string.IsNullOrEmpty(text))
                {
                    return text; // Return the text of the checked RadioButton
                }
            }
            return string.Empty; // No RadioButton is checked
        }

        private string GetCheckedRadioButtonTextRecursive(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is RadioButton radioButton && radioButton.Checked)
                {
                    return radioButton.Text; // Return the text of the checked RadioButton
                }

                // Recursively check child controls
                if (control.HasControls())
                {
                    string text = GetCheckedRadioButtonTextRecursive(control);
                    if (!string.IsNullOrEmpty(text))
                    {
                        return text;
                    }
                }
            }
            return string.Empty;
        }

        private void FillYearMonths()
        {
            int currentYear = DateTime.Now.Year;
            yearSelect.DataSource = new WM.Controllers.DueAndPaidDataImportingController().GetYearsList(25, 2040);
            yearSelect.DataBind();

            if (yearSelect.Items.Count > 1)
            {
                ListItem currentYearItem = yearSelect.Items.FindByValue(currentYear.ToString());
                if (currentYearItem != null)
                {
                    yearSelect.ClearSelection();
                    currentYearItem.Selected = true;
                }
            }

        }

        private bool ValidateInput()
        {
            if (Session["CurrentDAPExcelFile"] == null)
            {
                pc.ShowAlert(this, "First upload a file!");
                fileInput.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(excelSheetSelect.SelectedValue))
            {
                pc.ShowAlert(this, "Choose an Excel sheet!");
                excelSheetSelect.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(monthSelect.SelectedValue))
            {
                pc.ShowAlert(this, "Choose a month value!");
                monthSelect.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(yearSelect.SelectedValue))
            {
                pc.ShowAlert(this, "Choose a year value!");
                yearSelect.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(ddlImportDataType.SelectedValue))
            {
                pc.ShowAlert(this, "Choose an import data type!");
                ddlImportDataType.Focus();
                return false;
            }

            bool isMpped = (ddlFillPrevious.Items.Count>0 && 
                            (!string.IsNullOrEmpty(excelSheetSelect.SelectedValue)))
                            ? true: false;
            if (!isMpped)
            {
                pc.ShowAlert(this, "Mapping is required!");
                btnMapFields.Focus();
                return false;
            }

            return true;
        }



        #endregion





        #region New imp





        public void ProcessDropDownListValues(DropDownList ddl, out string joinedValues, out List<string> excelFields, out List<string> dbFields)
        {
            // Initialize variables
            joinedValues = "";    // To store all values joined with commas
            excelFields = new List<string>();   // List to store Excel field names
            dbFields = new List<string>();      // List to store DB field names

            // Loop through each item in the DropDownList
            foreach (ListItem item in ddl.Items)
            {
                // Get the value of the item (assuming format: "excel_field_name-->db_field_name")
                string value = item.Value;

                // Ensure the value contains "-->" before proceeding
                if (!string.IsNullOrWhiteSpace(value) && value.Contains("-->"))
                {
                    // Split the value into Excel field and DB field
                    string[] parts = value.Split(new string[] { "-->" }, StringSplitOptions.None);

                    // If the value has the correct format (two parts), process it
                    if (parts.Length == 2)
                    {
                        // 1. Join values (for the first variable)
                        joinedValues += value + ",";

                        // 2. Add Excel fields to the list (the part before -->)
                        excelFields.Add(parts[0].Trim().Replace('[', ' ').Replace(']', ' '));

                        // 3. Add DB fields to the list (the part after -->)
                        dbFields.Add(parts[1].Trim());
                    }
                }
            }

            // Remove the trailing comma from the joined values string
            if (joinedValues.EndsWith(","))
            {
                joinedValues = joinedValues.Substring(0, joinedValues.Length - 1);
            }
        }


     



        public void ImportDatainDB()
        {
            string monthValue = monthSelect.SelectedValue ?? string.Empty;
            string yearValue = yearSelect.SelectedValue ?? string.Empty;
            string ddlImportDataTypeValue = ddlImportDataType.SelectedValue ?? string.Empty;
            string logginId = pc.currentLoginID();
            string filePath = Session["CurrentDAPExcelFile"]?.ToString();
            if(string.IsNullOrEmpty(filePath))
            {
                pc.ShowAlert(this, "First upload a file!");
                fileInput.Focus();
                return;
            }
            string selectedSheet = excelSheetSelect.SelectedValue;

            DataTable currecntSheetData = (DataTable)Session["SheetData"] as DataTable;
            if (currecntSheetData == null)
            {
                pc.ShowAlert(this, "No data found in the selected sheet.");
                return;
            }
            
            
            string selectedText = GetCheckedRadioButtonText();
            string chkDataTypeValue = selectedText.Substring(0, 1).ToUpper();

            string fileName = lblFIleName.Text;

            // CHECK MAPPED DDL HAVE DATA OR NOT
            if (ddlFillPrevious.Items.Count == 0)
            {
                pc.ShowAlert(this, "Mapping is required");
                return;
            }


            string isMaskExist = string.Empty;
            List<string> excelFieldFromDbMapped = new List<string>();
            List<string> dbFieldFromDbMapped = new List<string>();
            pc.GetMappedFields("DUE_AND_PAID", ddlImportDataType.SelectedValue, out isMaskExist, out excelFieldFromDbMapped, out dbFieldFromDbMapped);





            // Step 1: Create a DropDownList for DB fields
            DropDownList ddlDbField = pc.CreateDropDownFromList(dbFieldFromDbMapped);
       

            // Step 2: Create a List for DB ddlDbField
            List<string> allDBFieldList = new List<string>();
            foreach (ListItem item in ddlDbField.Items)
            {
                allDBFieldList.Add(item.Text); // Collect all dropdown field names
            }

            // Step 3: Create a DropDownList forom masked string split with ','
            DropDownList ddlMappedFieldList = pc.CreateDropDownFromMask(isMaskExist);
            
            // Step 4: Create a List for mapped fields
            List<string> dropdownValues = new List<string>();
            foreach (ListItem item in ddlMappedFieldList.Items)
            {
                dropdownValues.Add(item.Text); // Collect mapped fields (Excel Field#DB Field)
            }
            
            // Step 5: Destructure dropdown values to get Excel fields and database fields
            List<string> excelFields = pc.GetSplitedMaskList(dropdownValues,0); // Contians excel field names with bracet like  [excel_field]
            List<string> databaseFields = pc.GetSplitedMaskList(dropdownValues, 1); // Contains database field names without bracet like  db_field



            // Step 6: Create a new DataTable current sheet data
            DataTable excelData = currecntSheetData;  


            // Step 7: Create a new DataTable with fields matching ddlDbField
            DataTable mappedDataTable = new DataTable();
            foreach (string dbField in allDBFieldList)
            {
                mappedDataTable.Columns.Add(dbField, typeof(string)); // Add columns for all database fields
            }


            // Step 8: Map data from Excel to the new DataTable based on the mapping
            foreach (DataRow excelRow in excelData.Rows)
            {
                DataRow newRow = mappedDataTable.NewRow();

                for (int i = 0; i < databaseFields.Count; i++)
                {
                    string dbField = databaseFields[i]; // Database field name
                    string excelField = pc.ReplaceThis(excelFields[i], "[,],-"); // Corresponding Excel field name

                    if (allDBFieldList.Contains(dbField) && excelData.Columns.Contains(excelField))
                    {
                        newRow[dbField] = excelRow[excelField]; // Map data from Excel to the new DataTable
                    }
                }
                mappedDataTable.Rows.Add(newRow); // Add the populated row to the DataTable
            }

            
            // Step 9: Create MyImportDataType and MyImport variables

            string MyImportDataType = "";
            string MyImport = "";

            if (!string.IsNullOrEmpty(ddlImportDataTypeValue))
            {
                if (ddlImportDataTypeValue == "DUE")
                {
                    MyImport = "D";
                    MyImportDataType = "DUEDATA";
                }
                if (ddlImportDataTypeValue == "LAPSED")
                {
                    MyImport = "L";
                    MyImportDataType = "LAPSEDDATA";
                }
                if (ddlImportDataTypeValue == "PAID")
                {
                    MyImport = "D";
                    MyImportDataType = "DUEDATA";
                }
                if (ddlImportDataTypeValue == "REINS")
                {
                    MyImport = "L";
                    MyImportDataType = "LAPSEDDATA";
                }
            }

           
            #region New import

            /* get previosue saved masked value form text field for current selected imporitng type 
            string currentImpDataName = ddlImportDataType.SelectedValue.ToString();
            string fieldSavedPathX = Server.MapPath("~/SampleFile/DuePaidMappedField" + currentImpDataName + ".txt");
            string fieldSavedPath = "";
            string isExceptionValue = "";
            if (System.IO.File.Exists(fieldSavedPathX))
            {
                fieldSavedPath = System.IO.File.ReadAllText(fieldSavedPathX);
            }
            */


            string currentImpDataName = ddlImportDataType.SelectedValue.ToString();
            string fieldSavedPathX = isMaskExist;
            string fieldSavedPath = "";
            string isExceptionValue = "";

            if (!string.IsNullOrEmpty(fieldSavedPathX))
            {
                fieldSavedPath = fieldSavedPathX;

            }



            //string[] delComma = fieldSavedPath.Split(',');
            DropDownList DDLx = new DropDownList(); // Create a new DropDownList

            // Assuming delComma is an array of strings
            string[] delComma = fieldSavedPath.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Populate the DropDownList
            foreach (string item in delComma)
            {
                DDLx.Items.Add(new ListItem(item));
            }
            string selectedFileField = "";

            string dataBaseField = "";
            string Excel_policy_no = "";
            string Excel_Status = ""; //-
            string Excel_Comp = "";
            string Excel_Payment_Mode = "";
            string Excel_Prem_Freq = "";
            string Excel_Due_Date = "";
            string doc = "";
            string Excel_Prem_Amt = "";  //-
            string excel_mobile = "";  //-
            string excel_pol_term = "";  //-
            string excel_sa = "";  //-


            int upb = delComma.GetUpperBound(0);
            for (int Count_Loop = 0; Count_Loop < DDLx.Items.Count; Count_Loop++)
            {
                string[] delHash = delComma[Count_Loop].Split(new string[] { "-->" }, StringSplitOptions.None);


                selectedFileField = selectedFileField + delHash[0] + ",";
                dataBaseField = dataBaseField + delHash[1] + ",";
                if (delHash[1].ToUpper().Trim() == "POLICY_NO")
                {
                    //Excel_policy_no = delHash[1].Replace("[", "").Replace("]", "").Replace("-", "");
                    Excel_policy_no = pc.ReplaceThis(delHash[0], "[,],-");


                }
                if (delHash[1].ToUpper().Trim() == "COMPANY_CD")
                {
                    Excel_Comp = pc.ReplaceThis(delHash[0], "[,],-");

                }
                if (delHash[1].ToUpper().Trim() == "PAY_MODE")
                {
                    //Excel_Payment_Mode = delHash[0].Replace("[", "").Replace("]", "").Replace("-", "");
                    Excel_Payment_Mode = pc.ReplaceThis(delHash[0], "[,],-");

                }
                if (delHash[1].ToUpper().Trim() == "PREM_FREQ")
                {
                    Excel_Prem_Freq = pc.ReplaceThis(delHash[0], "[,],-");
                }
                if (delHash[1].ToUpper().Trim() == "DUE_DATE")
                {
                    Excel_Due_Date = pc.ReplaceThis(delHash[0], "[,],-");
                }
                if (chkOptPaid.Checked == true)
                {
                    if (delHash[1].ToUpper().Trim() == "STATUS_CD")
                    {
                        Excel_Status = pc.ReplaceThis(delHash[0], "[,],-");
                    }
                }
                if (delHash[1].ToUpper().Trim() == "DOC")
                {
                    doc = pc.ReplaceThis(delHash[0], "[,],-");
                }
                if (delHash[1].ToUpper().Trim() == "PREM_AMT")
                {
                    Excel_Prem_Amt = pc.ReplaceThis(delHash[0], "[,],-");
                }
                if (delHash[1].ToUpper().Trim() == "CL_MOBILE")
                {
                    excel_mobile = pc.ReplaceThis(delHash[0], "[,],-");
                }
                if (delHash[1].ToUpper().Trim() == "PLY_TERM")
                {
                    excel_pol_term = pc.ReplaceThis(delHash[0], "[,],-");
                }
                if (delHash[1].ToUpper().Trim() == "SA")
                {
                    excel_sa = pc.ReplaceThis(delHash[0], "[,],-");
                }
            }




            // selectedFileField have  'EXCEL FIELDS STRING' list with braces like [Payment Type],[Policy No],[Address 1],...
            // dataBaseField have  'DB FIELDS STRING' list without braces like PAY_MODE,POLICY_NO,CL_ADD1,...
            if (selectedFileField.Length > 0)
                selectedFileField = selectedFileField.Substring(0, selectedFileField.Length - 1);
            if (dataBaseField.Length > 0)
                dataBaseField = dataBaseField.Substring(0, dataBaseField.Length - 1);

            selectedFileField.TrimEnd(',');
            dataBaseField.TrimEnd(',');

            List<string> selectedFileField_List = pc.ReplaceThis(selectedFileField, "[,],-").Split(',').ToList(); // replacing braces with empty string and spliting with comma and assigning in list
            List<string> databaseFields_List = dataBaseField.Split(',').ToList();

            int index = databaseFields_List.IndexOf("PREM_FREQ");
            string correspondingValue = "";
            if (index > 0 && index < selectedFileField_List.Count)
            {
                correspondingValue = selectedFileField_List[index];
            }

          
            string MyPremFreq = "";

            // CHANGIN DT FOR REBINDING  ON THE PREM_FREQ CHANGES  excelData-->mappedDataTable
            if (!string.IsNullOrEmpty(correspondingValue) && excelData.Columns.Contains(correspondingValue))
            {
                foreach (DataRow row in excelData.Rows)
                {
                    string premFreqVal = row[correspondingValue].ToString().Trim().ToUpper();
                    if (premFreqVal == "1" || premFreqVal == "01" || premFreqVal == "ANNUALLY" ||
                        premFreqVal == "ANNUAL" || premFreqVal == "YEARLY")
                    {
                        MyPremFreq = "1";
                    }
                    else if (premFreqVal == "12" || premFreqVal == "MONTHLY")
                    {
                        MyPremFreq = "12";
                    }
                    else if (premFreqVal == "0")
                    {
                        MyPremFreq = "0";
                    }
                    else if (premFreqVal == "QUARTERLY" || premFreqVal == "4")
                    {
                        MyPremFreq = "4";
                    }
                    else if (premFreqVal == "2" || premFreqVal == "SEMI ANNUALLY" || premFreqVal == "SEMI ANNUAL" ||
                             premFreqVal == "SEMI-ANNUALLY" || premFreqVal == "SEMI-ANNUAL" || premFreqVal == "HALF YEARLY")
                    {
                        MyPremFreq = "2";
                    }

                    row[correspondingValue] = MyPremFreq;
                }
            }
            else
            {
            }


            // IMPORTING LOOP OB EXCEL DATA
            int rowNum = 0;
             
            string isExceptino = "";
            DataTable dbTable = new DataTable();
            int impCount = 0;
            int Rec_Count = 0;
            int rec1 = 0;
            int Rec_Count_exl = 0;
            if (chkOptDue.Checked)
            {
                int excelData_rc = excelData.Rows.Count;
                foreach (DataRow row in excelData.Rows)
                {
                    if (!string.IsNullOrEmpty(Excel_Comp))
                    {
                        string SqlStr = " select * from bajaj_due_Data WHERE upper(trim(POLICY_no))=upper(trim('" + row[Excel_policy_no] + "')) and upper(trim(company_cd))= '" + row[Excel_Comp] + "' and mon_no = " + Convert.ToInt32(monthValue) + " and year_no=" + yearValue + " and importdatatype='" + MyImportDataType + "' ";
                        dbTable = pc.ExecuteCurrentQueryMaster(SqlStr, out rowNum, out isExceptino);

                        //If Rs_Chk_Excel.EOF = True Then
                        if (rowNum <= 0 && isExceptino == null)
                        {
                            string sqlX2 = "INSERT INTO BAJAJ_due_DATA (" + dataBaseField + ", Mon_no, Year_No, UserId, Import_Dt, ImportDataType, NEWINSERT) VALUES(";
                            for (int Count_Loop = 0; Count_Loop < selectedFileField_List.Count; Count_Loop++)
                            {
                                string fieldName = selectedFileField_List[Count_Loop]; // Matching Column Found or not

                                string fieldValue = row[fieldName].ToString().Trim();
                                if (fieldValue.Contains("'"))
                                {
                                    fieldValue = fieldValue.Replace("'", "");
                                }

                                if (excelData.Columns[fieldName].DataType == typeof(DateTime))
                                {
                                    DateTime dtValue;
                                    if (DateTime.TryParse(fieldValue, out dtValue))
                                    {
                                        string formattedDate = dtValue.ToString("dd-MMM-yyyy");
                                        sqlX2 += "'" + formattedDate + "',";
                                    }
                                    else
                                    {
                                        sqlX2 += "NULL,";
                                    }
                                }
                                else
                                {
                                    if (fieldValue.Contains(","))
                                    {
                                        fieldValue = fieldValue.Replace(",", "");
                                    }
                                    sqlX2 += "'" + fieldValue + "',";
                                }
                            }

                            sqlX2 += "'" + Convert.ToInt32(monthValue) + "', '" + yearValue + "', ";
                            sqlX2 += "'" + logginId + "', TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + "', 'DD-MON-YYYY'), '" + MyImportDataType + "', 'BAL' ";
                            sqlX2 += ")";
                            sqlX2 = sqlX2.Replace("''", "NULL");


                            /*
                             * 
                             * INSERT INTO BAJAJ_due_DATA (PAY_MODE,    POLICY_NO,  CL_ADD1,CL_ADD2,CL_ADD3,CL_ADD4,CL_ADD5,CL_CITY,CL_NAME,COMPANY_CD, DOC,            DUE_DATE,       CL_MOBILE,PLY_TERM, NET_AMOUNT, PREM_AMT,   PREM_TERM,  SA,     PREM_FREQ,  Mon_no, Year_No,    UserId,     Import_Dt,                              ImportDataType, NEWINSERT) 
                             *                      VALUES('Non ECS',   '558452792',NULL,   NULL,   NULL,   NULL,   NULL,   NULL,   NULL,  'BAJAJ A',   '28-Apr-2023',  '28-Apr-2025',  NULL,     '78',     '43062',    '44030.9',  '10',       NULL,   '1',        '2',    '2030',     '38387',    TO_DATE('12-May-2025', 'DD-MON-YYYY'), 'DUEDATA',       'BAL' )
                             */
                            dbTable = pc.ExecuteCurrentQueryMaster(sqlX2, out rowNum, out isExceptino);
                            
                            
                            
                            if (MyImportDataType == "LAPSEDDATA")
                            {
                                DateTime MyLapsedDate = DateTime.Now;
                                string sql = " SELECT   policy_no, company_cd, MAX (due_date) due_date,max(mon_no),max(year_no), ";
                                sql = sql + "         (SELECT MAX (status_cd) ";
                                sql = sql + "            FROM bajaj_due_data ";
                                sql = sql + "           WHERE UPPER (TRIM (policy_no)) = ";
                                sql = sql + "                                         UPPER (TRIM ('" + row[Excel_policy_no].ToString().ToUpper().Trim() + "')) ";
                                sql = sql + "             AND UPPER (TRIM (company_cd)) = '" + row[Excel_Comp].ToString().ToUpper().Trim() + "'";
                                sql = sql + "             AND importdatatype = 'DUEDATA' ";
                                sql = sql + "             AND due_date = ";
                                sql = sql + "                    (SELECT MAX (due_date) ";
                                sql = sql + "                       FROM bajaj_due_data ";
                                sql = sql + "                      WHERE UPPER (TRIM (policy_no)) = ";
                                sql = sql + "                                                    UPPER (TRIM ('" + row[Excel_policy_no].ToString().ToUpper().Trim() + "')) ";
                                sql = sql + "                        AND UPPER (TRIM (company_cd)) = ";
                                sql = sql + "                                                   '" + row[Excel_Comp].ToString().ToUpper().Trim() + "' ";
                                sql = sql + "                        AND due_date IS NOT NULL AND IMPORTDATATYPE='DUEDATA' ";
                                sql = sql + "                        )) status_cd ";
                                sql = sql + "    FROM bajaj_due_data a ";
                                sql = sql + "   WHERE   importdatatype = 'DUEDATA' ";
                                sql = sql + "                      AND UPPER (TRIM (policy_no)) = ";
                                sql = sql + "                                                    UPPER (TRIM ('" + row[Excel_policy_no].ToString().ToUpper().Trim() + "')) ";
                                sql = sql + "                        AND UPPER (TRIM (company_cd)) = ";
                                sql = sql + "                                                   '" + row[Excel_Comp].ToString().ToUpper().Trim() + "' ";
                                sql = sql + "GROUP BY policy_no, company_cd ";

                                dbTable = pc.ExecuteCurrentQueryMaster(sql, out rowNum, out isExceptino);
                                //if (!RsDueDate.EOF)
                                if (isExceptino == null && rowNum > 0)
                                {
                                    if (MyLapsedDate >= Convert.ToDateTime(dbTable.Columns["due_date"]))
                                    {
                                        string sql_Y = ("update bajaj_due_Data set status_Cd='LAPSED',last_update_dt='" + DateTime.Now.ToString("dd-MMM-yyyy") + "',last_update='" + logginId + "' WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().ToUpper().Trim() + "' and due_date='" + Convert.ToDateTime(row["due_date"]).ToString("dd-MMM-yyyy") + "' and importdatatype='DUEDATA' ");
                                        string sql_Y2 = ("update policy_details_master set last_status='L',UPDATE_PROG='" + ddlImportDataType.Text + "',UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().ToUpper().Trim() + "'");

                                        //update_bajajar_status(rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim(), rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim(), "L", "LAPSED DATA");
                                        UpdateBajajStatus(row[Excel_policy_no]?.ToString(), row[Excel_Comp]?.ToString(), "L", "LAPSSSSED DATA", monthValue, yearValue);
                                    }
                                }
                                //RsDueDate.Close();
                            }
                            //MyConn.Execute(SqlStr);

                            /* SAMPLE:
                            
                            select * from bajaj_due_Data 
                            WHERE upper(trim(POLICY_no))=upper(trim('558452792')) 
                            and upper(trim(company_cd))= 'BAJAJ A' 
                            and mon_no = 2 and year_no=2030 
                            and importdatatype='DUEDATA' 
                            
                             */
                            dbTable = pc.ExecuteCurrentQueryMaster(SqlStr, out rowNum, out isExceptino);



                            string fileNameNe = "DuePaidMappedField" + currentImpDataName + ".txt";

                            DataTable DTsqlX = new DataTable();
                            if (MyImportDataType == "DUEDATA")
                            {
                                /*SAMPLE:
                                update policy_details_master 
                                set FILE_NAME='DuePaidMappedFieldDUE.txt', 
                                PAYMENT_MODE='NON ECS',                                
                                UPDATE_PROG='DUE',
                                UPDATE_USER='38387',
                                UPDATE_DATE=TO_DATE('12-May-2025','DD/MM/YYYY') 
                                WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('558452792')) 
                                and upper(trim(company_cd))= 'BAJAJ A'
                                 */
                                
                                string sqlX3 = ("update policy_details_master set FILE_NAME='" + fileNameNe + "', PAYMENT_MODE='" +
                                    row[Excel_Payment_Mode].ToString().Trim().ToUpper() + "',UPDATE_PROG='" + ddlImportDataType.Text +
                                    "',UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") +
                                    "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() +
                                    "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                dbTable = pc.ExecuteCurrentQueryMaster(sqlX3, out rowNum, out isExceptino);

                                if (MyPremFreq != "")
                                {
                                    /* SAMPLE:
                                    update policy_details_master set 
                                    FILE_NAME='DuePaidMappedFieldDUE.txt', 
                                    PREM_FREQ='1',
                                    UPDATE_PROG='DUE',
                                    UPDATE_USER='38387',
                                    UPDATE_DATE=TO_DATE('12-May-2025','DD/MM/YYYY') 
                                    WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('558452792')) 
                                    and upper(trim(company_cd))= 'BAJAJ A' 
                                     */
                                    
                                    string sqlX4 = ("update policy_details_master set FILE_NAME='" + fileNameNe + "', PREM_FREQ='" + MyPremFreq +
                                        "',UPDATE_PROG='" + ddlImportDataType.Text + "',UPDATE_USER='" + logginId +
                                        "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" +
                                        row[Excel_policy_no].ToString().Trim().ToUpper() + "')) and upper(trim(company_cd))= '" +
                                        row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                    dbTable = pc.ExecuteCurrentQueryMaster(sqlX4, out rowNum, out isExceptino);


                                }

                                if ((row[doc].ToString().Trim().ToUpper() + "") != "")
                                {

                                    /*SAMPLE:
                                    
                                    update policy_details_master set 
                                    doc=to_date('4/28/2023 12:00:00 AM','dd/mm/rrrr'),
                                    UPDATE_USER='38387',
                                    UPDATE_DATE=TO_DATE('12-May-2025','DD/MM/YYYY') 
                                    WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('558452792')) 
                                    and upper(trim(company_cd))= 'BAJAJ A'
                                    
                                     */


                                    string sqlX5 = ("update policy_details_master set doc=to_date('" + row[doc].ToString().Trim().ToUpper() +
                                        "','dd/mm/rrrr'),UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") +
                                        "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() +
                                        "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                    dbTable = pc.ExecuteCurrentQueryMaster(sqlX5, out rowNum, out isExceptino);


                                }

                                if ((row[excel_pol_term].ToString().Trim().ToUpper() + "") != "")
                                {
                                    /* SAMPLE:
                                     
                                    update policy_details_master set 
                                    ply_term='78',
                                    UPDATE_USER='38387',
                                    UPDATE_DATE=TO_DATE('12-May-2025','DD/MM/YYYY') 
                                    WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('558452792')) 
                                    and upper(trim(company_cd))= 'BAJAJ A'

                                     */
                                    string sqlX6 = ("update policy_details_master set ply_term='" + row[excel_pol_term].ToString().Trim().ToUpper() +
                                        "',UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") +
                                        "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() +
                                        "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                    dbTable = pc.ExecuteCurrentQueryMaster(sqlX6, out rowNum, out isExceptino);


                                }
                                if ((row[excel_mobile].ToString().Trim().ToUpper() + "") != "")
                                {

                                    /* SAMPLE:
                                     
                                     */
                                    string sqlX7 = ("update policy_details_master set mobile='" + row[excel_mobile].ToString().Trim().ToUpper() +
                                        "',UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") +
                                        "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() +
                                        "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                    dbTable = pc.ExecuteCurrentQueryMaster(sqlX7, out rowNum, out isExceptino);


                                }
                                if ((row[excel_sa].ToString().Trim().ToUpper() + "") != "")
                                {
                                    /* SAMPLE:

                                    */
                                    string sqlX8 = ("update policy_details_master set sa='" + row[excel_sa].ToString().Trim().ToUpper() +
                                        "',UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") +
                                        "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() +
                                        "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                    dbTable = pc.ExecuteCurrentQueryMaster(sqlX8, out rowNum, out isExceptino);


                                }
                                if ((row[Excel_Prem_Amt].ToString().Trim().ToUpper() + "") != "")
                                {
                                    /* SAMPLE:
                                    
                                    update policy_details_master set 
                                    prem_amt='44030.9',
                                    UPDATE_USER='38387',
                                    UPDATE_DATE=TO_DATE('12-May-2025','DD/MM/YYYY') 
                                    WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('558452792')) 
                                    and upper(trim(company_cd))= 'BAJAJ A'

                                     */
                                    string sqlX9 = ("update policy_details_master set prem_amt='" + row[Excel_Prem_Amt].ToString().Trim().ToUpper() +
                                        "',UPDATE_USER='" + logginId + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") +
                                        "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() +
                                        "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                                    dbTable = pc.ExecuteCurrentQueryMaster(sqlX9, out rowNum, out isExceptino);
                                }
                            }
                            //Label5.Text = Rec_Count.ToString();

                        }
                    }

                    impCount += 1;
                }

                string returnMsg = $"Total {impCount} record(s) imported!";
                pc.ShowAlert(this, returnMsg);
                return;
            }


             
            else if (chkOptPaid.Checked)
            {
                foreach (DataRow row in excelData.Rows)
                {
                    if (!string.IsNullOrEmpty(row[Excel_Comp]?.ToString()))
                    {
                        // Insert if not in BAJAJ_DUE_DATA
                        if (ddlImportDataType.SelectedIndex == 3 || ddlImportDataType.SelectedIndex == 4) // paid and reinstate
                        {
                            #region Insert into BAJAJ_DUE_DATA
                            string checkLapSql = $@"
select POLICY_NO 
from bajaj_due_data 
where UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' 
and mon_no ={monthValue} 
and year_no={yearValue} 
AND IMPORTDATATYPE='{MyImportDataType}'";
                            DataTable lapTable = pc.ExecuteCurrentQueryMaster(checkLapSql, out int lapRowNum, out string lapException);
                            if (lapRowNum <= 0)
                            {
                                StringBuilder sqlInsert = new StringBuilder();
                                sqlInsert.Append($"Insert into BAJAJ_DUE_DATA ({dataBaseField},Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG) Values(");
                                for (int Count_Loop = 0; Count_Loop < selectedFileField_List.Count; Count_Loop++)

                                {
                                    string fieldName = selectedFileField_List[Count_Loop]; // Matching Column Found or not

                                    string fieldValue = row[fieldName].ToString();
                                    if (fieldValue.Contains("'"))
                                    {
                                        fieldValue = fieldValue.Replace("'", "");
                                    }

                                    if (excelData.Columns[fieldName].DataType == typeof(DateTime))
                                    {
                                        DateTime dtValue;
                                        if (DateTime.TryParse(fieldValue, out dtValue))
                                        {
                                            sqlInsert.Append($"'{dtValue.ToString("dd-MMM-yyyy")}',");
                                        }
                                        else
                                        {
                                            sqlInsert.Append("NULL,");
                                        }
                                    }
                                    else
                                    {
                                        sqlInsert.Append($"'{fieldValue.Trim()}',");
                                    }
                                }
                                sqlInsert.Append($"{monthValue}, {yearValue}, '{logginId}', '{ServerDateTime.ToString("dd-MMM-yyyy")}', '{MyImportDataType}', NULL, 'FORCE FULL')");
                                string finalSql = sqlInsert.ToString().Replace("''", "Null");
                                pc.ExecuteCurrentQueryMaster(finalSql, out _, out _);
                                impCount +=1;
                            }
                            #endregion
                        }

                        #region  Helpers to prepare to track data in BAJAJ_PAID_DATA

                        bool isExcelPolicyNo = pc.Dt_DoesColumnExistInRow(row, Excel_policy_no);
                        bool isExcelComp = pc.Dt_DoesColumnExistInRow(row, Excel_Comp);
                        if (!isExcelPolicyNo || !isExcelComp)
                        {
                            pc.ShowAlert(this, "Policy No or Company Code not found in the selected sheet.");
                            return;
                        }

                        string pExPolicyNo = ((pc.Dt_DoesColumnExistInRow(row, Excel_policy_no)) ? (row[Excel_policy_no].ToString().Trim()) : "");
                        string pExComp = ((pc.Dt_DoesColumnExistInRow(row, Excel_Comp)) ? (row[Excel_Comp].ToString().Trim()) : "");

                        string checkPaidSql = $@"
select * from bajaj_paid_Data 
WHERE upper(trim(POLICY_no))=upper(trim('{pExPolicyNo}')) 
and upper(trim(company_cd))= '{pExComp}' 
and mon_no={monthValue} 
and year_no={yearValue} 
and importdatatype='{MyImportDataType}'";

                        /* SAMPLE:

select * from bajaj_paid_Data 
WHERE upper(trim(POLICY_no))=upper(trim('558452792')) 
and upper(trim(company_cd))= 'BAJAJ A' 
and mon_no=2 
and year_no=2031 
and importdatatype='DUEDATA'
                        */
                        DataTable paidTable = pc.ExecuteCurrentQueryMaster(checkPaidSql, out int paidRowNum, out string paidException);

                        #endregion
                        
                        // Insert if not in BAJAJ_PAID_DATA
                        if (paidRowNum <= 0)
                        {
                            #region Helpers to prepare to track data in BAJAJ_PAID_DATA
                            int xy = 0;

                            /* SAMPLE:
                             Select policy_no,company_cd,last_status from policy_details_master 
                            WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('558452792')) and upper(trim(company_cd))= 'BAJAJ A'
                            */
                            string headerSql = $"Select policy_no,company_cd,last_status from policy_details_master WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                            DataTable headerTable = pc.ExecuteCurrentQueryMaster(headerSql, out int headerRowNum, out string headerException);

                            if (headerRowNum > 0)
                            {
                                if (headerTable.Rows[0]["last_status"].ToString().ToUpper() == "L" &&
                                    row[Excel_Status].ToString().ToUpper() == "PAID")
                                {
                                    string updateSql = $@"
                                            UPDATE POLICY_DETAILS_MASTER SET 
                                            LAST_STATUS='R',
                                            UPDATE_PROG='{ddlImportDataType.Text}',
                                            UPDATE_USER='{logginId}',
                                            UPDATE_DATE=to_date('{ServerDateTime.ToString("dd/MM/yyyy")}','dd/mm/yyyy') 
                                            WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) 
                                            AND UPPER(TRIM(COMPANY_CD))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                                    pc.ExecuteCurrentQueryMaster(updateSql, out _, out _);
                                }
                            }
                            else
                            {
                                string insertSql = $@"
                                        insert into PolicyNotInHeader(policy_no,company_Cd) 
                                        values(('{pc.DtToUpperTrim(row, Excel_policy_no)}'),'{pc.DtToUpperTrim(row,Excel_Comp)}')";

                                pc.ExecuteCurrentQueryMaster(insertSql, out _, out _);
                            }

                            string maxDateSql = $"select max(to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')) from_dt from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and status_cd='LAPSED'";
                            DataTable maxDateTable = pc.ExecuteCurrentQueryMaster(maxDateSql, out int maxDateRowNum, out string maxDateException);

                            string updateDueSql;
                            if (maxDateRowNum > 0 && maxDateTable.Rows[0][0] != DBNull.Value)
                            {
                                DateTime maxDate = Convert.ToDateTime(maxDateTable.Rows[0][0]);
                                updateDueSql = $"update bajaj_due_Data set status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',last_update_dt='{ServerDateTime.ToString("dd-MMM-yyyy")}',last_update='{logginId}' WHERE UPPER(TRIM(POLICY_no))=upper(trim('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and importdatatype='{MyImportDataType}' AND to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')>'{maxDate.ToString("dd-MMM-yyyy")}' and to_date('01/'||mon_no||'/'||year_no,'dd/mm/yyyy')<=last_day(to_date('01-{monthValue}-{yearValue}','dd-mm-yyyy'))";
                            }
                            else
                            {
                                updateDueSql = $"update bajaj_due_Data set status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',last_update_dt='{ServerDateTime.ToString("dd-MMM-yyyy")}',last_update='{logginId}' WHERE UPPER(TRIM(POLICY_no))=upper(trim('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and importdatatype='{MyImportDataType}' AND mon_no ={monthValue} and year_no={yearValue}";
                            }
                            pc.ExecuteCurrentQuery3(updateDueSql, out xy, out _, out bool isExecuted);
                            #endregion

                            // Insert in BAJAJ_PAID_DATA if got PAID in BAJAJ_DUE_DATA  
                            if (row[Excel_Status].ToString().ToUpper().Trim() == "PAID" && isExecuted)
                            {
                                #region Insert in BAJAJ_PAID_DATA
                                StringBuilder paidInsert = new StringBuilder();
                                paidInsert.Append($"INSERT INTO BAJAJ_PAID_DATA ({dataBaseField},MON_NO,YEAR_NO,USERID,IMPORT_DT,IMPORTDATATYPE) VALUES(");

                                for (int Count_Loop = 0; Count_Loop < selectedFileField_List.Count; Count_Loop++)
                                {
                                    string fieldName = selectedFileField_List[Count_Loop]; // Matching Column Found or not
                                    string fieldValue = row[fieldName].ToString();
                                    if (fieldValue.Contains("'"))
                                    {
                                        fieldValue = fieldValue.Replace("'", "");
                                    }
                                    if (excelData.Columns[fieldName].DataType == typeof(DateTime))

                                    {
                                        DateTime dtValue;
                                        if (DateTime.TryParse(fieldValue, out dtValue))
                                        {
                                            paidInsert.Append($"'{dtValue.ToString("dd-MMM-yyyy")}',");
                                        }
                                        else
                                        {
                                            paidInsert.Append("NULL,");
                                        }
                                    }
                                    else
                                    {
                                        paidInsert.Append($"'{fieldValue.Trim()}',");
                                    }

                                }

                                paidInsert.Append($"{monthValue}, {yearValue}, '{logginId}', '{ServerDateTime.ToString("dd-MMM-yyyy")}', '{MyImportDataType}')");
                                string finalPaidSql = paidInsert.ToString().Replace("''", "Null");
                                pc.ExecuteCurrentQueryMaster(finalPaidSql, out _, out _);

                                #endregion

                                #region Update  policy_details_master adn related tables
                                string updateStatusSql = $"update policy_details_master set last_status='A',UPDATE_PROG='{ddlImportDataType.Text}',UPDATE_USER='{logginId}',UPDATE_DATE=to_date('{ServerDateTime.ToString("dd/MM/yyyy")}','dd/mm/yyyy') WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}'";
                                pc.ExecuteCurrentQueryMaster(updateStatusSql, out _, out _);
                                UpdateBajajStatus(row[Excel_policy_no]?.ToString(), row[Excel_Comp]?.ToString(), "F", "PAID DATA", monthValue, yearValue);
                                #endregion

                                //lblCount.Text = Rec_Count.ToString();
                                impCount += 1;
                            }
                        }
                        else
                        {
                            string updatePaidSql = $"update bajaj_paid_Data set status_Cd='{row[Excel_Status].ToString().ToUpper().Trim()}',last_update_dt='{DateTime.Now.ToString("dd-MMM-yyyy")}',last_update='{logginId}' WHERE UPPER(TRIM(POLICY_no))=UPPER(TRIM('{row[Excel_policy_no].ToString().ToUpper().Trim()}')) and upper(trim(company_cd))= '{row[Excel_Comp].ToString().ToUpper().Trim()}' and mon_no={monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}'";
                            pc.ExecuteCurrentQueryMaster(updatePaidSql, out _, out _);

                            impCount -=1;
                            //lblRec1.Text = rec1.ToString();
                        }
                    }

                    Rec_Count_exl++;
                    //lblRecCountExl.Text = Rec_Count_exl.ToString();

                    rec1 += 1;

                }

                #region Handle duplicate policies
                string dupPolicySql = $"select policy_no from (select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_paid_Data b where upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = {monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}' group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(A.company_cd)>1)";
                DataTable dupPolicyTable = pc.ExecuteCurrentQueryMaster(dupPolicySql, out int dupPolicyRowNum, out string dupPolicyException);

                StringBuilder dupPolicyStr = new StringBuilder();
                pc.ExecuteCurrentQueryMaster("DELETE FROM DUP_POLICY", out _, out _);

                foreach (DataRow dupRow in dupPolicyTable.Rows)
                {
                    string insertDupSql = $"INSERT INTO DUP_POLICY VALUES('{dupRow["policy_no"]}')";
                    pc.ExecuteCurrentQueryMaster(insertDupSql, out _, out _);
                    dupPolicyStr.Append($"'{dupRow["policy_no"]}',");
                    impCount += 1;
                }

                if (dupPolicyStr.Length > 0)
                {
                    dupPolicyStr.Length--; // Remove last comma
                    string updateDupSql = $"update bajaj_paid_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no ={monthValue} and year_no={yearValue} and importdatatype='{MyImportDataType}'";
                    int recDel = 0;
                    pc.ExecuteCurrentQueryMaster(updateDupSql, out recDel, out _);
                    Rec_Count -= recDel;
                }

                #endregion

                #region Update employee and investor codes
                string updateEmpSql = $@"update bajaj_paid_data A set A.emp_no =
        (select MAX(B.emp_no) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1),
        inv_cd=(select MAX(B.client_Cd) from bajaj_ar_head B where upper(trim(B.Company_cd))=upper(trim(A.Company_cd)) and B.policy_no=A.policy_no AND B.FRESH_RENEWAL=1)
        where mon_no= {monthValue} and year_no= {yearValue} and dup_Rec is null and importdatatype='{MyImportDataType}'";
                
                pc.ExecuteCurrentQueryMaster(updateEmpSql, out _, out _);
                #endregion
                
                int totalRec = rec1;
                int impRec = impCount;
                int updRec = totalRec - impRec;

                string returnMsg = $"Total {updRec} record(s) processed!";
               lblMesageShow.Text = returnMsg;
                pc.ShowAlert(this, returnMsg);
                
            }
            
            
            #endregion
        }






        public void UpdateBajajStatus(string plyNo, string compCd, string statusCd, string importType, string cmbMonth, string txtYear)
        {
            try
            {
                string sysArNo = "";
                string sysArDt = "";

                // Query to check if the record exists in BAJAJ_AR_HEAD
                string queryHead = $@"
                SELECT sys_ar_no, TO_CHAR(sys_ar_dt, 'DD-MMM-YYYY') AS sys_ar_dt
                FROM BAJAJ_AR_HEAD 
                WHERE POLICY_NO = '{plyNo}' 
                AND COMPANY_CD = '{compCd}' 
                AND TO_CHAR(SYS_AR_DT, 'MON-YYYY') = '{cmbMonth.ToUpper()}-{txtYear}'
                AND STATUS_CD = '{statusCd}'";

                DataTable dtHead = pc.ExecuteCurrentQuery(queryHead);

                if (dtHead.Rows.Count > 0)
                {
                    sysArNo = dtHead.Rows[0]["sys_ar_no"].ToString();
                    sysArDt = dtHead.Rows[0]["sys_ar_dt"].ToString();
                }
                else
                {
                    return; // No record found, exit function
                }

                // Check if record exists in BAJAJ_AR_DETAILS
                string queryDetails = $@"
                SELECT 1 FROM BAJAJ_AR_DETAILS 
                WHERE SYS_AR_NO = '{sysArNo}' 
                AND STATUS_DT = LAST_DAY(TO_DATE('{sysArDt}', 'DD-MMM-YYYY'))";

                DataTable dtDetails = pc.ExecuteCurrentQuery(queryDetails);
                if (dtDetails.Rows.Count > 0) return; // Record already exists, exit function

                // Update BAJAJ_AR_HEAD
                string updateQuery = $@"
                UPDATE BAJAJ_AR_HEAD 
                SET STATUS_CD = '{statusCd}' 
                WHERE SYS_AR_NO = '{sysArNo}'";

                pc.ExecuteCurrentQuery(updateQuery);

                // Insert into BAJAJ_AR_DETAILS
                string insertQuery = $@"
                INSERT INTO BAJAJ_AR_DETAILS 
                (SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON) 
                VALUES 
                ('{sysArNo}', LAST_DAY(TO_DATE('{sysArDt}', 'DD-MMM-YYYY')), '{statusCd}', '{importType} ' || SYSDATE, TO_DATE('{sysArDt}', 'DD-MMM-YYYY'), SYSDATE)";

                pc.ExecuteCurrentQuery(insertQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating status: " + ex.Message);
            }
        }






        #endregion


        #endregion


        protected void Fun_FillPreviousMapped(string mappingFor, string mappingType, DropDownList ddlPreviousMapped)
        {
            if (!string.IsNullOrEmpty(mappingFor) && !string.IsNullOrEmpty(mappingType))
            {
                DataTable dt = new DataTable();
                string getOldMapped = $@"
SELECT 
FIELD_NAME AS TEXT, 
FIELD_VALUE AS VALUE 
FROM PSM_GLOBAL_WORKING_DATA 
WHERE UPPER(DATA_TYPE_1) = 'MAPPED_MASK' 
AND UPPER(DATA_TYPE_2) = UPPER('{mappingFor}' )
and UPPER(DATA_TYPE_3) = UPPER('{mappingType}') 
AND ROWNUM = 1 
";

                dt = pc.ExecuteCurrentQueryMaster(getOldMapped, out int rc, out string ie);
                if (rc > 0 && string.IsNullOrEmpty(ie))
                {
                    DataRow row = dt.Rows[0];
                    string maskedValue = row["TEXT"]?.ToString();
                    if (!string.IsNullOrEmpty(maskedValue))
                    {
                        ddlPreviousMapped.Items.Clear();
                        string[] split1 = maskedValue.Split(',');
                        foreach (string item in split1)
                        {
                            ddlPreviousMapped.Items.Add(item.Trim());
                        }
                    }
                }
                else
                {
                    ddlPreviousMapped.Items.Clear();
                }
            }
            else
            {
                ddlPreviousMapped.Items.Clear();
            }
            ddlPreviousMapped.Enabled = (ddlPreviousMapped.Items.Count > 1) ? true : false;
        }

        protected void Fun_FetchMapped(string filePath, string excelSheet, string mappingForValue, string mappingTypeValue, DropDownList ddlPreviousMapped)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) &&
                    !string.IsNullOrEmpty(excelSheet) &&
                    !string.IsNullOrEmpty(mappingForValue) &&
                    !string.IsNullOrEmpty(mappingTypeValue))
                {
                    // Get Excel headers
                    List<string> fetchedExcelHeaders = PsmController.GetExcelHeaders(filePath, excelSheet);
                    string currentExcelFields = string.Join(",", fetchedExcelHeaders.Select(h => h.Trim()));

                    // Query existing mapped values
                    string query = $@"
                SELECT FIELD_NAME AS TEXT, FIELD_VALUE AS VALUE 
                FROM PSM_GLOBAL_WORKING_DATA 
                WHERE UPPER(DATA_TYPE_1) = 'MAPPED_MASK' 
                  AND UPPER(DATA_TYPE_2) = UPPER('{mappingForValue}')
                  AND UPPER(DATA_TYPE_3) = UPPER('{mappingTypeValue}') 
                  AND ROWNUM = 1";

                    DataTable dt = pc.ExecuteCurrentQueryMaster(query, out int rc, out string ie);

                    string[] mappings = null;
                    List<string> mappedExcelFields = new List<string>();

                    if (rc > 0 && string.IsNullOrEmpty(ie))
                    {
                        DataRow row = dt.Rows[0];
                        string maskedValue = row["TEXT"]?.ToString(); // Format: [Excel_Field]-->db_field,...

                        if (!string.IsNullOrEmpty(maskedValue))
                        {
                             mappings = maskedValue.Split(',');

                            foreach (string item in mappings)
                            {
                                string[] pair = item.Split(new[] { "-->" }, StringSplitOptions.None);
                                if (pair.Length == 2)
                                {
                                    string excelField = pair[0].Replace("[", "").Replace("]", "").Trim();
                                    mappedExcelFields.Add(excelField);
                                }
                            }

                            string mappedFieldString = string.Join(",", mappedExcelFields);

                            if (string.Equals(currentExcelFields, mappedFieldString, StringComparison.OrdinalIgnoreCase))
                            {
                                ddlPreviousMapped.DataSource = mappedExcelFields;
                                ddlPreviousMapped.DataBind();
                                //Fun_FillPreviousMapped(mappingForValue, mappingTypeValue, ddlPreviousMapped);
                                pc.ShowAlert(this, "Mapping Exist: Uploaded Excel fields are present in the mapped string.");
                            }

                            /* // Case Free mapping check
                            var currentFieldsSet = new HashSet<string>(
                                fetchedExcelHeaders.Select(h => h.Trim().ToUpper())
                            );
                            var mappedFieldsSet = new HashSet<string>(
                                mappedExcelFields.Select(m => m.Trim().ToUpper())
                            );

                            */
                            
                            
                            
                            // Check if all mapped fields exist in current Excel fields
                            if (mappedExcelFields.All(field => fetchedExcelHeaders.Contains(field)))
                            {
                                Fun_FillPreviousMapped(mappingForValue, mappingTypeValue, ddlPreviousMapped);
                                pc.ShowAlert(this, "Mapping Exist: Uploaded Excel fields are present in the mapped string.");
                            
                            }
                            else
                            {
                                pc.ShowAlert(this, "Mapped Excel fields do not exist in the uploaded Excel file.");
                            }
                        }
                    }
                    else
                    {
                        ddlPreviousMapped.Items.Clear();

                    }
                }
                else
                {
                    ddlPreviousMapped.Items.Clear();
                }
            
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message);
                return;
            }
        }


        private void LoadMappingOptions(string mappingFor, PlaceHolder phMappingOptions)
        {
            phMappingOptions.Controls.Clear(); // Clear existing

            List<string> options = new List<string>();
            string defaultChecked = "";

            if (mappingFor == "DUEANDPAID")
            {
                options = new List<string> { "DUE", "PAID", "LAPSED", "REINS" };
                options.Sort();
                defaultChecked = "DUE";
            }
            
            foreach (string option in options)
            {
                RadioButton rb = new RadioButton();
                rb.ID = "rdo_" + option;
                rb.GroupName = "MappingOptions";
                rb.Text = option;
                rb.CssClass = "form-check-input";
                rb.Checked = (option == defaultChecked);

                // Wrap in a div with label
                LiteralControl wrapperStart = new LiteralControl($"<div class='form-check form-check-inline w-25'>");
                LiteralControl label = new LiteralControl($"<label class='form-check-label' for='{rb.ClientID}'></label>");
                LiteralControl wrapperEnd = new LiteralControl("</div>");

                phMappingOptions.Controls.Add(wrapperStart);
                phMappingOptions.Controls.Add(rb);
                phMappingOptions.Controls.Add(label);
                phMappingOptions.Controls.Add(wrapperEnd);
            }
        }

    }
}
