  protected void btnAddClick(object sender, EventArgs e)
  {
      try
      {

          MfPunchingController mfPunchingController = new MfPunchingController();

          string login = pc.currentLoginID();
          string role = pc.currentRoleID();
          string invCode = string.Empty;
          string trCode = string.Empty;
          string panc = string.Empty;

          var mfValidation = mfPunchingController.MF_Validation(login, role, invCode, trCode, panc);

          if (!mfValidation.isValid)
          {
              // Show alert if validation fails
              pc.ShowAlert(this, mfValidation.Message);
              return;
          }


      }
      catch (Exception ex)
      {
          // Handle any exceptions that occur during the button click
          pc.ShowAlert(this, "An error occurred: " + ex.Message);
          return;
      }


      MfPunchingController controller = new MfPunchingController();

      string loggedInUser = Session["LoginId"]?.ToString();
      string roleId = Session["roleId"]?.ToString();

      #region Validations
      if (roleId != "212")
      {
          pc.ShowAlert(this, "You are not authorized to punch the transaction");
          return;
      }

      // Validate required fields
      // Assuming 'pann' is a TextBox control
      if (string.IsNullOrWhiteSpace(dtNumberA.Text))
      {
          lblWarning.Text = "Please Select Investor to add";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }


      /* old pan validation 
      string checkinstype = iNSTYPE.Text.ToString();
      if (string.IsNullOrWhiteSpace(pann.Text)  && (checkinstype != "MICROSIP" && checkinstype != "MICRO"))
      {
          lblWarning.Text = "Please Provide Pan Number";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }

       string a_Pan1 = pann.Text.Substring(0, Math.Min(10, pann.Text.Length));
       string a_Pan2 = txtPanADD.Text.Substring(0, Math.Min(10, txtPanADD.Text.Length));


      if (a_Pan1.ToUpper() != a_Pan2.ToUpper() && checkinstype != "MICRO" && checkinstype != "MICROSIP")
      {
          lblWarning.Text = "Pan Number does not match Investor's Pan No.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }
      */


      var (isTrue, errorMsg) = SamValidationSave();
      if (!isTrue)
      {
          pc.ShowAlert(this, errorMsg);
          return;
           
      }


      #region Validaion 2


      if (string.IsNullOrWhiteSpace(amc.SelectedValue))
      {
          lblWarning.Text = "Please select an AMC.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }

      // Validate Scheme TextBox
      if (string.IsNullOrWhiteSpace(scheme.Text))
      {
          lblWarning.Text = "Please provide a Scheme.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }

      if (string.IsNullOrWhiteSpace(amountt.Text))
      {
          lblWarning.Text = "Please provide an Amount.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }

      if (!string.IsNullOrWhiteSpace(applicationNo.Text))
      {
          if (applicationNo.Text.Length < 6)
          {
              lblWarning.Text = "Minimum Length Of App No Should Be Greater or Equal To 6.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }
          else if (applicationNo.Text == "000000")
          {
              lblWarning.Text = "Please Enter A Valid App No.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }
      }

      // Validate Business Code
      if (string.IsNullOrWhiteSpace(businessCode.Text))
      {
          lblWarning.Text = "Business Code Cannot Be Left Blank.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }

      // Validate Investor Name
      if (string.IsNullOrWhiteSpace(accountHolder.Text))
      {
          lblWarning.Text = "Please Fill Investor Name.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }

      // Validate Client Code
      /*
      if (holderCode.Text.Length < 8)
      {
          lblWarning.Text = "Client Code Cannot Be Left Blank.";
          lblWarning.Visible = true;
          lblWarning.Focus();
          return;
      }*/

      // Validate ddlSipStp selection
      if (transactionType.SelectedValue == "PURCHASE")
      {
          if (string.IsNullOrWhiteSpace(ddlSipStp.SelectedValue))
          {
              lblWarning.Text = "Please select a value from SIP/STP.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }
      }

      // Check if SIP is selected
      if (ddlSipStp.SelectedValue == "SIP" && transactionType.SelectedValue == "PURCHASE")
      {
          // Validate SIP-specific fields
          if (string.IsNullOrWhiteSpace(txtSIPStartDate.Text))
          {
              lblWarning.Text = "Please provide a SIP Start Date.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

          if (string.IsNullOrWhiteSpace(txtSIPEndDate.Text))
          {
              lblWarning.Text = "Please provide a SIP End Date.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

          if (string.IsNullOrWhiteSpace(ddlFrequency.SelectedValue))
          {
              lblWarning.Text = "Please select a Frequency.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

          if (string.IsNullOrWhiteSpace(txtInstallmentsNos.Text))
          {
              lblWarning.Text = "Please provide the number of Installments.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

          if (string.IsNullOrWhiteSpace(sipamount.Text))
          {
              lblWarning.Text = "Please provide a SIP Amount.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }
      }

      // Check if STP is selected
      if (ddlSipStp.SelectedValue == "STP")
      {
          //   transactionType.SelectedValue = "SWITCH IN";
          // Validate STP-specific fields
          if (string.IsNullOrWhiteSpace(ddlFrequency.SelectedValue))
          {
              lblWarning.Text = "Please select a Frequency.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

          if (string.IsNullOrWhiteSpace(txtInstallmentsNos.Text))
          {
              lblWarning.Text = "Please provide the number of Installments.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

          if (string.IsNullOrWhiteSpace(txtSIPEndDate.Text))
          {
              lblWarning.Text = "Please provide a SIP End Date.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }

         
          if (ddlSipStp.SelectedValue == "STP")
          {
              if (string.IsNullOrWhiteSpace(formSwitchScheme.Text))
              {
                  lblWarning.Text = "Please provide a Form Switch/STP Scheme.";
                  lblWarning.Visible = true;
                  lblWarning.Focus();
                  return;
              }
          }


      }


      #endregion

      #region Validation 3
      // Validate payment method fields
      string selectedPaymentMethod = null;
      string paymentMode = null;
      string chequeNo = null;
      DateTime? chequeDate = null;

      if (cheque.Checked)
      {
          selectedPaymentMethod = "cheque";
          paymentMode = "C";
          chequeNo = txtChequeNo.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtChequeDated.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtChequeDated.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }
      else if (draft.Checked)
      {
          selectedPaymentMethod = "draft";
          paymentMode = "D";
          chequeNo = txtDraftNo.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtDraftDate.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtDraftDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }
      else if (ecs.Checked)
      {
          selectedPaymentMethod = "ecs";
          paymentMode = "E";
          chequeNo = txtEcsReference.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtEcsDate.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtEcsDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }
      else if (cash.Checked)
      {
          selectedPaymentMethod = "cash";
          paymentMode = "H";
          chequeNo = txtCashAmount.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtCashDate.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtCashDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }
      else if (others.Checked)
      {
          selectedPaymentMethod = "others";
          paymentMode = "R";
          chequeNo = txtOthersReference.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtOthersDate.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtOthersDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }
      else if (rtgs.Checked)
      {
          selectedPaymentMethod = "rtgs";
          paymentMode = "U";
          chequeNo = txtRtgsNo.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtRtgsDate.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtRtgsDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }
      else if (neft.Checked)
      {
          selectedPaymentMethod = "neft";
          paymentMode = "B";
          chequeNo = txtNeftNo.Text;
          chequeDate = string.IsNullOrWhiteSpace(txtNeftDate.Text)
              ? (DateTime?)null
              : DateTime.ParseExact(txtNeftDate.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
      }


      if (transactionType.SelectedValue == "PURCHASE")
      {
          if (string.IsNullOrEmpty(selectedPaymentMethod))
          {
              lblWarning.Text = "Please select a payment method.";
              lblWarning.Visible = true;
              lblWarning.Focus();
              return;
          }


          switch (selectedPaymentMethod)
          {
              case "cheque":
                  if (string.IsNullOrWhiteSpace(txtChequeNo.Text) || string.IsNullOrWhiteSpace(txtChequeDated.Text))
                  {
                      lblWarning.Text = "Please fill in all cheque details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              case "draft":
                  if (string.IsNullOrWhiteSpace(txtDraftNo.Text) || string.IsNullOrWhiteSpace(txtDraftDate.Text))
                  {
                      lblWarning.Text = "Please fill in all draft details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              case "rtgs":
                  if (string.IsNullOrWhiteSpace(txtRtgsNo.Text) || string.IsNullOrWhiteSpace(txtRtgsDate.Text))
                  {
                      lblWarning.Text = "Please fill in all RTGS details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              case "neft":
                  if (string.IsNullOrWhiteSpace(txtNeftNo.Text) || string.IsNullOrWhiteSpace(txtNeftDate.Text))
                  {
                      lblWarning.Text = "Please fill in all NEFT details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              case "ecs":
                  if (string.IsNullOrWhiteSpace(txtEcsReference.Text) || string.IsNullOrWhiteSpace(txtEcsDate.Text))
                  {
                      lblWarning.Text = "Please fill in all ECS details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              case "cash":
                  if (string.IsNullOrWhiteSpace(txtCashAmount.Text) || string.IsNullOrWhiteSpace(txtCashDate.Text))
                  {
                      lblWarning.Text = "Please fill in all cash payment details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              case "others":
                  if (string.IsNullOrWhiteSpace(txtOthersReference.Text) || string.IsNullOrWhiteSpace(txtOthersDate.Text))
                  {
                      lblWarning.Text = "Please fill in all other payment details.";
                      lblWarning.Visible = true;
                      lblWarning.Focus();
                      return;
                  }
                  break;

              default:
                  lblWarning.Text = "Invalid payment method selected.";
                  lblWarning.Visible = true;
                  lblWarning.Focus();
                  return;
          }
      }

      #endregion

      #region Getting Values

      // Handle decimal values with null checks
      decimal sipAmount = string.IsNullOrWhiteSpace(sipamount.Text) ? 0 : decimal.Parse(sipamount.Text);
      string clientCode = invcode.Text;
      string businessRmCode = businessCode.Text;
      string clientOwner = businessCode.Text;
      string busiBranchCode = branch.Text;
      string panno = pann.Text;
      string mutCode = amc.SelectedValue;
      string schCode = scheme.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

      // Parse DateTime values with null checks
      DateTime trDate = string.IsNullOrWhiteSpace(transactionDate.Text)
 ? DateTime.Now
 : DateTime.TryParseExact(transactionDate.Text, "dd/MM/yyyy",
     System.Globalization.CultureInfo.InvariantCulture,
     System.Globalization.DateTimeStyles.None, out var parsedTrDate)
 ? parsedTrDate
 : DateTime.Now; // Default to now if parsing fails

      string tranType = transactionType.SelectedValue;
      string appNo = applicationNo.Text;

      DateTime sipStartDate = string.IsNullOrWhiteSpace(txtSIPStartDate.Text)
? DateTime.Now
: DateTime.TryParseExact(txtSIPStartDate.Text, "dd/MM/yyyy",
    System.Globalization.CultureInfo.InvariantCulture,
    System.Globalization.DateTimeStyles.None, out var parsedSipStartDate)
? parsedSipStartDate
: DateTime.Now; // Default to now if parsing fails

      string pan = pann.Text;
      string folioNo = folioNoo.Text;
      string switchFolio = formSwitchFolio.Text;

      string switchScheme;
      if (string.IsNullOrWhiteSpace(formSwitchScheme.Text))
      {
          switchScheme = null;
      }
      else
      {
          switchScheme = formSwitchScheme.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

      }

      string bankName = ddlBankName.SelectedItem.ToString();


      // Handle cheque date with null checks
      decimal amount = string.IsNullOrWhiteSpace(amountt.Text) ? 0 : decimal.Parse(amountt.Text);


      string sipType = ddlSipStp.SelectedValue;

      //  string leadName = txtLeadName.Text;
      string sourceCode = invcode.Text.Length >= 8 ? invcode.Text.Substring(0, 8) : invcode.Text;

      string investorName = accountHolder.Text;

      decimal expRate = string.IsNullOrWhiteSpace(txtExpensesPercent.Text) ? 0 : decimal.Parse(txtExpensesPercent.Text);
      decimal expAmount = string.IsNullOrWhiteSpace(txtExpensesRs.Text) ? 0 : decimal.Parse(txtExpensesRs.Text);
      string acHolderCode = holderCode.Text;
      string frequency = ddlFrequency.SelectedValue;

      int installmentsNo = string.IsNullOrWhiteSpace(txtInstallmentsNos.Text) ? 0 : int.Parse(txtInstallmentsNos.Text);

      DateTime timestamp = DateTime.Now; // Current timestamp

      DateTime? sipEndDate = string.IsNullOrWhiteSpace(txtSIPEndDate.Text)
    ? (DateTime?)null
    : DateTime.TryParseExact(txtSIPEndDate.Text, "dd/MM/yyyy",
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None, out DateTime parsedDate)
    ? parsedDate
    : (DateTime?)null;

      string sipFr = null;
      if (ddlSipStp.SelectedValue == "SIP")
      {

          if (fresh.Checked)
          {
              sipFr = "F";
          }
          else if (renewal.Checked)
          {
              sipFr = "R";
          }
      }

      string microflag = null;

      if (ddlSipStp.SelectedValue == "SIP")
      {
          microflag = siptype.SelectedValue;
      }

      string dispatch = null;

      if (regular.Checked)
          dispatch = "R";
      else if (nfo.Checked)
          dispatch = "N";

      string docId = dtNumberA.Text;

      string microInvestment = null;

      microInvestment = iNSTYPE.SelectedValue;


      string cobFlag = chkCOBCase.Checked ? "1" : "0";
      string swpFlag = chkSWPCase.Checked ? "1" : "0";
      string freedomSipFlag = chkFreedomSIP.Checked ? "1" : "0";

      string targetSwitchScheme;
      if (string.IsNullOrWhiteSpace(txtSearchSchemeDetails.Text))
      {
          targetSwitchScheme = null;
      }
      else
      {
          targetSwitchScheme = txtSearchSchemeDetails.Text.Split(new string[] { ";;" }, StringSplitOptions.None)[1];

      }

      #endregion

      // Check if duplicate transactions exist
      bool hasDuplicate = CheckForDuplicateTransactions(clientCode, schCode, amount);

      if (hasDuplicate)
      {
          // Load duplicates and show popup
          LoadDuplicateTransactions(clientCode, schCode, amount, popupDuplicate, gvDuplicateTransactions);

          // Set the hidden field to 0 (waiting for user input)
          hfContinueTransaction.Value = "0";

          return; // Stop execution until user selects an option
      }

      #endregion
      Save_Method(); // btnAddClick

  }