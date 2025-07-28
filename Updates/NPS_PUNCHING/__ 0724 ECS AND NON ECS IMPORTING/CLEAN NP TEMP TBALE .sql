CREATE OR REPLACE PROCEDURE PSM_NPS_NES_TEMP_CLEAN (
P_LOG_ID IN VARCHAR2)

AS
BEGIN
    -- Step 1: Validate and clean data in temp table
    UPDATE PSM_NPS_NES_TBL_TEMP1
    SET 
        -- Clean REF_TRAN_CODE (remove special chars, trim spaces)
        REF_TRAN_CODE = REGEXP_REPLACE(TRIM(REF_TRAN_CODE), '[^a-zA-Z0-9]', ''),
        
        -- Convert and validate dates
        TR_DATE = CASE 
                    WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-[A-Z]{3}-\d{4}$') THEN TRIM(TR_DATE)
                    WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}/\d{2}/\d{4}$') THEN 
                        TO_CHAR(TO_DATE(TRIM(TR_DATE), 'DD/MM/YYYY'), 'DD-MON-YYYY')
                    ELSE NULL
                 END,
        
        -- Validate numeric amount
        ECS_AMT = CASE 
                    WHEN REGEXP_LIKE(TRIM(ECS_AMT), '^\d+(\.\d+)?$') THEN TRIM(ECS_AMT)
                    ELSE '0'
                  END,
        
        -- Clean ECS_PERIOD
        ECS_PERIOD = SUBSTR(TRIM(ECS_PERIOD), 1, 10),
        
        -- Convert and validate ECS_PAY_DT
        ECS_PAY_DT = CASE 
                       WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-[A-Z]{3}-\d{4}$') THEN TRIM(ECS_PAY_DT)
                       WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}/\d{2}/\d{4}$') THEN 
                           TO_CHAR(TO_DATE(TRIM(ECS_PAY_DT), 'DD/MM/YYYY'), 'DD-MON-YYYY')
                       ELSE NULL
                     END,
        
        -- Clean ECS_TRAN_CODE
        ECS_TRAN_CODE = SUBSTR(TRIM(ECS_TRAN_CODE), 1, 12),       
       
        
        -- Convert and validate TIMEST
        TIMEST = CASE 
                   WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-[A-Z]{3}-\d{4}') THEN TRIM(TIMEST)
                   ELSE TO_CHAR(SYSDATE, 'DD-MON-YYYY')
                 END,
        
        -- Convert and validate MODIFY_DATE
        MODIFY_DATE = CASE 
                         WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-[A-Z]{3}-\d{4}$') THEN TRIM(MODIFY_DATE)
                         ELSE NULL
                       END,
                
        -- Set IMPORT_DT to current date
        IMPORT_DT = TO_CHAR(SYSDATE, 'DD-MON-YYYY')
    WHERE ROWNUM > 0;  -- Ensures all rows are processed
    
    
    COMMIT;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20001, 'Error in clean_and_update_nps_nonecs: ' || SQLERRM);
END PSM_NPS_NES_TEMP_CLEAN;
/