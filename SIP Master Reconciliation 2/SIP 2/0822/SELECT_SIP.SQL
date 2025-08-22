       SELECT A.SEQ_NO,
            A.MUT_CODE,
            B.MUT_NAME,
            A.SCH_CODE,
            C.SCH_NAME,
            A.FOLIO_NO,
            IHNO_TRXNNO TRXN_NO,
            START_DATE     SIP_START_DATE,
            END_DATE       SIP_END_DATE,
            SIPREGDATE SIP_REG_DATE,
            SIP_OPTION,
            AMOUNT_SIP,
            TOTAL_SIP,
            RM_NAME,
            PAYROLL_ID,
            BRANCH_CODE,
            BRANCH_NAME
            FROM TRAN_SIP_FEED    A,
            MUT_FUND         B,
            SCHEME_INFO      C,
            EMPLOYEE_MASTER  E,
            BRANCH_MASTER    BR
            WHERE     A.MUT_CODE = B.MUT_CODE
            AND A.SCH_CODE = C.SCH_CODE
            AND BASE_TRAN_CODE IS NULL
            AND A.CLIENT_RM_CODE = E.RM_CODE
            AND A.CLIENT_BRANCH_CODE = BR.BRANCH_CODE
            AND ok_flag = 'OK'
            --AND A.AMOUNT_SIP = '2000'
            --AND ( UPPER (A.pan) = 'HVOPK6703G') 
            AND START_DATE BETWEEN ADD_MONTHS ( TO_DATE ('01/07/2025', 'DD/MM/RRRR'), -2) AND ADD_MONTHS ( TO_DATE ('01/07/2025', 'DD/MM/RRRR'), 2);
        END;