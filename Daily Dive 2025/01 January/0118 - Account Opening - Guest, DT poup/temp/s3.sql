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
        NVL(D.GUEST_CD,0)              AS GUEST_CD,
        NVL(D.GUEST_NAME,0)            AS CLIENT_NAME,
        NVL(D.MOBILE,0)                AS MOBILE,
        NVL(D.EMP_NO,0)                AS business_code,
        NVL(D.TELEPHONE, 0)     AS TEL2_RES,
        B.SEX                   AS GENDER,
        CASE 
            WHEN UPPER(B.SEX) = UPPER('Male') THEN 'MR.'
            WHEN UPPER(B.SEX) = UPPER('Female') AND UPPER(B.MATRIALST) = UPPER('Single') THEN 'Ms.'
            ELSE 'Mrs.'
        END AS TITLE,
        NVL(D.PITCH_BOOK_NO,0)         AS PITCH_BOOK_NO,
        NVL(A.RESIADD1,0)              AS ADD1,
        NVL(A.RESIADD2,0)              AS ADD1,
        NVL(A.CITY,0)                  AS CITY_NAME,
        NVL(A.STATE,0)                 AS STATE_NAME,
        NVL(A.RESIPINCODE,0)           AS RES_PIN,
        ''                             AS DOB,

        --NVL(A.DOB,0)                   AS DOB,
        NVL(A.EMAIL,0)                 AS EMAIL
    FROM 
        BAJAJ_VENUE_BOOKING D
        JOIN CLIENTBASICDET A ON TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO
        JOIN CLIENTPERSDET B ON A.ACCOUNTNO = B.ACCOUNTNO
       -- LEFT JOIN CLIENTWORKDET C ON A.ACCOUNTNO = C.ACCOUNTNO
    WHERE 
        D.GUEST_CD  = '3006913';
    
    -- Optional: You can add exception handling for errors here
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions here (e.g., log the error)
        RAISE;
END GetClientInformation;
/
