CREATE OR REPLACE PROCEDURE PSM_MPN_EXPORT_2 (
    P_IMPORT_TYPE   IN VARCHAR2,  -- AR, PC
    P_COMPANY_CD    IN VARCHAR2,
    P_CURSOR        OUT SYS_REFCURSOR
) AS
    V_QUERY         VARCHAR2(32767);
    IS_POLICY       NUMBER := 0;
    IS_COMPANY      NUMBER := 0;
    IS_AR           NUMBER := 0;
BEGIN
    -- Check if relevant columns exist in the temporary table
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

    -- Build dynamic SQL
    V_QUERY := '
        SELECT  
            A.SYS_AR_NO         AS AR_NO,
            FI.ITEMNAME         AS CHANNEL,
            R.REGION_NAME       AS REGION, 
            Z.ZONE_NAME         AS ZONE,
            B.BRANCH_NAME       AS BRANCH, 
            E.RM_NAME           AS RM_NAME, 
            D.STATUS_CD         AS STATUS,
            TO_CHAR(D.STATUS_DT, ''DD-MON-YYYY'')         AS STATUS_DATE,
            DECODE(A.PREM_FREQ, 1, ''Y'', 2, ''HY'', 4, ''Q'', 12, ''M'') AS FREQ, 
            A.PREM_AMT          AS PREMIUM,
            A.COMPANY_CD        AS COMPANY_CD,   
            PM.PLAN             AS PLAN_NAME,
            P.POLICY_NO         AS POLICY_NO, 
            A.PAYMENT_MODE      AS PAYMENT_MODE,
            A.SA                AS SA,
            TO_CHAR(A.PLY_ISSUE_DT, ''DD-MON-YYYY'') AS ISSUE_DATE,
            A.FRESH_RENEWAL     AS FRESH_RENEWAL,
            PM.SUB_CATEGORY     AS PLAN_TYPE
        FROM 
            BRANCH_MASTER B, 
            ZONE_MASTER Z, 
            POLICY_MAP_TEMP1 P, 
            BAJAJ_AR_HEAD A,
            BAJAJ_AR_DETAILS D,
            BAJAJ_PLAN_MASTER PM, 
            INVESTOR_MASTER I, 
            EMPLOYEE_MASTER E, 
            REGION_MASTER R, 
            STATE_MASTER S,
            CITY_MASTER C, 
            FIXEDITEM FI
        WHERE 
            I.CITY_ID         = C.CITY_ID 
            AND C.STATE_ID    = S.STATE_ID 
            AND E.RM_CODE     = I.RM_CODE 
            AND UPPER(TRIM(P.POLICY_NO)) = UPPER(TRIM(A.POLICY_NO)) 
            AND A.CLIENT_CD   = I.INV_CODE 
            AND I.BRANCH_CODE = B.BRANCH_CODE 
            AND B.REGION_ID   = R.REGION_ID 
            AND B.ZONE_ID     = Z.ZONE_ID
            AND A.SYS_AR_NO   = D.SYS_AR_NO(+)
            AND A.PLAN_NO     = PM.PLAN_NO
            AND B.BRANCH_TAR_CAT = FI.ITEMSERIALNUMBER ';


    IF IS_AR > 0 THEN
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(A.SYS_AR_NO)) = TRIM(UPPER(P.SYS_AR_NO)) ';
    ELSIF IS_POLICY > 0 THEN
        V_QUERY := V_QUERY || ' AND UPPER(TRIM(P.POLICY_NO)) = UPPER(TRIM(A.POLICY_NO)) ';
    END IF;


    -- Optional filter: Company Code
    IF TRIM(P_COMPANY_CD) IS NOT NULL THEN
        V_QUERY := V_QUERY || ' AND TRIM(UPPER(A.COMPANY_CD)) = TRIM(UPPER(''' || P_COMPANY_CD || ''')) ';
    END IF;
    
    

    V_QUERY := V_QUERY || '
        GROUP BY 
            A.SYS_AR_NO, 
            FI.ITEMNAME, 
            R.REGION_NAME, 
            Z.ZONE_NAME, 
            B.BRANCH_NAME, 
            E.RM_NAME, 
            D.STATUS_CD, 
            D.STATUS_DT, 
            A.PREM_FREQ, 
            A.PREM_AMT, 
            A.COMPANY_CD, 
            PM.PLAN, 
            P.POLICY_NO, 
            A.PAYMENT_MODE, 
            A.SA, 
            A.PLY_ISSUE_DT, 
            A.FRESH_RENEWAL, 
            PM.SUB_CATEGORY ';

    -- Open ref cursor
    OPEN P_CURSOR FOR V_QUERY;
END;
/
