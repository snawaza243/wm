using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Configuration;
using System.Web.Services.Description;
using Oracle.ManagedDataAccess.Client;

namespace WM.Controllers
{
    public class DueAndPaidDataImportingController : System.Web.UI.Page
    {
        string message = "";
        int insertCount = 0;
        string BDD_UPDATE_DUP_POL = "";
        private readonly string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;


        public string GetTextFieldValue(DataRow dataRow, string fieldName)
        {
            try
            {
                if (dataRow.Table.Columns.Contains(fieldName))
                {
                    return dataRow[fieldName] != DBNull.Value ? dataRow[fieldName].ToString() : null;
                }
                else
                {
                    throw new ArgumentException($"Field '{fieldName}' does not exist in the DataRow.");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static object ConvertToDDMMYY(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return DBNull.Value;

            DateTime parsedDate;

            // Try parsing the input string as a DateTime with any format
            if (DateTime.TryParse(dateString, out parsedDate))
            {
                // Return the date in ddMMyy format
                string FF=  parsedDate.ToString("dd/MM/yy", CultureInfo.InvariantCulture);
                return FF;
            }

            // Return DBNull if the date could not be parsed
            return DBNull.Value;
        }


        public static object HandleStringValue(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
               return DBNull.Value;
            }
            else
            {

            return inputString;
            }

        }


        public static object HandleIntValue(int inputString)
        {
            if (inputString == 0)
            {
                return DBNull.Value;
            }
            else
            {
                return inputString;
            }

        }

        public static object HandleDecimalValue(decimal? inputValue)
        {
            if (inputValue == null || inputValue == 0)
            {
            return  DBNull.Value;

            }
            else
            {

                return inputValue;
            }
        }


        public static object HandleNullableIntValue(object inputValue)
        {
            if (inputValue == null || inputValue == DBNull.Value || string.IsNullOrWhiteSpace(inputValue.ToString()))
            {
                return DBNull.Value;
            }

            return Convert.ToInt32(inputValue);
        }


        
        public string InsertDueData_N(DataTable dataTable, int monthValue, int yearValue, string ddlImportDataTypeValue, string MyImportDataType, string MyImport, string chkDataTypeValue, string loginId, string fileName)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_DAP_INSERT_N_BDD", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add all parameters here
                        cmd.Parameters.Add("p_COMPANY_CD", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "COMPANY_CD");// row["COMPANY_CD"];
                        cmd.Parameters.Add("p_STATUS_CD", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "STATUS_CD");//row["STATUS_CD"];
                        cmd.Parameters.Add("p_LOCATION", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "LOCATION");//row["LOCATION"];
                        cmd.Parameters.Add("p_POLICY_NO", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "POLICY_NO");//row["POLICY_NO"];
                        cmd.Parameters.Add("p_CL_NAME", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "CL_NAME");//row["CL_NAME"];
                        cmd.Parameters.Add("p_PREM_AMT", OracleDbType.Decimal).Value = HandleDecimalValue(row["PREM_AMT"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PREM_AMT"]));

                        cmd.Parameters.Add("p_PAY_MODE", OracleDbType.Varchar2).Value = row["PAY_MODE"];
                        cmd.Parameters.Add("p_DUE_DATE", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["DUE_DATE"].ToString());
                        cmd.Parameters.Add("p_CL_ADD1", OracleDbType.Varchar2).Value = row["CL_ADD1"];
                        cmd.Parameters.Add("p_CL_ADD2", OracleDbType.Varchar2).Value = row["CL_ADD2"];
                        cmd.Parameters.Add("p_CL_ADD3", OracleDbType.Varchar2).Value = row["CL_ADD3"];
                        cmd.Parameters.Add("p_CL_CITY", OracleDbType.Varchar2).Value = row["CL_CITY"];
                        cmd.Parameters.Add("p_CL_PIN", OracleDbType.Varchar2).Value = row["CL_PIN"];
                        cmd.Parameters.Add("p_CL_PHONE1", OracleDbType.Varchar2).Value = row["CL_PHONE1"];
                        cmd.Parameters.Add("p_CL_PHONE2", OracleDbType.Varchar2).Value = row["CL_PHONE2"];
                        cmd.Parameters.Add("p_CL_MOBILE", OracleDbType.Varchar2).Value = row["CL_MOBILE"];
                        cmd.Parameters.Add("p_MON_NO", OracleDbType.Int32).Value = HandleIntValue(monthValue); // row["MON_NO"];
                        cmd.Parameters.Add("p_YEAR_NO", OracleDbType.Int32).Value = HandleIntValue(yearValue); // row["YEAR_NO"];
                        cmd.Parameters.Add("p_USERID", OracleDbType.Varchar2).Value = HandleStringValue(loginId);
                        cmd.Parameters.Add("p_IMPORT_DT", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["IMPORT_DT"].ToString());
                        cmd.Parameters.Add("p_EMP_NO", OracleDbType.Varchar2).Value = row["EMP_NO"];
                        cmd.Parameters.Add("p_INV_CD", OracleDbType.Varchar2).Value = row["INV_CD"];
                        cmd.Parameters.Add("p_DUP_REC", OracleDbType.Varchar2).Value = row["DUP_REC"];
                        cmd.Parameters.Add("p_PLAN_NAME", OracleDbType.Varchar2).Value = row["PLAN_NAME"];
                        cmd.Parameters.Add("p_PREM_FREQ", OracleDbType.Varchar2).Value = row["PREM_FREQ"];
                        cmd.Parameters.Add("p_DOC", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["DOC"].ToString());
                        cmd.Parameters.Add("p_BRANCH_CD", OracleDbType.Varchar2).Value = row["BRANCH_CD"];
                        cmd.Parameters.Add("p_AR_BRANCH_CD", OracleDbType.Varchar2).Value = row["AR_BRANCH_CD"];
                        cmd.Parameters.Add("p_PLY_TERM", OracleDbType.Int32).Value = HandleNullableIntValue(row["PLY_TERM"]);
                        cmd.Parameters.Add("p_CL_ADD4", OracleDbType.Varchar2).Value = row["CL_ADD4"];
                        cmd.Parameters.Add("p_CL_ADD5", OracleDbType.Varchar2).Value = row["CL_ADD5"];
                        cmd.Parameters.Add("p_PLAN_NO", OracleDbType.Varchar2).Value = row["PLAN_NO"];
                        cmd.Parameters.Add("p_BPREM_FREQ", OracleDbType.Varchar2).Value = row["BPREM_FREQ"];
                        cmd.Parameters.Add("p_PREM_TERM", OracleDbType.Int32).Value = HandleNullableIntValue(row["PREM_TERM"]);
                        cmd.Parameters.Add("p_LAST_UPDATE_DT", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["LAST_UPDATE_DT"].ToString());
                        cmd.Parameters.Add("p_LAST_UPDATE", OracleDbType.Varchar2).Value = row["LAST_UPDATE"];
                        cmd.Parameters.Add("p_FRESH_RENEWAL4", OracleDbType.Varchar2).Value = row["FRESH_RENEWAL4"];
                        cmd.Parameters.Add("p_SYS_AR_NO", OracleDbType.Varchar2).Value = row["SYS_AR_NO"];
                        cmd.Parameters.Add("p_MAIL_FLAG", OracleDbType.Varchar2).Value = row["MAIL_FLAG"];
                        cmd.Parameters.Add("p_SEND_ID", OracleDbType.Varchar2).Value = row["SEND_ID"];
                        cmd.Parameters.Add("p_SEND_DATE", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["SEND_DATE"].ToString());
                        cmd.Parameters.Add("p_APP_NO", OracleDbType.Varchar2).Value = row["APP_NO"];
                        cmd.Parameters.Add("p_MATCHED", OracleDbType.Varchar2).Value = row["MATCHED"];
                        cmd.Parameters.Add("p_REN_STATUS", OracleDbType.Varchar2).Value = row["REN_STATUS"];
                        cmd.Parameters.Add("p_REMARKS", OracleDbType.Varchar2).Value = row["REMARKS"];
                        cmd.Parameters.Add("p_IMPORTDATATYPE", OracleDbType.Varchar2).Value = row["IMPORTDATATYPE"];
                        cmd.Parameters.Add("p_HISTORY", OracleDbType.Varchar2).Value = row["HISTORY"];
                        cmd.Parameters.Add("p_SYS_AR_NO_REN", OracleDbType.Varchar2).Value = row["SYS_AR_NO_REN"];
                        cmd.Parameters.Add("p_AUTOMAP", OracleDbType.Varchar2).Value = row["AUTOMAP"];
                        cmd.Parameters.Add("p_CLIENT_CD", OracleDbType.Varchar2).Value = row["CLIENT_CD"];
                        cmd.Parameters.Add("p_INV_CODE", OracleDbType.Varchar2).Value = row["INV_CODE"];
                        cmd.Parameters.Add("p_P_NAME", OracleDbType.Varchar2).Value = row["P_NAME"];
                        cmd.Parameters.Add("p_I_NAME", OracleDbType.Varchar2).Value = row["I_NAME"];
                        cmd.Parameters.Add("p_SA", OracleDbType.Decimal).Value = HandleDecimalValue(row["SA"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["SA"]));
                        cmd.Parameters.Add("p_REM_FLAGE", OracleDbType.Varchar2).Value = row["REM_FLAGE"];
                        cmd.Parameters.Add("p_STATE_CD", OracleDbType.Varchar2).Value = row["STATE_CD"];
                        cmd.Parameters.Add("p_COMM", OracleDbType.Decimal).Value = row["COMM"];
                        cmd.Parameters.Add("p_COMM_AMT", OracleDbType.Decimal).Value = HandleDecimalValue(row["COMM_AMT"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["COMM_AMT"])); ;
                        cmd.Parameters.Add("p_BASE_SYS_AR_NO", OracleDbType.Varchar2).Value = row["BASE_SYS_AR_NO"];
                        cmd.Parameters.Add("p_MATCH_HEADER", OracleDbType.Varchar2).Value = row["MATCH_HEADER"];
                        cmd.Parameters.Add("p_CPREM_FREQ", OracleDbType.Varchar2).Value = row["CPREM_FREQ"];
                        cmd.Parameters.Add("p_MATCH_HEADER1", OracleDbType.Varchar2).Value = row["MATCH_HEADER1"];
                        cmd.Parameters.Add("p_NEWINSERT", OracleDbType.Varchar2).Value = row["NEWINSERT"];
                        cmd.Parameters.Add("p_SLOT", OracleDbType.Varchar2).Value = row["SLOT"];
                        cmd.Parameters.Add("p_TALISMA_FLAG", OracleDbType.Varchar2).Value = row["TALISMA_FLAG"];
                        cmd.Parameters.Add("p_MARGIN4", OracleDbType.Decimal).Value = HandleDecimalValue(row["MARGIN4"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["MARGIN4"]));
                        cmd.Parameters.Add("p_SMS_FLAG", OracleDbType.Varchar2).Value = row["SMS_FLAG"];
                        cmd.Parameters.Add("p_FORCE_FLAG", OracleDbType.Varchar2).Value = row["FORCE_FLAG"];
                        cmd.Parameters.Add("p_UPD_FLAG", OracleDbType.Varchar2).Value = row["UPD_FLAG"];
                        cmd.Parameters.Add("p_PLAN_UPD", OracleDbType.Varchar2).Value = row["PLAN_UPD"];
                        cmd.Parameters.Add("p_FREQ_UPD", OracleDbType.Varchar2).Value = row["FREQ_UPD"];
                        cmd.Parameters.Add("p_ACTIVE_CLIENT", OracleDbType.Varchar2).Value = row["ACTIVE_CLIENT"];
                        cmd.Parameters.Add("p_P_ADD1", OracleDbType.Varchar2).Value = row["P_ADD1"];
                        cmd.Parameters.Add("p_P_ADD2", OracleDbType.Varchar2).Value = row["P_ADD2"];
                        cmd.Parameters.Add("p_P_CITY", OracleDbType.Varchar2).Value = row["P_CITY"];
                        cmd.Parameters.Add("p_P_STATE_CD", OracleDbType.Varchar2).Value = row["P_STATE_CD"];
                        cmd.Parameters.Add("p_P_PIN", OracleDbType.Varchar2).Value = row["P_PIN"];
                        cmd.Parameters.Add("p_IADD1", OracleDbType.Varchar2).Value = row["IADD1"];
                        cmd.Parameters.Add("p_IADD2", OracleDbType.Varchar2).Value = row["IADD2"];
                        cmd.Parameters.Add("p_ICITY", OracleDbType.Varchar2).Value = row["ICITY"];
                        cmd.Parameters.Add("p_ISTATE_CD", OracleDbType.Varchar2).Value = row["ISTATE_CD"];
                        cmd.Parameters.Add("p_IPIN", OracleDbType.Varchar2).Value = row["IPIN"];
                        cmd.Parameters.Add("p_PLAN_RATE4", OracleDbType.Decimal).Value = HandleDecimalValue(row["PLAN_RATE4"] == DBNull.Value ?
                            (decimal?)null : Convert.ToDecimal(row["PLAN_RATE4"]));
                        cmd.Parameters.Add("p_PAID_MONTH_YEAR", OracleDbType.Varchar2).Value = row["PAID_MONTH_YEAR"];
                        cmd.Parameters.Add("p_STATUS_CD_OLD", OracleDbType.Varchar2).Value = row["STATUS_CD_OLD"];
                        cmd.Parameters.Add("p_FREQ", OracleDbType.Varchar2).Value = row["FREQ"];
                        cmd.Parameters.Add("p_FRESH_RENEWAL", OracleDbType.Varchar2).Value = row["FRESH_RENEWAL"];
                        cmd.Parameters.Add("p_MARGIN", OracleDbType.Decimal).Value = HandleDecimalValue(row["MARGIN"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["MARGIN"]));
                        cmd.Parameters.Add("p_PLAN_RATE", OracleDbType.Decimal).Value = HandleDecimalValue(row["PLAN_RATE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PLAN_RATE"])); ;
                        cmd.Parameters.Add("p_RENEWAL_STATUS", OracleDbType.Varchar2).Value = row["RENEWAL_STATUS"];
                        cmd.Parameters.Add("p_FLAG_13M", OracleDbType.Varchar2).Value = row["FLAG_13M"];
                        cmd.Parameters.Add("p_ECS_DATE", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["ECS_DATE"].ToString());
                        cmd.Parameters.Add("p_NEW_STATUS_CD", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "NEW_STATUS_CD");
                        cmd.Parameters.Add("p_LOGIN", OracleDbType.Varchar2).Value = loginId;
                        cmd.Parameters.Add("p_ddlImportDataTypeValue", OracleDbType.Varchar2).Value = ddlImportDataTypeValue;
                        cmd.Parameters.Add("p_MyImportDataType", OracleDbType.Varchar2).Value = MyImportDataType;
                        cmd.Parameters.Add("p_MyImport", OracleDbType.Varchar2).Value = MyImport;
                        cmd.Parameters.Add("p_chkDataTypeValue", OracleDbType.Varchar2).Value = chkDataTypeValue;
                        cmd.Parameters.Add("p_FILE_NAME", OracleDbType.Varchar2).Value = fileName;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            message = "success";
                            insertCount += 1;
                        }
                        catch (OracleException ex)
                        {
                            string returnMsg = $"Error inserting row: {ex.Message}";
                            message = returnMsg;
                        }
                    }
                }
            }
            return $"Insert count: {insertCount.ToString()} rows. Message: {message}";

        }

        public string InsertDueData_N_For_Paid(DataTable dataTable, int monthValue, int yearValue, string ddlImportDataTypeValue, string MyImportDataType, string MyImport, string chkDataTypeValue, string loginId, string fileName)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_DAP_INSERT_BPD_ONLY_BDD", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add all parameters here
                        cmd.Parameters.Add("p_COMPANY_CD", OracleDbType.Varchar2).Value     = GetTextFieldValue(row, "COMPANY_CD");// row["COMPANY_CD"];
                        cmd.Parameters.Add("p_STATUS_CD", OracleDbType.Varchar2).Value      = GetTextFieldValue(row,"STATUS_CD");//row["STATUS_CD"];
                        cmd.Parameters.Add("p_LOCATION", OracleDbType.Varchar2).Value       = GetTextFieldValue(row,"LOCATION");//row["LOCATION"];
                        cmd.Parameters.Add("p_POLICY_NO", OracleDbType.Varchar2).Value      = GetTextFieldValue(row,"POLICY_NO");//row["POLICY_NO"];
                        cmd.Parameters.Add("p_CL_NAME", OracleDbType.Varchar2).Value        = GetTextFieldValue(row, "CL_NAME");//row["CL_NAME"];
                        cmd.Parameters.Add("p_PREM_AMT", OracleDbType.Decimal).Value        = HandleDecimalValue(row["PREM_AMT"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PREM_AMT"]));
                
                        cmd.Parameters.Add("p_PAY_MODE", OracleDbType.Varchar2).Value       = row["PAY_MODE"];
                        cmd.Parameters.Add("p_DUE_DATE", OracleDbType.Varchar2).Value       = ConvertToDDMMYY(row["DUE_DATE"].ToString());
                        cmd.Parameters.Add("p_CL_ADD1", OracleDbType.Varchar2).Value        = row["CL_ADD1"];
                        cmd.Parameters.Add("p_CL_ADD2", OracleDbType.Varchar2).Value        = row["CL_ADD2"];
                        cmd.Parameters.Add("p_CL_ADD3", OracleDbType.Varchar2).Value        = row["CL_ADD3"];
                        cmd.Parameters.Add("p_CL_CITY", OracleDbType.Varchar2).Value        = row["CL_CITY"];
                        cmd.Parameters.Add("p_CL_PIN", OracleDbType.Varchar2).Value         = row["CL_PIN"];
                        cmd.Parameters.Add("p_CL_PHONE1", OracleDbType.Varchar2).Value      = row["CL_PHONE1"];
                        cmd.Parameters.Add("p_CL_PHONE2", OracleDbType.Varchar2).Value      = row["CL_PHONE2"];
                        cmd.Parameters.Add("p_CL_MOBILE", OracleDbType.Varchar2).Value      = row["CL_MOBILE"];
                        cmd.Parameters.Add("p_MON_NO", OracleDbType.Int32).Value            = HandleIntValue(monthValue); // row["MON_NO"];
                        cmd.Parameters.Add("p_YEAR_NO", OracleDbType.Int32).Value           = HandleIntValue(yearValue); // row["YEAR_NO"];
                        cmd.Parameters.Add("p_USERID", OracleDbType.Varchar2).Value         = HandleStringValue(loginId);
                        cmd.Parameters.Add("p_IMPORT_DT", OracleDbType.Varchar2).Value          = ConvertToDDMMYY(row["IMPORT_DT"].ToString()) ;
                        cmd.Parameters.Add("p_EMP_NO", OracleDbType.Varchar2).Value = row["EMP_NO"];
                        cmd.Parameters.Add("p_INV_CD", OracleDbType.Varchar2).Value = row["INV_CD"];
                        cmd.Parameters.Add("p_DUP_REC", OracleDbType.Varchar2).Value = row["DUP_REC"];
                        cmd.Parameters.Add("p_PLAN_NAME", OracleDbType.Varchar2).Value = row["PLAN_NAME"];
                        cmd.Parameters.Add("p_PREM_FREQ", OracleDbType.Varchar2).Value = row["PREM_FREQ"];
                        cmd.Parameters.Add("p_DOC", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["DOC"].ToString());
                        cmd.Parameters.Add("p_BRANCH_CD", OracleDbType.Varchar2).Value = row["BRANCH_CD"];
                        cmd.Parameters.Add("p_AR_BRANCH_CD", OracleDbType.Varchar2).Value = row["AR_BRANCH_CD"];
                        cmd.Parameters.Add("p_PLY_TERM", OracleDbType.Int32).Value          = HandleNullableIntValue(row["PLY_TERM"]);
                        cmd.Parameters.Add("p_CL_ADD4", OracleDbType.Varchar2).Value = row["CL_ADD4"];
                        cmd.Parameters.Add("p_CL_ADD5", OracleDbType.Varchar2).Value = row["CL_ADD5"];
                        cmd.Parameters.Add("p_PLAN_NO", OracleDbType.Varchar2).Value = row["PLAN_NO"];
                        cmd.Parameters.Add("p_BPREM_FREQ", OracleDbType.Varchar2).Value = row["BPREM_FREQ"];
                        cmd.Parameters.Add("p_PREM_TERM", OracleDbType.Int32).Value = HandleNullableIntValue(row["PREM_TERM"]);
                        cmd.Parameters.Add("p_LAST_UPDATE_DT", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["LAST_UPDATE_DT"].ToString());
                        cmd.Parameters.Add("p_LAST_UPDATE", OracleDbType.Varchar2).Value = row["LAST_UPDATE"];
                        cmd.Parameters.Add("p_FRESH_RENEWAL4", OracleDbType.Varchar2).Value = row["FRESH_RENEWAL4"];
                        cmd.Parameters.Add("p_SYS_AR_NO", OracleDbType.Varchar2).Value = row["SYS_AR_NO"];
                        cmd.Parameters.Add("p_MAIL_FLAG", OracleDbType.Varchar2).Value = row["MAIL_FLAG"];
                        cmd.Parameters.Add("p_SEND_ID", OracleDbType.Varchar2).Value = row["SEND_ID"];
                        cmd.Parameters.Add("p_SEND_DATE", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["SEND_DATE"].ToString());
                        cmd.Parameters.Add("p_APP_NO", OracleDbType.Varchar2).Value = row["APP_NO"];
                        cmd.Parameters.Add("p_MATCHED", OracleDbType.Varchar2).Value = row["MATCHED"];
                        cmd.Parameters.Add("p_REN_STATUS", OracleDbType.Varchar2).Value = row["REN_STATUS"];
                        cmd.Parameters.Add("p_REMARKS", OracleDbType.Varchar2).Value = row["REMARKS"];
                        cmd.Parameters.Add("p_IMPORTDATATYPE", OracleDbType.Varchar2).Value = row["IMPORTDATATYPE"];
                        cmd.Parameters.Add("p_HISTORY", OracleDbType.Varchar2).Value = row["HISTORY"];
                        cmd.Parameters.Add("p_SYS_AR_NO_REN", OracleDbType.Varchar2).Value = row["SYS_AR_NO_REN"];
                        cmd.Parameters.Add("p_AUTOMAP", OracleDbType.Varchar2).Value = row["AUTOMAP"];
                        cmd.Parameters.Add("p_CLIENT_CD", OracleDbType.Varchar2).Value = row["CLIENT_CD"];
                        cmd.Parameters.Add("p_INV_CODE", OracleDbType.Varchar2).Value = row["INV_CODE"];
                        cmd.Parameters.Add("p_P_NAME", OracleDbType.Varchar2).Value = row["P_NAME"];
                        cmd.Parameters.Add("p_I_NAME", OracleDbType.Varchar2).Value = row["I_NAME"];
                        cmd.Parameters.Add("p_SA", OracleDbType.Decimal).Value              = HandleDecimalValue(row["SA"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["SA"]));
                        cmd.Parameters.Add("p_REM_FLAGE", OracleDbType.Varchar2).Value = row["REM_FLAGE"];
                        cmd.Parameters.Add("p_STATE_CD", OracleDbType.Varchar2).Value = row["STATE_CD"];
                        cmd.Parameters.Add("p_COMM", OracleDbType.Decimal).Value = row["COMM"];
                        cmd.Parameters.Add("p_COMM_AMT", OracleDbType.Decimal).Value = HandleDecimalValue(row["COMM_AMT"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["COMM_AMT"]));;
                        cmd.Parameters.Add("p_BASE_SYS_AR_NO", OracleDbType.Varchar2).Value = row["BASE_SYS_AR_NO"];
                        cmd.Parameters.Add("p_MATCH_HEADER", OracleDbType.Varchar2).Value = row["MATCH_HEADER"];
                        cmd.Parameters.Add("p_CPREM_FREQ", OracleDbType.Varchar2).Value = row["CPREM_FREQ"];
                        cmd.Parameters.Add("p_MATCH_HEADER1", OracleDbType.Varchar2).Value = row["MATCH_HEADER1"];
                        cmd.Parameters.Add("p_NEWINSERT", OracleDbType.Varchar2).Value = row["NEWINSERT"];
                        cmd.Parameters.Add("p_SLOT", OracleDbType.Varchar2).Value = row["SLOT"];
                        cmd.Parameters.Add("p_TALISMA_FLAG", OracleDbType.Varchar2).Value = row["TALISMA_FLAG"];
                        cmd.Parameters.Add("p_MARGIN4", OracleDbType.Decimal).Value = HandleDecimalValue(row["MARGIN4"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["MARGIN4"]));                      
                        cmd.Parameters.Add("p_SMS_FLAG", OracleDbType.Varchar2).Value = row["SMS_FLAG"];
                        cmd.Parameters.Add("p_FORCE_FLAG", OracleDbType.Varchar2).Value =  row["FORCE_FLAG"];
                        cmd.Parameters.Add("p_UPD_FLAG", OracleDbType.Varchar2).Value = row["UPD_FLAG"];
                        cmd.Parameters.Add("p_PLAN_UPD", OracleDbType.Varchar2).Value = row["PLAN_UPD"];
                        cmd.Parameters.Add("p_FREQ_UPD", OracleDbType.Varchar2).Value = row["FREQ_UPD"];
                        cmd.Parameters.Add("p_ACTIVE_CLIENT", OracleDbType.Varchar2).Value = row["ACTIVE_CLIENT"];
                        cmd.Parameters.Add("p_P_ADD1", OracleDbType.Varchar2).Value = row["P_ADD1"];
                        cmd.Parameters.Add("p_P_ADD2", OracleDbType.Varchar2).Value = row["P_ADD2"];
                        cmd.Parameters.Add("p_P_CITY", OracleDbType.Varchar2).Value = row["P_CITY"];
                        cmd.Parameters.Add("p_P_STATE_CD", OracleDbType.Varchar2).Value = row["P_STATE_CD"];
                        cmd.Parameters.Add("p_P_PIN", OracleDbType.Varchar2).Value = row["P_PIN"];
                        cmd.Parameters.Add("p_IADD1", OracleDbType.Varchar2).Value = row["IADD1"];
                        cmd.Parameters.Add("p_IADD2", OracleDbType.Varchar2).Value = row["IADD2"];
                        cmd.Parameters.Add("p_ICITY", OracleDbType.Varchar2).Value = row["ICITY"];
                        cmd.Parameters.Add("p_ISTATE_CD", OracleDbType.Varchar2).Value = row["ISTATE_CD"];
                        cmd.Parameters.Add("p_IPIN", OracleDbType.Varchar2).Value = row["IPIN"];
                        cmd.Parameters.Add("p_PLAN_RATE4", OracleDbType.Decimal).Value = HandleDecimalValue(row["PLAN_RATE4"] == DBNull.Value ?
                            (decimal?)null : Convert.ToDecimal(row["PLAN_RATE4"]));
                        cmd.Parameters.Add("p_PAID_MONTH_YEAR", OracleDbType.Varchar2).Value = row["PAID_MONTH_YEAR"];
                        cmd.Parameters.Add("p_STATUS_CD_OLD", OracleDbType.Varchar2).Value = row["STATUS_CD_OLD"];
                        cmd.Parameters.Add("p_FREQ", OracleDbType.Varchar2).Value = row["FREQ"];
                        cmd.Parameters.Add("p_FRESH_RENEWAL", OracleDbType.Varchar2).Value = row["FRESH_RENEWAL"];
                        cmd.Parameters.Add("p_MARGIN", OracleDbType.Decimal).Value = HandleDecimalValue(row["MARGIN"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["MARGIN"]));
                        cmd.Parameters.Add("p_PLAN_RATE", OracleDbType.Decimal).Value = HandleDecimalValue(row["PLAN_RATE"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PLAN_RATE"])); ;
                        cmd.Parameters.Add("p_RENEWAL_STATUS", OracleDbType.Varchar2).Value = row["RENEWAL_STATUS"];
                        cmd.Parameters.Add("p_FLAG_13M", OracleDbType.Varchar2).Value = row["FLAG_13M"];
                        cmd.Parameters.Add("p_ECS_DATE", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["ECS_DATE"].ToString());
                        cmd.Parameters.Add("p_NEW_STATUS_CD", OracleDbType.Varchar2).Value =  row["NEW_STATUS_CD"];
                        cmd.Parameters.Add("p_LOGIN", OracleDbType.Varchar2).Value = loginId;
                        cmd.Parameters.Add("p_ddlImportDataTypeValue", OracleDbType.Varchar2).Value = ddlImportDataTypeValue;
                        cmd.Parameters.Add("p_MyImportDataType", OracleDbType.Varchar2).Value = MyImportDataType;
                        cmd.Parameters.Add("p_MyImport", OracleDbType.Varchar2).Value = MyImport;
                        cmd.Parameters.Add("p_chkDataTypeValue", OracleDbType.Varchar2).Value = chkDataTypeValue;
                        cmd.Parameters.Add("p_FILE_NAME", OracleDbType.Varchar2).Value = fileName;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            message =  "success";
                            insertCount += 1;
                        }
                        catch (OracleException ex)
                        {
                            string returnMsg = $"Error inserting row: {ex.Message}";
                            message =  returnMsg;
                        }
                    }
                }
            }
            return $"Insert count: {insertCount.ToString()} rows. Message: {message}";

        }

        public string InsertPaidData_N(DataTable dataTable, int monthValue, int yearValue, string ddlImportDataTypeValue, string MyImportDataType, string MyImport, string chkDataTypeValue, string loginId, string fileName)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;
            string message = string.Empty;
            int insertCount = 0;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                foreach (DataRow row in dataTable.Rows)
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_DAP_INSERT_N_BPD", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add all parameters here
                        cmd.Parameters.Add("p_COMPANY_CD", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "COMPANY_CD");
                        cmd.Parameters.Add("p_STATUS_CD", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "STATUS_CD");
                        cmd.Parameters.Add("p_LOCATION", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "LOCATION");
                        cmd.Parameters.Add("p_POLICY_NO", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "POLICY_NO");
                        cmd.Parameters.Add("p_CL_NAME", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "CL_NAME");
                        cmd.Parameters.Add("p_PREM_AMT", OracleDbType.Decimal).Value = HandleDecimalValue(row["PREM_AMT"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PREM_AMT"]));
                        cmd.Parameters.Add("p_PAY_MODE", OracleDbType.Varchar2).Value = row["PAY_MODE"];
                        cmd.Parameters.Add("p_DUE_DATE", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["DUE_DATE"].ToString());
                        cmd.Parameters.Add("p_CL_ADD1", OracleDbType.Varchar2).Value = row["CL_ADD1"];
                        cmd.Parameters.Add("p_CL_ADD2", OracleDbType.Varchar2).Value = row["CL_ADD2"];
                        cmd.Parameters.Add("p_CL_ADD3", OracleDbType.Varchar2).Value = row["CL_ADD3"];
                        cmd.Parameters.Add("p_CL_CITY", OracleDbType.Varchar2).Value = row["CL_CITY"];
                        cmd.Parameters.Add("p_CL_PIN", OracleDbType.Varchar2).Value = row["CL_PIN"];
                        cmd.Parameters.Add("p_CL_PHONE1", OracleDbType.Varchar2).Value = row["CL_PHONE1"];
                        cmd.Parameters.Add("p_CL_PHONE2", OracleDbType.Varchar2).Value = row["CL_PHONE2"];
                        cmd.Parameters.Add("p_CL_MOBILE", OracleDbType.Varchar2).Value = row["CL_MOBILE"];
                        cmd.Parameters.Add("p_MON_NO", OracleDbType.Int32).Value = monthValue;
                        cmd.Parameters.Add("p_YEAR_NO", OracleDbType.Int32).Value = yearValue;
                        cmd.Parameters.Add("p_USERID", OracleDbType.Varchar2).Value = loginId;
                        cmd.Parameters.Add("p_IMPORT_DT", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["IMPORT_DT"].ToString());
                        cmd.Parameters.Add("p_EMP_NO", OracleDbType.Int64).Value = GetTextFieldValue(row,"EMP_NO");
                        cmd.Parameters.Add("p_INV_CD", OracleDbType.Varchar2).Value = row["INV_CD"];
                        cmd.Parameters.Add("p_DUP_REC", OracleDbType.Varchar2).Value = row["DUP_REC"];
                        cmd.Parameters.Add("p_PLY_TERM", OracleDbType.Int32).Value = HandleNullableIntValue(row["PLY_TERM"]);
                        cmd.Parameters.Add("p_CL_ADD4", OracleDbType.Varchar2).Value = row["CL_ADD4"];
                        cmd.Parameters.Add("p_CL_ADD5", OracleDbType.Varchar2).Value = row["CL_ADD5"];
                        cmd.Parameters.Add("p_PLAN_NAME", OracleDbType.Varchar2).Value = row["PLAN_NAME"];
                        cmd.Parameters.Add("p_PREM_FREQ", OracleDbType.Varchar2).Value = row["PREM_FREQ"];
                        cmd.Parameters.Add("p_DOC", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["DOC"].ToString());
                        cmd.Parameters.Add("p_BRANCH_CD", OracleDbType.Int64).Value = HandleNullableIntValue(row["BRANCH_CD"]);

                        cmd.Parameters.Add("p_AR_BRANCH_CD", OracleDbType.Int64).Value = HandleNullableIntValue(row["AR_BRANCH_CD"]);
                        cmd.Parameters.Add("p_PLAN_NO", OracleDbType.Int64).Value = HandleNullableIntValue(row["PLAN_NO"]);
                        cmd.Parameters.Add("p_BPREM_FREQ", OracleDbType.Decimal).Value = HandleNullableIntValue(row["BPREM_FREQ"]);
                        cmd.Parameters.Add("p_LAST_UPDATE_DT", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["LAST_UPDATE_DT"].ToString());
                        cmd.Parameters.Add("p_LAST_UPDATE", OracleDbType.Varchar2).Value = row["LAST_UPDATE"];
                        cmd.Parameters.Add("p_IMPORTDATATYPE", OracleDbType.Varchar2).Value = MyImportDataType;
                        cmd.Parameters.Add("p_NEWINSERT", OracleDbType.Varchar2).Value = row["NEWINSERT"];
                        cmd.Parameters.Add("p_WEALTHMAKER_UPDATE", OracleDbType.Char).Value =GetTextFieldValue(row,"WEALTHMAKER_UPDATE");
                        cmd.Parameters.Add("p_FORCE_FLAG", OracleDbType.Varchar2).Value = row["FORCE_FLAG"];
                        cmd.Parameters.Add("p_PREM_TERM", OracleDbType.Int32).Value = HandleNullableIntValue(row["PREM_TERM"]);
                        cmd.Parameters.Add("p_FRESH_RENEWAL", OracleDbType.Int32).Value = HandleNullableIntValue(GetTextFieldValue(row, "FRESH_RENEWAL"));
                        cmd.Parameters.Add("p_SYS_AR_NO", OracleDbType.Varchar2).Value = GetTextFieldValue(row, "SYS_AR_NO");
                        cmd.Parameters.Add("p_AR_GEN", OracleDbType.Char).Value = GetTextFieldValue(row, "AR_GEN");
                        cmd.Parameters.Add("p_ACTIVE_CLIENT", OracleDbType.Int32).Value = HandleNullableIntValue(row["ACTIVE_CLIENT"]);
                        string plan_rate1 = GetTextFieldValue(row, "PLAN_RATE");
                        cmd.Parameters.Add("p_PLAN_RATE", OracleDbType.Decimal).Value = null;
                            //HandleDecimalValue(plan_rate1) == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["PLAN_RATE"]));

                        cmd.Parameters.Add("p_MARGIN", OracleDbType.Decimal).Value = HandleDecimalValue(row["MARGIN"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["MARGIN"]));
                        cmd.Parameters.Add("p_SMS_FLAG", OracleDbType.Char).Value = row["SMS_FLAG"];
                        cmd.Parameters.Add("p_UPD_FLAG", OracleDbType.Char).Value = row["UPD_FLAG"];
                        cmd.Parameters.Add("p_PLAN_UPD", OracleDbType.Varchar2).Value = row["PLAN_UPD"];
                        cmd.Parameters.Add("p_FREQ_UPD", OracleDbType.Varchar2).Value = row["FREQ_UPD"];
                        cmd.Parameters.Add("p_P_ADD1", OracleDbType.Varchar2).Value = row["P_ADD1"];
                        cmd.Parameters.Add("p_P_ADD2", OracleDbType.Varchar2).Value = row["P_ADD2"];
                        cmd.Parameters.Add("p_P_CITY", OracleDbType.Varchar2).Value = row["P_CITY"];
                        cmd.Parameters.Add("p_P_STATE_CD", OracleDbType.Int32).Value = HandleNullableIntValue(row["P_STATE_CD"]);
                        cmd.Parameters.Add("p_P_PIN", OracleDbType.Varchar2).Value = row["P_PIN"];
                        cmd.Parameters.Add("p_IADD1", OracleDbType.Varchar2).Value = row["IADD1"];
                        cmd.Parameters.Add("p_IADD2", OracleDbType.Varchar2).Value = row["IADD2"];
                        cmd.Parameters.Add("p_ICITY", OracleDbType.Varchar2).Value = row["ICITY"];
                        cmd.Parameters.Add("p_ISTATE_CD", OracleDbType.Int32).Value = HandleNullableIntValue(row["ISTATE_CD"]);
                        cmd.Parameters.Add("p_IPIN", OracleDbType.Varchar2).Value = row["IPIN"];
                        cmd.Parameters.Add("p_DUE_MONTH_YEAR", OracleDbType.Varchar2).Value = ConvertToDDMMYY(row["DUE_MONTH_YEAR"].ToString());
                        cmd.Parameters.Add("p_FLAG_13M", OracleDbType.Int32).Value = HandleNullableIntValue(row["FLAG_13M"]);
                        cmd.Parameters.Add("p_LOGIN", OracleDbType.Varchar2).Value = loginId;
                        cmd.Parameters.Add("p_ddlImportDataTypeValue", OracleDbType.Varchar2).Value = ddlImportDataTypeValue;
                        cmd.Parameters.Add("p_MyImportDataType", OracleDbType.Varchar2).Value = MyImportDataType;
                        cmd.Parameters.Add("p_MyImport", OracleDbType.Varchar2).Value = MyImport;
                        cmd.Parameters.Add("p_chkDataTypeValue", OracleDbType.Varchar2).Value = chkDataTypeValue;
                        cmd.Parameters.Add("p_FILE_NAME", OracleDbType.Varchar2).Value = fileName;


                        try
                        {
                            cmd.ExecuteNonQuery();
                            message = "success";
                            insertCount += 1;
                        }
                        catch (OracleException ex)
                        {
                            string returnMsg = $"Error inserting row: {ex.Message}";
                            message = returnMsg;
                        }
                    }
                }
            }

            return $"Insert count: {insertCount.ToString()} rows. Message: {message}";

        }



        #region UpdateDuplicatePolicies
        public string BDD_UpdateDuplicatePolicies(int monthNo, int yearNo, string importDataType)
        {
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_DAP_DUE_UP_DUP_POL", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_month_no", OracleDbType.Int32).Value = monthNo;
                    cmd.Parameters.Add("p_year_no", OracleDbType.Int32).Value = yearNo;
                    cmd.Parameters.Add("p_import_data_type", OracleDbType.Varchar2).Value = importDataType;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        BDD_UPDATE_DUP_POL = "Duplicate policy update successfuly";
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        BDD_UPDATE_DUP_POL = ex.Message;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            return BDD_UPDATE_DUP_POL;
        }
        #endregion


        public List<int> GetYearsList(int numberOfYears, int startYear = -1)
        {
            List<int> yearsList = new List<int>();
            int currentYear = startYear > 0 ? startYear : DateTime.Now.Year;

            for (int i = 0; i < numberOfYears; i++)
            {
                yearsList.Add(currentYear - i);
            }

            yearsList.Reverse(); // Optional: If you want the list in ascending order
            return yearsList;
        }



        #region GetCompanyList
        public DataTable GetCompanyList()
        {
            DataTable dtCompanyList = new DataTable();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PRC_GET_COMPANY_MASTER", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding the OUT parameter for the cursor
                        cmd.Parameters.Add("OUT_COMPANIES_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        // Execute the command and fill the DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dtCompanyList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error message
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return dtCompanyList;
        }
        #endregion



        #region GetDbFieldList
        public DataTable GetDbFieldList()
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_DAP_DB_FIELD_LIST", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }
        #endregion



        #region GetDbFieldListByType
        public DataTable GetDbFieldListByType(string type)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_DAP_DB_FIELD_LIST", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter for the table type
                    cmd.Parameters.Add(new OracleParameter("tableType", type));

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }
        #endregion


        #region GetExportedData
        public DataTable GetExportedData(string dataType, int month, int year)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_DAP_EXPORT_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("p_data_type", OracleDbType.Varchar2).Value = dataType;
                    cmd.Parameters.Add("p_month", OracleDbType.Int32).Value = month;
                    cmd.Parameters.Add("p_year", OracleDbType.Int32).Value = year;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (OracleException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt;
        }
        #endregion


    }
}
