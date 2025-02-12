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
BEGIN
    -- Retrieve data into variables
    BEGIN
        SELECT NVL(ar_code, '0'),
               NVL(INV_CODE, 0),
               NVL(BUSI_BRANCH_CODE, 0),
               NVL(BUSI_RM_CODE, 0),
               NVL(common_id, 0)
        INTO at_flag,
             inv_flag,
             busi_branch_flag,
             busi_rm_flag,
             dt_flag
        FROM tb_doc_upload
        WHERE tran_type = 'FI' AND common_id = p_dtnumber AND ROWNUM = 1;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            dt_flag := 0; -- Handle no data case
    END;

    -- Check if data exists or is valid
    IF p_dtnumber IS NOT NULL THEN
        IF dt_flag IS NULL OR dt_flag = 0 THEN
            -- Return an error message
            OPEN result_cursor FOR
                SELECT 'Incorrect DT' AS message
                FROM DUAL;
            RETURN;
        END IF;

        IF p_arnumber IS NULL THEN
            -- Return data from tb_doc_upload
            OPEN result_cursor FOR
                SELECT 
                 'Validity: DT is valid' AS message,
                 NVL(ar_code, '0') AS ar_code,
                 NVL(INV_CODE, 0) AS inv_code,
                 NVL(BUSI_BRANCH_CODE, 0) AS busi_branch_code,
                 NVL(BUSI_RM_CODE, 0) AS busi_rm_code,
                 NVL(common_id, 0) AS common_id,
                 NVL((select investor_name from investor_master where investor_master.inv_code = INV_CODE and rownum = 1), 0) as INV_NAME
                FROM tb_doc_upload
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
                    transaction_sttemp.*
                    FROM transaction_sttemp
                    WHERE tran_code = p_arnumber;
            ELSE
                -- Return data from transaction_st
                OPEN result_cursor FOR
                    SELECT 
                    'Validity: Transaction data exist in st' AS message,

                        ST.CORPORATE_NAME as CORPORATE_NAME,
                        --ST.DOC_ID as DOC_ID,
                        nvl((select common_id from  tb_doc_upload where tran_type='FI' and ar_code =  p_arnumber and rownum = 1), null) as DOC_ID,
                        nvl((select investor_name from investor_master where inv_code = (select inv_code from tb_doc_upload where ar_code = st.tran_code)),'nope') as INV_NAME,
                        nvl((select AMOUNT1 from NPS_TRANSACTION where tran_code = p_arnumber),'0')as AMOUNT1,--ST.AMOUNT1 as AMOUNT1,
                        nvl((select amount2 from NPS_TRANSACTION where tran_code = p_arnumber),'0')as AMOUNT2,
                        nvl((select REG_CHARGE from NPS_TRANSACTION where tran_code = p_arnumber),'0') as REG_CHARGE,
                        nvl((select Tran_CHARGE from NPS_TRANSACTION where tran_code = p_arnumber),'0') as Tran_CHARGE,
                        nvl((select SERVICETAX from NPS_TRANSACTION where tran_code = p_arnumber),'0')  as SERVICETAX,    
                        --'tm1' as AMOUNT2,
                        --'tm2'  as REG_CHARGE,
                        --'tm3'  as Tran_CHARGE,
                        'tm4'   as SERVICETAX, 
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