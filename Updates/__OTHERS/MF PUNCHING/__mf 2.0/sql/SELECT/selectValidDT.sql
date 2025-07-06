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
            WHEN TDU.TRAN_TYPE != 'MF' THEN 'DT is not correct type'
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