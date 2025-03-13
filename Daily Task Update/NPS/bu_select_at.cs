    protected void gvARSearch_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "SelectRow")
        {
            // Retrieve the TRAN_CODE from CommandArgument
            string tranCode = e.CommandArgument.ToString();


            try
            {

                bool beforeAfterFlags = false;

                beforeAfterFlags = rbtDepositBefore.Checked ? true : false;

                if (!string.IsNullOrEmpty(tranCode))
                {

                    DataTable dtIfArExistForTempTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, true, Session["LoginId"]?.ToString());
                    DataTable dtIfArExistForStTran = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, tranCode, false, Session["LoginId"]?.ToString());

                    int temDataCount = dtIfArExistForTempTran.Rows.Count > 0 ? 1 : 0;
                    int dtCount3 = dtIfArExistForStTran.Rows.Count > 0 ? 1 : 0;


                    // Check for AR in temp transaction table
                    if (temDataCount == 0) // No data in temp
                    {
                        if (dtCount3 == 1) // Data exists in st
                        {
                            string dbMsg1 = dtIfArExistForStTran.Rows[0]["message"].ToString(); // to upper exist in st"
                            if (dbMsg1.Contains("Validity: Transaction data exist in st"))
                            {
                                DataRow row3 = dtIfArExistForStTran.Rows[0];

                                //SetFieldData(row3);
                                ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);
                                //arPopReset();
                                //btnSave.Enabled = false;
                                //btnModify.Enabled = true;
                                //txtBusinessRm.Enabled = false;


                            }
                        }
                        else
                        {

                            pc.ShowAlert(this, "No data exist!");

                        }
                    }
                    else
                    {
                        // Data exists in temp, extract values as needed
                        string dbMsg1 = dtIfArExistForTempTran.Rows[0]["message"].ToString();
                        if (dbMsg1.Contains("Validity: Transaction data exist in temp"))
                        {
                            DataRow row3 = dtIfArExistForTempTran.Rows[0];
                            //ShowAlert(dbMsg1);
                            //SetFieldData(row3);
                            ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSearchArModel", "closeModalSearchArModel();", true);
                            //arPopReset();
                            //btnSave.Enabled = false;
                            //btnModify.Enabled = true;
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                //btnSave.Enabled = true;
                //btnModify.Enabled = false;

            }



        }
    }
