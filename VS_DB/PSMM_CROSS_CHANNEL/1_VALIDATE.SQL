CREATE OR REPLACE PROCEDURE PSMM_CROSS_CH_VALIDATE (
    P_FRM_TYPE      VARCHAR2,
    P_COMMON_ID     VARCHAR2,
    P_SUB_CLIENT_CD VARCHAR2,
    P_LOGIN_ID      VARCHAR2,
    P_CNT           OUT INTEGER
) AS
   VPAN           WEALTHMAKER.INVESTOR_MASTER.PAN%TYPE;
   VMOBILE        VARCHAR2(20);
   VEMAIL         WEALTHMAKER.INVESTOR_MASTER.EMAIL%TYPE;
   VCNT           NUMBER(5) := 0;
   VCNTGROUPID    NUMBER(5) := 0;
   VLASTCLIENT    WEALTHMAKER.INVESTOR_MASTER.INV_CODE%TYPE;
   VCNTUNALOCATED NUMBER(2) := 0;
   VGROUPID       WEALTHMAKER.SUBBROKER_GRP_MAPPING.GROUP_ID%TYPE;
   ISVALIDPAN     VARCHAR2(10);
   ISVALIDMOBILE  NUMBER(1);
   ISVALIDEMAIL   NUMBER(1);
   VCNTMAININFO   NUMBER(2) := 0;
BEGIN
   IF P_SUB_CLIENT_CD LIKE '4%' THEN   /* no validation for client */
      P_CNT := 0;
      RETURN;
   END IF;
   DELETE FROM WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO
    WHERE COMMON_ID = P_COMMON_ID
      AND LOGGEDUSERID = P_LOGIN_ID;

   COMMIT;
   SELECT UPPER(TRIM(SUBSTR(
      PAN,
      1,
      10
   ))),
          MOBILE,
          UPPER(EMAIL),
          VALIDATEPAN(UPPER(TRIM(SUBSTR(
             PAN,
             1,
             10
          )))),
          VALIDATE_MOBILE(MOBILE),
          VALIDATE_EMAIL(UPPER(EMAIL))
     INTO
      VPAN,
      VMOBILE,
      VEMAIL,
      ISVALIDPAN,
      ISVALIDMOBILE,
      ISVALIDEMAIL
     FROM WEALTHMAKER.INVESTOR_MASTER
    WHERE INV_CODE = P_SUB_CLIENT_CD;

   IF
      ISVALIDPAN = 'InValid'
      AND ISVALIDMOBILE = 0
      AND ISVALIDEMAIL = 0
   THEN
      P_CNT := 0;
      RETURN;
   END IF;

          ----------------------------------------------------------------------------------INSERT ALL INFO ------------------------------------------------------------------------------------------
   IF ISVALIDPAN = 'Valid' THEN
      INSERT INTO WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO (
         COMMON_ID,
         SEARCH_KEY,
         INV_CODE,
         CLIENT_NAME,
         RM_NAME,
         PAN,
         EMAIL,
         MOBILE,
         BRANCH_NAME,
         ZONE_NAME,
         REGION_NAME,
         CHANNEL,
         LOGGEDUSERID
      )
         SELECT P_COMMON_ID,
                'PAN',
                CLIENT_CODEKYC,
                CLIENT_NAME,
                RM_NAME,
                CLIENT_PAN,
                EMAIL,
                MOBILE_NO,
                BRANCH_NAME,
                ZONE_NAME,
                REGION_NAME,
                (
                   SELECT ITEMNAME
                     FROM WEALTHMAKER.FIXEDITEM
                    WHERE ITEMSERIALNUMBER IN (
                      SELECT BRANCH_TAR_CAT
                        FROM WEALTHMAKER.BRANCH_MASTER
                       WHERE BRANCH_CODE = T.BRANCH_CODE
                   )
                ) ITEMNAME,
                P_LOGIN_ID
           FROM WEALTHMAKER.TEMP_CLIENT_DETAILS@MF.BAJAJCAPITAL T
          WHERE CLIENT_PAN = VPAN
            AND SUBSTR(
            CLIENT_CODEKYC,
            1,
            8
         ) <> SUBSTR(
            P_SUB_CLIENT_CD,
            1,
            8
         )
            AND LENGTH(CLIENT_CODEKYC) >= 11;
   END IF;

   VCNTMAININFO := 0;
   SELECT COUNT(*)
     INTO VCNTMAININFO
     FROM WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO
    WHERE COMMON_ID = P_COMMON_ID;

   IF
      ISVALIDMOBILE = 1
      AND ISVALIDEMAIL = 1
      AND VCNTMAININFO = 0
   THEN
      INSERT INTO WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO (
         COMMON_ID,
         SEARCH_KEY,
         INV_CODE,
         CLIENT_NAME,
         RM_NAME,
         PAN,
         EMAIL,
         MOBILE,
         BRANCH_NAME,
         ZONE_NAME,
         REGION_NAME,
         CHANNEL,
         LOGGEDUSERID
      )
         SELECT P_COMMON_ID,
                'MOBILE AND EMAIL',
                CLIENT_CODEKYC,
                CLIENT_NAME,
                RM_NAME,
                CLIENT_PAN,
                EMAIL,
                MOBILE_NO,
                BRANCH_NAME,
                ZONE_NAME,
                REGION_NAME,
                (
                   SELECT ITEMNAME
                     FROM WEALTHMAKER.FIXEDITEM
                    WHERE ITEMSERIALNUMBER IN (
                      SELECT BRANCH_TAR_CAT
                        FROM WEALTHMAKER.BRANCH_MASTER
                       WHERE BRANCH_CODE = T.BRANCH_CODE
                   )
                ) ITEMNAME,
                P_LOGIN_ID
           FROM WEALTHMAKER.TEMP_CLIENT_DETAILS@MF.BAJAJCAPITAL T
          WHERE MOBILE_NO = VMOBILE
            AND EMAIL = VEMAIL
            AND SUBSTR(
            CLIENT_CODEKYC,
            1,
            8
         ) <> SUBSTR(
            P_SUB_CLIENT_CD,
            1,
            8
         )
            AND LENGTH(CLIENT_CODEKYC) >= 11;
   END IF;

   VCNTMAININFO := 0;
   SELECT COUNT(*)
     INTO VCNTMAININFO
     FROM WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO
    WHERE COMMON_ID = P_COMMON_ID;

   IF
      ISVALIDMOBILE = 1
      AND VCNTMAININFO = 0
   THEN
      INSERT INTO WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO (
         COMMON_ID,
         SEARCH_KEY,
         INV_CODE,
         CLIENT_NAME,
         RM_NAME,
         PAN,
         EMAIL,
         MOBILE,
         BRANCH_NAME,
         ZONE_NAME,
         REGION_NAME,
         CHANNEL,
         LOGGEDUSERID
      )
         SELECT P_COMMON_ID,
                'MOBILE',
                CLIENT_CODEKYC,
                CLIENT_NAME,
                RM_NAME,
                CLIENT_PAN,
                EMAIL,
                MOBILE_NO,
                BRANCH_NAME,
                ZONE_NAME,
                REGION_NAME,
                (
                   SELECT ITEMNAME
                     FROM WEALTHMAKER.FIXEDITEM
                    WHERE ITEMSERIALNUMBER IN (
                      SELECT BRANCH_TAR_CAT
                        FROM WEALTHMAKER.BRANCH_MASTER
                       WHERE BRANCH_CODE = T.BRANCH_CODE
                   )
                ) ITEMNAME,
                P_LOGIN_ID
           FROM WEALTHMAKER.TEMP_CLIENT_DETAILS@MF.BAJAJCAPITAL T
          WHERE MOBILE_NO = VMOBILE
            AND SUBSTR(
            CLIENT_CODEKYC,
            1,
            8
         ) <> SUBSTR(
            P_SUB_CLIENT_CD,
            1,
            8
         )
            AND LENGTH(CLIENT_CODEKYC) >= 11;
   END IF;


          /* FLAGGING OF THOSE CASES WHICH IS APPROVED */
   UPDATE WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO T
      SET
      APPROVED = WEALTHMAKER.FN_GET_TRF_APPROVAL(
         P_COMMON_ID,
         P_SUB_CLIENT_CD,
         T.INV_CODE
      )
    WHERE COMMON_ID = P_COMMON_ID
      AND LOGGEDUSERID = P_LOGIN_ID;

   COMMIT;
   SELECT COUNT(*)
     INTO VCNT
     FROM WEALTHMAKER.CROSS_CHANNEL_INVESTOR_INFO
    WHERE COMMON_ID = P_COMMON_ID
      AND LOGGEDUSERID = P_LOGIN_ID;
   P_CNT := NVL(
      VCNT,
      0
   );
END;