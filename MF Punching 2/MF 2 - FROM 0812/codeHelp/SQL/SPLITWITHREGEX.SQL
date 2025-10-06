  DECLARE STR1 VARCHAR2(100); 

        BEGIN
            IF P_CLOSE_SCH_TEXT IS NOT NULL AND P_CLOSE_SCH_TEXT <> '' THEN
                STR1 := REGEXP_SUBSTR(P_CLOSE_SCH_TEXT, '[^=]+', 1, 2);
                MYCLOSESCHCODE := STR1;
            END IF;
        END;