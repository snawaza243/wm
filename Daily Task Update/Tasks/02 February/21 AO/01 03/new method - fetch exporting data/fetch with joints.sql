CREATE OR REPLACE PROCEDURE psm_mpn_export_data (
    p_company_cd IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS 
BEGIN
    OPEN p_cursor FOR
    SELECT /*+ PARALLEL(8) */ -- Parallel execution speeds up query
        P.POLICY_NO, 
        MAX(A.PREM_AMT) AS max_amt, 
        DECODE(MAX(A.PREM_FREQ), 1, 'Y', 2, 'HY', 4, 'Q', 12, 'M') AS PREM_FREQ, 
        MAX(A.NEXT_DUE_DT) AS NEXT_DUE_DT, 
        MAX(A.COMPANY_CD) AS COMPANY_CD, 
        R.REGION_NAME, 
        Z.ZONE_NAME, 
        E.RM_NAME, 
        B.BRANCH_NAME, 
        I.INVESTOR_NAME, 
        I.ADDRESS1, 
        I.ADDRESS2, 
        C.CITY_NAME, 
        S.STATE_NAME, 
        I.MOBILE, 
        I.PHONE 
    FROM 
        POLICY_MAP_TEMP1 P
        JOIN bajaj_ar_head A ON P.POLICY_NO = A.POLICY_NO  
        JOIN investor_master I ON A.CLIENT_CD = I.inv_Code 
        JOIN branch_master B ON I.BRANCH_CODE = B.BRANCH_CODE 
        JOIN region_master R ON B.region_id = R.region_id 
        JOIN zone_master Z ON B.zone_id = Z.zone_id 
        JOIN employee_master E ON E.RM_CODE = I.RM_CODE 
        JOIN city_master C ON I.CITY_ID = C.CITY_ID 
        JOIN state_master S ON C.STATE_ID = S.STATE_ID 
    WHERE 
        (p_company_cd IS NULL OR A.COMPANY_CD = p_company_cd) 
    GROUP BY 
        P.POLICY_NO, B.BRANCH_NAME, R.REGION_NAME, Z.ZONE_NAME, 
        I.INVESTOR_NAME, I.ADDRESS1, I.ADDRESS2, 
        I.MOBILE, I.PHONE, E.RM_NAME, C.CITY_NAME, S.STATE_NAME;
END;
