 

CREATE OR REPLACE PROCEDURE PSM_DAP_Update_AR_Details (
    p_data_type     IN  VARCHAR2,
    p_month         IN  NUMBER,
    p_year          IN  NUMBER,
    p_cursor   OUT SYS_REFCURSOR
)
AS






    v_sql           VARCHAR2(4000);
    v_report_caption VARCHAR2(100);
BEGIN
    IF p_data_type = 'DUE' THEN
        v_report_caption := 'Due Data Imported Policy';
        v_sql := 'SELECT POLICY_NO, COMPANY_CD, IMPORTDATATYPE 
                  FROM BAJAJ_DUE_DATA 
                  WHERE MON_NO = :p_month 
                    AND YEAR_NO = :p_year 
                    AND IMPORTDATATYPE = ''DUEDATA''';
    ELSIF p_data_type = 'LAPSED' THEN
        v_report_caption := 'Lapsed Data Imported Policy';
        v_sql := 'SELECT POLICY_NO, COMPANY_CD, IMPORTDATATYPE 
                  FROM BAJAJ_DUE_DATA 
                  WHERE MON_NO = :p_month 
                    AND YEAR_NO = :p_year 
                    AND IMPORTDATATYPE = ''LAPSEDDATA''';
    ELSIF p_data_type = 'PAID' THEN
        v_report_caption := 'Paid Data Imported Policy';
        v_sql := 'SELECT POLICY_NO, COMPANY_CD, IMPORTDATATYPE 
                  FROM BAJAJ_PAID_DATA 
                  WHERE MON_NO = :p_month 
                    AND YEAR_NO = :p_year 
                    AND IMPORTDATATYPE = ''DUEDATA''';
    ELSIF p_data_type = 'REINS' THEN
        v_report_caption := 'Reinstate Data Imported Policy';
        v_sql := 'SELECT POLICY_NO, COMPANY_CD, IMPORTDATATYPE 
                  FROM BAJAJ_PAID_DATA 
                  WHERE MON_NO = :p_month 
                    AND YEAR_NO = :p_year 
                    AND IMPORTDATATYPE = ''LAPSEDDATA''';
    ELSE
        RAISE_APPLICATION_ERROR(-20001, 'Invalid data type specified');
    END IF;

    -- Open the cursor with the constructed SQL
    OPEN p_cursor FOR v_sql USING p_month, p_year;
END PSM_DAP_EXPORT_DATA;
/
