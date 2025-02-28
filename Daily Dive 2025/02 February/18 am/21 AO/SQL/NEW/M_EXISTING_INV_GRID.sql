create or replace PROCEDURE PSM_AO_MM_Existing_Inv_Grid (
    p_source_code IN client_test.source_code%TYPE,
    p_exist       IN client_test.client_codekyc%TYPE,
    p_cursor      OUT SYS_REFCURSOR   
) AS
BEGIN
    -- Open a cursor to return the result of the select query
    OPEN p_cursor FOR
        SELECT 
            ct.client_code      as ah_client_code,
            ct.main_code        as ah_main_code,
            ct.client_codekyc   as inv_code,

            ct.title            AS investor_title,
            ct.client_name      AS investor_name,
            CASE 
                WHEN UPPER(SUBSTR(ct.gender, 1, 1)) = 'F' THEN 'Female'
                WHEN UPPER(SUBSTR(ct.gender, 1, 1)) = 'M' THEN 'Male'
                WHEN UPPER(SUBSTR(ct.gender, 1, 1)) = 'O' THEN 'Non-Indian'
                ELSE ct.gender
            END                 AS gender,  -- Fixed missing comma
            ct.mobile_no        AS mobile,   
            ct.email            AS email,              
            ct.client_pan       AS pan,
            NVL((SELECT RELATION_NAME FROM relationship_master WHERE RELATION_ID = ct.relation_id), '') 
                                AS our_relationship,


            ct.relation_id     AS OUR_RELATIONSHIP_ID,
            CT.occ_id           AS OCC_ID,
            NVL((SELECT OCC_NAME FROM OCCUPATION_MASTER WHERE OCC_ID = CT.occ_id), '') 
                                AS OCC_NAME,
            CASE 
                WHEN UPPER(SUBSTR(ct.kyc_status, 1, 1)) = 'Y' THEN 'YES'
                WHEN UPPER(SUBSTR(ct.kyc_status, 1, 1)) = 'N' THEN 'NO'
                ELSE ct.kyc_status
            END                 AS KYC,
            ct.g_name           AS g_name,
            ct.g_pan            AS g_pan,
            ct.approved         AS approved,
            ct.AADHAR_CARD_NO   AS AADHAR_CARD_NO,
            ct.is_nominee       AS is_nominee,                     
            ct.nominee_per      AS nominee_per        
        FROM client_test ct
        WHERE ct.source_code = p_source_code
          AND ct.client_code != ct.main_code
        ORDER BY ct.client_name;
END;