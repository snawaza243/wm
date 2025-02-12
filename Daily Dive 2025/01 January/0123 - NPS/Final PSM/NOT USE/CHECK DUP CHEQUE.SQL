create or replace PROCEDURE PSM_NPS_CHECK_DUPLICATE_CHEQUE(
    P_INDEX IN NUMBER,
    P_INV_CD IN VARCHAR2,
    P_LBTRAN_CODE IN NUMBER,
    P_CHQ_NO IN VARCHAR2,
    P_REQ_CODE IN VARCHAR2
) AS
    V_CLIENT_CATEGORY VARCHAR2(10);
    V_DUPLICATE_COUNT NUMBER;
BEGIN
    -- Only proceed if Index is 0 or 4
    IF P_INDEX = 0 OR P_INDEX = 4 THEN
        -- Fetch the client category based on the client code
        SELECT CATEGORY_ID 
        INTO V_CLIENT_CATEGORY
        FROM CLIENT_MASTER 
        WHERE CLIENT_CODE = SUBSTR(P_INV_CD, 1, 8);

        -- If client category is not "4004"
        IF V_CLIENT_CATEGORY <> '4004' THEN
            -- Condition for lbtrancode = "0"
            IF P_LBTRAN_CODE = 0 THEN
                -- Check for duplicate cheque number within the last 6 months
                SELECT COUNT(*)
                INTO V_DUPLICATE_COUNT
                FROM (
                    SELECT TRAN_CODE 
                    FROM TRANSACTION_ST 
                    WHERE MUT_CODE = 'IS02520' 
                    AND CHEQUE_NO = TRIM(P_CHQ_NO)
                    AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                    UNION ALL
                    SELECT TRAN_CODE 
                    FROM TRANSACTION_STTEMP 
                    WHERE MUT_CODE = 'IS02520' 
                    AND CHEQUE_NO = TRIM(P_CHQ_NO)
                    AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                );

                -- If duplicate found, raise an error
                IF V_DUPLICATE_COUNT > 0 THEN
                    RAISE_APPLICATION_ERROR(-20001, 'Duplicate Cheque Number!');
                END IF;
            ELSE
                -- Condition when ReqCode is "11"
                IF P_REQ_CODE = '11' THEN
                    SELECT COUNT(*)
                    INTO V_DUPLICATE_COUNT
                    FROM (
                        SELECT TRAN_CODE 
                        FROM TRANSACTION_ST 
                        WHERE TRAN_CODE <> P_LBTRAN_CODE
                        AND MUT_CODE = 'IS02520'
                        AND CHEQUE_NO = TRIM(P_CHQ_NO)
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                        AND REF_TRAN_CODE IS NULL
                        UNION ALL
                        SELECT TRAN_CODE 
                        FROM TRANSACTION_STTEMP 
                        WHERE MUT_CODE = 'IS02520'
                        AND CHEQUE_NO = TRIM(P_CHQ_NO)
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                        AND TRAN_CODE <> P_LBTRAN_CODE
                    );

                    -- If duplicate found, raise an error
                    IF V_DUPLICATE_COUNT > 0 THEN
                        RAISE_APPLICATION_ERROR(-20001, 'Duplicate Cheque Number!');
                    END IF;
                ELSE
                    -- General condition for duplicate check
                    SELECT COUNT(*)
                    INTO V_DUPLICATE_COUNT
                    FROM (
                        SELECT TRAN_CODE 
                        FROM TRANSACTION_ST 
                        WHERE TRAN_CODE <> P_LBTRAN_CODE
                        AND MUT_CODE = 'IS02520'
                        AND CHEQUE_NO = TRIM(P_CHQ_NO)
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                        UNION ALL
                        SELECT TRAN_CODE 
                        FROM TRANSACTION_STTEMP 
                        WHERE MUT_CODE = 'IS02520'
                        AND CHEQUE_NO = TRIM(P_CHQ_NO)
                        AND TR_DATE >= ADD_MONTHS(SYSDATE, -6)
                        AND TRAN_CODE <> P_LBTRAN_CODE
                    );

                    -- If duplicate found, raise an error
                    IF V_DUPLICATE_COUNT > 0 THEN
                        RAISE_APPLICATION_ERROR(-20001, 'Duplicate Cheque Number!');
                    END IF;
                END IF;
            END IF;
        END IF;
    END IF;
END PSM_NPS_CHECK_DUPLICATE_CHEQUE;