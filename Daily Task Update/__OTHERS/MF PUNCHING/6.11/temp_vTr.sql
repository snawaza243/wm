SELECT 
    b.BRANCH_NAME, tmf.TRAN_CODE,  tmf.INVESTOR_NAME,  tmf.PANNO,  tmf.SCH_CODE, tmf.switch_scheme, ssi.sch_name as sw_sch_name,  COALESCE(si.SCH_NAME, op.osch_name) AS SCH_NAME, tmf.MUT_CODE, ai.MUT_NAME, tmf.R_SCHEMECODE,  tmf.LEAD_NAME, tmf.CLIENT_CODE, 
    tmf.AC_HOLDER_CODE, tmf.TR_DATE,  tmf.TRAN_TYPE,  tmf.APP_NO, 
    tmf.PAYMENT_MODE, tmf.CHEQUE_NO, tmf.CHEQUE_DATE, tmf.AMOUNT, tmf.SIP_TYPE, tmf.LEAD_NO, tmf.BANK_NAME, tmf.RMCODE, tmf.R_AMCCODE,tmf.FOLIO_NO, tmf.DOC_ID, tmf.INVESTOR_TYPE, tmf.SWITCH_SCHEME, tmf.TARGET_SWITCH_SCHEME, tmf.remark,tmf.sip_end_date,tmf.frequency,tmf.installments_no,tmf.BASE_TRAN_CODE, tmf.BUSINESS_RMCODE, tmf.GOAL, im.MOBILE, im.INV_CODE, im.ADDRESS1, im.ADDRESS2, im.EMAIL, im.DOB, im.PINCODE, im.city_id, c.CITY_NAME, im.AADHAR_CARD_NO, im.PAN, stm.country_id
    FROM transaction_mf_temp1 tmf
    LEFT JOIN  scheme_info si ON tmf.SCH_CODE = si.SCH_CODE
    LEFT JOIN scheme_info ssi ON tmf.switch_scheme = ssi.SCH_CODE
    LEFT JOIN MUT_FUND ai ON tmf.MUT_CODE = ai.MUT_CODE
    LEFT JOIN CLIENT_MASTER ci ON tmf.PANNO = ci.PAN
    LEFT JOIN AGENT_MASTER xi ON tmf.SOURCE_CODE = xi.AGENT_CODE
    LEFT JOIN INVESTOR_MASTER im ON tmf.CLIENT_CODE = im.INV_CODE
    LEFT JOIN City_master c ON im.city_id = c.city_id
    LEFT JOIN STATE_MASTER stm ON c.state_id = stm.state_id
    LEFT JOIN Branch_master b ON tmf.busi_branch_code = b.branch_code
    LEFT JOIN other_product op ON tmf.SCH_CODE = op.osch_code
    WHERE 1 = 1
    AND tmf.TRAN_CODE  is not null