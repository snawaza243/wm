CREATE OR REPLACE PROCEDURE PSM_DAP_DUE_UP_DUP_POL(
    p_month_no IN NUMBER,
    p_year_no IN NUMBER,
    p_import_data_type IN VARCHAR2
) IS
    v_policy_no VARCHAR2(50);
    v_dup_policy_str VARCHAR2(4000) := '';
    v_rec_del NUMBER := 0;
BEGIN
    -- Delete existing records in DUP_POLICY
    EXECUTE IMMEDIATE 'DELETE FROM DUP_POLICY';

    -- Loop through the duplicate policies
    FOR rec IN (
        SELECT DISTINCT a.policy_no
        FROM bajaj_Ar_head a
        JOIN bajaj_due_Data b
        ON UPPER(TRIM(a.COMPANY_CD)) = UPPER(TRIM(b.COMPANY_CD))
        AND a.Policy_No = b.Policy_No
        WHERE b.mon_no = p_month_no
        AND b.year_no = p_year_no
        AND b.importdatatype = p_import_data_type
        GROUP BY a.policy_no, a.sys_ar_dt, a.company_Cd
        HAVING COUNT(a.policy_no) > 1 
        AND COUNT(a.sys_ar_dt) > 1
        AND COUNT(a.company_Cd) > 1
    ) LOOP
        -- Insert duplicate policy into DUP_POLICY table
        EXECUTE IMMEDIATE 'INSERT INTO DUP_POLICY (policy_no) VALUES (:1)' USING rec.policy_no;

        -- Build the string of policy numbers
        v_dup_policy_str := v_dup_policy_str || '''' || rec.policy_no || ''',';
    END LOOP;

    -- Remove the trailing comma from the string
    IF LENGTH(v_dup_policy_str) > 0 THEN
        v_dup_policy_str := RTRIM(v_dup_policy_str, ',');
    END IF;

    -- Update duplicate records in bajaj_due_Data table
    IF LENGTH(v_dup_policy_str) > 0 THEN
        EXECUTE IMMEDIATE 'UPDATE bajaj_due_Data 
                           SET dup_rec = ''Y'' 
                           WHERE policy_no IN (' || v_dup_policy_str || ') 
                           AND mon_no = :1 
                           AND year_no = :2 
                           AND importdatatype = :3'
        USING p_month_no, p_year_no, p_import_data_type;
    END IF;

    -- Optionally, return the number of records deleted or processed
    -- For example: 
    -- SELECT COUNT(*) INTO v_rec_del FROM DUP_POLICY;

END PSM_DAP_DUE_UP_DUP_POL;
