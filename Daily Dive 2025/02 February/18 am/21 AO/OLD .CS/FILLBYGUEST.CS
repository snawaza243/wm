    private void FillClientByGuestCode(string guestCode, string login)
    {
        try
        {
            DataTable dt = new AccountOpeningController().GetClientDetailsByGuest(guestCode, login);
            int dtRowCount = dt.Rows.Count;

            if (dtRowCount > 0)
            {
                DataRow row = dt.Rows[0]; 

                /*
        D.GUEST_CD          AS C1_GUEST_CODE,
        D.GUEST_NAME          AS C1_GUEST_NAME,
        D.MOBILE              AS C1_MOBILE_NO,
        D.EMP_NO              AS C1_EMP_NO,
        D.TELEPHONE         AS C1_TELEPHONE,
        B.SEX               AS C2_GUEST_SEX,
        B.MATRIALST         AS C2_GUEST_MATRIALST,
        CASE 
        WHEN UPPER(B.SEX) = UPPER('Male') THEN 'MR.'
        WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.'
        ELSE 'Mrs.'
        END AS TITLE,
        PITCH_BOOK_NO       AS C2_PITCH_NO,
        RESIADD1            AS C2_ADD1,
        RESIADD2            AS C2_ADD2,
        CITY                AS C2_CITY,
        cm.city_id          AS C2_CITY_ID,
        STATE               AS C2_STATE,
        sm.state_id         AS C2_STATE_CODE,
        sm.country_id       AS C2_COUNTRY_ID,
        RESIPINCODE         AS C2_PIN,
        B.DOB               AS C2_DOB,
        CORESSEMAIL         AS C2_EMAIL
                 */
                string c1Message = GetTextFieldValue(row, "C1_MESSAGE").ToUpper();

                string validateMsg = "VALID DATA IN C1".ToUpper();
                string validAlertMsg = "Guest code is valid".ToUpper();
                string usedCodeMsg = "Guest Code is Duplicate".ToUpper();
                string invalidCodeMsg = "This is not a valid Guest Code".ToUpper();

                if (c1Message.Contains(usedCodeMsg) || c1Message.Contains(invalidCodeMsg))
                {
                    ShowAlert(c1Message);
                    txtGuestCode.Text = guestCode;

                    RESET_GUEST_VALUE();
                   
                    return;
                }

                
                string c1EmpNo = GetTextFieldValue(row, "C1_EMP_NO"); 
                const string noBusinessCodeMsg = "Guest code is valid, but business code does not exist.";

                // Check the message to validate if data is correct
                if (c1Message.Contains(validateMsg.ToUpper()) && !string.IsNullOrEmpty(c1EmpNo))
                {
                    SET_FIELD_DATA_GUEST(row, c1EmpNo, guestCode, validAlertMsg);
                }
               
                else
                {
                    if (c1Message.Contains(usedCodeMsg.ToUpper()))
                    {
                        ShowAlert(usedCodeMsg);
                        ClearBusinessCodeFields();
                        PopulateDefaultDropdownValues();
                    }

                    if (c1Message.Contains(invalidCodeMsg.ToUpper()))
                    {
                        ShowAlert(invalidCodeMsg);
                        ClearBusinessCodeFields();
                        PopulateDefaultDropdownValues();
                    }

                }
            }
            else
            {
                // No data found for the given guest code
                ResetFormFields1();
                const string noDataMsg = "No Data!";
                ShowAlert(noDataMsg);

                txtGuestCode.Text = guestCode;
                txtGuestCode.Focus();
                lblHolderMessage.Text = noDataMsg;
            }


        }
        catch (Exception ex)
        {
            // Catch and display any errors that occur
            string alertMsg  = $"An error occurred while retrieving data: {ex.Message}";
            ShowAlert(alertMsg);
        }
    }
