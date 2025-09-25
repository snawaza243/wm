CREATE OR REPLACE PROCEDURE WEALTHMAKER.AddTransactionPUNCHINGPRA (
    p_sip_amount            IN NUMBER,
    p_client_code           IN NUMBER,
    p_business_rmcode       IN NUMBER,
    p_client_owner          IN NUMBER,
    p_busi_branch_code      IN NUMBER,
    p_panno                 IN VARCHAR2,
    p_mut_code              IN VARCHAR2,
    p_sch_code              IN VARCHAR2,
    p_tr_date               IN DATE,
    p_tran_type             IN VARCHAR2,
    p_app_no                IN VARCHAR2,
    p_sip_start_date        IN DATE,
    p_pan                   IN VARCHAR2,
    p_folio_no              IN VARCHAR2,
    p_switch_folio          IN VARCHAR2,
    p_switch_scheme         IN VARCHAR2,
    p_payment_mode          IN CHAR,
    p_bank_name             IN VARCHAR2,
    p_cheque_no             IN VARCHAR2,
    p_cheque_date           IN DATE,
    p_amount                IN NUMBER,
    p_sip_type              IN VARCHAR2,
    p_investor_name         IN VARCHAR2,
    p_exp_rate              IN NUMBER,
    p_exp_amount            IN NUMBER,
    p_ac_holder_code        IN VARCHAR2,
    p_frequency             IN NUMBER,
    p_installments_no       IN NUMBER,
    p_timestamp             IN DATE,
    p_sip_end_date          IN DATE,
    p_sip_fr                IN CHAR,
    p_dispatch              IN CHAR,
    p_doc_id                IN VARCHAR2,
    p_micro_investment      IN VARCHAR2,
    p_target_switch_scheme  IN VARCHAR2,
    p_cob_flag              IN CHAR,
    p_swp_flag              IN CHAR,
    p_freedom_sip_flag      IN CHAR,
    p_loggeruserid          IN VARCHAR2,
    p_source                IN NUMBER,
    p_microflag             IN VARCHAR2,
    p_tran_code             OUT VARCHAR2  -- Added OUT parameter
) IS
    v_scheme_code scheme_info.sch_code%TYPE;
     v_tran_code VARCHAR2(50); 
BEGIN
    -- Retrieve scheme code dynamically if p_switch_scheme is provided
    IF p_switch_scheme IS NOT NULL and upper(p_tran_type) = 'SWITCH IN' THEN
        BEGIN
            SELECT si.sch_code 
            INTO v_scheme_code
            FROM scheme_info si
            WHERE si.sch_code = p_switch_scheme;

        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                RAISE_APPLICATION_ERROR(-20001, 'No matching scheme code found for the given switch scheme.');
        END;
    ELSE
        v_scheme_code := NULL;
    END IF;

    -- Insert into transaction_mf_temp1 and retrieve generated TRAN_CODE
    INSERT INTO transaction_mf_temp1 (
        FAMILY_HEAD , microsip, sip_amount, client_code, business_rmcode, client_owner, busi_branch_code, 
        panno, mut_code, sch_code, tr_date, tran_type, app_no, sip_start_date, 
        pan, folio_no, switch_folio, switch_scheme, payment_mode, bank_name, 
        cheque_no, cheque_date, amount, sip_type, investor_name, exp_rate, 
        exp_amount, ac_holder_code, frequency, installments_no, timest, sip_end_date, 
        sip_fr, dispatch, doc_id, micro_investment, target_switch_scheme, cob_flag, 
        swp_flag, freedom_sip_flag , loggeduserid , source_code
    ) VALUES (
        (SELECT MIN(INV_CODE) FROM INVESTOR_MASTER WHERE SOURCE_ID = SUBSTR(p_client_code, 1, 8)),  p_microflag, p_sip_amount, p_client_code, p_business_rmcode, p_client_owner, p_busi_branch_code, 
        p_panno, p_mut_code, p_sch_code, p_tr_date, p_tran_type, p_app_no, p_sip_start_date, 
        p_pan, p_folio_no, p_switch_folio, 
        v_scheme_code,  -- Using scheme code if found, else NULL
        p_payment_mode, p_bank_name, p_cheque_no, p_cheque_date, p_amount, 
        p_sip_type, p_investor_name, p_exp_rate, p_exp_amount, p_ac_holder_code, 
        p_frequency, p_installments_no, p_timestamp, p_sip_end_date, p_sip_fr, 
        p_dispatch, p_doc_id, p_micro_investment, p_target_switch_scheme, p_cob_flag, 
        p_swp_flag, p_freedom_sip_flag , p_loggeruserid , p_source
    )RETURNING tran_code INTO v_tran_code;  -- Storing in local variable

    -- Assign transaction code to OUT parameter
    p_tran_code := v_tran_code;

    WEALTHMAKER.UPDATE_MF_MARGIN_TRAN(v_tran_code);    COMMIT;
END AddTransactionPUNCHINGPRA;
/