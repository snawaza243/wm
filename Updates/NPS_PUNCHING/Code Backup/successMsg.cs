 #region AR, DT fields
 //string currentAR = lblArNo.Text.ToString();
 //string currentDT = txtDtNumber.Text.ToString();
 //if (!string.IsNullOrEmpty(currentAR) || currentAR != "0"){}
 #endregion
 #region alert message and reload for insert/update
 // Reset the form fields after successful insert/update
 /*
 if (insertResult.Contains("Updation successful"))
 {
     pc.ShowAlert(this, insertResult);
     PsmController pc3 = new PsmController();
     pc3.ReloadCurrentPage(this);
     return;



 }

 else if (insertResult.Contains("Insertion successful"))
 {
     pc.ShowAlert(this, insertResult);
     PsmController pc3 = new PsmController();
     pc3.ReloadCurrentPage(this);
     return;
 }

 */
 // relaod with alert
 /*
  * 
    string scriptReloadPage = $@"
                 alert('{insertResult}');
                 window.location.href = window.location.href;
                 ";
 ResetMain();

 // re-load the updated transaction

 if (!string.IsNullOrEmpty(currentAR))
 {
     DataTable dt = new NpsTransactionPunchingController().GET_AR_BY_DTTS(null, currentAR, false, Session["LoginId"]?.ToString());
     if (dt.Rows.Count > 0)
     {
         DataRow row = dt.Rows[0];
         SetFieldData(row);
         pc.ShowAlert(this, insertResult);
         lblMessage.Text = insertResult;
         lblMessage.Focus();
         string currentDT1 = currentDT;
     }
 } 
 */

 #endregion
