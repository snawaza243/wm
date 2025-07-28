CREATE OR REPLACE PROCEDURE WEALTHMAKER.PSM_MFPI_GET_BY_DT(
    P_DOC_ID IN VARCHAR2,
    P_RESULT OUT SYS_REFCURSOR
) AS
    V_SCHEME_EXISTS NUMBER := 0;
BEGIN
    
    OPEN P_RESULT FOR
    SELECT 
        A.INVESTOR_NAME as INVESTOR_NAME,
        STM.COUNTRY_ID  as country_id,
        S.SCH_CODE      as sch_code,
        A.INV_CODE      as inv_code,
        A.ADDRESS1      as ADDRESS1,
        A.ADDRESS2      as ADDRESS2,
        A.EMAIL         as email,
        C.CITY_NAME     as city_name,
        B.BRANCH_NAME   as branch_name,
        TDU.BUSI_RM_CODE as BUSI_RM_CODE,
        CT.CLIENT_CODE  AS EXIST_CODE,
        E.RM_NAME       as RM_NAME,
        A.PAN           as pan,
        TDU.TRAN_TYPE   as tran_type,
        S.SCH_NAME      as sch_name,
        S.MUT_CODE      as MUT_CODE,
        A.AADHAR_CARD_NO as AADHAR_CARD_NO,
        A.PINCODE       as PINCODE,
        A.MOBILE        as mobile,
        to_char(A.DOB, 'dd/mm/rrrr') as dob,
        DECODE(A.CLIENT_TYPE, 'RELIGARE', 'RELIGARE', NULL) AS CLIENT_TYPE,
        CASE 
            WHEN TDU.VERIFICATION_FLAG = '0' THEN 'DT is not verified'
            WHEN TDU.REJECTION_STATUS = '1' THEN 'DT is rejected'
            WHEN TDU.PUNCHING_FLAG = '1' THEN 'DT is already punched'
            WHEN TDU.TRAN_TYPE != 'MF' THEN 'DT is not correct type'
            ELSE 'DT is valid'
        END AS STATUS_MESSAGE
    FROM TB_DOC_UPLOAD TDU
    LEFT JOIN         INVESTOR_MASTER A ON TDU.INV_CODE = A.INV_CODE
    LEFT JOIN         BRANCH_MASTER B ON TDU.BUSI_BRANCH_CODE = B.BRANCH_CODE
    LEFT JOIN         EMPLOYEE_MASTER E ON A.RM_CODE = E.RM_CODE
    LEFT JOIN         CLIENT_TEST CT ON CT.CLIENT_CODEKYC = A.INV_CODE
    LEFT JOIN         CITY_MASTER C ON A.CITY_ID = C.CITY_ID
    LEFT JOIN         STATE_MASTER STM ON C.STATE_ID = STM.STATE_ID
    LEFT JOIN         SCHEME_INFO S ON S.SCH_CODE = TDU.SCH_CODE
    WHERE 
        TDU.COMMON_ID = P_DOC_ID
        ORDER BY CASE WHEN TRAN_TYPE = 'MF' THEN 0 ELSE 1 END, TRAN_TYPE;

    -- Check if the sch_code exists in the scheme_info table
    SELECT COUNT(*) INTO V_SCHEME_EXISTS
    FROM SCHEME_INFO S
    WHERE S.SCH_CODE = (SELECT SCH_CODE FROM TB_DOC_UPLOAD WHERE COMMON_ID = P_DOC_ID AND TRAN_TYPE = 'MF' AND ROWNUM = 1);

    -- If sch_code not found in scheme_info, search in other_product
    IF V_SCHEME_EXISTS = 0 THEN
        OPEN P_RESULT FOR
        SELECT 
            A.INVESTOR_NAME,
            STM.COUNTRY_ID,
            O.OSCH_CODE AS SCH_CODE,
            A.INV_CODE,
            A.ADDRESS1,
            A.ADDRESS2,
            A.EMAIL,
            A.DOB,
            C.CITY_NAME,
            B.BRANCH_NAME,
            DECODE(A.CLIENT_TYPE, 'RELIGARE', 'RELIGARE', NULL) AS CLIENT_TYPE,
            TDU.BUSI_RM_CODE,
            CT.CLIENT_CODE AS EXIST_CODE,
            E.RM_NAME,
            A.PAN,
            TDU.TRAN_TYPE,
            O.OSCH_NAME AS SCH_NAME,
            O.ISS_CODE AS MUT_CODE,  -- Assuming 'MF' is a default value for MUT_CODE
            A.AADHAR_CARD_NO,
            A.PINCODE,
            A.MOBILE,
            -- Adding message logic for flags
            CASE
                WHEN TDU.TRAN_TYPE != 'MF' THEN 'DT is not correct type' 
                WHEN TDU.VERIFICATION_FLAG = '0' THEN 'DT is not verified'
                WHEN TDU.REJECTION_STATUS = '1' THEN 'DT is rejected'
                WHEN TDU.PUNCHING_FLAG = '1' THEN 'DT is already punched'
                
                ELSE 'DT is valid'
            END AS STATUS_MESSAGE
        FROM 
            TB_DOC_UPLOAD TDU
        LEFT JOIN 
            INVESTOR_MASTER A ON TDU.INV_CODE = A.INV_CODE
        LEFT JOIN 
            BRANCH_MASTER B ON TDU.BUSI_BRANCH_CODE = B.BRANCH_CODE
        LEFT JOIN 
            EMPLOYEE_MASTER E ON A.RM_CODE = E.RM_CODE
        LEFT JOIN 
            CLIENT_TEST CT ON CT.CLIENT_CODEKYC = A.INV_CODE
        LEFT JOIN 
            CITY_MASTER C ON A.CITY_ID = C.CITY_ID
        LEFT JOIN 
            STATE_MASTER STM ON C.STATE_ID = STM.STATE_ID
        LEFT JOIN 
            OTHER_PRODUCT O ON O.OSCH_CODE = TDU.SCH_CODE  -- Fallback to other_product
        WHERE 
 
        TDU.COMMON_ID = P_DOC_ID
        ORDER BY CASE WHEN TRAN_TYPE = 'MF' THEN 0 ELSE 1 END, TRAN_TYPE;
    END IF;

END PSM_MFPI_GET_BY_DT;
/