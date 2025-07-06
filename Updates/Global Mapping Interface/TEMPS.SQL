SELECT table_name as name, table_name as value  FROM all_tables where upper(table_name) is not null order by table_name;

SELECT table_name as name, table_name as value  FROM all_tables
--WHERE UPPER(table_name) LIKE 'BAJAJ_DUE%' OR UPPER(table_name) LIKE 'DUE%' OR UPPER(table_name) LIKE 'BAJAJ_PAID%' 
WHERE UPPER(TABLE_NAME) LIKE 'NPS%'
order by table_name;


SELECT COMPANY_CD, COMPANY_NAME FROM bajaj_company_master ORDER BY COMPANY_NAME;

SELECT * FROM GLOBAL_EXCELDB_MAPPING;

INSERT INTO DUEPAID_MAPPEDDATA (FILETYPE, MASKEDSTR) VALUES('DUE','ASDF');

SELECT * FROM ALL_TAB_COLS WHERE TABLE_NAME = 'BAJAJ_DUE_DATA' 
;