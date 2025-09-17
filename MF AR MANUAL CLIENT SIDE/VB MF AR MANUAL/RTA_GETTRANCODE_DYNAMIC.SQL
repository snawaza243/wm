CREATE OR REPLACE FUNCTION WEALTHMAKER.GETTRANCODE_DYNAMIC (
    P_BRANCH         IN VARCHAR2,
    P_BRANCHES       IN VARCHAR2,
    P_SRM_CODE       IN VARCHAR2,
    P_AMC            IN VARCHAR2,
    P_MUT_CODE       IN VARCHAR2,
    P_RECONCILE      IN VARCHAR2, -- 'Y', 'N', or NULL
    P_REGULAR_ST     IN VARCHAR2, -- 'Y' or NULL
    P_SIP_ST         IN VARCHAR2, -- 'Y' or NULL
    P_FROM_DATE      IN DATE,
    P_TO_DATE        IN DATE,
    P_CHEQUE_TYPE    IN VARCHAR2, -- 'CHEQUE', 'FOLIO', 'APP', 'PAN', 'SUBBROK'
    P_CHEQUE_SEARCH  IN VARCHAR2,
    P_INVESTOR_NAME  IN VARCHAR2,
    P_AMOUNT         IN NUMBER,
    P_SCH_CODE       IN VARCHAR2,
    P_FOLIO_NO       IN VARCHAR2,
    P_TR_DATE        IN DATE,
    P_FLAG           IN VARCHAR2   -- 'CAMS', 'KARVY', 'KARVY COB'
) RETURN CLOB
IS
    FINALTRANCODE   CLOB := NULL;
    TRANCODE        TRANSACTION_ST.TRAN_CODE%TYPE;
    CURSOR MYCUR IS
        SELECT t.tran_code
          FROM employee_master e,
               branch_master b,
               mut_fund amc,
               scheme_info sch,
               TRANSACTION_ST@MF.BAJAJCAPITAL t,
               investor_master i,
               CITY_MASTER c
         WHERE i.city_id = c.city_id(+)
           AND t.client_code = i.inv_code
           AND TO_CHAR(t.rmcode) = e.rm_code
           AND t.branch_code = b.branch_code
           AND t.mut_code = amc.mut_code
           AND t.sch_code = sch.sch_code
           -- Branch filter
           AND (
                (P_BRANCH IS NOT NULL AND P_BRANCH <> 'ALL' AND b.branch_code IN (SELECT REGEXP_SUBSTR(P_BRANCH, '[^,]+', 1, LEVEL) FROM dual CONNECT BY REGEXP_SUBSTR(P_BRANCH, '[^,]+', 1, LEVEL) IS NOT NULL))
                OR
                (P_BRANCH IS NULL OR P_BRANCH = 'ALL') AND b.branch_code IN (SELECT REGEXP_SUBSTR(P_BRANCHES, '[^,]+', 1, LEVEL) FROM dual CONNECT BY REGEXP_SUBSTR(P_BRANCHES, '[^,]+', 1, LEVEL) IS NOT NULL)
           )
           -- RM code
           AND (P_SRM_CODE IS NULL OR e.rm_code = P_SRM_CODE)
           -- AMC
           AND (P_AMC IS NULL OR TO_CHAR(t.mut_code) = P_AMC)
           -- MUT code
           AND (P_MUT_CODE IS NULL OR TO_CHAR(t.mut_code) = P_MUT_CODE)
           -- Reconcile
           AND (
                P_RECONCILE IS NULL OR
                (P_RECONCILE = 'Y' AND t.rec_flag = 'Y') OR
                (P_RECONCILE = 'N' AND (t.rec_flag = 'N' OR t.rec_flag IS NULL))
           )
           -- Regular/SIP
           AND (
                (P_REGULAR_ST = 'Y' AND (
                    (UPPER(t.reg_trantype) NOT LIKE '%SYS%' AND UPPER(t.reg_trantype) NOT LIKE '%SIP%' AND TRIM(UPPER(t.reg_trantype)) NOT LIKE '%S T P IN%'
                    AND TRIM(UPPER(t.reg_trantype)) NOT LIKE '%S T P IN REJ%'
                    AND TRIM(UPPER(t.reg_trantype)) NOT LIKE '%S T P IN REJ REVERSAL%'
                    AND TRIM(UPPER(t.reg_trantype)) NOT LIKE '%STP SWITCH IN%'
                    AND TRIM(UPPER(t.reg_trantype)) NOT LIKE '%STPI%'
                    AND TRIM(UPPER(t.reg_trantype)) NOT LIKE '%STPIR%'
                    ) OR t.reg_trantype IS NULL
                ))
                OR
                (P_SIP_ST = 'Y' AND (
                    UPPER(t.reg_trantype) LIKE '%SYS%'
                    OR UPPER(t.reg_trantype) LIKE '%SIP%'
                    OR TRIM(UPPER(t.reg_trantype)) LIKE '%S T P IN%'
                    OR TRIM(UPPER(t.reg_trantype)) LIKE '%S T P IN REJ%'
                    OR TRIM(UPPER(t.reg_trantype)) LIKE '%S T P IN REJ REVERSAL%'
                    OR TRIM(UPPER(t.reg_trantype)) LIKE '%STP SWITCH IN%'
                    OR TRIM(UPPER(t.reg_trantype)) LIKE '%STPI%'
                    OR TRIM(UPPER(t.reg_trantype)) LIKE '%STPIR%'
                ))
                OR (P_REGULAR_ST IS NULL AND P_SIP_ST IS NULL)
           )
           -- Date range
           AND (P_FLAG = 'KARVY COB' OR (P_FROM_DATE IS NULL OR t.tr_date >= P_FROM_DATE))
           AND (P_FLAG = 'KARVY COB' OR (P_TO_DATE IS NULL OR t.tr_date <= P_TO_DATE))
           -- Cheque/search
           AND (
                (P_CHEQUE_TYPE = 'CHEQUE' AND t.cheque_no = P_CHEQUE_SEARCH)
                OR (P_CHEQUE_TYPE = 'FOLIO' AND t.folio_no = P_CHEQUE_SEARCH)
                OR (P_CHEQUE_TYPE = 'APP' AND t.app_no = P_CHEQUE_SEARCH)
                OR (P_CHEQUE_TYPE = 'SUBBROK' AND t.reg_subbrok = P_CHEQUE_SEARCH)
                OR (P_CHEQUE_TYPE = 'PAN' AND (UPPER(t.pan1) = UPPER(P_CHEQUE_SEARCH) OR UPPER(t.pan2) = UPPER(P_CHEQUE_SEARCH) OR UPPER(t.pan3) = UPPER(P_CHEQUE_SEARCH)))
                OR (P_CHEQUE_TYPE IS NULL)
           )
           -- Investor name
           AND (P_INVESTOR_NAME IS NULL OR UPPER(TRIM(t.inv_name)) LIKE '%' || REPLACE(UPPER(P_INVESTOR_NAME), ' ', '%') || '%')
           -- Amount
           AND (P_AMOUNT IS NULL OR ABS(ROUND(t.amount)) = ABS(ROUND(P_AMOUNT)))
           -- Static filters
           AND LPAD(t.mut_code, 2) = 'MF'
           AND (t.asa <> 'C' OR t.asa IS NULL)
           AND t.tran_type IN ('PURCHASE', 'REINVESTMENT', 'SWITCH IN')
           AND t.sch_code = P_SCH_CODE
           AND t.folio_no = P_FOLIO_NO
           AND (P_FLAG = 'KARVY COB' OR t.tr_date = P_TR_DATE)
           -- CAMS/KARVY flag
           AND (
                (P_FLAG = 'CAMS' AND (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE = 'TICOB' AND DUP_FLAG2 IN (0, 9))))
                OR (P_FLAG = 'KARVY' AND (DUP_FLAG2 = 0 OR (REG_TRAN_FLAG = 'TI' AND DUP_FLAG2 IN (0, 9))))
                OR (P_FLAG = 'KARVY COB' AND (DUP_FLAG2 = 0 OR (REG_TRAN_TYPE = 'TICOB' AND DUP_FLAG2 IN (0, 9))))
           );
BEGIN
    FOR rec IN MYCUR LOOP
        IF FINALTRANCODE IS NOT NULL THEN
            FINALTRANCODE := FINALTRANCODE || ',' || '''' || rec.tran_code || '''';
        ELSE
            FINALTRANCODE := '''' || rec.tran_code || '''';
        END IF;
    END LOOP;
    RETURN FINALTRANCODE;
END;
/