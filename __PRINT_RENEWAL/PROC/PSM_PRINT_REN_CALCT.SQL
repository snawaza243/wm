CREATE OR REPLACE PROCEDURE PSM_PRINT_REN_CALCT(
    PX_LATER_TYPE IN VARCHAR2,
    PX_MON_YEAR IN VARCHAR2,
    PX_LOGE IN VARCHAR2,
    PX_ROLE IN VARCHAR2,
    PX_CURSOR OUT SYS_REFCURSOR
) AS 

    V_MON VARCHAR2(2);
    V_YEAR VARCHAR2(4);
    V_DATE1 VARCHAR2(10);
    V_DATE2 VARCHAR2(10);
    V_SQL VARCHAR2(4000);

BEGIN
    SAVEPOINT start_transaction;

    IF PX_MON_YEAR IS NOT NULL THEN
        V_MON := SUBSTR(PX_MON_YEAR, 1, 2);
        V_YEAR := SUBSTR(PX_MON_YEAR, 4, 4);

        -- Set V_DATE1 to '01/MM/YYYY'
        V_DATE1 := '01/' || LPAD(V_MON, 2, '0') || '/' || V_YEAR;

        -- Set V_DATE2 based on month and leap year logic
        IF V_MON IN ('01','03','05','07','08','10','12') THEN
            V_DATE2 := '31/' || LPAD(V_MON, 2, '0') || '/' || V_YEAR;
        ELSIF V_MON IN ('04','06','09','11') THEN
            V_DATE2 := '30/' || LPAD(V_MON, 2, '0') || '/' || V_YEAR;
        ELSIF V_MON = '02' AND MOD(TO_NUMBER(V_YEAR), 4) = 0 THEN
            V_DATE2 := '29/02/' || V_YEAR;
        ELSE
            V_DATE2 := '28/02/' || V_YEAR;
        END IF;
    END IF;
 
    -- Handle Letter Type A
    IF PX_LATER_TYPE = 'A' THEN
        -- Truncate the temporary table
        EXECUTE IMMEDIATE 'TRUNCATE TABLE RenewalLetter_Vw_sub';
        
        -- Prepare SQL with variables embedded directly
        V_SQL := 'INSERT INTO RenewalLetter_Vw_sub ' ||
                 'SELECT A.CLIENT_NAME, A.ADDRESS1, A.ADDRESS2, A.CITY_ID, B.GRADE, A.SOURCECODE, ' ||
                 'A.INVESTOR_NAME, A.AMOUNT, A.MAT_PERIOD, A.TRAN_CODE, A.ARDATE, A.MUT_NAME, ' ||
                 'A.CHEQUE_NO, A.CHEQUE_DATE, A.BANK_NAME, A.RENEWALDATE, A.CITY_NAME, A.PINCODE ' ||
                 'FROM MS_RENVEL_OP_NEW A, ISS_COMP_GRADE B ' ||
                 'WHERE A.ISS_CODE = B.ISS_CODE ' ||
                 'AND A.RENEWALDATE >= TO_DATE(''' || V_DATE1 || ''', ''DD/MM/YYYY'') ' ||
                 'AND A.RENEWALDATE <= TO_DATE(''' || V_DATE2 || ''', ''DD/MM/YYYY'') ' ||
                 'AND B.MON_NO = ' || V_MON ||
                 ' AND B.YEAR_NO = ' || V_YEAR ||
                 ' AND B.GRADE = ''' || PX_LATER_TYPE || '''';
        
        EXECUTE IMMEDIATE V_SQL;
        
        -- Truncate the main view table
        EXECUTE IMMEDIATE 'TRUNCATE TABLE RenewalLetter_Vw';
        
        -- Insert aggregated data into main view table
        V_SQL := 'INSERT INTO RenewalLetter_Vw ' ||
                 'SELECT MAX(A.SOURCECODE) as SOURCECODE1, MAX(A.CLIENT_NAME) as CLIENT_NAME1, ' ||
                 'MAX(A.ADDRESS1) as ADD1, MAX(A.ADDRESS2) as ADD2, MAX(A.CITY_ID) as CITY_CD, ' ||
                 'MAX(A.MUT_NAME) as MUTNAME, MAX(A.ARDATE) as ARDATE, MAX(A.CITY_NAME) as CITY_NAME, ' ||
                 'MAX(A.PINCODE) as PINCODE ' ||
                 'FROM RenewalLetter_Vw_sub A WHERE 1=1 ' ||
                 'GROUP BY SOURCECODE';
        
        EXECUTE IMMEDIATE V_SQL;
        
    -- Handle Letter Type B
    ELSIF PX_LATER_TYPE = 'B' THEN
        -- Truncate the temporary table for type B
        EXECUTE IMMEDIATE 'TRUNCATE TABLE RenewalLetter_Vw_sub_B';
        
        -- Prepare SQL with variables embedded directly
        V_SQL := 'INSERT INTO RenewalLetter_Vw_sub_B ' ||
                 'SELECT A.CLIENT_NAME, A.ADDRESS1, A.ADDRESS2, A.CITY_ID, B.GRADE, A.SOURCECODE, ' ||
                 'A.INVESTOR_NAME, A.AMOUNT, A.MAT_PERIOD, A.TRAN_CODE, A.ARDATE, A.MUT_NAME, ' ||
                 'A.CHEQUE_NO, A.CHEQUE_DATE, A.BANK_NAME, A.RENEWALDATE, A.CITY_NAME, A.PINCODE ' ||
                 'FROM MS_RENVEL_OP_NEW A, ISS_COMP_GRADE B ' ||
                 'WHERE A.ISS_CODE = B.ISS_CODE ' ||
                 'AND A.RENEWALDATE >= TO_DATE(''' || V_DATE1 || ''', ''DD/MM/YYYY'') ' ||
                 'AND A.RENEWALDATE <= TO_DATE(''' || V_DATE2 || ''', ''DD/MM/YYYY'') ' ||
                 'AND B.MON_NO = ' || V_MON ||
                 ' AND B.YEAR_NO = ' || V_YEAR ||
                 ' AND B.GRADE = ''' || PX_LATER_TYPE || '''';
        
        EXECUTE IMMEDIATE V_SQL;
        
    END IF;
    
    -- Commit the transaction if everything succeeded
    COMMIT;
    
    -- Return success message
    OPEN PX_CURSOR FOR 
        SELECT 'SUCCESS: Calculated Successfully' AS msg, tb.* FROM RenewalLetter_Vw_sub_B tb;

EXCEPTION
    WHEN OTHERS THEN
        -- Rollback to the savepoint in case of any error
        ROLLBACK TO start_transaction;
        
        -- Re-raise the exception to notify the caller
        RAISE_APPLICATION_ERROR(-20001, 'Error in PSM_PRINT_REN_CALCT: ' || SQLERRM);
END PSM_PRINT_REN_CALCT;
/