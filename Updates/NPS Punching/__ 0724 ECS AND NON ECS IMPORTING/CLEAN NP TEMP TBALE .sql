CREATE OR REPLACE PROCEDURE PSM_NPS_NES_TEMP_CLEAN AS
BEGIN
    -- Step 1: Validate and clean data in temp table
    UPDATE PSM_NPS_NES_TBL_TEMP
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
        
        -- Clean user IDs
        LOGGEDUSERID = SUBSTR(TRIM(LOGGEDUSERID), 1, 10),
        MODIFY_USER = SUBSTR(TRIM(MODIFY_USER), 1, 10),
        
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
        
        -- Clean transaction IDs
        TPSL_TRANID = SUBSTR(TRIM(TPSL_TRANID), 1, 20),
        CONSUMER_CODE = SUBSTR(TRIM(CONSUMER_CODE), 1, 20),
        
        -- Set IMPORT_DT to current date
        IMPORT_DT = TO_CHAR(SYSDATE, 'DD-MON-YYYY')
    WHERE ROWNUM > 0;  -- Ensures all rows are processed
    
    -- Step 2: Insert cleaned data into actual table
    INSERT INTO nps_nonecs_tbl_imp (
        REF_TRAN_CODE,
        TR_DATE,
        ECS_AMT,
        ECS_PERIOD,
        ECS_PAY_DT,
        ECS_TRAN_CODE,
        LOGGEDUSERID,
        TIMEST,
        MODIFY_USER,
        MODIFY_DATE,
        TPSL_TRANID,
        CONSUMER_CODE,
        IMPORT_DT
    )
    SELECT 
        REF_TRAN_CODE,
        TO_DATE(TR_DATE, 'DD-MON-YYYY'),
        TO_NUMBER(ECS_AMT),
        ECS_PERIOD,
        TO_DATE(ECS_PAY_DT, 'DD-MON-YYYY'),
        ECS_TRAN_CODE,
        LOGGEDUSERID,
        TO_DATE(TIMEST, 'DD-MON-YYYY'),
        MODIFY_USER,
        TO_DATE(MODIFY_DATE, 'DD-MON-YYYY'),
        TPSL_TRANID,
        CONSUMER_CODE,
        TO_DATE(IMPORT_DT, 'DD-MON-YYYY')
    FROM PSM_NPS_NES_TBL_TEMP;
    
    -- Step 3: Clear temp table (optional)
    DELETE FROM PSM_NPS_NES_TBL_TEMP;
    
    COMMIT;
    
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20001, 'Error in clean_and_update_nps_nonecs: ' || SQLERRM);
END PSM_NPS_NES_TEMP_CLEAN;
/