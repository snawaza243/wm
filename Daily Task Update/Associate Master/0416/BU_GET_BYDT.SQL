create or replace PROCEDURE PSM_AM_GET_BY_DT_ID (
    P_COMMON_ID IN VARCHAR2,
    P_CURSOR OUT SYS_REFCURSOR
) AS


    F_VALID_DT   NUMBER := 0;  -- DB COLUMN: COMMON_ID
    F_DT_REJ     NUMBER := 0;  -- DB COLUMN: rejection_status (0/1)
    F_DT_PUN     NUMBER := 0;  -- DB COLUMN: punching_flag (0/1)
    F_DT_VERI    NUMBER := 0;  -- DB COLUMN: verification_flag (0/1)
    F_DT_TYPE    NUMBER := 0; -- DB COLUMN: tran_type (ANA)

BEGIN

     BEGIN   -- Check if COMMON_ID exists
        SELECT COUNT(*) 
        INTO F_VALID_DT
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_COMMON_ID);

        IF F_VALID_DT = 0 THEN
            OPEN P_CURSOR FOR
            SELECT 'Invalid Data: DT is not valid' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END;

    BEGIN -- Check transaction type 'ANA'
        SELECT COUNT(*) 
        INTO F_DT_TYPE
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_COMMON_ID) 
          AND UPPER(TRIM(TRAN_TYPE)) = 'ANA';

        IF F_DT_TYPE = 0 THEN
            OPEN P_CURSOR FOR
            SELECT 'Invalid Data: Incorrect transaction type' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END;

    BEGIN -- Check rejection_status
        SELECT COUNT(*) 
        INTO F_DT_REJ
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_COMMON_ID) 
          AND REJECTION_STATUS = 1 AND UPPER(TRIM(TRAN_TYPE)) = 'ANA' AND ROWNUM = 1;

        IF F_DT_REJ > 0 THEN
            OPEN P_CURSOR FOR
            SELECT 'Invalid Data: Document is rejected' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END;

    BEGIN -- Check verification_flag
        SELECT COUNT(*) 
        INTO F_DT_VERI
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_COMMON_ID) 
        AND VERIFICATION_FLAG = 1 AND UPPER(TRIM(TRAN_TYPE)) = 'ANA' AND ROWNUM = 1;

        IF F_DT_VERI = 0 THEN
            OPEN P_CURSOR FOR
            SELECT 'Invalid Data: Verification not completed' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END;

    BEGIN -- Check punching_flag
        SELECT COUNT(*) 
        INTO F_DT_PUN
        FROM TB_DOC_UPLOAD 
        WHERE COMMON_ID = TRIM(P_COMMON_ID) 
          AND PUNCHING_FLAG = 1 AND UPPER(TRIM(TRAN_TYPE)) = 'ANA' AND ROWNUM = 1;

        IF F_DT_PUN >0 THEN
            OPEN P_CURSOR FOR
            SELECT 'DT already punched!' AS MESSAGE FROM DUAL;
            RETURN;
        END IF;
    END;



/* ENHANCED FEATURES
    -- Check punching_flag
    SELECT exist_code INTO F_DT_PUN FROM TB_DOC_UPLOAD  WHERE COMMON_ID = TRIM(p_common_id)  AND punching_flag = 1 AND UPPER(TRIM(tran_type)) = 'ANA';
    IF F_DT_PUN > 0 THEN OPEN p_cursor FOR SELECT 'Invalid Data: Punching is completed' AS message FROM dual; RETURN; END IF;

    -- If all checks pass
    IF F_VALID_DT > 0 AND F_DT_REJ = 0 AND F_DT_PUN = 0 AND F_DT_VERI > 0 AND F_DT_TYPE > 0 THEN OPEN p_cursor FOR SELECT 'Valid Data: All checks passed' AS message FROM dual; RETURN; END IF;
*/

    OPEN P_CURSOR FOR
        SELECT
        'Data Found: DT is valid.' AS MESSAGE,
        --busi_rm_code as tb_rm_code, 
        (SELECT RM_CODE FROM EMPLOYEE_MASTER WHERE PAYROLL_ID = BUSI_RM_CODE) TB_RM_CODE,
        BUSI_BRANCH_CODE AS TB_BRANCH_CODE, 
        COMMON_ID AS TB_COMMON_ID, 
        REJECTION_STATUS AS TB_REJECT, 
        PUNCHING_FLAG AS TB_PUNCH, 
        VERIFICATION_FLAG AS TB_VERIFY , 
        TRAN_TYPE AS TB_TR_TYPE, 
        EXIST_CODE AS TB_EXIST_CODE, 
        NVL((SELECT EXIST_CODE FROM AGENT_MASTER WHERE EXIST_CODE = TB_DOC_UPLOAD.EXIST_CODE AND ROWNUM = 1), 0 ) AS AM_EXIST_CODE,
        NVL((SELECT AGENT_CODE FROM AGENT_MASTER WHERE EXIST_CODE = TB_DOC_UPLOAD.EXIST_CODE AND ROWNUM = 1 AND AGENT_CODE IS NOT NULL), 0 ) AS AM_AGENT_CODE
        FROM TB_DOC_UPLOAD
        WHERE REJECTION_STATUS = 0
        --AND punching_flag = 1 AND exist_code in (select exist_code from agent_master)

        AND VERIFICATION_FLAG = 1
        AND UPPER(TRIM(TRAN_TYPE)) = 'ANA'
        AND COMMON_ID = TRIM(P_COMMON_ID)
        AND ROWNUM = 1
    ;
END;