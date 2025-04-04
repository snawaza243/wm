CREATE OR REPLACE PROCEDURE get_policy_map_data (
    p_company_cd IN VARCHAR2,
    p_cursor OUT SYS_REFCURSOR
)
AS
BEGIN
    -- Delete existing data from the temporary table
    EXECUTE IMMEDIATE 'DELETE FROM POLICY_MAP_TEMP1';

    -- Perform the select query to retrieve the necessary data
    OPEN p_cursor FOR
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
        branch_master b
        JOIN zone_master z ON b.zone_id = z.zone_id
        JOIN POLICY_MAP_TEMP1 p ON TRIM(UPPER(p.POLICY_NO)) = TRIM(UPPER(a.POLICY_NO))
        JOIN bajaj_ar_head a ON TRIM(UPPER(p.POLICY_NO)) = TRIM(UPPER(a.POLICY_NO))
        JOIN investor_master i ON i.inv_Code = a.CLIENT_CD
        JOIN employee_master e ON e.payroll_id = TO_CHAR(a.emp_no)
        JOIN region_master r ON b.region_id = r.region_id
        JOIN state_master s ON c.state_id = s.state_id
        JOIN city_master c ON i.city_id = c.city_id
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
        AND (p_company_cd IS NULL OR TRIM(UPPER(p.COMPANY_CD)) = TRIM(UPPER(p_company_cd)))
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
END get_policy_map_data;
/
