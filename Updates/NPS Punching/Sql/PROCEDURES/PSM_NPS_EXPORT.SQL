CREATE OR REPLACE PROCEDURE PSM_NPS_EXPORT (
    p_export_type   IN VARCHAR2, -- 'ECS', 'NON_ECS', or 'ALL'
    p_from_date     IN VARCHAR2,
    CboKRAImport    IN VARCHAR2,
    p_to_date       IN VARCHAR2, 
    p_cursor        OUT SYS_REFCURSOR
) AS
    v_sql           VARCHAR2(4000);
BEGIN
    -- Validate date parameters
    IF p_export_type IN ('NON_ECS', 'ALL') AND (p_to_date IS NULL OR p_to_date = '__/__/____') THEN
        RAISE_APPLICATION_ERROR(-20001, 'To Date cannot be blank');
    END IF;
    
    -- Build the SQL query based on parameters
    IF p_export_type = 'ECS' THEN 
        v_sql := 'select substr(a.unique_id,3,7)exitsr,manual_arno pran,investor_name,decode(payment_mode,''C'',''CHEQUE'',''E'',''ECS'',''H'',''CASH'',''D'',''DRAFT'')payment_mode,cheque_no,to_char(tr_date,''dd-Mon-yyyy'') ecs_date,bank_name,a.unique_id, c.amount1,c.amount2,reg_charge,tran_charge,Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2) service,(NVL(c.amount1,0) + NVL(c.amount2,0))-(reg_charge+tran_charge+Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2)) amt_inv, ';
        v_sql := v_sql || ' SUBSTR(A.UNIQUE_ID,10,17) RECEIPT_NO_10_17,CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN 58002 ELSE NULL END FC_REGISTRATION_NO, ';
        v_sql := v_sql || ' CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN ''REGISTRATION'' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NOT NULL THEN ''REGISTRATION CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NULL THEN ''NORMAL CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN(''11'',''12'') THEN ''OTHER CONTRIBUTION'' END REMARKS, ';
        v_sql := v_sql || ' CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN ''REGISTRATION'' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NOT NULL THEN ''REGISTRATION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NULL THEN ''NORMAL CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN(''11'',''12'') THEN ''OTHER CONTRIBUTION'' END REMARKS1, ';
        v_sql := v_sql || ' A.TRAN_CODE,REF_TRAN_CODE,CSF_TRANSACTION_ID ';
        v_sql := v_sql || ' from transaction_st a,investor_master b,NPS_TRANSACTION c Where a.tran_code = c.tran_code and a.client_code=b.INV_CODE and to_char(a.tr_date,''dd/mm/rrrr'')=''' || p_from_date || ''' and payment_mode=''E''';

        IF CboKRAImport = '1' THEN
            v_sql := v_sql || ' AND folio_no=''6036914''';
        ELSIF CboKRAImport = '2' THEN
            v_sql := v_sql || ' AND folio_no=''1171966''';
        END IF;
    ELSIF UPPER(p_export_type) = 'NON_ECS' THEN 
        v_sql := 'select substr(a.unique_id,3,7)exitsr,manual_arno pran,investor_name,decode(payment_mode,''C'',''CHEQUE'',''E'',''ECS'',''H'',''CASH'',''D'',''DRAFT'', ''M'',''Corporate NON ECS'')payment_mode,cheque_no,to_char(tr_date,''dd-Mon-yyyy'') ecs_date,bank_name,a.unique_id, c.amount1,c.amount2,reg_charge,tran_charge,Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2) service,(NVL(c.amount1,0) + NVL(c.amount2,0))-(reg_charge+tran_charge+Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2)) amt_inv,';
        v_sql := v_sql || ' SUBSTR(A.UNIQUE_ID,10,17) RECEIPT_NO_10_17,CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN 58002 ELSE NULL END FC_REGISTRATION_NO, ';
        v_sql := v_sql || ' CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN ''REGISTRATION'' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NOT NULL THEN ''REGISTRATION CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NULL THEN ''NORMAL CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN(''11'',''12'') THEN ''OTHER CONTRIBUTION'' END REMARKS, ';
        v_sql := v_sql || ' CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN ''REGISTRATION'' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NOT NULL THEN ''REGISTRATION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NULL THEN ''NORMAL CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN(''11'',''12'') THEN ''OTHER CONTRIBUTION'' END REMARKS1, ';
        v_sql := v_sql || ' A.TRAN_CODE,REF_TRAN_CODE,CSF_TRANSACTION_ID ';
        v_sql := v_sql || ' from transaction_st a,investor_master b,NPS_TRANSACTION c Where a.tran_code = c.tran_code and a.client_code=b.inv_code and a.tr_date between to_date(''' || p_from_date || ''', ''DD-MM-YYYY'') and to_date(''' || p_to_date || ''', ''DD-MM-YYYY'') and payment_mode<>''E''';
    
        IF CboKRAImport = '1' THEN
            v_sql := v_sql || ' AND folio_no=''6036914''';
        ELSIF CboKRAImport = '2' THEN
            v_sql := v_sql || ' AND folio_no=''1171966''';
        END IF;
        
    ELSE 
        v_sql := 'select substr(a.unique_id,3,7)exitsr,manual_arno pran,investor_name,decode(payment_mode,''C'',''CHEQUE'',''E'',''ECS'',''H'',''CASH'',''D'',''DRAFT'', ''M'',''Corporate NON ECS'')payment_mode,cheque_no,to_char(tr_date,''dd-Mon-yyyy'') ecs_date,bank_name,a.unique_id, c.amount1,c.amount2,reg_charge,tran_charge,Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2) service,(NVL(c.amount1,0) + NVL(c.amount2,0))-(reg_charge+tran_charge+Round(((nvl(c.reg_charge,0) + decode(NVL(c.amount2,0),0,0,nvl(c.reg_charge,0))) * 0.18), 2)) amt_inv,';
        v_sql := v_sql || ' SUBSTR(A.UNIQUE_ID,10,17) RECEIPT_NO_10_17,CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN 58002 ELSE NULL END FC_REGISTRATION_NO, ';
        v_sql := v_sql || ' CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN ''REGISTRATION'' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NOT NULL THEN ''REGISTRATION CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NULL THEN ''NORMAL CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN(''11'',''12'') THEN ''OTHER CONTRIBUTION'' END REMARKS, ';
        v_sql := v_sql || ' CASE WHEN SUBSTR(UNIQUE_ID,1,2) =''11'' THEN ''REGISTRATION'' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NOT NULL THEN ''REGISTRATION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) =''12'' AND REF_TRAN_CODE IS NULL THEN ''NORMAL CONTRIBUTION'' ';
        v_sql := v_sql || ' WHEN SUBSTR(UNIQUE_ID,1,2) NOT IN(''11'',''12'') THEN ''OTHER CONTRIBUTION'' END REMARKS1, ';
        v_sql := v_sql || ' A.TRAN_CODE,REF_TRAN_CODE,CSF_TRANSACTION_ID';
        v_sql := v_sql || ' from transaction_st a,investor_master b,NPS_TRANSACTION c Where a.tran_code = c.tran_code and a.client_code=b.inv_code and a.tr_date between to_date(''' || p_from_date || ''', ''DD-MM-YYYY'') and to_date(''' || p_to_date || ''', ''DD-MM-YYYY'')';
        
        IF CboKRAImport = '1' THEN
            v_sql := v_sql || ' AND folio_no=''6036914''';
        ELSIF CboKRAImport = '2' THEN
            v_sql := v_sql || ' AND folio_no=''1171966''';
        END IF;
    END IF;

    -- Open cursor with the dynamic SQL
    OPEN p_cursor FOR v_sql;
END PSM_NPS_EXPORT;
/