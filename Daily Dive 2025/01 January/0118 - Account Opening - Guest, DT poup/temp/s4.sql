CREATE OR REPLACE PROCEDURE GetClientInformation(
    p_guest_cd IN VARCHAR2,  -- Input parameter for guest code
    o_cursor OUT SYS_REFCURSOR   -- Output cursor for combined result
)
IS
BEGIN
    -- Open a single cursor that combines the data from both queries
    OPEN o_cursor FOR
    SELECT 
        'Valid Guest Code'      AS MESSAGE,
        D.GUEST_CD              AS GUEST_CD,
        D.GUEST_NAME            AS CLIENT_NAME,
        D.MOBILE                AS MOBILE,
        D.EMP_NO                AS business_code,
        NVL(D.TELEPHONE, 0)     AS TEL2_RES,
        B.SEX                   AS GENDER,
        CASE 
            WHEN UPPER(B.SEX) = UPPER('Male') THEN 'MR.'
            WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.'
            ELSE 'Mrs.'
        END AS TITLE,
        D.PITCH_BOOK_NO         AS PITCH_BOOK_NO,
        A.RESIADD1              AS ADD1,
        A.RESIADD2              AS ADD1,
        A.CITY                  AS CITY_NAME,
        A.STATE                 AS STATE_NAME,
        A.RESIPINCODE           AS RES_PIN,
        A.DOB                   AS DOB,
        A.EMAIL                 AS EMAIL
    FROM 
        BAJAJ_VENUE_BOOKING D
        JOIN CLIENTBASICDET A ON TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO
        JOIN CLIENTPERSDET B ON A.ACCOUNTNO = B.ACCOUNTNO
       -- LEFT JOIN CLIENTWORKDET C ON A.ACCOUNTNO = C.ACCOUNTNO
    WHERE 
        D.GUEST_CD = p_guest_cd;
    
    -- Optional: You can add exception handling for errors here
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions here (e.g., log the error)
        RAISE;
END GetClientInformation;
/
