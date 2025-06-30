CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_UPDATE_BAJAJ_STATUS(
    P_PLYNO       IN VARCHAR2,
    P_COMPCD      IN VARCHAR2,
    P_STATUSCD    IN VARCHAR2,
    P_IMPORTTYPE  IN VARCHAR2,
    P_CMBMONTH    IN VARCHAR2,
    P_TXTYEAR     IN VARCHAR2,
    P_RESULT      OUT VARCHAR2
) AS
    V_SYSARNO     VARCHAR2(50);
    V_SYSARDT     VARCHAR2(50);
    V_EXISTS      NUMBER;
BEGIN
    -- Initialize output
    P_RESULT := 'SUCCESS';
    
    -- Query to check if the record exists in BAJAJ_AR_HEAD
    BEGIN
        SELECT sys_ar_no, TO_CHAR(sys_ar_dt, 'DD-MON-YYYY')
        INTO V_SYSARNO, V_SYSARDT
        FROM BAJAJ_AR_HEAD 
        WHERE POLICY_NO = P_PLYNO
        AND COMPANY_CD = P_COMPCD
        AND TO_CHAR(SYS_AR_DT, 'MON-YYYY') = UPPER(P_CMBMONTH) || '-' || P_TXTYEAR
        AND STATUS_CD = P_STATUSCD
        AND ROWNUM = 1; -- Get only one record if multiple exist
        
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            P_RESULT := 'No matching record found in BAJAJ_AR_HEAD';
            RETURN;
    END;
    
    -- Check if record exists in BAJAJ_AR_DETAILS
    BEGIN
        SELECT 1 INTO V_EXISTS
        FROM BAJAJ_AR_DETAILS 
        WHERE SYS_AR_NO = V_SYSARNO
        AND STATUS_DT = LAST_DAY(TO_DATE(V_SYSARDT, 'DD-MON-YYYY'))
        AND ROWNUM = 1;
        
        -- If we get here, record exists
        P_RESULT := 'Record already exists in BAJAJ_AR_DETAILS';
        RETURN;
        
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            NULL; -- Continue processing
    END;
    
    -- Update BAJAJ_AR_HEAD
    BEGIN
        UPDATE BAJAJ_AR_HEAD 
        SET STATUS_CD = P_STATUSCD
        WHERE SYS_AR_NO = V_SYSARNO;
        
        IF SQL%ROWCOUNT = 0 THEN
            P_RESULT := 'No rows updated in BAJAJ_AR_HEAD';
            RETURN;
        END IF;
    EXCEPTION
        WHEN OTHERS THEN
            P_RESULT := 'Error updating BAJAJ_AR_HEAD: ' || SQLERRM;
            RETURN;
    END;
    
    -- Insert into BAJAJ_AR_DETAILS
    BEGIN
        INSERT INTO BAJAJ_AR_DETAILS 
        (SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON) 
        VALUES 
        (V_SYSARNO, 
         LAST_DAY(TO_DATE(V_SYSARDT, 'DD-MON-YYYY')), 
         P_STATUSCD, 
         P_IMPORTTYPE || ' ' || TO_CHAR(SYSDATE, 'DD-MON-YYYY HH24:MI:SS'), 
         TO_DATE(V_SYSARDT, 'DD-MON-YYYY'), 
         SYSDATE);
         
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            P_RESULT := 'Error inserting into BAJAJ_AR_DETAILS: ' || SQLERRM;
            ROLLBACK;
            RETURN;
    END;
    
EXCEPTION
    WHEN OTHERS THEN
        P_RESULT := 'Unexpected error: ' || SQLERRM;
        ROLLBACK;
END PSM_UPDATE_BAJAJ_STATUS;
/


DECLARE
    V_RESULT VARCHAR2(4000);
BEGIN
    PSM_UPDATE_BAJAJ_STATUS(
        '12345',         -- policy number
        'BAJAJ',         -- company code
        'F',             -- status code
        'PAID DATA',     -- import type
        'JAN',           -- month
        '2023',          -- year
        V_RESULT         -- output parameter
    );
    DBMS_OUTPUT.PUT_LINE('Result: ' || V_RESULT);
END;
/