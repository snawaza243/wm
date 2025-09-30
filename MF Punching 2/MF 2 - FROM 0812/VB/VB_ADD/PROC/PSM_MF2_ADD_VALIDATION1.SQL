CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF_ADD_VALIDATION1 (
    P_DT                IN VARCHAR2,
    P_ENTRY_DATE        IN VARCHAR2, -- ImEntryDtA
    P_SIP_STP           IN VARCHAR2, -- CmbSipStpA
    P_SIP_START_DT      IN VARCHAR2, -- ImSipStartDtA
    P_SIP_END_DT        IN VARCHAR2, -- ImSipEndDtA
    P_INV_CODE          IN VARCHAR2, -- Label32
    P_CHKCLOSE_FOR_SCH  IN VARCHAR2, -- FOR MUTILPE SCH POPUP
    P_SCH_CODE          IN VARCHAR2, -- MySchCode
    P_CLOSE_SCH_TEXT    IN VARCHAR2, -- P_CLOSE_SCH_TEXT
    P_AMOUNT            IN VARCHAR2, -- TxtAmountA
    P_INSTALLMENT_TYPE  IN VARCHAR2, -- CmbsubinvText
    P_SUB_SIP           IN VARCHAR2, -- CmbSubSipA
    P_SCH_NAME          IN VARCHAR2, -- TxtSchemeA
    P_BUSS_BRANCH       IN VARCHAR2, -- CmbBusiBranch
    P_BUSS_CODE         IN VARCHAR2, -- TxtBusiCodeA
    P_CHQ_DT            IN VARCHAR2, -- dtChqDate
    P_TRAN_TYPE         IN VARCHAR2, -- CmbTranTypeA
    P_FREQUENCY         IN VARCHAR2,  -- CmbFrequency
    P_INSTALLMENTS      IN VARCHAR2,  -- TxtInstallmens
    P_SIP_AMOUNT        IN VARCHAR2, -- TxtSipAmount
    P_SIP_FRESH_RENEWAL IN VARCHAR2, -- OptsipF, OptsipR
    P_INVESTOR          IN VARCHAR2, -- txtInvestorA
    P_AMC               IN VARCHAR2, -- CmbAmcA
    P_APP_NO            IN VARCHAR2, -- TxtAppnoA
    P_CLIENT_CODE       IN VARCHAR2, -- TxtClientCodeA
    P_SWITCH_SCH        IN VARCHAR2, -- TxtSwitchSchemeA
    P_SWITCH_FOLIO      IN VARCHAR2, -- TxtSwitchFolioA
    P_PANVARIFY         IN VARCHAR2, -- TxtPanVarify
    P_PANA              IN VARCHAR2, -- TxtPanA
    P_CHK_ATM_TR        IN VARCHAR2, -- ChkAtmTransactionA
    P_AMC_CODE          IN VARCHAR2, -- MyAmcCode
    P_GLBUP_PREVIOUSDATE IN VARCHAR2,
    P_GLBUP_NEXTDATE    IN VARCHAR2,
    P_PAYMENT_MODE      IN VARCHAR2, 
    P_EXPENSES          IN VARCHAR2,    
    P_LOG               IN  VARCHAR2,
    P_ROLE              IN  VARCHAR2,
    P_CURSOR    OUT SYS_REFCURSOR
) AS
    V_RETURN        CLOB := NULL;
    MICROPANFLAG    NUMBER;
    MYSCHCODE       VARCHAR2(100) := NULL;      -- SCHEME CODE
    MYCLOSESCHCODE  VARCHAR2(100) := NULL;      -- SECOND SCHEME
    MYBRANCHCODE    VARCHAR2(100) := NULL;
    MYSWITCHSCHCODEA VARCHAR2(100) := NULL;
    V_P_PANA        VARCHAR(10):=NULL;
BEGIN

    V_P_PANA:=P_PANA;

    IF P_ROLE NOT IN ('212') THEN        
        OPEN P_CURSOR FOR SELECT 'YOU ARE NOT AUTHORIZED TO PUNCH THE TRANSACTION' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;
    
    IF TRIM(P_DT) IS NOT NULL THEN
        DECLARE V_FN_CHECK_FOR_APPROVAL_ALL VARCHAR2(5);
        BEGIN
            SELECT WEALTHMAKER.FN_CHECK_FOR_APPROVAL_ALL(P_DT) INTO V_FN_CHECK_FOR_APPROVAL_ALL FROM DUAL;
            IF V_FN_CHECK_FOR_APPROVAL_ALL = '2' THEN
                OPEN P_CURSOR FOR SELECT 'Approval request for this transaction has already been raised' AS MESSAGE FROM DUAL; RETURN;
            ELSIF V_FN_CHECK_FOR_APPROVAL_ALL = '4' THEN
                OPEN P_CURSOR FOR SELECT 'Approval request for this transaction has been rejected by Management' AS MESSAGE FROM DUAL; RETURN;
            END IF;
        END;
    ELSE
        OPEN P_CURSOR FOR SELECT 'DT IS REQUIRED' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    IF P_SIP_STP = 'SIP' THEN
        IF P_SIP_END_DT IS NULL OR P_SIP_START_DT IS NULL THEN
            OPEN P_CURSOR FOR SELECT 'Please Enter SIP End Date ' AS MESSAGE FROM DUAL; RETURN;
        END IF;
    END IF;

    DECLARE
        STR1 VARCHAR2(100); 
    BEGIN
        IF P_CHKCLOSE_FOR_SCH = '1' THEN
            IF P_CLOSE_SCH_TEXT IS NOT NULL AND P_CLOSE_SCH_TEXT <> '' THEN
                STR1 := REGEXP_SUBSTR(P_CLOSE_SCH_TEXT, '[^=]+', 1, 2);
                MYCLOSESCHCODE := STR1;
            END IF;
        ELSE
            MYCLOSESCHCODE := '';
        END IF;
    END;

    IF P_INV_CODE <> '' AND MYSCHCODE <> '' AND P_AMOUNT <> '' THEN
        DECLARE 
            V_COUNT_DUP_TRAN_CODE NUMBER;
            V_AMOUNT1 NUMBER;
            V_AMOUNT2 NUMBER; 
        BEGIN
            V_AMOUNT1 := NVL(TO_NUMBER(P_AMOUNT),0) - 100;
            V_AMOUNT2 :=  NVL(TO_NUMBER(P_AMOUNT),0) + 100;

            SELECT COUNT(*) INTO V_COUNT_DUP_TRAN_CODE 
            FROM TRANSACTION_MF_TEMP1 A,SCHEME_INFO B 
            WHERE   (ASA <> 'C' OR ASA IS NULL) 
            AND A.SCH_CODE=B.SCH_CODE AND A.DOC_ID IS NOT NULL  
            AND CLIENT_CODE=P_INV_CODE
            AND TR_DATE>SYSDATE-90
            AND B.SCH_CODE= P_SCH 
            AND AMOUNT BETWEEN V_AMOUNT1 AND V_AMOUNT2;

            IF V_COUNT_DUP_TRAN_CODE > 0 THEN 
                -- OPEN CURSOR AND RETURN THE ALL ALL VALID WITH # CONCATENATED 
                OPEN P_CURSOR FOR
                SELECT 'DUPLICATEFORM#'|| 'MF#'|| P_AMOUNT || '#' || P_INV_CODE || '#' || P_SCH || '#' || '0' FROM DUAL;
                RETURN;
            END IF;
        END;
    ELSE
        -- save_method;
        BEGIN
            IF P_AMOUNT <> '' THEN
                MICROPANFLAG := 0;
                IF P_INSTALLMENT_TYPE = 'MICRO' AND P_AMOUNT < 50000 THEN
                    MICROPANFLAG := 1;
                ELSE
                    MICROPANFLAG := 0;
                END IF;
            END IF;
        END;

        DECLARE
            STR1 VARCHAR2(100); 
        BEGIN
            IF P_SCH_NAME IS NOT NULL AND P_SCH_NAME <> '' THEN
                STR1 := REGEXP_SUBSTR(P_SCH_NAME, '[^=]+', 1, 2);
                MYSCHCODE := STR1;
            END IF;
        END;


        IF P_DT IS NOT NULL AND P_DT <> '' THEN
            DECLARE
                V_COMBO_VAL NUMBER;
            BEGIN
                SELECT COMBO_PLAN_VAL(P_DT, MYSCHCODE, 0, 'MF')
                INTO V_COMBO_VAL
                FROM DUAL;

                IF V_COMBO_VAL = 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'ERROR: LI premium for combo plan can''t exceed 6.70 Lacs OR plan must be of combo' AS MESSAGE FROM DUAL;
                        RETURN;
                END IF;
            END;
        END IF;

        IF P_BUSS_BRANCH <> '' AND P_BUSS_BRANCH IS NOT NULL THEN
            MYBRANCHCODE := NVL(REGEXP_SUBSTR(P_BUSS_BRANCH, '[^#]+', 1, 2),0);
        END IF;

        IF MYBRANCHCODE IS NOT NULL AND MYBRANCHCODE <> '' AND MYBRANCHCODE <> 0 THEN
            DECLARE
                V_BRANCH_COUNT NUMBER;
            BEGIN
                SELECT COUNT(*)
                INTO V_BRANCH_COUNT
                FROM WEALTHMAKER.BRANCH_MASTER
                WHERE BRANCH_CODE = MYBRANCHCODE
                AND BRANCH_NAME LIKE 'UNALLO%';

                IF V_BRANCH_COUNT > 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'ERROR: Transaction can not be punched in Unallocated Branch. ' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END;
        END IF;

        -- VALIDATE UPLOADED CLIENT DT
        DECLARE
            V_RESULT  NUMBER;
            V_ERRMSG  VARCHAR2(200);
        BEGIN
            IF P_DT IS NOT NULL AND P_DT <> '' THEN
                -- Call the stored function/procedure
                WEALTHMAKER.FN_VALIDATE_CLIENT_DT(
                    PDT_NO      => P_DT,
                    PCLIENT_CODE => P_INV_CODE,
                    RESULT      => V_RESULT,
                    MESSAGE     => V_ERRMSG
                );

                -- If result = 0, then validation failed
                IF NVL(V_RESULT, 0) = 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT V_ERRMSG AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END;

        -- VALIDATE UPLOADED BRANCH DT
        DECLARE
            V_RESULT  NUMBER;
            V_ERRMSG  VARCHAR2(200);
        BEGIN
            IF P_BUSS_BRANCH IS NOT NULL AND P_BUSS_BRANCH <> '' THEN
                -- Call the validation procedure
                WEALTHMAKER.FN_VALIDATE_BRANCH_RM_DT(
                    PDT_NO       => P_DT,
                    PBRANCH_CODE => MYBRANCHCODE,
                    PPAYROLL_ID  => TRIM(P_BUSS_CODE),
                    RESULT       => V_RESULT,
                    MESSAGE      => V_ERRMSG
                );

                -- Check validation result
                IF NVL(V_RESULT, 0) = 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT V_ERRMSG AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END;

        -- VALIDATE FP STATUS
        /* NOT IN USE INTERNAL CODE IS COMMENTED
        DECLARE
            v_fp_status NUMBER;
        BEGIN
            IF P_DT IS NOT NULL AND P_DT <> '' 
            AND P_INV_CODE IS NOT NULL AND P_INV_CODE <> '' THEN

                -- Call the function and store the result
                SELECT fp_status_check1(P_INV_CODE, P_DT)
                INTO v_fp_status
                FROM dual;

                -- If status = 0, return error
                IF NVL(v_fp_status, 0) = 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Please create Financial planning for the investor first' AS MESSAGE
                        FROM dual;
                    RETURN;
                END IF;
            END IF;
        END;
        */

        -- VALIDATE DOCUMENT NUMBER IN transaction_st AND transaction_sttemp
        DECLARE
            V_EXISTS NUMBER;
        BEGIN
            IF P_DT IS NOT NULL AND P_DT <> '' THEN
                -- Check in transaction_st
                SELECT COUNT(*)
                INTO V_EXISTS
                FROM TRANSACTION_ST
                WHERE DOC_ID = P_DT;

                IF V_EXISTS > 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Document number already assigned.' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

                -- Check in transaction_sttemp
                SELECT COUNT(*)
                INTO V_EXISTS
                FROM TRANSACTION_STTEMP
                WHERE DOC_ID = P_DT;

                IF V_EXISTS > 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Document number already assigned.' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END;

        --If chkSaveValidation(True, False, "") = False Then

        -- chkSaveValidation equivalent
        
        DECLARE
            V_ORIGINAL_DATE DATE;
            V_COUNT         NUMBER;
            P_FRSAVE       NUMBER;
            P_FRMODY      NUMBER;
            P_HAVE_TR     VARCHAR2(20) := '';
            P_GLBINS_PREVIOUSDATE DATE;
            P_GLBINS_NEXTDATE     DATE;
            V_IMENTRYDATE        DATE:= TO_DATE(P_ENTRY_DATE,'DD/MM/YYYY');
        BEGIN
            P_FRSAVE := 1;
            P_FRMODY := 0;
            P_HAVE_TR := '';


            -- Get original date if tranCode is provided
            IF P_HAVE_TR IS NOT NULL AND P_HAVE_TR <> '' THEN
                SELECT TR_DATE
                INTO V_ORIGINAL_DATE
                FROM TRANSACTION_MF_TEMP1
                WHERE TRAN_CODE = P_HAVE_TR;
            END IF;

            -- Case 1: Saving a new record
            IF P_FRSAVE >= 1 AND P_FRMODY = 0 THEN
                IF (V_IMENTRYDATE < P_GLBINS_PREVIOUSDATE) OR (V_IMENTRYDATE > P_GLBINS_NEXTDATE) THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Security restrictions for date range' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            -- Case 2: Modifying an existing record
            IF P_FRSAVE = 0 AND P_FRMODY = 1 THEN
                IF (V_ORIGINAL_DATE < P_GLBUP_PREVIOUSDATE) OR (V_ORIGINAL_DATE > P_GLBUP_NEXTDATE) THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Security restrictions for date range' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

                SELECT COUNT(*)
                INTO V_COUNT
                FROM WEALTHMAKER.ANATRANDETAILTABLE_NEW_ALL_VB
                WHERE TRAN_CODE = P_HAVE_TR;

                IF V_COUNT > 0 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'ANG Bills generated for this transaction. You can not modify the payout of this AR.' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            -- If all checks pass, continue
        END;

        DECLARE
            V_SERVER_DATE   DATE := TRUNC(SYSDATE);
            V_IM_ENTRY_DATE DATE := TO_DATE(P_ENTRY_DATE,'DD/MM/YYYY');
            V_CHQ_DATE      DATE := TO_DATE(P_CHQ_DT,'DD/MM/YYYY'); 
            V_MONTH_DIFF    NUMBER;
            RESULT          NUMBER;
        BEGIN
            -- 1) Prevent punching transaction in advance
            IF (V_IM_ENTRY_DATE - V_SERVER_DATE) >= 1 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'You can not punch transaction in advance.' AS MESSAGE
                        FROM DUAL;
                    RETURN;
            END IF;

            -- 2) Cheque date validation (only if date is not null)
            IF V_CHQ_DATE IS NOT NULL THEN
                V_MONTH_DIFF := MONTHS_BETWEEN(TRUNC(V_CHQ_DATE), V_SERVER_DATE);

                IF V_MONTH_DIFF > 1 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'You can not give the cheque greater than one month' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END;

        DECLARE
            V_COUNT NUMBER;
        BEGIN
            SELECT COUNT(*)
            INTO V_COUNT
            FROM TB_DOC_UPLOAD
            WHERE COMMON_ID = TRIM(P_DT)
            AND TRAN_TYPE = 'MF';

            IF V_COUNT = 0 THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please enter a valid DT Number' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END;

        DECLARE
            V_FREQ_TEXT  VARCHAR2(100) := P_FREQUENCY;  -- Passed from front-end
            V_FREQ_PARTS DBMS_UTILITY.LNAME_ARRAY;         -- Array to hold split parts
            V_FREQ_COUNT INTEGER;
        BEGIN
            -- Check if frequency is provided (equivalent to ListIndex <> -1)
            IF V_FREQ_TEXT IS NOT NULL THEN
                -- Split by '#' using REGEXP_SUBSTR
                SELECT REGEXP_SUBSTR(V_FREQ_TEXT, '[^#]+', 1, 1),
                    REGEXP_SUBSTR(V_FREQ_TEXT, '[^#]+', 1, 2)
                INTO V_FREQ_PARTS(1), V_FREQ_PARTS(2)
                FROM DUAL;
            ELSE
                V_FREQ_PARTS(1) := '';
                V_FREQ_PARTS(2) := '';
            END IF;

            -- You can now use v_freq_parts(1) and v_freq_parts(2) in further logic
        END;

        IF TRIM(P_TRAN_TYPE) <> '' THEN
            IF P_TRAN_TYPE = 'PURCHASE' AND P_PAYMENT_MODE NOT IN ('CHEQUE', 'DRAFT', 'ECS', 'RTGS', 'FT') THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please Select Cheque/Draft/ECS/RTGS/Fund Transfer From Payment Mode' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        ELSE
            OPEN P_CURSOR FOR
                SELECT 'Please Select Transaction Type' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_SIP_STP) = '' THEN
            OPEN P_CURSOR FOR
                SELECT 'Please Select SIP Type' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF P_SIP_STP IN ('SIP','STP') THEN
            IF (TRIM(P_FREQUENCY) IS NULL OR TRIM(P_INSTALLMENTS) IS NULL) THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please Enter Frequency Type and No. Of Installments' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END IF;


        IF P_SIP_STP IN ('SIP') THEN
            IF TRIM(P_SIP_END_DT) IS NULL THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please Enter SIP End Date' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;

            --IS_NUMERIC(P_SIP_AMOUNT)
            IF TRIM(P_SIP_AMOUNT) IN ('', '0')  THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please Enter Valid SIP Amount' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;

            IF TRIM(P_SIP_FRESH_RENEWAL) NOT IN ('F', 'R') THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please select either Fresh or Renewal' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END IF;


        IF TRIM(P_SIP_FRESH_RENEWAL) NOT IN ('F') THEN
            IF  TRIM(P_PAYMENT_MODE) IN ('CHEQUE', 'DRAFT', 'ECS', 'RTGS', 'FT') THEN
                OPEN P_CURSOR FOR
                    SELECT 'Please Fill Cheque/Draft/MICR/UTR/Bank A/c No. ' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;

            IF TRIM(P_CHQ_DT) IS NULL THEN        
                OPEN P_CURSOR FOR
                    SELECT 'Cheque/Draft/ECS/UTR/Fund Transfer Can Not be Left Blank. ' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END IF;

        IF TRIM(P_TRAN_TYPE)  = 'SWITCH IN' AND TRIM(P_PAYMENT_MODE) IN ('CHEQUE', 'DRAFT', 'ECS', 'CASH', 'RTGS', 'FT') THEN
            OPEN P_CURSOR FOR
                SELECT 'Please Select Other Option ' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_INVESTOR) <>'' THEN
            OPEN P_CURSOR FOR
                SELECT 'Please Fill Investor Name' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_BUSS_CODE) <>'' THEN
            OPEN P_CURSOR FOR
                SELECT 'Please Fill Business Code A' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        -- CmbAmcA
        IF TRIM(P_AMC) = '' THEN
            OPEN P_CURSOR FOR
                SELECT 'Please Select AMC' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_TRAN_TYPE) = 'PURCHASE' THEN
            IF TRIM(P_APP_NO) IS NOT NULL AND TRIM(P_APP_NO) <> '' THEN
                IF LENGTH(P_APP_NO) < 6 THEN
                    OPEN P_CURSOR FOR
                        SELECT ' Minimum Length Of App No Should Be Greater or Equal To 6 ' FROM DUAL;
                        RETURN;
                ELSIF P_APP_NO = '000000' THEN
                    OPEN P_CURSOR FOR
                        SELECT ' Please Enter A Valid App No ' FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF TRIM(P_SCH_NAME) IS NULL THEN
            OPEN P_CURSOR FOR
                SELECT 'Select The Scheme' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_ENTRY_DATE) IS NULL THEN
            OPEN P_CURSOR FOR
                SELECT 'Transaction Date Can Not Be Left Blank' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_CLIENT_CODE) IS NULL THEN
            OPEN P_CURSOR FOR
                SELECT 'Client Code Can Not Left Blank' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;


        DECLARE
            V_AMOUNT NUMBER;
        BEGIN
            BEGIN
                -- Try converting to number
                V_AMOUNT := TO_NUMBER(P_AMOUNT);
            EXCEPTION
                WHEN VALUE_ERROR THEN
                    -- If conversion fails, treat as invalid
                    OPEN P_CURSOR FOR
                        SELECT 'Amount must be a valid number' AS MESSAGE
                        FROM DUAL;
                    RETURN;
            END;

            -- Now check if it's zero
            IF NVL(V_AMOUNT, 0) = 0 THEN
                OPEN P_CURSOR FOR
                    SELECT 'Amount cannot be zero' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END;

        IF TRIM(P_AMOUNT) IS NULL OR TRIM(P_AMOUNT) = '' THEN
            OPEN P_CURSOR FOR
                SELECT 'Amount Can Not Left Blank' AS MESSAGE
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_BUSS_BRANCH) <>'' THEN
            OPEN P_CURSOR FOR
                SELECT 'Business Branch Can Not Left Blank' AS MESSAGE
                FROM DUAL;
        END IF;
            
        DECLARE
            V_BRANCH_CODE VARCHAR2(100);
            V_SPLIT_POS   NUMBER;
        BEGIN
            -- Check if input is blank or NULL
            IF TRIM(P_BUSS_BRANCH) IS NULL THEN
                OPEN P_CURSOR FOR
                    SELECT 'Branch Name Cannot Be Left Blank' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;

            -- Find position of '#' character
            V_SPLIT_POS := INSTR(P_BUSS_BRANCH, '#');

            -- If '#' is found, extract the part after it
            IF V_SPLIT_POS > 0 THEN
                V_BRANCH_CODE := SUBSTR(P_BUSS_BRANCH, V_SPLIT_POS + 1);
                MYBRANCHCODE := V_BRANCH_CODE;
            ELSE
                OPEN P_CURSOR FOR
                    SELECT 'Invalid Branch Format. Expected format: Name#Code' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END;

        DECLARE
            V_SCH_CODE VARCHAR2(100);
        BEGIN
            -- Check if input is not blank
            IF TRIM(P_SCH_NAME) IS NOT NULL THEN
                -- Extract part after '=' using regex
                V_SCH_CODE := REGEXP_SUBSTR(P_SCH_NAME, '[^=]+', 1, 2);
                MYSCHCODE := V_SCH_CODE;
            END IF;

            -- Final validation: if scheme code is still blank
            IF TRIM(MYSCHCODE) IS NULL THEN
                OPEN P_CURSOR FOR
                    SELECT 'Select Scheme First' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END;
            
        DECLARE
            V_SWITCH_CODE VARCHAR2(100);
        BEGIN
            -- Check if input is not blank
            IF TRIM(P_SWITCH_SCH) IS NOT NULL THEN
                -- Extract part after '=' using regex
                V_SWITCH_CODE := REGEXP_SUBSTR(P_SWITCH_SCH, '[^=]+', 1, 2);
                MYSWITCHSCHCODEA := V_SWITCH_CODE;
            END IF;
        END;

        DECLARE
            V_COUNT NUMBER;
        BEGIN
            -- Check transaction type
            IF P_TRAN_TYPE = 'SWITCH IN' OR P_SIP_STP = 'STP' THEN

                -- Validate Switch Scheme
                IF TRIM(P_SWITCH_SCH) IS NULL THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Select the Scheme you have Switched From' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

                -- Validate Switch Folio
                IF TRIM(P_SWITCH_FOLIO) IS NULL THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Select the Folio you have Switched From' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

                -- Check if Switch From and To schemes are the same
                IF MYSCHCODE = MYSWITCHSCHCODEA THEN
                    OPEN P_CURSOR FOR
                        SELECT 'In case of Switch transaction, Switch from Scheme cannot be same as Switch to Scheme' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

                -- Check if both schemes belong to the same AMC
                SELECT COUNT(DISTINCT MUT_CODE)
                INTO V_COUNT
                FROM SCHEME_INFO
                WHERE SCH_CODE IN (MYSCHCODE, MYSWITCHSCHCODEA);

                IF V_COUNT > 1 THEN
                    OPEN P_CURSOR FOR
                        SELECT 'In case of switch Transaction, Switch from Scheme and Switch to Scheme should be from one AMC only' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

            END IF;
        END;

        DECLARE
            V_DOB           DATE;
            V_IS_MINOR      BOOLEAN := FALSE;
            V_PAN1          VARCHAR2(20);
            V_INV_CD        VARCHAR2(20);
            V_ACCOUNT_PAN   NUMBER;
            V_CATEGORY      VARCHAR2(10);
        BEGIN
            -- Skip PAN validation for MICROSIP under SIP
            IF NOT (P_TRAN_TYPE = 'SIP' AND P_SUB_SIP = 'MICROSIP') THEN
                
                -- Micro PAN validation
                IF MYSCHCODE <> 'OP#10826' THEN
                    IF MICROPANFLAG = 0 THEN
                    IF TRIM(P_PANVARIFY) IS NULL OR NOT REGEXP_LIKE(P_PANA, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                        IF NOT REGEXP_LIKE(P_PANVARIFY, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                            OPEN P_CURSOR FOR
                                SELECT 'Please enter a valid PAN number' AS MESSAGE FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;
                    END IF;
                END IF;

                -- Minor check for investor starting with '3'
                IF MYSCHCODE <> 'OP#10826' AND SUBSTR(P_INV_CODE, 1, 1) = '3' THEN
                    SELECT NVL(DOB, SYSDATE - 10000)
                    INTO V_DOB
                    FROM INVESTOR_MASTER
                    WHERE INV_CODE = P_INV_CODE;

                    IF V_DOB IS NOT NULL AND MONTHS_BETWEEN(SYSDATE, V_DOB) / 12 <= 18 THEN
                        V_IS_MINOR := TRUE;
                    END IF;

                    -- PAN match for non-minor
                    IF NOT V_IS_MINOR AND MICROPANFLAG = 0 THEN
                        SELECT UPPER(PAN)
                        INTO V_PAN1
                        FROM INVESTOR_MASTER
                        WHERE INV_CODE = P_INV_CODE;

                        IF V_PAN1 IS NOT NULL AND V_PAN1 <> '0' THEN
                            IF V_PAN1 <> UPPER(P_PANVARIFY) THEN
                                OPEN P_CURSOR FOR
                                    SELECT 'PAN entered does not match with the selected investor''s PAN' AS MESSAGE FROM DUAL;
                                RETURN;
                            END IF;
                        END IF;
                    END IF;
                END IF;

                -- Minor check for investor starting with '4'
                IF MYSCHCODE <> 'OP#10826' AND SUBSTR(P_INV_CODE, 1, 1) = '4' THEN
                    SELECT NVL(DOB, TO_DATE('01/01/1800', 'DD/MM/YYYY'))
                    INTO V_DOB
                    FROM CLIENT_TEST
                    WHERE CLIENT_CODEKYC = P_INV_CODE;

                    IF V_DOB = TO_DATE('01/01/1800', 'DD/MM/YYYY') THEN
                        OPEN P_CURSOR FOR
                            SELECT 'Please fill Date of Birth for this investor' AS MESSAGE FROM DUAL;
                        RETURN;
                    END IF;

                    SELECT NVL(INVESTOR_CODE, '1')
                    INTO V_CATEGORY
                    FROM CLIENT_MASTER
                    WHERE CLIENT_CODE = SUBSTR(P_INV_CODE, 1, 8);

                    IF V_DOB IS NOT NULL AND MONTHS_BETWEEN(SYSDATE, V_DOB) / 12 <= 18 THEN
                        V_IS_MINOR := (V_CATEGORY = '1');
                    END IF;

                    -- PAN match logic
                    IF NOT V_IS_MINOR THEN
                        IF MICROPANFLAG = 0 THEN
                            SELECT CLIENT_CODEKYC
                            INTO V_INV_CD
                            FROM CLIENT_TEST
                            WHERE UPPER(CLIENT_PAN) = UPPER(P_PANVARIFY);

                            IF V_INV_CD IS NOT NULL AND V_INV_CD <> P_INV_CODE THEN
                                OPEN P_CURSOR FOR
                                    SELECT 'PAN entered does not match with the selected investor''s PAN' AS MESSAGE FROM DUAL;
                                RETURN;
                            END IF;

                            SELECT COUNT(*)
                            INTO V_ACCOUNT_PAN
                            FROM CLIENT_TEST
                            WHERE CLIENT_CODEKYC = P_INV_CODE
                            AND (UPPER(CLIENT_PAN) = UPPER(P_PANVARIFY) OR CLIENT_PAN IS NULL);

                            IF V_ACCOUNT_PAN = 0 THEN
                                OPEN P_CURSOR FOR
                                    SELECT 'PAN entered does not match with the selected investor''s PAN' AS MESSAGE FROM DUAL;
                                RETURN;
                            END IF;
                        END IF;
                    ELSE
                        -- Minor PAN match via guardian PAN
                        IF MICROPANFLAG = 0 THEN
                            SELECT NVL(UPPER(G_PAN), '0')
                            INTO V_PAN1
                            FROM CLIENT_TEST
                            WHERE CLIENT_CODEKYC = P_INV_CODE;

                            IF V_PAN1 <> '0' AND V_PAN1 <> UPPER(P_PANVARIFY) THEN
                                OPEN P_CURSOR FOR
                                    SELECT 'PAN entered does not match with the guardian''s PAN' AS MESSAGE FROM DUAL;
                                RETURN;
                            END IF;

                            SELECT COUNT(*)
                            INTO V_ACCOUNT_PAN
                            FROM CLIENT_TEST
                            WHERE CLIENT_CODEKYC = P_INV_CODE
                            AND (UPPER(G_PAN) = UPPER(P_PANVARIFY) OR G_PAN IS NULL);

                            IF V_ACCOUNT_PAN = 0 THEN
                                OPEN P_CURSOR FOR
                                    SELECT 'PAN entered does not match with the guardian''s PAN' AS MESSAGE FROM DUAL;
                                RETURN;
                            END IF;
                        END IF;
                    END IF;
                END IF;

                -- Final fallback: copy PAN if missing
                IF TRIM(P_PANA) IS NULL OR NOT REGEXP_LIKE(P_PANA, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                    V_P_PANA := P_PANVARIFY;
                END IF;

            END IF;
        END;

        DECLARE
        V_MIN_AMOUNT NUMBER;
        BEGIN
            -- Check if ATM transaction is selected
            IF P_CHK_ATM_TR = '1' THEN

                -- Fetch minimum amount for the scheme
                SELECT MIN_AMOUNT
                INTO V_MIN_AMOUNT
                FROM RELIANCE_ATM_MASTER
                WHERE SCH_CODE = MYSCHCODE
                AND FROM_DT <= SYSDATE
                AND (TO_DT >= SYSDATE OR TO_DT IS NULL);

                -- Compare with entered amount
                IF NVL(P_AMOUNT, 0) < V_MIN_AMOUNT THEN
                    OPEN P_CURSOR FOR
                        SELECT 'Minimum amount condition to have Reliance ATM Card is not being fulfilled' AS MESSAGE
                        FROM DUAL;
                    RETURN;
                END IF;

            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                -- No matching scheme found, treat as condition not fulfilled
                OPEN P_CURSOR FOR
                    SELECT 'Minimum amount condition to have Reliance ATM Card is not being fulfilled' AS MESSAGE
                    FROM DUAL;
                RETURN;
        END;



        DECLARE
            V_EXISTS NUMBER;
        BEGIN
            -- Check if transaction already exists for the given App No and AMC Code
            SELECT COUNT(*)
            INTO V_EXISTS
            FROM TRANSACTION_MF_TEMP1
            WHERE MUT_CODE = P_AMC
            AND APP_NO = P_APP_NO
            AND (ASA <> 'C' OR ASA IS NULL);

            -- If record exists, show message and exit
            IF V_EXISTS > 0 THEN
                OPEN P_CURSOR FOR
                    SELECT 'Sorry, this App No has already been punched in this company' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END;




        DECLARE
            V_RECD_TOTAL NUMBER := 0;
        BEGIN
            -- Calculate total receivable using two functions
            SELECT NVL(UPFRONT_RECD_NONANG_DETAIL(P_AMC, MYSCHCODE, TO_DATE(P_ENTRY_DATE, 'MM/DD/YYYY'), P_AMOUNT, P_TRAN_TYPE, MYSWITCHSCHCODEA), 0)
                 + NVL(TRAIL_RECD_ANG_DETAIL(P_AMC, MYSCHCODE, TO_DATE(P_ENTRY_DATE, 'MM/DD/YYYY'), P_AMOUNT), 0)
            INTO V_RECD_TOTAL
            FROM DUAL;

            -- Compare with entered expenses
            IF NVL(P_EXPENSES, 0) > V_RECD_TOTAL AND V_RECD_TOTAL <> 0 THEN
                OPEN P_CURSOR FOR
                    SELECT 'Payable cannot be greater than Receivable' AS MESSAGE
                    FROM DUAL;
                RETURN;
            END IF;
        END;
    END IF;

    IF V_RETURN IS NOT NULL THEN
        OPEN P_CURSOR FOR 
            SELECT 'ERROR: ' || V_RETURN AS MESSAGE FROM DUAL;
        RETURN;
    ELSE 
        OPEN P_CURSOR FOR 
            SELECT 'SUCCESS: MF PUNCHING TRANSACTION VALIDATION PASS ' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        V_RETURN := 'Unhandled Exception: ' || SQLERRM;
        OPEN P_CURSOR FOR SELECT 'ERROR: ' || V_RETURN AS MESSAGE FROM DUAL;
END;
/