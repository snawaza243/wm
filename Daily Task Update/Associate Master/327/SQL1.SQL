/*TYPE (CmbStatus): INDIVIDUALS, PARTNERS, CORPORATE, NRI, OTHERS*/
select itemname, ITEMID, itemserialnumber from fixeditem where itemtype='5';
Select itemserialnumber from fixeditem where itemname IS NOT NULL;

PSM_AM_OTHER_TYPE;
select itemserialnumber, itemname from fixeditem where itemtype = '5' ORDER BY ITEMNAME ;





/*ASSOCIATE_TYPE/ INVESTOR_TYPE (CmbStatus): INDIVIDUALS, HUF, COMPANY, TRUST, SOCIETY, BAJAJ CAPITAL STAFF*/
select INVESTOR_TYPE from INVESTORTYPE  where Investor_code not in (13,14) ;

PSM_AM_AssociateType;




SELECT statusid, AGENT_MASTER.* FROM AGENT_MASTER WHERE statusid IS NOT NULL;


PSM_AM_INSERT_AGENT_MASTER;
PSM_AM_UPDATE_AGENT_MASTER;


