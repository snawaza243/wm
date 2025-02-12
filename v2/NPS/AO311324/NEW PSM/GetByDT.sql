CREATE OR REPLACE PROCEDURE PSM_AO_GET_CLIENTBYDT (
    P_DT_VALUE IN VARCHAR2,  
    P_RESULT OUT SYS_REFCURSOR
) AS
     COMMON_FLAG INT; 
BEGIN
    -- Calculate the COMMON_FLAG
    SELECT NVL(COUNT(COMMON_ID), 0) INTO COMMON_FLAG 
    FROM TB_DOC_UPLOAD 
    WHERE VERIFICATION_FLAG = '1' 
    AND REJECTION_STATUS = '0' 
    AND TRAN_TYPE = 'AC' 
    AND PUNCHING_FLAG = '0' 
    AND COMMON_ID = TRIM(P_DT_VALUE);  -- Fixed parameter usage from P_CLIENT_CODE to P_DT_VALUE

    -- Scenario 1: 
    IF COMMON_FLAG > 0 THEN
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
            WHERE tdu.common_id = TRIM(P_DT_VALUE)
            and rownum = 1            
            ;


    ELSE
        -- If no matching records found, handle the case where COMMON_FLAG <= 0
        OPEN P_RESULT FOR
        SELECT 
            'Invalid Data: No valid DT found' AS message,
            '' AS COMMON_ID,
            '' AS DOC_ID,
            '' AS business_code,
            '' AS rm_name,
            '' AS BRANCH_CODE,
            '' AS guest_code
        FROM DUAL;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        -- In case of any exception, return a safe result
        OPEN P_RESULT FOR
        SELECT 
            'Error: An exception occurred' AS message,
            '' AS COMMON_ID,
            '' AS DOC_ID,
            '' AS business_code,
            '' AS rm_name,
            '' AS BRANCH_CODE,
            '' AS guest_code
        FROM DUAL;
END PSM_AO_GET_CLIENTBYDT;
