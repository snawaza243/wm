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
using WM.Masters;
using DocumentFormat.OpenXml.Wordprocessing;



namespace WM.Controllers
{
    public class NpsTransactionPunchingController
    {
        OracleConnection connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);

        #region CALCULATE INVESTMENT DETAILS

        #region CALCULATE INVESTMENT DETAILS

        public Dictionary<string, decimal> CALCULATE_INVESTMENT_DETAILS(
            int reqCode,
            string scheme,
            decimal amountTire1,
            decimal amountTire2,
            decimal collectionAmount,
            DateTime currentDate,
            int type = 0,
            long arNo = 0)
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            result.Add("pop_reg_charge_ot", 0);
            result.Add("pop_reg_charge", 0);
            result.Add("gst", 0);
            result.Add("invested", 0);
            result.Add("status", 0); // 0=Success, 1=Error
            result.Add("error_msg", 0); // Will be converted to string later

            // Define connection string
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_NPS_CALC2", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true; // Important for Oracle parameter binding

                    // Input parameters
                    cmd.Parameters.Add("p_req_code", OracleDbType.Int32).Value = reqCode;
                    cmd.Parameters.Add("p_scheme", OracleDbType.Varchar2).Value = scheme;
                    cmd.Parameters.Add("p_amount_t1", OracleDbType.Decimal).Value = amountTire1;
                    cmd.Parameters.Add("p_amount_t2", OracleDbType.Decimal).Value = amountTire2;
                    cmd.Parameters.Add("p_collection", OracleDbType.Decimal).Value = collectionAmount;
                    cmd.Parameters.Add("p_date", OracleDbType.Date).Value = currentDate;
                    cmd.Parameters.Add("p_type", OracleDbType.Int32).Value = type;
                    cmd.Parameters.Add("p_ar_no", OracleDbType.Int64).Value = arNo;

                    // Output parameters
                    cmd.Parameters.Add("p_pop_reg_charge_ot", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_pop_reg_charge", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_gst", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_invested", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_status", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_error_msg", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.Output;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                         /*
                        // Safely extract output parameters directly as strings, handling nulls
                        string popRegChargeOt = cmd.Parameters["p_pop_reg_charge_ot"].Value != DBNull.Value
                            ? cmd.Parameters["p_pop_reg_charge_ot"].Value.ToString() : "";

                        string popRegCharge = cmd.Parameters["p_pop_reg_charge"].Value != DBNull.Value
                            ? cmd.Parameters["p_pop_reg_charge"].Value.ToString() : "";

                        string gst = cmd.Parameters["p_gst"].Value != DBNull.Value
                            ? cmd.Parameters["p_gst"].Value.ToString() : "";

                        string invested = cmd.Parameters["p_invested"].Value != DBNull.Value
                            ? cmd.Parameters["p_invested"].Value.ToString() : "";

                        string status = cmd.Parameters["p_status"].Value != DBNull.Value
                            ? cmd.Parameters["p_status"].Value.ToString() : "1"; // default status

                        string errorMsg = cmd.Parameters["p_error_msg"].Value != DBNull.Value
                            ? cmd.Parameters["p_error_msg"].Value.ToString() : "Unknown error";
                         
                        */
                        /*

                        */

                        // Safely extract output values with null checks
                        decimal popRegChargeOt = cmd.Parameters["p_pop_reg_charge_ot"].Value != DBNull.Value
                            ? Convert.ToDecimal(cmd.Parameters["p_pop_reg_charge_ot"].Value.ToString()) : 0;

                        decimal popRegCharge = cmd.Parameters["p_pop_reg_charge"].Value != DBNull.Value
                            ? Convert.ToDecimal(cmd.Parameters["p_pop_reg_charge"].Value.ToString()) : 0;

                        decimal gst = cmd.Parameters["p_gst"].Value != DBNull.Value
                            ? Convert.ToDecimal(cmd.Parameters["p_gst"].Value.ToString()) : 0;

                        decimal invested = cmd.Parameters["p_invested"].Value != DBNull.Value
                            ? Convert.ToDecimal(cmd.Parameters["p_invested"].Value.ToString()) : 0;

                        int status = cmd.Parameters["p_status"].Value != DBNull.Value
                            ? Convert.ToInt32(cmd.Parameters["p_status"].Value.ToString()) : 1; // Default to error if null

                        string errorMsg = cmd.Parameters["p_error_msg"].Value != DBNull.Value
                            ? Convert.ToString(cmd.Parameters["p_error_msg"].Value) : "Unknown error";

                        // Update result dictionary
                        result["pop_reg_charge_ot"] = popRegChargeOt;
                        result["pop_reg_charge"] = popRegCharge;
                        result["gst"] = gst;
                        result["invested"] = invested;
                        result["status"] = status;
                        result["error_msg"] = errorMsg != null ? Convert.ToDecimal(errorMsg.GetHashCode()) : 0; // Store hash if needed

                        // If there was an error, throw exception with Oracle error message
                        if (status == 1)
                        {
                            throw new Exception($"Oracle procedure error: {errorMsg}");
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Log Oracle-specific errors
                        result["status"] = 1;
                        result["error_msg"] = ex.Message.GetHashCode();
                        throw new Exception("Database error occurred: " + ex.Message, ex);
                    }
                    catch (Exception ex)
                    {
                        // Log general errors
                        result["status"] = 1;
                        result["error_msg"] = ex.Message.GetHashCode();
                        throw new Exception("Error in NPS calculation: " + ex.Message, ex);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }

            return result;
        }

        #endregion


        public Dictionary<string, decimal> CALCULATE_INVESTMENT_DETAILS1(
            int reqCode,
            string scheme,
            decimal amountTire1,
            decimal amountTire2,
            decimal collectionAmount,
            DateTime currentDate
        )
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            // Define connection string
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_NPS_CALC", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("p_req_code", OracleDbType.Int32).Value = reqCode;
                    cmd.Parameters.Add("p_scheme", OracleDbType.Varchar2).Value = scheme;
                    cmd.Parameters.Add("p_amount_t1", OracleDbType.Decimal).Value = amountTire1;
                    cmd.Parameters.Add("p_amount_t2", OracleDbType.Decimal).Value = amountTire2;
                    cmd.Parameters.Add("p_collection", OracleDbType.Decimal).Value = collectionAmount;
                    cmd.Parameters.Add("p_date", OracleDbType.Date).Value = currentDate;

                    // Output parameters
                    cmd.Parameters.Add("p_pop_reg_charge_ot", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_pop_reg_charge", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_gst", OracleDbType.Decimal).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_invested", OracleDbType.Decimal).Direction = ParameterDirection.Output;

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        // Extract output values safely
                        string str_pop_reg_charge_ot = Convert.ToString(cmd.Parameters["p_pop_reg_charge_ot"].Value);
                        string str_pop_reg_charge = Convert.ToString(cmd.Parameters["p_pop_reg_charge"].Value);
                        string str_gst = Convert.ToString(cmd.Parameters["p_gst"].Value);
                        string str_invested = Convert.ToString(cmd.Parameters["p_invested"].Value);

                        // Check and convert safely
                        result["pop_reg_charge_ot"] = !string.IsNullOrEmpty(str_pop_reg_charge_ot) ? Convert.ToDecimal(str_pop_reg_charge_ot) : 0;
                        result["pop_reg_charge"] = !string.IsNullOrEmpty(str_pop_reg_charge) ? Convert.ToDecimal(str_pop_reg_charge) : 0;
                        result["gst"] = !string.IsNullOrEmpty(str_gst) ? Convert.ToDecimal(str_gst) : 0;
                        result["invested"] = !string.IsNullOrEmpty(str_invested) ? Convert.ToDecimal(str_invested) : 0;
                    }
                    catch (OracleException ex)
                    {
                        // Handle exception or log as needed
                        throw new Exception("Oracle error occurred: " + ex.Message, ex);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }

            return result;
        }

        #endregion












        #region PSM_LOGIN_DATA
        public DataTable GET_PSM_LOGIN_DATA(string loginID)
        {
            DataTable dt = new DataTable();

            // Define the connection string
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_LOGIN_DATA", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("p_loginId", OracleDbType.Varchar2).Value = !string.IsNullOrEmpty(loginID) ? loginID : (Object)DBNull.Value;
                  
                    // Add output parameter for the result cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open(); // Open the database connection

                        // Use OracleDataAdapter to fill the DataTable with the result
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Populate the DataTable
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Log the exception details (use a logging framework in production)
                        Console.WriteLine("Oracle Error: " + ex.Message);

                        // Optionally rethrow the exception if it needs to propagate
                        throw;
                    }
                    finally
                    {
                        // Ensure the connection is closed
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt; // Return the filled DataTable
        }
        #endregion

        #region INSERT UPDATE CONTROLLER
        public string InsertClientData(
        #region Input parameter
            string mark,
            string prodValue,
            string investorTypeValue,
            string corporateNameValue,
            string dtNumberValue,
            string tranCodeValue,
            long invertorCodeValue,
            string schemeCodeValue,
            string craValue,
            int craBranchValue,
            string txtPopSpRegNoFolioValue,
            long businessRmValue,
            long businessBranchValue,
            string receiptNoUniueValue,
            char paymentModeValue,
            string chequeNoValue,
            string bankNameValue,
            string appNoValue,
            DateTime? chequeDateValue,
            DateTime? dateValue,
            DateTime? timeValue,
            DateTime? combinedDateTime,
            string subInvNameValue,
            string praManualARNoValue,
            string unfreezValue,
            decimal amountT1Value,
            decimal amountT2Value,
            decimal regharge1Value,
            decimal regharge2Value,
            decimal gstTaxValue,
            decimal amountCollectionValue,
            decimal amountInvestedValue,
            decimal amountInvested2Value,
            string remarkValue,
            string zeroComValue,
            string loggedinUser
        #endregion

            )



        {
            string result;
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_NPS_INSERT_UPDATE_PRA", conn)) // Make sure the name is correct
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("P_MARK", OracleDbType.Varchar2).Value = mark ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PRODUCT_CLASS", OracleDbType.Varchar2).Value = prodValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_INVESTOR_TYPE", OracleDbType.Varchar2).Value = investorTypeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CORPORATE_NAME", OracleDbType.Varchar2).Value = corporateNameValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DT_NUMBER", OracleDbType.Varchar2).Value = dtNumberValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_TRAN_CODE", OracleDbType.Varchar2).Value = tranCodeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_INVESTOR_CODE", OracleDbType.Int64).Value = invertorCodeValue;
                    cmd.Parameters.Add("P_SCHEME_CODE", OracleDbType.Varchar2).Value = schemeCodeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CRA", OracleDbType.Varchar2).Value = craValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CRA_BRANCH", OracleDbType.Int32).Value = craBranchValue;
                    cmd.Parameters.Add("P_FOLIO_NUMBER", OracleDbType.Varchar2).Value = txtPopSpRegNoFolioValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BUSINESS_RM", OracleDbType.Int64).Value = businessRmValue;
                    cmd.Parameters.Add("P_BUSINESS_BRANCH", OracleDbType.Int64).Value = businessBranchValue;
                    cmd.Parameters.Add("P_RECEIPT_NO", OracleDbType.Varchar2).Value = receiptNoUniueValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_PAYMENT_MODE", OracleDbType.Char).Value = paymentModeValue == '\0' ? (object)DBNull.Value : paymentModeValue;
                    cmd.Parameters.Add("P_CHEQUE_NO", OracleDbType.Varchar2).Value = chequeNoValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_BANK_NAME", OracleDbType.Varchar2).Value = bankNameValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_APP_NO", OracleDbType.Varchar2).Value = appNoValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_CHEQUE_DATE", OracleDbType.Date).Value = chequeDateValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_DATE", OracleDbType.Date).Value = dateValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_TIME", OracleDbType.Date).Value = timeValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_COMBINED_DATETIME", OracleDbType.Date).Value = combinedDateTime ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SUBSCRIBER_NAME", OracleDbType.Varchar2).Value = subInvNameValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_MANUAL_AR_NO", OracleDbType.Varchar2).Value = praManualARNoValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_UNFREEZE", OracleDbType.Varchar2).Value = unfreezValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_AMOUNT_T1", OracleDbType.Decimal).Value = amountT1Value;
                    cmd.Parameters.Add("P_AMOUNT_T2", OracleDbType.Decimal).Value = amountT2Value;
                    cmd.Parameters.Add("P_RECHARGE1", OracleDbType.Decimal).Value = regharge1Value;
                    cmd.Parameters.Add("P_RECHARGE2", OracleDbType.Decimal).Value = regharge2Value;
                    cmd.Parameters.Add("P_GST_TAX", OracleDbType.Decimal).Value = gstTaxValue;
                    cmd.Parameters.Add("P_COLLECTION_AMOUNT", OracleDbType.Decimal).Value = amountCollectionValue;
                    cmd.Parameters.Add("P_AMOUNT_INVESTED", OracleDbType.Decimal).Value = amountInvestedValue;
                    cmd.Parameters.Add("P_AMOUNT_INVESTED2", OracleDbType.Decimal).Value = amountInvested2Value;
                    cmd.Parameters.Add("P_REMARK", OracleDbType.Varchar2).Value = remarkValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ZERO_COM", OracleDbType.Varchar2).Value = zeroComValue ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_LOGGEDIN_USER", OracleDbType.Varchar2).Value = loggedinUser ?? (object)DBNull.Value;


                    // Define the output parameter
                    cmd.Parameters.Add("P_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Execute the command

                        // Retrieve the generated CLIENT_CODE from the RefCursor
                        using (OracleDataReader reader = ((OracleRefCursor)cmd.Parameters["P_RESULT"].Value).GetDataReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming the first column is the status
                                string status = reader.GetString(0);

                                if (status != null)
                                {
                                    result = status;
                                }
                                else
                                {
                                    result = "No result returned.";

                                }
                            }
                            else
                            {
                                result = "No result returned.";
                            }
                        }
                    }
                    catch (OracleException ex)
                    {
                        result = "Error inserting client data: " + ex.Message;
                    }
                    catch (Exception ex) // General exception handling
                    {
                        result = "An unexpected error occurred: " + ex.Message;
                    }
                }
                return result;
            }
        }


        #endregion

        #region PSM_NPS_GET_AR_BY_DTTS
        public DataTable GET_AR_BY_DTTS(string p_dtnumber, string p_arnumber, bool p_beforemark, string loginid)
        {
            DataTable dt = new DataTable();

            // Define the connection string
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_NPS_GET_AR_BY_DTTS", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameters
                    cmd.Parameters.Add("p_dtnumber", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(p_dtnumber) ? (object)DBNull.Value : p_dtnumber;
                    cmd.Parameters.Add("p_arnumber", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(p_arnumber) ? (object)DBNull.Value : p_arnumber;
                    cmd.Parameters.Add("p_beforemark", OracleDbType.Int32).Value = p_beforemark ? 1: 0; // Map BOOLEAN to integer (1 for TRUE, 0 for FALSE)
                    cmd.Parameters.Add("p_login_id", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(loginid) ? (object)DBNull.Value : loginid;

                    
                    // Add output parameter for the result cursor
                    cmd.Parameters.Add("result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    try
                    {
                        conn.Open(); // Open the database connection

                        // Use OracleDataAdapter to fill the DataTable with the result
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt); // Populate the DataTable
                        }
                    }
                    catch (OracleException ex)
                    {
                        // Log the exception details (use a logging framework in production)
                        Console.WriteLine("Oracle Error: " + ex.Message);

                        // Optionally rethrow the exception if it needs to propagate
                        throw;
                    }
                    finally
                    {
                        // Ensure the connection is closed
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }

            return dt; // Return the filled DataTable
        }
        
        
        
        #endregion



        #region SearchARDetails AR Model controller


        public static object GetNullableDBValue<T>(T? value) where T : struct
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        public static object GetNullableStringDBValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? (object)value : DBNull.Value;
        }

        public DataTable SearchARDetails(
            string arNo,
            string appNo,
            string chequeNo,
            string pranNo,
            string scheme,
            string invName,
            string anaExistCode,
            string arFromDate,
            string arToDate,
            string arBefore
        )
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_NPS_GET_ARTR_LIST", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        cmd.Parameters.Add("p_ar_no", OracleDbType.Varchar2).Value = GetNullableStringDBValue(arNo);
                        cmd.Parameters.Add("p_app_no", OracleDbType.Varchar2).Value = GetNullableStringDBValue(appNo);
                        cmd.Parameters.Add("p_cheque_no", OracleDbType.Varchar2).Value = GetNullableStringDBValue(chequeNo);
                        cmd.Parameters.Add("p_pran_no", OracleDbType.Varchar2).Value = GetNullableStringDBValue(pranNo);
                        cmd.Parameters.Add("p_scheme", OracleDbType.Varchar2).Value = GetNullableStringDBValue(scheme);
                        cmd.Parameters.Add("p_investor_name", OracleDbType.Varchar2).Value = GetNullableStringDBValue(invName);
                        cmd.Parameters.Add("p_ana_exist_code", OracleDbType.Varchar2).Value = GetNullableStringDBValue(anaExistCode);
                        cmd.Parameters.Add("p_ar_from_date", OracleDbType.Varchar2).Value = GetNullableStringDBValue(arFromDate);
                        cmd.Parameters.Add("p_ar_to_date", OracleDbType.Varchar2).Value = GetNullableStringDBValue(arToDate);
                        cmd.Parameters.Add("p_before_st", OracleDbType.Varchar2).Value = GetNullableStringDBValue(arBefore);

                        // Output parameter for the cursor
                        cmd.Parameters.Add("p_result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        try
                        {
                            conn.Open();
                            using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                            {
                                da.Fill(dt); // Fill the DataTable with results from the cursor
                            }
                        }
                        catch (OracleException ex)
                        {
                            Console.WriteLine("Oracle Error: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return dt; // Return the populated DataTable
        }

        #endregion

        #region PrintReceiptData

        public DataTable PrintReceiptData(string receiptNo)
        {
            DataTable receiptData = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_NPS_PRINT_RECEIPT_GRID", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter
                        cmd.Parameters.Add("p_unique_id", OracleDbType.Varchar2).Value = receiptNo;

                        // Output parameter (if your procedure has an output cursor)
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        try
                        {
                            conn.Open();

                            // Create an OracleDataAdapter to fill the DataTable
                            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                            {
                                adapter.Fill(receiptData);
                            }
                        }
                        catch (OracleException ex)
                        {
                            Console.WriteLine("Oracle Error: " + ex.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return receiptData;
        }

        #endregion



        #region NOT IN USE

        #region CheckDuplicateCheque VALID BUT NOT IN USE
        public bool CheckDuplicateCheque(int index, string invCode, int lbTranCode, string chqNo, string reqCode)
        {
            bool isDuplicate = false;

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PSM_NPS_CHECK_DUPLICATE_CHEQUE", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Adding parameters for the stored procedure
                        cmd.Parameters.Add("P_INDEX", OracleDbType.Int32).Value = index;
                        cmd.Parameters.Add("P_INV_CD", OracleDbType.Varchar2).Value = invCode;
                        cmd.Parameters.Add("P_LBTRAN_CODE", OracleDbType.Int32).Value = lbTranCode;
                        cmd.Parameters.Add("P_CHQ_NO", OracleDbType.Varchar2).Value = chqNo;
                        cmd.Parameters.Add("P_REQ_CODE", OracleDbType.Varchar2).Value = reqCode;

                        // Execute the procedure
                        cmd.ExecuteNonQuery();

                        // If no exception was thrown, no duplicates were found
                        isDuplicate = false;
                    }
                }
                catch (OracleException ex)
                {
                    // If a duplicate cheque number error occurs
                    if (ex.Number == 20001) // Assuming error -20001 is thrown for duplicates
                    {
                        isDuplicate = true;
                    }
                    else
                    {
                        // Log other errors (if applicable)
                        // ErrorHandler.ErrorLog("Check Duplicate Cheque", ex);
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }

            return isDuplicate;
        }
        #endregion

        #region GetUserDetailsByLoginID VALID BUT NOT IN USE
        public DataTable GetUserDetailsByLoginID(string loginId)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_GET_USERBYLOGIN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_login_id", OracleDbType.Varchar2).Value = loginId;  // Login ID (input parameter)
                    cmd.Parameters.Add("p_result_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;  // Output cursor

                    try
                    {
                        conn.Open();
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);  // Fill the DataTable with the result from the cursor
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

            return dt;  // Return the populated DataTable
        }
        #endregion

        #region SaveLogIn

        public void SaveLogIn(string userId, string action, string source)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_NPS_SAVE_LOGIN_ACTION", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        cmd.Parameters.Add("p_user_id", OracleDbType.Varchar2).Value = userId;
                        cmd.Parameters.Add("p_action", OracleDbType.Varchar2).Value = action;
                        cmd.Parameters.Add("p_source", OracleDbType.Varchar2).Value = source;
                        cmd.Parameters.Add("p_timestamp", OracleDbType.Date).Value = DateTime.Now;

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (OracleException ex)
                        {
                            Console.WriteLine("Oracle Error: " + ex.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error Logging user action: " + ex.Message);
            }
        }

        #endregion


        #endregion

        #region NOT IN USE: EXPORT INTO EXCEL 

        public DataTable ExportTransaction(string fromDate, string toDate, string transactionType, string cra)
        {
            DataTable dt = new DataTable();

            // Parse the input date strings
            DateTime from = DateTime.Parse(fromDate);
            DateTime to = DateTime.Parse(toDate);

            // Determine payment mode and folio number based on input parameters
            string paymentMode = transactionType == "all" ? "all" : transactionType;
            string folioNo = cra == "1" ? "6036914" : cra == "2" ? "1171966" : null;

            // Use the connection string from the configuration manager
            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                OracleCommand cmd = new OracleCommand("EXPORT_TRANSACTIONS", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add input parameters
                cmd.Parameters.Add("p_from_date", OracleDbType.Date).Value = from;
                cmd.Parameters.Add("p_to_date", OracleDbType.Date).Value = to;
                cmd.Parameters.Add("p_payment_mode", OracleDbType.Varchar2).Value = paymentMode;
                cmd.Parameters.Add("p_folio_no", OracleDbType.Varchar2).Value = folioNo ?? (object)DBNull.Value;

                // Add output cursor parameter
                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                try
                {
                    conn.Open();
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dt); // Fill the DataTable with the result set
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    throw new Exception("Database error: " + ex.Message);
                }
            }

            return dt; // Return the filled DataTable
        }


        #endregion
    

     
     
        #region GetBranchData

        public DataTable NTPCraGetBranchData(int branchCode)
        {
            DataTable branchData = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand cmd = new OracleCommand("PSM_NPT_GET_BRANCH_DATA", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Input parameter
                        cmd.Parameters.Add("p_branch_code", OracleDbType.Int32).Value = branchCode;

                        // Output parameter
                        cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        try
                        {
                            conn.Open();

                            // Create an OracleDataAdapter to fill the DataTable
                            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                            {
                                adapter.Fill(branchData);
                            }
                        }
                        catch (OracleException ex)
                        {
                            Console.WriteLine("Oracle Error: " + ex.Message);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return branchData;
        }

        #endregion

       

        #region GetSchemeCodeList
        public DataTable GetSchemeCodeList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))

                {
                    using (OracleCommand command = new OracleCommand("PSM_NPS_GET_SCH_CODE", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion

        #region GetCRABranchList
        public DataTable GetCRABranchMasterList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_NPS_Get_CRABranch_Master", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Output parameter for the cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_branch_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Ensure the connection is open before executing the command
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the results
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion


        #region GetRequestTypeList
        public DataTable GetRequestTypeList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))

                {
                    using (OracleCommand command = new OracleCommand("PSM_NTP_GET_REQUEST_TYPES", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion


        #region GetBankMasterList
        public DataTable GetBankMasterList()
        {
            DataTable dt = new DataTable();
            try
            {
                // Create a connection string variable
                string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

                // Create a connection to the Oracle database
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    // Create a command to execute the stored procedure
                    using (OracleCommand command = new OracleCommand("PSAM_GET_BANK_MASTER_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Define the cursor parameter
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "P_CURSOR",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        // Open the connection
                        conn.Open();

                        // Use OracleDataAdapter to fill the DataTable
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion




        #region GetBranchList
        public DataTable GetBranchMasterList()
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("PSM_NTP_BRANCH_MASTER_LIST", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "P_BRANCH_LIST",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open();
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        #endregion


    }
}