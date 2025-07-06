CREATE OR REPLACE PROCEDURE PSM_DAP_BULK1(
    P_FILENAME      VARCHAR2,
    P_SHEETNAME     VARCHAR2,
    P_MONTH         VARCHAR2,
    P_YEAR          VARCHAR2,
    P_DDLIMPORTTYPE VARCHAR2, 
    P_CHKTYPE       VARCHAR2,
    P_DATACOUNT     VARCHAR2,
    P_LOG_ID        VARCHAR2,
    P_ROLE_ID       VARCHAR2,
    P_CURSOR        OUT SYS_REFCURSOR,
    P_COMMIT_FLAG   BOOLEAN DEFAULT TRUE -- Added to control transaction commits
) AS
    V_MyImportDatType   VARCHAR2(100);
    V_MYIMPORT          VARCHAR2(10);    
    v_total_records     NUMBER;
    v_inserted_records  NUMBER;
    v_updated_records   NUMBER;
    v_error_msg         VARCHAR2(4000);
    v_procedure_name    VARCHAR2(30) := 'PSM_DAP_BULK1';
    v_start_time        TIMESTAMP := SYSTIMESTAMP;
    
    -- Custom exceptions
    e_invalid_parameter EXCEPTION;
    e_data_processing  EXCEPTION;
    PRAGMA EXCEPTION_INIT(e_invalid_parameter, -20001);
    PRAGMA EXCEPTION_INIT(e_data_processing, -20002);
BEGIN
    -- Log procedure start
    INSERT INTO PSM_PROCEDURE_LOG 
    (LOG_ID, PROCEDURE_NAME, START_TIME, PARAMETERS, STATUS, MESSAGE)
    VALUES 
    (P_LOG_ID, v_procedure_name, v_start_time, 
     'FILENAME='||P_FILENAME||', SHEETNAME='||P_SHEETNAME||', MONTH='||P_MONTH||
     ', YEAR='||P_YEAR||', IMPORTTYPE='||P_DDLIMPORTTYPE||', CHKTYPE='||P_CHKTYPE,
     'STARTED', NULL);
     
    -- Parameter validation
    IF P_CHKTYPE NOT IN ('DUE') THEN
        v_error_msg := 'Invalid P_CHKTYPE: '||P_CHKTYPE||'. Only ''DUE'' is supported.';
        RAISE e_invalid_parameter;
    END IF;
    
    IF P_DDLIMPORTTYPE NOT IN ('DUEDATA', 'LAPSEDDATA') THEN
        v_error_msg := 'Invalid P_DDLIMPORTTYPE: '||P_DDLIMPORTTYPE||'. Must be ''DUEDATA'' or ''LAPSEDDATA''.';
        RAISE e_invalid_parameter;
    END IF;
    
    IF NOT (TO_NUMBER(P_MONTH) BETWEEN 1 AND 12) THEN
        v_error_msg := 'Invalid P_MONTH: '||P_MONTH||'. Must be between 1 and 12.';
        RAISE e_invalid_parameter;
    END IF;
    
    IF TO_NUMBER(P_YEAR) < 2000 OR TO_NUMBER(P_YEAR) > 2100 THEN
        v_error_msg := 'Invalid P_YEAR: '||P_YEAR||'. Must be between 2000 and 2100.';
        RAISE e_invalid_parameter;
    END IF;

    -- Main processing
    IF P_CHKTYPE = 'DUE' THEN
        -- STEP 0: BULK INSERTION IN PSM_DAP_TEMP_DUE1 FROM EXCEL DATA DONE FROM CLIENT SIDE
        
        -- STEP 1: CLEAN THE BULK IMPORTED DATA IN TEMP TABLE
        BEGIN
            CLEAN_PSM_DUE1;
            
            -- Get count of records in temp table
            SELECT COUNT(*) INTO v_total_records 
            FROM PSM_DAP_TEMP_DUE1;
            
            -- Log cleaning completion
            INSERT INTO PSM_PROCEDURE_LOG 
            (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
            VALUES 
            (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
             'Cleaned temporary data in PSM_DAP_TEMP_DUE1', v_total_records);
             
            IF v_total_records = 0 THEN
                v_error_msg := 'No records found in PSM_DAP_TEMP_DUE1 after cleaning.';
                RAISE e_data_processing;
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                v_error_msg := 'Error in CLEAN_PSM_DUE1: '||SQLERRM;
                RAISE e_data_processing;
        END;
        
        -- Step 2: Bulk Insert new data in BAJAJ_DUE_DATA, avoiding duplicates
        BEGIN
            INSERT /*+ APPEND PARALLEL(4) */ INTO BAJAJ_DUE_DATA (
                POLICY_NO, COMPANY_CD, STATUS_CD, LOCATION, CL_NAME, PREM_AMT, 
                PAY_MODE, SA, DUE_DATE, CL_ADD1, CL_ADD2, CL_ADD3, CL_ADD4, 
                CL_ADD5, CL_CITY, CL_PIN, CL_PHONE1, CL_PHONE2, CL_MOBILE, 
                PLAN_NAME, DOC, PREM_FREQ, PLY_TERM, PPT, NET_AMOUNT, 
                MON_NO, YEAR_NO, USERID, IMPORT_DT, IMPORTDATATYPE, NEWINSERT
            )
            SELECT /*+ PARALLEL(4) */
                D.POLICY_NO, D.COMPANY_CD, D.STATUS_CD, D.LOCATION, D.CL_NAME,  
                CAST(D.PREM_AMT AS NUMBER(14,2)), 
                D.PAY_MODE, CAST(D.PREM_AMT AS NUMBER(14,2)), 
                TO_DATE(D.DUE_DATE, 'DD-MM-YYYY'), 
                D.CL_ADD1, D.CL_ADD2, D.CL_ADD3, D.CL_ADD4, D.CL_ADD5, 
                D.CL_CITY, D.CL_PIN, D.CL_PHONE1, D.CL_PHONE2, D.CL_MOBILE, 
                D.PLAN_NAME, TO_DATE(D.DOC, 'DD-MM-YYYY'), D.PREM_FREQ, 
                D.PLY_TERM, D.PPT, CAST(D.NET_AMOUNT AS NUMBER(14,2)),
                TO_NUMBER(P_MONTH), TO_NUMBER(P_YEAR), P_LOG_ID, SYSDATE, 
                P_DDLIMPORTTYPE, 'BAL'
            FROM PSM_DAP_TEMP_DUE1 D
            WHERE NOT EXISTS (
                SELECT /*+ HASH_SJ */ 1 
                FROM BAJAJ_DUE_DATA B
                WHERE UPPER(TRIM(B.POLICY_NO)) = UPPER(TRIM(D.POLICY_NO))
                  AND UPPER(TRIM(B.COMPANY_CD)) = UPPER(TRIM(D.COMPANY_CD))
                  AND B.MON_NO = TO_NUMBER(P_MONTH)
                  AND B.YEAR_NO = TO_NUMBER(P_YEAR)
                  AND B.IMPORTDATATYPE = P_DDLIMPORTTYPE
            );
            
            v_inserted_records := SQL%ROWCOUNT;
            
            -- Log insertion
            INSERT INTO PSM_PROCEDURE_LOG 
            (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
            VALUES 
            (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
             'Inserted records into BAJAJ_DUE_DATA', v_inserted_records);
             
            IF P_COMMIT_FLAG THEN
                COMMIT;
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                v_error_msg := 'Error inserting into BAJAJ_DUE_DATA: '||SQLERRM;
                RAISE e_data_processing;
        END;
        
        -- Update POLICY_DETAILS_MASTER for DUEDATA imports
        IF P_DDLIMPORTTYPE = 'DUEDATA' THEN 
            BEGIN
                MERGE INTO policy_details_master p
                USING (
                    SELECT 
                        UPPER(TRIM(policy_no)) as policy_no,
                        UPPER(TRIM(company_cd)) as company_cd,
                        MAX(pay_mode) as pay_mode,
                        MAX(prem_freq) as prem_freq,
                        MAX(doc) as doc,
                        MAX(ply_term) as ply_term,
                        MAX(cl_mobile) as mobile,
                        MAX(sa) as sa,
                        MAX(prem_amt) as prem_amt
                    FROM psm_dap_temp_due1
                    GROUP BY UPPER(TRIM(policy_no)), UPPER(TRIM(company_cd))
                ) d
                ON (
                    UPPER(TRIM(p.policy_no)) = d.policy_no
                    AND UPPER(TRIM(p.company_cd)) = d.company_cd
                )
                WHEN MATCHED THEN UPDATE SET
                    p.file_name = 'DuePaidMappedField' || P_DDLIMPORTTYPE || '.txt',
                    p.payment_mode = d.pay_mode,
                    p.update_prog = P_DDLIMPORTTYPE,
                    p.update_user = P_LOG_ID,
                    p.update_date = TRUNC(SYSDATE),
                    p.prem_freq = NVL(d.prem_freq, p.prem_freq),
                    p.doc = NVL(d.doc, p.doc),
                    p.ply_term = NVL(d.ply_term, p.ply_term),
                    p.mobile = NVL(d.mobile, p.mobile),
                    p.sa = NVL(d.sa, p.sa),
                    p.prem_amt = NVL(d.prem_amt, p.prem_amt);
                    
                v_updated_records := SQL%ROWCOUNT;
                
                -- Log update
                INSERT INTO PSM_PROCEDURE_LOG 
                (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
                VALUES 
                (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
                 'Updated records in policy_details_master', v_updated_records);
                 
                IF P_COMMIT_FLAG THEN
                    COMMIT;
                END IF;
            EXCEPTION
                WHEN OTHERS THEN
                    v_error_msg := 'Error updating policy_details_master: '||SQLERRM;
                    RAISE e_data_processing;
            END;
        END IF;
        
        -- LAPSEDDATA processing
        IF P_DDLIMPORTTYPE = 'LAPSEDDATA' THEN
            BEGIN -- STEP 3: BULK INSERT LAPSED DATA
                INSERT INTO psm_dap_temp_due1_lap
                (policy_no, company_cd, due_date, mon_no, year_no, status_cd, days_lapsed)
                SELECT 
                    d.policy_no,
                    d.company_cd,
                    MAX(a.due_date),
                    MAX(a.mon_no),
                    MAX(a.year_no),
                    (
                        SELECT MAX(b.status_cd)
                        FROM bajaj_due_data b
                        WHERE b.policy_no = d.policy_no
                        AND b.company_cd = d.company_cd
                        AND b.importdatatype = 'DUEDATA'
                        AND b.due_date = (SELECT MAX(c.due_date) 
                                         FROM bajaj_due_data c 
                                         JOIN psm_dap_temp_due1 e ON e.policy_no = c.policy_no
                                         WHERE UPPER(TRIM(c.policy_no)) = UPPER(TRIM(e.policy_no)) 
                                         AND UPPER(TRIM(c.company_cd)) = UPPER(TRIM(e.company_cd))
                                         AND c.due_date IS NOT NULL 
                                         AND c.importdatatype = 'DUEDATA'                    
                        )
                    ) as status_cd,
                    TRUNC(SYSDATE) - MAX(a.due_date)
                FROM 
                    psm_dap_temp_due1 d
                JOIN 
                    bajaj_due_data a ON a.policy_no = d.policy_no
                                     AND a.company_cd = d.company_cd
                WHERE 
                    a.importdatatype = 'DUEDATA'
                    AND a.due_date <= TRUNC(SYSDATE)
                GROUP BY 
                    d.policy_no, 
                    d.company_cd;

                v_inserted_records := SQL%ROWCOUNT;
                
                -- Log insertion
                INSERT INTO PSM_PROCEDURE_LOG 
                (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
                VALUES 
                (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
                 'Inserted lapsed records into psm_dap_temp_due1_lap', v_inserted_records);
                 
                IF P_COMMIT_FLAG THEN
                    COMMIT;
                END IF;
            EXCEPTION
                WHEN OTHERS THEN
                    v_error_msg := 'Error inserting into psm_dap_temp_due1_lap: '||SQLERRM;
                    RAISE e_data_processing;
            END;

            BEGIN -- UPDATE BAJAJ_DUE_DATA FROM LAPSED DATA            
                MERGE /*+ PARALLEL(4) */ INTO bajaj_due_data b
                USING (
                    SELECT 
                        UPPER(TRIM(policy_no)) as policy_no,
                        UPPER(TRIM(company_cd)) as company_cd,
                        due_date
                    FROM psm_dap_temp_due1_lap
                    WHERE due_date <= TRUNC(SYSDATE)
                ) d
                ON (
                    UPPER(TRIM(b.policy_no)) = d.policy_no
                    AND UPPER(TRIM(b.company_cd)) = d.company_cd
                    AND b.due_date = d.due_date
                    AND b.importdatatype = 'DUEDATA'
                )
                WHEN MATCHED THEN UPDATE SET
                    b.status_cd = 'LAPSED',
                    b.last_update_dt = TRUNC(SYSDATE),
                    b.last_update = P_LOG_ID;
                    
                v_updated_records := SQL%ROWCOUNT;
                
                -- Log update
                INSERT INTO PSM_PROCEDURE_LOG 
                (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
                VALUES 
                (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
                 'Updated lapsed records in bajaj_due_data', v_updated_records);
                 
                IF P_COMMIT_FLAG THEN
                    COMMIT;
                END IF;
            EXCEPTION
                WHEN OTHERS THEN
                    v_error_msg := 'Error updating bajaj_due_data: '||SQLERRM;
                    RAISE e_data_processing;
            END;
                        
            BEGIN -- UPDATE POLICY_DETAILS_MASTER FROM LAPSED DATA
                MERGE /*+ PARALLEL(4) */ INTO policy_details_master p
                USING (
                    SELECT 
                        UPPER(TRIM(policy_no)) as policy_no,
                        UPPER(TRIM(company_cd)) as company_cd
                    FROM psm_dap_temp_due1_lap
                ) d
                ON (
                    UPPER(TRIM(p.policy_no)) = d.policy_no
                    AND UPPER(TRIM(p.company_cd)) = d.company_cd
                )
                WHEN MATCHED THEN UPDATE SET
                    p.last_status = 'L',
                    p.update_prog = P_DDLIMPORTTYPE,
                    p.update_user = P_LOG_ID,
                    p.update_date = TRUNC(sysdate);

                v_updated_records := SQL%ROWCOUNT;
                
                -- Log update
                INSERT INTO PSM_PROCEDURE_LOG 
                (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
                VALUES 
                (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
                 'Updated lapsed records in policy_details_master', v_updated_records);
                 
                IF P_COMMIT_FLAG THEN
                    COMMIT;
                END IF;
            EXCEPTION
                WHEN OTHERS THEN
                    v_error_msg := 'Error updating policy_details_master: '||SQLERRM;
                    RAISE e_data_processing;
            END;

            BEGIN
                -- Update BAJAJ_AR_HEAD status in bulk
                UPDATE BAJAJ_AR_HEAD h
                SET status_cd = 'L'
                WHERE EXISTS (
                    SELECT 1 
                    FROM psm_dap_temp_due1 d
                    WHERE h.POLICY_NO = d.policy_no
                    AND h.COMPANY_CD = d.company_cd
                    AND TO_CHAR(h.SYS_AR_DT,'MON-YYYY') = TO_CHAR(P_MONTH|| '-' ||P_YEAR)
                );

                v_updated_records := SQL%ROWCOUNT;
                
                -- Log update
                INSERT INTO PSM_PROCEDURE_LOG 
                (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
                VALUES 
                (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
                 'Updated lapsed records in BAJAJ_AR_HEAD', v_updated_records);

                -- Bulk insert into BAJAJ_AR_DETAILS for updated records
                INSERT INTO BAJAJ_AR_DETAILS
                (SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON)
                SELECT 
                    h.SYS_AR_NO,
                    LAST_DAY(h.SYS_AR_DT),
                    'L',
                    'LAPSED DATA ' || TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),
                    h.SYS_AR_DT,
                    SYSDATE
                FROM 
                    BAJAJ_AR_HEAD h
                JOIN 
                    psm_dap_temp_due1 d ON h.POLICY_NO = d.policy_no AND h.COMPANY_CD = d.company_cd
                WHERE 
                    TO_CHAR(h.SYS_AR_DT,'MON-YYYY') = TO_CHAR(P_MONTH|| '-' || P_YEAR)
                    AND h.status_cd = 'L'
                    AND NOT EXISTS (
                        SELECT 1 
                        FROM BAJAJ_AR_DETAILS d2
                        WHERE d2.SYS_AR_NO = h.SYS_AR_NO
                        AND d2.STATUS_DT = LAST_DAY(h.SYS_AR_DT)
                    );

                v_inserted_records := SQL%ROWCOUNT;
                
                -- Log insertion
                INSERT INTO PSM_PROCEDURE_LOG 
                (LOG_ID, PROCEDURE_NAME, EVENT_TIME, MESSAGE, RECORD_COUNT)
                VALUES 
                (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
                 'Inserted records into BAJAJ_AR_DETAILS', v_inserted_records);
                 
                IF P_COMMIT_FLAG THEN
                    COMMIT;
                END IF;
            EXCEPTION
                WHEN OTHERS THEN
                    v_error_msg := 'Error processing BAJAJ_AR_HEAD/BAJAJ_AR_DETAILS: '||SQLERRM;
                    RAISE e_data_processing;
            END;
        END IF; -- CLOSE LAPSED IN DUE            
    END IF;
    
    -- Log successful completion
    INSERT INTO PSM_PROCEDURE_LOG 
    (LOG_ID, PROCEDURE_NAME, END_TIME, STATUS, MESSAGE)
    VALUES 
    (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
     'COMPLETED', 'Procedure completed successfully');
     
    -- Return success message
    OPEN P_CURSOR FOR 
    SELECT 'Procedure completed successfully. ' ||
           'Inserted: ' || NVL(v_inserted_records, 0) || 
           ', Updated: ' || NVL(v_updated_records, 0) || 
           ', Total: ' || NVL(v_total_records, 0) as MESSAGE
    FROM DUAL;
    
    IF P_COMMIT_FLAG THEN
        COMMIT;
    END IF;
EXCEPTION
    WHEN e_invalid_parameter THEN
        -- Log error
        INSERT INTO PSM_PROCEDURE_LOG 
        (LOG_ID, PROCEDURE_NAME, END_TIME, STATUS, MESSAGE)
        VALUES 
        (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
         'ERROR', 'Invalid parameter: ' || v_error_msg);
         
        -- Return error
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || v_error_msg as MESSAGE FROM DUAL;
        
        IF P_COMMIT_FLAG THEN
            COMMIT;
        END IF;
        
        RAISE;
        
    WHEN e_data_processing THEN
        -- Log error
        INSERT INTO PSM_PROCEDURE_LOG 
        (LOG_ID, PROCEDURE_NAME, END_TIME, STATUS, MESSAGE)
        VALUES 
        (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
         'ERROR', 'Data processing error: ' || v_error_msg);
         
        -- Return error
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || v_error_msg as MESSAGE FROM DUAL;
        
        IF P_COMMIT_FLAG THEN
            COMMIT;
        END IF;
        
        RAISE;
        
    WHEN OTHERS THEN
        v_error_msg := 'Unexpected error: ' || SQLERRM;
        
        -- Log error
        INSERT INTO PSM_PROCEDURE_LOG 
        (LOG_ID, PROCEDURE_NAME, END_TIME, STATUS, MESSAGE)
        VALUES 
        (P_LOG_ID, v_procedure_name, SYSTIMESTAMP, 
         'ERROR', v_error_msg);
         
        -- Return error
        OPEN P_CURSOR FOR 
        SELECT 'ERROR: ' || v_error_msg as MESSAGE FROM DUAL;
        
        IF P_COMMIT_FLAG THEN
            COMMIT;
        END IF;
        
        RAISE;
END PSM_DAP_BULK1;
/