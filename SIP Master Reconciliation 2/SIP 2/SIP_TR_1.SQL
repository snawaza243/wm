CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_SIP_FIND_TR (
    P_X              IN VARCHAR2,                           -- 1,2,NULL         RTA/TR/TRAN
    p_branch_cat     IN NUMBER,                             -- BRANCH_CATEGORY  TR
    p_region         IN branch_master.region_id%TYPE,       -- REGION_CODE      TR
    p_zone           IN branch_master.zone_id%TYPE,         -- ZONE_CODE        TR
    p_branch         IN branch_master.branch_code%TYPE,     -- BRANCH_CODE      TR
    p_rm             IN transaction_mf_temp1.rmcode%TYPE,   -- RM_CODE          TR
    p_amc            IN mut_fund.mut_name%TYPE,             -- AMC_CODE         TR
    p_tran_type      IN VARCHAR2,                           -- SIP,RENEWAL      TR/
    p_registrar      IN VARCHAR2,                           -- C,K,CCOB,KCOB    TR/
    p_status_type    IN CHAR,                               -- Y,N (RECONCILE)  TR/
    p_pms            IN CHAR,                               -- Y,N (CHECKED)    TR/
    p_cob            IN CHAR,                               -- Y,N (CHECKED)    TR/
    p_date_from      IN VARCHAR2,                           -- DD/MM/YYYY       TR/
    p_date_to        IN VARCHAR2,                           -- DD/MM/YYYY       TR/
    p_ar_num         IN VARCHAR2,                           -- TRXXXXXXX        TR/
    p_cheque_type    IN VARCHAR2,                           -- 0,1,2,3,4        RTA
    p_cheque_search  IN VARCHAR2,                           -- VALUES           RTA
    P_INVESTOR_NAME  IN VARCHAR2,                           -- INVESTOR NAME    RTA
    P_AMOUNT         IN NUMBER,                             -- AMOUNT           RTA
    p_sip_folio_no   IN VARCHAR2,                           -- SIP FOLIO NO     SIP
    p_sip_amount     IN NUMBER,                             -- SIP AMOUNT       SIP
    p_sip_pan        IN VARCHAR2,                           -- SIP PAN          SIP
    P_SIP_CLIENT_CODE IN VARCHAR2,                          -- SIP CLIENT CODE  SIP
    P_SIP_DATE       IN VARCHAR2,                           -- SIP DATE        SIP
    P_LOG_ID         IN VARCHAR2,                           -- LOG ID           
    P_ROLE_ID        IN VARCHAR2,                           -- ROLE ID          
    P_CURSOR         OUT SYS_REFCURSOR  
) AS

/*

Optcams = p_registrar (C)

*/

    v_query VARCHAR2(4000);
    v_srm   VARCHAR2(100);
    v_base_recon VARCHAR2(100);
BEGIN

    IF P_X = '1' THEN
        v_query := ' SELECT   FOLIO_NO, SCH_CODE, TR_DATE, TRAN_TYPE, ';
        v_query := v_query || '         MAX (INVESTOR_NAME) INVESTOR_NAME, MAX (ADDRESS) ADDRESS, ';
        v_query := v_query || '         MAX (BROKER_CODE) BROKER_CODE, MAX (CITY_NAME) CITY_NAME, MAX(SCH_NAME)SCH_NAME, ';
        v_query := v_query || '         MAX(MUT_CODE)MUT_CODE, MAX(MUT_NAME)MUT_NAME, MAX(RM_NAME)RM_NAME, MAX(BRANCH_NAME)BRANCH_NAME, MAX (APP_NO) APP_NO, ';
        v_query := v_query || '         MAX (CHEQUE_NO) CHEQUE_NO, SUM (AMOUNT) AMOUNT, ';
        v_query := v_query || '         MAX (BUSI_BRANCH_CODE) BUSI_BRANCH_CODE, ';
        v_query := v_query || '         MAX (BUSINESS_RMCODE) BUSINESS_RMCODE,';
        if upper(p_registrar) = 'C' THEN
            v_query := v_query || ' GETTRANCODE_NEW(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE,tran_id)UNIQUE_TRAN , ';
        else
            v_query := v_query || ' GETTRANCODE(FOLIO_NO,SCH_CODE,TR_DATE,TRAN_TYPE)UNIQUE_TRAN , ';
        end if;
        v_query := v_query || ' max(reg_trantype) reg_trantype,max(unq_key)unq_key ';
        v_query := v_query || '   FROM (  ';
        v_query := v_query || ' SELECT t.tran_id,tran_type, t.inv_name Investor_Name,(i.ADDRESS1||'',''||i.ADDRESS2||'',''||i.EMAIL) address,  ';
        v_query := v_query || '         REG_SUBBROK broker_code,c.city_name,  ';
        v_query := v_query || '         t.sch_code,sch_name sch_name,t.mut_code,amc.mut_name mut_name, rm_name,   ';
        v_query := v_query || '         branch_name,   ';
        v_query := v_query || '         app_no,folio_no, cheque_no,  ';
        v_query := v_query || '         amount,  t.tran_code,reg_date as tr_date,  ';
        v_query := v_query || '         busi_branch_code, business_rmcode, t.reg_trantype,t.unq_key ';
        v_query := v_query || '    FROM employee_master e,  ';
        v_query := v_query || '         branch_master b,  ';
        v_query := v_query || '         mut_fund amc,  ';
        v_query := v_query || '         scheme_info sch,  ';
        v_query := v_query || '         TRANSACTION_ST@MF.BAJAJCAPITAL t,  ';
        v_query := v_query || '         investor_master i,CITY_MASTER C  ';
        v_query := v_query || '   WHERE I.CITY_ID=C.CITY_ID(+)  ';
        v_query := v_query || '     AND t.client_code = i.inv_code  ';
        v_query := v_query || '     AND to_char(t.rmcode) = e.rm_code  ';
        v_query := v_query || '     AND t. BUSI_BRANCH_CODE = b.branch_code    ';
        v_query := v_query || '     AND t.mut_code = amc.mut_code AND t.sch_code = sch.sch_code AND DUP_FLAG2=0  ';

        IF P_BRANCH IS NOT NULL AND P_BRANCH != 'ALL' THEN
            V_QUERY := V_QUERY || ' and BUSI_BRANCH_CODE in ( ''' || P_BRANCH || ''')';
        ELSE
            V_QUERY := V_QUERY || ' and BUSI_BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='''||P_LOG_ID||''' AND ROLE_ID='''||P_ROLE_ID||''') ';
        END IF;

        IF v_srm IS NOT NULL THEN
            v_query := v_query || ' and e.rm_code = ''' || v_srm || '''';
        END IF;

        IF p_branch_cat IS NOT NULL THEN
            v_query := v_query || ' and b.branch_tar_cat=''' || p_branch_cat || ''' ';
        END IF;

        IF p_amc IS NOT NULL THEN
            v_query := v_query || '  and to_char(t.mut_code) = ''' || p_amc || '''';
        END IF;

        IF p_status_type IS NOT NULL THEN
            IF UPPER(p_status_type) = 'Y' THEN
                v_query := v_query || ' and t.rec_flag =''Y'' ';
            ELSIF UPPER(p_status_type) = 'N' THEN
                v_query := v_query || ' and (t.rec_flag =''N'' or rec_flag is null) ';
            END IF;
        END IF;

        IF p_tran_type IS NOT NULL THEN
            IF UPPER(p_tran_type) = 'SIP' THEN
                v_query := v_query || ' AND ((UPPER(t.REG_TRANTYPE)  LIKE ''%SYS%'' OR UPPER(t.REG_TRANTYPE)  LIKE ''%SIP%'')) ';
            ELSIF UPPER(p_tran_type) = 'REGULAR' THEN
                v_query := v_query || '  AND ((UPPER(t.REG_TRANTYPE) NOT LIKE ''%SYS%'' AND UPPER(t.REG_TRANTYPE) NOT LIKE ''%SIP%'') OR t.REG_TRANTYPE IS NULL) ';
            END IF;
        END IF;

        IF p_date_from IS NOT NULL THEN
            v_query := v_query || ' AND TR_DATE >= TO_DATE(''' || p_date_from || ''', ''DD/MM/YYYY'')';
        END IF;

        IF p_date_to IS NOT NULL THEN
            v_query := v_query || ' AND TR_DATE <= TO_DATE(''' || p_date_to || ''', ''DD/MM/YYYY'')';
        END IF;


        IF P_CHEQUE_TYPE IS NOT NULL AND P_CHEQUE_SEARCH IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND (';
            IF P_CHEQUE_TYPE = '0' THEN
                V_QUERY := V_QUERY || ' t.CHEQUE_NO = ''' || P_CHEQUE_SEARCH || '''';
            ELSIF P_CHEQUE_TYPE = '1' THEN
                V_QUERY := V_QUERY || ' t.FOLIO_NO = ''' || P_CHEQUE_SEARCH || '''';
            ELSIF P_CHEQUE_TYPE = '2' THEN
                V_QUERY := V_QUERY || ' t.APP_NO = ''' || P_CHEQUE_SEARCH || '''';
            ELSIF P_CHEQUE_TYPE = '3' THEN
            V_QUERY := V_QUERY || ' (UPPER(t.PAN1) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN2) = UPPER(''' || P_CHEQUE_SEARCH || ''') OR UPPER(t.PAN3) = UPPER(''' || P_CHEQUE_SEARCH || ''')) ';
            ELSIF P_CHEQUE_TYPE = '4' THEN
                V_QUERY := V_QUERY || ' t.REG_SUBBROK = ''' || P_CHEQUE_SEARCH || '''';
            END IF;
            V_QUERY := V_QUERY || ')';
        END IF;

        IF P_INVESTOR_NAME IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND UPPER(TRIM(t.INV_NAME)) LIKE ''%' || REPLACE(UPPER(P_INVESTOR_NAME),' ', '%') || '%''';
        END IF;

        IF P_AMOUNT IS NOT NULL THEN
            V_QUERY := V_QUERY || ' AND abs(round(t.amount)) = ' || ABS(ROUND(TO_NUMBER(P_AMOUNT)));
        END IF;


        V_QUERY := V_QUERY || '   AND LPAD (t.mut_code, 2) = ''MF''  ';
        V_QUERY := V_QUERY || '      AND (t.asa <> ''C'' OR t.asa IS NULL)  ';
        V_QUERY := V_QUERY || '                                     AND t.tran_type IN  ';
        V_QUERY := V_QUERY || '                                            (''PURCHASE'', ''REINVESTMENT'',  ';
        V_QUERY := V_QUERY || '                                             ''SWITCH IN'')  ';
        V_QUERY := V_QUERY || ' ORDER BY  ';
        V_QUERY := V_QUERY || '  TR_Date,rm_name ';

        V_QUERY := V_QUERY || '  )  ';
        V_QUERY := V_QUERY || ' GROUP BY FOLIO_NO,  ';
        V_QUERY := V_QUERY || '          SCH_CODE,  ';
        V_QUERY := V_QUERY || '          TR_DATE,  ';

        If upper(p_registrar) = 'C' Then
            V_QUERY := V_QUERY || '          TRAN_TYPE,tran_id ';
        Else
            V_QUERY := V_QUERY || '          TRAN_TYPE';
        END IF;


    ELSE IF P_X = '2' THEN
        
        v_query := ' select inv.Investor_Name,inv.address1||'',''||inv.address2||'',''||inv.phone||'',''||inv.email address ,ct.city_name ';
        v_query := v_query ||  ' ,mf.bank_name,mf.client_code,mf.sch_code sch_code,mf.mut_code,rm_name,branch_name,panno,amc.mut_name mut_Name,Sch_Name Sch_Name,tr_date,TRAN_TYPE,App_No,folio_no,payment_mode, cheque_no, CHEQUE_DATE,';
        v_query := v_query ||  ' Amount,Sip_Amount,Sip_Type,lEAD_nO,LEAD_NAME,TRAN_code,b.branch_code,BUSINESS_RMCODE,mf.INSTALLMENTS_NO,RETURNSTATUS(MF.TRAN_CODE)STATUS, CASE WHEN  mf.loggeduserid=''MFONLINE'' THEN ''Online'' WHEN  mf.loggeduserid=''Valuefy'' THEN ''Online'' ELSE ''Offline'' end loggeduserid ';
        v_query := v_query ||  ' from city_master ct,employee_master e,investor_master inv,branch_master b,ALLCOMPANY amc,ALLSCHEME sch,TRANSACTION_MF_TEMP1  mf ';
        v_query := v_query ||  ' where  MF.SIP_TYPE=''SIP'' AND AMOUNT>0 AND (MF.ASA<>''C'' OR MF.ASA IS NULL) and move_flag1 is null and sip_id is null AND inv.city_id=ct.city_id(+) and mf.client_code=inv.inv_code and  to_char(mf.BUSINESS_RMCODE)=to_char(e.payroll_id) and mf.BUSI_BRANCH_CODE=b.branch_code  ';

        IF P_BRANCH IS NOT NULL AND P_BRANCH != 'ALL' THEN
            V_QUERY := V_QUERY || ' and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in ( ''' || P_BRANCH || ''')';
        ELSE
            V_QUERY := V_QUERY || ' and mf.mut_code=amc.mut_code and mf.sch_code=sch.sch_code and b.branch_code in  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE LOGIN_ID='''||P_LOG_ID||''' AND ROLE_ID='''||P_ROLE_ID||''') ';
        END IF;

        IF p_branch_cat IS NOT NULL THEN
            v_query := v_query || ' and b.branch_tar_cat=''' || p_branch_cat || ''' ';
        END IF;

        IF p_region IS NOT NULL THEN
            v_query := v_query || '  and b.region_id = ''' || p_region || '''';
        END IF;

        IF p_zone IS NOT NULL THEN
            v_query := v_query || ' and b.zone_id = ''' || p_zone || '''';
        END IF;

        IF p_rm IS NOT NULL THEN
            v_query := v_query || ' and e.payroll_id = ''' || p_rm || '''';
        END IF;

        IF v_srm IS NOT NULL THEN
            v_query := v_query || ' AND ei.rm_code = ''' || v_srm || '''';
        END IF;

        IF p_ar_num IS NOT NULL THEN
            v_query := v_query || ' and mf.tran_code = ''' || p_ar_num || '''';
        END IF;

        IF p_tran_type IS NOT NULL THEN
            IF UPPER(p_tran_type) = 'SIP' THEN
                v_query := v_query || ' AND (MF.SIP_TYPE=''SIP'' AND (SIP_FR=''F'' OR SIP_FR IS NULL)) ';
            ELSIF UPPER(p_tran_type) = 'REGULAR' THEN
                v_query := v_query || '  AND (SIP_FR=''R'') ';
            END IF;
        END IF;

        IF p_pms = 'Y' THEN
            v_query := v_query || '  AND ((sch.prd=''DT027'') or sch.sch_code in(select sch_code From scheme_info where nature=''ETF'')) ';
        END IF;

        IF p_amc IS NOT NULL THEN
            v_query := v_query || ' and mf.mut_code =''''' || p_amc || ''''' ';
        END IF;

        IF p_status_type IS NOT NULL THEN
            IF UPPER(p_status_type) = 'Y' THEN
                v_query := v_query || ' and nvl(mf.rec_flag,''N'')=''Y'' ';
            ELSIF UPPER(p_status_type) = 'N' THEN
                v_query := v_query || ' and nvl(mf.rec_flag,''N'')=''N''';
            END IF;
        END IF;

        IF p_cob = 'Y' THEN
            v_query := v_query || ' and mf.cob_flag=''1'' ';
        END IF;

        IF p_date_from IS NOT NULL THEN
            v_query := v_query || ' AND TR_DATE >= TO_DATE(''' || p_date_from || ''', ''DD/MM/YYYY'')';
        END IF;

        IF p_date_to IS NOT NULL THEN
            v_query := v_query || ' AND TR_DATE <= TO_DATE(''' || p_date_to || ''', ''DD/MM/YYYY'')';
        END IF;

        if v_base_recon IS NOT NULL THEN
            if v_base_recon = 'BASE' then
                v_query := v_query || ' and RETURNSTATUS(MF.TRAN_CODE)=''BASE RECONCILE'' ';
            else IF v_base_recon = 'SIP' THEN
                v_query := v_query || ' and RETURNSTATUS(MF.TRAN_CODE)<>''SIP RECONCILE'' ';
            end if;
        END IF;

        v_query := v_query || ' ORDER BY tmf.TR_DATE, rm_name';
    ELSE

        v_query := ' SELECT A.SEQ_NO,A.MUT_CODE,B.MUT_NAME,A.SCH_CODE,C.SCH_NAME,A.FOLIO_NO,IHNO_TRXNNO,START_DATE SIP_START_DATE,END_DATE SIP_END_DATE, ';
        v_query := v_query || ' SIPREGDATE,SIP_OPTION,AMOUNT_SIP,TOTAL_SIP,RM_NAME,PAYROLL_ID,BRANCH_CODE,BRANCH_NAME FROM  ';
        v_query := v_query || ' TRAN_SIP_FEED A, ';
        v_query := v_query || ' MUT_FUND B, ';
        v_query := v_query || ' SCHEME_INFO C, ';
        v_query := v_query || ' EMPLOYEE_MASTER E, ';
        v_query := v_query || ' BRANCH_MASTER BR ';
        v_query := v_query || ' WHERE A.MUT_CODE=B.MUT_CODE AND A.SCH_CODE=C.SCH_CODE AND BASE_TRAN_CODE IS NULL ';
        v_query := v_query || ' AND A.CLIENT_RM_CODE=E.RM_CODE AND A.CLIENT_BRANCH_CODE=BR.BRANCH_CODE and ok_flag=''OK'' ';


        IF p_sip_folio_no IS NOT NULL THEN
            v_query := v_query || ' AND  A.FOLIO_NO = ''' || p_sip_folio_no || '''';
        END IF;

        IF p_sip_amount IS NOT NULL THEN
            v_query := v_query || ' AND A.AMOUNT_SIP = ''' || round(p_sip_amount) || '''';
        END IF;


        IF p_sip_pan IS NOT NULL THEN
            v_query := v_query || '  AND (upper(A.pan)= ''' || SUBSTR(UPPER(p_sip_pan), 1,10) || '''';
        END IF;

        IF P_SIP_DATE IS NOT NULL THEN
            v_query := v_query || '  and START_DATE BETWEEN ADD_MONTHS(TO_DATE(''' || P_SIP_DATE || ''',''DD/MM/RRRR''),-2) AND ADD_MONTHS(TO_DATE(''' || P_SIP_DATE || ''',''DD/MM/RRRR''),2) ';
        END IF;

        IF p_branch_cat IS NOT NULL THEN
            v_query := v_query || ' and br.branch_tar_cat=''' || p_branch_cat || ''' ';
        END IF;

        IF p_region IS NOT NULL THEN
            v_query := v_query || '  and br.region_id = ''' || p_region || '''';
        END IF;

        IF p_zone IS NOT NULL THEN
            v_query := v_query || ' and br.zone_id = ''' || p_zone || '''';
        END IF;

        IF p_rm IS NOT NULL THEN
            v_query := v_query || ' and e.payroll_id = ''' || p_rm || '''';
        END IF;

        IF v_srm IS NOT NULL THEN
            v_query := v_query || ' and e.rm_code = ''' || v_srm || '''';
        END IF;

        IF p_amc IS NOT NULL THEN
            v_query := v_query || ' and a.mut_code = ''' || p_amc || '''';
        END IF;
    END IF;

    OPEN P_CURSOR FOR v_query;
    --OPEN P_CURSOR FOR select v_query from dual;
END PSM_SIP_FIND_TR;
/