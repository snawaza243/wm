CREATE OR REPLACE PROCEDURE PSM_MF2_ADD_PROCESS (
    PX_LOG_ID                 IN VARCHAR2,
    PX_ROLE_ID                IN VARCHAR2,
    PX_DT_NUMBER              IN VARCHAR2,
    PX_TR_DATE                IN VARCHAR2,
    PX_BSS_CODE               IN VARCHAR2,
    PX_RM                     IN VARCHAR2,
    PX_INV_CODE               IN VARCHAR2,
    PX_CLIENT_CODE            IN VARCHAR2,
    PX_ANA_CODE               IN VARCHAR2,
    PX_AH_NAME                IN VARCHAR2,
    PX_AH_CODE                IN VARCHAR2,
    PX_BRANCH                 IN VARCHAR2,
    PX_AMC                    IN VARCHAR2,
    PX_LIST_LONG              IN VARCHAR2,
    PX_TRANSACTION_TYPE       IN VARCHAR2,
    PX_REGULAR_NFO            IN VARCHAR2,
    PX_SCHEME1                IN VARCHAR2,
    PX_HDN_SCHEME1            IN VARCHAR2,
    PX_SEARCH_STATE           IN VARCHAR2,
    PX_SEARCH_FORM            IN VARCHAR2,
    PX_FROM_SWITCH_FOLIO      IN VARCHAR2,
    PX_SCHEME2_SWITCH         IN VARCHAR2,
    PX_HDN_SCHEME2_SWITCH     IN VARCHAR2,
    PX_APP_NO                 IN VARCHAR2,
    PX_FOLIO_NO               IN VARCHAR2,
    PX_AMOUNT                 IN VARCHAR2,
    PX_PAYMENT_MODE           IN VARCHAR2,
    PX_CHEQUE_NO              IN VARCHAR2,
    PX_CHEQUE_DATE            IN VARCHAR2,
    PX_BANK_NAME              IN VARCHAR2,
    PX_EXP_PER                IN VARCHAR2,
    PX_EXP_RS                 IN VARCHAR2,
    PX_AUTO_SWITCH_MATURITY   IN CHAR,
    PX_SCHEME3_CLOSE          IN VARCHAR2,
    PX_HDN_SCHEME3_CLOSE      IN VARCHAR2,
    PX_SIP_STP                IN VARCHAR2,
    PX_INSTALLMENT_TYPE       IN VARCHAR2,
    PX_SIP_TYPE               IN VARCHAR2,
    PX_SIP_AMOUNT             IN VARCHAR2,
    PX_FREQUENCY              IN VARCHAR2,
    PX_INSTALLMENTS_NO        IN VARCHAR2,
    PX_SIP_START_DATE         IN VARCHAR2,
    PX_SIP_END_DATE           IN VARCHAR2,
    PX_FRESH_RENEWAL          IN VARCHAR2,
    PX_COB_CASE               IN CHAR,
    PX_SWP_CASE               IN CHAR,
    PX_FREEDOM_SIP            IN CHAR,
    PX_99_YEARS               IN CHAR,
    PX_PAN2                   IN VARCHAR2,
    PX_HDN_PAN1               IN VARCHAR2,
    PX_CURSOR                 OUT SYS_REFCURSOR
)
AS
BEGIN
    -- Example: Insert logic or processing
    -- You should replace this with actual INSERT/UPDATE logic into your database table(s)

    /*
    INSERT INTO MF2_TRANSACTIONS (
        DT_NUMBER, TR_DATE, BSS_CODE, ...
    ) VALUES (
        PX_DT_NUMBER, TO_DATE(PX_TR_DATE, 'DD-MM-YYYY'), PX_BSS_CODE, ...
    );
    */

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
