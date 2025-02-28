create or replace PROCEDURE psm_ao_inv_check_head(
    p_inv_code IN VARCHAR2, 
    p_cursor OUT SYS_REFCURSOR
) AS
    v_inv_flag NUMBER;
    v_only_cm NUMBER;

BEGIN
    -- Check if the investor exists in the client test table
    SELECT NVL(COUNT(inv_code), 0)
    INTO v_inv_flag
    FROM investor_master 
    WHERE inv_code = (SELECT client_codekyc FROM client_test WHERE client_codekyc = p_inv_code);


    -- Open the cursor with appropriate query based on the conditions
    IF v_inv_flag = 0 THEN
        OPEN p_cursor FOR 
            SELECT 
                CASE
                    WHEN v_only_cm > 0 THEN 'Invalid: Investor not in client test.' || p_inv_code
                    --WHEN v_only_cm > 0 THEN 'Invalid: Investor not in client test but in cm and im, inv code --> ' || p_inv_code

                    ELSE 'Invalid: Investor not in client test'
                END AS message
            FROM dual;
    ELSE
        OPEN p_cursor FOR
            SELECT 
                CASE
                    WHEN ct.client_code = ct.main_code THEN 'head'
                    ELSE 'member'
                END AS message,
                ct.client_code AS client_code, 
                ct.main_code AS main_code
            FROM client_test ct
            JOIN investor_master im 
                ON SUBSTR(im.inv_code, 1, 8) = ct.source_code  
            WHERE ct.client_codekyc = p_inv_code
            GROUP BY ct.client_code, ct.main_code;
    END IF;
END psm_ao_inv_check_head;