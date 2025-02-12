CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_GET_CLIENTBYDT (
    P_DT_VALUE IN VARCHAR2,  
    P_RESULT OUT SYS_REFCURSOR
) AS
    COMMON_FLAG NUMBER; 
    VERIFICATION_FLAG_CHECK VARCHAR2(1);
    REJECTION_STATUS_CHECK VARCHAR2(1);
    TRAN_TYPE_CHECK VARCHAR2(2);
    PUNCHING_FLAG_CHECK VARCHAR2(1);
    value_to_msg VARCHAR2(400);
BEGIN  
    -- Calculate the COMMON_FLAG
    SELECT NVL(COUNT(COMMON_ID), 0) INTO COMMON_FLAG 
    FROM TB_DOC_UPLOAD 
    WHERE VERIFICATION_FLAG = '1' 
    AND REJECTION_STATUS = '0' 
    AND TRAN_TYPE = 'AC' 
    AND PUNCHING_FLAG = '0' 
    AND COMMON_ID = TRIM(P_DT_VALUE);  

    -- Retrieve the flags individually for detailed error handling
    SELECT 
        VERIFICATION_FLAG,
        REJECTION_STATUS,
        TRAN_TYPE,
        PUNCHING_FLAG
    INTO 
        VERIFICATION_FLAG_CHECK,
        REJECTION_STATUS_CHECK,
        TRAN_TYPE_CHECK,
        PUNCHING_FLAG_CHECK
    FROM TB_DOC_UPLOAD
    WHERE COMMON_ID = TRIM(P_DT_VALUE)
    AND ROWNUM = 1;

    -- Construct the error message based on the flags
    value_to_msg := 'Invalid Data: This DT ' || P_DT_VALUE || ' is invalid because ';
    
    -- Check each flag and append an appropriate message with proper grammar
    IF VERIFICATION_FLAG_CHECK != '1' THEN
        value_to_msg := value_to_msg || 'it is not verified, ';
    END IF;
        
    IF REJECTION_STATUS_CHECK != '0' THEN
        value_to_msg := value_to_msg || 'it is rejected, ';
    END IF;
        
    IF TRAN_TYPE_CHECK != 'AC' THEN
        value_to_msg := value_to_msg || 'the transaction type is not AC, ';
    END IF;
        
    IF PUNCHING_FLAG_CHECK != '0' THEN
        value_to_msg := value_to_msg || 'it is already punched, ';
    END IF;
    
    -- Remove the trailing comma and space if any error conditions were appended
    IF value_to_msg LIKE '% ,%' THEN
        value_to_msg := RTRIM(value_to_msg, ', ');
    END IF;


    -- Scenario 1: If COMMON_FLAG > 0, retrieve the data
    IF COMMON_FLAG > 0 THEN
        OPEN P_RESULT FOR
            SELECT 
                'Valid Data: DT Exist - Open New Account' AS message,
                NVL(tdu.BUSI_RM_CODE, '') AS BUSI_RM_CODE,
                NVL(tdu.BUSI_RM_CODE, '') AS BUSI_BRANCH_CODE,
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
            AND ROWNUM = 1; 

    ELSE
        -- Scenario 2: If no matching records, return the constructed error message
        OPEN P_RESULT FOR
        SELECT 
            value_to_msg AS message,
            '' AS BUSI_RM_CODE,
            '' AS BUSI_BRANCH_CODE,
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
        -- Handle exceptions and provide error message for debugging
        OPEN P_RESULT FOR
        SELECT 
            'Error: '  AS message,
            '' AS BUSI_RM_CODE,
            '' AS BUSI_BRANCH_CODE,
            '' AS COMMON_ID,
            '' AS DOC_ID,
            '' AS business_code,
            '' AS rm_name,
            '' AS BRANCH_CODE,
            '' AS guest_code
        FROM DUAL;
END PSM_AO_GET_CLIENTBYDT;
