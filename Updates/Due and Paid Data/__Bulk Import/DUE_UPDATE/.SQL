-- Create temporary tables (run once)
CREATE GLOBAL TEMPORARY TABLE TEMP_POLICY_UPDATES_BASE (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50)
) ON COMMIT PRESERVE ROWS;

CREATE GLOBAL TEMPORARY TABLE TEMP_POLICY_UPDATES_PREM_FREQ (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50)
) ON COMMIT PRESERVE ROWS;
-- Create similar tables for other update types if needed