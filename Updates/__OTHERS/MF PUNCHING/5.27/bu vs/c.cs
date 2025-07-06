using System;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using WM.Models;
using System.Configuration;
using WM.Transaction;
using static WM.Masters.Mf_Punching;
using System.Web.UI.WebControls;

namespace WM.Controllers
{
    public class MfPunchingController
    {
        private OracleConnection connection;



        public MfPunchingController()
        {
            connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);
        }

        #region GetAMCList
        public DataTable GetAMCList()
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
                {
                    using (OracleCommand command = new OracleCommand("GetAMCList", conn)) // The name of the stored procedure
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Define the output parameter as a cursor
                        OracleParameter cursorParam = new OracleParameter
                        {
                            ParameterName = "p_cursor",
                            OracleDbType = OracleDbType.RefCursor,
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(cursorParam);

                        conn.Open(); // Open the database connection
                        using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                        {
                            adapter.Fill(dt); // Fill the DataTable with the result set
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, such as logging
                Console.WriteLine("Error: " + ex.Message);
            }

            return dt; // Return the filled DataTable
        }
        #endregion


        #region GetBranchList
        public DataTable GetBranchList()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(connString))
            {
                using (OracleCommand cmd = new OracleCommand("GetBranchListPRA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Output parameter for cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    con.Open();

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
        #endregion

        #region bank from bank master
        public DataTable Getbankdropdown()
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("Get_Bank_Asc", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Define the OUT parameter to receive the cursor
                        OracleParameter refCursorParam = new OracleParameter();
                        refCursorParam.OracleDbType = OracleDbType.RefCursor;
                        refCursorParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(refCursorParam);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();

                        // Retrieve the cursor
                        using (OracleDataReader reader = ((OracleRefCursor)refCursorParam.Value).GetDataReader())
                        {
                            // Load the data into a DataTable                          
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ErrorHandler.ErrorLog("State Master", ex);
                    //Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dt;
        }
        #endregion

        #region GetTransactionList
        public DataTable GetTransactionList(TransactionFilter filter)
        {
            DataTable dt = new DataTable();

            // Connection string from Web.config
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("Get_Transactions_mf_punc", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_from_date", OracleDbType.Date).Value = filter.FromDate.HasValue ? (object)filter.FromDate.Value : DBNull.Value;
                    cmd.Parameters.Add("p_to_date", OracleDbType.Date).Value = filter.ToDate.HasValue ? (object)filter.ToDate.Value : DBNull.Value;
                    cmd.Parameters.Add("p_order_by", OracleDbType.Varchar2).Value = filter.OrderBy ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_order_direction", OracleDbType.Varchar2).Value = filter.OrderDirection ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_pan_no", OracleDbType.Varchar2).Value = filter.PANNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_tr_no", OracleDbType.Varchar2).Value = filter.TRNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_unique_client_code", OracleDbType.Varchar2).Value = filter.UniqueClientCode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_cheque_no", OracleDbType.Varchar2).Value = filter.ChequeNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_app_no", OracleDbType.Varchar2).Value = filter.AppNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_ANAExistCode", OracleDbType.Varchar2).Value = filter.anaExistCode ?? (object)DBNull.Value;

                    // Output parameter for the cursor
                    cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    //catch (OracleException ex)
                    //{
                    //    // Handle exceptions as needed
                    //    Console.WriteLine("Error: " + ex.Message);
                    //}
                    //finally
                    //{
                    //    if (conn.State == ConnectionState.Open)
                    //    {
                    //        conn.Close();
                    //    }
                    //}
                }
            }

            return dt;
        }
        #endregion

        #region GetMfDataByTranCode
        public DataTable GetMfDataByTranCode(TransactionFilter filter)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PSM_GET_Tran_CODE", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.Add("p_tran_code", OracleDbType.Varchar2).Value = filter.TRNo ?? (object)DBNull.Value;

                    // Add output parameter for the cursor
                    cmd.Parameters.Add("p_result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

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

        #region GetCityDropdown
        public DataTable GetCityDropdown()
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("Get_CitiesPRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Define the OUT parameter to receive the cursor
                        OracleParameter refCursorParam = new OracleParameter();
                        refCursorParam.OracleDbType = OracleDbType.RefCursor;
                        refCursorParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(refCursorParam);

                        // Execute the stored procedure
                        using (OracleDataReader reader = cmd.ExecuteReader()) // Directly executing reader
                        {
                            // Load the data into a DataTable
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error (uncomment when ErrorHandler is available)
                    // ErrorHandler.ErrorLog("Get Cities", ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dt;
        }
        #endregion



        #region GetStatesByCity
        public DataTable GetStatesByCity(string cityName)
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("GetStatesByCityPRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add input parameter for city name
                        OracleParameter cityParam = new OracleParameter("p_city_name", OracleDbType.Varchar2);
                        cityParam.Value = cityName;
                        cmd.Parameters.Add(cityParam);

                        // Define the OUT parameter to receive the cursor
                        OracleParameter refCursorParam = new OracleParameter();
                        refCursorParam.OracleDbType = OracleDbType.RefCursor;
                        refCursorParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(refCursorParam);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();

                        // Retrieve the cursor
                        using (OracleDataReader reader = ((OracleRefCursor)refCursorParam.Value).GetDataReader())
                        {
                            // Load the data into a DataTable                          
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error (uncomment when ErrorHandler is available)
                    //ErrorHandler.ErrorLog("Get States By City", ex);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dt;
        }
        #endregion

        public DataTable GetStateList()
        {
            DataTable dt = new DataTable();
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                string query1 = $"select * from state_master where (del_flag is null or del_flag <> 'D') order by state_name";

                using (OracleCommand command = new OracleCommand(query1, connection))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        #region CancelTransaction
        public string CancelTransaction(string transactionCode, DateTime cancelDate, string reason, string reasonType)
        {
            string resultMessage = string.Empty;

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PRA_CAN_TRAN", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("TRAN_CODE", OracleDbType.Varchar2).Value = transactionCode;

                        cmd.Parameters.Add("CANCEL_DATE", OracleDbType.Date).Value = cancelDate;
                        cmd.Parameters.Add("REASON", OracleDbType.Varchar2).Value = reason;
                        cmd.Parameters.Add("REMARK", OracleDbType.Varchar2).Value = reasonType;

                        cmd.ExecuteNonQuery();

                        resultMessage = "Transaction Cancelled Successfully";
                    }
                }
                catch (Exception ex)
                {
                    resultMessage = "Error: " + ex.Message;
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return resultMessage;
        }
        #endregion

        #region GetDistinctReasons
        public DataTable GetDistinctReasons()
        {
            DataTable dt = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PRC_GET_DISTINCT_REASONSPRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Define the OUT parameter for the cursor
                        OracleParameter outCursor = new OracleParameter();
                        outCursor.OracleDbType = OracleDbType.RefCursor;
                        outCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outCursor);

                        // Execute the command and fill the DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or handle exception
                    // ErrorHandler.ErrorLog("MfPunchingController", ex);
                    throw new Exception("Error fetching reasons: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dt;
        }
        #endregion

        #region GetTransactionLog
        public DataTable GetTransactionLog(string tranCode)
        {
            DataTable dt = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

            using (OracleConnection con = new OracleConnection(connString))
            {
                try
                {
                    con.Open();

                    using (OracleCommand cmd = new OracleCommand("PRC_GET_TRANSACTION_LOGPRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Define the input parameter
                        cmd.Parameters.Add("TR_CODE", OracleDbType.Varchar2).Value = tranCode;

                        // Define the OUT parameter for the cursor
                        OracleParameter outCursor = new OracleParameter();
                        outCursor.OracleDbType = OracleDbType.RefCursor;
                        outCursor.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outCursor);

                        // Execute the command and fill the DataTable
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or handle exception
                    // ErrorHandler.ErrorLog("TransactionLogController", ex);
                    throw new Exception("Error fetching transaction log: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            return dt;
        }
        #endregion

        #region GetTransactionDetailsByDT
        public DataTable GetTransactionDetailsByDT(string dtNumber)
        {
            DataTable dt = new DataTable();

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("GETINVESTORDETAILSBYDOCID", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("p_dt_number", OracleDbType.Varchar2).Value = dtNumber;

                    // Output parameter for cursor
                    cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    con.Open();

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
        #endregion

        #region GetSchemeDetails
        public DataTable GetSchemeDetails(string schemeCode)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand("PRA_SP_SEARCH_SCHEME", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameter
                    cmd.Parameters.Add("P_SEARCH_TERM", OracleDbType.Varchar2).Value = schemeCode;

                    // Output parameter (cursor)
                    cmd.Parameters.Add("P_RESULT_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }
            }

            return dt;
        }
        #endregion

        #region UpdateInvestorDetails
        public void UpdateInvestorDetails(
          decimal invCode, string aadhar, string pan, decimal? mobile,
          string email, string address1, string address2, string pincode,
          string cityId, string stateId, DateTime? dob, string loggedInUser
        )
        {
            using (OracleConnection conn = new OracleConnection(
                WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                //UPDATE_INVESTOR_DETAILS_PRA
                using (OracleCommand cmd = new OracleCommand("UPDATE_DETAILS_v1", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters with proper data types and null handling
                    cmd.Parameters.Add("p_inv_code", OracleDbType.Decimal).Value = invCode;
                    cmd.Parameters.Add("p_aadhar", OracleDbType.Varchar2).Value = aadhar ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_pan", OracleDbType.Varchar2).Value = pan ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_mobile", OracleDbType.Decimal).Value = mobile ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = email ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_address1", OracleDbType.Varchar2).Value = address1 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_address2", OracleDbType.Varchar2).Value = address2 ?? (object)DBNull.Value;
                    cmd.Parameters.Add("pr_pincode", OracleDbType.Varchar2).Value = pincode ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_city_id", OracleDbType.Varchar2).Value = cityId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_dob", OracleDbType.Date).Value = dob ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_state_id", OracleDbType.Varchar2).Value = stateId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("p_loggeruserid", OracleDbType.Varchar2).Value = loggedInUser;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        #endregion


        #region AddTransaction
        public (bool, string, string) AddTransaction(
     decimal sipAmount, string clientCode, string businessRmCode,
     string clientOwner, string busiBranchCode, string panno,
    string mutCode, string schCode, DateTime trDate, string tranType, string appNo,
    DateTime sipStartDate, string pan, string folioNo, string switchFolio,
    string switchScheme, string paymentMode, string bankName, string chequeNo,
    DateTime? chequeDate, decimal amount, string sipType,
    string sourceCode, string investorName, decimal expRate, decimal expAmount,
    string acHolderCode, string frequency, int installmentsNo, DateTime timestamp,
    DateTime? sipEndDate, string sipFr, string dispatch, string docId,
    string microInvestment, string targetSwitchScheme, string cobFlag,
    string swpFlag, string freedomSipFlag, string loggedInUser, string microflag)
        {
            bool isInserted = false;
            OracleConnection con = null;
            string errorMessage = string.Empty;
            string tranCode = string.Empty;

            try
            {
                // Initialize the connection object
                con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString);

                // Open the connection if it is not already open
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Create the OracleCommand for executing the stored procedure
                using (OracleCommand cmd = new OracleCommand("AddTransactionPUNCHINGPRA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters for the stored procedure
                    cmd.Parameters.Add("p_sip_amount", OracleDbType.Decimal).Value = sipAmount;
                    cmd.Parameters.Add("p_client_code", OracleDbType.Varchar2).Value = clientCode;
                    cmd.Parameters.Add("p_business_rmcode", OracleDbType.Varchar2).Value = businessRmCode;
                    cmd.Parameters.Add("p_client_owner", OracleDbType.Varchar2).Value = clientOwner;
                    cmd.Parameters.Add("p_busi_branch_code", OracleDbType.Varchar2).Value = busiBranchCode;
                    cmd.Parameters.Add("p_panno", OracleDbType.Varchar2).Value = panno;
                    cmd.Parameters.Add("p_mut_code", OracleDbType.Varchar2).Value = mutCode;
                    cmd.Parameters.Add("p_sch_code", OracleDbType.Varchar2).Value = schCode;
                    cmd.Parameters.Add("p_tr_date", OracleDbType.Date).Value = trDate;
                    cmd.Parameters.Add("p_tran_type", OracleDbType.Varchar2).Value = tranType;
                    cmd.Parameters.Add("p_app_no", OracleDbType.Varchar2).Value = appNo;
                    cmd.Parameters.Add("p_sip_start_date", OracleDbType.Date).Value = sipStartDate;
                    cmd.Parameters.Add("p_pan", OracleDbType.Varchar2).Value = pan;
                    cmd.Parameters.Add("p_folio_no", OracleDbType.Varchar2).Value = folioNo;
                    cmd.Parameters.Add("p_switch_folio", OracleDbType.Varchar2).Value = switchFolio;
                    cmd.Parameters.Add("p_switch_scheme", OracleDbType.Varchar2).Value = switchScheme;
                    cmd.Parameters.Add("p_payment_mode", OracleDbType.Char).Value = paymentMode;
                    cmd.Parameters.Add("p_bank_name", OracleDbType.Varchar2).Value = bankName;
                    cmd.Parameters.Add("p_cheque_no", OracleDbType.Varchar2).Value = chequeNo;
                    cmd.Parameters.Add("p_cheque_date", OracleDbType.Date).Value = chequeDate;
                    cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;
                    cmd.Parameters.Add("p_sip_type", OracleDbType.Varchar2).Value = sipType;
                    cmd.Parameters.Add("p_investor_name", OracleDbType.Varchar2).Value = investorName;
                    cmd.Parameters.Add("p_exp_rate", OracleDbType.Varchar2).Value = expRate;
                    cmd.Parameters.Add("p_exp_amount", OracleDbType.Varchar2).Value = expAmount;
                    cmd.Parameters.Add("p_ac_holder_code", OracleDbType.Varchar2).Value = acHolderCode;
                    cmd.Parameters.Add("p_frequency", OracleDbType.Varchar2).Value = frequency;
                    cmd.Parameters.Add("p_installments_no", OracleDbType.Int64).Value = installmentsNo;
                    cmd.Parameters.Add("p_timestamp", OracleDbType.Date).Value = timestamp;
                    cmd.Parameters.Add("p_sip_end_date", OracleDbType.Date).Value = sipEndDate;
                    cmd.Parameters.Add("p_sip_fr", OracleDbType.Char).Value = sipFr;
                    cmd.Parameters.Add("p_dispatch", OracleDbType.Char).Value = dispatch;
                    cmd.Parameters.Add("p_doc_id", OracleDbType.Varchar2).Value = docId;
                    cmd.Parameters.Add("p_micro_investment", OracleDbType.Varchar2).Value = microInvestment;
                    cmd.Parameters.Add("p_target_switch_scheme", OracleDbType.Varchar2).Value = targetSwitchScheme;
                    cmd.Parameters.Add("p_cob_flag", OracleDbType.Char).Value = cobFlag;
                    cmd.Parameters.Add("p_swp_flag", OracleDbType.Char).Value = swpFlag;
                    cmd.Parameters.Add("p_freedom_sip_flag", OracleDbType.Char).Value = freedomSipFlag;
                    cmd.Parameters.Add("p_loggeruserid", OracleDbType.Varchar2).Value = loggedInUser;
                    cmd.Parameters.Add("p_source", OracleDbType.Decimal).Value = sourceCode;
                    cmd.Parameters.Add("p_microflag", OracleDbType.Varchar2).Value = microflag;

                    OracleParameter outputParam = new OracleParameter("p_tran_code", OracleDbType.Varchar2, 50);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);

                    // Execute the command
                    cmd.ExecuteNonQuery();

                    tranCode = outputParam.Value.ToString();
                    isInserted = true; // Set success flag to true if executed without exception
                }
            }

            catch (OracleException ex)
            {
                errorMessage = "Database Error: " + ex.Message;  // Capture Oracle exception
                isInserted = false; // Set success flag to false on exception
            }
            catch (Exception ex)
            {
                errorMessage = "Error: " + ex.Message;  // Capture general exception
                isInserted = false; // Set success flag to false on exception
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure connection is closed
                }
            }

            return (isInserted, errorMessage, tranCode);   // Return the success flag
        }

        #endregion


        //public void LoadDuplicateTransactions(string clientCode, string schemeCode, decimal premAmount, Panel duplicatePopupPanel, GridView gvDuplicateTransactions)
        //{
        //    string connStr = WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString;

        //    using (OracleConnection con = new OracleConnection(connStr))
        //    {
        //        con.Open();

        //        string query = @"
        //    SELECT NVL(inv_code, 0) AS ClientCode, 
        //           NVL(i.investor_name, 0) AS ClientName,
        //           NVL(mobile, 0) AS Mobile, 
        //           NVL(tr_date, TO_DATE('01-01-1900', 'dd-MM-yyyy')) AS ARDate, 
        //           NVL(b.sch_name, 0) AS SchemeName, 
        //           NVL(b.sch_code, 0) AS SchemeCode, 
        //           NVL(amount, 0) AS Amount, 
        //           NVL(tran_code, 0) AS ARNumber, 
        //           NVL(doc_id, 0) AS DTNumber, 
        //           NVL(cheque_no, 0) AS ChequeNumber 
        //    FROM transaction_mf_temp1 a
        //    JOIN scheme_info b ON a.sch_code = b.sch_code
        //    JOIN investor_master i ON i.inv_code = a.client_code
        //    WHERE A.DOC_ID IS NOT NULL  
        //        AND CLIENT_CODE = :clientCode  
        //        AND tr_date > SYSDATE - 90  
        //        AND b.sch_code = :schemeCode
        //        AND amount BETWEEN :amountMin AND :amountMax";

        //        using (OracleCommand cmd = new OracleCommand(query, con))
        //        {
        //            cmd.Parameters.Add("clientCode", OracleDbType.Varchar2).Value = clientCode;
        //            cmd.Parameters.Add("schemeCode", OracleDbType.Varchar2).Value = schemeCode;
        //            cmd.Parameters.Add("amountMin", OracleDbType.Decimal).Value = premAmount - 100;
        //            cmd.Parameters.Add("amountMax", OracleDbType.Decimal).Value = premAmount + 100;

        //            OracleDataAdapter da = new OracleDataAdapter(cmd);
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);

        //            if (dt.Rows.Count > 0)
        //            {
        //                gvDuplicateTransactions.DataSource = dt;
        //                gvDuplicateTransactions.DataBind();
        //                duplicatePopupPanel.Visible = true; // Show the popup if data exists
        //            }
        //            else
        //            {
        //                duplicatePopupPanel.Visible = true ;
        //            }
        //        }
        //    }
        //}


        #region ModifyTransaction
        public (bool, string) ModifyTransaction(
      string tranCode,
      string closeSchCode,
      decimal sipAmount,
      string clientCode,
      string businessRmCode,
      string clientOwner,
      string mutCode,
      string busiBranchCode,
      string panno,
      string schCode,
      DateTime trDate,
      string tranType,
      string appNo,
      DateTime sipStartDate,
      string pan,
      string folioNo,
      string switchFolio,
      string switchScheme,
      string paymentMode,
      string bankName,
      string chequeNo,
      DateTime? chequeDate,
      decimal amount,
      string sipType,
      string sourceCode,
      string investorName,
      decimal expRate,
      decimal expAmount,
      string acHolderCode,
      string frequency,
      decimal installmentsNo,
      DateTime timestamp,
      DateTime? sipEndDate,
      string sipFr,
      string dispatch,
      string docId,
      string microInvestment,
      string targetSwitchScheme,
      string cobFlag,
      string swpFlag,
      string freedomSipFlag,
      string baseTranCode,
      DateTime? dropDate,
       string loggedInUser,
       DateTime? tran_date
      )
        {
            bool isInserted = false;
            string errorMessage = string.Empty;

            using (OracleConnection con = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {
                try
                {

                    con.Open();

                    // Check for duplicates
                    using (OracleCommand cmdCheck = new OracleCommand("ChkDupTranPunPRA", con))
                    {
                        cmdCheck.CommandType = CommandType.StoredProcedure;

                        cmdCheck.Parameters.Add("p_Mut_Code", OracleDbType.Varchar2).Value = mutCode ?? (object)DBNull.Value;
                        cmdCheck.Parameters.Add("p_App_No", OracleDbType.Varchar2).Value = appNo ?? (object)DBNull.Value;
                        cmdCheck.Parameters.Add("p_Tran_Code", OracleDbType.Varchar2).Value = tranCode ?? (object)DBNull.Value;
                        cmdCheck.Parameters.Add("p_Base_Tran_Code", OracleDbType.Varchar2).Value = baseTranCode ?? (object)DBNull.Value; // Assume you pass this variable
                        cmdCheck.Parameters.Add("p_Tran_Type", OracleDbType.Varchar2).Value = tranType ?? (object)DBNull.Value;

                        OracleParameter outParameter = new OracleParameter("o_Duplicate", OracleDbType.Varchar2, 100);
                        outParameter.Direction = ParameterDirection.Output;
                        cmdCheck.Parameters.Add(outParameter);

                        cmdCheck.ExecuteNonQuery();

                        string isDuplicate = outParameter.Value.ToString();

                        if (isDuplicate == "Yes")
                        {
                            return (false, "Sorry, this App No. in Transaction has been already punched for this Company.");
                        }
                    }

                    using (OracleCommand cmd = new OracleCommand("ModifyTransactionPUNCHINGPRA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_tran_code", OracleDbType.Varchar2).Value = tranCode;
                        cmd.Parameters.Add("p_close_sch_code", OracleDbType.Varchar2).Value = closeSchCode;
                        cmd.Parameters.Add("p_ac_holder_code", OracleDbType.Varchar2).Value = acHolderCode;
                        cmd.Parameters.Add("p_exp_rate", OracleDbType.Decimal).Value = expRate;
                        cmd.Parameters.Add("p_exp_amount", OracleDbType.Decimal).Value = expAmount;
                        cmd.Parameters.Add("p_client_code", OracleDbType.Varchar2).Value = clientCode;
                        cmd.Parameters.Add("p_investor_name", OracleDbType.Varchar2).Value = investorName;
                        cmd.Parameters.Add("p_business_rmcode", OracleDbType.Decimal).Value = businessRmCode;
                        cmd.Parameters.Add("p_client_owner", OracleDbType.Decimal).Value = clientOwner;
                        cmd.Parameters.Add("p_busi_branch_code", OracleDbType.Varchar2).Value = busiBranchCode;
                        cmd.Parameters.Add("p_panno", OracleDbType.Varchar2).Value = panno;
                        cmd.Parameters.Add("p_mut_code", OracleDbType.Varchar2).Value = mutCode;
                        cmd.Parameters.Add("p_sch_code", OracleDbType.Varchar2).Value = schCode;
                        cmd.Parameters.Add("p_tr_date", OracleDbType.Date).Value = trDate;
                        cmd.Parameters.Add("p_tran_type", OracleDbType.Varchar2).Value = tranType;
                        cmd.Parameters.Add("p_app_no", OracleDbType.Varchar2).Value = appNo;
                        cmd.Parameters.Add("p_folio_no", OracleDbType.Varchar2).Value = folioNo;
                        cmd.Parameters.Add("p_sip_start_date", OracleDbType.Date).Value = sipStartDate;
                        cmd.Parameters.Add("p_sip_end_date", OracleDbType.Date).Value = sipEndDate;
                        cmd.Parameters.Add("p_switch_scheme", OracleDbType.Varchar2).Value = switchScheme;
                        cmd.Parameters.Add("p_switch_folio", OracleDbType.Varchar2).Value = switchFolio;
                        cmd.Parameters.Add("p_payment_mode", OracleDbType.Char).Value = paymentMode;
                        cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;
                        cmd.Parameters.Add("p_frequency", OracleDbType.Varchar2).Value = frequency;
                        cmd.Parameters.Add("p_installments_no", OracleDbType.Decimal).Value = installmentsNo;
                        cmd.Parameters.Add("p_sip_type", OracleDbType.Varchar2).Value = sipType;
                        cmd.Parameters.Add("p_sip_fr", OracleDbType.Char).Value = sipFr;
                        cmd.Parameters.Add("p_dispatch", OracleDbType.Char).Value = dispatch;
                        cmd.Parameters.Add("p_source_code", OracleDbType.Varchar2).Value = sourceCode;
                        cmd.Parameters.Add("p_doc_id", OracleDbType.Varchar2).Value = docId;
                        cmd.Parameters.Add("p_micro_investment", OracleDbType.Varchar2).Value = microInvestment;
                        cmd.Parameters.Add("p_cob_flag", OracleDbType.Char).Value = cobFlag;
                        cmd.Parameters.Add("p_freedom_sip_flag", OracleDbType.Char).Value = freedomSipFlag;
                        cmd.Parameters.Add("p_swp_flag", OracleDbType.Char).Value = swpFlag;
                        cmd.Parameters.Add("p_drop_date", OracleDbType.Date).Value = dropDate;
                        cmd.Parameters.Add("p_loggeruserid", OracleDbType.Varchar2).Value = loggedInUser;
                        cmd.Parameters.Add("p_bank_name", OracleDbType.Varchar2).Value = bankName;
                        cmd.Parameters.Add("p_tran_date", OracleDbType.Date).Value = tran_date;
                        cmd.Parameters.Add("p_cheque_no", OracleDbType.Varchar2).Value = chequeNo;
                        cmd.Parameters.Add("p_cheque_date", OracleDbType.Date).Value = chequeDate;

                        // Execute the command
                        cmd.ExecuteNonQuery();
                        isInserted = true; // Set success flag to true if executed without exception
                    }
                }
                catch (OracleException ex)
                {
                    errorMessage = "Database Error: " + ex.Message; // Capture Oracle exception
                                                                    // Log the exception here if you have a logging mechanism
                }
                catch (Exception ex)
                {
                    errorMessage = "Error: " + ex.Message; // Capture general exception
                                                           // Log the exception here if you have a logging mechanism
                }

                return (isInserted, errorMessage); // Retrn the success flag
            }
        }

        #endregion


        public DataTable MFClientSearch(MFQuery query)
        {
            using (var connection = new OracleConnection(WebConfigurationManager.ConnectionStrings["ConnectionStringVM"].ConnectionString))
            {



                using (OracleCommand command = new OracleCommand("PRA_MF_search_investor_data", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_Category", OracleDbType.NVarchar2).Value = query.Category;
                    command.Parameters.Add("p_BranchCode", OracleDbType.NVarchar2).Value = query.BranchCode;
                    command.Parameters.Add("p_CityCode", OracleDbType.NVarchar2).Value = query.CityCode;
                    command.Parameters.Add("p_RmCode", OracleDbType.NVarchar2).Value = query.RmCode;
                    command.Parameters.Add("p_InvName", OracleDbType.NVarchar2).Value = query.ClientName;
                    command.Parameters.Add("p_InvCode", OracleDbType.NVarchar2).Value = query.InvCode;
                    command.Parameters.Add("p_Phone", OracleDbType.NVarchar2).Value = query.Phone;
                    command.Parameters.Add("p_Mobile", OracleDbType.NVarchar2).Value = query.Mobile;
                    command.Parameters.Add("p_PanNo", OracleDbType.NVarchar2).Value = query.PanNo;
                    command.Parameters.Add("p_Address1", OracleDbType.NVarchar2).Value = query.Address1;
                    command.Parameters.Add("p_Address2", OracleDbType.NVarchar2).Value = query.Address2;
                    command.Parameters.Add("p_SortBy", OracleDbType.NVarchar2).Value = query.SortBy;
                    command.Parameters.Add("p_Email", OracleDbType.NVarchar2).Value = query.Email;
                    command.Parameters.Add("p_DOB", OracleDbType.NVarchar2).Value = query.DOB;
                    command.Parameters.Add("p_AccountCode", OracleDbType.NVarchar2).Value = query.AccountCode;

                    command.Parameters.Add("o_Result", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

    }

}

