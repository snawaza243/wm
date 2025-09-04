CREATE OR REPLACE PROCEDURE PSM_MF2_BUSI_CORE_LOST_F(
    PRM_BUSI_CODE   VARCHAR2,
    PRM_INV_CODE    VARCHAR2,
    PRM_LOG         VARCHAR2, 
    PRM_ROLE        VARCHAR2,
    PRM_CURSOR      OUT REFCURSOR
) AS 

BEGIN 
    
    IF PRM_BUSI_CODE <> '0' THEN
        V_BRANCH :='';

        IF PRM_INV_CODE <> '0' THEN 
            select i.branch_code INTO V1_BRANCH_CODE from investor_master i where I.inv_code=PRM_INV_CODE;
            IF V1_BRANCH_CODE IS NOT NULL THEN V_BRANCH_CODE:= V1_BRANCH_CODE;
        END IF;

        IF PRM_ALL_INDIA = 'F' THEN
            IF V_BRANCH_CODE = '' THEN
                VA_BUSI_CODE : = '';
                OPEN PRM_CURSOR FOR SELECT 'Please Select Investor First' from dual; return;
            END IF;
        END IF;

        IF PRM_ALL_INDIA_SEARCH_LAG <> 'ALL' THEN 

            Select source,branch_name,rm_name,rm_code 
            INTO V2_source,V2_branch_name,V2_rm_name,V2_rm_code
            from employee_master e,branch_master b where b.branch_code in (V_BRANCHES) 
            and e.payroll_id= PRM_TxtBusiCodeA 
            and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018);
        ELSE 
            Select source,branch_name,rm_name,rm_code 
            INTO V2_source,V2_branch_name,V2_rm_name,V2_rm_code 
            from employee_master e,branch_master b             
            where e.payroll_id=PRM_TxtBusiCodeA
            and e.source=b.branch_code and (e.type='A' or e.type is null) and e.CATEGORY_ID in(2001,2018);
        END IF;

        IF V2_branch_name IS NOT NULL THEN
            
        END IF;

    END IF;



END; 