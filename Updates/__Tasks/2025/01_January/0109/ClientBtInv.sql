create or replace PROCEDURE psm_ao_get_client_by_invcode (
    p_inv_code IN VARCHAR2, -- Common ID
    p_result   OUT SYS_REFCURSOR
) AS
    row_count   INT;
    common_flag INT;
    inv_flag    INT;
    ct_count    INT;
    valid_inv   INT;
    dt_flag     INT;
BEGIN
    -- VALID INV CODE
    SELECT nvl(COUNT(*), 0) 
    INTO valid_inv
    FROM investor_master
    WHERE investor_master.inv_code = p_inv_code;

    -- Scenario 1: COMMON and INV exist 
    IF valid_inv > 0 THEN
        OPEN p_result FOR 
        SELECT
        'Valid Data: Investor exist' as message,
                    ''                        AS doc_id,
                    nvl(ct.client_code, '')   AS client_code,
                    im.pan                    AS client_pan,
                    im.inv_code               AS client_codekyc,
                    im.source_id              AS source_code,
                    im.approved               AS approved,
                    nvl(ct.approved_flag, '') AS approved_flag,
                    em.payroll_id             AS business_code,
                    em.rm_name                AS rm_name,
                    em.rm_code                AS BRANCH_CODE,
                    cm.guest_cd               AS guest_code,
                    cm.status                 AS status,
                    im.occupation_id          AS occ_id,
                    nvl(ct.status, '')        AS ct_status,
                    cm.category_id            AS cm_category_id,
                    nvl(ct.act_cat, '')       AS ct_act_cat,
                    im.investor_title         AS title,
                    im.investor_name          AS client_name,
                    nvl(ct.title_other, '')   AS title_other,
                    nvl(ct.other, '')         AS other,
                    nvl(ct.others1, '')       AS others1,
                    im.gender                 AS gender,
                    im.mar_status             AS marital_status,
                    nvl(ct.nationality, '')   AS nationality,
                    nvl(ct.resident_nri, '')  AS resident_nri,
                    im.dob                    AS dob,
                    nvl(ct.annual_income, '') AS annual_income,
                    cm.lead_source            AS lead_type,
                    im.g_name                 AS g_name,
                    nvl(ct.g_nationality, '') AS g_nationality,
                    im.g_pan                  AS g_pan,
                    im.address1               AS add1,
                    im.address2               AS add2,
                    nvl(ct.state_id, '')      AS state_id,
                    im.pincode                AS pincode,
                    im.city_id                AS city_id,
                    cm.per_add1               AS per_add1,
                    cm.per_add2               AS per_add2,
                    cm.per_state_id           AS per_state_id,
                    cm.per_city_id            AS per_city_id,
                    cm.per_pincode            AS per_pincode,
                    nvl(ct.overseas_add, '')  AS overseas_add,
                    im.aadhar_card_no         AS aadhar_card_no,
                    im.email                  AS email,
                    cm.fax                    AS FAX,
                    cm.office_email           AS office_email,
                    im.phone1                 AS tel1,
                    im.phone2                 AS tel2,
                    cm.std1                   AS std1,
                    cm.std2                   AS std2,
                    im.mobile                 AS mobile_no,
                    cm.ref_name1              AS ref_name1,
                    cm.ref_name2              AS ref_name2,
                    cm.ref_name3              AS ref_name3,
                    cm.ref_name4              AS ref_name4,
                    cm.ref_mob1               AS ref_mob1,
                    cm.ref_mob2               AS ref_mob2,
                    cm.ref_mob3               AS ref_mob3,
                    cm.ref_mob4               AS ref_mob4,
                    
                            -- Mailing Address Details
                    CIM_MAIL.city_id            AS Mailing_City,
                    STM_MAIL.state_id           AS Mailing_State,
                    ctm_mail.country_id         AS Mailing_Country,
                
                    -- Permanent Address Details
                    CIM_PERM.city_id            AS Permanent_City,
                    STM_PERM.STATE_ID           AS Permanent_State,
                    CTM_PERM.COUNTRY_ID         AS Permanent_Country

               FROM
    investor_master im
    LEFT JOIN client_master cm ON cm.client_code = SUBSTR(p_inv_code, 1, 8)
    LEFT JOIN employee_master em ON em.rm_code = im.rm_code
    LEFT JOIN client_test ct ON ct.client_codekyc = p_inv_code
    
    
        -- Mailing Address Joins
    LEFT JOIN CITY_MASTER CIM_MAIL ON CIM_MAIL.CITY_ID = cm.CITY_ID
    LEFT JOIN STATE_MASTER STM_MAIL ON STM_MAIL.STATE_ID = CIM_MAIL.STATE_ID
    LEFT JOIN COUNTRY_MASTER CTM_MAIL ON CTM_MAIL.COUNTRY_ID = STM_MAIL.COUNTRY_ID

    -- Permanent Address Joins
    LEFT JOIN CITY_MASTER CIM_PERM ON CIM_PERM.CITY_ID = CM.PER_CITY_ID
    LEFT JOIN STATE_MASTER STM_PERM ON STM_PERM.STATE_ID = CIM_PERM.STATE_ID
    LEFT JOIN COUNTRY_MASTER CTM_PERM ON CTM_PERM.COUNTRY_ID = STM_PERM.COUNTRY_ID
    


    
WHERE
    im.inv_code = p_inv_code AND ROWNUM = 1;
    ELSE
        OPEN p_result FOR SELECT
                              '' AS doc_id,
                              '' AS client_code,
                              '' AS client_pan,
                              '' AS client_codekyc,
                              '' AS main_code,
                              '' AS approved,
                              '' AS approved_flag,
                              '' AS business_code_1,
                              '' AS source_code,
                              '' AS status,
                              '' AS occ_id,
                              '' AS ct_status,
                              '' AS cm_category_id,
                              '' AS ct_act_cat,
                              '' AS title,
                              '' AS client_name,
                              '' AS title_other,
                              '' AS other,
                              '' AS others1,
                              '' AS gender,
                              '' AS marital_status,
                              '' AS nationality,
                              '' AS resident_nri,
                              '' AS dob,
                              '' AS annual_income,
                              '' AS client_pan,
                              '' AS lead_type,
                              '' AS g_name,
                              '' AS g_nationality,
                              '' AS g_pan,
                              '' AS add1,
                              '' AS add2,
                              '' AS city_id,
                              '' AS state_id,
                              '' AS pincode,
                              '' AS per_add1,
                              '' AS per_add2,
                              '' AS per_city_id,
                              '' AS per_state_id,
                              '' AS per_pincode,
                              '' AS fax,
                              '' AS aadhar_card_no,
                              '' AS email,
                              '' AS office_email,
                              '' AS std1,
                              '' AS tel1,
                              '' AS std2,
                              '' AS tel2,
                              '' AS mobile_no,
                              '' AS ref_name1,
                              '' AS ref_name2,
                              '' AS ref_name3,
                              '' AS ref_name4,
                              '' AS ref_mob1,
                              '' AS ref_mob2,
                              '' AS ref_mob3,
                              '' AS ref_mob4,
                            ''            AS Mailing_City,
                            ''           AS Mailing_State,
                            ''         AS Mailing_Country,
                        
                            -- Permanent Address Details
                            ''            AS Permanent_City,
                            ''           AS Permanent_State,
                            ''         AS Permanent_Country
                          FROM
                              dual;

    END IF;

END psm_ao_get_client_by_invcode;  