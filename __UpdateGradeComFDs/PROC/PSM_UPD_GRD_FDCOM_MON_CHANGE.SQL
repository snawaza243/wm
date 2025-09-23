CREATE OR REPLACE PROCEDURE PSM_UPD_GRD_FDCOM_MON_CHANGE(
    PX_LOGIN     IN VARCHAR2,
    PX_ROLE      IN VARCHAR2,
    PX_MONTH     IN VARCHAR2,
    PX_YEAR      IN VARCHAR2,
    PX_CURSOR    OUT SYS_REFCURSOR
) AS
    V_MON      VARCHAR2(2);
    V_YEAR     VARCHAR2(4);
    V_DATE1    VARCHAR2(10);
    V_DATE2    VARCHAR2(10);
    V_SQL      VARCHAR2(4000);
BEGIN

    IF PX_MONTH IS NULL OR PX_YEAR IS NULL THEN
        RAISE_APPLICATION_ERROR(-20001, 'Month and Year must be provided.');
    END IF;
    
    V_MON := LPAD(PX_MONTH, 2, '0');
    V_YEAR := PX_YEAR;

    -- Set V_DATE1 to '01/MM/YYYY'
    V_DATE1 := '01/' || V_MON || '/' || V_YEAR;

    -- Set V_DATE2 based on month and leap year logic
    IF V_MON IN ('01','03','05','07','08','10','12') THEN
        V_DATE2 := '31/' || V_MON || '/' || V_YEAR;
    ELSIF V_MON IN ('04','06','09','11') THEN
        V_DATE2 := '30/' || V_MON || '/' || V_YEAR;
    ELSIF V_MON = '02' AND MOD(TO_NUMBER(V_YEAR), 4) = 0 THEN
        V_DATE2 := '29/02/' || V_YEAR;
    ELSE
        V_DATE2 := '28/02/' || V_YEAR;
    END IF;

    V_SQL := '
        SELECT DISTINCT
            '''' AS tag,
            m.iss_code,
            m.mut_name COMPANY_NAME,
            g.grade
        FROM ms_renvel_op m
        LEFT JOIN iss_comp_grade g
          ON m.iss_code = g.iss_code
         AND g.mon_no = ' || TO_NUMBER(V_MON) || '
         AND g.year_no = ' || TO_NUMBER(V_YEAR) || '
        WHERE (m.naturecode = ''NT003'' OR m.pcode = ''DT010'')
          AND m.renewaldate >= TO_DATE(''' || V_DATE1 || ''', ''DD/MM/YYYY'')
          AND m.renewaldate <= TO_DATE(''' || V_DATE2 || ''', ''DD/MM/YYYY'')
        ORDER BY m.mut_name
    ';

    OPEN PX_CURSOR FOR V_SQL;
END PSM_UPD_GRD_FDCOM_MON_CHANGE;
/