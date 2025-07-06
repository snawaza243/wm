reate or replace PROCEDURE PSM_MPN_InsertExcelPolicyData (
    p_policy_no IN VARCHAR2,
    p_column2 IN VARCHAR2
) 
IS 
BEGIN


    INSERT INTO POLICY_MAP_TEMP1 (POLICY_NO, company_cd) 
    VALUES (p_policy_no, p_column2);

    COMMIT;
END;
