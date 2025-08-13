CREATE OR REPLACE FUNCTION WEALTHMAKER.FN_CHECK_FOR_APPROVAL_ALL(PDT_NO VARCHAR2) RETURN NUMBER IS
VFLAG                                            NUMBER;
VGI_REQUEST                              WEALTHMAKER.TB_DOC_UPLOAD.GI_REQUEST%TYPE;
VGI_REQUEST_STATUS               WEALTHMAKER.TB_DOC_UPLOAD.GI_REQUEST_STATUS%TYPE;
VSCH_CODE                                 WEALTHMAKER.TB_DOC_UPLOAD.SCH_CODE%TYPE;
VFRESH_RENEW                         WEALTHMAKER.TB_DOC_UPLOAD.FRESH_RENEW%TYPE;
VMAIN_PT_CODE                        WEALTHMAKER.BAJAJ_POLICY_TYPE.MAIN_PT_CODE%TYPE;
BEGIN
           BEGIN
                      SELECT NVL(GI_REQUEST,0),NVL(GI_REQUEST_STATUS,0),SCH_CODE,FRESH_RENEW INTO 
                      VGI_REQUEST,VGI_REQUEST_STATUS,VSCH_CODE,VFRESH_RENEW FROM WEALTHMAKER.TB_DOC_UPLOAD 
                      WHERE COMMON_ID=PDT_NO AND TRAN_TYPE IN('LI','MF','FI','GI') AND ROWNUM=1;
           EXCEPTION WHEN OTHERS THEN
                       VGI_REQUEST:=0;                      
           END;           
          
           IF VGI_REQUEST=1 AND VGI_REQUEST_STATUS=0 THEN
                     VFLAG:=2;       /*Approval request for this transaction has already been raised.*/
           ELSIF VGI_REQUEST=1 AND VGI_REQUEST_STATUS=1 THEN
                     VFLAG:=0;       /* Approval Request for this transaction has been approved.*/        
           ELSIF VGI_REQUEST=1 AND VGI_REQUEST_STATUS=2 THEN
                     VFLAG:=4;       /*Approval request for this transaction has rejected by Management.*/
           ELSIF VGI_REQUEST=0 THEN
                     VFLAG:=1;       /*Raise A Approval Request*/
           END IF;   
                 
           RETURN VFLAG;
END;
/