CREATE OR REPLACE PROCEDURE PROCESS_MF_TRANSACTION_DOC(
    P_INDEX         IN VARCHAR2,
    P_COMMON_ID     IN VARCHAR2,
    P_LOGIN_ID      IN VARCHAR2,
    P_ROLE_ID       IN VARCHAR2,
    P_CHK_SWITCHA    IN VARCHAR2,
    P_CURSOR        OUT SYS_REFCURSOR,
    P_MESSAGE       OUT VARCHAR2
)
AS
    V_BUSI_RM_CODE VARCHAR2(50);
    V_BUSI_BRANCH_CODE VARCHAR2(50);
    V_BUSI_TR_DATE DATE;
    V_SCH_CODE VARCHAR2(50);
    V_INV_CODE VARCHAR2(50);
    V_BRANCH_NAME VARCHAR2(100);
    V_EXPENSE NUMBER;

    V_APPROVAL_FLAG VARCHAR2(1);

    V_SCH_NAME VARCHAR2(200);
    V_MUT_NAME VARCHAR2(100);
    V_MUT_CODE VARCHAR2(100);

    V_CHEK_SCH_NATURE VARCHAR2(1);
    V_CHK_ATM_TR VARCHAR2(1);






    V_INVESTOR_NAME VARCHAR2(200);
    V_PAN VARCHAR2(20);
    V_NATURE VARCHAR2(1);
    V_CROSS_CHANEL_COUNT NUMBER;
    V_LOGGED_USERID VARCHAR2(50);
    V_MAIN_CODE VARCHAR2(50);
    V_UPD_PROC VARCHAR2(1);
    V_ATM_SCHEME BOOLEAN;
    V_RM_NAME VARCHAR2(100);

    V_EXPENSE_PER VARCHAR2(10);
    V_EXPENSE_RS VARCHAR2(20);
    V_MY_NAT VARCHAR2(1);
    V_CHK_ATM_TR VARCHAR2(1);



    VM_BUSI_RM_CODE VARCHAR2(50);
    VM_SCH_CODE VARCHAR2(50);
    VM_INV_CODE VARCHAR2(50);
    VM_BUSI_BRANCH_CODE VARCHAR2(50);
    VM_BUSI_TR_DATE DATE;
    VM_EXPENSE NUMBER;
    VM_BRANCH_NAME VARCHAR2(100);
    VM_SCH_NAME VARCHAR2(200);
    VM_MUT_NAME VARCHAR2(100);
    VM_INVESTOR_NAME VARCHAR2(200);
    VM_PAN VARCHAR2(20);
    VM_NATURE VARCHAR2(1);
    VM_APPROVAL_FLAG VARCHAR2(1);
    VM_CROSS_CHANEL_COUNT NUMBER;
    VM_LOGGED_USERID VARCHAR2(50);
    VM_MAIN_CODE VARCHAR2(50);
    VM_UPD_PROC VARCHAR2(1);
    VM_ATM_SCHEME BOOLEAN;
    VM_RM_NAME VARCHAR2(100);
    VM_CHK_ATM_TR VARCHAR2(1);
    VM_SWITCH_AMC VARCHAR2(100);
    VM_SWITCH_SCH VARCHAR2(100);
    VM_SWITCH_AMC_CODE VARCHAR2(100);
    VM_SWITCH_SCH_CODE VARCHAR2(100);


BEGIN
     -- Get basic document information
    BEGIN
        SELECT A.BUSI_RM_CODE,A.BUSI_BRANCH_CODE,A.BUSI_TR_DATE,A.SCH_CODE,A.INV_CODE,B.BRANCH_NAME,A.EXPENSE 
        INTO   V_BUSI_RM_CODE, V_BUSI_BRANCH_CODE, V_BUSI_TR_DATE, V_SCH_CODE, V_INV_CODE, V_BRANCH_NAME, V_EXPENSE
        FROM TB_DOC_UPLOAD A,BRANCH_MASTER B WHERE A.BUSI_BRANCH_CODE=B.BRANCH_CODE AND COMMON_ID=TRIM(P_COMMON_ID) AND TRAN_TYPE='MF' AND VERIFICATION_FLAG='1' AND REJECTION_STATUS='0'  AND PUNCHING_FLAG='0' AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Document not found or already processed' AS MESSAGE FROM DUAL;
            RETURN;
    END;

    IF record%COUNT  > 0 THEN
        -- Check approval status
        BEGIN
            SELECT WEALTHMAKER.FN_CHECK_FOR_APPROVAL_ALL(P_COMMON_ID)
            INTO V_APPROVAL_FLAG
            FROM DUAL;
            
            IF V_APPROVAL_FLAG = '2' THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Approval request already raised' AS MESSAGE FROM DUAL;
                RETURN;
            ELSIF V_APPROVAL_FLAG = '4' THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Approval request rejected by management' AS MESSAGE FROM DUAL;
                RETURN;
            END IF;
        END;



        -- Get scheme information
        IF V_SCH_CODE IS NOT NULL THEN
            BEGIN
                SELECT SCH_CODE, SCH_NAME, MUT_NAME, MUT_CODE
                INTO V_SCH_CODE, V_SCH_NAME, V_MUT_NAME, V_MUT_CODE
                FROM (SELECT OSCH_CODE SCH_CODE,OSCH_NAME SCH_NAME,LONGNAME SHORT_NAME,NAME,ISS_NAME MUT_NAME,'' MUT_CODE FROM OTHER_PRODUCT O, PRODUCT_MASTER P,ISS_MASTER I WHERE OSCH_CODE =V_SCH_CODE AND O.PROD_CLASS_CODE=P.PROD_CODE AND O.ISS_CODE=I.ISS_CODE AND (I.BRANCH_CODE<>'N' OR I.BRANCH_CODE IS NULL) AND (O.FLAG<>'N' OR O.FLAG IS NULL)
                UNION ALL
                SELECT SCH_CODE,SCH_NAME,SHORT_NAME,'MF' NAME,MUT_NAME, M.MUT_CODE FROM SCHEME_INFO S,MUT_FUND M WHERE S.SCH_CODE =V_SCH_CODE AND S.MUT_CODE=M.MUT_CODE AND M.MUT_CODE NOT IN ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') AND S.SCH_CODE IN (SELECT SCH_CODE FROM MP_SCHEME_MASTER SM WHERE SM.ACTIVE_STATUS='Active' )
                ) WHERE ROWNUM=1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    P_MESSAGE := 'WARNING: Scheme information not found';
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
                        V_EXPENSE_PER := 'Expenses Trail%';
                        V_EXPENSE_RS  := 'Expenses Trail Rs.';
                        V_MYNAT      := 'C';
                    END IF;
                END;

                BEGIN
                    IF P_CHK_SWITCHA = '0' THEN 
                        VA_AMC := V_MUT_NAME;
                        VA_AMC_CODE := V_MUT_CODE;
                        VA_SCH_NAME := V_SCH_NAME;
                        VA_SCH_CODE := V_SCH_CODE;

                        --If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                        SELECT NVL(COUNT(*),0) INTO V_CHK_ATM_TR 
                        FROM RELIANCE_ATM_MASTER WHERE 
                        SCH_CODE = V_SCH_CODE AND FROM_DT<=SYSDATE AND (TO_DT>=SYSDATE OR TO_DT IS NULL);

                        IF V_CHK_ATM_TR >0 THEN
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
                BEGIN
                    SELECT NVL(GET_SCHEME_NATURE(V_SCH_CODE), '') AS A
                    INTO V_CHEK_SCH_NATURE
                    FROM DUAL;

                    IF V_CHEK_SCH_NATURE = 'O' THEN
                        VA_EXPENSE_PER := 'Expenses agnst. Trail%';
                        VA_EXPENSE_RS  := 'Expenses agnst. Trail Rs.';
                        V_MYNAT      := 'O';
                    ELSE
                        V_EXPENSE_PER := 'Expenses Trail%';
                        V_EXPENSE_RS  := 'Expenses Trail Rs.';
                        V_MYNAT      := 'C';
                    END IF;
                END;

                BEGIN
                    IF P_CHK_SWITCHM = '0' THEN 
                        VM_AMC := V_MUT_NAME;
                        VM_AMC_CODE := V_MUT_CODE;
                        VM_SCH_NAME := V_SCH_NAME;
                        VM_SCH_CODE := V_SCH_CODE;

                        --If get_ATM_scheme(UCase(rsFind("sch_code"))) = True Then
                        SELECT NVL(COUNT(*),0) INTO V_CHK_ATM_TR 
                        FROM RELIANCE_ATM_MASTER WHERE 
                        SCH_CODE = V_SCH_CODE AND FROM_DT<=SYSDATE AND (TO_DT>=SYSDATE OR TO_DT IS NULL);

                        IF V_CHK_ATM_TR >0 THEN
                            VM_ATM_TR := '1';
                        ELSE
                            VM_ATM_TR := '0';
                        END IF;
                    ELSE
                        VA_SWITCH_AMC := V_MUT_NAME;
                        VA_SWITCH_AMC_CODE := V_MUT_CODE;
                        VA_SWITCH_SCH := V_SCH_NAME;
                        VA_SWITCH_SCH_CODE := V_SCH_CODE;                  
                    END IF;
                END;

            END IF;
        ELSE 
            VM_AMC := '';
            VM_AMC_CODE := '';
            VM_SCH_NAME := '';
            VM_SCH_CODE := '';
            OPEN P_CURSOR FOR SELECT 'OPEN:AMCA' AS MESSAGE FROM DUAL;
        END IF;

        -- Get investor information
        IF V_INV_CODE IS NOT NULL THEN

            Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) CLIENT_TYPE,e.rm_name,'' BLANK,a.pan 
            INTO V2_INVESTOR_NAME, V2_INV_CODE,V2_address1,V2_address2,V2_city_name,V2_Branch_name,V2_CLIENT_TYPE,V2_RM_NAME,V2_BLANK,V2_PAN
            FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) 
            and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 and a.inv_code=V_INV_CODE AND ROWNUM=1;
            

            select loggeduserid, main_code
            INTO V_loggeduserid, V_MAIN_CODE
            from client_test where client_codekyc = V_INV_CODE;

            IF V_loggeduserid = 'PROC' THEN
                select NVL(UPD_PROC,'N') INTO V_MyUpdProc from client_test where client_codekyc=V_INV_CODE and dob is not null;
                IF (SUBSTR(V_INV_CODE, 1, 1) <> '3') THEN
                    IF V_MyUpdProc IN ('N','0') THEN
                        --Some Mandatory Information Needs To Be Filled Before Punching Any Transaction Of This Account
                        OPEN P_CURSOR FOR SELECT 'OPEN_ACCOUNT_OPENING_FORM' AS MESSAGE FROM DUAL;
                        RETURN;
                    END IF;
                END IF;
            END IF;

            IF P_INDEX = '0' THEN
            -- frmtransactionmf.TxtClientCodeA = Mid(rsGetDocPath("Inv_code"), 1, 8)

                VA_CLIENT_CODE := SUBSTR(V_INV_CODE,1,8);
                VA_LABEL32 := V_INV_CODE;
                VA_INVESTOR_NAME := V2_INVESTOR_NAME;

                IF SUBSTR(V_INV_CODE,1,1) = '4' THEN


                    select payroll_id INTO VA_BUSI_CODE from employee_master where rm_code=(select rm_code from client_master where client_code=SUBSTR(V_INV_CODE,1,8));
                    select client_code INTO VA_AH_CODE from client_test where client_codekyc=V_INV_CODE;

                    VA_PAN := V2_PAN;
                    VA_BUSI_CODE := VA_BUSI_CODE;
                    VA_AH_CODE := VA_AH_CODE;

                    IF VA_AH_CODE IS NULL OR LENGTH(VA_AH_CODE) <6 THEN
                        OPEN P_CURSOR FOR SELECT 'Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same' AS MESSAGE FROM DUAL;
                        RETURN;
                    END IF;
                ELSE
                    VA_PAN := V2_PAN;
                    VA_BUSI_CODE := V_BUSI_RM_CODE;
                END IF;
            ELSE IF P_INDEX = '1' THEN
                VM_CLIENT_CODE := SUBSTR(V_INV_CODE,1,8);
                VM_INVESTOR_NAME := V2_INVESTOR_NAME;

                IF(SUBSTR(V_INV_CODE,1,1) = '4') THEN
                    SELECT client_code INTO VM_AH_CODE FROM client_test WHERE client_codekyc = V_INV_CODE;
                    SELECT payroll_id INTO VM_BUSI_CODE FROM employee_master WHERE rm_code=(SELECT rm_code FROM client_master WHERE client_code=SUBSTR(V_INV_CODE,1,8));
                    VM_PAN := V2_PAN;
                    VM_AH_CODE := VM_AH_CODE;
                    VM_BUSI_CODE := VM_BUSI_CODE;
                ELSE
                    SELECT client_code INTO VM_AH_CODE FROM client_test WHERE client_codekyc = V_INV_CODE;
                    SELECT payroll_id INTO VM_BUSI_CODE FROM employee_master WHERE rm_code=(SELECT rm_code FROM client_master WHERE client_code=SUBSTR(V_INV_CODE,1,8));
                    VM_PAN := V2_PAN;
                    VM_AH_CODE := VM_AH_CODE;
                    VM_BUSI_CODE := VM_BUSI_CODE;

                END IF;

                VM_Label42 := V_INV_CODE;

            END IF;

        -- Cross-channel validation
        BEGIN
            WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO(
                P_COMMON_ID, 
                V_INV_CODE, 
                P_LOGIN_ID, 
                V_CROSS_CHANEL_COUNT
            );
            
            IF V_CROSS_CHANEL_COUNT > 0 THEN
                P_MESSAGE := 'OPEN_CROSS_CHANNEL_VALIDATION';
                RETURN;
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                P_MESSAGE := 'WARNING: Cross-channel validation failed: ' || SQLERRM;
        END;

        -- Return all data through cursor
        OPEN P_CURSOR FOR
        SELECT 
            V_SCH_CODE AS SCH_CODE,
            V_SCH_NAME AS SCH_NAME,
            V_MUT_NAME AS MUT_NAME,
            V_NATURE AS NATURE,
            V_INV_CODE AS INV_CODE,
            V_INVESTOR_NAME AS INVESTOR_NAME,
            V_PAN AS PAN,
            V_BUSI_RM_CODE AS BUSI_RM_CODE,
            V_BUSI_BRANCH_CODE AS BUSI_BRANCH_CODE,
            V_BUSI_TR_DATE AS BUSI_TR_DATE,
            V_EXPENSE AS EXPENSE,
            V_BRANCH_NAME AS BRANCH_NAME,
            V_LOGGED_USERID AS LOGGED_USERID,
            V_MAIN_CODE AS MAIN_CODE,
            V_RM_NAME AS RM_NAME,
            CASE WHEN V_ATM_SCHEME THEN 'Y' ELSE 'N' END AS ATM_SCHEME,
            'SUCCESS' AS STATUS_MESSAGE
        FROM DUAL;

        P_MESSAGE := 'SUCCESS';
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        P_MESSAGE := 'ERROR: ' || SQLERRM;
        OPEN P_CURSOR FOR
        SELECT 
            NULL AS SCH_CODE,
            NULL AS SCH_NAME,
            NULL AS MUT_NAME,
            NULL AS NATURE,
            NULL AS INV_CODE,
            NULL AS INVESTOR_NAME,
            NULL AS PAN,
            NULL AS BUSI_RM_CODE,
            NULL AS BUSI_BRANCH_CODE,
            NULL AS BUSI_TR_DATE,
            NULL AS EXPENSE,
            NULL AS BRANCH_NAME,
            NULL AS LOGGED_USERID,
            NULL AS MAIN_CODE,
            NULL AS RM_NAME,
            NULL AS ATM_SCHEME,
            'ERROR: ' || SQLERRM AS STATUS_MESSAGE
        FROM DUAL;
END PROCESS_MF_TRANSACTION_DOC;
/