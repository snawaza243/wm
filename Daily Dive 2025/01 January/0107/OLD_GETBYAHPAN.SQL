create or replace PROCEDURE             PSM_AO_GET_CLIENT_BY_ID_PAN(
    P_CLIENT_CODE       IN  VARCHAR2,
    P_CLIENT_PAN        IN  VARCHAR2,
    P_EXIST_CLIENT_CODE IN  VARCHAR2,
    P_RESULT            OUT SYS_REFCURSOR
) AS
BEGIN
        OPEN P_RESULT FOR
        SELECT
        'Client Data Exist' as message,
            nvl(DI.common_id, '')                      AS doc_id,
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
            cm.ref_mob4               AS ref_mob4
       FROM
        client_test ct
        LEFT JOIN client_master cm ON cm.client_code = SUBSTR(ct.client_codekyc, 1, 8)
        LEFT JOIN employee_master em ON em.payroll_id = CT.business_code
        left join investor_master im on im.inv_code = ct.client_codekyc
        LEFT JOIN tb_doc_upload DI ON DI.INV_CODE = TO_CHAR(CT.CLIENT_CODEKYC)
    WHERE 
        (UPPER(CT.CLIENT_CODE) = UPPER(P_CLIENT_CODE) OR P_CLIENT_CODE IS NULL)
        AND (UPPER(CT.CLIENT_PAN) = UPPER(P_CLIENT_PAN) OR P_CLIENT_PAN IS NULL)
      --  AND (UPPER(CT.CLIENT_CODEKYC) = UPPER(P_EXIST_CLIENT_CODE) OR P_EXIST_CLIENT_CODE IS NULL)
        AND UPPER(CT.CLIENT_CODE) = UPPER(CT.MAIN_CODE);

END PSM_AO_GET_CLIENT_BY_ID_PAN;