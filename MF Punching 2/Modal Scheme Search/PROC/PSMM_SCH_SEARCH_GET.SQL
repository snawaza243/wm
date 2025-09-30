CREATE OR REPLACE PROCEDURE PSMM_SCH_SEARCH_GET (
  PX_FIND_STATE   IN VARCHAR2,
  PX_SCH_STR      IN VARCHAR2,
  PX_LOG          IN VARCHAR2,
  PX_ROLE         IN VARCHAR2,
  PX_CURSOR       OUT SYS_REFCURSOR
) AS

V_QUERY         VARCHAR2(4000);
V_QUERY2        VARCHAR2(4000);
V_STR_MF        VARCHAR2(100);
V_CODE_MF       VARCHAR2(20);
V_STR_SEARCH    VARCHAR2(100);

BEGIN

  V_STR_SEARCH := UPPER(TRIM(PX_SCH_STR));
  SELECT NAME,PROD_CODE INTO V_STR_MF,V_CODE_MF FROM PRODUCT_MASTER 
   WHERE NATURE_CODE='NT001'
     AND ROWNUM = 1;

  IF V_STR_SEARCH IS NOT NULL THEN
    IF PX_ROLE = '1' THEN
      V_QUERY := 'select '''||V_STR_MF||''' AS NAME, Short_name, sch_name, mut_name, '''||V_CODE_MF||''' AS V_CODE_MF, sch_code from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE ''%' || V_STR_SEARCH || '%'' and s.mut_code=m.mut_code and m.mut_code not in (''MF001'',''MF036'',''MF002'',''MF013'',''MF039'',''MF029'',''MF027'',''MF035'',''MF020'') ORDER BY sch_name,Short_name';
    ELSE
     V_QUERY := 'select '''||V_STR_MF||''' AS NAME, Short_name, sch_name, mut_name, '''||V_CODE_MF||''' AS V_CODE_MF, sch_code from scheme_info s,mut_fund m where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE ''%' || V_STR_SEARCH || '%'' and s.mut_code=m.mut_code and m.mut_code not in (''MF001'',''MF036'',''MF002'',''MF013'',''MF039'',''MF029'',''MF027'',''MF035'',''MF020'') and exists (SELECT sch_code FROM SCH_CITY_MAPPING WHERE sch_code=s.sch_code and (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE||'''))) OR CITY_ID=''ALL'') and iss_code like ''MF%'' ) ORDER BY sch_name,Short_name';
    END IF;

    IF PX_ROLE = '1' THEN
      V_QUERY2 := 'select NAME, osch_name AS Short_name, longname AS sch_name, iss_name AS MUT_NAME, '' AS V_CODE_MF, osch_code AS sch_code from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE ''%' || V_STR_SEARCH || '%'' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>''N'' or i.branch_code is null) and (o.flag<>''N'' or o.flag is null) ORDER BY longname,osch_name';
    ELSE
      V_QUERY2 := 'select NAME, osch_name AS Short_name, longname AS sch_name, iss_name AS MUT_NAME, '''' AS V_CODE_MF, osch_code AS sch_code from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE ''%' || V_STR_SEARCH || '%'' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>''N'' or i.branch_code is null) and (o.flag<>''N'' or o.flag is null) and o.osch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = ''' || PX_LOG || ''' AND USERDETAILS_JI.ROLE_ID = '''||PX_ROLE||'''))) OR CITY_ID=''ALL'') and (iss_code like ''IS%'' or iss_code like ''MFIS%'')) ORDER BY longname,osch_name';
    END IF;
  END IF;

/*
  IF PX_FIND_STATE = '1' THEN
    OPEN PX_CURSOR FOR V_QUERY;
    RETURN;
  ELSIF PX_FIND_STATE = '2' THEN
    OPEN PX_CURSOR FOR V_QUERY2;
    RETURN;
  ELSE
    -- RETURN THE DATA OF BOTH QUERY DATA IN A SINGEL CURSOR PX_CURSOR
    OPEN PX_CURSOR FOR '
      SELECT * FROM (' || V_QUERY || ')
      UNION ALL
      SELECT * FROM (' || V_QUERY2 || ')
    ';
    RETURN;
  END IF;
 */
 
  OPEN PX_CURSOR FOR '
      SELECT * FROM (' || V_QUERY || ')
      UNION ALL
      SELECT * FROM (' || V_QUERY2 || ')
    ';
    RETURN;
 
END PSMM_SCH_SEARCH_GET;