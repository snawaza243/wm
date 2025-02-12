create or replace PROCEDURE PSM_NPS_GET_AR_BY_DTTS (
    p_dtnumber      IN VARCHAR2,
    p_arnumber      IN VARCHAR2,
    p_beforemark    IN NUMBER, -- Changed BOOLEAN to NUMBER (0 or 1)
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
        WHERE tran_type = 'FI' AND common_id = p_dtnumber;

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
                NVL(tdu.ar_code, '0') AS ar_code,
                NVL(tdu.INV_CODE, 0) AS inv_code,
                NVL(tdu.BUSI_BRANCH_CODE, 0) AS busi_branch_code,
                NVL(tdu.BUSI_RM_CODE, 0) AS busi_rm_code,
                NVL(tdu.common_id, 0) AS common_id,
                -- Replacing subquery for INV_NAME with a LEFT JOIN
                NVL(IM.investor_name, 'No Investor') AS INV_NAME
            FROM 
                tb_doc_upload tdu
            -- LEFT JOIN with investor_master to fetch INV_NAME
            LEFT JOIN 
                investor_master IM ON IM.inv_code = tdu.INV_CODE
            WHERE 
                tdu.tran_type = 'FI' 
                AND tdu.common_id = p_dtnumber;

            RETURN;
        END IF;
    END IF;

    IF p_arnumber IS NOT NULL THEN
        IF p_dtnumber IS NULL THEN
            IF p_beforemark = 1 THEN -- Handle BOOLEAN-like logic with NUMBER
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
                st.doc_id as DOC_ID,
                NVL(IM.investor_name, 'No Investor Name') AS INV_NAME,
                NVL(NPT.amount1, '0') AS AMOUNT1,
                NVL(NPT.amount2, '0') AS AMOUNT2,
                NVL(NPT.REG_CHARGE, '0') AS REG_CHARGE,
                NVL(NPT.Tran_CHARGE, '0') AS Tran_CHARGE,
                NVL(NPT.SERVICETAX, '0') AS SERVICETAX,
                NVL(NPT.REMARK,'') AS REMARK,
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
                st.*
            FROM 
                transaction_st ST
            LEFT JOIN investor_master IM ON IM.inv_code = ST.CLIENT_CODE
            LEFT JOIN NPS_TRANSACTION NPT ON NPT.tran_code = ST.tran_code
          --  LEFT JOIN tb_doc_upload tdu ON tdu.ar_code = ST.tran_code
            WHERE 
                ST.tran_code = p_arnumber;

            END IF;
            RETURN;
        END IF;
    END IF;

END;