CREATE OR REPLACE PROCEDURE GetClientInformation(
    p_guest_cd IN VARCHAR2,  -- Input parameter for guest code
    o_cursor1 OUT SYS_REFCURSOR,  -- Output cursor for first query result
    o_cursor2 OUT SYS_REFCURSOR   -- Output cursor for second query result
)
IS
BEGIN
    -- First cursor for BAJAJ_VENUE_BOOKING table with guest code
    OPEN o_cursor1 FOR
    SELECT 
        GUEST_NAME,
        MOBILE,
        EMP_NO,
        NVL(TELEPHONE, 0) AS TELEPHONE
    FROM 
        BAJAJ_VENUE_BOOKING
    WHERE 
        GUEST_CD = p_guest_cd;

    -- Second cursor for CLIENTBASICDET, CLIENTPERSDET, CLIENTWORKDET, and BAJAJ_VENUE_BOOKING tables
    OPEN o_cursor2 FOR
    SELECT 
        NVL(B.SEX,0),
        NVL(D.PITCH_BOOK_NO,0),
        NVL(B.RESIADD1,0),
        NVL(B.RESIADD2,0),
        NVL(B.CITY,0),
        NVL(B.STATE,0),
        NVL(B.RESIPINCODE,0),
        NVL(B.DOB,0),
        NVL(B.CORESSEMAIL,0)
    FROM 
        CLIENTBASICDET A
        JOIN CLIENTPERSDET B ON A.ACCOUNTNO = B.ACCOUNTNO
        LEFT JOIN CLIENTWORKDET C ON A.ACCOUNTNO = C.ACCOUNTNO
        JOIN BAJAJ_VENUE_BOOKING D ON TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO
    WHERE 
        D.GUEST_CD = p_guest_cd;
    
    -- Optional: You can add exception handling for errors here
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions here (e.g., log the error)
        RAISE;
END GetClientInformation;
/
