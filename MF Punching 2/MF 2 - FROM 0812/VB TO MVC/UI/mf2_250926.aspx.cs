using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WM.Controllers;
using WM.Models;


namespace WM.Masters
{
    public partial class mf_punching_interface : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LoginId"] = "121397";
            Session["roleId"] = "212";
            hdnLoginId.Value = Session["LoginId"]?.ToString();
            hdnRoleId.Value = Session["roleId"]?.ToString();

            // PSM_MF2_AMC_MASTER
            PsmController pc = new PsmController();
            var amcList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_MF2_AMC_MASTER", "MF_PUNCHING", null, null); // MUT_NAME, MUT_CODE
            var branchList = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_PUNCHING", null, null); // BRANCH_NAME, BRANCH_CODE


            var DATA = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_COUNTRY_STATE_CITY", "FOR_COUNTRY", null, null);  // P_FOR: 'COUNTRY_LIST', 'STATE_LIST', 'CITY_LIST', ##//P_BY: 'BY_COUNTRY', 'BY_STATE', 'BY_CITY'


        }

        #region HELPING FUNCTION

        // ONLY FOR MESSAGE PRINT WITH BOOL STATUS
        private static string CreateJsonResponse_(bool success, string message)
        {
            return JsonConvert.SerializeObject(new
            {
                success,
                message
            }, Formatting.None);
        }


        private static object CreateJsonResponse(bool success, string message)
        {
            return new
            {
                Success = success,
                Message = message
            };
        }


        #endregion

        #region DDL DATA

        [WebMethod]
        public static string GetAMCMasterList()
        {
            PsmController pc = new PsmController();

            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_MF2_AMC_MASTER", "MF_PUNCHING", null, null); // ZONE_NAME, ZONE_ID

            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["mut_name"]), value = Convert.ToString(row["mut_code"]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }


        [WebMethod]
        public static string GetBranchList()
        {
            PsmController pc = new PsmController();

            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_BRANCH_LIST", "MF_PUNCHING", null, null); // BRANCH_NAME, BRANCH_CODE

            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row["BRANCH_NAME"]), value = Convert.ToString(row["BRANCH_CODE"]) });

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
        public static string GetCountryStateCity(string for_x, string by_y, string y)
        {
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList("WEALTHMAKER.PSM_LOG_COUNTRY_STATE_CITY", for_x, by_y, y); // FOR: 'COUNTRY_LIST', 'STATE_LIST', 'CITY_LIST' ## // BY:  'BY_STATE', 'BY_CITY'

            foreach (DataRow row in data.Rows)
            {
                if (for_x == "COUNTRY_LIST")
                {
                    list.Add(new { text = Convert.ToString(row["COUNTRY_NAME"]), value = Convert.ToString(row["COUNTRY_ID"]) });
                }
                else if (for_x == "STATE_LIST")
                {
                    list.Add(new { text = Convert.ToString(row["STATE_NAME"]), value = Convert.ToString(row["STATE_ID"]) });

                }
                else if (for_x == "CITY_LIST")
                {
                    list.Add(new { text = Convert.ToString(row["CITY_NAME"]), value = Convert.ToString(row["CITY_ID"]) });
                }
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }


        [WebMethod]
        public static string GetDropdownData(string psm, string for_x, string by_y, string y, string get_name, string get_code)
        {
            string proc = "WEALTHMAKER." + psm;
            PsmController pc = new PsmController();
            List<dynamic> list = new List<dynamic>();
            var data = pc.PSM_LoggedTableList(proc, for_x, by_y, y);
            foreach (DataRow row in data.Rows)
            {
                list.Add(new { text = Convert.ToString(row[get_name]), value = Convert.ToString(row[get_code]) });
            }
            var outPut = new { data = list };
            return JsonConvert.SerializeObject(outPut, Formatting.None);
        }


        #endregion


        [WebMethod]
        public static string GetGridFillList(string tabIndex,string businessRmCodeA,string fromDate,string toDate,string panS,string clientS,string tranNo,string anaCode,string chequeNo,string appNo,string installments,string sipType,string clientCode,string acHolderCode,string businessRmCodeM,string panM,string frequencyCode,string orderBy,string descending)
        {
            MfPunchingController m = new MfPunchingController();

            PsmController pc = new PsmController();
            string branches = null;// pc.LogBranches() ?? string.Empty;
            string logId = pc.currentLoginID() ?? string.Empty;
            string roleId = pc.currentRoleID() ?? string.Empty;

            
            var gridData = m.MF2_GetTrX(
                tabIndex,
                businessRmCodeA,
                branches,        // Added
                logId,           // Added
                roleId,          // Added
                fromDate,
                toDate,
                panS,
                clientS,
                tranNo,
                anaCode,
                chequeNo,
                appNo,
                installments,
                sipType,
                clientCode,
                acHolderCode,
                businessRmCodeM,
                panM,
                frequencyCode,
                orderBy,
                descending
            );

            return JsonConvert.SerializeObject(new { data = gridData }, Formatting.None);
        }


        [WebMethod]
        public static string ProcessMfAdd_GetByDT(string index, string commonId, string chkSwitch)
        {
            MfPunchingController m = new MfPunchingController();

            // Call controller method
            var result = m.ProcessMfTransactionDocByDT(index, commonId, chkSwitch);

            // Build JSON response
            return JsonConvert.SerializeObject(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            }, Formatting.None);
        }

        [WebMethod]
        public static string ProcessBusiCodeLostFocus(string busiCode, string invCode, string allIndia, string allIndiaSearchLag, string branches)
        {
            try
            {
                var controller = new MfPunchingController();
                var result = controller.ProcssMF2_BusiCodeLostFocuse(busiCode, invCode, allIndia, allIndiaSearchLag, branches);

                return JsonConvert.SerializeObject(new
                {
                    success = result.Success,
                    message = result.Message,
                    data = result.Data
                }, Formatting.None);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = $"System Error: {ex.Message}",
                    data = new Dictionary<string, object>()
                }, Formatting.None);
            }
        }

        [WebMethod]
        public static string ProcessCrossChannelGetData(string connonID, string inv_code, string frm_type)
        {
            PsmController m = new PsmController();

            var isValid = m.PSMM_CROSS_CH_VALIDATE(frm_type, connonID, inv_code);

            if (isValid.Success && isValid.Count > 0)
            {
                var result = m.PSMM_CROSS_CH_GET_DATA1(connonID, inv_code);

                if (result.Rows.Count > 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = true,
                        message = "SUCCESS: Record Found!",
                        data = result
                    }, Formatting.None);
                }
                else
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "Record Not Found!",
                        data = (object)null
                    }, Formatting.None);
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Validation failed or no records found",
                    data = (object)null
                }, Formatting.None);
            }
        }

        [WebMethod]
        public static string ProcessInvestoUpdateGetData(string inv_code)
        {
            PsmController m = new PsmController();

            var result = m.PSMM_INV_ADD_UP_GET_DATA(inv_code);

            return JsonConvert.SerializeObject(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            }, Formatting.None);
        }


        [WebMethod]
        public static object ProcessInvAddUp_UpdateData(Dictionary<string, string> formData)
        {
            try
            {
                string code = formData["code"];
                string name = formData["name"];
                string address1 = formData["address1"];
                string address2 = formData["address2"];
                string pin = formData["pin"];
                string email = formData["email"];
                string pan = formData["pan"];
                string mobile = formData["mobile"];
                string aadhar = formData["aadhar"];
                string dobStr = formData["dob"];
                string cityId = formData["city_id"];
                string stateId = formData["state_id"];
                string loginID = HttpContext.Current.Session["LoginId"]?.ToString();

                PsmController controller = new PsmController();
                var result = controller.PSMM_INV_ADD_UP_UP_DATA(code, mobile, pan, email, aadhar, address1, address2, pin, cityId, stateId, dobStr);

                bool isSuccess = result.Success;
                string isMe = result.Message;
                
                //return CreateJsonResponse(result.Success, result.Message);
                return new { Success = result.Success, Message = result.Message };

            }
            catch (Exception ex)
            {
                return new { Success = false, Message = "Error: " + ex.Message };
            }
        }

        #region PSM MODAL: INVESTOR SEARCH TREE

       
        [WebMethod]
        public static string ProcessModalInvestorSearch_Find(string pxBranch, string pxCat, string pxCity, string pxCode,string pxName, string pxAdd1, string pxAdd2, string pxPhone,string pxPan, string pxMobile, string pxNewRm, string pxAhCode,string pxClientSubName, string pxClientBroker, string pxStrForm,string pxCurrentForm, string pxRm, string pxOldRm, string pxSort)
        {
            try
            {
                

                if (string.IsNullOrEmpty(pxCat))
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "Category is required",
                        data = new List<object>()
                    });
                }

                // Process the search
                PsmController controller = new PsmController();
                DataTable result = controller.PSMM_INV_SEARCH_TREE_FIND(
                    pxBranch, pxCat, pxCity, pxCode, pxName, pxAdd1, pxAdd2,
                    pxPhone, pxPan, pxMobile, pxNewRm, pxAhCode, pxClientSubName,
                    pxClientBroker, pxStrForm, pxCurrentForm, pxRm, pxOldRm, pxSort
                );

                if (result != null && result.Rows.Count > 0)
                {
                    return DataTableToJsonWithSchema(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = true,
                        message = "No records found",
                        data = new List<object>(),
                        totalCount = 0
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Error: " + ex.Message,
                    data = new List<object>()
                });
            }
        }
        // Enhanced JSON conversion with schema information
       
        private static string DataTableToJsonWithSchema(DataTable dt)
        {
            try
            {
                // Check if the DataTable is null or has no rows
                if (dt == null || dt.Rows.Count == 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "ERROR: No data returned",
                        data = (object)null,
                        schema = (object)null,
                        totalCount = 0,
                        columns = 0
                    });
                }

                // Check if 'msg' column exists — this means it's not actual data
                if (dt.Columns.Contains("msg"))
                {
                    string msg = dt.Rows[0]["msg"]?.ToString().ToUpper();

                    // If msg is present and not empty
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            success = false,
                            message = "ERROR: " + msg,
                            data = (object)null,
                            schema = (object)null,
                            totalCount = 0,
                            columns = 0
                        });
                    }
                }

                // If no 'msg' column or it's empty, treat as successful data
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                List<object> schema = new List<object>();

                // Schema info
                foreach (DataColumn col in dt.Columns)
                {
                    schema.Add(new
                    {
                        columnName = col.ColumnName,
                        dataType = col.DataType.Name,
                        isNullable = col.AllowDBNull
                    });
                }

                // Row data
                foreach (DataRow dr in dt.Rows)
                {
                    var row = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        row[col.ColumnName] = dr[col] == DBNull.Value ? null : dr[col];
                    }
                    rows.Add(row);
                }

                return JsonConvert.SerializeObject(new
                {
                    success = true,
                    message = "SUCCESS: Data retrieved successfully",
                    data = rows,
                    schema = schema,
                    totalCount = rows.Count,
                    columns = dt.Columns.Count
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "ERROR: " + ex.Message,
                    data = (object)null,
                    schema = (object)null,
                    totalCount = 0,
                    columns = 0
                });
            }
        }

        [WebMethod] // not in use
        public static string ProcessInvSearchRowClick_Main(string PX_CUR_FORM,string PX_INV_CODE,string PX_INDEX,string PX_STR_FORM,string PX_FCT_CD_CODE,string PX_CAT,string PX_FIM_CMB_CLIENT,string PX_FCIR_CD,string PX_CM_INVESTORS,string PX_CM_BRANCH_NAMES,string PX_INV_BRANCH_NAME,string PX_FAR_AR_TYPE,string PX_FAR_AR_MY_OPT,string PX_FARREN_AR_TYPE,string PX_FPAY_CL_CD,string PX_FJV_CL_CD,string PX_FJV_AG_CD,string PX_FSYNC_REV_TR,string PX_FRM_SYNC_TM1,string PX_FRM_SYNC_TM10,string PX_FRM_SYNC_TM12, string V_chkIndia)
        {
            PsmController m = new PsmController();

            var result = m.PSMM_INV_SEARCH_ROW_CLICK(
                PX_CUR_FORM,
                PX_INV_CODE,
                PX_INDEX,
                PX_STR_FORM,
                PX_FCT_CD_CODE,
                PX_CAT,
                PX_FIM_CMB_CLIENT,
                PX_FCIR_CD,
                PX_CM_INVESTORS,
                PX_CM_BRANCH_NAMES,
                PX_INV_BRANCH_NAME,
                PX_FAR_AR_TYPE,
                PX_FAR_AR_MY_OPT,
                PX_FARREN_AR_TYPE,
                PX_FPAY_CL_CD,
                PX_FJV_CL_CD,
                PX_FJV_AG_CD,
                PX_FSYNC_REV_TR,
                PX_FRM_SYNC_TM1,
                PX_FRM_SYNC_TM10,
                PX_FRM_SYNC_TM12,
                V_chkIndia);

            return JsonConvert.SerializeObject(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            }, Formatting.None);
        }

        [WebMethod]
        public static string ProcessInvSearchRowClick_MF(string PX_INV_CODE, string PX_INDEX, string PX_CAT)
        {
            string PX_CUR_FORM = "frmtransactionmf";
            string PX_STR_FORM = "frmtransactionmf";

            PsmController controller = new PsmController();

            var result = controller.PSMM_INV_SEARCH_ROW_CLICK(
                PX_CUR_FORM,      // Forced
                PX_INV_CODE,
                PX_INDEX,
                PX_STR_FORM,      // Forced
                null,             // PX_FCT_CD_CODE
                PX_CAT,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null
            );

            return JsonConvert.SerializeObject(new
            {
                success = result.Success,
                message = result.Message,
                data = result.Data
            }, Formatting.None);
        }



        #endregion PSM MODAL: INVESTOR SEARCH TREE

        #region PSM MODAL: SCHEME SEARCH TREE

        [WebMethod]
        public static string ProcessSchemeSearch(string pxSchStr)
        {
            try
            {
                // You can add validation if needed
                if (string.IsNullOrEmpty(pxSchStr))
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "Search string is required",
                        data = new List<object>()
                    });
                }

                // Call your controller method
                PsmController controller = new PsmController();
                DataTable result = controller.PSMM_SCHEME_SEARCH_GET(pxSchStr);

                if (result != null && result.Rows.Count > 0)
                {
                    return controller.DataTableToJsonWithSchema(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = true,
                        message = "No records found",
                        data = new List<object>(),
                        totalCount = 0
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Error: " + ex.Message,
                    data = new List<object>()
                });
            }
        }


        [WebMethod]
        public static string ProcessSchemeSearchGetRow(string pxRowData,string pxFrmStr,string pxAmcCol = null,string pxCol1 = null,string pxCol2 = null,string pxCol3 = null,string pxCol4 = null,string pxCol5 = null,string pxCol6 = null,string pxCol7 = null,string pxOther1 = null,string pxOther2 = null)
        {
            try
            {
                // Required parameter validation
                if (string.IsNullOrEmpty(pxRowData))
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "Row data is required",
                        data = new List<object>()
                    });
                }

                if (string.IsNullOrEmpty(pxFrmStr))
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = false,
                        message = "Form string is required",
                        data = new List<object>()
                    });
                }

                // Call your controller method
                PsmController controller = new PsmController();
                DataTable result = controller.PSMM_SHEME_SEARCH_GET_ROW_DB(
                    pxRowData,
                    pxFrmStr,
                    pxAmcCol,
                    pxCol1,
                    pxCol2,
                    pxCol3,
                    pxCol4,
                    pxCol5,
                    pxCol6,
                    pxCol7,
                    pxOther1,
                    pxOther2
                );

                if (result != null && result.Rows.Count > 0)
                {
                    return controller.DataTableToJsonWithSchema(result);
                }
                else
                {
                    return JsonConvert.SerializeObject(new
                    {
                        success = true,
                        message = "No records found",
                        data = new List<object>(),
                        totalCount = 0
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    success = false,
                    message = "Error: " + ex.Message,
                    data = new List<object>()
                });
            }
        }
        
        
        #endregion
    }
}

