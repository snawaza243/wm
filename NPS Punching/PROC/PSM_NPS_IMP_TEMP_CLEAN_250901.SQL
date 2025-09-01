CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_NPS_IMP_TEMP_CLEAN (
    P_TYPE IN VARCHAR2
)
AS
BEGIN

    IF P_TYPE = 'ECS' THEN

       UPDATE PSM_NPS_NES_TBL_TEMP2
           SET TR_DATE = CASE
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-[A-Z]{3}-\d{4}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD-MON-YYYY')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD-MON-RR HH:MI:SS AM')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}/\d{2}/\d{4}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD/MM/YYYY')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-\d{2}-\d{4}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD-MM-YYYY')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{4}/\d{2}/\d{2}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'YYYY/MM/DD')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{4}-\d{2}-\d{2}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'YYYY-MM-DD')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                THEN TO_DATE(TRIM(TR_DATE), 'MM/DD/YYYY HH:MI:SS AM')
                            ELSE to_date(TR_DATE, 'dd-mm-yyyy')
                         END,

               ECS_AMT = CASE 
                            WHEN REGEXP_LIKE(TRIM(ECS_AMT), '^\d+(\.\d+)?$')
                                THEN TRIM(ECS_AMT)
                            ELSE '0'
                         END,

               ECS_PERIOD = SUBSTR(TRIM(ECS_PERIOD), 1, 10),

               ECS_PAY_DT = CASE
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-[A-Z]{3}-\d{4}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD-MON-YYYY')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD-MON-RR HH:MI:SS AM')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}/\d{2}/\d{4}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD/MM/YYYY')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-\d{2}-\d{4}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD-MM-YYYY')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{4}/\d{2}/\d{2}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'YYYY/MM/DD')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{4}-\d{2}-\d{2}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'YYYY-MM-DD')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'MM/DD/YYYY HH:MI:SS AM')
                               ELSE to_date(ECS_PAY_DT, 'dd-mm-yyyy')
                            END,

               ECS_TRAN_CODE = SUBSTR(TRIM(ECS_TRAN_CODE), 1, 12),
               LOGGEDUSERID  = SUBSTR(TRIM(LOGGEDUSERID), 1, 10),
               MODIFY_USER   = SUBSTR(TRIM(MODIFY_USER), 1, 10),

               TIMEST = CASE
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-[A-Z]{3}-\d{4}$')
                               THEN TO_DATE(TRIM(TIMEST), 'DD-MON-YYYY')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                               THEN TO_DATE(TRIM(TIMEST), 'DD-MON-RR HH:MI:SS AM')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}/\d{2}/\d{4}$')
                               THEN TO_DATE(TRIM(TIMEST), 'DD/MM/YYYY')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-\d{2}-\d{4}$')
                               THEN TO_DATE(TRIM(TIMEST), 'DD-MM-YYYY')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{4}/\d{2}/\d{2}$')
                               THEN TO_DATE(TRIM(TIMEST), 'YYYY/MM/DD')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{4}-\d{2}-\d{2}$')
                               THEN TO_DATE(TRIM(TIMEST), 'YYYY-MM-DD')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                               THEN TO_DATE(TRIM(TIMEST), 'MM/DD/YYYY HH:MI:SS AM')
                           ELSE to_date(TIMEST, 'dd-mm-yyyy')
                        END,

               MODIFY_DATE = CASE
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-[A-Z]{3}-\d{4}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD-MON-YYYY')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD-MON-RR HH:MI:SS AM')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}/\d{2}/\d{4}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD/MM/YYYY')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-\d{2}-\d{4}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD-MM-YYYY')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{4}/\d{2}/\d{2}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'YYYY/MM/DD')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{4}-\d{2}-\d{2}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'YYYY-MM-DD')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'MM/DD/YYYY HH:MI:SS AM')
                                ELSE to_date(MODIFY_DATE, 'dd-mm-yyyy')
                             END,

               TPSL_TRANID   = SUBSTR(TRIM(TPSL_TRANID), 1, 20),
               CONSUMER_CODE = SUBSTR(TRIM(CONSUMER_CODE), 1, 20),
               IMPORT_DT     = SYSDATE
         WHERE CONSUMER_CODE IS NOT NULL;
   

    ELSIF P_TYPE = 'NON_ECS' THEN
        UPDATE PSM_NPS_NES_TBL_TEMP1
           SET TR_DATE = CASE
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-[A-Z]{3}-\d{4}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD-MON-YYYY')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD-MON-RR HH:MI:SS AM')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}/\d{2}/\d{4}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD/MM/YYYY')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{2}-\d{2}-\d{4}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'DD-MM-YYYY')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{4}/\d{2}/\d{2}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'YYYY/MM/DD')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{4}-\d{2}-\d{2}$')
                                THEN TO_DATE(TRIM(TR_DATE), 'YYYY-MM-DD')
                            WHEN REGEXP_LIKE(TRIM(TR_DATE), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                THEN TO_DATE(TRIM(TR_DATE), 'MM/DD/YYYY HH:MI:SS AM')
                            ELSE to_date(TR_DATE, 'dd-mm-yyyy')
                         END,

               ECS_AMT = CASE 
                            WHEN REGEXP_LIKE(TRIM(ECS_AMT), '^\d+(\.\d+)?$')
                                THEN TRIM(ECS_AMT)
                            ELSE '0'
                         END,

               ECS_PERIOD = SUBSTR(TRIM(ECS_PERIOD), 1, 10),

               ECS_PAY_DT = CASE
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-[A-Z]{3}-\d{4}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD-MON-YYYY')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD-MON-RR HH:MI:SS AM')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}/\d{2}/\d{4}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD/MM/YYYY')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{2}-\d{2}-\d{4}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'DD-MM-YYYY')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{4}/\d{2}/\d{2}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'YYYY/MM/DD')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{4}-\d{2}-\d{2}$')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'YYYY-MM-DD')
                               WHEN REGEXP_LIKE(TRIM(ECS_PAY_DT), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                   THEN TO_DATE(TRIM(ECS_PAY_DT), 'MM/DD/YYYY HH:MI:SS AM')
                               ELSE to_date(ECS_PAY_DT, 'dd-mm-yyyy')
                            END,

               ECS_TRAN_CODE = SUBSTR(TRIM(ECS_TRAN_CODE), 1, 12),
               LOGGEDUSERID  = SUBSTR(TRIM(LOGGEDUSERID), 1, 10),
               MODIFY_USER   = SUBSTR(TRIM(MODIFY_USER), 1, 10),

               TIMEST = CASE
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-[A-Z]{3}-\d{4}$')
                               THEN TO_DATE(TRIM(TIMEST), 'DD-MON-YYYY')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                               THEN TO_DATE(TRIM(TIMEST), 'DD-MON-RR HH:MI:SS AM')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}/\d{2}/\d{4}$')
                               THEN TO_DATE(TRIM(TIMEST), 'DD/MM/YYYY')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{2}-\d{2}-\d{4}$')
                               THEN TO_DATE(TRIM(TIMEST), 'DD-MM-YYYY')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{4}/\d{2}/\d{2}$')
                               THEN TO_DATE(TRIM(TIMEST), 'YYYY/MM/DD')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{4}-\d{2}-\d{2}$')
                               THEN TO_DATE(TRIM(TIMEST), 'YYYY-MM-DD')
                           WHEN REGEXP_LIKE(TRIM(TIMEST), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                               THEN TO_DATE(TRIM(TIMEST), 'MM/DD/YYYY HH:MI:SS AM')
                           ELSE to_date(TIMEST, 'dd-mm-yyyy')
                        END,

               MODIFY_DATE = CASE
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-[A-Z]{3}-\d{4}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD-MON-YYYY')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-[A-Z]{3}-\d{2} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD-MON-RR HH:MI:SS AM')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}/\d{2}/\d{4}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD/MM/YYYY')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{2}-\d{2}-\d{4}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'DD-MM-YYYY')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{4}/\d{2}/\d{2}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'YYYY/MM/DD')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{4}-\d{2}-\d{2}$')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'YYYY-MM-DD')
                                WHEN REGEXP_LIKE(TRIM(MODIFY_DATE), '^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} (AM|PM)$','i')
                                    THEN TO_DATE(TRIM(MODIFY_DATE), 'MM/DD/YYYY HH:MI:SS AM')
                                ELSE to_date(MODIFY_DATE, 'dd-mm-yyyy')
                             END,

               TPSL_TRANID   = SUBSTR(TRIM(TPSL_TRANID), 1, 20),
               CONSUMER_CODE = SUBSTR(TRIM(CONSUMER_CODE), 1, 20),
               IMPORT_DT     = SYSDATE
         WHERE CONSUMER_CODE IS NOT NULL;
    END IF;

    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20001, 'Error in PSM_NPS_NES_TEMP_CLEAN: ' || SQLERRM);
END PSM_NPS_IMP_TEMP_CLEAN;
/
