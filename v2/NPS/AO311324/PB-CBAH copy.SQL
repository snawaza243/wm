create or replace PROCEDURE PSM_AO_GET_CLIENT_BY_ID(
    P_CLIENT_CODE      IN  VARCHAR2,
    P_RESULT           OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN P_RESULT FOR
        SELECT
        'Valid Data: Client Exist' AS message,

        nvl(DI.common_id, '')     AS doc_id,
        nvl(ct.client_code, '')   AS client_code,
        im.pan                    AS client_pan,
        im.inv_code               AS client_codekyc,
        im.source_id              AS source_code,
        im.approved               AS approved,
        nvl(ct.approved_flag, '') AS approved_flag,
        em.payroll_id             AS business_code,
        em.rm_name                AS rm_name,
        NVL((SELECT branch_master.branch_name FROM branch_master WHERE branch_master.branch_code = CT.BRANCH_CODE  FETCH FIRST 1 ROWS ONLY), 'No Branch') AS BRANCH_CODE,  

        NVL(cm.guest_cd, '')               AS guest_code,
        cm.status                 AS status,
        im.occupation_id          AS occ_id,
        nvl(ct.status, '')        AS ct_status,
        cm.category_id            AS cm_category_id,
        nvl(ct.act_cat, '')       AS ct_act_cat,
        ct.title         AS title,
        ct.client_name AS client_name,
        nvl(ct.title_other, '')   AS title_other,
        nvl(ct.other, '')         AS other,
        nvl(ct.others1, '')       AS others1,
        im.gender                 AS gender,
        ct.marital_status             AS marital_status,
        nvl(ct.nationality, '')   AS nationality,
        nvl(ct.resident_nri, '')  AS resident_nri,
        im.dob                    AS dob,
        nvl(ct.annual_income, '') AS annual_income,
        cm.lead_source            AS lead_type,
        ct.g_name                 AS g_name,
        nvl(ct.g_nationality, '') AS g_nationality,
        ct.g_pan                  AS g_pan,
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
        ct.aadhar_card_no         AS aadhar_card_no,
        ct.email                  AS email,
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
        cm.ref_mob4               AS ref_mob4
    FROM
    client_test ct
    LEFT JOIN client_master cm ON cm.client_code = SUBSTR(ct.client_codekyc, 1, 8)
    LEFT JOIN employee_master em ON em.payroll_id = CT.business_code
    left join investor_master im on im.inv_code = ct.client_codekyc
    LEFT JOIN tb_doc_upload DI ON DI.INV_CODE = TO_CHAR(CT.CLIENT_CODEKYC)

 WHERE CT.CLIENT_CODE = P_CLIENT_CODE
    AND ROWNUM = 1; 


END PSM_AO_GET_CLIENT_BY_ID;