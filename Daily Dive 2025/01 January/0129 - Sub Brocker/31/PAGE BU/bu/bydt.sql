create or replace PROCEDURE PSM_ASS_M_GetDocumentCodes (
    p_common_id IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_cursor FOR
    SELECT busi_rm_code, busi_branch_code, common_id
    FROM tb_doc_upload
    WHERE tran_type = 'ANA' 
    AND common_id = p_common_id
    and (rejection_status is null or rejection_status = 0);
END;