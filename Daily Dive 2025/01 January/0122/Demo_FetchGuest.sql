SELECT GUEST_NAME,MOBILE,EMP_NO,NVL(TELEPHONE,0)TELEPHONE 
FROM BAJAJ_VENUE_BOOKING WHERE GUEST_CD='90306002' ;


/* Client Information*/
SELECT B.SEX,PITCH_BOOK_NO,RESIADD1,RESIADD2,CITY,STATE,RESIPINCODE,B.DOB,CORESSEMAIL
FROM CLIENTBASICDET       A,CLIENTPERSDET        B,CLIENTWORKDET        C,BAJAJ_VENUE_BOOKING  D
WHERE     D.GUEST_CD = '90306002'
AND TO_CHAR (A.ACCOUNTNO) = D.PITCH_BOOK_NO
AND A.ACCOUNTNO = B.ACCOUNTNO
AND A.ACCOUNTNO = C.ACCOUNTNO(+);


PSM_AO_CLIENT_BY_GUEST