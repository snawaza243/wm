        SELECT 
            'VALID DATA IN C2',
            B.SEX,
            B.MATRIALST,
            CASE 
                WHEN UPPER(B.SEX) = UPPER('Male') THEN 'Mr.'
                WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.'
                ELSE 'Mrs.'
            END,
            PITCH_BOOK_NO,
            RESIADD1,
            RESIADD2,
            CITY,
            NVL((SELECT CM.city_id FROM CITY_MASTER CM WHERE CITY_NAME = CITY),NULL),--CM.city_id,
            STATE,
            NVL((SELECT CM.state_id FROM CITY_MASTER CM WHERE CITY_NAME = CITY),NULL),--CM.state_id,
            NVL((SELECT SM.country_id FROM STATE_MASTER SM WHERE SM.STATE_ID = (SELECT CM.state_id FROM CITY_MASTER CM WHERE CITY_NAME = CITY)),NULL),--SM.country_id,
            RESIPINCODE,
            B.DOB,
            CORESSEMAIL
        --INTO C2_MESSAGE, C2_GUEST_SEX, C2_GUEST_MATRIALST, TITLE, C2_PITCH_NO, C2_ADD1, C2_ADD2, C2_CITY, C2_CITY_ID, C2_STATE, C2_STATE_CODE, C2_COUNTRY_ID, C2_PIN, C2_DOB, C2_EMAIL
        FROM 
            CLIENTBASICDET A,
            CLIENTPERSDET B,
            CLIENTWORKDET C,
            BAJAJ_VENUE_BOOKING D
        WHERE 
            D.GUEST_CD  = '94489129' 
            AND TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO
            AND A.ACCOUNTNO = B.ACCOUNTNO
            AND A.ACCOUNTNO = C.ACCOUNTNO(+)
            AND ROWNUM = 1;
            
            
 SELECT 
            'VALID DATA IN C1',
            D.GUEST_CD,
            D.GUEST_NAME,
            D.MOBILE,
            D.EMP_NO,
            D.TELEPHONE
        --INTO C1_MESSAGE, C1_GUEST_CODE, C1_GUEST_NAME, C1_MOBILE_NO, C1_EMP_NO, C1_TELEPHONE
        FROM 
            BAJAJ_VENUE_BOOKING D
        WHERE 
            D.GUEST_CD = '94489129'
            AND ROWNUM = 1;