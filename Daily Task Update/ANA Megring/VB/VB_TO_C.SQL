CREATE OR REPLACE PROCEDURE MERGE_INVESTORS(
    P_MSFGMAIN_CODE         IN VARCHAR2,
    P_MSFGMERGEDINVESTORS   IN TABLE_OF_VARCHAR2
) AS
    V_NEW_INV_CODE          VARCHAR2(50);
    V_MCOUNT                NUMBER := 0;
    V_FLAG                  BOOLEAN := TRUE;
    V_BRANCH_CD             VARCHAR2(50);
    V_RM_CD                 VARCHAR2(50);
    V_FAM_HEAD              VARCHAR2(50);
    V_MEMBERS1              VARCHAR2(50);
    V_MEMBERS2              VARCHAR2(50);
    V_MEMBERS3              VARCHAR2(50);

    Members                 VARCHAR2(100);
    Fam_Head                VARCHAR2(100);


BEGIN
    -- Initialize variables
    V_BRANCH_CD := '';
    V_RM_CD := '';
    
    -- Begin transaction
    BEGIN
        -- Loop through each merged investor code
        FOR I IN 1..P_MSFGMERGEDINVESTORS.COUNT LOOP
            -- Find RM and Branch Code (Assuming this is done by a function)
            -- Find_RM(v_branch_cd, v_Rm_cd);
            
            -- Process each investor
            FOR REC IN (SELECT INV_CODE, INVESTOR_NAME FROM INVESTOR_MASTER WHERE SOURCE_ID = P_MSFGMERGEDINVESTORS(I)) LOOP
                
                -- Check if investor exists in the main code
                BEGIN
                    SELECT INV_CODE INTO V_NEW_INV_CODE FROM INVESTOR_MASTER WHERE SOURCE_ID = P_MSFGMAIN_CODE AND UPPER(REPLACE(REPLACE(INVESTOR_NAME, '.', ''), ' ', '')) LIKE '%' || UPPER(REPLACE(REPLACE(REC.INVESTOR_NAME, '.', ''), ' ', '')) || '%' AND INSTR(UPPER(INVESTOR_NAME), 'HUF') = 0;
                EXCEPTION
                    WHEN NO_DATA_FOUND THEN
                        V_MCOUNT := V_MCOUNT + 1;
                        IF V_MCOUNT >= 999 THEN
                            V_NEW_INV_CODE := P_MSFGMAIN_CODE || LPAD(V_MCOUNT, 5, '0');
                        ELSE
                            V_NEW_INV_CODE := P_MSFGMAIN_CODE || LPAD(V_MCOUNT, 3, '0');
                        END IF;
                        
                        -- Update investor_master
                        UPDATE INVESTOR_MASTER SET SOURCE_ID = P_MSFGMAIN_CODE, BRANCH_CODE = V_BRANCH_CD, RM_CODE = V_RM_CD, INV_CODE = V_NEW_INV_CODE WHERE INV_CODE = REC.INV_CODE;
                END;
                
                -- Update related tables
                UPDATE FP_INVESTOR SET FAMILYHEAD_CODE = V_NEW_INV_CODE WHERE FAMILYHEAD_CODE = REC.INV_CODE;
                UPDATE FP_INVESTOR SET FAM_MEM1 = REPLACE(FAM_MEM1, REC.INV_CODE, V_NEW_INV_CODE) WHERE FAMILYHEAD_CODE LIKE SUBSTR(REC.INV_CODE, 1, 8) || '%' OR FAMILYHEAD_CODE LIKE SUBSTR(V_NEW_INV_CODE, 1, 8) || '%';
                UPDATE FP_INVESTOR SET FAM_MEM2 = REPLACE(FAM_MEM2, REC.INV_CODE, V_NEW_INV_CODE) WHERE FAMILYHEAD_CODE LIKE SUBSTR(REC.INV_CODE, 1, 8) || '%' OR FAMILYHEAD_CODE LIKE SUBSTR(V_NEW_INV_CODE, 1, 8) || '%';
                UPDATE FP_INVESTOR SET FAM_MEM3 = REPLACE(FAM_MEM3, REC.INV_CODE, V_NEW_INV_CODE) WHERE FAMILYHEAD_CODE LIKE SUBSTR(REC.INV_CODE, 1, 8) || '%' OR FAMILYHEAD_CODE LIKE SUBSTR(V_NEW_INV_CODE, 1, 8) || '%';
                
                -- Update transaction tables
                UPDATE TRANSACTION_ST SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD, MODIFY_TALISMA = SYSDATE WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE TRANSACTION_MF_TEMP1 SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE TRANSACTION_ST@mf.bajajcapital SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE TRANSACTION_STTEMP SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD, modify_TALISMA = sysdate WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE REDEM@mf.bajajcapital SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE INVESTOR_FOLIO@mf.bajajcapital SET INVESTOR_CODE = V_NEW_INV_CODE WHERE INVESTOR_CODE = REC.INV_CODE;
                UPDATE INVESTOR_MASTER_IPO SET INV_CODE = V_NEW_INV_CODE, AGENT_CODE = P_MSFGMAIN_CODE  WHERE INV_CODE = REC.INV_CODE;
                UPDATE REVERTAL_TRANSACTION SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE TRANSACTION_IPO SET INV_CODE = V_NEW_INV_CODE, AGENT_CODE = P_MSFGMAIN_CODE WHERE INV_CODE = REC.INV_CODE;
                UPDATE TRAN_PAYOUT@mf.bajajcapital SET INV_CODE = V_NEW_INV_CODE WHERE INV_CODE = REC.INV_CODE;
                UPDATE BAJAJ_AR_HEAD SET CLIENT_CODE = V_NEW_INV_CODE, modify_TALISMA = SYSDATE WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE TRAN_NET_BALANCE6@mf.bajajcapital SET CLIENT_CODE = V_NEW_INV_CODE  WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE TRAN_IPO SET INV_CODE = V_NEW_INV_CODE, CLIENT_CODE= P_MSFGMAIN_CODE WHERE INV_CODE = REC.INV_CODE;
                UPDATE TRAN_LEAD SET INV_CODE = V_NEW_INV_CODE  WHERE INV_CODE = REC.INV_CODE;
                UPDATE LEADS.LEAD_DETAIL SET INV_CODE = V_NEW_INV_CODE  WHERE INV_CODE = REC.INV_CODE;
                UPDATE port_TRANSACTION_ST@mf.bajajcapital SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD WHERE CLIENT_CODE = REC.INV_CODE;
                UPDATE online_transaction_st SET CLIENT_CODE = V_NEW_INV_CODE, BRANCH_CODE = V_BRANCH_CD, SOURCE_CODE = P_MSFGMAIN_CODE, RMCODE = V_RM_CD WHERE CLIENT_CODE = REC.INV_CODE;

                
                -- Insert into history table
                INSERT INTO INV_DEL_HIST_AGENT_MERGE (INV_CODE, NEW_INV_CODE, UPDATEON, UPDATEDBY) VALUES (REC.INV_CODE, V_NEW_INV_CODE, SYSDATE, USER);
                
                'Online Just Trade'
                update transaction_st_online    set client_code=V_NEW_INV_CODE      where client_code=REC.INV_CODE
                update online_client_request    set inv_code=V_NEW_INV_CODE         where inv_code=REC.INV_CODE
                update online_client_request_hist  set inv_code=V_NEW_INV_CODE      where inv_code=REC.INV_CODE
                update online_business_summary  set client_codewm=V_NEW_INV_CODE    where client_codewm=REC.INV_CODE
                update offline_business_summary set client_codewm=V_NEW_INV_CODE    where client_codewm=REC.INV_CODE

                -- Delete old records if necessary
                IF V_NEW_INV_CODE IS NOT NULL THEN
                    INSERT INTO CLIENT_INV_MERGE_LOG VALUES (V_NEW_INV_CODE, REC.INV_CODE, USER, SYSDATE);
                    Insert into INVESTOR_del select * from INVESTOR_MASTER  where inv_code = REC.INV_CODE
                    DELETE FROM INVESTOR_MASTER WHERE INV_CODE = REC.INV_CODE;
                    DELETE FROM INVESTOR_MASTER@mf.bajajcapital WHERE INV_CODE = REC.INV_CODE;
                END IF;
            END LOOP;
            
            UPDATE INVESTOR_MASTER SET  BRANCH_CODE = V_BRANCH_CD, RM_CODE = V_RM_CD, MODIFY_DATE = SYSDATE WHERE SOURCE_ID = P_MSFGMAIN_CODE,;
            UPDATE AGENT_MASTER SET SOURCEID = V_BRANCH_CD, RM_CODE = V_RM_CD, MODIFY_DATE = SYSDATE WHERE AGENT_CODE = P_MSFGMAIN_CODE;
            

            update TRANSACTION_ST            set   branch_code= V_BRANCH_CD ,rmcode= V_RM_CD, modify_TALISMA= SYSDATE where source_code = P_MSFGMAIN_CODE
            update TRANSACTION_MF_TEMP1      set   branch_code= V_BRANCH_CD ,rmcode= V_RM_CD  where source_code = P_MSFGMAIN_CODE
            update TRANSACTION_ST@mf.bajajcapital            set   branch_code=V_BRANCH_CD, rmcode = V_RM_CD where source_code = P_MSFGMAIN_CODE
            update port_TRANSACTION_ST@mf.bajajcapital            set   branch_code=V_BRANCH_CD, rmcode=V_RM_CD where source_code=P_MSFGMAIN_CODE
            update TRANSACTION_STTEMP        set   branch_code=V_BRANCH_CD, rmcode=V_RM_CD, modify_TALISMA=SYSDATE where source_code = P_MSFGMAIN_CODE
            update REDEM                     set   branch_code=V_BRANCH_CD, rmcode=V_RM_CD  where source_code = P_MSFGMAIN_CODE
         

            update PAYMENT_DETAIL            set agent_code = P_MSFGMAIN_CODE where agent_code=P_MSFGMERGEDINVESTORS(I)
            update LEDGER                    set AGENT_code = P_MSFGMAIN_CODE where AGENT_code=P_MSFGMERGEDINVESTORS(I)
            

            -- By Vinay Hatwal
            update upfront_paid set client_agent_code=P_MSFGMAIN_CODE where client_agent_code=P_MSFGMERGEDINVESTORS(I)
            update ADD_INCENTIVE_PAID set client_agent_code=P_MSFGMAIN_CODE where client_agent_code=P_MSFGMERGEDINVESTORS(I)
            update SIP_BROKER_BILLING1 set SOURCE_CODE=P_MSFGMAIN_CODE where SOURCE_CODE=P_MSFGMERGEDINVESTORS(I)
            update STP_BROKER_BILLING1 set SOURCE_CODE=P_MSFGMAIN_CODE where SOURCE_CODE=P_MSFGMERGEDINVESTORS(I)
            
            
            -- Paid ANA Subscription table
            update ADVISORSUBENTRY set anacode=P_MSFGMAIN_CODE where anacode=P_MSFGMERGEDINVESTORS(I)
            

            FOR rsClient IN (Select * from agent_master where agent_code = P_MSFGMAIN_CODE) LOOP
                FOR rsclient1 (Select * from agent_master where agent_code=P_MSFGMERGEDINVESTORS(I) LOOP

                IF rsClient.phone = null or rsClient.phone = '' and rsClient1.phone IS NOT NULL THEN
                    rsClient.phone = rsclient1.phone
                END IF ;

                IF rsClient.EMAIL = null   and rsClient1.EMAIL IS NOT NULL THEN
                    rsClient.EMAIL = rsclient1.EMAIL
                END IF ;

                IF rsClient.MOBILE = null AND rsClient1.MOBILE IS NOT NULL THEN
                    rsClient.MOBILE = rsClient1.MOBILE
                END IF;

                If rsClient.PINCODE = NULL AND rsclient1.PINCODE  IS NOT NULL Then
                rsClient.PINCODE = rsclient1.PINCODE
                End If
                
                If rsClient.CITY_ID = NULL AND rsclient1.CITY_ID  IS NOT NULL Then
                    rsClient.CITY_ID = rsclient1.CITY_ID
                End If
                If rsClient.DOB = NULL AND rsclient1.DOB  IS NOT NULL Then
                    rsClient.DOB = rsclient1.DOB
                End If
                If rsClient.EXIST_CODE = NULL AND rsclient1.EXIST_CODE  IS NOT NULL Then
                    rsClient.EXIST_CODE = rsclient1.EXIST_CODE
                End If

                If rsClient.TDS = NULL AND rsclient1.TDS  IS NOT NULL Then
                    rsClient.TDS = rsclient1.TDS
                End If
                If rsClient.INTRODUCER = NULL AND rsclient1.INTRODUCER  IS NOT NULL Then
                    rsClient.INTRODUCER = rsclient1.INTRODUCER
                End If
                
                If rsclient1.JOININGDATE  IS NOT NULL Then
                    -- If CheckDate(Format(rsClient.JOININGDATE, "dd/mm/yyyy, Format(rsclient1.JOININGDATE, "dd/mm/yyyy) = False Then
                        rsClient.JOININGDATE = rsclient1.creation_date
                    -- End If
                End If
                
                If rsClient.JOININGDATE = NULL AND rsclient1.JOININGDATE  IS NOT NULL Then
                    rsClient.JOININGDATE = rsclient1.JOININGDATE
                End If
                
                If rsclient1.LAST_TRAN_DT1  IS NOT NULL Then
                    -- If CheckDate(Format(rsclient1.LAST_TRAN_DT1, "dd/mm/yyyy, Format(rsClient.LAST_TRAN_DT1, "dd/mm/yyyy) = False Then
                        rsClient.LAST_TRAN_DT1 = rsclient1.LAST_TRAN_DT1
                    -- End If
                End If
                
                If rsClient.LAST_TRAN_DT1 IS NULL AND rsclient1.LAST_TRAN_DT1  IS NOT NULL Then
                    rsClient.LAST_TRAN_DT1 = rsclient1.LAST_TRAN_DT1
                End If

                END LOOP
            END LOOP


            insert into client_inv_merge_log values(P_MSFGMAIN_CODE,P_MSFGMERGEDINVESTORS(I),Glbloginid,sysdate);
            insert into agent_del select * from agent_master where agent_code=P_MSFGMERGEDINVESTORS(I);
            Delete from agent_master where agent_code = P_MSFGMERGEDINVESTORS(I);
            Delete from agent_master@mf.bajajcapital where agent_code=P_MSFGMERGEDINVESTORS(I);
             
            -- Insert into history table
            INSERT INTO AGENT_DEL_HIST_AGENT_MERGE (AGENT_CODE, NEW_AGENT_CODE, UPDATEON, UPDATEDBY)
            VALUES (P_MSFGMERGEDINVESTORS(I), P_MSFGMAIN_CODE, SYSDATE, Glbloginid);
        

            Select NVL(familyhead_code, NULL), NVL(fam_mem1, NULL ) INTO Fam_Head, Members1
            from fp_investor where substr(familyhead_code,1,8)= P_MSFGMAIN_CODE and (fp_type='Snapshot' or Fp_type='Comprehensive') order by familyhead_code desc;

            IF Fam_Head IS NOT NULL AND Members1 IS NOT NULL THEN 
                insert into dup_fp_investor select * from fp_investor where familyhead_code= Fam_Head
                update fp_investor set fam_mem1=fam_mem1||'#'||Members1 
                where substr(familyhead_code,1,8)=P_MSFGMAIN_CODE and (fp_type='Snapshot' or Fp_type='Comprehensive');
        
        END LOOP;
        
        -- Commit transaction
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            -- Rollback transaction on error
            ROLLBACK;
            RAISE_APPLICATION_ERROR(-20001, 'Error occurred: ' || SQLERRM);
    END;
END MERGE_INVESTORS;
/