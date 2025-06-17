SELECT 
            B.BRANCH_NAME,
            TMF.TRAN_CODE, 
            TMF.INVESTOR_NAME, 
            TMF.PANNO, 
            TMF.SCH_CODE,
            TMF.SWITCH_SCHEME,
            SSI.SCH_NAME AS SW_SCH_NAME,
            COALESCE(SI.SCH_NAME, OP.OSCH_NAME) AS SCH_NAME,
            TMF.MUT_CODE,
            AI.MUT_NAME,
            TMF.R_SCHEMECODE, 
            TMF.LEAD_NAME,
            TMF.CLIENT_CODE,
            TMF.AC_HOLDER_CODE,
            TMF.TR_DATE, 
            TMF.TRAN_TYPE, 
            TMF.APP_NO, 
            TMF.PAYMENT_MODE, 
            TMF.CHEQUE_NO,  
            TO_CHAR(tmf.sip_end_date, 'DD/MM/YYYY') AS sip_end_date, 
            TO_CHAR(tmf.sip_start_date, 'DD/MM/YYYY') AS sip_start_date,
            TMF.CHEQUE_DATE, 
            TMF.AMOUNT, 
            TMF.SIP_TYPE, 
            TMF.LEAD_NO, 
            TMF.BANK_NAME, 
            TMF.RMCODE, 
            TMF.R_AMCCODE,
            TMF.FOLIO_NO, 
            TMF.DOC_ID, 
            TMF.INVESTOR_TYPE, 
            TMF.SWITCH_SCHEME, 
            TMF.TARGET_SWITCH_SCHEME, 
            TMF.REMARK,
            TRUNC(TMF.SIP_END_DATE),
            TRUNC(TMF.SIP_START_DATE),
            TMF.FREQUENCY,
            TMF.INSTALLMENTS_NO,
            TMF.BASE_TRAN_CODE,
            TMF.BUSINESS_RMCODE,
            TMF.GOAL,
            IM.MOBILE,
            IM.INV_CODE,
            IM.ADDRESS1,
            IM.ADDRESS2,
            IM.EMAIL,
            IM.DOB,
            IM.PINCODE,
            IM.CITY_ID,
            C.CITY_NAME,
            IM.AADHAR_CARD_NO,
            IM.PAN,
            STM.COUNTRY_ID
        FROM 
            TRANSACTION_MF_TEMP1 TMF
        LEFT JOIN 
            SCHEME_INFO SI ON TMF.SCH_CODE = SI.SCH_CODE
        LEFT JOIN 
            SCHEME_INFO SSI ON TMF.SWITCH_SCHEME = SSI.SCH_CODE
        LEFT JOIN 
            MUT_FUND AI ON TMF.MUT_CODE = AI.MUT_CODE
        LEFT JOIN 
            CLIENT_MASTER CI ON TMF.PANNO = CI.PAN
        LEFT JOIN 
            AGENT_MASTER XI ON TMF.SOURCE_CODE = XI.AGENT_CODE
        LEFT JOIN 
            INVESTOR_MASTER IM ON TMF.CLIENT_CODE = IM.INV_CODE
        LEFT JOIN 
            CITY_MASTER C ON IM.CITY_ID = C.CITY_ID
        LEFT JOIN 
            STATE_MASTER STM ON C.STATE_ID = STM.STATE_ID
        LEFT JOIN
            BRANCH_MASTER B ON TMF.BUSI_BRANCH_CODE = B.BRANCH_CODE
        LEFT JOIN
            OTHER_PRODUCT OP ON TMF.SCH_CODE = OP.OSCH_CODE
        WHERE 1 = 1 AND TMF.TR_DATE >= TO_DATE('2017-09-26', 'YYYY-MM-DD')  AND TMF.TR_DATE <= TO_DATE('2017-10-28', 'YYYY-MM-DD')  ORDER BY TMF.TRAN_CODE
