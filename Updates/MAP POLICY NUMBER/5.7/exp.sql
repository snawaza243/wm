CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MPN_EXPORT_2 (
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

    IF IS_AR > 0 THEN
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
                LEFT JOIN BAJAJ_AR_DETAILS D ON A.SYS_AR_NO = D.SYS_AR_NO
                JOIN INVESTOR_MASTER I ON A.CLIENT_CD = I.INV_CODE
                JOIN EMPLOYEE_MASTER E ON E.RM_CODE = I.RM_CODE
                JOIN BRANCH_MASTER B ON I.BRANCH_CODE = B.BRANCH_CODE
                JOIN REGION_MASTER R ON B.REGION_ID = R.REGION_ID
                JOIN ZONE_MASTER Z ON B.ZONE_ID = Z.ZONE_ID
                JOIN CITY_MASTER C ON I.CITY_ID = C.CITY_ID
                JOIN STATE_MASTER S ON C.STATE_ID = S.STATE_ID
                JOIN BAJAJ_PLAN_MASTER PM ON A.PLAN_NO = PM.PLAN_NO
                JOIN FIXEDITEM FI ON B.BRANCH_TAR_CAT = FI.ITEMSERIALNUMBER

                where A.SYS_AR_NO IN (SELECT SYS_AR_NO FROM POLICY_MAP_TEMP1 WHERE SYS_AR_NO IS NOT NULL)

            GROUP BY  
                A.SYS_AR_NO, A.SYS_AR_DT, D.REMARKS, FI.ITEMNAME, R.REGION_NAME, Z.ZONE_NAME,
                B.BRANCH_NAME, E.RM_NAME, D.STATUS_CD, D.STATUS_DT, A.PREM_FREQ, A.PREM_AMT, 
                A.COMPANY_CD, PM.PLAN, A.POLICY_NO, A.PAYMENT_MODE, A.SA, A.PLY_ISSUE_DT,
                A.FRESH_RENEWAL, PM.SUB_CATEGORY ;

    ELSIF IS_POLICY > 0 AND TRIM(P_COMPANY_CD) IS NOT NULL AND IS_COMPANY > 0 THEN
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
                LEFT JOIN BAJAJ_AR_DETAILS D ON A.SYS_AR_NO = D.SYS_AR_NO
                JOIN INVESTOR_MASTER I ON A.CLIENT_CD = I.INV_CODE
                JOIN EMPLOYEE_MASTER E ON E.RM_CODE = I.RM_CODE
                JOIN BRANCH_MASTER B ON I.BRANCH_CODE = B.BRANCH_CODE
                JOIN REGION_MASTER R ON B.REGION_ID = R.REGION_ID
                JOIN ZONE_MASTER Z ON B.ZONE_ID = Z.ZONE_ID
                JOIN CITY_MASTER C ON I.CITY_ID = C.CITY_ID
                JOIN STATE_MASTER S ON C.STATE_ID = S.STATE_ID
                JOIN BAJAJ_PLAN_MASTER PM ON A.PLAN_NO = PM.PLAN_NO
                JOIN FIXEDITEM FI ON B.BRANCH_TAR_CAT = FI.ITEMSERIALNUMBER

                where A.POLICY_NO IN (SELECT POLICY_NO FROM POLICY_MAP_TEMP1 WHERE POLICY_NO IS NOT NULL)
                AND UPPER(TRIM(a.company_cd)) = UPPER(TRIM(P_COMPANY_CD))
            GROUP BY  
                A.SYS_AR_NO, A.SYS_AR_DT, D.REMARKS, FI.ITEMNAME, R.REGION_NAME, Z.ZONE_NAME,
                B.BRANCH_NAME, E.RM_NAME, D.STATUS_CD, D.STATUS_DT, A.PREM_FREQ, A.PREM_AMT, 
                A.COMPANY_CD, PM.PLAN, A.POLICY_NO, A.PAYMENT_MODE, A.SA, A.PLY_ISSUE_DT,
                A.FRESH_RENEWAL, PM.SUB_CATEGORY ;

    ELSIF IS_POLICY > 0 THEN
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
                LEFT JOIN BAJAJ_AR_DETAILS D ON A.SYS_AR_NO = D.SYS_AR_NO
                JOIN INVESTOR_MASTER I ON A.CLIENT_CD = I.INV_CODE
                JOIN EMPLOYEE_MASTER E ON E.RM_CODE = I.RM_CODE
                JOIN BRANCH_MASTER B ON I.BRANCH_CODE = B.BRANCH_CODE
                JOIN REGION_MASTER R ON B.REGION_ID = R.REGION_ID
                JOIN ZONE_MASTER Z ON B.ZONE_ID = Z.ZONE_ID
                JOIN CITY_MASTER C ON I.CITY_ID = C.CITY_ID
                JOIN STATE_MASTER S ON C.STATE_ID = S.STATE_ID
                JOIN BAJAJ_PLAN_MASTER PM ON A.PLAN_NO = PM.PLAN_NO
                JOIN FIXEDITEM FI ON B.BRANCH_TAR_CAT = FI.ITEMSERIALNUMBER

                where A.POLICY_NO IN (SELECT POLICY_NO FROM POLICY_MAP_TEMP1 WHERE POLICY_NO IS NOT NULL)
            GROUP BY  
                A.SYS_AR_NO, A.SYS_AR_DT, D.REMARKS, FI.ITEMNAME, R.REGION_NAME, Z.ZONE_NAME,
                B.BRANCH_NAME, E.RM_NAME, D.STATUS_CD, D.STATUS_DT, A.PREM_FREQ, A.PREM_AMT, 
                A.COMPANY_CD, PM.PLAN, A.POLICY_NO, A.PAYMENT_MODE, A.SA, A.PLY_ISSUE_DT,
                A.FRESH_RENEWAL, PM.SUB_CATEGORY ;

    END IF;   
END;
/
