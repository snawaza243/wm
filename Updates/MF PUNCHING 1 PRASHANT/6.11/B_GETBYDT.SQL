CREATE OR REPLACE PROCEDURE WEALTHMAKER.GetInvestorDetailsByDocID(
    p_doc_id IN VARCHAR2,
    p_result OUT SYS_REFCURSOR
) AS
    V_SCHEME_EXISTS NUMBER := 0;  -- Variable to check if sch_code exists in scheme_info
BEGIN
    -- First query to fetch details from tb_doc_upload, investor_master, and scheme_info
    OPEN p_result FOR
    SELECT 
        a.INVESTOR_NAME,
        stm.country_id,
        s.sch_code,
        a.INV_CODE,
        a.ADDRESS1,
        a.ADDRESS2,
        a.EMAIL,
        a.DOB,
        c.CITY_NAME,
        b.BRANCH_NAME,
        DECODE(a.CLIENT_TYPE, 'RELIGARE', 'RELIGARE', NULL) AS CLIENT_TYPE,
        tdu.BUSI_RM_CODE,
        ct.client_code as EXIST_CODE,
        e.RM_NAME,
        a.PAN,
        tdu.TRAN_TYPE,
        s.SCH_NAME,
        s.MUT_CODE,
        a.AADHAR_CARD_NO,
        a.PINCODE,
        a.MOBILE,
        -- Adding message logic for flags
        CASE 
            WHEN tdu.VERIFICATION_FLAG = '0' THEN 'DT is not verified'
            WHEN tdu.REJECTION_STATUS = '1' THEN 'DT is rejected'
            WHEN tdu.PUNCHING_FLAG = '1' THEN 'DT is already punched'
            ELSE 'DT is valid'
        END AS STATUS_MESSAGE
    FROM 
        tb_doc_upload tdu
    LEFT JOIN 
        investor_master a ON tdu.inv_code = a.inv_code
    LEFT JOIN 
        Branch_master b ON tdu.busi_branch_code = b.branch_code
    LEFT JOIN 
        EMPLOYEE_MASTER e ON a.RM_CODE = e.RM_CODE
    LEFT JOIN 
        CLIENT_TEST ct ON ct.client_codekyc = a.inv_code
    LEFT JOIN 
        City_master c ON a.city_id = c.city_id
    LEFT JOIN 
        STATE_MASTER stm ON c.state_id = stm.state_id
    LEFT JOIN 
        SCHEME_INFO s ON s.sch_code = tdu.sch_code
    WHERE 
        tdu.common_id = p_doc_id;

    -- Check if the sch_code exists in the scheme_info table
    SELECT COUNT(*) INTO V_SCHEME_EXISTS
    FROM SCHEME_INFO s
    WHERE s.sch_code = (SELECT sch_code FROM tb_doc_upload WHERE common_id = p_doc_id AND TRAN_TYPE = 'MF' AND ROWNUM = 1);

    -- If sch_code not found in scheme_info, search in other_product
    IF V_SCHEME_EXISTS = 0 THEN
        OPEN p_result FOR
        SELECT 
            a.INVESTOR_NAME,
            stm.country_id,
            o.osch_code AS sch_code,
            a.INV_CODE,
            a.ADDRESS1,
            a.ADDRESS2,
            a.EMAIL,
            a.DOB,
            c.CITY_NAME,
            b.BRANCH_NAME,
            DECODE(a.CLIENT_TYPE, 'RELIGARE', 'RELIGARE', NULL) AS CLIENT_TYPE,
            tdu.BUSI_RM_CODE,
            ct.client_code as EXIST_CODE,
            e.RM_NAME,
            a.PAN,
            tdu.TRAN_TYPE,
            o.osch_name AS SCH_NAME,
            o.iss_code AS MUT_CODE,  -- Assuming 'MF' is a default value for MUT_CODE
            a.AADHAR_CARD_NO,
            a.PINCODE,
            a.MOBILE,
            -- Adding message logic for flags
            CASE 
                WHEN tdu.VERIFICATION_FLAG = '0' THEN 'DT is not verified'
                WHEN tdu.REJECTION_STATUS = '1' THEN 'DT is rejected'
                WHEN tdu.PUNCHING_FLAG = '1' THEN 'DT is already punched'
                ELSE 'DT is valid'
            END AS STATUS_MESSAGE
        FROM 
            tb_doc_upload tdu
        LEFT JOIN 
            investor_master a ON tdu.inv_code = a.inv_code
        LEFT JOIN 
            Branch_master b ON tdu.busi_branch_code = b.branch_code
        LEFT JOIN 
            EMPLOYEE_MASTER e ON a.RM_CODE = e.RM_CODE
        LEFT JOIN 
            CLIENT_TEST ct ON ct.client_codekyc = a.inv_code
        LEFT JOIN 
            City_master c ON a.city_id = c.city_id
        LEFT JOIN 
            STATE_MASTER stm ON c.state_id = stm.state_id
        LEFT JOIN 
            other_product o ON o.osch_code = tdu.sch_code  -- Fallback to other_product
        WHERE 
            tdu.common_id = p_doc_id;
    END IF;

END GetInvestorDetailsByDocID;
/
