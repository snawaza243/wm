CREATE OR REPLACE PROCEDURE GetClientInformation(
    p_guest_cd IN VARCHAR2,  -- Input parameter for guest code
    o_cursor OUT SYS_REFCURSOR   -- Output cursor for combined result
)
IS
BEGIN
    -- Open a single cursor that combines the data from both queries
    OPEN o_cursor FOR
    SELECT 
        GUEST_NAME,
        MOBILE,
        EMP_NO,
        NVL(TELEPHONE, 0) AS TELEPHONE,
        B.SEX,
        D.PITCH_BOOK_NO,
        B.RESIADD1,
        B.RESIADD2,
        B.CITY,
        B.STATE,
        B.RESIPINCODE,
        B.DOB,
        B.CORESSEMAIL
    FROM 
        BAJAJ_VENUE_BOOKING D
        JOIN CLIENTBASICDET A ON TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO
        JOIN CLIENTPERSDET B ON A.ACCOUNTNO = B.ACCOUNTNO
        LEFT JOIN CLIENTWORKDET C ON A.ACCOUNTNO = C.ACCOUNTNO
    WHERE 
        D.GUEST_CD = p_guest_cd;
    
    -- Optional: You can add exception handling for errors here
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions here (e.g., log the error)
        RAISE;
END GetClientInformation;
/
