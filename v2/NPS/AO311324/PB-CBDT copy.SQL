CREATE OR REPLACE PROCEDURE PSM_AO_GET_CLIENTBYDT (
    P_CLIENT_CODE IN VARCHAR2,  -- Common ID
    P_RESULT OUT SYS_REFCURSOR
) AS
    ROW_COUNT    INT; 
    COMMON_FLAG  INT; 
    INV_FLAG     INT;
    CT_COUNT     INT;
    CM_FLAG      INT;  -- Corrected to consistent variable naming (from 'CM_flag' to 'CM_FLAG')
BEGIN
 -- Calculate the COMMON_FLAG
    SELECT NVL(COUNT(COMMON_ID), 0) INTO COMMON_FLAG 
    FROM TB_DOC_UPLOAD 
    WHERE VERIFICATION_FLAG = '1' 
    AND REJECTION_STATUS = '0' 
    AND TRAN_TYPE = 'AC' 
    AND PUNCHING_FLAG = '0' 
    AND COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Calculate INV_FLAG
    SELECT NVL(COUNT(*), 0) INTO INV_FLAG 
    FROM TB_DOC_UPLOAD DU
    JOIN INVESTOR_MASTER IM ON DU.INV_CODE = IM.INV_CODE
    WHERE DU.VERIFICATION_FLAG = '1' 
    AND DU.REJECTION_STATUS = '0' 
    AND DU.TRAN_TYPE = 'AC' 
    AND DU.PUNCHING_FLAG = '0' 
    AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Calculate CT_COUNT
    SELECT NVL(COUNT(*), 0) INTO CT_COUNT 
    FROM TB_DOC_UPLOAD DU
    JOIN CLIENT_TEST CT ON DU.INV_CODE = CT.CLIENT_CODEKYC
    WHERE DU.VERIFICATION_FLAG = '1' 
    AND DU.REJECTION_STATUS = '0' 
    AND DU.TRAN_TYPE = 'AC' 
    AND DU.PUNCHING_FLAG = '0' 
    AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Calculate ROW_COUNT
    SELECT NVL(COUNT(*), 0) INTO ROW_COUNT 
    FROM TB_DOC_UPLOAD DU
    JOIN CLIENT_TEST CT ON DU.INV_CODE = CT.CLIENT_CODEKYC
    JOIN CLIENT_MASTER CM ON CM.CLIENT_CODE = SUBSTR(CT.CLIENT_CODEKYC, 1, 8)
    WHERE DU.VERIFICATION_FLAG = '1' 
    AND DU.REJECTION_STATUS = '0' 
    AND DU.TRAN_TYPE = 'AC' 
    AND DU.PUNCHING_FLAG = '0' 
    AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);
    
    -- Calculate CM_FLAG
    SELECT NVL(COUNT(COMMON_ID), 0) 
    INTO CM_FLAG
    FROM TB_DOC_UPLOAD DU
    JOIN CLIENT_MASTER CM 
        ON CM.CLIENT_CODE = SUBSTR(DU.INV_CODE, 1, 8) 
    WHERE DU.VERIFICATION_FLAG = '1' 
        AND DU.REJECTION_STATUS = '0' 
        AND DU.TRAN_TYPE = 'AC' 
        AND DU.PUNCHING_FLAG = '0'
        AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Scenario 1: Only COMMON exists (no INV, CT, or ROW)
    IF COMMON_FLAG > 0 AND INV_FLAG = 0 AND CM_FLAG = 0 AND CT_COUNT = 0 AND ROW_COUNT = 0 THEN
        OPEN P_RESULT FOR
            SELECT 
                'Only DT Exist - Open New Account' AS message,
                NVL(tdu.common_id, '') AS COMMON_ID,
                NVL(tdu.common_id, '') AS DOC_ID,            
                NVL(em.payroll_id, 'No code') AS business_code,
                NVL(em.rm_name, 'No RM') AS rm_name,
                NVL(
                    (SELECT branch_master.branch_name 
                     FROM branch_master 
                     WHERE branch_master.branch_code = tdu.busi_branch_code 
                     FETCH FIRST 1 ROWS ONLY), 
                    'No Branch'
                ) AS BRANCH_CODE,  
                NVL(tdu.guest_cd, '') AS guest_code
            FROM tb_doc_upload tdu
            LEFT JOIN employee_master em ON em.payroll_id = tdu.busi_rm_code
            LEFT JOIN branch_master bm ON bm.branch_code = tdu.busi_branch_code
            WHERE tdu.common_id = TRIM(P_CLIENT_CODE)
            and rownum = 1
            
            ;



    -- Scenario 3: COMMON, row_count
    ELSIF COMMON_FLAG > 0 AND ROW_COUNT > 0 THEN
        OPEN P_RESULT FOR
            SELECT DISTINCT
                'Account Exist' AS message,
                NVL(tdu.common_id, '') AS COMMON_ID,
                NVL(tdu.common_id, '') AS DOC_ID,            
                NVL(em.payroll_id, 'No code') AS business_code,
                NVL(em.rm_name, 'No RM') AS rm_name,
                NVL( (SELECT branch_master.branch_name FROM branch_master WHERE branch_master.branch_code = tdu.busi_branch_code FETCH FIRST 1 ROWS ONLY),  'No Branch' ) AS BRANCH_CODE,  
                NVL(tdu.guest_cd, '') AS guest_code,
                NVL(tdu.inv_code, '') AS inv_code,
                NVL(SUBSTR(tdu.inv_code, 1, 8), '') AS cm_client_code,
                NVL(ct.client_code,'') AS ct_client_code

                
            FROM tb_doc_upload tdu
            LEFT JOIN employee_master em ON em.payroll_id = tdu.busi_rm_code
            LEFT JOIN branch_master bm ON bm.branch_code = tdu.busi_branch_code
            LEFT JOIN client_test ct ON ct.client_codekyc = tdu.inv_code

            WHERE tdu.COMMON_ID = TRIM(P_CLIENT_CODE)
            AND ROWNUM = 1;

    -- Scenario 2: COMMON and INV exist
    ELSIF COMMON_FLAG > 0 AND INV_FLAG > 0 and row_count =0 THEN
        OPEN P_RESULT FOR
            SELECT DISTINCT
                'Only Investor Exist' AS message,
                NVL(tdu.common_id, '') AS COMMON_ID,
                NVL(tdu.common_id, '') AS DOC_ID,            
                NVL(em.payroll_id, 'No code') AS business_code,
                NVL(em.rm_name, 'No RM') AS rm_name,
                NVL(
                    (SELECT branch_master.branch_name 
                     FROM branch_master 
                     WHERE branch_master.branch_code = tdu.busi_branch_code 
                     FETCH FIRST 1 ROWS ONLY), 
                    'No Branch'
                ) AS BRANCH_CODE,  
                NVL(tdu.guest_cd, '') AS guest_code,
                NVL(tdu.inv_code, '') AS inv_code
            FROM tb_doc_upload tdu
            LEFT JOIN employee_master em ON em.payroll_id = tdu.busi_rm_code
            LEFT JOIN branch_master bm ON bm.branch_code = tdu.busi_branch_code
            WHERE tdu.COMMON_ID = TRIM(P_CLIENT_CODE)
            AND ROWNUM = 1;
            
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        OPEN P_RESULT FOR
        SELECT 
            '' AS DOC_ID,
            '' AS COMMON_ID,
            '' AS CLIENT_CODE
        FROM DUAL;
END PSM_AO_GET_CLIENTBYDT;
