CREATE OR REPLACE PROCEDURE PRC_CMD_SHOW (
    p_common_id   IN  VARCHAR2,
    p_login_id    IN  VARCHAR2,
    p_role_id     IN  NUMBER,
    p_result      OUT SYS_REFCURSOR
) AS
    vApprovalFlag NUMBER;
    vResult       NUMBER;
    vInvCode      VARCHAR2(50);
    vSchCode      VARCHAR2(50);
    vExpense      NUMBER;
    vBranchName   VARCHAR2(200);
    vMessage      VARCHAR2(4000);

    vExpPercStr   VARCHAR2(100);
    vExpRsStr     VARCHAR2(100);

BEGIN
    -- Step 1: Get document details
    SELECT a.busi_rm_code,
           a.busi_branch_code,
           a.busi_tr_date,
           a.sch_code,
           a.inv_code,
           b.branch_name,
           a.expense
      INTO vInvCode, vInvCode, vInvCode, vSchCode, vInvCode, vBranchName, vExpense
      FROM tb_doc_upload a,
           branch_master b
     WHERE a.busi_branch_code = b.branch_code
       AND a.common_id        = TRIM(p_common_id)
       AND tran_type          = 'MF'
       AND verification_flag  = '1'
       AND rejection_status   = '0'
       AND punching_flag      = '0'
       FETCH FIRST 1 ROWS ONLY;

    -- If no data found
    IF vSchCode IS NULL THEN
        OPEN p_result FOR
            SELECT 'Information not present' AS MESSAGE FROM dual;
        RETURN;
    END IF;

    -- Step 2: Check approval status
    SELECT wealthmaker.fn_check_for_approval_all(p_common_id)
      INTO vApprovalFlag
      FROM dual;

    IF vApprovalFlag = 2 THEN
        OPEN p_result FOR
            SELECT 'Approval request for this transaction has already been raised.' AS MESSAGE FROM dual;
        RETURN;
    ELSIF vApprovalFlag = 4 THEN
        OPEN p_result FOR
            SELECT 'Approval request for this transaction has been rejected by Management.' AS MESSAGE FROM dual;
        RETURN;
    END IF;

    -- Step 3: Call cross channel validation proc
    DECLARE
        vCnt NUMBER;
    BEGIN
        WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO(
            PCOMMON_ID   => p_common_id,
            PSUB_CLIENT_CD => vInvCode,
            PLOGIN_ID    => p_login_id,
            PCNT         => vCnt
        );

        IF vCnt > 0 THEN
            OPEN p_result FOR
                SELECT 'open_cross_channel_model' AS MESSAGE FROM dual;
            RETURN;
        END IF;
    END;

    DECLARE 
        vSchNature VARCHAR2(50);
        

    BEGIN
        SELECT nvl(get_scheme_nature(vSchCode),'') INTO vSchNature FROM dual;
    END;

    vExpPercStr:= 'Expenses agnst. Trail%';
    vExpRsStr  := 'Expenses agnst. Trail Rs.';

    IF vSchNature = 'O' THEN
        vExpPercStr:= 'Expenses%';
        vExpRsStr  := 'Expenses(Rs.)';
    END IF;

    -- Step 4: Build return cursor for UI values
    OPEN p_result FOR
        SELECT vSchCode       AS SCH_CODE,
               vInvCode       AS INV_CODE,
               vBranchName    AS BRANCH_NAME,
               vExpense       AS EXPENSE,
               p_role_id      AS ROLE_ID,
               'SUCCESS:'         AS MESSAGE
          FROM dual;

EXCEPTION
    WHEN NO_DATA_FOUND THEN
        OPEN p_result FOR
            SELECT 'Information not present' AS MESSAGE FROM dual;
    WHEN OTHERS THEN
        OPEN p_result FOR
            SELECT 'Error: ' || SQLERRM AS MESSAGE FROM dual;
END PRC_CMD_SHOW;
/
