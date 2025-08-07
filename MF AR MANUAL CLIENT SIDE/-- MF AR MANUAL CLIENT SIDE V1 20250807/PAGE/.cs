using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using WM.Controllers;
using WM.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Oracle.ManagedDataAccess.Client;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Generic;
using System.Web.Script.Services;
using NPOI.SS.Formula.Functions;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Globalization;
using Newtonsoft.Json;
using System.Web.Services;
using System.Web;
using System.Web.Script.Serialization;
using System.EnterpriseServices;

namespace WM.Masters
{
    public partial class MfManualReconciliation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PsmController pc = new PsmController();

            #region LOGIN SESSION HANDLING

            Session["LoginId"] = "112650";
            Session["roleId"] = "29";

            hdnLoginId.Value = Session["LoginId"]?.ToString();
            hdnRoleId.Value = Session["roleId"]?.ToString();

            #endregion

            #region DDL LIST DYNAMIC NEW

            var channelList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_CHANNEL_LIST", "MF_AR_MANUAL_RECO", null, null); // CHANNEL_NAME, CHANNEL_CODE
            var regionList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_REGION_LIST", "MF_AR_MANUAL_RECO", null, null); // REGION_NAME, REGION_CODE
            var zoneList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_ZONE_LIST", "MF_AR_MANUAL_RECO", null, null); // ZONE_NAME, ZONE_ID
            var branchList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_AR_MANUAL_RECO", null, null); // BRANCH_NAME, BRANCH_CODE
            var rmList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_RM_LIST", "MF_AR_MANUAL_RECO", null, null); // RM_NAME, PAYROLL_ID, RM_CODE
            var amcList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_AMC_LIST", "MF_AR_MANUAL_RECO", null, null); // MUT_NAME, MUT_CODE

            var regionByChannel = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_REGION_LIST", "MF_AR_MANUAL_RECO", "CHANNEL", "184"); // 184
            var zoneByChannel = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_ZONE_LIST", "MF_AR_MANUAL_RECO", "CHANNEL", "184"); // 184
            var branchByChannel = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_AR_MANUAL_RECO", "CHANNEL", "184"); // 184
            var zoneByRegion = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_ZONE_LIST", "MF_AR_MANUAL_RECO", "REGION", "R01");  // R01
            var branchByZone = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_AR_MANUAL_RECO", "ZONE", "Z03");  // Z03
            var rmByBranch = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_RM_LIST", "MF_AR_MANUAL_RECO", "BRANCH", "10010020"); // 10010020
            #endregion

        }

        #region HELPING FUNCTIONS
        private static string CreateJsonResponse(bool success, string message)
        {
            return JsonConvert.SerializeObject(new
            {
                success,
                message
            }, Formatting.None);
        }

        #endregion

        #region ON LAOD AND ON CHANGE: DDL LIST AND BY DDL LIST

        [WebMethod]
        public static string GetChannelList()
        {
            PsmController pc = new PsmController();

            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_CHANNEL_LIST", "MF_AR_MANUAL_RECO", null, null); // CHANNEL_NAME, CHANNEL_CODE
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["channel_name"]), value = Convert.ToString(row["channel_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetBranchList()
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_AR_MANUAL_RECO", null, null); // BRANCH_NAME, BRANCH_CODE
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["BRANCH_NAME"]), value = Convert.ToString(row["BRANCH_CODE"]) });
            }
            var outPut = new { data = list };

            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetZoneList()
        {
            PsmController pc = new PsmController();

            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_ZONE_LIST", "MF_AR_MANUAL_RECO", null, null);


            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["ZONE_NAME"]), value = Convert.ToString(row["ZONE_ID"]) });
            }
            var outPut = new { data = list };

            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetRegionList()
        {
            PsmController pc = new PsmController();

            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_REGION_LIST", "MF_AR_MANUAL_RECO", null, null); // REGION_NAME, REGION_CODE
            foreach (DataRow row in data.Rows)
            {

                list.Add(new { text = Convert.ToString(row["REGION_NAME"]), value = Convert.ToString(row["region_code"]) });
            }
            var outPut = new { data = list };

            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }


        [WebMethod]
        public static string GetRmList()
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_RM_LIST", "MF_AR_MANUAL_RECO", null, null);

            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["RM_NAME"]), value = Convert.ToString(row["payroll_id"]) });
            }
            var outPut = new { data = list };

            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetAMCList()
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_AMC_LIST", "MF_AR_MANUAL_RECO", null, null); // MUT_NAME, MUT_CODE
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["mut_name"]), value = Convert.ToString(row["mut_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetBranchListByChannel(string channel)
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_AR_MANUAL_RECO", "CHANNEL", channel);

            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["Branch_name"]), value = Convert.ToString(row["Branch_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetBranchListByZone(string zone)
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_AR_MANUAL_RECO", "ZONE", zone);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["Branch_name"]), value = Convert.ToString(row["Branch_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetRegionListByChannel(string channel)
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_REGION_LIST", "MF_AR_MANUAL_RECO", "CHANNEL", channel);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["REGION_NAME"]), value = Convert.ToString(row["REGION_CODE"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetZoneListByChannel(string channel)
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_ZONE_LIST", "MF_AR_MANUAL_RECO", "CHANNEL", channel);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["ZONE_NAME"]), value = Convert.ToString(row["ZONE_ID"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetZoneListByRegion(string region)
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_ZONE_LIST", "MF_AR_MANUAL_RECO", "REGION", region);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["ZONE_NAME"]), value = Convert.ToString(row["ZONE_ID"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }

        [WebMethod]
        public static string GetRmListByBranch(string branchCode)
        {
            List<dynamic> list = new List<dynamic>();
            PsmController pc = new PsmController();
            var rmByBr = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_RM_LIST", "MF_AR_MANUAL_RECO", "BRANCH", branchCode);
            foreach (DataRow row in rmByBr.Rows)
            {
                list.Add(new { text = Convert.ToString(row["RM_NAME"]), value = Convert.ToString(row["payroll_id"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }


        #endregion

        #region GET LIST: RT, RTA AND TRAN LIST
        [WebMethod] // TEST PASS
        public static string GetTRList(string channel, string region, string zone, string branch, string rm, string dateFrom, string dateTo, string amc, string arNo, string reconciliationStatus, string cobFlag, string tranType, string registrar)
        {
            var trTransactions = new MfManualReconciliationController().GetTrDetails(channel, region, zone, branch, rm, dateFrom, dateTo, amc, arNo, reconciliationStatus, cobFlag, tranType, registrar);
            return JsonConvert.SerializeObject(new { data = trTransactions }, Formatting.None);
        }

        [WebMethod] // TEST PASS
        public static string GetRTAList(string rtaDtFrom, string rtaDtTo, string rtaRecoStatus, string rtaAmc, string rtaBranch, string rtaCheqType, string rtaCheqValue, string rtaInvName, string rtaAmount, string rtaTranType, string rtaFindText, string trTranType, string trRegistrar)
        {
            //DataTable dt = new MfManualReconciliationController().SearchTransactions(dateFrom, dateTo, status, amc, branch, chequeType, chequeSearch, investorName, amount, tranType, searchText, trTrantype, trReg);
            var rtaTransactions = new MfManualReconciliationController().GetRtaDetails(rtaDtFrom, rtaDtTo, rtaRecoStatus, rtaAmc, rtaBranch, rtaCheqType, rtaCheqValue, rtaInvName, rtaAmount, rtaTranType, rtaFindText, trTranType, trRegistrar);
            return JsonConvert.SerializeObject(new { data = rtaTransactions }, Formatting.None);
        }


        [System.Web.Services.WebMethod] // 
        public static string GetTranList(string tranCode)
        {
            //DataTable dt = new MfManualReconciliationController().SearchTransactions(dateFrom, dateTo, status, amc, branch, chequeType, chequeSearch, investorName, amount, tranType, searchText, trTrantype, trReg);
            var rtaTransactions = new MfManualReconciliationController().GetTranDetails(tranCode);
            return JsonConvert.SerializeObject(new { data = rtaTransactions }, Formatting.None);
        }

        #endregion

        #region Exist functions: NOT IN USE

        [WebMethod]
        public static string ExitToWelcomePage_0()
        {
            PsmController pc = new PsmController();

            string curLog = pc.currentLoginID();
            string curRole = pc.currentRoleID();

            string welcomeUrl = $"~/welcome?loginid={HttpUtility.UrlEncode(curLog)}&roleid={HttpUtility.UrlEncode(curRole)}";
            return JsonConvert.SerializeObject(new { data = welcomeUrl }, Formatting.None);
        }

        [WebMethod]
        public static string ExitToWelcomePage()
        {
            List<dynamic> resultList = new List<dynamic>();

            try
            {
                PsmController pc = new PsmController();
                string curLog = pc.currentLoginID();
                string curRole = pc.currentRoleID();

                // Add user data as an object to the list
                resultList.Add(new
                {
                    success = true,
                    loginid = curLog,
                    roleid = curRole
                });

                // Return as a structured object with a "data" property
                var output = new { data = resultList };
                return JsonConvert.SerializeObject(output, Formatting.None);
            }
            catch (Exception ex)
            {
                // Add error case to the list
                resultList.Add(new
                {
                    success = false,
                    message = "Exit failed: " + ex.Message
                });

                var output = new { data = resultList };
                return JsonConvert.SerializeObject(output, Formatting.None);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Dictionary<string, string> GetSessionData()
        {
            return new Dictionary<string, string>
    {
        { "LoginId", HttpContext.Current.Session["LoginId"]?.ToString() },
        { "RoleId", HttpContext.Current.Session["roleId"]?.ToString() }
    };
        }

        #endregion

        #region RTA ACTION: REMARK SAVE, RECONCILE

        [WebMethod]// TESTING PASS
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveRemark(string trCode, string txtRemark)
        {
            try
            {
                #region VALIDATIONS
                if (string.IsNullOrWhiteSpace(trCode))
                {
                    return CreateJsonResponse(false, "Transaction Code is required.");
                }

                if (string.IsNullOrWhiteSpace(txtRemark))
                {
                    return CreateJsonResponse(false, "Remark cannot be empty.");
                }
                #endregion

                var controller = new MfManualReconciliationController();
                var result = controller.SetTransactionRemark(
                    transactionCode: trCode,
                    remark: txtRemark
                );

                return CreateJsonResponse(result.Success, result.Message);
            }
            catch (Exception ex)
            {
                return CreateJsonResponse(false, $"System Error: {ex.Message}");
            }
        }


     

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string ReconcileTransactions(
    string trTranCodeValue, string trTranTypeValue, string rtaTranCodeValue,
    string rtaTranDateValue, string rtaFolioValue, string rtaAmountValue,
    string rtaDispatchValue)
        {
            try
            {
                #region VALIDATIONS
                if (string.IsNullOrWhiteSpace(trTranCodeValue))
                {
                    return CreateJsonResponse(false, "Transaction Code is required.");
                }

                if (string.IsNullOrWhiteSpace(rtaTranCodeValue))
                {
                    return CreateJsonResponse(false, "RTA Transaction Code is required.");
                }

                if (!decimal.TryParse(rtaAmountValue, out decimal rtaAmountValue_))
                {
                    return CreateJsonResponse(false, "RTA Amount must be a valid number.");
                }

                if (rtaAmountValue_ <= 0)
                {
                    return CreateJsonResponse(false, "RTA Amount must be greater than zero.");
                }
                #endregion

                MfManualReconciliationController controller = new MfManualReconciliationController();
                var result = controller.SetReconcileTransactions(
                    trTranCode: trTranCodeValue,
                    trTranType: trTranTypeValue.ToUpper(),
                    rtaTranCode: rtaTranCodeValue,
                    rtaTranDate: rtaTranDateValue,
                    rtaFolio: rtaFolioValue,
                    rtaAmount: rtaAmountValue_,
                    rtaDispatch: rtaDispatchValue.ToUpper()
                );

                return CreateJsonResponse(result.Success, result.Message);
            }
            catch (Exception ex)
            {
                return CreateJsonResponse(false, $"System Error: {ex.Message}");
            }
        }




        [WebMethod]
        public static string RtaConfirmPMS(string trCode, string remark, bool pmsStatus, bool atmStatus)
        {
            try
            {
                if (string.IsNullOrEmpty(trCode)) return CreateJsonResponse(false, "Select Record");

                if (pmsStatus && string.IsNullOrWhiteSpace(remark)) return CreateJsonResponse(false, "Enter Remark for PMS confirmation");

                if (!pmsStatus && !atmStatus) return CreateJsonResponse(false, "Select Either PMS or ATM Transaction Type");

                if ((pmsStatus || atmStatus) && string.IsNullOrEmpty(trCode)) return CreateJsonResponse(false, "First Select The Record You Want To Map");

                MfManualReconciliationController controller = new MfManualReconciliationController();
                var result = controller.SetPMSATMConfirmation(
                    tranCode: trCode,
                    remarks: remark,
                    optPMS: pmsStatus,
                    optATM: atmStatus
                );

                return CreateJsonResponse(result.Success, result.Message);
            }
            catch (Exception ex)
            {
                return CreateJsonResponse(false, $"Error: {ex.Message}");
            }
        }

        [WebMethod]
        public static string RtaUnConfirmPMS(string trCode, string remark, bool pmsStatus, bool atmStatus)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(trCode))
                    return CreateJsonResponse(false, "Please select a transaction record to unconfirm");

                if (string.IsNullOrWhiteSpace(remark))
                    return CreateJsonResponse(false, "Remarks are required for PMS unconfirmation");

                if (!pmsStatus && !atmStatus)
                    return CreateJsonResponse(false, "Please select either PMS or ATM transaction type to unconfirm");

                // Call the controller method
                MfManualReconciliationController controller = new MfManualReconciliationController();
                var result = controller.SetPMSATMUnConfirmation(
                    tranCode: trCode,
                    remarks: remark,
                    optPMS: pmsStatus,
                    optATM: atmStatus
                );

                // Return JSON response
                return CreateJsonResponse(result.Success, result.Message);
            }
            catch (Exception ex)
            {
                // Log the error if needed
                // Logger.Error(ex, "Error in RtaUnConfirmPMS WebMethod");

                return CreateJsonResponse(false, $"Unconfirmation failed: {ex.Message}");
            }
        }



        #endregion

    }
}
