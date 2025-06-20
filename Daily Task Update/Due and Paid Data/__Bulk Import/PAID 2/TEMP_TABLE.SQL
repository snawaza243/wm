-- Drop existing temporary tables if they exist
DECLARE
    v_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO v_count FROM user_tables WHERE table_name = 'TEMP_PAID_IMPORT';
    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'DROP TABLE TEMP_PAID_IMPORT';
    END IF;
    
    SELECT COUNT(*) INTO v_count FROM user_tables WHERE table_name = 'TEMP_POLICY_UPDATES';
    IF v_count > 0 THEN
        EXECUTE IMMEDIATE 'DROP TABLE TEMP_POLICY_UPDATES';
    END IF;
END;
/

-- Create temporary tables
CREATE GLOBAL TEMPORARY TABLE TEMP_PAID_IMPORT (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50),
    status_cd VARCHAR2(20),
    mon_no NUMBER,
    year_no NUMBER,
    importdatatype VARCHAR2(50),
    user_id VARCHAR2(50),
    import_dt DATE
) ON COMMIT PRESERVE ROWS;

CREATE GLOBAL TEMPORARY TABLE TEMP_POLICY_UPDATES (
    policy_no VARCHAR2(50),
    company_cd VARCHAR2(50),
    new_status VARCHAR2(1)
) ON COMMIT PRESERVE ROWS;