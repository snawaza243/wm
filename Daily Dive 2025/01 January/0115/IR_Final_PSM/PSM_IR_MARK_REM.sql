CREATE OR REPLACE PROCEDURE PSM_IR_MARK_REM (
    p_company_cd IN VARCHAR2,
    p_month        IN NUMBER,
    p_year         IN NUMBER,
    p_policy_no    IN VARCHAR2,
    p_result_msg   OUT VARCHAR2
) AS
    v_comp_code    VARCHAR2(100);
    v_updated_rows NUMBER;
BEGIN


    IF p_policy_no IS NOT NULL THEN
        UPDATE bajaj_due_data
        SET rem_flage = 'YES'
        WHERE POLICY_NO = p_policy_no
          AND UPPER(TRIM(pay_mode)) = 'NON ECS'
          AND importdatatype = 'DUEDATA'
          AND (p_month IS NULL OR mon_no = p_month)
          AND (p_year IS NULL OR year_no = p_year)
        ;
                
    ELSE
        IF p_company_cd IS NOT NULL THEN
            UPDATE bajaj_due_data
            SET rem_flage = 'YES'
            WHERE company_cd = p_company_cd
              AND mon_no = p_month
              AND year_no = p_year
              AND UPPER(TRIM(pay_mode)) = 'NON ECS'
              AND importdatatype = 'DUEDATA';
        ELSE
            UPDATE bajaj_due_data
            SET rem_flage = 'YES'
            WHERE  mon_no = p_month
              AND year_no = p_year
              AND UPPER(TRIM(pay_mode)) = 'NON ECS'
              AND importdatatype = 'DUEDATA';
              
        END IF ;
    END IF;

    -- Get the number of rows updated
    v_updated_rows := SQL%ROWCOUNT;

    -- Prepare the result message
    p_result_msg := TO_CHAR(v_updated_rows) || ' record(s) remark flage updated successfully.';

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        p_result_msg := 'Error: ' || SQLERRM;
        ROLLBACK;
END PSM_IR_MARK_REM;
