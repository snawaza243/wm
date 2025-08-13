CREATE OR REPLACE PROCEDURE WEALTHMAKER.FN_VALIDATE_BRANCH_RM_DT(PDT_NO VARCHAR2,PBRANCH_CODE NUMBER,PPAYROLL_ID VARCHAR2,RESULT OUT NUMBER,MESSAGE OUT VARCHAR2) AS
VBUSI_BRANCH_CODE    WEALTHMAKER.TB_DOC_UPLOAD.BUSI_BRANCH_CODE%TYPE; 
VDTBRANCH_NAME       WEALTHMAKER.BRANCH_MASTER.BRANCH_NAME%TYPE;
VPUNCH_BRANCH_NAME   WEALTHMAKER.BRANCH_MASTER.BRANCH_NAME%TYPE;

VBUSI_RM_CODE    WEALTHMAKER.TB_DOC_UPLOAD.BUSI_RM_CODE%TYPE; 
VDTRM_NAME       WEALTHMAKER.EMPLOYEE_MASTER.RM_NAME%TYPE;
VPUNCH_RM_NAME   WEALTHMAKER.EMPLOYEE_MASTER.RM_NAME%TYPE;
VREMARK          WEALTHMAKER.TB_DOC_UPLOAD.REMARK%TYPE;
BEGIN
    IF PDT_NO IS NULL THEN
             RESULT:=1; 
             RETURN;
    END IF;
    
    IF PBRANCH_CODE=10010081 THEN  /*Dummy Client 42366873001's branch*/
            RESULT:=1; 
            RETURN;
    END IF;
    
    -------------------------------------------------------------------------Business RM Validation--------------------------------------------------------------------------------------------
    
    BEGIN
            SELECT BUSI_RM_CODE,REMARK INTO VBUSI_RM_CODE,VREMARK FROM WEALTHMAKER.TB_DOC_UPLOAD WHERE COMMON_ID=PDT_NO
            AND TRAN_TYPE IN('LI','GI') AND ROWNUM=1;
    EXCEPTION WHEN NO_DATA_FOUND THEN
            VBUSI_RM_CODE:=NULL;
            RESULT:=1; 
    END;         
    
    IF INSTR(VREMARK,'Fyntune')>0 THEN
       RESULT:=1; 
       RETURN;
    END IF; 
    
    
    BEGIN
                SELECT RM_NAME INTO VDTRM_NAME FROM WEALTHMAKER.EMPLOYEE_MASTER 
                WHERE PAYROLL_ID=VBUSI_RM_CODE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
                VDTRM_NAME:=NULL;                
    END;
    
    BEGIN
                SELECT RM_NAME INTO VPUNCH_RM_NAME FROM WEALTHMAKER.EMPLOYEE_MASTER 
                WHERE PAYROLL_ID=PPAYROLL_ID;
    EXCEPTION WHEN NO_DATA_FOUND THEN
                VPUNCH_BRANCH_NAME:=NULL;                
    END;
    
    IF VBUSI_RM_CODE<>PPAYROLL_ID THEN
          RESULT:=0;  
          MESSAGE:='Document Uploaded in Business RM '||VDTRM_NAME||',You can not Punch in Business RM '||VPUNCH_RM_NAME||'.';  
          RETURN;
    ELSE
           RESULT:=1;          
    END IF;
    
    -------------------------------------------------------------------------Business Branch Validation----------------------------------------------------------------------------------------
    BEGIN
            SELECT BUSI_BRANCH_CODE INTO VBUSI_BRANCH_CODE FROM WEALTHMAKER.TB_DOC_UPLOAD WHERE COMMON_ID=PDT_NO
            AND TRAN_TYPE IN('LI','GI') AND ROWNUM=1;
    EXCEPTION WHEN NO_DATA_FOUND THEN
            VBUSI_BRANCH_CODE:=NULL;
            RESULT:=1; 
    END;         
    
    IF VBUSI_BRANCH_CODE='10020648' THEN /*NATIONAL ANG UNIT*/
                RESULT:=1;      
                RETURN;
    END IF;
    
    BEGIN
                SELECT BRANCH_NAME INTO VDTBRANCH_NAME FROM WEALTHMAKER.BRANCH_MASTER 
                WHERE BRANCH_CODE=VBUSI_BRANCH_CODE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
                VDTBRANCH_NAME:=NULL;                
    END;
    
    BEGIN
                SELECT BRANCH_NAME INTO VPUNCH_BRANCH_NAME FROM WEALTHMAKER.BRANCH_MASTER 
                WHERE BRANCH_CODE=PBRANCH_CODE;
    EXCEPTION WHEN NO_DATA_FOUND THEN
                VPUNCH_BRANCH_NAME:=NULL;                
    END;
    
    IF VBUSI_BRANCH_CODE<>PBRANCH_CODE THEN
          RESULT:=0;  
          MESSAGE:='Document Uploaded in Branch '||VDTBRANCH_NAME||',You can not Punch in Branch '||VPUNCH_BRANCH_NAME||'.';  
          RETURN;
    ELSE
           RESULT:=1;          
    END IF;
    ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
END;
/