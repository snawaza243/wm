CREATE OR REPLACE PROCEDURE PSM_AO_CLIENT_BY_GUEST(
    p_guest_cd IN VARCHAR2,  
    o_cursor1 OUT SYS_REFCURSOR,  
    o_cursor2 OUT SYS_REFCURSOR   
)
IS
BEGIN
    OPEN o_cursor1 FOR
        SELECT 
            'VALID DATA IN C1'  AS C1_MESSAGE,
            D.GUEST_NAME          AS C1_GUEST_NAME,
            D.MOBILE              AS C1_MOBILE_NO,
            D.EMP_NO              AS C1_EMP_NO,
            D.NVL(TELEPHONE,0)    AS C1_TELEPHONE 
        FROM BAJAJ_VENUE_BOOKING D 
        WHERE GUEST_CD = p_guest_cd;

    OPEN o_cursor2 FOR
        SELECT 
            'VALID DATA IN C1'  AS C1_MESSAGE,
            D.GUEST_NAME          AS C1_GUEST_NAME,
            D.MOBILE              AS C1_MOBILE_NO,
            D.EMP_NO              AS C1_EMP_NO,
            D.NVL(TELEPHONE,0)    AS C1_TELEPHONE 
            'VALID DATA IN C2'  AS C2_MESSAGE,
            B.SEX               AS C2_GUEST_SEX,
            PITCH_BOOK_NO       AS C2_PITCH_NO,
            RESIADD1            AS C2_ADD1,
            RESIADD2            AS C2_ADD2,
            CITY                AS C2_CITY,
            STATE               AS C2_STATE,
            RESIPINCODE         AS C2_PIN,
            B.DOB               AS C2_DOB,
            CORESSEMAIL         AS C2_EMAIL,
        FROM CLIENTBASICDET       A,CLIENTPERSDET        B,CLIENTWORKDET        C,BAJAJ_VENUE_BOOKING  D
        WHERE    TO_CHAR (A.ACCOUNTNO) = D.PITCH_BOOK_NO
        AND A.ACCOUNTNO = B.ACCOUNTNO
        AND A.ACCOUNTNO = C.ACCOUNTNO(+)
        AND D.GUEST_CD = p_guest_cd;
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE;
END PSM_AO_CLIENT_BY_GUEST;
/
