DELETE FROM PSM_GLOBAL_WORKING_DATA WHERE FIELD_NAME = '[CODE]-->COMPANY_CD,[Policy No]-->POLICY_NO,[Client Name]-->CL_NAME,[Status]-->STATUS_CD,[Payment Type]-->PAY_MODE,[Doc]-->DOC,[Policy Term]-->PLY_TERM,[Mobile No]-->CL_MOBILE,[Sum Assured]-->SA';

SELECT * FROM PSM_GLOBAL_WORKING_DATA;
COMMIT;

ALTER TABLE PSM_GLOBAL_WORKING_DATA MODIFY FIELD_name VARCHAR2(3000 BYTE);

desc PSM_GLOBAL_WORKING_DATA;