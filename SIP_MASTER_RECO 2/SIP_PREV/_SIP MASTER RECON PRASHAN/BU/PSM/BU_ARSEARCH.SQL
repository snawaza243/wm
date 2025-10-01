CREATE OR REPLACE PROCEDURE WEALTHMAKER.SP_GET_TRANSACTIONS_PRA (
    p_branch_cat     IN NUMBER,
    p_ar_num         IN transaction_mf_temp1.tran_code%TYPE,
    p_branch         IN branch_master.branch_code%TYPE, 
    p_region         IN branch_master.region_id%TYPE,  
    p_rm             IN transaction_mf_temp1.rmcode%TYPE, 
    p_zone           IN branch_master.zone_id%TYPE,  
    p_tran_type      IN transaction_mf_temp1.tran_type%TYPE,  
    p_status_type    IN VARCHAR2,   
    p_cob            IN CHAR,
    p_pms            IN VARCHAR2,
    p_date_from      IN transaction_mf_temp1.tr_date%TYPE,  
    p_date_to        IN transaction_mf_temp1.tr_date%TYPE,  
    p_amc            IN mut_fund.mut_name%TYPE,
   
    o_transactions   OUT SYS_REFCURSOR  
) AS
    v_query VARCHAR2(4000);
BEGIN
    v_query := 'SELECT 
                inv.Investor_Name,
                inv.address1||'',''||inv.address2||'',''||inv.phone||'',''||inv.email AS address,
                ct.city_name,
                tmf.BROKER_ID,
                tmf.RMCODE,
                tmf.busi_branch_code,
                tmf.BANK_NAME,
                tmf.REC_FLAG,
                tmf.FLAG,
                tmf.CLIENT_CODE,
                tmf.SCH_CODE,
                tmf.MUT_CODE,
                ei.rm_name,
                bi.branch_name,
                tmf.PANNO,
                amc.MUT_NAME,
                sch.SCH_NAME,
                tmf.TR_DATE,
                tmf.TRAN_TYPE,
                tmf.APP_NO,
                tmf.FOLIO_NO,
                tmf.PAYMENT_MODE,
                tmf.CHEQUE_NO,
                tmf.CHEQUE_DATE,
                tmf.AMOUNT,
                tmf.SIP_AMOUNT,
                tmf.SIP_TYPE,
                tmf.LEAD_NO,
                tmf.LEAD_NAME,
                tmf.TRAN_CODE,
                bi.BRANCH_CODE,
                tmf.BUSINESS_RMCODE,
                tmf.INSTALLMENTS_NO,
                NVL(tmf.cob_flag, 0) AS cob_flag,
                    -- Inline logic for FN_IDENTIFY_REGISTRAR
                    (SELECT REG 
                     FROM REG_SCHEMES
                     WHERE MUT_CODE = tmf.MUT_CODE 
                     AND ROWNUM = 1) AS REGISTRAR,
                RETURNSTATUS(tmf.TRAN_CODE) AS STATUS,
                CASE 
                    WHEN tmf.loggeduserid IN (''MFONLINE'',''Valuefy'') THEN ''Online''
                    ELSE ''Offline''
                END AS loggeduserid,
                NVL(tmf.cob_flag, 0) AS cob_flag
              FROM 
                transaction_mf_temp1 tmf
                LEFT JOIN investor_master inv ON tmf.client_code = inv.inv_code
                LEFT JOIN city_master ct ON inv.city_id = ct.city_id
                LEFT JOIN ALLSCHEME sch ON tmf.SCH_CODE = sch.SCH_CODE
                LEFT JOIN ALLCOMPANY amc ON tmf.MUT_CODE = amc.MUT_CODE
                LEFT JOIN branch_master bi ON tmf.BUSI_BRANCH_CODE = bi.branch_code
                LEFT JOIN employee_master ei ON TO_CHAR(tmf.BUSINESS_RMCODE) = TO_CHAR(ei.payroll_id)
              WHERE 1=1
                AND tmf.SIP_TYPE = ''SIP''
                AND tmf.AMOUNT > 0
                AND (tmf.ASA <> ''C'' OR tmf.ASA IS NULL)
                AND tmf.move_flag1 IS NULL
                AND tmf.sip_id IS NULL';

    -- Add dynamic conditions
    IF p_ar_num IS NOT NULL THEN
        v_query := v_query || ' AND tmf.tran_code = ''' || p_ar_num || '''';
    END IF;

    IF p_branch IS NOT NULL THEN
        v_query := v_query || ' AND bi.branch_code IN (' || p_branch || ')';
    END IF;

    IF p_region IS NOT NULL THEN
        v_query := v_query || ' AND bi.region_id = ''' || p_region || '''';
    END IF;

    IF p_rm IS NOT NULL THEN
        v_query := v_query || ' AND tmf.rmcode = ''' || p_rm || '''';
    END IF;

    IF p_zone IS NOT NULL THEN
        v_query := v_query || ' AND bi.zone_id = ''' || p_zone || '''';
    END IF;

    IF p_branch_cat IS NOT NULL THEN
        v_query := v_query || ' AND bi.branch_tar_cat = ' || p_branch_cat; -- No quotes, as it's a number
    END IF;

    IF p_rm IS NOT NULL THEN
        v_query := v_query || ' AND ei.rm_code = ''' || p_rm || '''';
    END IF;

    IF p_tran_type IS NOT NULL THEN
        IF UPPER(p_tran_type) = 'SIP' THEN
            v_query := v_query || ' AND (tmf.SIP_FR = ''F'' OR tmf.SIP_FR IS NULL)';
        ELSIF UPPER(p_tran_type) = 'REGULAR' THEN
            v_query := v_query || ' AND tmf.SIP_FR = ''R''';
        END IF;
    END IF;

    IF p_status_type IS NOT NULL THEN
        IF UPPER(p_status_type) = 'Y' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''Y''';
        ELSIF UPPER(p_status_type) = 'N' THEN
            v_query := v_query || ' AND NVL(tmf.rec_flag, ''N'') = ''N''';
        END IF;
    END IF;

    IF p_cob = '1' THEN
        v_query := v_query || ' AND tmf.cob_flag = ''1''';
    END IF;

    IF p_pms = '1' THEN
        v_query := v_query || ' AND (sch.prd = ''DT027'' 
                     OR sch.SCH_CODE IN (SELECT SCH_CODE FROM scheme_info WHERE nature = ''ETF''))';
    END IF;

    IF p_date_from IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TR_DATE >= TO_DATE(''' || TO_CHAR(p_date_from, 'DD/MM/YYYY') || ''', ''DD/MM/YYYY'')';
    END IF;

    IF p_date_to IS NOT NULL THEN
        v_query := v_query || ' AND tmf.TR_DATE <= TO_DATE(''' || TO_CHAR(p_date_to, 'DD/MM/YYYY') || ''', ''DD/MM/YYYY'')';
    END IF;

    IF p_amc IS NOT NULL THEN
        v_query := v_query || ' AND amc.MUT_NAME = ''' || p_amc || '''';
    END IF;

    -- Add default ordering
    v_query := v_query || ' ORDER BY tmf.TR_DATE, ei.rm_name';

    OPEN o_transactions FOR v_query;
END SP_GET_TRANSACTIONS_PRA;
/
