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
    -- Consolidated variable declarations
    v_myTranCode                VARCHAR2(100);
    v_myGstNo                   VARCHAR2(100);
    v_newClientCode             VARCHAR2(100);
    v_myMutCode                 VARCHAR2(100) := NULL;
    v_currentRole               NUMBER;
    v_tempRole                  NUMBER;
    v_flag_nps_tran             NUMBER;
    v_generated_tr              NUMBER;
    v_myTranCode1               VARCHAR2(100);
    v_mySecReq                  VARCHAR2(100);
    v_glbRoleId                 NUMBER := 0;
    v_glbRoleId2                NUMBER := 0;
    v_count_fatca               NUMBER;     
    v_client_cat_dup_cheque     VARCHAR2(10);
    v_count_tran_dup_cheque     NUMBER;
    v_busi_rm_cd                VARCHAR2(100) := NULL;
    v_busi_branch_cd            VARCHAR2(100) := NULL;
    v_clientBranchCode          VARCHAR2(100) := NULL;
    v_clientRmCode              VARCHAR2(100) := NULL;
    v_4_tr_date                 VARCHAR2(100) := NULL; 
    v_4_client_code             VARCHAR2(100) := NULL; 
    v_4_source_code             VARCHAR2(100) := NULL; 
    v_4_busi_branch_code        VARCHAR2(100) := NULL; 
    v_4_business_rmcode         VARCHAR2(100) := NULL; 
    v_4_mut_code                VARCHAR2(100) := NULL; 
    v_4_sch_code                VARCHAR2(100) := NULL; 
    v_4_amount                  VARCHAR2(100) := NULL; 
    v_4_folio_no                VARCHAR2(100) := NULL; 
    v_4_app_no                  VARCHAR2(100) := NULL; 
    v_4_payment_mode            VARCHAR2(100) := NULL; 
    v_4_cheque_date             VARCHAR2(100) := NULL; 
    v_4_cheque_no               VARCHAR2(100) := NULL; 
    v_4_bank_name               VARCHAR2(100) := NULL; 
    v_4_manual_arno             VARCHAR2(100) := NULL; 
    v_4_corporate_name          VARCHAR2(100) := NULL; 
    v_4_unique_id               VARCHAR2(100) := NULL; 
    v_4_modify_user             VARCHAR2(100) := NULL; 
    v_4_modify_date             VARCHAR2(100) := NULL; 
    v_count_dup_tran            NUMBER := 0;
    v_count_fam_h               NUMBER := 0;
    v_found_fam_h               VARCHAR2(20) := NULL;
    v_null_value                VARCHAR2(100) := NULL;
    v_return_message            VARCHAR2(200) := NULL;
    v_err_ret_msg               varchar2(500) := null;

    -- Check save validation variables
    v_csv_get_busiRmCode        VARCHAR2(100) := NULL;
    v_csv_myrs_validate         VARCHAR2(100) := NULL;
    v_csv_manual_arno           VARCHAR2(100) := NULL;
    v_csv_myrs_validate1        VARCHAR2(100) := NULL;

    v_serverDateTime            DATE := SYSDATE; 
    v_ins_next_day              NUMBER;
    v_glbIns_nextDate           DATE;

    -- Variables for role_master columns
    v_up_pre_dur                NUMBER;
    v_up_next_dur               NUMBER;
    v_in_pre_dur                NUMBER;
    v_in_next_dur               NUMBER;
    v_up_pre_dur_type           NUMBER;
    v_up_next_dur_type          NUMBER;
    v_in_pre_dur_type           NUMBER;
    v_in_next_dur_type          NUMBER;

    -- Date calculation variables
    v_glbIns_previousDate       DATE;
    v_glbUp_previousDate        DATE;
    v_glbUp_nextDate            DATE;

    v_fy_start                  DATE;
    v_fy_end                    DATE;
    v_actual_date               DATE;
    v_chkSaveValidation         BOOLEAN := TRUE;
    v_rm_code                   VARCHAR2(20);

    V_UNIQUE_ID                 VARCHAR2(100) :=NULL;

BEGIN
    -- Validate punching and modification team
    IF P_MARK = '0' THEN 

        -- Punching demo: (121397/212)            
        SELECT COUNT(ROLE_ID) INTO v_glbRoleId2 
        FROM USERDETAILS_JI 
        WHERE LOGIN_ID = TRIM(P_LOGGEDIN_USER)  
        AND ROLE_ID IN (212, 1) 
        AND ROWNUM = 1; 

        IF v_glbRoleId2 = 0 THEN
            OPEN P_RESULT FOR SELECT 'Only Punching Team can punch the transaction...' AS STATUS FROM DUAL; 
            --RETURN;  
        END IF;
    ELSIF P_MARK = '4' THEN
        -- Modifying demo: (39006/146)
        SELECT COUNT(ROLE_ID) INTO v_glbRoleId2 
        FROM USERDETAILS_JI 
        WHERE LOGIN_ID = TRIM(P_LOGGEDIN_USER) 
        AND ROLE_ID IN (146, 1) 
        AND ROWNUM = 1;

        IF v_glbRoleId2 = 0 THEN
            OPEN P_RESULT FOR 
            SELECT 'Only NPS Team can modify the transaction....' AS STATUS FROM DUAL;
            --RETURN; 
        END IF;
    END IF;

    -- Fetch the fiscal year start and end dates
    BEGIN
        SELECT UP_PRE_DUR, UP_NEXT_DUR, IN_PRE_DUR, IN_NEXT_DUR, UP_PRE_DUR_TYPE, UP_NEXT_DUR_TYPE, IN_PRE_DUR_TYPE, IN_NEXT_DUR_TYPE
        INTO   v_up_pre_dur, v_up_next_dur, v_in_pre_dur, v_in_next_dur, v_up_pre_dur_type, v_up_next_dur_type, v_in_pre_dur_type, v_in_next_dur_type
        FROM   ROLE_MASTER WHERE  ROLE_ID = P_ROLE_ID;

        IF v_in_pre_dur_type = 1 AND v_in_next_dur_type = 1 THEN
            v_glbIns_previousDate := SYSDATE - v_in_pre_dur;
            v_glbIns_nextDate := SYSDATE + (v_in_next_dur - 1);
        ELSIF v_in_pre_dur_type = 1 AND v_in_next_dur_type = 2 THEN
            v_glbIns_previousDate := SYSDATE - v_in_pre_dur;
            v_glbIns_nextDate := ADD_MONTHS(SYSDATE, v_in_next_dur - 1);
        ELSIF v_in_pre_dur_type = 1 AND v_in_next_dur_type = 3 THEN
            v_actual_date := SYSDATE - v_in_pre_dur;
            IF v_fy_start >= v_actual_date THEN
                v_glbIns_previousDate := v_fy_start;
            ELSE
                v_glbIns_previousDate := v_actual_date;
            END IF;
            v_glbIns_nextDate := ADD_MONTHS(v_fy_end, 12 * (v_in_next_dur - 1)); -- year add
        END IF;

        IF v_up_pre_dur_type = 1 AND v_up_next_dur_type = 1 THEN
            v_glbUp_previousDate := SYSDATE - v_up_pre_dur;
            v_glbUp_nextDate := SYSDATE + (v_up_next_dur - 1);
        ELSIF v_up_pre_dur_type = 2 AND v_up_next_dur_type = 2 THEN
            v_glbUp_previousDate := ADD_MONTHS(SYSDATE, -v_up_pre_dur);
            v_glbUp_nextDate := ADD_MONTHS(SYSDATE, v_up_next_dur - 1);
        END IF;

    END;

    -- Fetch ISS_CODE for the provided P_SCHEME_CODE
    BEGIN
        SELECT ISS_CODE INTO v_myMutCode
        FROM OTHER_PRODUCT 
        WHERE OSCH_CODE = P_SCHEME_CODE 
        AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            OPEN P_RESULT FOR SELECT 'Scheme code not found' AS STATUS FROM DUAL;
            RETURN;
    END;

    -- Find family head code
    BEGIN
        SELECT COUNT(*) INTO v_count_fam_h
        FROM CLIENT_TEST 
        WHERE SOURCE_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8) 
        AND CLIENT_CODE = MAIN_CODE;  

        IF v_count_fam_h > 0 THEN
            SELECT NVL(CLIENT_CODEKYC, NULL) INTO v_found_fam_h 
            FROM CLIENT_TEST 
            WHERE SOURCE_CODE = SUBSTR(P_INVESTOR_CODE,1,8) 
            AND CLIENT_CODE = MAIN_CODE;
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            v_found_fam_h := NULL;
    END;

    -- DT NUMBER is required and return    
    IF TRIM(P_MARK) = '0' AND TRIM(P_DT_NUMBER) IS NULL THEN 
        OPEN P_RESULT FOR SELECT 'DT No cannot be left blank.' AS STATUS FROM DUAL;
        RETURN; 
    END IF;

    -- FATCA_VALIDATION : P_CORPORATE_NAME, P_UNFREEZE, DELETE FROM NPS_FATCA_NON_COMPLIANT
    IF P_MARK = '0' THEN
        IF P_INVESTOR_TYPE = 1 THEN -- Check if corporate name is required
            IF P_CORPORATE_NAME IS NULL OR TRIM(P_CORPORATE_NAME) = '' THEN
                OPEN P_RESULT FOR SELECT 'Corporate name cannot be left blank.' AS STATUS FROM DUAL;
                RETURN; 
            END IF;
        END IF;

        IF P_UNFREEZE = '0' THEN -- FATCA Non-Compliance Check
            IF P_MANUAL_AR_NO IS NOT NULL AND TRIM(P_MANUAL_AR_NO) <> '' THEN
                SELECT COUNT(*)
                INTO v_count_fatca
                FROM NPS_FATCA_NON_COMPLIANT
                WHERE PRAN_NO = P_MANUAL_AR_NO;

                IF v_count_fatca >= 1 THEN
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

    -- CHECK_DUPLICATE_CHEQUE
    IF P_MARK = '0' OR P_MARK = '4' THEN
        -- Fetch Client Category
        BEGIN
            SELECT CATEGORY_ID INTO v_client_cat_dup_cheque
            FROM CLIENT_MASTER 
            WHERE CLIENT_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8);
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                v_client_cat_dup_cheque := '4004'; -- Default to non-corporate
        END;

        IF v_client_cat_dup_cheque <> '4004' THEN
            -- Check if transaction code is "0"
            IF P_TRAN_CODE IS NULL THEN
                -- Check for duplicate cheque number in the last 6 months
                SELECT COUNT(*) INTO v_count_tran_dup_cheque 
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

                IF v_count_tran_dup_cheque > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            ELSE
                -- If ReqCode = "11"
                IF P_APP_NO = '11' THEN
                    -- Check for duplicate cheque number in the last 6 months, excluding current TRAN_CODE
                    SELECT COUNT(*) INTO v_count_tran_dup_cheque 
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

                    IF v_count_tran_dup_cheque > 0 THEN
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;
                ELSE
                    -- Check for duplicate cheque number in the last 6 months, excluding current TRAN_CODE
                    SELECT COUNT(*) INTO v_count_tran_dup_cheque 
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

                    IF v_count_tran_dup_cheque > 0 THEN
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;
                END IF;
            END IF;
        END IF;
    END IF;

    -- NSDL Branch on P_RECEIPT_NO
    IF P_MARK NOT IN ('3', '4') THEN
        IF P_FOLIO_NUMBER IS NULL OR TRIM(P_FOLIO_NUMBER) = '' THEN
            OPEN P_RESULT FOR SELECT 'Please Select NSDL Branch First!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;
    END IF;

    -- Get payroll_id BY BUSINESS_RM
    /*
    BEGIN
        SELECT PAYROLL_ID 
        INTO v_busi_rm_cd    
        FROM EMPLOYEE_MASTER 
        WHERE PAYROLL_ID = P_BUSINESS_RM;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            OPEN P_RESULT FOR SELECT 'Invalid Business RM Code' AS STATUS FROM DUAL;
            RETURN;
    END;*/

    -- Get RM Code and Branch Code BY P_INVESTOR_CODE
    IF P_INVESTOR_CODE IS NOT NULL THEN
        BEGIN
            SELECT RM_CODE, BRANCH_CODE 
            INTO v_clientRmCode, v_clientBranchCode
            FROM INVESTOR_MASTER 
            WHERE INV_CODE = P_INVESTOR_CODE;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                v_clientRmCode := NULL;
                v_clientBranchCode := NULL;
        END;
    END IF;

    -- Modify(4): corp, ar on update, checkSaveValidation, update nps_transaction
    IF P_MARK = '4' THEN -- 4 FOR MODIFYING
        IF P_INVESTOR_TYPE = '1' THEN -- IF CORPORATE NAME IS REQUIRED if P_INVESTOR_TYPE = 1
            IF P_CORPORATE_NAME IS NULL OR TRIM(P_CORPORATE_NAME) = '' THEN
                OPEN P_RESULT FOR SELECT 'Corporate name cannot be left blank.' AS STATUS FROM DUAL;
                RETURN; 
            END IF;
        END IF;

        IF P_TRAN_CODE IS NULL OR P_TRAN_CODE = '0' THEN -- TRAN_CODE IS NULL
            OPEN P_RESULT FOR SELECT 'Please select a transaction code to Modify.' AS STATUS FROM DUAL;
            RETURN; 
        END IF;

        -- Update NPS_TRANSACTION
        UPDATE NPS_TRANSACTION SET 
            AMOUNT1 = P_AMOUNT_T1,
            AMOUNT2 = P_AMOUNT_T2,
            REG_CHARGE = P_RECHARGE1,
            TRAN_CHARGE = P_RECHARGE2,
            SERVICETAX = P_GST_TAX,
            remark = P_REMARK
        WHERE TRAN_CODE = P_TRAN_CODE;
        COMMIT;

        v_return_message := 'Transaction Updated Successfully';
    END IF;

    IF P_MARK = '0' THEN -- SAVE(0) : CHECK_DUPLICATE_CHEQUE, Vclientcategory
        -- chkSaveValidation

        IF P_PRODUCT_CLASS IS NOT NULL THEN
            DECLARE
                V_PRD_NAME   VARCHAR2(100);
                V_PRD_CODE   VARCHAR2(100);
                V_PRD_MUT_CD VARCHAR2(100);   
                V_PRD_NAT_CD VARCHAR2(100);  
                V_PRD_PRD_CD VARCHAR2(100);
            BEGIN
                SELECT mut_name INTO V_PRD_NAME 
                FROM (
                    SELECT mut_name FROM mut_fund WHERE mut_code = P_PRODUCT_CLASS
                    UNION ALL
                    SELECT iss_name mut_name 
                    FROM iss_master a, product_class_issuer_mf b 
                    WHERE TRIM(iss_code) = P_PRODUCT_CLASS
                    AND a.iss_code = b.issuermf_code
                ) WHERE ROWNUM = 1;

                IF V_PRD_NAME IS NOT NULL THEN
                    SELECT nature_code, prod_code 
                    INTO V_PRD_NAT_CD, V_PRD_PRD_CD  
                    FROM product_master 
                    WHERE name LIKE '%' || V_PRD_NAME || '%';

                    IF V_PRD_NAT_CD = 'NT001' THEN
                        SELECT mut_code INTO V_PRD_MUT_CD 
                        FROM mut_fund 
                        WHERE mut_name = V_PRD_NAME;
                    ELSE
                        SELECT iss_code INTO V_PRD_MUT_CD 
                        FROM iss_master a, product_class_issuer_mf b 
                        WHERE TRIM(iss_name) = V_PRD_NAME;
                    END IF;

                    IF V_PRD_MUT_CD IS NULL THEN
                        OPEN P_RESULT FOR SELECT 'Please select a Product.' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;
                END IF;
            EXCEPTION  
                WHEN OTHERS THEN
                    NULL;
            END;
        ELSE 
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

        IF P_DATE < TO_DATE('01/01/1980', 'DD/MM/YYYY') THEN 
            OPEN P_RESULT FOR SELECT 'Please enter a correct transaction date!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        IF P_AMOUNT_INVESTED IS NULL THEN
            OPEN P_RESULT FOR SELECT 'Please enter amount.' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        IF NOT REGEXP_LIKE(TO_CHAR(P_AMOUNT_INVESTED), '^[0-9]+(\.[0-9]+)?$') THEN
            OPEN P_RESULT FOR SELECT 'Please enter a correct amount.' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        IF P_BUSINESS_RM IS NULL OR LENGTH(TRIM(P_BUSINESS_RM)) < 5 THEN
            OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        BEGIN
            SELECT RM_CODE INTO v_rm_code
            FROM EMPLOYEE_MASTER
            WHERE PAYROLL_ID = TRIM(P_BUSINESS_RM)
            AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' AS STATUS FROM DUAL; 
                RETURN;
        END;

        -- Check if the transaction date is within the allowed range
        IF P_MARK = '0' THEN
            IF P_DATE < TRUNC(SYSDATE) OR P_DATE > v_glbIns_nextDate THEN
                OPEN P_RESULT FOR SELECT 'Security restrictions for date range' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        END IF;

        IF P_MARK = '4' THEN
            IF P_DATE < v_glbUp_previousDate OR P_DATE > v_glbUp_nextDate THEN
                OPEN P_RESULT FOR SELECT 'Security restrictions for date range' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        END IF;

        -- Check if the transaction date is greater than the current date
        IF P_DATE > SYSDATE THEN
            OPEN P_RESULT FOR SELECT 'Transaction Date Cannot Be Greater than Current Date' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        -- Payment Mode Codes: Cheque = C, Draft = D, Cash = H, ECS = E, Corporate = M, Others = R
        IF P_PAYMENT_MODE IS NULL THEN
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
            IF P_CHEQUE_DATE IS NULL THEN
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

            IF P_CHEQUE_DATE IS NULL THEN
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

            IF P_CHEQUE_DATE IS NULL THEN
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

            IF P_CHEQUE_DATE IS NULL THEN
                OPEN P_RESULT FOR SELECT 'Please Insert renewal Date' AS STATUS FROM DUAL; 
                RETURN;
            END IF;

            IF P_CHEQUE_DATE IS NOT NULL THEN
                DECLARE
                    v_dtf DATE := TO_DATE('01/' || TO_CHAR(SYSDATE, 'MM/YYYY'), 'DD/MM/YYYY');
                    v_dtl DATE := LAST_DAY(ADD_MONTHS(v_dtf, 1)) - 1;
                BEGIN
                    IF P_CHEQUE_DATE < v_dtf OR P_CHEQUE_DATE > v_dtl THEN
                        OPEN P_RESULT FOR SELECT 'Cheque/Draft Date Should be in Current Month' AS STATUS FROM DUAL; 
                        RETURN;
                    END IF;
                END;
            END IF;
        END IF;

        -- Validate Business Branch
        IF P_BUSINESS_BRANCH IS NULL THEN
            OPEN P_RESULT FOR SELECT 'Please Select Business Branch' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        IF P_MARK = '0' THEN
            BEGIN
                SELECT SCH_CODE INTO v_csv_myrs_validate
                FROM TRANSACTION_ST
                WHERE CLIENT_CODE = P_INVESTOR_CODE
                AND SCH_CODE IN ('OP#09971') 
                AND TRAN_TYPE IN ('PURCHASE','REINVESTMENT','SWITCH IN')
                AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    v_csv_myrs_validate := NULL;
            END;
        END IF;

        BEGIN
            SELECT SCH_CODE INTO v_csv_myrs_validate
            FROM TRANSACTION_ST 
            WHERE CLIENT_CODE = P_INVESTOR_CODE
            AND SCH_CODE IN ('OP#09971','OP#09973') 
            AND TRAN_TYPE IN ('PURCHASE','REINVESTMENT','SWITCH IN')
            AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                v_csv_myrs_validate := NULL;
        END;

        IF v_csv_myrs_validate IS NOT NULL THEN
            -- Check if the scheme code is valid
            IF P_SCHEME_CODE = 'OP#09972' THEN
                OPEN P_RESULT FOR SELECT 'Not Allowed in this Scheme Please Select Scheme Tier1 or Tier1+2' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        END IF;

        IF P_INVESTOR_TYPE = '1' AND P_APP_NO = '11' THEN
            IF P_TRAN_CODE IS NULL OR P_TRAN_CODE = '0' THEN
                BEGIN
                    SELECT SCH_CODE, MANUAL_ARNO INTO v_csv_myrs_validate, v_csv_manual_arno
                    FROM TRANSACTION_ST 
                    WHERE CLIENT_CODE = P_INVESTOR_CODE 
                    AND SCH_CODE IN ('OP#09971','OP#09972','OP#09973') 
                    AND TRAN_TYPE IN ('PURCHASE','REINVESTMENT','SWITCH IN')
                    AND ROWNUM = 1;
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        v_csv_myrs_validate := NULL;
                        v_csv_manual_arno := NULL;
                END;
            ELSE 
                BEGIN
                    SELECT SCH_CODE, MANUAL_ARNO INTO v_csv_myrs_validate, v_csv_manual_arno
                    FROM TRANSACTION_ST 
                    WHERE TRAN_CODE <> P_TRAN_CODE 
                    AND CLIENT_CODE = P_INVESTOR_CODE 
                    AND SCH_CODE IN ('OP#09971','OP#09972','OP#09973') 
                    AND TRAN_TYPE IN ('PURCHASE','REINVESTMENT','SWITCH IN')
                    AND ROWNUM = 1;
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        v_csv_myrs_validate := NULL;
                        v_csv_manual_arno := NULL;
                END;
            END IF;

            IF v_csv_myrs_validate IS NOT NULL THEN
                IF v_csv_myrs_validate = 'OP#09971' AND (P_SCHEME_CODE = 'OP#09971' OR P_SCHEME_CODE = 'OP#09972') THEN
                    IF TRIM(v_csv_manual_arno) <> P_MANUAL_AR_NO THEN
                        OPEN P_RESULT FOR SELECT 'Please Enter Same PRAN No' AS STATUS FROM DUAL; 
                        RETURN; 
                    END IF;
                ELSIF v_csv_myrs_validate = 'OP#09973' AND P_SCHEME_CODE = 'OP#09973' THEN
                    IF TRIM(v_csv_manual_arno) <> P_MANUAL_AR_NO THEN
                        OPEN P_RESULT FOR SELECT 'Please Enter Same PRAN No' AS STATUS FROM DUAL; 
                        RETURN; 
                    END IF;
                END IF;
            ELSE 
                IF P_TRAN_CODE IS NULL OR P_TRAN_CODE = '0' THEN
                    BEGIN
                        SELECT SCH_CODE, MANUAL_ARNO INTO v_csv_myrs_validate, v_csv_manual_arno
                        FROM TRANSACTION_ST 
                        WHERE MANUAL_ARNO = P_MANUAL_AR_NO 
                        AND TRAN_TYPE IN ('PURCHASE','REINVESTMENT','SWITCH IN')
                        AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            v_csv_myrs_validate := NULL;
                            v_csv_manual_arno := NULL;
                    END;
                ELSE 
                    BEGIN
                        SELECT SCH_CODE, MANUAL_ARNO INTO v_csv_myrs_validate, v_csv_manual_arno
                        FROM TRANSACTION_ST 
                        WHERE TRAN_CODE <> P_TRAN_CODE 
                        AND MANUAL_ARNO = P_MANUAL_AR_NO 
                        AND TRAN_TYPE IN ('PURCHASE','REINVESTMENT','SWITCH IN')
                        AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            v_csv_myrs_validate := NULL;
                            v_csv_manual_arno := NULL;
                    END;
                END IF;

                IF v_csv_myrs_validate IS NOT NULL THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Different PRAN No.' AS STATUS FROM DUAL; 
                    RETURN;
                END IF;
            END IF;
        END IF;

        IF P_SCHEME_CODE = 'OP#09972' AND P_APP_NO = '11' THEN
            IF P_AMOUNT_T1 = 0 OR P_AMOUNT_T1 IS NULL OR P_AMOUNT_T2 = 0 OR P_AMOUNT_T2 IS NULL THEN
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
            IF P_AMOUNT_T1 = 0 OR P_AMOUNT_T1 IS NULL OR P_AMOUNT_T2 = 0 OR P_AMOUNT_T2 IS NULL THEN
                OPEN P_RESULT FOR SELECT 'Please Enter Tier1 and Tier2 Amount in this Scheme' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        END IF;

        -- CHECK DUP CHEQUE ON V_CLIENT_CAT_DUP_CHEQUE
        IF v_client_cat_dup_cheque <> '4004' THEN
            -- Count matching transactions
            SELECT COUNT(*) 
            INTO v_count_tran_dup_cheque
            FROM TRANSACTION_ST
            WHERE CHEQUE_NO = TRIM(P_CHEQUE_NO)
            AND TRIM(BANK_NAME) = TRIM(P_BANK_NAME)
            AND TRAN_TYPE IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN')
            AND ROWNUM = 1;

            -- If duplicates exist, return error message
            IF v_count_tran_dup_cheque > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' AS STATUS FROM DUAL; 
                RETURN;
            END IF;
        END IF;

        -- Check for duplicate transaction in 'PURCHASE', 'REINVESTMENT', 'SWITCH IN' transactions
        SELECT COUNT(*) INTO v_count_dup_tran
        FROM TRANSACTION_ST
        WHERE CLIENT_CODE = P_INVESTOR_CODE
        AND SCH_CODE = P_SCHEME_CODE
        AND APP_NO = P_APP_NO
        AND AMOUNT = P_AMOUNT_INVESTED
        AND CHEQUE_NO = P_CHEQUE_NO
        AND TRIM(BANK_NAME) = P_BANK_NAME
        AND TRAN_TYPE IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN')
        AND ROWNUM = 1;

        IF v_count_dup_tran > 0 THEN
            OPEN P_RESULT FOR SELECT 'Duplicate Transaction!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        -- Insert into transaction_sttemp
        INSERT INTO TRANSACTION_STTEMP (
            CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, 
            TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, 
            RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, REMARK, DOC_ID, 
            MATURITY_PERIOD, FAMILY_HEAD
        ) VALUES (
            P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, P_PAYMENT_MODE, P_INVESTOR_TYPE, 
            P_DATE, P_INVESTOR_CODE, v_myMutCode, P_SCHEME_CODE, 'PURCHASE', P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, P_CRA_BRANCH, 
            P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, P_CHEQUE_NO, P_CHEQUE_DATE, P_REMARK, P_DT_NUMBER,
            '58', v_found_fam_h
        );

        -- Retrieve recent TRAN_CODE and GST_NO
        BEGIN
            SELECT MAX(TRAN_CODE) 
            INTO v_myTranCode
            FROM TEMP_TRAN
            WHERE BRANCH_CODE = P_BUSINESS_BRANCH
            AND SUBSTR(TRAN_CODE, 1, 2) = '07';

            -- Retrieve the invoice_no from transaction_sttemp based on the tran_code
            SELECT INVOICE_NO
            INTO v_myGstNo
            FROM TRANSACTION_STTEMP
            WHERE TRAN_CODE = v_myTranCode;        
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                OPEN P_RESULT FOR SELECT 'Error generating transaction code' AS STATUS FROM DUAL;
                RETURN;
        END;

        -- Insert into transaction_st
        INSERT INTO TRANSACTION_ST (
            DOC_ID, INVOICE_NO, CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, 
            PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, 
            AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, 
            CHEQUE_NO, CHEQUE_DATE, REMARK, LOGGEDUSERID, INV_NAME,
            MATURITY_PERIOD, FAMILY_HEAD
        ) VALUES ( 
            P_DT_NUMBER, v_myGstNo, P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, 
            P_PAYMENT_MODE, v_myTranCode, P_INVESTOR_TYPE, SYSDATE, P_INVESTOR_CODE, v_myMutCode, P_SCHEME_CODE, 'PURCHASE', 
            P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, SUBSTR(P_INVESTOR_CODE, 1, 8), P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, 
            P_CHEQUE_NO, P_CHEQUE_DATE, 'NPS', P_LOGGEDIN_USER, P_SUBSCRIBER_NAME,
            '58', v_found_fam_h
        );

        -- Insert into nps_transaction
        INSERT INTO NPS_TRANSACTION (TRAN_CODE, AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK) 
        VALUES (v_myTranCode, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK);
        COMMIT;

        SELECT UNIQUE_ID INTO V_UNIQUE_ID
        FROM TRANSACTION_ST WHERE TRAN_CODE = v_myTranCode AND ROWNUM = 1;


        v_return_message := 'Your Transaction No Is ' || v_myTranCode || ' and Your Recpt No Is ' ||  V_UNIQUE_ID ;

        -- DOUBLE TRANSACTION OF CONTRIBUTION WHEN REGISTRATION AND INV_TYPE IS OptIndividual
        IF P_APP_NO = '11' AND P_INVESTOR_TYPE = '0' THEN
            INSERT INTO TRANSACTION_STTEMP (
                CORPORATE_NAME, REF_TRAN_CODE, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, 
                PAYMENT_MODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, 
                TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, 
                BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, remark, doc_id
            ) 
            SELECT 
                CORPORATE_NAME, TRAN_CODE, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, 
                PAYMENT_MODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, 
                TRAN_TYPE, 0, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, 
                BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, remark, ''
            FROM transaction_sttemp 
            WHERE tran_code = v_myTranCode;
            COMMIT;

            -- Retrieve the maximum tran_code from temp_tran where branch_code matches the input
            BEGIN
                SELECT MAX(TRAN_CODE) INTO v_myTranCode1
                FROM TEMP_TRAN 
                WHERE BRANCH_CODE = P_BUSINESS_BRANCH 
                AND SUBSTR(TRAN_CODE,1,2) = '07';
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN P_RESULT FOR SELECT 'Error generating secondary transaction code' AS STATUS FROM DUAL;
                    RETURN;
            END;

            UPDATE TB_DOC_UPLOAD 
            SET AR_CODE = v_myTranCode1 
            WHERE COMMON_ID = P_DT_NUMBER;
            COMMIT;

            INSERT INTO TRANSACTION_ST (
                REF_TRAN_CODE, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, 
                TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, 
                TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, 
                BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, remark, LOGGEDUSERID, doc_id
            ) 
            SELECT 
                REF_TRAN_CODE, MANUAL_ARNO, BANK_NAME, FOLIO_NO, '12', PAYMENT_MODE, 
                TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, 
                TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, 
                BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, remark, LOGGEDUSERID, doc_id 
            FROM transaction_sttemp 
            WHERE tran_code = v_myTranCode1;
            COMMIT;

            INSERT INTO NPS_TRANSACTION (
                TRAN_CODE, AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK
            ) 
            VALUES (
                v_myTranCode1, 0, 0, 0, 0, 0, P_REMARK
            );
            COMMIT;

            DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE = v_myTranCode1;
            COMMIT;

            BEGIN
                SELECT UNIQUE_ID INTO v_mySecReq
                FROM TRANSACTION_ST 
                WHERE TRAN_CODE = v_myTranCode1
                AND ROWNUM = 1;

                v_return_message := 'Your Duplicate Transaction No Is ' || v_myTranCode1 || 
                                   ' and Your Recpt No IS ' || v_mySecReq;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    v_return_message := 'Transaction completed but could not retrieve receipt number';
            END;
        END IF;

        DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE = v_myTranCode;
        COMMIT;

        -- CHECK v_return_message IS NOT NULL MEAN HASING ANY SUCCESS MESSAGE THEN OPEN P_RESULT
        IF v_return_message IS NOT NULL THEN
            OPEN P_RESULT FOR
            SELECT 'TRANSACTION PUNCHED SUCCESSFULLY: ' || v_return_message AS STATUS FROM DUAL;
            RETURN;
        END IF;

    ELSIF P_MARK = '4' THEN
        -- Fetch CLIENT_CODE and TRAN_CODE
        /*
        BEGIN
            SELECT CLIENT_CODE, TRAN_CODE
            INTO v_newClientCode, v_myTranCode
            FROM TRANSACTION_ST
            WHERE DOC_ID = P_DT_NUMBER
            AND ROWNUM = 1;
        EXCEPTION 
            WHEN NO_DATA_FOUND THEN 
                OPEN P_RESULT FOR SELECT 'Transaction not found' AS STATUS FROM DUAL;
                RETURN;
        END;*/

        -- Update transaction_st
        UPDATE TRANSACTION_ST
        SET TR_DATE = P_DATE, 
            CLIENT_CODE = P_INVESTOR_CODE, 
            SOURCE_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8),
            BUSI_BRANCH_CODE = P_BUSINESS_BRANCH, 
            BUSINESS_RMCODE = P_BUSINESS_RM, 
            MUT_CODE = v_myMutCode, 
            SCH_CODE = P_SCHEME_CODE, 
            AMOUNT = P_AMOUNT_INVESTED, 
            FOLIO_NO = P_FOLIO_NUMBER, 
            APP_NO = P_APP_NO, 
            PAYMENT_MODE = P_PAYMENT_MODE, 
            CHEQUE_DATE = P_CHEQUE_DATE, 
            CHEQUE_NO = P_CHEQUE_NO, 
            BANK_NAME = P_BANK_NAME, 
            MANUAL_ARNO = P_MANUAL_AR_NO, 
            CORPORATE_NAME = P_CORPORATE_NAME, 
            UNIQUE_ID = P_RECEIPT_NO, 
            MODIFY_USER = P_LOGGEDIN_USER, 
            MODIFY_DATE = P_DATE, 
            REMARK = P_REMARK,
            MATURITY_PERIOD = '58', 
            FAMILY_HEAD = v_found_fam_h
        WHERE TRAN_CODE = P_TRAN_CODE;
        COMMIT;

        -- INSERT/UPDATE IN nps_transaction
        BEGIN
            SELECT COUNT(TRAN_CODE) 
            INTO v_flag_nps_tran 
            FROM NPS_TRANSACTION 
            WHERE TRAN_CODE = P_TRAN_CODE; 

            IF v_flag_nps_tran = 0 THEN 
                INSERT INTO NPS_TRANSACTION (
                    TRAN_CODE, AMOUNT1, AMOUNT2, REG_CHARGE, TRAN_CHARGE, SERVICETAX, REMARK
                ) 
                VALUES (
                    P_TRAN_CODE, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK
                );
            ELSE
                UPDATE NPS_TRANSACTION
                SET AMOUNT1 = P_AMOUNT_T1, 
                    AMOUNT2 = P_AMOUNT_T2, 
                    REG_CHARGE = P_RECHARGE1, 
                    TRAN_CHARGE = P_RECHARGE2, 
                    SERVICETAX = P_GST_TAX, 
                    REMARK = P_REMARK
                WHERE TRAN_CODE = P_TRAN_CODE;
            END IF;
            COMMIT;
        EXCEPTION
            WHEN OTHERS THEN
                v_err_ret_msg := 'Error updating NPS transaction: ' || SQLERRM;                
                OPEN P_RESULT FOR SELECT v_err_ret_msg AS STATUS FROM DUAL;
                RETURN;
        END;        

        OPEN P_RESULT FOR
            SELECT 'Updation successful' AS STATUS FROM DUAL;
            return;
    END IF;
    /*
EXCEPTION
    WHEN OTHERS THEN
        v_err_ret_msg := 'Error in procedure: ' || SQLERRM;
        OPEN P_RESULT FOR SELECT v_err_ret_msg AS STATUS FROM DUAL;
        return;*/
END PSM_NPS_INSERT_UPDATE_PRA;
/