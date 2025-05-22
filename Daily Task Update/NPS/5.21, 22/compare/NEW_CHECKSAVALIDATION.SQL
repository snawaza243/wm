        BEGIN -- chkSaveValidation

            IF P_PRODUCT_CLASS IS NULL OR TRIM(P_PRODUCT_CLASS) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Product Class.' FROM DUAL;
                RETURN;
            END IF;


            -- MUTCODE IS MANUAL_AR
            IF P_MANUAL_AR_NO IS NULL OR TRIM(P_MANUAL_AR_NO) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Product.' FROM DUAL;
                RETURN;
            END IF;





            IF P_SCHEME_CODE IS NULL OR TRIM(P_SCHEME_CODE) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Scheme Code.' FROM DUAL;
                RETURN;
            END IF;

            IF P_INVESTOR_CODE IS NULL OR TRIM(P_INVESTOR_CODE) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please select a Investor.' FROM DUAL;
                RETURN;
            END IF;



            IF P_APP_NO IS NULL OR TRIM(P_APP_NO) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please Select a Request Id.' FROM DUAL;
                RETURN;
            END IF;



            IF P_DATE IS NULL THEN
                OPEN P_RESULT FOR SELECT 'Please enter a correct Transaction Date.' FROM DUAL;
                RETURN;
            END IF;


            IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE('01/01/1980', 'DD/MM/YYYY') THEN 
                OPEN P_RESULT FOR SELECT 'Please enter a correct transaction date!' FROM dual;
                RETURN;
            END IF;



            IF P_AMOUNT_INVESTED IS NULL OR TRIM(P_AMOUNT_INVESTED) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please enter amount.' FROM DUAL;
                RETURN;
            END IF;


        
            IF NOT REGEXP_LIKE(P_AMOUNT_INVESTED, '^[0-9]+(\.[0-9]+)?$') THEN
                OPEN P_RESULT FOR SELECT 'Please enter a correct amount.' FROM DUAL;
                RETURN;
            END IF;

            IF P_BUSINESS_RM IS NULL OR LENGTH(TRIM(P_BUSINESS_RM)) < 5 THEN
                OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' FROM DUAL;
                RETURN;
            END IF;

            BEGIN
                SELECT RM_CODE INTO V_RM_CODE
                FROM EMPLOYEE_MASTER
                WHERE PAYROLL_ID = TRIM(P_BUSINESS_RM)
                AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' FROM DUAL;
                    RETURN;
            END;


            -- Check if the transaction date is within the allowed range
            IF P_MARK = 0 THEN
                IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE('01/01/1980', 'DD/MM/YYYY') OR 
                TO_DATE(P_DATE, 'DD/MM/YYYY') > TO_DATE('31/12/2099', 'DD/MM/YYYY') THEN
                    OPEN P_RESULT FOR SELECT 'Transaction Date is out of allowed range.' FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            -- Check if the transaction date is greater than the current date
            IF TO_DATE(P_DATE, 'DD/MM/YYYY') > SYSDATE THEN
                OPEN P_RESULT FOR SELECT 'Transaction Date Cannot Be Greater than Current Date' FROM DUAL;
                RETURN;
            END IF;

            -- Validate Payment Mode
            /* Payment Mode Codes
                Cheque = C
                Draft = D
                Cash = H
                ECS = E
                Corporate = M
                Others = R
            */

            IF P_PAYMENT_MODE IS NULL OR TRIM(P_PAYMENT_MODE) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please Select a Payment Mode.' FROM DUAL;
                RETURN;
            END IF;


            -- Validate Cheque Payment Mode
            IF P_PAYMENT_MODE = 'C' THEN
                -- Validate Bank Name
                IF P_BANK_NAME IS NULL OR TRIM(P_BANK_NAME) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Select a Bank Name' FROM DUAL;
                    RETURN;
                END IF;

                -- Validate Cheque Number
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Cheque Number' FROM DUAL;
                    RETURN;
                END IF;

                -- Validate Cheque Date
                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Cheque Date' FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            -- Validate Payment Mode Conditions
            IF P_PAYMENT_MODE = 'D' THEN -- Draft Payment
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Draft Number' FROM DUAL;
                    RETURN;
                END IF;

                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Draft Date' FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            -- Validate ECS Payment Mode
            IF P_PAYMENT_MODE = 'E' THEN -- ECS Payment
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a MCR Number' FROM DUAL;
                    RETURN;
                END IF;

                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a Date' FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            -- VALIDATE OTHER PAYMENT MODES
            IF P_PAYMENT_MODE = 'R' THEN -- Cash Payment
                IF P_CHEQUE_NO IS NULL OR TRIM(P_CHEQUE_NO) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert a FDR Number' FROM DUAL;
                    RETURN;
                END IF;

                IF P_CHEQUE_DATE IS NULL OR TRIM(P_CHEQUE_DATE) = '' THEN
                    OPEN P_RESULT FOR SELECT 'Please Insert renewal Date' FROM DUAL;
                    RETURN;
                END IF;
                
                IF P_CHEQUE_DATE IS NOT NULL THEN
                    dtf := '01/' || TO_CHAR(SYSDATE, 'MM/YYYY');
                    DtL := LAST_DAY(ADD_MONTHS(TO_DATE(dtf, 'DD/MM/YYYY'), 1)) - 1;
                    IF TO_DATE(P_CHEQUE_DATE, 'DD/MM/YYYY') < TO_DATE(dtf, 'DD/MM/YYYY') OR
                    TO_DATE(P_CHEQUE_DATE, 'DD/MM/YYYY') > DtL THEN
                        OPEN P_RESULT FOR SELECT 'Cheque/Draft Date Should be in Current Month' FROM DUAL;
                        RETURN;
                    END IF;
                END IF;

            END IF;

            -- Validate Business Branch
            IF P_BUSINESS_BRANCH IS NULL OR TRIM(P_BUSINESS_BRANCH) = '' THEN
                OPEN P_RESULT FOR SELECT 'Please Select Business Branch' FROM DUAL;
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
                    OPEN P_RESULT FOR SELECT 'Not Allowed in this Scheme Please Select Scheme Tier1 or Tier1+2' FROM DUAL;
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
                            OPEN P_RESULT FOR SELECT 'Please Enter Same PRAN No' FROM DUAL;
                            RETURN; 
                        END IF;
                    ELSIF CSV_MyRs_validate = 'OP#09973' AND P_SCHEME_CODE = "OP#09973" THEN
                        IF TRIM(CSV_manual_arno) <> P_MANUAL_AR_NO THEN
                            OPEN P_RESULT FOR SELECT 'Please Enter Same PRAN No' FROM DUAL;
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
                        OPEN P_RESULT FOR SELECT 'Please Enter Different PRAN No.' FROM DUAL;

                    END IF;

                END IF;

                OPEN P_RESULT FOR SELECT 'Please select a transaction code to Modify.' AS STATUS FROM DUAL;
                RETURN; 
            END IF;

            IF P_SCHEME_CODE = 'OP#09972' AND P_APP_NO = '11' THEN
                IF P_AMOUNT_T1 = 0 OR P_AMOUNT_T1 IS NULL OR P_AMOUNT_T2 =0 OR P_AMOUNT_T2 IS NULL THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Tier1 and Tier2 Amount in this Scheme' FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            IF P_APP_NO = '11' AND P_SCHEME_CODE = 'OP#09973' THEN
                IF P_AMOUNT_T1 < 500 OR P_AMOUNT_T2 < 1000 THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Correct Amount in this Scheme' FROM DUAL;
                    RETURN;
                END IF;
            END IF;

            IF P_APP_NO = '11' AND P_SCHEME_CODE = 'OP#09973' THEN
                IF P_AMOUNT_T1 = 0 OR P_AMOUNT_T1 IS NULL OR P_AMOUNT_T2 =0 OR P_AMOUNT_T2 IS NULL THEN
                    OPEN P_RESULT FOR SELECT 'Please Enter Tier1 and Tier2 Amount in this Scheme' FROM DUAL;
                    RETURN;
                END IF;
            END IF;
        END; -- chkSaveValidation
       
