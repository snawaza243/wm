CREATE OR REPLACE PROCEDURE PSM_MF2_CALCULATE_SIP_END_DATE (
    P_START_DATE        IN  VARCHAR2,   -- Format: 'DD/MM/YYYY'
    P_INSTALLMENTS      IN  NUMBER,
    P_FREQUENCY_CODE    IN  VARCHAR2,   -- e.g., '208', '173', etc.
    P_OPT99             IN  BOOLEAN,
    P_END_DATE          OUT VARCHAR2    -- Format: 'DD/MM/YYYY'
) AS
    V_START_DATE        DATE;
    V_END_DATE          DATE;
    V_MAX_INSTALLMENTS  NUMBER;
BEGIN
    -- Convert input date
    V_START_DATE := TO_DATE(P_START_DATE, 'DD/MM/YYYY');

    -- Default end date
    P_END_DATE := '__/__/____';

    -- Calculate max possible installments
    SELECT ROUND(MONTHS_BETWEEN(TO_DATE('01/12/2099', 'DD/MM/YYYY'), V_START_DATE))
    INTO V_MAX_INSTALLMENTS
    FROM DUAL;

    -- If Opt99 is TRUE and start date is valid
    IF P_OPT99 AND P_START_DATE <> '__/__/____' THEN
        P_END_DATE := '01/12/2099';
        RETURN;
    END IF;

    -- If frequency is selected and Opt99 is FALSE
    IF P_FREQUENCY_CODE IS NOT NULL AND NOT P_OPT99 THEN
        CASE P_FREQUENCY_CODE
            WHEN '208' THEN
                V_END_DATE := ADD_MONTHS(V_START_DATE, FLOOR(P_INSTALLMENTS / 30));
            WHEN '173' THEN
                V_END_DATE := WEALTHMAKER.ADD_WEEKS(V_START_DATE, P_INSTALLMENTS);
            WHEN '174' THEN
                V_END_DATE := ADD_MONTHS(V_START_DATE, 6 * P_INSTALLMENTS);
            WHEN '175' THEN
                IF P_INSTALLMENTS < V_MAX_INSTALLMENTS THEN
                    V_END_DATE := ADD_MONTHS(V_START_DATE, 1 * P_INSTALLMENTS);
                ELSE
                    P_END_DATE := '01/12/2099';
                    RETURN;
                END IF;
            WHEN '176' THEN
                V_END_DATE := ADD_MONTHS(V_START_DATE, 3 * P_INSTALLMENTS);
            WHEN '301' THEN
                V_END_DATE := ADD_MONTHS(V_START_DATE, 12 * P_INSTALLMENTS);
            ELSE
                V_END_DATE := NULL;
        END CASE;

        -- Format output date
        IF V_END_DATE IS NOT NULL THEN
            P_END_DATE := TO_CHAR(V_END_DATE - 1, 'DD/MM/YYYY');
        END IF;
    END IF;
END PSM_MF2_CALCULATE_SIP_END_DATE;