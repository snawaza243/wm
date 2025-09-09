CREATE OR REPLACE PROCEDURE WEALTHMAKER.psm_mf2_v_cross_channel_temp (
    p_common_id     IN  VARCHAR2,
    p_label32       IN  VARCHAR2,
    p_login_id      IN  VARCHAR2,
    p_cursor        OUT SYS_REFCURSOR
)
AS
    v_error             VARCHAR2(4000);
    v_busi_rm_code      VARCHAR2(15);
    v_busi_branch_code  VARCHAR2(15);
    v_busi_tr_date      DATE;
    v_sch_code          VARCHAR2(15);
    v_inv_code          VARCHAR2(15);
    v_branch_name       VARCHAR2(100);
    v_expense           NUMBER;
    v_approval_status   VARCHAR2(5);
    v_cnt               NUMBER;
BEGIN
    -- Fetch document details
    SELECT 
    --a.common_id, a.tran_type, a.verification_flag, a.rejection_status, a.punching_flag,
    a.busi_rm_code, a.busi_branch_code, a.busi_tr_date, a.sch_code, a.inv_code,b.branch_name, a.expense
     INTO v_busi_rm_code, v_busi_branch_code, v_busi_tr_date, v_sch_code, v_inv_code, v_branch_name, v_expense
      FROM tb_doc_upload a
      JOIN branch_master b ON a.busi_branch_code = b.branch_code
     WHERE a.common_id = p_common_id
       AND a.tran_type = 'MF'
       AND a.verification_flag = '1'
       AND a.rejection_status = '0'
       AND a.punching_flag = '0';

    -- Check if record exists
    IF SQL%ROWCOUNT > 0 THEN
    
        
        -- Check approval status
        v_approval_status := wealthmaker.fn_check_for_approval_all(v_inv_code);

        --OPEN p_cursor FOR SELECT 'FOUND COUNT IN CROSS CHECK. WITH AP STD: ' || v_approval_status AS message FROM dual; RETURN;
        
        IF v_approval_status = '2' THEN
            OPEN p_cursor FOR SELECT 'Approval request for this transaction has already been raised.' AS message FROM dual;
            RETURN;
        ELSIF v_approval_status = '4' THEN
            OPEN p_cursor FOR SELECT 'Approval request for this transaction has been rejected by Management.' AS message FROM dual;
            RETURN;
        END IF;

        -- Check if label32 is missing
        IF p_label32 IS NULL THEN
            OPEN p_cursor FOR SELECT 'Information not present' AS message FROM dual;
            RETURN;
        END IF;

        -- Call cross-channel validation procedure
        WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO(
            PCOMMON_ID    => p_common_id,
            PSUB_CLIENT_CD => p_label32,
            PLOGIN_ID     => p_login_id,
            PCNT          => v_cnt
        );

        -- Handle validation result
        IF v_cnt > 0 THEN
            OPEN p_cursor FOR SELECT 'SHOW_CROSS_CHANNEL_INFO' AS message FROM dual;
            RETURN;
        ELSE
            IF p_label32 IS NOT NULL THEN
                OPEN p_cursor FOR SELECT 'SHOW_ADDRESS_UPDATE' AS message FROM dual;
                RETURN;
            ELSE
                OPEN p_cursor FOR SELECT 'Information not present' AS message FROM dual;
                RETURN;
            END IF;
        END IF;

    ELSE
        -- No document found
        OPEN p_cursor FOR SELECT 'Information not present' AS message FROM dual;
        RETURN;
    END IF;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        OPEN p_cursor FOR SELECT 'Information not present' AS message FROM dual;
        return;
    WHEN OTHERS THEN
        DECLARE
            v_err_msg VARCHAR2(4000);
        BEGIN
            v_err_msg := 'Error occurred: ' || SQLERRM;
            OPEN p_cursor FOR SELECT v_err_msg AS message FROM dual;
        END;
            
END;
/