CREATE OR REPLACE PROCEDURE WEALTHMAKER.NPS_ECSTRAN_GENERATE_IMP(CONSUMERCODE VARCHAR2)
IS
   CURSOR ECS_INF
   IS
    SELECT A.REF_TRAN_CODE, A.ECS_AMT,
     A.LOGGEDUSERID,A.CONSUMER_CODE,A.IMPORT_DT,A.TR_DATE
         FROM NPS_ECS_TBL_IMP A WHERE TO_CHAR(REF_TRAN_CODE)=TO_CHAR(CONSUMERCODE);

   I         NUMBER;
   J         NUMBER;   
   K         NUMBER;   
   INV       VARCHAR2 (15);
   BR        NUMBER;
   CL        NUMBER;
   RM        NUMBER;
   NO_DAYS   NUMBER;
   APPNO     VARCHAR(5);
   FOLIONO   VARCHAR2(10);
   STARTON1  DATE;
   ENDON1    DATE;
   EXECTIME1 VARCHAR2(15);     
BEGIN
    
    STARTON1    := SYSDATE;
    ENDON1      := NULL;
    EXECTIME1   := '';
    INSERT INTO PROCEDURE_RUN_V
                  (PROC, STARTON, ENDON, RUNDATE, RUNBY, OWNER,
                   RUNFOR, MODULE, SUBMODULE, LOGIN_ID
                  )
           VALUES ('NPS_ECS_TRAN_IMP', STARTON1, NULL, TO_DATE (STARTON1, 'dd/MM/yyyy'), 'PROC', 'MAYANK',
                   SYSDATE, 'WEALTHMAKER', NULL, '99743'
                  );

      COMMIT;
      
   FOR C11 IN ECS_INF
   LOOP
         DBMS_OUTPUT.PUT_LINE('IN');
         SELECT MAX(CLIENT_CODE)
           INTO INV
           FROM TRANSACTION_ST
          WHERE MANUAL_ARNO = C11.REF_TRAN_CODE;
        BEGIN
         SELECT BRANCH_CODE, SOURCE_ID, RM_CODE
           INTO BR, CL, RM
           FROM INVESTOR_MASTER
          WHERE INV_CODE = INV;
        EXCEPTION WHEN NO_DATA_FOUND THEN
            BR:=NULL;
        END;   
        IF BR IS NOT NULL THEN
                      ECS_ENTRY_IMP1(C11.REF_TRAN_CODE,                                
                                C11.ECS_AMT,
                                C11.TR_DATE,
                                C11.LOGGEDUSERID,
                                'NPS_ECS'
                               );
        END IF;                         
     
   END LOOP;

   ENDON1      := SYSDATE;
    EXECTIME1   := PROCEDURETIMESTAMP (STARTON1, ENDON1);
    UPDATE PROCEDURE_RUN_V
         SET ENDON = ENDON1,
             EXECTIME = EXECTIME1
       WHERE UPPER (TO_CHAR (STARTON, 'dd/MM/yyyy hh:mi:ss am')) = UPPER (TO_CHAR (STARTON1, 'dd/MM/yyyy hh:mi:ss am'));
      COMMIT;
END;
/