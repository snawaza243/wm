create or replace PROCEDURE psm_ao_inv_check_head(
    p_inv_code IN VARCHAR2, 
    p_cursor OUT SYS_REFCURSOR
) AS
    v_inv_flag NUMBER;
    v_only_cm NUMBER;
    v_in_cm     NUMBER;
    v_inv_head  number;

BEGIN
    -- Check if the investor exists in the client test table
    SELECT NVL(COUNT(inv_code), 0)
    INTO v_inv_flag
    FROM investor_master 
    WHERE inv_code = (SELECT client_codekyc FROM client_test WHERE client_codekyc = p_inv_code);

    SELECT NVL(COUNT(CLIENT_CODE), 0)
    INTO v_only_cm
    FROM CLIENT_MASTER 
    WHERE CLIENT_CODE = SUBSTR ((SELECT INV_CODE FROM INVESTOR_MASTER WHERE INV_CODE = p_inv_code AND ROWNUM = 1),1,8);
    
    
    SELECT nvl(inv_code,0)
    into v_inv_head
    FROM investor_master
    WHERE SUBSTR(inv_code, -3) = '001' AND investor_master.source_id = SUBSTR(p_inv_code, 1, 8) AND ROWNUM = 1;


    -- Open the cursor with appropriate query based on the conditions
    IF v_inv_flag = 0 THEN -- INV NOT IN CT
        IF v_only_cm > 0 THEN  
            OPEN p_cursor FOR
                SELECT 
                    CASE
                        WHEN v_inv_head = p_inv_code THEN 'inv head'
                        ELSE 'inv member'
                    END AS message
                    , v_inv_head AS client_code,  v_inv_head AS main_code
                FROM dual;
            
        ELSE
            OPEN p_cursor FOR 
                SELECT 
                    CASE
                        --WHEN v_only_cm > 0 THEN 'Invalid: Investor not in client test.' || p_inv_code
                        WHEN v_only_cm > 0 THEN 'Invalid: Investor not in client test but in cm and im, inv code --> ' || p_inv_code
    
                        ELSE 'Invalid: Investor data not found!'
                    END AS message
                FROM dual;
        END IF;
    ELSE
        OPEN p_cursor FOR
            SELECT 
                CASE
                    WHEN ct.client_code = ct.main_code THEN 'ct head'
                    ELSE 'ct member'
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