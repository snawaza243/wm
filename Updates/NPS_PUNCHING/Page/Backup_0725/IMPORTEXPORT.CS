using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WM.Controllers;
using System.Globalization;
using Oracle.ManagedDataAccess.Client; 
using System.Web.Configuration;

namespace WM.Tree
{
    public partial class ImportExportExcel : System.Web.UI.Page
    {
        PsmController pc = new PsmController();
        NpsTransactionPunchingController npsCon = new NpsTransactionPunchingController();

        #region GLOBAL VARIABLES
        public Label Label5;
        public string Glbloginid ;
        public DateTime ServerDateTime;
        public string nps_imp_flag;
        public string npm_imp_zComm;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
           string loginID = Session["LoginId"] as string;
            if (loginID == null)
            {
                pc.RedirectToWelcomePage();
            }
            else
            {
                Glbloginid = loginID;

                if (!IsPostBack)
                {
                    //string curremtNPS_IMP = Session["Nps_Importing_flag"] as string; // ECS, NON_ECS
                    //string curremtNPS_IMP_CZ = Session["VChkZeroCommission"] as string; // Y,N -- on non ecs

                    string npsImportingFlag = Request.QueryString["Nps_Importing_flag"];
                    string vChkZeroCommission = Request.QueryString["VChkZeroCommission"];

                    string imp_flag = null;
                    string zComm = null;
                   
                    ParseQueryStringNPS(out imp_flag, out zComm);

                    if (string.IsNullOrEmpty(imp_flag) || string.IsNullOrEmpty(zComm))
                    {
                        Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
                    }
                    else
                    {
                        NPS_Onlod(imp_flag, zComm);
                    }
                }
            }
        }
   

        public static void ParseQueryStringNPS(out string imp_flag, out string zComm)
        {
             
            var queryString = HttpContext.Current.Request.QueryString;

            imp_flag = queryString["Nps_Importing_flag"];
            zComm = queryString["VChkZeroCommission"];
        }


        public void NPS_Onlod(string imp_flag, string zComm)
        {
            #region SET COMPANY DDL DATA

            npsEcsDdlCompany.Items.Clear();
            string sql1 = ("select iss_name,iss_code from iss_master where iss_code='IS02520'");
            npsEcsDdlCompany.Items.Clear();

            DataTable dtSql1 = pc.ExecuteCurrentQuery(sql1);
            if (dtSql1.Rows.Count > 0)
            { 
                npsEcsDdlCompany.DataSource = dtSql1;
                npsEcsDdlCompany.DataTextField = "iss_name";
                npsEcsDdlCompany.DataValueField = "iss_code";
                npsEcsDdlCompany.DataBind();
                npsEcsDdlCompany.SelectedIndex = (npsEcsDdlCompany.Items.Count > 0) ? 0 : -1;
            }
            #endregion

            #region SET STATUS DDL DATA

            string sql2 = (" select status,status_cd from bajaj_status_master where status_Cd='A' or status_Cd='D' or status_Cd='B' order by status");
            DataTable dtSql2 = pc.ExecuteCurrentQuery(sql2);
            npExsDdlStatus.Items.Clear();
            if (dtSql2.Rows.Count > 0)
            {
                npExsDdlStatus.DataSource = dtSql2;
                npExsDdlStatus.DataTextField = "status";
                npExsDdlStatus.DataValueField = "status_cd";
                npExsDdlStatus.DataBind();
                npExsDdlStatus.SelectedIndex = 0;
                npExsDdlStatus.SelectedIndex = (npExsDdlStatus.Items.Count > 0) ?  0 : -1;
            }

            #endregion

            #region SET NPS IMPORTING FLAG FOR PAGE TITLE AND ZEROCOMM FOR GLOBAL VARIABLE 

            if (!string.IsNullOrEmpty(imp_flag))
            {
                nps_imp_flag = imp_flag;
                npm_imp_zComm = zComm;
                if (imp_flag == "ECS")
                {
                    lblNPSECSNonECSPage.Text = "NPS ECS Transaction Importing";
                }
                else if (imp_flag == "NON_ECS")
                {
                    lblNPSECSNonECSPage.Text = "NPS NON ECS Transaction Importing";
                }
            }
            else
            {
                
                Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
            }
            #endregion
        }


        #region DEFAULT BUTTON FUNCTIONS LIKE RESET, EXIT, AND PAGE INDEX CHANGING
        protected void NpsEcsReset_Click(object sender, EventArgs e)
        {
            Session["SheetData"] = null;
            npsEcsLblFileName.Text = "File Name: ";

            npsEcsDdlSheetlist.Items.Clear();

            npsEcsDdlSheetlist.SelectedIndex = (npsEcsDdlSheetlist.Items.Count > 0) ? 0 : -1;
            npsEcsDdlCompany.SelectedIndex = (npsEcsDdlCompany.Items.Count > 0) ? 0 : -1;
            npsEcsInputField.Text = string.Empty;
            npsEcsSuccessMessage.Text = string.Empty;

            GridView1.DataSource = null;
            GridView1.DataBind();
        }

        protected void NpsEcsExit_Click(object sender, EventArgs e) // BTNEXIT
        {

            try
            {
                string imp_flag = null;
                string zComm = null;

                ParseQueryStringNPS(out imp_flag, out zComm);
                if (!string.IsNullOrEmpty(imp_flag))
                {
                    Response.Redirect("~/Masters/NpsTransactionPunching.aspx", false);
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

        protected void PaginationGridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            DataTable currentSheetData = (DataTable)Session["SheetData"];
            SetCurrentSheetDataToGrid(currentSheetData);

        }

        #endregion


        #region EXCEL FILE UPLOAD AND SHOW SELECTED SHEET DATA IN GRIDVIEW
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SheetData"] = null;
                if (Request.Form[hfFileSelected.UniqueID] != "1")
                {
                    pc.ShowAlert(this, "Kindly choose a file before uploading!");
                    return;
                }

                if (NpsEcsFileInput.HasFile)
                {
                    // two type of action ECS and NON ECS
                    string currentNPS_IMP = Session["Nps_Importing_flag"] as string; // ECS, NON_ECS

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

                    try
                    {
                        NpsEcsFileInput.SaveAs(filePath);
                        Session["CurrentNpsEcsExcelFile"] = filePath;

                        LoadExcelSheets(filePath, npsEcsDdlSheetlist);
                        npsEcsLblFileName.Text = "Uploaded File: " + NpsEcsFileInput.FileName;
                        string curMsg = "File uploaded successfully";
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

        public static void LoadExcelSheets(string filePath, DropDownList sheetDropDown)
        {

            using (OleDbConnection conn = new OleDbConnection(GetExcelConnectionString_N(filePath)))
            {
                conn.Open();
                DataTable dtSheets = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                sheetDropDown.Items.Clear();
                sheetDropDown.Items.Add(new ListItem("Select Sheet", ""));

                foreach (DataRow row in dtSheets.Rows)
                {
                    string sheetName = row["TABLE_NAME"].ToString();
                    // Remove $ from sheet name that Excel adds
                    sheetName = sheetName.Replace("$", "").Replace("'", "");
                    sheetDropDown.Items.Add(new ListItem(sheetName, sheetName));
                }
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
                        SetCurrentSheetDataToGrid(currentSheetData);
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

        public void SetCurrentSheetDataToGrid(DataTable dt)
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


        #region IMPORT BUTTON CLICK EVENT

        public DataTable PrepareNpsDataTable(DataTable sourceDt)
        {
            // Create new DataTable with correct column structure
            DataTable targetDt = new DataTable();

            // Add all columns matching the database table structure
            targetDt.Columns.Add("REF_TRAN_CODE", typeof(string));
            targetDt.Columns.Add("TR_DATE", typeof(string));
            targetDt.Columns.Add("ECS_AMT", typeof(string));
            targetDt.Columns.Add("ECS_PERIOD", typeof(string));
            targetDt.Columns.Add("ECS_PAY_DT", typeof(string));
            targetDt.Columns.Add("ECS_TRAN_CODE", typeof(string));
            targetDt.Columns.Add("LOGGEDUSERID", typeof(string));
            targetDt.Columns.Add("TIMEST", typeof(string));
            targetDt.Columns.Add("MODIFY_USER", typeof(string));
            targetDt.Columns.Add("MODIFY_DATE", typeof(string));
            targetDt.Columns.Add("TPSL_TRANID", typeof(string));
            targetDt.Columns.Add("CONSUMER_CODE", typeof(string));
            targetDt.Columns.Add("IMPORT_DT", typeof(string));

            // Validate source DataTable has required columns
            if (!sourceDt.Columns.Contains("Consumer_Code") ||
                !sourceDt.Columns.Contains("Date") ||
                !sourceDt.Columns.Contains("Amount"))
            {
                throw new ArgumentException("Source DataTable must contain Consumer_Code, Date, and Amount columns");
            }

            // Populate the target DataTable
            foreach (DataRow sourceRow in sourceDt.Rows)
            {
                DataRow targetRow = targetDt.NewRow();

                // Map source columns to target columns
                targetRow["REF_TRAN_CODE"] = sourceRow["Consumer_Code"]?.ToString() ?? string.Empty;
                targetRow["TR_DATE"] = sourceRow["Date"]?.ToString() ?? string.Empty;
                targetRow["ECS_AMT"] = sourceRow["Amount"]?.ToString() ?? "0";

                // Set default values for other columns
                targetRow["ECS_PERIOD"] = string.Empty;
                targetRow["ECS_PAY_DT"] = string.Empty;
                targetRow["ECS_TRAN_CODE"] = string.Empty;
                targetRow["LOGGEDUSERID"] = pc.currentLoginID();
                targetRow["TIMEST"] = string.Empty;
                targetRow["MODIFY_USER"] = string.Empty;
                targetRow["MODIFY_DATE"] = string.Empty;
                targetRow["TPSL_TRANID"] = string.Empty;
                targetRow["CONSUMER_CODE"] = string.Empty;
                targetRow["IMPORT_DT"] = targetRow["IMPORT_DT"] = DateTime.Now.ToString("dd/MM/yyyy");

                targetDt.Rows.Add(targetRow);
            }

            return targetDt;
        }

        public bool BulkInsertToOracle(DataTable data, string destinationTable, out string error)
        {
            error = null;

            try
            {
                using (var conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    conn.Open();

                    using (var bulkCopy = new OracleBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = destinationTable;
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.BulkCopyTimeout = 300; // <-- Extend timeout to 5 mins

                        foreach (DataColumn col in data.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.NotifyAfter = 1000;
                        bulkCopy.OracleRowsCopied += (s, e) =>
                        {
                            Console.WriteLine($"{e.RowsCopied} rows copied.");
                        };

                        bulkCopy.WriteToServer(data);
                    }

                    conn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        
        protected void NpsEcsSubmit_Click(object sender, EventArgs e) // btnimport
        {
            try
            {
                if (pc.currentLoginID() == null)
                {
                    pc.RedirectToWelcomePage();
                }
                else
                {
                    string imp_flag = null;
                    string zComm = null;
                    ParseQueryStringNPS(out imp_flag, out zComm); // btnimport
                                                                  // imp_flag contiant ECS or NON_ECS

                    if (!new[] { "ECS", "NON_ECS" }.Contains(imp_flag))
                    {
                        pc.ShowAlert(this, "Invalid import flag.");
                        return;
                    }

                    DataTable excelData = (DataTable)Session["SheetData"];
                    string imp_type = imp_flag; // ECS. NON_ECS
                    string zero_comm = zComm; // Y,N -- on non ecs
                    string log_id = pc.currentLoginID();
                    string role_id = pc.currentRoleID();
                    string cur_comp = npsEcsDdlCompany.SelectedValue;
                    string cur_status = npExsDdlStatus.SelectedValue;

                    DataTable cleanNonEcsDt = PrepareNpsDataTable(excelData);
                    pc.CleanTable("PSM_NPS_NES_TBL_TEMP1", out _);

                    BulkInsertToOracle(cleanNonEcsDt, "PSM_NPS_NES_TBL_TEMP1", out string nsertMessage);

                    if (!string.IsNullOrEmpty(nsertMessage))
                    {
                        pc.ShowAlert(this, nsertMessage);
                        return;
                    }

                    npsCon.ProcessFinalNpsEcsBulkImport(imp_type, zero_comm, log_id, role_id, cur_comp, cur_status, out string retMsg);
                    if (!string.IsNullOrEmpty(retMsg))
                    {
                        pc.ShowAlert(this, retMsg);
                        return;
                    }
                    //Glbloginid = loginID;
                    //nen_pic_import_Click();
                }
            } 
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error: " + ex.Message); 
                return;
            }
        }

        #endregion


        #region IMPORT FUNCITON FOR NPS ECS & NON-ECS

        private void nen_pic_import_Click()
        {

            string[] tabCol = pc.GetTableColumns("nps_nonecs_tbl_imp").ToArray();

            if (tabCol.Length == 0)
            {
                //pc.ShowAlert(this, "Table columns not found.");
                //return;
            }

            bool Chk_OPENFORM = true;
            bool OptLife = true;
            bool OptGeneral = false;
            bool OptNPS = true;
            string filepath = string.Empty;
            string sheetName = string.Empty;

            DataTable sheetDataTable = (DataTable)Session["SheetData"];


            int sheetCount = npsEcsDdlSheetlist.Items.Count;
            if (sheetCount == 0)
            {
                pc.ShowAlert(this, "Please First Select Sheet");
                return;
            }

            else if (sheetCount > 0 && !string.IsNullOrEmpty(npsEcsDdlSheetlist.SelectedValue))
            {
                 filepath = "TxtFileName.Text"; //   'lblfile.Caption
                 sheetName = npsEcsDdlSheetlist.SelectedValue;

                string Nps_Importing_flag = Session["Nps_Importing_flag"]?.ToString() ?? null;


                if (!string.IsNullOrEmpty(Nps_Importing_flag) && Nps_Importing_flag == "ECS")
                {
                    nen_ImportECSTransaction(sheetDataTable); // importing NPS ECS transaction
                }
                else {
                    nen_ImportNonECSTransaction(sheetDataTable); // importing NPS NON-ECS transaction
                }

                if (OptLife)
                {
                   nen_OptLife_Click();
                }
                else if (OptGeneral)
                {
                    nen_GIAPPMAP(sheetDataTable);
                    pc.ShowAlert(this, "All Records Inserted Successfully");

                }
                else if (OptNPS)
                {
                    if (Nps_Importing_flag == "ECS")
                    {
                        nen_NPSECSIMP(sheetDataTable);
                    }
                    else
                    {
                        string VChkZeroCommission = Session["VChkZeroCommission"]  as string;
                        nen_NPSNONECSIMP(sheetDataTable, VChkZeroCommission);
                    }
                    pc.ShowAlert(this, "All Records Inserted Successfully");
                }
            }
            else
            {
                pc.ShowAlert(this, "Please First Select Sheet");
            }
        }

        public void nen_ImportECSTransaction(DataTable sheetDataTable)
        {
            //DataTable sheetDataTable = (DataTable)Session["SheetData"];

            string sql = string.Empty;
            string FileFields = string.Empty;
            string dataDaseField = string.Empty;
            string selectedFileField = string.Empty;
            string TxtFile_Name = string.Empty;
            string str1 = string.Empty;
                        
            string Status_Chk = npExsDdlStatus.SelectedValue.ToString();
            string Comp_Cd = npsEcsDdlCompany.SelectedValue;

            bool OptLife = false;
            bool OptGeneral = false;
            bool OptNPS = true;
            bool Chk_OPENFORM = true;  
            
            int Total_records = 0;
            int countNull = 0;
            int Already_Exist = 0;

            // Check if the DataTable has any columns and rows then replace .,/,- inot empty
            foreach (DataColumn column in sheetDataTable.Columns)
            {
                if (string.IsNullOrEmpty(column.ColumnName))
                {
                    break;
                }
                column.ColumnName = column.ColumnName.Replace(".", "")
                                                   .Replace("/", "")
                                                   .Replace("-", "");
            }

            // Check if the DataTable has any columns then get the column names with string.Join
            if (sheetDataTable.Columns.Count > 0)
            {
                FileFields = string.Join(",", sheetDataTable.Columns.Cast<DataColumn>()
                                                             .Select(col => col.ColumnName));
 
            }

            // set the file name on the basis of the company code for retrieving the mapping fields
            TxtFile_Name = Get_Comp_TextFilePath(Comp_Cd, OptGeneral, OptNPS); // using in nen_ImportTransaction function

            #region  Retriving mapping masked string on the bases of TxtFile_Name        
            str1 = FieldsParametersName(TxtFile_Name); // using in nen_ImportTransaction function

            if (string.IsNullOrWhiteSpace(str1))
            {
                Chk_OPENFORM = false;
                // Show message and exit
                pc.ShowAlert(this, "File Format Is Wrong");
                return;
            }
            #endregion

            #region spliting the mapping masked string into two selectedFileField, dataDaseField list  
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
            #endregion
            
            // 'RsImport.Close
            // 'If CheckFieldsVis(FileFields, selectedFileField) = False Then Exit Sub
            //if (!CheckFieldMapping(dataDaseField.ToUpper(), Status_Chk.ToUpper()))

            string dbTabCol = string.Join(", ", pc.GetTableColumns("nps_nonecs_tbl_imp").ToArray());
           
            //string checkFieldMapping = CheckFieldsVis(dbTabCol.ToUpper(), Status_Chk.ToUpper());
            if (!CheckFieldMapping(dbTabCol.ToUpper(), Status_Chk.ToUpper(), Comp_Cd, OptGeneral, OptNPS))
            {
                Chk_OPENFORM = false;
                return;
            }
            
            if (OptLife)
            {
                pc.ExecuteCurrentQueryMaster(" delete from Bajaj_PolicyInfo_Data ", out _, out _);

                // Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")

                //DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);

                Total_records = 0;
                // Do While Not RsImport.EOF
                foreach (DataRow row in sheetDataTable.Rows)
                {
                    sql = "";
                    string SqlChk = "";
                    string Xls_Fld = "";
                    SqlChk = " select * from Bajaj_PolicyInfo_Data where ";
                    // For i = 0 To UBound(delComma)
                    for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                    {
                        string[] delHash = delComma[i].Split('#');
                        // Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
                        Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");

                        //If InStr(1, Xls_Fld, "&") > 0 Then
                        if (Xls_Fld.IndexOf("&") >= 0)
                        {
                            Xls_Fld = Exc_Clent_FldName();
                        }
                        // Check the type of field from the Excel DataTable
                        if (sheetDataTable.Columns.Contains(Xls_Fld))
                        {
                            object fieldValue = row[Xls_Fld];
                            // Check if field is Date type (adDate = 7)
                            if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(DateTime))
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
                            else if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(string))
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
                    DataTable rschk = pc.ExecuteCurrentQueryMaster(SqlChk, out _, out _);
                    
                    // 'JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
                    // 'AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO
                    if (rschk.Rows.Count == 0)
                    {
                        countNull = 0;
                        Already_Exist = 0;
                        sql = "Insert into Bajaj_PolicyInfo_Data (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                        for (int i = 0; i < sheetDataTable.Columns.Count; i++)
                        {
                            string fieldName = sheetDataTable.Columns[i].ColumnName;
                            object fieldVal = row[fieldName];
                            if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                            {
                                string Value1 = fieldVal.ToString().Replace("'", "");
                                sql += "'" + Value1 + "',";
                            }
                            else
                            {
                                // Check if field is date type or string date format based on length checks
                                if (sheetDataTable.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
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
                                    sql += "'" + formattedDate + "',";
                                }
                                else
                                {
                                    sql += "'" + fieldVal.ToString().Trim() + "',";
                                }
                            }
                        }
                        sql += "'" + Comp_Cd + "','" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                        sql += ")";
                        sql = sql.Replace("''", "Null");
                        sql = newString(sql);

                        // Execute the insert query
                        DataTable insertRecord = pc.ExecuteCurrentQueryMaster(sql, out _, out _);
                        if (countNull >= 50)
                        {
                            break;
                        }
                        else
                        {
                            Already_Exist = Already_Exist + 1;
                        }
                    }

                    Total_records = Total_records + 1;
                    //Label5.Text = Total_records.ToString();
                }
            }
            
            else if (OptGeneral)
            {
                DataTable delTable = pc.ExecuteCurrentQueryMaster("delete from Bajaj_PolicyInfo_Data_gen", out _, out _);
                //DataTable RsImport = GetExcelDataTable(importFile, sheetName, selectedFileField);
                //DataTable RsImport = (DataTable)Session["SheetData"];

                Total_records = 0;
                foreach (DataRow row in sheetDataTable.Rows)
                {
                    sql = "";
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
                        if (sheetDataTable.Columns.Contains(Xls_Fld))
                        {
                            object fieldValue = row[Xls_Fld];
                            if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(DateTime))
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
                            else if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(string))
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
                    DataTable rschk = pc.ExecuteCurrentQueryMaster(SqlChk, out _, out _);
                    if (rschk.Rows.Count == 0)
                    {
                        countNull = 0;
                        Already_Exist = 0;
                        sql = "Insert into Bajaj_PolicyInfo_Data_gen (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                        for (int i = 0; i < sheetDataTable.Columns.Count; i++)
                        {
                            string fieldName = sheetDataTable.Columns[i].ColumnName;
                            object fieldVal = row[fieldName];
                            if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                            {
                                string Value1 = fieldVal.ToString().Replace("'", "");
                                sql += "'" + Value1 + "',";
                            }
                            else
                            {
                                if (sheetDataTable.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
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
                                    sql += "'" + formattedDate + "',";
                                }
                                else
                                {
                                    sql += "'" + fieldVal.ToString().Trim() + "',";
                                }
                            }
                        }
                        sql += "'" + Comp_Cd + "','" + ServerDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "','" + Glbloginid + "'";
                        sql += ")";
                        sql = sql.Replace("''", "Null");
                        sql = newString(sql);
                        DataTable insertOpGenerate = pc.ExecuteCurrentQueryMaster(sql, out _, out _);
                        if (countNull >= 50)
                        {
                            break;
                        }
                    }
                    else
                    {
                        Already_Exist = Already_Exist + 1;
                    }
                    Total_records = Total_records + 1;
                    
                    
                    //Label5.Text = Total_records.ToString();
                }
            }
           
            else if (OptNPS)
            {
                DataTable dtInsertExecut = pc.ExecuteCurrentQueryMaster("insert into nps_ecs_tbl_imp_bk select * from nps_ecs_tbl_imp", out _, out _);

                DataTable dtDel = pc.ExecuteCurrentQueryMaster("delete from nps_ecs_tbl_imp", out _, out _);

                //DataTable RsImport = (DataTable)Session["SheetData"];

                Total_records = 0;
                foreach (DataRow row in sheetDataTable.Rows)
                {
                    sql = "";
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
                        if (sheetDataTable.Columns.Contains(Xls_Fld))
                        {
                            object fieldValue = row[Xls_Fld];
                            if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(DateTime))
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
                            else if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(string))
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
                    DataTable rschk = pc.ExecuteCurrentQueryMaster(SqlChk, out int rnrschk, out string ierschk);

                    if (rschk.Rows.Count == 0)
                    {
                        countNull = 0;
                        Already_Exist = 0;
                        sql = "Insert into nps_ecs_tbl_imp (" + dataDaseField + ",Import_dt,LOGGEDUserID)  Values(";
                        for (int i = 0; i < sheetDataTable.Columns.Count; i++)
                        {
                            string fieldName = sheetDataTable.Columns[i].ColumnName;
                            if (selectedFileField.Contains(fieldName))
                            {
                                object fieldVal = row[fieldName];
                                string Value1 = "";
                                if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                                {
                                     Value1 = fieldVal.ToString().Replace("'", "");
                                    sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    if (sheetDataTable.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                                    {
                                        string formattedDate = fieldVal.ToString();
                                        if (!IsNull(fieldVal))
                                        {
                                            //TO_DATE('01/02/2021', 'DD/MM/YYYY')
                                            formattedDate = " TO_DATE('" + formattedDate + " ', 'DD/MM/YYYY') ";
                                        }
                                        sql += " " + formattedDate + " ,";
                                    }
                                    else
                                    {
                                        sql += "'" + fieldVal.ToString().Trim() + "',";
                                    }
                                }
                            }
                        }
                        sql += "sysdate, '" + Glbloginid + "'";

                        sql += ")";
                        //sql = sql.Replace("''", "Null");
                        sql = newString(sql);

                        DataTable insertOptNPS = pc.ExecuteCurrentQueryMaster(sql, out _, out _);
                        if (countNull >= 50)
                        { 
                            break; 
                        }
                    }
                    else
                    {
                        Already_Exist = Already_Exist + 1;
                    }
                    Total_records = Total_records + 1;
                    //Label5.Text = Total_records.ToString();
                }
            }

            pc.ExecuteCurrentQueryMaster("delete nps_ecs_tbl_imp where ref_tran_code is null", out _, out _);
            pc.ExecuteCurrentQueryMaster("commit", out _, out _);

            int lblVal;
            //if (int.TryParse(Label5.Text, out lblVal))
            //{
            //    Label5.Text = (lblVal - 1).ToString();
            //}
            pc.ShowAlert(this, " Out OF " + (Total_records - 1).ToString() + " Records " + Already_Exist.ToString() + " Are Already Exist ");
            return;
        }

 
        public void nen_ImportNonECSTransaction(DataTable sheetDataTable)
        {
            try
            {
                //DataTable sheetDataTable = (DataTable)Session["SheetData"];
                string sql = string.Empty;
                string FileFields = string.Empty;
                string dataDaseField = string.Empty;
                string selectedFileField = string.Empty;
                string TxtFile_Name = string.Empty;
                string str1 = string.Empty;

                string Status_Chk = npExsDdlStatus.SelectedValue.ToString();
                string Comp_Cd = npsEcsDdlCompany.SelectedValue;

                bool OptLife = false;
                bool OptGeneral = false;
                bool OptNPS = true;
                bool Chk_OPENFORM = true;

                int Total_records = 0;
                int countNull = 0;
                int Already_Exist = 0;

                foreach (DataColumn column in sheetDataTable.Columns)
                {
                    if (string.IsNullOrEmpty(column.ColumnName))
                    {
                        break;
                    }

                    column.ColumnName = column.ColumnName.Replace(".", "")
                                                       .Replace("/", "")
                                                       .Replace("-", "");
                }

                if (sheetDataTable.Columns.Count > 0)
                {
                    // Using string.Join is more efficient than concatenation in a loop
                    FileFields = string.Join(",", sheetDataTable.Columns.Cast<DataColumn>()
                                                                 .Select(col => col.ColumnName));

                    /* Alternative using StringBuilder (similar to original VB approach)
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < RsImport.Columns.Count; i++)
                    {
                        sb.Append(RsImport.Columns[i].ColumnName);
                        if (i < RsImport.Columns.Count - 1)
                            sb.Append(",");
                    }
                    FileFields = sb.ToString();
                    */
                }

                 
                TxtFile_Name = Get_Comp_TextFilePath(Comp_Cd, OptGeneral, OptNPS);

                // "FieldParametersNPS.txt"
                // "ref_tran_code#[Consumer_Code],tr_date#[date],ecs_amt#[Amount]"


                str1 = FieldsParametersName(TxtFile_Name); // Mapping fields are shown in str1

                if (string.IsNullOrWhiteSpace(str1))
                {
                    Chk_OPENFORM = false;
                    pc.ShowAlert(this, "File Format Is Wrong");
                    return;
                }

                string[] delComma = str1.Split(',');
                selectedFileField = "";
                dataDaseField = "";


                for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                {
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

                //string dbTabCol = string.Join(", ", pc.GetTableColumns("nps_nonecs_tbl_imp").ToArray());
                //string checkFieldMapping = CheckFieldsVis(dbTabCol.ToUpper(), Status_Chk.ToUpper());

                if (!CheckFieldMapping(dataDaseField, Status_Chk.ToUpper(), Comp_Cd, OptGeneral, OptNPS))
                {
                    Chk_OPENFORM = false;
                    return;
                }

                if (OptNPS)
                {
                    pc.ExecuteCurrentQueryMaster("insert into nps_nonecs_tbl_imp_bk select * from  nps_nonecs_tbl_imp", out _, out _);
                    DataTable dtDel = pc.ExecuteCurrentQueryMaster(" delete from  nps_nonecs_tbl_imp ", out _, out _);

                    Total_records = 0;
                    foreach (DataRow row in sheetDataTable.Rows)
                    {
                        sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from  nps_nonecs_tbl_imp where ";
                        for (int i = 0; i <= delComma.GetUpperBound(0); i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.IndexOf("&") >= 0)
                            {
                                Xls_Fld = Exc_Clent_FldName(); // need to check
                            }
                            if (sheetDataTable.Columns.Contains(Xls_Fld))
                            {
                                object fieldValue = row[Xls_Fld];
                                if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(DateTime))
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
                                else if (sheetDataTable.Columns[Xls_Fld].DataType == typeof(string))
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
                                SqlChk += " and ";
                            }
                        }
                        // SqlChk = SqlChk & " Company_cd='" & Comp_Cd & "' "
                        SqlChk += " 1=1 ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = pc.ExecuteCurrentQueryMaster(SqlChk, out int rnRschk, out string ieRschk);
                        // JIN FIELD SE MAPPING KI HAI UNHI FIELD SE CHECK KARTE HAI KI VO RECORD DATABASE KE ANDAR EXIST HAI YA NAHI
                        // AGAR NAHI HAI TO  Bajaj_PolicyInfo_Data KE ANDAR INSERT KARA DO

                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            sql = "Insert into  nps_nonecs_tbl_imp (" + dataDaseField + ",Import_dt,LOGGEDUserID)  Values(";
                            for (int i = 0; i < sheetDataTable.Columns.Count; i++)
                            {
                                string fieldName = sheetDataTable.Columns[i].ColumnName;

                                if (selectedFileField.Contains(fieldName))
                                {
                                    object fieldVal = row[fieldName];
                                    string Value1 = "";
                                    if (fieldVal != null && fieldVal.ToString().IndexOf("'") >= 0)
                                    {
                                         Value1 = fieldVal.ToString().Replace("'", "");
                                        sql += "'" + Value1 + "',";
                                    }
                                    else
                                    {
                                        if (sheetDataTable.Columns[fieldName].DataType == typeof(DateTime) || IsStringDate(fieldVal))
                                        {
                                            string formattedDate = fieldVal.ToString();
                                            if (!IsNull(fieldVal))
                                            {
                                                //TO_DATE('01/02/2021', 'DD/MM/YYYY')
                                                formattedDate = " TO_DATE('" + formattedDate + " ', 'DD/MM/YYYY') ";
                                            }
                                            sql += " " + formattedDate + " ,";
                                        }
                                        else
                                        {
                                            sql += "'" + fieldVal.ToString().Trim() + "',";
                                        }
                                    }
                                }
                            }
                            sql += "sysdate, '"  + Glbloginid + "'";
                            sql += ")";
                            //sql = sql.Replace("''", "Null");
                            sql = newString(sql);

                            
                            DataTable insertOptNPS = pc.ExecuteCurrentQueryMaster(sql, out int rninsertOptNPS, out string ieinsertOptNPS);

                            if (countNull >= 50)
                            {
                                break;
                            }
                            else
                            {
                                Already_Exist = Already_Exist + 1;
                            }
                            Total_records = Total_records + 1;
                            //Label5.Text = Total_records.ToString();
                        }
                    }
                }

                pc.ExecuteCurrentQueryMaster("delete nps_nonecs_tbl_imp where ref_tran_code is null", out _, out _);
                pc.ExecuteCurrentQueryMaster("commit", out _, out _);

                //int lblVal;
                //if (int.TryParse(Label5.Text, out lblVal))
                //{
                //    Label5.Text = (lblVal - 1).ToString();
                //}
                pc.ShowAlert(this, " Out OF " + (Total_records - 1).ToString() + " Records " +(Already_Exist != 0 ? Already_Exist.ToString() : null) + " Are Already Exist ");
                return;

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
                // Handle exception
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void nen_OptLife_Click()
        {
            bool OptLife = true;
            DataTable rsCompany = new DataTable();

            string sql = string.Empty;
            sql = "";
            sql = "select company_cd,company_name from bajaj_company_master where catagory='L' order by company_name";

            rsCompany = pc.ExecuteCurrentQueryMaster(sql, out int rc1, out string ie1);

            if (rc1 > 0 && string.IsNullOrEmpty(ie1))
            {
                // Loop through the DataTable rows
                foreach (DataRow row in rsCompany.Rows)
                {
                    string companyCode = row["company_cd"].ToString();
                    string companyName = row["company_name"].ToString();
                    // Do something with companyCode and companyName

                    npsEcsDdlCompany.DataSource = rsCompany;
                    npsEcsDdlCompany.DataTextField = "company_name";
                    npsEcsDdlCompany.DataValueField = "company_cd";
                    npsEcsDdlCompany.DataBind();
                    npsEcsDdlCompany.Items.Insert(0, new ListItem("Select Company", ""));
                }
            }
            else
            {
            }


        }


        public void nen_GIAPPMAP(DataTable excelSheetData)
        {
            DataTable Rs_GIApp = new DataTable();
            string comp_cd = npsEcsDdlCompany.SelectedValue;
            string sql = string.Empty;
            string MyApp = string.Empty;
            string sql1 = string.Empty;

            sql = "";

            sql = "select app_no,login_dt,policy_no from Bajaj_PolicyInfo_Data_gen ";

            // make varivabel for all slect fields wint empty value 
            string MyAR = "";
            string MyDt = "";
            string App_No = "";
            string Login_Dt = "";
            string Policy_No = "";



            Rs_GIApp = pc.ExecuteCurrentQueryMaster(sql, out int rc1, out string ie1);
            if (rc1 > 0 && string.IsNullOrEmpty(ie1))
            {
                // Loop through the DataTable rows
                foreach (DataRow row in Rs_GIApp.Rows)
                {
                    MyApp = row["app_no"].ToString();
                    //App_No = row["app_no"].ToString();
                    //Login_Dt = row["login_dt"].ToString();
                    //Policy_No = row["policy_no"].ToString();

                    if (!string.IsNullOrEmpty(MyApp))
                    {
                        sql = "select count(*) from bajaj_ar_head where company_cd= '" + comp_cd + "' and app_no='" + MyApp + "'";
                        pc.ExecuteCurrentQueryMaster(sql, out int rc2, out string ie2);

                        sql = "select count(*) from bajaj_ar_head where company_cd= '" + comp_cd + "' and ptype<>0 and app_no='" + MyApp + "'";
                        pc.ExecuteCurrentQueryMaster(sql, out int rc3, out string ie3);

                        if (rc2 > 0 && string.IsNullOrEmpty(ie2))
                        {

                        }

                        else if (rc3 == 1 && string.IsNullOrEmpty(ie3))
                        {
                            sql = "select sys_ar_no from bajaj_ar_head where company_cd= '" + comp_cd + "' and ptype<>0 and app_no='" + MyApp + "' ";
                            DataTable dt1 = pc.ExecuteCurrentQueryMaster(sql, out int rc4, out string ie4);
                            if (dt1.Rows.Count > 0)
                            {
                                MyAR = dt1.Rows[0]["sys_ar_no"].ToString();
                            }
                            else
                            {
                                MyAR = "";
                            }



                            sql = "select sys_ar_dt from bajaj_ar_head where company_cd= '" + comp_cd + "' and ptype<>0 and app_no='" + MyApp + "' ";

                            DataTable dt2 = pc.ExecuteCurrentQueryMaster(sql, out int rc5, out string ie5);
                            if (dt2.Rows.Count > 0)
                            {
                                MyDt = dt2.Rows[0]["sys_ar_dt"].ToString();
                            }
                            else
                            {
                                MyDt = "";
                            }

                            sql = "Insert into gicovernoteimport select * from bajaj_ar_head  where sys_ar_no='" + MyAR + "'";
                            pc.ExecuteCurrentQueryMaster(sql, out int rc6, out string ie6);

                            sql = "Update bajaj_ar_head set status_cd ='A', login_dt=to_date('" + row["login_dt"].ToString() + "','DD/MM/RRRR'),ply_issue_dt=add_months(to_date('" + row["login_dt"].ToString() + "','DD/MM/RRRR'),12)-1 , policy_no='" + row["policy_no"].ToString() + "',modified_user='AutoI' where company_cd= '" + comp_cd + "' and app_no='" + MyApp + "' and ptype<>0 and modified_user<>'AutoI'";
                            pc.ExecuteCurrentQueryMaster(sql, out int rc7, out string ie7);

                            sql = "";

                            sql = "insert into bajaj_ar_details (sys_ar_no,status_dt,status_cd,remarks,sys_ar_dt,userid,status_update_on,reason_cd) values";
                            sql += "('" + MyAR + "',to_date(sysdate,'DD/MM/RRRR'),'A','Imported data',";
                            sql += "to_date('" + MyDt + "','DD/MM/RRRR'),'AutoI',sysdate,'A001'";
                            sql += ")";

                            pc.ExecuteCurrentQueryMaster(sql, out int rc8, out string ie8);

                        }
                        else
                        {
                            MarkinExcel(MyApp, excelSheetData);
                        }


                    }
                    else
                    {
                        MyAR = MyApp;
                    }


                    MyDt = row["login_dt"].ToString();
                    if (!string.IsNullOrEmpty(MyAR))
                    {
                        // Check if the record exists in TRANSACTION_ST
                        string checkSql = $"select count(*) from TRANSACTION_ST where MANUAL_ARNO= '{MyAR}' and TO_DATE(TR_DATE,'DD/MM/RRRR')=TO_DATE('{MyDt}','DD/MM/RRRR')";
                        DataTable recordCount = pc.ExecuteCurrentQueryMaster(checkSql, out int rc2, out string ie2);
                        if (rc1 == 0)
                        {
                            // Call stored procedure
                            string procSql = "GIAPPMAP";

                            OracleParameter[] pram1 = new OracleParameter[] { new OracleParameter("CONSUMERCODE", OracleDbType.Varchar2) { Value = MyAR } };
                            DataTable dt_procSql = pc.ExecuteStoredProcedureMaster(procSql, pram1, out int rc3, out string ie3);
                        }
                        else
                        {
                            // Mark in Excel
                            MarkinExcel(MyAR, excelSheetData);
                        }
                    }
                }

            }
        }

        public void nen_NPSECSIMP(DataTable excelSheetData)
        {


            DataTable Rs_GIApp = new DataTable();
            string MyAR = string.Empty;
            string MyDt = string.Empty;
            string sql = string.Empty;
            string MyApp = string.Empty;

            sql = "";
            sql = "select REF_TRAN_CODE,TR_DATE,ECS_AMT,CONSUMER_CODE from NPS_ECS_TBL_IMP ";

            Rs_GIApp = pc.ExecuteCurrentQueryMaster(sql, out int rc1, out string ie1);

            if (rc1 > 0 && string.IsNullOrEmpty(ie1))
            {
                // Loop through the DataTable rows
                foreach (DataRow row in Rs_GIApp.Rows)
                {
                    MyApp = row["REF_TRAN_CODE"].ToString();
                    MyDt = row["TR_DATE"].ToString();
                    if (!string.IsNullOrEmpty(MyApp))
                    {
                        // Check if the record exists in TRANSACTION_ST
                        string checkSql = $"select count(*) from TRANSACTION_ST where MANUAL_ARNO= '{MyApp}' and TO_DATE(TR_DATE,'DD/MM/RRRR')=TO_DATE('{MyDt}','DD/MM/RRRR')";
                        DataTable recordCount = pc.ExecuteCurrentQueryMaster(checkSql, out int rc2, out string ie2);
                        if (rc2 == 0)
                        {
                            // Call stored procedure
                            string procSql = "NPS_ECSTRAN_GENERATE_IMP";

                            OracleParameter[] pram1 = new OracleParameter[] { new OracleParameter("CONSUMERCODE", OracleDbType.Varchar2) { Value = MyApp } };
                            DataTable dt_procSql = pc.ExecuteStoredProcedureMaster(procSql, pram1, out int rc3, out string ie3);
                        }
                        else
                        {
                            // Mark in Excel
                            MarkinExcel(MyApp, excelSheetData);
                        }
                    }
                }

            }





        }

        public void nen_NPSNONECSIMP(DataTable excelSheetData, string VChkZeroCommission)
        {
            DataTable Rs_GIApp = new DataTable();
            string MyAR = string.Empty;
            string MyDt = string.Empty;
            string sql = string.Empty;
            string MyApp = string.Empty;

            sql = "";
            sql = "select REF_TRAN_CODE,TR_DATE,ECS_AMT,CONSUMER_CODE from NPS_NONECS_TBL_IMP ";

            Rs_GIApp = pc.ExecuteCurrentQueryMaster(sql, out int rc1, out string ie1);

            if (rc1 > 0 && string.IsNullOrEmpty(ie1))
            {
                // Loop through the DataTable rows
                foreach (DataRow row in Rs_GIApp.Rows)
                {
                    MyApp = row["REF_TRAN_CODE"].ToString();
                    MyDt = row["TR_DATE"].ToString();
                    if (!string.IsNullOrEmpty(MyApp))
                    {
                        string checkSql = $"select count(*) from TRANSACTION_ST where MANUAL_ARNO= '{MyApp}' and TO_DATE(TR_DATE,'DD/MM/RRRR')=TO_DATE('{MyDt}','DD/MM/RRRR')";
                        DataTable recordCount = pc.ExecuteCurrentQueryMaster(checkSql, out int rc2, out string ie2);
                        if (rc2 == 0)
                        {
                            string procSql = "NPS_NONECSTRAN_GENERATE_IMP";

                            OracleParameter[] pram1 = new OracleParameter[] {
                                new OracleParameter("CONSUMERCODE", OracleDbType.Varchar2) { Value = MyApp },
                                new OracleParameter("PFLAG", OracleDbType.Varchar2) { Value = VChkZeroCommission }
                            };


                            DataTable dt_procSql = pc.ExecuteStoredProcedureMaster(procSql, pram1, out int rc3, out string ie3);
                        }
                        else
                        {
                            // Mark in Excel
                            MarkinExcel(MyApp, excelSheetData);
                        }
                    }
                }

            }
        }


        private string Get_Comp_TextFilePath(string companyName, bool isGeneralInsurance, bool isNPS)
        {
            NpsTransactionPunchingController cont = new NpsTransactionPunchingController();

            return cont.GetCompanyTextFilePath(companyName, isGeneralInsurance, isNPS);
            /*
            string upperName = companyName.ToUpper();

            if (upperName == "ALL INSURANCE CO. (SERVICE)") return "FieldParametersALL1_Comm.txt";
            if (upperName == "AEGON RELIGARE LIFE INSURANCE") return "FieldParametersAEGON_comm.txt";
            if (upperName == "METLIFE INSURANCE COMPANY") return "FieldParametersMET_comm.txt";
            if (upperName == "AVIVA LIFE INSURANCE CO. LTD") return "FieldParametersAVIVA1_comm.txt";
            if (upperName == "BAJAJ ALLIANZ GENERAL INURANCE") return "FieldParametersBAGI1.txt";
            if (upperName == "BAJAJ ALLIANZ LIFE INURANCE CO. LTD.") return "FieldParametersBAJAJ1_Comm.txt";
            if (upperName == "BHARTI AXA") return "FieldParametersBHTA1_Comm.txt";
            if (upperName == "BIRLA SUN LIFE INSURANCE CO. LTD.") return "FieldParametersBIRLA1_Comm.txt";
            if (upperName == "CHOLAMANDALAM MS GENERAL INSURALCE CO.LTD.") return "FieldParametersMSCHOLA1.txt";
            if (upperName == "HDFC CHUB GENERAL INSURANCE CO.LTD.") return "FieldParametersHDFCCHUBB1.txt";
            if (upperName == "HDFC STANDARD LIFE INSURANCE CO. LTD") return "FieldParametersHDFC1_Comm.txt";
            if (upperName == "ICICI LOMBARD GENERAL INSURANCE LTD") return "FieldParametersICICIL1.txt";
            if (upperName == "ICICI PRUDENTIAL LIFE INSURANCE CO. LTD") return "FieldParametersICICI1_Comm.txt";
            if (upperName == "IFFCO-TOKIO GENERAL INSURANCE CO.LTD.") return "FieldParametersITGI1.txt";
            if (upperName == "ING VYSAYA LIFE INSURANCE") return "FieldParametersINGVYS1_Comm.txt";
            if (upperName == "LIFE INSURANCE CORPORATION") return "FieldParametersLIC11_Comm.txt";
            if (upperName == "MAX NEW YORK LIFE INSURANCE CO. LTD.") return "FieldParametersMNYL1_Comm.txt";
            if (upperName == "NATIONAL INSURANCE COMPANY LTD") return "FieldParametersNIC1_Comm.txt";
            if (upperName == "NEW INDIA ASSURANCE CO. LTD.") return "FieldParametersNIAC1_COmm.txt";
            if (upperName == "KOTAK LIFE INSURANCE CO. LTD.") return "FieldParametersKOTAK1_Comm.txt";
            if (upperName == "ORIENTAL INSURANCE CO. LTD") return "FieldParametersOIC1_Comm.txt";
            if (upperName == "RELIANCE LIFE INSURANCE CO. LTD.") return "FieldParametersRELLIFE1_Comm.txt";
            if (upperName == "RELIENCE GENERAL INSURANCE CO. LTD.") return "FieldParametersRELGEN1_Comm.txt";
            if (upperName == "ROYAL SUNDARAM ALLIANCE INSURANCE CO.LTD.") return "FieldParametersRSA1_Comm.txt";
            if (upperName == "SBI LIFE INSURANCE") return "FieldParametersSBI1_Comm.txt";
            if (upperName == "STAR HEALTH") return "FieldParameters5501_Comm.txt";
            if (upperName == "TATA AIG GENERAL INSURANCE CO.LTD.") return "FieldParametersTATAAIG1.txt";
            if (upperName == "TATA-AIG LIFE INSURANCE CO. LTD") return "FieldParametersTATA1_Comm.txt";
            if (upperName == "UNITED INDIA INSURANCE CO. LTD") return "FieldParametersUIIC1_Comm.txt";

            if (isGeneralInsurance) return "FieldParametersGIBHARTI_Comm.txt";
            if (isNPS) return "FieldParametersNPS.txt";
            throw new ArgumentException($"");
            */
        }


        public string FieldsParametersName_sasa(string txtFile)
        {
            string filePath = Path.Combine("life\\insufld\\PolicyInfo", txtFile);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} was not found.");
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                return reader.ReadLine();
            }
        }

        public void MarkinExcel(string MyApp, DataTable excelSheetData)
        {
            if (excelSheetData == null || excelSheetData.Rows.Count == 0)
                return;

            // Find column indexes
            int remarkColIndex = -1; // MyCol = j
            int consumerCodeColIndex = -1; //  MyColApp = j


            // Check header row (first row) for column names
            DataRow headerRow = excelSheetData.Rows[0];
            for (int j = 0; j < excelSheetData.Columns.Count; j++)
            {
                string columnName = headerRow[j].ToString().Trim().ToUpper();

                if (columnName == "REMARK")
                {
                    remarkColIndex = j;
                }
                else if (columnName == "CONSUMER_CODE")
                {
                    consumerCodeColIndex = j;
                }
            }

            // If we didn't find required columns, return
            if (remarkColIndex == -1 || consumerCodeColIndex == -1)
                return;

            // Process each data row (skip header row if it's in the DataTable)
            for (int i = 1; i < excelSheetData.Rows.Count; i++)
            {
                DataRow row = excelSheetData.Rows[i];

                // Skip empty rows
                if (row.ItemArray.All(item => item == null || string.IsNullOrEmpty(item.ToString())))
                    continue;

                string consumerCode = row[consumerCodeColIndex].ToString().Trim();

                if (consumerCode.Equals(MyApp, StringComparison.OrdinalIgnoreCase))
                {
                    row[remarkColIndex] = "Not Inserted";
                }
            }

            
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


        // Helper function that simulates checking if a field mapping is valid.
        private bool CheckFieldMapping(string data_Base_Fld, string statusCode, string Comp_Cd, bool OptGeneral, bool OptNPS)
        {
            bool result = false;
            string upperDataField = data_Base_Fld.ToUpper();

            if (Comp_Cd == "D" || Comp_Cd == "A" || Comp_Cd == "B")
            {
                if (!upperDataField.Contains("STATUS"))
                {
                    pc.ShowAlert(this, "Status is Not Mapped");
                    return result;
                }
                if (!upperDataField.Contains("REASON"))
                {
                    pc.ShowAlert(this, "Reason is Not Mapped");
                    return result;
                }
                if (Comp_Cd == "A")
                {
                    if (!upperDataField.Contains("POLICY_ISSUE_DT"))
                    {
                        pc.ShowAlert(this, "POLICY ISSUE DATE is Not Mapped");
                        return result;
                    }
                    if (!upperDataField.Contains("DOC"))
                    {
                        pc.ShowAlert(this, "DOC is Not Mapped");
                        return result;
                    }
                }
                else if (Comp_Cd == "D")
                {
                    if (!upperDataField.Contains("LOGIN_DT"))
                    {
                        pc.ShowAlert(this, "LoginDate is Not Mapped");
                        return result;
                    }
                }
            }

            if (Comp_Cd == "OM KOTAK" || Comp_Cd == "SBI LIFE")
            {
                if (!upperDataField.Contains("POLICY_NO"))
                {
                    pc.ShowAlert(this, "POLICY_NO is Not Mapped");
                    return result;
                }
                // Commented out as per original VB code
                // if (!upperDataField.Contains("CHEQUE_NO"))
                // {
                //     MessageBox.Show("Cheque_No is Not Mapped", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //     return result;
                // }
                if (!upperDataField.Contains("PREMIUM"))
                {
                    pc.ShowAlert(this, "PREMIUM is Not Mapped");
                    return result;
                }
            }
            else if (Comp_Cd == "TATAAIG")
            {
                if (!upperDataField.Contains("POLICY_NO"))
                {
                    pc.ShowAlert(this, "POLICY_NO is Not Mapped");
                    return result;
                }
            }
            else if (Comp_Cd == "RELLIFE" || Comp_Cd == "ICICI" || Comp_Cd == "AVIVA" ||
                     Comp_Cd == "BIRLA SUN" || Comp_Cd == "BHT A")
            {
                if (!upperDataField.Contains("POLICY_NO"))
                {
                    pc.ShowAlert(this, "POLICY_NO is Not Mapped");
                    return result;
                }
                if (!upperDataField.Contains("APP_NO"))
                {
                    pc.ShowAlert(this, "Application No is Not Mapped");
                    return result;
                }
            }
            else if (Comp_Cd == "BAJAJ A")
            {
                if (!upperDataField.Contains("POLICY_NO"))
                {
                    pc.ShowAlert(this, "POLICY_NO is Not Mapped");
                    return result;
                }
                // Commented out as per original VB code
                // if (!upperDataField.Contains("CHEQUE_NO"))
                // {
                //     MessageBox.Show("Cheque_No is Not Mapped", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //     return result;
                // }
                if (!upperDataField.Contains("APP_NO"))
                {
                    pc.ShowAlert(this, "Application No is Not Mapped");
                    return result;
                }
            }
            // Mayank GI status update
            else if (OptGeneral)
            {
                if (!upperDataField.Contains("POLICY_NO"))
                {
                    pc.ShowAlert(this, "POLICY_NO is Not Mapped");
                    return result;
                }
                if (!upperDataField.Contains("LOGIN_DT"))
                {
                    pc.ShowAlert(this, "Policy Issue Dt is Not Mapped");
                    return result;
                }
                if (!upperDataField.Contains("APP_NO"))
                {
                    pc.ShowAlert(this, "Application No is Not Mapped");
                    return result;
                }
            }
            else if (OptNPS)
            {
                if (!upperDataField.Contains("REF_TRAN_CODE"))
                {
                    pc.ShowAlert(this, "Consumer Code is Not Mapped");
                    return result;
                }
                if (!upperDataField.Contains("TR_DATE"))
                {
                    pc.ShowAlert(this, "Transaction Date is Not Mapped");
                    return result;
                }
                if (!upperDataField.Contains("ECS_AMT"))
                {
                    pc.ShowAlert(this, "Amount is Not Mapped");
                    return result;
                }
            }

            result = true;
            return result;
        }



        public static string FieldsParametersName(string txtFile)
        {
            // Combine base directory with the relative path
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string relativeDirectory = Path.Combine("SampleFile", "Life", "InsuFld", "PolicyInfo");
            string fullDirectoryPath = Path.Combine(baseDirectory, relativeDirectory);

            // Ensure directory exists
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            // Combine with the file name to get the full file path
            string fullFilePath = Path.Combine(fullDirectoryPath, txtFile);

            // Normalize the path
            fullFilePath = Path.GetFullPath(fullFilePath);

            // Check if file exists
            if (!File.Exists(fullFilePath))
            {
                return null;
            }

            try
            {
                // Read and return the first line of the file
                using (StreamReader reader = new StreamReader(fullFilePath))
                {
                    return reader.ReadLine();
                }
            }
            catch (Exception)
            {
                // You can log the exception here if needed
                return null;
            }
        }


        // Helper function that simulates Exc_Clent_FldName.
        private string Exc_Clent_FldName()
        {
            // Complete implementation that returns a field name.
            // For simulation, return a dummy field name.
            return "";
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