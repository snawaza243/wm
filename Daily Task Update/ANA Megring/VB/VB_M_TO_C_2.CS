using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;

namespace MyNamespace
{
    public partial class MergeControl : System.Web.UI.UserControl
    {
        // Global members used in the merge process
        private OracleConnection MyConn;
        private string branch_cd;
        private string Rm_cd;
        private int mCount = 0;
        private DateTime ServerDateTime = DateTime.Now;

        // Constructor - initialize connection using a connection string from configuration
        public MergeControl()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MyOracleConnection"].ConnectionString;
            MyConn = new OracleConnection(connStr);
        }

        // Executes a query and returns the results as a DataTable.
        // If provided, the OracleTransaction is used.
        private DataTable ExecuteQuery(string query, OracleTransaction trans = null)
        {
            DataTable dt = new DataTable();
            OracleCommand cmd = new OracleCommand(query, MyConn);
            if (trans != null)
                cmd.Transaction = trans;
            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        // Executes a non-query SQL command. Returns the number of rows affected.
        private int ExecuteNonQuery(string query, OracleTransaction trans = null)
        {
            OracleCommand cmd = new OracleCommand(query, MyConn);
            if (trans != null)
                cmd.Transaction = trans;
            return cmd.ExecuteNonQuery();
        }

        // A dummy validation function. Returns true if validation passes.
        private bool ValidateInput()
        {
            // Dummy implementation. Replace with actual validation logic if needed.
            return true;
        }

        // Dummy implementation to set branch_cd and Rm_cd.
        private void Find_RM()
        {
            // In the original VB code, this would set the RM details.
            branch_cd = "1";
            Rm_cd = "1";
        }

        // Checks if two date strings (in dd/MM/yyyy format) are the same.
        private bool CheckDate(string date1, string date2)
        {
            return string.Equals(date1, date2, StringComparison.OrdinalIgnoreCase);
        }

        // Returns the leftmost substring of the specified length.
        private string Left(string s, int len)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            return s.Length >= len ? s.Substring(0, len) : s;
        }

        // Dummy implementation to (re)configure grid controls.
        private void SetGrid()
        {
            // Implementation for grid configuration goes here.
        }

        // MergeClients performs the merge operation.
        // Parameters:
        //    mainAgent   - (PARAM1) The main agent code (used instead of msfgMain.TextMatrix(1,2))
        //    agentsArray - (PARAM2) An array of agent codes to be merged (used instead of msfgMergedInvestors.TextMatrix(...))
        //    loginId     - (PARAM3) The login ID (Glbloginid)
        public void MergeClients(string mainAgent, string[] agentsArray, string loginId)
        {
            try
            {
                // Open the database connection if not already open.
                if (MyConn.State != ConnectionState.Open)
                    MyConn.Open();

                // Validate the input; if validation fails, exit.
                if (!ValidateInput())
                    return;

                // Confirm the merge action.
                // (In ASPX the confirmation is assumed to have been handled on the client side.)
                // Setting the MousePointer to busy and disabling frames is commented as these are UI actions.
                // Me.MousePointer = 11;
                // Frame1.Enabled = false; Frame2.Enabled = false; Frame3.Enabled = false; Frame4.Enabled = false;

                branch_cd = "";
                Rm_cd = "";
                Find_RM();

                string Fam_Head = "";
                string Members1 = "";
                string Members2 = "";
                string Members3 = "";

                // Loop over the agentsArray starting from index 1 (simulating VB's For i = 1 To Rows - 1)
                for (int i = 1; i < agentsArray.Length; i++)
                {
                    // Open recordset for main agent (rsClient)
                    string queryRsClient = "Select * from agent_master where agent_code=" + mainAgent;
                    DataTable rsClient = ExecuteQuery(queryRsClient);

                    // Open recordset for the merging agent (rsclient1)
                    string queryRsClient1 = "Select * from agent_master where agent_code=" + agentsArray[i];
                    DataTable rsclient1 = ExecuteQuery(queryRsClient1);

                    // Open recordset for investor_master for the current merging agent (RsData)
                    string queryRsData = "select inv_code,investor_name from investor_master where source_id=" + agentsArray[i];
                    DataTable RsData = ExecuteQuery(queryRsData);

                    // Begin transaction for this agent merge
                    OracleTransaction trans = MyConn.BeginTransaction();
                    bool flag = true;

                    // Process each record in RsData
                    foreach (DataRow rsDataRow in RsData.Rows)
                    {
                        // Construct and open rsInv_check recordset to check if a matching investor exists
                        string investorName = rsDataRow["investor_name"].ToString();
                        string cleanedInvestorName = investorName.Trim().ToUpper().Replace(".", "").Replace(" ", "");
                        string searchPattern = "%" + Left(cleanedInvestorName, 8) + "%";
                        string queryRsInvCheck = "Select inv_code from investor_master where source_id=" + mainAgent +
                            " and substr(replace(replace(trim(upper(investor_name)),'.',''),' ',''),1,8) like '" + searchPattern +
                            "' and instr(trim(upper(investor_name)),'HUF')=0";
                        DataTable rsInv_check = ExecuteQuery(queryRsInvCheck, trans);

                        string New_Inv_Code = "";
                        if (rsInv_check.Rows.Count > 0)
                        {
                            New_Inv_Code = rsInv_check.Rows[0]["inv_code"].ToString();
                        }
                        else
                        {
                            mCount = mCount + 1;
                            if (mCount >= 999)
                                New_Inv_Code = mainAgent + mCount.ToString("00000");
                            else
                                New_Inv_Code = mainAgent + mCount.ToString("000");
                            string updateInvMaster = "update INVESTOR_MASTER       set   SOURCE_ID=" + mainAgent +
                                ",BRANCH_CODE=" + branch_cd + ",RM_CODE=" + Rm_cd + ",INV_CODE='" + New_Inv_Code +
                                "' where INV_CODE=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(updateInvMaster, trans);
                        }

                        string updateFpInvestor1 = "update fp_investor set familyhead_code='" + New_Inv_Code +
                            "' where familyhead_code='" + rsDataRow["inv_code"] + "'";
                        ExecuteNonQuery(updateFpInvestor1, trans);

                        string updateFpInvestor2 = "update fp_investor set fam_mem1=replace(fam_mem1," + rsDataRow["inv_code"] +
                            "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) +
                            "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                        ExecuteNonQuery(updateFpInvestor2, trans);

                        string updateFpInvestor3 = "update fp_investor set fam_mem2=replace(fam_mem2," + rsDataRow["inv_code"] +
                            "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) +
                            "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                        ExecuteNonQuery(updateFpInvestor3, trans);

                        string updateFpInvestor4 = "update fp_investor set fam_mem3=replace(fam_mem3," + rsDataRow["inv_code"] +
                            "," + New_Inv_Code + ") where familyhead_code like '" + Left(rsDataRow["inv_code"].ToString(), 8) +
                            "%' or familyhead_code like '" + Left(New_Inv_Code, 8) + "%'";
                        ExecuteNonQuery(updateFpInvestor4, trans);

                        string updateTransSt = "update TRANSACTION_ST        set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransSt, trans);

                        string updateTransMFTemp1 = "update TRANSACTION_MF_TEMP1  set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransMFTemp1, trans);

                        string updateTransSt_Bajaj = "update TRANSACTION_ST@mf.bajajcapital        set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransSt_Bajaj, trans);

                        string updateTransStTemp = "update TRANSACTION_STTEMP    set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransStTemp, trans);

                        string updateRedem = "update REDEM@mf.bajajcapital                 set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateRedem, trans);

                        string updateInvFolio = "update INVESTOR_FOLIO@mf.bajajcapital        set INVESTOR_CODE=" + New_Inv_Code +
                            " where INVESTOR_CODE=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateInvFolio, trans);

                        string updateInvMasterIPO = "update INVESTOR_MASTER_IPO   set      inv_code=" + New_Inv_Code +
                            ",AGENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateInvMasterIPO, trans);

                        string updateRevertal = "update REVERTAL_TRANSACTION  set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateRevertal, trans);

                        string updateTransIPO = "update TRANSACTION_IPO       set      inv_code=" + New_Inv_Code +
                            ",AGENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransIPO, trans);

                        string updateTranPayout = "update TRAN_PAYOUT@mf.bajajcapital           set      inv_code=" + New_Inv_Code +
                            " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTranPayout, trans);

                        string updateBajajArHead = "update BAJAJ_AR_HEAD         set     CLIENT_CD=" + New_Inv_Code +
                            ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR') where CLIENT_CD=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateBajajArHead, trans);

                        string updateTransNetBalance = "update TRAN_NET_BALANCE6@mf.bajajcapital      set   CLIENT_CODE=" + New_Inv_Code +
                            " where CLIENT_CODE=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransNetBalance, trans);

                        string updateTransIPO2 = "update TRAN_IPO              set      inv_code=" + New_Inv_Code +
                            ",CLIENT_CODE=" + mainAgent + " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTransIPO2, trans);

                        string updateTranLead = "update TRAN_LEAD             set      inv_code=" + New_Inv_Code +
                            " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateTranLead, trans);

                        string updateLeadDetail = "update LEADS.LEAD_DETAIL     set      inv_code=" + New_Inv_Code +
                            " where inv_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateLeadDetail, trans);

                        string updatePortTransSt = "update port_TRANSACTION_ST@mf.bajajcapital        set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updatePortTransSt, trans);

                        string updateOnlineTransSt = "update online_transaction_st   set   client_code=" + New_Inv_Code +
                            ",branch_code=" + branch_cd + ",source_code=" + mainAgent + ",rmcode=" + Rm_cd +
                            " where client_code=" + rsDataRow["INV_CODE"];
                        ExecuteNonQuery(updateOnlineTransSt, trans);

                        string insertInvDelHist = "insert into Inv_Del_Hist_Agent_Merge (inv_code,new_inv_code,UpdateOn,UpdatedBy) values ('" +
                            rsDataRow["INV_CODE"] + "','" + New_Inv_Code + "',to_date('" + DateTime.Now.ToString("dd/MM/yyyy") +
                            "','dd/MM/yyyy'),'" + loginId + "')";
                        ExecuteNonQuery(insertInvDelHist, trans);

                        string updateTransStOnline = "update transaction_st_online    set client_code='" + New_Inv_Code +
                            "'      where client_code='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateTransStOnline, trans);

                        string updateOnlineClientReq = "update online_client_request    set inv_code='" + New_Inv_Code +
                            "'         where inv_code='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOnlineClientReq, trans);

                        string updateOnlineClientReqHist = "update online_client_request_hist    set inv_code='" + New_Inv_Code +
                            "'         where inv_code='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOnlineClientReqHist, trans);

                        string updateOnlineBusSum = "update online_business_summary  set client_codewm='" + New_Inv_Code +
                            "'    where client_codewm='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOnlineBusSum, trans);

                        string updateOfflineBusSum = "update offline_business_summary set client_codewm='" + New_Inv_Code +
                            "'    where client_codewm='" + rsDataRow["INV_CODE"] + "'";
                        ExecuteNonQuery(updateOfflineBusSum, trans);

                        if (rsInv_check.Rows.Count > 0)
                        {
                            string insertClientInvMergeLog = "insert into client_inv_merge_log values('" + New_Inv_Code +
                                "','" + rsDataRow["INV_CODE"] + "','" + loginId + "',sysdate)";
                            ExecuteNonQuery(insertClientInvMergeLog, trans);
                            string insertInvDel = "insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(insertInvDel, trans);
                            string deleteInvMaster = "Delete from INVESTOR_MASTER  where inv_code=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(deleteInvMaster, trans);
                            string deleteInvMasterBajaj = "Delete from INVESTOR_MASTER@mf.bajajcapital  where inv_code=" + rsDataRow["INV_CODE"];
                            ExecuteNonQuery(deleteInvMasterBajaj, trans);
                        }
                        // rsInv_check is now closed (goes out of scope)
                        // RsData.MoveNext handled by foreach
                    }

                    string updateInvestorMasterAfter = "update INVESTOR_MASTER           set BRANCH_CODE=" + branch_cd +
                        ",RM_CODE=" + Rm_cd + ",modify_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where source_id=" + mainAgent;
                    ExecuteNonQuery(updateInvestorMasterAfter, trans);

                    string updateAgentMasterAfter = "update agent_MASTER             set sourceid=" + branch_cd +
                        ",RM_CODE=" + Rm_cd + ",modify_DATE=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where agent_code=" + mainAgent;
                    ExecuteNonQuery(updateAgentMasterAfter, trans);

                    string updateTransactionStAfter = "update TRANSACTION_ST            set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionStAfter, trans);

                    string updateTransactionMFTemp1After = "update TRANSACTION_MF_TEMP1      set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionMFTemp1After, trans);

                    string updateTransactionStBajajAfter = "update TRANSACTION_ST@mf.bajajcapital            set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionStBajajAfter, trans);

                    string updatePortTransStAfter = "update port_TRANSACTION_ST@mf.bajajcapital            set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updatePortTransStAfter, trans);

                    string updateTransactionStTempAfter = "update TRANSACTION_STTEMP        set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + ",modify_TALISMA=TO_DATE('" + ServerDateTime.ToString("dd/MM/yyyy") + "','DD/MM/RRRR')" +
                        " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateTransactionStTempAfter, trans);

                    string updateRedemAfter = "update REDEM                     set   branch_code=" + branch_cd +
                        ",rmcode=" + Rm_cd + " where source_code=" + mainAgent;
                    ExecuteNonQuery(updateRedemAfter, trans);

                    string updatePaymentDetail = "update PAYMENT_DETAIL            set agent_code=" + mainAgent +
                        " where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(updatePaymentDetail, trans);

                    string updateLedger = "update LEDGER                    set AGENT_code=" + mainAgent +
                        " where AGENT_code=" + agentsArray[i];
                    ExecuteNonQuery(updateLedger, trans);

                    string updateUpfrontPaid = "update upfront_paid set client_agent_code='" + mainAgent +
                        "' where client_agent_code='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateUpfrontPaid, trans);

                    string updateAddIncentive = "update ADD_INCENTIVE_PAID set client_agent_code='" + mainAgent +
                        "' where client_agent_code='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateAddIncentive, trans);

                    string updateSIPBrokerBilling = "update SIP_BROKER_BILLING1 set SOURCE_CODE='" + mainAgent +
                        "' where SOURCE_CODE='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateSIPBrokerBilling, trans);

                    string updateSTPBrokerBilling = "update STP_BROKER_BILLING1 set SOURCE_CODE='" + mainAgent +
                        "' where SOURCE_CODE='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateSTPBrokerBilling, trans);

                    string updateAdvisorSubEntry = "update ADVISORSUBENTRY set anacode='" + mainAgent +
                        "' where anacode='" + agentsArray[i] + "'";
                    ExecuteNonQuery(updateAdvisorSubEntry, trans);

                    //######################################################################################3

                    // --- Update rsClient fields with values from rsclient1 ---
                    if (rsClient.Rows.Count > 0 && rsclient1.Rows.Count > 0)
                    {
                        DataRow rowClient = rsClient.Rows[0];
                        DataRow rowClient1 = rsclient1.Rows[0];

                        // If PHONE is null or empty and rsclient1 has a PH0NE value, update it.
                        if ((rowClient["PHONE"] == DBNull.Value || string.IsNullOrWhiteSpace(rowClient["PHONE"].ToString())) &&
                            rowClient1["PHONE"] != DBNull.Value)
                        {
                            rowClient["PHONE"] = rowClient1["PHONE"];
                        }
                        if (rowClient["EMAIL"] == DBNull.Value && rowClient1["EMAIL"] != DBNull.Value)
                        {
                            rowClient["EMAIL"] = rowClient1["EMAIL"];
                        }
                        if (rowClient["MOBILE"] == DBNull.Value && rowClient1["MOBILE"] != DBNull.Value && rowClient1["MOBILE"].ToString() != "0")
                        {
                            rowClient["MOBILE"] = rowClient1["MOBILE"];
                        }
                        if (rowClient["PINCODE"] == DBNull.Value && rowClient1["PINCODE"] != DBNull.Value)
                        {
                            rowClient["PINCODE"] = rowClient1["PINCODE"];
                        }
                        if (rowClient["CITY_ID"] == DBNull.Value && rowClient1["CITY_ID"] != DBNull.Value)
                        {
                            rowClient["CITY_ID"] = rowClient1["CITY_ID"];
                        }
                        if (rowClient["DOB"] == DBNull.Value && rowClient1["DOB"] != DBNull.Value)
                        {
                            rowClient["DOB"] = rowClient1["DOB"];
                        }
                        if (rowClient["EXIST_CODE"] == DBNull.Value && rowClient1["EXIST_CODE"] != DBNull.Value)
                        {
                            rowClient["EXIST_CODE"] = rowClient1["EXIST_CODE"];
                        }
                        if (rowClient["TDS"] == DBNull.Value && rowClient1["TDS"] != DBNull.Value)
                        {
                            rowClient["TDS"] = rowClient1["TDS"];
                        }
                        if (rowClient["INTRODUCER"] == DBNull.Value && rowClient1["INTRODUCER"] != DBNull.Value)
                        {
                            rowClient["INTRODUCER"] = rowClient1["INTRODUCER"];
                        }

                        if (rowClient1["JOININGDATE"] != DBNull.Value)
                        {
                            string clientJoining = rowClient["JOININGDATE"] != DBNull.Value ? Convert.ToDateTime(rowClient["JOININGDATE"]).ToString("dd/MM/yyyy") : "";
                            string client1Joining = Convert.ToDateTime(rowClient1["JOININGDATE"]).ToString("dd/MM/yyyy");
                            if (!CheckDate(clientJoining, client1Joining))
                            {
                                // If the dates do not match, use creation_date (if available) from rsclient1.
                                if (rowClient1.Table.Columns.Contains("creation_date") && rowClient1["creation_date"] != DBNull.Value)
                                    rowClient["JOININGDATE"] = rowClient1["creation_date"];
                                else
                                    rowClient["JOININGDATE"] = rowClient1["JOININGDATE"];
                            }
                        }
                        if (rowClient["JOININGDATE"] == DBNull.Value && rowClient1["JOININGDATE"] != DBNull.Value)
                        {
                            rowClient["JOININGDATE"] = rowClient1["JOININGDATE"];
                        }
                        if (rowClient1["LAST_TRAN_DT1"] != DBNull.Value)
                        {
                            string clientLastTran = rowClient["LAST_TRAN_DT1"] != DBNull.Value ? Convert.ToDateTime(rowClient["LAST_TRAN_DT1"]).ToString("dd/MM/yyyy") : "";
                            string client1LastTran = Convert.ToDateTime(rowClient1["LAST_TRAN_DT1"]).ToString("dd/MM/yyyy");
                            if (!CheckDate(client1LastTran, clientLastTran))
                            {
                                rowClient["LAST_TRAN_DT1"] = rowClient1["LAST_TRAN_DT1"];
                            }
                        }
                        if (rowClient["LAST_TRAN_DT1"] == DBNull.Value && rowClient1["LAST_TRAN_DT1"] != DBNull.Value)
                        {
                            rowClient["LAST_TRAN_DT1"] = rowClient1["LAST_TRAN_DT1"];
                        }

                        // Update the adjusted rsClient record back to the database.
                        string updateAgentMaster = "update agent_master set " +
                            "PHONE=" + (rowClient["PHONE"] != DBNull.Value ? "'" + rowClient["PHONE"].ToString() + "'" : "null") + "," +
                            "EMAIL=" + (rowClient["EMAIL"] != DBNull.Value ? "'" + rowClient["EMAIL"].ToString() + "'" : "null") + "," +
                            "MOBILE=" + (rowClient["MOBILE"] != DBNull.Value ? rowClient["MOBILE"].ToString() : "null") + "," +
                            "PINCODE=" + (rowClient["PINCODE"] != DBNull.Value ? rowClient["PINCODE"].ToString() : "null") + "," +
                            "CITY_ID=" + (rowClient["CITY_ID"] != DBNull.Value ? rowClient["CITY_ID"].ToString() : "null") + "," +
                            "DOB=" + (rowClient["DOB"] != DBNull.Value ? "TO_DATE('" + Convert.ToDateTime(rowClient["DOB"]).ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy')" : "null") + "," +
                            "EXIST_CODE=" + (rowClient["EXIST_CODE"] != DBNull.Value ? rowClient["EXIST_CODE"].ToString() : "null") + "," +
                            "TDS=" + (rowClient["TDS"] != DBNull.Value ? rowClient["TDS"].ToString() : "null") + "," +
                            "INTRODUCER=" + (rowClient["INTRODUCER"] != DBNull.Value ? rowClient["INTRODUCER"].ToString() : "null") +
                            (rowClient["JOININGDATE"] != DBNull.Value ? ",JOININGDATE=TO_DATE('" + Convert.ToDateTime(rowClient["JOININGDATE"]).ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy')" : "") +
                            (rowClient["LAST_TRAN_DT1"] != DBNull.Value ? ",LAST_TRAN_DT1=TO_DATE('" + Convert.ToDateTime(rowClient["LAST_TRAN_DT1"]).ToString("dd/MM/yyyy") + "', 'dd/MM/yyyy')" : "") +
                            " where agent_code=" + mainAgent;
                        ExecuteNonQuery(updateAgentMaster, trans);
                    }
                    // **********************

                    // Pankaj
                    string insertClientInvMergeLog2 = "insert into client_inv_merge_log values('" + mainAgent + "','" + agentsArray[i] + "','" + loginId + "',sysdate)";
                    ExecuteNonQuery(insertClientInvMergeLog2, trans);
                    string insertAgentDel = "insert into agent_del select * from agent_master where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(insertAgentDel, trans);
                    string deleteAgentMaster = "Delete from agent_master where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(deleteAgentMaster, trans);
                    string deleteAgentMasterBajaj = "Delete from agent_master@mf.bajajcapital where agent_code=" + agentsArray[i];
                    ExecuteNonQuery(deleteAgentMasterBajaj, trans);

                    // History of updations (investor wise)
                    string insertAgentDelHist = "insert into Agent_Del_Hist_Agent_Merge (agent_code,new_agent_code,UpdateOn,UpdatedBy) values ('" +
                        agentsArray[i] + "','" + mainAgent + "',to_date('" + DateTime.Now.ToString("dd/MM/yyyy") + "','dd/MM/yyyy'),'" + loginId + "')";
                    ExecuteNonQuery(insertAgentDelHist, trans);

                    // Commit the transaction for this merge iteration.
                    trans.Commit();

                    // If rsHead is open then close it.
                    // Reset Members and Fam_Head variables.
                    string Members = "";
                    Fam_Head = "";
                    string queryRsHead = "Select * from fp_investor where substr(familyhead_code,1,8)=" + mainAgent +
                        " and (fp_type='Snapshot' or Fp_type='Comprehensive') order by familyhead_code desc ";
                    DataTable rsHead = ExecuteQuery(queryRsHead);
                    if (rsHead.Rows.Count > 1)
                    {
                        Fam_Head = rsHead.Rows[0]["familyhead_code"].ToString();
                        Members1 = rsHead.Rows[0]["fam_mem1"].ToString();
                        string insertDupFpInvestor = "insert into dup_fp_investor select * from fp_investor where familyhead_code=" + Fam_Head;
                        ExecuteNonQuery(insertDupFpInvestor);
                        string updateFpInvestorDup = "update fp_investor set fam_mem1=fam_mem1||'#'||'" + Members1 + "' where substr(familyhead_code,1,8)=" + mainAgent +
                            " and (fp_type='Snapshot' or Fp_type='Comprehensive')";
                        ExecuteNonQuery(updateFpInvestorDup);
                    }
                    flag = false;
                    // RsData is closed by going out of scope.
                } // End for loop

                // Reset MousePointer and show success message.
                // Me.MousePointer = 0;
                // MsgBox "Client(s) Merged Successfully", vbInformation
                // Reset grid rows (simulated by calling SetGrid).
                SetGrid();
            }
            catch (Exception err)
            {
                // Error trap:
                // Rollback any pending transaction.
                try
                {
                    if (MyConn != null && MyConn.State == ConnectionState.Open)
                    {
                        // Note: In this implementation transactions are committed per iteration.
                        // Any active transaction here should be rolled back.
                    }
                }
                catch { }
                // Reset MousePointer and display error.
                // Me.MousePointer = 0;
                // MsgBox err.Description
                // Re-enable frames.
                // Frame1.Enabled = true; Frame2.Enabled = true; Frame3.Enabled = true; Frame4.Enabled = true;
                throw new Exception(err.Message);
            }
            finally
            {
                // Re-enable frames if needed.
                // Frame1.Enabled = true; Frame2.Enabled = true; Frame3.Enabled = true; Frame4.Enabled = true;
                if (MyConn != null && MyConn.State == ConnectionState.Open)
                    MyConn.Close();
            }
        }
    
    
    
    }
}
