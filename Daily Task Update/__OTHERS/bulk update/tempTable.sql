-- 1. Create a temporary table
CREATE GLOBAL TEMPORARY TABLE TEMP_POLICY_UPDATES (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50)
) ON COMMIT PRESERVE ROWS;

-- 2. Procedure to perform bulk update
CREATE OR REPLACE PROCEDURE BULK_UPDATE_POLICIES(
    p_file_name IN VARCHAR2,
    p_payment_mode IN VARCHAR2,
    p_update_prog IN VARCHAR2,
    p_update_user IN VARCHAR2,
    p_success OUT NUMBER,
    p_error_message OUT VARCHAR2
)
AS
BEGIN
    p_success := 1;
    
    BEGIN
        -- Update from temp table
        UPDATE POLICY_DETAILS_MASTER p
        SET 
            FILE_NAME = p_file_name,
            PAYMENT_MODE = p_payment_mode,
            UPDATE_PROG = p_update_prog,
            UPDATE_USER = p_update_user,
            UPDATE_DATE = SYSDATE
        WHERE EXISTS (
            SELECT 1 FROM TEMP_POLICY_UPDATES t
            WHERE UPPER(TRIM(p.POLICY_no)) = UPPER(TRIM(t.policy_no))
            AND UPPER(TRIM(p.COMPANY_CD)) = UPPER(TRIM(t.company_cd))
        );
        
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            p_success := 0;
            p_error_message := SQLERRM;
            ROLLBACK;
    END;
END BULK_UPDATE_POLICIES;
/