CREATE OR REPLACE PROCEDURE WEALTHMAKER.ChkDupTranPunPRA(
    p_Mut_Code       IN VARCHAR2,
    p_App_No         IN VARCHAR2,
    p_Tran_Code      IN VARCHAR2,
    p_Base_Tran_Code IN VARCHAR2,
    p_Tran_Type      IN VARCHAR2,
    o_Duplicate      OUT VARCHAR2
) AS
    v_Count NUMBER;
BEGIN
    IF p_Tran_Type = 'PURCHASE' THEN
        IF p_Base_Tran_Code IS NULL OR p_Base_Tran_Code = '0' THEN
            SELECT COUNT(*) INTO v_Count 
            FROM transaction_mf_Temp1
            WHERE mut_code = p_Mut_Code
              AND app_no = p_App_No
              AND tran_code <> p_Tran_Code
              AND MOVE_FLAG1 IS NULL
              AND SIP_ID IS NULL
              AND (ASA <> 'C' OR ASA IS NULL);
        ELSE
            SELECT COUNT(*) INTO v_Count 
            FROM transaction_mf_Temp1
            WHERE mut_code = p_Mut_Code
              AND app_no = p_App_No
              AND tran_code <> p_Tran_Code
              AND tran_code <> p_Base_Tran_Code
              AND (ASA <> 'C' OR ASA IS NULL);
        END IF;

        IF v_Count > 0 THEN
            o_Duplicate := 'Yes';
        ELSE
            o_Duplicate := 'No';
        END IF;
    ELSE
        o_Duplicate := 'No';
    END IF;
END ChkDupTranPunPRA;
/
