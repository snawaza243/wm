CREATE OR REPLACE PROCEDURE WEALTHMAKER.ModifyTransactionPUNCHINGPRA (
    p_Tran_Code            IN transaction_mf_temp1.tran_code%TYPE,
    p_Close_Sch_Code       IN transaction_mf_temp1.target_switch_scheme%TYPE,
    p_Ac_Holder_Code       IN transaction_mf_temp1.ac_holder_code%TYPE,
    p_Exp_Rate             IN transaction_mf_temp1.exp_rate%TYPE,
    p_Exp_Amount           IN transaction_mf_temp1.exp_amount%TYPE,
    p_Client_Code          IN transaction_mf_temp1.client_code%TYPE,
    p_Investor_Name        IN transaction_mf_temp1.investor_name%TYPE,
    p_Business_Rm_Code     IN transaction_mf_temp1.business_rmcode%TYPE,
    p_Client_Owner         IN transaction_mf_temp1.client_owner%TYPE,
    p_busi_branch_code     IN transaction_mf_temp1.busi_branch_code%TYPE,
    p_Panno                IN transaction_mf_temp1.panno%TYPE,
    p_Mut_Code             IN transaction_mf_temp1.mut_code%TYPE,
    p_Sch_Code             IN transaction_mf_temp1.sch_code%TYPE,
    p_Tr_Date              IN transaction_mf_temp1.tr_date%TYPE,
    p_Tran_Type            IN transaction_mf_temp1.tran_type%TYPE,
    p_App_No               IN transaction_mf_temp1.app_no%TYPE,
    p_Folio_No             IN transaction_mf_temp1.folio_no%TYPE,
    p_Sip_Start_Date       IN transaction_mf_temp1.sip_start_date%TYPE,
    p_Sip_End_Date         IN transaction_mf_temp1.sip_end_date%TYPE,
    p_Switch_Scheme        IN transaction_mf_temp1.switch_scheme%TYPE,
    p_Switch_Folio         IN transaction_mf_temp1.switch_folio%TYPE,
    p_Payment_Mode         IN transaction_mf_temp1.payment_mode%TYPE,
    p_Amount               IN transaction_mf_temp1.amount%TYPE,
    p_Frequency            IN transaction_mf_temp1.frequency%TYPE,
    p_Installments_No      IN transaction_mf_temp1.installments_no%TYPE,
    p_Sip_Type             IN transaction_mf_temp1.sip_type%TYPE,
    p_Sip_fr               IN transaction_mf_temp1.sip_fr%TYPE,
    p_Dispatch             IN transaction_mf_temp1.dispatch%TYPE,
    p_Source_Code          IN transaction_mf_temp1.source_code%TYPE,
    p_Doc_ID               IN transaction_mf_temp1.doc_id%TYPE,
    p_Micro_Investment     IN transaction_mf_temp1.micro_investment%TYPE,
    p_Cob_Flag             IN transaction_mf_temp1.cob_flag%TYPE,
    p_Freedom_Sip_Flag     IN transaction_mf_temp1.freedom_sip_flag%TYPE,
    p_Swp_Flag             IN transaction_mf_temp1.swp_flag%TYPE,
    p_Drop_Date            IN WM_TRAN_SIP.sip_deactivation_date%TYPE,
    p_loggeruserid         IN transaction_mf_temp1.MODIFY_USER%TYPE
 
) AS
BEGIN
    -- Update the transaction_mf_temp1 table
    UPDATE transaction_mf_temp1
    SET 
        target_switch_scheme = p_Close_Sch_Code,
        switch_scheme = p_Switch_Scheme,
        switch_folio = p_switch_folio,
        AC_HOLDER_CODE = p_Ac_Holder_Code,
        INVESTOR_NAME = p_Investor_Name,
        UPDATE_DATE = SYSDATE,
        MODIFY_DATE = TRUNC(SYSDATE),

        BUSINESS_RMCODE = p_Business_Rm_Code,
        CLIENT_OWNER = p_Client_Owner,
        BUSI_BRANCH_CODE = p_Busi_Branch_Code,
        PANNO = p_Panno,
        MUT_CODE = p_Mut_Code,
        SCH_CODE = UPPER(p_Sch_Code),
        TRAN_TYPE = p_Tran_Type,
        APP_NO = p_App_No,
        FOLIO_NO = p_Folio_No,
        SIP_START_DATE = p_Sip_Start_Date,
        SIP_END_DATE = p_Sip_End_Date,
        PAYMENT_MODE = p_Payment_Mode,
        AMOUNT = p_Amount,
        SIP_TYPE = p_Sip_Type,
        SIP_fr = p_Sip_fr,
        dispatch = p_Dispatch,
        micro_investment = p_Micro_Investment,
        cob_flag = p_Cob_Flag,
        FREEDOM_SIP_FLAG = p_Freedom_Sip_Flag,
        swp_flag = p_Swp_Flag,
        MODIFY_USER = p_loggeruserid

        WHERE TRAN_CODE = p_Tran_Code;

    -- Update WM_TRAN_SIP table
    UPDATE WM_TRAN_SIP
    SET 
        AMOUNT_SIP = p_Amount,
        TOTAL_SIP = p_Installments_No
      WHERE base_tran_code = p_Tran_Code;

   UPDATE investor_master
SET 
    MODIFY_DATE = SYSDATE,
    pan = p_Panno,
    MODIFY_USER = p_loggeruserid
WHERE inv_code = (
    SELECT CLIENT_CODE 
    FROM TRANSACTION_MF_TEMP1 
    WHERE TRAN_CODE = p_Tran_Code
)
AND EXISTS (
    SELECT 1 
    FROM TRANSACTION_MF_TEMP1 
    WHERE TRAN_CODE = p_Tran_Code
) 
AND p_Amount > 3000;


    -- Call the procedure for SIP transaction
    IF p_Tran_Type = 'PURCHASE' THEN
        EXECUTE IMMEDIATE 'CALL PRCINSERTWMTRANSIP(:1, :2)' USING p_Tran_Code, 2;
      --  IF p_Update_User = 29 THEN
      --      EXECUTE IMMEDIATE 'CALL PRCINSERTWMTRANSIP_HIS(:1, :2)' USING p_Tran_Code, p_Update_User;
      --  END IF;
    END IF;

    -- Update SIP deactivation date
    IF p_Drop_Date IS NOT NULL THEN
        UPDATE WM_TRAN_SIP
        SET SIP_DEACTIVATION_DATE = p_Drop_Date
        WHERE base_tran_code = p_Tran_Code;
    ELSE
        UPDATE WM_TRAN_SIP
        SET SIP_DEACTIVATION_DATE = NULL
        WHERE base_tran_code = p_Tran_Code;
    END IF;

    -- Commit the transaction
    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20001, SQLERRM);
END ModifyTransactionPUNCHINGPRA;
/