create or replace PROCEDURE PSM_AO_GUESTVALIDAIOTN(
    P_GUEST_CODE      IN VARCHAR2,
    P_BUSI_CODE       OUT VARCHAR2,
    P_MESSAGE         OUT VARCHAR2
)
AS
    V_EMP_NO          VARCHAR2(100);
BEGIN
    -- Check if Guest Code is provided
    IF P_GUEST_CODE IS NULL OR TRIM(P_GUEST_CODE) = '' THEN
        P_MESSAGE := 'Guest code is empty or invalid';
        P_BUSI_CODE := NULL;
        RETURN;
    END IF;

    -- Fetch business code for the given Guest Code
    BEGIN
        SELECT NVL(EMP_NO, '0') 
        INTO V_EMP_NO
        FROM BAJAJ_VENUE_BOOKING
        WHERE GUEST_CD = P_GUEST_CODE;

        P_BUSI_CODE := V_EMP_NO;
        P_MESSAGE := 'Operation Successful';
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            P_MESSAGE := 'Guest code not found in the database';
            P_BUSI_CODE := NULL;
        WHEN OTHERS THEN
            P_MESSAGE := 'An unexpected error occurred while fetching data';
            P_BUSI_CODE := NULL;
    END;
END PSM_AO_GUESTVALIDAIOTN;