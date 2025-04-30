CREATE OR REPLACE PROCEDURE PSM_MPN_EXPORT_2 (
    P_IMPORT_TYPE   IN VARCHAR2, -- AR, PC
    P_COMPANY_CD    IN VARCHAR2,
    P_CURSOR        OUT SYS_REFCURSOR
) AS 
    V_QUERY         VARCHAR2(32767);
    IS_POLICY       NUMBER := 0;
    IS_COMPANY      NUMBER := 0;
    IS_AR           NUMBER := 0;
    V_CHECK_IMP_TB  NUMBER := 0;
BEGIN
    -- This COUNT logic doesn't really count non-null rows for each column; it's not typical. But retaining your logic.
    SELECT 
        COUNT(POLICY_NO),   
        COUNT(COMPANY_CD),  
        COUNT(SYS_AR_NO)    
    INTO 
        IS_POLICY,     
        IS_COMPANY,         
        IS_AR
    FROM POLICY_MAP_TEMP1 
    WHERE ROWNUM = 1;

    V_QUERY := '';
    V_QUERY := V_QUERY || '
        SELECT DISTINCT 
            P.POLICY_NO         AS POLICY_NO, 
            MAX(A.PREM_AMT)     AS MAX_AMT, 
            DECODE(MAX(A.PREM_FREQ), 1, ''Y'', 2, ''HY'', 4, ''Q'', 12, ''M'') AS PREM_FREQ, 
            MAX(A.NEXT_DUE_DT)  AS NEXT_DUE_DT, 
            MAX(A.COMPANY_CD)   AS COMPANY_CD, 
            R.REGION_NAME       AS REGION_NAME, 
            Z.ZONE_NAME         AS ZONE_NAME, 
            E.RM_NAME           AS RM_NAME, 
            B.BRANCH_NAME       AS BRANCH_NAME, 
            I.INVESTOR_NAME     AS INVESTOR_NAME, 
            I.ADDRESS1          AS ADDRESS1, 
            I.ADDRESS2          AS ADDRESS2, 
            C.CITY_NAME         AS CITY_NAME, 
            S.STATE_NAME        AS STATE_NAME, 
            I.MOBILE            AS MOBILE, 
            I.PHONE             AS PHONE
        FROM 
            BRANCH_MASTER B, 
            ZONE_MASTER Z, 
            POLICY_MAP_TEMP1 P, 
            BAJAJ_AR_HEAD A,
            INVESTOR_MASTER I, 
            EMPLOYEE_MASTER E, 
            REGION_MASTER R, 
            STATE_MASTER S,
            CITY_MASTER C
        WHERE 
            I.CITY_ID = C.CITY_ID 
            AND C.STATE_ID = S.STATE_ID 
            AND E.RM_CODE = I.RM_CODE 
            AND UPPER(TRIM(P.POLICY_NO)) = UPPER(TRIM(A.POLICY_NO)) 
            AND A.CLIENT_CD = I.INV_CODE 
            AND I.BRANCH_CODE = B.BRANCH_CODE 
            AND B.REGION_ID = R.REGION_ID 
            AND B.ZONE_ID = Z.ZONE_ID
    ';

    -- Apply company filter if provided
    IF TRIM(P_COMPANY_CD) IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(A.COMPANY_CD)) = TRIM(UPPER(''' || P_COMPANY_CD || ''')) ';
    END IF;

    -- Apply SYS_AR_NO condition if column exists (IS_AR > 0)
    IF IS_AR > 0 THEN
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(A.SYS_AR_NO)) = TRIM(UPPER(P.SYS_AR_NO)) ';
    END IF;

    V_QUERY := V_QUERY || '
        GROUP BY 
            P.POLICY_NO,    
            P.COMPANY_CD,       
            B.BRANCH_NAME,      
            R.REGION_NAME, 
            Z.ZONE_NAME,    
            I.INVESTOR_NAME,    
            I.ADDRESS1,         
            I.ADDRESS2, 
            I.MOBILE,       
            I.PHONE,            
            E.RM_NAME,          
            C.CITY_NAME, 
            S.STATE_NAME
    ';

    OPEN P_CURSOR FOR V_QUERY;
END;
/
