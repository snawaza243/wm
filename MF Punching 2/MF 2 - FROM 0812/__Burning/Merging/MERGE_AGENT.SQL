CREATE OR REPLACE PROCEDURE WEALTHMAKER.PRC_INVESTOR_MERGE (  
    P_MAIN_CODE         VARCHAR2,
    P_TO_MERGE_CODE  VARCHAR2,
    P_LOGIN_ID       VARCHAR2,            
    P_ERRORMESSAGE   OUT VARCHAR2,
    P_SUCCESS         OUT VARCHAR2
    
    )
AS
-- LOCAL VARIABLE FOR CLIENT MERGE
    MYCLIENTCODE VARCHAR2(100) := NULL;
    MYMAINCODE VARCHAR2(100) := NULL;
    MYNEWCLIENTCODE VARCHAR2(100) := NULL;
    MYNEWMAINCODE VARCHAR2(100) := NULL;
    
    
    
    
     NEW_INV_CODE VARCHAR2(100) := NULL;
    M_COUNT NUMBER := 0;
    V_MAIN_AGENT_CODE VARCHAR2(100) := P_MAIN_CODE;
    
    -- Cursors for agent data
    CURSOR C_MERGED_AGENTS IS SELECT COLUMN_VALUE AS AGENT_CODE FROM TABLE(ANKIT_SPLIT(P_TO_MERGE_CODE,'#'));
    
    CURSOR C_INVESTORS(P_AGENT_CODE VARCHAR2) IS SELECT INV_CODE, INVESTOR_NAME FROM INVESTOR_MASTER WHERE SOURCE_ID = P_AGENT_CODE;
    
    CURSOR C_SIMILAR_INVESTOR(P_MAIN_AGENT VARCHAR2, P_INV_NAME VARCHAR2) IS
        SELECT INV_CODE 
        FROM INVESTOR_MASTER 
        WHERE SOURCE_ID = P_MAIN_AGENT 
        AND SUBSTR(REPLACE(REPLACE(TRIM(UPPER(INVESTOR_NAME)),'.',''),' ', ''), 1, 8) LIKE '%' || SUBSTR(REPLACE(REPLACE(TRIM(UPPER(P_INV_NAME)),'.',''),' ', ''), 1, 8) || '%'
        AND INSTR(TRIM(UPPER(INVESTOR_NAME)),'HUF') = 0
        AND ROWNUM = 1;
    
    V_SIMILAR_INV_CODE VARCHAR2(100);
    V_EXISTS NUMBER;
    
    

BEGIN
    P_ERRORMESSAGE := NULL;
    
    IF SUBSTR(P_MAIN_CODE,1,1) = '3' THEN    
        BEGIN
            FOR MERGED_AGENT IN C_MERGED_AGENTS
            
            LOOP
                -- Begin transaction for each merged agent
                SAVEPOINT start_agent_merge;
                
                -- Process each investor under the merged agent
                FOR RS_DATA IN C_INVESTORS(MERGED_AGENT.AGENT_CODE)
                LOOP
                    BEGIN
                        -- Check if similar investor exists for main agent
                        OPEN C_SIMILAR_INVESTOR(V_MAIN_AGENT_CODE, RS_DATA.INVESTOR_NAME);
                        FETCH C_SIMILAR_INVESTOR INTO V_SIMILAR_INV_CODE;
                        
                        IF C_SIMILAR_INVESTOR%FOUND THEN
                            NEW_INV_CODE := V_SIMILAR_INV_CODE;
                        ELSE
                            M_COUNT := M_COUNT + 1;
                            IF M_COUNT >= 999 THEN
                                NEW_INV_CODE := V_MAIN_AGENT_CODE || LPAD(M_COUNT, 5, '0');
                            ELSE
                                NEW_INV_CODE := V_MAIN_AGENT_CODE || LPAD(M_COUNT, 3, '0');
                            END IF;
                            
                            UPDATE INVESTOR_MASTER 
                            SET SOURCE_ID = V_MAIN_AGENT_CODE,
                                BRANCH_CODE = P_BRANCH_CD,
                                RM_CODE = P_RM_CD,
                                INV_CODE = NEW_INV_CODE 
                            WHERE INV_CODE = RS_DATA.INV_CODE;
                        END IF;
                        
                        CLOSE C_SIMILAR_INVESTOR;
                        
                        -- Update FP Investor tables
                        UPDATE FP_INVESTOR SET FAMILYHEAD_CODE = NEW_INV_CODE WHERE FAMILYHEAD_CODE = RS_DATA.INV_CODE;
                        UPDATE FP_INVESTOR SET FAM_MEM1 = REPLACE(FAM_MEM1, RS_DATA.INV_CODE, NEW_INV_CODE) WHERE FAMILYHEAD_CODE LIKE SUBSTR(RS_DATA.INV_CODE, 1, 8) || '%' OR FAMILYHEAD_CODE LIKE SUBSTR(NEW_INV_CODE, 1, 8) || '%';
                        UPDATE FP_INVESTOR SET FAM_MEM2 = REPLACE(FAM_MEM2, RS_DATA.INV_CODE, NEW_INV_CODE) WHERE FAMILYHEAD_CODE LIKE SUBSTR(RS_DATA.INV_CODE, 1, 8) || '%' OR FAMILYHEAD_CODE LIKE SUBSTR(NEW_INV_CODE, 1, 8) || '%';
                        UPDATE FP_INVESTOR SET FAM_MEM3 = REPLACE(FAM_MEM3, RS_DATA.INV_CODE, NEW_INV_CODE) WHERE FAMILYHEAD_CODE LIKE SUBSTR(RS_DATA.INV_CODE, 1, 8) || '%' OR FAMILYHEAD_CODE LIKE SUBSTR(NEW_INV_CODE, 1, 8) || '%';
                        
                        -- Update Transaction tables
                        UPDATE TRANSACTION_ST SET CLIENT_CODE = NEW_INV_CODE,BRANCH_CODE = P_BRANCH_CD,SOURCE_CODE = V_MAIN_AGENT_CODE,RMCODE = P_RM_CD,MODIFY_TALISMA = SYSDATE WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRANSACTION_MF_TEMP1 SET CLIENT_CODE = NEW_INV_CODE,BRANCH_CODE = P_BRANCH_CD,SOURCE_CODE = V_MAIN_AGENT_CODE,RMCODE = P_RM_CD WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRANSACTION_ST@mf.bajajcapital SET CLIENT_CODE = NEW_INV_CODE,BRANCH_CODE = P_BRANCH_CD,SOURCE_CODE = V_MAIN_AGENT_CODE,RMCODE = P_RM_CD WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRANSACTION_STTEMP SET CLIENT_CODE = NEW_INV_CODE,BRANCH_CODE = P_BRANCH_CD,SOURCE_CODE = V_MAIN_AGENT_CODE,RMCODE = P_RM_CD,MODIFY_TALISMA = SYSDATE WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE REDEM@mf.bajajcapital SET CLIENT_CODE = NEW_INV_CODE,BRANCH_CODE = P_BRANCH_CD,SOURCE_CODE = V_MAIN_AGENT_CODE,RMCODE = P_RM_CD WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE INVESTOR_FOLIO@mf.bajajcapital SET INVESTOR_CODE = NEW_INV_CODE WHERE INVESTOR_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE INVESTOR_MASTER_IPO SET INV_CODE = NEW_INV_CODE,AGENT_CODE = V_MAIN_AGENT_CODE WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE REVERTAL_TRANSACTION SET CLIENT_CODE = NEW_INV_CODE,BRANCH_CODE = P_BRANCH_CD,SOURCE_CODE = V_MAIN_AGENT_CODE,RMCODE = P_RM_CD WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRANSACTION_IPO SET INV_CODE = NEW_INV_CODE,AGENT_CODE = V_MAIN_AGENT_CODE WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRAN_PAYOUT@mf.bajajcapital SET INV_CODE = NEW_INV_CODE WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE BAJAJ_AR_HEAD SET CLIENT_CD = NEW_INV_CODE,MODIFY_TALISMA = SYSDATE WHERE CLIENT_CD = RS_DATA.INV_CODE;
                        
                        UPDATE TRAN_NET_BALANCE6@mf.bajajcapital SET CLIENT_CODE = NEW_INV_CODE WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRAN_IPO SET 
                            INV_CODE = NEW_INV_CODE,
                            CLIENT_CODE = V_MAIN_AGENT_CODE 
                        WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE TRAN_LEAD SET 
                            INV_CODE = NEW_INV_CODE 
                        WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE LEADS.LEAD_DETAIL SET 
                            INV_CODE = NEW_INV_CODE 
                        WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE PORT_TRANSACTION_ST@mf.bajajcapital SET 
                            CLIENT_CODE = NEW_INV_CODE,
                            BRANCH_CODE = P_BRANCH_CD,
                            SOURCE_CODE = V_MAIN_AGENT_CODE,
                            RMCODE = P_RM_CD 
                        WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE ONLINE_TRANSACTION_ST SET 
                            CLIENT_CODE = NEW_INV_CODE,
                            BRANCH_CODE = P_BRANCH_CD,
                            SOURCE_CODE = V_MAIN_AGENT_CODE,
                            RMCODE = P_RM_CD 
                        WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        -- History of updations (investor wise)
                        INSERT INTO INV_DEL_HIST_AGENT_MERGE (INV_CODE, NEW_INV_CODE, UPDATEON, UPDATEDBY) 
                        VALUES (RS_DATA.INV_CODE, NEW_INV_CODE, SYSDATE, P_LOGIN_ID);
                        
                        -- Online Just Trade updates
                        UPDATE TRANSACTION_ST_ONLINE SET 
                            CLIENT_CODE = NEW_INV_CODE 
                        WHERE CLIENT_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE ONLINE_CLIENT_REQUEST SET 
                            INV_CODE = NEW_INV_CODE 
                        WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE ONLINE_CLIENT_REQUEST_HIST SET 
                            INV_CODE = NEW_INV_CODE 
                        WHERE INV_CODE = RS_DATA.INV_CODE;
                        
                        UPDATE ONLINE_BUSINESS_SUMMARY SET 
                            CLIENT_CODEWM = NEW_INV_CODE 
                        WHERE CLIENT_CODEWM = RS_DATA.INV_CODE;
                        
                        UPDATE OFFLINE_BUSINESS_SUMMARY SET 
                            CLIENT_CODEWM = NEW_INV_CODE 
                        WHERE CLIENT_CODEWM = RS_DATA.INV_CODE;
                        
                        -- If investor was merged into existing one, delete original record
                        IF V_SIMILAR_INV_CODE IS NOT NULL AND V_SIMILAR_INV_CODE != RS_DATA.INV_CODE THEN
                            INSERT INTO CLIENT_INV_MERGE_LOG VALUES(NEW_INV_CODE, RS_DATA.INV_CODE, P_LOGIN_ID, SYSDATE);
                            INSERT INTO INVESTOR_DEL SELECT * FROM INVESTOR_MASTER WHERE INV_CODE = RS_DATA.INV_CODE;
                            DELETE FROM INVESTOR_MASTER WHERE INV_CODE = RS_DATA.INV_CODE;
                            DELETE FROM INVESTOR_MASTER@mf.bajajcapital WHERE INV_CODE = RS_DATA.INV_CODE;
                        END IF;
                        
                    EXCEPTION
                        WHEN OTHERS THEN
                            IF C_SIMILAR_INVESTOR%ISOPEN THEN
                                CLOSE C_SIMILAR_INVESTOR;
                            END IF;
                            RAISE;
                    END;
                END LOOP; -- End investor loop
                
                -- Update main investor master with branch and RM codes
                UPDATE INVESTOR_MASTER SET BRANCH_CODE = P_BRANCH_CD,RM_CODE = P_RM_CD,MODIFY_DATE = SYSDATE WHERE SOURCE_ID = V_MAIN_AGENT_CODE;
                
                -- Agent MERGING - Update main agent master
                UPDATE AGENT_MASTER SET SOURCEID = P_BRANCH_CD,RM_CODE = P_RM_CD,MODIFY_DATE = SYSDATE WHERE AGENT_CODE = V_MAIN_AGENT_CODE;
                
                -- Update transaction records with new agent codes
                UPDATE TRANSACTION_ST SET BRANCH_CODE = P_BRANCH_CD,RMCODE = P_RM_CD,MODIFY_TALISMA = SYSDATE WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE TRANSACTION_MF_TEMP1 SET BRANCH_CODE = P_BRANCH_CD,RMCODE = P_RM_CD WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE TRANSACTION_ST@mf.bajajcapital SET BRANCH_CODE = P_BRANCH_CD,RMCODE = P_RM_CD WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE PORT_TRANSACTION_ST@mf.bajajcapital SET BRANCH_CODE = P_BRANCH_CD,RMCODE = P_RM_CD WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE TRANSACTION_STTEMP SET BRANCH_CODE = P_BRANCH_CD,RMCODE = P_RM_CD,MODIFY_TALISMA = SYSDATE WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE REDEM SET BRANCH_CODE = P_BRANCH_CD,RMCODE = P_RM_CD WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE PAYMENT_DETAIL SET AGENT_CODE = V_MAIN_AGENT_CODE WHERE AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE LEDGER SET AGENT_CODE = V_MAIN_AGENT_CODE WHERE AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                
                -- Updates by Vinay Hatwal
                UPDATE UPFRONT_PAID SET CLIENT_AGENT_CODE = V_MAIN_AGENT_CODE WHERE CLIENT_AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE ADD_INCENTIVE_PAID SET CLIENT_AGENT_CODE = V_MAIN_AGENT_CODE WHERE CLIENT_AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE SIP_BROKER_BILLING1 SET SOURCE_CODE = V_MAIN_AGENT_CODE WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                UPDATE STP_BROKER_BILLING1 SET SOURCE_CODE = V_MAIN_AGENT_CODE WHERE SOURCE_CODE = MERGED_AGENT.AGENT_CODE;
                
                -- Paid ANA Subscription table
                UPDATE ADVISORSUBENTRY SET ANACODE = V_MAIN_AGENT_CODE WHERE ANACODE = MERGED_AGENT.AGENT_CODE;
                
                -- Update main agent details with merged agent's information
                UPDATE AGENT_MASTER MAIN
                SET (
                    PHONE, EMAIL, MOBILE, PINCODE, CITY_ID, DOB, EXIST_CODE, 
                    TDS, INTRODUCER, JOININGDATE, LAST_TRAN_DT1
                ) = (
                    SELECT 
                        CASE WHEN (MAIN.PHONE IS NULL OR TRIM(MAIN.PHONE) = '') AND M.PHONE IS NOT NULL THEN M.PHONE ELSE MAIN.PHONE END,
                        CASE WHEN MAIN.EMAIL IS NULL AND M.EMAIL IS NOT NULL THEN M.EMAIL ELSE MAIN.EMAIL END,
                        CASE WHEN MAIN.MOBILE IS NULL AND (M.MOBILE IS NOT NULL AND M.MOBILE != '0') THEN M.MOBILE ELSE MAIN.MOBILE END,
                        CASE WHEN MAIN.PINCODE IS NULL AND M.PINCODE IS NOT NULL THEN M.PINCODE ELSE MAIN.PINCODE END,
                        CASE WHEN MAIN.CITY_ID IS NULL AND M.CITY_ID IS NOT NULL THEN M.CITY_ID ELSE MAIN.CITY_ID END,
                        CASE WHEN MAIN.DOB IS NULL AND M.DOB IS NOT NULL THEN M.DOB ELSE MAIN.DOB END,
                        CASE WHEN MAIN.EXIST_CODE IS NULL AND M.EXIST_CODE IS NOT NULL THEN M.EXIST_CODE ELSE MAIN.EXIST_CODE END,
                        CASE WHEN MAIN.TDS IS NULL AND M.TDS IS NOT NULL THEN M.TDS ELSE MAIN.TDS END,
                        CASE WHEN MAIN.INTRODUCER IS NULL AND M.INTRODUCER IS NOT NULL THEN M.INTRODUCER ELSE MAIN.INTRODUCER END,
                        CASE 
                            WHEN MAIN.JOININGDATE IS NULL AND M.JOININGDATE IS NOT NULL THEN M.JOININGDATE
                            WHEN MAIN.JOININGDATE IS NOT NULL AND M.JOININGDATE IS NOT NULL AND M.JOININGDATE < MAIN.JOININGDATE THEN M.JOININGDATE
                            ELSE MAIN.JOININGDATE 
                        END,
                        CASE 
                            WHEN MAIN.LAST_TRAN_DT1 IS NULL AND M.LAST_TRAN_DT1 IS NOT NULL THEN M.LAST_TRAN_DT1
                            WHEN MAIN.LAST_TRAN_DT1 IS NOT NULL AND M.LAST_TRAN_DT1 IS NOT NULL AND M.LAST_TRAN_DT1 > MAIN.LAST_TRAN_DT1 THEN M.LAST_TRAN_DT1
                            ELSE MAIN.LAST_TRAN_DT1 
                        END
                    FROM AGENT_MASTER M
                    WHERE M.AGENT_CODE = MERGED_AGENT.AGENT_CODE
                )
                WHERE MAIN.AGENT_CODE = V_MAIN_AGENT_CODE;
                
                -- Log agent merge
                INSERT INTO CLIENT_INV_MERGE_LOG VALUES(V_MAIN_AGENT_CODE, MERGED_AGENT.AGENT_CODE, P_LOGIN_ID, SYSDATE);
                
                -- Archive and delete merged agent
                INSERT INTO AGENT_DEL SELECT * FROM AGENT_MASTER WHERE AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                DELETE FROM AGENT_MASTER WHERE AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                DELETE FROM AGENT_MASTER@mf.bajajcapital WHERE AGENT_CODE = MERGED_AGENT.AGENT_CODE;
                
                -- History of updations (agent wise) - By Vinay Hatwal
                INSERT INTO AGENT_DEL_HIST_AGENT_MERGE (AGENT_CODE, NEW_AGENT_CODE, UPDATEON, UPDATEDBY) 
                VALUES (MERGED_AGENT.AGENT_CODE, V_MAIN_AGENT_CODE, SYSDATE, P_LOGIN_ID);
                
                -- Handle FP Investor consolidation
                DECLARE
                    V_FAM_HEAD VARCHAR2(100);
                    V_MEMBERS1 VARCHAR2(4000);
                BEGIN
                    SELECT FAMILYHEAD_CODE, FAM_MEM1 INTO V_FAM_HEAD, V_MEMBERS1
                    FROM (
                        SELECT FAMILYHEAD_CODE, FAM_MEM1
                        FROM FP_INVESTOR 
                        WHERE SUBSTR(FAMILYHEAD_CODE, 1, 8) = V_MAIN_AGENT_CODE 
                        AND (FP_TYPE = 'Snapshot' OR FP_TYPE = 'Comprehensive') 
                        ORDER BY FAMILYHEAD_CODE DESC
                    ) WHERE ROWNUM = 1;
                    
                    IF V_FAM_HEAD IS NOT NULL THEN
                        INSERT INTO DUP_FP_INVESTOR SELECT * FROM FP_INVESTOR WHERE FAMILYHEAD_CODE = V_FAM_HEAD;
                        
                        UPDATE FP_INVESTOR 
                        SET FAM_MEM1 = FAM_MEM1 || '#' || V_MEMBERS1 
                        WHERE SUBSTR(FAMILYHEAD_CODE, 1, 8) = V_MAIN_AGENT_CODE 
                        AND (FP_TYPE = 'Snapshot' OR FP_TYPE = 'Comprehensive');
                    END IF;
                    
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        NULL; -- No FP records found, continue
                END;
                
                COMMIT; -- Commit transaction for this agent
                
                P_SUCCESS := 'SUCCESS';
                
            
            END LOOP; -- End merged agent loop
            
        END;
    ELSE     
        BEGIN
            FOR MAIN IN(SELECT * FROM WEALTHMAKER.INVESTOR_MASTER WHERE INV_CODE=P_MAIN_CODE)
            LOOP
                FOR MERGED IN  (SELECT * FROM WEALTHMAKER.INVESTOR_MASTER WHERE INV_CODE IN(SELECT * FROM TABLE(ANKIT_SPLIT(P_TO_MERGE_CODE,'#')))) 
                LOOP
                  BEGIN
                    UPDATE WEALTHMAKER.TRANSACTION_ST  SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8),MODIFY_TALISMA=SYSDATE WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE WEALTHMAKER.TRANSACTION_MF_TEMP1 SET   CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8) WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE TRANSACTION_ST@mf.bajajcapital SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8) WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE TRANSACTION_STTEMP SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8),MODIFY_TALISMA=SYSDATE WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE INVESTOR_FOLIO@mf.bajajcapital SET INVESTOR_CODE=P_MAIN_CODE WHERE INVESTOR_CODE=MERGED.INV_CODE;
                    UPDATE INVESTOR_MASTER_IPO SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE REVERTAL_TRANSACTION SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8)WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE TRANSACTION_IPO SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE TRAN_PAYOUT@mf.bajajcapital SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE BAJAJ_AR_HEAD SET CLIENT_CD=P_MAIN_CODE,MODIFY_TALISMA=SYSDATE WHERE CLIENT_CD=MERGED.INV_CODE;
                    UPDATE TRAN_NET_BALANCE6@mf.bajajcapital SET CLIENT_CODE=P_MAIN_CODE WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE REDEM@mf.bajajcapital SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8) WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE TRAN_IPO SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE TRAN_LEAD SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;                         
                    UPDATE TB_DOC_UPLOAD SET   INV_CODE=P_MAIN_CODE WHERE INV_CODE=TO_CHAR(MERGED.INV_CODE);
                    UPDATE WEALTHMAKER.CLIENT_VOUCHER_DETAILS SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE PORT_TRANSACTION_ST@mf.bajajcapital  SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8) WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE PORTFOLIO_TRANS@mf.bajajcapital SET CLIENT_CODE=P_MAIN_CODE,SOURCE_CODE=SUBSTR(P_MAIN_CODE,0,8) WHERE CLIENT_CODE=MERGED.INV_CODE;
                                          
                    UPDATE FP_INVESTOR SET FAMILYHEAD_CODE=P_MAIN_CODE WHERE FAMILYHEAD_CODE=MERGED.INV_CODE;
                    UPDATE FP_INVESTOR SET FAM_MEM1=REPLACE(FAM_MEM1,MERGED.INV_CODE,P_MAIN_CODE) WHERE FAMILYHEAD_CODE LIKE ''|| MERGED.INV_CODE ||'%' OR FAMILYHEAD_CODE LIKE ''||P_MAIN_CODE ||'%';
                    UPDATE FP_INVESTOR SET FAM_MEM2=REPLACE(FAM_MEM2,MERGED.INV_CODE,P_MAIN_CODE) WHERE FAMILYHEAD_CODE LIKE ''|| MERGED.INV_CODE ||'%' OR FAMILYHEAD_CODE LIKE ''||P_MAIN_CODE ||'%';
                    UPDATE FP_INVESTOR SET FAM_MEM3=REPLACE(FAM_MEM3,MERGED.INV_CODE,P_MAIN_CODE) WHERE FAMILYHEAD_CODE LIKE ''|| MERGED.INV_CODE ||'%' OR FAMILYHEAD_CODE LIKE ''||P_MAIN_CODE ||'%';
                                
                    UPDATE TRANSACTION_ST_ONLINE    SET CLIENT_CODE=P_MAIN_CODE WHERE CLIENT_CODE=MERGED.INV_CODE;
                    UPDATE ONLINE_CLIENT_REQUEST    SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE ONLINE_CLIENT_REQUEST_HIST    SET INV_CODE=P_MAIN_CODE WHERE INV_CODE=MERGED.INV_CODE;
                    UPDATE ONLINE_BUSINESS_SUMMARY  SET CLIENT_CODEWM=P_MAIN_CODE WHERE CLIENT_CODEWM=MERGED.INV_CODE;
                    UPDATE OFFLINE_BUSINESS_SUMMARY SET CLIENT_CODEWM=P_MAIN_CODE WHERE CLIENT_CODEWM=MERGED.INV_CODE;
                    UPDATE ADVISORSUBENTRY SET CLIENT_CODE=P_MAIN_CODE WHERE CLIENT_CODE=MERGED.INV_CODE;
                    
                     IF MAIN.FAMILY_HEAD IS NULL AND MERGED.FAMILY_HEAD IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET FAMILY_HEAD = MERGED.FAMILY_HEAD WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.IMPLEMENTED_DATE IS NULL AND MERGED.IMPLEMENTED_DATE IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET IMPLEMENTED_DATE = MERGED.IMPLEMENTED_DATE WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.PHONE IS NULL AND MERGED.PHONE IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET PHONE = MERGED.PHONE WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.EMAIL IS NULL AND MERGED.EMAIL IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET EMAIL = MERGED.EMAIL WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.MOBILE IS NULL AND (MERGED.MOBILE IS NOT NULL AND MERGED.MOBILE <>0) THEN
                     
                        UPDATE INVESTOR_MASTER SET MOBILE = MERGED.MOBILE WHERE INV_CODE=MERGED.INV_CODE;
                        
                     ELSIF (MERGED.MOBILE IS NOT NULL AND MERGED.MOBILE <>0)  THEN                         
                        
                         UPDATE INVESTOR_MASTER SET PHONE = MAIN.PHONE ||','|| MERGED.MOBILE  WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.PINCODE IS NULL AND MERGED.PINCODE IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET PINCODE = MERGED.PINCODE WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.INVESTOR_CODE IS NULL AND MERGED.INVESTOR_CODE IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET INVESTOR_CODE = MERGED.INVESTOR_CODE WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MAIN.PAN IS NULL AND MERGED.PAN IS NOT NULL THEN
                     
                        UPDATE INVESTOR_MASTER SET PAN = MERGED.PAN WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;
                     
                     IF MERGED.MANDATE_FLAG='Y' THEN
                     
                        UPDATE INVESTOR_MASTER SET MANDATE_FLAG = MERGED.MANDATE_FLAG WHERE INV_CODE=MERGED.INV_CODE;
                        
                     END IF;                     
                     
                     UPDATE INVESTOR_MASTER SET MODIFY_DATE = SYSDATE WHERE INV_CODE=MERGED.INV_CODE;
                     
                     INSERT INTO CLIENT_INV_MERGE_LOG VALUES(P_MAIN_CODE,MERGED.INV_CODE,P_LOGIN_ID,SYSDATE);
                     INSERT INTO INVESTOR_DEL SELECT * FROM INVESTOR_MASTER  WHERE INV_CODE=MERGED.INV_CODE;
                     DELETE FROM INVESTOR_MASTER  WHERE INV_CODE=MERGED.INV_CODE;
                     INSERT INTO ACCOUNT_MERGE_LOG VALUES(P_MAIN_CODE,MERGED.INV_CODE,P_LOGIN_ID,SYSDATE);
            
                     SELECT CLIENT_CODE INTO MYCLIENTCODE FROM CLIENT_TEST WHERE CLIENT_CODEKYC=TO_NUMBER(MERGED.INV_CODE);
                     SELECT MAIN_CODE INTO MYMAINCODE  FROM CLIENT_TEST WHERE CLIENT_CODEKYC=TO_NUMBER(MERGED.INV_CODE);
                     SELECT CLIENT_CODE INTO MYNEWCLIENTCODE FROM CLIENT_TEST WHERE CLIENT_CODEKYC=TO_NUMBER(P_MAIN_CODE);
                     SELECT MAIN_CODE INTO MYNEWMAINCODE FROM CLIENT_TEST WHERE CLIENT_CODEKYC=TO_NUMBER(P_MAIN_CODE);
                     
                     IF LENGTH(MYCLIENTCODE)>2 AND MYCLIENTCODE is not null THEN
                     -- if account exists for the merged investor(Which is not possible)
                        IF TRIM(MAIN.PAN)='' OR MAIN.PAN IS NULL THEN
                        
                           IF  TRIM(MERGED.PAN)<>'' AND  MERGED.PAN IS NOT NULL THEN
                           
                               UPDATE CLIENT_TEST SET CLIENT_PAN=MERGED.PAN WHERE CLIENT_CODEKYC=P_MAIN_CODE AND CLIENT_PAN IS NULL;
                           
                           END IF;

                        END IF;

                     
                     END IF;
            
                    UPDATE INVESTOR_FOLIO@mf.bajajcapital SET CLIENT_AH_CODE= MYNEWCLIENTCODE ,UPD_FLAG1='MRG',CLIENT_MAIN_CODE=MYNEWMAINCODE WHERE CLIENT_AH_CODE= MYCLIENTCODE AND CLIENT_MAIN_CODE= MYMAINCODE ;
                    DELETE FROM CLIENT_TEST  WHERE CLIENT_CODEKYC=MERGED.INV_CODE;
                    DELETE FROM INVESTOR_MASTER@mf.bajajcapital  WHERE INV_CODE=MERGED.INV_CODE;
                    
                    P_SUCCESS := 'SUCCESS';
                  EXCEPTION WHEN OTHERS THEN
                    RAISE_APPLICATION_ERROR(-20001, SQLERRM(SQLCODE));
                  END;    
                   DBMS_OUTPUT.PUT_LINE(P_SUCCESS || ' : ' || MERGED.INV_CODE);                   
                END LOOP;
            END LOOP;
            
        EXCEPTION WHEN OTHERS THEN
            P_ERRORMESSAGE := SUBSTR(SQLERRM(SQLCODE),1,4000);
            RETURN;
        END;  
   
   END IF;
END;
/