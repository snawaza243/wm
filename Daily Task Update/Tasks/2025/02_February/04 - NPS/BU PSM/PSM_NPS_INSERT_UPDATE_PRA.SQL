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
    MyTranCode VARCHAR2(100);
    MyGSTNO VARCHAR2(100);
    NewClientCode NUMBER;
    MyMutCode VARCHAR2(100);
    currentRole NUMBER;
    temprole  number;

    v_flag_nps_tran number;
    v_generated_tr  NUMBER;
BEGIN
    temprole :=212;

    BEGIN
        SELECT role_id 
        INTO currentRole 
        FROM USERDETAILS_Ji where LOGIN_ID = P_LOGGEDIN_USER AND ROLE_ID  = 212 AND ROWNUM = 1;
        EXCEPTION 
            WHEN NO_DATA_FOUND THEN
            currentRole := 0; 
    END;

    -- Fetch ISS_CODE for the provided P_SCHEME_CODE
    BEGIN
        SELECT ISS_CODE
        INTO MyMutCode
        FROM other_product
        WHERE OSCH_CODE = P_SCHEME_CODE;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            MyMutCode := NULL;
    END;

    -- Fetch CLIENT_CODE and TRAN_CODE
    BEGIN
        SELECT CLIENT_CODE, TRAN_CODE
        INTO NewClientCode, MyTranCode
        FROM transaction_st
        WHERE DOC_ID = P_DT_NUMBER;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
        NewClientCode := 0;
        MyTranCode := NULL;
    END;

    -- currentRole
    IF currentRole = 212 THEN


        IF P_MARK IS NULL THEN
            -- Insert into transaction_sttemp
            INSERT INTO transaction_sttemp (
                CORPORATE_NAME, manual_arno, BANK_NAME, folio_no, APP_NO, PAYMENT_MODE, INVESTOR_TYPE, 
                TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, BRANCH_CODE, SOURCE_CODE, 
                RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, CHEQUE_DATE, remark, doc_id
            ) VALUES (
                P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, P_PAYMENT_MODE, 
                P_INVESTOR_TYPE, P_DATE, P_INVESTOR_CODE, MyMutCode, P_SCHEME_CODE, 'PURCHASE', 
                P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, P_CRA_BRANCH, P_BUSINESS_RM, P_BUSINESS_RM, 
                P_BUSINESS_BRANCH, P_CHEQUE_NO, P_CHEQUE_DATE, P_REMARK, P_DT_NUMBER
            );



            -- Insert into transaction_st
            INSERT INTO transaction_st (
                invoice_no, CORPORATE_NAME, manual_arno, BANK_NAME, FOLIO_NO, APP_NO, PAYMENT_MODE, 
                TRAN_CODE, INVESTOR_TYPE, TR_DATE, CLIENT_CODE, MUT_CODE, SCH_CODE, TRAN_TYPE, AMOUNT, 
                BRANCH_CODE, SOURCE_CODE, RMCODE, BUSINESS_RMCODE, BUSI_BRANCH_CODE, cheque_no, 
                CHEQUE_DATE, remark, LOGGEDUSERID, doc_id
            ) VALUES (
                P_GST_TAX, P_CORPORATE_NAME, P_MANUAL_AR_NO, P_BANK_NAME, P_FOLIO_NUMBER, P_APP_NO, 
                P_PAYMENT_MODE, MyTranCode, P_INVESTOR_TYPE, SYSDATE, NewClientCode, MyMutCode, 
                P_SCHEME_CODE, 'PURCHASE', P_AMOUNT_INVESTED, P_BUSINESS_BRANCH, 
                SUBSTR(NewClientCode, 1, 8), P_BUSINESS_RM, P_BUSINESS_RM, P_BUSINESS_BRANCH, 
                P_CHEQUE_NO, P_CHEQUE_DATE, 'NPS', P_LOGGEDIN_USER, P_DT_NUMBER
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
            -- Update transaction_st
            UPDATE transaction_st
            SET TR_DATE = P_DATE, CLIENT_CODE = P_INVESTOR_CODE, SOURCE_CODE = SUBSTR(P_INVESTOR_CODE, 1, 8),
                BUSI_BRANCH_CODE = P_BUSINESS_BRANCH, BUSINESS_RMCODE = P_BUSINESS_RM, MUT_CODE = MyMutCode, 
                SCH_CODE = P_SCHEME_CODE, AMOUNT = P_AMOUNT_INVESTED, FOLIO_NO = P_FOLIO_NUMBER, APP_NO = P_APP_NO, 
                PAYMENT_MODE = P_PAYMENT_MODE, CHEQUE_DATE = P_CHEQUE_DATE, CHEQUE_NO = P_CHEQUE_NO, 
                BANK_NAME = P_BANK_NAME, MANUAL_ARNO = P_MANUAL_AR_NO, CORPORATE_NAME = P_CORPORATE_NAME, 
                UNIQUE_ID = P_RECEIPT_NO, MODIFY_USER = P_LOGGEDIN_USER, MODIFY_DATE = P_DATE
            WHERE TRAN_CODE = P_TRAN_CODE;

            -- Update nps_transaction



            BEGIN 
                SELECT TRAN_CODE 
                INTO v_flag_nps_tran FROM nps_transaction WHERE TRAN_CODE = P_TRAN_CODE; 
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






            -- Return confirmation
            OPEN P_RESULT FOR
                SELECT 'Updation successful ' AS STATUS
                FROM DUAL;
        END IF;
    ELSE
        OPEN P_RESULT FOR
            SELECT 'Only punching team can punch!' AS STATUS
            FROM DUAL;
    END IF;

END PSM_NPS_INSERT_UPDATE_PRA;