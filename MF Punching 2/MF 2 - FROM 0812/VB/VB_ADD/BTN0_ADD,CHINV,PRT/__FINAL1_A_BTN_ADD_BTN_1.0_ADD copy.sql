--2025-10-06


CREATE OR REPLACE PROCEDURE PSM_MF2_ADD_PROCESS (
    PX_LOG_ID               IN VARCHAR2,
    PX_ROLE_ID              IN VARCHAR2,
    PX_DT_NUMBER            IN VARCHAR2,
    PX_TR_DATE              IN VARCHAR2,
    PX_BSS_CODE             IN VARCHAR2,
    PX_RM                   IN VARCHAR2,
    PX_INV_CODE             IN VARCHAR2,
    PX_CLIENT_CODE          IN VARCHAR2,
    PX_ANA_CODE             IN VARCHAR2,
    PX_AH_NAME              IN VARCHAR2,
    PX_AH_CODE              IN VARCHAR2,
    PX_BRANCH               IN VARCHAR2,
    PX_AMC                  IN VARCHAR2,
    PX_LIST_LONG            IN VARCHAR2,
    PX_TRANSACTION_TYPE     IN VARCHAR2,
    PX_REGULAR_NFO          IN VARCHAR2,
    PX_HDN_SCHEME1          IN VARCHAR2,
    PX_HDN_SCHEME1          IN VARCHAR2,
    PX_SEARCH_STATE         IN VARCHAR2,
    PX_SEARCH_FORM          IN VARCHAR2,
    PX_FROM_SWITCH_FOLIO    IN VARCHAR2,
    PX_SCHEME2_SWITCH       IN VARCHAR2,
    PX_HDN_SCHEME2_SWITCH   IN VARCHAR2,
    PX_APP_NO               IN VARCHAR2,
    PX_FOLIO_NO             IN VARCHAR2,
    PX_AMOUNT               IN VARCHAR2,
    PX_PAYMENT_MODE         IN VARCHAR2,
    PX_CHEQUE_NO            IN VARCHAR2,
    PX_CHEQUE_DATE          IN VARCHAR2,
    PX_BANK_NAME            IN VARCHAR2,
    PX_EXP_PER              IN VARCHAR2,
    PX_EXP_RS               IN VARCHAR2,
    PX_AUTO_SWITCH_MATURITY IN CHAR,
    PX_SCHEME3_CLOSE        IN VARCHAR2,
    PX_HDN_SCHEME3_CLOSE    IN VARCHAR2,
    PX_SIP_STP              IN VARCHAR2,
    PX_INSTALLMENT_TYPE     IN VARCHAR2,
    PX_SIP_TYPE             IN VARCHAR2,
    PX_SIPX_AMOUNT          IN VARCHAR2,
    PX_FREQUENCY            IN VARCHAR2,
    PX_INSTALLMENTS_NO      IN VARCHAR2,
    PX_SIP_START_DATE       IN VARCHAR2,
    PX_SIP_END_DATE         IN VARCHAR2,
    PX_FRESH_RENEWAL        IN VARCHAR2,
    PX_COB_CASE             IN CHAR,
    PX_SWP_CASE             IN CHAR,
    PX_FREEDOM_SIP          IN CHAR,
    PX_99_YEARS             IN CHAR,
    PX_PAN2                 IN VARCHAR2,
    PX_HDN_PAN1             IN VARCHAR2,
    PX_CURSOR               OUT SYS_REFCURSOR
)
AS

VP_MICRO_PAN_FLAG           NUMBER;
V_CHK_ATM_TR                VARCHAR2:=NULL;


/*
PAYMENT_MODE
C-->Cheque
D-->Draft
E-->ECS
H-->Cash
R-->Others
U-->RTGS
B-->File_Transfer

*/

BEGIN
    
-- VALIDATION 1
    BEGIN 

        IF PX_ROLE_ID NOT IN ('212') THEN        
            OPEN P_CURSOR FOR SELECT 'YOU ARE NOT AUTHORIZED TO PUNCH THE TRANSACTION' AS MSG FROM DUAL;
            RETURN;
        END IF;

        
        IF TRIM(PX_DT) IS NOT NULL THEN
            DECLARE V_FN_CHECK_FOR_APPROVAL_ALL VARCHAR2(1);
            BEGIN
                SELECT WEALTHMAKER.FN_CHECK_FOR_APPROVAL_ALL(PX_DT) INTO V_FN_CHECK_FOR_APPROVAL_ALL FROM DUAL WHERE ROWNUM = 1;
                IF V_FN_CHECK_FOR_APPROVAL_ALL = '2' THEN
                    OPEN PX_CURSOR FOR SELECT 'Approval request for this transaction has already been raised' AS MSG FROM DUAL; RETURN;
                ELSIF V_FN_CHECK_FOR_APPROVAL_ALL = '4' THEN
                    OPEN PX_CURSOR FOR SELECT 'Approval request for this transaction has been rejected by Management' AS MSG FROM DUAL; RETURN;
                END IF;
            END;
        ELSE
            OPEN PX_CURSOR FOR SELECT 'DT IS REQUIRED' AS MSG FROM DUAL;
            RETURN;
        END IF;

        IF PX_SIP_STP = 'SIP' THEN
            IF PX_SIP_END_DATE IS NULL OR P_SIP_START_DT IS NULL THEN
                OPEN PX_CURSOR FOR SELECT 'Please Enter SIP End Date ' AS MSG FROM DUAL; RETURN;
            END IF;
        END IF;

        IF PX_AUTO_SWITCH_MATURITY = '1' AND PX_HDN_SCHEME3_CLOSE IS NOT NULL THEN
            V_SCH_CD_CLOSE:= PX_HDN_SCHEME3_CLOSE;
        ELSE
            V_SCH_CD_CLOSE := '';
        END IF;

        IF PX_INV_CODE <> '' AND PX_HDN_SCHEME1 <> '' AND PX_AMOUNT <> '' THEN
            DECLARE 
                VP_COUNT_DUP_TR NUMBER;
                VP_AMT1 NUMBER;
                VP_AMT2 NUMBER; 
            BEGIN
                VP_AMT1 := NVL(TO_NUMBER(PX_AMOUNT),0) - 100;
                VP_AMT2 :=  NVL(TO_NUMBER(PX_AMOUNT),0) + 100;

                SELECT COUNT(*) INTO VP_COUNT_DUP_TR 
                  FROM TRANSACTION_MF_TEMP1 A,SCHEME_INFO B 
                 WHERE (ASA <> 'C' OR ASA IS NULL) 
                   AND A.SCH_CODE=B.SCH_CODE AND A.DOC_ID IS NOT NULL  
                   AND CLIENT_CODE=PX_INV_CODE
                   AND TR_DATE>SYSDATE-90
                   AND B.SCH_CODE= PX_HDN_SCHEME1 
                   AND AMOUNT BETWEEN VP_AMT1 AND VP_AMT2;

                IF VP_COUNT_DUP_TR > 0 THEN 
                    OPEN PX_CURSOR FOR
                    SELECT 
                        'DUPLICATEFORM' AS MSG,
                        'MF'            AS FORMTYPE,
                        PX_AMOUNT       AS PREM_AMT,
                        PX_INV_CODE     AS CCODE,
                        PX_HDN_SCHEME1  AS sch_code 
                    FROM DUAL;
                    RETURN;
                END IF;
            END;
        ELSE

            -- VALIDATION 2
            IF PX_AMOUNT IS NOT NULL THEN
                VP_MICRO_PAN_FLAG :=0;
                IF PX_SIP_TYPE = 'MICRO' AND PX_AMOUNT <'50000' THEN
                    VP_MICRO_PAN_FLAG :=1;
                ELSE
                    VP_MICRO_PAN_FLAG :=0;
                END IF;
            END IF;

            -- LI premium EXCEED AMOUNT
            IF PX_DT IS NOT NULL THEN
                DECLARE VP_VAL_COUNT NUMBER;
                BEGIN
                    SELECT COMBO_PLAN_VAL(PX_DT, PX_HDN_SCHEME1, 0, 'MF') INTO VP_VAL_COUNT FROM DUAL;
                    IF VP_VAL_COUNT = 0 THEN
                        OPEN PX_CURSOR FOR SELECT 'ERROR: LI premium for combo plan can''t exceed 6.70 Lacs OR plan must be of combo' AS MSG FROM DUAL;
                        RETURN;
                    END IF;
                END;
            END IF;

            -- validation of not to punch in Unallocated Branch and uploaded client , rm, branch 
            IF PX_BRANCH IS NOT NULL AND PX_BRANCH <> '' AND PX_BRANCH <> 0 THEN
                DECLARE
                    V_BRANCH_COUNT NUMBER;
                BEGIN
                    SELECT COUNT(*)
                    INTO V_BRANCH_COUNT
                    FROM WEALTHMAKER.BRANCH_MASTER
                    WHERE BRANCH_CODE = PX_BRANCH
                    AND BRANCH_NAME LIKE 'UNALLO%';

                    IF V_BRANCH_COUNT > 0 THEN
                        OPEN PX_CURSOR FOR SELECT 'ERROR: Transaction can not be punched in Unallocated Branch. ' AS MSG FROM DUAL;
                        RETURN;
                    END IF;
                END;
            END IF;

            -- VALIDATE UPLOADED CLIENT DT
            IF PX_DT IS NOT NULL THEN
                DECLARE
                    VP_RESULT  NUMBER;
                    VP_ERRMSG  VARCHAR2(200);
                BEGIN
                    -- Call the stored function/procedure
                    WEALTHMAKER.FN_VALIDATE_CLIENT_DT(
                        PDT_NO      => PX_DT,
                        PCLIENT_CODE => PX_INV_CODE,
                        RESULT      => VP_RESULT,
                        MESSAGE     => VP_ERRMSG
                    );

                    -- If result = 0, then validation failed
                    IF NVL(VP_RESULT, 0) = 0 THEN
                        OPEN PX_CURSOR FOR
                            SELECT VP_ERRMSG AS MSG
                            FROM DUAL;
                        RETURN;
                    END IF;

                END;
            END IF;

            -- VALIDATE UPLOADED BRANCH DT
            IF PX_BRANCH IS NOT NULL THEN
                DECLARE
                    VP_RESULT  NUMBER;
                    VP_ERRMSG  VARCHAR2(200);
                BEGIN
                    IF PX_BSS_CODE IS NOT NULL AND PX_BSS_CODE <> '' THEN
                        -- Call the validation procedure
                        WEALTHMAKER.FN_VALIDATE_BRANCH_RM_DT(
                            PDT_NO       => PX_DT,
                            PBRANCH_CODE => PX_BRANCH,
                            PPAYROLL_ID  => TRIM(PX_BSS_CODE),
                            RESULT       => VP_RESULT,
                            MESSAGE      => VP_ERRMSG
                        );

                        -- Check validation result
                        IF NVL(VP_RESULT, 0) = 0 THEN
                            OPEN PX_CURSOR FOR
                                SELECT VP_ERRMSG AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;
                END;
            END IF;

            -- VALIDATE FP STATUS
            IF PX_DT IS NOT NULL AND PX_INV_CODE IS NOT NULL  THEN 
                DECLARE
                    vP_fp_status NUMBER;
                BEGIN
                    IF PX_DT IS NOT NULL AND PX_DT <> '' 
                    AND PX_INV_CODE IS NOT NULL AND PX_INV_CODE <> '' THEN

                        -- Call the function and store the result
                        SELECT fp_status_check1(PX_INV_CODE, PX_DT)
                        INTO vP_fp_status
                        FROM dual;

                        -- If status = 0, return error
                        IF NVL(vP_fp_status, 0) = 0 THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'Please create Financial planning for the investor first' AS MSG
                                FROM dual;
                            RETURN;
                        END IF;
                    END IF;
                END;
                
                SELECT NUL FROM DUAL;
            END IF;

            -- VALIDATE DOCUMENT NUMBER IN transaction_st AND transaction_sttemp
            IF PX_DT IS NOT NULL THEN
                DECLARE
                    VP_EXISTS NUMBER;
                BEGIN
                    IF PX_DT IS NOT NULL AND PX_DT <> '' THEN
                        -- Check in transaction_st
                        SELECT COUNT(*)
                        INTO VP_EXISTS
                        FROM TRANSACTION_ST
                        WHERE DOC_ID = PX_DT;

                        IF VP_EXISTS > 0 THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'Document number already assigned.' AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;

                        -- Check in transaction_sttemp
                        SELECT COUNT(*)
                        INTO VP_EXISTS
                        FROM TRANSACTION_STTEMP
                        WHERE DOC_ID = PX_DT;

                        IF VP_EXISTS > 0 THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'Document number already assigned.' AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;
                END;
            END IF;

            -- SAVE VALIDATION MODIVICATIN VALIDATION

            IF PX_TR_DATE IS NOT NULL THEN
                DECLARE
                    VP_ORG_DT      DATE;
                    VP_COUNT       NUMBER;
                    VP_FRSAVE      NUMBER;
                    VP_FRMODY      NUMBER;
                    VP_HAVE_TR     VARCHAR2(20) := '';
                    VP_GLB_PRV_DT  DATE;
                    VP_GLB_NEX_DT  DATE;
                    VP_ENTRY_DT    DATE:= TO_DATE(PX_TR_DATE,'DD/MM/YYYY');
                BEGIN
                    VP_FRSAVE := 1; -- 1 FOR SAVE
                    VP_FRMODY := 0; -- 0 FOR MODIFY
                    VP_HAVE_TR := '';

                    VP_GLB_PRV_DT:= NULL;
                    VP_GLB_NEX_DT := NULL;
                    VP_ORG_DT:=NULL;


                    -- Get original date if tranCode is provided
                    IF VP_HAVE_TR IS NOT NULL AND VP_HAVE_TR <> '' THEN
                        SELECT TR_DATE
                        INTO VP_ORG_DT
                        FROM TRANSACTION_MF_TEMP1
                        WHERE TRAN_CODE = VP_HAVE_TR;
                    END IF;

                    -- Case 1: Saving a new record
                    IF VP_FRSAVE >= 1 AND VP_FRMODY = 0 THEN
                        IF (VP_ENTRY_DT < VP_GLB_PRV_DT) OR (VP_ENTRY_DT > VP_GLB_NEX_DT) THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'Security restrictions for date range' AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;

                    -- Case 2: Modifying an existing record
                    IF VP_FRSAVE = 0 AND VP_FRMODY = 1 THEN
                        IF (VP_ORG_DT < P_GLBUP_PREVIOUSDATE) OR (VP_ORG_DT > P_GLBUP_NEXTDATE) THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'Security restrictions for date range' AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;

                        SELECT COUNT(*)
                        INTO VP_COUNT
                        FROM WEALTHMAKER.ANATRANDETAILTABLE_NEW_ALL_VB
                        WHERE TRAN_CODE = VP_HAVE_TR;

                        IF VP_COUNT > 0 THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'ANG Bills generated for this transaction. You can not modify the payout of this AR.' AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;

                    -- If all checks pass, continue
                END;

            END IF;

            -- CANNT PUNCH IN FUTURE AND CHQ_DT CANNT BE A MONTH
            IF PX_TR_DATE IS NOT NULL AND PX_CHEQUE_DATE IS NOT NULL THEN
                DECLARE
                    VP_SERVER_DT   DATE := TRUNC(SYSDATE);
                    VP_IM_ENTRY_DT DATE := TO_DATE(PX_TR_DATE,'DD/MM/YYYY');
                    VPX_CHEQUE_DATE      DATE := TO_DATE(PX_CHEQUE_DATE,'DD/MM/YYYY'); 
                    VP_MON_DIFF    NUMBER;
                    RESULT         NUMBER;
                BEGIN
                    -- 1) Prevent punching transaction in advance
                    IF (VP_IM_ENTRY_DT - VP_SERVER_DT) >= 1 THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'You can not punch transaction in advance.' AS MSG
                                FROM DUAL;
                            RETURN;
                    END IF;

                    -- 2) Cheque date validation (only if date is not null)
                    IF VPX_CHEQUE_DATE IS NOT NULL THEN
                        VP_MON_DIFF := MONTHS_BETWEEN(TRUNC(VPX_CHEQUE_DATE), VP_SERVER_DT);

                        IF VP_MON_DIFF > 1 THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'You can not give the cheque greater than one month' AS MSG
                                FROM DUAL;
                            RETURN;
                        END IF;
                    END IF;
                END;
            END IF;

            -- VALID DT TRAN TYPE 
            IF PX_DT IS NOT NULL THEN
                DECLARE
                    V_COUNT NUMBER;
                BEGIN
                    SELECT COUNT(*)
                    INTO V_COUNT
                    FROM TB_DOC_UPLOAD
                    WHERE COMMON_ID = TRIM(PX_DT)
                    AND TRAN_TYPE = 'MF';

                    IF V_COUNT = 0 THEN
                        OPEN PX_CURSOR FOR
                            SELECT 'Please enter a valid DT Number' AS MSG
                            FROM DUAL;
                        RETURN;
                    END IF;
                END;                
            END IF;


            -- PX_TRANSACTION_TYPE AND PAYMENT_MOTH 
            IF PX_TRANSACTION_TYPE IS NOT NULL THEN
                IF PX_TRANSACTION_TYPE = 'PURCHASE' AND PX_PAYMENT_MODE NOT IN ('CHEQUE', 'DRAFT', 'ECS', 'RTGS', 'FT') THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Please Select Cheque/Draft/ECS/RTGS/Fund Transfer From Payment Mode' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            ELSE
                OPEN PX_CURSOR FOR
                    SELECT 'Please Select Transaction Type' AS MSG
                    FROM DUAL;
                RETURN;
            END IF;


            IF PX_TRANSACTION_TYPE = 'PURCHASE' THEN
                IF PX_SIP_STP IS NULL  THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Please Select SIP Type' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            --FREQ AND INSTALLMENTS
            IF PX_SIP_STP IN ('SIP','STP') THEN
                IF (TRIM(PX_FREQUENCY) IS NULL OR TRIM(PX_INSTALLMENTS_NO) IS NULL) THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Please Enter Frequency Type and No. Of Installments' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;


            -- SIP END DT AND SIP AMOUNT
            IF PX_SIP_STP IN ('SIP') THEN
                IF TRIM(PX_SIP_END_DATE) IS NULL THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Please Enter SIP End Date' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;

                --IS_NUMERIC(PX_SIP_AMOUNT)
                IF TRIM(PX_SIP_AMOUNT) IN ('', '0') OR PX_SIP_AMOUNT IS NULL  THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Please Enter Valid SIP Amount' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;

                IF TRIM(PX_FRESH_RENEWAL) NOT IN ('F', 'R') THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Please select either Fresh or Renewal' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            END IF;


            -- PAYMENT MODE NUMBER ADN DT VALIDATION
            IF TRIM(PX_FRESH_RENEWAL) IN ('F') THEN
                IF  TRIM(PX_PAYMENT_MODE) IN ('CHEQUE', 'DRAFT', 'ECS', 'RTGS', 'FT') THEN

                    IF  TRIM(PX_CHEQUE_NO) IS NULL THEN
                        OPEN PX_CURSOR FOR
                            SELECT 'Please Fill Cheque/Draft/MICR/UTR/Bank A/c No. ' AS MSG
                            FROM DUAL;
                        RETURN;
                    END IF;

                    IF TRIM(PX_CHEQUE_DATE) IS NULL THEN        
                        OPEN PX_CURSOR FOR
                            SELECT 'Cheque/Draft/ECS/UTR/Fund Transfer Can Not be Left Blank. ' AS MSG
                            FROM DUAL;
                        RETURN;
                    END IF;
                END IF;
            END IF;


        IF TRIM(PX_TRANSACTION_TYPE)  = 'SWITCH IN' AND TRIM(PX_PAYMENT_MODE) IN ('CHEQUE', 'DRAFT', 'ECS', 'CASH', 'RTGS', 'FT') THEN
            OPEN PX_CURSOR FOR
                SELECT 'Please Select Other Option ' AS MSG
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(PX_AH_NAME) <>'' THEN
            OPEN PX_CURSOR FOR
                SELECT 'Please Fill Investor Name' AS MSG
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(PX_BSS_CODE) <>'' THEN
            OPEN PX_CURSOR FOR
                SELECT 'Please Fill Business Code A' AS MSG
                FROM DUAL;
            RETURN;
        END IF;

        IF PX_AMC IS NULL THEN
            OPEN PX_CURSOR FOR
                SELECT 'Please Select AMC' AS MSG
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(PX_TRANSACTION_TYPE) = 'PURCHASE' THEN
            IF TRIM(PX_APP_NO) IS NOT NULL THEN
                IF LENGTH(PX_APP_NO) < 6 THEN
                    OPEN PX_CURSOR FOR
                        SELECT ' Minimum Length Of App No Should Be Greater or Equal To 6 ' FROM DUAL;
                        RETURN;
                ELSIF PX_APP_NO = '000000' THEN
                    OPEN PX_CURSOR FOR
                        SELECT ' Please Enter A Valid App No ' FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF TRIM(PX_HDN_SCHEME1) IS NULL THEN
            OPEN PX_CURSOR FOR
                SELECT 'Select The Scheme' AS MSG
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(PX_TR_DATE) IS NULL THEN
            OPEN PX_CURSOR FOR
                SELECT 'Transaction Date Can Not Be Left Blank' AS MSG
                FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(PX_CLIENT_CODE) IS NULL THEN
            OPEN PX_CURSOR FOR
                SELECT 'Client Code Can Not Left Blank' AS MSG
                FROM DUAL;
            RETURN;
        END IF;


        IF PX_AMOUNT IS NOT NULL THEN
            DECLARE
                VP_AMOUNT NUMBER;
            BEGIN
                BEGIN
                    -- Try converting to number
                    VP_AMOUNT := TO_NUMBER(PX_AMOUNT);
                EXCEPTION
                    WHEN VALUE_ERROR THEN
                        -- If conversion fails, treat as invalid
                        OPEN PX_CURSOR FOR
                            SELECT 'Amount must be a valid number' AS MSG
                            FROM DUAL;
                        RETURN;
                END;

                -- Now check if it's zero
                IF NVL(VP_AMOUNT, 0) = 0 THEN
                    OPEN PX_CURSOR FOR SELECT 'Amount cannot be zero' AS MSG FROM DUAL;
                    RETURN;
                END IF;
            END;
        ELSE
            OPEN PX_CURSOR FOR SELECT 'Amount cannot be null' AS MSG FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(PX_BRANCH) <>'' THEN
            OPEN PX_CURSOR FOR
                SELECT 'Branch Can Not Left Blank' AS MSG
                FROM DUAL;
        END IF;

        IF TRIM(PX_HDN_SCHEME1) IS NULL THEN
            OPEN PX_CURSOR FOR
                SELECT 'Select Scheme First' AS MSG
                FROM DUAL;
            RETURN;
        END IF;
            

        IF PX_TRANSACTION_TYPE = 'SWITCH IN' OR PX_SIP_STP = 'STP' THEN
            -- Validate Switch Scheme
            IF TRIM(PX_HDN_SCHEME2_SWITCH) IS NULL THEN
                OPEN PX_CURSOR FOR
                    SELECT 'Select the Scheme you have Switched From' AS MSG
                    FROM DUAL;
                RETURN;
            END IF;

            -- Validate Switch Folio
            IF TRIM(PX_FROM_SWITCH_FOLIO) IS NULL THEN
                OPEN PX_CURSOR FOR
                    SELECT 'Select the Folio you have Switched From' AS MSG
                    FROM DUAL;
                RETURN;
            END IF;

            -- Check if Switch From and To schemes are the same
            IF PX_HDN_SCHEME1 = PX_HDN_SCHEME2_SWITCH THEN
                OPEN PX_CURSOR FOR
                    SELECT 'In case of Switch transaction, Switch from Scheme cannot be same as Switch to Scheme' AS MSG
                    FROM DUAL;
                RETURN;
            END IF;

            declare V_COUNT number;
            begin
                -- Check if both schemes belong to the same AMC
                SELECT COUNT(DISTINCT MUT_CODE)
                INTO V_COUNT
                FROM SCHEME_INFO
                WHERE SCH_CODE IN (PX_HDN_SCHEME1, PX_HDN_SCHEME2_SWITCH);

                IF V_COUNT > 1 THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'In case of switch Transaction, Switch from Scheme and Switch to Scheme should be from one AMC only' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            end;
        END IF;
  
        --Cross Checking Of Pan With Account Master
        IF PX_HDN_SCHEME1 IS NOT NULL THEN
            DECLARE
                V_DOB           DATE;
                V_IS_MINOR      BOOLEAN := FALSE;
                V_PAN1          VARCHAR2(20);
                V_INV_CD        VARCHAR2(20);
                V_ACCOUNT_PAN   NUMBER;
                V_CATEGORY      VARCHAR2(10);
            BEGIN
                -- Skip PAN validation for MICROSIP under SIP
                IF NOT (PX_TRANSACTION_TYPE = 'SIP' AND P_SUB_SIP = 'MICROSIP') THEN
                    
                    -- Micro PAN validation
                    IF PX_HDN_SCHEME1 <> 'OP#10826' THEN
                        IF VP_MICRO_PAN_FLAG = 0 THEN
                        IF TRIM(PX_PAN2) IS NULL OR NOT REGEXP_LIKE(PX_HDN_PAN1, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                            IF NOT REGEXP_LIKE(PX_PAN2, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                                OPEN PX_CURSOR FOR
                                    SELECT 'Please enter a valid PAN number' AS MSG FROM DUAL;
                                RETURN;
                            END IF;
                        END IF;
                        END IF;
                    END IF;

                    -- Minor check for investor starting with '3'
                    IF PX_HDN_SCHEME1 <> 'OP#10826' AND SUBSTR(PX_INV_CODE, 1, 1) = '3' THEN
                        SELECT NVL(DOB, SYSDATE - 10000)
                        INTO V_DOB
                        FROM INVESTOR_MASTER
                        WHERE INV_CODE = PX_INV_CODE;

                        IF V_DOB IS NOT NULL AND MONTHS_BETWEEN(SYSDATE, V_DOB) / 12 <= 18 THEN
                            V_IS_MINOR := TRUE;
                        END IF;

                        -- PAN match for non-minor
                        IF NOT V_IS_MINOR AND VP_MICRO_PAN_FLAG = 0 THEN
                            SELECT UPPER(PAN)
                            INTO V_PAN1
                            FROM INVESTOR_MASTER
                            WHERE INV_CODE = PX_INV_CODE;

                            IF V_PAN1 IS NOT NULL AND V_PAN1 <> '0' THEN
                                IF V_PAN1 <> UPPER(PX_PAN2) THEN
                                    OPEN PX_CURSOR FOR
                                        SELECT 'PAN entered does not match with the selected investor''s PAN' AS MSG FROM DUAL;
                                    RETURN;
                                END IF;
                            END IF;
                        END IF;
                    END IF;

                    -- Minor check for investor starting with '4'
                    IF PX_HDN_SCHEME1 <> 'OP#10826' AND SUBSTR(PX_INV_CODE, 1, 1) = '4' THEN
                        SELECT NVL(DOB, TO_DATE('01/01/1800', 'DD/MM/YYYY'))
                        INTO V_DOB
                        FROM CLIENT_TEST
                        WHERE CLIENT_CODEKYC = PX_INV_CODE;

                        IF V_DOB = TO_DATE('01/01/1800', 'DD/MM/YYYY') THEN
                            OPEN PX_CURSOR FOR
                                SELECT 'Please fill Date of Birth for this investor' AS MSG FROM DUAL;
                            RETURN;
                        END IF;

                        SELECT NVL(INVESTOR_CODE, '1')
                        INTO V_CATEGORY
                        FROM CLIENT_MASTER
                        WHERE CLIENT_CODE = SUBSTR(PX_INV_CODE, 1, 8);

                        IF V_DOB IS NOT NULL AND MONTHS_BETWEEN(SYSDATE, V_DOB) / 12 <= 18 THEN
                            V_IS_MINOR := (V_CATEGORY = '1');
                        END IF;

                        -- PAN match logic
                        IF NOT V_IS_MINOR THEN
                            IF VP_MICRO_PAN_FLAG = 0 THEN
                                SELECT CLIENT_CODEKYC
                                INTO V_INV_CD
                                FROM CLIENT_TEST
                                WHERE UPPER(CLIENT_PAN) = UPPER(PX_PAN2);

                                IF V_INV_CD IS NOT NULL AND V_INV_CD <> PX_INV_CODE THEN
                                    OPEN PX_CURSOR FOR
                                        SELECT 'PAN entered does not match with the selected investor''s PAN' AS MSG FROM DUAL;
                                    RETURN;
                                END IF;

                                SELECT COUNT(*)
                                INTO V_ACCOUNT_PAN
                                FROM CLIENT_TEST
                                WHERE CLIENT_CODEKYC = PX_INV_CODE
                                AND (UPPER(CLIENT_PAN) = UPPER(PX_PAN2) OR CLIENT_PAN IS NULL);

                                IF V_ACCOUNT_PAN = 0 THEN
                                    OPEN PX_CURSOR FOR
                                        SELECT 'PAN entered does not match with the selected investor''s PAN' AS MSG FROM DUAL;
                                    RETURN;
                                END IF;
                            END IF;
                        ELSE
                            -- Minor PAN match via guardian PAN
                            IF VP_MICRO_PAN_FLAG = 0 THEN
                                SELECT NVL(UPPER(G_PAN), '0')
                                INTO V_PAN1
                                FROM CLIENT_TEST
                                WHERE CLIENT_CODEKYC = PX_INV_CODE;

                                IF V_PAN1 IS NOT NULL AND V_PAN1 <> '0' AND V_PAN1 <> UPPER(PX_PAN2) THEN
                                    OPEN PX_CURSOR FOR
                                        SELECT 'PAN entered does not match with the guardian''s PAN' AS MSG FROM DUAL;
                                    RETURN;
                                END IF;

                                SELECT COUNT(*)
                                INTO V_ACCOUNT_PAN
                                FROM CLIENT_TEST
                                WHERE CLIENT_CODEKYC = PX_INV_CODE
                                AND (UPPER(G_PAN) = UPPER(PX_PAN2) OR G_PAN IS NULL);

                                IF V_ACCOUNT_PAN = 0 THEN
                                    OPEN PX_CURSOR FOR
                                        SELECT 'PAN entered does not match with the guardian''s PAN' AS MSG FROM DUAL;
                                    RETURN;
                                END IF;
                            END IF;
                        END IF;
                    END IF;

                    -- Final fallback: copy PAN if missing
                    IF TRIM(PX_HDN_PAN1) IS NULL OR NOT REGEXP_LIKE(PX_HDN_PAN1, '^[A-Z]{5}[0-9]{4}[A-Z]{1}$') THEN
                        V_PX_HDN_PAN1 := PX_PAN2;
                    END IF;

                END IF;
            END;
        END IF;
        
        -- CHECK ATM TRANSACTIONS
        IF V_CHK_ATM_TR IS NOT NULL THEN
            DECLARE
            V_MIN_AMOUNT NUMBER;
            BEGIN
                -- Check if ATM transaction is selected
                IF V_CHK_ATM_TR = '1' THEN

                    -- Fetch minimum amount for the scheme
                    SELECT MIN_AMOUNT
                    INTO V_MIN_AMOUNT
                    FROM RELIANCE_ATM_MASTER
                    WHERE SCH_CODE = PX_HDN_SCHEME1
                    AND FROM_DT <= SYSDATE
                    AND (TO_DT >= SYSDATE OR TO_DT IS NULL)
                    AND ROWNUM = 1;

                    -- Compare with entered amount
                    IF NVL(PX_AMOUNT, 0) < V_MIN_AMOUNT THEN
                        OPEN PX_CURSOR FOR
                            SELECT 'Minimum amount condition to have Reliance ATM Card is not being fulfilled' AS MSG
                            FROM DUAL;
                        RETURN;
                    END IF;

                END IF;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Minimum amount condition to have Reliance ATM Card is not being fulfilled' AS MSG
                        FROM DUAL;
                    RETURN;
            END;
        END IF;

        -- APP NUMBER VERIFY
        IF PX_APP_NO IS NOT NULL AND PX_AMC IS NOT NULL THEN
            DECLARE
                VP_EXISTS NUMBER;
            BEGIN
                -- Check if transaction already exists for the given App No and AMC Code
                SELECT COUNT(*)
                INTO VP_EXISTS
                FROM TRANSACTION_MF_TEMP1
                WHERE MUT_CODE = PX_AMC
                AND APP_NO = PX_APP_NO
                AND (ASA <> 'C' OR ASA IS NULL);

                -- If record exists, show message and exit
                IF VP_EXISTS > 0 THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Sorry, this App No has already been punched in this company' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            END;
        END IF;

        -- Received
        IF PX_AMC IS NOT NULL AND PX_HDN_SCHEME1 IS NOT NULL AND PX_TR_DATE IS NOT NULL THEN
            DECLARE
                V_RECD_TOTAL NUMBER := 0;
            BEGIN
                -- Calculate total receivable using two functions
                SELECT NVL(UPFRONT_RECD_NONANG_DETAIL(PX_AMC, PX_HDN_SCHEME1, TO_DATE(PX_TR_DATE, 'MM/DD/YYYY'), PX_AMOUNT, PX_TRANSACTION_TYPE, MYSWITCHSCHCODEA), 0)
                    + NVL(TRAIL_RECD_ANG_DETAIL(PX_AMC, PX_HDN_SCHEME1, TO_DATE(PX_TR_DATE, 'MM/DD/YYYY'), PX_AMOUNT), 0)
                INTO V_RECD_TOTAL
                FROM DUAL;

                -- Compare with entered expenses
                IF NVL(PX_EXP_RS, 0) > V_RECD_TOTAL AND V_RECD_TOTAL <> 0 THEN
                    OPEN PX_CURSOR FOR
                        SELECT 'Payable cannot be greater than Receivable' AS MSG
                        FROM DUAL;
                    RETURN;
                END IF;
            END;
        END IF;

        IF PX_PAYMENT_MODE IN ('C', 'D', 'E', 'U','B') AND PX_FRESH_RENEWAL = 'F' THEN
            INSERT INTO transaction_mf_Temp1 (
                ATM_FLAG,SIP_AMOUNT,CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE,
                APP_NO,SIP_START_DATE,Pan,FOLIO_NO,SWITCH_FOLIO,SWITCH_SCHEME,PAYMENT_MODE,BANK_NAME,CHEQUE_NO,CHEQUE_DATE,
                AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,TIMEST,SIP_END_DATE,
                sip_fr,dispatch,doc_id,micro_investment,target_switch_scheme,cob_flag,SWP_flag,FREEDOM_SIP_FLAG
                ) VALUES(
                    V_CHK_ATM_TR,PX_SIP_AMOUNT, PX_INV_CODE, PX_BSS_CODE, PX_LOG_ID, PX_BSS_CODE, PX_BRANCH, PX_HDN_PAN1, PX_AMC, PX_HDN_SCHEME1, TO_DATE(PX_TR_DATE, 'DD/MM/YYYY'),PX_TRANSACTION_TYPE,
                    PX_APP_NO, NVL(TO_DATE(PX_SIP_START_DATE,'DD/MM/YYYY'), NULL),PX_PAN2, PX_FOLIO_NO, PX_FROM_SWITCH_FOLIO, PX_HDN_SCHEME2_SWITCH, PX_PAYMENT_MODE, PX_BANK_NAME, PX_CHEQUE_NO, PX_CHEQUE_DATE,
                    PX_AMOUNT, PX_SIP_STP, '', PX_CLIENT_CODE, PX_AH_NAME, PX_EXP_PER, PX_EXP_RS, PX_AH_CODE, PX_FREQUENCY, PX_INSTALLMENTS_NO, SYSDATE,  NVL(TO_DATE(PX_SIP_END_DATE,'DD/MM/YYYY'), NULL),
                    PX_FRESH_RENEWAL, PX_REGULAR_NFO, PX_DT_NUMBER,PX_SIP_TYPE, PX_HDN_SCHEME3_CLOSE, PX_COB_CASE, PX_SWP_CASE, PX_FREEDOM_SIP 
                );
        ELSE
            INSERT INTO transaction_mf_Temp1 (
                ATM_FLAG,SIP_AMOUNT,CLIENT_CODE,BUSINESS_RMCODE,LOGGEDUSERID,CLIENT_OWNER, BUSI_BRANCH_CODE,PANNO,MUT_CODE,SCH_CODE,TR_DATE,TRAN_TYPE,
                APP_NO,SIP_START_DATE,Pan,FOLIO_NO,SWITCH_FOLIO,SWITCH_SCHEME,PAYMENT_MODE,
                AMOUNT,SIP_TYPE,LEAD_NAME,SOURCE_CODE,INVESTOR_NAME,EXP_RATE,EXP_AMOUNT,AC_HOLDER_CODE,frequency,installments_no,TIMEST,SIP_END_DATE,
                sip_fr,dispatch,doc_id,micro_investment,target_switch_scheme,cob_flag,SWP_flag,FREEDOM_SIP_FLAG
                ) VALUES(
                    V_CHK_ATM_TR,PX_SIP_AMOUNT, PX_INV_CODE, PX_BSS_CODE, PX_LOG_ID, PX_BSS_CODE, PX_BRANCH, PX_HDN_PAN1, PX_AMC, PX_HDN_SCHEME1, TO_DATE(PX_TR_DATE, 'DD/MM/YYYY'),PX_TRANSACTION_TYPE,
                    PX_APP_NO, NVL(TO_DATE(PX_SIP_START_DATE,'DD/MM/YYYY'), NULL),PX_PAN2, PX_FOLIO_NO, PX_FROM_SWITCH_FOLIO, PX_HDN_SCHEME2_SWITCH, PX_PAYMENT_MODE,
                    PX_AMOUNT, PX_SIP_STP, '', PX_CLIENT_CODE, PX_AH_NAME, PX_EXP_PER, PX_EXP_RS, PX_AH_CODE, PX_FREQUENCY, PX_INSTALLMENTS_NO, SYSDATE,  NVL(TO_DATE(PX_SIP_END_DATE,'DD/MM/YYYY'), NULL),
                    PX_FRESH_RENEWAL, PX_REGULAR_NFO, PX_DT_NUMBER,PX_SIP_TYPE, PX_HDN_SCHEME3_CLOSE, PX_COB_CASE, PX_SWP_CASE, PX_FREEDOM_SIP 
                );
        END IF;

        Declare 
            v_ret_msg varchar2(1000):='Current transaction has been recorded successfully';

            v_GENERATED_TR varchar2(1000);



        begin

        select max(tran_code) INTO v_GENERATED_TR from transaction_mf_Temp1 where BUSINESS_RMCODE=PX_BSS_CODE
        and PANNO = PX_HDN_PAN1 OR PX_HDN_PAN1 IS NULL

        and PX_PAYMENT_MODE IN  ('C', 'D', 'E', 'U','B') AND CHEQUE_NO =  PX_CHEQUE_NO OR  IS NULL
and APP_NO = PX_APP_NO OR PX_APP_NO IS NULL AND ROWNUM = 1;

                    OPEN PX_CURSOR FOR
                        SELECT 'SUCCESS' AS MSG
                        'Current transaction has been recorded successfully' AS MSG1,
                        'Your ARNo is' || v_GENERATED_TR AS MSG2
                        FROM DUAL;
                    RETURN;


        end;


 















    END IF;

    IF V_RETURN IS NOT NULL THEN
        OPEN PX_CURSOR FOR 
            SELECT 'ERROR: ' || V_RETURN AS MSG FROM DUAL;
        RETURN;
    ELSE 
        OPEN PX_CURSOR FOR 
            SELECT 'SUCCESS: MF PUNCHING TRANSACTION VALIDATION PASS ' AS MSG FROM DUAL;
        RETURN;

        END IF;

    -- Dummy result to test the output (replace with actual SELECT)
    OPEN PX_CURSOR FOR
        SELECT
            PX_DT_NUMBER       AS DT_NUMBER,
            PX_TR_DATE         AS TR_DATE,
            PX_CLIENT_CODE     AS CLIENT_CODE,
            PX_AMOUNT          AS AMOUNT,
            'SUCCESS'          AS STATUS
        FROM DUAL;

EXCEPTION
    WHEN OTHERS THEN
        -- Optional: log or handle error
        OPEN PX_CURSOR FOR
            SELECT 'ERROR' AS STATUS, SQLERRM AS ERROR_MESSAGE FROM DUAL;
END;
