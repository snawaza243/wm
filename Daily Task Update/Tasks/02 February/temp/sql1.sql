PSM_ANA_SR_BR;
psm_am_branch_master_3;

psm_am_get_location_list;

PSM_AM_ASSOCIATETYPECAT;  -- asc cat
PSM_AM_AssociateType;
PSM_AM_GET_PAYMODEID;
PSM_AM_GET_BANKACCTYPEID;
PSM_AM_OTHER_TYPE;
PSM_AM_SANA_RMBSRC;

PSM_AO_GetCitiesByState;


PSM_AM_AGENT_BY_ID;


SELECT LOCATION_ID, LOCATION_NAME, city_id,() pincode
    FROM LOCATION_MASTER;
    
    select itemname from fixeditem where itemtype='10';     -- payment mode
    select itemname from fixeditem where itemtype='7';      -- ACCOUNT TYPE
    select itemname from fixeditem where itemtype='6';      -- EXAM TYPE
    select itemname from fixeditem where itemtype='5';      -- TYPE 
    select pincode, location_name from location_master where location_name ='patna';
    
    
    select state_id, state_name, country_id from state_master ;
    
    city_id in ('C1586', 'C0087', 'C0182') and
NEW DELHI	C0087
NEW DELHI	C1586
    select agent_name from agent_master where city_id = 'C1586';
    
    select city_name, CITY_ID, DEL_FLAG from city_master where city_name like upper('%DELHI%'); --city_id in ('C1586', 'C0087', 'C0182')


-- loginid = 130835, 

        SELECT br.*
        --, (select login_id from USERDETAILS_JI where BRANCH_ID = BR.BRANCH_CODE and rownum = 1) as loginid
        FROM BRANCH_MASTER BR
        where branch_tar_cat in (186, 615, 308)
        and branch_code is not null 
        AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = '107822' AND ROLE_ID is not null);









SELECT BR.*
FROM BRANCH_MASTER BR
INNER JOIN USERDETAILS_JI UJ ON BR.BRANCH_CODE = UJ.BRANCH_ID
WHERE BR.branch_tar_cat IN (186, 615, 308)
AND UJ.LOGIN_ID = '38387'

select * from USERDETAILS_JI where login_id = '107822';

select * from branch_master where branch_code = '10010193';

SELECT *
        FROM BRANCH_MASTER BR
        where branch_tar_cat in (186, 615, 308)
        --(184, 185, 186, 187, 189, 231, 283, 308, 368, 369, 609, 610, 611, 615) -- branch_tar_cat - 186, 615, 308 corporate partner , -- only ang : 189, 231, 186
        and branch_code is not null 
        AND EXISTS( SELECT 1 FROM USERDETAILS_JI WHERE BRANCH_ID = BR.BRANCH_CODE AND LOGIN_ID = '38387' AND ROLE_ID is not null)

    