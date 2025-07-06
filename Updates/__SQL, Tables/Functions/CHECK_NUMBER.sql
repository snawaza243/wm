CREATE OR REPLACE FUNCTION WEALTHMAKER.CHECK_NUMBER(MOBILENO VARCHAR) RETURN VARCHAR2 IS
RES VARCHAR2(1):='N';
BEGIN
     IF MOBILENO='9999998279' AND VALIDATE_MOBILE('9999998279')=1 THEN
         RES:='Y';   
     ELSE
         IF SUBSTR(MOBILENO,1,1) ='7' OR SUBSTR(MOBILENO,1,1) ='8' OR SUBSTR(MOBILENO,1,1) ='6' OR SUBSTR(MOBILENO,1,1) ='9' OR TRIM(LPAD(MOBILENO,1)) ='9' THEN
               IF SUBSTR(MOBILENO,1,1) <>'p' THEN
                   IF REGEXP_LIKE(MOBILENO, '^[[:digit:]]{10}$') THEN
                       IF (REGEXP_COUNT(MOBILENO,'1'))>6 OR (REGEXP_COUNT(MOBILENO,'2'))>6 OR  (REGEXP_COUNT(MOBILENO,'3'))>6 OR (REGEXP_COUNT(MOBILENO,'4'))>6 
                              OR (REGEXP_COUNT(MOBILENO,'5'))>6  OR (REGEXP_COUNT(MOBILENO,'6'))>6 OR (REGEXP_COUNT(MOBILENO,'7'))>6 OR (REGEXP_COUNT(MOBILENO,'8'))>6 
                              OR (REGEXP_COUNT(MOBILENO,'9'))>6 OR (REGEXP_COUNT(MOBILENO,'0'))>6 THEN
                              RES:='N';
                       ELSE  RES:='Y';
                       END IF;
                   END IF;
               END IF;      
         END IF;
     END IF;        
     RETURN RES;            
END;
/
