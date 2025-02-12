SELECT 
            D.GUEST_CD          AS C1_GUEST_CODE,
            D.GUEST_NAME          AS C1_GUEST_NAME,
            D.MOBILE              AS C1_MOBILE_NO,
            D.EMP_NO              AS C1_EMP_NO,
            D.TELEPHONE         AS C1_TELEPHONE,
            B.SEX               AS C2_GUEST_SEX,
            B.MATRIALST         AS C2_GUEST_MATRIALST,
            CASE 
            WHEN UPPER(B.SEX) = UPPER('Male') THEN 'MR.'
            WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.'
            ELSE 'Mrs.'
            END AS TITLE,
            PITCH_BOOK_NO       AS C2_PITCH_NO,
            RESIADD1            AS C2_ADD1,
            RESIADD2            AS C2_ADD2,
            CITY                AS C2_CITY,
            cm.city_id          AS C2_CITY_ID,
            STATE               AS C2_STATE,
            sm.state_id         AS C2_STATE_CODE,
            sm.country_id       AS C2_COUNTRY_ID,
            RESIPINCODE         AS C2_PIN,
            B.DOB               AS C2_DOB,
            CORESSEMAIL         AS C2_EMAIL
        FROM CLIENTBASICDET       A,CLIENTPERSDET        B,CLIENTWORKDET        C,BAJAJ_VENUE_BOOKING  D, CITY_MASTER CM, state_master SM

        WHERE    TO_CHAR (A.ACCOUNTNO) = D.PITCH_BOOK_NO
        AND A.ACCOUNTNO = B.ACCOUNTNO
        AND A.ACCOUNTNO = C.ACCOUNTNO(+)
        AND UPPER(A.CITY) = UPPER(CM.CITY_NAME)
        AND UPPER(A.STATE) = UPPER(sm.state_name)

        AND D.GUEST_CD   IS NOT NULL
        
        AND  D.GUEST_CD    IS NOT NULL
        AND  D.GUEST_NAME  IS NOT NULL
        AND  D.MOBILE      IS NOT NULL
        AND  D.EMP_NO      IS NOT NULL
        AND  D.TELEPHONE   IS NOT NULL
        AND  B.SEX         IS NOT NULL
        AND  PITCH_BOOK_NO IS NOT NULL
        AND  RESIADD1      IS NOT NULL
        AND  RESIADD2      IS NOT NULL
        AND  CITY          IS NOT NULL
        AND  STATE         IS NOT NULL
        AND  RESIPINCODE   IS NOT NULL
        AND  B.DOB         IS NOT NULL
        AND  CORESSEMAIL   IS NOT NULL;