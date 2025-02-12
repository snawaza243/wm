create or replace PROCEDURE PSM_NPS_GET_AR_BY_DTTS (
    P_DT_CODE IN VARCHAR2,
    P_BEFORE_ST IN VARCHAR2 DEFAULT NULL,
    P_RESULT_CURSOR OUT SYS_REFCURSOR
) AS
    dt_v_flag NUMBER;
    dt_p_flag NUMBER;
    dt_r_flag NUMBER;
BEGIN
    -- Query for verification_flag, punching_flag, and rejection_status counts
    SELECT COUNT(*)
    INTO dt_v_flag
    FROM tb_doc_upload
    WHERE common_id = P_DT_CODE
      AND verification_flag = '1';

    SELECT COUNT(*)
    INTO dt_p_flag
    FROM tb_doc_upload
    WHERE common_id = P_DT_CODE
      AND punching_flag = '1';

    SELECT COUNT(*)
    INTO dt_r_flag
    FROM tb_doc_upload
    WHERE common_id = P_DT_CODE
      AND rejection_status = '1';

    -- Check the flags and return appropriate messages
    IF dt_v_flag = 0 THEN
        OPEN P_RESULT_CURSOR FOR
        SELECT 'Incorrect Data by dt: DT is not verified' AS message
        FROM dual;
        RETURN;
    ELSIF dt_p_flag > 0 THEN
        OPEN P_RESULT_CURSOR FOR
        SELECT 'Incorrect Data by dt: DT punched already' AS message
        FROM dual;
        RETURN;
    ELSIF dt_r_flag > 0 THEN
        OPEN P_RESULT_CURSOR FOR
        SELECT 'Incorrect Data by dt: DT is rejected' AS message
        FROM dual;
        RETURN;
    ELSE
        OPEN P_RESULT_CURSOR FOR
        SELECT
            'Valid Data by dt' AS message,
            TS.*,
            TS.INV_NAME AS TS_INV_NAME,
            NT.AMOUNT1 AS NT_AMOUNT1,
            NT.AMOUNT2 AS NT_AMOUNT2,
            NT.REG_CHARGE AS NT_REG_CHARGE,
            NT.TRAN_CHARGE AS NT_TRAN_CHARGE,
            NT.SERVICETAX AS NT_SERVICETAX,
            NT.REMARK AS NT_REMARK,
            NVL((SELECT INVESTOR_NAME
                 FROM INVESTOR_MASTER IM
                 WHERE IM.INV_CODE = TU.INV_CODE), 'N/A') AS DC_INV_NAME
        FROM
            TRANSACTION_ST TS
            JOIN NPS_TRANSACTION NT ON TS.TRAN_CODE = NT.TRAN_CODE
            JOIN TB_DOC_UPLOAD TU ON TU.COMMON_ID = TS.DOC_ID
        WHERE
            TS.DOC_ID = P_DT_CODE;
    END IF;

END PSM_NPS_GET_AR_BY_DTTS;