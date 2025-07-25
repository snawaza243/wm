using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using System.Windows.Input;
using System.Globalization;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace WM.Tree
{
    public partial class ImportExportExcel : System.Web.UI.Page
    {
        PsmController pc = new PsmController();
        protected void Page_Load(object sender, EventArgs e)
        {
           

            if (Session["LoginId"] == null)
            {
                pc.RedirectToWelcomePage();
            }
            else
            {
                if (!IsPostBack)
                {
                    newFun();
                    string curremtNPS_IMP = Session["NPS_IMP"] as string; // ECS, NON_ECS
                    string curremtNPS_IMP_CZ = Session["NPS_IMP_ZC"] as string; // Y,N -- on non ecs


                    if (curremtNPS_IMP == "ECS")
                    {
                        lblNPSECSNonECSPage.Text = "NPS ECS Transaction Importing";
                    }
                    else if (curremtNPS_IMP == "NON_ECS")
                    {
                        lblNPSECSNonECSPage.Text = "NPS NON ECS Transaction Importing";
                    }
                    else
                    {
                        Session["NPS_IMP"] = null;
                        Session["NPS_IMP_ZC"] = null;
                        Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
                    }
                }
            }

        }

        public void newFun()
        {

            npsEcsDdlCompany.Items.Clear();
            string sql1 = ("select iss_name,iss_code from iss_master where iss_code='IS02520'");
            npsEcsDdlCompany.Items.Clear();

            DataTable dtSql1 = pc.ExecuteCurrentQuery(sql1);

            if (dtSql1.Rows.Count > 0)
            {
                // bind dtsql1 to npsEcsDdlCompany dropdown with .iss_name,iss_code
                npsEcsDdlCompany.DataSource = dtSql1;
                npsEcsDdlCompany.DataTextField = "iss_name";
                npsEcsDdlCompany.DataValueField = "iss_code";
                npsEcsDdlCompany.DataBind();
            }
            if (npsEcsDdlCompany.Items.Count > 0)
            {
                npsEcsDdlCompany.SelectedIndex = 0;
            }


            string sql2 = (" select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status");

            DataTable dtSql2 = pc.ExecuteCurrentQuery(sql2);
            npExsDdlStatus.Items.Clear();

            if (dtSql2.Rows.Count > 0)
            {
                // bind dtsql2 to npExsDdlStatus dropdown with .status,status_cd
                npExsDdlStatus.DataSource = dtSql2;
                npExsDdlStatus.DataTextField = "status";
                npExsDdlStatus.DataValueField = "status_cd";
                npExsDdlStatus.DataBind();
                npExsDdlStatus.SelectedIndex = 0;
                 
            }
            if (npExsDdlStatus.Items.Count > 0)
            {
                npExsDdlStatus.SelectedIndex = 0;
            }
 
        }


        #region NPSECS Model
        // Submit Button Click Event

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            try
            {

                Session["SheetData"] = null;
                // currrent nps form sesion

                if (Request.Form[hfFileSelected.UniqueID] != "1")
                {
                    pc.ShowAlert(this, "Kindly choose a file before uploading!");
                    return;
                }

                if (NpsEcsFileInput.HasFile)
                {
                    // two type of action ECS and NON ECS
                    string currentNPS_IMP = Session["NPS_IMP"] as string; // ECS, NON_ECS

                    string uploadPath = Server.MapPath("~/Uploads/");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                     

                    // Get the previous file name stored in the session for this type
                    string sessionKey = $"UploadedFile_{currentNPS_IMP}"; 
                    string oldFileName = Session[sessionKey] as string;

                    // Check if an old file exists and delete it
                    if (!string.IsNullOrEmpty(oldFileName))
                    {
                        string oldFilePath = Path.Combine(uploadPath, oldFileName);
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }
                    string newFileName = Path.GetFileName(NpsEcsFileInput.FileName);

                    // Store the extension of the file
                    string fileExtension = Path.GetExtension(NpsEcsFileInput.FileName);

                    string filePath = Path.Combine(uploadPath, sessionKey + "_" + fileExtension);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }

                    NpsEcsFileInput.SaveAs(filePath);


                    try
                    {
                        NpsEcsFileInput.SaveAs(filePath);
                        Session["CurrentNpsEcsExcelFile"] = filePath;

                        LoadExcelSheets(filePath, npsEcsDdlSheetlist);
                        npsEcsLblFileName.Text = "Uploaded File: " + NpsEcsFileInput.FileName;
                        //npsEcsDdlSheetlist.Focus();
                        string curMsg = "File uploaded successfully";
                        //pc.ShowAlert(this, curMsg);
                        npsEcsSuccessMessage.Text = curMsg;
                        npsEcsDdlSheetlist.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        pc.ShowAlert(this, ex.Message);
                        npsEcsDdlSheetlist.Enabled = false;
                        npsEcsLblFileName.Text = "File Name: ";


                    }
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
            }
        
        }


        protected void NpsEcsExcelSheetSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filePath = Session["CurrentNpsEcsExcelFile"]?.ToString();
            string selectedSheet = npsEcsDdlSheetlist.SelectedValue;
            if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(selectedSheet))
            {
                try
                {
                    DataTable sheetData = LoadSheetData(filePath, selectedSheet);

                    if(sheetData.Rows.Count == 0)
                    {
                        pc.ShowAlert(this, "No data found in the selected sheet.");
                        npsEcsSuccessMessage.Text = "No data found in the selected sheet.";
                        Session["SheetData"] = null;

                        GridView1.DataSource = null;
                        GridView1.DataBind();
                        return;
                    }
                    else
                    {
                        npsEcsSuccessMessage.Text = $"Selected sheet: {selectedSheet}, has " + (sheetData.Rows.Count > 0 ? sheetData.Rows.Count.ToString() : "not any") + " rows.";
                        Session["SheetData"] = sheetData;
                        DataTable currentSheetData = (DataTable)Session["SheetData"];
                        CurrentSheetData(currentSheetData);
                    }
                }
                catch (Exception ex)
                {
                    pc.ShowAlert(this, "Error loading sheet data: " + ex.Message);
                    npsEcsSuccessMessage.Text = ex.Message;
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                }
            }
            else
            {
                pc.ShowAlert(this, "File or sheet not present. ");
                npsEcsSuccessMessage.Text = string.Empty;
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
        }

        // IMPORT BUTTON CLICK EVENT
        protected void NpsEcsSubmit_Click(object sender, EventArgs e)
        {

            string[] tabCol =  pc.GetTableColumns("nps_nonecs_tbl_imp").ToArray();

            if (tabCol.Length == 0)
            {
                pc.ShowAlert(this, "Table columns not found.");
                return;
            }
            else
            {
                //pc.ShowAlert(this, string.Join(", ", tabCol));
                //ImportData();
                //NewCOdeImp();
                ImportTransaction();
            }


        }

        public void ImportData()
        {
            string Nps_Importing_flag = "";
            CheckBox OptLife = new CheckBox();
            CheckBox OptGeneral = new CheckBox();
            CheckBox OptNPS = new CheckBox();   
            try
            {
                if (Nps_Importing_flag == "ECS")
                {
                    //ImportTransaction();
                }
                else
                {
                    //ImportNonECSTransaction();
                }

                
                if (OptLife.Checked)
                {
                    // Do nothing
                }
                else if (OptGeneral.Checked)
                {
                    //GIAPPMAP();
                    //pc.ShowAlert(this, "All Records Updated Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (OptNPS.Checked)
                {
                    if (Nps_Importing_flag == "ECS")
                    {
                        //NPSECSIMP();
                    }
                    else
                    {
                        //NPSNONECSIMP();
                    }

                    //pc.ShowAlert(this, "All Records Inserted Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                //pc.ShowAlert(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public void Import_ecs_non_ecs()
        {

            string getLoginID = Session["LoginId"]?.ToString();
            DataTable currentSheetData = Session["SheetData"] as DataTable;
            string selectedSheet = npsEcsDdlSheetlist.SelectedValue;
            string selectedCompany = npsEcsDdlCompany.SelectedValue;
            string selectedStatus = npExsDdlStatus.SelectedValue;
            string inputFieldValue = npsEcsInputField.Text;

            string succRetMsg = "";

            try
            {

                if (currentSheetData == null || currentSheetData.Rows.Count == 0)
                {
                    succRetMsg = "No data found in the selected sheet.";
                    pc.ShowAlert(this, succRetMsg);
                    npsEcsSuccessMessage.Text = succRetMsg;
                    return;
                }
                else
                {
                    string curremtNPS_IMP = Session["NPS_IMP"] as string; // ECS, NON_ECS
                    string currentNPS_IMPCZ = Session["NPS_IMP_ZC"] as string; // Y,N
                    if (curremtNPS_IMP == "ECS")
                    {
                        Session["NPS_IMP_ZC"] = null;
                        currentNPS_IMPCZ = "N";
                        //DataTable excelData = GetGridViewData(selectedFilePath, selectedSheet);
                        DataTable excelData = currentSheetData;

                        try
                        {
                            //int importSummary = WM.Controllers.NpsTransactionPunchingController,ImportDataToDatabase(excelData, getCompany, getLoginID);
                            int importSummary = 1;
                            succRetMsg = $"Total inserted row(s): {importSummary}";
                            pc.ShowAlert(this, succRetMsg);
                            npsEcsSuccessMessage.Text = succRetMsg;
                        }
                        catch (Exception ex)
                        {
                            succRetMsg = $"An error occurred: {ex.Message}";
                            npsEcsSuccessMessage.Text = succRetMsg;
                            pc.ShowAlert(this, "An error occurred while importing data. Please try again.");
                            return;
                        }
                    }
                    else if (curremtNPS_IMP == "NON_ECS")
                    {
                        try
                        {
                            //int importSummary = WM.Controllers.NpsTransactionPunchingController,ImportDataToDatabase(excelData, getCompany, getLoginID);
                            int importSummary = 1;
                            succRetMsg = $"Total inserted row(s): {importSummary}";
                            pc.ShowAlert(this, succRetMsg);
                            npsEcsSuccessMessage.Text = succRetMsg;
                        }
                        catch (Exception ex)
                        {
                            // Handle unexpected errors
                            succRetMsg = $"An error occurred: {ex.Message}";
                            npsEcsSuccessMessage.Text = succRetMsg;
                            pc.ShowAlert(this, "An error occurred while importing data. Please try again.");
                            return;
                        }
                    }
                    else
                    {
                        pc.ShowAlert(this, "Invalid NPS Importing flag.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                succRetMsg = $"An error occurred: {ex.Message}";
                npsEcsSuccessMessage.Text = succRetMsg;
                npsEcsSuccessMessage.CssClass = "text-danger";
                return;
            }

        }
        // Reset Button Click Event
        protected void NpsEcsReset_Click(object sender, EventArgs e)
        {
            Session["SheetData"] = null;
            npsEcsLblFileName.Text = "File Name: ";
            DdlSelectIndexZero(npsEcsDdlSheetlist);
            DdlSelectIndexZero(npsEcsDdlCompany);
            npsEcsInputField.Text = string.Empty;
            npsEcsSuccessMessage.Text = string.Empty;
            GridView1.DataSource = null;
            GridView1.DataBind();
        }

        protected void NpsEcsExit_Click(object sender, EventArgs e)
        {
            if (Session["NPS_IMP"] != null)
            {
                Session["NPS_IMP"] = null;
                Session["NPS_IMP_ZC"] = null;
                Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
            }
            else
            {
                Session["NPS_IMP"] = null;
                Session["NPS_IMP_ZC"] = null;
                pc.RedirectToWelcomePage();
            }
        }

        public static void LoadExcelSheets(string filePath, DropDownList sheetDropDown)
        {

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString_N(filePath)))
            {
                conn.Open();
                DataTable dtSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                sheetDropDown.Items.Clear();
                sheetDropDown.Items.Add(new ListItem("-- Select Sheet --", ""));

                foreach (DataRow row in dtSheets.Rows)
                {
                    string sheetName = row["TABLE_NAME"].ToString();
                    // Remove $ from sheet name that Excel adds
                    sheetName = sheetName.Replace("$", "").Replace("'", "");
                    sheetDropDown.Items.Add(new ListItem(sheetName, sheetName));
                }
            }
        }

        public static DataTable LoadSheetData(string filePath, string sheetName)
        {
            DataTable dtResult = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString_N(filePath)))
            {
                conn.Open();
                string query = $"SELECT * FROM [{sheetName}$]";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        adapter.Fill(dtResult);
                    }
                }
            }

            return dtResult;
        }

        private static string GetExcelConnectionString_N(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            string connectionString = string.Empty;

            if (extension == ".xls")
            {
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
            }
            else if (extension == ".xlsx")
            {
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1';";
            }

            return connectionString;
        }

        private DataTable GetGridViewData(string filePath, string sheetName)
        {
            // Use the ReadExcelData method to get the initial DataTable
            DataTable rawData = ReadExcelData(filePath, sheetName);
            DataTable cleanData = new DataTable();

            // Check if rawData has columns
            if (rawData.Columns.Count > 0)
            {
                // Trim spaces from column names and add them to cleanData
                foreach (DataColumn column in rawData.Columns)
                {
                    string trimmedColumnName = column.ColumnName.Trim(); // Trim spaces
                    cleanData.Columns.Add(trimmedColumnName);
                }

                // Add rows to cleanData
                foreach (DataRow row in rawData.Rows)
                {
                    DataRow newRow = cleanData.NewRow();
                    foreach (DataColumn column in rawData.Columns)
                    {
                        newRow[column.ColumnName.Trim()] = row[column]; // Keep the trimmed column name
                    }
                    cleanData.Rows.Add(newRow);
                }
            }

            return cleanData;
        }

        private DataTable ReadExcelData(string filePath, string sheetName)
        {
            DataTable dataTable = new DataTable();

            string connString = GetExcelConnectionString_N(filePath);
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {

                    // Select all data from the sheet

                    // Ensure that the sheetName does not include the $ symbol and append it only if necessary
                    string query = $"SELECT * FROM [{sheetName.TrimEnd('$')}$]";

                    //string query = $"SELECT * FROM [{sheetName}$]";

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, conn);
                    dataAdapter.Fill(dataTable);

                }
                catch (Exception ex)
                {
                    npsEcsSuccessMessage.Text = ex.Message;

                }
            }

            return dataTable;
        }

        protected void PaginationGridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataTable currentSheetData = (DataTable)Session["SheetData"];
            CurrentSheetData(currentSheetData);

        }

        public void CurrentSheetData(DataTable dt)
        {

            if (dt != null)
            {
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                Session["SheetData"] = null;
                GridView1.DataSource = null;
                GridView1.DataBind();

            }
        }
        #endregion


        #region Helper Methods

        // a void type mathos with ddl prama that will select index 0 if have items
        public void DdlSelectIndexZero(DropDownList ddl)
        {
            if (ddl.Items.Count > 0)
            {
                ddl.SelectedIndex = 0;
            }
        }
        #endregion


        #region NEW CODE

        public void NewCOdeImp()
        {
            /* TransactionImporter importer = new TransactionImporter();

             // Initialize sample values for testing
             importer.filepath = "sample.xls";
             importer.sheetName = "Sheet1";
             importer.Glbloginid = "User1";
             importer.ServerDateTime = DateTime.Now;
             importer.TxtFile_Name = "mapping.txt";

             // Fix DropDownList issue
             importer.cmbCompany.Items.Add(new ListItem("COMPANY#001", "COMPANY#001"));
             importer.cmbCompany.SelectedValue = "COMPANY#001";

             importer.CmbStatus.Items.Add(new ListItem("ACTIVE#A", "ACTIVE#A"));
             importer.CmbStatus.SelectedIndex = 0;

             // Fix RadioButton selection
             importer.OptLife.Checked = true;
             importer.OptGeneral.Checked = false;
             importer.OptNPS.Checked = false;

             // Ensure ImportTransaction() exists
             //importer.ImportTransaction();
 */
            ImportTransaction();
        }

        #region GLOBAL VARIABLES

        public DropDownList CmbStatus;
        public DropDownList cmbCompany;
        public RadioButton OptLife;
        public RadioButton OptGeneral;
        public RadioButton OptNPS;
        public Label Label5;
        public string TxtFile_Name;
        public string filepath;
        public string sheetName;
        public string Glbloginid;
        public DateTime ServerDateTime;

        // Global variables used in the code
        public string Status_Chk;
        public string importFile;
        public int Total_records = 0;
        public int Already_Exist = 0;
        public int countNull = 0;

        #endregion

        public class TransactionImporter
        {
            public DropDownList CmbStatus { get; set; } = new DropDownList();
            public DropDownList cmbCompany { get; set; } = new DropDownList();
            public RadioButton OptLife { get; set; } = new RadioButton();
            public RadioButton OptGeneral { get; set; } = new RadioButton();
            public RadioButton OptNPS { get; set; } = new RadioButton();
            public Label Label5 { get; set; } = new Label();

            public string TxtFile_Name;
            public string filepath;
            public string sheetName;
            public string Glbloginid;
            public DateTime ServerDateTime;

            // Global variables used in the code
            public string Status_Chk;
            public string Comp_Cd;
            public string importFile;
            public int Total_records = 0;
            public int Already_Exist = 0;
            public int countNull = 0;

            public void ImportTransaction()
            {
                // Dummy method to avoid errors
                Console.WriteLine("Import Transaction called.");
            }
        }


        public void ImportTransaction()
        {
            try
            {
                string Sql = "";
                string FileFields = "";
                string dataDaseField = "";
                string selectedFileField = "";
                string str1;

                string curremtNPS_IMP = Session["NPS_IMP"] as string; // ECS, NON_ECS
                bool OptNPS = (!string.IsNullOrEmpty(curremtNPS_IMP)) ? true : false;
                Status_Chk = npExsDdlStatus.SelectedValue.ToString();
                string Comp_Cd = npsEcsDdlCompany.SelectedValue;

                // 1. Get columns from session DataTable
                DataTable dtExcel = (DataTable)Session["SheetData"];
                List<string> excelSheetCol = new List<string>();

                if (dtExcel != null)
                {
                    foreach (DataColumn col in dtExcel.Columns)
                    {
                        excelSheetCol.Add(col.ColumnName);
                    }
                }

                // 2. Get columns from Oracle Table
                string findTableCol = "NPS_ECS_TBL_IMP";
                string getDBCol = $"SELECT COLUMN_NAME FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = '{findTableCol}' ORDER BY COLUMN_ID";

                DataTable dtDBColumns = pc.ExecuteCurrentQuery(getDBCol);
                List<string> dbColumns = new List<string>();

                if (dtDBColumns.Rows.Count > 0)
                {
                    foreach (DataRow row in dtDBColumns.Rows)
                    {
                        dbColumns.Add(row["COLUMN_NAME"].ToString());
                    }
                }

                // 3. Compare Columns
                List<string> missingColumns = excelSheetCol.Except(dbColumns).ToList();
                List<string> extraColumns = dbColumns.Except(excelSheetCol).ToList();

                // 4. Output results
                if (missingColumns.Count > 0)
                {
                    Console.WriteLine("Missing Columns in Database: " + string.Join(", ", missingColumns));
                }

                if (extraColumns.Count > 0)
                {
                    Console.WriteLine("Extra Columns in Database: " + string.Join(", ", extraColumns));
                }





                #region REPLACE SPECIAL CHARACTERS IN COLUMN NAMES
                for (int i = 0; i < dtExcel.Columns.Count; i++)
                {
                    string colName = dtExcel.Columns[i].ColumnName;
                    if (!string.IsNullOrEmpty(colName))
                    {
                        colName = colName.Replace(".", "").Replace("/", "").Replace("-", "");
                        dtExcel.Columns[i].ColumnName = colName;
                    }
                }

                #endregion

                #region GET FILE FIELDS COLUMN NAMES
                if (dtExcel.Columns.Count > 0)
                {
                    for (int i = 0; i < dtExcel.Columns.Count; i++)
                    {
                        FileFields += dtExcel.Columns[i].ColumnName + ",";
                    }
                    if (FileFields.Length > 0)
                    {
                        FileFields = FileFields.Substring(0, FileFields.Length - 1);
                    }
                }
                #endregion

                string GetCompTextFilePath_Value = "FieldParametersNPS.txt";// GetCompTextFilePath(Comp_Cd);
                str1 = FieldsParametersName(GetCompTextFilePath_Value);
                //if (str1.Trim() == "") { pc.ShowAlert(this, "File Format Is Wrong"); return; }

                string[] delComma = str1.Split(',');
                selectedFileField = "";
                dataDaseField = "";


                for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                {
                    // delHash = Split(delComma(i), "#")
                    string[] delHash = delComma[i].Split('#');
                    if (delHash.Length >= 2)
                    {
                        selectedFileField += delHash[1] + ",";
                        dataDaseField += delHash[0] + ",";
                    }
                }
                if (selectedFileField.Length > 0)
                {
                    selectedFileField = selectedFileField.Substring(0, selectedFileField.Length - 1);
                }
                if (dataDaseField.Length > 0)
                {
                    dataDaseField = dataDaseField.Substring(0, dataDaseField.Length - 1);
                }

                // 'RsImport.Close
                // 'If CheckFieldsVis(FileFields, selectedFileField) = False Then Exit Sub
                //if (!CheckFieldMapping(dataDaseField.ToUpper(), Status_Chk.ToUpper()))

                string dbTabCol = string.Join(", ", pc.GetTableColumns("nps_nonecs_tbl_imp").ToArray());

                string checkFieldMapping = CheckFieldsVis(dbTabCol.ToUpper(), Status_Chk.ToUpper());
                if (!string.IsNullOrEmpty(checkFieldMapping))
                {
                    pc.ShowAlert(this, checkFieldMapping);
                    return;
                }

                if (OptLife.Checked)
                {
                    // If OptLife.Value = True Then
                    // MyConn.Execute " delete from  Bajaj_PolicyInfo_Data "
                    pc.ExecuteCurrentQuery("delete from Bajaj_PolicyInfo_Data");

                    // Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
                    DataTable RsImport = (DataTable)Session["SheetData"];

                    //DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);

                    Total_records = 0;
                    // Do While Not RsImport.EOF
                    foreach (DataRow row in RsImport.Rows)
                    {
                        Sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from Bajaj_PolicyInfo_Data where ";
                        // For i = 0 To UBound(delComma)
                        for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            // Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.IndexOf("&") >= 0)
                            {
                                Xls_Fld = Exc_Clent_FldName();
                            }
                            // Check the type of field from the Excel DataTable
                            if (RsImport.Columns.Contains(Xls_Fld))
                            {
                                object fieldValue = row[Xls_Fld];
                                // Check if field is Date type (adDate = 7)
                                if (RsImport.Columns[Xls_Fld].DataType == typeof(DateTime))
                                {
                                    // Format as dd-mmm-yyyy
                                    Xls_Fld = Convert.ToDateTime(fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                    Xls_Fld = Xls_Fld.Replace("'", "");
                                    SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                                // Else if the field appears to be a date in string format and meets specific length/position checks
                                else if (IsStringDate(row[Xls_Fld]))
                                {
                                    if (IsNull(row[Xls_Fld]))
                                    {
                                        Xls_Fld = "";
                                        SqlChk += "(" + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = row[Xls_Fld].ToString();
                                        Xls_Fld = Xls_Fld.Replace("'", "");
                                        DateTime dt;
                                        if (DateTime.TryParse(Xls_Fld, out dt))
                                        {
                                            SqlChk += delHash[0] + "='" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                        }
                                        else
                                        {
                                            SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                        }
                                    }
                                }
                                // Else if field is assumed to be string (adVarChar = 202)
                                else if (RsImport.Columns[Xls_Fld].DataType == typeof(string))
                                {
                                    if (IsNull(row[Xls_Fld]))
                                    {
                                        Xls_Fld = "";
                                        SqlChk += " (" + delHash[0] + "='" + Xls_Fld.ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = row[Xls_Fld].ToString().Replace("'", "");
                                        SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                    }
                                }
                                else
                                {
                                    // Default numeric case
                                    if (IsNull(row[Xls_Fld]))
                                    {
                                        Xls_Fld = "0";
                                        SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = row[Xls_Fld].ToString().Replace("'", "");
                                        // Format numeric value using "######0"
                                        decimal decValue;
                                        if (Decimal.TryParse(Xls_Fld, out decValue))
                                        {
                                            SqlChk += delHash[0] + "=" + decValue.ToString("######0") + " ";
                                        }
                                        else
                                        {
                                            SqlChk += delHash[0] + "=" + Xls_Fld + " ";
                                        }
                                    }
                                }
                            }
                            SqlChk += " and ";
                        }
                        SqlChk += " Company_cd='" + Comp_Cd + "' ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = ExecuteSQL(SqlChk);
                        // 'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
                        // 'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            Sql = "Insert into Bajaj_PolicyInfo_Data (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                            for (int i = 0; i < RsImport.Columns.Count; i++)
                            {
                                string fieldName = RsImport.Columns[i].ColumnName;
                                object fieldVal = row[fieldName];
                                if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                                {
                                    string Value1 = fieldVal.ToString().Replace("'", "");
                                    Sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    // Check if field is date type or string date format based on length checks
                                    if (RsImport.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                                    {
                                        string formattedDate = "";
                                        if (!IsNull(fieldVal))
                                        {
                                            DateTime dt;
                                            if (DateTime.TryParse(fieldVal.ToString(), out dt))
                                            {
                                                formattedDate = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim();
                                            }
                                        }
                                        Sql += "'" + formattedDate + "',";
                                    }
                                    else
                                    {
                                        Sql += "'" + fieldVal.ToString().Trim() + "',";
                                    }
                                }
                            }
                            Sql += "'" + Comp_Cd + "','" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                            Sql += ")";
                            Sql = Sql.Replace("''", "Null");
                            Sql = newString(Sql);
                            INSERT_RECORD(Sql);
                            if (countNull >= 50)
                                break;
                        }
                        else
                        {
                            Already_Exist = Already_Exist + 1;
                        }
                        Total_records = Total_records + 1;
                        Label5.Text = Total_records.ToString();
                    }
                }
                else if (OptGeneral.Checked)
                {
                    // ElseIf OptGeneral.Value = True Then
                    DataTable delTable = ExecuteSQL("delete from Bajaj_PolicyInfo_Data_gen");
                    //DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);
                    DataTable RsImport = (DataTable)Session["SheetData"];
                    
                    Total_records = 0;
                    foreach (DataRow row in RsImport.Rows)
                    {
                        Sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from Bajaj_PolicyInfo_Data_gen where ";
                        for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.IndexOf("&") >= 0)
                            {
                                Xls_Fld = Exc_Clent_FldName();
                            }
                            if (RsImport.Columns.Contains(Xls_Fld))
                            {
                                object fieldValue = row[Xls_Fld];
                                if (RsImport.Columns[Xls_Fld].DataType == typeof(DateTime))
                                {
                                    Xls_Fld = Convert.ToDateTime(fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                    Xls_Fld = Xls_Fld.Replace("'", "");
                                    SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                                else if (IsStringDate(fieldValue))
                                {
                                    if (IsNull(fieldValue))
                                    {
                                        Xls_Fld = "";
                                        SqlChk += "(" + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = fieldValue.ToString();
                                        Xls_Fld = Xls_Fld.Replace("'", "");
                                        DateTime dt;
                                        if (DateTime.TryParse(Xls_Fld, out dt))
                                        {
                                            SqlChk += delHash[0] + "='" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                        }
                                        else
                                        {
                                            SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                        }
                                    }
                                }
                                else if (RsImport.Columns[Xls_Fld].DataType == typeof(string))
                                {
                                    if (IsNull(fieldValue))
                                    {
                                        Xls_Fld = "";
                                        SqlChk += " (" + delHash[0] + "='" + Xls_Fld.ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = fieldValue.ToString().Replace("'", "");
                                        SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                    }
                                }
                                else
                                {
                                    if (IsNull(fieldValue))
                                    {
                                        Xls_Fld = "0";
                                        SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = fieldValue.ToString().Replace("'", "");
                                        decimal decValue;
                                        if (Decimal.TryParse(Xls_Fld, out decValue))
                                        {
                                            SqlChk += delHash[0] + "=" + decValue.ToString("######0") + " ";
                                        }
                                        else
                                        {
                                            SqlChk += delHash[0] + "=" + Xls_Fld + " ";
                                        }
                                    }
                                }
                            }
                            SqlChk += " and ";
                        }
                        SqlChk += " Company_cd='" + Comp_Cd + "' ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = ExecuteSQL(SqlChk);
                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            Sql = "Insert into Bajaj_PolicyInfo_Data_gen (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                            for (int i = 0; i < RsImport.Columns.Count; i++)
                            {
                                string fieldName = RsImport.Columns[i].ColumnName;
                                object fieldVal = row[fieldName];
                                if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                                {
                                    string Value1 = fieldVal.ToString().Replace("'", "");
                                    Sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    if (RsImport.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                                    {
                                        string formattedDate = "";
                                        if (!IsNull(fieldVal))
                                        {
                                            DateTime dt;
                                            if (DateTime.TryParse(fieldVal.ToString(), out dt))
                                            {
                                                formattedDate = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim();
                                            }
                                        }
                                        Sql += "'" + formattedDate + "',";
                                    }
                                    else
                                    {
                                        Sql += "'" + fieldVal.ToString().Trim() + "',";
                                    }
                                }
                            }
                            Sql += "'" + Comp_Cd + "','" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                            Sql += ")";
                            Sql = Sql.Replace("''", "Null");
                            Sql = newString(Sql);
                            INSERT_RECORD(Sql);
                            if (countNull >= 50)
                                break;
                        }
                        else
                        {
                            Already_Exist = Already_Exist + 1;
                        }
                        Total_records = Total_records + 1;
                        Label5.Text = Total_records.ToString();
                    }
                }
                else if (OptNPS)
                {
                    // ElseIf OptNPS.Value = True Then
                    DataTable dtInsertExecut = pc.ExecuteCurrentQuery("insert into nps_ecs_tbl_imp_bk select * from nps_ecs_tbl_imp");
                    
                    DataTable dtDel = pc.ExecuteCurrentQuery("delete from nps_ecs_tbl_imp");

                    DataTable RsImport = (DataTable)Session["SheetData"];

                    Total_records = 0;
                    foreach (DataRow row in RsImport.Rows)
                    {
                        Sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from nps_ecs_tbl_imp where ";
                        for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.IndexOf("&") >= 0)
                            {
                                Xls_Fld = Exc_Clent_FldName();
                            }
                            if (RsImport.Columns.Contains(Xls_Fld))
                            {
                                object fieldValue = row[Xls_Fld];
                                if (RsImport.Columns[Xls_Fld].DataType == typeof(DateTime))
                                {
                                    Xls_Fld = Convert.ToDateTime(fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
                                    Xls_Fld = Xls_Fld.Replace("'", "");
                                    SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                                else if (IsStringDate(fieldValue))
                                {
                                    if (IsNull(fieldValue))
                                    {
                                        Xls_Fld = "";
                                        SqlChk += "(" + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = fieldValue.ToString();
                                        Xls_Fld = Xls_Fld.Replace("'", "");
                                        DateTime dt;
                                        if (DateTime.TryParse(Xls_Fld, out dt))
                                        {
                                            SqlChk += delHash[0] + "='" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                        }
                                        else
                                        {
                                            SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                        }
                                    }
                                }
                                else if (RsImport.Columns[Xls_Fld].DataType == typeof(string))
                                {
                                    if (IsNull(fieldValue))
                                    {
                                        Xls_Fld = "";
                                        SqlChk += " (" + delHash[0] + "='" + Xls_Fld.ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = fieldValue.ToString().Replace("'", "");
                                        SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                    }
                                }
                                else
                                {
                                    if (IsNull(fieldValue))
                                    {
                                        Xls_Fld = "0";
                                        SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                                    }
                                    else
                                    {
                                        Xls_Fld = fieldValue.ToString().Replace("'", "");
                                        decimal decValue;
                                        if (Decimal.TryParse(Xls_Fld, out decValue))
                                        {
                                            SqlChk += delHash[0] + "=" + decValue.ToString("######0") + " ";
                                        }
                                        else
                                        {
                                            SqlChk += delHash[0] + "=" + Xls_Fld + " ";
                                        }
                                    }
                                }
                            }
                            SqlChk += " and ";
                        }
                        // SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
                        SqlChk += " 1=1 ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = pc.ExecuteCurrentQuery(SqlChk);
                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            Sql = "Insert into nps_ecs_tbl_imp (" + dataDaseField + ",Import_dt,LOGGEDUserID)  Values(";
                            for (int i = 0; i < RsImport.Columns.Count; i++)
                            {
                                string fieldName = RsImport.Columns[i].ColumnName;
                                object fieldVal = row[fieldName];
                                if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                                {
                                    string Value1 = fieldVal.ToString().Replace("'", "");
                                    Sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    if (RsImport.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                                    {
                                        string formattedDate = "";
                                        if (!IsNull(fieldVal))
                                        {
                                            DateTime dt;
                                            if (DateTime.TryParse(fieldVal.ToString(), out dt))
                                            {
                                                formattedDate = dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim();
                                            }
                                        }
                                        Sql += "'" + formattedDate + "',";
                                    }
                                    else
                                    {
                                        Sql += "'" + fieldVal.ToString().Trim() + "',";
                                    }
                                }
                            }
                            Sql += "'" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                            Sql += ")";
                            Sql = Sql.Replace("''", "Null");
                            Sql = newString(Sql);
                            INSERT_RECORD(Sql);
                            if (countNull >= 50)
                                break;
                        }
                        else
                        {
                            Already_Exist = Already_Exist + 1;
                        }
                        Total_records = Total_records + 1;
                        Label5.Text = Total_records.ToString();
                    }
                }

                Sql = "delete nps_ecs_tbl_imp where ref_tran_code is null";
                DataTable dtInsertSql = pc.ExecuteCurrentQuery(Sql);

                int lblVal;
                if (int.TryParse(Label5.Text, out lblVal))
                {
                    Label5.Text = (lblVal - 1).ToString();
                }
                pc.ShowAlert(this, " Out OF " + (Total_records - 1).ToString() + " Records " + Already_Exist.ToString() + " Are Already Exist ");
            
                return;
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideServerLoader", "hideServerLoader();", true);
                return;
            }
        }

        private DataTable ExecuteSQL(string sql)
        {
            
            DataTable dt = pc.ExecuteCurrentQuery(sql);
            return dt;
        }

        // Helper function that simulates inserting a record by executing an SQL command.
        private void INSERT_RECORD(string sql)
        {
            ExecuteSQL(sql);
        }

        // Helper function that simulates cleaning up a SQL string by removing the trailing " and " if present.
        private string newString(string sql)
        {
            sql = sql.Trim();
            if (sql.EndsWith("and", StringComparison.InvariantCultureIgnoreCase))
            {
                sql = sql.Substring(0, sql.Length - 3).Trim();
            }
            else if (sql.EndsWith("and ", StringComparison.InvariantCultureIgnoreCase))
            {
                sql = sql.Substring(0, sql.Length - 4).Trim();
            }
            return sql;
        }

        // Helper function that simulates getting Excel data from a file as a DataTable.
        // Optionally, you can pass a list of columns (selectedFileField) to return subset columns.
        private DataTable GetExcelDataTable(string filePath, string sheetName, string selectedFileField = "*")
        {
            // Complete implementation that reads Excel file.
            // For now, we return a new DataTable with dummy columns.
            DataTable dt = new DataTable();
            // For simulation purposes, assume selectedFileField is comma separated list of column names.
            if (selectedFileField != "*" && !string.IsNullOrEmpty(selectedFileField))
            {
                string[] cols = selectedFileField.Split(',');
                foreach (string col in cols)
                {
                    dt.Columns.Add(col.Trim());
                }
            }
            else
            {
                // Dummy columns
                dt.Columns.Add("Column1");
                dt.Columns.Add("Column2");
            }
            // Add dummy data rows if needed.
            return dt;
        }

        // Helper function that simulates checking if a field mapping is valid.
        private bool CheckFieldMapping(string databaseFields, string statusChk)
        {
            // Complete implementation
            // For simulation, return true.
            return true;
        }

        public static string CheckFieldsVis(string strField, string selectFileField)
        {
            try
            {
                bool flag = false;
                string[] selectFF = selectFileField.Split(',');
                foreach (string field in selectFF)
                {
                    string cleanedField = field.Trim().Replace("[", "").Replace("]", "").ToUpper();
                    if (!cleanedField.Contains("&")) // Ignore fields containing "&"
                    {
                        if (!strField.ToUpper().Contains(cleanedField))
                        {
                            return ("Excel File does not match on " + cleanedField + " parameter.");
                            //return false; // Exit function if a mismatch is found
                        }
                    }
                }
                return string.Empty; // All fields matched
            }
            catch (Exception ex)
            {
                return ("Error: " + ex.Message);
            }
        }

        private bool CheckFields(string StrField)
        {
            string[] StrFields;
            string[] StrFieldExcel;
            string str1 = "";
            if (str1 == "")
            {
                //pc.ShowAlert(this, "PLease Enter Fields Parameters into File" + Application.StartupPath + "FieldParameters");
                pc.ShowAlert(this, "PLease Enter Fields Parameters into File");

                return false;
            }
            else
            {
                StrFields = str1.Split(',');
                StrFieldExcel = StrField.Split(',');
                for (int i = 0; i <= StrFields.Length - 1; i++)
                {
                    if (StrFields[i] != StrFieldExcel[i])
                    {
                        pc.ShowAlert(this,"Excel File and Fiels Parameters are not Same.");
                        return false;
                    }
                }
            }
            return true;
        }

        // Helper function that simulates the Get_Comp_TextFilePath call.
        private void Get_Comp_TextFilePath(string comp)
        {
            // Complete implementation as needed.
            // For simulation, do nothing.
        }


        public static string GetCompTextFilePath(string companyName)
        {
            bool optGeneralChecked = true, optNPSChecked = false;
            Dictionary<string, string> fileMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ALL INSURANCE CO. (SERVICE)", "FieldParametersALL1_Comm.txt" },
            { "AEGON RELIGARE LIFE INSURANCE", "FieldParametersAEGON_comm.txt" },
            { "METLIFE INSURANCE COMPANY", "FieldParametersMET_comm.txt" },
            { "AVIVA LIFE INSURANCE CO. LTD", "FieldParametersAVIVA1_comm.txt" },
            { "BAJAJ ALLIANZ GENERAL INURANCE", "FieldParametersBAGI1.txt" },
            { "BAJAJ ALLIANZ LIFE INURANCE CO. LTD.", "FieldParametersBAJAJ1_Comm.txt" },
            { "BHARTI AXA", "FieldParametersBHTA1_Comm.txt" },
            { "BIRLA SUN LIFE INSURANCE CO. LTD.", "FieldParametersBIRLA1_Comm.txt" },
            { "CHOLAMANDALAM MS GENERAL INSURALCE CO.LTD.", "FieldParametersMSCHOLA1.txt" },
            { "HDFC CHUB GENERAL INSURANCE CO.LTD.", "FieldParametersHDFCCHUBB1.txt" },
            { "HDFC STANDARD LIFE INSURANCE CO. LTD", "FieldParametersHDFC1_Comm.txt" },
            { "ICICI Lombard General Insurance  LTD", "FieldParametersICICIL1.txt" },
            { "ICICI PRUDENTIAL LIFE INSURANCE CO. LTD", "FieldParametersICICI1_Comm.txt" },
            { "IFFCO-TOKIO GENERAL INSURANCE CO.LTD.", "FieldParametersITGI1.txt" },
            { "ING Vysaya Life Insurance", "FieldParametersINGVYS1_Comm.txt" },
            { "LIFE INSURANCE CORPORATION", "FieldParametersLIC11_Comm.txt" },
            { "MAX NEW YORK LIFE INSURANCE CO. LTD.", "FieldParametersMNYL1_Comm.txt" },
            { "NATIONAL INSURANCE COMPANY LTD", "FieldParametersNIC1_Comm.txt" },
            { "NEW INDIA ASSURANCE CO. LTD.", "FieldParametersNIAC1_COmm.txt" },
            { "KOTAK LIFE INSURANCE CO. LTD.", "FieldParametersKOTAK1_Comm.txt" },
            { "ORIENTAL INSURANCE CO. LTD", "FieldParametersOIC1_Comm.txt" },
            { "RELIANCE LIFE INSURANCE CO. LTD.", "FieldParametersRELLIFE1_Comm.txt" },
            { "RELIENCE GENERAL INSURANCE CO. LTD.", "FieldParametersRELGEN1_Comm.txt" },
            { "ROYAL SUNDARAM ALLIANCE INSURANCE CO.LTD.", "FieldParametersRSA1_Comm.txt" },
            { "SBI LIFE INSURANCE", "FieldParametersSBI1_Comm.txt" },
            { "STAR HEALTH", "FieldParameters5501_Comm.txt" },
            { "TATA AIG GENERAL INSURANCE CO.LTD.", "FieldParametersTATAAIG1.txt" },
            { "TATA-AIG LIFE INSURANCE CO. LTD", "FieldParametersTATA1_Comm.txt" },
            { "UNITED INDIA INSURANCE CO. LTD", "FieldParametersUIIC1_Comm.txt" }
        };

            if (fileMapping.TryGetValue(companyName, out string filePath))
            {
                return filePath;
            }
            else if (optGeneralChecked)
            {
                return "FieldParametersGIBHARTI_Comm.txt";
            }
            else if (optNPSChecked)
            {
                return "FieldParametersNPS.txt";
            }
            else
            {
                return "DefaultFilePath.txt";  // Optional: Default case if no match found
            }
        }
        // Helper function that simulates FieldsParametersName.
        private string FieldsParametersName_Old(string fileName)
        {
            // Complete implementation that returns mapping fields string.
            // For simulation, return a dummy mapping if fileName is not empty.
            if (string.IsNullOrEmpty(fileName))
                return "";
            // For example, return "dbField1#ExcelField1,dbField2#ExcelField2"
            return "dbField1#ExcelField1,dbField2#ExcelField2";
        }

        public static string FieldsParametersName(string txtFile)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "life", "insufld", "PolicyInfo", txtFile);

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return reader.ReadLine();  
                }
            }
            else
            {
                return "File not found!";
            }
        }

        // Helper function that simulates Exc_Clent_FldName.
        private string Exc_Clent_FldName()
        {
            // Complete implementation that returns a field name.
            // For simulation, return a dummy field name.
            return "ClientField";
        }

        // Helper function to simulate IsNull check.
        private bool IsNull(object value)
        {
            return (value == null || Convert.IsDBNull(value));
        }

        // Helper function to check if an object (expected a string) is a date string in the specified formats.
        private bool IsStringDate(object value)
        {
            if (IsNull(value))
                return false;
            string s = value.ToString();
            // Check conditions:
            // (Len(s)=10 and InStr(s, "/") = 3 and InStr(s, "/") = 6) or (Len(s)=9 and ... )
            if (s.Length == 10)
            {
                // In VB, InStr returns position starting at 1.
                // So we expect character at index 2 (position 3) and index 5 (position 6) to be '/'
                if (s.Length > 5 && s[2] == '/' && s[5] == '/')
                    return true;
            }
            else if (s.Length == 9)
            {
                if (s.Length > 4 && s[2] == '/' && s[4] == '/')
                    return true;
            }
            return false;
        }




     

        #endregion
    
    
    }
}