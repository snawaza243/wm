
SELECT DISTINCT  a.iss_code AS mut_code, b.osch_code AS sch_code, b.OSCH_NAME AS SCH_NAME, c.fiGure 
FROM iss_master a
LEFT JOIN other_product b ON a.iss_code = b.iss_code
LEFT JOIN upfront_recd c ON b.osch_code = c.sch_code
WHERE 
    a.iss_code IN ('MFIS02335', 'MFIS02336', 'MFIS02342')
    AND a.iss_code = 'MFIS02335';

select DISTINCT 

a.iss_code mut_code, -- for value add

b.osch_code sch_code, -- for vlaue adn add wiht '#' character
B.OSCH_NAME SCH_NAME ,-- for item name 
C.fiGure 
from iss_master a, other_product b, upfront_recd c
where a.iss_code=b.iss_code and b.osch_code=c.sch_code 
and A.iss_code in ('MFIS02335','MFIS02336','MFIS02342')
