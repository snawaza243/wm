CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_PRINT_REN_REPORT(
    PX_LATER_TYPE   IN VARCHAR2,
    PX_MON_YEAR     IN VARCHAR2,
    PX_LOGE         IN VARCHAR2,
    PX_MAIN_SUB     IN VARCHAR2, -- 1 = MAIN, 2 SUB
    PX_SUB_CODE     IN VARCHAR2, -- SOURCE_CODE FOR SUB REPORT
    PX_ROLE         IN VARCHAR2,
    PX_CURSOR       OUT SYS_REFCURSOR
) AS 
    V_DATE1         DATE;
    V_DATE2         DATE;
    V_MON_YEAR_VALIDATED VARCHAR2(7);
BEGIN
    -- Validate input parameters
    IF PX_LATER_TYPE NOT IN ('A', 'B') THEN
        RAISE_APPLICATION_ERROR(-20002, 'Invalid PX_LATER_TYPE. Must be A or B.');
    END IF;
    
    IF PX_MAIN_SUB NOT IN ('1', '2') THEN
        RAISE_APPLICATION_ERROR(-20003, 'Invalid PX_MAIN_SUB. Must be 1 or 2.');
    END IF;

    SAVEPOINT start_transaction;

    -- Process month/year parameter
    IF PX_MON_YEAR IS NOT NULL THEN
        -- Validate format MM/YYYY
        BEGIN
            V_MON_YEAR_VALIDATED := TO_CHAR(TO_DATE('01/' || PX_MON_YEAR, 'DD/MM/YYYY'), 'DD/MM/YYYY');
            V_DATE1 := TO_DATE('01/' || PX_MON_YEAR, 'DD/MM/YYYY');
            V_DATE2 := LAST_DAY(V_DATE1);
        EXCEPTION
            WHEN OTHERS THEN
                RAISE_APPLICATION_ERROR(-20004, 'Invalid PX_MON_YEAR format. Use MM/YYYY');
        END;
    END IF;

    -- Main logic with proper validation
    IF PX_LATER_TYPE = 'A' THEN
        IF PX_MAIN_SUB = '1' THEN
            OPEN PX_CURSOR FOR 
                SELECT
                    SOURCECODE1,
                    CLIENT_NAME1,
                    ADD1,
                    ADD2, 
                    MUTNAME,
                    TO_CHAR(ARDATE,'DD/MM/YYYY') AS ARDATE,
                    CITY_NAME,
                    PINCODE
                FROM RenewalLetter_Vw;
                
        ELSIF PX_MAIN_SUB = '2' THEN
            IF PX_SUB_CODE IS NULL THEN
                RAISE_APPLICATION_ERROR(-20005, 'PX_SUB_CODE is required for SUB report');
            END IF;
            
            OPEN PX_CURSOR FOR 
                SELECT MUT_NAME, AMOUNT, INVESTOR_NAME, MAT_PERIOD, ARDATE, 
                       CHEQUE_NO, CHEQUE_DATE, BANK_NAME
                FROM RenewalLetter_Vw_sub
                WHERE SOURCECODE1 = PX_SUB_CODE;
        END IF;
        
    ELSIF PX_LATER_TYPE = 'B' THEN
        OPEN PX_CURSOR FOR 
            SELECT tb.* FROM RenewalLetter_Vw_sub_B tb;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK TO start_transaction;
        RAISE_APPLICATION_ERROR(-20001, 
            'Error in PSM_PRINT_REN_REPORT: ' || SQLERRM || 
            ' (Error Code: ' || SQLCODE || ')');
END;
/