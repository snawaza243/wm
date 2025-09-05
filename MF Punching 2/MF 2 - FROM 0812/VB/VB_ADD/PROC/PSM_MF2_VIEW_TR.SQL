CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MF2_VIEW_TR (
    P_TAB_INDEX           IN VARCHAR2,
    P_BUSINESS_RMCODE_A   IN VARCHAR2,
    P_BRANCHES            IN VARCHAR2,
    P_LOG_ID              IN VARCHAR2,
    P_ROLE_ID             IN VARCHAR2,
    P_FROM_DATE           IN VARCHAR2,
    P_TO_DATE             IN VARCHAR2,
    P_PAN_S               IN VARCHAR2,
    P_CLIENT_S            IN VARCHAR2,
    P_TRAN_NO             IN VARCHAR2,
    P_ANA_CODE            IN VARCHAR2,
    P_CHEQUE_NO           IN VARCHAR2,
    P_APP_NO              IN VARCHAR2,
    P_INSTALLMENTS        IN VARCHAR2,
    P_SIP_TYPE            IN VARCHAR2,
    P_CLIENT_CODE         IN VARCHAR2,
    P_AC_HOLDER_CODE      IN VARCHAR2,
    P_BUSINESS_RMCODE_M   IN VARCHAR2,
    P_PAN_M               IN VARCHAR2,
    P_FREQUENCY_CODE      IN VARCHAR2,
    P_ORDER_BY            IN VARCHAR2,
    P_DESCENDING          IN VARCHAR2,
    P_RESULT              OUT SYS_REFCURSOR
) AS
    V_SQL     CLOB;
BEGIN

    --open P_RESULT for  select * from TRANSACTION_MF_TEMP1 where rownum<=100; return;
    IF P_TAB_INDEX = 0 THEN
        V_SQL := 'SELECT mf.investor_name, mf.bank_name, mf.SCH_CODE, rm_name, sip_type, branch_name, PANNO, ' ||
                'amc.mut_name AS Amc_Name, sch_name AS Scheme_Name, TR_DATE, TRAN_TYPE, App_No, PAYMENT_MODE, ' ||
                'CHEQUE_NO, CHEQUE_DATE, Amount, TRAN_TYPE, lEAD_nO, LEAD_NAME, TRAN_code, b.branch_code, ' ||
                'BUSINESS_RMCODE, mf.frequency, mf.installments_no, micro_investment, target_switch_scheme ' ||
                'FROM employee_master e, branch_master b, mut_fund amc, scheme_info sch, transaction_mf_Temp1 mf ' ||
                'WHERE TO_CHAR(mf.BUSINESS_RMCODE) = TO_CHAR(e.payroll_id) ' ||
                'AND mf.BUSI_BRANCH_CODE = b.branch_code ' ||
                'AND mf.MUT_CODE = amc.mut_code ' ||
                'AND mf.SCH_CODE = sch.sch_code ' ||
                'AND mf.BUSINESS_RMCODE = ''' || P_BUSINESS_RMCODE_A || ''' ' ||
                'AND TRUNC(update_date) = TRUNC(SYSDATE) ';
    ELSE
        V_SQL := 'SELECT * FROM temporaryBusiness WHERE BUSI_BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || P_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||P_ROLE_ID||''') ';

        IF P_FROM_DATE IS NOT NULL AND P_FROM_DATE <> '__/__/____' THEN
            V_SQL := V_SQL || 'AND TR_DATE >= TO_DATE(''' || P_FROM_DATE || ''', ''DD/MM/YYYY'') ';
        END IF;

        IF P_TO_DATE IS NOT NULL AND P_TO_DATE <> '__/__/____' THEN
            V_SQL := V_SQL || 'AND TR_DATE <= TO_DATE(''' || P_TO_DATE || ''', ''DD/MM/YYYY'') ';
        END IF;

        IF P_PAN_S IS NOT NULL THEN
            V_SQL := V_SQL || 'AND PANNO = ''' || P_PAN_S || ''' ';
        END IF;

        IF P_CLIENT_S IS NOT NULL THEN
            V_SQL := V_SQL || 'AND SOURCE_CODE = ''' || P_CLIENT_S || ''' ';
        END IF;

        IF P_TRAN_NO IS NOT NULL THEN
            V_SQL := V_SQL || 'AND TRAN_CODE = ''' || P_TRAN_NO || ''' ';
        END IF;

        IF P_ANA_CODE IS NOT NULL THEN
            V_SQL := V_SQL || 'AND SOURCE_CODE = (SELECT agent_code FROM agent_master WHERE UPPER(exist_code) = UPPER(''' || P_ANA_CODE || ''')) ';
        END IF;

        IF P_CHEQUE_NO IS NOT NULL THEN
            V_SQL := V_SQL || 'AND CHEQUE_NO = ''' || P_CHEQUE_NO || ''' ';
        END IF;

        IF P_APP_NO IS NOT NULL THEN
            V_SQL := V_SQL || 'AND APP_NO = ''' || P_APP_NO || ''' ';
        END IF;

        IF P_INSTALLMENTS IS NOT NULL THEN
            V_SQL := V_SQL || 'AND INSTALLMENTS_NO = ''' || P_INSTALLMENTS || ''' ';
        END IF;

        IF P_SIP_TYPE IS NOT NULL THEN
            V_SQL := V_SQL || 'AND SIP_TYPE = ''' || P_SIP_TYPE || ''' ';
        END IF;

        IF P_CLIENT_CODE IS NOT NULL THEN
            V_SQL := V_SQL || 'AND CLIENT_CODE = ''' || P_CLIENT_CODE || ''' ';
        END IF;

        IF P_AC_HOLDER_CODE IS NOT NULL THEN
            V_SQL := V_SQL || 'AND AC_HOLDER_CODE = ''' || P_AC_HOLDER_CODE || ''' ';
        END IF;

        IF P_BUSINESS_RMCODE_M IS NOT NULL THEN
            V_SQL := V_SQL || 'AND BUSINESS_RMCODE = ''' || P_BUSINESS_RMCODE_M || ''' ';
        END IF;

        IF P_PAN_M IS NOT NULL THEN
            V_SQL := V_SQL || 'AND PANNO = ''' || P_PAN_M || ''' ';
        END IF;

        IF P_FREQUENCY_CODE IS NOT NULL THEN
            V_SQL := V_SQL || 'AND FREQUENCY = ''' || P_FREQUENCY_CODE || ''' ';
        END IF;
    END IF;

    -- Add ORDER BY clause
    V_SQL := V_SQL || 'ORDER BY ';
    IF P_ORDER_BY IS NULL OR P_ORDER_BY = '' THEN
        V_SQL := V_SQL || 'TR_DATE, rm_name ';
    ELSE
        V_SQL := V_SQL || P_ORDER_BY || ' ';
    END IF;

    IF P_DESCENDING = '1' THEN
        V_SQL := V_SQL || 'DESC';
    END IF;

    OPEN P_RESULT FOR V_SQL;
    --OPEN P_RESULT FOR SELECT V_SQL FROM DUAL;

END PSM_MF2_VIEW_TR;
/