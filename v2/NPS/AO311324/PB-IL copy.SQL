
create or replace PROCEDURE PSM_AO_SEARCH_INVESTORS (
    P_BRANCH        IN NUMBER,
    P_RM            IN NUMBER,
    P_DOB           IN DATE,
    P_CLIENT_CODE   IN NUMBER,
    P_CLIENT_NAME   IN VARCHAR2,
    P_MOBILE        IN NUMBER,
    P_CITY          IN VARCHAR2,
    P_PHONE         IN VARCHAR2,
    P_ACCOUNT_CODE  IN VARCHAR2,
    P_PAN           IN VARCHAR2,
    P_INVESTOR_NAME IN VARCHAR2,
    P_ADDRESS1      IN VARCHAR2,
    P_ADDRESS2      IN VARCHAR2,
    P_RESULT        OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN P_RESULT FOR
    SELECT 
    INVESTOR_MASTER.inv_code AS inv_code, 
    INVESTOR_MASTER.investor_name AS investor_name, 
    INVESTOR_MASTER.address1 AS address1, 
    INVESTOR_MASTER.address2 AS address2, 
    NVL((SELECT CITY_MASTER.CITY_NAME FROM CITY_MASTER WHERE CITY_MASTER.city_id = INVESTOR_MASTER.city_id  FETCH FIRST 1 ROWS ONLY), 'No City') AS city_id, 
    INVESTOR_MASTER.phone AS phone, 
    NVL((SELECT branch_master.branch_name FROM branch_master WHERE branch_master.branch_code = INVESTOR_MASTER.BRANCH_CODE  FETCH FIRST 1 ROWS ONLY), 'No Branch') AS BRANCH_CODE,  
    INVESTOR_MASTER.mobile AS mobile, 
    INVESTOR_MASTER.dob AS dob, 
    INVESTOR_MASTER.pan AS pan,     
    NVL((SELECT employee_master.rm_name FROM employee_master WHERE employee_master.rm_code = INVESTOR_MASTER.rm_code  FETCH FIRST 1 ROWS ONLY), 'No RM') AS rm_code,  

    INVESTOR_MASTER.source_id AS source_id
    FROM INVESTOR_MASTER
    WHERE (P_BRANCH IS NULL OR BRANCH_CODE = P_BRANCH)
      AND (P_RM IS NULL OR RM_CODE = P_RM)
      AND (P_DOB IS NULL OR dob = P_DOB)
      AND (P_CLIENT_CODE IS NULL OR source_id = P_CLIENT_CODE)
      AND (P_MOBILE IS NULL OR mobile = P_MOBILE)
      AND (P_PHONE IS NULL OR phone = P_PHONE)
      AND (P_ACCOUNT_CODE IS NULL OR INV_CODE = P_ACCOUNT_CODE)
      AND (P_PAN IS NULL OR pan = P_PAN)
      AND (P_INVESTOR_NAME IS NULL OR UPPER(INVESTOR_NAME) LIKE '%' || UPPER(P_INVESTOR_NAME) || '%')
      AND (P_ADDRESS1 IS NULL OR UPPER(address1) LIKE '%' || UPPER(P_ADDRESS1) || '%')
      AND (P_ADDRESS2 IS NULL OR UPPER(address2) LIKE '%' || UPPER(P_ADDRESS2) || '%')
      AND ROWNUM <=500
    ORDER BY investor_name

    ;
END;
