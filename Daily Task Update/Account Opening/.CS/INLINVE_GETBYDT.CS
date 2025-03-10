if (!string.IsNullOrWhiteSpace(commonID))
{
    string sql = "select CHECK_DT_NEWINV('" + commonID.Trim() +"') as isvaliddt from dual";

    DataTable dt = CurrentSql(sql);
    if (dt.Rows.Count > 0)
    {
        DataRow row = dt.Rows[0];
        string isvalid = GetTextFieldValue(row, "isvaliddt");
        if ((isvalid) != "0")
        {
            string whatValid = "SELECT 'Valid Data: DT Exist - Open New Account' AS message, NVL(tdu.BUSI_RM_CODE, '') AS BUSI_RM_CODE, NVL(tdu.BUSI_BRANCH_CODE, '') AS BUSI_BRANCH_CODE, NVL(tdu.common_id, '') AS COMMON_ID, NVL(tdu.common_id, '') AS DOC_ID, NVL(em.payroll_id, 'No code') AS business_code, NVL(em.rm_name, 'No RM') AS rm_name, NVL(bm.branch_name, 'No Branch') AS BRANCH_CODE, NVL(tdu.guest_cd, '') AS guest_code FROM tb_doc_upload tdu LEFT JOIN employee_master em ON em.payroll_id = tdu.busi_rm_code LEFT JOIN branch_master bm ON bm.branch_code = tdu.busi_branch_code WHERE tdu.common_id = '" + commonID.Trim() + "' AND ROWNUM = 1";
            DataTable whatValidData = CurrentSql(whatValid);
            if (whatValidData.Rows.Count > 0)
            {
                DataRow rowValid = whatValidData.Rows[0];
                string rccDTValue = GetTextFieldValue(rowValid, "COMMON_ID");
                string rccDOCValue = GetTextFieldValue(rowValid, "DOC_ID");
                string rccBusiValue = GetTextFieldValue(rowValid, "business_code");
                string rccRmValue = GetTextFieldValue(rowValid, "rm_name");
                string rccRmBranchValue = GetTextFieldValue(rowValid, "BRANCH_CODE");
                string rccGuestCode = GetTextFieldValue(rowValid, "guest_code");
                string rccBussRMCodeValue = GetTextFieldValue(rowValid, "BUSI_RM_CODE");
                string rccBussBranchCodeValue = GetTextFieldValue(rowValid, "BUSI_BRANCH_CODE");

                if (!string.IsNullOrEmpty(rccGuestCode))
                {
                    
                    string isValidGC = IS_VALID_GUEST(rccGuestCode);
                    if (isValidGC.ToUpper().Contains("PASS"))
                    {
                        FetchedByGuestCode(rccGuestCode);
                        //FillClientByGuestCode(currentGuestValue, loginValue);
                    }
                    else
                    {
                        RESET_GUEST_VALUE();
                        ShowAlert(isValidGC);
                        txtGuestCode.Text = rccGuestCode;
                    }
                    //FillClientByGuestCode(rccGuestCode, Session["LoginId"]?.ToString());

                }
                else
                {

                    UpdateBusinessCodeDetails(rccBusiValue);// SET BUSS RM NAME AND BRANCH
                    txtBusinessCode.Text = rccBusiValue;
                }
                
                txtDTNumber.Text = commonID;
            }

            
            
            
        }
        else
        { 
            string whatInvalid = "select common_id as dt, verification_flag as vf, rejection_status as rf, tran_type as tt, punching_flag as pf from tb_doc_upload where  common_id ='" + commonID + "'";
            DataTable invalidDTData = CurrentSql(whatInvalid);
            if (invalidDTData.Rows.Count > 0)
            {
                DataRow rowInvalid = invalidDTData.Rows[0];
                string dtValue = GetTextFieldValue(rowInvalid, "dt"); // dt number
                string vfValue = GetTextFieldValue(rowInvalid, "vf"); // vefirication flag 1 or 0
                string rfValue = GetTextFieldValue(rowInvalid, "rf"); // rejection status 1 or 0
                string ttValue = GetTextFieldValue(rowInvalid, "tt"); // tran type AC or other
                string pfValue = GetTextFieldValue(rowInvalid, "pf"); // punching flag 1 or 0
                string invalidMsg = $"DT Value: {dtValue}";

                if (vfValue == "0")
                {
                    invalidMsg += ", Not Verified"; // 
                }
                if (rfValue == "1")
                {
                    invalidMsg += ", Rejected Entry";
                }
                if (ttValue != "AC")
                {
                    invalidMsg += $", Invalid Transaction Type";
                }
                if (pfValue == "1")
                {
                    invalidMsg += ", Already Punched";
                }

                // Final Message Output
                if (invalidMsg == $"DT Value: {dtValue}")
                {
                    invalidMsg = "All conditions are valid.";
                }

                // make a single msg on the bases above values


                //invalidMsg = "Invalid DT: " + dtValue + " VF: " + vfValue + " RF: " + rfValue + " TT: " + ttValue + " PF: " + pfValue;
                ShowAlert(invalidMsg);
            }
            else
            {
                ShowAlert("Invalid DT: " + commonID);
            }
            //ShowAlert(isvalid);
        }
    }
    /*
    SEARCH_BY_DT(commonID);
     IF FOUND GUEST CODE THEN 
    IF VLAID GUEST THEN FILL BY GUEST AND AGAIN SET DT NUMBER WITH PREVIOUSE DT VALUE
     */
}
else
{
    ShowAlert("ENTER DT NUMBER");
}