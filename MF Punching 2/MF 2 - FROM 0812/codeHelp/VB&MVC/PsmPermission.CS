using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WM.Models;
using System.Web.Configuration;
using System.Web.ModelBinding;
using Oracle.ManagedDataAccess.Types;
using System.Web.UI;
using System.Web.UI.WebControls; 
using System.IO;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Globalization;
using MathNet.Numerics.Distributions;
using System.Text;
using NPOI.SS.Formula.Functions;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace WM.Controllers
{
    public class PsmPermission
    {
        // Database connection
        private OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
        public static string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

        // Windows API declarations
        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern int WritePrinter(IntPtr hPrinter, IntPtr pBuf, int cdBuf, ref int pcWritten);

        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern int OpenPrinter(string pPrinterName, ref IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv")]
        public static extern int ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", CharSet = CharSet.Auto)]
        public static extern int StartDocPrinter(IntPtr hPrinter, int Level, byte[] pDocInfo);

        [DllImport("winspool.drv")]
        public static extern int EndDocPrinter(IntPtr hPrinter);

        // Global variables
        public static string Glbloginid = "";
        public static string GlbPass = "";
        public static string GlbroleId = "";
        private static string GlbUpdatePre = "";
        private static string Glbupdatenext = "";
        private static string GlbINSPre = "";
        private static string GlbINSnext = "";

        private static string GlbUpdatePreType = "";
        private static string GlbupdatenextType = "";
        private static string GlbINSPreType = "";
        private static string GlbINSnextType = "";

        public static DateTime Glbins_previousdate;
        public static DateTime Glbins_nextdate;

        public static string GLBlocal_host = "";
        public static string GLBglobal_host = "";
        public static string GLBLOCAL_UID = "";
        public static string GLBGLOBAL_UID = "";
        public static string GLBLOCAL_PWD = "";
        public static string GLBGLOBAL_PWD = "";

        public static DateTime Glbup_previousdate;
        public static DateTime Glbup_nextdate;
        public static string GlbDataFilter = "";
        public static bool LockedUser = false;
        public static bool LockedAllLI = false;

        // Additional global variables from VB code
        public static string Tr_Branches = "";
        public static string Branches = "";
        public static string InactiveBranches = "";
        public static string Products = "";
        public static string SRmCode = "";
        public static string SPayrollId = "";

        public enum enmDateFormat
        {
            e1or2M_1or2D_YY = 1,
            e1or2D_1or2M_2or4Y,
            eMM_DD_YY,
            eMM_DD_YYYY,
            eYYYY_MM_DD
        }

        private const int CB_LIMITTEXT = 0x141;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        public static void SetComboBoxMaxLength(IntPtr hwnd, int MaxLength)
        {
            SendMessage(hwnd, CB_LIMITTEXT, MaxLength, 0);
        }

        public static void RolePermission(string loginId, string userPassword)
        {
            // This would need to be adapted based on your actual form implementation
            // frmLoginMaster.cborolename.Clear();
            
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();
                string sql = $"select * from user_master where login_id='{loginId.Trim()}' and login_pass='{userPassword}' and status='1' AND LOGINVARIFY('{loginId.Trim()}')=1";
                
                using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                {
                    using (OracleDataReader rsRole = cmd.ExecuteReader())
                    {
                        if (rsRole.HasRows)
                        {
                            Glbloginid = rsRole["login_id"].ToString();
                            GlbPass = userPassword.Trim();

                            string roleId = rsRole["Role_id"].ToString();
                            if (roleId.Contains(","))
                            {
                                string[] rolefind = roleId.Split(',');
                                foreach (string role in rolefind)
                                {
                                    string roleSql = $"select role_name from role_master where role_id='{role.Trim()}'";
                                    using (OracleCommand roleCmd = new OracleCommand(roleSql, MyConn))
                                    {
                                        using (OracleDataReader RsRName = roleCmd.ExecuteReader())
                                        {
                                            if (RsRName.HasRows && RsRName.Read())
                                            {
                                                // frmLoginMaster.cborolename.AddItem(RsRName["role_name"].ToString());
                                            }
                                        }
                                    }
                                }
                                // frmLoginMaster.cborolename.ListIndex = 0;
                                // frmLoginMaster.cborolename_Click();
                            }
                            else
                            {
                                string roleSql = $"select role_name from role_master where role_id='{roleId.Trim()}'";
                                using (OracleCommand roleCmd = new OracleCommand(roleSql, MyConn))
                                {
                                    using (OracleDataReader RsRName = roleCmd.ExecuteReader())
                                    {
                                        if (RsRName.HasRows && RsRName.Read())
                                        {
                                            // frmLoginMaster.cborolename.AddItem(RsRName["role_name"].ToString());
                                            // frmLoginMaster.cborolename.ListIndex = 0;
                                            // frmLoginMaster.cborolename_Click();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ButtonPermission(Page page)
        {
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();
                string sql = $"select * from ROLE_FORM_PERMISSION_VB where role_id='{GlbroleId.Trim()}'";
                
                using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                {
                    using (OracleDataReader RsFormPermission = cmd.ExecuteReader())
                    {
                        if (RsFormPermission.HasRows)
                        {
                            while (RsFormPermission.Read())
                            {
                                if (RsFormPermission["view_Button"].ToString() == "1" || 
                                    RsFormPermission["add_Button"].ToString() == "1" || 
                                    RsFormPermission["update_Button"].ToString() == "1" || 
                                    RsFormPermission["print_Button"].ToString() == "1")
                                {
                                    string formSql = $"select main from backoffice_forms where formname='{RsFormPermission["Form_name"].ToString().Trim()}'";
                                    using (OracleCommand formCmd = new OracleCommand(formSql, MyConn))
                                    {
                                        using (OracleDataReader RsFormName = formCmd.ExecuteReader())
                                        {
                                            if (RsFormName.HasRows && RsFormName.Read())
                                            {
                                                // This would need adaptation for ASP.NET forms
                                                // if (frm.Name == RsFormName["main"].ToString().Trim())
                                                // {
                                                //     // Handle button permissions for ASP.NET
                                                // }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Show_Tr_Branches_new(string strBusiRM)
        {
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();
                string sql = $@"select branch_id from userdetails_ji where login_id='{strBusiRM}' and datafilter in( 
                              select data_filter datafilter from datafilter_master where login_id in(select payroll_id from employee_master where type='A' and category_id=2001) and login_id='{strBusiRM}' AND ROLE_DEFAULT='1' )";
                
                using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                {
                    using (OracleDataReader RsData_filter = cmd.ExecuteReader())
                    {
                        while (RsData_filter.Read())
                        {
                            string branchSql = $"SELECT getBranch_TR('{RsData_filter["branch_id"]}') FROM DUAL";
                            using (OracleCommand branchCmd = new OracleCommand(branchSql, MyConn))
                            {
                                object result = branchCmd.ExecuteScalar();
                                if (result != null && result != DBNull.Value)
                                {
                                    string branchData = result.ToString();
                                    if (!string.IsNullOrEmpty(branchData) && branchData.Length > 1)
                                    {
                                        string[] BrCode = branchData.Substring(1).Split('#');
                                        foreach (string branch in BrCode)
                                        {
                                            Tr_Branches += branch + ",";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (!string.IsNullOrEmpty(Tr_Branches))
                {
                    Tr_Branches = Tr_Branches.TrimEnd(',');
                }
            }
        }

        public static void DataPermission()
        {
            try
            {
                using (OracleConnection MyConn = new OracleConnection(connectionString))
                {
                    MyConn.Open();
                    string sql = $"select * from role_master where role_id='{GlbroleId.Trim()}'";
                    
                    using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                    {
                        using (OracleDataReader RsData = cmd.ExecuteReader())
                        {
                            if (RsData.HasRows && RsData.Read())
                            {
                                GlbUpdatePre = RsData["up_pre_dur"].ToString().Trim();
                                Glbupdatenext = RsData["up_next_dur"].ToString().Trim();
                                GlbINSPre = RsData["in_pre_dur"].ToString().Trim();
                                GlbINSnext = RsData["in_next_dur"].ToString().Trim();
                                
                                GlbUpdatePreType = RsData["up_pre_dur_type"].ToString().Trim();
                                GlbupdatenextType = RsData["up_next_dur_type"].ToString().Trim();
                                GlbINSPreType = RsData["in_pre_dur_type"].ToString().Trim();
                                GlbINSnextType = RsData["in_next_dur_type"].ToString().Trim();
                                
                                DataFilterWithDatesforINs(
                                    int.Parse(GlbINSPre), 
                                    int.Parse(GlbINSPreType), 
                                    int.Parse(GlbINSnext), 
                                    int.Parse(GlbINSnextType));
                                
                                DataFilterWithDatesforUPd(
                                    int.Parse(GlbUpdatePre), 
                                    int.Parse(GlbUpdatePreType), 
                                    int.Parse(Glbupdatenext), 
                                    int.Parse(GlbupdatenextType));
                                
                                string filterSql = $"select data_filter from datafilter_master where role_id='{GlbroleId.Trim()}' and login_id='{Glbloginid.Trim()}'";
                                using (OracleCommand filterCmd = new OracleCommand(filterSql, MyConn))
                                {
                                    using (OracleDataReader rsRole = filterCmd.ExecuteReader())
                                    {
                                        if (rsRole.HasRows && rsRole.Read())
                                        {
                                            GlbDataFilter = rsRole["data_filter"].ToString().Trim();
                                        }
                                    }
                                }
                                
                                // Check user lock status
                                string lockSql = $"select * from USER_LOCKED where userid='{Glbloginid.Trim()}'";
                                using (OracleCommand lockCmd = new OracleCommand(lockSql, MyConn))
                                {
                                    using (OracleDataReader rsRole = lockCmd.ExecuteReader())
                                    {
                                        LockedUser = rsRole.HasRows;
                                    }
                                }
                                
                                // Check LI block status
                                string liSql = "select BLOCKED from RDC_BLOCK WHERE MODULE='LI'";
                                using (OracleCommand liCmd = new OracleCommand(liSql, MyConn))
                                {
                                    using (OracleDataReader rsRole = liCmd.ExecuteReader())
                                    {
                                        if (rsRole.HasRows && rsRole.Read())
                                        {
                                            LockedAllLI = rsRole["BLOCKED"].ToString() == "Y";
                                        }
                                        else
                                        {
                                            LockedAllLI = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public static void Show_Tr_Branches(string strBusiRM)
        {
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();
                string sql = $"select data_filter datafilter from datafilter_master where login_id in(select payroll_id from employee_master where type='A' and category_id=2018) and login_id='{strBusiRM}' AND ROLE_DEFAULT='1'";
                
                using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                {
                    using (OracleDataReader RsData_filter = cmd.ExecuteReader())
                    {
                        if (RsData_filter.HasRows && RsData_filter.Read())
                        {
                            string dataFilter = RsData_filter["datafilter"].ToString();
                            if (!dataFilter.StartsWith("2") && !dataFilter.StartsWith("7"))
                            {
                                string[] GlCode = dataFilter.Split('#');
                                foreach (string code in GlCode)
                                {
                                    string branchSql = $"SELECT getBranch_TR('{code}') FROM DUAL";
                                    using (OracleCommand branchCmd = new OracleCommand(branchSql, MyConn))
                                    {
                                        object result = branchCmd.ExecuteScalar();
                                        if (result != null && result != DBNull.Value)
                                        {
                                            string branchData = result.ToString();
                                            if (!string.IsNullOrEmpty(branchData) && branchData.Length > 1)
                                            {
                                                string[] BrCode = branchData.Substring(1).Split('#');
                                                foreach (string branch in BrCode)
                                                {
                                                    Tr_Branches += branch + ",";
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                if (!string.IsNullOrEmpty(Tr_Branches))
                                {
                                    Tr_Branches = Tr_Branches.TrimEnd(',');
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ShowProducts()
        {
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();
                string sql = $"SELECT prod_code from bajaj_product_permissions where role_id='{GlbroleId}'";
                
                using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                {
                    using (OracleDataReader rsData1 = cmd.ExecuteReader())
                    {
                        if (rsData1.HasRows && rsData1.Read())
                        {
                            Products = rsData1["prod_code"].ToString();
                            if (!string.IsNullOrEmpty(Products))
                            {
                                Products = Products.Substring(0, Products.Length - 1);
                                Products = "'" + Products.Replace("#", "','") + "'";
                            }
                        }
                    }
                }
            }
        }

        public static DateTime ServerDateTime()
        {
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();
                string sql = "select sysdate from dual";
                
                using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return DateTime.Parse(result.ToString());
                    }
                }
            }
            return DateTime.Now;
        }

        // Date validation functions
        public static bool IsValidDateString(string p_sSource, enmDateFormat p_lMode, bool p_IsValidateSeperator = false, bool p_AllowInternalSpaces = false)
        {
            bool bReturn = false;
            string sTemp = p_sSource.Replace(".", "/");
            
            if (DateTime.TryParse(sTemp, out DateTime tempDate))
            {
                if (!LengthCheck(p_lMode, p_sSource, p_AllowInternalSpaces))
                {
                    return false;
                }
                else
                {
                    p_sSource = p_sSource.Trim();
                }

                if (!YearCheck(p_lMode, p_sSource, out long lYear))
                    return false;

                long lMonth = MonthExtract(p_lMode, p_sSource);
                if (lMonth == 0) return false;

                long lday = DayCheck(p_lMode, p_sSource, lMonth, lYear);
                if (lday == 0) return false;

                if (p_IsValidateSeperator)
                {
                    char[] vDelims_ = { '/', '-', '.' };

                    switch (p_lMode)
                    {
                        case enmDateFormat.eMM_DD_YY:
                        case enmDateFormat.eMM_DD_YYYY:
                            if (!vDelims_.Contains(p_sSource[2]) || !vDelims_.Contains(p_sSource[5]))
                                return false;
                            break;
                        case enmDateFormat.eYYYY_MM_DD:
                            if (!vDelims_.Contains(p_sSource[4]) || !vDelims_.Contains(p_sSource[7]))
                                return false;
                            break;
                        case enmDateFormat.e1or2M_1or2D_YY:
                            // Implementation for variable length dates
                            break;
                    }
                }

                if (!DateTime.TryParse($"{lday}/{lMonth}/{lYear}", out DateTime finalDate))
                {
                    return false;
                }
                else
                {
                    bReturn = true;
                }
            }

            return bReturn;
        }

        private static bool LengthCheck(enmDateFormat p_lMode, string p_sSource, bool p_AllowInternalSpaces)
        {
            bool bReturn = false;

            if (!p_AllowInternalSpaces)
            {
                p_sSource = p_sSource.Replace(" ", "");
            }

            switch (p_lMode)
            {
                case enmDateFormat.eMM_DD_YY:
                    if (p_sSource.Trim().Length == 8)
                        bReturn = true;
                    break;
                case enmDateFormat.eMM_DD_YYYY:
                case enmDateFormat.eYYYY_MM_DD:
                    if (p_sSource.Trim().Length == 10)
                        bReturn = true;
                    break;
                case enmDateFormat.e1or2M_1or2D_YY:
                    if (p_sSource.Trim().Length >= 6 && p_sSource.Trim().Length <= 8)
                        bReturn = true;
                    break;
                case enmDateFormat.e1or2D_1or2M_2or4Y:
                    if (p_sSource.Trim().Length >= 6 && p_sSource.Trim().Length <= 10)
                        bReturn = true;
                    break;
            }

            return bReturn;
        }

        private static bool YearCheck(enmDateFormat p_lMode, string p_sSource, out long P_LYEAR)
        {
            P_LYEAR = 0;
            string sYear = "";

            switch (p_lMode)
            {
                case enmDateFormat.eMM_DD_YY:
                case enmDateFormat.e1or2M_1or2D_YY:
                    sYear = p_sSource.Substring(p_sSource.Length - 2);
                    break;
                case enmDateFormat.eMM_DD_YYYY:
                    sYear = p_sSource.Substring(p_sSource.Length - 4);
                    break;
                case enmDateFormat.eYYYY_MM_DD:
                    sYear = p_sSource.Substring(0, 4);
                    break;
                case enmDateFormat.e1or2D_1or2M_2or4Y:
                    if (IsDigitsOnly(p_sSource.Substring(p_sSource.Length - 4)))
                        sYear = p_sSource.Substring(p_sSource.Length - 4);
                    else
                        sYear = p_sSource.Substring(p_sSource.Length - 2);
                    break;
            }

            if (!IsDigitsOnly(sYear))
                return false;
            else
                P_LYEAR = long.Parse(sYear);

            if (P_LYEAR < 0)
                return false;
            else
                return true;
        }

        private static long MonthExtract(enmDateFormat p_lMode, string p_sSource)
        {
            long lReturn = 0;
            string sMonth = "";

            switch (p_lMode)
            {
                case enmDateFormat.eMM_DD_YY:
                case enmDateFormat.eMM_DD_YYYY:
                    sMonth = p_sSource.Substring(0, 2);
                    break;
                case enmDateFormat.eYYYY_MM_DD:
                    sMonth = p_sSource.Substring(5, 2);
                    break;
                case enmDateFormat.e1or2M_1or2D_YY:
                    switch (p_sSource.Length)
                    {
                        case 6:
                            sMonth = p_sSource.Substring(0, 1);
                            break;
                        case 7:
                            if (IsDigitsOnly(p_sSource.Substring(0, 2)))
                                sMonth = p_sSource.Substring(0, 2);
                            else
                                sMonth = p_sSource.Substring(0, 1);
                            break;
                        case 8:
                            sMonth = p_sSource.Substring(0, 2);
                            break;
                    }
                    break;
                case enmDateFormat.e1or2D_1or2M_2or4Y:
                    switch (p_sSource.Length)
                    {
                        case 6:
                            sMonth = p_sSource.Substring(0, 1);
                            break;
                        case 10:
                            sMonth = p_sSource.Substring(3, 2);
                            break;
                        default:
                            if (IsDigitsOnly(p_sSource.Substring(0, 2)))
                                sMonth = p_sSource.Substring(0, 2);
                            else
                                sMonth = p_sSource.Substring(0, 1);
                            break;
                    }
                    break;
            }

            if (IsDigitsOnly(sMonth))
            {
                long month = long.Parse(sMonth);
                if (month >= 1 && month <= 12)
                {
                    lReturn = month;
                }
            }

            return lReturn;
        }

        private static long DayCheck(enmDateFormat p_lMode, string p_sSource, long p_lMonth, long P_LYEAR)
        {
            long lReturn = 0;
            string sDay = "";

            switch (p_lMode)
            {
                case enmDateFormat.eMM_DD_YY:
                case enmDateFormat.eMM_DD_YYYY:
                    sDay = p_sSource.Substring(3, 2);
                    break;
                case enmDateFormat.eYYYY_MM_DD:
                    sDay = p_sSource.Substring(8, 2);
                    break;
                case enmDateFormat.e1or2M_1or2D_YY:
                    switch (p_sSource.Length)
                    {
                        case 8:
                            sDay = p_sSource.Substring(3, 2);
                            break;
                        case 6:
                            sDay = p_sSource.Substring(2, 1);
                            break;
                        default:
                            if (IsDigitsOnly(p_sSource.Substring(2, 2)))
                                sDay = p_sSource.Substring(2, 2);
                            else
                                sDay = p_sSource.Substring(3, 1);
                            break;
                    }
                    break;
                case enmDateFormat.e1or2D_1or2M_2or4Y:
                    switch (p_sSource.Length)
                    {
                        case 10:
                            sDay = p_sSource.Substring(0, 2);
                            break;
                        case 6:
                            sDay = p_sSource.Substring(2, 1);
                            break;
                        case 7:
                            if (IsDigitsOnly(p_sSource.Substring(2, 2)))
                                sDay = p_sSource.Substring(2, 2);
                            else
                                sDay = p_sSource.Substring(3, 1);
                            break;
                        case 8:
                            if (IsDigitsOnly(p_sSource.Substring(3, 2)))
                                sDay = p_sSource.Substring(3, 2);
                            else
                                sDay = p_sSource.Substring(2, 1);
                            break;
                        case 9:
                            if (IsDigitsOnly(p_sSource.Substring(2, 2)))
                                sDay = p_sSource.Substring(2, 2);
                            else
                                sDay = p_sSource.Substring(3, 1);
                            break;
                    }
                    break;
            }

            if (!IsDigitsOnly(sDay))
                return 0;
            else
            {
                long lday = long.Parse(sDay);
                if (lday < 0) return 0;
            }

            long lMax = GetMaxDaysInMonth(p_lMonth, P_LYEAR);

            long ldayCheck = long.Parse(sDay);
            if (ldayCheck > lMax) return 0;

            return ldayCheck;
        }

        private static long GetMaxDaysInMonth(long month, long year)
        {
            switch (month)
            {
                case 4: case 6: case 9: case 11:
                    return 30;
                case 1: case 3: case 5: case 7: case 8: case 10: case 12:
                    return 31;
                case 2:
                    return IsLeapYear(year) ? 29 : 28;
                default:
                    return 0;
            }
        }

        private static bool IsLeapYear(long year)
        {
            return (year % 4 == 0) && ((year % 100 != 0) || (year % 400 == 0));
        }

        public static bool IsDigitsOnly(string Value)
        {
            return !Value.Any(c => !char.IsDigit(c));
        }

        // Print functions
        public static void Print_File(string Fname)
        {
            if (string.IsNullOrEmpty(Fname))
            {
                // MessageBox.Show("Nothing to Print", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IntPtr lhPrinter = IntPtr.Zero;
            int PrntErr = OpenPrinter(PrinterSettings.InstalledPrinters[0], ref lhPrinter, IntPtr.Zero);

            if (PrntErr == 0)
            {
                // MessageBox.Show("Error in Printing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PrntErr = StartDocPrinter(lhPrinter, 1, new byte[0]);

            if (PrntErr == 0)
            {
                PrintToPort(Fname);
                return;
            }

            using (StreamReader sr = new StreamReader(Fname))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line + "\r\n";
                    int bytesWritten = 0;
                    IntPtr pBuf = Marshal.StringToHGlobalAnsi(line);
                    WritePrinter(lhPrinter, pBuf, line.Length, ref bytesWritten);
                    Marshal.FreeHGlobal(pBuf);
                }
            }

            EndDocPrinter(lhPrinter);
            ClosePrinter(lhPrinter);
        }

        private static void PrintToPort(string Fname)
        {
            // This would need to be adapted for your specific printing needs
            using (StreamReader sr = new StreamReader(Fname))
            using (StreamWriter sw = new StreamWriter("132.0.0.250")) // This would need proper network stream handling
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
            }
        }

        public static bool CheckDay(long lday, long p_lMonth, long P_LYEAR)
        {
            long lMax = GetMaxDaysInMonth(p_lMonth, P_LYEAR);
            return lday == lMax;
        }

        public static void ShowData()
        {
            using (OracleConnection MyConn = new OracleConnection(connectionString))
            {
                MyConn.Open();

                if (GlbDataFilter.StartsWith("7"))
                {
                    string sql = "select branch_code from branch_master where (BRANCH_TYPE <>'Inactive' OR BRANCH_TYPE IS NULL) and category_id not in (1006)";
                    using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                    {
                        using (OracleDataReader RsData = cmd.ExecuteReader())
                        {
                            string New_Branch = "";
                            while (RsData.Read())
                            {
                                New_Branch += "#" + RsData[0];
                            }

                            if (!string.IsNullOrEmpty(New_Branch))
                            {
                                string[] BrCode = New_Branch.Substring(1).Split('#');
                                Branches = string.Join(",", BrCode);
                            }
                        }
                    }
                }

                if (GlbDataFilter.StartsWith("2"))
                {
                    string[] GlCode = GlbDataFilter.Split('#');
                    string sql = $"SELECT getBranchRM({GlCode[0]}) FROM DUAL";
                    using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            SRmCode = GlCode[0];
                            string rmSql = $"SELECT PAYROLL_ID FROM EMPLOYEE_MASTER WHERE RM_CODE={SRmCode}";
                            using (OracleCommand rmCmd = new OracleCommand(rmSql, MyConn))
                            {
                                using (OracleDataReader rsRMDATA = rmCmd.ExecuteReader())
                                {
                                    if (rsRMDATA.HasRows && rsRMDATA.Read())
                                    {
                                        SPayrollId = rsRMDATA[0].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!GlbDataFilter.StartsWith("2") && !GlbDataFilter.StartsWith("7"))
                {
                    string[] GlCode = GlbDataFilter.Split('#');
                    foreach (string code in GlCode)
                    {
                        string sql = $"SELECT getBranch('{code}') FROM DUAL";
                        using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                        {
                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                string branchData = result.ToString();
                                if (!string.IsNullOrEmpty(branchData) && branchData.Length > 1)
                                {
                                    string[] BrCode = branchData.Substring(1).Split('#');
                                    foreach (string branch in BrCode)
                                    {
                                        Branches += branch + ",";
                                    }
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Branches))
                    {
                        Branches = Branches.TrimEnd(',');
                    }
                }

                if (GlbDataFilter.StartsWith("2"))
                {
                    // Handle case for data filter starting with "2"
                    string sql = $"SELECT getBranchRM({GlbDataFilter.Substring(1).Split('#')[0]}) FROM DUAL";
                    using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            string branchData = result.ToString();
                            if (!string.IsNullOrEmpty(branchData) && branchData.Length > 1)
                            {
                                string[] BrCode = branchData.Substring(1).Split('#');
                                Branches = string.Join(",", BrCode);
                            }
                        }
                    }
                }

                if (GlbDataFilter.StartsWith("7"))
                {
                    string sql = "select branch_code from branch_master where category_id in (1006)";
                    using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                    {
                        using (OracleDataReader RsData = cmd.ExecuteReader())
                        {
                            while (RsData.Read())
                            {
                                InactiveBranches += RsData[0] + ",";
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(InactiveBranches))
                    {
                        InactiveBranches = InactiveBranches.TrimEnd(',');
                    }
                }

                ShowProducts();
            }
        }

        public static void DataFilterWithDatesforINs(int ins_pre_day, int ins_pre_type, int ins_next_day, int ins_next_type)
        {
            DateTime serverDate = ServerDateTime();

            if (ins_pre_type == 1 && ins_next_type == 1)
            {
                Glbins_previousdate = serverDate.AddDays(-ins_pre_day);
                Glbins_nextdate = serverDate.AddDays(ins_next_day - 1);
            }
            else if (ins_pre_type == 1 && ins_next_type == 2)
            {
                Glbins_previousdate = serverDate.AddDays(-ins_pre_day);
                Glbins_nextdate = serverDate.AddMonths(ins_next_day - 1);
            }
            else if (ins_pre_type == 1 && ins_next_type == 3)
            {
                using (OracleConnection MyConn = new OracleConnection(connectionString))
                {
                    MyConn.Open();
                    string sql = "select date_fr,date_to from financial_year";
                    using (OracleCommand cmd = new OracleCommand(sql, MyConn))
                    {
                        using (OracleDataReader GetCurYeardateFRTO = cmd.ExecuteReader())
                        {
                            if (GetCurYeardateFRTO.HasRows && GetCurYeardateFRTO.Read())
                            {
                                DateTime FinYearStartDate = DateTime.Parse(GetCurYeardateFRTO["date_fr"].ToString());
                                DateTime ActaulStartDate = serverDate.AddDays(-ins_pre_day);
                                
                                Glbins_previousdate = FinYearStartDate >= ActaulStartDate ? FinYearStartDate : ActaulStartDate;
                                Glbins_nextdate = DateTime.Parse(GetCurYeardateFRTO["date_to"].ToString()).AddYears(ins_next_day - 1);
                            }
                        }
                    }
                }
            }
            // Additional cases would be implemented similarly...
        }

        public static void DataFilterWithDatesforUPd(int up_pre_day, int up_pre_type, int up_next_day, int up_next_type)
        {
            DateTime serverDate = ServerDateTime();

            if (up_pre_type == 1 && up_next_type == 1)
            {
                Glbup_previousdate = serverDate.AddDays(-up_pre_day);
                Glbup_nextdate = serverDate.AddDays(up_next_day - 1);
            }
            else if (up_pre_type == 1 && up_next_type == 2)
            {
                Glbup_previousdate = serverDate.AddDays(-up_pre_day);
                Glbup_nextdate = serverDate.AddMonths(up_next_day - 1);
            }
            // Additional cases would be implemented similarly...
        }
    }
}