CREATE OR REPLACE PROCEDURE PSM_MPN_InsertExcelPolicyData (
    p_policy_no_arr IN SYS.ODCIVARCHAR2LIST,
    p_column2_arr IN SYS.ODCIVARCHAR2LIST
)
IS
BEGIN
    FORALL i IN 1 .. p_policy_no_arr.COUNT
        INSERT INTO POLICY_MAP_TEMP1 (POLICY_NO, company_cd) 
        VALUES (p_policy_no_arr(i), p_column2_arr(i));

    COMMIT;
END;
