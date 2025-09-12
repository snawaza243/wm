-- CAT-INV, SUB-CLIENT

-- WITHOUT FILTER
Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) as client_type,e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257;

-- WITH SUB/CLIENT CODE
Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) as client_type,e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257  and cm.client_code=a.source_id  and lpad(a.inv_code,1)=4  and source_id in (select client_code from client_master where ( TO_CHAR(client_code)= '42519319'))  and B.BRANCH_CODE IN  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212');
 

-- WITH SUB/CLIENT NAME

Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) as client_type,e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257  and cm.client_code=a.source_id  and lpad(a.inv_code,1)=4  and source_id in (select client_code from client_master where ( TO_CHAR(client_code)= '42519319'))  and source_id in (select client_code from client_master where upper(trim(client_name)) like '%RAJES%')  and B.BRANCH_CODE IN  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212');

Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) as client_type,e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257  and cm.client_code=a.source_id  and lpad(a.inv_code,1)=4  and source_id in (select client_code from client_master where upper(trim(client_name)) like '%ARJUN%')  and B.BRANCH_CODE IN  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212') ;

 
SELECT * FROM CLIENT_TEST WHERE CLIENT_CODEKYC = '42519858001';

-- INV-SUB, WITH NAME
Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) as client_type,e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257  and cm.client_code=a.source_id  and lpad(a.inv_code,1)=4  and source_id in (select client_code from client_master where upper(trim(client_name)) like '%SADAF%')  and B.BRANCH_CODE IN  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212'); 

-- INV-SUB, WITH SUB CODE

Select INVESTOR_name,INV_code,cm.address1,cm.address2,c.city_name,b.Branch_name,DECODE(a.client_type,'RELIGARE','RELIGARE',NULL) as client_type,e.rm_name,cm.GUEST_CD,a.pan 
FROM Branch_master b,EMPLOYEE_MASTER E,investor_master a,client_master cm,City_master c 
where E.source=b.branch_code AND a.RM_CODE=E.RM_CODE AND cm.city_id=C.city_id(+) and INVESTOR_name IS NOT NULL AND b.branch_code<>10010226 AND b.branch_code<>10070257  and cm.client_code=a.source_id  and lpad(a.inv_code,1)=4  and source_id in (select client_code from client_master where ( TO_CHAR(client_code)= '42159158'))  and source_id in (select client_code from client_master where upper(trim(client_name)) like '%SADAF%')  and B.BRANCH_CODE IN  (SELECT BRANCH_ID FROM USERDETAILS_JI WHERE USERDETAILS_JI.LOGIN_ID = '121397' AND USERDETAILS_JI.ROLE_ID = '212') ;




-- DATA BY ROW CLICK



SELECT loggeduserid, main_code, NVL(UPD_PROC, 'N') --INTO V_LOG_USER_ID, V_MAIN_CODE, V_UPD_PROC
FROM client_test WHERE client_codekyc = '42159158002';

SELECT INV_CODE, SUBSTR(INV_CODE, 1, 8), INVESTOR_NAME, PAN --INTO V_MFA_INV_CODE, V_MFA_CLIENT_CODE, M_MFA_INVESTOR_NAME, V_MFA_PAN
FROM INVESTOR_MASTER WHERE INV_CODE = '42159158002';

SELECT client_code --INTO V_MFA_AH_CODE 
FROM client_test WHERE client_codekyc = '42159158002';

SELECT payroll_id --INTO V_MFA_BUSI_CODE 
FROM employee_master WHERE rm_code = (SELECT rm_code FROM agent_master  WHERE agent_code = SUBSTR('42159158002', 1, 8));