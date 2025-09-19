CREATE OR REPLACE PROCEDURE PSMM_SHEME_SEARCH_GET (
    PX_SCH_STR      IN VARCHAR2,
    PX_LOG          IN VARCHAR2,
    PX_ROLE         IN VARCHAR2,
    PX_CURSOR       OUT SYS_REFCURSOR
) AS

V_QUERY VARCHAR2(4000);
V_STR_MF  VARCHAR2(100);
V_CODE_MF VARCHAR2(20);
V_STR_SEARCH VARCHAR2(100);

BEGIN

    V_STR_SEARCH := UPPER(TRIM(PX_SCH_STR));
    Select name,PROD_CODE into V_STR_MF,V_CODE_MF from product_master where nature_code='NT001';

    IF PX_ROLE = '1' THEN
        V_QUERY := '
            SELECT
                ''MF'' AS PRODUCT_TYPE,
                S.SCH_CODE,
                S.SCH_NAME,
                S.SHORT_NAME,
                M.MUT_NAME,
                ''' || V_STR_MF || ''' AS PROD_NAME,
                ''' || V_CODE_MF || ''' AS PROD_CODE
            FROM SCHEME_INFO S, MUT_FUND M
            WHERE UPPER(M.MUT_NAME) || UPPER(S.SCH_NAME) || UPPER(M.MUT_NAME) LIKE ''%' || V_STR_SEARCH || '%''
              AND S.MUT_CODE = M.MUT_CODE
              AND M.MUT_CODE NOT IN (''MF001'',''MF036'',''MF002'',''MF013'',''MF039'',''MF029'',''MF027'',''MF035'',''MF020'')
            UNION ALL
            SELECT
                ''OTHER'' AS PRODUCT_TYPE,
                O.OSCH_CODE AS SCH_CODE,
                O.OSCH_NAME AS SCH_NAME,
                O.LONGNAME AS SHORT_NAME,
                I.ISS_NAME AS MUT_NAME,
                P.NAME AS PROD_NAME,
                P.PROD_CODE AS PROD_CODE
            FROM OTHER_PRODUCT O, PRODUCT_MASTER P, ISS_MASTER I
            WHERE UPPER(I.ISS_NAME) || UPPER(O.LONGNAME) || UPPER(I.ISS_NAME) LIKE ''%' || V_STR_SEARCH || '%''
              AND O.PROD_CLASS_CODE = P.PROD_CODE
              AND O.ISS_CODE = I.ISS_CODE
              AND (I.BRANCH_CODE <> ''N'' OR I.BRANCH_CODE IS NULL)
              AND (O.FLAG <> ''N'' OR O.FLAG IS NULL)
            ORDER BY SCH_NAME, SHO RT_NAME
        ';
    ELSE
        V_QUERY := '
            SELECT
                ''MF'' AS PRODUCT_TYPE,
                S.SCH_CODE,
                S.SCH_NAME,
                S.SHORT_NAME,
                M.MUT_NAME,
                ''' || V_STR_MF || ''' AS PROD_NAME,
                ''' || V_CODE_MF || ''' AS PROD_CODE
            FROM SCHEME_INFO S, MUT_FUND M
            WHERE UPPER(M.MUT_NAME) || UPPER(S.SCH_NAME) || UPPER(M.MUT_NAME) LIKE ''%' || V_STR_SEARCH || '%''
              AND S.MUT_CODE = M.MUT_CODE
              AND M.MUT_CODE NOT IN (''MF001'',''MF036'',''MF002'',''MF013'',''MF039'',''MF029'',''MF027'',''MF035'',''MF020'')
              AND EXISTS (
                SELECT SCH_CODE FROM SCH_CITY_MAPPING
                WHERE SCH_CODE = S.SCH_CODE
                  AND (CITY_ID IN (
                        SELECT DISTINCT CITY_ID FROM BRANCH_MASTER
                        WHERE BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE||''')
                      ) OR CITY_ID = ''ALL'')
                  AND ISS_CODE LIKE ''' || V_CODE_MF || '%''
              )
            UNION ALL
            SELECT
                ''OTHER'' AS PRODUCT_TYPE,
                O.OSCH_CODE AS SCH_CODE,
                O.OSCH_NAME AS SCH_NAME,
                O.LONGNAME AS SHORT_NAME,
                I.ISS_NAME AS MUT_NAME,
                P.NAME AS PROD_NAME,
                P.PROD_CODE AS PROD_CODE
            FROM OTHER_PRODUCT O, PRODUCT_MASTER P, ISS_MASTER I
            WHERE UPPER(I.ISS_NAME) || UPPER(O.LONGNAME) || UPPER(I.ISS_NAME) LIKE ''%' || V_STR_SEARCH || '%''
              AND O.PROD_CLASS_CODE = P.PROD_CODE
              AND O.ISS_CODE = I.ISS_CODE
              AND (I.BRANCH_CODE <> ''N'' OR I.BRANCH_CODE IS NULL)
              AND (O.FLAG <> ''N'' OR O.FLAG IS NULL)
              AND O.OSCH_CODE IN (
                SELECT SCH_CODE FROM SCH_CITY_MAPPING
                WHERE (CITY_ID IN (
                        SELECT DISTINCT CITY_ID FROM BRANCH_MASTER
                        WHERE BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE||''')
                      ) OR CITY_ID = ''ALL'')
                  AND (ISS_CODE LIKE ''IS%'' OR ISS_CODE LIKE ''MFIS%'')
              )
            ORDER BY SCH_NAME, SHORT_NAME
        ';
    END IF;

    OPEN PX_CURSOR FOR V_QUERY;

END PSMM_SHEME_SEARCH_GET;