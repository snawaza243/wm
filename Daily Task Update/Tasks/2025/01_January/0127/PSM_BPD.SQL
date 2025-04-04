CREATE OR REPLACE PROCEDURE PSM_DAP_INSERT_N_BPD (
    p_COMPANY_CD            IN VARCHAR2,
    p_STATUS_CD             IN VARCHAR2,
    p_LOCATION              IN VARCHAR2,
    p_POLICY_NO             IN VARCHAR2,
    p_CL_NAME               IN VARCHAR2,
    p_PREM_AMT              IN NUMBER,
    p_PAY_MODE              IN VARCHAR2,
    p_DUE_DATE              IN VARCHAR2,--
    p_CL_ADD1               IN VARCHAR2,
    p_CL_ADD2               IN VARCHAR2,
    p_CL_ADD3               IN VARCHAR2,
    p_CL_CITY               IN VARCHAR2,
    p_CL_PIN                IN VARCHAR2,
    p_CL_PHONE1             IN VARCHAR2,
    p_CL_PHONE2             IN VARCHAR2,
    p_CL_MOBILE             IN VARCHAR2,
    p_MON_NO                IN NUMBER,
    p_YEAR_NO               IN NUMBER,
    p_USERID                IN VARCHAR2,
    p_IMPORT_DT             IN VARCHAR2,--
    p_EMP_NO                IN NUMBER,
    p_INV_CD                IN VARCHAR2,
    p_DUP_REC               IN VARCHAR2,
    p_PLY_TERM              IN NUMBER,
    p_CL_ADD4               IN VARCHAR2,
    p_CL_ADD5               IN VARCHAR2,
    p_PLAN_NAME             IN VARCHAR2,
    p_PREM_FREQ             IN VARCHAR2,
    p_DOC                   IN VARCHAR2,  --
    p_BRANCH_CD             IN NUMBER,
    p_AR_BRANCH_CD          IN NUMBER,
    p_PLAN_NO               IN NUMBER,
    p_BPREM_FREQ            IN NUMBER,
    p_LAST_UPDATE_DT        IN VARCHAR2,--
    p_LAST_UPDATE           IN VARCHAR2,
    p_IMPORTDATATYPE        IN VARCHAR2,
    p_NEWINSERT             IN VARCHAR2,
    p_WEALTHMAKER_UPDATE    IN CHAR,
    p_FORCE_FLAG            IN VARCHAR2,
    p_PREM_TERM             IN NUMBER,
    p_FRESH_RENEWAL         IN NUMBER,
    p_SYS_AR_NO             IN VARCHAR2,
    p_AR_GEN                IN CHAR,
    p_ACTIVE_CLIENT         IN NUMBER,
    p_PLAN_RATE             IN NUMBER,
    p_MARGIN                IN NUMBER,
    p_SMS_FLAG              IN CHAR,
    p_UPD_FLAG              IN CHAR,
    p_PLAN_UPD              IN VARCHAR2,
    p_FREQ_UPD              IN VARCHAR2,
    p_P_ADD1                IN VARCHAR2,
    p_P_ADD2                IN VARCHAR2,
    p_P_CITY                IN VARCHAR2,
    p_P_STATE_CD            IN NUMBER,
    p_P_PIN                 IN VARCHAR2,
    p_IADD1                 IN VARCHAR2,
    p_IADD2                 IN VARCHAR2,
    p_ICITY                 IN VARCHAR2,
    p_ISTATE_CD             IN NUMBER,
    p_IPIN                  IN VARCHAR2,
    p_DUE_MONTH_YEAR        IN DATE,
    p_FLAG_13M              IN NUMBER,
    p_LOGIN                 in varchar2
) AS
BEGIN
    
    BEGIN
    INSERT INTO BAJAJ_PAID_DATA (
        COMPANY_CD, STATUS_CD, LOCATION, POLICY_NO, CL_NAME, PREM_AMT, PAY_MODE, DUE_DATE, 
        CL_ADD1, CL_ADD2, CL_ADD3, CL_CITY, CL_PIN, CL_PHONE1, CL_PHONE2, CL_MOBILE, 
        MON_NO, YEAR_NO, USERID, IMPORT_DT, EMP_NO, INV_CD, DUP_REC, PLY_TERM, CL_ADD4, 
        CL_ADD5, PLAN_NAME, PREM_FREQ, DOC, BRANCH_CD, AR_BRANCH_CD, PLAN_NO, BPREM_FREQ, 
        LAST_UPDATE_DT, LAST_UPDATE, IMPORTDATATYPE, NEWINSERT, WEALTHMAKER_UPDATE, 
        FORCE_FLAG, PREM_TERM, FRESH_RENEWAL, SYS_AR_NO, AR_GEN, ACTIVE_CLIENT, PLAN_RATE, 
        MARGIN, SMS_FLAG, UPD_FLAG, PLAN_UPD, FREQ_UPD, P_ADD1, P_ADD2, P_CITY, P_STATE_CD, 
        P_PIN, IADD1, IADD2, ICITY, ISTATE_CD, IPIN, DUE_MONTH_YEAR, FLAG_13M
    )
    VALUES (
        p_COMPANY_CD, p_STATUS_CD, p_LOCATION, p_POLICY_NO, p_CL_NAME, p_PREM_AMT, p_PAY_MODE, TO_DATE(p_DUE_DATE, 'DD/MM/YYYY') , 
        p_CL_ADD1, p_CL_ADD2, p_CL_ADD3, p_CL_CITY, p_CL_PIN, p_CL_PHONE1, p_CL_PHONE2, p_CL_MOBILE, 
        p_MON_NO, p_YEAR_NO, p_USERID, TO_DATE(p_IMPORT_DT, 'DD/MM/YYYY') , p_EMP_NO, p_INV_CD, p_DUP_REC, p_PLY_TERM, p_CL_ADD4, 
        p_CL_ADD5, p_PLAN_NAME, p_PREM_FREQ, TO_DATE(p_DOC, 'DD/MM/YYYY') , p_BRANCH_CD, p_AR_BRANCH_CD, p_PLAN_NO, p_BPREM_FREQ, 
        TO_DATE(p_LAST_UPDATE_DT, 'DD/MM/YYYY') , p_LAST_UPDATE, p_IMPORTDATATYPE, p_NEWINSERT, p_WEALTHMAKER_UPDATE, 
        p_FORCE_FLAG, p_PREM_TERM, p_FRESH_RENEWAL, p_SYS_AR_NO, p_AR_GEN, p_ACTIVE_CLIENT, p_PLAN_RATE, 
        p_MARGIN, p_SMS_FLAG, p_UPD_FLAG, p_PLAN_UPD, p_FREQ_UPD, p_P_ADD1, p_P_ADD2, p_P_CITY, p_P_STATE_CD, 
        p_P_PIN, p_IADD1, p_IADD2, p_ICITY, p_ISTATE_CD, p_IPIN, p_DUE_MONTH_YEAR, p_FLAG_13M
    );
   COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20002, 'Insert failed: ' || SQLERRM);
    END;
END PSM_DAP_INSERT_N_BPD;