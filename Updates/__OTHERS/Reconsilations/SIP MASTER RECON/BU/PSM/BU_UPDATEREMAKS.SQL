CREATE OR REPLACE PROCEDURE WEALTHMAKER.SP_UPDATE_REMARK_RECO (
    p_tr_code     IN VARCHAR2,
    p_remark      IN VARCHAR2
) AS
BEGIN
    -- Update for TRAN_CODE
    UPDATE Transaction_mf_temp1
    SET remark_reco = p_remark
    WHERE TRAN_CODE = p_tr_code;

    -- Update for BASE_TRAN_CODE
    UPDATE Transaction_mf_temp1
    SET remark_reco = p_remark
    WHERE BASE_TRAN_CODE = p_tr_code;

    -- Commit the changes
    COMMIT;
END SP_UPDATE_REMARK_RECO;
/
