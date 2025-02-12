CREATE OR REPLACE PROCEDURE PSM_AO_SEARCH_INVESTORS (
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
        IM.inv_code AS inv_code, 
        IM.investor_name AS investor_name, 
        im.address1 AS address1, 
        im.address2 AS address2, 
        NVL(CIM.city_name, '') AS CITY_ID, 
        IM.phone AS phone, 
        NVL(BRM.branch_name, '') AS branch_CODE,  
        IM.mobile AS mobile, 
        IM.dob AS dob, 
        IM.pan AS pan,     
        NVL(EMM.rm_name, '') AS rm_CODE,  
        IM.source_id AS source_id
    FROM 
        INVESTOR_MASTER IM
    LEFT JOIN CLIENT_MASTER CM ON CM.CLIENT_CODE = im.source_id
    LEFT JOIN BRANCH_MASTER BRM ON BRM.branch_code = im.branch_code
    LEFT JOIN EMPLOYEE_MASTER EMM ON EMM.rm_code = im.rm_code
    LEFT JOIN CITY_MASTER CIM ON CIM.city_id = im.city_id
    
    WHERE 
        (P_BRANCH IS NULL OR IM.branch_code = P_BRANCH)
        AND (P_RM IS NULL OR IM.rm_code = P_RM)
        AND (P_DOB IS NULL OR IM.dob = P_DOB)
        AND (P_CLIENT_CODE IS NULL OR IM.source_id = P_CLIENT_CODE)
        AND (P_MOBILE IS NULL OR IM.mobile = P_MOBILE)
        AND (P_PHONE IS NULL OR IM.phone = P_PHONE)
        AND (P_ACCOUNT_CODE IS NULL OR IM.inv_code = P_ACCOUNT_CODE)
        AND (P_PAN IS NULL OR IM.pan = P_PAN)
        AND (P_INVESTOR_NAME IS NULL OR UPPER(IM.investor_name) LIKE '%' || UPPER(P_INVESTOR_NAME) || '%')
        AND (P_ADDRESS1 IS NULL OR UPPER(IM.address1) LIKE '%' || UPPER(P_ADDRESS1) || '%')
        AND (P_ADDRESS2 IS NULL OR UPPER(IM.address2) LIKE '%' || UPPER(P_ADDRESS2) || '%')
        AND ROWNUM <= 500
    ORDER BY 
        IM.investor_name;
END;
/
