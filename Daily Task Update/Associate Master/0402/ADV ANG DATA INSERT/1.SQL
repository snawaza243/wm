   str_test = " insert into client_master 
   CLIENT_NAME,
   ADDRESS1,
   ADDRESS2,
   CITY_ID,
   PHONE,
   EMAIL,
   PAN,
   RM_CODE,
   SOURCEID,
   CATEGORY_ID,
   CREATION_DATE,
   DOB,
   MOBILE,
   MODIFY_USER,
   MODIFY_DATE
   

   --



   
   rs_get_source("AGENT_NAME") & "','" & 
   rs_get_source("ADDRESS1") & "','" & 
   rs_get_source("ADDRESS2") & "','" & 
   rs_get_source("CITY_ID") & "','" & 
   rs_get_source("PHONE")
   rs_get_source("EMAIL") & "','" & 
   rs_get_source("PAN") & "','" & 
   rs_get_source("rm_code")
   rs_get_source("SOURCEID") & 
   '4003',
   sysdate," 
   to_date('" & Format(rs_get_source("DOB"), "dd/MM/yyyy") & "','dd/MM/yyyy'),'" & 
   rs_get_source("MOBILE") & 
   "',
   'Auto',sysdate);" 


   -- UPDATE

   UPDATE CLIENT_TEST
   
   SET CLIENT_NAME = (SELECT AGENT_NAME FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1, 
   SET ADDRESS1 = (SELECT ADDRESS1 FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1,
   SET ADDRESS2 = (SELECT ADDRESS2 FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1, 
   SET CITY_ID = (SELECT CITY_ID FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1, 
   SET PHONE = (SELECT PHONE FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1)
   SET EMAIL = (SELECT EMAIL FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1, 
   SET PAN = (SELECT PAN FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1, 
   SET RM_CODE = (SELECT rm_code FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1),
   SET SOURCEID = (SELECT SOURCEID FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1), 
   SET CATEGORY_ID = '4003',
   SET CREATION_DATE = sysdate,
   SET DOB = to_date((SELECT DOB FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1), "dd/MM/yyyy"),
   SET MOBILE = (SELECT MOBILE FROM AGENT_MASTER WHERE AGENT_CODE = NEW_AGENT_CODE AND ROWNUM = 1),
   SET MODIFY_USER = 'Auto',
   SET MODIFY_DATE = sysdate) 
   WHERE 



   ---

     select 
  agent_code,
  rm_code,
  SOURCEID,
  AGENT_NAME,
  ADDRESS1,
  ADDRESS2,
  CITY_ID,
  PHONE,
  MOBILE,
  EMAIL,
  DOB,
  PAN 
  from agent_master where 
  agent_coDe=(select MAX(agent_code) from agent_master)")
  
   MyConn.Execute ("update ADVISORSUBENTRY set ANACODE='" & rs_get_source(0) & "' ,ANABRANCH='" & rs_get_source(2) & "',ANARM='" & rs_get_source(1) & "' WHERE UNIQUENO='" & my_TBLUID & "'")
   


   SELECT WEALTHMAKER.USER_UNLOG('DCB8A36076015EECA000744EA0A221EF') FROM DUAL

SELECT WEALTHMAKER.USER_LOG('338485663440') FROM DUAL