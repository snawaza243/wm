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
P_CURSOR        OUT SYS_REFCURSOR
) AS

V_MyImportDatType   varchar2(100);
V_MYIMPORT      VARCHAR2(10);

BEGIN

    

    
    IF P_CHKTYPE = 'DUE' THEN
        -- STEP 1: BULK INSERTION IN PSM_DAP_TEMP_DUE1 FROM EXCEL DATA DONE FROM CILENT SIDE
        
        -- Step 2: Bulk Insert new data in BAJAJ_DUE_DATA, avoiding duplicates from psm_dap_temp_due1
        INSERT /*+ APPEND PARALLEL(4) */ INTO BAJAJ_DUE_DATA (
            POLICY_NO, COMPANY_CD, STATUS_CD, LOCATION, CL_NAME, PREM_AMT, 
            PAY_MODE, SA, DUE_DATE, CL_ADD1, CL_ADD2, CL_ADD3, CL_ADD4, 
            CL_ADD5, CL_CITY, CL_PIN, CL_PHONE1, CL_PHONE2, CL_MOBILE, 
            PLAN_NAME, DOC, PREM_FREQ, PLY_TERM, PPT, NET_AMOUNT, 
            MON_NO, YEAR_NO, USERID, IMPORT_DT, IMPORTDATATYPE, NEWINSERT
        )
        SELECT /*+ PARALLEL(4) */
            D.POLICY_NO, D.COMPANY_CD, D.STATUS_CD, D.LOCATION, D.CL_NAME, D.PREM_AMT, 
            D.PAY_MODE, D.SA, D.DUE_DATE, D.CL_ADD1, D.CL_ADD2, D.CL_ADD3, D.CL_ADD4, 
            D.CL_ADD5, D.CL_CITY, D.CL_PIN, D.CL_PHONE1, D.CL_PHONE2, D.CL_MOBILE, 
            D.PLAN_NAME, D.DOC, D.PREM_FREQ, D.PLY_TERM, D.PPT, D.NET_AMOUNT,         
            P_MONTH, P_YEAR, P_LOG_ID, SYSDATE, P_IMPORTDATATYPE, 'BAL'
            
        FROM PSM_DAP_TEMP_DUE1 D
        WHERE NOT EXISTS (
            SELECT /*+ HASH_SJ */ 1 
            FROM BAJAJ_DUE_DATA B
            WHERE UPPER(TRIM(B.POLICY_NO)) = UPPER(TRIM(D.POLICY_NO))
              AND UPPER(TRIM(B.COMPANY_CD)) = UPPER(TRIM(D.COMPANY_CD))
              AND B.MON_NO = D.MON_NO
              AND B.YEAR_NO = D.YEAR_NO
              AND B.IMPORTDATATYPE = D.IMPORTDATATYPE
        );
        
        IF P_IMPORTDATATYPE = 'LAPSEDDATA' THEN -- Step 2: Bulk insert IN TEMP TABLE TO LAPSED DATA FROM PSM_DAP_TEMP_DUE1
            
            BEGIN -- STEP 3: BULK INSERT ALL LAPSED DATA IN psm_dap_temp_due1_lap  FROM PSM_DAP_TEMP_DUE1 
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
                        AND b.due_date = ( SELECT MAX(c.due_date) FROM bajaj_due_data c join psm_dap_temp_due1 e on e.policy_no = c.policy_no
                                WHERE UPPER(TRIM(c.policy_no)) = e.policy_no AND UPPER(TRIM(c.company_cd)) = e.company_cd 
                                AND c.cdue_date IS NOT NULL AND c.importdatatype = 'DUEDATA'                    
                        )
                    ) as status_cd ,
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

                COMMIT;
            END;

            BEGIN -- UDPATE BAJAJ_DUE_DATA FROM LAPSED DATA            
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
                    b.last_update = p_log_id;          
            END;
                        
            BEGIN -- UDPATE POLICYT_DETAILS_MASTER FROM LAPSED DATA
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
                    p.update_prog = P_IMPORTDATATYPE,
                    p.update_user = p_log_id,
                    p.update_date = TRUNC(sysdate);

                COMMIT;
            
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
                    AND h.status_cd = 'L'
                );

                -- STEP 2: Bulk insert into BAJAJ_AR_DETAILS for updated records
                INSERT INTO BAJAJ_AR_DETAILS
                (SYS_AR_NO, STATUS_DT, STATUS_CD, REMARKS, SYS_AR_DT, STATUS_UPDATE_ON)
                SELECT 
                    h.SYS_AR_NO,
                    LAST_DAY(h.SYS_AR_DT),
                    'L',
                    'LAPSSSSED DATA' || ' ' || TO_CHAR(SYSDATE,'DD-MON-YYYY HH24:MI:SS'),
                    h.SYS_AR_DT,
                    SYSDATE
                FROM 
                    BAJAJ_AR_HEAD h
                JOIN 
                    psm_dap_temp_due1 d ON h.POLICY_NO = d.policy_no AND h.COMPANY_CD = d.company_cd
                WHERE 
                    TO_CHAR(h.SYS_AR_DT,'MON-YYYY') =  TO_CHAR(P_MONTH|| '-' || P_YEAR)
                    AND h.status_cd = 'L'
                    AND NOT EXISTS (
                        SELECT 1 
                        FROM BAJAJ_AR_DETAILS d2
                        WHERE d2.SYS_AR_NO = h.SYS_AR_NO
                        AND d2.STATUS_DT = LAST_DAY(h.SYS_AR_DT)
                    );
                COMMIT;                        
            END;
        END IF; -- CLOSE LAPSED IN DUE
    END IF; -- CLOSE P_CHKTYPE = DUE
    OPEN P_CURSOR FOR 
        SELECT 'procedure working' FROM DUAL;
        

END;