
  CREATE OR REPLACE FORCE EDITIONABLE VIEW "WEALTHMAKER"."BAJAJ_DUE_DATA_REMINDER" ("AR_BRANCH_CD", "COMPANY_CD", "CLIENT_NAME", "ADDRESS1", "ADDRESS2", "CL_ADD3", "CL_ADD4", "CL_ADD5", "CL_PHONE1", "CL_PHONE2", "CITY_NAME", "DUE_DATE", "REM_FLAGE", "STATE_NAME", "COMPANY_NAME", "FAVOUR_NAME", "BRANCH_NAME", "BRANCH_ADD1", "BRANCH_ADD2", "PLAN_NAME1", "PAY_MODE", "POLICY_NO", "P_NAME", "I_NAME", "PREM_FREQ", "BPREM_FREQ", "PLAN_NAME", "SA", "PREM_AMT", "MON_NO", "YEAR_NO", "CL_PIN", "PIN1", "INV_CODE", "INV_CODE1", "IMPORTDATATYPE") AS 
  SELECT                                                                ar_branch_cd,
                                                            company_cd,
            DECODE (a.p_name, NULL, a.i_name, p_name)               client_name,
            DECODE (p_add1, NULL, iadd1, p_add1) address1,
            DECODE (p_add2, NULL, iadd2, p_add2) address2,
            NULL cl_add3,
            NULL cl_add4,
            NULL cl_add5,
            DECODE (c.phone1, NULL, c.phone2, c.phone1) cl_phone1,
            DECODE (c.mobile, NULL, c.mobile1, c.mobile) cl_phone2,
            p_city city_name,
            due_date,
            rem_flage,
            (SELECT state_name
               FROM state_master
              WHERE state_id = a.state_cd)
               state_name,
            (SELECT company_name
               FROM bajaj_company_master
              WHERE company_cd = a.company_cd)
               company_name,
            (SELECT favour_name
               FROM favour_master
              WHERE company_cd = a.company_cd)
               favour_name,
            (SELECT branch_name
               FROM branch_master
              WHERE branch_code = c.sourceid)
               branch_name,
            (SELECT address1
               FROM branch_master
              WHERE branch_code = c.sourceid)
               branch_add1,
            (SELECT address2
               FROM branch_master
              WHERE branch_code = c.sourceid)
               branch_add2,
            (SELECT PLAN
               FROM bajaj_plan_master
              WHERE plan_no = a.plan_no)
               plan_name1,
            pay_mode,
            policy_no,
            p_name,
            i_name,
            CASE prem_freq
               WHEN '1' THEN 'ANNUALLY'
               WHEN '12' THEN 'MONTHLY'
               WHEN '4' THEN 'QUARTERLY'
               WHEN '2' THEN 'SEMI-ANNUALLY'
            END
               prem_freq,
            bprem_freq,
            plan_name,
            DECODE (
               sa,
               NULL, (SELECT MAX (sa)
                        FROM bajaj_ar_head f
                       WHERE     f.company_cd = a.company_cd
                             AND f.policy_no = a.policy_no),
               sa)
               sa,
            prem_amt,
            mon_no,
            year_no,
            DECODE (DECODE (p_pin, NULL, pincode, p_pin),
                    NULL, ipin,
                    DECODE (p_pin, NULL, pincode, p_pin))
               cl_pin,
            (SELECT MAX (pin)
               FROM bajaj_ar_head
              WHERE     UPPER (TRIM (company_cd)) = UPPER (TRIM (a.company_cd))
                    AND UPPER (TRIM (policy_no)) = UPPER (TRIM (a.policy_no)))
               pin1,
            SUBSTR (inv_cd, 1, 8) inv_code,
            inv_cd AS inv_code1,
            importdatatype
       FROM bajaj_due_data a, client_master c, employee_master e
      WHERE     mon_no = TO_CHAR (SYSDATE + 5, 'mm')
            AND year_no IN ('2018')
            AND UPPER (TRIM (pay_mode)) = 'NON ECS'
            AND c.rm_code = e.rm_code
            AND (   cl_add1 IS NOT NULL
                 OR cl_add2 IS NOT NULL
                 OR cl_add3 IS NOT NULL)
            AND rem_flage IS NULL
            AND e.TYPE = 'A'
            AND (a.p_add1 IS NOT NULL OR cl_add1 IS NOT NULL)
            AND p_city IS NOT NULL
            AND SUBSTR (a.inv_cd, 1, 8) = c.client_code
            AND prem_amt > 0
            --and company_cd='AVIVA'
            AND sourceid IN
                   (SELECT branch_code
                      FROM branch_master
                     WHERE     branch_tar_cat IN (185, 184, 187, 283)
                           AND category_id NOT IN (1004, 1005, 1006))
   ORDER BY cl_pin, client_code
 ;

