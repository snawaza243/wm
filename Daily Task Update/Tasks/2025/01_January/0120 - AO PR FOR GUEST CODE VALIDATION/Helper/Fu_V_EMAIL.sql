create or replace FUNCTION VALIDATE_EMAIL(STR VARCHAR2) RETURN NUMBER
  AS
   A NUMBER(10);

VFIRSTPART VARCHAR2 (100);

BEGIN
        IF LENGTH(STR)>8 THEN
        
            SELECT UPPER(REGEXP_SUBSTR (STR,'[^@]+', 1, 1)) INTO VFIRSTPART FROM DUAL;
            IF LENGTH(VFIRSTPART)<=4 THEN
                    RETURN 0;
            END IF;
            
            IF VFIRSTPART NOT IN('NOEMAIL')  THEN
                BEGIN
                    SELECT 1 INTO A FROM DUAL WHERE REGEXP_LIKE   (STR ,'^\w+(\.\w+)*+@\w+(\.\w+)+$');
                EXCEPTION WHEN NO_DATA_FOUND THEN
                    A:=0;
                END; 
            ELSE
                A:=0;
            END IF;       
        ELSE
            A:=0;
        END IF;    
        RETURN NVL(A,0);
  END;