using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using WM.Controllers;
using OfficeOpenXml; 
using ListItem = System.Web.UI.WebControls.ListItem;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration; 
using System.Data.OleDb;
using NPOI.SS.Formula.PTG; 
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using Amazon.Runtime.Internal.Util;


namespace WM.Masters


{
    public partial class NpsTransactionPunching : Page
    {
        PsmController pc = new PsmController();

        
       
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
                    #region ONLOAD FILL
                    FillSchemaCode();
                    txtAmountReceivedTire2.Enabled = false;
                    //FillCRABranchList();
                    //fillCRABrnach();
                    fillBranchMasterList();
                    fillRequestType();
                    FillBankMasterList();
                    //RegisterToggleCorporateNameScript();
                    //cboKRA_SelectedIndexChanged();
                     

                    CRA_AND_POP_REG_NUM("0");
                    #endregion

                    #region LOG BASED PERMISSIONS
 
                    try
                    {
                        string loginSessionValue = Session["LoginId"]?.ToString() ?? string.Empty;

                        if (pc.currentLoginID() != null && pc.currentRoleID() != null)
                        {
                            string glbLoginId = pc.currentLoginID();
                            string glbRoleId =  pc.currentRoleID();

                            string[] glbLoginIdsForBoth = { "115514", "46183", "114678" };
                            string[] glbLoginIdsForExport = { "1", "91" };
                            string[] glbLoginIdsForSaveModify = { "212", };

                            bool glbLoginIdValuesForBoth = glbRoleId.AsEnumerable().Any(row => glbLoginIdsForBoth.Contains(glbRoleId.ToString()));
                            bool glbLoginIdValuesForExport = glbRoleId.AsEnumerable().Any(row => glbLoginIdsForExport.Contains(glbRoleId.ToString()));
                            bool glbLoginIdValuesForSaveModity = glbRoleId.AsEnumerable().Any(row => glbLoginIdsForExport.Contains(glbRoleId.ToString()));

                            if (glbLoginIdValuesForBoth)
                            {
                                btnImportEcs.Enabled = true;
                                btnImportCorporateNonEsc.Enabled = true;
                            }
                            else
                            {
                                btnImportEcs.Enabled = false;
                                btnImportCorporateNonEsc.Enabled = false;
                            }

                            if (glbLoginIdValuesForExport) { 
                                btnExportToExcel.Enabled = true; 
                            }

                            if (glbLoginIdValuesForSaveModity)
                            {
                                btnSave.Enabled = true;
                                btnModify.Enabled = true;
                            }
                        }

                        // Set Date and Time fields
                        txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                        txtTime.Text = DateTime.Now.ToString("HH:mm");

                        // Default options
                        //OptIndividual.Checked = true;
                        //optExECS.Checked = true;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                        Console.WriteLine("Error in Page_Load: " + ex.Message);
                    }
                    

                    #endregion

                    #region FIND INV_CODE AND INVESTOR_NAME
                    string arByInv = Session["AR_NPS_FIND_INV"]?.ToString();
                    string[] arByInvSplit = arByInv?.Split('#');
                    if(arByInvSplit != null && arByInvSplit.Length > 0)
                    {
                        string invCode = arByInvSplit[0];
                        string invName = arByInvSplit[1];
                        fetchSetByDtInv(invCode, false);
                        txtInvestorCode.Text = invCode;
                        txtNameOfSubscriber.Text = invName;
                        Session["AR_NPS_FIND_INV"] = null;
                        return;
                    }
                    #endregion


                    #region FILL BY FOUND AR ROW
                    DataRow currentArRow = Session["AR_FIND_TR_ROW"] as DataRow;
                    if (currentArRow != null)
                    {
                        SetFieldData(currentArRow);
                        btnSave.Enabled = false;
                        btnModify.Enabled = true;
                        txtBusinessRm.Enabled = false;
                        Session["AR_FIND_TR_ROW"] = null;
                        return;
                    }
                    #endregion
                }
            }
        }

        #region

        public void ImportNPSData(string selectedFileField, string sheetName, string dataDaseField,  string[] delComma)
        {
            string Glbloginid = Session["LoginId"]?.ToString() ?? string.Empty; 
            try
            {
                string insertBackupQuery = "INSERT INTO nps_ecs_tbl_imp_bk SELECT * FROM nps_ecs_tbl_imp";
                string deleteQuery = "DELETE FROM nps_ecs_tbl_imp";

                pc.ExecuteCurrentQuery(insertBackupQuery);
                pc.ExecuteCurrentQuery(deleteQuery);


                using (OleDbConnection excelConn = new OleDbConnection("your_excel_connection_string"))
                {
                    excelConn.Open();
                    string excelQuery = $"SELECT {selectedFileField} FROM [{sheetName}$]";

                    DataTable dt = pc.ExecuteCurrentQuery(excelQuery);

                    int totalRecords = 0, alreadyExist = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        string sqlChk = "SELECT * FROM nps_ecs_tbl_imp WHERE ";

                        foreach (var field in delComma)
                        {
                            string[] delHash = field.Split('#');
                            string xlsField = delHash[1].Replace("[", "").Replace("]", "").Replace("'", "");

                            if (row[xlsField] is DBNull)
                            {
                                sqlChk += $"{delHash[0]} IS NULL AND ";
                            }
                            else
                            {
                                sqlChk += $"{delHash[0]} = '{row[xlsField]}' AND ";
                            }
                        }
                        sqlChk = sqlChk.TrimEnd(" AND ".ToCharArray());

                        DataTable dtChk = pc.ExecuteCurrentQuery(sqlChk);

                        if (dtChk.Rows.Count>0)
                        {
                            string sqlInsert = "INSERT INTO nps_ecs_tbl_imp(" + dataDaseField + ", Import_dt, LOGGEDUserID) VALUES(";

                            foreach (var item in row.ItemArray)
                            {
                                sqlInsert += item is DBNull ? "NULL, " : $"'{item.ToString().Replace("'", "")}', ";
                            }

                            sqlInsert += $"'{DateTime.Now:dd-MMM-yyyy}', '{Glbloginid}')";
                            sqlInsert = sqlInsert.Replace("''", "NULL");

                            DataTable dt_sqlInsert = pc.ExecuteCurrentQuery(sqlInsert);
                        }
                        else
                        {
                            alreadyExist++;
                        }
                        totalRecords++;
                    }

                    string deleteNullRecords = "DELETE FROM nps_ecs_tbl_imp WHERE ref_tran_code IS NULL";
                   DataTable dt_deleteNullRecords = pc.ExecuteCurrentQuery(deleteNullRecords);

                    pc.ShowAlert(this, $"Out of {totalRecords} records, {alreadyExist} were already existing.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }



        #endregion



        // ONLOAD FILL FUNCTIONS
        #region ONLOAD FILL

        private void FillBankMasterList()
        {
            // Create an instance of the controller that retrieves bank master data
            var bankController = new WM.Controllers.NpsTransactionPunchingController();

            // Fetch the bank master list from the database
            DataTable bankMasterList = bankController.GetBankMasterList();
            ddlBankName.DataSource = bankMasterList;
            ddlBankName.DataTextField = "BANK_NAME";
            ddlBankName.DataValueField = "BRANCH_CODE";
            ddlBankName.DataBind();

            // Insert a default item at the top of the dropdown list
            ddlBankName.Items.Insert(0, new ListItem("Select Bank", "0"));
        }


        private void FillCRABranchList()
        {
            // Fetch the branch master list
            DataTable dt = new WM.Controllers.NpsTransactionPunchingController().GetCRABranchMasterList();

            // Bind the fetched data to the dropdown
            ddlCraBranch.DataSource = dt;
            ddlCraBranch.DataTextField = "BRANCH_NAME";   // Text field for dropdown display
            ddlCraBranch.DataValueField = "BRANCH_CODE";  // Value field for dropdown value
            ddlCraBranch.DataBind();

            // Insert a default "Select" option at the top
            ddlCraBranch.Items.Insert(0, new ListItem("Select", "0"));


        }

        private void FillSchemaCode()
        {
            // Get the list of scheme codes from the database
            DataTable schemeCodeList = new WM.Controllers.NpsTransactionPunchingController().GetSchemeCodeList();

            if (schemeCodeList != null && schemeCodeList.Rows.Count > 0)
            {
                // Clear existing items before binding
                ddlScheme.Items.Clear();

                // Iterate through the DataTable and add items with custom text
                foreach (DataRow row in schemeCodeList.Rows)
                {
                    string schemeCode = row["SCH_CODE"].ToString();
                    string displayText;

                    // Set the display text based on the scheme code
                    switch (schemeCode)
                    {
                        case "OP#09971":
                            displayText = "New Pension Scheme Tier 1";
                            break;
                        case "OP#09972":
                            displayText = "New Pension Scheme Tier 2";
                            break;
                        case "OP#09973":
                            displayText = "New Pension Scheme Tier 1+2";
                            break;
                        default:
                            displayText = "Unknown Scheme"; // Fallback text for any unhandled codes
                            break;
                    }

                    // Add the item to the dropdown list
                    ddlScheme.Items.Add(new ListItem(displayText, schemeCode));
                }
            }

            // Add a default item to the dropdown list
            //ddlScheme.Items.Insert(0, new ListItem("Select Scheme", "0"));
        }

        private void fillRequestType()
        {
            DataTable requestTypeList = new WM.Controllers.NpsTransactionPunchingController().GetRequestTypeList();
            DataView dv = requestTypeList.DefaultView;
            if (dv != null)
            {
                ddlRequestType.DataSource = dv;
                ddlRequestType.DataTextField = "REQUEST_NAME";
                ddlRequestType.DataValueField = "REQUEST_CODE";
                dv.Sort = "REQUEST_NAME ASC";  
                ddlRequestType.DataSource = dv.ToTable();
                ddlRequestType.DataBind();
                ddlRequestType.Items.Insert(0, new ListItem("Select", ""));
            }
        }

        private void fillCRABrnach()
        {
            int craBranchCode = 0;
            DataTable dt = new WM.Controllers.NpsTransactionPunchingController().NTPCraGetBranchData(craBranchCode);

            ddlCraBranch.DataSource = dt;
            ddlCraBranch.DataTextField = "BRANCH_NAME";
            ddlCraBranch.DataValueField = "BRANCH_CODE";
            ddlCraBranch.DataBind();
        }

        private void fillBranchMasterList()
        {

            DataTable dt = new WM.Controllers.NpsTransactionPunchingController().GetBranchMasterList();
            AddDefaultItem(dt, "BRANCH_NAME", "BRANCH_CODE", "Select");
            ddlBusinessBranch.DataSource = dt;
            ddlBusinessBranch.DataTextField = "BRANCH_NAME";
            ddlBusinessBranch.DataValueField = "BRANCH_CODE";
            ddlBusinessBranch.DataBind();
        }

        private void AddDefaultItem(DataTable dt, string textField, string valueField, string defaultText)
        {
            DataRow row = dt.NewRow();
            row[textField] = defaultText;

            if (dt.Columns[valueField].DataType == typeof(string))
            {
                row[valueField] = string.Empty;
            }
            else
            {
                row[valueField] = DBNull.Value;
            }

            dt.Rows.InsertAt(row, 0);
        }

        #endregion

        // ONCHANGE EVENTS
        #region ONCHANGE EVENTS

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check the selected value of ddlType
            if (ddlType.SelectedValue == "1") // Corporate selected
            {
                corporateName.Enabled = true;  // Enable Corporate Name textbox
            }
            else
            {
                corporateName.Enabled = false; // Disable Corporate Name textbox
                corporateName.Text = ""; // Clear the textbox when disabled
            }
        }


        protected void ddlScheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get selected value of the dropdown
            string selectedValue = ddlScheme.SelectedValue;

            // Enable/disable textboxes based on selection
            if (selectedValue == "OP#09971") // Tier 1
            {
                txtAmountReceivedTire1.Enabled = true;
                txtAmountReceivedTire2.Enabled = false;
                txtAmountReceivedTire2.Text = ""; // Clear Tier 2 input if disabled
            }
            else if (selectedValue == "OP#09972") // Tier 2
            {
                txtAmountReceivedTire1.Enabled = false;
                txtAmountReceivedTire2.Enabled = true;
                txtAmountReceivedTire1.Text = ""; // Clear Tier 1 input if disabled
            }
            else if (selectedValue == "OP#09973") // Tier 1+2
            {
                txtAmountReceivedTire1.Enabled = true;
                txtAmountReceivedTire2.Enabled = true;
            }
            else // Default selection
            {
                txtAmountReceivedTire1.Enabled = false;
                txtAmountReceivedTire2.Enabled = false;
                txtAmountReceivedTire1.Text = "";
                txtAmountReceivedTire2.Text = "";
            }
        }


        protected void cboKRA_SelectedIndexChanged_SelectedIndexChanged(object sender, EventArgs e)
        {

            cboKRA_SelectedIndexChanged();

            string ddlCraValue = ddlCra.SelectedValue;

            CRA_AND_POP_REG_NUM(ddlCraValue);

        }

        public void CRA_AND_POP_REG_NUM(string ddlCraValue)
        {
            cboKRA_SelectedIndexChanged();
             

            if (ddlCraValue == "0")
            {
                txtPopSpRegNo.Text = "6036914";
            }
            else if (ddlCraValue == "1")
            {
                txtPopSpRegNo.Text = "1171966";
            }
        }

        protected void cboKRA_SelectedIndexChanged()
        {
            //int selectedIndex = Convert.ToInt32(ddlCra.SelectedValue);
            int selectedIndex = 0;

            NpsTransactionPunchingController controller = new NpsTransactionPunchingController();
            DataTable branchData = controller.NTPCraGetBranchData(selectedIndex);

            ddlCraBranch.Items.Clear();
            foreach (DataRow row in branchData.Rows)
            {
                string branchName = row["branch_name"].ToString();
                string branchCode = row["branch_code"].ToString();
                string nsdlNo = row["NSDL_NO"].ToString();
                //ddlCraBranch.Items.Add($"{branchName.PadRight(80)} #{branchCode.PadRight(10)} #{nsdlNo}");
                ddlCraBranch.Items.Add($"{branchName.PadRight(80)}");

            }

            if (ddlCraBranch.Items.Count > 0)
            {
                ddlCraBranch.SelectedIndex = 0;
            }
        }

        protected void ddlPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            string paymentMethod = ddlPaymentMethod.SelectedValue;

            fun_PaymentModeChange(paymentMethod);
        }
        public void fun_PaymentModeChange(string paymentMethod)
        {

            // Reset field states and labels
            //txtChequeNo.Enabled = false;
            //txtChequeDated.Enabled = false;
            //ddlBankName.Enabled = false;

            lblNumber.Text = "Number";
            lblDate.Text = "Date";

            // Update field availability and labels based on payment method
            if (paymentMethod == "C")  // Cheque
            {
                txtChequeNo.Enabled = true;
                txtChequeDated.Enabled = true;
                ddlBankName.Enabled = true;
                lblNumber.Text = "Cheque No";
                lblDate.Text = "Cheque Dated";
            }
            else if (paymentMethod == "D")  // Draft
            {
                txtChequeNo.Enabled = true;
                txtChequeDated.Enabled = true;
                ddlBankName.Enabled = true;
                lblNumber.Text = "Draft No";
                lblDate.Text = "Draft Dated";
            }
            else if (paymentMethod == "E" || paymentMethod == "H")  // ECS or Corporate NON ECS
            {
                txtChequeNo.Enabled = false;
                txtChequeDated.Enabled = false;
                ddlBankName.Enabled = false;
                lblNumber.Text = "Number";
                lblDate.Text = "Date";
            }
        }

        public DateTime MasterParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return DateTime.MinValue; // or return null if you prefer

            // Define possible date formats (e.g., dd/MM/yyyy, MM/dd/yyyy, etc.)
            string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };

            // Try parsing with specified formats
            if (DateTime.TryParseExact(dateString, formats, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            // If parsing fails, return DateTime.MinValue or handle it as needed
            return DateTime.MinValue;  // or return null if that's your preference
        }

        // NEED TO APPY SHOW HIDE DIV

        protected Double Val(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (Double.TryParse(input, out Double result))
            {
                return result;
            }

            return 0; // or handle the error differently
        }

        private decimal Val2(string input)
        {
            decimal result;
            return decimal.TryParse(input, out result) ? result : 0;
        }


        protected int Req(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            if (int.TryParse(input, out int result))
            {
                return result;
            }

            return 0; // or handle the error differently
        }
        protected void ddlRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {

            /* NPS VB GST CALCULATION
             * 
             Public Sub cboRequestType_Click()
    Dim MyReqType As Double
    Dim TranAmount As Double
    Dim v_servicetax As Double

    If CDate(DtDate) < CDate("01/06/2015") Then
        v_servicetax = 0.1236
    ElseIf CDate(DtDate) >= CDate("01/06/2015") And CDate(DtDate) < CDate("15/11/2015") Then
        v_servicetax = 0.14
    ElseIf CDate(DtDate) >= CDate("15/11/2015") And CDate(DtDate) < CDate("01/06/2016") Then
        v_servicetax = 0.145
    ElseIf CDate(DtDate) >= CDate("01/06/2017") And CDate(DtDate) < CDate("01/07/2017") Then
        v_servicetax = 0.15
    ElseIf CDate(DtDate) >= CDate("01/07/2017") Then
        v_servicetax = 0.18
    End If

    If cboRequestType.Text = "" Then
        Exit Sub
    End If

    If Val(lbtrancode.Caption) > 0 Then
    MyReqType = SqlRet("select nvl(app_no,0) from transaction_st where tran_code='" & lbtrancode.Caption & "'")
    TranAmount = SqlRet("select amount from transaction_st where tran_code='" & lbtrancode.Caption & "'")
    End If

    '11,12,21 ke alawa sab me

    If Val(lbtrancode.Caption) > 0 And MyReqType = 12 And TranAmount = 0 Then '12 Contribution
    Else
        If txtregistrationno.Text = "" Then
        MsgBox "Please Select NSDL Branch First", vbInformation
        Exit Sub
        End If
        Req = Split(cboRequestType.Text, "#")
        ReqCode = Req(1)
            
        If ReqCode = "11" Or ReqCode = "12" Then   '11 SUBSCRIBER REGISTRATION
            If VarScheme = "TIRE1" Then
                TxtTire1.Enabled = True
                TxtTire2.Enabled = False
            ElseIf VarScheme = "TIRE2" Then
                TxtTire2.Enabled = True
                TxtTire1.Enabled = False
            End If
        End If
        If ReqCode = "11" And Val(TxtTire1.Text) = 0 Then '11 SUBSCRIBER REGISTRATION
            txtpopregistration1.Text = "0"
        Else
            If ReqCode = "11" Or ReqCode = "12" Then   'We do not have to calculate pop registration charge other then 11,12,21
                FreshContri = Val(TxtTire1.Text) * 0.0025
                If FreshContri < 20 Then
                FreshContri = 20
                End If
                If FreshContri >= 25000 Then
                FreshContri = 25000
                End If
                If Val(TxtTire1.Text) > 0 Then
                    If CDate(DtDate) >= CDate("01/11/2017") Then
                        txtpopregistration1.Text = 200 + FreshContri
                    Else
                        txtpopregistration1.Text = 100 + FreshContri
                    End If
                Else
                    txtpopregistration1.Text = 0
                End If
                If VarScheme = "TIRE2" Or VarScheme = "TIRE1-2" Then
                    TxtTire2.Enabled = True
                    If Val(TxtTire2.Text) = 0 Then
                        txtpopregistration2.Text = "0"
                    Else
                        txtpopregistration2.Text = "0"
                    End If
                End If
            End If
        End If
        If Val(TxtTire2) <> 0 And ReqCode = "11" Then '11 SUBSCRIBER REGISTRATION
            txtpopregistration2.Text = Val(TxtTire2.Text) * 0.0025
            If Val(txtpopregistration2.Text) < 20 Then
                txtpopregistration2.Text = 20
            End If
        End If
        If (ReqCode <> "11" And Val(TxtTire1) <> 0) Then  '11 SUBSCRIBER REGISTRATION
            If ReqCode = "11" Or ReqCode = "12" Then  'We do not have to calculate pop registration charge other then 11,12,21
                txtpopregistration1.Text = Val(TxtTire1.Text) * 0.0025
                If Val(txtpopregistration1.Text) < 20 Then
                txtpopregistration1.Text = 20
                End If
                If Val(txtpopregistration1.Text) >= 25000 Then
                txtpopregistration1.Text = 25000
                End If
            End If
        End If
        If (ReqCode <> "11" And Val(TxtTire2) <> 0) Then  '11 SUBSCRIBER REGISTRATION
            txtpopregistration2.Text = Val(TxtTire2.Text) * 0.0025
            If Val(txtpopregistration2.Text) < 20 Then
                txtpopregistration2.Text = 20
            End If
            If Val(txtpopregistration2.Text) >= 25000 Then
                txtpopregistration2.Text = 25000
            End If
        End If
        If (ReqCode <> "11" And Val(TxtTire2) <> 0) And VarScheme = "TIRE1-2" Then
        End If
        If (ReqCode <> "11" And ReqCode <> "12") Then   '11 SUBSCRIBER REGISTRATION   '12 Contribution
            TxtCollection.Visible = True
            Label22.Visible = True
            Label19.Caption = "Misclenious collection"
            Label22.Caption = "Collection Amount"
            TxtTire2.Text = "0"
            txtpopregistration1.Text = "0"
            txtpopregistration2.Text = "0"
            txtAmountInvest = "0"
            TxtTire2.Enabled = False
            txtServiceAmount.Text = Round((Val(TxtTire1.Text) * v_servicetax), 2)
            txtAmountInvest = Round(((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - ((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(txtServiceAmount.Text)))), 2) 'added by pankaj pundir on 30112018
        Else
            TxtCollection.Text = "0"
            TxtCollection.Visible = False
            Label22.Visible = False
            Label22.Caption = "Amount Invested"
            Label19.Caption = "Amount Invested"
            MiscAmt.Visible = False
            txtAmountInvest.Visible = True
            If VarScheme = "TIRE1" Or VarScheme = "TIRE1-2" Then
                TxtTire1.Enabled = True
            End If
            If (ReqCode <> "11") Then
                txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
            Else
                txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
            End If
            txtAmountInvest = Round(((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - ((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(txtServiceAmount.Text)))), 2)
        End If
        If (ReqCode <> "11") Then
            If ReqCode = "11" Or ReqCode = "12" Then
                If Val(TxtTire1.Text) > 0 Then
                    txtpopregistration1.Text = Val(TxtTire1.Text) * 0.0025
                    If Val(txtpopregistration1.Text) < 20 Then
                    txtpopregistration1.Text = 20
                    End If
                    If Val(txtpopregistration1.Text) >= 25000 Then
                    txtpopregistration1.Text = 25000
                    End If
                Else
                    txtpopregistration1.Text = 0
                End If
                
                txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)) * v_servicetax), 2)
                txtAmountInvest.Text = Abs(Round((Val(TxtTire1.Text) + Val(TxtTire2.Text)) - (Val(txtServiceAmount.Text) + Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)), 2))
                
            End If
        End If
        If Val(TxtCollection.Text) > 0 Then
            txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text) + Val(TxtCollection.Text)) * v_servicetax), 2)
            txtAmountInvest.Text = Round((Val(TxtCollection.Text)) - (Val(txtServiceAmount.Text) + Val(txtpopregistration1.Text) + Val(txtpopregistration2.Text)), 2)
        End If
    End If
    If OptCorporate.Value = True And ReqCode = 11 Then
        txtpopregistration1.Text = Round((Val(TxtTire1.Text) / (100 + v_servicetax * 100)) * 100, 2)
        txtServiceAmount.Text = Round(((Val(txtpopregistration1.Text)) * v_servicetax), 2)
        txtAmountInvest.Text = 0
    End If
End Sub
             */

            try
            {

                Fun_OnChangeRequestType(); // Call the function to handle request type change
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
            finally
            {
            }
        }

        protected void Fun_OnReqTypeChangeByDirect()
        {
            try
            {
                // 11 = SUBSCRIPTION REGISTRATION
                // 12 = CONTRIBUTION
                // t1 = OP#09971
                // t2 = OP#09972
                // t1+2 = OP#09973

                #region NEW FUNCTIONALITY  ON REQRUEST TYPE CHAGNE
                string VarScheme = ddlScheme.SelectedValue;

                #region Tire enable disabe by schemas
                if (VarScheme == "OP#09971")
                {
                    VarScheme = "TIRE1";
                    txtAmountReceivedTire1.Enabled = true;
                    txtAmountReceivedTire2.Enabled = false;
                }

                else if (VarScheme == "OP#09972")
                {
                    VarScheme = "TIRE2";
                    txtAmountReceivedTire2.Enabled = true;
                    txtAmountReceivedTire1.Enabled = false;


                }
                else if (VarScheme == "OP#09973")
                {
                    VarScheme = "TIRE12";
                    txtAmountReceivedTire1.Enabled = true;
                    txtAmountReceivedTire2.Enabled = true;
                }
                #endregion

                Double MyReqType = 0;
                Double TranAmount = 0;
                Double v_servicetax = 0;
                Double FreshContri = 0;

                string onlyDate = DateTime.Now.ToString("yyyy-MM-dd");
                DateTime DtDate = MasterParseDate(onlyDate);
                int ReqCode = Req(ddlRequestType.SelectedValue);

                #region calulate service tax by date
                if (DtDate < MasterParseDate("01/06/2015"))
                {
                    v_servicetax = 0.1236;
                }
                else if (DtDate >= MasterParseDate("01/06/2015") && DtDate < MasterParseDate("15/11/2015"))
                {
                    v_servicetax = 0.14;
                }
                else if (DtDate >= MasterParseDate("15/11/2015") && DtDate < MasterParseDate("01/06/2016"))
                {
                    v_servicetax = 0.145;
                }
                else if (DtDate >= MasterParseDate("01/06/2017") && DtDate < MasterParseDate("01/07/2017"))
                {
                    v_servicetax = 0.15;
                }
                else if (DtDate >= MasterParseDate("01/07/2017"))
                {
                    v_servicetax = 0.18;
                }

                #endregion
                if (string.IsNullOrEmpty(ddlRequestType.SelectedValue))
                {
                    return;
                }
                if (Val(lblArNo.Text) > 0)
                {
                    #region MyReqType Get Set 
                    string sql1 = "select nvl(app_no,0) as app_no from transaction_st where tran_code='" + lblArNo.Text + "'";
                    DataTable dt1 = pc.ExecuteCurrentQueryMaster(sql1, out int rc1, out string ie1);
                    if (rc1 > 0 && string.IsNullOrEmpty(ie1))
                    {
                        DataRow dt1Row = dt1.Rows[0];
                        MyReqType = Convert.ToDouble(dt1Row["app_no"]);
                    }
                    #endregion

                    #region TranAmount Get Set
                    string sql2 = "select amount from transaction_st where tran_code='" + lblArNo.Text + "'";
                    DataTable dt2 = pc.ExecuteCurrentQueryMaster(sql2, out int rc2, out string ie2);
                    if (rc2 > 0 && string.IsNullOrEmpty(ie2))
                    {
                        DataRow dt2Row = dt2.Rows[0];
                        TranAmount = Convert.ToDouble(dt2Row["amount"]);
                    }
                    #endregion
                }
                if (Val(lblArNo.Text) > 0 && MyReqType == 12 && TranAmount == 0) // 12 Contribution
                {

                }
                else
                {
                    if (string.IsNullOrEmpty(txtPopSpRegNo.Text))
                    {
                        pc.ShowAlert(this, "Please Select NSDL Branch First");
                        return;
                    }

                    if (ReqCode == 11 || ReqCode == 12) // 11 SUBSCRIBER REGISTRATION
                    {
                        if (VarScheme == "TIRE1")
                        {
                            txtAmountReceivedTire1.Enabled = true;
                            txtAmountReceivedTire2.Enabled = false;
                        }
                        else if (VarScheme == "TIRE2")
                        {
                            txtAmountReceivedTire2.Enabled = true;
                            txtAmountReceivedTire1.Enabled = false;
                        }
                    }

                    if (ReqCode == 11 && (Val(txtAmountReceivedTire1.Text) == 0)) // 11 SUBSCRIBER REGISTRATION
                    {
                        txtPopRegistrationChargesOneTime.Text = "0";
                    }
                    else
                    {
                        if (ReqCode == 11 || ReqCode == 12) // We do not have to calculate pop registration charge other then 11,12,21
                        {
                            FreshContri = Val(txtAmountReceivedTire1.Text) * 0.0025;

                            if (FreshContri < 20)
                            {
                                FreshContri = 20;
                            }

                            if (FreshContri >= 25000)
                            {
                                FreshContri = 25000;
                            }

                            if (Val(txtAmountReceivedTire1.Text) > 0)
                            {
                                if (DtDate >= MasterParseDate("01/11/2017"))
                                {
                                    txtPopRegistrationChargesOneTime.Text = (200 + FreshContri).ToString();
                                }
                                else
                                {
                                    txtPopRegistrationChargesOneTime.Text = (100 + FreshContri).ToString();
                                }
                            }
                            else
                            {
                                txtPopRegistrationChargesOneTime.Text = "0";
                            }

                            if (VarScheme == "TIRE2" || VarScheme == "TIRE12")
                            {
                                txtAmountReceivedTire2.Enabled = true;
                                if (Val(txtAmountReceivedTire2.Text) == 0)
                                {
                                    txtPopRegistrationCharges.Text = "0";
                                }
                                else
                                {
                                    txtPopRegistrationCharges.Text = "0";
                                }
                            }
                        }
                    }


                    if (Val(txtAmountReceivedTire2.Text) != 0 && ReqCode != 11) // 11 subscriber registration
                    {
                        txtPopRegistrationCharges.Text = (Val(txtAmountReceivedTire2.Text) * 0.0025).ToString();
                        if (Val(txtPopRegistrationCharges.Text) < 20)
                        {
                            txtPopRegistrationCharges.Text = "20";
                        }
                    }

                    if (ReqCode != 11 && Val(txtAmountReceivedTire1.Text) != 0) // 11 SUBSCRIBER REGISTRATION
                    {
                        if (ReqCode == 11 || ReqCode == 12)
                        {
                            // We do not have to calculate pop registration charge other than 11,12,21
                            txtPopRegistrationChargesOneTime.Text = (Val(txtAmountReceivedTire1.Text) * 0.0025).ToString();

                            if (Val(txtPopRegistrationChargesOneTime.Text) < 20)
                            {
                                txtPopRegistrationChargesOneTime.Text = "20";
                            }

                            if (Val(txtPopRegistrationChargesOneTime.Text) >= 25000)
                            {
                                txtPopRegistrationChargesOneTime.Text = "25000";
                            }
                        }
                    }

                    if (ReqCode != 11 && Val(txtAmountReceivedTire2.Text) != 0) // 11 SUBSCRIBER REGISTRATION
                    {
                        txtPopRegistrationCharges.Text = (Val(txtAmountReceivedTire2.Text) * 0.0025).ToString();
                        if (Val(txtPopRegistrationCharges.Text) < 20)
                        {
                            txtPopRegistrationCharges.Text = "20";
                        }

                        if (Val(txtPopRegistrationCharges.Text) >= 25000)
                        {
                            txtPopRegistrationCharges.Text = "25000";
                        }
                    }

                    if (ReqCode != 11 && Val(txtAmountReceivedTire2.Text) != 0 && VarScheme == "TIRE12")
                    {

                    }

                    if (ReqCode != 11 && ReqCode != 12)
                    {
                        // 11 SUBSCRIBER REGISTRATION, 12 Contribution

                        txtCollectionAmount.Enabled = true;
                        lblInvestorAmountn.Text = "Misleanious Amount";
                        txtAmountReceivedTire2.Text = "0";
                        txtAmountReceivedTire2.Enabled = false;
                        txtPopRegistrationChargesOneTime.Text = "0";
                        txtPopRegistrationCharges.Text = "0";
                        txtAmountInvested.Text = "0";

                        Double tier1Value = Val(txtAmountReceivedTire1.Text);
                        Double tier2Value = Val(txtAmountReceivedTire2.Text);
                        Double regCharge1 = Val(txtPopRegistrationChargesOneTime.Text);
                        Double regCharge2 = Val(txtPopRegistrationCharges.Text);


                        Double calculateGsTServiceValue = Math.Round((tier1Value * v_servicetax), 2);
                        txtGst.Text = calculateGsTServiceValue.ToString();

                        Double gstValue = Val(txtGst.Text);
                        Double calculateAmtInv = Math.Round(((tier1Value + tier2Value) - (regCharge1 + regCharge2 + gstValue)), 2);
                        txtAmountInvested.Text = calculateAmtInv.ToString();

                    }
                    else
                    {
                        txtCollectionAmount.Text = "0";
                        txtCollectionAmount.Enabled = false;
                        lblInvestorAmountn.Text = "Amount Invested";
                        txtAmountInvestedAdditional.Enabled = false;
                        txtAmountInvested.Enabled = true;

                        if (VarScheme == "TIRE1" || VarScheme == "TIRE12")
                        {
                            txtAmountReceivedTire1.Enabled = true;
                        }


                        Double tier1Value = Val(txtAmountReceivedTire1.Text);
                        Double tier2Value = Val(txtAmountReceivedTire2.Text);
                        Double regCharge1 = Val(txtPopRegistrationChargesOneTime.Text);
                        Double regCharge2 = Val(txtPopRegistrationCharges.Text);
                        if (ReqCode != 11)
                        {
                            txtGst.Text = Math.Round((regCharge1 + regCharge2 * v_servicetax), 2).ToString();
                        }
                        else
                        {
                            txtGst.Text = Math.Round((regCharge1 + regCharge2 * v_servicetax), 2).ToString();
                        }

                        Double gstValue = Val(txtGst.Text);
                        Double calInvValue = Math.Round(((tier1Value + tier2Value) - (regCharge1 + regCharge2 + gstValue)), 2);
                        txtAmountInvested.Text = calInvValue.ToString();
                    }

                    if (ReqCode != 11)
                    {
                        if (ReqCode == 11 || ReqCode == 12)
                        {
                            if (Val(txtAmountReceivedTire1.Text) > 0)
                            {
                                txtPopRegistrationChargesOneTime.Text = (Val(txtAmountReceivedTire1.Text) * 0.0025).ToString();
                                if (Val(txtPopRegistrationChargesOneTime.Text) < 20)
                                {
                                    txtPopRegistrationChargesOneTime.Text = "20";
                                }
                                if (Val(txtPopRegistrationChargesOneTime.Text) >= 25000)
                                {
                                    txtPopRegistrationChargesOneTime.Text = "25000";
                                }
                            }
                            else
                            {
                                txtPopRegistrationChargesOneTime.Text = "0";
                            }

                            Double tier1Value = Val(txtAmountReceivedTire1.Text);
                            Double tier2Value = Val(txtAmountReceivedTire2.Text);
                            Double regCharge1 = Val(txtPopRegistrationChargesOneTime.Text);
                            Double regCharge2 = Val(txtPopRegistrationCharges.Text);

                            Double calGst = Math.Round((regCharge1 + regCharge2 * v_servicetax), 2);
                            txtGst.Text = calGst.ToString();
                            Double getGstValue = Val(txtGst.Text);

                            Double calInvAmt = Math.Round(((tier1Value + tier2Value) - (getGstValue + regCharge1 + regCharge2)), 2);
                            txtAmountInvested.Text = Math.Abs(calInvAmt).ToString();
                        }
                    }

                    if (Val(txtCollectionAmount.Text) > 0)
                    {
                        Double tier1Value = Val(txtAmountReceivedTire1.Text);
                        Double tier2Value = Val(txtAmountReceivedTire2.Text);
                        Double collValue = Val(txtCollectionAmount.Text);
                        Double regCharge1 = Val(txtPopRegistrationChargesOneTime.Text);
                        Double regCharge2 = Val(txtPopRegistrationCharges.Text);

                        Double calGst = Math.Round(((regCharge1 + regCharge2 + collValue) * v_servicetax), 2);
                        txtGst.Text = calGst.ToString();
                        Double getGstValue = Val(txtGst.Text);

                        Double calInvAmt = Math.Round(((collValue) - (getGstValue + regCharge1 + regCharge2)), 2);
                        txtAmountInvested.Text = Math.Abs(Math.Round(calInvAmt, 2)).ToString();
                    }
                }

                // ddlType.SelectedValue = "0"= Inv, "1" = Cor;
                if (ddlType.SelectedValue == "1" && ReqCode == 11)
                {
                    Double tier1Value = Val(txtAmountReceivedTire1.Text);
                    Double tier2Value = Val(txtAmountReceivedTire2.Text);
                    Double collValue = Val(txtCollectionAmount.Text);
                    Double regCharge1 = Val(txtPopRegistrationChargesOneTime.Text);
                    Double regCharge2 = Val(txtPopRegistrationCharges.Text);

                    Double calRegCharge1 = Math.Round(((tier1Value / (100 + v_servicetax * 100)) * 100), 2);
                    txtPopRegistrationChargesOneTime.Text = calRegCharge1.ToString();

                    Double calGst = Math.Round((calRegCharge1 * v_servicetax), 2);
                    txtGst.Text = calGst.ToString();
                    txtAmountInvested.Text = "0";
                }
                #endregion

            }
            catch (Exception e)
            {
                pc.ShowAlert(this, e.Message);
                return;
            }

        }

        protected void Fun_OnReqTypeChangeByPsm()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(ddlRequestType.SelectedValue))
                {
                    pc.ShowAlert(this, "Please select a request type.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlScheme.SelectedValue))
                {
                    pc.ShowAlert(this, "Please select a scheme.");
                    return;
                }

                // Get form values with proper validation
                int reqCode = Convert.ToInt32(ddlRequestType.SelectedValue);
                string scheme = ddlScheme.SelectedValue;
                decimal amountT1 = Val2(txtAmountReceivedTire1.Text);
                decimal amountT2 = Val2(txtAmountReceivedTire2.Text);
                decimal collectionAmt = Val2(txtCollectionAmount.Text);
                DateTime now = DateTime.Now;

                // Get AR No if available (from your original code)
                long arNo = (!string.IsNullOrEmpty(lblArNo.Text)? Convert.ToInt64(lblArNo.Text):0);
                int type = ddlType.SelectedValue == "1" ? 1 : 0; // 0=Inv, 1=Cor

                // Call the calculation service
                NpsTransactionPunchingController nps = new NpsTransactionPunchingController();
                var investmentData = nps.CALCULATE_INVESTMENT_DETAILS(
                    reqCode,
                    scheme,
                    amountT1,
                    amountT2,
                    collectionAmt,
                    now,
                    type,
                    arNo);

                // Check for errors from the procedure
                if (investmentData["status"] != 0)
                {
                    string errorMsg = $"Calculation error (Code: {investmentData["status"]})";
                    if (investmentData.ContainsKey("error_msg") && investmentData["error_msg"] != 0)
                    {
                        errorMsg += $": {investmentData["error_msg"]}";
                    }
                    pc.ShowAlert(this, errorMsg);
                    return;
                }

                // Update UI elements with calculated values
                txtPopRegistrationChargesOneTime.Text = investmentData["pop_reg_charge_ot"].ToString("F2");
                txtPopRegistrationCharges.Text = investmentData["pop_reg_charge"].ToString("F2");
                txtGst.Text = investmentData["gst"].ToString("F2");
                txtAmountInvested.Text = investmentData["invested"].ToString("F2");

                // Additional UI updates based on scheme type (from your original code)
                if (scheme == "OP#09971") // TIRE1
                {
                    txtAmountReceivedTire1.Enabled = true;
                    txtAmountReceivedTire2.Enabled = false;
                }
                else if (scheme == "OP#09972") // TIRE2
                {
                    txtAmountReceivedTire2.Enabled = true;
                    txtAmountReceivedTire1.Enabled = false;
                }
                else if (scheme == "OP#09973") // TIRE12
                {
                    txtAmountReceivedTire1.Enabled = true;
                    txtAmountReceivedTire2.Enabled = true;
                }

                // Handle label text based on request type (from your original code)
                if (reqCode != 11 && reqCode != 12)
                {
                    lblInvestorAmountn.Text = "Miscellaneous Amount";
                    txtCollectionAmount.Enabled = true;
                }
                else
                {
                    lblInvestorAmountn.Text = "Amount Invested";
                    txtCollectionAmount.Enabled = false;
                }
            }
            catch (FormatException ex)
            {
                pc.ShowAlert(this, "Invalid input format. Please check your values.");
                //pc.ShowAlert(this, ex.Message + "Format exception in Fun_OnReqTypeChangeByPsm");
                return;
            }
            catch (OverflowException ex)
            {
                pc.ShowAlert(this, "Value is too large. Please enter smaller amounts.");
                //pc.ShowAlert(this, ex.Message + "Overflow exception in Fun_OnReqTypeChangeByPsm");
                return;

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, $"An unexpected error occurred: {ex.Message}");
                //pc.ShowAlert(this, ex.Message + "Unexpected error in Fun_OnReqTypeChangeByPsm");
                return;

            }
        }
        protected void Fun_OnReqTypeChangeByPsm1()
        {
            try
            {
                if (string.IsNullOrEmpty(ddlRequestType.SelectedValue))
                {
                    pc.ShowAlert(this, "Please select a request type.");
                    return;
                }
                // Assume these values are from the form
                int reqCode = Convert.ToInt32(ddlRequestType.SelectedValue);
                string scheme = ddlScheme.SelectedValue;
                decimal amountT1 = Val2(txtAmountReceivedTire1.Text);
                decimal amountT2 = Val2(txtAmountReceivedTire2.Text);
                decimal collectionAmt = Val2(txtCollectionAmount.Text);
                DateTime now = DateTime.Now;
                
                NpsTransactionPunchingController nps = new NpsTransactionPunchingController();
                var investmentData = nps.CALCULATE_INVESTMENT_DETAILS(reqCode, scheme, amountT1, amountT2, collectionAmt, now);

                // Assign outputs to TextBoxes
                txtPopRegistrationChargesOneTime.Text = investmentData["pop_reg_charge_ot"].ToString("F2");
                txtPopRegistrationCharges.Text = investmentData["pop_reg_charge"].ToString("F2");
                txtGst.Text = investmentData["gst"].ToString("F2");
                txtAmountInvested.Text = investmentData["invested"].ToString("F2");

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, ex.Message);
                return;
            }
            finally
            {
            }
        }

        protected void Fun_OnChangeRequestType()
        {
            try
            {
                //Fun_OnReqTypeChangeByDirect();
                Fun_OnReqTypeChangeByPsm();
                           }
            catch(Exception ex)
            {

            }
        }

        public double GetTextNumericValue(string txt, int decimalPlaces = 2)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return 0;

            if (double.TryParse(txt.Trim(), out double result))
                return Math.Round(result, decimalPlaces); // Round to specified decimal places

            return 0;
        }



        #endregion

        // BUTTON CLICK EVENTS
        #region Button Section Cliect Events

        protected void btnCmdShow(object sender, EventArgs e)
        {
            fetchSetByDtInv(txtDtNumber.Text, true);
        }

        public void fetchSetByDtInv(string code, bool isDt)
        {

            try
            {
                if (string.IsNullOrEmpty(pc.currentLoginID()))
                {
                    pc.RedirectToWelcomePage();
                    return;
                }

                else
                {
                    string dtNumber = txtDtNumber.Text.Trim();
                    if (isDt && string.IsNullOrEmpty(dtNumber))
                    {
                        pc.ShowAlert(this, "Enter DT Number");
                        txtDtNumber.Focus();
                        return;
                    }

                    else
                    {
                        string sql1 = "";
                        DataTable dt1 = new DataTable();

                        string dtOrInv = isDt ? " common_id " : " inv_code ";
                        sql1 = @"
select 
nvl(ar_code, null)              as ar,
inv_code                        as inv,
nvl(common_id, null)            as dt,  
nvl(VERIFICATION_FLAG, null)    as vf,  
nvl(PUNCHING_FLAG, null)        as pf, 
nvl(REJECTION_STATUS, null)     as rf,  
nvl(TRAN_TYPE, null)            as tt,
nvl(BUSI_BRANCH_CODE,0)         as bss_branch,
nvl(BUSI_RM_CODE,0)             as bss_rm

from tb_doc_upload where " + dtOrInv + " = "+ code +" and tran_type='FI' ";

                        dt1 = pc.ExecuteCurrentQueryMaster(sql1, out int rc1, out string ie1);
                        if (!string.IsNullOrEmpty(ie1))
                        {
                            pc.ShowAlert(this, ie1);
                            return;
                        }

                        else if(rc1>0)
                        {
                            DataRow dt1Row = dt1.Rows[0];

                            string dt1_dt = dt1Row["dt"]?.ToString();
                            string dt1_rf = dt1Row["rf"]?.ToString();
                            string dt1_vf = dt1Row["vf"]?.ToString();
                            string dt1_tt = dt1Row["tt"]?.ToString();
                            string dt1_pf = dt1Row["pf"]?.ToString();
                            string dt1_ar = dt1Row["ar"]?.ToString();
                            string dt1_inv = dt1Row["inv"]?.ToString();
                            string dt1_bss_branch = dt1Row["bss_branch"]?.ToString();
                            string dt1_bss_rm = dt1Row["bss_rm"]?.ToString();

                            #region CHECKING DT NUMBER AND RETURN

                            if (isDt)
                            {


                                if (dt1_rf == "1")
                                {
                                    pc.ShowAlert(this, $"DT {dtNumber} is rejected!");
                                    txtDtNumber.Text = string.Empty;
                                    txtDtNumber.Focus();
                                    return;
                                }

                                if (dt1_vf != "1")
                                {
                                    pc.ShowAlert(this, $"DT {dtNumber} is not verified!");
                                    txtDtNumber.Text = string.Empty;
                                    txtDtNumber.Focus();
                                    return;
                                }

                                if (dt1_pf == "none")
                                {
                                    pc.ShowAlert(this, $"DT {dtNumber} is already punched!");
                                    txtDtNumber.Text = string.Empty;
                                    txtDtNumber.Focus();
                                    return;
                                }

                            }

                            if (!string.IsNullOrEmpty(dt1_inv) || dt1_inv != "0")
                            {
                                string sql2 = "select inv_code as code,investor_name as name from investor_master where inv_code='" + dt1_inv + "'";
                                DataTable dt2 = pc.ExecuteCurrentQueryMaster(sql2, out int ir2, out string ie2);

                                if (dt2.Rows.Count > 0 && string.IsNullOrEmpty(ie2))
                                {
                                    DataRow dt2_row = dt2.Rows[0];
                                    string dt2_inv = dt2_row["code"]?.ToString();
                                    string dt2_name = dt2_row["name"]?.ToString();
                                    txtInvestorCode.Text = dt2_inv;
                                    txtNameOfSubscriber.Text = dt2_name;
                                }

                                if(!isDt && !string.IsNullOrEmpty(dt1_dt))
                                {
                                    txtDtNumber.Text = dt1_dt;
                                }
                                txtBusinessRm.Text = dt1_bss_rm;
                                BranchFill();
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, "Error " + ex.Message);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideServerLoader", "hideServerLoader();", true);
                return;
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "hideServerLoader", "hideServerLoader();", true);
            }

        }

        protected void BranchFill()
        {
            string payrollId = txtBusinessRm.Text.Trim();
            if (string.IsNullOrEmpty(payrollId))
            {
                return;
            }

            else
            {
                try
                {
                    string q1 = "SELECT category_id FROM employee_master WHERE payroll_id = '" + payrollId + "' AND type = 'A'";
                    DataTable dt1 = pc.ExecuteCurrentQuery(q1);

                    if (dt1.Rows.Count > 0)
                    {
                        string category_id = dt1.Rows[0]["category_id"].ToString();

                        if (category_id != null && category_id != "" && category_id != "2001" && category_id != "2018")
                        {
                            pc.ShowAlert(this, "Rm Should be Sales and Support Only");
                            txtBusinessRm.Text = "";
                            ddlBusinessBranch.Items.Clear();
                            return;
                        }

                    }

                    // If payrollId length is 5 or 6, fetch branch details
                    if (payrollId.Length == 5 || payrollId.Length == 6)
                    {
                        ddlBusinessBranch.Items.Clear();

                        string strbranch = "";// "brnachName #1000001";
                        if (!string.IsNullOrEmpty(strbranch))
                        {
                            //ddlBusinessBranch.Items.Add(new ListItem(strbranch));
                        }

                        string q2 = "Select source,branch_name from employee_master e,branch_master b where e.payroll_id='" + payrollId + "' and e.source=b.branch_code and (e.type='A' or e.type is null)";
                        DataTable dt2 = pc.ExecuteCurrentQuery(q2);

                        if (dt2.Rows.Count > 0)
                        {
                            DataRow dt2_row = dt2.Rows[0];
                            string branchCode = dt2_row["source"].ToString();
                            string branchName = dt2_row["branch_name"].ToString();

                            if (!string.IsNullOrEmpty(strbranch))
                            {
                                string strBrnachSelectedValue = ddlBusinessBranch.SelectedValue;
                                if (strBrnachSelectedValue != branchCode) 
                                {
                                    //ddlBusinessBranch.Items.Add(new ListItem(branchName + new string(' ', 100) + "#" + branchCode));
                                    ddlBusinessBranch.Items.Add(new ListItem( branchName , branchCode));
                                }
                            }
                            else
                            {
                                ddlBusinessBranch.Items.Add(new ListItem(branchName, branchCode));
                            }
                            ddlBusinessBranch.SelectedIndex = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    pc.ShowAlert(this, ex.Message);
                }
            }
        }


        protected void btnSearchAR_Click(object sender, EventArgs e)
        {
            Session["AR_FIND_TR"] = "NPS";
            Response.Redirect("~/Tree/FindTransaction.aspx", true);
        }
        
        protected void btnSearchInvestor_Click(object sender, EventArgs e)
        {
            string loggedinUser = Session["LoginId"]?.ToString();

            if (!string.IsNullOrEmpty(loggedinUser))
            {

                //Session["AC_FIND"] = "INVESTOR";
                //Response.Redirect("../Tree/frm_tree_mf");

                string redirectUrl = "../Tree/frm_tree_mf?SEARCH_TYPE=TRANSACTION_INV";
                Response.Redirect(redirectUrl);
            }
            else
            {
                pc.RedirectToWelcomePage();
            }
        }

        protected void btnAddNewBank_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Masters/addnewbank.aspx"); // Redirect to home page or any other page

        }



        protected void SaveButton_Click(object sender, EventArgs e)
        {
            string Busi_Branch_cd = string.Empty;
            string Busi_Rm_Cd = string.Empty;
            string ClientBranchCode = string.Empty;
            string ClientRmCode = string.Empty;
            string paymode = string.Empty;
            string invCode = string.Empty;
            string dipo_type = string.Empty;
            string Pay_type = string.Empty;
            string str_test = string.Empty;
            string MyTranCode = string.Empty;
            string MyGSTNO = string.Empty;
            string MyTranCode1 = string.Empty;
            string MySecReq = string.Empty;
            string Vclientcategory = string.Empty;
            string selectedCorporateValue = ddlType.SelectedValue;
            string selectedCorporateName = corporateName.Text; // .Text gives the actual value from the TextBox
            string selecteScheme = ddlScheme.SelectedValue;
            string currentAR = lblArNo.Text.ToString();
            string currentDT = txtDtNumber.Text.ToString();

            /*
             Punching id
            index ; 0   : GlbroleId = 212, 1,261
            index ; 4   : GlbroleId = 146, 1            
             */

            if (!string.IsNullOrEmpty(currentAR))
            {
                pc.ShowAlert(this, "Transaction already exist.");
                return;
            }

            if (string.IsNullOrEmpty(txtDtNumber.Text))
            {
                string msg = "DT No cannot be left blank";
                pc.ShowAlert(this, msg);
                txtDtNumber.Focus();
                return;
            }

          

            if (selectedCorporateValue == "1" && string.IsNullOrEmpty(selectedCorporateName))
            {
                pc.ShowAlert(this, "Corporate name is required.");
                corporateName.Focus();
                return;
            }

            if (chkZeroCommission.Checked)
            {
                if (txtPran.Text.Trim() != "")
                {
                    //if (SqlRet($"SELECT COUNT(*) FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO='{ar.Text}'") >= 1)

                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('FATCA for this PRAN is non-compliant. Please contact product team for the same.');", true);
                    return;
                }
            }

            if (string.IsNullOrEmpty(selecteScheme))
            {
                pc.ShowAlert(this, "Scheme is required.");
                ddlScheme.Focus();
                return;
            }


            InsertUpdate("0");


        }

        protected void ModifyButton_Click(object sender, EventArgs e)
        {
            string tran_code = lblArNo.Text.ToString();
            string selectedCorporateValue = ddlType.SelectedValue;
            string selectedCorporateName = corporateName.Text;  
            string selecteScheme = ddlScheme.SelectedValue;

            if (string.IsNullOrEmpty(tran_code) || tran_code == "0")
            {
                pc.ShowAlert(this, "Load a transasction");
                lblArNo.Focus();
                return;
            }

            else if (selectedCorporateValue == "1" && string.IsNullOrEmpty(selectedCorporateName))
            {
                pc.ShowAlert(this, "Corporate name is required.");
                corporateName.Focus();
                return;
            }

            else if(string.IsNullOrEmpty(selecteScheme) || selecteScheme == "0")
            {
                pc.ShowAlert(this, "Scheme is required.");
                ddlScheme.Focus();
                return;
            }
            else
            {
                InsertUpdate("4");
            }

        }

        protected void ResetButton_Click(object sender, EventArgs e)
        {
            ResetMain();
        }



        protected void btnImportCorporateNonEsc_Click(object sender, EventArgs e)
        {
            Session["Nps_Importing_flag"] = "NON_ECS";
            if (chkZeroCommission.Checked)
            {
                Session["VChkZeroCommission"] = "Y";
            }
            else
            {
                Session["VChkZeroCommission"] = "N";
            }

            Response.Redirect("~/Tree/ImportExportExcel.aspx", false);
        }

        protected void btnImportEcs_Click(object sender, EventArgs e)
        {
            Session["Nps_Importing_flag"] = "ECS";
            Session["VChkZeroCommission"] = "N";

            Response.Redirect("~/Tree/ImportExportExcel.aspx", false);


        }
        protected void ExitButton_Click(object sender, EventArgs e)
        {
            pc.RedirectToWelcomePage();
        }

        public void PrintButton_Click(object sender, EventArgs e)
        {

            string receiptNo = txtReceiptNo.Text;

            printData(receiptNo);

        }


        #endregion


        #region Helper Methods GET, SET , RESET, INSERT, UPDATE

        public void fun_GetByDT_O()
        {


            string dtNumberValue = txtDtNumber.Text.Trim();
            string loginID = Session["LoginId"]?.ToString();

            int dtCount1 = 0;
            int dtCount2 = 0;
            int dtCount3 = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(dtNumberValue))
                {
                    string msg = "Enter DT Number";
                    pc.ShowAlert(this, msg);
                    return;
                }

                DataTable dt1 = new NpsTransactionPunchingController().GET_AR_BY_DTTS(dtNumberValue, null, false, loginID);
                dtCount1 = dt1.Rows.Count;
                if (dtCount1 > 0)
                {
                    ResetMain();
                    txtDtNumber.Text = dtNumberValue;



                    DataRow row = dt1.Rows[0];
                    string message_1_value = GetDBStringValue(row, "message");

                    if (message_1_value.Contains("Validity: DT is valid"))
                    {
                        string tb_f_ar_value = GetDBStringValue(row, "tb_f_ar");
                        string tb_f_inv_value = GetDBStringValue(row, "tb_f_inv");
                        string tb_f_bss_br_value = GetDBStringValue(row, "tb_f_bss_br");
                        string tb_f_bss_rm_value = GetDBStringValue(row, "tb_f_bss_rm");
                        string tb_f_common_value = GetDBStringValue(row, "tb_f_common");
                        string tb_f_rej_value = GetDBStringValue(row, "tb_f_rej");
                        string tb_f_ver_value = GetDBStringValue(row, "tb_f_ver");
                        string tb_f_pun_value = GetDBStringValue(row, "tb_f_pun");
                        string tb_f_sch_value = GetDBStringValue(row, "tb_f_sch");
                        string im_inv_name_value = GetDBStringValue(row, "im_inv_name");
                        string im_inv_code_value = GetDBStringValue(row, "im_inv_code");

                        if (
                            tb_f_ar_value == "0" || tb_f_ar_value == null &&
                            tb_f_rej_value == "0" || tb_f_rej_value == null &&
                            tb_f_ver_value == "1" &&
                            tb_f_pun_value == "0" || tb_f_pun_value == null)
                        {
                            txtDtNumber.Text = dtNumberValue;
                            lblArNo.Text = string.Empty;
                            txtInvestorCode.Text = im_inv_code_value;
                            try
                            {

                                ddlBusinessBranch.SelectedValue = tb_f_bss_br_value;
                            }
                            catch (Exception ex)
                            {

                            }
                            txtBusinessRm.Text = tb_f_bss_rm_value;
                            txtNameOfSubscriber.Text = im_inv_name_value;
                            btnSave.Enabled = true;
                            btnModify.Enabled = false;
                            return;

                        }

                        // If AR exists, proceed with checking transaction in temp and st tables
                        if (!string.IsNullOrEmpty(tb_f_ar_value) || tb_f_ar_value != "0")
                        {
                            // Check for AR in temp transaction table
                            DataTable dtIfArExistForTempTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tb_f_ar_value, true, loginID);
                            DataTable dtIfArExistForStTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tb_f_ar_value, false, loginID);


                            dtCount2 = dtIfArExistForTempTran.Rows.Count > 0 ? 1 : 0;
                            dtCount3 = dtIfArExistForStTran.Rows.Count > 0 ? 1 : 0;


                            if (dtCount2 == 0) // No data in temp
                            {
                                if (dtCount3 > 0) // Data exists in st
                                {
                                    DataRow row3 = dtIfArExistForStTran.Rows[0];

                                    string message_3_value = GetDBStringValue(row3, "message");
                                    if (message_3_value.Contains("Validity: Transaction data exist in st"))
                                    {

                                        lblArNo.Text = tb_f_ar_value;
                                        txtInvestorCode.Text = tb_f_inv_value;
                                        try
                                        {

                                            ddlBusinessBranch.SelectedValue = tb_f_bss_br_value;
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        txtBusinessRm.Text = tb_f_bss_rm_value;
                                        txtNameOfSubscriber.Text = im_inv_name_value;
                                        SetFieldData(row3);
                                        btnSave.Enabled = false;
                                        btnModify.Enabled = true;
                                        txtDtNumber.Text = dtNumberValue;
                                        txtBusinessRm.Enabled = false;

                                    }
                                }
                                else
                                {
                                    // ShowAlert(dbMsg);

                                    pc.ShowAlert(this, "No data in transaction st");
                                }
                            }

                        }
                    }

                    else if (message_1_value.Contains("Validity: DT is valid") && (dtCount3 == 0))
                    {
                        pc.ShowAlert(this, "Only DT exist but not transaction");
                    }
                    else
                    {
                        pc.ShowAlert(this, message_1_value);
                    }

                }
                else
                {
                    pc.ShowAlert(this, "No data found for the given DT number");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or show error message to the user
                pc.ShowAlert(this, ex.Message);
            }
        }

        public void printData(string receiptNo)
        {

            if (string.IsNullOrEmpty(receiptNo))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please Enter Receipt No First');", true);
                return;
            }

            try
            {
                DataTable receiptData = new WM.Controllers.NpsTransactionPunchingController().PrintReceiptData(receiptNo);
                if (receiptData != null && receiptData.Rows.Count > 0)
                {
                    DataRow row = receiptData.Rows[0];

                    // Populate all required fields for modal and print sheet
                    popSaveBUSINESS_RMCODE.Text = GetSafeString(row["BUSINESS_RMCODE"]);
                    popSaveTRAN_ID.Text = GetSafeString(row["TRAN_CODE"]);
                    popSaveLOGGEDUSERID.Text = GetSafeString(row["LOGGEDUSERID"]);
                    popSaveAMOUNT1.Text = GetSafeDecimal(row["AMOUNT"]).ToString("N2");
                    popSaveAMOUNT2.Text = GetSafeDecimal(row["AMT1"]).ToString("N2");
                    popSaveTRANCODE.Text = GetSafeString(row["PAYMENT_MODE"]);
                    popSaveTRAN_SRC.Text = GetSafeString(row["TRAN_SRC"]);
                    popSaveREG_TRANTYPE.Text = GetSafeString(row["REG_TRANTYPE"]);
                    popSaveREG_recieptnoT.Text = GetSafeString(row["UNIQUE_ID"]);
                    popTRDATE.Text = GetSafeString(row["TR_DATE"]);

                    // Show the modal and trigger print
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModalAndPrint", @"
                $('#printSaveRecietDataModel').modal('show');
                setTimeout(function() { 
                    printGrid(); 
                }, 500);", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('Fields found are filled !.');", true);
                }
            }
            catch (Exception ex)
            {
                LogError(ex); // Assuming you have a logging mechanism
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    $"alert('Error occurred while fetching receipt data. Please try again.');", true);
            }


        }
        // Helper methods for safe data conversion
        private string GetSafeString(object value)
        {
            return value == DBNull.Value ? string.Empty : value.ToString().Trim();
        }

        private decimal GetSafeDecimal(object value)
        {
            if (value == DBNull.Value) return 0;
            return decimal.TryParse(value.ToString(), out decimal result) ? result : 0;
        }

        private void LogError(Exception ex)
        {
            // Implement your error logging mechanism here
            System.Diagnostics.Debug.WriteLine($"Error in PrintButton_Click: {ex.Message}");
            // You might want to log to a file or database
        }

        public static string GetDBStringValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return row[columnName] != DBNull.Value ? row[columnName].ToString() : null;
            }
            return null;
        }


        protected void ResetMain()
        {
            corporateName.Text = "";
            lblMessage.Text = string.Empty;
            btnShow.Enabled = true;
            lblArNo.Text = string.Empty;
            ddlProductClass.SelectedIndex = 0;
            ddlScheme.SelectedIndex = 0;
            ddlCra.SelectedIndex = 0;
            ddlCraBranch.SelectedIndex = 0;
            ddlBusinessBranch.SelectedIndex = 0;
            ddlRequestType.SelectedIndex = 0;
            ddlBankName.SelectedIndex = 0;

            // RadioButtonList
            ddlType.ClearSelection();
            ddlPaymentMethod.ClearSelection();
            rblTransactionType.ClearSelection();

            // TextBox
            txtDtNumber.Text = "";
            txtInvestorCode.Text = "";
            txtPopSpRegNo.Text = "";
            txtBusinessRm.Text = "";
            txtReceiptNo.Text = "";
            txtChequeNo.Text = "";
            txtChequeDated.Text = "";
            txtPran.Text = "";
            txtAmountReceivedTire1.Text = "";
            txtAmountReceivedTire2.Text = "";
            txtPopRegistrationChargesOneTime.Text = "";
            txtPopRegistrationCharges.Text = "";
            txtGst.Text = "";
            txtCollectionAmount.Text = "";
            txtAmountInvested.Text = "";
            txtAmountInvestedAdditional.Text = "";
            txtRemark.Text = "";
            txtNameOfSubscriber.Text = "";
            txtDate.Text = "";
            txtTime.Text = "";
            txtInvestorCode.Text = "";

            // Label
            lblArNo.Text = "0";
            txtInvestorCode.Text = string.Empty;

            // CheckBox
            chkZeroCommission.Checked = false;

            // Set date and time pickers to default values
            fromDate.Text = "";

            toDate.Text = "";
            txtChequeDated.Text = ""; // If using date picker control

            // Button enable states
            //  btnSave.Enabled = true; // Enable Save button
            //  btnModify.Enabled = false; // Enable Modify button
            // btnImportCorporateNonEsc.Enabled = true;
            // btnImportEcs.Enabled = true;
            //   btnPrint.Enabled = false;
        }

        private DateTime? ParseDate(string dateText, string dateFormat = "dd/MM/yyyy")
        {
            if (string.IsNullOrWhiteSpace(dateText)) return null; // Handle empty or whitespace input

            // Use DateTime.TryParseExact to ensure proper date format handling
            return DateTime.TryParseExact(dateText, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
                ? parsedDate
                : (DateTime?)null;
        }

        private void SetDateValue(DataRow row, string columnName, string dateFormat, System.Web.UI.WebControls.TextBox uiField)
        {
            if (DateTime.TryParse(row[columnName]?.ToString(), out DateTime dateValue))
            {
                uiField.Text = dateValue.ToString(dateFormat); // Set the date in the specified format
            }
            else
            {
                uiField.Text = string.Empty; // If date parsing fails, clear the field
            }
        }

        public string GetTextFieldValue(DataRow dataRow, string fieldName)
        {
            try
            {

                if (lblMessage.Text != null)
                {
                    lblMessage.Text = "";
                }

                // Check if the field exists in the DataRow
                if (dataRow.Table.Columns.Contains(fieldName))
                {
                    // Check if the field value is not null and return it, otherwise return null
                    return dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : null;
                }
                else
                {
                    throw new ArgumentException($"Field '{fieldName}' does not exist in the DataRow.");
                }
            }
            catch (Exception ex)
            {
                // If the label is found, show the error message
                lblMessage.Text = ex.Message;
                return null;
            }
        }

        private void SelectValueInDropdown(DropDownList dropdown, string valueToSelect)
        {
            if (dropdown == null || dropdown.Items.Count == 0) return;

            // Check if the value exists in the DropDownList
            ListItem item = dropdown.Items.FindByValue(valueToSelect);
            if (item != null)
            {
                // If the value exists, select it
                dropdown.SelectedValue = valueToSelect;
            }
            else
            {
                // If the value doesn't exist, select a fallback value (e.g., the first item)
                dropdown.SelectedIndex = 0; // Select the first item as default
            }
        }

        public static class DateTimeParser
        {
            // Function to parse a date string
            public static DateTime? ParseDate(string dateString, string format = "dd/MM/yyyy")
            {
                if (string.IsNullOrWhiteSpace(dateString))
                {
                    return null; // Return null if the input string is null or empty
                }

                if (DateTime.TryParseExact(dateString, format, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    return parsedDate; // Return parsed date if successful
                }

                // Handle invalid date format
                throw new FormatException("Invalid date format. Expected format is: " + format);
            }

            // Function to parse a time string
            public static DateTime? ParseTime(string timeString, string format = "HH:mm")
            {
                if (string.IsNullOrWhiteSpace(timeString))
                {
                    return null; // Return null if the input string is null or empty
                }

                if (DateTime.TryParseExact(timeString, format, null, System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                {
                    return parsedTime; // Return parsed time if successful
                }

                // Handle invalid time format
                throw new FormatException("Invalid time format. Expected format is: " + format);
            }
        }

        private void SetFieldData(DataRow row)
        {
            #region UI data frtch from row

            string corporateNameValue       = GetTextFieldValue(row, "CORPORATE_NAME");
            string docId                    = GetTextFieldValue(row, "DOC_ID");
            string tranCode                 = GetTextFieldValue(row, "TRAN_CODE");
            string clientCode               = GetTextFieldValue(row, "CLIENT_CODE");
            string schCode                  = row["SCH_CODE"]?.ToString();
            string folioNo                  = GetTextFieldValue(row, "FOLIO_NO");
            string businessRmCode           = GetTextFieldValue(row, "BUSINESS_RMCODE");
            string businessBranchCode       = row["BUSI_BRANCH_CODE"]?.ToString();
            string uniqueId                 = row["UNIQUE_ID"]?.ToString();
            string paymentMode              = row["PAYMENT_MODE"]?.ToString();
            string chequeNo                 = GetTextFieldValue(row, "CHEQUE_NO");
            DateTime? chequeDate            = row["CHEQUE_DATE"] != DBNull.Value ? (DateTime?)row["CHEQUE_DATE"] : null;
            string bankName                 = row["BANK_NAME"]?.ToString().ToLower();
            string appNo                    = row["APP_NO"]?.ToString();
            DateTime? trDate                = row["TR_DATE"] != DBNull.Value ? (DateTime?)row["TR_DATE"] : null;
            string InvNameValue             = GetTextFieldValue(row, "INV_NAME");
            string manualArNo               = GetTextFieldValue(row, "manual_arno");
            string ntAmount1                = GetTextFieldValue(row, "AMOUNT1");
            string ntAmount2                = GetTextFieldValue(row, "AMOUNT2");
            string ntRegCharge              = GetTextFieldValue(row, "REG_CHARGE");
            string ntTranCharge             = GetTextFieldValue(row, "Tran_CHARGE");
            string ntServiceTax             = GetTextFieldValue(row, "SERVICETAX");
            string amountInvested           = GetTextFieldValue(row, "AMOUNT");
            string remark                   = GetTextFieldValue(row, "REMARK");

            #endregion


            corporateName.Text = corporateNameValue;
            txtDtNumber.Text = docId;
            lblArNo.Text = tranCode;
            txtInvestorCode.Text = clientCode;
            txtPopSpRegNo.Text = folioNo;
            txtBusinessRm.Text = businessRmCode;
            txtReceiptNo.Text = uniqueId;
            txtChequeNo.Text = chequeNo;

            fun_PaymentModeChange(paymentMode.ToUpper());
             

            pc.SetDdlByText(bankName, ddlBankName, true);
            GetSetDateField(row, "CHEQUE_DATE", txtChequeDated);
            SelectValueInDropdown(ddlRequestType, appNo);
            GetSetDateField(row, "TR_DATE", txtDate);
            SelectValueInDropdown(ddlType, row["INVESTOR_TYPE"]?.ToString());
            if(ddlType.SelectedValue == "1")
            {
                corporateName.Enabled = true;
            }
            SelectValueInDropdown(ddlScheme, schCode);
            SelectValueInDropdown(ddlBusinessBranch, businessBranchCode);
            SelectValueInDropdown(ddlPaymentMethod, paymentMode);

            txtTime.Text = trDate.HasValue ? trDate.Value.ToString("HH:mm") : string.Empty;
            txtPran.Text = manualArNo;
            txtNameOfSubscriber.Text = InvNameValue;
            txtAmountReceivedTire1.Text = ntAmount1;
            txtAmountReceivedTire2.Text = ntAmount2;
            txtPopRegistrationChargesOneTime.Text = ntRegCharge;
            txtPopRegistrationCharges.Text = ntTranCharge;
            txtGst.Text = ntServiceTax;
            txtCollectionAmount.Text = string.Empty;
            txtAmountInvested.Text = amountInvested;
            txtRemark.Text = remark;

            txtDtNumber.Enabled = false;
            btnShow.Enabled = false;


            if(string.IsNullOrEmpty(corporateNameValue))
            {
                ddlType.SelectedValue = "0"; // SELECT INV
                corporateName.Enabled = false;
            }
            else
            {
                ddlType.SelectedValue = "1";
                corporateName.Enabled = true;
            }
            Fun_OnChangeRequestType(); // Call the function to handle on transaction load

        }


        private void GetSetDateField(DataRow row, string columnName, TextBox inputField)
        {
            if (System.DateTime.TryParse(row[columnName]?.ToString(), out System.DateTime parsedDate))
            {
                inputField.Text = parsedDate.ToString("dd/MM/yyyy");
            }
            else
            {
                inputField.Text = string.Empty;
            }
        }


        public bool npsInUpValidateion(bool isIn)
        {
            if (string.IsNullOrEmpty(txtDtNumber.Text))
            {
                pc.ShowAlert(this, "DT No ca not be left blank");
                return false;
            }

            if (isIn)
            {
                if(ddlType.SelectedValue == "1" && string.IsNullOrEmpty(corporateName.Text))
                {
                    pc.ShowAlert(this, "Corporate name cannot be left blank.");
                    corporateName.Focus();
                    return false;
                }

                if (!chkZeroCommission.Checked && !string.IsNullOrEmpty(txtPran.Text))
                {
                    string vq1 = "SELECT COUNT(*) as cf FROM NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO='" + txtPran.Text + "'";
                    DataTable dt_vq1 = pc.ExecuteCurrentQuery(vq1);
                    if (dt_vq1.Rows.Count > 0)
                    {
                        DataRow dtVq1_row = dt_vq1.Rows[0];

                        if (Convert.ToInt32(dtVq1_row["cf"]) > 0)
                        {
                            pc.ShowAlert(this, "FATCA for this PRAN is non compliant.Please contact product team for the same");
                            return false;
                        }
                    }

                }
                else
                {
                    string vq2 = "delete from NPS_FATCA_NON_COMPLIANT WHERE PRAN_NO=trim('" + txtPran.Text + "')";
                    DataTable dt_vq2 = pc.ExecuteCurrentQuery(vq2);
                }
            }

            string vq3 = "select category_id from client_master where client_code='"+ txtInvestorCode.Text+ "'";
            DataTable dt_vq3 = pc.ExecuteCurrentQueryMaster(vq3, out int rn3, out string ie3);



            if (string.IsNullOrEmpty(ie3) && rn3 > 0)
            {
                DataRow dtVq3_row = dt_vq3.Rows[0];

                if (dtVq3_row["category_id"].ToString() != "4004")
                {

                    string vq4 = "select count(*) from ( select TRAN_CODE from transaction_st where mut_code='IS02520' and cheque_no='" + txtChequeNo.Text + "' and tr_date >= add_months(sysdate,-6) Union All select TRAN_CODE from transaction_sttemp where mut_code='IS02520' and cheque_no='" + txtChequeNo.Text + "' and tr_date >= add_months(sysdate,-6) )";
                    DataTable dtVq4 = pc.ExecuteCurrentQueryMaster(vq4, out int rn4, out string ie4);

                    if (!string.IsNullOrEmpty(ie4))
                    {
                        pc.ShowAlert(this, "Duplicate Cheque Number !");
                        return false;
                    }


                }


            }
            return false;
        }

        private void InsertUpdate(string ui)
        {
           
            try
            {
                #region UI VALUES
                string mark = ui;
                string prodValue                = pc.SafeParseString(ddlProductClass.SelectedValue);
                string investorTypeValue        = pc.SafeParseString(ddlType.SelectedValue);
                string corporateNameValue       = pc.SafeParseString(corporateName.Text);
                string dtNumberValue            = pc.SafeParseString(txtDtNumber.Text);
                string tranCodeValue            = pc.SafeParseString(lblArNo.Text);
                long invertorCodeValue          = pc.SafeParseLong(txtInvestorCode.Text);
                string schemeCodeValue          = pc.SafeParseString(ddlScheme.SelectedValue);
                string craValue                 = pc.SafeParseString(ddlCra.SelectedValue);
                int craBranchValue              = 0; // ddlCraBranch.SelectedValue
                string txtPopSpRegNoFolioValue  = pc.SafeParseString(txtPopSpRegNo.Text);
                long businessRmValue            = pc.SafeParseLong(txtBusinessRm.Text);
                long businessBranchValue = string.IsNullOrWhiteSpace(ddlBusinessBranch.SelectedValue) ? 0 : Convert.ToInt64(ddlBusinessBranch.SelectedValue);
                string receiptNoUniueValue = pc.SafeParseString(txtReceiptNo.Text);
                char paymentModeValue = string.IsNullOrWhiteSpace(ddlPaymentMethod.SelectedValue) ? '\0' : Convert.ToChar(ddlPaymentMethod.SelectedValue);
                string chequeNoValue = string.IsNullOrWhiteSpace(txtChequeNo.Text) ? string.Empty : txtChequeNo.Text;
                string bankNameValue = ddlBankName.SelectedIndex ==  0 ? string.Empty : ddlBankName.SelectedItem.ToString();
                string appNoValue               = pc.SafeParseString(ddlRequestType.SelectedValue);
                DateTime? chequeDateValue = DateTimeParser.ParseDate(txtChequeDated.Text);
                DateTime? dateValue = DateTimeParser.ParseDate(txtDate.Text);
                DateTime? timeValue = DateTimeParser.ParseTime(txtTime.Text, "HH:mm");
                DateTime? combinedDateTime = null;

            

                if (dateValue.HasValue && timeValue.HasValue)
                {
                    // Combine date and time into one DateTime variable
                    combinedDateTime = new DateTime(
                        dateValue.Value.Year,
                        dateValue.Value.Month,
                        dateValue.Value.Day,
                        timeValue.Value.Hour,
                        timeValue.Value.Minute,
                        timeValue.Value.Second
                    );
                }

                // Use combinedDateTime as needed
                if (combinedDateTime.HasValue)
                {
                    // Do something with combinedDateTime.Value
                }
                else
                {
                    // Handle the case where the date or time was not provided
                    //Console.WriteLine("Date or time input was invalid or missing.");
                }

                string subInvNameValue = pc.SafeParseString(txtNameOfSubscriber.Text);
                string praManualARNoValue = pc.SafeParseString(txtPran.Text);
                string unfreezValue = pc.SafeParseCheckBoxZeroOne(CheckBox1.Checked);
                decimal amountT1Value = pc.SafeParsedecimal(txtAmountReceivedTire1.Text);
                decimal amountT2Value = pc.SafeParsedecimal(txtAmountReceivedTire2.Text);
                decimal regharge1Value = pc.SafeParsedecimal(txtPopRegistrationChargesOneTime.Text);
                decimal regharge2Value = pc.SafeParsedecimal(txtPopRegistrationCharges.Text);
                decimal gstTaxValue = pc.SafeParsedecimal(txtGst.Text);
                decimal amountCollectionValue = pc.SafeParsedecimal(txtCollectionAmount.Text);
                decimal amountInvestedValue = pc.SafeParsedecimal(txtAmountInvested.Text);
                decimal amountInvested2Value = pc.SafeParsedecimal(txtAmountInvestedAdditional.Text);
                
                string remarkValue = pc.SafeParseString(txtRemark.Text);
                string zeroComValue = pc.SafeParseCheckBoxYesNo(chkZeroCommission.Checked);
                string loggedinUser = pc.currentLoginID();
                #endregion

                #region VALIDATION


                if (string.IsNullOrEmpty(schemeCodeValue) || schemeCodeValue == "0")
                {
                    pc.ShowAlert(this, "Choose any pension schema");
                    ddlScheme.Focus();
                    return;
                }

                // TIRE AMOUNT VALIDATION BY INV/CORP AND SELECTED SCHEMAS
                if (investorTypeValue =="0") // 1 IS CORPORATE, 0 IS INDIVISUAL
                {
                    if (schemeCodeValue == "OP#09971")
                    {
                        /*
                        //txtAmountReceivedTire1 ==> amountT1Value
                        //txtAmountReceivedTire2 ==> amountT2Value
                        // Only t1 is required
                        */
                        if (string.IsNullOrWhiteSpace(txtAmountReceivedTire1.Text) || txtAmountReceivedTire1.Text == "0")
                        {
                            pc.ShowAlert(this, "Please enter a value for Tier 1.");
                            txtAmountReceivedTire1.Focus();
                            return;
                        }
                        else
                        {
                            amountT2Value = 0;

                        }
                    }

                    if (schemeCodeValue == "OP#09972")
                    {
                        // Only t2 is required
                        if (string.IsNullOrWhiteSpace(txtAmountReceivedTire2.Text) || txtAmountReceivedTire2.Text == "0")
                        {
                            pc.ShowAlert(this, "Please enter a value for Tier 2.");
                            txtAmountReceivedTire2.Focus();

                            return;
                        }
                        else
                        {
                            amountT1Value = 0;

                        }
                    }

                    if (schemeCodeValue == "OP#09973")
                    {
                        // Both t1 and t2 are required
                        if (string.IsNullOrWhiteSpace(txtAmountReceivedTire1.Text) ||
                            string.IsNullOrWhiteSpace(txtAmountReceivedTire2.Text) ||
                            txtAmountReceivedTire1.Text == "0" ||
                            txtAmountReceivedTire2.Text == "0")
                        {
                            pc.ShowAlert(this, "Please enter values for both Tier 1 and Tier 2.");
                            txtAmountReceivedTire1.Focus();
                            return;
                        }
                    }
                }

                #endregion

                string insertResult = new WM.Controllers.NpsTransactionPunchingController().InsertClientData(
                mark,
                prodValue,
                investorTypeValue,
                corporateNameValue,
                dtNumberValue,
                tranCodeValue,
                invertorCodeValue,
                schemeCodeValue,
                craValue,
                craBranchValue,
                txtPopSpRegNoFolioValue,
                businessRmValue,
                businessBranchValue,
                receiptNoUniueValue,
                paymentModeValue,
                chequeNoValue,
                bankNameValue,
                appNoValue,
                chequeDateValue,
                dateValue,
                timeValue,
                combinedDateTime,
                subInvNameValue,
                praManualARNoValue,
                unfreezValue,
                amountT1Value,
                amountT2Value,
                regharge1Value,
                regharge2Value,
                gstTaxValue,
                amountCollectionValue,
                amountInvestedValue,
                amountInvested2Value,
                remarkValue,
                zeroComValue,
                loggedinUser
                    );

                lblMessage.CssClass = insertResult.Contains("success") ? "message-label-success" : "message-label-error";

                lblMessage.Text = insertResult;
                lblMessage.Focus();


                if (insertResult.Contains("successful"))
                {
                    string currentAR = lblArNo.Text.ToString();
                    string currentDT = txtDtNumber.Text.ToString();

                    if (!string.IsNullOrEmpty(currentAR))
                    {
                        // If the AR number or DT number is empty, we need to alert that the transaction was updated
                        if (insertResult.Contains("Updation successful"))
                        {
                            ResetMain();
                            if (!string.IsNullOrEmpty(currentAR))
                            {

                                DataTable dt = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, currentAR, false, Session["LoginId"]?.ToString());
                                if (dt.Rows.Count > 0)
                                {
                                    DataRow row = dt.Rows[0];
                                    SetFieldData(row);
                                    pc.ShowAlert(this, insertResult);
                                    lblMessage.Text = insertResult;
                                    lblMessage.Focus();

                                    string currentDT1 = currentDT;


                                }
                            }


                        }

                        else if (insertResult.Contains("Insertion successful"))
                        {
                            pc.ShowAlert(this, insertResult);
                            lblMessage.Text = insertResult;

                        }
                    }

                }
                else
                {
                    pc.ShowAlert(this, insertResult);
                }

            }
            catch (Exception ex)
            {
                pc.ShowAlert(this, $"Error: Kindly fill form properly. {ex.Message}");
               
            }


        }

        #endregion


        #region NO NEED SECIONT FUNCTIONALITY

        protected void FillArBySession(string tranCode)
        {

            try
            {

                bool beforeAfterFlags = false;

                if (!string.IsNullOrEmpty(tranCode))
                {

                    DataTable dtIfArExistForTempTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, true, Session["LoginId"]?.ToString());
                    DataTable dtIfArExistForStTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, false, Session["LoginId"]?.ToString());

                    int temDataCount = dtIfArExistForTempTran.Rows.Count > 0 ? 1 : 0;
                    int dtCount3 = dtIfArExistForStTran.Rows.Count > 0 ? 1 : 0;


                    // Check for AR in temp transaction table
                    if (temDataCount == 0) // No data in temp
                    {
                        if (dtCount3 == 1) // Data exists in st
                        {
                            string dbMsg1 = dtIfArExistForStTran.Rows[0]["message"].ToString();
                            if (dbMsg1.Contains("Validity: Transaction data exist in st"))
                            {
                                DataRow row3 = dtIfArExistForStTran.Rows[0];
                                SetFieldData(row3);
                                ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);
                                btnSave.Enabled = false;
                                btnModify.Enabled = true;
                                txtBusinessRm.Enabled = false;


                            }
                        }
                        else
                        {

                            pc.ShowAlert(this, "No data exist!");

                        }
                    }
                    else
                    {
                        // Data exists in temp, extract values as needed
                        string dbMsg1 = dtIfArExistForTempTran.Rows[0]["message"].ToString();
                        if (dbMsg1.Contains("Validity: Transaction data exist in temp"))
                        {
                            DataRow row3 = dtIfArExistForTempTran.Rows[0];
                            //ShowAlert(dbMsg1);
                            SetFieldData(row3);
                            ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);
                            btnSave.Enabled = false;
                            btnModify.Enabled = true;
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                //btnSave.Enabled = true;
                //btnModify.Enabled = false;

            }

        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("NPS_tran");

                    // Set headers
                    string[] headers = new string[] {
                    "PoP NO", "RRAN No", "Client Name", "Payment Mode", "Cheque Number",
                    "Date", "Bank Name", "Receipt no", "Tier 1", "Tier 2",
                    "Reg Charges", "Tran Charges", "GST", "Amount Invested",
                    "Receipt no(10-17)", "Fc Registration No", "Remarks",
                    "Remarks1", "Tran_Code", "Ref_Tran_Code", "CSF_TRANSACTION_ID"
                };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    // Get data from database
                    using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                    {
                        conn.Open();
                        using (OracleCommand cmd = new OracleCommand("PKG_NPS_EXPORT_PRA.GET_NPS_TRANSACTIONS_PRA", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            cmd.Parameters.Add("p_from_date", OracleDbType.Varchar2).Value = fromDate.Text;
                            cmd.Parameters.Add("p_to_date", OracleDbType.Varchar2).Value = toDate.Text;
                            cmd.Parameters.Add("p_transaction_type", OracleDbType.Varchar2).Value = rblTransactionType.SelectedValue;
                            cmd.Parameters.Add("p_cra_type", OracleDbType.Varchar2).Value = cra.SelectedValue;
                            cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                int row = 2;
                                while (reader.Read())
                                {
                                    for (int col = 0; col < reader.FieldCount; col++)
                                    {
                                        worksheet.Cells[row, col + 1].Value = reader[col]?.ToString() ?? "";
                                    }
                                    row++;
                                }
                            }
                        }
                    }

                    // Format and autofit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Prepare response
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=NPS_Transaction_Export.xlsx");

                    // Write to response stream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        package.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                    }
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                // Log error and show user-friendly message
                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('An error occurred while exporting data. Please try again.');", true);
            }
        }

        #endregion

       
    }
}