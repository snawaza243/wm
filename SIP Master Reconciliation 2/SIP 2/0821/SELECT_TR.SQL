/* Formatted on 20-08-2025 18:34:14 (QP5 v5.336) */
-- x=2

  SELECT
  mf.REMARK_RECO REMARK,
   inv.Investor_Name,
            inv.address1
         || ','
         || inv.address2
         || ','
         || inv.phone
         || ','
         || inv.email                   address,
         ct.city_name,
         mf.bank_name,
         mf.client_code,
         mf.sch_code                    sch_code,
         mf.mut_code,
         rm_name,
         branch_name,
         panno,
         amc.mut_name                   mut_Name,
         Sch_Name                       Sch_Name,
         tr_date,
         TRAN_TYPE,
         App_No,
         folio_no,
         payment_mode,
         cheque_no,
         CHEQUE_DATE,
         Amount,
         Sip_Amount,
         Sip_Type,
         lEAD_nO,
         LEAD_NAME,
         TRAN_code,
         b.branch_code,
         BUSINESS_RMCODE,
         mf.INSTALLMENTS_NO,
         RETURNSTATUS (MF.TRAN_CODE)    STATUS,
         CASE
             WHEN mf.loggeduserid = 'MFONLINE' THEN 'Online'
             WHEN mf.loggeduserid = 'Valuefy' THEN 'Online'
             ELSE 'Offline'
         END                            loggeduserid
         ,SIP_FR
    FROM city_master         ct,
         employee_master     e,
         investor_master     inv,
         branch_master       b,
         ALLCOMPANY          amc,
         ALLSCHEME           sch,
         TRANSACTION_MF_TEMP1 mf
   WHERE     1=1--MF.SIP_TYPE = 'SIP'
         AND AMOUNT > 0
         AND (MF.ASA <> 'C' OR MF.ASA IS NULL)
         AND move_flag1 IS NULL
         AND sip_id IS NULL
         AND inv.city_id = ct.city_id(+)
         AND mf.client_code = inv.inv_code
         AND TO_CHAR (mf.BUSINESS_RMCODE) = TO_CHAR (e.payroll_id)
         AND mf.BUSI_BRANCH_CODE = b.branch_code
         AND mf.mut_code = amc.mut_code
         AND mf.sch_code = sch.sch_code
         AND b.branch_code IN (SELECT BRANCH_ID
                                 FROM USERDETAILS_JI
                                WHERE LOGIN_ID = '112650' AND ROLE_ID = '29')
         AND (SIP_FR = 'R')
         AND NVL (mf.rec_flag, 'N') = 'N'
         AND TR_DATE >= TO_DATE ('01/06/2025', 'DD/MM/YYYY')
         --AND TR_DATE <= TO_DATE ('20/08/2025', 'DD/MM/YYYY')
        --AND TRAN_CODE = 'TR24142915'
ORDER BY TR_DATE, rm_name