CREATE OR REPLACE PROCEDURE PSM_MF2_SCH3_CHK_CLOSE(
    PX_LOG_ID   IN VARCHAR2,
    PX_ROLE_ID  IN VARCHAR2,
    PX_INDEX    IN VARCHAR2,
    PX_SCHA     IN VARCHAR2,
    PX_CHK      IN CHAR,
    PX_SIP_STP  IN VARCHAR2,
    PX_CURSOR   OUT SYS_REFCURSOR
)AS 

    V_CLOSE_SCH_END CHAR;

BEGIN

    IF PX_SCHA IS NOT NULL THEN
        select get_sch_status_close_end(PX_SCHA) INTO V_CLOSE_SCH_END from dual;
        IF PX_CHK = '1' THEN
            IF PX_SIP_STP IN ('SIP', 'SIP') OR V_CLOSE_SCH_END = '0' THEN
                OPEN PX_CURSOR FOR 
                    SELECT 
                    'Auto Switch on Maturity can be selected for Close Ended and NON-SIP transaction' AS MSG,
                    '' AS CHK_CLOSE_SCH,
                    '0' AS CHK_CLOSE_STATUS                    
                    FROM DUAL;
                    RETURN;
            END IF;
        ELSE
            OPEN PX_CURSOR FOR 
                SELECT 
                'SUCCESS:CHK CLOSE' AS MSG,
                '' AS CHK_CLOSE_SCH,
                '1' AS CHK_CLOSE_STATUS                    
                FROM DUAL;
                RETURN;
        END IF;
    END IF;
END;