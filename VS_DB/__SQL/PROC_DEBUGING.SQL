-- 🔍 1) Check if the procedure exists and is valid
-- PROMPT ==== Checking Procedure Status ====
SELECT OBJECT_NAME, OBJECT_TYPE, STATUS
FROM ALL_OBJECTS
WHERE OBJECT_NAME = 'PRC_VALIDATE_CROSS_CHNL_INFO'
AND OWNER = 'WEALTHMAKER';

---- if status is envalid then see error

-- Show compilation errors for the procedure
-- SHOW ERRORS PROCEDURE WEALTHMAKER.PRC_VALIDATE_CROSS_CHNL_INFO;

-- OR (if SHOW ERRORS doesn't work in your tool):
SELECT LINE, POSITION, TEXT
FROM ALL_ERRORS
WHERE OWNER = 'WEALTHMAKER'
  AND NAME = 'PRC_VALIDATE_CROSS_CHNL_INFO'
ORDER BY SEQUENCE;






-- 🔍 2) Check if you have EXECUTE privilege on it
-- PROMPT ==== Checking Procedure Privileges ====
SELECT GRANTEE, PRIVILEGE, OWNER, TABLE_NAME
FROM ALL_TAB_PRIVS
WHERE TABLE_NAME = 'PRC_VALIDATE_CROSS_CHNL_INFO'
AND OWNER = 'WEALTHMAKER'
AND PRIVILEGE = 'EXECUTE';

-- 🔍 3) Check if there is a synonym and where it points
-- PROMPT ==== Checking Synonym Resolution ====
SELECT OWNER, SYNONYM_NAME, TABLE_OWNER, TABLE_NAME
FROM ALL_SYNONYMS
WHERE SYNONYM_NAME = 'PRC_VALIDATE_CROSS_CHNL_INFO';

-- 🔍 4) Optional: Show procedure parameters (to confirm signature)
-- PROMPT ==== Checking Procedure Parameters ====
SELECT ARGUMENT_NAME, POSITION, DATA_TYPE, IN_OUT
FROM ALL_ARGUMENTS
WHERE OBJECT_NAME = 'PRC_VALIDATE_CROSS_CHNL_INFO'
AND OWNER = 'WEALTHMAKER'
ORDER BY POSITION;
