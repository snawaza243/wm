create or replace PROCEDURE PSM_AM_GET_BY_DT_ID (
    p_common_id IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS


    F_VALID_DT   NUMBER := 0;  -- DB COLUMN: COMMON_ID
    F_DT_REJ     NUMBER := 0;  -- DB COLUMN: rejection_status (0/1)
    F_DT_PUN     NUMBER := 0;  -- DB COLUMN: punching_flag (0/1)
    F_DT_VERI    NUMBER := 0;  -- DB COLUMN: verification_flag (0/1)
    F_DT_TYPE    NUMBER := 0; -- DB COLUMN: tran_type (ANA)

BEGIN
        -- Check if COMMON_ID exists
    SELECT COUNT(*) 
    INTO F_VALID_DT
    FROM TB_DOC_UPLOAD 
    WHERE COMMON_ID = TRIM(p_common_id);

    IF F_VALID_DT = 0 THEN
        OPEN p_cursor FOR
        SELECT 'Invalid Data: DT is not valid' AS message FROM dual;
        RETURN;
    END IF;

    -- Check transaction type 'ANA'
    SELECT COUNT(*) 
    INTO F_DT_TYPE
    FROM TB_DOC_UPLOAD 
    WHERE COMMON_ID = TRIM(p_common_id) 
      AND UPPER(TRIM(tran_type)) = 'ANA';

    IF F_DT_TYPE = 0 THEN
        OPEN p_cursor FOR
        SELECT 'Invalid Data: Incorrect transaction type' AS message FROM dual;
        RETURN;
    END IF;


    -- Check rejection_status
    SELECT COUNT(*) 
    INTO F_DT_REJ
    FROM TB_DOC_UPLOAD 
    WHERE COMMON_ID = TRIM(p_common_id) 
      AND rejection_status = 1 AND UPPER(TRIM(tran_type)) = 'ANA' AND ROWNUM = 1;

    IF F_DT_REJ > 0 THEN
        OPEN p_cursor FOR
        SELECT 'Invalid Data: Document is rejected' AS message FROM dual;
        RETURN;
    END IF;

    -- Check verification_flag
    SELECT COUNT(*) 
    INTO F_DT_VERI
    FROM TB_DOC_UPLOAD 
    WHERE COMMON_ID = TRIM(p_common_id) 
      AND verification_flag = 1 AND UPPER(TRIM(tran_type)) = 'ANA' AND ROWNUM = 1;

    IF F_DT_VERI = 0 THEN
        OPEN p_cursor FOR
        SELECT 'Invalid Data: Verification not completed' AS message FROM dual;
        RETURN;
    END IF;

    -- Check punching_flag
    SELECT COUNT(*) 
    INTO F_DT_PUN
    FROM TB_DOC_UPLOAD 
    WHERE COMMON_ID = TRIM(p_common_id) 
      AND punching_flag = 1 AND UPPER(TRIM(tran_type)) = 'ANA' AND ROWNUM = 1;

    -- Check punching_flag
    --SELECT exist_code INTO F_DT_PUN FROM TB_DOC_UPLOAD  WHERE COMMON_ID = TRIM(p_common_id)  AND punching_flag = 1 AND UPPER(TRIM(tran_type)) = 'ANA';

    --IF F_DT_PUN > 0 THEN OPEN p_cursor FOR SELECT 'Invalid Data: Punching is completed' AS message FROM dual; RETURN; END IF;


    -- If all checks pass
    --IF F_VALID_DT > 0 AND F_DT_REJ = 0 AND F_DT_PUN = 0 AND F_DT_VERI > 0 AND F_DT_TYPE > 0 THEN OPEN p_cursor FOR SELECT 'Valid Data: All checks passed' AS message FROM dual; RETURN; END IF;


    OPEN p_cursor FOR
    SELECT
    'Data Found: DT is valid.' as message,
    --busi_rm_code as tb_rm_code, 
    (select rm_code from employee_master where payroll_id = busi_rm_code) tb_rm_code,
    busi_branch_code as tb_branch_code, 
    common_id as tb_common_id, 
    rejection_status as tb_reject, 
    punching_flag as tb_punch, 
    verification_flag as tb_verify , 
    tran_type as tb_tr_type, 
    exist_code as tb_exist_code, 
    nvl((select exist_code from agent_master where exist_code = tb_doc_upload.exist_code AND ROWNUM = 1), 0 ) as am_exist_code,
    nvl((select AGENT_CODE from agent_master where exist_code = tb_doc_upload.exist_code AND ROWNUM = 1 and AGENT_CODE is not null), 0 ) as am_AGENT_code
    FROM tb_doc_upload
    WHERE rejection_status = 0
    --AND punching_flag = 1 AND exist_code in (select exist_code from agent_master)

    AND verification_flag = 1
    AND UPPER(TRIM(tran_type)) = 'ANA'
    AND common_id = TRIM(p_common_id)
    and rownum = 1
    ;
END;