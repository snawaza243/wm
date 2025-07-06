using System;
using System.IO;
using ADODB;
using System.Web.UI;

public partial class ImportPage : System.Web.UI.Page
{
    // Assume these server controls are defined in the .aspx page.
    protected System.Web.UI.WebControls.TextBox TxtFileName;
    protected System.Web.UI.WebControls.Button CmdBrowse;
    protected System.Web.UI.WebControls.ListBox lstsheet;
    protected System.Web.UI.WebControls.TextBox TxtYear;
    protected System.Web.UI.WebControls.RadioButton OptDue;
    protected System.Web.UI.WebControls.RadioButton OptPaid;
    protected System.Web.UI.WebControls.DropDownList CmbMonth;
    protected System.Web.UI.WebControls.Label Label9;
    protected System.Web.UI.WebControls.Label Label5;
    protected System.Web.UI.WebControls.DropDownList CmbDataType;

    // Global variables assumed from original code.
    // These should be properly initialized elsewhere in the application.
    private dynamic XLW = null;          // Excel Workbook object
    private dynamic XL = null;           // Excel Application object
    private Connection importExcelcon = null;
    private Recordset rsExcel = null;
    private Recordset Rs_Chk_Excel = null;
    private Connection MyConn = null;
    private Recordset RsDueDate = null;
    private Recordset rslap = null;
    private Recordset Rs_Dup_policy = null;
    private string Glbloginid = "YourUserId";         // Replace with actual login id value
    private string MyImportDataType = "DUEDATA";        // Replace with actual import data type
    private DateTime ServerDateTime = DateTime.Now;     // Replace with actual server date/time if needed

    // Global variables for use inside the function.
    private string MyPremFreq = "";
    private DateTime MyLapsedDate = DateTime.MinValue;
    private string UpMon = "";
    private string UpYear = "";
    private string sql = "";
    private string doc = "";
    private string Field_value = "";

    protected void cmdImport_Click(object sender, EventArgs e)
    {
        //On Error Resume Next
        //On Error GoTo err
        if (TxtFileName.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                "alert('Select Valid Excel File');", true);
            CmdBrowse.Focus();
            return;
        }
        if (lstsheet.Items.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                "alert('No Excel Sheet Is Available For Importing Data');", true);
            CmdBrowse.Focus();
            return;
        }
        bool Chk_Lst_Sel = false;
        string Sheet_Name = "";
        int Count_Loop = 0;
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                "alert('Select Excel Sheet For Importing');", true);
            return;
        }
        if (Convert.ToInt32(TxtYear.Text) <= 1900 && Convert.ToInt32(TxtYear.Text) > 2100)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                "alert('Enter Valid Year');", true);
            TxtYear.Focus();
            return;
        }
        //Code For Saving The Excel Heading
        //'Set XLW = XL.Workbooks.open(TxtFileName.Text)
        Count_Loop = 1;
        while (XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value.ToString() != "")
        {
            XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value = XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value.ToString().Replace(".", "");
            XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value = XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value.ToString().Replace("/", "");
            XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value = XLW.Worksheets[Sheet_Name].Cells[1, Count_Loop].Value.ToString().Replace("-", "");
            Count_Loop = Count_Loop + 1;
        }
        XLW.Save();
        XLW.Close();
        XL.Quit();
        //Set XLW = Nothing: Set XL = Nothing

        //End Code For Saving The Excel Files
        if (!File.Exists(Server.MapPath("~/Life/insufld/Field_Parameter_Paid.txt")))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                "alert('Field Parameter File Not Found');", true);
            return;
        }
        string File_content = "";
        if (OptDue.Checked == true)
        {
            File_content = FieldsParametersName(Server.MapPath("~/Life/insufld/Field_Parameter_due.txt"));
        }
        else
        {
            File_content = FieldsParametersName(Server.MapPath("~/Life/insufld/Field_Parameter_paid.txt"));
        }
        if (File_content == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "msg",
                "alert('File Format Is Wrong');", true);
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
        selectedFileField = selectedFileField.Substring(0, selectedFileField.Length - 1);
        dataBaseField = dataBaseField.Substring(0, dataBaseField.Length - 1);
        if (importExcelcon != null && importExcelcon.State == ObjectStateEnum.adStateOpen)
        {
            importExcelcon.Close();
            importExcelcon = null;
        }
        importExcelcon = new Connection();
        importExcelcon.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + TxtFileName.Text + ";Extended Properties=\"Excel 8.0;HDR=Yes;\";";
        importExcelcon.Open();
        rsExcel = new Recordset();
        rsExcel = importExcelcon.Execute("Select " + selectedFileField + " from [" + Sheet_Name + "$] ");
        int Rec_Count = 0;
        int Rec_Count_exl = 0;
        Label9.Text = Rec_Count_exl.ToString();
        int rec1 = 0;
        string SqlStr = "";
        if (OptDue.Checked == true)
        {
            do
            {
                if ((rsExcel.Fields[Excel_Comp].Value + "").ToString() != "")
                {
                    SqlStr = " select * from bajaj_due_Data WHERE upper(trim(POLICY_no))=upper(trim('" + 
                        rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + 
                        "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + 
                        "' and mon_no = " + (CmbMonth.SelectedIndex + 1) + " and year_no=" + TxtYear.Text + " and importdatatype='" + MyImportDataType + "' ";
                    Rs_Chk_Excel = new Recordset();
                    Rs_Chk_Excel.Open(SqlStr, MyConn, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic);
                    if (Rs_Chk_Excel.EOF == true)
                    {
                        // commented by pankaj according to parvesh
                        //  If UCase(Trim(rsExcel("" & Trim(Excel_Comp) & "").Value)) <> "BIRLA SUN" Then
                        MyPremFreq = "";
                        if (rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "1" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "01" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "ANNUALLY" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "ANNUAL" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "YEARLY")
                        {
                            MyPremFreq = "1";
                        }
                        else if (rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "12" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "MONTHLY")
                        {
                            MyPremFreq = "12";
                        }
                        else if (rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "0")
                        {
                            MyPremFreq = "0";
                        }
                        else if (rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "QUARTERLY")
                        {
                            MyPremFreq = "4";
                        }
                        else if (rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "2" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "SEMI ANNUALLY" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "SEMI ANNUAL" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "SEMI-ANNUALLY" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "SEMI-ANNUAL" ||
                            rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "HALF YEARLY")
                        {
                            MyPremFreq = "2";
                        }
                        else if (rsExcel.Fields[Excel_Prem_Freq].Value.ToString().ToUpper().Trim() == "4")
                        {
                            MyPremFreq = "4";
                        }
                        SqlStr = "Insert into BAJAJ_due_DATA (" + dataBaseField + ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT)  Values(";
                        for (Count_Loop = 0; Count_Loop <= rsExcel.Fields.Count - 1; Count_Loop++)
                        {
                            if (rsExcel.Fields[Count_Loop].Value.ToString().IndexOf("'") > -1)
                            {
                                Field_value = rsExcel.Fields[Count_Loop].Value.ToString().Replace("'", "");
                                SqlStr = SqlStr + "'" + Field_value.Trim() + "',";
                            }
                            else
                            {
                                if (rsExcel.Fields[Count_Loop].Type == (int)DataTypeEnum.adDate)
                                {
                                    SqlStr = SqlStr + "'" + Convert.ToDateTime(rsExcel.Fields[Count_Loop].Value).ToString("dd-MMM-yyyy") + "',";
                                    if (rsExcel.Fields[Count_Loop].Name.ToString().ToUpper().Trim() == Excel_Due_Date.ToUpper().Trim())
                                    {
                                        if (Convert.ToDateTime(rsExcel.Fields[Count_Loop].Value).ToString("dd-MMM-yyyy") != "")
                                            MyLapsedDate = Convert.ToDateTime(rsExcel.Fields[Count_Loop].Value);
                                    }
                                }
                                else
                                {
                                    if (rsExcel.Fields[Count_Loop].Value.ToString().Trim().IndexOf(",") > -1)
                                    {
                                        SqlStr = SqlStr + "'" + rsExcel.Fields[Count_Loop].Value.ToString().Trim().Replace(",", "") + "',";
                                    }
                                    else
                                    {
                                        SqlStr = SqlStr + "'" + rsExcel.Fields[Count_Loop].Value.ToString().Trim() + "',";
                                    }
                                }
                            }
                        }
                        SqlStr = SqlStr + "" + (CmbMonth.SelectedIndex + 1) + "," + TxtYear.Text + ",'" + Glbloginid + "','" + ServerDateTime.ToString("dd-MMM-yyyy") + "','" + MyImportDataType + "','BAL' ";
                        SqlStr = SqlStr + ")";
                        SqlStr = SqlStr.Replace("''", "Null");
                        Rec_Count = Rec_Count + 1;

                        if (MyImportDataType == "LAPSEDDATA")
                        {
                            MyLapsedDate = MyLapsedDate;
                            UpMon = MyLapsedDate.ToString("MM");
                            UpYear = MyLapsedDate.ToString("yyyy");
                            sql = " SELECT   policy_no, company_cd, MAX (due_date) due_date,max(mon_no),max(year_no), ";
                            sql = sql + "         (SELECT MAX (status_cd) ";
                            sql = sql + "            FROM bajaj_due_data ";
                            sql = sql + "           WHERE UPPER (TRIM (policy_no)) = ";
                            sql = sql + "                                         UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) ";
                            sql = sql + "             AND UPPER (TRIM (company_cd)) = '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'";
                            sql = sql + "             AND importdatatype = 'DUEDATA' ";
                            sql = sql + "             AND due_date = ";
                            sql = sql + "                    (SELECT MAX (due_date) ";
                            sql = sql + "                       FROM bajaj_due_data ";
                            sql = sql + "                      WHERE UPPER (TRIM (policy_no)) = ";
                            sql = sql + "                                                    UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) ";
                            sql = sql + "                        AND UPPER (TRIM (company_cd)) = ";
                            sql = sql + "                                                   '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "' ";
                            sql = sql + "                        AND due_date IS NOT NULL AND IMPORTDATATYPE='DUEDATA' ";
                            sql = sql + "                        )) status_cd ";
                            sql = sql + "    FROM bajaj_due_data a ";
                            sql = sql + "   WHERE   importdatatype = 'DUEDATA' ";
                            sql = sql + "                      AND UPPER (TRIM (policy_no)) = ";
                            sql = sql + "                                                    UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) ";
                            sql = sql + "                        AND UPPER (TRIM (company_cd)) = ";
                            sql = sql + "                                                   '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "' ";
                            sql = sql + "GROUP BY policy_no, company_cd ";
                            RsDueDate = new Recordset();
                            RsDueDate.Open(sql, MyConn, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockOptimistic);
                            if (!RsDueDate.EOF)
                            {
                                if (MyLapsedDate >= Convert.ToDateTime(RsDueDate.Fields["due_date"].Value))
                                {
                                    MyConn.Execute("update bajaj_due_Data set status_Cd='LAPSED',last_update_dt='" + DateTime.Now.ToString("dd-MMM-yyyy") + "',last_update='" + Glbloginid + "' WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "' and due_date='" + Convert.ToDateTime(RsDueDate.Fields["due_date"].Value).ToString("dd-MMM-yyyy") + "' and importdatatype='DUEDATA' ");
                                    MyConn.Execute("update policy_details_master set last_status='L',UPDATE_PROG='" + CmbDataType.SelectedItem.Text + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                                    update_bajajar_status(rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim(), rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim(), "L", "LAPSED DATA");
                                }
                            }
                            RsDueDate.Close();
                        }
                        MyConn.Execute(SqlStr);

                        
                        if (MyImportDataType == "DUEDATA")
                        {
                            MyConn.Execute("update policy_details_master set FILE_NAME='" + TxtFileName.Text + "', PAYMENT_MODE='" + rsExcel.Fields[Excel_Payment_Mode].Value.ToString().ToUpper().Trim() + "',UPDATE_PROG='" + CmbDataType.SelectedItem.Text + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            if (MyPremFreq != "")
                            {
                                MyConn.Execute("update policy_details_master set FILE_NAME='" + TxtFileName.Text + "', PREM_FREQ='" + MyPremFreq + "',UPDATE_PROG='" + CmbDataType.SelectedItem.Text + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            }
                            if (rsExcel.Fields[doc].Value.ToString().ToUpper().Trim() != "")
                            {
                                MyConn.Execute("update policy_details_master set doc=to_date('" + rsExcel.Fields[doc].Value.ToString().ToUpper().Trim() + "','dd/mm/rrrr'),UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            }
                            if (rsExcel.Fields[excel_pol_term].Value.ToString().ToUpper().Trim() != "")
                            {
                                MyConn.Execute("update policy_details_master set ply_term='" + rsExcel.Fields[excel_pol_term].Value.ToString().ToUpper().Trim() + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            }
                            if (rsExcel.Fields[excel_mobile].Value.ToString().ToUpper().Trim() != "")
                            {
                                MyConn.Execute("update policy_details_master set mobile='" + rsExcel.Fields[excel_mobile].Value.ToString().ToUpper().Trim() + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            }
                            if (rsExcel.Fields[excel_sa].Value.ToString().ToUpper().Trim() != "")
                            {
                                MyConn.Execute("update policy_details_master set sa='" + rsExcel.Fields[excel_sa].Value.ToString().ToUpper().Trim() + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            }
                            if (rsExcel.Fields[Excel_Prem_Amt].Value.ToString().ToUpper().Trim() != "")
                            {
                                MyConn.Execute("update policy_details_master set prem_amt='" + rsExcel.Fields[Excel_Prem_Amt].Value.ToString().ToUpper().Trim() + "',UPDATE_USER='" + Glbloginid + "',UPDATE_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/YYYY') WHERE UPPER(TRIM(POLICY_no))=UPPER (TRIM ('" + rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + "'");
                            }
                        }
                        Label5.Text = Rec_Count.ToString();
                        DoEvents();
                    }
                    DoEvents();
                    Rs_Chk_Excel.Close();
                }
                Label9.Text = Rec_Count_exl.ToString();
                Rec_Count_exl = Rec_Count_exl + 1;
                if (Rec_Count_exl == 5501)
                {
                    //MsgBox UCase(Trim(rsExcel("" & Trim(Excel_policy_no) & "").Value))
                }
                rsExcel.MoveNext();
            } while (!rsExcel.EOF);


            SqlStr = " select  distinct policy_no from (select a.policy_no,a.sys_ar_Dt,a.company_Cd from bajaj_Ar_head a ,bajaj_due_Data b where";
            SqlStr = SqlStr + " upper(trim(a.COMPANY_CD)) = upper(trim(B.COMPANY_CD)) And a.Policy_No = B.Policy_No and mon_no = " + (CmbMonth.SelectedIndex + 1) + " and year_no=" + TxtYear.Text + " and importdatatype='" + MyImportDataType + "'";
            SqlStr = SqlStr + " group by a.policy_no,a.sys_ar_dt,a.company_Cd having count(a.policy_no)>1 and count(a.sys_ar_dt)>1 and count(a.company_Cd)>1)";
            Rs_Dup_policy = new Recordset();
            Rs_Dup_policy.Open(SqlStr, MyConn, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic);
            string Dup_Policy_Str = "";
            MyConn.Execute("DELETE FROM DUP_POLICY");
            while (!Rs_Dup_policy.EOF)
            {
                MyConn.Execute("INSERT INTO DUP_POLICY VALUES('" + Rs_Dup_policy.Fields["policy_no"].Value.ToString() + "')");
                Dup_Policy_Str = Dup_Policy_Str + "'" + Rs_Dup_policy.Fields["policy_no"].Value.ToString() + "'" + ",";
                Rs_Dup_policy.MoveNext();
            }
            if (Dup_Policy_Str != "")
                Dup_Policy_Str = Dup_Policy_Str.Substring(0, Dup_Policy_Str.Length - 1);
            int Rec_Del = 0;
            if (Dup_Policy_Str != "")
            {
                MyConn.Execute(" update bajaj_due_Data set dup_rec='Y' where policy_no in (SELECT POLICY_NO FROM DUP_POLICY) and mon_no =" + (CmbMonth.SelectedIndex + 1) + " and year_no=" + TxtYear.Text + " and importdatatype='" + MyImportDataType + "' ", out Rec_Del);
            }
            Rec_Count = Rec_Count - Rec_Del;
        }
        else if (OptPaid.Checked == true)
        {
            do
            {
                if ((rsExcel.Fields[Excel_Comp].Value + "").ToString() != "")
                {
                    if (CmbDataType.SelectedIndex == 2 || CmbDataType.SelectedIndex == 3)
                    {
                        if (rslap.State == ObjectStateEnum.adStateOpen)
                            rslap.Close();
                        rslap.Open("select POLICY_NO  from bajaj_due_data where UPPER(TRIM(POLICY_no))=UPPER(TRIM('" + 
                            rsExcel.Fields[Excel_policy_no].Value.ToString().ToUpper().Trim() + 
                            "')) and upper(trim(company_cd))= '" + rsExcel.Fields[Excel_Comp].Value.ToString().ToUpper().Trim() + 
                            "' and mon_no =" + (CmbMonth.SelectedIndex + 1) + " and year_no=" + TxtYear.Text + " AND IMPORTDATATYPE='" + MyImportDataType + "'", MyConn, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic);
                        if (rslap.EOF)
                        {
                            SqlStr = "Insert into BAJAJ_DUE_DATA (" + dataBaseField + ",Mon_no,Year_No,UserId,Import_Dt,ImportDataType,NEWINSERT,FORCE_FLAG)  Values(";
                            for (Count_Loop = 0; Count_Loop <= rsExcel.Fields.Count - 1; Count_Loop++)
                            {
                                if (rsExcel.Fields[Count_Loop].Value.ToString().IndexOf("'") > -1)
                                {
                                    Field_value = rsExcel.Fields[Count_Loop].Value.ToString().Replace("'", "");
                                    SqlStr = SqlStr + "'" + Field_value.Trim() + "',";
                                }
                                else
                                {
                                    if (rsExcel.Fields[Count_Loop].Type == (int)DataTypeEnum.adDate)
                                    {
                                        SqlStr = SqlStr + "'" + Convert.ToDateTime(rsExcel.Fields[Count_Loop].Value).ToString("dd-MMM-yyyy") + "',";
                                    }
                                    else
                                    {
                                        if (rsExcel.Fields[Count_Loop].Value.ToString().Trim().IndexOf(",") > -1)
                                        {
                                            SqlStr = SqlStr + "'" + rsExcel.Fields[Count_Loop].Value.ToString().Trim().Replace(",", "") + "',";
                                        }
                                        else
                                        {
                                            SqlStr = SqlStr + "'" + rsExcel.Fields[Count_Loop].Value.ToString().Trim() + "',";
                                        }
                                    }
                                }
                            }
                            SqlStr = SqlStr + "" + (CmbMonth.SelectedIndex + 1) + "," + TxtYear.Text + ",'" + Glbloginid + "','" + ServerDateTime.ToString("dd-MMM-yyyy") + "','" + MyImportDataType + "','BAL','Y' ";
                            SqlStr = SqlStr + ")";
                            SqlStr = SqlStr.Replace("''", "Null");
                            Rec_Count = Rec_Count + 1;
                            MyConn.Execute(SqlStr);
                            Label5.Text = Rec_Count.ToString();
                            DoEvents();
                        }
                    }
                }
                rsExcel.MoveNext();
            } while (!rsExcel.EOF);
        }
    }

    // Helper function to simulate DoEvents in a web environment. In web pages, this is not needed.
    private void DoEvents()
    {
        // No equivalent in web applications.
    }

    // Helper function to read the field parameters from a text file.
    private string FieldsParametersName(string filePath)
    {
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            // If any exception occurs, return an empty string.
            return "";
        }
    }

    // Stub for update_bajajar_status method as used in the code.
    private void update_bajajar_status(string policy_no, string company_cd, string status, string remark)
    {
        // Complete implementation should be provided based on business logic.
        // For now, this is a stub that performs no operation.
    }
}
