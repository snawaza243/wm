CREATE OR REPLACE PROCEDURE PSMM_INV_SEARCH_ROW_CLICK(

)AS

    v_fct_cd_new_inv varchar2(12);

    v_fim_found varchar2(1);

BEGIN

    v_fim_found :='0';


    V_ALL_INDIA_INV_CODE:= '';

    IF PX_CUR_FORM = 'frmtransactionmf' THEN
        select loggeduserid INTO V_LOG_USER_ID from client_test where client_codekyc= PX_INV_CODE;
        select main_code INTO V_MAIN_CODE from client_test where client_codekyc=PX_INV_CODE;

        IF V_LOG_USER_ID = 'PROC' THEN
            select NVL(UPD_PROC,'N') INTO V_UPD_PROC from client_test where client_codekyc = PX_INV_CODE;

            IF V_UPD_PROC IN ('N', '0') THEN
                OPEN PX_CURSOR SELECT 'Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account (Main Code):' || V_MAIN_CODE || '#OPEN_POPUP#AO' AS MSG FROM DUAL;
                RETURN;
            END IF;
        END IF;


    END IF;

    IF PX_STR_FORM 'Client Transfer' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(px_fct_cd_code, '[^#]+', 1, LEVEL) AS v_px_fct_cd_code
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(px_fct_cd_code, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.v_px_fct_cd_code  THEN
                OPEN PX_CURSOR SELECT 'This Client has already been Added' AS MSG FROM DUAL;
                RETURN;
            END IF;

            v_fct_cd_new_inv:= PX_INV_CODE
        END LOOP;

        IF PX_CAT = 'CLIENT' THEN
            select C.rm_code, rm_name INTO V_FCT_CS_NEW_RM_NAME, V_FCT_CS_NEW_RM_CODE from client_master c,employee_master e where client_code=SUBSTR(PX_INV_CODE, 1,8) and c.rm_code=e.rm_code
        ELSE
            select C.rm_code, rm_name INTO V_FCT_CS_NEW_RM_NAME, V_FCT_CS_NEW_RM_CODE from agent_master c,employee_master e where agent_code= SUBSTR(PX_INV_CODE, 1,8) and c.rm_code=e.rm_code
        END IF;
    END IF;

    IF PX_CUR_FORM = 'frmInvestorMerge' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(px_fim_cmb_client, '[^#]+', 1, LEVEL) AS v_px_fim_cmb_client
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(px_fim_cmb_client, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.v_px_fim_cmb_client THEN
                v_px_fim_new_cmb_cliend:= PX_INV_CODE;
                v_fim_found := '1';
            END IF;

        END LOOP;

        if v_fim_found = '0' then
            OPEN PX_CURSOR SELECT 'This Client is not Having Duplicate Investors' AS MSG FROM DUAL; RETURN;
        end if;

    END IF;

    IF PX_CUR_FORM = 'frmClientInvestmentReport' THEN
        FOR rec IN (
            SELECT REGEXP_SUBSTR(px_FCIR_CD, '[^#]+', 1, LEVEL) AS V_px_FCIR_CD
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(px_FCIR_CD, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.V_px_FCIR_CD THEN
                OPEN PX_CURSOR SELECT 'This Client has already been Added' AS MSG FROM DUAL; RETURN;
            END IF;
        END LOOP;

        V_px_FCIR_NEW_INV:= PX_INV_CODE;

        IF PX_CAT = 'CLIENT' THEN
            select C.rm_code, rm_name INTO V_px_FCIR_RM_CODE, V_px_FCIR_RM_NAME from client_master c,employee_master e where client_code=PX_INV_CODE and c.rm_code=e.rm_code
        ELSE
            select C.rm_code,rm_name INTO V_px_FCIR_RM_CODE, V_px_FCIR_RM_NAME from agent_master c,employee_master e where agent_code=PX_INV_CODE and c.rm_code=e.rm_code

        END IF;
        
    END IF;

    IF PX_CUR_FORM = 'frmClientMerging' THEN 
        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_CM_INVESTORS, '[^#]+', 1, LEVEL) AS V_PX_CM_INVESTORS
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_CM_INVESTORS, '#') + 1
        )
        LOOP
            IF PX_INV_CODE = rec.V_PX_CM_INVESTORS THEN
                OPEN PX_CURSOR SELECT 'This Client has already been Added' AS MSG FROM DUAL; RETURN;
            END IF;
        END LOOP;

        FOR rec IN (
            SELECT REGEXP_SUBSTR(PX_CM_BRANCH_NAMES, '[^#]+', 1, LEVEL) AS V_PX_CM_BRANCH_NAMES
            FROM dual
            CONNECT BY LEVEL <= REGEXP_COUNT(PX_CM_BRANCH_NAMES, '#') + 1
        )
        LOOP
            IF PX_INV_BRANCH_NAME <> rec.V_PX_CM_BRANCH_NAMES THEN

                IF rec.V_PX_CM_BRANCH_NAMES <> 'MF Data Import' AND PX_INV_BRANCH_NAME <> 'MF Data Import' THEN

                OPEN PX_CURSOR SELECT 'The Clients should be of the same Branch' AS MSG FROM DUAL; RETURN;
                END IF;
            END IF;
        END LOOP;

         select sum(cnt) INTO V_CM_MAX_ROW_cnt from(select count(*) cnt from transaction_st where source_code=SUBSTR(PX_INV_CODE,1,8) and (ASA<>'C' or ASA is null) and (flag<>'NEWTRAN' or flag is null) 
          Union All 
         select count(*) cnt from transaction_sttemp where source_code=SUBSTR(PX_INV_CODE,1,8) and (ASA<>'C' or ASA is null) 
          Union All 
        select count(*) cnt from bajaj_ar_head where lpad(client_cd,8)=SUBSTR(PX_INV_CODE,1,8) and (status_cd<>'B' or status_cd is null))
                

        Select last_tran_dt1 INTO V_CM_LAST_TR from client_master where client_code=SUBSTR(PX_INV_CODE,1,8);

        IF V_CM_LAST_TR IS NOT NULL THEN
            V_CM_LAST_TR:= TO_CHAR(V_CM_LAST_TR,'DD/MM/RRRR');
        ELSE
            V_CM_LAST_TR:='';
        END IF;

        Select pincode,creation_date INTO V_CM_PINCODE, V_CM_CREATION_DT from client_master where client_code=SUBSTR(PX_INV_CODE,1,8);
    END IF;

    IF PX_CUR_FORM = 'frmtransactionmf' THEN
        IF PX_INDEX = '0' THEN
            SELECT INV_CODE,SUBSTR(INV_CODE,1,8), INVESTOR_NAME 
            INTO V_MFA_INV_CODE, V_MFA_CLIENT_CODE, M_MFA_INVESTOR_NAME FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

            IF(SUBSTR(PX_INV_CODE,1,1) = '4') THEN
                SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                --select payroll_id INTO V_MFA_BUSI_CODE from employee_master where rm_code=(select rm_code from client_master where client_code=SUBSTR(PX_INV_CODE,1,8));
                select client_code INTO V_MFA_AH_CODE from client_test where client_codekyc=PX_INV_CODE;

                IF V_MFA_AH_CODE IS NULL OR LENGTH(V_MFA_AH_CODE)<6 THEN
                    OPEN PX_CURSOR SELECT 'Account Opening Process For This Client Is Not Done .Punch Account Opening Form to do the Same' AS MSG FROM DUAL; RETURN;
                    
                END IF;
            ELSE
                SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                --select payroll_id INTO V_MFA_BUSI_CODE from employee_master where rm_code=(select rm_code from agent_master where agent_code=SUBSTR(PX_INV_CODE,1,8));
            END IF;
        ELSE IF PX_INDEX = '1' THEN
            SELECT INV_CODE,SUBSTR(INV_CODE,1,8), INVESTOR_NAME 
            INTO V_MFM_INV_CODE, V_MFM_CLIENT_CODE, M_MFM_INVESTOR_NAME FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;

            IF(SUBSTR(PX_INV_CODE,1,1) = '4') THEN
                SELECT PAN INTO V_MFM_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                select payroll_id INTO V_MFM_BUSI_CODE from employee_master where rm_code=(select rm_code from client_master where client_code=SUBSTR(PX_INV_CODE,1,8));
                select client_code INTO V_MFM_AH_CODE from client_test where client_codekyc=PX_INV_CODE;
            ELSE
                SELECT PAN INTO V_MFA_PAN FROM INVESTOR_MASTER WHERE INV_CODE = PX_INV_CODE;
                select client_code INTO V_MFM_AH_CODE from client_test where client_codekyc=PX_INV_CODE;
                select payroll_id INTO V_MFM_BUSI_CODE from employee_master where rm_code=(select rm_code from agent_master where agent_code=SUBSTR(PX_INV_CODE,1,8));

            END IF;

            V_MF_LABEL42:= PX_INV_CODE;



        END IF;



    END IF;


    IF PX_CUR_FORM = 'frmIPO' THEN

    END IF;




END;