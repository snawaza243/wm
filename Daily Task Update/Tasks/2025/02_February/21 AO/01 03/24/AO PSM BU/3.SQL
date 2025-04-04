create or replace PROCEDURE             PSM_AO_GET_CLIENTBYDT(
    P_DT_VALUE IN VARCHAR2,  
    P_RESULT OUT SYS_REFCURSOR
) AS
    COMMON_FLAG NUMBER;
    VERIFICATION_FLAG_CHECK VARCHAR2(1);
    REJECTION_STATUS_CHECK VARCHAR2(1);
    TRAN_TYPE_CHECK VARCHAR2(2);
    TRAN_TYPE_FLAG VARCHAR2(5);
    PUNCHING_FLAG_CHECK VARCHAR2(1);
    VALUE_TO_MSG VARCHAR2(400);
    VFLAG        NUMBER:=0;
BEGIN  

    SELECT COUNT(*)
    INTO COMMON_FLAG
    FROM TB_DOC_UPLOAD
    WHERE COMMON_ID = TRIM(P_DT_VALUE);

    ---- add a validation here to check the givcen dt is exit in db tbale or not if not then return with return mesage and message named columnd
    IF COMMON_FLAG = 0 THEN
        OPEN P_RESULT FOR
        SELECT
            'Invalid Data: This DT ' || P_DT_VALUE || ' does not exist in the database.' AS message,
            '' AS BUSI_RM_CODE,
            '' AS BUSI_BRANCH_CODE,
            '' AS COMMON_ID,
            '' AS DOC_ID,
            '' AS business_code,
            '' AS rm_name,
            '' AS BRANCH_CODE,
            '' AS guest_code
        FROM DUAL;
        RETURN;
    END IF;



     /* This Dt No Contains The Ac Documnet Or Not*/
    SELECT NVL(COUNT(TRAN_TYPE), 0) AS TRAN_TYPE_FLAG
    INTO TRAN_TYPE_FLAG
    FROM TB_DOC_UPLOAD
    WHERE COMMON_ID = TRIM(P_DT_VALUE)
    AND (TRAN_TYPE = 'AC');


    -- Retrieve the flags individually for detailed error handling
    BEGIN
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
        WHERE COMMON_ID = TRIM(P_DT_VALUE) AND TRAN_TYPE = 'AC' AND ROWNUM = 1;
    EXCEPTION WHEN NO_DATA_FOUND THEN
        VERIFICATION_FLAG_CHECK:=NULL;
        REJECTION_STATUS_CHECK:=NULL;
        TRAN_TYPE_CHECK:=NULL;
        PUNCHING_FLAG_CHECK:=NULL;
    END;    

    -- Construct the error message based on the flags
    VFLAG:=0;
    value_to_msg := 'Invalid Data: This DT ' || P_DT_VALUE || ' is invalid because ';

    IF TRAN_TYPE_FLAG =0 THEN
        value_to_msg := value_to_msg || ' does not contains the document for AC.';
        VFLAG:=1;
    ELSE
        -- Check each flag and append an appropriate message with proper grammar
        IF VERIFICATION_FLAG_CHECK = '0' THEN
            value_to_msg := value_to_msg || 'it is not verified, ';
            VFLAG:=1;
        END IF;

        IF REJECTION_STATUS_CHECK = '1' THEN
            value_to_msg := value_to_msg || 'it is rejected, ';
            VFLAG:=1;
        END IF;

        IF VERIFICATION_FLAG_CHECK='1' AND PUNCHING_FLAG_CHECK= '1' THEN
            value_to_msg := value_to_msg || 'it is already punched, ';
            VFLAG:=1;
        END IF;

        -- Remove the trailing comma and space if any error conditions were appended
        IF value_to_msg LIKE '%,%' THEN
            value_to_msg := RTRIM(value_to_msg, ', ');
        END IF;
    END IF;

    IF VFLAG =1 THEN
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

        RETURN;
     END IF;

    -- Scenario 1: If COMMON_FLAG > 0, retrieve the data
    IF COMMON_FLAG > 0 THEN
        OPEN P_RESULT FOR
            SELECT
                'Valid Data: DT Exist - Open New Account' AS message,
                NVL(tdu.BUSI_RM_CODE, '') AS BUSI_RM_CODE,
                NVL(tdu.BUSI_BRANCH_CODE, '') AS BUSI_BRANCH_CODE,
                NVL(tdu.common_id, '') AS COMMON_ID,
                NVL(tdu.common_id, '') AS DOC_ID,            
                NVL(em.payroll_id, 'No code') AS business_code,
                NVL(em.rm_name, 'No RM') AS rm_name,
                NVL(bm.branch_name, 'No Branch') AS BRANCH_CODE,  
                NVL(tdu.guest_cd, '') AS guest_code
            FROM tb_doc_upload tdu
            LEFT JOIN employee_master em ON em.payroll_id = tdu.busi_rm_code
            LEFT JOIN branch_master bm ON bm.branch_code = tdu.busi_branch_code
            WHERE tdu.common_id = TRIM(P_DT_VALUE)AND ROWNUM = 1;
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
            'Invalid Data: This DT ' || P_DT_VALUE || ' the transaction type is not AC' AS message,
            '' AS BUSI_RM_CODE,
            '' AS BUSI_BRANCH_CODE,
            '' AS COMMON_ID,
            '' AS DOC_ID,
            '' AS business_code,
            '' AS rm_name,
            '' AS BRANCH_CODE,
            '' AS guest_code
        FROM DUAL;
END ;
