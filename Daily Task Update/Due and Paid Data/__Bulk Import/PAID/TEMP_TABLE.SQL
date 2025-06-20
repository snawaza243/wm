-- Create temporary tables
CREATE GLOBAL TEMPORARY TABLE TEMP_PAID_IMPORT (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50),
    status_cd VARCHAR2(20),
    payment_date DATE,
    amount NUMBER(18,2),
    receipt_no VARCHAR2(50),
    mon_no NUMBER,
    year_no NUMBER,
    importdatatype VARCHAR2(50)
) ON COMMIT PRESERVE ROWS;

CREATE GLOBAL TEMPORARY TABLE TEMP_POLICY_UPDATES (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50),
    new_status VARCHAR2(1)
) ON COMMIT PRESERVE ROWS;