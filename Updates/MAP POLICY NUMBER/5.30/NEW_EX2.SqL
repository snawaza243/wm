CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MPN_EXPORT_2 (
    P_IMPORT_TYPE   IN VARCHAR2,  -- AR, PC
    P_COMPANY_CD    IN VARCHAR2,
    P_CURSOR        OUT SYS_REFCURSOR
) AS
    V_QUERY         VARCHAR2(32767);
    IS_POLICY       NUMBER := 0;
    IS_COMPANY      NUMBER := 0;
    IS_AR           NUMBER := 0;
    V_NEW_PLY       NUMBER := 1;
BEGIN
    -- Check if relevant columns exist in the temporary table
    SELECT  COUNT(POLICY_NO),   COUNT(COMPANY_CD),  COUNT(SYS_AR_NO)
    INTO    IS_POLICY,          IS_COMPANY,         IS_AR
    FROM POLICY_MAP_TEMP1 WHERE ROWNUM = 1;

    IF V_NEW_PLY > 0 THEN 
        IF IS_AR > 0 AND IS_POLICY = 0 THEN            
            OPEN P_CURSOR FOR  
                SELECT  
                    A.SYS_AR_NO                         AS AR_NO,
                    TO_CHAR(A.SYS_AR_DT, 'DD-MON-YYYY') AS AR_DATE,
                    TO_CHAR(MAX(A.NEXT_DUE_DT), 'DD-MON-YYYY') AS NEXT_DUE_DATE,
                    D.REMARKS                           AS REMARKS,
                    FI.ITEMNAME                         AS CHANNEL,
                    R.REGION_NAME                       AS REGION, 
                    Z.ZONE_NAME                         AS ZONE,
                    B.BRANCH_NAME                       AS BRANCH, 
                    E.RM_NAME                           AS RM_NAME, 
                    D.STATUS_CD                         AS STATUS,
                    TO_CHAR(D.STATUS_DT, 'DD-MON-YYYY') AS STATUS_DATE,
                    DECODE(A.PREM_FREQ, 1, 'Y', 2, 'HY', 4, 'Q', 12, 'M') AS FREQ, 
                    A.PREM_AMT                          AS PREMIUM,
                    A.COMPANY_CD                        AS COMPANY_CD,   
                    PM.PLAN                             AS PLAN_NAME,
                    A.POLICY_NO                         AS POLICY_NO, 
                    A.PAYMENT_MODE                      AS PAYMENT_MODE,
                    A.SA                                AS SA,
                    TO_CHAR(A.PLY_ISSUE_DT, 'DD-MON-YYYY') AS ISSUE_DATE,
                    A.FRESH_RENEWAL                     AS FRESH_RENEWAL,
                    PM.SUB_CATEGORY                     AS PLAN_TYPE 
                 FROM 
                    BAJAJ_AR_HEAD A
                    --LEFT JOIN BAJAJ_AR_DETAILS D ON A.SYS_AR_NO = D.SYS_AR_NO
                    LEFT JOIN (SELECT * FROM BAJAJ_AR_DETAILS D1 WHERE D1.STATUS_DT = (SELECT MAX(D2.STATUS_DT) FROM BAJAJ_AR_DETAILS D2 WHERE D2.SYS_AR_NO = D1.SYS_AR_NO )) D ON A.SYS_AR_NO = D.SYS_AR_NO
                    JOIN INVESTOR_MASTER I ON A.CLIENT_CD = I.INV_CODE
                    JOIN EMPLOYEE_MASTER E ON E.RM_CODE = I.RM_CODE
                    JOIN BRANCH_MASTER B ON I.BRANCH_CODE = B.BRANCH_CODE
                    JOIN REGION_MASTER R ON B.REGION_ID = R.REGION_ID
                    JOIN ZONE_MASTER Z ON B.ZONE_ID = Z.ZONE_ID
                    JOIN CITY_MASTER C ON I.CITY_ID = C.CITY_ID
                    JOIN STATE_MASTER S ON C.STATE_ID = S.STATE_ID
                    JOIN BAJAJ_PLAN_MASTER PM ON A.PLAN_NO = PM.PLAN_NO
                    JOIN FIXEDITEM FI ON B.BRANCH_TAR_CAT = FI.ITEMSERIALNUMBER
                    JOIN POLICY_MAP_TEMP1 P ON A.SYS_AR_NO = P.SYS_AR_NO
                 WHERE 
                    TRIM(A.SYS_AR_NO) = TRIM(P.SYS_AR_NO)
                 GROUP BY  
                    A.SYS_AR_NO, A.SYS_AR_DT, D.REMARKS, FI.ITEMNAME, R.REGION_NAME, Z.ZONE_NAME,
                    B.BRANCH_NAME, E.RM_NAME, D.STATUS_CD, D.STATUS_DT, A.PREM_FREQ, A.PREM_AMT, 
                    A.COMPANY_CD, PM.PLAN, A.POLICY_NO, A.PAYMENT_MODE, A.SA, A.PLY_ISSUE_DT,
                    A.FRESH_RENEWAL, PM.SUB_CATEGORY;

        ELSIF IS_POLICY > 0 AND IS_COMPANY > 0 AND IS_AR = 0 THEN
            IF TRIM(P_COMPANY_CD) IS NULL THEN
                OPEN P_CURSOR FOR 
                    SELECT  
                        A.SYS_AR_NO                         AS AR_NO,
                        TO_CHAR(A.SYS_AR_DT, 'DD-MON-YYYY') AS AR_DATE,
                        TO_CHAR(MAX(A.NEXT_DUE_DT), 'DD-MON-YYYY') AS NEXT_DUE_DATE,
                        D.REMARKS                           AS REMARKS,
                        FI.ITEMNAME                         AS CHANNEL,
                        R.REGION_NAME                       AS REGION, 
                        Z.ZONE_NAME                         AS ZONE,
                        B.BRANCH_NAME                       AS BRANCH, 
                        E.RM_NAME                           AS RM_NAME, 
                        D.STATUS_CD                         AS STATUS,
                        TO_CHAR(D.STATUS_DT, 'DD-MON-YYYY') AS STATUS_DATE,
                        DECODE(A.PREM_FREQ, 1, 'Y', 2, 'HY', 4, 'Q', 12, 'M') AS FREQ, 
                        A.PREM_AMT                          AS PREMIUM,
                        A.COMPANY_CD                        AS COMPANY_CD,   
                        PM.PLAN                             AS PLAN_NAME,
                        MAX(P.POLICY_NO)                    AS POLICY_NO,   
                        A.PAYMENT_MODE                      AS PAYMENT_MODE,
                        A.SA                                AS SA,
                        TO_CHAR(A.PLY_ISSUE_DT, 'DD-MON-YYYY') AS ISSUE_DATE,
                        A.FRESH_RENEWAL                     AS FRESH_RENEWAL,
                        PM.SUB_CATEGORY                     AS PLAN_TYPE 
                    FROM 
                        BAJAJ_AR_HEAD A
                        --LEFT JOIN BAJAJ_AR_DETAILS D ON A.SYS_AR_NO = D.SYS_AR_NO
                        LEFT JOIN (SELECT * FROM BAJAJ_AR_DETAILS D1 WHERE D1.STATUS_DT = (SELECT MAX(D2.STATUS_DT) FROM BAJAJ_AR_DETAILS D2 WHERE D2.SYS_AR_NO = D1.SYS_AR_NO )) D ON A.SYS_AR_NO = D.SYS_AR_NO
                        JOIN INVESTOR_MASTER I ON A.CLIENT_CD = I.INV_CODE
                        JOIN EMPLOYEE_MASTER E ON E.RM_CODE = I.RM_CODE
                        JOIN BRANCH_MASTER B ON I.BRANCH_CODE = B.BRANCH_CODE
                        JOIN REGION_MASTER R ON B.REGION_ID = R.REGION_ID
                        JOIN ZONE_MASTER Z ON B.ZONE_ID = Z.ZONE_ID
                        JOIN CITY_MASTER C ON I.CITY_ID = C.CITY_ID
                        JOIN STATE_MASTER S ON C.STATE_ID = S.STATE_ID
                        JOIN BAJAJ_PLAN_MASTER PM ON A.PLAN_NO = PM.PLAN_NO
                        JOIN FIXEDITEM FI ON B.BRANCH_TAR_CAT = FI.ITEMSERIALNUMBER
                        JOIN POLICY_MAP_TEMP1 P ON A.POLICY_NO = P.POLICY_NO
                    WHERE 
                        A.FRESH_RENEWAL = '1'  
                        AND UPPER(TRIM(A.COMPANY_CD)) = UPPER(TRIM(P.COMPANY_CD))
                    GROUP BY  
                        A.SYS_AR_NO, A.SYS_AR_DT, D.REMARKS, FI.ITEMNAME, R.REGION_NAME, Z.ZONE_NAME,
                        B.BRANCH_NAME, E.RM_NAME, D.STATUS_CD, D.STATUS_DT, A.PREM_FREQ, A.PREM_AMT, 
                        A.COMPANY_CD, PM.PLAN, A.PAYMENT_MODE, A.SA, A.PLY_ISSUE_DT,
                        A.FRESH_RENEWAL, PM.SUB_CATEGORY;                        
            END IF; 
        ELSE
            OPEN P_CURSOR FOR 
                SELECT DISTINCT(P.POLICY_NO)                    AS POLICY_NO,
                    MAX(A.PREM_AMT)                             AS MAX_AMT,
                    DECODE(MAX(A.PREM_FREQ), 1, 'Y', 2, 'HY', 4, 'Q', 12, 'M') AS PREM_FREQ,
                    MAX(A.NEXT_DUE_DT)                          AS NEXT_DUE_DT,
                    MAX(A.COMPANY_CD)                           AS COMPANY_CD,
                    R.REGION_NAME                               AS REGION_NAME,
                    Z.ZONE_NAME                                 AS ZONE_NAME,
                    E.RM_NAME                                   AS RM_NAME,
                    B.BRANCH_NAME                               AS BRANCH_NAME,
                    I.INVESTOR_NAME                             AS INVESTOR_NAME,
                    I.ADDRESS1                                  AS ADDRESS1,
                    I.ADDRESS2                                  AS ADDRESS2,
                    C.CITY_NAME                                 AS CITY_NAME,
                    S.STATE_NAME                                AS STATE_NAME,
                    I.MOBILE                                    AS MOBILE,
                    I.PHONE                                     AS PHONE
                FROM BRANCH_MASTER B,
                    ZONE_MASTER Z,
                    POLICY_MAP_TEMP1 P,
                    BAJAJ_AR_HEAD A,
                    INVESTOR_MASTER I,
                    EMPLOYEE_MASTER E,
                    REGION_MASTER R,
                    STATE_MASTER S,
                    CITY_MASTER C
                WHERE I.CITY_ID = C.CITY_ID
                    AND C.STATE_ID = S.STATE_ID
                    AND E.RM_CODE = I.RM_CODE
                    AND TRIM(TO_CHAR(P.POLICY_NO)) = TRIM(TO_CHAR(A.POLICY_NO))
                    AND A.CLIENT_CD = I.INV_CODE
                    AND I.BRANCH_CODE = B.BRANCH_CODE
                    AND B.REGION_ID = R.REGION_ID
                    AND B.ZONE_ID = Z.ZONE_ID
                    AND (TRIM(UPPER(p.COMPANY_CD)) = UPPER(TRIM(a.COMPANY_CD)))
                    AND (P_COMPANY_CD IS NULL OR a.COMPANY_CD = P_COMPANY_CD)
                GROUP BY P.POLICY_NO, P.COMPANY_CD, B.BRANCH_NAME, R.REGION_NAME,
                    Z.ZONE_NAME, I.INVESTOR_NAME, I.ADDRESS1, I.ADDRESS2,
                    I.MOBILE, I.PHONE, E.RM_NAME, C.CITY_NAME, S.STATE_NAME;
        END IF;
    END IF;
END;
/