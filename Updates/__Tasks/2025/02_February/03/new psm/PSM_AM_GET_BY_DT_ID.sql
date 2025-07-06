create or replace PROCEDURE PSM_AM_GET_BY_DT_ID (
    p_common_id IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN

    OPEN p_cursor FOR
    SELECT
    'DT Found for Assoicate Master.' as message,
    busi_rm_code as tb_rm_code, 
    busi_branch_code as tb_branch_code, 
    common_id as tb_common_id, 
    rejection_status as tb_reject, 
    punching_flag as tb_punch, 
    verification_flag as tb_verify , 
    tran_type as tb_tr_type, 
    exist_code as tb_exist_code, 
    nvl((select exist_code from agent_master where exist_code = tb_doc_upload.exist_code AND ROWNUM = 1), 0 ) as am_exist_code,
    nvl((select AGENT_CODE from agent_master where exist_code = tb_doc_upload.exist_code AND ROWNUM = 1), 0 ) as am_AGENT_code


    FROM tb_doc_upload
    WHERE tran_type  = 'ANA' 
    AND common_id = p_common_id
    and rownum = 1
    --and (rejection_status is null or rejection_status = 0)
    ;
END;