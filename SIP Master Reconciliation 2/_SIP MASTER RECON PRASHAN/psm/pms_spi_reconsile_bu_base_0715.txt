CREATE OR REPLACE PROCEDURE WEALTHMAKER.SP_RECONCILE_TRANSACTIONS_PRA(
    p_tran_code         IN VARCHAR2,
    p_rta_amount        IN NUMBER,
    p_rta_tran_code     IN VARCHAR2,
    p_login_user        IN VARCHAR2)  
AS
BEGIN
    -- Update the first table (Transaction_mf_temp1 based on TRAN_CODE)
    UPDATE Transaction_mf_temp1
    SET amount = p_rta_amount,
        REC_FLAG = 'Y',
        RECO_DATE = TO_DATE(SYSDATE),
        REC_USER = p_login_user,
        RTA_TRAN_CODE = p_rta_tran_code
    WHERE TRAN_CODE = p_tran_code;

    -- Update the second table (Transaction_mf_temp1 based on BASE_TRAN_CODE)
    UPDATE Transaction_mf_temp1
    SET amount = p_rta_amount,
        REC_FLAG = 'Y',
        RECO_DATE = TO_DATE(SYSDATE),
        REC_USER = p_login_user,
        RTA_TRAN_CODE = p_rta_tran_code
    WHERE BASE_TRAN_CODE = p_tran_code;

    -- Update the third table (Transaction_st@MF.BAJAJCAPITAL)
    UPDATE Transaction_st@MF.BAJAJCAPITAL
    SET REC_FLAG = 'Y',
        HO_TRAN_CODE = p_tran_code
    WHERE tran_code = p_rta_tran_code;

    COMMIT;
END;
/