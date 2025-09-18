CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSMM_CROSS_CH_GET_DATA ( 
    P_COMMONID     VARCHAR2,
    P_LOG          VARCHAR2,
    P_INVCODE      VARCHAR2,
    P_CURSOR       OUT SYS_REFCURSOR
)
AS
BEGIN

    OPEN P_CURSOR FOR 
        SELECT 
            INV_CODE                AS INV_CODE, 
            INITCAP(CLIENT_NAME)    AS CLIENT_NAME, 
            INITCAP(RM_NAME)        AS RM_NAME, 
            UPPER(PAN)              AS PAN, 
            INITCAP(EMAIL)          AS EMAIL, 
            MOBILE                  AS MOBILE, 
            INITCAP(BRANCH_NAME)    AS BRANCH_NAME, 
            INITCAP(ZONE_NAME)      AS ZONE_NAME, 
            INITCAP(REGION_NAME)    AS REGION_NAME,
            INITCAP(CHANNEL)        AS CHANNEL, 
            LOGGEDUSERID            AS REMARK,
        --    CASE 
        --        WHEN APPROVED = '1' THEN 'This has been approved by Management' 
        --    else null end REMARK, 
            approved                AS APPROVED
            FROM WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO 
            WHERE COMMON_ID = P_COMMONID AND LOGGEDUSERID = P_LOG AND LENGTH(INV_CODE) >= 11 
            ORDER BY SUBSTR(INV_CODE,1,8);
END;
/