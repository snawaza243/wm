create or replace PROCEDURE PSM_NPS_GET_AR_BY_DTTS (
    p_dtnumber      IN VARCHAR2,
    p_arnumber      IN VARCHAR2,
    p_beforemark    IN number,
        p_login_id      in varchar2,
    result_cursor   OUT SYS_REFCURSOR
) AS


    dt_flag            NUMBER;
    at_flag            VARCHAR2(50);
    inv_flag           NUMBER;
    busi_branch_flag   NUMBER;
    busi_rm_flag       NUMBER;

    tb_f_ar            VARCHAR2(50);
    tb_f_inv           VARCHAR2(50);
    tb_f_bss_br        VARCHAR2(50);
    tb_f_bss_rm        VARCHAR2(50);
    tb_f_common        VARCHAR2(50);
    tb_f_rej           VARCHAR2(50);
    tb_f_ver           VARCHAR2(50);
    tb_f_pun           VARCHAR2(50);
    tb_f_sch           VARCHAR2(50);


BEGIN
    -- Retrieve data into variables
    BEGIN
        SELECT ar_code, INV_CODE, BUSI_BRANCH_CODE, BUSI_RM_CODE, common_id, rejection_status, verification_flag, punching_flag, sch_code
            INTO tb_f_ar, tb_f_inv, tb_f_bss_br, tb_f_bss_rm, tb_f_common, tb_f_rej, tb_f_ver, tb_f_pun, tb_f_sch
            FROM tb_doc_upload
            WHERE tran_type = 'FI' 
            AND sch_code IN ('OP#09971', 'OP#09972', 'OP#09973')
            AND common_id = p_dtnumber 
            AND ROWNUM = 1;
        EXCEPTION
        WHEN NO_DATA_FOUND THEN
            tb_f_ar := '0';     tb_f_inv := '0';    tb_f_bss_br := '0';     tb_f_bss_rm := '0'; tb_f_common := '0'; 
            tb_f_rej := '0';    tb_f_ver := '0';    tb_f_pun := '0';        tb_f_sch := '0';
    END;


    -- Check if data exists or is valid
    IF p_dtnumber IS NOT NULL THEN
        IF tb_f_common = '0' THEN
            OPEN result_cursor FOR
                SELECT 'Incorrect DT' AS message
                FROM DUAL;
            RETURN;
        END IF;

        IF p_arnumber IS NULL THEN
            -- Return data from tb_doc_upload
            OPEN result_cursor FOR
                SELECT 
                    'Validity: DT is valid'     AS message, 
                    tdu.ar_code                 as tb_f_ar,
                    tdu.INV_CODE                as tb_f_inv,
                    tdu.BUSI_BRANCH_CODE        as tb_f_bss_br,
                    tdu.BUSI_RM_CODE            as tb_f_bss_rm,
                    tdu.common_id               as tb_f_common,
                    tdu.rejection_status        as tb_f_rej, 
                    tdu.verification_flag       as tb_f_ver, 
                    tdu.punching_flag           as tb_f_pun,
                    tdu.sch_code                as tb_f_sch,
                    NVL((select investor_name from investor_master where investor_master.inv_code = tdu.INV_CODE and rownum = 1), 0) as im_inv_name,
                    NVL((select inv_code from investor_master where investor_master.inv_code = tdu.INV_CODE and rownum = 1), 0) as im_inv_code

                FROM tb_doc_upload tdu
                WHERE tran_type = 'FI' AND common_id = p_dtnumber AND ROWNUM = 1;
            RETURN;
        END IF;
    END IF;

    IF p_arnumber IS NOT NULL THEN
        IF p_dtnumber IS NULL THEN
            IF p_beforemark = 1 THEN
                -- Return data from transaction_sttemp
                OPEN result_cursor FOR
                    SELECT 
                    'Validity: Transaction data exist in temp' AS message,
                    '' as UNIQUE_ID,

                    transaction_sttemp.*
                    FROM transaction_sttemp
                    WHERE tran_code = p_arnumber;
            ELSE
                -- Return data from transaction_st
                OPEN result_cursor FOR
                    SELECT 
                    
                        'Validity: Transaction data exist in st' AS message,
                        ST.CORPORATE_NAME as CORPORATE_NAME,
                        nvl((select common_id from  tb_doc_upload where tran_type='FI' and ar_code =  p_arnumber and rownum = 1), null) as DOC_ID,
                        nvl((select investor_name from investor_master where inv_code = (select inv_code from tb_doc_upload where ar_code = st.tran_code and rownum = 1)),'nope') as INV_NAME,
                        nvl((select AMOUNT1 from NPS_TRANSACTION where tran_code = p_arnumber and rownum = 1),'0')as AMOUNT1,--ST.AMOUNT1 as AMOUNT1,
                        nvl((select amount2 from NPS_TRANSACTION where tran_code = p_arnumber and rownum = 1),'0')as AMOUNT2,
                        nvl((select REG_CHARGE from NPS_TRANSACTION where tran_code = p_arnumber and rownum = 1),'0') as REG_CHARGE,
                        nvl((select Tran_CHARGE from NPS_TRANSACTION where tran_code = p_arnumber and rownum = 1),'0') as Tran_CHARGE,
                        nvl((select SERVICETAX from NPS_TRANSACTION where tran_code = p_arnumber and rownum = 1),'0')  as SERVICETAX,    
                        --'tm1' as AMOUNT2, 'tm2'  as REG_CHARGE,   'tm3'  as Tran_CHARGE,'tm4'   as SERVICETAX, 
                        ST.TRAN_CODE as TRAN_CODE,
                        ST.CLIENT_CODE as CLIENT_CODE,
                        ST.SCH_CODE as SCH_CODE,
                        ST.FOLIO_NO as FOLIO_NO,
                        ST.BUSINESS_RMCODE as BUSINESS_RMCODE,
                        ST.BUSI_BRANCH_CODE as BUSI_BRANCH_CODE,
                        ST.UNIQUE_ID as UNIQUE_ID,
                        ST.PAYMENT_MODE as PAYMENT_MODE,
                        ST.CHEQUE_NO as CHEQUE_NO,
                        ST.CHEQUE_DATE as CHEQUE_DATE,
                        ST.BANK_NAME as BANK_NAME,
                        ST.APP_NO as APP_NO,
                        ST.TR_DATE as TR_DATE,    
                        ST.manual_arno as manual_arno,
                        ST.AMOUNT as AMOUNT,
                        ST.REMARK as REMARK,
                        ST.*
                    FROM
                        transaction_st st
                    WHERE
                        st.tran_code = p_arnumber and rownum = 1;
            END IF;
            RETURN;
        END IF;
    END IF;

END;