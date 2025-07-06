SELECT DISTINCT
        p.POLICY_NO,
        MAX(a.PREM_AMT) AS max_amt,
        DECODE(MAX(a.PREM_FREQ), 1, 'Y', 2, 'HY', 4, 'Q', 12, 'M') AS PREM_FREQ,
        MAX(a.NEXT_DUE_DT) AS NEXT_DUE_DT,
        MAX(a.COMPANY_CD) AS COMPANY_CD,
        r.REGION_NAME AS REGION_NAME,
        z.ZONE_NAME AS ZONE_NAME,
        e.RM_NAME AS RM_NAME,
        b.BRANCH_NAME AS BRANCH_NAME,
        i.INVESTOR_NAME AS INVESTOR_NAME,
        i.ADDRESS1 AS ADDRESS1,
        i.ADDRESS2 AS ADDRESS2,
        c.CITY_NAME AS CITY_NAME,
        s.STATE_NAME AS STATE_NAME,
        i.MOBILE AS MOBILE,
        i.PHONE AS PHONE
    FROM
        branch_master b,
        zone_master z,
        POLICY_MAP_TEMP1 p,
        bajaj_ar_head a,
        investor_master i,
        employee_master e,
        region_master r,
        state_master s,
        city_master c
    WHERE
        a.FRESH_RENEWAL IN ('1', '5')
        AND TRIM(UPPER(p.POLICY_NO)) = TRIM(UPPER(a.POLICY_NO))
        AND i.CITY_ID = c.CITY_ID
        AND c.STATE_ID = s.STATE_ID
        AND e.RM_CODE = i.RM_CODE
        AND i.BRANCH_CODE = b.BRANCH_CODE
        AND b.REGION_ID = r.REGION_ID
        AND b.ZONE_ID = z.ZONE_ID
        AND a.CLIENT_CD = i.inv_Code
        --AND (p_company_cd IS NULL OR TRIM(UPPER(p.COMPANY_CD)) = TRIM(UPPER(p_company_cd)))
    GROUP BY
        p.POLICY_NO,
        p.COMPANY_CD,
        b.BRANCH_NAME,
        r.REGION_NAME,
        z.ZONE_NAME,
        i.INVESTOR_NAME,
        i.ADDRESS1,
        i.ADDRESS2,
        i.MOBILE,
        i.PHONE,
        e.RM_NAME,
        c.CITY_NAME,
        s.STATE_NAME;
