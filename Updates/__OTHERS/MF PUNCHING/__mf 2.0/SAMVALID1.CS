   public (bool isValid, string errorMessage) SamValidationSave()
   {
       string CmbSipStpA = ddlSipStp.Text.ToString();      // REGULAR, SIP, STP
       string CmbSubSipA = siptype.Text.ToString();        // NORMAL, MICROSIP
       string Cmbsubinv = iNSTYPE.SelectedValue.ToString();    // MICRO, NORMAL

       string FreshRenA = fresh.Checked ? fresh.Text.ToUpper() : renewal.Text.ToUpper();       // Fresh, Renewal
       string SchCode1A = transactionType.SelectedValue.ToString();                            // PURCHASE, SWITCH IN

       string TxtPanA = txtPanADD.Text.Length >= 10 ? txtPanADD.Text.Substring(0, 10).ToUpper() : txtPanADD.Text.ToUpper(); // Pan from address popup that opened after dt entered
       string TxtPanVarify = pann.Text.Length >= 10 ? pann.Text.Substring(0, 10).ToUpper() : pann.Text.ToUpper();

       string TxtAmountA = amountt.Text.ToString();
       string TxtSchemeA = scheme.ToString();
       string TxtCloseSch = txtSearchSchemeDetails.ToString();
       string TxtSwitchSchemeA = formSwitchScheme.ToString();

       string MySchCode = SplitSchemeFromDoubleColon(TxtSchemeA, true);
       string MyCloseSchCode = SplitSchemeFromDoubleColon(TxtCloseSch, true);
       string MySwitchScheme = SplitSchemeFromDoubleColon(TxtSwitchSchemeA, true);

       string Label32 = invcode.Text.ToString();
       int micropanflag = 0;

       if (!string.IsNullOrWhiteSpace(TxtAmountA))
       {
           if (Cmbsubinv == "MICRO" && decimal.TryParse(TxtAmountA, out decimal amt) && amt < 50000)
           {
               micropanflag = 1;
           }
       }

       if ((CmbSipStpA == "SIP" && CmbSubSipA == "MICROSIP"))
       { 

       }
       // If not SIP+MICROSIP, validate PAN
       else { 
           if (MySchCode != "OP#10826")
           {
               if (micropanflag == 0)
               {
                   if (string.IsNullOrWhiteSpace(TxtPanVarify) || !pc.ValidatePan(TxtPanA))
                   {
                       if (!pc.ValidatePan(TxtPanVarify))
                       {
                           pann.Focus();
                           return (false, "Please Either Enter a Valid PAN Number");
                       }
                   }
               } 
               // Minor check for codes starting with 3
               if (!string.IsNullOrWhiteSpace(Label32) && Label32.StartsWith("3"))
               {
                   DateTime dob = PsmController.SqlRetDate($"SELECT NVL(dob, SYSDATE - 10000) FROM INVESTOR_MASTER WHERE INV_CODE='{Label32}'");
                   bool IsMinor = dob != DateTime.MinValue && (DateTime.Now.Year - dob.Year) <= 18;

                   if (!IsMinor && micropanflag == 0)
                   {
                       string PAN1 = PsmController.SqlRetString($"SELECT UPPER(PAN) FROM investor_master WHERE INV_CODE='{Label32}'");
                       if (!string.IsNullOrWhiteSpace(PAN1) && PAN1 != "0")
                       {
                           if (PAN1 != TxtPanVarify.ToUpper())
                           {
                               return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                           }
                       }
                   }
               }

               // Minor check for codes starting with 4
               if (!string.IsNullOrWhiteSpace(Label32) && Label32.StartsWith("4"))
               {
                   DateTime dob = PsmController.SqlRetDate($"SELECT NVL(dob, TO_DATE('01/01/1800','MM/DD/YYYY')) FROM client_test WHERE client_codekyc='{Label32}'");
                   if (dob == DateTime.Parse("01/01/1800"))
                   {
                       return (false, "Please fill Date of Birth for this investor");
                   }

                   string category = PsmController.SqlRetString($"SELECT NVL(investor_code,1) FROM client_master WHERE client_code='{Label32.Substring(0, 8)}'");
                   bool IsMinor = (DateTime.Now.Year - dob.Year) <= 18 && category == "1";

                   if (!IsMinor && micropanflag == 0)
                   {
                       string inv_cd = PsmController.SqlRetString($"SELECT client_codekyc FROM client_test WHERE upper(CLIENT_PAN)='{TxtPanVarify}'");
                       if (!string.IsNullOrWhiteSpace(inv_cd) && inv_cd != "0" && inv_cd != Label32)
                       {
                           return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                       }

                       int AccountPan = PsmController.SqlRetInt($"SELECT NVL(COUNT(*),0) FROM client_test WHERE client_codekyc='{Label32}' AND (upper(CLIENT_PAN)='{TxtPanVarify}' OR CLIENT_PAN IS NULL)");
                       if (AccountPan == 0)
                       {
                           return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                       }
                   }
                   else if (IsMinor && micropanflag == 0)
                   {
                       string PAN1 = PsmController.SqlRetString($"SELECT NVL(upper(g_pan),0) FROM client_test WHERE client_codekyc='{Label32}'");
                       if (!string.IsNullOrWhiteSpace(PAN1) && PAN1 != "0")
                       {
                           if (PAN1 != TxtPanVarify)
                           {
                               return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                           }
                       }

                       int AccountPan = PsmController.SqlRetInt($"SELECT NVL(COUNT(*),0) FROM client_test WHERE client_codekyc='{Label32}' AND (upper(G_PAN)='{TxtPanVarify}' OR G_PAN IS NULL)");
                       if (AccountPan == 0)
                       {
                           return (false, "PAN Entered Did Not Match With The Selected Investor's PAN");
                       }
                   }
               }

               if (string.IsNullOrWhiteSpace(TxtPanA) || !pc.ValidatePan(TxtPanA))
               {
                   txtPanADD.Text = TxtPanVarify;
               }
           }
       }

       // Extra validation
       if (CmbSubSipA == "MICROSIP" && string.IsNullOrWhiteSpace(TxtPanVarify))
       {
           return (false, "Please Provide Pan Number");
       }

       return (true, string.Empty);
   }
