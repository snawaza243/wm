CREATE OR REPLACE PROCEDURE PSM_MF2_AMC_CHANGE(
    PX_LOG_ID   IN VARCHAR2,
    PX_ROLE_ID  IN VARCHAR2,
    PX_INDEX    IN VARCHAR2,
    PX_AMC_CD   IN VARCHAR2,
    PX_AMC_NM   IN VARCHAR2,
    PX_CURSOR   OUT SYS_REFCURSOR
) AS 
    V_MUT_CODE1 VARCHAR2(10);
    V_ISS_CODE  VARCHAR2(10);
    V_QUERY     VARCHAR2(4000);
BEGIN
    -- Check if AMC name parameter is not null
    IF PX_AMC_NM IS NOT NULL THEN
        -- Try to find mut_code from mut_fund using PX_AMC_NM
        BEGIN
            SELECT mut_code 
              INTO V_MUT_CODE1 
              FROM mut_fund 
             WHERE UPPER(TRIM(mut_name)) = UPPER(TRIM(PX_AMC_NM)) 
               AND ROWNUM = 1;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                V_MUT_CODE1 := NULL;
        END;

        IF V_MUT_CODE1 IS NULL THEN
            -- If mut_code not found, try iss_code from iss_master
            BEGIN
                SELECT iss_code 
                  INTO V_ISS_CODE 
                  FROM iss_master 
                 WHERE UPPER(TRIM(iss_name)) = UPPER(TRIM(PX_AMC_NM)) 
                   AND ROWNUM = 1;
            EXCEPTION
                WHEN NO_DATA_FOUND THEN
                    V_ISS_CODE := NULL;
            END;

            IF V_ISS_CODE IS NOT NULL THEN
                -- If iss_code found, select osch_code and osch_name from other_product 
                OPEN PX_CURSOR FOR SELECT 'SUCCESS' AS MSG, osch_code AS CODE, osch_name AS NAME FROM other_product WHERE iss_code = V_ISS_CODE ORDER BY osch_name;
                RETURN;
            ELSE
                -- No matching AMC found
                OPEN PX_CURSOR FOR SELECT '' AS MSG, NULL AS code, NULL AS name FROM DUAL WHERE 1=0;
                RETURN;
            END IF;

        ELSE
            -- If mut_code found, select schemes from scheme_info
            OPEN PX_CURSOR FOR SELECT 'SUCCESS' AS MSG, sch_code CODE, sch_name NAME FROM scheme_info WHERE mut_code = V_MUT_CODE1 ORDER BY sch_name;
            RETURN;
        END IF;

    ELSE
        -- PX_AMC_NM is NULL: return empty cursor
        OPEN PX_CURSOR FOR SELECT '' AS MSG, NULL AS code, NULL AS name FROM DUAL WHERE 1=0;
    END IF;

END;
/
