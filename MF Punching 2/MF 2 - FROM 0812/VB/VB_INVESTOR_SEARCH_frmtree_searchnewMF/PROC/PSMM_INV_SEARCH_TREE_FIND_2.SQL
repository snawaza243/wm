create or replace PROCEDURE PSMM_INV_SEARCH_TREE_FIND(
    PX_BRANCH           IN VARCHAR2,
    PX_CAT              IN VARCHAR2,
    PX_CITY             IN VARCHAR2,
    PX_CODE             IN VARCHAR2,
    PX_NAME             IN VARCHAR2,
    PX_ADD1             IN VARCHAR2,
    PX_ADD2             IN VARCHAR2,
    PX_PHONE            IN VARCHAR2,
    PX_PAN              IN VARCHAR2,
    PX_MOBILE           IN VARCHAR2,
    PX_NEW_RM           IN VARCHAR2,
    PX_AH_CODE          IN VARCHAR2,
    PX_CLIENT_SUB_NAME  IN VARCHAR2,
    PX_CLIENT_BROKER    IN VARCHAR2,
    PX_LOG_ID           IN VARCHAR2,
    PX_ROLE_ID          IN VARCHAR2,
    PX_STR_FORM       IN VARCHAR2,
    PX_CURRENT_FORM     IN VARCHAR2,
    PX_RM               IN VARCHAR2,
    PX_OLD_RM           IN VARCHAR2,
    PX_SORT             IN VARCHAR2,
    PX_CURSOR           OUT SYS_REFCURSOR
)
AS 
    V_CHK_INDIA             VARCHAR2(1);
    V_ALL_INDIA             VARCHAR2(1);
    V_ALL_INDIA_SEARCH_FLAG VARCHAR2(20);
    V_SRMCODE               VARCHAR2(100);
    V_BRANCHES              VARCHAR2(100);
    V_TEMP_SQL_REGION       VARCHAR2(100);
    V_GLB_DATA_FILTER       VARCHAR2(100);
    V_SQL1                  VARCHAR2(4000);
    V_SQL2                  VARCHAR2(4000);
    V_TEMP_SQL              VARCHAR2(4000);

BEGIN
    -- Initialize variables
    V_SRMCODE := '';
    V_BRANCHES := '';
    V_GLB_DATA_FILTER := '';

    IF PX_BRANCH = '' THEN 
        OPEN PX_CURSOR FOR SELECT 'Please Select Atleast One Branch' AS MSG FROM DUAL; 
        RETURN;
    END IF;

    IF PX_CAT IS NOT NULL THEN 
        IF PX_BRANCH = '' AND PX_CITY = '' AND PX_CODE = '' AND PX_NAME = '' AND PX_ADD1 = '' AND PX_ADD2 = '' AND PX_PHONE = '' AND PX_PAN = '' AND PX_MOBILE = '' AND PX_NEW_RM = '' AND PX_AH_CODE = '' THEN 
            IF PX_CAT = 'INVESTOR' AND PX_CLIENT_SUB_NAME = '' THEN
                OPEN PX_CURSOR FOR SELECT 'Please Enter Atleast One Searching Criteria !' AS MSG FROM DUAL; 
                RETURN;
            ELSIF PX_CAT IN ('CLIENT', 'AGENT') THEN
                OPEN PX_CURSOR FOR SELECT 'Please Enter Atleast One Searching Criteria !' AS MSG FROM DUAL; 
                RETURN;
            END IF;
        END IF;

        IF PX_NAME = '' THEN
            IF (PX_PAN <> '' OR PX_AH_CODE <> '') AND (PX_NAME = '' AND PX_ADD1 = '' AND PX_ADD2 = '' AND PX_PHONE = '' AND PX_MOBILE = '' AND PX_CODE = '') THEN
                V_CHK_INDIA := '1';
                V_ALL_INDIA := '1';
                V_ALL_INDIA_SEARCH_FLAG:= 'ALL';
            ELSE
                V_CHK_INDIA := '0';
                V_ALL_INDIA := '0';
                V_ALL_INDIA_SEARCH_FLAG:= 'SPECIFIC';
            END IF;
        ELSE
            IF PX_PAN = '' AND PX_AH_CODE = '' THEN
                V_CHK_INDIA := '0';
                V_ALL_INDIA := '0';
                V_ALL_INDIA_SEARCH_FLAG:= 'SPECIFIC';
            ELSE
                V_CHK_INDIA := '1';
                V_ALL_INDIA := '1';
                V_ALL_INDIA_SEARCH_FLAG:= 'ALL';
            END IF;
        END IF;

        IF V_CHK_INDIA = '1' THEN
            -- ALL_INDIA_SEARCH
            BEGIN 
                IF PX_CLIENT_BROKER = 'CLIENT' THEN
                    V_SQL1:= 'Select INVESTOR_name,INV_code,'' address1,'' address2,'' city_name,b.Branch_name,'' phone,e.rm_name,cm.guest_cd,a.pan,T.CLIENT_CODE FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,CLIENT_TEST T,client_master cm  where  cm.client_code=a.source_id and T.CLIENT_CODEKYC=A.INV_CODE AND a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL';
                    IF PX_PAN <> '' THEN
                        V_SQL1 := V_SQL1 || ' AND UPPER(A.PAN) = ''' || PX_PAN || ''' ';
                    END IF;
                    IF PX_AH_CODE <> '' THEN
                        V_SQL1 := V_SQL1 || ' AND UPPER(T.CLIENT_CODE) = ''' || PX_AH_CODE || ''' ';
                    END IF;
                    V_SQL1:=  V_SQL1 || ' ORDER BY UPPER(A.INVESTOR_NAME) ';
                ELSE 
                    V_SQL1 := ' Select INVESTOR_name,INV_code,'' address1,'' address2,'' city_name,b.Branch_name,'' phone,e.rm_name,'' guest_cd,a.pan,T.CLIENT_CODE FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,CLIENT_TEST T  where T.CLIENT_CODEKYC=A.INV_CODE AND a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL';
                    IF PX_PAN <> '' THEN
                        V_SQL1 := V_SQL1 || ' AND UPPER(A.PAN) = ''' || PX_PAN || ''' ';
                    END IF;
                    IF PX_AH_CODE <> '' THEN
                        V_SQL1 := V_SQL1 || ' AND UPPER(T.CLIENT_CODE) = ''' || PX_AH_CODE || ''' ';
                    END IF;
                END IF;
                OPEN PX_CURSOR FOR SELECT V_SQL1 FROM DUAL;
            END;
        ELSE
            -- SHOW_FILTER_DATA
            BEGIN 
                IF PX_STR_FORM <> 'Client Transter' THEN 
                    IF PX_CAT = 'INVESTOR' THEN
                        IF PX_CURRENT_FORM = 'frmtransactionmf' THEN
                            IF PX_CLIENT_BROKER = 'CLIENT' THEN
                                IF V_SRMCODE <> '' THEN
                                    SELECT branch_tar_cat INTO V_TEMP_SQL_REGION FROM branch_master WHERE branch_code = V_BRANCHES;
                                    IF V_TEMP_SQL_REGION IS NOT NULL AND V_TEMP_SQL_REGION IN ('187', '188') THEN
                                        V_SQL2:= ' Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,cm.GUEST_CD,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(' || V_SRMCODE || ') AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                    ELSE
                                        V_SQL2:= 'Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,cm.GUEST_CD,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                    END IF;
                                ELSE
                                    V_SQL2 := 'Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,cm.GUEST_CD,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                END IF;
                            ELSIF PX_CLIENT_BROKER = 'SUB BROKER' THEN
                                IF V_SRMCODE <> '' THEN
                                    SELECT branch_tar_cat INTO V_TEMP_SQL_REGION FROM branch_master WHERE branch_code = V_BRANCHES;
                                    IF V_TEMP_SQL_REGION IS NOT NULL AND V_TEMP_SQL_REGION IN ('187', '188') THEN
                                        V_SQL2 := ' Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(' || V_SRMCODE || ') AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                    ELSE
                                        V_SQL2:= ' Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                    END IF;
                                ELSE
                                    V_SQL2 := ' Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                END IF;
                            END IF;
                        ELSE
                            IF PX_CLIENT_BROKER = 'CLIENT' THEN
                                IF V_SRMCODE = '' THEN
                                    V_SQL2:= ' Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,cm.GUEST_CD,a.pan  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and cm.city_id=C.city_id(+) AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                ELSE
                                    V_SQL2:= ' Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,cm.GUEST_CD,a.pan  FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c,client_master cm where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(' || V_SRMCODE || ') AND cm.city_id=C.city_id(+) and  INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                END IF;
                            ELSIF PX_CLIENT_BROKER = 'SUB BROKER' THEN
                                IF V_SRMCODE = '' THEN
                                    V_SQL2:= ' Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                ELSE
                                    V_SQL2:= ' Select INVESTOR_name,INV_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name,a.pan FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in(' || V_SRMCODE || ') AND INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                                END IF;
                            END IF;
                        END IF; -- frmtransactionmf

                        IF PX_CLIENT_BROKER = 'CLIENT' THEN
                            V_SQL2 := V_SQL2 || ' and cm.client_code=a.source_id ';
                        END IF;

                        IF PX_CLIENT_BROKER = 'SUB BROKER' THEN
                            V_SQL2 := V_SQL2 || ' and lpad(a.inv_code,1)=3 ';
                            V_SQL2 := V_SQL2 || ' and lpad(a.inv_code,8) not in(select agent_code from agent_master where block_agent=''1'') ';
                        ELSIF PX_CLIENT_BROKER = 'CLIENT' THEN
                            V_SQL2 := V_SQL2 || ' and lpad(a.inv_code,1)=4 ';
                        END IF;

                        IF PX_CODE = '' AND PX_NAME = '' AND PX_ADD1 = '' AND PX_ADD2 = '' AND PX_PHONE = '' AND PX_PAN = '' AND PX_MOBILE = '' AND PX_BRANCH = '' AND PX_CITY = '' AND PX_CLIENT_SUB_NAME = '' THEN
                            V_SQL2 := V_SQL2 || ' and  B.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                        ELSE
                            IF PX_CODE <> '' THEN
                                IF PX_CLIENT_BROKER = 'SUB BROKER' THEN
                                    V_SQL2 := V_SQL2 || ' and source_id in (select agent_code from agent_master where (upper(exist_code) = '''|| PX_CODE ||''' OR TO_CHAR(agent_code)= '''|| PX_CODE ||''')) ';
                                ELSIF PX_CLIENT_BROKER = 'CLIENT' THEN
                                    V_SQL2 := V_SQL2 || ' and source_id in (select client_code from client_master where ( TO_CHAR(client_code)= '''|| PX_CODE ||''')) ';
                                END IF;
                            END IF;

                            IF PX_CLIENT_SUB_NAME <> '' THEN
                                IF PX_CLIENT_BROKER = 'SUB BROKER' THEN 
                                    V_SQL2 := V_SQL2 || ' and source_id in (select agent_code from agent_master where upper(trim(agent_name)) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_CLIENT_SUB_NAME)), ' ', '%') ||'''||''%'') ';
                                ELSIF PX_CLIENT_BROKER = 'CLIENT' THEN
                                    V_SQL2 := V_SQL2 || ' and source_id in (select client_code from client_master where upper(trim(client_name)) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_CLIENT_SUB_NAME)), ' ', '%') ||'''||''%'') ';
                                END IF;
                            END IF;

                            IF PX_NAME IS NOT NULL AND PX_NAME <> '' THEN
                                V_SQL2:= V_SQL2 || ' and upper(a.investor_name) like ''%''||''' || REPLACE(UPPER(TRIM(PX_NAME)), ' ', '%') || '''||''%'') ';
                            END IF;

                            IF PX_CLIENT_BROKER = 'SUB BROKER' THEN
                                IF PX_ADD1 <> '' THEN
                                    V_SQL2 := V_SQL2 || ' and upper(a.address1) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD1)), ' ', '%') || '''||''%'') ';
                                END IF;

                                IF PX_ADD2 <> '' THEN
                                    V_SQL2 := V_SQL2 || ' and upper(a.address2) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD2)), ' ', '%') || '''||''%'') ';
                                END IF;

                                IF PX_CITY <> '' THEN
                                    V_SQL2 := V_SQL2 || ' and a.city_id='''|| PX_CITY || ''' ';
                                END IF;

                            ELSIF PX_CLIENT_BROKER = 'CLIENT' THEN
                                IF PX_ADD1 <> '' THEN
                                    V_SQL2 := V_SQL2 || ' and upper(cm.address1) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD1)), ' ', '%') || '''||''%'') ';
                                END IF;

                                IF PX_ADD2 <> '' THEN
                                    V_SQL2 := V_SQL2 || ' and upper(cm.address2) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD2)), ' ', '%') || '''||''%'') ';
                                END IF;

                                IF PX_CITY <> '' THEN
                                    V_SQL2 := V_SQL2 || ' and CM.city_id='''|| PX_CITY || ''' ';
                                END IF;
                            END IF;

                            IF PX_PHONE <> '' THEN
                                V_SQL2 :=V_SQL2 || ' and upper(a.Phone) like ''%''||'''|| UPPER(TRIM(PX_PHONE)) || '''||''%'') ';
                            END IF;

                            IF PX_PAN <> '' THEN
                                V_SQL2 := V_SQL2 || ' AND UPPER(A.PAN) LIKE ''%''||''' || UPPER(TRIM(PX_PAN)) || '''||''%'') ';
                            END IF;

                            IF PX_MOBILE <> '' THEN
                                V_SQL2 := V_SQL2 || ' AND UPPER(A.MOBILE) = '''|| UPPER(TRIM(PX_MOBILE)) || ''' ';
                            END IF;

                            IF PX_BRANCH <> '' THEN
                                V_SQL2 := V_SQL2 || ' and B.BRANCH_CODE=''' || PX_BRANCH || ''' ';
                            ELSE
                                V_SQL2 := V_SQL2 || ' and B.BRANCH_CODE IN  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                            END IF;
                        END IF;

                        IF PX_NEW_RM <> '' THEN
                            V_SQL2 := V_SQL2 ||' and e.rm_code= '''|| PX_NEW_RM ||''' ';
                        END IF;
                    ELSIF PX_CAT = 'CLIENT' THEN
                        IF V_SRMCODE = '' THEN
                            IF V_GLB_DATA_FILTER = '72' THEN
                                V_SQL2 := ' Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL ';
                            ELSE
                                V_SQL2:= ' Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                            END IF;
                        ELSE
                            V_SQL2 := ' Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where  E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in('''|| V_SRMCODE ||''') AND a.city_id=C.city_id(+) and CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                        END IF;

                        IF PX_CODE = '' AND PX_NAME = '' AND PX_ADD1 = '' AND PX_ADD2 = '' AND PX_PHONE = '' AND PX_PAN = '' AND PX_MOBILE = '' AND PX_BRANCH = '' AND PX_CITY = '' THEN
                            V_SQL2:= V_SQL2 || ' and  B.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                        ELSE
                            IF PX_CODE <> '' THEN
                                V_SQL2 := V_SQL2 || ' and (upper(a.exist_code) = '''||UPPER(TRIM(PX_CODE))||''' or TO_CHAR(a.client_code)='''||TRIM(PX_CODE)||''') ';
                            END IF;

                            IF PX_NAME <> '' THEN
                                V_SQL2 := V_SQL2 || ' and upper(a.CLIENT_NAME) like ''%''||'''||REPLACE(UPPER(TRIM(PX_NAME)), ' ', '%') ||'''||''%'') ';
                            END IF;

                            IF PX_ADD1 <> '' THEN
                                V_SQL2 := V_SQL2 || ' and upper(a.address1) like ''%''||'''||REPLACE(UPPER(TRIM(PX_ADD1)), ' ', '%') ||'''||''%'') ';
                            END IF;

                            IF PX_ADD2<> '' THEN
                                V_SQL2 := V_SQL2 || ' and upper(a.address2) like ''%''||'''||REPLACE(UPPER(TRIM(PX_ADD2)), ' ', '%') ||'''||''%'') ';
                            END IF;

                            IF PX_PHONE <> '' THEN
                                V_SQL2 := V_SQL2 || ' and upper(a.Phone) like ''%''||''' || UPPER(TRIM(PX_PHONE)) || '''||''%'') ';
                            END IF;

                            IF PX_PAN <> '' THEN
                                V_SQL2 := V_SQL2 || '  and upper(a.Pan) like ''%''||'''|| UPPER(TRIM(PX_PAN)) || '''||''%'') ';
                            END IF;

                            IF PX_MOBILE <> '' THEN
                                V_SQL2 := V_SQL2 || ' and (a.mobile) = '''|| UPPER(TRIM(PX_MOBILE)) ||''' ';
                            END IF;

                            IF PX_CITY <> '' THEN
                                V_SQL2 := V_SQL2 || ' and a.city_id='''|| PX_CITY|| ''' ';
                            END IF;

                            IF PX_BRANCH <> '' THEN
                                V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE='''|| PX_BRANCH || ''' ';
                            ELSE
                                V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                            END IF;

                            IF PX_CURRENT_FORM = 'frmInvestorMerge' THEN
                                V_SQL2 := V_SQL2 || ' AND A.RM_CODE = '''|| PX_RM || ''' ';
                            END IF;

                            IF PX_CURRENT_FORM IN ('frmInvestorMerge','frmClientMerging')  AND PX_RM <> ''  THEN
                                V_SQL2 := V_SQL2 || ' AND A.RM_CODE = '''|| PX_RM || ''' ';
                            END IF;
                        END IF;

                        IF PX_NEW_RM <> '' THEN
                            V_SQL2:= V_SQL2 || ' AND E.RM_CODE = '''|| PX_NEW_RM || ''' ';
                        END IF;
                    ELSIF PX_CAT = 'AGENT' THEN
                        IF V_SRMCODE = '' THEN
                            V_SQL2:= ' Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                        ELSE
                            V_SQL2 := ' Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and E.source=b.branch_code AND a.RM_CODE=E.RM_CODE and E.RM_CODE in('''|| V_SRMCODE ||''') AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                        END IF;
                        
                        IF PX_CODE = '' AND PX_NAME = '' AND PX_ADD1 = '' AND PX_ADD2 = '' AND PX_PHONE = '' AND PX_PAN = '' AND PX_MOBILE = '' AND PX_BRANCH = '' AND PX_CITY = '' THEN
                            V_SQL2:= V_SQL2 || ' and  B.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                        ELSE
                            IF PX_CODE <> '' THEN
                                V_SQL2:= V_SQL2 || ' and (upper(a.exist_code) = '''||UPPER(PX_CODE)||''' or to_char(a.agent_code)='''|| PX_CODE ||''') ';
                            END IF;

                            IF PX_NAME <> '' THEN
                                V_SQL2:= V_SQL2 || ' and upper(a.agent_NAME) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_NAME)), ' ', '%') ||'''||''%'') ';
                            END IF;

                            IF PX_ADD1 <> '' THEN
                                V_SQL2:= V_SQL2 || ' and upper(a.ADDRESS1) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD1)), ' ', '%') ||'''||''%'') ';
                            END IF;

                            IF PX_ADD2 <> '' THEN
                                V_SQL2:= V_SQL2 || ' and upper(a.ADDRESS2) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD2)), ' ', '%') ||'''||''%'') ';
                            END IF;

                            IF PX_PHONE <> '' THEN
                                V_SQL2:= V_SQL2 || ' and upper(a.Phone) like ''%''||''' || UPPER(TRIM(PX_PHONE)) || '''||''%'') ';
                            END IF;

                            IF PX_PAN <> '' THEN
                                V_SQL2:= V_SQL2 || ' and upper(a.Pan) like ''%''||''' || UPPER(TRIM(PX_PAN)) || '''||''%'') ';
                            END IF;

                            IF PX_MOBILE <> '' THEN
                                V_SQL2:= V_SQL2 || ' and (a.Mobile) = '''|| UPPER(TRIM(PX_MOBILE)) || ''' ';
                            END IF;

                            IF PX_CITY <> '' THEN
                                V_SQL2:= V_SQL2 || ' and a.city_id='''|| PX_CITY|| ''' ';
                            END IF;

                            IF PX_BRANCH <> '' THEN
                                V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE='''||PX_BRANCH || ''' ';
                            ELSE
                                V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                            END IF;

                            IF PX_CURRENT_FORM = 'frmInvestorMerge' AND PX_RM <> '' THEN
                                V_SQL2:= V_SQL2 || ' and A.RM_CODE ='''|| PX_RM ||''' ';
                            END IF;
                        END IF;

                        IF PX_NEW_RM <> '' AND PX_RM <> '' THEN
                            V_SQL2 := V_SQL2 || ' and A.RM_CODE ='''||PX_RM ||''' ';
                        END IF;
                        V_SQL2 := V_SQL2 || ' ORDER BY upper(a.agent_NAME) ';
                    END IF; -- CLOSING: INVESTOR, CLIEND, AGENT
                ELSE
                    IF PX_CAT = 'CLIENT' THEN 
                        V_SQL2:= ' Select CLIENT_name,CLIENT_CODE,a.address1,a.address2,C.CITY_NAME,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,CLIENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND CLIENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                        V_SQL2:= V_SQL2 || ' and A.FROM_RM_CODE ='''|| PX_OLD_RM || ''' '; -- OLD RM DDL INDEX 2
                    
                        IF PX_CODE <> '' THEN
                            V_SQL2 := V_SQL2 || ' and (upper(a.exist_code) = '''||UPPER(TRIM(PX_CODE))||''' ';
                        END IF;

                        IF PX_NAME <> '' THEN
                            V_SQL2 := V_SQL2 || ' and upper(a.CLIENT_NAME) like ''%''||'''||REPLACE(UPPER(TRIM(PX_NAME)), ' ', '%') ||'''||''%'') ';
                        END IF;

                        IF PX_ADD1 <> '' THEN
                            V_SQL2 := V_SQL2 || ' and upper(a.address1) like ''%''||'''||REPLACE(UPPER(TRIM(PX_ADD1)), ' ', '%') ||'''||''%'') ';
                        END IF;

                        IF PX_ADD2<> '' THEN
                            V_SQL2 := V_SQL2 || ' and upper(a.address2) like ''%''||'''||REPLACE(UPPER(TRIM(PX_ADD2)), ' ', '%') ||'''||''%'') ';
                        END IF;

                        IF PX_PHONE <> '' THEN
                            V_SQL2 := V_SQL2 || ' and upper(a.Phone) like ''%''||''' || UPPER(TRIM(PX_PHONE)) || '''||''%'') ';
                        END IF;

                        IF PX_PAN <> '' THEN
                            V_SQL2 := V_SQL2 || '  and upper(a.Pan) like ''%''||'''|| UPPER(TRIM(PX_PAN)) || '''||''%'') ';
                        END IF;

                        IF PX_MOBILE <> '' THEN
                            V_SQL2 := V_SQL2 || ' and (a.mobile) = '''|| UPPER(TRIM(PX_MOBILE)) ||''' ';
                        END IF;

                        IF PX_CITY <> '' THEN
                            V_SQL2 := V_SQL2 || ' and a.city_id='''|| PX_CITY|| ''' ';
                        END IF;

                        IF PX_BRANCH <> '' THEN
                            V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE='''|| PX_BRANCH || ''' ';
                        ELSE
                            V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE in (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                        END IF;

                        IF PX_NEW_RM <> '' THEN
                            V_SQL2:= V_SQL2 || ' and e.rm_code=''' || PX_NEW_RM || ''' '; -- NEW RM INDEX 2
                        END IF;

                        IF PX_SORT = 'NAME' THEN
                            V_SQL2 := V_SQL2 || ' ORDER BY upper(a.CLIENT_NAME) ';
                        ELSIF PX_SORT = 'ADDRESS1' THEN 
                            V_SQL2 := V_SQL2 || ' ORDER BY upper(trim(a.Address1)) ';
                        ELSIF PX_SORT = 'ADDRESS2' THEN 
                            V_SQL2 := V_SQL2 || ' ORDER BY upper(trim(a.Address2)) ';
                        ELSIF PX_SORT = 'CITY' THEN 
                            V_SQL2 := V_SQL2 || ' ORDER BY upper(trim(C.CITY_NAME)) ';
                        ELSIF PX_SORT = 'PHONE' THEN 
                            V_SQL2 := V_SQL2 || ' ORDER BY upper(trim(a.phone)) ';
                        END IF;
                    
                    ELSIF PX_CAT = 'AGENT' THEN
                        V_SQL2:= ' Select AGENT_name,AGENT_code,a.address1,a.address2,c.city_name,b.Branch_name,DECODE(a.client_type,''RELIGARE'',''RELIGARE'',NULL),e.rm_name FROM Branch_master b,EMPLOYEE_MASTER E,AGENT_master a,City_master c where a.city_id=C.city_id(+) and A.SOURCEID=b.branch_code AND a.RM_CODE=E.RM_CODE AND AGENT_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257 ';
                        V_SQL2:= V_SQL2 || ' and A.FROM_RM_CODE ='''|| PX_OLD_RM || ''' '; -- OLD RM INDEX 2

                        IF PX_CODE <> '' THEN
                            V_SQL2:= V_SQL2 || ' and (upper(a.exist_code) = '''||UPPER(PX_CODE)||''' ';
                        END IF;

                        IF PX_NAME <> '' THEN
                            V_SQL2:= V_SQL2 || ' and upper(a.agent_NAME) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_NAME)), ' ', '%') ||'''||''%'') ';
                        END IF;

                        IF PX_ADD1 <> '' THEN
                            V_SQL2:= V_SQL2 || ' and upper(a.ADDRESS1) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD1)), ' ', '%') ||'''||''%'') ';
                        END IF;

                        IF PX_ADD2 <> '' THEN
                            V_SQL2:= V_SQL2 || ' and upper(a.ADDRESS2) like ''%''||'''|| REPLACE(UPPER(TRIM(PX_ADD2)), ' ', '%') ||'''||''%'') ';
                        END IF;

                        IF PX_PHONE <> '' THEN
                            V_SQL2:= V_SQL2 || ' and upper(a.Phone) like ''%''||''' || UPPER(TRIM(PX_PHONE)) || '''||''%'') ';
                        END IF;

                        IF PX_PAN <> '' THEN
                            V_SQL2:= V_SQL2 || ' and upper(a.Pan) like ''%''||''' || UPPER(TRIM(PX_PAN)) || '''||''%'') ';
                        END IF;

                        IF PX_MOBILE <> '' THEN
                            V_SQL2:= V_SQL2 || ' and (a.Mobile) = '''|| UPPER(TRIM(PX_MOBILE)) || ''' ';
                        END IF;

                        IF PX_CITY <> '' THEN
                            V_SQL2:= V_SQL2 || ' and a.city_id='''|| PX_CITY|| ''' ';
                        END IF;

                        IF PX_BRANCH <> '' THEN
                            V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE='''||PX_BRANCH || ''' ';
                        ELSE
                            V_SQL2:= V_SQL2 || ' and B.BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG_ID || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE_ID||''') ';
                        END IF;

                        IF PX_NEW_RM <> '' THEN
                            V_SQL2 := V_SQL2 || ' and E.RM_CODE ='''||PX_NEW_RM ||''' '; --  AS PER OLD VB NEW_RM INDEX 2
                        END IF;
                        V_SQL2:= V_SQL2 || ' ORDER BY upper(a.agent_NAME) ';
                    END IF; -- CLOSING: CLISNT, AGENT OF ELSE PART OF PX_STR_FORM <> 'Client Transter'
                END IF; -- CLOSING: CLIENT TRANSFER

                OPEN PX_CURSOR FOR  SELECT V_SQL2 FROM DUAL;
                RETURN;
            END; -- CLOSING: SHOW_FILTER_DATA
        END IF;
    ELSE
        OPEN PX_CURSOR FOR SELECT 'Please Select Category-->' || PX_CAT AS MSG FROM DUAL;
        RETURN;
    END IF;

END PSMM_INV_SEARCH_TREE_FIND;