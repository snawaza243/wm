CREATE OR REPLACE PROCEDURE PSM_NPS_GET_AR_BY_DTTS (
    p_dtnumber IN tb_doc_upload.common_id%TYPE,
    result_cursor OUT SYS_REFCURSOR
) AS
    dt_flag NUMBER;
    at_flag VARCHAR2(50);
    inv_flag NUMBER;
    busi_branch_flag NUMBER;
    busi_rm_flag NUMBER;

BEGIN
    -- Retrieve data into variables
    BEGIN
        SELECT NVL(ar_code, '0'),
               NVL(INV_CODE, 0),
               NVL(BUSI_BRANCH_CODE, 0),
               NVL(BUSI_RM_CODE, 0),
               NVL(common_id, 0)
        INTO at_flag,
             inv_flag,
             busi_branch_flag,
             busi_rm_flag,
             dt_flag
        FROM tb_doc_upload
        WHERE tran_type = 'FI' AND common_id = p_dtnumber;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            dt_flag := 0; -- Handle no data case
    END;

    -- Check if data exists or is valid
    IF dt_flag IS NULL OR dt_flag = 0 THEN
        -- Return an error message
        OPEN result_cursor FOR
            SELECT 'Incorrect' AS message
            FROM DUAL;
        RETURN;
    ELSE
        -- Check if any matching rows exist (optional count validation)
            -- Return the actual data
            OPEN result_cursor FOR
                SELECT 
                      'Validity: DT is valid' AS message,
                       NVL(ar_code, '0') AS ar_code,
                       NVL(INV_CODE, 0) AS INV_CODE,
                       NVL(BUSI_BRANCH_CODE, 0) AS BUSI_BRANCH_CODE,
                       NVL(BUSI_RM_CODE, 0) AS BUSI_RM_CODE,
                       NVL(common_id, 0) AS common_id
                FROM tb_doc_upload
                WHERE tran_type = 'FI' AND common_id = p_dtnumber;
        END IF;

END;
/
