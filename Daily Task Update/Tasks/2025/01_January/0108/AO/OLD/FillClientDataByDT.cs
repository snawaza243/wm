    private void FillClientDataByDTNumber(string id)
    {
        try
        {
            DataTable dt = new AccountOpeningController().GetClientDataByDTNumber(id);                
            if (dt == null || dt.Rows.Count <= 0)
            {
                string emptyMsg = "No Data!";
                ShowAlert(emptyMsg);
            }

            else if (dt.Rows.Count > 0)
            {

                DataRow row = dt.Rows[0];
                string message = dt.Rows[0]["message"].ToString();

                string DtNotMsg = "not exist";
                string onlyDTmsg = "Valid Data: DT Exist";

                string onlyInvMsg = "Only Investor";
                string onlyINVCMMsg = "Only Investor and Client Master";
                string onlyFullMsg = "Account Exist";
                string INVALID_DT_DATA_MSG = "Invalid Data";

                if (message.Contains(DtNotMsg))
                {

                }
                else if (message.Contains(onlyDTmsg))
                {
                    ShowAlert(message);
                    string rccDTValue = GetTextFieldValue(row, "COMMON_ID");
                    string rccDOCValue = GetTextFieldValue(row, "DOC_ID");
                    string rccBusiValue = GetTextFieldValue(row, "business_code");
                    string rccRmValue = GetTextFieldValue(row, "rm_name");
                    string rccRmBranchValue = GetTextFieldValue(row, "BRANCH_CODE");
                    string rccCommondValue = GetTextFieldValue(row, "guest_code");

                    string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                    string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");

                    ResetFormFields1();
                    AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                    AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                    AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                    AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");

                    txtDTNumber.Text = !string.IsNullOrEmpty(rccDTValue) ? rccDTValue : string.Empty;
                    txtBusinessCode.Text = !string.IsNullOrEmpty(rccBusiValue) ? rccBusiValue : string.Empty;
                    //txtBusinessCodeName.Text = !string.IsNullOrEmpty(rccRmValue) ? rccRmValue : string.Empty;
                    //txtBusinessCodeBranch.Text = !string.IsNullOrEmpty(rccRmBranchValue) ? rccRmBranchValue : string.Empty;
                    UpdateBusinessCodeDetails(rccBussRMCodeValue);
                    txtGuestCode.Text = !string.IsNullOrEmpty(rccCommondValue) ? rccCommondValue : string.Empty;

                }
                
                else if (message.Contains(onlyInvMsg))
                {
                    string rccDTValue = GetTextFieldValue(row, "COMMON_ID");
                    string rccDOCValue = GetTextFieldValue(row, "DOC_ID");
                    string rccBusiValue = GetTextFieldValue(row, "business_code");
                    string rccRmValue = GetTextFieldValue(row, "rm_name");
                    string rccRmBranchValue = GetTextFieldValue(row, "BRANCH_CODE");
                    string rccCommondValue = GetTextFieldValue(row, "guest_code");

                    string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                    string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");


                    string rcvInvValue = GetTextFieldValue(row, "inv_code");


                    ResetFormFields1();
                    AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                    AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                    AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                    AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                    HandleSelectedInvestor(rcvInvValue);

                    txtDTNumber.Text = !string.IsNullOrEmpty(rccDTValue) ? rccDTValue : string.Empty;
                    //txtBusinessCode.Text = !string.IsNullOrEmpty(rccBusiValue) ? rccBusiValue : string.Empty;
                    //txtBusinessCodeName.Text = !string.IsNullOrEmpty(rccRmValue) ? rccRmValue : string.Empty;
                    //txtBusinessCodeBranch.Text = !string.IsNullOrEmpty(rccRmBranchValue) ? rccRmBranchValue : string.Empty;
                    UpdateBusinessCodeDetails(rccBussRMCodeValue);
                    txtGuestCode.Text = !string.IsNullOrEmpty(rccCommondValue) ? rccCommondValue : string.Empty;

                }


               
                else if (message.Contains(onlyFullMsg))
                {

                    string rccDTValue = GetTextFieldValue(row, "COMMON_ID");
                    string rccDOCValue = GetTextFieldValue(row, "DOC_ID");
                    string rccBusiValue = GetTextFieldValue(row, "business_code");
                    string rccRmValue = GetTextFieldValue(row, "rm_name");
                    string rccRmBranchValue = GetTextFieldValue(row, "BRANCH_CODE");
                    string rccCommondValue = GetTextFieldValue(row, "guest_code");

                    string rcvInvValue = GetTextFieldValue(row, "inv_code");
                    string rcvCmValue = GetTextFieldValue(row, "cm_client_code");
                    string rcvCtValue = GetTextFieldValue(row, "ct_client_code");

                    string rccBussRMCodeValue = GetTextFieldValue(row, "BUSI_RM_CODE");
                    string rccBussBranchCodeValue = GetTextFieldValue(row, "BUSI_BRANCH_CODE");


                    ResetFormFields1();
                    AutoSelectOrInsertDropdownItem(ddlTaxStatus, "Others");
                    AutoSelectOrInsertDropdownItem(ddlAOClientCategory, "Retail");
                    AutoSelectOrInsertDropdownItem(ddlAONationality, "Indian");
                    AutoSelectOrInsertDropdownItem(ddlAOResidentNRI, "Resident");
                    if (rcvCtValue.Contains("AH"))
                    {
                        FillClientDataByAHNum(rcvCtValue);
                    }

                    txtDTNumber.Text = !string.IsNullOrEmpty(rccDTValue) ? rccDTValue : string.Empty;
                    //txtBusinessCode.Text = !string.IsNullOrEmpty(rccBusiValue) ? rccBusiValue : string.Empty;
                    //txtBusinessCodeName.Text = !string.IsNullOrEmpty(rccRmValue) ? rccRmValue : string.Empty;
                    //txtBusinessCodeBranch.Text = !string.IsNullOrEmpty(rccRmBranchValue) ? rccRmBranchValue : string.Empty;
                    UpdateBusinessCodeDetails(rccBussRMCodeValue);
                    txtGuestCode.Text = !string.IsNullOrEmpty(rccCommondValue) ? rccCommondValue : string.Empty;
                    
                }


                else
                {

                    ShowAlert(message);
                    txtDTNumber.Text = id;
                    txtDTNumber.Focus();
                }

            }
        }
        catch (Exception ex)
        {
            // Consider logging the exception or showing an error message for better error handling
            // Example: Console.WriteLine(ex.Message);
        }

    }
