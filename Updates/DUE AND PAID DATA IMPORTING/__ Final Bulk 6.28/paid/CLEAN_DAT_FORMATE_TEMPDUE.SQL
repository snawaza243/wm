CREATE OR REPLACE PROCEDURE WEALTHMAKER.CLEAN_PSM_DUE1 AS
BEGIN
    -- Clean date fields: DUE_DATE, DOC
    UPDATE PSM_DAP_TEMP_DUE1
    SET 
        DUE_DATE = CASE 
                     WHEN REGEXP_LIKE(DUE_DATE, '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$') 
                     THEN TO_CHAR(TO_DATE(DUE_DATE, 'FMMM/FMDD/YYYY HH:MI:SS AM'), 'DD-MM-YYYY') 
                     ELSE NULL 
                   END,
        DOC = CASE 
                 WHEN REGEXP_LIKE(DOC, '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$') 
                 THEN TO_CHAR(TO_DATE(DOC, 'FMMM/FMDD/YYYY HH:MI:SS AM'), 'DD-MM-YYYY') 
                 ELSE NULL 
              END;

    -- Clean numeric fields: remove commas, validate numeric structure
    UPDATE PSM_DAP_TEMP_DUE1
    SET 
        PREM_AMT   = CASE 
                        WHEN REGEXP_LIKE(REPLACE(PREM_AMT, ',', ''), '^\d+(\.\d+)?$') 
                        THEN REPLACE(PREM_AMT, ',', '') 
                        ELSE NULL 
                    END,
        SA         = CASE 
                        WHEN REGEXP_LIKE(REPLACE(SA, ',', ''), '^\d+(\.\d+)?$') 
                        THEN REPLACE(SA, ',', '') 
                        ELSE NULL 
                    END,
        NET_AMOUNT = CASE 
                        WHEN REGEXP_LIKE(REPLACE(NET_AMOUNT, ',', ''), '^\d+(\.\d+)?$') 
                        THEN REPLACE(NET_AMOUNT, ',', '') 
                        ELSE NULL 
                    END,
        PLY_TERM   = CASE 
                        WHEN REGEXP_LIKE(PLY_TERM, '^\d+$') 
                        THEN PLY_TERM 
                        ELSE NULL 
                    END,
        PPT        = CASE 
                        WHEN REGEXP_LIKE(PPT, '^\d+$') 
                        THEN PPT 
                        ELSE NULL 
                    END;
END;
/