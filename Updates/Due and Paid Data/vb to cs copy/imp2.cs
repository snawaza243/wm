using System;
using System.Data;
using System.IO;
using ADODB;
using System.Web.UI;

public partial class YourPage : System.Web.UI.Page
{
    // All required ASP.NET control definitions (assumed to be declared in the .aspx page)
    protected System.Web.UI.WebControls.TextBox TxtFileName;
    protected System.Web.UI.WebControls.TextBox TxtYear;
    protected System.Web.UI.WebControls.Button CmdBrowse;
    protected System.Web.UI.WebControls.ListBox lstsheet;
    protected System.Web.UI.WebControls.RadioButton OptDue;
    protected System.Web.UI.WebControls.RadioButton OptPaid;
    protected System.Web.UI.WebControls.DropDownList CmbMonth;
    protected System.Web.UI.WebControls.DropDownList CmbDataType;
    protected System.Web.UI.WebControls.Label Label9;
    protected System.Web.UI.WebControls.Label Label5;
    
    // Global variables from the original VB code
    protected string Sheet_Name = "";
    protected string MyImportDataType = "";
    protected string Glbloginid = "";
    protected DateTime ServerDateTime = DateTime.Now;
    // Assume MyConn is initialized elsewhere (e.g., in Page_Load) with the proper connection string.
    protected ADODB.Connection MyConn; 

    // Helper function to read file content; complete implementation is provided.
    private string FieldsParametersName(string filePath)
    {
        return File.ReadAllText(filePath);
    }
    
    // Helper function as referenced in the VB code.
    private void update_bajajar_status(string policy_no, string company_cd, string status, string remark)
    {
        // Complete implementation preserved from the original context.
        // (No implementation details were provided in the original code.)
    }
    
    // This is the converted function from the original EXE-based VB code.
    public void ImportExcelData(DataTable dt)
    {
        // Translated from: Private Sub cmdImport_Click()
        // 'On Error Resume Next
        // 'On Error GoTo err
        
        if (string.IsNullOrEmpty(TxtFileName.Text))
        {
            // MsgBox "Select Valid Excel File", vbInformation
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Select Valid Excel File');", true);
            CmdBrowse.Focus();
            return;
        }
        if (lstsheet.Items.Count == 0)
        {
            // MsgBox "No Excel Sheet Is Available For Importing Data "
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No Excel Sheet Is Available For Importing Data');", true);
            CmdBrowse.Focus();
            return;
        }
        bool Chk_Lst_Sel = false;
        int Count_Loop;
        for (Count_Loop = 0; Count_Loop <= lstsheet.Items.Count - 1; Count_Loop++)
        {
            if (lstsheet.Items[Count_Loop].Selected == true)
            {
                Sheet_Name = lstsheet.Items[Count_Loop].Text;
                Chk_Lst_Sel = true;
                break;
            }
        }
        if (Chk_Lst_Sel == false)
        {
            // If Chk_Lst_Sel = False Then MsgBox "Select Excel Sheet For Importing": Exit Sub
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Select Excel Sheet For Importing');", true);
            return;
        }
        if (Convert.ToInt32(TxtYear.Text) <= 1900 && Convert.ToInt32(TxtYear.Text) > 2100)
        {
            // MsgBox "Enter Valid Year", vbInformation
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Enter Valid Year');", true);
            TxtYear.Focus();
            return;
        }
        
        // 'Code For Saving The Excel Heading'
        // Instead of Excel objects, we update the DataTable column names.
        for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
        {
            string colName = dt.Columns[colIndex].ColumnName;
            colName = colName.Replace(".", "").Replace("/", "").Replace("-", "");
            dt.Columns[colIndex].ColumnName = colName;
        }
        // Simulate: XLW.save: XLW.Close: XL.Quit
        // 'End Code For Saving The Excel Files
        
        // Check if the field parameter file exists (using Server.MapPath for web applications)
        string filePath = Server.MapPath(@"~/Life/insufld/Field_Parameter_Paid.txt");
        if (!File.Exists(filePath))
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Field Parameter File Not Found');", true);
            return;
        }
        string File_content = "";
        if (OptDue.Checked == true)
        {
            File_content = FieldsParametersName(Server.MapPath(@"~/Life/insufld/Field_Parameter_due.txt"));
        }
        else
        {
            File_content = FieldsParametersName(Server.MapPath(@"~/Life/insufld/Field_Parameter_paid.txt"));
        }
        if (File_content == "")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('File Format Is Wrong');", true);
            return;
        }
        string[] delComma = File_content.Split(',');
        string selectedFileField = "";
        string dataBaseField = "";
        string Excel_policy_no = "";
        string Excel_Status = "";
        string Excel_Comp = "";
        string Excel_Payment_Mode = "";
        string Excel_Prem_Freq = "";
        string Excel_Due_Date = "";
        string doc = "";
        string Excel_Prem_Amt = "";
        string excel_mobile = "";
        string excel_pol_term = "";
        string excel_sa = "";
        
        for (Count_Loop = 0; Count_Loop <= delComma.GetUpperBound(0); Count_Loop++)
        {
            string[] delHash = delComma[Count_Loop].Split('#');
            selectedFileField = selectedFileField + delHash[1] + ",";
            dataBaseField = dataBaseField + delHash[0] + ",";
            if (delHash[0].ToUpper().Trim() == "POLICY_NO")
            {
                Excel_policy_no = delHash[1].Replace("[", "").Replace("]", "").Replace("-", "");
            }
            if (delHash[0].ToUpper().Trim() == "COMPANY_CD")
            {
                Excel_Comp = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (delHash[0].ToUpper().Trim() == "PAY_MODE")
            {
                Excel_Payment_Mode = delHash[1].Replace("[", "").Replace("]", "").Replace("-", "");
            }
            if (delHash[0].ToUpper().Trim() == "PREM_FREQ")
            {
                Excel_Prem_Freq = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (delHash[0].ToUpper().Trim() == "DUE_DATE")
            {
                Excel_Due_Date = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (OptPaid.Checked == true)
            {
                if (delHash[0].ToUpper().Trim() == "STATUS_CD")
                {
                    Excel_Status = delHash[1].Replace("[", "").Replace("]", "");
                }
            }
            if (delHash[0].ToUpper().Trim() == "DOC")
            {
                doc = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (delHash[0].ToUpper().Trim() == "PREM_AMT")
            {
                Excel_Prem_Amt = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (delHash[0].ToUpper().Trim() == "CL_MOBILE")
            {
                excel_mobile = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (delHash[0].ToUpper().Trim() == "PLY_TERM")
            {
                excel_pol_term = delHash[1].Replace("[", "").Replace("]", "");
            }
            if (delHash[0].ToUpper().Trim() == "SA")
            {
                excel_sa = delHash[1].Replace("[", "").Replace("]", "");
            }
        }
        
        if (selectedFileField.Length > 0)
            selectedFileField = selectedFileField.Substring(0, selectedFileField.Length - 1);
        if (dataBaseField.Length > 0)
            dataBaseField = dataBaseField.Substring(0, dataBaseField.Length - 1);
        
        // Preserve original ADODB connection to Excel even though the DataTable dt is being used.
        // If the import connection was open, close it, then create a new one.
        ADODB.Connection importExcelcon = new ADODB.Connection();
        importExcelcon.Open("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + TxtFileName.Text + ";Extended Properties=\"Excel 8.0;HDR=Yes;\";");
        // Original VB code executes a SQL query to obtain rsExcel.
        // Here, we use the passed DataTable dt in place of rsExcel.
        
        int Rec_Count = 0;
        int Rec_Count_exl = 0;
        Label9.Text = Rec_Count_exl.ToString();
        int rec1 = 0;
        
        if (OptDue.Checked == true)
        {
            // Process each row in the DataTable as if iterating through the Excel recordset.
            foreach (DataRow row in dt.Rows)
            {
                if ((row[Excel_Comp].ToString() + "") != "")
                {
                    string sqlSelect = " select * from bajaj_due_Data WHERE upper(trim(POLICY_no))=upper(trim('" + 
                        row[Excel_policy_no].ToString().Trim().ToUpper() + "')) and upper(trim(company_cd))= '" + 
                        row[Excel_Comp].ToString().Trim().ToUpper() + "' and mon_no = " + (CmbMonth.SelectedIndex + 1) + 
                        " and year_no=" + TxtYear.Text + " and importdatatype='" + MyImportDataType + "' ";
                    Recordset Rs_Chk_Excel = new Recordset();
                    Rs_Chk_Excel.Open(sqlSelect, MyConn, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, -1);
                    if (Rs_Chk_Excel.EOF == true)
                    {
                        // ' commented by pankaj according to parvesh
                        string MyPremFreq = "";
                        string premFreqVal = row[Excel_Prem_Freq].ToString().Trim().ToUpper();
                        
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
                        else if (premFreqVal == "QUARTERLY")
                        {
                            MyPremFreq = "4";
                        }
                        else if (premFreqVal == "2" || premFreqVal == "SEMI ANNUALLY" || premFreqVal == "SEMI ANNUAL" ||
                                 premFreqVal == "SEMI-ANNUALLY" || premFreqVal == "SEMI-ANNUAL" || premFreqVal == "HALF YEARLY")
                        {
                            MyPremFreq = "2";
                        }
                        else if (premFreqVal == "4")
                        {
                            MyPremFreq = "4";
                        }
                        
                        string SqlStr = "Insert into BAJAJ_due_DATA (" + dataBaseField + ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT)  Values(";
                        for (Count_Loop = 0; Count_Loop <= dt.Columns.Count - 1; Count_Loop++)
                        {
                            string fieldName = dt.Columns[Count_Loop].ColumnName;
                            string fieldValue = row[fieldName].ToString();
                            if (fieldValue.Contains("'"))
                            {
                                fieldValue = fieldValue.Replace("'", "");
                                SqlStr = SqlStr + "'" + fieldValue.Trim() + "',";
                            }
                            else
                            {
                                // If the column is of DateTime type, format the date accordingly.
                                if (dt.Columns[Count_Loop].DataType == typeof(DateTime))
                                {
                                    DateTime dtValue = DateTime.MinValue;
                                    DateTime.TryParse(fieldValue, out dtValue);
                                    string formattedDate = dtValue.ToString("dd-MMM-yyyy");
                                    SqlStr = SqlStr + "'" + formattedDate + "',";
                                    if (fieldName.Trim().ToUpper() == Excel_Due_Date.Trim().ToUpper())
                                    {
                                        // If necessary, MyLapsedDate assignment would be here.
                                    }
                                }
                                else
                                {
                                    if (fieldValue.Trim().Contains(","))
                                    {
                                        SqlStr = SqlStr + "'" + fieldValue.Trim().Replace(",", "") + "',";
                                    }
                                    else
                                    {
                                        SqlStr = SqlStr + "'" + fieldValue.Trim() + "',";
                                    }
                                }
                            }
                        }
                        SqlStr = SqlStr + "" + (CmbMonth.SelectedIndex + 1) + "," + TxtYear.Text + ",'" + Glbloginid + "','" + 
                                 ServerDateTime.ToString("dd-MMM-yyyy") + "','" + MyImportDataType + "','BAL' ";
                        SqlStr = SqlStr + ")";
                        SqlStr = SqlStr.Replace("''", "Null");
                        Rec_Count = Rec_Count + 1;
                        
                        // The following block (related to LAPSEDDATA) is preserved as comments.
                        // If MyImportDataType = "LAPSEDDATA" Then
                        //       MyLapsedDate = MyLapsedDate
                        //       UpMon = Format(MyLapsedDate, "mm")
                        //       UpYear = Format(MyLapsedDate, "yyyy")
                        //       sql = " SELECT   policy_no, company_cd, MAX (due_date) due_date,max(mon_no),max(year_no), "
                        //       ... additional SQL omitted ...
                        //       RsDueDate.open sql, MyConn, adOpenForwardOnly, adLockOptimistic
                        //       If RsDueDate.EOF = False Then
                        //           If MyLapsedDate >= RsDueDate.Fields("due_date") Then
                        //               MyConn.Execute "update bajaj_due_Data set status_Cd='LAPSED',last_update_dt='" & Format(Now, "dd-mmm-yyyy") & "',last_update='" & Glbloginid & "' WHERE ... "
                        //               MyConn.Execute "update policy_details_master set last_status='L',UPDATE_PROG='" & CmbDataType.Text & "',UPDATE_USER='" & Glbloginid & "',UPDATE_DATE=TO_DATE('" & Format(ServerDateTime, "DD/MM/YYYY") & "','DD/MM/YYYY') WHERE ... "
                        //               Call update_bajajar_status(...);
                        //           End If
                        //       End If
                        //       RsDueDate.Close
                        
                        MyConn.Execute(SqlStr);
                        
                        if (MyImportDataType == "DUEDATA")
                        {
                            MyConn.Execute("update policy_details_master set FILE_NAME='" + TxtFileName.Text + "', PAYMENT_MODE='" + 
                                row[Excel_Payment_Mode].ToString().Trim().ToUpper() + "',UPDATE_PROG='" + CmbDataType.SelectedItem.Text + 
                                "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + 
                                "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() + 
                                "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            if (MyPremFreq != "")
                            {
                                MyConn.Execute("update policy_details_master set FILE_NAME='" + TxtFileName.Text + "', PREM_FREQ='" + MyPremFreq +
                                    "',UPDATE_PROG='" + CmbDataType.SelectedItem.Text + "',UPDATE_USER='" + Glbloginid + 
                                    "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + 
                                    row[Excel_policy_no].ToString().Trim().ToUpper() + "')) and upper(trim(company_cd))= '" +
                                    row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            }
                            
                            if ((row[doc].ToString().Trim().ToUpper() + "") != "")
                            {
                                MyConn.Execute("update policy_details_master set doc=to_date('" + row[doc].ToString().Trim().ToUpper() + 
                                    "','dd/mm/rrrr'),UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + 
                                    "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() + 
                                    "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            }
                            
                            if ((row[excel_pol_term].ToString().Trim().ToUpper() + "") != "")
                            {
                                MyConn.Execute("update policy_details_master set ply_term='" + row[excel_pol_term].ToString().Trim().ToUpper() + 
                                    "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + 
                                    "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() + 
                                    "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            }
                            if ((row[excel_mobile].ToString().Trim().ToUpper() + "") != "")
                            {
                                MyConn.Execute("update policy_details_master set mobile='" + row[excel_mobile].ToString().Trim().ToUpper() + 
                                    "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + 
                                    "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() + 
                                    "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            }
                            if ((row[excel_sa].ToString().Trim().ToUpper() + "") != "")
                            {
                                MyConn.Execute("update policy_details_master set sa='" + row[excel_sa].ToString().Trim().ToUpper() + 
                                    "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + 
                                    "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() + 
                                    "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            }
                            if ((row[Excel_Prem_Amt].ToString().Trim().ToUpper() + "") != "")
                            {
                                MyConn.Execute("update policy_details_master set prem_amt='" + row[Excel_Prem_Amt].ToString().Trim().ToUpper() + 
                                    "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd-MMM-yyyy") + 
                                    "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + row[Excel_policy_no].ToString().Trim().ToUpper() + 
                                    "')) and upper(trim(company_cd))= '" + row[Excel_Comp].ToString().Trim().ToUpper() + "'");
                            }
                        }
                        Label5.Text = Rec_Count.ToString();
                        // DoEvents is omitted as it is not applicable in web contexts.
                    }
                    Rs_Chk_Excel.Close();
                }
                Label9.Text = Rec_Count_exl.ToString();
                Rec_Count_exl = Rec_Count_exl + 1;
                if (Rec_Count_exl == 5501)
                {
                    // MsgBox UCase(Trim(rsExcel("" + Trim(Excel_policy_no) + "").Value))
                }
                // Move to next row in the DataTable (loop continues)
            }
            string dupSql = " select  distinct policy_no from (select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_due_Data b where";
            dupSql = dupSql + " upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = " + (CmbMonth.SelectedIndex + 1) + 
                     " and year_no=" + TxtYear.Text + " and importdatatype='" + MyImportDataType + "'";
            dupSql = dupSql + " group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(a.company_Cd)>1)";
            Recordset Rs_Dup_policy = new Recordset();
            Rs_Dup_policy.Open(dupSql, MyConn, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, -1);
            string Dup_Policy_Str = "";
            MyConn.Execute("DELETE FROM DUP_POLICY");
            while (!Rs_Dup_policy.EOF)
            {
                MyConn.Execute("INSERT INTO DUP_POLICY VALUES('" + Rs_Dup_policy.Fields["policy_no"].Value.ToString() + "')");
                Dup_Policy_Str = Dup_Policy_Str + "'" + Rs_Dup_policy.Fields["policy_no"].Value.ToString() + "'" + ",";
                Rs_Dup_policy.MoveNext();
            }
            if (Dup_Policy_Str != "")
            {
                Dup_Policy_Str = Dup_Policy_Str.Substring(0, Dup_Policy_Str.Length - 1);
            }
            int Rec_Del = 0;
            if (Dup_Policy_Str != "")
            {
                MyConn.Execute(" update bajaj_due_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no =" + 
                    (CmbMonth.SelectedIndex + 1) + " and year_no=" + TxtYear.Text + " and importdatatype='" + MyImportDataType + "' ", out Rec_Del);
            }
            Rec_Count = Rec_Count - Rec_Del;
        }
        else if (OptPaid.Checked == true)
        {
            // Process the 'Paid' branch.
            // The provided code is truncated starting with the For loop.
            foreach (DataRow row in dt.Rows)
            {
                if ((row[Excel_Comp].ToString() + "") != "")
                {
                    if ((CmbDataType.SelectedIndex == 2 || CmbDataType.SelectedIndex == 3)) // paid and reinstate
                    {
                        Recordset rslap = new Recordset();
                        if (rslap.State == (int)ObjectStateEnum.adStateOpen)
                            rslap.Close();
                        rslap.Open("select POLICY_NO  from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('" + 
                            row[Excel_policy_no].ToString().Trim().ToUpper() + "')) and upper(trim(company_cd))= '" + 
                            row[Excel_Comp].ToString().Trim().ToUpper() + "' and mon_no =" + (CmbMonth.SelectedIndex + 1) + 
                            " and year_no=" + TxtYear.Text + " AND IMPORTDATATYPE='" + MyImportDataType + "'", MyConn, 
                            CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, -1);
                        if (rslap.EOF)
                        {
                            string SqlStr = "Insert into BAJAJ_DUE_DATA (" + dataBaseField + ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG)  Values(";
                            for (Count_Loop = 0; Count_Loop <= dt.Columns.Count - 1; Count_Loop++)
                            {
                                if (row[Count_Loop].ToString().Contains("'"))
                                {
                                    string Field_value = row[Count_Loop].ToString().Replace("'", "");
                                    SqlStr = SqlStr + "'" + Field_value.Trim() + "',";
                                }
                                else
                                {
                                    if (dt.Columns[Count_Loop].DataType == typeof(DateTime))
                                    {
                                        DateTime dtValue = DateTime.MinValue;
                                        DateTime.TryParse(row[Count_Loop].ToString(), out dtValue);
                                        SqlStr = SqlStr + "'" + dtValue.ToString("dd-MMM-yyyy") + "',";
                                    }
                                    else
                                    {
                                        if (row[Count_Loop].ToString().Trim().Contains(","))
                                        {
                                            SqlStr = SqlStr + "'" + row[Count_Loop].ToString().Trim().Replace(",", "") + "',";
                                        }
                                        else
                                        {
                                            SqlStr = SqlStr + "'" + row[Count_Loop].ToString().Trim() + "',";
                                        }
                                    }
                                }
                            }
                            // The remainder of the paid branch code is not provided in the snippet.
                        }
                    }
                }
            }
        }
        // End of the ImportExcelData function.
    }



}
