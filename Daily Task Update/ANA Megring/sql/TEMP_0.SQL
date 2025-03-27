-- agent_code='30036978'"


Select * from agent_master where agent_code='30036978';

Select * from agent_master where agent_code=30041888;

select inv_code,investor_name from investor_master where source_id=30041888;
select * from investor_master where inv_code = '30036978001';

-- "Meet K Tahkkar", CLEANED =  "MEETKTAHKKAR", WITH PTR LEFT TO 8 %MEETKTAH%"
Select inv_code, investor_name, source_id from investor_master where source_id=30036978 and investor_name = 'Meet K Tahkkar' and instr(trim(upper(investor_name)),'HUF')=0;

30041888
select * from investor_master where investor_name = 'Meet K Tahkkar';
select * from agent_master where agent_code = 30036978;


Select inv_code from investor_master where source_id=" & msfgMain.TextMatrix(1, 2) & " and substr(replace(replace(trim(upper(investor_name)),'.',''),' ',''),1,8) like '%" & Left(Replace(Replace(Trim(UCase(RsData("investor_name"))), ".", ""), " ", ""), 8) & "%' and instr(trim(upper(investor_name)),'HUF')