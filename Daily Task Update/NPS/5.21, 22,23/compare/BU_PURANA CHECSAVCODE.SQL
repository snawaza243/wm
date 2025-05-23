
        IF TRIM(P_INVESTOR_CODE) IS NULL THEN
             OPEN P_RESULT FOR SELECT 'Please select a Investor!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        IF TRIM(P_APP_NO) IS NULL THEN
             OPEN P_RESULT FOR SELECT 'Please Select a Request Id!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;


        IF P_DATE IS NULL THEN
            OPEN P_RESULT FOR SELECT 'Please enter a correct Transaction Date!' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        
        IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE('01/01/1980', 'DD/MM/YYYY') THEN 
            OPEN P_RESULT FOR SELECT 'Please enter a correct transaction date! ' AS STATUS FROM DUAL; 
            RETURN;
        END IF; 

        -- Validate if the transaction date is greater than the current date
        IF TO_DATE(P_DATE, 'DD/MM/YYYY') > SYSDATE THEN
            OPEN P_RESULT FOR SELECT 'Transaction Date Cannot Be Greater than Current Date' AS STATUS FROM DUAL; 
            RETURN;
        END IF;

        -- Validate Payment Mode
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



        -- Validate Amount Input
        IF P_AMOUNT_INVESTED IS NULL OR TRIM(P_AMOUNT_INVESTED) = '' THEN
            OPEN P_RESULT FOR SELECT 'Please enter amount.' AS STATUS FROM DUAL;
            RETURN;
        END IF;

        IF NOT REGEXP_LIKE(P_AMOUNT_INVESTED, '^[0-9]+(\.[0-9]+)?$') THEN
            OPEN P_RESULT FOR SELECT 'Please enter a correct amount.' AS STATUS FROM DUAL;
            RETURN;
        END IF;

        -- Validate RM Business Code
        IF P_BUSINESS_RM IS NULL OR LENGTH(TRIM(P_BUSINESS_RM)) < 5 THEN
            OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' AS STATUS FROM DUAL;
            RETURN;
        END IF;

        -- Check if RM Business Code exists in employee_master
        DECLARE
            V_RM_CODE VARCHAR2(100);
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

        -- Check for duplicate transaction in 'PURCHASE', 'REINVESTMENT', 'SWITCH IN' transactions
        SELECT COUNT(*) INTO V_COUNT_TRAN_DUP_CHEQUE
        FROM TRANSACTION_ST
        WHERE CLIENT_CODE = TRIM(P_INVESTOR_CODE)
          AND SCH_CODE = P_SCHEME_CODE
          AND APP_NO = P_APP_NO
          AND AMOUNT = P_AMOUNT_INVESTED
          AND CHEQUE_NO = TRIM(P_CHEQUE_NO)
          AND TRIM(BANK_NAME) = TRIM(P_BANK_NAME)
          AND TRAN_TYPE IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN');

        -- If duplicates found, return error message
        IF V_COUNT_TRAN_DUP_CHEQUE > 0 THEN

            OPEN P_RESULT FOR SELECT 'Duplicate Transaction!' AS STATUS FROM DUAL;
            RETURN;
        END IF;