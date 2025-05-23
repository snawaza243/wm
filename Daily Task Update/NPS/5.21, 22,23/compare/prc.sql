CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_NPS_INSERT_UPDATE_PRA(
    P_MARK                      IN VARCHAR2,
    P_PRODUCT_CLASS             IN VARCHAR2,
    P_INVESTOR_TYPE             IN VARCHAR2,
    P_CORPORATE_NAME            IN VARCHAR2,
    P_DT_NUMBER                 IN VARCHAR2,
    P_TRAN_CODE                 IN VARCHAR2,
    P_INVESTOR_CODE             IN NUMBER,
    P_SCHEME_CODE               IN VARCHAR2,
    P_CRA                       IN VARCHAR2,
    P_CRA_BRANCH                IN NUMBER,
    P_FOLIO_NUMBER              IN VARCHAR2,
    P_BUSINESS_RM               IN NUMBER,
    P_BUSINESS_BRANCH           IN NUMBER,
    P_RECEIPT_NO                IN VARCHAR2,
    P_PAYMENT_MODE              IN CHAR,
    P_CHEQUE_NO                 IN VARCHAR2,
    P_BANK_NAME                 IN VARCHAR2,
    P_APP_NO                    IN VARCHAR2,
    P_CHEQUE_DATE               IN DATE,
    P_DATE                      IN DATE,
    P_TIME                      IN DATE,
    P_COMBINED_DATETIME         IN DATE,
    P_SUBSCRIBER_NAME           IN VARCHAR2,
    P_MANUAL_AR_NO              IN VARCHAR2,
    P_UNFREEZE                  IN VARCHAR2,
    P_AMOUNT_T1                 IN NUMBER,
    P_AMOUNT_T2                 IN NUMBER,
    P_RECHARGE1                 IN NUMBER,
    P_RECHARGE2                 IN NUMBER,
    P_GST_TAX                   IN NUMBER,
    P_COLLECTION_AMOUNT         IN NUMBER,
    P_AMOUNT_INVESTED           IN NUMBER,
    P_AMOUNT_INVESTED2          IN NUMBER,
    P_REMARK                    IN VARCHAR2,
    P_ZERO_COM                  IN VARCHAR2,
    P_LOGGEDIN_USER             IN VARCHAR2,
    P_ROLE_ID                   IN VARCHAR2,    
    P_FRM_SAVE                  IN VARCHAR2,
    P_FRM_MODIFY                IN VARCHAR2,

    P_RESULT                    OUT SYS_REFCURSOR
) AS
    MYTRANCODE                  VARCHAR2(100);
    MYGSTNO                     VARCHAR2(100);
    NEWCLIENTCODE               VARCHAR2(100);
    MYMUTCODE                   VARCHAR2(100);
    CURRENTROLE                 NUMBER;
    TEMPROLE                    NUMBER;
    V_FLAG_NPS_TRAN             NUMBER;
    V_GENERATED_TR              NUMBER;
    MyTranCode1                 VARCHAR2(100);
    MySecReq                    VARCHAR2(100);
    GLBROLEID                   NUMBER:=0;
    GLBROLEID2                  NUMBER:=0;
    V_COUNT_FATCA               NUMBER;     -- FACTA VALIDATION
    V_CLIENT_CAT_DUP_CHEQUE     VARCHAR2(10);
    V_COUNT_TRAN_DUP_CHEQUE     NUMBER;
    V_Busi_Rm_Cd                VARCHAR2(100):= NULL;
    V_Busi_Branch_Cd            VARCHAR2(100):= NULL;
    V_TRAN_CODE_INSERTION       VARCHAR2(100):= NULL;
    V_GST_NO_INSERTION          VARCHAR2(100):= NULL;
    V_ClientBranchCode          VARCHAR2(100):= NULL;
    V_ClientRmCode              VARCHAR2(100):= NULL;
    V_4_tr_date                 VARCHAR2(100):= NULL; 
    V_4_client_Code             VARCHAR2(100):= NULL; 
    V_4_source_code             VARCHAR2(100):= NULL; 
    V_4_BUSI_BRANCH_CODE        VARCHAR2(100):= NULL; 
    V_4_BUSINESS_RMCODE         VARCHAR2(100):= NULL; 
    V_4_mut_code                VARCHAR2(100):= NULL; 
    V_4_sch_code                VARCHAR2(100):= NULL; 
    V_4_amount                  VARCHAR2(100):= NULL; 
    V_4_folio_no                VARCHAR2(100):= NULL; 
    V_4_app_no                  VARCHAR2(100):= NULL; 
    V_4_PAYMENT_MODE            VARCHAR2(100):= NULL; 
    V_4_CHEQUE_DATE             VARCHAR2(100):= NULL; 
    V_4_cheque_no               VARCHAR2(100):= NULL; 
    V_4_BANK_NAME               VARCHAR2(100):= NULL; 
    V_4_manual_arno             VARCHAR2(100):= NULL; 
    V_4_corporate_name          VARCHAR2(100):= NULL; 
    V_4_unique_id               VARCHAR2(100):= NULL; 
    V_4_MODIFY_USER             VARCHAR2(100):= NULL; 
    V_4_MODIFY_DATE             VARCHAR2(100):= NULL; 
    V_COUNT_DUP_TRAN            NUMBER       := 0;
    V_TRAN_CODE_INSERTION       VARCHAR2(50) := NULL;
    V_GST_NO_INSERTION          VARCHAR2(50) := NULL;
    v_count_fam_h               NUMBER       :=0;
    v_found_fam_h               VARCHAR(20)  :=NULL;
    V_null_value                VARCHAR2(100):= NULL;
    v_return_message            VARCHAR2(200):= NULL;

    -- CHECK SAVE VALIDATION VARIABLES
    CSV_GEt_busiRMCode          VARCHAR2(100):=NULL;
    CSV_MyRs_validate           VARCHAR2(100):=NULL;
    CSV_manual_arno             VARCHAR2(100):=NULL;
    CSV_MyRs_validate1          VARCHAR2(100):=NULL;

    ServerDateTime              DATE         := SYSDATE; 
    ins_next_day                NUMBER;
    Glbins_nextdate             DATE;

    -- Variables for role_master columns
    V_UP_PRE_DUR                NUMBER;
    V_UP_NEXT_DUR               NUMBER;
    V_IN_PRE_DUR                NUMBER;
    V_IN_NEXT_DUR               NUMBER;
    V_UP_PRE_DUR_TYPE           NUMBER;
    V_UP_NEXT_DUR_TYPE          NUMBER;
    V_IN_PRE_DUR_TYPE           NUMBER;
    V_IN_NEXT_DUR_TYPE          NUMBER;

    -- Date calculation variables
    V_GLBINS_PREVIOUSDATE       DATE;
    V_GLBINS_NEXTDATE           DATE;
    V_GLBUP_PREVIOUSDATE        DATE;
    V_GLBUP_NEXTDATE            DATE;

    V_FY_START                  DATE;
    V_FY_END                    DATE;
    V_ACTUAL_DATE               DATE;
    ServerDateTime              DATE    := SYSDATE;
    chkSaveValidation           BOOLEAN := TRUE;

    V_MyTranCode                VARCHAR2(100):= NULL;
    V_MyGSTNO                   VARCHAR2(100):=NULL;
    V_MyTranCode1               VARCHAR2(100):=NULL;
    V_MySecReq                  VARCHAR2(100):=NULL; 

BEGIN

BEGIN -- Fetch the fiscal year start and end dates
    SELECT UP_PRE_DUR, UP_NEXT_DUR, IN_PRE_DUR, IN_NEXT_DUR,
           UP_PRE_DUR_TYPE, UP_NEXT_DUR_TYPE, IN_PRE_DUR_TYPE, IN_NEXT_DUR_TYPE
    INTO   V_UP_PRE_DUR, V_UP_NEXT_DUR, V_IN_PRE_DUR, V_IN_NEXT_DUR,
           V_UP_PRE_DUR_TYPE, V_UP_NEXT_DUR_TYPE, V_IN_PRE_DUR_TYPE, V_IN_NEXT_DUR_TYPE
    FROM   ROLE_MASTER
    WHERE  ROLE_ID = P_ROLE_ID;

    IF V_IN_PRE_DUR_TYPE = 1 AND V_IN_NEXT_DUR_TYPE = 1 THEN
        V_GLBINS_PREVIOUSDATE := SYSDATE - V_IN_PRE_DUR;
        V_GLBINS_NEXTDATE := SYSDATE + (V_IN_NEXT_DUR - 1);

    ELSIF V_IN_PRE_DUR_TYPE = 1 AND V_IN_NEXT_DUR_TYPE = 2 THEN
        V_GLBINS_PREVIOUSDATE := SYSDATE - V_IN_PRE_DUR;
        V_GLBINS_NEXTDATE := ADD_MONTHS(SYSDATE, V_IN_NEXT_DUR - 1);

    ELSIF V_IN_PRE_DUR_TYPE = 1 AND V_IN_NEXT_DUR_TYPE = 3 THEN
        V_ACTUAL_DATE := SYSDATE - V_IN_PRE_DUR;
        IF V_FY_START >= V_ACTUAL_DATE THEN
            V_GLBINS_PREVIOUSDATE := V_FY_START;
        ELSE
            V_GLBINS_PREVIOUSDATE := V_ACTUAL_DATE;
        END IF;
        V_GLBINS_NEXTDATE := ADD_MONTHS(V_FY_END, 12 * (V_IN_NEXT_DUR - 1)); -- year add
    END IF;



    IF V_UP_PRE_DUR_TYPE = 1 AND V_UP_NEXT_DUR_TYPE = 1 THEN
        V_GLBUP_PREVIOUSDATE := SYSDATE - V_UP_PRE_DUR;
        V_GLBUP_NEXTDATE := SYSDATE + (V_UP_NEXT_DUR - 1);

    ELSIF V_UP_PRE_DUR_TYPE = 2 AND V_UP_NEXT_DUR_TYPE = 2 THEN
        V_GLBUP_PREVIOUSDATE := ADD_MONTHS(SYSDATE, -V_UP_PRE_DUR);
        V_GLBUP_NEXTDATE := ADD_MONTHS(SYSDATE, V_UP_NEXT_DUR - 1);

    -- Add other combinations here
    END IF;
END;

BEGIN -- Fetch ISS_CODE for the provided P_SCHEME_CODE
    BEGIN
        SELECT ISS_CODE
        INTO MYMUTCODE
        FROM OTHER_PRODUCT
        WHERE OSCH_CODE = P_SCHEME_CODE 
        AND ROWNUM = 1;

    EXCEPTION 
        WHEN NO_DATA_FOUND THEN 
            MYMUTCODE := NULL;
    END;
END;

BEGIN -- FIND FAMILY HEAD CODE
    SELECT COUNT(*) INTO v_count_fam_h
    FROM CLIENT_TEST WHERE SOURCE_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8) AND CLIENT_CODE = MAIN_CODE;  

    IF v_count_fam_h > 0 THEN
        SELECT NVL(CLIENT_CODEKYC, NULL) INTO v_found_fam_h 
        FROM CLIENT_TEST WHERE SOURCE_CODE = SUBSTR(P_INVESTOR_CODE,1,8) AND CLIENT_CODE = MAIN_CODE;

    END IF;
END;

BEGIN -- VALIDATE PUNCHING AND MODIFICATION TEAM
     IF P_MARK = 0 THEN -- 0 FOR PUNCING , 4 FOR MODIFYING
        /*'38387' -- (121397/212), (39006/146) */ 

        SELECT COUNT(ROLE_ID) INTO GLBROLEID2 
        FROM USERDETAILS_JI WHERE LOGIN_ID = TRIM(P_LOGGEDIN_USER) 
        AND ROLE_ID IN (212, 1) -- PUNCHING ROLE
        and rownum = 1; 

        IF GLBROLEID2 = 0 THEN
            OPEN P_RESULT FOR SELECT 'Only Punching Team can punch the transaction...' AS STATUS FROM DUAL; RETURN;  
        END IF;

    ELSIF P_MARK = 4 THEN

        SELECT COUNT(ROLE_ID) INTO GLBROLEID2 
        FROM USERDETAILS_JI WHERE LOGIN_ID = TRIM(P_LOGGEDIN_USER)
        AND ROLE_ID IN (146, 1) -- NPS MODIFICATION ROLE
        and rownum = 1;


        IF GLBROLEID2 = 0 THEN
            OPEN P_RESULT FOR 
            SELECT 'Only NPS Team can modify the transaction....' AS STATUS FROM DUAL;
            RETURN; 
        END IF;
    END IF;

END;

BEGIN -- DT NUMBER IS REQUIRED AND RETURN    
    IF TRIM(P_MARK) = '0' AND TRIM(P_DT_NUMBER) IS NULL THEN 
        OPEN P_RESULT FOR SELECT 'DT No cannot be left blank.' AS STATUS FROM DUAL;
        RETURN; 
    END IF;
END;

BEGIN -- FATCA_VALIDATION : P_CORPORATE_NAME, P_UNFREEZE, DELETE FROM NPS_FATCA_NON_COMPLIANT
    IF P_MARK = 0 THEN
        -- Check if corporate name is required
        IF P_INVESTOR_TYPE = 1 THEN 
            IF P_CORPORATE_NAME IS NULL OR TRIM(P_CORPORATE_NAME) = '' THEN
                OPEN P_RESULT FOR SELECT 'Corporate name cannot be left blank.' AS STATUS FROM DUAL;
                RETURN; 
            END IF;
        END IF;

        -- FATCA Non-Compliance Check
        IF P_UNFREEZE = 0 THEN
            IF P_MANUAL_AR_NO IS NOT NULL AND TRIM(P_MANUAL_AR_NO) <> '' THEN
                SELECT COUNT(*)
                INTO V_COUNT_FATCA
                FROM NPS_FATCA_NON_COMPLIANT
                WHERE PRAN_NO = P_MANUAL_AR_NO;

                IF V_COUNT_FATCA >= 1 THEN
                    OPEN P_RESULT FOR SELECT 'FATCA for this PRAN is non-compliant. Please contact the product team.' AS STATUS FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        ELSE
            -- Delete non-compliance record if Unfreeze is checked
            DELETE FROM NPS_FATCA_NON_COMPLIANT WHERE TRIM(PRAN_NO) = TRIM(P_MANUAL_AR_NO);
            COMMIT;
        END IF;
    END IF;
END ;

BEGIN -- CHECK_DUPLICATE_CHEQUE
    IF P_MARK = '0' OR P_MARK = '4' THEN
        -- Fetch Client Category
        SELECT CATEGORY_ID INTO V_CLIENT_CAT_DUP_CHEQUE
        FROM CLIENT_MASTER WHERE CLIENT_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8);

        IF V_CLIENT_CAT_DUP_CHEQUE <> '4004' THEN
            -- Check if transaction code is "0"
            IF P_TRAN_CODE IS NULL THEN
                -- Check for duplicate cheque number in the last 6 months
                SELECT COUNT(*) INTO V_COUNT_TRAN_DUP_CHEQUE 
                FROM (
                    SELECT TRAN_CODE 
                    FROM TRANSACTION_ST 
                    WHERE MUT_CODE = 'IS02520' 
                    AND CHEQUE_NO = TRIM(P_CHEQUE_NO) 
                    AND TR_DATE >= ADD_MONTHS(SYSDATE, -6) 

                    UNION ALL 

                    SELECT TRAN_CODE 
                    FROM TRANSACTION_STTEMP 
                    WHERE MUT_CODE = 'IS02520' 
                    AND CHEQUE_NO = TRIM(P_CHEQUE_NO) 
                    AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                );

                IF V_COUNT_TRAN_DUP_CHEQUE > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!'  AS STATUS FROM DUAL; 
                    RETURN;
                END IF;

            ELSE
                -- If ReqCode = "11"
                IF P_APP_NO = '11' THEN
                    -- Check for duplicate cheque number in the last 6 months, excluding current TRAN_CODE
                    SELECT COUNT(*) INTO V_COUNT_TRAN_DUP_CHEQUE 
                    FROM (
                        SELECT TRAN_CODE 
                        FROM TRANSACTION_ST 
                        WHERE TRAN_CODE <> P_TRAN_CODE
                        AND MUT_CODE = 'IS02520' 
                        AND CHEQUE_NO = TRIM(P_CHEQUE_NO) 
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                        AND REF_TRAN_CODE IS NULL

                        UNION ALL 

                        SELECT TRAN_CODE 
                        FROM TRANSACTION_STTEMP 
                        WHERE MUT_CODE = 'IS02520' 
                        AND CHEQUE_NO = TRIM(P_CHEQUE_NO) 
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6) 
                        AND TRAN_CODE <> P_TRAN_CODE
                    );

                    IF V_COUNT_TRAN_DUP_CHEQUE > 0 THEN
                        --p_Duplicate_Found := 1;
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;

                ELSE
                    -- Check for duplicate cheque number in the last 6 months, excluding current TRAN_CODE
                    SELECT COUNT(*) INTO V_COUNT_TRAN_DUP_CHEQUE 
                    FROM (
                        SELECT TRAN_CODE 
                        FROM TRANSACTION_ST 
                        WHERE TRAN_CODE <> P_TRAN_CODE
                        AND MUT_CODE = 'IS02520' 
                        AND CHEQUE_NO = TRIM(P_CHEQUE_NO) 
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)

                        UNION ALL 

                        SELECT TRAN_CODE 
                        FROM TRANSACTION_STTEMP 
                        WHERE MUT_CODE = 'IS02520' 
                        AND CHEQUE_NO = TRIM(P_CHEQUE_NO) 
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6) 
                        AND TRAN_CODE <> P_TRAN_CODE
                    );

                    IF V_COUNT_TRAN_DUP_CHEQUE > 0 THEN
                        --p_Duplicate_Found := 1;
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;
                END IF;
            END IF;
        END IF;
    END IF;

    -- If no duplicate found, return 0
    -- p_Duplicate_Found := 0;
END ;

BEGIN -- NSDL Branch on P_RECEIPT_NO
    IF P_MARK  <> 3 AND P_MARK  <> 4 THEN
        IF P_RECEIPT_NO IS NULL OR TRIM(P_RECEIPT_NO) = '' THEN
            OPEN P_RESULT FOR SELECT 'Please Select NSDL Branch First!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;
    END IF;
END;

-- ReqCode         is P_APP_NO
-- Busi_Branch_cd  is P_BUSINESS_BRANCH IS
-- Busi_Rm_Cd      is P_BUSINESS_RM

BEGIN -- Get payroll_id BY BUSINESS_RM
    select payroll_id 
    INTO V_Busi_Rm_Cd    
    from employee_master where payroll_id=P_BUSINESS_RM;
END;

-- UNLOAD FORM IF INDEX = 1 AND RETURN

BEGIN -- Get RM Code and Branch Code BY P_INVESTOR_CODE
    If P_INVESTOR_CODE <> "" Then
        select rm_code,branch_code 
        INTO V_ClientBranchCode, V_ClientRmCode
        from investor_master where inv_code=P_INVESTOR_CODE;
    End If
END;

-- CALL CLEAR FORM IF INDEX = 3

BEGIN -- Modify(4): corp, ar on update, checkSaveValidation, update nps_transaction
    IF P_MARK = 4 THEN -- 4 FOR MODIFYING
        IF P_INVESTOR_TYPE = 1 THEN -- IF CORPORATE NAME IS REQUIRED if P_INVESTOR_TYPE = 1
            IF P_CORPORATE_NAME IS NULL OR TRIM(P_CORPORATE_NAME) = '' THEN
                OPEN P_RESULT FOR SELECT 'Corporate name cannot be left blank.' AS STATUS FROM DUAL;
                RETURN; 
            END IF;
        END IF;

        IF P_TRAN_CODE IS NULL OR P_TRAN_CODE = '0' THEN -- TRAN_CODE IS NULL
            OPEN P_RESULT FOR SELECT 'Please select a transaction code to Modify.' AS STATUS FROM DUAL;
            RETURN; 
        END IF;

        -- chkSaveValidation(False, True)
        -- Busi_Branch_cd   ---> P_BUSINESS_BRANCH 
        -- P_PAYMENT_MODE HAVE C, D, H, E, R, M THAT IS PAYMODE

        /*
            select 
            tr_date, client_Code, source_code, BUSI_BRANCH_CODE, 
            BUSINESS_RMCODE, mut_code, sch_code, amount, folio_no, 
            app_no, PAYMENT_MODE, CHEQUE_DATE, cheque_no, BANK_NAME,  
            manual_arno, corporate_name, unique_id, MODIFY_USER, MODIFY_DATE
            into
            V_4_tr_date, V_4_client_Code, V_4_source_code, V_4_BUSI_BRANCH_CODE, 
            V_4_BUSINESS_RMCODE, V_4_mut_code, V_4_sch_code, V_4_amount, V_4_folio_no, 
            V_4_app_no, V_4_PAYMENT_MODE, V_4_CHEQUE_DATE, V_4_cheque_no, V_4_BANK_NAME, 
            V_4_ manual_arno, V_4_corporate_name, V_4_unique_id, V_4_MODIFY_USER, V_4_MODIFY_DATE
            from transaction_st where tran_code=P_TRAN_CODE;
        */
    
        update nps_transaction set 
            amount1=P_AMOUNT_T1,
            amount2=P_AMOUNT_T2,
            REG_CHARGE=P_RECHARGE1,
            Tran_CHARGE=P_RECHARGE2,
            SERVICETAX=P_GST_TAX,
            remark=P_REMARK
            where tran_code=P_TRAN_CODE;
        COMMIT;

        v_return_message := 'Transaction Updated Successfully';

    END IF;
END;

BEGIN -- SAVE(0) : CHECK_DUPLICATE_CHEQUE, Vclientcategory
    IF P_MARK = '0' THEN
        BEGIN -- chkSaveValidation
            IF P_PRODUCT_CLASS IS NULL OR TRIM(P_PRODUCT_CLASS) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Product Class.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            -- P_MANUAL_AR_NO is MutCode
            IF P_MANUAL_AR_NO IS NULL OR TRIM(P_MANUAL_AR_NO) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Product.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_SCHEME_CODE IS NULL OR TRIM(P_SCHEME_CODE) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Scheme Code.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_INVESTOR_CODE IS NULL OR TRIM(P_INVESTOR_CODE) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Investor.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            -- P_APP_NO is Request Id or ReqCode
            IF P_APP_NO IS NULL OR TRIM(P_APP_NO) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please Select a Request Id.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_DATE IS NULL THEN
                OPEN P_RESULT FOR SELECT 'Please enter a correct Transaction Date.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;


            IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE('01/01/1980', 'DD/MM/YYYY') THEN 
                OPEN P_RESULT FOR SELECT 'Please enter a correct transaction date!' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_AMOUNT_INVESTED IS NULL OR TRIM(P_AMOUNT_INVESTED) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please enter amount.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        
            IF NOT REGEXP_LIKE(P_AMOUNT_INVESTED, '^[0-9]+(\.[0-9]+)?$') THEN
                OPEN P_RESULT FOR SELECT 'Please enter a correct amount.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_BUSINESS_RM IS NULL OR LENGTH(TRIM(P_BUSINESS_RM)) < 5 THEN
                OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            BEGIN
                SELECT RM_CODE INTO V_RM_CODE
                FROM EMPLOYEE_MASTER
                WHERE PAYROLL_ID = TRIM(P_BUSINESS_RM)
                AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' AS STATUS FROM DUAL; 
                    RETURN;
            END;


            -- Check if the transaction date is within the allowed range
            IF P_MARK = 0 THEN
                IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE(sysdate, 'DD/MM/YYYY') OR 
                TO_DATE(P_DATE, 'DD/MM/YYYY') > TO_DATE(v_Glbins_nextdate, 'DD/MM/YYYY') THEN
                    OPEN P_RESULT FOR SELECT 'Security restrictions for date range' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            IF P_MARK = 4 THEN
                IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE(v_Glbup_previousdate, 'DD/MM/YYYY') OR 
                TO_DATE(P_DATE, 'DD/MM/YYYY') > TO_DATE(v_Glbup_nextdate, 'DD/MM/YYYY') THEN
                    OPEN P_RESULT FOR SELECT 'Security restrictions for date range' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            -- Check if the transaction date is greater than the current date
            IF TO_DATE(P_DATE, 'DD/MM/YYYY') > SYSDATE THEN
                OPEN P_RESULT FOR SELECT 'Transaction Date Cannot Be Greater than Current Date' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            -- Payment Mode Codes: Cheque = C, Draft = D, Cash = H, ECS = E, Corporate = M, Others = R
            IF P_PAYMENT_MODE IS NULL OR TRIM(P_PAYMENT_MODE) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please Select a Payment Mode.' AS STATUS FROM DUAL; 
                RETURN;
            END IF;


            -- Validate Cheque Payment Mode
            IF P_PAYMENT_MODE = 'C' THEN
                -- Validate Bank Name
                IF P_BANK_NAME IS NULL OR TRIM(P_BANK_NAME) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Select a Bank Name' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;

                -- Validate Cheque Number
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Cheque Number' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;

                -- Validate Cheque Date
                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Cheque Date' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            -- Validate Payment Mode Conditions
            IF P_PAYMENT_MODE = 'D' THEN -- Draft Payment
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Draft Number' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;

                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Draft Date' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            -- Validate ECS Payment Mode
            IF P_PAYMENT_MODE = 'E' THEN -- ECS Payment
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a MCR Number' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;

                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Date' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            -- VALIDATE OTHER PAYMENT MODES
            IF P_PAYMENT_MODE = 'R' THEN -- Cash Payment
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a FDR Number' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;

                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert renewal Date' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
                
                IF P_CHEQUE_DATE IS NOT NULL THEN
                    dtf := '01/' || TO_CHAR(SYSDATE, 'MM/YYYY');
                    DtL := LAST_DAY(ADD_MONTHS(TO_DATE(dtf, 'DD/MM/YYYY'), 1)) - 1;
                    IF TO_DATE(P_CHEQUE_DATE, 'DD/MM/YYYY') < TO_DATE(dtf, 'DD/MM/YYYY') OR
                    TO_DATE(P_CHEQUE_DATE, 'DD/MM/YYYY') > DtL THEN
                        OPEN P_RESULT FOR SELECT 'Cheque/Draft Date Should be in Current Month' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;
                END IF;

            END IF;

            -- Validate Business Branch
            IF P_BUSINESS_BRANCH IS NULL OR TRIM(P_BUSINESS_BRANCH) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please Select Business Branch' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_MARK  = 0 THEN
                SELECT sch_code INTO CSV_MyRs_validate
                FROM transaction_st
                WHERE client_code= P_INVESTOR_CODE
                AND sch_code IN ('OP#09971') 
                AND tran_type IN ('PURCHASE','REINVESTMENT','SWITCH IN');
            END;

            select sch_code INTO CSV_MyRs_validate
            from transaction_st where 
            client_code=P_INVESTOR_CODE
            and sch_code in ('OP#09971','OP#09973') 
            and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN');

            IF CSV_MyRs_validate IS NOT NULL THEN
                -- Check if the scheme code is valid
                IF P_SCHEME_CODE = 'OP#09972' THEN
                    OPEN P_RESULT FOR SELECT 'Not Allowed in this Scheme Please Select Scheme Tier1 or Tier1+2' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            IF P_INVESTOR_TYPE = 1 AND P_APP_NO = '11' THEN

                IF P_TRAN_CODE IS NULL OR P_TRAN_CODE = '0' THEN
                    select sch_code, manual_arno INTO CSV_MyRs_validate, CSV_manual_arno
                    from transaction_st 
                    where client_code=P_INVESTOR_CODE 
                    and sch_code in ('OP#09971','OP#09972','OP#09973') 
                    and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN');
                ELSE 
                    select sch_code, manual_arno INTO CSV_MyRs_validate , CSV_manual_arno
                    from transaction_st 
                    where tran_code<>P_TRAN_CODE 
                    and client_code=P_INVESTOR_CODE 
                    and sch_code in ('OP#09971','OP#09972','OP#09973') 
                    and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN');
                END IF;


                IF CSV_MyRs_validate IS NOT NULL THEN
                    IF CSV_MyRs_validate = 'OP#09971' AND (P_SCHEME_CODE = "OP#09971" Or P_SCHEME_CODE = "OP#09972") THEN
                        IF TRIM(CSV_manual_arno) <> P_MANUAL_AR_NO THEN
                            OPEN P_RESULT FOR SELECT 'Please Enter Same PRAN No' AS STATUS FROM DUAL; 
                            RETURN; 
                        END IF;
                    ELSIF CSV_MyRs_validate = 'OP#09973' AND P_SCHEME_CODE = "OP#09973" THEN
                        IF TRIM(CSV_manual_arno) <> P_MANUAL_AR_NO THEN
                            OPEN P_RESULT FOR SELECT 'Please Enter Same PRAN No' AS STATUS FROM DUAL; 
                            RETURN; 
                        END IF;
                    END IF;
                ELSE 
                    IF P_TRAN_CODE IS NULL OR P_TRAN_CODE = '0' THEN
                        select sch_code, manual_arno INTO CSV_MyRs_validate, CSV_manual_arno
                        from transaction_st 
                        where manual_arno=P_MANUAL_AR_NO 
                        and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN');
                    ELSE 
                        select sch_code, manual_arno INTO CSV_MyRs_validate , CSV_manual_arno
                        from transaction_st 
                        where tran_code<>P_TRAN_CODE 
                        and manual_arno=P_MANUAL_AR_NO 
                        and tran_type in ('PURCHASE','REINVESTMENT','SWITCH IN');
                    END IF;

                    IF CSV_MyRs_validate IS NOT NULL THEN
                        OPEN P_RESULT FOR SELECT 'Please Enter Different PRAN No.' AS STATUS FROM DUAL; 
                    END IF;
                END IF;
                OPEN P_RESULT FOR SELECT 'Please select a transaction code to Modify.' AS STATUS AS STATUS FROM DUAL; 
                RETURN; 
            END IF;

            IF P_SCHEME_CODE = 'OP#09972' AND P_APP_NO = '11' THEN
                IF P_AMOUNT_T1 = 0 OR P_AMOUNT_T1 IS NULL OR P_AMOUNT_T2 =0 OR P_AMOUNT_T2 IS NULL THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Tier1 and Tier2 Amount in this Scheme' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            IF P_APP_NO = '11' AND P_SCHEME_CODE = 'OP#09973' THEN
                IF P_AMOUNT_T1 < 500 OR P_AMOUNT_T2 < 1000 THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Correct Amount in this Scheme' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;

            IF P_APP_NO = '11' AND P_SCHEME_CODE = 'OP#09973' THEN
                IF P_AMOUNT_T1 = 0 OR P_AMOUNT_T1 IS NULL OR P_AMOUNT_T2 =0 OR P_AMOUNT_T2 IS NULL THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Tier1 and Tier2 Amount in this Scheme' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;
        END; -- chkSaveValidation

        BEGIN -- CHECK DUP CHEQUE ON V_CLIENT_CAT_DUP_CHEQUE
            IF V_CLIENT_CAT_DUP_CHEQUE <> '4004' THEN
                -- Count matching transactions
                SELECT COUNT(*) 
                INTO V_COUNT_TRAN_DUP_CHEQUE
                FROM TRANSACTION_ST
                WHERE CHEQUE_NO = TRIM(P_CHEQUE_NO)
                AND TRIM(BANK_NAME) = TRIM(P_BANK_NAME)
                AND TRAN_TYPE IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN');

                -- If duplicates exist, return error message
                IF V_COUNT_TRAN_DUP_CHEQUE > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;
        END; -- CHECK DUP CHEQUE ON V_CLIENT_CAT_DUP_CHEQUE

        BEGIN -- Check for duplicate transaction in 'PURCHASE', 'REINVESTMENT', 'SWITCH IN' transactions
            SELECT COUNT(*) INTO V_COUNT_DUP_TRAN
            FROM TRANSACTION_ST
            WHERE CLIENT_CODE = P_INVESTOR_CODE
            AND SCH_CODE = P_SCHEME_CODE
            AND APP_NO = P_APP_NO
            AND AMOUNT = P_AMOUNT_INVESTED
            AND CHEQUE_NO = P_CHEQUE_NO
            AND TRIM(BANK_NAME) = P_BANK_NAME
            AND TRAN_TYPE IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN');

            IF V_COUNT_DUP_TRAN > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Transaction!' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        END; -- Check for duplicate transaction in 'PURCHASE', 'REINVESTMENT', 'SWITCH IN' transactions

        -- P_PAYMENT_MODE HAVE C, D, H, E, R, M THAT IS PAYMODE

        -- Insert into transaction_sttemp
        INSERT INTO TRANSACTION_STTEMP (
            CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, 
            TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, 
            RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, REMARK, DOC_ID, 
            MATURITY_PERIOD, FAMILY_hEAD
        ) VALUES (
            P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, P_PAYMENT_MODE, P_INVESTOR_TYPE, 
            P_DATE, P_INVESTOR_CODE, MYMUTCODE, P_SCHEME_CODE, 'PURCHASE', P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, P_CRA_BRANCH, 
            P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, P_CHEQUE_NO, P_CHEQUE_DATE, P_REMARK, P_DT_NUMBER,
            '58', v_found_fam_h
        );

        BEGIN -- RETRIVE RECENT TRAN_CODE AND GST_NO
            SELECT MAX(TRAN_CODE) 
            INTO V_MyTranCode
            FROM TEMP_TRAN
            WHERE BRANCH_CODE = P_BUSINESS_BRANCH
            AND SUBSTR(TRAN_CODE, 1, 2) = '07';

            -- Retrieve the invoice_no from transaction_sttemp based on the tran_code
            SELECT INVOICE_NO
            INTO V_MyGSTNO
            FROM TRANSACTION_STTEMP
            WHERE TRAN_CODE = V_MyTranCode;        
        END;

        -- Insert into transaction_st
        INSERT INTO TRANSACTION_ST (
            DOC_ID, INVOICE_NO, CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, 
            PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, 
            AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, 
            CHEQUE_NO, CHEQUE_DATE, REMARK, LOGGEDUSERID, INV_NAME,
            MATURITY_PERIOD, FAMILY_hEAD
        ) VALUES ( 
            P_DT_NUMBER, V_MyGSTNO , P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, 
            P_PAYMENT_MODE, V_MyTranCode, P_INVESTOR_TYPE, SYSDATE,P_INVESTOR_CODE /*NewClientCode*/, MYMUTCODE, P_SCHEME_CODE, 'PURCHASE', 
            P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, SUBSTR(P_INVESTOR_CODE, 1, 8), P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, 
            P_CHEQUE_NO, P_CHEQUE_DATE, 'NPS', P_LOGGEDIN_USER, P_SUBSCRIBER_NAME,
            '58', v_found_fam_h
        ); 

        -- Insert into nps_transaction
        INSERT INTO NPS_TRANSACTION (TRAN_CODE, AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK) 
        VALUES (V_MyTranCode, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK);
        COMMIT;

        v_return_message := 'Your Transaction No Is ' || V_MyTranCode || ' and Your Recpt No Is ' || (select unique_id from transaction_st where tran_code=V_MyTranCode);

        --DOUBLE TRANSACTION OF CONTRIBUTION WHEN REGISTRATION AND INV_TYPE IS OptIndividual
        IF P_REQUEST_ID = '11' AND P_INVESTOR_TYPE = '0' THEN
            insert into transaction_sttemp (CORPORATE_NAME,ref_tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,doc_id) 
            select CORPORATE_NAME,tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,0, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,'' from transaction_sttemp where tran_code= V_MyTranCode;
            COMMIT;

            BEGIN -- Retrieve the maximum tran_code from temp_tran where branch_code matches the input
                select max(tran_code) into V_MyTranCode1
                from temp_tran where branch_code=P_BUSINESS_BRANCH and substr(tran_code,1,2)='07'
            END;

            UPDATE TB_DOC_UPLOAD SET AR_CODE = V_MyTranCode1 WHERE COMMON_ID = P_DT_NUMBER;
            COMMIT;

            insert into transaction_st (ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id) 
            select ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,12,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id from transaction_sttemp where tran_code=V_MyTranCode1;
            COMMIT;

            insert into nps_transaction (TRAN_CODE,AMOUNT1,AMOUNT2,REG_CHARGE,TRAN_CHARGE,SERVICETAX,REMARK) values (V_MyTranCode1,0,0,0,0,0,P_REMARK);
            COMMIT;

            DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE=V_MyTranCode1;

            select unique_id into V_MySecReq
            from transaction_st where tran_code=V_MyTranCode1;

            v_return_message := "Your Duplicate Transaction No Is " || V_MyTranCode1 || " and Your Recpt No IS " ||  MySecReq;
        END IF;
        DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE = V_MyTranCode;
        COMMIT;

        -- CHEK v_return_message IS NOT NULL MEAN HASING ANY SUCCESS MESSAGE THEN OPEN P_RESULT
        IF v_return_message IS NOT NULL THEN
            OPEN P_RESULT FOR
            --SELECT 'Your Duplicate Transaction No Is ' ||(V_TRAN_CODE_INSERTION) || ' and Your Recpt No Is ' ||  (SELECT UNIQUE_ID FROM TRANSACTION_ST WHERE TRAN_CODE = V_TRAN_CODE_INSERTION) AS STATUS
            SELECT 'TRANSACTION PUNCHED SUCCESSFULLY: ' || v_return_message AS STATUS FROM DUAL;
            RETURN;
        END IF;
    END IF;
END;









IF P_MARK = '0' THEN

    --IF UPPER(TRIM(P_PAYMENT_MODE)) IN ('C', 'D') THEN

    -- Insert into transaction_sttemp
    INSERT INTO TRANSACTION_STTEMP (
        CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, 
        TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, 
        RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, REMARK, DOC_ID, 
        MATURITY_PERIOD, FAMILY_hEAD
    ) VALUES (
        P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, P_PAYMENT_MODE, P_INVESTOR_TYPE, 
        P_DATE, P_INVESTOR_CODE, MYMUTCODE, P_SCHEME_CODE, 'PURCHASE', P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, P_CRA_BRANCH, 
        P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, P_CHEQUE_NO, P_CHEQUE_DATE, P_REMARK, P_DT_NUMBER,
        '58', v_found_fam_h
    );

    BEGIN -- Retrieve the maximum tran_code from temp_tran where branch_code matches the input
        SELECT MAX(TRAN_CODE) 
        INTO V_TRAN_CODE_INSERTION
        FROM TEMP_TRAN
        WHERE BRANCH_CODE = P_BUSINESS_BRANCH
        AND SUBSTR(TRAN_CODE, 1, 2) = '07';

        -- Retrieve the invoice_no from transaction_sttemp based on the tran_code
        SELECT INVOICE_NO
        INTO V_GST_NO_INSERTION
        FROM TRANSACTION_STTEMP
        WHERE TRAN_CODE = V_TRAN_CODE_INSERTION; 

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            -- If no data is found for either query, set the output parameters to NULL
            V_TRAN_CODE_INSERTION := NULL;
            V_GST_NO_INSERTION := NULL;
        WHEN OTHERS THEN
            -- Handle any other errors
            V_TRAN_CODE_INSERTION := NULL;
            V_GST_NO_INSERTION := NULL;
            RAISE;
    END ;



    -- Insert into transaction_st
    INSERT INTO TRANSACTION_ST (
        DOC_ID, INVOICE_NO, CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, 
        PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, 
        AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, 
        CHEQUE_NO, CHEQUE_DATE, REMARK, LOGGEDUSERID, INV_NAME,
        MATURITY_PERIOD, FAMILY_hEAD
    ) VALUES ( 
        P_DT_NUMBER, V_GST_NO_INSERTION /*P_GST_TAX*/ , P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, 
        P_PAYMENT_MODE, V_TRAN_CODE_INSERTION, P_INVESTOR_TYPE, SYSDATE,P_INVESTOR_CODE /*NewClientCode*/, MYMUTCODE, P_SCHEME_CODE, 'PURCHASE', 
        P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, SUBSTR(P_INVESTOR_CODE, 1, 8), P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, 
        P_CHEQUE_NO, P_CHEQUE_DATE, 'NPS', P_LOGGEDIN_USER, P_SUBSCRIBER_NAME,
        '58', v_found_fam_h
    );

     BEGIN
        SELECT TS.TRAN_CODE 
        INTO V_GENERATED_TR
        FROM TRANSACTION_ST TS
        WHERE TS.DOC_ID = P_DT_NUMBER; 
        EXCEPTION WHEN NO_DATA_FOUND 
        THEN V_GENERATED_TR :=0; 
    END;

    -- Insert into nps_transaction
    INSERT INTO NPS_TRANSACTION (
        TRAN_CODE, AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK
    ) VALUES (
        V_TRAN_CODE_INSERTION, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK
    );

    v_return_message := 'Your Transaction No Is ' || V_TRAN_CODE_INSERTION || ' and Your Recpt No Is ' || P_RECEIPT_NO;


    IF P_REQUEST_ID = '11' AND P_INVESTOR_TYPE = '0' THEN
        insert into transaction_sttemp (CORPORATE_NAME,ref_tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,doc_id) 
        select CORPORATE_NAME,tran_code,manual_arno,BANK_NAME,folio_no,APP_NO,PAYMENT_MODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,0, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,V_null_value from transaction_sttemp where tran_code= V_TRAN_CODE_INSERTION;



         select max(tran_code) into MyTranCode1
         from temp_tran where branch_code=P_BUSINESS_BRANCH and substr(tran_code,1,2)='07'


        UPDATE TB_DOC_UPLOAD SET AR_CODE = MyTranCode1 WHERE COMMON_ID= P_DT_NUMBER;

        insert into transaction_st (ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,APP_NO,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id) select ref_tran_code,manual_arno,BANK_NAME,FOLIO_NO,12,PAYMENT_MODE,TRAN_CODE,INVESTOR_TYPE,TR_DATE,CLIENT_CODE,MUT_CODE,SCH_CODE,TRAN_TYPE,AMOUNT, BRANCH_CODE,SOURCE_CODE,RMCODE,BUSINESS_RMCODE,BUSI_BRANCH_CODE,cheque_no,CHEQUE_DATE,remark,LOGGEDUSERID,doc_id from transaction_sttemp where tran_code=MyTranCode1;


        insert into nps_transaction (TRAN_CODE,AMOUNT1,AMOUNT2,REG_CHARGE,TRAN_CHARGE,SERVICETAX,REMARK) values (MyTranCode1,0,0,0,0,0,P_REMARK);

        DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE=MyTranCode1;

        select unique_id into MySecReq
        from transaction_st where tran_code=MyTranCode1;

        v_return_message := "Your Duplicate Transaction No Is " || MyTranCode1 || " and Your Recpt No IS " ||  MySecReq;


    END IF;
    


    DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE = V_TRAN_CODE_INSERTION;

    COMMIT;
    -- Return confirmation
    OPEN P_RESULT FOR
        --SELECT 'Your Duplicate Transaction No Is ' ||(V_TRAN_CODE_INSERTION) || ' and Your Recpt No Is ' ||  (SELECT UNIQUE_ID FROM TRANSACTION_ST WHERE TRAN_CODE = V_TRAN_CODE_INSERTION) AS STATUS
        select v_return_message from dual;
    RETURN;



ELSIF P_MARK = '4' THEN
    BEGIN -- Fetch CLIENT_CODE and TRAN_CODE
        BEGIN
            SELECT CLIENT_CODE, TRAN_CODE
            INTO NEWCLIENTCODE, MYTRANCODE
            FROM TRANSACTION_ST
            WHERE DOC_ID = P_DT_NUMBER;
        EXCEPTION 
            WHEN NO_DATA_FOUND THEN 
                NEWCLIENTCODE := NULL; 
                MYTRANCODE := NULL;
        END;
    END;

    -- Update transaction_st
    UPDATE TRANSACTION_ST
    SET TR_DATE = P_DATE, CLIENT_CODE = P_INVESTOR_CODE, SOURCE_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8),
        BUSI_BRANCH_CODE = P_BUSINESS_BRANCH, BUSINESS_RMCODE = P_BUSINESS_RM, MUT_CODE = MYMUTCODE, 
        SCH_CODE = P_SCHEME_CODE, AMOUNT = P_AMOUNT_INVESTED, FOLIO_NO = P_FOLIO_NUMBER, APP_NO = P_APP_NO, 
        PAYMENT_MODE = P_PAYMENT_MODE, CHEQUE_DATE = P_CHEQUE_DATE, CHEQUE_NO = P_CHEQUE_NO, 
        BANK_NAME = P_BANK_NAME, MANUAL_ARNO = P_MANUAL_AR_NO, CORPORATE_NAME = P_CORPORATE_NAME, 
        UNIQUE_ID = P_RECEIPT_NO, MODIFY_USER = P_LOGGEDIN_USER, MODIFY_DATE = P_DATE, REMARK = P_REMARK,
        MATURITY_PERIOD = '58', FAMILY_hEAD = v_found_fam_h

    WHERE TRAN_CODE = P_TRAN_CODE;


    -- INSERT/UPDATE IN nps_transaction
    BEGIN -- FLAG FOR CHECKING DATA EXIST IN nps_transaction OR NOT
        BEGIN 
        SELECT TRAN_CODE 
        INTO V_FLAG_NPS_TRAN 
        FROM NPS_TRANSACTION WHERE TRAN_CODE = P_TRAN_CODE; 
        EXCEPTION WHEN NO_DATA_FOUND THEN V_FLAG_NPS_TRAN :=0; END;
    END;

    IF V_FLAG_NPS_TRAN = 0 THEN 
        INSERT INTO NPS_TRANSACTION ( TRAN_CODE, AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK ) VALUES (V_GENERATED_TR, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK);
    ELSE
        UPDATE NPS_TRANSACTION
        SET AMOUNT1 = P_AMOUNT_T1, AMOUNT2 = P_AMOUNT_T2, REG_CHARGE = P_RECHARGE1, TRAN_CHARGE = P_RECHARGE2, 
            SERVICETAX = P_GST_TAX, REMARK = P_REMARK
        WHERE TRAN_CODE = P_TRAN_CODE;
    END IF;

    -- END INSERT/UPDATE IN nps_transaction

    -- FINAL RESULT Return confirmation
    OPEN P_RESULT FOR
        SELECT 'Updation successful' AS STATUS
        FROM DUAL;
END IF;



END PSM_NPS_INSERT_UPDATE_PRA;
/
