/* Formatted on 02/08/2025 13:50:19 (QP5 v5.336) */
  SELECT E.RM_CODE RMCODE,
         MF.BROKER_ID,
         MF.SIP_AMOUNT,
         MF.REMARK,
         MF.SOURCE_CODE,
         MF.DISPATCH,
         WEALTHMAKER.FN_IDENTIFY_REGISTRAR@MF.BAJAJCAPITAL (MF.MUT_CODE)
             REGISTRAR,
         NVL (MF.COB_FLAG, 0)
             AS COB_FLAG,
         INV.INVESTOR_NAME,
            INV.ADDRESS1
         || ','
         || INV.ADDRESS2
         || ','
         || INV.PHONE
         || ','
         || INV.EMAIL
             ADDRESS1,
         CT.CITY_NAME,
         MF.BANK_NAME,
         MF.CLIENT_CODE,
         MF.SCH_CODE
             SCH_CODE,
         MF.MUT_CODE,
         RM_NAME,
         BRANCH_NAME,
         PANNO,
         AMC.MUT_NAME
             MUT_NAME,
         SCH_NAME
             SCH_NAME,
         TR_DATE,
         TRAN_TYPE,
         APP_NO,
         FOLIO_NO,
         PAYMENT_MODE,
         CHEQUE_NO,
         CHEQUE_DATE,
         AMOUNT,
         LEAD_NO,
         LEAD_NAME,
         TRAN_CODE,
         B.BRANCH_CODE,
         BUSINESS_RMCODE,
         MF.TRAN_TYPE
             TRAN_TYPE,
         MF.SIP_TYPE
             SIP_TYPE,
         CASE
             WHEN MF.LOGGEDUSERID = 'MFONLINE' THEN 'ONLINE'
             WHEN MF.LOGGEDUSERID = 'VALUEFY' THEN 'ONLINE'
             ELSE 'OFFLINE'
         END
             FLAG
    FROM CITY_MASTER         CT,
         EMPLOYEE_MASTER     E,
         INVESTOR_MASTER     INV,
         BRANCH_MASTER       B,
         ALLCOMPANY          AMC,
         ALLSCHEME           SCH,
         TRANSACTION_MF_TEMP1 MF
   WHERE     MF.MOVE_FLAG1 IS NULL
         AND MF.SIP_ID IS NULL
         AND TRAN_TYPE NOT IN ('REVERTAL')
         AND (MF.ASA <> 'C' OR MF.ASA IS NULL)
         AND INV.CITY_ID = CT.CITY_ID(+)
         AND MF.CLIENT_CODE = INV.INV_CODE
         AND TO_CHAR (MF.BUSINESS_RMCODE) = TO_CHAR (E.PAYROLL_ID)
         AND MF.BUSI_BRANCH_CODE = B.BRANCH_CODE
         AND MF.MUT_CODE = AMC.MUT_CODE
         AND MF.SCH_CODE = SCH.SCH_CODE
         AND B.BRANCH_CODE IN (SELECT BRANCH_ID
                                 FROM USERDETAILS_JI
                                WHERE LOGIN_ID = '112650' AND ROLE_ID = '29')
         AND (   MF.SIP_TYPE = 'REGULAR'
              OR MF.SIP_TYPE IS NULL
              OR MF.SIP_TYPE = 'STP')
         AND (MF.REC_FLAG = 'N' OR MF.REC_FLAG IS NULL)
         AND TR_DATE >= TO_DATE ('06-01-2025', 'DD-MM-YYYY')
         AND TR_DATE <= TO_DATE ('06-10-2025', 'DD-MM-YYYY')
ORDER BY TR_DATE