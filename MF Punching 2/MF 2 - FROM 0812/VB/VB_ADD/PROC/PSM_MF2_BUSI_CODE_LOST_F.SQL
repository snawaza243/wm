CREATE OR REPLACE PROCEDURE PSM_MF2_BUSI_CODE_LOST_F(
    PRM_BUSI_CODE           IN VARCHAR2,
    PRM_INV_CODE            IN VARCHAR2,
    PRM_ALL_INDIA           IN VARCHAR2 DEFAULT 'F',
    PRM_ALL_INDIA_SEARCH_LAG IN VARCHAR2 DEFAULT NULL,
    PRM_BRANCHES            IN VARCHAR2,
    PRM_LOG                 IN VARCHAR2, 
    PRM_ROLE                IN VARCHAR2,
    PRM_CURSOR              OUT SYS_REFCURSOR
) AS 
    V_EXCEPTION             VARCHAR2(400);
    V_BRANCH_CODE           VARCHAR2(50);
    V1_BRANCH_CODE          VARCHAR2(50);
    V2_source               VARCHAR2(50);
    V2_branch_name          VARCHAR2(100);
    V2_rm_name              VARCHAR2(100);
    V2_rm_code              VARCHAR2(50);
    
    VA_BUSI_CODE            VARCHAR2(50);
    VA_BRANCH_NAME          VARCHAR2(100);
    VA_BRANCH_CODE          VARCHAR2(50);
    VA_RM_NAME              VARCHAR2(100);
    VA_RM_CODE               VARCHAR2(50);
    VA_INV_CODE             VARCHAR2(50);
    VA_INV_NAME             VARCHAR2(100);
    MyCurrentBranchCode     VARCHAR2(50);
BEGIN 
    -- Initialize variables
    VA_BUSI_CODE := PRM_BUSI_CODE;
    VA_INV_CODE := PRM_INV_CODE;
    
    IF PRM_BUSI_CODE IS NOT NULL AND PRM_BUSI_CODE <> '0' THEN
        V_BRANCH_CODE := '';

        IF PRM_INV_CODE IS NOT NULL AND PRM_INV_CODE <> '0' THEN 
            BEGIN
                SELECT i.branch_code INTO V1_BRANCH_CODE FROM investor_master i WHERE i.inv_code = PRM_INV_CODE;
                
                IF V1_BRANCH_CODE IS NOT NULL THEN 
                    V_BRANCH_CODE := V1_BRANCH_CODE;
                END IF;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    V_BRANCH_CODE := '';
            END;
        END IF;

        IF PRM_ALL_INDIA = 'F' THEN
            IF V_BRANCH_CODE IS NULL OR V_BRANCH_CODE = '' THEN
                OPEN PRM_CURSOR FOR 
                    SELECT 'Please Select Investor First' AS V_MSG,
                           '' AS VA_BUSI_CODE,
                           '' AS VA_BRANCH_NAME,
                           '' AS VA_BRANCH_CODE,
                           '' AS VA_RM_NAME,
                           '' AS VA_RM_CODE,
                           '' AS VA_INV_CODE,
                           '' AS VA_INV_NAME,
                           '' AS MyCurrentBranchCode
                    FROM DUAL;
                RETURN;
            END IF;
        END IF;

        BEGIN
            IF PRM_ALL_INDIA_SEARCH_LAG <> 'ALL' THEN
                SELECT e.source, b.branch_name, e.rm_name, e.rm_code 
                INTO V2_source, V2_branch_name, V2_rm_name, V2_rm_code
                FROM employee_master e, branch_master b 
                WHERE b.branch_code IN (PRM_BRANCHES)
                AND e.payroll_id = TRIM(PRM_BUSI_CODE)
                AND e.source = b.branch_code 
                AND (e.type = 'A' OR e.type IS NULL) 
                AND e.CATEGORY_ID IN (2001, 2018)
                AND ROWNUM = 1;
            ELSE
                SELECT e.source, b.branch_name, e.rm_name, e.rm_code 
                INTO V2_source, V2_branch_name, V2_rm_name, V2_rm_code
                FROM employee_master e, branch_master b 
                WHERE e.payroll_id = TRIM(PRM_BUSI_CODE)
                AND e.source = b.branch_code 
                AND (e.type = 'A' OR e.type IS NULL) 
                AND e.CATEGORY_ID IN (2001, 2018)
                AND ROWNUM = 1;
            END IF;
            
            -- If we get here, employee was found
            VA_BRANCH_NAME := V2_branch_name;
            VA_BRANCH_CODE := V2_source;
            VA_RM_NAME := V2_rm_name;
            VA_RM_CODE := V2_rm_code;
            MyCurrentBranchCode := V2_source;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                OPEN PRM_CURSOR FOR 
                    SELECT 'Employee Should Be Of Sale Support or marketing' AS V_MSG,
                           '' AS VA_BUSI_CODE,
                           '' AS VA_BRANCH_NAME,
                           '' AS VA_BRANCH_CODE,
                           '' AS VA_RM_NAME,
                           '' AS VA_RM_CODE,
                           '' AS VA_INV_CODE,
                           '' AS VA_INV_NAME,
                           '' AS MyCurrentBranchCode
                    FROM DUAL;
                RETURN;
            WHEN TOO_MANY_ROWS THEN
                -- If multiple rows found, take the first one
                IF PRM_ALL_INDIA_SEARCH_LAG <> 'ALL' THEN
                    SELECT source, branch_name, rm_name, rm_code 
                    INTO V2_source, V2_branch_name, V2_rm_name, V2_rm_code
                    FROM (
                        SELECT e.source, b.branch_name, e.rm_name, e.rm_code 
                        FROM employee_master e, branch_master b 
                        WHERE b.branch_code IN (PRM_BRANCHES)
                        AND e.payroll_id = TRIM(PRM_BUSI_CODE)
                        AND e.source = b.branch_code 
                        AND (e.type = 'A' OR e.type IS NULL) 
                        AND e.CATEGORY_ID IN (2001, 2018)
                        ORDER BY e.source
                    ) WHERE ROWNUM = 1;
                ELSE
                    SELECT source, branch_name, rm_name, rm_code 
                    INTO V2_source, V2_branch_name, V2_rm_name, V2_rm_code
                    FROM (
                        SELECT e.source, b.branch_name, e.rm_name, e.rm_code 
                        FROM employee_master e, branch_master b 
                        WHERE e.payroll_id = TRIM(PRM_BUSI_CODE)
                        AND e.source = b.branch_code 
                        AND (e.type = 'A' OR e.type IS NULL) 
                        AND e.CATEGORY_ID IN (2001, 2018)
                        ORDER BY e.source
                    ) WHERE ROWNUM = 1;
                END IF;
                
                VA_BRANCH_NAME := V2_branch_name;
                VA_BRANCH_CODE := V2_source;
                VA_RM_NAME := V2_rm_name;
                VA_RM_CODE := V2_rm_code;
                MyCurrentBranchCode := V2_source;
        END;

    END IF;

    OPEN PRM_CURSOR FOR 
        SELECT 
            'SUCCESS: VALIDATION DONE!' AS V_MSG,
            VA_BUSI_CODE AS VA_BUSI_CODE,
            VA_BRANCH_NAME AS VA_BRANCH_NAME,
            VA_BRANCH_CODE AS VA_BRANCH_CODE,
            VA_RM_NAME AS VA_RM_NAME,
            VA_RM_CODE AS VA_RM_CODE,
            VA_INV_CODE AS VA_INV_CODE,
            VA_INV_NAME AS VA_INV_NAME,
            MyCurrentBranchCode AS MyCurrentBranchCode
        FROM DUAL;

EXCEPTION
    WHEN OTHERS THEN
        V_EXCEPTION:= 'ERROR: ' || SQLERRM ;
        OPEN PRM_CURSOR FOR 
            SELECT 
                V_EXCEPTION AS V_MSG,
                '' AS VA_BUSI_CODE,
                '' AS VA_BRANCH_NAME,
                '' AS VA_BRANCH_CODE,
                '' AS VA_RM_NAME,
                '' AS VA_RM_CODE,
                '' AS VA_INV_CODE,
                '' AS VA_INV_NAME,
                '' AS MyCurrentBranchCode
            FROM DUAL;
END PSM_MF2_BUSI_CODE_LOST_F;
/