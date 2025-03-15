using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;

namespace MyWebApp
{
    public class TransactionImporter
    {
        // Global variables and simulated UI controls
        private string Status_Chk;
        private string Comp_Cd;
        // importFile will hold the full path to the Excel file; assumed to be set externally
        private string importFile;
        // sheetName used in Excel queries; assumed to be set externally or defaulted here
        private string sheetName = "Sheet1";
        // Simulated UI controls (for web conversion you may substitute with your own implementations)
        public ComboBox CmbStatus;
        public ComboBox cmbCompany;
        public Label Label5;
        public RadioButton OptLife;
        public RadioButton OptGeneral;
        public RadioButton OptNPS;
        public TextBox TxtFile_Name;
        // Other global variables
        private int Total_records = 0;
        private int Already_Exist = 0;
        private int countNull = 0;
        private DateTime ServerDateTime = DateTime.Now;
        private string Glbloginid = "User";

        // Main method translated from the original VB.NET ImportTransaction subroutine.
        public void ImportTransaction()
        {
            try
            {
                // -------------------- Begin VB Code Translation --------------------
                // Dim Rsunmatch As New ADODB.Recordset
                // (Recordset Rsunmatch is not used further in the logic)

                string Sql = "";
                string FileFields = "";
                string dataDaseField = "";
                string selectedFileField = "";
                string str1 = "";

                // VB.NET:
                // Status_Chk = Mid(CmbStatus.List(CmbStatus.ListIndex), InStr(1, CmbStatus.List(CmbStatus.ListIndex), "#") + 1, Len(CmbStatus.List(CmbStatus.ListIndex)) - InStr(1, CmbStatus.List(CmbStatus.ListIndex), "#"))
                string cmbStatusItem = CmbStatus.Items[CmbStatus.SelectedIndex].ToString();
                int pos = cmbStatusItem.IndexOf("#");
                if (pos >= 0)
                {
                    Status_Chk = cmbStatusItem.Substring(pos + 1);
                }
                else
                {
                    Status_Chk = "";
                }

                // VB.NET:
                // Split_Company = Split(cmbCompany.Text, "#")
                // Comp_Cd = Trim(Split_Company(1))
                string[] Split_Company = cmbCompany.Text.Split('#');
                if (Split_Company.Length > 1)
                    Comp_Cd = Split_Company[1].Trim();
                else
                    Comp_Cd = "";

                // VB.NET:
                // Set importExcelcon = New ADODB.Connection
                // importFile = filepath
                // (Assume importFile is already set externally)

                // -------------------- Excel Header Modification using Interop --------------------
                // Create an Excel application instance
                Application XL = new Application();
                Workbook XLW = XL.Workbooks.Open(importFile);
                int i = 1;
                Worksheet ws = (Worksheet)XLW.Worksheets[sheetName];
                // While XLW.Worksheets(sheetName).Cells(1, i) <> ""
                while (ws.Cells[1, i].Value2 != null && ws.Cells[1, i].Value2.ToString() != "")
                {
                    // Remove unwanted characters from the header cell
                    ws.Cells[1, i].Value2 = ws.Cells[1, i].Value2.ToString().Replace(".", "");
                    ws.Cells[1, i].Value2 = ws.Cells[1, i].Value2.ToString().Replace("/", "");
                    ws.Cells[1, i].Value2 = ws.Cells[1, i].Value2.ToString().Replace("-", "");
                    i++;
                }
                XLW.Save();
                XLW.Close(false);
                Marshal.ReleaseComObject(XLW);
                XL.Quit();
                Marshal.ReleaseComObject(XL);

                // Reopen workbook and immediately close it as in VB code
                Application XL2 = new Application();
                Workbook XLW2 = XL2.Workbooks.Open(importFile);
                XLW2.Close(false);
                Marshal.ReleaseComObject(XLW2);
                XL2.Quit();
                Marshal.ReleaseComObject(XL2);

                // -------------------- Open Excel File via OleDb connection --------------------
                OleDbConnection importExcelcon = new OleDbConnection();
                importExcelcon.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + importFile + ";Extended Properties=\"Excel 8.0;HDR=Yes;\"";
                importExcelcon.Open();

                // In VB: RsImport is an ADODB.Recordset. In C# we use a DataTable.
                DataTable RsImport = GetExcelData("Select * from [" + sheetName + "$]", importExcelcon);

                // VB.NET:
                // If RsImport.Fields.count > 0 Then
                //    FileFields = ""
                //    For i = 0 To RsImport.Fields.count - 1
                //        FileFields = FileFields & RsImport.Fields(i).Name & ","
                //    Next
                //    FileFields = Left(FileFields, Len(FileFields) - 1)
                // End If
                if (RsImport.Columns.Count > 0)
                {
                    FileFields = "";
                    for (i = 0; i < RsImport.Columns.Count; i++)
                    {
                        FileFields += RsImport.Columns[i].ColumnName + ",";
                    }
                    FileFields = FileFields.Substring(0, FileFields.Length - 1);
                }

                // VB.NET:
                // Call Get_Comp_TextFilePath(Trim(UCase(Split_Company(0)))
                Get_Comp_TextFilePath(Split_Company[0].Trim().ToUpper());

                // VB.NET:
                // str1 = FieldsParametersName(TxtFile_Name) 'Maping Fileds Are Shown In This Str1
                str1 = FieldsParametersName(TxtFile_Name.Text);
                if (str1.Trim() == "")
                {
                    // Chk_OPENFORM = False
                    // Clean up and show error message
                    RsImport = null;
                    importExcelcon.Close();
                    importExcelcon.Dispose();
                    MessageBox.Show("File Format Is Wrong", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // VB.NET:
                // delComma = Split(str1, ","): selectedFileField = "": dataDaseField = ""
                string[] delComma = str1.Split(',');
                selectedFileField = "";
                dataDaseField = "";
                for (i = 0; i < delComma.Length; i++)
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
                    dataDaseField = dataDaseField.Substring(0, dataDaseField.Length - 1);
                }

                // VB.NET commented out: 'If CheckFieldsVis(FileFields, selectedFileField) = False Then Exit Sub

                // VB.NET:
                // If CheckFieldMapping(UCase(dataDaseField), UCase(Status_Chk)) = False Then Chk_OPENFORM = False: Exit Sub
                if (!CheckFieldMapping(dataDaseField.ToUpper(), Status_Chk.ToUpper()))
                {
                    return;
                }

                // -------------------- Option Handling --------------------
                if (OptLife.Checked)
                {
                    // VB.NET:
                    // MyConn.Execute " delete from  Bajaj_PolicyInfo_Data "
                    MyConnExecute("delete from Bajaj_PolicyInfo_Data");
                    // VB.NET:
                    // Set RsImport = importExcelcon.Execute("Select " & selectedFileField & " from [" & sheetName & "$] ")
                    RsImport = GetExcelData("Select " + selectedFileField + " from [" + sheetName + "$]", importExcelcon);
                    Total_records = 0;
                    foreach (DataRow row in RsImport.Rows)
                    {
                        Sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from Bajaj_PolicyInfo_Data where ";
                        // Loop for each mapping field
                        for (i = 0; i < delComma.Length; i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            if (delHash.Length < 2)
                                continue;
                            // Xls_Fld = Replace(Replace(Replace(delHash(1), "[", ""), "]", ""), "'", "")
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.Contains("&"))
                            {
                                Xls_Fld = Exc_Clent_FldName();
                            }
                            object fieldValue = row[Xls_Fld];
                            if (fieldValue is DateTime)
                            {
                                string formatted = ((DateTime)fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Replace("'", "");
                                SqlChk += delHash[0] + "='" + formatted + "' ";
                            }
                            else if (fieldValue is string && ((string)fieldValue).Length == 10 &&
                                ((string)fieldValue).IndexOf("/") == 3 && ((string)fieldValue).IndexOf("/", 4) == 6)
                            {
                                if (string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    Xls_Fld = "";
                                    SqlChk += "(" + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    DateTime tempDate;
                                    if (DateTime.TryParse(Xls_Fld, out tempDate))
                                        SqlChk += delHash[0] + "='" + tempDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                    else
                                        SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                            }
                            else if (fieldValue is string && ((string)fieldValue).Length == 10 &&
                                ((string)fieldValue).IndexOf("/") == 3 && ((string)fieldValue).IndexOf("/", 4) == 6)
                            {
                                if (string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    Xls_Fld = "";
                                    SqlChk += " (" + delHash[0] + "='" + fieldValue.ToString().ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                            }
                            else
                            {
                                if (fieldValue == null || fieldValue == DBNull.Value)
                                {
                                    Xls_Fld = "0";
                                    SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    SqlChk += delHash[0] + "=" + string.Format("{0:######0}", Convert.ToDecimal(Xls_Fld)) + " ";
                                }
                            }
                            SqlChk += " and ";
                        }
                        SqlChk += " Company_cd='" + Comp_Cd + "' ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = MyConnExecuteQuery(SqlChk);
                        // If record does not exist then insert
                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            Sql = "Insert into Bajaj_PolicyInfo_Data (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                            for (i = 0; i < RsImport.Columns.Count; i++)
                            {
                                object val = row[i];
                                if (val != null && val.ToString().Contains("'"))
                                {
                                    string Value1 = val.ToString().Replace("'", "");
                                    Sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    if (val is DateTime || (val is string && IsLikelyDate(val.ToString())))
                                    {
                                        DateTime dt;
                                        if (val is DateTime)
                                            dt = (DateTime)val;
                                        else
                                            DateTime.TryParse(val.ToString(), out dt);
                                        Sql += "'" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim() + "',";
                                    }
                                    else
                                    {
                                        Sql += "'" + val.ToString().Trim() + "',";
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
                            Already_Exist++;
                        }
                        Total_records++;
                        Label5.Text = Total_records.ToString();
                        Application.DoEvents();
                    }
                }
                else if (OptGeneral.Checked)
                {
                    MyConnExecute("delete from Bajaj_PolicyInfo_Data_gen");
                    RsImport = GetExcelData("Select " + selectedFileField + " from [" + sheetName + "$]", importExcelcon);
                    Total_records = 0;
                    foreach (DataRow row in RsImport.Rows)
                    {
                        Sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from Bajaj_PolicyInfo_Data_gen where ";
                        for (i = 0; i < delComma.Length; i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            if (delHash.Length < 2)
                                continue;
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.Contains("&"))
                            {
                                Xls_Fld = Exc_Clent_FldName();
                            }
                            object fieldValue = row[Xls_Fld];
                            if (fieldValue is DateTime)
                            {
                                string formatted = ((DateTime)fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Replace("'", "");
                                SqlChk += delHash[0] + "='" + formatted + "' ";
                            }
                            else if (fieldValue is string && ((string)fieldValue).Length == 10 &&
                                ((string)fieldValue).IndexOf("/") == 3 && ((string)fieldValue).IndexOf("/", 4) == 6)
                            {
                                if (string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    Xls_Fld = "";
                                    SqlChk += "(" + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    DateTime tempDate;
                                    if (DateTime.TryParse(Xls_Fld, out tempDate))
                                        SqlChk += delHash[0] + "='" + tempDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                    else
                                        SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                            }
                            else if (fieldValue is string && ((string)fieldValue).Length == 10 &&
                                ((string)fieldValue).IndexOf("/") == 3 && ((string)fieldValue).IndexOf("/", 4) == 6)
                            {
                                if (string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    Xls_Fld = "";
                                    SqlChk += " (" + delHash[0] + "='" + fieldValue.ToString().ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                            }
                            else
                            {
                                if (fieldValue == null || fieldValue == DBNull.Value)
                                {
                                    Xls_Fld = "0";
                                    SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    SqlChk += delHash[0] + "=" + string.Format("{0:######0}", Convert.ToDecimal(Xls_Fld)) + " ";
                                }
                            }
                            SqlChk += " and ";
                        }
                        SqlChk += " Company_cd='" + Comp_Cd + "' ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = MyConnExecuteQuery(SqlChk);
                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            Sql = "Insert into Bajaj_PolicyInfo_Data_gen (" + dataDaseField + ",Company_cd,Import_dt,UserID)  Values(";
                            for (i = 0; i < RsImport.Columns.Count; i++)
                            {
                                object val = row[i];
                                if (val != null && val.ToString().Contains("'"))
                                {
                                    string Value1 = val.ToString().Replace("'", "");
                                    Sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    if (val is DateTime || (val is string && IsLikelyDate(val.ToString())))
                                    {
                                        DateTime dt;
                                        if (val is DateTime)
                                            dt = (DateTime)val;
                                        else
                                            DateTime.TryParse(val.ToString(), out dt);
                                        Sql += "'" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim() + "',";
                                    }
                                    else
                                    {
                                        Sql += "'" + val.ToString().Trim() + "',";
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
                            Already_Exist++;
                        }
                        Total_records++;
                        Label5.Text = Total_records.ToString();
                        Application.DoEvents();
                    }
                }
                else if (OptNPS.Checked)
                {
                    MyConnExecute("insert into nps_ecs_tbl_imp_bk select * from nps_ecs_tbl_imp");
                    MyConnExecute("delete from nps_ecs_tbl_imp");
                    if (RsImport != null)
                        RsImport.Dispose();
                    RsImport = GetExcelData("Select " + selectedFileField + " from [" + sheetName + "$]", importExcelcon);
                    Total_records = 0;
                    foreach (DataRow row in RsImport.Rows)
                    {
                        Sql = "";
                        string SqlChk = "";
                        string Xls_Fld = "";
                        SqlChk = " select * from nps_ecs_tbl_imp where ";
                        for (i = 0; i < delComma.Length; i++)
                        {
                            string[] delHash = delComma[i].Split('#');
                            if (delHash.Length < 2)
                                continue;
                            Xls_Fld = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");
                            if (Xls_Fld.Contains("&"))
                            {
                                Xls_Fld = Exc_Clent_FldName();
                            }
                            object fieldValue = row[Xls_Fld];
                            if (fieldValue is DateTime)
                            {
                                string formatted = ((DateTime)fieldValue).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Replace("'", "");
                                SqlChk += delHash[0] + "='" + formatted + "' ";
                            }
                            else if (fieldValue is string && ((string)fieldValue).Length == 10 &&
                                ((string)fieldValue).IndexOf("/") == 3 && ((string)fieldValue).IndexOf("/", 4) == 6)
                            {
                                if (string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    Xls_Fld = "";
                                    SqlChk += "(" + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    DateTime tempDate;
                                    if (DateTime.TryParse(Xls_Fld, out tempDate))
                                        SqlChk += delHash[0] + "='" + tempDate.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "' ";
                                    else
                                        SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                            }
                            else if (fieldValue is string && ((string)fieldValue).Length == 10 &&
                                ((string)fieldValue).IndexOf("/") == 3 && ((string)fieldValue).IndexOf("/", 4) == 6)
                            {
                                if (string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    Xls_Fld = "";
                                    SqlChk += " (" + delHash[0] + "='" + fieldValue.ToString().ToUpper().Trim() + "' or " + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    SqlChk += delHash[0] + "='" + Xls_Fld + "' ";
                                }
                            }
                            else
                            {
                                if (fieldValue == null || fieldValue == DBNull.Value)
                                {
                                    Xls_Fld = "0";
                                    SqlChk += "(" + delHash[0] + "=" + Xls_Fld + " or " + delHash[0] + " is null  )  ";
                                }
                                else
                                {
                                    Xls_Fld = fieldValue.ToString().Replace("'", "");
                                    SqlChk += delHash[0] + "=" + string.Format("{0:######0}", Convert.ToDecimal(Xls_Fld)) + " ";
                                }
                            }
                            SqlChk += " and ";
                        }
                        SqlChk += " 1=1 ";
                        SqlChk = newString(SqlChk);
                        DataTable rschk = MyConnExecuteQuery(SqlChk);
                        if (rschk.Rows.Count == 0)
                        {
                            countNull = 0;
                            Already_Exist = 0;
                            Sql = "Insert into nps_ecs_tbl_imp (" + dataDaseField + ",Import_dt,LOGGEDUserID)  Values(";
                            for (i = 0; i < RsImport.Columns.Count; i++)
                            {
                                object val = row[i];
                                if (val != null && val.ToString().Contains("'"))
                                {
                                    string Value1 = val.ToString().Replace("'", "");
                                    Sql += "'" + Value1 + "',";
                                }
                                else
                                {
                                    if (val is DateTime || (val is string && IsLikelyDate(val.ToString())))
                                    {
                                        DateTime dt;
                                        if (val is DateTime)
                                            dt = (DateTime)val;
                                        else
                                            DateTime.TryParse(val.ToString(), out dt);
                                        Sql += "'" + dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture).Trim() + "',";
                                    }
                                    else
                                    {
                                        Sql += "'" + val.ToString().Trim() + "',";
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
                            Already_Exist++;
                        }
                        Total_records++;
                        Label5.Text = Total_records.ToString();
                        Application.DoEvents();
                    }
                }
                // VB.NET:
                // Sql = "delete nps_ecs_tbl_imp where ref_tran_code is null"
                Sql = "delete nps_ecs_tbl_imp where ref_tran_code is null";
                INSERT_RECORD(Sql);
                Label5.Text = (Total_records - 1).ToString();
                MessageBox.Show(" Out OF " + (Total_records - 1).ToString() + " Records " + Already_Exist.ToString() + " Are Already Exist ");
                // Cleanup
                RsImport = null;
                importExcelcon.Close();
                importExcelcon.Dispose();
                // -------------------- End VB Code Translation --------------------
            }
            catch (Exception ex)
            {
                // Error handling as per VB commented block
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // -------------------- Supporting Helper Methods --------------------

        // Executes a SQL non-query command using ADO.NET.
        private void MyConnExecute(string sql)
        {
            using (SqlConnection conn = new SqlConnection("YOUR_CONNECTION_STRING"))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Executes a SQL query and returns the results in a DataTable.
        private DataTable MyConnExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection("YOUR_CONNECTION_STRING"))
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // Retrieves Excel data using the provided query and OleDb connection.
        private DataTable GetExcelData(string query, OleDbConnection conn)
        {
            DataTable dt = new DataTable();
            using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
            {
                da.Fill(dt);
            }
            return dt;
        }

        // Mimics the newString function from VB; here it simply trims the string.
        private string newString(string input)
        {
            return input.Trim();
        }

        // Inserts a record by executing the provided SQL.
        private void INSERT_RECORD(string sql)
        {
            MyConnExecute(sql);
        }

        // Stub for checking field mapping; returns true (implement your logic as needed).
        private bool CheckFieldMapping(string dataBaseField, string statusChk)
        {
            return true;
        }

        // Stub for Get_Comp_TextFilePath; implement your logic based on compText.
        private void Get_Comp_TextFilePath(string compText)
        {
            // Implementation logic here.
        }

        // Stub for FieldsParametersName; returns a mapping string based on file name.
        private string FieldsParametersName(string fileName)
        {
            // For demonstration, returning a dummy mapping string.
            // Format: "DatabaseField#ExcelField,DatabaseField2#ExcelField2"
            return "Field1#ExcelField1,Field2#ExcelField2";
        }

        // Stub for Exc_Clent_FldName; returns a default field name when needed.
        private string Exc_Clent_FldName()
        {
            return "DefaultField";
        }

        // Helper to check if a string is likely a date.
        private bool IsLikelyDate(string input)
        {
            DateTime dt;
            return DateTime.TryParse(input, out dt);
        }
    }
}
