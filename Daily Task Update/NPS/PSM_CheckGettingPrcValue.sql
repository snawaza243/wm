-- For testing procedure value
/*
OPEN P_RESULT FOR  
SELECT 
    'P_MARK --> ' || P_MARK || ', ' ||
    'P_PRODUCT_CLASS --> ' || P_PRODUCT_CLASS || ', ' ||
    'P_INVESTOR_TYPE --> ' || P_INVESTOR_TYPE || ', ' ||
    'P_CORPORATE_NAME --> ' || P_CORPORATE_NAME || ', ' ||
    'P_DT_NUMBER --> ' || P_DT_NUMBER || ', ' ||
    'P_TRAN_CODE --> ' || P_TRAN_CODE || ', ' ||
    'P_INVESTOR_CODE --> ' || P_INVESTOR_CODE || ', ' ||
    'P_SCHEME_CODE --> ' || P_SCHEME_CODE || ', ' ||
    'P_CRA --> ' || P_CRA || ', ' ||
    'P_CRA_BRANCH --> ' || P_CRA_BRANCH || ', ' ||
    'P_FOLIO_NUMBER --> ' || P_FOLIO_NUMBER || ', ' ||
    'P_BUSINESS_RM --> ' || P_BUSINESS_RM || ', ' ||
    'P_BUSINESS_BRANCH --> ' || P_BUSINESS_BRANCH || ', ' ||
    'P_RECEIPT_NO --> ' || P_RECEIPT_NO || ', ' ||
    'P_PAYMENT_MODE --> ' || P_PAYMENT_MODE || ', ' ||
    'P_CHEQUE_NO --> ' || P_CHEQUE_NO || ', ' ||
    'P_BANK_NAME --> ' || P_BANK_NAME || ', ' ||
    'P_APP_NO --> ' || P_APP_NO || ', ' ||
    'P_CHEQUE_DATE --> ' || P_CHEQUE_DATE || ', ' ||
    'P_DATE --> ' || P_DATE || ', ' ||
    'P_TIME --> ' || P_TIME || ', ' ||
    'P_COMBINED_DATETIME --> ' || P_COMBINED_DATETIME || ', ' ||
    'P_SUBSCRIBER_NAME --> ' || P_SUBSCRIBER_NAME || ', ' ||
    'P_MANUAL_AR_NO --> ' || P_MANUAL_AR_NO || ', ' ||
    'P_UNFREEZE --> ' || P_UNFREEZE || ', ' ||
    'P_AMOUNT_T1 --> ' || P_AMOUNT_T1 || ', ' ||
    'P_AMOUNT_T2 --> ' || P_AMOUNT_T2 || ', ' ||
    'P_RECHARGE1 --> ' || P_RECHARGE1 || ', ' ||
    'P_RECHARGE2 --> ' || P_RECHARGE2 || ', ' ||
    'P_GST_TAX --> ' || P_GST_TAX || ', ' ||
    'P_COLLECTION_AMOUNT --> ' || P_COLLECTION_AMOUNT || ', ' ||
    'P_AMOUNT_INVESTED --> ' || P_AMOUNT_INVESTED || ', ' ||
    'P_AMOUNT_INVESTED2 --> ' || P_AMOUNT_INVESTED2 || ', ' ||
    'P_REMARK --> ' || P_REMARK || ', ' ||
    'P_ZERO_COM --> ' || P_ZERO_COM || ', ' ||
    'P_LOGGEDIN_USER --> ' || P_LOGGEDIN_USER 
AS STATUS FROM DUAL;
