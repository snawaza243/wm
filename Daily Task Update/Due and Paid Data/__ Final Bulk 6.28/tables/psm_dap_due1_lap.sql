-- Step 1: Create empty table with proper structure
CREATE TABLE psm_dap_temp_due1_lap (
    policy_no      VARCHAR2(100),
    company_cd     VARCHAR2(100),
    due_date       VARCHAR2(100),
    mon_no         VARCHAR2(100),
    year_no        VARCHAR2(100),
    status_cd      VARCHAR2(100),
    days_lapsed    VARCHAR2(100),
    last_prem_amt  VARCHAR2(100),
    pay_mode       VARCHAR2(100),
    sa_amt         VARCHAR2(100),
    cl_name        VARCHAR2(100),
    import_date    VARCHAR2(100)
);


ALTER TABLE psm_dap_temp_due1_lap 
ALTER COLUMN days_lapsed [100];