CREATE OR REPLACE PROCEDURE WEALTHMAKER.CLEAN_PSM_PAID1 AS
BEGIN
    -- Clean date fields: PAID_DATE, DUE_DATE, NEXT_DUE_DATE, DOC
    UPDATE PSM_DAP_TEMP_PAID1
    SET 
        PAID_DATE = CASE 
                      WHEN REGEXP_LIKE(PAID_DATE, '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$') 
                      THEN TO_CHAR(TO_DATE(PAID_DATE, 'FMMM/FMDD/YYYY HH:MI:SS AM'), 'DD-MM-YYYY') 
                      WHEN REGEXP_LIKE(PAID_DATE, '^\d{1,2}-\w{3}-\d{4}$') 
                      THEN PAID_DATE
                      ELSE NULL 
                    END,
        DUE_DATE = CASE 
                     WHEN REGEXP_LIKE(DUE_DATE, '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$') 
                     THEN TO_CHAR(TO_DATE(DUE_DATE, 'FMMM/FMDD/YYYY HH:MI:SS AM'), 'DD-MM-YYYY') 
                     WHEN REGEXP_LIKE(DUE_DATE, '^\d{1,2}-\w{3}-\d{4}$') 
                     THEN DUE_DATE
                     ELSE NULL 
                   END,
        NEXT_DUE_DATE = CASE 
                          WHEN REGEXP_LIKE(NEXT_DUE_DATE, '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$') 
                          THEN TO_CHAR(TO_DATE(NEXT_DUE_DATE, 'FMMM/FMDD/YYYY HH:MI:SS AM'), 'DD-MM-YYYY') 
                          WHEN REGEXP_LIKE(NEXT_DUE_DATE, '^\d{1,2}-\w{3}-\d{4}$') 
                          THEN NEXT_DUE_DATE
                          ELSE NULL 
                        END,
        DOC = CASE 
                 WHEN REGEXP_LIKE(DOC, '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$') 
                 THEN TO_CHAR(TO_DATE(DOC, 'FMMM/FMDD/YYYY HH:MI:SS AM'), 'DD-MM-YYYY') 
                 WHEN REGEXP_LIKE(DOC, '^\d{1,2}-\w{3}-\d{4}$') 
                 THEN DOC
                 ELSE NULL 
              END;

    -- Clean numeric fields: remove commas, validate numeric structure
    UPDATE PSM_DAP_TEMP_PAID1
    SET 
        PREM_AMT = CASE 
                      WHEN REGEXP_LIKE(REPLACE(PREM_AMT, ',', ''), '^\d+(\.\d+)?$') 
                      THEN REPLACE(PREM_AMT, ',', '') 
                      ELSE NULL 
                   END,
        NET_AMOUNT = CASE 
                        WHEN REGEXP_LIKE(REPLACE(NET_AMOUNT, ',', ''), '^\d+(\.\d+)?$') 
                        THEN REPLACE(NET_AMOUNT, ',', '') 
                        ELSE NULL 
                     END;

    -- Clean text fields: trim and standardize case
    UPDATE PSM_DAP_TEMP_PAID1
    SET 
        POLICY_NO = UPPER(TRIM(POLICY_NO)),
        COMPANY_CD = UPPER(TRIM(COMPANY_CD)) ;

    
                    
                    
END;
/
