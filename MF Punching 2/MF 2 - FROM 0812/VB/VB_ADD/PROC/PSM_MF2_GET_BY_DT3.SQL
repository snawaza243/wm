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
V_RETURN_MESSAGE VARCHAR2(4000);


BEGIN
     -- Get basic document information
    BEGIN
        SELECT A.BUSI_RM_CODE,A.BUSI_BRANCH_CODE,A.BUSI_TR_DATE,A.SCH_CODE,A.INV_CODE,B.BRANCH_NAME,A.EXPENSE 
        INTO   V_BUSI_RM_CODE, V_BUSI_BRANCH_CODE, V_BUSI_TR_DATE, V_SCH_CODE, V_INV_CODE, V_BRANCH_NAME, V_EXPENSE
        FROM TB_DOC_UPLOAD A,BRANCH_MASTER B WHERE A.BUSI_BRANCH_CODE=B.BRANCH_CODE AND COMMON_ID=TRIM(P_COMMON_ID) AND TRAN_TYPE='MF' AND VERIFICATION_FLAG='1' AND REJECTION_STATUS='0'  AND PUNCHING_FLAG='0' AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            OPEN P_CURSOR FOR SELECT 'ERROR: Document not found or already processed' AS V_RETURN_MESSAGE FROM DUAL;
            RETURN;
    END;

    IF record%COUNT  > 0 THEN

        -- Check approval status
        BEGIN
            SELECT WEALTHMAKER.FN_CHECK_FOR_APPROVAL_ALL(P_COMMON_ID)
            INTO V_APPROVAL_FLAG
            FROM DUAL;
            
            IF V_APPROVAL_FLAG = '2' THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Approval request already raised' AS V_RETURN_MESSAGE FROM DUAL;
                RETURN;
            ELSIF V_APPROVAL_FLAG = '4' THEN
                OPEN P_CURSOR FOR SELECT 'ERROR: Approval request rejected by management' AS V_RETURN_MESSAGE FROM DUAL;
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
                    OPEN P_CURSOR FOR SELECT 'WARNING: Scheme information not found' AS V_RETURN_MESSAGE FROM DUAL;
                    RETURN;
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
                    IF P_CHK_SWITCHA = '0' THEN 
                        VA_AMC := V_MUT_NAME;
                        VA_AMC_CODE := V_MUT_CODE;
                        VA_SCH_NAME := V_SCH_NAME;
                        VA_SCH_CODE := V_SCH_CODE;

                        -- CHECK ATM SCH TRANSACTION
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

                -- CHECK SCHEME NATURE
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

                -- CHECK ATM SCH TRANSACTION
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
                        VM_SWITCH_AMC := V_MUT_NAME;
                        VM_SWITCH_AMC_CODE := V_MUT_CODE;
                        VM_SWITCH_SCH := V_SCH_NAME;
                        VM_SWITCH_SCH_CODE := V_SCH_CODE;                  
                    END IF;
                END;
            END IF;
        ELSE 
            -- CmbAmcA_Click
            VA_AMC := '';
            VA_AMC_CODE := '';
            VA_SCH_NAME := '';
            VA_SCH_CODE := '';
            OPEN P_CURSOR FOR SELECT 'OPEN:AMCA' AS V_RETURN_MESSAGE FROM DUAL;
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
                        OPEN P_CURSOR FOR SELECT 'OPEN_ACCOUNT_OPENING_FORM#'||V_MAIN_CODE AS V_RETURN_MESSAGE FROM DUAL;
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
                        OPEN P_CURSOR FOR SELECT 'Account Opening Process For This Client Is Not Done. Punch Account Opening Form to do the Same' AS V_RETURN_MESSAGE FROM DUAL;
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
            IF P_INDEX = '0' THEN 
                OPEN P_CURSOR FOR SELECT 'EXECUTE_LOST_FOCUS:VA_BUSI_CODE' AS V_RETURN_MESSAGE FROM DUAL;
            ELSE
                OPEN P_CURSOR FOR SELECT 'EXECUTE_LOST_FOCUS:VM_BUSI_CODE' AS V_RETURN_MESSAGE FROM DUAL;
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

        VA_BUSI_CODE := V_BUSI_RM_CODE
        VA_IM_ENTRY_DT := V_BUSI_TR_DATE;

        IF P_ROL = '212' THEN
            VA_AMC_ENABLED := 'N';
            VA_SCHEM_ENABLED := 'N';
            VA_IM_ENTRY_DT_ENABLED := 'N';
        ELSE
            VA_AMC_ENABLED := 'Y';
            VA_SCHEM_ENABLED := 'Y';
            VA_IM_ENTRY_DT_ENABLED := 'Y';
        END IF;

        IF V_EXPENSE > '0' THEN
            VA_IM_N_EXP_PER := V_EXPENSE;
        ELSE
            VA_IM_N_EXP_PER := '0';
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
                OPEN P_CURSOR FOR SELECT 'OPEN_POPUP:CROSS_CHANNEL_VALIDATION' FROM DUAL;
                RETURN;
            ELSE
                -- FINALLY IF ALL VALIDAIOTN PASS AND NOT RETURN CURSOR THEN OPEN INVESTOR_ADDRESS_UPDATE POPUP 
                --OPEN P_CURSOR FOR SELECT 'OPEN_POPUP:INVESTOR_ADDRESS_UPDATE0' FROM DUAL;
                --RETURN;
            END IF;
        EXCEPTION
            WHEN OTHERS THEN
                OPEN P_CURSOR FOR SELECT 'WARNING: Cross-channel validation failed: ' || SQLERRM FROM DUAL;
                RETURN;
        END;

        -- RETURN ALL V___, VA__, VM__ VARIBALES WIHT A EXTRA COLUMN V_MESSAGE
        OPEN P_CURSOR FOR
        SELECT 
            'SUCCESS:OPEN_POPUP:INVESTOR_ADDRESS_UPDATE0' AS V_MESSAGE ,
            VA_BUSI_CODE AS VA_BUSI_CODE,
            ---
            ---
            -- AND SO ON ALL THE VARIABLES THAT AARE ADDED IN THE PROCEDRUE
            ---   VA_IM_ENTRY_DT,
            ---   VA_IM_N_EXP_PER,
            ---   VA_BUSI_BRANCH_CODE,
            ---   VA_BUSI_BRANCH_NAME ETC 
        FROM DUAL;

    END IF;
EXCEPTION
    WHEN OTHERS THEN
        V_RETURN_MESSAGE := 'ERROR: ' || SQLERRM;
        OPEN P_CURSOR FOR SELECT V_RETURN_MESSAGE AS  STATUS_MESSAGE FROM DUAL;
        RETURN;
END PROCESS_MF_TRANSACTION_DOC;
/