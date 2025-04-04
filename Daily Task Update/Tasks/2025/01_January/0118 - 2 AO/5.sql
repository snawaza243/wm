CREATE OR REPLACE PROCEDURE PSM_AO_CLIENT_BY_GUEST(
    p_guest_cd IN VARCHAR2,  
    p_login_id IN VARCHAR2,
    o_cursor OUT SYS_REFCURSOR   
)
IS
BEGIN
    OPEN o_cursor FOR
        SELECT 
            D.GUEST_CD          AS C1_GUEST_CODE,
            D.GUEST_NAME          AS C1_GUEST_NAME,
            D.MOBILE              AS C1_MOBILE_NO,
            D.EMP_NO              AS C1_EMP_NO,
            D.TELEPHONE    AS C1_TELEPHONE,
            B.SEX               AS C2_GUEST_SEX,
            PITCH_BOOK_NO       AS C2_PITCH_NO,
            RESIADD1            AS C2_ADD1,
            RESIADD2            AS C2_ADD2,
            CITY                AS C2_CITY,
            STATE               AS C2_STATE,
            RESIPINCODE         AS C2_PIN,
            B.DOB               AS C2_DOB,
            CORESSEMAIL         AS C2_EMAIL
        FROM CLIENTBASICDET       A,CLIENTPERSDET        B,CLIENTWORKDET        C,BAJAJ_VENUE_BOOKING  D
        WHERE    TO_CHAR (A.ACCOUNTNO) = D.PITCH_BOOK_NO
        AND A.ACCOUNTNO = B.ACCOUNTNO
        AND A.ACCOUNTNO = C.ACCOUNTNO(+)
        AND D.GUEST_CD   = p_guest_cd
        
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
        AND  CORESSEMAIL   IS NOT NULL
        ;
    
EXCEPTION
    WHEN OTHERS THEN
        RAISE;
END PSM_AO_CLIENT_BY_GUEST;
/
