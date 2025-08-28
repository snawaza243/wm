CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_MODIFY_VALIDATION (
    P_TRAN_CODE          IN VARCHAR2,
    P_SIPSTP_M           IN VARCHAR2, -- CmbSipStpM
    P_SIP_START_DT       IN VARCHAR2, -- ImSipStartDtM
    P_SIP_END_DT         IN VARCHAR2, -- ImSipEndDtM
    P_CHKCLOSE_FOR_SCH   IN VARCHAR2, -- ChkCloseM
    P_TXTCLOSESCHTEXT    IN VARCHAR2, -- TxtCloseSchM
    P_CMBSUBINVU         IN VARCHAR2, -- Cmbsubinvu
    P_TXTAMOUNTM         IN VARCHAR2, -- TxtAmountM
    P_TXTSCHEMEM         IN VARCHAR2, -- TxtSchemeM
    P_TXTSWITCHSCHEMEM   IN VARCHAR2, -- TxtSwitchSchemeM
    P_TRAN_TYPE          IN VARCHAR2, -- cmbTranTypeM
    P_MYAMCCODE          IN VARCHAR2, -- MyAmcCode
    P_TXTCLIENTCODEM     IN VARCHAR2, -- TxtClientCodeM
    P_IMENTRYDTM         IN VARCHAR2, -- ImEntryDtM
    P_DTCHQDATEM         IN VARCHAR2, -- dtChqDateM
    P_TXTPANM            IN VARCHAR2, -- TxtPanM
    P_TXTAPPNOM          IN VARCHAR2, -- TxtAppnoM
    P_TXTBUSICODEM       IN VARCHAR2, -- TxtBusiCodeM
    P_TXTINVESTORM       IN VARCHAR2, -- txtInvestorM
    P_CMBAMCM            IN VARCHAR2, -- CmbAmcM
    P_SIP_FRESH_RENEWAL  IN VARCHAR2, -- OptSIPFM, OptSIPRM
    P_PAYMENT_MODE       IN VARCHAR2, -- paymode
    P_TXTCHQNOM          IN VARCHAR2, -- txtChqNoM
    P_CMBBANKNAMEM       IN VARCHAR2, -- cmbBankNameM
    P_BUSS_BRANCH        IN VARCHAR2, -- cmbBusiBranchM
    P_TXTSWITCHFOLIOM    IN VARCHAR2, -- TxtSwitchFolioM
    P_CHKATMTRANSACTIONM IN VARCHAR2, -- ChkAtmTransactionM
    P_TXTACHOLDERM       IN VARCHAR2, -- TxtAcHolderM
    P_EXP_RATE           IN VARCHAR2, -- ImnExp_Per1
    P_EXP_AMOUNT         IN VARCHAR2, -- ImExpenses1
    P_TXTLEADNAMEM       IN VARCHAR2, -- TxtLeadNameM
    P_TXTDOCID           IN VARCHAR2, -- txtdocID
    P_CHKCOBM            IN VARCHAR2, -- chkcobM
    P_CHKFREEDOMM        IN VARCHAR2, -- ChkFreedomM
    P_CHKSWPM            IN VARCHAR2, -- chkSWPM
    P_TXTFOLIONOM        IN VARCHAR2, -- TxtFolioNoM
    P_CMBFREQUENCY1      IN VARCHAR2, -- CmbFrequency1
    P_TXTINSTALLMENTS1   IN VARCHAR2, -- Txtinstallments1
    P_DROPDATE           IN VARCHAR2, -- dtDropDate
    P_ROLE_ID            IN NUMBER,   -- GlbroleId
    P_LOGIN_ID           IN NUMBER,   -- Glbloginid
    P_RECON_STATUS       IN VARCHAR2, -- LblRecoStatus.Caption
    P_CURSOR             OUT SYS_REFCURSOR
) AS
    V_MYCLOSESCHCODEM    VARCHAR2(100);
    V_MYSCHCODE          VARCHAR2(100);
    V_MYSWITCHSCHCODEM   VARCHAR2(100);
    V_MYBRANCHCODE       VARCHAR2(100);
    V_MICROPANFLAG       NUMBER := 0;
    V_MODIFYDOCID        VARCHAR2(100);
    V_MYFREQ1            VARCHAR2(100);
    V_PAYMODE            VARCHAR2(1);
    V_MYRECDM            NUMBER := 0;
    V_COUNT              NUMBER;
    V_SQL                VARCHAR2(4000);
    V_BASETRANCODE       VARCHAR2(100);
    V_MYUNIQUEKEY        VARCHAR2(100);
    V_SIPADV             NUMBER;
    V_MYBROKRECD         NUMBER;
    V_MYBROKPAY          NUMBER;
    V_MYMARGIN           NUMBER;
BEGIN
    -- Admin role validation
    IF P_ROLE_ID = 1 THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: You are not authorized to update the Transaction Details' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- SIP validation
    IF P_SIPSTP_M = 'SIP' THEN
        IF P_SIP_END_DT IS NULL OR P_SIP_START_DT IS NULL THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Please Enter SIP End Date' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Close scheme validation
    IF P_CHKCLOSE_FOR_SCH = '1' THEN
        IF P_TXTCLOSESCHTEXT IS NOT NULL THEN
            V_MYCLOSESCHCODEM := REGEXP_SUBSTR(P_TXTCLOSESCHTEXT, '[^=]+', 1, 2);
        END IF;
    ELSE
        V_MYCLOSESCHCODEM := NULL;
    END IF;

    -- Sub-investor type validation
    IF P_CMBSUBINVU NOT IN ('NORMAL', 'MICRO') THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Please Select Correct Sub investor type' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Micro PAN validation
    IF P_TXTAMOUNTM IS NOT NULL THEN
        IF P_CMBSUBINVU = 'MICRO' AND TO_NUMBER(P_TXTAMOUNTM) < 50000 THEN
            V_MICROPANFLAG := 1;
        ELSE
            V_MICROPANFLAG := 0;
        END IF;
    END IF;

    -- Scheme code extraction
    IF P_TXTSCHEMEM IS NOT NULL THEN
        V_MYSCHCODE := REGEXP_SUBSTR(P_TXTSCHEMEM, '[^=]+', 1, 2);
    END IF;

    -- Switch scheme code extraction
    IF P_TXTSWITCHSCHEMEM IS NOT NULL THEN
        V_MYSWITCHSCHCODEM := REGEXP_SUBSTR(P_TXTSWITCHSCHEMEM, '[^=]+', 1, 2);
    END IF;

    -- Switch transaction validation
    IF P_TRAN_TYPE = 'SWITCH IN' OR P_SIPSTP_M = 'STP' THEN
        IF P_TXTSWITCHSCHEMEM IS NULL THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Select the Scheme you have Switched From' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
        
        IF P_TXTSWITCHFOLIOM IS NULL THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Select the Folio you have Switched From' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;

        IF V_MYSCHCODE = V_MYSWITCHSCHCODEM THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Switch from Scheme cannot be same as Switch to Scheme' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;

        SELECT COUNT(DISTINCT MUT_CODE)
        INTO V_COUNT
        FROM SCHEME_INFO
        WHERE SCH_CODE IN (V_MYSCHCODE, V_MYSWITCHSCHCODEM);

        IF V_COUNT > 1 THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Switch from and to Scheme should be from one AMC only' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Combo plan validation
    IF P_TXTDOCID IS NOT NULL AND P_TXTDOCID != 'NA' THEN
        SELECT WEALTHMAKER.COMBO_PLAN_VAL(
            (SELECT DISTINCT COMMON_ID FROM TB_DOC_UPLOAD WHERE AR_CODE = P_TRAN_CODE),
            V_MYSCHCODE, 0, 'MF')
        INTO V_COUNT
        FROM DUAL;

        IF V_COUNT = 0 THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: LI premium for combo plan cannot exceed 6.70 Lacs OR plan must be of combo' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Date validation
    IF MONTHS_BETWEEN(TO_DATE(P_IMENTRYDTM, 'DD/MM/YYYY'), SYSDATE) >= 1 THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: You cannot punch transaction in advance' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    IF P_DTCHQDATEM IS NOT NULL THEN
        IF MONTHS_BETWEEN(TO_DATE(P_DTCHQDATEM, 'DD/MM/YYYY'), SYSDATE) > 1 THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: You cannot give cheque date greater than one month' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- DT Number validation
    SELECT NVL(DOC_ID, 'NA') INTO V_MODIFYDOCID 
    FROM WEALTHMAKER.TRANSACTION_MF_TEMP1 
    WHERE TRAN_CODE = P_TRAN_CODE;

    IF V_MODIFYDOCID != 'NA' THEN
        SELECT COUNT(*) INTO V_COUNT
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_TXTDOCID) AND TRAN_TYPE = 'MF';

        IF V_COUNT = 0 THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Please enter a valid DT Number' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Reconciled transaction validation
    IF P_RECON_STATUS = 'Confirmed' AND P_ROLE_ID NOT IN (29, 1, 21) THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Reconciled transactions cannot be modified' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Frequency validation
    IF P_CMBFREQUENCY1 IS NOT NULL THEN
        V_MYFREQ1 := REGEXP_SUBSTR(P_CMBFREQUENCY1, '[^#]+', 1, 2);
    ELSE
        V_MYFREQ1 := NULL;
    END IF;

    -- Transaction type validation
    IF P_TRAN_TYPE IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Please Select Transaction Type' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- SIP type validation
    IF P_SIPSTP_M IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Please Select SIP Type' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Frequency and installments validation
    IF P_SIPSTP_M IN ('SIP', 'STP') THEN
        IF P_CMBFREQUENCY1 IS NULL OR P_TXTINSTALLMENTS1 IS NULL THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Please Enter Frequency Type and No. Of Installments' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- PAN validation
    IF V_MICROPANFLAG = 0 THEN
        IF P_TXTPANM IS NOT NULL THEN
            IF NOT REGEXP_LIKE(P_TXTPANM, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Please enter a valid PAN Number' AS MESSAGE FROM DUAL;
                RETURN;
            END IF;
        END IF;
    END IF;

    -- App number validation
    IF P_TXTAPPNOM IS NOT NULL THEN
        IF LENGTH(P_TXTAPPNOM) < 6 THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Minimum Length Of App No Should Be Greater or Equal To 6' AS MESSAGE FROM DUAL;
            RETURN;
        ELSIF P_TXTAPPNOM = '000000' THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Please Enter A Valid App No' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Business code validation
    IF P_TXTBUSICODEM IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: BusiCode Cannot Be Left Blank' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Investor name validation
    IF P_TXTINVESTORM IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Please Fill Investor Name' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Client code validation
    IF LENGTH(P_TXTCLIENTCODEM) < 8 THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Client Code Cannot Be Left Blank' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- AMC validation
    IF P_CMBAMCM IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Select The AMC' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- SIP fresh/renewal validation
    IF P_SIPSTP_M = 'SIP' THEN
        IF P_SIP_FRESH_RENEWAL NOT IN ('F', 'R') THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Please select either Fresh or Renewal' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Entry date validation
    IF P_IMENTRYDTM IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Transaction Date Cannot Be Left Blank' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Payment mode validation for fresh SIP
    IF P_SIP_FRESH_RENEWAL = 'F' THEN
        IF P_PAYMENT_MODE IN ('CHEQUE', 'DRAFT', 'ECS', 'RTGS', 'FT') THEN
            IF P_TXTCHQNOM IS NULL THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Please Fill Payment Details' AS MESSAGE FROM DUAL;
                RETURN;
            END IF;
            
            IF P_DTCHQDATEM IS NULL THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Payment Date Cannot Be Left Blank' AS MESSAGE FROM DUAL;
                RETURN;
            END IF;
        END IF;
    END IF;

    -- Switch transaction payment mode validation
    IF P_TRAN_TYPE = 'SWITCH IN' AND P_PAYMENT_MODE IN ('CHEQUE', 'DRAFT', 'ECS', 'CASH', 'RTGS', 'FT') THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Please Select Other Option for payment mode' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Amount validation
    IF P_TXTAMOUNTM IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Amount Cannot Be Left Blank' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    IF TO_NUMBER(P_TXTAMOUNTM) = 0 THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Amount Cannot Be Zero' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- Branch code extraction
    IF P_BUSS_BRANCH IS NOT NULL THEN
        V_MYBRANCHCODE := REGEXP_SUBSTR(P_BUSS_BRANCH, '[^#]+', 1, 2);
    END IF;

    -- Scheme validation
    IF V_MYSCHCODE IS NULL THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Scheme Cannot Be Left Blank' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- ATM transaction validation
    IF P_CHKATMTRANSACTIONM = '1' THEN
        IF WEALTHMAKER.GET_ATM_SCHEME_AMT_CONDITION(V_MYSCHCODE, TO_NUMBER(P_TXTAMOUNTM)) = 0 THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Minimum amount condition for Reliance ATM Card not fulfilled' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END IF;

    -- Receivable validation
    SELECT NVL(UPFRONT_RECD_NONANG(P_TRAN_CODE), 0) + NVL(TRAIL_RECD_ANG(P_TRAN_CODE), 0)
    INTO V_MYRECDM
    FROM DUAL;

    IF TO_NUMBER(P_EXP_AMOUNT) > V_MYRECDM AND V_MYRECDM <> 0 THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: Payable Cannot Be Greater Than Receivable' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    -- All validations passed
    OPEN P_CURSOR FOR SELECT 'SUCCESS: Validation passed, ready to update transaction' AS MESSAGE FROM DUAL;

EXCEPTION
    WHEN OTHERS THEN
        OPEN P_CURSOR FOR SELECT 'ERROR: ' || SQLERRM AS MESSAGE FROM DUAL;
END PSM_MF_MODIFY_VALIDATION;
/