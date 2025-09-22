
            SELECT
                'MF' AS PRODUCT_TYPE,
                S.SCH_CODE,
                S.SCH_NAME,
                S.SHORT_NAME,
                M.MUT_NAME,
                'MF' AS PROD_NAME,
                'DT001' AS PROD_CODE
            FROM SCHEME_INFO S, MUT_FUND M
            WHERE UPPER(M.MUT_NAME) || UPPER(S.SCH_NAME) || UPPER(M.MUT_NAME) LIKE '%NEW%'
              AND S.MUT_CODE = M.MUT_CODE
              AND M.MUT_CODE NOT IN ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020')
              AND EXISTS (
                SELECT SCH_CODE FROM SCH_CITY_MAPPING
                WHERE SCH_CODE = S.SCH_CODE
                  AND (CITY_ID IN (
                        SELECT DISTINCT CITY_ID FROM BRANCH_MASTER
                        WHERE BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212')
                      ) OR CITY_ID = 'ALL')
                  AND ISS_CODE LIKE 'DT001%'
              )
            UNION ALL
            SELECT
                'OTHER' AS PRODUCT_TYPE,
                O.OSCH_CODE AS SCH_CODE,
                O.OSCH_NAME AS SCH_NAME,
                O.LONGNAME AS SHORT_NAME,
                I.ISS_NAME AS MUT_NAME,
                P.NAME AS PROD_NAME,
                P.PROD_CODE AS PROD_CODE
            FROM OTHER_PRODUCT O, PRODUCT_MASTER P, ISS_MASTER I
            WHERE UPPER(I.ISS_NAME) || UPPER(O.LONGNAME) || UPPER(I.ISS_NAME) LIKE '%NEW%'
              AND O.PROD_CLASS_CODE = P.PROD_CODE
              AND O.ISS_CODE = I.ISS_CODE
              AND (I.BRANCH_CODE <> 'N' OR I.BRANCH_CODE IS NULL)
              AND (O.FLAG <> 'N' OR O.FLAG IS NULL)
              AND O.OSCH_CODE IN (
                SELECT SCH_CODE FROM SCH_CITY_MAPPING
                WHERE (CITY_ID IN (
                        SELECT DISTINCT CITY_ID FROM BRANCH_MASTER
                        WHERE BRANCH_CODE IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212')
                      ) OR CITY_ID = 'ALL')
                  AND (ISS_CODE LIKE 'IS%' OR ISS_CODE LIKE 'MFIS%')
              )
            ORDER BY SCH_NAME, SHORT_NAME
        