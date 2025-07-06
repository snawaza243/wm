SELECT * FROM BAJAJ_DUE_DATA 
                              WHERE UPPER(TRIM(POLICY_NO)) = 558452792
                              AND UPPER(TRIM(COMPANY_CD)) = 'BAJAJ A'
                              AND MON_NO = 9 
                              AND YEAR_NO = 2040
                              AND IMPORTDATATYPE = 'DUEDATA'
                              
                              /*
                              "[policyNo, 558452792]",policyNo,558452792
"[companyCd, BAJAJ A]",companyCd,"BAJAJ A"
"[month, 9]",month,9
"[year, 2040]",year,2040
"[importType, DUEDATA]",importType,DUEDATA

*/



select count(*) from bajaj_due_data where mon_no = 9 and year_no = 2040;