CREATE OR REPLACE PROCEDURE PSM_MFPI_VALID(
P_LOGIN             IN VARCHAR2, 
P_ROLE              IN VARCHAR2, 
P_INV_CODE          IN VARCHAR2,
P_TR_CODE           IN VARCHAR2, 
P_PAN               IN VARCHAR2, -- pann.Text
P_DT_NUMBER         IN VARCHAR2, -- dtNumberA.Text
P_CmbSipStpA        IN VARCHAR2, --  ddlSipStp.Text.ToString();      // REGULAR, SIP, STP
P_CmbSubSipA        IN VARCHAR2, --  siptype.Text.ToString();        // NORMAL, MICROSIP
P_Cmbsubinv         IN VARCHAR2, --  iNSTYPE.SelectedValue.ToString();    // MICRO, NORMAL
P_FreshRenA         IN VARCHAR2, --  fresh.Checked ? fresh.Text.ToUpper() : renewal.Text.ToUpper();       // Fresh, Renewal
P_TxtCloseSch       IN VARCHAR2, --  txtSearchSchemeDetails.ToString();
P_SCHEME_A          IN VARCHAR2, --  TxtSchemeA.Text.ToString(); // scheme.Text sche_name;;sche_code
P_MySchCode         IN VARCHAR2, --  SplitSchemeFromDoubleColon(TxtSchemeA, true);
P_MyCloseSchCode    IN VARCHAR2, --  SplitSchemeFromDoubleColon(TxtCloseSch, true);
P_MySwitchScheme    IN VARCHAR2, --  SplitSchemeFromDoubleColon(TxtSwitchSchemeA, true);
P_txtdocID          IN VARCHAR2, --  dtNumberA.Text.ToString();
P_ImSipEndDtA       IN VARCHAR2, --  txtSIPEndDate.Text.ToString();
P_Label32           IN VARCHAR2, -- = invcode.Text.ToString();
P_AMC               IN VARCHAR2, -- amc.SelectedValue MUTE CODE 
P_SCHEME            IN VARCHAR2,  -- scheme.Text sche_name;;sche_code
P_SCHEME_CODE       IN VARCHAR2,  -- scheme.Text split with ;; and take second value 
P_AMOUNT            IN VARCHAR2, -- amountt.Text
P_APP               IN VARCHAR2, -- applicationNo.Text
P_BSS_CODE          IN VARCHAR2, -- businessCode.Text
P_AH_NAME           IN VARCHAR2, -- accountHolder.Text
P_AH_CODE           IN VARCHAR2, -- holderCode.Text
P_TRAN_TYPE         IN VARCHAR2, -- transactionType.SelectedValue
P_SIP_STP           IN VARCHAR2, -- ddlSipStp.SelectedValue
P_SIP_START_DT      IN VARCHAR2, -- txtSIPStartDate.Text
P_SIP_END_DT        IN VARCHAR2, -- txtSIPEndDate
P_FREQ              IN VARCHAR2, -- ddlFrequency.SelectedValue
P_INSTALL_NO        IN VARCHAR2, -- txtInstallmentsNos.Text
P_SIP_AMOUNT        IN VARCHAR2, -- sipamount.Text
P_FROM_SWICTH_SCH   IN VARCHAR2, -- formSwitchScheme.Text sche_name;;sche_code
P_FROM_SWITCH_FOLIO IN VARCHAR2, -- formSwitchFolio.Text
P_PAYMENT_MODE      IN VARCHAR2, --  rtbPaymentA.SelectedValue
P_CHEQUE_NO         IN VARCHAR2, -- txtChequeNo.Text
P_CHEQUE_DT         IN VARCHAR2, -- txtChequeDated.Text
P_BRANCH_NAME       IN VARCHAR2,-- branch.Text
P_TR_DT             IN VARCHAR2,-- transactionDate.Text
P_FOLIO_NO          IN VARCHAR2, -- = folioNoo.Text;
P_BANK_NAME         IN VARCHAR2, -- ddlBankName.SelectedItem
P_EXP_PER           IN VARCHAR2, --txtExpensesPercent.Text
P_EXP_AMT           IN VARCHAR2, -- txtExpensesRs.Text
P_REGU_NFO          IN VARCHAR2, -- rdblRegNfo.SelectedValue
P_COB_FLAG          IN VARCHAR2, -- chkCOBCase.Checked ? "1" : "0";
P_SWP_FLAG          IN VARCHAR2, -- chkSWPCase.Checked ? "1" : "0";
P_FREEDOM_SIP_FLAG  IN VARCHAR2, -- chkFreedomSIP.Checked ? "1" : "0";
P_99_YEAR           IN VARCHAR2, -- rdo99Years.Checked ? "1" : "0";

P_CURSOR        OUT SYS_REFCURSOR) 
AS 

V_IS_VALID      NUMBER := 0;
V_STATUS        VARCHAR2(5);
V_MESSAGE       VARCHAR2(200);
V_VALID_ROLES   VARCHAR2(100);

BEGIN


    V_MESSAGE := V_MESSAGE || ' P_LOGIN: ' || P_LOGIN;
    V_MESSAGE := V_MESSAGE || ' P_ROLE: ' || P_ROLE;
    V_MESSAGE := V_MESSAGE || ' P_INV_CODE: ' || P_INV_CODE;
    V_MESSAGE := V_MESSAGE || ' P_TR_CODE: ' || P_TR_CODE;
    V_MESSAGE := V_MESSAGE || ' P_PAN: ' || P_PAN;
    V_MESSAGE := V_MESSAGE || ' P_DT_NUMBER: ' || P_DT_NUMBER;
    V_MESSAGE := V_MESSAGE || ' P_CmbSipStpA: ' || P_CmbSipStpA;
    V_MESSAGE := V_MESSAGE || ' P_CmbSubSipA: ' || P_CmbSubSipA;
    V_MESSAGE := V_MESSAGE || ' P_Cmbsubinv: ' || P_Cmbsubinv;
    V_MESSAGE := V_MESSAGE || ' P_FreshRenA: ' || P_FreshRenA;
    V_MESSAGE := V_MESSAGE || ' P_TxtCloseSch: ' || P_TxtCloseSch;
    V_MESSAGE := V_MESSAGE || ' P_SCHEME_A: ' || P_SCHEME_A;
    V_MESSAGE := V_MESSAGE || ' P_MySchCode: ' || P_MySchCode;
    V_MESSAGE := V_MESSAGE || ' P_MyCloseSchCode: ' || P_MyCloseSchCode;
    V_MESSAGE := V_MESSAGE || ' P_MySwitchScheme: ' || P_MySwitchScheme;
    V_MESSAGE := V_MESSAGE || ' P_txtdocID: ' || P_txtdocID;
    V_MESSAGE := V_MESSAGE || ' P_ImSipEndDtA: ' || P_ImSipEndDtA;
    V_MESSAGE := V_MESSAGE || ' P_Label32: ' || P_Label32;
    V_MESSAGE := V_MESSAGE || ' P_AMC: ' || P_AMC;
    V_MESSAGE := V_MESSAGE || ' P_SCHEME: ' || P_SCHEME;
    V_MESSAGE := V_MESSAGE || ' P_SCHEME_CODE: ' || P_SCHEME_CODE;
    V_MESSAGE := V_MESSAGE || ' P_AMOUNT: ' || P_AMOUNT;
    V_MESSAGE := V_MESSAGE || ' P_APP: ' || P_APP;
    V_MESSAGE := V_MESSAGE || ' P_BSS_CODE: ' || P_BSS_CODE;
    V_MESSAGE := V_MESSAGE || ' P_AH_NAME: ' || P_AH_NAME;
    V_MESSAGE := V_MESSAGE || ' P_AH_CODE: ' || P_AH_CODE;
    V_MESSAGE := V_MESSAGE || ' P_TRAN_TYPE: ' || P_TRAN_TYPE;
    V_MESSAGE := V_MESSAGE || ' P_SIP_STP: ' || P_SIP_STP;
    V_MESSAGE := V_MESSAGE || ' P_SIP_START_DT: ' || P_SIP_START_DT;
    V_MESSAGE := V_MESSAGE || ' P_SIP_END_DT: ' || P_SIP_END_DT;
    V_MESSAGE := V_MESSAGE || ' P_FREQ: ' || P_FREQ;
    V_MESSAGE := V_MESSAGE || ' P_INSTALL_NO: ' || P_INSTALL_NO;
    V_MESSAGE := V_MESSAGE || ' P_SIP_AMOUNT: ' || P_SIP_AMOUNT;
    V_MESSAGE := V_MESSAGE || ' P_FROM_SWICTH_SCH: ' || P_FROM_SWICTH_SCH;
    V_MESSAGE := V_MESSAGE || ' P_FROM_SWITCH_FOLIO: ' || P_FROM_SWITCH_FOLIO;
    V_MESSAGE := V_MESSAGE || ' P_PAYMENT_MODE: ' || P_PAYMENT_MODE;
    V_MESSAGE := V_MESSAGE || ' P_CHEQUE_NO: ' || P_CHEQUE_NO;
    V_MESSAGE := V_MESSAGE || ' P_CHEQUE_DT: ' || P_CHEQUE_DT;
    V_MESSAGE := V_MESSAGE || ' P_BRANCH_NAME: ' || P_BRANCH_NAME;
    V_MESSAGE := V_MESSAGE || ' P_TR_DT: ' || P_TR_DT;
    V_MESSAGE := V_MESSAGE || ' P_FOLIO_NO: ' || P_FOLIO_NO;
    V_MESSAGE := V_MESSAGE || ' P_BANK_NAME: ' || P_BANK_NAME;
    V_MESSAGE := V_MESSAGE || ' P_EXP_PER: ' || P_EXP_PER;
    V_MESSAGE := V_MESSAGE || ' P_EXP_AMT: ' || P_EXP_AMT;
    V_MESSAGE := V_MESSAGE || ' P_REGU_NFO: ' || P_REGU_NFO;
    V_MESSAGE := V_MESSAGE || ' P_COB_FLAG: ' || P_COB_FLAG;
    V_MESSAGE := V_MESSAGE || ' P_SWP_FLAG: ' || P_SWP_FLAG;
    V_MESSAGE := V_MESSAGE || ' P_FREEDOM_SIP_FLAG: ' || P_FREEDOM_SIP_FLAG;
    V_MESSAGE := V_MESSAGE || ' P_99_YEAR: ' || P_99_YEAR;
 

  OPEN P_CURSOR FOR 
        SELECT 'FALSE' AS STATUS, V_MESSAGE AS MESSAGE FROM DUAL;
        RETURN;

    IF P_LOGIN IS NOT NULL THEN
        V_MESSAGE := V_MESSAGE|| 'USER LOG ID '|| P_LOGIN;
    END IF;
    
    IF P_ROLE IS NOT NULL THEN
        V_MESSAGE := V_MESSAGE|| 'USER ROLE ID '|| P_ROLE;
    END IF;
    

    /*
        if (roleId != "212")
      {
          pc.ShowAlert(this, "You are not authorized to punch the transaction");
          return;
      }
    */

    IF P_ROLE IS NULL OR P_ROLE NOT IN ('212', '213', '214') THEN
        V_MESSAGE := V_MESSAGE || 'You are not authorized to punch the transaction';
        V_STATUS := 'FALSE';
    END IF;

    IF P_DT_NUMBER IS NULL OR P_DT_NUMBER = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please enter Document Number OR Please Select Investor to add';
        V_STATUS := 'FALSE';
    END IF;



    IF P_INV_CODE IS NULL OR P_INV_CODE = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select Investment Code';
        V_IS_VALID := 1;
    END IF;


    IF P_TR_CODE IS NULL OR P_TR_CODE = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select Transaction Code';
        V_IS_VALID := 1;
    END IF;
    IF P_PAN IS NULL OR LENGTH(P_PAN) < 10 THEN
        V_MESSAGE := V_MESSAGE || 'Please enter valid PAN';
        V_IS_VALID := 1;
    END IF;

    IF P_CmbSipStpA IS NULL OR P_CmbSipStpA = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select SIP/STP';
        V_IS_VALID := 1;
    END IF;
    IF P_CmbSubSipA IS NULL OR P_CmbSubSipA = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select SIP Type';
        V_IS_VALID := 1;
    END IF;
    IF P_Cmbsubinv IS NULL OR P_Cmbsubinv = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select Sub Investment Type';
        V_IS_VALID := 1;
    END IF;
    IF P_FreshRenA IS NULL OR P_FreshRenA = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select Fresh or Renewal';
        V_IS_VALID := 1;
    END IF;
    IF P_TranType IS NULL OR P_TranType = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please select Transaction Type';
        V_IS_VALID := 1;
    END IF;
    IF P_TxtPanA IS NULL OR LENGTH(P_TxtPanA) < 10 THEN
        V_MESSAGE := V_MESSAGE || 'Please enter valid PAN from Address Popup';
        V_IS_VALID := 1;
    END IF;
    IF P_TxtPanVarify IS NULL OR LENGTH(P_TxtPanVarify) < 10 THEN
        V_MESSAGE := V_MESSAGE || 'Please enter valid PAN from PAN Textbox';
        V_IS_VALID := 1;
    END IF;
    IF P_TxtAmountA IS NULL OR P_TxtAmountA = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please enter Amount';
        V_IS_VALID := 1;
    END IF;
    IF P_TxtSchemeA IS NULL OR P_TxtSchemeA = '' THEN
        V_MESSAGE := V_MESSAGE || 'Please enter Scheme';
        V_IS_VALID := 1;
    END IF;






























    IF V_MESSAGE IS NOT NULL THEN
        V_STATUS:='FALSE';
        
    END IF;
    
    OPEN P_CURSOR FOR 
        SELECT V_STATUS AS STATUS, V_MESSAGE AS MESSAGE FROM DUAL;
     


END;