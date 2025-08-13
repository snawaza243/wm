CREATE OR REPLACE PROCEDURE WEALTHMAKER.FN_VALIDATE_CLIENT_DT(PDT_NO VARCHAR2,PCLIENT_CODE NUMBER,RESULT OUT NUMBER,MESSAGE OUT VARCHAR2) AS
VINV_CODE  WEALTHMAKER.TB_DOC_UPLOAD.INV_CODE%TYPE;
BEGIN
    IF PDT_NO IS NULL THEN
             RESULT:=1; 
             RETURN;
    END IF;
    
    IF SUBSTR(PCLIENT_CODE,1,8)=42366873 THEN  /*Dummy Client*/
            RESULT:=1; 
            RETURN;
    END IF;
    
    BEGIN
            SELECT INV_CODE INTO VINV_CODE FROM WEALTHMAKER.TB_DOC_UPLOAD WHERE COMMON_ID=PDT_NO
            AND TRAN_TYPE IN('LI','GI') AND ROWNUM=1;
    EXCEPTION WHEN NO_DATA_FOUND THEN
            VINV_CODE:=NULL;
            RESULT:=1; 
    END;         
    
    IF SUBSTR(VINV_CODE,1,1)='4' AND SUBSTR(PCLIENT_CODE,1,1)='3' THEN
          RESULT:=0;  
          MESSAGE:='Document Uploaded in Client '||SUBSTR(VINV_CODE,1,8)||',You can not Update in Subbroker '||SUBSTR(PCLIENT_CODE,1,8)||'.';  
          RETURN;
    END IF;
    
    IF SUBSTR(VINV_CODE,1,1)='3' AND SUBSTR(PCLIENT_CODE,1,1)='4' THEN
          RESULT:=0;  
          MESSAGE:='Document Uploaded in Subbroker '||SUBSTR(VINV_CODE,1,8)||',You can not Update in Client.';  
          RETURN;
    END IF;
    
    IF SUBSTR(VINV_CODE,1,1)='3' AND SUBSTR(PCLIENT_CODE,1,1)='3'  THEN
        IF SUBSTR(VINV_CODE,1,8)=SUBSTR(PCLIENT_CODE,1,8) THEN
            RESULT:=1;
        ELSE
            RESULT:=0;  
            MESSAGE:='This AR Can Not be Updated in Subbroker '||SUBSTR(PCLIENT_CODE,1,8)||'.Document Uploaded in '|| SUBSTR(VINV_CODE,1,8) ||'.'; 
            RETURN;  
        END IF;    
    ELSE
        RESULT:=1;    
    END IF;
    
END;
/