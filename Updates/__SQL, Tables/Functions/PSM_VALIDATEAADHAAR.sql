CREATE OR REPLACE FUNCTION WEALTHMAKER.PSM_VALIDATEAADHAAR(STR VARCHAR2) RETURN VARCHAR2 IS
    STRTEMP VARCHAR2(50);
BEGIN
    STRTEMP := TRIM(STR);

    -- Check if NULL or empty
    IF STRTEMP IS NULL OR LENGTH(STRTEMP) = 0 THEN
        RETURN 'Miss';
    END IF;

    -- Ensure exactly 12 characters and numeric only
    IF LENGTH(STRTEMP) <> 12 OR NOT REGEXP_LIKE(STRTEMP, '^[0-9]+$') THEN
        RETURN 'Invalid';
    END IF;

    -- Ensure it is not all zeros
    IF STRTEMP = '000000000000' THEN
        RETURN 'Invalid';
    END IF;

    RETURN 'Valid';
END;
/
