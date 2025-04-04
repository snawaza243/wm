create or replace PROCEDURE PSM_AO_CLIENT_BY_GUEST(
    p_guest_cd IN VARCHAR2,  
    p_login_id IN VARCHAR2,
    o_cursor OUT SYS_REFCURSOR   
)
IS
    -- Declare the variables to hold the result set values
    C1_MESSAGE VARCHAR2(100);
    C1_GUEST_CODE VARCHAR2(20);
    C1_GUEST_NAME VARCHAR2(100);
    C1_MOBILE_NO VARCHAR2(20);
    C1_EMP_NO VARCHAR2(20);
    C1_TELEPHONE VARCHAR2(20);

    C2_MESSAGE VARCHAR2(100);
    C2_GUEST_SEX VARCHAR2(10);
    C2_GUEST_MATRIALST VARCHAR2(20);
    TITLE VARCHAR2(10);
    C2_PITCH_NO VARCHAR2(20);
    C2_ADD1 VARCHAR2(200);
    C2_ADD2 VARCHAR2(200);
    C2_CITY VARCHAR2(50);
    C2_CITY_ID VARCHAR2(20);
    C2_STATE VARCHAR2(50);
    C2_STATE_CODE VARCHAR2(10);
    C2_COUNTRY_ID VARCHAR2(20);
    C2_PIN VARCHAR2(20);
    C2_DOB DATE;
    C2_EMAIL VARCHAR2(100);
    GUEST_CD_EXIST_FLAG NUMBER :=0;
    DUP_GUEST_FLAG number:= 0;

begin


BEGIN
    BEGIN
        SELECT COUNT(*) 
        INTO GUEST_CD_EXIST_FLAG
        FROM BAJAJ_VENUE_BOOKING 
        WHERE GUEST_CD = trim(p_guest_cd);

    EXCEPTION 
        WHEN NO_DATA_FOUND THEN
            GUEST_CD_EXIST_FLAG := 0;
        WHEN OTHERS THEN
            GUEST_CD_EXIST_FLAG := 0; -- Handle unexpected errors safely
    END;

    -- If duplicate records exist, return a message
    IF GUEST_CD_EXIST_FLAG = 0 THEN
        OPEN o_cursor FOR SELECT 'This is not a valid Guest Code' AS C1_MESSAGE FROM DUAL;
        RETURN;
    END IF;

    BEGIN
        SELECT COUNT(*) 
        INTO DUP_GUEST_FLAG
        FROM CLIENT_MASTER 
        WHERE GUEST_CD = trim(p_guest_cd);
    EXCEPTION 
        WHEN NO_DATA_FOUND THEN
            DUP_GUEST_FLAG := 0;
        WHEN OTHERS THEN
            DUP_GUEST_FLAG := 0; 
    END;

    -- If duplicate records exist, return a message
    IF DUP_GUEST_FLAG > 0 THEN
        OPEN o_cursor FOR SELECT 'Guest Code is Duplicate and clinent name is ' || (select client_name from client_master where guest_cd = trim(p_guest_cd) and rownum = 1 ) AS C1_MESSAGE FROM DUAL;
        RETURN;
    END IF;

END ;




    BEGIN
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
        INTO C2_MESSAGE, C2_GUEST_SEX, C2_GUEST_MATRIALST, TITLE, C2_PITCH_NO, C2_ADD1, C2_ADD2, C2_CITY, C2_CITY_ID, C2_STATE, C2_STATE_CODE, C2_COUNTRY_ID, C2_PIN, C2_DOB, C2_EMAIL
        FROM 
            CLIENTBASICDET A,
            CLIENTPERSDET B,
            CLIENTWORKDET C,
            BAJAJ_VENUE_BOOKING D
        WHERE 
            D.GUEST_CD  = p_guest_cd 
            AND TO_CHAR(A.ACCOUNTNO) = D.PITCH_BOOK_NO
            AND A.ACCOUNTNO = B.ACCOUNTNO
            AND A.ACCOUNTNO = C.ACCOUNTNO(+)
            AND ROWNUM = 1;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            C2_MESSAGE := 'No data found for C2';
            C2_GUEST_SEX := NULL;
            C2_GUEST_MATRIALST := NULL;
            TITLE := NULL;
            C2_PITCH_NO := NULL;
            C2_ADD1 := NULL;
            C2_ADD2 := NULL;
            C2_CITY := NULL;
            C2_CITY_ID := NULL;
            C2_STATE := NULL;
            C2_STATE_CODE := NULL;
            C2_COUNTRY_ID := NULL;
            C2_PIN := NULL;
            C2_DOB := NULL;
            C2_EMAIL := NULL;
        WHEN OTHERS THEN
            RAISE;
    END;

    -- Fetching the details for C1 (Client's Basic Details)
    BEGIN
        SELECT 
            'VALID DATA IN C1',
            D.GUEST_CD,
            D.GUEST_NAME,
            D.MOBILE,
            D.EMP_NO,
            D.TELEPHONE
        INTO C1_MESSAGE, C1_GUEST_CODE, C1_GUEST_NAME, C1_MOBILE_NO, C1_EMP_NO, C1_TELEPHONE
        FROM 
            BAJAJ_VENUE_BOOKING D
        WHERE 
            D.GUEST_CD = p_guest_cd
            AND ROWNUM = 1;

    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            C1_MESSAGE := 'No data found for C1';
            C1_GUEST_CODE := NULL;
            C1_GUEST_NAME := NULL;
            C1_MOBILE_NO := NULL;
            C1_EMP_NO := NULL;
            C1_TELEPHONE := NULL;
        WHEN OTHERS THEN
            RAISE;
    END;

    -- Open the cursor for the combined results
    OPEN o_cursor FOR
    SELECT
        C1_MESSAGE AS C1_MESSAGE,
        C1_GUEST_CODE AS C1_GUEST_CODE,
        C1_GUEST_NAME AS C1_GUEST_NAME,
        C1_MOBILE_NO AS C1_MOBILE_NO,
        C1_EMP_NO AS C1_EMP_NO,
        C1_TELEPHONE AS C1_TELEPHONE,
        C2_MESSAGE AS C2_MESSAGE,
        C2_GUEST_SEX AS C2_GUEST_SEX,
        C2_GUEST_MATRIALST AS C2_GUEST_MATRIALST,
        TITLE AS TITLE,
        C2_PITCH_NO AS C2_PITCH_NO,
        C2_ADD1 AS C2_ADD1,
        C2_ADD2 AS C2_ADD2,
        C2_CITY AS C2_CITY,
        C2_CITY_ID AS C2_CITY_ID,
        C2_STATE AS C2_STATE,
        C2_STATE_CODE AS C2_STATE_CODE,
        C2_COUNTRY_ID AS C2_COUNTRY_ID,
        C2_PIN AS C2_PIN,
        C2_DOB AS C2_DOB,
        C2_EMAIL AS C2_EMAIL
    FROM DUAL;

EXCEPTION
    WHEN OTHERS THEN
        RAISE;
END PSM_AO_CLIENT_BY_GUEST;