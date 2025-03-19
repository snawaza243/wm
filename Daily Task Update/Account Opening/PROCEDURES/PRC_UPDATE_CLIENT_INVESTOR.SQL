create or replace PROCEDURE             PRC_UPDATE_CLIENT_INVESTOR (
   MAINCODE1   VARCHAR2,
   LOGINIDX    VARCHAR2,
   CATEGORYID  VARCHAR2,
   INVESTORCODE VARCHAR2,
   GUESTCD     VARCHAR2
)
IS
   RMCODE     VARCHAR2 (10);
   CITYNAME   VARCHAR2 (100);
BEGIN
   FOR I IN (SELECT *
               FROM CLIENT_TEST
              WHERE MAIN_CODE = MAINCODE1)
   LOOP
      RMCODE:='';
      IF I.CLIENT_CODE = I.MAIN_CODE AND SUBSTR (I.CLIENT_CODEKYC, 9, 3) ='001' THEN
         --UPDATE CLIENT_MASTER
         DBMS_OUTPUT.PUT_LINE ('M1');
         SELECT CITY_NAME
          INTO CITYNAME
          FROM CITY_MASTER
          WHERE CITY_ID = I.CITY_ID;
          
          SELECT RM_CODE
          INTO RMCODE
          FROM EMPLOYEE_MASTER
          WHERE PAYROLL_ID = I.BUSINESS_CODE;

         UPDATE CLIENT_MASTER
            SET CATEGORY_ID = CATEGORYID,
                INVESTOR_CODE = INVESTORCODE,
                GUEST_CD = GUESTCD,
                CLIENT_FNAME = I.TITLE,
                CLIENT_LNAME = '',
                CLIENT_NAME = I.CLIENT_NAME,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = SUBSTR (I.GENDER, 1, 1),
                CITY_NAME = CITYNAME,
                STD1 = I.STD1,
                STD2 = I.STD2,
                PHONE1 = I.TEL1,
                PHONE2 = I.TEL2,
                STATUS = I.STATUS,
                OCC_ID = I.OCC_ID,
                CITY_ID = I.CITY_ID,
                COMM_ID = I.COMM_ID,
                COMM_INT_ID = I.COMM_INTER_ID,
                KYC = 'YES',
                SOURCEID = I.BRANCH_CODE,
                RM_CODE = RMCODE,
                LOGGEDUSERID = LOGINIDX,
                ACT_CAT = I.ACT_CAT
          WHERE CLIENT_CODE = SUBSTR (I.CLIENT_CODEKYC, 1, 8);

         --MAIN EXISTING KE LIYE INVESTOR UPDATE KIYA HAI
         UPDATE INVESTOR_MASTER
            SET INVESTOR_NAME = I.CLIENT_NAME,
                OCCUPATION_ID = I.OCC_ID,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = LPAD (I.GENDER, 1),
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES'
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          
          
          -----------------------for mf------------------------------------------------------
          UPDATE CLIENT_MASTER@MF.BAJAJCAPITAL
            SET CATEGORY_ID = CATEGORYID,
                INVESTOR_CODE = INVESTORCODE,
                GUEST_CD = GUESTCD,
                CLIENT_FNAME = I.TITLE,
                CLIENT_LNAME = '',
                CLIENT_NAME = I.CLIENT_NAME,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = SUBSTR (I.GENDER, 1, 1),
                CITY_NAME = CITYNAME,
                STD1 = I.STD1,
                STD2 = I.STD2,
                PHONE1 = I.TEL1,
                PHONE2 = I.TEL2,
                STATUS = I.STATUS,
                OCC_ID = I.OCC_ID,
                CITY_ID = I.CITY_ID,
                COMM_ID = I.COMM_ID,
                COMM_INT_ID = I.COMM_INTER_ID,
                KYC = 'YES',
                SOURCEID = I.BRANCH_CODE,
                RM_CODE = RMCODE,
                LOGGEDUSERID = LOGINIDX,
                ACT_CAT = I.ACT_CAT
          WHERE CLIENT_CODE = SUBSTR (I.CLIENT_CODEKYC, 1, 8);

         --MAIN EXISTING KE LIYE INVESTOR UPDATE KIYA HAI
         UPDATE INVESTOR_MASTER@MF.BAJAJCAPITAL
            SET INVESTOR_NAME = I.CLIENT_NAME,
                OCCUPATION_ID = I.OCC_ID,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = LPAD (I.GENDER, 1),
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES'
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          ----------------------------------------------------------------------------------------
      ELSIF     I.CLIENT_CODE = I.MAIN_CODE AND SUBSTR (I.CLIENT_CODEKYC, 9, 3) <> '001' THEN
         --UPDATE INVESTOR_MASTER
           DBMS_OUTPUT.PUT_LINE ('M2');
        --MAIN EXISTING KE LIYE INVESTOR UPDATE KIYA HAI
         UPDATE INVESTOR_MASTER
            SET INVESTOR_NAME = I.CLIENT_NAME,
                OCCUPATION_ID = I.OCC_ID,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = LPAD (I.GENDER, 1),
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES'
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          
          UPDATE INVESTOR_MASTER@MF.BAJAJCAPITAL
            SET INVESTOR_NAME = I.CLIENT_NAME,
                OCCUPATION_ID = I.OCC_ID,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = LPAD (I.GENDER, 1),
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES'
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          
      END IF;

      IF I.CLIENT_CODE <> MAINCODE1 AND SUBSTR (I.CLIENT_CODEKYC, 9, 3) ='001' THEN
         --UPDATE CLIENT_MASTER
         DBMS_OUTPUT.PUT_LINE ('M3');

         SELECT CITY_NAME
           INTO CITYNAME
           FROM CITY_MASTER
          WHERE CITY_ID = I.CITY_ID;

         SELECT RM_CODE
           INTO RMCODE
           FROM EMPLOYEE_MASTER
          WHERE PAYROLL_ID = I.BUSINESS_CODE;

         --MAIN EXISTING KE LIYE INVESTOR UPDATE KIYA HAI
         UPDATE INVESTOR_MASTER
            SET INVESTOR_NAME = I.CLIENT_NAME,
                OCCUPATION_ID = I.OCC_ID,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = LPAD (I.GENDER, 1),
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES'
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          
          UPDATE INVESTOR_MASTER@MF.BAJAJCAPITAL
            SET INVESTOR_NAME = I.CLIENT_NAME,
                OCCUPATION_ID = I.OCC_ID,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = LPAD (I.GENDER, 1),
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES'
          WHERE INV_CODE = I.CLIENT_CODEKYC;
      ELSIF     I.CLIENT_CODE <> MAINCODE1 AND SUBSTR (I.CLIENT_CODEKYC, 9, 3) <> '001' THEN
         DBMS_OUTPUT.PUT_LINE ('M4');

         SELECT RM_CODE
           INTO RMCODE
           FROM EMPLOYEE_MASTER
          WHERE PAYROLL_ID = I.BUSINESS_CODE;

         UPDATE INVESTOR_MASTER
            SET INVESTOR_NAME = I.CLIENT_NAME,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = SUBSTR (I.GENDER, 1, 1),
                PHONE = I.TEL1,
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES',
                SOURCE_ID = SUBSTR(I.CLIENT_CODEKYC,1,8),
                RM_CODE = RMCODE,
                REL_ID = I.RELATION_ID
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          
          UPDATE INVESTOR_MASTER@MF.BAJAJCAPITAL
            SET INVESTOR_NAME = I.CLIENT_NAME,
                ADDRESS1 = I.ADD1,
                ADDRESS2 = I.ADD2,
                FAX = I.FAX,
                EMAIL = I.EMAIL,
                PAN = I.CLIENT_PAN,
                DOB = I.DOB,
                MOBILE = I.MOBILE_NO,
                PINCODE = I.PINCODE,
                GENDER = SUBSTR (I.GENDER, 1, 1),
                PHONE = I.TEL1,
                STATUS = I.STATUS,
                CITY_ID = I.CITY_ID,
                KYC = 'YES',
                SOURCE_ID = SUBSTR(I.CLIENT_CODEKYC,1,8),
                RM_CODE = RMCODE,
                REL_ID = I.RELATION_ID
          WHERE INV_CODE = I.CLIENT_CODEKYC;
          
      END IF;
   END LOOP;
   COMMIT;
END; 
 