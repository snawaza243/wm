CREATE OR REPLACE PROCEDURE WEALTHMAKER.PRA_PMS_TransactionUnReco(
    p_tran_code IN VARCHAR2,
    p_remarks IN VARCHAR2,
    p_user IN VARCHAR2,
    p_opt_pms IN BOOLEAN,
    p_opt_atm IN BOOLEAN
)
AS
BEGIN
    IF p_opt_pms THEN
        UPDATE Transaction_mf_temp1
        SET remark_reco = p_remarks,
            REC_FLAG = 'N',
            RECO_DATE = NULL,
            REC_USER = NULL
        WHERE TRAN_CODE = p_tran_code;

        UPDATE Transaction_mf_temp1
        SET remark_reco = p_remarks,
            REC_FLAG = 'N',
            RECO_DATE = NULL,
            REC_USER = NULL
        WHERE BASE_TRAN_CODE = p_tran_code;
    ELSIF p_opt_atm THEN
        UPDATE Transaction_mf_temp1
        SET ATM_RECO_FLAG = 'N'
        WHERE TRAN_CODE = p_tran_code;
    END IF;

    COMMIT;
END;
/