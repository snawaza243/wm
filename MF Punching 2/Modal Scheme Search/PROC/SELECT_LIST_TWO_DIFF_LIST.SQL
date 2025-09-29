select 'MF' AS NAME, Short_name, sch_name, mut_name, 'DT001' AS V_CODE_MF, sch_code from scheme_info s,mut_fund m 
where UPPER(MUT_NAME)||upper(sch_name)||UPPER(MUT_NAME) LIKE '%NEW%' and s.mut_code=m.mut_code 
and m.mut_code not in ('MF001','MF036','MF002','MF013','MF039','MF029','MF027','MF035','MF020') 
and exists (SELECT sch_code FROM SCH_CITY_MAPPING WHERE sch_code=s.sch_code and (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212'))) OR CITY_ID='ALL') and iss_code like 'MF%' ) ORDER BY sch_name,Short_name
;



select NAME, osch_name AS Short_name, longname AS sch_name, iss_name AS MUT_NAME, '' AS V_CODE_MF, osch_code AS sch_code from other_product o, product_master p,iss_master i where UPPER(ISS_NAME)||upper(longname)||UPPER(ISS_NAME) LIKE '%NEW%' and o.prod_class_code=p.prod_code and o.iss_code=i.iss_code and (i.branch_code<>'N' or i.branch_code is null) and (o.flag<>'N' or o.flag is null) and o.osch_code in (SELECT sch_code FROM SCH_CITY_MAPPING WHERE (city_id IN ((SELECT DISTINCT city_id FROM BRANCH_MASTER WHERE branch_code IN (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212'))) OR CITY_ID='ALL') and (iss_code like 'IS%' or iss_code like 'MFIS%')) ORDER BY longname,osch_name






