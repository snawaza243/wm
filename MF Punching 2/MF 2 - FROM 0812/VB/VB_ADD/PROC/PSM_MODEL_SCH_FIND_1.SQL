CREATE OR REPLACE PROCEDURE PSM_MODEL_SCH_FIND_1 (
    P_FIND_TEXT     IN  VARCHAR2,
    P_LOGIN         IN  VARCHAR2,
    P_ROLE          IN  VARCHAR2,
    P_RESULT        OUT SYS_REFCURSOR
) AS
    V_TEXT       VARCHAR2(4000);
    V_NAME       VARCHAR2(100);
    V_PROD_CODE  VARCHAR2(100);
    V_QUERY      VARCHAR2(4000);
BEGIN
    -- Normalize search text
    V_TEXT := CONVERTSTRING(UPPER(TRIM(P_FIND_TEXT)));

    -- Get product name and code
    SELECT NAME, PROD_CODE INTO V_NAME, V_PROD_CODE
    FROM PRODUCT_MASTER
    WHERE NATURE_CODE = 'NT001';

    -- Build query based on role
    IF P_ROLE = '1' THEN
        V_QUERY := 
            'SELECT SCH_CODE, SCH_NAME, SHORT_NAME, MUT_NAME ' ||
            'FROM SCHEME_INFO S, MUT_FUND M ' ||
            'WHERE UPPER(MUT_NAME) || UPPER(SCH_NAME) || UPPER(MUT_NAME) LIKE ''%' || V_TEXT || '%'' ' ||
            'AND S.MUT_CODE = M.MUT_CODE ' ||
            'AND M.MUT_CODE NOT IN (''MF001'', ''MF036'', ''MF002'', ''MF013'', ''MF039'', ''MF029'', ''MF027'', ''MF035'', ''MF020'') ' ||
            'ORDER BY SCH_NAME, SHORT_NAME ' ||

            'UNION ALL ' ||

            'SELECT OSCH_CODE, OSCH_NAME, LONGNAME, NAME, ISS_NAME ' ||
            'FROM OTHER_PRODUCT O, PRODUCT_MASTER P, ISS_MASTER I ' ||
            'WHERE UPPER(ISS_NAME) || UPPER(LONGNAME) || UPPER(ISS_NAME) LIKE ''%' || V_TEXT || '%'' ' ||
            'AND O.PROD_CLASS_CODE = P.PROD_CODE ' ||
            'AND O.ISS_CODE = I.ISS_CODE ' ||
            'AND (I.BRANCH_CODE <> ''N'' OR I.BRANCH_CODE IS NULL) ' ||
            'AND (O.FLAG <> ''N'' OR O.FLAG IS NULL) ' ||
            'ORDER BY LONGNAME, OSCH_NAME';
    ELSE
        V_QUERY := 
            'SELECT SCH_CODE, SCH_NAME, SHORT_NAME, MUT_NAME ' ||
            'FROM SCHEME_INFO S, MUT_FUND M ' ||
            'WHERE UPPER(MUT_NAME) || UPPER(SCH_NAME) || UPPER(MUT_NAME) LIKE ''%' || V_TEXT || '%'' ' ||
            'AND S.MUT_CODE = M.MUT_CODE ' ||
            'AND M.MUT_CODE NOT IN (''MF001'', ''MF036'', ''MF002'', ''MF013'', ''MF039'', ''MF029'', ''MF027'', ''MF035'', ''MF020'') ' ||
            'AND EXISTS (SELECT SCH_CODE FROM SCH_CITY_MAPPING ' ||
            'WHERE SCH_CODE = S.SCH_CODE ' ||
            'AND (CITY_ID IN (SELECT DISTINCT CITY_ID FROM BRANCH_MASTER ' ||
            'WHERE BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI ' ||
            'WHERE LOGIN_ID = ''' || P_LOGIN || ''' AND ROLE_ID = ''' || P_ROLE || ''')) OR CITY_ID = ''ALL'') ' ||
            'AND ISS_CODE LIKE ''MF%'') ' ||
            'ORDER BY SCH_NAME, SHORT_NAME ' ||

            'UNION ALL ' ||

            'SELECT OSCH_CODE, OSCH_NAME, LONGNAME, NAME, ISS_NAME ' ||
            'FROM OTHER_PRODUCT O, PRODUCT_MASTER P, ISS_MASTER I ' ||
            'WHERE UPPER(ISS_NAME) || UPPER(LONGNAME) || UPPER(ISS_NAME) LIKE ''%' || V_TEXT || '%'' ' ||
            'AND O.PROD_CLASS_CODE = P.PROD_CODE ' ||
            'AND O.ISS_CODE = I.ISS_CODE ' ||
            'AND (I.BRANCH_CODE <> ''N'' OR I.BRANCH_CODE IS NULL) ' ||
            'AND (O.FLAG <> ''N'' OR O.FLAG IS NULL) ' ||
            'AND O.OSCH_CODE IN (SELECT SCH_CODE FROM SCH_CITY_MAPPING ' ||
            'WHERE (CITY_ID IN (SELECT DISTINCT CITY_ID FROM BRANCH_MASTER ' ||
            'WHERE BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI ' ||
            'WHERE LOGIN_ID = ''' || P_LOGIN || ''' AND ROLE_ID = ''' || P_ROLE || ''')) OR CITY_ID = ''ALL'') ' ||
            'AND (ISS_CODE LIKE ''IS%'' OR ISS_CODE LIKE ''MFIS%'')) ' ||
            'ORDER BY LONGNAME, OSCH_NAME';
    END IF;

    -- Open cursor with final query
    OPEN P_RESULT FOR V_QUERY;
END PSM_MODEL_SCH_FIND_1;