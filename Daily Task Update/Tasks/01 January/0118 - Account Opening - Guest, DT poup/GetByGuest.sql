create or replace PROCEDURE PSM_AO_GET_CLIENT_BY_ID(
    P_CLIENT_CODE      IN  VARCHAR2,
    P_RESULT           OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN P_RESULT FOR
        SELECT
        'Valid Guest Code'   AS message,
        ''      AS doc_id,
        ''      AS client_code,
        ''      AS main_code,
        ''      AS client_pan,
        ''      AS client_codekyc,
        ''      AS source_code,
        ''      AS approved,
        ''      AS approved_flag,
        ''      AS business_code,
        ''      AS rm_name,
        ''      AS BRANCH_CODE ,
        ''      AS guest_code,
        ''      AS status,
        ''      AS occ_id,
        ''      AS ct_status,
        ''      AS cm_category_id,
        ''      AS ct_act_cat,
        ''      AS title,
        ''      AS client_name,
        ''      AS title_other,
        ''      AS other,
        ''      AS others1,
        ''      AS gender,
        ''      AS marital_status,
        ''      AS nationality,
        ''      AS resident_nri,
        ''      AS dob,
        ''      AS annual_income,
        ''      AS lead_type,
        ''      AS g_name,
        ''      AS g_nationality,
        ''      AS g_pan,
        ''      AS add1,
        ''      AS add2,
        ''      AS state_id,
        ''      AS pincode,
        ''      AS city_id,
        ''      AS per_add1,
        ''      AS per_add2,
        ''      AS per_state_id,
        ''      AS per_city_id,
        ''      AS per_pincode,
        ''      AS overseas_add,
        ''      AS aadhar_card_no,
        ''      AS email,
        ''      AS FAX,
        ''      AS office_email,
        ''      AS tel1,
        ''      AS tel2,
        ''      AS std1,
        ''      AS std2,
        ''      AS mobile_no,
        ''      AS ref_name1,
        ''      AS ref_name2,
        ''      AS ref_name3,
        ''      AS ref_name4,
        ''      AS ref_mob1,
        ''      AS ref_mob2,
        ''      AS ref_mob3,
        ''      AS ref_mob4,
        ''      AS Mailing_City,
        ''      AS Mailing_State,
        ''      AS Mailing_Country,
        ''      AS Permanent_City,
        ''      AS Permanent_State,
        ''      AS Permanent_Country

    FROM
    client_test ct
    LEFT JOIN client_master cm ON cm.client_code = SUBSTR(ct.client_codekyc, 1, 8)
    LEFT JOIN employee_master em ON em.payroll_id = CT.business_code
    left join investor_master im on im.inv_code = ct.client_codekyc
    LEFT JOIN tb_doc_upload DI ON DI.INV_CODE = TO_CHAR(CT.CLIENT_CODEKYC)
    LEFT JOIN branch_master bm ON bm.branch_code = CT.BRANCH_CODE

        -- Mailing Address Joins
    LEFT JOIN CITY_MASTER CIM_MAIL ON CIM_MAIL.CITY_ID = cm.CITY_ID
    LEFT JOIN STATE_MASTER STM_MAIL ON STM_MAIL.STATE_ID = CIM_MAIL.STATE_ID
    LEFT JOIN COUNTRY_MASTER CTM_MAIL ON CTM_MAIL.COUNTRY_ID = STM_MAIL.COUNTRY_ID

    -- Permanent Address Joins
    LEFT JOIN CITY_MASTER CIM_PERM ON CIM_PERM.CITY_ID = CM.PER_CITY_ID
    LEFT JOIN STATE_MASTER STM_PERM ON STM_PERM.STATE_ID = CIM_PERM.STATE_ID
    LEFT JOIN COUNTRY_MASTER CTM_PERM ON CTM_PERM.COUNTRY_ID = STM_PERM.COUNTRY_ID

    WHERE CT.CLIENT_CODE = P_CLIENT_CODE
    AND ROWNUM = 1; 
END PSM_AO_GET_CLIENT_BY_ID;