SELECT  'VALID DATA IN C2', B.SEX, B.MATRIALST,  
        CASE  WHEN UPPER(B.SEX) = UPPER('Male') THEN 'MR.' WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.' ELSE 'Mrs.'END, 
        PITCH_BOOK_NO, RESIADD1, RESIADD2, CITY, CM.city_id, STATE, CM.state_id, SM.country_id, RESIPINCODE, B.DOB, CORESSEMAIL
        --INTO C2_MESSAGE, C2_GUEST_SEX, C2_GUEST_MATRIALST, TITLE, C2_PITCH_NO, C2_ADD1, C2_ADD2, C2_CITY, C2_CITY_ID, C2_STATE, C2_STATE_CODE, C2_COUNTRY_ID, C2_PIN, C2_DOB, C2_EMAIL
        FROM CLIENTBASICDET A, CLIENTPERSDET B, CLIENTWORKDET C, BAJAJ_VENUE_BOOKING D, CITY_MASTER CM, STATE_MASTER SM
        WHERE D.GUEST_CD = '91206106' AND TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO AND A.ACCOUNTNO = B.ACCOUNTNO AND A.ACCOUNTNO = C.ACCOUNTNO(+) AND ROWNUM = 1;
       
SELECT  'VALID DATA IN C1', D.GUEST_CD, D.GUEST_NAME, D.MOBILE, D.EMP_NO, D.TELEPHONE
    --INTO C1_MESSAGE, C1_GUEST_CODE, C1_GUEST_NAME, C1_MOBILE_NO, C1_EMP_NO, C1_TELEPHONE
FROM  BAJAJ_VENUE_BOOKING D WHERE  D.GUEST_CD = '90306002' AND ROWNUM = 1;



select GUEST_CD from BAJAJ_VENUE_BOOKING D, CLIENTBASICDET A
where TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO;



SELECT B.SEX,PITCH_BOOK_NO,RESIADD1,RESIADD2,CITY,STATE,RESIPINCODE,B.DOB,CORESSEMAIL
FROM CLIENTBASICDET       A,CLIENTPERSDET        B,CLIENTWORKDET        C,BAJAJ_VENUE_BOOKING  D
WHERE     D.GUEST_CD = '91882268'
AND TO_CHAR (A.ACCOUNTNO) = D.PITCH_BOOK_NO
AND A.ACCOUNTNO = B.ACCOUNTNO
AND A.ACCOUNTNO = C.ACCOUNTNO(+);



