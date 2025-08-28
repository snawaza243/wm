CREATE OR REPLACE PROCEDURE PSM_MF2_CHK_AUTO_SWITCH (
    P_SCHEME_TEXT     IN  VARCHAR2,   -- e.g., "SchemeName=SCH001"
    P_TXN_TYPE        IN  VARCHAR2,   -- e.g., "SIP", "STP", or others
    P_IS_CHECKED      IN  BOOLEAN,    -- Checkbox value: TRUE or FALSE
    P_CURSOR          OUT VARCHAR2    -- Message to show if rejected
) AS
    V_MESSAGE         VARCHAR2(255);
    V_SCHEME_CODE     VARCHAR2(100);
    V_STATUS          NUMBER;
BEGIN

    V_MESSAGE:='SUCCESS: Auto Switch is valid';
    
    IF INSTR(P_SCHEME_TEXT, '=') > 0 THEN
        V_SCHEME_CODE := SUBSTR(P_SCHEME_TEXT, INSTR(P_SCHEME_TEXT, '=') + 1);
    ELSE
        V_SCHEME_CODE := ' ';
    END IF;

    -- If checkbox is checked
    IF P_IS_CHECKED THEN
        -- Check if transaction type is SIP or STP
        IF P_TXN_TYPE IN ('SIP', 'STP') THEN
            V_MESSAGE := 'Auto Switch on Maturity can be selected for Close Ended and NON-SIP transaction';
            RETURN;
        END IF;

        -- Call function to check scheme status
        BEGIN
            SELECT GET_SCH_STATUS_CLOSE_END(V_SCHEME_CODE)
            INTO V_STATUS
            FROM DUAL;

            V_MESSAGE := 'Auto Switch on Maturity can be selected for Close Ended and NON-SIP transaction';
        END IF;

        BEGIN
            SELECT GET_SCH_STATUS_CLOSE_END(V_SCHEME_CODE)
            INTO V_STATUS
            FROM DUAL;

            IF V_STATUS = 0 THEN
                V_MESSAGE := 'Auto Switch on Maturity can be selected for Close Ended and NON-SIP transaction';
            END IF;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                V_MESSAGE := 'Scheme status not found';
        END;
    ELSE
        V_MESSAGE := 'SUCCESS: Auto Switch is valid';
    END IF;

    OPEN P_CURSOR FOR SELECT V_MESSAGE AS MESSAGE FROM DUAL;

END PSM_MF2_CHK_AUTO_SWITCH;