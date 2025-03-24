CREATE OR REPLACE PROCEDURE PSM_NPS_INSERT_UPDATE_PRA(
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
    MYTRANCODE          VARCHAR2(100);
    MYGSTNO             VARCHAR2(100);
    NEWCLIENTCODE       VARCHAR2(100);
    MYMUTCODE           VARCHAR2(100);
    CURRENTROLE         NUMBER;
    TEMPROLE            NUMBER;
    V_FLAG_NPS_TRAN     NUMBER;
    V_GENERATED_TR      NUMBER;



    GLBROLEID                   NUMBER:=0;
    GLBROLEID2                   NUMBER:=0;

    V_COUNT_FATCA               NUMBER;     -- FACTA VALIDATION
    V_CLIENT_CAT_DUP_CHEQUE     VARCHAR2(10);
    V_COUNT_TRAN_DUP_CHEQUE     NUMBER;
    V_TRAN_CODE_INSERTION       VARCHAR2(50):= NULL;
    V_GST_NO_INSERTION          VARCHAR2(50):= NULL;
BEGIN


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


-- For testing procedure value
/*
OPEN P_RESULT FOR  
SELECT 
    'P_MARK --> ' || P_MARK || ', ' ||
    'P_PRODUCT_CLASS --> ' || P_PRODUCT_CLASS || ', ' ||
    'P_INVESTOR_TYPE --> ' || P_INVESTOR_TYPE || ', ' ||
    'P_CORPORATE_NAME --> ' || P_CORPORATE_NAME || ', ' ||
    'P_DT_NUMBER --> ' || P_DT_NUMBER || ', ' ||
    'P_TRAN_CODE --> ' || P_TRAN_CODE || ', ' ||
    'P_INVESTOR_CODE --> ' || P_INVESTOR_CODE || ', ' ||
    'P_SCHEME_CODE --> ' || P_SCHEME_CODE || ', ' ||
    'P_CRA --> ' || P_CRA || ', ' ||
    'P_CRA_BRANCH --> ' || P_CRA_BRANCH || ', ' ||
    'P_FOLIO_NUMBER --> ' || P_FOLIO_NUMBER || ', ' ||
    'P_BUSINESS_RM --> ' || P_BUSINESS_RM || ', ' ||
    'P_BUSINESS_BRANCH --> ' || P_BUSINESS_BRANCH || ', ' ||
    'P_RECEIPT_NO --> ' || P_RECEIPT_NO || ', ' ||
    'P_PAYMENT_MODE --> ' || P_PAYMENT_MODE || ', ' ||
    'P_CHEQUE_NO --> ' || P_CHEQUE_NO || ', ' ||
    'P_BANK_NAME --> ' || P_BANK_NAME || ', ' ||
    'P_APP_NO --> ' || P_APP_NO || ', ' ||
    'P_CHEQUE_DATE --> ' || P_CHEQUE_DATE || ', ' ||
    'P_DATE --> ' || P_DATE || ', ' ||
    'P_TIME --> ' || P_TIME || ', ' ||
    'P_COMBINED_DATETIME --> ' || P_COMBINED_DATETIME || ', ' ||
    'P_SUBSCRIBER_NAME --> ' || P_SUBSCRIBER_NAME || ', ' ||
    'P_MANUAL_AR_NO --> ' || P_MANUAL_AR_NO || ', ' ||
    'P_UNFREEZE --> ' || P_UNFREEZE || ', ' ||
    'P_AMOUNT_T1 --> ' || P_AMOUNT_T1 || ', ' ||
    'P_AMOUNT_T2 --> ' || P_AMOUNT_T2 || ', ' ||
    'P_RECHARGE1 --> ' || P_RECHARGE1 || ', ' ||
    'P_RECHARGE2 --> ' || P_RECHARGE2 || ', ' ||
    'P_GST_TAX --> ' || P_GST_TAX || ', ' ||
    'P_COLLECTION_AMOUNT --> ' || P_COLLECTION_AMOUNT || ', ' ||
    'P_AMOUNT_INVESTED --> ' || P_AMOUNT_INVESTED || ', ' ||
    'P_AMOUNT_INVESTED2 --> ' || P_AMOUNT_INVESTED2 || ', ' ||
    'P_REMARK --> ' || P_REMARK || ', ' ||
    'P_ZERO_COM --> ' || P_ZERO_COM || ', ' ||
    'P_LOGGEDIN_USER --> ' || P_LOGGEDIN_USER 
AS STATUS FROM DUAL;

RETURN;
 
*/
         
BEGIN -- VALIDATE PUNCHING AND MIDIFICATION TEAM
     IF P_MARK = 0 THEN -- 0 FOR PUNCING , 4 FOR MODIFYING
     
        /*'38387' -- punch(121397/212), modify(39006/146)   */
        --WHERE LOGIN_ID = '121397' AND ROLE_ID  = 212 
        --WHERE LOGIN_ID = '39006' AND ROLE_ID  = 146 
         
        SELECT  nvl(ROLE_ID, null)
        INTO GLBROLEID 
        FROM USERDETAILS_JI
        WHERE LOGIN_ID = P_LOGGEDIN_USER AND ROLE_ID  = 212 and rownum = 1;
       
        IF GLBROLEID NOT IN (212,1) THEN
            OPEN P_RESULT FOR SELECT 'Only Punching Team can punch the transaction.' AS STATUS FROM DUAL; RETURN;  
        END IF;

    ELSIF P_MARK = 4 THEN
        SELECT nvl(ROLE_ID, null) 
        INTO GLBROLEID2 
        FROM USERDETAILS_JI
        WHERE LOGIN_ID = '39006' AND ROLE_ID  = 146  and rownum = 1;


        IF GLBROLEID2 NOT IN (146, 1) THEN
            OPEN P_RESULT FOR 
            SELECT 'Only NPS Team can modify the transaction.' || GLBROLEID2 AS STATUS FROM DUAL;
            RETURN; 
        END IF;
    END IF;
 
END;

BEGIN -- DT NUMBER IS REQUIRED AND RETURN    
    IF TRIM(P_DT_NUMBER) IS NULL  AND TRIM(P_MARK) = '0' THEN 
        OPEN P_RESULT FOR SELECT 'DT No cannot be left blank.xxx  (' || P_MARK || ') --- ' || P_DT_NUMBER AS STATUS FROM DUAL;
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
    IF P_MARK = 0 OR P_MARK = 4 THEN
        -- Fetch Client Category
        SELECT CATEGORY_ID 
        INTO V_CLIENT_CAT_DUP_CHEQUE
        FROM CLIENT_MASTER 
        WHERE CLIENT_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8);

        -- If client category is not "4004"
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
                    OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' FROM DUAL;
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
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' FROM DUAL;
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
                        OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' FROM DUAL;
                        RETURN;
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
        IF V_CLIENT_CAT_DUP_CHEQUE <> '4004' THEN
            -- Count matching transactions
            SELECT COUNT(*) INTO V_COUNT_TRAN_DUP_CHEQUE
            FROM TRANSACTION_ST
            WHERE CHEQUE_NO = TRIM(P_CHEQUE_NO)
              AND TRIM(BANK_NAME) = TRIM(P_BANK_NAME)
              AND TRAN_TYPE IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN');

            -- If duplicates exist, return error message
            IF V_COUNT_TRAN_DUP_CHEQUE > 0 THEN
                OPEN P_RESULT FOR SELECT 'Duplicate Cheque Number!' FROM DUAL;
                RETURN;
            END IF;
        END IF;
        
        
        IF TRIM(P_INVESTOR_CODE) IS NULL THEN
             OPEN P_RESULT FOR SELECT 'Please select a Investor!' FROM DUAL;
            RETURN;
        END IF;

        IF TRIM(P_APP_NO) IS NULL THEN
             OPEN P_RESULT FOR SELECT 'Please Select a Request Id!' FROM DUAL;
            RETURN;
        END IF;


        IF P_DATE IS NULL THEN
            OPEN P_RESULT FOR SELECT 'Please enter a correct Transaction Date!' FROM DUAL;
            RETURN;
        END IF;
        
        /*
        IF TO_DATE(P_DATE, 'DD/MM/YYYY') < TO_DATE('01/01/1980', 'DD/MM/YYYY') THEN 
            OPEN P_RESULT FOR SELECT 'Please enter a correct transaction date! ' || P_DATE || '--> ' || TO_DATE(P_DATE, 'DD/MM/YYYY')FROM dual;
            RETURN;
        END IF; */
        
        -- Validate if the transaction date is greater than the current date
        IF TO_DATE(P_DATE, 'DD/MM/YYYY') > SYSDATE THEN
            OPEN P_RESULT FOR SELECT 'Transaction Date Cannot Be Greater than Current Date' FROM DUAL;
            RETURN;
        END IF;

        -- Validate Payment Mode
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



        -- Validate Amount Input
        IF P_AMOUNT_INVESTED IS NULL OR TRIM(P_AMOUNT_INVESTED) = '' THEN
            OPEN P_RESULT FOR SELECT 'Please enter amount.' FROM DUAL;
            RETURN;
        END IF;

        IF NOT REGEXP_LIKE(P_AMOUNT_INVESTED, '^[0-9]+(\.[0-9]+)?$') THEN
            OPEN P_RESULT FOR SELECT 'Please enter a correct amount.' FROM DUAL;
            RETURN;
        END IF;

        -- Validate RM Business Code
        IF P_BUSINESS_RM IS NULL OR LENGTH(TRIM(P_BUSINESS_RM)) < 5 THEN
            OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' FROM DUAL;
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
                OPEN P_RESULT FOR SELECT 'Not a Valid RM Business Code' FROM DUAL;
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

            OPEN P_RESULT FOR SELECT 'Duplicate Transaction!' FROM DUAL;
            RETURN;
        END IF;
    END IF;
END;









IF P_MARK = '0' THEN
    -- Insert into transaction_sttemp
    INSERT INTO TRANSACTION_STTEMP (
        CORPORATE_NAME, MANUAL_ARNO, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, 
        TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, 
        RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, CHEQUE_NO, CHEQUE_DATE, REMARK, DOC_ID
    ) VALUES (
        P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, P_PAYMENT_MODE, P_INVESTOR_TYPE, 
        P_DATE, P_INVESTOR_CODE, MYMUTCODE, P_SCHEME_CODE, 'PURCHASE', P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, P_CRA_BRANCH, 
        P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, P_CHEQUE_NO, P_CHEQUE_DATE, P_REMARK, P_DT_NUMBER
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
        CHEQUE_NO, CHEQUE_DATE, REMARK, LOGGEDUSERID, INV_NAME
    ) VALUES ( 
        P_DT_NUMBER, V_GST_NO_INSERTION /*P_GST_TAX*/ , P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, 
        P_PAYMENT_MODE, V_TRAN_CODE_INSERTION, P_INVESTOR_TYPE, SYSDATE,P_INVESTOR_CODE /*NewClientCode*/, MYMUTCODE, P_SCHEME_CODE, 'PURCHASE', 
        P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, SUBSTR(P_INVESTOR_CODE, 1, 8), P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, 
        P_CHEQUE_NO, P_CHEQUE_DATE, 'NPS', P_LOGGEDIN_USER, P_SUBSCRIBER_NAME
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


    UPDATE TB_DOC_UPLOAD SET AR_CODE = V_TRAN_CODE_INSERTION WHERE COMMON_ID= P_DT_NUMBER;

    DELETE FROM TRANSACTION_STTEMP WHERE TRAN_CODE = V_TRAN_CODE_INSERTION;
    
    COMMIT;
    -- Return confirmation
    OPEN P_RESULT FOR
        SELECT 'Your Duplicate Transaction No Is ' ||(V_TRAN_CODE_INSERTION) || ' and Your Recpt No Is ' ||  (SELECT UNIQUE_ID FROM TRANSACTION_ST WHERE TRAN_CODE = V_TRAN_CODE_INSERTION) AS STATUS
          --SELECT 'Insertion successful and Tran Code Is ' || ts.TRAN_CODE AS STATUS FROM transaction_st ts WHERE ts.DOC_ID = P_DT_NUMBER;
        FROM DUAL 
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
        UNIQUE_ID = P_RECEIPT_NO, MODIFY_USER = P_LOGGEDIN_USER, MODIFY_DATE = P_DATE, REMARK = P_REMARK
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