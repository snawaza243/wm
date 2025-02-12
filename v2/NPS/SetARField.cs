
        private void SetARFieldData(DataRow row)
        {
            string InvTypeValue = GetTextFieldValue(row, "CORPORATE_NAME");
            
            //SelectValueInDropdown(ddlProductClass, row["ISS_CODE"].ToString());
            SelectValueInDropdown(ddlType, row["INVESTOR_TYPE"].ToString());

            corporateName.Text = GetTextFieldValue(row, "CORPORATE_NAME");
            txtDtNumber.Text = GetTextFieldValue(row, "DOC_ID");
            lblArNo.Text = GetTextFieldValue(row, "TRAN_CODE");
            txtInvestorCode.Text = GetTextFieldValue(row, "CLIENT_CODE");
            SelectValueInDropdown(ddlScheme, row["SCH_CODE"].ToString());
            // ddlCra.SelectedValue = GetTextFieldValue(row, "TRAN_CODE");
            // SelectValueInDropdown(ddlCraBranch, row["BRANCH_CODE"].ToString());




            txtPopSpRegNo.Text = GetTextFieldValue(row, "FOLIO_NO");
            txtBusinessRm.Text = GetTextFieldValue(row, "BUSINESS_RMCODE");
            SelectValueInDropdown(ddlBusinessBranch, row["BUSI_BRANCH_CODE"].ToString());
            txtReceiptNo.Text = (row["UNIQUE_ID"].ToString());

            SelectValueInDropdown(ddlPaymentMethod, row["PAYMENT_MODE"].ToString());
            txtChequeNo.Text = GetTextFieldValue(row, "CHEQUE_NO");

            DateTime? chequeDate = row["CHEQUE_DATE"] != DBNull.Value ? (DateTime?)row["CHEQUE_DATE"] : null;

            if (chequeDate.HasValue)
            {
                // Set the date part in the desired format
                txtChequeDated.Text = chequeDate.Value.ToString("dd/MM/yyyy"); // Adjust the format as needed
            }
            else
            {
                // Handle the case when CHEQUE_DATE is null or empty
                txtChequeDated.Text = string.Empty;
            }

            if (row["BANK_NAME"] != DBNull.Value && !string.IsNullOrEmpty(row["BANK_NAME"].ToString()))
            {
                string bankName = row["BANK_NAME"].ToString();

                // Check if the value exists in the dropdown list
                if (ddlBankName.Items.FindByValue(bankName) != null)
                {
                    ddlBankName.SelectedValue = bankName; // Set the selected value
                }
                else
                {
                    // Add the new item and set it as the selected item
                    ddlBankName.Items.Add(new ListItem(bankName, bankName));
                    ddlBankName.SelectedValue = bankName;
                }
            }
            else
            {
                // Handle cases where BANK_NAME is null or empty by deselecting any item
                ddlBankName.SelectedIndex = -1;
            }

            SelectValueInDropdown(ddlRequestType, row["APP_NO"].ToString());
            DateTime? trDateTime = row["TR_DATE"] != DBNull.Value ? (DateTime?)row["TR_DATE"] : null;

            if (trDateTime.HasValue)
            {
                // Set the date part
                txtDate.Text = trDateTime.Value.ToString("dd/MM/yyyy"); // Adjust the format as needed

                // Set the time part
                txtTime.Text = trDateTime.Value.ToString("HH:mm"); // Adjust the format as needed
            }
            else
            {
                // Handle the case when TR_DATE is null or empty
                txtDate.Text = string.Empty;
                txtTime.Text = string.Empty;
            }
            txtNameOfSubscriber.Text = GetTextFieldValue(row, "TS_INV_NAME") == null ? GetTextFieldValue(row, "DC_INV_NAME") : GetTextFieldValue(row, "TS_INV_NAME");
            txtPran.Text = GetTextFieldValue(row, "manual_arno");
            //CheckBox1.Checked = row[""].ToString().Equals(); // Unfreez


            txtAmountReceivedTire1.Text = GetTextFieldValue(row, "NT_AMOUNT1");
            txtAmountReceivedTire2.Text = GetTextFieldValue(row, "NT_AMOUNT2");
            txtPopRegistrationChargesOneTime.Text = GetTextFieldValue(row, "NT_REG_CHARGE");
            txtPopRegistrationCharges.Text = GetTextFieldValue(row, "NT_Tran_CHARGE");
            txtGst.Text = GetTextFieldValue(row, "NT_SERVICETAX");
            txtCollectionAmount.Text = GetTextFieldValue(row, "");
            txtAmountInvested.Text = GetTextFieldValue(row, "AMOUNT");

            txtRemark.Text = GetTextFieldValue(row, "REMARK");

            lblMessage.Text = "Found Data Filled";


        }
