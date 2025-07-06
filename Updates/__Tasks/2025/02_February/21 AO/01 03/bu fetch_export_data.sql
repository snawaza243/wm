create or replace PROCEDURE psm_mpn_export_data (
    p_company_cd IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
) AS 
BEGIN
    OPEN p_cursor FOR
    SELECT DISTINCT 
        P.POLICY_NO AS POLICY_NO, 
        MAX(A.PREM_AMT) AS max_amt, 
        DECODE(MAX(A.PREM_FREQ), 1, 'Y', 2, 'HY', 4, 'Q', 12, 'M') AS PREM_FREQ, 
        MAX(A.NEXT_DUE_DT) AS NEXT_DUE_DT, 
        MAX(A.COMPANY_CD) AS COMPANY_CD, 
        R.REGION_NAME AS REGION_NAME, 
        Z.ZONE_NAME AS ZONE_NAME, 
        E.RM_NAME AS RM_NAME, 
        B.BRANCH_NAME AS BRANCH_NAME, 
        I.INVESTOR_NAME AS INVESTOR_NAME, 
        I.ADDRESS1, 
        I.ADDRESS2, 
        C.CITY_NAME, 
        S.STATE_NAME, 
        I.MOBILE, 
        I.PHONE 
    FROM 
        branch_master B,
        zone_master Z,
        POLICY_MAP_TEMP1 P,
        bajaj_ar_head A,
        investor_master I,
        employee_master E,
        region_master R,
        STATE_MASTER S,
        CITY_MASTER C
    WHERE 
        I.CITY_ID = C.CITY_ID 
        AND C.STATE_ID = S.STATE_ID 
        AND E.RM_CODE = I.RM_CODE 
        AND UPPER(TRIM(P.POLICY_NO)) = UPPER(TRIM(A.POLICY_NO)) 
        AND A.CLIENT_CD = I.inv_Code 
        AND I.BRANCH_CODE = B.BRANCH_CODE 
        AND B.region_id = R.region_id 
        AND B.zone_id = Z.zone_id 
        AND (p_company_cd IS NULL OR TRIM(UPPER(a.COMPANY_CD)) = TRIM(UPPER(p_company_cd)))
    GROUP BY 
        P.policy_no, P.company_cd,  B.BRANCH_NAME,  R.region_name, Z.zone_name, I.INVESTOR_NAME, I.address1, 
        I.address2, 
        I.mobile, 
        I.phone, 
        E.rm_name, 
        C.CITY_NAME, 
        S.STATE_NAME;
END;