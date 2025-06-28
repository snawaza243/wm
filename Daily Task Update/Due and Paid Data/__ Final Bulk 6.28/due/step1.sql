   
   
   -- Step 1: Insert new data in BAJAJ_DUE_DATA, avoiding duplicates from psm_dap_temp_due1
    INSERT /*+ APPEND PARALLEL(4) */ INTO BAJAJ_DUE_DATA (
        policy_no, company_cd, status_cd, location, cl_name, prem_amt, 
        pay_mode, sa, due_date, cl_add1, cl_add2, cl_add3, cl_add4, 
        cl_add5, cl_city, cl_pin, cl_phone1, cl_phone2, cl_mobile, 
        plan_name, doc, prem_freq, ply_term, ppt, net_amount, 
        MON_NO, YEAR_NO, IMPORTDATATYPE, NEWINSERT
    )
    SELECT /*+ PARALLEL(4) */
        d.policy_no, d.company_cd, d.status_cd, d.location, d.cl_name, d.prem_amt, 
        d.pay_mode, d.sa, d.due_date, d.cl_add1, d.cl_add2, d.cl_add3, d.cl_add4, 
        d.cl_add5, d.cl_city, d.cl_pin, d.cl_phone1, d.cl_phone2, d.cl_mobile, 
        d.plan_name, d.doc, d.prem_freq, d.ply_term, d.ppt, d.net_amount, 
        d.MON_NO, d.YEAR_NO, d.IMPORTDATATYPE, d.NEWINSERT
    FROM psm_dap_temp_due1 d
    WHERE NOT EXISTS (
        SELECT /*+ HASH_SJ */ 1 
        FROM BAJAJ_DUE_DATA b
        WHERE UPPER(TRIM(b.POLICY_NO)) = UPPER(TRIM(d.POLICY_NO))
          AND UPPER(TRIM(b.COMPANY_CD)) = UPPER(TRIM(d.COMPANY_CD))
          AND b.MON_NO = d.MON_NO
          AND b.YEAR_NO = d.YEAR_NO
          AND b.IMPORTDATATYPE = d.IMPORTDATATYPE
    );
