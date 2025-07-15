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
    V_MYIMPORT          VARCHAR2(10);    
    v_total_records     number;
    v_inserted_records  number;
    v_updated_records   number;
BEGIN

/*
open p_cursor for 
select 
    P_FILENAME || ' - ' ||
    P_SHEETNAME || ' - ' ||
    P_MONTH || ' - ' ||
    P_YEAR || ' - ' ||
    P_DDLIMPORTTYPE || ' - ' || 
    P_CHKTYPE || ' - ' ||
    P_DATACOUNT || ' - ' ||
    P_LOG_ID || ' - ' ||
    P_ROLE_ID || ' - '
    from dual;
    return;
    
*/


    IF P_CHKTYPE = 'DUE' THEN
        -- STEP 0: BULK INSERTION IN PSM_DAP_TEMP_DUE1 FROM EXCEL DATA DONE FROM CLIENT SIDE
        
        -- STEP 1: CLEAN THE BULK IMPORTED DATA IN TEMP TABLE
        CLEAN_PSM_DUE1;
        
        -- Step 2: Bulk Insert new data in BAJAJ_DUE_DATA, avoiding duplicates from psm_dap_temp_due1
        INSERT /*+ APPEND PARALLEL(4) */ INTO BAJAJ_DUE_DATA (
            POLICY_NO, COMPANY_CD, STATUS_CD, LOCATION, CL_NAME, PREM_AMT, 
            PAY_MODE, SA, DUE_DATE, CL_ADD1, CL_ADD2, CL_ADD3, CL_ADD4, 
            CL_ADD5, CL_CITY, CL_PIN, CL_PHONE1, CL_PHONE2, CL_MOBILE, 
            PLAN_NAME, DOC, PREM_FREQ, PLY_TERM, PPT, NET_AMOUNT, 
            MON_NO, YEAR_NO, USERID, IMPORT_DT, IMPORTDATATYPE, NEWINSERT
        )
        SELECT /*+ PARALLEL(4) */
            D.POLICY_NO, D.COMPANY_CD, D.STATUS_CD, D.LOCATION, D.CL_NAME,  CAST(D.PREM_AMT AS NUMBER(14,2)), 
            D.PAY_MODE, CAST(D.PREM_AMT AS NUMBER(14,2)), TO_DATE(D.DUE_DATE, 'DD-MM-YYYY'), 
            D.CL_ADD1, D.CL_ADD2, D.CL_ADD3, D.CL_ADD4, D.CL_ADD5, D.CL_CITY, D.CL_PIN, 
            D.CL_PHONE1, D.CL_PHONE2, D.CL_MOBILE, 
            D.PLAN_NAME,  TO_DATE(D.DOC, 'DD-MM-YYYY') , D.PREM_FREQ, D.PLY_TERM, D.PPT, CAST(D.NET_AMOUNT AS NUMBER(14,2))
            , to_number(P_MONTH), to_number(P_YEAR), P_LOG_ID, SYSDATE, P_DDLIMPORTTYPE, 'BAL'
        FROM PSM_DAP_TEMP_DUE1 D
        WHERE NOT EXISTS (
            SELECT /*+ HASH_SJ */ 1 
            FROM BAJAJ_DUE_DATA B
            WHERE UPPER(TRIM(B.POLICY_NO)) = UPPER(TRIM(D.POLICY_NO))
              AND UPPER(TRIM(B.COMPANY_CD)) = UPPER(TRIM(D.COMPANY_CD))
              AND B.MON_NO = to_number(P_MONTH)
              AND B.YEAR_NO = to_number(P_YEAR)
              AND B.IMPORTDATATYPE = P_DDLIMPORTTYPE
        );
        
        -- Update POLICY_DETAILS_MASTER for DUEDATA imports
        IF P_DDLIMPORTTYPE = 'DUEDATA' THEN -- Update payment mode and basic info
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
        END IF;
        
        IF P_DDLIMPORTTYPE = 'LAPSEDDATA' THEN
            -- Step 2: Bulk insert IN TEMP TABLE TO LAPSED DATA FROM PSM_DAP_TEMP_DUE1
            
            BEGIN -- STEP 3: BULK INSERT ALL LAPSED DATA IN psm_dap_temp_due1_lap FROM PSM_DAP_TEMP_DUE1 
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
                    b.last_update = P_LOG_ID;
                    
                COMMIT;          
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
                    p.update_prog = P_DDLIMPORTTYPE,
                    p.update_user = P_LOG_ID,
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
                );

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
                COMMIT;                        
            END;
        END IF; -- CLOSE LAPSED IN DUE            
    END IF;
    
    OPEN P_CURSOR FOR 
    -- all inserted and updated count return message
        SELECT 'procedure working' FROM DUAL;
END;
/