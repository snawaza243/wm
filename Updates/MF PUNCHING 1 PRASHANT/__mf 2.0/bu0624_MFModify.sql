CREATE OR REPLACE PROCEDURE WEALTHMAKER.MODIFYTRANSACTIONPUNCHINGPRA (
    P_TRAN_CODE            IN TRANSACTION_MF_TEMP1.TRAN_CODE%TYPE,
    P_CLOSE_SCH_CODE       IN TRANSACTION_MF_TEMP1.TARGET_SWITCH_SCHEME%TYPE,
    P_AC_HOLDER_CODE       IN TRANSACTION_MF_TEMP1.AC_HOLDER_CODE%TYPE,
    P_EXP_RATE             IN TRANSACTION_MF_TEMP1.EXP_RATE%TYPE,
    P_EXP_AMOUNT           IN TRANSACTION_MF_TEMP1.EXP_AMOUNT%TYPE,
    P_CLIENT_CODE          IN TRANSACTION_MF_TEMP1.CLIENT_CODE%TYPE,
    P_INVESTOR_NAME        IN TRANSACTION_MF_TEMP1.INVESTOR_NAME%TYPE,
    P_BUSINESS_RM_CODE     IN TRANSACTION_MF_TEMP1.BUSINESS_RMCODE%TYPE,
    P_CLIENT_OWNER         IN TRANSACTION_MF_TEMP1.CLIENT_OWNER%TYPE,
    P_BUSI_BRANCH_CODE     IN TRANSACTION_MF_TEMP1.BUSI_BRANCH_CODE%TYPE,
    P_PANNO                IN TRANSACTION_MF_TEMP1.PANNO%TYPE,
    P_MUT_CODE             IN TRANSACTION_MF_TEMP1.MUT_CODE%TYPE,
    P_SCH_CODE             IN TRANSACTION_MF_TEMP1.SCH_CODE%TYPE,
    P_TR_DATE              IN TRANSACTION_MF_TEMP1.TR_DATE%TYPE,
    P_TRAN_TYPE            IN TRANSACTION_MF_TEMP1.TRAN_TYPE%TYPE,
    P_APP_NO               IN TRANSACTION_MF_TEMP1.APP_NO%TYPE,
    P_FOLIO_NO             IN TRANSACTION_MF_TEMP1.FOLIO_NO%TYPE,
    P_SIP_START_DATE       IN TRANSACTION_MF_TEMP1.SIP_START_DATE%TYPE,
    P_SIP_END_DATE         IN TRANSACTION_MF_TEMP1.SIP_END_DATE%TYPE,
    P_SWITCH_SCHEME        IN TRANSACTION_MF_TEMP1.SWITCH_SCHEME%TYPE,
    P_SWITCH_FOLIO         IN TRANSACTION_MF_TEMP1.SWITCH_FOLIO%TYPE,
    P_PAYMENT_MODE         IN TRANSACTION_MF_TEMP1.PAYMENT_MODE%TYPE,
    P_AMOUNT               IN TRANSACTION_MF_TEMP1.AMOUNT%TYPE,
    P_FREQUENCY            IN TRANSACTION_MF_TEMP1.FREQUENCY%TYPE,
    P_INSTALLMENTS_NO      IN TRANSACTION_MF_TEMP1.INSTALLMENTS_NO%TYPE,
    P_SIP_TYPE             IN TRANSACTION_MF_TEMP1.SIP_TYPE%TYPE,
    P_SIP_FR               IN TRANSACTION_MF_TEMP1.SIP_FR%TYPE,
    P_DISPATCH             IN TRANSACTION_MF_TEMP1.DISPATCH%TYPE,
    P_SOURCE_CODE          IN TRANSACTION_MF_TEMP1.SOURCE_CODE%TYPE,
    P_DOC_ID               IN TRANSACTION_MF_TEMP1.DOC_ID%TYPE,
    P_MICRO_INVESTMENT     IN TRANSACTION_MF_TEMP1.MICRO_INVESTMENT%TYPE,
    P_COB_FLAG             IN TRANSACTION_MF_TEMP1.COB_FLAG%TYPE,
    P_FREEDOM_SIP_FLAG     IN TRANSACTION_MF_TEMP1.FREEDOM_SIP_FLAG%TYPE,
    P_SWP_FLAG             IN TRANSACTION_MF_TEMP1.SWP_FLAG%TYPE,
    P_DROP_DATE            IN WM_TRAN_SIP.SIP_DEACTIVATION_DATE%TYPE,
    P_LOGGERUSERID         IN TRANSACTION_MF_TEMP1.MODIFY_USER%TYPE,
    P_BANK_NAME            IN TRANSACTION_MF_TEMP1.BANK_NAME%TYPE,
    P_TRAN_DATE            IN DATE,
    P_CHEQUE_NO            IN VARCHAR2,
    P_CHEQUE_DATE          IN DATE             
) AS
BEGIN
    -- Update the transaction_mf_temp1 table
    UPDATE TRANSACTION_MF_TEMP1
    SET 
        TARGET_SWITCH_SCHEME    = P_CLOSE_SCH_CODE,
        SWITCH_SCHEME           = P_SWITCH_SCHEME,
        SWITCH_FOLIO            = P_SWITCH_FOLIO,
        AC_HOLDER_CODE          = P_AC_HOLDER_CODE,
        INVESTOR_NAME           = P_INVESTOR_NAME,
        UPDATE_DATE             = SYSDATE,
        MODIFY_DATE             = TRUNC(SYSDATE),
        client_code             = P_CLIENT_CODE, 
        BUSINESS_RMCODE         = P_BUSINESS_RM_CODE,
        CLIENT_OWNER            = P_CLIENT_OWNER, 
        installments_no         = P_INSTALLMENTS_NO,
        FREQUENCY               = P_FREQUENCY,
        --BUSI_BRANCH_CODE        = P_BUSI_BRANCH_CODE,
        PANNO                   = P_PANNO,
        MUT_CODE                = P_MUT_CODE,
        SCH_CODE                = UPPER(P_SCH_CODE),
        TRAN_TYPE               = P_TRAN_TYPE,
        APP_NO                  = P_APP_NO,
        FOLIO_NO                = P_FOLIO_NO,
        SIP_START_DATE          = P_SIP_START_DATE,
        SIP_END_DATE            = P_SIP_END_DATE,
        PAYMENT_MODE            = P_PAYMENT_MODE,
        AMOUNT                  = P_AMOUNT,
        SIP_TYPE                = P_SIP_TYPE,
        SIP_FR                  = P_SIP_FR,
        DISPATCH                = P_DISPATCH,
        MICRO_INVESTMENT        = P_MICRO_INVESTMENT,
        COB_FLAG                = P_COB_FLAG,
        FREEDOM_SIP_FLAG        = P_FREEDOM_SIP_FLAG,
        SWP_FLAG                = P_SWP_FLAG,
        MODIFY_USER             = P_LOGGERUSERID,
        BANK_NAME               = P_BANK_NAME,
        TR_DATE                 = P_TRAN_DATE,
        CHEQUE_NO               = P_CHEQUE_NO,
        CHEQUE_DATE             = P_CHEQUE_DATE
    WHERE TRAN_CODE             = P_TRAN_CODE;

    -- Update WM_TRAN_SIP table
    UPDATE WM_TRAN_SIP
    SET 
        AMOUNT_SIP          = P_AMOUNT,
        TOTAL_SIP           = P_INSTALLMENTS_NO
    WHERE BASE_TRAN_CODE    = P_TRAN_CODE;


    UPDATE INVESTOR_MASTER
    SET 
        MODIFY_DATE     = SYSDATE,
        PAN             = P_PANNO,
        MODIFY_USER     = P_LOGGERUSERID
    WHERE INV_CODE      = (SELECT CLIENT_CODE FROM TRANSACTION_MF_TEMP1 WHERE TRAN_CODE = P_TRAN_CODE)
    AND EXISTS ( SELECT 1  FROM TRANSACTION_MF_TEMP1 WHERE TRAN_CODE = P_TRAN_CODE ) 
    AND P_AMOUNT > 3000;


    -- Call the procedure for SIP transaction
    IF P_TRAN_TYPE = 'PURCHASE' THEN
        EXECUTE IMMEDIATE 'CALL PRCINSERTWMTRANSIP(:1, :2)' USING P_TRAN_CODE, 2;
      --  IF p_Update_User = 29 THEN
      --      EXECUTE IMMEDIATE 'CALL PRCINSERTWMTRANSIP_HIS(:1, :2)' USING p_Tran_Code, p_Update_User;
      --  END IF;
    END IF;

    -- Update SIP deactivation date
    IF P_DROP_DATE IS NOT NULL THEN
        UPDATE WM_TRAN_SIP
        SET SIP_DEACTIVATION_DATE = P_DROP_DATE
        WHERE BASE_TRAN_CODE = P_TRAN_CODE;
    ELSE
        UPDATE WM_TRAN_SIP
        SET SIP_DEACTIVATION_DATE = NULL
        WHERE BASE_TRAN_CODE = P_TRAN_CODE;
    END IF;

    -- Commit the transaction
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20001, SQLERRM);
END MODIFYTRANSACTIONPUNCHINGPRA;
/
