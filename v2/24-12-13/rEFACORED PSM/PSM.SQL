create or replace PROCEDURE PSM_ANA_GET_RM_BY_SRCBR (
    p_source_id     VARCHAR2,
    p_branch_cursor OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_branch_cursor FOR
    SELECT a.RM_CODE, a.AGENT_NAME, a.AGENT_CODE, a.EXIST_CODE,a.SOURCEID, b.BRANCH_CODE
    FROM AGENT_MASTER a
    LEFT JOIN BRANCH_MASTER b ON b.BRANCH_CODE = a.SOURCEID
    WHERE a.SOURCEID = p_source_id;
END PSM_ANA_GET_RM_BY_SRCBR;

create or replace PROCEDURE PSM_ANA_Get_AGENTLIST (
    p_sourceid       IN VARCHAR2,    -- New parameter: sourceid
    p_rm_code        IN VARCHAR2,    -- Existing parameter: rm_code
    p_agent_name     IN VARCHAR2,    -- New parameter: agent_name
    p_exist_code     IN VARCHAR2,    -- New parameter: exist_code
    p_agents_cursor  OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN p_agents_cursor FOR
    SELECT AGENT_CODE, EXIST_CODE, AGENT_NAME, JOININGDATE, Address1, Address2
    FROM AGENT_MASTER
    WHERE 
        (p_sourceid IS NULL OR SOURCEID = p_sourceid) -- Filter by sourceid if provided
        AND (p_rm_code IS NULL OR RM_CODE = p_rm_code) -- Filter by rm_code if provided
        AND (p_agent_name IS NULL OR UPPER(AGENT_NAME) LIKE '%' || UPPER(p_agent_name) || '%') -- Filter by agent_name if provided, case-insensitive match
        AND (p_exist_code IS NULL OR EXIST_CODE = p_exist_code)
        AND rownum <200;
END PSM_ANA_Get_AGENTLIST;

create or replace PROCEDURE PSM_AO_GET_CLIENTLIST (
    P_BRANCH_ID     IN VARCHAR2,
    P_MOBILE        IN VARCHAR2,
    P_CITY_ID       IN VARCHAR2,
    P_PHONE         IN VARCHAR2,
    P_CLIENT_CODE   IN VARCHAR2,
    P_BUSINESS_CD   IN VARCHAR2,
    P_CLIENT_NAME   IN VARCHAR2,
    P_CLIENT_LIST   OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN P_CLIENT_LIST FOR
    SELECT *
    FROM CLIENT_TEST
    WHERE 
        (P_BRANCH_ID IS NULL OR UPPER(BRANCH_CODE) LIKE '%' || UPPER(P_BRANCH_ID) || '%')
        AND (P_MOBILE IS NULL OR UPPER(MOBILE_NO) LIKE '%' || UPPER(P_MOBILE) || '%')
        AND (P_CITY_ID IS NULL OR UPPER(CITY_ID) LIKE '%' || UPPER(P_CITY_ID) || '%')
        AND (P_PHONE IS NULL OR UPPER(TEL2) LIKE '%' || UPPER(P_PHONE) || '%')
        AND (P_CLIENT_CODE IS NULL OR UPPER(CLIENT_CODE) LIKE '%' || UPPER(P_CLIENT_CODE) || '%')
        AND (P_BUSINESS_CD IS NULL OR UPPER(CLIENT_PAN) LIKE '%' || UPPER(P_BUSINESS_CD) || '%')
        AND (P_CLIENT_NAME IS NULL OR UPPER(CLIENT_NAME) LIKE '%' || UPPER(P_CLIENT_NAME) || '%')
        AND UPPER(CLIENT_CODE) = UPPER(MAIN_CODE)
        AND ROWNUM < 200
    ORDER BY UPPER(CLIENT_NAME);
END;

create or replace PROCEDURE PSM_AO_GET_CLIENTBYDT (
    P_CLIENT_CODE IN VARCHAR2, -- Common ID
    P_RESULT OUT SYS_REFCURSOR
) AS
    ROW_COUNT    INT; 
    COMMON_FLAG  INT; 
    INV_FLAG     INT;
    CT_COUNT     INT;
BEGIN
    -- Calculate the COMMON_FLAG
    SELECT NVL(COUNT(COMMON_ID), 0) INTO COMMON_FLAG 
    FROM TB_DOC_UPLOAD 
    WHERE VERIFICATION_FLAG = '1' 
    AND REJECTION_STATUS = '0' 
    AND TRAN_TYPE = 'AC' 
    AND PUNCHING_FLAG = '0' 
    AND COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Calculate INV_FLAG
    SELECT NVL(COUNT(*), 0) INTO INV_FLAG 
    FROM TB_DOC_UPLOAD DU
    JOIN INVESTOR_MASTER IM ON DU.INV_CODE = IM.INV_CODE
    WHERE DU.VERIFICATION_FLAG = '1' 
    AND DU.REJECTION_STATUS = '0' 
    AND DU.TRAN_TYPE = 'AC' 
    AND DU.PUNCHING_FLAG = '0' 
    AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Calculate CT_COUNT
    SELECT NVL(COUNT(*), 0) INTO CT_COUNT 
    FROM TB_DOC_UPLOAD DU
    JOIN CLIENT_TEST CT ON DU.INV_CODE = CT.CLIENT_CODEKYC
    WHERE DU.VERIFICATION_FLAG = '1' 
    AND DU.REJECTION_STATUS = '0' 
    AND DU.TRAN_TYPE = 'AC' 
    AND DU.PUNCHING_FLAG = '0' 
    AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Calculate ROW_COUNT
    SELECT NVL(COUNT(*), 0) INTO ROW_COUNT 
    FROM TB_DOC_UPLOAD DU
    JOIN CLIENT_TEST CT ON DU.INV_CODE = CT.CLIENT_CODEKYC
    JOIN CLIENT_MASTER CM ON CM.CLIENT_CODE = SUBSTR(CT.CLIENT_CODEKYC, 1, 8)
    WHERE DU.VERIFICATION_FLAG = '1' 
    AND DU.REJECTION_STATUS = '0' 
    AND DU.TRAN_TYPE = 'AC' 
    AND DU.PUNCHING_FLAG = '0' 
    AND DU.COMMON_ID = TRIM(P_CLIENT_CODE);

    -- Scenario 1: Only COMMON exists (no INV, CT, or ROW)
    IF COMMON_FLAG > 0 AND INV_FLAG = 0 AND CT_COUNT = 0 AND ROW_COUNT = 0 THEN
        OPEN P_RESULT FOR
        SELECT 
 'Valid Data: DT  (IM, CM, CT NOT EXIST) - Open New Account' AS message,
            TRIM(P_CLIENT_CODE) AS COMMON_ID,
            TRIM(P_CLIENT_CODE) AS DOC_ID,
                '' AS CLIENT_CODE,
                '' AS CLIENT_PAN,
                '' AS CLIENT_CODEKYC,
                '' AS MAIN_CODE,
                '' AS APPROVED,
                '' AS APPROVED_FLAG,
                em.payroll_id             AS business_code,
                em.rm_name                AS rm_name,
                em.rm_code                AS BRANCH_CODE,
                tb_doc_upload.guest_cd as guest_code,
                '0' AS SOURCE_CODE,
                '' AS STATUS,
                '' AS OCC_ID,
                '' AS CT_STATUS,
                '' AS CM_CATEGORY_ID,
                '' AS CT_ACT_CAT,
                '' AS TITLE,
                '' AS CLIENT_NAME,
                '' AS TITLE_OTHER,
                '' AS OTHER,
                '' AS OTHERS1,
                '' AS GENDER,
                '' AS MARITAL_STATUS,
                '' AS NATIONALITY,
                '' AS RESIDENT_NRI,
                '' AS DOB,
                '' AS ANNUAL_INCOME,
                '' AS CLIENT_PAN,
                '' AS LEAD_TYPE,
                '' AS G_NAME,
                '' AS G_NATIONALITY,
                '' AS G_PAN,
                '' AS ADD1,
                '' AS ADD2,
                '' AS CITY_ID,
                '' AS STATE_ID,
                '' AS PINCODE,
                '' AS PER_ADD1,
                '' AS PER_ADD2,
                '' AS PER_CITY_ID,
                '' AS PER_STATE_ID,
                '' AS PER_PINCODE,
                '' AS FAX,
                '' AS AADHAR_CARD_NO,
                '' AS EMAIL,
                '' AS OFFICE_EMAIL,
                '' AS STD1,
                '' AS TEL1,
                '' AS STD2,
                '' AS TEL2,
                '' AS MOBILE_NO,
                '' AS REF_NAME1,
                '' AS REF_NAME2,
                '' AS REF_NAME3,
                '' AS REF_NAME4,
                '' AS REF_MOB1,
                '' AS REF_MOB2,
                '' AS REF_MOB3,
                '' AS REF_MOB4
        from tb_doc_upload
LEFT JOIN employee_master em ON em.payroll_id = tb_doc_upload.busi_rm_code
LEFT JOIN branch_master bm ON bm.branch_code = tb_doc_upload.busi_branch_code
       where common_id = TRIM(P_CLIENT_CODE);

    -- Scenario 2: COMMON and ROW exist (full joins)
    ELSIF COMMON_FLAG > 0 AND ROW_COUNT > 0 THEN
        OPEN P_RESULT FOR
        SELECT
                    'Valid Data: DT - Account exist' AS message,

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
            DI.COMMON_ID = TRIM('20906662')
            AND CT.CLIENT_CODE = CT.MAIN_CODE
            AND ROWNUM = 1;

    -- Scenario 3: COMMON and INV exist 
    ELSIF COMMON_FLAG > 0 AND INV_FLAG > 0 THEN
        OPEN P_RESULT FOR
        SELECT
                    'Valid Data: IM exist' AS message,

            NVL(IM.PAN, NULL) AS CLIENT_PAN,
            NVL(IM.INV_CODE, NULL) AS CLIENT_CODEKYC,
            NVL(EM.PAYROLL_ID, '') AS BUSINESS_CODE,
            NVL(SUBSTR(IM.INV_CODE,1,8), 0) AS SOURCE_CODE,
            NVL(IM.STATUS, NULL) AS STATUS,
            NVL(IM.REL_ID, NULL) AS OCC_ID,
            NVL(IM.STATUS, NULL) AS CT_STATUS,
            NVL(IM.INVESTOR_TITLE, NULL) AS TITLE,
            NVL(IM.INVESTOR_NAME, '') AS CLIENT_NAME,
             TRIM(P_CLIENT_CODE) AS COMMON_ID,
            TRIM(P_CLIENT_CODE) AS DOC_ID,
            '' AS CLIENT_CODE,
            '' AS MAIN_CODE,
            '' AS APPROVED,
            '' AS APPROVED_FLAG,
            '' AS CM_CATEGORY_ID,
            '' AS CT_ACT_CAT,
            '' AS TITLE_OTHER,
            '' AS OTHER,
            '' AS OTHERS1,
            '' AS GENDER,
            '' AS MARITAL_STATUS,
            '' AS NATIONALITY,
            '' AS RESIDENT_NRI,
            '' AS DOB,
            '' AS ANNUAL_INCOME,
            '' AS CLIENT_PAN,
            '' AS LEAD_TYPE,
            '' AS G_NAME,
            '' AS G_NATIONALITY,
            '' AS G_PAN,
            '' AS ADD1,
            '' AS ADD2,
            '' AS CITY_ID,
            '' AS STATE_ID,
            '' AS PINCODE,
            '' AS PER_ADD1,
            '' AS PER_ADD2,
            '' AS PER_CITY_ID,
            '' AS PER_STATE_ID,
            '' AS PER_PINCODE,
            '' AS FAX,
            '' AS AADHAR_CARD_NO,
            '' AS EMAIL,
            '' AS OFFICE_EMAIL,
            '' AS STD1,
            '' AS TEL1,
            '' AS STD2,
            '' AS TEL2,
            '' AS MOBILE_NO,
            '' AS REF_NAME1,
            '' AS REF_NAME2,
            '' AS REF_NAME3,
            '' AS REF_NAME4,
            '' AS REF_MOB1,
            '' AS REF_MOB2,
            '' AS REF_MOB3,
            '' AS REF_MOB4
        FROM 
            INVESTOR_MASTER IM
            LEFT JOIN TB_DOC_UPLOAD DI ON DI.INV_CODE = IM.INV_CODE 
            LEFT JOIN EMPLOYEE_MASTER EM ON EM.RM_CODE = IM.RM_CODE
        WHERE
            DI.COMMON_ID = TRIM(P_CLIENT_CODE)
            AND ROWNUM = 1;

    -- Scenario 4: COMMON and CT_COUNT exist
    ELSIF COMMON_FLAG > 0 AND CT_COUNT > 0 THEN
        OPEN P_RESULT FOR
        SELECT
                        'Valid Data: DT  (IM, CM, CT NOT EXIST) - Open New Account' AS message,

            CT.*,
            DI.COMMON_ID AS DOC_ID,
            CT.CLIENT_CODE,
            CT.MAIN_CODE,
            CT.CLIENT_NAME,
            CT.CLIENT_CODEKYC,
            CT.STATUS AS CT_STATUS,
            CT.ACT_CAT AS CT_ACT_CAT
        FROM 
            CLIENT_TEST CT
            LEFT JOIN TB_DOC_UPLOAD DI ON DI.INV_CODE = CT.CLIENT_CODEKYC
        WHERE 
            DI.COMMON_ID = TRIM(P_CLIENT_CODE)
            AND ROWNUM = 1;

    -- If no conditions match

    END IF;

EXCEPTION
    WHEN OTHERS THEN
        OPEN P_RESULT FOR
        SELECT 
            '' AS DOC_ID,
            '' AS COMMON_ID,
            '' AS CLIENT_CODE
        FROM DUAL;
END PSM_AO_GET_CLIENTBYDT;


