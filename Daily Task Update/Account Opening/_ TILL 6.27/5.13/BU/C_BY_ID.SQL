CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_GET_CLIENT_BY_ID(
    P_CLIENT_CODE      IN  VARCHAR2,
    P_RESULT           OUT SYS_REFCURSOR
) AS


ISINCT  NUMBER :=0; -- IS ACCOUNT IN CLIENT_TEST MEANS ACCOUNT EXIST OF CLIENT
ISINIM  NUMBER :=0; -- IS ACCOUNT IN INVESTOR_MASTER MEANS ACCOUNT DOES NOT EXIST OF CLIENT


BEGIN
    SELECT COUNT(CLIENT_CODE) INTO ISINCT
    FROM CLIENT_TEST WHERE CLIENT_CODEKYC = TRIM(P_CLIENT_CODE);


    IF (TRIM(P_CLIENT_CODE)) IS NOT NULL THEN

        IF ISINCT >0 THEN
            OPEN P_RESULT FOR
               SELECT
                    'Valid Data: Client Exist' AS MESSAGE,
                    ''                        AS DOC_ID,  
                    NVL(CT.CLIENT_CODE, '')   AS CLIENT_CODE,
                    NVL(CT.MAIN_CODE,'')      AS MAIN_CODE,
                    IM.PAN                    AS CLIENT_PAN,
                    IM.INV_CODE               AS CLIENT_CODEKYC,
                    IM.SOURCE_ID              AS SOURCE_CODE,
                    NVL(CT.APPROVED,'')       AS APPROVED,
                    NVL(CT.APPROVED_FLAG, '') AS APPROVED_FLAG,
                    CT.BUSINESS_CODE/*EM.PAYROLL_ID*/             AS BUSINESS_CODE,
                    EM.RM_NAME                AS RM_NAME,
                    NVL(BM.BRANCH_NAME,'')    AS BRANCH_CODE ,
                    NVL(CM.GUEST_CD, '')               AS GUEST_CODE,
                    NVL(CT.STATUS, '')                 AS STATUS, -         -- TAXT STATUS      FROM CT
                    IM.OCCUPATION_ID                   AS OCC_ID,           -- OCCUPATION       FROM IM
                    IM.OCCUPATION_ID                   AS IM_OCC_ID,            
                    NVL(CM.INVESTOR_CODE, '')          AS CT_STATUS,          -- CATEGORY STATUS  FORM CM
                    CM.CATEGORY_ID                     AS CM_CATEGORY_ID,            -- CLIENT CATEGORY  FORM CM
                    NVL(CT.ACT_CAT, '')                AS CT_ACT_CAT,                -- A/C CATEGORY     FROM CT
                    im.INVESTOR_TITLE                  AS TITLE,
                    im.INVESTOR_NAME                   AS CLIENT_NAME,
                    NVL(cm.TITLE_FATHER_SPOUSE, '')    AS TITLE_OTHER,
                    NVL(cm.FATHER_SPOUSE_NAME, '')     AS OTHER,
                    NVL(CT.OTHERS1, '')                AS OTHERS1,
                    IM.GENDER                          AS GENDER,
                    CT.MARITAL_STATUS                  AS MARITAL_STATUS,
                    NVL(CT.NATIONALITY, '')   AS NATIONALITY,
                    NVL(CT.RESIDENT_NRI, '')  AS RESIDENT_NRI,
                    IM.DOB                    AS DOB,
                    NVL(CT.ANNUAL_INCOME, '') AS ANNUAL_INCOME,
                    CM.LEAD_SOURCE            AS LEAD_TYPE,
                    CT.G_NAME                 AS G_NAME,
                    NVL(CT.G_NATIONALITY, '') AS G_NATIONALITY,
                    CT.G_PAN                  AS G_PAN,
                    IM.ADDRESS1               AS ADD1,
                    IM.ADDRESS2               AS ADD2,
                    NVL(CT.STATE_ID, '')      AS STATE_ID,
                    IM.PINCODE                AS PINCODE,
                    IM.CITY_ID                AS CITY_ID,
                    CM.PER_ADD1               AS PER_ADD1,
                    CM.PER_ADD2               AS PER_ADD2,
                    CM.PER_STATE_ID           AS PER_STATE_ID,
                    CM.PER_CITY_ID            AS PER_CITY_ID,
                    CM.PER_PINCODE            AS PER_PINCODE,
                    NVL(CT.OVERSEAS_ADD, '')  AS OVERSEAS_ADD,
                    CT.AADHAR_CARD_NO         AS AADHAR_CARD_NO,                
                    /*CASE
                        WHEN CT.AADHAR_CARD_NO IS NOT NULL AND LENGTH(CT.AADHAR_CARD_NO) > 12 THEN
                            ( SELECT NVL(WEALTHMAKER.USER_UNLOG(CT.AADHAR_CARD_NO), NULL) FROM DUAL )
                    END                         AS AADHAR_CARD_NO,    
                    */          
                    LOWER(CT.EMAIL)           AS EMAIL,
                    CM.FAX                    AS FAX,
                    LOWER(CM.OFFICE_EMAIL)    AS OFFICE_EMAIL,
                    CM.PHONE1                 AS TEL1,
                    CM.PHONE2                 AS TEL2,
                    CM.STD1                   AS STD1,
                    CM.STD2                   AS STD2,
                    IM.MOBILE                 AS MOBILE_NO,
                    CM.REF_NAME1              AS REF_NAME1,
                    CM.REF_NAME2              AS REF_NAME2,
                    CM.REF_NAME3              AS REF_NAME3,
                    CM.REF_NAME4              AS REF_NAME4,
                    CM.REF_MOB1               AS REF_MOB1,
                    CM.REF_MOB2               AS REF_MOB2,
                    CM.REF_MOB3               AS REF_MOB3,
                    CM.REF_MOB4               AS REF_MOB4,

                    -- Mailing Address Details
                    CIM_MAIL.CITY_ID          AS MAILING_CITY,
                    STM_MAIL.STATE_ID         AS MAILING_STATE,
                    CTM_MAIL.COUNTRY_ID       AS MAILING_COUNTRY,

                    -- Permanent Address Details
                    CIM_PERM.CITY_ID          AS PERMANENT_CITY,
                    STM_PERM.STATE_ID         AS PERMANENT_STATE,
                    CTM_PERM.COUNTRY_ID       AS PERMANENT_COUNTRY

                FROM
                INVESTOR_MASTER IM
                LEFT JOIN CLIENT_MASTER   CM ON CM.CLIENT_CODE = SUBSTR(INV_CODE, 1, 8)
                LEFT JOIN EMPLOYEE_MASTER EM ON EM.RM_CODE = IM.RM_CODE
                LEFT JOIN CLIENT_TEST     CT ON IM.INV_CODE = CT.CLIENT_CODEKYC
                LEFT JOIN TB_DOC_UPLOAD   DI ON DI.INV_CODE = TO_CHAR(CT.CLIENT_CODEKYC)
                LEFT JOIN BRANCH_MASTER   BM ON BM.BRANCH_CODE = EM.SOURCE

                -- Mailing Address Joins
                LEFT JOIN CITY_MASTER CIM_MAIL ON CIM_MAIL.CITY_ID = IM.CITY_ID
                LEFT JOIN STATE_MASTER STM_MAIL ON STM_MAIL.STATE_ID = CIM_MAIL.STATE_ID
                LEFT JOIN COUNTRY_MASTER CTM_MAIL ON CTM_MAIL.COUNTRY_ID = STM_MAIL.COUNTRY_ID

                -- Permanent Address Joins
                LEFT JOIN CITY_MASTER CIM_PERM ON CIM_PERM.CITY_ID = CT.PER_CITY_ID
                LEFT JOIN STATE_MASTER STM_PERM ON STM_PERM.STATE_ID = CT.PER_STATE_ID
                LEFT JOIN COUNTRY_MASTER CTM_PERM ON CTM_PERM.COUNTRY_ID = STM_PERM.COUNTRY_ID

                WHERE CT.SOURCE_CODE = SUBSTR(P_CLIENT_CODE,1,8) AND CT.CLIENT_CODE = CT.MAIN_CODE
                AND ROWNUM = 1;
            RETURN;
           
        ELSE
            OPEN P_RESULT FOR
               SELECT
                    'Valid Data: Client Exist' AS MESSAGE,
                    ''                        AS DOC_ID,  
                    NVL(CT.CLIENT_CODE, '')   AS CLIENT_CODE,
                    NVL(CT.MAIN_CODE,'')      AS MAIN_CODE,
                    IM.PAN                    AS CLIENT_PAN,
                    IM.INV_CODE               AS CLIENT_CODEKYC,
                    IM.SOURCE_ID              AS SOURCE_CODE,
                    NVL(CT.APPROVED,'')       AS APPROVED,
                    NVL(CT.APPROVED_FLAG, '') AS APPROVED_FLAG,
                    EM.PAYROLL_ID             AS BUSINESS_CODE,
                    EM.RM_NAME                AS RM_NAME,
                    NVL(BM.BRANCH_NAME,'')    AS BRANCH_CODE ,
                    NVL(CM.GUEST_CD, '')               AS GUEST_CODE,
                    NVL(CT.STATUS, '')                 AS STATUS, -         -- TAXT STATUS      FROM CT
                    IM.OCCUPATION_ID                   AS OCC_ID,           -- OCCUPATION       FROM IM
                    IM.OCCUPATION_ID                   AS IM_OCC_ID,            
                    NVL(CM.INVESTOR_CODE, '')          AS CT_STATUS,          -- CATEGORY STATUS  FORM CM
                    CM.CATEGORY_ID                     AS CM_CATEGORY_ID,            -- CLIENT CATEGORY  FORM CM
                    NVL(CT.ACT_CAT, '')                AS CT_ACT_CAT,                -- A/C CATEGORY     FROM CT
                    im.INVESTOR_TITLE                  AS TITLE,
                    im.INVESTOR_NAME                   AS CLIENT_NAME,
                    NVL(cm.TITLE_FATHER_SPOUSE, '')    AS TITLE_OTHER,
                    NVL(cm.FATHER_SPOUSE_NAME, '')     AS OTHER,
                    NVL(CT.OTHERS1, '')                AS OTHERS1,
                    IM.GENDER                          AS GENDER,
                    CT.MARITAL_STATUS                  AS MARITAL_STATUS,
                    NVL(CT.NATIONALITY, '')   AS NATIONALITY,
                    NVL(CT.RESIDENT_NRI, '')  AS RESIDENT_NRI,
                    IM.DOB                    AS DOB,
                    NVL(CT.ANNUAL_INCOME, '') AS ANNUAL_INCOME,
                    CM.LEAD_SOURCE            AS LEAD_TYPE,
                    CT.G_NAME                 AS G_NAME,
                    NVL(CT.G_NATIONALITY, '') AS G_NATIONALITY,
                    CT.G_PAN                  AS G_PAN,
                    IM.ADDRESS1               AS ADD1,
                    IM.ADDRESS2               AS ADD2,
                    NVL(CT.STATE_ID, '')      AS STATE_ID,
                    IM.PINCODE                AS PINCODE,
                    IM.CITY_ID                AS CITY_ID,
                    CM.PER_ADD1               AS PER_ADD1,
                    CM.PER_ADD2               AS PER_ADD2,
                    CM.PER_STATE_ID           AS PER_STATE_ID,
                    CM.PER_CITY_ID            AS PER_CITY_ID,
                    CM.PER_PINCODE            AS PER_PINCODE,
                    NVL(CT.OVERSEAS_ADD, '')  AS OVERSEAS_ADD,
                    CT.AADHAR_CARD_NO         AS AADHAR_CARD_NO,
                    /*CASE
                        WHEN CT.AADHAR_CARD_NO IS NOT NULL AND LENGTH(CT.AADHAR_CARD_NO) > 12 THEN
                            ( SELECT NVL(WEALTHMAKER.USER_UNLOG(CT.AADHAR_CARD_NO), NULL) FROM DUAL )
                    END                         AS AADHAR_CARD_NO,
                    */
                    LOWER(CT.EMAIL)           AS EMAIL,
                    CM.FAX                    AS FAX,
                    LOWER(CM.OFFICE_EMAIL)    AS OFFICE_EMAIL,
                    CM.PHONE1                 AS TEL1,
                    CM.PHONE2                 AS TEL2,
                    CM.STD1                   AS STD1,
                    CM.STD2                   AS STD2,
                    IM.MOBILE                 AS MOBILE_NO,
                    CM.REF_NAME1              AS REF_NAME1,
                    CM.REF_NAME2              AS REF_NAME2,
                    CM.REF_NAME3              AS REF_NAME3,
                    CM.REF_NAME4              AS REF_NAME4,
                    CM.REF_MOB1               AS REF_MOB1,
                    CM.REF_MOB2               AS REF_MOB2,
                    CM.REF_MOB3               AS REF_MOB3,
                    CM.REF_MOB4               AS REF_MOB4,

                    -- Mailing Address Details
                    CIM_MAIL.CITY_ID          AS MAILING_CITY,
                    STM_MAIL.STATE_ID         AS MAILING_STATE,
                    CTM_MAIL.COUNTRY_ID       AS MAILING_COUNTRY,

                    -- Permanent Address Details
                    CIM_PERM.CITY_ID          AS PERMANENT_CITY,
                    STM_PERM.STATE_ID         AS PERMANENT_STATE,
                    CTM_PERM.COUNTRY_ID       AS PERMANENT_COUNTRY

                FROM
                INVESTOR_MASTER IM
                LEFT JOIN CLIENT_MASTER   CM ON CM.CLIENT_CODE = SUBSTR(INV_CODE, 1, 8)
                LEFT JOIN EMPLOYEE_MASTER EM ON EM.RM_CODE = IM.RM_CODE
                LEFT JOIN CLIENT_TEST     CT ON IM.INV_CODE = CT.CLIENT_CODEKYC
                LEFT JOIN TB_DOC_UPLOAD   DI ON DI.INV_CODE = TO_CHAR(CT.CLIENT_CODEKYC)
                LEFT JOIN BRANCH_MASTER   BM ON BM.BRANCH_CODE = EM.SOURCE

                -- Mailing Address Joins
                LEFT JOIN CITY_MASTER CIM_MAIL ON CIM_MAIL.CITY_ID = IM.CITY_ID
                LEFT JOIN STATE_MASTER STM_MAIL ON STM_MAIL.STATE_ID = CIM_MAIL.STATE_ID
                LEFT JOIN COUNTRY_MASTER CTM_MAIL ON CTM_MAIL.COUNTRY_ID = STM_MAIL.COUNTRY_ID

                -- Permanent Address Joins
                LEFT JOIN CITY_MASTER CIM_PERM ON CIM_PERM.CITY_ID = CM.PER_CITY_ID
                LEFT JOIN STATE_MASTER STM_PERM ON STM_PERM.STATE_ID = CM.PER_STATE_ID
                LEFT JOIN COUNTRY_MASTER CTM_PERM ON CTM_PERM.COUNTRY_ID = STM_PERM.COUNTRY_ID

                WHERE IM.INV_CODE = trim(P_CLIENT_CODE)
                AND ROWNUM = 1
                ;
            RETURN;

        END IF;

    END IF;
END;
/
