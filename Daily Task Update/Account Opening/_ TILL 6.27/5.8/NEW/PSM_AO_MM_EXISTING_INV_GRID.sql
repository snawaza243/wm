CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_AO_MM_EXISTING_INV_GRID (
    P_SOURCE_CODE IN CLIENT_TEST.SOURCE_CODE%TYPE,
    P_EXIST       IN CLIENT_TEST.CLIENT_CODEKYC%TYPE,
    P_CURSOR      OUT SYS_REFCURSOR   
) AS
BEGIN
    -- Open a cursor to return the result of the select query
    OPEN P_CURSOR FOR
        SELECT 
            CT.CLIENT_CODE      AS AH_CLIENT_CODE,
            CT.MAIN_CODE        AS AH_MAIN_CODE,
            CT.CLIENT_CODEKYC   AS INVESTORCODE,

            UPPER(CT.TITLE)     AS INVESTOR_TITLE,
            UPPER(CT.CLIENT_NAME)      AS INVESTOR_NAME,
            CASE 
                WHEN UPPER(SUBSTR(CT.GENDER, 1, 1)) = 'F' THEN 'FEMALE'
                WHEN UPPER(SUBSTR(CT.GENDER, 1, 1)) = 'M' THEN 'MALE'
                WHEN UPPER(SUBSTR(CT.GENDER, 1, 1)) = 'O' THEN 'OTHER'
                WHEN UPPER(SUBSTR(CT.GENDER, 1, 1)) = 'N' THEN 'OTHER'
                ELSE ''
            END                 AS GENDER,  -- Fixed missing comma
            CT.MOBILE_NO        AS MOBILE,   
            CT.EMAIL            AS EMAIL,    
            CT.DOB              AS DOB,
            CT.CLIENT_PAN       AS PAN,
            NVL((SELECT RELATION_NAME FROM RELATIONSHIP_MASTER WHERE RELATION_ID = CT.RELATION_ID), '')                                                 AS OUR_RELATIONSHIP,
            CT.RELATION_ID                                                                                                                              AS OUR_RELATIONSHIP_ID,
            CT.OCC_ID                                                                                                                                   AS OCC_ID,
            NVL((SELECT OCC_NAME FROM OCCUPATION_MASTER WHERE OCC_ID = CT.OCC_ID), '')                                                                  AS OCC_NAME,
            CASE WHEN UPPER(SUBSTR(CT.INV_KYC, 1, 1)) = 'Y'  THEN 'YES'  WHEN UPPER(SUBSTR(CT.KYC_STATUS, 1, 1)) = 'N'   THEN 'NO'   ELSE '' END        AS KYC,
            CT.G_NAME                                                                                                                                   AS G_NAME,
            CT.G_PAN                                                                                                                                    AS G_PAN,
            CASE WHEN UPPER(SUBSTR(CT.APPROVED, 1, 1)) = 'Y'    THEN 'YES'  WHEN UPPER(SUBSTR(CT.APPROVED, 1, 1)) = 'N'     THEN 'NO'   ELSE '' END     AS APPROVED,                     
            CT.AADHAR_CARD_NO                                                                                                                           AS AADHAR_CARD_NO,
            CASE WHEN UPPER(SUBSTR(CT.IS_NOMINEE, 1, 1)) = 'Y'  THEN 'YES'  WHEN UPPER(SUBSTR(CT.IS_NOMINEE, 1, 1)) = 'N'   THEN 'NO'   ELSE '' END     AS IS_NOMINEE,                     
            CT.NOMINEE_PER                                                                                                                              AS NOMINEE_PER        
        FROM CLIENT_TEST CT
        WHERE CT.SOURCE_CODE = P_SOURCE_CODE
          AND CT.CLIENT_CODE != CT.MAIN_CODE
        ORDER BY CT.CLIENT_NAME;
END;
/
