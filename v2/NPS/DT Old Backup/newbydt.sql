CREATE OR REPLACE PROCEDURE PSM_NPS_GET_AR_BY_DTTS (
    p_dtnumber IN tb_doc_upload.common_id%TYPE,
    result_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN result_cursor FOR
        SELECT NVL(ar_code, '0') AS ar_code,
               NVL(INV_CODE, 0) AS INV_CODE,
               NVL(BUSI_BRANCH_CODE, 0) AS BUSI_BRANCH_CODE,
               NVL(BUSI_RM_CODE, 0) AS BUSI_RM_CODE,
               common_id
        FROM tb_doc_upload
        WHERE tran_type = 'FI'
          AND common_id = p_dtnumber;
END;
/
