CREATE OR REPLACE PROCEDURE PSM_MF2_GET_BY_DT(
    P_INDEX         IN VARCHAR2, -- 0 for new, 1 for modification
    P_COMMON_ID     IN VARCHAR2, -- TEST 11629684 CROSS CHANNEL DT
    P_LOGIN_ID      IN VARCHAR2,
    P_ROLE_ID       IN VARCHAR2,
    P_CHK_SWITCH    IN VARCHAR2, -- 0 for new, 1 for modification
    P_CURSOR        OUT SYS_REFCURSOR
)
AS
    V_EXPENTION_MESSAGE VARCHAR2(4000);
    V_RETURN_MESSAGE VARCHAR2(4000);
    V_BUSI_RM_CODE VARCHAR2(50);
    V_BUSI_BRANCH_CODE VARCHAR2(50);
    V_BUSI_TR_DATE DATE;
    V_SCH_CODE VARCHAR2(50);
    V_INV_CODE VARCHAR2(50);
    V_BRANCH_NAME VARCHAR2(100);
    V_EXPENSE NUMBER;
    V_APPROVAL_FLAG VARCHAR2(10);
    V_SCH_NAME VARCHAR2(100);
    V_MUT_NAME VARCHAR2(100);
    V_MUT_CODE VARCHAR2(50);
    V_CHEK_SCH_NATURE VARCHAR2(1);
    V_MYNAT VARCHAR2(1);
    V_CHK_ATM_TR NUMBER;
    V_ATM_TR VARCHAR2(1);
    V2_INVESTOR_NAME VARCHAR2(100);
    V2_INV_CODE VARCHAR2(50);
    V2_address1 VARCHAR2(100);
    V2_address2 VARCHAR2(100);
    V2_city_name VARCHAR2(50);
    V2_Branch_name VARCHAR2(100);
    V2_CLIENT_TYPE VARCHAR2(50);
    V2_RM_NAME VARCHAR2(100);
    V2_BLANK VARCHAR2(1);
    V2_PAN VARCHAR2(20);
    V_loggeduserid VARCHAR2(50);
    V_MAIN_CODE VARCHAR2(50);
    V_MyUpdProc VARCHAR2(1);
    V_CROSS_CHANEL_COUNT NUMBER;
    
    -- Variables for Add tab (index 0)
    VA_EXPENSE_PER VARCHAR2(50);
    VA_EXPENSE_RS VARCHAR2(50);
    VA_AMC VARCHAR2(100);
    VA_AMC_CODE VARCHAR2(50);
    VA_SCH_NAME VARCHAR2(100);
    VA_SCH_CODE VARCHAR2(50);
    VA_ATM_TR VARCHAR2(1);
    VA_SWITCH_AMC VARCHAR2(100);
    VA_SWITCH_AMC_CODE VARCHAR2(50);
    VA_SWITCH_SCH VARCHAR2(100);
    VA_SWITCH_SCH_CODE VARCHAR2(50);
    VA_CLIENT_CODE VARCHAR2(50);
    VA_LABEL32 VARCHAR2(50);
    VA_INVESTOR_NAME VARCHAR2(100);
    VA_PAN VARCHAR2(20);
    VA_BUSI_CODE VARCHAR2(50);
    VA_AH_CODE VARCHAR2(50);
    VA_IM_ENTRY_DT DATE;
    VA_IM_N_EXP_PER NUMBER;
    VA_BUSI_BRANCH_CODE VARCHAR2(50);
    VA_BUSI_BRANCH_NAME VARCHAR2(100);
    VA_AMC_ENABLED VARCHAR2(1);
    VA_SCHEM_ENABLED VARCHAR2(1);
    VA_IM_ENTRY_DT_ENABLED VARCHAR2(1);
    
    -- Variables for Modification tab (index 1)
    VM_EXPENSE_PER VARCHAR2(50);
    VM_EXPENSE_RS VARCHAR2(50);
    VM_AMC VARCHAR2(100);
    VM_AMC_CODE VARCHAR2(50);
    VM_SCH_NAME VARCHAR2(100);
    VM_SCH_CODE VARCHAR2(50);
    VM_ATM_TR VARCHAR2(1);
    VM_SWITCH_AMC VARCHAR2(100);
    VM_SWITCH_AMC_CODE VARCHAR2(50);
    VM_SWITCH_SCH VARCHAR2(100);
    VM_SWITCH_SCH_CODE VARCHAR2(50);
    VM_CLIENT_CODE VARCHAR2(50);
    VM_LABEL42 VARCHAR2(50);
    VM_INVESTOR_NAME VARCHAR2(100);
    VM_PAN VARCHAR2(20);
    VM_BUSI_CODE VARCHAR2(50);
    VM_AH_CODE VARCHAR2(50);
    
    -- Record type for checking if data exists
    TYPE t_record IS RECORD (dummy NUMBER);
    TYPE t_record_table IS TABLE OF t_record;
    l_record t_record_table;
    
BEGIN

    DECLARE
        IS_VALID_DT VARCHAR(10);
    BEGIN -- CHECK VALID DT

        SELECT 
        --COMMON_ID, TRAN_TYPE, VERIFICATION_FLAG, REJECTION_STATUS,  PUNCHING_FLAG--  
        COUNT(*)  INTO IS_VALID_DT
        FROM TB_DOC_UPLOAD A, BRANCH_MASTER B 
        WHERE A.BUSI_BRANCH_CODE = B.BRANCH_CODE 
        AND COMMON_ID = TRIM(P_COMMON_ID) -- 10000201
        AND TRAN_TYPE = 'MF' 
        AND VERIFICATION_FLAG = '1' 
        AND REJECTION_STATUS = '0'  
        AND PUNCHING_FLAG = '0' 
        AND ROWNUM = 1;

        IF IS_VALID_DT > 0 THEN          
            
            -- Get basic document information
            BEGIN
                SELECT A.BUSI_RM_CODE, A.BUSI_BRANCH_CODE, A.BUSI_TR_DATE, A.SCH_CODE, A.INV_CODE, B.BRANCH_NAME, A.EXPENSE 
                INTO V_BUSI_RM_CODE, V_BUSI_BRANCH_CODE, V_BUSI_TR_DATE, V_SCH_CODE, V_INV_CODE, V_BRANCH_NAME, V_EXPENSE
                FROM TB_DOC_UPLOAD A, BRANCH_MASTER B 
                WHERE A.BUSI_BRANCH_CODE = B.BRANCH_CODE 
                  AND COMMON_ID = TRIM(P_COMMON_ID) 
                  AND TRAN_TYPE = 'MF' 
                  AND VERIFICATION_FLAG = '1' 
                  AND REJECTION_STATUS = '0'  
                  AND PUNCHING_FLAG = '0' 
                  AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    V_RETURN_MESSAGE := 'ERROR: Document not found or already processed';
            END;

            SELECT COUNT(*) 
            BULK COLLECT INTO l_record
            FROM TB_DOC_UPLOAD 
            WHERE COMMON_ID = TRIM(P_COMMON_ID) 
              AND TRAN_TYPE = 'MF' 
              AND VERIFICATION_FLAG = '1' 
              AND REJECTION_STATUS = '0'  
              AND PUNCHING_FLAG = '0';

            IF l_record.COUNT > 0 THEN
                -- Check approval status
                BEGIN
                    SELECT WEALTHMAKER.FN_CHECK_FOR_APPROVAL_ALL(P_COMMON_ID)
                    INTO V_APPROVAL_FLAG
                    FROM DUAL;
                    
                    IF V_APPROVAL_FLAG = '2' THEN
                        V_RETURN_MESSAGE := 'ERROR: Approval request already raised';
                    ELSIF V_APPROVAL_FLAG = '4' THEN
                        V_RETURN_MESSAGE := 'ERROR: Approval request rejected by management';
                    END IF;
                END;

                -- Get scheme information
                IF V_SCH_CODE IS NOT NULL THEN
                    BEGIN
                        SELECT SCH_CODE, SCH_NAME, MUT_NAME, MUT_CODE
                        INTO V_SCH_CODE, V_SCH_NAME, V_MUT_NAME, V_MUT_CODE
                        FROM (
                            SELECT OSCH_CODE SCH_CODE, OSCH_NAME SCH_NAME, LONGNAME, NAME, ISS_NAME MUT_NAME, '' MUT_CODE 
                            FROM OTHER_PRODUCT O, PRODUCT_MASTER P, ISS_MASTER I 
                            WHERE OSCH_CODE = V_SCH_CODE 
                              AND O.PROD_CLASS_CODE = P.PROD_CODE 
                              AND O.ISS_CODE = I.ISS_CODE 
                              AND (I.BRANCH_CODE <> 'N' OR I.BRANCH_CODE IS NULL) 
                              AND (O.FLAG <> 'N' OR O.FLAG IS NULL)
                            UNION ALL
                            SELECT S.SCH_CODE, S.SCH_NAME, S.SHORT_NAME, 'MF' NAME, M.MUT_NAME, M.MUT_CODE 
                            FROM SCHEME_INFO S, MUT_FUND M 
                            WHERE S.SCH_CODE = V_SCH_CODE 
                              AND S.MUT_CODE = M.MUT_CODE 
                              AND M.MUT_CODE NOT IN ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') 
                              AND S.SCH_CODE IN (SELECT SCH_CODE FROM MP_SCHEME_MASTER SM WHERE SM.ACTIVE_STATUS = 'Active')
                        ) WHERE ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            V_RETURN_MESSAGE := 'WARNING: Scheme information not found';
                    END;

                    IF P_INDEX = '0' THEN
                        BEGIN
                            SELECT NVL(GET_SCHEME_NATURE(V_SCH_CODE), '') AS A
                            INTO V_CHEK_SCH_NATURE
                            FROM DUAL;

                            IF V_CHEK_SCH_NATURE = 'O' THEN
                                VA_EXPENSE_PER := 'Expenses agnst. Trail%';
                                VA_EXPENSE_RS  := 'Expenses agnst. Trail Rs.';
                                V_MYNAT      := 'O';
                            ELSE
                                VA_EXPENSE_PER := 'Expenses Trail%';
                                VA_EXPENSE_RS  := 'Expenses Trail Rs.';
                                V_MYNAT      := 'C';
                            END IF;
                        END;

                        BEGIN
                            IF P_CHK_SWITCH = '0' THEN 
                                VA_AMC := V_MUT_NAME;
                                VA_AMC_CODE := V_MUT_CODE;
                                VA_SCH_NAME := V_SCH_NAME;
                                VA_SCH_CODE := V_SCH_CODE;

                                -- CHECK ATM SCH TRANSACTION
                                SELECT NVL(COUNT(*),0) INTO V_CHK_ATM_TR 
                                FROM RELIANCE_ATM_MASTER 
                                WHERE SCH_CODE = V_SCH_CODE 
                                  AND FROM_DT <= SYSDATE 
                                  AND (TO_DT >= SYSDATE OR TO_DT IS NULL);

                                IF V_CHK_ATM_TR > 0 THEN
                                    VA_ATM_TR := '1';
                                ELSE
                                    VA_ATM_TR := '0';
                                END IF;
                            ELSE
                                VA_SWITCH_AMC := V_MUT_NAME;
                                VA_SWITCH_AMC_CODE := V_MUT_CODE;
                                VA_SWITCH_SCH := V_SCH_NAME;
                                VA_SWITCH_SCH_CODE := V_SCH_CODE;                  
                            END IF;
                        END;
                    ELSE -- 1 : MODIFICATION
                        -- CHECK SCHEME NATURE
                        BEGIN
                            SELECT NVL(GET_SCHEME_NATURE(V_SCH_CODE), '') AS A
                            INTO V_CHEK_SCH_NATURE
                            FROM DUAL;

                            IF V_CHEK_SCH_NATURE = 'O' THEN
                                VM_EXPENSE_PER := 'Expenses agnst. Trail%';
                                VM_EXPENSE_RS  := 'Expenses agnst. Trail Rs.';
                                V_MYNAT      := 'O';
                            ELSE
                                VM_EXPENSE_PER := 'Expenses Trail%';
                                VM_EXPENSE_RS  := 'Expenses Trail Rs.';
                                V_MYNAT      := 'C';
                            END IF;
                        END;

                        -- CHECK ATM SCH TRANSACTION
                        BEGIN
                            IF P_CHK_SWITCH = '0' THEN 
                                VM_AMC := V_MUT_NAME;
                                VM_AMC_CODE := V_MUT_CODE;
                                VM_SCH_NAME := V_SCH_NAME;
                                VM_SCH_CODE := V_SCH_CODE;

                                SELECT NVL(COUNT(*),0) INTO V_CHK_ATM_TR 
                                FROM RELIANCE_ATM_MASTER 
                                WHERE SCH_CODE = V_SCH_CODE 
                                  AND FROM_DT <= SYSDATE 
                                  AND (TO_DT >= SYSDATE OR TO_DT IS NULL);

                                IF V_CHK_ATM_TR > 0 THEN
                                    VM_ATM_TR := '1';
                                ELSE
                                    VM_ATM_TR := '0';
                                END IF;
                            ELSE
                                VM_SWITCH_AMC := V_MUT_NAME;
                                VM_SWITCH_AMC_CODE := V_MUT_CODE;
                                VM_SWITCH_SCH := V_SCH_NAME;
                                VM_SWITCH_SCH_CODE := V_SCH_CODE;                  
                            END IF;
                        END;
                    END IF;
                ELSE 
                    -- CmbAmcA_Click equivalent
                    VA_AMC := '';
                    VA_AMC_CODE := '';
                    VA_SCH_NAME := '';
                    VA_SCH_CODE := '';
                    V_RETURN_MESSAGE := 'OPEN_DDL:AMCA';
                END IF;

                -- Get investor information
                IF V_INV_CODE IS NOT NULL THEN
                    BEGIN
                        SELECT INVESTOR_name, INV_code, a.address1, a.address2, c.city_name, 
                               b.Branch_name, DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) CLIENT_TYPE, 
                               e.rm_name, '' BLANK, a.pan 
                        INTO V2_INVESTOR_NAME, V2_INV_CODE, V2_address1, V2_address2, V2_city_name, 
                             V2_Branch_name, V2_CLIENT_TYPE, V2_RM_NAME, V2_BLANK, V2_PAN
                        FROM Branch_master b, EMPLOYEE_MASTER E, investor_master a, City_master c 
                        WHERE E.source = b.branch_code 
                          AND a.RM_CODE = E.RM_CODE 
                          AND a.city_id = C.city_id(+) 
                          AND INVESTOR_name IS NOT NULL 
                          AND b.branch_code <> 10010226 
                          AND b.branch_code <> 10070257 
                          AND a.inv_code = V_INV_CODE 
                          AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            V_RETURN_MESSAGE := 'ERROR: Investor information not found';
                    END;

                    BEGIN
                        SELECT loggeduserid, main_code
                        INTO V_loggeduserid, V_MAIN_CODE
                        FROM client_test 
                        WHERE client_codekyc = V_INV_CODE 
                          AND ROWNUM = 1;
                    EXCEPTION
                        WHEN NO_DATA_FOUND THEN
                            V_loggeduserid := NULL;
                            V_MAIN_CODE := NULL;
                    END;

                    IF V_loggeduserid = 'PROC' THEN
                        BEGIN
                            SELECT NVL(UPD_PROC,'N') 
                            INTO V_MyUpdProc 
                            FROM client_test 
                            WHERE client_codekyc = V_INV_CODE 
                              AND dob IS NOT NULL 
                              AND ROWNUM = 1;
                        EXCEPTION
                            WHEN NO_DATA_FOUND THEN
                                V_MyUpdProc := 'N';
                        END;
                        
                        IF (SUBSTR(V_INV_CODE, 1, 1) <> '3') THEN
                            IF V_MyUpdProc IN ('N','0') THEN
                                -- Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account
                                V_RETURN_MESSAGE := 'OPEN_FORM#ACCOUNT_OPENING'||V_MAIN_CODE;
                            END IF;
                        END IF;
                    END IF;

                    IF P_INDEX = '0' THEN
                        VA_CLIENT_CODE := SUBSTR(V_INV_CODE,1,8);
                        VA_LABEL32 := V_INV_CODE;
                        VA_INVESTOR_NAME := V2_INVESTOR_NAME;

                        IF SUBSTR(V_INV_CODE,1,1) = '4' THEN
                            BEGIN
                                SELECT payroll_id 
                                INTO VA_BUSI_CODE 
                                FROM employee_master 
                                WHERE rm_code = (SELECT rm_code FROM client_master WHERE client_code = SUBSTR(V_INV_CODE,1,8))
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    VA_BUSI_CODE := NULL;
                            END;
                            
                            BEGIN
                                SELECT client_code 
                                INTO VA_AH_CODE 
                                FROM client_test 
                                WHERE client_codekyc = V_INV_CODE 
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    VA_AH_CODE := NULL;
                            END;

                            VA_PAN := V2_PAN;

                            IF VA_AH_CODE IS NULL OR LENGTH(VA_AH_CODE) < 6 THEN
                                V_RETURN_MESSAGE := 'Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same';
                            END IF;
                        ELSE
                            VA_PAN := V2_PAN;
                            VA_BUSI_CODE := V_BUSI_RM_CODE;
                        END IF;
                    ELSIF P_INDEX = '1' THEN
                        VM_CLIENT_CODE := SUBSTR(V_INV_CODE,1,8);
                        VM_INVESTOR_NAME := V2_INVESTOR_NAME;

                        IF SUBSTR(V_INV_CODE,1,1) = '4' THEN
                            BEGIN
                                SELECT client_code 
                                INTO VM_AH_CODE 
                                FROM client_test 
                                WHERE client_codekyc = V_INV_CODE 
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    VM_AH_CODE := NULL;
                            END;
                            
                            BEGIN
                                SELECT payroll_id 
                                INTO VM_BUSI_CODE 
                                FROM employee_master 
                                WHERE rm_code = (SELECT rm_code FROM client_master WHERE client_code = SUBSTR(V_INV_CODE,1,8))
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    VM_BUSI_CODE := NULL;
                            END;
                            
                            VM_PAN := V2_PAN;
                        ELSE
                            BEGIN
                                SELECT client_code 
                                INTO VM_AH_CODE 
                                FROM client_test 
                                WHERE client_codekyc = V_INV_CODE 
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    VM_AH_CODE := NULL;
                            END;
                            
                            BEGIN
                                SELECT payroll_id 
                                INTO VM_BUSI_CODE 
                                FROM employee_master 
                                WHERE rm_code = (SELECT rm_code FROM agent_master WHERE agent_code = SUBSTR(V_INV_CODE,1,8))
                                  AND ROWNUM = 1;
                            EXCEPTION
                                WHEN NO_DATA_FOUND THEN
                                    VM_BUSI_CODE := NULL;
                            END;
                            
                            VM_PAN := V2_PAN;
                        END IF;
                        VM_LABEL42 := V_INV_CODE;
                    END IF;
                    
                    IF P_INDEX = '0' THEN 
                        -- Execute lost focus for VA_BUSI_CODE
                        NULL; -- This would typically call another procedure in a real implementation
                    ELSE
                        -- Execute lost focus for VM_BUSI_CODE
                        NULL; -- This would typically call another procedure in a real implementation
                    END IF;
                ELSE
                    VA_CLIENT_CODE := '';
                    VA_LABEL32 := '';
                    VA_INVESTOR_NAME := '';

                    IF SUBSTR(V_INV_CODE,1,1) = '4' THEN 
                        VA_PAN := '';
                        VA_BUSI_CODE := '';
                        VA_AH_CODE := '';
                    ELSE
                        VA_PAN := '';
                        VA_BUSI_CODE := '';
                    END IF;
                END IF; -- CLOSING V_INV_CODE IS NOT NULL

                VA_BUSI_CODE := V_BUSI_RM_CODE;
                VA_IM_ENTRY_DT := V_BUSI_TR_DATE;

                IF P_ROLE_ID = '212' THEN
                    VA_AMC_ENABLED := 'N';
                    VA_SCHEM_ENABLED := 'N';
                    VA_IM_ENTRY_DT_ENABLED := 'N';
                ELSE
                    VA_AMC_ENABLED := 'Y';
                    VA_SCHEM_ENABLED := 'Y';
                    VA_IM_ENTRY_DT_ENABLED := 'Y';
                END IF;

                IF V_EXPENSE > 0 THEN
                    VA_IM_N_EXP_PER := V_EXPENSE;
                ELSE
                    VA_IM_N_EXP_PER := 0;
                END IF;

                VA_BUSI_BRANCH_CODE := V_BUSI_BRANCH_CODE;
                VA_BUSI_BRANCH_NAME := V_BRANCH_NAME;

                -- Cross-channel validation
                BEGIN
                    WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO(
                        P_COMMON_ID, 
                        V_INV_CODE, 
                        P_LOGIN_ID, 
                        V_CROSS_CHANEL_COUNT
                    );
                    
                    IF V_CROSS_CHANEL_COUNT > 0 THEN
                        V_RETURN_MESSAGE := 'SUCCESS:OPEN_POPUP:CROSS_CHANNEL_VALIDATION';
                    ELSE
                        V_RETURN_MESSAGE := 'SUCCESS:OPEN_POPUP:INVESTOR_ADDRESS_UPDATE0';
                    END IF;
                EXCEPTION
                    WHEN OTHERS THEN
                        V_EXPENTION_MESSAGE:= 'WARNING: Cross-channel validation failed: ' || SQLERRM;
                        V_RETURN_MESSAGE := V_EXPENTION_MESSAGE;
                        RETURN;
                END;

                -- Return all variables with a success message
                IF P_INDEX = '0' THEN
                    OPEN P_CURSOR FOR
                    SELECT 
                        V_RETURN_MESSAGE AS V_RETURN_MESSAGE,
                        VA_BUSI_CODE AS VA_BUSI_CODE,
                        VA_AMC AS VA_AMC,
                        VA_AMC_CODE AS VA_AMC_CODE,
                        VA_SCH_NAME AS VA_SCH_NAME,
                        VA_SCH_CODE AS VA_SCH_CODE,
                        VA_ATM_TR AS VA_ATM_TR,
                        VA_SWITCH_AMC AS VA_SWITCH_AMC,
                        VA_SWITCH_AMC_CODE AS VA_SWITCH_AMC_CODE,
                        VA_SWITCH_SCH AS VA_SWITCH_SCH,
                        VA_SWITCH_SCH_CODE AS VA_SWITCH_SCH_CODE,
                        VA_CLIENT_CODE AS VA_CLIENT_CODE,
                        VA_LABEL32 AS VA_LABEL32,
                        VA_INVESTOR_NAME AS VA_INVESTOR_NAME,
                        VA_PAN AS VA_PAN,
                        VA_AH_CODE AS VA_AH_CODE,
                        VA_IM_ENTRY_DT AS VA_IM_ENTRY_DT,
                        VA_IM_N_EXP_PER AS VA_IM_N_EXP_PER,
                        VA_BUSI_BRANCH_CODE AS VA_BUSI_BRANCH_CODE,
                        VA_BUSI_BRANCH_NAME AS VA_BUSI_BRANCH_NAME,
                        VA_AMC_ENABLED AS VA_AMC_ENABLED,
                        VA_SCHEM_ENABLED AS VA_SCHEM_ENABLED,
                        VA_IM_ENTRY_DT_ENABLED AS VA_IM_ENTRY_DT_ENABLED,
                        VA_EXPENSE_PER AS VA_EXPENSE_PER,
                        VA_EXPENSE_RS AS VA_EXPENSE_RS,
                        V_MYNAT AS VA_MYNAT
                    FROM DUAL;
                ELSE
                    OPEN P_CURSOR FOR
                    SELECT 
                        V_RETURN_MESSAGE AS V_RETURN_MESSAGE,
                        VM_BUSI_CODE AS VA_BUSI_CODE,
                        VM_AMC AS VA_BUSI_CODE,
                        VM_AMC_CODE AS VA_BUSI_CODE,
                        VM_SCH_NAME AS VA_BUSI_CODE,
                        VM_SCH_CODE AS VA_BUSI_CODE,
                        VM_ATM_TR AS VA_BUSI_CODE,
                        VM_SWITCH_AMC AS VA_BUSI_CODE,
                        VM_SWITCH_AMC_CODE AS VA_BUSI_CODE,
                        VM_SWITCH_SCH AS VA_BUSI_CODE,
                        VM_SWITCH_SCH_CODE AS VA_BUSI_CODE,
                        VM_CLIENT_CODE AS VA_BUSI_CODE,
                        VM_LABEL42 AS VA_BUSI_CODE,
                        VM_INVESTOR_NAME AS VA_BUSI_CODE,
                        VM_PAN AS VA_BUSI_CODE,
                        VM_AH_CODE AS VA_BUSI_CODE,
                        NULL AS VA_IM_ENTRY_DT, -- Not available for modification tab
                        NULL AS VA_IM_N_EXP_PER, -- Not available for modification tab
                        NULL AS VA_BUSI_BRANCH_CODE, -- Not available for modification tab
                        NULL AS VA_BUSI_BRANCH_NAME, -- Not available for modification tab
                        NULL AS VA_AMC_ENABLED, -- Not available for modification tab
                        NULL AS VA_SCHEM_ENABLED, -- Not available for modification tab
                        NULL AS VA_IM_ENTRY_DT_ENABLED, -- Not available for modification tab
                        VM_EXPENSE_PER AS VA_EXPENSE_PER,
                        VM_EXPENSE_RS AS VA_EXPENSE_RS,
                        V_MYNAT AS VA_MYNAT
                    FROM DUAL;
                END IF;

            ELSE
                OPEN P_CURSOR FOR SELECT 'ERROR: No matching document found' AS V_RETURN_MESSAGE FROM DUAL;
            END IF;
        ELSE
            
            DECLARE
                V_INVALID_DT_MSG VARCHAR(100);
            BEGIN   
                SELECT 
                    CASE WHEN TRAN_TYPE <> 'MF' THEN 'DT IS NOT A VALID TYPE'
                            WHEN VERIFICATION_FLAG <> '1' THEN 'DT IS NOT VERIFIED'
                            WHEN REJECTION_STATUS <> '0' THEN 'DT IS REJECTED'
                            WHEN PUNCHING_FLAG <> '0' THEN 'DT IS PUNCHED'
                            ELSE 'INCORRECT DT'
                END INTO V_INVALID_DT_MSG
                FROM TB_DOC_UPLOAD A, BRANCH_MASTER B
                WHERE A.BUSI_BRANCH_CODE = B.BRANCH_CODE
                AND COMMON_ID = TRIM(P_COMMON_ID)
                AND ROWNUM = 1;
                
                OPEN P_CURSOR FOR 
                SELECT 
                    CASE WHEN TRIM(V_INVALID_DT_MSG) IS NOT NULL THEN V_INVALID_DT_MSG
                         ELSE 'INCORRECT DT'
                    END AS V_RETURN_MESSAGE
                FROM DUAL;
                RETURN;
            END;            
        END IF;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            V_RETURN_MESSAGE := 'ERROR: Document not found or already processed';
            OPEN P_CURSOR FOR  
                SELECT V_RETURN_MESSAGE AS V_RETURN_MESSAGE FROM DUAL;
                RETURN;                
    END;    

 
EXCEPTION
    WHEN OTHERS THEN
        V_RETURN_MESSAGE := 'ERROR: ' || SQLERRM;
        OPEN P_CURSOR FOR SELECT V_RETURN_MESSAGE AS STATUS_MESSAGE FROM DUAL;
END PSM_MF2_GET_BY_DT;
/