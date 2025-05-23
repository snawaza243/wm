create or replace PROCEDURE PSM_AM_AGENT_BY_ID (
-- This is agent search list procedure for associate master
    P_BRANCH_ID IN VARCHAR2,    
    P_CITY_ID IN VARCHAR2,
    P_MOBILE IN VARCHAR2,
    P_PHONE IN VARCHAR2,
    P_ASSOCIATE_CODE IN VARCHAR2, 
    P_AGENT_CODE IN VARCHAR2, 

    P_PAN_NO IN VARCHAR2,
    P_ASSOCIATE_NAME IN VARCHAR2,
    P_ASSOCIATE_LIST OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN P_ASSOCIATE_LIST FOR
    SELECT Agent_Master.*, (select city_name from city_master where city_id = Agent_Master.city_id ) as cm_city_name
    FROM Agent_Master
    WHERE (SOURCEID = P_BRANCH_ID OR P_BRANCH_ID IS NULL)
    AND (UPPER(TRIM(CITY_ID)) = UPPER(TRIM(P_CITY_ID)) OR P_CITY_ID IS NULL)
    AND (UPPER(TRIM(AGENT_CODE)) = UPPER(TRIM(P_AGENT_CODE)) OR P_AGENT_CODE IS NULL)
    AND (UPPER(TRIM(MOBILE)) = UPPER(TRIM(P_MOBILE)) OR P_MOBILE IS NULL)
    AND (UPPER(TRIM(PHONE)) = UPPER(TRIM(P_PHONE)) OR P_PHONE IS NULL)
    AND (UPPER(TRIM(EXIST_CODE)) = UPPER(TRIM(P_ASSOCIATE_CODE)) OR P_ASSOCIATE_CODE IS NULL)
    AND (UPPER(TRIM(PAN)) = UPPER(TRIM(P_PAN_NO)) OR P_PAN_NO IS NULL)
    AND (UPPER(TRIM(AGENT_NAME)) LIKE '%' || UPPER(TRIM(P_ASSOCIATE_NAME)) || '%' OR P_ASSOCIATE_NAME IS NULL)
    and SOURCEID in (
     SELECT branch_code FROM BRANCH_MASTER BR where branch_tar_cat in (186, 615, 308) 
     --AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = '107822' AND ROLE_ID is not null);
    )
    ;

END;