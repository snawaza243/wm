create or replace PROCEDURE PSM_NPS_INSERT_UPDATE_PRA(
    P_MARK               IN VARCHAR2,
    P_PRODUCT_CLASS      IN VARCHAR2,
    P_INVESTOR_TYPE      IN VARCHAR2,
    P_CORPORATE_NAME     IN VARCHAR2,
    P_DT_NUMBER          IN VARCHAR2,
    P_TRAN_CODE          IN VARCHAR2,
    P_INVESTOR_CODE      IN NUMBER,
    P_SCHEME_CODE        IN VARCHAR2,
    P_CRA                IN VARCHAR2,
    P_CRA_BRANCH         IN NUMBER,
    P_FOLIO_NUMBER       IN VARCHAR2,
    P_BUSINESS_RM        IN NUMBER,
    P_BUSINESS_BRANCH    IN NUMBER,
    P_RECEIPT_NO         IN VARCHAR2,
    P_PAYMENT_MODE       IN CHAR,
    P_CHEQUE_NO          IN VARCHAR2,
    P_BANK_NAME          IN VARCHAR2,
    P_APP_NO             IN VARCHAR2,
    P_CHEQUE_DATE        IN DATE,
    P_DATE               IN DATE,
    P_TIME               IN DATE,
    P_COMBINED_DATETIME  IN DATE,
    P_SUBSCRIBER_NAME    IN VARCHAR2,
    P_MANUAL_AR_NO       IN VARCHAR2,
    P_UNFREEZE           IN VARCHAR2,
    P_AMOUNT_T1          IN NUMBER,
    P_AMOUNT_T2          IN NUMBER,
    P_RECHARGE1          IN NUMBER,
    P_RECHARGE2          IN NUMBER,
    P_GST_TAX            IN NUMBER,
    P_COLLECTION_AMOUNT  IN NUMBER,
    P_AMOUNT_INVESTED    IN NUMBER,
    P_AMOUNT_INVESTED2   IN NUMBER,
    P_REMARK             IN VARCHAR2,
    P_ZERO_COM           IN VARCHAR2,
    P_LOGGEDIN_USER      IN VARCHAR2,
    P_RESULT             OUT SYS_REFCURSOR
) AS
    MyTranCode          VARCHAR2(100);
    MyGSTNO             VARCHAR2(100);
    NewClientCode       VARCHAR2(100);
    MyMutCode           VARCHAR2(100);
    currentRole         NUMBER;
    temprole            NUMBER;
    v_flag_nps_tran     NUMBER;
    v_generated_tr      NUMBER;



    GlbroleId                   NUMBER:=0;
    v_Count_FATCA               NUMBER;     -- FACTA VALIDATION
    v_client_cat_dup_cheque     VARCHAR2(10);
    v_count_tran_dup_cheque     NUMBER;
    v_tran_code_insertion       VARCHAR2(50):= NULL;
    v_gst_no_insertion          VARCHAR2(50):= NULL;
BEGIN

BEGIN -- Get GlbroleId by login
    BEGIN
        SELECT ROLE_ID 
        INTO GlbroleId 
        FROM USERDETAILS_Ji 
        WHERE LOGIN_ID = P_LOGGEDIN_USER /*'38387'  */
        /* AND ROLE_ID  = 212 */ 
        AND ROWNUM = 1;

    EXCEPTION 
        WHEN NO_DATA_FOUND THEN 
            GlbroleId := 0;
    END;
END;

BEGIN -- Fetch ISS_CODE for the provided P_SCHEME_CODE

    BEGIN
        SELECT ISS_CODE
        INTO MyMutCode
        FROM other_product
        WHERE OSCH_CODE = P_SCHEME_CODE 
        AND ROWNUM = 1;

    EXCEPTION 
        WHEN NO_DATA_FOUND THEN 
            MyMutCode := NULL;
    END;
END;



BEGIN -- VALIDATE PUNCHING AND MIDIFICATION TEAM
    IF P_MARK = 0 THEN -- 0 FOR PUNCING , 4 FOR MODIFYING
        IF GlbroleId NOT IN (212,1) THEN
            OPEN P_RESULT FOR SELECT 'Only Punching Team can punch the transaction.' AS STATUS FROM DUAL;
            RETURN;  
        END IF;

    ELSIF P_MARK = 4 THEN
        IF GlbroleId NOT IN (146, 1) THEN
            OPEN P_RESULT FOR 
            SELECT 'Only NPS Team can modify the transaction.' AS STATUS FROM DUAL;
            RETURN; -- Exit the procedure/function
        END IF;
    END IF;
END;

BEGIN -- DT NUMBER IS REQUIRED AND RETURN    
    IF P_DT_NUMBER IS NULL OR TRIM(P_DT_NUMBER) IS NULL  AND TRIM(P_MARK) = 0 THEN 
        OPEN P_RESULT FOR SELECT 'DT No cannot be left blank.' AS STATUS FROM DUAL;
        RETURN; 
    END IF;
END;

BEGIN -- FATCA_VALIDATION 
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
                INTO v_Count_FATCA
                FROM NPS_FATCA_NON_COMPLIANT
                WHERE PRAN_NO = P_MANUAL_AR_NO;

                IF v_Count_FATCA >= 1 THEN
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
    IF P_MARK = 0 OR P_MARK = 4 THEN
        -- Fetch Client Category
        SELECT category_id 
        INTO v_client_cat_dup_cheque
        FROM client_master 
        WHERE client_code = SUBSTR(P_INVESTOR_CODE, 1, 8);

        -- If client category is not "4004"
        IF v_client_cat_dup_cheque <> '4004' THEN
            -- Check if transaction code is "0"
            IF P_TRAN_CODE = '0' OR TRIM(P_TRAN_CODE) = '' THEN
                -- Check for duplicate cheque number in the last 6 months
                SELECT COUNT(*) INTO v_count_tran_dup_cheque 
                FROM (
                    SELECT TRAN_CODE 
                    FROM transaction_st 
                    WHERE mut_code = 'IS02520' 
                    AND cheque_no = TRIM(P_CHEQUE_NO) 
                    AND tr_date >= ADD_MONTHS(SYSDATE, -6) 

                    UNION ALL 

                    SELECT TRAN_CODE 
                    FROM transaction_sttemp 
                    WHERE mut_code = 'IS02520' 
                    AND cheque_no = TRIM(P_CHEQUE_NO) 
                    AND tr_date >= ADD_MONTHS(SYSDATE, -6)
                );

                IF v_count_tran_dup_cheque > 0 THEN
                    OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' from dual;
                    return;
                END IF;

            ELSE
                -- If ReqCode = "11"
                IF P_APP_NO = '11' THEN
                    -- Check for duplicate cheque number in the last 6 months, excluding current TRAN_CODE
                    SELECT COUNT(*) INTO v_count_tran_dup_cheque 
                    FROM (
                        SELECT TRAN_CODE 
                        FROM transaction_st 
                        WHERE TRAN_CODE <> P_TRAN_CODE
                        AND mut_code = 'IS02520' 
                        AND cheque_no = TRIM(P_CHEQUE_NO) 
                        AND tr_date >= ADD_MONTHS(SYSDATE, -6)
                        AND REF_TRAN_CODE IS NULL

                        UNION ALL 

                        SELECT TRAN_CODE 
                        FROM transaction_sttemp 
                        WHERE mut_code = 'IS02520' 
                        AND cheque_no = TRIM(P_CHEQUE_NO) 
                        AND tr_date >= ADD_MONTHS(SYSDATE, -6) 
                        AND TRAN_CODE <> P_TRAN_CODE
                    );

                    IF v_count_tran_dup_cheque > 0 THEN
                        --p_Duplicate_Found := 1;
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' from dual;
                        return;
                    END IF;

                ELSE
                    -- Check for duplicate cheque number in the last 6 months, excluding current TRAN_CODE
                    SELECT COUNT(*) INTO v_count_tran_dup_cheque 
                    FROM (
                        SELECT TRAN_CODE 
                        FROM transaction_st 
                        WHERE TRAN_CODE <> P_TRAN_CODE
                        AND mut_code = 'IS02520' 
                        AND cheque_no = TRIM(P_CHEQUE_NO) 
                        AND tr_date >= ADD_MONTHS(SYSDATE, -6)

                        UNION ALL 

                        SELECT TRAN_CODE 
                        FROM transaction_sttemp 
                        WHERE mut_code = 'IS02520' 
                        AND cheque_no = TRIM(P_CHEQUE_NO) 
                        AND tr_date >= ADD_MONTHS(SYSDATE, -6) 
                        AND TRAN_CODE <> P_TRAN_CODE
                    );

                    IF v_count_tran_dup_cheque > 0 THEN
                        --p_Duplicate_Found := 1;
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' from dual;
                        return;
                    END IF;
                END IF;
            END IF;
        END IF;
    END IF;

    -- If no duplicate found, return 0
    -- p_Duplicate_Found := 0;
END ;


BEGIN -- SAVE = 0 : CHECK_DUPLICATE_CHEQUE
    IF P_MARK = 0 THEN
        IF v_client_cat_dup_cheque <> '4004' THEN
            -- Count matching transactions
            SELECT COUNT(*) INTO v_count_tran_dup_cheque
            FROM transaction_st
            WHERE cheque_no = TRIM(P_CHEQUE_NO)
              AND TRIM(bank_name) = TRIM(P_BANK_NAME)
              AND tran_type IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN');

            -- If duplicates exist, return error message
            IF v_count_tran_dup_cheque > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' from dual;
                RETURN;
            END IF;
        END IF;



        -- Check for duplicate transaction in 'PURCHASE', 'REINVESTMENT', 'SWITCH IN' transactions
        SELECT COUNT(*) INTO v_count_tran_dup_cheque
        FROM transaction_st
        WHERE CLIENT_code = TRIM(P_INVESTOR_CODE)
          AND sch_code = P_SCHEME_CODE
          AND app_no = P_APP_NO
          AND amount = P_AMOUNT_INVESTED
          AND cheque_no = TRIM(P_CHEQUE_NO)
          AND TRIM(bank_name) = TRIM(P_BANK_NAME)
          AND tran_type IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN');

        -- If duplicates found, return error message
        IF v_count_tran_dup_cheque > 0 THEN

            OPEN P_RESULT FOR SELECT 'Duplicate Transaction!' from dual;
            RETURN;
        END IF;
    end if;
END;









IF P_MARK = 0 THEN
    -- Insert into transaction_sttemp
    INSERT INTO transaction_sttemp (
        CORPORATE_NAME, manual_arno, BANK_NAME, folio_no, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, 
        TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, 
        RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, doc_id
    ) VALUES (
        P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, P_PAYMENT_MODE, P_INVESTOR_TYPE, 
        P_DATE, P_INVESTOR_CODE, MyMutCode, P_SCHEME_CODE, 'PURCHASE', P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, P_CRA_BRANCH, 
        P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, P_CHEQUE_NO, P_CHEQUE_DATE, P_REMARK, P_DT_NUMBER
    );

    BEGIN -- Retrieve the maximum tran_code from temp_tran where branch_code matches the input
        SELECT MAX(tran_code) 
        INTO v_tran_code_insertion
        FROM temp_tran
        WHERE branch_code = P_BUSINESS_BRANCH
        AND SUBSTR(tran_code, 1, 2) = '07';

        -- Retrieve the invoice_no from transaction_sttemp based on the tran_code
        SELECT invoice_no
        INTO v_gst_no_insertion
        FROM transaction_sttemp
        WHERE tran_code = v_tran_code_insertion; 

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            -- If no data is found for either query, set the output parameters to NULL
            v_tran_code_insertion := NULL;
            v_gst_no_insertion := NULL;
        WHEN OTHERS THEN
            -- Handle any other errors
            v_tran_code_insertion := NULL;
            v_gst_no_insertion := NULL;
            RAISE;
    END ;

    -- Insert into transaction_st
    INSERT INTO transaction_st (
        doc_id, invoice_no, CORPORATE_NAME, manual_arno, BANK_NAME, FOLIO_NO, APP_NO, 
        PAYMENT_MODE, TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, 
        AMOUNT, BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, 
        cheque_no, CHEQUE_DATE, remark, LOGGEDUSERID, inv_name
    ) VALUES ( 
        P_DT_NUMBER, v_gst_no_insertion /*P_GST_TAX*/ , P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, 
        P_PAYMENT_MODE, MyTranCode, P_INVESTOR_TYPE, SYSDATE,P_INVESTOR_CODE /*NewClientCode*/, MyMutCode, P_SCHEME_CODE, 'PURCHASE', 
        P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, SUBSTR(P_INVESTOR_CODE, 1, 8), P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, 
        P_CHEQUE_NO, P_CHEQUE_DATE, 'NPS', P_LOGGEDIN_USER, P_SUBSCRIBER_NAME
    );

     BEGIN
        SELECT ts.TRAN_CODE INTO v_generated_tr
        FROM transaction_st ts
        WHERE ts.DOC_ID = P_DT_NUMBER;
          EXCEPTION WHEN NO_DATA_FOUND THEN v_generated_tr :=0;
    END;

    -- Insert into nps_transaction
    INSERT INTO nps_transaction (
        tran_code, amount1, amount2, reg_charge, tran_charge, SERVICETAX, remark
    ) VALUES (
        v_generated_tr, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK
    );

    -- Return confirmation
    OPEN P_RESULT FOR
        SELECT 'Insertion successful and Tran Code Is ' || ts.TRAN_CODE AS STATUS
        FROM transaction_st ts
        WHERE ts.DOC_ID = P_DT_NUMBER;
ELSE

    BEGIN -- Fetch CLIENT_CODE and TRAN_CODE

        BEGIN
            SELECT CLIENT_CODE, TRAN_CODE
            INTO NewClientCode, MyTranCode
            FROM transaction_st
            WHERE DOC_ID = P_DT_NUMBER;

        EXCEPTION 
            WHEN NO_DATA_FOUND THEN 
                NewClientCode := NULL; 
                MyTranCode := NULL;
        END;
    END;

    -- Update transaction_st
    UPDATE transaction_st
    SET TR_DATE = P_DATE, CLIENT_CODE = P_INVESTOR_CODE, SOURCE_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8),
        BUSI_BRANCH_CODE = P_BUSINESS_BRANCH, BUSINESS_RMCODE = P_BUSINESS_RM, MUT_CODE = MyMutCode, 
        SCH_CODE = P_SCHEME_CODE, AMOUNT = P_AMOUNT_INVESTED, FOLIO_NO = P_FOLIO_NUMBER, APP_NO = P_APP_NO, 
        PAYMENT_MODE = P_PAYMENT_MODE, CHEQUE_DATE = P_CHEQUE_DATE, CHEQUE_NO = P_CHEQUE_NO, 
        BANK_NAME = P_BANK_NAME, MANUAL_ARNO = P_MANUAL_AR_NO, CORPORATE_NAME = P_CORPORATE_NAME, 
        UNIQUE_ID = P_RECEIPT_NO, MODIFY_USER = P_LOGGEDIN_USER, MODIFY_DATE = P_DATE, REMARK = P_REMARK
    WHERE TRAN_CODE = P_TRAN_CODE;


    -- INSERT/UPDATE IN nps_transaction
        -- FLAG FOR CHECKING DATA EXIST IN nps_transaction OR NOT 
        BEGIN 
            SELECT TRAN_CODE 
            INTO v_flag_nps_tran 
            FROM nps_transaction WHERE TRAN_CODE = P_TRAN_CODE; 
            EXCEPTION WHEN NO_DATA_FOUND THEN v_flag_nps_tran :=0;
        END;

        IF v_flag_nps_tran = 0 THEN 
            INSERT INTO nps_transaction ( tran_code, amount1, amount2, reg_charge, tran_charge, SERVICETAX, remark ) VALUES (v_generated_tr, P_AMOUNT_T1, P_AMOUNT_T2, P_RECHARGE1, P_RECHARGE2, P_GST_TAX, P_REMARK);
        ELSE
            UPDATE nps_transaction
            SET AMOUNT1 = P_AMOUNT_T1, AMOUNT2 = P_AMOUNT_T2, REG_CHARGE = P_RECHARGE1, TRAN_CHARGE = P_RECHARGE2, 
                SERVICETAX = P_GST_TAX, REMARK = P_REMARK
            WHERE TRAN_CODE = P_TRAN_CODE;
        END IF;

    -- END INSERT/UPDATE IN nps_transaction

    -- FINAL RESULT Return confirmation
    OPEN P_RESULT FOR
        SELECT 'Updation successful ' AS STATUS
        FROM DUAL;
END IF;



END PSM_NPS_INSERT_UPDATE_PRA;