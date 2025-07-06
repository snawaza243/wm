CREATE OR REPLACE PROCEDURE MarkBajajData (
    p_company_text IN VARCHAR2,
    p_month        IN NUMBER,
    p_year         IN NUMBER
) AS
    v_comp_code VARCHAR2(100);
BEGIN
    -- Check if the company text is not empty
    IF p_company_text IS NOT NULL THEN
        -- Extract the company code (assumes '#' is the delimiter)
        v_comp_code := REGEXP_SUBSTR(p_company_text, '[^#]+', 1, 2);
    END IF;

    -- Execute the appropriate update query based on whether v_comp_code is populated
    IF v_comp_code IS NOT NULL THEN
        UPDATE bajaj_due_data
        SET rem_flage = 'YES'
        WHERE company_cd = v_comp_code
          AND mon_no = p_month
          AND year_no = p_year
          AND UPPER(TRIM(pay_mode)) = 'NON ECS'
          AND importdatatype = 'DUEDATA';
    ELSE
        UPDATE bajaj_due_data
        SET rem_flage = 'YES'
        WHERE mon_no = p_month
          AND year_no = p_year
          AND UPPER(TRIM(pay_mode)) = 'NON ECS'
          AND importdatatype = 'DUEDATA';
    END IF;

    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        -- Log the error or handle it as needed
        DBMS_OUTPUT.PUT_LINE('Error: ' || SQLERRM);
        ROLLBACK;
END MarkBajajData;
