CREATE OR REPLACE PROCEDURE WEALTHMAKER.PMS_MF_RECO_M_SETREMARK (
    p_tr_code     IN VARCHAR2,
    p_remark      IN VARCHAR2,
    P_CURSOR      OUT SYS_REFCURSOR
) AS
BEGIN
    IF p_tr_code IS NULL OR p_remark IS NULL  THEN
        OPEN P_CURSOR FOR SELECT 'Please double click the record,you want to remark and renter remark' AS MESSAGE FROM DUAL;
        RETURN;
    END IF;

    SAVEPOINT START_TRANSACTION;
    
    UPDATE Transaction_mf_temp1
    SET remark_reco = p_remark
    WHERE TRAN_CODE = p_tr_code;

    UPDATE Transaction_mf_temp1
    SET remark_reco = p_remark
    WHERE BASE_TRAN_CODE = p_tr_code;

    OPEN P_CURSOR FOR
        SELECT 'SUCCESS: The AR has been remarked sucessfully' AS MESSAGE
        FROM DUAL; 
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO START_TRANSACTION;

        V_ERROR_MSG := SQLERRM;
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || V_ERROR_MSG AS MESSAGE FROM DUAL;
END SP_UPDATE_REMARK_RECO;
/
