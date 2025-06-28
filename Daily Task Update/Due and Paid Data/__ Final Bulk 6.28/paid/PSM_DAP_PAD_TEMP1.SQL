CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_DAP_BULK1(
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
    
    -- At the beginning of your procedure where you declare variables
    V_MYIMPORT          VARCHAR2(1);
    V_MYIMPORTDATATYPE  VARCHAR2(20);  
    V_CURDATE           VARCHAR2(20) := TO_CHAR(SYSDATE, 'DD/MM/YYYY');
    V_SERVERDATE        VARCHAR2(20) := TO_CHAR(SYSDATE, 'DD-MON-YYYY');

    v_total_records     NUMBER;
    v_inserted_records  NUMBER;
    v_updated_records   NUMBER;
    v_rec_count         NUMBER := 0;
    v_rec_del           NUMBER := 0;
    
BEGIN

    IF P_DDLIMPORTTYPE IS NOT NULL THEN
        IF P_DDLIMPORTTYPE = 'DUE' THEN
            V_MYIMPORT := 'D';
            V_MYIMPORTDATATYPE := 'DUEDATA';
        ELSIF P_DDLIMPORTTYPE = 'LAPSED' THEN
            V_MYIMPORT := 'L';
            V_MYIMPORTDATATYPE := 'LAPSEDDATA';
        ELSIF P_DDLIMPORTTYPE = 'PAID' THEN
            V_MYIMPORT := 'D';
            V_MYIMPORTDATATYPE := 'DUEDATA';
        ELSIF P_DDLIMPORTTYPE = 'REINS' THEN
            V_MYIMPORT := 'L';
            V_MYIMPORTDATATYPE := 'LAPSEDDATA';
        END IF;
    END IF;

    IF P_CHKTYPE = 'DUE' THEN
        OPEN P_CURSOR FOR 
            SELECT 'Procedure task done for due. ' as MESSAGE
            FROM DUAL;
            
    
    ELSIF P_CHKTYPE = 'PAID' THEN
        -- First insert any new records that don't exist in BAJAJ_DUE_DATA
        INSERT /*+ APPEND PARALLEL(4) */ INTO BAJAJ_DUE_DATA (
            POLICY_NO, COMPANY_CD, PREM_AMT, PAID_DATE, STATUS_CD, NET_AMOUNT, 
            DUE_DATE, NEXT_DUE_DATE, PREM_FREQ, PAY_MODE, DOC, PLAN_NAME,
            MON_NO, YEAR_NO, USERID, IMPORT_DT, IMPORTDATATYPE, NEWINSERT, FORCE_FLAG            
        )
        SELECT /*+ PARALLEL(4) */
            TP.policy_no, TP.company_cd, TP.prem_amt, TP.paid_date, TP.status_cd, TP.net_amount, 
            TP.due_date, TP.next_due_date, TP.prem_freq, TP.pay_mode, TP.doc, TP.plan_name,
            TO_NUMBER(P_MONTH), TO_NUMBER(P_YEAR), P_LOG_ID, SYSDATE, P_DDLIMPORTTYPE, NULL, 'FORCE FULL'
        FROM psm_dap_temp_paid1 TP
        WHERE NOT EXISTS (
            SELECT 1 
            FROM BAJAJ_DUE_DATA BDD
            WHERE UPPER(TRIM(BDD.POLICY_NO)) = UPPER(TRIM(TP.POLICY_NO))
              AND UPPER(TRIM(BDD.COMPANY_CD)) = UPPER(TRIM(TP.COMPANY_CD))
              AND BDD.MON_NO = TO_NUMBER(P_MONTH)
              AND BDD.YEAR_NO = TO_NUMBER(P_YEAR)
              AND BDD.IMPORTDATATYPE = P_DDLIMPORTTYPE
        );
        
        v_inserted_records := SQL%ROWCOUNT;
        
        -- Update POLICY_DETAILS_MASTER for policies with status changing from L to PAID
        UPDATE POLICY_DETAILS_MASTER PDM
        SET LAST_STATUS = 'R',
            UPDATE_PROG = P_DDLIMPORTTYPE,
            UPDATE_USER = P_LOG_ID,
            UPDATE_DATE = SYSDATE
        WHERE EXISTS (
            SELECT 1 
            FROM psm_dap_temp_paid1 TP
            WHERE UPPER(TRIM(PDM.POLICY_NO)) = UPPER(TRIM(TP.POLICY_NO))
              AND UPPER(TRIM(PDM.COMPANY_CD)) = UPPER(TRIM(TP.COMPANY_CD))
              AND PDM.LAST_STATUS = 'L'
              AND UPPER(TP.STATUS_CD) = 'PAID'
        );
        
        -- Insert policies not found in POLICY_DETAILS_MASTER
        INSERT INTO POLICYNOTINHEADER(POLICY_NO, COMPANY_CD)
        SELECT TP.POLICY_NO, TP.COMPANY_CD
        FROM psm_dap_temp_paid1 TP
        WHERE NOT EXISTS (
            SELECT 1 
            FROM POLICY_DETAILS_MASTER PDM
            WHERE UPPER(TRIM(PDM.POLICY_NO)) = UPPER(TRIM(TP.POLICY_NO))
              AND UPPER(TRIM(PDM.COMPANY_CD)) = UPPER(TRIM(TP.COMPANY_CD))
        );
        
        -- Update BAJAJ_DUE_DATA for policies with max date condition
        MERGE INTO BAJAJ_DUE_DATA BDD
        USING (
            SELECT 
                TP.POLICY_NO,
                TP.COMPANY_CD,
                TP.STATUS_CD,
                MAX(TO_DATE('01/'||P_MONTH||'/'||P_YEAR,'dd/mm/yyyy')) as MAX_DATE
            FROM psm_dap_temp_paid1 TP
            JOIN (
                SELECT POLICY_NO, COMPANY_CD, MAX(TO_DATE('01/'||MON_NO||'/'||YEAR_NO,'dd/mm/yyyy')) as FROM_DT
                FROM BAJAJ_DUE_DATA
                WHERE STATUS_CD = 'LAPSED'
                GROUP BY POLICY_NO, COMPANY_CD
            ) MD ON UPPER(TRIM(MD.POLICY_NO)) = UPPER(TRIM(TP.POLICY_NO))
                 AND UPPER(TRIM(MD.COMPANY_CD)) = UPPER(TRIM(TP.COMPANY_CD))
            WHERE TO_DATE('01/'||P_MONTH||'/'||P_YEAR,'dd/mm/yyyy') > MD.FROM_DT
            GROUP BY TP.POLICY_NO, TP.COMPANY_CD, TP.STATUS_CD
        ) SRC
        ON (UPPER(TRIM(BDD.POLICY_NO)) = UPPER(TRIM(SRC.POLICY_NO))
           AND UPPER(TRIM(BDD.COMPANY_CD)) = UPPER(TRIM(SRC.COMPANY_CD))
           AND BDD.IMPORTDATATYPE = P_DDLIMPORTTYPE
           AND TO_DATE('01/'||BDD.MON_NO||'/'||BDD.YEAR_NO,'dd/mm/yyyy') > TO_DATE(V_CURDATE, 'dd/mm/yyyy')
           AND TO_DATE('01/'||BDD.MON_NO||'/'||BDD.YEAR_NO,'dd/mm/yyyy') <= LAST_DAY(TO_DATE('01/'||BDD.MON_NO||'/'||BDD.YEAR_NO,'dd/mm/yyyy'))
        WHEN MATCHED THEN
            UPDATE SET 
                BDD.STATUS_CD = SRC.STATUS_CD,
                BDD.LAST_UPDATE_DT = TO_DATE(V_SERVERDATE, 'DD-MON-YYYY'),
                BDD.LAST_UPDATE = P_LOG_ID;
        
        -- Update BAJAJ_DUE_DATA for other policies
        UPDATE BAJAJ_DUE_DATA BDD
        SET STATUS_CD = (
                SELECT UPPER(TP.STATUS_CD)
                FROM psm_dap_temp_paid1 TP
                WHERE UPPER(TRIM(TP.POLICY_NO)) = UPPER(TRIM(BDD.POLICY_NO))
                  AND UPPER(TRIM(TP.COMPANY_CD)) = UPPER(TRIM(BDD.COMPANY_CD))
            ),
            LAST_UPDATE_DT = TO_DATE(V_SERVERDATE, 'DD-MON-YYYY'),
            LAST_UPDATE = P_LOG_ID
        WHERE EXISTS (
            SELECT 1
            FROM psm_dap_temp_paid1 TP
            WHERE UPPER(TRIM(TP.POLICY_NO)) = UPPER(TRIM(BDD.POLICY_NO))
              AND UPPER(TRIM(TP.COMPANY_CD)) = UPPER(TRIM(BDD.COMPANY_CD))
              AND BDD.MON_NO = TO_NUMBER(P_MONTH)
              AND BDD.YEAR_NO = TO_NUMBER(P_YEAR)
              AND BDD.IMPORTDATATYPE = P_DDLIMPORTTYPE
        );
        
        v_updated_records := SQL%ROWCOUNT;
        
        -- Insert into BAJAJ_PAID_DATA for paid policies
        INSERT /*+ APPEND PARALLEL(4) */ INTO BAJAJ_PAID_DATA (
            POLICY_NO, COMPANY_CD, PREM_AMT, PAID_DATE, STATUS_CD, NET_AMOUNT, 
            DUE_DATE, NEXT_DUE_DATE, PREM_FREQ, PAY_MODE, DOC, PLAN_NAME,
            MON_NO, YEAR_NO, USERID, IMPORT_DT, IMPORTDATATYPE
        )
        SELECT /*+ PARALLEL(4) */
            TP.policy_no, TP.company_cd, TP.prem_amt, TP.paid_date, TP.status_cd, TP.net_amount, 
            TP.due_date, TP.next_due_date, TP.prem_freq, TP.pay_mode, TP.doc, TP.plan_name,
            TO_NUMBER(P_MONTH), TO_NUMBER(P_YEAR), P_LOG_ID, TO_DATE(V_SERVERDATE, 'DD-MON-YYYY'), P_DDLIMPORTTYPE
        FROM psm_dap_temp_paid1 TP
        WHERE UPPER(TP.STATUS_CD) = 'PAID'
          AND NOT EXISTS (
              SELECT 1 
              FROM BAJAJ_PAID_DATA BPD
              WHERE UPPER(TRIM(BPD.POLICY_NO)) = UPPER(TRIM(TP.POLICY_NO))
                AND UPPER(TRIM(BPD.COMPANY_CD)) = UPPER(TRIM(TP.COMPANY_CD))
                AND BPD.MON_NO = TO_NUMBER(P_MONTH)
                AND BPD.YEAR_NO = TO_NUMBER(P_YEAR)
                AND BPD.IMPORTDATATYPE = P_DDLIMPORTTYPE
          );
        
        v_rec_count := SQL%ROWCOUNT;
        
        -- Update POLICY_DETAILS_MASTER for paid policies
        UPDATE POLICY_DETAILS_MASTER PDM
        SET LAST_STATUS = 'A',
            UPDATE_PROG = P_DDLIMPORTTYPE,
            UPDATE_USER = P_LOG_ID,
            UPDATE_DATE = TO_DATE(V_SERVERDATE, 'DD/MM/YYYY')
        WHERE EXISTS (
            SELECT 1 
            FROM psm_dap_temp_paid1 TP
            WHERE UPPER(TRIM(PDM.POLICY_NO)) = UPPER(TRIM(TP.POLICY_NO))
              AND UPPER(TRIM(PDM.COMPANY_CD)) = UPPER(TRIM(TP.COMPANY_CD))
              AND UPPER(TP.STATUS_CD) = 'PAID'
        );
        
        -- Update status for paid policies (assuming UpdateBajajStatus functionality)
        -- This would need to be implemented as a separate procedure call or logic
        
        -- Update existing records in BAJAJ_PAID_DATA
        UPDATE BAJAJ_PAID_DATA BPD
        SET STATUS_CD = (
                SELECT UPPER(TP.STATUS_CD)
                FROM psm_dap_temp_paid1 TP
                WHERE UPPER(TRIM(TP.POLICY_NO)) = UPPER(TRIM(BPD.POLICY_NO))
                  AND UPPER(TRIM(TP.COMPANY_CD)) = UPPER(TRIM(BPD.COMPANY_CD))
            ),
            LAST_UPDATE_DT = TO_DATE(V_SERVERDATE, 'DD-MON-YYYY'),
            LAST_UPDATE = P_LOG_ID
        WHERE EXISTS (
            SELECT 1
            FROM psm_dap_temp_paid1 TP
            WHERE UPPER(TRIM(TP.POLICY_NO)) = UPPER(TRIM(BPD.POLICY_NO))
              AND UPPER(TRIM(TP.COMPANY_CD)) = UPPER(TRIM(BPD.COMPANY_CD))
              AND BPD.MON_NO = TO_NUMBER(P_MONTH)
              AND BPD.YEAR_NO = TO_NUMBER(P_YEAR)
              AND BPD.IMPORTDATATYPE = P_DDLIMPORTTYPE
        );
        
        -- Handle duplicate policies
        DELETE FROM DUP_POLICY;
        
        INSERT INTO DUP_POLICY
        SELECT policy_no FROM (
            SELECT a.policy_no, a.sys_ar_Dt, a.company_Cd 
            FROM bajaj_Ar_head a 
            JOIN bajaj_paid_Data b ON UPPER(TRIM(a.COMPANY_CD)) = UPPER(TRIM(B.COMPANY_CD)) 
                AND a.Policy_No = B.Policy_No 
            WHERE b.mon_no = TO_NUMBER(P_MONTH) 
              AND b.year_no = TO_NUMBER(P_YEAR) 
              AND b.importdatatype = P_DDLIMPORTTYPE
            GROUP BY a.policy_no, a.sys_ar_dt, a.company_Cd 
            HAVING COUNT(a.policy_no) > 1 
               AND COUNT(a.sys_ar_dt) > 1 
               AND COUNT(A.company_cd) > 1
        );
        
        -- Mark duplicate records
        UPDATE bajaj_paid_Data 
        SET dup_rec = 'Y' 
        WHERE policy_no IN (SELECT POLICY_NO FROM DUP_POLICY) 
          AND mon_no = TO_NUMBER(P_MONTH) 
          AND year_no = TO_NUMBER(P_YEAR) 
          AND importdatatype = P_DDLIMPORTTYPE;
        
        v_rec_del := SQL%ROWCOUNT;
        v_rec_count := v_rec_count - v_rec_del;
        
        -- Update employee and investor codes
        UPDATE bajaj_paid_data A 
        SET A.emp_no = (
                SELECT MAX(B.emp_no) 
                FROM bajaj_ar_head B 
                WHERE UPPER(TRIM(B.Company_cd)) = UPPER(TRIM(A.Company_cd)) 
                  AND B.policy_no = A.policy_no 
                  AND B.FRESH_RENEWAL = 1
            ),
            inv_cd = (
                SELECT MAX(B.client_Cd) 
                FROM bajaj_ar_head B 
                WHERE UPPER(TRIM(B.Company_cd)) = UPPER(TRIM(A.Company_cd)) 
                  AND B.policy_no = A.policy_no 
                  AND B.FRESH_RENEWAL = 1
            )
        WHERE mon_no = TO_NUMBER(P_MONTH) 
          AND year_no = TO_NUMBER(P_YEAR) 
          AND dup_Rec IS NULL 
          AND importdatatype = P_DDLIMPORTTYPE;
        
        OPEN P_CURSOR FOR 
            SELECT 'Records Imported Successfully. Inserted: ' || v_inserted_records || 
                   ', Updated: ' || v_updated_records || 
                   ', Paid Records: ' || v_rec_count as MESSAGE
            FROM DUAL;
    END IF;
    
EXCEPTION
    WHEN OTHERS THEN
        OPEN P_CURSOR FOR 
            SELECT 'Error: ' || SQLERRM as MESSAGE FROM DUAL;
END;
/